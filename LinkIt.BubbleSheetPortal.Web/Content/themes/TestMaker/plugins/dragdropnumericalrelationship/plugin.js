(function() {
    'use strict';

    var pluginName = 'dragdropnumericalrelationship';
    var $dndNumericRelationPoint;
    var $dndNumericRelationPattern;
    var $listSourceObj;
    var $listDestObj;

    CKEDITOR.plugins.add(pluginName, {
        init: function(editor) {
            // Add command when execute dialog
            editor.addCommand(
                pluginName,
                new CKEDITOR.dialogCommand(pluginName)
            );

            // Buttons
            editor.ui.addButton('DragDropNumericalRelationship', {
                label: 'Embed Drag Drop Numerical Relationship',
                command: pluginName
            });

            // Dialogs
            CKEDITOR.dialog.add(pluginName, getDialog);
        }
    });

    var resetGradingAlgorithmic = function (dialog) {
        var $dialog = $(dialog);
    
        $dialog.find('#dragdropnumericalrelationshipPoint').parent().addClass('is-disabled');
        $dialog.find('#dragdropnumericalrelationshipPoint').val('0');
        $dialog.find('#dragdropnumericalrelationshipPattern').val('');
        $dialog.find('#dragdropnumericalrelationshipPattern').prop('disabled', true);
        $dialog.find('.dndAddDest').addClass('is-disabled');
    };
    
    
    var resetGradingNormal = function (dialog) {
        var $dialog = $(dialog);
    
        $dialog.find('#dragdropnumericalrelationshipPoint').parent().removeClass('is-disabled');
        $dialog.find('#dragdropnumericalrelationshipPoint').val('1');
        $dialog.find('#dragdropnumericalrelationshipPattern').prop('disabled', false);
        $dialog.find('.dndAddDest').removeClass('is-disabled');
    };

    var getDialog = function(editor) {
        var dialogHtml = '';

        dialogHtml =  '\
                        <div id="' + pluginName + '-dialog">\
                            <ttable cellspacing="0" border="0" align="" style="width:100%;" role="presentation">\
                                <tbody>\
                                    <tr>\
                                        <td>\
                                            <div class="m-b-15">\
                                                 <input type="radio" name="dndn-grading" id="dndn-grading-normal" checked/>\
                                                 <label for="dndn-grading-normal">Normal Grading</label>\
                                                 <input type="radio" name="dndn-grading" id="dndn-grading-algorithmic"/>\
                                                 <label for="dndn-grading-algorithmic">Algorithmic Grading</label>\
                                            </div>\
                                        </td>\
                                    </tr>\
                                    <tr>\
                                        <td>\
                                          Point value: <input type="text" class="txtFullcreate" id="' + pluginName + 'Point" value="1"/>\
                                        </td>\
                                    </tr>\
                                    <tr>\
                                        <td class="p-t-10">\
                                            <p>Relationship:</p>\
                                            <input type="text" id="' + pluginName + 'Pattern" placeholder="Sample: {DEST_1} - 2 = 5" class="form-input m-t-10" />\
                                        </td>\
                                    </tr>\
                                    <tr>\
                                        <td class="p-t-10">\
                                            <fieldset class="fieldset">\
                                                <legend>Destination</legend>\
                                                <div class="list-obj">\
                                                    <ul id="list-dest-obj"></ul>\
                                                </div>\
                                            </fieldset>\
                                        </td>\
                                    </tr>\
                                    <tr>\
                                        <td class="p-t-10">\
                                            <fieldset class="fieldset">\
                                                <legend>Source Object</legend>\
                                                <div class="list-obj">\
                                                    <ul id="list-source-obj"></ul>\
                                                </div>\
                                            </fieldset>\
                                        </td>\
                                    </tr>\
                                </tbody>\
                            </table>\
                        </div>';
        return {
            title: 'Drag and Drop Numeric Relationship',
            minWidth: IS_V2 ? 500 : 400,
            minHeight: 200,
            contents: [{
                id: pluginName,
                label: 'Drag and Drop Numeric Relationship',
                elements: [{
                    type: 'html',
                    html: dialogHtml,
                    onLoad: function () {
                        var $dialog = $(this.getDialog().getElement().$);
                        $dndNumericRelationPoint = $('#' + pluginName + 'Point');
                        $dndNumericRelationPattern = $('#' + pluginName + 'Pattern');
                        $listSourceObj = $('#list-source-obj');
                        $listDestObj = $('#list-dest-obj');

                        $dndNumericRelationPoint.ckUpDownNumber({
                            maxNumber: 1000,
                            minNumber: 0,
                            width: 18,
                            height: 13
                        });

                        $dialog.find('input[type="radio"][name="dndn-grading"]').on('change', function () {
                            var $grading = $(this);
                            var gradingMethod = $grading.attr('id');

                            if (gradingMethod === 'dndn-grading-algorithmic') {
                                resetGradingAlgorithmic($dialog);
                            } else {
                                resetGradingNormal($dialog);
                            }
                        });
                    },
                    onShow: function () {
                        var lenResult = iResult.length;
                        var data = $('<div/>').append(CKEDITOR.instances[ckID].getData());
                        var $data = $(data);
                        var isAlgorithmicGrading = false;

                        refreshPartialCredit();

                        // Empty list source object and list destination object
                        $listSourceObj.empty();
                        $listDestObj.empty();

                        // Check iResult exists
                        if (lenResult) {
                            var sourceObjAnswer = [];
                            var destObjAnswer = [];
                            var destAutoComplete = [];

                            for (var i = 0; i < lenResult; i++) {
                                var iResultItem = iResult[i];

                                if (iResultItem.type === 'dragDropNumerical') {
                                    // Load source object
                                    for (var si = 0; si < iResultItem.source.length; si++) {
                                        var dndSrc = iResultItem.source[si];
                                        var dndSrcIdentifier = dndSrc.srcIdentifier;
                                        var dndSrcValue = dndSrc.value;
                                        var dndSrcHtml = '';
                                        var orderSrc = si + 1;

                                        var source = {
                                            order: orderSrc,
                                            identifier: dndSrcIdentifier,
                                            value: dndSrcValue
                                        };

                                        // Tweet sized javascript templating engine from ckeditor_utils.js
                                        dndSrcHtml += t('<li>{order}. <strong>{{identifier}}</strong> has the value of <strong>{value}</strong></li>', source);

                                        sourceObjAnswer.push(dndSrcHtml);
                                    }

                                    // Load destination object
                                    for (var di = 0; di < iResultItem.destination.length; di++) {
                                        var dndDest = iResultItem.destination[di];
                                        var dndDestIdentifier = dndDest.destIdentifier;
                                        var dndDestValue;
                                        var dndDestHtml = '';
                                        var orderDest = di + 1;

                                        dndDestValue = $data.find('.partialAddDestinationNumerical[destidentifier=' + dndDestIdentifier + ']').text();

                                        var dest = {
                                            order: orderDest,
                                            identifier: dndDestIdentifier,
                                            value: dndDestValue
                                        };

                                        // Tweet sized javascript templating engine from ckeditor_utils.js
                                        dndDestHtml += t('<li><input type="button" des_value="{identifier}" class="dndAddDest" title="Add destination to relationship."/> {order}. <strong>{{identifier}}</strong> has the value of <strong>{value}</strong></li>', dest);

                                        destObjAnswer.push(dndDestHtml);
                                        destAutoComplete.push(dndDestIdentifier);
                                    }

                                    // Load correct response
                                    var iResultResponsePoint = iResultItem.correctResponse.pointsValue;
                                    var iResultResponsePattern = iResultItem.correctResponse.expressionPattern;

                                    iResultResponsePoint = iResultResponsePoint !== undefined ? iResultResponsePoint : 1;
                                    iResultResponsePattern = iResultResponsePattern !== undefined ? iResultResponsePattern : '';

                                    $dndNumericRelationPoint.val(iResultResponsePoint);
                                    $dndNumericRelationPattern.val(iResultResponsePattern);

                                    $dndNumericRelationPoint.parent().removeClass('is-disabled');
                                    $dndNumericRelationPattern.prop('disabled', false);

                                    if (iResultItem.responseDeclaration.algorithmicGrading === '1') {
                                        $('#dndn-grading-algorithmic').prop('checked', true);
                                        $dndNumericRelationPoint.parent().addClass('is-disabled');
                                        $dndNumericRelationPattern.prop('disabled', true);
                                        isAlgorithmicGrading = true;
                                    } else {
                                        $('#dndn-grading-normal').prop('checked', true);
                                    }
                                }
                            }

                            $listSourceObj.append(sourceObjAnswer.join(''));
                            $listDestObj.append(destObjAnswer.join(''));

                            //Define click event for button inside click
                            $listDestObj.find(".dndAddDest").click(function () {
                                var btnValue = $(this).attr("des_value");

                                insertAtCaret("dragdropnumericalrelationshipPattern", "{" + btnValue + "}");
                            });

                            if (isAlgorithmicGrading) {
                                $listDestObj.find('.dndAddDest').addClass('is-disabled');
                            } else {
                                $listDestObj.find('.dndAddDest').removeClass('is-disabled');
                            }

                          // Keyboard input limit in relationship field
                          setTimeout(function () {
                            $dndNumericRelationPattern.on('keydown', function (event) {

                              // Allow: backspace, delete, tab, escape, enter and .
                              var keyAllowed = [
                                8, 9, 13, 27, 32, 46,
                                187, 188, 189, 190, 191,
                                219, 221, 107, 109, 106, 111, 110
                              ];

                              if (event.keyCode) {
                                var keyCode = event.keyCode;

                                if ($.browser.mozilla) {
                                  if (keyCode == 61 || keyCode == 173) {
                                    return;
                                  }
                                }

                                //Skip Enter key
                                if (keyCode == 13) {
                                  return false;
                                }

                                if ($.inArray(keyCode, keyAllowed) !== -1 ||
                                  // Allow: Ctrl+A
                                  (keyCode == 65 && event.ctrlKey) ||
                                  // Allow: home, end, left, right
                                  (keyCode >= 35 && keyCode <= 39)) {
                                  // let it happen, don't do anything

                                  return;
                                }
                                else {
                                  // Ensure that it is a number and stop the keypress
                                  if (event.shiftKey && 48 || event.shiftKey && 57) {
                                    return;
                                  } else if ((event.shiftKey ||
                                    (keyCode < 48 || keyCode > 57) &&
                                    (keyCode < 96 || keyCode > 105)) ||
                                    event.shiftKey && 191) {
                                    event.preventDefault();
                                  }
                                }
                              }
                            });
                          }, 1000);
                          
                        }
                    }
                }]
            }],
            onOk: function() {
                var dndRelationPoint = $dndNumericRelationPoint.val();
                var dndRelationPattern = $dndNumericRelationPattern.val();
                var isAlgorithmicGrading = $('#dndn-grading-algorithmic').is(':checked');

                if ($.trim(dndRelationPattern) === '' && !isAlgorithmicGrading) {
                    var msg = 'Please input relationship pattern of drag and drop numerical.';
                    // Common popup alert message from ckeditor_utils.js
                    customAlert(msg);
                    return false;
                }

                var duplicateId = false;

                // Load destination object
                for (var i = 0; i < iResult.length; i++) {
                    var iResultItem = iResult[i];

                    if (iResultItem.type === 'dragDropNumerical') {
                        for (var di = 0; di < iResultItem.destination.length; di++) {
                            var dndDest = iResultItem.destination[di];
                            var dndDestIdentifier = dndDest.destIdentifier;

                            var regExp = new RegExp(dndDestIdentifier, "gi");
                            var count = (dndRelationPattern.match(regExp) || []).length;

                            if (count > 1) {
                                duplicateId = true;
                                break;
                            }
                        }
                    }
                }

                if (duplicateId) {
                    alert('Destination has duplicated. Please check your relationship string.');
                    return false;
                }

                // Update value for correct response in iResult
                for (var i = 0, lenIResult = iResult.length; i < lenIResult; i++) {
                    var iResultItem = iResult[i];

                    iResultItem.correctResponse.pointsValue = dndRelationPoint;
                    iResultItem.correctResponse.expressionPattern = dndRelationPattern;

                    iResultItem.responseDeclaration.absoluteGrading = '1';
                    iResultItem.responseDeclaration.algorithmicGrading = '0';

                    if (isAlgorithmicGrading) {
                        iResultItem.responseDeclaration.absoluteGrading = '0';
                        iResultItem.responseDeclaration.algorithmicGrading = '1';
                    }
                }

                if (isAlgorithmicGrading) {
                    TestMakerComponent.isShowAlgorithmicConfiguration = true;
                } else {
                    TestMakerComponent.isShowAlgorithmicConfiguration = false;
                }

                // Clear the cached search term and make every search new
                $dndNumericRelationPattern.autocomplete('destroy');
            },
            onCancel: function() {
                // Clear the cached search term and make every search new
                $dndNumericRelationPattern.autocomplete('destroy');
            }
        };
    };
}());
