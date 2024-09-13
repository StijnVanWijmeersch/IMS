using FluentAssertions;
using IMS.Application.Abstractions;
using IMS.Infrastructure.Concretes;
using Microsoft.EntityFrameworkCore;
using NetArchTest.Rules;
using System.Reflection;

namespace ArchitectureTest.Infrastructure
{
    public class InfrastructureTests
    {
        private static readonly Assembly _InfrastructureAssembly = typeof(UnitOfWork).Assembly;

        [Fact]
        public void Configurations_Should_BeSealed()
        {
            var testResult = Types.InAssembly(_InfrastructureAssembly)
                .That()
                .ImplementInterface(typeof(IEntityTypeConfiguration<>))
                .Should()
                .BeSealed()
                .GetResult();

            testResult.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Configuration_Should_HaveNameEndingWith_Configuration()
        {
            var testResult = Types.InAssembly(_InfrastructureAssembly)
                .That()
                .ImplementInterface(typeof(IEntityTypeConfiguration<>))
                .Should()
                .HaveNameEndingWith("Configuration")
                .GetResult();

            testResult.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Infrastructure_Should_HaveDependencyOnApplicationOnly()
        {
            var testResultIsNotDependent = Types.InAssembly(_InfrastructureAssembly)
                .Should()
                .NotHaveDependencyOnAll(["StockManagement.API", "StockManagement.Domain"])
                .GetResult();

            testResultIsNotDependent.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Interceptors_Should_HaveNameEndingWith_Interceptor()
        {
            var testResult = Types.InAssembly(_InfrastructureAssembly)
                .That()
                .ResideInNamespace("StockManagement.Infrastructure.Interceptors")
                .And().AreClasses()
                .Should().HaveNameEndingWith("Interceptor")
                .And().BeSealed()
                .GetResult();

            testResult.IsSuccessful.Should().BeTrue();
        }

        [Fact]
        public void Repositories_Should_HaveNameEndingWith_Respository_And_Should_BeSealed()
        {
            var TestResult = Types.InAssembly(_InfrastructureAssembly)
                .That()
                .ImplementInterface(typeof(IBaseRepository<>))
                .And().AreClasses()
                .Should().HaveNameEndingWith("Repository")
                .And().BePublic()
                .And().BeSealed()
                .GetResult();

            TestResult.IsSuccessful.Should().BeTrue();
        }
    }
}
