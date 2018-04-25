var BooleanButton = function (control, falseIsNull, captionTrue, captionFalse, value, callback) {
    var me = this;
    me.control = control;
    me.callback = callback;
    me.falseIsNull = falseIsNull;
    me.captionTrue = captionTrue;
    me.captionFalse = captionFalse;
    me.SetValue(value);
    me.control.on('click', function () { me.onClick(); });
}

BooleanButton.prototype.SetValue = function (value) {
    var paramStr;
    if (value == '0' && this.falseIsNull == '1') {
        paramStr = '';
    } else {
        paramStr = value;
    }
    if (value == '0') {
        this.control.val(this.captionTrue);
    } else {
        this.control.val(this.captionFalse);
    }
    this.control.data('paramvalue', paramStr);
}

BooleanButton.prototype.onClick = function() {
    var value;
    var paramStr = this.control.data('paramvalue');
    if (paramStr == '' && this.falseIsNull == '1') {
        value = '0';
    } else if (paramStr == '' && this.falseIsNull == '0') {
        value = '1';
    } else {
        value = paramStr;
    }
    if (value == '0') {
        value = '1';
    } else {
        value = '0';
    }
    this.SetValue(value);
    if (this.callback) {
        this.callback();
    }
}