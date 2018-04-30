<%@ Page Language="C#" AutoEventWireup="True" Async="true" Inherits="titan.preview" MasterPageFile="titanMaster.master" 
         EnableEventValidation="false" Title="TitanDWS: Preview Method" CodeBehind="preview.aspx.cs" %>
<asp:Content ID="titanBody" ContentPlaceHolderID="titanContent" runat="server">
<div class="doc-hero headroom headroom--unpinned">
<div class="container">
    <div class="row">
      <div class="col-md-12">
        <div class="hero-header">
          <div class="eyebrow closed">Titan -> Preview Method</div>
            <div class="right eyebrow closed"><a href="<%=((titan.titanMaster)(this.Master)).titan_url%>/config/<%=group%>/<%=method%>/">Config</a></div>
            <div class="right eyebrow closed" style="margin-right:10px"><a href="<%=((titan.titanMaster)(this.Master)).titan_url%>/dir/">Index</a></div>
        </div>
      </div>
    </div>
  </div>
</div>

    <link href="<%=((titan.titanMaster)(this.Master)).titan_url %>/media/titan/titanDWS.css"  rel="stylesheet" />
    <link href="<%=((titan.titanMaster)(this.Master)).titan_url %>/media/daterangepicker/daterangepicker.css" rel="stylesheet" />
    <link rel="stylesheet" type="text/css" href="<%# ResolveUrl("~/media/PB/design.system.css")                                    %>"  />

<script src="<%=((titan.titanMaster)(this.Master)).titan_url %>/media/daterangepicker/moment-with-locales.min.js"></script>
<script src="<%=((titan.titanMaster)(this.Master)).titan_url %>/media/daterangepicker/daterangepicker.js"></script>
<script src="<%=((titan.titanMaster)(this.Master)).titan_url %>/media/titan/titanDWS_client.js"></script>
<link  href="<%=((titan.titanMaster)(this.Master)).titan_url %>/media/titan/titanDWS.css" rel="stylesheet" />
<div id="json_dialog">
    <div id="json_pre"></div>
</div>
    <style type="text/css">
        .right{ 
            float:right;
        }
        .right a{
            color:white;
        }

        #json_pre{
            width:100%;
            height:100%;
            
        }
        .json_dialog{
            display:none;
        }
        .ui_spinner-ovd{
            border:none!important;
        }
        .ui_spinner-ovd input {
            margin: 0px !important;
        }
        .ui_spinner-ovd .ui-spinner-down {
            border-bottom: solid 1px silver !important;
            border-right: solid 1px silver !important;
        }
        .ui_spinner-ovd .ui-spinner-up {
            border-top: solid 1px silver !important;
            border-right: solid 1px silver !important;
        }
       .child_row{
           display:none;
       }
       .lh-49{
           line-height:37px;
       }
    </style>
    <script type="text/javascript">
        $(function(){
                       generateTitanAjaxObject=function () {                                //this is the ajax post object
                var titanAjaxObject = {};
                titanAjaxObject.check         = '<%=check%>';   
                titanAjaxObject.reference1    = '<%=ref1%>';  
                titanAjaxObject.reference2    = '<%=ref2%>';  
                titanAjaxObject.reference3    = '<%=ref3%>';  
                titanAjaxObject.reference4    = '<%=ref4%>';  
                titanAjaxObject.reference5    = '<%=ref5%>';    
                titanAjaxObject.reference6    = '<%=ref6%>';    
                titanAjaxObject.reference7    = '<%=ref7%>';    
                titanAjaxObject.reference8    = '<%=ref8%>';    
                titanAjaxObject.reference9    = '<%=ref9%>';    
                titanAjaxObject.reference10   = '<%=ref10%>';   
                titanAjaxObject.reference11   = '<%=ref11%>';   
                titanAjaxObject.reference12   = '<%=ref12%>';   
                titanAjaxObject.reference13   = '<%=ref13%>';   
                titanAjaxObject.reference14   = '<%=ref14%>';   
                titanAjaxObject.reference15   = '<%=ref15%>';  
                titanAjaxObject.parameterless = false;
                titanAjaxObject.parameters    = {};
                titanAjaxObject.aggregates    = {};
                titanAjaxObject.group         = '<%=group%>';                    //Group (Controller/class)
                titanAjaxObject.method        = '<%=method%>';                        //Method (function)
                titanAjaxObject.owner         = '<%=owner%>';                        //Method (function)
                titanAjaxObject.parameterless =false;
                titanAjaxObject.configure     ="preview";

                return titanAjaxObject;
            }//end func
            
            titanDWS.loadReport("#titanParameters", "#titanReport");         //this is the actual js call that makes the report
            
        });//end on document ready...

    </script>
    <div id="titan_config" style="width:1200px; margin-left:auto; margin-right:auto; margin-top:20px;">
        <div class="titan_content">
            <div id="titanParameters">
            </div>
            <div id="titanReport">
            </div>
        </div>
    </div>
    
</asp:Content>