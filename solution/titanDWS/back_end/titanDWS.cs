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
using System.Data.SqlClient;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using static titan.models;
using titan.core;
using titan;
using System.Text;
using nocodedb.data;
using nocodedb.data.models;

namespace titan {


    public class web_service{
            public List<string> groups=new List<string>();
            public List<method> methods=new List<method>();
            method_results results=new method_results ();
            public lambda   input           = null;
            public string   titan_url    = "";
            public bool     titan_debug     = false;
            public method   m               = new method();
            //public List<Hashtable>      external_data_mapping=db.fetchAll("titan","SELECT name from titanDWS.dbo.external_mapping where active=1");
            public data_types data_types=new data_types();                
            public web_service() {
               init();
            }
          
            public web_service(lambda input){
                this.input=input;
                init();
            }

            public void init() {
                titan_url="";
                titan_debug=false;
                if(null!=ConfigurationManager.AppSettings["titan_debug"] && ConfigurationManager.AppSettings["titan_debug"]=="true") {
                    titan_debug =true;
                }
                if(null!=ConfigurationManager.AppSettings["titan_url"])    titan_url=ConfigurationManager.AppSettings["titan_url"];
            }

            public void run_crons() {
                string query="SELECT * FROM titanDWS WITH (NOLOCK) WHERE cron=cast(1 as bit)"; //get all methods that are cron's
                data_set rows=db.fetch_all("titan",query);

                foreach(row row in rows.rows) {
                    lambda i=new lambda();
                    i.group =(string)row["group"];
                    i.method=(string)row["method"];
                    string cron=(string)row["schedule"];
                    string[] tokens=cron.Split(' ');
                    if(tokens.Length!=5) continue;          //we need 5 pieces.
                    string minute       =tokens[0];
                    string hour         =tokens[1];
                    string day          =tokens[2];
                    string month        =tokens[3];
                    string day_of_week  =tokens[4];
                    DateTime time=DateTime.Now;
                    if(minute       !="*" && time.Minute.ToString()     !=minute) continue;
                    if(hour         !="*" && time.Hour.ToString()       !=hour) continue;
                    if(day          !="*" && time.Day.ToString()        !=day) continue;
                    if(month        !="*" && time.Month.ToString()      !=month) continue;
                    if(day_of_week  !="*" && time.DayOfWeek.ToString()  !=day_of_week) continue;
                    //we only get here if we have passed all the checks.
                    //web_service w=new web_service();
                    method m=new method();
                    m.execute(i,true,null,true);
                }
            }
           
            public  method fetch_method(lambda i,security.titan_token token) {      //for building the method in the report page/editing
                method m=new method();
                m.titan_debug=titan_debug;
                if(string.IsNullOrWhiteSpace(i.group) || string.IsNullOrWhiteSpace(i.method)) return new method();
                bool found=m.load(i.group,i.method,i.owner,i.configure,token);
                m.base_init();
                if(ConfigurationManager.AppSettings["titan_debug"]=="true") {
                    m.titan_debug=true;
                    m.generate_queries();
                }
                
                return m;
            }
                
            public string export(lambda i,string web_token) {
                string query="SELECT TOP 1 json,token FROM titanDWS_exports WITH (NOLOCK) WHERE [id]=@export_id AND [web_token]=@token";
                parameters param=new parameters();
                param.add("@token"     ,web_token);
                param.add("@export_id" ,i.export_id);

                System.Web.Script.Serialization.JavaScriptSerializer jss=new System.Web.Script.Serialization.JavaScriptSerializer();
                lambda export_input=null;
                security.titan_token s_token=null;
                data_set result=db.fetch("titan",query,param);

                if(null!=results) {
                    string json= (string)result["json"];
                    string token=(string)result["token"];
                    export_input=jss.Deserialize<lambda>(json);
                    s_token=jss.Deserialize<security.titan_token>(token);
                } else {
                    return null;                                                 //nothing... lets exit
                }
                                       
            if(null==export_input) return null;
            string name=String.Format("{0}-{1}-{2}.csv",export_input.group,export_input.method,DateTime.Now.ToLongTimeString());
            /*HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ClearHeaders();
            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment; filename=" + name);
            HttpContext.Current.Response.ContentType = "text/csv";
            HttpContext.Current.Response.AddHeader("Pragma", "public");
            */
            

            export_input.pageLength=0;
            export_input.page=0;
            this.input=export_input;

            StringBuilder sb=new StringBuilder();
            
            method m=new method();
            m.execute(input,true,s_token,false);
     
                int index=0;
                foreach (query.column c in results.columns) {
                    core.column mc=m.data_schema.Find(x=>x.name==c.name);
                    if(null!=mc && mc.export) {
                        if (index!=0) {
                            sb.Append(",");
                        }
                        index++;
                       sb.Append(String.Format("\"{0}\"",c.name));
                    }
                }        
                sb.Append("\r\n");
                   
                 
                foreach (string[] row in results.rows) {        //row
                    index=0;
                    foreach (string column in row) {        //column
                        if (index!=0) {
                            sb.Append(",");
                        }
                        index++;
                        sb.Append(String.Format("\"{0}\"",column));
                    }
                    sb.Append("\r\n");
                }

                return sb.ToString();
                //HttpContext.Current.Response.Flush();
                //HttpContext.Current.Response.End();
            }//end export
            /*
            public bool can_export_column(query.column c) {
                if(c.type=="link"           ) return false;
                if(c.type=="mlink"          ) return false;
                if(c.type=="plink"          ) return false;
                if(c.type=="imagelink"      ) return false;
                if(c.type=="js"             ) return false;
                if(c.type=="report"         ) return false;
                if(c.type=="shipdetails"    ) return false;
                if(c.type=="shipreturnlabel") return false;
                if(c.type=="shipcancel"     ) return false;
                return true;
            }
            public int create_export_link_id(string web_token,security.titan_token s_token) {
                string query=@"INSERT INTO titanDWS_exports ([token],[json],[web_token]) 
                            VALUES (@token,@json,@web_token);
                            SELECT CAST(SCOPE_IDENTITY() AS int) AS ProfileKey;";

                System.Web.Script.Serialization.JavaScriptSerializer jss=new System.Web.Script.Serialization.JavaScriptSerializer();
                string json=jss.Serialize(input);
                string token=jss.Serialize(s_token);
                if(string.IsNullOrWhiteSpace(web_token)) web_token="error";
                SqlParameter[] param={
                    new SqlParameter("@web_token"   ,web_token),
                    new SqlParameter("@token"       ,token),
                    new SqlParameter("@json"        ,json),
                    };
                string res=db.execute_scalar("titan",query,param);
                int res_id=0;
                Int32.TryParse(res,out res_id);
                return res_id;
                //return 0;
            }
            public string execute_query(query q,bool noLinks) {
                string extracted_query="";
                try{
                    if(m.count) {
                        Hashtable res=null;
                        if(m.query_is_sp) {
                            res=db.fetchSP(m.connection_string,q.getTotalQuery,q.totalParameters);
                        } else {
                            res=db.fetch(m.connection_string,q.getTotalQuery,q.totalParameters);
                        }
                        if(null!=res) {
                             results.total_rows=(int)res["count"];
                        }
                    }
                    List<Hashtable> rows=null;
                    if(m.query_is_sp) {
                        rows=db.fetchAllSP(m.connection_string,q.getResultsQuery,q.resultsParameters);
                        extracted_query=db.extract_query(m.connection_string,q.getResultsQuery,q.resultsParameters);
                    } else {
                        if(m.query_is_single) {
                            rows=new List<Hashtable>();
                            rows.Add(db.fetch(m.connection_string,q.getResultsQuery,q.resultsParameters));
                        } else {
                            rows=db.fetchAll(m.connection_string,q.getResultsQuery,q.resultsParameters);
                        }
                        extracted_query=db.extract_query(m.connection_string,q.getResultsQuery,q.resultsParameters);

                    }
                    if(null!=rows) {
                        results.rows.Clear();
            
                        foreach(Hashtable row in rows) {
                            process_data_row(q,row,noLinks);
                        }
                    }//while reader has rows
                } //try
                catch (Exception e){
                    //Log.Write(LogSeverity.Error, "mango WS: execute err: "+ e.Message);
                }
                return extracted_query;
            }

            public void process_data_row(query q,Hashtable row,bool noLinks) {
                int len=0;
                if(noLinks) {
                    foreach(query.column c in q.columns) {
                    core.column mc=m.columns.Find(x=>x.name==c.name);
                    if(null!=mc && mc.export) len++;
                    }
                } else len=q.columns.Count;
                string[] columns=new string[len];
                int index=0;
                foreach(query.column c in q.columns) {
                    column p;
                    string data="";
                    int cI=m.columns.FindIndex(x=>x.name==c.name);
                    if(cI>=0) p =m.columns[cI];
                    else p=new column(c.name,"number",0,true);
                    if(p.visible|| p.export) {
                    if(!row.ContainsKey(p.name)) { columns[index]="*"; index++; continue; }   //skip bad structure?
                        if(null==row[p.name]) data="";
                        else                  data=row[p.name].ToString();
                        if(noLinks && !p.export) continue;
                        columns[index]=data_types.transform_data(p.type,data,c);                  //does the conversion of types String to whatever is in the data types table for that type.
                        index++;
                    }//end p visible
                }//end for each columns
                results.rows.Add(columns);

            }
       
            public void create_export_link(security.titan_token s_token) {
                if(!m.export) {
                    results.export_id=0;
                    results.export_link="";
                    return;
                }
                Guid web_token=Guid.NewGuid();
                results.export_id=create_export_link_id(web_token.ToString(),s_token);
                
                string filename=String.Format("{0}-{1}-{2}.csv",
                                    input.group,input.method,DateTime.Now.ToString("yyyy-MM-dd-HH-mm") );
                string href=String.Format("{0}/{1}/{2}",
                    web_token.ToString(),
                    results.export_id,
                    HttpContext.Current.Server.UrlEncode(filename) );

                if(m.export) {
                    results.export_link=String.Format("<a class='btn btn-primary' target='_blank' href='{0}/titanDWS/export/{1}'><i class='nc-icon-mini arrows-1_share-66'></i>&nbsp;Export</a>",            
                                                    ConfigurationManager.AppSettings["MangoWebAPI_URL"],href);
                } 
            }           
  
            public List<query.column> generate_visible_column_list(bool noLinks) {
                List<query.column> columns=new List<query.column>();
                query.column defaultFunc=new query.column("count","count","function","count(*)");
                if (input.aggregates !=null && input.aggregates.Count>0) {                 //ok if we have a group by then this blows out normal columns
                    foreach(KeyValuePair<string,bool> d in input.aggregates) {
                        if(d.Value) {
                            string aggName=d.Key;
                            columns.Add(new query.column(aggName.Substring(("Titan_Aggregate_").Length),aggName.Substring(("Titan_Aggregate_").Length),"aggregate",""));
                        }
                    }
                    columns.Add(defaultFunc);
                } else {
                    if(null!=m.columns) {
                        foreach(column c in m.columns) {     //Normal columns... add by actual names, display is for javascript.
                            if(c.visible) {
                                if(noLinks) {
                                    if (c.type=="link"           ) continue;
                                    if(c.type=="mlink"          ) continue;
                                    if(c.type=="plink"          ) continue;
                                    if(c.type=="imagelink"      ) continue;
                                    if(c.type=="js"             ) continue;
                                    if(c.type=="report"         ) continue;
                                    if(c.type=="shipdetails"    ) continue;
                                    if(c.type=="shipreturnlabel") continue;
                                    if(c.type=="shipcancel"     ) continue;
                                }
                                columns.Add(new query.column(c.name,c.display,"column",""));
                            }
                        }
                    }//end null columns
                }
                return columns;
            }
        
            public void adjust_input_values() {
                results.page=input.page;
                if(input.pageLength==0) {
                    input.pageLength=100000;
                    input.page=0;
                }
                results.pageLength=input.pageLength;
                results.parameters=input.parameters;
            
            }

            public string generate_sort(query q) {
                List<string> orderBy=new List<string>();
                if(null!=input.sort && input.sort.Count>0) {                            //if i have sort parameters
                    foreach (string[] column in input.sort) {
                        orderBy.Add(string.Format("[{0}] {1}",column[0],column[1]));
                    }
                } else {                                                                //use default sort, if non use first column.
                    if(null!=m.columns) {
                        foreach (column c in m.columns) {
                            //int index=q.columns.FindIndex(x=>x.ToString()==c.display);
                            //if(index>-1) {
                            string dir="ASC";
                            if(!c.sort_default_order_asc) dir="DESC";
                                if (c.sortDefault) orderBy.Add(string.Format("[{0}] {1}", c.name, dir));
                           //}
                        }//end foreach
                    }

                    if(orderBy.Count==0) {                                             //no defaults.... Sort by something!
                        if(null!=input.aggregates && input.aggregates.Count>0) {       //if aggregated.. default on ag column 1.
                            foreach(KeyValuePair<string,bool> d in input.aggregates) {
                                orderBy.Add(string.Format("[{0}] {1}", d.Key.Substring(("Titan_Aggregate_").Length), "ASC"));
                                break;
                            }

                        } else {                                                       //else default on first visible column
                            if(null!=m.columns) {
                                foreach (column c in m.columns) {
                                    if(c.visible) {
                                        orderBy.Add(string.Format("[{0}] {1}", c.name, "ASC"));
                                        break;
                                    }
                                }
                            }
                        }

                    }
                }

                return "ORDER BY "+string.Join(",",orderBy);
            }
             
            public method_results execute(bool noLinks,security.titan_token token,bool init) {                  //actual method execution
                query q=new query();
                adjust_input_values();                                                                          //if a page is set to unlimited rows, adjust the page length
                q.page=input.page;
                q.pageLength=input.pageLength;
                m.load_execute(input.group,input.method,input.owner);                                           //load the method
                q.m=m;
                q.resultsQuery=m.strip_comments(m.query);                                                       //clean the query of comments. so that it can be simplified in the log. copy/pastable
                if(init) {
                    m.get_signature();
                    q.resultsParameters=m.execute_load_parameters(token,input.parameters,true);              //load the parameters
                    q.totalParameters  =m.execute_load_parameters(token,input.parameters,true);
                } else {
                    q.resultsParameters=m.execute_load_parameters(token,input.parameters,false);              //load the parameters
                    q.totalParameters  =m.execute_load_parameters(token,input.parameters,false);
                }
                q.columns=generate_visible_column_list(noLinks);            //create a list of available columns for the query.
                q.orderByString=generate_sort(q);                           //create the sort
                q.create_filters(input);
                create_export_link(token);                                  //Create the link for the expot Downoad.. if applicable
                results.executed_query=execute_query(q,noLinks);                                   //This is what does the magic.....
                results.success=true;
                results.columns=q.columns;

                int index=0;
                foreach (query.column c in q.columns) { results.keys[c.name]=index; index++; }
                return results;
            }//end execute
*/
        
    }//end class
}//end namespace