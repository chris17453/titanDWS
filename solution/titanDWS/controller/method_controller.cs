/******************************************************************** 
  ████████╗██╗████████╗ █████╗ ███╗   ██╗██████╗ ██╗    ██╗███████╗
  ╚══██╔══╝██║╚══██╔══╝██╔══██╗████╗  ██║██╔══██╗██║    ██║██╔════╝
     ██║   ██║   ██║   ███████║██╔██╗ ██║██║  ██║██║ █╗ ██║███████╗
     ██║   ██║   ██║   ██╔══██║██║╚██╗██║██║  ██║██║███╗██║╚════██║
     ██║   ██║   ██║   ██║  ██║██║ ╚████║██████╔╝╚███╔███╔╝███████║
     ╚═╝   ╚═╝   ╚═╝   ╚═╝  ╚═╝╚═╝  ╚═══╝╚═════╝  ╚══╝╚══╝ ╚══════╝
*********************************************************************
- Created: 01-23-2017
- Author : Charles Watkins
- Email  : chris17453@gmail.com
********************************************************************/
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Web.Http;
using static titan.models;

namespace titan.controller {
    [RoutePrefix("titan")]
    public class methodController : ApiController {
        //security.titan_token token;

        internal string JWT {
            get{ 
                if(this.ControllerContext.Request.Headers.Contains("JWT")) {
                    IEnumerable<string> headerValues = this.ControllerContext.Request.Headers.GetValues("JWT");
                    foreach (string s in  headerValues) {
                        return s;
                    }
                }            
                return String.Empty;
            }
        }

        [Route("cron")]
        public string cron() {
            web_service w=new web_service();
            w.run_crons();
            return "";
        }

        public string ToHex(byte[] bytes, bool upperCase) {
            StringBuilder result = new StringBuilder(bytes.Length*2);
            for (int i = 0; i < bytes.Length; i++)
                result.Append(bytes[i].ToString(upperCase ? "X2" : "x2"));
            return result.ToString();
        }

        [Route("get_token")]
        [HttpPost]
        public string get_token(lambda i) {                                                         //all get tokens are verified by login params....
            if(null==i) return "";
            security.titan_token token=new security.titan_token();
            if(String.IsNullOrWhiteSpace(i.reference1 )) i.reference1="";
            if(String.IsNullOrWhiteSpace(i.reference2 )) i.reference2="";
            if(String.IsNullOrWhiteSpace(i.reference3 )) i.reference3="";
            if(String.IsNullOrWhiteSpace(i.reference4 )) i.reference4="";
            if(String.IsNullOrWhiteSpace(i.reference5 )) i.reference5="";
            if(String.IsNullOrWhiteSpace(i.reference6 )) i.reference6="";
            if(String.IsNullOrWhiteSpace(i.reference7 )) i.reference7="";
            if(String.IsNullOrWhiteSpace(i.reference8 )) i.reference8="";
            if(String.IsNullOrWhiteSpace(i.reference9 )) i.reference9="";
            if(String.IsNullOrWhiteSpace(i.reference10)) i.reference10="";
            if(String.IsNullOrWhiteSpace(i.reference11)) i.reference11="";
            if(String.IsNullOrWhiteSpace(i.reference12)) i.reference12="";
            if(String.IsNullOrWhiteSpace(i.reference13)) i.reference13="";
            if(String.IsNullOrWhiteSpace(i.reference14)) i.reference14="";
            if(String.IsNullOrWhiteSpace(i.reference15)) i.reference15="";

            byte[] hash=System.Security.Cryptography.MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(
                i.reference1+
                i.reference2+
                i.reference3+
                i.reference4+
                i.reference5+
                i.reference6+
                i.reference7+
                i.reference8+
                i.reference9+
                i.reference10+
                i.reference11+
                i.reference12+
                i.reference13+
                i.reference14+
                i.reference15
            ));
            string check=ToHex(hash,true);
            if(check!=i.check.Trim().ToUpper()) return "";      //this is a variable check. if the md5 doesnt match the 
                                                                //variables have been tampered with. otherwose we are good to go.

            
            return security.Encode(i.reference1,
                                   i.reference2,
                                   i.reference3,
                                   i.reference4,
                                   i.reference5,
                                   i.reference6,
                                   i.reference7,
                                   i.reference8,
                                   i.reference9,
                                   i.reference10,
                                   i.reference11,
                                   i.reference12,
                                   i.reference13,
                                   i.reference14,
                                   i.reference15);
        }//end get_token
        
        [Route("lambda")]
        [HttpPost]
        public json_results get(lambda i) {
            return security.wrapper(JWT,i,(token) => {
                bool no_links=false;
                bool init=false;
                if(i.configure=="preview") {
                    init=true;
                }
                web_service w=new web_service(i);
                method m=new titan.method();
                if(ConfigurationManager.AppSettings["titan_debug"]=="true") {
                    m.titan_debug=true;
                }
                method_results results=m.execute(i,no_links,token,init);
                return results;
            });
        }//end titan lambda

        [Route("fetch_method")]
        [HttpPost]
        public json_results fetch_method(lambda i) {
            return security.wrapper(JWT,i,(token) => {
                web_service w=new web_service(i);
                method results=w.fetch_method(i,token);
               
                if(null==results) {
                    results=new method(i,i.group,i.method,i.owner,i.configure,token);

                    results.generate_queries();
                    
                    if (null==results) {
                    //results="Failed to load empty method";
                    }    
                    return results;
                }
                return results;
            });
        }//end fetch_method

        [Route("save_method")]
        [HttpPost]
        public json_results save_method(lambda i) {
            return security.wrapper(JWT,i,(token) => {
                method m=method.from_json(i.crud,token,true);
                string res;
                if(m.data_schema==null || m.data_schema.Count==0) res="No columns in this query";
                else res=String.Format("Column count {0}",m.data_schema.Count);
                return res;
            });
        }//end titan lambda


        [Route("test_method")]
        [HttpPost]
        public json_results test_method(lambda i) {
            return security.wrapper(JWT,i,(token) => {
                method m=method.from_json(i.crud,token,false);
                string res;
                if(m.data_schema==null || m.data_schema.Count==0) res="No columns in this query";
                else res=String.Format("Column count {0}",m.data_schema.Count);
                return res;
            });
        }//end titan test method

        //strictly for monitoring the health of the service.
        [Route("heartbeat")]
        [HttpGet]
        public string heartbeat() {
            return DateTime.Now.ToString();
        }//end heartbeat

        [Route("export/{export_id1}/{export_id}/{name}.{type}")]
        [HttpGet]
        public void export(string export_id1,int export_id,string name,string type){               
            lambda i=new lambda();
            i.export_id=export_id;
            string web_token=string.Format("{0}",export_id1);
            //enable web service jwt validation
            web_service w=new web_service(i);
            w.export(i,web_token);          
        
        }//end export

    }
}