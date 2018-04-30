/******************************************************************** 
  ████████╗██╗████████╗ █████╗ ███╗   ██╗██████╗ ██╗    ██╗███████╗
  ╚══██╔══╝██║╚══██╔══╝██╔══██╗████╗  ██║██╔══██╗██║    ██║██╔════╝
     ██║   ██║   ██║   ███████║██╔██╗ ██║██║  ██║██║ █╗ ██║███████╗
     ██║   ██║   ██║   ██╔══██║██║╚██╗██║██║  ██║██║███╗██║╚════██║
     ██║   ██║   ██║   ██║  ██║██║ ╚████║██████╔╝╚███╔███╔╝███████║
     ╚═╝   ╚═╝   ╚═╝   ╚═╝  ╚═╝╚═╝  ╚═══╝╚═════╝  ╚══╝╚══╝ ╚══════╝
*********************************************************************
   Created: 01-23-2017
   Author : Charles Watkins
   Email  : charles.watkins@pb.com
********************************************************************/
using System.Collections.Generic;

namespace titan {
    public partial class models {
        public class method_results {
            public bool                     success     =false;
            public int                      page        =0;
            public int                      pageLength  =10;
            public int                      total_rows  =0;
            public string                   combined_search="";
            public string[]                 sort;
            public string[]                 filter;
            public List<query.column>       columns     =new List<query.column>();
            public Dictionary<string, int>  keys        =new Dictionary<string, int>();
            public Dictionary<string,string>parameters  =new Dictionary<string, string>();
            public List<string[]>           rows        =new List<string[]>();
            public List<string>             error       =new List<string>();
            public int export_id                        =0;
            public string export_link                   ="";
            public string executed_query                ="";
        }//end class
    }//end model class
}//end namespace