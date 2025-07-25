(function ($) {
    $.widget('jquery.NumberlineAlgorithmic', {
        options: {
            NumberlineAlgorithmicUtil: null,
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
            if (!options.NumberlineAlgorithmicUtil.IsNullOrEmpty(question.Answer())) {
                answerText = question.Answer().AnswerText();
            }

            // Mapping answer
            if (!options.NumberlineAlgorithmicUtil.IsNullOrEmpty(answerText)) {
                mappingsAnswer.push(answerText);
            }

            // Update correct answer
            if (!options.NumberlineAlgorithmicUtil.IsNullOrEmpty(question.CorrectAnswer())) {
                mappingsCorrect.push(question.CorrectAnswer());
            }

            // Display answer of student
            answerHtml = that.GetContentNumberLineHotspot(self, $(question.ItemBody()), mappingsAnswer);
            // Display correct answer
            correctHtml = that.GetContentNumberLineHotspot(self, $(question.ItemBody()), mappingsCorrect);

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
            Reviewer.popupAlertMessage(question.CorrectAnswerHTML, 'ui-popup-fullpage', 700, 500, false);
            self.ReviewerWidget.ReviewerWidget('LoadImages', $('.ui-popup-fullpage'));
        },

        ShowAllCorrectAnswers: function (self, question) {
            var that = this;
            var options = that.options;
            var self = options.Self;
            var algorithmicCorrectAnswers = $('<div/>');
            var algorithmicPoints = [];

            ko.utils.arrayForEach(question.AlgorithmicCorrectAnswers(), function (item) {
                var correctAnswer;
                var tree;
                var correctHtml;

                if (typeof item.Amount === 'function' && item.Amount() > 0) {
                    correctAnswer = item.ConditionValue();
                    correctHtml = that.GetContentNumberLineHotspot(self, $(question.ItemBody()), correctAnswer);
                    tree = $('<div/>');
                    tree.addClass('box-answer');
                    var elAtleast = Reviewer.getAtleast(item.Amount(), item.PointsEarned(), question.QTIItemSchemaID());
                    tree.append(elAtleast.outerHTML);
                    correctHtml.appendTo(tree);
                    tree.appendTo(algorithmicCorrectAnswers);
                    algorithmicPoints.push(item.PointsEarned);
                } else {
                    correctAnswer = item.ConditionValue();
                    correctHtml = that.GetContentNumberLineHotspot(self, $(question.ItemBody()), correctAnswer);
                    tree = $('<div/>');
                    tree.addClass('box-answer');
                    correctHtml.appendTo(tree);
                    tree.appendTo(algorithmicCorrectAnswers);
                    algorithmicPoints.push(item.PointsEarned);
                }
            });
           
            var questionDetails = algorithmicCorrectAnswers.outerHTML();
            Reviewer.popupAlertMessage(questionDetails, 'ui-popup-fullpage ui-popup-algorithmic-correct-answer', 700, 500, false);
            Reviewer.createTabWidget('.ui-popup-fullpage.ui-popup-algorithmic-correct-answer', algorithmicPoints);
        },


        GetContentNumberLineHotspot: function (self, selector, mappings) {
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
                if (mappings.length > 0) {
                    for (var i = 0, len = mappings.length; i < len; i++) {
                        var mapper = mappings[i].split(',');

                        for (var j = 0, lenMapper = mapper.length; j < lenMapper; j++) {
                            var mapping = mapper[j];

                            if (mapping === sourceItemIdentifier) {
                                $newSourceItem.addClass('selected');
                            }
                        }
                    }
                }

                return $newSourceItem;
            });

            $selector.find('.numberLine svg').replaceWith(function () {
                var $svg = $(this);
                var $newSVG = $('<svg/>');

                CopyAttributes($svg, $newSVG);
                $newSVG.html($svg.html());
                $newSVG.attr('width', '100%');

                return $newSVG;
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
