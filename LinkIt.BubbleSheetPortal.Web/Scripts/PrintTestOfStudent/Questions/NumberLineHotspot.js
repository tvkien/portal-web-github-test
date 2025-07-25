(function ($) {
    $.widget('jquery.TOSNumberLineHotspot', {
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
            var maxWidth = 260;
            var mappings;

            if (question.Answer !== null &&
                question.Answer.AnswerText !== null) {
                answerText = question.Answer.AnswerText;
            }

            if (answerText !== '') {
                mappings = answerText.split(',');
            }

            var $tree = $('<div/>');
            $tree.html(question.HtmlContent);
            $tree.find('#questionNumber').remove();
            $tree.find('.numberLine, .numberline').replaceWith(function () {
                var $numberline = $(this);
                var numberlineWidth = $numberline.attr('width');
                var numberlineHeight = $numberline.attr('height');
                var $result = $('<div/>');
                var reducePercent = 0;

                $result.html($numberline.html());

                options.Util.CopyAttributes($numberline, $result);

                if (numberlineWidth > maxWidth) {
                    reducePercent = (numberlineWidth - maxWidth) / numberlineWidth;
                    numberlineWidth = maxWidth;
                    numberlineHeight = numberlineHeight - numberlineHeight * reducePercent;
                }

                $result
                    .removeClass()
                    .addClass('Numberline')
                    .css({
                        'width': numberlineWidth + 'px',
                        'height': numberlineHeight + 'px'
                    });

                return $result;
            });

            $tree.find('.numberLineItem, .numberlineitem').replaceWith(function () {
                var $sourceItem = $(this);
                var sourceItemIdentifier = $sourceItem.attr('identifier');
                var sourceItemTop = $sourceItem.attr('top');
                var sourceItemLeft = $sourceItem.attr('left');
                var $newSourceItem = $('<span/>');

                $newSourceItem.html($sourceItem.html());

                options.Util.CopyAttributes($sourceItem, $newSourceItem);

                $newSourceItem
                    .removeClass()
                    .addClass('Numberline-item')
                    .attr('identifier', sourceItemIdentifier)
                    .css({
                        'top': sourceItemTop + '%',
                        'left': sourceItemLeft + '%'
                    });

                if (mappings !== undefined) {
                    if (mappings.length) {
                        for (var i = 0, len = mappings.length; i < len; i++) {
                            var mapping = mappings[i];

                            if (mapping === sourceItemIdentifier) {
                                $newSourceItem.addClass('is-checked');
                            }
                        }
                    }
                }

                return $newSourceItem;
            });

            $tree.find('.numberLine svg').replaceWith(function() {
                var $svg = $(this);
                var $newSvg = $('<svg></svg>');

                options.Util.CopyAttributes($svg, $newSvg);
                $newSvg.html($svg.html());
                $newSvg.attr('width', '100%');

                return $newSvg;
            });

            question.HtmlContent = $tree.html();
        },

        _getHtmlValue: function (id) {
            return $.trim($('#' + id).html());
        },

        _setHtmlValue: function (id, val) {
            $('#' + id).html(val);
        }
    });
}(jQuery));
