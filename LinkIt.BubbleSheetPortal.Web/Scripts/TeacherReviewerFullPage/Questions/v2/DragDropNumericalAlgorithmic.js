(function ($) {
    $.widget('jquery.DragDropNumericalAlgorithmic', {
        options: {
            DragDropNumericalAlgorithmicUtil: null,
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
            if (!options.DragDropNumericalAlgorithmicUtil.IsNullOrEmpty(answerOfStudentForSelectedQuestion)) {
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

            if (!options.DragDropNumericalAlgorithmicUtil.IsNullOrEmpty(answerText)) {
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
                                        var srcObjHtml = tree.find('sourceObject[srcIdentifier="' + srcObjArr[si] + '"], sourceobject[srcIdentifier="' + srcObjArr[si] + '"]').outerHTML();
                                        $desItem.append(srcObjHtml);
                                        si++;
                                    }
                                }
                            }
                        }

                        desItemHtml = '<div style="width: ' + desItemWidth + 'px; height: ' + desItemHeight + 'px; overflow: hidden;">' + $desItem.html() + '</div>';

                        $result.append(desItemHtml);
                    });

                    return $result;
                });
            });

            var questionDetails = tree.outerHTML();

            if (question.AlgorithmicCorrectAnswers() != null && question.AlgorithmicCorrectAnswers().length) {
                questionDetails += '<br> <div class="btn-show-all-correct-answer big-button" onClick="ShowAllCorrectAnswers()">Show all correct answers</div>';
            }

            if (options.PostProcessQuestionDetails !== null &&
                typeof (options.PostProcessQuestionDetails) == "function") {
                questionDetails = options.PostProcessQuestionDetails(questionDetails);
            }

            self.Respones(questionDetails);
            self.SectionInstruction(question.SectionInstruction());
        },

        ShowAllCorrectAnswers: function (self, question) {
            var that = this;
            var algorithmicCorrectAnswers = $('<div/>');
            var algorithmicPoints = [];

            ko.utils.arrayForEach(question.AlgorithmicCorrectAnswers(), function (item) {
                var $tree = $('<div/>');
                var $questionClone = $(question.ItemBody()).clone(true);
                var mappingCorrects = [];
                var correctHtml = '';
                var correctAnswerDnd = '';
                
                $tree.addClass('box-answer');

                if (typeof item.Amount === 'function' && item.Amount() > 0) {
                    mappingCorrects = item.ConditionValue();
                    correctHtml = that.GetContentDragDropNumerical(self, $(question.ItemBody()), []);
                    var elAtleast = Reviewer.getAtleast(item.Amount(), item.PointsEarned(), question.QTIItemSchemaID());
                    $tree.append(elAtleast.outerHTML);
                    correctHtml.appendTo($tree);

                    if (mappingCorrects.length) {
                        correctAnswerDnd = Reviewer.getCorrectAnswerDnd($questionClone, mappingCorrects);
                        $tree.append(correctAnswerDnd.outerHTML);
                    }
                    
                    $tree.appendTo(algorithmicCorrectAnswers);
                    algorithmicPoints.push(item.PointsEarned);
                } else {
                    mappingCorrects = item.ConditionValue();
                    correctHtml = that.GetContentDragDropNumerical(self, $(question.ItemBody()), []);
                    correctHtml.appendTo($tree);

                    if (mappingCorrects.length) {
                        correctAnswerDnd = Reviewer.getCorrectAnswerDnd($questionClone, mappingCorrects);
                        $tree.append(correctAnswerDnd.outerHTML);
                    }

                    $tree.appendTo(algorithmicCorrectAnswers);
                    algorithmicPoints.push(item.PointsEarned);
                }
            });

            var questionDetails = algorithmicCorrectAnswers.outerHTML();
            Reviewer.popupAlertMessage(questionDetails, 'ui-popup-fullpage ui-popup-algorithmic-correct-answer', 700, 500, false);
            Reviewer.createTabWidget('.ui-popup-fullpage.ui-popup-algorithmic-correct-answer', algorithmicPoints);
        },

        GetContentDragDropNumerical: function (self, selector, mappings) {
            var $selector = $(selector);

            $selector.find('destinationObject, destinationobject').each(function (index, desObj) {
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
                                        var srcObjHtml = $selector.find('sourceObject[srcIdentifier="' + srcObjArr[si] + '"], sourceobject[srcIdentifier="' + srcObjArr[si] + '"]').outerHTML();
                                        $desItem.append(srcObjHtml);
                                        si++;
                                    }
                                }
                            }
                        }

                        desItemHtml = '<div style="width: ' + desItemWidth + 'px; height: ' + desItemHeight + 'px; overflow: hidden;">' + $desItem.html() + '</div>';

                        $result.append(desItemHtml);
                    });

                    return $result;
                });
            });

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
