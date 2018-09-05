function selDepart()
{
    //使用迴圈取得所有被選擇的項目
	$("select#department").find(":selected").each(function() {
	    //alert("text:" + this.text + "\nvalue:" + this.value);
	    $("select#workin").append($("<option></option>").attr("value", this.value).text(this.text));
	});
	//移動完成後，移除選擇的項目
	$("select#department").find(":selected").remove();
}


function delDepart()
{
    //使用迴圈取得所有被選擇的項目
    $("select#workin").find(":selected").each(function () {
        //alert("text:" + this.text + "\nvalue:" + this.value);
        $("select#department").append($("<option></option>").attr("value", this.value).text(this.text));
    });
    //移動完成後，移除選擇的項目
    $("select#workin").find(":selected").remove();
}


function checkForm()
{
  //送出所有Workin的項目
    $("select#workin").find("option").attr("selected", true);
  return true;
}