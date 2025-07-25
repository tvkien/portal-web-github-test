/**
 * Drag And Drop Numerical
 * @param  {[type]} function (             [description]
 * @return {[type]}          [description]
 */
var DragDropNumerical = (function () {

    /**
     * Display answer of student
     * @param  {[type]} xmlContent [description]
     * @param  {[type]} answer     [description]
     * @return {[type]}            [description]
     */
    function displayByAnswer (answer) {
        var $tree = $('<div/>');

        if (!Utils.isNullOrEmpty(answer)) {
            var ddNumericalContainer = document.createElement('div');

            ddNumericalContainer.innerHTML = answer;

            $tree.append(ddNumericalContainer);
        }

        return $tree.html();
    }

    /**
     * Update html answer of student
     * @param  {[type]} xmlContent [description]
     * @param  {[type]} answer     [description]
     * @return {[type]}            [description]
     */
     function updateAnswerHtml (question, xmlContent) {
         var resultHtml = '';

         resultHtml = displayByAnswer(question.CorrectAnswer);

         return resultHtml;
     }

    return {
        updateAnswerHtml: updateAnswerHtml
    }
})();
