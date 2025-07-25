(function ($) {
    $.widget('jquery.MultipleChoiceAlgorithmic', {
        options: {
            MultipleChoiceAlgorithmicUtil: null,
            PostProcessQuestionDetails: null,
            Self: null
        },

        Display: function (question) {
            var that = this;
            var options = that.options;
            var self = options.Self;

            //apply guidance and rationale
            var objTypeMessage = '';
            var htmlGuidanceRationale = '';
            var itemHtml = '';

            var answerChoice = '';
            if (!Reviewer.IsNullOrEmpty(question.Answer())) {
                answerChoice = question.Answer().AnswerChoice();
            }

            var tree = $('<div></div>');
            tree.addClass('box-answer');
            tree.html(question.ItemBody());
            $('<div class="clearfix"></div>').prependTo(tree);
            $('<div class="clearfix"></div>').appendTo(tree);
            tree.find('choiceInteraction')
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
                            answerElement.after('<span style="float: right; padding-right: 40px;" class="iconGuidance"></span>');
                            answerElement.removeClass('answer');
                        });
                        //get data guidance if exist
                        if (simpleChoice.find('div[typemessage]').length) {

                            var iconString = '<img identifier="' + identifier + '" alt="Guidance" style="margin-left: 5px;" class="imageupload bntGuidance" src="../../Content/themes/TestMaker/images/guidance_checked_medium.png" title="">';
                            var tagGuidances = simpleChoice.find('div[typemessage]');
                            var isShowIconGuidance = false;
                            for (var i = 0, lenTagGuidances = tagGuidances.length; i < lenTagGuidances; i++) {
                                var itemGuidance = tagGuidances[i];
                                var $itemGuidance = $(itemGuidance);
                                var audio = $itemGuidance.attr('audioref');
                                var typemessage = $itemGuidance.attr('typemessage');
                                var stringHtml = $itemGuidance.html();
                                var isRatioContent = false;

                                isRatioContent = Reviewer.GetGuidanceRationaleContent($itemGuidance);

                                if (isRatioContent) {
                                    if (typemessage === 'rationale' || typemessage === 'guidance_rationale') {
                                        objTypeMessage = {
                                            typeMessage: typemessage,
                                            audioRef: audio,
                                            valueContent: stringHtml,
                                            identifier: identifier,
                                            responseidentifier: $(choiceInteraction).attr('responseidentifier')
                                        };

                                        itemHtml = self.CreateGraphicGuidance(objTypeMessage, objTypeMessage.typeMessage, 'simpleChoice');
                                        htmlGuidanceRationale += itemHtml;
                                        isShowIconGuidance = true;
                                    }
                                }

                                objTypeMessage = '';
                                itemHtml = '';
                            }
                            simpleChoice.find('div[typemessage]').remove();
                            if (isShowIconGuidance) {
                                simpleChoice.find('.iconGuidance').html(iconString);
                            }
                            simpleChoice.append(htmlGuidanceRationale);
                            simpleChoice.attr('stylename', 'answer');
                            htmlGuidanceRationale = '';
                        }
                        var checkbox = $('<input type="checkbox" name="' + identifier + '" disabled="disabled" style="margin-right:10px;">');

                        var newSimpleChoice = $('<div></div>');
                        newSimpleChoice.html(simpleChoice.html());
                        CopyAttributes(simpleChoice, newSimpleChoice);
                        newSimpleChoice.addClass('white').addClass('answer');

                        if (isStudentChoose) {
                            newSimpleChoice.addClass('answer-student');
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

            var questionDetails = tree.outerHTML();

            if (question.AlgorithmicCorrectAnswers() != null && question.AlgorithmicCorrectAnswers().length) {
                questionDetails += '<br> <div class="btn-show-all-correct-answer big-button" onClick="ShowAllCorrectAnswers()">Show all correct answers</div>';
            }

            if (options.PostProcessQuestionDetails != null && typeof (options.PostProcessQuestionDetails) == "function") {
                questionDetails = options.PostProcessQuestionDetails(questionDetails);
            }

            self.Respones(questionDetails);
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
                    correctAnswer = item.ConditionValue().toString();
                    tree = that.FillCorrectAnswer(self, question, correctAnswer, true, item.Amount(), item.PointsEarned());
                    tree.appendTo(algorithmicCorrectAnswers);
                    algorithmicPoints.push(item.PointsEarned);
                } else {
                    correctAnswer = item.ConditionValue().toString();
                    tree = that.FillCorrectAnswer(self, question, correctAnswer);
                    tree.appendTo(algorithmicCorrectAnswers);
                    algorithmicPoints.push(item.PointsEarned);
                }
            });

            var questionDetails = algorithmicCorrectAnswers.outerHTML();
            Reviewer.popupAlertMessage(questionDetails, 'ui-popup-fullpage ui-popup-algorithmic-correct-answer', 700, 500);
            Reviewer.createTabWidget('.ui-popup-fullpage.ui-popup-algorithmic-correct-answer', algorithmicPoints);
        },

        FillCorrectAnswer: function (self, question, correctAnswers, isAtleast, amount, pointEarned) {
            var tree = $('<div></div>');
            tree.addClass('box-answer');
            if (isAtleast) {
                var elAtleast = Reviewer.getAtleast(amount, pointEarned, question.QTIItemSchemaID());
                tree.append(elAtleast.outerHTML);
            }
            tree.append(question.ItemBody());
            $('<div class="clearfix"></div>').prependTo(tree);
            $('<div class="clearfix"></div>').appendTo(tree);
            tree.find('choiceInteraction')
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
                            answerElement.after('<span style="float: right; padding-right: 40px;" class="iconGuidance"></span>');
                            answerElement.css('float', 'unset');
                            answerElement.removeClass('answer');
                        });
                        //get data guidance if exist

                        var checkbox = $('<input type="checkbox" name="' + identifier + '" disabled="disabled" style="margin-right:10px;">');

                        var newSimpleChoice = $('<div></div>');
                        newSimpleChoice.html(simpleChoice.html());
                        CopyAttributes(simpleChoice, newSimpleChoice);
                        newSimpleChoice.addClass('white').addClass('answer');

                        if (isStudentChoose) {
                            newSimpleChoice.addClass('green').css('border', '1px solid green').css('margin-bottom', '5px');
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
