using System.Reflection;
using Background.API;
using Background.Application;
using Background.Domain;
using Background.Infrastructure;

namespace Background.Architecture.UnitTests;

public abstract class ProjectLayers
{
    protected static readonly Assembly DomainAssembly = typeof(IDomainPointer).Assembly;
    protected static readonly Assembly ApplicationAssembly = typeof(IApplicationPointer).Assembly;
    protected static readonly Assembly InfrastructureAssembly = typeof(IInfrastructurePointer).Assembly;
    protected static readonly Assembly PresentationAssembly = typeof(IApiPointer).Assembly;
}
