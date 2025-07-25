CKEDITOR.plugins.add('imageupload', {
    lang: 'en', // %REMOVE_LINE_CORE%
    icons: 'imageupload',
    requires: 'dialog',
    hidpi: true, // %REMOVE_LINE_CORE%
    init: function (editor) {

        editor.addCommand('insertImage', new CKEDITOR.dialogCommand('insertImage'));

        editor.ui.addButton('ImageUpload',
		{
		    label: 'Insert Image',
		    command: 'insertImage',
		    icon: this.path + 'icons/image.png',
		    toolbar: 'insertImage,30'
		});

        editor.widgets.add('imageupload', {
            inline: true,
            mask: true
        });

        var eleTextEntry = ""; //This use for firstime load popup

        window.editImage = false;

        editor.on('doubleclick', function (evt) {

            var ele = evt.data.element;

            if (ele.hasClass("imageupload")) {
                if (ele.getAttribute("data-math-type") === 'leaui_fomula') {
                    evt.data.dialog = 'myiframedialogDialog';
                    return;
                }
                evt.data.dialog = 'insertImage';
                eleImage = ele;
                //The status to editor know this is update
                window.editImage = true;
            }
        });



        CKEDITOR.dialog.add('insertImage', function (editor) {
            var checkChanged = false,
                checkDrawable = false,
                formUpload = null,
                formUploadButton = null,
                formImageUploadFile = null,
                selectPercent = null,
                selectAlignment = null,
                checkboxDrawable = null,
                previewImage = null;

            myhtml = '\
                    <style>\
                        #previewImage{ max-width:400px; max-height:200px;}\
                    </style>\
                    <div id="wrap_' + editor.name + '_imageupload_dialog" class="imageUploadContent">\
                        <table cellspacing="0" border="0" align="left" style="width:100%;" role="presentation">\
                            <tbody>\
                                <tr>\
                                    <td class="cke_dialog_ui_vbox_child" role="presentation">\
                                        <div class="cke_dialog_ui_file" role="presentation" style="width:90%;float:left;">\
                                            <label class="cke_dialog_ui_labeled_label"></label>\
                                            <div aria-labelledby="cke_263_label" role="radiogroup" class="cke_dialog_ui_labeled_content cke_dialog_ui_input_file">\
                                                <form name="form-upload" class="formImageUpload" id="formImageUpload" lang="en" action="uploader.php?type=images" dir="ltr" method="POST" enctype="multipart/form-data">\
                                                    <input type="file" size="38" name="file" aria-labelledby="cke_262_label" class="formImageUploadFile" style="height: 21px;border: solid 1px #cccccc;width: 400px;" />\
                                                    <input type="hidden" name="id" id="objectId" />\
                                                </form>\
                                            </div>\
                                        </div>\
                                    </td>\
                                    <td class="cke_dialog_ui_vbox_child" role="presentation" style="text-align:right;">\
                                        <a class="uploadImageButton cke_dialog_ui_fileButton cke_dialog_ui_button" role="button" hidefocus="true" title="Upload" href="javascript:void(0)" style="-moz-user-select: none; margin-right:15px;float: right;">\
                                            <span class="cke_dialog_ui_button">Upload</span>\
                                        </a>\
                                    </td>\
                                </tr>\
                                <tr>\
                                    <td class="cke_dialog_ui_vbox_child" role="presentation" colspan="2">\
                                        <table class="cke_dialog_ui_hbox" role="presentation">\
                                            <tbody>\
                                                <tr class="cke_dialog_ui_hbox">\
                                                    <td style="width:20%;" role="presentation" class="cke_dialog_ui_hbox_first">\
                                                        <div class="cke_dialog_ui_vbox" role="presentation">\
                                                            <table cellspacing="0" border="0" align="left" style="width:100%;" role="presentation">\
                                                                <tbody>\
                                                                    <tr>\
                                                                        <td class="cke_dialog_ui_vbox_child" role="presentation">\
                                                                            <div style="width : 100px" class="cke_dialog_ui_select" role="presentation">\
                                                                                <div aria-labelledby="cke_272_label" role="radiogroup" class="cke_dialog_ui_labeled_content">\
                                                                                    <div role="presentation" class="cke_dialog_ui_input_select">\
                                                                                        <input checked="checked" type="checkbox" name="checkboxDrawable" class="checkboxDrawable" id="checkboxDrawable" style="margin-right:10px;"><label style="vertical-align:top;" for="checkboxDrawable" class="cke_dialog_ui_labeled_label">Drawable</label>\
                                                                                    </div>\
                                                                                </div>\
                                                                            </div>\
                                                                        </td>\
                                                                    </tr>\
                                                                    <tr>\
                                                                        <td class="cke_dialog_ui_vbox_child" role="presentation">\
                                                                            <div style="width : 100px" class="cke_dialog_ui_select" role="presentation">\
                                                                                <label style="line-height:2em;" for="selectPercent" class="cke_dialog_ui_labeled_label">Percent</label>\
                                                                                <div aria-labelledby="cke_272_label" role="radiogroup" class="cke_dialog_ui_labeled_content">\
                                                                                    <div role="presentation" class="cke_dialog_ui_input_select">\
                                                                                        <select style="width : 100px" aria-labelledby="cke_272_label" class="cke_dialog_ui_input_select selectPercent">\
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
                                                                                </div>\
                                                                            </div>\
                                                                        </td>\
                                                                    </tr>\
                                                                    <tr>\
                                                                        <td class="cke_dialog_ui_vbox_child" role="presentation">\
                                                                            <div style="width : 100px" class="cke_dialog_ui_select" role="presentation">\
                                                                                <label style="line-height:2em;" for="selectAlignment" class="cke_dialog_ui_labeled_label">Float</label>\
                                                                                <div aria-labelledby="cke_273_label" role="radiogroup" class="cke_dialog_ui_labeled_content">\
                                                                                    <div role="presentation" class="cke_dialog_ui_input_select">\
                                                                                        <select style="width : 100px" aria-labelledby="cke_273_label" class="cke_dialog_ui_input_select selectAlignment">\
                                                                                            <option value="">None</option>\
                                                                                            <option value="float: left;">Left</option>\
                                                                                            <option value="float: right;">Right</option>\
                                                                                        </select>\
                                                                                    </div>\
                                                                                </div>\
                                                                            </div>\
                                                                        </td>\
                                                                    </tr>\
                                                                </tbody>\
                                                            </table>\
                                                        </div>\
                                                    </td>\
                                                    <td style="width:80%;" role="presentation" class="cke_dialog_ui_hbox_last">\
                                                        <div class="cke_dialog_ui_vbox" role="presentation">\
                                                            <table cellspacing="0" border="0" align="left" style="width:100%;" role="presentation">\
                                                                <tbody>\
                                                                    <tr>\
                                                                        <td class="cke_dialog_ui_vbox_child" role="presentation">\
                                                                            <div style="width:95%" class="cke_dialog_ui_html">\
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
                                                                        </td>\
                                                                    </tr>\
                                                                </tbody>\
                                                            </table>\
                                                        </div>\
                                                    </td>\
                                                </tr>\
                                            </tbody>\
                                        </table>\
                                    </td>\
                                </tr>\
                                <tr>\
                                    <td class="cke_dialog_ui_vbox_child" role="presentation" colspan="2">\
                                        Description of image:<br />\
                                        <textarea id="img-text-to-speech" class="img-text-to-speech"></textarea>\
                                    </td>\
                                </tr>\
                            </tbody>\
                        </table>\
                      </div>';

            return {
                title: 'Image Upload',
                minWidth: 550,
                minHeight: 200,
                contents:
		        [
			        {
			            id: 'imageUploadGenerate',
			            label: 'Settings',
			            elements:
				        [
					        {
					            type: 'html',
					            html: myhtml,
					            onLoad: function () {
					                var img;
                                    var file;
                                    var imageuploadEditor = editor.name;
                                    var $imageuploadDialog = formUpload = $('#wrap_' + imageuploadEditor + '_imageupload_dialog');
                                    var $imageuploadGenerate = $('div[name="imageUploadGenerate"]:visible');

					                formUpload = $imageuploadDialog.find('.formImageUploadFile'),
                                    formUploadButton = $imageuploadDialog.find('.uploadImageButton'),
                                    formImageUploadFile = $imageuploadDialog.find('.formImageUploadFile'),
					                selectPercent = $imageuploadGenerate.find('.selectPercent'),
					                selectAlignment = $imageuploadGenerate.find('.selectAlignment'),
					                checkboxDrawable = $imageuploadGenerate.find('.checkboxDrawable'),
                                    previewImage = $imageuploadDialog.find('.previewImage'),
					                textToSpeech = $imageuploadGenerate.find('.img-text-to-speech');

					                checkDrawable = false;

					                if (previewImage.is(':visible')) {
					                    img = previewImage;
					                    imgWidth = img[0].naturalWidth;
					                    imgHeight = img[0].naturalHeight;
					                }

					                previewImage.attr({ 'drawable': 'false', 'src': '', 'style': '' });
					                previewImage.hide();
					                $('#wrap_' + imageuploadEditor + '_imageupload_dialog .formImageUploadFile,#wrap_' + imageuploadEditor + '_imageupload_dialog .selectDrawable,#wrap_' + imageuploadEditor + '_imageupload_dialog .selectAlignment').val('');
					                $('#wrap_' + imageuploadEditor + '_imageupload_dialog .checkboxDrawable').prop('checked', false);
					                selectPercent.val(10);

					                $('.uploadImageButton').click(function (event) {
					                    refeshConfig();
					                    imageUploadClick = true;
					                    var imgFile = $(this).parents('.cke_dialog_contents_body').find('.formImageUploadFile');
					                    if (imgFile.get(0).value == "") {
                                            customAlert('Please select image file');
					                        return;
					                    }

					                    var file = imgFile.get(0).value;
					                    var extension = file.substr((file.lastIndexOf('.') + 1)).toLowerCase();

					                    var isCorrectFormat = false;
                                        var fileExtensions = ['jpg', 'jpeg', 'png', 'gif', 'svg'];

                                        isCorrectFormat = _.some(fileExtensions, function(ext) {
                                            return ext === extension;
                                        });

                                        if (!isCorrectFormat) {
                                            var fileExts = fileExtensions.join(', ');
                                            var fileExtsMsg = 'Unsupported file type. Please select ' + fileExts + ' file.';
                                            customAlert(fileExtsMsg);
                                            return;
                                        }

					                    /* Act on the event */
					                    checkChanged = true;

					                    me = this;
					                    $(this).parents('.cke_dialog_contents_body').find('#objectId').val(objectId);
					                    previewImage = $(this).parents('.cke_dialog_contents_body').find('.previewImage');

					                    imageUpload($(this).parents('.cke_dialog_contents_body').find('#formImageUpload').get(0), previewImage, $(this).parents('.cke_dialog_contents_body').find('.selectPercent'), imgUpload);

					                    return false;
					                });

					                formUpload.change(function (event) {
					                    /* Act on the event */
					                    checkChanged = true;
					                });

					                selectPercent.change(function (event) {
					                    /* Act on the event */
					                    checkChanged = true;
					                    var percentValue = parseInt($(this).val());
					                    previewImage.attr('percent', percentValue);
					                });

					                selectAlignment.change(function (event) {
					                    /* Act on the event */
					                    checkChanged = true;
					                    var alignValue = $(this).val();
					                    previewImage.attr('style', alignValue);
					                });

					                checkboxDrawable.change(function (event) {
					                    /* Act on the event */
					                    checkChanged = true;
					                    var drawableValue = false;
					                    if (this.checked) {
					                        var drawableValue = true;
					                    }
					                    checkDrawable = drawableValue || false;
					                    previewImage.attr('drawable', checkDrawable);
					                });


					            },
					            onShow: function () {
					                checkDrawable = false;
					                //hide tooltip
					                $('#tips .tool-tip-tips').css({
					                    'display': 'none'
					                });

					                if (editor.name === "txtTestPropertiesInstruction" || editor.name === "PrintTestInstructions") {
					                    //Hide Draw attribute
					                    $(".checkboxDrawable").parents("tr").eq(0).hide();
					                }

					                previewImage.attr('drawable', 'false');
					                $('#wrap_' + editor.name + '_imageupload_dialog .formImageUploadFile,#wrap_' + editor.name + '_imageupload_dialog .selectDrawable,#wrap_' + editor.name + '_imageupload_dialog .selectAlignment').val('');
					                $('#wrap_' + editor.name + '_imageupload_dialog .checkboxDrawable').prop('checked', false);
					                previewImage.attr('src', '').hide();

					                if (window.editImage) {
					                    var loadedSrc = eleImage.$.attributes['src'] ? eleImage.$.attributes['src'].nodeValue : "";
					                    var loadedDra = eleImage.$.attributes['drawable'] ? eleImage.$.attributes['drawable'].nodeValue : "false";
					                    var percentImg = eleImage.$.attributes['percent'] ? eleImage.$.attributes['percent'].nodeValue : -1;
					                    var alignmentImg = eleImage.$.attributes['style'] ? eleImage.$.attributes['style'].nodeValue : "";
					                    var dataMathType = eleImage.$.attributes['data-math-type'] ? eleImage.$.attributes['data-math-type'].nodeValue : "";
					                    if (loadedDra == "false") {
					                        checkboxDrawable.prop('checked', false);
					                        previewImage.attr('drawable', "false");
					                        checkDrawable = false;
					                    } else if (loadedDra == "true") {
					                        checkboxDrawable.prop('checked', true);
					                        previewImage.attr('drawable', "true");
					                        checkDrawable = true;
					                    } else {
					                        checkboxDrawable.prop('checked', false);
					                        previewImage.attr('drawable', "false");
					                        checkDrawable = false;
					                    }

					                    if (dataMathType !== "") {
					                        previewImage.attr('data-math-type', dataMathType);
					                    }

					                    selectPercent.val(percentImg);
					                    selectAlignment.val(alignmentImg);
					                    previewImage.attr({ 'src': loadedSrc, 'style': alignmentImg });
					                    previewImage.show();
					                    if (parseInt(percentImg) != -1) {
					                        selectPercent.val(percentImg);
					                        previewImage.attr('percent', percentImg);
					                    } else {
					                        var loadedWid = 0;

					                        if (eleImage.$.attributes['width'] != undefined) {
					                            loadedWid = parseInt(eleImage.$.attributes['width'].nodeValue) || 0
					                        }

					                        if (!loadedWid) {
					                            percentImg = "10";
					                            previewImage.attr('percent', percentImg);
					                        } else {
					                            //perTemp = loadedWid
					                            selectPercent.val(10);
					                            previewImage.attr('percent', 10);
					                            var theImage = new Image();
					                            theImage.onload = function () {
					                                // Get accurate measurements from that.
					                                var per = Math.round((loadedWid / this.width) * 10);
					                                selectPercent.val(per);
					                                previewImage.attr('percent', per);
					                            };
					                            theImage.src = loadedSrc;
					                        }
					                    }
					                    //$('#formImageUploadFile').val(loadedSrc);
					                    // Get on screen image
					                    var screenImage = previewImage;

					                    // Create new offscreen image to test
					                    var theImage = new Image();
					                    theImage.onload = function () {
					                        // Get accurate measurements from that.
					                        imgWidth = this.width;
					                        imgHeight = this.height;
					                    };
					                    theImage.src = screenImage.attr("src");

                                        var texttospeechImg = $(eleImage.$).attr('texttospeech');
                                        texttospeechImg = texttospeechImg === undefined ? '' : texttospeechImg;
                                        textToSpeech.val(texttospeechImg);
					                } else {
                                        // Reset image upload text to speech
                                        textToSpeech.val('');
                                    }


					                if (hasGuidance) {
					                    checkboxDrawable.parent('.cke_dialog_ui_input_select').hide();
					                }
					            }
					        }
				        ]
			        }
		        ],
                onOk: function () {
                    isAddImage = true;
                    imageUploadOkCancelClick = true;
                    var imgSrc = previewImage.attr('src'),
                        imgDraw = previewImage.attr('drawable'),
                        percent = previewImage.attr('percent'),
                        alignment = selectAlignment.val(),
                        tts = convertTexttoHTML(textToSpeech.val()),
                        dataMathType = previewImage.attr('data-math-type') ? 'data-math-type="'+ previewImage.attr('data-math-type') +'"': "";
                        htmlImage = '';
                    imgWidth *= (percent / 10);
                    imgHeight *= (percent / 10);

                    var classLeft = $('.selectAlignment option:selected').text() === 'Left' ? 'leftImage' : ''; // add class if image left

                    if (imgSrc != "" && previewImage.width() != 0) {
                      //Insert Image to Editor
                      var style = "";
                      if (alignment != "") {
                          style = 'style="' + alignment + '" ';
                      }

                      var $currentSelection = $(editor.getSelection().getStartElement().$);

                      if ($currentSelection.parents(".linkit-table").length > 0)
                      {
                          //This is adjust size for image when import
                          var tdWidth = $currentSelection.width();
                          if (tdWidth < imgWidth)
                          {
                              imgHeight = "auto";
                              imgWidth = "100%";
                          }
                      }

                      if (tts == undefined) {
                          tts = "";
                      }
                      var className = 'class="imageupload ' + classLeft
                      htmlImage = '<img contenteditable="false" ' + style + className.trim() + '" percent="' + percent + '" height="' + imgHeight + '" width="' + imgWidth + '" src="' + imgSrc + '" drawable="' + imgDraw + '" texttospeech="' + tts + '" ' + dataMathType + ' />';

                      var imageuploadElement = CKEDITOR.dom.element.createFromHtml(htmlImage);
                        editor.insertElement(imageuploadElement);

                      isAddImage = false;

                      $('iframe[allowtransparency]').contents().find('body').trigger('focus.right');
                      $('iframe[allowtransparency]').contents().find('body').trigger('focus.left');
                    }
                    checkChanged = false;
                  window.editImage = false;
                  $('.divMultipleChoice').css('display', 'table');
                  setTimeout(function() {
                    $('.divMultipleChoice').css('display', 'block');
                  }, 500)
                  $('.divMultipleChoiceVariable').css('display', 'table');
                  setTimeout(function() {
                    $('.divMultipleChoiceVariable').css('display', 'block');
                  }, 500)
                },
                onCancel: function () {
                    var that = this;
                    imageUploadOkCancelClick = true;
                    window.editImage = false;

                    //exit popup audio when at guidance or rationales
                    if (hasGuidance) {
                        checkChanged = false;
                    }

                    if (checkChanged) {
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
function imageUpload(form, previewImage, selectPercent, action_url) {
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
        if (data.success == 'false') {
            customAlert(data.errorMessage);
        }
        // Del the iframe...
        setTimeout(removeIFrame, 250);

        if (data.url != null)
            data.url = data.url.replace('+', encodeURIComponent('+'));
        previewImage.attr('src', data.absoluteUrl);
        var percentFirst = selectPercent.val();
        previewImage.attr({ 'percent': percentFirst, 'style': '' });
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
