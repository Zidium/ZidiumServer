var smartBlocks = (function () {

    var unknownError = 'Произошла неизвестная ошибка';

    var globalErrorCallback = null;

    function onFormSubmit(event) {
        event.preventDefault();
        var form = $(this);

        if (!form.valid()) {
            return false;
        }

        var url = form.attr('action');
        var method = form.attr('method');
        var isPost = method != null && method.toLowerCase() === "post";
        var data = isPost ? new FormData(form[0]) : (collectFormData(form) || form.serializeArray());

        doAjaxSubmit(url, method, data, form);

        return false;
    }

    function collectFormData(form) {
        var collectFunctionName = form.data('smart-block-collect-data');
        if (collectFunctionName != null) {
            var collectFunction = getFunction(collectFunctionName);
            if (collectFunction != null) {
                return collectFunction.call(form, form);
            }
        }
        return null;
    }

    function doAjaxSubmit(url, method, data, element, onComplete) {
        var isPost = method != null && method.toLowerCase() === "post";

        if (data != null && data.constructor.name !== "FormData" && isPost)
            data = objectToFormData(data);

        // если параметры не указаны явно, то попробуем выполним поиск данных в клиентском хранилище
        if (!data) {
            // zem - не знаю все ли данные тут всегда будут, поэтому проверим все на null, чтобы случайно ничего не сломать
            if (element) {
                var config = getConfig(element);
                if (config) {
                    data = config.getData();
                }
            }
        }

        $.ajax({
            url: url,
            method: method,
            data: data,
            success: function (response) { onAjaxSuccessWrapper(response, element); },
            error: function (jqXhr, textStatus, errorThrown) { onAjaxFailureWrapper(jqXhr, textStatus, errorThrown, element); },
            beforeSend: function () { onBeforeSendWrapper(data, element, isPost); },
            complete: onComplete,
            headers: { 'SmartBlocksRequest': 'true' },
            processData: !isPost,
            contentType: !isPost
        });
    }

    function objectToFormData(obj) {
        var formData = new FormData();
        $.each(obj, function (key, value) {
            formData.append(key, value);
        });
        return formData;
    }

    function onAjaxCompleteWrapper(element) {
        var container = getContainer(element);
        var completeFunctionName = container.data('smart-block-complete');
        if (completeFunctionName != null) {
            var completeFunction = getFunction(completeFunctionName);
            if (completeFunction != null)
                completeFunction();
        }
    }

    function onAjaxSuccessWrapper(response, element) {
        completeSubmit(element);

        // If response is not an object, view is returned.
        // Otherwise, it's a json result
        if (!(response instanceof Object)) {
            processView(response, element);
        } else {
            processJson(response, element);
        }

        onAjaxCompleteWrapper(element);
    }

    function processView(html, element) {
        var container = getContainer(element);
        if (container != null) {
            var mode = (container.data('smart-block-mode') || 'insert').toLowerCase();

            if (mode === 'insert')
                container.html(html);
            else
                container.replaceWith(html);

            // Revalidate all forms inside loaded html
            var forms = $('form', container);
            $.each(forms, function (index, elm) {
                var form = $(elm);
                form.removeData('validator');
                form.removeData('unobtrusiveValidation');
                $.validator.unobtrusive.parse(form);
            });
        }
    }

    function processJson(json, element) {
        var actionFunction = null;
        var container = getContainer(element);
        var actionFunctionName = container.data('smart-block-action');
        if (actionFunctionName != null) {
            actionFunction = getFunction(actionFunctionName);
        }
        processJsonWithAction(json, actionFunction, element);
    }

    function processJsonWithAction(json, callback, element, onError) {
        // Если результат не json, то вернём результат как есть
        if (!(json instanceof Object)) {
            if (callback != null)
                callback(json);
            return;
        }

        if (json.success) {
            if (callback != null)
                callback(json.data);
        } else {
            var error = json.error;
            if (!error) {
                error = unknownError;
            }
            showError(error, element, onError);
        };
    }

    function onAjaxFailureWrapper(jqXhr, textStatus, errorThrown, element) {
        completeSubmit(element);
        onAjaxFailure(jqXhr, textStatus, errorThrown, element, null);
        onAjaxCompleteWrapper(element);
    }

    function onAjaxFailure(jqXhr, textStatus, errorThrown, element, onErrorCallback) {
        // Если запрос отменён из браузера, просто ничего не делаем
        if (textStatus === "abort")
            return;

        var errorMessage = jqXhr.status + ": " + textStatus + ": " + errorThrown;
        showError(errorMessage, element, onErrorCallback);
    }

    function showError(message, element, onErrorCallback) {
        if (onErrorCallback != null) {
            onErrorCallback(message);
            return;
        }

        if (element != null) {
            var container = getContainer(element);
            var errorFunctionName = container.data('smart-block-on-error');
            if (errorFunctionName != null) {
                var errorFunction = getFunction(errorFunctionName);
                errorFunction(message);
                return;
            }
        }

        if (globalErrorCallback != null)
            globalErrorCallback(message);
        else
            console.error(message);
    }

    function getContainer(element) {
        var containerId = element.data('smart-block-container');
        if (containerId != null)
            return $('#' + containerId);
        return element.closest('.smart-block');
    }

    function getLoader(element) {
        var loaderId = element.data('smart-block-loader');
        return $('#' + loaderId);
    }

    function beginSubmit(element) {
        getLoader(element).show();

        var initiator = getSubmitInitiator(getContainer(element));
        if (initiator != null && initiator instanceof Object) {
            smartButtons.beginAction(initiator);
        }
    }

    function completeSubmit(element) {
        getLoader(element).hide();

        var container = getContainer(element);
        var initiator = getSubmitInitiator(container);
        if (initiator != null && initiator instanceof Object) {
            smartButtons.endAction(initiator);
            container.data('initiator', '');
        }
    }

    function getFunction(name) {
        if (typeof (name) === "function")
            return name;

        var fn = window, parts = (name || "").split(".");
        while (fn && parts.length) {
            fn = fn[parts.shift()];
        }
        if (typeof (fn) === "function") {
            return fn;
        }
        return null;
    }

    function getSubmitInitiator(container) {
        return container.data('initiator');
    }

    function onBeforeSendWrapper(data, element, isPost) {
        beginSubmit(element);

        if (!isPost) {
            var container = getContainer(element);
            var beginFunctionName = container.data('smart-block-begin');
            if (beginFunctionName != null) {
                var beginFunction = getFunction(beginFunctionName);
                if (beginFunction != null) {
                    data = ensureArray(data);
                    var params = data.filter(function (param) {
                        return param.value != null && param.value !== "";
                    });
                    beginFunction.call(container, $.param(params));
                }
            }
        }
    }

    function ensureArray(data) {
        if (Array.isArray(data))
            return data;

        return $.map(data,
            function (value, index) {
                return { name: index, value: value };
            });
    }

    function setGlobalErrorCallback(newErrorCallback) {
        globalErrorCallback = newErrorCallback;
    }

    function onSmartButtonClick() {
        var button = $(this);
        var form = button.closest('.smart-block-form');
        var container = getContainer(form);
        if (container != null)
            container.data('initiator', button);
    }

    function doAjax(url, method, data, onAction, onComplete, onError) {
        return $.ajax({
            url: url,
            method: method,
            data: data,
            success: function (response) { processJsonWithAction(response, onAction, null, onError); },
            error: function (jqXhr, textStatus, errorThrown) { onAjaxFailure(jqXhr, textStatus, errorThrown, null, onError) },
            complete: onComplete,
            headers: { 'SmartBlocksRequest': 'true' }
        });
    }

    function getFormDataForPost(form) {
        var formData = {};
        form.serializeArray().map(function (x) { formData[x.name] = x.value; });
        return formData;
    }

    function doContainerAction(element, success, data, error) {
        var json = {
            success: success,
            error: error,
            data: data
        };

        processJson(json, element);
    }

    function loadSmartBlock(params) {
        var elements = params.elements;
        var method = params.method;
        var onComplete = params.onComplete;
        elements.each(function (index, elem) {
            var element = $(elem);
            var url = params.url || element.data("url");
            if (url) {
                if (!onComplete) {
                    onComplete = function () { };
                }
                method = method || "get";
                doSubmit(url, null, element, onComplete, method);
            }
        });
    }

    function doSubmit(url, data, element, onComplete, method) {
        doAjaxSubmit(url, method || 'get', data, element, onComplete);
    }

    function getConfig(element) {
        var container = getContainer(element);
        if (container) {
            var name = container.data("config");
            if (name) {
                return smartConfig.get(name);
            }
        }
        return null;
    }

    var waitDivId = 0;

    function showWaitDiv(elem) {
        elem = $(elem);
        var lastWaitDivId = elem.data("smart-wait-div-id");
        if (lastWaitDivId) {
            $("#" + lastWaitDivId).remove();
        }
        var width = elem.outerWidth(true);
        var height = elem.outerHeight(true);
        var top = elem.offset().top;
        var left = elem.offset().left;

        var style = "left:" + left + "px;"
            + "top:" + top + "px;"
            + "height:" + height + "px;"
            + "width:" + width + "px;";

        var id = "smart-wait-div-" + (waitDivId++);
        var waitDivHtml = "<div id='" + id + "' class='smart-wait-div' style='" + style + "'></div>";
        elem.append(waitDivHtml);
        //elem.append("<p>Test</p>");
        elem.data("smart-wait-div-id", id);
        return id;
    }

    return {
        onFormSubmit: onFormSubmit,
        setGlobalErrorCallback: setGlobalErrorCallback,
        doSubmit: doSubmit,
        doAjax: doAjax,
        onSmartButtonClick: onSmartButtonClick,
        getFormDataForPost: getFormDataForPost,
        doContainerAction: doContainerAction,
        load: loadSmartBlock,
        showWaitDiv: showWaitDiv
    };
})();

$(function () {
    $(document).on('submit', 'form.smart-block-form', smartBlocks.onFormSubmit);

    // Save submit initiator in form data for use later in onSubmit event
    $(document).on('click', 'button[type=submit].smart-block-button', smartBlocks.onSmartButtonClick);

    $.extend($.fn,
        {
            smartBlocks: function () {
                if (!this.length)
                    return;

                var me = $(this);

                if (me.hasClass('smart-block-auto-load')) {
                    var url = me.data('url');
                    smartBlocks.doSubmit(url, null, me);
                }

                $('.smart-block-auto-load', me).each(function (index, element) {
                    var url = $(element).data('url');
                    smartBlocks.doSubmit(url, null, $(element));
                });

                if (me.hasClass('smart-block-auto-refresh')) {
                    var url = me.data('url');
                    var interval = parseInt(me.data('interval'));
                    setInterval(function () {
                        smartBlocks.doSubmit(url, null, me);
                    }, interval * 1000);
                }

                $('.smart-block-auto-refresh', me).each(function (index, element) {
                    var url = $(element).data('url');
                    var interval = parseInt($(element).data('interval'));
                    setInterval(function () {
                        smartBlocks.doSubmit(url, null, $(element));
                    }, interval * 1000);
                });

            }
        });

});