CKEDITOR.plugins.add('inlinechoice', {
  lang: 'en', // %REMOVE_LINE_CORE%
  icons: 'inlinechoice',
  requires: 'dialog',
  hidpi: true, // %REMOVE_LINE_CORE%
  init: function(editor) {
    var pluginName = 'insertInlineChoice';

    editor.addCommand(pluginName, new CKEDITOR.dialogCommand(pluginName));

    editor.ui.addButton('InlineChoice', {
      label: 'Inline Choice',
      command: pluginName,
      icon: this.path + 'icons/inlinechoice.png',
      toolbar: 'insertInlineChoice,30'
    });

    editor.widgets.add('inlinechoice', {
      inline: true,
      mask: true,
      allowedContent: {
        span: {
          classes: 'inlineChoiceInteraction',
          attributes: '!id,name,contenteditable'
        }
      },
      template: '<span class="inlineChoiceInteraction"></span>'
    });

    var isEditInlineChoice = false,
      currentInlineChoiceResId = "",
      isAddinlineAnswer = false,
      eleInlineChoice = ""; //This use for firstime load popup
    var isOk = false;

    var resetGradingAlgorithmic = function(dialog, isShow) {
      var $dialog = $(dialog);

      $dialog.find('.point').parent().addClass('is-disabled');
      $dialog.find('input[type="radio"].choiceCheckbox').prop('disabled', true).removeAttr('checked');

      if (!isShow) {
        $dialog.find('.point').val('0');
      }
    };

    var resetGradingNormal = function(dialog) {
      var $dialog = $(dialog);

      $dialog.find('.point').parent().removeClass('is-disabled');
      $dialog.find('.point').val('1');
      $dialog.find('input[type="radio"].choiceCheckbox').prop('disabled', false);
      $dialog.find('input[type="radio"].choiceCheckbox').first().prop('checked', true);
      $dialog.find('#ic-grading-normal').prop('checked', true);
    };

    //set font size for inline choice by single click
    if (iSchemeID === '8') {
      editor.on('contentDom', function() {
        var singleclick = $(editor.window.getFrame().$).contents().find('.inlineChoiceInteraction');
        var tagBody = $(editor.window.getFrame().$).contents().find('body');

        singleclick.unbind("click").on('click', function(evt) {
          if (navigator.userAgent.indexOf('Firefox') > -1 || navigator.userAgent.indexOf('Trident') > -1) {
            $(evt.target).parent('span.inlineChoiceInteraction').addClass('typeFontSize firefox-ie');
          } else {
            $(evt.target).parent('span.inlineChoiceInteraction').addClass('typeFontSize');
          }

          if (navigator.userAgent.indexOf('Trident') > -1) {
            var tagInlineChoice = $(evt.target).parent();
            var tagFontSize = tagInlineChoice.parent();
            if ($(tagFontSize).hasClass('smallText') || $(tagFontSize).hasClass('normalText') || $(tagFontSize).hasClass('largeText') || $(tagFontSize).hasClass('veryLargeText')) {
              var eleFontSize = getStyleFontSizeInlineChoice(tagFontSize);
              $('.editorArea .cke_combo__fontsize').find('a .cke_combo_text').text(eleFontSize);
              $('.editorArea .cke_combo__fontsize').find('.cke_combo_label').text(eleFontSize);
            }
          }
        });

        $(tagBody).click(function(evt) {
          if (!$(evt.target).hasClass('inlineChoiceInteractionMark')) {
            if ($(tagBody).find('span.inlineChoiceInteraction').hasClass('typeFontSize')) {

              $(tagBody).find('span.inlineChoiceInteraction').removeClass('typeFontSize');
              if (navigator.userAgent.indexOf('Firefox') > -1 || navigator.userAgent.indexOf('Trident') > -1) {
                $(tagBody).find('span.inlineChoiceInteraction').removeClass('firefox-ie');
              }
            }
          } else {
            $(tagBody).find('span.inlineChoiceInteraction').removeClass('typeFontSize');
            if (navigator.userAgent.indexOf('Firefox') > -1 || navigator.userAgent.indexOf('Trident') > -1) {
              $(tagBody).find('span.inlineChoiceInteraction').removeClass('firefox-ie');
            }

            if (navigator.userAgent.indexOf('Firefox') > -1 || navigator.userAgent.indexOf('Trident') > -1) {
              $(evt.target).parent('span.inlineChoiceInteraction').addClass('typeFontSize firefox-ie');
            } else {
              $(evt.target).parent('span.inlineChoiceInteraction').addClass('typeFontSize');
            }

          }

        });
      });
    }

    editor.on('doubleclick', function(evt) {
      var element = evt.data.element;

      if (element.hasClass('inlineChoiceInteractionMark')) {
        var parents = element.getParents();
        var parent;

        for (var i = 0; i < parents.length; i++) {
          parent = parents[i];
          if (parent.hasClass('inlineChoiceInteraction')) {
            break;
          }
        }

        $('#inlineTop').html('');
        //Move selection to parent of multipleChoiceMark
        eleInlineChoice = parent;
        editor.getSelection().selectElement(eleInlineChoice);
        evt.data.dialog = pluginName;

        //The status to editor know this is update
        isEditInlineChoice = true;
        isEditMultipChoiceGuidance = true;

        currentInlineChoiceResId = loadDataforInlineChoice(eleInlineChoice);
        idSimpleChoicesPopup = currentInlineChoiceResId;

        classNameStyleFontInlineChoice = getStyleFontSizeInlineChoice(eleInlineChoice);
        isStyleFontInlineChoice = true;

        dblickHandlerToolbar(editor);
      }
    });


    CKEDITOR.dialog.add(pluginName, function(editor) {

      myhtml = '<div class="divInlineChoice">';
      myhtml += '    <div class="m-b-15">';
      myhtml += '         <input type="radio" name="ic-grading" id="ic-grading-normal" checked/>';
      myhtml += '         <label for="ic-grading-normal">Normal Grading</label>';
      myhtml += '         <input type="radio" name="ic-grading" id="ic-grading-algorithmic"/>';
      myhtml += '         <label for="ic-grading-algorithmic">Algorithmic Grading</label>';
      myhtml += '    </div>';
      myhtml += '    <div class="boxDiv m-b-15">';
      myhtml += '     Point value: <input type="text" id="point" class="point" value="1" />';
      myhtml += '     <span class="visible-dimension" style="margin-left: 15px"> <input type="checkbox" name="checkBoxVisibleDimension" value ="0" id="checkBoxVisibleDimension" /> Specify Visible Dimension:</span>';
      myhtml += '     <span class="dimension-width"> <input type="text" id="expWidthILC" class="expLength" value="30" /> </span>';
      myhtml += '    </div>';
      myhtml += '    <div>';
      myhtml += '         <div class="clear"></div>';
      myhtml += '         <div id="inlineTop"></div>';
      myhtml += '         <div class="clear"></div>';
      myhtml += '         <ul id="inlineChoice">';
      myhtml += '         </ul>';
      myhtml += '         <div class="clear"></div>';
      myhtml += '         <div class="addMore">';
      myhtml += '         	<input type="button" class="ckbutton" id="bntAddInlineChoice" value="Add choice" />';
      myhtml += '         </div>';
      myhtml += '         <div class="clear"></div>';
      myhtml += '         <div id="inlineBot"></div>';
      myhtml += '         <div class="clear"></div>';
      myhtml += '    </div>';
      myhtml += '    <div class="clear10"></div>  ';
      myhtml += '</div>';

      return {
        title: 'Inline Choice Properties',
        minWidth: 600,
        minHeight: 200,
        contents: [{
          id: 'inlineChoice',
          label: 'Settings',
          elements: [{
            type: 'html',
            html: myhtml,
            onLoad: function(a) {
              var $dialog = $(this.getDialog().getElement().$);

              for (var k = 0; k < 2; k++) {
                addNewIteminlineChoice();
                inlineChoiceSelection();
              }

              //Checked first item as default when create multiple item
              $("#inlineChoice .correctAnswer input[type=radio]").first().prop("checked", true);

              $("#bntAddInlineChoice").click(function() {
                isAddinlineAnswer = true;
                isAddAnswerInlineChoiceLast = false;
                isEditMultipChoiceGuidance = false;
                var editorid = addNewIteminlineChoice();
                inlineChoiceSelection();
                createinlineCKEditorMulti(editorid);

              });

              $("#checkBoxVisibleDimension").change(function () {
                let visibleDimension = $(this).prop('checked');
                if (visibleDimension) {
                  $(".dimension-width .ckUpDownNumber").removeClass("is-disabled");
                }
                else {
                  $(".dimension-width .ckUpDownNumber").addClass("is-disabled");
                }
              })

              $dialog.find('input[type="radio"][name="ic-grading"]').on('change', function() {
                var $grading = $(this);
                var gradingMethod = $grading.attr('id');

                if (gradingMethod === 'ic-grading-algorithmic') {
                  resetGradingAlgorithmic($dialog);
                } else {
                  resetGradingNormal($dialog);
                }
              });

              //load data into popup for first time if status is Edit
              if (isEditInlineChoice) {
                loadDataforInlineChoice(eleInlineChoice);
              }
            },
            onShow: function() {
              var questionQtiSchemeId = CKEDITOR.instances[ckID].config.qtiSchemeID;
              var $dialog = $(this.getElement().$);

              //Check if text entry already exist. Don't allow create new one.
              if (!isEditInlineChoice) {
                createinlineAnswer();
                isStyleFontInlineChoice = true;
                classNameStyleFontInlineChoice = 'Normal';
                resetInlineChoiceOnload();
              }
              $('#tips .tool-tip-tips').css({
                'display': 'none'
              });
              refreshResponseId();
              resetGradingNormal($dialog);

              CKEDITOR.on('instanceReady', function(ev) {

                //handle resize when show content into editor
                var tagContent = $('#inlineChoice').find('.content');
                tagContent.find('.cke_contents').resizable({
                  handles: "s"
                });

                //Show the first toolbar when popup has created
                $("#inlineTop > div").hide();
                $("#inlineTop > div:first").show();
                $('.divInlineChoice .cke_combo__fontsize').hide();

                $('ul#inlineChoice li').each(function(index, value) {
                  var that = this;
                  var ind = index + 1;
                  var ckId = $(that).attr('id');
                  var data = CKEDITOR.instances['editor' + ckId].getData();

                  $('#' + ckId).find('iframe[allowtransparency]').contents().find('body').on('keypress', function(e) {
                    $(e.target).attr('isEnter', 'true');
                  });

                  $('#' + ckId).find('iframe[allowtransparency]').contents().find('body').on('focus', function() {
                    var me = this;
                    var ans = 'Answer ' + ind;
                    var newAns = $(me).text();
                    var newNumber = newAns.replace(/[^0-9]/g, '');
                    var newString = newAns.replace(newNumber, '');

                    if ($(me).text() === ans || newString.trim() === 'Answer') {
                      $(me).attr('defaultText', newAns);
                      var isenter = $(me).attr('isenter');
                      if (isenter == 'true') {
                        return;
                      } else {
                        $(me).text('');
                      }
                    }
                  });
                  $('#' + ckId).find('iframe[allowtransparency]').contents().find('body').on('blur', function() {
                    var me = this;

                    if (isOk === true) {

                      if ($(me).text() !== '') {
                        return 1;
                      } else {
                        $(me).text($('#' + ckId).find('iframe[allowtransparency]').contents().find('body').attr('defaulttext'));
                      }

                    } else {
                      if ($(me).text() === '') {
                        $(me).text('Answer ' + ind); // set text when no press Ok

                        $(me).text($('#' + ckId).find('iframe[allowtransparency]').contents().find('body').attr('defaulttext'));
                      }
                    }

                  });
                });

                //classNameStyleFontInlineChoice
                if (classNameStyleFontInlineChoice !== '') {
                  $('.divInlineChoice .cke_combo__fontsize').find('a .cke_combo_text').text(classNameStyleFontInlineChoice);
                  $('.divInlineChoice .cke_combo__fontsize').find('.cke_combo_label').text(classNameStyleFontInlineChoice);
                }

              });

              if (isEditInlineChoice) {

                //Only create CKEditor after html appended
                $("#inlineChoice li").each(function() {
                  createinlineCKEditorMulti("editor" + $(this).attr("id"));
                });

                for (var i = 0; i < iResult.length; i++) {
                  if (iResult[i].responseIdentifier == currentInlineChoiceResId && iResult[i].type == "inlineChoiceInteraction") {

                    //Clear all item before add new
                    $("#point").val(iResult[i].responseDeclaration.pointsValue);
                    var arrInlineChoice = iResult[i].inlineChoice;
                    for (var m = 0; m < arrInlineChoice.length; m++) {

                      //Load data to each item
                      var currentItem = $("#inlineChoice li").eq(m);
                      CKEDITOR.instances["editor" + currentItem.attr("id")].setData(arrInlineChoice[m].value);
                      //load point for answer
                      currentItem.find('.audio input[type=text]').val(arrInlineChoice[m].pointsValue);
                      //Load correct answer
                      if (arrInlineChoice[m].answerCorrect != undefined && arrInlineChoice[m].answerCorrect.toString() == "true") {
                        currentItem.find(".correctAnswer input[type=radio]").prop("checked", true);
                      }

                      //apply data guidance has into iResult to iMessageTemp
                      if (isEditMultipChoiceGuidance) {
                        if (arrInlineChoice[m].arrMessageGuidance.length) {
                          for (var k = 0, lensChoice = arrInlineChoice[m].arrMessageGuidance.length; k < lensChoice; k++) {

                            if (arrInlineChoice[m].arrMessageGuidance[k].typeMessage === 'guidance') {
                              currentItem.find('#selected_' + currentItem.attr('id')).attr('title', 'Guidance');
                            }

                            if (arrInlineChoice[m].arrMessageGuidance[k].typeMessage === 'rationale') {
                              currentItem.find('#selected_' + currentItem.attr('id')).attr('title', 'Rationale');
                            }

                            if (arrInlineChoice[m].arrMessageGuidance.length === 2) {
                              currentItem.find('#selected_' + currentItem.attr('id')).attr('title', 'Guidance and Rationale');
                            }

                            if (arrInlineChoice[m].arrMessageGuidance[k].typeMessage === 'guidance_rationale') {
                              currentItem.find('#selected_' + currentItem.attr('id')).attr('title', 'Guidance and Rationale');
                            }
                          }

                          currentItem.find('#selected_' + currentItem.attr('id')).parent('.savedGuidance').show();
                          currentItem.find('#unselected_' + currentItem.attr('id')).hide();
                        }

                        iMessageTemp.push({
                          identifier: arrInlineChoice[m].identifier,
                          idTemp: currentItem.attr('id'),
                          arrMessage: arrInlineChoice[m].arrMessageGuidance,
                        });

                      }

                    }

                    if (iResult[i].responseDeclaration.method === 'algorithmic') {
                      $dialog.find('#ic-grading-algorithmic').prop('checked', true);
                      resetGradingAlgorithmic($dialog, true);
                    } else {
                      $dialog.find('#ic-grading-normal').prop('checked', true);
                    }

                    checkElementRemoveIntoIResult();
                    break;

                  }
                }
              }

              if (questionQtiSchemeId == 21) {
                $dialog.find('#ic-grading-algorithmic').parent().hide();
              }

              /*** Check enable or disable checkbox visible demension */
              let visibleDimension = $("#checkBoxVisibleDimension").prop('checked');
              if (visibleDimension) {
                $(".dimension-width .ckUpDownNumber").removeClass("is-disabled");
              }
              else {
                $(".dimension-width .ckUpDownNumber").addClass("is-disabled");
              }
            }
          }]

        }],
        onOk: function() {
          var expWidthILC = $("#expWidthILC").val() || 20;
          if (parseInt(expWidthILC) < 7 && $('#checkBoxVisibleDimension').is(":checked")) {
            customAlert("The minimum visible dimension is 7.");
            $('#expWidthILC').val('7');
            return false;
          }
          isAddinlineAnswer = false;

          //Check in case user has checked in correct answer but they don't fill text
          var isEmpty = false,
            qValue = "";
          $("#inlineChoice li .correctAnswer input[type=radio]").each(function() {

            var currentEditor = CKEDITOR.instances['editor' + $(this).parents("li").attr("id")];

            if ($(this).is(":checked") && currentEditor.getData() == "") {
              isEmpty = true;
              if (qValue != "") {
                qValue += ", ";
              }
              qValue += $(this).val();
            }
          });

          if (isEmpty) {
            alert("Please fill content for answer " + qValue);
            return false;
          }

          var cardinalityChoice = "single",
            simpleMulChoice = [],
            htmlInlineChoice = "",
            correctMulResponse = "",
            currentAnswerIndex = 0, // Store correct alphebet for question
            alphaBe = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H'],
            pointCorrect = '';

          var typeGuidance = '';
          var typeRationale = '';
          var typeGuidanceRationale = '';
          var hastypeMessageGuidance = '';
          var gradingMethod = 'default';

          if ($('#ic-grading-algorithmic').is(':checked')) {
            gradingMethod = 'algorithmic';
          }

          if (CKEDITOR.instances[ckID].config.alphaBeta != undefined) {
            alphaBe = CKEDITOR.instances[ckID].config.alphaBeta;
          }

          $("#inlineChoice li").each(function(index) {

            //add data guidance and rationale into simplechocie
            var idTagLi = $(this).attr('id');
            var arrMessageGuidance = [];

            if ($(this).find("#selected_" + idTagLi).is(':visible')) {

              if (iMessageTempEdit.length) {
                for (var j = 0, leArrMessageTempEdit = iMessageTempEdit.length; j < leArrMessageTempEdit; j++) {
                  var itemMessageTempEdit = iMessageTempEdit[j];
                  for (var k = 0, lenArrMessage = iMessageTemp.length; k < lenArrMessage; k++) {
                    if (itemMessageTempEdit.idTemp === iMessageTemp[k].idTemp) {
                      iMessageTemp[k].arrMessage = itemMessageTempEdit.arrMessage;
                    }
                  }
                }
              }


              for (var i = 0, lenArrMessage = iMessageTemp.length; i < lenArrMessage; i++) {
                if (iMessageTemp[i].idTemp === idTagLi) {
                  arrMessageGuidance = iMessageTemp[i].arrMessage;
                  break;
                }
              }

              hastypeMessageGuidance = 'hasGuidance';

              for (var k = 0, lenArrMessageGuidance = arrMessageGuidance.length; k < lenArrMessageGuidance; k++) {
                var itemMessage = arrMessageGuidance[k];

                if (itemMessage.typeMessage === "guidance") {
                  typeGuidance = includeHtmlGuidance(itemMessage, "guidance");
                }
                if (itemMessage.typeMessage === "rationale") {
                  typeRationale = includeHtmlGuidance(itemMessage, "rationale");
                }
                if (itemMessage.typeMessage === "guidance_rationale") {
                  typeGuidanceRationale = includeHtmlGuidance(itemMessage, "guidance_rationale");
                }
              }
            }

            var iddentify = alphaBe[currentAnswerIndex],
              editorId = $(this).find(".content textarea").attr("id"),
              point = $(this).find('.point').val(),
              hasCorrectAnwser = 'class="item"';

            //remove span speChar
            var tempData = CKEDITOR.instances[editorId].getData().toString();

            var item = {
              identifier: iddentify,
              value: tempData,
              pointsValue: "",
              arrMessageGuidance: arrMessageGuidance
            };
            //save text Default
            var textDefault = $(this).find('iframe[allowtransparency]').contents().find('body').attr('defaultText');
            if (textDefault === undefined) {
              textDefault = '';
            }

            //Only add item if item has content
            if (CKEDITOR.instances[editorId].getData() == "") {
              return;
            }

            if ($(this).find(".correctAnswer input[type=radio]").is(':checked')) {
              if (correctMulResponse != "") {
                correctMulResponse += ",";
              }
              correctMulResponse += iddentify;
              hasCorrectAnwser = 'class="item answerCorrect"';
              item.answerCorrect = true;

              pointCorrect = $(this).find('.point').val();

            }

            //Add item for multiple choice
            htmlInlineChoice += '<div defaultText="' + textDefault + '" ' + hasCorrectAnwser + '><div><img alt="Play audio" class="imageupload bntPlay" src="' + CKEDITOR.plugins.getImgByVersion('multiplechoice', 'images/small_audio_play.png') + '" title="Play audio"><img alt="Stop audio" class="bntStop" src="' + CKEDITOR.plugins.getImgByVersion('multiplechoice', 'images/small_audio_stop.png') + '" title="Stop audio"><span class="audioRef"></span></div><div class="answer">' + item.identifier + '.</div> <div class="answerContent">' + item.value + '</div></div>';

            simpleMulChoice.push(item);

            currentAnswerIndex += 1;

          });

          var responseId = "";
          if (isEditInlineChoice) {
            var $eleInlineChoice = $(eleInlineChoice.$);

            //Update for current inline Choice
            for (var n = 0; n < iResult.length; n++) {
              if (iResult[n].responseIdentifier == currentInlineChoiceResId) {
                responseId = currentInlineChoiceResId;
                iResult[n].correctResponse = correctMulResponse;
                iResult[n].responseDeclaration.pointsValue = $("#point").val();
                iResult[n].inlineChoice = simpleMulChoice;
                iResult[n].responseDeclaration.method = gradingMethod;
                iResult[n].expectedWidth = parseInt(expWidthILC) * 10;
                iResult[n].visibleDimension = $('#checkBoxVisibleDimension').is(":checked") ? 1 : 0;

                if (iResult[n].visibleDimension == 1)
                  $(eleInlineChoice.$).css('max-width', parseInt(expWidthILC) * 10 + 'px')
                else
                  $(eleInlineChoice.$).css('max-width', '250px')

                break;
              }
            }


            //apply font size when edit inline choice
            if (isOnClickFontSize && classNameStyleFontInlineChoice != '') {
              var $eleInlineChoiceParent = $eleInlineChoice.parent();
              var isSelectedFontSize = $eleInlineChoiceParent.hasClass('smallText') || $eleInlineChoiceParent.hasClass('normalText') || $eleInlineChoiceParent.hasClass('largeText') || $eleInlineChoiceParent.hasClass('veryLargeText');

              if (isSelectedFontSize) {
                var styleFontSize = convertStyleFontSizeInlineChoice(classNameStyleFontInlineChoice);
                $eleInlineChoiceParent.attr('class', styleFontSize);
                $eleInlineChoiceParent.attr('stylename', styleFontSize);
              } else {
                var bodyEditor = $eleInlineChoice.parents('body');
                var $bodyEditorResponseId = bodyEditor.find('#' + responseId);

                switch (classNameStyleFontInlineChoice) {
                  case 'Small':
                    $bodyEditorResponseId.wrap('<span class="smallText" stylename="smallText"></span>');
                    break;
                  case 'Normal':
                    $bodyEditorResponseId.wrap('<span class="normalText" stylename="normalText"></span>');

                    break;
                  case 'Large':
                    $bodyEditorResponseId.wrap('<span class="largeText" stylename="largeText"></span>');
                    break;
                  case 'X-Large':
                    $bodyEditorResponseId.wrap('<span class="veryLargeText" stylename="veryLargeText"></span>');
                    break;
                  default:
                    break;
                }
              }
            }

            $eleInlineChoice.find('img.bntGuidance').hide();
          } else {
            var visibleDimension = $('#checkBoxVisibleDimension').is(":checked") ? 1 : 0;
            //Create response identify and make sure it doesn't conflict with current.
            responseId = createResponseIdFromDOM('#itemtypeonimagePreview');
            expWidthILC = $("#expWidthILC").val() || 20;
            var inlinechoiceHtml = '<span class="inlineChoiceInteraction" id="' + responseId + '" title="' + responseId + '" ';
            if (visibleDimension == 1)
              inlinechoiceHtml += 'style="max-width:' + parseInt(expWidthILC) * 10 + 'px;" ';
            else
              inlinechoiceHtml += 'style="max-width: 250px;" ';

            inlinechoiceHtml += ' contenteditable="false">';
            inlinechoiceHtml += ' <img style = "padding-left: 3px; padding-top: 0.5px;display: none;" alt = "Guidance" class="imageupload bntGuidance" src = "../../Content/themes/TestMaker/images/guidance_checked_small.png" title = "Guidance" >';
            inlinechoiceHtml += ' <img class="cke_reset cke_widget_mask inlineChoiceInteractionMark" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" /></span >& nbsp;';
            var allowItemTypeOnImage = window.dialogItemtypeonimage;

            //check has add font size when instant
            if (isOnClickFontSize && classNameStyleFontInlineChoice != '') {
              switch (classNameStyleFontInlineChoice) {
                case 'Small':
                  inlinechoiceHtml = '<span class="smallText" stylename="smallText">' + inlinechoiceHtml + '</span>';
                  break;
                case 'Normal':
                  inlinechoiceHtml = '<span class="normalText" stylename="normalText">' + inlinechoiceHtml + '</span>';
                  break;
                case 'Large':
                  inlinechoiceHtml = '<span class="largeText" stylename="largeText">' + inlinechoiceHtml + '</span>';
                  break;
                case 'X-Large':
                  inlinechoiceHtml = '<span class="veryLargeText" stylename="veryLargeText">' + inlinechoiceHtml + '</span>';
                  break;
                default:
                  break;
              }
            }

            var inlinechoiceElement = CKEDITOR.dom.element.createFromHtml(inlinechoiceHtml);
            editor.insertElement(inlinechoiceElement);

            if (allowItemTypeOnImage) {
              var $editor = $(editor.document.$).find('body');
              var $itemtypeonimagePreview = $('#itemtypeonimagePreview:visible');
              var w = parseInt($itemtypeonimagePreview.find('.itemtypeonimageMarkObject').attr('width'), 10);
              var h = parseInt($itemtypeonimagePreview.find('.itemtypeonimageMarkObject').attr('height'), 10);
              var offsetTop = h / 2 - 10;
              var offsetLeft = w / 2 - 45;
              var $container = $('<div/>');

              if (iSchemeID == 8) {
                inlinechoiceHtml = $container.append(inlinechoiceHtml)
                  .find('.inlineChoiceInteraction')
                  .append('<span class="remove-item"></span>')
                  .prop('outerHTML');
              } else if (iSchemeID == 21) {
                inlinechoiceHtml = $container.append(inlinechoiceHtml)
                  .find('.inlineChoiceInteraction')
                  .append('<span class="itemtypeonimage-tooltip">' + responseId.replace('RESPONSE_', '') + '</span>')
                  .append('<span class="remove-item"></span>')
                  .prop('outerHTML');
              }

              $itemtypeonimagePreview.find('.itemtypeonimage').append(inlinechoiceHtml);
              $itemtypeonimagePreview
                .find('.itemtypeonimage')
                .find('.inlineChoiceInteraction[id="' + responseId + '"]')
                .css({
                  'left': offsetLeft + 'px',
                  'top': offsetTop + 'px'
                })
                .attr({
                  'data-left': offsetLeft,
                  'data-top': offsetTop
                });
              $itemtypeonimagePreview
                .find('.itemtypeonimage')
                .find('.inlineChoiceInteraction')
                .draggable({
                  containment: 'parent',
                  drag: function(event, ui) {
                    var $target = $(event.target);

                    $target.attr('data-top', ui.position.top);
                    $target.attr('data-left', ui.position.left);
                  }
                });

              $itemtypeonimagePreview.find(".remove-item").on("click", function() {
                var item = $(this).parent();
                var resId = item.attr("id");
                var select = $(this).parents(".cke_dialog_contents").find("#itemtypeonimageSelect");

                item.remove();
                //need remove data in iResult
                for (var i = 0; i < iResult.length; i++) {
                  if (iResult[i].responseIdentifier === resId) {
                    iResult.splice(i, 1);
                  }
                }
              });

              $editor.find('.inlineChoiceInteraction[id="' + responseId + '"]').css('display', 'none');
            }

            hastypeMessageGuidance = '';
            //add data
            iResult.push({
              type: "inlineChoiceInteraction",
              responseIdentifier: responseId,
              shuffle: false,
              inlineChoice: simpleMulChoice,
              responseDeclaration: {
                baseType: "identifier",
                cardinality: "single",
                method: gradingMethod,
                caseSensitive: "false",
                type: "string",
                pointsValue: $("#point").val()
              },
              correctResponse: $("#inlineChoice li").find("input.choiceCheckbox:checked").val(),
              expectedWidth: parseInt(expWidthILC) * 10,
              visibleDimension: $('#checkBoxVisibleDimension').is(":checked") ? 1 : 0,
            });

            ////Hide button on toolbar after add inline choice incase qtItem is 8
            if (iSchemeID == "8") {
              $(".cke_button__inlinechoice").parents("span.cke_toolbar").hide();
            }

            if (iSchemeID == "21" && !$(editor.document.$).find('body').children(':first-child').is('br')) {
              $(editor.document.$).find('body').prepend('<br>');
            }

            ResIdElemModul = responseId;

          }

          //Reset to default after update or create new textEntry
          isEditInlineChoice = false;
          currentInlineChoiceResId = "";
          isOk = true;

          newResult = iResult;

          //set new style font size for inline choice
          isStyleFontInlineChoice = false;
          classNameStyleFontInlineChoice = '';
          isOnClickFontSize = false;

          isEditMultipChoiceGuidance = false;
          iMessageTemp = [];
          idSimpleChoicesPopup = '';
          iMessageTempEdit = [];

          if (gradingMethod === 'algorithmic') {
            TestMakerComponent.isShowAlgorithmicConfiguration = true;
          } else {
            TestMakerComponent.isShowAlgorithmicConfiguration = false;
          }
        },
        onCancel: function() {
          //Reset to default after update or create new textEntry
          isEditInlineChoice = false;
          currentInlineChoiceResId = "";
          ResIdElemModul == "";
          classNameStyleFontInlineChoice = '';
          isStyleFontInlineChoice = false;
          isOnClickFontSize = false;

          isEditMultipChoiceGuidance = false;
          iMessageTemp = [];
          idSimpleChoicesPopup = '';
          iMessageTempEdit = [];
        }
      };
    });
  }
});

/***
 * Load data to Inline Choice popup form
 * Return: responseIdentifier
 ***/
function loadDataforInlineChoice(element) {
  var currentInlineChoiceResId = "";
  var arrayInlineChoice = [];

  $('.divInlineChoice .point').attr("disabled", false);

  for (var i = 0; i < iResult.length; i++) {
    if (iResult[i].responseIdentifier == element.getId() && iResult[i].type == "inlineChoiceInteraction") {
      currentInlineChoiceResId = iResult[i].responseIdentifier;

      //Clear all item before add new
      $("#inlineChoice").empty();

      arrayInlineChoice = iResult[i].inlineChoice;

      for (var m = 0; m < arrayInlineChoice.length; m++) {
        addNewIteminlineChoice();
        inlineChoiceSelection();
      }
      var expectedWidth = iResult[i].expectedWidth ? parseInt(iResult[i].expectedWidth) / 10 : 20;
      $('#expWidthILC').val(expectedWidth);

      var visibleDimension = false;
      if (iResult[i].visibleDimension == 1) {
        visibleDimension = true;
      }

      if (visibleDimension) {
        $(".dimension-width .ckUpDownNumber").removeClass("is-disabled");
      }
      else {
        $(".dimension-width .ckUpDownNumber").addClass("is-disabled");
      }
      $('#checkBoxVisibleDimension').attr('checked', visibleDimension);

      break;
    }
  }

  return currentInlineChoiceResId;
}
/***
 * Reset data to Inline Choice popup form
 ***/
function resetInlineChoiceOnload() {
  $("ul#choiceList").html('<li><span>Answer A</span><input type="radio" name="listChoice" checked="checked" value="A" class="choiceCheckbox" /></li><li><span>Answer B</span><input type="radio" name="listChoice" value="B" class="choiceCheckbox" /></li>');
  $('#point').val("1");
  $('#choiInput').val("");
  $('#expWidthILC').val('30');
  $("#checkBoxVisibleDimension").val(0);
  $('#checkBoxVisibleDimension').attr('checked', false);

  //Register event after reset
  $("#choiceList li").click(function(event) {
    if ($(event.target).is('.choiceCheckbox')) {
      return;
    }
    $("#choiceList li").removeClass("active");
    $(this).addClass("active");
    txtVal = $(this).text();
    $("#choiInput").val(txtVal);
  });
}
/***
 * Function to add new an anwser for multiple Choice
 * Return EditorID this will help to create CKEditor
 ***/
function addNewIteminlineChoice() {

  var inlineChoiceLength = $("#inlineChoice li").length;
  var alphabet = [];
  if (CKEDITOR.instances[ckID].config.alphaBeta != undefined) {
    alphabet = CKEDITOR.instances[ckID].config.alphaBeta;
  }

  //maximum answer for multipleChoise is 25 answers
  if (inlineChoiceLength > 40) {
    return;
  }

  var now = $.now(),
    editorId = 'editor' + alphabet[inlineChoiceLength] + now,
    pointId = 'point' + alphabet[inlineChoiceLength] + now;

  //push id item answer into iMessageTemp
  if (!isEditMultipChoiceGuidance) {
    iMessageTemp.push({
      idTemp: alphabet[inlineChoiceLength] + now,
      identifier: alphabet[inlineChoiceLength],
      arrMessage: []
    });
  }
  var getImgByVersion = CKEDITOR.plugins.getImgByVersion;

  var defaultText = "Answer " + ($("#inlineChoice li").length + 1);

  var strHtml = '<li id="' + alphabet[inlineChoiceLength] + now + '">';

  strHtml += '<div class="alphabet">' + alphabet[inlineChoiceLength] + '</div>';
  strHtml += '<div class="correctAnswer"><input type="radio" class="choiceCheckbox" name="inlineChoice" value="' + alphabet[inlineChoiceLength] + '"/></div>';
  strHtml += '<div class="content">';
  strHtml += '<textarea cols="100" id="' + editorId + '" name="editor1" rows="1" tabindex="' + (inlineChoiceLength + 1) + '">' + defaultText + '</textarea>';
  strHtml += '</div>';
  strHtml += '<div class="sort actions">';
  strHtml += '<input id="unselected_' + (alphabet[inlineChoiceLength]) + now + '" type="image" src="' + getImgByVersion('multiplechoice', 'images/guidance_unchecked.png') + '" class="addGuidance" title="Add Guidance/Rationales" />';
  strHtml += '<div class="savedGuidance" style="display: none;"><span class="btnRemoveGuidance" title="Remove">x</span><input issavedmessage="true" id="selected_' + (alphabet[inlineChoiceLength] + now) + '" type="image" src="' + getImgByVersion('multiplechoice', 'images/guidance_checked.png') + '" class="addGuidance" title="" /></div>';
  strHtml += '<div class="audio" style="visibility: hidden; display: none;"><input issavedmessage="false" type="text" id="' + pointId + '" class="point" value="0" />';
  strHtml += '</div>';
  strHtml += '<input type="image" title="Move Up" src="' + getImgByVersion('multiplechoice', 'images/up.png') + '" class="ckImageButton ckUp" />';
  strHtml += '<input type="image" title="Move Down" src="' + getImgByVersion('multiplechoice', 'images/down.png') + '" class="ckImageButton ckDown" />';
  strHtml += '<input type="image" title="Remove" src="' + getImgByVersion('multiplechoice', 'images/remove.png') + '" class="ckImageButton ckRemove" />';
  strHtml += '</div>';
  strHtml += '<div class="clear"></div>';
  strHtml += '</li>';

  $("#inlineChoice").append(strHtml);
  IS_V2 && $("#inlineChoice").find('[title]').tip();

  return editorId;
}
/***
 * Function to register all events for inline choice
 ***/
function inlineChoiceSelection() {
  $(".ckRemove").unbind("click").click(function() {
    var $inlineChoice = $('#inlineChoice');
    //Don't let's user remove all answer
    isAddAnswerInlineChoiceLast = true;

    if ($("#inlineChoice li").length == 2) {
      return;
    }
    var answer = $(this).parent().parent(),
      instanceName = answer.find("textarea").attr("id");
    var index = answer.index();
    if (CKEDITOR.instances[instanceName]) CKEDITOR.instances[instanceName].destroy();
    answer.remove();
    sortItemInlineChoice(index);
    //Show the first CK Toolbar when remove an item
    $("#inlineTop > div").hide();
    $("#inlineTop > div:first").show();
    // Always set correct answer the first
    if (!$inlineChoice.find('.choiceCheckbox').is(':checked')) {
      $inlineChoice.find('li').eq(0).find('.choiceCheckbox').prop('checked', true);
    }
  });

  var createCkeditorInlineChoice = function(editorId) {
    try {
      CKEDITOR.instances[editorId].destroy(true);
    } catch (e) {}

    CKEDITOR.replace(editorId, {
      toolbar: [
        ['SpecialChar'],
        ['Bold', 'Italic', 'Underline'],
        ['Subscript', 'Superscript', '-', 'RemoveFormat']
      ],
      sharedSpaces: {
        top: 'inlineTop',
        bottom: 'inlineBot'
      },
      extraAllowedContent: 'span[stylename]; span(smallText,normalText,largeText,veryLargeText);',
      height: 60,
      width: 350
    });

    // Show the first CK Toolbar when remove an item
    $('#inlineTop > div:first').show();
  }


  $(".ckUp").unbind("click").click(function(event) {
    isAddAnswerInlineChoiceLast = true;
    event.preventDefault();
    var parent = $(this).parents('li');
    var parentIndex = parent.index();

    if (parentIndex == 0) {
      return;
    }

    var editorId = 'editor' + parent.attr('id');
    if (CKEDITOR.instances[editorId]) {
      CKEDITOR.instances[editorId].destroy();
    }

    isExtraChar = false;

    //Change alphabet of anwser
    parent.prev().find('.alphabet').empty().html(alphabet[parentIndex]);
    parent.prev().find('.correctAnswer input:radio').val(alphabet[parentIndex]);
    parent.prev().find('textarea').attr({
      'tabindex': parentIndex + 1
    });
    parent.find('.alphabet').empty().html(alphabet[parentIndex - 1]);
    parent.find('.correctAnswer input:radio').val(alphabet[parentIndex - 1]);
    parent.find('textarea').attr({
      'tabindex': parentIndex
    });
    parent.insertBefore(parent.prev());

    createCkeditorInlineChoice(editorId);
  });

  $(".ckDown").unbind("click").click(function(event) {
    event.preventDefault();
    isAddAnswerInlineChoiceLast = true;
    var parent = $(this).parents('li');
    var parentIndex = parent.index();
    var textContent = parent.find('.content').find('iframe[allowtransparency]').contents().find('body').text();
    if (parentIndex == $('#inlineChoice li').length - 1) {
      return;
    }

    var editorId = 'editor' + parent.attr('id');
    if (CKEDITOR.instances[editorId]) {
      CKEDITOR.instances[editorId].destroy();
    }

    isExtraChar = false;

    // Change alphabet of anwser
    parent.next().find('.alphabet').empty().html(alphabet[parentIndex]);
    parent.next().find('.correctAnswer input:radio').val(alphabet[parentIndex]);
    parent.next().find('textarea').attr({
      'tabindex': parentIndex + 1
    });
    parent.find('.alphabet').empty().html(alphabet[parentIndex + 1]);
    parent.find('.correctAnswer input:radio').val(alphabet[parentIndex + 1]);
    parent.find('textarea').attr({
      'tabindex': parentIndex + 2
    });
    parent.find('textarea').attr('defaultText', textContent);
    parent.insertAfter(parent.next());


    createCkeditorInlineChoice(editorId);
  });

  $(".point").each(function() {
    var idPoint = $(this).attr('id');
    $('#' + idPoint).ckUpDownNumber({
      maxNumber: 1000,
      width: 18,
      height: 13
    });
  });
  $('.point').ckUpDownNumber({
    maxNumber: 1000,
    width: 18,
    height: 13
  });

  $(".expLength").each(function () {
    var idPoint = $(this).attr('id');
    $('#' + idPoint).ckUpDownNumber({
      maxNumber: 1000,
      width: 18,
      height: 13
    });
  });
  $('.expLength').ckUpDownNumber({
    maxNumber: 1000,
    width: 18,
    height: 13
  });

  $(".addGuidance").unbind("click").click(function(event) {
    var isSavedMessage = $(this).attr('issavedmessage');

    if (isSavedMessage === 'true') {
      isEditGuidancePopup = true;
    } else {
      isEditGuidancePopup = false;
    }

    var parent = $(this).parents("li");
    var editorId = 'editor' + parent.attr("id");
    idMessage = parent.attr("id");

    CKEDITOR.instances[editorId].openDialog('messageGuidanceRationales');
  });

  $(".btnRemoveGuidance").unbind("click").click(function(event) {
    var idTagLi = $(event.target).parents('li').attr('id');

    if (iMessageTempEdit.length) {
      for (var i = 0, lenMessageTemp = iMessageTempEdit.length; i < lenMessageTemp; i++) {
        var itemMessage = iMessageTempEdit[i];
        if (itemMessage.idTemp === idTagLi) {
          iMessageTempEdit.splice(i, 1);
          break;
        }
      }
    }

    $(this).parents(".sort").find('.savedGuidance').hide();
    $(this).parents(".sort").find("#unselected_" + idTagLi).show();
  });

  if ($('#ic-grading-algorithmic').is(':checked')) {
    $('input[type="radio"].choiceCheckbox').prop('disabled', true).removeAttr('checked');
  }
}
//Create new 2 anwsers
function createinlineAnswer() {
  $("#inlineChoice li").each(function() {
    var myId = "editor" + this.id;
    if (CKEDITOR.instances[myId]) CKEDITOR.instances[myId].destroy();
  });
  $("#inlineChoice").empty();

  //Create default (2)item for inlineChoice
  for (var k = 0; k < 2; k++) {
    var editorId = addNewIteminlineChoice();

    inlineChoiceSelection();
    createinlineCKEditorMulti(editorId);
  }

  $("#inlineChoice .correctAnswer input[type=radio]").first().prop("checked", true);
  $(".divInlineChoice .point").first().val("1");
  $("#inlineTop > div:first").show();

}
//build a instance ckEditor
function createinlineCKEditorMulti(ckId) {
  isExtraChar = false;

  try {
    CKEDITOR.instances[ckId].destroy(true);
  } catch (e) {}

  CKEDITOR.replace(ckId, {
    toolbar: [
      ['SpecialChar'],
      ['Bold', 'Italic', 'Underline'],
      ['Subscript', 'Superscript', '-', 'RemoveFormat']
    ],
    sharedSpaces: {
      top: 'inlineTop',
      bottom: 'inlineBot'
    },
    extraAllowedContent: 'span[stylename]; span(smallText,normalText,largeText,veryLargeText);',
    height: 60,
    width: 350
  });
}
/***
 * Function to sort when remove an anwser of inline Choice
 ***/
function sortItemInlineChoice(index) {
  for (var i = index; i < $("#inlineChoice li").length; i++) {
    var currentAnwser = $("#inlineChoice li").eq(i);
    currentAnwser.find(".alphabet").empty().html(alphabet[i]);
    currentAnwser.find(".correctAnswer input:radio").val(alphabet[i]);
    currentAnwser.find("textarea").attr({
      "tabindex": i + 1
    });
  }
}
