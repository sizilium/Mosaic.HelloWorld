using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VP.FF.PT.Common.GenericScanStation.RestInterfaces
{
    public class BarcodeResultJsonType
    {
        public string Barcode { get; set; }
        public string Message { get; set; }

        public int Result { get; set; }
    }
}
