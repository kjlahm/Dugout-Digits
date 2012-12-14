function show_feedback_form() {
    var formHTML = "<h3>Feedback Message</h3>\n";
    formHTML += "<textarea id='feedback-message' rows='10' cols='27'></textarea>\n";
    formHTML += "<div onClick='submit_feedback()'>Submit</div>\n";
    $('#lightbox-content-feedback').html(formHTML);
    $('#lightbox-black-feedback').css("display", "block");
    $('#lightbox-content-feedback').css("display", "block");
}

function hide_feedback_form() {
    $('#lightbox-black-feedback').css("display", "none");
    $('#lightbox-content-feedback').css("display", "none");
    $('#lightbox-content-feedback').html("");
}

function submit_feedback() {
    /* get the team name from the input field */
    var p = { "message": $('textarea#feedback-message').val() };

    /* make the call to the add team web service */
    $.ajax({
        url: "Home/AJAX_SubmitFeedback",
        data: p,
        dataType: "json",
        success: function (data) {
            hide_feedback_form();
        },
        error: function () {
            alert(data.message);
        }
    });
}