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
    public class parameter {
        public string name="";
        public string alias="";
        public bool   sort=false;
        public string type="text";
        public int    order=0;
        public bool   visible=true;
        public string desc;
        public string _default="";
        public string query="";
        public string test="";
        public bool   hidden=false;
        public string display {
            get {
                if (String.IsNullOrWhiteSpace(alias)) return name;
                else return alias;
        } set { } }
   
        public parameter() {

        }
        public parameter(string name,string type,int order,bool visible) {
            this.name=name;
            this.type=type;
            this.order=order;
            this.visible=visible;
        }

    public parameter(string name,string alias,string type,string order,bool visible,string desc,string _default,string query,string test,bool hidden) {
            this.name=name;
            this.alias=alias;
            this.query=query;
            this.type=type;
            Int32.TryParse(order,out this.order);               
            this.visible=visible;
            this.desc=desc;
            this._default=_default;
            this.test=test;
            this.hidden=hidden;
        }
        public string htmlName {
            get {
                string o="";
                o="Titan_Parameter_"+this.name.ToLower();
                return o;
            }
            set { }
        }
    }//end class
}//end namespace