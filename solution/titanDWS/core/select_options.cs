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

namespace titan.core{
    public class select_option{
        public string value="";
        public string display="";
        public bool _default=false;
        public select_option(){
        }
        public select_option(string value,string display,bool _default) {
            this.value=value;
            this.display=display;
            this._default=_default;
            if(!String.IsNullOrWhiteSpace(value) &&value.Length>0 && _default && value[0]=='*') {
                this.value=value.Substring(1);
                if(value==display) this.display=display.Substring(1);
            }
        }
    }//end class
}//end namespace