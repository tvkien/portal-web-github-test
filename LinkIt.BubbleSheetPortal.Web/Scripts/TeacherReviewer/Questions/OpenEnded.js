(function ($) {
    $.widget('jquery.OpenEnded', {
        options: {
            OpenEndedUtil: null,
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
                if (question.VirtualQuestionID() == testOnlineSessionAnswer.VirtualQuestionID()) {
                    answerOfStudentForSelectedQuestion = testOnlineSessionAnswer;
                }
            });
            var baseQuestion = null;
            if (question.IsGhostVirtualQuestion()) {
                ko.utils.arrayForEach(self.Questions(), function (item) {
                    if (question.BaseVirtualQuestionID() == item.VirtualQuestionID()) {
                        baseQuestion = item;
                    }
                });
            }

            if (baseQuestion != null) {
                question.XmlContent(baseQuestion.XmlContent());
                question.DataXmlContent(baseQuestion.XmlContent());
                question.SectionInstruction(baseQuestion.SectionInstruction());

                if (!options.OpenEndedUtil.IsNullOrEmpty(answerOfStudentForSelectedQuestion)) {
                    ko.utils.arrayForEach(self.TestOnlineSessionAnswers(), function (answerOfBaseQuestion) {
                        if (baseQuestion.VirtualQuestionID() === answerOfBaseQuestion.VirtualQuestionID()) {
                            answerOfStudentForSelectedQuestion.AnswerText(answerOfBaseQuestion.AnswerText());
                            answerOfStudentForSelectedQuestion.AnswerImage(answerOfBaseQuestion.AnswerImage());
                            answerOfStudentForSelectedQuestion.HighlightQuestion(answerOfBaseQuestion.HighlightQuestion());
                            self.DisplayItemFeedback(true,answerOfStudentForSelectedQuestion);
                        }
                    });
                }
            }

            var answerChoice = '';
            var responseProcessingTypeID = '';
            if (!options.OpenEndedUtil.IsNullOrEmpty(answerOfStudentForSelectedQuestion)) {
                self.PointsEarned(answerOfStudentForSelectedQuestion.PointsEarned());
                self.OldPointsEarned(answerOfStudentForSelectedQuestion.PointsEarned());
                self.AnswerID(answerOfStudentForSelectedQuestion.QTIOnlineTestSessionAnswerID());
                answerChoice = answerOfStudentForSelectedQuestion.AnswerText();
                self.AnswerImage(answerOfStudentForSelectedQuestion.AnswerImage());
                self.ShowHightLight(answerOfStudentForSelectedQuestion.HighlightQuestion(), question);                
                responseProcessingTypeID = answerOfStudentForSelectedQuestion.ResponseProcessingTypeID();
                self.ResponseProcessingTypeID(answerOfStudentForSelectedQuestion.ResponseProcessingTypeID());
                self.DisplayItemFeedback(true, answerOfStudentForSelectedQuestion);
            }
            else {
                question.XmlContent(question.DataXmlContent());
                self.PointsEarned(0);
                self.OldPointsEarned(0);
                self.AnswerID(0);
                answerChoice = '';
                self.AnswerImage('');
                self.ShowHightLight('', question);
                self.DisplayItemFeedback(false, null);
                self.ResponseProcessingTypeID(0);
            }
			
			if(answerChoice == null) answerChoice = '';
			
            var drawable = false;
            var tree = $('<div></div>');
            tree.addClass('box-answer');
            tree.html(question.ItemBody());
            $('<div class="clearfix"></div>').prependTo(tree);
            $('<div class="clearfix"></div>').appendTo(tree);
            tree.find('extendedTextInteraction,extendedtextInteraction').replaceWith(function () {
                var notReviewed;
                if (self.SelectedQuestion()) {
                    notReviewed = self.SelectedQuestion().NotYetReviewCSS();
                }

                var extendedText = $(this);
                var extendedTextHeight = parseInt(extendedText.get(0).style.height.replace('px', ''), 10);
                extendedText.attr('notReviewed', notReviewed);
                drawable = extendedText.attr('drawable');
                if (drawable == 'true') {
                    return $(extendedText.outerHTML());
                }

                var newExtendedText = $('<div/>');
                newExtendedText.addClass('textarea openEndedText');

                if (notReviewed) {
                    newExtendedText.addClass('red-border');
                } else {
                    newExtendedText.removeClass('red-border');
                }

                if (self.ResponseProcessingTypeID() == 3)
                    newExtendedText.removeClass('red-border');

                // Set default height extended text is 90 if before not set height
                if (isNaN(extendedTextHeight)) {
                    extendedTextHeight = 90;
                }

                newExtendedText.css('height', extendedTextHeight + 'px');

                CopyAttributes(extendedText, newExtendedText);
                newExtendedText.html(answerChoice.replace(/\r?\n/g, '<br />'));

                return $('<div>Answer Area</div>' + newExtendedText.outerHTML());
            });

            var questionDetails = tree.outerHTML();
            if (options.PostProcessQuestionDetails != null && typeof (options.PostProcessQuestionDetails) == "function") {
                questionDetails = options.PostProcessQuestionDetails(questionDetails);
            }

            self.SectionInstruction(question.SectionInstruction());
            self.Respones(questionDetails);

            self.LoadImages();

            MathJax.Hub.Queue(["Typeset", MathJax.Hub]);
        },

        _getHtmlValue: function (id) {
            return $.trim($('#' + id).html());
        },

        _setHtmlValue: function (id, val) {
            $('#' + id).html(val);
        }
    });
}(jQuery));
