var accessTokenTmp = "";
var disableFbButton = false;

function facebookClick() {
    if (disableFbButton) {
        return;
    }
    FB.login(function (response) {
        if (response.status == "connected") {
            accessTokenTmp = response.authResponse.accessToken;
            var hasProfile = (response.authResponse.grantedScopes.indexOf("public_profile") >= 0);
            var hasEmail = (response.authResponse.grantedScopes.indexOf("email") >= 0);
            if ((hasProfile) && (hasEmail)) {
                $("#Identifier").attr("disabled", "disabled");
                $("#Password").attr("disabled", "disabled");
                $("#loginButton").attr("disabled", "disabled");
                disableFbButton = true;
                $.post("CheckFacebookAccount", { Value: response.authResponse.userID }, function (data) {
                    if (data["unregistered"] == "true") {
                        showModal();
                    } else {
                        loginFunc();
                    }
                    $("#Identifier").removeAttr("disabled");
                    $("#Password").removeAttr("disabled");
                    $("#loginButton").removeAttr("disabled");
                    disableFbButton = false;
                });
            } else {
                temporaryFacebookMessage("All permissions must be granted.");
            }
        } else {
            temporaryFacebookMessage("Please try again.");
        }
    }, { scope: "public_profile,email", return_scopes: true, auth_type: "rerequest" });
}

function temporaryFacebookMessage(text) {
    $("#fbMsg").html(text);
    $("#idFacebook").slideDown(200, function () {
        setTimeout(function () {
            $("#idFacebook").slideUp(200);
        }, 3500);
    });
}

function showModal() {
    $("#UsernameFB").val("");
    hideMessage("usernameInfo");
    $("#modalWindow").modal();
}

var allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_.-";
var typingTimer;
var doneTypingInterval = 1200;
var messageDisplayInterval = 220;
var prevStyleUsername = "";
var prevTextUsername = "";

function showMessage(name) {
    $("#" + name).slideDown(messageDisplayInterval);
}

function hideMessage(name) {
    $("#" + name).slideUp(messageDisplayInterval);
}

function hideMessageIf(name, cond, func) {
    if (cond) {
        $("#" + name).slideUp(messageDisplayInterval, func);
    } else {
        func();
    }
}

function setMessageStyle(name, style) {
    $("#" + name).removeClass();
    $("#" + name).addClass("altAlert " + style);
}

function setMessageText(name, text) {
    $("#" + name + "Text").html(text);
}

function usernameKeyUp() {
    if (prevTextUsername == $("#UsernameFB").val()) {
        return;
    }
    prevTextUsername = $("#UsernameFB").val();
    clearTimeout(typingTimer);
    if ($("#UsernameFB").val().length >= 3) {
        var isUsernameOk = true;
        for (i = 0; i < $("#UsernameFB").val().length; i++) {
            if (allowedChars.indexOf($("#UsernameFB").val().charAt(i)) < 0) {
                isUsernameOk = false;
                break;
            }
        }
        if (isUsernameOk) {
            hideMessageIf("usernameInfo", prevStyleUsername != "checking", function () {
                setMessageStyle("usernameInfo", "alert-warning");
                setMessageText("usernameInfo", "Checking username availability...");
                showMessage("usernameInfo");
            });
            prevStyleUsername = "checking";
            typingTimer = setTimeout(usernameDoneTyping, doneTypingInterval);
        } else {
            hideMessageIf("usernameInfo", prevStyleUsername != "invalid", function () {
                setMessageStyle("usernameInfo", "alert-danger");
                setMessageText("usernameInfo", "Username contains invalid characters.");
                showMessage("usernameInfo");
            });
            prevStyleUsername = "invalid";
        }
    } else {
        if ($("#UsernameFB").val().length > 0) {
            hideMessageIf("usernameInfo", prevStyleUsername != "short", function () {
                setMessageStyle("usernameInfo", "alert-danger");
                setMessageText("usernameInfo", "Username is too short.");
                showMessage("usernameInfo");
            });
            prevStyleUsername = "short";
        } else {
            hideMessageIf("usernameInfo", prevStyleUsername != "empty", function () {
                setMessageStyle("usernameInfo", "alert-danger");
                setMessageText("usernameInfo", "This field is required.");
                showMessage("usernameInfo");
            });
            prevStyleUsername = "empty";
        }
    }
    validateAll();
}

function usernameDoneTyping() {
    $.post("CheckUsername", { Value: $("#UsernameFB").val() }, function (data) {
        if (data["available"] == "true") {
            hideMessageIf("usernameInfo", prevStyleUsername != "usernameOk", function () {
                setMessageStyle("usernameInfo", "alert-success");
                setMessageText("usernameInfo", "This username is available.");
                showMessage("usernameInfo");
            });
            prevStyleUsername = "usernameOk";
        } else {
            hideMessageIf("usernameInfo", prevStyleUsername != "usernameInUse", function () {
                setMessageStyle("usernameInfo", "alert-danger");
                setMessageText("usernameInfo", "This username is not available.");
                showMessage("usernameInfo");
            });
            prevStyleUsername = "usernameInUse";
        }
        validateAll();
    });
}

function validateAll() {
    if (prevStyleUsername == "usernameOk") {
        $("#continueButtonFB").removeAttr("disabled");
    } else {
        $("#continueButtonFB").attr("disabled", "disabled");
    }
}

function continueClick() {
    $("#fbAccountUsername").val($("#UsernameFB").val());
    $("#fbAccountAccessToken").val(accessTokenTmp);
    $("#fbAccountCreator").submit();
}

function loginFunc() {
    $("#fbLoginAccessToken").val(accessTokenTmp);
    $("#fbAccountLogin").submit();
}
