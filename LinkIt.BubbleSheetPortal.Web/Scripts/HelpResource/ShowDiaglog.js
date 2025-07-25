function ConfirmDiaglog(options, yesCallBack, noCallBack) {
    if (options.ShowNoButton == null) options.ShowNoButton = true;

    var now = new Date().getTime();
    var contentHtml = '';
    var $dialog = $('<div/>');
    var $doc = $(document);

    // Remove old confirm submit test popup
    $doc.find('[id*=confirmSubmitDialog-]').dialog('destroy').remove();

    contentHtml += '<div class="popup-fullpage">';
    contentHtml += '<div class="popup-fullpage-content">';
    contentHtml += '<p class="popup-fullpage-text">' + options.Message + '</p>';
    contentHtml += '</div>';
    contentHtml += '<div class="popup-fullpage-controls">';
    contentHtml += '<button id="btn-submit-test-yes-' + now + '" class="btn-submit-test">'
    contentHtml += options.YesButtonCaption != null ? options.YesButtonCaption : 'Yes';
    contentHtml += '</button>';
    if (options.ShowNoButton) contentHtml += '<button id="btn-submit-test-no-' + now + '" class="btn-submit-test">No</button>';
    contentHtml += '</div>';
    contentHtml += '</div>';

    var width = options.width != null ? options.with : 460;

    $dialog
        .html(contentHtml)
        .attr('id', 'confirmSubmitDialog-' + now)
        .appendTo('body')
        .dialog({
            modal: true,
            width: width,
            maxheight: 500,
            resizable: false,
            dialogClass: 'ui-popup-fullpage ui-popup-fullpage-nostudent',
            open: function () {
                var $dl = $(this).parents('.ui-dialog');

                if (options.HideCloseButton == 1) {
                    $dl.find('.ui-dialog-titlebar-close').remove();
                }
            }
        });

    $doc.on('click', '#btn-submit-test-yes-' + now, function () {
        $doc.find('#confirmSubmitDialog-' + now).dialog('destroy').remove();
        if (typeof yesCallBack === 'function') {
            yesCallBack(options);
        }
    });

    if (options.ShowNoButton) {
        $doc.on('click', '#btn-submit-test-no-' + now, function () {
            $doc.find('#confirmSubmitDialog-' + now).dialog('destroy').remove();
            if (typeof noCallBack === 'function') {
                noCallBack(options);
            }
        });
    }
}