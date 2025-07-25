(function ($) {
    $.widget('jquery.TOSMultipleChoiceVariable', {
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

            if (!options.TheQuestionContent) {
                question.HtmlContent = '';
                return;
            }

            var tree = $('<div>' + question.HtmlContent + '</div>');
            tree.find('responsedeclaration, responseDeclaration, stylesheet').replaceWith(function () {
                return $('');
            });

            tree.find('choiceinteraction, choiceInteraction').replaceWith(function () {
                var interaction = $(this);
                var newInteraction = $('<ol></ol>');
                newInteraction.addClass('choiceInteraction choiceInteractionVariable');
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
                        if ($(this).text() == identifier) isCorrectIdentifier = true;
                    });

                    if (isCorrectIdentifier) newSimpleChoice.addClass('grey');

                    if (question.Answer != null) {
                        var answerChoice = question.Answer.AnswerChoice;
                        if (answerChoice != null && answerChoice != '') {
                            ko.utils.arrayForEach(answerChoice.split(','), function (item) {
                                if ($.trim(item) == identifier) {
									newSimpleChoice.addClass('grey');
                                    /*var mark = $('<img class="mark-choice" alt="" />');
                                    if (isCorrectIdentifier) {
                                        mark.attr('src', options.Self.MapPath + '/Content/themes/Print/TestOfStudent/icon-correct.png');
                                    } else {
                                        mark.attr('src', options.Self.MapPath + '/Content/themes/Print/TestOfStudent/icon-incorrect.png');
                                    }
                                    newSimpleChoice.append(mark);*/
                                }
                            });
                        }
                    }

                    return newSimpleChoice;
                });

                return newInteraction;
            });

            question.HtmlContent = tree.html();
        },
    });
}(jQuery));
