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
using System.Collections;
using System.Collections.Generic;
using nocodedb.data;
using nocodedb.data.models;

namespace titan.core {
    public static class template {
        /*    public string id="";
            public string name="";
            public string parameters="";
            public string report="";
            public string theme="";
            public string javascript="";
            public string created="";
            public string modified="";
            */
        public static data_set get()
        {
            return db.fetch_all("titan", "SELECT [name] FROM titanDWS_templates WHERE [active]=1  ORDER BY [order]");
        }

        public static data_set get_default() {
            return db.fetch("titan","SELECT * FROM titanDWS_templates WHERE [active]=1 AND [name]='Default'");
        }
        public static data_set get(string name) {
            
            return db.fetch("titan","SELECT TOP 1 * FROM titanDWS_templates WHERE [active]=1 AND [name]=@name  ORDER BY [order]",
                            new parameters().add("name",name) );
        }
    }//end class
}//end namespace