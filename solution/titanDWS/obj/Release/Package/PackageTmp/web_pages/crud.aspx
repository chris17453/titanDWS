<%@ Page Language="C#" AutoEventWireup="True" Async="true" MasterPageFile="titanMaster.master" 
         EnableEventValidation="false" Title="TitanDWS: Crud Preview" CodeBehind="crud.aspx.cs" Inherits="titan.web_pages.crud" %>
<asp:Content ID="titanBody" ContentPlaceHolderID="titanContent" runat="server">
<div class="doc-hero headroom headroom--unpinned">
<div class="container">
    <div class="row">
      <div class="col-md-12">
        <div class="hero-header">
          <div class="eyebrow closed">Titan -> Crud Preview</div>
            <div class="right eyebrow closed"><a href="<%=((titan.titanMaster)(this.Master)).titan_url%>/preview/<%=group%>/<%=method%>/">Preview</a></div>
            <div class="right eyebrow closed" style="margin-right:10px"><a href="<%=((titan.titanMaster)(this.Master)).titan_url%>/dir/">Index</a></div>

        </div>
      </div>
    </div>
  </div>
</div>
<script src="<%=((titan.titanMaster)(this.Master)).titan_url%>/media/titan/titanDWS_core.js"></script>
<script src="<%=((titan.titanMaster)(this.Master)).titan_url%>/media/titan/titanDWS_ui.js"></script>
<script src="<%=((titan.titanMaster)(this.Master)).titan_url%>/media/titan/titanDWS_crud.js"></script>
<script src="<%=((titan.titanMaster)(this.Master)).titan_url%>/media/titan/titanDWS_config.js"></script>
<script src="<%=((titan.titanMaster)(this.Master)).titan_url%>/media/ace/src-noconflict/ace.js" type="text/javascript" charset="utf-8"></script>

<div id="titan_msg"  class="titan_msg"></div>
<div id="titan_crud" style="width:1200px; margin-left:auto; margin-right:auto; margin-top:20px;">
</div>
<div id="json_dialog">
    <div id="json_pre"></div>
</div>
    <style type="text/css">
        #config_query{
            
            position: relative !important;
            border: 1px solid lightgray;
            margin: auto;
            height: 600px;
            width: 100%;
        }
        .right{ float:right;

        }
        .right a{
            color:white;
        }

        .t-column{
            width:500px;
            float:left;
        }
        .t-column .col-xs-4{
            width:100% !important;
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
       .query{
           height:600px;
           background:lightblue;
           color:green;
           border-style:solid;
           border-collapse:collapse;
       }
    </style>
    <script type="text/javascript">
        $(function(){
            generateTitanAjaxObject=function () {                                //this is the ajax post object
                var titanAjaxObject = {};
                titanAjaxObject.check         = '<%=crud_obj.check%>';   
                titanAjaxObject.reference1    = '<%=crud_obj.ref1%>';  
                titanAjaxObject.reference2    = '<%=crud_obj.ref2%>';  
                titanAjaxObject.reference3    = '<%=crud_obj.ref3%>';  
                titanAjaxObject.reference4    = '<%=crud_obj.ref4%>';  
                titanAjaxObject.reference5    = '<%=crud_obj.ref5%>';    
                titanAjaxObject.reference6    = '<%=crud_obj.ref6%>';    
                titanAjaxObject.reference7    = '<%=crud_obj.ref7%>';    
                titanAjaxObject.reference8    = '<%=crud_obj.ref8%>';    
                titanAjaxObject.reference9    = '<%=crud_obj.ref9%>';    
                titanAjaxObject.reference10   = '<%=crud_obj.ref10%>';   
                titanAjaxObject.reference11   = '<%=crud_obj.ref11%>';   
                titanAjaxObject.reference12   = '<%=crud_obj.ref12%>';   
                titanAjaxObject.reference13   = '<%=crud_obj.ref13%>';   
                titanAjaxObject.reference14   = '<%=crud_obj.ref14%>';   
                titanAjaxObject.reference15   = '<%=crud_obj.ref15%>';  
                titanAjaxObject.parameterless = false;
                titanAjaxObject.parameters    = {};
                titanAjaxObject.aggregates    = {};
                titanAjaxObject.group         = '<%=crud_obj.group%>';                      //Group (Controller/class)
                titanAjaxObject.method        = '<%=crud_obj.method%>';                     //Method (function)
                titanAjaxObject.owner         = '<%=crud_obj.owner%>';  
                titanAjaxObject.admin_auth    =true;                                        //use admin authentication method
                titanAjaxObject.in_titan      = true;
                titanAjaxObject.configure   ="";                                            //not configuring...

                return titanAjaxObject;
            }//end func
            titanDWS.set_instance("config");
            titanDWS.set_titan_ajax_object(generateTitanAjaxObject());
            titanDWS.load_method();
        });//end on document ready...

    </script>

<br />
<div style="width:1200px; margin-left:auto; margin-right:auto; ">
</div>
    <br />
    <br />
    <br />
    <br />
</asp:Content>