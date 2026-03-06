using EventFlowInspector.Api;
using EventFlowInspector.Application;
using EventFlowInspector.Infrastructure.Messaging;
using EventFlowInspector.Infrastructure.Worker;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<KafkaOptions>(builder.Configuration.GetSection(KafkaOptions.SectionName));
builder.Services.Configure<ListenerOptions>(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IMessageGeneratorService, MessageGeneratorService>();
builder.Services.AddSingleton<IMessageTraceService, MessageTraceService>();
builder.Services.AddSingleton<IMessageSearchService, MessageSearchService>();
builder.Services.AddSingleton<KafkaProducer>();
builder.Services.AddSingleton<KafkaConsumer>();
builder.Services.AddSingleton<IMessagePublisherService, MessagePublisherService>();

builder.Services.AddSingleton<EventListenerWorker>();
builder.Services.AddSingleton<IListenerReadiness>(sp => sp.GetRequiredService<EventListenerWorker>());
builder.Services.AddHostedService(sp => sp.GetRequiredService<EventListenerWorker>());

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

var api = app.MapGroup(string.Empty);
api.MapSampleEndpoints();
api.MapPublishEndpoints();
api.MapTraceEndpoints();
api.MapSearchEndpoints();

app.Run();
