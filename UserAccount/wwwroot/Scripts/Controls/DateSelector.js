var DateSelector = function(control, hidden, displayFormat, paramFormat, callback) {
    var me = this;
    me.control = control;
    me.hidden = hidden;
    me.paramFormat = paramFormat;
    me.callback = callback;
    me.control.parent().datetimepicker({
        format: displayFormat,
        locale: 'ru',
        showClear: true
    });
    var date = me.control.parent().data("DateTimePicker").date();
    me.SetDate(date);
    me.control.parent().on('dp.change', function (e) { me.onChange(e); });
    $(control).inputmask();
}

DateSelector.prototype.onChange = function (e) {
    var newDate;
    if (e.date != null && e.date != false)
        newDate = moment(e.date);
    else
        newDate = null;
    this.SetDate(newDate);
    if (this.callback) {
        this.callback();
    }
    this.control.parent().data("DateTimePicker").hide();
    this.control.trigger('dateChanged');
}

DateSelector.prototype.SetDate = function (date) {
    var paramStr;
    if (date != null)
        paramStr = date.format(this.paramFormat);
    else
        paramStr = null;
    this.control.data('paramvalue', paramStr);
    this.hidden.val(paramStr);
}