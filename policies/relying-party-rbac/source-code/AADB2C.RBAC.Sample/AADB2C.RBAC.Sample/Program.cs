using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AADB2C.RBAC.Sample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)

                // Demo: The following code uses the JSON configuration provider
                .ConfigureAppConfiguration((webHostBuilderContext, configurationbuilder) =>
                {
                    configurationbuilder
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: false);

                    // Demo: Configuration sources are read in the order that they're specified. 
                    // The environment variables are read last. Any configuration values set through the environment 
                    // replace those set in the two previous providers. 
                    // You can use Azure App Sevices app settings to overwrite the settins in appsettings.json 
                    // in following format: AppSettings:Tenant, AppSettings:ClientId and AppSettings:ClientSecret
                    configurationbuilder.AddEnvironmentVariables();
                    configurationbuilder.Build();
                })
                .UseStartup<Startup>()
                .Build();
    }
}
