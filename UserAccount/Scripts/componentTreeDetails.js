var componentTreeDetails = (function () {

    // тип вьюшки детализации (детализация компонента или детализация юнит-теста)
    var _detailsType = "unknown";

    // у каждой вьюшки детализации свой конфиг
    function getConfig() {
        var config = smartConfig.getOrCreate("components-tree-details-panel/" + _detailsType);
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
        smartBlock.html('<p><img src="/Content/Icons/ajax-loader-white.gif" /></p>');
        smartBlocks.load({ elements: smartBlock });
    }

    function accordionReloadCurrentGroup(element) {
        var accordionGroupContent;
        if (element != null)
            accordionGroupContent = element.closest('.accordion-group-content');
        else {
            var id = getLastTabId();
            accordionGroupContent = $('.accordion-group-content', $('#' + id));
        }

        if (accordionGroupContent != null)
            accordionLoadGroup(accordionGroupContent);
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

    function load(detailsType, url, data) {
        _detailsType = detailsType;
        showLoader();
        var tempDetails = $("<div class='smart-block'></div>");
        var emptyHtml = tempDetails[0].outerHTML;

        var replaceDetails = function () {
            var html = tempDetails.html();
            var container = getDetailsContainer();
            container.html(html);
            hideLoader();
        }
        var onComplete = function () {
            setTimeout(function () {
                var newHtml = tempDetails[0].outerHTML;
                if (newHtml !== emptyHtml) {
                    // значит контент загружен успешно
                    var lastTab = getLastTabId();
                    if (!lastTab) {
                        lastTab = $(".accordion-group", tempDetails).attr("id");
                        setLastTabId(lastTab);
                    }
                    if (lastTab) {
                        var tab = $("#" + lastTab, tempDetails);
                        if (tab.length === 1) {
                            var smartTab = $(".smart-block", tab);
                            smartBlocks.load({
                                elements: smartTab,
                                onComplete: function () {
                                    setTimeout(function () {
                                        var tabGroup = $("#" + lastTab + " .accordion-group-content", tempDetails);
                                        //tabGroup.removeClass("collapse");
                                        tabGroup.addClass("in");
                                        tabGroup.attr("data-loaded", "true"); //data('loaded', 'true');
                                        replaceDetails();
                                    },
                                        1);
                                }
                            });
                        } else {
                            replaceDetails();
                        }
                    } else {
                        replaceDetails();
                    }
                } else {
                    hideLoader();
                }
            }, 1);
        }
        smartBlocks.doSubmit(url, data, tempDetails, onComplete);
    }

    function init() {

        // Обработчик клика по названию раздела
        $('body').on('click', '.accordion .accordion-group-caption > a', function (event) {
            event.preventDefault();
            var accordion = $(this).closest('.accordion');
            $('.accordion-group-content', accordion).collapse('hide');
            $('.accordion-group-content', $(this).closest('.accordion-group')).collapse('show');
        });

        // Обработчик открытия раздела
        $('body').on('show.bs.collapse', '.accordion .accordion-group-content', function () {
            var me = $(this);
            var loaded = me.data('loaded');
            if (!loaded) {
                // Загрузим содержимое
                accordionLoadGroup(me);

                // Поставим признак загрузки, чтобы не загружать при повторном открытии
                me.data('loaded', 'true');
            }

            // Запомним выбранный элемент
            var id = me.closest('.accordion-group').attr('id');
            setLastTabId(id);

        });

        // Обработчик загрузки accordion
        $('body').on('accordion.loaded', '.accordion', function () {
            var me = $(this);
            var id = getLastTabId();

            if (!id) {
                id = $('.accordion-group', me).first().attr('id');
            }

            $('.accordion-group-content', $('#' + id)).collapse('show');
        });
    }

    init();

    return {
        load: load,
        accordionReloadCurrentGroup: accordionReloadCurrentGroup
    };
})();


$(function () {
    var config = smartConfig.getOrCreate("components-tree-details-panel");
});
