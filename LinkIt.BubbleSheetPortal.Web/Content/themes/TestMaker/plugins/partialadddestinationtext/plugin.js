CKEDITOR.plugins.add('partialadddestinationtext', {
    lang: 'en', // %REMOVE_LINE_CORE%
    icons: 'partialadddestinationtext',
    requires: 'dialog',
    hidpi: true, // %REMOVE_LINE_CORE%
    init: function (editor) {
        var pluginName = 'insertPartialAddDestinationText';
        var eleAddDestinationText = "";

        editor.addCommand(pluginName, new CKEDITOR.dialogCommand(pluginName));

        editor.ui.addButton('PartialAddDestinationText', {
            label: 'Destination Text Label',
            command: pluginName,
            icon: this.path + 'icons/audio.png',
            toolbar: 'insertPartialAddDestinationText,30'
        });

        editor.widgets.add('partialadddestinationtext', {
            inline: true,
            mask: true
        });

        editor.on('doubleclick', function (evt) {
            var element = evt.data.element;

            if (element.hasClass('partialAddDestinationTextMark')) {
                var parents = element.getParents();
                var parent;

                for (var i = 0; i < parents.length; i++) {
                    parent = parents[i];
                    if (parent.hasClass('partialAddDestinationText')) {
                        break;
                    }
                }

                // Move selection to parent of multipleChoiceMark
                eleAddDestinationText = parent;
                editor.getSelection().selectElement(eleAddDestinationText);
                evt.data.dialog = pluginName;
            }
        });

        CKEDITOR.dialog.add(pluginName, function (editor) {
            var destinationTextDialog = '\
                <style>\
                    .cke_dialog_contents_body { \
                        overflow: visible !important; \
                    }\
                    .cke_dialog_ui_vbox_child { \
                        padding: 5px; \
                    }\
                    #cke_sourceTextDestination { \
                        margin: 10px 0; \
                    }\
                    .ckeditor-destination { \
                        width: 100%; \
                        min-height: 150px; \
                    }\
                </style>\
                <div id="audioUploadContent">\
                    <table cellspacing="0" border="0" align="left" style="width:100%;" role="presentation">\
                        <tbody>\
                            <tr>\
                                <td>\
                                    Width: <input type="text" id="sourceTextDestinationWidth" value="120"/>&nbsp;&nbsp;&nbsp;\
                                    Height: <input type="text" id="sourceTextDestinationHeight" value="40"/>&nbsp;&nbsp;&nbsp;\
                                    <div class="fright u-m-t-5">\
                                        <button class="cke_dialog_ui_button js-btn-add-threshold">\
                                            <span class="cke_dialog_ui_button">Add Threshold</span>\
                                        </button>\
                                    </div>\
                                </td>\
                            </tr>\
                            <tr>\
                                <td class="p-t-10">\
                                    Maximum number of draggable objects for this destination is: <input type="text" class="txtFullcreate" id="sourceTextDestinationNumberDroppable" value="1"/>\
                                </td>\
                            </tr>\
                            <tr>\
                                <td class="p-t-10">\
                                    <div class="thresholds">\
                                        <h3>List threshold</h3>\
                                        <table class="table-threshold">\
                                            <thead>\
                                                <tr>\
                                                    <th>Low</th>\
                                                    <th>High</th>\
                                                    <th>Points</th>\
                                                    <th>{deleteColumnTitle}</th>\
                                                </tr>\
                                            </thead>\
                                            <tbody></tbody>\
                                        </table>\
                                    </div>\
                                </td>\
                            </tr>\
                            <tr>\
                                <td>\
                                    <br/><p>Value:</p><br/>\
                                    <textarea id="sourceTextDestination" class="ckeditor-destination"></textarea>\
                                </td>\
                            </tr>\
                            <tr>\
                                <td>\
                                    <fieldset class="fieldsetTextField">\
                                        <div class="d-flex flex-row justify-content-between align-items-baseline">\
                                          <legend>Select Correct Answer</legend>\
                                          <label for="require_all_answers" class="form-label m-0"><input type="checkbox" name="require_all_answers" id="require_all_answers" class="form-checkbox" checked> Require all answer(s)</label>\
                                        </div>\
                                        <div class="correctAnswer destinationAnswerList"><ul id="correctAnswer" class="correctAnswer">\
                                        </ul></div>\
                                    </fieldset>\
                                </td>\
                            </tr>\
                        </tbody>\
                    </table>\
                </div>';
            destinationTextDialog = destinationTextDialog.replace('{deleteColumnTitle}', IS_V2 ? 'Delete' : '');

            /**
             * Get up and down number
             * @param  {[type]} selector [description]
             * @param  {[type]} min      [description]
             * @param  {[type]} max      [description]
             * @return {[type]}          [description]
             */
            function getUpDownNumber(selector, min, max) {
                var $selector = $(selector);

                $selector.ckUpDownNumber({
                    minNumber: min,
                    maxNumber: max,
                    width: 18,
                    height: 13
                });
            }

            /**
             * Get dimension width or height source from target
             * @param  {[type]} source    [description]
             * @param  {[type]} target    [description]
             * @param  {[type]} dimension [description]
             * @param  {[type]} style     [description]
             * @return {[type]}           [description]
             */
            function getDimensions(source, target, dimension, style) {
                var $source = $(source);

                $source.focusout(function () {
                    var $self = $(this);
                    var value = $self.val();

                    if (value === '') {
                        $self.val(dimension);
                    }
                }).val(target.getStyle(style).replace('px', ''));
            }

            /**
             * Build list item threshold
             * @param  {[type]} container [description]
             * @param  {[type]} point     [description]
             * @return {[type]}           [description]
             */
            function addItemThreshold(container) {
                var $container = $(container);

                $container.append(createItemThreshold());

                $container.find('.js-threshold-remove').on('click', function () {
                    removeItemThreshold(this);
                });

                getOnlyNumberic($container.find('td input[type="text"]'));
                supportTabThreshold($container);
            }

            /**
             * Create item threshold from point
             * @param  {[type]} point [description]
             * @return {[type]}       [description]
             */
            function createItemThreshold() {
                var item = document.createElement('tr');

                // Build the inner HTML of our new item
                var html = '';
                html += '<td><input type="text" name="min-threshold"/></td>';
                html += '<td><input type="text" name="max-threshold"/></td>';
                html += '<td><input type="text" name="point-threshold"/></td>';
                html += '<td><span class="icon icon-remove-threshold js-threshold-remove" title="Remove threshold"></span></td>';

                item.innerHTML = html;

                return item;
            }

            /**
             * Remove item threshold
             * @return {[type]} [description]
             */
            function removeItemThreshold(el) {
                var $el = $(el);
                var $elItem = $el.parent('td').parent('tr');

                $elItem.remove();
            }

            /**
             * Fill item threshold point
             * @param  {[type]} container       [description]
             * @param  {[type]} thresholdpoints [description]
             * @return {[type]}                 [description]
             */
            function fillItemThreshold(container, thresholdpoints) {
                var $container = $(container);

                for (var i = 0, len = thresholdpoints.length; i < len; i++) {
                    $container.append(createItemThreshold());
                }

                $container.find('tr').each(function (ind, tr) {
                    var $tr = $(tr);

                    $tr.find('input[name="min-threshold"]').val(thresholdpoints[ind].low);
                    $tr.find('input[name="max-threshold"]').val(thresholdpoints[ind].hi);
                    $tr.find('input[name="point-threshold"]').val(thresholdpoints[ind].pointsvalue);
                });

                $container.find('.js-threshold-remove').on('click', function () {
                    removeItemThreshold(this);
                });

                getOnlyNumberic($container.find('td input[type="text"]'));
                supportTabThreshold($container);
            }

            /**
             * Accept numeric when keydown
             * @param  {[type]} target [description]
             * @return {[type]}         [description]
             */
            function getOnlyNumberic(target) {
                $(target).on('keydown', function (event) {
                    var allowedKey = [46, 8, 9, 27, 13];
                    // Allow: backspace, delete, tab, escape, enter and .
                    if ($.inArray(event.keyCode, allowedKey) !== -1 ||
                        // Allow: Ctrl+A
                        (event.keyCode === 65 && event.ctrlKey === true) ||
                        // Allow: home, end, left, right
                        (event.keyCode >= 35 && event.keyCode <= 39)) {
                        // let it happen, don't do anything
                        return;
                    }

                    // Ensure that it is a number and stop the keypress
                    if (event.shiftKey || (event.keyCode < 48 || event.keyCode > 57) &&
                        (event.keyCode < 96 || event.keyCode > 105)) {
                        event.preventDefault();
                    }
                });
            }

            /**
             * Support tab in dialog
             * @param  {[type]} element [description]
             * @return {[type]}         [description]
             */
            function supportTabThreshold(element) {
                var $element = $(element);

                $element.on('keydown', 'input[type="text"]', function (e) {
                    var focusable;
                    var next;

                    if (e.keyCode === 9) {
                        focusable = $element.find('input[type="text"]').filter(':visible');
                        next = focusable.eq(focusable.index(this) + 1);

                        if (next.length) {
                            next.focus();
                        }

                        return false;
                    }
                });
            }

            return {
                title: 'Drag and Drop - Destination Text Field',
                minWidth: IS_V2 ? 500 : 350,
                minHeight: 150,
                resizable: CKEDITOR.DIALOG_RESIZE_NONE,
                contents: [{
                    id: 'imageUploadExe',
                    label: 'Settings',
                    elements: [{
                        type: 'html',
                        html: destinationTextDialog,
                        onLoad: function () {
                            var $dialog = $(this.getDialog().getElement().$);

                            // Initialize up and down number of destination text
                            getUpDownNumber('#sourceTextDestinationWidth', 1, 1000);
                            getUpDownNumber('#sourceTextDestinationHeight', 1, 1000);
                            getUpDownNumber('#sourceTextDestinationNumberDroppable', 1, 100);

                            $dialog.find('.js-btn-add-threshold').on('click', function () {
                                addItemThreshold('.table-threshold tbody');
                            });
                        },
                        onShow: function () {
                            var $dialog = $(this.getDialog().getElement().$);
                            var $correctAnswer = $('.correctAnswer #correctAnswer');
                            var $sourceDesNumberDroppable = $('#sourceTextDestinationNumberDroppable');
                            var lenIResult = iResult.length;
                            var isThresholdGrading = iResult[0].responseDeclaration.thresholdGrading;
                            var isAlgorithmicGrading = iResult[0].responseDeclaration.algorithmicGrading;
                            var isRelativeGrading = iResult[0].responseDeclaration.relativeGrading;
                            var isAbsoluteGrading = iResult[0].responseDeclaration.absoluteGrading === '1' || (isThresholdGrading==='0' && isAlgorithmicGrading==='0' && isRelativeGrading==='0');
                            var thresholdPoints = [];

                            // Initialize CKEditor for destination text
                            if (!CKEDITOR.instances.sourceTextDestination) {
                                CKEDITOR.replace('sourceTextDestination', {
                                    height: '150px',
                                    width: '100%',
                                    extraPlugins: 'mathfraction,mathjax',
                                    toolbar: [
                                        ['Mathjax']
                                    ],
                                    removePlugins: 'elementspath',
                                    resize_enabled: false,
                                    autoGrow_minHeight: 150,
                                    autoGrow_maxHeight: 400,
                                    autoGrow_bottomSpace: 50,
                                    allowedContent: true
                                });
                            }

                            // Set content if editing existing destination
                            if (eleAddDestinationText) {
                                var contentElement = eleAddDestinationText.$.cloneNode(true);
                                $(contentElement).find('.partialAddDestinationTextMark').remove();
                                if (contentElement.childElementCount === 1 && contentElement.childNodes[0].tagName === 'SPAN') {
                                    contentElement = contentElement.childNodes[0];
                                }
                                CKEDITOR.instances.sourceTextDestination.setData(loadMathML(contentElement.innerHTML));
                            }

                            var i = 0;
                            var n = 0;
                            refreshPartialCredit();

                            // Clear the answer before append
                            $correctAnswer.empty();
                            $dialog.find('.thresholds tbody').empty();

                            // Check drag drop is Absolute Grading
                            if (isThresholdGrading === '1') {
                                $dialog.find('.js-btn-add-threshold').removeClass('visuallyhidden');
                                $dialog.find('.thresholds').removeClass('visuallyhidden');
                                if (IS_V2) {
                                  $dialog.find('.js-btn-add-threshold').removeClass('d-none');
                                }
                            } else {
                                $dialog.find('.js-btn-add-threshold').addClass('visuallyhidden');
                                $dialog.find('.thresholds').addClass('visuallyhidden');
                                if (IS_V2) {
                                  $dialog.find('.js-btn-add-threshold').addClass('d-none');
                                }
                            }

                            // Check if iResult exist
                            if (lenIResult > 0) {
                                var answerArr = [];
                                var iResultItem;

                                // Load answer to list for user choice then append to "correctAnswer" id
                                for (i = 0; i < lenIResult; i++) {
                                    iResultItem = iResult[i];

                                    if (iResultItem.type === 'partialCredit') {
                                        for (n = 0; n < iResultItem.source.length; n++) {
                                            var iResultItemPartial = iResultItem.source[n];
                                            var srcPartial = iResultItemPartial.srcIdentifier;
                                            var valuePartial = iResultItemPartial.value;

                                            if (iResultItemPartial.type == 'image') {
                                                valuePartial = $('<div />')
                                                    .append(valuePartial)
                                                    .find('img')
                                                    .removeAttr('style')
                                                    .prop('outerHTML');
                                            }

                                            answerArr.push('<li srcIdentifier="' + srcPartial + '"><label for="' + srcPartial + '" class="form-label"><input type="checkbox" name="src" id="' + srcPartial + '" value="' + srcPartial + '" class="form-checkbox" /> ' + valuePartial + '</label></li>');
                                        }
                                    }
                                }

                                $correctAnswer.append(answerArr.join(''));

                                // This is show the answer has been choice before
                                for (i = 0; i < lenIResult; i++) {
                                    iResultItem = iResult[i];

                                    if (iResultItem.type === 'partialCredit') {
                                        for (n = 0; n < iResultItem.correctResponse.length; n++) {
                                            if (iResultItem.correctResponse[n].destIdentifier == eleAddDestinationText.getAttribute('destidentifier')) {
                                                var answerSrc = iResultItem.correctResponse[n].srcIdentifier.split(';');

                                                // Check if answer do not select correct answer
                                                if (answerSrc[0] != '') {
                                                    for (var m = 0, lenAnswerSrc = answerSrc.length; m < lenAnswerSrc; m++) {
                                                        $('#correctAnswer li input[id=' + answerSrc[m] + ']').prop('checked', true);
                                                    }
                                                }

                                                if (iResultItem.correctResponse[n].thresholdpoints !== undefined) {
                                                    thresholdPoints = iResultItem.correctResponse[n].thresholdpoints;
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            getDimensions('#sourceTextDestinationWidth', eleAddDestinationText, 120, 'width');
                            getDimensions('#sourceTextDestinationHeight', eleAddDestinationText, 40, 'height');
                            $('#sourceTextDestination').val(eleAddDestinationText.getText());

                            var numberDroppable = parseInt($(eleAddDestinationText).attr('numberDroppable'), 10);

                            if (isNaN(numberDroppable)) {
                                $sourceDesNumberDroppable.val('1');
                            } else {
                                $sourceDesNumberDroppable.val(numberDroppable);
                            }
                            if (isAbsoluteGrading) {
                                var notRequireAllAnswers = parseInt($(eleAddDestinationText).attr('notRequireAllAnswers'), 10);
                                if (notRequireAllAnswers === 1) {
                                  $dialog.find('#require_all_answers').prop('checked', false);
                                } else {
                                  $dialog.find('#require_all_answers').prop('checked', true);
                                }
                                $dialog.find('#require_all_answers').parent().show();
                            } else {
                              $dialog.find('#require_all_answers').prop('checked', true);
                              $dialog.find('#require_all_answers').parent().hide();
                            }
                            // Hide tooltip
                            $('#tips .tool-tip-tips').css('display', 'none');

                            if (thresholdPoints.length) {
                                fillItemThreshold($dialog.find('.table-threshold tbody'), thresholdPoints);
                            }

                            if (isAlgorithmicGrading === '1') {
                                $dialog.find('.correctAnswer input[type="checkbox"]').prop('disabled', true);
                            } else {
                                $dialog.find('.correctAnswer input[type="checkbox"]').prop('disabled', false);
                            }
                            MathJax.Hub.Queue(['Typeset', MathJax.Hub]);
                        },
                        onHide: function() {
                            // Destroy CKEditor instance when dialog is closed
                            if (CKEDITOR.instances.sourceTextDestination) {
                                CKEDITOR.instances.sourceTextDestination.destroy();
                            }
                        }
                    }]
                }],
                onOk: function () {
                    var $dialog = $(this.getElement().$);
                    // Get content from CKEditor
                    var desText = CKEDITOR.instances.sourceTextDestination.getData();
                    var desWidth = $('#sourceTextDestinationWidth').val();
                    var desHeight = $('#sourceTextDestinationHeight').val();
                    var desNumberDroppable = parseInt($('#sourceTextDestinationNumberDroppable').val(), 10);
                    var destIdentifier = eleAddDestinationText.getAttribute('destidentifier');
                    var srcId = '';
                    var answerCheckedCount = 0;
                    var msgNotification = '';
                    var isThresholdGrading = iResult[0].responseDeclaration.thresholdGrading;
                    var thresholdPointsTemp = [];
                    var thresholdPoints = [];
                    //to reduce the migration old data load, decided to set the default state as opposed to the checkbox state
                    var notRequireAllAnswers = $dialog.find('#require_all_answers').prop('checked') ? '0' : '1';

                    // Check selected answer
                    $('#correctAnswer li input[type=checkbox]').each(function () {
                        var $self = $(this);

                        if ($self.is(':checked')) {
                            answerCheckedCount++;
                            srcId = srcId.concat($self.attr('id'), ';');
                        }
                    });

                    // Check item threshold drag and drop
                    if (isThresholdGrading === '1') {
                        var isValidate = false;
                        var isOverlap = false;

                        $dialog.find('.thresholds tbody tr').each(function () {
                            var $tr = $(this);
                            var minThreshold = parseInt($tr.find('input[name="min-threshold"]').val(), 10);
                            var maxThreshold = parseInt($tr.find('input[name="max-threshold"]').val(), 10);
                            var pointThreshold = parseInt($tr.find('input[name="point-threshold"]').val(), 10);

                            if (isNaN(minThreshold) || isNaN(maxThreshold) || isNaN(pointThreshold)) {
                                $tr.addClass('is-not-validate');
                                msgNotification = 'Please input value for all fields.';
                                isValidate = true;
                                return false;
                            }

                            if (minThreshold === 0) {
                                $tr.addClass('is-not-validate');
                                msgNotification = 'Low value must be greater than 0.';
                                isValidate = true;
                                return false;
                            }

                            if (minThreshold > maxThreshold) {
                                $tr.addClass('is-not-validate');
                                msgNotification = 'High value must be larger than Low value.';
                                isValidate = true;
                                return false;
                            }

                            if (maxThreshold > answerCheckedCount) {
                                $tr.addClass('is-not-validate');
                                msgNotification = 'Maximum High value cannot exceed the maximum number of correct answer.';
                                isValidate = true;
                                return false;
                            }

                            if (maxThreshold > desNumberDroppable) {
                                $tr.addClass('is-not-validate');
                                msgNotification = 'Maximum High value cannot exceed the maximum number of draggable objects for this destination.';
                                isValidate = true;
                                return false;
                            }

                            $tr.removeClass('is-not-validate');

                            if (thresholdPointsTemp.length > 0) {
                                $.each(thresholdPointsTemp, function (index, value) {
                                    if ((value.low <= minThreshold && minThreshold <= value.hi) ||
                                        (value.low <= maxThreshold && maxThreshold <= value.hi)) {
                                        $tr.addClass('is-not-validate');
                                        isOverlap = true;
                                    } else {
                                        thresholdPointsTemp.push({
                                            low: minThreshold,
                                            hi: maxThreshold,
                                            pointsvalue: pointThreshold
                                        });
                                    }
                                });
                            } else {
                                thresholdPointsTemp.push({
                                    low: minThreshold,
                                    hi: maxThreshold,
                                    pointsvalue: pointThreshold
                                });
                            }

                            thresholdPoints.push({
                                low: minThreshold,
                                hi: maxThreshold,
                                pointsvalue: pointThreshold
                            });
                        });

                        // Check if value you input do not validate
                        if (isValidate) {
                            customAlert(msgNotification);
                            return false;
                        }

                        // Check if threshold is overlapped
                        if (isOverlap) {
                            msgNotification = 'The threshold range should not be overlapped together in same item threshold.';
                            customAlert(msgNotification);
                            return false;
                        }
                    } else {
                        if (answerCheckedCount > desNumberDroppable && notRequireAllAnswers === '0') {
                            msgNotification = 'Please increase the maximum number of hot spots a student can select.';
                            customAlert(msgNotification);
                            return false;
                        }
                    }

                    var correctAnswerAvaiable = $('#correctAnswer li input[type=checkbox]').length;
                    if (correctAnswerAvaiable < desNumberDroppable) {
                        msgNotification = 'Maximum hot spots that can be selected cannot be greater than the total number of hot spots in the item.';
                        customAlert(msgNotification);
                        return false;
                    }

                    // Update answer and save to iResult
                    for (var i = 0, lenIResult = iResult.length; i < lenIResult; i++) {
                        var iResultItem = iResult[i];

                        if (iResultItem.type === 'partialCredit') {
                            for (var n = 0; n < iResultItem.correctResponse.length; n++) {
                                if (destIdentifier === iResultItem.correctResponse[n].destIdentifier) {
                                    iResultItem.correctResponse[n].srcIdentifier = srcId.slice(0, -1);
                                    iResultItem.correctResponse[n].thresholdpoints = thresholdPoints;
                                }
                            }
                        }
                    }

                    if (parseInt(isThresholdGrading)) {
                        var pointsValue = 0;
                        for (var i = 0; i < iResult[0].correctResponse.length; i++) {
                            var crItem = iResult[0].correctResponse[i];
                            if (crItem.thresholdpoints && crItem.thresholdpoints.length > 0) {
                                var thresholdpointMax = crItem.thresholdpoints[0].pointsvalue;
                                for (var j = 0; j < crItem.thresholdpoints.length; j++) {
                                    if (crItem.thresholdpoints[j].pointsvalue > thresholdpointMax) {
                                        thresholdpointMax = crItem.thresholdpoints[j].pointsvalue
                                    }
                                }
                                pointsValue += parseInt(thresholdpointMax);
                            }
                        }
                        iResult[0].responseDeclaration.pointsValue = pointsValue.toString();
                    }

                    if($.trim(desText) !== ''){
                        desText = $('<div />').html(desText);
                        desText.find('.math-tex').each(function () {
                            if ($(this).parent().children().length === 1 && $(this).parent().is('span')) {
                                $(this).unwrap();
                            }
                        })
                        desText = desText.html();
                    }

                    // Create the destination object with rich text content
                    var partialDestinationObject = '<span class="partialDestinationObject partialAddDestinationText" ' +
                        'unselectable="on" ' +
                        'style="width: ' + desWidth + 'px; height: ' + desHeight + 'px;" ' +
                        'partialID="Partial_1" ' +
                        'contenteditable="false" ' +
                        'type="text" ' +
                        'notRequireAllAnswers="' + notRequireAllAnswers + '" ' +
                        'destIdentifier="' + destIdentifier + '" ' +
                        'numberDroppable="' + desNumberDroppable + '">' +
                        '<img class="cke_reset cke_widget_mask partialAddDestinationTextMark" ' +
                        'src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" />' +
                        '<span>' + loadMathML(desText) + '</span>' +
                        '</span>&nbsp;';

                    if ($.browser.msie && parseInt($.browser.version, 10) < 10) {
                        partialDestinationObject = '&nbsp;' + partialDestinationObject;
                    }

                    var destinationObjectElement = CKEDITOR.dom.element.createFromHtml(partialDestinationObject);
                    editor.insertElement(destinationObjectElement);
                    $(destinationObjectElement.$).find('.math-tex').each(function () {
                        this.parentElement.classList.add('cke_widget_new');
                        editor.widgets.initOn(new CKEDITOR.dom.element(this), 'mathjax');
                    });

                    newResult = iResult;
                }
            };
        });
    }
});
