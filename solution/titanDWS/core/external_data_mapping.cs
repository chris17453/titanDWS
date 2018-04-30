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

namespace titan.core {
    //This is what maps the canned "ref's" to a user defined parameter-> @reference1->@sys_customer_id
    public class external_data_mapping:crud<external_data_mapping> {
        public string name           {  get; set; }
        public string reference1     {  get; set; }
        public string reference2     {  get; set; }
        public string reference3     {  get; set; }
        public string reference4     {  get; set; }
        public string reference5     {  get; set; }
        public string reference6     {  get; set; }
        public string reference7     {  get; set; }
        public string reference8     {  get; set; }
        public string reference9     {  get; set; }
        public string reference10    {  get; set; }
        public string reference11    {  get; set; }
        public string reference12    {  get; set; }
        public string reference13    {  get; set; }
        public string reference14    {  get; set; }
        public string reference15    {  get; set; }
        public bool   active         {  get; set; }
        public  external_data_mapping() {
                _table="titanDWS_external_mapping";
                _pk=new string[] { "name"};
        }
    }//end class
}//end namespace