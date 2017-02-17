using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using VP.FF.PT.Common.PlatformEssentials;

namespace MosaicSample.DemoModuleA
{
    [InheritedExport]
    [ServiceContract(SessionMode = SessionMode.Allowed)]
    public interface IDemoModuleAServiceREST
    {
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(IDemoModuleAServiceFault))]
        bool KeepAlive();

        bool IsInitialized
        {
            [OperationContract]
            [WebGet(UriTemplate = "/IsInitialized", ResponseFormat = WebMessageFormat.Json)]
            [FaultContract(typeof(IDemoModuleAServiceFault))]
            get;
        }

        [OperationContract]
        //[WebInvoke(ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(IDemoModuleAServiceFault))]
        void Start();

        [OperationContract]
        //[WebInvoke(ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(IDemoModuleAServiceFault))]
        void Stop();

        [OperationContract]
        [WebInvoke(UriTemplate = "/ResetAlarms?moduleInstance={moduleInstance}", ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(IDemoModuleAServiceFault))]
        void ResetAlarms(int moduleInstance);

        [OperationContract]
        void SubscribeEvents(int moduleInstance);

        [OperationContract]
        void UnsubscribeEvents(int moduleInstance);
    }

    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IDemoModuleAServiceEvents))]
    public interface IDemoModuleAService : IDemoModuleAServiceREST, IPlatformModuleWcfService
    {
        
    }

    public interface IDemoModuleAServiceEvents : IPlatformModuleServiceEvents
    {
    }

    [DataContract]
    public class IDemoModuleAServiceFault
    {

    }

}
