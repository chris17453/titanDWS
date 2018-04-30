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
using System.Collections;
using System.Collections.Generic;
using titan.core;

namespace titan {
    /*
     This stuff litterallyis always loaded. saves on queries like you wouldnt believe.
    */
  /*  public  class global {
        public List<Hashtable>      templates           =template.get();
        public List<Hashtable>      result_types        =column_types.get();
        public List<Hashtable>      parameter_types     =parameter_types.get();
        public List<Hashtable>      output_types        =output_types.get();
        public List<Hashtable>      external_mapping    =external_data_mapping.get_maps();
        public data_types           data_types          =new data_types();

        //sometimes data gets stale. reload it
        public void reload() {
            templates           =template.get();
            result_types        =column_types.get();
            parameter_types     =parameter_types.get();
            output_types        =output_types.get();
            external_mapping    =external_data_mapping.get_maps();
            data_types.init();                
        }
    }*/
}