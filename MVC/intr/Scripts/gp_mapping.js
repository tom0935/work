function selProgram() {
    //使用迴圈取得所有被選擇的項目
    $("select#program").find(":selected").each(function () {
        //alert("text:" + this.text + "\nvalue:" + this.value);
        $("select#hasprogram").append($("<option></option>").attr("value", this.value).text(this.text));
    });
    //移動完成後，移除選擇的項目
    $("select#program").find(":selected").remove();
}


function delProgram() {
    //使用迴圈取得所有被選擇的項目
    $("select#hasprogram").find(":selected").each(function () {
        //alert("text:" + this.text + "\nvalue:" + this.value);
        $("select#program").append($("<option></option>").attr("value", this.value).text(this.text));
    });
    //移動完成後，移除選擇的項目
    $("select#hasprogram").find(":selected").remove();
    
}


