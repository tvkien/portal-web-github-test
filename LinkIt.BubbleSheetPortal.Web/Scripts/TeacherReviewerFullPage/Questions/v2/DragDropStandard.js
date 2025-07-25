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

    $.widget('jquery.DragDropStandard', {
        options: {
            DragDropStandardUtil: null,
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
            if (!options.DragDropStandardUtil.IsNullOrEmpty(question.Answer())) {
                answerText = question.Answer().AnswerText();
            }

            // Mapping answer
            if (!options.DragDropStandardUtil.IsNullOrEmpty(answerText)) {
                mappingsAnswer = answerText.split(',');
            }

            // Update correct answer
            if (!options.DragDropStandardUtil.IsNullOrEmpty(question.CorrectAnswer())) {
                mappingsCorrect = question.CorrectAnswer().split(',');
            }

            var responseDeclaration = $(question.XmlContent()).find('responseDeclaration').hide();
            responseDeclaration[0].innerHTML = '';
            // Display answer of student
            answerHtml = that.GetContentDragDrop(self, $(question.ItemBody()), mappingsAnswer);
            answerHtml = that.updateFontSizeAnswer(answerHtml);
            // Display correct answer
            correctHtml = that.GetContentDragDrop(self, $(question.ItemBody()), mappingsCorrect);

            questionDetails = '<div>' + responseDeclaration.prop('outerHTML') + answerHtml.outerHTML() + '</div>';
            question.CorrectAnswerHTML = '<div>' + responseDeclaration.prop('outerHTML') + correctHtml.outerHTML() + '</div>';

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

            displayLineMatching();
        },

        ShowCorrectAnswer: function (self, question) {
            var $contentCorrect = $(question.CorrectAnswerHTML);
            $contentCorrect = this.updateFontSizeAnswer($contentCorrect);
            $contentCorrect.find('.highlighted').removeAttr('style').removeClass();
            Reviewer.popupAlertMessage($contentCorrect.outerHTML(), 'ui-popup-fullpage', 700, 500, false);
            self.ReviewerWidget.ReviewerWidget('LoadImages', $('.ui-popup-fullpage'));
            displayLineMatching();
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
                        var desvalue = $desItem.prop('innerHTML');
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
                                    // $desItem.html('');
                                    
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

            //self.LoadImagesSelector($selector);

            return $selector;
        },

        _getHtmlValue: function (id) {
            return $.trim($('#' + id).html());
        },

        _setHtmlValue: function (id, val) {
            $('#' + id).html(val);
        },

        updateFontSizeAnswer: function (selector) {
          var $sourceObjects = selector.find('.partialDestinationObject sourceobject')
          if ($sourceObjects.length === 0) {
            return selector;
          }
          $sourceObjects.each(function() {
            var srcIdentifier = $(this).attr('srcidentifier');
            var matchedItem = $('sourceobject').not('.partialDestinationObject sourceobject').filter('[srcidentifier="'+srcIdentifier+'"]');
            if (matchedItem.length > 0) {
              $(this).attr('style', $(this).attr('style') + '; font-size: ' + matchedItem.css('font-size') + ' !important;');
            }
          });
          return selector
        }
    });
}(jQuery));
