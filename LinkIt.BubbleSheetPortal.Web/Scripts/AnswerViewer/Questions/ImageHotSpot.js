/**
 * Image Hot Spot Module
 * @param  {[type]} function (             [description]
 * @return {[type]}          [description]
 */
var ImageHotSpot = (function () {
    /**
     * Display xml to html
     * @param  {[type]} xmlContent [description]
     * @return {[type]}            [description]
     */
    function displayHtml (xmlContent) {
        var $tree = $('<div/>');

        $tree.append(xmlContent);

        $tree.find('imagehotspot').replaceWith(function () {
            var $imagehotspot = $(this);
            var imagehotspotResponseId =  $imagehotspot.attr('responseIdentifier');
            var imagehotspotWidth = $imagehotspot.attr('width');
            var imagehotspotHeight = $imagehotspot.attr('height');
            var $imagehotspotImg = $('<img/>');
            var $newImagehotspot = $('<div/>');

            Utils.copyAttributes($imagehotspot, $imagehotspotImg);

            $imagehotspotImg.css({
                'width': imagehotspotWidth + 'px',
                'height': imagehotspotHeight + 'px'
            });

            $newImagehotspot.attr('responseIdentifier', imagehotspotResponseId);

            $newImagehotspot
                .addClass('imagehotspot')
                .css({
                    'width': imagehotspotWidth + 'px',
                    'height': imagehotspotHeight + 'px'
                })
                .html($imagehotspot.html())
                .prepend($imagehotspotImg);

            $newImagehotspot.find('sourceitem').replaceWith(function () {
                var $sourceitem = $(this);
                var sourceitemTop = $sourceitem.attr('top');
                var sourceitemLeft = $sourceitem.attr('left');
                var sourceitemWidth = $sourceitem.attr('width');
                var sourceitemHeight = $sourceitem.attr('height');
                var $newSourceItem = $('<span/>');

                Utils.copyAttributes($sourceitem, $newSourceItem);

                $newSourceItem
                    .addClass('imagehotspot-item')
                    .css({
                        'width': sourceitemWidth + 'px',
                        'height': sourceitemHeight + 'px',
                        'top': sourceitemTop + 'px',
                        'left': sourceitemLeft + 'px',
                        'line-height': sourceitemHeight + 'px'
                    })
                    .html('<span class="imagehotspot-item-value">' + $sourceitem.html() + '</span>')

                return $newSourceItem;
            });

            return $newImagehotspot;
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

        if (!Utils.isNullOrEmpty(answer) && $tree.find('.imagehotspot').length) {
            answer = answer.split(',');

            for (var i = 0, len = answer.length; i < len; i++) {
                $texthotspot = $tree.find('.imagehotspot-item[identifier="' + answer[i] + '"]');
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
