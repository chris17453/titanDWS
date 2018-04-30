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
using System.Configuration;
using nocodedb.data;
using nocodedb.data.models;

namespace titan.core {

    //this class transforms any data returned from MSSQL to the correct format for display.
    public class data_types {
        data_set types=null;
        public data_types() {
        }

        public void init() {
            types=db.fetch_all("titan","SELECT display,name,format,reference1,reference2,reference3,reference4 FROM titanDWS_data_types  WHERE active='1' order by [order]");
        }

       
        public string transform_data(string type,string data,query.column c) {
            if(null==types || types.Count==0) {
                init();
                return data;
             }
            foreach(row data_type in types.rows) {
                if(((string)data_type["name"]).ToLower()==type.ToLower()) {                     //if this is the data type.. lets transform it!
                    if(String.IsNullOrWhiteSpace((string)data_type["format"])) return data;     //if no format then return data
                    string format= (string)data_type["format"];
                    string d1="";
                    string d2="";
                    string d3="";
                    string d4="";
                    if(null!=data_type["reference1"]) d1=get_data((string)data_type["reference1"],data,c);
                    if(null!=data_type["reference2"]) d2=get_data((string)data_type["reference2"],data,c);
                    if(null!=data_type["reference3"]) d3=get_data((string)data_type["reference3"],data,c);
                    if(null!=data_type["reference4"]) d4=get_data((string)data_type["reference4"],data,c);
                    return String.Format(format,data,d1,d2,d3,d4);
                }
            }
            return data;
        }

        public string get_data(string data_reference,string data,query.column c) {
            string output="";
            string[] tokens=data.Split('|');

            switch (data_reference.ToLower()) {
                case "d1"               : if(tokens.Length<=1) output=tokens[0]; break;
                case "d2"               : if(tokens.Length<=2) output=tokens[1]; break;
                case "d3"               : if(tokens.Length<=3) output=tokens[2]; break;
                case "d4"               : if(tokens.Length<=4) output=tokens[3]; break;
                case "titan_report"     : output="/"; break;
                case "titan_url"        : output=ConfigurationManager.AppSettings["titan_api_url"] +"titan/"; break;
                case "column_type"      : output=c.type; break;
                case "column_name"      : output=c.name; break;
                case "column_display"   : output=c.display; break;
                case "*"                : output=data; break;
                default: output=data_reference; break;
            }
            return output;
        }//end function
    }
}