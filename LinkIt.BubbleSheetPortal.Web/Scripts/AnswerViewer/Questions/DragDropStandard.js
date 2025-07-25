/**
 * Drag And Drop Standard Module
 * @param  {[type]} function (             [description]
 * @return {[type]}          [description]
 */
var DragDropStandard = (function () {
    /**
     * Display xml to html
     * @param  {[type]} xmlContent [description]
     * @return {[type]}            [description]
     */
    function displayHtml (xmlContent) {
        var $tree = $('<div/>');

        $tree.append(xmlContent);

        $tree.find('sourceobject').replaceWith(function () {
            var $sourceobject = $(this);
            var $newSourceobject = $('<div/>');

            Utils.copyAttributes($sourceobject, $newSourceobject);

            $newSourceobject
                .addClass('dragdrop-source')
                .html($sourceobject.html());

            return $newSourceobject;
        });

        $tree.find('destinationobject').replaceWith(function () {
            var $destinationobject = $(this);
            var destinationobjectType = $destinationobject.attr('type');
            var destinationobjectWidth = $destinationobject.attr('width');
            var destinationobjectHeight = $destinationobject.attr('height');
            var $newDestinationobject = $('<div/>');
            var isDestinationImage = destinationobjectType === 'image' ? true : false;

            $newDestinationobject
                .addClass('dragdrop-destination')
                .attr('type', destinationobjectType)
                .html($destinationobject.html());

            if (isDestinationImage) {
                var $destinationobjectImg = $('<img/>');

                Utils.copyAttributes($destinationobject, $destinationobjectImg);

                $newDestinationobject.css({
                    width: destinationobjectWidth + 'px',
                    height: destinationobjectHeight + 'px'
                })

                $destinationobjectImg.css({
                    width: destinationobjectWidth + 'px',
                    height: destinationobjectHeight + 'px'
                });

                $newDestinationobject.append($destinationobjectImg);
            }

            $newDestinationobject.find('destinationitem').replaceWith(function () {
                var $destinationitem = $(this);
                var destinationitemWidth = $destinationitem.attr('width');
                var destinationitemHeight = $destinationitem.attr('height');
                var $newDestinationitem = $('<div/>');
                var desvalue = $destinationitem.prop('innerHTML');

                Utils.copyAttributes($destinationitem, $newDestinationitem);

                $newDestinationitem
                    .css({
                        width: destinationitemWidth + 'px',
                        height: destinationitemHeight + 'px',
                    })
                    .attr('desvalue', desvalue)
                    .addClass('dragdrop-destination-item')
                    .html($destinationitem.html());

                if (isDestinationImage) {
                    var destinationitemTop = $destinationitem.attr('top');
                    var destinationitemLeft = $destinationitem.attr('left');

                    $newDestinationitem.css({
                        top: destinationitemTop + 'px',
                        left: destinationitemLeft + 'px'
                    });
                }

                return $newDestinationitem;
            });

            return $newDestinationobject;
        });

        return $tree.html();
    }

    /**
     * Display answer of student
     * @param  {[type]} xmlContent [description]
     * @param  {[type]} answer     [description]
     * @return {[type]}            [description]
     */
    function displayByAnswer (xmlContent, answer) {
        var $tree = $('<div/>');

        $tree.append(xmlContent);
        var isMatchingPair = !!$tree.find('[linematching="1"]').length;

        if (!Utils.isNullOrEmpty(answer)) {
            answer = answer.split(',');

            $tree.find('.dragdrop-destination-item').each(function (index, destitem) {
                var $destitem = $(destitem);
                var destitemId = $destitem.attr('destidentifier');

                for (var i = 0, len = answer.length; i < len; i++) {
                    var mapping = answer[i];
                    var mappingValue = mapping.split('-');
                    var mappingDest = mappingValue[0];
                    var mappingSrc = mappingValue[1];

                    // Compare destination identifier with mapping
                    if (destitemId === mappingDest && mappingSrc !== '') {
                        var si = 0;
                        var srcObjArr = mappingSrc.split(';');

                        // Reset destination item when destination item have source object
                        !isMatchingPair && $destitem.html('');

                        while (si < srcObjArr.length) {
                            $destitem.append($tree.find('.dragdrop-source[srcidentifier="' + srcObjArr[si] + '"]').clone(true));
                            si++;
                        }
                    }
                }
            });
        }

        return $tree.html();
    }

    /**
     * Update html answer of student
     * @param  {[type]} xmlContent [description]
     * @param  {[type]} answer     [description]
     * @return {[type]}            [description]
     */
     function updateAnswerHtml (question, xmlContent, isCorrect) {
         var resultHtml = '';

         if (isCorrect) {
             resultHtml = displayByAnswer(xmlContent, question.CorrectAnswer);
         } else {
             resultHtml = displayByAnswer(xmlContent, question.AnswerText);
         }

         return resultHtml;
     }

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
        $('responsedeclaration').each(function (_, responseDeclaration) {
            var qtiConfig = $(responseDeclaration);
            var qtiXml = qtiConfig.parent();
            if (qtiConfig.attr('linematching') == '1') {
              var anchorObject = qtiConfig.attr('anchorpositionobject') || 'right';
              var anchorDestination = qtiConfig.attr('anchorpositiondestination') || 'left';
              qtiXml.addClass('line-matching')
                .addClass('object-' + anchorObject)
                .addClass('destination-' + anchorDestination);
              qtiXml.find('.dragdrop-source[type="image"]').append('<div class="anchor"></div>')
              qtiXml.find('.dragdrop-destination[type="text"]').wrap('<div class="text-wrapper destination"></div>');
              qtiXml.find('.dragdrop-source[type="text"]').not('.dragdrop-destination-item .dragdrop-source').wrap('<div class="text-wrapper object"></div>');
              qtiXml.find('.text-wrapper').append('<div class="anchor"></div>');
            //   qtiXml.find('.dragdrop-destination[type="text"] .dragdrop-destination-item').html(function(_, innerHtml) {
            //     return innerHtml + $(this).attr('desvalue');
            //   })

              $('.destinationItem .sourceObject script[type="math/mml"]').remove();
              MathJax.Hub.Queue(['Typeset', MathJax.Hub]);

              var sources = qtiXml.find('.dragdrop-source').not('.dragdrop-destination-item sourceobject');
              var anwerSources = qtiXml.find('.dragdrop-destination-item .dragdrop-source');
              anwerSources.each(function (_, answer) {
                var answerSrc = $(answer);
                var destination = answerSrc.closest('.dragdrop-destination');
                var isTextDes = destination.attr('type') === 'text';
                var src = sources.filter("[srcidentifier=\"" + answerSrc.attr('srcidentifier') + "\"]");
                var anchorSrc = src.attr('type') === 'text' ? src.parent().find('.anchor')[0] : src.find('.anchor')[0];
                var anchorDes = isTextDes ? destination.closest('.text-wrapper').find('.anchor').last()[0] : answerSrc.closest('.dragdrop-destination-item')[0];
                var desVal = (isTextDes ? destination.find('.dragdrop-destination-item') : answerSrc.closest('.dragdrop-destination-item')).attr('destidentifier');
                setTimeout(function() {
                    addLineBetweenElements(anchorSrc, anchorDes, [src.attr('srcidentifier'), desVal], qtiXml[0]);
                }, 500);
              })
            }
        })
     }

    return {
        displayHtml: displayHtml,
        updateAnswerHtml: updateAnswerHtml,
        displayLineMatching: displayLineMatching
    }
})();
