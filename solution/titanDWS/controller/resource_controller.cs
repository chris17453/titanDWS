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
using System.Configuration;
using System.Net.Http;
using System.Web.Http;

namespace titan.controller{
    [RoutePrefix("titan")]    
    public class resourceController:ApiController{
        public string server_url { get {
                string url=ConfigurationManager.AppSettings["titan_api_url"];
                if(String.IsNullOrWhiteSpace(url)) {
                    url=this.ControllerContext.Request.RequestUri.AbsoluteUri;
                    int i=url.IndexOf("titan",StringComparison.CurrentCulture);
                    if(i>0) {
                        url=url.Substring(0,i);
                    }
                }
                url+="titan/";
                return url;

            } }
        [Route("")]
        [HttpGet]
        public HttpResponseMessage index() {
            string path=@"titan.index.html";
            return resources.core.EmbededResource(path,"html");
        }
        [Route("config/{group}/{method}/{owner?}/")]
        [HttpGet]
        public HttpResponseMessage config(string @group,string method,string owner="0") {
            string [][] d=new string [][]{ new string[]{"{method}",method},new string[]{"{group}",@group},new string[]{"{owner}",owner},
                new string []{"{titan_url}",server_url}};
            string path=@"titan.web_pages.config.html";
            return resources.core.EmbededResource(path,"html",d);
        }
        [Route("preview/{group?}/{method?}/{owner?}/")]
        [HttpGet]
        public HttpResponseMessage preview(string @group="titan",string method="list",string owner="0") {
            string [][] d=new string [][]{ new string[]{"{method}",method},new string[]{"{group}",@group},new string[]{"{owner}",owner},
                new string []{"{titan_url}",server_url}};
            string path=@"titan.web_pages.preview.html";
            return resources.core.EmbededResource(path,"html",d);
        }

        [Route("titanDWS.js")]
        [HttpGet]
        public HttpResponseMessage titanDWS_js() {
            string [][] d=new string [][]{  new string []{"{titan_url}",server_url}};
            string path=@"titan.media.titan.titanDWS.js";
            return resources.core.EmbededResource(path,"js",d);
        }

        [Route("jquery.tablesorter.pager.js")]
        [HttpGet]
        public HttpResponseMessage tablesorter_pager_js() {
            string [][] d=new string [][]{  new string []{"{titan_url}",server_url}};
            string path=@"titan.media.tablesorter.js.extras.jquery.tablesorter.pager.js";
            return resources.core.EmbededResource(path,"js",d);
        }        
        [Route("jquery.tablesorter.js")]
        [HttpGet]
        public HttpResponseMessage tablesorter_js() {
            string [][] d=new string [][]{  new string []{"{titan_url}",server_url}};
            string path=@"titan.media.tablesorter.js.jquery.tablesorter.js";
            return resources.core.EmbededResource(path,"js",d);
        }        

        [Route("titanDWS.css")]
        [HttpGet]
        public HttpResponseMessage browsy_css() {
            string path=@"titan.media.titan.titanDWS.css";
            return resources.core.EmbededResource(path,"css");
        }
    }
}
