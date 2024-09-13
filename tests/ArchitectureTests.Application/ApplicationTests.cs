using FluentAssertions;
using IMS.Application.Abstractions;
using MediatR;
using NetArchTest.Rules;
using System.Reflection;

namespace ArchitectureTests.Application;

public class ApplicationTests
{
    private static Assembly _applicationAssembly = typeof(IUnitOfWork).Assembly;

    [Fact] 
    public void ApplicationBehaviours_Should_HaveNameEndingWithBehaviour()
    {
        var testResult = Types.InAssembly(_applicationAssembly)
            .That()
            .ResideInNamespace("StockManagement.Application.Behaviours")
            .Should()
            .HaveNameEndingWith("Behaviour")
            .GetResult();

        testResult.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Handlers_Should_HaveNameEndingWith_Handler()
    {
        var testResult = Types.InAssembly(_applicationAssembly)
            .That()
            .ImplementInterface(typeof(IRequestHandler<,>))
            .Should()
            .HaveNameEndingWith("Handler")
            .GetResult();

        testResult.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void TypeRespositories_Should_StartWith_I_And_EndWith_Respository()
    {
        var testResults = Types.InAssembly(_applicationAssembly)
            .That()
            .ImplementInterface(typeof(IBaseRepository<>))
            .Should()
            .BeInterfaces()
            .And()
            .HaveNameStartingWith("I")
            .And()
            .HaveNameEndingWith("Repository")
            .GetResult();

        testResults.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void CommandsAndQueries_Should_BeSealed()
    {
        var testResult = Types.InAssembly(_applicationAssembly)
            .That()
            .ImplementInterface(typeof(IRequest<>))
            .Should()
            .BeSealed()
            .GetResult();

        testResult.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void EventHadlers_Should_HaveNameEndingWith_EventHandler()
    {
        var testResult = Types.InAssembly(_applicationAssembly)
            .That()
            .ImplementInterface(typeof(INotificationHandler<>))
            .Should()
            .HaveNameEndingWith("EventHandler")
            .GetResult();

        testResult.IsSuccessful.Should().BeTrue();
    }   
}
