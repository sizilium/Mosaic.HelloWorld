using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MosaicSample.Infrastructure.Properties;
using Newtonsoft.Json;
using VP.FF.PT.Common.Infrastructure.Logging;

namespace MosaicSample.Infrastructure.Workflow
{
    [Export]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class WorkflowProvider
    {
        private WorkflowCollection _workflowCollection;
        private ILogger _logger;

        public WorkflowCollection WorkflowCollection
        {
            get
            {
                if (_workflowCollection == null)
                {
                    LoadJson();
                }

                return _workflowCollection;
            }

            private set { _workflowCollection = value; }
        }


        [ImportingConstructor]
        public WorkflowProvider(ILogger logger)
        {
            _logger = logger;
        }

        public void LoadJson()
        {
            try
            {
                var r = new StreamReader(Settings.Default.RecipeDefinitionFileName);
                var json = r.ReadToEnd();

                _workflowCollection = JsonConvert.DeserializeObject<WorkflowCollection>(json);
            }
            catch (Exception e)
            {
                _logger?.ErrorFormat("RecipeProvicer - LoadJson Error : {0}",e.Message);
            }
        }
    }
}
