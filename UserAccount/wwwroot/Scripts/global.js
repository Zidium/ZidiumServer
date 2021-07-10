$(function() {
    smartBlocks.setGlobalErrorCallback(function (message) {
        dialogs.error(message);
    });

    smartButtons.setGlobalLoaderImage('/Content/Icons/ajax-loader-white.gif');
    smartButtons.registerLoaderImage('blue', '/Content/Icons/ajax-loader-blue.gif');

})