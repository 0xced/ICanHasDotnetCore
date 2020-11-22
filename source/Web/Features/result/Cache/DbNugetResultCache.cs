using System;
using System.Threading;
using System.Threading.Tasks;
using ICanHasDotnetCore.NugetPackages;
using ICanHasDotnetCore.Plumbing;
using ICanHasDotnetCore.Web.Database;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging.Core;
using Serilog;

namespace ICanHasDotnetCore.Web.Features.result.Cache
{
    public class DbNugetResultCache : INugetResultCache
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public DbNugetResultCache(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<Option<NugetPackage>> GetAsync(PackageIdentity identity, CancellationToken cancellationToken)
        {
            try
            {
                await using var context = _contextFactory.CreateDbContext();
                // Finding a entity with a value converter on its primary key is tricky, see https://github.com/aspnet/EntityFrameworkCore/issues/14180
                var package = await context.NugetResultCache.FindAsync(new object[] {identity.Id, identity.Version.Some()}, cancellationToken);
                return package?.Some() ?? package.None();
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Could not retrieve {id} {version} in Nuget Package Cache", identity.Id, identity.Version);
                return Option<NugetPackage>.ToNone;
            }
        }

        public async Task StoreAsync(NugetPackage package, CancellationToken cancellationToken)
        {
            try
            {
                if (package.Version.None)
                    return;

                await using var context = _contextFactory.CreateDbContext();
                context.NugetResultCache.Add(package);
                await context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Log.Warning(ex, "Could not store {id} {version} in Nuget Package Cache", package.Id, package.Version.Value);
            }
        }
    }
}
