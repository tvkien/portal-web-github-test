CKEDITOR.plugins.add('partialaddproperties', {
    lang: 'en', // %REMOVE_LINE_CORE%
    icons: 'partialaddproperties',
    requires: 'dialog',
    hidpi: true, // %REMOVE_LINE_CORE%
    init: function (editor) {
        var pluginName = 'insertPartialAddProperties';

        editor.addCommand(pluginName, new CKEDITOR.dialogCommand(pluginName));

        editor.ui.addButton('PartialAddProperties', {
            label: 'Partial Edit Properties',
            command: pluginName,
            icon: this.path + 'icons/drawtool.png'
        });

        editor.widgets.add('partialaddproperties', {
            inline: true,
            mask: true,
            allowedContent: {
                p: {
                    classes: 'partialAddProperties',
                    attributes: '!id,name,contenteditable'
                }
            },
            template: '<p class="partialAddProperties"></p>'
        });

        CKEDITOR.dialog.add(pluginName, function (editor) {

            var partialAddPropertiesHtml = '';

            partialAddPropertiesHtml = '\
                <div class="property_parameters">\
                    <p>\
                        <label class="widthLabel" for="absolute">\
                            <input type="checkbox" id="absolute" value="0" name="absolute"/>Absolute Grading\
                        </label>\
                    </p>\
                    <p class="fieldAbsolute"><span class="widthLabel">Absolute grading points:</span> <input type="text" value="1" name="absolute" id="txtAbsolute" class="txtAbsolute"/></p>\
                    <p class="gradingThreshold"><span class="widthLabel">Partial grading threshold(%):</span> <input type="text" value="50" name="threshold" id="threshold" class="threshold"/></p>\
                    <p>\
                        <label class="widthLabel" for="cbRelative">\
                            <input type="checkbox" value="0" id="cbRelative" name="cbRelative"/>Relative Grading\
                        </label>\
                    </p>\
                    <p class="fieldRelative">\
                        <span class="widthLabel">Relative grading points:</span>\
                        <input type="text" value="1" name="cbRelative" id="txtRelative" class="txtRelative"/>\
                    </p>\
                    <p>\
                        <label class="widthLabel" for="cbThreshold">\
                            <input type="checkbox" value="0" id="cbThreshold" name="cbThreshold"/>Destination Level Partial Credit Grading\
                        </label>\
                    </p>\
                    <p>\
                        <label class="widthLabel" for="cbAlgorithmic">\
                            <input type="checkbox" value="0" id="cbAlgorithmic" name="cbAlgorithmic"/>Algorithmic Grading\
                        </label>\
                    </p>\
                    <p class="partialPoint u-ws-normal">\
                        Note: For this option, you will double click on each<br/> destination to determine its point value.\
                        Would you like <br/>for this question\'s overall point value to be the sum of <br/>each destination\'s point value?\
                        Or, would you like to set <br/>a cap on maximum points earned for this question?\
                    </p>\
                    <p class="partialPoint u-ws-normal">\
                        <input type="radio" id="cb-cap-1" name="cb-cap" class="u-align-middle js-change-cap" checked/>\
                        <label class="widthLabel u-ws-normal u-align-middle u-w-p-91" for="cb-cap-1">\
                            Use the sum of all destinations to determine overall <br/>point value.\
                        </label>\
                    </p>\
                    <p class="partialPoint"><span class="widthLabel"><input type="radio" id="cb-cap-2" name="cb-cap" class="u-align-middle js-change-cap"/><label for="cb-cap-2"> Set cap at:</label></span> <input type="text" value="1" name="fullcreatepartial" id="txtFullCreatePartial" class="txtFullCreatePartial"/></p>\
                    <p class="absoluteRelativePoint"><span class="widthLabel">Max points:</span> <input type="text" value="1" name="fullcreate" id="txtFullcreate" class="txtFullcreate"/></p>\
                    <div class="sortResponse"><fieldset><legend>Order Correct Answer </legend><ul id="sortResponse"></ul></fieldset></div>\
                    <p style="padding-top: 10px;">\
                        <label class="widthLabel" for="cbLineMatching">\
                            <input type="checkbox" id="cbLineMatching" name="cbLineMatching"/>Display item as line match\
                        </label>\
                    </p>\
                    <p style="display: none">\
                        <label class="widthLabel" for="anchorPositionObject">Anchor Position Object</label>\
                        <select style="width : 100px" name="anchorPositionObject" id="anchorPositionObject">\
                            <option value="left">Left</option>\
                            <option value="top">Top</option>\
                            <option selected value="right">Right</option>\
                            <option value="bottom">Bottom</option>\
                        </select>\
                    </p>\
                    <p style="display: none">\
                        <label class="widthLabel" for="anchorPositionDestination">Anchor Position Destination</label>\
                        <select style="width : 100px" name="anchorPositionDestination" id="anchorPositionDestination">\
                            <option selected value="left">Left</option>\
                            <option value="top">Top</option>\
                            <option value="right">Right</option>\
                            <option value="bottom">Bottom</option>\
                        </select>\
                    </p>\
                </div>\
            ';

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

            return {
                title: 'Drag and Drop - Properties',
                minWidth: IS_V2 ? 400 : 300,
                minHeight: 100,
                resizable: CKEDITOR.DIALOG_RESIZE_NONE,
                contents: [{
                    id: 'partialaddproperties',
                    label: 'Settings',
                    elements: [{
                        type: 'html',
                        html: partialAddPropertiesHtml,
                        onLoad: function () {
                            var $partialPoint = $('.partialPoint');
                            var $absoluteRelativePoint = $('.absoluteRelativePoint');
                            var $fullCreatePartial = $('#txtFullCreatePartial');

                            $('.property_parameters').parents('.cke_dialog_contents_body').addClass('chrome');

                            $('#absolute').on('click', function () {
                                if (this.checked) {
                                    $('.fieldAbsolute, .gradingThreshold').show();
                                    $partialPoint.hide();
                                    $absoluteRelativePoint.show();

                                    $(this).val('1');

                                    // Hide Relative
                                    $('#cbRelative').attr('checked', false).val('0');
                                    $('.fieldRelative, .sortResponse').hide();

                                    // Reset threshold grading
                                    $('#cbThreshold').prop('checked', false).val('0');
                                    $('#txtFullcreate').val('1');
                                    $('#txtFullcreate').parent().removeClass('is-disabled');

                                    $('#cbAlgorithmic').prop('checked', false).val('0');
                                } else {
                                    $('.fieldAbsolute, .gradingThreshold').hide();
                                    $(this).val('0');
                                }
                            });

                            $('#cbRelative').on('click', function () {
                                if (this.checked) {
                                    $('.fieldRelative').show();
                                    $partialPoint.hide();
                                    $absoluteRelativePoint.show();
                                    if ($('#sortResponse li').length > 0) {
                                        $('.sortResponse').show();
                                    }
                                    $(this).val('1');

                                    // Hide absolute
                                    $('#absolute').attr('checked', false).val('0');
                                    $('.fieldAbsolute, .gradingThreshold').hide();

                                    // Reset threshold grading
                                    $('#cbThreshold').prop('checked', false).val('0');
                                    $('#txtFullcreate').val('1');
                                    $('#txtFullcreate').parent().removeClass('is-disabled');

                                    $('#cbAlgorithmic').prop('checked', false).val('0');
                                } else {
                                    $('.fieldRelative, .sortResponse').hide();
                                    $(this).val('0');
                                }
                            });

                            $('#cbThreshold').on('click', function () {
                                var $self = $(this);

                                if (this.checked) {
                                    $self.val('1');
                                    $partialPoint.show();
                                    $absoluteRelativePoint.hide();

                                    // Hide absolute
                                    $('#absolute').prop('checked', false).val('0');
                                    $('.fieldAbsolute, .gradingThreshold').hide();

                                    // Hide Relative
                                    $('#cbRelative').prop('checked', false).val('0');
                                    $('.fieldRelative, .sortResponse').hide();

                                    $fullCreatePartial.val('1');
                                    $fullCreatePartial.parent().addClass('hidden');
                                    $('#cb-cap-1').prop('checked', true);

                                    $('#cbAlgorithmic').prop('checked', false).val('0');
                                } else {
                                    $partialPoint.hide();
                                    $absoluteRelativePoint.show();
                                    $self.val('0');
                                }
                            });

                            $('#cbAlgorithmic').on('click', function () {
                                var $self = $(this);

                                $partialPoint.hide();
                                $absoluteRelativePoint.show();

                                if (this.checked) {
                                    $self.val('1');

                                    // Hide absolute
                                    $('#absolute').prop('checked', false).val('0');
                                    $('.fieldAbsolute, .gradingThreshold').hide();

                                    // Hide Relative
                                    $('#cbRelative').prop('checked', false).val('0');
                                    $('.fieldRelative, .sortResponse').hide();

                                    // Hide destination level partial credit grading
                                    $('#cbThreshold').prop('checked', false).val('0');

                                    $('#txtFullcreate').val('0');
                                    $('#txtFullcreate').parent().addClass('is-disabled');
                                } else {
                                    $self.val('0');
                                    $('#txtFullcreate').val('1');
                                    $('#txtFullcreate').parent().removeClass('is-disabled');
                                }
                            });

                            $('#cbLineMatching').on('click', function () {
                                var $self = $(this);
                                if (this.checked) {
                                    $('#anchorPositionObject').parent().show();
                                    $('#anchorPositionDestination').parent().show();
                                } else {
                                    $('#anchorPositionObject').parent().hide();
                                    $('#anchorPositionDestination').parent().hide();
                                }
                            });

                            $('.js-change-cap').on('change', function () {
                                if (this.id === 'cb-cap-1') {
                                    $fullCreatePartial.parent().addClass('hidden');
                                } else {
                                    $fullCreatePartial.parent().removeClass('hidden');
                                    $fullCreatePartial.val('1');
                                }
                            });
                        },
                        onShow: function () {
                            var $partialPoint = $('.partialPoint');
                            var $absoluteRelativePoint = $('.absoluteRelativePoint');
                            var $fullCreatePartial = $('#txtFullCreatePartial');

                            getUpDownNumber('.txtFullcreate', 0, 100);
                            getUpDownNumber('.threshold', 0, 100);
                            getUpDownNumber('.txtAbsolute', 0, 100);
                            getUpDownNumber('.txtRelative', 0, 100);
                            getUpDownNumber('.txtFullCreatePartial', 0, 100);

                            // Hide the list sort before check is show or hide
                            $('.sortResponse').hide();

                            for (var i = 0; i < iResult.length; i++) {
                                var iResultItem = iResult[i];

                                if (iResultItem.type == "partialCredit") {
                                    var isThresholdGrading = iResultItem.responseDeclaration.thresholdGrading.toString();
                                    var isAbsoluteGrading = iResultItem.responseDeclaration.absoluteGrading.toString();
                                    var isRelativeGrading = iResultItem.responseDeclaration.relativeGrading.toString();
                                    var isAlgorithmicGrading = iResultItem.responseDeclaration.algorithmicGrading.toString();
                                    // Load absoulte grading
                                  if (isThresholdGrading == '0' && isAbsoluteGrading == '0' && isRelativeGrading == '0' && isAlgorithmicGrading == '0') {
                                    isAbsoluteGrading = '1';
                                  }
                                    if (isAbsoluteGrading == "1") {
                                        $('#absolute').attr('checked', true).val("1");
                                        $('.fieldAbsolute, .gradingThreshold').show();
                                        $('#txtAbsolute').val(iResultItem.responseDeclaration.absoluteGradingPoints);
                                    } else {
                                        $('#absolute').attr('checked', false).val("0");
                                        $('.fieldAbsolute, .gradingThreshold').hide();
                                        $('#txtAbsolute').val('1');
                                    }

                                    // Load relative grading
                                    if (isRelativeGrading == "1") {
                                        $('#cbRelative').attr('checked', true).val("1");
                                        $('.fieldRelative').show();
                                        // Only show order answer if it has value
                                        if (iResultItem.correctResponse.length) {
                                            $('.sortResponse').show();
                                        }
                                        $('#txtRelative').val(iResultItem.responseDeclaration.relativeGradingPoints);
                                    } else {
                                        $('#cbRelative').attr('checked', false).val("0");
                                        $('.fieldRelative, .sortResponse').hide();
                                        $('#txtRelative').val('1');
                                    }

                                    // Load threshold grading
                                    var isSumCap = iResultItem.responseDeclaration.isSumCap;
                                    if (isThresholdGrading === '1') {
                                        $('#cbThreshold').prop('checked', true).val('1');
                                        $partialPoint.css('display', 'block').show();
                                        $absoluteRelativePoint.hide();
                                        $('.fieldAbsolute, .gradingThreshold').hide();
                                        $('.fieldRelative, .sortResponse').hide();

                                        if (isSumCap === 'true') {
                                            $('#cb-cap-1').prop('checked', true);
                                            $fullCreatePartial.parent().addClass('hidden');
                                        } else {
                                            $fullCreatePartial.parent().removeClass('hidden');
                                            $('#cb-cap-2').prop('checked', true);
                                        }
                                    } else {
                                        $('#cbThreshold').prop('checked', false).val('0');
                                        $partialPoint.hide();
                                        $absoluteRelativePoint.show();
                                    }

                                    if (isAlgorithmicGrading === '1') {
                                        $('#cbAlgorithmic').prop('checked', true).val('1');
                                        $('#txtFullcreate').parent().addClass('is-disabled');
                                    } else {
                                        $('#cbAlgorithmic').prop('checked', false).val('0');
                                        $('#txtFullcreate').parent().removeClass('is-disabled');
                                    }

                                    //load threshold
                                    $('#threshold').val(iResultItem.responseDeclaration.partialGradingThreshold * 100);

                                    //load Full credit points
                                    $('#txtFullcreate').val(iResultItem.responseDeclaration.pointsValue);
                                    $fullCreatePartial.val(iResultItem.responseDeclaration.pointsValue);

                                    // load line matching setting
                                    var lineMatching = iResultItem.responseDeclaration.lineMatching == '1';
                                    $('#cbLineMatching').prop('checked', lineMatching)
                                    if (lineMatching) {
                                        $('#anchorPositionObject').val(iResultItem.responseDeclaration.anchorPositionObject || 'right').parent().show();
                                        $('#anchorPositionDestination').val(iResultItem.responseDeclaration.anchorPositionDestination || 'left').parent().show();
                                    } else {
                                        $('#anchorPositionObject').parent().hide();
                                        $('#anchorPositionDestination').parent().hide();
                                    }

                                    //Append correctResponse to drag and drop list
                                    //Clear the list before add new
                                    $('#sortResponse').empty();
                                    var currentContent = $("<div>" + CKEDITOR.instances[ckID].getData() + "</div>");
                                    var minValue = [];
                                    for (var n = 0; n < iResultItem.correctResponse.length; n++) {
                                        var desObj = currentContent.find(".partialDestinationObject[destidentifier=" + iResultItem.correctResponse[n].destIdentifier + "]");
                                        var desValue = desObj.text();
                                        var sourceObjArr = iResultItem.correctResponse[n].srcIdentifier.split(';');
                                        var sourceObjHtml = '';
                                        var si = 0;

                                        if (desObj.length == 0) {
                                            desValue = iResultItem.correctResponse[n].destIdentifier;
                                        }

                                        while (si < sourceObjArr.length) {
                                            var sourceObj = currentContent.find(".partialSourceObject[srcidentifier=\"" + sourceObjArr[si] + "\"]");
                                            var sourceValue = sourceObj.text();

                                            if (sourceObj.hasClass("partialAddSourceImage")) {
                                                sourceValue = sourceObj.prop("outerHTML");
                                            }

                                            sourceObjHtml += sourceValue + ';';
                                            si++;
                                        }

                                        sourceObjHtml = sourceObjHtml.slice(0, -1);

                                        $('#sortResponse').append('<li order="' + iResultItem.correctResponse[n].order + '" destIdentifier="' + iResultItem.correctResponse[n].destIdentifier + '" srcIdentifier="' + iResultItem.correctResponse[n].srcIdentifier + '">' + desValue + ' - ' + sourceObjHtml + '</li>');
                                    }

                                    $("#sortResponse").sortable("destroy").sortable();

                                    break;
                                }
                            }

                            refreshResponseId();
                        }
                    }]

                }],
                onOk: function () {
                    var isThresholdGrading = $('#cbThreshold').is(':checked');
                    var isSumCap = $('#cb-cap-1').is(':checked');
                    var $fullCreatePartial = $('#txtFullCreatePartial');
                    var $editorContent = $('<div>' + CKEDITOR.instances[ckID].getData() + '</div>');

                    // Check validate when change absolute/partial grading or partial grading
                    if (!isThresholdGrading) {
                        var arrDestination = [];
                        var isValidate = false;
                        var isAbsoluteGrading = $('#absolute').is(':checked');
                        for (var i = 0; i < iResult.length; i++) {
                            var iResultItem = iResult[i];

                            if (iResultItem.correctResponse.length) {
                                for (var n = 0; n < iResultItem.correctResponse.length; n++) {
                                    var destIdentifier = iResultItem.correctResponse[n].destIdentifier;
                                    var $destinationObj = $editorContent.find('.partialDestinationObject[destidentifier="' + destIdentifier + '"]');
                                    var numberDropable = 0;
                                    var numberOfAnswers = iResultItem.correctResponse[n].srcIdentifier.split(';');

                                    if (!$destinationObj.length) {
                                        $destinationObj = $editorContent.find('.partialDestinationObject.partialAddDestinationImage .hotSpot[destidentifier="' + destIdentifier + '"]');
                                    }

                                    numberDropable = $destinationObj.attr('numberdroppable');
                                    var  notRequireAllAnswers = 0
                                    if (isAbsoluteGrading) {
                                       notRequireAllAnswers = parseInt($destinationObj.attr('notrequireallanswers'), 10) || 0
                                    }
                                    if (typeof numberDropable !== 'undefined' && parseInt(numberDropable, 10) < numberOfAnswers.length && notRequireAllAnswers === 0) {
                                        isValidate = true;
                                        arrDestination.push(destIdentifier);
                                    }
                                }
                            }
                        }

                        if (isValidate) {
                            var msgNotification = 'Please increase the maximum number of hot spots a student can select. Then change properties grading to Absolute/Relative Grading. ';
                            msgNotification += 'Destination need increase number of hot spots is: ' + arrDestination.join(', ');
                            customAlert(msgNotification);
                            return false;
                        }
                    }

                    // Update for current textEntry
                    for (var n = 0; n < iResult.length; n++) {
                        var iResultItem = iResult[n];

                        if (iResultItem.type === 'partialCredit') {
                            var iResultItemRd = iResultItem.responseDeclaration;
                            var correctResponseTemp = [];

                            // Update response
                            iResultItemRd.absoluteGrading = $('#absolute').val().toString();
                            iResultItemRd.absoluteGradingPoints = $('#txtAbsolute').val().toString();
                            iResultItemRd.partialGradingThreshold = (parseInt($('#threshold').val()) / 100).toString();
                            iResultItemRd.relativeGrading = $('#cbRelative').val().toString();
                            iResultItemRd.relativeGradingPoints = $('#txtRelative').val().toString();
                            iResultItemRd.thresholdGrading = $('#cbThreshold').val();
                            iResultItemRd.algorithmicGrading = $('#cbAlgorithmic').val();
                            iResultItemRd.pointsValue = $('#txtFullcreate').val().toString();
                            iResultItemRd.lineMatching = $('#cbLineMatching').is(':checked') ? '1' : '0';
                            iResultItemRd.anchorPositionObject = $('#anchorPositionObject').val();
                            iResultItemRd.anchorPositionDestination = $('#anchorPositionDestination').val();

                            if (iResultItemRd.thresholdGrading === '1') {
                                if (isSumCap) {
                                    iResultItemRd.isSumCap = 'true';
                                } else {
                                    iResultItemRd.isSumCap = 'false';
                                }

                                correctResponseTemp = iResultItem.correctResponse;

                                if (isSumCap) {
                                    var maxThresholdPoints = [];

                                    for (var cr = 0, lenCr = correctResponseTemp.length; cr < lenCr; cr++) {
                                        var thresholdpointsArr = correctResponseTemp[cr].thresholdpoints;
                                        if (thresholdpointsArr !== undefined && thresholdpointsArr.length) {
                                            maxThresholdPoints.push(maxOfArray(thresholdpointsArr, 'pointsvalue'));
                                        }
                                    }

                                    if (maxThresholdPoints.length) {
                                        iResultItemRd.pointsValue = String(sumOfArray(maxThresholdPoints));
                                    } else {
                                        iResultItemRd.pointsValue = '0';
                                    }
                                } else {
                                    iResultItemRd.pointsValue = $fullCreatePartial.val().toString();
                                }
                            }

                            iResultItem.correctResponse = [];

                            if (iResultItemRd.algorithmicGrading !== '1') {
                                $('#sortResponse li').each(function (index) {
                                    var $self = $(this);

                                    iResultItem.correctResponse.push({
                                        order: index + 1,
                                        destIdentifier: $self.attr('destidentifier'),
                                        srcIdentifier: $self.attr('srcidentifier')
                                    });

                                    if (correctResponseTemp.length) {
                                        for (var cr = 0, lenCr = correctResponseTemp.length; cr < lenCr; cr++) {
                                            if (iResultItem.correctResponse[index].destIdentifier === correctResponseTemp[cr].destIdentifier) {
                                                iResultItem.correctResponse[index].thresholdpoints = correctResponseTemp[cr].thresholdpoints;
                                                break;
                                            }
                                        }
                                    }
                                });
                            }

                            break;
                        }
                    }

                    if (iResultItemRd.algorithmicGrading === '1') {
                        TestMakerComponent.isShowAlgorithmicConfiguration = true;
                    } else {
                        TestMakerComponent.isShowAlgorithmicConfiguration = false;
                    }

                    var lineMatchingConfig = (iResult.find(function(e) {
                      return e.responseDeclaration.lineMatching == 1
                    }) || {}).responseDeclaration;
                    var bodyClassList = CKEDITOR.instances[ckID].document.$.body.classList;
                    bodyClassList.remove(
                        'object-left',
                        'object-right',
                        'object-top',
                        'object-bottom',
                        'destination-left',
                        'destination-right',
                        'destination-top',
                        'destination-bottom'
                    )
                    if (lineMatchingConfig) {
                        bodyClassList.add(
                            'line-matching',
                            'object-' + lineMatchingConfig.anchorPositionObject,
                            'destination-' + lineMatchingConfig.anchorPositionDestination
                        )
                    } else {
                        bodyClassList.remove('line-matching')
                    }
                }
            };
        });
    }
});
