using System.Net;
using System.Runtime.Serialization;

namespace titan {
    public partial class models{
        public class json_results {
            public bool    success=true;
            //public string  error_msg="";
            //public string  status="OK";
            //public string ip="";
            //public string user_agent="";
            public object  results;
            public string request_for="";
            
            public json_results() {

            }
        }//end json results
    }//end class models
}//end namespace