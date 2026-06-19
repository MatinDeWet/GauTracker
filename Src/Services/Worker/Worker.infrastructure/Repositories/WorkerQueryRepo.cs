using Repository.Contracts;
using Repository.Implementation;
using Shared.Persistence.Data.Contexts;

namespace Worker.infrastructure.Repositories;

/// <summary>
/// Unsecured query repository for the worker. Background jobs run with no current-user identity, so
/// they read through this (no row-level security / locks) rather than the WebApi's secured repos.
/// </summary>
internal sealed class WorkerQueryRepo(GauContext context) : QueryRepo<GauContext>(context), IQueryRepo;
