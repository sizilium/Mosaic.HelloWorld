using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VP.FF.PT.Common.GenericScanStation.Interface
{
    public class BarcodeEventArgs:EventArgs
    {
        public BarcodeEventArgs(string barcode)
        {
            Barcode = barcode;
        }

        public string Barcode { get; set; }
    }
}
