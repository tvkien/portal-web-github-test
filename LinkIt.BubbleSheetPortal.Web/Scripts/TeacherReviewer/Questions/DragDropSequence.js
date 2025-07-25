(function ($) {
    $.widget('jquery.DragDropSequence', {
        options: {
            DragDropSequenceUtil: null,
            PostProcessQuestionDetails: null
        },

        Display: function (self, question) {
            var that = this;
            var options = that.options;
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

            var answerOfStudentForSelectedQuestion;
            ko.utils.arrayForEach(self.TestOnlineSessionAnswers(), function(testOnlineSessionAnswer) {
                if (question.VirtualQuestionID() === testOnlineSessionAnswer.VirtualQuestionID()) {
                    answerOfStudentForSelectedQuestion = testOnlineSessionAnswer;
                }
            });

            var answerText = '';
            if (!options.DragDropSequenceUtil.IsNullOrEmpty(answerOfStudentForSelectedQuestion)) {
                self.PointsEarned(answerOfStudentForSelectedQuestion.PointsEarned());
                self.OldPointsEarned(answerOfStudentForSelectedQuestion.PointsEarned());
                self.AnswerID(answerOfStudentForSelectedQuestion.QTIOnlineTestSessionAnswerID());
                answerText = answerOfStudentForSelectedQuestion.AnswerText();
                self.AnswerImage(answerOfStudentForSelectedQuestion.AnswerImage());
                self.ShowHightLight(answerOfStudentForSelectedQuestion.HighlightQuestion(), question);
                self.DisplayItemFeedback(true, answerOfStudentForSelectedQuestion);
            } else {
                question.XmlContent(question.DataXmlContent());
                self.PointsEarned(0);
                self.OldPointsEarned(0);
                self.AnswerID(0);
                answerText = '';
                self.AnswerImage('');
                self.ShowHightLight('', question);
                self.DisplayItemFeedback(false, null);
            }

            //answerText = 'SRC_1,SRC_4,SRC_2,SRC_3';
            if (answerText == null) answerText = '';

            var tree = $('<div></div>');
            tree.addClass('box-answer');
            tree.html(question.ItemBody());
            $('<div class="clearfix"></div>').prependTo(tree);
            $('<div class="clearfix"></div>').appendTo(tree);

            if (answerText != '') {
                var mappings = answerText.split(',');
                tree.find('partialsequence').replaceWith(function () {
                    var partialsequence = $(this);
                    var result = $('<partialsequence></partialsequence>');
                    CopyAttributes(partialsequence, result);
                    $.each(mappings, function (idx, mappingItem) {
                        var newSourceItem = $('<div class="sourceItem"></div>');
                        var currentSourceItem = partialsequence.find('sourceItem[identifier="' + mappingItem + '"]');
                        if (currentSourceItem != null) {
                            CopyAttributes(currentSourceItem, newSourceItem);
                            newSourceItem.css({ "width": currentSourceItem.attr("width"), "height": currentSourceItem.attr("height") }).html(currentSourceItem.html());
                            result.append(newSourceItem);
                        }
                    });
                    return result;
                });
            } else {
                tree.find('partialsequence').replaceWith(function () {
                    var partialsequence = $(this);
                    var result = $('<partialsequence></partialsequence>');
                    CopyAttributes(partialsequence, result);
                    partialsequence.find('sourceItem').each(function () {
                        var newSourceItem = $('<div class="sourceItem"></div>');
                        var currentSourceItem =$(this);
                        CopyAttributes(currentSourceItem, newSourceItem);
                        newSourceItem.css({ "width": currentSourceItem.attr("width"), "height": currentSourceItem.attr("height") }).html(currentSourceItem.html());
                        result.append(newSourceItem);
                    });
                    
                    return result;
                });
            }
           
            var questionDetails = tree.outerHTML();

            var mark = $('');
            if (!options.DragDropSequenceUtil.IsNullOrEmpty(self.QTIOnlineTestSessionID()) && (self.IsComplete() || self.IsPendingReview())) {
                mark = $('<i class="jsIsAnswerCorrect" style="float:none;display:inline-block;position: relative;margin-left:15px;"></i>');
                if (self.PointsEarned() == self.PointsPossible()) {
                    mark.addClass('correct');
                } else if (self.PointsEarned() > 0) {
                    mark.addClass('partial');
                } else {
                    mark.addClass('incorrect');
                }
            }

            questionDetails = questionDetails + mark.outerHTML();

            var correctAnswer = $(question.DataXmlContent()).find('correctResponse value').text();
            if (correctAnswer != null) {
                var correctMappings = correctAnswer.split(',');
                tree = $('<div></div>');
                tree.addClass('box-answer');
                tree.html(question.ItemBody());
                tree.find('partialsequence').each(function () {
                    var partialsequence = $(this);
                    var result = $('<partialsequence></partialsequence>');
                    CopyAttributes(partialsequence, result);
                    $.each(correctMappings, function (idx, mappingItem) {
                        var newSourceItem = $('<div class="sourceItem"></div>');
                        var currentSourceItem = partialsequence.find('sourceItem[identifier="' + mappingItem + '"]');
                        if (currentSourceItem != null) {
                            CopyAttributes(currentSourceItem, newSourceItem);
                            newSourceItem.html(currentSourceItem.html());
                            result.append(newSourceItem);
                        }
                    });

                    questionDetails = questionDetails + '<div class="resultSequence"><div class="correct_label">Correct Answer:</div> <br/><div class="clear"></div>' + result.outerHTML() + "</div>";
                });
            }

            if (options.PostProcessQuestionDetails != null && typeof (options.PostProcessQuestionDetails) == "function") {
                questionDetails = options.PostProcessQuestionDetails(questionDetails);
            }

            self.Respones(questionDetails);
            self.SectionInstruction(question.SectionInstruction());

            self.LoadImages();
            MathJax.Hub.Queue(["Typeset", MathJax.Hub]);
        },

        _getHtmlValue: function (id) {
            return $.trim($('#' + id).html());
        },

        _setHtmlValue: function (id, val) {
            $('#' + id).html(val);
        }
    });
}(jQuery));
