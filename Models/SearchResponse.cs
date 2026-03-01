    using System.ComponentModel.DataAnnotations;

    namespace KMITL_WebDev_MiniProject;

    public class SearchResponse
    {
        public string Message {get; set;}
        public List<Object> Activity {get; set;}
    }