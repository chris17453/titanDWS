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
using titan.core;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using nocodedb.data;
using nocodedb.data.models;

namespace titan {
    public partial class method:data  {
             
         public static method from_json(string text,security.titan_token token,bool save=true) {
                method m=JsonConvert.DeserializeObject<method>(text, new JsonSerializerSettings{
                    Error = delegate(object sender, ErrorEventArgs args){
                        Console.WriteLine(args.ErrorContext.Error.Message);
                        args.ErrorContext.Handled = true;
                    }
                });

                
                if(null==m.data_schema) m.data_schema=new List<titan.core.column>();
                if(null==m.parameters)  m.parameters =new List<titan.core.parameter>();

                m.parameters.RemoveAll(item=> item==null);
                m.parameters.RemoveAll(item=> String.IsNullOrWhiteSpace(item.name));

                parameters param=m.execute_load_parameters(token,null,true);

                if(m.regenerate_columns ) {
                    m.data_schema.Clear();
                    data_set row =db.fetch(m.connection_string,strip_comments(m.query),param);                             //no conn string denotes localhost, aything else is treated as a conn string.. (titan)
                    int index=0;
                    if(null!=row) {                                                     //only update the titan.core.columns if we have atlest 1 result.
                        foreach(string key in row.Keys) {
                                index++;
                                titan.core.column c=new titan.core.column(key,"text",index,true);
                                m.data_schema.Add(c);                                         //only add if it doesnt exist...
                        }
                    }
                    //   if(null!=titan.core.columns) titan.core.columns.RemoveAll(item=> !row.ContainsKey(item.name) );
                }//
                //global.reload();
                if(save) {
                    if(!string.IsNullOrWhiteSpace(m.data_mapping_name)) {
                        if(null==m.sig_init) {
                            m.sig_init=new external_data_defaults();
                            m.sig_init.group  =m.group; 
                            m.sig_init.method =m.method; 
                            m.sig_init.owner  =m.owner;
                        }

                        m.sig_init.save();
                    }
                    m.save();
                    m.generate_queries();

                } else {
                    m.generate_queries();
            }
                return m;
            }
            public static string strip_comments(string sql) {
                var blockComments = @"/\*(.*?)\*/";
                var lineComments = @"--(.*?)\r?\n";
                var strings = @"""((\\[^\n]|[^""\n])*)""";
                var verbatimStrings = @"@(""[^""]*"")+";

                string noComments = Regex.Replace(sql+"\r\n",
                blockComments + "|" + lineComments + "|" + strings + "|" + verbatimStrings,
                me => {
                    if (me.Value.StartsWith("/*") || me.Value.StartsWith("--"))
                        return " ";
                    return me.Value;
                },
                RegexOptions.Singleline);
                return noComments;
            }


    }
}
