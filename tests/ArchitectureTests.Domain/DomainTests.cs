using FluentAssertions;
using IMS.Domain.Products;
using IMS.SharedKernel;
using NetArchTest.Rules;
using System.Reflection;

namespace ArchitectureTests.Domain;

public class DomainTests
{
    private static readonly Assembly _domainAssembly = typeof(Product).Assembly;

    [Fact]
    public void DomainEntities_Should_BeSealed()
    {
        var testResult = Types.InAssembly(_domainAssembly)
            .That()
            .Inherit(typeof(Entity))
            .Should()
            .BeSealed()
            .GetResult();

        testResult.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void DomainEntities_Should_HaveParameterlessPrivateConstructor()
    {
        var domainEntities = Types.InAssembly(_domainAssembly)
            .That()
            .Inherit(typeof(Entity))
            .GetTypes();

        var fails = new List<Type>();

        foreach (var type in domainEntities)
        {
            var ctor = type.GetConstructors(
                BindingFlags.NonPublic |
                BindingFlags.Instance);

            if (!ctor.Any(c => c.IsPrivate && c.GetParameters().Length == 0))
            {
                fails.Add(type);
            }
        }

        fails.Should().BeEmpty();
    }

    [Fact]
    public void DomainEntities_Should_HaveStaticCreateMethod()
    {
        var domainEntities = Types.InAssembly(_domainAssembly)
            .That()
            .Inherit(typeof(Entity))
            .GetTypes();

        var fails = new List<Type>();

        foreach (var type in domainEntities)
        {
            var createMethod = type.GetMethod("Create", 
                BindingFlags.Public |
                BindingFlags.Static);

            if (createMethod is null)
            {
                fails.Add(type);
            }
        }

        fails.Should().BeEmpty();
    }

    [Fact] 
    public void DomainEvent_Should_HaveNameEndingWith_Event()
    {
        var testResult = Types.InAssembly(_domainAssembly)
            .That()
            .ImplementInterface(typeof(IDomainEvent))
            .Should()
            .HaveNameEndingWith("Event")
            .GetResult();

        testResult.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void DomainEvents_Should_BeSealed()
    {
        var testResult = Types.InAssembly(_domainAssembly)
            .That()
            .ImplementInterface(typeof(IDomainEvent))
            .Should()
            .BePublic()
            .And()
            .BeSealed()
            .GetResult();

        testResult.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void DomainEvents_Should_BeRecords()
    {
        var domainEvents = Types.InAssembly(_domainAssembly)
            .That()
            .ImplementInterface(typeof(IDomainEvent))
            .GetTypes();

        var fails = new List<Type>();

        foreach (var type in domainEvents)
        {
            if (type.GetMethod("<Clone>$") is not object)
            {
                fails.Add(type);
            }
        }

        fails.Should().BeEmpty();
    }

    [Fact]
    public void Domain_Should_HaveNoDependency_OnAnyOtherLayer()
    {
        var testResult = Types.InAssembly(_domainAssembly)
            .Should()
            .NotHaveDependencyOnAny(
                    "StockManagement.API",
                    "StockManagement.Infrastructure",
                    "StockManagement.Application")
            .GetResult();

        testResult.IsSuccessful.Should().BeTrue();
    }
}
