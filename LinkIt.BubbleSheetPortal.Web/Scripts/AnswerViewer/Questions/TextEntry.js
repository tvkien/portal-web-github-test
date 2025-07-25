/**
 * Text Entry Module
 * @param  {[type]} function (             [description]
 * @return {[type]}          [description]
 */
var TextEntry = (function () {
    /**
     * Display xml to html
     * @param  {[type]} xmlContent [description]
     * @return {[type]}            [description]
     */
    function displayHtml (xmlContent) {
        var $tree = $('<div/>');

        $tree.append(xmlContent);

        $tree.find('textentryinteraction').replaceWith(function () {
            var $textentry = $(this);
            var $newTextentry = $('<div/>');

            Utils.copyAttributes($textentry, $newTextentry);

            $newTextentry
                .addClass('textentry')
                .html($textentry.html());

            return $newTextentry;
        });

        return $tree.html();
    }

    /**
     * Display answer of student
     * @param  {[type]} xmlContent [description]
     * @param  {[type]} answer     [description]
     * @return {[type]}            [description]
     */
    function displayByAnswer (xmlContent, answer, isCorrect, question) {
        var $tree = $('<div/>');

        $tree.append(xmlContent);

        if (!Utils.isNullOrEmpty(answer) && $tree.find('.textentry').length) {
            var $textentry = $tree.find('.textentry');
            var textentryResponseId = $textentry.attr('responseidentifier');

            if (isCorrect) {
                if (question.PointsEarned !== question.PointsPossible) {
                    var textentryContainer = updateCorrectAnswer(xmlContent, textentryResponseId);
                    $(textentryContainer).insertAfter($textentry);
                }
            } else {
                $textentry.html(answer);
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
    function displayByAnswerMultipart (xmlContent, answerSub, isCorrect, question) {
        var $tree = $('<div/>');

        $tree.append(xmlContent);

        if (answerSub.length && $tree.find('.textentry').length) {
            var testAnswerSubs = question.TestOnlineSessionAnswerSubs;

            for (var i = 0, len = answerSub.length; i < len; i++) {
                var answerSubItem = answerSub[i];

                $tree.find('.textentry').each(function (ind, textentry) {
                    var $textentry = $(textentry);
                    var textentryResponseId = $textentry.attr('responseidentifier');

                    if (answerSubItem.ResponseIdentifier === textentryResponseId &&
                        !Utils.isNullOrEmpty(answerSubItem.AnswerText)) {

                        if (isCorrect) {
                            if (Utils.isArray(testAnswerSubs)) {
                                var textentryAnswer = R.filter(R.propEq('ResponseIdentifier', textentryResponseId))(testAnswerSubs)[0];

                                if (textentryAnswer.PointsEarned !== textentryAnswer.PointsPossible) {
                                    var textentryContainer = updateCorrectAnswer(xmlContent, textentryResponseId);
                                    $(textentryContainer).insertAfter($textentry);
                                }
                            } else {
                                var textentryContainer = updateCorrectAnswer(xmlContent, textentryResponseId);
                                $(textentryContainer).insertAfter($textentry);
                            }
                        } else {
                            $textentry.html(answerSubItem.AnswerText);
                        }
                    }
                })
            }
        }

        return $tree.html();
    }

    /**
     * Update correct answer
     * @param  {[type]} xmlContent          [description]
     * @param  {[type]} textentryResponseId [description]
     * @return {[type]}                     [description]
     */
    function updateCorrectAnswer (xmlContent, textentryResponseId) {
        var $tree = $('<div/>');
        var textentryContainer = '';
        var greaterThanOrEqual = '&#8805;';
        var lessThanOrEqual = '&#8804;';

        $tree.append(xmlContent);

        $tree.find('.textentry').each(function (index, textentry) {
            var $textentry = $(textentry);
            var $textentryResponse = $tree.find('responsedeclaration[identifier="' + textentryResponseId + '"]');
            var textentryCorrect = [];
            var textentryCorrectHtml = '';

            if ($textentryResponse.length) {
                $textentryResponse.find('correctresponse > value').each(function (ind, value) {
                    var $value = $(value);
                    var startValue = '';
                    var endValue = '';
                    var startExclusivity = '';
                    var endExclusivity = '';
                    var valueCorrectAnswer = '';

                    $value.find('rangevalue').each(function () {
                        var $rangevalue = $(this);

                        if ($rangevalue.find('name').text() === 'start') {
                            startExclusivity = $rangevalue.find('exclusivity').text();
                            startValue = $rangevalue.find('value').text();
                        } else if ($rangevalue.find('name').text() === 'end') {
                            endExclusivity = $rangevalue.find('exclusivity').text();
                            endValue = $rangevalue.find('value').text();
                        }
                    });

                    var startOperator = startExclusivity == '1' ? '>' : greaterThanOrEqual;
                    var endOperator = endExclusivity == '1' ? '<' : lessThanOrEqual;

                    if (startValue != '') {
                        valueCorrectAnswer = startOperator + ' ' + startValue;
                        if (endValue != '') {
                            valueCorrectAnswer = valueCorrectAnswer + ' and ' + endOperator + ' ' + endValue;
                        }
                    } else {
                        if (endValue != '') {
                            valueCorrectAnswer = valueCorrectAnswer + ' ' + endOperator + ' ' + endValue;
                        }
                    }

                    if (valueCorrectAnswer === '') {
                        valueCorrectAnswer = $value.html();
                    }

                    textentryCorrect.push(valueCorrectAnswer);
                });

                textentryCorrectHtml = textentryCorrect.join('<br/>');

                textentryContainer = displayAnswerTemplate(textentryCorrectHtml);
            }
        });

        return textentryContainer;
    }

    /**
     * Display answer template
     * @param  {[type]} answer [description]
     * @return {[type]}        [description]
     */
    function displayAnswerTemplate (answer) {
        var badgeContainer = document.createElement('div');
        var badgeHeader = document.createElement('div');
        var badgeContent = document.createElement('div');

        badgeHeader.className = 'app-badge-header';
        badgeHeader.innerHTML = 'Correct answer.';

        badgeContent.className = 'app-badge-content';
        badgeContent.innerHTML = answer;

        badgeContainer.className = 'app-badge';
        badgeContainer.appendChild(badgeHeader);
        badgeContainer.appendChild(badgeContent);
        return badgeContainer;
    }

    /**
     * Display answer single fill in the blank
     * @param  {[type]} question [description]
     * @return {[type]}          [description]
     */
    function displayByAnswerSingle (question) {
        var $tree = $('<div/>');

        $tree.append(question.XmlContent);

        var $textentry = $tree.find('.textentry');
        var textentryResponseId = $textentry.attr('responseidentifier');
        var textentryContainer = '';

        if (question.PointsEarned !== question.PointsPossible) {
            textentryContainer = updateCorrectAnswer(question.XmlContent, textentryResponseId);
        }

        if (textentryContainer !== '') {
            var $div = $('<div/>');
            var correctTextEntry = '';
            $div.append(textentryContainer);
            correctTextEntry = $div.find('.app-badge-content').html();

            return correctTextEntry;
        }

        return textentryContainer;
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
                resultHtml = displayByAnswerMultipart(resultHtml, question.TestOnlineSessionAnswerSubs, false, question);
            }
            resultHtml = displayByAnswerMultipart(resultHtml, Multipart.getCorrectAnswerSubs(xmlContent), true, question);
        } else {
            resultHtml = displayByAnswer(resultHtml, question.AnswerText, false, question);
        }

        return resultHtml;
    }

    return {
        displayHtml: displayHtml,
        displayByAnswerSingle: displayByAnswerSingle,
        updateAnswerHtml: updateAnswerHtml,
    }
})();
