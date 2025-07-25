(function ($) {
    $.widget('jquery.InlineChoice', {
        options: {
            InlineChoiceUtil: null,
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

            var testOnlineSessionAnswer;
            ko.utils.arrayForEach(self.TestOnlineSessionAnswers(), function (item) {
                if (question.VirtualQuestionID() === item.VirtualQuestionID()) {
                    testOnlineSessionAnswer = item;
                }
            });

            var selectedAnswer = '';
            if (!options.InlineChoiceUtil.IsNullOrEmpty(testOnlineSessionAnswer)) {
                selectedAnswer = testOnlineSessionAnswer.AnswerChoice();
                self.PointsEarned(testOnlineSessionAnswer.PointsEarned());
                self.OldPointsEarned(testOnlineSessionAnswer.PointsEarned());
                self.AnswerID(testOnlineSessionAnswer.QTIOnlineTestSessionAnswerID());
                self.AnswerImage(testOnlineSessionAnswer.AnswerImage());
                self.PointsPossible(question.PointsPossible());
                if (!Reviewer.IsNullOrEmpty(testOnlineSessionAnswer.HighlightQuestion()))
                    self.ShowHightLight(testOnlineSessionAnswer.HighlightQuestion(), question);
                self.DisplayItemFeedback(true,testOnlineSessionAnswer);
            } else {
                question.XmlContent(question.DataXmlContent());
                selectedAnswer = '';
                self.PointsEarned(0);
                self.OldPointsEarned(0);
                self.AnswerID(0);
                self.AnswerImage('');
                self.PointsPossible(0);
                self.DisplayItemFeedback(false, null);
            }

            var correctAnswer = '';
            var correctIdentifier = '';
            var tree = $('<div></div>');
            tree.html(question.ItemBody());
            $('<div class="clearfix"></div>').prependTo(tree);
            $('<div class="clearfix"></div>').appendTo(tree);
            
            //apply guidance and rationale
            var listMessageGuidance = [];
            var resIdentifier = '';// show idres to update anwser for inline drop down
            
            var arrMessageGuidance = [];
            var listObjTypeMessages = [];
            var objTypeMessage = '';
            var htmlContentGuidance = '';
            var htmlGuidance = '';
            var responseIdentifier = '';
            var iconString = '';
            
            tree.find('.boxedText').replaceWith(function () {
                var $me = $(this);
                var contentHTML = $me.html();

                if ($me.prop("tagName").toLowerCase() == "p") {
                    var attrBoxes = "";
                    $(this).each(function () {
                        $.each(this.attributes, function () {
                            // this.attributes is not a plain object, but an array
                            // of attribute nodes, which contain both the name and value
                            if (this.specified) {
                                attrBoxes += '' + this.name + '="' + this.value + '" '
                            }
                        });
                    });

                    var $newBoxes = $('<div ' + attrBoxes + '>' + contentHTML + '</div>');
                    return $newBoxes
                }
            });

            tree.find('inlinechoiceinteraction').replaceWith(function () {
                var inlineChoice = $(this);
                    responseIdentifier = inlineChoice.attr('responseIdentifier');
                var newInlineChoice = $('<div></div>');
                newInlineChoice.html(inlineChoice.html());

                //newInlineChoice.append('<li></li>');
                newInlineChoice.prepend('<li></li>');
                newInlineChoice.find('inlinechoice').replaceWith(function () {

                    if ($(this).find('div[typemessage]').length) {
                        $(this).find('div[typemessage]').find('inlinechoice').remove();
                        var arrGuidances = $(this).find('div[typemessage]');
                        var valItemOption = '';
                        for (var i = 0, lenArrMessage = arrGuidances.length; i < lenArrMessage; i++) {
                            arrMessageGuidance.push(arrGuidances[i]);
                        }
                        listMessageGuidance.push({
                            identifier: $(this).attr('identifier'),
                            arrMessageGuidance: arrMessageGuidance,
                            value: ''
                        });
                    }

                    var taginlinechoice = this.outerHTML;
                    for (var j = 0, lenArrMessageGuidance = arrMessageGuidance.length; j < lenArrMessageGuidance; j++) {
                        taginlinechoice = taginlinechoice.replace(arrMessageGuidance[j].outerHTML, '');
                    }
                    valItemOption = $(taginlinechoice).find('.inlineChoiceAnswer').html();
                    for (var k = 0, lenlistMessageGuidance = listMessageGuidance.length; k < lenlistMessageGuidance; k++) {
                        if (listMessageGuidance[k].identifier === $(this).attr('identifier')) {
                            listMessageGuidance[k].value = valItemOption;
                        }
                    }

                    var item = $(taginlinechoice);
                    var identifier = item.attr('identifier');
                    $(question.XmlContent()).find('responsedeclaration[identifier="' + responseIdentifier + '"] correctresponse value').each(function () {
                        if ($(this).text() == identifier) {
                            //correctAnswer = item.text();
                            correctAnswer = item.html();
                            correctIdentifier = identifier;
                        }
                    });
                    
                    var newItem = $('<li></li>');
                    newItem.html(item.html());
                    CopyAttributes(item, newItem);
                    newItem.attr('disabled', 'disabled');
                    if (selectedAnswer === identifier) newItem.attr('selected', 'selected');
                    arrMessageGuidance = [];
                    //update anwser inline choice drop down
                    resIdentifier = responseIdentifier;
                    
                    return $(newItem.outerHTML());
                });

                var mark = $('<i style="z-index: 0;float:none;display:inline-block;position: relative;margin-left:15px;" class="jsIsAnswerCorrect"></i>');
                for (var icount = 0, lenlistMessageGuidance = listMessageGuidance.length; icount < lenlistMessageGuidance; icount++) {
                    var itemType = listMessageGuidance[icount];
                    var id = itemType.identifier;
                    var value = itemType.value;
                    var arrMessage = [];

                    for (var i = 0, leMessageGuidance = itemType.arrMessageGuidance.length; i < leMessageGuidance; i++) {
                        var objItemType = itemType.arrMessageGuidance[i];
                        var audio = $(objItemType).attr('audioref');
                        var typemessage = $(objItemType).attr('typemessage');
                        var stringHtml = $(objItemType).html();
                        var isRatioContent = false;

                        isRatioContent = Reviewer.GetGuidanceRationaleContent($(objItemType));

                        if (isRatioContent) {
                            objTypeMessage = {
                                typeMessage: typemessage,
                                audioRef: audio,
                                valueContent: stringHtml
                            };

                            arrMessage.push(objTypeMessage);
                        }
                        objTypeMessage = '';
                    }

                    listObjTypeMessages.push({
                        identifier: id,
                        value: value,
                        arrMessage: arrMessage
                    });

                    arrMessage = [];
                }

                if (listObjTypeMessages.length) {
                    htmlGuidance = self.buildGuidanceInlineChoice(listObjTypeMessages, responseIdentifier);
                }
                
                if (htmlGuidance != '') {
                    iconString = '<img responseidentifier="' + responseIdentifier + '" alt="Guidance" style="margin-left: 5px;" class="imageupload bntGuidance" src="../../Content/themes/TestMaker/images/guidance_checked_medium.png" title="">';
                    htmlContentGuidance = '<div name="typeGuidance" style="display: none; border: 1px solid #999;padding: 5px;" idReponse="' + responseIdentifier + '">' + htmlGuidance + '</div>';
                }
                
                if (correctIdentifier === selectedAnswer) {
                    mark.addClass('correct');
                } else {
                    mark.addClass('incorrect');
                }
       
                var correctPanel = $('<p class="correct-answer-teacher-review" style="color:red; display: inline-block;"></p>');
                correctPanel.html('Correct Answer: ' + correctAnswer);
                var select = $('<ul class="inlineChoiceFormat"></ul>');
                CopyAttributes(inlineChoice, select);
                select.html(newInlineChoice.html());
                //select.css('max-width', '100%');
                
                select.css({
                    'visibility': 'hidden',
                    'height': 0,
                    'overflow': 'hidden',
                    'position': 'absolute'
                });

                if (selectedAnswer != '') {
                    return $(select.outerHTML() + mark.outerHTML() + '<br>' + htmlContentGuidance + correctPanel.outerHTML() + iconString + '<br>');
                } else {
                    return $(select.outerHTML() + iconString + mark.outerHTML() + '<br>' + htmlContentGuidance + correctPanel.outerHTML() + '<br>');
                }
                
                
            });

            var questionDetails = tree.outerHTML();
            if (options.PostProcessQuestionDetails != null && typeof (options.PostProcessQuestionDetails) == "function") {
                questionDetails = options.PostProcessQuestionDetails(questionDetails);
            }
            self.SectionInstruction(question.SectionInstruction());
            self.Respones(questionDetails);

            //show dropdown inline choice customize
            $('.inlineChoiceFormat').selectbox({ speed: 100, is_Disabled: true });

            if (selectedAnswer != "" && selectedAnswer != 'U') {
                var htmlAnswer = $('ul[responseidentifier="' + resIdentifier + '"]').find('li[identifier="' + selectedAnswer + '"]').html();
                $('a[responseidentifier="' + resIdentifier + '"]').attr('identifier', selectedAnswer).html(htmlAnswer);
                $('a[responseidentifier="' + resIdentifier + '"]').parent().css({                    
                    'width': 'auto',
                });

                var wParent = $('a[responseidentifier="' + resIdentifier + '"]').parent().width();
                var wMainBody = $('#divQuestionDetails .mainBody').width();
                
                if (wParent > wMainBody) {
                    $('a[responseidentifier="' + resIdentifier + '"]').parent().css({
                        'width': wMainBody,
                    });
                }
                
                //reset question when switch other question
                resIdentifier = '';
            }
           
            var tagsUl = $('#divQuestionDetails .mainBody:visible').find('.inlineChoiceFormat');
            $.each(tagsUl, function (idx, tagul) {
                var idRes = $(tagul).attr('responseidentifier');
                var wtagUl = $(tagul).show().width() + 52;
                if ($('#divQuestionDetails .mainBody:visible').width() < wtagUl) {
                    wtagUl = $('#divQuestionDetails .mainBody:visible').width();
                }
                $('.sbSelector[responseidentifier="' + idRes + '"]').parent().css({ 'max-width': wtagUl, 'width': '100%' });
               
            });
            
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
