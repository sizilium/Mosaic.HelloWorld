using MosaicSample.Shell.CommonServices;
using System.ComponentModel.Composition;
using System.ServiceModel;
using VP.FF.PT.Common.GuiEssentials.Wcf;

namespace MosaicSample.Shell.ViewModels
{
    [Export(typeof(IDuplexCommunicationObjectFactory<ICommonServices, ICommonServicesCallback>))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class CommonServicesClientFactory : IDuplexCommunicationObjectFactory<ICommonServices, ICommonServicesCallback>
    {
        public ICommonServices CreateCommunicationObject(ICommonServicesCallback callbackInstance)
        {
            return new CommonServicesClient(new InstanceContext(callbackInstance));
        }
    }
}