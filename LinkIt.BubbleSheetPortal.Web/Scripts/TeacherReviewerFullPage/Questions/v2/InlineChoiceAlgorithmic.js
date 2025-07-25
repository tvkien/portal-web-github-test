(function ($) {
    $.widget('jquery.InlineChoiceAlgorithmic', {
        options: {
            InlineChoiceAlgorithmicUtil: null,
            PostProcessQuestionDetails: null,
            Self: null
        },

        Display: function (question) {
            var that = this;
            var options = that.options;
            var self = options.Self;

            var selectedAnswer = '';

            if (!Reviewer.IsNullOrEmpty(question.Answer())) {
                selectedAnswer = question.Answer().AnswerChoice();
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
                                attrBoxes += '' + this.name + '="' + this.value + '" ';
                            }
                        });
                    });

                    var $newBoxes = $('<div ' + attrBoxes + '>' + contentHTML + '</div>');
                    return $newBoxes;
                }
            });

            tree.find('inlinechoiceinteraction').replaceWith(function () {
                var inlineChoice = $(this);
                var responseIdentifier = inlineChoice.attr('responseIdentifier');
                var originChoiceInteraction = $(question.DataXmlContent()).find('[responseIdentifier="' + responseIdentifier + '"]');
                var newInlineChoice = $('<div></div>');
                newInlineChoice.html(inlineChoice.html());

                //Replace if the item has an Item Type Interactions onto an Image
                if (inlineChoice.parents('.itemtypeonimage').length > 0) {
                    var getStyle = inlineChoice.attr('style');

                    inlineChoice.removeAttr('style');
                    inlineChoice.wrap('<div style="' + getStyle + '"></div>');
                }

                newInlineChoice.prepend('<li></li>');
                newInlineChoice.find('inlinechoice').replaceWith(function () {
                      var identifier = $(this).attr('identifier');
                      var orginInlineChoice = originChoiceInteraction.find('inlineChoice[identifier="' + identifier + '"]');
                      if ($(this).find('div[typemessage]').length || orginInlineChoice.find('div[typemessage]').length) {
                      var arrGuidances = $(this).find('div[typemessage]');
                      if (arrGuidances.length === 0) {
                        arrGuidances = orginInlineChoice.find('div[typemessage]');
                      }
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

                for (var icount = 0, lenlistMessageGuidance = listMessageGuidance.length; icount < lenlistMessageGuidance; icount++) {
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
                    htmlGuidance = self.buildGuidanceInlineChoice(listObjTypeMessages, responseIdentifier);
                }

                if (htmlGuidance != '') {
                    iconString = '<img responseidentifier="' + responseIdentifier + '" alt="Guidance" style="margin-left: 5px; width: 1.5em; height: 1.5em" class="imageupload bntGuidance" src="../../Content/themes/TestMaker/v2/multiplechoice_images_guidance_unchecked.svg" title="">';
                  htmlContentGuidance = '<div name="typeGuidance" style="display: none; border: 1px solid var(--grey7);padding: 5px;" idReponse="' + responseIdentifier + '">' + htmlGuidance + '</div>';
                }

              var correctPanel = $('<p class="correct-answer-teacher-review" style="color:var(--red); display: inline-block;"></p>');
                correctPanel.html('Correct Answer: ' + correctAnswer);

                var select = $('<ul class="inlineChoiceFormat"></ul>');
                CopyAttributes(inlineChoice, select);
                select.html(newInlineChoice.html());

                select.css({
                    'visibility': 'hidden',
                    'height': 0,
                    'overflow': 'hidden',
                    'position': 'absolute'
                });

                // Replace if the item has an Item Type Interactions onto an Image
                if (inlineChoice.parents('.itemtypeonimage').length > 0) {
                    inlineChoice.parent().after(htmlContentGuidance + iconString);
                    return $(select.outerHTML() + '<br>');
                } else {
                    if (selectedAnswer != '') {
                        return $(select.outerHTML() + '<br>' + htmlContentGuidance + iconString + '<br>');
                    } else {
                        return $(select.outerHTML() + iconString + '<br>' + htmlContentGuidance + '<br>');
                    }
                }
            });

            var questionDetails = tree.outerHTML();
            if (question.AlgorithmicCorrectAnswers() != null && question.AlgorithmicCorrectAnswers().length) {
                questionDetails += '<br> <div class="btn-show-all-correct-answer big-button" onClick="ShowAllCorrectAnswers()">Show all correct answers</div>';
            }

            if (options.PostProcessQuestionDetails != null && typeof (options.PostProcessQuestionDetails) == "function") {
                questionDetails = options.PostProcessQuestionDetails(questionDetails);
            }
            self.Respones(questionDetails);

            //show dropdown inline choice customize
            $('.inlineChoiceFormat').selectbox({
                speed: 100,
                is_Disabled: true,
                onOpen: function (inst) {
                    var $self = $(this);
                    var $parent = $self.parents('td');
                    var $doc = $(document);

                    if ($parent.length > 0) {
                        // Visible inline choice in table
                        $parent.css('overflow', 'visible');
                    }

                    // Reduce z-index other inline choice
                    $doc.find('.sbHolder').not($self.next('.sbHolder')).css('z-index', 'auto');
                },
                onClose: function (inst) {
                    var $self = $(this);
                    var $parent = $self.parents('td');
                    var $doc = $(document);

                    if ($parent.length > 0) {
                        // Reset after closed inline choice
                        $parent.css('overflow', 'hidden');
                    }

                    // Reset z-index all inline choice
                    $doc.find('.sbHolder').css('z-index', 'auto');
                }
            });

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
        },

        ShowAllCorrectAnswers: function (self, question) {
            var that = this;
            var options = that.options;
            var algorithmicCorrectAnswers = $('<div/>');
            var algorithmicPoints = [];

            ko.utils.arrayForEach(question.AlgorithmicCorrectAnswers(), function (item) {
                var correctAnswer = item.ConditionValue().toString();
                var tree = that.FillCorrectAnswer(self, question, correctAnswer);
                tree.appendTo(algorithmicCorrectAnswers);
                algorithmicPoints.push(item.PointsEarned);
            });

            var questionDetails = algorithmicCorrectAnswers.outerHTML();
            Reviewer.popupAlertMessage(questionDetails, 'ui-popup-fullpage ui-popup-algorithmic-correct-answer', 700, 500, false);
            Reviewer.createTabWidget('.ui-popup-fullpage.ui-popup-algorithmic-correct-answer', algorithmicPoints);
        },

        FillCorrectAnswer: function (self, question, selectedAnswer) {
            var that = this;
            var options = that.options;
            var self = options.Self;

            var correctAnswer = '';
            var correctIdentifier = '';
            var tree = $('<div></div>');
            tree.addClass('box-answer');
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
                                attrBoxes += '' + this.name + '="' + this.value + '" ';
                            }
                        });
                    });

                    var $newBoxes = $('<div ' + attrBoxes + '>' + contentHTML + '</div>');
                    return $newBoxes;
                }
            });

            tree.find('inlinechoiceinteraction').replaceWith(function () {
                var inlineChoice = $(this);
                responseIdentifier = inlineChoice.attr('responseIdentifier');
                var newInlineChoice = $('<div></div>');
                newInlineChoice.html(inlineChoice.html());

                newInlineChoice.prepend('<li></li>');
                newInlineChoice.find('inlinechoice').replaceWith(function () {

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

                    if (identifier == selectedAnswer) {
                        correctAnswer = item.html();
                    }

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

                var select = $('<ul class="inlineChoiceFormat"></ul>');
                CopyAttributes(inlineChoice, select);
                select.html(newInlineChoice.html());

                select.css({
                    'visibility': 'hidden',
                    'height': 0,
                    'overflow': 'hidden',
                    'position': 'absolute'
                });

                if (selectedAnswer != '') {
                    return $(select.outerHTML() + '<br>' + htmlContentGuidance + iconString + '<br>');
                } else {
                    return $(select.outerHTML() + iconString + '<br>' + htmlContentGuidance + '<br>');
                }
            });

            //show dropdown inline choice customize
            tree.find('.inlineChoiceFormat').selectbox({
                speed: 100,
                is_Disabled: true,
                onOpen: function (inst) {
                    var $self = $(this);
                    var $parent = $self.parents('td');
                    var $doc = $(document);

                    if ($parent.length > 0) {
                        // Visible inline choice in table
                        $parent.css('overflow', 'visible');
                    }

                    // Reduce z-index other inline choice
                    $doc.find('.sbHolder').not($self.next('.sbHolder')).css('z-index', 'auto');
                },
                onClose: function (inst) {
                    var $self = $(this);
                    var $parent = $self.parents('td');
                    var $doc = $(document);

                    if ($parent.length > 0) {
                        // Reset after closed inline choice
                        $parent.css('overflow', 'hidden');
                    }

                    // Reset z-index all inline choice
                    $doc.find('.sbHolder').css('z-index', 'auto');
                }
            });

            if (selectedAnswer != "" && selectedAnswer != 'U') {
                var htmlAnswer = tree.find('ul[responseidentifier="' + resIdentifier + '"]').find('li[identifier="' + selectedAnswer + '"]').html();
                tree.find('a[responseidentifier="' + resIdentifier + '"]').attr('identifier', selectedAnswer).html(htmlAnswer);
                tree.find('a[responseidentifier="' + resIdentifier + '"]').parent().css({
                    'width': 'auto',
                });

                var wParent = tree.find('a[responseidentifier="' + resIdentifier + '"]').parent().width();
                var wMainBody = tree.find('#divQuestionDetails .mainBody').width();

                if (wParent > wMainBody) {
                    tree.find('a[responseidentifier="' + resIdentifier + '"]').parent().css({
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
                tree.find('.sbSelector[responseidentifier="' + idRes + '"]').parent().css({'width': 'auto' });
            });

            tree.find('.sbToggle').remove();
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
