﻿using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace ICanHasDotnetCore.Tests.Web.Helpers
{
    public class FakeConfigurationRoot : Dictionary<string, string>, IConfigurationRoot
    {
       
        public IConfigurationSection GetSection(string key)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IConfigurationSection> GetChildren()
        {
            throw new System.NotImplementedException();
        }

        public IChangeToken GetReloadToken()
        {
            throw new System.NotImplementedException();
        }

        public void Reload()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<IConfigurationProvider> Providers => throw new System.NotImplementedException();
    }
}