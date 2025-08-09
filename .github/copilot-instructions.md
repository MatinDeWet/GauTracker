# GitHub Copilot Instructions for GauTracker

## Code Style and Formatting

### General Formatting
- Use 4 spaces for indentation (no tabs)
- Use CRLF line endings
- Always insert a final newline at the end of files
- Preserve single-line blocks but avoid single-line statements

### C# Specific Guidelines

#### Namespace and Using Directives
- Use file-scoped namespaces: `namespace GauTracker.ProjectName;`
- Place using directives outside the namespace
- Sort system directives first
- Don't separate import directive groups

#### Variable Declarations
- Avoid `var` for built-in types: use `int`, `string`, `bool` instead of `var`
- Avoid `var` in general contexts unless type is apparent from assignment
- Use `var` only when the type is apparent: `var user = new User();`

#### Code Structure
- Use file-scoped namespaces
- Prefer top-level statements where appropriate
- Use primary constructors when suitable
- Don't use expression-bodied methods or constructors
- Use expression-bodied properties, indexers, accessors, operators, and lambdas

#### Pattern Matching and Modern C# Features
- Use pattern matching over `as` with null checks
- Use pattern matching over `is` with cast checks
- Prefer switch expressions over traditional switch statements
- Use null-conditional operators: `obj?.Property`
- Use null-coalescing operators: `value ?? defaultValue`

#### Collections and LINQ
- Use collection initializers: `new List<string> { "item1", "item2" }`
- Use object initializers: `new Person { Name = "John", Age = 30 }`
- Prefer simplified boolean expressions
- Use range and index operators when appropriate

#### Method and Property Preferences
- Always use braces for code blocks, even single statements
- Use simple using statements: `using var stream = ...`
- Prefer auto-properties over backing fields
- Use readonly fields where appropriate
- Make methods static when they don't access instance data

#### Null Handling
- Use conditional delegate calls: `handler?.Invoke()`
- Prefer `is null` checks over `== null`
- Use throw expressions where appropriate

#### Accessibility and Modifiers
- Always specify accessibility modifiers for non-interface members
- Follow modifier order: `public static readonly`
- Use `static` for local functions when possible

### Naming Conventions
- Use PascalCase for types, methods, properties, and events
- Interfaces should begin with 'I': `IUserService`
- Use PascalCase for public members
- Match folder structure with namespaces when possible

### Code Quality Rules
- Avoid catching general exception types without specific handling
- Don't expose generic lists directly in public APIs
- Use appropriate data types and avoid unnecessary boxing
- Implement standard exception constructors when creating custom exceptions
- Use LoggerMessage delegates for structured logging
- Prefer specific collection methods over LINQ extensions where available

### Spacing and Layout
- No space after casts: `(int)value`
- Space after keywords in control flow: `if (condition)`
- Space around binary operators: `a + b`
- Space after commas: `Method(param1, param2)`
- Space after colons in inheritance: `class Derived : Base`
- No space before dots: `object.Method()`
- No space before square brackets: `array[index]`

### Line Breaks
- Open braces on new lines for all constructs
- New line before `else`, `catch`, and `finally`
- Avoid line breaks in object/anonymous type initializers unless necessary

### Migration Files
- Migration files (in **/Migrations/*.cs) are excluded from most style rules
- Focus on functionality over style in migration files

## Project-Specific Guidelines
- This appears to be a .NET project called "GauTracker"
- Maintain consistency with the established codebase patterns
- Follow the error suppression patterns defined in the EditorConfig for external/generated code

## Error Handling
- Many CA (Code Analysis) rules are disabled - focus on functionality over strict analysis rules
- Nullable reference type warnings (CS8xxx) are disabled - but still write null-safe code when possible
- Some IDE suggestions are disabled - prioritize readability and maintainability