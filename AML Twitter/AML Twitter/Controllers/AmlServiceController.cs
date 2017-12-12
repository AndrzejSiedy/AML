using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AML.Twitter.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AML.Twitter.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class AmlServiceController : Controller
    {

        private readonly IAmlServiceWatcher _amlServiceWatcher;

        public AmlServiceController(IAmlServiceWatcher amlServiceWatcher)
        {
            _amlServiceWatcher = amlServiceWatcher;
        }

        [HttpPost("[action]")]
        public void StartAmlService() => Task.Run(async () =>
        {
            await _amlServiceWatcher.StartServiceAsync();
        });


        [HttpPost("[action]")]
        public void StopAmlService() => Task.Run(async () =>
        {
            await _amlServiceWatcher.StopServiceAsync();
        });
    }
}