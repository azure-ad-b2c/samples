using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace AADMagicLinks
{
    using AADMagicLinks.Models;
    using AADMagicLinks.Utility;
    using Azure.Identity;
    using Microsoft.AspNetCore.Authentication.OpenIdConnect;
    using Microsoft.Extensions.Azure;
    using Microsoft.Extensions.Primitives;
    using Microsoft.Identity.Web;

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
            // Configuration to sign-in users with Azure AD B2C
            services
                .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(options => {
                    Configuration.Bind("AzureAdB2C", options);
                    options.Events ??= new OpenIdConnectEvents();
                    options.Events.OnRedirectToIdentityProvider += async context =>
                    {
                        if (context.HttpContext.Request.Query.TryGetValue("id_token_hint", out StringValues idTokenHintValues))
                        {
                            var idTokenHint = idTokenHintValues.FirstOrDefault();   //If there's more than one token hint, one has to be chosen...
                            if (string.IsNullOrWhiteSpace(idTokenHint) == false)
                            {
                                // If a token hint was provided in the original request, pass it along to the IdP request.
                                context.ProtocolMessage.IdTokenHint = idTokenHint;
                            }
                        }
                        await Task.CompletedTask.ConfigureAwait(false);
                    };
                });

            services.AddOptions();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "PMVInvite", Version = "v1"});
            });
            services.Configure<AppSettingsModel>(Configuration.GetSection("AppSettings"));
            services.AddAzureClients(builder =>
            {
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PMVInvite v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // Add the Microsoft Identity Web cookie policy
            app.UseCookiePolicy();
            // Add the ASP.NET Core authentication service
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}