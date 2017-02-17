using System.Collections.Generic;
using VP.FF.PT.Common.WpfInfrastructure.Screens.ViewModels;

namespace Cimpress.ACS.FP3.UIInfrastructure.AlarmService
{
    public interface IAlarmManagementFactory
    {
        AlarmSummaryViewModel CreateAlarmSummaryViewModel(string module);
        AlarmSummaryViewModel CreateAlarmSummaryViewModel(ICollection<string> modules);
    }
}