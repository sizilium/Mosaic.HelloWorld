using System;
using System.ComponentModel.Composition;
using VP.FF.PT.Common.Infrastructure.Logging;
using VP.FF.PT.Common.PlatformEssentials.Entities;

namespace MosaicSample.Infrastructure.Workflow
{
    [Export(typeof(IMosaicSampleWorkflowFactory))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    class MosaicSampleWorkflowFactory : WorkflowFactoryBase, IMosaicSampleWorkflowFactory
    {
        [ImportingConstructor]
        protected MosaicSampleWorkflowFactory(IEntityContextFactory contextFactory, ILogger logger) : base(contextFactory, logger)
        {
        }

        public Route CreateRecipe()
        {
            return new Route();
        }

        public Route CreateRecipe(Route route, Workflow workflowDefinition)
        {
            foreach (var step in workflowDefinition.WorkFlow)
            {
                PredefinedModuleTypes stepType;

                if (Enum.TryParse(step, true, out stepType))
                {
                    AddRecipeItem(route, (int)stepType);
                }
                else
                {
                    _logger.ErrorFormat("FP3RecipeFactory - CreateRecipe: Invalid step {0}",step);
                }
            }

            return route;
        }
    }
}
