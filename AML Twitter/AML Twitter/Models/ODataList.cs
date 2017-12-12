namespace AML.Twitter.Models
{
    public class ODataList: ODataListBase
    {
        public string Name {get;set;}
        public int LookupSourceId { get; set; }
        public string LookupSourceName { get; set; }
        public string ShortCode { get; set; }
        
    }
}
