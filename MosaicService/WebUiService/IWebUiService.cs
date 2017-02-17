using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Common.WebUiService
{
    public interface IWebUiService
    {
        /// <summary>
        /// Start the selfhosted webserver for the WebUI page and the Rest Interfaces
        /// </summary>
        /// <param name="port">Port to listen on</param>
        /// <param name="traceEnable">true = enable extended tracing</param>
        void Start(int port, bool traceEnable);

        /// <summary>
        /// Stop the selfhosted webserver
        /// </summary>
        void Stop();
    }
}
