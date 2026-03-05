    using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;

namespace KMITL_WebDev_MiniProject;

    public class SearchResponse
    {
        public Object Message {get; set;} = new {keyword = "All" , type = "All"};
        public List<Object> Result_User {get; set;} = new List<object>();
        public List<Object> Result_Activity {get; set;}  = new List<object>();
    }