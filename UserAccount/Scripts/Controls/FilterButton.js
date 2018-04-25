var FilterButton = function (control, filterElementId) {
    var me = this;
    me.control = control;
    me.filterElementId = filterElementId;
    me.control.on('click', function () { me.onClick(); });
}

FilterButton.prototype.onClick = function () {
    var url = document.location.href;
    var controls = $("[data-filter='true']", $('#' + this.filterElementId));
    controls.each(function (index, element) {
        url = SetQueryParamFromControl(url, $(element));
    });
    controls = $("[data-filterbyvalue='true']", $('#' + this.filterElementId));
    controls.each(function (index, element) {
        url = SetQueryParamFromControlByValue(url, $(element));
    });
    document.location.href = url;
}