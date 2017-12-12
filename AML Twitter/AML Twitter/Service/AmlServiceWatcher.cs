using AML.Twitter.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
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


        /// <summary>
        /// state to monitor if service is running
        /// </summary>
        private bool _isRunning = false;

        /// <summary>
        /// when set to true, will stop calling AmlService
        /// </summary>
        private bool _shouldStopService = false;

        /// <summary>
        /// read only property to check if service is running (actually is marked as running)
        /// </summary>
        public bool IsRunning { get => _isRunning; }

        /// <summary>
        /// List conataing all records from last call for change discovery
        /// </summary>
        public List<HarvesterRecord> HarvesterRecords { get; set; }

        public AmlServiceWatcher(IOptions<AmlServiceSettings> amlServiceSettings)
        {
            _amlServiceSettings = amlServiceSettings;
            HarvesterRecords = new List<HarvesterRecord>();
        }

        /// <summary>
        /// Start AmlService if not running
        /// </summary>
        /// <returns></returns>
        public async Task StartServiceAsync() {
            _shouldStopService = false;
            await CallService();
        }


        /// <summary>
        /// Disable service
        /// </summary>
        /// <returns></returns>
        public async Task StopServiceAsync()
        {
            await Task.Run(() => {
                _shouldStopService = true;
            });
            
        }

        /// <summary>
        /// Acctual calls to AML services to check for updates and send Twitter notification
        /// </summary>
        /// <returns></returns>
        private async Task _CallService()
        {
            var listVersion = await CallOData<ODataListVersion>(_amlServiceSettings.Value.AmlListVersionUrl);
            var harvestData = await CallOData<HarvesterRecord>(string.Format(_amlServiceSettings.Value.AmlHarvesterUrl, 17681));

            // self call to get latest from services and if required do Twitter notification
            await Task.Delay(_amlServiceSettings.Value.ServiceCallInterval).ContinueWith(async _ => {
                await _CallService();
            });
        }

        /// <summary>
        /// Just initiates service call and Twitter notification if not running already
        /// </summary>
        /// <returns></returns>
        private async Task CallService()
        {
            // prevent next call to AmlServices
            if (_shouldStopService) return;

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

            // do not call services if marked as "should stop exeqution", http request already called will not get killed
            if (_shouldStopService) return output;

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
