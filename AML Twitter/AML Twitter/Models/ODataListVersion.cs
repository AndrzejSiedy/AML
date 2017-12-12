using System;

namespace AML.Twitter.Models
{
    public class ODataListVersion: ODataListBase
    {
        public int ListId { get; set; }
        public string VersionRef { get; set; }
        public int? PreviousListVersionId { get; set; }
        public int? NextListVersionId { get; set; }
        public int Added { get; set; }
        public int Modified { get; set; }
        public int Removed { get; set; }
        public int TotalNumberOfRecords { get; set; }
        public int WatchListPollId { get; set; }
        public DateTime ContextDate { get; set; }
        public bool IsSATGenerated { get; set; }
    }
}
