using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using VP.FF.PT.Common.PlatformEssentials;

namespace MosaicSample.FinishingStationModule
{
    [InheritedExport]
    [ServiceContract(SessionMode = SessionMode.Allowed)]
    public interface IFinishingStationModuleServiceREST
    {
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(FinishingStationModuleServiceFault))]
        bool KeepAlive();

        bool IsInitialized
        {
            [OperationContract]
            [WebGet(UriTemplate = "/IsInitialized", ResponseFormat = WebMessageFormat.Json)]
            [FaultContract(typeof(FinishingStationModuleServiceFault))]
            get;
        }

        [OperationContract]
        [FaultContract(typeof(FinishingStationModuleServiceFault))]
        void Start();

        [OperationContract]
        [FaultContract(typeof(FinishingStationModuleServiceFault))]
        void Stop();

        [OperationContract]
        [WebInvoke(UriTemplate = "/ResetAlarms?moduleInstance={moduleInstance}", ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(FinishingStationModuleServiceFault))]
        void ResetAlarms(int moduleInstance);

        [OperationContract]
        void SubscribeEvents(int moduleInstance);

        [OperationContract]
        void UnsubscribeEvents(int moduleInstance);

        [OperationContract]
        [WebInvoke(UriTemplate = "/UpdateZebraPrinterIp?moduleInstance={moduleInstance}&ipaddress={ipaddress}", ResponseFormat = WebMessageFormat.Json)]
        void UpdateZebraPrinterIp(int moduleInstance, string ipaddress);

        [OperationContract]
        [WebInvoke(UriTemplate = "/UpdateZplTemplateUrl?moduleInstance={moduleInstance}&url={url}", ResponseFormat = WebMessageFormat.Json)]
        void UpdateZplTemplateUrl(int moduleInstance, string url);

    }

    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IFinishingStationModuleServiceEvents))]
    public interface IFinishingStationModuleService : IFinishingStationModuleServiceREST, IPlatformModuleWcfService
    {

    }

    public interface IFinishingStationModuleServiceEvents : IPlatformModuleServiceEvents
    {
        [OperationContract(IsOneWay = true)]
        void ZebraPrinterIpChanged(string ipAddress);

        [OperationContract(IsOneWay = true)]
        void ZplTemplateChanged(string url);
    }

    [DataContract]
    public class FinishingStationModuleServiceFault
    {
    }

}
