(function ($) {
    $.widget('jquery.DragDropStandardAlgorithmic', {
        options: {
            DragDropStandardAlgorithmicUtil: null,
            PostProcessQuestionDetails: null,
            Self: null
        },

        Display: function (question) {
            var that = this;
            var options = that.options;
            var self = options.Self;

            // answerText = 'DEST_2-SRC_2;SRC_3;SRC4,DEST_1-SRC_1,DEST_3-SRC_3;SRC_2;,DEST_5-SRC_3';
            var answerText = '';
            var mappingsAnswer = '';
            var mappingsCorrect = '';
            var answerHtml = '';
            var correctHtml = '';
            var questionDetails = '';

            // Update answer text
            if (!options.DragDropStandardAlgorithmicUtil.IsNullOrEmpty(question.Answer())) {
                answerText = question.Answer().AnswerText();
            }

            // Mapping answer
            if (!options.DragDropStandardAlgorithmicUtil.IsNullOrEmpty(answerText)) {
                mappingsAnswer = answerText.split(',');
            }

            // Update correct answer
            if (!options.DragDropStandardAlgorithmicUtil.IsNullOrEmpty(question.CorrectAnswer())) {
                mappingsCorrect = question.CorrectAnswer().split(',');
            }

            // Display answer of student
            answerHtml = that.GetContentDragDrop(self, $(question.ItemBody()), mappingsAnswer);
            // Display correct answer
            correctHtml = that.GetContentDragDrop(self, $(question.ItemBody()), mappingsCorrect);

            questionDetails = answerHtml.outerHTML();
            question.CorrectAnswerHTML = correctHtml.outerHTML();
            
            if (question.AlgorithmicCorrectAnswers() != null && question.AlgorithmicCorrectAnswers().length) {
                questionDetails += '<br> <div class="btn-show-all-correct-answer big-button" onClick="ShowAllCorrectAnswers()">Show all correct answers</div>';
            }
            if (options.PostProcessQuestionDetails !== null &&
                typeof (options.PostProcessQuestionDetails) == "function") {
                questionDetails = options.PostProcessQuestionDetails(questionDetails);
            }

            self.Respones(questionDetails);
        },

        ShowCorrectAnswer: function (self, question) {
            Reviewer.popupAlertMessage(question.CorrectAnswerHTML, 'ui-popup-fullpage', 700, 500);
            self.ReviewerWidget.ReviewerWidget('LoadImages', $('.ui-popup-fullpage'));
        },
        ShowAllCorrectAnswers: function (self, question) {

            var that = this;
            var algorithmicCorrectAnswers = $('<div></div>');
            var algorithmicPoints = [];
            
            ko.utils.arrayForEach(question.AlgorithmicCorrectAnswers(), function (item) {
                var $tree = $('<div/>');
                var $questionClone = $(question.ItemBody()).clone(true);
                var mappingCorrects = [];
                var correctHtml = '';
                var correctAnswerDnd = '';

                $tree.addClass('box-answer');

                if (typeof item.Amount === 'function' && item.Amount() > 0) {
                    var elAtleast = '';

                    mappingCorrects = item.ConditionValue();
                    correctHtml = that.GetContentDragDrop(self, $(question.ItemBody()), []);
                    elAtleast = Reviewer.getAtleast(item.Amount(), item.PointsEarned(), question.QTIItemSchemaID());
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
                    correctHtml = that.GetContentDragDrop(self, $(question.ItemBody()), []);
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
            Reviewer.popupAlertMessage(questionDetails, 'ui-popup-fullpage ui-popup-algorithmic-correct-answer', 700, 500);
            Reviewer.createTabWidget('.ui-popup-fullpage.ui-popup-algorithmic-correct-answer', algorithmicPoints);
        },

        GetContentDragDrop: function (self, selector, mappings) {
            var $selector = $(selector);

            $selector.find('destinationObject').each(function (index, desObj) {
                var $desObj = $(desObj);

                $desObj.replaceWith(function () {
                    var desObjType = $desObj.attr('type');
                    var $result = $('<div/>').attr({
                        'class': 'partialDestinationObject',
                        'type': desObjType
                    });

                    if (desObjType === 'image') {
                        var $img = $('<img />');
                        CopyAttributes($desObj, $img);

                        $result.append($img);
                    }

                    $desObj.find('destinationItem').each(function (index, desItem) {
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
