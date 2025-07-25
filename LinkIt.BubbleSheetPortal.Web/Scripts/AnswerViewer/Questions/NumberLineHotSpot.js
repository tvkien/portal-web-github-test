/**
 * Number Line Hot Spot Module
 * @param  {[type]} function (             [description]
 * @return {[type]}          [description]
 */
var NumberLineHotSpot = (function () {

    /**
     * Display xml to html
     * @param  {[type]} xmlContent [description]
     * @return {[type]}            [description]
     */
    function displayHtml (xmlContent) {
        var $tree = $('<div/>');

        $tree.append(xmlContent);

        $tree.find('numberline').replaceWith(function () {
            var $numberline = $(this);
            var numberlineWidth = $numberline.attr('width');
            var numberlineHeight = $numberline.attr('height');
            var $newNumberline = $('<div/>');

            Utils.copyAttributes($numberline, $newNumberline);

            $newNumberline
                .addClass('numberline')
                .css({
                    'width': numberlineWidth + 'px',
                    'height': numberlineHeight + 'px'
                })
                .html($numberline.html());

            return $newNumberline;
        });

        $tree.find('numberlineitem').replaceWith(function () {
            var $numberlineitem = $(this);
            var numberlineitemTop = $numberlineitem.attr('top');
            var numberlineitemLeft = $numberlineitem.attr('left');
            var $newNumberlineitem = $('<span/>');

            Utils.copyAttributes($numberlineitem, $newNumberlineitem);

            $newNumberlineitem
                .addClass('numberline-item')
                .css({
                    'top': numberlineitemTop + '%',
                    'left': numberlineitemLeft + '%'
                })
                .html($numberlineitem.html());

            return $newNumberlineitem;
        });

        $tree.find('.numberlineitem svg').replaceWith(function () {
            var $svg = $(this);
            var $newSVG = $('<svg/>');

            Utils.copyAttributes($svg, $newSVG);

            $newSVG
                .attr('width', '100%')
                .html($svg.html());

            return $newSVG;
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

       if (!Utils.isNullOrEmpty(answer) && $tree.find('.numberline').length) {
           answer = answer.split(',');

           for (var i = 0, len = answer.length; i < len; i++) {
               var $numberlineItem = $tree.find('.numberline-item[identifier="' + answer[i] + '"]');

               $numberlineItem.addClass('is-selected');
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
