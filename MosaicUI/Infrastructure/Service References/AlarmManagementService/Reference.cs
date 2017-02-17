﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Cimpress.ACS.FP3.UIInfrastructure.AlarmManagementService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="AlarmManagementService.IAlarmManagementService", CallbackContract=typeof(Cimpress.ACS.FP3.UIInfrastructure.AlarmManagementService.IAlarmManagementServiceCallback), SessionMode=System.ServiceModel.SessionMode.Required)]
    public interface IAlarmManagementService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAlarmManagementService/KeepAlive", ReplyAction="http://tempuri.org/IAlarmManagementService/KeepAliveResponse")]
        bool KeepAlive();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAlarmManagementService/KeepAlive", ReplyAction="http://tempuri.org/IAlarmManagementService/KeepAliveResponse")]
        System.Threading.Tasks.Task<bool> KeepAliveAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAlarmManagementService/GetCurrentAlarms", ReplyAction="http://tempuri.org/IAlarmManagementService/GetCurrentAlarmsResponse")]
        VP.FF.PT.Common.PlatformEssentials.Entities.DTOs.AlarmDTO[] GetCurrentAlarms(string moduleName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAlarmManagementService/GetCurrentAlarms", ReplyAction="http://tempuri.org/IAlarmManagementService/GetCurrentAlarmsResponse")]
        System.Threading.Tasks.Task<VP.FF.PT.Common.PlatformEssentials.Entities.DTOs.AlarmDTO[]> GetCurrentAlarmsAsync(string moduleName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAlarmManagementService/GetCurrentAlarmsOfModules", ReplyAction="http://tempuri.org/IAlarmManagementService/GetCurrentAlarmsOfModulesResponse")]
        System.Collections.Generic.Dictionary<string, VP.FF.PT.Common.PlatformEssentials.Entities.DTOs.AlarmDTO[]> GetCurrentAlarmsOfModules(string[] moduleNames);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAlarmManagementService/GetCurrentAlarmsOfModules", ReplyAction="http://tempuri.org/IAlarmManagementService/GetCurrentAlarmsOfModulesResponse")]
        System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<string, VP.FF.PT.Common.PlatformEssentials.Entities.DTOs.AlarmDTO[]>> GetCurrentAlarmsOfModulesAsync(string[] moduleNames);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAlarmManagementService/GetHistoricAlarms", ReplyAction="http://tempuri.org/IAlarmManagementService/GetHistoricAlarmsResponse")]
        VP.FF.PT.Common.PlatformEssentials.Entities.DTOs.AlarmDTO[] GetHistoricAlarms(string moduleName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAlarmManagementService/GetHistoricAlarms", ReplyAction="http://tempuri.org/IAlarmManagementService/GetHistoricAlarmsResponse")]
        System.Threading.Tasks.Task<VP.FF.PT.Common.PlatformEssentials.Entities.DTOs.AlarmDTO[]> GetHistoricAlarmsAsync(string moduleName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAlarmManagementService/GetHistoricAlarmsOfModules", ReplyAction="http://tempuri.org/IAlarmManagementService/GetHistoricAlarmsOfModulesResponse")]
        System.Collections.Generic.Dictionary<string, VP.FF.PT.Common.PlatformEssentials.Entities.DTOs.AlarmDTO[]> GetHistoricAlarmsOfModules(string[] moduleNames);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAlarmManagementService/GetHistoricAlarmsOfModules", ReplyAction="http://tempuri.org/IAlarmManagementService/GetHistoricAlarmsOfModulesResponse")]
        System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<string, VP.FF.PT.Common.PlatformEssentials.Entities.DTOs.AlarmDTO[]>> GetHistoricAlarmsOfModulesAsync(string[] moduleNames);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAlarmManagementService/AcknowledgeAlarms", ReplyAction="http://tempuri.org/IAlarmManagementService/AcknowledgeAlarmsResponse")]
        void AcknowledgeAlarms(string moduleName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAlarmManagementService/AcknowledgeAlarms", ReplyAction="http://tempuri.org/IAlarmManagementService/AcknowledgeAlarmsResponse")]
        System.Threading.Tasks.Task AcknowledgeAlarmsAsync(string moduleName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAlarmManagementService/SubscribeForAlarmChangesOnModule", ReplyAction="http://tempuri.org/IAlarmManagementService/SubscribeForAlarmChangesOnModuleRespon" +
            "se")]
        void SubscribeForAlarmChangesOnModule(string moduleName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAlarmManagementService/SubscribeForAlarmChangesOnModule", ReplyAction="http://tempuri.org/IAlarmManagementService/SubscribeForAlarmChangesOnModuleRespon" +
            "se")]
        System.Threading.Tasks.Task SubscribeForAlarmChangesOnModuleAsync(string moduleName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAlarmManagementService/SubscribeForAlarmChangesOnModules", ReplyAction="http://tempuri.org/IAlarmManagementService/SubscribeForAlarmChangesOnModulesRespo" +
            "nse")]
        void SubscribeForAlarmChangesOnModules(string[] moduleNames);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAlarmManagementService/SubscribeForAlarmChangesOnModules", ReplyAction="http://tempuri.org/IAlarmManagementService/SubscribeForAlarmChangesOnModulesRespo" +
            "nse")]
        System.Threading.Tasks.Task SubscribeForAlarmChangesOnModulesAsync(string[] moduleNames);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAlarmManagementService/UnsubscribeFromAlarmChangesFromModule", ReplyAction="http://tempuri.org/IAlarmManagementService/UnsubscribeFromAlarmChangesFromModuleR" +
            "esponse")]
        void UnsubscribeFromAlarmChangesFromModule(string moduleName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAlarmManagementService/UnsubscribeFromAlarmChangesFromModule", ReplyAction="http://tempuri.org/IAlarmManagementService/UnsubscribeFromAlarmChangesFromModuleR" +
            "esponse")]
        System.Threading.Tasks.Task UnsubscribeFromAlarmChangesFromModuleAsync(string moduleName);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAlarmManagementService/UnsubscribeFromAlarmChangesFromModules" +
            "", ReplyAction="http://tempuri.org/IAlarmManagementService/UnsubscribeFromAlarmChangesFromModules" +
            "Response")]
        void UnsubscribeFromAlarmChangesFromModules(string[] moduleNames);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IAlarmManagementService/UnsubscribeFromAlarmChangesFromModules" +
            "", ReplyAction="http://tempuri.org/IAlarmManagementService/UnsubscribeFromAlarmChangesFromModules" +
            "Response")]
        System.Threading.Tasks.Task UnsubscribeFromAlarmChangesFromModulesAsync(string[] moduleNames);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IAlarmManagementServiceCallback {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/IAlarmManagementService/AlarmsChanged")]
        void AlarmsChanged(string module);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IAlarmManagementServiceChannel : Cimpress.ACS.FP3.UIInfrastructure.AlarmManagementService.IAlarmManagementService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class AlarmManagementServiceClient : System.ServiceModel.DuplexClientBase<Cimpress.ACS.FP3.UIInfrastructure.AlarmManagementService.IAlarmManagementService>, Cimpress.ACS.FP3.UIInfrastructure.AlarmManagementService.IAlarmManagementService {
        
        public AlarmManagementServiceClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public AlarmManagementServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public AlarmManagementServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public AlarmManagementServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public AlarmManagementServiceClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public bool KeepAlive() {
            return base.Channel.KeepAlive();
        }
        
        public System.Threading.Tasks.Task<bool> KeepAliveAsync() {
            return base.Channel.KeepAliveAsync();
        }
        
        public VP.FF.PT.Common.PlatformEssentials.Entities.DTOs.AlarmDTO[] GetCurrentAlarms(string moduleName) {
            return base.Channel.GetCurrentAlarms(moduleName);
        }
        
        public System.Threading.Tasks.Task<VP.FF.PT.Common.PlatformEssentials.Entities.DTOs.AlarmDTO[]> GetCurrentAlarmsAsync(string moduleName) {
            return base.Channel.GetCurrentAlarmsAsync(moduleName);
        }
        
        public System.Collections.Generic.Dictionary<string, VP.FF.PT.Common.PlatformEssentials.Entities.DTOs.AlarmDTO[]> GetCurrentAlarmsOfModules(string[] moduleNames) {
            return base.Channel.GetCurrentAlarmsOfModules(moduleNames);
        }
        
        public System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<string, VP.FF.PT.Common.PlatformEssentials.Entities.DTOs.AlarmDTO[]>> GetCurrentAlarmsOfModulesAsync(string[] moduleNames) {
            return base.Channel.GetCurrentAlarmsOfModulesAsync(moduleNames);
        }
        
        public VP.FF.PT.Common.PlatformEssentials.Entities.DTOs.AlarmDTO[] GetHistoricAlarms(string moduleName) {
            return base.Channel.GetHistoricAlarms(moduleName);
        }
        
        public System.Threading.Tasks.Task<VP.FF.PT.Common.PlatformEssentials.Entities.DTOs.AlarmDTO[]> GetHistoricAlarmsAsync(string moduleName) {
            return base.Channel.GetHistoricAlarmsAsync(moduleName);
        }
        
        public System.Collections.Generic.Dictionary<string, VP.FF.PT.Common.PlatformEssentials.Entities.DTOs.AlarmDTO[]> GetHistoricAlarmsOfModules(string[] moduleNames) {
            return base.Channel.GetHistoricAlarmsOfModules(moduleNames);
        }
        
        public System.Threading.Tasks.Task<System.Collections.Generic.Dictionary<string, VP.FF.PT.Common.PlatformEssentials.Entities.DTOs.AlarmDTO[]>> GetHistoricAlarmsOfModulesAsync(string[] moduleNames) {
            return base.Channel.GetHistoricAlarmsOfModulesAsync(moduleNames);
        }
        
        public void AcknowledgeAlarms(string moduleName) {
            base.Channel.AcknowledgeAlarms(moduleName);
        }
        
        public System.Threading.Tasks.Task AcknowledgeAlarmsAsync(string moduleName) {
            return base.Channel.AcknowledgeAlarmsAsync(moduleName);
        }
        
        public void SubscribeForAlarmChangesOnModule(string moduleName) {
            base.Channel.SubscribeForAlarmChangesOnModule(moduleName);
        }
        
        public System.Threading.Tasks.Task SubscribeForAlarmChangesOnModuleAsync(string moduleName) {
            return base.Channel.SubscribeForAlarmChangesOnModuleAsync(moduleName);
        }
        
        public void SubscribeForAlarmChangesOnModules(string[] moduleNames) {
            base.Channel.SubscribeForAlarmChangesOnModules(moduleNames);
        }
        
        public System.Threading.Tasks.Task SubscribeForAlarmChangesOnModulesAsync(string[] moduleNames) {
            return base.Channel.SubscribeForAlarmChangesOnModulesAsync(moduleNames);
        }
        
        public void UnsubscribeFromAlarmChangesFromModule(string moduleName) {
            base.Channel.UnsubscribeFromAlarmChangesFromModule(moduleName);
        }
        
        public System.Threading.Tasks.Task UnsubscribeFromAlarmChangesFromModuleAsync(string moduleName) {
            return base.Channel.UnsubscribeFromAlarmChangesFromModuleAsync(moduleName);
        }
        
        public void UnsubscribeFromAlarmChangesFromModules(string[] moduleNames) {
            base.Channel.UnsubscribeFromAlarmChangesFromModules(moduleNames);
        }
        
        public System.Threading.Tasks.Task UnsubscribeFromAlarmChangesFromModulesAsync(string[] moduleNames) {
            return base.Channel.UnsubscribeFromAlarmChangesFromModulesAsync(moduleNames);
        }
    }
}