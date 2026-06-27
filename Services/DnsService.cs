using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HostForge.Services
{
    internal class DnsService
    {
        [DllImport("dnsapi.dll", EntryPoint = "DnsFlushResolverCache")]
        private static extern uint DnsFlushResolverCache();
        public bool FlushCache()
        {
            return DnsFlushResolverCache() == 1;
        }
    }
}
