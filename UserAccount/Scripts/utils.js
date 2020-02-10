$(function () {
    ziBeginHandleWindowErrors();

    $('.btn-modal').each(function (index, element) {
        $(element).click(function (event) {
            event.preventDefault();
            var url = $(this).data('href');
            HideModal();
            UrlShowModal(url);
        });
    });

    setPostLink('.post-link');
    jQuery.validator.methods["date"] = function (value, element) { return true; }
});


function initTooltips(context) {

    if (!context) {
        context = $(document);
    }

    // добавляем иконку
    $(".ziTooltipContent", context).before('<a href="#" class="helpIcon" onclick="return false"><img src="/Content/Icons/Round_help_button_20.png" /></a>');

    // добавляем тултип
    $('.helpIcon', context).tooltipster({
        arrow: true,
        //theme: 'tooltipster-light',
        contentAsHTML: true,
        interactive: true, // чтобы можно было кликать ссылки внутри тултипа
        functionInit: function (origin, content) {
            var html = origin.next(".ziTooltipContent").html();
            return "<div class='ziTooltipWrapper'>" + html + "</div>";
        }
    });
}

function submitPost(url) {
    if (!url) {
        url = "";
    }
    var form = $('<form></form>');
    form.attr("action", url);
    form.attr("method", "POST");
    form.appendTo('body').submit();
}

// превращает ссылку в кнопку submit. При клике выполняется submit методом post по адресу href
function setPostLink(selector) {
    $(selector).each(function (index, elem) {
        var link = $(elem);
        var href = link.attr("href");
        link.click(function () {
            submitPost(href);
            return false;
        });
    });
}

function SetQueryParam(queryStr, param, val) {
    var res = '';
    var d = queryStr.split("?");
    var base = d[0];
    var query = d[1];
    if (query) {
        var params = query.split("&");
        for (var i = 0; i < params.length; i++) {
            var keyval = params[i].split("=");
            if (keyval[0] && keyval[0].toUpperCase() != param.toUpperCase()) {
                if (res != '')
                    res += '&';
                res += params[i];
            }
        }
    }
    if (val != null && val != '') {
        if (res != '')
            res += '&';
        res += param + '=' + encodeURIComponent(val);
    }
    var result = base.replace('#', '');
    if (res != '')
        result += '?' + res;
    return result;
}

function SetQueryParamFromControl(queryStr, control) {
    var val = control.data('paramvalue');
    return SetQueryParamFromControlInternal(queryStr, control, val);
}

function SetQueryParamFromControlByValue(queryStr, control) {

    var val = control.val();
    return SetQueryParamFromControlInternal(queryStr, control, val);
}

function SetQueryParamFromControlInternal(queryStr, control, val) {
    var param = control.data('filter-param-id');
    if (param == undefined)
        param = control.attr('id');
    return SetQueryParam(queryStr, param, val);
}

function SetQueryParamFromControlExact(queryStr, control, queryParam) {
    var val = control.data('paramvalue');
    return SetQueryParam(queryStr, queryParam, val);
}

function SetQueryParamFromControlExactValue(queryStr, control, queryParam) {
    var val = control.val();
    return SetQueryParam(queryStr, queryParam, val);
}

function autoRefreshPage(control) {
    var url = document.location.href;
    var newUrl = SetQueryParamFromControl(url, control);
    navigateTo(newUrl);
}

function navigateTo(url) {
    document.location.href = url;
}

function myGetPleaseWaitLoadingDiv(message) {
    if (!message) {
        message = "Загрузка...";
    } else {
        message = myEncodeHtml(message);
    }
    return '<div class="LoadingDiv"><img src="/Content/Icons/ajax-loader.gif" style="margin-right: 10px; height:24px" /><span style="font-size:12px">' + message + '</span></div>';
}

function myEncodeHtml(html) {
    return $('<div/>').text(html).html();
}

function myShowErrorMessage(text) {
    var encodedText = myEncodeHtml(text);
    DialogInfo(encodedText);
}

function myExecuteJson(command, parameters, successCallback, errorCallback) {
    if (!command) {
        myShowErrorMessage("Команда ExecuteJson не задана");
        return;
    }
    if (!errorCallback) {
        errorCallback = function (error) {
            myShowErrorMessage(error);
        };
    }
    if (!successCallback) {
        successCallback = function () { };
    }
    if (!parameters) {
        parameters = {};
    }
    var request = parameters;
    request["executeJsonCommand"] = command;
    request["random"] = Math.random();

    var requestSuccessCallback = function (response) {
        if (response.success) {
            successCallback(response.data);
        } else {
            var error = response.error;
            if (!error) {
                error = "Произошла неизвестная ошибка";
            }
            errorCallback(error);
        };
    };

    var requestErrorCallBack = function (httpRequest, textStatus) {
        errorCallback(textStatus);
    };

    $.ajax({
        url: "/json",
        dataType: 'json',
        data: request,
        success: requestSuccessCallback,
        error: requestErrorCallBack,
        type: "POST"
    });
}

// выполняет json, в отличие от myExecuteJson указывается урл, а не команда
// successCallback = func (data) {...}
// errorCallback = func (errorMessage) {...}
function ziExecuteJsonUrl(url, parameters, successCallback, errorCallback) {
    if (!url) {
        myShowErrorMessage("Url ExecuteJson не задана");
        return;
    }
    if (!errorCallback) {
        errorCallback = function (error) {
            myShowErrorMessage(error);
        };
    }
    if (!successCallback) {
        successCallback = function () { };
    }
    if (!parameters) {
        parameters = {};
    }

    var requestSuccessCallback = function (response) {
        if (response.success) {
            successCallback(response.data);
        } else {
            var error = response.error;
            if (!error) {
                error = "Произошла неизвестная ошибка";
            }
            errorCallback(error);
        };
    };

    var requestErrorCallBack = function (httpRequest, textStatus) {
        errorCallback(textStatus);
    };

    $.ajax({
        url: url,
        dataType: 'json',
        data: parameters,
        success: requestSuccessCallback,
        error: requestErrorCallBack,
        type: "POST"
    });
}

function confirmDeletion(message, callback) {
    if (!callback) {
        callback = function () { };
    }
    bootbox.setDefaults("locale", "ru");
    bootbox.dialog({
        message: message,
        title: "Подтвердите удаление",
        buttons: {
            yes: {
                label: "Удалить",
                className: "btn-danger",
                callback: function () {
                    callback();
                }
            },
            no: {
                label: "Отмена",
                className: "btn-default"
            }
        }
    }
    );
}

// открывает модальное окно по адресу href текущего элемента
function ModalBtnClick(sender, event) {
    event.preventDefault();
    var url = $(sender).attr('href');
    HideModal();
    UrlShowModal(url);
}

// открывает модальное окно по адресу href текущего элемента
function ziShowModalDialogByLink(sender, eventObj) {
    eventObj.preventDefault();
    //var url = $(sender).attr('href');
    var url = ziGetAjaxUrl($(sender));
    HideModal();
    UrlShowModal(url);
}

function ziGetAjaxUrl(elem) {
    var url = elem.data("ajax-url");
    if (!url) {
        url = elem.attr('href');
    }
    return url;
}

function ziCreateModalDialog(selector) {
    var elements = $(selector);
    elements.click(function (event) {
        event.preventDefault();
        //var url = $(this).attr('href');
        var url = ziGetAjaxUrl($(this));
        HideModal();
        UrlShowModal(url);
    });
}

// Открывает модальное окно, и выполняет callback после загрузки HTML диалога
function UrlShowModal(url, callback) {
    var body = $('body', $(document));
    var div = $('#ModalDlgDiv', body);
    if (div.length == 0) {
        div = $('<div id="ModalDlgDiv"></div>');
        body.append(div);
    }
    div.load(url, function () {
        $('#ModalDlg', div).modal('show');
        if (callback) {
            if (typeof (callback) == "string") {
                callback = window[callback];
            }
            callback(div);
        }
    });
}

// закрывает модальное окно
function HideModal() {
    var body = $('body', $(document));
    var div = $('#ModalDlgDiv', body);
    $('#ModalDlg', div).modal('hide');
}

// создаем случайный Guid
function myGuid() {
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000)
            .toString(16)
            .substring(1);
    }
    return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
        s4() + '-' + s4() + s4() + s4();
}

function addKeyValueRow(collectionName) {
    var table = $("#" + collectionName);
    var guid = myGuid();

    var template = '<tr><td>'
        + '<input type="hidden" name="products.Index" value="guid" />'
        + '<input type="text" name="products[guid].Key" class="form-control" />'
        + '</td><td>'
        + '<input type="text" name="products[guid].Value" class="form-control" />'
        + '</td><td>'
        + '<a href="#" onclick="$(this).closest(\'tr\').remove(); return false;"><span class="glyphicon glyphicon-remove"></span></a>'
        + '</td></tr>"';

    var html = template.replace(new RegExp("products", 'g'), collectionName);
    html = html.replace(new RegExp("guid", 'g'), guid);
    table.append(html);
    return false;
}


function setEqualHeight(selector) {
    var highestBox = 0;
    $(selector).each(function () {
        if ($(this).height() > highestBox) {
            highestBox = $(this).height();
        }
    });
    $(selector).height(highestBox);
}

$(function () {
    Inputmask.extendAliases({
        'number': {
            alias: "numeric",
            placeholder: '0',
            allowPlus: false,
            allowMinus: false,
            autoGroup: false,
            groupSeparator: "",
            rightAlign: false,
            digits: 0,
            digitsOptional: false
        }
    });

    Inputmask.extendAliases({
        'number': {
            alias: "numeric",
            placeholder: '0',
            allowPlus: false,
            allowMinus: false,
            autoGroup: false,
            groupSeparator: "",
            rightAlign: false,
            digits: 0,
            digitsOptional: false
        }
    });

    Inputmask.extendAliases({
        "time": {
            mask: "h:s",
            placeholder: "__:__",
            alias: "datetime",
            autoUnmask: !1
        }
    }
    );

});

function DialogInfo(message) {
    bootbox.setDefaults("locale", "ru");
    bootbox.dialog({
        message: message,
        title: "Сообщение",
        buttons: {
            ok: {
                label: "OK",
                className: "btn-primary smart-button"
            }
        }
    }
    );
}

function getFunction(code, argNames) {
    var fn = window, parts = (code || "").split(".");
    while (fn && parts.length) {
        fn = fn[parts.shift()];
    }
    if (typeof (fn) === "function") {
        return fn;
    }
    argNames.push(code);
    return Function.constructor.apply(null, argNames);
}

function setTableHeaderTooltip(element, html) {
    //element.tooltip({
    //    container: 'body',
    //    html: true,
    //    placement: 'auto bottom',
    //    title: html
    //});
    element.tooltipster({
        arrow: true,
        contentAsHTML: true,
        interactive: true, // чтобы можно было кликать ссылки внутри тултипа
        functionInit: function (origin, content) {
            return "<div class='ziTooltipWrapper'>" + html + "</div>";
        }
    });
}

function ziReloadPage(parameter, value, event) {
    if (event) {
        event.preventDefault();
    }
    if (parameter) {
        var url = document.location.href;
        var newUrl = SetQueryParam(url, parameter, value);
        if (newUrl == url) {
            document.location.reload();
        } else {
            document.location.href = newUrl;
        }
    } else {
        document.location.reload();
    }
}

// запускает колесико внутри элемента (кнопки)
function ziBeginWaitAnimation(elem) {
    // если элемент в статусе "в процессе", то ничего не делаем
    elem = $(elem);
    var status = elem.data("zi-wait-status");
    if (status == "inProgress") {
        return;
    }
    // зафиксируем ширину, чтобы размер кнопки не изменился после изменения содержимого (innerHtml)
    var width = elem.width();
    elem.width(width);

    // изменим статус
    elem.data("zi-wait-status", "inProgress");

    // запомним старое содержимое кнопки, чтобы вернуть его после обработки
    var oldHtml = elem.html();
    elem.data("zi-wait-old-html", oldHtml);

    // покажем колесико внутри конопки
    var waitElemHtml = "<img style='height:16px;' src='/content/icons/ajax-loader.gif'>";
    elem.html(waitElemHtml);
}

// завершает показ колесика внутри элемента (кнопки)
function ziStopWaitAnimation(elem) {
    // проверим статус
    var status = elem.data("zi-wait-status");
    if (status != "inProgress") {
        console.log("error: status not inProgress...");
        return;
    }
    // вернем старый html
    var oldHtml = elem.data("zi-wait-old-html");
    if (!oldHtml) {
        console.log("error: не удалось найти oldHtml...");
        return;
    }
    elem.html(oldHtml);

    // установим новый статус
    elem.data("zi-wait-status", "complited");
}

// Обработчик события onclick для ajax-ссылки
// - запускает проигрывание колесика
// - блокирует повторное нажатие
function ziWaitAnimationProcessor(elem, execution) {

    // если в статус "в процессе", то ничего не делаем (ждем завершения)
    elem = $(elem);
    var status = elem.data("zi-wait-status");
    if (status == "inProgress") {
        return;
    }

    // запускаем колесико
    ziBeginWaitAnimation(elem);

    // выполняем полезную работу
    execution();
}

// превращает обычную ссылку в ajax-ссылку, при клике на которую выполняется ajax-команда,
// во время выполнения команды крутится колесико, повторное нажатие блокируется
// параметры:
// - selector ссылок для обработки
// - onSuccess - функция func(data). Если не задана, то обновляет страницу
// - onError - функция func(errorMessage). Если не задана, то показывает ошибку в новом диалоговом окне
function ziSetAjaxLink(selector, onSuccess, onError) {
    $(selector).click(function (event) {
        ziAjaxLinkOnClickHandler(this, event, onSuccess, onError);
    });
}

// Обработчик события onclick для ajax-ссылки
function ziAjaxLinkOnClickHandler(elem, event, onSuccess, onError) {

    // отключаем открытие новой страницы
    event.preventDefault();

    // выполняем ajax-команду
    elem = $(elem);
    var url = ziGetAjaxUrl(elem);
    var data = null;
    ziWaitAjaxRequest(elem, url, data, onSuccess, onError);
}

// Ждет выполнения ajax-команды, а пока команда выполняется крутим колесико в  элементе управления (кнопке)
function ziWaitAjaxRequest(elem, url, data, onSuccess, onError) {

    // если в статус "в процессе", то ничего не делаем (ждем завершения)
    elem = $(elem);
    var status = elem.data("zi-wait-status");
    if (status == "inProgress") {
        return;
    }

    // запускаем колесико
    ziBeginWaitAnimation(elem);

    // если ajax-команда выполнилась успешно
    var onSuccessWrapper = function (responseData) {
        if (onSuccess) {
            ziStopWaitAnimation(elem);
            onSuccess(responseData);
        } else {
            // тут анимацию НЕ останавливаем, чтобы не было НЕПОНЯТНОЙ паузы между анимацией и показом новой страницы
            ziReloadPage();
        }
    }

    // если ajax-команда выполнилась с ошибкой
    var onErrorWrapper = function (errorMessage) {
        ziStopWaitAnimation(elem);
        if (onError) {
            onError(errorMessage);
        } else {
            myShowErrorMessage(errorMessage);
        }
    }

    // выполняем ajax-команду
    ziExecuteJsonUrl(url, data, onSuccessWrapper, onErrorWrapper);
}

function ziBeginHandleWindowErrors() {
    window.onerror = function (message, url, line, col, error) {
        var data = {
            pageUrl: document.location.href,
            message: message,
            scriptUrl: url,
            line: line,
            column: col,
            error: error
        };
        if (error && error.stack) {
            data.stack = error.stack;
        }
        $.post('/debug/logJsError/', data);
    }
}

// Все методы нужно перенести сюда
var utils = (function () {
    function scrollTo(element, offset) {
        $('html, body').scrollTop(element.offset().top - fixedHeader.height() - offset);
    }

    function guid() {
        function s4() {
            return Math.floor((1 + Math.random()) * 0x10000)
                .toString(16)
                .substring(1);
        }
        return s4() + s4() + '-' + s4() + '-' + s4() + '-' +
            s4() + '-' + s4() + s4() + s4();
    }

    function reloadPage() {
        document.location.reload();
    }

    return {
        scrollTo: scrollTo,
        guid: guid,
        reloadPage: reloadPage
    };
})();