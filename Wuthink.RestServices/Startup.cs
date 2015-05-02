using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Wuthink.RestServices.Startup))]

namespace Wuthink.RestServices
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
