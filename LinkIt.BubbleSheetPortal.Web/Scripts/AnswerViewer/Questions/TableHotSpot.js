/**
 * Table Hot Spot Module
 * @param  {[type]} function (             [description]
 * @return {[type]}          [description]
 */
var TableHotSpot = (function () {
    /**
     * Display xml to html
     * @param  {[type]} xmlContent [description]
     * @return {[type]}            [description]
     */
    function displayHtml (xmlContent) {
        var $tree = $('<div/>');

        $tree.append(xmlContent);

        $tree.find('tableitem').replaceWith(function () {
            var $tableitem = $(this);
            var $newTableitem = $('<span/>');

            Utils.copyAttributes($tableitem, $newTableitem);

            $newTableitem.addClass('tablehotspot');

            return $newTableitem;
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

        if (!Utils.isNullOrEmpty(answer) && $tree.find('[qtischemeid="33"]').length) {
            answer = answer.split(',');

            for (var i = 0, len = answer.length; i < len; i++) {
                var $tablehotspot = $tree.find('[identifier="' + answer[i] + '"]');

                $tablehotspot.addClass('is-selected');
            }
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
