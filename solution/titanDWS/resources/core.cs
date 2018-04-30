using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;

namespace titan.resources {
    public class core {
        public static HttpResponseMessage  EmbededResource(string path,string type,string[][] rewrite=null)  {
            var response = new HttpResponseMessage(HttpStatusCode.OK);
            var asm = Assembly.GetExecutingAssembly();
            using (var stream = asm.GetManifestResourceStream(path)){
                if (stream != null){
                    var reader = new StreamReader(stream);
                    if(null!=rewrite ) {
                        string document=reader.ReadToEnd();
                        foreach(string [] g in rewrite) {
                            document=document.Replace(g[0],g[1]);
                        }
                        response.Content =new StringContent(document);
                    } else {
                        response.Content =new StringContent(reader.ReadToEnd());
                    }
                }
            }
            if(null==response.Content) response.Content=new StringContent(String.Empty);
            mime_type m=new mime_type(type);
            response.Content.Headers.ContentType=new MediaTypeHeaderValue(m.mime);
            response.Content.Headers.ContentType.CharSet = Encoding.UTF8.HeaderName;
            response.Content.Headers.Add("CodePage", Encoding.UTF8.CodePage.ToString());
            return response;
        } //end function   
    }//end class
}//end namespace