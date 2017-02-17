using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Cimpress.ACS.FP3.UIInfrastructure.AlarmManagementService;
using VP.FF.PT.Common.GuiEssentials.Wcf;
using VP.FF.PT.Common.Infrastructure.Logging;
using VP.FF.PT.Common.PlatformEssentials.Entities.DTOs;
using VP.FF.PT.Common.WpfInfrastructure.Screens.Model;
using VP.FF.PT.Common.WpfInfrastructure.Screens.Services;
using VP.FF.PT.Common.WpfInfrastructure.Threading;

namespace Cimpress.ACS.FP3.UIInfrastructure.AlarmService
{
    /// <summary>
    /// The <see cref="AlarmManagerServiceAdapter"/> serves as client for the
    /// <see cref="IAlarmManagementService"/> and as receiver of events over the <see cref="IAlarmManagementServiceCallback"/>.
    /// </summary>
    public class AlarmManagerServiceAdapter : IResetAlarms, IProvideAlarms, IAlarmManagementServiceCallback
    {
        private const int KeepAliveInterval = 60000;

        private readonly ICollection<string> _moduleNames;
        private readonly Timer _keepAliveTimer;
        private readonly IClient<IAlarmManagementService> _client;
        private readonly IDispatcher _dispatcher;
        private readonly ILogger _logger;
        private readonly ConcurrentDictionary<string, Action<IEnumerable<Alarm>, IEnumerable<Alarm>>> _handlerByModule;

        /// <summary>
        /// Initilizes a new <see cref="AlarmManagerServiceAdapter"/> instance.
        /// </summary>
        /// <param name="moduleName">The name of the modules this instance is dedicated to.</param>
        /// <param name="clientFactory">An instance capable of creating a <see cref="IClient{TService}"/> for the alarm management service.</param>
        /// <param name="dispatcher">A dispatcher.</param>
        /// <param name="logger">The logger.</param>
        public AlarmManagerServiceAdapter(
            string moduleName,
            IDuplexClientFactory<IAlarmManagementService, IAlarmManagementServiceCallback> clientFactory,
            IDispatcher dispatcher,
            ILogger logger)
        {
            _moduleNames = new Collection<string>();
            _moduleNames.Add(moduleName);
            _dispatcher = dispatcher;
            _logger = logger;
            _handlerByModule = new ConcurrentDictionary<string, Action<IEnumerable<Alarm>, IEnumerable<Alarm>>>();
            _client = clientFactory.CreateClient(this);

            _keepAliveTimer = new Timer(UpdateCallback, null, 0, KeepAliveInterval);
        }

        public AlarmManagerServiceAdapter(
            ICollection<string> moduleNames,
            IDuplexClientFactory<IAlarmManagementService,
            IAlarmManagementServiceCallback> clientFactory,
            IDispatcher dispatcher,
            ILogger logger)
        {
            _moduleNames = moduleNames;
            _dispatcher = dispatcher;
            _logger = logger;
            _handlerByModule = new ConcurrentDictionary<string, Action<IEnumerable<Alarm>, IEnumerable<Alarm>>>();
            _client = clientFactory.CreateClient(this);

            _keepAliveTimer = new Timer(UpdateCallback, null, 0, KeepAliveInterval);
        }

        /// <summary>
        /// Requests the current and the historic alarms of the specified <paramref name="modules"/>.
        /// </summary>
        /// <param name="modules">The name of the platform modules to get the alarms from.s</param>
        /// <returns>A <see cref="Task"/> instance.</returns>
        public async Task RequestAlarms(ICollection<string> modules)
        {
            Dictionary<string, AlarmDTO[]> currentAlarms = await _client.InvokeOnService(c => c.GetCurrentAlarmsOfModulesAsync(modules.ToArray()));
            Dictionary<string, AlarmDTO[]> historicAlarms = await _client.InvokeOnService(c => c.GetHistoricAlarmsOfModulesAsync(modules.ToArray()));

            List<Alarm> currentAlarmViewModels = new List<Alarm>();
            List<Alarm> historicAlarmViewModels = new List<Alarm>();

            foreach (var module in modules)
            {
                if (currentAlarms != null && currentAlarms.ContainsKey(module))
                    currentAlarmViewModels.AddRange(ToAlarmViewModels(module, currentAlarms[module]));
                if (historicAlarms != null && historicAlarms.ContainsKey(module))
                    historicAlarmViewModels.AddRange(ToAlarmViewModels(module, historicAlarms[module]));
            }

            Action<IEnumerable<Alarm>, IEnumerable<Alarm>> notifyObserver;
            if (_handlerByModule.TryGetValue(modules.First(), out notifyObserver))
            {
                _dispatcher.Dispatch(() => notifyObserver(currentAlarmViewModels, historicAlarmViewModels));
            }
        }

        /// <summary>
        /// Resets the alarms of the platform modules with the name <see cref="module"/>.
        /// </summary>
        /// <param name="module">The name of the modules which alarms should get reseted.</param>
        /// <returns>A <see cref="Task"/> instance.</returns>
        public Task ResetAlarms(string module)
        {
            _logger.InfoFormat("Acknowledge current alarms for module '{0}'", module);
            return _client.InvokeOnService(c => c.AcknowledgeAlarmsAsync(module));
        }

        /// <summary>
        /// Subscribes the specified <paramref name="handler"/> to get invoked when alarms
        /// on the specified <paramref name="modules"/> change.
        /// </summary>
        /// <param name="modules">The name of the modules to observe.</param>
        /// <param name="handler">The handler to invoke on changes.</param>
        /// <returns>A <see cref="Task"/> instance.</returns>
        public Task SubscribeForAlarmChanges(ICollection<string> modules, Action<IEnumerable<Alarm>, IEnumerable<Alarm>> handler)
        {
            foreach (var module in modules)
            {
                _logger.InfoFormat("Subscribing for alarm changes on module '{0}'", module);

                if (_handlerByModule.ContainsKey(module))
                {
                    Action<IEnumerable<Alarm>, IEnumerable<Alarm>> dummy;
                    _handlerByModule.TryRemove(module, out dummy);
                }

                _handlerByModule.TryAdd(module, handler);
            }

            return _client.InvokeOnService(c => c.SubscribeForAlarmChangesOnModulesAsync(modules.ToArray()));
        }

        /// <summary>
        /// Unsubscribes the specified <paramref name="handler"/> from getting invoked when alarms
        /// on the specified <paramref name="modules"/> change.
        /// </summary>
        /// <param name="modules">The name of the modules to observe.</param>
        /// <param name="handler">The handler to not anymore invoke on changes.</param>
        /// <returns>A <see cref="Task"/> instance.</returns>
        public Task UnsubscribeFromAlarmChanges(ICollection<string> modules, Action<IEnumerable<Alarm>, IEnumerable<Alarm>> handler)
        {
            foreach (var module in modules)
            {
                _logger.InfoFormat("Unsubscribing from alarm changes on module '{0}'", module);
                Action<IEnumerable<Alarm>, IEnumerable<Alarm>> dummy;
                _handlerByModule.TryRemove(module, out dummy);
            }

            return _client.InvokeOnService(c => c.UnsubscribeFromAlarmChangesFromModulesAsync(modules.ToArray()));
        }

        public void AlarmsChanged(string module)
        {
            _logger.InfoFormat("Received alarms changed notification from module '{0}'.", module);
            if (_moduleNames.Contains(module))
                _dispatcher.Dispatch(async () => await RequestAlarms(_moduleNames));
        }

        private IEnumerable<Alarm> ToAlarmViewModels(string module, IEnumerable<AlarmDTO> alarmDtos)
        {
            return new ObservableCollection<Alarm>(alarmDtos.Select(dto => new Alarm
            {
                AlarmId = dto.Id,
                Message = dto.Message,
                Source = ParseAlarmSource(module, dto.Source),
                Timestamp = dto.Timestamp,
                Type = (AlarmType)dto.Type
            }));
        }

        private string ParseAlarmSource(string moduleName, string oldSource)
        {
            string firstpart = null, secondpart = null, thirdpart = null;
            Match match;

            firstpart = moduleName;

            if (oldSource == null)
                return null;

            match = Regex.Match(oldSource, @"r([A-Za-z0-9\-]+)",
                           RegexOptions.IgnoreCase);
            if (match.Success)
                secondpart = "." + match.Groups[1].Value;

            match = Regex.Match(oldSource, @"ctrl.([A-Za-z0-9\-]+)",
                            RegexOptions.IgnoreCase);
            if (match.Success)
                thirdpart = "." + match.Groups[1].Value;

            var newSource = string.Concat(firstpart, secondpart, thirdpart);

            if (String.IsNullOrEmpty(newSource))
                return oldSource;

            return newSource;
        }

        private void UpdateCallback(object state)
        {
            try
            {
                _client.InvokeOnService(c => c.KeepAliveAsync());
                _keepAliveTimer.Change(KeepAliveInterval, Timeout.Infinite);
            }
            catch (Exception e)
            {
                _logger.Error("No keep alive to server! Module disabled.", e);
                //ShowFatalErrorMessage("No keep alive to server! Module disabled.", CommonErrorMessageSuggestionText, e);
            }
        }
    }
}