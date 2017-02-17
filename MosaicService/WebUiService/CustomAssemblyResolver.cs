using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dispatcher;


namespace Common.WebUiService
{
    /// <summary>
    /// This class is needed to find all assemblies that contain an ApiController derived class and add it to the assembly list
    /// This class is only needed if the assemblies with the REST interface classes (derivated ApiController) is not loaded automatically
    /// 
    /// </summary>
    class CustomAssemblyResolver:DefaultAssembliesResolver
    {
        public override ICollection<Assembly> GetAssemblies()
        {
            ICollection<Assembly> baseAssemblies = base.GetAssemblies();
            List<Assembly> assemblies = new List<Assembly>(baseAssemblies);

            // Find all Assemblies which have a class that derives from ApiController but is not called ApiController
            var type = typeof (ApiController);

            try
            {
                var types = baseAssemblies.SelectMany(s => s.GetTypes()).Where(p => type.IsAssignableFrom(p) && !string.Equals(p.Name,"ApiController"));

                if (types.Any())
                {
                    // ToDo: Load any dll's that contain RestInterface Classes. Normally it is enough to add them to add a reference to the main project.
                    // ToDo: The load of dll's could be done vie an entry in the app.config. Not needed if using the common bootstrapper because it loads all available dll's recursively.
                }

            }
            catch (ReflectionTypeLoadException e)
            {
                Console.WriteLine(e.Message);
            }

            return assemblies;
        }
    }
}
