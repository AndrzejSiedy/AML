using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AML.Twitter.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace AML.Twitter.Controllers
{
    public class HomeController : Controller
    {
        private readonly IAmlServiceWatcher _amlServiceWatcher;
        public HomeController(IAmlServiceWatcher amlServiceWatcher)
        {
            _amlServiceWatcher = amlServiceWatcher;
        }

        public IActionResult Index()
        {

            Task.Run(async () => {
                await _amlServiceWatcher.CallService();
            });

            return View();
        }

        public IActionResult Error()
        {
            ViewData["RequestId"] = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            return View();
        }
    }
}
