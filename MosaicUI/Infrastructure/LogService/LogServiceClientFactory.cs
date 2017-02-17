using System.ComponentModel.Composition;
using VP.FF.PT.Common.GuiEssentials.Wcf;

namespace Cimpress.ACS.FP3.UIInfrastructure.LogService
{
    /// <summary>
    /// The <see cref="LogServiceClientFactory"/> is capable of creating a 
    /// <see cref="LogServiceClient"/> instance.
    /// </summary>
    [Export(typeof(ICommunicationObjectFactory<ILogService>))]
    public class LogServiceClientFactory : ICommunicationObjectFactory<ILogService>
    {
        /// <summary>
        /// Creates a communication object communicating with a <see cref="ILogService"/> instance.
        /// </summary>
        /// <returns>A communication object implementing the <see cref="ILogService"/> interface of the service it communicates with.</returns>
        public ILogService CreateCommunicationObject()
        {
            return new LogServiceClient();
        }
    }
}