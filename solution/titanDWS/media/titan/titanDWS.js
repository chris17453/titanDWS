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
//Titan core functions... Ajax/ Math.. Reusable items

var titanDWS=(function(my){
    my.key_left         ="_KL_";                                                //template indexer
    my.key_right        ="_KR_";                                                //template indexer
    my.instance         =my.instance||"method";                                 //method prefix
    my.webServiceURL    =my.webServiceURL||$("[id$=titanWebService]").val();    //ajax api utl
    my.m                =my.m||{};                                              //method data object
    my.data_object      =my.data_object||{};                                    //ajax object
    my.JWT              =my.JWT||"";                                            //Javascript web token

    my.load =function(method,success_func){
        $.ajax({type: "POST", url: my.webServiceURL +method, data:JSON.stringify(my.data_object), headers:{'JWT':my.JWT},
                success: function(results){ if(success_func){success_func(results);} }, error: this.error,contentType:"application/json", dataType: "json" });
};
    my.token_success    =function(json_result) {
        if(json_result) { 
            my.JWT=json_result; 
        }
    };
    my.get_token        =function(func){ 
        if(my.webServiceURL==="" || my.webServiceURL===undefined) my.webServiceURL=$("[id$=titanWebService]").val();
        my.load("get_token", function(results){ my.token_success(results); 
            if(func) func(); });   
    };
    my.obj_to_string=function(obj){
        return  JSON.stringify(obj);                                                    //needs everything to be an object... {} not []
    };
    my.set_instance         =function(_instance){ 
        my.instance=_instance; 
    };
    my.set_titan_ajax_object=function(_data_obj){ 
        my.data_object=_data_obj; 
    };


    /*config*/

    $(document).on("click", ".preview_json_out",function(){
        var new_obj=my.load_json_from_html();
        $("#json_pre").html("<pre>"+JSON.stringify(new_obj,"\t",4)+"</pre>");
        $("#json_dialog").dialog({title: "Preview Outgoing JSON",width:1200,height:400});
    });
    $(document).on("click", ".preview_json_in",function(){
        var new_obj=my.load_json_from_html();
        $("#json_pre").html("<pre>"+JSON.stringify(my.m,"\t",4)+"</pre>");
        $("#json_dialog").dialog({title: "Preview Incomming JSON",width:1200,height:400});
    });

    my.load_method_success  =function(jsonResult) {
        my.m = jsonResult.results;
        my.method=my.m;
        my.update_UI();
        $(document).on("click", ".save_method", my.save_method);
        $(document).on("click", ".test_method", my.test_method);
    
    };
    my.save_success         =function(json_result) {
        if(json_result.results) {
            alert(json_result.results);
        }
    };
    my.test_success         =function(json_result) {
        if(json_result.results) {
            alert(json_result.results);
        }
    };
    my.load_method          =function() {  
        if(my.JWT==="") {
            my.get_token(function() { my.load("fetch_method", my.load_method_success);   });
        }
    };
    my.save_method          =function(){   
        my.data_object.crud=JSON.stringify(my.load_json_from_html());
        my.load("save_method", my.save_success);   
    };       
    my.test_method          =function(){   
        my.data_object.crud=JSON.stringify(my.load_json_from_html());
        my.load("api/test_method",my.test_success);   
    };

    
    /* crud */


     //Get the localization of a particular HTML_ID
    my.get_local            =function(id){
        var index;
        for(index in my.m.localization) if(my.m.localization[index].field===id) return my.m.localization[index];
        return null;
    };
    my.set_html_value_for_element=function(type,id,value){
        if(null===value || 
            null===id    ||
            null===type  ||
            "undefined" === typeof type ||
            "undefined" === typeof id ||
            "undefined" === typeof value) {
            return;                                                   //dont set null things or null values
        }
        switch(type){
            case "CHECKBOX": if(value===true) $("#"+id).prop("checked",true); 
                                if(value===false) $("#"+id).prop("checked",false); 
                                break;
            case 'ACE'     : var editor = ace.edit(id);
                                editor.setValue(value);
                                break;
            default       :  if (typeof value === "string" ) value=value.trim();
                                if(value==="''") value="";
                                if(value==='""') value="";
                                $("#"+id).val(value);                 
                            break;
        }//end switch
    };
    my.get_html_value_for_element=function(type,id,fragment){
        if(null===id    ||
            null===type  ||
            "undefined" === typeof type ||
            "undefined" === typeof id ) {
            return;                                                   //dont set null things or null values
        }
        if(null===fragment ||"undefined" === typeof fragment) {
            fragment="0";
        }
               
        var value="";
        switch(type){
            case "CHECKBOX": value=$("#"+id).prop("checked");
                                break;
            case 'ACE'     : var editor = ace.edit(id);
                                value=editor.getValue();
                                break;
            default       :  value=$("#"+id).val();                 
                                if(null!==value && "undefined"!== typeof value) {
                                    if (typeof value === "string") value=value.trim();
                                    if(Number.isInteger(fragment)){
                                    value=parseInt(value);                                           
                                    }//end if is number
                                }//end if null 
                                break;
        }//end switch
        if(Number.isInteger(fragment) && (value===null || "undefined"=== typeof value)){
            value=0;
        }//end if is number
        if("undefined"=== typeof value || value===null) {
            value="";
        }
        return value;
    };
  //This populates Existing HTML Form elements with JSON data    -------------> FILL
    my.load_json_into_html=function(update_element){
        var i,key,key2,key3,local,element,value;

        if(!my.m.parameters || my.m.parameters.length===0) my.m.parameters=[];
        //my.m.parameters.push(jQuery.extend(true, {}, my.m.parameter_template));
            
        for(index in my.m.localization){
            element=my.prep_element_data(my.m.localization[index]);
            id=element.html_id;
            switch(element.data_type){
                case "flat": value=my.m[element.field];                                                //sets the flat/ROOT element values of the JSON object
                                if(null===element.type) continue;                                      //skip nulls
                                my.set_html_value_for_element(element.type,id,value);
                                break;
                case "arr":  for(key in my.m[element.name]){                                           //arr is a loop on a root element NOT FLAT, INDEX MUST BE A NUMBER.
                                if(null===element.type) continue;                                   
                                if(null===my.m[element.name]) continue;
                                if(null===my.m[element.name][key]) continue;//
                                if(null===my.m[element.name][key][element.field]) continue;            //skip nulls
                                id=my.calc_html_id(element,key);            
                                value=my.m[element.name][key][element.field];  
                                my.set_html_value_for_element(element.type,id,value);
                                }
                                break;
                case "obj":  value=my.m[element.name][element.field];                                  //object and element are FLAT, just obj has a parent element.
                                if(null===element.type) continue;                                      //skip nulls
                                my.set_html_value_for_element(element.type,id,value);
                                break;
            }
                
        }//end localization loop
    };//end function
    //This creates a JSON object based of HTML elements that have CHANGED since load.   EXTRACT  <---------------------
    my.load_json_from_html=function(){
        var i,key,key2,key3,local,element,value;
        var m_update=jQuery.extend(true, {}, my.m);
        delete m_update.localization;
        delete m_update.template;
        delete m_update.templates;
        delete m_update.parameter_template;
        delete m_update.column_template;
        delete m_update.result_types ;
        delete m_update.output_types;
        delete m_update.parameter_types;
        delete m_update.external_map;
        delete m_update.methods;
        delete m_update.linked_method_template;
        delete m_update.queries;
        
        for(index in my.m.localization){
            element=my.prep_element_data(my.m.localization[index]);
            id=element.html_id;
            if(element.display==='ORDR') {
                var a="";
            }
            switch(element.data_type){
                case "flat": if("undefined"===typeof my.m[element.field] || null===element.type) continue;                                      //skip nulls
                                m_update[element.field]=my.get_html_value_for_element(element.type,id,my.m[element.field]);
                                break;
                case "arr":  for(key in my.m[element.name]){                                           //arr is a loop on a root element NOT FLAT, INDEX MUST BE A NUMBER.
                                if(null===element.type) continue;                                   
                                if(null===my.m[element.name]) continue;
                                if(null===my.m[element.name][key]) continue;//
                                if("undefined"===typeof my.m[element.name][key][element.field]) continue;            //skip nulls
                                id=my.calc_html_id(element,key);            
                                    
                                m_update[element.name][key][element.field]=my.get_html_value_for_element(element.type,id,my.m[element.name][key][element.field]);
                                }
                                break;
                case "obj":  if("undefined"===typeof my.m[element.name] || null===element.type) continue;                                      //skip nulls
                                m_update[element.name][element.field]=my.get_html_value_for_element(element.type,id,my.m[element.name][element.field]);
                                break;
            }//en switch
        }//end localization loop
        m_update.insert_query           ="";
        m_update.update_query           ="";

        for(key in m_update.parameters) {
            if(m_update.parameters[key].name==="") m_update.parameters[key]=null;           //remove empty parameters
        }
        return m_update;
    };//end function
    my.calc_html_id=function(element,key){
        if(null===key || key===undefined) key='0';
            
        var calc_field=element.field;
        switch(element.data_type){
            case "obj" : calc_field=my.instance+"_"+element.name+"_OBJ_"  +element.field; break;
            case "arr" : calc_field=my.instance+"_"+element.name+my.key_left+key+my.key_right+element.field; break;
            case "flat": 
            default    : calc_field=my.instance+"_"+element.field; break;
        }
        return calc_field;
    };
    my.prep_element_data=function (element) {
        var calc_display=element.display;
        var calc_field  =my.calc_html_id(element);
        var calc_desc   =element.description;
            

        if(null!==element.data_display && element.data_display!=="") {                                  //change the html display based on dynamic stuff.
            //var disp_tokens=element.data_display.split('.');
            if(element.data_source.length>0) {
                //calc_display=my.m[element.data_source][element.data_display];
                calc_desc=my.get_element(element.data_source+"["+element.data_display+"]");
            } else {
                calc_display=my.get_element(element.data_display);
            }
            if(undefined===calc_display  || null===calc_display || calc_display.trim()==='') { 
                calc_display=element.display; 
            }       //overide reert if mapping is a dud.
        }
        if(null!==element.data_desc && element.data_desc  !=="") {                                      //change the html display based on dynamic stuff.
            if(element.data_source.length>0) {
                //calc_desc=my.m[element.data_source][element.data.desc];
                calc_desc=my.get_element(element.data_source+"["+element.data_desc+"]");
            } else {
                calc_desc=my.get_element(element.data_desc);

            }

            if(undefined===calc_desc || null===calc_desc || calc_desc.trim()==='') { calc_desc=element.description; }           //overide reert if mapping is a dud.
        }   
        var new_element=jQuery.extend(true, {}, element);                                               //returned cloned object. Jquery deep clone.
        new_element.html_id     =calc_field;
        new_element.html_display=calc_display;
        new_element.html_desc   =calc_desc;
        return new_element;
    };
    //this creates the HTML for the form
    my.create_form=function(update_element){
        var sections=[],cat="",o="";
        var id="",value="",element=null,group="",last_group="";
        var in_child_row=false;
        var in_multi_loop=false;
        var html_data,update=false;
        if(!my.m) return;
        if(!my.m.localization) return;
        if(null!==update_element && undefined!==update_element) {
            update=true;
        }
            
        for(index in my.m.localization){
            if(update){
                if(my.m.localization[index].name!==update_element) continue;
            }

            element=my.prep_element_data(my.m.localization[index]);                    
                    
            last_group=group;
            group=element.category;
            if(!sections[group]) {
                sections[group]={type:element.container,headers:"",data:'',length:element.colspan,name:element.name};
            }

            switch(element.container){
                case "TABSTART"     : sections[group].data+=my.ui_tabs_start           (element); break;
                case "TABCLOSE"     : sections[group].data+=my.ui_tabs_close           (); break;
                case "TABEND"       : sections[group].data+=my.ui_tabs_end             (); break;
                case "TABCONTSTART" : sections[group].data+=my.ui_tab_container_start  (element); break;
                case "TABCONTEND"   : sections[group].data+=my.ui_tab_container_end    (); break;
                case "COLSTART"     : sections[group].data+=my.ui_column2_start        (element); break;
                case "COLEND"       : sections[group].data+=my.ui_column2_end          (); break;
                case "ROW"          : sections[group].data+=my.ui_row_start();             break;
                case "ROWSTART"     : sections[group].data+=my.ui_row_start();             break;
                case "ROWSTART"     : sections[group].data+=my.ui_row_start();             break;
                case "BREAK"        : sections[group].data+=my.ui_table_child_row      (); in_child_row=true; continue; 
                case "TABLEROW"     : sections[group].data+=my.ui_table_data_start     (element.colspan); break;
            }
            switch(element.wrapper){
                case 'col'          : sections[group].data+=my.ui_column_start     (4); break;
                default: break;
            }

            if(element.container==="TABLEROW") {
                if(last_group!==group) {
                    in_child_row=false;
                }
                if(element.type && element.type!=="" && !in_child_row){
                    sections[group].headers+=my.ui_table_column(element.html_display,element.html_desc);
                }
                in_multi_loop=true;
            } else {
                in_multi_loop=false;
                in_child_row=false;
            }                    
            var css_wrap="sm-1"; if(null!== element.css_wrapper  && element.css_wrapper  !=="") { css_wrap=element.css_wrapper;   }                                 //css override
    
            switch(element.type){
                case "ACE"          : sections[group].data+=my.ui_ace     (element); break;                        //this is special. Ace GUI TExt DIV
                case "ADD"          : sections[group].data+=my.ui_add     (element); break;                        //this is special. It adds new JSON templates to the data object so NEW ROW of *Stuff                 
                case "CHILDROW"     : sections[group].data+=my.ui_table_child_toggle(element.colspan); break;
                case "TAB"          : sections[group].data+=my.ui_tab     (element); break;
                case "BUTTON"       : sections[group].data+=my.ui_button  (element); break;
                case "DATE"         : sections[group].data+=my.ui_date    (element); break;
                case "NUMBER"       : sections[group].data+=my.ui_number  (element); break;
                case "LABEL"        : sections[group].data+=my.ui_label   (element); break;
                case "TEXT"         : sections[group].data+=my.ui_textbox (element); break;
                case "CHECKBOX"     : sections[group].data+=my.ui_checkbox(element); break;
                case "SELECT"       : sections[group].data+=my.ui_select  (element); break;
                case "TEXTAREA"     : sections[group].data+=my.ui_textarea(element); break;
            }
            
            switch(element.wrapper){
                case 'col'          : sections[group].data+=my.ui_column_end     (); break;
                default: break;
            }
            switch(element.container){
                case "ROW"          : sections[group].data+=my.ui_row_end();             break;
                case "ROWEND"       : sections[group].data+=my.ui_row_end();             break;
                case "TABLEROW"     : sections[group].data+=my.ui_table_data_end();             break;
            }//end switch after element

        }//end element loop / localization

        //join the actual HTML
        for(group in sections) {
            switch(sections[group].type){
                case 'TABLEROW':
                                var count=0;
                                if(my.m[sections[group].name]) {
                                    count+=my.m[sections[group].name].length;
                                 }
                                if(update) {
                                    var row_data=my.ui_table_row(sections[group].name,sections[group].data);
                                    row_data=my.ui_table_row_re_id(row_data,count-1);
                                    $('.data_'+sections[group].name).last().after(row_data);  //directly inject th data into the form. it already exists
                                    
                                    return row_data;
                                }
                                table=  my.ui_table(
                                            my.ui_table_header(my.ui_table_row(sections[group].name,sections[group].headers) ),
                                            my.ui_table_body(my.ui_table_rows(my.ui_table_row(sections[group].name,sections[group].data),count) 
                                                    ) );
                                o+=my.ui_section(group,table);
                                break;
                default :     o+=sections[group].data;
            }
        }
        return o;
    };//end function
    //This function binds new HTML elements to the jQuery actions
    my.get_element=function(name) {
        try{
        var element=eval("my.m."+name);
        return element;
        } catch(ex){
            var r=0;
        }
        return "";
    }
    my.bind_jQuery=function(){
        $(".tabs" ).tabs();                                                 //activate jQuery UI Tabs
        $(".number").spinner();                                             //activate jQuery UI Number Picker
        $(".date").datepicker();                                            //activate jQuery UI Date Picker
        $(".number").spinner({ classes: {
                "ui-spinner"     : "ui_spinner-ovd",//"ui-corner-all",
                "ui-spinner-down": "ui-corner-br",
                "ui-spinner-up"  : "ui-corner-tr"
            }});
        $(document).on("click", ".child_toggle" ,function(){                //display child rows action
            var data_id=$(this).closest('tr').data('id');
            var element=$('.child_row[data-id='+data_id+']');
            var display=$('.child_row[data-id='+data_id+']').css("display");
            if(display==="none") display="table-row";
            else display="none";
            $('.child_row[data-id='+data_id+']').css("display",display);
                return false;
            }
        );
        $(document).on("click", ".add_template" ,function(){                //add templated data to UI
            //this.load_json_from_html();
            var source=$(this).data("source");
            var target=$(this).data("target");
            var template=my.get_element(source);
            var dest=my.get_element(target);
            if(!dest) {
                dest=[];
            }
            if(dest){
                dest.push(jQuery.extend(true, {},template ));
                var html_fragment=my.create_form(target);
                $(".number").spinner();                                             //activate jQuery UI Number Picker
                //hack
                $(".date").datepicker();                                            //activate jQuery UI Date Picker
                $(".number").spinner({ classes: {
                        "ui-spinner"     : "ui_spinner-ovd",//"ui-corner-all",
                        "ui-spinner-down": "ui-corner-br",
                        "ui-spinner-up"  : "ui-corner-tr"
                    }});
        
                //alert (html_fragment);
               } else {//end if dest
                 console.log(target+" not found")
                //end if dest
               }
            }
        );

        $( ".ace" ).each(function( index ) {                               //loops through all ace editors and sets the theme and language type...
            var id=$( this ).attr('id');
            var editor = ace.edit(id);
            ace.require("ace/ext/chromevox");
            editor.session.setMode("ace/mode/sql");
            editor.setTheme("ace/theme/tomorrow");
        });
    };
    //this is the main configuration UI creation method.
    my.update_UI=function(){
        var o=my.create_form();
        $("#titan_crud").html(o);                                                       //BLIT auto built form to the page.
        my.bind_jQuery();
        my.load_json_into_html();
    };


    /* UI */


    my.ui_checkbox=function (element){
        var o="",read_only=""; 
        if(element.read_only===true) read_only='readonly';
            
        o+='<input type="checkbox" id="'+element.html_id+'" '+read_only+'>';
        if(element.html_desc!=="")  o+='<label class="control checkbox" for="'+element.html_id+'">';
        else                        o+='<label class="control checkbox" for="'+element.html_id+'" style="margin-top:0px;">';
//        o+='<span class="control-indicator"></span>';
        if(element.data_type!=="arr") o+=element.html_desc;
        //if(element.html_desc!=="") o+='</label>';
        //if(element.html_desc==="") 
            o+='</label>';
        return o;
    };                                             //BEGIN HTML ELEMENTS RULED BY-->element
    my.ui_textbox=function (element){
        var o="",read_only=""; 
        if(element.read_only===true) read_only='readonly';
        if(element.data_type!=="arr") 
            if(element.html_desc!=="") o='<label for="'+element.html_id+'">'+element.html_desc+'</label><br>';
            o+='<input class="form-control '+element.css_element+'" id="'+element.html_id+'" placeholder="'+element.html_display+'" type="text" '+read_only+' />';
        return o;
    };
    my.ui_number=function (element){
        var o="",read_only=""; 
        if(element.read_only===true) {
            read_only='readonly';
            if(element.data_type!=="arr") 
                if(element.html_desc!=="") o='<label for="'+element.html_id+'">'+element.html_desc+'</label><br>';
            o+='<input class="form-control" id="'+element.html_id+'" placeholder="'+element.html_display+'" type="text" '+read_only+' />';
        } else {
            if(element.data_type!=="arr") 
                if(element.html_desc!=="") o='<label for="'+element.html_id+'">'+element.html_desc+'</label><br>';
            o+='<input class="form-control number" id="'+element.html_id+'" placeholder="'+element.html_display+'" type="text" />';
        }
        return o;
    };
    my.ui_date=function (element){
        var o="",read_only=""; 
        if(element.read_only===true) read_only='readonly';
        if(element.data_type!=="arr") 
            if(element.html_desc!=="") o='<label for="'+element.html_id+'">'+element.html_desc+'</label><br>';
        o+='<input class="form-control date" id="'+element.html_id+'" placeholder="'+element.html_display+'" type="text" '+read_only+' />';
        return o;
    };
    my.ui_button=function (element){
        var o="",read_only=""; 
        if(element.read_only===true) read_only='readonly';
            o+='<button class="btn btn-secondary" id="'+element.html_id+'" title="'+element.html_desc+'" type="button" >'+element.html_display+'</button>';
        return o;
    };
    my.ui_textarea=function (element){
        var o="",read_only=""; 
        if(element.read_only===true) read_only='readonly';
        if(element.data_type!=="arr") 
            if(element.html_desc!=="") o='<label for="'+element.html_id+'">'+element.html_desc+'</label>';
        o+='<textarea class="form-control '+element.css_element+'" rows=10 id="'+element.html_id+'" placeholder="'+element.html_display+'" '+read_only+' >'+element.html_display+'</textarea>';
        return o;
    };
    my.ui_select=function (element){
        var o="",read_only=""; 
        if(element.read_only===true) read_only='readonly';
        if(element.data_type!=="arr") 
            if(element.html_desc!=="") o='<label for="'+element.html_id+'">'+element.html_desc+'</label>';
        o+='<select class="form-control" id="'+element.html_id+'" '+read_only+'>';
        if(my.m[element.data_source]){
            var display="",value="";
            for(index in my.m[element.data_source]) {
                value   =my.m[element.data_source][index][element.data_value];
                display =my.m[element.data_source][index][element.data_display];
                o+=my.ui_option(display,value);
            }//end for
        }//end if
        o+='</select>';
        return o;
    };                                               //END HTML ELEMENTS RULED BY-->element
    my.ui_label=function(element){                                                     
        return '<span class="label">'+element.display+'</span>';
    };
    my.ui_option=function(display,value){                                                     //sub item.. no element
        return '<option value="'+value+'">'+display+'</option>';
    };
    my.ui_form_group=function(data){
        return '<div class="form-group">'+data+"</div>";
    };
    my.ui_row=function(data){
        return '<div class="row">'+data+"</div>";
    };
    my.ui_row_start=function(){
        return '<div class="row">';
    }; 
    my.ui_row_end=function(data){
        return "</div>";
    };
    my.ui_ace=function(element){
        return '<div class="ace" id="'+element.html_id+'"></div>';
    };
    my.ui_column=function(data,size){
        return '<div class="col-'+size+'">'+data+"</div>";
    };
    my.ui_column_start=function(size){
        return '<div class="col-xs-'+size+'" style="clear both!important">';
    };
    my.ui_column_end=function(data,size){
        return "</div>";
    };
    my.ui_tabs_start=function(){
        return '<div class="tabs"><ul>';
    };
    my.ui_tabs_close=function(datasize){
        return '</ul>';
    };
    my.ui_tabs_end=function(datasize){
        return '</div>';
    };
    my.ui_tab=function(element){
        return '<li><a href="#tab_'+element.html_id+'">'+element.html_display+"</a></li>";
    };                                                   //RULED BY MERCILESS ELEMENT
    my.ui_tab_container_start=function(element){
        return '<div class="tab-container" id="tab_'+element.html_id+'">';
    };                                   //RULED BY MERCILESS ELEMENT
    my.ui_tab_container_close=function(){
        return '</div>';
    };
    my.ui_tab_container_end=function(){
        return '<div style="clear:both;"></div></div>';
    };
    my.ui_column2_start=function(element){
        return '<div class="col-'+element.colspan+'">';
    };                                   //RULED BY MERCILESS ELEMENT
    my.ui_column2_end=function(element){
        return '</div>';
    };                                   //RULED BY MERCILESS ELEMENT
    my.ui_section=function(section,data){
        return '<div class="row"><h3>'+section+'</h3>'+data+"</div>";
    };
    my.ui_add=function(element){
        var o="",read_only=""; 
        if(element.read_only===true) read_only='readonly';
            o+='<button class="btn btn-secondary add_template" data-source="'+element.data_source+'" data-target="'+element.target+'" id="'+element.html_id+'" title="'+element.html_desc+'" type="button" >'+element.html_display+'</button>';
        return o;
    };
    my.ui_table=function(head,body){
        return '<table class="table table-condensed table-sm">'+head+body+'</table>';
    };
    my.ui_table_header=function(data){
        return '<thead>'+data+'</thead>';
    };
    my.ui_table_body=function(data){
        return '<tbody>'+data+'</tbody>';
    };
    my.ui_table_column=function(data,hover){
        return '<th title="'+hover+'">'+data+'</th>';
    };
    my.ui_table_row=function(name,data){
        return '<tr data-id="row_'+my.key_left+"0"+my.key_right+'" class="data_'+name+'">'+data+'</tr>';
    };
    my.ui_table_child_row=function(data){
        return '</tr><tr data-id="row_'+my.key_left+"0"+my.key_right+'" class="child_row">';
    };
    my.ui_table_data=function(colspan,data){
        return '<td colspan="'+colspan+'">'+data+'</td>';
    };
    my.ui_table_data_start=function(colspan,data){
        return '<td colspan="'+colspan+'">';
    };
    my.ui_table_data_end=function(colspan,data){
        return '</td>';
    };
    my.ui_table_child_toggle=function(colspan){
        return '<div class="child_toggle">X</div>';
    };
    my.ui_table_row_re_id=function (data,id){
        var o="";
        o=data.split(my.key_left+"0"+my.key_right).join(my.key_left+id+my.key_right);
        return o;
    }
    my.ui_table_rows=function(data,count){      
        var o="";
        for(i=0; i<count; i++ ){
            o+=data.split(my.key_left+"0"+my.key_right).join(my.key_left+i+my.key_right);
        }
        return o;
    }

   my.create_signature=function(group,methodName,owner,
                     check,ref1,ref2 ,ref3 ,ref4 ,ref5 ,ref6 ,ref7,
                     ref8 ,ref9,ref10,ref11,ref12,ref13,ref14,ref15,configure) {                                //this is the ajax post object
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
        this.data_object.group         = group;                       
        this.data_object.method        = methodName;                  
        this.data_object.owner         = owner;                       
        this.data_object.parameterless=false;
        this.data_object.configure    =configure;
    }//end func
    

    return my;
}(titanDWS || {}));//auto execute the function, closure.. etc..





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
        this.data_object.group         = group;                       
        this.data_object.method        = methodName;                  
        this.data_object.owner         = owner;                       
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
        for (var index in this.m.data_schema) {
            if (!this.m.data_schema[index].visible) continue;
            thead += "<th ";
            if (!this.m.data_schema[index].filter) thead += "data-filter=\"false\" ";
            if (!this.m.data_schema[index].sort  ) thead += "data-sorter=\"false\" ";
            if ( this.m.data_schema[index].filter && this.m.data_schema[index].selectOptions.length>0) {      //if filter and has select options
                thead +=" class=\"filter-select\" " ;
            }
            if(this.m.data_schema[index].selectOptions.length>0){
                for(var oindex in this.m.data_schema[index].selectOptions){
                    if(this.m.data_schema[index].selectOptions[oindex]._default){
                        thead+="data-value=\""+this.m.data_schema[index].selectOptions[oindex].value+"\"";
                        filterDefaults[index]= this.m.data_schema[index].selectOptions[oindex].value;
                        }
                }
            }
            name = this.m.data_schema[index].name;
            if (this.m.data_schema[index].alias !== "") name = this.m.data_schema[index].alias;
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
        for (var index in this.m.data_schema) {
            if (!this.m.data_schema[index].visible) continue;
            if (this.m.data_schema[index].width === "") continue;
            tableWidth +=this.resultsDiv+" .tablesorter td:nth-of-type(" + (parseInt(i)+1) + ") {";
            if(this.m.data_schema[index].overflow) tableWidth +="overflow:visible !important; ";
            if(this.m.data_schema[index].fixed_width!==true) tableWidth += "max-width: " + this.m.data_schema[index].width + "px;"
            if(this.m.data_schema[index].width!=="0") {
                tableWidth += "width: " + this.m.data_schema[index].width + "px;";
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
            if(null!==this.m.template && null!==this.m.template.report && typeof this.m.template.report!== "undefined"){
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

        if(null!==this.m.template && typeof this.m.template !== "undefined"){
            if(null!==this.m.template.report && typeof this.m.template.report !== "undefined"){
                this.m.template.report =styles + this.m.template.report.replace(injectTargetThead, thead);
                if(!this.m.has_footer) tfoot="";
                this.m.template.report = this.m.template.report.replace(injectTargetTfoot, tfoot);

            }
            if(null!==this.m.template.parameters && typeof this.m.template.parameters !=="undefined" ){
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
                ajaxUrl             : this.webServiceURL + "lambda",
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
});
 


