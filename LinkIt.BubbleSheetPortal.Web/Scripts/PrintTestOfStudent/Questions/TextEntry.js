(function ($) {
    $.widget('jquery.TOSTextEntry', {
        options: {
            Util: null,
            Self: null,
            TheCorrectAnswer: null,
            GuidanceAndRationale: null,
            TheQuestionContent: null
        },

        Display: function (question) {
            var that = this;
            var options = that.options;

            var greaterThanOrEqual = '&#8805;';
            var lessThanOrEqual = '&#8804;';

            var tree = $('<div>' + question.HtmlContent + '</div>');
            var responseHtml = '';
            tree.find('textentryinteraction, textEntryInteraction').replaceWith(function () {
                var interaction = $(this);
                var newInteraction = $('<div></div>');
                newInteraction.addClass('textEntryInteraction');
                $.each(this.attributes, function (i, attribute) {
                    newInteraction.attr(attribute.name, attribute.value);
                });
                var responseProcessingTypeID = null;
                if (question.Answer !== null) {
                    newInteraction.html(question.Answer.AnswerText);
                    responseProcessingTypeID = question.Answer.ResponseProcessingTypeID;
                }

                if (responseProcessingTypeID == 3) {
                    $('div[virtualquestionid="' + question.VirtualQuestionID + '"] .jsScore').css('display', 'none');
                    return newInteraction;
                }

                var responseIdentifier = interaction.attr('responseIdentifier');
                var correctAnswer = $('<div></div>');
                correctAnswer.text('Correct Answer: ');
                var totalLength = $(question.XmlContent).find('responsedeclaration[identifier="' + responseIdentifier + '"] > correctresponse > value').length;
                $(question.XmlContent).find('responsedeclaration[identifier="' + responseIdentifier + '"] > correctresponse > value').each(function (index) {
                    var value = $(this);
                    var correctAnswerText = '';
                    var startValue = '';
                    var endValue = '';
                    var startExclusivity = '';
                    var endExclusivity = '';
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

                    if (startValue !== '') {
                        correctAnswerText = startOperator + ' ' + startValue;
                        if (endValue !== '') {
                            correctAnswerText = correctAnswerText + ' and ' + endOperator + ' ' + endValue;
                        }
                    } else {
                        if (endValue !== '') correctAnswerText = correctAnswerText + ' ' + endOperator + ' ' + endValue;
                    }

                    if (correctAnswerText === '') correctAnswerText = value.html();

                    if (index === totalLength - 1) {
                        correctAnswer.append('<span>' + correctAnswerText + '</span>');
                    } else {
                        correctAnswer.append('<span>' + correctAnswerText + ', </span>');
                    }

                    responseHtml = correctAnswer.html();
                });

                return newInteraction;
            });

            if (responseHtml !== '') {
                responseHtml = '<span class="textentry-correct">(' + responseHtml + ')</span>';
            } else {
                responseHtml = ' ';
            }

            if (options.TheCorrectAnswer) {
                tree.find('.mainbody, .mainBody').replaceWith(function () {
                    var mainBody = $(this);
                    var newMainBody = $('<div></div>');
                    newMainBody.addClass('mainBody');
                    newMainBody.html(mainBody.html() + responseHtml);
                    return newMainBody;
                });
            }

            if (options.GuidanceAndRationale) {
                question.GuidanceHTML = that.RenderRationale(question, tree);
            }

            if (options.TheQuestionContent) {
                question.HtmlContent = tree.html();
            } else {
                var answer = $('<div class="mainBody" styleName="mainBody"></div>');
                if (question.Answer != null && question.Answer.AnswerText != '') {
                    answer.html('<div class="textEntryInteraction">' + question.Answer.AnswerText + '</div>');
                }
                if (options.TheCorrectAnswer) {
                    answer.append(responseHtml);
                }
                question.HtmlContent = answer.html();
            }
        },
        RenderRationale: function (question, tree) {
            var that = this;
            var options = that.options;
            var guidanceHTML = '';
            if (tree.find('value[type="guidance"],value[type="guidance_rationale"]').length) {
                tree.find('value[type="guidance"],value[type="guidance_rationale"]').each(function (k, v) {
                    var identifier = $(v).attr('ansIdentifier');
                    var correctAnswer = tree.find('correctresponse value[identifier="' + identifier + '"]').html();
                    var newGuidance = $('<div class="guidance-printTOS-item"><div>');
                    var isRatioContent = false;

                    isRatioContent = options.Util.GetGuidanceRationaleContent($(v));

                    if (isRatioContent) {
                        newGuidance.html('<div class="guidance-printTOS-Label">' + correctAnswer + '</div><div class="guidance-printTOS-Body">' + $(v).html() + '</div>');
                        guidanceHTML += newGuidance.outerHTML();
                    }
                });
            }

            return guidanceHTML;
        }
    });
}(jQuery));
