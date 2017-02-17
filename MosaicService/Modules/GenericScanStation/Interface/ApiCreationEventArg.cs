using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VP.FF.PT.Common.GenericScanStation.Interface
{
    public class ApiCreationEventArg:EventArgs
    {
        public ApiCreationEventArg(IScanStation scanStation)
        {
            ScanStation = scanStation;
        }

        public IScanStation ScanStation { get; set; }
    }
}
