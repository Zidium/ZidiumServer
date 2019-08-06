var componentTreeDetails = (function () {

    // тип вьюшки детализации (детализация компонента или детализация юнит-теста)
    var detailsType = "unknown";

    // у каждой вьюшки детализации свой конфиг
    function getConfig() {
        var config = smartConfig.getOrCreate("components-tree-details-panel/" + detailsType);
        return config;
    }

    function getLastTabId() {
        var config = getConfig();
        var field = config.getField("lastTabId");
        return field.getValue();
    }

    function setLastTabId(value) {
        var config = getConfig();
        var field = config.getField("lastTabId");
        return field.setValue(value);
    }

    function accordionLoadGroup(element) {
        var smartBlock = $('.smart-block', element);
        //smartBlock.html('<p><img src="/Content/Icons/ajax-loader-white.gif" /></p>');
        showLoader();
        smartBlocks.load({
            elements: smartBlock,
            onComplete: function () {
                hideLoader();
            }
        });
    }

    function accordionReloadCurrentGroup(element) {
        var groupContent;
        if (element !== null)
            groupContent = element.closest('.tree-details-content');
        else {
            var container = getDetailsContainer();
            var group = getLastTabId();
            groupContent = getContentByGroup(container, group);
        }

        if (groupContent !== null)
            accordionLoadGroup(groupContent);
    }

    function showGroup(groupName) {
        var link = $('.tree-details-container .tree-details-menu-item[data-group="' + groupName + '"] > a');
        link.click();
    }

    function getDetailsContainer() {
        return $('#details-panel-container');
    }

    function showLoader() {
        $("#details-panel-loader").show();
    }

    function hideLoader() {
        $("#details-panel-loader").hide();
    }

    function load(aDetailsType, url, data, onLoad) {
        detailsType = aDetailsType;
        showLoader();
        var tempDetails = $("<div class='smart-block'></div>");
        var emptyHtml = tempDetails[0].outerHTML;

        var replaceDetails = function () {
            var html = tempDetails.html();
            var container = getDetailsContainer();
            container.html(html);

            // Выберем элемент меню
            var lastTab = getLastTabId();
            selectMenuItem(lastTab);

            hideLoader();
        };

        var onComplete = function () {
            setTimeout(function () {
                var newHtml = tempDetails[0].outerHTML;
                if (newHtml !== emptyHtml) {
                    // Контент загружен успешно
                    var lastTab = getLastTabId();

                    // Проверим, что такой раздел существует
                    var tab = getContentByGroup(tempDetails, lastTab);
                    if (tab.length === 0) {
                        lastTab = null;
                    }

                    // Если раздела нет, или он не выбирался, возьмём первый из списка
                    if (!lastTab) {
                        lastTab = $(".tree-details-content", tempDetails).data("group");
                        setLastTabId(lastTab);
                    }

                    tab = getContentByGroup(tempDetails, lastTab);
                    if (tab.length === 1) {
                        var smartTab = $(".smart-block", tab);
                        smartBlocks.load({
                            elements: smartTab,
                            onComplete: function () {
                                setTimeout(function () {
                                    //var tabGroup = $('tree-details-content[data-group="' + lastTab + '"]', tempDetails);
                                    tab.removeClass("collapse");
                                    //tabGroup.addClass("in");
                                    tab.attr("data-loaded", "true"); //data('loaded', 'true');
                                    replaceDetails();

                                    if (onLoad) {
                                        onLoad();
                                    }
                                }, 1);
                            }
                        });
                    } else {
                        replaceDetails();

                        if (onLoad) {
                            onLoad();
                        }
                    }

                } else {
                    hideLoader();

                    if (onLoad) {
                        onLoad();
                    }
                }
            }, 1);
        };

        smartBlocks.doSubmit(url, data, tempDetails, onComplete);
    }

    function onContextShown() {
        var me = $(this);
        var loaded = me.data('loaded');
        if (!loaded) {
            // Загрузим содержимое
            accordionLoadGroup(me);

            // Поставим признак загрузки, чтобы не загружать при повторном открытии
            me.data('loaded', 'true');
        }

        // Запомним выбранный элемент
        var group = me.data('group');
        setLastTabId(group);

        // Выберем элемент меню
        selectMenuItem(group);
    }

    function getContentByGroup(container, group) {
        return $('.tree-details-content[data-group="' + group + '"]', container);
    }

    function selectMenuItem(group) {
        var container = getDetailsContainer();
        $('.tree-details-menu-item', container).removeClass('selected');
        $('.tree-details-menu-item[data-group="' + group + '"]', container).addClass('selected');
    }

    function init() {

        // Обработчик клика по картинке раздела
        $('body').on('click', '.tree-details-container .tree-details-menu-item > a', function (event) {
            event.preventDefault();
            var container = $(this).closest('.tree-details-container');
            $('.tree-details-content', container).hide();
            var group = $(this).closest('.tree-details-menu-item').data('group');
            getContentByGroup(container, group).show(0, onContextShown);
        });

    }

    init();

    return {
        load: load,
        showGroup: showGroup,
        accordionReloadCurrentGroup: accordionReloadCurrentGroup
    };
})();


$(function () {
    var config = smartConfig.getOrCreate("components-tree-details-panel");
});
