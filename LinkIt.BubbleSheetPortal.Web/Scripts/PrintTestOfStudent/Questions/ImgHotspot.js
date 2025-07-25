(function ($) {
    $.widget('jquery.TOSImgHotspot', {
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
            if (question.Answer != null && question.Answer.AnswerText != null) {
                // answerText looks like HS_1,HS_3
                answerText = question.Answer.AnswerText;
            }
            var selectedIdentifiers = answerText.split(','); // mappings looks like "HS_1", "HS_3"

            var tree = $('<div></div>');
            tree.addClass('box-answer');
            tree.html(question.HtmlContent);
            tree.find('#questionNumber').remove();
            tree.find('.imageHotSpot, .imagehotspot')
               .replaceWith(function () {
                    var imgHotSpot = $(this);

                    var image = $('<img/>');
                    options.Util.CopyAttributes(imgHotSpot, image);
                    image.attr('src', imgHotSpot.attr('src'));
                    var questionContentWidth = options.Self.WidthPrintByColumn;
                    var orginalImageWidth = imgHotSpot.attr('width');
                    var newWidth = imgHotSpot.attr('width');
                    var newHeight = imgHotSpot.attr('height');
                    var reducePercent = 0;

                    if (orginalImageWidth > questionContentWidth) {
                       reducePercent = (newWidth - questionContentWidth) / newWidth;
                       newWidth = questionContentWidth;
                       newHeight = newHeight - newHeight * reducePercent;
                    }

                    image
                        .css({
                            'width': newWidth + 'px',
                            'height': newHeight + 'px'
                        });

                    var result = $('<div class="imageHotspotInteraction"></div>');
                    result.attr('responseIdentifier', imgHotSpot.attr('responseIdentifier'));
                    result.css('width', newWidth + 'px');
                    result.css('height', 'auto');

                    result.html(imgHotSpot.html());
                    result.prepend(image);
                    result.css('position', 'relative');

                    result.find('.sourceItem, .sourceitem').replaceWith(function () {
                        var sourceItem = $(this);
                        var newSourceItem = $('<span class="hotspot-item-type"></span>');
                        var sourceItemTop = sourceItem.attr('top') - sourceItem.attr('top') * reducePercent;
                        var sourceItemWidth = sourceItem.attr('width') - sourceItem.attr('width') * reducePercent;
                        var sourceItemHeight = sourceItem.attr('height') - sourceItem.attr('height') * reducePercent;
                        var sourceItemLeft = sourceItem.attr('left') - sourceItem.attr('left') * reducePercent;
                        var sourceItemTypeHotspot = sourceItem.attr('typeHotSpot');
                        var sourceItemIdentifier = sourceItem.attr('identifier');
                        var sourceItemShowBorderHotspot = sourceItem.attr('showborderhotspot');
                        var sourceItemFillHotspot = sourceItem.attr('fillhotspot');
                        var sourceItemHidden = sourceItem.attr('hiddenHotSpot') == undefined ? false : sourceItem.attr('hiddenHotSpot');

                        newSourceItem.html('<span class=" hotspot-item-value">' + sourceItem.html() + '</span>');
                        newSourceItem.css('position', 'absolute');
                        newSourceItem.css('top', sourceItemTop + 'px');
                        newSourceItem.css('width', sourceItemWidth + 'px');
                        newSourceItem.css('height', sourceItemHeight + 'px');
                        newSourceItem.css('line-height', sourceItemHeight + 'px');
                        newSourceItem.css('left', sourceItemLeft + 'px');
                        newSourceItem.attr('typehotspot', sourceItemTypeHotspot);
                        newSourceItem.attr('identifier', sourceItemIdentifier);
                        newSourceItem.attr('showborderhotspot', sourceItemShowBorderHotspot);
                        newSourceItem.attr('fillhotspot', sourceItemFillHotspot);

                        ko.utils.arrayForEach(selectedIdentifiers, function (selectedIdentifier) {
                            if (selectedIdentifier === sourceItem.attr('identifier')) {
                                newSourceItem.addClass('checked');
                            }
                        });

                        if (sourceItemHidden == 'true' && !newSourceItem.hasClass('checked')) { //hidden and is not correct answer
                            newSourceItem.addClass('hotspot-hidden');
                        }

                        return newSourceItem;
                    });

                    return $(result);
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
