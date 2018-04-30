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
using System.Collections;
using System.Collections.Generic;
using nocodedb.data;
using nocodedb.data.models;

namespace titan.core {
    public class linked_method {
        private string name       ="";
        private string group      ="";
        private string owner      ="";
        private string title      ="";
        private string description="";
        public string uid {
            get { return String.Format("[{3}]-[{0}]-[{1}]-[{2}]",group,name,owner,title); }
            set { }
        }

        public static List<linked_method> get() {
            data_set results=db.fetch_all("titan","SELECT [title],[method],[group],[owner],[description] FROM titanDWS WHERE [active]=1 ORDER BY [order],[group],[method],[owner] ");
            List<linked_method> methods=new List<linked_method>();
            linked_method def=new linked_method();
            def.title="CHOOSE Method";
            methods.Add(def);
            foreach (row result in results.rows) {
                linked_method lm=new linked_method();
                lm.title        =(string)result["title"];
                lm.name         =(string)result["method"];
                lm.group        =(string)result["group"];
                lm.owner        =(string)result["owner"];
                lm.description  =(string)result["description"];
                methods.Add(lm);
            }
            return methods;
        }
    }
}