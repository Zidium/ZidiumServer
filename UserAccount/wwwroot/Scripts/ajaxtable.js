function AddedTableRow(id, rowurl, tablebodyid) {
    var row = $('<div />');
    $('#' + tablebodyid).prepend(row);
    UpdateTableRow(row, id, rowurl);
}

function ChangedTableRow(id, rowurl, rowprefix, tablebodyid) {
    var row = $('#' + rowprefix + id, $('#' + tablebodyid));
    UpdateTableRow(row, id, rowurl);
}

function UpdateTableRow(row, id, rowurl) {
    var url = rowurl + '/' + id;
    $.ajax({
        type: "GET",
        url: url,
        error: onAjaxFailure,
        success: function (data) {
            row.replaceWith(data);
            onAjaxSuccess();
        }
    });
}

function DeletedTableRow(id, rowprefix, tablebodyid) {
    var row = $('#' + rowprefix + id, $('#' + tablebodyid));
    row.remove();
}