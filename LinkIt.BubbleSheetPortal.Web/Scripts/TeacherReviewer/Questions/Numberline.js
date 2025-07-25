(function ($) {
    $.widget('jquery.Numberline', {
        options: {
            NumberlineUtil: null,
            PostProcessQuestionDetails: null
        },

        Display: function (self, question) {
            var that = this;
            var options = that.options;
            if (self.RequireApplyGrade()) {
                AlertMessage('The points has been changed. You must apply grade.');
                return;
            }

            self.SelectedQuestion(question);
            self.RefObjects(question.RefObjects());
            self.HightLightSelectedQuestion(question);
            self.PointsPossible(question.PointsPossible());
            self.QTIItemSchemaID(question.QTIItemSchemaID());
            self.AnswerSubID('');

            var answerOfStudentForSelectedQuestion;
            ko.utils.arrayForEach(self.TestOnlineSessionAnswers(), function (testOnlineSessionAnswer) {
                if (question.VirtualQuestionID() === testOnlineSessionAnswer.VirtualQuestionID()) {
                    answerOfStudentForSelectedQuestion = testOnlineSessionAnswer;
                }
            });

            var answerText = '';
            if (!options.NumberlineUtil.IsNullOrEmpty(answerOfStudentForSelectedQuestion)) {
                self.PointsEarned(answerOfStudentForSelectedQuestion.PointsEarned());
                self.OldPointsEarned(answerOfStudentForSelectedQuestion.PointsEarned());
                self.AnswerID(answerOfStudentForSelectedQuestion.QTIOnlineTestSessionAnswerID());
                answerText = answerOfStudentForSelectedQuestion.AnswerText();
                self.AnswerImage(answerOfStudentForSelectedQuestion.AnswerImage());
                self.ShowHightLight(answerOfStudentForSelectedQuestion.HighlightQuestion(), question);
                self.DisplayItemFeedback(true,answerOfStudentForSelectedQuestion);
            } else {
                question.XmlContent(question.DataXmlContent());
                self.PointsEarned(0);
                self.OldPointsEarned(0);
                self.AnswerID(0);
                answerText = '';
                self.AnswerImage('');
                self.ShowHightLight('', question);
                self.DisplayItemFeedback(false, null);
            }

            // answerText = 'DEST_2,DEST_1,DEST_3,DEST_5';
            var mappingsAnswer = answerText;
            var mappingsCorrect = question.CorrectAnswer();
            var answerHtml = '';
            var correctHtml = '';
            var questionDetails = '';

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
            self.SectionInstruction(question.SectionInstruction());

            self.LoadImages();
            MathJax.Hub.Queue(["Typeset", MathJax.Hub]);
        },

        ShowCorrectAnswer: function (self, question) {
            // popupAlertMessage from ckeditor_utils
            var $contentCorrect = $(question.CorrectAnswerHTML);
            $contentCorrect.find('.highlighted').removeAttr('style').removeClass();

            question.CorrectAnswerHTML = $contentCorrect.outerHTML();
            popupAlertMessage('alert', question.CorrectAnswerHTML, 700, 500);
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

            self.LoadImagesSelector($selector);

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
