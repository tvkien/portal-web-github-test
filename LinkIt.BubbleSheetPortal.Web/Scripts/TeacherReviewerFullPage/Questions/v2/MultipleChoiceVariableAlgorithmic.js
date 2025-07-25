(function ($) {
    $.widget('jquery.MultipleChoiceVariableAlgorithmic', {
        options: {
            MultipleChoiceAlgorithmicUtil: null,
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
            if (!options.MultipleChoiceAlgorithmicUtil.IsNullOrEmpty(answerOfStudentForSelectedQuestion)) {
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
            var tree = $('<div></div>');
            tree.addClass('box-answer');

            tree.html(question.ItemBody());
            $('<div class="clearfix"></div>').prependTo(tree);
            $('<div class="clearfix"></div>').appendTo(tree);

            tree.find('choiceInteraction[variablepoints=true], choiceinteraction[variablepoints=true]')
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
                          newSimpleChoice.addClass("studentChoose simplechoiceSelect");
                          $('<span class="jsIsUserAnswer"><i class="fa-solid fa-user me-2"></i> Student’s answer</span>').appendTo(newSimpleChoice);
                        }

                        var checkBoxHtml = self.MultipleChoiceClickMethod() === '0' ? checkbox.outerHTML() : '';
                        $('<div style="width:auto; margin-right:2px;" class="answer-label">' + checkBoxHtml + identifier + '.</div>').prependTo(newSimpleChoice);

                        return $(newSimpleChoice.outerHTML());
                    });

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

            if (question.AlgorithmicCorrectAnswers() != null && question.AlgorithmicCorrectAnswers().length) {
                questionDetails += '<br> <div class="btn-show-all-correct-answer big-button" onClick="ShowAllCorrectAnswers()">Show all correct answers</div>';
            }

            if (options.PostProcessQuestionDetails != null && typeof (options.PostProcessQuestionDetails) == "function") {
                questionDetails = options.PostProcessQuestionDetails(questionDetails);
            }

            self.Respones(questionDetails);
            self.SectionInstruction(question.SectionInstruction());
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

                if (typeof item.Amount === 'function' && item.Amount() > 0) {
                    correctAnswer = item.ConditionValue();
                    tree = that.FillCorrectAnswer(self, question, correctAnswer.toString(), true, item.Amount(), item.PointsEarned());
                    tree.appendTo(algorithmicCorrectAnswers);
                    algorithmicPoints.push(item.PointsEarned);
                } else {
                    correctAnswer = item.ConditionValue().toString();
                    tree = that.FillCorrectAnswer(self, question, correctAnswer);
                    tree.appendTo(algorithmicCorrectAnswers);
                    algorithmicPoints.push(item.PointsEarned);
                }
            });

            algorithmicCorrectAnswers.find('label.sc-label').each(function(index, element) {
                !$(element).text() && $(element).remove();
            })
            algorithmicCorrectAnswers.find('.highlighted').each(function(index, element) {
                $(element).replaceWith($(element).html())
            })
            var questionDetails = algorithmicCorrectAnswers.outerHTML();
            Reviewer.popupAlertMessage(questionDetails, 'ui-popup-fullpage ui-popup-algorithmic-correct-answer', 700, 500, false);
            Reviewer.createTabWidget('.ui-popup-fullpage.ui-popup-algorithmic-correct-answer', algorithmicPoints);
        },


        FillCorrectAnswer: function (self, question, correctAnswers, isAtleast, amount, pointEarned) {
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

            var tree = $('<div></div>');
            tree.addClass('box-answer');

            if (isAtleast) {
                var elAtleast = Reviewer.getAtleast(amount, pointEarned, question.QTIItemSchemaID());
                tree.append(elAtleast.outerHTML);
            }

            tree.append(question.ItemBody());
            $('<div class="clearfix"></div>').prependTo(tree);
            $('<div class="clearfix"></div>').appendTo(tree);

            tree.find('choiceInteraction[variablepoints=true], choiceinteraction[variablepoints=true]')
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
                        if (!Reviewer.IsNullOrEmpty(correctAnswers)) {
                            ko.utils.arrayForEach(correctAnswers.split(','), function (item) {
                                if ($.trim(item) === identifier) isStudentChoose = true;
                            });
                        }

                        simpleChoice.find('.answer').each(function () {
                            var answerElement = $(this);
                            answerElement.css('float', 'unset');
                            answerElement.removeClass('answer');
                        });

                        var checkbox = $('<input type="checkbox" name="' + identifier + '" disabled="disabled" style="margin-right:10px;">');

                        var newSimpleChoice = $('<div></div>');
                        newSimpleChoice.html(simpleChoice.html());
                        CopyAttributes(simpleChoice, newSimpleChoice);
                        newSimpleChoice.addClass('white');
                        newSimpleChoice.addClass('answer');


                        if (isStudentChoose) {
                            newSimpleChoice.addClass('green').css('border', '1px solid green');
                            newSimpleChoice.css('margin-bottom', '5px');
                        }

                        var checkBoxHtml = self.MultipleChoiceClickMethod() === '0' ? checkbox.outerHTML() : '';
                        $('<div style="width:auto; margin-right:2px;">' + checkBoxHtml + identifier + '.</div>').prependTo(newSimpleChoice);

                        return $(newSimpleChoice.outerHTML());
                    });

                    return $(newChoiceInteraction.outerHTML());
                });


            tree.find('p').replaceWith(function () {
                var p = $(this);
                var div = $('<div></div>');
                div.html(p.html());
                CopyAttributes(p, div);
                return $(div.outerHTML());
            });

            return tree;
        },

        _getHtmlValue: function (id) {
            return $.trim($('#' + id).html());
        },

        _setHtmlValue: function (id, val) {
            $('#' + id).html(val);
        }
    });
}(jQuery));
