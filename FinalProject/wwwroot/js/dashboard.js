var working = false;

function onChangePw() {
    if (($("#opass").val().length < 6) || ($("#opass").val().length > 64) || ($("#npass").val().length < 6) || ($("#npass").val().length > 64)) {
        $("#pwbtn").attr("disabled", "disabled");
        return;
    }
    if ($("#npass").val() != $("#rpass").val()) {
        $("#pwbtn").attr("disabled", "disabled");
        return;
    }
    $("#pwbtn").removeAttr("disabled");
}

function clickPw() {
    if (working) {
        return;
    }
    working = true;
    if (($("#opass").val().length < 6) || ($("#opass").val().length > 64) || ($("#npass").val().length < 6) || ($("#npass").val().length > 64)) {
        return;
    }
    if ($("#npass").val() != $("#rpass").val()) {
        return;
    }
    $.post("ChangePassword", { Username: $("#userName").html().trim(), OldPassword: $("#opass").val(), NewPassword: $("#npass").val() }, function (data) {
        $("#opass").val("");
        $("#npass").val("");
        $("#rpass").val("");
        if (data["success"] == "true") {
            $("#successPw").slideDown(200, function () {
                setTimeout(function () {
                    $("#successPw").slideUp(200);
                }, 3500);
            });
        } else {
            $("#failurePw").slideDown(200, function () {
                setTimeout(function () {
                    $("#failurePw").slideUp(200);
                }, 3500);
            });
        }
        working = false;
    });
}
