# Qwiq Code Style and Conventions

## C# Style

### General Settings

- **Indentation**: 4 spaces
- **Line Endings**: CRLF (Windows)
- **Nullable**: Enabled project-wide
- **Implicit Usings**: Enabled

### Naming Conventions

- **Classes/Methods/Properties**: PascalCase
- **Private fields**: \_camelCase with underscore prefix
- **Parameters/Locals**: camelCase
- **Test methods**: Given_When_Then pattern (underscores allowed)

### Null Handling

Use runtime checks, not JetBrains annotations:

```csharp
public void Method(SomeType param)
{
    if (param == null) throw new ArgumentNullException(nameof(param));
}
```

### Factory Pattern

All stores created via factories, never direct construction:

```csharp
IWorkItemStore store = WorkItemStoreFactory.Default.Create(options);
```

## Test Conventions

### Test Pattern (ContextSpecification)

```csharp
[TestClass]
public class Given_context : ContextSpecification
{
    public override void Given() { /* Arrange */ }
    public override void When() { /* Act */ }

    [TestMethod]
    public void Then_behavior() { /* Assert with Shouldly */ }
}
```

### Test Categories

- No category: Standard unit tests
- `localOnly`: Requires local environment
- `Benchmark`: Performance benchmarks
- `SOAP`: Requires SOAP server
- `REST`: Requires REST server
- `IntegrationTests`: Full integration tests

## Markdown Style

- **Indentation**: 2 spaces
- **Max line length**: 120 characters (handled by prettier)
- **Heading style**: ATX (#)
- **Code blocks**: Fenced with language specified
- **Lists**: Ordered numbers for ordered lists

## YAML/JSON Style

- **Indentation**: 2 spaces
- **Style**: space-based indentation

## Analyzer Suppressions (Technical Debt)

| Rule   | Reason                                           |
| ------ | ------------------------------------------------ |
| CS1591 | ~4200 missing XML docs - tracked separately      |
| CS0618 | TimeZone used in public APIs (breaking change)   |
| CA1707 | Test classes use Given_When_Then naming          |
| CA1716 | When() method in test pattern                    |
| CA1822 | Instance methods kept for API compatibility      |
| CA1859 | Interface types used for abstraction/testability |
| CA1863 | CompositeFormat requires .NET 8+ (multi-target)  |

## Package Management

**Central Package Management** is enabled:

- All package versions go in `Directory.Packages.props`
- Individual csproj files reference packages WITHOUT versions
- Use `<PackageReference Include="..." />` (no Version attribute)

## Commit Conventions

Use conventional commits:

- `fix(scope): description` - Bug fixes
- `feat(scope): description` - New features
- `refactor(scope): description` - Code refactoring
- `docs(scope): description` - Documentation
- `test(scope): description` - Test changes
- `chore(scope): description` - Maintenance tasks
