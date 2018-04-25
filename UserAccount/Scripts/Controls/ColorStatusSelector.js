function getColorStatusSelector(id) {
    var div = $(id);
    var selector = {};
    selector.autoRefresh = false;
    selector.multiSelectMode = true;
    selector.div = div;
    selector.hidden = div.find("input[type='hidden']");
    selector.buttons = div.find(".btn");
    selector.callback = null;

    selector.update = function () {
        this.buttons.each(function (index, elem) {
            var button = $(elem);
            //var color = button.data("color");
            var value = button.data("value");
            if (value) {
                button.addClass("colorStatusChecked");
            } else {
                button.removeClass("colorStatusChecked");
            }
        });
    };

    selector.setValue = function (color, value) {
        var button = div.find(".btn[data-color='" + color + "']");
        if (this.multiSelectMode == false) {
            this.buttons.removeClass("colorStatusChecked");
            this.buttons.data("value", false);
        }
        button.data("value", value);

        if (value) {
            button.addClass("colorStatusChecked");
        } else {
            button.removeClass("colorStatusChecked");
        }
        var val = "";
        this.buttons.each(function (index, elem) {
            var button = $(elem);
            var color = button.data("color");
            var value = button.data("value");
            if (value) {
                if (val == "") {
                    val = color;
                } else {
                    val = val + "~" + color;
                }
            }
        });
        this.hidden.val(val);
        this.hidden.data("paramvalue", val);

        if (this.autoRefresh) {
            autoRefreshPage(this.hidden);
        }

        if (this.callback) {
            getFunction(this.callback, ["value"]).apply(this, [this.getValue()]);
        }
    };

    selector.getValue = function () {
        return this.hidden.val();
    }

    selector.buttons.click(function () {
        var me = $(this);
        var color = me.data("color");
        var oldValue = me.data("value");
        var newValue = !oldValue;
        selector.setValue(color, newValue);
    });

    return selector;
}