using DotNetCoreAngularHMR_Spa.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DotNetCoreAngularHMR_Spa
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            // Captures webpack-dev-server related requests such as
            // websocket requests (/sockjs-node/) requests to the webpack dev server
            // These requests are made outside of '/app' sub-path so capture them here
            // These websocket requests cant be changed to use a sub-path until this has been released
            // https://github.com/webpack/webpack-dev-server/pull/1553
            // however to use that fix with Angular would require ejecting from the CLI
            app.MapWhen(context => webPackDevServerMatcher(context), webpackDevServer => {
                webpackDevServer.UseSpa(spa => {
                    spa.UseProxyToSpaDevelopmentServer(baseUri: "http://localhost:4200");
                });
            });


            // Serve the Angular app from a sub-path '/app' off the root
            // Map the path segment '/app' to the Spa middleware
            // Custom middleware MapPath has additional option 'removeMatchedPathSegment'
            // In development we need to keep the matched path segment
            // as the request is proxied to the Angular dev server which is set to serve files from /app/
            // In production we remove the matched segment as the files exist in the root of the 'ClientApp/dist' dir.
            app.MapPath("/app", !env.IsDevelopment(), frontendApp => {
                if (!env.IsDevelopment()) {
                    // In Production env, ClientApp is served using minified and bundled code from 'ClientApp/dist'
                    frontendApp.UseSpaStaticFiles();
                }
                frontendApp.UseSpa(spa => {
                    spa.Options.SourcePath = "ClientApp";

                    if (env.IsDevelopment()) {
                        // Can't use UseAngularCliServer because we need a fixed port for the dev server
                        // in order to correctly forward the /sockjs-node/ requests to the dev server in the app.MapWhen block above
                        //spa.UseAngularCliServer(npmScript: "hmr");
                        // Angular Webpack Dev Server runs on port 4200
                        spa.UseProxyToSpaDevelopmentServer(baseUri: "http://localhost:4200");
                    }
                });
            });
        }

        // Captures the requests generated when using webpack dev server in the following ways:
        // via: https://localhost:5001/app/ (inline mode)
        // via: https://localhost:5001/webpack-dev-server/app/  (iframe mode)
        // captures requests like these:
        // https://localhost:5001/webpack_dev_server.js
        // https://localhost:5001/webpack_dev_server/app/
        // https://localhost:5001/__webpack_dev_server__/live.bundle.js
        // wss://localhost:5001/sockjs-node/978/qhjp11ck/websocket
        private bool webPackDevServerMatcher(HttpContext context) {
            string pathString = context.Request.Path.ToString();
            return pathString.Contains(context.Request.PathBase.Add("/webpack-dev-server")) ||
                context.Request.Path.StartsWithSegments("/__webpack_dev_server__") ||
                context.Request.Path.StartsWithSegments("/sockjs-node");
        }
    }
}
