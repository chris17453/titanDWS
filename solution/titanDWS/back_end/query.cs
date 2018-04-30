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
using nocodedb.data;
using nocodedb.data.models;

namespace titan {
    public class query {
        public method m=null;
        public class column {
            public string type="";
            public string name="";
            public string aggFunc="";
            public string display="";
                   
            public column(string name,string display,string type,string aggFunc) {
                this.name=name;
                this.display=display;
                this.type=type;
                this.aggFunc=aggFunc;
            }
        }
        public int page=0;
        public int pageLength=100000;
        public List<string> aggregates  =new List<string>();
        public List<column> columns     =new List<column>();
        public string resultsQuery="";
        public string totalQuery="";
        public parameters resultsParameters;
        public parameters totalParameters;
        private List<string> conditions=new List<string>();
       /* public string aggregateColumns {
            get {
                List<string> tCol=new List<string>();
                foreach (column c in columns) {
                    if(c.type=="aggregate") tCol.Add(c.name);
                    if(c.type=="function") tCol.Add(String.Format("{1} as '{0}'",c.name,c.aggFunc));
                }
                if(tCol.Count==0) return "*";
                else return string.Join(",",tCol);
            }
            set { }
        }
        public string aggregateGroupBy {
            get {
                List<string> tCol=new List<string>();
                foreach (column c in columns) {
                    if(c.type=="aggregate") tCol.Add(c.name);
                }
                if(tCol.Count==0) return "";
                else return "GROUP BY "+ string.Join(",",tCol);

            }
            set { }
        }*/
        public int startRow { get { return page*pageLength+1; } set { } }
        public int endRow   { get { return (page+1)*pageLength+1; } set { } }
        public string orderByString ="";

        private string queryWrapper="SELECT * FROM ( SELECT ROW_NUMBER() OVER ( {4} ) AS RowNo, * FROM ( "+
                                "{0} \r\n"+ 
                                ") as WRAPPEDROWS {1} ) as PAGINATED WHERE   RowNo >= {2} AND RowNo < {3} ORDER BY RowNo ";
                
        public string getResultsQuery {
            get {
                    return string.Format(queryWrapper,resultsQuery,where,startRow,endRow);
            }
            set { }
        }
        public string getTotalQuery{
            get{
                    return String.Format("SELECT count(*) AS 'count' FROM (SELECT * FROM ({0}\r\n) as r {1}) AS res OPTION(RECOMPILE)",
                                         resultsQuery,where);        //,aggregateColumns,aggregateGroupBy 
 
            }   
            set{ }
        }
        public void addWhere(string column,string value,string type) {
            conditions.Add(create_condition(column,value,type));
        }

        public string create_condition(string column,string value,string type) {
            switch(type) {
                case "text":     
                case "varchar":  
                case "nvarchar": 
                case "ntext":    
                case "nchar":            
                case "char":     return string.Format("[{0}] LIKE '%{1}%'",column,value);  
                case "datetime": return string.Format("convert(varchar, [{0}], 120) LIKE '%{1}%'",column,value);  
                case "date":     return string.Format("convert(varchar, [{0}], 105) LIKE '%{1}%'",column,value);  
                case "time":     return string.Format("convert(varchar, {0}, 108) LIKE '%{1}%'",column,value);  
                case "bit":      return string.Format("[{0}]= cast(case when '{1}'='true' then 1 else 0 END as bit)",column,value);  
                default:         return string.Format("cast([{0}] as varchar(500)) LIKE '%{1}%'",column,value);  
            }
               
        }
        public string where{
        get {
            if(conditions.Count>0) return "WHERE "+string.Join(" AND ",conditions);
            else return "";
        } set{} }
        /*public List<column> get_columns() {
            //List<string,string> cols=new List<string,string>();
            //foreach(column c in columns) cols.Add(c.name,c.display);
            return cols;
        }*/


           


        public void create_filters(models.lambda input) {
            List<string[]> filters=input.filters;
            if(null==filters) return;                                                   //nothing to do here...
            foreach(string[] d in filters) {
                    
                int index=m.data_schema.FindIndex(x=>x.display==d[0]);
                if(index>=0) {
                    addWhere(m.data_schema[index].name,d[1],m.data_schema[index].type);
                } else {
                    addWhere(d[0],d[1],"number");
                }
            }
            
            if(m.combined_search) {                                             //do we combo search?
                string combined_search="";              
                if(!input.parameters.ContainsKey("multi_search")) return;
                if(!String.IsNullOrWhiteSpace(input.parameters["multi_search"])) combined_search=input.parameters["multi_search"];
                if(String.IsNullOrWhiteSpace(combined_search)) return;          //skip empty searches
                List<string> multi=new List<string>();
                foreach(titan.core.column c in m.data_schema) {
                    if(c.combined_search) multi.Add(create_condition(c.name,combined_search,c.type));
                }
                if(multi.Count>0) {
                    conditions.Add(String.Format("({0})",String.Join(" OR ",multi)));
                }
            }
            //addWhere(multi_search
        }

    }//end query class...
}//end namespace