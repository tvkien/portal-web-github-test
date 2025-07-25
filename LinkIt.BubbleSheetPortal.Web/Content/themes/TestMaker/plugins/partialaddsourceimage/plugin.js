CKEDITOR.plugins.add('partialaddsourceimage', {
    lang: 'en', // %REMOVE_LINE_CORE%
    icons: 'partialaddsourceimage',
    requires: 'dialog',
    hidpi: true, // %REMOVE_LINE_CORE%
    init: function (editor) {
        var pluginName = 'insertPartialAddSourceImage';

        editor.addCommand(pluginName, new CKEDITOR.dialogCommand(pluginName));

        editor.ui.addButton('PartialAddSourceImage', {
		    label: 'Source Image',
		    command: pluginName,
		    icon: this.path + 'icons/image.png',
		    toolbar: 'insertPartialAddSourceImage,30'
		});

        editor.widgets.add('partialaddsourceimage', {
            inline: true,
            mask: true
        });

        var sourcePartial = [];// This is store all source of partial credit
        window.editImage = false;
        var eleImage = "";//This use for firstime load popup
        var eleAddSourceText = "";

        editor.on('doubleclick', function (evt) {
            var ele = evt.data.element;

            if (ele.hasClass('partialAddSourceImage')) {
                evt.data.dialog = pluginName;

                eleImage = ele;
                eleAddSourceText = ele;
                //The status to editor know this is update
                window.editImage = true;

                //Get source from iResult to partialSource
                for (var i = 0; i < iResult.length; i++) {
                    if (iResult[i].type == "partialCredit") {
                        sourcePartial = iResult[i].source;
                    }
                }

                dblickHandlerToolbar(editor);
            }
        });

        CKEDITOR.dialog.add(pluginName, function (editor) {
            var checkChanged = false,
                checkDrawable = false,
                formUpload = null,
                formUploadButton = null,
                formImageUploadFile = null,
                selectPercent = null,
                selectAlignment = null,
                previewImage = null,
                tts = null;

            myhtml = '\
                    <style>\
                        #previewImage{ max-width:570px; max-height:200px;}\
                    </style>\
                    <div id="wrap_' + editor.name + '_partialAddSourceImage_dialog" style="min-width: 550px;">\
                        <div class="drag-drop-section">\
                            <div class="item-dd">\
                                <div class="title-dd">Upload Image:</div>\
                                <div class="upload-dd">\
                                    <form name="form-upload" class="formPartialAddSourceImage" id="formPartialAddSourceImage" lang="en" action="uploader.php?type=images" dir="ltr" method="POST" enctype="multipart/form-data">\
                                        <input type="file" size="38" name="file" aria-labelledby="cke_262_label" class="formPartialAddSourceImageFile" style="height: 21px;border: solid 1px #cccccc;" />\
                                        <input type="hidden" name="id" id="objectId" />\
                                    </form>\
                                </div>\
                                <div class="upload-button-section">\
                                    <a class="uploadImageButton cke_dialog_ui_fileButton cke_dialog_ui_button" role="button" hidefocus="true" title="Upload" href="javascript:void(0)" >\
                                        <span class="cke_dialog_ui_button">Upload</span>\
                                    </a>\
                                </div>\
                                <div class="clear"></div>\
                            </div>\
                            <div class="item-dd">\
                                <div class="perecent-dd">\
                                    Percent: \
                                    <select aria-labelledby="cke_272_label" class="cke_dialog_ui_input_select selectPercent">\
                                        <option value="1">10%</option>\
                                        <option value="2">20%</option>\
                                        <option value="3">30%</option>\
                                        <option value="4">40%</option>\
                                        <option value="5">50%</option>\
                                        <option value="6">60%</option>\
                                        <option value="7">70%</option>\
                                        <option value="8">80%</option>\
                                        <option value="9">90%</option>\
                                        <option selected value="10">100%</option>\
                                    </select>\
                                </div>\
                                <div class="float-dd">\
                                    Float:\
                                    <select aria-labelledby="cke_273_label" class="cke_dialog_ui_input_select selectAlignment">\
                                        <option value="">None</option>\
                                        <option value="float: left;">Left</option>\
                                        <option value="float: right;">Right</option>\
                                    </select>\
                                </div>\
                                <div class="clear"></div>\
                            </div>\
                            <div class="item-dd">\
                                <div class="limit-number-dd">\
                                    <input type="checkbox" id="sourceImageChecked" />\
                                    <label style="white-space: normal;" for="selectLimit" class="cke_dialog_ui_labeled_label">Limit the number of times this object is draggable:</label>\
                                    <div aria-labelledby="cke_273_label" role="radiogroup" class="cke_dialog_ui_labeled_content" id="sourceImageWrapper">\
                                        <input type="text" id="sourceImageLimit" class="txtFullcreate" />\
                                    </div>\
                                    <div class="clear"></div>\
                                </div>\
                            </div>\
                            <div class="item-dd">\
                                <div class="image-section-dd">\
                                    <div class="cke_dialog_ui_html">\
                                        <div style="display:none" class="ImagePreviewLoader">\
                                            <div class="loading">&nbsp;</div>\
                                        </div>\
                                        <div class="ImagePreviewBox" style="width:100%;">\
                                            <table>\
                                                <tbody>\
                                                    <tr>\
                                                        <td>\
                                                            <a onclick="return false;" target="_blank" href="javascript:void(0)">\
                                                                <img alt="" class="previewImage" style="display: none;" drawable="false">\
                                                            </a>\
                                                        </td>\
                                                    </tr>\
                                                </tbody>\
                                            </table>\
                                        </div>\
                                    </div>\
                                </div>\
                            </div>\
                            <div class="item-dd">\
                                <div class="image-section-dd">\
                                    Description of image:<br />\
                                    <textarea id="source-image-dd" class="img-description"></textarea>\
                                </div>\
                            </div>\
                        </div>\
                      </div>';

            return {
                title: 'Drag and Drop - Source Image',
                minWidth: IS_V2 ? 700 : 550,
                minHeight: 200,
                contents:
		        [
			        {
			            id: 'imageUploadExe',
			            label: 'Settings',
			            elements:
				        [
					        {
					            type: 'html',
					            html: myhtml,
					            onLoad: function (a) {

					                var img,
                                        file;

					                var $currentDialog = $('#wrap_' + editor.name + '_partialAddSourceImage_dialog');

					                formUpload = $currentDialog.find('.formPartialAddSourceImageFile'),
                                    formUploadButton = $currentDialog.find('.uploadImageButton'),
                                    formImageUploadFile = $currentDialog.find('.formPartialAddSourceImageFile'),
                                    selectPercent = $currentDialog.find('.selectPercent'),
                                    selectAlignment = $currentDialog.find('.selectAlignment'),
                                    previewImage = $currentDialog.find('.previewImage');
					                tts = $currentDialog.find('.img-description');

                                    var $sourceImageChecked = $('#sourceImageChecked');
                                    var $sourceImageWrapper = $('#sourceImageWrapper');
                                    var $sourceImageLimit = $('#sourceImageLimit');
                                    var sourceImageLimitValue = $sourceImageLimit.val();


                                    checkDrawable = false;

                                    if(previewImage.is(':visible')){
                                        img = previewImage;
                                        imgWidth   = img[0].naturalWidth;
                                        imgHeight  = img[0].naturalHeight;
                                    }

                                    previewImage.attr({'drawable':'false','src':'','style':''});
                                    previewImage.hide();
                                    $('#wrap_' + editor.name + '_partialAddSourceImage_dialog .formPartialAddSourceImageFile,#wrap_' + editor.name + '_partialAddSourceImage_dialog .selectDrawable,#wrap_' + editor.name + '_partialAddSourceImage_dialog .selectAlignment').val('');
                                    selectPercent.val(10);


                                    formUploadButton.click(function(event) {
                                        imageUploadClick = true;
                                        var imgFile = formImageUploadFile;
                                        if (imgFile.get(0).value == "") {
                                            customAlert("Please select image file.");
                                            return;
                                        }

                                        var file = imgFile.get(0).value;
                                        var extension = file.substr((file.lastIndexOf('.') + 1)).toLowerCase();

                                        var isCorrectFormat = false;
                                        var fileExtensions = ['jpg', 'jpeg', 'png', 'tif', 'svg'];

                                        isCorrectFormat = _.some(fileExtensions, function(ext) {
                                            return ext === extension;
                                        });

                                        if (extension === 'tif') {
                                            customAlert('This image with format .tif only show on safari.');
                                        }

                                        if (!isCorrectFormat) {
                                            var fileExts = fileExtensions.join(', ');
                                            var fileExtsMsg = 'Unsupported file type. Please select ' + fileExts + ' file.';
                                            customAlert(fileExtsMsg);
                                            return;
                                        }

                                        /* Act on the event */
                                        checkChanged = true;

                                        me = this;
                                        $('#wrap_' + editor.name + '_partialAddSourceImage_dialog').find('#objectId').val(objectId);
                                        PartialAddSourceImage($('#wrap_' + editor.name + '_partialAddSourceImage_dialog #formPartialAddSourceImage').get(0), previewImage, selectPercent, imgUpload);

                                        return false;
                                    });

                                    formUpload.change(function(event) {
                                        /* Act on the event */
                                        checkChanged = true;
                                    });

                                    selectPercent.change(function(event) {
                                        /* Act on the event */
                                        checkChanged = true;
                                        var percentValue = parseInt($(this).val());
                                        previewImage.attr('percent',percentValue);
                                    });

                                    selectAlignment.change(function(event) {
                                        /* Act on the event */
                                        checkChanged = true;
                                        var alignValue = $(this).val();
                                        previewImage.attr('style',alignValue);
                                    });

                                    // Check status limit the number of draggable is checked or not checked
                                    $sourceImageChecked.on('change', function() {
                                        var $self = $(this);

                                        if ($self.is(':checked')) {
                                            $sourceImageWrapper.show();
                                            if (sourceImageLimitValue === 'unlimited' || sourceImageLimitValue === '') {
                                                $sourceImageLimit.focus().val(1);
                                            }
                                        } else {
                                            $sourceImageWrapper.hide();
                                        }
                                    });
					            },
					            onShow: function () {
					                //hide tooltip
					                $('#tips .tool-tip-tips').css({
					                    'display': 'none'
					                });
                                    checkDrawable = false;
                                    refreshPartialCredit();
                                    previewImage.attr('drawable','false');
                                    $('#wrap_' + editor.name + '_partialAddSourceImage_dialog .formPartialAddSourceImageFile,#wrap_' + editor.name + '_partialAddSourceImage_dialog .selectDrawable,#wrap_' + editor.name + '_partialAddSourceImage_dialog .selectAlignment').val('');
                                    previewImage.attr('src', '').hide();
                                    tts.val("");

                                    if(window.editImage){
                                        var loadedSrc = eleImage.$.attributes['src'] ? eleImage.$.attributes['src'].nodeValue : "";
                                        var loadedDra = eleImage.$.attributes['drawable'] ? eleImage.$.attributes['drawable'].nodeValue : "false";
                                        var percentImg = eleImage.$.attributes['percent'] ? eleImage.$.attributes['percent'].nodeValue : -1;
                                        var alignmentImg = eleImage.$.attributes['style'] ? eleImage.$.attributes['style'].nodeValue : "";
                                        var ttsImg = eleImage.$.attributes['texttospeech'] ? eleImage.$.attributes['texttospeech'].nodeValue : "";
                                        if(loadedDra == "false"){
                                            previewImage.attr('drawable',"false");
                                            checkDrawable = false;
                                        }else if(loadedDra == "true"){
                                            previewImage.attr('drawable',"true");
                                            checkDrawable = true;
                                        }else{
                                            previewImage.attr('drawable',"false");
                                            checkDrawable = false;
                                        }
                                        selectPercent.val(percentImg);
                                        selectAlignment.val(alignmentImg);
                                        previewImage.attr({'src':loadedSrc,'style':alignmentImg});
                                        previewImage.show();
                                        tts.val(ttsImg);
                                        if(percentImg != -1){
                                            selectPercent.val(percentImg);
                                            previewImage.attr('percent',percentImg);
                                        }else{
                                            var loadedWid = parseInt(eleImage.$.attributes['width'].nodeValue) || 0;
                                            if(!loadedWid){
                                                percentImg = "10";
                                                previewImage.attr('percent',percentImg);
                                            }else{
                                                //perTemp = loadedWid
                                                selectPercent.val(10);
                                                previewImage.attr('percent',10);
                                                var theImage = new Image();
                                                theImage.onload = function(){
                                                    // Get accurate measurements from that.
                                                    var per = Math.round((loadedWid/this.width)*10);
                                                    selectPercent.val(per);
                                                    previewImage.attr('percent',per);
                                                };
                                                theImage.src = loadedSrc;
                                            }
                                        }
                                        //$('#formImageUploadFile').val(loadedSrc);
                                        // Get on screen image
                                        var screenImage = previewImage;

                                        // Create new offscreen image to test
                                        var theImage = new Image();
                                        theImage.onload = function(){
                                            // Get accurate measurements from that.
                                            imgWidth = this.width;
                                            imgHeight = this.height;
                                        };
                                        theImage.src = screenImage.attr("src");
                                    }

                                    var $sourceImageLimit = $('#sourceImageLimit');
                                    var $sourceImageChecked = $('#sourceImageChecked');
                                    var $sourceImageWrapper = $('#sourceImageWrapper');
                                    var limit;

                                    $sourceImageLimit.ckUpDownNumber({
                                        maxNumber: 999,
                                        width: 18,
                                        height: 13
                                    });

                                    $sourceImageLimit.attr('maxlength', 3);

                                    // Check the first time called source image
                                    if (previewImage.attr('src') == '') {
                                        // Reset all to default options
                                        limit = 'unlimited';
                                        selectPercent.prop('selectedIndex', selectPercent.find('option').length - 1);
                                        selectAlignment.prop('selectedIndex', 0);
                                    } else {
                                        // Check source partial exist
                                        if (sourcePartial.length > 0) {
                                            for (var i = 0; i < sourcePartial.length; i++) {
                                                var referIdentifier = eleAddSourceText.getAttribute("srcidentifier");
                                                if (sourcePartial[i].srcIdentifier == referIdentifier) {
                                                    limit = sourcePartial[i].limit;

                                                    // Check limit if it is unlimited or before not set
                                                    limit = (limit === 'unlimited' || limit === 'undefined' || limit === undefined || limit === '') ? 'unlimited' : limit;

                                                    $sourceImageLimit.val(limit);
                                                }
                                            }
                                        }
                                    }

                                    if (limit === 'unlimited') {
                                        $sourceImageChecked.prop('checked', false);
                                        $sourceImageWrapper.hide();
                                    } else {
                                        $sourceImageChecked.prop('checked', true);
                                        $sourceImageWrapper.show();
                                    }

                        if (IS_V2) {
                          $('.uploadImageButton.cke_dialog_ui_fileButton').css('text-decoration', 'none')
                        }
					            }
					        }
				        ]
			        }
		        ],
                onOk: function () {

                    imageUploadOkCancelClick = true;
                    var imgSrc  = previewImage.attr('src'),
                        imgDraw = previewImage.attr('drawable'),
                        percent = previewImage.attr('percent'),
                        alignment = selectAlignment.val(),
                        texttospeech = convertTexttoHTML($("<div></div>").append(tts.val()).text()),
                        htmlImage = '';
                        imgWidth  *= (percent/10);
                        imgHeight  *= (percent/10);

                    var sourceImageLimit = $('#sourceImageLimit').val();
                    var $sourceImageChecked = $('#sourceImageChecked');
                    var errorMessage = 'Please input valid value of limit the number of draggable';

                    if (sourceImageLimit == 0 && $sourceImageChecked.is(':checked')) {
                        alert(errorMessage);
                        return false;
                    }

                    sourceImageLimit = sourceImageLimit > 0 ? parseInt(sourceImageLimit) : 'unlimited';


                    if (imgSrc != "" && previewImage.width() != 0) {

                        //Create scrIdentifier
                        var newsrcIdentifier = "";

                        if (!$sourceImageChecked.is(':checked')) {
                            sourceImageLimit = 'unlimited';
                        }

                        if (window.editImage) {
                            //Update value for source
                            for (var i = 0; i < sourcePartial.length; i++) {
                                newsrcIdentifier = eleAddSourceText.getAttribute("srcidentifier");
                                if (sourcePartial[i].srcIdentifier == newsrcIdentifier) {
                                    sourcePartial[i].value = '<img style="' + alignment + '" srcIdentifier="' + newsrcIdentifier + '" percent="' + percent + '" height="' + imgHeight + '" width="' + imgWidth + '" src="' + imgSrc + '" texttospeech="' + texttospeech + '" />';
                                    sourcePartial[i].limit = sourceImageLimit; // Adding limit dragged for source image
                                }
                            }

                        } else {
                            //Get source from iResult to partialSource
                            for (var i = 0; i < iResult.length; i++) {
                                if (iResult[i].type == "partialCredit") {
                                    sourcePartial = iResult[i].source;
                                }
                            };

                            //Create srcIdentifier
                            newsrcIdentifier = createSrcIdentifier(sourcePartial);

                            sourcePartial.push({ srcIdentifier: newsrcIdentifier, type: "image", limit: sourceImageLimit, value: '<img style="' + alignment + '" srcIdentifier="' + newsrcIdentifier + '" percent="' + percent + '" height="' + imgHeight + '" width="' + imgWidth + '" src="' + imgSrc + '" texttospeech="' + texttospeech + '" />' });


                        }

                        //Add source from temp to iResult
                        for (var i = 0; i < iResult.length; i++) {
                            if (iResult[i].type == "partialCredit") {
                                iResult[i].source = sourcePartial;
                            }
                        }

                        //Insert Image to Editor
                        htmlImage = '<img contenteditable="false" style="' + alignment + '" srcIdentifier="' + newsrcIdentifier + '" partialID="Partial_1" class="partialSourceObject partialAddSourceImage" percent="' + percent + '" height="' + imgHeight + '" width="' + imgWidth + '" src="' + imgSrc + '" texttospeech="' + texttospeech + '" />';

                        if (window.editImage) {
                            $(eleImage.$).replaceWith(htmlImage);
                        } else {
                            htmlImage = '<span class="partialSourceImageWrapper" contenteditable="false" unselectable="on">' + htmlImage + '</span>'
                            var sourceImageElement = CKEDITOR.dom.element.createFromHtml(htmlImage);
                            editor.insertElement(sourceImageElement);
                        }
                    }

                    checkChanged = false;
                    window.editImage = false;

                },
                onCancel: function () {
                    var that = this;
                    imageUploadOkCancelClick = true;
                    window.editImage = false;
                    if(checkChanged){
                        customConfirm('You have changed the image. Are you sure you want to close?').then(function(sure) {
                            if (sure) {
                                checkChanged = false;
                                that.hide();
                            }
                        })
                        return false;
                    }
                    return true;
                }
            };
        });
    }
});

var imgWidth, imgHeight, imgWidthOriginal, imgHeightOriginal, iframeId, iframe = document.createElement("iframe");
function PartialAddSourceImage(form, previewImage, selectPercent, action_url) {
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

        //previewImage.attr('src', GetViewReferenceImg + data.url);
        previewImage.attr('src', data.absoluteUrl);
        var percentFirst = selectPercent.val();
        previewImage.attr({'percent': percentFirst,'style':''});
        previewImage.show();
        // Create new offscreen image to test
        var theImage = new Image();
        theImage.onload = function () {
            //Get accurate measurements from that.
            imgWidthOriginal = this.width;
            imgHeightOriginal = this.height;
            imgWidth = this.width;
            imgHeight = this.height;
        };
        theImage.src = previewImage.attr("src");
        document.getElementById("formPartialAddSourceImage").reset();//Reset upload
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
var imageUploadOkCancelClick = false;
var imageUploadClick = false;
