function showAddPassagePopup(props) {
    var selectedQtiItemIds = props?.qtiItemId;
    var onClose = props?.onClose;
    var worker = $('<div />');
    worker
        .addClass("dialog PassagePopUpDialogCSS")
        .attr("id", "addPassageDialog")
        .appendTo("body")
        .load('/QTIItem/ShowPassagePopupForManyQtiItem?selectedQtiItemId=' + selectedQtiItemIds, function () {
            worker.dialog({
                open: function () {
                    $('#tips').html('');
                },
                title: $(this).attr("Standard"),
                close: function () {
                    $('.ui-widget-overlay').remove();
                    $(this).remove();
                    $('#tips').html('');
                    if (onClose && typeof onClose === 'function') {
                        onClose();
                    }
                    //reload qtiItem list
                    $('#qtiItemDataTable').dataTable().fnDraw(false);
                },
                modal: false,
                width: 1200,
                resizable: false
            });
        });
    showModalDialogBG();
}