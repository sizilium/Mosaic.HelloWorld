using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VP.FF.PT.Common.GenericScanStation.RestInterfaces;

namespace VP.FF.PT.Common.GenericScanStation.Interface
{
    public class BarcodeReceivedFeedback
    {
        public ScanStationErrors Result { get; set; }
        public BarcodeResultJsonType Data { get; set; }
    }
}
