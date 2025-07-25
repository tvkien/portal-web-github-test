(function ($) {
    $.widget('jquery.TOSComplexItem', {
        questionHtml: '',
        options: {
            Util: null,
            Self: null,
            TheCorrectAnswer: null,
            GuidanceAndRationale: null,
            TheQuestionContent: null
        },
        widgetScope: {
            rationaleDictionary: []
        },
        Display: function (question) {
            var that = this;
            var options = that.options;

            that.widgetScope.rationaleDictionary = [];

            that._RenderOpenEnded(question);
            that._RenderTextEntry(question);
            that._SimpleChoice(question);
            that._InlineChoice(question);

            if (!options.TheQuestionContent) {
                question.HtmlContent = that.questionHtml;
            }

            if (options.GuidanceAndRationale) {
                that._CombineAndRenderRationale(question);
            }
        },
        _RenderOpenEnded: function (question) {
            var that = this;
            var options = that.options;

            if (!options.TheQuestionContent) {
                question.HtmlContent = '';
                return;
            }

            var tree = $('<div>' + question.HtmlContent + '</div>');
            tree.find('extendedtextinteraction, extendedTextInteraction').replaceWith(function () {
                var interaction = $(this);
                var newInteraction = $('<div></div>');
                if (interaction.attr('drawable') == 'true') {
                    newInteraction.addClass('extendedTextInteractionDrawable');
                } else {
                    var newInteractionHeight = parseInt(interaction.get(0).style.height.replace('px', ''), 10);

                    // Set default height extended text is 90 if before not set height
                    if (isNaN(newInteractionHeight)) {
                        newInteractionHeight = 90;
                    }

                    newInteraction.addClass('extendedTextInteraction');
                    newInteraction.css('height', newInteractionHeight + 'px');
                }

                if (interaction.attr('drawable') == 'true') {
                    newInteraction.html(interaction.html());
                } else {
                    var responseIdentifier = interaction.attr('responseIdentifier');
                    var answerSub = that._GetAnswerSub(responseIdentifier, question);
                    if (answerSub !== null) {
                        var questionAnswerText = answerSub.AnswerText;
                        // Replace "\n" to "<br/>" tag in extended text
                        if (questionAnswerText !== '' && questionAnswerText !== null) {
                            questionAnswerText = options.Util.replaceStringLessOrLarge(questionAnswerText);
                            questionAnswerText = questionAnswerText.replace(new RegExp('\r?\n','g'), '<span class="u-linebreak"></span>');
                            questionAnswerText = questionAnswerText.replace(/<br\s*[\/]?>/gi, '<span class="u-linebreak"></span>');
                            newInteraction.css('height', 'auto');
                        }

                        newInteraction.html(questionAnswerText);
                    }
                }

                that.questionHtml += '<div></div>';

                return newInteraction;
            });

            question.HtmlContent = tree.html();
        },
        _SimpleChoice: function (question) {
            var that = this;
            var options = that.options;

            if (!options.TheQuestionContent) {
                question.HtmlContent = '';
                return;
            }

            var tree = $('<div>' + question.HtmlContent + '</div>');
            var studentAnswer = [];
            var correctAnswer = [];

            tree.find('choiceinteraction, choiceInteraction').replaceWith(function () {
                studentAnswer = [];
                correctAnswer = [];
                var interaction = $(this);
                var newInteraction = $('<ol></ol>');
                newInteraction.addClass('choiceInteraction');
                newInteraction.html(interaction.html());
                options.Util.CopyAttributes(interaction, newInteraction);
                var responseIdentifier = $(interaction).attr('responseIdentifier');

                that._RenderRationaleChoiceIteraction(question, interaction, responseIdentifier);

                newInteraction.find('simplechoice, simpleChoice').replaceWith(function () {
                    var simpleChoice = $(this);
                    var newSimpleChoice = $('<li>&nbsp</li>');
                    newSimpleChoice.addClass('simpleChoice');
                    newSimpleChoice.html(simpleChoice.html());
                    var identifier = simpleChoice.attr('identifier');
                    var isCorrectIdentifier = false;
                    $(question.XmlContent).find('responsedeclaration[identifier="' + responseIdentifier + '"] correctresponse value').each(function () {
                        if ($(this).text() === identifier) isCorrectIdentifier = true;
                    });
                    if (isCorrectIdentifier && options.TheCorrectAnswer) {
                        newSimpleChoice.addClass('grey');
                        correctAnswer.push(identifier);
                    }
                    newSimpleChoice.attr('identifier', identifier);
                    var answerSub = that._GetAnswerSub(responseIdentifier, question);
                    if (answerSub !== null) {
                        if (answerSub.AnswerChoice == identifier) {
                            newSimpleChoice.addClass('grey');
                            var mark = $('<img class="mark-choice" alt="" />');
                            if (isCorrectIdentifier) {
                                mark.attr('src', options.Self.MapPath + '/Content/themes/Print/TestOfStudent/icon-correct.png');
                            } else {
                                mark.attr('src', options.Self.MapPath + '/Content/themes/Print/TestOfStudent/icon-incorrect.png');
                            }
                            newSimpleChoice.append(mark);
                            studentAnswer.push(identifier);
                        }
                    }

                    return newSimpleChoice;
                });

                that.questionHtml += '<div>' + studentAnswer.join(',') + '</div>';
                if (options.TheCorrectAnswer) {
                    that.questionHtml += '<div>(Correct Answer: ' + correctAnswer.join(',') + ')</div>';
                }

                return newInteraction;
            });

            question.HtmlContent = tree.html();
        },
        _InlineChoice: function (question) {
            var that = this;
            var options = that.options;

            if (!options.TheQuestionContent) {
                question.HtmlContent = '';
                return;
            }

            var tree = $('<div>' + question.HtmlContent + '</div>');
            var studentAnswer = [];
            var correctAnswer = [];

            tree.find('inlinechoiceinteraction, inlineChoiceInteraction').replaceWith(function () {
                studentAnswer = [];
                correctAnswer = [];
                var interaction = $(this);
                var newInteraction = $('<ol></ol>');
                newInteraction.addClass('inlineChoiceInteraction');
                newInteraction.html(interaction.html());
                var responseIdentifier = $(interaction).attr('responseIdentifier');

                that._RenderRationaleChoiceIteraction(question, interaction, responseIdentifier);

                newInteraction.find('inlinechoice, inlineChoice').replaceWith(function () {
                    var simpleChoice = $(this);
                    var newSimpleChoice = $('<li>&nbsp</li>');
                    newSimpleChoice.addClass('inlineChoice');
                    newSimpleChoice.html(simpleChoice.html());
                    var identifier = simpleChoice.attr('identifier');
                    var isCorrectIdentifier = false;
                    $(question.XmlContent).find('responsedeclaration[identifier="' + responseIdentifier + '"] correctresponse value').each(function () {
                        if ($(this).text() === identifier) isCorrectIdentifier = true;
                    });
                    if (isCorrectIdentifier && options.TheCorrectAnswer) {
                        newSimpleChoice.addClass('grey');
                        correctAnswer.push(identifier);
                    }
                    var answerSub = that._GetAnswerSub(responseIdentifier, question);
                    if (answerSub !== null) {
                        if (answerSub.AnswerChoice == identifier) {
                            newSimpleChoice.addClass('grey');
                            var mark = $('<img class="mark-choice" alt="" />');
                            if (isCorrectIdentifier) {
                                mark.attr('src', options.Self.MapPath + '/Content/themes/Print/TestOfStudent/icon-correct.png');
                            } else {
                                mark.attr('src', options.Self.MapPath + '/Content/themes/Print/TestOfStudent/icon-incorrect.png');
                            }
                            newSimpleChoice.append(mark);
                            studentAnswer.push(identifier);
                        }
                    }

                    return newSimpleChoice;
                });

                that.questionHtml += '<div>' + studentAnswer.join(',') + '</div>';
                if (options.TheCorrectAnswer) {
                    that.questionHtml += '<div>(Correct Answer: ' + correctAnswer.join(',') + ')</div>';
                }

                return newInteraction;
            });

            question.HtmlContent = tree.html();
        },
        _RenderTextEntry: function (question) {
            var that = this;
            var options = that.options;

            if (!options.TheQuestionContent) {
                question.HtmlContent = '';
                return;
            }

            var greaterThanOrEqual = '&#8805;';
            var lessThanOrEqual = '&#8804;';

            var tree = $('<div>' + question.HtmlContent + '</div>');
            tree.find('textentryinteraction, textEntryInteraction').replaceWith(function () {
                var interaction = $(this);
                var newInteraction = $('<div></div>');

                var responseIdentifier = interaction.attr('responseIdentifier');

                that._RenderRationaleTextEntry(question, tree, responseIdentifier);

                var values = $(question.XmlContent).find('responsedeclaration[identifier="' + responseIdentifier + '"] > correctresponse > value');
                var correctAnswerText = '';

                var startValue = '';
                var endValue = '';
                var startExclusivity = '';
                var endExclusivity = '';

                values.find('rangeValue, rangevalue').each(function () {

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

                if (startValue !== '') {
                    correctAnswerText = startOperator + ' ' + startValue;
                    if (endValue !== '') {
                        correctAnswerText = correctAnswerText + ' and ' + endOperator + ' ' + endValue;
                    }
                } else {
                    if (endValue !== '') correctAnswerText = correctAnswerText + ' ' + endOperator + ' ' + endValue;
                }

                if (correctAnswerText === '') {
                    values.each(function () {
                        var value = $(this);
                        if (correctAnswerText !== '') correctAnswerText = correctAnswerText + ', ';
                        correctAnswerText = correctAnswerText + value.html();
                    });
                }

                var responseProcessingTypeID = null;
                var answerSub = that._GetAnswerSub(responseIdentifier, question);
                if (answerSub != null) {
                    var questionAnswerText = answerSub.AnswerText;
                    responseProcessingTypeID = answerSub.ResponseProcessingTypeID;
                    // Replace "\n" to "<br/>" tag in extended text
                    if (questionAnswerText !== '' && questionAnswerText !== null) {
                        questionAnswerText = questionAnswerText.replace(new RegExp('\r?\n','g'), '<span class="u-linebreak"></span>');
                    }
                    newInteraction.html(questionAnswerText);
                }

                if (correctAnswerText !== '') correctAnswerText = '(Correct answer: ' + correctAnswerText + ')';
                var htmlResult = '';
                if (responseProcessingTypeID == 3) {
                    htmlResult = '<span><span class="textEntryInteraction">' + newInteraction.html() + '</span></span>';
                } else {
                    htmlResult = '<span><span class="textEntryInteraction">' + newInteraction.html() + '</span><span class="textentry-correct">' + correctAnswerText + '</span></span>';
                }


                that.questionHtml += '<div></div>';

                return $(htmlResult);
            });

            question.HtmlContent = tree.html();
        },
        _GetAnswerSub: function (responseIdentifier, question) {
            var answerSub = null;

            if (question.Answer !== null && question.Answer.AnswerSubs.length > 0) {
                $.each(question.Answer.AnswerSubs, function (idx, item) {
                    if (responseIdentifier == item.ResponseIdentifier) {
                        answerSub = item;
                    }
                });
            }

            return answerSub;
        },
        _RenderRationaleTextEntry: function (question, tree, identifier) {
            var that = this;
            var widgetScope = that.widgetScope;
            var options = that.options;

            var guidanceHTML = '';
            var responseDeclarationSelector = 'responsedeclaration[identifier = "' + identifier + '"]';
            var rationaleSelector = responseDeclarationSelector + ' value[type="guidance"],' + responseDeclarationSelector + ' value[type="guidance_rationale"]';
            if (tree.find(rationaleSelector).length) {
                guidanceHTML += '<div class="guidance-printTOS-Heading">' + identifier.replace('RESPONSE_', 'Part ') + '. </div>';
                tree.find(rationaleSelector).each(function (k, v) {
                    var ansIdentifier = $(v).attr('ansIdentifier');
                    var correctResponseSelector = responseDeclarationSelector + ' correctResponse value[identifier="' + ansIdentifier + '"]';
                    var correctAnswer = tree.find(correctResponseSelector).html();
                    var newGuidance = $('<div class="guidance-printTOS-item"><div>');
                    var isRatioContent = false;

                    isRatioContent = options.Util.GetGuidanceRationaleContent($(v));

                    if (isRatioContent) {
                        newGuidance.html('<div class="guidance-printTOS-Label">' + correctAnswer + '</div><div class="guidance-printTOS-Body">' + $(v).html() + '</div>');
                        guidanceHTML += newGuidance.outerHTML();
                    }
                });
            }

            // Check guidance is blank
            if (!$(guidanceHTML).find('.guidance-printTOS-Body').length) {
                guidanceHTML = '';
            }

            widgetScope.rationaleDictionary.push({
                Identifier: parseInt(identifier.replace('RESPONSE_', '')),
                GuidenceHTML: guidanceHTML
            });

            return guidanceHTML;
        },
        _RenderRationaleChoiceIteraction: function (question, interaction, identifier) {
            var that = this;
            var widgetScope = that.widgetScope;
            var options = that.options;

            var guidanceHTML = '';
            if (interaction.find('.guidance, .guidance_rationale').length) {
                guidanceHTML += '<div class="guidance-printTOS-Heading">' + identifier.replace('RESPONSE_', 'Part ') + '. </div>';
                interaction.find('.guidance, .guidance_rationale').each(function (k, v) {
                    var ansIdentifier = $(v).attr('identifier');
                    var newGuidance = $('<div class="guidance-printTOS-item guidance-printTOS-multiple"><div>');
                    var isRatioContent = false;

                    isRatioContent = options.Util.GetGuidanceRationaleContent($(v));

                    if (isRatioContent) {
                        newGuidance.html('<div class="guidance-printTOS-Label">' + ansIdentifier + ' -</div><div class="guidance-printTOS-Body">' + $(v).html() + '</div>');
                        guidanceHTML += newGuidance.outerHTML();
                    }
                });
            }

            // Check guidance is blank
            if (!$(guidanceHTML).find('.guidance-printTOS-Body').length) {
                guidanceHTML = '';
            }

            widgetScope.rationaleDictionary.push({
                Identifier: parseInt(identifier.replace('RESPONSE_', '')),
                GuidenceHTML: guidanceHTML
            });

            return guidanceHTML;
        },
        _CombineAndRenderRationale: function (question) {
            var that = this;
            var widgetScope = that.widgetScope;

            var sorted = widgetScope.rationaleDictionary.sort(function (a, b) {
                var a1 = a.Identifier, b1 = b.Identifier;
                if (a1 == b1) return 0;
                return a1 > b1 ? 1 : -1;
            });

            $.each(sorted, function (index, item) {
                question.GuidanceHTML += item.GuidenceHTML;
            });
        },
    });
}(jQuery));
