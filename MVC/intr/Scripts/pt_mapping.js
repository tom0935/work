function selToner() {
    //使用迴圈取得所有被選擇的項目
    $("select#toner").find(":selected").each(function () {
        $("select#hastoner").append($("<option></option>").attr("value", this.value).text(this.text));
    });
    //移動完成後，移除選擇的項目
    $("select#toner").find(":selected").remove();
}


function delToner() {
    //使用迴圈取得所有被選擇的項目
    $("select#hastoner").find(":selected").each(function () {
        $("select#toner").append($("<option></option>").attr("value", this.value).text(this.text));
    });
    //移動完成後，移除選擇的項目
    $("select#hastoner").find(":selected").remove();
}


