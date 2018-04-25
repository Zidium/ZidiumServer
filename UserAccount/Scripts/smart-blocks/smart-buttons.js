var smartButtons = (function () {

    var loaders = new Object();

    function SmartButtonData() { }

    SmartButtonData.prototype = {
        active: false,
        html: null
    }

    var globalLoaderImage = null;

    function onButtonClick(event) {
        if (event.target == null)
            return;

        var button = $(event.target).closest('.smart-block-button');
        if (button == null)
            return;

        var data = getData(button);

        if (data.active) {
            event.preventDefault();
            event.stopPropagation();
            return;
        }
    }

    function beginAction(button) {
        var data = getData(button);

        data.active = true;
        button.css('min-width', button.css('width'));
        data.html = button.html();

        // disable button after all events
        setTimeout(function () { button.prop('disabled', true); }, 0);

        var loaderImage = getLoader(button);
        if (loaderImage != null)
            button.html("<img src='" + loaderImage + "'/>");
        else
            button.html("<span>Please wait...</span>");

        button.data('smart-button', data);
    }

    function endAction(button) {
        var data = getData(button);

        if (!data.active)
            return;

        button.html(data.html);
        button.prop('disabled', false);
        button.css('min-width', '');
        data.active = false;

        button.data('smart-button', data);
    }

    function getData(button) {
        return button.data('smart-button') || new SmartButtonData();
    }

    function getLoader(button) {
        var loaderImage = null;

        var loaderName = button.data('smart-button-loader');
        if (loaderName != null)
            loaderImage = loaders[loaderName];

        if (loaderImage == null)
            loaderImage = globalLoaderImage;

        return loaderImage;
    }

    function registerLoaderImage(name, image) {
        loaders[name] = image;
    }

    return {
        onButtonClick: onButtonClick,
        beginAction: beginAction,
        endAction: endAction,
        registerLoaderImage: registerLoaderImage,
        setGlobalLoaderImage: function (image) { globalLoaderImage = image; }
    };
})();

$(function () {
    document.addEventListener("click", smartButtons.onButtonClick, true);
});