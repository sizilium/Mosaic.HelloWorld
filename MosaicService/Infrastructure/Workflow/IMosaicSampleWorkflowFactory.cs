using VP.FF.PT.Common.PlatformEssentials.Entities;

namespace MosaicSample.Infrastructure.Workflow
{
    public interface IMosaicSampleWorkflowFactory
    {
        Route CreateRecipe(Route route, Workflow workflowDefinition);
        Route CreateRecipe();
    }
}
