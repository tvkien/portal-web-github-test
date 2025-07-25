function loadContentNumberLineHotspot(el) {
    var $el = $(el);
    var $numberLine = $el.find('numberline');

    if ($numberLine.length) {
        $numberLine.each(function (ind, nl) {
            var $nl = $(nl);
            var nlWidth = $nl.attr('width');
            var nlHeight = $nl.attr('height');
            var $hotspots = $nl.find('numberlineitem');

            $hotspots.each(function (index, hs) {
                var $hs = $(hs);
                var hsId = $hs.attr('identifier');
                var hsTop = $hs.attr('top');
                var hsLeft = $hs.attr('left');
                var hsPoint = $hs.attr('pointValue');
                var hsCorrect = $hs.attr('correct');
                var hsHtml = '';

                hsHtml = '<span class="numberline-hotspot" id="' + hsId + '" data-correct="' + hsCorrect + '" data-point="' + hsPoint + '" style="top: ' + hsTop + '%; left: ' + hsLeft + '%;"></span>';

                $hs.replaceWith(hsHtml);
            });

            $nl.addClass('numberline-preview')
                .css({
                    'width': nlWidth + 'px',
                    'height': nlHeight + 'px'
                });
        });
    }
}

function loadContentImageHotSpot(selector) {
    var container = $(selector);
    var $imageHotSpot = container.find('imageHotSpot');
    if ($imageHotSpot.length == 0) {
        $imageHotSpot = container.find('.imageHotSpot');
    }
    var defaultSrc = window.location.protocol + "//" + window.location.hostname + '/TestAssignmentRegrader/GetViewReferenceImg?imgPath=';

    if ($imageHotSpot.length) {
        var $hdDivXmlContnet = $('#hdDivXmlContnet');
        var isAbsoluteGrading = false;
        var correctResponseArr = [];


        if ($hdDivXmlContnet.find('responseDeclaration').attr('absoluteGrading') === '1') {
            isAbsoluteGrading = true;
            $hdDivXmlContnet.find('correctResponse').each(function (ind, item) {
                var $item = $(item);
                correctResponseArr.push($item.attr('identifier'));
            });
        } else {
            $hdDivXmlContnet.find('correctResponse').each(function (ind, item) {
                var $item = $(item);
                correctResponseArr.push({
                    identifier: $item.attr('identifier'),
                    pointvalue: $item.attr('pointvalue')
                });
            });
        }

        $imageHotSpot.each(function (index, imgHs) {
            var $imgHs = $(imgHs);
            var imgHsId = $imgHs.attr('responseidentifier');
            var imgHsSrc = $imgHs.attr('src');
            var imgHsWidth = $imgHs.attr('width');
            var imgHsHeight = $imgHs.attr('height');
            var imgHsTexttospeech = $imgHs.attr('texttospeech');
            var imgHsHtml = '';

            var $hotspots = $imgHs.find('.sourceItem');
            var hotspotHtml = '';

            imgHsTexttospeech = imgHsTexttospeech === undefined ? '' : imgHsTexttospeech;

            $hotspots.each(function () {
                var $hotspot = $(this);
                var hotspotIdentifier = $hotspot.attr('identifier');
                var hotspotLeft = $hotspot.attr('left');
                var hotspotTop = $hotspot.attr('top');
                var hotspotWidth = $hotspot.attr('width');
                var hotspotHeight = $hotspot.attr('height');
                var hotspotType = $hotspot.attr('typehotspot');
                var hotspotValue = $hotspot.text();
                var hotspotPoint = 0;
                var hotspotCorrect = false;
                var ci = 0;
                var ciLen = correctResponseArr.length;

                if (isAbsoluteGrading) {
                    for (ci; ci < ciLen; ci++) {
                        if (hotspotIdentifier === correctResponseArr[ci]) {
                            hotspotCorrect = true;
                            break;
                        }
                    }
                } else {
                    for (ci; ci < ciLen; ci++) {
                        if (hotspotIdentifier === correctResponseArr[ci].identifier) {
                            hotspotPoint = correctResponseArr[ci].pointvalue;
                            break;
                        }
                    }
                }

                if (hotspotType === 'border-style') {
                    var hotspotShowBorder = $hotspot.attr('showborderhotspot');
                    var hotspotFill = $hotspot.attr('fillhotspot');
                    var hotspotRolloverPreview = $hotspot.attr('rolloverpreviewhotspot');

                    hotspotShowBorder = hotspotShowBorder == undefined ? false : hotspotShowBorder;
                    hotspotFill = hotspotFill == undefined ? false : hotspotFill;
                    hotspotRolloverPreview = hotspotRolloverPreview == undefined ? false : hotspotRolloverPreview;

                    hotspotHtml += '<span class="hotspot-item-type" identifier="' + hotspotIdentifier + '" style="position: absolute; width: ' + hotspotWidth + 'px; height:' + hotspotHeight + 'px; top: ' + hotspotTop + 'px; left: ' + hotspotLeft + 'px; line-height: ' + hotspotHeight + 'px;" data-type="' + hotspotType + '" data-correct="' + hotspotCorrect + '" data-point="' + hotspotPoint + '" data-show-border="' + hotspotShowBorder + '" data-fill="' + hotspotFill + '" data-rollover-preview="' + hotspotRolloverPreview + '"><span class="hotspot-item-value">' + hotspotValue + '</span></span>';
                } else {
                    hotspotHtml += '<span class="hotspot-item-type" identifier="' + hotspotIdentifier + '" style="position: absolute; width: ' + hotspotWidth + 'px; height:' + hotspotHeight + 'px; top: ' + hotspotTop + 'px; left: ' + hotspotLeft + 'px; line-height: ' + hotspotHeight + 'px;" data-type="' + hotspotType + '" data-correct="' + hotspotCorrect + '" data-point="' + hotspotPoint + '"><span class="hotspot-item-value">' + hotspotValue + '</span></span>';
                }
            });
            var newUrl = '';
            if (imgHsSrc.toLowerCase().indexOf('http') == 0) {
                newUrl = imgHsSrc;
            } else {
                newUrl = defaultSrc + imgHsSrc;
            }

            imgHsHtml += '<div id="' + imgHsId + '" class="imageHotspotInteraction" style="width: ' + imgHsWidth + 'px;height: ' + imgHsHeight + 'px" contenteditable="false">';
            imgHsHtml += '<img class="imageHotspotMarkObject" src="' + newUrl + '" style="width: ' + imgHsWidth + 'px;height: ' + imgHsHeight + 'px" texttospeech="' + imgHsTexttospeech + '"/>';
            imgHsHtml += hotspotHtml;
            imgHsHtml += '</div>';

            $imgHs.replaceWith(imgHsHtml);
        });
    }
}

/**
 * Processing load content of drag and drop
 * @param  {[type]} selector [description]
 * @return {[type]}          [description]
 */
function loadContentDragAndDrop(selector) {
    var $divQtiItemDetail = $(selector);
    var $destinationObject = $divQtiItemDetail.find('.destinationObject');
    var defaultSrc = window.location.protocol + "//" + window.location.hostname + '/TestAssignmentRegrader/GetViewReferenceImg?imgPath=';
    var desObjHtml = '';

    if ($destinationObject.length) {
        $destinationObject.each(function (ind, desObj) {
            var $desObj = $(desObj);
            var desObjType = $desObj.attr('type');
            var $destinationItem = $desObj.find('.destinationItem');
            var desItemHtml = '';
            var desItemWidth, desItemHeight, desItemIdentifier;

            if (desObjType === 'image') {
                var desObjWidth = $desObj.attr('width');
                var desObjHeight = $desObj.attr('height');
                var desObjSrc = $desObj.attr('src');
                var desObjFloat = $desObj.attr('float');
                desObjHtml = '';

                $destinationItem.each(function (i, desItem) {
                    var $desItem = $(desItem);
                    var desItemTop = $desItem.attr('top');
                    var desItemLeft = $desItem.attr('left');
                    desItemWidth = $desItem.attr('width');
                    desItemHeight = $desItem.attr('height');
                    desItemIdentifier = $desItem.attr('destidentifier');

                    desItemHtml += '<div class="hotSpot" destidentifier=" ' + desItemIdentifier + ' " style="display: block; position: absolute; top: ' + desItemTop + 'px; left: ' + desItemLeft + 'px; width: ' + desItemWidth + 'px; height: ' + desItemHeight + 'px;">';
                    desItemHtml += desItemIdentifier;
                    desItemHtml += '</div>';
                });
                var newSrc = '';
                if (desObjSrc.toLowerCase().indexOf('http') == 0) {
                    newSrc = desObjSrc;
                } else {
                    newSrc = defaultSrc + desObjSrc;
                }
                desObjHtml += '<div class="partialDestinationObject partialAddDestinationImage" style="width: ' + desObjWidth + 'px; height: ' + desObjHeight + 'px; float:' + desObjFloat + '" type="image">';
                desObjHtml += '<img src="' + newSrc + '"/>';
                desObjHtml += desItemHtml;
                desObjHtml += '</div>';
            } else {
                desObjHtml = '';

                $destinationItem.each(function (i, desItem) {
                    var $desItem = $(desItem);
                    desItemWidth = $desItem.attr('width');
                    desItemHeight = $desItem.attr('height');
                    desItemIdentifier = $desItem.attr('destidentifier');
                    desItemText = $desItem.html();
                });

                desObjHtml += '<span class="partialDestinationObject partialAddDestinationText" destidentifier="' + desItemIdentifier + '" type="text" style="width: ' + desItemWidth + 'px; height: ' + desItemHeight + 'px;">';
                desObjHtml += desItemText;
                desObjHtml += '</span>';
            }
            if (selector === "#divEditQtiItem") {
              var temp = $(desObjHtml).css('width');
              $(selector).css('width', temp);
            }
            $desObj.replaceWith(desObjHtml);

        });
    }
    //sourceobject
    var $sourceObject = $divQtiItemDetail.find('sourceobject');
    if ($sourceObject.length) {
        $sourceObject.each(function(i, srcObj) {
            var $srcObj = $(srcObj);
            var sourceObjType = $srcObj.attr('type');
            if (sourceObjType === 'image') {
                $srcObj.each(function(i, srcItem) {
                    var $srcItem = $(srcItem);
                    var srcObjFloat = $srcItem.attr('float');
                    $srcItem.attr('style', 'float:' + srcObjFloat);
                });
            }
        });
    }

    var $lineMatching = $('#hdDivXmlContnet responsedeclaration[linematching="1"]');
    if ($lineMatching.attr('linematching') == 1) {
      $divQtiItemDetail.find('sourceobject[type="image"]').append('<div class="anchor"></div>')
      $divQtiItemDetail.find('.partialAddDestinationText').wrap('<div class="text-wrapper destination"></div>');
      $divQtiItemDetail.find('sourceobject[type="text"]').wrap('<div class="text-wrapper object"></div>');
      $divQtiItemDetail.find('.text-wrapper').append('<div class="anchor"></div>')
      $divQtiItemDetail.addClass("line-matching")
        .addClass("object-" + ($lineMatching.attr('anchorPositionObject') || "right"))
        .addClass("destination-" + ($lineMatching.attr('anchorPositionDestination') || "left"))
    } else {
      $divQtiItemDetail.removeClass("line-matching object-left object-right object-top object-bottom destination-left destination-right destination-top destination-bottom")
    }
}

/**
 * Re calculator width sequence/order
 * @param  {[type]} selector [description]
 * @return {[type]}          [description]
 */
function calculatorSequenceWidth(selector) {
    var $partialSequence = $(selector);
        $partialSequence.each(function () {
            var $currentSourceitem = $(this).find(".sourceItem");
            if ($currentSourceitem.length == 0) {
                $currentSourceitem = $(this).find("sourceItem");
            }
            $currentSourceitem.width($currentSourceitem.attr("width"));
        });
}

/**
 * Load content glossary
 * @param  {[type]} element  [description]
 * @param  {[type]} glossary [description]
 * @return {[type]}          [description]
 */
function loadContentGlossary(element, glossary) {
    var $element = $(element);

    $element.on('click', 'span.glossary', function (e) {
        var $glossary = $(glossary);
        var $self = $(this);
        var glossaryText = $self.html();
        var glossaryContent = $self.attr('glossary')
                                        .replace(/&lt;br\/&gt;/gi, '<br/>')
                                        .replace(/&gt;/g, '>')
                                        .replace(/&lt;/g, '<');

        $glossary.find('.glossary_text').html(glossaryText);
        $glossary.find('.glossary_define').html(glossaryContent);

        var zIndex = parseInt($self.parents('.ui-dialog').css('z-index'), 10);
        var $doc = $(document);
        var $uiOverlay = $('<div/>');

        if (isNaN(zIndex)) {
            zIndex = 1000;
        }

        $uiOverlay
            .addClass('ui-widget-overlay')
            .css({
                'width': $doc.width() + 'px',
                'height': $doc.height() + 'px',
                'z-index': zIndex + 1
            });

        $('body').prepend($uiOverlay);

        $glossary.dialog({
            modal: false,
            width: 480,
            resizable: false,
            open: function (dialog) {
                $glossary.prev().css('top', '37px');
            },
            close: function () {
                $('.ui-widget-overlay:first').remove();
            }
        });

    }).on({
        mouseenter: function () {
            var currentID = $(this).attr('glossary_id');
            $element.find('span.glossary[glossary_id=' + currentID + ']').addClass('glossary-hover');
        },
        mouseleave: function () {
            var currentID = $(this).attr('glossary_id');
            $element.find('span.glossary[glossary_id=' + currentID + ']').removeClass('glossary-hover');
        }
    }, 'span.glossary');
}

function AdjustQtiItemDetail() {
  //sometime the answer has special xmlcontent structure //LNKT-6197

  $("#divQtiItemDetail .answerText").each(function (index, value) {
      AdjustQtiItemAnswers(value);
  });

  $(".itembody li[identifier]").each(function (index, value) {
      AdjustQtiItemAnswers(value);
  });
  $(".itemBody li[identifier]").each(function (index, value) {
      AdjustQtiItemAnswers1(value);
  });

}

function AdjustQtiItemAnswers(item) {
  var divAnswerText = item;
  //add the image checkbox for each answer
  var identifier = divAnswerText.attributes["identifier"];
  var src = '';
  var responseidentifier = $(item.parentNode).attr('responseidentifier');
  if (responseidentifier == null) {
      responseidentifier = '';
  }
  if (identifier != null) {
      var iconCheck = '@Url.Content("~/Content/themes/Constellation/images/icons/fugue/square-check-16.svg")';
      var iconUnCheck = '@Url.Content("~/Content/themes/Constellation/images/icons/fugue/square-uncheck-16.svg")';
      //var idx = correctAnswer.indexOf('-' + identifier.nodeValue + '-,');
      var idx = correctAnswer.indexOf('-' + responseidentifier + ':' + identifier.nodeValue + '-,');
      var img = '';
      if (idx >= 0) {
          img = '<img src="' + iconCheck + '" width="18" height="22" style="margin-right: 8px !important">';
          src = iconCheck;
      } else {
          img = '<img src="' + iconUnCheck + '" width="18" height="22" style="margin-right: 8px !important">';
          src = iconUnCheck;
      }

      src = src.trim();
      if (src.length > 1) {
          src = src.substring(1, src.length); //remove the firt character '/'
      }
      var divAnswer = $(divAnswerText).children().first();
      if (divAnswer != null) {
          //check if the checkbox is existing or not
          var html = divAnswer.html();
          if (html == null) {
              html = '';
          }

          var $html = $('<div/>').html(html);

          var imgCheckbox = $html.filter('img[src="' + src + '"]').first();

          if (imgCheckbox.length == 0) {
              //sometime src starts with '/', check again to make sure
              imgCheckbox = $html.filter('img[src="/' + src + '"]').first();
              if (imgCheckbox.length == 0) {
                  //sometime filter does not work when html is wrapped by tag p
                  if (html.indexOf(src) < 0) {
                      html = img + html;
                      divAnswer.html(html);
                  }
              }

          }
      }
  }
}
function AdjustQtiItemAnswers1(item) {
  var divAnswerText = item;
  //add the image checkbox for each answer
  var identifier = divAnswerText.attributes["identifier"];
  var src = '';
  var responseidentifier = $(item.parentNode).attr('responseidentifier');
  if (responseidentifier == null) {
      responseidentifier = '';
  }
  if (identifier != null) {
      var iconCheck = '@Url.Content("~/Content/themes/Constellation/images/icons/fugue/square-check-16.svg")';
      var iconUnCheck = '@Url.Content("~/Content/themes/Constellation/images/icons/fugue/square-uncheck-16.svg")';
      //var idx = correctAnswer.indexOf('-' + identifier.nodeValue + '-,');
      var idx = correctAnswer.indexOf('-' + responseidentifier + ':' + identifier.nodeValue + '-,');
      var img = '';
      if (idx >= 0) {
          img = '<img src="' + iconCheck + '" width="18" height="22" style="margin-right: 8px !important">';
          src = iconCheck;
      } else {
          img = '<img src="' + iconUnCheck + '" width="18" height="22" style="margin-right: 8px !important">';
          src = iconUnCheck;
      }

      src = src.trim();
      if (src.length > 1) {
          src = src.substring(1, src.length); //remove the firt character '/'
      }
      var html = $(divAnswerText).html();
      if (html == null) {
          html = '';
      }

      var $html = $('<div/>').html(html);

      var imgCheckbox = $html.filter('img[src="' + src + '"]').first();

      if (imgCheckbox.length == 0) {
          //sometime src starts with '/', check again to make sure
          imgCheckbox = $html.filter('img[src="/' + src + '"]').first();
          if (imgCheckbox.length == 0) {
              //sometime filter does not work when html is wrapped by tag p
              if (html.indexOf(src) < 0) {
                  html = '<div style="float:left;margin-top:5px">' + img + '</div>' + html;
                  $(divAnswerText).html(html);
              }
          }

      }
  }
}