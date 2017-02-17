using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VP.FF.PT.Common.GenericScanStation.RestInterfaces;

namespace VP.FF.PT.Common.GenericScanStation.Interface
{
    public delegate void ScanStationCreatedEvent(object sender, ApiCreationEventArg e);

    /// <summary>
    /// This class was needed to circumvent the fact that the ApiController class is only created with the first REST call to this interface
    /// Classes interrested in the ApiController can subscribe to the ScanStationCreatedEvent;
    /// ToDo: Check if it is possible to precreate the ApiController
    /// </summary>
    public static class ScanStationSinglton
    {
        private static IScanStation _scanStation;
        private static TitleJsonType _title = new TitleJsonType();
        private static VersionJsonType _version = new VersionJsonType();
        public static string Version
        {
            get { return _version.Version; }
            set { _version.Version = value; }
        }

        public static event ScanStationCreatedEvent ScanStationCreatedEvent;
        public static string Title
        {
            get { return _title.Title; }
            set { _title.Title = value; }
        }

        public static string Station
        {
            get { return _title.Station; }
            set { _title.Station = value; }
        }

        public static IScanStation ScanStation
        {
            get
            {
                return _scanStation;
            }
            set
            {
                if (value != null)
                {
                    _scanStation = value;

                    var handler = ScanStationCreatedEvent;

                    if (handler != null)
                    {
                        handler(null ,new ApiCreationEventArg(_scanStation));
                    }
                }
            }
        }
    }
}
