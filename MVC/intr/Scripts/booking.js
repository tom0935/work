/* File Created: 十二月 30, 2014 */

function initOrderDataGrid() {

    $('#OrderDataGrid').datagrid({
        title: '線上訂房資料 (查詢條件：CheckOut >= Today)',
        autoRowHeight: false,
        url: getGridJSON,
        border: false,
        nowrap: false,
        pagination: false,
        //pageSize: 20,
        //pageList: [20, 50, 100],
        fit: true,
        idField: 'SN',
        striped: true,
        showHeader: true,
        fitColumns: true,
        remoteSort: true,
        singleSelect: true,
        sortName: 'SN',
        sortOrder: 'desc',
        //  maximized: true,
        rownumbers: true,
        striped: true,
        showPageList: true,
        pagePosition: 'bottom',
        toolbar: [{
            id: 'removePidBtn',
            text: '移除前台PID',
            disabled: false,
            iconCls: 'icon-no',
            handler: function () {
                var selectRow = $('#OrderDataGrid').datagrid('getSelected');
                var pidStatus;
                if (selectRow != null) {
                    if (selectRow.RSVSTAT != "30" && selectRow.RSVSTAT != "14" && selectRow.ResultNo == "1") {
                        pidStatus = removePid(selectRow.SN);
                        if (pidStatus == "OK") {
                            $.messager.alert('系統訊息', '此訂單' + selectRow.SN + '已移除PID。', 'info');
                            initOrderDataGrid();
                        } else {
                            $.messager.alert(pidStatus, 'info');
                        }
                    } else {
                        $.messager.alert('系統訊息', '此訂單' + selectRow.SN + '非正常狀態，無法移除PID!', 'warning');
                    }
                } else {
                    $.messager.alert('系統訊息', '您尚未選取資料', 'warning');
                }
            }
        }/*, {
            id: 'orderTransformBtn',
            text: '重新轉入前台',
            iconCls: 'icon-reload',
            disabled: false,
            handler: function () {
                var selectRow = $('#OrderDataGrid').datagrid('getSelected');
                if (selectRow != null) {
                    $.messager.confirm('系統訊息', '單號' + selectRow.SN + ' 確定重新轉入訂單?', function (r) {
                        if (r) {
                            printOrder(selectRow.ODDNO, selectRow.STATUS);
                        }
                    });
                } else {
                    $.messager.alert('系統訊息', '您尚未選取資料', 'warning');
                }
            }
        }, {
            id: 'searchDateBtn',
            text: '日期範圍',
            disabled: false,
            iconCls: 'Calendar-icon',
            handler: function () {
                //alert('近日訂購');
                openDialog('訂房日期');
            }
        }*/],
        columns: [[
                    { field: 'SN',
                        title: '官網單號(PID)',
                        //width: 100,
                        halign: "center",
                        align: "center",
                        sortable: false
                    },
                    { field: 'HotelSn',
                        title: '飯店',
                        //width: 100,
                        halign: "center",
                        align: "center",
                        sortable: false,
                        formatter: function (value, row, index) {
                            var h = parseInt(row.HotelSn);
                            switch (h) {
                                case 162:
                                    value = "台北";
                                    break;
                                case 164:
                                    value = "台中";
                                    break;
                                case 165:
                                    value = "高雄";
                                    break;
                            }
                            return value;
                        }
                    },
                    {
                        field: 'Name',
                        title: '姓名',
                        //width: 150,
                        halign: "center",
                        align: "center",
                        sortable: false
                    },
                    {
                        field: 'PackageName',
                        title: '專案',
                        //width: 150,
                        halign: "center",
                        align: "center",
                        sortable: false
                    },
                    {
                        field: 'WebName',
                        title: '房型',
                        //width: 150,
                        halign: "center",
                        align: "center",
                        sortable: false
                    },
                    {
                        field: 'ResultNo',
                        title: '狀態',
                        //width: 50,
                        halign: "center",
                        align: "center",
                        sortable: false,
                        formatter: function (value, row, index) {
                            var r = parseInt(row.ResultNo);
                            switch (r) {
                                case 0:
                                    value = "<font color='#FE2E64'>後台取消</font>(" + value + ")";
                                    break;
                                case 1:
                                    value = "<font color='#2E64FE'>已付款</font>(" + value + ")";
                                    break;
                                case 3:
                                    value = "<font color='#000'>已過期</font>(" + value + ")";
                                    break;
                                case 9:
                                    value = "<font color='#A4A4A4'>未付款</font>(" + value + ")";
                                    break;
                                case 20:
                                case 22:
                                case 24:
                                case 28:
                                    value = "<font color='#A4A'>ATM暫訂</font>(" + value + ")";
                                    break;
                            }
                            return value;
                        }
                    },
                    {
                        field: 'ProcessNo',
                        title: '前台轉入',
                        //width: 50,
                        halign: "center",
                        align: "center",
                        sortable: false,
                        formatter: function (value, row, index) {
                            var p = parseInt(row.ProcessNo);
                            switch (p) {
                                case 0:
                                    value = "<font color='#FE2E64'>未處理</font>(" + value + ")";
                                    break;
                                case 2:
                                    value = "<font color='#2E64FE'>已處理</font>(" + value + ")";
                                    break;
                                case 1:
                                    value = "<font color='#A4A'>處理中</font>(" + value + ")";
                                    break;
                            }
                            return value;
                        }
                    },
                    {
                        field: 'RSVNO',
                        title: '前台單號',
                        //width: 50,
                        halign: "center",
                        align: "center",
                        sortable: false
                    },
                    {
                        field: 'RSVSTAT',
                        title: '前台狀態',
                        //width: 50,
                        halign: "center",
                        align: "center",
                        sortable: false,
                        formatter: function (value, row, index) {
                            var rs = parseInt(row.RSVSTAT);
                            switch (rs) {
                                case 10:
                                    value = "<font color='#2E64FE'>ACTV</font>(" + value + ")";
                                    break;
                                case 11:
                                    value = "<font color='#A4A'>RCFM</font>(" + value + ")";
                                    break;
                                case 12:
                                    value = "<font color='#A4A'>CFM</font>(" + value + ")";
                                    break;
                                case 13:
                                    value = "<font color='#A4A'>WAIT</font>(" + value + ")";
                                    break;
                                case 14:
                                    value = "<font color='#FE2E64'>CXNL</font>(" + value + ")";
                                    break;
                                case 15:
                                    value = "<font color='#A4A'>NOSH</font>(" + value + ")";
                                    break;
                                case 16:
                                    value = "<font color='#A4A'>DUMM</font>(" + value + ")";
                                    break;
                                case 20:
                                    value = "<font color='#088A29'>I/H</font>(" + value + ")";
                                    break;
                                case 21:
                                    value = "<font color='#088A29'>WALK</font>(" + value + ")";
                                    break;
                                case 30:
                                    value = "<font color='#FE2E64'>C/O</font>(" + value + ")";
                                    break;
                            }
                            return value;
                        }
                    },
                    {
                        field: 'TotalAmount',
                        title: '訂金',
                        //width: 150,
                        halign: "center",
                        align: "center",
                        sortable: false,
                        formatter: function (value, row, index) {
                            value = "$" + row.TotalAmount;
                            return value;
                        }
                    },
                    {
                        field: 'BookingDate',
                        title: '訂房日期',
                        //width: 50,
                        halign: "center",
                        align: "center",
                        sortable: false,
                        formatter: function (value, row, index) {
                            value = "<span title='" + row.StartSearchTime + "'>" + value + "</span>";
                            return value;
                        }
                    }
                ]],
        onLoadSuccess: function (data) {
            //$('#panelNorth').panel('setTitle', '訂單基本資料:部門別-' + data.title);
        }
    });

}


function openDialog(searchStr) {
    var d = new Date();
    var month = d.getMonth();
    var day = d.getDate();
    var dlg;
    var ndate = d.getFullYear() + '-' + (month < 10 ? '0' : '') + month + '-' + (day < 10 ? '0' : '') + day;
    var dateToday = DateTimeUtil.parseDate(DateTimeUtil.transferIsoDate(ndate));
    var sdt = new Date(dateToday); // 範圍最早日期
    sdt.setDate(sdt.getDate());
    var sdtStr = DateTimeUtil.convertDateToISOString(sdt);

    var edtDate = new Date();
    edtDate.setDate(edtDate.getDate());
    var edtStr = DateTimeUtil.convertDateToISOString(edtDate);
    $('#SDT').datebox('setValue', sdtStr);
    $('#EDT').datebox('setValue', edtStr);
    $('#SDT1').datebox('setValue', sdtStr);
    $('#EDT1').datebox('setValue', edtStr);

    //$('#posnoDiv').hide();
    dlgmode = "2";
    dlg = 'dlg';
    /*
    if (searchStr == "訂購排行") {
    dlgmode = "1";
    dlg = 'dlgOrder';
    } else if (searchStr == "近日訂購") {
    dlgmode = "2";
    dlg = 'dlg';
    } else if (searchStr == "彙總表") {
    dlgmode = "3";
    dlg = 'dlg';
    //$('#posnoDiv').show();
    }
    */
    $('#dlgmode').val(dlgmode);

    var obj = {};
    $('#' + dlg).dialog({
        title: '查詢服務:' + searchStr,
        width: 400,
        height: 210,
        closed: false,
        cache: false,
        modal: true,
        iconCls: 'icon-search',
        buttons: [{
            text: '查詢',
            iconCls: 'icon-search',
            handler: function () {
                doSearchByDlg();
            }
        },
                  {
                      text: '取消',
                      iconCls: 'icon-cancel',
                      handler: function () {
                          $('#' + dlg).dialog('close');
                      }
                  }]
    });
}

function dlgCheckNowDate(obj) {
    var edtDate = new Date();
    edtDate.setDate(edtDate.getDate());
    var edtStr = DateTimeUtil.convertDateToISOString(edtDate);
    $('#' + obj).datebox('setValue', edtStr);
}