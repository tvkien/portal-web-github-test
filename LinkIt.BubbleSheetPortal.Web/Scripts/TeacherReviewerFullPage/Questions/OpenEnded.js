(function ($) {
    $.widget('jquery.OpenEnded', {
        options: {
            OpenEndedUtil: null,
            PostProcessQuestionDetails: null,
            Self: null
        },

        Display: function (question) {
            var that = this;
            var options = that.options;
            var self = options.Self;
            
            var baseQuestion = null;
            if (question.IsGhostVirtualQuestion()) {
                ko.utils.arrayForEach(self.Questions(), function (item) {
                    if (question.BaseVirtualQuestionID() === item.VirtualQuestionID()) {
                        baseQuestion = item;
                    }
                });
            }

            var answerOfStudentForSelectedQuestion = question.Answer();
            if (baseQuestion != null) {
                question.XmlContent(baseQuestion.XmlContent());
                question.DataXmlContent(baseQuestion.XmlContent());
                question.SectionInstruction(baseQuestion.SectionInstruction());

                if (!options.OpenEndedUtil.IsNullOrEmpty(answerOfStudentForSelectedQuestion)) {
                    ko.utils.arrayForEach(self.TestOnlineSessionAnswers(), function (answerOfBaseQuestion) {
                        if (baseQuestion.VirtualQuestionID() === answerOfBaseQuestion.VirtualQuestionID()) {
                            answerOfStudentForSelectedQuestion.AnswerText(answerOfBaseQuestion.AnswerText());
                            answerOfStudentForSelectedQuestion.AnswerImage(answerOfBaseQuestion.AnswerImage());
                            answerOfStudentForSelectedQuestion.HighlightQuestion(answerOfBaseQuestion.HighlightQuestion());
                        }
                    });
                }
            }

            var answerChoice = '';
            if (!options.OpenEndedUtil.IsNullOrEmpty(question.Answer())) {
                answerChoice = question.Answer().AnswerText();

                var data = question.Answer().PostAnswerLogs();
                self.PostAnswerLogs(data);
            }

            var drawable = false;
            var tree = $('<div></div>');
            tree.addClass('box-answer');
            tree.html(question.ItemBody());
            $('<div class="clearfix"></div>').prependTo(tree);
            $('<div class="clearfix"></div>').appendTo(tree);
            tree.find('extendedTextInteraction,extendedtextInteraction').replaceWith(function () {
                var notReviewed;
                if (self.SelectedQuestion()) {
                    notReviewed = self.SelectedQuestion().NotYetReviewCSS();
                }

                var extendedText = $(this);
                var extendedTextHeight = parseInt(extendedText.get(0).style.height.replace('px', ''), 10);
                var extendedTextResponse = extendedText.attr('responseidentifier');
                extendedText.attr('notReviewed', 'notReviewed');
                drawable = extendedText.attr('drawable');
                var uploadfile = extendedText.attr('uploadfile');
                if (drawable === 'true') {
                    return $(extendedText.outerHTML());
                } else if (uploadfile === "true") {
                    //This is created upload file
                    var htmlUpload = '';
                    htmlUpload += '<div class="upload-file-title-answer">Answer Area</div>';
                    htmlUpload += '<div class="upload-file-area">';
                    htmlUpload += '<div class="upload-file-title">Uploaded files:</div>';
                    htmlUpload += '<div class="upload-file-list">';
                    htmlUpload += '<a href="/Content/themes/AssignmentRegrader/images/icon-question.jpg" class="upload-file-item" target="_blank">File name show here</a>';
                    htmlUpload += '<a href="/Content/themes/AssignmentRegrader/images/icon-question.jpg" class="upload-file-item" target="_blank">File name show here</a>';
                    htmlUpload += '<a href="/Content/themes/AssignmentRegrader/images/icon-question.jpg" class="upload-file-item" target="_blank">File name show here</a>';
                    htmlUpload += '</div>';
                    htmlUpload += '</div>';
                    return $(htmlUpload);
                }

                var newExtendedText = $('<div/>');

                if (notReviewed) {
                    newExtendedText.addClass('red-border');
                } else {
                    newExtendedText.removeClass('red-border');
                }

                // Set default height extended text is 90 if before not set height
                if (isNaN(extendedTextHeight)) {
                    extendedTextHeight = 90;
                }

                if (self.ResponseProcessingTypeID() === '3') {
                    newExtendedText.removeClass('red-border');
                }

                newExtendedText
                    .addClass('textarea openEndedText')
                    .css({
                        'width': '100%',
                        'height': extendedTextHeight + 'px',
                        'padding': '5px'
                    })
                    .attr({ "data-response-id": extendedText.attr("responseidentifier") });

                CopyAttributes(extendedText, newExtendedText);

                if (!options.OpenEndedUtil.IsNullOrEmpty(answerChoice)) {
                    if (answerChoice.indexOf('&#60;') > -1 && answerChoice.indexOf('&#62;') > -1) {
                        answerChoice = options.OpenEndedUtil.replaceStringLessOrLarge(answerChoice);
                    } else {
                        answerChoice = answerChoice.replace(/[\r\n]+/g, '<br/>');
                    }
                }

                newExtendedText.append(answerChoice);

                return $('<div>Answer Area</div>' + newExtendedText.outerHTML());
            });

            var questionDetails = tree.outerHTML();
            if (options.PostProcessQuestionDetails != null &&
                typeof (options.PostProcessQuestionDetails) == "function") {
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
