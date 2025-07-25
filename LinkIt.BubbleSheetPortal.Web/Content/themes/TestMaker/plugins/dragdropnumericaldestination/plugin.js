(function() {
    'use strict';

    var pluginName = 'dragdropnumericaldestination';
    var $dndNumericDestWidth;
    var $dndNumericDestHeight;
    var $dndNumericDestLimitTimes;
    var $dndNumericDestLimitTimesContainer;
    var eleAddDestinationNumerical;

    CKEDITOR.plugins.add(pluginName, {
        init: function (editor) {

            // Add command when execute dialog
            editor.addCommand(
                pluginName,
                new CKEDITOR.dialogCommand(pluginName)
            );

            // Buttons
            editor.ui.addButton('DragDropNumericalDestination', {
                label: 'Embed Drag Drop Numerical Destination',
                command: pluginName
            });

            // Call when double click
            editor.on('doubleclick', function(evt) {
                var element = evt.data.element;

                if (element.hasClass('partialDragDropNumericalDestinationMark')) {
                    var parents = element.getParents();
                    var parent;
        
                    for (var i = 0; i < parents.length; i++) {
                        parent = parents[i];
                        if (parent.hasClass('partialAddDestinationNumerical')) {
                            break;
                        }
                    }

                    eleAddDestinationNumerical = parent;
                    editor.getSelection().selectElement(eleAddDestinationNumerical);
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
                                            Width: <input type="text" class="txtFullcreate" id="' + pluginName + 'Width" value="100"/>&nbsp;&nbsp;&nbsp;\
                                            Height: <input type="text" class="txtFullcreate" id="' + pluginName + 'Height" value="20"/>&nbsp;&nbsp;&nbsp;\
                                        </td>\
                                    </tr>\
                                    <tr>\
                                        <td class="p-t-10">\
                                            <div class="form-checkbox-inline">Maximum number of draggable objects for this destination is:</div>\
                                            <div class="form-checkbox-inline limittimes-container"><input type="text" class="txtFullcreate" id="' + pluginName + 'LimitTimes" value="1"/></div>\
                                        </td>\
                                    </tr>\
                                    <tr>\
                                        <td class="p-t-10">\
                                            <p>Value:</p>\
                                            <input type="text" class="form-input m-t-10" id="' + pluginName + 'Value" placeholder="Sample: DEST_1"/>\
                                        </td>\
                                    </tr>\
                                </tbody>\
                            </table>\
                        </div>';
        return {
            title: 'Drag and Drop Numeric Destination',
            minWidth: 410,
            minHeight: 200,
            contents: [{
                id: pluginName,
                label: 'Drag and Drop Numeric Destination',
                elements: [{
                    type: 'html',
                    html: dialogHtml,
                    onLoad: function () {
                        $dndNumericDestWidth = $('#' + pluginName + 'Width');
                        $dndNumericDestHeight = $('#' + pluginName + 'Height');
                        $dndNumericDestLimitTimes = $('#' + pluginName + 'LimitTimes');
                        $dndNumericDestLimitTimesContainer = $('#' + pluginName + '-dialog').find('.limittimes-container');

                        getUpDownNumber($dndNumericDestWidth, 50, 600);
                        getUpDownNumber($dndNumericDestHeight, 20, 600);
                        getUpDownNumber($dndNumericDestLimitTimes, 1, 100);
                    },
                    onShow: function () {
                        refreshPartialCredit();

                        $('#' + pluginName + 'Value').val(eleAddDestinationNumerical.getText());

                        $('#' + pluginName + 'Width')
                            .focusout(function () {
                                var $self = $(this);

                                if ($self.val() === '') {
                                    $self.val(100);
                                }
                            })
                            .val(eleAddDestinationNumerical.getStyle('width').replace('px', ''));


                        $('#' + pluginName + 'Height')
                            .focusout(function () {
                                var $self = $(this);

                                if ($self.val() === '') {
                                    $self.val(20);
                                }
                            })
                            .val(eleAddDestinationNumerical.getStyle('height').replace('px', ''));

                        if ($(eleAddDestinationNumerical).attr('numberDroppable') !== undefined) {
                            $('#' + pluginName + 'LimitTimes').val($(eleAddDestinationNumerical).attr('numberDroppable'));
                        } else {
                            $('#' + pluginName + 'LimitTimes').val('1');
                        }
                    }
                }]
            }],
            onOk: function () {
                var desText = $('#' + pluginName + 'Value').val();
                var desWidth = $('#' + pluginName + 'Width').val();
                var desHeight = $('#' + pluginName + 'Height').val();
                var desNumberDroppable = $('#' + pluginName + 'LimitTimes').val();
                var destIdentifier = eleAddDestinationNumerical.getAttribute('destidentifier');

                //This to make sure the input just get text only
                desText = $('<div />')
                                .html(desText)
                                .text()
                                .replace(/&/g, '&amp;')
                                .replace(/</g, '&lt;');

                if ($.trim(desText) === '') {
                    var msg = 'Please input destination of drag and drop numerical.';
                    // Common popup alert message from ckeditor_utils.js
                    customAlert(msg);
                    return false;
                }

                var options = {
                    identifier: destIdentifier,
                    width: desWidth,
                    height: desHeight,
                    limit: desNumberDroppable,
                    value: desText
                };

                var dragdropnumericaldestinationHtml = '';

                // Tweet sized javascript templating engine from ckeditor_utils.js
                dragdropnumericaldestinationHtml = t('<span class="partialDestinationObject partialAddDestinationNumerical" unselectable="on"  style="width: {width}px; height: {height}px;" partialID="Partial_1" contenteditable="false" type="text" destIdentifier="{identifier}" numberDroppable="{limit}"><img class="cke_reset cke_widget_mask partialDragDropNumericalDestinationMark" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" />{value}</span>', options);

                if ($.browser.msie && parseInt($.browser.version, 10) < 10) {
                    dragdropnumericaldestinationHtml = '&nbsp;' + dragdropnumericaldestinationHtml;
                } else {
                    dragdropnumericaldestinationHtml = dragdropnumericaldestinationHtml + '&#8203;';
                }

                var dragdropnumericaldestinationElement = CKEDITOR.dom.element.createFromHtml(dragdropnumericaldestinationHtml);
                editor.insertElement(dragdropnumericaldestinationElement);

                newResult = iResult;
            }
        };
    };
}());
