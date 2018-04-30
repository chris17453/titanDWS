using System.Collections.Generic;

namespace titan{
    public partial class models {
    public class lambda {
            public string method {get; set; }                  //the funciton
            public string group {get; set; }                   //the functions group
            public string owner {get; set; }                   //the the owner of this method, as there might be multiples
            public string reference1 {get; set; }              //limiter for outside data
            public string reference2 {get; set; }              //limiter for outside data
            public string reference3 {get; set; }              //limiter for outside data
            public string reference4 {get; set; }              //limiter for outside data
            public string reference5 {get; set; }              //limiter for outside data
            public string reference6 {get; set; }              //limiter for outside data
            public string reference7 {get; set; }              //limiter for outside data
            public string reference8 {get; set; }              //limiter for outside data
            public string reference9 {get; set; }              //limiter for outside data
            public string reference10 {get; set; }              //limiter for outside data
            public string reference11 {get; set; }              //limiter for outside data
            public string reference12 {get; set; }              //limiter for outside data
            public string reference13 {get; set; }              //limiter for outside data
            public string reference14 {get; set; }              //limiter for outside data
            public string reference15 {get; set; }              //limiter for outside data
            public string check { get; set; }                   //check hash md5 for ref 1+2
            public bool   admin_auth  {get; set; }              //limiter for outside data
            public string configure {get; set; }             //Which localization to pull.. method or configure method

            public string external_ref      {get; set; }                //What mapping to use for reference fields
            public int page                 {get; set; }                //for page get
            public int pageLength           {get; set; }                //for page get
            public List<string[]> filters   {get; set; }
            public List<string[]> sort      {get; set; }
            public Dictionary<string,string> parameters { get; set; }   //the actual parameters
            public List<string[]>           param { get; set; }   //the actual parameters
            public Dictionary<string,bool> aggregates { get; set; }     //the group by aggregates
            public bool export              { get; set; }
            public int export_id            {get; set; }
            public string export_link       {get; set; }
            public bool in_titan            {get; set; }
            public string crud              {get; set; }
            public string ip                {get; set; }
        }//end model

    }//end class
}//end namespace