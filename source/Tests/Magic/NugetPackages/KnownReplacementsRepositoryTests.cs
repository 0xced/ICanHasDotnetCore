﻿using FluentAssertions;
using ICanHasDotnetCore.NugetPackages;
using Xunit;

namespace ICanHasDotnetCore.Tests.Magic.NugetPackages
{
    public class KnownReplacementsRepositoryTests
    {
        [Fact]
        public void EntriesCanBeReadAndAtLeastOneEntryExists()
        {
            KnownReplacementsRepository.All.Should().NotBeEmpty();
        }
    }
}