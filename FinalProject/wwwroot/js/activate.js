var validChars = "1234567890";

function onChange() {
    $("#activateButton").attr("disabled", "disabled");
    var code = $("#Code").val();
    if (code.length < 6) {
        return;
    }
    for (var i = 0; i < code.length; i++) {
        if (validChars.indexOf(code.charAt(i)) < 0) {
            return;
        }
    }
    $("#activateButton").removeAttr("disabled");
}
