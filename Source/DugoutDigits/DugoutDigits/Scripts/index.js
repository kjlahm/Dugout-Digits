var refreshRequestsIntervalID = 0;
var refreshInvitesIntervalID = 1;
var refreshOpenRequestsIntervalID = 2;
var refreshOpenInvitesIntervalID = 2;

function getCookie(c_name) {
    var i, x, y, ARRcookies = document.cookie.split(";");
    for (i = 0; i < ARRcookies.length; i++) {
        x = ARRcookies[i].substr(0, ARRcookies[i].indexOf("="));
        y = ARRcookies[i].substr(ARRcookies[i].indexOf("=") + 1);
        x = x.replace(/^\s+|\s+$/g, "");
        if (x == c_name) {
            return unescape(y);
        }
    }
}

function render_myteams() {
    $('#inner-content').html("");

    var myteamsHTML = "<div class='tileContent'><h3>My Teams</h3>";
    myteamsHTML += "<div id='team-tables'><img src='./../Content/images/loading.gif' height='47px' width='47px' alt='loading' /></div></div>\n";

    myteamsHTML += "<div class='leftColB'><h3>Requests You've Sent</h3>\n";
    myteamsHTML += "<div id='team-openrequests' class='tileContent'>";
    myteamsHTML += "<img src='./../Content/images/loading.gif' height='47px' width='47px' alt='loading' />";
    myteamsHTML += "</div><div id='openrequestresponse' class='tileContent'></div>";
    myteamsHTML += "</div>\n";
    myteamsHTML += "<div class='rightColB'><h3>Pending Invites</h3>";
    myteamsHTML += "<div id='team-invites' class='tileContent'><img src='./../Content/images/loading.gif' height='47px' width='47px' alt='loading' /></div><div id='inviteresponse' class='tileContent'></div>";
    myteamsHTML += "</div>\n";
    myteamsHTML += "<div class='clear'></div>";

    var coachEnabled = getCookie("DD_coachEnabled");
    if (coachEnabled != null && coachEnabled == "true") {
        myteamsHTML += "<div class='leftColB'><h3>Pending Requests</h3>";
        myteamsHTML += "<div id='team-requests' class='tileContent'><img src='./../Content/images/loading.gif' height='47px' width='47px' alt='loading' /></div><div id='requestresponse' class='tileContent'></div>";
        myteamsHTML += "</div>\n";
        myteamsHTML += "<div class='rightColB'><h3>Invites You've Sent</h3>";
        myteamsHTML += "<div id='team-openinvites' class='tileContent'><img src='./../Content/images/loading.gif' height='47px' width='47px' alt='loading' /></div><div id='openinviteresponse' class='tileContent'></div>";
        myteamsHTML += "</div>\n";
        myteamsHTML += "<div class='clear'></div>";
    }

    $('#content-buffer').html(myteamsHTML);
    load_myteams();
    load_myrequests();
    load_myinvites();
    load_myopenrequests();
    load_myopeninvites();
    refreshRequestsIntervalID = setInterval('load_myrequests()', 10000);
    refreshInvitesIntervalID = setInterval('load_myinvites()', 10000);
    refreshOpenRequestsIntervalID = setInterval('load_myopenrequests()', 10000);
    refreshOpenInvitesIntervalID = setInterval('load_myopeninvites()', 10000);

    // Grow the content section to the right size
    var newHeight = $('#content-buffer').height();
    $('#inner-content').animate({ height: newHeight-30 }, 500, "linear", function () {
        $('#inner-content').html($('#content-buffer').html()).hide().fadeIn();
    });
}

function load_myteams() {
    $.ajax({
        url: "Team/AJAX_GetTeamsTable",
        success: function (data) {
            $('#team-tables').html(data.message).hide().fadeIn();
        },
        error: function () {
            alert("error making get teams table call");
        }
    });
}

function load_myrequests() {
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

function load_myinvites() {
    $.ajax({
        url: "Team/AJAX_GetInviteTable",
        success: function (data) {
            $('#team-invites').html(data.message);
        },
        error: function () {
            alert("error making get requests table call");
        }
    });
}

function load_myopenrequests() {
    $.ajax({
        url: "Team/AJAX_GetOpenRequestTable",
        success: function (data) {
            $('#team-openrequests').html(data.message);
        },
        error: function () {
            alert("error making get requests table call");
        }
    });
}

function load_myopeninvites() {
    $.ajax({
        url: "Team/AJAX_GetOpenInviteTable",
        success: function (data) {
            $('#team-openinvites').html(data.message);
        },
        error: function () {
            alert("error making get requests table call");
        }
    });
}

function render_searchteams() {
    $('#inner-content').html("");

    var returnVal = "<h3>Search For a Team</h3>\n";
    returnVal += "<p>Use the form to search for a team by name.</p>\n";
    returnVal += "<form>\n<h4>Team Name</h4>\n";
    returnVal += "<input class='editor-field' type='text' name='search-team-name' onkeypress='formsubmithandler(event, \"search\")' />\n";
    returnVal += "<div class='submit-button' onClick='action_searchteams()'>Search</div>\n</form>\n";
    returnVal += "<div id='searchteams-success-message'></div><div id='searchteams-request-message'></div>";
    $('#content-buffer').html(returnVal);

    // Grow the content section to the right size
    var newHeight = $('#content-buffer').height();
    $('#inner-content').animate({ height: newHeight + 30 }, 500, "linear", function () {
        $('#inner-content').html($('#content-buffer').html()).hide().fadeIn();
    });
}

function render_addteam() {
    $('#inner-content').html("");

    var returnVal = "<h3>Add New Team</h3>\n";
    returnVal += "<p><strong>Note:</strong> If you are not the coach, have the coach create the team. The coach can then ";
    returnVal += "invite players to join the team or players can search for and request to join the team.</p>\n";
    returnVal += "<form>\n<h4>Team Name</h4>\n";
    returnVal += "<input class='editor-field' type='text' name='team-name' onkeypress='formsubmithandler(event, \"add\")' />\n";
    returnVal += "<div class='submit-button' onClick='action_addteam()'>Add Team</div>\n</form>\n";
    returnVal += "<div id='addteam-success-message'></div>";
    $('#content-buffer').html(returnVal);

    // Grow the content section to the right size
    var newHeight = $('#content-buffer').height();
    $('#inner-content').animate({ height: newHeight + 30 }, 500, "linear", function () {
        $('#inner-content').html($('#content-buffer').html()).hide().fadeIn();
    });
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
            returnVal += "Digits they will be able to receive messages from you about game and practice information as ";
            returnVal += "well as browse through your team's data including season stats and game scorecards.</p>\n";
            returnVal += "<form>\n<h4>Email</h4>\n";
            returnVal += "<input class='editor-field' type='text' name='invite-email' />\n";
            returnVal += "<h4>Message</h4>\n";
            /*returnVal += "<input class='editor-field' type='text' name='invite-message' onkeypress='formsubmithandler(event, \"invite\")' />\n";*/

            returnVal += "<textarea id='invite-message' class='editor-field' rows='10' cols='27' onkeypress='formsubmithandler(event, \"invite\")'></textarea>\n";

            returnVal += "<h4>Invite To Join</h4>\n";
            returnVal += data.message;
            returnVal += "<div class='submit-button' onClick='action_inviteuser()'>Send an Invite</div>\n</form>\n";
            returnVal += "<div id='inviteuser-success-message'></div>";
            $('#content-buffer').html(returnVal);

            // Grow the content section to the right size
            var newHeight = $('#content-buffer').height();
            $('#inner-content').animate({ height: newHeight+50 }, 500, "linear", function () {
                $('#innercontent-inviteuser').html($('#content-buffer').html()).hide().fadeIn();
            });
        },
        error: function () {
            $('#innercontent-inviteuser').html("<p>Error building invitation form.</p>");
        }
    });

    return "<div id='innercontent-inviteuser'></div>";
}

function render_registercoach() {
    $('#inner-content').html("");

    var returnVal = "<h3>Interested in Joining?</h3>\n";
    returnVal += "<p>If you'd like to use Dugout Digits to help manage your team ";
    returnVal += "<div class='submit-button' onClick='action_requestcoach()'>Click Here</div></p>\n";
    returnVal += "<p>If you don't hear back in a weeks time send an email to <a href='mailto:admin@dugoutdigits.com'>admin@dugoutdigits.com</a>.</p>\n";
    returnVal += "<div id='requestcoach-success-message'></div>";
    $('#content-buffer').html(returnVal);

    // Grow the content section to the right size
    var newHeight = $('#content-buffer').height();
    $('#inner-content').animate({ height: newHeight + 30 }, 500, "linear", function () {
        $('#inner-content').html($('#content-buffer').html()).hide().fadeIn();
        $('#content-buffer').html("");
    });
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

function action_removeteam(teamID) {
    /* put the teamID in an array to send to the server */
    var p = { "teamID": teamID };

    /* make the call to the add team web service */
    $.ajax({
        url: "Team/AJAX_RemoveTeam",
        data: p,
        dataType: "json",
        success: function (data) {
            sidebar_listTeams();
            load_myteams();
        },
        error: function () {
            alert("error making remove team call");
        }
    });

    action_hidedetails();
}

function action_detailsremoveteam(teamID) {
    var lightboxText = "<div class='lightbox-content-close clickable-text' onClick='action_hidedetails()'>Close</div>\n";
    lightboxText += "<h3>Are you sure?</h3>\n";
    lightboxText += "<p>I know confirmation prompts are annoying but this action cannot be undone.</p>\n";
    lightboxText += "<div class='submit-button' onClick='action_removeteam(" + teamID + ")'>Remove</div>\n";

    $('#lightbox-content-index').html(lightboxText);
    $('#lightbox-black-index').css("display", "block");
    $('#lightbox-content-index').css("display", "block");
}

function action_inviteuser() {
    $('#inviteuser-success-message').html("");

    /* get the team name from the input field */
    var p = { "inviteEmail": $('input[name="invite-email"]').val(), "inviteMessage": $('textarea#invite-message').val(), "teamID": $('#teamdropdown').val() };

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

function action_requestcoach() {
    /* make the call to the invite user web service */
    $.ajax({
        url: "Admin/AddCoachRequest",
        dataType: "json",
        success: function (data) {
            $('#requestcoach-success-message').html(data.message);
        },
        error: function () {
            $('#requestcoach-success-message').html("Error requesting coach access.");
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
            $('#searchteams-request-message').html(data.message);
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
            /*$('#requestresponse').html(data.message);*/
            load_myrequests();
            action_hidedetails();
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
            /*$('#requestresponse').html(data.message);*/
            load_myrequests();
            action_hidedetails();
        },
        error: function () {
            alert(data.message);
        }
    });
}

function action_detailsrequest(requestID) {
    var p = { "requestID": requestID };

    /* make the call to the remove request web service */
    $.ajax({
        url: "Team/AJAX_GetRequest",
        data: p,
        dataType: "json",
        success: function (data) {
            $('#lightbox-content-index').html(data.message);
        },
        error: function () {
            alert(data.message);
        }
    });

    $('#lightbox-black-index').css("display", "block");
    $('#lightbox-content-index').css("display", "block");
}

function action_removerequest(requestID) {
    var p = { "requestID": requestID };

    /* make the call to the remove request web service */
    $.ajax({
        url: "Team/AJAX_RemoveRequest",
        data: p,
        dataType: "json",
        success: function (data) {
            load_myopenrequests();
        },
        error: function () {
            alert(data.message);
        }
    });
}

function action_acceptinvite(inviteID) {
    var p = { "inviteID": inviteID };

    /* make the call to the accept invite web service */
    $.ajax({
        url: "Team/AJAX_AcceptInvite",
        data: p,
        dataType: "json",
        success: function (data) {
            /*$('#inviteresponse').html(data.message);*/
            load_myinvites();
            load_myteams();
            sidebar_listTeams();
            action_hidedetails();
        },
        error: function () {
            alert(data.message);
        }
    });
}

function action_declineinvite(inviteID) {
    var p = { "inviteID": inviteID };

    /* make the call to the remove invite web service */
    $.ajax({
        url: "Team/AJAX_RemoveInvite",
        data: p,
        dataType: "json",
        success: function (data) {
            /*$('#inviteresponse').html(data.message);*/
            load_myinvites();
            action_hidedetails();
        },
        error: function () {
            alert(data.message);
        }
    });
}

function action_detailsinvite(inviteID) {
    var p = { "inviteID": inviteID };

    /* make the call to the invite details web service */
    $.ajax({
        url: "Team/AJAX_GetInvite",
        data: p,
        dataType: "json",
        success: function (data) {
            $('#lightbox-content-index').html(data.message);
        },
        error: function () {
            alert(data.message);
        }
    });

    $('#lightbox-black-index').css("display", "block");
    $('#lightbox-content-index').css("display", "block");
}

function action_removeinvite(inviteID) {
    var p = { "inviteID": inviteID };

    /* make the call to the remove invite web service */
    $.ajax({
        url: "Team/AJAX_RemoveInvite",
        data: p,
        dataType: "json",
        success: function (data) {
            load_myopeninvites();
        },
        error: function () {
            alert(data.message);
        }
    });
}

function action_hidedetails() {
    $('#lightbox-black-index').css("display", "none");
    $('#lightbox-content-index').css("display", "none");
    $('#lightbox-content-index').html("");
}

function action_leaveteam(teamID) {
    var p = { "teamID": teamID };

    /* make the call to the add request web service */
    $.ajax({
        url: "Team/AJAX_LeaveTeam",
        data: p,
        dataType: "json",
        success: function (data) {
            load_myteams();
            sidebar_listTeams();
        },
        error: function () {
            alert(data.message);
        }
    });
}

function submenu_handler(itemClicked) {
    clearInterval(refreshRequestsIntervalID);
    clearInterval(refreshInvitesIntervalID);
    clearInterval(refreshOpenRequestsIntervalID);
    clearInterval(refreshOpenInvitesIntervalID);
    submenu_clearClasses();
    $("#"+itemClicked).removeClass("inactive-subtab").addClass("active-subtab");
    switch (itemClicked) {
        case "subtab-myteams":
            render_myteams();
            break;
        case "subtab-searchteams":
            render_searchteams();
            break;
        case "subtab-addteam":
            render_addteam();
            break;
        case "subtab-inviteuser":
            $('#inner-content').html(render_inviteuser());
            break;
        case "subtab-registercoach":
            render_registercoach();
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

function action_gototeam(teamID) {
    window.location = "/Team/Overview?teamID="+teamID;
}