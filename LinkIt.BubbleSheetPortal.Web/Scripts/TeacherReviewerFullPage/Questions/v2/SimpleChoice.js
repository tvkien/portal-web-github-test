(function ($) {
    $.widget('jquery.SimpleChoice', {
        options: {
            SimpleChoiceUtil: null,
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
            if (!options.SimpleChoiceUtil.IsNullOrEmpty(question.Answer())) answerChoice = question.Answer().AnswerChoice();

            var tree = $('<div/>');
            tree.addClass('box-answer');
            tree.html(question.ItemBody());
            tree.find('p').replaceWith(function () {
                var p = $(this);
                var div = $('<div/>');
                div.html(p.html());
                CopyAttributes(p, div);
                return $(div.outerHTML());
            });

            $('<div class="clearfix"/>').prependTo(tree);
            $('<div class="clearfix"/>').appendTo(tree);
            tree.find('choiceInteraction')
                .replaceWith(function () {
                    var choiceInteraction = $(this);
                    var responseIdentifier = $(choiceInteraction).attr('responseIdentifier');
                    var originChoiceInteraction = $(question.DataXmlContent()).find('[responseIdentifier="' + responseIdentifier + '"]');
                    var newChoiceInteraction = $('<div/>');
                    newChoiceInteraction.html(choiceInteraction.html());
                    CopyAttributes(choiceInteraction, newChoiceInteraction);
                    newChoiceInteraction.find('simpleChoice').replaceWith(function () {
                        var simpleChoice = $(this);
                        var identifier = simpleChoice.attr('identifier');
                        var originSimpleChoice = originChoiceInteraction.find('simpleChoice[identifier="' + identifier + '"]');
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
                      if (simpleChoice.find('div[typemessage]').length || originSimpleChoice.find('div[typemessage]').length) {

                           var iconString = '<img identifier="' + identifier + '" alt="Guidance" style="margin-left: 5px; width: 1.5em; height: 1.5em;" class="imageupload bntGuidance" src="../../Content/themes/TestMaker/v2/multiplechoice_images_guidance_unchecked.svg" title="">';
                            var tagGuidances = simpleChoice.find('div[typemessage]');
                            if (tagGuidances.length === 0) {
                              tagGuidances = originSimpleChoice.find('div[typemessage]');
                            }
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
                        var checkbox = '<input name="' + responseIdentifier + '" type="radio" disabled="disabled" style="margin-right:10px;">';

                        var newSimpleChoice = $('<div></div>');
                        newSimpleChoice.html(simpleChoice.html());
                        CopyAttributes(simpleChoice, newSimpleChoice);
                        newSimpleChoice.addClass('white');
                        newSimpleChoice.addClass('answer');

                        var answerDiv = '';
                        if (isCorrectIdentifier) {
                            newSimpleChoice.addClass('green');
                            newSimpleChoice.css('border', '1px solid green');
                            answerDiv = '<i class="jsIsAnswerCorrect correct"></i>';
                        }

                        if (isStudentChoose) {
                          checkbox = '<input name="' + responseIdentifier + '" type="radio" disabled="disabled" style="margin-right:10px;" checked>';
                          newSimpleChoice.addClass('studentChoose studentSingleChoice')
                          $('<span class="jsIsUserAnswer"><i class="fa-solid fa-user me-2"></i> Student’s answer</span>').appendTo(newSimpleChoice);
                            if (question.IsInformationalOnly()) {
                                newSimpleChoice.addClass('simplechoiceSelectInformationalOnly');
                          }

                          if (!isCorrectIdentifier) {
                            newSimpleChoice.css('border', '1px solid red');
                            newSimpleChoice.addClass('answerbg');
                            answerDiv = '<i class="jsIsAnswerCorrect incorrect"></i>';
                          }
                        }

                        var checkBoxHtml = self.MultipleChoiceClickMethod() === '0' ? checkbox : '';
                        $('<div style="width:auto; margin-right:2px;">' + checkBoxHtml + identifier + '.</div>' + answerDiv).prependTo(newSimpleChoice);

                        return $(newSimpleChoice.outerHTML());
                    });

                    return $(newChoiceInteraction.outerHTML());
                });

            var questionDetails = tree.outerHTML();
            if (options.PostProcessQuestionDetails != null && typeof (options.PostProcessQuestionDetails) == "function") {
                questionDetails = options.PostProcessQuestionDetails(questionDetails);
            }

            self.Respones(questionDetails);
        },

        _getHtmlValue: function (id) {
            return $.trim($('#' + id).html());
        },

        _setHtmlValue: function (id, val) {
            $('#' + id).html(val);
        }
    });
}(jQuery));
