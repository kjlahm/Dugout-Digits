function menu_handler(itemClicked, teamID) {
    switch (itemClicked) {
        case "tab-overview":
            window.location = "/Team/Overview?teamID=" + teamID;
            break;
        case "tab-schedule":
            window.location = "/Team/Schedule?teamID=" + teamID;
            break;
        case "tab-roster":
            window.location = "/Team/Roster?teamID=" + teamID;
            break;
        case "tab-statistics":
            window.location = "/Team/Stats?teamID=" + teamID;
            break;
        case "tab-messages":
            window.location = "/Team/Messages?teamID=" + teamID;
            break;
        default:
            break;
    }
}

function action_gototeam(teamID) {
    window.location = "/Team/Overview?teamID=" + teamID;
}

function load_dropdown(teamID) {
    var p = { "teamID": teamID };

    $.ajax({
        url: "/Team/AJAX_GetTeams2",
        data: p,
        dataType: "json",
        success: function (data) {
            $('#dropdown-content-buffer').html(data.message);
        }
    });
}

function expand_dropdown() {
    $('#dropdown-content').html($('#dropdown-content-buffer').html());
    $('#dropdown-content li').css('border-style', 'solid').css('margin-bottom', '-3px').css('margin-top', '-3px');
    $('#dropdown').css('margin-left', '0').css('margin-right', '-3px');
}

function collapse_dropdown() {
    $('#dropdown-content').html($('#dropdown-default-team').html());
    $('#dropdown-content li').css('border-style', 'none').css('margin-bottom', '3px').css('margin-top', '0');
    $('#dropdown').css('margin-left', '3px').css('margin-right', '0');
}

$(document).ready(function () {
    /* Put the current team in the dropdown div */
    $('#dropdown-content').html($('#dropdown-default-team').html());

    /* On hover show the teams */
    $('#dropdown-content').hover(
    function () {
        expand_dropdown();
    },
    function () {
        collapse_dropdown();
    });
})