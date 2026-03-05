using Confluent.Kafka;
using EventFlowInspector.Application;
using EventFlowInspector.Domain.Models;
using EventFlowInspector.Infrastructure.Messaging;
using Microsoft.Extensions.Options;

namespace EventFlowInspector.Infrastructure.Worker;

public sealed class EventListenerWorker(
    KafkaConsumer consumerFactory,
    IMessageTraceService traceService,
    IOptions<ListenerOptions> listenerOptions,
    IOptions<KafkaOptions> kafkaOptions,
    ILogger<EventListenerWorker> logger) : BackgroundService, IListenerReadiness
{
    private readonly TaskCompletionSource _ready = new(TaskCreationOptions.RunContinuationsAsynchronously);
    private readonly TimeSpan _readyTimeout = TimeSpan.FromMinutes(listenerOptions.Value.ListenerReadyTimeoutMinutes);
    private readonly KafkaOptions _kafkaOptions = kafkaOptions.Value;

    public bool IsReady => _ready.Task.IsCompletedSuccessfully;

    public async Task WaitUntilReadyAsync(CancellationToken cancellationToken)
    {
        using var timeoutCts = new CancellationTokenSource(_readyTimeout);
        using var linked = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

        try
        {
            await _ready.Task.WaitAsync(linked.Token);
        }
        catch (OperationCanceledException)
        {
            throw new TimeoutException("Listener readiness timeout exceeded.");
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var queues = Enum.GetValues<KafkaQueue>();
        var consumers = new List<IConsumer<string, string>>();

        foreach (var queue in queues)
        {
            var queueName = queue.ToString();
            var topic = _kafkaOptions.ResolveTopic(queueName);
            var consumerGroup = $"{_kafkaOptions.ConsumerGroupPrefix}-{queueName.ToLowerInvariant()}";
            var consumer = consumerFactory.Build(consumerGroup);
            consumer.Subscribe(topic);
            consumers.Add(consumer);
            logger.LogInformation("Subscribed queue {Queue} to topic {Topic} with group {ConsumerGroup}", queueName, topic, consumerGroup);
        }

        _ready.TrySetResult();
        logger.LogInformation("Kafka listeners ready for queues: {Queues}", string.Join(", ", queues.Select(q => q.ToString())));

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                foreach (var consumer in consumers)
                {
                    try
                    {
                        var consumed = consumer.Consume(TimeSpan.FromMilliseconds(100));
                        if (consumed is null)
                        {
                            continue;
                        }

                        var trace = KafkaConsumer.ToTrace(consumed.Topic, consumed);
                        traceService.AddTrace(trace);
                    }
                    catch (ConsumeException ex)
                    {
                        logger.LogError(ex, "Consume error.");
                    }
                }

                await Task.Delay(50, stoppingToken);
            }
        }
        finally
        {
            foreach (var consumer in consumers)
            {
                consumer.Close();
                consumer.Dispose();
            }
        }
    }
}
