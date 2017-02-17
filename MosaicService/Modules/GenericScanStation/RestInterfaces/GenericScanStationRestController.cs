using System;
using System.ComponentModel.Composition;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VP.FF.PT.Common.GenericScanStation.Interface;

namespace VP.FF.PT.Common.GenericScanStation.RestInterfaces
{
    [Export(typeof(IScanStation))]
    public class GenericScanStationRestController : ApiController, IScanStation
    {
        private VersionJsonType _version = new VersionJsonType();
        private TitleJsonType _title = new TitleJsonType();

        private object _lock = new object();

        public string Version {
            get { return _version.Version; }
            set { _version.Version = value; }
        }

        public string Title
        {
            get { return _title.Title; }
            set { _title.Title = value; }
        }

        public string Station
        {
            get { return _title.Station; }
            set { _title.Station = value; }
        }

        public event BarcodeReceivedDelegate BarcodeReceivedEvent;
        public event BarcodeProveDelegate BarcodeProveRequestEvent;

        protected GenericScanStationRestController() : base()
        {
            Version = ScanStationSinglton.Version;
            Title = ScanStationSinglton.Title;
            Station = ScanStationSinglton.Station;

            ScanStationSinglton.ScanStation = this;
        }

        [HttpGet]
        [Route("rest/barcode/version")]
        public IHttpActionResult GetVersion()
        {
            if (string.IsNullOrEmpty(Version))
            {
                return BadRequest();
            }

            return Ok(JsonConvert.SerializeObject(_version));
        }

        [HttpGet]
        [Route("rest/barcode/title")]
        public IHttpActionResult GetTitle()
        {
            return Ok(JsonConvert.SerializeObject(_title));
        }

        [HttpPost]
        [Route("rest/barcode")]
        public IHttpActionResult PostBarcode(JObject barcode)
        {
            lock (_lock)
            {

                if (barcode != null)
                {
                    try
                    {
                        BarcodeJsonType message = JsonConvert.DeserializeObject<BarcodeJsonType>(barcode.ToString());

                        if (OnBarcodeProveRequest(new BarcodeEventArgs(message.Barcode)) == ScanStationErrors.Success)
                        {
                            var result = OnBarcodeReceived(new BarcodeEventArgs(message.Barcode));

                            if (result.Result == ScanStationErrors.Success)
                            {
                                result.Data.Result = (int) ScanStationErrors.Success;

                                return Ok(JsonConvert.SerializeObject(result.Data));
                            }

                            result.Data.Result = (int)ScanStationErrors.Failed;

                            return Ok(JsonConvert.SerializeObject(result.Data));
                        }
                    }
                    catch (Exception e)
                    {
                        return Ok(JsonConvert.SerializeObject(new BarcodeResultJsonType() {Barcode = "", Message = "Failed", Result = (int)ScanStationErrors.Failed}));
                    }
                }

                return Ok(JsonConvert.SerializeObject(new BarcodeResultJsonType() { Barcode = "", Message = "Failed", Result = (int)ScanStationErrors.BarcodeMissing }));
            }
        }

        private BarcodeReceivedFeedback OnBarcodeReceived(BarcodeEventArgs e)
        {
            var handler = BarcodeReceivedEvent;

            if (handler != null)
            {
                return handler(this, e);
            }

            return new BarcodeReceivedFeedback() {Result = ScanStationErrors.Failed, Data = new BarcodeResultJsonType() {Barcode = e.Barcode, Message = "Barcode receiver missing - Check source code"} };
        }

        private ScanStationErrors OnBarcodeProveRequest(BarcodeEventArgs e)
        {
            var handler = BarcodeProveRequestEvent;

            if (handler != null)
            {
                return handler(this, e);
            }

            return ScanStationErrors.Success;
        }

    }
}
