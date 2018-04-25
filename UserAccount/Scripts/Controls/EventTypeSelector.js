var EventTypeSelector = function (hidden, textbox, div, value, str, callback, allowEmpty, modelName) {
    var me = this;
    me.Textbox = textbox;
    me.Div = div;
    var selector = new DropdownSelectorBase(hidden, textbox, div, value, str,
        function () { me.OnOpen(me); },
        function () { me.OnClose(me); },
        allowEmpty);
    me.Selector = selector;
    me.Callback = callback;
    me.ModelName = modelName;
    me.InLoading = false;
    me.NeedReload = false;
    me.externalEventCategorySelectorId = null;
}

EventTypeSelector.prototype.OnFilterChange = function (me) {
    if (!me.Selector.IsOpened)
        return;
    me.LoadData(me);
}

EventTypeSelector.prototype.LoadData = function (me, ignoreName) {
    if (me.InLoading) {
        me.NeedReload = true;
        return;
    }
    me.InLoading = true;
    $('#loading', me.Div).show();
    var eventtypesDiv = $('#eventtypes', me.Div);
    var url = eventtypesDiv.data('url');
    
    var category = me.externalEventCategorySelectorId != null ?
        $('#' + me.externalEventCategorySelectorId) :
        $('#' + me.ModelName + '_category', me.Div);
    url = SetQueryParamFromControlExactValue(url, category, 'category');
    if (!ignoreName) {
        var search = me.Textbox;
        url = SetQueryParamFromControlExactValue(url, search, 'search');
    }
    eventtypesDiv.load(url, function () {
        $('a[data-id]', me.Div).click(function () { me.OnSelectCallback(this); });
        $('#loading', me.Div).hide();
        eventtypesDiv.show();
        if (me.NeedReload) {
            me.LoadData(me);
            me.NeedReload = false;
        }
        me.InLoading = false;
    });
}

EventTypeSelector.prototype.OnOpen = function (me) {
    me.LoadData(me, true);
    me.Textbox.on('input', function () { me.OnFilterChange(me); });
    var category = me.externalEventCategorySelectorId != null ?
        $('#' + me.externalEventCategorySelectorId) :
        $('#' + me.ModelName + '_category', me.Div);
    category.on('change', function () { me.OnFilterChange(me); });
}

EventTypeSelector.prototype.OnClose = function (me) {
    var componentsDiv = $('#eventtypes', me.Div);
    componentsDiv.hide();
}

EventTypeSelector.prototype.OnSelectCallback = function (e) {
    var elem = $(e);
    var id = elem.data('id');
    var value = elem.text();
    this.ValueSelected(id, value);
}

EventTypeSelector.prototype.ValueSelected = function (value, str) {
    this.Selector.ValueSelected(value, str);
    if (this.Callback)
        this.Callback();
}