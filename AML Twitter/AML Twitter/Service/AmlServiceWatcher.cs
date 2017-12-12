using AML.Twitter.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;

namespace AML.Twitter.Service
{

    class HarvesterRecordsComparer : IEqualityComparer<HarvesterRecord>
    {

        public bool Equals(HarvesterRecord c1, HarvesterRecord c2)
        {
            if (c1.PrimaryKey == c2.PrimaryKey && c1.HarvesterRecordChangeType == c2.HarvesterRecordChangeType)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(HarvesterRecord c)
        {
            return c.PrimaryKey.GetHashCode();
        }
    }

    /// <summary>
    /// Class to call AML service and monitor changes, when call Twitter service to publish changes to Twitter
    /// in Startup.cs class instantiated  as singleton, but should be converted to utilize threads, or run as system service, out of scope of this exercise
    /// </summary>
    public class AmlServiceWatcher : IAmlServiceWatcher
    {

        private readonly IOptions<AmlServiceSettings> _amlServiceSettings;
        private readonly IOptions<AmlTwitterConfig> _amlTwitterConfig;

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
        /// List containing all unique records since service run
        /// this should be stored somewhere but for this excercise we only care if there are any changes, to notify on Twitter
        /// </summary>
        private List<HarvesterRecord> _harvesterRecords { get; set; }

        public AmlServiceWatcher(IOptions<AmlServiceSettings> amlServiceSetting, IOptions<AmlTwitterConfig> amlTwitterConfig)
        {
            _amlServiceSettings = amlServiceSetting;
            _amlTwitterConfig = amlTwitterConfig;
            _harvesterRecords = new List<HarvesterRecord>();
        }

        /// <summary>
        /// Start AmlService if not running
        /// </summary>
        /// <returns></returns>
        public async Task StartServiceAsync() {

            _harvesterRecords.Clear();
            _shouldStopService = false;
            await _CallService();

            return;
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

            _harvesterRecords.Clear();
            return;
        }

        private void TwitterPublish(string msg)
        {
            Auth.SetUserCredentials(_amlTwitterConfig.Value.ConsumerKey, _amlTwitterConfig.Value.ConsumerSecret,
                _amlTwitterConfig.Value.AccessToken, _amlTwitterConfig.Value.AccessTokenSecret);
            var user = User.GetAuthenticatedUser();
            Tweet.PublishTweet(msg);
        }

        private void AddNewRecordsAndCallPolice(List<HarvesterRecord> recs)
        {
            try
            {
                // add only missing records
                var addedRecs = new List<HarvesterRecord>();
                recs.ForEach(r =>
                {
                    if (!_harvesterRecords.Exists(h => h.PrimaryKey == r.PrimaryKey))
                    {
                        addedRecs.Add(r);
                    }
                });

                if (addedRecs.Count > 0)
                {
                    // call Twitter service
                    _harvesterRecords.AddRange(addedRecs);
                    TwitterPublish($"Msg from AML, there has been an update on the sanctioned list. Number of updates: {addedRecs.Count}");
                }
            }
            catch(Exception e)
            {

            }
            
        }

        /// <summary>
        /// Acctual calls to AML services to check for updates and send Twitter notification
        /// </summary>
        /// <returns></returns>
        private async Task _CallService()
        {
            if (_shouldStopService) {
                _isRunning = false;
                return;
            };

            _isRunning = true;
            var listVersion = await CallOData<ODataListVersion>(_amlServiceSettings.Value.AmlListVersionUrl);

            for(var i = 0; i < listVersion.Value.Count; i++)
            {
                var l = listVersion.Value[i];
                var harvestResponse = await CallOData<HarvesterRecord>(string.Format(_amlServiceSettings.Value.AmlHarvesterUrl, l.PrimaryKey));
                if (harvestResponse != null)
                {
                    AddNewRecordsAndCallPolice(harvestResponse.Value);
                }
            }
            
            // self call to get latest from services and if required do Twitter notification
            await Task.Delay(_amlServiceSettings.Value.ServiceCallInterval).ContinueWith(async _ => {
                await _CallService();
            });

            return;
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
