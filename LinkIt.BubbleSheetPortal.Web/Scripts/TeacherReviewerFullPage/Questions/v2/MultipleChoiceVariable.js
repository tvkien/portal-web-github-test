(function ($) {
    $.widget('jquery.MultipleChoiceVariable', {
        options: {
            MultipleChoiceUtil: null,
            PostProcessQuestionDetails: null,
            Self: null
        },

        Display: function (question) {
            var that = this;
            var options = that.options;
            var self = options.Self;

            self.RefObjects(question.RefObjects());
            self.PointsPossible(question.PointsPossible());
            self.QTIItemSchemaID(question.QTIItemSchemaID());
            self.AnswerSubID('');

            var answerOfStudentForSelectedQuestion;

            if (self.TestOnlineSessionAnswers() !== null) {
                ko.utils.arrayForEach(self.TestOnlineSessionAnswers(), function (testOnlineSessionAnswer) {
                    if (question.VirtualQuestionID() === testOnlineSessionAnswer.VirtualQuestionID()) {
                        answerOfStudentForSelectedQuestion = testOnlineSessionAnswer;
                    }
                });
            }

            var answerChoice = '';
            if (!options.MultipleChoiceUtil.IsNullOrEmpty(answerOfStudentForSelectedQuestion)) {
                self.PointsEarned(answerOfStudentForSelectedQuestion.PointsEarned());
                self.OldPointsEarned(answerOfStudentForSelectedQuestion.PointsEarned());
                self.AnswerID(answerOfStudentForSelectedQuestion.QTIOnlineTestSessionAnswerID());
                answerChoice = answerOfStudentForSelectedQuestion.AnswerChoice();
                self.AnswerImage(answerOfStudentForSelectedQuestion.AnswerImage());
                self.ShowHightLight(answerOfStudentForSelectedQuestion.HighlightQuestion(), question);
                self.VisitedTimes(answerOfStudentForSelectedQuestion.VisitedTimes());
                self.TotalSpentTimeOnQuestion(answerOfStudentForSelectedQuestion.TotalSpentTimeOnQuestion());
            }
            else {
                question.XmlContent(question.DataXmlContent());
                self.PointsEarned(0);
                self.OldPointsEarned(0);
                self.AnswerID(0);
                answerChoice = '';
                self.AnswerImage('');
                self.ShowHightLight('', question);
                self.VisitedTimes(0);
                self.TotalSpentTimeOnQuestion('0s');
            }

            var tree = $('<div/>');
            tree.addClass('box-answer');
            tree.html(question.ItemBody());
            $('<div class="clearfix"/>').prependTo(tree);
            $('<div class="clearfix"/>').appendTo(tree);

            tree.find('choiceInteraction[variablepoints=true], choiceinteraction[variablepoints=true]')
                .replaceWith(function () {
                    var choiceInteraction = $(this);
                    var responseIdentifier = $(choiceInteraction).attr('responseIdentifier');
                    var newChoiceInteraction = $('<div/>');
                    newChoiceInteraction.html(choiceInteraction.html());
                    CopyAttributes(choiceInteraction, newChoiceInteraction);
                    newChoiceInteraction.find('simpleChoice').replaceWith(function () {
                        var simpleChoice = $(this);
                        var identifier = simpleChoice.attr('identifier');
                        var isCorrectIdentifier = false;

                        $(question.XmlContent()).find('responsedeclaration[identifier="' + responseIdentifier + '"] correctresponse value').each(function (i, el) {
                            if (el.getAttribute('identifier') === identifier && parseInt(el.innerHTML, 10) > 0) isCorrectIdentifier = true;
                        });

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

                        var newSimpleChoice = $('<div/>');
                        newSimpleChoice.html(simpleChoice.html());
                        CopyAttributes(simpleChoice, newSimpleChoice);
                        newSimpleChoice.addClass('white');
                        newSimpleChoice.addClass('answer');              

                        if (isStudentChoose) {

                            if (isCorrectIdentifier) {
                                newSimpleChoice.addClass('correctAnswer');
                            }

                            if (isCorrectIdentifier) {
                                newSimpleChoice.addClass('correctStudentChoose');
                                if (self.PointsPossible() > self.PointsEarned() && self.PointsEarned() > 0) {
                                    newSimpleChoice.addClass('partialStudentChoose');
                                }
                            } else {
                                newSimpleChoice.addClass('wrongStudentChoose');
                            }

                            newSimpleChoice.addClass("studentChoose simplechoiceSelect");

                            if (question.IsInformationalOnly()) {
                                newSimpleChoice.addClass('simplechoiceSelectInformationalOnly');
                            }

                            if (self.PointsPossible() > self.PointsEarned() && self.PointsEarned() > 0 && isCorrectIdentifier) {
                                $('<span class="jsIsUserAnswer"><i class="fa-solid fa-user me-2"></i> Student’s answer (Partially Correct)</span>').appendTo(newSimpleChoice);
                            } else {
                                $('<span class="jsIsUserAnswer"><i class="fa-solid fa-user me-2"></i> Student’s answer</span>').appendTo(newSimpleChoice);
                            }
                        }

                        var checkBoxHtml = self.MultipleChoiceClickMethod() === '0' ? checkbox.outerHTML() : '';
                        $('<div style="width:auto; margin-right:2px;">' + checkBoxHtml + identifier + '.</div>').prependTo(newSimpleChoice);

                        return $(newSimpleChoice.outerHTML());
                    });

                    if (self.PointsPossible() > 0 && !question.IsInformationalOnly()) {
                        var newChoiceInteractionHtml = newChoiceInteraction.prop('outerHTML');

                        if (self.PointsPossible() == self.PointsEarned()) {
                            //Show correct icon
                            newChoiceInteraction = $('<div>' + newChoiceInteractionHtml + '<i class="jsIsAnswerCorrect mcVariableCorrect correct"></i>' + '</div>');
                        } else if (self.PointsPossible() > self.PointsEarned() && self.PointsEarned() > 0) {
                            //Show a haft correct
                            //newChoiceInteraction = $('<div>' + newChoiceInteractionHtml + '<i class="jsIsAnswerCorrect mcVariableCorrect partial"></i>' + '</div>');
                        } else {
                            //Show wrong
                            newChoiceInteraction = $('<div>' + newChoiceInteractionHtml + '<i class="jsIsAnswerCorrect mcVariableCorrect incorrect"></i>' + '</div>');
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
        },

        _getHtmlValue: function (id) {
            return $.trim($('#' + id).html());
        },

        _setHtmlValue: function (id, val) {
            $('#' + id).html(val);
        }
    });
}(jQuery));
