/**
 * Draw Interaction Module
 * @param  {[type]} function (             [description]
 * @return {[type]}          [description]
 */
var DrawInteraction = (function () {
    /**
     * Display xml to html
     * @param  {[type]} xmlContent [description]
     * @return {[type]}            [description]
     */
    function displayHtml (xmlContent) {
        var $tree = $('<div/>');

        $tree.append(xmlContent);

        $tree.find('extendedtextinteraction[drawable], img[drawable]').each(function () {
            var $element = $(this).clone();
            var $container = $('<div/>');
            var width = $element.attr('width');
            var height = $element.attr('height');

            Utils.copyAttributes($element, $container);
        
            $container.css({
                width: width + 'px',
                height: height + 'px'
            }).addClass('extendedtext-drawable');
        
            if ($element.is('extendedtextinteraction')) {
                $container.html($element.html());
            } else {
                $container.append($element);
            }
        
            $(this).replaceWith($container);
        });
        return $tree.html();
    }

    /**
     * [displayByAnswer description]
     * @param  {[type]} xmlContent [description]
     * @param  {[type]} answer     [description]
     * @return {[type]}            [description]
     */
    function displayByAnswer (xmlContent, answer) {
        var $tree = $('<div/>');

        $tree.append(xmlContent);

        if (!Utils.isNullOrEmpty(answer) && $tree.find('.extendedtext-drawable').length) {
            var $divDrawAnswer = $('<div/>');

            $divDrawAnswer.append(answer);

            $tree.find('.extendedtext-drawable').each(function (ind, drawable) {
                if($(this).attr('drawable') == 'false') return;
                var $drawable = $(drawable);
                var $imgDrawable = $('<img/>');

                $imgDrawable.attr('src', $divDrawAnswer.find('drawimg').eq(ind).attr('data'));

                if (!Utils.isNullOrEmpty($imgDrawable.attr('src'))) {
                    $drawable.append($imgDrawable);
                }
            });
        }

        return $tree.html();
    }

    function displayByAnswerMultipart (xmlContent, answerSub) {}

    /**
     * Update html answer of student
     * @param  {[type]} question   [description]
     * @param  {[type]} xmlContent [description]
     * @return {[type]}            [description]
     */
    function updateAnswerHtml (question, xmlContent) {
        var resultHtml = '';

        resultHtml = displayByAnswer(xmlContent, question.AnswerImage);

        return resultHtml;
    }

    return {
        displayHtml: displayHtml,
        updateAnswerHtml: updateAnswerHtml
    }
})();
