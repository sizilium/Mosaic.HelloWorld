using System.ComponentModel.Composition;
using VP.FF.PT.Common.Infrastructure;
using VP.FF.PT.Common.PlatformEssentials;
using VP.FF.PT.Common.PlatformEssentials.Entities;
using VP.FF.PT.Common.PlatformEssentials.HardwareAbstraction.ItemTracking;

namespace MosaicSample.DemoModuleA
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DemoModuleA : PlatformModule, IPartImportsSatisfiedNotification
    {
        private bool _importsSatisfied;

        public IBarcodeReader BarcodeReader { get; set; }

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

        public void OnImportsSatisfied()
        {
            if (!_importsSatisfied)
            {
                _importsSatisfied = true;
                AddEquipment(BarcodeReader);

                BarcodeReader.BarcodeReceivedEvent +=
                    (sender, args) => RaisePlatformItemDetected(HashHelper.ConvertStringToLong(args.Barcode));
            }
        }
    }
}