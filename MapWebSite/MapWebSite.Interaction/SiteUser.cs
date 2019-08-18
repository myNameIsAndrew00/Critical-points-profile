﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace MapWebSite.Interaction
{
    public class SiteIdentity : IIdentity
    {
        public string Name { get; set; }

        public string AuthenticationType => "default"; //modify here in the future

        public bool IsAuthenticated => true; //modify here in the future
    }

    public class SiteUser : IPrincipal
    {
        private readonly SiteIdentity identity = null;

        public IIdentity Identity => identity;

        public SiteUser() { }

        public SiteUser(SiteIdentity identity)
        {
            this.identity = identity;
        }

        public bool IsInRole(string role)
        {
            return true;
        }
    }
}
