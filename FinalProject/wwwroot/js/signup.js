var allowedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890_.-";
var typingTimer;
var emailTypingTimer;
var doneTypingInterval = 1200;
var messageDisplayInterval = 220;
var infoShowDuration = 400;
var slideImageSpeed = 750;
var prevStyleUsername = "";
var prevStyleEmail = "";
var prevStylePassword = "";
var prevStyleRepPassword = "";
var prevTextUsername = "";
var prevTextEmail = "";
var prevTextPassword = "";
var prevTextRepPassword = "";

function onLoadImg() {
    $("#mainImageDesktop").slideDown(slideImageSpeed);
}

function setPrbValue(value) {
    $("#registrationProgressBar").css("width", value + "%");
    $("#registrationProgressBar").attr("aria-valuenow", value);
}

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

function updateUsername() {
    if (prevTextUsername.length > 0) {
        $("#accountAddress").css("display", "block");
    } else {
        $("#accountAddress").css("display", "none");
    }
    $("#urlAddress").html($("#idAddress").html().trim() + "User/" + prevTextUsername);
}

function usernameKeyUp() {
    if (prevTextUsername == $("#Username").val()) {
        return;
    }
    prevTextUsername = $("#Username").val();
    updateUsername();
    clearTimeout(typingTimer);
    if ($("#Username").val().length >= 3) {
        var isUsernameOk = true;
        for (i = 0; i < $("#Username").val().length; i++) {
            if (allowedChars.indexOf($("#Username").val().charAt(i)) < 0) {
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
        if ($("#Username").val().length > 0) {
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
    $.post("CheckUsername", { Value: $("#Username").val() }, function (data) {
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

function emailKeyUp() {
    if (prevTextEmail == $("#Email").val()) {
        return;
    }
    prevTextEmail = $("#Email").val();
    clearTimeout(emailTypingTimer);
    if ($("#Email").val().length > 0) {
        var idxAt = $("#Email").val().lastIndexOf("@");
        var idxDot = $("#Email").val().lastIndexOf(".");
        if ((idxAt >= 0) && (idxDot >= 0) && (idxDot > idxAt + 1) && (!($("#Email").val().endsWith(".")))) {
            hideMessageIf("emailInfo", prevStyleEmail != "checking", function () {
                setMessageStyle("emailInfo", "alert-warning");
                setMessageText("emailInfo", "Checking email availability...");
                showMessage("emailInfo");
            });
            prevStyleEmail = "checking";
            emailTypingTimer = setTimeout(emailDoneTyping, doneTypingInterval);
        } else {
            hideMessageIf("emailInfo", prevStyleEmail != "invalid", function () {
                setMessageStyle("emailInfo", "alert-danger");
                setMessageText("emailInfo", "Email is invalid.");
                showMessage("emailInfo");
            });
            prevStyleEmail = "invalid";
        }
    } else {
        hideMessageIf("emailInfo", prevStyleEmail != "empty", function () {
            setMessageStyle("emailInfo", "alert-danger");
            setMessageText("emailInfo", "This field is required.");
            showMessage("emailInfo");
        });
        prevStyleEmail = "empty";
    }
    validateAll();
}

function emailDoneTyping() {
    $.post("CheckEmail", { Value: $("#Email").val() }, function (data) {
        if (data["available"] == "true") {
            hideMessageIf("emailInfo", prevStyleEmail != "emailOk", function () {
                setMessageStyle("emailInfo", "alert-success");
                setMessageText("emailInfo", "This email is available.");
                showMessage("emailInfo");
            });
            prevStyleEmail = "emailOk";
        } else {
            hideMessageIf("emailInfo", prevStyleEmail != "emailInUse", function () {
                setMessageStyle("emailInfo", "alert-danger");
                setMessageText("emailInfo", "This email is not available.");
                showMessage("emailInfo");
            });
            prevStyleEmail = "emailInUse";
        }
        validateAll();
    });
}

function passwordKeyUp() {
    if (prevTextPassword == $("#Password").val()) {
        return;
    }
    prevTextPassword = $("#Password").val();

    if ($("#Password").val().length >= 6) {
        var alpha = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var numeric = "1234567890";

        var containsAlpha = false;
        var containsNumeric = false;
        var containsOther = false;

        for (i = 0; i < $("#Password").val().length; i++) {
            if (alpha.indexOf($("#Password").val().charAt(i)) >= 0) {
                containsAlpha = true;
            } else if (numeric.indexOf($("#Password").val().charAt(i)) >= 0) {
                containsNumeric = true;
            } else {
                containsOther = true;
            }
        }

        if ((containsAlpha && containsNumeric && containsOther) && ($("#Password").val().length >= 16)) {
            hideMessageIf("passwordInfo", prevStylePassword != "passwordOk", function () {
                setMessageStyle("passwordInfo", "alert-success");
                setMessageText("passwordInfo", "Strong password.");
                showMessage("passwordInfo");
            });
        } else if (((containsAlpha && containsNumeric) || (containsAlpha && containsOther) || (containsNumeric && containsOther)) && ($("#Password").val().length >= 8)) {
            hideMessageIf("passwordInfo", prevStylePassword != "passwordOk", function () {
                setMessageStyle("passwordInfo", "alert-success");
                setMessageText("passwordInfo", "Fair password.");
                showMessage("passwordInfo");
            });
        } else {
            hideMessageIf("passwordInfo", prevStylePassword != "passwordOk", function () {
                setMessageStyle("passwordInfo", "alert-success");
                setMessageText("passwordInfo", "Weak password.");
                showMessage("passwordInfo");
            });
        }
        
        prevStylePassword = "passwordOk";
    } else {
        if ($("#Password").val().length == 0) {
            hideMessageIf("passwordInfo", prevStylePassword != "empty", function () {
                setMessageStyle("passwordInfo", "alert-danger");
                setMessageText("passwordInfo", "This field is required.");
                showMessage("passwordInfo");
            });
            prevStylePassword = "empty";
        } else {
            hideMessageIf("passwordInfo", prevStylePassword != "short", function () {
                setMessageStyle("passwordInfo", "alert-danger");
                setMessageText("passwordInfo", "Password is too short.");
                showMessage("passwordInfo");
            });
            prevStylePassword = "short";
        }
    }

    $("#RepeatPassword").val("");
    prevStyleRepPassword = "";
    hideMessage("repPasswordInfo");

    validateAll();
}

function repPasswordKeyUp() {
    if (prevTextRepPassword == $("#RepeatPassword").val()) {
        return;
    }
    prevTextRepPassword = $("#RepeatPassword").val();
    if (prevStylePassword == "passwordOk") {
        if ($("#Password").val() == $("#RepeatPassword").val()) {
            hideMessageIf("repPasswordInfo", prevStyleRepPassword != "repPasswordOk", function () {
                setMessageStyle("repPasswordInfo", "alert-success");
                setMessageText("repPasswordInfo", "Passwords match.");
                showMessage("repPasswordInfo");
            });
            prevStyleRepPassword = "repPasswordOk";
        } else {
            hideMessageIf("repPasswordInfo", prevStyleRepPassword != "invalid", function () {
                setMessageStyle("repPasswordInfo", "alert-danger");
                setMessageText("repPasswordInfo", "Passwords do not match.");
                showMessage("repPasswordInfo");
            });
            prevStyleRepPassword = "invalid";
        }
    }
    validateAll();
}

function infoShow(focObj, doShow) {
    var target = null;
    switch (focObj) {
        case 0:
            target = $("#usernameInfoData");
            break;
        case 1:
            target = $("#emailInfoData");
            break;
        case 2:
            target = $("#passwordInfoData");
            break;
        case 3:
            target = $("#repPasswordInfoData");
            break;
        case 4:
            target = $("#firstNameInfoData");
            break;
        case 5:
            target = $("#lastNameInfoData");
            break;
        case 6:
            target = $("#genderInfoData");
            break;
    }
    if (target != null) {
        if (doShow) {
            target.slideDown(infoShowDuration);
        } else {
            target.slideUp(infoShowDuration);
        }
    }
}

function focusChange(focObj) {
    infoShow(focObj, true);
}

function blurChange(focObj) {
    infoShow(focObj, false);
}

function validateAll() {
    var isOk = true;
    var prbValue = 0;
    if (prevStyleUsername == "usernameOk") {
        prbValue += 20;
    } else {
        $("#registerButton").attr("disabled", "disabled");
        isOk = false;
    }
    if (prevStyleEmail == "emailOk") {
        prbValue += 20;
    } else {
        $("#registerButton").attr("disabled", "disabled");
        isOk = false;
    }
    if (prevStylePassword == "passwordOk") {
        prbValue += 20;
    } else {
        $("#registerButton").attr("disabled", "disabled");
        isOk = false;
    }
    if (prevStyleRepPassword == "repPasswordOk") {
        prbValue += 20;
    } else {
        $("#registerButton").attr("disabled", "disabled");
        isOk = false;
    }
    if (($("#FirstName").val() != "") && ($("#LastName").val() != "") && ($("#Gender").find(":selected").val() != "")) {
        prbValue += 20;
    } else {
        $("#registerButton").attr("disabled", "disabled");
        isOk = false;
    }
    setPrbValue(prbValue);
    if (isOk) {
        $("#registerButton").removeAttr("disabled");
    }
}
