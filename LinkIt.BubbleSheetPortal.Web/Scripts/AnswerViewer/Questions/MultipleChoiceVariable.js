/**
 * Multiple Choice Variable Module
 * @param  {[type]} function (             [description]
 * @return {[type]}          [description]
 */
var MultipleChoiceVariable = (function () {
    /**
     * Display xml to html
     * @param  {[type]} xmlContent [description]
     * @return {[type]}            [description]
     */
    function displayHtml (xmlContent) {
        var $tree = $('<div/>');

        $tree.append(xmlContent);

        $tree.find('choiceinteraction[variablepoints]').replaceWith(function () {
            var $choiceinteraction = $(this);
            var $newChoiceinteraction = $('<div/>');

            Utils.copyAttributes($choiceinteraction, $newChoiceinteraction);

            $newChoiceinteraction
                .addClass('multiplechoice-variable')
                .html($choiceinteraction.html());

            $newChoiceinteraction.find('simplechoice').replaceWith(function () {
                var $simplechoice = $(this);
                var $newSimplechoice = $('<div/>');

                Utils.copyAttributes($simplechoice, $newSimplechoice);

                $newSimplechoice
                    .addClass('multiplechoice-variable-item')
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

        if (!Utils.isNullOrEmpty(answer) && $tree.find('.multiplechoice-variable').length) {
            var isInformationalOnly = $tree.find('responseDeclaration').attr('method') === 'informational-only';
            answer = answer.split(',');

            for (i = 0, len = answer.length; i < len; i++) {
                var $multiplechoiceVariable = $tree.find('.multiplechoice-variable-item[identifier="' + answer[i] + '"]');

                if (isCorrect) {
                    $multiplechoiceVariable.addClass('is-correct');
                } else {
                    $multiplechoiceVariable.addClass('is-answer');
                }

                if (isInformationalOnly) {
                    $multiplechoiceVariable.addClass('is-informational-only');
                }
            }
        }

        return $tree.html();
    }

    function getCorrectAnswer (xmlContent) {
        var $tree = $('<div/>');
        var correctAnswerArr = [];

        $tree.append(xmlContent);

        if ($tree.find('responseDeclaration').attr('method') === 'informational-only') {
            return '';
        }

        $tree.find('responsedeclaration correctresponse > value').each(function (ind, value) {
            var $value = $(value);
            var valuePoint = parseInt($value.text(), 10);
            var valueIdentifier = $value.attr('identifier');

            if (!isNaN(valuePoint) && valuePoint > 0) {
                correctAnswerArr.push(valueIdentifier);
            }
        });

        if (correctAnswerArr.length) {
            return correctAnswerArr.join(',');
        }

        return '';
    }

    /**
     * Update html answer of student
     * @param  {[type]} xmlContent [description]
     * @param  {[type]} answer     [description]
     * @param  {[type]} type       [description]
     * @return {[type]}            [description]
     */
    function updateAnswerHtml (question, xmlContent) {
        var resultHtml = '';

        resultHtml = xmlContent;

        resultHtml = displayByAnswer(resultHtml, question.AnswerChoice);
        resultHtml = displayByAnswer(resultHtml, getCorrectAnswer(resultHtml), true);

        return resultHtml;
    }

    return {
        displayHtml: displayHtml,
        updateAnswerHtml: updateAnswerHtml
    }
})();
