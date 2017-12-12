using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AML.Twitter
{
    public class AmlServiceSettings : IAmlServiceSettings
    {
        public int ServiceCallInterval { get; set; }
        public string AmlBaseUrl { get; set; }
        public string AmlListVersionUrl { get; set; }
        public string AmlHarvesterUrl { get; set; }
    }
}
