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

namespace titan.core{
    public class localization {
        public localization () { }
        public data_set get() {
            return db.fetch_all("titan","SELECT * FROM titanDWS_localization where [active]=1 AND [method]='config' AND [group]='titan' ORDER BY [order] asc, sub_order asc");
        }
        public data_set get(string group,string method) {
            return db.fetch_all("titan","SELECT * FROM titanDWS_localization where [active]=1 AND [method]=@method AND [group]=@group ORDER BY [order] asc, sub_order asc",new parameters().add("group",group).add("method",method));;

        }
    }//end localization
}//end namespace