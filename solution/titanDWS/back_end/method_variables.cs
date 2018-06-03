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
using System.Reflection;
using System.Runtime.Remoting;
using System.Text;
using nocodedb.data;
using nocodedb.data.models;
using titan.core;

namespace titan {

    public class data:nocodedb.data.crud<data> {
        public string               display                { get; set; }
        public string               group                  { get; set; }
        public string               method                 { get; set; }
        public string               comment                { get; set; }
        public string               owner                  { get; set; }
        public string               title                  { get; set; }
        public string               description            { get; set; }
        public string               output_type            { get; set; }
        public bool                 auto_load              { get; set; }
        public bool                 auto_build             { get; set; }
        public bool                 visible                { get; set; }
        public bool                 export                 { get; set; }
        public int                  order                  { get; set; }
        public bool                 combined_search        { get; set; }
        public bool                 count                  { get; set; }
        public string               query                  { get; set; }
        public List<titan.core.column>     data_schema            { get; set; }
        public List<titan.core.parameter>  parameters             { get; set; }
        public string               template_name          { get; set; }
        public string               menu_name              { get; set; }
        public DateTime             created                { get; set; }
        public DateTime             modified               { get; set; }
        public bool                 active                 { get; set; }
        public bool                 bulk_edit              { get; set; }
        public bool                 quick_edit             { get; set; }
        public bool                 import                 { get; set; }
        public bool                 import_truncate        { get; set; }
        public bool                 import_insert          { get; set; }
        public bool                 import_update          { get; set; }
        public string               connection_string      { get; set; }
        public string               data_mapping_name      { get; set; }
        public bool                 query_is_sp            { get; set; }
        public bool                 cron                   { get; set; }
        public string               schedule               { get; set; }
        public bool                 query_is_single        { get; set; }
        public string               checksum               { get; set; }
        public bool                 data_template          { get; set; }
        public List<linked_method>  linked_methods         { get; set; }
        public bool                 hide_column_search     { get; set; }
        public bool                 use_zebra_rows         { get; set; }
        public bool                 has_footer             { get; set; }
        public string               no_results_msg         { get; set; }
        public bool                 export_sftp            { get; set; }
        public string               sftp_host              { get; set; }
        public string               sftp_user              { get; set; }
        public string               sftp_password          { get; set; }
        public string               sftp_folder            { get; set; }
        public int                  sftp_port              { get; set; }
        public string               sftp_file_name         { get; set; }
        public bool                 export_filesystem      { get; set; }
        public string               file_drive             { get; set; }
        public string               file_folder            { get; set; }
        public string               file_name              { get; set; }
        public string               plugin_name            { get; set; }
        public int                  version                { get; set; }

    //***********************************************************************************************
    //dynamic data

    //dont be a moron
    //a lot of you reading this are stupid
    //look to your left
    //look to your right
    //do you see stupid?
    //if you dont, it's you. You're stupid.
    //DO NOT INIT A CLASS FOR NO REASON. it's a HUGE performance loss.

       
        public string                       export_link         ="";                                //actual export link
        public int                          export_id           =0;                                 //is for a export link
        public bool                         regenerate_columns  =false;                             //should we rebuild the columns (only for save/update/create)
        internal string                     configure           ="";                                //are we in configuration mode?
        internal security.titan_token       token=null;                                             //security token
        internal bool                       execute_init        =false;                             //on this pull do we need to init all data?
        internal bool                       titan_debug         =false;                             //use debuging? changes available JSON data like queries
        public string                       insert_query        ="";                                //query to insert this method
        public string                       update_query        ="";                                //query to update this method
        public linked_method                linked_methods_t    =new linked_method();               //templated for linked methods
        public titan.core.parameter         parameter_template  =new titan.core.parameter();        //JSON template for a parameter
        public data_set                     template            =null;                              //visual template for plugin
        public data_set                     localization        =null;                              //titan UI localiazation for auto page
        public external_data_mapping        external_map        =null;                              //configure custom reference fields
        public data_set                     external_mapping    =null;                              //all maps
        public external_data_defaults       sig_init            =null;                              //init variables when none are available
                                            
        /*These guys are global to reduce duplicate queries for standard data, required in javascript */
        public data_set                     plugins             =null;
        public data_types                   data_types          =new data_types();
        public data_set                     output_types        =null;
        public data_set                     templates           =null;
        public data_set                     result_types        =null;
        public data_set                     parameter_types     =null;
                                             
        public List<linked_method>          methods             =new List<linked_method>();     //all methods available
        public models.method_results        results             =new models.method_results();    //results to return to user
        internal models.lambda              input               =null;
        public data () {
            _table="titanDWS";
            _pk=new string[] { "group","method","owner" };
            _json=new string[] {"data_schema","parameters", "linked_methods"};
 
        }
    }
}