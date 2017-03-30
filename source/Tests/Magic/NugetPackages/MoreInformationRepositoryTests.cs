﻿using FluentAssertions;
using ICanHasDotnetCore.NugetPackages;
using Xunit;

namespace ICanHasDotnetCore.Tests.Magic.NugetPackages
{
    public class MoreInformationRepositoryTests
    {
        [Fact]
        public void EntriesCanBeReadAndAtLeastOneEntryExists()
        {
            MoreInformationRepository.All.Should().NotBeEmpty();
        }
    }
}