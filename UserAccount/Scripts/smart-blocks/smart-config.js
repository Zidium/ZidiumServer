var smartConfig = (function () {

    var keyPrefix = "-smart-";

    function loadData(name) {
        var json = localStorage.getItem(keyPrefix + name);
        if (json == null) {
            return null;
        }
        return JSON.parse(json);
    }

    function saveData(name, data) {
        var key = keyPrefix + name;
        if (data) {
            var json = JSON.stringify(data);
            localStorage.setItem(key, json);
        } else {
            localStorage.removeItem(key);
        }
    }

    function ConfigField(config, name) {
        var private = {
            config: config,
            name: name
        };
        var getNotNullData = function () {
            var data = private.config.getData();
            if (data == null) {
                data = {};
                private.config.setData(data);
            }
            return data;
        };
        this.getValueOrDefault = function(defaultValue) {
            var data = getNotNullData();
            var value = data[name];
            if (typeof (value) != 'undefined' && value != null) {
                return value;
            }
            return defaultValue;
        };
        this.setValue = function(value) {
            var data = getNotNullData();
            data[private.name] = value;
            private.config.save();
        };
    }

    ConfigField.prototype = {
        getValue : function() {
            return this.getValueOrDefault(null);
        }
    };

    function ConfigData(name, info, data) {

        if (!info) {
            info = {
                created : new Date()
            };
        }

        var private = {
            name: name,
            info: info,
            data: data
        };

        this.save = function () {
            private.info.lastSave = new Date();
            var json = {
                info : private.info,
                data : private.data
            };
            saveData(private.name, json);
        };

        this.name = function() {
            return private.name;
        }

        this.setData = function(data) {
            private.data = data;
            this.save();
        };

        this.getData = function() {
            return private.data;
        };
    }

    ConfigData.prototype = {
        getField : function(name) {
            return new ConfigField(this, name);
        },
        hasField: function (name) {
            var data = this.getData();
            if (data == null) {
                return false;
            }
            var value = data[name];
            if (typeof (value) != 'undefined' && value != null) {
                return true;
            }
            return false;
        }
    };

    return {
        get : function(name) {
            var config = loadData(name);
            if (config==null) {
                return null;
            }
            return new ConfigData(name, config.info, config.data);
        },
        getOrCreate: function (name) {
            var config = loadData(name);
            if (config == null) {
                var info = {
                    created: new Date()
                };
                var data = null;
                return new ConfigData(name, info, data);
            }
            return new ConfigData(name, config.info, config.data);
        },
        has : function(name) {
            var json = localStorage.getItem(keyPrefix + name);
            return json != null;
        }
    };
})();