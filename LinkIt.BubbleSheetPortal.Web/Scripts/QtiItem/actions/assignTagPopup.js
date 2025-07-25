function showAssignTagPopup(props) {
    var selectedQtiItemIds = props?.qtiItemId;
    var worker = $('<div />');
    worker
        .addClass("dialog TagPopUpDialogCSS")
        .attr("id", "addTagDialog")
        .appendTo("body")
        .load('/QTIItem/ShowTagPopup?qtiItemIdString=' + selectedQtiItemIds, function () {
            worker.dialog({
                open: function () {
                    $('#tips').html('');
                },
                title: $(this).attr("Tag"),
                close: function () {
                    selectStateId_dt = $('#selectState').val();
                    if (selectStateId_dt == null || selectStateId_dt == '' || selectStateId_dt == 'select' || selectStateId_dt == 'All') {
                        selectStateId_dt = 0;
                    }
                    selectDistrictId_dt = $('#selectDistrict').val();
                    if (selectDistrictId_dt == null || selectDistrictId_dt == '' || selectDistrictId_dt == 'select' || selectDistrictId_dt == 'All') {
                        selectDistrictId_dt = '';
                    }
                    selectCategoryId_dt = $('#selectCategory').val();
                    if (selectCategoryId_dt == null || selectCategoryId_dt == '' || selectCategoryId_dt == 'select' || selectCategoryId_dt == 'All') {
                        selectCategoryId_dt = '';
                    }
                    textToSearch_dt = $('#txtTextToSearch').val();

                    $('.ui-widget-overlay').remove();
                    $(this).remove();
                    $('#tips').html('');
                    $('#qtiItemDataTable').dataTable().fnDraw(false);
                },
                modal: false,
                width: 980,
                resizable: false
            });
        });
    showModalDialogBG();
}

function showModalDialogBG() {
    var win = $('body');
    $('body').prepend('<div class="ui-widget-overlay" style="width: ' + win.width() + 'px; height: ' + win.height() + 'px; z-index: 1001;"></div>');
}