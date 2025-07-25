(function ($) {
    $.widget('jquery.ImgHotspot', {
        options: {
            ImgHotspotUtil: null,
            PostProcessQuestionDetails: null,
            Self: null
        },

        Display: function (question) {
            var that = this;
            var options = that.options;
            var self = options.Self;

            // answerText = 'DEST_2,DEST_1,DEST_3,DEST_5';
            var answerText = '';            
            var mappingsAnswer = '';
            var mappingsCorrect = '';
            var answerHtml = '';
            var correctHtml = '';
            var questionDetails = '';

            // Update answer text
            if (!options.ImgHotspotUtil.IsNullOrEmpty(question.Answer())) {
                answerText = question.Answer().AnswerText();
            }

            // Mapping answer
            if (!options.ImgHotspotUtil.IsNullOrEmpty(answerText)) {
                mappingsAnswer = answerText;
            }

            // Update correct answer
            if (!options.ImgHotspotUtil.IsNullOrEmpty(question.CorrectAnswer())) {
                mappingsCorrect = question.CorrectAnswer();
            }

            // Display answer of student
            answerHtml = that.GetContentImgHotspot(self, $(question.ItemBody()), mappingsAnswer);
            // Display correct answer
            correctHtml = that.GetContentImgHotspot(self, $(question.ItemBody()), mappingsCorrect);

            questionDetails = answerHtml.outerHTML();
            question.CorrectAnswerHTML = correctHtml.outerHTML();

            var mark = $('');
            if (!options.ImgHotspotUtil.IsNullOrEmpty(self.QTIOnlineTestSessionID()) && (self.IsComplete() || self.IsPendingReview())) {
                mark = $('<i class="jsIsAnswerCorrect" style="float:none;display:inline-block;position: relative;margin-left:15px;"></i>');
                if (self.PointsEarned() == self.PointsPossible()) {
                    mark.addClass('correct');
                } else if (self.PointsEarned() > 0) {
                    mark.addClass('partial');
                } else {
                    mark.addClass('incorrect');
                }
            }

            questionDetails += mark.outerHTML();
            questionDetails += '<div class="btn-show-correct-answer big-button" onClick="ShowCorrectAnswer()">Show Correct Answer</div>';

            if (options.PostProcessQuestionDetails !== null &&
                typeof (options.PostProcessQuestionDetails) == "function") {
                questionDetails = options.PostProcessQuestionDetails(questionDetails);
            }

            self.Respones(questionDetails);
        },

        ShowCorrectAnswer: function (self, question) {
            var $contentCorrect = $(question.CorrectAnswerHTML);
            $contentCorrect.find('.highlighted').removeAttr('style').removeClass();

            question.CorrectAnswerHTML = $contentCorrect.outerHTML();
            Reviewer.popupAlertMessage(question.CorrectAnswerHTML, 'ui-popup-fullpage', 700, 500);
            self.ReviewerWidget.ReviewerWidget('LoadImages', $('.ui-popup-fullpage'));
        },

        GetContentImgHotspot: function(self, selector, mappings) {
            var $selector = $(selector);

            // Remove if content contains question number when higlighted
            $selector.find('#questionNumber').remove();
            $selector.find('imageHotSpot').replaceWith(function() {
                var $imgHotSpot = $(this);
                var $image = $('<img />');
                var $result = $('<div class="imageHotspotInteraction"/>');

                $image
                    .attr('src', $imgHotSpot.attr('src'))
                    .css({
                        'width': $imgHotSpot.attr('width') + 'px',
                        'height': $imgHotSpot.attr('height') + 'px'
                    });

                $result
                    .attr('responseIdentifier', $imgHotSpot.attr('responseIdentifier'))
                    .css({
                        'width': $imgHotSpot.attr('width') + 'px',
                        'height': $imgHotSpot.attr('height') + 'px'
                    });

                $result.html($imgHotSpot.html());
                $result.prepend($image);

                $result.find('sourceItem').replaceWith(function() {
                    var $sourceItem = $(this);
                    var sourceItemTop = $sourceItem.attr('top');
                    var sourceItemLeft = $sourceItem.attr('left');
                    var sourceItemWidth = $sourceItem.attr('width');
                    var sourceItemHeight = $sourceItem.attr('height');
                    var sourceItemType = $sourceItem.attr('typeHotSpot');
                    var sourceItemIdentifier = $sourceItem.attr('identifier');
                    var sourceItemShowBorder = $sourceItem.attr('showBorderHotSpot');
                    var sourceItemFill = $sourceItem.attr('fillHotSpot');
                    var sourceItemRollover = $sourceItem.attr('rolloverPreviewHotSpot');
                    var $newSourceItem = $('<span class="hotspot-item-type"/>');
                    var mapper = [];

                    $newSourceItem.html('<span class="hotspot-item-value">' + $sourceItem.html() + '</span>');
                    $newSourceItem
                        .attr({
                            'typehotspot': sourceItemType,
                            'identifier': sourceItemIdentifier,
                            'showborderhotspot': sourceItemShowBorder,
                            'fillhotspot': sourceItemFill,
                            'rolloverhotspot': sourceItemRollover
                        })
                        .css({
                            'position': 'absolute',
                            'top': sourceItemTop + 'px',
                            'left': sourceItemLeft + 'px',
                            'width': sourceItemWidth + 'px',
                            'height': sourceItemHeight + 'px',
                            'line-height': sourceItemHeight + 'px'
                        });

                    // Check answer from student or correct answer
                    if (mappings !== '') {
                        mapper = mappings.split(',');

                        for (var i = 0, lenMapper = mapper.length; i < lenMapper; i++) {
                            var mapping = mapper[i];

                            if (mapping === sourceItemIdentifier) {
                                $newSourceItem.addClass('checked');
                            }
                        }
                    }

                    return $newSourceItem;
                });

                return $result;
            });

            //self.LoadImagesSelector($selector);

            return $selector;
        },

        _getHtmlValue: function (id) {
            return $.trim($('#' + id).html());
        },

        _setHtmlValue: function (id, val) {
            $('#' + id).html(val);
        }
    });
}(jQuery));
