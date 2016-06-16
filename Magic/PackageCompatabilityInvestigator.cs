﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ICanHasDotnetCore.Magic.NugetPackages;
using Serilog;

namespace ICanHasDotnetCore.Magic
{
    public class PackageCompatabilityInvestigator
    {
        private readonly PackagesFileReader _packagesFileReader;
        private readonly NugetPackageInfoRetriever _nugetPackageInfoRetriever;
        private readonly ConcurrentDictionary<string, Task<PackageResult>> _results = new ConcurrentDictionary<string, Task<PackageResult>>();
        public PackageCompatabilityInvestigator(PackagesFileReader packagesFileReader, NugetPackageInfoRetriever nugetPackageInfoRetriever)
        {
            _packagesFileReader = packagesFileReader;
            _nugetPackageInfoRetriever = nugetPackageInfoRetriever;
        }

        public static PackageCompatabilityInvestigator Create()
        {
           var repository = new PackageRepositoryWrapper();
            return new PackageCompatabilityInvestigator(
                new PackagesFileReader(),
                new NugetPackageInfoRetriever(
                    repository
                    )
                );
        }

        public async Task<InvestigationResult> Go(IReadOnlyList<PackagesFileData> files)
        {
            var results = files.Select(Process).ToArray();
            return new InvestigationResult(await Task.WhenAll(results));
        }

        private async Task<PackageResult> Process(PackagesFileData file)
        {
            try
            {
                var dependencies = _packagesFileReader.ReadDependencies(file.Contents);
                var dependencyResults = await GetDependencyResults(dependencies);
                return PackageResult.Success(file.Name, dependencyResults, SupportType.InvestigationTarget);
            }
            catch (Exception ex)
            {
                return PackageResult.Failed(file.Name, "An error occured - " + ex.Message);
            }
        }

        private async Task<IReadOnlyList<PackageResult>> GetDependencyResults(IReadOnlyList<string> dependencies)
        {
            var tasks = dependencies.Select(d => _results.GetOrAdd(d, GetPackageAndDependencies));
            return await Task.WhenAll(tasks);
        }


        private async Task<PackageResult> GetPackageAndDependencies(string id)
        {
            try
            {
                var package = await _nugetPackageInfoRetriever.Retrieve(id);
                if (package.WasFailure)
                    return PackageResult.Failed(id, package.ErrorString);

                var dependencyResults = await GetDependencyResults(package.Value.Dependencies);
                return PackageResult.Success(id, dependencyResults, package.Value.SupportType);
            }
            catch (Exception ex)
            {
                return PackageResult.Failed(id, "An error occured - " + ex.Message);
            }
        }
    }
}