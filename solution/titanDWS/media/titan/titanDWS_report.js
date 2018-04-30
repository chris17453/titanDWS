/******************************************************************** 
  ████████╗██╗████████╗ █████╗ ███╗   ██╗██████╗ ██╗    ██╗███████╗
  ╚══██╔══╝██║╚══██╔══╝██╔══██╗████╗  ██║██╔══██╗██║    ██║██╔════╝
     ██║   ██║   ██║   ███████║██╔██╗ ██║██║  ██║██║ █╗ ██║███████╗
     ██║   ██║   ██║   ██╔══██║██║╚██╗██║██║  ██║██║███╗██║╚════██║
     ██║   ██║   ██║   ██║  ██║██║ ╚████║██████╔╝╚███╔███╔╝███████║
     ╚═╝   ╚═╝   ╚═╝   ╚═╝  ╚═╝╚═╝  ╚═══╝╚═════╝  ╚══╝╚══╝ ╚══════╝
*********************************************************************
   Created: 01-23-2017
   Author : Charles Watkins
   Email  : charles.watkins@pb.com
********************************************************************/
//titanDWS report plugin, core functionality


class titanDWS_report {

    constructor(results,params) {
        this.webServiceURL    = $("[id$=titanWebService]").val();         //ajax api utl
        this.instance         = "method";                                 //method prefix
        this.webServiceURL    = "";                                       //ajax api url
        this.m                = {};                                       //method data object
        this.data_object      = {};                                       //ajax object
        this.JWT              = "";                                       //Javascript web token
        this.paramDiv         = "";
        this.resultsDiv       = "";
        this.calendar         = "";
        this.data_object      = {};
        this.titan_loaded     = false;
        this.resultsDiv       = results;
        this.paramDiv         = params;
        this.default_option   = null;
    }
 
    padd (d, len) {
        var o = "";
        for (a = d.length; a < len; a++) o += "_";
        return d + o;
    }

    set_JWT(data){
        this.JWT=data;
    }

    get_JWT(data){
        return this.JWT;
    }
    
    set_method(data){
        this.m=data;
    }
    add_parameter(name,value){
        this.data_object.parameters[name]=value;
    }

    load(method,success_func){
        if (this.webServiceURL===undefined || this.webServiceURL==="")  this.webServiceURL=$("[id$=titanWebService]").val();
        $.ajax({
            type: "POST", url: this.webServiceURL +""+ method,data: JSON.stringify(this.data_object), headers:{'JWT':this.JWT},
            success: function(results){ if(success_func){success_func(results);} }, error: this.error,contentType:"application/json", dataType: "json" });
    }//end load
    refresh(){
        this.update_data();
    }
    update_data(){
        if($(this.resultsDiv).find('table').length) {
            var table=$(this.resultsDiv).find('table')
            table.trigger('pagerUpdate');                              
            table.trigger('Update');                              
            this.api_out();
        } else {
         // no table found
        }
    }

    create_signature(group,methodName,owner,
                     check,ref1,ref2 ,ref3 ,ref4 ,ref5 ,ref6 ,ref7,
                     ref8 ,ref9,ref10,ref11,ref12,ref13,ref14,ref15) {                                //this is the ajax post object
        this.data_object = {};
        this.data_object.check         = check;   
        this.data_object.reference1    = ref1;  
        this.data_object.reference2    = ref2;  
        this.data_object.reference3    = ref3;  
        this.data_object.reference4    = ref4;  
        this.data_object.reference5    = ref5;    
        this.data_object.reference6    = ref6;    
        this.data_object.reference7    = ref7;    
        this.data_object.reference8    = ref8;    
        this.data_object.reference9    = ref9;    
        this.data_object.reference10   = ref10;   
        this.data_object.reference11   = ref11;   
        this.data_object.reference12   = ref12;   
        this.data_object.reference13   = ref13;   
        this.data_object.reference14   = ref14;   
        this.data_object.reference15   = ref15;  
        this.data_object.parameters    = {};
        this.data_object.param         = {};
        this.data_object.aggregates    = {};
        this.data_object.group         = group;                        //Group (Controller/class)
        this.data_object.method        = methodName;                   //Method (function)
        this.data_object.owner         = owner;                        //Method (function)
        this.data_object.parameterless=false;
    }//end func
    
    get_token(func){
        this.get_token_function=func;
        var internal_class=this;
        

        if(this.JWT===undefined || this.JWT==="") {
            this.load("get_token",  function (json_result) {
                if(json_result) {
                    internal_class.JWT=json_result;
                    if(internal_class.get_token_function!==undefined || internal_class.get_token_function!==null){
                        internal_class.get_token_function();
                        internal_class.get_token_function=null;                           //delete this so its not reused.
                    }//end if
                }//end if
            });
        } else {
            if(this.get_token_function!==undefined || this.get_token_function!==null){
                this.get_token_function();
                this.get_token_function=null;                           //delete this so its not reused.
            }
        }//end if
     }//end func


     load_report() {
        this.titan_loaded=true;
        var internal_class=this;
        if(this.JWT===undefined || this.JWT==="") {
            this.load("get_token",  function (json_result) {
                if(json_result) {
                    internal_class.JWT=json_result;
                    internal_class.load("fetch_method", function (jsonResult) {
                            internal_class.m = jsonResult.results;
                            internal_class.buildReport();
                            if (internal_class.m.parameters.length === 0 || internal_class.m.auto_load===true) {                                          //autoload if no input needed
                                internal_class.load_data();
                            }
                            document.title = internal_class.m.title;
                            internal_class.initTableSorter();
                        })//enf fetch;   
                }//end if json
            });   //end get token ajax
        } else {
            internal_class.load("fetch_method", function (jsonResult) {
                            internal_class.m = jsonResult.results;
                            internal_class.buildReport();
                            if (internal_class.m.parameters.length === 0 || internal_class.m.auto_load===true) {                                          //autoload if no input needed
                                internal_class.load_data();
                            }
                            document.title = internal_class.m.title;
                            internal_class.initTableSorter();
                        })//enf fetch;   
        }
    }//end get token


    load_calendar(){
        var internal_class=this;
        if(this.JWT===undefined || this.JWT==="") {
            this.load("get_token",  function (json_result) {
                if(json_result) {
                    internal_class.JWT=json_result;
                    internal_class.load("lambda", function (calendar_result) {
                            internal_class.calendar_success(calendar_result);
                        })//enf fetch;   
                }//end if json
            });   //end get token ajax
        } else {
            internal_class.load("lambda", function (calendar_result) {internal_class.calendar_success(calendar_result);}); //end fetch;   

        }
    }//end get token
    set_default_option(value,text) {
        this.default_option={};
        this.default_option.value=value;
        this.default_option.text=text;
    }
    load_selector(){
        var internal_class=this;
        if(this.JWT===undefined || this.JWT==="") {
            this.load("get_token",  function (json_result) {
                if(json_result) {
                    internal_class.JWT=json_result;
                    internal_class.load("lambda", function (selector_result) {
                            internal_class.selector_success(selector_result);
                        })//enf fetch;   
                }//end if json
            });   //end get token ajax
        } else {
            internal_class.load("lambda", function (selector_result) { internal_class.selector_success(selector_result); })//end fetch;   
                }
    }//end get token

    load_data(func){
        var internal_class=this;
        var func_to_run=func;
        if(this.JWT===undefined || this.JWT==="") {
            this.load("get_token",  function (json_result) {
                if(json_result) {
                    internal_class.JWT=json_result;
                    internal_class.load("lambda", function (lamda_result) {
                            func_to_run(lamda_result);
                        })//enf fetch;   
                }//end if json
            });   //end get token ajax
        } else {
            internal_class.load("lambda", function (lamda_result) { if(func_to_run!==undefined) func_to_run(lamda_result); });//end fetch;   
        }
    }//end get token

    build_report_filters(){
        var filterDefaults=[];
        var thead ="";
        var tfoot ="";
        var name  ="";
        for (var index in this.m.columns) {
            if (!this.m.columns[index].visible) continue;
            thead += "<th ";
            if (!this.m.columns[index].filter) thead += "data-filter=\"false\" ";
            if (!this.m.columns[index].sort  ) thead += "data-sorter=\"false\" ";
            if ( this.m.columns[index].filter && this.m.columns[index].selectOptions.length>0) {      //if filter and has select options
                thead +=" class=\"filter-select\" " ;
            }
            if(this.m.columns[index].selectOptions.length>0){
                for(var oindex in this.m.columns[index].selectOptions){
                    if(this.m.columns[index].selectOptions[oindex]._default){
                        thead+="data-value=\""+this.m.columns[index].selectOptions[oindex].value+"\"";
                        filterDefaults[index]= this.m.columns[index].selectOptions[oindex].value;
                        }
                }
            }
            name = this.m.columns[index].name;
            if (this.m.columns[index].alias !== "") name = this.m.columns[index].alias;
            if (name==="*") name = "";
            thead += ">" + name + "</th>";
            tfoot += "<td>" + name + "</td>";
        }
        return {'filterDefaults':filterDefaults,'thead':thead,'tfoot':tfoot};
    }

    build_report_params(){
        var glyph   ="glyphicon glyphicon-pencil";
        var type    ="";
        var isHidden=false;
        var params  ="";
        for (var index in this.m.parameters) {
            if(this.m.parameters[index].type==="hidden") {
                isHidden=true;
                continue;
            }
            if(this.m.parameters[index].type==='date') glyph="glyphicon-calendar";

            params += "<tr>"; //<td>" + m.parameters[index].display + "</td>
            params += "<td title='"+ this.m.parameters[index].desc+"'><div class='titan_parameter_box input-group'>    <span class='input-group-addon titan-param-name' ><i class='glyphicon "+
                                    glyph+"'></i>&nbsp;"+ 
                                    this.m.parameters[index].display+"</span><input id=\"" + this.instance+"_"+ this.m.parameters[index].htmlName + "\" class=\"form-control  ";
            if (this.m.parameters[index].type === "date") params += "titan_date ";
            if (this.m.parameters[index].type === "daterange") params += "titan_date_range ";
            var _default="";
            if(this.m.parameters[index].type!=="date") {
                _default=this.m.parameters[index]._default;
            } 
            params += "\" type=\"text\" value=\""+_default+"\" /></div></td>";
            params += " </tr>";
        }

        if(isHidden){
            params += "<tr><td colspan=2>";
            for (index in m.parameters) {
                if(m.parameters[index].type!=="hidden") continue;
                params += "<input type=\"hidden\" id=\"" + this.instance+"_"+this.m.parameters[index].htmlName + "\" value=\""+_default+"\" />";
            }
            params += "</td> </tr>";
        }
        return params;
    }

    build_report_aggregates(){
        //For Group by..... name.alias.htmlgroup
        // for the love of smething... lets get the pivot table stuff somewhere else.
        var aggregate = "";
        if (this.m.aggregates && this.m.aggregates.length > 0) {
            aggregate += "<tr><td><button type='button' class='titan_activate_groups'>Group</button></td><td><div class='titan_aggregates'><div class='titan_group_list'>";

            for (index in this.m.aggregates) {
                aggregate +=
                                    "<div class='titan_aggregate titan_fl titan_ml'><div class='titan_left squaredOne'>" +
                                    "<input type='checkbox' class='form-control 'id='" + this.m.aggregates[index].htmlName + "' />" +
                                    "<label for='" + this.m.aggregates[index].htmlName + "'></label>" +
                                    "</div><div class='titan_label'>" + this.m.aggregates[index].display + "</div></div>";
            }
            aggregate += "</div></div><div class='titan_group_order'></div></td></tr>";
        }
        return aggregate;
    }

    build_table_css(){
        var tableWidth = "<style type='text/css'>\n\r";
        tableWidth += this.resultsDiv+" .tablesorter {width: 100%;}\n\r";
        var i=0;
        for (var index in this.m.columns) {
            if (!this.m.columns[index].visible) continue;
            if (this.m.columns[index].width === "") continue;
            tableWidth +=this.resultsDiv+" .tablesorter td:nth-of-type(" + (parseInt(i)+1) + ") {";
            if(this.m.columns[index].overflow) tableWidth +="overflow:visible !important; ";
            if(this.m.columns[index].fixed_width!==true) tableWidth += "max-width: " + this.m.columns[index].width + "px;"
            if(this.m.columns[index].width!=="0") {
                tableWidth += "width: " + this.m.columns[index].width + "px;";
            }
            tableWidth += "}\r\n";
            i++;
        }
        tableWidth += "</style>"
        return tableWidth;
    }

    inject_multi_search(){
        var injectTargetComboSearch = "{$combined_search}";
        if(this.m.combined_search) {
            this.m.template.report=this.m.template.report.replace(injectTargetComboSearch,
            '<div class="input-group"><div class="input-group-addon"><i class="nc-icon-mini ui-1_zoom"></i></div>\
            <input id="'+this.instance+"_"+'multi_search" class="form-control search " type="text" placeholder="Search" /></div>');
        } else {
            if(null!==this.m.template && null!==this.m.template.report){
                this.m.template.report=this.m.template.report.replace(injectTargetComboSearch,'');
            }
        }
    }

    display_report(thead,tfoot,params,styles){
        var injectTargetThead       = "{$thead}";
        var injectTargetTfoot       = "{$tfoot}";
        var injectTargetName        = "{$name}";
        var injectTargetDesc        = "{$desc}";
        var injectTargetParameters  = "{$parameters}";

        if(null!==this.m.template ){
            if(null!==this.m.template.report){
                this.m.template.report =styles + this.m.template.report.replace(injectTargetThead, thead);
                if(!this.m.has_footer) tfoot="";
                this.m.template.report = this.m.template.report.replace(injectTargetTfoot, tfoot);

            }
            if(null!==this.m.template.parameters){
                this.m.template.parameters = this.m.template.parameters.replace(injectTargetName, this.m.name);
                $(this.resultsDiv).html(this.m.template.report);
                this.m.template.parameters = this.m.template.parameters.replace(injectTargetDesc, this.m.desc);
                this.m.template.parameters = this.m.template.parameters.replace(injectTargetParameters, params);
                $(this.paramDiv).html(this.m.template.parameters);                            //inject into page
            }
        }
    };
    
    api_out(){
        if(this.load_run) this.load_run();
    }
    
    reset_date_pickers(){
        var index;
        for (index in this.m.parameters) {
            if(this.m.parameters[index].type==="date") {
                _default=this.m.parameters[index]._default;
                $("#"+this.instance+"_"+this.m.parameters[index].htmlName).datepicker("setDate",_default); 
            } 
        }
    }

    reset_date_range(){
        var start = moment().subtract(7, 'days');
        var end = moment();
        var titan_report=this;
        function cb(start, end) {
            $('.titan_date_range span').html(start.format('MMMM D, YYYY') + ' - ' + end.format('MMMM D, YYYY'));//.promise().done(update_data);
            setTimeout(function(){ titan_report.update_data(); }, 200); //html not available asap?

        }
        $('.titan_date_range ').daterangepicker({
            locale: {
                format: 'MM/DD/YYYY'
            },
            "opens": "left",
            startDate: start,
            endDate: end,
            ranges: {
                'Today': [moment(), moment()],
                'Yesterday': [moment().subtract(1, 'days'), moment().subtract(1, 'days')],
                'Last 7 Days': [moment().subtract(6, 'days'), moment()],
                'Last 30 Days': [moment().subtract(29, 'days'), moment()],
                'This Month': [moment().startOf('month'), moment().endOf('month')],
                'Last Month': [moment().subtract(1, 'month').startOf('month'), moment().subtract(1, 'month').endOf('month')]
            }
        }, cb);

        cb(start, end);
    }

    hide_empty_params(params){
        if (params.length=== 0) {                      //hide parameter if nothing to do
            $(".titan_report_params").hide();
        }
    }
    
    init_date_picker(){
        $(".titan_date").datepicker();
    }
   
    auto_load(){
        //if(this.m.auto_load) this.load_data();    //if this is an autoload function. Just run the report.
    }
    
       
    buildReport () {
        if (!this.m) {
            alert("Not a valid Report");
            return;
        }
        //Generating html
        var filter_v        =this.build_report_filters();
        var filterDefaults  =filter_v.filterDefaults;
        var thead           =filter_v.thead;
        var tfoot           =filter_v.tfoot;
        var params          =this.build_report_params();
        var styles          =this.build_table_css();
        
        //running logic
        this.inject_multi_search();                                           //add group spanning multi search
        this.display_report(thead,tfoot,params,styles);                       //put the html on the webpage
        this.hide_empty_params(params);                                       //hide paramaters that are empty.
        this.init_date_picker();        
        this.reset_date_range();                                              //reset single date pickers
        this.reset_date_pickers();                                            //reset date range controls
        this.api_out();                                                       //do something with external stuff??
        this.auto_load();                                                     //if this function auto runs.. lets launch it
        this.bind();                                                          //attach auto events
    
    }
   
    calendar_success(jsonResult) {
        var results= jsonResult.results;
        var rows=results.rows;
        var head=results.columns;
        var dateIndex=0;
        var date=date;
        var value="";

        for(var column in head) {
            if(head[column].name==="date") dateIndex=column;
        }
                    
        for(var row in rows) {
            for(column in rows[row]) {
                date=rows[row][dateIndex];
                value=rows[row][column];
                switch(head[column].name) {
                    case 'c1': $('.day[data-date="'+date+'"] > .c1').html(value); break;
                    case 'c2': $('.day[data-date="'+date+'"] > .c2').html(value); break;
                    case 'c3': $('.day[data-date="'+date+'"] > .c3').html(value); break;
                    case 'c4': $('.day[data-date="'+date+'"] > .c4').html(value); break;
                    case 'c5': $('.day[data-date="'+date+'"] > .c5').html(value); break;
                    case 'c6': $('.day[data-date="'+date+'"] > .c6').html(value); break;
                    case 'c7': $('.day[data-date="'+date+'"] > .c7').html(value); break;
                    case 'c8': $('.day[data-date="'+date+'"] > .c8').html(value); break;
                    case 'c9': $('.day[data-date="'+date+'"] > .c9').html(value); break;
                }
            }
        }
    }

    selector_success(jsonResult) {
        var results= jsonResult.results;
        var rows=results.rows;
        var head=results.columns;
        var desc_index=-1;
        var val_index=-1;
        var d1_index=-1;
        var desc="";

        for(var column in head) {
            if(head[column].name==="d1")   d1_index=column;
            if(head[column].name==="value") val_index=column;
            if(head[column].name==="desc") desc_index=column;
        }
            
            var options="";
        if(this.default_option===null) {
            this.set_default_option("","Choose One");
        }
            options+="<option value=''></option>";

        for(var row in rows) {
            
            if(d1_index!==-1) desc=rows[row][d1_index]+"|"+rows[row][desc_index];
            else              desc=rows[row][desc_index];
            if(null!==this.sel_value && rows[row][val_index]===this.sel_value) {
                
                options+="<option selected='selected' value='"+rows[row][val_index]+"'>"+desc+"</option>";
            } else {
                options+="<option value='"+rows[row][val_index]+"'>"+desc+"</option>";
            }
        }

        $(this.resultsDiv).html('<select id="'+this.resultsDiv.substring(1)+'_sel">'+options+'</select>')
        $(this.resultsDiv+"_sel").combobox({place_holder:this.default_option.text,default_value:this.sel_value});
      //  this.set_value(this.sel_value);
        
    }

    set_value(data){
        if(null===data || undefined===data) return;
        this.sel_value=data;
        $(this.resultsDiv+"_sel option[value='"+data+"']").prop('selected', true);

        $(this.resultsDiv+'_sel').val(data);
        $(this.resultsDiv+'_sel').trigger('select');
        $(this.resultsDiv+' input ').val(data)
        
    }
    get_value(){
        return $(this.resultsDiv+'_sel').val();
    }
    bind(){
        var rd=this.resultsDiv;
        var ti=this.instance;
        $("body").on("keyup","#"+ti+"_"+'multi_search',function(event){
            if (event.keyCode === 13) {
                var table=$(rd+" table");

                $(table).trigger('pagerUpdate');                               //update pager
                $(table).trigger('Update');                                    //update pager
                return false;
            }
        });
    }
   /*********************************************************************************************/
   /* TableSorter plugin init and ajax handeling */
   /*********************************************************************************************/
    table_sorter_custom_ajax (table, url) {
        var obj         = table.config.pager.ajaxObject;
        var page        = table.config.pager.page;
        var pageLength  = table.config.pager.size;
        var sort        = [];
        var filter      = [];
        var key, data, item;
        var dir="";
        var titan_report=this.titan_report;
        var sortListTemp=table.config.sortList;
        var dataObj     =this.titan_report.data_object;

        for (item in table.config.sortList) {

            key  = table.config.sortList[item][0];
            data = table.config.sortList[item][1];
            if(this.titan_report.m.new_columns) {                                                        //columns may have changed use that set
                for(var index in this.titan_report.m.new_columns) {
                    dir="";
                    if(data===1) dir="DESC";
                    if(data===0) dir="ASC";
                    if(key===parseInt(index)) sort.push([this.titan_report.m.new_columns[index].name, dir]);
                }
            } else {                                                                       //use origonal column set
                for(index in method.columns) {
                    dir="";
                    if(data===1) dir="DESC";
                    if(data===0) dir="ASC";
                    if(key===parseInt(index)) sort.push([this.titan_report.m.columns[index].name, dir]);
                }
            }
        }
               
        for (item in table.config.lastSearch) {
            key = item;
            data = table.config.lastSearch[item];
            if (data.trim() === "") continue;
            if(this.titan_report.m.new_columns) {
                for(index in this.titan_report.m.new_columns) {
                    if(key===index) filter.push([this.titan_report.m.new_columns[index].name, data]);
                }
            }else {
                for(index in this.titan_report.m.columns) {
                    if(key===index) filter.push([this.titan_report.m.columns[index].name, data]);
                }
            }
        }
        
        if(this.titan_report.data_object.parameterless === false) {
            this.titan_report.data_object.parameters = {};
            $('[id^='+this.titan_report.instance+'Titan_Parameter_]').each(function () {
                titan_report.data_object.parameters[this.id] = this.value;
            });
        }

        this.titan_report.data_object.aggregates = {};
        /*$('[id^=Titan_Aggregate_]').each(function () {
            if(this.checked) titan_report.data_object.aggregates[this.id] = this.checked;
        });*/
                    
        this.titan_report.data_object.aggregates = {};
        this.titan_report.data_object.parameters['multi_search']=$("#"+this.titan_report.instance+"_"+"multi_search").val();
        dataObj.page        = page;
        dataObj.pageLength  = pageLength;
        dataObj.sort        = sort;
        dataObj.filters     = filter;
        obj.data            = JSON.stringify(dataObj);
        table.config.pager.ajaxObject = obj;  //?
        return url;
    }
    ajax_processing(data) {
        if (data && data.hasOwnProperty('results')) {
            data = data.results;
            if(!data) return;
            if(data.export_link!=='' && data.export_id!=='0') $(".titan_export").html(data.export_link);
            var index, r, row, c, d = data.rows,
            total = data.total_rows,
            rows = [],
            len = d.length;
            if(len===0) {
               // $.tablesorter.showError(this.titan_report.table, "No Data Found");d

            }
            for (r = 0; r < len; r++) {
                row = []; // new row array
                index = 0;
                for (c in d[r]) {
                    row[index] = "<span>" + d[r][c] + "</span>";
                    index++;
                }
                rows.push(row); // add new row array to rows array
            }
            var rebuild=false;
            if(this.titan_report.m.new_columns && data.columns.length!==this.titan_report.m.new_columns.length) rebuild=true;
                        
            if(rebuild){            //only redo the header if the columncount changes. causes odd sorting behvior
                
                this.table.find("thead tr").remove();
                this.table.find("tfoot tr").remove();
                tr = "<tr class='tablesorter-header'>";
                var name="";
                for (index in data.columns_display) { 
                    name=data.columns[index];
                    if(name==="*") name="";
                    tr += "<th class='tablesorter-header'>" + name + "</th>"; 
                }
                tr += "</tr>";
                this.table.find("thead").html(tr);
                no_results_msg
                this.table.trigger('filterReset');
                this.table.trigger('sortReset');         //reset sort and filter
                this.table.trigger('pageAndSize', [1, 10]);                     //reset pagination and size of pages
                this.table.trigger("updateAll");
            }
            this.titan_report.m.new_columns=data.columns;
            var cA=[];
            for(var i in data.columns) {
                name=data.columns[i].display;
                if(name==="*") name="";
                cA.push(name);
            }
            return [data.total_rows, rows, cA];
        }
    }

    initTableSorter () {
        //Ajax tablesorter init
        var filter_object={};
        for (var index in this.m.columns) {
            if (!this.m.columns[index].visible) continue;

            if (this.m.columns[index].filter && this.m.columns[index].selectOptions.length>0) {      //if filter and has select options
                filter_object[index]=[];
                var selectObject={};
                for(var oindex in this.m.columns[index].selectOptions){
                    var display=this.m.columns[index].selectOptions[oindex].display;
                    var value  =this.m.columns[index].selectOptions[oindex].value;
                    var key    =display;                                                        //todo for future update. make display value work! not supported in tablesorter.
                    selectObject[key]=display;
                }
                filter_object[parseInt(index)]=selectObject;
            }
        }//end index
        var widgets=[];
        var widget_options={};
        if(this.m.use_zebra_rows)      {
            widget_options.zebra=["even", "odd"];
            widgets.push("zebra");
        }
        if(!this.m.hide_column_search) {
            widgets.push("filter");
            widget_options.filter_reset=".reset";
            widget_options.filter_functions=filter_object;
            widget_options.filter_saveFilters= true;
        }

        if($(this.resultsDiv).find('table').length) {
            this.table=$(this.resultsDiv+" table");
            this.table.tablesorter({
                titan_report        : this,
                theme: "table",
                widthFixed: true,
                widgets: widgets,
                widgetOptions:widget_options,
            })
            .on('filterEnd filterReset pagerComplete', function(e, table2){
                var fr, table = this;
                if (table.config.pager) {
                    if($.tablesorter.showInfo){
                        $.tablesorter.showError(table);
                        fr = table.config.pager.filteredRows;
                    
                        if (fr === 0) {
                            $.tablesorter.showInfo(table, this.config.titan_report.m.no_results_msg);
                        } else {
                            $.tablesorter.removeInfo(table);
                        }
                    }//if function exists
                }
            })
            .tablesorterPager({
                container           : $(".pager"),
                ajaxUrl             : this.webServiceURL + "/" + "lambda",
                customAjaxUrl       : this.table_sorter_custom_ajax,
                ajaxError           : null,
                ajaxObject          : { contentType: "application/json", method: "POST", dataType: 'json',headers:{'JWT':this.JWT} },
                ajaxProcessing      : this.ajax_processing,
                processAjaxOnInit   : true,
                output              : '{startRow} to {endRow} ({totalRows})',
                updateArrows        : true,
                page                : 0,
                size                : 10,
                savePages           : true,
                storageKey          : 'tablesorter-pager',
                pageReset           : 0,
                fixedHeight         : false,
                removeRows          : false,
                countChildRows      : false,
                cssNext             : '.next',
                cssPrev             : '.prev',
                cssFirst            : '.first',
                cssLast             : '.last',
                cssGoto             : '.gotoPage',
                cssPageDisplay      : '.page_display',
                cssPageSize         : '.page_size',
                cssDisabled         : 'disabled',
                cssErrorRow         : 'tablesorter-errorRow',
                titan_report        : this
                });
            }//end if

            if(this.resultsDiv!=="") {
                $(this.resultsDiv+" .titanData").show();
            }
            //this.update_data();
        }//end load data...
   
    reset(){
        $("#"+this.instance+"_"+"multi_search").val('');                    //update multi search
        this.reset_date_range();      
        this.table.trigger('sortReset');                                //reset sort and filter
        $.tablesorter.setFilters( this.table, this.filterDefaults, true );
        this.table.trigger('pageAndSize', [1, 10]);                     //reset pagination and size of pages
        this.table.trigger('pagerUpdate');                              //update pager
        this.table.trigger('Update');                                   //update pager
    };
     
    on_load(func){
        this.load_run=func;
    };

   
}

$( function() {
    $.widget( "custom.combobox", {
        options:{
            place_holder:"Choose One",
            default_value:""
        },
        _renderMenu: function( ul, items ) {
          var that = this;
          $.each( items, function( index, item ) {
            that._renderItemData( ul, item );
          });
          $( ul ).find( "li:odd" ).addClass( "odd" );
        },
      _create: function() {
        this.wrapper = $( "<span>" )
          .addClass( "custom-combobox" )
          .insertAfter( this.element );
        this.element.hide();
        this._createAutocomplete();
        this._createShowAllButton();
          
      },
      _createAutocomplete: function() {
        var selected = this.element.children( ":selected" ),
          value = selected.val() ? selected.text() : "";
          
 
        this.input = $( "<input>" )
          .appendTo( this.wrapper )
          .val( value )
          .attr( "title", "" )
          .attr( "placeholder", this.options.place_holder )
          .addClass( "form-control custom-combobox-input ui-widget ui-widget-content ui-state-default ui-corner-left" )
          .autocomplete({
            delay: 0,
            minLength: 0,
            source: $.proxy( this, "_source" ),
              create: function (){
                  $(this).data('ui-autocomplete')._renderItem= function( ul, item ) {
                      var text=item.label;
                      if(text.indexOf("|")>-1) {
                        var t=text.split("|");
                        text="<div class='titan_selector_item'><span class='titan_selector_d1'>"+t[0]+"</span>"+t[1]+"</div>";
                      }
                        return $( "<li></li>" )
                            .data( "item.autocomplete", item )
                            .append(text) 
                            .appendTo( ul );
                    };
        

              }
          })
          .tooltip({
            classes: {
              "ui-tooltip": "ui-state-highlight"
            }
          });
       
      this._on( this.input, {
          autocompleteselect: function( event, ui ) {
            ui.item.option.selected = true;
            this._trigger( "select", event, {
              item: ui.item.option
            });

          },
 
          autocompletechange: "_removeIfInvalid"
        });
      },
 
      _createShowAllButton: function() {
        var input = this.input,
          wasOpen = false;
 
        $( "<a>" )
          .attr( "tabIndex", -1 )
          .attr( "title", "Show All Items" )
          .tooltip()
          .appendTo( this.wrapper )
          .button({
            icons: {
              primary: "ui-icon-triangle-1-s"
            },
            text: false
          })
          .removeClass( "ui-corner-all" )
          .addClass( "custom-combobox-toggle ui-corner-right" )
          .on( "mousedown", function() {
            wasOpen = input.autocomplete( "widget" ).is( ":visible" );
          })
          .on( "click", function() {
            input.trigger( "focus" );
 
            // Close if already visible
            if ( wasOpen ) {
              return;
            }
 
            // Pass empty string as value to search for, displaying all results
            input.autocomplete( "search", "" );
          });
      },
 
      _source: function( request, response ) {
        var matcher = new RegExp( $.ui.autocomplete.escapeRegex(request.term), "i" );
        response( this.element.children( "option" ).map(function() {
          var text = $( this ).text();
          if ( this.value &&( !request.term || matcher.test(text) ) )
            {
                return {
                  label: text,
                  value: text,
                  option: this
                    }
            };
        }) );
      },
       
       select: function( event, ui ) {
            ui.item.option.selected = true;
            self._trigger( "selected", event, {
                item: ui.item.option
            });
            this.trigger("change");                             
        },
      _removeIfInvalid: function( event, ui ) {
 
        // Selected an item, nothing to do
        if ( ui.item ) {
          return;
        }
 
        // Search for a match (case-insensitive)
        var value = this.input.val(),
          valueLowerCase = value.toLowerCase(),
          valid = false;
        this.element.children( "option" ).each(function() {
          if ( $( this ).text().toLowerCase() === valueLowerCase ) {
            this.selected = valid = true;
            return false;
          }
        });
 
        // Found a match, nothing to do
        if ( valid ) {
          return;
        }
 
        // Remove invalid value
        this.input
          .val( "" )
          .attr( "title", value + " didn't match any item" )
          .tooltip( "open" );
        this.element.val( "" );
        this._delay(function() {
          this.input.tooltip( "close" ).attr( "title", "" );
        }, 2500 );
        this.input.autocomplete( "instance" ).term = "";
      },
 
      _destroy: function() {
        this.wrapper.remove();
        this.element.show();
      },
         _set_value:function(value){
            this._trigger( "select", 'autocompleteselect', {
              item: function(){
                      var matcher = new RegExp( $.ui.autocomplete.escapeRegex(default_value), "i" );
                        response( this.element.children( "option" ).map(function() {
                          var text = $( this ).text();
                          if ( this.value &&( !request.term || matcher.test(text) ) )
                            {
                                return {
                                  label: text,
                                  value: text,
                                  option: this
                                    }
                            };
                        }) );
              
                      }
                    });//end func
                 }

    });
 
  } );

    /*
    try{
        if(null!==init && undefined!==init) init();
    }catch(x){
    }
*/

