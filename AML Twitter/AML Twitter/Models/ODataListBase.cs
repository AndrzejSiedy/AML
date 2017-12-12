using System;

namespace AML.Twitter.Models
{
    public class ODataListBase: ODataSuperBase
    {
        public bool IsPublished { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }
        public string Status { get; set; }
        public Guid CreatedById { get; set; }
        public Guid ModifiedById { get; set; }
        public string Revision { get; set; }
        
    }
}
