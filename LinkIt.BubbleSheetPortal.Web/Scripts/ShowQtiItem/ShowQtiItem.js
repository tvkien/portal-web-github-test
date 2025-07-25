$.fn.inlineStyle = function(prop) {
    var styles = this.attr('style');
    var value;

    if (styles !== undefined) {
        styles.split(';').forEach(function(e) {
            var style = e.split(':');

            if ($.trim(style[0]) === prop) {
                value = style[1];
            }
        });
    }

    return value;
};

function copyAttributes(from, to) {
    var attrs = from.prop('attributes');
    $.each(attrs, function (index, attribute) {
        to.attr(attribute.name, attribute.value);
    });
}

function addLineBetweenElements(element1, element2, info, rootSelector) {
    if(!rootSelector)
        rootSelector = '#qtiItemView';
    var container = document.querySelector(rootSelector + ' .mainBody');
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

function VirtualQuestion(element, maxwidth) {
    this.element = element;
    this.maxwidth = maxwidth - 50;

    var self = {};
    var $el = $(this.element);
    var maxWidth = this.maxwidth;
    var reducePercent = 0;

    // Display text entry correct answer
    self.displayTextEntry = function () {
        var greaterThanOrEqual = '&#8805;';
        var lessThanOrEqual = '&#8804;';

        $el.find('.textEntryInteraction').each(function (index, textentry) {
            var $textentry = $(textentry);
            var textentryResponseId = $textentry.attr('responseidentifier');
            var $textentryResponse = $el.find('responsedeclaration[identifier="' + textentryResponseId + '"]');
            var textentryCorrect = [];
            var textentryCorrectHtml = '';
            var textentryContainer = '';

            if ($textentryResponse.length) {
                $textentryResponse.find('correctresponse > value').each(function (ind, value) {
                    var $value = $(value);
                    var startValue = '';
                    var endValue = '';
                    var startExclusivity = '';
                    var endExclusivity = '';
                    var valueCorrectAnswer = '';

                    $value.find('rangevalue').each(function () {
                        var $rangevalue = $(this);

                        if ($rangevalue.find('name').text() === 'start') {
                            startExclusivity = $rangevalue.find('exclusivity').text();
                            startValue = $rangevalue.find('value').text();
                        } else if ($rangevalue.find('name').text() === 'end') {
                            endExclusivity = $rangevalue.find('exclusivity').text();
                            endValue = $rangevalue.find('value').text();
                        }
                    });

                    var startOperator = startExclusivity == '1' ? '>' : greaterThanOrEqual;
                    var endOperator = endExclusivity == '1' ? '<' : lessThanOrEqual;

                    if (startValue != '') {
                        valueCorrectAnswer = startOperator + ' ' + startValue;
                        if (endValue != '') {
                            valueCorrectAnswer = valueCorrectAnswer + ' and ' + endOperator + ' ' + endValue;
                        }
                    } else {
                        if (endValue != '') {
                            valueCorrectAnswer = valueCorrectAnswer + ' ' + endOperator + ' ' + endValue;
                        }
                    }

                    if (valueCorrectAnswer === '') {
                        valueCorrectAnswer = $value.html();
                    }

                    textentryCorrect.push(valueCorrectAnswer);
                });

                textentryCorrectHtml = textentryCorrect.join('<br/>');

                textentryContainer = self.displayCorrectAnswerTemplate(textentryCorrectHtml);

                $(textentryContainer).insertAfter($textentry);
            }
        });
    };

    // Display draw interactive
    self.displayDrawInteractive = function() {
        $el.find('.extendedTextInteraction[drawable="true"]').each(function(index, drawinteractive) {
            var $drawinteractive = $(drawinteractive);
            var drawinteractiveWidth = $drawinteractive.attr('width');
            var drawinteractiveHeight = $drawinteractive.attr('height');

            reducePercent = 0;

            if (drawinteractiveWidth > maxWidth) {
                reducePercent = (drawinteractiveWidth - maxWidth) / drawinteractiveWidth;
                drawinteractiveWidth = maxWidth;
                drawinteractiveHeight = drawinteractiveHeight - drawinteractiveHeight * reducePercent;
            }

            $drawinteractive.css({
                'width': drawinteractiveWidth + 'px',
                'height': drawinteractiveHeight + 'px'
            });

            $drawinteractive.find('img').css({
                'width': drawinteractiveWidth + 'px',
                'height': drawinteractiveHeight + 'px'
            });
        });
    };

    // Display drag and drop standard
    self.displayDragAndDrop = function() {
        var dragdropMappingCorrect = '';
        var dragdropAnswerText = $el.find('#hdDivCorrectanswer').text().trim();

        if (!IsNullOrEmpty(dragdropAnswerText)) {
            dragdropMappingCorrect = dragdropAnswerText.split(',');
        }

        var isMatchingPair = !!$('responsedeclaration[linematching="1"]').length;

        $el.find('.destinationObject').replaceWith(function() {
            var $desObj = $(this);
            var desObjType = $desObj.attr('type');
            var desObjWidthImg;
            var desObjHeightImg;
            var $desObjHtml = $('<div/>');

            reducePercent = 0;

            $desObjHtml.attr({
                'class': 'destinationObject',
                'type': desObjType
            });

            // Check destination has image
            if (desObjType === 'image') {
                var $img = $('<img/>');
                copyAttributes($desObj, $img);
                var imgWidth = $img.attr('width');
                var imgHeight = $img.attr('height');

                desObjWidthImg = imgWidth !== undefined ? imgWidth : 250;
                desObjHeightImg = imgHeight !== undefined ? imgHeight : 100;

                if (desObjWidthImg > maxWidth) {
                    reducePercent = (desObjWidthImg - maxWidth) / desObjWidthImg;
                    desObjWidthImg = maxWidth;
                    desObjHeightImg = desObjHeightImg - desObjHeightImg * reducePercent;

                    $img.attr({
                        'width': desObjWidthImg,
                        'height': desObjHeightImg
                    });
                }

                $desObjHtml.append($img);
            }

            $desObj.find('.destinationItem').each(function(ind, desItem) {
                var $desItem = $(desItem);
                var desItemWidth = $desItem.attr('width') - $desItem.attr('width') * reducePercent;
                var desItemHeight = $desItem.attr('height') - $desItem.attr('height') * reducePercent;
                var desItemIdentifier = $desItem.attr('destIdentifier');
                var desText = $desItem.prop('innerText');
                var desItemHtml = '';

                desItemWidth = desItemWidth === undefined ? 55 : desItemWidth;
                desItemHeight = desItemHeight === undefined ? 20 : desItemHeight;

                // Mapping correct answer
                if (dragdropMappingCorrect.length) {
                    for (var i = 0, len = dragdropMappingCorrect.length; i < len; i++) {
                        var mapping = dragdropMappingCorrect[i];
                        var mappingValue = mapping.split('-');
                        var mappingDest = mappingValue[0];
                        var mappingSrc = mappingValue[1];

                        // Compare destination identifier with mapping
                        if (desItemIdentifier === mappingDest && mappingSrc !== '') {
                            var si = 0;
                            var srcObjArr = mappingSrc.split(';');

                            // Reset destination item when destin ation item have source object
                            var desContent = $desItem.html();
                            $desItem.html('');

                            while (si < srcObjArr.length) {
                                var srcObjHtml = $el.find('sourceObject[srcIdentifier="' + srcObjArr[si] + '"]').outerHTML();
                                $desItem.append((srcObjHtml || ''));
                                si++;
                            }
                            isMatchingPair && $desItem.append(desContent);
                        }
                    }
                }
                // Check destionation object is image or text
                if (desObjType === 'image') {
                    var desItemTop = $desItem.attr('top') - $desItem.attr('top') * reducePercent;
                    var desItemLeft = $desItem.attr('left') - $desItem.attr('left') * reducePercent;
                    var percentWidth, percentHeight, percentTop, percentLeft;

                    desItemTop = desItemTop === undefined ? 10 : desItemTop;
                    desItemLeft = desItemLeft === undefined ? 10 : desItemLeft;

                    percentWidth = (desItemWidth / desObjWidthImg) * 100;
                    percentHeight = (desItemHeight / desObjHeightImg) * 100;
                    percentTop = (desItemTop / desObjHeightImg) * 100;
                    percentLeft = (desItemLeft / desObjWidthImg) * 100;

                    desItemHtml = '<div class="destinationItem" style="width: ' + percentWidth + '%; height: ' + percentHeight + '%; top: ' + percentTop + '%; left: ' + percentLeft + '%; position: absolute; overflow: hidden;">' + $desItem.html() + '</div>';
                } else {
                    if (desItemWidth > maxWidth) {
                        reducePercent = (desItemWidth - maxWidth) / desItemWidth;
                        desItemWidth = maxWidth;
                        desItemHeight = desItemHeight - desItemHeight * reducePercent;
                    }
                    desItemHtml = '<div class="destinationItem" style="width: ' + desItemWidth + 'px; height: ' + desItemHeight + 'px; overflow: hidden;">' + $desItem.html() + '</div>';
                }

                $desObjHtml.append(desItemHtml);
            });

            return $desObjHtml;
        });

        $('responsedeclaration').each(function (_, responseDeclaration) {
            var qtiConfig = $(responseDeclaration);
            var qtiXml = qtiConfig.parent();
            if (qtiConfig.attr('linematching') == '1') {
              var anchorObject = qtiConfig.attr('anchorpositionobject') || 'right';
              var anchorDestination = qtiConfig.attr('anchorpositiondestination') || 'left';
              qtiXml.addClass('line-matching')
                .addClass('object-' + anchorObject)
                .addClass('destination-' + anchorDestination);
              qtiXml.find('sourceobject[type="image"]').append('<div class="anchor"></div>')
              qtiXml.find('.destinationObject[type="text"]').wrap('<div class="text-wrapper destination"></div>');
              qtiXml.find('sourceobject[type="text"]').not('.destinationItem sourceobject').wrap('<div class="text-wrapper object"></div>');
              qtiXml.find('.text-wrapper').append('<div class="anchor"></div>');

              var rootSelector = '#divQtiItemDetail';
              var sources = $(rootSelector + ' sourceobject').not('.destinationItem sourceobject');
              var anwerSources = $(rootSelector + ' .destinationItem sourceobject');
              anwerSources.each(function(_, answer) {
                var answerSrc = $(answer);
                var destination = answerSrc.closest('.destinationObject');
                var isTextDes = destination.attr('type') === 'text';
                var src = sources.filter("[srcidentifier=\"" + answerSrc.attr('srcidentifier') + "\"]");
                var anchorSrc = src.attr('type') === 'text' ? src.parent().find('.anchor')[0] : src.find('.anchor')[0];
                var anchorDes = isTextDes ? destination.parent().find('.anchor').last()[0] : answerSrc.closest('.destinationItem')[0];
                var desVal = (isTextDes ? destination.find('.destinationItem') : answerSrc.closest('.destinationItem')).attr('id');
                addLineBetweenElements(anchorSrc, anchorDes, [src.attr('srcidentifier'), desVal], rootSelector)
              })
            }
        })
    };

    // Display drag and drop numerical
    self.displayDragAndDropNumerical = function () {
        if ($el.find('assessmentitem').attr('qtischemeid') === '35') {
            var dragdropAnswerText = $el.find('#hdDivCorrectanswer').html();
            var dragdropContainer = self.displayCorrectAnswerTemplate(dragdropAnswerText);

            $el.find('.VirtualQuestion').append(dragdropContainer);
        }
    };

    self.displayDragAndDropSequence = function () {
        if ($el.find('assessmentitem').attr('qtischemeid') === '36') {
            var dragdropMappingCorrect = '';
            var dragdropAnswerText = $el.find('#hdDivCorrectanswer').text().trim();
            var $dragdropHtml = $('<div/>');

            if (!IsNullOrEmpty(dragdropAnswerText)) {
                dragdropMappingCorrect = dragdropAnswerText.split(',');
            }

            $el.find('.VirtualQuestion-xml partialsequence').each(function () {
                var $self = $(this);
                var $partialsequence = $('<partialsequence></partialsequence>');

                $self.find('.sourceItem').each(function (ind, sourceitem) {
                    var $sourceitem = $(sourceitem);
                    var sourceitemWidth = $sourceitem.attr('width');

                    $sourceitem.css('width', sourceitemWidth + 'px');
                });

                copyAttributes($self, $partialsequence);

                // Mapping correct answer
                if (dragdropMappingCorrect.length) {
                    for (var i = 0, len = dragdropMappingCorrect.length; i < len; i++) {
                        $partialsequence.append($self.find('.sourceItem[identifier="' + dragdropMappingCorrect[i] + '"]').clone(true));
                    }
                }

                $dragdropHtml.append($partialsequence);
            });

            var dragdropContainer = self.displayCorrectAnswerTemplate($dragdropHtml.html());
            $el.find('.VirtualQuestion').append(dragdropContainer);
        }
    };

    // Display text hot spot
    self.displayTextHotspot = function() {
        var $response = $el.find('assessmentitem responsedeclaration');

        $el.find('sourcetext').each(function(index, sourcetext) {
            var $sourcetext = $(sourcetext);

            if ($response.attr('partialgrading') === '1') {
                var sourcetextPoint = parseInt($sourcetext.attr('pointvalue'), 10);
                if (sourcetextPoint > 0) {
                    $sourcetext.addClass('marker-correct');
                }
            } else {
                $response.find('correctresponse').each(function (ind, correctresponse) {
                    var $correctresponse = $(correctresponse);
                    var sourcetextIdentifier = $correctresponse.attr('identifier');
                    $el.find('sourcetext[identifier="' + sourcetextIdentifier + '"]').addClass('marker-correct');
                });
            }
        });
    };

    // Display image hot spot
    self.displayImageHotspot = function() {
        var $response = $el.find('assessmentitem responsedeclaration');
        var responseArr = [];

        $el.find('.imageHotSpot').replaceWith(function () {
            var $imghotspot = $(this);
            var imghotspotWidth = $imghotspot.attr('width');
            var imghotspotHeight = $imghotspot.attr('height');
            var imghotspotSrc = $imghotspot.attr('src');
            var imghotspotId = $imghotspot.attr('responseidentifier');
            var $img = $('<img/>');

            reducePercent = 0;

            if (imghotspotId === $response.attr('identifier')) {
                var $responseCorrect = $response.find('correctresponse');

                $responseCorrect.each(function(index, rpc) {
                    var $rpc = $(rpc);

                    responseArr.push({
                        'identifier': $rpc.attr('identifier'),
                        'pointvalue': $rpc.attr('pointvalue')
                    });
                });
            }

            imghotspotWidth = imghotspotWidth === undefined ? 20 : imghotspotWidth;
            imghotspotHeight = imghotspotHeight === undefined ? 20 : imghotspotHeight;

            if (imghotspotWidth > maxWidth) {
                reducePercent = (imghotspotWidth - maxWidth) / imghotspotWidth;
                imghotspotWidth = maxWidth;
                imghotspotHeight = imghotspotHeight - (imghotspotHeight * reducePercent);
            }

            copyAttributes($imghotspot, $img);

            $img
                .css({
                    'width': imghotspotWidth + 'px',
                    'height': imghotspotHeight + 'px'
                })
                .attr('src', imghotspotSrc);

            var $result = $('<div class="imageHotspotInteraction"/>');

            $result
                .attr('responseIdentifier', $imghotspot.attr('responseIdentifier'))
                .css({
                    'width': imghotspotWidth + 'px',
                    'height': imghotspotHeight + 'px',
                    'position': 'relative'
                });

            $result.html($imghotspot.html());
            $result.prepend($img);

            $result.find('.sourceItem').replaceWith(function () {
                var $sourceItem = $(this);
                var sourceItemIdentifer = $sourceItem.attr('identifier');
                var sourceItemWidth = $sourceItem.attr('width') - ($sourceItem.attr('width') * reducePercent);
                var sourceItemHeight = $sourceItem.attr('height') - ($sourceItem.attr('height') * reducePercent);
                var sourceItemTop = $sourceItem.attr('top') - ($sourceItem.attr('top') * reducePercent);
                var sourceItemLeft = $sourceItem.attr('left') - ($sourceItem.attr('left') * reducePercent);

                var $newSourceItem = $('<span class="hotspot-item-type"><span class=" hotspot-item-value">' + $sourceItem.html() + '</span></span>');
                copyAttributes($sourceItem, $newSourceItem);

                for (var temp = 0, len = responseArr.length; temp < len; temp++) {
                    var responseArrItem = responseArr[temp];

                    if (responseArrItem.identifier === sourceItemIdentifer) {
                        $newSourceItem.addClass('checked');
                    }
                }

                $newSourceItem
                    .addClass('hotspot-item-type')
                    .css({
                        'top': sourceItemTop + 'px',
                        'left': sourceItemLeft + 'px',
                        'width': sourceItemWidth + 'px',
                        'height': sourceItemHeight + 'px',
                        'line-height': sourceItemHeight + 'px',
                        'position': 'absolute'
                    });

                return $newSourceItem;
            });

            return $result;
        });
    };

    // Display table hot spot
    self.displayTableHotspot = function() {
        $el.find('.linkit-table').each(function(index, tablelinkit) {
            var $tablelinkit = $(tablelinkit);
            var tablelinkitWidth = $tablelinkit.inlineStyle('width');
            var tablelinkitHeight = $tablelinkit.inlineStyle('height');

            if (tablelinkitWidth !== undefined) {
                tablelinkitWidth = parseInt(tablelinkitWidth.replace('px', ''), 10);
            } else {
                tablelinkitWidth = 'auto';
            }

            if (tablelinkitHeight !== undefined) {
                tablelinkitHeight = parseInt(tablelinkitHeight.replace('px', ''), 10);
            } else {
                tablelinkitHeight = 'auto';
            }

            if (tablelinkitWidth !== undefined && tablelinkitWidth < maxWidth) {
                var styleAttr = 'height: ' + tablelinkitHeight + 'px; width: ' + tablelinkitWidth + 'px !important;';
                $tablelinkit.attr('style', styleAttr);
            }
        });
    };

    // Display number line
    self.displayNumberlineHotspot = function() {
        $el.find('.numberLine').each(function(index, numberline) {
            var $numberline = $(numberline);
            var numberlineWidth = $numberline.attr('width');
            var numberlineHeight = $numberline.attr('height');

            reducePercent = 0;

            if (numberlineWidth > maxWidth) {
                reducePercent = (numberlineWidth - maxWidth) / numberlineWidth;
                numberlineWidth = maxWidth;
                numberlineHeight = numberlineHeight - numberlineHeight * reducePercent;
            }

            $numberline.css({
                'width': numberlineWidth + 'px',
                'height': numberlineHeight + 'px'
            });
        });

        $el.find('.numberLineItem').each(function (index, numberlineitem) {
            var $numberlineitem = $(numberlineitem);
            var numberlineitemTop = $numberlineitem.attr('top');
            var numberlineitemLeft = $numberlineitem.attr('left');

            $numberlineitem.css({
                'top': numberlineitemTop + '%',
                'left': numberlineitemLeft + '%'
            });
        })
    };

    // Display glossary
    self.displayGlossary = function() {
        $el.on('click', 'span.glossary', function () {
            var $glossary = $(this);
            var $glossaryMessage = $('#glossaryMessage');
            var glossaryText = $glossary.html();
            var glossaryContent = $glossary.attr('glossary')
                                            .replace(/&lt;br\/&gt;/gi, '<br/>')
                                            .replace(/&gt;/g, '>')
                                            .replace(/&lt;/g, '<');

            $glossaryMessage.find('.GlossaryMessage__title').html(glossaryText);
            $glossaryMessage.find('.GlossaryMessage__content').html(glossaryContent);

            $glossaryMessage.dialog({
                modal: true,
                width: 480,
                resizable: false,
                dialogClass: 'GlossaryPopup'
            });
        }).on({
            mouseenter: function () {
                var currentID = $(this).attr('glossary_id');
                $el.find('span.glossary[glossary_id=' + currentID + ']').addClass('glossary-hover');
            },
            mouseleave: function () {
                var currentID = $(this).attr('glossary_id');
                $el.find('span.glossary[glossary_id=' + currentID + ']').removeClass('glossary-hover');
            }
        }, 'span.glossary');
    };

    // Display correct images
    self.displayCorrectImage = function() {
        $el.find('img').not('img[data-latex]').each(function(index, img) {
            var $img = $(img);
            var imgWidth = $img.attr('width');
            var imgHeight = $img.attr('height');

            imgWidth = imgWidth ? imgWidth : 250;
            imgHeight = imgHeight ? imgHeight : 100;

            reducePercent = 0;

            if (imgWidth > maxWidth) {
                reducePercent = (imgWidth - maxWidth) / imgWidth;
                imgWidth = maxWidth;
                imgHeight = imgHeight - imgHeight * reducePercent;
            }

            $img.css({
                'width': imgWidth + 'px',
                'height': imgHeight + 'px'
            });
        });
    };

    // Display icon guidance/rationale
    self.displayGuidance = function() {
        this.displayMultipleChoiceGuidance();
        this.displayInlineChoiceGuidance();
        this.displayTextEntryGuidance();
    };

    // Display multiple choice guidance/rationale
    self.displayMultipleChoiceGuidance = function() {
        // Check question multiple choice has guidance/rationale
        var that = this;

        if ($el.find('.choiceInteraction').length) {
            $el.find('.choiceInteraction').each(function(index, choiceinteraction) {
                var $choiceinteraction = $(choiceinteraction);

                $choiceinteraction.find('li').each(function(ind, simplechoice) {
                    var $simplechoice = $(simplechoice);
                    var $simplechoiceAnswer = $simplechoice.find('div[stylename="answer"]');

                    $simplechoice.find('div[typemessage]').each(function(i, type) {
                        var $type = $(type);

                        if (that.existContentGuidanceRationale($type)) {
                            var typemessage = $type.attr('typemessage');

                            if (typemessage === 'guidance') {
                                $simplechoice.append('<span class="Icon-guidance Icon-tooltip" data-type="guidance"></span>');
                            } else if (typemessage === 'rationale') {
                                $simplechoice.append('<span class="Icon-rationale Icon-tooltip" data-type="rationale"></span>');
                            } else {
                                $simplechoice.append('<span class="Icon-guidance Icon-tooltip" data-type="guidance_rationale"></span>');
                                $simplechoice.append('<span class="Icon-rationale Icon-tooltip" data-type="guidance_rationale"></span>');
                            }
                        }
                    });
                });
            });
        }
    };

    // Display inline choice guidance/rationale
    self.displayInlineChoiceGuidance = function() {
        var that = this;

        if ($el.find('.inlineChoiceInteraction').length) {
            $el.find('.inlineChoiceInteraction').each(function(index, inlinechoiceinteraction) {
                var $inlinechoiceinteraction = $(inlinechoiceinteraction);

                $inlinechoiceinteraction.find('.inlineChoice').each(function(ind, inlinechoice) {
                    var $inlinechoice = $(inlinechoice);
                    var $inlinechoiceAnswer = $inlinechoice.find('.inlineChoiceAnswer');

                    $inlinechoice.find('div[typemessage]').each(function(i, type) {
                        var $type = $(type);
                        var typemessage = $type.attr('typemessage');

                        if (that.existContentGuidanceRationale($type)) {
                            if (typemessage === 'guidance') {
                                $inlinechoice.append('<span class="Icon-guidance Icon-tooltip" data-type="guidance"></span>');
                            } else if (typemessage === 'rationale') {
                                $inlinechoice.append('<span class="Icon-rationale Icon-tooltip" data-type="rationale"></span>');
                            } else {
                                $inlinechoice.append('<span class="Icon-guidance Icon-tooltip" data-type="guidance_rationale"></span>');
                                $inlinechoice.append('<span class="Icon-rationale Icon-tooltip" data-type="guidance_rationale"></span>');
                            }
                        }
                    });
                });
            });
        }
    };

    // Display text entry guidance/rationale
    self.displayTextEntryGuidance = function() {
        var that = this;

        if ($el.find('.textEntryInteraction').length) {
            $el.find('.textEntryInteraction').each(function(index, textentry) {
                var $textentry = $(textentry);
                var textentryResponseId = $textentry.attr('responseidentifier');

                $el.find('assessmentitem responsedeclaration').each(function(ind, response) {
                    var $response = $(response);
                    var responseId = $response.attr('identifier');

                    if (responseId === textentryResponseId) {
                        var $responserubric = $response.find('responserubric');
                        var responserubricGuidance = $responserubric.find('value[type="guidance"]').length;
                        var responserubricRationale = $responserubric.find('value[type="rationale"]').length;
                        var responserubricGR = $responserubric.find('value[type="guidance_rationale"]').length;
                        var guidanceLength = 0;
                        var rationaleLength = 0;

                        // Adding icon tooltip guidance/rationale
                        if ((responserubricGuidance > 0 || responserubricGR > 0) &&
                            !$textentry.siblings('.Icon-tooltip[data-type="guidance"]').length) {
                            $textentry.after('<span class="Icon-guidance Icon-tooltip" data-type="guidance" data-id="' + responseId + '"></span>');
                        }

                        if ((responserubricRationale > 0 || responserubricGR > 0) &&
                            !$textentry.siblings('.Icon-tooltip[data-type="rationale"]').length) {
                            $textentry.after('<span class="Icon-rationale Icon-tooltip" data-type="rationale" data-id="' + responseId + '"></span>');
                        }

                        // Remove icon if guidance/rationale is empty
                        $responserubric.find('value').each(function(vi, value) {
                            var $value = $(value);
                            var valueType = $value.attr('type');
                            var valueExist = that.existContentGuidanceRationale($value);

                            if (!valueExist && valueType === 'guidance') {
                                guidanceLength += 1;
                            }

                            if (!valueExist && valueType === 'rationale') {
                                rationaleLength += 1;
                            }

                            if (!valueExist && valueType === 'guidance_rationale') {
                                guidanceLength += 1;
                                rationaleLength += 1;
                            }
                        });

                        // Remove icon if content guidance/rationale is empty
                        if (guidanceLength === (responserubricGuidance + responserubricGR)) {
                            $textentry.siblings('.Icon-tooltip[data-type="guidance"]').remove();
                        }

                        if (rationaleLength === (responserubricRationale + responserubricGR)) {
                            $textentry.siblings('.Icon-tooltip[data-type="rationale"]').remove();
                        }
                    }
                });
            });
        }
    };

    // Check exist content guidance/rationale
    self.existContentGuidanceRationale = function(ratio) {
        var emptyGuidanceRationale = false;
        var $ratio = $(ratio);

        if ($ratio.attr('audioref') !== undefined &&
            $ratio.attr('audioref') !== '') {
            emptyGuidanceRationale = true;
            return emptyGuidanceRationale;
        }

        if($ratio.find('img, video').length > 0) {
            emptyGuidanceRationale = true;
            return emptyGuidanceRationale;
        } else if ($.trim($ratio.text()) !== '') {
            emptyGuidanceRationale = true;
            return emptyGuidanceRationale;
        }

        return emptyGuidanceRationale;
    };

    // Display video in question content
    self.displayVideo = function() {
        if ($el.find('videolinkit').length) {
            $el.find('#divQtiItemDetail')
                .find('videolinkit')
                .not('div[typemessage] videolinkit, assessmentitem responserubric value video')
                .replaceWith(function() {
                    var $videolinkit = $(this);
                    var $newVideolinkit = $('<video />');

                    $newVideolinkit.html($videolinkit.html());

                    copyAttributes($videolinkit, $newVideolinkit);

                    $newVideolinkit.removeAttr('autoplay');

                    return $newVideolinkit;
                });

            $el.find('#divQtiItemDetail')
                .find('sourcelinkit')
                .not('div[typemessage] sourcelinkit, assessmentitem responserubric value sourcelinkit')
                .replaceWith(function() {
                    var $sourcelinkit = $(this);
                    var $newSourcelinkit = $('<source />');

                    $newSourcelinkit .html($sourcelinkit.html());

                    copyAttributes($sourcelinkit, $newSourcelinkit );

                    return $newSourcelinkit;
                });
        }
    };

    // Hide Checkbox Label show Teacher Rationale or Student Guidance
    self.displayCheckboxGuidanceRationale = function () {
        var $showTeacherRationale = $('#ShowTeacherRationale');
        var $showStudentGuidance = $('#ShowStudentGuidance');

        if (!$el.find('.Icon-guidance').length) {
            $showStudentGuidance.parents('li').remove();
        }

        if (!$el.find('.Icon-rationale').length) {
            $showTeacherRationale.parents('li').remove();
        }

        if (!$el.find('.VirtualQuestion-menu-guidance label').length) {
            $el.find('.VirtualQuestion-menu-guidance').remove();
        }
    };

    // Display Virtual Question Header and Content
    self.displayQuestionHeaderAndContent = function () {
        if ($el.find('.VirtualQuestion-navbar').children().length) {
            $el.find('.VirtualQuestion-content').addClass('is-header');
        } else {
            $el.find('.VirtualQuestion-header').remove();
            $el.find('.VirtualQuestion-content').removeClass('is-header');
        }
    };

    // Template For Correct Answer
    self.displayCorrectAnswerTemplate = function (correctAnswer) {
        var badgeContainer = document.createElement('div');
        var badgeHeader = document.createElement('div');
        var badgeContent = document.createElement('div');

        badgeHeader.className = 'VirtualQuestion-badge-header';
        badgeHeader.innerHTML = 'Correct answer';

        badgeContent.className = 'VirtualQuestion-badge-content';
        badgeContent.innerHTML = correctAnswer;

        badgeContainer.className = 'VirtualQuestion-badge';
        badgeContainer.appendChild(badgeHeader);
        badgeContainer.appendChild(badgeContent);

        return badgeContainer;
    };

    // Display correct answer multiple choice variable
    self.displayMultiplechoiceVariable = function () {
        if ($el.find('assessmentitem').attr('qtischemeid') === '37') {
            var correctAnswerArr = [];

            $el.find('assessmentitem responsedeclaration correctresponse > value').each(function (ind, value) {
                var $value = $(value);
                var valuePoint = parseInt($value.text(), 10);
                var valueIdentifier = $value.attr('identifier');

                if (!isNaN(valuePoint) && valuePoint > 0) {
                    correctAnswerArr.push(valueIdentifier);
                }
            });
          var choiceInteraction = $('.choiceInteraction[variablepoints] li');

          choiceInteraction.find('.answerTextIndent')
            .addClass('answerTextCorrect')
            .append('<span class="icon-incorrect-variable"></span>');
            if (correctAnswerArr.length) {
                $el.find('.choiceInteraction[variablepoints] li').each(function (ind, li) {
                    var $li = $(li);
                    var liIdentifier = $li.attr('identifier');
                    for (var i = 0, len = correctAnswerArr.length; i < len; i++) {
                      if (liIdentifier === correctAnswerArr[i]) {
                        var answerTextIndent = $li.find('.answerTextIndent');
                        answerTextIndent.find('.icon-incorrect-variable').remove();
                        answerTextIndent
                                .addClass('answerTextCorrect')
                                .append('<span class="icon-correct-variable"></span>');
                        }
                    }
                })
            }
        }
    }

    return self;
}

function updateFontSizeAnswer () {
  var $sourceObjects = $('.VirtualQuestion .destinationItem sourceobject');
  if ($sourceObjects.length === 0) {
      return
  }
  $sourceObjects.each(function() {
      var srcIdentifier = $(this).attr('srcidentifier');
      var matchedItem = $('.VirtualQuestion sourceobject ').not('.VirtualQuestion .destinationItem sourceobject').filter('[srcidentifier="'+srcIdentifier+'"]');
      if (matchedItem.length > 0) {
          $(this).attr('style', $(this).attr('style') + '; font-size: ' + matchedItem.css('font-size') + ' !important;');
      }
  });
}

(function($, window, document, undefined) {
    'use trict';

    $(document).ready(function() {
        var virtualQuestion = new VirtualQuestion('body', 620);

        virtualQuestion.displayTextEntry();
        virtualQuestion.displayDrawInteractive();
        virtualQuestion.displayDragAndDrop();
        virtualQuestion.displayDragAndDropNumerical();
        virtualQuestion.displayDragAndDropSequence();
        virtualQuestion.displayTextHotspot();
        virtualQuestion.displayImageHotspot();
        virtualQuestion.displayTableHotspot();
        virtualQuestion.displayNumberlineHotspot();
        virtualQuestion.displayMultiplechoiceVariable();
        virtualQuestion.displayGlossary();
        virtualQuestion.displayCorrectImage();
        virtualQuestion.displayGuidance();
        virtualQuestion.displayVideo();
        virtualQuestion.displayCheckboxGuidanceRationale();
      virtualQuestion.displayQuestionHeaderAndContent();

        MathJax.Callback.call(function () {
          MathJax.Hub.Rerender();
        })
        updateFontSizeAnswer()
    });

})(jQuery, window, document);
