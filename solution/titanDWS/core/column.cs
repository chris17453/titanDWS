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
using System.Data.SqlClient;
using nocodedb.data;
using nocodedb.data.models;

namespace titan.core{
    public class column {
        public bool   is_identity               =false;
        public string name                      ="";
        public string alias                     ="";
        public bool   export                    =true;      //can this be exported to CSV
        public bool   sort                      =true;      //can this be sorted
        public bool   sortDefault               =false;     //is this a default sort column
        public bool   sort_default_order_asc    =true;      //sort direction is it asc?
        public bool   filter                    =true;      //can you search this column
        public string type                      ="text";
        public int    order                     =0;
        public int    groupOrder                =0;
        public string width                     ="";
        public bool   visible                   =true;
        public string desc;
        public bool   custom_aggregate          =false;
        public string base_type                 ="";
        public string selectData                ="";
        public bool   key                       =false; 
        public bool   combined_search           =false;
        public bool   fixed_width               =false;     
        public bool   overflow                  =false;     
        public bool   options_locked            =false;
                
        public List<select_option> selectOptions=new List<select_option>();
        // public bool   custom_parameter=false;
        public string display {
        get {
            if (String.IsNullOrWhiteSpace(alias)) return name;
            else return alias;
        } set { } }
   
        public column() {
            buildOptions();
        }
        public column(string name,string type,int order,bool visible) {
            this.name=name;
            this.type=type;
            this.order=order;
            this.visible=visible;
            buildOptions();
        }


        public void buildOptions(parameters parameters=null) {
            selectOptions.Clear();
            if(selectData.ToUpper().Contains("SELECT")) {
               data_set  results=db.fetch_all("",selectData,parameters);
               foreach(row res in results) {
                    if(results.ContainsKey("display") && results.ContainsKey("value") && results.ContainsKey("selected")) {
                        string v1=res["display"];
                        string v2=res["value"];
                        bool   v3=res["selected"];
                        selectOptions.Add(new select_option(v1,v2,v3));
                    }
               }
            } else {
                string[] options=selectData.Split(',');
                if(options.Length>0)
                foreach(string option in options){
                    string[]tokens=option.Split('|');
                    if(tokens.Length==1 && !String.IsNullOrWhiteSpace(tokens[0])) selectOptions.Add(new select_option(tokens[0],tokens[0],tokens[0][0]=='*'?true:false));
                    if(tokens.Length>=2 && !String.IsNullOrWhiteSpace(tokens[0]) 
                                        && !String.IsNullOrWhiteSpace(tokens[1])) selectOptions.Add(new select_option(tokens[0],tokens[1],tokens[0][0]=='*'?true:false));
                }
            }
        }
        public string htmlName {
            get {
                string o="";
                o="TitanColumn_"+this.name.ToLower();
                return o;
            }
            set { }
        }
    }//end class
}//end namespace