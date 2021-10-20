﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.ExpressApp.Blazor.Services;
using DevExpress.Persistent.Base;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AgencyPortaXaf.Blazor.Server.Services;
using DevExpress.ExpressApp.Security;
using AgencyPortaXaf.Module.BusinessObjects;
using AgencyPortaXaf.Blazor.Server.Module;

namespace AgencyPortaXaf.Blazor.Server {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services) {

            services.AddXafSecurity(options =>
            {
                options.Events.OnSecurityStrategyCreated = securityStrategy =>
                {
                    ((SecurityStrategy)securityStrategy).AnonymousAllowedTypes.Add(typeof(Company));
                    ((SecurityStrategy)securityStrategy).AnonymousAllowedTypes.Add(typeof(Employee));
                };
            });

            services.AddXafSecurity(options => {
                options.UserType = typeof(Employee);
            }).AddExternalAuthentication<HttpContextPrincipalProvider>()
            .AddAuthenticationProvider<AuthenticationStandardProviderOptions, CustomAuthenticationStandardProvider>(options => {
                options.IsSupportChangePassword = true;
                options.LogonParametersType = typeof(CustomLogonParameters);
            });

            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddHttpContextAccessor();
            services.AddSingleton<XpoDataStoreProviderAccessor>();
            services.AddScoped<CircuitHandler, CircuitHandlerProxy>();
            services.AddXaf<AgencyPortaXafBlazorApplication>(Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env) {
            if(env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
            }
            else {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseRequestLocalization();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseXaf();
            app.UseEndpoints(endpoints => {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
