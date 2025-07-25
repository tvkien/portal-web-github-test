CKEDITOR.plugins.add('multiplechoicevariable', {
  lang: 'en', // %REMOVE_LINE_CORE%
  icons: 'multiplechoicevariable',
  requires: 'dialog',
  hidpi: true, // %REMOVE_LINE_CORE%
  init: function(editor) {
    var pluginName = 'insertMultipleChoiceVariable';

    editor.addCommand(pluginName, new CKEDITOR.dialogCommand(pluginName));

    editor.ui.addButton('MultipleChoiceVariable', {
      label: 'Multiple Choice with Variable Points',
      command: pluginName,
      icon: this.path + 'icons/multiplechoice.png',
      toolbar: 'insertMultipleChoiceVariable,30'
    });

    editor.widgets.add('multiplechoicevariable', {
      inline: true,
      mask: true,
      allowedContent: {
        div: {
          classes: 'multipleChoice,item,audioIcon,multipleChoiceVariableMark',
          attributes: '!id,name,contenteditable'
        }
      },
      template: '<div class="multipleChoice multipleChoiceVariable"></div>'
    });

    var isEditMultipleChoiceVariable = false,
      currentMultipleChoiceResId = "",
      isAddAnswer = false,
      eleMultipleChoice = ""; //This use for first time load popup
    var isOk = false;

    var resetGradingAlgorithmic = function(dialog, isShow) {
      dialog.find('.point').parent().addClass('is-disabled');
      dialog.find('.point-item').parent().addClass('is-disabled');
      dialog.find('.point-item').val('0');
      dialog.find('#mcv-display-vertical').prop('checked', true);
      dialog.find('#mcv-display-grid-per-row').addClass('hide');
      dialog.find('#mcv-grid-per-row').val(2);

      if (!isShow) {
        dialog.find('.point').val('0');
      }
    };

    var resetGradingNormal = function(dialog) {
      dialog.find('.point').parent().removeClass('is-disabled');
      dialog.find('.point-item').parent().removeClass('is-disabled');
    };

    var resetGradingInformationalOnly = function(dialog) {
      dialog.find('.point').parent().removeClass('is-disabled');
      dialog.find('.point-item').parent().removeClass('is-disabled');
    };

    var resetMultipleChoice = function(dialog) {
      dialog.find('#mcv-grading-normal').prop('checked', true);
      resetGradingNormal(dialog);
    };

    var handlerMultiSelect = function(dialog) {
      dialog.find('#select_multi_variable').on('click', function() {
        var $multiSelect = $(this);

        if ($multiSelect.is(':checked')) {
          dialog.find('.maxChoiceField').show();
          dialog.find('#maxChoices').val(dialog.find('#multipleChoiceVariable li').length);
        }
      });
    };

    var handlerSingleSelect = function(dialog) {
      dialog.find('#select_single_variable').on('click', function() {
        var $singleSelect = $(this);

        if ($singleSelect.is(':checked')) {
          var $correctAnswerInput = dialog.find('#multipleChoiceVariable .correctAnswer input[type="checkbox"]')
          dialog.find('.maxChoiceField').hide();
          $correctAnswerInput.prop('checked', false);
          $correctAnswerInput.first().prop('checked', true);
        }
      });
    };

    var handlerOnChangeGradingMethod = function(dialog) {
      dialog.find('input[type="radio"][name="mcv-grading"]').on('change', function() {
        var $grading = $(this);
        var gradingMethod = $grading.attr('id');

        if (gradingMethod === 'mcv-grading-algorithmic') {
          resetGradingAlgorithmic(dialog);
        } else if (gradingMethod === 'mcv-grading-informational-only') {
          resetGradingInformationalOnly(dialog);
        } else {
          resetGradingNormal(dialog);
        }
      });
    };

    var handlerOnChangeDisplayMethod = function(dialog) {
      dialog.find('input[type="radio"][name="mcv-display"]').on('change', function() {
        var $display = $(this);
        var displayMethod = $display.attr('id');
        var $displayGridPerRow = dialog.find('#mcv-display-grid-per-row');

        if (displayMethod === 'mcv-display-grid') {
          $displayGridPerRow.removeClass('hide');
        } else {
          $displayGridPerRow.addClass('hide');
        }
      });
    };

    var getGradingMethod = function(dialog) {
      if (dialog.find('#mcv-grading-algorithmic').is(':checked')) {
        return 'algorithmic';
      }

      if (dialog.find('#mcv-grading-informational-only').is(':checked')) {
        return 'informational-only';
      }

      return 'default';
    };

    var getDisplayMethod = function(dialog) {
      if (dialog.find('#mcv-display-horizontal').is(':checked')) {
        return 'horizontal';
      }

      if (dialog.find('#mcv-display-grid').is(':checked')) {
        return 'grid';
      }

      return 'vertical';
    };

    var onShowGradingMethod = function(dialog, gradingMethod) {
      if (gradingMethod === 'algorithmic') {
        dialog.find('#mcv-grading-algorithmic').prop('checked', true);
        resetGradingAlgorithmic(dialog, true);
      } else if (gradingMethod === 'informational-only') {
        dialog.find('#mcv-grading-informational-only').prop('checked', true);
      } else {
        dialog.find('#mvc-grading-normal').prop('checked', true);
      }
    };

    var onShowDisplayMethod = function(dialog, displayMethod, gridPerRow) {
      if (displayMethod === 'horizontal') {
        dialog.find('#mcv-display-horizontal').prop('checked', true);
      } else if (displayMethod === 'grid') {
        dialog.find('#mcv-display-grid').prop('checked', true);
        dialog.find('#mcv-grid-per-row').val(gridPerRow);
        dialog.find('#mcv-display-grid-per-row').removeClass('hide');
      } else {
        dialog.find('#mcv-display-vertical').prop('checked', true);
      }
    };

    //set event click play audio
    editor.on('contentDom', function() {

      //play audio
      var btnPlay = $(editor.window.getFrame().$).contents().find('.audioIcon .bntPlay');
      btnPlay.unbind("click").on('click', function(e) {
        $(editor.window.getFrame().$).contents().find('.audioIcon .bntStop').hide();
        $(editor.window.getFrame().$).contents().find('.audioIcon .bntPlay').show();
        resetUIAudio();
        var audioUrl = $(this).parent().find(".audioRef").text();
        playAudio(this, audioUrl);
      });
      //stop audio
      var bntStop = $(editor.window.getFrame().$).contents().find('.audioIcon .bntStop');
      bntStop.unbind("click").on('click', function(e) {
        stopAudio(this);
      });

      //set single click popup
      var singleclick = $(editor.window.getFrame().$).contents().find('.multipleChoiceVariable .single-click-variable');
      singleclick.unbind("click").on('click', function(e) {

        var element = e.target.parentElement;

        //Move selection to parent of multipleChoiceMark
        if (CKEDITOR.env.safari) {
          editor.getSelection().getSelectedElement();
        } else {
          editor.getSelection().selectElement(editor.document.getActive().getParent());
        }


        $('#multipleTopVariable').html('');
        //The status to editor know this is update
        isEditMultipleChoiceVariable = true;
        eleMultipleChoice = element;

        editor.openDialog(pluginName, function() {
          currentMultipleChoiceResId = loadDataforMultipleChoiceVariable(element);
        });
      });
    });


    editor.on('doubleclick', function(evt) {
      var element = evt.data.element;

      if (element.hasClass('multipleChoiceVariableMark')) {
        var parents = element.getParents();
        var parent;

        for (var i = 0; i < parents.length; i++) {
          parent = parents[i];
          if (parent.hasClass('multipleChoiceVariable')) {
            break;
          }
        }

        $('#multipleTopVariable').html('');

        //Move selection to parent of multipleChoiceMark
        eleMultipleChoice = parent;
        editor.getSelection().selectElement(eleMultipleChoice);
        evt.data.dialog = pluginName;
        //The status to editor know this is update
        isEditMultipleChoiceVariable = true;
        //Load data to popup
        currentMultipleChoiceResId = loadDataforMultipleChoiceVariable(eleMultipleChoice);

        dblickHandlerToolbar(editor);
      }
    });

    CKEDITOR.dialog.add(pluginName, function(editor) {

      myhtml = '<div class="divMultipleChoiceVariable">';
      myhtml += '    <div class="m-b-15" id="mcv-grading">';
      myhtml += '         <input type="radio" name="mcv-grading" id="mcv-grading-normal" checked/>';
      myhtml += '         <label for="mcv-grading-normal">Normal Grading</label>';
      myhtml += '         <input type="radio" name="mcv-grading" id="mcv-grading-algorithmic"/>';
      myhtml += '         <label for="mcv-grading-algorithmic">Algorithmic Grading</label>';
      myhtml += '         <input type="radio" name="mcv-grading" id="mcv-grading-informational-only"/>';
      myhtml += '         <label for="mcv-grading-informational-only">Informational Only</label>';
      myhtml += '    </div>';
      myhtml += '    <div class="m-b-15" id="mcv-display">';
      myhtml += '         <input type="radio" name="mcv-display" id="mcv-display-vertical" checked/>';
      myhtml += '         <label for="mcv-display-vertical">Vertical</label>';
      myhtml += '         <input type="radio" name="mcv-display" id="mcv-display-horizontal"/>';
      myhtml += '         <label for="mcv-display-horizontal">Horizontal</label>';
      myhtml += '         <input type="radio" name="mcv-display" id="mcv-display-grid"/>';
      myhtml += '         <label for="mcv-display-grid">Grid</label>';
      myhtml += '         <div id="mcv-display-grid-per-row" class="u-inline-block hide"> &nbsp;No. of columns <input type="text" id="mcv-grid-per-row" value="2" class="txtFullcreate"/></div>';
      myhtml += '    </div>';
      myhtml += '    <div id="mcv-point">';
      myhtml += '        <div class="fleft">Point value: <input type="text" class="point" value="1" /></div>';
      myhtml += '        <div class="fright divMoreOne"><input type="radio" autocomplete="off" name="selectionmutiple" checked="checked" id="select_single_variable"/><label class="label_select" for="select_single_variable">Single-select</label> <input type="radio" autocomplete="off" name="selectionmutiple" id="select_multi_variable"/><label class="label_select" for="select_multi_variable">Multi-select</label>';
      myhtml += '         <div class="clear"></div><div class="maxChoiceField">Max Choices: <input type="text" id="maxChoices" value="4"/></div>';
      myhtml += '         <div class="clear"></div>  ';
      myhtml += '        </div>';
      myhtml += '        <div class="clear10"></div>  ';
      myhtml += '    </div>';
      myhtml += '    <div class="box-dialog-mutiple">';
      myhtml += '         <div class="clear"></div>';
      myhtml += '         <div id="multipleTopVariable"></div>';
      myhtml += '         <div class="clear"></div>';
      myhtml += '         <ul id="multipleChoiceVariable">';
      myhtml += '         </ul>';
      myhtml += '         <div class="clear"></div>';
      myhtml += '         <div class="addMore">';
      myhtml += '         	<input type="button" class="ckbutton" id="bntAddChoiceVariable" value="Add choice" />';
      myhtml += '         </div>';
      myhtml += '         <div class="clear"></div>';
      myhtml += '         <div id="multipleBotVariable"></div>';
      myhtml += '         <div class="clear"></div>';
      myhtml += '    </div>';
      myhtml += '    <div class="clear10"></div>  ';
      myhtml += '</div>';

      return {
        title: 'Multiple Choice with Variable Points Properties',
        minWidth: 650,
        width: IS_V2 ? 800 : undefined,
        minHeight: 200,
        contents: [{
          id: 'multipleChoice',
          label: 'Settings',
          elements: [{
            type: 'html',
            html: myhtml,
            onLoad: function() {
              var $dialog = $(this.getDialog().getElement().$);

              for (k = 0; k < 4; k++) {
                addNewItemMultipleChoiceVariable();
                multipleChoiceSelectionVariable();
              }

              $("#bntAddChoiceVariable").click(function() {
                isAddAnswer = true;
                isAddAnswerLast = false;
                var editorID = addNewItemMultipleChoiceVariable();
                createCKEditorMultiVariable(editorID);
                multipleChoiceSelectionVariable();
              });

              handlerMultiSelect($dialog);
              handlerSingleSelect($dialog);
              handlerOnChangeGradingMethod($dialog);
              handlerOnChangeDisplayMethod($dialog);

              //load data into popup for first time if status is Edit
              if (isEditMultipleChoiceVariable == true) {
                //Checked first item as default when create multiple item
                $("#select_single_variable").prop("checked", true);
                loadDataforMultipleChoiceVariable(eleMultipleChoice);
              }
            },
            onShow: function() {
              var $dialog = $(this.getElement().$);
              var questionQtiSchemeId = CKEDITOR.instances[ckID].config.qtiSchemeID;

              //Check if text entry already exist. Don't allow create new one.
              if (isEditMultipleChoiceVariable == false) {
                createAnswerVariable();
              }
              //hide tooltip
              $('#tips .tool-tip-tips').css({
                'display': 'none'
              });

              getUpDownNumber('.point', 0, 1000);
              getUpDownNumber('#maxChoices', 2, 1000);
              getUpDownNumber('#mcv-grid-per-row', 2, 8);
              resetMultipleChoice($dialog);

              //Check multiple choice is single or multiple
              if (questionQtiSchemeId == '37') {
                $(".divMultipleChoiceVariable .divMoreOne").show();
                $($(".divMultipleChoiceVariable").parents(".cke_dialog_body")).find(".cke_dialog_title").text("Multiple Choice with Variable Points");
              } else if (questionQtiSchemeId == '21') {
                $(".divMultipleChoiceVariable .divMoreOne").show();
                $($(".divMultipleChoiceVariable").parents(".cke_dialog_body")).find(".cke_dialog_title").text("Multiple Choice with Variable Points");
              }

              refreshResponseId();

              // set arrIdResp null
              checkElementRemoveIntoIResult();

              $("#select_single_variable").prop("checked", true); //Check as default
              if (global.isSurvey == 1) {
                $dialog.find('#mcv-grading').hide();
                $dialog.find('#mcv-point').hide();
              }
              if (isEditMultipleChoiceVariable) {
                var $mcvMaxChoices = $('#maxChoices');
                $('iframe[allowtransparency]').contents().find('body').find('div.active-border').removeClass('active-border');
                //Only create CKEditor after html appended
                $("#multipleChoiceVariable li").each(function() {
                  createCKEditorMultiVariable("editor" + $(this).attr("id"));
                });

                for (i = 0; i < iResult.length; i++) {
                  if (iResult[i].responseIdentifier == currentMultipleChoiceResId && iResult[i].type == "choiceInteractionVariable") {
                    var isMultiple = iResult[i].responseDeclaration.cardinality === 'multiple';
                    //Clear all item before add new
                    if (isMultiple) {
                      $("#select_multi_variable").prop("checked", true); //Check as default
                      $(".divMultipleChoiceVariable .maxChoiceField").css({
                        "display": "block"
                      });
                    } else {
                      $(".divMultipleChoiceVariable .maxChoiceField").css({
                        "display": "none"
                      });
                    }

                    sChoice = iResult[i].simpleChoice;
                    for (m = 0; m < sChoice.length; m++) {


                      //Load data to each item
                      var currentItem = $("#multipleChoiceVariable li").eq(m);
                      CKEDITOR.instances["editor" + currentItem.attr("id")].setData(loadMathML(sChoice[m].value));

                      //Load mp3 to popup
                      if (sChoice[m].audioRef != undefined && sChoice[m].audioRef != "") {
                        currentItem.find(".bntUploadAudio").hide();
                        currentItem.find(".audioRemove").show().find(".audioRef").append(sChoice[m].audioRef);
                      }

                      //load point to item
                      currentItem.find(".point-item").val(sChoice[m].answerPoint);
                    }

                    $('.divMultipleChoiceVariable .point').val(iResult[i].responseDeclaration.pointsValue);
                    $('#bntAddChoiceVariable,.ckRemove').css('display', 'inline-block');

                    if (questionQtiSchemeId == '37') {
                      $(".divMultipleChoiceVariable .divMoreOne").show();
                    }

                    onShowGradingMethod($dialog, iResult[i].responseDeclaration.method);
                    onShowDisplayMethod($dialog, iResult[i].display, iResult[i].gridPerRow);

                    $mcvMaxChoices.val(iResult[i].maxChoices);

                    break;
                  }
                }
              }

              CKEDITOR.on('instanceReady', function(ev) {
                //Show the first toolbar when popup has created
                $("#multipleTopVariable > div").hide();
                $("#multipleTopVariable > div:first").show();

                var getData = ev.editor.getData();
                if (getData != "") {
                  ev.editor.setData(loadMathML(getData));
                }

                // If Users edit multiplechoice, this code fix double scroller - Thinh Le
                if (isEditMultipleChoiceVariable) {
                  var IdLastAnswer = $('ul#multipleChoiceVariable li').length;
                  var lastHtml = CKEDITOR.instances['editor' + $('ul#multipleChoiceVariable li:last-child').attr("id")].getData(); //get last text of li

                  $('ul#multipleChoiceVariable li').each(function(index, value) {
                    index = index + 1;
                    var ckID = $(this).attr('id');
                    var data = CKEDITOR.instances['editor' + ckID].getData();
                    if (data.trim() == '' && isAddAnswer == false) {
                      CKEDITOR.instances['editor' + ckID].setData('Answer ' + index);
                    } else if (index == IdLastAnswer) {
                      if (isAddAnswer) {
                        CKEDITOR.instances['editor' + ckID].setData('Answer ' + index);
                        var $multiplechoiceLast = $('ul#multipleChoiceVariable li').last();

                      }

                      if (isAddAnswerLast == true) {
                        CKEDITOR.instances["editor" + $('ul#multipleChoiceVariable li:last-child').attr("id")].setData(loadMathML(lastHtml));
                      }
                      isAddAnswer = false; //Reset status for add new Answer.
                    }
                  });
                } else {
                  var IdLastAnswer = $('ul#multipleChoiceVariable li').length;
                  $('ul#multipleChoiceVariable li').each(function(index, value) {

                    index = index + 1;
                    var ckID = $(this).attr('id');
                    var data = CKEDITOR.instances['editor' + ckID].getData();

                    if (data.trim() == '') {
                      CKEDITOR.instances['editor' + ckID].setData('Answer ' + index);
                    } else if (index == IdLastAnswer) { // update text or image when new create multip choice

                      if (isAddAnswer == true) {
                        CKEDITOR.instances['editor' + ckID].setData('Answer ' + index);
                        //fix last content for editor multip choice
                        var textContent = $('ul#multipleChoiceVariable li:last-child').find("textarea").attr("defaultText");
                        if (textContent != "") {
                          CKEDITOR.instances["editor" + $('ul#multipleChoiceVariable li:last-child').attr("id")].setData(loadMathML(textContent));
                        }
                      }

                    }
                  });
                }
                ev.editor.on('focus', function(evt) {
                  var idAnswer = evt.editor.name.replace('editor', '');
                  var me = $('#' + idAnswer).find('iframe[allowtransparency]').contents().find('body');
                  var $me = $(me);
                  var ind = $('ul#multipleChoiceVariable li').index($('#' + idAnswer)) + 1;

                    var ans = 'Answer ' + ind;
                    var newAns = $me.text();
                    var newNumber = newAns.replace(/[^0-9]/g, '');
                    var newString = newAns.replace(newNumber, '');
                    var mathjax = $me.find('.math-tex');

                    if ($me.text() === ans || newString.trim() === 'Answer' && !mathjax.length) {
                      $me.attr('defaultText', newAns);
                      var isenter = $me.attr('isenter');
                      if (isenter == 'true') {
                        return;
                      } else {
                        if (isAddImage) {
                          $me.text('');
                          return;
                        }
                        if ($me.find('img').hasClass('imageupload')) {
                          return;
                        }
                        $me.text('');

                      }

                    }
                });
                ev.editor.on('blur', function(evt) {
                  var idAnswer = evt.editor.name.replace('editor', '');
                  var ind = $('ul#multipleChoiceVariable li').index($('#' + idAnswer)) + 1;
                  var me = $('#' + idAnswer).find('iframe[allowtransparency]').contents().find('body');
                   var $me = $(me);
                    var $imageUpload = $me.find('img').hasClass('imageupload');
                    var newAns = $me.text();
                    var mathjax = $me.find('.math-tex');

                    if (isOk === true) {
                      //$(me).text('Answer ' + ind);// $(me).text(data); //set text default when no change input
                      if (newAns !== '' || $imageUpload) {
                        return;
                      } else {
                        $me.text($('#' + ckID).find('iframe[allowtransparency]').contents().find('body').attr('defaulttext'));
                      }

                    } else {
                      // Check zero-width spaces when remove all text
                      if (newAns === '\u200b' || newAns === '' && !mathjax.length) {
                        if ($imageUpload) {
                          return;
                        }
                        $me.text('Answer ' + ind); // set text when no press Ok
                        $me.text($('#' + ckID).find('iframe[allowtransparency]').contents().find('body').attr('defaulttext'));
                      }
                    }

                });
                //watermark
                var $ckeditorBody = $('#' + ckID).find('iframe[allowtransparency]').contents().find('body');
                $('.divMultipleChoiceVariable').css('display', 'table');
                setTimeout(function() {
                  $('.divMultipleChoiceVariable').css('display', 'block');
                }, 500)
                $('ul#multipleChoiceVariable li').each(function(index, value) {
                  var that = this;
                  var ind = index + 1;
                  var ckID = $(that).attr('id');
                  var data = CKEDITOR.instances['editor' + ckID].getData();

                  $ckeditorBody.on('keypress', function(e) {
                    $(e.target).attr('isEnter', 'true');
                  });
                });

              });
            }
          }]
        }],
        onOk: function() {
          var $dialog = $(this.getElement().$);
          var msgNotification = '';
          isAddAnswer = false;
          //Check incase user has checked in correct answer but they don't fill text
          var isEmpty = false,
            qValue = "";
          $answerItem = $("#multipleChoiceVariable li");

          var pointValue = $(".point").val();

          if (isEmpty) {
            customAlert("Please fill content for answer " + qValue);
            return false;
          }

          if ($answerItem.length == 1) {
            customAlert("Multiple choice question must be has at least 2 answers.");
            return false;
          }

          var cardinalityChoice = "single",
            maxMulChoices = 1,
            simpleMulChoice = [],
            htmlMulChoice = "",
            currentAnswerIndex = 0, // Store correct alphebet for question
            alphaBe = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H'];

          var gradingMethod = getGradingMethod($dialog);
          var displayMethod = getDisplayMethod($dialog);
          var gridPerRow = $dialog.find('#mcv-grid-per-row').val();

          if (CKEDITOR.instances[ckID].config.alphaBeta != undefined) {
            alphaBe = CKEDITOR.instances[ckID].config.alphaBeta;
          }
          var pointArr = [];
          var maxPointEarn = 0;
          $(".point-item").each(function () {
            pointArr.push($(this).val());
          });
          pointArr.sort(function (a, b) {
            return b - a
          });
          if (global.isSurvey == 1 && _.union(pointArr).length != pointArr.length) {
            msgNotification = 'The point values must be different.';
            customAlert(msgNotification);
            return false;
          }
          if ($("#select_multi_variable").is(':checked')) {
            cardinalityChoice = "multiple";
            maxMulChoices = parseInt($("#maxChoices").val(), 10);
            var mcvItemLength = $('#multipleChoiceVariable li').length;

            if (maxMulChoices <= 1 || mcvItemLength < maxMulChoices) {
              msgNotification = '<p style="text-align: left; margin-bottom: 0;">The maximum selectable answer choices for multi-select questions cannot be less than 2 and cannot exceed the maximum number of answer choices. Please adjust the value listed in the <b>Max choices</b> option.</p>';
              customAlert(msgNotification);
              return false
            }

            //Check max point
            for (var i = 0; i < maxMulChoices; i++) {
              if (parseInt(pointArr[i]) > 0) {
                maxPointEarn += parseInt(pointArr[i]);
              }
            }
  
            var maxPointAnswer = Math.max.apply(null, pointArr);

            if (parseInt(pointValue) < maxPointAnswer && gradingMethod !== 'algorithmic') {
              customAlert("No single answer choice can be worth more points than the overall question. Please lower the point value of the answer choice or increase the point value of the overall question.");
              return false;
            }

            if (parseInt(pointValue) < maxPointEarn && gradingMethod !== 'algorithmic') {
              var maxChoices = parseInt($('#maxChoices').val(), 10);
              msgNotification = '<p style="text-align: left; margin-bottom: 0;">If you compare the Overall Point Value (' + pointValue + '), Max Choices (' + maxChoices + '), and the individual point values allocated to each answer choice, you will see that it is possible to exceed the full credit on this question. You should 1) increase the Overall Point Value of the question, 2) decrease the Max Choices a student can select, and/or 3) decrease the individual point value(s) of some answer choices.</p>';
              customAlert(msgNotification);
              return false;
            }


            if (parseInt(pointValue) > maxPointEarn && gradingMethod !== 'algorithmic') {
              var maxChoices = parseInt($('#maxChoices').val(), 10);  
              msgNotification = '<p style="text-align: left; margin-bottom: 0;">If you compare the Overall Point Value  (' + pointValue + '), Max Choices (' + maxChoices + '), and the individual point values allocated to each answer choice, you will see that it is not possible to earn full credit on this question. You should 1) lower the Overall Point Value of the question, 2) increase the Max Choices a student can select, and/or 3) increase the individual point value(s) of some answer choices.</p>';
              customAlert(msgNotification);
              return false;
            }
          }

          var responseId = "";
          var largePoint = 0;
          var isValidPointAnswer = true;
          var totalPoint = 0;
          var maxLengthItemError = [];

          $answerItem.each(function(index) {

            var iddentify = alphaBe[currentAnswerIndex],
              pointAnswer = $(this).find(".point-item").val();
            editorId = $(this).find(".content textarea").attr("id"),
              item = {
                identifier: iddentify,
                value: CKEDITOR.instances[editorId].getData(),
                'answerPoint': pointAnswer
              },
              hasAudio = 'class="nonAudioIcon"',
              audioLink = '';
            hasCorrectAnwser = 'class="item"';


            //save text Default
            var textDefault = $(this).find('iframe[allowtransparency]').contents().find('body').attr('defaultText');

            //Only add item if item has content
            if (CKEDITOR.instances[editorId].getData() == "") {
              return;
            }
            var flatTextValue = $('<span>' + CKEDITOR.instances[editorId].getData() + '</span>').text();
            if (global.isSurvey == 1 && flatTextValue.length > 100) {
              maxLengthItemError.push(iddentify)
            }
            if ($(this).find(".audioRef").length == 1 && $(this).find(".audioRef").html() != "") {
              item.audioRef = $(this).find(".audioRef").text();
              hasAudio = 'class="audioIcon"';
              audioLink = $(this).find(".audioRef").html();
            }

            //Add item for multiple choice
            htmlMulChoice += '<div defaultText="' + textDefault + '" ' + hasCorrectAnwser + '><div ' + hasAudio + '><img alt="Play audio" class="imageupload bntPlay" src="' + CKEDITOR.plugins.getImgByVersion('multiplechoice', 'images/small_audio_play.png') + '" title="Play audio"><img alt="Stop audio" class="bntStop" src="' + CKEDITOR.plugins.getImgByVersion('multiplechoice', 'images/small_audio_stop.png') + '" title="Stop audio"><span class="audioRef">' + audioLink + '</span></div><div class="answer">' + item.identifier + '.</div> <div class="answerContent">' + item.value + '</div></div>';

            simpleMulChoice.push(item);

            currentAnswerIndex += 1;

            var itemPoint = parseInt(pointAnswer);
            if (pointAnswer.indexOf("-") > 0) {
              isValidPointAnswer = false;
            }

            if (index == 0) {
              largePoint = itemPoint;
            } else {
              if (largePoint < itemPoint) {
                largePoint = itemPoint
              }
            }

            if (itemPoint > 0) {
              totalPoint += itemPoint;
            }
          });

          if (global.isSurvey == 1 && maxLengthItemError.length) {
            msgNotification = '<p style="text-align: left; margin-bottom: 0;">The answer value for item(s) <strong>' + maxLengthItemError.join(', ') + '</strong> must be less than or equal to 100 characters.</p>';
            customAlert(msgNotification);
            return false;
          }

          if (isValidPointAnswer === false) {
            customAlert("Point's Answer invalid.");
            return false;
          }

          if (global.isSurvey == 0 && parseInt(largePoint) > parseInt(pointValue)) {
            customAlert("No single answer choice can be worth more points than the overall question. Please lower the point value of the answer choice or increase the point value of the overall question.");
            return false;
          }

          if ($("#select_single_variable").is(':checked') && parseInt(pointValue) > parseInt(largePoint) && gradingMethod !== 'algorithmic') {
            var maxChoices = parseInt($('#maxChoices').val(), 10);
            msgNotification = '<p style="text-align: left; margin-bottom: 0;">If you compare the overall Points value (' + pointValue + '), the Max Choices (' + maxChoices + '), and the individual point values allocated for each answer choice, you will see that the students will not be able to earn full credit on this question. You should either 1) lower the overall Points value on this question, 2) increase the maximum number of answer choices a student can select, and/or 3) increase the point values on some of the individual answer choices.</p>';
            customAlert(msgNotification);
            return false;
          }

          if (isEditMultipleChoiceVariable) {
            //Update for current textEntry
            for (n = 0; n < iResult.length; n++) {
              if (iResult[n].responseIdentifier == currentMultipleChoiceResId) {
                responseId = currentMultipleChoiceResId;
                iResult[n].responseDeclaration.pointsValue = pointValue;
                iResult[n].maxChoices = maxMulChoices;
                iResult[n].responseDeclaration.cardinality = cardinalityChoice;
                iResult[n].simpleChoice = simpleMulChoice;
                iResult[n].responseDeclaration.method = gradingMethod;
                iResult[n].display = displayMethod;

                if (displayMethod === 'grid') {
                  iResult[n].gridPerRow = gridPerRow;
                }
                break;
              }
            }
          } else {
            //Create response identify and make sure it doesn't conflict with current.
            responseId = createResponseId();

            var multiplechoicevariableResult = {
              type: 'choiceInteractionVariable',
              responseIdentifier: responseId,
              shuffle: 'false',
              maxChoices: maxMulChoices,
              responseDeclaration: {
                baseType: 'identifier',
                cardinality: cardinalityChoice,
                method: gradingMethod,
                caseSensitive: 'false',
                type: 'string',
                pointsValue: pointValue
              },
              simpleChoice: simpleMulChoice,
              display: displayMethod
            };

            if (displayMethod === 'grid') {
              multiplechoicevariableResult.gridPerRow = gridPerRow;
            }

            iResult.push(multiplechoicevariableResult);

            //Hide button on toolbar after add multiple choice incase qtItem is 1 or 3
            if (iSchemeID == "37") {
              $(".cke_button__multiplechoicevariable").parents("span.cke_toolbar").hide();
            }
          }


          htmlMulChoice = loadMathML(htmlMulChoice);

          $('iframe[allowtransparency]').contents().find('body').find('div.active-border').removeClass('active-border');

          var newMultipleChoice = '<div class="multipleChoice multipleChoiceVariable" id="' + responseId + '" title="' + responseId + '" contenteditable="false"><button class="single-click-variable">Click here to edit answer choices</button><img class="cke_reset cke_widget_mask multipleChoiceVariableMark" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" />' + htmlMulChoice + ' </div>&nbsp;';
          //Remove old multiple choice and add new one. This case just for safari only.
          if (CKEDITOR.env.safari && isEditMultipleChoiceVariable) {
            var oldHtml = $('iframe[allowtransparency]').contents().find('body').find('#' + responseId);
            oldHtml.replaceWith(newMultipleChoice); // cause duplicate responseId,so remove old
          } else {
            var spaces = '&#160;&#160;&#160;&#160;&#160;';
            editor.insertHtml(spaces);
            editor.insertHtml(newMultipleChoice);

            var $body = $(editor.window.getFrame().$).contents().find('body');
            var content = $body.html().replaceAll('&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;', '');
            $body.html(content);
          }
          //set event click play audio
          //play audio
          var $ckeditorContents = $(editor.window.getFrame().$).contents();
          var btnPlay = $ckeditorContents.find('.audioIcon .bntPlay');
          btnPlay.unbind("click").on('click', function(e) {
            $ckeditorContents.find('.audioIcon .bntStop').hide();
            $ckeditorContents.find('.audioIcon .bntPlay').show();
            resetUIAudio();
            var audioUrl = $(this).parent().find(".audioRef").text();
            playAudio(this, audioUrl);
          });
          //stop audio
          var bntStop = $ckeditorContents.find('.audioIcon .bntStop');
          bntStop.unbind("click").on('click', function(e) {
            stopAudio(this);
          });

          //set single click popup
          var singleclick = $ckeditorContents.find('.multipleChoiceVariable .single-click-variable');
          singleclick.unbind("click").on('click', function(e) {
            var element = e.target.parentElement;
            //Move selection to parent of multipleChoiceMark
            if (CKEDITOR.env.safari) {
              editor.getSelection().getSelectedElement();
            } else {
              editor.getSelection().selectElement(editor.document.getActive().getParent());
            }

            $('#multipleTopVariable').html('');
            //The status to editor know this is update
            isEditMultipleChoiceVariable = true;
            editor.openDialog('insertMultipleChoiceVariable', function() {
              currentMultipleChoiceResId = loadDataforMultipleChoiceVariableSingleClick(element);
            });
          });

          stopVNSAudio();
          isEditMultipleChoiceVariable = false;
          setDefault = true;
          currentMultipleChoiceResId = "";
          isOk = true;
          ResIdElemModul = responseId;

          newResult = iResult;

          if (gradingMethod === 'algorithmic') {
            TestMakerComponent.isShowAlgorithmicConfiguration = true;
          } else {
            TestMakerComponent.isShowAlgorithmicConfiguration = false;
          }
        },
        onCancel: function() {
          //Reset to default after update or create new textEntry
          stopVNSAudio();
          isEditMultipleChoiceVariable = false;
          currentMultipleChoiceResId = "";
        }
      };
    });
  }
});

/***
 * Load data to Draw Tool popup form
 * Return: responseIdentifier
 ***/
function loadDataforMultipleChoiceVariable(element) {
  var currentMultipleChoiceResId = "";
  var sChoice = [];
  var idCurr = "";
  if (element.getId == undefined) {
    idCurr = element.id;
  } else {
    idCurr = element.getId();
  }

  for (i = 0; i < iResult.length; i++) {
    if (iResult[i].responseIdentifier == idCurr && iResult[i].type == "choiceInteractionVariable") {


      currentMultipleChoiceResId = iResult[i].responseIdentifier;
      //Load data

      $(".divMultipleChoiceVariable .point").val(iResult[i].responseDeclaration.pointsValue);

      //Clear all item before add new
      $("#multipleChoiceVariable").empty();
      sChoice = iResult[i].simpleChoice;
      for (m = 0; m < sChoice.length; m++) {
        addNewItemMultipleChoiceVariable();
        multipleChoiceSelectionVariable();

      }

      break;
    }
  }

  return currentMultipleChoiceResId;
}

/***
 * Load data
 * Return: responseIdentifier
 ***/
function loadDataforMultipleChoiceVariableSingleClick(ele) {
  var currentMultipleChoiceResId = "";
  var sChoice = [];
  for (var i = 0; i < iResult.length; i++) {
    if (iResult[i].responseIdentifier == ele.id && iResult[i].type == "choiceInteractionVariable") {
      currentMultipleChoiceResId = iResult[i].responseIdentifier;
      //Load data

      $(".divMultipleChoiceVariable .point").val(iResult[i].responseDeclaration.pointsValue);

      //Clear all item before add new
      $("#multipleChoiceVariable").empty();
      sChoice = iResult[i].simpleChoice;
      for (var m = 0; m < sChoice.length; m++) {
        addNewItemMultipleChoiceVariable();
        multipleChoiceSelectionVariable();
      }
    }
  }
  return currentMultipleChoiceResId;
}
/***
 * Function to add new an anwser for multiple Choice
 * Return EditorID this will help to create CKEditor
 ***/
function addNewItemMultipleChoiceVariable() {
  var ckeditorPath = CKEDITOR.plugins.get('multiplechoicevariable').path;
  var multiChoiceLength = $("#multipleChoiceVariable li").length;
  var alphabet = [];
  if (CKEDITOR.instances[ckID].config.alphaBeta != undefined) {
    alphabet = CKEDITOR.instances[ckID].config.alphaBeta;
  }

  //maximum answer for multipleChoise is 25 answers
  if (multiChoiceLength > 40) {
    return;
  }

  var getImgByVersion = CKEDITOR.plugins.getImgByVersion;

  var now = $.now(),
    editorId = 'editor' + alphabet[multiChoiceLength] + now;

  var strHTML = '<li id="' + alphabet[multiChoiceLength] + now + '">';

  strHTML += '<div class="alphabet">' + alphabet[multiChoiceLength] + '</div>';
  strHTML += '<div class="content">';
  strHTML += '<textarea cols="100" id="' + editorId + '" name="editor1" rows="1" tabindex="' + (multiChoiceLength + 1) + '"></textarea>';
  strHTML += '</div>';
  strHTML += '<div class="point-question-item"><input type="text" class="point-item" value="0" maxnumber="1000" minnumber="0" /></div>';
  strHTML += '<div class="sort actions">';
  strHTML += '<div class="audio">';
  strHTML += '<div class="bntUploadAudio">';
  strHTML += '<form name="form-upload-' + alphabet[multiChoiceLength] + now + '" id="form-upload-' + alphabet[multiChoiceLength] + now + '" lang="en" dir="ltr" action="uploader.php?type=mp3" method="POST" enctype="multipart/form-data">';
  strHTML += '    <input type="file" name="file" class="hiddenUpload" accept="audio/mp3" size="60" />';
  strHTML += '    <input type="hidden" name="id" />';
  strHTML += '</form>';
  strHTML += '<input type="image" src="' + getImgByVersion('multiplechoice', 'images/audio-add.png') + '" class="addAudio" title="Add audio" />';
  strHTML += '</div>';
  strHTML += '<div class="audioRemove">';
  strHTML += '    <img alt="Play audio" class="bntPlay" src="' +getImgByVersion('multiplechoice', 'images/small_audio_play.png') + '" title="Play audio" />';
  strHTML += '    <img alt="Stop audio" class="bntStop" src="' + getImgByVersion('multiplechoice', 'images/small_audio_stop.png') + '" title="Stop audio" />';
  strHTML += '    <input type="image" src="' + getImgByVersion('multiplechoice', 'images/audio-remove.png') + '" class="removeAudio" title="Remove audio" />';
  strHTML += '    <span class="audioRef"></span>';
  strHTML += '    <div class="clear"></div>';
  strHTML += '</div>';
  strHTML += '</div>';
  strHTML += '<input type="image" title="Move Up" src="' + getImgByVersion('multiplechoice', 'images/up.png') + '" class="ckImageButton ckUp" />';
  strHTML += '<input type="image" title="Move Down" src="' + getImgByVersion('multiplechoice', 'images/down.png') + '" class="ckImageButton ckDown" />';
  strHTML += '<input type="image" title="Remove" src="' + getImgByVersion('multiplechoice', 'images/remove.png') + '" class="ckImageButton ckRemove" />';
  strHTML += '</div>';
  strHTML += '<div class="clear"></div>';
  strHTML += '</li>';

  $("#multipleChoiceVariable").append(strHTML);
  IS_V2 && $("#multipleChoiceVariable").find('[title]').tip();

  return editorId;
}

/***
 * Function to register all events for multiple choice
 ***/
function multipleChoiceSelectionVariable() {
  $("#multipleChoiceVariable .audio .addAudio").unbind("click").click(function() {

    var $elm = $('a#overlink');
    if (document.createEvent) {
      var e = document.createEvent('MouseEvents');
      e.initEvent('click', true, true);
      $(this).parent().find(".hiddenUpload").val("").get(0).dispatchEvent(e);
    } else {
      $(this).parent().find(".hiddenUpload").val("").trigger("click");
    }


  });

  $("#multipleChoiceVariable .audio .hiddenUpload").unbind("change").change(function() {

    var file = this.value;
    var extension = file.substr((file.lastIndexOf('.') + 1));

    if (extension.toLowerCase() != "mp3") {
      customAlert('Unsupported file type. Please select mp3 file.');
      return;
    }

    $(this).parent().children("input[name='id']").val(objectId);
    audioUploadMultipleChoiceVariable($(this).parent().get(0), audioConfig, this);

  });

  $(".bntPlay").unbind("click").click(function() {
    $(CKEDITOR.instances[ckID].window.getFrame().$).contents().find('.audioIcon .bntStop').hide();
    $(CKEDITOR.instances[ckID].window.getFrame().$).contents().find('.audioIcon .bntPlay').show();
    resetUIAudio();

    var me = this;
    $(me).next().show();
    $(me).hide();

    var audioUrl = $(this).parent().find(".audioRef").text();
    audioUrl = audioUrl.substring(0, audioUrl.lastIndexOf("/")) + "|" + audioUrl.substring(audioUrl.lastIndexOf("/") + 1, audioUrl.length);

    if (window.playsound != undefined) {
      window.playsound.pause();
    }

    var audioLink = "";
    if (CKEDITOR.instances[ckID].config.playAudio != undefined) {
      audioLink = CKEDITOR.instances[ckID].config.playAudio;
    }

    window.playsound = new vnsAudio({
      src: audioLink + audioUrl,
      onEnded: function() {
        $(me).next().hide();
        $(me).show();
      }
    });


  });

  $(".bntStop").unbind("click").click(function() {
    $(this).prev().show();
    $(this).hide();
    if (window.playsound != undefined) {
      window.playsound.pause();
    }
  });

  $(".removeAudio").unbind("click").click(function() {
    $(this).parents(".audioRemove").hide();
    $(this).parents(".audio").find(".bntUploadAudio").show();
    $(this).next().empty(); // Remove url of audio
    if (window.playsound != undefined) {
      window.playsound.pause();
    }
  });

  $(".ckRemove").unbind("click").click(function() {

    if ($("#multipleChoiceVariable li").length == 1) {
      return;
    }
    var answer = $(this).parent().parent(),
      instanceName = answer.find("textarea").attr("id");
    index = answer.index();
    if (CKEDITOR.instances[instanceName]) CKEDITOR.instances[instanceName].destroy();
    answer.remove();
    sortItemMultipleChoice(index);
    //Show the first CK Toolbar when remove an item
    $("#multipleTopVariable > div").hide();
    $("#multipleTopVariable > div:first").show();

  });

  $(".ckUp").unbind("click").click(function(event) {
    event.preventDefault();

    var parent = $(this).parents("li");
    if (parent.index() == 0) {
      return;
    }
    isAddAnswerLast = false;
    if ($(this).parents("li").index() == ($("#multipleChoiceVariable li").length - 1)) {
      isAddAnswerLast = true;
    }

    editorId = 'editor' + parent.attr("id");
    CKEDITOR.instances[editorId].destroy();


    //Change alphabet of anwser
    parent.prev().find(".alphabet").empty().html(alphabet[parent.index()]);
    parent.prev().find("textarea").attr({
      "tabindex": parent.index() + 1
    });
    //Change alphabet of anwser
    parent.find(".alphabet").empty().html(alphabet[parent.index() - 1]);
    parent.find("textarea").attr({
      "tabindex": parent.index()
    });


    parent.insertBefore(parent.prev());
    createCKEditorMultiVariable(editorId);

    //Show the first CK Toolbar when remove an item
    $("#multipleTopVariable > div:first").show();
  });

  $(".ckDown").unbind("click").click(function(event) {
    event.preventDefault();
    isDownUp = true;
    var parent = $(this).parents("li");
    var textContent = parent.find('.content').find('iframe[allowtransparency]').contents().find('body').text();
    if (parent.index() == $("#multipleChoiceVariable li").length - 1) {
      return;
    }

    editorId = 'editor' + parent.attr("id");
    CKEDITOR.instances[editorId].destroy();

    ////Change alphabet of anwser
    parent.next().find(".alphabet").empty().html(alphabet[parent.index()]);
    parent.next().find("textarea").attr({
      "tabindex": parent.index() + 1
    });
    //Change alphabet of anwser
    parent.find(".alphabet").empty().html(alphabet[parent.index() + 1]);
    parent.find("textarea").attr({
      "tabindex": parent.index() + 2
    });
    parent.find("textarea").attr("defaultText", textContent);

    parent.insertAfter(parent.next());
    createCKEditorMultiVariable(editorId);

    //Show the first CK Toolbar when remove an item
    $("#multipleTopVariable > div:first").show();
  });

  getUpDownNumber($('.point-item'), -100, 100);

  if ($('#mcv-grading-algorithmic').is(':checked')) {
    $('.point-item').parent().addClass('is-disabled');
  }
};

/***
 * Function to sort when remove an anwser of multiple Choice
 ***/
function sortItemMultipleChoice(index) {
  for (i = index; i < $("#multipleChoiceVariable li").length; i++) {
    currentAnwser = $("#multipleChoiceVariable li").eq(i);
    currentAnwser.find(".alphabet").empty().html(alphabet[i]);
    currentAnwser.find("textarea").attr({
      "tabindex": i + 1
    });
  }
}

//Create new 4 anwsers
function createAnswerVariable() {
  $("#multipleChoiceVariable li").each(function() {
    var myId = "editor" + this.id;
    if (CKEDITOR.instances[myId]) CKEDITOR.instances[myId].destroy();
  });
  $("#multipleChoiceVariable").empty();

  //Create default (4)item for inlineChoice
  for (var k = 0; k < 4; k++) {
    var editorID = addNewItemMultipleChoiceVariable();

    multipleChoiceSelectionVariable();
    createCKEditorMultiVariable(editorID);
    $('#bntAddChoiceVariable,.ckRemove').css('display', 'inline-block');
  }

  $(".divMultipleChoiceVariable .point").val("1");
  $("#multipleTopVariable > div:first").show();
}

function createCKEditorMultiVariable(ckId) {
  try {
    CKEDITOR.instances[ckId].destroy(true);
  } catch (e) {}

  CKEDITOR.replace(ckId, {
    extraPlugins: 'mathjax,sharedspace,mathfraction,glossary,tabspaces,leaui_formula',
    toolbar: [
      ['JustifyLeft', 'JustifyCenter', 'JustifyRight', '-', 'NumberedList', 'BulletedList'],
      ['Bold', 'Italic', 'Underline'],
      ['Sameline'],
      ['Mathfraction'],
      ['FontSize', 'Subscript', 'Superscript', '-', 'RemoveFormat'],
      ['ImageUpload', 'SpecialChar', 'Mathjax', 'LeauiFormula', 'Glossary'],
      ['Indent','Outdent','Tabspaces']
    ],
    sharedSpaces: {
      top: 'multipleTopVariable'
    },
    extraAllowedContent: 'span[stylename]; span(smallText,normalText,largeText,veryLargeText);',
    height: 60,
    width: '100%',
    resize_dir: 'both',
    resize_minWidth: 'calc(100% - 250px)'
  });
}

//Play audio
function playAudio($this, audioUrl) {
  var me = $this;
  $(me).next().show();
  $(me).hide();

  audioUrl = audioUrl.substring(0, audioUrl.lastIndexOf("/")) + "|" + audioUrl.substring(audioUrl.lastIndexOf("/") + 1, audioUrl.length);

  if (window.playsound != undefined) {
    window.playsound.pause();
  }

  var audioLink = "";
  if (CKEDITOR.instances[ckID].config.playAudio != undefined) {
    audioLink = CKEDITOR.instances[ckID].config.playAudio;
  }

  window.playsound = new vnsAudio({
    src: audioLink + audioUrl,
    onEnded: function() {
      $(me).next().hide();
      $(me).show();
    }
  });
}

//stop audio
function stopAudio($this) {
  var me = $this;
  $(me).prev().show();
  $(me).hide();
  if (window.playsound != undefined) {
    window.playsound.pause();
  }
}

var iframeId = document.getElementById("upload_iframe");

function audioUploadMultipleChoiceVariable(form, action_url, currentElement) {
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
  var iframe = document.createElement("iframe");
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
  var eventHandler = function() {
    //hide overlay
    if (IS_V2) {
      $('body').unblock();
    } else {
      $("body").ckOverlay.destroy();
    }

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
    $(currentElement).parents(".audio").find(".audioRemove").show();
    $(currentElement).parents(".audio").find(".audioRef").text(data.url);
    $(currentElement).parents(".bntUploadAudio").hide();
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

function getUpDownNumber(selector, min, max) {
  var $selector = $(selector);

  $selector.ckUpDownNumber({
    minNumber: min,
    maxNumber: max,
    width: 18,
    height: 13
  });
}
