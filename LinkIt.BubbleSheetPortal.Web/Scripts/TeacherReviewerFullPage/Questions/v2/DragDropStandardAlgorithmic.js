(function ($) {
    function addLineBetweenElements(element1, element2, info, rootSelector) {
        if(!rootSelector)
            rootSelector = '#qtiItemView';
        var container = typeof rootSelector === 'string' ? document.querySelector(rootSelector + ' .mainBody') : rootSelector.querySelector('.mainBody');
        // Get the bounding rectangles of both elements
        var rect1 = element1.getBoundingClientRect();
        var rect2 = element2.getBoundingClientRect();
        var containerRect = container.getBoundingClientRect();
        // Calculate the center points of each element relative to the container
        var x1 = rect1.left - containerRect.left + rect1.width / 2;
        var y1 = rect1.top - containerRect.top + rect1.height / 2;
        var x2 = rect2.left - containerRect.left + rect2.width / 2;
        var y2 = rect2.top - containerRect.top + rect2.height / 2;
        // Calculate the distance between the two elements
        var distance = Math.sqrt(Math.pow(x2 - x1, 2) + Math.pow(y2 - y1, 2));
        // Calculate the angle between the two points
        var angle = Math.atan2(y2 - y1, x2 - x1) * (180 / Math.PI);
        // Create the line element
        var line = document.createElement('div');
        line.className = 'line';
        line.style.width = distance + 'px';
        line.style.transform = 'rotate(' + angle + 'deg)';
        line.style.left = x1 + 'px';
        line.style.top = y1 + 'px';
        line.setAttribute('from', info.from);
        line.setAttribute('to', info.to);
        // Append the line to the container
        container.append(line);
    }
    function displayLineMatching() {
        $('responsedeclaration').not('.line-matching responsedeclaration').each(function (_, responseDeclaration) {
            var qtiConfig = $(responseDeclaration);
            var qtiXml = qtiConfig.parent();
            if (qtiConfig.attr('linematching') == '1') {
              var anchorObject = qtiConfig.attr('anchorpositionobject') || 'right';
              var anchorDestination = qtiConfig.attr('anchorpositiondestination') || 'left';
              qtiXml.addClass('line-matching')
                .addClass('object-' + anchorObject)
                .addClass('destination-' + anchorDestination);
              qtiXml.find('sourceobject[type="image"]').append('<div class="anchor"></div>')
              qtiXml.find('.partialDestinationObject[type="text"]').wrap('<div class="text-wrapper destination"></div>');
              qtiXml.find('sourceobject[type="text"]').not('.drag-drop-single sourceobject').not('.drag-drop-multiple sourceobject').wrap('<div class="text-wrapper object"></div>');
              qtiXml.find('.text-wrapper').append('<div class="anchor"></div>');
              qtiXml.find('.partialDestinationObject[type="text"] .drag-drop-single,.partialDestinationObject[type="text"] .drag-drop-multiple').html(function(_, innerHtml) {
                if ($(this).find('sourceobject').length) {
                  return innerHtml + ($(this).attr('desvalue') || '')
                }
                return innerHtml;
              })
              var sources = qtiXml.find('sourceobject').not('.drag-drop-single sourceobject').not('.drag-drop-multiple sourceobject');
              var anwerSources = qtiXml.find('.drag-drop-single sourceobject,.drag-drop-multiple sourceobject');
              anwerSources.each(function (_, answer) {
                var answerSrc = $(answer);
                var destination = answerSrc.closest('.partialDestinationObject');
                var isTextDes = destination.attr('type') === 'text';
                var src = sources.filter("[srcidentifier=\"" + answerSrc.attr('srcidentifier') + "\"]");
                var anchorSrc = src.attr('type') === 'text' ? src.parent().find('.anchor')[0] : src.find('.anchor')[0];
                var anchorDes = isTextDes ? destination.parent().find('.anchor').last()[0] : answerSrc.closest('.drag-drop-single,.drag-drop-multiple')[0];
                var desVal = (isTextDes ? destination.find('.drag-drop-single,.drag-drop-multiple') : answerSrc.closest('.drag-drop-single,.drag-drop-multiple')).attr('id');
                addLineBetweenElements(anchorSrc, anchorDes, [src.attr('srcidentifier'), desVal], qtiXml[0])
              })
            }
        })
    }

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

            var responseDeclaration = $(question.XmlContent()).find('responseDeclaration').hide();
            responseDeclaration[0].innerHTML = '';
            // Display answer of student
            answerHtml = that.GetContentDragDrop(self, $(question.ItemBody()), mappingsAnswer);
            // Display correct answer
            correctHtml = that.GetContentDragDrop(self, $(question.ItemBody()), mappingsCorrect);

            questionDetails = '<div>' + responseDeclaration.prop('outerHTML') + answerHtml.outerHTML() + '</div>';
            question.CorrectAnswerHTML = '<div>' + responseDeclaration.prop('outerHTML') + correctHtml.outerHTML() + '</div>';

            if (question.AlgorithmicCorrectAnswers() != null && question.AlgorithmicCorrectAnswers().length) {
                questionDetails += '<br> <div class="btn-show-all-correct-answer big-button" onClick="ShowAllCorrectAnswers()">Show all correct answers</div>';
            }
            if (options.PostProcessQuestionDetails !== null &&
                typeof (options.PostProcessQuestionDetails) == "function") {
                questionDetails = options.PostProcessQuestionDetails(questionDetails);
            }

            self.Respones(questionDetails);
            displayLineMatching();
        },

        ShowCorrectAnswer: function (self, question) {
            var $contentCorrect = $(question.CorrectAnswerHTML);
            $contentCorrect.find('.highlighted').removeAttr('style').removeClass();
            Reviewer.popupAlertMessage($contentCorrect.outerHTML(), 'ui-popup-fullpage', 700, 500, false);
            self.ReviewerWidget.ReviewerWidget('LoadImages', $('.ui-popup-fullpage'));
            displayLineMatching();
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
            Reviewer.popupAlertMessage(questionDetails, 'ui-popup-fullpage ui-popup-algorithmic-correct-answer', 800, 500, false);
            Reviewer.createTabWidget('.ui-popup-fullpage.ui-popup-algorithmic-correct-answer', algorithmicPoints);
            displayLineMatching();
            $('.ui-popup-fullpage.ui-popup-algorithmic-correct-answer .box-title').on('click', function() {
                displayLineMatching();
            })
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
                        var desvalue = $desItem.prop('innerText');
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
                            desItemHtml = '<div desvalue="' + ($desObj.attr('type') === 'text' ? desvalue : '') + '" class="' + desItemClass + '" style="width: ' + desItemWidth + 'px; height: ' + desItemHeight + 'px; overflow: hidden;">' + $desItem.html() + '</div>';
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
