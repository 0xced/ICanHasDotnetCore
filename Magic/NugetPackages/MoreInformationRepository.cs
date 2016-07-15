﻿using System.Collections.Generic;
using System.IO;
using ICanHasDotnetCore.Plumbing;
using Newtonsoft.Json;
using System.Linq;

namespace ICanHasDotnetCore.NugetPackages
{
    public class MoreInformationRepository
    {
        static MoreInformationRepository()
        {
            using (var s = typeof(KnownReplacementsRepository).Assembly.GetManifestResourceStream($"Magic.NugetPackages.Data.KnownReplacements.json"))
            using (var sr = new StreamReader(s))
            {
                All = JsonConvert.DeserializeObject<MoreInformation[]>(sr.ReadToEnd())
                    .OrderBy(r => r.StartsWith) // false first
                    .ToArray();
            }
        }

        public static readonly IReadOnlyList<MoreInformation> All;

        public static Option<MoreInformation> Get(string id) => All.FirstOrNone(k => k.AppliesTo(id));
    }
}