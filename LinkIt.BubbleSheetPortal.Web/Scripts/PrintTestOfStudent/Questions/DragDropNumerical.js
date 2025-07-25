(function ($) {
    $.widget('jquery.TOSDragDropNumerical', {
        options: {
            Util: null,
            Self: null,
            TheCorrectAnswer: null,
            GuidanceAndRationale: null,
            TheQuestionContent: null
        },

        Display: function (question) {
            var that = this;
            var options = that.options;

            if (!options.TheQuestionContent) {
                question.HtmlContent = '';
                return;
            }
            
            var answerText = '';

            if (question.Answer !== null && question.Answer.AnswerText !== null) {
                answerText = question.Answer.AnswerText;
            }

            // mappings looks like "DEST_1-SRC_1;SRC_2", "DEST_2-SRC_3", "DEST_3-SRC_4"
            // answerText is student's answer
            var mappings = answerText.split(',');

            var tree = $('<div></div>');
            tree.html(question.HtmlContent);
            tree.find('.destinationObject, .destinationobject').each(function(index, desObj) {
                var $desObj = $(desObj);

                $desObj.replaceWith(function() {
                    var desObjType = $desObj.attr('type');
                    var $result = $('<div/>').attr({
                                        'class': 'partialDestinationObject',
                                        'type': desObjType
                                    });

                    $desObj.find('.destinationItem, .destinationitem').each(function(ind, desItem) {
                        var $desItem = $(desItem);
                        var desItemIdentifier = $desItem.attr('destidentifier');
                        var desItemWidth = $desItem.attr('width');
                        var desItemHeight = $desItem.attr('height');
                        var desItemClass = '';
                        var desItemHtml = '';
                        var srcObjArr = [];

                        desItemWidth = desItemWidth === undefined ? 55 : desItemWidth;
                        desItemHeight = desItemHeight === undefined ? 20 : desItemHeight;

                        // Check if question have answer or not answer
                        if (mappings[0] !== '') {
                            for (var mi = 0, lenMappings = mappings.length; mi < lenMappings; mi++) {
                                var mapping = mappings[mi];
                                var mappingValue = mapping.split('-');
                                var mappingDest = mappingValue[0];
                                var mappingSrc = mappingValue[1];

                                // Compare destination identifier with mapping
                                if (desItemIdentifier === mappingDest && mappingSrc !== '') {
                                    var si = 0;

                                    srcObjArr = mappingSrc.split(';');

                                    // Reset destination item when destination item have source object
                                    $desItem.html('');

                                    while (si < srcObjArr.length) {
                                        var srcObjHtml = tree.find('.sourceObject[srcidentifier="' + srcObjArr[si] + '"], .sourceobject[srcidentifier="' + srcObjArr[si] + '"]');
                                        $desItem.append($(srcObjHtml).outerHTML());
                                        si++;
                                    }
                                }
                            }
                        }

                        desItemHtml = '<div class="' + desItemClass + '" style="width: ' + desItemWidth + 'px; height: ' + desItemHeight + 'px; overflow: hidden;">' + $desItem.html() + '</div>';

                        $result.append(desItemHtml);
                    });

                    return $result;
                });
            });

            question.HtmlContent = tree.outerHTML();
        }
    });
}(jQuery));
