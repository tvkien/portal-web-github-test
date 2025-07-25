(function ($) {
    $.widget('jquery.Numberline', {
        options: {
            NumberlineUtil: null,
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
            if (!options.NumberlineUtil.IsNullOrEmpty(question.Answer())) {
                answerText = question.Answer().AnswerText();
            }

            // Mapping answer
            if (!options.NumberlineUtil.IsNullOrEmpty(answerText)) {
                mappingsAnswer = answerText;
            }

            // Update correct answer
            if (!options.NumberlineUtil.IsNullOrEmpty(question.CorrectAnswer())) {
                mappingsCorrect = question.CorrectAnswer();
            }

            // Display answer of student
            answerHtml = that.GetContentNumberLineHotspot(self, $(question.ItemBody()), mappingsAnswer);
            // Display correct answer
            correctHtml = that.GetContentNumberLineHotspot(self, $(question.ItemBody()), mappingsCorrect);

            questionDetails = answerHtml.outerHTML();
            question.CorrectAnswerHTML = correctHtml.outerHTML();

            var mark = $('');
            if (!options.NumberlineUtil.IsNullOrEmpty(self.QTIOnlineTestSessionID()) && (self.IsComplete() || self.IsPendingReview())) {
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

        GetContentNumberLineHotspot: function(self, selector, mappings) {
            var $selector = $(selector);

            // Remove if content contains question number when higlighted
            $selector.find('#questionNumber').remove();
            $selector.find('numberLine').replaceWith(function () {
                var $numberline = $(this);
                var $result = $('<div class="numberLine"/>');

                $result.html($numberline.html());
                CopyAttributes($numberline, $result);

                $result.css({
                    'width': $numberline.attr('width') + 'px',
                    'height': $numberline.attr('height') + 'px'
                });

                return $result;
            });

            $selector.find('numberLineItem').replaceWith(function () {
                var $sourceItem = $(this);
                var sourceItemIdentifier = $sourceItem.attr('identifier');
                var sourceItemTop = $sourceItem.attr('top');
                var sourceItemleft = $sourceItem.attr('left');
                var $newSourceItem = $('<span class="numberLineItem"/>');

                $newSourceItem.html($sourceItem.html());
                CopyAttributes($sourceItem, $newSourceItem);

                $newSourceItem
                    .attr('identifier', sourceItemIdentifier)
                    .css({
                        'top': sourceItemTop + '%',
                        'left': sourceItemleft + '%'
                    });

                // Check answer from student or correct answer
                if (mappings !== '') {
                    mapper = mappings.split(',');

                    for (var i = 0, lenMapper = mapper.length; i < lenMapper; i++) {
                        var mapping = mapper[i];

                        if (mapping === sourceItemIdentifier) {
                            $newSourceItem.addClass('selected');
                        }
                    }
                }

                return $newSourceItem;
            });

            $selector.find('.numberLine svg').replaceWith(function() {
                var $svg = $(this);
                var $newSVG = $('<svg/>');

                CopyAttributes($svg, $newSVG);
                $newSVG.html($svg.html());
                $newSVG.attr('width', '100%');

                return $newSVG;
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
