using AML.Twitter.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AML.Twitter.Service
{
    public interface IAmlServiceWatcher
    {

        List<HarvesterRecord> HarvesterRecords { get; set; }

        Task StartServiceAsync();
        Task StopServiceAsync();
    }
}