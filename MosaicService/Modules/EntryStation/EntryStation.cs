using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VP.FF.PT.Common.GenericScanStation.Interface;
using VP.FF.PT.Common.GenericScanStation.RestInterfaces;
using VP.FF.PT.Common.Infrastructure;
using VP.FF.PT.Common.Infrastructure.Logging;
using VP.FF.PT.Common.PlatformEssentials;
using VP.FF.PT.Common.PlatformEssentials.Entities;
using MosaicSample.Infrastructure.Workflow;
using VP.FF.PT.Common.PlatformEssentials.HardwareAbstraction;

namespace MosaicSample.EntryStation
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class EntryStation : PlatformModule, IPartImportsSatisfiedNotification
    {
        private bool _importsSatisfied;
        private IScanStation _scanStation;
        private readonly ILogger _logger;
        private readonly IMosaicSampleWorkflowFactory _workflowFactory;
        private readonly WorkflowProvider _workflowProvider;
        public event EventHandler<BarcodeEventArgs> BarcodeEventHandler;

        public IJunction Junction { get; set; }

        [ImportingConstructor]
        public EntryStation(ILogger logger, IMosaicSampleWorkflowFactory workflowFactory, WorkflowProvider workflowProvider)
        {
            _logger = logger;
            _workflowFactory = workflowFactory;
            _workflowProvider = workflowProvider;
        }

        public override void ActivateModule()
        {
            base.ActivateModule();

            ScanStationSinglton.Version = "1.0.0.0";
            ScanStationSinglton.Title = "Mosaic Demo";
            ScanStationSinglton.Station = "Entry Scan";
            ScanStationSinglton.ScanStationCreatedEvent += ScanStationCreatedHandler;

            //autostart module
            Start();
        }

        public override void Start()
        {
            base.Start();
            State = PlatformModuleState.Run;
        }

        public override void Stop()
        {
            base.Stop();
            State = PlatformModuleState.Off;
        }

        public void Shutdown()
        {
            ScanStationSinglton.ScanStationCreatedEvent -= ScanStationCreatedHandler;

            if (_scanStation != null)
            {
                _scanStation.BarcodeProveRequestEvent -= BarcodeSanityCheckHandler;
                _scanStation.BarcodeReceivedEvent -= BarcodeReceivedHandler;
            }
        }

        public override async Task Initialize(CancellationToken token)
        {
            State = PlatformModuleState.Initializing;
            token.ThrowIfCancellationRequested();
            await base.Initialize(token);
        }

        public void OnImportsSatisfied()
        {
            if (!_importsSatisfied)
            {
                _importsSatisfied = true;
                AddEquipment(Junction);
            }
        }

        private bool CreateEntity(string barcode, Workflow workflow)
        {
            var item = new PlatformItem
            {
                DetectedInModuleCount = 1,
                DetectedCount = 1,
                ItemId = HashHelper.ConvertStringToLong(barcode),
                LastDetectionTime = DateTime.Now,
                Route = _workflowFactory.CreateRecipe()
            };

            _workflowFactory.CreateRecipe(item.Route, workflow);

            try
            {
                AddItem(item);
                EventAggregator.Publish(new PlatformItemEvent(item.ItemId, this, PlatformItemEventType.NewItemCreated) { NewItem = item });
            }
            catch (Exception exception)
            {
                _logger.Error($"Creating PlatformItem Failed. {exception.Message}");
                return false;
            }
            
            OnBarcodeEventHandler(new BarcodeEventArgs(barcode));

            return true;
        }

        public override void AddItemRouting(PlatformItem item, int outputPortIndex)
        {
            base.AddItemRouting(item, outputPortIndex);
            Junction.RouteItem(item, outputPortIndex);
        }

        private BarcodeReceivedFeedback BarcodeReceivedHandler(object sender, BarcodeEventArgs e)
        {
            _logger.DebugFormat("Barcode Received: {0}", e.Barcode);

            var workflowsCollection = _workflowProvider.WorkflowCollection;
            var workflow = workflowsCollection.Workflows.FirstOrDefault(x => x.Barcode.Equals(e.Barcode));

            if (workflow == null)
            {
                _logger.ErrorFormat("Barcode ({0}) could not be found", e.Barcode);
                return new BarcodeReceivedFeedback
                {
                    Result = ScanStationErrors.Failed,
                    Data = new BarcodeResultJsonType
                    {
                        Barcode = e.Barcode,
                        Message = "Barcode not found"
                    }
                };
            }

            if (!CreateEntity(e.Barcode, workflow))
            {
                return new BarcodeReceivedFeedback
                {
                    Result = ScanStationErrors.Failed,
                    Data = new BarcodeResultJsonType
                    {
                        Barcode = e.Barcode,
                        Message = "Job generation Failed"
                    }
                };
            }

            return new BarcodeReceivedFeedback
            {
                Result = ScanStationErrors.Success,
                Data = new BarcodeResultJsonType
                {
                    Barcode = e.Barcode,
                    Message = "Well done my great Operator"
                }
            };
        }

        private ScanStationErrors BarcodeSanityCheckHandler(object sender, BarcodeEventArgs e)
        {
            _logger.DebugFormat("Barcode Sanity Check: {0}", e.Barcode);

            if (!string.IsNullOrEmpty(e.Barcode) && (e.Barcode.Length < 10))
            {
                ulong tmp;
                if (!ulong.TryParse(e.Barcode, out tmp))
                    return ScanStationErrors.Failed;
    
                return ScanStationErrors.Success;
            }

            return ScanStationErrors.Failed;
        }

        private void ScanStationCreatedHandler(object sender, ApiCreationEventArg e)
        {

            _scanStation = e.ScanStation;

            if (_scanStation != null)
            {
                _scanStation.BarcodeProveRequestEvent += BarcodeSanityCheckHandler;
                _scanStation.BarcodeReceivedEvent += BarcodeReceivedHandler;
            }
        }

        protected virtual void OnBarcodeEventHandler(BarcodeEventArgs e)
        {
            BarcodeEventHandler?.Invoke(this, e);
        }
    }
}
