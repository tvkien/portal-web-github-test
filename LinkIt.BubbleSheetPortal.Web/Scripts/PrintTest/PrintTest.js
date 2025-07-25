//It is depend on imagesloaded.pkgd.js

function ResizeImageBaseOnPercent(image) {
    //Correct image incase image has width or height is NaN
    var newHeight = image.height();
    var newWidth = image.width();
    if (image.attr('height') == undefined || image.attr('height').toString() == "NaN") {
        if (image.attr('percent') != undefined) {
            newHeight = (newHeight * parseInt(image.attr('percent').toString() + "0")) / 100;
            image.attr({ 'height': newHeight });
        } else {
            image.attr({ 'height': newHeight });
        }
    }
    if (image.attr('width') == undefined || image.attr('width').toString() == "NaN") {
        if (image.attr('percent') != undefined) {
            newWidth = (newWidth * parseInt(image.attr('percent').toString() + "0")) / 100;
            image.attr({ 'width': newWidth });
        } else {
            image.attr({ 'width': newWidth });
        }
    }
}

function ResizeImagesBaseOnPercent(containerSelector) {
    $(containerSelector).imagesLoaded(function () {
        $(containerSelector + ' img').each(function () {
           var image = $(this);
            ResizeImageBaseOnPercent(image);
       });
    });
}

function DisplayImageHotspot(selectorStr, path) {
    var $selectorStr = $(selectorStr);
    var maxWidthAssessmentItem = $selectorStr.attr('maxWidth') - 20;
    var reducePercent = 0;
    var width = null;
    var height = null;

    $selectorStr.find('imageHotSpot, .imageHotSpot')
        .replaceWith(function () {
            var imgHotSpot = $(this);

            width = imgHotSpot.attr('width');
            height = imgHotSpot.attr('height');

            if (width > maxWidthAssessmentItem) {
                reducePercent = (width - maxWidthAssessmentItem) / width;
                width = maxWidthAssessmentItem;
                height = height - (height * reducePercent);
            }

            var image = $('<img/>');
            CopyAttributes(imgHotSpot, image);

            image.css({
                'width': width + 'px',
                'height': height + 'px'
            });

            var src = imgHotSpot.attr('src');
            if (src.indexOf('http') < 0) { 
                if (src.charAt(0) == '/') src = src.substring(1);
                src = path + src;
            }
            src = src.replace(/ /g, '%20');
            image.attr('src', src);

            var result = $('<div class="imageHotspotInteraction"></div>');
            result.attr('responseIdentifier', imgHotSpot.attr('responseIdentifier'));
            result.css('width', width + 'px');
            result.css('height', height + 'px');

            result.html(imgHotSpot.html());
            result.prepend(image);
            result.css('position', 'relative');

            width = width === undefined ? 20 : width;
            height = height === undefined ? 20 : height;

            result.find('.sourceItem')
                .replaceWith(function () {
                    var sourceItem = $(this);

                    var widthSrc = sourceItem.attr('width') - (sourceItem.attr('width') * reducePercent);
                    var heigthSrc = sourceItem.attr('height') - (sourceItem.attr('height') * reducePercent);
                    var topSrc = sourceItem.attr('top') - (sourceItem.attr('top') * reducePercent);
                    var leftSrc = sourceItem.attr('left') - (sourceItem.attr('left') * reducePercent);

                    var widthPercent = (widthSrc / width) * 100;
                    var heigthPercent = (heigthSrc / height) * 100;
                    var topPercent = (topSrc / height) * 100;
                    var leftPercent = (leftSrc / width) * 100;

                    var sourceItemHidden = sourceItem.attr('hiddenHotSpot') == undefined ? false : sourceItem.attr('hiddenHotSpot');

                    var newSourceItem = $('<span class="hotspot-item-type"><span class=" hotspot-item-value">' + sourceItem.html() + '</span></span>');
                    CopyAttributes(sourceItem, newSourceItem);
                    newSourceItem.addClass('hotspot-item-type');
                    newSourceItem.css('top', topPercent + '%');
                    newSourceItem.css('left', leftPercent + '%');
                    newSourceItem.css('width', widthPercent + '%');
                    newSourceItem.css('height', heigthPercent + '%');
                    newSourceItem.css('position', 'absolute');
                    if (sourceItemHidden == 'true') {
                        newSourceItem.addClass('hotspot-hidden');
                    }

                    return newSourceItem;
                });

            result.css({
                'width': width + 'px',
                'height': 'auto'
            });

            return $(result);
        });
}

function DisplayDragAndDrop(maxWidth) {

    if (maxWidth === undefined) {
        maxWidth = 260;
    } else {
        maxWidth = maxWidth - 20;
    }

    $('.destinationObject').replaceWith(function() {
        var $desObj = $(this);
        var desObjType = $desObj.attr('type');
        var desObjWidthImg;
        var desObjHeightImg;
        var $desObjHtml = $('<div/>');
        var reducePercent = 0;
        var questionContentWidth = maxWidth;

        $desObjHtml.attr({
            'class': 'destinationObject',
            'type': desObjType
        });

        // Check destination has image
        if (desObjType === 'image') {
            var $img = $('<img/>');
            CopyAttributes($desObj, $img);
            var imgWidth = $img.attr('width');
            var imgHeight = $img.attr('height');

            desObjWidthImg = imgWidth !== undefined ? imgWidth : 250;
            desObjHeightImg = imgHeight !== undefined ? imgHeight : 100;

            if (desObjWidthImg > questionContentWidth) {
                reducePercent = (desObjWidthImg - questionContentWidth) / desObjWidthImg;
                desObjWidthImg = questionContentWidth;
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
            var desItemIdentifier = $desItem.attr('destidentifier');
            var desItemWidth = $desItem.attr('width') - $desItem.attr('width') * reducePercent;
            var desItemHeight = $desItem.attr('height') - $desItem.attr('height') * reducePercent;
            var desItemHtml = '';

            desItemWidth = desItemWidth === undefined ? 55 : desItemWidth;
            desItemHeight = desItemHeight === undefined ? 20 : desItemHeight;

            // Check destionation object is image or text
            if (desObjType === 'image') {
                var desItemTop = $desItem.attr('top') - $desItem.attr('top') * reducePercent;
                var desItemLeft = $desItem.attr('left') - $desItem.attr('left') * reducePercent;
                var percentWidth, percentHeight, percentTop, percentLeft;

                desItemTop = desItemTop == undefined ? 10 : desItemTop;
                desItemLeft = desItemLeft == undefined ? 10 : desItemLeft;

                percentWidth = (desItemWidth / desObjWidthImg) * 100;
                percentHeight = (desItemHeight / desObjHeightImg) * 100;
                percentTop = (desItemTop / desObjHeightImg) * 100;
                percentLeft = (desItemLeft / desObjWidthImg) * 100;

                desItemHtml = '<div class="destinationItem" style="width: ' + percentWidth + '%; height: ' + percentHeight + '%; top: ' + percentTop + '%; left: ' + percentLeft + '%; position: absolute; overflow: hidden;">' + $desItem.html() + '</div>';
            } else {
                if (desItemWidth > questionContentWidth) {
                    reducePercent = (desItemWidth - questionContentWidth) / desItemWidth;
                    desItemWidth = questionContentWidth;
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
        qtiXml.find('.text-wrapper').append('<div class="anchor"></div>')
      }
    })
}

function DisplayNumberlineHotspot(maxWidth) {

    if (maxWidth === undefined) {
        maxWidth = 260;
    } else {
        maxWidth = maxWidth - 20;
    }

    $('.numberLine').replaceWith(function () {
        var $numberline = $(this);
        var numberlineWidth = $numberline.attr('width');
        var numberlineHeight = $numberline.attr('height');
        var $result = $('<div/>');
        var reducePercent = 0;
        var questionContentWidth = maxWidth;

        $result.html($numberline.html());

        CopyAttributes($numberline, $result);

        if (numberlineWidth > questionContentWidth) {
            reducePercent = (numberlineWidth - questionContentWidth) / numberlineWidth;
            numberlineWidth = questionContentWidth;
            numberlineHeight = numberlineHeight - numberlineHeight * reducePercent;
        }

        $result.css({
            'width': numberlineWidth + 'px',
            'height': numberlineHeight + 'px'
        });

        return $result;
    });
}

function DisplayTableLinkit(maxWidth) {

    if (maxWidth === undefined) {
        maxWidth = 260;
    } else if (maxWidth < 300) {
        maxWidth = 260;
    } else {
        maxWidth = maxWidth - 40;
    }

    $('.linkit-table, table').each(function(index, table) {
        var $table = $(table);
        var tableStyle = $table.attr('style');
        var tableWidth;
        var arrTd = [];
        var reducePercent = 0;
        var questionContentWidth = maxWidth;

        if (tableStyle !== undefined) {
            var tableStyle = tableStyle.match(/width:(.*?)px;/g);

            tableStyle = tableStyle[0].replace('width:', '').replace('px;', '');

            tableWidth = parseInt(tableStyle, 10);
        }

        if ($table.parents('.passage').length) {
            questionContentWidth = 620;
        }

        if (tableWidth > questionContentWidth) {
            reducePercent = (tableWidth - questionContentWidth) / tableWidth;
            tableWidth = questionContentWidth;
        }

        $table.find('tr').eq(0).find('td').each(function(i, td) {
            var $td = $(td);

            if ($td.attr('style') !== undefined) {
                var tdStyle = $td.attr('style');

                tdStyle = tdStyle.match(/width:(.*?)px;/g);

                if (tdStyle !== null) {
                    tdStyle = tdStyle[0].replace('width:', '').replace('px;', '');

                    tdStyle = parseInt(tdStyle, 10);

                    tdStyle = tdStyle - tdStyle * reducePercent;

                    tdStyle = parseInt(tdStyle, 10) - 10;

                    arrTd.push(tdStyle);
                }
            } else {
                arrTd.push('auto');
            }
        });

        // Remove attribute width of td
        $table.css('width', tableWidth + 'px');
        $table.find('td').removeAttr('width');

        if (arrTd.length > 0) {
            // Apply to td width
            for (var d = 0, len = arrTd.length; d < len; d++) {
                var arrTdItem = arrTd[d];

                if (arrTdItem === 'auto') {
                    $table.find('tr').find('td').eq(d).css('width', arrTd[d]);
                } else {
                    $table.find('tr').find('td').eq(d).css('width', arrTd[d] + 'px');
                }
            }
        }
    });
}

function CopyAttributes(from, to) {
    var attrs = from.prop("attributes");
    $.each(attrs, function (index, attribute) {
        to.attr(attribute.name, attribute.value);
    });
}

function DisplayGlossary(selector) {
    $(selector).each(function() {
        var element = $(this);
        var hasGlossary = false;
        element.find('span.glossary').each(function() {
            var spanGlossary = $(this);
            var glossaryContent = spanGlossary.attr('glossary');
            glossaryContent = glossaryContent.replace(/&lt;br\/&gt;/gi, '<br/>')
                .replace(/&gt;/g, '>')
                .replace(/&lt;/g, '<');
            glossaryContent = '<div class="glossaryDiv"><b>' + spanGlossary.html() + '</b>: ' + glossaryContent + '</div>';
            AddToSortedList(glossaryContent, $('#glossaryContainer'));
            hasGlossary = true;
        });

        if (hasGlossary) {
            $('#glossaryContainer').css('page-break-before','auto');
            $('#glossaryContainer').prepend('<h2>Glossary</h2>');
        } else {
        $('#glossaryContainer').remove();
        }
    });
}

function AddToSortedList(content, target) {
    var added = false;
    var $content = $(content);
    $(".glossaryDiv", target).each(function(){
        if ($(this).text().toLowerCase() > $content.text().toLowerCase()) {
            $(content).insertBefore($(this));
            added = true;
            return false;
        } else if ($(this).text().toLowerCase() == $content.text().toLowerCase()) {
            added = true;
            return false;
        }
    });
    if(!added) $content.appendTo(target);
}

function CorrectImages(selector, mapPath, maxWidth) {
    var $selector = $(selector);
    var destinationText = '';

    if (maxWidth === undefined) {
        maxWidth = 260;
    } else {
        maxWidth = maxWidth - 40;
    }

    $selector.find('img').replaceWith(function() {
        var $image = $(this);
        var imageFloat = $image.attr('float');
        var imageUrl = $image.attr('src');
        var imageWidth = $image.attr('width');
        var imageHeight = $image.attr('height');
        var $imageContainer = $('<div/>');
        var $imageNew = $('<img/>');
        var reducePercent = 0;
        var questionContentWidth = maxWidth;
        var isFloat = false;

        if ($image.parents('.passage').length) {
            questionContentWidth = 620;
        }

        if ($image.attr('style') != null && $image.attr('style').indexOf('float') > -1) {
            $image.attr('style', $image.attr('style'));
            isFloat = true;
        }

        if (!isFloat) {
            // Check float of image
            if (imageFloat == null) {
                imageFloat = '';
            }
            $image.css('float', imageFloat);
        }

        // Check if image from Certica
        if (imageUrl === undefined) {
            imageUrl = $image.attr('source');
        }

        // Check images exists or not
        if (imageUrl === null || imageUrl === '' || imageUrl === undefined) {
            imageUrl = mapPath + '/Content/images/emptybg.png';
        }

        // Substring images url
        if (imageUrl.charAt(0) == '/') {
            imageUrl = imageUrl.substring(1);
        }

        // Set attribute for images
        $image.attr({
            'source': '',
            'src': imageUrl
        });

        if (imageUrl && imageUrl.toLowerCase().indexOf('itemset') >= 0 &&
            imageUrl.toLowerCase().indexOf('http') < 0 &&
            imageUrl.toLowerCase().indexOf('getviewreferenceimg') < 0) {
            imageUrl = mapPath + '/Asset/GetViewReferenceImg?imgPath=' + imageUrl;
            $image.attr('src', imageUrl);
        }

        imageUrl = $image.attr('src');
        // PrintPDF tool (Prince) does not understand file path includes space, 
        // so that replace space to %20 to let it understand
        imageUrl = imageUrl.replace(/ /g, '%20');
        $image.attr('src', imageUrl);

        if (imageWidth > questionContentWidth) {
            reducePercent = (imageWidth - questionContentWidth) / imageWidth;
            imageWidth = questionContentWidth;
            imageHeight = imageHeight - imageHeight * reducePercent;
        }

        $image.css({
            'width': imageWidth + 'px',
            'height': imageHeight + 'px'
        }).attr({
            'data-real-width': imageWidth,
            'data-real-height': imageHeight
        });

        if (!!$image.parents('.extendedTextInteraction').length) {
            $image.parents('.extendedTextInteraction').css({
                'width': imageWidth + 'px',
                'height': 'auto'
            });
        }

        CopyAttributes($image, $imageNew);
        $imageContainer.append($imageNew);

        return $imageContainer.html();
    });

    return $selector.html();
}

function ImageItemType() {

    $(".questions .assessmentItem").each(function () {

        //Code for add item type to image
        var htmlPin = '<div class="img_type_pin"></div>';
        var htmlPin_Item = '<div class="img_type_pin_item"></div>';
        var me = $(this);

        $(this).find(".itemtypeonimage .itemtypeonimageMarkObject").wrap('<div class="wrap_image_item_type"></div>');
        $(this).find(".itemtypeonimage .inlineChoiceInteraction, .itemtypeonimage .textEntryInteraction").each(function () {
            var me = $(this);
            var parent = $(this).parents(".itemtypeonimage");
            var img = parent.find(".itemtypeonimageMarkObject");
            var percent = parseInt(img.attr('data-percent')) * 10 / 100;
            var img_orgin_w = parseInt(img.attr("data-original-width")) * percent;
            var img_orgin_h = parseInt(img.attr("data-original-height")) * percent;
            var img_w = parseInt(img.attr('data-real-width'));
            var img_h = parseInt(img.attr('data-real-height'));
            var img_w_scale = img_w * 100 / img_orgin_w;
            var img_h_scale = img_h * 100 / img_orgin_h;
            var top = parseInt($(this).attr("data-top")) * img_h_scale / 100;
            var left = parseInt($(this).attr("data-left")) * img_w_scale / 100;
            var style = $(this).attr("style");
            var resId = $(this).attr("responseidentifier");
            var pin = $(htmlPin).attr({
                "data-res": resId,
                "style": style
            }).css({
                "top": top,
                "left": left,
                "position": "absolute"
            });
            var pinItem = $(htmlPin_Item).attr({
                "data-res": resId
            });

            $(this).before(pinItem);
            $(this).parents(".itemtypeonimage").find(".wrap_image_item_type").append(pin);
            $(this).removeAttr("style");
        });

        $(this).find(".img_type_pin").each(function (index) {
            var resId = $(this).attr("data-res");
            $(this).text(index + 1);
            me.find(".img_type_pin_item[data-res='" + resId + "']").text(index + 1);
        });

        $(this).find(".itemtypeonimage").css({
            "height": "auto",
            "width": "auto"
        });
    });
}
