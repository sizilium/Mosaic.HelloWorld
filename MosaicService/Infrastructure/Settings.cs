using System.ComponentModel.Composition;

namespace MosaicSample.Infrastructure.Properties
{
    [Export]
    public sealed partial class Settings
    {
        public Settings()
        {
            this.SettingChanging += this.SettingChangingEventHandler;
            this.SettingsSaving += this.SettingsSavingEventHandler;
        }

        private void SettingChangingEventHandler(object sender, System.Configuration.SettingChangingEventArgs e)
        {
            Save();
        }

        private void SettingsSavingEventHandler(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }
    }
}