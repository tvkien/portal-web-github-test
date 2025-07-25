(function ($) {
    $.widget('jquery.TOSSimpleChoice', {
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

            tree.find('choiceinteraction, choiceInteraction').replaceWith(function () {
                var interaction = $(this);
                var newInteraction = $('<ol></ol>');
                newInteraction.addClass('choiceInteraction');
                newInteraction.html(interaction.html());
                var responseIdentifier = $(interaction).attr('responseIdentifier');
                newInteraction.find('simplechoice, simpleChoice').replaceWith(function () {
                    var simpleChoice = $(this);
                    var newSimpleChoice = $('<li></li>');
                    newSimpleChoice.addClass('simpleChoice');
                    newSimpleChoice.html(simpleChoice.html());
                    var identifier = simpleChoice.attr('identifier');
                    var isCorrectIdentifier = false;
                    $(question.XmlContent).find('responsedeclaration[identifier="' + responseIdentifier + '"] correctresponse value').each(function () {
                        if ($(this).text() === identifier) isCorrectIdentifier = true;
                    });
                    if (isCorrectIdentifier && options.TheCorrectAnswer) {
                        newSimpleChoice.addClass('grey');
                        correctAnsers.push(identifier);
                    }
                    newSimpleChoice.attr('identifier', identifier);
                    if (question.Answer != null) {
                        if (question.Answer.AnswerChoice == identifier) {
                            newSimpleChoice.addClass('grey');
                            var mark = $('<img class="mark-choice" alt="" />');
                            if (isCorrectIdentifier) {
                                mark.attr('src', options.Self.MapPath + '/Content/themes/Print/TestOfStudent/icon-correct.png');
                            } else {
                                mark.attr('src', options.Self.MapPath + '/Content/themes/Print/TestOfStudent/icon-incorrect.png');
                            }
                            newSimpleChoice.append(mark);
                        }
                    }

                    return newSimpleChoice;
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
