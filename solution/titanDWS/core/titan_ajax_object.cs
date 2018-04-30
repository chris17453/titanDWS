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
namespace titan.core {
    public class titan_ajax_object {
        
        public string group ="";
        public string method="";
        public string owner ="";
        public string ref1  ="";
        public string ref2  ="";
        public string ref3  ="";
        public string ref4  ="";
        public string ref5  ="";
        public string ref6  ="";
        public string ref7  ="";
        public string ref8  ="";
        public string ref9  ="";
        public string ref10 ="";
        public string ref11 ="";
        public string ref12 ="";
        public string ref13 ="";
        public string ref14 ="";
        public string ref15 ="";
        public string check {
            get { 
                 return titan.security.MD5((ref1+ref2+ref3+ref4+ref5+ref6+ref7+ref8+ref9+ref10+ref11+ref12+ref13+ref14+ref15) );
                }
            set {
            }
        }

        public titan_ajax_object (){
            

        }//end init
    }//end class
}//end namespace