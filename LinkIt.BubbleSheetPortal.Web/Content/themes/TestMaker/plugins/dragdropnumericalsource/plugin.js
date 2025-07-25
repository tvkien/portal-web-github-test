(function() {
    'use strict';

    var pluginName = 'dragdropnumericalsource';
    var $dndNumericSrcWidth;
    var $dndNumericSrcHeight;
    var $dndNumericSrcCheckLimitTimes;
    var $dndNumericSrcLimitTimes;
    var $dndNumericSrcLimitTimesContainer;
    var dndNumericSrcEditor;
    var elementDragDropSrcSource;

    CKEDITOR.plugins.add(pluginName, {
        init: function(editor) {
            // Add command when execute dialog
            editor.addCommand(pluginName, new CKEDITOR.dialogCommand(pluginName));

            // Buttons
            editor.ui.addButton('DragDropNumericalSource', {
                label: 'Embed Drag Drop Numerical Source',
                command: pluginName
            });

            // Call when double click
            editor.on('doubleclick', function(evt) {
                var element = evt.data.element;

                if (element.hasClass('partialDragDropNumericalSourceMark')) {
                    var parents = element.getParents();
                    var parent;
        
                    for (var i = 0; i < parents.length; i++) {
                        parent = parents[i];
                        if (parent.hasClass('partialAddSourceNumerical')) {
                            break;
                        }
                    }

                    elementDragDropSrcSource = parent;
                    editor.getSelection().selectElement(elementDragDropSrcSource);
                    evt.data.dialog = pluginName;

                    dblickHandlerToolbar(editor);
                }
            });

            // Dialog
            CKEDITOR.dialog.add(pluginName, getDialog);
        }
    });

    var getUpDownNumber = function (selector, min, max) {
        var $selector = $(selector);
    
        $selector.ckUpDownNumber({
            minNumber: min,
            maxNumber: max,
            width: 18,
            height: 13
        });
    };

    var getDialog = function(editor) {
        var dialogHtml = '';

        dialogHtml =  '\
                        <div id="' + pluginName + '-dialog">\
                            <table cellspacing="0" border="0" align="" style="width:100%;" role="presentation">\
                                <tbody>\
                                    <tr>\
                                        <td>\
                                            Width: <input type="text" class="txtFullcreate" id="' + pluginName + 'Width" value="50"/>\
                                            Height: <input type="text" class="txtFullcreate" id="' + pluginName + 'Height" value="20"/>\
                                        </td>\
                                    </tr>\
                                    <tr>\
                                        <td class="p-t-10">\
                                            <textarea id="' + pluginName + 'editor" name="' + pluginName + 'editor" class="dnd-numerical-editor"></textarea>\
                                        </td>\
                                    </tr>\
                                    <tr>\
                                        <td class="p-t-10">\
                                            <div class="form-checkbox-inline"><span>Limit the number of times this object is draggable:</span></div>\
                                            <div class="form-checkbox-inline"><input type="checkbox" id="' + pluginName +'CheckLimitTimes" class="form-checkbox"/></div>\
                                            <div class="form-checkbox-inline limittimes-container hidden"><input type="text" class="txtFullcreate" id="' + pluginName +'LimitTimes" value="1"/></span></div>\
                                        </td>\
                                    </tr>\
                                </tbody>\
                            </table>\
                        </div>';
        return {
            title: 'Drag and Drop Numeric Draggable',
            minWidth: 400,
            minHeight: 200,
            contents: [{
                id: pluginName,
                label: 'Drag and Drop Numeric Draggable',
                elements: [{
                    type: 'html',
                    html: dialogHtml,
                    onLoad: function () {
                        $dndNumericSrcWidth = $('#' + pluginName + 'Width');
                        $dndNumericSrcHeight = $('#' + pluginName + 'Height');
                        $dndNumericSrcCheckLimitTimes = $('#' + pluginName + 'CheckLimitTimes');
                        $dndNumericSrcLimitTimes = $('#' + pluginName + 'LimitTimes');
                        $dndNumericSrcLimitTimesContainer = $('#' + pluginName + '-dialog').find('.limittimes-container');
                        dndNumericSrcEditor = pluginName + 'editor';

                        $dndNumericSrcCheckLimitTimes.on('change', function() {
                            var $self = $(this);
                            if ($self.is(':checked')) {
                                var dndSrcLimitTimes = $dndNumericSrcLimitTimes.val();

                                $dndNumericSrcLimitTimesContainer.css('display', 'inline-block');

                                if (dndSrcLimitTimes === 'unlimited') {
                                    $dndNumericSrcLimitTimes.focus().val('1');
                                }
                            } else {
                                $dndNumericSrcLimitTimesContainer.hide();
                            }
                        });

                        getUpDownNumber($dndNumericSrcWidth, 20, 600);
                        getUpDownNumber($dndNumericSrcHeight, 20, 600);
                        getUpDownNumber($dndNumericSrcLimitTimes, 1, 100);
                    },
                    onShow: function () {
                        var dndSrcWidth = elementDragDropSrcSource.getStyle('width');
                        var dndSrcHeight = elementDragDropSrcSource.getStyle('height');
                        var dndSrcValue = elementDragDropSrcSource.getText();
                        var dndSrcLimitTimes = elementDragDropSrcSource.data('limit');
                        var globalEditor;

                        // Initialize ckeditor for drag and drop numerical
                        createCKEditor(dndNumericSrcEditor);

                        globalEditor = CKEDITOR.instances[dndNumericSrcEditor];

                        globalEditor.on('key', function(event) {
                            /**
                            * 1. Allow: backspace, delete
                            * 2. Allow numbers: 0 -9 (Plus and minus) and Numeric keypad
                            * 3. Basic operators: + - x ÷ /
                            * 4. Relational operators: =, <, >, <=, >=, <>
                            * 5. Open and close brackets: ( ), [ ]
                            * 6. Separator: dot, comma
                            */
                            var keyAllowed = [
                                8, 37, 38, 39, 40, 46,
                                106, 107, 109, 111, 187,
                                188, 189, 190, 191, 219,
                                221, 1114177, 2228272, 2228281,
                                2228411, 2228412, 2228414, 2228277
                            ];

                            if (event.data.keyCode) {
                                var keyCode = event.data.keyCode;

                                if (_.indexOf(keyAllowed, keyCode) !== -1 ||
                                    keyCode >= 48 && keyCode <= 57 ||
                                    keyCode >= 96 && keyCode <= 105) {
                                    return;
                                } else {
                                    event.cancel();
                                }
                            }
                        });

                        globalEditor.on('focus', function() {
                            var data = globalEditor.getData();

                            if (data === 'Number') {
                                globalEditor.setData('');
                            }
                        });

                        dndSrcWidth = dndSrcWidth !== undefined ? dndSrcWidth.replace('px', '') : 50;
                        dndSrcHeight = dndSrcHeight !== undefined ? dndSrcHeight.replace('px', '') : 20;

                        dndSrcLimitTimes = (dndSrcLimitTimes === 'unlimited' && dndSrcLimitTimes === undefined) ? 'unlimited' : dndSrcLimitTimes;

                        // Set new width and height
                        $dndNumericSrcWidth.val(dndSrcWidth);
                        $dndNumericSrcHeight.val(dndSrcHeight);

                        // If limit times is unlimited, checkbox is checked or not
                        if (dndSrcLimitTimes === 'unlimited') {
                            $dndNumericSrcCheckLimitTimes.prop('checked', false);
                            $dndNumericSrcLimitTimesContainer.hide();
                        } else {
                            $dndNumericSrcCheckLimitTimes.prop('checked', true);
                            $dndNumericSrcLimitTimesContainer.css('display', 'inline-block');
                        }

                        // Set new value of limit times
                        $dndNumericSrcLimitTimes.val(dndSrcLimitTimes);

                        // Set regular math for source drag and drop numeric
                        globalEditor.setData(dndSrcValue);

                        refreshPartialCredit();
                    }
                }]
            }],
            onOk: function() {
                var dndSrcWidth = $dndNumericSrcWidth.val();
                var dndSrcHeight = $dndNumericSrcHeight.val();
                var dndSrcLimitTimes = $dndNumericSrcLimitTimes.val();
                var dndSrcIdentifier = $(elementDragDropSrcSource).attr('srcidentifier');
                var dndSrcValue = CKEDITOR.instances[dndNumericSrcEditor].getData();

                if (!$dndNumericSrcCheckLimitTimes.is(':checked')) {
                    dndSrcLimitTimes = 'unlimited';
                }

                if ($.trim(dndSrcValue) === '') {
                    var msg = 'Please input source of drag and drop numerical.';
                    // Common popup alert message from ckeditor_utils.js
                    customAlert(msg);
                    return false;
                }

                dndSrcValue = dndSrcValue.replace('</div>', '');
                dndSrcValue = dndSrcValue.replace('<div>', '');

                // Update value for source in iResult
                for (var i = 0, lenIResult = iResult.length; i < lenIResult; i++) {
                    var iResultItem = iResult[i];

                    for (var si = 0, lenIResultSource = iResultItem.source.length; si < lenIResultSource; si++) {
                        var iResultSource = iResultItem.source[si];
                        if (iResultSource.srcIdentifier === dndSrcIdentifier) {
                            iResultSource.value = dndSrcValue;
                            iResultSource.limit = dndSrcLimitTimes;
                            break;
                        }
                    }
                }

                var options = {
                    identifier: dndSrcIdentifier,
                    width: dndSrcWidth,
                    height: dndSrcHeight,
                    limit: dndSrcLimitTimes,
                    value: dndSrcValue
                };

                var dragdropnumericalsourceHtml = '';

                // Tweet sized javascript templating engine from ckeditor_utils.js
                dragdropnumericalsourceHtml = t('<span class="partialSourceObject partialAddSourceNumerical" unselectable="on" contenteditable="false" style="width: {width}px; height: {height}px;" srcIdentifier="{identifier}" data-limit="{limit}"><img class="cke_reset cke_widget_mask partialDragDropNumericalSourceMark" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" />{value}</span>', options);

                if ($.browser.msie && parseInt($.browser.version, 10) < 10) {
                    dragdropnumericalsourceHtml = '&nbsp;' + dragdropnumericalsourceHtml;
                } else {
                    dragdropnumericalsourceHtml = dragdropnumericalsourceHtml + '&#8203;';
                }

                var dragdropnumericalsourceElement = CKEDITOR.dom.element.createFromHtml(dragdropnumericalsourceHtml);

                editor.insertElement(dragdropnumericalsourceElement);

                newResult = iResult;

                // Reset special characters
                isCharNumeric = false;
            },
            onCancel: function() {
                // Reset special characters
                isCharNumeric = false;
            }
        };
    };

    /**
     * Create ckeditor for content of drag and drop source numerical
     * @param  {[type]} ckEditor [description]
     * @return {[type]}      [description]
     */
    function createCKEditor(ckEditor) {
        // Destroy ckeditor
        try {
            CKEDITOR.instances[ckEditor].destroy(true);
        } catch (e) {}

        isNewPalette = true;

        CKEDITOR.replace(ckEditor, {
            extraPlugins: 'specialchar',
            toolbar: [
                ['SpecialChar']
            ],
            sharedSpaces: {
                top: 'multipleTop',
                bottom: 'multipleBot'
            },
            extraAllowedContent: 'span[stylename]; span(smallText,normalText,largeText,veryLargeText);',
            height: 100,
            width: 400,
            hideSpecialcharTabs: ['palletTabSpanishSpecial', 'palletTabFrenchSpecial']
        });
    }
}());
