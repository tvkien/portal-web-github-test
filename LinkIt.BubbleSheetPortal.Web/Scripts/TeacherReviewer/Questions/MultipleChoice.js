(function ($) {
    $.widget('jquery.MultipleChoice', {
        options: {
            MultipleChoiceUtil: null,
            PostProcessQuestionDetails: null
        },

        Display: function (self, question) {
            var that = this;
            var options = that.options;
            
            //apply guidance and rationale
            var objTypeMessage = '';
            var htmlGuidanceRationale = '';
            var itemHtml = '';
            
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
                self.DisplayItemFeedback(true,answerOfStudentForSelectedQuestion);
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
                        var isCorrectIdentifier = false;
                        $(question.XmlContent()).find('responsedeclaration[identifier="' + responseIdentifier + '"] correctresponse value').each(function () {
                            if ($(this).text() === identifier) isCorrectIdentifier = true;
                        });

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
                                var audio = $(itemGuidance).attr('audioref');
                                var typemessage = $(itemGuidance).attr('typemessage');
                                var stringHtml = $(itemGuidance).html();
                                var isRatioContent = false;

                                isRatioContent = Reviewer.GetGuidanceRationaleContent($(itemGuidance));

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
                        newSimpleChoice.addClass('white');
                        newSimpleChoice.addClass('answer');

                        if (isCorrectIdentifier) {
                            newSimpleChoice.addClass('green');
                        }
                        if (isStudentChoose) {
                            checkbox = $('<input type="checkbox" name="' + identifier + '" disabled="disabled" style="margin-right:10px;" checked>');
                            if (isCorrectIdentifier) {
                                newSimpleChoice.css('border', '1px solid green');
                                $('<i class="jsIsAnswerCorrect correct"></i>').appendTo(newSimpleChoice);
                            } else {
                                newSimpleChoice.css('border', '1px solid red');
                                $('<i class="jsIsAnswerCorrect incorrect"></i>').appendTo(newSimpleChoice);
                            }
                        }

                        var checkBoxHtml = self.MultipleChoiceClickMethod() === '0' ? checkbox.outerHTML() : '';
                        $('<div style="width:auto; margin-right:2px;">' + checkBoxHtml + identifier + '.</div>').prependTo(newSimpleChoice);

                        return $(newSimpleChoice.outerHTML());
                    });

                    return $(newChoiceInteraction.outerHTML());
                });
            
            //.replaceAll('<p', "<div").replaceAll('</p>', "</div>");
           
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
