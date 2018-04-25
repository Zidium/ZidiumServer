var ComponentSelector = function (hidden, textbox, div, value, str, callback, allowEmpty, modelName) {
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
    me.externalComponentTypeSelectId = null;
}

ComponentSelector.prototype.OnFilterChange = function (me, ignoreName) {
    if (!me.Selector.IsOpened)
        return;

    var componentType;
    if (me.externalComponentTypeSelectId == null) {
        componentType = $('#' + me.ModelName + '_componentTypeId', me.Div).data('paramvalue');
    } else {
        componentType = $('#' + me.externalComponentTypeSelectId).data('paramvalue');
    }

    var status = $('#' + me.ModelName + '_status', me.Div).data('paramvalue');
    var search = me.Textbox.val();

    var root = $('#components > ul > li', me.Div);
    me.FilterElement(me, root, componentType, status, search, ignoreName);
}

ComponentSelector.prototype.FilterElement = function (me, element, componentType, status, search, ignoreName) {
    search = search || '';
    var name = element.data('name') || '';

    var f = (componentType === '') || (componentType == null) || (element.data('componenttype') === componentType);
    f = f && ((status === '') || (status == null) || (status.indexOf(element.data('status')) > -1));
    f = f && (ignoreName || (search === '') || (name.toLowerCase().indexOf(search.toLowerCase()) > -1));

    var b = false;
    var childs = $('> ul > li', element);
    childs.each(function(index, e) {
        var child = $(e);
        var r = me.FilterElement(me, child, componentType, status, search, ignoreName);
        b = b || r;
    });

    var result = f || b;
    if (result === true)
        element.show();
    else
        element.hide();

    return result;
}

ComponentSelector.prototype.LoadData = function(me) {
    $('#loading', me.Div).show();
    var componentsDiv = $('#components', me.Div);
    var url = componentsDiv.data('url');
    componentsDiv.load(url, function () {
        me.OnFilterChange(me, true);
        $('a[data-id]', me.Div).click(function () { me.OnSelectCallback(this); });
        $('#loading', me.Div).hide();
        componentsDiv.show();
    });
}

ComponentSelector.prototype.OnOpen = function (me) {
    me.InitialValue = me.Textbox.val();
    me.LoadData(me);
    me.Textbox.on('input', function () { me.OnFilterChange(me); });
    var componentType = $('#' + me.ModelName + '_componentTypeId', me.Div);
    componentType.on('change', function () { me.OnFilterChange(me, true); });
    var status = $('#' + me.ModelName + '_status', me.Div);
    status.on('change', function () { me.OnFilterChange(me); });
}

ComponentSelector.prototype.OnClose = function(me) {
    var componentsDiv = $('#components', me.Div);
    componentsDiv.hide();
    if (me.Selector.AllowEmpty && me.Textbox.val() === '' && me.IsValueChanged())
        this.ValueSelected(null, '');
}

ComponentSelector.prototype.OnSelectCallback = function(e) {
    var elem = $(e);
    var id = elem.data('id');
    var value = elem.text();
    this.ValueSelected(id, value);
}

ComponentSelector.prototype.ValueSelected = function(value, str) {
    this.Selector.ValueSelected(value, str);
    if (this.Callback)
        this.Callback();
}

ComponentSelector.prototype.IsValueChanged = function() {
    return this.InitialValue !== this.Textbox.val();
}