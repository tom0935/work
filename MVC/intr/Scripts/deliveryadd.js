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
    $("input").removeAttr("disabled");
    $("form").submit();
}


