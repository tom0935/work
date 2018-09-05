//初始化參數
if (typeof (seqno) == 'undefined')
    var seqno = new Number(1);

$('#DepartmentLists').tooltip('show')

function addDepartment() {
    var did = $("#DepartmentLists :selected").val();
    
    var selectedName = $("#DepartmentLists :selected").text().split("：");
    //加入一行TR資料
    $("table tbody").append(
        $("<tr>").append(
            $("<td>").text(seqno),
            $("<td>").text(selectedName[0]),
            $("<td>").text(selectedName[1]),
            $("<td>").html(
                $("<input>").attr("type", "hidden")
                .attr("id", did + "_" + seqno)
                .attr("name", did + "_" + seqno)
                .attr("value", did)
            ).append(
                $("<input>").attr("class", "form-control")
                .attr("type", "text")
                .attr("id", did + "_" + seqno + "T")
                .attr("name", did + "_" + seqno + "T")
                .attr("value", "")
                .attr("placeholder", "細分部門小單位，於此填寫名稱")
            ),
            $("<td>").html(
                $("<input>").attr("class", "form-control chk-size")
                .attr("type", "checkbox")
                .attr("id", did + "_" + seqno + "NS")
                .attr("name", did + "_" + seqno + "NS")
                .attr("value", "1")
                .attr("checked", "checked")
            ),
            $("<td>").html(
                $("<button>").attr("type", "button")
                .attr("class", "btn btn-danger")
                .text("整筆移除")
                .click(function () {
                    $(this).parents("tr:first").remove();
                })
            )
        )
    );

    ++seqno;
}

function rmDepartment(it) {
    $(it).parents("tr:first").remove();
}
