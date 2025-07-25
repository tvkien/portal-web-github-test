(function ($) {
    $.widget('jquery.MultipleChoiceVariable', {
        options: {
            MultipleChoiceUtil: null,
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

            var answerChoice = '';
            if (!options.MultipleChoiceUtil.IsNullOrEmpty(answerOfStudentForSelectedQuestion)) {
                self.PointsEarned(answerOfStudentForSelectedQuestion.PointsEarned());
                self.OldPointsEarned(answerOfStudentForSelectedQuestion.PointsEarned());
                self.AnswerID(answerOfStudentForSelectedQuestion.QTIOnlineTestSessionAnswerID());
                answerChoice = answerOfStudentForSelectedQuestion.AnswerChoice();
                self.AnswerImage(answerOfStudentForSelectedQuestion.AnswerImage());
                self.ShowHightLight(answerOfStudentForSelectedQuestion.HighlightQuestion(), question);
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
            }

            var tree = $('<div></div>');
            tree.addClass('box-answer');
            tree.html(question.ItemBody());
            $('<div class="clearfix"></div>').prependTo(tree);
            $('<div class="clearfix"></div>').appendTo(tree);

            tree.find('choiceInteraction[variablepoints=true]')
                .replaceWith(function () {
                    var choiceInteraction = $(this);
                    var responseIdentifier = $(choiceInteraction).attr('responseIdentifier');
                    var newChoiceInteraction = $('<div></div>');
                    newChoiceInteraction.html(choiceInteraction.html());
                    CopyAttributes(choiceInteraction, newChoiceInteraction);
                    newChoiceInteraction.find('simpleChoice').replaceWith(function () {
                        var simpleChoice = $(this);
                        var identifier = simpleChoice.attr('identifier');


                        var isStudentChoose = false;
                        if (!Reviewer.IsNullOrEmpty(answerChoice)) {
                            ko.utils.arrayForEach(answerChoice.split(','), function (item) {
                                if ($.trim(item) === identifier) isStudentChoose = true;
                            });
                        }

                        simpleChoice.find('.answer').each(function () {
                            var answerElement = $(this);
                            answerElement.removeClass('answer');
                        });
                        
                        var checkbox = $('<input type="checkbox" name="' + identifier + '" disabled="disabled" style="margin-right:10px;">');

                        var newSimpleChoice = $('<div></div>');
                        newSimpleChoice.html(simpleChoice.html());
                        CopyAttributes(simpleChoice, newSimpleChoice);
                        newSimpleChoice.addClass('white');
                        newSimpleChoice.addClass('answer');


                        if (isStudentChoose) {
                            newSimpleChoice.addClass("simplechoiceSelect");
                        }

                        var checkBoxHtml = self.MultipleChoiceClickMethod() === '0' ? checkbox.outerHTML() : '';
                        $('<div style="width:auto; margin-right:2px;">' + checkBoxHtml + identifier + '.</div>').prependTo(newSimpleChoice);

                        return $(newSimpleChoice.outerHTML());
                    });
                    if (self.PointsPossible() > 0)
                    {
                        if (self.PointsPossible() == self.PointsEarned()) {
                            //Show correct icon
                            newChoiceInteraction = $('<div>' + newChoiceInteraction.prop("outerHTML") + '<i class="jsIsAnswerCorrect mcVariableCorrect correct"></i>' + '</div>');
                        } else if (self.PointsPossible() > self.PointsEarned() && self.PointsEarned() > 0) {
                            //Show a haft correct
                            newChoiceInteraction = $('<div>' + newChoiceInteraction.prop("outerHTML") + '<i class="jsIsAnswerCorrect mcVariableCorrect partial"></i>' + '</div>');
                        } else {
                            //Show wrong
                            newChoiceInteraction = $('<div>' + newChoiceInteraction.prop("outerHTML") + '<i class="jsIsAnswerCorrect mcVariableCorrect incorrect"></i>' + '</div>');
                        }
                    }
                    

                    return $(newChoiceInteraction.outerHTML());
                });

           
            tree.find('p').replaceWith(function () {
                var p = $(this);
                var div = $('<div></div>');
                div.html(p.html());
                CopyAttributes(p, div);
                return $(div.outerHTML());
            });


            var questionDetails = tree.outerHTML();
            if (options.PostProcessQuestionDetails != null && typeof (options.PostProcessQuestionDetails) == "function") {
                questionDetails = options.PostProcessQuestionDetails(questionDetails);
            }

            self.Respones(questionDetails);
            self.SectionInstruction(question.SectionInstruction());

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
