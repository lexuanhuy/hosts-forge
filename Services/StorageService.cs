using HostForge.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HostForge.Services
{
    internal class StorageService
    {
        private readonly string _systemHostPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), @"drivers\etc\hosts");
        private readonly string _configFilePath;

        public StorageService()
        {
            string appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "HostForge");
            if(!Directory.Exists(appDataFolder))
            {
                Directory.CreateDirectory(appDataFolder);
            }
            _configFilePath = Path.Combine(appDataFolder, "config.json");
        }

        public List<HostsProfile> LoadProfiles()
        {
            if(File.Exists(_configFilePath))
            {
                try
                {
                    string jsonString = File.ReadAllText(_configFilePath);
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    return JsonSerializer.Deserialize<List<HostsProfile>>(jsonString, options);
                }
                catch { }
            }
            return GetDefaultProfiles();
        }

        public void SaveProfiles(List<HostsProfile> profiles)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = JsonSerializer.Serialize(profiles, options);
            File.WriteAllText(_configFilePath, jsonString);
        }

        public void WriteToSystemHosts(string content)
        {
            File.WriteAllText(_systemHostPath, content);
        }

        private List<HostsProfile> GetDefaultProfiles()
        {
            string currentSystemContent = "";
            try
            {
                if (File.Exists(_systemHostPath))
                {
                    currentSystemContent = File.ReadAllText(_systemHostPath);
                }
            }
            catch { }

            // add profile default
            var defaults = new List<HostsProfile> {
                new HostsProfile("Default (System)", currentSystemContent)
            };

            SaveProfiles(defaults);
            return defaults;
        }
    }
}
