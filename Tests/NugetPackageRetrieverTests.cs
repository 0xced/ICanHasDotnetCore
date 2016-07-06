﻿using FluentAssertions;
using ICanHasDotnetCore.NugetPackages;
using NUnit.Framework;

namespace ICanHasDotnetCore.Tests
{
    public class NugetPackageRetrieverTests
    {
        [Test]
        public void DependenciesCanBeParsedForSerilogSinkSeq()
        {
            var retriever = GetRetriever();
            var pkg = retriever.Retrieve("Serilog.Sinks.Seq", true).Result;
            pkg.Id.Should().Be("Serilog.Sinks.Seq");
            pkg.Dependencies.Should().BeEquivalentTo("Serilog.Sinks.RollingFile", "Serilog", "Serilog.Sinks.PeriodicBatching");
        }

        [Test]
        public void DependenciesCanBeParsedForAutofac()
        {
            var retriever = GetRetriever();
            var pkg = retriever.Retrieve("Autofac", true).Result;
            pkg.Id.Should().Be("Autofac");
            pkg.Dependencies.Should().BeEquivalentTo();
        }

        private NugetPackageInfoRetriever GetRetriever()
        {
            return new NugetPackageInfoRetriever(new PackageRepositoryWrapper());
        }

        [Test]
        public void CompatiblePackageCanBeIdentified()
        {
            var retriever = GetRetriever();
            var pkg = retriever.Retrieve("Serilog.Sinks.Seq", true).Result;
            pkg.SupportType.Should().Be(SupportType.Supported);
        }

        [Test]
        public void IncompatiblePackageCanBeIdentified()
        {
            var retriever = GetRetriever();
            var pkg = retriever.Retrieve("BootstrapMvcHelpers", true).Result;
            pkg.SupportType.Should().Be(SupportType.Unsupported);
        }

        [Test]
        public void NonExistantPackageShouldBeNotFound()
        {
            var retriever = GetRetriever();
            var pkg = retriever.Retrieve("FooFooFoo", true).Result;
            pkg.SupportType.Should().Be(SupportType.NotFound);
        }
    }
}