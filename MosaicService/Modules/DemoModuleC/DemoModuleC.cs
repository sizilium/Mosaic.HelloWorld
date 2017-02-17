using System;
using System.ComponentModel.Composition;
using VP.FF.PT.Common.Infrastructure;
using VP.FF.PT.Common.Infrastructure.Logging;
using VP.FF.PT.Common.PlatformEssentials;
using VP.FF.PT.Common.PlatformEssentials.Entities;
using VP.FF.PT.Common.PlatformEssentials.HardwareAbstraction;
using VP.FF.PT.Common.PlatformEssentials.HardwareAbstraction.ItemTracking;

namespace MosaicSample.DemoModuleC
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DemoModuleC : PlatformModule, IPartImportsSatisfiedNotification
    {
        private bool _importsSatisfied;
        private readonly ILogger _logger;

        public IJunction Junction { get; set; }

        public IBarcodeReader BarcodeReader { get; set; }

        [ImportingConstructor]
        public DemoModuleC(ILogger logger)
        {
            _logger = logger;
            _logger.Init(GetType());
        }

        public override void ActivateModule()
        {
            // demo module starts automatically
            base.ActivateModule();
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

        public override void AddItemRouting(PlatformItem item, int outputPortIndex)
        {
            base.AddItemRouting(item, outputPortIndex);
            Junction.RouteItem(item, outputPortIndex);
        }

        public void OnImportsSatisfied()
        {
            if (!_importsSatisfied)
            {
                _importsSatisfied = true;
                AddEquipment(Junction);
                AddEquipment(BarcodeReader);

                BarcodeReader.BarcodeReceivedEvent += OnBarcodeDetected;
            }
        }

        private void OnBarcodeDetected(object sender, BarcodeEventArgs barcodeEventArgs)
        {
            var itemId = HashHelper.ConvertStringToLong(barcodeEventArgs.Barcode);
            RaisePlatformItemDetected(itemId);
        }
    }
}