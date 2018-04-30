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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;
using nocodedb.data;
using nocodedb.data.models;

namespace titan {
        public class crud<T> {
            internal string _table      =null;
            internal string [] _pk      =null;
            internal string [] _json    =null;

            private string get_pk() {
                List<string> o=new List<string>();
                foreach(string _k in _pk) {
                    o.Add("["+_k+"]=@"+_k);
                }
                return String.Join(" AND ",o);
            }
            private string get_columns() {
                List<string> o=new List<string>();
                Type t=this.GetType();
                
                foreach (PropertyInfo pi in t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) { 
                    string field_name     = pi.Name;
                    object property_value = pi.GetValue(this, null);
                    if(null==property_value ) continue;
                    else o.Add("["+field_name+"]");
                    
                }
                return String.Join(",",o);
            }
            private string get_params() {
                List<string> o=new List<string>();
                Type t=this.GetType();
                
                foreach (PropertyInfo pi in t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) { 
                    string field_name     = pi.Name;
                    object property_value = pi.GetValue(this, null);

                    if(null==property_value ) continue;
                    else o.Add("@"+field_name);
                }
                return String.Join(",",o);
            }

            private string get_col_param() {
                List<string> o=new List<string>();
                Type t=this.GetType();
                
                foreach (PropertyInfo pi in t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) { 
                    string field_name     = pi.Name;
                    object property_value = pi.GetValue(this, null);
                    if(null==property_value ) continue;
                    else o.Add("["+field_name+"]="+"@"+field_name);
                }
                return String.Join(",",o);
            }

            public bool set_property(string field_name,object value) {
                PropertyInfo pi=this.GetType().GetProperty(field_name);
                if (null!=pi) {
                    if(null!=_json) {
                        foreach(string s in _json) {
                            if (s==field_name) {
                                Type type = pi.PropertyType;
                                if(type.IsGenericType && type.GetGenericTypeDefinition()== typeof(List<>)) {
                                    try {
                                        object o2;
                                
                                        object instance=null;
                                        Type itemType = type.GetGenericArguments()[0]; // use this...
                                        Type constructed = typeof(List<>).MakeGenericType(itemType);
                                        instance = Activator.CreateInstance(constructed);

                                        object s2=itemType.MakeByRefType();
                                        o2=JsonConvert.DeserializeAnonymousType((string)value,instance,new JsonSerializerSettings{
                                                Error = delegate(object sender, ErrorEventArgs args){
                                                    Console.WriteLine(args.ErrorContext.Error.Message);
                                                    args.ErrorContext.Handled = true;
                                                }
                                        });

                                       
                                        this.GetType().GetProperty(field_name).SetValue(this,((JArray)o2).ToObject(constructed), null);

                                    } catch (Exception ex) {
                                        Console.WriteLine(ex.Message);
                                        return false;
                                    }
                                }
                                return false;
                        
                            }//end if s==name
                        }//end for
                    }//end if in _json
                    
                    //normal assignement
                    this.GetType().GetProperty(field_name).SetValue(this, value, null);
                    return true;
                }
                return false;
            }
      

            public parameters generate_crud_parameters() {
               /*d.parameters.RemoveAll(item => item == null);
                int p;
                while ((p=parameters.FindIndex(x=>String.IsNullOrWhiteSpace(x.name) ))>0) {
                    parameters.RemoveAt(p);
                }*/
             
                /*string serial_linked_methods=jss.Serialize(d.linked_methods);
                string serial_columns       =jss.Serialize(d.);
                string serial_param         =jss.Serialize(d.parameters);
                */

                parameters param=new parameters();
                PropertyInfo[] properties = typeof(method).GetProperties();
                Type t=this.GetType();
                foreach (PropertyInfo pi in t.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) { 
                    string field_name     = pi.Name;
                    if(field_name=="_table" || field_name=="_pk" || field_name=="_json") continue;              //dont paramaterize the base properties
                    object property_value = pi.GetValue(this, null);
                    if(null!=_json) {
                        bool found=false;
                        foreach(string s in _json) { 
                            if (s==field_name) {

                                string serial=JsonConvert.SerializeObject(property_value);
                                if(pi.GetValue(this, null)!=null) {
                                    param.add("@"+field_name,serial);
                                    found=true;
                                    break;
                                }
                            }
                        }
                        if(found) continue;
                    }
                    if(property_value is Guid) {
                        if(((Guid)property_value) == Guid.Empty) continue;
                        param.add("@"+field_name,(Guid)property_value);
                        continue;
                    }
                    if (property_value is DateTime) {
                        if((DateTime)property_value==DateTime.MinValue) continue;
                        param.add("@"+field_name,(DateTime)property_value);
                        continue;
                    }
                    if(property_value is string) {
                        param.add("@"+field_name,(string)property_value);
                        continue;
                    }
                    if(property_value is int) {
                        param.add("@"+field_name,(int)property_value);
                        continue;
                    }
                    if(property_value is bool) {
                        param.add("@"+field_name,(bool)property_value);
                        continue;
                    }

                    if (pi.GetValue(this, null)!=null) {
                        param.add("@"+field_name,property_value);
                        continue;
                    }

                }
                return param;
            }
        
            public bool load() {
                parameters q_param=generate_crud_parameters();
                string query=String.Format("SELECT TOP 1 * FROM {0} WHERE {1} ",this._table,this.get_pk());
                data_set res=db.fetch("titan",query,q_param);
                
                
                if (null!=res) {
                    foreach (String field_name in res.Keys) {
                        this.set_property(field_name,res[field_name]);
                    }//loop through all keys
                    return true;
                }//if it exist
                return false;
            }//if it exist

        
        /*


                    JavaScriptSerializer jss= new JavaScriptSerializer();
                    this.columns            = jss.Deserialize<List<column>>(column_jsss);
                    this.parameters         = jss.Deserialize<List<parameter>>(param_jsss);
                    this.linked_methods     = jss.Deserialize<List<linked_method>>(linked_jsss);

                    if(null==columns)           columns         =new List<column>();
                    if(null==parameters)        parameters      =new List<parameter>();
                    if(null==linked_methods)    linked_methods  =new List<linked_method>();
                    if(columns.Count>0) {
                        foreach(column c in columns) {
                            //if(!c.is_identity) {  //not doing this because we can do it in the crud update/insert
                                results_template.Add(c.name);
                            //}
                        }
                    }
                    if(!execute_init)   base_init(false);
                    else                base_init(true);
                    foreach(linked_method lm in linked_methods ){
                        method m=new method();
                        m.token=token;
                        m.configure=configure;
                        m.execute_init=execute_init;
                        m.load(lm.id);
                        linked.Add(m);
                    }
                }
            }
            */

            public void create() {
                parameters q_param=generate_crud_parameters();
                string dbQ=string.Format("INSERT into [{0}] ({1}) VALUES ({2})",this._table,this.get_columns(),this.get_params());
                db.execute_non_query("titan",dbQ,q_param);
            }//end create function
   
            public void update() {
                parameters q_param=generate_crud_parameters();
                string dbQ=string.Format("UPDATE [{0}] SET {1} WHERE {2}",this._table,this.get_col_param(),this.get_pk());
                db.execute_non_query("titan",dbQ,q_param);
            }//end update function

            public bool is_in_db() {
                string query=String.Format("SELECT TOP 1 * FROM {0} WHERE {1} ",this._table,this.get_pk());
                parameters q_param=generate_crud_parameters();
                data_set res=db.fetch("titan",query,q_param);
                if (null!=res) return true;
                return false;
            }
            public void save() {
                if(this.is_in_db())    this.update();
                else                   this.create();
            }
            public string generate_insert_query() {
                return "";
            }
            public string generate_update_query() {
                return "";
            }

    }//end partial class
}//end namespace