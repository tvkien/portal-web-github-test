function showStandardPopup(props) {
    var qtiItemId = props?.qtiItemId;
    if (!qtiItemId) {
        return;
    }

    var url = ""
    //If user has selected more than one qti item to assign standard
    if (qtiItemId.length > 0) {
        url = '/QTIItem/ShowStandardPopupForManyQtiItem?qtiItemIdString=' + (qtiItemId || []).join(',');
    } else {
        url = '/QTIItem/ShowStandardPopup?qtiItemId=' + qtiItemId;
    }

    var worker = $('<div />');
    worker
        .addClass("dialog StandardPopUpDialogCSS")
        .attr("id", "addStandardDialog")
        .appendTo("body")
        .load(url, function () {
            worker.dialog({
                open: function () {
                    $('#tips').html('');
                    $("#selectState").marquee();
                    $("#selectSubject").marquee();
                    $("#selectGrade").marquee();
                },
                title: $(this).attr("Standard"),
                close: function () {
                    $('.ui-widget-overlay').remove();//will be remove when table display completelly
                    selectStateId = $('#selectState').val();
                    if (selectStateId == null || selectStateId == '' || selectStateId == 'select' || selectStateId == 'All') {
                        selectStateId = 0;
                    }
                    selectSubjectId = $('#selectSubject').val();
                    if (selectSubjectId == null || selectSubjectId == '' || selectSubjectId == 'select' || selectSubjectId == 'All') {
                        selectSubjectId = '';
                    }
                    selectGradeId = $('#selectGrade').val();
                    if (selectGradeId == null || selectGradeId == '' || selectGradeId == 'select' || selectGradeId == 'All') {
                        selectGradeId = '';
                    }

                    $(this).remove();
                    $('#tips').html('');
                    $('#qtiItemDataTable').dataTable().fnDraw(false);
                },
                modal: false,
                width: 905,
                resizable: false
            });
        });
    showModalDialogBG();
}

function showModalDialogBG() {
    var win = $('body');
    $('body').prepend('<div class="ui-widget-overlay" style="width: ' + win.width() + 'px; height: ' + win.height() + 'px; z-index: 1001;"></div>');
}