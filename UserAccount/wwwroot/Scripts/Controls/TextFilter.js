var TextFilter = function (control, value, callback) {
    var me = this;
    me.control = control;
    me.callback = callback;
    me.SetValue(value);
    me.control.on('change', function () { me.onChange(); });
}

TextFilter.prototype.onChange = function() {
    var value = this.control.val();
    this.SetValue(value);
    if (this.callback) {
        this.callback();
    }
}

TextFilter.prototype.SetValue = function(value) {
    this.control.data('paramvalue', value);
}