var ComponentStatusSelector = function(control, callback) {
    var me = this;
    me.control = control;
    me.callback = callback;
    me.control.chosen({
        placeholder_text_multiple: "Выберите статус",
        width: "100%"
    });
    me.SetValue(me.control.val());
    me.control.on('change', function () { me.onChange(); });
}

ComponentStatusSelector.prototype.onChange = function() {
    this.SetValue(this.control.val());
    if (this.callback) {
        this.callback();
    }
}

ComponentStatusSelector.prototype.SetValue = function(value) {
    var paramStr;
    if (value != null)
        paramStr = value.join('~');
    else
        paramStr = null;
    this.control.data('paramvalue', paramStr);
}