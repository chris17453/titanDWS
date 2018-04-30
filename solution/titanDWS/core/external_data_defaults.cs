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
    public class external_data_defaults:crud<external_data_defaults> {
        public string method {  get; set; }
        public string group  {  get; set; } 
        public string owner  {  get; set; }
        public string r1     {  get; set; }
        public string r2     {  get; set; }
        public string r3     {  get; set; }
        public string r4     {  get; set; }
        public string r5     {  get; set; }
        public string r6     {  get; set; }
        public string r7     {  get; set; }
        public string r8     {  get; set; }
        public string r9     {  get; set; }
        public string r10    {  get; set; }
        public string r11    {  get; set; }
        public string r12    {  get; set; }
        public string r13    {  get; set; }
        public string r14    {  get; set; }
        public string r15    {  get; set; }
        public bool   active {  get; set; }
        public  external_data_defaults() {
                _table="titanDWS_external_defaults";
                _pk=new string[] { "group","method","owner" };
        }
    }
}