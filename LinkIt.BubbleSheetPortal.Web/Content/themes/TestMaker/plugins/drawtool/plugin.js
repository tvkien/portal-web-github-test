CKEDITOR.plugins.add('drawtool', {
  lang: 'en', // %REMOVE_LINE_CORE%
  icons: 'drawtool',
  requires: 'dialog',
  hidpi: true, // %REMOVE_LINE_CORE%
  init: function(editor) {
    var pluginName = 'insertDrawTool';

    editor.addCommand(pluginName, new CKEDITOR.dialogCommand(pluginName));

    editor.ui.addButton('DrawTool', {
      label: 'Drawing Interaction',
      command: pluginName,
      icon: this.path + 'icons/drawtool.png',
      toolbar: 'insertDrawTool,30'
    });

    editor.widgets.add('drawtool', {
      inline: true,
      mask: true,
      allowedContent: {
        p: {
          classes: 'drawTool',
          attributes: '!id,name,contenteditable'
        }
      },
      template: '<p class="drawTool"></p>'
    });

    var isEditDrawTool = false;
    var currentDrawToolResId = "";
    var eleDrawTool = "";
    var iframe, iframeId;

    function getDrawingResponseId(element) {
      var drawingResposeId = 0;

      for (var i = 0, len = iResult.length; i < len; i++) {
        var iResultItem = iResult[i];

        if (iResultItem.responseIdentifier == element.getId() && iResultItem.type == 'extendedTextInteractionDraw') {
          drawingResposeId = iResultItem.responseIdentifier;
          break;
        }
      }

      return drawingResposeId;
    }

    function loadDataforDrawTool(element) {
      var $element = $(element.$);
      var $dialog = $('.divDrawTool');
      var $dialogDrawHoder = $dialog.find('#drawHoder');
      var $dialogPoint = $dialog.find('.point');
      var $dialogDrawWidth = $dialog.find('#drawWidth');
      var $dialogDrawHeight = $dialog.find('#drawHeight');
      var $dialogSelectPercent = $dialog.find('#selectPercent');
      var $dialogSelectGridSize = $dialog.find('#selectGridSize');
      var $dialogImageDrawTool = $dialog.find('#imageDrawTool');
      var $dialogBtnRemoveImage = $dialog.find('.btnRemoveImage');
      var $dialogIdPercent = $dialog.find('#idPercent');
      var $dialogTxtImagePart = $dialog.find('.txt-image-part');
      var $dialogIdGridSize = $dialog.find('#idGridSize');
      var $dialogDrawingDimension = $dialog.find('#drawingDimension');
      var $dialogDrawingDataWidth = $dialog.find('#drawingDataWidth');
      var $dialogDrawingDataHeight = $dialog.find('#drawingDataHeight');
      var $dialogTextToSpeech = $dialog.find('#draw-tool-text-to-speech');
      var $dialogDrawingBasic = $dialog.find('#drawing-basic');
      var $dialogDrawingFreeFormatted = $dialog.find('#drawing-free-formatted');

      for (var i = 0, len = iResult.length; i < len; i++) {
        var iResultItem = iResult[i];

        if (iResultItem.responseIdentifier == element.getId() && iResultItem.type == "extendedTextInteractionDraw") {
          $dialogPoint.val(iResultItem.responseDeclaration.pointsValue);
          $dialogDrawWidth.val(iResultItem.width);
          $dialogDrawHeight.val(iResultItem.height);
          $dialogSelectPercent.val(iResultItem.percent);
          $dialogSelectGridSize.val(iResultItem.gridSize);
          $dialogImageDrawTool.val(iResultItem.percent).attr('src', iResultItem.srcImage)
          updateDimensionDrawHoder($dialogDrawHoder, iResultItem.width, iResultItem.height);

          if (!!iResultItem.srcImage && getImageDrawTool(iResultItem.srcImage)) {
            $dialogBtnRemoveImage.css('visibility', 'visible');
            $dialogDrawWidth.prop('disabled', false);
            $dialogDrawHeight.prop('disabled', false);
            $dialogIdPercent.show();

            var img = new Image();

            img.onload = function() {
              $dialogImageDrawTool
                .attr('worgimage', iResultItem.wOrgImage)
                .attr('horgimage', iResultItem.hOrgImage)
                .css({
                  'width': iResultItem.width + 'px',
                  'height': iResultItem.height + 'px'
                })
                .show();

              $dialogTxtImagePart.show().css('display', 'block');
            };

            img.src = iResultItem.srcImage;
          } else {
            $dialogImageDrawTool.hide();
            $dialogDrawWidth.prop('disabled', true);
            $dialogDrawHeight.prop('disabled', true);
            $dialogIdPercent.hide();
            $dialogBtnRemoveImage.css('visibility', 'hidden');
          }

          if (iResultItem.dataType === 'basic') {
            $dialogDrawingBasic.prop('checked', true);
            $dialogDrawingDimension.show();
            $dialogIdGridSize.show();
          } else {
            $dialogDrawingFreeFormatted.prop('checked', true);
            $dialogDrawingDimension.hide();
            $dialogIdGridSize.hide();
          }

          iResultItem.dataWidth = iResultItem.dataWidth != null ? iResultItem.dataWidth : 600;
          iResultItem.dataHeight = iResultItem.dataHeight != null ? iResultItem.dataHeight : 600;
          $dialogDrawingDataWidth.val(iResultItem.dataWidth);
          $dialogDrawingDataHeight.val(iResultItem.dataHeight);

          var texttospeechVal = $element.find('img[percent]').attr('texttospeech');
          texttospeechVal = texttospeechVal == null ? '' : texttospeechVal;
          $dialogTextToSpeech.val(texttospeechVal);

          break;
        }
      }
    }

    function resetDrawTool(dialog) {
      dialog.find('#drawWidth').val('400').prop('disabled', true);
      dialog.find('#drawHeight').val('300').prop('disabled', true);
      dialog.find('#point').val('1');
      dialog.find('#drawHoder').css({
        'width': '400px',
        'height': '300px'
      });
      dialog.find('#imageDrawTool').attr('src', '').hide();
      dialog.find('#imageDrawTool').get(0).value = '';
      dialog.find('.btnRemoveImage ').css('visibility', 'hidden');
      dialog.find('#selectPercent').val(10);
      dialog.find('#selectGridSize').val('x1');
      dialog.find('#idPercent').hide();
      dialog.find('#drawHoder').find('.divContent').show();
      dialog.find('#draw-tool-text-to-speech:visible').val('');
      dialog.find('.txt-image-part').hide();
      dialog.find('#drawing-free-formatted').prop('checked', true);
      dialog.find('#drawingDimension').hide();
      dialog.find('#idGridSize').hide();
      dialog.find('#drawingDataWidth').val(600);
      dialog.find('#drawingDataHeight').val(600);
    }

    function imageUploadDrawTool(form, selectPercent, action_url) {
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
      iframe.setAttribute("id", "upload_iframe_drawtool");
      iframe.setAttribute("name", "upload_iframe_drawtool");
      iframe.setAttribute("width", "0");
      iframe.setAttribute("height", "0");
      iframe.setAttribute("border", "0");
      iframe.setAttribute("style", "width: 0; height: 0; border: none;");

      // Add to document...
      form.parentNode.appendChild(iframe);

      iframeId = document.getElementById("upload_iframe_drawtool");

      // Add event...
      var eventHandler = function() {

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

        //This is upload for destination
        var percentFirst = selectPercent.val();
        var imageDrawTool = $('#imageDrawTool');
        var drawHoder = $('#drawHoder');

        imageDrawTool.attr({
          "src": data.url,
          'percent': percentFirst
        }).show();
        drawHoder.find('.divContent').hide();

        $('#idPercent').show();
        $('#selectPercent').val(10);

        // Create new offscreen image to test
        var img = new Image();

        img.onload = function() {
          imageDrawTool
            .width(this.width)
            .height(this.height)
            .attr('worgimage', this.width)
            .attr('horgimage', this.height);

          updateDimensionDrawHoder(drawHoder, this.width, this.height);

          $('.btnRemoveImage ').css('visibility', 'visible');
          $('.txt-image-part').show();

          //hide overlay
          if (IS_V2) {
            $('body').unblock();
          } else {
            $("body").ckOverlay.destroy();
          }
        };

        img.src = imageDrawTool.attr("src");
      };

      if (iframeId.addEventListener) iframeId.addEventListener("load", eventHandler, true);
      if (iframeId.attachEvent) iframeId.attachEvent("onload", eventHandler);

      // Set properties of form...
      form.setAttribute("target", "upload_iframe_drawtool");
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

    function getUpDownNumber(selector, min, max) {
      var $selector = $(selector);

      $selector.ckUpDownNumber({
        minNumber: min,
        maxNumber: max,
        width: 18,
        height: 13
      });
    }

    function loadUpDownNumberDrawTool(dialog) {
      getUpDownNumber(dialog.find('.point'), 0, 1000);
      getUpDownNumber(dialog.find('.drawWidth'), 0, 600);
      getUpDownNumber(dialog.find('.drawHeight'), 0, 1000);
      getUpDownNumber(dialog.find('#drawingDataWidth'), 200, 2000);
      getUpDownNumber(dialog.find('#drawingDataHeight'), 200, 2000);
    }

    function getImageDrawTool(filename) {
      return (/\.(jpg|jpeg|png)$/i).test(filename);
    }

    function updateDimensionDrawHoder(drawHoder, width, height) {
      if (width > 800) {
        drawHoder.width(width / 2).css({
          'overflow-x': 'scroll'
        });
      } else {
        drawHoder.width(width).css({
          'overflow-x': 'initial'
        });
      }

      if (height > 600) {
        drawHoder.height(height / 2).css({
          'overflow-y': 'scroll'
        });
      } else {
        drawHoder.height(height).css({
          'overflow-y': 'initial'
        });
      }
    }

    editor.on('doubleclick', function(evt) {
      var element = evt.data.element;

      if (element.hasClass('imageDrawTool')) {
        var parents = element.getParents();
        var parent;

        for (var i = 0; i < parents.length; i++) {
          parent = parents[i];
          if (parent.hasClass('drawTool')) {
            break;
          }
        }

        eleDrawTool = parent;
        editor.getSelection().selectElement(eleDrawTool);
        evt.data.dialog = pluginName;

        //The status to editor know this is update
        isEditDrawTool = true;

        currentDrawToolResId = getDrawingResponseId(eleDrawTool);

        dblickHandlerToolbar(editor);
      }
    });

    CKEDITOR.dialog.add(pluginName, function(editor) {
      var checkChanged = false;

      myhtml = '<div class="divDrawTool">';
      myhtml += '    <div class="u-clearfix" style="margin-bottom: 10px;" id="drawingType">';
      myhtml += '        <div class="fleft"><input type="radio" id="drawing-free-formatted" name="draw-type" checked="checked"/> <label for="drawing-free-formatted">Free Formatted Drawing Tool</label></div>';
      myhtml += '        <div class="fleft u-m-l-10"><input type="radio" id="drawing-basic" name="draw-type"/> <label for="drawing-basic">Graphing and Labeling Tool</label></div>';
      myhtml += '    </div>';
      myhtml += '    <div id="drawingDimension" class="u-clearfix" style="margin-bottom: 10px;">';
      myhtml += '        <div class="fleft">Drawing Width: <input type="text" id="drawingDataWidth" class="txtFullcreate" value="600" /></div>';
      myhtml += '        <div class="fleft u-m-l-10">Drawing Height: <input type="text" id="drawingDataHeight" class="txtFullcreate" value="600" /></div>';
      myhtml += '    </div>';
      myhtml += '    <div style="min-width: 465px;">';
      myhtml += '        <div class="fleft" style="display: none;">Width: <input type="text" id="drawWidth" class="drawWidth" value="400" /></div>';
      myhtml += '        <div class="fleft divDrawHeight" style="display: none;">Height: <input type="text" id="drawHeight" class="drawHeight" value="300" /></div>';
      myhtml += '        <div id="idPercent" class="fleft u-m-r-10">Percent<select style="width : 100px; height: 30px; margin-left: 10px;" aria-labelledby="cke_272_label" class="cke_dialog_ui_input_select" id="selectPercent">';
      myhtml += '             <option value="1">10%</option>';
      myhtml += '             <option value="2">20%</option>';
      myhtml += '             <option value="3">30%</option>';
      myhtml += '             <option value="4">40%</option>';
      myhtml += '             <option value="5">50%</option>';
      myhtml += '             <option value="6">60%</option>';
      myhtml += '             <option value="7">70%</option>';
      myhtml += '             <option value="8">80%</option>';
      myhtml += '             <option value="9">90%</option>';
      myhtml += '             <option selected value="10">100%</option>';
      myhtml += '            </select></div>';
      myhtml += '        <div id="idGridSize" class="fleft divDrawHeight u-p-l-0">Grid Size<select style="width : 100px; height: 30px; margin-left: 10px;" aria-labelledby="cke_272_label" class="cke_dialog_ui_input_select" id="selectGridSize">';
      myhtml += '             <option selected value="x1">x1</option>';
      myhtml += '             <option value="x2">x2</option>';
      myhtml += '             <option value="x3">x3</option>';
      myhtml += '             <option value="x4">x4</option>';
      myhtml += '            </select></div>';
      myhtml += '        <div class="fright">Point value: <input type="text" id="point" class="point" value="1" /></div>';
      myhtml += '        <div class="clear10"></div>  ';
      myhtml += '    </div>';
      myhtml += '    <div>';
      myhtml += '        <div class="formDrawTool">';
      myhtml += '         <form name="form-upload" class="UploadDrawTool" id="ImageUploadDrawTool" lang="en" dir="ltr" method="POST" enctype="multipart/form-data">';
      myhtml += '             <input type="file" size="38" name="file" id="uploadDrawTool" aria-labelledby="cke_262_label" class="uploadDrawTool" style="height: 21px;border: solid 1px #cccccc;" />';
      myhtml += '             <input type="hidden" name="id" id="objectImageId" />';
      myhtml += '         </form>';
      myhtml += '         <a class="btnRemoveImage cke_dialog_ui_fileButton cke_dialog_ui_button" role="button" hidefocus="true" title="Upload" href="javascript:void(0)" style="-moz-user-select: none;vertical-align: top;margin-left: 10px;position: relative; left: 25px;"><span class="cke_dialog_ui_button">Remove</span></a>';
      myhtml += '         <a class="uploadImageDrawTool cke_dialog_ui_fileButton cke_dialog_ui_button" role="button" hidefocus="true" title="Upload" href="#" style="-moz-user-select: none;vertical-align: top;margin-left: 28px;"><span class="cke_dialog_ui_button">Upload</span></a>';
      myhtml += '        </div>';
      myhtml += '        <div id="drawHoder" class="txtHoder">';
      myhtml += '         <img id="imageDrawTool" style="display: none; position: relative; z-index: 9;" alt="" src="" />';
      myhtml += '         <div class="divContent">Draw Area.</div></div>';
      myhtml += '    </div>';
      myhtml += '    <div class="u-m-t-10 txt-image-part">Description of image:</div>';
      myhtml += '    <textarea id="draw-tool-text-to-speech" class="img-text-to-speech txt-image-part"></textarea>';
      myhtml += '</div>';

      return {
        title: 'Drawing Interaction Properties',
        minWidth: 400,
        minHeight: 200,
        contents: [{
          id: 'drawTool',
          label: 'Settings',
          elements: [{
            type: 'html',
            html: myhtml,
            onLoad: function(a) {
              var $dialog = $(this.getDialog().getElement().$);

              loadUpDownNumberDrawTool($dialog);

              $dialog.find('.uploadImageDrawTool').on('click', function() {
                var sPercent = $('#selectPercent');
                uploadImageClick = true;

                var imageUpload = $('.uploadDrawTool');
                if (imageUpload.get(0).value == "") {
                  customAlert("Please select image file.");
                  return;
                }

                var file = imageUpload.get(0).value;
                var extension = file.substr((file.lastIndexOf('.') + 1));
                var isCorrectFormat = false;
                var fileExtensions = ['jpg', 'jpeg', 'png' , 'svg'];
                isCorrectFormat = _.some(fileExtensions, function(ext) {
                  return ext === extension.toLowerCase();
                });

                if (!isCorrectFormat) {
                  var fileExts = fileExtensions.join(', ');
                  var fileExtsMsg = 'Unsupported file type. Please select ' + fileExts + ' file.';
                  customAlert(fileExtsMsg);
                  return;
                }

                $('#objectImageId').val(objectId);
                imageUploadDrawTool($('#ImageUploadDrawTool').get(0), sPercent, imgUpload);
                // Reload default file
                imageUpload.val('');
                return false;
              });

              $dialog.find('.btnRemoveImage ').on('click', function() {
                var drawHoder = $('#drawHoder');
                var imageDrawTool = $('#imageDrawTool');
                var idPercent = $('#idPercent');

                imageDrawTool.attr('src', '').css({
                  'width': '',
                  'height': ''
                }).hide();
                $(this).css('visibility', 'hidden');

                var imageUpload = $('.uploadDrawTool');
                imageUpload.get(0).value = "";

                drawHoder.attr('src', '').css({
                  'width': '',
                  'height': ''
                });
                drawHoder.find('.divContent').show();
                idPercent.hide();
                $(".txt-image-part").hide();
              });


              $dialog.find('#selectPercent').change(function(event) {
                /* Act on the event */
                var imageDrawTool = $('#imageDrawTool');
                var drawHoder = $('#drawHoder');
                var percentValue = parseInt($(this).val());
                var originalWidth = parseInt(imageDrawTool.attr('worgimage'), 10);
                var originalHeight = parseInt(imageDrawTool.attr('horgimage'), 10);

                if (originalWidth && originalHeight) {
                  var newWidth = originalWidth * percentValue / 10;
                  var newHeight = originalHeight * percentValue / 10;

                  imageDrawTool.width(newWidth).height(newHeight);
                  drawHoder.width(newWidth).height(newHeight);
                }

                imageDrawTool.attr('percent', percentValue);

                updateDimensionDrawHoder(drawHoder, imageDrawTool.width(), imageDrawTool.height());
              });

              $dialog.find('input[type="radio"][name="draw-type"]').on('change', function() {
                var $drawType = $(this);
                var $drawingDimension = $('#drawingDimension');
                var $idGridSize = $('#idGridSize');
                var $selectGridSize = $('#selectGridSize');

                $selectGridSize.val('x1');

                if ($drawType.attr('id') === 'drawing-basic') {
                  $drawingDimension.show();
                  $idGridSize.show();
                } else {
                  $drawingDimension.hide();
                  $idGridSize.hide();
                }
              })
            },
            onShow: function() {
              var $dialog = $(this.getDialog().getElement().$);
              if (iResult.length && isEditDrawTool) {
                loadDataforDrawTool(eleDrawTool);
              } else {
                if (!isEditDrawTool) {
                  resetDrawTool($dialog);
                }
              }

              refreshResponseId();
              checkElementRemoveIntoIResult();

              $('#tips .tool-tip-tips').css({
                'display': 'none'
              });
            }
          }]

        }],
        onOk: function() {
          var $imageDrawTool = $('#imageDrawTool');
          var sourceImage = $imageDrawTool.attr('src');
          var percent = $('#selectPercent').val();
          var gridSize = $('#selectGridSize').val();
          var drawingDataType = 'free-formatted';
          var drawingDataWidth = 600;
          var drawingDataHeight = 600;
          var drawingOriginalWidth = 400;
          var drawingOriginalHeight = 300;
          var drawingWidth = 400;
          var drawingHeight = 300;
          var drawingPoint = parseInt($(".divDrawTool .point").val(), 10);
          var isDrawingBasic = $('#drawing-basic').is(':checked');

          if (sourceImage == '') {
            percent = 10;
          } else {
            drawingOriginalWidth = parseInt($imageDrawTool.attr('worgimage'), 10);
            drawingOriginalHeight = parseInt($imageDrawTool.attr('horgimage'), 10);
            drawingWidth = parseInt($imageDrawTool.width(), 10);
            drawingHeight = parseInt($imageDrawTool.height(), 10);
          }

          var buildHTMl = '',
            responseId = '';

          if (isDrawingBasic) {
            drawingDataType = 'basic';
            drawingDataWidth = parseInt($('#drawingDataWidth').val(), 10);
            drawingDataHeight = parseInt($('#drawingDataHeight').val(), 10);
          }

          if (isEditDrawTool) {
            //Update for current textEntry
            for (var n = 0, len = iResult.length; n < len; n++) {
              var iResultItem = iResult[n];
              if (iResultItem.responseIdentifier == currentDrawToolResId) {
                responseId = iResultItem.responseIdentifier;
                iResultItem.responseDeclaration.pointsValue = drawingPoint;
                iResultItem.wOrgImage = drawingOriginalWidth;
                iResultItem.hOrgImage = drawingOriginalHeight;
                iResultItem.width = drawingWidth;
                iResultItem.height = drawingHeight;
                iResultItem.srcImage = sourceImage;
                iResultItem.percent = percent || 10;
                iResultItem.gridSize = gridSize;
                iResultItem.dataType = drawingDataType;
                iResultItem.dataWidth = drawingDataWidth;
                iResultItem.dataHeight = drawingDataHeight;
                break;
              }
            }
          } else {
            //Create response identify and make sure it doesn't conflict with current.
            responseId = createResponseId();

            //add data
            iResult.push({
              type: "extendedTextInteractionDraw",
              responseIdentifier: responseId,
              wOrgImage: drawingOriginalWidth,
              hOrgImage: drawingOriginalHeight,
              width: drawingWidth,
              height: drawingHeight,
              percent: percent,
              gridSize: gridSize,
              srcImage: sourceImage,
              responseDeclaration: {
                baseType: "string",
                cardinality: "single",
                method: "default",
                caseSensitive: "false",
                pointsValue: drawingPoint,
                type: "string"
              },
              drawable: true,
              dataType: drawingDataType,
              dataWidth: drawingDataWidth,
              dataHeight: drawingDataHeight
            });

            //Hide button on toolbar after add drawtool incase qtItem is 10d
            if (iSchemeID == "10d") {
              $(".cke_button__drawtool").parents("span.cke_toolbar").hide();
            }
          }

          var texttospeech = $('#draw-tool-text-to-speech:visible').val();

          texttospeech = convertTexttoHTML(texttospeech);

          //build html for draw tool
          if (sourceImage != '') {
            buildHTMl += '<img percent="' + percent + '" data-cke-saved-src="' + sourceImage + '" src="' + sourceImage + '" width="' + drawingWidth + '" height="' + drawingHeight + '" alt="" class="imageDrawTool" style="position: absolute; border: none;" texttospeech="' + texttospeech + '"/>';
          }

          //add html into ckEditor
          var drawtoolHtml = '';

          if (isDrawingBasic) {
            drawtoolHtml = '<p gridSize="' + gridSize + '" id="' + responseId + '" title="' + responseId + '" style="width:' + drawingWidth + 'px; height: ' + drawingHeight + 'px" class="drawTool" contenteditable="false"><img style="width:' + drawingWidth + 'px; height: ' + drawingHeight + 'px" class="cke_reset cke_widget_mask imageDrawTool" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" />' + buildHTMl + '&nbsp;</p>&nbsp;';
          } else {
            drawtoolHtml = '<p id="' + responseId + '" title="' + responseId + '" style="width:' + drawingWidth + 'px; height: ' + drawingHeight + 'px" class="drawTool" contenteditable="false"><img style="width:' + drawingWidth + 'px; height: ' + drawingHeight + 'px" class="cke_reset cke_widget_mask imageDrawTool" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" />' + buildHTMl + '&nbsp;</p>&nbsp;';
          }

          var drawtoolElement = CKEDITOR.dom.element.createFromHtml(drawtoolHtml);
          editor.insertElement(drawtoolElement);

          //Reset to default after update or create new textEntry
          $('#uploadDrawTool').val('');
          isEditDrawTool = false;
          currentDrawToolResId = "";

          ResIdElemModul = responseId;
          newResult = iResult;
        },
        onCancel: function() {
          //Reset to default after update or create new textEntry
          isEditDrawTool = false;
          currentDrawToolResId = "";
        }
      };
    });
  }
});
