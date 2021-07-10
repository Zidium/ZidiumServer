var UnitTestTypeSelector = function (control, callback) {
    var me = this;
    me.control = control;
    me.callback = callback;
    me.SetValue(control.val());
    me.control.on('change', function () { me.onChange(); });
}

UnitTestTypeSelector.prototype.SetValue = function (value) {
    this.control.data('paramvalue', value);
}

UnitTestTypeSelector.prototype.onChange = function () {
    this.SetValue(this.control.val());
    if (this.callback) {
        this.callback();
    }
}