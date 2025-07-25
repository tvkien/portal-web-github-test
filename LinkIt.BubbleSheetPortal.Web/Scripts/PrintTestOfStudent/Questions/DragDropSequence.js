(function ($) {
    $.widget('jquery.TOSDragDropSequence', {
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
            if (question.Answer != null && question.Answer.AnswerText != null) answerText = question.Answer.AnswerText;

            var tree = $('<div></div>');
            tree.html(question.HtmlContent);
            if (answerText != '') {
                var mappings = answerText.split(',');
                tree.find('partialsequence').replaceWith(function () {
                    var partialsequence = $(this);
                    var result = $('<partialsequence></partialsequence>');
                    options.Util.CopyAttributes(partialsequence, result);
                    $.each(mappings, function (idx, mappingItem) {
                        var newSourceItem = $('<div class="sourceItem"></div>');
                        var currentSourceItem = partialsequence.find('.sourceItem[identifier="' + mappingItem + '"], .sourceitem[identifier="' + mappingItem + '"]');
                        if (currentSourceItem != null) {
                            options.Util.CopyAttributes(currentSourceItem, newSourceItem);
                            newSourceItem.css({ "width": currentSourceItem.attr("width"), "height": currentSourceItem.attr("height") }).html(currentSourceItem.html());
                            result.append(newSourceItem);
                        }
                    });
                    return result;
                });
            } else {
                tree.find('partialsequence').replaceWith(function () {
                    var partialsequence = $(this);
                    var result = $('<partialsequence></partialsequence>');
                    options.Util.CopyAttributes(partialsequence, result);
                    partialsequence.find('.sourceItem').each(function () {
                        var newSourceItem = $('<div class="sourceItem"></div>');
                        var currentSourceItem = $(this);
                        options.Util.CopyAttributes(currentSourceItem, newSourceItem);
                        newSourceItem.css({ "width": currentSourceItem.attr("width"), "height": currentSourceItem.attr("height") }).html(currentSourceItem.html());
                        result.append(newSourceItem);
                    });

                    return result;
                });
            }

            question.HtmlContent = tree.outerHTML();
        },
    });
}(jQuery));
