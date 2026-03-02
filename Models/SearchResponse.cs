    using System.ComponentModel.DataAnnotations;

    namespace KMITL_WebDev_MiniProject;

    public class SearchResponse
    {
        public string Message {get; set;} = "Initial mesage";
        public List<Object> Result_User {get; set;} = new List<object>();
        public List<Object> Result_Activity {get; set;}  = new List<object>();
    }