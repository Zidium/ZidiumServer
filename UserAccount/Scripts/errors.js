$(function () {
    $('#button-close-error').click(function () {
        $('#panel-error').hide(500);
    });
});

function showError(error, details, number) {
    var panel = $('#panel-error');
    $('#error-message', panel).text(error);
    $('#error-details', panel).text(details);
    $('#error-number', panel).text(number);
    panel.show(500);
}

function hideError() {
    var panel = $('#panel-error');
    panel.hide(500);
}

function onAjaxFailure(jqXhr, textStatus, errorThrown) {
    showError(textStatus, errorThrown);
}

function onAjaxSuccess() {
    hideError();
}