using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using VP.FF.PT.Common.PlatformEssentials;

namespace MosaicSample.DemoModuleC
{
    [InheritedExport]
    [ServiceContract(SessionMode = SessionMode.Allowed)]
    public interface IDemoModuleCServiceREST
    {
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(IDemoModuleCServiceFault))]
        bool KeepAlive();

        bool IsInitialized
        {
            [OperationContract]
            [WebGet(UriTemplate = "/IsInitialized", ResponseFormat = WebMessageFormat.Json)]
            [FaultContract(typeof(IDemoModuleCServiceFault))]
            get;
        }

        [OperationContract]
        [FaultContract(typeof(IDemoModuleCServiceFault))]
        void Start(int moduleInstance);

        [OperationContract]
        [FaultContract(typeof(IDemoModuleCServiceFault))]
        void Stop(int moduleInstance);

        [OperationContract]
        [WebInvoke(UriTemplate = "/ResetAlarms?moduleInstance={moduleInstance}", ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(IDemoModuleCServiceFault))]
        void ResetAlarms(int moduleInstance);

        [OperationContract]
        void SubscribeEvents(int moduleInstance);

        [OperationContract]
        void UnsubscribeEvents(int moduleInstance);
    }

    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IDemoModuleCServiceEvents))]
    public interface IDemoModuleCService : IDemoModuleCServiceREST, IPlatformModuleWcfService
    {

    }

    public interface IDemoModuleCServiceEvents : IPlatformModuleServiceEvents
    {
    }

    [DataContract]
    public class IDemoModuleCServiceFault
    {

    }

}
