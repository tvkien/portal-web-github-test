(function ($) {
    $.widget('jquery.TOSTextHotspot', {
        options: {
            Util: null,
            Self: null,
            TheCorrectAnswer: null,
            GuidanceAndRationale: null,
            TheQuestionContent: null
        },

        Display: function(question) {
            var that = this;
            var options = that.options;

            if (!options.TheQuestionContent) {
                question.HtmlContent = '';
                return;
            }

            var answerText = '';
            if (question.Answer != null && question.Answer.AnswerText != null) answerText = question.Answer.AnswerText; //answerText looks like HS_1,HS_3
            var selectedIdentifiers = answerText.split(','); //mappings looks like "HS_1", "HS_3"

            var tree = $('<div></div>');
            tree.addClass('box-answer');
            tree.html(question.HtmlContent);
            $('<div class="clearfix"></div>').prependTo(tree);
            $('<div class="clearfix"></div>').appendTo(tree);
            tree.find('.textHotSpot').addClass('textHotspotInteraction');

            tree.find('.sourceText, .sourcetext').each(function() {
                var sourceItem = $(this);
                sourceItem.addClass('sourcetext');
                ko.utils.arrayForEach(selectedIdentifiers, function(selectedIdentifier) {
                    if (selectedIdentifier === sourceItem.attr('identifier')) {
                        sourceItem.addClass('active');
                    }
                });
            });

            question.HtmlContent = tree.outerHTML();
        },

        _getHtmlValue: function (id) {
            return $.trim($('#' + id).html());
        },

        _setHtmlValue: function (id, val) {
            $('#' + id).html(val);
        }
    });
}(jQuery));
