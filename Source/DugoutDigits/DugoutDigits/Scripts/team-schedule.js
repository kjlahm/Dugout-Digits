function render_viewschedule() {
    var innerHTML = "schedule subtab";
    $('#inner-content').html(innerHTML);
}

function render_addseason() {
    var innerHTML = "<div class='leftColB'><h3>Current Seasons</h3>";
    innerHTML += "<div id='current-seasons'></div></div>";
    innerHTML += "<div class='rightColB'><h3>Add A Season</h3>";
    innerHTML += "<div id='add-season-form'></div></div>";
    innerHTML += "<div class='clear'></div>";
    $('#inner-content').html(innerHTML);

    var data = { "teamID": teamID };

    load_currentseasons();

    $.ajax({
        url: "/Team/AJAX_RenderAddSeasonForm",
        data: data,
        dataType: "json",
        success: function (data) {
            $('#add-season-form').html(data.message);
        },
        error: function () {
            $('#add-season-form').html("Error getting the add season form.");
        }
    });
}

function render_addgame() {
    var innerHTML = "<h3>Add New Game</h3>";
    innerHTML += "<div id='add-game-form'></div>";
    $('#inner-content').html(innerHTML);

    var data = { "teamID": teamID };

    $.ajax({
        url: "/Team/AJAX_RenderAddGameForm",
        data: data,
        dataType: "json",
        success: function (data) {
            $('#add-game-form').html(data.message);
        },
        error: function () {
            $('#add-game-form').html("Error getting the add game form.");
        }
    });
}

function render_addpractice() {
    var innerHTML = "<h3>Add New Practice</h3>";
    innerHTML += "<div id='add-practice-form'></div>";
    $('#inner-content').html(innerHTML);

    var data = { "teamID": teamID };

    $.ajax({
        url: "/Team/AJAX_RenderAddPracticeForm",
        data: data,
        dataType: "json",
        success: function (data) {
            $('#add-practice-form').html(data.message);
        },
        error: function () {
            $('#add-practice-form').html("Error getting the add practice form.");
        }
    });
}

function action_addseason() {
    var data = { "teamID": teamID, "season": $('#yearpicker').val() };

    $.ajax({
        url: "/Team/AJAX_AddSeason",
        data: data,
        dataType: "json",
        success: function (data) {
            $('#add-season-feedback').html(data.message);
            load_currentseasons();
        }
    });
}

function action_addgame() {
    var data = {
        "teamID": teamID,
        "opponent": $('input[name="add-game-opponent"]').val(),
        "homeOrAway": isHome,
        "location": $('input[name="add-game-location"]').val(),
        "date": $('input[name="add-game-date"]').val(),
        "time": $('input[name="add-game-time"]').val(),
        "seasonID": $('#add-game-season').val()
    };

    $.ajax({
        url: "/Team/AJAX_AddGame",
        data: data,
        dataType: "json",
        success: function (data) {
            $('#add-game-feedback').html(data.message);
        }
    });
}

function action_addpractice() {
    var data = {
        "teamID": teamID,
        "location": $('input[name="add-practice-location"]').val(),
        "date": $('input[name="add-practice-date"]').val(),
        "time": $('input[name="add-practice-time"]').val(),
        "seasonID": $('#add-practice-season').val()
    };

    $.ajax({
        url: "/Team/AJAX_AddPractice",
        data: data,
        dataType: "json",
        success: function (data) {
            $('#add-practice-feedback').html(data.message);
        }
    });
}

function load_currentseasons() {
    var data = { "teamID": teamID };

    $.ajax({
        url: "/Team/AJAX_GetSeasons",
        data: data,
        dataType: "json",
        success: function (data) {
            $('#current-seasons').html(data.message);
        }
    });
}

function submenu_clearClasses() {
    $('#subtab-viewschedule').removeClass("active-subtab inactive-subtab").addClass("inactive-subtab");
    $('#subtab-addseason').removeClass("active-subtab inactive-subtab").addClass("inactive-subtab");
    $('#subtab-addgame').removeClass("active-subtab inactive-subtab").addClass("inactive-subtab");
    $('#subtab-addpractice').removeClass("active-subtab inactive-subtab").addClass("inactive-subtab");
}

function submenu_handler(itemClicked, teamID) {
    submenu_clearClasses();
    $("#" + itemClicked).removeClass("inactive-subtab").addClass("active-subtab");
    switch (itemClicked) {
        case "subtab-viewschedule":
            render_viewschedule();
            break;
        case "subtab-addseason":
            render_addseason();
            break;
        case "subtab-addgame":
            render_addgame();
            break;
        case "subtab-addpractice":
            render_addpractice();
            break;
        default:
            $('#inner-content').html("An error seems to have occured. Sorry for the inconvenience.");
            break;
    }
}