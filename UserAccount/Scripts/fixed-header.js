var fixedHeader = (function () {
    function updateHeader() {
        $('body').css('padding-top', height() + 'px');
        $('body').trigger('fixedHeaderUpdated');
    }

    function height() {
        var header = $('#fixed-header');
        if (header != null)
            return header.outerHeight();
        return 0;
    }

    return {
        updateHeader: updateHeader,
        height: height
    };
})();

$(function () {
    fixedHeader.updateHeader();
    $(window).on('resize', fixedHeader.updateHeader);
});