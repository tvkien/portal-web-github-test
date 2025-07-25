/**
 * Text Hot Spot Module
 * @param  {[type]} function (             [description]
 * @return {[type]}          [description]
 */
var TextHotSpot = (function () {
    /**
     * Update xml to html
     * @param  {[type]} xmlContent [description]
     * @return {[type]}            [description]
     */
    function displayHtml (xmlContent) {
        var $tree = $('<div/>');

        $tree.append(xmlContent);

        $tree.find('sourcetext').replaceWith(function () {
            var $sourcetext = $(this);
            var $newSourcetext = $('<span/>');

            Utils.copyAttributes($sourcetext, $newSourcetext);

            $newSourcetext
                .addClass('texthotspot')
                .html($sourcetext.html());

            return $newSourcetext;
        });

        return $tree.html();
    }

    /**
     * Display answer of student
     * @param  {[type]} xmlContent [description]
     * @param  {[type]} answer     [description]
     * @return {[type]}            [description]
     */
    function displayByAnswer(xmlContent, answer) {
        var $tree = $('<div/>');

        $tree.append(xmlContent);

        if (!Utils.isNullOrEmpty(answer) && $tree.find('.texthotspot').length) {
            answer = answer.split(',');

            for (var i = 0, len = answer.length; i < len; i++) {
                $texthotspot = $tree.find('.texthotspot[identifier="' + answer[i] + '"]');
                $texthotspot.addClass('is-selected');
            }
        }

        return $tree.html();
    }

    /**
     * Update html answer of student
     * @param  {[type]} xmlContent [description]
     * @param  {[type]} answer     [description]
     * @return {[type]}            [description]
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
