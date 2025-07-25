(function ($) {
    $.widget('jquery.TableHotspot', {
        options: {
            TableHotspotUtil: null,
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
            if (!options.TableHotspotUtil.IsNullOrEmpty(question.Answer())) {
                answerText = question.Answer().AnswerText();
            }

            // Mapping answer
            if (!options.TableHotspotUtil.IsNullOrEmpty(answerText)) {
                mappingsAnswer = answerText;
            }

            // Update correct answer
            if (!options.TableHotspotUtil.IsNullOrEmpty(question.CorrectAnswer())) {
                mappingsCorrect = question.CorrectAnswer();
            }

            // Display answer of student
            answerHtml = that.GetContentTableHotspot(self, $(question.ItemBody()), mappingsAnswer);
            // Display correct answer
            correctHtml = that.GetContentTableHotspot(self, $(question.ItemBody()), mappingsCorrect);

            questionDetails = answerHtml.outerHTML();
            question.CorrectAnswerHTML = correctHtml.outerHTML();

            var mark = $('');
            if (!options.TableHotspotUtil.IsNullOrEmpty(self.QTIOnlineTestSessionID()) && (self.IsComplete() || self.IsPendingReview())) {
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

        GetContentTableHotspot: function(self, selector, mappings) {
            var $selector = $(selector);

            // Remove if content contains question number when higlighted
            $selector.find('#questionNumber').remove();
            $selector.find('tableitem').replaceWith(function() {
                var $sourceItem = $(this);
                var sourceItemIdentifier = $sourceItem.attr('identifier');
                var sourceItemId = $sourceItem.attr('id');
                var $newSourceItem = $('<tableitem class="hotspot-item-type"></tableitem>');
                var mapper = [];

                CopyAttributes($sourceItem, $newSourceItem);
                $newSourceItem.removeClass('selected');

                // Check answer from student or correct answer
                if (mappings !== '') {
                    mapper = mappings.split(',');

                    for (var i = 0, lenMapper = mapper.length; i < lenMapper; i++) {
                        var mapping = mapper[i];

                        if (mapping === sourceItemIdentifier || mapping === sourceItemId) {
                            $newSourceItem.addClass('selected');
                        }
                    }
                }

                return $newSourceItem;
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
