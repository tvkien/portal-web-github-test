CKEDITOR.plugins.add('audioupload', {
    lang: 'en', // %REMOVE_LINE_CORE%
    icons: 'audioupload',
    requires: 'dialog',
    hidpi: true, // %REMOVE_LINE_CORE%
    init: function (editor) {
        var pluginName = 'insertAudio';

        editor.addCommand(pluginName, new CKEDITOR.dialogCommand(pluginName));

        editor.ui.addButton('AudioUpload', {
		    label: 'Instructional Audio',
		    command: pluginName,
		    icon: this.path + 'icons/audio.png',
		    toolbar: 'insertAudio,30'
		});

        editor.widgets.add('audioupload', {
            inline: true,
            mask: true
        });

        CKEDITOR.dialog.add(pluginName, function () {
            var checkChanged = false;
            var getImgByVersion = CKEDITOR.plugins.getImgByVersion;

            myhtml = '\
                    <div id="audioUploadContent">\
                        <table cellspacing="0" border="0" align="left" style="width:100%;" role="presentation">\
                            <tbody>\
                                <tr style="padding-bottom: 20px;">\
                                    <td class="cke_dialog_ui_vbox_child" role="presentation">\
                                        <div class="cke_dialog_ui_file" id="cke_265_uiElement" role="presentation">\
                                            <label id="cke_263_label" class="cke_dialog_ui_labeled_label"></label>\
                                            <div aria-labelledby="cke_263_label" role="radiogroup" class="cke_dialog_ui_labeled_content cke_dialog_ui_input_file">\
                                                <form name="form-upload" id="formAudioUpload" lang="en" action="uploader.php?type=mp3" dir="ltr" method="POST" enctype="multipart/form-data">\
                                                    <input type="file" size="38" name="file" aria-labelledby="cke_262_label" id="formAudioUploadFile" style="max-width: 100%; overflow: hidden; text-overflow: ellipsis; width: 94%;height: auto;border: solid 1px #cccccc;">\
                                                    <input type="hidden" name="id" id="objectId" />\
                                                </form>\
                                            </div>\
                                        </div>\
                                    </td>\
                                    <td class="cke_dialog_ui_vbox_child" role="presentation">\
                                        <a id="uploadAudioButton" role="button" class="cke_dialog_ui_fileButton cke_dialog_ui_button" hidefocus="true" title="Upload" href="javascript:void(0)" style="-moz-user-select: none;float: right;">\
                                            <span class="cke_dialog_ui_button" id="cke_266_label">Upload</span>\
                                        </a>\
                                    </td>\
                                </tr>\
                                <tr>\
                                    <td colspan="2">\
                                        <div id="demoAudio" style="width:72%; -moz-user-select: none; display:none; background:#026211; border-radius:25px;padding: 3px 0; margin: 10px auto;">\
                                            <table>\
                                                <tr>\
                                                    <td style="vertical-align:middle; text-align:left;">\
                                                        <div class="audio" id="audioQuestionDemo" style="margin-left:-35px">\
                                                            <div id="audioRemoveQuestion" style="width: 40px;padding-top:2px;">\
                                                                <img alt="Play audio" class="bntPlay" src="{{audio_play}}" title="Play audio" />\
                                                                <img alt="Stop audio" class="bntStop" src="{{audio_stop}}" title="Stop audio" />\
                                                                <span class="audioRef"></span>\
                                                            </div>\
                                                        </div>\
                                                    </td>\
                                                    <td style="vertical-align:middle !important;">\
                                                        <p style="font-size:12px; color:#fff; text-indent:3px;">Click on icon to play audio.</p>\
                                                    </td>\
                                                </tr>\
                                            </table>\
                                        </div>\
                                    </td>\
                                </tr>\
                            </tbody>\
                        </table>\
                      </div>';
            myhtml = myhtml.replace('{{audio_play}}', getImgByVersion('multiplechoice', 'images/small_audio_play.png'))
                .replace('{{audio_stop}}', getImgByVersion('multiplechoice', 'images/small_audio_stop.png'));

            return {
                title: 'Instructional Audio Upload',
                minWidth: IS_V2 ? 470 : 350,
                minHeight: 100,
                contents:
		        [
			        {
			            id: 'AudioUploadExe',
			            label: 'Settings',
			            elements:
				        [
					        {
					            type: 'html',
					            html: myhtml,
					            onLoad: function (a) {

                                    $('#audio-name').attr('style','').parent('td').attr('style','').next('td').attr('style','');

                                    var $audioUploadExe = $('div[name="AudioUploadExe"]:visible');
					                var formUpload = $audioUploadExe.find('#formAudioUploadFile');
                                    var formUploadButton = $audioUploadExe.find('#uploadAudioButton');
                                    var formAudioUploadFile = $audioUploadExe.find('#formAudioUploadFile');


                                    formUploadButton.click(function (event) {
                                        refeshConfig();
                                        audioUploadClick = true;
                                        var file = formAudioUploadFile.get(0).value;

                                        if (file == "") {
                                            customAlert("Please select audio file.");
                                            return;
                                        }

                                        var extension = file.substr((file.lastIndexOf('.') + 1));

                                        if (extension.toLowerCase() != "mp3") {
                                            customAlert("Unsupported file type. Please select mp3 file.");
                                            return;
                                        }

                                        checkChanged = true;
                                        $audioUploadExe.find("#audioUploadContent").find('#objectId').val(objectId);

                                        audioInstructionUpload($audioUploadExe.find('#formAudioUpload').get(0), audioConfig);

                                        return false;
                                    });

                                    formUpload.change(function(event) {
                                        /* Act on the event */
                                        checkChanged = true;
                                        $("#audioRemoveQuestion .bntPlay").show().next().hide();
                                        stopVNSAudio();
                                        resetUIAudio();
                                    });

                                    //Handlers when controls is clicked
                                    $audioUploadExe.find("#audioQuestionDemo .bntPlay").click(playVNSAudio);
                                    $audioUploadExe.find("#audioQuestionDemo .bntStop").click(stopVNSAudio);

					            },
					            onShow: function () {
                                    var $audioUploadExe = $('div[name="AudioUploadExe"]:visible');
					                $audioUploadExe.find('#audio-name').attr('style', '').parent('td').attr('style', '').next('td').attr('style', '');
                                    $audioUploadExe.find('#formAudioUploadFile').val('');
                                    $audioUploadExe.find("#audioQuestionDemo audioRef").html('');
                                    $audioUploadExe.find('#demoAudio').hide();
                                    $audioUploadExe.find('#audio-name').hide();
					            }
					        }
				        ]
			        }
		        ],
                onOk: function () {
                    audioUploadOkCancelClick = true;
                    var audioSrc = $('div[name="AudioUploadExe"]:visible').find("#audioQuestionDemo").find('.audioRef').html();

                    if (audioSrc != '') {
                        if (hasGuidance) {
                            //Insert Audio to Editor Guidance and Rationales Message
                            if ($('.cke_editor_textarea_' + idtypeGuidanceMessage + '_dialog').is(':visible')) {
                                $('.questionTypeGuidance .audioRef_' + idtypeGuidanceMessage).text(audioSrc);
                                $('#questionType_' + idtypeGuidanceMessage).show();
                            }

                            if ($('.cke_editor_textarea_' + idtypeRationaleMessage + '_dialog').is(':visible')) {
                                $('.questionTypeGuidance .audioRef_' + idtypeRationaleMessage).text(audioSrc);
                                $('#questionType_' + idtypeRationaleMessage).show();
                            }

                            if ($('.cke_editor_textarea_' + idtypeGuidanceRationaleMessage + '_dialog').is(':visible')) {
                                $('.questionTypeGuidance .audioRef_' + idtypeGuidanceRationaleMessage).text(audioSrc);
                                $('#questionType_' + idtypeGuidanceRationaleMessage).show();
                            }
                        } else {
                            var dialogClasses = $(this.getElement().$).attr('class');

                            if (dialogClasses.indexOf('passageContent') > -1) {
                                $('#audioPassage').find('.audioRef').text(audioSrc);
                                if (IS_V2) {
                                    $('#topSpacePassage .cke_toolbox').append($('#questionType'));
                                }
                                $('#audioRemovePassage').show();
                                $('#audioRemovePassage').parents('.questionType').show();
                            } else {
                                $('#audioQuestion').find('.audioRef').text(audioSrc);
                                if (IS_V2) {
                                    $('#audioRemoveQuestion .bntPlay').attr('src', getImgByVersion('multiplechoice', 'images/small_audio_play.png'));
                                    $('#audioRemoveQuestion .bntStop').attr('src', getImgByVersion('multiplechoice', 'images/small_audio_stop.png'));
                                    $('#topSpace .cke_toolbox').append($('#questionType'));
                                }
                                $('#audioRemoveQuestion').show();
                                $('#audioRemoveQuestion').parents('.questionType').show();
                            }
                        }
                    }

                    stopVNSAudio();
                    checkChanged = false;

                },
                onCancel: function () {
                    var that = this;
                    audioUploadOkCancelClick = true;

                    //exit popup audio when at guidance or rationales
                    if (hasGuidance) {
                        checkChanged = false;
                    }

                    if(checkChanged) {
                        customConfirm('You have changed the audio. Are you sure you want to close?').then(function(sure) {
                            if(sure){
                                checkChanged = false;
                                stopVNSAudio();
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

var iframeId = document.getElementById("upload_iframe_audio"),
    iframe = document.createElement("iframe");
function audioInstructionUpload(form, action_url) {
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
    iframe.setAttribute("id", "upload_iframe_audio");
    iframe.setAttribute("name", "upload_iframe_audio");
    iframe.setAttribute("width", "0");
    iframe.setAttribute("height", "0");
    iframe.setAttribute("border", "0");
    iframe.setAttribute("style", "width: 0; height: 0; border: none;");

    // Add to document...
    form.parentNode.appendChild(iframe);
    //window.frames['upload_iframe'].name = "upload_iframe";

    iframeId = document.getElementById("upload_iframe_audio");

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
        setTimeout(removeFrame, 250);
        var $audioUploadExe = $('div[name="AudioUploadExe"]:visible');
        var audioName = $audioUploadExe.find('#formAudioUploadFile').get(0).value.substr($audioUploadExe.find('#formAudioUploadFile').get(0).value.lastIndexOf('\\') + 1);
        $audioUploadExe.find('#audio-name').text(audioName).show();
        $audioUploadExe.find("#audioQuestionDemo .audioRef").html(data.absoluteUrl);
        $audioUploadExe.find("#demoAudio").show();
        $audioUploadExe.find('#audioQuestionDemo .bntStop').hide().end().find('#audioQuestionDemo .bntPlay').show();
        stopVNSAudio();
        $('#audio-name').css({
            'white-space': 'normal',
            'display': 'block',
            'color': 'white',
            'padding-left': '10px'
        }).parent('td').css({
            'vertical-align': 'middle',
            'background': '#616161',
            'color': 'white',
            'border-top-left-radius': '5px',
            'border-bottom-left-radius': '5px'
        }).next('td').css({
            'vertical-align': 'middle',
            'background': '#616161',
            'color': 'white',
            'border-top-right-radius': '5px',
            'border-bottom-right-radius': '5px'
        });

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
    form.setAttribute("target", "upload_iframe_audio");
    form.setAttribute("action", action_url);
    form.setAttribute("method", "post");
    form.setAttribute("enctype", "multipart/form-data");
    form.setAttribute("encoding", "multipart/form-data");

    // Submit the form...
    form.submit();
}

function removeFrame() {
    //Check iFrame and only remove iframe has existed
    if (iframeId != null && iframeId.parentNode != null) {
        iframeId.parentNode.removeChild(iframeId);
    }
}

var audioUploadOkCancelClick = false;
var audioUploadClick = false;
