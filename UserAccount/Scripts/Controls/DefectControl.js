function reloadDefectCell(eventTypeId, url) {
    var data = {
        eventTypeId: eventTypeId
    };
    var onSuccess = function (responseData) {
        $('#defect-control-' + eventTypeId).replaceWith(responseData.html);
        HideModal();
    }
    ziExecuteJsonUrl(url, data, onSuccess);
}

// данную функцию вызывает диалог после успешного выполнения своего post-а
var changeStatusDialogCallback = null;

function beginChangeDefectStatus(elem, event, url) {
    elem = $(elem);
    var eventTypeId = elem.data("event-type-id");
    changeStatusDialogCallback = function () {
        reloadDefectCell(eventTypeId, url);
    }
    ziShowModalDialogByLink(elem, event);
}

function beginCreateAndCloseDefect(elem, event, eventTypeId, actionUrl, dialogUrl ) {
    event.preventDefault();
    elem = $(elem);
    var data = {
        eventTypeId: eventTypeId
    };
    var onSuccess = function () {
        reloadDefectCell(eventTypeId, dialogUrl);
    }
    ziWaitAjaxRequest(elem, actionUrl, data, onSuccess);
}
