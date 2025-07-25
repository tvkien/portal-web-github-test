(function() {
    'use strict';

    var pluginName = 'numberlinehotspot';
    var isEdit = false;
    var elementNumberLine = '';
    var HEIGHT = 100;
    var $numberLineDialog = null;
    var $numberLinePreview = null;
    var $numberLineHotspotProperties = null;
    var $numberLineWidth = null;
    var $numberLineDivisions = null;
    var $numberLineStarting = null;
    var $numberLineEnding = null;
    var $numberLineFullPoints = null;
    var $numberLineMaxChoice = null;
    var $btnClearNumberLineHotspot = null;

    // Load Raphael library to draw number line tool
    CKEDITOR.scriptLoader.load('/Content/themes/TestMaker/raphael-min.js');

    CKEDITOR.plugins.add(pluginName, {
        requires: 'dialog',
        lang: 'en',
        icons: pluginName, // %REMOVE_LINE_CORE%
        hidpi: true, // %REMOVE_LINE_CORE%
        init: function(editor) {
            var plugin = this;

            // Add a command that when executed opens up the number line hot spot dialog
            editor.addCommand(
                pluginName,
                new CKEDITOR.dialogCommand(pluginName)
            );

            editor.widgets.add('numberline-selection', {
                inline: true,
                mask: true,
                allowedContent: { p: { classes: 'numberline-selection', attributes: '!id,name,contenteditable'} },
                template: '<div class="numberline-selection"></div>'
            });

            // Add a toolbar button (named: NumberLineHotSpot) that executes the plugin
            editor.ui.addButton('NumberLineHotSpot', {
                label: 'Number Line Hot Spot',
                command: pluginName
            });

            // Make it so that when you double-click on one of the placeholder
            // elements it opens up the scratchpad dialog for editing.
            editor.on('doubleclick', function(evt) {
                var element = evt.data.element;
                var parents = element.getParents();
                var parent;

                for (var i = 0; i < parents.length; i++) {
                    parent = parents[i];
                    if (parent.hasClass('numberline-selection')) {
                        break;
                    }
                }

                elementNumberLine = parent;
                editor.getSelection().selectElement(elementNumberLine);
                evt.data.dialog = pluginName;

                if (elementNumberLine.hasClass('numberline-selection-interaction')) {
                    isEdit = true;
                }

                dblickHandlerToolbar(editor);
            });

            // Bring in the dialog
            CKEDITOR.dialog.add(pluginName, getDialog);
        }
    });

    /**
     * Dialog Number Line Tool
     * @param  {[type]} editor [description]
     * @return {[type]}        [description]
     */
    var getDialog = function(editor) {
        var dialogHtml = '';

        dialogHtml = '\
                    <div id="' + pluginName + '-dialog">\
                        <div class="hotspot-dialog">\
                            <div class="hotspot-header">\
                                <div class="hotspot-header-action">\
                                    <div class="g-1-2">\
                                        <div class="hotspot-header-item">\
                                            <h3 class="hotspot-title"><strong>Number Line Tool</strong></h3>\
                                            <span class="hotspot-note">Customize the number line by changing the values below.</span>\
                                        </div>\
                                    </div>\
                                    <div class="g-1-2 text-right">\
                                        <div class="hotspot-header-item ">\
                                            <a href="javascript:void(0)" id="btn-clear-numberline-hotspot" class="is-visible cke_dialog_ui_button" role="button" hidefocus="true">\
                                                <span class="cke_dialog_ui_button">Clear Hot Spot</span>\
                                            </a>\
                                        </div>\
                                    </div>\
                                </div>\
                                <div class="hotspot-header-action">\
                                    <div class="g-1-2">\
                                        <div class="hotspot-list-property">\
                                            <span class="widthLabel">Width:</span>\
                                            <input type="text" value="300" name="numberline-width" id="numberline-width" class="txtFullcreate"/>\
                                        </div>\
                                        <div class="hotspot-list-property">\
                                            <span class="widthLabel">Starting Number:</span>\
                                            <input type="text" value="0" name="numberline-starting-number" id="numberline-starting-number" class="txtFullcreate"/>\
                                        </div>\
                                    </div>\
                                    <div class="g-1-2">\
                                        <div class="hotspot-list-property text-right">\
                                            <span class="widthLabel">Number of Hash Marks:</span>\
                                            <input type="text" value="1" name="numberline-divisions" id="numberline-divisions" class="txtFullcreate"/>\
                                        </div>\
                                        <div class="hotspot-list-property text-right">\
                                            <span class="widthLabel">Ending Number:</span>\
                                            <input type="text" value="1" name="numberline-ending-number" id="numberline-ending-number" class="txtFullcreate"/>\
                                        </div>\
                                    </div>\
                                </div>\
                                <div class="hotspot-header-action hotspot-header-property" id="numberline-hotspot-properties">\
                                    <div class="g-1-3">\
                                        <div class="hotspot-list-property m-l-4 m-b-11">\
                                            <label class="widthLabel hotspot-label" for="numberline-absoluting">\
                                                <input type="radio" id="numberline-absoluting" name="grading" checked="checked"/>\
                                                All or Nothing Grading\
                                            </label>\
                                        </div>\
                                        <div class="hotspot-list-property m-l-4 m-b-11">\
                                            <label class="widthLabel hotspot-label" for="numberline-partial">\
                                                <input type="radio" id="numberline-partial" name="grading"/>\
                                                    Partial Credit Grading\
                                            </label>\
                                        </div>\
                                        <div class="hotspot-list-property m-l-4" >\
                                            <label class="widthLabel hotspot-label" for="numberline-algorithmic">\
                                                <input type="radio" id="numberline-algorithmic" name="grading" />\
                                                    Algorithmic Grading\
                                            </label>\
                                        </div >\
                                    </div>\
                                    <div class="g-2-3">\
                                        <div class="hotspot-list-property text-right">\
                                            <span class="widthLabel">Full Credit Points:</span><input type="text" value="1" name="numberline-full-point" id="numberline-full-point" class="txtFullcreate"/>\
                                        </div>\
                                        <div class="hotspot-list-property text-right">\
                                            <span class="widthLabel widthLabelSpecial">Maximum hot spots that can be selected:</span><input type="text" value="1" name="numberline-max-choice" id="numberline-max-choice" class="txtFullcreate"/>\
                                        </div>\
                                    </div>\
                                </div>\
                            </div>\
                            <div class="hotspot-content">\
							   <span>Click on the number line below to add hot spots.</span> \
                                <div class="hotspot-container">\
                                    <div class="numberline-container">\
                                        <div class="numberline-preview" id="numberline-preview"></div>\
                                    </div>\
                                </div>\
                            </div>\
                        </div>\
                    </div>\
                    <div class="popup-numberline-property is-hidden" id="popup-numberline-property"></div>\
                    <div class="popup-overlay is-hidden" id="popup-numberline-overlay"></div>';

        return {
            title: 'Embed Number Line Hot Spot',
            minWidth: IS_V2 ? 740 : 500,
            minHeight: 300,
            contents: [{
                id: 'general',
                label: 'Settings',
                elements: [{
                    type: 'html',
                    html: dialogHtml,
                    onLoad: function () {
                        // Initialize variables
                        $numberLineDialog = $('#' + pluginName + '-dialog');
                        $numberLinePreview = $numberLineDialog.find('#numberline-preview');
                        $numberLineHotspotProperties = $numberLineDialog.find('#numberline-hotspot-properties');
                        $numberLineWidth = $numberLineDialog.find('#numberline-width');
                        $numberLineDivisions = $numberLineDialog.find('#numberline-divisions');
                        $numberLineStarting = $numberLineDialog.find('#numberline-starting-number');
                        $numberLineEnding = $numberLineDialog.find('#numberline-ending-number');
                        $numberLineFullPoints = $numberLineDialog.find('#numberline-full-point');
                        $numberLineMaxChoice = $numberLineDialog.find('#numberline-max-choice');
                        $btnClearNumberLineHotspot = $numberLineDialog.find('#btn-clear-numberline-hotspot');

                        // Call initialize up and down number
                        initUpDownNumber();

                        // Auto create numberline when change value
                        $('#numberline-width, #numberline-divisions, #numberline-starting-number, #numberline-ending-number')
                            .on('change', calledNumberLine);

                        // Clear all hot spot in number line
                        $btnClearNumberLineHotspot.on('click', clearNumberLineHotspot);

                        // Check hot spot is absolute grading or partial grading
                        $numberLineHotspotProperties.find('input[type="radio"][name="grading"]').on('change', checkGradingMethod);

                        // Call properties of number line hot spot
                        $(document).on('click', '.numberline-hotspot', hotspotProperty);

                        // Trigger change input when click ckupnum or ckdownnum
                        $(document).on('click', '.ckUpDownNumber .ckUpNum, .ckUpDownNumber .ckDownNum', function() {
                            var $self = $(this);
                            $self.siblings('input[type="text"]').trigger('change');
                        });
                    },
                    onShow: function () {
                        // Refresh response id
                        refreshResponseId();
                        checkElementRemoveIntoIResult();

                        // Edit or create new number line
                        if (isEdit) {
                            loadDataNumberLine(elementNumberLine);
                        } else {
                            resetDataNumberLine();
                        }

                        // Hide tooltip
                        $('#tips .tool-tip-tips').hide();
                    }
                }]
            }],
            onOk: function() {
                var numberLineHtml = '';
                var hotspotHtml = '';
                var sourceItemArr = [];
                var correctResponseArr = [];
                var w = $numberLineWidth.val();
                var isSafari = /safari/i.test(navigator.userAgent) && /apple computer/i.test(navigator.vendor);
                var isAbsoluteGrading = $('#numberline-absoluting').is(':checked');
                var isAlgorithmicGrading = $('#numberline-algorithmic').is(':checked');

                // Check number line tool exist or not
                if (!$numberLinePreview.find('svg').length) {
                    customAlert('Please create number line tool first');
                    return false;
                }

                // Check number line tool have hot spot or not
                if (!$numberLinePreview.find('.numberline-hotspot').length) {
                    customAlert('Please add hot spot for number line tool');
                    return false;
                }

                // Show validation error absolute grading and partial grading
                if ($numberLinePreview.find('.numberline-hotspot').length < parseInt($numberLineMaxChoice.val(), 10)) {
                    showMessageValidChoiceError();
                    return false;
                }

                var starting = parseInt($('#numberline-starting-number').val(), 10);
                var ending = parseInt($('#numberline-ending-number').val(), 10);
                // Show validation start number and end number
                if (starting >= ending) {
                    customAlert('Ending Number must be greater than Starting Number');
                    return false;
                }

                if (isAbsoluteGrading) {
                    var $hotspotItemCorrect = $numberLinePreview.find('.numberline-hotspot[data-correct="true"]');

                    if (!$hotspotItemCorrect.length) {
                        customAlert('You should select at least one correct answer');
                        return false;
                    }
                } else if (!isAlgorithmicGrading) {
                    var bool = checkPointPartialGrading();

                    if (!bool) {
                        showMessageError();
                        return false;
                    }
                }

                // Build hot spot of number line
                $numberLinePreview.find('.numberline-hotspot').each(function(ind, hotspot) {
                    var $hotspot = $(hotspot);
                    var hsId = $hotspot.attr('id');
                    var hsPoint = $hotspot.attr('data-point');
                    var hsCorrect = JSON.parse($hotspot.attr('data-correct'));
                    var hsStyle = $hotspot.attr('style');
                    var hsTop;
                    var hsLeft;

                    if (isSafari) {
                        hsTop = $hotspot.css('top').replace('%', '');
                        hsLeft = $hotspot.css('left').replace('%', '');
                    } else {
                        hsTop = $hotspot.css('top').replace('px', '') / HEIGHT * 100;
                        hsLeft = $hotspot.css('left').replace('px', '') / w * 100;
                    }

                    hotspotHtml += '<div class="numberline-hotspot" identifier="' + hsId + '" ' +
                                    'data-correct="' + hsCorrect + '" ' +
                                    'data-point="' + hsPoint + '" ' +
                                    'style="' + hsStyle + '">&nbsp;</div>';

                    // Store sourceItem of iResult
                    sourceItemArr.push({
                        identifier: hsId,
                        top: hsTop,
                        left: hsLeft,
                        pointValue: hsPoint,
                        correct: hsCorrect
                    });

                    // Store correctResponse of iResult
                    // Absolute Grading is store correct hot spot
                    // Partial Grading is store point of click and point larger than 0
                    if (hsCorrect) {
                        correctResponseArr.push({
                            identifier: hsId,
                            pointValue: hsPoint
                        });
                    } else if (hsPoint > 0) {
                        correctResponseArr.push({
                            identifier: hsId,
                            pointValue: hsPoint
                        });
                    }
                });

                // Integrating SVG to CKEDITOR
                var outer = document.createElement('div');
                var $outer = $(outer);
                $numberLinePreview.find('svg').find('circle').remove();
                $outer.append($numberLinePreview.find('svg').clone(true));

                numberLineHtml += '<div id="RESPONSE_1" class="numberline-selection numberline-selection-interaction" ';
                numberLineHtml += 'style="width: ' + w + 'px; height: ' + HEIGHT + 'px;" contenteditable="false">';
                numberLineHtml += '<img class="cke_reset cke_widget_mask numberLineInteraction" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" />';
                numberLineHtml += $outer.html();
                numberLineHtml += hotspotHtml;
                numberLineHtml += '</div>&nbsp;';

                var numberlineElement = CKEDITOR.dom.element.createFromHtml(numberLineHtml);
                editor.insertElement(numberlineElement);

                // Store to iResult
                for (var i = 0, len = iResult.length; i < len; i++) {
                    var iResultItem = iResult[i];

                    if (iResultItem.type === 'numberLineHotSpot') {
                        iResultItem.source.width = w;
                        iResultItem.source.height = HEIGHT;
                        iResultItem.source.maxhotspot = $('#numberline-max-choice').val();
                        iResultItem.source.start = $('#numberline-starting-number').val();
                        iResultItem.source.end = $('#numberline-ending-number').val();
                        iResultItem.source.numberDivision = $('#numberline-divisions').val();

                        iResultItem.correctResponse = correctResponseArr;
                        iResultItem.sourceItem = sourceItemArr;
                        iResultItem.responseDeclaration.absoluteGrading = '0';
                        iResultItem.responseDeclaration.partialGrading = '0';
                        iResultItem.responseDeclaration.algorithmicGrading = '0';

                        if (isAbsoluteGrading) {
                            iResultItem.responseDeclaration.absoluteGrading = '1';
                        } else if (isAlgorithmicGrading) {
                            iResultItem.responseDeclaration.algorithmicGrading = '1';
                        } else {
                            iResultItem.responseDeclaration.partialGrading = '1';
                        }

                        iResultItem.responseDeclaration.pointsValue = $('#numberline-full-point').val();
                    }
                }

                // Store to newResult
                newResult = iResult;

                // Hide number line button
                $('.cke_button__numberlinehotspot').parents('.cke_toolbar').hide();

                if (isAlgorithmicGrading) {
                    TestMakerComponent.isShowAlgorithmicConfiguration = true;
                } else {
                    TestMakerComponent.isShowAlgorithmicConfiguration = false;
                }
            },
            onCancel: function() {
                var isAbsoluteGrading = $('#numberline-absoluting').is(':checked');
                var isAlgorithmicGrading = $('#numberline-algorithmic').is(':checked');

                if ($numberLinePreview.find('.numberline-hotspot').length) {
                    if (isAbsoluteGrading) {
                        if (!$numberLinePreview.find('.numberline-hotspot[data-correct="true"]').length) {
                            customAlert('You should select at least one correct answer');
                            return false;
                        }
                    } else if (!isAlgorithmicGrading) {
                        var bool = checkPointPartialGrading();

                        if (!bool) {
                            showMessageError();
                            return false;
                        }
                    }
                }

                isEdit = false;
            }
        };
    };

    function getUpDownNumber (selector, min, max) {
        var $selector = $(selector);

        $selector.ckUpDownNumber({
            minNumber: min,
            maxNumber: max,
            width: 18,
            height: 13
        });
    }

    function getUpDownNumberNumberline (selector, min, max) {
        var $selector = $(selector);

        $selector.ckUpDownNumberNumberLine({
            minNumber: min,
            maxNumber: max,
            width: 18,
            height: 13
        });
    }

    /**
     * Initialize Up and Down Number
     * @return {[type]} [description]
     */
    function initUpDownNumber() {
        // Width of number line tool
        getUpDownNumber($('#numberline-width'), 300, 600);

        // Number division of number line tool
        getUpDownNumber($('#numberline-divisions'), 1, 50);

        // Starting number of number line tool
        getUpDownNumberNumberline($('#numberline-starting-number'), -1000, 1000);

        // Ending number of number line tool
        getUpDownNumberNumberline($('#numberline-ending-number'), -1000, 1000);

        // Full points of number line question
        getUpDownNumber($('#numberline-full-point'), 0, 100);

        // Max choice of number line question
        getUpDownNumber($('#numberline-max-choice'), 1, 100);
    }

    /**
     * Button Action Called Number Line
     * @return {[type]} [description]
     */
    function calledNumberLine() {
        var w = parseInt($('#numberline-width').val(), 10);
        var starting = parseInt($('#numberline-starting-number').val(), 10);
        var ending = parseInt($('#numberline-ending-number').val(), 10);
        var divisions = parseInt($('#numberline-divisions').val(), 10);

        if (starting >= ending) {
            customAlert('Ending Number must be greater than Starting Number');
            return false;
        }

        createNumberLine(w, HEIGHT, starting, ending, divisions);

        // Clear all hot spot exist
        clearNumberLineHotspot();
        isShowBtnClearNumberLineHotspot();
    }

    /**
     * Create Number Line
     * @param  {[type]} w     [description]
     * @param  {[type]} h     [description]
     * @param  {[type]} start [description]
     * @param  {[type]} end   [description]
     * @param  {[type]} step  [description]
     * @return {[type]}       [description]
     */
    function createNumberLine(w, h, start, end, divisions) {
        // Reset number line preview
        $numberLinePreview
            .html('')
            .css({
                'width': w + 'px',
                'height': h + 'px',
                'margin': '0 auto'
            });

        // Draw number line tool
        createGraph('numberline-preview', w, h, start, end, divisions);
    }

    /**
     * Draw Number Line Tool Graph
     * @param  {[type]} el        [description]
     * @param  {[type]} w         [description]
     * @param  {[type]} h         [description]
     * @param  {[type]} start     [description]
     * @param  {[type]} end       [description]
     * @param  {[type]} divisions [description]
     * @return {[type]}           [description]
     */
    function createGraph(el, w, h, start, end, divisions) {
        var sketch = Raphael(el, w, h);
        var baseX = 30;
        var baseY = h / 2;
        var baseW = w - (2 * baseX);
        var points = sketch.set();
        var unitW = baseW / divisions;
        var r = 9;
        var unit = 6;

        sketch.setViewBox(0, 0, w, h);

        var linePath = [ ['M', baseX, baseY],['L', baseW + baseX, baseY] ];
        var line = sketch.path(linePath)
                            .attr({
                                'stroke': '#000',
                                'stroke-width': 2
                            });

        var arrowLeftPath = [ ['M', baseX, baseY], ['L', 0, baseY] ];
        var arrowLeftTopPath = [ ['M', 0.5, baseY], ['L', unit, baseY - unit] ];
        var arrowLeftBottomPath = [ ['M', 0.5, baseY], ['L', unit, baseY + unit] ];
        var arrowRightPath = [ ['M',  baseW + baseX, baseY], ['L', w, baseY] ];
        var arrowRightTopPath = [ ['M',  w - 0.5, baseY], ['L', w - unit, baseY - unit] ];
        var arrowRightBottomPath = [ ['M',  w - 0.5, baseY], ['L', w - unit, baseY + unit] ];

        sketch.path(arrowLeftPath)
                .attr({
                    'stroke': '#000',
                    'stroke-width': 2
                });

        sketch.path(arrowLeftTopPath)
                .attr({
                    'stroke': '#000',
                    'stroke-width': 2
                });

        sketch.path(arrowLeftBottomPath)
                .attr({
                    'stroke': '#000',
                    'stroke-width': 2
                });

        sketch.path(arrowRightPath)
                .attr({
                    'stroke': '#000',
                    'stroke-width': 2
                });

        sketch.path(arrowRightTopPath)
                .attr({
                    'stroke': '#000',
                    'stroke-width': 2
                });

        sketch.path(arrowRightBottomPath)
                .attr({
                    'stroke': '#000',
                    'stroke-width': 2
                });

        for (var i = 0; i < divisions + 1; i++) {
            var unitX = baseX + i * unitW;

            var unitPath = [ ['M', unitX, baseY - 5],['L', unitX, baseY + 5] ];
            var unit = sketch.path(unitPath)
                                .attr({
                                    'stroke': '#000',
                                    'stroke-width': 2
                                });

            var textValue = start + (i * (end - start) / divisions);

            if (!isInteger(textValue)) {
                textValue = textValue.toFixed(2).replace(/([0-9]+(\.[0-9]+[1-9])?)(\.?0+$)/, '$1');
            }

            var text = sketch.text(unitX, baseY + 25, textValue)
                            .attr({
                                'font-size': '12px',
                                'font-family': 'Verdana'
                            });

            points.push(unit);
        }

        var circle = sketch.circle(0, 0, r)
                            .attr({
                                'stroke-linecap': 'round',
                                'stroke-width': 2,
                                'stroke-dasharray': '-',
                                'cursor': 'pointer'
                            })
                            .hide();

        points
            .mouseover(function() {
                var that = this;
                var x = that.getBBox().x;
                var y = that.getBBox().y;

                circle
                    .show()
                    .attr({
                        'fill': 'rgba(0, 0, 0, .015)',
                        'cx': x,
                        'cy': y + r / 2
                    })
            });

        circle.click(createNumberLineHotSpot);
    }

    /**
     * Create Number Line Hot Spot
     * @param  {[type]} e [description]
     * @return {[type]}   [description]
     */
    function createNumberLineHotSpot(e) {
        var that = this;
        var wHs = 12;
        var hHs = 12;
        var r = 9;
        var w = $numberLinePreview.width();
        var h = $numberLinePreview.height();
        var x = (that.getBBox().x + r - wHs / 2) / w * 100;
        var y = (that.getBBox().y + r - hHs / 2) / h * 100;
        var hotspotID = 0;

        hotspotID = sequenceHotSpotID($numberLinePreview.find('.numberline-hotspot'), 'id', 'NHS_');

        var $hotspot = $('<span class="numberline-hotspot"/>')
                            .css({
                                'top': y + '%',
                                'left': x + '%'
                            })
                            .attr({
                                'id': hotspotID,
                                'data-correct': 'false',
                                'data-point': '0'
                            })
                            .appendTo($numberLinePreview);

        that.hide();
        isShowBtnClearNumberLineHotspot();
    }

    /**
     * Hot Spot Property
     * @return {[type]} [description]
     */
    function hotspotProperty(e) {
        var $self = $(this);
        $self.addClass('active');
        var $popupNumberLineProperty = $('#popup-numberline-property');
        var $popupNumberLineOverlay = $('#popup-numberline-overlay');
        var html = '';
        var isAbsoluteGrading = $('#numberline-absoluting').is(':checked');
        var isAlgorithmicGrading = $('#numberline-algorithmic').is(':checked');

        html += '\
                <div class="cke_dialog_body cke_dialog_property_numberline">\
                    <div class="cke_dialog_title">Property of the hot spot</div>\
                    <a type="image" title="Remove" class="cke_dialog_close_button" id="btn-numberline-property-close">\
                        <span class="cke_label">X</span>\
                    </a>\
                    <div class="hotspot-list">\
                        <div class="hotspot-list-item hotspot-list-item-property">\
                            <span class="widthLabel">Point Value:</span>\
                            <input type="text" value="0" name="numberline-point" id="numberline-point" class="txtFullcreate"/>\
                        </div>\
                        <div class="hotspot-list-item hotspot-list-item-property">\
                            <span class="widthLabel">Correct:</span>\
                            <input type="checkbox" name="numberline-correct" id="numberline-correct"/>\
                        </div>\
                    </div>\
                    <div class="cke_dialog_footer">\
                        <div class="cke_dialog_ui_hbox cke_dialog_footer_buttons">\
                            <div class="cke_dialog_ui_hbox_first" role="presentation">\
                                <a title="OK" id="btn-numberline-property-ok" class="cke_dialog_ui_button cke_dialog_ui_button_ok" role="button" type="hotpot">\
                                    <span class="cke_dialog_ui_button">OK</span>\
                                </a>\
                            </div>\
                            <div class="cke_dialog_ui_hbox_last" role="presentation">\
                                <a title="Cancel" id="btn-numberline-property-cancel" class="cke_dialog_ui_button cke_dialog_ui_button_cancel" role="button">\
                                    <span class="cke_dialog_ui_button">Cancel</span>\
                                </a>\
                            </div>\
                        </div>\
                    </div>\
                </div>';

        $popupNumberLineProperty
            .html(html)
            .show()
            .draggable({
                cursor: 'move',
                handle: '.cke_dialog_title'
            });

        $popupNumberLineOverlay.show();

        getUpDownNumber($('#numberline-point'), 0, 100);

        var $currentHotspot = $numberLinePreview.find('.numberline-hotspot.active');
        var tempPoint, tempChecked;
        tempPoint = parseInt($currentHotspot.attr('data-point'), 10);
        tempChecked = JSON.parse($currentHotspot.attr('data-correct'));

        $('#numberline-correct').prop('disabled', false);

        // Check status absolute grading or partial grading
        if (isAbsoluteGrading) {
            tempPoint = 0;
            $('#numberline-point').parents('.hotspot-list-item').hide();
            $('#numberline-correct').parents('.hotspot-list-item').show();
        } else if (isAlgorithmicGrading) {
            tempPoint = 0;
            tempChecked = false;
            $('#numberline-point').parents('.hotspot-list-item').hide();
            $('#numberline-correct').parents('.hotspot-list-item').show();
            $('#numberline-correct').prop('disabled', true);
        } else {
            $('#numberline-point').parents('.hotspot-list-item').show();
            $('#numberline-correct').parents('.hotspot-list-item').hide();
        }

        $('#numberline-point').val(tempPoint);
        $('#numberline-correct').prop('checked', tempChecked);

        // Cancel or close popup hot spot property
        $('#btn-numberline-property-close, #btn-numberline-property-cancel')
            .unbind('click')
            .on('click', function () {
                $popupNumberLineProperty.html('').hide();
                $popupNumberLineOverlay.hide();
                $numberLinePreview.find('.numberline-hotspot').removeClass('active');
            });

        // Save point or status checked of hot spot active
        $('#btn-numberline-property-ok')
            .unbind('click')
            .on('click', function() {
                var maxChoice = parseInt($('#numberline-max-choice').val(), 10);

                $currentHotspot.attr('data-point', parseInt($('#numberline-point').val(), 10));

                if ($('#numberline-correct').is(':checked') && isAbsoluteGrading) {
                    $currentHotspot.attr('data-correct', true);
                } else {
                    $currentHotspot.attr('data-correct', false);
                }

                if ($numberLinePreview.find('.numberline-hotspot[data-correct="true"]').length > maxChoice) {
                    $currentHotspot.attr('data-correct', false);
                    customAlert('Please increase the maximum number of hot spots a student can select.');
                    return;
                }

                $numberLinePreview.find('.numberline-hotspot').removeClass('active');
                $popupNumberLineProperty.html('').hide();
                $popupNumberLineOverlay.hide();
            });
    }

    /**
     * Clear Number Line Hot Spot
     * @return {[type]} [description]
     */
    function clearNumberLineHotspot() {
        var $self = $(this);
        $numberLinePreview.find('.numberline-hotspot').remove();
        $self.addClass('is-visible');
    }

    /**
     * Load Data Number Line Hot Spot
     * @param  {[type]} el [description]
     * @return {[type]}    [description]
     */
    function loadDataNumberLine(el) {
        var $el = $(el);
        var hotspotHtml = '';
        var w;
        var start;
        var end;
        var divisions;
        var sourceItemArr = [];

        // Load Data Number Line
        for (var i = 0, len = iResult.length; i < len; i++) {
            var iResultItem = iResult[i];

            if (iResultItem.type === 'numberLineHotSpot' && iResultItem.responseIdentifier === $el.attr('id')) {
                w = parseInt(iResultItem.source.width, 10);
                start = parseInt(iResultItem.source.start, 10);
                end = parseInt(iResultItem.source.end, 10);
                divisions = parseInt(iResultItem.source.numberDivision, 10);

                $numberLineWidth.val(w);
                $numberLineDivisions.val(divisions);
                $numberLineStarting.val(start);
                $numberLineEnding.val(end);
                $numberLineMaxChoice.val(iResultItem.source.maxhotspot);
                $numberLineFullPoints.val(iResultItem.responseDeclaration.pointsValue);

                $numberLineDialog.find('#numberline-full-point').parent().removeClass('is-disabled');

                if (iResultItem.responseDeclaration.absoluteGrading === '1') {
                    $numberLineHotspotProperties.find('input[type="radio"][id="numberline-absoluting"]').prop('checked', true);
                } else if (iResultItem.responseDeclaration.algorithmicGrading === '1') {
                    $numberLineHotspotProperties.find('input[type="radio"][id="numberline-algorithmic"]').prop('checked', true);
                    $numberLineDialog.find('#numberline-full-point').parent().addClass('is-disabled');
                } else {
                    $numberLineHotspotProperties.find('input[type="radio"][id="numberline-partial"]').prop('checked', true);
                }

                // Store sourceItem
                sourceItemArr = iResultItem.sourceItem;
            }
        }

        // Build html for hot spot
        for (var si = 0, siLen = sourceItemArr.length; si < siLen; si++) {
            var sourceItemChild = sourceItemArr[si];
            var scId = sourceItemChild.identifier;
            var scTop = sourceItemChild.top;
            var scLeft = sourceItemChild.left;
            var scCorrect = sourceItemChild.correct;
            var scPointValue = sourceItemChild.pointValue;
            hotspotHtml += '<span class="numberline-hotspot" id="' + scId + '" data-correct="' + scCorrect + '" data-point="' + scPointValue + '" style="top: ' + scTop + '%; left: ' + scLeft + '%;"></span>';
        }

        $btnClearNumberLineHotspot.removeClass('is-visible');

        $numberLinePreview
            .html('')
            .css({
                'width': w + 'px',
                'height': HEIGHT + 'px',
                'margin': '0 auto'
            });

        createGraph('numberline-preview', w, HEIGHT, start, end, divisions);

        $numberLinePreview.append(hotspotHtml);
    }

    /**
     * Reset Number Line Hot Spot
     * @return {[type]} [description]
     */
    function resetDataNumberLine() {
        // Reset default all properties
        $numberLineWidth.val('300');
        $numberLineDivisions.val('1');
        $numberLineStarting.val('0');
        $numberLineEnding.val('1');
        $numberLineFullPoints.val('1');
        $numberLineMaxChoice.val('1');
        $numberLineHotspotProperties.find('input[type="radio"][id="numberline-absoluting"]').prop('checked', true);
        $btnClearNumberLineHotspot.addClass('is-visible');
        $numberLinePreview
            .html('')
            .css({
                'width': 'auto',
                'height': 'auto'
            });

        // Create initialize number line
        var w = 300;
        var h = HEIGHT;
        var start = 0;
        var end = 1;
        var divisions = 1;
        createNumberLine(w, h, start, end, divisions);
    }

    /**
     * Check Type Grading Is Absolute Grading or Partial Grading
     * @param  {[type]} e [description]
     * @return {[type]}   [description]
     */
    function checkGradingMethod (e) {
        var $grading = $(this);
        var gradingMethod = $grading.attr('id');

        $numberLineDialog.find('#numberline-full-point').parent().removeClass('is-disabled');
        $numberLineDialog.find('#numberline-full-point').val('1');

        if (gradingMethod === 'numberline-partial') {
            $numberLinePreview.find('.numberline-hotspot').attr('data-correct', false);
        } else if (gradingMethod === 'numberline-algorithmic') {
            $numberLinePreview.find('.numberline-hotspot').attr('data-correct', false);
            $numberLinePreview.find('.numberline-hotspot').attr('data-point', 0);
            $numberLineDialog.find('#numberline-full-point').parent().addClass('is-disabled');
            $numberLineDialog.find('#numberline-full-point').val('0');
        } else {
            $numberLinePreview.find('.numberline-hotspot').attr('data-point', 0);
        }
    }

    /**
     * Check hidden or show button clear number line hot spot
     * @return {Boolean} [description]
     */
    function isShowBtnClearNumberLineHotspot() {
        if ($numberLinePreview.find('.numberline-hotspot').length > 0) {
            $btnClearNumberLineHotspot.removeClass('is-visible');
        } else {
            $btnClearNumberLineHotspot.addClass('is-visible');
        }
    }

    /**
     * Show Message Error
     * @return {[type]} [description]
     */
    function showMessageError() {
        customAlert('Students will not be able to earn the maximum points possible on this question'
                + 'based on the current point allocation.'
                + ' You can 1) reduce the total points possible on the item,'
                + ' 2) increase the maximum number of hot spots a student can select,'
                + ' and/or 3) increase the points earned by certain hot spots.');
    }

    /**
     * Show Message Valid Choice Error
     * @return {[type]} [description]
     */
    function showMessageValidChoiceError() {
        customAlert('Maximum hot spots that can be selected cannot be '
                + 'greater than the total number of hot spots in the item.');
    }

    /**
     * Check Point Partial Grading
     * @return {[type]} [description]
     */
    function checkPointPartialGrading() {
        var totalPoint = 0;
        var pointArr = [];
        var totalMaxSelected = 0;
        var result = false;
        var fullMaxChoice = parseInt($('#numberline-max-choice').val(), 10);
        var fullPoints = parseInt($('#numberline-full-point').val(), 10);
        var largest = 0;

        $numberLinePreview.find('.numberline-hotspot').each(function(ind, hotspot) {
            var $hotspot = $(hotspot);
            var point = parseInt($hotspot.attr('data-point'), 10);

            totalPoint += point;

            if (point > 0) {
                pointArr.push(point);
            }
        });

        for (var i = 0; i < fullMaxChoice; i++) {
            if (pointArr.length) {
                largest = Math.max.apply(Math, pointArr);
                totalMaxSelected += largest;

                for (var temp = 0, lenPoint = pointArr.length; temp < lenPoint; temp++) {
                    if (largest === pointArr[temp]) {
                        pointArr.splice(temp, 1);
                        break;
                    }
                }
            }
        }

        var isPoint = totalPoint >= fullPoints;
        var isMaxPoint = totalMaxSelected >= fullPoints;

        result = isPoint && isMaxPoint;

        return result;
    }

    /**
     * Sequence Hot Spot ID
     * @param  {[type]} element     [description]
     * @param  {[type]} elementAttr [description]
     * @param  {[type]} prefix      [description]
     * @return {[type]}             [description]
     */
    function sequenceHotSpotID(element, elementAttr, prefix) {
        var $element = $(element);
        var len = $element.length;
        var srcId = prefix + (len + 1);

        for (var m = 0; m < len; m++) {
            var resId = prefix + (m + 1);
            var isOnlyOne = true;
            var elM = $element.eq(m).attr(elementAttr);

            if (elM != resId) {
                for (var k = 0; k < len; k++) {
                    var elK = $element.eq(k).attr(elementAttr);
                    if (resId == elK) {
                        isOnlyOne = false;
                    }
                }

                if (isOnlyOne) {
                    srcId = resId;
                    break;
                }
            }
        }

        return srcId;
    }

    /**
     * Check number is integer or not
     * @param  {[type]}  x [description]
     * @return {Boolean}   [description]
     */
    function isInteger(x) {
        return x % 1 === 0;
    }

    //Plugin for update and down number for number line
    ;(function ($, window, document, undefined) {

        var pluginName = "ckUpDownNumberNumberLine",
            defaults = {
                height: 32,
                width: 24,
                minNumber: -9000,
                maxNumber: 9000
            };

        function Plugin(element, options) {
            this.element = element;

            this.settings = $.extend({}, defaults, options);

            this._defaults = defaults;
            this._name = pluginName;
            this.init();
        }

        // Avoid Plugin.prototype conflicts
        $.extend(Plugin.prototype, {

            init: function () {
                this.generate();
                this.change();
            },

            generate: function () {
                var self = this;
                var el = self.element;
                var $el = $(el);

                $el
                    .attr({
                        'maxNumber': self.settings.maxNumber,
                        'minNumber': self.settings.minNumber
                    })
                    .wrap("<span class='ckUpDownNumber' style='height:" + self.settings.height + "px;'></span>")
                    .parent()
                    .append('<input class="ckbutton ckUDNumber ckUpNum' +
                        '" style="width: ' + self.settings.width + 'px;height: '
                        + self.settings.height + 'px" type="button" value="&#9650;" />' +
                        '<input class="ckbutton ckUDNumber ckDownNum' +
                        '" type="button" style="width: ' + self.settings.width +
                        'px;height: ' + self.settings.height + 'px" value="&#9660;" />');

                $el.siblings('.ckUpNum').unbind('click').on('click', function (event) {
                    event.preventDefault();
                    var elValue = parseInt($el.val(), 10);

                    if (elValue >= $el.attr('maxnumber')) {
                        return;
                    }

                    if (elValue == '') {
                        $el.val('0');
                    }

                    $el.val(elValue + 1);
                    //trigger auto change for resize content area
                    $el.parent('.ckUpDownNumber').find('#nHeight').trigger('change.height');
                    $el.parent('.ckUpDownNumber').find('#nWidth').trigger('change.width');
                });

                $el.siblings('.ckDownNum').unbind('click').on('click', function (event) {
                    event.preventDefault();
                    var elValue = parseInt($el.val(), 10);

                    if ($el.attr('minnumber') >= elValue) {
                        return;
                    }

                    $el.val(elValue - 1);
                    //trigger auto change for resize content area
                    $el.parent('.ckUpDownNumber').find('#nHeight').trigger('change.height');
                    $el.parent('.ckUpDownNumber').find('#nWidth').trigger('change.width');
                });
            },
            change: function () {
                var self = this;
                var el = self.element;
                var $el = $(el);

                $el.on('keydown', function (event) {
                    var $that = $(this);
                    var valNumber = $that.val();
                    var valStart = valNumber.slice(0, 1);
                    var isDisabled = true;

                    if (valNumber.length > 1) {
                        var valEnd = valNumber.slice(1, valNumber.length - 1);

                        if (valEnd.indexOf('-') === -1) {
                            isDisabled = false;
                        }
                    }

                    // Allow: backspace, delete, tab, escape, enter and .
                    if ($.inArray(event.keyCode, [46, 8, 9, 27, 13]) !== -1 ||
                        // Allow: Ctrl+A
                        (event.keyCode == 65 && event.ctrlKey === true) ||
                        // Allow: home, end, left, right
                        (event.keyCode >= 35 && event.keyCode <= 39) ||
                        // Allow: - negative number
                        (event.keyCode == 189 && valStart != '-' && isDisabled)) {
                        // let it happen, don't do anything
                        return;
                    }
                    else {
                        // Ensure that it is a number and stop the keypress
                        if (event.shiftKey || (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)) {
                            event.preventDefault();
                        }
                    }
                });

                $el.on('change', function () {
                    var $that = $(this);
                    var iVal = $that.val();
                    if (parseInt(iVal) > self.settings.maxNumber) {
                        iVal = self.settings.maxNumber;
                    } else if (parseInt(iVal) < self.settings.minNumber) {
                        iVal = self.settings.minNumber;
                    }
                    $that.val(iVal);
                });
            }
        });

        // A really lightweight plugin wrapper around the constructor,
        // preventing against multiple instantiations
        $.fn[pluginName] = function (options) {
            this.each(function () {
                if (!$.data(this, "plugin_" + pluginName)) {
                    $.data(this, "plugin_" + pluginName, new Plugin(this, options));
                }
            });

            // chain jQuery functions
            return this;
        };

    })(jQuery, window, document);
})();
