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
        public IActionResult StartAmlService()
        {
            // in this scenario we don't really check what is going on with services
            // it is fire & forget for this implementation
            _amlServiceWatcher.StartServiceAsync();
            return Ok();
        }

        [HttpPost("[action]")]
        public IActionResult StopAmlService()
        {
            // in this scenario we don't really check what is going on with services
            // it is fire & forget for this implementation
            _amlServiceWatcher.StopServiceAsync();
            return Ok();
        }
    }
}