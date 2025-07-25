(function ($) {
    $.widget('jquery.TextHotspotAlgorithmic', {
        options: {
            TextHotspotAlgorithmicUtil: null,
            PostProcessQuestionDetails: null,
            Self: null
        },

        Display: function (question) {
            var that = this;
            var options = that.options;
            var self = options.Self;

            // answerText = 'DEST_2,DEST_1,DEST_3,DEST_5';
            var answerText = '';
            var mappingsAnswer = [];
            var mappingsCorrect = [];
            var answerHtml = '';
            var correctHtml = '';
            var questionDetails = '';


            // Update answer text
            if (!options.TextHotspotAlgorithmicUtil.IsNullOrEmpty(question.Answer())) {
                answerText = question.Answer().AnswerText();
            }

            // Mapping answer
            if (!options.TextHotspotAlgorithmicUtil.IsNullOrEmpty(answerText)) {
                mappingsAnswer.push(answerText);
            }

            // Update correct answer
            if (!options.TextHotspotAlgorithmicUtil.IsNullOrEmpty(question.CorrectAnswer())) {
                mappingsCorrect.push(question.CorrectAnswer());
            }

            // Display answer of student
            answerHtml = that.GetContentTextHotspot(self, $(question.ItemBody()), mappingsAnswer);
            // Display correct answer
            correctHtml = that.GetContentTextHotspot(self, $(question.ItemBody()), mappingsCorrect);

            questionDetails = answerHtml.outerHTML();
            question.CorrectAnswerHTML = correctHtml.outerHTML();

            if (question.AlgorithmicCorrectAnswers() != null && question.AlgorithmicCorrectAnswers().length) {
                questionDetails += '<br> <div class="btn-show-all-correct-answer big-button" onClick="ShowAllCorrectAnswers()">Show all correct answers</div>';
            }
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

        ShowAllCorrectAnswers: function (self, question) {
            var that = this;
            var options = that.options;
            var self = options.Self;
            var algorithmicCorrectAnswers = $('<div/>');
            var popupWidth = 700;
            var algorithmicPoints = [];
            
            ko.utils.arrayForEach(question.AlgorithmicCorrectAnswers(), function (item) {
                var correctAnswer;
                var tree;
                var correctHtml;

                if (typeof item.Amount === 'function' && item.Amount() > 0) {
                    correctAnswer = item.ConditionValue();
                    correctHtml = that.GetContentTextHotspot(self, $(question.ItemBody()), correctAnswer);
                    tree = $('<div/>');
                    tree.addClass('box-answer');
                    var elAtleast = Reviewer.getAtleast(item.Amount(), item.PointsEarned(), question.QTIItemSchemaID());
                    tree.append(elAtleast.outerHTML);
                    correctHtml.appendTo(tree);
                    tree.appendTo(algorithmicCorrectAnswers);
                    algorithmicPoints.push(item.PointsEarned);
                } else {
                    correctAnswer = item.ConditionValue();
                    correctHtml = that.GetContentTextHotspot(self, $(question.ItemBody()), correctAnswer);
                    tree = $('<div/>');
                    tree.addClass('box-answer');
                    correctHtml.appendTo(tree);
                    tree.appendTo(algorithmicCorrectAnswers);
                    algorithmicPoints.push(item.PointsEarned);
                }
            });

            var questionDetails = algorithmicCorrectAnswers.outerHTML();
            Reviewer.popupAlertMessage(questionDetails, 'ui-popup-fullpage ui-popup-algorithmic-correct-answer', 700, 500);
            Reviewer.createTabWidget('.ui-popup-fullpage.ui-popup-algorithmic-correct-answer', algorithmicPoints);
        },

        GetContentTextHotspot: function (self, selector, mappings) {
            var $selector = $(selector);

            // Remove if content contains question number when higlighted
            $selector.find('#questionNumber').remove();
            $selector.find('textHotSpot').replaceWith(function () {
                var $textHotspot = $(this);
                var $result = $('<div class="textHotspotInteraction"/>');
                $result.html($textHotspot.html());
                $result.attr('responseIdentifier', $textHotspot.attr('responseIdentifier'));

                return $result;
            });

            $selector.find('sourceText').replaceWith(function () {
                var $sourceItem = $(this);
                var sourceItemType = $sourceItem.attr('typeHotSpot');
                var sourceItemIdentifier = $sourceItem.attr('identifier');
                var $newSourceItem = $('<span class="sourcetext"/>');
                var mapper = [];

                $newSourceItem.html($sourceItem.html());
                $newSourceItem.attr({
                    'typehotspot': sourceItemType,
                    'identifier': sourceItemIdentifier
                });

                // Check answer from student or correct answer
                if (mappings.length > 0) {
                    for (var i = 0, len = mappings.length; i < len; i++) {
                        mapper = mappings[i].split(',');

                        for (var j = 0, lenMapper = mapper.length; j < lenMapper; j++) {
                            var mapping = mapper[j];

                            if (mapping === sourceItemIdentifier) {
                                $newSourceItem.addClass('active');
                            }
                        }
                    }
                }
                return $newSourceItem;
                });

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
