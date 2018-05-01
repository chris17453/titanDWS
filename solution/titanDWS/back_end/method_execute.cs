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
using System.Configuration;
using titan.core;
using titan;
using nocodedb.data;
using nocodedb;
using nocodedb.data.models;

namespace titan {
    public partial class method  {
               
            public int create_export_link_id(string web_token,security.titan_token s_token) {
                string query=@"INSERT INTO titanDWS_exports ([token],[json],[web_token]) 
                            VALUES (@token,@json,@web_token);
                            SELECT CAST(SCOPE_IDENTITY() AS int) AS ProfileKey;";

                System.Web.Script.Serialization.JavaScriptSerializer jss=new System.Web.Script.Serialization.JavaScriptSerializer();
                string json=jss.Serialize(this.input);
                string token=jss.Serialize(s_token);
                if(string.IsNullOrWhiteSpace(web_token)) web_token="error";
                    parameters param=new parameters();
                    param.add("@web_token"   ,web_token);
                    param.add("@token"       ,token);
                    param.add("@json"        ,json);
                    
                column_data res=db.execute_scalar("titan",query,param);
                int res_id=0;
                Int32.TryParse((string)res.ToString(),out res_id);
                return res_id;
                //return 0;
            }
            public string execute_query(query q,bool noLinks) {
                string extracted_query="";
                try{
                    if(this.count) {
                        data_set res=null;
                        /*if(this.query_is_sp) {
                            res=db.sp_fetch(this.connection_string,q.getTotalQuery,q.totalParameters);
                        } else {*/
                        column_data res_count=db.execute_scalar(this.connection_string,q.getTotalQuery,q.totalParameters);
                        //}
                        if(null!=res_count) {
                            this.results.total_rows=res_count;
                        }
                    }
                    data_set rows=null;
                    if(this.query_is_sp) {
                        rows=db.sp_fetch_all(this.connection_string,q.getResultsQuery,q.resultsParameters);
                        extracted_query=db.extract_query(new query_params(connection_string, 
                                                                          q.getResultsQuery,
                                                                          q.resultsParameters,
                                                                          true,
                                                                          query_types.sp_multiple));
                    } else {
                        if(this.query_is_single) {
                            rows=db.fetch(this.connection_string,q.getResultsQuery,q.resultsParameters);
                            extracted_query=db.extract_query(new query_params(connection_string, 
                                                                              q.getResultsQuery,
                                                                              q.resultsParameters,
                                                                              true,
                                                                              query_types.single));
                        } else {
                            rows=db.fetch_all(this.connection_string,q.getResultsQuery,q.resultsParameters);
                            extracted_query=db.extract_query(new query_params(connection_string, 
                                                                              q.getResultsQuery,
                                                                              q.resultsParameters,
                                                                              true,
                                                                              query_types.multiple));

                        }

                    }
                    if(null!=rows) {
                        this.results.rows.Clear();
            
                        foreach(row row in rows) {
                            process_data_row(q,row,noLinks);
                        }
                    }//while reader has rows
                } //try
                catch (Exception e){
                    //Log.Write(LogSeverity.Error, "mango WS: execute err: "+ e.Message);
                }
                return extracted_query;
            }

            public void process_data_row(query q,row row,bool noLinks) {
                int len=0;
                if(noLinks) {
                    foreach(query.column c in q.columns) {
                    core.column mc=this.data_schema.Find(x=>x.name==c.name);
                    if(null!=mc && mc.export) len++;
                    }
                } else len=q.columns.Count;
                string[] data_columns=new string[len];
                int index=0;
                foreach(query.column c in q.columns) {
                    titan.core.column p;
                    string data="";
                    int cI=this.data_schema.FindIndex(x=>x.name==c.name);
                    if(cI>=0) p =this.data_schema[cI];
                else p=new titan.core.column(c.name,"number",0,true);
                    if(p.visible|| p.export) {
                    if(!row.ContainsKey(p.name)) { data_columns[index]="*"; index++; continue; }   //skip bad structure?
                        if(null==row[p.name]) data="";
                        else                  data=row[p.name].ToString();
                        if(noLinks && !p.export) continue;
                        data_columns[index]=this.data_types.transform_data(p.type,data,c);                  //does the conversion of types String to whatever is in the data types table for that type.
                        index++;
                    }//end p visible
                }//end for each columns
                this.results.rows.Add(data_columns);

            }
       
            public void create_export_link(security.titan_token s_token) {
                if(!this.export) {
                    this.results.export_id=0;
                    this.results.export_link="";
                    return;
                }
                Guid web_token=Guid.NewGuid();
                this.results.export_id=create_export_link_id(web_token.ToString(),s_token);
                
                string filename=String.Format("{0}-{1}-{2}.csv",
                                    this.input.group,this.input.method,DateTime.Now.ToString("yyyy-MM-dd-HH-mm") );
                string href=String.Format("{0}/{1}/{2}",
                    web_token.ToString(),
                    this.results.export_id,
                    System.Net.WebUtility.UrlEncode(filename) );

                if(this.export) {
                    this.results.export_link=String.Format("<a class='btn btn-primary' target='_blank' href='{0}/titanDWS/export/{1}'><i class='nc-icon-mini arrows-1_share-66'></i>&nbsp;Export</a>",            
                                                    ConfigurationManager.AppSettings["MangoWebAPI_URL"],href);
                } 
            }           
  
            public List<query.column> generate_visible_column_list(bool noLinks) {
                List<query.column> column_list=new List<query.column>();
                query.column defaultFunc=new query.column("count","count","function","count(*)");
                if (this.input.aggregates !=null && this.input.aggregates.Count>0) {                 //ok if we have a group by then this blows out normal columns
                    foreach(KeyValuePair<string,bool> d in this.input.aggregates) {
                        if(d.Value) {
                            string aggName=d.Key;
                            column_list.Add(new query.column(aggName.Substring(("Titan_Aggregate_").Length),aggName.Substring(("Titan_Aggregate_").Length),"aggregate",""));
                        }
                    }
                    column_list.Add(defaultFunc);
                } else {
                    if(null!=this.data_schema) {
                        foreach(core.column c in this.data_schema) {     //Normal columns... add by actual names, display is for javascript.
                            if(c.visible) {
                                column_list.Add(new query.column(c.name,c.display,"column",""));
                            }
                        }
                    }//end null columns
                }
                return column_list;
            }
        
            public void adjust_input_values() {
                try {
                    this.results.page=this.input.page;
                    if(this.input.pageLength==0) {
                        this.input.pageLength=100000;
                        this.input.page=0;
                    }
                    this.results.pageLength=this.input.pageLength;
                    this.results.parameters=this.input.parameters;
                } catch(Exception ex) {
                    log.write("method",log_type.Error,ex.Message);
                }
            
            }

            public string generate_sort(query q) {
                List<string> orderBy=new List<string>();
                if(null!=this.input.sort && this.input.sort.Count>0) {                            //if i have sort parameters
                    foreach (string[] column in this.input.sort) {
                        orderBy.Add(string.Format("[{0}] {1}",column[0],column[1]));
                    }
                } else {                                                                //use default sort, if non use first column.
                    if(null!=this.data_schema) {
                    foreach (titan.core.column c in this.data_schema) {
                            //int index=q.columns.FindIndex(x=>x.ToString()==c.display);
                            //if(index>-1) {
                            string dir="ASC";
                            if(!c.sort_default_order_asc) dir="DESC";
                                if (c.sortDefault) orderBy.Add(string.Format("[{0}] {1}", c.name, dir));
                           //}
                        }//end foreach
                    }

                    if(orderBy.Count==0) {                                             //no defaults.... Sort by something!
                        if(null!=this.input.aggregates && this.input.aggregates.Count>0) {       //if aggregated.. default on ag column 1.
                            foreach(KeyValuePair<string,bool> d in this.input.aggregates) {
                                orderBy.Add(string.Format("[{0}] {1}", d.Key.Substring(("Titan_Aggregate_").Length), "ASC"));
                                break;
                            }

                        } else {                                                       //else default on first visible column
                            if(null!=this.data_schema) {
                            foreach (titan.core.column c in this.data_schema) {
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
       
            public titan.models.method_results execute(models.lambda _input,bool noLinks,security.titan_token token,bool init) {                  //actual method execution
                this.input=_input;
                query q=new query();
                adjust_input_values();                                                                          //if a page is set to unlimited rows, adjust the page length
                q.page=this.input.page;
                q.pageLength=this.input.pageLength;
                load_execute(this.input.group,this.input.method,this.input.owner);                                           //load the method

                q.m=this;
                q.resultsQuery=strip_comments(this.query);                                                       //clean the query of comments. so that it can be simplified in the log. copy/pastable
                if(init) {
                    get_signature();
                    q.resultsParameters=execute_load_parameters(token,this.input.parameters,true);              //load the parameters
                    q.totalParameters  =execute_load_parameters(token,this.input.parameters,true);
                } else {
                    q.resultsParameters=execute_load_parameters(token,this.input.parameters,false);              //load the parameters
                    q.totalParameters  =execute_load_parameters(token,this.input.parameters,false);
                }
                q.columns=generate_visible_column_list(noLinks);            //create a list of available columns for the query.
                q.orderByString=generate_sort(q);                           //create the sort
                q.create_filters(this.input);
                create_export_link(token);                                  //Create the link for the expot Downoad.. if applicable
                string temp_q=execute_query(q,noLinks);                                   //This is what does the magic.....
                if(this.titan_debug) {
                    this.results.executed_query=temp_q;
                    
                }
                this.results.success=true;
                this.results.columns=q.columns;

                int index=0;
                foreach (query.column c in q.columns) { this.results.keys[c.name]=index; index++; }
                return this.results;
            }//end execute
    }//end partial class
}//end namespace