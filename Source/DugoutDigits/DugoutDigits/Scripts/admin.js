function render_coachrequests() {
    $.ajax({
        url: "Admin/GetCoachRequests",
        dataType: "json",
        success: function (data) {
            var returnVal = "<h3>Pending Coach Permission Requests</h3>\n";
            returnVal += data.message;
            returnVal += "<div id='coachrequests-message'></div>";

            $('#content-buffer').html(returnVal);

            // Grow the content section to the right size
            var newHeight = $('#content-buffer').height();
            $('#inner-content').animate({ height: newHeight + 50 }, 500, "linear", function () {
                $('#innercontent-coachrequests').html($('#content-buffer').html()).hide().fadeIn();
            });
        },
        error: function () {
            $('#innercontent-coachrequests').html("<p>Error getting coach permission requests.</p>");
        }
    });

    return "<div id='innercontent-coachrequests'></div>";
}

function render_invalidrequests() {
    $.ajax({
        url: "Admin/GetInvalidRequests",
        dataType: "json",
        success: function (data) {
            var returnVal = "<h3>Invalid Request Attempts</h3>\n";
            returnVal += data.message;
            returnVal += "<div id='invalidrequests-message'></div>";

            $('#content-buffer').html(returnVal);

            // Grow the content section to the right size
            var newHeight = $('#content-buffer').height();
            $('#inner-content').animate({ height: newHeight + 50 }, 500, "linear", function () {
                $('#innercontent-invalidrequests').html($('#content-buffer').html()).hide().fadeIn();
            });
        },
        error: function () {
            $('#innercontent-invalidrequests').html("<p>Error getting invalid requests.</p>");
        }
    });

    return "<div id='innercontent-invalidrequests'></div>";
}

function action_acceptcoachrequest(requestID) {
    var p = { "requestID": requestID };

    /* make the call to the accept coach request web service */
    $.ajax({
        url: "Admin/AcceptCoachRequest",
        data: p,
        dataType: "json",
        success: function (data) {
            submenu_handler("subtab-coachrequests");
            $('#coachrequests-message').html(data.message);
        },
        error: function () {
            $('#coachrequests-message').html("Error making the accept request call.");
        }
    });
}

function action_declinecoachrequest(requestID) {
    var p = { "requestID": requestID };

    /* make the call to the decline coach request web service */
    $.ajax({
        url: "Admin/DeclineCoachRequest",
        data: p,
        dataType: "json",
        success: function (data) {
            submenu_handler("subtab-coachrequests");
            $('#coachrequests-message').html(data.message);
        },
        error: function () {
            $('#coachrequests-message').html("Error making the decline request call.");
        }
    });
}

function submenu_handler(itemClicked) {
    submenu_clearClasses();
    $("#" + itemClicked).removeClass("inactive-subtab").addClass("active-subtab");
    switch (itemClicked) {
        case "subtab-coachrequests":
            $('#inner-content').html(render_coachrequests());
            break;
        case "subtab-invalidrequests":
            $('#inner-content').html(render_invalidrequests());
            break;
        default:
            $('#inner-content').html("An error seems to have occured. Sorry for the inconvenience.");
            break;
    }
}

function submenu_clearClasses() {
    $('#subtab-coachrequests').removeClass("active-subtab inactive-subtab").addClass("inactive-subtab");
    $('#subtab-invalidrequests').removeClass("active-subtab inactive-subtab").addClass("inactive-subtab");
}

$(document).ready(function () {
    /* register click listeners with the sub tabs */
    $('#subtab-coachrequests').click(function () { submenu_handler("subtab-coachrequests"); });
    $('#subtab-invalidrequests').click(function () { submenu_handler("subtab-invalidrequests"); });

    /* set the default sub tab */
    submenu_handler("subtab-coachrequests");
})