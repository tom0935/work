       
        $("#QTY").textbox({ onChange: function (newValue, oldValue) {
            alert(oldValue);
        }
        });
        /*
        $('#SALE_RATE').textbox('textbox').css({ "background-color": "#E3E3E3" });
        $('#TOT').numberbox('textbox').css({ "background-color": "#E3E3E3" });
        var Today = new Date();
        var NowDate = DateTimeUtil.convertDateToISOString(Today);
        $('#APPLY_DT').datebox('setValue', NowDate);
 

        $("#CMPNAME_SEARCH").textbox('textbox').bind('keypress', function (e) {
        if (e.which == 13) {
        var row = $('#CusCompanyDG').datagrid('getSelected');
        if (row != null) {
        selectCusCompany();
        $('#CusCompanyDialog').dialog('close');
        } else {
        openCusCompanyDialog(this.value);
        }
        }

        });

        $("#CUS_SEARCH").textbox('textbox').bind('keypress', function (e) {
        if (e.which == 13) {
        var row = $('#CusDG').datagrid('getSelected');
        if (row != null) {
        selectCus();
        $('#CusDialog').dialog('close');
        } else {
        openCusDialog(this.value);
        }
        }
        });


        $("#QTY").numberbox('textbox').bind('blur', function (e) {
        

        var prc = $("#UPRC").numberbox('getValue');
        showSumPrice(prc);
        });

        $("#UPRC").numberbox('textbox').bind('blur', function (e) {
        showSumPrice(this.value);
        });

        $("#ACTAMT").numberbox('textbox').bind('blur', function (e) {
        showRate();
        });



        $('#img').change(function (event) {
        $('#imgDiv').text($('#img').val());
        $('#imgDiv2').text('');
        });*/