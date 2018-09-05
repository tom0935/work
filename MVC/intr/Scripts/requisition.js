$('#verify').tooltip('show');
$('#department').tooltip('show');

function plusMaterial(mid) {
    var NewArray = new Array();
    var NewArray = mid.split("_");
    //目前數量
    var p = parseInt($("input#" + mid).val()) + 1;
    //剩餘數量
    var inventory;
    $("span#inventory_" + NewArray[1]).each(function () {
        inventory = parseInt($(this).text());
    });
    //alert(inventory);
    if (inventory > 0) {
        $("input#" + mid).val(p);
        var NewInventory = inventory - 1;
        $("span#inventory_" + NewArray[1]).text(inventory - 1);
    }
}

function minusMaterial(mid) {
    var NewArray = new Array();
    var NewArray = mid.split("_");

    var p = 0;
    var inventory;
    var NewInventor;
    if (parseInt($("input#" + mid).val()) > 0) {
        p = parseInt($("input#" + mid).val()) - 1;

        $("span#inventory_" + NewArray[1]).each(function () {
            inventory = parseInt($(this).text());
        });

        $("input#" + mid).val(p);
        NewInventory = inventory - 1;
        $("span#inventory_" + NewArray[1]).text(inventory + 1);
    }
}

function toVerify() {
    var sum = 0;
    $("input[type='number']").each(function () {
        sum = sum + parseInt($(this).val());
    });
    if (sum > 0) {
        //alert("有選擇耗材!!" + sum + "個");
        $("input").removeAttr("disabled");
        $("input#STATUS").val("2");
        $("form").submit();
    } else {
        alert("沒有選擇任何耗材，無法送出審核!!");
    }
}

function tempSave() {
    $("input").removeAttr("disabled");
    $("form").submit();
}

