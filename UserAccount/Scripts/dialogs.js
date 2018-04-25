var dialogs = (function () {

    function showModal(url, title, onSuccess, data, isLarge) {
        var dialogId = "dialog_" + utils.guid();

        var dialog = bootbox.dialog({
            title: title,
            message: '<div id="' + dialogId + '"></div>',
            onEscape: true,
            size: isLarge === true ? 'large' : null
        });

        dialog.init(function () {
            var dialogDiv = $('#' + dialogId);

            var errorDiv = $('<div class="collapse alert alert-danger"></div>');
            var errorButton = $('<button type="button" class="close" id="dialog-error-hide-button"><span>&times;</span></button>');
            errorDiv.append(errorButton);
            var errorText = $('<p id="dialog-error-text"></p>');
            errorDiv.append(errorText);
            dialogDiv.append(errorDiv);

            var containerDiv = $('<div class="smart-block" id="dialog-container"></div>');
            var loader = $('<div class="loader-container"><img id="dialog-loader" class="loader-horizontal" src="/Content/Icons/ajax-loader.gif" /></div>');
            containerDiv.append(loader);
            dialogDiv.append(containerDiv);

            containerDiv.data('smart-block-on-error', function (message) {
                errorText.text(message);
                errorDiv.show();
            });

            containerDiv.data('smart-block-action', function (data) {
                dialog.modal('hide');
                if (onSuccess != null)
                    onSuccess(data);
            });

            containerDiv.data('smart-block-begin', function () {
                errorDiv.hide();
            });

            errorButton.click(function () {
                errorDiv.hide();
            });

            smartBlocks.doSubmit(url, data, containerDiv);
        });

    }

    function confirmDelete(callback, data) {
        if (!callback) {
            callback = function () { };
        }
        bootbox.dialog({
            title: "Подтвердите удаление",
            message: "<p class='alert alert-danger' style='margin-bottom: 0;'>Действительно хотите удалить этот объект?</p>",
            buttons: {
                yes: {
                    label: "Удалить",
                    className: "btn-danger",
                    callback: function () {
                        callback(data);
                    }
                },
                no: {
                    label: "Закрыть",
                    className: "btn-default"
                }
            },
            onEscape: true
        }
        );
    }

    function error(message, callback) {
        bootbox.dialog({
            title: "Ошибка",
            message: "<p class='alert alert-danger' style='margin-bottom: 0;'>" + message + "</p>",
            buttons: {
                ok: {
                    label: "Закрыть",
                    className: "btn-default",
                    callback: callback
                }
            },
            onEscape: true
        }
        );
    }

    var commonDialogs = (function () {

        // диалог удаления подписки
        var deleteSubscription = function (id, callback) {
            var title = "Удаление подписки";
            var url = "/Subscriptions/Delete/" + id;
            dialogs.showModal(url, title, callback);
        };
        return {
            deleteSubscription: deleteSubscription // // диалог удаления подписки
        }
    })();

    return {
        showModal: showModal,
        confirmDelete: confirmDelete,
        error: error,
        common : commonDialogs // коллекция общий диалогов
    };
})();