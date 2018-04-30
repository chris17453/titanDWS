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
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Collections;
//using System.Web.Script.Serialization;
using titan.core;
using System.Text.RegularExpressions;
using nocodedb.data;
using nocodedb.data.models;

namespace titan {
    public partial class method:data  {
             
            public method() {
            }

            public method(models.lambda i,string _group,string _method,string _owner,string _localization,titan.security.titan_token token) {
                input     =i;
                this.token    =token;
                this.group     =_group;
                this.method    =_method;
                this.owner     =_owner;
                this.configure=_localization;
                base_init();
            }


            public bool load(string group,string method,string owner,string configure,titan.security.titan_token token) {//titan fetch method
                this.token    =token;
                this.group     =group;
                this.method    =method;
                this.owner     =owner;
                this.configure=configure;
                bool found=this.load();
                if(found) {
                    base_init();
                }
                return found;
            }


            public bool load_execute(string group,string method,string owner) {
                this.execute_init=true;
                this.group     =group;
                this.method    =method;
                this.owner     =owner;
                this.configure="none";
                bool found=this.load();
                this.base_init(true);
                return found;
            }


            public void get_signature() {
                external_data_defaults ed=new external_data_defaults();
                ed.group  =group;
                ed.method =method;
                ed.owner  =owner;
                ed.load();
                this.sig_init        =ed;
            }
            public void get_signature_defaults() {
                external_data_mapping ed=new external_data_mapping();
                ed.name =this.data_mapping_name;
                if(!String.IsNullOrWhiteSpace(this.data_mapping_name)) {
                    ed.load();
                    this.external_map=ed;
                } else {
                    Console.WriteLine("Cant get Signature for {0}",ed._table);
                }
            }

            public void base_init(bool execute=false) {
                if(!execute) {
                    this.external_mapping=db.fetch_all("titan","SELECT [name] from titanDWS_external_mapping where active=1");
                    get_signature();
                    get_signature_defaults();
                    if(this.configure=="")          {
                        if (this.data_template) {
                            this.localization    = new localization().get(this.group,this.method);
                        }
                    }      //if set to none its not needed
                    if(this.configure=="configure") { this.localization    = new localization().get(); }                //config ui is not running ui

                    this.output_types        =core.output_types.get();
                    this.templates           =core.template.get();
                    this.result_types        =core.column_types.get();
                    this.parameter_types     =core.parameter_types.get();
                    this.methods             =linked_method.get();
                    this.plugins             =db.fetch_all("titan","SELECT [name],[display] FROM titanDWS_plugins where active=1 ORDER BY display");

                    if(null==this.parameters) this.parameters=new List<titan.core.parameter>();
                    if(null==this.data_schema) {
                        this.data_schema=new List<titan.core.column>();
                    } else {
                        parameters sub_parameters=execute_load_parameters(this.token,null,false);                                  //add param's for sub queries.
                        foreach(titan.core.column c in this.data_schema) {
                            //if(c.custom_aggregate) this.aggregates.Add(new aggregate(c.name,c.alias,c.groupOrder));
                            if(String.IsNullOrWhiteSpace(c.width)) c.width=((int)(1200/this.data_schema.Count)).ToString();
                            c.buildOptions(sub_parameters);
                        }
                    }

                }
                

              /* this.aggregates.Sort(delegate (aggregate x, aggregate y) {
                    if(null==y) return -1;
                    if(null==x) return 1;
                    if (x.order == y.order) {
                        return x.name.CompareTo(y.name);
                    }
                    if (x.order < y.order) return -1;
                    if (x.order > y.order) return 1;
                    return 0;
                });*/
                                    
                this.data_schema.Sort(delegate (titan.core.column x, titan.core.column y) {
                    if(null==y) return -1;
                    if(null==x) return 1;
                    if (x.order == y.order) {
                        return x.name.CompareTo(y.name);
                    }
                    if (x.order < y.order) return -1;
                    if (x.order > y.order) return 1;
                    return 0;
                });

                this.parameters.Sort(delegate (titan.core.parameter x, titan.core.parameter y) {
                    if(null==y) return -1;
                    if(null==x) return 1;
                    if (x.order == y.order) {
                        return x.name.CompareTo(y.name);
                    }
                    if (x.order < y.order) return -1;
                    if (x.order > y.order) return 1;
                    return 0;
                });
                if(String.IsNullOrWhiteSpace(this.template_name)) {
                    this.template=core.template.get_default();
                    if(String.IsNullOrWhiteSpace(this.template_name)) {                     //try to set the template name.
                        if(null==this.template || !template[0].ContainsKey("name")) {
                            template_name="Default";
                        } else {
                            template_name=(string)this.template[0]["name"];
                        }
                    }
                } else {
                    this.template=core.template.get(this.template_name);
                }
                
            }
        /*
            public void autoMethod(string group,string method) {
                method=method.Replace(' ','_');
                method=method.Replace('\'','_');
                method=method.Replace('\"','_');
                method=method.Replace(';','_');

                this.display=method;
                this.group=group;
                this.query=String.Format("SELECT * from [dbo].[{0}]",method);
                this.export=true;
                this.parameters=new List<parameter>();
                this.aggregates=new List<aggregate>();

    
                this.name = "System Table -> "+method;
                this.desc = "This is an automatic live system method. It will change anytime a table changes.";
                this.type = "JSON";
                this.visible = true;
                this._internal =true;
                this.external =false;
                this.sort =0;
                this.menuID = 0;
                this.created = DateTime.Now;
                this.modified = DateTime.Now;
                this.active = true;
                //this.template=getDefaultTemplate();
                //this.template_id=this.template.id;
                string query=@"SELECT c.[name],t.[name] as 'type',cast(case when coalesce(t.collation_name,'NO')='NO' then 0 else 1 END as bit)  as searchable
                                FROM syscolumns c
                                LEFT JOIN sys.types t ON c.xusertype = t.user_type_id 
                                WHERE id = (SELECT id FROM sysobjects WHERE type = 'U' AND [Name] =  @table)";
                    using (DatabaseAdapter da = new DatabaseAdapter(AdapterType.DEFAULT)) {

                        SqlParameter[] param = { new SqlParameter("@table",method) };
                        using (SqlDataReader reader = da.ExecuteReader(query, param)) {
                        int index=0;
                            if(reader.HasRows) {
                                while(  reader.Read()) {
                                string name= reader.GetString("name");
                                string underlyingType= reader.GetString("type");
                                bool   searchable= reader.GetBoolean("searchable");
                                column c=new column(name,searchable,underlyingType,type,index.ToString(),true,String.Format("Column {0}",index),"",true,true,true,true,index,"100px","",false);
                                this.columns.Add(c);
                                if (c.custom_aggregate) aggregates.Add(new aggregate(c.name,c.alias,c.groupOrder));                                

                            index++;
                            }//end while read
                        }//end hasrows
                    }//end reader
                  
                }//end using da
            }//end func
            */

            public parameters get_parameter_from_html_name(string name,string value) {
            parameters param=new parameters();
                core.parameter matchedP=this.parameters.Find(x=>x.htmlName==name);                //find the parameter in the list with lamba find
                if(null!=matchedP && null!=matchedP.name && value!=null) {              //ok we got a match...
                    if(String.IsNullOrWhiteSpace(matchedP.name)) return null;
                    if(matchedP.type=="daterange") {
                        string to="";
                        string from="";
                        if (!String.IsNullOrWhiteSpace(value)) {
                            string[] tokens= Regex.Split(value, " - ");
                            if(tokens.Length>0) from=tokens[0];
                            if(tokens.Length>1) to  =tokens[1];
                        }
                        
                        if(!string.IsNullOrWhiteSpace(from)) param.add("@" +matchedP.name+"_from",from);
                        if(!string.IsNullOrWhiteSpace(to  )) param.add("@" +matchedP.name+"_to"  ,to);
                    } else {//end if daterange
                        if(null!=value) {
                            param.add("@" +matchedP.name+"_from",value);
                        }
                    }
                }
                return param;
            }
          //borrowed from the internet.... stackoverflow  
            /*public void backup_method() {
                JavaScriptSerializer jss=new JavaScriptSerializer();
                string method_json=jss.Serialize(this);
                db.execute_non_query("titan","INSERT INTO titanDWS_history ([titanDWS_id],[json]) VALUES (@id,@json)",new string[,]{ { "id",this.id.ToString()},{ "json",method_json} } );
            }*/
            public parameters execute_load_parameters(security.titan_token token,Dictionary<string,string> form_parameters,bool init) {
        
                parameters paramList=new parameters();
                if(init) {                               //caching...
                    get_signature();
                    get_signature_defaults();
                }
                models.lambda input=new models.lambda();
                if(null!=token) {
                    input.reference1=token.r1;        //good place for the defaults
                    input.reference2=token.r2;
                    input.reference3=token.r3;
                    input.reference4=token.r4;
                    input.reference5=token.r5;
                    input.reference6=token.r6;
                    input.reference7=token.r7;
                    input.reference8=token.r8;
                    input.reference9=token.r9;
                    input.reference10=token.r10;
                    input.reference11=token.r11;
                    input.reference12=token.r12;
                    input.reference13=token.r13;
                    input.reference14=token.r14;
                    input.reference15=token.r15;
                }
                input.parameters=new Dictionary<string, string>();
                if(init) {                                                              //testing data? i remember this then i did something 
                    foreach (titan.core.parameter p in this.parameters) {                               //and then im like... important but move? <-Im not sure? 
                        string value=p.test;                                            //Rewrite test data injection (pretty sure I cleaned this up)
                        if(String.IsNullOrWhiteSpace(p.test)) value="";
                        parameters range=get_parameter_from_html_name(p.htmlName ,p.test);
                        if(null!=range) paramList.AddRange(range);
                    }

                    if( null!=this.external_map &&
                        null!=this.sig_init       ) {
                            if(!string.IsNullOrWhiteSpace(external_map.reference1 )) { paramList.add("@"+external_map.reference1 ,sig_init.r1  ); }
                            if(!string.IsNullOrWhiteSpace(external_map.reference2 )) { paramList.add("@"+external_map.reference2 ,sig_init.r2  ); }
                            if(!string.IsNullOrWhiteSpace(external_map.reference3 )) { paramList.add("@"+external_map.reference3 ,sig_init.r3  ); }
                            if(!string.IsNullOrWhiteSpace(external_map.reference4 )) { paramList.add("@"+external_map.reference4 ,sig_init.r4  ); }
                            if(!string.IsNullOrWhiteSpace(external_map.reference5 )) { paramList.add("@"+external_map.reference5 ,sig_init.r5  ); }
                            if(!string.IsNullOrWhiteSpace(external_map.reference6 )) { paramList.add("@"+external_map.reference6 ,sig_init.r6  ); }
                            if(!string.IsNullOrWhiteSpace(external_map.reference7 )) { paramList.add("@"+external_map.reference7 ,sig_init.r7  ); }
                            if(!string.IsNullOrWhiteSpace(external_map.reference8 )) { paramList.add("@"+external_map.reference8 ,sig_init.r8  ); }
                            if(!string.IsNullOrWhiteSpace(external_map.reference9 )) { paramList.add("@"+external_map.reference9 ,sig_init.r9  ); }
                            if(!string.IsNullOrWhiteSpace(external_map.reference10)) { paramList.add("@"+external_map.reference10,sig_init.r10 ); }
                            if(!string.IsNullOrWhiteSpace(external_map.reference11)) { paramList.add("@"+external_map.reference11,sig_init.r11 ); }
                            if(!string.IsNullOrWhiteSpace(external_map.reference12)) { paramList.add("@"+external_map.reference12,sig_init.r12 ); }
                            if(!string.IsNullOrWhiteSpace(external_map.reference13)) { paramList.add("@"+external_map.reference13,sig_init.r13 ); }
                            if(!string.IsNullOrWhiteSpace(external_map.reference14)) { paramList.add("@"+external_map.reference14,sig_init.r14 ); }
                            if(!string.IsNullOrWhiteSpace(external_map.reference15)) { paramList.add("@"+external_map.reference15,sig_init.r15 ); }
                            
                    }//end if external_data_maping exists

                } else { //end p1 init
                    input.parameters=form_parameters;
                
                    //System variables
                    
                    if(null!=this.external_map) {
                        if(!string.IsNullOrWhiteSpace(external_map.reference1 )) { paramList.add("@"+external_map.reference1 ,sig_init.r1  ); }
                        if(!string.IsNullOrWhiteSpace(external_map.reference2 )) { paramList.add("@"+external_map.reference2 ,sig_init.r2  ); }
                        if(!string.IsNullOrWhiteSpace(external_map.reference3 )) { paramList.add("@"+external_map.reference3 ,sig_init.r3  ); }
                        if(!string.IsNullOrWhiteSpace(external_map.reference4 )) { paramList.add("@"+external_map.reference4 ,sig_init.r4  ); }
                        if(!string.IsNullOrWhiteSpace(external_map.reference5 )) { paramList.add("@"+external_map.reference5 ,sig_init.r5  ); }
                        if(!string.IsNullOrWhiteSpace(external_map.reference6 )) { paramList.add("@"+external_map.reference6 ,sig_init.r6  ); }
                        if(!string.IsNullOrWhiteSpace(external_map.reference7 )) { paramList.add("@"+external_map.reference7 ,sig_init.r7  ); }
                        if(!string.IsNullOrWhiteSpace(external_map.reference8 )) { paramList.add("@"+external_map.reference8 ,sig_init.r8  ); }
                        if(!string.IsNullOrWhiteSpace(external_map.reference9 )) { paramList.add("@"+external_map.reference9 ,sig_init.r9  ); }
                        if(!string.IsNullOrWhiteSpace(external_map.reference10)) { paramList.add("@"+external_map.reference10,sig_init.r10 ); }
                        if(!string.IsNullOrWhiteSpace(external_map.reference11)) { paramList.add("@"+external_map.reference11,sig_init.r11 ); }
                        if(!string.IsNullOrWhiteSpace(external_map.reference12)) { paramList.add("@"+external_map.reference12,sig_init.r12 ); }
                        if(!string.IsNullOrWhiteSpace(external_map.reference13)) { paramList.add("@"+external_map.reference13,sig_init.r13 ); }
                        if(!string.IsNullOrWhiteSpace(external_map.reference14)) { paramList.add("@"+external_map.reference14,sig_init.r14 ); }
                        if(!string.IsNullOrWhiteSpace(external_map.reference15)) { paramList.add("@"+external_map.reference15,sig_init.r15 ); }

                    }//end if
                    if (null!=input.parameters && 0<input.parameters.Count) {
                        foreach (KeyValuePair<string,string> p in  input.parameters) {
                            core.parameter matchedP=this.parameters.Find(x=>x.htmlName==p.Key);                //find the parameter in the list with lamba find
                            if(null!=matchedP && null!=p.Value && null!=matchedP.name) {                                                                //ok we got a match...
                                if(String.IsNullOrWhiteSpace(matchedP.name)) continue;
                                if(matchedP.type=="daterange") {
                                    string to=null;
                                    string from=null;
                                    if (!String.IsNullOrWhiteSpace(p.Value)) {
                                        string[] tokens= Regex.Split(p.Value, " - ");
                                        if(tokens.Length>0) from=tokens[0];
                                        if(tokens.Length>1) to  =tokens[1];
                                    }
                                    
                                    if(string.IsNullOrWhiteSpace(to))   paramList.add("@" +matchedP.name+"_from",from);
                                    if(string.IsNullOrWhiteSpace(from)) paramList.add("@" +matchedP.name+"_to"  ,to);
                            
                                } else {
                                    if(null!=p.Value) {
                                        paramList.add("@" +matchedP.name,p.Value);
                                    }
                                }
                            }//end if matched
                        }//end parameter Loop
                    }//end if params

            }//end else
                return paramList;
            }//end create parameters
            public void generate_queries() {
                this.insert_query=generate_insert_query();
                this.update_query=generate_update_query();
            }
        }//end class
}