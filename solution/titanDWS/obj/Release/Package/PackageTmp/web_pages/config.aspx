<%@ Page Language="C#" AutoEventWireup="True" Async="true" Inherits="titan.config" MasterPageFile="titanMaster.master" 
         EnableEventValidation="false" Title="TitanDWS: Edit Method" CodeBehind="config.aspx.cs" %>
<asp:Content ID="titanBody" ContentPlaceHolderID="titanContent" runat="server">
<div class="doc-hero headroom headroom--unpinned">
<div class="container">
    <div class="row">
      <div class="col-md-12">
        <div class="hero-header">
          <div class="eyebrow closed">Titan -> Configure Method</div>
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
                titanAjaxObject.owner         = '<%=owner%>';  
                titanAjaxObject.admin_auth    =true;                    //use admin authentication method
                titanAjaxObject.in_titan      = true;
                titanAjaxObject.configure   ="configure";

                return titanAjaxObject;
            }//end func
            titanDWS.set_instance("config");
            titanDWS.set_titan_ajax_object(generateTitanAjaxObject());
            titanDWS.load_method();
        });//end on document ready...

    </script>

<br />
<div style="width:1200px; margin-left:auto; margin-right:auto; ">
    <div class="row" style="float:right">
        <div style="float:right"><button type="button" class="save_method btn btn-primary">Save</button></div>
        <div style="float:right; margin-right:10px;"><button type="button" class="test_method btn btn-secondary">Test</button></div>
        <div style="float:right; margin-right:10px;"><button type="button" class="preview_json_out btn btn-secondary">JSON-OUT</button></div>
        <div style="float:right; margin-right:10px;"><button type="button" class="preview_json_in btn btn-secondary">JSON-IN</button></div>
     </div>
</div>
    <br />
    <br />
    <br />
    <br />
</asp:Content>