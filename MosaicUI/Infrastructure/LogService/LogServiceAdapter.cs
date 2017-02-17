using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VP.FF.PT.Common.GuiEssentials.Wcf;
using VP.FF.PT.Common.Infrastructure.Logging;
using VP.FF.PT.Common.PlatformEssentials.LogInformation;
using IProvideLogMessages = VP.FF.PT.Common.WpfInfrastructure.Screens.Services.IProvideLogMessages;

namespace Cimpress.ACS.FP3.UIInfrastructure.LogService
{
    /// <summary>
    /// The <see cref="LogServiceAdapter"/> provides log messages which it
    /// receives from sabers <see cref="ILogService"/> WCF service.
    /// </summary>
    public class LogServiceAdapter : IProvideLogMessages
    {
        private readonly IClient<ILogService> _client;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new <see cref="LogServiceAdapter"/> instance.
        /// </summary>
        /// <param name="clientFactory">An instance capable of creating a wcf client.</param>
        /// <param name="logger">The logger.</param>
        public LogServiceAdapter(
            IClientFactory<ILogService> clientFactory,
            ILogger logger)
        {
            _client = clientFactory.CreateClient();
            _logger = logger;
        }

        /// <summary>
        /// Gets the log messages which were emitted by the specified <paramref name="emitters"/>.
        /// </summary>
        /// <param name="emitters">The emitters to get the log messages from.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of strings.</returns>
        public async Task<IEnumerable<string>> GetMessages(IEnumerable<string> emitters)
        {
            string[] emittersArray = emitters.ToArray();
            _logger.InfoFormat("Requesting messages of emitters '{0}' from log service", string.Join(", ", emittersArray));
            LogMessageDto[] dtos = await _client.InvokeOnService(s => s.GetLogMessagesFromEmittersAsync(emittersArray));
            _logger.InfoFormat("Received '{0}' messages from log service", dtos.Length);
            return dtos.Select(d => d.Text);
        }
    }
}