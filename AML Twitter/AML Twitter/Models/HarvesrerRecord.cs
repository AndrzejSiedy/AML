namespace AML.Twitter.Models
{
    public class HarvesterRecord: ODataSuperBase
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string HarvesterRecordChangeType { get; set; }
        public string SourceReference { get; set; }
        public string Details { get; set; }
        public string DataType { get; set; }
    }
}
