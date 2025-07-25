(function ($) {
    $.widget('jquery.ComplexItem', {
        options: {
            ComplexItemUtil: null,
            PostProcessQuestionDetails: null
        },

        Display: function (self, question) {
            var that = this;
            var options = that.options;
            var aSelectAnswerInlineChoice = [];

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

            if (!options.ComplexItemUtil.IsNullOrEmpty(testOnlineSessionAnswer)) {
                self.ShowHightLight(testOnlineSessionAnswer.HighlightQuestion(), question);
                self.DisplayItemFeedback(true,testOnlineSessionAnswer);
            } else {
                self.DisplayItemFeedback(false, null);
                question.XmlContent(question.DataXmlContent());
            }

            var tree = $('<div/>');

            tree.html(question.ItemBody());
            tree.find('.boxedText').replaceWith(function () {
                var $boxedText = $(this);
                var boxedTextHTML = $boxedText.html();
                var $newBoxedText = $('<div />');

                CopyAttributes($boxedText, $newBoxedText);

                $newBoxedText.html(boxedTextHTML);

                return $newBoxedText;
            });

            var mainBody =  tree.outerHTML();

            $('.border_0C5FA5').removeClass('border_0C5FA5');

            $(question.ItemBody()).find('inlinechoiceinteraction').each(function () {
                var inlineChoice = this.outerHTML;
                var parsedInlineChoice = that._renderInlineChoiceResult(self, inlineChoice, self.TestOnlineSessionAnswers(), question);
                mainBody = mainBody.replace(inlineChoice, parsedInlineChoice);
            });

            //process for multiple choice sub type(1)
            $(question.ItemBody()).find('choiceinteraction').each(function () {
                var choice = this.outerHTML;
                var parsedMultipleChoice = that._renderSimpleChoiceResult(choice, self.TestOnlineSessionAnswers(), question, self);

                mainBody = mainBody.replace(choice, parsedMultipleChoice);
            });

            //process for open ended type(10)
            $(question.ItemBody()).find('extendedTextInteraction').each(function () {
                var openEnded = this.outerHTML;
                var drawable = $(this).attr('drawable');
                var parsedOpenEnded = '';
                var openEndedHeight = parseInt($(this).get(0).style.height.replace('px', ''), 10);

                // Set default height extended text is 90 if before not set height
                if (isNaN(openEndedHeight)) {
                    openEndedHeight = 90;
                }

                if (drawable == 'true') {
                    parsedOpenEnded = that._renderOpenEndedResultDrawable(openEnded, self.TestOnlineSessionAnswers(), question, self);
                } else {
                    parsedOpenEnded = that._renderOpenEndedResult(openEnded, self.TestOnlineSessionAnswers(), question, self, openEndedHeight);
                }
                mainBody = mainBody.replace(openEnded, parsedOpenEnded);
            });

            mainBody = that._renderTextEntryResult(mainBody, self, question);

            if (options.PostProcessQuestionDetails != null && typeof (options.PostProcessQuestionDetails) == "function") {
                mainBody = options.PostProcessQuestionDetails(mainBody);
            }

            if (options.PostProcessQuestionDetails != null && typeof (options.PostProcessQuestionDetails) == "function") {
                mainBody = options.PostProcessQuestionDetails(mainBody);
            }

            self.SectionInstruction(question.SectionInstruction());
            self.Respones(mainBody);

            //show dropdown inline choice customize
            $('.inlineChoiceFormat').selectbox({ speed: 100, is_Disabled: true });
            $('.inlineChoiceFormat').each(function () {

                var selectedAnswer = '';
                var htmlString = '';
                var idResponseInlineChoice = $(this).attr('responseidentifier');
                if ($(this).find('li[selected]').length) {
                    selectedAnswer = $(this).find('li[selected]').attr('identifier');
                    htmlString = $(this).find('li[selected]').html();
                }

                if (htmlString != '') {
                    $('a[responseidentifier="' + idResponseInlineChoice + '"]').parent().css({
                        'width': 'auto',
                    });
                }

                $('a[responseidentifier="' + idResponseInlineChoice + '"]').attr('identifier', selectedAnswer).html(htmlString);

            });

            var tagsUl = $('#divQuestionDetails .mainBody:visible').find('.inlineChoiceFormat');
            $.each(tagsUl, function (idx, tagul) {
                var idRes = $(tagul).attr('responseidentifier');
                $(tagul).css({
                    'visibility': 'hidden',
                    'height': '0',
                    'position': 'absolute'
                }).show();

                var wtagUl = $(tagul).width() + 52;
                if ($('#divQuestionDetails .mainBody:visible').width() < wtagUl) {
                    wtagUl = $('#divQuestionDetails .mainBody:visible').width();
                }
                $('.sbSelector[responseidentifier="' + idRes + '"]').parent().css({ 'max-width': wtagUl, 'width': '100%' });
            });

            if (!options.ComplexItemUtil.IsNullOrEmpty(testOnlineSessionAnswer)) {
                self.PointsEarned(testOnlineSessionAnswer.PointsEarned());
                self.OldPointsEarned(testOnlineSessionAnswer.PointsEarned());
                self.AnswerImage(testOnlineSessionAnswer.AnswerImage());
            }

            self.LoadImages();
            MathJax.Hub.Queue(["Typeset", MathJax.Hub]);
        },

        _renderSimpleChoiceResult: function (questionItemBody, testOnlineSessionAnswerList, question, self) {
            var that = this;
            var options = that.options;

            //apply guidance and rationale
            var objTypeMessage = '';

            var answerOfStudentForSelectedQuestion = null;
            ko.utils.arrayForEach(testOnlineSessionAnswerList, function (testOnlineSessionAnswer) {
                if (question.VirtualQuestionID() === testOnlineSessionAnswer.VirtualQuestionID()) {
                    answerOfStudentForSelectedQuestion = testOnlineSessionAnswer;
                }
            });

            $('.jsIsAnswerCorrect').remove();
            var answerContainer = '';
            var choiceInteraction = $(questionItemBody);

            choiceInteraction.find('p').replaceWith(function () {
                var p = $(this);
                var div = $('<div></div>');
                div.html(p.html());
                CopyAttributes(p, div);
                return $(div.outerHTML());
            });

            var responseIdentifier = $(choiceInteraction).attr('responseIdentifier');
            var answerID = '';
            var answerChoice = '';
            var answerSubID = '';
            var pointsPossible = '';
            var pointsEarned;
            var updatedBy = '';
            var updatedDate = '';
            var overridden = false;
            var isMultipleChoice = false;
            if (answerOfStudentForSelectedQuestion != null) {
                ko.utils.arrayForEach(answerOfStudentForSelectedQuestion.TestOnlineSessionAnswerSubs(), function (testOnlineSessionAnswerSub) {
                    if (testOnlineSessionAnswerSub.ResponseIdentifier() == responseIdentifier) {
                        answerID = testOnlineSessionAnswerSub.QTIOnlineTestSessionAnswerID();
                        answerSubID = testOnlineSessionAnswerSub.QTIOnlineTestSessionAnswerSubID();
                        answerChoice = testOnlineSessionAnswerSub.AnswerChoice();
                        pointsPossible = testOnlineSessionAnswerSub.PointsPossible();
                        pointsEarned = testOnlineSessionAnswerSub.PointsEarned();
                        isMultipleChoice = testOnlineSessionAnswerSub.QTISchemaID() === 3;
                        updatedBy = testOnlineSessionAnswerSub.UpdatedBy();
                        updatedDate = testOnlineSessionAnswerSub.UpdatedDate();
                        overridden = testOnlineSessionAnswerSub.Overridden();
                    }
                });
            }

            $(choiceInteraction).find('simpleChoice').each(function () {
                var simpleChoice = $(this);
                var identifier = simpleChoice.attr('identifier');

                var isCorrectIdentifier = false;
                $(question.XmlContent()).find('responsedeclaration[identifier="' + responseIdentifier + '"] correctresponse value').each(function () {
                    if ($(this).text() === identifier) isCorrectIdentifier = true;
                });

                var isStudentChoose = false;
                if (!options.ComplexItemUtil.IsNullOrEmpty(answerChoice)) {
                    ko.utils.arrayForEach(answerChoice.split(','), function (item) {
                        if ($.trim(item) === identifier) isStudentChoose = true;
                    });
                }

                var checkBoxHtml = '';
                if (self.MultipleChoiceClickMethod() === '0') {
                    var checkbox = $('<input type="radio" disabled="disabled" style="margin-right:10px;" checked>');
                    if (isMultipleChoice) {
                        checkbox = $('<input type="checkbox" disabled="disabled" style="margin-right:10px;" checked>');
                    }

                    if (!isStudentChoose) checkbox.removeAttr('checked');
                    checkBoxHtml = checkbox.outerHTML();
                }

                simpleChoice.find('div.answer').each(function () {
                    var divIdentifier = $(this);
                    divIdentifier.attr('identifier', identifier).prepend('<div style="width:auto; margin-right:2px;">' + checkBoxHtml + identifier + '.</div>');
                    divIdentifier.append('<span style="float: right; padding-right: 40px;" class="iconGuidance"></span>');
                    divIdentifier.removeClass('green');
                    divIdentifier.removeClass('white');
                    divIdentifier.addClass('white');
                    divIdentifier.css('border', '');

                    if (isCorrectIdentifier) divIdentifier.addClass('green');
                    if (isStudentChoose) {
                        divIdentifier.css('border', '');
                        if (isCorrectIdentifier) {
                            divIdentifier.css('border', '1px solid green');
                            divIdentifier.append('<i class="jsIsAnswerCorrect correct"></i>');
                        } else {
                            divIdentifier.css('border', '1px solid red');
                            divIdentifier.append('<i class="jsIsAnswerCorrect incorrect"></i>');
                        }
                    }
                });

                //get data guidance if exist
                if (simpleChoice.find('div[typemessage]').length) {
                    var htmlGuidanceRationale = '';
                    var itemHtml = '';
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

                }

                answerContainer += simpleChoice.html();
            });

            var onclick = that._getOnClickFunction(1, answerID, answerSubID, pointsEarned, pointsPossible, updatedBy, updatedDate, overridden);

            answerContainer = '<div class="clearfix"></div><div responseidentifier="' + $(choiceInteraction).attr('responseidentifier') + '" class="box-answer" onclick="' + onclick + '" >' + answerContainer + '<div class="clearfix"></div></div>';

            return answerContainer;
        },

        _renderOpenEndedResult: function (questionItembody, testOnlineSessionAnswerList, question, self, openEndedHeight) {
            var that = this;
            var options = that.options;

            var answerText = '';
            var responseIdentifier = $(questionItembody).attr('responseidentifier');
            var answerID = '';
            var answerSubID = '';
            var pointsEarned = '';
            var pointsPossible = '';
            var isReviewed = false;
            var updatedBy = '';
            var updatedDate = '';
            var overridden = false;
            var responseProcessingTypeID = '';
            ko.utils.arrayForEach(testOnlineSessionAnswerList, function (testOnlineSessionAnswer) {
                if (question.VirtualQuestionID() === testOnlineSessionAnswer.VirtualQuestionID()) {
                    answerID = testOnlineSessionAnswer.QTIOnlineTestSessionAnswerID();
                    if (testOnlineSessionAnswer.TestOnlineSessionAnswerSubs().length > 0) {
                        ko.utils.arrayForEach(testOnlineSessionAnswer.TestOnlineSessionAnswerSubs(), function (testOnlineSessionAnswerSub) {
                            if (responseIdentifier == testOnlineSessionAnswerSub.ResponseIdentifier()) {
                                answerText = testOnlineSessionAnswerSub.AnswerText();
                                pointsEarned = testOnlineSessionAnswerSub.PointsEarned();
                                pointsPossible = testOnlineSessionAnswerSub.PointsPossible();
                                answerSubID = testOnlineSessionAnswerSub.QTIOnlineTestSessionAnswerSubID();
                                isReviewed = testOnlineSessionAnswerSub.IsReviewed();
                                updatedBy = testOnlineSessionAnswerSub.UpdatedBy();
                                updatedDate = testOnlineSessionAnswerSub.UpdatedDate();
                                overridden = testOnlineSessionAnswerSub.Overridden();
                                responseProcessingTypeID = testOnlineSessionAnswerSub.ResponseProcessingTypeID();
                            }
                        });
                    }
                }
            });

            answerText = $("<div/>").text(answerText).text().replace(/\r?\n/g, '<br />');
            var onclick = that._getOnClickFunction(10, answerID, answerSubID, pointsEarned, pointsPossible, updatedBy, updatedDate, overridden);
            if (responseProcessingTypeID == 3) {
                onclick = that._getOnClickFunction(10, answerID, answerSubID, 0, 0, null, null, false);
                var answerContainer = '<div class="clearfix"></div><div class="box-answer"><div class="textarea openEndedText" onclick="' + onclick + '" style="width:100%; height:200px;">' + answerText + '</div>' +
                '<div class="clearfix"></div></div>';
                return answerContainer;
            }

            var reviewableIcon = '';
            if (!options.ComplexItemUtil.IsNullOrEmpty(self.QTIOnlineTestSessionID())) {
                if (isReviewed) {
                    reviewableIcon = '<img src="/Content/themes/AssignmentRegrader/images/icon-reviewed.png" alt="" title="Reviewable">';
                } else {
                    reviewableIcon = '<img src="/Content/themes/AssignmentRegrader/images/icon-review.png" alt="" title="Reviewable">';
                }

            }

            var answerContainer = '<div class="clearfix"></div><div class="box-answer">' + reviewableIcon + '<div class="textarea openEndedText" onclick="' + onclick + '" style="width:100%; height:200px;">' + answerText + '</div>' +
                '<div class="clearfix"></div></div>';
            if (!isReviewed) {
                answerContainer = '<div class="clearfix"></div><div class="box-answer">' + reviewableIcon + '<div class="textarea openEndedText red-border" onclick="' + onclick + '" style="width:100%; height:200px;">' + answerText + '</div>' +
                '<div class="clearfix"></div></div>';
            }

            return answerContainer;
        },

        _renderOpenEndedResultDrawable: function (questionItembody, testOnlineSessionAnswerList, question, self) {
            var that = this;
            var options = that.options;

            var answerText = '';
            var responseIdentifier = $(questionItembody).attr('responseidentifier');
            var answerID = '';
            var answerSubID = '';
            var pointsEarned = '';
            var pointsPossible = '';
            var isReviewed = false;
            var updatedBy = '';
            var updatedDate = '';
            var overridden = false;
            var responseProcessingTypeID = '';
            ko.utils.arrayForEach(testOnlineSessionAnswerList, function (testOnlineSessionAnswer) {
                if (question.VirtualQuestionID() === testOnlineSessionAnswer.VirtualQuestionID()) {
                    answerID = testOnlineSessionAnswer.QTIOnlineTestSessionAnswerID();
                    if (testOnlineSessionAnswer.TestOnlineSessionAnswerSubs().length > 0) {
                        ko.utils.arrayForEach(testOnlineSessionAnswer.TestOnlineSessionAnswerSubs(), function (testOnlineSessionAnswerSub) {
                            if (responseIdentifier == testOnlineSessionAnswerSub.ResponseIdentifier()) {
                                answerText = testOnlineSessionAnswerSub.AnswerText();
                                pointsEarned = testOnlineSessionAnswerSub.PointsEarned();
                                pointsPossible = testOnlineSessionAnswerSub.PointsPossible();
                                answerSubID = testOnlineSessionAnswerSub.QTIOnlineTestSessionAnswerSubID();
                                isReviewed = testOnlineSessionAnswerSub.IsReviewed();
                                updatedBy = testOnlineSessionAnswerSub.UpdatedBy();
                                updatedDate = testOnlineSessionAnswerSub.UpdatedDate();
                                overridden = testOnlineSessionAnswerSub.Overridden();
                                responseProcessingTypeID = testOnlineSessionAnswerSub.ResponseProcessingTypeID();
                            }
                        });
                    }
                }
            });

            var onclick = that._getOnClickFunction(10, answerID, answerSubID, pointsEarned, pointsPossible, updatedBy, updatedDate, overridden);
            if (responseProcessingTypeID == 3)
                onclick = that._getOnClickFunction(10, answerID, answerSubID, 0, 0, null, null, false);

            var reviewableIcon = '';
            if (!options.ComplexItemUtil.IsNullOrEmpty(self.QTIOnlineTestSessionID())) {
                if (isReviewed) {
                    reviewableIcon = '<img src="/Content/themes/AssignmentRegrader/images/icon-reviewed.png" alt="" title="Reviewable" isReviewed="true">';
                } else {
                    reviewableIcon = '<img src="/Content/themes/AssignmentRegrader/images/icon-review.png" alt="" title="Reviewable" isReviewed="false">';
                }

            }

            var clearFixDiv = $('<div class="clearfix"></div>');
            var answerDiv = $('<div></div>').attr('onclick', onclick).css('margin-bottom', '15px').append($(reviewableIcon)).append($(questionItembody));
            if (responseProcessingTypeID == 3)
                answerDiv = $('<div></div>').attr('onclick', onclick).css('margin-bottom', '15px').append($(questionItembody));

            answerDiv.addClass('box-answer');
            answerDiv.before(clearFixDiv);
            answerDiv.after(clearFixDiv);

            return answerDiv.outerHTML();
        },

        _renderInlineChoiceResult: function (self, questionItemBody, testOnlineSessionAnswerList, question) {

            var that = this;
            var options = that.options;

            var option = questionItemBody;
            var index = option.indexOf('>');
            var output = [option.slice(0, index + 1), '<inlinechoice identifier=""></inlinechoice>', option.slice(index + 1)].join('');
            questionItemBody = questionItemBody.replace(option, output);

            var itemBody = questionItemBody;

            var select = questionItemBody.replaceAll('inlinechoiceinteraction', 'ul class="inlineChoiceFormat"');
            //apply guidance and rationale
            var listMessageGuidance = [];
            var arrMessageGuidance = [];
            var listObjTypeMessages = [];
            var objTypeMessage = '';
            var responseidentifier = '';

            itemBody = itemBody.replace(questionItemBody, select);
            responseidentifier = $(questionItemBody).attr('responseidentifier');
            $(questionItemBody).find('inlinechoice').each(function () {
                var option1 = this.outerHTML.replaceAll('inlinechoice', 'li');
                if ($(this).find('div[typemessage]').length) {
                    //var arrGuidances = $(this).find('div[typemessage]').not('div[typemessage="guidance"]');
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

                //var option1 = this.outerHTML.replaceAll('inlinechoice', 'option disabled="disabled"');
                for (var j = 0, lenArrMessageGuidance = arrMessageGuidance.length; j < lenArrMessageGuidance; j++) {
                    option1 = option1.replace(arrMessageGuidance[j].outerHTML, '');
                }

                valItemOption = $(option1).find('.inlineChoiceAnswer').html();
                for (var k = 0, lenlistMessageGuidance = listMessageGuidance.length; k < lenlistMessageGuidance; k++) {
                    if (listMessageGuidance[k].identifier === $(this).attr('identifier')) {
                        listMessageGuidance[k].value = valItemOption;
                    }
                }

                itemBody = itemBody.replace(this.outerHTML, option1);
                arrMessageGuidance = [];
            });

            //create array objects guidance, rationale
            for (var icount = 0, lenlistMessageGuidance = listMessageGuidance.length;icount < lenlistMessageGuidance; icount++) {
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
                var htmlGuidance = self.buildGuidanceInlineChoice(listObjTypeMessages, responseidentifier);
            }

            var selectedAnswer = '';
            var answerID = '';
            var answerSubID = '';
            var pointsEarned = '';
            var pointsPossible = '';
            var updatedBy = '';
            var updatedDate = '';
            var overridden = false;

            if (question.CorrectResponse.length == 1 && typeof question.CorrectResponse === 'string') {

                ko.utils.arrayForEach(testOnlineSessionAnswerList, function (testOnlineSessionAnswer) {
                    if (question.VirtualQuestionID() === testOnlineSessionAnswer.VirtualQuestionID()) {
                        selectedAnswer = testOnlineSessionAnswer.AnswerChoice();
                        pointsEarned = testOnlineSessionAnswer.PointsEarned();
                        answerID = testOnlineSessionAnswer.QTIOnlineTestSessionAnswerID();

                        if (htmlGuidance !== undefined && htmlGuidance !== '') {
                            var iconString = '<img responseidentifier="' + responseidentifier + '" alt="Guidance" style="margin-left: 5px;" class="imageupload bntGuidance" src="../../Content/themes/TestMaker/images/guidance_checked_medium.png" title="">';
                            itemBody += iconString;
                            itemBody += '<div name="typeGuidance" style="display: none; border: 1px solid #999;padding: 5px;" idReponse="' + responseidentifier + '">' + htmlGuidance + '</div>';
                        }

                        if (testOnlineSessionAnswer.AnswerChoice() === question.CorrectResponse) {
                            itemBody += '<i class="jsIsAnswerCorrect ' + 'correct' + '" style="float:none;display:inline-block;position: relative;margin-left:15px;"></i>';
                        } else {
                            itemBody += ' <i class="jsIsAnswerCorrect ' + 'incorrect' + '" style="float:none;display:inline-block;position: relative;margin-left:10px;"></i>';
                        }
                        itemBody += '<p style="color:red">' + 'Correct Answer: ' + that._getCorrectValueInlineChoice(question.CorrectResponse, itemBody) + '</p>';
                    }
                });
            } else {
                var responseIdentifier = $(questionItemBody).attr('responseidentifier');

                ko.utils.arrayForEach(testOnlineSessionAnswerList, function (testOnlineSessionAnswer) {
                    if (question.VirtualQuestionID() === testOnlineSessionAnswer.VirtualQuestionID()) {
                        pointsEarned = testOnlineSessionAnswer.PointsEarned();
                        answerID = testOnlineSessionAnswer.QTIOnlineTestSessionAnswerID();
                        pointsPossible = question.PointsPossible();

                        if (testOnlineSessionAnswer.TestOnlineSessionAnswerSubs().length > 0) {
                            ko.utils.arrayForEach(testOnlineSessionAnswer.TestOnlineSessionAnswerSubs(), function (testOnlineSessionAnswerSub) {
                                if (testOnlineSessionAnswerSub.ResponseIdentifier() == responseIdentifier) {
                                    for (var j = 0; j < question.CorrectResponse.length; j++) {
                                        if (question.CorrectResponse[j].key == responseIdentifier) {
                                            var correctResponses = question.CorrectResponse[j].value;
                                            selectedAnswer = testOnlineSessionAnswerSub.AnswerChoice();

                                            answerSubID = testOnlineSessionAnswerSub.QTIOnlineTestSessionAnswerSubID();
                                            pointsEarned = testOnlineSessionAnswerSub.PointsEarned();
                                            pointsPossible = testOnlineSessionAnswerSub.PointsPossible();
                                            updatedBy = testOnlineSessionAnswerSub.UpdatedBy();
                                            updatedDate = testOnlineSessionAnswerSub.UpdatedDate();
                                            overridden = testOnlineSessionAnswerSub.Overridden();
                                            var isCheck = false;
                                            var correctResponseText = '';


                                            for (var i = 0; i < correctResponses.length; i++) {
                                                var correctResponse = correctResponses[i];
                                                if (i != correctResponses.length - 1) {
                                                    correctResponseText += that._getCorrectValueInlineChoice(correctResponse, itemBody) + ', ';
                                                } else {
                                                    correctResponseText += that._getCorrectValueInlineChoice(correctResponse, itemBody);
                                                }

                                                if (correctResponse == selectedAnswer) {
                                                    if (question.Answered()) {
                                                        itemBody += '<i class="jsIsAnswerCorrect ' + 'correct' + '" style="float:none;display:inline-block;position: relative;;margin-left:15px;"></i>';
                                                    }

                                                    itemBody += '<p class="correct-answer-teacher-review" style="color:red;">' + 'Correct Answer: ' + correctResponseText + '</p>';
                                                    isCheck = true;
                                                }
                                            }
                                            if (isCheck == false) {
                                                if (correctResponseText.length > 0) {
                                                    if (correctResponseText.endsWith(', ')) {
                                                        correctResponseText = correctResponseText.substring(0, correctResponseText.length - 2);
                                                    }
                                                }
                                                if (question.Answered() || self.IsPendingReview()) {
                                                    itemBody += ' <i class="jsIsAnswerCorrect ' + 'incorrect' + '" style="float:none;display:inline-block;position: relative;margin-left:10px;"></i>';
                                                }
                                                itemBody += '<p class="correct-answer-teacher-review" style="color:red;">' + 'Correct Answer: ' + correctResponseText + '</p>';
                                            }
                                            if (htmlGuidance !== undefined && htmlGuidance !== '') {
                                                var iconString = '<img responseidentifier="' + responseidentifier + '" alt="Guidance" style="margin-left: 5px;" class="imageupload bntGuidance" src="../../Content/themes/TestMaker/images/guidance_checked_medium.png" title="">';
                                                itemBody += iconString;
                                                itemBody += '<div name="typeGuidance" style="display: none; border: 1px solid #999;padding: 5px;" idReponse="' + responseidentifier + '">' + htmlGuidance + '</div>';
                                            }
                                        }
                                    }

                                }
                            });
                        }
                    }
                });

                if (testOnlineSessionAnswerList.length === 0) {
                    if (htmlGuidance !== undefined && htmlGuidance !== '') {
                        var iconString = '<img responseidentifier="' + responseidentifier + '" alt="Guidance" style="margin-left: 5px;" class="imageupload bntGuidance" src="../../Content/themes/TestMaker/images/guidance_checked_medium.png" title="">';
                        itemBody += iconString;
                        itemBody += '<div name="typeGuidance" style="display: none; border: 1px solid #999;padding: 5px;" idReponse="' + responseidentifier + '">' + htmlGuidance + '</div>';
                    }
                }
            }

            var stringselectedAnswer = '';
            var strOptionSelectedAnswer = '';
            $(itemBody).find("li").each(function () {
                var pro = 'disabled';

                if (selectedAnswer != '') {
                    if ($(this).attr('identifier').toLowerCase() == selectedAnswer.toLowerCase()) {
                        pro = ' selected';
                        stringselectedAnswer = 'identifier="' + selectedAnswer + '"';
                        strOptionSelectedAnswer = 'identifier="' + selectedAnswer + '" selected';
                    }
                }

                var option = this.outerHTML;
                var index = option.indexOf('>');
                var output = [option.slice(0, index), pro, option.slice(index)].join('');
                itemBody = itemBody.replace(option, output);
            });

            var onclick = that._getOnClickFunction(8, answerID, answerSubID, pointsEarned, pointsPossible, updatedBy, updatedDate, overridden);
            if (selectedAnswer != '') {
                itemBody = itemBody.replace(stringselectedAnswer, strOptionSelectedAnswer);
                stringselectedAnswer = '';
                strOptionSelectedAnswer = '';
            }

            var buildSelect = '<select style="font-size:small;max-width:87%;" onclick="' + onclick + '" ';
            itemBody = itemBody.replace('<select', buildSelect);

            return itemBody;
        },

        _getTestOnlineSessionAnswerSub: function (self, virtualQuestionID, responseIdentifier) {
            var result = null;
            ko.utils.arrayForEach(self.TestOnlineSessionAnswers(), function (item) {
                if (virtualQuestionID === item.VirtualQuestionID()) {
                    if (item.TestOnlineSessionAnswerSubs().length > 0) {
                        ko.utils.arrayForEach(item.TestOnlineSessionAnswerSubs(), function (testOnlineSessionAnswerSub) {
                            if (testOnlineSessionAnswerSub.ResponseIdentifier() == responseIdentifier) result = testOnlineSessionAnswerSub;
                        });
                    }
                }
            });

            return result;
        },

        _renderTextEntryResult: function (currentItemBody, self, question) {
            var greaterThanOrEqual = '&#8805;';
            var lessThanOrEqual = '&#8804;';

            var that = this;

            var answerID = '';
            var answerSubID = '';
            var pointsEarned = '';
            var pointsPossible = '';
            var responseProcessingTypeID = '';
            var isReviewed = '';
            var selectedAnswer = '';
            var updatedBy = '';
            var updatedDate = '';
            var overridden = false;

            var tree = $('<div></div>');
            tree.html(currentItemBody);

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
                    //var tagsValue = $(itemRubric).find('value').not('value[type="guidance"]');
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
                                            if (newObjTypeMessage.typeMessage === 'rationale' || newObjTypeMessage.typeMessage === 'guidance_rationale') {
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
                var testOnlineSessionAnswerSub = that._getTestOnlineSessionAnswerSub(self, question.VirtualQuestionID(), responseIdentifier);
                if (testOnlineSessionAnswerSub != null) {
                    selectedAnswer = testOnlineSessionAnswerSub.AnswerText();
                    answerID = testOnlineSessionAnswerSub.QTIOnlineTestSessionAnswerID();
                    answerSubID = testOnlineSessionAnswerSub.QTIOnlineTestSessionAnswerSubID();
                    pointsEarned = testOnlineSessionAnswerSub.PointsEarned();
                    pointsPossible = testOnlineSessionAnswerSub.PointsPossible();
                    responseProcessingTypeID = testOnlineSessionAnswerSub.ResponseProcessingTypeID();
                    isReviewed = testOnlineSessionAnswerSub.IsReviewed();
                    updatedBy = testOnlineSessionAnswerSub.UpdatedBy();
                    updatedDate = testOnlineSessionAnswerSub.UpdatedDate();
                    overridden = testOnlineSessionAnswerSub.Overridden();
                }

                var newTextEntry = $('<span></span>');

                CopyAttributes(textEntry, newTextEntry);
                newTextEntry.addClass('input');
                newTextEntry.html(selectedAnswer);
                newTextEntry.css('min-width', '10px');
				newTextEntry.css('max-width', '100%');

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
                    value.find('rangeValue').each(function () {
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

                    correctAnswer.append('<br><br>' + correctAnswerText);
                });

                if (htmlListRationale != '') {
                    correctAnswer.append(htmlListRationale);
                }

                var reviewableIcon = '';
                if (responseProcessingTypeID == '2') {
                    if (!that.options.ComplexItemUtil.IsNullOrEmpty(self.QTIOnlineTestSessionID())) {
                        if (isReviewed) {
                            reviewableIcon = '<img src="/Content/themes/AssignmentRegrader/images/icon-reviewed.png" alt="" title="Reviewable">';
                            newTextEntry.removeClass('red-border');
                        } else {
                            reviewableIcon = '<img src="/Content/themes/AssignmentRegrader/images/icon-review.png" alt="" title="Reviewable">';
                            newTextEntry.addClass('red-border');
                        }
                    }
                }

                var onclick = that._getOnClickFunction(9, answerID, answerSubID, pointsEarned, pointsPossible, updatedBy, updatedDate, overridden);

                if (responseProcessingTypeID == 3) {
                    onclick = that._getOnClickFunction(9, null, answerSubID, 0, 0, null, null, false);
                    newTextEntry.attr('onclick', onclick);
                    return $(newTextEntry.outerHTML());
                }

                newTextEntry.attr('onclick', onclick);

                var textEntryMark = $('');
                if (!that.options.ComplexItemUtil.IsNullOrEmpty(self.QTIOnlineTestSessionID()) && (self.IsComplete() || self.IsPendingReview()) && responseProcessingTypeID != '2') {
                    textEntryMark = $('<i class="jsIsAnswerCorrect" style="float:none;display:inline-block;position: relative;margin-left:15px;"></i>');
                    if (pointsEarned != '' && pointsPossible != '' && pointsEarned == pointsPossible) {
                        textEntryMark.addClass('correct');
                    } else if (pointsEarned > 0) {
                        textEntryMark.addClass('partial');
                    } else {
                        textEntryMark.addClass('incorrect');
                    }
                }

                return $(reviewableIcon + newTextEntry.outerHTML() + textEntryMark.outerHTML() + '<br>' + '<br>' + correctAnswer.outerHTML() + '<br>');
            });

            return tree.outerHTML();
        },

        _getCorrectValueInlineChoice: function (correctIdentifier, option) {
            var correctValue = '';
            $(option).find("li").filter(function () {
                //may want to use $.trim in here
                return $(this).attr('identifier').toLowerCase() == correctIdentifier.toLowerCase();
            }).each(function () {
                correctValue = $(this).html();
            });
            return correctValue;
        },

        _getHtmlValue: function (id) {
            return $.trim($('#' + id).html());
        },

        _setHtmlValue: function (id, val) {
            $('#' + id).html(val);
        },

        _getNullValueIfEmpty: function (val) {
          var that = this;
          if (that.options.ComplexItemUtil.IsNullOrEmpty(val)) return null;
          val = '' + val;
          val = val.replaceAll("'", "\'");
          return "'" + val + "'";
        },

        _getOnClickFunction: function (qtiSchemaID, answerID, answerSubID, pointsEarned, pointsPossible, updatedBy, updatedDate, overridden) {
            var that = this;
            var onclick = 'NewItemClick(this,';
            onclick += that._getNullValueIfEmpty(answerID);
            onclick += ',' + that._getNullValueIfEmpty(answerSubID);
            onclick += ',' + that._getNullValueIfEmpty(pointsEarned);
            onclick += ',' + that._getNullValueIfEmpty(pointsPossible);
            onclick += ',' + that._getNullValueIfEmpty(qtiSchemaID);
            onclick += ',' + that._getNullValueIfEmpty(updatedBy);
            onclick += ',' + that._getNullValueIfEmpty(updatedDate);
            onclick += ',' + that._getNullValueIfEmpty(overridden);
            onclick += ');';

            return onclick;
        }

    });
}(jQuery));
