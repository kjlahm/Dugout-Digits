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

$(document).ready(function () {
    /* populate the teams in the sidebar */
    sidebar_listTeams();
})