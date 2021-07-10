var DropdownSelectorBase = function (hidden, textbox, div, value, str, openCallback, closeCallback, allowEmpty) {
    var me = this;
    me.Hidden = hidden;
    me.Textbox = textbox;
    me.Div = div;
    me.IsOpened = false;
    me.SetValue(value, str);
    me.ValueChanged = false;
    me.OpenCallback = openCallback;
    me.CloseCallback = closeCallback;
    me.AllowEmpty = allowEmpty;
    me.Textbox.on('focus', function () { me.OnFocus(); });

    me.Div.parent().on('hidden.bs.dropdown', function () {
        me.OnHide();
    });

    me.Div.on('click.bs.dropdown.data-api', function(e) {
         e.stopPropagation();
    });

    me.Textbox.on('click.bs.dropdown.data-api', function(e) {
         e.stopPropagation();
    });
}

DropdownSelectorBase.prototype.OnFocus = function() {
    if (this.IsOpened)
        return;
    this.Open();
}

DropdownSelectorBase.prototype.Open = function () {
    this.IsOpened = true;
    this.Hidden.dropdown('toggle');

    if (this.OpenCallback)
        this.OpenCallback();
}

DropdownSelectorBase.prototype.OnHide = function () {
    if (!this.IsOpened)
        return;
    this.IsOpened = false;
    if (this.AllowEmpty && this.Textbox.val() === '') {
        this.SetValue(null, null);
    }
    else if (!this.ValueChanged) {
        this.Textbox.val(this.Hidden.data('strvalue'));
    }
    this.ValueChanged = false;
    if (this.CloseCallback)
        this.CloseCallback();
}

DropdownSelectorBase.prototype.SetValue = function (value, str) {
    this.Hidden.data('paramvalue', value);
    this.Hidden.data('strvalue', str);
    this.Hidden.val(value);
    this.Textbox.val(str);
    this.ValueChanged = true;
}

DropdownSelectorBase.prototype.ValueSelected = function (value, str) {
    this.SetValue(value, str);
    this.DoClose();
}

DropdownSelectorBase.prototype.DoClose = function () {
    if (!this.IsOpened)
        return;
    this.Hidden.dropdown('toggle');
}