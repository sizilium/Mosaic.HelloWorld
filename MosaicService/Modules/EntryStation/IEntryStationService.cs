using System.ComponentModel.Composition;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using VP.FF.PT.Common.PlatformEssentials;

namespace MosaicSample.EntryStation
{
    [InheritedExport]
    [ServiceContract(SessionMode = SessionMode.Allowed)]
    public interface IEntryStationServiceREST
    {
        [OperationContract]
        [WebGet(ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(IEntryStationServiceFault))]
        bool KeepAlive();

        bool IsInitialized
        {
            [OperationContract]
            [WebGet(UriTemplate = "/IsInitialized", ResponseFormat = WebMessageFormat.Json)]
            [FaultContract(typeof(IEntryStationServiceFault))]
            get;
        }

        [OperationContract]
        [FaultContract(typeof(IEntryStationServiceFault))]
        void Start();

        [OperationContract]
        [FaultContract(typeof(IEntryStationServiceFault))]
        void Stop();

        [OperationContract]
        [WebInvoke(UriTemplate = "/ResetAlarms?moduleInstance={moduleInstance}", ResponseFormat = WebMessageFormat.Json)]
        [FaultContract(typeof(IEntryStationServiceFault))]
        void ResetAlarms(int moduleInstance);

        [OperationContract]
        void SubscribeEvents(int moduleInstance);

        [OperationContract]
        void UnsubscribeEvents(int moduleInstance);
    }

    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(IEntryStationServiceEvents))]
    public interface IEntryStationService : IEntryStationServiceREST, IPlatformModuleWcfService
    {

    }

    public interface IEntryStationServiceEvents : IPlatformModuleServiceEvents
    {
    }

    [DataContract]
    public class IEntryStationServiceFault
    {

    }

}
