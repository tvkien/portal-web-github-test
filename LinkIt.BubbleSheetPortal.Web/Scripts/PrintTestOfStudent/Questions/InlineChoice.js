(function ($) {
    $.widget('jquery.TOSInlineChoice', {
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

            var tree = $('<div>' + question.HtmlContent + '</div>');
            var correctAnsers = [];

            tree.find('inlinechoiceinteraction, inlineChoiceInteraction').replaceWith(function () {
                var interaction = $(this);
                var newInteraction = $('<ol></ol>');
                newInteraction.addClass('inlineChoiceInteraction');
                $.each(this.attributes, function (i, attribute) {
                    newInteraction.attr(attribute.name, attribute.value);
                });
                newInteraction.html(interaction.html());
                options.Util.CopyAttributes(interaction, newInteraction);
                var responseIdentifier = $(newInteraction).attr('responseIdentifier');
                newInteraction.find('inlinechoice, inlineChoice').replaceWith(function () {
                    var inlineChoice = $(this);
                    var newInlineChoice = $('<li></li>');
                    newInlineChoice.addClass('inlineChoice');
                    newInlineChoice.html(inlineChoice.html());
                    var identifier = inlineChoice.attr('identifier');
                    var isCorrectIdentifier = false;
                    $(question.XmlContent).find('responsedeclaration[identifier="' + responseIdentifier + '"] correctresponse value').each(function () {
                        if ($(this).text() == identifier) isCorrectIdentifier = true;
                    });
                    if (isCorrectIdentifier && options.TheCorrectAnswer) {
                        newInlineChoice.addClass('grey');
                        correctAnsers.push(identifier);
                    }
                    if (question.Answer !== null) {
                        if (question.Answer.AnswerChoice == identifier) {
                            newInlineChoice.addClass('grey');
                            var mark = $('<img class="mark-choice" alt="" />');
                            if (isCorrectIdentifier) {
                                mark.attr('src', options.Self.MapPath + '/Content/themes/Print/TestOfStudent/icon-correct.png');
                            } else {
                                mark.attr('src', options.Self.MapPath + '/Content/themes/Print/TestOfStudent/icon-incorrect.png');
                            }
                            newInlineChoice.append(mark);
                        }
                    }

                    return newInlineChoice;
                });

                return newInteraction;
            });

            if (options.GuidanceAndRationale) {
                question.GuidanceHTML = that.RenderRationale(question, tree);
            }

            if (options.TheQuestionContent) {
                question.HtmlContent = tree.html();
            } else {
                var answer = $('<div></div>');
                if (question.Answer != null) {
                    answer.html(question.Answer.AnswerChoice);
                }
                if (options.TheCorrectAnswer) {
                    answer.append('<p>(Correct Answer: ' + correctAnsers.join(',') + ')</p>');
                }
                question.HtmlContent = answer.html();
            }
        },
        RenderRationale: function (question, tree) {
            var that = this;
            var options = that.options;
            var guidanceHTML = '';
            if (tree.find('.guidance, .guidance_rationale').length) {
                tree.find('.guidance, .guidance_rationale').each(function (ind, guidance) {
                    var $guidance = $(guidance);
                    var guidanceIdentifier = $guidance.attr('identifier');
                    var guidanceHtml = '';
                    var isRatioContent = false;

                    isRatioContent = options.Util.GetGuidanceRationaleContent($guidance);

                    if (isRatioContent) {
                        guidanceHtml += '<div class="guidance-printTOS-Label">' + guidanceIdentifier + ' -</div>';
                        guidanceHtml += '<div class="guidance-printTOS-Body">' + $guidance.html() + '</div>';

                        var $divGuidance = $('<div class="guidance-printTOS-item"/>');
                        $divGuidance.addClass('guidance-printTOS-multiple');
                        $divGuidance.html(guidanceHtml);

                        guidanceHTML += $divGuidance.outerHTML();
                    }
                });
            }

            return guidanceHTML;
        }
    });
}(jQuery));
