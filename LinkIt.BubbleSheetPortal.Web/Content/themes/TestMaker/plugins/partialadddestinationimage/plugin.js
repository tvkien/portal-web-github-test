CKEDITOR.plugins.add('partialadddestinationimage', {
    lang: 'en', // %REMOVE_LINE_CORE%
    icons: 'partialadddestinationimage',
    requires: 'dialog',
    hidpi: true, // %REMOVE_LINE_CORE%
    init: function (editor) {
        var pluginName = 'insertPartialAddDestinationImage';

        editor.addCommand(pluginName, new CKEDITOR.dialogCommand(pluginName));

        editor.ui.addButton('PartialAddDestinationImage', {
            label: 'Drag and Drop - Destination Image',
            command: pluginName,
            icon: this.path + 'icons/drawtool.png'
        });

        editor.widgets.add('partialadddestinationimage', {
            inline: true,
            mask: true,
            allowedContent: {
                p: {
                    classes: 'partialPartialAddDestinationImage',
                    attributes: '!id,name,contenteditable'
                }
            },
            template: '<p class="partialPartialAddDestinationImage"></p>'
        });

        var isEditPartialCredit = false,
            currentPartialCreditResId = "",
            elePartialCredit = ""; //This use for first time load popup

        editor.on('doubleclick', function (evt) {
            // Show popup when double click
            var element = evt.data.element;

            if (element.hasClass('partialAddDestinationImageMark')) {
                var parents = element.getParents();
                var parent;

                for (var i = 0; i < parents.length; i++) {
                    parent = parents[i];
                    if (parent.hasClass('partialAddDestinationImage')) {
                        break;
                    }
                }

                // Move selection to parent of multipleChoiceMark
                elePartialCredit = parent;
                editor.getSelection().selectElement(elePartialCredit);
                evt.data.dialog = pluginName;

                // The status to editor know this is update
                isEditPartialCredit = true;
                isEditPCredit = true; // Remove item

                currentPartialCreditResId = elePartialCredit.getId();

                dblickHandlerToolbar(editor);
            }
        });

        CKEDITOR.dialog.add(pluginName, function (editor) {
            var myhtml = '';
            myhtml = '<div id="partialDestinationImage" class="divDrawTool contentArea">';
            myhtml += '<div class="ideal-image">Ideal Image about size height: 400px, width: 640px or scale: 1: 1.6</div>';
            myhtml += '        <div class="fleft" style="display: none;">Width: <input type="text" id="nWidth" class="drawWidth" value="400"  tabindex="1" /></div>';
            myhtml += '        <div class="fleft divDrawHeight" style="display: none;">Height: <input type="text" id="nHeight" class="drawHeight" value="200" tabindex="2" /></div>';
            myhtml += '        <div id="idPercentDestinationImage" class="fleft divFloat">Percent<select style="width : 100px; margin-left: 10px;" aria-labelledby="cke_272_label" class="cke_dialog_ui_input_select" id="selectPercentDestinationImage">';
            myhtml += '             <option value="1">10%</option>';
            myhtml += '             <option value="2">20%</option>';
            myhtml += '             <option value="3">30%</option>';
            myhtml += '             <option value="4">40%</option>';
            myhtml += '             <option value="5">50%</option>';
            myhtml += '             <option value="6">60%</option>';
            myhtml += '             <option value="7">70%</option>';
            myhtml += '             <option value="8">80%</option>';
            myhtml += '             <option value="9">90%</option>';
            myhtml += '             <option value="10">100%</option>';
            myhtml += '            </select></div>';
            myhtml += '        <div class="fleft divFloat">Float: <select style="width : 60px" id="nFloat" aria-labelledby="cke_273_label" class="cke_dialog_ui_input_select selectAlignment" tabindex="3"><option value="">None</option><option value="float: left;">Left</option><option value="float: right;">Right</option></select></div>';
            myhtml += '        <div class="fright fields_upload_content divDesUpload">';
            myhtml += '             <div class="group_upload_image">';
            myhtml += '                     <div class="divUpload"><form name="form-upload" class="formImageUpload" id="formPCImageUpload_destination" lang="en" dir="ltr" method="POST" enctype="multipart/form-data">';
            myhtml += '                         <input type="file" title="Upload Image" size="60" name="file" aria-labelledby="cke_262_label" class="formPCImageUploadFile formImageHotspot"/>';
            myhtml += '                         <input type="hidden" name="id" id="objectId" />';
            myhtml += '                     </form>';
            myhtml += '                     <a class="cke_dialog_ui_button btnHotSpot addImages" title="Upload Image" tabindex="4">Upload Image</a></div>';
            myhtml += '                  <div class="divUpload"><a class="cke_dialog_ui_button btnHotSpot addHotSpot" id="btnAddHotSpot" type="hotpot" title="Add Destination">Add Destination</a></div>';
            myhtml += '                  <div class="divUpload"><a class="cke_dialog_ui_button btnHotSpot btnClearDestinations" id="btnClearHotSpot" title="Clear All Destinations" style="display: none;">Clear All Destinations</a></div>';
            myhtml += '             </div>';
            myhtml += '        </div>';
            myhtml += '    <div class="clear10"></div> ';
            myhtml += '    <div id="resizeContentArea" class="imageDestinationArea"><div class="divContent resizeContent"><img id="destinationImage" style="display: none;" alt="" src="" /><div class="contentInputs"></div><div class="grid-area" id="grid-area"></div></div></div>';
            myhtml += '    <div id="txtEnterInput" class="txtEnterInput" style="display: none;"><div class="itemSelected"></div><span class="close_item">x</span></div>';
            myhtml += '    <div class="texttospeech">Description of image: <br /><textarea id="txtToSpeech"></textarea></div>';
            myhtml += '</div><img src="#" style="display: none;" alt="your image" id="ImageDemo" />';
            myhtml += '<div style="display: none;" id="popupList"></div>';
            myhtml += '<div style="display: none;" id="popupHotspots"></div>';
            myhtml += '<div style="display: none;" class="overlayList" id="overlayList"></div>';

            return {
                title: 'Drag and Drop - Destination Image',
                minWidth: IS_V2 ? 800 : 630,
                minHeight: 200,
                resizable: CKEDITOR.DIALOG_RESIZE_NONE,
                contents: [{
                    id: 'partialadddestinationimage',
                    label: 'Settings',
                    elements: [{
                        type: 'html',
                        html: myhtml,
                        onLoad: function () {
                            var destinationImagePercent = $('#selectPercentDestinationImage');
                            var currentTabIndex = 1;
                            $('.contentArea').parents('.cke_dialog_contents_body').addClass('chrome');
                            var $gridArea = $('#grid-area');

                            getChangeDimensions('#nWidth', '#resizeContentArea .resizeContent', 'width');
                            getChangeDimensions('#nHeight', '#resizeContentArea .resizeContent', 'height');

                            $('#upload_image').on('click', function () {

                                $('.btnAddInput').css('visibility', 'hidden');
                                $('.group_upload_image').removeClass('is-hide');

                                //show image of hotspot
                                $("#destinationImage").show();

                                //hide textinput and show hotpot
                                $('.resizeContent .txtEnterInput[type="hotpot"]').show();
                            });

                            $('#btnAddHotSpot').on('click', function () {
                                showHotspotDialog();
                                return false;
                            });

                            $('#btnClearHotSpot').on('click', function () {
                                $("#resizeContentArea .contentInputs .txtEnterInput").each(function () {
                                    $(this).find(".close_item").trigger("click"); // This is trigger remove hotspots
                                });
                                return false;
                            });

                            $(".formPCImageUploadFile").unbind("change").change(function () {
                                var file = this.value;
                                var extension = file.substr((file.lastIndexOf('.') + 1)).toLowerCase();
                                var that = this;
                                showImageDemo(that);

                                var isCorrectFormat = false;
                                var fileExtensions = ['jpg', 'jpeg', 'png', 'svg'];

                                isCorrectFormat = _.some(fileExtensions, function (ext) {
                                    return ext === extension;
                                });

                                if (!isCorrectFormat) {
                                    var fileExts = fileExtensions.join(', ');
                                    var fileExtsMsg = 'Unsupported file type. Please select ' + fileExts + ' file.';
                                    customAlert(fileExtsMsg);
                                    return;
                                }



                                if ($("#destinationImage").attr("src") != "" && IS_V2) {
                                    customConfirm('Destinations will be lost. Are you sure you want to change?', {
                                            minWidth: '500px',
                                            buttons: [
                                                {
                                                    label: 'No',
                                                    color: 'grey',
                                                    style: "background: none;",
                                                    callback: function() {
                                                      $('#formPCImageUpload_destination').find('input[type="file"]').wrap('<form>').closest('form').get(0).reset();
                                                      $('#formPCImageUpload_destination').find('input[type="file"]').unwrap();
                                                    }
                                                },
                                                {
                                                    label: 'Yes but keep the destinations',
                                                    color: 'red',
                                                    callback: function() {
                                                      $(that).parent().children("input[name='id']").val(objectId);
                                                      partialCreditImageUpload($(that).parent().get(0), destinationImagePercent, imgUpload, that);
                                                      return true;
                                                    }
                                                },
                                                {
                                                    label: 'Yes',
                                                    color: 'red',
                                                    callback: function() {
                                                      $("#resizeContentArea .contentInputs .txtEnterInput").each(function () {
                                                        $(this).find(".close_item").trigger("click"); // This is trigger remove hotspots
                                                      });

                                                      $(that).parent().children("input[name='id']").val(objectId);
                                                      partialCreditImageUpload($(that).parent().get(0), destinationImagePercent, imgUpload, that);
                                                    }
                                                }
                                            ]
                                      })
                                }else if($("#destinationImage").attr("src") != "" ) {
                                    var htmlPopup = '<div id="uploadConfirm" class="uploadConfirm">' +
                                      '<div class="uploadConfirmMessage">Destinations will be lost. Are you sure you want to change?</div>' +
                                      '<div class="uploadConfirmButton">' +
                                      '<button id="bntYesConfirmMessage" style="width:63px;">Yes</button>' +
                                      '<button id="bntYesKeepConfirmMessage">Yes but keep the destinations</button>' +
                                      '<button id="bntNoConfirmMessage" style="width:63px;">No</button>' +
                                      '</div>' +
                                      '</div>';

                                    $(htmlPopup).modal({
                                        closeButton: false,
                                        resizable: false,
                                        title: "&nbsp;"
                                    });

                                    $("#uploadConfirm #bntYesConfirmMessage").unbind("click").click(function () {
                                        //Clear all hot spots
                                        $(this).parents("#modal").closeModal();
                                        $("#resizeContentArea .contentInputs .txtEnterInput").each(function () {
                                            $(this).find(".close_item").trigger("click"); // This is trigger remove hotspots
                                        });

                                        $(that).parent().children("input[name='id']").val(objectId);
                                        partialCreditImageUpload($(that).parent().get(0), destinationImagePercent, imgUpload, that);
                                    });

                                    $("#uploadConfirm #bntYesKeepConfirmMessage").unbind("click").click(function () {
                                        //Clear image without hot spots
                                        $(this).parents("#modal").closeModal();
                                        $(that).parent().children("input[name='id']").val(objectId);
                                        partialCreditImageUpload($(that).parent().get(0), destinationImagePercent, imgUpload, that);
                                        return true;
                                    });

                                    $("#uploadConfirm #bntNoConfirmMessage").unbind("click").click(function () {
                                        $(this).parents("#modal").closeModal();
                                        //Will not upload image
                                        $('#formPCImageUpload_destination').find('input[type="file"]').wrap('<form>').closest('form').get(0).reset();
                                        $('#formPCImageUpload_destination').find('input[type="file"]').unwrap();
                                        return false;
                                    });

                                } else {
                                    $(that).parent().children("input[name='id']").val(objectId);
                                    partialCreditImageUpload($(that).parent().get(0), destinationImagePercent, imgUpload, that);
                                }

                            });

                            $('.resizeContent').on('click', '.txtEnterInput', function (event) {
                                if (!$(event.target).is("span.close_item")) {
                                    var idItem = $(this).attr('id');
                                    getListItem('#popupList', '#overlayList', idItem);
                                }

                            });
                            $('#partialDestinationImage').parents(".cke_dialog_contents").bind('keydown', function (e) {
                                if (e.keyCode == 9) {
                                    $('[tabindex=' + currentTabIndex + ']').focus();
                                    currentTabIndex += 1;

                                    if (currentTabIndex > $('#partialDestinationImage').parents(".cke_dialog_contents").find("[tabindex]").length) {
                                        currentTabIndex = 1;
                                    }
                                    return false;
                                }
                            });

                            destinationImagePercent.change(function (e) {
                                /* Act on the event */
                                var curW, curH, newW, newH, nPercent, tempPercent;
                                var desImage = $('#destinationImage');
                                var reContent = $('#resizeContentArea .resizeContent');
                                var percentCur = parseInt($(this).val());
                                desImage.attr('percent', percentCur);

                                curW = desImage.width();
                                curH = desImage.height();

                                desImage.width((widthofImageCurrent * percentCur) / 10);
                                desImage.height((heightofImageCurrent * percentCur) / 10);

                                reContent.width((widthofImageCurrent * percentCur) / 10);
                                reContent.height((heightofImageCurrent * percentCur) / 10);

                                newW = desImage.width();
                                newH = desImage.height();

                                if (newW > curW || newH > curH) {
                                    nPercent = percentCur;
                                    resizeWidthIncreaseContentArea(desImage.width(), nPercent, oPercent);
                                    resizeHeightIncreaseContentArea(desImage.height(), nPercent, oPercent);
                                    oPercent = nPercent;

                                } else {
                                    oPercent = percentCur;
                                    resizeWidthContentArea(desImage.width(), widthofImageCurrent, percentCur);
                                    resizeHeightContentArea(desImage.height(), heightofImageCurrent, percentCur);
                                }

                            });

                            $gridArea
                                .hide()
                                .draggable({
                                    containment: 'parent'
                                })
                                .resizable({
                                    containment: 'parent'
                                });
                        },
                        onShow: function () {
                            refreshPartialCredit();

                            // Hide tooltip
                            $('#tips .tool-tip-tips').css({
                                'display': 'none'
                            });

                            var totalIndex = $('#partialDestinationImage').find("[tabindex]").length;

                            // Add index for Ok and cancel button on popup
                            $('#partialDestinationImage').parents(".cke_dialog_contents").find(".cke_dialog_ui_button_ok").attr("tabindex", totalIndex + 1);
                            $('#partialDestinationImage').parents(".cke_dialog_contents").find(".cke_dialog_ui_button_cancel").attr("tabindex", totalIndex + 2);


                            if (isEditPartialCredit == false) {
                                // Reset Partial Credit
                                resetNewData();
                            } else {
                                loadPartialDestinationImage(elePartialCredit);
                            }

                            refreshResponseId();

                            isShowClearBtnHotspot();
                            refreshGridArea();

                            if (IS_V2) {
                              $('.divDesUpload input.formPCImageUploadFile.formImageHotspot').css('height', '0')
                              $('.divDesUpload .cke_dialog_ui_button.btnHotSpot').addClass('btn-blue').css({ margin: '0', padding: '8px', 'font-weight': 'bold', 'text-decoration': 'none' });
                              $('.divDesUpload .cke_dialog_ui_button.btnHotSpot.addImages').addClass('btn-blue');

                              $('.cke_dialog_ui_button.addImages').unbind('click').click(function () {
                                  $('.divDesUpload input.formPCImageUploadFile.formImageHotspot').trigger('click')
                              })
                            }
                        }
                    }]

                }],
                onOk: function () {
                    var percent = $('#selectPercentDestinationImage').val();
                    var widthofImage = $("#destinationImage").width();
                    var heightofImage = $("#destinationImage").height();
                    var imgUrl = $("#destinationImage").attr("src");
                    var buildHotspot = "";
                    var buildHTMl = "";
                    var destinationObject = [];
                    var correctResponse = [];
                    var indexPartialCredit = 0;
                    var i = 0;
                    var isThresholdGrading = iResult[0].responseDeclaration.thresholdGrading;
                    var textToSpeech = $('.texttospeech:visible').find('#txtToSpeech');
                    var tts = convertTexttoHTML(textToSpeech.val());
                    // Check list source if empty will be return
                    if ($("#destinationImage").attr("src") === "") {
                        alert("Please upload image!");
                        return false;
                    }

                    //Check text field and hotspot if empty will be return
                    if ($("#resizeContentArea .contentInputs .txtEnterInput").length == 0) {
                        alert("Please add destinations.");
                        return false;
                    }

                    for (i = 0; i < iResult.length; i++) {
                        if (iResult[i].type == "partialCredit") {
                            destinationObject = iResult[i].destination;
                            indexPartialCredit = i;
                            correctResponse = iResult[i].correctResponse;
                        }
                    }
                    var pathUrl = $('#destinationImage').attr('src').replace(GetViewReferenceImg, '');

                    //Show the confirm if user has not select answer or some text field or hotspots are not select
                    var isConfirmShowed = false;

                    $("#resizeContentArea .contentInputs .txtEnterInput").each(function (index, textInput) {
                        var $textInput = $(textInput);
                        var textInputId = $textInput.attr('id');
                        var textInputStyle = $textInput.attr('style');
                        var textInputLeft = $textInput.css('left');
                        var textInputTop = $textInput.css('top');
                        var textInputNumberDroppable = $textInput.attr('numberDroppable');
                        var gridcell = $textInput.attr('gridcell');
                        var thresholdPoints = [];

                        textInputNumberDroppable = textInputNumberDroppable == undefined ? '1' : textInputNumberDroppable;
                         //to reduce the migration old data load, decided to set the default state as opposed to the checkbox state
                        var notRequireAllAnswers =  $textInput.attr('notrequireallanswers')|| '0';

                        // pdate destination
                        for (i = 0; i < destinationObject.length; i++) {
                            if (textInputId == destinationObject[i].destIdentifier) {

                                if (gridcell !== undefined && gridcell === 'true') {
                                    buildHotspot += '<div class="hotSpot" destidentifier="' + textInputId + '" style="' + textInputStyle + '" gridcell="true" numberDroppable="' + textInputNumberDroppable + '" notRequireAllAnswers="' + notRequireAllAnswers + '">' + textInputId + '</div>';
                                } else {
                                    buildHotspot += '<div class="hotSpot" destidentifier="' + textInputId + '" style="' + textInputStyle + '" numberDroppable="' + textInputNumberDroppable + '" notRequireAllAnswers="' + notRequireAllAnswers  +  '">' + textInputId + '</div>';
                                }

                                destinationObject[i].desheight = heightofImage;
                                destinationObject[i].deswidth = widthofImage;
                                destinationObject[i].group = $("#resizeContentArea .resizeContent").attr("groupid");
                                destinationObject[i].height = heightofImage / 10;
                                destinationObject[i].width = widthofImage / 10;
                                destinationObject[i].left = textInputLeft;
                                destinationObject[i].top = textInputTop;
                                destinationObject[i].percent = percent;
                                break;
                            }
                        }

                        if (isThresholdGrading && $textInput.find('.threshold-image').length) {
                            thresholdPoints = getPointItemThreshold($textInput.find('.threshold-image'));
                        }

                        // Update correctResponse
                        var isUpdateResponse = false;
                        for (i = 0; i < correctResponse.length; i++) {
                            if (textInputId === correctResponse[i].destIdentifier) {
                                correctResponse[i].srcIdentifier = $textInput.children(".itemSelected").attr("sourceid");
                                correctResponse[i].thresholdpoints = thresholdPoints;
                                isUpdateResponse = true;
                                break;
                            }
                        }

                        // Add new response if it doesn't exist.
                        if (!isUpdateResponse) {
                            var sourceId = $textInput.children(".itemSelected").attr("sourceid");

                            if (sourceId === undefined) {
                                sourceId = "";
                            }

                            correctResponse.push({
                                order: (correctResponse.length + 1).toString(),
                                destIdentifier: $textInput.attr("id"),
                                srcIdentifier: sourceId,
                                thresholdpoints: thresholdPoints
                            });
                        }
                    });

                    // Update new destination and correctResponse
                    iResult[indexPartialCredit].destination = destinationObject;

                    for (var ci = 0, lenCorrect = correctResponse.length; ci < lenCorrect; ci++) {
                        var correctResponseItem = correctResponse[ci];

                        if (correctResponseItem.srcIdentifier === 'undefined' || correctResponseItem.srcIdentifier === undefined) {
                            correctResponseItem.srcIdentifier = '';
                        }
                    }

                    if (isThresholdGrading != 0) {
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

                    iResult[indexPartialCredit].correctResponse = correctResponse;

                    var strFloat = $("#nFloat").val() + " ";
                    if ($("#nFloat").val() == "") {
                        strFloat = "";
                    }
                    buildHTMl += '&nbsp;<div class="partialDestinationObject partialAddDestinationImage" style="' + strFloat + 'width: ' + widthofImage + 'px; height: ' + heightofImage + 'px;" contenteditable="false" type="image" groupid="' + $("#resizeContentArea .resizeContent").attr("groupid") + '" percent="' + percent + '" imgOrgW="' + widthofImageCurrent + '" imgOrgH="' + heightofImageCurrent + '">';
                    buildHTMl += '<img class="cke_reset cke_widget_mask partialAddDestinationImageMark" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D">';
                    buildHTMl += '<img src="' + imgUrl + '" class="destinationImage" style="width: ' + widthofImage + 'px;height: ' + heightofImage + 'px;" texttospeech="' + tts + '"/>';
                    buildHTMl += buildHotspot;
                    buildHTMl += '</div>&nbsp;';

                    editor.insertHtml(buildHTMl);

                    isEditPartialCredit = false;

                    newResult = iResult;
                },
                onCancel: function () {
                    isEditPartialCredit = false;
                }
            };
        });
    }
});

/***
 * Load data to Draw Tool popup form
 * Return: responseIdentifier
 ***/
function loadPartialDestinationImage(element) {
    getUpDownNumber('.drawWidth', 100, 600);
    getUpDownNumber('.drawHeight', 100, 600);

    var currentPartialCreditResId = "";
    var iResultItem = '';

    for (var i = 0; i < iResult.length; i++) {
        iResultItem = iResult[i];

        if (iResultItem.type === "partialCredit") {
            currentPartialCreditResId = iResultItem.responseIdentifier;
            // Load data
            var mWidth = element.getStyle("width").replace("px", "");
            var mHeight = element.getStyle("height").replace("px", "");
            var mFloat = "float: " + element.getStyle("float") + ";";
            var percent = element.getAttribute('percent');
            var imgorgw = element.getAttribute('imgorgw');
            var imgorgh = element.getAttribute('imgorgh');
            widthofImageCurrent = parseInt(imgorgw, 10);
            heightofImageCurrent = parseInt(imgorgh, 10);

            if (element.getStyle("float") == "") {
                mFloat = "";
            }

            // Load draw width
            $('#nWidth').val(mWidth);
            // Load draw height
            $('#nHeight').val(mHeight);
            // Load float
            $('#nFloat').val(mFloat);
            // Load percent
            $('#selectPercentDestinationImage').val('');
            $('#selectPercentDestinationImage').val(parseInt(percent, 10));

            // Load draw frame content view
            $('#resizeContentArea .resizeContent').css({
                'width': mWidth + 'px',
                'height': mHeight + 'px'
            });

            // Load image to destination popup
            var srcImageObject = $(element.getOuterHtml()).find(".destinationImage");

            if (srcImageObject.attr("src") != '') {
                $('.fleft span.ckUpDownNumber').css({
                    'pointer-events': 'inherit',
                    'background': '#fff'
                });
            }

            $("#destinationImage").attr({
                "src": srcImageObject.attr("src"),
                "style": srcImageObject.attr("style")
            }).css({
                'width': mWidth + 'px',
                'height': mHeight + 'px'
            });

            $("#txtToSpeech").val(srcImageObject.attr("texttospeech"));

            // Show add hotspot button
            $("#btnAddHotSpot").css({
                "display": "block"
            });
            $('#idPercentDestinationImage').show();
            $('#partialDestinationImage .ideal-image').hide();

            // Get hotspot
            var htmlHotSpot = "";

            $(element.getOuterHtml()).find(".hotSpot").each(function (index, hotspot) {
                var srcId = "";
                var $hotspot = $(hotspot);
                var gridcell = $hotspot.attr('gridcell');
                var hotspotNumberDroppable = $hotspot.attr('numberDroppable');
                var hotspotIdentifier = $hotspot.attr('destidentifier');
                var hotspotLeft = $hotspot.css('left');
                var hotspotTop = $hotspot.css('top');
                var hotspotWidth = $hotspot.css('width');
                var hotspotHeight = $hotspot.css('height');
                var hotspotThreshold = [];
                var buildItemThreshold = '';

                hotspotNumberDroppable = hotspotNumberDroppable === undefined ? '1' : hotspotNumberDroppable;
                var notRequireAllAnswers = parseInt($hotspot.attr('notRequireAllAnswers'), 10);
                for (var k = 0; k < iResultItem.correctResponse.length; k++) {
                    if (iResultItem.correctResponse[k].destIdentifier == $hotspot.attr('destidentifier')) {
                        srcId = iResultItem.correctResponse[k].srcIdentifier;

                        if (iResultItem.correctResponse[k].thresholdpoints !== undefined) {
                            hotspotThreshold = iResultItem.correctResponse[k].thresholdpoints;
                        }
                    }
                }

                if (hotspotThreshold.length) {
                    for (var its = 0; its < hotspotThreshold.length; its++) {
                        var threshold = hotspotThreshold[its];
                        buildItemThreshold += '<span class="threshold-image"  low="' + threshold.low + '" hi="' + threshold.hi + '" pointsValue="' + threshold.pointsvalue + '"></span>';
                    }
                }

                if (gridcell !== undefined && gridcell === 'true') {
                    htmlHotSpot += '<div id="' + hotspotIdentifier + '" class="txtEnterInput hotpot ui-draggable" style="display: block; height: ' + hotspotHeight + '; top: ' + hotspotTop + '; left: ' + hotspotLeft + '; width: ' + hotspotWidth + '; position: absolute;" type="hotpot" gridcell="true" numberDroppable="' + hotspotNumberDroppable + '" notRequireAllAnswers="' + notRequireAllAnswers + '"><div class="itemSelected" sourceid="' + srcId + '">' + buildItemThreshold + hotspotIdentifier + '</div><span class="close_item">x</span></div>';
                } else {
                    htmlHotSpot += '<div id="' + hotspotIdentifier + '" class="txtEnterInput hotpot ui-draggable" style="display: block; height: ' + hotspotHeight + '; top: ' + hotspotTop + '; left: ' + hotspotLeft + '; width: ' + hotspotWidth + '; position: absolute;" type="hotpot" numberDroppable="' + hotspotNumberDroppable + '" notRequireAllAnswers="' + notRequireAllAnswers + '"><div class="itemSelected" sourceid="' + srcId + '">' + buildItemThreshold + hotspotIdentifier + '</div><span class="close_item">x</span></div>';
                }
            });

            $('#resizeContentArea .contentInputs').html(htmlHotSpot);
            dragTextInput('.resizeContent');
            deleteInput('.close_item');
        }
    }

    return currentPartialCreditResId;
}
/***
 * Reset data to Draw Tool popup form
 ***/
function resetDrawToolOnload() {
    $('#nWidth').val("400");
    $('#nHeight').val("200");
}
/***
 * get value enter input
 ***/
function getValueInput(id) {
    var value = parseInt($('#' + id).val());
    return value;
}
/***
 * resize width content area
 ***/
function resizeWidthContentArea(nWidth, oWidth, percent) {
    var width = nWidth || '100' + 'px';

    var wInput;
    if (widthofImageCurrent > 800) {
        wInput = parseInt(nWidth / 10);
    } else {
        wInput = parseInt(nWidth / 7);
    }

    $('.txtEnterInput').css('width', wInput);

    $('#resizeContentArea .divContent, #destinationImage').css({
        'width': width
    });

    //Adjust position of txtEnterInput
    $("#resizeContentArea .txtEnterInput:visible").each(function () {
        var currentLeft = parseInt($(this).css("left").replace("px", ""));
        $(this).css({
            "left": parseInt((currentLeft * percent) / 10) + 'px'
        });
    });

    dragTextInput('.divContent');
    deleteInput('.close_item');
}

function resizeWidthIncreaseContentArea(nWidth, nPercent, oPercent) {
    var width = nWidth || '100' + 'px';

    var wInput;
    if (widthofImageCurrent > 800) {
        wInput = parseInt(nWidth / 10);
    } else {
        wInput = parseInt(nWidth / 7);
    }

    $('.txtEnterInput').css('width', wInput);

    //Adjust position of txtEnterInput
    $("#resizeContentArea .txtEnterInput:visible").each(function () {
        var currentLeft = parseInt($(this).css("left").replace("px", ""));
        $(this).css({
            "left": parseInt((currentLeft * nPercent) / oPercent) + 'px'
        });
    });

    dragTextInput('.divContent');
    deleteInput('.close_item');
}
/***
 * resize height content area
 ***/
function resizeHeightContentArea(nHeight, oHeight, percent) {

    var height = nHeight || '200' + 'px';
    var hInput;

    if (heightofImageCurrent > 600) {
        hInput = parseInt(height / 10);
    } else {
        hInput = parseInt(height / 7);
    }

    $('.txtEnterInput').css('height', hInput);
    $('#resizeContentArea .divContent, #destinationImage').css({
        'height': height
    });

    //Adjust position of txtEnterInput
    $("#resizeContentArea .txtEnterInput:visible").each(function () {
        var currentTop = parseInt($(this).css("top").replace("px", ""));
        var newCurTop = parseInt((currentTop * percent) / 10);
        $(this).css({
            "top": newCurTop + 'px'
        });
    });

    dragTextInput('.divContent');
    deleteInput('.close_item');
}

function resizeHeightIncreaseContentArea(nHeight, nPercent, oPercent) {
    var height = nHeight || '200' + 'px';
    var hInput;
    if (heightofImageCurrent > 600) {
        hInput = parseInt(height / 10);
    } else {
        hInput = parseInt(height / 7);
    }

    $('.txtEnterInput').css('height', hInput);

    //Adjust position of txtEnterInput
    $("#resizeContentArea .txtEnterInput:visible").each(function () {
        var currentTop = parseInt($(this).css("top").replace("px", ""));
        $(this).css({
            "top": parseInt((currentTop * nPercent) / oPercent) + 'px'
        });

    });

    dragTextInput('.divContent');
    deleteInput('.close_item');
}
/*
 * remove text input
 */
var currentPartialCreditId = '';
var isEditPCredit = false;

function deleteInput(item) {
    $(item).unbind("click").on("click", function () {
        //remove item text input
        if (item == ".close_item") {
            $(this).parents(".txtEnterInput").remove(); //Remove current destination when user click on remove button

            //Remove destination
            var isHasRemoved = false;
            for (var i = 0; i < iResult.length; i++) {
                if (iResult[i].type == "partialCredit") {
                    for (var m = 0; m < iResult[i].destination.length; m++) {
                        if (iResult[i].destination[m].destIdentifier == $(this).parents(".txtEnterInput").attr("id")) {
                            iResult[i].destination.splice(m, 1);
                            isHasRemoved = true;
                            break;
                        }

                    }
                    if (isHasRemoved) {
                        break;
                    }
                }
            }

        }

        isShowClearBtnHotspot();
    });
}
/*
 * edit text
 */
function editText(item) {
    $(item).unbind("click").on('click', function (e) {
        if ($(this).find('img').hasClass('img')) {
            return;
        }

        //Prevent if user click textarea
        if ($(e.target).is(".edittext")) {
            return;
        }

        if ($(this).find('input').length) {
            $(this).text($(this).find('input').val());
        }

        var val = $(this).text();
        $(this).html('<input class="edittext" value="' + val + '" oldValue="' + val + '"></input>');
        $('.edittext').keydown(function (event) {

            var keycode = (event.keyCode ? event.keyCode : event.which);
            if (keycode == '13') {
                var valInput = $(this).val().trim();

                $(this).remove();
                if (valInput == "") {
                    valInput = $(this).attr("oldValue");
                }

                $(item).text(valInput);
                //Update value for destination item
                $("#resizeContentArea .itemSelected span[id=" + $(item).parents(".itemWord").attr("id") + "]").html(valInput);
                $('.cke_dialog_ui_button_ok').css('pointer-events', 'all');
            }

        });
        $('.edittext').trigger('focus');
    });

    $(item).on('focus', '.edittext', function (e) {
        $(e.target).val();
        $('.cke_dialog_ui_button_ok').css('pointer-events', 'none');
        $(item).off('focus');
    });

    $(item).on('blur', '.edittext', function () {
        var valInput = $(this).val().trim();

        $(this).remove();
        if (valInput == "") {
            valInput = $(this).attr("oldValue");
        }

        $(item).text(valInput);
        //Update value for destination item
        $("#resizeContentArea .itemSelected span[id=" + $(item).parents(".itemWord").attr("id") + "]").html(valInput);
        $('.cke_dialog_ui_button_ok').css('pointer-events', 'all');

    });
    return false;
}

/*
 *  add text field input
 */
function addTextInput(appendTo, closeInput, type, rows, cols, ri, ci) {
    var isGrid = rows > 0 && cols > 0;
    var $txtEnterInput = $('#txtEnterInput');
    var $destinationImage = $('#destinationImage');
    var $gridArea = $('#grid-area');

    $txtEnterInput.css("display", "block");

    var itemClone = $txtEnterInput.clone();

    var currentWidth = $destinationImage.width();
    var currentheight = $destinationImage.height();

    var indexPartial = 0;
    var destinationPartial = [];
    for (var i = 0; i < iResult.length; i++) {
        if (iResult[i].type == "partialCredit") {
            destinationPartial = iResult[i].destination;
            indexPartial = i;
        }
    };

    var desId = createDestinationIdentifier(destinationPartial);

    currentWidth = currentWidth - 40;
    currentheight = currentheight - 40;

    var rContent = $('.resizeContent').find('.txtEnterInput');

    for (var m = 0; m < rContent.length; m++) {
        var resId = m + 1;
        var identifier = $(rContent[m]).attr('id');

        if (identifier != resId) {
            var isOnlyOne = true;
            for (var k = 0; k < rContent.length; k++) {
                var newId = $(rContent[k]).attr('id');
                if (resId == newId) {
                    isOnlyOne = false;
                }
            }
            if (isOnlyOne) {
                IdGuide = resId;
                break;
            }

        }
    }

    if (type == 'hotpot') {
        itemClone.addClass('hotpot');
    }

    var top,
        left,
        width,
        height;

    var nWidth = isGrid ? $gridArea.width() : $destinationImage.width();
    var nHeight = isGrid ? $gridArea.height() : $destinationImage.height();
    var imageOrgWidth = widthofImageCurrent;
    var imageOrgHeight = heightofImageCurrent;

    var percentDes = $('#destinationImage').attr('percent');

    if (nWidth > 800 || nHeight > 800) {
        width = isGrid ? (nWidth / cols) : parseInt(nWidth / 10);
        height = isGrid ? (nHeight / rows) : parseInt(nHeight / 10);
    } else {
        if (400 <= nWidth <= 800 || 400 <= nHeight <= 800) {
            width = isGrid ? (nWidth / cols) : parseInt(nWidth / 7);
            height = isGrid ? (nHeight / rows) : parseInt(nHeight / 7);
        } else {
            width = 40;
            height = 40;
        }
    }

    var wImage = $('#destinationImage').css('width').replace('px', '');
    var hImage = $('#destinationImage').css('height').replace('px', '');

    var gridTop = $gridArea.offset().top - $destinationImage.offset().top;
    var gridLeft = $gridArea.offset().left - $destinationImage.offset().left;

    top = isGrid ? ((ri - 1) * height) + gridTop : (hImage - height) / 2;
    left = isGrid ? ((ci - 1) * width) + gridLeft : (wImage - width) / 2;

    itemClone
        .attr("id", desId)
        .attr("type", type)
        .appendTo("div" + appendTo)
        .css({
            'top': top + 'px',
            'left': left + 'px',
            'width': width + 'px',
            'height': height + 'px'
        })
        .find(".itemSelected")
        .html(desId);

    if (isGrid) {
        itemClone.attr('gridcell', 'true');
    }

    dragTextInput(appendTo);
    deleteInput(closeInput);

    $txtEnterInput.css("display", "none");

    //add destination to iResult
    iResult[indexPartial].destination.push({
        destIdentifier: desId,
        left: left,
        top: top,
        width: width,
        height: height,
        desheight: nHeight,
        deswidth: nWidth,
        type: "image",
        group: "des1",
        percent: percentDes,
        imgOrgW: imageOrgWidth,
        imgOrgH: imageOrgHeight
    });

    isShowClearBtnHotspot();

    return false;
}
/*
 * add item List into list words
 */
function addItemList(ul, className) {
    $('#demoTempList').css("display", "block");
    var valItem = $('#applylistWords').find('textarea').val().trim();
    var itemList = $('#demoTempList').find('li').clone();

    itemList.find('.itemWord').attr('id', alphabet[$(ul).find("li").length]);
    itemList.find('.textArea').addClass('valItem').html('<span>' + valItem + '</span>');
    itemList.find('.image_upload').remove();
    itemList.find('.bnt_add').remove();

    itemList.appendTo(ul);
    deleteInput(className);
    editText(itemList.find('.textArea span'));

    $('#applylistWords').find('textarea').val('');

    $('#demoTempList').css("display", "none");
    return false;
}
/*
 *  add Upload Image into list words
 */
function readURL(ul, input, className) {

    $('#demoTempList').css("display", "block");
    var itemList = $('#demoTempList').find('li').clone();

    var readURLLength = $(ul).find("li").length;
    alphabet = [];

    if (CKEDITOR.instances[ckID].config.alphaBeta != undefined) {
        alphabet = CKEDITOR.instances[ckID].config.alphaBeta;
    }
    //var idItem = createId();

    itemList.find('.itemWord').attr('id', alphabet[readURLLength]).addClass('divImage');
    itemList.find('.textArea').find('textarea').remove();
    itemList.find('.image_upload').remove();
    itemList.find('.bnt_add').remove();
    itemList.find('#imgUpload')
        .addClass('img')
        .attr('src', input);

    itemList.appendTo(ul);
    deleteInput(className);

    $('#demoTempList').css("display", "none");
    return false;
}
/*
 * drag text input
 */
function dragTextInput(boxContent) {
    $(boxContent).find('div').each(function () {
        var idTxt = $(this).attr('id');
        $('#' + idTxt).draggable("destroy").draggable({
            cursor: "move",
            containment: ".resizeContent"
        }).css('position', 'absolute');

        $('#' + idTxt).resizable("destroy").resizable({
            minHeight: 30,
            minWidth: 30,
            containment: "parent"
        });
    });

}
/*
 *  reset data when press cancel or close
 */

function resetNewData() {
    $('#nHeight').val('200');
    $('#nWidth').val('400');
    $('#nFloat').val("");
    $("#txtToSpeech").val("");
    $('#resizeContentArea .resizeContent .txtEnterInput').remove();
    $('#resizeContentArea .resizeContent, #resizeContentArea .resizeContent img').css({
        'width': '400px',
        'height': '200px'
    }).removeAttr("groupid");
    $('#destinationImage').attr('src', '');
    $("#btnAddHotSpot").hide();
    document.getElementById("formPCImageUpload_destination").reset();

    getUpDownNumber('.drawWidth', 100, 600);
    getUpDownNumber('.drawHeight', 100, 600);

    $('.fleft span.ckUpDownNumber').css({
        'pointer-events': 'none',
        'background': '#eee'
    });

    $('#idPercentDestinationImage').hide();
    $('#selectPercentDestinationImage').val(10);
    $('#destinationImage').attr('percent', '').hide();
    $('#partialDestinationImage .ideal-image').show();
}

/**
 * Get list item from source object
 * @param  {[type]} pop     [description]
 * @param  {[type]} overlay [description]
 * @param  {[type]} idItem  [description]
 * @return {[type]}         [description]
 */
function getListItem(pop, overlay, idItem) {
    var $pop = $(pop);
    var $overlay = $(overlay);
    var $outer = $('<div>');
    var $correctAnswer = $('<ul class="correctAnswer" />');
    var $popupList = $('#popupList');
    var $overlayList = $('#overlayList');
    var $itemDes = $('.resizeContent #' + idItem);
    var correctAnswerArr = [];
    var popupListHtml = '';
    var lenIResult = iResult.length;
    var isThresholdGrading = iResult[0].responseDeclaration.thresholdGrading;
    var isRelativeGrading = iResult[0].responseDeclaration.relativeGrading;
    var isAlgorithmicGrading = iResult[0].responseDeclaration.algorithmicGrading;
    var isAbsoluteGrading = iResult[0].responseDeclaration.absoluteGrading === '1' || (isThresholdGrading==='0' && isAlgorithmicGrading==='0' && isRelativeGrading==='0');
    var thresholdPoints = [];
    var msgNotification = '';

    if (lenIResult === 0) {
        return;
    }

    for (var k = 0; k < lenIResult; k++) {
        for (var i = 0; i < iResult[k].source.length; i++) {
            correctAnswerArr.push('<li><label for="' + iResult[0].source[i].srcIdentifier + '"><input type="checkbox" name="selectAnswer" id="' + iResult[0].source[i].srcIdentifier + '" class="form-checkbox"/>' + iResult[0].source[i].value + '</label></li>');
        }
    }

    $correctAnswer.append(correctAnswerArr.join('')).appendTo($outer);

    if (isThresholdGrading && $itemDes.find('.threshold-image').length) {
        thresholdPoints = getPointItemThreshold($itemDes.find('.threshold-image'));
    }

    popupListHtml += '<div class="poplistItem"><div class="cke_dialog_title">Select Answer</div>';
    popupListHtml += '<a type="image" title="Remove" class="ckImageButton ckRemove cke_dialog_close_button" id="closeListItem"><span class="cke_label">X</span></a>';
    popupListHtml += '<div class="lists">';
    popupListHtml += '<div>Maximum number of draggable objects for this destination is: <input type="text" class="txtFullcreate" id="sourceImgDestinationNumberDroppable" value="1"/></div>';
    popupListHtml += '<button class="cke_dialog_ui_button js-btn-add-threshold"><span class="cke_dialog_ui_button">Add Threshold</span></button>';
    popupListHtml += '<div class="thresholds thresholds-destination-image">';
    popupListHtml += '<h3>List threshold</h3>';
    popupListHtml += '<table class="table-threshold">';
    popupListHtml += '<thead><tr><th>Low</th><th>High</th><th>Points</th>';
    popupListHtml += '<th></th>';
    popupListHtml += '</tr></thead>';
    popupListHtml += '<tbody></tbody>';
    popupListHtml += '</table>';
    popupListHtml += '</div>';
    popupListHtml += '<div class="fieldsetTextField">';
    popupListHtml += '<legend>Select Correct Answer</legend>';
    popupListHtml += '<div class="destinationAnswerList">' + $outer.html() + '</div>';
    popupListHtml += '</div>';
    popupListHtml += '</div>';
    popupListHtml += '<div class="cke_dialog_footer">';
    popupListHtml += '   <div class="cke_dialog_ui_hbox">';
    popupListHtml += '       <div class="cke_dialog_ui_hbox_first" role="presentation"><a title="OK" id="selectedAnswer" class="cke_dialog_ui_button cke_dialog_ui_button_ok" role="button"><span class="cke_dialog_ui_button">OK</span></a></div>';
    popupListHtml += '       <div class="cke_dialog_ui_hbox_last" role="presentation"><a title="Cancel" id="closeListCancel" class="cke_dialog_ui_button cke_dialog_ui_button_cancel" role="button"><span class="cke_dialog_ui_button">Cancel</span></a></div>';
    popupListHtml += '   </div>';
    popupListHtml += '</div></div>';

    var popupListHtmlV2 = '';

    popupListHtmlV2 += '<div class="poplistItem cke_dialog_body" style="width: auto"><div class="cke_dialog_title">Select Answer</div>';
    popupListHtmlV2 += '<span class="btn-close-area"><a type="image" title="Remove" class="ckImageButton cke_dialog_close_button " id="closeListItem"><span class="cke_label">X</span></a></span>';
    popupListHtmlV2 += '<div class="lists">';
    popupListHtmlV2 += '<div>Maximum number of draggable objects for this destination is: <input type="text" class="txtFullcreate" id="sourceImgDestinationNumberDroppable" value="1"/></div>';
    popupListHtmlV2 += '<button class="cke_dialog_ui_button js-btn-add-threshold"><span class="cke_dialog_ui_button">Add Threshold</span></button>';
    popupListHtmlV2 += '<div class="thresholds thresholds-destination-image">';
    popupListHtmlV2 += '<h3>List threshold</h3>';
    popupListHtmlV2 += '<table class="table-threshold">';
    popupListHtmlV2 += '<thead><tr><th>Low</th><th>High</th><th>Points</th>';
    popupListHtmlV2 += '<th>Delete</th>';
    popupListHtmlV2 += '</tr></thead>';
    popupListHtmlV2 += '<tbody></tbody>';
    popupListHtmlV2 += '</table>';
    popupListHtmlV2 += '</div>';
    popupListHtmlV2 += '<div class="d-flex flex-row justify-content-between align-items-baseline">';
    popupListHtmlV2 += '<h3 class="mt-4" style="">Select Correct Answer</h3>';
    popupListHtmlV2 += '<label for="require_all_answers" class="form-label m-0"><input type="checkbox" name="require_all_answers" id="require_all_answers" class="form-checkbox" checked> Require all answer(s)</label>';
    popupListHtmlV2 += '</div>';
    popupListHtmlV2 += '<div class="mt-1 fieldsetTextField">';
    popupListHtmlV2 += '<div class="destinationAnswerList" style="width: 100%">' + $outer.html() + '</div>';
    popupListHtmlV2 += '</div>';
    popupListHtmlV2 += '</div>';
    popupListHtmlV2 += '<div class="cke_dialog_footer">';
    popupListHtmlV2 += '   <div class="cke_dialog_ui_hbox d-flex justify-content-around align-items-center">';
    popupListHtmlV2 += '       <div class="cke_dialog_ui_hbox_last" role="presentation"><a title="Cancel" id="closeListCancel" class="cke_dialog_ui_button cke_dialog_ui_button_cancel" role="button"><span class="cke_dialog_ui_button">Cancel</span></a></div>';
    popupListHtmlV2 += '       <div class="cke_dialog_ui_hbox_first" role="presentation"><a title="OK" id="selectedAnswer" class="cke_dialog_ui_button cke_dialog_ui_button_ok" role="button"><span class="cke_dialog_ui_button">OK</span></a></div>';
    popupListHtmlV2 += '   </div>';
    popupListHtmlV2 += '</div></div>';

    $pop.html(IS_V2 ? popupListHtmlV2:  popupListHtml);
    $pop.find('.group_button').remove();
    $pop.show();
    $pop.draggable({
            cursor: "move",
            handle: ".cke_dialog_title"
        })
        .css({
            'position': 'absolute',
            'z-index': '99999',
            'left': IS_V2 ? '50%' : '21%',
            'top': IS_V2 ? '15%' : '30%',
            'transform': IS_V2 ? 'translate(-50%, 0)' : 'none'
        });

    if (IS_V2) {
      $pop.css({ 'width': 'auto' });
      $pop.addClass('cke_editor_dialog_V2');
      $('#overlayList').css('height', 'calc(100% + 70px)').css('background', '#00000080');
    }

    $overlay.show();

    supportTabThreshold($pop);
    getUpDownNumber('#sourceImgDestinationNumberDroppable', 1, 100);

    // Check selected answer of destination item
    var srcId = $itemDes.find('.itemSelected').attr('sourceid');

    if (srcId != undefined) {
        srcId = srcId.split(';');

        if (srcId[0] != '') {
            for (var si = 0, lenSrcId = srcId.length; si < lenSrcId; si++) {
                $pop.find('.correctAnswer input[id="' + srcId[si] + '"]').prop('checked', true);
            }
        }
    }

    // Check number of droppable
    if ($itemDes.attr('numberDroppable') == undefined) {
        $('#sourceImgDestinationNumberDroppable').val(1);
    } else {
        $('#sourceImgDestinationNumberDroppable').val($itemDes.attr('numberDroppable'));
    }

    // Check drag drop is Absolute Grading
    if (isThresholdGrading === '1') {
        $pop.find('.js-btn-add-threshold').removeClass('visuallyhidden');
        $pop.find('.thresholds').removeClass('visuallyhidden');

        // Load item threshold
        if (thresholdPoints.length) {
            fillItemThreshold($pop.find('.table-threshold tbody'), thresholdPoints);
        }

        if (IS_V2) {
          $pop.find('.js-btn-add-threshold').removeClass('d-none');
        }

    } else {
        $pop.find('.js-btn-add-threshold').addClass('visuallyhidden');
        $pop.find('.thresholds').addClass('visuallyhidden');
        if (IS_V2) {
          $pop.find('.js-btn-add-threshold').addClass('d-none');
        }
    }

    /**
     * Click event of close and cancel popup list
     */
    $('#popupList #closeListItem, #popupList #closeListCancel').unbind('click').on('click', function () {
        $popupList.hide();
        $overlayList.hide();
        $('.destination-image-overlay').remove();
        $('#overlayList').css('height', '100%')
    });

    /**
     * Click selected answer for destination item of image
     */
    $pop.unbind('click').on('click', '#selectedAnswer', function () {
        var srcId = '';
        var answerCheckedCount = 0;
        var thresholdPointsTemp = [];
        var thresholdPoints = [];
        var desNumberDroppable = parseInt($('#sourceImgDestinationNumberDroppable').val(), 10);
        var notRequireAllAnswers = $pop.find('#require_all_answers').prop('checked') ? '0' : '1';
        // Check selected answer
        $pop.find('.correctAnswer input[type=checkbox]').each(function () {
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

            $pop.find('.thresholds tbody tr').each(function () {
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
                    msgNotification = 'The value must be greater than 0.';
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

            if (thresholdPoints !== undefined && thresholdPoints.length) {
                var buildItemThreshold = '';
                for (var its = 0; its < thresholdPoints.length; its++) {
                    var threshold = thresholdPoints[its];
                    buildItemThreshold += '<span class="threshold-image"  low="' + threshold.low + '" hi="' + threshold.hi + '" pointsValue="' + threshold.pointsvalue + '"></span>';
                }

                $itemDes.find('.threshold-image').remove();
                $(buildItemThreshold).insertAfter($itemDes.find('.itemSelected'));
            } else {
                $itemDes.find('.threshold-image').remove();
            }
        } else {
            if (answerCheckedCount > desNumberDroppable && notRequireAllAnswers === '0') {
                msgNotification = 'Please increase the maximum number of hot spots a student can select.';
                customAlert(msgNotification);
                return false;
            }
        }

        var correctAnswerAvaiable = $pop.find('.correctAnswer input[type=checkbox]').length;
        if (correctAnswerAvaiable < desNumberDroppable) {
            msgNotification = 'Maximum hot spots that can be selected cannot be greater than the total number of hot spots in the item.';
            customAlert(msgNotification);
            return false;
        }

        $itemDes.find('.itemSelected').attr('sourceid', srcId.slice(0, -1));
        $itemDes.attr('numberDroppable', $('#sourceImgDestinationNumberDroppable').val());
        $itemDes.attr('notRequireAllAnswers',  notRequireAllAnswers);
        $popupList.hide();
        $overlayList.hide();
        $('.destination-image-overlay').remove();
        $('#overlayList').css('height', '100%')
    });

    $pop.on('click', '.js-btn-add-threshold', function () {
        addItemThreshold($pop.find('.table-threshold tbody'));
    });

    var isAlgorithmicGrading = iResult[0].responseDeclaration.algorithmicGrading;

    if (isAlgorithmicGrading === '1') {
        $pop.find('.correctAnswer input[type="checkbox"]').prop('disabled', true);
    } else {
        $pop.find('.correctAnswer input[type="checkbox"]').prop('disabled', false);
    }

    if (isAbsoluteGrading) {
      var notRequireAllAnswers = parseInt($itemDes.attr('notRequireAllAnswers'), 10);
      if(notRequireAllAnswers === 1){
        $pop.find('#require_all_answers').prop('checked', false);
      } else {
        $pop.find('#require_all_answers').prop('checked', true);
      }
      $pop.find('#require_all_answers').parent().show();
    } else {
      $pop.find('#require_all_answers').prop('checked', true);
      $pop.find('#require_all_answers').parent().hide();
    }
}

var iframe = document.createElement("iframe");
var iframeId = "",
    widthofImageCurrent, heightofImageCurrent, oPercent, counterInc, counterDec; // widthofImageOriginal, heightofImageOriginal, tempWImage, tempHImage;
function partialCreditImageUpload(form, destinationPercent, action_url, currentElement) {
    if (IS_V2) {
        ShowBlock($('body'), 'Uploading');
        setTimeout(function () {
          $(".blockUI.blockOverlay").css({ 'z-index': 11010, opacity: 0.3 })
          $(".blockUI.blockMsg.blockElement").css('z-index', '11011')
        }, 100)

      } else {
        $("body").ckOverlay();
      }

    // Create the iframe...
    iframe = document.createElement("iframe");
    iframe.setAttribute("id", "upload_iframe");
    iframe.setAttribute("name", "upload_iframe");
    iframe.setAttribute("width", "0");
    iframe.setAttribute("height", "0");
    iframe.setAttribute("border", "0");
    iframe.setAttribute("style", "width: 0; height: 0; border: none;");

    // Add to document...
    form.parentNode.appendChild(iframe);
    //window.frames['upload_iframe'].name = "upload_iframe";

    iframeId = document.getElementById("upload_iframe");

    // Add event...
    var eventHandler = function () {

        if (iframeId.detachEvent) iframeId.detachEvent("onload", eventHandler);
        else iframeId.removeEventListener("load", eventHandler, false);

        var content = '';

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

        //Create Group ID
        var myArray = [];

        $("<div></div>").append(CKEDITOR.instances[ckID].getData()).find(".partialDestinationObject[groupid]").each(function () {
            myArray.push($(this).attr("groupid"));
        });

        var groupId = "group" + getMaxNumber(myArray);


        if ($("#resizeContentArea .resizeContent").attr("groupId") == undefined || $("#resizeContentArea .resizeContent").attr("groupId") == "") {
            $("#resizeContentArea .resizeContent").attr({
                "groupId": groupId
            });
        }

        //This is upload for destination
        var percentFirst = destinationPercent.val();
        var widthDemo = $('#ImageDemo').width();
        var heightDemo = $('#ImageDemo').height();

        //$("#destinationImage").attr({ "src": GetViewReferenceImg + data.url }).css({ "width": widthDemo + "px", "height": heightDemo + "px" }).show();
        $("#destinationImage").attr({
            "src": data.absoluteUrl
        }).css({
            "width": widthDemo + "px",
            "height": heightDemo + "px"
        }).show();
        $('#resizeContentArea .resizeContent').css({
            "width": widthDemo + "px",
            "height": heightDemo + "px"
        });

        widthofImageCurrent = widthDemo;
        heightofImageCurrent = heightDemo;

        $("#destinationImage").show();

        $("#btnAddHotSpot").css({
            "display": "block"
        });
        $('#partialDestinationImage .ideal-image').hide();
        document.getElementById("formPCImageUpload_destination").reset(); //Reset upload

        $('#idPercentDestinationImage').show();
        $('#selectPercentDestinationImage').val(10);

        //hide overlay
        if (IS_V2) {
          $('body').unblock();
        } else {
          $("body").ckOverlay.destroy();
        }
    };

    if (iframeId.addEventListener) iframeId.addEventListener("load", eventHandler, true);
    if (iframeId.attachEvent) iframeId.attachEvent("onload", eventHandler);

    // Set properties of form...
    form.setAttribute("target", "upload_iframe");
    form.setAttribute("action", action_url);
    form.setAttribute("method", "post");
    form.setAttribute("enctype", "multipart/form-data");
    form.setAttribute("encoding", "multipart/form-data");

    // Submit the form...
    form.submit();
}

function removeIFrame() {
    //Check iFrame and only remove iframe has existed
    if (iframeId != null && iframeId.parentNode != null) {
        iframeId.parentNode.removeChild(iframeId);
    }
}

function showImageDemo(input) {
    if (input.files && input.files[0]) {
        var reader = new FileReader();
        reader.onload = function (e) {
            $('#ImageDemo').attr('src', e.target.result);
        };
        reader.readAsDataURL(input.files[0]);
    }
}

function showHotspotDialog() {
    var $popupHotspots = $('#popupHotspots');
    var $overlayList = $('#overlayList');
    var html = '';

    html += '<div class="cke_dialog_body cke_dialog_hotspots"><div class="cke_dialog_title">Create Drag-To Destinations</div>';
    html += '<a type="image" title="Remove" class="cke_dialog_close_button" id="closeHotspotPopup"><span class="cke_label">X</span></a>';
    html += '<div class="lists">';
    html += '<div class="lists-item"><label class="widthLabel" for="singleHotspot"><input type="radio" id="singleHotspot" name="hotspotType" checked="checked"/> Manual Destination(s)</label></div>';
    html += '<div class="lists-item" id="gridSetting"><label class="widthLabel" for="gridHotspot"><input type="radio" id="gridHotspot" name="hotspotType"/> Grid-Style Destinations</label></div>';
    html += '<div id="gridAreaSetting" style="display: none">';
    html += '<div class="lists-item lists-item-modified"><span class="widthLabel">Define size of grid area:</span></div>';
    html += '<div class="lists-item lists-item-modified"><span class="widthLabel">Width:</span><input type="text" value="50" name="txtGridWidth" id="txtGridWidth" class="txtFullcreate"/></div>';
    html += '<div class="lists-item lists-item-modified"><span class="widthLabel">Height:</span><input type="text" value="50" name="txtGridHeight" id="txtGridHeight" class="txtFullcreate"/></div>';
    html += '</div>';
    html += '<div id="hotspotSetting" style="display: none">';
    html += '<div class="lists-item lists-item-modified"><span class="widthLabel">Grid Rows:</span><input type="text" value="2" name="txtHotspotRows" id="txtHotspotRows" class="txtFullcreate"/></div>';
    html += '<div class="lists-item lists-item-modified"><span class="widthLabel">Grid Cols:</span><input type="text" value="2" name="txtHotspotCols" id="txtHotspotCols" class="txtFullcreate"/></div>';
    html += '</div>';
    html += '</div>';
    html += '<div class="cke_dialog_footer">';
    html += '<div class="cke_dialog_ui_hbox cke_dialog_footer_buttons">';
    html += '<div class="cke_dialog_ui_hbox_first" role="presentation"><a title="OK" id="selectedHotspotPopup" class="cke_dialog_ui_button cke_dialog_ui_button_ok" role="button" type="hotpot"><span class="cke_dialog_ui_button">OK</span></a></div>';
    html += '<div class="cke_dialog_ui_hbox_last" role="presentation"><a title="Cancel" id="closeHotspotPopupCancel" class="cke_dialog_ui_button cke_dialog_ui_button_cancel" role="button"><span class="cke_dialog_ui_button">Cancel</span></a></div>';
    html += '</div>';
    html += '</div>';
    html += '</div>';

    $overlayList.show();
    $popupHotspots
        .append(html)
        .show()
        .draggable({
            cursor: 'move',
            handle: '.cke_dialog_title'
        })
        .css({
            'position': 'absolute',
            'z-index': '99999',
            'left': '21%',
            'top': '20%'
        });

    var $hotspotSetting = $popupHotspots.find('#hotspotSetting');
    var $gridAreaSetting = $popupHotspots.find('#gridAreaSetting');
    var $hotSpotType = $popupHotspots.find('input[type="radio"][name="hotspotType"]');
    var $txtHotspotRows = $('#txtHotspotRows');
    var $txtHotspotCols = $('#txtHotspotCols');
    var $txtGridWidth = $('#txtGridWidth');
    var $txtGridHeight = $('#txtGridHeight');
    var $gridArea = $('#grid-area');
    var $destinationImage = $('#destinationImage');

    getUpDownNumber($txtHotspotRows, 1, 100);
    getUpDownNumber($txtHotspotCols, 1, 100);
    getUpDownNumber($txtGridWidth, 50, 1000);
    getUpDownNumber($txtGridHeight, 50, 1000);

    $hotSpotType.on('change', function () {
        var $self = $(this);
        var idAttr = $self.attr('id');

        if ($self.is(':checked') && idAttr === 'gridHotspot') {
            if ($gridArea.css('display') === 'none') {
                $gridAreaSetting.show();
            } else {
                $hotspotSetting.show();
            }
        } else {
            $hotspotSetting.hide();
            $gridAreaSetting.hide();
        }
    });

    $popupHotspots.unbind('click').on('click', '#closeHotspotPopup, #closeHotspotPopupCancel', function () {
        $popupHotspots.html('').hide();
        $overlayList.hide();
    });

    $popupHotspots.on('click', '#selectedHotspotPopup', function () {
        var $self = $(this);
        var type = $self.attr('type');
        var isGridHotspot = false;

        $hotSpotType.each(function (index, spType) {
            var $spType = $(spType);
            var spTypeId = $spType.attr('id');

            if ($spType.is(':checked') && spTypeId === 'gridHotspot') {
                isGridHotspot = true;
            }
        });

        if (isGridHotspot) {
            if ($gridArea.css('display') === 'none') {
                var width = $txtGridWidth.val();
                var height = $txtGridHeight.val();

                if (width > $destinationImage.width()) {
                    alert("Width of grid area doesn't larger than width of destination image");
                    return;
                }

                if (height > $destinationImage.height()) {
                    alert("Height of grid area doesn't larger than height of destination image");
                    return;
                }

                var wImage = $('#destinationImage').css('width').replace('px', '');
                var hImage = $('#destinationImage').css('height').replace('px', '');
                var top = (hImage - height) / 2;
                var left = (wImage - width) / 2;

                $gridArea
                    .show()
                    .css({
                        'top': top + 'px',
                        'left': left + 'px',
                        'width': width + 'px',
                        'height': height + 'px'
                    });
            } else {
                var rows = $txtHotspotRows.val();
                var cols = $txtHotspotCols.val();

                for (var ri = 1; ri <= rows; ri++) {
                    for (var ci = 1; ci <= cols; ci++) {
                        addTextInput('.resizeContent .contentInputs', '.close_item', type, rows, cols, ri, ci);
                    }
                }
            }
        } else {
            addTextInput('.resizeContent .contentInputs', '.close_item', type, 0, 0, 0, 0);
        }

        $popupHotspots.html('').hide();
        $overlayList.hide();
    });

    isShowClearBtnHotspot();
}

/**
 * Show or Hide Clear Hotspot Button
 */
function isShowClearBtnHotspot() {
    var $btnClearHotSpot = $('#btnClearHotSpot');

    if ($('#resizeContentArea .contentInputs .txtEnterInput').length > 0) {
        $btnClearHotSpot.show();
    } else {
        $btnClearHotSpot.hide();
    }
}

/**
 * Refresh Grid Area
 */
function refreshGridArea() {
    var $gridArea = $('#grid-area');

    $gridArea
        .hide()
        .css({
            'top': '0',
            'left': '0'
        });
}

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
 * Accept numeric when keydown
 * @param  {[type]} element [description]
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
 * Get change dimension with or height
 * @param  {[type]} source   [description]
 * @param  {[type]} target    [description]
 * @param  {[type]} dimension [description]
 * @return {[type]}           [description]
 */
function getChangeDimensions(source, target, dimension) {
    var $source = $(source);

    $source.on('change.' + dimension, function () {
        var $self = $(this);
        var identifierEl = $self.attr('id');
        var valueInputEl = getValueInput(identifierEl);
        var minNumberEl = parseInt($self.attr('minnumber'), 10);
        var maxNumberEl = parseInt($self.attr('maxnumber'), 10);
        var oldValue = 0;

        if (dimension === 'width') {
            oldValue = $(target).width();
        }

        if (dimension === 'height') {
            oldValue = $(target).height();
        }

        if (maxNumberEl < valueInputEl) {
            valueInputEl = maxNumberEl;
        } else if (minNumberEl > valueInputEl) {
            valueInputEl = minNumberEl;
        }

        resizeWidthContentArea(valueInputEl, oldValue);
    });
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
}

/**
 * Get point item threshold
 * @param  {[type]} thresholds [description]
 * @return {[type]}            [description]
 */
function getPointItemThreshold(thresholds) {
    var $thresholds = $(thresholds);
    var thresholdPoints = [];

    $thresholds.each(function () {
        var $threshold = $(this);

        thresholdPoints.push({
            low: $threshold.attr('low'),
            hi: $threshold.attr('hi'),
            pointsvalue: $threshold.attr('pointsValue')
        });
    });

    return thresholdPoints;
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
