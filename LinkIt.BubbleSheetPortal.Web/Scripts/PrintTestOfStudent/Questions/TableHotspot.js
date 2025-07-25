(function ($) {
    $.widget('jquery.TOSTableHotspot', {
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
            var mappings;
            var portalImgPrint = options.Self.PortalImgPrint;

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
            $tree.find('.tableitem').replaceWith(function() {
                var $sourceItem = $(this);
                var sourceItemId = $sourceItem.attr('id');
                var sourceItemIdentifier = $sourceItem.attr('identifier');
                var sourceItemType = $sourceItem.attr('typehotspot');
                var $newSourceItem = $('<span/>');

                options.Util.CopyAttributes($sourceItem, $newSourceItem);

                $newSourceItem
                    .removeClass()
                    .addClass('TableHotspot-item TableHotspot-item--' + sourceItemType);

                if (mappings !== undefined) {
                    if (mappings.length) {
                        for (var i = 0, len = mappings.length; i < len; i++) {
                            var mapping = mappings[i];

                            if (mapping === sourceItemId ||
                                mapping === sourceItemIdentifier) {
                                $newSourceItem.addClass('is-checked');
                            }
                        }
                    }
                }

                return $newSourceItem;
            });

            // Fix Background Of Prince Below Version 9.0
            $tree.find('.TableHotspot-item--checkbox')
                .css('background', 'url(' + portalImgPrint + 'icon-table-checkbox.png) no-repeat');
            $tree.find('.TableHotspot-item--checkbox.is-checked')
                .css('background', 'url(' + portalImgPrint + 'icon-table-checkbox-checked.png) no-repeat');
            $tree.find('.TableHotspot-item--circle')
                .css('background', 'url(' + portalImgPrint + 'icon-table-circle.png) no-repeat');
            $tree.find('.TableHotspot-item--circle.is-checked')
                .css('background', 'url(' + portalImgPrint + 'icon-table-circle-checked.png) no-repeat');

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
