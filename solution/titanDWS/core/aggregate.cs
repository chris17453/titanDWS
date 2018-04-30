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

namespace titan.core{
    public class aggregate {
        public string name="";
        public string alias="";
        public string display {
            get {
                if (String.IsNullOrWhiteSpace(alias)) return name;
                else return alias;
            } set { } }
        public int order=0;
        public string htmlName { get { return "Titan_Aggregate_"+this.name; } set { } }
        public aggregate(string name,string alias,int order) {
            this.order=order;
            this.name=name;
            this.alias=alias;
        }
    }//end class
}//end namespace            