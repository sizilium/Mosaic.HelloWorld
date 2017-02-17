using System.Runtime.Serialization;
using System.ServiceModel;
using VP.FF.PT.Common.PlatformEssentials.Entities.DTOs;
using VP.FF.PT.Common.PlatformEssentials.ItemFlow.DTOs;

namespace MosaicSample.CommonServices
{
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(ICommonServicesEvents))]
    public interface ICommonServices
    {
        [OperationContract]
        void SubscribeEvents();

        // handles ICommonServicesModuleStateEvents
        [OperationContract]
        void SubscribeModuleStateEvents();

        [OperationContract]
        void UnsubscribeEvents();

        /// <summary>
        /// Gets the line control software version.
        /// </summary>
        string LineControlVersion
        {
            [OperationContract]
            [FaultContract(typeof(CommonServicesFault))]
            get;
        }

        /// <summary>
        /// Keep alive service for WCF communication to detect communication issues.
        /// Determines whether this service is alive which means whether whole LineControl is reachable or not.
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance is alive; otherwise, <c>false</c>.
        /// </returns>
        bool IsAliveAndInitialized
        {
            [OperationContract]
            [FaultContract(typeof(CommonServicesFault))]
            get;
        }

        [OperationContract]
        [FaultContract(typeof(CommonServicesFault))]
        void StartAll();

        [OperationContract]
        [FaultContract(typeof(CommonServicesFault))]
        void StopAll();

        [OperationContract]
        [FaultContract(typeof(CommonServicesFault))]
        ModuleGraphDTO GetModuleGraph();

        // fires all services events (refreshes all module states)
        [OperationContract]
        [FaultContract(typeof(CommonServicesFault))]
        PlatformModuleDTO[] RequestStates();
    }

    public interface ICommonServicesEvents
    {
        [OperationContract(IsOneWay = true)]
        void PlatformModuleStateChanged(PlatformModuleDTO platformModule);

        [OperationContract(IsOneWay = true)]
        void PlatformStateChanged(PlatformStateDTO platformState);

        [OperationContract(IsOneWay = true)]
        void ResetIgnoreDownstreamModules();

        [OperationContract(IsOneWay = true)]
        void ModuleInitializationStarted(string moduleName);

        [OperationContract(IsOneWay = true)]
        void ModuleManualModeChanged(bool controllerManualMode, string moduleName);

        [OperationContract(IsOneWay = true)]
        void SimulationModeChanged(string moduleName, bool controllerSimulationMode);

        [OperationContract]
        void RequestShutdown();
    }

    // TODO: move it to common
    [DataContract]
    public enum PlatformStateDTO
    {
        [EnumMember]
        Idle,
        [EnumMember]
        Production,
        [EnumMember]
        StoppedAll
    }

    [DataContract]
    public class CommonServicesFault
    {
        public CommonServicesFault()
        {
        }

        public CommonServicesFault(string reason)
        {
            Reason = reason;
        }

        [DataMember]
        public string Reason { get; set; }
    }
}
