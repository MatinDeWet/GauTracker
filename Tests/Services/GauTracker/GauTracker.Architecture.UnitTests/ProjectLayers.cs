using System.Reflection;
using GauTracker.API;
using GauTracker.Application;
using GauTracker.Domain;
using GauTracker.Infrastructure;

namespace GauTracker.Architecture.UnitTests;

public abstract class ProjectLayers
{
    protected static readonly Assembly DomainAssembly = typeof(IDomainPointer).Assembly;
    protected static readonly Assembly ApplicationAssembly = typeof(IApplicationPointer).Assembly;
    protected static readonly Assembly InfrastructureAssembly = typeof(IInfrastructurePointer).Assembly;
    protected static readonly Assembly PresentationAssembly = typeof(IApiPointer).Assembly;
}
