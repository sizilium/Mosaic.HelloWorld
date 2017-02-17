using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VP.FF.PT.Common.GenericScanStation.Interface
{

    public delegate BarcodeReceivedFeedback BarcodeReceivedDelegate(object sender, BarcodeEventArgs e);
    public delegate ScanStationErrors BarcodeProveDelegate(object sender, BarcodeEventArgs e);

    public interface IScanStation
    {
        string Version { get; set; }

        string Title { get; set; }

        string Station { get; set; }

        /// <summary>
        /// This event is fired if a barcode is received.
        /// </summary>
        event BarcodeReceivedDelegate BarcodeReceivedEvent;

        /// <summary>
        /// This event is fired if a barcode is received to make sanity checks on the barcode
        /// </summary>
        event BarcodeProveDelegate BarcodeProveRequestEvent;
    }
}
