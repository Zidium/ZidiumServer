var reloadedTiles = (function () {
    
    function reload(selector) {
        var tilesMetadatas = $(selector);
        var tiles = [];
        tilesMetadatas.each(function (index, element) {
            var tile = {};
            tile.url = $(element).data("url");
            tile.hash = $(element).data("hash");
            tile.element = $(element);
            tiles.push(tile);
        });
        var data = {
            tiles: []
        };
        tiles.forEach(function (tile) {
            data.tiles.push({
                url: tile.url,
                hash: tile.hash
            });
        });

        var onErrorMessage = function (errorMessage) {
            //alert("ajax error: " + errorMessage); // todo
        };
        var onSuccess = function (response) {
            if (!response.success) {
                onErrorMessage(response.errorMessage);
            }
            else {
                var newTiles = response.data.tiles;
                newTiles.forEach(function (newTile) {
                    if (newTile.html) {
                        var oldTile = null;
                        tiles.forEach(function (old) {
                            if (old.url == newTile.url) {
                                oldTile = old;
                            }
                        });
                        if (oldTile) {
                            oldTile.element.data("hash", newTile.hash);
                            var oldContent = $(oldTile.element).next();
                            oldContent.replaceWith(newTile.html);
                        }
                    }                    
                });
            }
        };
        var onError = function (jqXhr, textStatus, errorThrown) {
            onErrorMessage(textStatus);
      };
      console.log('GetChanged', data);
        $.ajax({
            url: "/ReloadedTiles/GetChanged",
            type: "POST",
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: JSON.stringify(data),
            success: onSuccess,
            error: onError
        });
    }

    return {
        reload: reload        
    };
})();