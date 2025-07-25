(function ($) {
    var $doc = $(document);
    var $divQuestionDetails = $('#divQuestionDetails');

    $doc.on('click', 'span.glossary', function() {
        var $glossary = $(this);

        if (!$glossary.parents('#divQuestionMenu').length) {
            var glossaryText = $glossary.html();
            var glossaryContent = $glossary.attr('glossary')
                                            .replace(/&lt;br\/&gt;/gi, '<br/>')
                                            .replace(/&gt;/g, '>')
                                            .replace(/&lt;/g, '<');
            var glossaryHtml = '';
            var $glossaryPopup = $('<div/>');

            glossaryHtml += '<h4 class="glossary_text">' + glossaryText + '</h4>';
            glossaryHtml += '<div class="glossary_define">';
            glossaryHtml += glossaryContent;
            glossaryHtml += '</div>';

            $glossaryPopup.append(glossaryHtml);
            $glossaryPopup.find('span').removeAttr('style');
            $glossaryPopup.find('.highlighted').removeClass('highlighted');

            glossaryHtml = $glossaryPopup.html();

            Reviewer.popupAlertMessage(glossaryHtml, 'ui-popup-fullpage', 400, 400);
        }
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
