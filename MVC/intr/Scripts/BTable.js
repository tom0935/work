function showalert(message, alerttype) {
    var i = 6000;
    $('#alert_placeholder').append('<div id="alertdiv" class="alert ' + alerttype + '"><a class="close" data-dismiss="alert"> x </a><span>' + message + '</span></div>');
    if (alerttype == "alert-danger") {
        i= 6000   //¿ù»~°T®§°±¯d1¤À
    }
        setTimeout(function () { 
        $("#alertdiv").remove();
    }, i);
}
/*
function initTable() {
    var scripts = [
               location.search.substring(1) || '/GiftCard/Scripts/bootstrap-table.min.js',
               '/GiftCard/Scripts/bootstrap-table-zh-TW.min.js',
               '/GiftCard/Scripts/extensions/export/bootstrap-table-export.js',
               '/GiftCard/Scripts/tableExport.js',
               '/GiftCard/Scripts/extensions/editable/bootstrap-table-editable.js',
               '/GiftCard/Scripts//bootstrap-editable.js'
    ],
           eachSeries = function (arr, iterator, callback) {
               callback = callback || function () { };
               if (!arr.length) {
                   return callback();
               }
               var completed = 0;
               var iterate = function () {
                   iterator(arr[completed], function (err) {
                       if (err) {
                           callback(err);
                           callback = function () { };
                       }
                       else {
                           completed += 1;
                           if (completed >= arr.length) {
                               callback(null);
                           }
                           else {
                               iterate();
                           }
                       }
                   });
               };
               iterate();
           };

    eachSeries(scripts, getScript, initTable);
}

*/

function getScript(url, callback) {
    var head = document.getElementsByTagName('head')[0];
    var script = document.createElement('script');
    script.src = url;

    var done = false;
    // Attach handlers for all browsers
    script.onload = script.onreadystatechange = function () {
        if (!done && (!this.readyState ||
                this.readyState == 'loaded' || this.readyState == 'complete')) {
            done = true;
            if (callback)
                callback();

            // Handle memory leak in IE
            script.onload = script.onreadystatechange = null;
        }
    };

    head.appendChild(script);

    // We handle everything using the script element injection
    return undefined;
}

function queryParams(params) {
    return {
        limit: params.pageSize,
        offset: params.pageSize * (params.pageNumber - 1),
        search: params.searchText,
        name: params.sortName,
        order: params.sortOrder
    };
}

function totalTextFormatter(data) {
    return 'Total';
}

function totalNameFormatter(data) {
    return data.length;
}

function totalPriceFormatter(data) {
    var total = 0;
    $.each(data, function (i, row) {
        total += +(row.price.substring(1));
    });
    return '$' + total;
}

function getHeight() {
    return $(window).height() - $('h1').outerHeight(true);
}


function getIdSelections() {
    
    return $.map($('#table').bootstrapTable('getSelections'), function (row) {
        return row.UUID
    });
}


function getDTLIdSelections() {

    return $.map($('#dtltable').bootstrapTable('getSelections'), function (row) {
        return row.UUID_DTL
    });
}


function responseHandler(res) {
    
    $.each(res.rows, function (i, row) {
        row.state = $.inArray(row.UUID, selections) !== -1;
    });
    return res;
}
function responseHandlerDTL(res) {

    $.each(res.rows, function (i, row) {
        row.state = $.inArray(row.UUID_DTL, selections) !== -1;
    });
    return res;
}

function getIdSelect(str) {

    return $.map($('#'+str).bootstrapTable('getSelections'), function (row) {
        return row.UUID
    });
}