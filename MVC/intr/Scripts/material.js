$('#MaterialType').tooltip('show')
$('#Materials').tooltip('show')
$('#store').tooltip('show')

function addMaterial() {
    var mid = $("#Materials :selected").val();
    var flag = $("input[name='" + mid + "']").val();
    //alert(flag);
    if (flag == "" || flag == null || flag == "undefined ") {
        var selectedName = $("#Materials :selected").text().split("：");
        //加入一行TR資料
        $("table tbody").append(
        $("<tr>").append(
            $("<td>").text($("table tbody tr").length + 1),
            $("<td>").text(selectedName[0]),
            $("<td>").text(selectedName[1]),
            $("<td>").html(
                $("<input>").attr("class", "form-control")
                .attr("type", "number")
                .attr("id", mid)
                .attr("name", mid)
                .attr("value", "1")
            ),
            $("<td>").html(
                $("<button>").attr("type", "button")
                .attr("class", "btn btn-info")
                .text("＋")
                .click(function () {
                    var p = parseInt($("input#" + mid).val()) + 1;
                    $("input#" + mid).val(p);
                })
            ).append(
                $("<button>").attr("type", "button")
                .attr("class", "btn btn-warning")
                .text("－")
                .click(function () {
                    var p = ($("input#" + mid).val() == "1") ? "1" : (parseInt($("input#" + mid).val()) - 1);
                    $("input#" + mid).val(p);
                })
            ).append(
                $("<button>").attr("type", "button")
                .attr("class", "btn btn-danger")
                .text("整筆移除")
                .click(function () {
                    $(this).parents("tr:first").remove();
                })
            )
        )
        );
    }
}

function rmMaterial(it) {
    $(it).parents("tr:first").remove();
}

function plusMaterial(mid) {
    var p = parseInt($("input#" + mid).val()) + 1;
    $("input#" + mid).val(p);
}

function minusMaterial(mid) {
    var p = ($("input#" + mid).val() == "1") ? "1" : (parseInt($("input#" + mid).val()) - 1);
    $("input#" + mid).val(p);
}

function toStore() {
    $("input#STATUS").val("2");
    $("form").submit();
}