using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace masschat
{
    public class Configuration
    {
        private IConfiguration AppSettingsConfig { get; set; }

        private static Configuration instance;

        public string Nick { get; }
        public string Password { get; }
        public string ClientId { get; }

        public static Configuration Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Configuration();
                }
                return instance;
            }
        }


        private Configuration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory() + "\\settings\\")
                .AddJsonFile("appsettings.json");

            AppSettingsConfig = builder.Build();

            Nick = AppSettingsConfig["nick"];
            Password = AppSettingsConfig["password"];
            ClientId = AppSettingsConfig["clientid"];

        }

    }
}
