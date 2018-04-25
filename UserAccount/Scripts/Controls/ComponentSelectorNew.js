var ComponentSelectorNew = function (hidden, textbox, caption, div, value, str, callback, allowEmpty, modelName) {
    var me = this;
    me.Hidden = hidden;
    me.Textbox = textbox;
    me.Caption = caption;
    me.Div = div;
    me.Callback = callback;
    me.ModelName = modelName;
    me.externalComponentTypeSelectId = null;

    me.Caption.click(function (e) {
        me.Hidden.dropdown('toggle');
        e.stopPropagation();
    });

    me.Div.parent().on('shown.bs.dropdown', function () {
        me.OnOpen(me);
    });

    me.Div.parent().on('hidden.bs.dropdown', function () {
        me.OnClose(me);
    });

    me.Div.on('click.bs.dropdown.data-api', function (e) {
        e.stopPropagation();
    });
}

ComponentSelectorNew.prototype.OnFilterChange = function (me, ignoreName) {
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

ComponentSelectorNew.prototype.FilterElement = function (me, element, componentType, status, search, ignoreName) {
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

ComponentSelectorNew.prototype.LoadData = function (me) {
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

ComponentSelectorNew.prototype.OnOpen = function (me) {
    me.InitialValue = me.Textbox.val();
    me.LoadData(me);
    me.Textbox.on('input', function () { me.OnFilterChange(me); });
    var componentType = $('#' + me.ModelName + '_componentTypeId', me.Div);
    componentType.on('change', function () { me.OnFilterChange(me, true); });
    var status = $('#' + me.ModelName + '_status', me.Div);
    status.on('change', function () { me.OnFilterChange(me); });
}

ComponentSelectorNew.prototype.OnClose = function (me) {
    var componentsDiv = $('#components', me.Div);
    componentsDiv.hide();
}

ComponentSelectorNew.prototype.OnSelectCallback = function (e) {
    var elem = $(e);
    var id = elem.data('id');
    var value = elem.data('full-name');
    this.ValueSelected(id, value);
}

ComponentSelectorNew.prototype.ValueSelected = function(id, value) {
    this.Hidden.data('paramvalue', id);
    this.Hidden.val(id);
    this.Caption.text(value);
    this.Caption.attr('title', value);

    this.Hidden.dropdown('toggle');

    if (this.Callback)
        this.Callback();
}

ComponentSelectorNew.prototype.Clear = function() {
    this.ValueSelected(null, 'Выберите компонент');
}
