using AML.Twitter.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AML.Twitter.Service
{

    public class AmlServiceWatcher : IAmlServiceWatcher
    {

        private readonly IOptions<AmlServiceSettings> _amlServiceSettings;

        private static HttpClient client = new HttpClient();


        private bool _isRunning = false;

        public bool IsRunning { get => _isRunning; }

        public AmlServiceWatcher(IOptions<AmlServiceSettings> amlServiceSettings)
        {
            _amlServiceSettings = amlServiceSettings;
        }

        /// <summary>
        /// Acctual calls to AML services to check for updates and send Twitter notification
        /// </summary>
        /// <returns></returns>
        private async Task _CallService()
        {
            var listVersion = await CallOData<ODataListVersion>(_amlServiceSettings.Value.AmlListVersionUrl);
            var harvestData = await CallOData<HarvesrerRecord>(string.Format(_amlServiceSettings.Value.AmlHarvesterUrl, 17681));

            // self call to get latest from services and if required do Twitter notification
            await Task.Delay(_amlServiceSettings.Value.ServiceCallInterval).ContinueWith(async _ => {
                await _CallService();
            });
        }

        /// <summary>
        /// Just initiates service call and Twitter notification if not running already
        /// </summary>
        /// <returns></returns>
        public async Task CallService()
        {
            // if process already running, do nothing
            if (_isRunning) return;

            _isRunning = true;
            await _CallService();
        }
        
        // Semi generic method to call OData Services
        private async Task<ODataResponse<T>> CallOData<T>(string endUrl) where T: ODataSuperBase
        {

            string jsonString = string.Empty;
            var output = new ODataResponse<T>();

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, _amlServiceSettings.Value.AmlBaseUrl + endUrl);
            await client.SendAsync(request)
            .ContinueWith(async responseTask =>
            {
                var stream = await responseTask.Result.Content.ReadAsStreamAsync();
                jsonString = new StreamReader(new GZipStream(stream, CompressionMode.Decompress)).ReadToEnd();
            });

            try
            {
                output = JsonConvert.DeserializeObject<ODataResponse<T>>(jsonString);
            }
            catch
            {
                // swallow exception
                // TODO: Add logger
            }

            return output;
        }

    }
}
