using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Net.Http;
using Microsoft.AspNetCore.Components;
using MecWise.Blazor.Common;
using MecWise.Blazor.Components;
using Microsoft.JSInterop;
using System.IO;

using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace MecWise.Blazor.Host
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

            //[27/05/2021, PhyoZin] Added for Azure AD (OpenIDConnect)
            if (Configuration["AzureAdSSO"].ToBool()) {
                
                services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
                .AddMicrosoftIdentityWebApp(Configuration.GetSection("AzureAd"));

                services.AddControllersWithViews(options =>
                {
                    var policy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .Build();
                    options.Filters.Add(new AuthorizeFilter(policy));
                });
                services.AddRazorPages().AddMicrosoftIdentityUI();
            }

            if (Configuration["RunOnServer"].ToBool())
            {
                services.AddServerSideBlazor();
                services.AddDevExpressBlazor();
            }
            
            services.AddRazorPages();

            services.AddScoped<HttpClient>(s =>
            {
                var navigationManager = s.GetRequiredService<NavigationManager>();
                return new HttpClient
                {
                    BaseAddress = new Uri(navigationManager.BaseUri)
                };
            });

            services.AddScoped<SessionState>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();


            
            app.UseStaticFiles();

            //app.UseHttpsRedirection();

            app.UseRouting();

            //[27/05/2021, PhyoZin] Added for Azure AD (OpenIDConnect)
            if (Configuration["AzureAdSSO"].ToBool()) {
                app.UseAuthentication();
                app.UseAuthorization();
            }
            
            app.UseEndpoints(endpoints => 
            {
                if (Configuration["RunOnServer"].ToBool()) {
                    endpoints.MapBlazorHub();
                }
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToPage("/_Host");
            });

                        
        }


    }
}
