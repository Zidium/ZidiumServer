function processMenu(mainMenu, activeItems) {
    if (activeItems.length > 0) {
        var activeItemsArray = activeItems.split(',');
        for (var i = 0; i < activeItemsArray.length; i++) {
            var menuItem = $('#' + activeItemsArray[i], mainMenu);
            menuItem.addClass('active');
        }
    }
}