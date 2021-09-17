using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using AADB2C.SignInWithEmailUsingKeyVault.Models;
using AADB2C.SignInWithEmailUsingKeyVault.Utility;
using Azure.Identity;

namespace AADB2C.SignInWithEmailUsingKeyVault
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.Configure<AppSettingsModel>(Configuration.GetSection("AppSettings"));

            var azureCredential = new DefaultAzureCredential(new DefaultAzureCredentialOptions
            {
                ////// This is only to accomodate local machine shennanigans
                ////ExcludeManagedIdentityCredential = true,
            });

            services.AddSingleton<ITokenValidationConfigurationProvider>(s =>
            {
                var settings = s.GetService<IOptions<AppSettingsModel>>();
                var tokenValidationConfigurationProvider = new LocalCertFromKeyVaultTokenValidationConfigurationProvider(new Uri(settings.Value.VaultUrl), settings.Value.CertificateName, azureCredential);
                return tokenValidationConfigurationProvider;
            });

            services.AddSingleton<ITokenProvider>(s =>
            {
                var settings = s.GetService<IOptions<AppSettingsModel>>();
                var tokenProvider = new RemoteCertFromKeyVaultTokenProvider(new Uri(settings.Value.VaultUrl), settings.Value.CertificateName, azureCredential);
                ////var tokenProvider = new LocalCertFromKeyVaultTokenProvider(new Uri(settings.Value.VaultUrl), settings.Value.CertificateName, azureCredential);
                return tokenProvider;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
