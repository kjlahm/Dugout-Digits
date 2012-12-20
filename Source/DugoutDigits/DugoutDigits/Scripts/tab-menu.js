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
            window.location = "/Team/Statistics?teamID=" + teamID;
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

$(document).ready(function () {
    /* Put the current team in the dropdown div */
    $('#dropdown-content').html($('#dropdown-default-team').html());

    /* On hover show the teams */
    $('#dropdown-content').hover(
    function () {
        $('#dropdown-content').html($('#dropdown-content-buffer').html());
    },
    function () {
        $('#dropdown-content').html($('#dropdown-default-team').html());
    });
})