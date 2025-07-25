var $btnCreateHotspotImage = null;
var $btnClearHotspotImage = null;
var $btnHotspotImageUpload = null;
var $hotspotUploadFile = null;
var $hotspotImagePreview = null;
var $hotspotImagePercent = null;
var $hotspotImageDestionation = null;
var $hotspotPercent = null;
var $hotspotFloat = null;
var $hotspotFormUpload = null;
var $hotspotProperties = null;
var $hotspotStyleType = null;
var $idealImage = null;
var $hotspotTexttospeech = null;
var iframe;
var iframeId;
var widthImageOriginal;
var heightImageOriginal;

/**
 * Image Hot Spot Selection Plugin
 */
CKEDITOR.plugins.add('imagehotspotselection', {
    lang: 'en', // %REMOVE_LINE_CORE%
    icons: 'imagehotspotselection',
    requires: 'dialog',
    hidpi: true, // %REMOVE_LINE_CORE%
    init: function (editor) {
        var pluginName = 'insertImageHotSpotSelection';
        var isEdit = false;
        var elementImageHotspot = '';
        var currentImageHotspotId = '';

        editor.addCommand(pluginName, new CKEDITOR.dialogCommand(pluginName));

        editor.ui.addButton('ImageHotSpotSelection', {
            label: 'Insert Image Hot Spot Selection',
            command: pluginName,
            icon: this.path + 'icons/image.png',
            toolbar: 'insertImageHotSpotSelection, 30'
        });

        editor.widgets.add('imagehotspotselection', {
            inline: true,
            mask: true
        });

        editor.on('doubleclick', function (evt) {
            var element = evt.data.element;
            var parents = element.getParents();
            var parent;

            for (var i = 0; i < parents.length; i++) {
                parent = parents[i];
                if (parent.hasClass('imageHotspotInteraction')) {
                    break;
                }
            }

            elementImageHotspot = parent;
            editor.getSelection().selectElement(elementImageHotspot);
            evt.data.dialog = pluginName;

            if (element.hasClass('imageHotspotMarkObject')) {
                isEdit = true;
            }

            dblickHandlerToolbar(editor);
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

        CKEDITOR.dialog.add(pluginName, function (editor) {
            var htmlContent = '';

            htmlContent = '\
                <div id="hotspotSelection" class="hotspot-selection">\
                    <div class="hotspot-dialog">\
                        <div class="hotspot-header">\
                            <div class="ideal-image">Ideal Image about size height: 400px, width: 640px or scale: 1: 1.6</div>\
                            <div class="hotspot-header-action">\
                                <div class="hotspot-header-left">\
                                    <div class="hotspot-header-item">Percent:\
                                        <select name="hotspotImagePercent" id="hotspotImagePercent" class="cke_dialog_ui_input_select">\
                                            <option value="1">10%</option>\
                                            <option value="2">20%</option>\
                                            <option value="3">30%</option>\
                                            <option value="4">40%</option>\
                                            <option value="5">50%</option>\
                                            <option value="6">60%</option>\
                                            <option value="7">70%</option>\
                                            <option value="8">80%</option>\
                                            <option value="9">90%</option>\
                                            <option value="10" selected>100%</option>\
                                        </select>\
                                    </div>\
                                </div>\
                                <div class="hotspot-header-right">\
                                    <div class="hotspot-header-item">\
                                        <a href="javascript:void(0)" id="btnClearHotspotImage" class="is-visible cke_dialog_ui_button" role="button" hidefocus="true">\
                                            <span class="cke_dialog_ui_button">Clear Hot Spot</span>\
                                        </a>\
                                    </div>\
                                    <div class="hotspot-header-item">\
                                        <a href="javascript:void(0)" id="btnCreateHotspotImage" class="is-visible cke_dialog_ui_button" role="button" hidefocus="true">\
                                            <span class="cke_dialog_ui_button">Create Hot Spot</span>\
                                        </a>\
                                    </div>\
                                    <div class="hotspot-header-item">\
                                        <form name="hotspot-form-upload" id="hotspotFormUpload" class="hotspot-form-upload" lang="en" dir="ltr" method="POST" enctype="multipart/form-data">\
                                            <input type="file" name="file" class="hotspot-upload-file" id="hotspotUploadFile" value="Upload" />\
                                            <input type="hidden" name="id" id="objectId" />\
                                        </form>\
                                        <a href="javascript:void(0)" id="btnHotspotImageUpload" class="cke_dialog_ui_button" role="button" hidefocus="true">\
                                            <span class="cke_dialog_ui_button">Upload</span>\
                                        </a>\
                                    </div>\
                                </div>\
                            </div>\
                            <div class="hotspot-header-action hotspot-header-property" id="hotspotProperties" style="display: none;">\
                                <div class="g-1-3">\
                                    <div class="hotspot-list-property m-l-4 m-b-11">\
                                        <label class="widthLabel hotspot-label" for="absoluteGrading"><input type="radio" id="absoluteGrading" name="grading" checked="checked"> All or Nothing Grading</label>\
                                    </div>\
                                    <div class="hotspot-list-property m-l-4 m-b-11">\
                                        <label class="widthLabel hotspot-label" for="partialGrading"><input type="radio" id="partialGrading" name="grading"> Partial Credit Grading</label>\
                                    </div>\
                                    <div class="hotspot-list-property m-l-4">\
                                        <label class="widthLabel hotspot-label" for="algorithmicGrading"><input type="radio" id="algorithmicGrading" name="grading"> Algorithmic Grading</label>\
                                    </div>\
                                </div>\
                                <div class="g-2-3">\
                                    <div class="hotspot-list-property text-right">\
                                        <span class="widthLabel">Full Credit Points:</span><input type="text" value="1" name="txtPointTotal" id="txtPointTotal" class="txtFullcreate"/>\
                                    </div>\
                                    <div class="hotspot-list-property text-right">\
                                        <span class="widthLabel widthLabelSpecial">Maximum hot spots that can be selected:</span><input type="text" value="1" name="txtMaxChoice" id="txtMaxChoice" class="txtFullcreate"/>\
                                    </div>\
                                </div>\
                            </div>\
                        </div>\
                        <div class="hotspot-content">\
                            <div class="hotspot-container">\
                                <div id="hotspotImagePreview" class="hotspot-preview">\
                                    <img id="hotspotImageDestionation" src="" alt="">\
                                </div>\
                            </div>\
                            <div class="hotspot-container">\
                                <div class="u-m-t-10">Description of image:</div>\
                                <textarea id="image-hotspot-text-to-speech" class="img-text-to-speech"></textarea>\
                            </div>\
                        </div>\
                    </div>\
                </div>\
                <div style="display: none;" class="popup-image-hotspot" id="popupImageHotspot"></div>\
                <div style="display: none;" class="popup-property-image-hotspot" id="popupPropertyImageHotspot"></div>\
                <div style="display: none;" class="popup-overlay" id="popupImageOverlay"></div>\
                <div style="display: none;" id="hotspotStyleType" data-style="0"></div>';

            return {
                title: 'Insert Image Hot Spot Selection',
                minWidth: IS_V2 ? 730 : 560,
                minHeight: 300,
                contents:
                [
                    {
                        id: 'imageHotSpotSelection',
                        label: 'Settings',
                        elements:
                        [
                            {
                                type: 'html',
                                html: htmlContent,
                                onLoad: function () {
                                    $btnCreateHotspotImage = $('#btnCreateHotspotImage');
                                    $btnClearHotspotImage = $('#btnClearHotspotImage');
                                    $btnHotspotImageUpload = $('#btnHotspotImageUpload');
                                    $hotspotUploadFile = $('#hotspotUploadFile');
                                    $hotspotImagePreview = $('#hotspotImagePreview');
                                    $hotspotImagePercent = $('#hotspotImagePercent');
                                    $hotspotImageDestionation = $('#hotspotImageDestionation');
                                    $hotspotPercent = $('#hotspotPercent');
                                    $hotspotFloat = $('#hotspotFloat');
                                    $hotspotFormUpload = $('#hotspotFormUpload');
                                    $hotspotProperties = $('#hotspotProperties');
                                    $hotspotStyleType = $('#hotspotStyleType');
                                    $idealImage = $('#hotspotSelection').find('.ideal-image');
                                    $hotspotTexttospeech = $('#image-hotspot-text-to-speech');

                                    getUpDownNumber('#txtPointTotal', 0, 100);
                                    getUpDownNumber('#txtMaxChoice', 1, 100);

                                    /**
                                     * File Upload Change
                                     */
                                    $hotspotUploadFile.on('change', function() {
                                        var self = this;
                                        var file = this.value;
                                        var extension = (file.substr(file.lastIndexOf('.') + 1)).toLowerCase();
                                        var characterException = ['&', '#'];
                                        var fileExtensions = ['jpg', 'jpeg', 'png', 'svg'];

                                        if (file === '') {
                                            customAlert('Please select image file');
                                            return;
                                        }

                                        isCorrectFormat = _.some(fileExtensions, function(ext) {
                                            return ext === extension;
                                        });

                                        if (!isCorrectFormat) {
                                            var fileExts = fileExtensions.join(', ');
                                            var fileExtsMsg = 'Unsupported file type. Please select ' + fileExts + ' file.';
                                            customAlert(fileExtsMsg);
                                            return;
                                        }

                                        if ($hotspotImageDestionation.attr('src') !== '' && IS_V2) {
                                            customConfirm('Hot spots will be lost. Are you sure you want to change?', {
                                                minWidth: '500px',
                                                buttons: [
                                                    {
                                                        label: 'No',
                                                        color: 'grey',
                                                        style: "background: none;",
                                                        callback: function() {
                                                            $hotspotUploadFile.wrap('<form>').closest('form').get(0).reset();
                                                            $hotspotUploadFile.unwrap();
                                                        }
                                                    },
                                                    {
                                                        label: 'Yes but keep the hot spots',
                                                        color: 'red',
                                                        callback: function() {
                                                            $(self).parent().children('input[name="id"]').val(objectId);
                                                            uploadImageHotSpot($(self).parent().get(0), 100, imgUpload, self);
                                                        }
                                                    },
                                                    {
                                                        label: 'Yes',
                                                        color: 'red',
                                                        callback: function() {
                                                            $(document).find('.hotspot-item-close').trigger('click');
                                                            $(self).parent().children('input[name="id"]').val(objectId);
                                                            uploadImageHotSpot($(self).parent().get(0), 100, imgUpload, self);
                                                        }
                                                    }
                                                ]
                                            })
                                        } else if ($hotspotImageDestionation.attr('src') !== '') {
                                            var confirmHtml = '';
                                            confirmHtml += '<div id="uploadConfirm" class="uploadConfirm">';
                                            confirmHtml += '<div class="uploadConfirmMessage">Hot spots will be lost. Are you sure you want to change?</div>';
                                            confirmHtml += '<div class="uploadConfirmButton">';
                                            confirmHtml += '<button id="btnYesConfirmMessage" style="width:63px;">Yes</button>';
                                            confirmHtml += '<button id="btnYesKeepConfirmMessage">Yes but keep the hot spots</button>';
                                            confirmHtml += '<button id="btnNoConfirmMessage" style="width:63px;">No</button>';
                                            confirmHtml += '</div>';
                                            confirmHtml += '</div>';

                                            $(confirmHtml).modal({
                                                closeButton: false,
                                                resizable: false,
                                                title: ''
                                            });

                                            $('#btnYesConfirmMessage').on('click', function() {
                                                var $that = $(this);
                                                $(document).find('.hotspot-item-close').trigger('click');
                                                $that.parents("#modal").closeModal();
                                                $(self).parent().children('input[name="id"]').val(objectId);
                                                uploadImageHotSpot($(self).parent().get(0), 100, imgUpload, self);
                                            });

                                            $('#btnYesKeepConfirmMessage').on('click', function() {
                                                var $that = $(this);
                                                $that.parents("#modal").closeModal();
                                                $(self).parent().children('input[name="id"]').val(objectId);
                                                uploadImageHotSpot($(self).parent().get(0), 100, imgUpload, self);
                                            });

                                            $('#btnNoConfirmMessage').on('click', function () {
                                                var $that = $(this);
                                                $that.parents("#modal").closeModal();
                                                //Will not upload image
                                                $hotspotUploadFile.wrap('<form>').closest('form').get(0).reset();
                                                $hotspotUploadFile.unwrap();
                                                return false;
                                            });
                                        } else {
                                            $(self).parent().children('input[name="id"]').val(objectId);
                                            uploadImageHotSpot($(self).parent().get(0), 100, imgUpload, self);
                                        }
                                        return false;
                                    });

                                    /**
                                     * Create Hot Spot Type
                                     */
                                    $btnCreateHotspotImage.on('click', function() {
                                        showPopupImageHotspot();
                                        return false;
                                    });

                                    /**
                                     * Clear Hot Spot Type
                                     */
                                    $btnClearHotspotImage.on('click', function() {
                                        $(document).find('.hotspot-item-close').trigger('click');
                                        isShowBtnClearHotspotImage();
                                        return false;
                                    });

                                    /**
                                     * Change Percent Image Hot Spot Upload
                                     */
                                    $hotspotImagePercent.on('change', function() {
                                        var newWidth,
                                            newHeight;

                                        var $self = $(this);
                                        var percent = parseInt($self.val(), 10);

                                        $hotspotImageDestionation.attr('percent', percent);

                                        newWidth = parseInt(widthImageOriginal * percent / 10, 10);
                                        newHeight = parseInt(heightImageOriginal * percent / 10, 10);

                                        $hotspotImageDestionation
                                            .width(newWidth)
                                            .height(newHeight);

                                        $hotspotImagePreview
                                            .width(newWidth)
                                            .height(newHeight);
                                    });

                                    /**
                                     * Check Hot Spot is Absolute Grading or Partial Grading
                                     */
                                    $hotspotProperties.find('input[type="radio"][name="grading"]').on('change', function() {
                                        var $grading = $(this);
                                        var gradingMethod = $grading.attr('id');
                                        var $pointTotal = $('#txtPointTotal');

                                        $pointTotal.parent().removeClass('is-disabled');
                                        $pointTotal.val('1');

                                        if (gradingMethod === 'partialGrading') {
                                            $hotspotImagePreview.find('.hotspot-item-type').attr('data-correct', false);
                                        } else if (gradingMethod === 'algorithmicGrading') {
                                            $hotspotImagePreview.find('.hotspot-item-type').attr('data-correct', false);
                                            $hotspotImagePreview.find('.hotspot-item-type').attr('data-point', 0);
                                            $pointTotal.parent().addClass('is-disabled');
                                            $pointTotal.val('0');
                                        } else {
                                            $hotspotImagePreview.find('.hotspot-item-type').attr('data-point', 0);
                                        }
                                    });

                                    selectImageHotspotType('.hotspot-item-type');
                                    deleteImageHotspotType('.hotspot-item-close');
                                },
                                onShow: function () {
                                    refreshResponseId();
                                    checkElementRemoveIntoIResult();

                                    if (isEdit) {
                                        loadDataImageHotspot(elementImageHotspot);
                                    } else {
                                        resetDataImageHotspot();
                                    }

                                    // Hide tooltip
                                    $('#tips .tool-tip-tips').hide();
                                }
                            }
                        ]
                    }
                ],
                onOk: function () {
                    var buildHtml = '';
                    var buildHotspot = '';
                    var percent = $hotspotImagePercent.val();
                    var wImage = $hotspotImageDestionation.width();
                    var hImage = $hotspotImageDestionation.height();
                    var urlAbsoluteImage = $hotspotImageDestionation.attr('src');
                    var srcItemArr = [];
                    var correctArr = [];
                    var lenIResult = iResult.length;
                    var isAbsoluteGrading = $('#absoluteGrading').is(':checked');
                    var isAlgorithmicGrading = $('#algorithmicGrading').is(':checked');

                    // Check image upload
                    if ($hotspotImageDestionation.attr('src') === '') {
                        customAlert('Please upload image');
                        return false;
                    }

                    // Check hot spot add to image
                    if (!$hotspotImagePreview.find('.hotspot-item-type').length) {
                        customAlert('Please add hot spot for image');
                        return false;
                    }

                    // Show valid absolute grading or partial grading
                    if ($hotspotImagePreview.find('.hotspot-item-type').length < parseInt($('#txtMaxChoice').val(), 10)) {
                            customAlert('Maximum hot spots that can be selected cannot be ' +
                                    'greater than the total number of hot spots in the item.');
                            return false;
                    }

                    if (isAbsoluteGrading) {
                        var $hotspotItemCorrect = $hotspotImagePreview.find('.hotspot-item-type[data-correct="true"]');
                        if (!$hotspotItemCorrect.length) {
                            customAlert('You should select at least one correct answer');
                            return false;
                        }

                        if ($hotspotItemCorrect.length > parseInt($('#txtMaxChoice').val(), 10)) {
                            showMessageError();
                            return false;
                        }
                    } else if (!isAlgorithmicGrading) {
                        var result = validPartialGrading();
                        if (!result) {
                            showMessageError();
                            return false;
                        }
                    }

                    $hotspotImagePreview.find('.hotspot-item-type').each(function(index, hotspotItem) {
                        var $hotspotItem = $(hotspotItem);
                        var hsId = $hotspotItem.attr('id');
                        var hsTop = $hotspotItem.position().top;
                        var hsLeft = $hotspotItem.position().left;
                        var hsWidth = $hotspotItem.width();
                        var hsHeight = $hotspotItem.height();
                        var hsPoint = parseInt($hotspotItem.attr('data-point'), 10);
                        var hsType = $hotspotItem.attr('data-type');
                        var hsCorrect = JSON.parse($hotspotItem.attr('data-correct'));
                        var hsValue = $hotspotItem.attr('data-value');
                        var hsStyle = $hotspotItem.attr('style');
                        var hsHidden;

                        hsHidden = $hotspotItem.attr('data-hidden') == undefined ? false : JSON.parse($hotspotItem.attr('data-hidden'));

                        // Check absolute grading or partial grading
                        if (hsCorrect) {
                            correctArr.push({
                                identifier: hsId,
                                pointValue: hsPoint
                            });
                        }else {
                            if (hsPoint > 0) {
                                correctArr.push({
                                    identifier: hsId,
                                    pointValue: hsPoint
                                });
                            }
                        }

                        if (hsType === 'border-style') {
                            var hsShowBorder, hsFill, hsRolloverPreview;

                            hsShowBorder = $hotspotItem.attr('data-show-border') == undefined ? false : JSON.parse($hotspotItem.attr('data-show-border'));
                            hsFill = $hotspotItem.attr('data-fill') == undefined ? false : JSON.parse($hotspotItem.attr('data-fill'));
                            hsRolloverPreview = $hotspotItem.attr('data-rollover-preview') == undefined ? false : JSON.parse($hotspotItem.attr('data-rollover-preview'));

                            srcItemArr.showBorder = hsShowBorder;
                            srcItemArr.fill = hsFill;
                            srcItemArr.rolloverPreview = hsRolloverPreview;

                            srcItemArr.push({
                                identifier: hsId,
                                pointValue: hsPoint,
                                left: hsLeft,
                                top: hsTop,
                                width: hsWidth,
                                height: hsHeight,
                                typeHotSpot: hsType,
                                value: hsValue,
                                correct: hsCorrect,
                                hidden: hsHidden,
                                showBorder: hsShowBorder,
                                fill: hsFill,
                                rolloverPreview: hsRolloverPreview
                            });

                            buildHotspot += '<span class="hotspot-item-type" identifier="' + hsId + '" style="' + hsStyle + '" data-type="' + hsType + '" data-correct="' + hsCorrect + '" data-point="' + hsPoint + '" data-hidden="' + hsHidden + '" data-show-border="' + hsShowBorder + '" data-fill="' + hsFill + '" data-rollover-preview="' + hsRolloverPreview + '"><span class="hotspot-item-value">' + hsValue + '</span></span>';
                        } else {
                            srcItemArr.push({
                                identifier: hsId,
                                pointValue: hsPoint,
                                left: hsLeft,
                                top: hsTop,
                                width: hsWidth,
                                height: hsHeight,
                                typeHotSpot: hsType,
                                value: hsValue,
                                correct: hsCorrect,
                                hidden: hsHidden
                            });

                            buildHotspot += '<span class="hotspot-item-type" identifier="' + hsId + '" style="' + hsStyle + '" data-type="' + hsType + '" data-correct="' + hsCorrect + '" data-point="' + hsPoint + '" data-hidden="' + hsHidden + '"><span class="hotspot-item-value">' + hsValue + '</span></span>';
                        }
                    });

                    for (var i = 0; i < lenIResult; i++) {
                        var iResultItem = iResult[i];

                        if (iResultItem.type == 'imageHotSpot') {
                            //iResultItem.source.src = urlImage;
                            iResultItem.source.src = urlAbsoluteImage;
                            iResultItem.source.percent = percent;
                            iResultItem.source.imgorgw = widthImageOriginal;
                            iResultItem.source.imgorgh = heightImageOriginal;
                            iResultItem.source.width = wImage;
                            iResultItem.source.height = hImage;
                            iResultItem.source.maxhotspot = $('#txtMaxChoice').val();

                            iResultItem.sourceItem = srcItemArr;
                            iResultItem.correctResponse = correctArr;
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

                            iResultItem.responseDeclaration.pointsValue = $('#txtPointTotal').val();
                        }
                    }

                    var texttospeech = $hotspotTexttospeech.val();

                    texttospeech = convertTexttoHTML(texttospeech);

                    buildHtml += '<div id="RESPONSE_1" class="imageHotspotInteraction" style="width: ' + wImage + 'px;height: ' + hImage + 'px" contenteditable="false">';
                    buildHtml += '<img class="cke_reset cke_widget_mask imageHotspotMark" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D">';
                    buildHtml += '<img class="imageHotspotMarkObject" src="' + urlAbsoluteImage + '" style="width: ' + wImage + 'px;height: ' + hImage + 'px" texttospeech="' + texttospeech + '"/>';
                    buildHtml += buildHotspot;
                    buildHtml += '</div>&nbsp;';

                    var imagehotspotElement = CKEDITOR.dom.element.createFromHtml(buildHtml);
                    editor.insertElement(imagehotspotElement);

                    newResult = iResult;

                    // Hide image hot spot button
                    $('.cke_button__imagehotspotselection').parents('span.cke_toolbar').hide();

                    if (isAlgorithmicGrading) {
                        TestMakerComponent.isShowAlgorithmicConfiguration = true;
                    } else {
                        TestMakerComponent.isShowAlgorithmicConfiguration = false;
                    }
                },
                onCancel: function () {
                    isEdit = false;
                }
            };
        });
    }
});

/**
 * Show Popup Image Hot Spot
 * @return {[type]} [description]
 */
function showPopupImageHotspot() {
    var $popupImageHotspot = $('#popupImageHotspot');
    var $popupImageOverlay = $('#popupImageOverlay');
    var type = '';
    var html = '';

    html += '<div class="cke_dialog_body cke_dialog_image_hotspot"><div class="cke_dialog_title">Style of the hot spot</div>';
    html += '<a type="image" title="Remove" class="cke_dialog_close_button" id="btnImageHotspotClose"><span class="cke_label">X</span></a>';
    html += '<div class="hotspot-list">';
    html += '<div class="hotspot-list-item"><label class="widthLabel" for="type-number"><input type="radio" id="type-number" name="type-hotspot" data-type="number" checked="checked"/> Number</label></div>';
    html += '<div class="hotspot-list-item"><label class="widthLabel" for="type-letter"><input type="radio" id="type-letter" name="type-hotspot" data-type="letter"/> Letter</label></div>';
    html += '<div class="hotspot-list-item"><label class="widthLabel" for="type-circle"><input type="radio" id="type-circle" name="type-hotspot" data-type="circle"/> Circle</label></div>';
    html += '<div class="hotspot-list-item"><label class="widthLabel" for="type-checkbox"><input type="radio" id="type-checkbox" name="type-hotspot" data-type="checkbox"/> Checkbox</label></div>';
    html += '<div class="hotspot-list-item"><label class="widthLabel" for="type-border-style"><input type="radio" id="type-border-style" name="type-hotspot" data-type="border-style"/> Border Style</label></div>';
    html += '</div>';
    html += '<div class="cke_dialog_footer">';
    html += '<div class="cke_dialog_ui_hbox cke_dialog_footer_buttons">';
    html += '<div class="cke_dialog_ui_hbox_first" role="presentation"><a title="OK" id="btnImageHotspotOk" class="cke_dialog_ui_button cke_dialog_ui_button_ok" role="button" type="hotpot"><span class="cke_dialog_ui_button">OK</span></a></div>';
    html += '<div class="cke_dialog_ui_hbox_last" role="presentation"><a title="Cancel" id="btnImageHotspotCancel" class="cke_dialog_ui_button cke_dialog_ui_button_cancel" role="button"><span class="cke_dialog_ui_button">Cancel</span></a></div>';
    html += '</div>';
    html += '</div>';

    $popupImageHotspot
        .html(html)
        .show()
        .draggable({
            cursor: 'move',
            handle: '.cke_dialog_title'
        });

    $popupImageHotspot.find('input[type="radio"]')
                        .eq($hotspotStyleType.attr('data-style'))
                        .prop('checked', true);

    $popupImageOverlay.show();

    $popupImageHotspot.on('click', '#btnImageHotspotCancel, #btnImageHotspotClose', function () {
        $popupImageHotspot.html('').hide();
        $popupImageOverlay.hide();
    });

    $popupImageHotspot.on('click', '#btnImageHotspotOk', function() {
        $popupImageHotspot.find('input[name="type-hotspot"]').each(function (index, typeItem) {
            var $typeItem = $(typeItem);

            if ($typeItem.is(':checked')) {
                type = $typeItem.data('type');
                $hotspotStyleType.attr('data-style', $typeItem.parents('.hotspot-list-item').index());
            }
        });

        if (type !== '') {
            addImageHotspotType(type);
        }

        $popupImageHotspot.html('').hide();
        $popupImageOverlay.hide();

        isShowBtnClearHotspotImage();
    });
}

/**
 * Load Data Image Hot Spot
 * @param  {[type]} element [description]
 * @return {[type]}         [description]
 */
function loadDataImageHotspot(element) {
    var buildHotspot = '';
    var $element = $(element.$);

    for (var i = 0, lenIResult = iResult.length; i < lenIResult; i++) {
        var iResultItem = iResult[i];

        if (iResultItem.type === 'imageHotSpot' && iResultItem.responseIdentifier === $(element).attr('id')) {
            $hotspotImagePreview.css({
                'width': iResultItem.source.width + 'px',
                'height': iResultItem.source.height + 'px'
            });

            $hotspotImageDestionation
                //.attr('src', defaultSrc + iResultItem.source.src)
                .attr('src',  iResultItem.source.src)
                .css({
                    'width': iResultItem.source.width + 'px',
                    'height': iResultItem.source.height + 'px'
                })
                .show();

            var img = new Image();

            img.onload = function() {
                widthImageOriginal = this.width;
                heightImageOriginal = this.height;

                if (widthImageOriginal > 640) {
                    var ratio = widthImageOriginal / heightImageOriginal;
                    widthImageOriginal = 640;
                    heightImageOriginal = widthImageOriginal / ratio;
                }
            }

            img.src = iResultItem.source.src;

            $hotspotImagePercent.val(iResultItem.source.percent);

            $('#txtPointTotal').parent().removeClass('is-disabled');

            if (iResultItem.responseDeclaration.absoluteGrading == '1') {
                $hotspotProperties.find('input[type="radio"][id="absoluteGrading"]').prop('checked', true);
            } else if (iResultItem.responseDeclaration.algorithmicGrading == '1') {
                $hotspotProperties.find('input[type="radio"][id="algorithmicGrading"]').prop('checked', true);
                $('#txtPointTotal').parent().addClass('is-disabled');
            } else {
                $hotspotProperties.find('input[type="radio"][id="partialGrading"]').prop('checked', true);
            }

            $('#txtPointTotal').val(iResultItem.responseDeclaration.pointsValue);
            $('#txtMaxChoice').val(iResultItem.source.maxhotspot);

            for (var k = 0, lenSourceItem = iResultItem.sourceItem.length; k < lenSourceItem; k++) {
                var sourceItemChild = iResultItem.sourceItem[k];
                var scIdentifier = sourceItemChild.identifier;
                var scPoint = sourceItemChild.pointValue;
                var scLeft = sourceItemChild.left;
                var scTop = sourceItemChild.top;
                var scWidth = sourceItemChild.width;
                var scHeight = sourceItemChild.height;
                var scType = sourceItemChild.typeHotSpot;
                var scValue = sourceItemChild.value;
                var scCorrect = sourceItemChild.correct;
                var scHidden;

                scHidden = sourceItemChild.hidden == undefined ? false : sourceItemChild.hidden;

                if (scType === 'border-style') {
                    var scShowBorder, scFill, scRolloverPreview;

                    scShowBorder = sourceItemChild.showBorder == undefined ? false : sourceItemChild.showBorder;
                    scFill = sourceItemChild.fill == undefined ? false : sourceItemChild.fill;
                    scRolloverPreview = sourceItemChild.rolloverPreview == undefined ? false : sourceItemChild.rolloverPreview;

                    buildHotspot += '<div class="hotspot-item-type" id="' + scIdentifier + '" style="position: absolute; width: ' + scWidth + 'px; height: ' + scHeight + 'px; top: ' + scTop + 'px; left: ' + scLeft + 'px; line-height: ' + scHeight + 'px;" data-type="' + scType + '" data-point="' + scPoint + '" data-correct="' + scCorrect + '" data-value="' + scValue + '" data-hidden="' + scHidden + '" data-show-border="' + scShowBorder + '" data-fill="' + scFill + '" data-rollover-preview="' + scRolloverPreview + '"><span class="hotspot-item-value">'+ sourceItemChild.value +'</span><span class="hotspot-item-close">x</span></div>';
                } else {
                    buildHotspot += '<div class="hotspot-item-type" id="' + scIdentifier + '" style="position: absolute; width: ' + scWidth + 'px; height: ' + scHeight + 'px; top: ' + scTop + 'px; left: ' + scLeft + 'px; line-height: ' + scHeight + 'px;" data-type="' + scType + '" data-point="' + scPoint + '" data-correct="' + scCorrect + '" data-value="' + scValue + '" data-hidden="' + scHidden + '"><span class="hotspot-item-value">'+ sourceItemChild.value +'</span><span class="hotspot-item-close">x</span></div>';
                }
            }

            var texttospeech = $element.find('.imageHotspotMarkObject').attr('texttospeech');

            texttospeech = texttospeech === undefined ? '' : texttospeech;

            $hotspotTexttospeech.val(texttospeech);
        }
    }

    $btnCreateHotspotImage.removeClass('is-visible');
    $btnClearHotspotImage.removeClass('is-visible');
    $hotspotProperties.show();
    $idealImage.hide();

    $hotspotImagePreview
        .empty()
        .append($hotspotImageDestionation)
        .append(buildHotspot)
        .find('.hotspot-item-type')
        .draggable({
            'containment': 'parent'
        });

    $hotspotImagePreview.find('.hotspot-item-type').each(function(index, hotspot) {
        var $hotspot = $(hotspot);

        if ($hotspot.attr('data-type') === 'border-style') {
            $hotspot.resizable({
                'containment': 'parent',
                'handles': 'se',
                'aspectRatio': false,
                'minWidth': 30,
                'minHeight': 30,
                'maxWidth': 150,
                'maxHeight': 150
            });
        } else {
            $hotspot.resizable({
                'containment': 'parent',
                'handles': 'se',
                'aspectRatio': true,
                'minWidth': 30,
                'minHeight': 30,
                'maxWidth': 150,
                'maxHeight': 150
            });
        }
    });
}

/**
 * Reset Data Image Hot Spot
 * @return {[type]} [description]
 */
function resetDataImageHotspot() {
    $hotspotImagePercent.val(10);
    $btnClearHotspotImage.addClass('is-visible');
    $btnCreateHotspotImage.addClass('is-visible');
    $hotspotImageDestionation.attr('src', '');
    $hotspotFormUpload.get(0).reset();
    $hotspotImagePreview
        .css({
            width: '400px',
            height: '250px'
        })
        .find('.hotspot-item-type')
        .remove();
    $hotspotImageDestionation.hide();
    $hotspotProperties.hide();
    $hotspotProperties.find('input[type="radio"][id="absoluteGrading"]').prop('checked', true);
    $hotspotStyleType.attr('data-style', 0);
    $idealImage.show();
    $hotspotTexttospeech.val('');
}

/**
 * Add Image Hot Spot Type
 * @param {[type]} type [description]
 */
function addImageHotspotType(type) {
    var $hotspotItemType = $('<span/>');
    var value = '&nbsp;';
    var tempId = 0;
    var srcId = 0;

    if (type === 'number') {
        tempId = sequenceHotSpotID($hotspotImagePreview.find('.hotspot-item-type[data-type="number"]'), 'data-value', '');
        value = tempId;
    } else if (type === 'letter') {
        tempId = sequenceHotSpotLetter($hotspotImagePreview.find('.hotspot-item-type[data-type="letter"]'), 'data-value', '');
        value = alphabet[tempId - 1];
    }

    var imageWidth = $hotspotImageDestionation.width();
    var imageHeight = $hotspotImageDestionation.height();

    srcId = sequenceHotSpotID($hotspotImagePreview.find('.hotspot-item-type'), 'id', 'IHS_');

    var srcWidth = 40;
    var srcHeight = 40;
    // Position of hot spot always middle of image
    var srcTop = $hotspotImageDestionation.height() / 2 - srcHeight;
    var srcLeft = $hotspotImageDestionation.width() / 2 - srcWidth;

    $hotspotItemType
        .attr('id', srcId)
        .addClass('hotspot-item-type')
        .css({
            'position': 'absolute',
            'width': srcWidth + 'px',
            'height': srcHeight + 'px',
            'top': srcTop + 'px',
            'left': srcLeft + 'px',
            'line-height': srcHeight + 'px'
        })
        .attr('data-type', type)
        .attr('data-point', 0)
        .attr('data-correct', false)
        .attr('data-value', value)
        .attr('data-hidden', false)
        .append('<span class="hotspot-item-value">' + value + '</span><span class="hotspot-item-close">x</span>')
        .draggable({
            'containment': 'parent'
        });

    if (type === 'border-style') {
        $hotspotItemType
            .resizable({
                'containment': 'parent',
                'handles': 'se',
                'aspectRatio': false,
                'minWidth': 30,
                'minHeight': 30,
                'maxWidth': 150,
                'maxHeight': 150
            })
            .appendTo($hotspotImagePreview);
    } else {
        $hotspotItemType
            .resizable({
                'containment': 'parent',
                'handles': 'se',
                'aspectRatio': true,
                'minWidth': 30,
                'minHeight': 30,
                'maxWidth': 150,
                'maxHeight': 150
            })
            .appendTo($hotspotImagePreview);
    }
}

/**
 * Select Image Hot Spot Type
 * @param  {[type]} item [description]
 * @return {[type]}      [description]
 */
function selectImageHotspotType(item) {
    $(document).on('click', item, function(event) {
        var $self = $(this);
        var isBorderStyle = this.getAttribute('data-type');

        if (event.target !== this) {
            return;
        }

        isBorderStyle = isBorderStyle === 'border-style' ? true : false;

        $self.addClass('active');
        propertyImageHotspotType(isBorderStyle);
    });
}

function propertyImageHotspotType(bool) {
    var $popupPropertyImageHotspot = $('#popupPropertyImageHotspot');
    var $popupImageOverlay = $('#popupImageOverlay');
    var html = '';

    html += '<div class="cke_dialog_body cke_dialog_property_image_hotspot"><div class="cke_dialog_title">Hot Spot Properties</div>';
    html += '<a type="image" title="Remove" class="cke_dialog_close_button" id="btnPropertyImageHotspotClose"><span class="cke_label">X</span></a>';
    html += '<div class="hotspot-list">';
    html += '<div class="g-1-2">';
    html += '<div class="hotspot-list-item hotspot-list-item-property"><span class="widthLabel first">Point Value:</span><input type="text" value="0" name="txtPointValue" id="txtPointValue" class="txtFullcreate"/></div>';
    html += '</div>';
    html += '<div class="g-1-2">';
    html += '<div class="hotspot-list-item hotspot-list-item-property"><span class="widthLabel first">Is Correct:</span><input type="checkbox" id="hotspotCorrect" name="hotspot-correct"/></div>';
    html += '</div>';
    html += '<div class="g-1-2">';
    html += '<div class="hotspot-list-item hotspot-list-item-property last is-hidden"><span class="widthLabel last">Show Border on Select:</span><input type="checkbox" id="hotspotShowBorder" name="hotspot-show-border"/></div>';
    html += '</div>';
    html += '<div class="g-1-2">';
    html += '<div class="hotspot-list-item hotspot-list-item-property last"><span class="widthLabel first">Initial Hide:</span><input type="checkbox" id="hotspotHidden" name="hotspot-hidden"/></div>';
    html += '</div>';
    html += '<div class="g-1-2">';
    html += '<div class="hotspot-list-item hotspot-list-item-property last is-hidden"><span class="widthLabel last">Fill Hot Spot on Select:</span><input type="checkbox" id="hotspotFill" name="hotspot-fill"/></div>';
    html += '</div>';
    html += '<div class="g-1-2">';
    html += '<div class="hotspot-list-item hotspot-list-item-property is-hidden"><span class="widthLabel first">Rollover Preview:</span><input type="checkbox" id="hotspotRolloverPreview" name="hotspot-roller-preview" disabled="true"/></div>';
    html += '</div>';
    html += '</div>';
    html += '<div class="cke_dialog_footer">';
    html += '<div class="cke_dialog_ui_hbox cke_dialog_footer_buttons">';
    html += '<div class="cke_dialog_ui_hbox_first" role="presentation"><a title="OK" id="btnPropertyImageHotspotOk" class="cke_dialog_ui_button cke_dialog_ui_button_ok" role="button" type="hotpot"><span class="cke_dialog_ui_button">OK</span></a></div>';
    html += '<div class="cke_dialog_ui_hbox_last" role="presentation"><a title="Cancel" id="btnPropertyImageHotspotCancel" class="cke_dialog_ui_button cke_dialog_ui_button_cancel" role="button"><span class="cke_dialog_ui_button">Cancel</span></a></div>';
    html += '</div>';
    html += '</div>';

    $popupPropertyImageHotspot
        .html(html)
        .show()
        .draggable({
            cursor: 'move',
            handle: '.cke_dialog_title'
        });

    $popupImageOverlay.show();

    var i = 0;
    var j = 0;
    var tempPoint, tempChecked, tempHidden;

    $('#txtPointValue').ckUpDownNumber({
        maxNumber: 100,
        minNumber: 0,
        width: 18,
        height: 13
    });

    var $currentHotspot = $hotspotImagePreview.find('.hotspot-item-type.active');
    var currentHotspotHidden = $currentHotspot.attr('data-hidden');
    var isAbsoluteGrading = $('#absoluteGrading').is(':checked');
    var isAlgorithmicGrading = $('#algorithmicGrading').is(':checked');

    tempPoint = parseInt($currentHotspot.attr('data-point'), 10);
    tempChecked = JSON.parse($currentHotspot.attr('data-correct'));
    tempHidden = currentHotspotHidden == undefined ? false : JSON.parse(currentHotspotHidden);


    if (isAbsoluteGrading) {
        tempPoint = 0;
        $('#txtPointValue').parents('.hotspot-list-item').hide();
        $('#hotspotCorrect').parents('.hotspot-list-item').show();
    } else if (isAlgorithmicGrading) {
        tempPoint = 0;
        tempChecked = false;
        $('#txtPointValue').parents('.hotspot-list-item').hide();
        $('#hotspotCorrect').parents('.hotspot-list-item').hide();
    } else {
        $('#txtPointValue').parents('.hotspot-list-item').show();
        $('#hotspotCorrect').parents('.hotspot-list-item').hide();
    }

    $('#txtPointValue').val(tempPoint);
    $('#hotspotCorrect').prop('checked', tempChecked);
    $('#hotspotHidden').prop('checked', tempHidden);

    // Check if style of the hot spot is 'border-style'
    if (bool) {
        var currentHotspotShowBorder = $currentHotspot.attr('data-show-border');
        var currentHotspotFill = $currentHotspot.attr('data-fill');
        var currentHotspotRolloverPreview = $currentHotspot.attr('data-rollover-preview');
        var tempShowBorder, tempFill, tempRolloverPreview;

        // Show all properties for only 'border-style'
        $('#hotspotShowBorder').parents('.hotspot-list-item').show();
        $('#hotspotFill').parents('.hotspot-list-item').show();
        $('#hotspotRolloverPreview').parents('.hotspot-list-item').show();

        if (!isAlgorithmicGrading) {
            $('#hotspotHidden').parents('.hotspot-list-item').removeClass('last');
        }

        if (isAlgorithmicGrading) {
            $('#hotspotRolloverPreview').parents('.hotspot-list-item').addClass('last');
            $('#hotspotShowBorder').parents('.hotspot-list-item').removeClass('last');
            $('#hotspotFill').parents('.hotspot-list-item').removeClass('last');
        }

        // Asign value for properties of hot spot type is 'border-style'
        tempShowBorder = currentHotspotShowBorder == undefined ? false : JSON.parse(currentHotspotShowBorder);
        tempFill = currentHotspotFill == undefined ? false : JSON.parse(currentHotspotFill);
        tempRolloverPreview = currentHotspotRolloverPreview == undefined ? false : JSON.parse(currentHotspotRolloverPreview);

        $('#hotspotShowBorder').prop('checked', tempShowBorder);
        $('#hotspotFill').prop('checked', tempFill);
        $('#hotspotRolloverPreview').prop('checked', tempRolloverPreview);

        if (tempHidden) {
            $('#hotspotRolloverPreview').prop('disabled', false);
        }
    }

    $popupPropertyImageHotspot.on('change', 'input[type="checkbox"]', function() {
        var $self = $(this);

        if ($self.is(':checked')) {
            var checkboxId = this.id;

            if (checkboxId === 'hotspotHidden') {
                $('#hotspotRolloverPreview').prop('disabled', false);
            }

            // If show border on select is checked, fill hot spot on select
            // will not checked and vice versa
            if (checkboxId === 'hotspotShowBorder') {
                $('#hotspotFill').prop('checked', false);
            }

            if (checkboxId === 'hotspotFill') {
                $('#hotspotShowBorder').prop('checked', false);
            }
        }

        // Check if initial hide is not checked, roller preview checkbox is not enabled
        if (!$('#hotspotHidden').is(':checked')) {
            $('#hotspotRolloverPreview').prop('checked', false).prop('disabled', true);
        }
    });

    $('#btnPropertyImageHotspotCancel, #btnPropertyImageHotspotClose').unbind('click').on('click', function () {
        $popupPropertyImageHotspot.html('').hide();
        $popupImageOverlay.hide();
        $hotspotImagePreview.find('.hotspot-item-type').removeClass('active');
    });

    $('#btnPropertyImageHotspotOk').unbind('click').on('click', function() {
        var maxChoice = parseInt($('#txtMaxChoice').val(), 10);

        $currentHotspot.attr('data-point', parseInt($('#txtPointValue').val(), 10));

        if ($('#hotspotCorrect').is(':checked')) {
            $currentHotspot.attr('data-correct', true);
        } else {
            $currentHotspot.attr('data-correct', false);
        }

        if ($('#hotspotHidden').is(':checked')) {
            $currentHotspot.attr('data-hidden', true);
        } else {
            $currentHotspot.attr('data-hidden', false);
        }

        if (bool) {
            if ($('#hotspotShowBorder').is(':checked')) {
                $currentHotspot.attr('data-show-border', true);
            } else {
                $currentHotspot.attr('data-show-border', false);
            }

            if ($('#hotspotFill').is(':checked')) {
                $currentHotspot.attr('data-fill', true);
            } else {
                $currentHotspot.attr('data-fill', false);
            }

            if ($('#hotspotRolloverPreview').is(':checked')) {
                $currentHotspot.attr('data-rollover-preview', true);
            } else {
                $currentHotspot.attr('data-rollover-preview', false);
            }
        }

        if (!isAbsoluteGrading) {
            $currentHotspot.attr('data-correct', false);
        }

        if ($hotspotImagePreview.find('.hotspot-item-type[data-correct="true"]').length > maxChoice) {
            $currentHotspot.attr('data-correct', false);
            customAlert('Please increase the maximum number of hot spots a student can select.');
            return;
        }

        $hotspotImagePreview.find('.hotspot-item-type').removeClass('active');
        $popupPropertyImageHotspot.html('').hide();
        $popupImageOverlay.hide();
    });
}

/**
 * Delete Image Hot Spot Type
 * @param  {[type]} item [description]
 * @return {[type]}      [description]
 */
function deleteImageHotspotType(item) {
    $(document).on('click', item, function(event) {
        var $self = $(this);
        var $parent = $self.parent('.hotspot-item-type');

        if (event.target !== this) {
            return;
        }

        if ($self.hasClass('hotspot-item-close')) {
            $parent.remove();
            isShowBtnClearHotspotImage();
        }
    });
}

/**
 * Show or Hide Clear Image Hotspot Button
 */
function isShowBtnClearHotspotImage() {
    if ($hotspotImagePreview.find('.hotspot-item-type').length > 0) {
        $btnClearHotspotImage.removeClass('is-visible');
    } else {
        $btnClearHotspotImage.addClass('is-visible');
    }
}

/**
 * Generate Sequence Hot Spot ID
 * @param  {[type]} element [description]
 * @param  {[type]} prefix  [description]
 * @return {[type]}         [description]
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
 * Generate Sequence Hot Spot Letter
 * @param  {[type]} element     [description]
 * @param  {[type]} elementAttr [description]
 * @param  {[type]} prefix      [description]
 * @return {[type]}             [description]
 */
function sequenceHotSpotLetter(element, elementAttr, prefix) {
    var $element = $(element);
    var len = $element.length;
    var srcId = prefix + (len + 1);

    for (var m = 0; m < len; m++) {
        var resId = prefix + (m + 1);
        var isOnlyOne = true;
        var elM = parseInt(alphabet.indexOf($element.eq(m).attr(elementAttr)), 10) + 1;

        if (elM != resId) {
            for (var k = 0; k < len; k++) {
                var elK = parseInt(alphabet.indexOf($element.eq(k).attr(elementAttr)), 10) + 1;
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
 * Show Message Error
 */
function showMessageError() {
    customAlert('Students will not be able to earn the maximum points possible'
            + ' on this question based on the current point allocation.'
            + ' You can 1) reduce the total points possible on the item,'
            + ' 2) increase the maximum number of hot spots a student can select,'
            + ' and/or 3) increase the points earned by certain hot spots.');
}

/**
 * Valid Partial Grading
 * @return {[type]} [description]
 */
function validPartialGrading() {
    var totalPoint = 0;
    var pointArr = [];
    var totalMaxSelected = 0;
    var result = false;
    var fullMaxChoice = parseInt($('#txtMaxChoice').val(), 10);
    var fullPoints = parseInt($('#txtPointTotal').val(), 10);
    var largest = 0;

    $hotspotImagePreview.find('.hotspot-item-type').each(function(index, itemType) {
        var $itemType = $(itemType);
        var point = parseInt($itemType.attr('data-point'), 10);

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
 * Upload Image Hot Spot
 * @param  {[type]} form               [description]
 * @param  {[type]} destinationPercent [description]
 * @param  {[type]} action_url         [description]
 * @param  {[type]} currentElement     [description]
 * @return {[type]}                    [description]
 */
function uploadImageHotSpot(form, destinationPercent, action_url, currentElement) {
    var $body = $('body');
    if (IS_V2) {
    ShowBlock($body, 'Uploading');
    setTimeout(function () {
        $(".blockUI.blockOverlay").css({ 'z-index': 11010, opacity: 0.3 })
        $(".blockUI.blockMsg.blockElement").css('z-index', '11011')
    }, 100)

    } else {
        $body.ckOverlay();
    }

    // Create the iframe...
    iframe = document.createElement('iframe');
    iframe.setAttribute('id', 'upload_iframe');
    iframe.setAttribute('name', 'upload_iframe');
    iframe.setAttribute('width', '0');
    iframe.setAttribute('height', '0');
    iframe.setAttribute('border', '0');
    iframe.setAttribute('style', 'width: 0; height: 0; border: none;');

    // Add to document...
    form.parentNode.appendChild(iframe);

    iframeId = document.getElementById('upload_iframe');

    // Add event...
    var eventHandler = function () {
        var content = '';

        if (iframeId.detachEvent) {
            iframeId.detachEvent('onload', eventHandler);
        } else {
            iframeId.removeEventListener('load', eventHandler, false);
        }

        // Message from server...
        if (iframeId.contentDocument) {
            content = iframeId.contentDocument.body.innerHTML;
        } else if (iframeId.contentWindow) {
            content = iframeId.contentWindow.document.body.innerHTML;
        } else if (iframeId.document) {
            content = iframeId.document.body.innerHTML;
        }

        var data = $.parseJSON(content.substr(content.indexOf('{'), content.lastIndexOf('}') - content.indexOf('{') + 1));

        // Del the iframe...
        setTimeout(removeIFrame, 250);

        var reader = new FileReader();
        var file = $(form).find('input[type="file"]').get(0).files[0];

        reader.readAsDataURL(file);

        reader.onload = function() {
            var img = new Image();

            img.src = reader.result;

            img.onload = function() {
                // Get width and height original of image
                widthImageOriginal = this.width;
                heightImageOriginal = this.height;

                if (widthImageOriginal > 640) {
                    var ratio = widthImageOriginal / heightImageOriginal;
                    widthImageOriginal = 640;
                    heightImageOriginal = widthImageOriginal / ratio;
                }

                $hotspotImageDestionation
                    //.attr('src', GetViewReferenceImg + data.url)
                    .attr('src', data.absoluteUrl)
                    .css({
                        'width': widthImageOriginal + 'px',
                        'height': heightImageOriginal + 'px'
                    })
                    .show();

                $hotspotImagePreview
                    .width(widthImageOriginal)
                    .height(heightImageOriginal);
            };
        };

        $btnCreateHotspotImage.removeClass('is-visible');
        $hotspotProperties.show();
        $hotspotFormUpload.get(0).reset();
        $idealImage.hide();

        // Hide overlay
        if (IS_V2) {
            $body.unblock();
        } else {
            $body.ckOverlay.destroy();
        }

    };

    if (iframeId.addEventListener) {
        iframeId.addEventListener('load', eventHandler, true);
    }

    if (iframeId.attachEvent) {
        iframeId.attachEvent('onload', eventHandler);
    }

    // Set properties of form...
    form.setAttribute('target', 'upload_iframe');
    form.setAttribute('action', action_url);
    form.setAttribute('method', 'post');
    form.setAttribute('enctype', 'multipart/form-data');
    form.setAttribute('encoding', 'multipart/form-data');

    // Submit the form...
    form.submit();
}

/**
 * Remove Iframe After Upload Image
 * @return {[type]} [description]
 */
function removeIFrame() {
    //Check iFrame and only remove iframe has existed
    if (iframeId !== null && iframeId.parentNode !== null) {
        iframeId.parentNode.removeChild(iframeId);
    }
}
