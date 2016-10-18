﻿using System.Linq;
using System.Text;
using FluentAssertions;
using ICanHasDotnetCore.SourcePackageFileReaders;
using NUnit.Framework;

namespace ICanHasDotnetCore.Tests.Magic.SourcePackageFileReaders
{
    [TestFixture]
    public class PaketDependenciesReaderTests : ReaderTestsBase
    {
        protected override string Contents => @"source https://nuget.org/api/v2

// NuGet packages
nuget NUnit ~> 2.6.3
nuget 
    FAKE 
        ~> 
            3.4
nuget DotNetZip >= 1.9
nuget SourceLink.Fake

// Files from GitHub repositories
github forki/FsUnit FsUnit.fs

// Gist files
gist Thorium/1972349 timestamp.fs

// HTTP resources
http http://www.fssnip.net/1n decrypt.fs";


        protected override void Execute(byte[] encodedFile)
        {
            var result = new PaketDependenciesReader().ReadDependencies(encodedFile);
            result.Should().BeEquivalentTo("NUnit", "FAKE", "DotNetZip", "SourceLink.Fake");
        }

    }
}