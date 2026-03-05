When generating or reviewing code, follow these principles.

PRIMARY GOALS
- Clean code
- SOLID principles
- Dependency inversion
- Simple architecture
- Testability
- Easy unit testing

ARCHITECTURE

Use a simple layered structure:

src/
  core/
    models/
    interfaces/

  services/

  infrastructure/

  api/ or controllers/

RULES

1. Keep architecture simple. Avoid unnecessary patterns.

2. Apply SOLID principles.

3. Use Dependency Inversion.

High-level modules must depend on interfaces, not implementations.

Example:

interface UserRepository {
  save(user: User): Promise<void>
  findById(id: string): Promise<User | null>
}

4. Inject dependencies via constructors.

Example:

class UserService {
  constructor(private userRepository: UserRepository) {}
}

5. Avoid tight coupling with frameworks or databases.

Business logic should not depend directly on external libraries.

6. Keep classes small and focused.

Prefer small services instead of large "God classes".

7. Write unit-testable code.

Avoid global state, static dependencies, or hidden side effects.

8. Separate business logic from I/O logic.

Example:

Service -> business rules  
Controller -> HTTP handling

9. Prefer pure functions when possible.

10. Provide unit tests for services.

Example test:

describe("UserService", () => {
  it("creates a user", async () => {}
})