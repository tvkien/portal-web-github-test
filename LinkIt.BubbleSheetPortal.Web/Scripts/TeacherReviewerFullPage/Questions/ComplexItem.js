(function ($) {
    $.widget('jquery.ComplexItem', {
        options: {
            ComplexItemUtil: null,
            PostProcessQuestionDetails: null,
            Self: null
        },
        escapeRegExp: function (string, strReplace , newString){
          var indexString = string.indexOf(strReplace);
          var replaceSpace = string.replace(strReplace, '');
          return string.substring(0 ,indexString) + newString + replaceSpace.substring(indexString);
        },
      decodeHTMl: function(string) {
          var regex = new RegExp(/&amp;/g);
          while(regex.exec(string) !== null) {
            string = string.replace(/&amp;/g, '&');
          }
          return string;
        },
        Display: function (question) {
            var that = this;
            var options = that.options;
            var self = options.Self;

            self.SelectedQuestion(question);
            self.RefObjects(question.RefObjects());
            self.PointsPossible(question.PointsPossible());
            self.QTIItemSchemaID(question.QTIItemSchemaID());
            self.AnswerSubID('');
            var testOnlineSessionAnswer = question.Answer();

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

            if (!options.ComplexItemUtil.IsNullOrEmpty(question.Answer())) {
                var data = question.Answer().PostAnswerLogs();
                self.PostAnswerLogs(data);
            }

            $(question.ItemBody()).find('inlinechoiceinteraction').each(function () {
                var inlineChoice = this.outerHTML;
                var inlineChoiceWrap = $(this);
                var getStyle = "";

                //Replace if the item has an Item Type Interactions onto an Image
                if (inlineChoiceWrap.parents(".itemtypeonimage").length > 0) {
                    getStyle = inlineChoiceWrap.attr("style");
                    inlineChoiceWrap.removeAttr("style");
                }

                var parsedInlineChoice = that._renderInlineChoiceResult(self, inlineChoice, self.TestOnlineSessionAnswers(), question);

                if (getStyle !== "" && getStyle != undefined) {
                    parsedInlineChoice = '<div class="wrapTempInlineChoice" style="' + getStyle + '">' + parsedInlineChoice + '<span class="tag_number" idresponse="' + inlineChoiceWrap.attr('responseidentifier') +  '"></span></div>';
                }
                parsedInlineChoice = that.decodeHTMl(parsedInlineChoice);
                mainBody = that.escapeRegExp(mainBody, inlineChoice, parsedInlineChoice);
                // mainBody = mainBody.replace(inlineChoice, parsedInlineChoice);
            });

            //process for multiple choice sub type(1)
            $(question.ItemBody()).find('choiceinteraction').each(function () {
                var choice = this.outerHTML;
                var parsedMultipleChoice = that._renderSimpleChoiceResult(choice, self.TestOnlineSessionAnswers(), question, self);
                parsedMultipleChoice = that.decodeHTMl(parsedMultipleChoice);
                mainBody = that.escapeRegExp(mainBody, choice, parsedMultipleChoice);
                // mainBody = mainBody.replace(choice, parsedMultipleChoice);
            });

            //process for open ended type(10)
            $(question.ItemBody()).find('extendedTextInteraction').each(function () {
                var openEnded = this.outerHTML;
                var drawable = $(this).attr('drawable');
                var uploadfile = $(this).attr('uploadfile');
                var parsedOpenEnded = '';
                var openEndedHeight = parseInt($(this).get(0).style.height.replace('px', ''), 10);

                // Set default height extended text is 90 if before not set height
                if (isNaN(openEndedHeight)) {
                    openEndedHeight = 90;
                }

                if (drawable == 'true') {
                    parsedOpenEnded = that._renderOpenEndedResultDrawable(openEnded, self.TestOnlineSessionAnswers(), question, self);
                } else if (uploadfile == 'true') {
                    parsedOpenEnded = that._renderOpenEndedResultUploadFile(openEnded, self.TestOnlineSessionAnswers(), question, self);
                } else {
                    parsedOpenEnded = that._renderOpenEndedResult(openEnded, self.TestOnlineSessionAnswers(), question, self, openEndedHeight);
                }
                parsedOpenEnded = that.decodeHTMl(parsedOpenEnded);
                mainBody = that.escapeRegExp(mainBody, openEnded, parsedOpenEnded);
                // mainBody = mainBody.replace(openEnded, parsedOpenEnded);
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
            $('.inlineChoiceFormat').selectbox({
                speed: 100,
                is_Disabled: true,
                onOpen: function(inst) {
                    var $self = $(this);
                    var $parent = $self.parents('td');
                    var $doc = $(document);

                    if ($parent.length > 0) {
                        // Visible inline choice in table
                        $parent.css('overflow', 'visible');
                    }

                    if ($self.parent('.wrapTempInlineChoice').length) {
                        $self.parent('.wrapTempInlineChoice').css('z-index', '2');
                    }

                    // Reduce z-index other inline choice
                    $doc.find('.sbHolder').not($self.next('.sbHolder')).css('z-index', 'auto');
                },
                onClose: function(inst) {
                    var $self = $(this);
                    var $parent = $self.parents('td');
                    var $doc = $(document);

                    if ($parent.length > 0) {
                        // Reset after closed inline choice
                        $parent.css('overflow', 'hidden');
                    }

                    if ($self.parent('.wrapTempInlineChoice').length) {
                        $self.parent('.wrapTempInlineChoice').css('z-index', 'auto');
                    }

                    // Reset z-index all inline choice
                    $doc.find('.sbHolder').css('z-index', 'auto');
                }
            });

            $('.inlineChoiceFormat').each(function () {

                var selectedAnswer = '';
                var htmlString = '';
              var idResponseInlineChoice = $(this).attr('responseidentifier');
              var onclick =  $(this).attr('onclick')
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
              $('a[responseidentifier="' + idResponseInlineChoice + '"]').attr('onclick', onclick).html(htmlString);

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

            //Process for correct answer in the image item type
            $(".itemtypeonimage .correct-answer-teacher-review").each(function (index) {
                var me = $(this);
                var resId = me.attr("idresponse");
                var isInlinechoice = me.parent('.wrapTempInlineChoice').length;
                var htmlAnswer;

                // Check item is inline choice
                if (isInlinechoice) {
                    var $wrapInlinechoice = $('<div/>');
                    $wrapInlinechoice.html(me.parent('.wrapTempInlineChoice').clone(true).html());
                    $wrapInlinechoice.find('.inlineChoiceFormat, .sbHolder, .jsIsAnswerCorrect').remove();
                    htmlAnswer = $wrapInlinechoice.html();
                } else {
                    htmlAnswer = me.prop('outerHTML');
                }

                var num = index + 1;
                var divNum = "<div><span class='tag_number_item'>" + num + "</span></div>"
                var parent = me.parents(".itemtypeonimage");
                parent.find(".tag_number[idresponse=" + resId + "]").text(num);
                parent.append(divNum + htmlAnswer);

                // Remove guidance on item image inline choice
                if (isInlinechoice) {
                    me.parent('.wrapTempInlineChoice').find('.bntGuidance, [name="typeGuidance"]').remove();
                }

                me.remove();
            });

            // Remove tag number is empty
            $('.tag_number:empty').remove();

            if (!options.ComplexItemUtil.IsNullOrEmpty(testOnlineSessionAnswer)) {
                self.PointsEarned(testOnlineSessionAnswer.PointsEarned());
                self.OldPointsEarned(testOnlineSessionAnswer.PointsEarned());
                self.AnswerImage(testOnlineSessionAnswer.AnswerImage());
            }
        },

        _renderSimpleChoiceResult: function (questionItemBody, testOnlineSessionAnswerList, question, self) {
            var that = this;
            var options = that.options;

            //apply guidance and rationale
            var objTypeMessage = '';

            var answerOfStudentForSelectedQuestion = null;

            if (testOnlineSessionAnswerList !== null) {
                ko.utils.arrayForEach(testOnlineSessionAnswerList, function (testOnlineSessionAnswer) {
                    if (question.VirtualQuestionID() === testOnlineSessionAnswer.VirtualQuestionID()) {
                        answerOfStudentForSelectedQuestion = testOnlineSessionAnswer;
                    }
                });
            }

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
            if (testOnlineSessionAnswerList !== null) {
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
            }

            var onclick = that._getOnClickFunction(10, answerID, answerSubID, pointsEarned, pointsPossible, updatedBy, updatedDate, overridden);

          answerText = $("<div/>").text(answerText).text();
            if (!options.ComplexItemUtil.IsNullOrEmpty(answerText)) {
                if (answerText.indexOf('&#60;') > -1 && answerText.indexOf('&#62;') > -1) {
                    answerText = options.ComplexItemUtil.replaceStringLessOrLarge(answerText);
                } else {
                    answerText = answerText.replace(/[\r\n]+/g, '<br/>');
                }
            }

            var answerContainer = '';
            if (responseProcessingTypeID === 3) {
                onclick = that._getOnClickFunction(10, answerID, answerSubID, 0, 0, null, null, false);
                answerContainer = '<div class="box-answer"><div class="textarea openEndedText" data-response-id="' + responseIdentifier + '" onclick="' + onclick + '" style="width:100%; height:200px;padding: 5px;">' + answerText + '</div></div>';
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

            answerContainer += '<div class="box-answer is-manually-graded">';
            answerContainer += reviewableIcon;

            if (!isReviewed) {
                answerContainer += '<div class="textarea openEndedText red-border" data-response-id="' + responseIdentifier + '" onclick="' + onclick + '" style="width:100%;padding: 5px;height: ' + openEndedHeight + 'px;">' + answerText + '</div>';
            } else {
                answerContainer += '<div class="textarea openEndedText" data-response-id="' + responseIdentifier + '" onclick="' + onclick + '" style="width:100%;padding: 5px;height: ' + openEndedHeight + 'px;">' + answerText + '</div>';
            }

            answerContainer += '</div>';
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
            if (testOnlineSessionAnswerList !== null) {
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
            }

            var onclick = that._getOnClickFunction(10, answerID, answerSubID, pointsEarned, pointsPossible, updatedBy, updatedDate, overridden);
            if (responseProcessingTypeID === 3)
                onclick = that._getOnClickFunction(10, answerID, answerSubID, 0, 0, null, null, false);

            var reviewableIcon = '';
            if (!options.ComplexItemUtil.IsNullOrEmpty(self.QTIOnlineTestSessionID())) {
                if (isReviewed) {
                    reviewableIcon = '<img src="/Content/themes/AssignmentRegrader/images/icon-reviewed.png" alt="" title="Reviewable" isReviewed="true">';
                } else {
                    reviewableIcon = '<img src="/Content/themes/AssignmentRegrader/images/icon-review.png" alt="" title="Reviewable" isReviewed="false">';
                }

            }

            var clearFixDiv = $('<div class="clearfix"/>');
            var answerDiv = $('<div/>')
                .attr('onclick', onclick)
                .css('margin-bottom', '15px')
                .append($(reviewableIcon))
                .append($(questionItembody));

            if (responseProcessingTypeID === 3) {
                answerDiv = $('<div/>').attr('onclick', onclick).css('margin-bottom', '15px').append($(questionItembody));
            }

            answerDiv.addClass('box-answer is-manually-graded');
            answerDiv.before(clearFixDiv);
            answerDiv.after(clearFixDiv);

            return answerDiv.outerHTML();
        },

        _renderOpenEndedResultUploadFile: function (questionItembody, testOnlineSessionAnswerList, question, self) {
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
                            }
                        });
                    }
                }
            });

            var onclick = that._getOnClickFunction(10, answerID, answerSubID, pointsEarned, pointsPossible, updatedBy, updatedDate, overridden);

            var reviewableIcon = '';
            if (!options.ComplexItemUtil.IsNullOrEmpty(self.QTIOnlineTestSessionID())) {
                if (isReviewed) {
                    reviewableIcon = '<img src="/Content/themes/AssignmentRegrader/images/icon-reviewed.png" alt="" title="Reviewable">';
                } else {
                    reviewableIcon = '<img src="/Content/themes/AssignmentRegrader/images/icon-review.png" alt="" title="Reviewable">';
                }

            }

            var htmlUpload = '<div class="upload-file-title-answer">Answer Area</div>';
            htmlUpload += '<div class="upload-file-area">';
            htmlUpload += '    <div class="upload-file-title">Uploaded files:</div>';
            htmlUpload += '    <div class="upload-file-list">';
            htmlUpload += '        <a href="/Content/themes/AssignmentRegrader/images/icon-question.jpg" class="upload-file-item" target="_blank">File name show here</a>';
            htmlUpload += '        <a href="/Content/themes/AssignmentRegrader/images/icon-question.jpg" class="upload-file-item" target="_blank">File name show here</a>';
            htmlUpload += '        <a href="/Content/themes/AssignmentRegrader/images/icon-question.jpg" class="upload-file-item" target="_blank">File name show here</a>';
            htmlUpload += '    </div>';
            htmlUpload += '</div>';

            var answerContainer = '<div class="clearfix"></div><div class="box-answer">' + reviewableIcon + '<div onclick="' + onclick + '">' + htmlUpload + '</div>' + '<div class="clearfix"></div></div>';
            if (!isReviewed) {
                answerContainer = '<div class="clearfix"></div><div class="box-answer">' + reviewableIcon + '<div onclick="' + onclick + '">' + htmlUpload + '</div>' + '<div class="clearfix"></div></div>';
            }
            return answerContainer;
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

                valItemOption = $(option1).html();
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
                    var $objItemType = $(objItemType);
                    var audio = $objItemType.attr('audioref');
                    var typemessage = $objItemType.attr('typemessage');
                    var stringHtml = $objItemType.html();
                    var isRatioContent = false;

                    isRatioContent = Reviewer.GetGuidanceRationaleContent($objItemType);

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
                        itemBody += '<p style="color:red; display: inline-block;">' + 'Correct Answer: ' + that._getCorrectValueInlineChoice(question.CorrectResponse, itemBody) + '</p>';
                    }
                });
            } else {
                var responseIdentifier = $(questionItemBody).attr('responseidentifier');

                if (testOnlineSessionAnswerList !== null) {
                    ko.utils.arrayForEach(testOnlineSessionAnswerList, function(testOnlineSessionAnswer) {
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
                                                resId = testOnlineSessionAnswerSub.ResponseIdentifier();
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
                                                            itemBody += '<i class="jsIsAnswerCorrect ' + 'correct' + '" style="float:none;display:inline-block;position: relative; margin-left:15px;"></i>';
                                                        }

                                                        itemBody += '<p class="correct-answer-teacher-review" style="color:red; display: inline-block;" idresponse="' + resId + '">' + 'Correct Answer: ' + correctResponseText + '</p>';
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
                                                    itemBody += '<p class="correct-answer-teacher-review" style="color:red; display: inline-block;" idresponse="' + resId + '">' + 'Correct Answer: ' + correctResponseText + '</p>';
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
                }

                if (testOnlineSessionAnswerList !== null) {
                    if (testOnlineSessionAnswerList.length === 0) {
                        if (htmlGuidance !== undefined && htmlGuidance !== '') {
                            var iconString = '<img responseidentifier="' + responseidentifier + '" alt="Guidance" style="margin-left: 5px;" class="imageupload bntGuidance" src="../../Content/themes/TestMaker/images/guidance_checked_medium.png" title="">';
                            itemBody += iconString;
                            itemBody += '<div name="typeGuidance" style="display: none; border: 1px solid #999;padding: 5px;" idReponse="' + responseidentifier + '">' + htmlGuidance + '</div>';
                        }
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

          var buildSelect = '<ul style="font-size:small;max-width:87%;" onclick="' + onclick + '" ';
          itemBody = itemBody.replace('<ul', buildSelect);

            return itemBody;
        },

        _getTestOnlineSessionAnswerSub: function (self, virtualQuestionID, responseIdentifier) {
            var result = null;

            if (self.TestOnlineSessionAnswers() !== null) {
                ko.utils.arrayForEach(self.TestOnlineSessionAnswers(), function (item) {
                    if (virtualQuestionID === item.VirtualQuestionID()) {
                        if (item.TestOnlineSessionAnswerSubs().length > 0) {
                            ko.utils.arrayForEach(item.TestOnlineSessionAnswerSubs(), function (testOnlineSessionAnswerSub) {
                                if (testOnlineSessionAnswerSub.ResponseIdentifier() == responseIdentifier) result = testOnlineSessionAnswerSub;
                            });
                        }
                    }
                });
            }

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
                    textEntry.wrap('<div class="wrapTempTextEntry" style="' + getStyle + '"></div>');
                }

                var newTextEntry = $('<span></span>');
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
                    if (selectedAnswer !== "") {
                        newTextEntry.addClass('inputHasAnswered');
                    }
                }

                CopyAttributes(textEntry, newTextEntry);
                newTextEntry.addClass('input');
                newTextEntry.html(selectedAnswer);

				var correctAnswer = $('<span class="correct-answer-teacher-review" idresponse=' + responseIdentifier + '></span>');
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

                    correctAnswer.append('<br><br>' + correctAnswerText);
                });

                if (htmlListRationale != '') {
                    correctAnswer.append(htmlListRationale);
                }

                var reviewableIcon = '';
                if (responseProcessingTypeID == '2') {
                    newTextEntry.addClass('is-manually-graded');

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
                if (responseProcessingTypeID === 3) {
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

                if (textEntry.parents(".itemtypeonimage").length > 0) {
                    textEntry.parent().after(correctAnswer.outerHTML());
                    return $(reviewableIcon + newTextEntry.outerHTML() + textEntryMark.outerHTML() + '<br><span class="tag_number" idresponse="' + responseIdentifier + '"></span>');
                } else {
                    return $(reviewableIcon + newTextEntry.outerHTML() + textEntryMark.outerHTML() + '<br>' + '<br>' + correctAnswer.outerHTML() + '<br><span class="tag_number" idresponse="' + responseIdentifier + '"></span>');
                }


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
