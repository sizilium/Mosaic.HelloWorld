using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using VP.FF.PT.Common.PlatformEssentials;

namespace MosaicSample.DemoModuleB
{
    [InheritedExport]
    [ServiceContract(SessionMode = SessionMode.Allowed)]
    public interface IDemoModuleBServiceREST
    {
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(IDemoModuleBServiceFault))]
        bool KeepAlive();

        bool IsInitialized
        {
            [OperationContract]
            [WebGet(UriTemplate = "/IsInitialized", ResponseFormat = WebMessageFormat.Json)]
            [FaultContract(typeof(IDemoModuleBServiceFault))]
            get;
        }

        [OperationContract]
        [FaultContract(typeof(IDemoModuleBServiceFault))]
        void Start();

        [OperationContract]
        [FaultContract(typeof(IDemoModuleBServiceFault))]
        void Stop();

        [OperationContract]
        [WebInvoke(UriTemplate = "/ResetAlarms?moduleInstance={moduleInstance}", ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(IDemoModuleBServiceFault))]
        void ResetAlarms(int moduleInstance);

        [OperationContract]
        void SubscribeEvents(int moduleInstance);

        [OperationContract]
        void UnsubscribeEvents(int moduleInstance);
    }

    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IDemoModuleBServiceEvents))]
    public interface IDemoModuleBService : IDemoModuleBServiceREST, IPlatformModuleWcfService
    {

    }

    public interface IDemoModuleBServiceEvents : IPlatformModuleServiceEvents
    {
    }

    [DataContract]
    public class IDemoModuleBServiceFault
    {

    }

}
