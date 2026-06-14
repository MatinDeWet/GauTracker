# Entity Framework Core Migrations

All commands should be run from the **solution root** (`/GauTracker`).

- **Startup project**: `Src/Services/WebApi/WebApi.Presentation`
- **Migrations project**: `Src/Services/WebApi/WebApi.infrastructure`
- **DbContext**: `GauContext`

---

## Common Commands

### Add a new migration

```bash
dotnet ef migrations add <MigrationName> \
  --project Src/Services/WebApi/WebApi.infrastructure \
  --startup-project Src/Services/WebApi/WebApi.Presentation
```

### Remove the last migration

```bash
dotnet ef migrations remove \
  --project Src/Services/WebApi/WebApi.infrastructure \
  --startup-project Src/Services/WebApi/WebApi.Presentation
```

### Apply migrations to the database

```bash
dotnet ef database update \
  --project Src/Services/WebApi/WebApi.infrastructure \
  --startup-project Src/Services/WebApi/WebApi.Presentation
```

### Apply migrations up to a specific migration

```bash
dotnet ef database update <MigrationName> \
  --project Src/Services/WebApi/WebApi.infrastructure \
  --startup-project Src/Services/WebApi/WebApi.Presentation
```

### Rollback to a previous migration

```bash
dotnet ef database update <PreviousMigrationName> \
  --project Src/Services/WebApi/WebApi.infrastructure \
  --startup-project Src/Services/WebApi/WebApi.Presentation
```

> To rollback all migrations, use `0` as the migration name.

### List all migrations

```bash
dotnet ef migrations list \
  --project Src/Services/WebApi/WebApi.infrastructure \
  --startup-project Src/Services/WebApi/WebApi.Presentation
```

### Generate a SQL script for migrations

```bash
dotnet ef migrations script \
  --project Src/Services/WebApi/WebApi.infrastructure \
  --startup-project Src/Services/WebApi/WebApi.Presentation \
  --output migrations.sql
```

### Drop the database

```bash
dotnet ef database drop \
  --project Src/Services/WebApi/WebApi.infrastructure \
  --startup-project Src/Services/WebApi/WebApi.Presentation
```

---

## Notes

- Migrations are stored in `Src/Services/WebApi/WebApi.infrastructure/Data/Migrations/GauMigrations/`.
- The migrations history table is stored in the `migrations` schema (`migrations.__EFMigrationsHistory`).
- Make sure `dotnet-ef` is installed globally: `dotnet tool install --global dotnet-ef`
- Migrations are automatically applied on startup via `app.ApplyDatabaseMigrationsAsync()` in `Program.cs`.
