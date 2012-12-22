function load_teammembers(teamID) {
    /* get the team name from the input field */
    var data = { "teamID": teamID };

    /* make the call to the add team web service */
    $.ajax({
        url: "/Team/AJAX_GetTeamMembers",
        data: data,
        dataType: "json",
        success: function (data) {
            $('#sidebar-playerlist-content').html(data.message);
        },
        error: function () {
            $('#sidebar-playerlist-content').html("Error getting the team's players.");
        }
    });
}

function load_teamcoaches(teamID) {
    /* get the team name from the input field */
    var data = { "teamID": teamID };

    /* make the call to the add team web service */
    $.ajax({
        url: "/Team/AJAX_GetTeamCoaches",
        data: data,
        dataType: "json",
        success: function (data) {
            $('#sidebar-coachlist-content').html(data.message);
        },
        error: function () {
            $('#sidebar-playerlist-content').html("Error getting the team's coaches.");
        }
    });
}