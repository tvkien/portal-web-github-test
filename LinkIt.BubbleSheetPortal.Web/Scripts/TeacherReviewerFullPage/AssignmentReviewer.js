(function($) {
    'use strict';

    // Get height max assignment note
    getHeightAssignmentNote();

    // Select custom filter student
    $('#assignment-filter-student').select2({
        minimumResultsForSearch: Infinity,
        containerCssClass: 'assignment-select',
        dropdownCssClass: 'assignment-select-dropdown'
    });

    // Add scrollbar for safari on teacher reviewer popup
    var browserUserAgent = navigator.userAgent.toString().toLowerCase();
    var isSafari = (browserUserAgent.indexOf('safari') != -1) && (browserUserAgent.indexOf('chrome') == -1);

    if (isSafari) {
        $('#divQuestionMenu, #divSectionInstruction, .assignment-desc-question').addClass('scrollbar-safari');
    }

    function getHeightAssignmentNote() {
        var maxHeightAssignNote = 0;
        var arrAssignNote = [];

        $('.assignment-note').each(function(ind, assignnote) {
            var $assignnote = $(assignnote);

            arrAssignNote.push($assignnote.height());
        });

        maxHeightAssignNote = Math.max.apply(null, arrAssignNote);

        $('.assignment-note').css('height', maxHeightAssignNote + 'px');
    }
}(jQuery));

jQuery.fn.outerHTML = function (s) {
    return (s)
        ? this.before(s).remove()
        : jQuery("<p>").append(this.eq(0).clone()).html();
};

(function ($) {
    $.fn.imagesLoaded = function (callback) {
        var elems = this.filter('img'),
            len = elems.length,
            // data uri bypasses webkit log warning (thx doug jones (cjboco))
            blank = "data:image/gif;base64,R0lGODlhAQABAIAAAAAAAP///ywAAAAAAQABAAACAUwAOw==";
        elems.bind('load', function () {
            // check image src to prevent firing twice (thx doug jones (cjboco))
            if (--len <= 0 && this.src !== blank) {
                callback.call(elems, this);
            }
        }).each(function () {
            // cached images don't fire load sometimes, so we reset src.
            if (this.complete || this.complete === undefined) {
                var src = this.src;
                // webkit hack from http://groups.google.com/group/jquery-dev/browse_thread/thread/eee6ab7b2da50e1f
                this.src = blank;
                this.src = src;
            }
        });
    };
}(jQuery));

function CopyAttributes(from, to) {
    var attrs = from.prop("attributes");
    $.each(attrs, function (index, attribute) {
        to.attr(attribute.name, attribute.value);
    });
}

function ConfirmSubmitTest(options, yesCallBack, noCallBack) {
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
    contentHtml += '<button id="btn-submit-test-yes-' + now + '" class="btn-submit-test">Yes</button>';
    contentHtml += '<button id="btn-submit-test-no-' + now + '" class="btn-submit-test">No</button>';
    contentHtml += '</div>';
    contentHtml += '</div>';

    $dialog
        .html(contentHtml)
        .attr('id', 'confirmSubmitDialog-' + now)
        .appendTo('body')
        .dialog({
            modal: true,
            width: 460,
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

    $doc.on('click', '#btn-submit-test-yes-' + now , function() {
        $doc.find('#confirmSubmitDialog-' + now).dialog('destroy').remove();
        if (typeof yesCallBack === 'function') {
            yesCallBack(options);
        }
    });

    $doc.on('click', '#btn-submit-test-no-' + now , function() {
        $doc.find('#confirmSubmitDialog-' + now).dialog('destroy').remove();
        if (typeof noCallBack === 'function') {
            noCallBack(options);
        }
    });
}

function CopyAttributes(from, to) {
    var attrs = from.prop("attributes");
    $.each(attrs, function (index, attribute) {
        to.attr(attribute.name, attribute.value);
    });
}

function ConfirmWarningMessage(options) {
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
    contentHtml += '<button id="btn-ok-' + now + '" class="btn-submit-test">OK</button>';
    contentHtml += '</div>';
    contentHtml += '</div>';

    $dialog
        .html(contentHtml)
        .attr('id', 'confirmSubmitDialog-' + now)
        .appendTo('body')
        .dialog({
            modal: true,
            width: 460,
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

    $doc.on('click', '#btn-ok-' + now, function () {
        $doc.find('#confirmSubmitDialog-' + now).dialog('destroy').remove();        
    });
}
