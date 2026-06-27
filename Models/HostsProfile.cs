using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HostForge.Models
{
    class HostsProfile
    {
        public string Name { get; set; }
        public string Content { get; set; }

        public HostsProfile()
        {
        }

        public HostsProfile(string name, string content)
        {
            Name = name;
            Content = content;
        }
    }
}
