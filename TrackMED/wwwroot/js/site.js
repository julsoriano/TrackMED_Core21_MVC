// Section 1a: CRUD ON VARIOUS TABLES USING jQuery MODAL FORMS
// ADD RECORD
    /*
    $("#openDialog").on("click", function (e) {
        e.preventDefault();
        var url = $(this).attr('href');
        alert(url);

        $("#dialog-edit").dialog({
            title: 'Add Record',
            autoOpen: false,
            resizable: false,
            height: 500,
            width: 500,
            show: { effect: 'drop', direction: "up" },
            modal: true,
            draggable: true,
            open: function (event, ui) {
                event.preventDefault();
                $(this).load(url);
            },
            close: function (event, ui) {
                $(this).dialog('close');
            }
        });

        $("#dialog-edit").dialog('open');
        return false;
    });
    */

// EDIT RECORD
function editRecord(editthis, modelName) {
    // https://www.mindstick.com/Articles/1117/crud-operation-using-modal-dialog-in-asp-dot-net-mvc
    var entJson = $(editthis).attr('rel');      // in Json format            

    // The JSON.parse() method parses a JSON string, constructing the JavaScript value or object described by the string.
    // https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/JSON/parse
    var obj = JSON.parse(entJson);      // https://www.w3schools.com/js/js_json.asp and https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/JSON/parse

    document.getElementById("tableName").innerHTML = modelName;

    // Fill up modal form with original data
    $(".modal-body #id").val(obj.id);
    $(".modal-body #desc").val(obj.desc);

    $("#tagDiv").hide();
    if (modelName === "DESCRIPTION") {
        $(".modal-body #tag").val(obj.tag);
        $("#tagDiv").show();
    }

    $(".modal-body #createdAtUtc").val(obj.createdAtUtc);

    // open edit dialog
    $("#dialog-edit").dialog({
        title: 'Edit Record',
        autoOpen: false,
        resizable: false,
        height: "auto",
        width: 400,
        show: { effect: 'drop', direction: "up" },
        modal: true,
        draggable: true,

        // if not using ASP.NET
        buttons: {
            Save: function () {
                event.preventDefault();

                // get new values from form based on method described in http://www.c-sharpcorner.com/blogs/using-ajax-in-asp-net-mvc
                var entity = {};
                entity.id = $(".modal-body #id").val();
                entity.desc = $(".modal-body #desc").val();

                if (modelName === "DESCRIPTION") {
                    entity.tag = $(".modal-body #tag").val();
                }

                entity.createdAtUtc = $(".modal-body #createdAtUtc").val();

                editConfirmed(editthis, modelName, entity);
                $(this).dialog('close');
            },
            Cancel: function () {
                $(this).dialog('close');
            }
        }
    });

    $("#dialog-edit").dialog('open');
    return false;
}

function editConfirmed(editthis, modelName, entity) {
    var url = "/" + modelName + "/Edit";

    //var entity = JSON.stringify($('form').serializeObject());

    // Send the data using post. See https://api.jquery.com/jquery.post/
    // var posting = $.post(url, { entity: entity });
    // above is equivalent to:
    var posting = 
        $.ajax({
          type: "POST",
          url: url,
          // https://stackoverflow.com/questions/15317856/asp-net-mvc-posting-json
          data: { "id" : entity.id, "desc": entity.desc, "tag": entity.tag, "createdAtUtc": entity.createdAtUtc},
          //contentType: "application/json; charset=utf-8",
          accept: "application/json"
        });
    
    // remove record from list
    posting.done(function (data, textStatus, xhr) {

        // See http://www.w3schools.com/json/json_eval.asp to parse a JSON string
        var obj = JSON.parse(xhr.responseText);
        if (obj.success) {
            alert("done");
        }
        else {
            alert("Failure is not an option");
        }
    });

    // alert if not successful
    posting.fail(function (xhr, textStatus, errorThrown) {
        if (xhr.status !== null) {
            switch (xhr.status) {
                case "404":
                    alert(xhr.status + ": Page not found ");
                    break;

                default:
                    alert(xhr.status + ": " + xhr.statusText);
                    break;
            }
        }

        alert("error here");
        // possible values for the second argument (besides null) are "timeout", "error", "abort", and "parsererror". 
        if (textStatus !== null) alert("Error Status: " + textStatus);

        // when an HTTP error occurs, errorThrown receives the textual portion of the HTTP status, such as "Not Found" or "Internal Server Error." 
        alert("HTTP error thrown: " + errorThrown);
    });
}

$.fn.serializeObject = function () {
    var o = {};
    var a = this.serializeArray();
    $.each(a, function () {
        if (o[this.name]) {
            if (!o[this.name].push) {
                o[this.name] = [o[this.name]];
            }
            o[this.name].push(this.value || '');
        } else {
            o[this.name] = this.value || '';
        }
    });
    return o;
};

// DELETE RECORD
function deleteRecord(removethis, modelName) {
    var bgColor = $(removethis).closest('tr').css("background-color");
    var color = $(removethis).closest('tr').css("color");

    $(removethis).closest('tr').css({ "background-color": "yellow", "color": "blue" });
    //$(removethis).closest('tr').addClass("selected").addClass("highlight");
    var dialog = $("#dialog-confirm").dialog({              // https://api.jqueryui.com/dialog/ 
        autoOpen: false,                                    // https://api.jqueryui.com/dialog/#option-autoOpen
        resizable: false,
        height: "auto",
        width: 350,
        show: {                                             // https://api.jqueryui.com/show/
            effect: "blind",                                // https://api.jqueryui.com/category/effects/
            duration: 1000
        },
        hide: {                                             // https://jqueryui.com/hide/
            effect: "clip",
            duration: 1000
        },
        modal: true,                                        // https://jqueryui.com/dialog/#modal-confirmation
        draggable: true,

        buttons: {
            OK: function () {
                event.preventDefault();
                deleteConfirmed(removethis, modelName, bgColor, color);
                dialog.dialog("close");
            },
            Cancel: function () {
                $(removethis).closest('tr').css({ "background-color": bgColor, "color": color });
                //$(removethis).closest('tr').removeClass("selected").removeClass("highlight");
                dialog.dialog("close");
            }
        }
    });

    dialog.dialog('open');
    return false;
}

function deleteConfirmed(removethis, modelName, bgColor, color) {
    var value = $(removethis).attr('rel');     // or: var value = (removethis.id).substr(1);
    var url = "/" + modelName + "/Remove";
    //alert(value);
    //alert(url);
    // Send the data using post. See https://api.jquery.com/jquery.post/
    var posting = $.post(url, { id: value });
    /*  above is equivalent to:
        $.ajax({
          type: "POST",
          url: url,
          data: { id: value },
          success: success,
          dataType: dataType
        });
    */

    // remove record from list
    posting.done(function (data, textStatus, xhr) {
        // See http://api.jquery.com/jQuery.ajax/#jqXHR for various properties and methods of the jqXHR object
        /*
        alert(JSON.stringify(data));    // {"success":true, "status":"Completed Successfully"}      {"success":false, "status:"Can't delete component because CedarITT is using it"}
        alert(textStatus);              // success                                                  success
        alert(xhr.status);              // 200                                                      200
        alert(xhr.statusText);          // OK                                                       OK
        alert(xhr.responseText);        // {"success":true, "status":"Completed Successfully"}      {"success":false, "status:"Can't delete component because CedarITT is using it"}   
        */

        // See http://www.w3schools.com/json/json_eval.asp to parse a JSON string
        var obj = JSON.parse(xhr.responseText);
        if (obj.success) {
            $(removethis).closest('tr').remove();
        }
        else {
            alert(obj.status);
            $(removethis).closest('tr').css({ "background-color": bgColor, "color": color });
            //$(removethis).closest('tr').removeClass("selected").removeClass("highlight");
        }

    });

    // alert if not successful
    posting.fail(function (xhr, textStatus, errorThrown) {
        if(xhr.status !== null) {
            switch (xhr.status) {
                case "404":
                    alert(xhr.status + ": Page not found ");
                    break;

                default:
                    alert(xhr.status + ": " + xhr.statusText);
                    break;
            }
        }

        alert("error here");
        // possible values for the second argument (besides null) are "timeout", "error", "abort", and "parsererror". 
        if(textStatus !== null) alert("Error Status: " + textStatus);

        // when an HTTP error occurs, errorThrown receives the textual portion of the HTTP status, such as "Not Found" or "Internal Server Error." 
        alert("HTTP error thrown: " + errorThrown);
    });
}

// Section 2a: CRUD ON VARIOUS TABLES USING Bootsrap MODAL FORMS
// ADD RECORD
/*
$("#openDialog").on("click", function (e) {
    e.preventDefault();
    var url = $(this).attr('href');
    alert(url);

    $("#dialog-edit").dialog({
        title: 'Add Record',
        autoOpen: false,
        resizable: false,
        height: 500,
        width: 500,
        show: { effect: 'drop', direction: "up" },
        modal: true,
        draggable: true,
        open: function (event, ui) {
            event.preventDefault();
            $(this).load(url);
        },
        close: function (event, ui) {
            $(this).dialog('close');
        }
    });

    $("#dialog-edit").dialog('open');
    return false;
});
*/

// EDIT RECORD
function editRecordBootstrap(editthis, tableName) {
    // https://www.mindstick.com/Articles/1117/crud-operation-using-modal-dialog-in-asp-dot-net-mvc
    var val = $(editthis).attr('rel');                  // or: var value = (editthis.id).substr(1);
    // var val = $(editthis).attr('asp-route-id');    // Error: HTTP Error 404.11 - Not Found | The request filtering module is configured to deny a request that contains a double escape sequence
    alert(val);

    var obj = JSON.parse(val);                          // https://www.w3schools.com/js/js_json.asp and https://developer.mozilla.org/en-US/docs/Web/JavaScript/Reference/Global_Objects/JSON/parse
    var myDesc = obj.desc;
    alert(myDesc);
    var myCreatedAtUtc = obj.createdAtUtc;
    alert(myCreatedAtUtc);
    // https://stackoverflow.com/questions/28701164/how-to-use-html-rawjson-encodemodel-properly

    $(".modal-body #desc").val(obj.desc);
    $(".modal-body #tag").val(obj.tag);
    $(".modal-body #createdAtUtc").val(obj.createdAtUtc);

    $('.editDialog').modal('show', function () {
        // $('#myInput').trigger('focus')
        e.preventDefault(); // added jss 09Aug'16
        $("#dialog-edit").dialog('open');
        return false;
    });
}

/*
// DELETE RECORD

function deleteRecord(removethis, tableName) {
    var bgColor = $(removethis).closest('tr').css("background-color");
    var color = $(removethis).closest('tr').css("color");

    $(removethis).closest('tr').css({ "background-color": "yellow", "color": "blue" });
    //$(removethis).closest('tr').addClass("selected").addClass("highlight");
    var dialog = $("#dialog-confirm").dialog({              // https://api.jqueryui.com/dialog/ 
        autoOpen: false,                                    // https://api.jqueryui.com/dialog/#option-autoOpen
        resizable: false,
        height: "auto",
        width: 350,
        show: {                                             // https://api.jqueryui.com/show/
            effect: "blind",                                // https://api.jqueryui.com/category/effects/
            duration: 1000
        },
        hide: {                                             // https://jqueryui.com/hide/
            effect: "clip",
            duration: 1000
        },
        modal: true,                                        // https://jqueryui.com/dialog/#modal-confirmation
        draggable: true,

        buttons: {
            OK: function () {
                event.preventDefault();
                deleteConfirmed(removethis, tableName, bgColor, color);
                dialog.dialog("close");
            },
            Cancel: function () {
                $(removethis).closest('tr').css({ "background-color": bgColor, "color": color });
                //$(removethis).closest('tr').removeClass("selected").removeClass("highlight");
                dialog.dialog("close");
            }
        }
    });

    dialog.dialog('open');
}

function deleteConfirmed(removethis, tableName, bgColor, color) {
    var value = $(removethis).attr('rel');     // or: var value = (removethis.id).substr(1);
    var url = "/" + tableName + "/Remove";

    // Send the data using post. See https://api.jquery.com/jquery.post/
    var posting = $.post(url, { id: value });

    // remove record from list
    posting.done(function (data, textStatus, xhr) {
        // See http://www.w3schools.com/json/json_eval.asp to parse a JSON string
        var obj = JSON.parse(xhr.responseText);
        if (obj.success) {
            $(removethis).closest('tr').remove();
        }
        else {
            alert(obj.status);
            $(removethis).closest('tr').css({ "background-color": bgColor, "color": color });
            //$(removethis).closest('tr').removeClass("selected").removeClass("highlight");
        }

    });

    // alert if not successful
    posting.fail(function (xhr, textStatus, errorThrown) {
        if (xhr.status !== null) {
            switch (xhr.status) {
                case "404":
                    alert(xhr.status + ": Page not found ");
                    break;

                default:
                    alert(xhr.status + ": " + xhr.statusText);
                    break;
            }
        }

        // possible values for the second argument (besides null) are "timeout", "error", "abort", and "parsererror". 
        if (textStatus !== null) alert("Error Status: " + textStatus);

        // when an HTTP error occurs, errorThrown receives the textual portion of the HTTP status, such as "Not Found" or "Internal Server Error." 
        alert("HTTP error thrown: " + errorThrown);
    });
}
*/

// Section 2a: DATATABLE OPERATIONS

// PageMe: Superceded by Datatable
/*
jQuery.fn.pageMe = function (opts) {    // opts are definedwhen pageMe is invoked during loading of DOM
    var $this = this,                   // "this" is #myTable
        defaults = {
            perPage: 7,
            showPrevNext: false,
            hidePageNumbers: false
        },
        settings = $.extend(defaults, opts); // merge opts into defaults and overrides similar fields in defaults https://api.jquery.com/jquery.extend/

    var listElement = $this;
    //alert(JSON.stringify(settings));
    var perPage = settings.perPage;
    var children = listElement.children();
    //alert(JSON.stringify(children));
    var pager = $('.pager');

    if (typeof settings.childSelector != "undefined") {
        children = listElement.find(settings.childSelector);
    }

    if (typeof settings.pagerSelector != "undefined") {
        pager = $(settings.pagerSelector);
    }

    var numItems = children.size();
    var numPages = Math.ceil(numItems / perPage);

    pager.data("curr", 0);

    if (settings.showPrevNext) {
        $('<li><a href="#" class="prev_link">«</a></li>').appendTo(pager);
    }

    var curr = 0;
    while (numPages > curr && (settings.hidePageNumbers == false)) {
        $('<li><a href="#" class="page_link">' + (curr + 1) + '</a></li>').appendTo(pager);
        curr++;
    }

    if (settings.showPrevNext) {
        $('<li><a href="#" class="next_link">»</a></li>').appendTo(pager);
    }

    pager.find('.page_link:first').addClass('active');
    pager.find('.prev_link').hide();
    if (numPages <= 1) {
        pager.find('.next_link').hide();
    }
    pager.children().eq(1).addClass("active");

    children.hide();
    children.slice(0, perPage).show();

    pager.find('li .page_link').click(function () {
        var clickedPage = $(this).html().valueOf() - 1;
        goTo(clickedPage, perPage);
        return false;
    });
    pager.find('li .prev_link').click(function () {
        previous();
        return false;
    });
    pager.find('li .next_link').click(function () {
        next();
        return false;
    });

    function previous() {
        var goToPage = parseInt(pager.data("curr")) - 1;
        goTo(goToPage);
    }

    function next() {
        goToPage = parseInt(pager.data("curr")) + 1;
        goTo(goToPage);
    }

    function goTo(page) {
        var startAt = page * perPage,
            endOn = startAt + perPage;

        children.css('display', 'none').slice(startAt, endOn).show();

        if (page >= 1) {
            pager.find('.prev_link').show();
        }
        else {
            pager.find('.prev_link').hide();
        }

        if (page < (numPages - 1)) {
            pager.find('.next_link').show();
        }
        else {
            pager.find('.next_link').hide();
        }

        pager.data("curr", page);
        pager.children().removeClass("active");
        pager.children().eq(page + 1).addClass("active");

    }
};
*/

/*function showRelatedTable(value, tableName) {
    var url = "/" + tableName;
    switch (tableName){
        case "SystemsDescription":
            url += "/LoadSystems";
            break;

        case "SystemTab":
            url += "/LoadDeployments";
            break;

        case "Component":
            url += "/LoadActivities";
            break;

        case "Location":
            url += "/LoadSystems";
            break;

        default:
            url += "/LoadComponents";
            break;
   }
*/
function showRelatedTable(value, tableName) {
    var url = "/" + tableName;
    // https://toddmotto.com/deprecating-the-switch-statement-for-object-literals/
    var loadtable = {
        'Component': function () {
            return "/LoadActivities";
        },
        'SystemTab': function () {
            return "/LoadDeployments";
        },
        'SystemsDescription': function () {
            return "/LoadSystems";
        },
        'Location': function () {
            return "/LoadSystems";
        },
        'default': function () {
            return "/LoadComponents";
        }
    };
    url += (loadtable[tableName] || loadtable['default'])();

    var nestedTable = null;
    
    // TODO: Replace with Fetch. See https://fetch.spec.whatwg.org/#fetch-api
    // Not Working: Probably because of the asynchronous nature of 'Fetch'
    /*
    var itemUrl = new URL("https://localhost:50120" + url);
    var params = { tableName: tableName, descId: value }; //alert("params " + JSON.stringify(params));    
    Object.keys(params).forEach(key => itemUrl.searchParams.append(key, params[key]));     //alert(itemUrl);
   
    fetch(itemUrl)
        .then((res) => res.json())
        .then(data => {
            nestedTable = showRelatedComponents(data);
            if (nestedTable !== null) {
                return nestedTable;
            }

            return '<p> No Records to Display </p>';
        });
    */

    // get related data
    // See http://api.jquery.com/jQuery.ajax/
    
    $.ajax({
        method: "GET",
        url: url,
        data: { tableName: tableName, descId: value },
        async: false,
        cache: false
    })  
    .done(function (data) {
        //alert('length of data = ' + data.length);
        //console.log(data);
        //console.log(JSON.stringify(data));
        switch (tableName) {
            case "Component":
                if (data.length > 0) nestedTable = showComponentActivities(data);
                break;

            case "SystemTab":
                if (data.length > 0) nestedTable = showDeployments(data);
                break;

            case "SystemsDescription":
                if (data.length > 0) nestedTable = showSystems(data);
                break;

            case "Location":
                if (data.length > 0) nestedTable = showSystems(data);
                break;

            default:
                if (data.length > 0) nestedTable = showRelatedComponents(data);
                break;
        }
    });

    if (nestedTable !== null) {
        return nestedTable;
    }

    return '<p> No Records to Display </p>';
    
}

function showComponentActivities(data) {
    nestedTable =
        '<tr>' +
            '<td colspan="4">' +
                '<table class="table table-striped table-condensed table-hover table-component" cellpadding="5" cellspacing="0" border="0" style="padding-left:50px;">' +
                    '<thead>' +
                            '<th>' + 'Deployment ID' + '</th>' +
                            '<th>' + 'Test System' + '</th>' +
                            '<th>' + 'Work Order' + '</th>' +
                            '<th>' + 'Schedule' + '</th>' +
                            '<th>' + 'eRecord' + '</th>' +
                            '<th>' + 'Deployment Date' + '</th>' +
                            '<th>' + 'WO Scheduled Due' + '</th>' +
                            '<th>' + 'WO Done Date' + '</th>' +
                            '<th>' + 'WO Calculated Due' + '</th>' +
                            '<th>' + 'Activity Type' + '</th>' +
                            '<th>' + 'Service Provider' + '</th>' +
                            '<th>' + 'Status' + '</th>' +
                        '</tr>' +
                    '</thead>' +
                    '<tbody>';
    $.each(data, function (index, itemData) {
        nestedTable +=
            '<tr>' +
                '<td>' + itemData.deploymentID + '</td>' +
                '<td>' + itemData.systemID + '</td>' +
                '<td>' + itemData.work_Order + '</td>' +
                '<td>' + itemData.schedule + '</td>' +
                '<td>' + itemData.eRecord + '</td>';

        if (itemData.deploymentDate !== null) nestedTable += '<td>' + formattedDate(itemData.deploymentDate) + '</td>';
        else nestedTable += '<td></td>';

        if(itemData.wO_Scheduled_Due !== null) nestedTable += '<td>' + formattedDate(itemData.wO_Scheduled_Due) + '</td>';
        else nestedTable += '<td></td>';

        if(itemData.wO_Done_Date !== null) nestedTable += '<td>' + formattedDate(itemData.wO_Done_Date) + '</td>';
        else nestedTable += '<td></td>';

        if(itemData.wO_Calculated_Due_Date !== null) nestedTable += '<td>' + formattedDate(itemData.wO_Calculated_Due_Date) + '</td>';
        else nestedTable += '<td></td>';

        if (itemData.activityTypeID !== null) nestedTable += '<td>' + itemData.activityType.desc + '</td>';
        else nestedTable += '<td></td>';

        if (itemData.serviceProviderID !== null) nestedTable += '<td>' + itemData.serviceProvider.desc + '</td>';
        else nestedTable += '<td></td>';

        if (itemData.statusID !== null) nestedTable += '<td>' + itemData.status.desc + '</td>';
        else nestedTable += '<td></td>';
        
        nestedTable += '</tr>';
    });

    nestedTable += '</tbody>' + '</table>' + '</td>' + '</tr>';

    return nestedTable;
}

function showSystems(data) {
    nestedTable =
        '<tr>' +
            '<td colspan="4">' +
                '<table class="table table-striped table-condensed table-hover table-component" cellpadding="5" cellspacing="0" border="0" style="padding-left:50px;">' +
                    '<thead>' +
                        '<tr>' +
                            '<th>' + 'IMTE' + '</th>' +
                            '<th>' + 'Reference No.' + '</th>' +
                            '<th>' + 'System Description' + '</th>' +
                            '<th>' + 'Deployment Date' + '</th>' +
                            '<th>' + 'Location' + '</th>' + '</tr>' +
                    '</thead>' +
                    '<tbody>';

    $.each(data, function (index, itemData) {
        // add detail row
        nestedTable +=
            '<tr>' +
                '<td>' + itemData.imte + '</td>' +
                '<td>' + itemData.referenceNo + '</td>';

        if (itemData.systemsDescriptionID !== null) nestedTable += '<td>' + itemData.systemsDescription.desc + '</td>';
        else nestedTable += '<td></td>';

        if (itemData.deploymentDate !== null) nestedTable += '<td>' + formattedDate(itemData.deploymentDate) + '</td>';
        else nestedTable += '<td></td>';

        if (itemData.locationID !== null) nestedTable += '<td>' + itemData.location.desc + '</td>';
        else nestedTable += '<td></td>';

        nestedTable += '</tr>';
    });

    nestedTable += '</tbody>' + '</table>' + '</td>' + '</tr>';

    return nestedTable;
}

function showRelatedComponents(data) {
    //alert(JSON.stringify(data));
    nestedTable =
        '<tr>' +
            '<td colspan="4">' +
                '<table class="table table-striped table-condensed table-hover table-light" cellpadding="5" cellspacing="0" border="0" style="padding-left:50px;">' +
                    '<thead>' +
                        '<tr>' +
                            '<th>' + 'Index' + '</th>' +
                            '<th>' + 'Asset#' + '</th>' +
                            '<th>' + 'IMTE' + '</th>' +
                            '<th>' + 'Serial Number' + '</th>' +
                            '<th>' + 'Description' + '</th>' +
                            '<th>' + 'Owner' + '</th>' +
                            '<th>' + 'Status' + '</th>' +
                            '<th>' + 'Model/Manufacturer' + '</th>' +
                            '<th>' + 'Service Provider' + '</th>' +
                            '<th>' + 'Calibration Due Date' + '</th>' +
                            '<th>' + 'Maintenance Due Date' + '</th>' +
                        '</tr>' +
                    '</thead>' +
                    '<tbody>';

    $.each(data, function (index, itemData) {
        // add detail row 

        nestedTable +=
            '<tr>' +
            '<td>' + (index +1) + '</td>' +
            '<td>' + itemData.assetnumber + '</td>' +
            '<td>' + itemData.imte + '</td>' +
            '<td>' + itemData.serialnumber + '</td>';

        if (itemData.description !== null) nestedTable += '<td>' + itemData.description.desc + '</td>';
        else nestedTable += '<td></td>';

        if (itemData.owner !== null) nestedTable += '<td>' + itemData.owner.desc + '</td>';
        else nestedTable += '<td></td>';

        if (itemData.status !== null) nestedTable += '<td>' + itemData.status.desc + '</td>';
        else nestedTable += '<td></td>';

        if (itemData.model_Manufacturer !== null) nestedTable += '<td>' + itemData.model_Manufacturer.desc + '</td>';
        else nestedTable += '<td></td>';

        if (itemData.providerOfService !== null) nestedTable += '<td>' + itemData.providerOfServiceID.desc + '</td>';
        else nestedTable += '<td></td>';

        if (itemData.calibrationDate !== null) nestedTable += '<td>' + formattedDate(itemData.calibrationDate) + '</td>';
        else nestedTable += '<td></td>';

        if (itemData.maintenanceDate !== null) nestedTable += '<td>' + formattedDate(itemData.maintenanceDate) + '</td>';
        else nestedTable += '<td></td>';
    });

    nestedTable += '</tbody>' + '</table>' + '</td>' + '</tr>';
    return nestedTable;
}

function showDeployments(data) {
    nestedTable =
        '<tr>' +
            '<td colspan="4">' +
                '<table class="table table-striped table-condensed table-hover table-component" cellpadding="5" cellspacing="0" border="0" style="padding-left:50px;">' +
                    '<thead>' +
                            '<th>' + 'Deployment ID' + '</th>' +
                            '<th>' + 'Reference No.' + '</th>' +
                            '<th>' + 'Deployment Date' + '</th>' +
                            '<th>' + 'Location' + '</th>' +
                        '</tr>' +
                    '</thead>' +
                    '<tbody>';
    $.each(data, function (index, itemData) {
        nestedTable +=
            '<tr>' +
                '<td>' + itemData.deploymentID + '</td>' +
                '<td>' + itemData.referenceNo + '</td>';

        if (itemData.deploymentDate !== null) nestedTable += '<td>' + formattedDate(itemData.deploymentDate) + '</td>';
        else nestedTable += '<td></td>';

        if (itemData.locationID !== null) nestedTable += '<td>' + itemData.location.desc + '</td>';
        else nestedTable += '<td></td>';

        nestedTable += '</tr>';
    });

    nestedTable += '</tbody>' + '</table>' + '</td>' + '</tr>';

    return nestedTable;
}

// SECTION 2b: SOLELY FOR SYSTEMTABS
// Compose Select List of Components
function showComponentDDL(myform) {
    /*  Or, if inside $(function) {
        $('#ComponentDescID').change(function () {
    */
    $("#divComponent").parent().addClass("hidden");

    var selectedDesc = $("#ComponentDescID option:selected").val();
    if (selectedDesc === null) return;

    // get components
    $.getJSON("/Systems/LoadComponents", { descId: selectedDesc },

        // then: compose
        function (data) {
            var select = $("#ComponentID");
            select.empty();
            select.append($('<option/>',
            {
                value: 0,
                text: "Select a Component"
            }));
            $.each(data, function (index, itemData) {
                select.append($('<option/>',
                {
                    value: itemData.value,
                    text: itemData.text
                }));
            });

            $("#divComponent").parent().slideDown().removeClass("hidden");

            // disable all components that have been previously selected
            $('#imteX tr').each(function () {
                $(this).children('td').each(function (i) {
                    if (i === 0) {
                        var imtenum = $(this).text().trim();  // always the first td

                        // If already selected: disable and strike it through
                        $('#ComponentID option:contains(' + imtenum + ')').prop('disabled', true).addClass("strikethrough");  // strikethrough: Works in IE11, FireFox; not in Chrome, Safari
                    }
                });
            });
        });

    // show
    //$("#divComponent").parent().slideDown().removeClass("hidden");
}

// Compose Table of Selected Components
function addIMTE(myform) {
    /*  Or, If inside $(function) {:
        $('#ComponentID').change(function () { 
    */
    // compose heading if the first row
    if (document.getElementById("imteX").rows.length === 0) {        //if ($('#imteX').rows.length == 0) {  // does not work
        // $('#imteX').append('<thead><tr><th scope="col" + >IMTE</th><th scope="col" + >Description</th><th scope="col" + >Calibration Date</th><th scope="col" + >Orientation</th></tr></thead>');
        $('#imteX').append('<thead><tr><th scope="col">IMTE</th>' +
                                      '<th scope="col">Description</th>' +
                                      '<th scope="col">Calibration Date</th>' +
                                      '<th scope="col">Maintenance Date</th>' +
                                      '<th scope="col" + >Orientation</th></tr></thead>');
    }

    // show details
    $("#ComponentID option:selected")
        .each(function () {
            // split DDL text into two parts: IMTE and Calibration Date/Time  alert($(this)[0].text);

            /* Note: Special character "/" need not be escaped inside [].
               See https://developer.mozilla.org/en-US/docs/Web/JavaScript/Guide/Regular_Expressions Section: "Special characters in regular expressions." [xyz] */
            //var partsArray = /\s*(\w+)\s*\(?([A-Za-z0-9/-]*)\)?$/.exec($(this)[0].text);
            var partsArray = /\s*(\w+)\s*\{?([A-Za-z0-9/-]*)\}?\{?([A-Za-z0-9/-]*)\}?$/.exec($(this)[0].text);
            //alert(partsArray);
            //alert(partsArray[1]);       // \s*(\w+)\s* : all alpha preceded and followed by 0|n spaces
            //alert(partsArray[2]);       // \{?([A-Za-z0-9/]*)\}?$ : 0|1 "{" followed by 0|n of [A-Za-z0-9/-] then followed by 0|1 "}"
            //alert(partsArray[3]);       // \{?([A-Za-z0-9/]*)\}?$ : 0|1 "{" followed by 0|n of [A-Za-z0-9/-] then followed by 0|1 "}"
            
            // get orientation value of selected option: 0 for Left or 1 for Right
            var checkedValue = $('.moduleRadio:checked').val();

            // get orientation text of selected option using id of the parent Label tag
            var checkedInnerHTML = document.querySelector('.moduleRadio:checked').parentNode.id;
            //var submodule = checkedValue + $(this)[0].value;

            // append the selected option
            $('#imteX > tbody:last')
                .append('<tr><td> <input type = "textbox" name = "selectedIMTEs" value = ' + checkedValue + $(this)[0].value + ' hidden >'
                    + partsArray[1] + '</td><td>'                                   // IMTE
                    + $("#ComponentDescID option:selected").val() + '</td><td>'     // Description
                    + partsArray[2] + '</td><td>'                                   // Calibration Date
                    + partsArray[3] + '</td><td>'                                   // Maintenance Date
                    + checkedInnerHTML + '</td><td><a href="#!" id=a' + $(this)[0].value + ' class="remove" rel='
                    + $(this)[0].value + ' onclick="returnIMTE(this)">Remove</a></td></tr>');

            // $(this).remove();  // remove selected option from DDL; commented out in favour of disabled and strikethrough
            $(this).prop('disabled', true).addClass("strikethrough");  // strikethrough: Works in IE11, FireFox; not in Chrome, Safari
        });
    $("#ComponentID option:eq(0)").prop('selected', true);  // default to first option
}

// Remove Selected Component and Return to cList of Components
function returnIMTE(removethis) {
    var value = $('#' + removethis.id).attr('rel');
    $("#ComponentID option[value=" + value + "]").removeAttr('disabled').removeClass("strikethrough");
    $('#a' + value).closest('tr').remove();
}

// SECTION 2c: DOCUMENT ONREADY
$(document).ready(function () {

    //checkloadjscssfile("https://cdn.datatables.net/r/bs-3.3.5/jqc-1.11.3,dt-1.10.8/datatables.min.js", "js") //success

    // Superceded by Data Tables
    //$('#myTable').pageMe({ pagerSelector: '#myPager', showPrevNext: true, hidePageNumbers: false, perPage: 10 }); // invokes jQuery.fn.pageMe plugin

    var editor; // use a global for the submit and return data rendering in the examples

    $('table.dataTable').DataTable({
        "lengthMenu": [[25, 10, 50, -1], [25, 10, 50, "All"]]
    });

    /*
    $('table' + '#scrollingTable').DataTable({
        "scrollY": "218px",
        "scrollCollapse": false,
        "paging": false,
        select: true,
        buttons: [
            { extend: "create", editor: editor },
            { extend: "edit", editor: editor },
            { extend: "remove", editor: editor }
        ]
    });
    */

    // Add event listener for opening and closing details
    // See https://datatables.net/examples/api/row_details.html
    $('#nestedTable tbody').on('click', 'td.details-control', function () {
        var table = $('#nestedTable').DataTable();
        var tr = $(this).closest('tr');
        var value = tr.attr('rel').trim();
        var partsArray = /\s*(\w+)\s*TrackMED.Models.(.+$)/.exec(value);
        var row = table.row(tr);
        var td = $(this).closest('td');
        if (row.child.isShown()) {
            // This row is already open - close it
            row.child.hide();

            tr.removeClass('shown');
            td.removeClass('glyphicon-minus');
            td.addClass('glyphicon-plus');
            td.setAttribute("colspan", "5");
            
        }
        else {
            // Open this row
            var nestedTable = showRelatedTable(partsArray[1], partsArray[2]);
            row.child(nestedTable).show();
            //row.child(showRelatedTable(partsArray[1], partsArray[2])).show();

            // row.child(formatThis(row.data())).show();
            tr.addClass('shown');
            td.removeClass('glyphicon-plus');
            td.addClass('glyphicon-minus');
            td.setAttribute("colspan", "5");
        }
    });

    // Deleting records
    /*
    $("#deleteRecord").on("click", function () {
        dialog.dialog("open");
    });
    */

});

// SECTION 2d: HELPER FUNCTIONS FOR DATATABLES
// http://stackoverflow.com/questions/13459866/javascript-change-date-into-format-of-dd-mm-yyyy
function formattedDate(date) {
    var d = new Date(date || Date.now()),
        day = '' + d.getDate(),
        month = '' + (d.getMonth() + 1),
        year = d.getFullYear();

    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;

    return [day, month, year].join('/');
}

// http://stackoverflow.com/questions/13459866/javascript-change-date-into-format-of-dd-mm-yyyy
function convertDate(inputFormat) {
    function pad(s) { return s < 10 ? '0' + s : s; }
    var d = new Date(inputFormat);
    return [pad(d.getDate()), pad(d.getMonth() + 1), d.getFullYear()].join('/');
}

function formatThis(d) {
    //alert("Inside formatThis function");
    // `d` is the original data object for the row
    return '<table cellpadding="5" cellspacing="0" border="0" style="padding-left:50px;">' +
        '<tr>' +
        '<td>IMTE:</td>' +
        '<td>' + d[1] + '</td>' +
        '</tr>' +
        '<tr>' +
        '<td>Serial number:</td>' +
        '<td>' + d[2] + '</td>' +
        '</tr>' +
        '<tr>' +
        '<td>Extra info:</td>' +
        '<td>And any further details here (images etc)...</td>' +
        '</tr>' +
        '</table>';
}

//var filesadded = "" //list of files already added

// http://www.javascriptkit.com/javatutors/loadjavascriptcss.shtml
function loadjscssfile(filename, filetype) {
    if (filetype === "js") { //if filename is a external JavaScript file
        alert("inside loadjscssfile");
        var fileref = document.createElement('script');
        fileref.setAttribute("type", "text/javascript");
        fileref.setAttribute("src", filename);
        alert("file loaded");
    }
    else if (filetype === "css") { //if filename is an external CSS file
        fileref = document.createElement("link");
        fileref.setAttribute("rel", "stylesheet");
        fileref.setAttribute("type", "text/css");
        fileref.setAttribute("href", filename);
    }
    if (typeof fileref !== "undefined")
        document.getElementsByTagName("head")[0].appendChild(fileref);
}

// http://www.javascriptkit.com/javatutors/loadjavascriptcss2.shtml
function removejscssfile(filename, filetype) {
    alert("inside removejscssfile");
    var targetelement = filetype === "js" ? "script" : filetype === "css" ? "link" : "none"; //determine element type to create nodelist from
    var targetattr = filetype === "js" ? "src" : filetype === "css" ? "href" : "none"; //determine corresponding attribute to test for
    var allsuspects = document.getElementsByTagName(targetelement);
    for (var i = allsuspects.length; i >= 0; i--) { //search backwards within nodelist for matching elements to remove
        if (allsuspects[i] && allsuspects[i].getAttribute(targetattr) !== null && allsuspects[i].getAttribute(targetattr).indexOf(filename) !== -1)
            allsuspects[i].parentNode.removeChild(allsuspects[i]); //remove element by calling parentNode.removeChild()
    }
}

//removejscssfile("somestyle.css", "css") //remove all occurences "somestyle.css" on page

function checkloadjscssfile(filename, filetype) {
    if (filesadded.indexOf("[" + filename + "]") === -1) {
        loadjscssfile(filename, filetype);
        filesadded += "[" + filename + "]"; //List of files added in the form "[filename1],[filename2],etc"
        alert("file loaded");
    }
    else
        alert("file already added!");
}
