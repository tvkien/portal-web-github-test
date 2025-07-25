/*
*
* Function when import item type: Text Hot Spot
*
*/
function importTextHotSpot(xmlContent, pointsValueAlgorithmic) {
    $(xmlContent).find("textHotSpot").each(function () {
        var resId = $(this).attr("responseIdentifier");
        var textHPResult = {
                type: 'textHotSpot',
                responseIdentifier: resId,
                correctResponse: [],
                maxSelected: $(this).attr('maxSelected'),
                responseDeclaration: {
                    absoluteGrading: '1',
                    partialGrading: '0',
                    algorithmicGrading: '0',
                    pointsValue: '0'
                },
                source: []
            };

        //Extract for correct text hot spot
        $(xmlContent).find("responseDeclaration").each(function () {
            if (resId == $(this).attr("identifier")) {

                textHPResult.responseDeclaration.absoluteGrading = $(this).attr("absoluteGrading");
                textHPResult.responseDeclaration.partialGrading = $(this).attr("partialGrading");
                textHPResult.responseDeclaration.algorithmicGrading = $(this).attr('algorithmicGrading') === '1' ? '1' : '0';
                textHPResult.responseDeclaration.pointsValue = pointsValueAlgorithmic != null ? pointsValueAlgorithmic : $(this).attr("pointsValue");

                $(this).find("correctResponse").each(function () {
                    textHPResult.correctResponse.push({
                        identifier: $(this).attr("identifier"),
                        pointValue: $(this).attr("pointValue")
                    });
                });
            }
        });

        //Extract for sourceText Hot Spot
        $(xmlContent).find("sourceText").each(function () {
            var iden = $(this).attr("identifier");
            var pointVal = $(this).attr("pointValue");
            var currentSourceText = '<span class="marker-linkit" hs_id="' + iden + '">' + $(this).html() + '</span>';

            //This is check only show correct answer( Absolute grading)
            if (textHPResult.responseDeclaration.absoluteGrading == "1") {
                for (var k = 0; k < textHPResult.correctResponse.length; k++) {
                    if (textHPResult.correctResponse[k].identifier == iden) {
                        currentSourceText = '<span class="marker-linkit marker-correct" hs_id="' + iden + '">' + $(this).html() + '</span>';
                        break;
                    }
                }
            } else {
                //This is for partial grading
                if (parseInt(pointVal) > 0) {
                    currentSourceText = '<span class="marker-linkit marker-correct" hs_id="' + iden + '">' + $(this).html() + '</span>';
                }
            }

            //This is check to make sure current identify is exist or not in current textHPResult.source
            var hasExistID = false;
            for (var i = 0; i < textHPResult.source.length; i++) {
                if (textHPResult.source[i].identifier == $(this).attr("identifier")) {
                    hasExistID = true;
                }
            }

            //Only push new identify to textHPResult.source
            if (!hasExistID) {
                textHPResult.source.push({ identifier: $(this).attr("identifier"), pointValue: pointVal });
            }

            //Replace with new text hot spot
            xmlContent = xmlContent.replace($(this).prop("outerHTML").replace('<?XML:NAMESPACE PREFIX = "[default] http://www.imsglobal.org/xsd/imsqti_v2p0" NS = "http://www.imsglobal.org/xsd/imsqti_v2p0" />', ''), currentSourceText);
        });

        iResult.push(textHPResult);
    });

    return xmlContent;
}

/**
 * Processing import xml content of partial credit
 * @param  {[type]} xmlContent [description]
 * @return {[type]}            [description]
 */
function xmlImportPartialCredit(xmlContent, pointsValueAlgorithmic) {
    var $xmlContent = $(xmlContent);

    $xmlContent.find("partialCredit").each(function (index, partialCredit) {
        var $partialCredit = $(partialCredit);
        var resId = $partialCredit.attr("responseIdentifier");
        var partialID = $partialCredit.attr("partialID");
        var destination = [];
        var source = [];

        // Import Partial source to editor
        $xmlContent.find("sourceObject").each(function (index, sourceObject) {
            var $sourceObj = $(sourceObject);
            var srcId = $sourceObj.attr("srcIdentifier");
            var type = $sourceObj.attr("type");
            var value = $sourceObj.html();
            var limit = $sourceObj.data('limit');
            var strObj = $sourceObj.prop("outerHTML");

            //Fix for IE9
            if ($.browser.msie && parseInt($.browser.version, 10) < 10) {
                strObj = xmlContentForIE(strObj);
            }

            if (type === "text") {
                var txtHeight = $sourceObj.css('height');
                var txtWidth = $sourceObj.css('width');

                txtWidth = txtWidth == '0px' ? 'auto' : txtWidth;
                txtHeight = txtHeight !== '0px' ? txtHeight : '20px';

                if (iSchemeID === '35') {
                    xmlContent = xmlContent.replace(strObj, '<span class="partialSourceObject partialAddSourceNumerical" contenteditable="false" unselectable="on" style="width: ' + txtWidth + '; height: ' + txtHeight + ';" srcidentifier="' + srcId + '" partialid="' + partialID + '" data-limit="' + limit + '"><img class="cke_reset cke_widget_mask partialDragDropNumericalSourceMark" data-cke-saved-src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D">' + value + '</span>');
                } else {
                  xmlContent = xmlContent.replace(strObj, '<span class="partialSourceObject partialAddSourceText" contenteditable="false" unselectable="on" style="width: ' + txtWidth + '; height: ' + txtHeight + ';" srcidentifier="' + srcId + '" partialid="' + partialID + '" data-limit="' + limit + '"><img class="cke_reset cke_widget_mask partialAddSourceTextMark" data-cke-saved-src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D"><span>' + value + '</span></span>');
                }
            } else if (type === "image") {
                var $sourceObjImg = $sourceObj.find('img');
                var dWidth = $sourceObjImg.attr("width");
                var dHeight = $sourceObjImg.attr("height");
                var myUrl = $sourceObjImg.attr("src");
                var percent = $sourceObjImg.attr("percent");
                var style = $sourceObjImg.attr("style");
                var tts = $sourceObjImg.attr("texttospeech");

                var innerContent = '<img class="partialSourceObject partialAddSourceImage" unselectable="on" width="' + dWidth + '" height="' + dHeight + '" style="' + style + '" contenteditable="false" src="' + myUrl + '" data-cke-saved-src="' + myUrl + '" percent="' + percent + '" partialid="' + partialID + '" srcidentifier="' + srcId + '" data-limit="' + limit + '" texttospeech="' + tts+'"/>'
                innerContent = '<span class="partialSourceImageWrapper" contenteditable="false" unselectable="on">' + innerContent + '</span>';
                xmlContent = xmlContent.replace(strObj, innerContent);
            }

            source.push({ srcIdentifier: srcId, type: type, value: value, limit: limit });
        });

        // Import partial destination
        $xmlContent.find("destinationObject").each(function (index, destinationObject) {
            var $desObj = $(destinationObject);
            var $destinationItem = $desObj.find("destinationItem");
            var strDes = $desObj.prop("outerHTML");
            var desObjType = $desObj.attr('type');

            //Fix for IE9
            if ($.browser.msie && parseInt($.browser.version, 10) < 10) {
                strDes = xmlContentForIE(strDes);
            }

            if (desObjType == "text") {
                var srcId = $destinationItem.attr("destIdentifier");
                var type = $desObj.attr("type");
                var value = $destinationItem.html();
                var widthDesText = $destinationItem.attr("width");
                var heightDesText = $destinationItem.attr("height");
                var numberDroppableDesText = $destinationItem.attr('numberDroppable');
                var notRequireAllAnswers = $destinationItem.attr('notRequireAllAnswers') || '0';
                destination.push({ destIdentifier: srcId, type: "text", notRequireAllAnswers: notRequireAllAnswers });

                widthDesText = widthDesText == undefined ? '55' : widthDesText;
                heightDesText = heightDesText == undefined ? '20': heightDesText;
                numberDroppableDesText = numberDroppableDesText == undefined ? '1' : numberDroppableDesText;

                if (iSchemeID === '35') {
                    xmlContent = xmlContent.replace(strDes, '<span class="partialDestinationObject partialAddDestinationNumerical" unselectable="on" contenteditable="false" destidentifier="' + srcId + '" type="text" style="width: ' + widthDesText + 'px; height: ' + heightDesText + 'px;" numberDroppable="' + numberDroppableDesText + '" notRequireAllAnswers="' + notRequireAllAnswers + '"><img class="cke_reset cke_widget_mask partialDragDropNumericalDestinationMark" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" data-cke-saved-src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D">' + value + '</span>');
                } else {
                    xmlContent = xmlContent.replace(strDes, '<span class="partialDestinationObject partialAddDestinationText" unselectable="on" contenteditable="false" destidentifier="' + srcId + '" type="text" style="width: ' + widthDesText + 'px; height: ' + heightDesText + 'px;" numberDroppable="' + numberDroppableDesText + '" notRequireAllAnswers="' + notRequireAllAnswers + '"><img class="cke_reset cke_widget_mask partialAddDestinationTextMark" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" data-cke-saved-src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D"><span>' + value + '</span></span>');
                }
            } else if (desObjType == 'image') {
                var groupId = "group" + index;
                var mWidth = $desObj.attr("width");
                var mHeight = $desObj.attr("height");
                var mUrl = GetViewReferenceImg + $desObj.attr("src");
                var mFloat = $desObj.attr("float");
                var percent = $desObj.attr("percent");
                var imgorgw = $desObj.attr("imgorgw");
                var imgorgh = $desObj.attr("imgorgh");

                var tts = $desObj.attr("texttospeech");
                // Build html for hot spot
                var buildHotspot = "";
                // Build html for destination image
                var strHtml = '';

                $destinationItem.each(function (index, desItem) {
                    var $desItem = $(desItem);
                    var destId = $desItem.attr('destIdentifier');
                    var destLeft = $desItem.attr('left');
                    var destTop = $desItem.attr('top');
                    var destWidth = $desItem.attr('width');
                    var destHeight = $desItem.attr('height');
                    var desGridCell = $desItem.attr('gridcell');
                    var desNumberDroppable = $desItem.attr('numberDroppable');

                    desNumberDroppable = desNumberDroppable == undefined ? '1': desNumberDroppable;
                    var notRequireAllAnswers = $desItem.attr('notRequireAllAnswers') || '0';

                    destination.push({
                        destIdentifier: destId,
                        left: destLeft,
                        top: destTop,
                        width: destWidth,
                        height: destHeight,
                        desheight: mHeight,
                        deswidth: mWidth,
                        type: "image",
                        group: groupId,
                        percent: percent,
                        imgOrgW: imgorgw,
                        imgOrgH: imgorgh,
                        notRequireAllAnswers: notRequireAllAnswers
                    });

                    if (desGridCell !== undefined && desGridCell === 'true') {
                        buildHotspot += '<div class="hotSpot" style="display: block; top: ' + destTop + 'px; left: ' + destLeft + 'px; width: ' + destWidth + 'px; height: ' + destHeight + 'px; position: absolute;" destidentifier="' + destId + '" gridcell="true" numberDroppable="' + desNumberDroppable + '" notRequireAllAnswers="' + notRequireAllAnswers + '">' + destId + '</div>';
                    } else {
                        buildHotspot += '<div class="hotSpot" style="display: block; top: ' + destTop + 'px; left: ' + destLeft + 'px; width: ' + destWidth + 'px; height: ' + destHeight + 'px; position: absolute;" destidentifier="' + destId + '" numberDroppable="' + desNumberDroppable + '" notRequireAllAnswers="' + notRequireAllAnswers + '">' + destId + '</div>';
                    }
                });

                if ($desObj.attr("src").indexOf('http') == 0) {
                    mUrl = $desObj.attr("src"); //img from S3 https://s3.amazonaws.com/testitemmedia/Vina/ItemSet_16503/take%20after-201503120339575024.png
                } else {
                    mUrl = GetViewReferenceImg + $desObj.attr("src");
                }

                if (mFloat != undefined) {
                    mFloat = " float: " + mFloat + ";";
                } else {
                    mFloat = "";
                }

                //Build html for destination image
                strHtml += '<div class="partialDestinationObject partialAddDestinationImage" contenteditable="false" unselectable="on" groupId="' + groupId + '" type="image" style="width: ' + mWidth + 'px; height: ' + mHeight + 'px;' + mFloat + '" percent="' + percent + '" imgOrgW="' + imgorgw + '" imgOrgH="' + imgorgh + '">';
                strHtml += '<img class="cke_reset cke_widget_mask partialAddDestinationImageMark" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" data-cke-saved-src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D">';
                strHtml += '<img class="destinationImage" style="width: ' + mWidth + 'px; height: ' + mHeight + 'px;" src="' + mUrl + '" data-cke-saved-src="' + mUrl + '" texttospeech="' + tts + '">';
                strHtml += buildHotspot;
                strHtml += '</div>';

                xmlContent = xmlContent.replace(strDes, strHtml);
            }
        });

        // Import response declaration
        $xmlContent.find("responseDeclaration").each(function (index, responseDeclaration) {
            var $responseDeclaration = $(responseDeclaration);
            var responseIdentifier = $responseDeclaration.attr('identifier');
            var isThresholdGrading = $responseDeclaration.attr('thresholdGrading');
            var myResult;

            if (iSchemeID === '35') {
                var responsePointsValue = $responseDeclaration.attr('pointsValue');
                var responseExpressionPattern = $responseDeclaration.attr('expressionPattern');
                var absoluteGrading = '1';
                var algorithmicGrading = $responseDeclaration.attr('algorithmicGrading') === '1' ? '1' : '0';

                if (algorithmicGrading === '1') {
                    absoluteGrading = '0';
                }

                myResult = {
                    correctResponse: {
                        pointsValue: pointsValueAlgorithmic != null ? pointsValueAlgorithmic : responsePointsValue,
                        expressionPattern: responseExpressionPattern
                    },
                    responseIdentifier: resId,
                    type: 'dragDropNumerical',
                    partialID: 'Partial_1',
                    source: source,
                    destination: destination,
                    responseDeclaration: {
                        absoluteGrading: absoluteGrading,
                        algorithmicGrading: algorithmicGrading
                    }
                };
            } else {
                if (resId == responseIdentifier) {
                    var correctRes = [];
                    $responseDeclaration.find("correctResponse").each(function () {
                        var $correctResponse = $(this);
                        var thresholdpoints = [];
                        if (isThresholdGrading === '1') {
                            $correctResponse.find('threshold').each(function () {
                                var $threshold = $(this);

                                thresholdpoints.push({
                                    low: $threshold.attr('low'),
                                    hi: $threshold.attr('hi'),
                                    pointsvalue: $threshold.attr('pointsValue')
                                });
                            });
                        }

                        correctRes.push({
                            order: $correctResponse.attr("order"),
                            destIdentifier: $correctResponse.attr("destIdentifier") && $correctResponse.attr("destIdentifier").toString(),
                            srcIdentifier: $correctResponse.attr("srcIdentifier"),
                            thresholdpoints: thresholdpoints
                        });
                    });

                    var rdthresholdGrading = typeof $responseDeclaration.attr('thresholdGrading') === 'undefined' ? '0' : $responseDeclaration.attr('thresholdGrading');
                    var rdSumCap = typeof $responseDeclaration.attr('isSumCap') === 'undefined' ? false : $responseDeclaration.attr('isSumCap');

                    myResult = {
                        responseIdentifier: resId,
                        type: "partialCredit",
                        partialID: partialID,
                        responseDeclaration: {
                            absoluteGrading: $responseDeclaration.attr("absoluteGrading"), //yes/no
                            absoluteGradingPoints: $responseDeclaration.attr("absoluteGradingPoints"),
                            partialGradingThreshold: $responseDeclaration.attr("partialGradingThreshold"),
                            relativeGrading: $responseDeclaration.attr("relativeGrading"), //yes/no
                            relativeGradingPoints: $responseDeclaration.attr("relativeGradingPoints"),
                            thresholdGrading: rdthresholdGrading,
                            algorithmicGrading: $responseDeclaration.attr("algorithmicGrading") === '1' ? '1' : '0',
                            pointsValue: pointsValueAlgorithmic != null ? pointsValueAlgorithmic : $responseDeclaration.attr("pointsValue"),
                            lineMatching: $responseDeclaration.attr("lineMatching") === '1' ? '1' : '0',
                            anchorPositionObject: $responseDeclaration.attr("anchorPositionObject") || 'right',
                            anchorPositionDestination: $responseDeclaration.attr("anchorPositionDestination") || 'left',
                            isSumCap: rdSumCap
                        },
                        correctResponse: correctRes,
                        destination: destination,
                        source: source
                    };
                }
            }

            iResult.push(myResult);
        });

        xmlContent = xmlContent
                        .replace(
                            $partialCredit.prop("outerHTML"),
                            $partialCredit.attr({ "id": resId }).prop("outerHTML")
                        );
    });

    return xmlContent;
}

/**
 * Processing import xml content of number line hot spot
 * @param  {[type]} xmlContent [description]
 * @return {[type]}            [description]
 */
function xmlImportNumberLine(xmlContent, pointsValueAlgorithmic) {
    var uA = navigator.userAgent;
    var numberLineInteraction = 'numberline-selection-interaction';
    var numberLineTitle = '';

    // Fix for IE9
    if ($.browser.msie || (uA.indexOf('Trident') != -1 && uA.indexOf('rv:11') != -1)) {
        xmlContent = xmlContentForIE(xmlContent);
    }

    var $xmlContent = $(xmlContent);

    $xmlContent.find('numberLine').each(function (ind, numberLine) {
        var $numberLine = $(numberLine);
        var numberLineResId = $numberLine.attr('responseIdentifier');
        var numberLineWidth = parseInt($numberLine.attr('width'), 10);
        var numberLineHeight = parseInt($numberLine.attr('height'), 10);
        var numberLineStart = parseInt($numberLine.attr('start'), 10);
        var numberLineEnd = parseInt($numberLine.attr('end'), 10);
        var numberLineDivision = parseInt($numberLine.attr('numberDivision'), 10);
        var numberLineMaxHotspot = $numberLine.attr('maxhotspot');
        var numberLineOuterHtml = $numberLine.prop('outerHTML');
        var numberLineHtml = '';
        var hotspotHtml = '';
        var correctResponseArr = [];
        var sourceItemArr = [];
        var $outer = $('<div/>');
        var $responseDeclaration = $xmlContent.find('responseDeclaration');

        $numberLine.find('svg').wrap('div').appendTo($outer);

        // Extract correct response number line hot spot
        if (numberLineResId === $responseDeclaration.attr('identifier')) {
            $responseDeclaration.find('correctResponse').each(function (index, respond) {
                var $respond = $(respond);
                correctResponseArr.push({
                    identifier: $respond.attr('identifier'),
                    pointValue: $respond.attr('pointValue')
                });
            });
        }

        // Check number line item have or exist
        if (!$xmlContent.find('numberLineItem').length) {
            numberLineWidth = 300;
            numberLineHeight = 100;
            numberLineInteraction = '';
            numberLineTitle = '<span class="numberline-selection-title">Number Line Hot Spot</span>';
        }

        // Extract number line item hot spot
        $xmlContent.find('numberLineItem').each(function (index, numberLineItem) {
            var $numberLineItem = $(numberLineItem);
            var nId = $numberLineItem.attr('identifier');
            var nPoints = $numberLineItem.attr('pointValue');
            var nTop = $numberLineItem.attr('top');
            var nLeft = $numberLineItem.attr('left');
            var nCorrect = false;

            for (var ci = 0, ciLen = correctResponseArr.length; ci < ciLen; ci++) {
                var correctItem = correctResponseArr[ci];

                if (correctItem.identifier === nId) {
                    nCorrect = true;
                }
            }

            sourceItemArr.push({
                identifier: nId,
                pointValue: nPoints,
                left: nLeft,
                top: nTop,
                correct: nCorrect
            });

            hotspotHtml += '<span class="numberline-hotspot" identifier="' + nId + '" ' +
                            'data-correct="' + nCorrect + '" ' +
                            'data-point="' + nPoints + '" ' +
                            'style="top: ' + nTop + '%; left: ' + nLeft + '%">&nbsp;</span>';
        });

        numberLineHtml += '<div id="' + numberLineResId + '" class="numberline-selection ' + numberLineInteraction + '" ';
        numberLineHtml += 'style="width: ' + numberLineWidth + 'px; height: ' + numberLineHeight + 'px;" contenteditable="false">';
        numberLineHtml += '<img class="cke_reset cke_widget_mask numberLineInteraction" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" />';
        numberLineHtml += numberLineTitle;
        numberLineHtml += $outer.html();
        numberLineHtml += hotspotHtml;
        numberLineHtml += '</div>&nbsp;';

        // Fix for IE9
        if ($.browser.msie || (uA.indexOf('Trident') != -1 && uA.indexOf('rv:11') != -1)) {
            numberLineOuterHtml = xmlContentForIE(numberLineOuterHtml);

            // Replace all svg in IE
            var regexSvg = /<svg .*><\/svg>/ig;

            xmlContent = xmlContent.replace(regexSvg, '');
            numberLineOuterHtml = numberLineOuterHtml.replace(regexSvg, '');
        }

        xmlContent = xmlContent.replace(numberLineOuterHtml, numberLineHtml);

        // Store iResult
        var tempResult = {
            type: 'numberLineHotSpot',
            responseIdentifier: numberLineResId,
            partialID: 'Partial_1',
            responseDeclaration: {
                absoluteGrading: $responseDeclaration.attr('absoluteGrading'),
                partialGrading: $responseDeclaration.attr('partialgrading'),
                algorithmicGrading: $responseDeclaration.attr('algorithmicGrading') === '1' ? '1' : '0',
                pointsValue: pointsValueAlgorithmic != null ? pointsValueAlgorithmic : $responseDeclaration.attr('pointsValue')
            },
            source: {
                width: numberLineWidth,
                height: numberLineHeight,
                start: numberLineStart,
                end: numberLineEnd,
                numberDivision: numberLineDivision,
                maxhotspot: numberLineMaxHotspot
            },
            sourceItem: sourceItemArr,
            correctResponse: correctResponseArr
        };

        iResult.push(tempResult);
    });

    return xmlContent;
}

/**
 * XML Import for Image Hot Spot
 * @param  {[type]} xmlContent [description]
 * @return {[type]}            [description]
 */
function xmlImportImageHotSpot(xmlContent, pointsValueAlgorithmic) {
    var $xmlContent = $(xmlContent);

    var $responseDeclarationImageHotSpot = $xmlContent.find('responseDeclaration');
    $xmlContent.find('imageHotSpot').each(function (index, imageHotSpot) {
        var $imageHs = $(imageHotSpot);
        var imageHsResId = $imageHs.attr('responseIdentifier');
        var imageHsSrc = $imageHs.attr('src');
        var imageHsPercent = $imageHs.attr('percent');
        var imageHsImgorgw = $imageHs.attr('imgorgw');
        var imageHsImgorgh = $imageHs.attr('imgorgh');
        var imageHsWidth = $imageHs.attr('width');
        var imageHsHeight = $imageHs.attr('height');
        var imageHsMaxHotspot = $imageHs.attr('maxhotspot');
        var strImageHotspot = $imageHs.prop('outerHTML');
        var strImageHotspotBuild = '';
        var sourceItemArr = [];
        var responseArr = [];
        var sourceItemHtml = '';
        var imageSrcHtml = '';
        var pointsValue = 0;
        var imageHsTexttospeech = $imageHs.attr('texttospeech');

        imageHsTexttospeech = imageHsTexttospeech === undefined ? '' : imageHsTexttospeech;

        if (imageHsResId === $responseDeclarationImageHotSpot.attr('identifier')) {
            var $correctReponse = $responseDeclarationImageHotSpot.find('correctResponse');
            pointsValue = parseInt($responseDeclarationImageHotSpot.attr('pointsValue'), 10);

            $correctReponse.each(function(index, respond) {
                var $respond = $(respond);
                responseArr.push({
                    identifier: $respond.attr('identifier'),
                    pointValue: $respond.attr('pointValue')
                });
            });
        }

        // Source item
        $xmlContent.find('sourceItem').each(function (index, sourceItem) {
            var $sourceItem = $(sourceItem);
            var sourceItemSrcId = $sourceItem.attr('identifier');
            var sourceItemPointValue = $sourceItem.attr('pointValue');
            var sourceItemLeft = $sourceItem.attr('left');
            var sourceItemTop = $sourceItem.attr('top');
            var sourceItemWidth = $sourceItem.attr('width');
            var sourceItemHeight = $sourceItem.attr('height');
            var sourceItemType = $sourceItem.attr('typeHotSpot');
            var sourceItemValue = $sourceItem.text();
            var sourceItemCorrect = false;
            var sourceItemHidden;

            sourceItemHidden = $sourceItem.attr('hiddenHotSpot') == undefined ? false : $sourceItem.attr('hiddenHotSpot');

            if ($responseDeclarationImageHotSpot.attr('absoluteGrading') === '1') {
                for (var temp = 0; temp < responseArr.length; temp++) {
                    var responseArrItem = responseArr[temp];

                    if (responseArrItem.identifier === sourceItemSrcId) {
                        sourceItemCorrect = true;
                    }
                }
            }

            if (sourceItemType === 'border-style') {
                var sourceItemShowBorder, sourceItemFill, sourceItemRolloverPreview;

                sourceItemShowBorder = $sourceItem.attr('showBorderHotSpot') == undefined ? false : $sourceItem.attr('showBorderHotSpot');
                sourceItemFill = $sourceItem.attr('fillHotSpot') == undefined ? false : $sourceItem.attr('fillHotSpot');
                sourceItemRolloverPreview = $sourceItem.attr('rolloverPreviewHotSpot') == undefined ? false : $sourceItem.attr('rolloverPreviewHotSpot');

                sourceItemArr.push({
                    identifier: sourceItemSrcId,
                    pointValue: sourceItemPointValue,
                    left: sourceItemLeft,
                    top: sourceItemTop,
                    width: sourceItemWidth,
                    height: sourceItemHeight,
                    typeHotSpot: sourceItemType,
                    value: sourceItemValue,
                    correct: sourceItemCorrect,
                    hidden: sourceItemHidden,
                    showBorder: sourceItemShowBorder,
                    fill: sourceItemFill,
                    rolloverPreview: sourceItemRolloverPreview
                });

                sourceItemHtml += '<span class="hotspot-item-type" identifier="' + sourceItemSrcId + '" style="position: absolute; width: ' + sourceItemWidth + 'px; height:' + sourceItemHeight + 'px; top: ' + sourceItemTop + 'px; left: ' + sourceItemLeft + 'px; line-height: ' + sourceItemHeight + 'px;" data-type="' + sourceItemType + '" data-correct="' + sourceItemCorrect + '" data-point="' + sourceItemPointValue + '" data-hidden="' + sourceItemHidden + '" data-show-border="' + sourceItemShowBorder + '" data-fill="' + sourceItemFill + '" data-rollover-preview="' + sourceItemRolloverPreview + '"><span class="hotspot-item-value">' + (sourceItemValue || '&nbsp;') + '</span></span>';
            } else {
                sourceItemArr.push({
                    identifier: sourceItemSrcId,
                    pointValue: sourceItemPointValue,
                    left: sourceItemLeft,
                    top: sourceItemTop,
                    width: sourceItemWidth,
                    height: sourceItemHeight,
                    typeHotSpot: sourceItemType,
                    value: sourceItemValue,
                    correct: sourceItemCorrect,
                    hidden: sourceItemHidden
                });

                sourceItemHtml += '<span class="hotspot-item-type" identifier="' + sourceItemSrcId + '" style="position: absolute; width: ' + sourceItemWidth + 'px; height:' + sourceItemHeight + 'px; top: ' + sourceItemTop + 'px; left: ' + sourceItemLeft + 'px; line-height: ' + sourceItemHeight + 'px;" data-type="' + sourceItemType + '" data-correct="' + sourceItemCorrect + '" data-point="' + sourceItemPointValue + '" data-hidden="' + sourceItemHidden + '"><span class="hotspot-item-value">' + (sourceItemValue || '&nbsp;') + '</span></span>';
            }
        });
        if (imageHsSrc === '' || imageHsSrc == 'undefined') {
            imageSrcHtml = '<span class="imageHotspotTitle">Image Hot Spot Selection</span><img style="width: 400px; height: 300px;" class="cke_reset cke_widget_mask imageHotspotMark" data-cke-saved-src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D"/>';
        } else {
            imageSrcHtml = '<img class="cke_reset cke_widget_mask imageHotspotMark" data-cke-saved-src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D"/><img class="imageHotspotMarkObject" data-cke-saved-src="' + imageHsSrc + '" src="' + imageHsSrc + '" style="width: ' + imageHsWidth + 'px;height: ' + imageHsHeight + 'px" texttospeech="' + imageHsTexttospeech + '"/>';
        }

        if (imageHsWidth == 'undefined' || imageHsHeight == 'undefined') {
            imageHsWidth = 400;
            imageHsHeight = 300;
        }

        strImageHotspotBuild += '<div id="RESPONSE_1" class="imageHotspotInteraction" style="width: ' + imageHsWidth + 'px;height: ' + imageHsHeight + 'px" contenteditable="false">';
        strImageHotspotBuild += imageSrcHtml;
        strImageHotspotBuild += sourceItemHtml;
        strImageHotspotBuild += '</div>';

        var myResult = {
            type: "imageHotSpot",
            responseIdentifier: imageHsResId,
            partialID: "Partial_1",
            responseDeclaration: {
                absoluteGrading: $responseDeclarationImageHotSpot.attr('absoluteGrading'),
                partialGrading: $responseDeclarationImageHotSpot.attr('partialgrading'),
                algorithmicGrading: $responseDeclarationImageHotSpot.attr('algorithmicgrading') === '1' ? '1' : '0',
                pointsValue: pointsValueAlgorithmic != null ? pointsValueAlgorithmic : $responseDeclarationImageHotSpot.attr('pointsValue')
            },
            source: {
                src: imageHsSrc,
                percent: imageHsPercent,
                imgorgw: imageHsImgorgw,
                imgorgh: imageHsImgorgh,
                width: imageHsWidth,
                height: imageHsHeight,
                maxhotspot: imageHsMaxHotspot
            },
            sourceItem: sourceItemArr,
            correctResponse: responseArr
        };

        iResult.push(myResult);

        // Fix for IE9
        if ($.browser.msie && parseInt($.browser.version, 10) < 10) {
            strImageHotspot = xmlContentForIE(strImageHotspot);
        }

        xmlContent = xmlContent.replace(strImageHotspot, strImageHotspotBuild);
    });

    return xmlContent;
}


/**
 * XML Import for Image Hot Spot
 * @param  {[type]} xmlContent [description]
 * @return {[type]}            [description]
 */
function xmlImportSequence(xmlContent, pointsValueAlgorithmic) {
    var $xmlContent = $(xmlContent);

    $xmlContent.find('partialSequence').each(function (index, sequenceBlock) {
        var pointsValue = 0;
        var respId = $(this).attr("responseIdentifier");
        var orientation = $(this).attr("orientation");
        var $responseDeclarationSequence = $xmlContent.find('responseDeclaration[identifier=' + respId + ']');
        var correctAnswer = "";
        var originalSequence = $(this).prop("outerHTML");
        var widthItem = $(this).find("sourceItem").attr("width");
        var algorithmicGrading = $responseDeclarationSequence.attr('algorithmicGrading') === '1' ? '1' : '0';
        var absoluteGrading = '1';

        if (algorithmicGrading === '1') {
            absoluteGrading = '0';
        }

        if ($responseDeclarationSequence.length == 1) {
            pointsValue = parseInt($responseDeclarationSequence.attr('pointsValue'), 10);
            correctAnswer = $responseDeclarationSequence.find("correctResponse value").text();
        }

        //Build sequence item
        var sequenceItem = ""; //<span class="sequenceItem" identifier="SRC_1">Answer 1</span>
        $(this).find("sourceItem").each(function () {
            sequenceItem += '<span class="sequenceItem" style="width: '+ widthItem +'px;" identifier="' + $(this).attr("identifier") + '">' + $(this).find('div[styleName="value"]').prop('outerHTML') + '</span>';
        });

        var htmlSequence = '<div contenteditable="false" class="sequenceBlock" id="' + respId + '" orientation="' + orientation + '"><button id="single-click" class="single-click">Click here to edit Sequence</button><img src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" data-cke-saved-src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" class="cke_reset cke_widget_mask sequenceMark"><span class="sequence">' + sequenceItem + '</span></div>';

        var myResult = {
            type: "sequence",
            responseIdentifier: respId,
            responseDeclaration: {
                pointsValue: pointsValueAlgorithmic != null ? pointsValueAlgorithmic : pointsValue,
                absoluteGrading: absoluteGrading,
                algorithmicGrading: algorithmicGrading,
                baseType: "identifier"
            },
            correctResponse: correctAnswer,
            orientation: orientation,
            widthItem: widthItem
        };

        iResult.push(myResult);

        // Fix for IE9
        if ($.browser.msie && parseInt($.browser.version, 10) < 10) {
            originalSequence = xmlContentForIE(originalSequence);
        }

        xmlContent = xmlContent.replace(originalSequence, htmlSequence);
    });

    return xmlContent;
}
/**
 * XML Import for Multiple Choice with Variable Points Per Answer
 * @param  {[type]} xmlContent [description]
 * @return {[type]}            [description]
 */
function xmlMultipleChoiceVariable(multipleChoice, xmlContent, pointsValueAlgorithmic) {

    //This is for normal choiceInteraction
    var $multipleChoice = $(multipleChoice);
    var resId = $multipleChoice.attr("responseIdentifier"),
        iShuffle = $multipleChoice.attr("shuffle"),
        iMaxChoices = $multipleChoice.attr("maxChoices"),
        strChoiceInteraction = $multipleChoice.prop("outerHTML"),
        newChoiceInteraction = "",
        currentChoice = multipleChoice,
        htmlMulChoice = ""

    var multipleChoiceDisplay = $multipleChoice.data('display') == null ? 'vertical' : $multipleChoice.data('display');
    var multipleChoiceGridPerRow = $multipleChoice.data('grid-per-row') == null ? 2 : $multipleChoice.data('grid-per-row');


    //Fix for IE9
    if ($.browser.msie && parseInt($.browser.version, 10) < 10) {
        strChoiceInteraction = xmlContentForIE(strChoiceInteraction);
    }

    $(xmlContent).find("responseDeclaration").each(function () {

        if (resId == $(this).attr("identifier")) {

            var iPoint = $(this).attr("pointsValue");

            iPoint = pointsValueAlgorithmic != null ? pointsValueAlgorithmic : iPoint;

            iCardinality = (typeof $(this).attr("cardinality") === 'undefined') ? 'single' : $(this).attr("cardinality"),
            simpleMulChoice = [];


            //Extract correct answer
            var $current = $(this);

            //Build Multiple question item to append to CKEditor
            $(currentChoice).find("simpleChoice").each(function (index) {

                var iddentify = alphabet[index],
                    item = { identifier: iddentify, value: $(this).find(".answer").html(),answerPoint: $current.find("value[identifier='" + iddentify + "']").text() },
                    hasAudio = 'class="nonAudioIcon"',
                    hasCorrectAnwser = 'class="item"',
                    audioLink = $(this).attr("audioRef");

                if (audioLink != undefined && audioLink != "") {
                    item.audioRef = audioLink;
                    hasAudio = 'class="audioIcon "';
                }

                htmlMulChoice += '<div ' + hasCorrectAnwser + '><div ' + hasAudio + '><img alt="Play audio" class="bntPlay" src="../../Content/themes/TestMaker/images/small_audio_play.png" title="Play audio"><img alt="Stop audio" class="bntStop" src="../../Content/themes/TestMaker/plugins/multiplechoice/images/small_audio_stop.png" title="Stop audio"><span class="audioRef">' + audioLink + '</span></div><div class="answer">' + item.identifier + '.</div> <div class="answerContent">' + item.value + '</div></div>';
                simpleMulChoice.push(item);
            });


            newChoiceInteraction = '<br/><div class="multipleChoice multipleChoiceVariable" id="' + resId + '" contenteditable="false"><button class="single-click-variable">Click here to edit answer choices</button><img class="cke_reset cke_widget_mask multipleChoiceVariableMark" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" />' + htmlMulChoice + ' </div>';
            xmlContent = xmlContent.replace(strChoiceInteraction, newChoiceInteraction);
            var myResult = {
                type: "choiceInteractionVariable",
                responseIdentifier: resId,
                maxChoices: iMaxChoices,
                shuffle: iShuffle,
                responseDeclaration: {
                    baseType: (typeof $(this).attr("baseType") === 'undefined') ? 'identifier' : $(this).attr("baseType"),
                    cardinality: iCardinality,
                    method: (typeof $(this).attr("method") === 'undefined') ? 'default' : $(this).attr("method"),
                    caseSensitive: (typeof $(this).attr("caseSensitive") === 'undefined') ? 'false' : $(this).attr("caseSensitive"),
                    pointsValue: iPoint,
                    type: (typeof $(this).attr("type") === 'undefined') ? 'string' : $(this).attr("type")
                },
                simpleChoice: simpleMulChoice,
                display: multipleChoiceDisplay
            };

            if (multipleChoiceDisplay === 'grid') {
                myResult.gridPerRow = multipleChoiceGridPerRow;
            }

            //Processing to import major and depending
            createImportMajorDepending(this);

            iResult.push(myResult);

            return false;
        }
    });

    return xmlContent;
}

function xmlImportItemTypeOnImage (xmlContent) {
    var $xmlContent = $(xmlContent);

    $xmlContent.find('.itemtypeonimage').replaceWith(function () {
        var $oldItemTypeOnImage = $(this);
        var $newItemTypeOnImage = $('<div/>');
        var patternMask = '<img class="cke_reset cke_widget_mask itemtypeonimageMark" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D"/>';

        copyAttributes($oldItemTypeOnImage, $newItemTypeOnImage);

        $newItemTypeOnImage.append(patternMask);
        $newItemTypeOnImage.append($oldItemTypeOnImage.html());
        $newItemTypeOnImage = $newItemTypeOnImage.after('&nbsp;');

        return $newItemTypeOnImage;
    });

    xmlContent = $xmlContent.prop('outerHTML');

    return xmlContent;
}

/**
 * Get text to speech from xmlContent
 * @param  {[type]} xmlContent [description]
 * @return {[type]}            [description]
 */
function getTexttospeechFromXml(xmlContent) {
    var $xmlContent = $(xmlContent);
    var texttospeechEnable = $xmlContent.attr('texttospeechenable');
    var texttospeechRate = $xmlContent.attr('texttospeechrate');
    var texttospeechVolume = $xmlContent.attr('texttospeechvolume');

    texttospeechEnable = texttospeechEnable === undefined ? true : texttospeechEnable;
    texttospeechRate = texttospeechRate === undefined ? '0.8' : texttospeechRate;
    texttospeechVolume = texttospeechVolume === undefined ? '0.5' : texttospeechVolume;

    var texttospeech = {
        enable: texttospeechEnable,
        rate: texttospeechRate,
        volume: texttospeechVolume
    };

    return texttospeech;
}
/**
 * Replace xmlContent for IE
 * @param  {[type]} xml [description]
 * @return {[type]}     [description]
 */
function xmlContentForIE (xml) {
    if (xml != null) {
        xml = xml.replace('<?XML:NAMESPACE PREFIX = \"[default] http://www.imsglobal.org/xsd/imsqti_v2p0\" NS = \"http://www.imsglobal.org/xsd/imsqti_v2p0\" />', '')
                .replace('<?XML:NAMESPACE PREFIX = [default] http://www.imsglobal.org/xsd/imsqti_v2p0 NS = "http://www.imsglobal.org/xsd/imsqti_v2p0" />', '')
                .replace('<?XML:NAMESPACE PREFIX = "[default] http://www.w3.org/1998/Math/MathML" NS = "http://www.w3.org/1998/Math/MathML" />', '')
                .replace('<?XML:NAMESPACE PREFIX = [default] http://www.w3.org/1998/Math/MathML NS = "http://www.w3.org/1998/Math/MathML" />', '')
                .replace('<?XML:NAMESPACE PREFIX = "[default] http://www.imsglobal.org/xsd/imsqti_v2p0" NS = "http://www.imsglobal.org/xsd/imsqti_v2p0" />', '');
    }

    return xml;
}

function validateDragAndDrop (result) {
    for (var i = 0, len = result.length; i < len; i++) {
        var resultItem = result[i];

        if (resultItem.type == 'partialCredit') {
            var isThresholdGrading = String(resultItem.responseDeclaration.thresholdGrading);
            var isSumCap = resultItem.responseDeclaration.isSumCap;
            var maxThresholdPoints = [];
            var minThresholdPoints = [];

            if (isThresholdGrading === '1' && (typeof isSumCap === 'undefined' || isSumCap === 'false')) {
                var pointsValue = parseInt(resultItem.responseDeclaration.pointsValue, 10);

                for (var rd = 0, rdLen = resultItem.correctResponse.length; rd < rdLen; rd++) {
                    var thresholdpointsArr = resultItem.correctResponse[rd].thresholdpoints;

                    if (thresholdpointsArr !== undefined && thresholdpointsArr.length) {
                        maxThresholdPoints.push(maxOfArray(thresholdpointsArr, 'pointsvalue'));
                    }
                }

                if (maxThresholdPoints.length) {
                    var maxPoint = sumOfArray(maxThresholdPoints);

                    maxPoint = parseInt(maxPoint, 10);

                    if (pointsValue > maxPoint) {
                        return false;
                    }
                }
            }
        }
    }

    return true;
}

/**
 * Max value of object array by key
 * @param  {[type]} arr [description]
 * @param  {[type]} key [description]
 * @return {[type]}     [description]
 */
function maxOfArray(arr, key) {
    return Math.max.apply(Math, arr.map(function (o) {
        return o[key];
    }));
}

/**
 * Min value of object array by key
 * @param  {[type]} arr [description]
 * @param  {[type]} key [description]
 * @return {[type]}     [description]
 */
function minOfArray(arr, key) {
    return Math.min.apply(Math, arr.map(function (o) {
        return o[key];
    }));
}

/**
 * Sum of array
 * @param  {[type]} arr [description]
 * @return {[type]}     [description]
 */
function sumOfArray(arr) {
    return arr.reduce(function (a, b) {
        return a + b;
    }, 0);
}
