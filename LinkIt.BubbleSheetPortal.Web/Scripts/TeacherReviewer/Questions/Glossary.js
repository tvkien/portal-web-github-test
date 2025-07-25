(function ($) {
    var $doc = $(document);
    var $divQuestionDetails = $('#divQuestionDetails');

    $doc.on('click', 'span.glossary', function (e) {
        var $self = $(this);
        var $glossaryMessage = $('#glossaryMessage');
        var glossary_text = $self.html();
        var glossary_content = $self.attr('glossary')
                                        .replace(/&lt;br\/&gt;/gi, '<br/>')
                                        .replace(/&gt;/g, '>')
                                        .replace(/&lt;/g, '<');
        var win = $(document);
        var z_index = parseInt($self.parents('.ui-dialog').css('z-index'));

        $glossaryMessage.find('.glossary_text').html(glossary_text);
        $glossaryMessage.find('.glossary_define').html(glossary_content);

        $('body').prepend('<div class="ui-widget-overlay" style="width: ' + win.width() + 'px; height: ' + win.height() + 'px; z-index:' + (z_index + 1) + ';"></div>');

        $glossaryMessage.dialog({
            modal: false,
            width: 480,
            resizable: false,
            open: function (dialog) {
                $glossaryMessage.prev().css('top', '37px');
            },
            close: function () {
                $('.ui-widget-overlay:first').remove();
            }
        });
    }).on({
        mouseenter: function () {
            var currentID = $(this).attr('glossary_id');
            $divQuestionDetails.find('span.glossary[glossary_id=' + currentID + ']').addClass('glossary-hover');
        },
        mouseleave: function () {
            var currentID = $(this).attr('glossary_id');
            $divQuestionDetails.find('span.glossary[glossary_id=' + currentID + ']').removeClass('glossary-hover');
        }
    }, 'span.glossary');

}(jQuery));
