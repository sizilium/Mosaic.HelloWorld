using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using VP.FF.PT.Common.Infrastructure;
using VP.FF.PT.Common.Infrastructure.Logging;
using VP.FF.PT.Common.PlatformEssentials;
using VP.FF.PT.Common.PlatformEssentials.Entities;
using VP.FF.PT.Common.PlatformEssentials.HardwareAbstraction.ItemTracking;
using VP.FF.PT.Common.Utils.Security;

namespace MosaicSample.FinishingStationModule
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class FinishingStationModule : PlatformModule, IPartImportsSatisfiedNotification
    {
        private readonly ILogger _logger;
        //todo make the following two items visible on the UI
        private string ZebraPrinterIP { get; set; } = "0.0.0.0";

        public static string ZplTemplateFile { get; set; } =
            "MosaicSample.FinishingStationModule.Resources.ZPL_Template.xml";

        internal Action<string> ZebraIpChanged = t => { };
        internal Action<string> ZplTemplateChanged = t => { };

        public IBarcodeReader BarcodeReader { get; set; }
        private bool _importsSatisfied;

        [ImportingConstructor]
        public FinishingStationModule(ILogger logger)
        {
            _logger = logger;
        }

        public override async Task Initialize(CancellationToken token)
        {
            State = PlatformModuleState.Initializing;
            token.ThrowIfCancellationRequested();
            await base.Initialize(token);

            RaiseZebraIpChangedEvent();
            RaiseZplTemplateChanged();
        }

        public override void ActivateModule()
        {
            base.ActivateModule();

            // autostart
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

        public void UpdateZebraPrinterIp(string ipAddress)
        {
            ZebraPrinterIP = ipAddress;
            RaiseZebraIpChangedEvent();
        }

        public void UpdateZplTemplateUrl(string url)
        {
            ZplTemplateFile = url;
            RaiseZplTemplateChanged();
        }

        private string ReadAndEncodeTemplate(Stream filestream)
        {
            using (StreamReader reader = new StreamReader(filestream))
            {
                var template = reader.ReadToEnd();
                return template.EncodeBase64();
            }
        }

        private void PrintLabel(List<Field> fields )
        {
            var assembly = Assembly.GetExecutingAssembly();
            var file = assembly.GetManifestResourceStream(ZplTemplateFile);

            var encodedtext = ReadAndEncodeTemplate(file);
            var zpl = LabelService.NewRequest(encodedtext, fields, _logger);
            
            ZebraPrinter printer = new ZebraPrinter(ZebraPrinterIP, 9100);
            printer.PrintLabel(zpl.DecodeBase64());
        }

        protected internal void RaiseZebraIpChangedEvent()
        {
            if(EventRaiser != null)
                EventRaiser.Raise(ref ZebraIpChanged, ZebraPrinterIP);
        }

        protected internal void RaiseZplTemplateChanged()
        {
            if (EventRaiser != null)
                EventRaiser.Raise(ref ZplTemplateChanged, ZplTemplateFile);
        }

        private List<Field> ComposeLabelFields(PlatformItem item)
        {
            //Start with a predefined label
            var fields = GetEmptyFieldsListForDemonstrator();

            //overwrite the barcode
            fields[0] = new Field("TrackingIdBarcode", item.ItemId.ToString(), "string");

            //ToDo: collect data from item.LogHistory and print it
            //var item = Entities.PlatformItems.FirstOrDefault();
            //var recipeList = item.Recipe.GetOrderedList();

            if (item.Route == null)
                return fields;

            var recipeItems = item.Route.GetOrderedList();

            for (int i = 0; i < Math.Min(recipeItems.Count, fields.Count) ; i++)
            {
                if (!recipeItems[i].ModuleType.ToString().IsNullOrEmpty())
                    fields[i].value = recipeItems[i].ModuleType.ToString();
            }

            return fields;
        }

        /// <summary>
        /// dummy code for demonstrator, to populate a matching number of fields that are needed for the used ZPL template
        /// </summary>
        /// <returns></returns>
        private List<Field> GetEmptyFieldsListForDemonstrator()
        {
            List<Field> fields = new List<Field>()
            {
                new Field("TrackingIdBarcode", "00000000", "string"),
                new Field("STEP1", "none", "string"),
                new Field("STEP2", "none", "string"),
                new Field("STEP3", "none", "string"),
                new Field("STEP4", "none", "string"),
                new Field("STEP5", "none", "string"),
                new Field("STEP6", "none", "string"),
                new Field("STEP7", "none", "string"),
            };

            return fields;
        }

        public override void AddItem(PlatformItem item)
        {
            base.AddItem(item);
            var fieldList = ComposeLabelFields(item);
            PrintLabel(fieldList);
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
