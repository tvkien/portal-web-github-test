/**
 * Extended Text (Open Ended) Module
 * @param  {[type]} function (             [description]
 * @return {[type]}          [description]
 */
var ExtendedText = (function () {
    /**
     * Display xml to html
     * @param  {[type]} xmlContent [description]
     * @return {[type]}            [description]
     */
    function displayHtml (xmlContent) {
        var $tree = $('<div/>');

        $tree.append(xmlContent);

        $tree.find('extendedtextinteraction').not('[drawable]').replaceWith(function () {
            var $extendedtext = $(this);
            var $newextEndedtext = $('<div/>');

            Utils.copyAttributes($extendedtext, $newextEndedtext);

            $newextEndedtext
                .addClass('extendedtext')
                .html($extendedtext.html());

            return $newextEndedtext;
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
    function displayByAnswer (xmlContent, answer) {
        var $tree = $('<div/>');

        $tree.append(xmlContent);

        if (!Utils.isNullOrEmpty(answer) && $tree.find('.extendedtext').length) {
            if (answer.indexOf('&#60;') > -1 && answer.indexOf('&#62;') > -1) {
                answer = Utils.replaceStringLessOrLarge(answer);
            } else {
                answer = Utils.replaceParagraph(answer);
            }

            $tree.find('.extendedtext').html(answer);
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
    function displayByAnswerMultipart (xmlContent, answerSub) {
        var $tree = $('<div/>');

        $tree.append(xmlContent);

        if (answerSub.length && $tree.find('.extendedtext').length) {
            for (var i = 0, len = answerSub.length; i < len; i++) {
                var answerSubItem = answerSub[i];

                $tree.find('.extendedtext').each(function (ind, extendedtext) {
                    var $extendedtext = $(extendedtext);
                    var extendedtextResponseId = $extendedtext.attr('responseidentifier');

                    if (answerSubItem.ResponseIdentifier === extendedtextResponseId &&
                        !Utils.isNullOrEmpty(answerSubItem.AnswerText)) {
                        var answerSubItemAnswerText = answerSubItem.AnswerText;

                        if (answerSubItemAnswerText.indexOf('&#60;') > -1 && answerSubItemAnswerText.indexOf('&#62;') > -1) {
                            answerSubItemAnswerText = Utils.replaceStringLessOrLarge(answerSubItemAnswerText);
                        } else {
                            answerSubItemAnswerText = Utils.replaceParagraph(answerSubItemAnswerText);
                        }

                        $extendedtext.html(answerSubItemAnswerText);
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
        } else {
            resultHtml = displayByAnswer(resultHtml, question.AnswerText);
        }

        return resultHtml;
    }

    return {
        displayHtml: displayHtml,
        updateAnswerHtml: updateAnswerHtml
    }
})();
