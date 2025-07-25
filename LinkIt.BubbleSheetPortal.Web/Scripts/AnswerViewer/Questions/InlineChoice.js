/**
 * Inline Choice Module
 * @param  {[type]} function (             [description]
 * @return {[type]}          [description]
 */
var InlineChoice = (function () {
    /**
     * Display xml to html
     * @param  {[type]} xmlContent [description]
     * @return {[type]}            [description]
     */
    function displayHtml (xmlContent) {
        var $tree = $('<div/>');

        $tree.append(xmlContent);

        $tree.find('inlinechoiceinteraction').replaceWith(function () {
            var $inlinechoice = $(this);
            var $newInlinechoice = $('<div/>');

            Utils.copyAttributes($inlinechoice, $newInlinechoice);

            $newInlinechoice
                .addClass('inlinechoice')
                .html($inlinechoice.html());

            $newInlinechoice.find('inlinechoice').replaceWith(function () {
                var $inlinechoiceItem = $(this);
                var $newInlinechoiceItem = $('<div/>');

                Utils.copyAttributes($inlinechoiceItem, $newInlinechoiceItem);

                $newInlinechoiceItem
                    .addClass('inlinechoice-item')
                    .html($inlinechoiceItem.html());

                return $newInlinechoiceItem;
            });

            return $newInlinechoice;
        });

        return $tree.html();
    }

    /**
     * Display answer of student
     * @param  {[type]}  xmlContent [description]
     * @param  {[type]}  answer     [description]
     * @param  {Boolean} isCorrect  [description]
     * @return {[type]}             [description]
     */
    function displayByAnswer (xmlContent, answer, isCorrect) {
        var $tree = $('<div/>');

        $tree.append(xmlContent);

        if (!Utils.isNullOrEmpty(answer) && $tree.find('.inlinechoice').length) {
            answer = answer.split(',');

            for (i = 0, len = answer.length; i < len; i++) {
                var $inlinechoice = $tree.find('.inlinechoice-item[identifier="' + answer[i] + '"]');

                if (isCorrect) {
                    $inlinechoice.addClass('is-correct');
                } else {
                    $inlinechoice.addClass('is-answer');
                }
            }
        }

        return $tree.html();
    }

    /**
     * Display answer of student multipart question
     * @param  {[type]}  xmlContent [description]
     * @param  {[type]}  answerSub  [description]
     * @param  {Boolean} isCorrect  [description]
     * @return {[type]}             [description]
     */
    function displayByAnswerMultipart (xmlContent, answerSub, isCorrect) {
        var $tree = $('<div/>');

        $tree.append(xmlContent);

        if (answerSub.length && $tree.find('.inlinechoice').length) {
            for (var i = 0, len = answerSub.length; i < len; i++) {
                var answerSubItem = answerSub[i];

                $tree.find('.inlinechoice').each(function (ind, inlinechoice) {
                    var $inlinechoice = $(inlinechoice);
                    var inlinechoiceResponseId = $inlinechoice.attr('responseidentifier');

                    if (answerSubItem.ResponseIdentifier === inlinechoiceResponseId &&
                        !Utils.isNullOrEmpty(answerSubItem.AnswerChoice)) {
                        var answerSubItemAnswerChoice = answerSubItem.AnswerChoice.split(',');

                        for (var j = 0, jlen = answerSubItemAnswerChoice.length; j < jlen; j++) {
                            var $inlinechoiceItem = $inlinechoice.find('.inlinechoice-item[identifier="' + answerSubItemAnswerChoice[j] + '"]');

                            if (isCorrect) {
                                $inlinechoiceItem.addClass('is-correct');
                            } else {
                                $inlinechoiceItem.addClass('is-answer');
                            }
                        }
                    }
                })
            }
        }

        return $tree.html();
    }

    /**
     * Update html answer of student
     * @param  {[type]} question   [description]
     * @param  {[type]} xmlContent [description]
     * @return {[type]}            [description]
     */
    function updateAnswerHtml (question, xmlContent) {
        var resultHtml = '';

        resultHtml = xmlContent;

        if (question.QTISchemaID === 21) {
            if (Utils.isArray(question.TestOnlineSessionAnswerSubs)) {
                resultHtml = displayByAnswerMultipart(resultHtml, question.TestOnlineSessionAnswerSubs);
            }
            resultHtml = displayByAnswerMultipart(resultHtml, Multipart.getCorrectAnswerSubs(xmlContent), true);
        } else {
            resultHtml = displayByAnswer(resultHtml, question.AnswerChoice);
            resultHtml = displayByAnswer(resultHtml, question.CorrectAnswer, true);
        }

        return resultHtml;
    }

    return {
        displayHtml: displayHtml,
        updateAnswerHtml: updateAnswerHtml
    }
})();
