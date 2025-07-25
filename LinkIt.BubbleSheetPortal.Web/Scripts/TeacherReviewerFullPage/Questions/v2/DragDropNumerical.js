(function ($) {
    $.widget('jquery.DragDropNumerical', {
        options: {
            DragDropStandardUtil: null,
            PostProcessQuestionDetails: null,
            Self: null
        },

        Display: function (question) {
            var that = this;
            var options = that.options;
            var self = options.Self;

            self.RefObjects(question.RefObjects());
            self.PointsPossible(question.PointsPossible());
            self.QTIItemSchemaID(question.QTIItemSchemaID());
            self.AnswerSubID('');

            var answerOfStudentForSelectedQuestion;

            if (self.TestOnlineSessionAnswers() !== null) {
                ko.utils.arrayForEach(self.TestOnlineSessionAnswers(), function (testOnlineSessionAnswer) {
                    if (question.VirtualQuestionID() === testOnlineSessionAnswer.VirtualQuestionID()) {
                        answerOfStudentForSelectedQuestion = testOnlineSessionAnswer;
                    }
                });
            }

            var answerText = '';
            if (!options.DragDropStandardUtil.IsNullOrEmpty(answerOfStudentForSelectedQuestion)) {
                self.PointsEarned(answerOfStudentForSelectedQuestion.PointsEarned());
                self.OldPointsEarned(answerOfStudentForSelectedQuestion.PointsEarned());
                self.AnswerID(answerOfStudentForSelectedQuestion.QTIOnlineTestSessionAnswerID());
                answerText = answerOfStudentForSelectedQuestion.AnswerText();
                self.AnswerImage(answerOfStudentForSelectedQuestion.AnswerImage());
                self.ShowHightLight(answerOfStudentForSelectedQuestion.HighlightQuestion(), question);
                self.VisitedTimes(answerOfStudentForSelectedQuestion.VisitedTimes());                
                self.TotalSpentTimeOnQuestion(answerOfStudentForSelectedQuestion.TotalSpentTimeOnQuestion());
            } else {
                question.XmlContent(question.DataXmlContent());
                self.PointsEarned(0);
                self.OldPointsEarned(0);
                self.AnswerID(0);
                answerText = '';
                self.AnswerImage('');
                self.ShowHightLight('', question);
                self.VisitedTimes(0);
                self.TotalSpentTimeOnQuestion('0s');
            }

            // answerText = '{SRC_1} + {SRC_2} = {DEST_1}';
            var mappings = '';
            var tree = $('<div></div>');
            tree.addClass('box-answer');
            tree.html(question.ItemBody());
            $('<div class="clearfix"></div>').prependTo(tree);
            $('<div class="clearfix"></div>').appendTo(tree);

            if (!options.DragDropStandardUtil.IsNullOrEmpty(answerText)) {
                mappings = answerText.split(',');
            }

            tree.find('destinationObject, destinationobject').each(function (index, desObj) {
                var $desObj = $(desObj);

                $desObj.replaceWith(function () {
                    var desObjType = $desObj.attr('type');
                    var $result = $('<div/>').attr({
                        'class': 'partialDestinationObject',
                        'type': desObjType
                    });

                    $desObj.find('destinationItem, destinationitem').each(function (index, desItem) {
                        var $desItem = $(desItem);
                        var desItemIdentifier = $desItem.attr('destIdentifier');
                        var desItemWidth = $desItem.attr('width');
                        var desItemHeight = $desItem.attr('height');
                        var desItemHtml = '';
                        var srcObjArr = [];

                        desItemWidth = desItemWidth === undefined ? 70 : desItemWidth;
                        desItemHeight = desItemHeight === undefined ? 20 : desItemHeight;

                        // Check if question have answer or not answer
                        if (mappings[0] !== '') {
                            for (var mi = 0, lenMappings = mappings.length; mi < lenMappings; mi++) {
                                var mapping = mappings[mi];
                                var mappingValue = mapping.split('-');
                                var mappingDest = mappingValue[0];
                                var mappingSrc = mappingValue[1];

                                // Compare destination identifier with mapping
                                if (desItemIdentifier === mappingDest && mappingSrc !== '') {
                                    var si = 0;

                                    srcObjArr = mappingSrc.split(';');

                                    // Reset destination item when destination item have source object
                                    $desItem.html('');

                                    while (si < srcObjArr.length) {
                                        var srcObjHtml = tree.find('sourceObject[srcIdentifier="' + srcObjArr[si] + '"], sourceobject[srcIdentifier="' + srcObjArr[si] + '"]').outerHTML();
                                        $desItem.append(srcObjHtml);
                                        si++;
                                    }
                                }
                            }
                        }

                        desItemHtml = '<div style="width: ' + desItemWidth + 'px; height: ' + desItemHeight + 'px; overflow: hidden;"><span style="vertical-align: middle;line-height: 48px;">' + $desItem.html() + '</span></div>';

                        $result.append(desItemHtml);
                    });

                    return $result;
                });
            });

            var questionDetails = tree.outerHTML();

            var mark = $('');
            if (!options.DragDropStandardUtil.IsNullOrEmpty(self.QTIOnlineTestSessionID()) && (self.IsComplete() || self.IsPendingReview())) {
                mark = $('<i class="jsIsAnswerCorrect" style="float:none;display:inline-block;position: relative;margin-left:15px;"></i>');
                if (self.PointsEarned() == self.PointsPossible()) {
                    mark.addClass('correct');
                } else if (self.PointsEarned() > 0) {
                    mark.addClass('partial');
                } else {
                    mark.addClass('incorrect');
                }
            }

            questionDetails = questionDetails + mark.outerHTML() + '<br>' + '<br><div class="dragdropnumerical-correct"><h3>Correct Answer:</h3>' + question.CorrectAnswer() + '</div>';

            if (options.PostProcessQuestionDetails !== null && 
                typeof (options.PostProcessQuestionDetails) == "function") {
                questionDetails = options.PostProcessQuestionDetails(questionDetails);
            }

            self.Respones(questionDetails);
            self.SectionInstruction(question.SectionInstruction());
        },

        _getHtmlValue: function (id) {
            return $.trim($('#' + id).html());
        },

        _setHtmlValue: function (id, val) {
            $('#' + id).html(val);
        }
    });
}(jQuery));
