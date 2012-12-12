function render_myteams() {
    $('#inner-content').html("<div id='team-tables'></div><div id='team-requests'></div>");
    $.ajax({
        url: "Team/AJAX_GetTeamsTable",
        success: function (data) {
            $('#team-tables').html(data.message);
        },
        error: function () {
            alert("error making get teams table call");
        }
    });

    $.ajax({
        url: "Team/AJAX_GetRequestTable",
        success: function (data) {
            $('#team-requests').html(data.message);
        },
        error: function () {
            alert("error making get requests table call");
        }
    });
}

function render_searchteams() {
    var returnVal = "<h3>Search For a Team</h3>\n";
    returnVal += "<p>Use the form to search for a team by name.</p>\n";
    returnVal += "<form>\n<h4>Team Name</h4>\n";
    returnVal += "<input type='text' name='search-team-name' onkeypress='formsubmithandler(event, \"search\")' />\n";
    returnVal += "<div class='submit-button' onClick='action_searchteams()'>Search</div>\n</form>\n";
    returnVal += "<div id='searchteams-success-message'></div>";
    return returnVal;
}

function render_addteam() {
    var returnVal = "<h3>Add New Team</h3>\n";
    returnVal += "<p><strong>Note:</strong> If you are not the coach, have the coach create the team. The coach can then ";
    returnVal += "invite players to join the team or players can search for and request to join the team.</p>\n";
    returnVal += "<form>\n<h4>Team Name</h4>\n";
    returnVal += "<input type='text' name='team-name' onkeypress='formsubmithandler(event, \"add\")' />\n";
    returnVal += "<div class='submit-button' onClick='action_addteam()'>Add Team</div>\n</form>\n";
    returnVal += "<div id='addteam-success-message'></div>";
    return returnVal;
}

function render_inviteuser() {
    /* Call and get a drop down of the teams. */
    var p = { "association": 0 };
    $.ajax({
        url: "Team/AJAX_GetTeamsDropDown",
        data: p,
        dataType: "json",
        success: function (data) {
            var returnVal = "<h3>Invite Your Team</h3>\n";
            returnVal += "<p>Send invites to your team using the form below. When players create an account with Dugout ";
            returnVal += "Digits they will be able to COMPLETE THIS INFORMATION LATER.</p>\n";
            returnVal += "<form>\n<h4>Email</h4>\n";
            returnVal += "<input type='text' name='invite-email' />\n";
            returnVal += "<h4>Message</h4>\n";
            returnVal += "<input type='text' name='invite-message' onkeypress='formsubmithandler(event, \"invite\")' />\n";
            returnVal += "<h4>Invite To Join</h4>\n";
            returnVal += data.message;
            returnVal += "<div class='submit-button' onClick='action_inviteuser()'>Send an Invite</div>\n</form>\n";
            returnVal += "<div id='inviteuser-success-message'></div>";
            $('#innercontent-inviteuser').html(returnVal);
        },
        error: function () {
            $('#innercontent-inviteuser').html("<p>Error building invitation form.</p>");
        }
    });

    return "<div id='innercontent-inviteuser'></div>";
}

function render_registercoach() {
    var returnVal = "<h3>Interested in Joining?</h3>\n";
    returnVal += "<p>If you'd like to use Dugout Digits to help manage your team send an email ";
    returnVal += "to <a href='mailto:admin@dugoutdigits.com'>admin@dugoutdigits.com</a></p>\n";
    returnVal += "<p>Be sure to include the email you use to login to Dugout Digits in the message</p>\n";
    return returnVal;
}

function action_searchteams() {
    /* get the team name from the input field */
    var p = { "teamName": $('input[name="search-team-name"]').val() };

    /* make the call to the add team web service */
    $.ajax({
        url: "Team/AJAX_SearchTeams",
        data: p,
        dataType: "json",
        success: function (data) {
            $('#searchteams-success-message').html(data.message);
        },
        error: function () {
            alert("error making search teams call");
        }
    });
}

function action_addteam() {
    /* get the team name from the input field */
    var p = { "teamName": $('input[name="team-name"]').val() };

    /* make the call to the add team web service */
    $.ajax({
        url: "Team/AJAX_AddTeam",
        data: p,
        dataType: "json",
        success: function (data) {
            if (data.message == $('input[name="team-name"]').val() + " successfully added.") {
                $('input[name="team-name"]').val("");
                sidebar_listTeams();
            }
            $('#addteam-success-message').html(data.message);
        },
        error: function () {
            alert("error making add team call");
        }
    });
}

function action_inviteuser() {
    $('#inviteuser-success-message').html("");

    /* get the team name from the input field */
    var p = { "inviteEmail": $('input[name="invite-email"]').val(), "inviteMessage": $('input[name="invite-message"]').val(), "teamID": $('#teamdropdown').val() };
    /*var p = { "inviteEmail": $('input[name="invite-email"]').val(), "inviteMessage": $('input[name="invite-message"]').val(), "teamID": $('input[name="teamdropdown"]').val() };*/

    /* make the call to the invite user web service */
    $.ajax({
        url: "Team/AJAX_InviteUser",
        data: p,
        dataType: "json",
        success: function (data) {
            $('#inviteuser-success-message').html(data.message);
        },
        error: function () {
            alert("error making invite call");
        }
    });
}

function action_requestjoin(teamID) {
    var p = { "teamID": teamID };

    /* make the call to the add request web service */
    $.ajax({
        url: "Team/AJAX_AddRequest",
        data: p,
        dataType: "json",
        success: function (data) {
            alert(data.message);
        },
        error: function () {
            alert(data.message);
        }
    });
}

function action_acceptrequest(requestID) {
    var p = { "requestID": requestID };

    /* make the call to the accept request web service */
    $.ajax({
        url: "Team/AJAX_AcceptRequest",
        data: p,
        dataType: "json",
        success: function (data) {
            alert(data.message);
            render_myteams();
        },
        error: function () {
            alert(data.message);
        }
    });
}

function action_declinerequest(requestID) {
    var p = { "requestID": requestID };

    /* make the call to the remove request web service */
    $.ajax({
        url: "Team/AJAX_RemoveRequest",
        data: p,
        dataType: "json",
        success: function (data) {
            alert(data.message);
            render_myteams();
        },
        error: function () {
            alert(data.message);
        }
    });
}

function submenu_handler(itemClicked) {
    submenu_clearClasses();
    $("#"+itemClicked).removeClass("inactive-subtab").addClass("active-subtab");
    switch (itemClicked) {
        case "subtab-myteams":
            render_myteams();
            break;
        case "subtab-searchteams":
            $('#inner-content').html(render_searchteams());
            break;
        case "subtab-addteam":
            $('#inner-content').html(render_addteam());
            break;
        case "subtab-inviteuser":
            $('#inner-content').html(render_inviteuser());
            break;
        case "subtab-registercoach":
            $('#inner-content').html(render_registercoach());
            break;
        default:
            $('#inner-content').html("An error seems to have occured. Sorry for the inconvenience.");
            break;
    }
}

function sidebar_listTeams() {
    $.ajax({
        url: "Team/AJAX_GetTeams",
        success: function (data) {
            $('#sidebar-teamlist-content').html(data.message);
        },
        error: function () {
            alert("error making get teams call");
        }
    });
}

function formsubmithandler(e, form) {
    if (e.which == 13) {
        e.preventDefault();
        switch (form) {
            case "search":
                action_searchteams();
                break;
            case "add":
                action_addteam();
                break;
            case "invite":
                action_inviteuser();
                break;
            default:
                break;
        }
    }
}

function submenu_clearClasses() {
    $('#subtab-myteams').removeClass("active-subtab inactive-subtab").addClass("inactive-subtab");
    $('#subtab-searchteams').removeClass("active-subtab inactive-subtab").addClass("inactive-subtab");
    $('#subtab-addteam').removeClass("active-subtab inactive-subtab").addClass("inactive-subtab");
    $('#subtab-inviteuser').removeClass("active-subtab inactive-subtab").addClass("inactive-subtab");
    $('#subtab-registercoach').removeClass("active-subtab inactive-subtab").addClass("inactive-subtab");
}

$(document).ready(function () {
    /* register click listeners with the sub tabs */
    $('#subtab-myteams').click(function () { submenu_handler("subtab-myteams"); });
    $('#subtab-searchteams').click(function () { submenu_handler("subtab-searchteams"); });
    $('#subtab-addteam').click(function () { submenu_handler("subtab-addteam"); });
    $('#subtab-inviteuser').click(function () { submenu_handler("subtab-inviteuser"); });
    $('#subtab-registercoach').click(function () { submenu_handler("subtab-registercoach"); });

    /* set the default sub tab */
    submenu_handler("subtab-myteams");

    /* populate the teams in the sidebar */
    sidebar_listTeams();
})