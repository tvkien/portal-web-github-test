(function ($) {
    $.widget('jquery.DragDropStandard', {
        options: {
            DragDropStandardUtil: null,
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
            if (!options.DragDropStandardUtil.IsNullOrEmpty(answerOfStudentForSelectedQuestion)) {
                self.PointsEarned(answerOfStudentForSelectedQuestion.PointsEarned());
                self.OldPointsEarned(answerOfStudentForSelectedQuestion.PointsEarned());
                self.AnswerID(answerOfStudentForSelectedQuestion.QTIOnlineTestSessionAnswerID());
                answerText = answerOfStudentForSelectedQuestion.AnswerText();
                self.AnswerImage(answerOfStudentForSelectedQuestion.AnswerImage());
                self.ShowHightLight(answerOfStudentForSelectedQuestion.HighlightQuestion(), question);
                self.DisplayItemFeedback(true,answerOfStudentForSelectedQuestion);
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

            //answerText = 'DEST_2-SRC_2;SRC_3;SRC4,DEST_1-SRC_1,DEST_3-SRC_3;SRC_2;,DEST_5-SRC_3';
            var mappingsAnswer = answerText.split(',');
            var mappingsCorrect = question.CorrectAnswer().split(',');
            var answerHtml = '';
            var correctHtml = '';
            var questionDetails = '';

            // Display answer of student
            answerHtml = that.GetContentDragDrop(self, $(question.ItemBody()), mappingsAnswer);
            // Display correct answer
            correctHtml = that.GetContentDragDrop(self, $(question.ItemBody()), mappingsCorrect);

            questionDetails = answerHtml.outerHTML();
            question.CorrectAnswerHTML = correctHtml.outerHTML();

            var mark = $('');
            if (!options.DragDropStandardUtil.IsNullOrEmpty(self.QTIOnlineTestSessionID()) &&
                (self.IsComplete() || self.IsPendingReview())) {
                mark = $('<i class="jsIsAnswerCorrect" style="float:none;display:inline-block;position: relative;margin-left:15px;"></i>');
                if (self.PointsEarned() != 0 && self.PointsEarned() == self.PointsPossible()) {
                    mark.addClass('correct');
                } else if (self.PointsEarned() > 0) {
                    mark.addClass('partial');
                } else {
                    mark.addClass('incorrect');
                }
            }

            questionDetails += mark.outerHTML();
            questionDetails += '<div class="btn-show-correct-answer big-button" onClick="ShowCorrectAnswer()">Show Correct Answer</div>';

            if (options.PostProcessQuestionDetails !== null &&
                typeof (options.PostProcessQuestionDetails) == "function") {
                questionDetails = options.PostProcessQuestionDetails(questionDetails);
            }

            self.Respones(questionDetails);
            self.SectionInstruction(question.SectionInstruction());

            self.LoadImages();
            MathJax.Hub.Queue(["Typeset", MathJax.Hub]);
        },

        ShowCorrectAnswer: function (self, question) {
            // popupAlertMessage from ckeditor_utils
            popupAlertMessage('alert', question.CorrectAnswerHTML, 700, 500);
        },

        GetContentDragDrop: function(self, selector, mappings) {
            var $selector = $(selector);

            $selector.find('destinationObject').each(function(index, desObj) {
                var $desObj = $(desObj);

                $desObj.replaceWith(function() {
                    var desObjType = $desObj.attr('type');
                    var $result = $('<div/>').attr({
                                        'class': 'partialDestinationObject',
                                        'type': desObjType
                                    });

                    if (desObjType === 'image') {
                        var $img = $('<img />');
                        CopyAttributes($desObj , $img);

                        $result.append($img);
                    }

                    $desObj.find('destinationItem').each(function(index, desItem) {
                        var $desItem = $(desItem);
                        var desItemIdentifier = $desItem.attr('destIdentifier');
                        var desItemWidth = $desItem.attr('width');
                        var desItemHeight = $desItem.attr('height');
                        var desItemNumberDroppable = parseInt($desItem.attr('numberdroppable'), 10);
                        var desItemClass = '';
                        var desItemHtml = '';
                        var srcObjArr = [];

                        desItemWidth = desItemWidth === undefined ? 55 : desItemWidth;
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
                                        var srcObjHtml = $selector.find('sourceObject[srcIdentifier="' + srcObjArr[si] + '"]').outerHTML();
                                        $desItem.append(srcObjHtml);
                                        si++;
                                    }
                                }
                            }
                        }

                        // Check destination item has single or multiple answer
                        if (desItemNumberDroppable !== undefined && desItemNumberDroppable > 1) {
                            desItemClass = 'drag-drop-multiple';
                        } else {
                            desItemClass = 'drag-drop-single';
                        }

                        // Check destionation object is image or text
                        if (desObjType === 'image') {
                            var desItemTop = $desItem.attr('top');
                            var desItemLeft = $desItem.attr('left');

                            desItemHtml = '<div class="' + desItemClass + '" style="width: ' + desItemWidth + 'px; height: ' + desItemHeight + 'px; top: ' + desItemTop + 'px; left: ' + desItemLeft + 'px; position: absolute; overflow: hidden;">' + $desItem.html() + '</div>';
                        } else {
                            desItemHtml = '<div class="' + desItemClass + '" style="width: ' + desItemWidth + 'px; height: ' + desItemHeight + 'px; overflow: hidden;">' + $desItem.html() + '</div>';
                        }

                        $result.append(desItemHtml);
                    });

                    return $result;
                });
            });

            self.LoadImagesSelector($selector);

            return $selector;
        },

        _getHtmlValue: function (id) {
            return $.trim($('#' + id).html());
        },

        _setHtmlValue: function (id, val) {
            $('#' + id).html(val);
        }
    });
}(jQuery));
