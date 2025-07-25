var $ckEditorDialog;
var editVideo = false;
var editAudio = false;
var iframeId = document.getElementById('upload_iframe_video');
var iframe = document.createElement('iframe');
var videoUploadOkCancelClick = false;
var videoUploadClick = false;

var demoAudio = `\
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
</div>`

CKEDITOR.plugins.add('video', {
    lang: 'en', // %REMOVE_LINE_CORE%
    icons: 'videoupload',
    requires: 'dialog',
    hidpi: true, // %REMOVE_LINE_CORE%
    init: function (editor) {
        window.editVideo = false;
        editor.addCommand('insertVideo', new CKEDITOR.dialogCommand('insertVideo'));

        editor.ui.addButton('VideoUpload', {
            label: 'Upload Video/Audio Content',
            command: 'insertVideo',
            icon: this.path + 'icons/icon.png',
            toolbar: 'insertVideo,26'
		});

        editor.widgets.add('videoupload', {
            inline: true,
            mask: true
        });

        editor.on('doubleclick', function (evt) {
            var ele = evt.data.element;
            if (ele.hasClass('editvideo')) {
                //Move selection to parent
                editor.getSelection().selectElement(ele.getParent());

                evt.data.dialog = 'insertVideo';
                eleVideo = ele;
                //The status to editor know this is update
                editVideo = true;
            }
            if (ele.hasClass('audio-mask')) {
              //Move selection to parent
              var parent = ele.getParent()
              editor.getSelection().selectElement(parent);
              eleAudio = parent.find('audio').getItem(0)
              if(eleAudio){
                evt.data.dialog = 'insertVideo';
                //The status to editor know this is update
                editAudio = true;
              }
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

        CKEDITOR.dialog.add('insertVideo', function (editor) {
            var checkChanged = false;
            var getImgByVersion = CKEDITOR.plugins.getImgByVersion;
            var myhtml = '';

            myhtml += '<div class="divDrawTool" id="wrap_' + editor.name + '_dialog">';
            myhtml += '    <div class="group_inputs">';
            myhtml += '        <div class="fleft">Width: <input type="text" id="video_width" class="drawWidth" value="320" /></div>';
            myhtml += '        <div class="fleft divDrawHeight heightInput">Height: <input type="text" id="video_height" class="drawHeight" value="240" /></div>';
            myhtml += '        <div class="fleft divDrawHeight" style="padding-top: 5px;">Float: <select style="width : 100px" aria-labelledby="cke_273_label" id="floatVideo" class="cke_dialog_ui_input_select selectAlignment"><option value="">None</option><option value="float: left;">Left</option><option value="float: right;">Right</option></select></div>';
            myhtml += '        <div class="clear10"></div>  ';
            myhtml += '    </div>';
            myhtml += '    <div>';
            myhtml += '        <div class="formDrawTool">';
            myhtml += '         <form name="form-upload" style="display: inline-block;" class="UploadDrawTool" id="formVideoUpload" lang="en" dir="ltr" method="POST" enctype="multipart/form-data">';
            myhtml += '             <input type="file" size="38" name="file" id="formVideoUploadFile" aria-labelledby="cke_262_label" class="uploadDrawTool" style="height: 21px;border: solid 1px #cccccc;" />';
            myhtml += '             <input type="hidden" name="id" id="objectId" />';
            myhtml += '         </form>';
            myhtml += '         <a class="btnRemoveImage cke_dialog_ui_fileButton cke_dialog_ui_button" role="button" hidefocus="true" title="Upload" href="javascript:void(0)" style="-moz-user-select: none;vertical-align: top;margin-left: 10px;"><span class="cke_dialog_ui_button">Remove</span></a>';
            myhtml += '         <a id="uploadVideoButton" class="uploadImageDrawTool cke_dialog_ui_fileButton cke_dialog_ui_button" role="button" hidefocus="true" title="Upload" href="#" style="-moz-user-select: none;vertical-align: top;margin-left: 10px;"><span class="cke_dialog_ui_button">Upload</span></a>';
            myhtml += '        </div>';
            myhtml += '        <div id="videoHoder" class="txtHoder">';
            myhtml += '        <video id="my_video" class="my_video" width="" height="" controls autoplay preload="metadata" oncontextmenu="return false">';
            myhtml += '         <source src="" type="">';
            myhtml += '         <p>Your browser does not support HTML5 video.</p>';
            myhtml += '        </video>';
            myhtml += '    </div>';
            myhtml += demoAudio;
            myhtml += '    <div class="clear10"></div>  ';
            myhtml += '</div>';
            myhtml = myhtml.replace('{{audio_play}}', getImgByVersion('multiplechoice', 'images/small_audio_play.png'))
            .replace('{{audio_stop}}', getImgByVersion('multiplechoice', 'images/small_audio_stop.png'));
            return {
                title: 'Video/Audio Upload',
                minWidth: IS_V2 ? 600 : 400,
                minHeight: 100,
                contents:
                [
                    {
                        id: 'videoUploadExe',
                        label: 'Settings',
                        elements:
                        [
                            {
                                type: 'html',
                                html: myhtml,
                                onLoad: function () {
                                    $ckEditorDialog = $('.cke_editor_' + editor.name + '_dialog');

                                    // Initilize up and down number width of video
                                    getUpDownNumber($ckEditorDialog.find('.drawWidth'), 0, 600);

                                    // Initialize up and down number height of video
                                    getUpDownNumber($ckEditorDialog.find('.drawHeight'), 0, 1000);

                                    // Wrapper video
                                    $ckEditorDialog.find('.txtHoder').css({
                                        'width': $ckEditorDialog.find('.drawWidth').val(),
                                        'height': $ckEditorDialog.find('.drawHeight').val(),
                                        'margin': '0 auto',
                                        'background': 'gainsboro',
                                        'margin-top': '10px'
                                    });

                                    // Initialize width and height of video
                                    $ckEditorDialog.find('.my_video').css({
                                        'width': $ckEditorDialog.find('.drawWidth').val(),
                                        'height': $ckEditorDialog.find('.drawHeight').val(),
                                        'display': 'none'
                                    });

                                    $ckEditorDialog.find('.btnRemoveImage ').css('visibility', 'hidden');

                                    // Upload video button
                                    $ckEditorDialog.find('#uploadVideoButton').click(function (event) {
                                        var lenCkEditor = $ckEditorDialog.find('#formVideoUploadFile').length;
                                        var file = $ckEditorDialog.find('#formVideoUploadFile').get(lenCkEditor - 1).value;
                                        var isCorrectFormat = false;
                                        var extension;
                                        var allowFileUpload = ['mp4','mp3']
                                        // Refresh config video
                                        refeshConfig();
                                        videoUploadClick = true;

                                        if (file === '') {
                                            customAlert('Please select video/audio file.');
                                            return;
                                        }

                                        extension = file.substr((file.lastIndexOf('.') + 1));
                                        extensionFile = extension.toLowerCase() || '';

                                        if (allowFileUpload.includes(extensionFile)) {
                                            isCorrectFormat = true;
                                        }

                                        if (!isCorrectFormat) {
                                          customAlert('Unsupported file type. Please select mp4/mp3 file.');
                                          return;
                                        }


                                        $ckEditorDialog.find('#objectId').val(objectId);
                                        if(extensionFile === 'mp4') {
                                          $ckEditorDialog.find('#video_height').val(240)
                                          videoUpload($ckEditorDialog.find('#formVideoUpload').get(0), videoConfig);
                                        } else if (extensionFile === 'mp3'){
                                          $ckEditorDialog.find('#video_height').val(54)
                                          audioUpload($ckEditorDialog.find('#formVideoUpload').get(0), audioConfig);
                                        }
                                        return false;
                                    });

                                    // Removevideo
                                    $ckEditorDialog.find('.btnRemoveImage ').on('click', function () {
                                        $(this).css('visibility', 'hidden');
                                        $ckEditorDialog.find('#my_video').css('display', 'none');
                                        $ckEditorDialog.find('#my_video source').attr('src', '');
                                        $ckEditorDialog.find('#formVideoUploadFile').val('');
                                        $ckEditorDialog.find('.UploadDrawTool').get(0).value = '';
                                        $ckEditorDialog.find("#audioQuestionDemo .audioRef").empty();
                                    });
                                    $ckEditorDialog.find("#audioQuestionDemo .bntPlay").click(playVNSAudio);
                                    $ckEditorDialog.find("#audioQuestionDemo .bntStop").click(stopVNSAudio);
                                },
                                onShow: function () {
                                    $ckEditorDialog = $('.cke_editor_' + editor.name + '_dialog');

                                    // Reset video
                                    resetHtmlVideo();

                                    // Load edit video
                                    if (editVideo) {
                                        var eleVideoAttr = eleVideo.$.attributes;
                                        var videoWidth = eleVideoAttr.width.value;
                                        var videoHeight = eleVideoAttr.height.value;
                                        var videoFile = eleVideo.$.currentSrc;
                                        var extensionVideo = videoFile.substr((videoFile.lastIndexOf('.') + 1));
                                        var buildVideo = '';
                                        var isStyle = eleVideoAttr.style;

                                        isStyle = isStyle !== undefined ? isStyle.value : '';

                                        if (isStyle.indexOf('float: left') != -1) {
                                            $ckEditorDialog.find('#floatVideo').val('float: left;');
                                        } else if (isStyle.indexOf('float: right') != -1) {
                                            $ckEditorDialog.find('#floatVideo').val('float: right;');
                                        }

                                        $ckEditorDialog.find('.drawWidth,.drawHeight').css('background', '#fff');
                                        $ckEditorDialog.find('.drawWidth,.drawHeight,.ckUDNumber').prop('disabled', false);
                                        $ckEditorDialog.find('#video_width').attr('value', videoWidth);
                                        $ckEditorDialog.find('#video_height').attr('value', videoHeight);
                                        $ckEditorDialog.find('.group_inputs').css('display', 'block');
                                        $ckEditorDialog.find('#videoHoder').css({
                                            'height': videoHeight + 'px',
                                            'width': videoWidth + 'px'
                                        });

                                        $ckEditorDialog.find('#videoHoder').empty();

                                        //Insert Video to Editor
                                        buildVideo += '<video id="my_video" class="editvideo" width="' + videoWidth + '" height="' + videoHeight + '" controls autoplay preload="metadata" oncontextmenu="return false">';
                                        buildVideo += '<source src="' + videoFile + '" type="video/' + extensionVideo + '" />';
                                        buildVideo += 'Your browser does not support the video tag.';
                                        buildVideo += '</video>';

                                        $ckEditorDialog.find('#videoHoder').html(buildVideo);

                                        if (videoFile === '') {
                                            $ckEditorDialog.find('#my_video').hide();
                                        } else {
                                            $ckEditorDialog.find('#my_video').css({
                                                'width': videoWidth + 'px',
                                                'height': videoHeight + 'px'
                                            });
                                            $ckEditorDialog.find('#my_video').show();
                                            $ckEditorDialog.find('.btnRemoveImage ').css('visibility', 'visible');
                                        }
                                    }

                                    // Load edit audio
                                    if (editAudio) {
                                      var eleAudioAttr = eleAudio.$.attributes;

                                      var audioFile = eleAudio.$.currentSrc;
                                      var isStyle = eleAudioAttr.style;

                                      isStyle = isStyle !== undefined ? isStyle.value : '';

                                      if (isStyle.indexOf('float: left') != -1) {
                                          $ckEditorDialog.find('#floatVideo').val('float: left;');
                                      } else if (isStyle.indexOf('float: right') != -1) {
                                          $ckEditorDialog.find('#floatVideo').val('float: right;');
                                      }
                                      var heightMatch = isStyle.match(/height:\s*([\d.]+)(px|%|em|rem)?/);
                                      var audioHeight = heightMatch ? heightMatch[1] : 54;

                                      var widthMatch = isStyle.match(/width:\s*([\d.]+)(px|%|em|rem)?/);
                                      var audioWidth = widthMatch ? widthMatch[1]  : 320 ;

                                      $ckEditorDialog.find('.drawWidth,.drawHeight').css('background', '#fff');
                                      $ckEditorDialog.find('.drawWidth,.ckUDNumber').prop('disabled', false);
                                      $ckEditorDialog.find('.divDrawHeight.heightInput').hide();
                                      $ckEditorDialog.find('#video_width').attr('value', audioWidth);
                                      $ckEditorDialog.find('#video_height').attr('value', audioHeight);
                                      $ckEditorDialog.find('#video_width').val(audioWidth);
                                      $ckEditorDialog.find('#video_height').val(audioHeight);
                                      $ckEditorDialog.find('.group_inputs').css('display', 'block');
                                      $ckEditorDialog.find('#videoHoder').hide()
                                      //Insert Audio to Editor
                                      $ckEditorDialog.find("#audioQuestionDemo .audioRef").html(audioFile);
                                      $ckEditorDialog.find("#demoAudio").show();
                                      $ckEditorDialog.find('#audioQuestionDemo .bntStop').hide().end().find('#audioQuestionDemo .bntPlay').show();
                                      stopVNSAudio();

                                      if (audioFile === '') {
                                          $ckEditorDialog.find('#my_video').hide();
                                      } else {
                                          $ckEditorDialog.find('#my_video').show();
                                          $ckEditorDialog.find('.btnRemoveImage ').css('visibility', 'visible');
                                      }
                                  }
                                }
                            }
                        ]
                    }
                ],
                onOk: function () {
                  var lenCkEditor = $ckEditorDialog.find('#formVideoUploadFile').length;
                  var file = $ckEditorDialog.find('#formVideoUploadFile').get(lenCkEditor - 1).value;
                  var extension = file.substr((file.lastIndexOf('.') + 1));
                  var extensionFile = extension.toLowerCase() || '';
                  switch (extensionFile) {
                    case 'mp4':
                      insertVideoToCkEditor(editor);
                      break;
                    case 'mp3':
                      insertAudioToCkEditor(editor);
                      break;
                    default:
                      if (extensionFile === '') {
                        if (editVideo) insertVideoToCkEditor(editor);
                        if (editAudio) insertAudioToCkEditor(editor);
                      }
                      break;
                  }
                },
                onCancel: function() {
                    $ckEditorDialog = $('.cke_editor_' + editor.name + '_dialog');
                    stopVNSAudio();
                    videoUploadOkCancelClick = true;
                    editVideo = false;
                    editAudio = false;
                    $ckEditorDialog.find('#my_video').trigger('pause');

                    if (checkChanged) {
                        var sure = confirm('You have changed the video/audio. Are you sure you want to close?');
                        if (sure) {
                            checkChanged = false;
                        } else {
                            return sure;
                        }
                    }

                }
            };
        });
    }
});

function resetHtmlVideo() {
    $(this).css('visibility', 'hidden');

    // Reset up down width and height of video
    $ckEditorDialog.find('.drawWidth').val('320');
    $ckEditorDialog.find('.drawHeight').val('240');

    // Reset video
    $ckEditorDialog.find('#my_video')
        .attr({
            'width': '320',
            'height': '240'
        })
        .css({
            'width': '320px',
            'height': '240p',
            'display': 'none'
        });

    $ckEditorDialog.find('#my_video source').attr('src', '');

    // Reset form video
    $ckEditorDialog.find('#formVideoUploadFile').val('');

    $ckEditorDialog.find('#videoHoder').css({
        'height': '240px',
        'width': '320px'
    });

    $ckEditorDialog.find('.btnRemoveImage').css('visibility', 'hidden');
    $ckEditorDialog.find('.drawWidth, .drawHeight').css('background', '#eee');
    $ckEditorDialog.find('.drawWidth, .drawHeight, .ckUDNumber').prop('disabled', true);
    $ckEditorDialog.find('.group_inputs').css('display', 'none');
    $ckEditorDialog.find("#demoAudio").hide();
    $ckEditorDialog.find("#videoHoder").show();
    $ckEditorDialog.find("#audioQuestionDemo .audioRef").empty();
}

function videoUpload(form, action_url) {
    $ckEditorDialog.find("#demoAudio").hide();
    $ckEditorDialog.find("#videoHoder").show();
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
    iframe.setAttribute("id", "upload_iframe_video");
    iframe.setAttribute("name", "upload_iframe_video");
    iframe.setAttribute("width", "0");
    iframe.setAttribute("height", "0");
    iframe.setAttribute("border", "0");
    iframe.setAttribute("style", "width: 0; height: 0; border: none;");

    // Add to document...
    form.parentNode.appendChild(iframe);
    //window.frames['upload_iframe'].name = "upload_iframe";
    iframeId = document.getElementById("upload_iframe_video");

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

        var myVid = $ckEditorDialog.find('#my_video');
        $ckEditorDialog.find('#my_video source').attr({
            'src': data.ReturnValue,
            'type': 'video/' + extensionFile
        }).show();

        myVid.load();
        myVid.autoplay = true;
        myVid.get(0).addEventListener('loadeddata', function() {
            $ckEditorDialog.find('.group_inputs').css('display', 'block');
            $ckEditorDialog.find('.drawWidth, .drawHeight').css('background', '#fff');
            $ckEditorDialog.find('.drawWidth, .drawHeight, .ckUDNumber').prop('disabled', false);
         }, false);

        $ckEditorDialog.find('#my_video').css('display', 'block');
        $ckEditorDialog.find('.btnRemoveImage ').css('visibility', 'visible');

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
    form.setAttribute("target", "upload_iframe_video");
    form.setAttribute("action", action_url);
    form.setAttribute("method", "post");
    form.setAttribute("enctype", "multipart/form-data");
    form.setAttribute("encoding", "multipart/form-data");
    // Submit the form...
    form.submit();
}

function removeFrame() {
    //Check iFrame and only remove iframe has existed
    if (iframeId !== null && iframeId.parentNode !== null) {
        iframeId.parentNode.removeChild(iframeId);
    }
}

function audioUpload(form, action_url) {
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
      $ckEditorDialog.find("#audioQuestionDemo .audioRef").html(data.absoluteUrl);
      $ckEditorDialog.find("#videoHoder").hide();
      $ckEditorDialog.find("#demoAudio").show();
      $ckEditorDialog.find('#audioQuestionDemo .bntStop').hide().end().find('#audioQuestionDemo .bntPlay').show();
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
      $ckEditorDialog.find('.btnRemoveImage ').css('visibility', 'visible');
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

function insertVideoToCkEditor(editor) {
  $ckEditorDialog = $('.cke_editor_' + editor.name + '_dialog');
  var videoWidth;
  var videoHeight;
  var videoSrc;
  var videoFloat;
  var htmlVideo = '';

  if ($ckEditorDialog.length) {
      $ckEditorDialog.each(function(ind, dialog) {
          var $dialog = $(dialog);

          if ($dialog.is(':visible')) {
              $ckEditorDialog = $dialog;
          }
      });
  }

  videoWidth = $ckEditorDialog.find('#video_width').val();
  videoHeight = $ckEditorDialog.find('#video_height').val();
  videoFloat = $ckEditorDialog.find('#floatVideo').val();
  videoSrc = $ckEditorDialog.find('#my_video source').attr('src');

  videoUploadOkCancelClick = true;
  $ckEditorDialog.find('#my_video').trigger('pause');

  // Check source if empty will be return
  if (videoSrc === '') {
      customAlert('Please upload video!');
      return false;
  }

  // Get extension of file base on url
  extensionFile = videoSrc.substr((videoSrc.lastIndexOf('.') + 1));

  // Insert Video to Editor //preload="metadata" contenteditable="true"
  htmlVideo += '<span contenteditable="false" class="videoSpan">';
  htmlVideo += '<video class="editvideo" width="' + videoWidth + '" height="' + videoHeight + '" style="' + videoFloat + '" controls autoplay preload="metadata" oncontextmenu="return false">';
  htmlVideo += '<source src="' + videoSrc + '" type="video/' + extensionFile + '" />';
  htmlVideo += 'Your browser does not support the video tag.';
  htmlVideo += '</video></span>&nbsp;';

  if (hasGuidance && editVideo) {
      var newHtmlVideo = '';

      // Get new html video
      newHtmlVideo += '<video class="editvideo" width="' + videoWidth + '" height="' + videoHeight + '" style="' + videoFloat + '" controls autoplay preload="metadata" oncontextmenu="return false">';
      newHtmlVideo += '<source src="' + videoSrc + '" type="video/' + extensionFile + '" />';
      newHtmlVideo += 'Your browser does not support the video tag.';
      newHtmlVideo += '</video>';

      $(eleVideo.$).replaceWith(newHtmlVideo);
  } else {
      if (CKEDITOR.env.safari) {
          CKEDITOR.instances[editor.name].insertHtml('&nbsp;&nbsp;&nbsp;' + htmlVideo + ' ');
      } else {
          // Remove sign &#8203
          CKEDITOR.instances[editor.name].insertHtml('&nbsp;' + htmlVideo + ' ');
      }
  }

  checkChanged = false;
  editVideo = false;

  resetHtmlVideo();
  var myVid = $(editor.window.getFrame().$).contents().find('video');
  myVid.attr('oncontextmenu', 'return false');
  myVid.trigger('pause');
}

function insertAudioToCkEditor(editor) {
  stopVNSAudio();
  $ckEditorDialog = $('.cke_editor_' + editor.name + '_dialog');
  var audioWidth;
  var audioHeight;
  var audioSrc;
  var audioFloat;
  var htmlAudio = '';

  if ($ckEditorDialog.length) {
      $ckEditorDialog.each(function(ind, dialog) {
          var $dialog = $(dialog);

          if ($dialog.is(':visible')) {
              $ckEditorDialog = $dialog;
          }
      });
  }

  audioWidth = $ckEditorDialog.find('#video_width').val();
  audioHeight = $ckEditorDialog.find('#video_height').val();
  audioFloat = $ckEditorDialog.find('#floatVideo').val();
  audioSrc = $ckEditorDialog.find("#audioQuestionDemo .audioRef").html();

  videoUploadOkCancelClick = true;
  $ckEditorDialog.find('#my_video').trigger('pause');

  // Check source if empty will be return
  if (audioSrc === '') {
      customAlert('Please upload audio!');
      return false;
  }

  // Get extension of file base on url
  extensionFile = audioSrc.substr((audioSrc.lastIndexOf('.') + 1));

  // Insert Video to Editor //preload="metadata" contenteditable="true"
  htmlAudio += '<div contenteditable="false" class="videoSpan audio-container">';
  htmlAudio += '<audio class="editvideo"' + 'style="width: {{audioWidth}}px;height: {{audioHeight}}px' + audioFloat + '" controls preload="metadata" oncontextmenu="return false">';
  htmlAudio += '<source src="' + audioSrc + '" type="audio/' + extensionFile + '" />';
  htmlAudio += '</audio>';
  htmlAudio += '<div class="audio-mask top"></div>';
  htmlAudio += '<div class="audio-mask bottom"></div>';
  htmlAudio += '<div class="audio-mask left"></div>';
  htmlAudio += '<div class="audio-mask right"></div>';
  htmlAudio += '</div>&nbsp;';
  htmlAudio = htmlAudio.replace('{{audioWidth}}', audioWidth).replace('{{audioHeight}}', audioHeight)
  if (hasGuidance && editAudio) {
      $(eleVideo.$).replaceWith(htmlAudio);
  } else {
      if (CKEDITOR.env.safari) {
          CKEDITOR.instances[editor.name].insertHtml('&nbsp;&nbsp;&nbsp;' + htmlAudio + ' ');
      } else {
          // Remove sign &#8203
          CKEDITOR.instances[editor.name].insertHtml('&nbsp;' + htmlAudio + ' ');
      }
  }

  checkChanged = false;
  editAudio = false;

  resetHtmlVideo();
  var myVid = $(editor.window.getFrame().$).contents().find('video');
  myVid.attr('oncontextmenu', 'return false');
  myVid.trigger('pause');
}
