using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

            services.AddAzureClients(builder =>
            {
                // This is only used to accomodate my own local machine shennanigans
                ////var azureCredential = new DefaultAzureCredential(new DefaultAzureCredentialOptions { ExcludeManagedIdentityCredential = true });
                var azureCredential = new DefaultAzureCredential();
                builder.UseCredential(azureCredential);
                builder.AddCertificateClient(Configuration.GetSection("KeyVault"));

                // Use the CryptographyClientHelper to create instances of the CryptographyClient using the AzureCredential established here
                // once the URI for the certificate key is known/available
                services.AddSingleton<CryptographyClientFactory>(s =>
                {
                    return new CryptographyClientFactory(azureCredential);
                });
            });

            // Singleton to read the certificate once from KV and use/share it throughout the application lifetime.
            services.AddSingleton<KeyVaultCertificateHelper>();

            services.AddScoped<IEmailSender, SendGridApiEmailSender>();
            //services.AddScoped<IEmailSender, SmtpClientMailSender>();
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
