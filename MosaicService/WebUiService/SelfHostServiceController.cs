using System;
using System.ComponentModel.Composition;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.Logging;
using VP.FF.PT.Common.Infrastructure.Logging;

namespace Common.WebUiService
{
    [Export(typeof(IWebUiService))]
    public class SelfHostServiceController:IWebUiService
    {
        private IDisposable _server = null;
        private bool _traceEnable = false;

        [Import]
        private VP.FF.PT.Common.Infrastructure.Logging.ILogger _logger;

        public void Start(int port, bool traceEnable)
        {
            _traceEnable = traceEnable;

            try
            {
                _server = WebApp.Start<Startup>(string.Format("http://+:{0}/", port));
            }
            catch (Exception e)
            {
                _logger?.ErrorFormat("WebApp.Start failed: {0}",e.Message);
            }
        }

        public void Stop()
        {
            _server.Dispose();
        }
    }
}
