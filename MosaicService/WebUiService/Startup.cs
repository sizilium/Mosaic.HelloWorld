using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.Tracing;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;
using Microsoft.Owin.Logging;
using Microsoft.Owin.StaticFiles;
using Owin;

[assembly: OwinStartup(typeof(Common.WebUiService.Startup))]

namespace Common.WebUiService
{
    public class Startup
    {
        [Import]
        private ILogger _logger;

        public void Configuration(IAppBuilder app)
        {
            //  Hack from http://stackoverflow.com/a/17227764/19020 to load controllers in 
            //  another assembly.  Another way to do this is to create a custom assembly resolver
            //  var name = typeof(RestScanStationController).FullName;
            // ToDo: Check if ApiController residing in other asseblies can be dynamically added
            // Set our own assembly resolver where we add the assemblies we need

            //app.Use(async (env, next) =>
            //{
            //    Console.WriteLine(string.Concat("Http method: ", env.Request.Method, ", path: ", env.Request.Path));
            //    await next();
            //    Console.WriteLine(string.Concat("Response code: ", env.Response.StatusCode));
            //});

            // Configure Web API for self-host. 
            var config = new HttpConfiguration();

            var assemblyResolver = new CustomAssemblyResolver();
            config.Services.Replace(typeof(IAssembliesResolver), assemblyResolver);

            //Uncomment the following lines to enable extended tracing
            //---------------------------------------------------------- 
            //var traceWriter = config.EnableSystemDiagnosticsTracing();
            //traceWriter.IsVerbose = true;
            //traceWriter.MinimumLevel = TraceLevel.Debug;
            //---------------------------------------------------------- 

            // Enable Attribute based routing
            config.MapHttpAttributeRoutes();
            app.UseWebApi(config);

            var options = new FileServerOptions()
            {
                EnableDirectoryBrowsing = false,
                EnableDefaultFiles = true,
                FileSystem = new PhysicalFileSystem(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"StaticWebContent")),
                DefaultFilesOptions = { DefaultFileNames = { "Index.html" } }
            };

            app.UseFileServer(options);
        }
    }
}
