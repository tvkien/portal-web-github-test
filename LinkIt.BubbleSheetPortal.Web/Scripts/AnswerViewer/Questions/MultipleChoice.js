/**
 * Multiple Choice Module
 * @param  {[type]} function (             [description]
 * @return {[type]}          [description]
 */
var MultipleChoice = (function () {
    /**
     * Display xml to html
     * @param  {[type]} xmlContent [description]
     * @return {[type]}            [description]
     */
    function displayHtml (xmlContent) {
        var $tree = $('<div/>');

        $tree.append(xmlContent);

        $tree.find('choiceinteraction').not('[variablepoints]').replaceWith(function () {
            var $choiceinteraction = $(this);
            var $newChoiceinteraction = $('<div/>');

            Utils.copyAttributes($choiceinteraction, $newChoiceinteraction);

            $newChoiceinteraction
                .addClass('multiplechoice')
                .html($choiceinteraction.html());

            $newChoiceinteraction.find('simplechoice').replaceWith(function () {
                var $simplechoice = $(this);
                var $newSimplechoice = $('<div/>');

                Utils.copyAttributes($simplechoice, $newSimplechoice);

                $newSimplechoice
                    .addClass('multiplechoice-item')
                    .html($simplechoice.html());

                return $newSimplechoice;
            });

            return $newChoiceinteraction;
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

        if (!Utils.isNullOrEmpty(answer) && $tree.find('.multiplechoice')) {
            var isInformationalOnly = $tree.find('responseDeclaration').attr('method') === 'informational-only';

            if (answer.indexOf(';') > 0) {
                // For correct answer multi-select
                answer = answer.split(';');
            } else {
                answer = answer.split(',');
            }

            for (i = 0, len = answer.length; i < len; i++) {
                var $multiplechoice = $tree.find('.multiplechoice-item[identifier="' + answer[i] + '"]');

                if (isCorrect) {
                    $multiplechoice.addClass('is-correct');
                } else {
                    $multiplechoice.addClass('is-answer');
                }

                if (isInformationalOnly) {
                    $multiplechoice.addClass('is-informational-only');
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

        if (answerSub.length && $tree.find('.multiplechoice').length) {
            for (var i = 0, len = answerSub.length; i < len; i++) {
                var answerSubItem = answerSub[i];

                $tree.find('.multiplechoice').each(function (ind, multiplechoice) {
                    var $multiplechoice = $(multiplechoice);
                    var multiplechoiceResponseId = $multiplechoice.attr('responseidentifier');

                    if (answerSubItem.ResponseIdentifier === multiplechoiceResponseId &&
                        !Utils.isNullOrEmpty(answerSubItem.AnswerChoice)) {
                        var answerSubItemAnswerChoice = answerSubItem.AnswerChoice.split(',');

                        for (var j = 0, jlen = answerSubItemAnswerChoice.length; j < jlen; j++) {
                            var $multiplechoiceItem = $multiplechoice.find('.multiplechoice-item[identifier="' + answerSubItemAnswerChoice[j] + '"]');

                            if (isCorrect) {
                                $multiplechoiceItem.addClass('is-correct');
                            } else {
                                $multiplechoiceItem.addClass('is-answer');
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
