(function ($) {
    $.widget('jquery.TextEntry', {
        options: {
            TextEntryUtil: null,
            PostProcessQuestionDetails: null,
            Self: null
        },

        Display: function (question) {
            var that = this;
            var options = that.options;
            var self = options.Self;
            
            var greaterThanOrEqual = '&#8805;';
            var lessThanOrEqual = '&#8804;';

            var responseProcessingTypeID = '';
            var selectedAnswer = '';
            if (!options.TextEntryUtil.IsNullOrEmpty(question.Answer())) {
                responseProcessingTypeID = question.Answer().ResponseProcessingTypeID();
                selectedAnswer = question.Answer().AnswerText();
            }

            var textEntryMark = $('');
            if (!options.TextEntryUtil.IsNullOrEmpty(self.QTIOnlineTestSessionID()) && (self.IsComplete() || self.IsPendingReview()) && responseProcessingTypeID != '2') {
                textEntryMark = $('<i class="jsIsAnswerCorrect" style="float:none;display:inline-block;position: relative;margin-left:15px;"></i>');
                if (self.PointsEarned() == self.PointsPossible()) {
                    textEntryMark.addClass('correct');
                } else if (self.PointsEarned() > 0) {
                    textEntryMark.addClass('partial');
                } else {
                    textEntryMark.addClass('incorrect');
                }
            }

            var tree = $('<div></div>');
            tree.html(question.ItemBody());
            $('<div class="clearfix"></div>').prependTo(tree);
            $('<div class="clearfix"></div>').appendTo(tree);
            
            //get data guidance from text entry
            var xmlContent = question.XmlContent();
            var objItemTextEntry = '';
            var objTextEntry = '';
            var arrCorrectResponse = [];
            var listTextEntry = [];
            var arrTypeMessage = [];
            var objItemMessage = '';
            var newListMessage = [];
            var iconString = '';
            var htmlListRationale = '';
            
            var tagsresponseDeclaration = $(xmlContent).find('responseDeclaration');
            tagsresponseDeclaration.each(function () {
                if ($(this).find('correctResponse value').length) {
                    var responseId = $(this).attr('identifier');
                    var tagsValue = $(this).find('correctResponse value');
                    for (var i = 0, lenTagsValue = tagsValue.length; i < lenTagsValue; i++) {
                        var tagValue = tagsValue[i];
                        if ($(tagValue).attr('identifier') != undefined) {
                            var identifier = $(tagValue).attr('identifier');
                            objItemTextEntry = {
                                identifier: identifier,
                                arrMessage: []
                            };
                            arrCorrectResponse.push(objItemTextEntry);
                        }

                    }
                    objTextEntry = {
                        responseIdentifier: responseId,
                        arrCorrectResponse: arrCorrectResponse
                    };
                    arrCorrectResponse = [];
                    listTextEntry.push(objTextEntry);
                }
            });
            if (tagsresponseDeclaration.find('responseRubric').length) {
                var tagsresponseRubric = tagsresponseDeclaration.find('responseRubric');

                for (var i = 0, lenTagsresponseRubric = tagsresponseRubric.length; i < lenTagsresponseRubric; i++) {
                    var responseIdentifier = $(tagsresponseRubric[i]).parent('responseDeclaration').attr('identifier');
                    var itemRubric = tagsresponseRubric[i];
                    var tagsValue = $(itemRubric).find('value');
                    for (var j = 0, lentagsValue = tagsValue.length; j < lentagsValue; j++) {
                        var objItemType = tagsValue[j];
                        var audio = $(objItemType).attr('audioref');
                        var typemessage = $(objItemType).attr('type');
                        var stringHtml = $(objItemType).html();
                        var ansIdentifier = $(objItemType).attr('ansIdentifier');

                        objItemMessage = {
                            typeMessage: typemessage,
                            audioRef: audio,
                            valueContent: stringHtml,
                            ansIdentifier: ansIdentifier
                        };
                        arrTypeMessage.push(objItemMessage);
                    }

                    for (var k = 0, lenlistTextEntry = listTextEntry.length; k < lenlistTextEntry; k++) {
                        var itemEntry = listTextEntry[k];
                        var newObjTypeMessage = '';
                        var itemHtml = '';

                        if (itemEntry.responseIdentifier === responseIdentifier) {
                            for (var m = 0, lenarrCorrectResponse = itemEntry.arrCorrectResponse.length; m < lenarrCorrectResponse; m++) {
                                var itemCorrect = itemEntry.arrCorrectResponse[m];
                                for (var n = 0, lenarrTypeMessage = arrTypeMessage.length; n < lenarrTypeMessage; n++) {
                                    if (arrTypeMessage[n].ansIdentifier === itemCorrect.identifier) {
                                        var isRatioContent = false;
                                        var $div = $('<div/>');

                                        $div.append(arrTypeMessage[n].valueContent);

                                        isRatioContent = Reviewer.GetGuidanceRationaleContent($div);

                                        if (isRatioContent) {
                                            itemCorrect.arrMessage.push({
                                                audioRef: arrTypeMessage[n].audioRef,
                                                valueContent: arrTypeMessage[n].valueContent,
                                                typeMessage: arrTypeMessage[n].typeMessage
                                            });

                                            newObjTypeMessage = {
                                                typeMessage: arrTypeMessage[n].typeMessage,
                                                audioRef: arrTypeMessage[n].audioRef,
                                                valueContent: arrTypeMessage[n].valueContent,
                                                identifier: arrTypeMessage[n].ansIdentifier,
                                                responseidentifier: responseIdentifier
                                            };
                                            if (newObjTypeMessage.typeMessage === 'rationale' ||
                                                newObjTypeMessage.typeMessage === 'guidance_rationale') {
                                                itemHtml = self.CreateGraphicGuidance(newObjTypeMessage, newObjTypeMessage.typeMessage, 'textEntryInteraction');
                                                htmlListRationale += itemHtml;
                                            }
                                        }
                                        newObjTypeMessage = '';
                                    }
                                }
                            }
                            newListMessage.push(itemEntry);
                        }
                    }
                    objItemMessage = '';
                    arrTypeMessage = [];
                }
            }

            tree.find('textEntryInteraction').replaceWith(function () {
                var textEntry = $(this);
                var responseIdentifier = textEntry.attr('responseIdentifier');

                //Replace if the item has an Item Type Interactions onto an Image
                if (textEntry.parents(".itemtypeonimage").length > 0) {
                    var getStyle = textEntry.attr("style");

                    textEntry.removeAttr("style");
                    textEntry.wrap('<div style="' + getStyle + '"></div>');
                }

                var newTextEntry = $('<span></span>');
                if (responseProcessingTypeID == '2') //Manual grade
                {
                    var notReviewed;
                    if (self.SelectedQuestion()) {
                        notReviewed = self.SelectedQuestion().NotYetReviewCSS();
                    }
                    if (notReviewed) {
                        newTextEntry.addClass('red-border');
                    } else {
                        newTextEntry.removeClass('red-border');
                    }
                }
                CopyAttributes(textEntry, newTextEntry);
                newTextEntry.addClass('input');
                newTextEntry.html(selectedAnswer);
                if (selectedAnswer !== "") {
                    newTextEntry.addClass('inputHasAnswered');
                }
                var correctAnswer = $('<span idresponse=' + responseIdentifier + ' class="correct-answer-teacher-review"></span>');
                correctAnswer.css('color', 'red');
                correctAnswer.text('Correct Answer:');
                $(question.XmlContent()).find('responsedeclaration[identifier="' + responseIdentifier + '"] > correctresponse > value').each(function () {
                    var value = $(this);
                    var correctAnswerText = '';
                    var startValue = '';
                    var endValue = '';
                    var startExclusivity = '';
                    var endExclusivity = '';
                    var identifier = $(this).attr('identifier');
                    value.find('rangeValue, rangevalue').each(function () {
                        if ($(this).find("name").text() === 'start') {
                            startExclusivity = $(this).find("exclusivity").text();
                            startValue = $(this).find("value").text();
                        } else if ($(this).find("name").text() === 'end') {
                            endExclusivity = $(this).find("exclusivity").text();
                            endValue = $(this).find("value").text();
                        }
                    });

                    var startOperator = startExclusivity == '1' ? '>' : greaterThanOrEqual;
                    var endOperator = endExclusivity == '1' ? '<' : lessThanOrEqual;

                    if (startValue != '') {
                        correctAnswerText = startOperator + ' ' + startValue;
                        if (endValue != '') {
                            correctAnswerText = correctAnswerText + ' and ' + endOperator + ' ' + endValue;
                        }
                    } else {
                        if (endValue != '') correctAnswerText = correctAnswerText + ' ' + endOperator + ' ' + endValue;
                    }

                    if (correctAnswerText == '') {

                        for (var i = 0, lenNewListMessage = newListMessage.length; i < lenNewListMessage; i++) {
                            var itemTextntry = newListMessage[i];
                            if (itemTextntry.responseIdentifier === responseIdentifier) {
                                for (var j = 0, lenArrCorrectResponse = itemTextntry.arrCorrectResponse.length; j < lenArrCorrectResponse; j++) {
                                    var objEntry = itemTextntry.arrCorrectResponse[j];
                                    if (objEntry.arrMessage.length) {
                                        if (objEntry.identifier === identifier) {
                                            for (var k = 0, lenArrMessage = objEntry.arrMessage.length; k < lenArrMessage; k++) {
                                                if (objEntry.arrMessage[k].typeMessage === 'rationale' || objEntry.arrMessage[k].typeMessage === 'guidance_rationale') {
                                                    iconString = '<img name="textEntryGuidance" responseidentifier="' + identifier + '" alt="Guidance" style="margin-left: 5px;" class="imageupload bntGuidance" src="../../Content/themes/TestMaker/images/guidance_checked_medium.png" title="">';
                                                }
                                            }
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        correctAnswerText = '<span identifier=' + identifier + '>' + value.html() + '</span>' + iconString;
                        iconString = '';
                    }

                    correctAnswer.append('<br/>' + correctAnswerText);
                });
                
                if (htmlListRationale != '') {
                    correctAnswer.append(htmlListRationale);
                }
                if (self.ResponseProcessingTypeID() == '3') return $(newTextEntry.outerHTML() + '<br>');

                //Replace if the item has an Item Type Interactions onto an Image
                if (textEntry.parents('.itemtypeonimage').length > 0) {
                    textEntry.parent().after(correctAnswer.outerHTML());
                    return $(newTextEntry.outerHTML() + textEntryMark.outerHTML() + '<br>');
                } else {
                    return $(newTextEntry.outerHTML() + textEntryMark.outerHTML() + '<br><br>' + correctAnswer.outerHTML() + '<br>');
                }
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
