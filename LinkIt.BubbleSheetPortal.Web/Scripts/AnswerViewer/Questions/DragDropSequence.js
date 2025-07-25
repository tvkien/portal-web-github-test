/**
 * Drag And Drop Sequence/Order Module
 * @param  {[type]} function (             [description]
 * @return {[type]}          [description]
 */
var DragDropSequence = (function () {
    /**
     * Display xml to html
     * @param  {[type]} xmlContent [description]
     * @return {[type]}            [description]
     */
    function displayHtml (xmlContent) {
        var $tree = $('<div/>');

        $tree.append(xmlContent);

        $tree.find('partialsequence').replaceWith(function () {
            var $partialsequence = $(this);
            var $newPartialsequence = $('<div/>');

            Utils.copyAttributes($partialsequence, $newPartialsequence);

            $newPartialsequence
                .addClass('dragdrop-sequence')
                .html($partialsequence.html());

            $newPartialsequence.find('sourceitem').replaceWith(function () {
                var $sourceitem = $(this);
                var sourceitemWidth = $sourceitem.attr('width');
                var $newSourceitem = $('<div>/');

                Utils.copyAttributes($sourceitem, $newSourceitem);

                $newSourceitem
                    .addClass('dragdrop-sequence-item')
                    .css('width', sourceitemWidth + 'px')
                    .html($sourceitem.html());

                return $newSourceitem;
            });

            return $newPartialsequence;
        });

        return $tree.html();
    }

    /**
     * Display answer of student
     * @param  {[type]} xmlContent [description]
     * @param  {[type]} answer     [description]
     * @return {[type]}            [description]
     */
    function displayByAnswer (xmlContent, answer) {
        var $tree = $('<div/>');

        $tree.append(xmlContent);

        if (!Utils.isNullOrEmpty(answer) && $tree.find('.dragdrop-sequence').length) {
            answer = answer.split(',');
            var $newSourceItem = $('<div/>');

            for (var i = 0, len = answer.length; i < len; i++) {
                var $dragDropSequenceItem = $tree.find('.dragdrop-sequence-item[identifier="' + answer[i] + '"]');
                $newSourceItem.append($dragDropSequenceItem.clone(true));
            }

            $tree.find('.dragdrop-sequence').html($newSourceItem.html());
        }

        return $tree.html();
    }

    /**
     * Update html answer of student
     * @param  {[type]}  question   [description]
     * @param  {[type]}  xmlContent [description]
     * @param  {Boolean} isCorrect  [description]
     * @return {[type]}             [description]
     */
    function updateAnswerHtml (question, xmlContent, isCorrect) {
        var resultHtml = '';

        if (isCorrect) {
            resultHtml = displayByAnswer(xmlContent, question.CorrectAnswer);
        } else {
            resultHtml = displayByAnswer(xmlContent, question.AnswerText);
        }

        return resultHtml;
    }

    return {
        displayHtml: displayHtml,
        updateAnswerHtml: updateAnswerHtml
    }
})();
