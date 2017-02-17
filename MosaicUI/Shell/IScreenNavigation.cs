using VP.FF.PT.Common.WpfInfrastructure.CaliburnIntegration;

namespace MosaicSample.Shell
{
    public interface IScreenNavigation
    {
        void NavigateToHome();

        void NavigateToScreen(IModuleScreen screen);

        void NavigateToScreen(string moduleKey);

        void NavigateBack();
    }
}