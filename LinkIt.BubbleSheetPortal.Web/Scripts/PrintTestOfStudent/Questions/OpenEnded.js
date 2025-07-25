(function ($) {
    $.widget('jquery.TOSOpenEnded', {
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

            if (options.TheQuestionContent) {
                var tree = $('<div>' + question.HtmlContent + '</div>');
                tree.find('responsedeclaration, responseDeclaration, stylesheet').replaceWith(function () {
                    return $('');
                });

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

                    var responseProcessingTypeID = null;
                    if (interaction.attr('drawable') == 'true') {
                        newInteraction.html(interaction.html());
                    } else if (question.Answer != null) {
                        var questionAnswerText = question.Answer.AnswerText;
                        responseProcessingTypeID = question.Answer.ResponseProcessingTypeID;
                        // Replace "\n" to "<br/>" tag in extended text
                        if (questionAnswerText !== '' && questionAnswerText !== null) {
                            questionAnswerText = options.Util.replaceStringLessOrLarge(questionAnswerText);
                            questionAnswerText = questionAnswerText.replace(new RegExp('\r?\n','g'), '<span class="u-linebreak"></span>');
                            questionAnswerText = questionAnswerText.replace(/<br\s*[\/]?>/gi, '<span class="u-linebreak"></span>');
                            newInteraction.css('height', 'auto');
                        }

                        newInteraction.html(questionAnswerText);
                    }
                    if (responseProcessingTypeID == 3)
                        $('div[virtualquestionid="' + question.VirtualQuestionID + '"] .jsScore').css('display', 'none');

                    return newInteraction;
                });

                question.HtmlContent = tree.html();
            } else {
                var answer = $('<div class="mainBody" styleName="mainBody"></div>');
                var responseProcessingTypeID = null;
                if (question.Answer != null && (question.Answer.AnswerText != '' || question.Answer.AnswerText !== null)) {
                    var questionAnswerText = question.Answer.AnswerText;
                    // Replace "\n" to "<br/>" tag in extended text
                    responseProcessingTypeID = question.Answer.ResponseProcessingTypeID;
                    questionAnswerText = questionAnswerText.replace(new RegExp('\r?\n','g'), '<span class="u-linebreak"></span>');

                    answer.html('<div class="extendedTextInteraction">' + questionAnswerText + '</div>');
                }
                if (responseProcessingTypeID == 3)
                    $('div[virtualquestionid="' + question.VirtualQuestionID + '"] .jsScore').css('display', 'none');

                question.HtmlContent = answer.html();
            }
        },
    });
}(jQuery));
