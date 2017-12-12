using System.Collections.Generic;

namespace AML.Twitter.Models
{
    public class ODataResponse<T>
    {
        public ODataResponse()
        {
            Value = new List<T>();
        }
        public List<T> Value { get; set; }
    }
}
