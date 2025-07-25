CKEDITOR.plugins.add('multiplechoice', {
  lang: 'en', // %REMOVE_LINE_CORE%
  icons: 'multiplechoice',
  requires: 'dialog',
  hidpi: true, // %REMOVE_LINE_CORE%
  init: function(editor) {
    var pluginName = 'insertMultipleChoice';
    var isStatusChecked = '';

    editor.addCommand(pluginName, new CKEDITOR.dialogCommand(pluginName));

    editor.ui.addButton('MultipleChoice', {
      label: 'Multi-Select',
      command: pluginName,
      icon: this.path + 'icons/multiplechoice.png',
      toolbar: 'insertMultipleChoice,30'
    });

    editor.widgets.add('multiplechoice', {
      inline: false,
      mask: false,
      allowedContent: {
        div: {
          classes: 'multipleChoice,item,audioIcon,multipleChoiceMark',
          attributes: '!id,name,contenteditable'
        }
      },
      template: '<div class="multipleChoice"></div>'
    });

    var isEditMultipleChoice = false,
      currentMultipleChoiceResId = "",
      isAddAnswer = false,
      eleMultipleChoice = ""; //This use for first time load popup
    var isOk = false;
    var isEditMultipleChoiceSingle = false;
    var txtTrueFalse = document.URL.split('&')[2];

    var resetGradingAlgorithmic = function(dialog, isShow) {
      var $dialog = $(dialog);
      var $mcCorrectAnswer = $dialog.find('#multipleChoice .correctAnswer input[type=checkbox]');

      $mcCorrectAnswer.prop('checked', false).prop('disabled', true);
      $dialog.find('.point').parent().addClass('is-disabled');
      $dialog.find('.mc-thresholds').hide();
      $dialog.find('#cbThreshold').prop('disabled', true).prop('checked', false);

      if (!isShow) {
        $dialog.find('.point').val('0');
      }
    };

    var resetGradingNormal = function(dialog) {
      var $dialog = $(dialog);
      var $mcCorrectAnswer = $dialog.find('#multipleChoice .correctAnswer input[type=checkbox]');

      $mcCorrectAnswer.prop('checked', false).prop('disabled', false);
      $mcCorrectAnswer.first().prop('checked', true);
      $dialog.find('.point').parent().removeClass('is-disabled');
      $dialog.find('.point').val('1');
      $dialog.find('#cbThreshold').prop('disabled', false);
    };

    var resetMultipleChoice = function(dialog) {
      var $dialog = $(dialog);

      $dialog.find('#mc-grading-normal').prop('checked', true);
      $dialog.find('#select_single').prop('checked', true);
      $dialog.find('.maxChoiceField').hide();
      $dialog.find('#cbThreshold').parent().hide();
      $dialog.find('#mc-grading-normal').prop('checked', true);
      $dialog.find('#mc-display-vertical').prop('checked', true);
      $dialog.find('#mc-display-grid-per-row').addClass('hide');
      $dialog.find('#mc-grid-per-row').val(2);

      resetGradingNormal();
    };

    var resetGradingInformationalOnly = function(dialog) {
      var $dialog = $(dialog);
      var $mcCorrectAnswer = $dialog.find('#multipleChoice .correctAnswer input[type=checkbox]');

      $mcCorrectAnswer.prop('checked', false).prop('disabled', true);
      $dialog.find('.point').parent().addClass('is-disabled');
      $dialog.find('.point').val('0');
      $dialog.find('.mc-thresholds').hide();
      $dialog.find('#cbThreshold').prop('disabled', true).prop('checked', false);
    };

    var resetGradingInformationalOnlyConfirm = function(msg, w, h, dialog) {
      var $dialog = $(dialog);
      var now = new Date().getTime();
      var msgHtml = '';
      var maxIndex;
      var zIndexArr = [];
      var $ckeDialog = $('.cke_dialog');

      w = w != null ? w : 400;
      h = h != null ? h : 100;

      msgHtml += '<div class="popup-alert">';
      msgHtml += '<div class="popup-alert-border">';
      msgHtml += '<div class="popup-alert-content">';
      msgHtml += '<table class="popup-alert-table">';
      msgHtml += '<tr>';
      msgHtml += '<td><p>' + msg + '</p></td>';
      msgHtml += '</tr>';
      msgHtml += '<tr>';
      msgHtml += '<td>';
      msgHtml += '<div class="popup-alert-controls">';
      msgHtml += '<button id="btn-yes-' + now + '">Continue</button>';
      msgHtml += '<button id="btn-cancel-' + now + '">Cancel</button>';
      msgHtml += '</div>';
      msgHtml += '</td>';
      msgHtml += '</tr>';
      msgHtml += '</table>';
      msgHtml += '</div>';
      msgHtml += '</div>';
      msgHtml += '</div>';

      $('<div/>')
        .html(msgHtml)
        .attr({
          'id': 'popup-alert-' + now,
          'class': 'dialog popup-ckeditor-v2'
        })
        .appendTo('body')
        .dialog({
          open: function() {
            $(this).parent().find('.ui-dialog-titlebar-close').remove();
          },
          modal: true,
          width: w,
          maxheight: h,
          resizable: false
        });

      // Check when ckeditor dialog exist
      if ($ckeDialog.length) {
        $ckeDialog.each(function(ind, dialog) {
          var $dialog = $(dialog);
          var zIndex = $dialog.css('z-index');

          zIndexArr.push(zIndex);
        });

        maxIndex = Math.max.apply(Math, zIndexArr);
      }

      // Ui dialog appear
      var $uiDialog = $('.ui-dialog');
      $uiDialog.css('height', 'auto');

      if (maxIndex) {
        $uiDialog.css('z-index', maxIndex + 2);
        $('.ui-widget-overlay').css('z-index', maxIndex + 1);
      }

      $(document).on('click', '#btn-yes-' + now, function() {
        $(document).find('#popup-alert-' + now).dialog('destroy').remove();
        $('.ui-widget-overlay').remove();
        resetGradingInformationalOnly($dialog);
      });

      $(document).on('click', '#btn-cancel-' + now, function() {
        $(document).find('#popup-alert-' + now).dialog('destroy').remove();
        if (isStatusChecked === 'algorithmic') {
          $dialog.find('#mc-grading-algorithmic').prop('checked', true);
        } else {
          $dialog.find('#mc-grading-normal').prop('checked', true);
          resetGradingNormal(dialog);
        }
        $('.ui-widget-overlay').remove();
      });
    };

    var handlerOnChangeGradingMethod = function(dialog) {
      dialog.find('input[type="radio"][name="mc-grading"]').on('change', function() {
        var $grading = $(this);
        var gradingMethod = $grading.attr('id');

        if (gradingMethod === 'mc-grading-algorithmic') {
          isStatusChecked = 'algorithmic';
          resetGradingAlgorithmic(dialog);
        } else if (gradingMethod === 'mc-grading-informational-only') {
          var msg = 'Points possible will be set to 0 and correct answer choices will be disabled, would you like to continue?';
          resetGradingInformationalOnlyConfirm(msg, 400, 100, dialog);
        } else {
          isStatusChecked = 'normal';
          resetGradingNormal(dialog);
        }
      });
    };

    var handlerOnChangeDisplayMethod = function(dialog) {
      dialog.find('input[type="radio"][name="mc-display"]').on('change', function() {
        var $display = $(this);
        var displayMethod = $display.attr('id');
        var $displayGridPerRow = dialog.find('#mc-display-grid-per-row');

        if (displayMethod === 'mc-display-grid') {
          $displayGridPerRow.removeClass('hide');
        } else {
          $displayGridPerRow.addClass('hide');
        }
      });
    };

    var getGradingMethod = function(dialog) {
      if (dialog.find('#mc-grading-algorithmic').is(':checked')) {
        return 'algorithmic';
      }

      if (dialog.find('#mc-grading-informational-only').is(':checked')) {
        return 'informational-only';
      }

      return 'default';
    };

    var getDisplayMethod = function(dialog) {
      if (dialog.find('#mc-display-horizontal').is(':checked')) {
        return 'horizontal';
      }

      if (dialog.find('#mc-display-grid').is(':checked')) {
        return 'grid';
      }

      return 'vertical';
    };

    var onShowGradingMethod = function(dialog, gradingMethod) {
      if (gradingMethod === 'algorithmic') {
        dialog.find('#mc-grading-algorithmic').prop('checked', true);
        resetGradingAlgorithmic(dialog, true);
      } else if (gradingMethod === 'informational-only') {
        dialog.find('#mc-grading-informational-only').prop('checked', true);
        resetGradingInformationalOnly(dialog);
      } else {
        dialog.find('#mc-grading-normal').prop('checked', true);
      }
    };

    var onShowDisplayMethod = function(dialog, displayMethod, gridPerRow) {
      if (displayMethod === 'horizontal') {
        dialog.find('#mc-display-horizontal').prop('checked', true);
      } else if (displayMethod === 'grid') {
        dialog.find('#mc-display-grid').prop('checked', true);
        dialog.find('#mc-grid-per-row').val(gridPerRow);
        dialog.find('#mc-display-grid-per-row').removeClass('hide');
      } else {
        dialog.find('#mc-display-vertical').prop('checked', true);
      }
    };

    var getNotCheckMultipleChoice = function(dialog) {
      if (dialog.find('#multipleChoice li .correctAnswer input[type=checkbox]').is(':checked')) {
        return false;
      }

      if (dialog.find('#mc-grading-algorithmic').is(':checked')) {
        return false;
      }

      if (dialog.find('#mc-grading-informational-only').is(':checked')) {
        return false;
      }

      return true;
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
      var singleclick = $(editor.window.getFrame().$).contents().find('.single-click');
      singleclick.unbind("click").on('click', function(e) {

        var element = $(e.target).parents('.multipleChoice')[0];

        if(!element) return;

        var txtTypeTrueFalse = element.getAttribute('texttruefalse'); // get type truefalse or mutiple choice

        //Move selection to parent of multipleChoiceMark
        if (CKEDITOR.env.safari) {
          editor.getSelection().getSelectedElement();
        } else {
          editor.getSelection().selectElement(editor.document.getById(element.getAttribute("id")));
        }


        // $('#multipleTop').html('');
        //The status to editor know this is update
        isEditMultipleChoice = true;
        isEditMultipleChoiceSingle = true;
        isEditMultipChoiceGuidance = true;
        eleMultipleChoice = element;

        //show type for mutiple part
        if (txtTypeTrueFalse == "TrueFalse") {
          isTrueFalse = true;
        } else {
          isTrueFalse = false;
        }

        editor.openDialog(pluginName, function() {
          currentMultipleChoiceResId = loadDataforMultipleChoice(element);
          idSimpleChoicesPopup = currentMultipleChoiceResId;
        });
      });
    });


    editor.on('doubleclick', function(evt) {
      var element = evt.data.element;

      if (element.hasClass('multipleChoiceMark')) {
        var parents = element.getParents();
        var parent;

        for (var i = 0; i < parents.length; i++) {
          parent = parents[i];
          if (parent.hasClass('multipleChoice')) {
            break;
          }
        }

        // $('#multipleTop').html('');
        //Move selection to parent of multipleChoiceMark
        eleMultipleChoice = parent;
        editor.getSelection().selectElement(eleMultipleChoice);
        evt.data.dialog = pluginName;
        //The status to editor know this is update
        isEditMultipleChoice = true;
        isEditMultipChoiceGuidance = true;

        //Load data to popup
        currentMultipleChoiceResId = loadDataforMultipleChoice(eleMultipleChoice);
        idSimpleChoicesPopup = currentMultipleChoiceResId;

        dblickHandlerToolbar(editor);
      }
    });

    CKEDITOR.dialog.add(pluginName, function(editor) {

      myhtml = global.isSurvey == 0 ?
                '<div class="divMultipleChoice">' :
                '<div class="divMultipleChoice survey-test">';
      myhtml += '    <div class="m-b-15" id="mc-grading">';
      myhtml += '         <input type="radio" name="mc-grading" id="mc-grading-normal" checked/>';
      myhtml += '         <label for="mc-grading-normal">Normal Grading</label>';
      myhtml += '         <input type="radio" name="mc-grading" id="mc-grading-algorithmic"/>';
      myhtml += '         <label for="mc-grading-algorithmic">Algorithmic Grading</label>';
      myhtml += '         <input type="radio" name="mc-grading" id="mc-grading-informational-only"/>';
      myhtml += '         <label for="mc-grading-informational-only">Informational Only</label>';
      myhtml += '    </div>';
      myhtml += '    <div class="m-b-15" id="mc-display">';
      myhtml += '         <input type="radio" name="mc-display" id="mc-display-vertical" checked/>';
      myhtml += '         <label for="mc-display-vertical">Vertical</label>';
      myhtml += '         <input type="radio" name="mc-display" id="mc-display-horizontal"/>';
      myhtml += '         <label for="mc-display-horizontal">Horizontal</label>';
      myhtml += '         <input type="radio" name="mc-display" id="mc-display-grid"/>';
      myhtml += '         <label for="mc-display-grid">Grid</label>';
      myhtml += '         <div id="mc-display-grid-per-row" class="u-inline-block hide"> &nbsp;No. of columns <input type="text" id="mc-grid-per-row" value="2" class="txtFullcreate"/></div>';
      myhtml += '    </div>';
      myhtml += '    <div>';
      myhtml += '        <div class="fleft">Point value: <input type="text" class="point" value="1" /><br/><br/>';
      myhtml += '            <label class="mc-widthLabel" for="cbThreshold"><input type="checkbox" value="1" id="cbThreshold" name="cbThreshold"> Partial Credit Grading</label>';
      myhtml += '            <div class="u-m-t-10 mc-thresholds is-hidden">';
      myhtml += '                <button class="cke_dialog_ui_button js-btn-mc-add-threshold"><span class="cke_dialog_ui_button">Add Threshold</span></button>';
      myhtml += '            </div>';
      myhtml += '        </div>';
      myhtml += '        <div class="fright divMoreOne"><input type="radio" autocomplete="off" name="selectionmutiple" id="select_single"/><label class="label_select" for="select_single">Single-select</label> <input type="radio" autocomplete="off" name="selectionmutiple" id="select_multi"/><label class="label_select" for="select_multi">Multi-select</label>';
      myhtml += '         <div class="clear"></div><div class="maxChoiceField">Max Choices: <input type="text" id="maxChoices" value="4"/></div>';
      myhtml += '         <div class="clear"></div>  ';
      myhtml += '        </div>';
      myhtml += '        <div class="clear10"></div>  ';
      myhtml += '    </div>';
      myhtml += '    <div class="p-t-10 mc-thresholds is-hidden">';
      myhtml += '        <div class="thresholds">';
      myhtml += '            <h3>List threshold</h3>';
      myhtml += '            <table class="table-threshold">';
      myhtml += '                <thead>';
      myhtml += '                    <tr>';
      myhtml += '                        <th>Low</th>';
      myhtml += '                        <th>High</th>';
      myhtml += '                        <th>Points</th>';
      myhtml += IS_V2 ? '<th>Delete</th>' : '<th></th>';
      myhtml += '                    </tr>';
      myhtml += '                </thead>';
      myhtml += '                <tbody></tbody>';
      myhtml += '            </table>';
      myhtml += '        </div>';
      myhtml += '        <div class="clear10"></div>';
      myhtml += '    </div>';
      myhtml += '    <div class="box-dialog-mutiple">';
      myhtml += '         <div class="clear"></div>';
      myhtml += '         <div id="multipleTop"></div>';
      myhtml += '         <div class="clear"></div>';
      myhtml += '         <ul id="multipleChoice">';
      myhtml += '         </ul>';
      myhtml += '         <div class="clear"></div>';
      myhtml += '         <div class="addMore">';
      myhtml += '         	<input type="button" class="ckbutton" id="bntAddChoice" value="Add choice" />';
      myhtml += '         </div>';
      myhtml += '         <div class="clear"></div>';
      myhtml += '         <div id="multipleBot"></div>';
      myhtml += '         <div class="clear"></div>';
      myhtml += '    </div>';
      myhtml += '    <div class="clear10"></div>  ';
      myhtml += '</div>';
      myhtml += '<div style="display: none;" class="popup-image-hotspot popup_duidance" id="popupGuidance"></div>';
      myhtml += '<div style="display: none;" class="popup-overlay" id="popupGuidanceOverlay"></div>';

      /**
       * Get integer number
       * @param  {[type]} number [description]
       * @return {[type]}        [description]
       */
      function getIntegerNumber(number) {
        return parseInt(number, 10);
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
       * Add item threshold
       * @param {[type]} container [description]
       */
      function addItemThreshold(container) {
        var $container = $(container);

        $container.append(createItemThreshold());
        $container.find('.js-threshold-remove').on('click', function() {
          removeItemThreshold(this);
        });
        supportOnlyNumberic($container.find('td input[type="text"]'));
        supportTabThreshold($container);
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

        $container.find('tr').each(function(ind, tr) {
          var $tr = $(tr);

          $tr.find('input[name="min-threshold"]').val(thresholdpoints[ind].low);
          $tr.find('input[name="max-threshold"]').val(thresholdpoints[ind].hi);
          $tr.find('input[name="point-threshold"]').val(thresholdpoints[ind].pointsvalue);
        });

        $container.find('.js-threshold-remove').on('click', function() {
          removeItemThreshold(this);
        });

        supportOnlyNumberic($container.find('td input[type="text"]'));
        supportTabThreshold($container);
      }

      /**
       * Accept numeric when keydown
       * @param  {[type]} target [description]
       * @return {[type]}         [description]
       */
      function supportOnlyNumberic(target) {
        var $target = $(target);

        $target.on('keydown', function(event) {
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
       * Support tab in dialog
       * @param  {[type]} element [description]
       * @return {[type]}         [description]
       */
      function supportTabThreshold(element) {
        var $element = $(element);

        $element.on('keydown', 'input[type="text"]', function(e) {
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

      return {
        title: 'Multi-Select Properties',
        minWidth: 615,
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

              //Checked first item as default when create multiple item
              $("#multipleChoice .correctAnswer input[type=checkbox]").first().prop("checked", true);

              $("#bntAddChoice").click(function() {
                isAddAnswer = true;
                isAddAnswerLast = false;
                isEditMultipChoiceGuidance = false;
                var editorID = addNewItemMultipleChoice();
                createCKEditorMulti(editorID);
                multipleChoiceSelection();
              });

              $("#cbThreshold").click(function() {
                if ($(this).is(":checked")) {
                  $(".js-btn-mc-add-threshold, .mc-thresholds").show();
                  $('.divMultipleChoice .point').parent('.ckUpDownNumber').addClass('is-disabled');
                } else {
                  $(".js-btn-mc-add-threshold, .mc-thresholds").hide();
                  $('.divMultipleChoice .point').parent('.ckUpDownNumber').removeClass('is-disabled');
                }
              });

              $dialog.find('input[name="selectionmutiple"]').on('change', function() {
                var $select = $(this);
                var selectType = $select.attr('id');

                if (selectType === 'select_single') {
                  isMultiSelection = false;
                  resetSelectSingle($dialog);
                } else if (selectType === 'select_multi') {
                  isMultiSelection = true;
                  resetSelectMulti($dialog);
                }

                if (iSchemeID == 21) {
                  $dialog.find('#cbThreshold').parent().hide();
                  $dialog.find('.mc-thresholds').addClass('is-hidden');
                  $dialog.find('#cbThreshold').parent().hide();
                }
              });

              $dialog.find('.js-btn-mc-add-threshold').on('click', function() {
                addItemThreshold($dialog.find('.mc-thresholds .table-threshold tbody'));
              });

              handlerOnChangeGradingMethod($dialog);
              handlerOnChangeDisplayMethod($dialog);

              //load data into popup for first time if status is Edit
              if (isEditMultipleChoice) {
                //Checked first item as default when create multiple item
                $("#select_single").prop("checked", true);
                loadDataforMultipleChoice(eleMultipleChoice);
              }
            },
            onShow: function() {
              var questionQtiSchemeId = CKEDITOR.instances[ckID].config.qtiSchemeID;
              var $dialog = $(this.getDialog().getElement().$);
              var thresholdPoints = [];

              if (isEditMultipleChoice == false) {
                createAnswer();
              }
              //hide tooltip
              $('#tips .tool-tip-tips').css({
                'display': 'none'
              });

              getUpDownNumber('.point', 0, 1000);
              getUpDownNumber('#maxChoices', 2, 1000);
              getUpDownNumber('#mc-grid-per-row', 2, 8);
              resetMultipleChoice($dialog);

              //Check multiple choice is single or multiple
              if (questionQtiSchemeId == '1') {
                $(".divMultipleChoice .divMoreOne").show();
                $($(".divMultipleChoice").parents(".cke_dialog_body")).find(".cke_dialog_title").text("Multiple Choice Properties");
              } else if (questionQtiSchemeId == '3') {
                if (!$("#mcMoreOne").is(':checked')) {
                  $(".divMultipleChoice #mcMoreOne").prop("checked", true);
                }
                $(".divMultipleChoice .divMoreOne").hide();
                $($(".divMultipleChoice").parents(".cke_dialog_body")).find(".cke_dialog_title").text("Multi-Select Properties");

              } else if (questionQtiSchemeId == '21') {
                if (isTrueFalse == true) {
                  $(".divMultipleChoice .divMoreOne").hide();
                } else {
                  $(".divMultipleChoice .divMoreOne").show();
                }
                $($(".divMultipleChoice").parents(".cke_dialog_body")).find(".cke_dialog_title").text("Multiple Choice Properties");
                $dialog.find('#mc-grading').hide();
              }

              refreshResponseId();

              // set arrIdResp null
              checkElementRemoveIntoIResult();

              $dialog.find('.divMultipleChoice .point').parent('.ckUpDownNumber').removeClass('is-disabled');
              $dialog.find('.js-btn-mc-add-threshold, .mc-thresholds').hide();
              $dialog.find('.mc-widthLabel[for="cbThreshold"]').hide();
              $dialog.find('.mc-thresholds .thresholds tbody').empty();
              $dialog.find('.mc-thresholds').addClass('is-hidden');

              //the first single click
              var isMcTrueFalse = txtTrueFalse == 'TrueFalse' || isTrueFalse;
              if (isEditMultipleChoiceSingle == true && isMcTrueFalse) {
                $('ul#multipleChoice li:eq(2)').hide();
                $('ul#multipleChoice li:eq(3)').hide();
              }

              if (isMcTrueFalse) {
                var $mcGradingInformationalOnly = $('#mc-grading-informational-only');
                $mcGradingInformationalOnly.next('label').hide();
                $mcGradingInformationalOnly.hide();
                $('#mc-display').hide();
              }

              if (isEditMultipleChoice) {
                var $mcMaxChoices = $('#maxChoices');

                $('iframe[allowtransparency]').contents().find('body').find('div.active-border').removeClass('active-border');
                //Only create CKEditor after html appended
                $("#multipleChoice li").each(function() {
                  createCKEditorMulti("editor" + $(this).attr("id"));
                });

                for (i = 0; i < iResult.length; i++) {
                  if (iResult[i].responseIdentifier == currentMultipleChoiceResId && iResult[i].type == "choiceInteraction") {

                    //Clear all item before add new
                    sChoice = iResult[i].simpleChoice;
                    for (m = 0; m < sChoice.length; m++) {

                      //Load data to each item
                      var currentItem = $("#multipleChoice li").eq(m);
                      CKEDITOR.instances["editor" + currentItem.attr("id")].setData(loadMathML(sChoice[m].value));

                      //Load correct answer
                      if (sChoice[m].answerCorrect != undefined && sChoice[m].answerCorrect.toString() == "true") {
                        currentItem.find(".correctAnswer input[type=checkbox]").prop("checked", true);
                      } else {
                        currentItem.find(".correctAnswer input[type=checkbox]").prop("checked", false);
                      }

                      //Load mp3 to popup
                      if (sChoice[m].audioRef != undefined && sChoice[m].audioRef != "") {
                        currentItem.find(".bntUploadAudio").hide();
                        currentItem.find(".audioRemove").show().find(".audioRef").append(sChoice[m].audioRef);
                      }

                      //apply data guidance has into iResult to iMessageTemp
                      if (isEditMultipChoiceGuidance) {
                        if (sChoice[m].arrMessageGuidance.length) {
                          for (var k = 0, lensChoice = sChoice[m].arrMessageGuidance.length; k < lensChoice; k++) {

                            if (sChoice[m].arrMessageGuidance[k].typeMessage === 'guidance') {
                              currentItem.find('#selected_' + currentItem.attr('id')).attr('title', 'Guidance');
                            }

                            if (sChoice[m].arrMessageGuidance[k].typeMessage === 'rationale') {
                              currentItem.find('#selected_' + currentItem.attr('id')).attr('title', 'Rationale');
                            }

                            if (sChoice[m].arrMessageGuidance.length === 2) {
                              currentItem.find('#selected_' + currentItem.attr('id')).attr('title', 'Guidance and Rationale');
                            }

                            if (sChoice[m].arrMessageGuidance[k].typeMessage === 'guidance_rationale') {
                              currentItem.find('#selected_' + currentItem.attr('id')).attr('title', 'Guidance and Rationale');
                            }
                          }

                          currentItem.find('#selected_' + currentItem.attr('id')).parent('.savedGuidance').show();
                          currentItem.find('#unselected_' + currentItem.attr('id')).hide();
                        }

                        iMessageTemp.push({
                          identifier: sChoice[m].identifier,
                          idTemp: currentItem.attr('id'),
                          arrMessage: sChoice[m].arrMessageGuidance,
                        });

                      }
                    }

                    if (iResult[i].subtype != undefined) {
                      if (iResult[i].subtype == "TrueFalse" || txtTrueFalse == "TrueFalse") {
                        $('#bntAddChoice,.ckRemove').css('display', 'none');
                        $(".divMultipleChoice .divMoreOne").hide();
                        $('ul#multipleChoice li:eq(2)').hide();
                        $('ul#multipleChoice li:eq(3)').hide();
                      } else {
                        if (questionQtiSchemeId == '1' || questionQtiSchemeId == '3') {
                          $(".divMultipleChoice .divMoreOne").show();
                        }
                        $('#bntAddChoice,.ckRemove').css('display', 'inline-block');

                        if (isTrueFalse == true) {
                          $('#bntAddChoice,.ckRemove').css('display', 'none');
                          $(".divMultipleChoice .divMoreOne").hide();
                          $('ul#multipleChoice li:eq(2)').hide();
                          $('ul#multipleChoice li:eq(3)').hide();
                        } else {
                          $('#bntAddChoice,.ckRemove').css('display', 'inline-block');
                          $(".divMultipleChoice .divMoreOne").show();
                          if (questionQtiSchemeId == '1' || questionQtiSchemeId == '3') {
                            $(".divMultipleChoice .divMoreOne").hide();
                          }
                        }
                      }
                    } else {
                      $('#bntAddChoice,.ckRemove').css('display', 'inline-block');
                      $(".divMultipleChoice .divMoreOne").show();
                    }

                    var responseDeclarationCardinality = iResult[i].responseDeclaration.cardinality;

                    if (responseDeclarationCardinality === 'multiple' && !!iResult[i].responseDeclaration.thresholdpoints) {
                      thresholdPoints = iResult[i].responseDeclaration.thresholdpoints;
                      $dialog.find('.mc-widthLabel[for="cbThreshold"]').show();
                    }

                    if (responseDeclarationCardinality === 'multiple') {
                      $dialog.find('#select_multi').prop('checked', true);
                      $dialog.find('.maxChoiceField').show();

                      if (questionQtiSchemeId != 21) {
                        $dialog.find('.mc-widthLabel[for="cbThreshold"]').show();
                      }
                    } else {
                      $dialog.find('#select_single').prop('checked', true);
                      $dialog.find('.maxChoiceField').hide();
                    }
                    onShowGradingMethod($dialog, iResult[i].responseDeclaration.method);
                    onShowDisplayMethod($dialog, iResult[i].display, iResult[i].gridPerRow);
                    isStatusChecked = iResult[i].responseDeclaration.method;
                    $mcMaxChoices.val(iResult[i].maxChoices);
                    if (global.isSurvey == 1) {
                      $dialog.find('#mc-grading').hide();
                      $dialog.find('.divMultipleChoice .point').closest('.fleft').hide();
                    }
                    break;
                  }
                }
              }

              if (!!thresholdPoints && thresholdPoints.length) {
                var $mcThresholds = $dialog.find('.mc-thresholds');
                $mcThresholds.removeClass('is-hidden');
                fillItemThreshold($dialog.find('.mc-thresholds .thresholds tbody'), thresholdPoints);
                $dialog.find('#cbThreshold').prop('checked', true);
                $dialog.find('.mc-thresholds, .js-btn-mc-add-threshold').show();
                $dialog.find('.divMultipleChoice .point').parent('.ckUpDownNumber').addClass('is-disabled');
              }

              CKEDITOR.on('instanceReady', function(ev) {
                //Show the first toolbar when popup has created
                $("#multipleTop > div").hide();
                $("#multipleTop > div:first").show();

                var getData = ev.editor.getData();
                if (getData != "") {
                  ev.editor.setData(loadMathML(getData));
                }
                ev.editor.on('focus', function(evt) {
                  var idAnswer = evt.editor.name.replace('editor', '');
                  var me = $('#' + idAnswer).find('iframe[allowtransparency]').contents().find('body');
                  var $me = $(me);
                  var ind = $('ul#multipleChoice li').index($('#' + idAnswer)) + 1;

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
                  var ind = $('ul#multipleChoice li').index($('#' + idAnswer)) + 1;
                  var me = $('#' + idAnswer).find('iframe[allowtransparency]').contents().find('body');
                   var $me = $(me);
                    var $imageUpload = $me.find('img').hasClass('imageupload');
                    var newAns = $me.text();
                    var mathjax = $me.find('.math-tex');
                    var ans = 'Answer ' + ind;
                    if (isOk === true) {
                      //$(me).text('Answer ' + ind);// $(me).text(data); //set text default when no change input
                      if (newAns !== '' || $imageUpload) {
                        return;
                      } else {
                        var textDefault = $('#' + idAnswer).find('iframe[allowtransparency]').contents().find('body').attr('defaulttext');
                        textDefault = textDefault ? textDefault : ans;
                        $me.text(textDefault);
                      }

                    } else {
                      // Check zero-width spaces when remove all text
                      if (newAns === '\u200b' || newAns === '' && !mathjax.length) {
                        if ($imageUpload) {
                          return;
                        }
                        $me.text('Answer ' + ind); // set text when no press Ok
                        $me.text($('#' + idAnswer).find('iframe[allowtransparency]').contents().find('body').attr('defaulttext'));
                      }
                    }

                });
                // If Users edit multiplechoice, this code fix double scroller - Thinh Le
                if (isEditMultipleChoice) {
                  var IdLastAnswer = $('ul#multipleChoice li').length;
                  var lastHtml = CKEDITOR.instances['editor' + $('ul#multipleChoice li:last-child').attr("id")].getData(); //get last text of li

                  $('ul#multipleChoice li').each(function(index, value) {
                    index = index + 1;
                    var ckID = $(this).attr('id');
                    var data = CKEDITOR.instances['editor' + ckID].getData();
                    if (data.trim() == '' && isAddAnswer == false) {
                      CKEDITOR.instances['editor' + ckID].setData('Answer ' + index);
                      //the first single click

                      var isMcTrueFalse = txtTrueFalse == 'TrueFalse' || isTrueFalse;
                      if (isEditMultipleChoiceSingle && isMcTrueFalse) {
                        $('ul#multipleChoice li:eq(2)').remove();
                        $('ul#multipleChoice li:eq(3)').remove();
                      }
                    } else if (index == IdLastAnswer) {
                      if (isAddAnswer) {
                        CKEDITOR.instances['editor' + ckID].setData('Answer ' + index);
                      }

                      if (isAddAnswerLast == true) {
                        CKEDITOR.instances["editor" + $('ul#multipleChoice li:last-child').attr("id")].setData(loadMathML(lastHtml));
                      }
                      isAddAnswer = false; //Reset status for add new Answer.
                    }
                  });
                } else {
                  var IdLastAnswer = $('ul#multipleChoice li').length;
                  $('ul#multipleChoice li').each(function(index, value) {

                    index = index + 1;
                    var ckID = $(this).attr('id');
                    var data = CKEDITOR.instances['editor' + ckID].getData();

                    if (data.trim() == '') {
                      if (isTrueFalse == false) {
                        CKEDITOR.instances['editor' + ckID].setData('Answer ' + index);

                      } else {
                        $('ul#multipleChoice li:eq(0)').find('iframe[allowtransparency]').contents().find('body').text('True');
                        $('ul#multipleChoice li:eq(1)').find('iframe[allowtransparency]').contents().find('body').text('False');
                      }
                    } else if (index == IdLastAnswer) { // update text or image when new create multip choice

                      if (isAddAnswer == true) {
                        CKEDITOR.instances['editor' + ckID].setData('Answer ' + index);
                        //fix last content for editor multip choice
                        var textContent = $('ul#multipleChoice li:last-child').find("textarea").attr("defaultText");
                        if (textContent != "") {
                          CKEDITOR.instances["editor" + $('ul#multipleChoice li:last-child').attr("id")].setData(loadMathML(textContent));
                        }
                      }

                    }
                  });
                }

                //watermark
                $('.divMultipleChoice').css('display', 'table');
                setTimeout(function() {
                  $('.divMultipleChoice').css('display', 'block');
                }, 500)
                $('ul#multipleChoice li').each(function(index, value) {
                  var that = this;
                  var ind = index + 1;
                  var ckID = $(that).attr('id');
                  var data = CKEDITOR.instances['editor' + ckID].getData();

                  $('#' + ckID).find('iframe[allowtransparency]').contents().find('body').on('keypress', function(e) {
                    $(e.target).attr('isEnter', 'true');
                  });

                });
              });

            }
          }]
        }],
        onOk: function() {
          var $dialog = $(this.getElement().$);
          var isMultiSelectSingle = $dialog.find('input[name="selectionmutiple"]:checked').attr('id') === 'select_multi' && iSchemeID != 21;
          var thresholdPoints = [];
          var maxPointArr = [];

          isAddAnswer = false;
          //Check incase user has checked in correct answer but they don't fill text
          var isEmpty = false,
            qValue = "";
          var textTrueFalse = ""; //save type truefalse into xmlContent
          var pointValue = $(".point").val(),
            responseId = "";

          if ($('#mc-grading-normal').is(':checked')) {
            if (!$dialog.find('#multipleChoice .correctAnswer input[type="checkbox"]:checked').length) {
              customAlert('Please select a one least correct answer.');
              return false;
            }
          }

          if (isMultiSelectSingle && $("#cbThreshold").is(":checked")) {
            var maxChoices = getIntegerNumber($dialog.find('#maxChoices').val());
            var answerCheckedCount = $dialog.find('#multipleChoice .correctAnswer input[type=checkbox]:checked').length;
            var thresholdPointsTemp = [];
            var maxPointEarn = 0;
            var isValidate = false;
            var isOverlap = false;
            var msgNotification = '';

            $dialog.find('.thresholds tbody tr').each(function() {
              var $tr = $(this);
              var minThreshold = getIntegerNumber($tr.find('input[name="min-threshold"]').val());
              var maxThreshold = getIntegerNumber($tr.find('input[name="max-threshold"]').val());
              var pointThreshold = getIntegerNumber($tr.find('input[name="point-threshold"]').val());

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

              if (maxThreshold > maxChoices) {
                $tr.addClass('is-not-validate');
                msgNotification = 'Maximum High value cannot exceed the maximum number of max choices.';
                isValidate = true;
                return false;
              }

              $tr.removeClass('is-not-validate');

              if (thresholdPointsTemp.length > 0) {
                $.each(thresholdPointsTemp, function(index, value) {
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

              maxPointArr.push(pointThreshold);
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

            //Check point is valid or not
            if (thresholdPointsTemp.length > 0) {
              for (var i = 0; i < thresholdPointsTemp.length; i++) {
                maxPointEarn = maxPointEarn < thresholdPointsTemp[i].pointsvalue ? thresholdPointsTemp[i].pointsvalue : maxPointEarn;
              }
            }

            if (parseInt(pointValue) > maxPointEarn) {
              msgNotification = '<p style="text-align: left; margin-bottom: 0;">If you compare the overall Points value (' + pointValue + '), the Max Choices (' + maxChoices + '), and the individual point values allocated for each answer choice, you will see that the students will not be able to earn full credit on this question. You should either 1) lower the overall Points value on this question, 2) increase the maximum number of answer choices a student can select, and/or 3) increase the point values on some of the individual answer choices.</p>';
              customAlert(msgNotification);
              return false;
            }

          }

          notCheckMultipChoice = getNotCheckMultipleChoice($dialog);

          $("#multipleChoice li .correctAnswer input[type=checkbox]").each(function() {

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
            msgNotification = 'Please fill content for answer  ' + qValue;
            customAlert(msgNotification);
            return false;
          }

          var cardinalityChoice = "single",
            maxMulChoices = 1,
            simpleMulChoice = [],
            correctMulResponse = "",
            htmlMulChoice = "",
            currentAnswerIndex = 0, // Store correct alphebet for question
            alphaBe = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H'];

          if (CKEDITOR.instances[ckID].config.alphaBeta != undefined) {
            alphaBe = CKEDITOR.instances[ckID].config.alphaBeta;
          }

          if ($("#select_multi").is(':checked')) {
            cardinalityChoice = "multiple";
            maxMulChoices = parseInt($("#maxChoices").val(), 10);
            var mcItemLength = $("#multipleChoice li").length;

            if (maxMulChoices <= 1 || mcItemLength < maxMulChoices) {
              msgNotification = '<p style="text-align: left; margin-bottom: 0;">The maximum selectable answer choices for multi-select questions cannot be less than 2 and cannot exceed the maximum number of answer choices. Please adjust the value listed in the <b>Max choices</b> option.</p>';
              customAlert(msgNotification);
              return false
            }

            if (maxMulChoices < $("#multipleChoice li .correctAnswer input[type=checkbox]:checked").length) {
              msgNotification = 'Please increase the maximum number of answer choices a student can select.';
              customAlert(msgNotification);
              return false
            }
          }
          var maxLengthItemError = [];
          $("#multipleChoice li").each(function(index) {

            //add data guidance and rationale into simplechocie
            var idTagLi = $(this).attr('id');
            var arrMessageGuidance = [];
            var hastypeMessageGuidance = 'noGuidance';
            var typeGuidance = '';
            var typeRationale = '';
            var typeGuidanceRationale = '';

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
              item = {
                identifier: iddentify,
                value: CKEDITOR.instances[editorId].getData(),
                arrMessageGuidance: arrMessageGuidance
              },
              hasAudio = 'class="nonAudioIcon"',
              audioLink = '';
            hasCorrectAnwser = 'class="item"';

            //save text Default
            var textDefault = $(this).find('iframe[allowtransparency]').contents().find('body').attr('defaultText');

            if (textDefault === undefined) {

              textDefault = '';
            }
            var flatTextValue = $('<span>' + CKEDITOR.instances[editorId].getData() + '</span>').text();
            if (global.isSurvey == 1 && flatTextValue.length > 100) {
              maxLengthItemError.push(alphaBe[currentAnswerIndex])
            }
            if ($(this).find(".audioRef").length == 1 && $(this).find(".audioRef").html() != "") {
              item.audioRef = $(this).find(".audioRef").text();
              hasAudio = 'class="audioIcon"';
              audioLink = $(this).find(".audioRef").html();
            }

            if ($(this).find(".correctAnswer input[type=checkbox]").is(':checked')) {
              if (correctMulResponse != "") {
                correctMulResponse += ",";
              }
              correctMulResponse += iddentify;
              hasCorrectAnwser = 'class="item answerCorrect"';
              item.answerCorrect = true;
            }

            //Add item for multiple choice
            htmlMulChoice += '<div defaultText="' + textDefault + '" ' + hasCorrectAnwser + '>';
            htmlMulChoice += "<div " + hasAudio + ">";
            htmlMulChoice += '<img alt="Play audio" class="imageupload bntPlay" src="' + CKEDITOR.plugins.getImgByVersion('multiplechoice', 'images/small_audio_play.png') + '" title="Play audio">';
            htmlMulChoice += '<img alt="Stop audio" class="bntStop" src="' + CKEDITOR.plugins.getImgByVersion('multiplechoice', 'images/small_audio_stop.png') + '" title="Stop audio">';
            htmlMulChoice += "<span class='audioRef'>" + audioLink + "</span></div>";
            htmlMulChoice += "<div style='display: none;' class='" + hastypeMessageGuidance + "'>";
            htmlMulChoice += '    <img alt="Guidance" class="imageupload bntGuidance" src="' + CKEDITOR.plugins.getImgByVersion('multiplechoice', 'images/guidance_unchecked.png') + '" title="Guidance">';
            htmlMulChoice += "</div>";
            htmlMulChoice += "<div class='answer'>" + item.identifier + ".</div>";
            htmlMulChoice += "<div class='answerContent'>" + item.value + "</div>";
            htmlMulChoice += typeGuidance;
            htmlMulChoice += typeRationale;
            htmlMulChoice += typeGuidanceRationale;
            htmlMulChoice += "</div>";

            simpleMulChoice.push(item);

            currentAnswerIndex += 1;

          });
          if (global.isSurvey == 1 && maxLengthItemError.length) {
            msgNotification = '<p style="text-align: left; margin-bottom: 0;">The answer value for item(s) <strong>' + maxLengthItemError.join(', ') + '</strong> must be less than or equal to 100 characters.</p>';
            customAlert(msgNotification);
            return false;
          }
          var gradingMethod = getGradingMethod($dialog);
          var displayMethod = getDisplayMethod($dialog);
          var gridPerRow = $dialog.find('#mc-grid-per-row').val();

          if (txtTrueFalse == 'TrueFalse' || isTrueFalse) {
            textTrueFalse = 'TrueFalse';
          }

          if (isEditMultipleChoice) {
            //Update for current textEntry
            if (maxPointArr.length) {
              pointValue = Math.max.apply(Math, maxPointArr);
            }

            for (n = 0; n < iResult.length; n++) {
              if (iResult[n].responseIdentifier == currentMultipleChoiceResId) {
                responseId = currentMultipleChoiceResId;
                iResult[n].responseDeclaration.pointsValue = pointValue;
                iResult[n].maxChoices = maxMulChoices;
                iResult[n].correctResponse = correctMulResponse;
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

            var multiplechoiceResult = {
              type: 'choiceInteraction',
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
              correctResponse: correctMulResponse,
              simpleChoice: simpleMulChoice,
              display: displayMethod
            };

            if (txtTrueFalse == 'TrueFalse' || isTrueFalse) {
              multiplechoiceResult.subtype = textTrueFalse;
            } else {
              if (displayMethod === 'grid') {
                multiplechoiceResult.gridPerRow = gridPerRow;
              }
            }

            iResult.push(multiplechoiceResult);

            //Hide button on toolbar after add multiple choice incase qtItem is 1 or 3
            if (iSchemeID == "1" || iSchemeID == "3") {
              $(".cke_button__multiplechoice").parents("span.cke_toolbar").hide();
            }
          }

          if (isMultiSelectSingle) {
            for (n = 0; n < iResult.length; n++) {
              if (iResult[n].responseIdentifier == currentMultipleChoiceResId) {
                iResult[n].responseDeclaration.thresholdpoints = thresholdPoints;
                break;
              }
            }
          }

          htmlMulChoice = loadMathML(htmlMulChoice);

          $('iframe[allowtransparency]').contents().find('body').find('div.active-border').removeClass('active-border');

          var newMultipleChoice = '<div textTrueFalse="' + textTrueFalse + '" class="multipleChoice" id="' + responseId + '" title="' + responseId + '" contenteditable="false"><button class="single-click" id="single-click">Click here to edit answer choices</button><img class="cke_reset cke_widget_mask multipleChoiceMark" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" />' + htmlMulChoice + ' </div>&nbsp;'; 
          //Remove old multiple choice and add new one. This case just for safari only.
          if (CKEDITOR.env.safari && isEditMultipleChoice) {
            var oldHtml = $('iframe[allowtransparency]').contents().find('body').find('#' + responseId);
            oldHtml.replaceWith(newMultipleChoice); // cause duplicate responseId,so remove old
          } else {
            var spaces = '&#160;&#160;&#160;&#160;&#160;';
            editor.insertHtml(spaces);
            editor.insertHtml(newMultipleChoice)

            var $body = $(editor.window.getFrame().$).contents().find('body');
            var content = $body.html().replaceAll('&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;', '');
            $body.html(content);
          }

          //set event click play audio
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
          var singleclick = $(editor.window.getFrame().$).contents().find('.single-click');
          singleclick.unbind("click").on('click', function(e) {
            var element = $(e.target).parents('.multipleChoice')[0];

            if(!element) return;

            var txtTypeTrueFalse = element.getAttribute('texttruefalse');

            //Move selection to parent of multipleChoiceMark
            if (CKEDITOR.env.safari) {
              editor.getSelection().getSelectedElement();
            } else {
              editor.getSelection().selectElement(editor.document.getActive().getParent());
            }

            $('#multipleTop').html('');
            //The status to editor know this is update
            isEditMultipleChoice = true;
            isEditMultipChoiceGuidance = true;

            // Show type for mutiple part
            if (txtTypeTrueFalse == 'TrueFalse') {
              isTrueFalse = true;
            } else {
              isTrueFalse = false;
            }

            editor.openDialog('insertMultipleChoice', function() {
              currentMultipleChoiceResId = loadDataforMultipleChoiceSingleClick(element);
              idSimpleChoicesPopup = currentMultipleChoiceResId;
            });
          });

          //Reset to default after update or create new textEntry
          stopVNSAudio();
          isEditMultipleChoice = false;
          setDefault = true;
          currentMultipleChoiceResId = "";
          isOk = true;
          isTrueFalse = false;
          ResIdElemModul = responseId;
          newResult = iResult;

          isEditMultipChoiceGuidance = false;
          iMessageTemp = [];
          idSimpleChoicesPopup = '';
          iMessageTempEdit = [];
          isRemoveGuidancePopup = false;


          if (gradingMethod === 'algorithmic') {
            TestMakerComponent.isShowAlgorithmicConfiguration = true;
          } else {
            TestMakerComponent.isShowAlgorithmicConfiguration = false;
          }
        },
        onCancel: function() {
          //Reset to default after update or create new textEntry
          stopVNSAudio();
          isEditMultipleChoice = false;
          currentMultipleChoiceResId = "";

          isEditMultipChoiceGuidance = false;
          iMessageTemp = [];
          idSimpleChoicesPopup = '';
          iMessageTempEdit = [];
          isRemoveGuidancePopup = false;
        }
      };
    });
  }
});

/***
 * Load data to Draw Tool popup form
 * Return: responseIdentifier
 ***/
function loadDataforMultipleChoice(element) {
  var currentMultipleChoiceResId = "";
  var sChoice = [];
  var idCurr = "";
  if (element.getId == undefined) {
    idCurr = element.id;
  } else {
    idCurr = element.getId();
  }

  for (i = 0; i < iResult.length; i++) {
    if (iResult[i].responseIdentifier == idCurr && iResult[i].type == "choiceInteraction") {

      if (iResult[i].subtype != undefined) {
        isTrueFalse = true;
      }
      currentMultipleChoiceResId = iResult[i].responseIdentifier;
      //Load data

      $(".divMultipleChoice .point").val(iResult[i].responseDeclaration.pointsValue);
      if (iResult[i].responseDeclaration.cardinality == "multiple") {
        $(".divMultipleChoice #select_multi").prop("checked", true);
        $(".divMultipleChoice #select_single").prop("checked", false);
        $(".divMultipleChoice .maxChoiceField").css({
          "display": "block"
        });
      } else {
        $(".divMultipleChoice #select_single").prop("checked", true);
        $(".divMultipleChoice #select_multi").prop("checked", false);
        $(".divMultipleChoice .maxChoiceField").css({
          "display": "none"
        });
      }

      //Clear all item before add new
      $("#multipleChoice").empty();
      sChoice = iResult[i].simpleChoice;
      for (m = 0; m < sChoice.length; m++) {
        addNewItemMultipleChoice();
        multipleChoiceSelection();
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
function loadDataforMultipleChoiceSingleClick(ele) {
  var currentMultipleChoiceResId = "";
  var sChoice = [];
  for (var i = 0; i < iResult.length; i++) {
    if (iResult[i].responseIdentifier == ele.id && iResult[i].type == "choiceInteraction") {
      currentMultipleChoiceResId = iResult[i].responseIdentifier;
      //Load data

      $(".divMultipleChoice .point").val(iResult[i].responseDeclaration.pointsValue);
      if (iResult[i].responseDeclaration.cardinality == "multiple") {
        //$(".divMultipleChoice #mcMoreOne").prop("checked", true);
        $(".divMultipleChoice #select_multi").prop("checked", true);
        $(".divMultipleChoice #select_single").prop("checked", false);
        $(".divMultipleChoice .maxChoiceField").css({
          "display": "block"
        });
      } else {
        $(".divMultipleChoice #select_single").prop("checked", true);
        $(".divMultipleChoice #select_multi").prop("checked", false);
        $(".divMultipleChoice .maxChoiceField").css({
          "display": "none"
        });
      }

      //Clear all item before add new
      $("#multipleChoice").empty();
      sChoice = iResult[i].simpleChoice;
      for (var m = 0; m < sChoice.length; m++) {
        addNewItemMultipleChoice();
        multipleChoiceSelection();
      }
    }
  }
  return currentMultipleChoiceResId;
}
/***
 * Function to add new an anwser for multiple Choice
 * Return EditorID this will help to create CKEditor
 ***/
function addNewItemMultipleChoice() {

  var multiChoiceLength = $("#multipleChoice li").length;
  var alphabet = [];
  if (CKEDITOR.instances[ckID].config.alphaBeta != undefined) {
    alphabet = CKEDITOR.instances[ckID].config.alphaBeta;
  }

  //maximum answer for multipleChoise is 25 answers
  if (multiChoiceLength > 40) {
    return;
  }

  var now = $.now(),
    editorId = 'editor' + alphabet[multiChoiceLength] + now;

  //push id item answer into iMessageTemp
  if (!isEditMultipChoiceGuidance) {
    iMessageTemp.push({
      idTemp: alphabet[multiChoiceLength] + now,
      identifier: alphabet[multiChoiceLength],
      arrMessage: []
    });
  }

  var getImgByVersion = CKEDITOR.plugins.getImgByVersion;

  var strHTML = '<li id="' + alphabet[multiChoiceLength] + now + '">';

  strHTML += '<div class="alphabet">' + alphabet[multiChoiceLength] + '</div>';
  strHTML += '<div class="correctAnswer"><input type="checkbox" name="multipleChoice" value="' + alphabet[multiChoiceLength] + '"/></div>';
  strHTML += '<div class="content">';
  strHTML += '<textarea cols="100" id="' + editorId + '" name="editor1" rows="1" tabindex="' + (multiChoiceLength + 1) + '"></textarea>';
  strHTML += '</div>';
  strHTML += '<div class="sort actions">';
  strHTML += '<input issavedmessage="false" id="unselected_' + (alphabet[multiChoiceLength] + now) + '" type="image" src="' + getImgByVersion('multiplechoice', 'images/guidance_unchecked.png') + '" class="addGuidance" title="Add Guidance/Rationales" />';
  strHTML += '<div class="savedGuidance" style="display: none;"><span class="btnRemoveGuidance" title="Remove">x</span><input issavedmessage="true" id="selected_' + (alphabet[multiChoiceLength] + now) + '" type="image" src="' + getImgByVersion('multiplechoice', 'images/guidance_checked.png') + '" class="addGuidance" title="" /></div>';
  strHTML += '<div class="audio">';
  strHTML += '<div class="bntUploadAudio">';
  strHTML += '<form name="form-upload-' + alphabet[multiChoiceLength] + now + '" id="form-upload-' + alphabet[multiChoiceLength] + now + '" lang="en" dir="ltr" action="uploader.php?type=mp3" method="POST" enctype="multipart/form-data">';
  strHTML += '    <input type="file" name="file" class="hiddenUpload" accept="audio/mp3" size="60" />';
  strHTML += '    <input type="hidden" name="id" />';
  strHTML += '</form>';
  strHTML += '<input type="image" src="' + getImgByVersion('multiplechoice', 'images/audio-add.png') + '" class="addAudio" title="Add audio" />';
  strHTML += '</div>';
  strHTML += '<div class="audioRemove">';
  strHTML += '    <img alt="Play audio" class="bntPlay" src="' + getImgByVersion('multiplechoice', 'images/small_audio_play.png') + '" title="Play audio" />';
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

  $("#multipleChoice").append(strHTML);
  IS_V2 && $("#multipleChoice").find('[title]').tip();

  return editorId;
}

/***
 * Function to register all events for multiple choice
 ***/
function multipleChoiceSelection() {
  $('#multipleChoice .correctAnswer').find('input[type="checkbox"]').unbind('click').click(function() {
    var $self = $(this);

    if (!$('#select_multi').is(':checked')) {
      if ($self.is(':checked')) {
        var $group = $('#multipleChoice .correctAnswer').find('input[type="checkbox"][name="' + $self.attr('name') + '"]');
        $group.prop('checked', false);
        $self.prop('checked', true);
      } else {
        $self.prop('checked', false);
      }
    }

    if (!$('#multipleChoice .correctAnswer').find('input[type="checkbox"]:checked').length) {
      $self.prop('checked', true);
    }
  });

  $("#multipleChoice .audio .addAudio").unbind("click").click(function() {

    if (document.createEvent) {
      var e = document.createEvent('MouseEvents');
      e.initEvent('click', true, true);
      $(this).parent().find(".hiddenUpload").val("").get(0).dispatchEvent(e);
    } else {
      $(this).parent().find(".hiddenUpload").val("").trigger("click");
    }
  });

  $("#multipleChoice .audio .hiddenUpload").unbind("change").change(function() {

    var file = this.value;
    var extension = file.substr((file.lastIndexOf('.') + 1));

    if (extension.toLowerCase() != "mp3") {
      customAlert("Unsupported file type. Please select mp3 file.");
      return;
    }

    $(this).parent().children("input[name='id']").val(objectId);
    audioUploadMultipleChoice($(this).parent().get(0), audioConfig, this);

  });

  $(".bntPlay").unbind("click").click(function() {
    var $editorContent = $(CKEDITOR.instances[ckID].window.getFrame().$).contents();
    var $audio = $(this);
    var audioUrl = $audio.parent().find('.audioRef').text();

    $editorContent.find('.audioIcon .bntStop').hide();
    $editorContent.find('.audioIcon .bntPlay').show();

    resetUIAudio();

    $audio.next().show();
    $audio.hide();

    if (window.playsound != undefined) {
      window.playsound.pause();
    }

    window.playsound = new vnsAudio({
      src: audioUrl,
      onEnded: function() {
        $audio.next().hide();
        $audio.show();
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

  $(".ckRemove").unbind("click").click(function(evt) {

    //remove item answer into iMessageTemp
    var idAnswer = $(evt.target).parents('li').attr('id');
    if (iMessageTemp.length) {
      for (var i = 0, lenMessageTemp = iMessageTemp.length; i < lenMessageTemp; i++) {
        var itemMessage = iMessageTemp[i];
        if (itemMessage.idTemp === idAnswer) {
          iMessageTemp.splice(i, 1);
          break;
        }
      }
    }

    if ($("#multipleChoice li").length == 1) {
      return;
    }
    var answer = $(this).parent().parent(),
      instanceName = answer.find("textarea").attr("id");
    index = answer.index();
    if (CKEDITOR.instances[instanceName]) CKEDITOR.instances[instanceName].destroy();
    answer.remove();
    sortItemMultipleChoice(index);
    //Show the first CK Toolbar when remove an item
    $("#multipleTop > div").hide();
    $("#multipleTop > div:first").show();

    if ($('#mc-grading-normal').is(':checked')) {
      if (!$('#multipleChoice .correctAnswer input[type="checkbox"]:checked').length) {
        $('#multipleChoice').find('li').first().find('.correctAnswer input[type="checkbox"]').prop('checked', true);
      }
    }
  });

  $(".ckUp").unbind("click").click(function(event) {
    event.preventDefault();

    var parent = $(this).parents("li");
    if (parent.index() == 0) {
      return;
    }
    isAddAnswerLast = false;
    if ($(this).parents("li").index() == ($("#multipleChoice li").length - 1)) {
      isAddAnswerLast = true;
    }

    editorId = 'editor' + parent.attr("id");
    CKEDITOR.instances[editorId].destroy();


    //Change alphabet of anwser
    parent.prev().find(".alphabet").empty().html(alphabet[parent.index()]);
    parent.prev().find(".correctAnswer input:checkbox").val(alphabet[parent.index()]);
    parent.prev().find("textarea").attr({
      "tabindex": parent.index() + 1
    });
    parent.find(".alphabet").empty().html(alphabet[parent.index() - 1]);
    parent.find(".correctAnswer input:checkbox").val(alphabet[parent.index() - 1]);
    parent.find("textarea").attr({
      "tabindex": parent.index()
    });


    parent.insertBefore(parent.prev());
    createCKEditorMulti(editorId);

    //Show the first CK Toolbar when remove an item
    $("#multipleTop > div:first").show();
  });

  $(".ckDown").unbind("click").click(function(event) {
    event.preventDefault();
    var parent = $(this).parents("li");
    var textContent = parent.find('.content').find('iframe[allowtransparency]').contents().find('body').text();
    if (parent.index() == $("#multipleChoice li").length - 1) {
      return;
    }

    editorId = 'editor' + parent.attr("id");
    CKEDITOR.instances[editorId].destroy();

    ////Change alphabet of anwser
    parent.next().find(".alphabet").empty().html(alphabet[parent.index()]);
    parent.next().find(".correctAnswer input:checkbox").val(alphabet[parent.index()]);
    parent.next().find("textarea").attr({
      "tabindex": parent.index() + 1
    });
    parent.find(".alphabet").empty().html(alphabet[parent.index() + 1]);
    parent.find(".correctAnswer input:checkbox").val(alphabet[parent.index() + 1]);
    parent.find("textarea").attr({
      "tabindex": parent.index() + 2
    });
    parent.find("textarea").attr("defaultText", textContent);

    parent.insertAfter(parent.next());
    createCKEditorMulti(editorId);

    //Show the first CK Toolbar when remove an item
    $("#multipleTop > div:first").show();
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
    isRemoveGuidancePopup = true;

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
};

/***
 * Function to sort when remove an anwser of multiple Choice
 ***/
function sortItemMultipleChoice(index) {
  for (i = index; i < $("#multipleChoice li").length; i++) {
    currentAnwser = $("#multipleChoice li").eq(i);
    currentAnwser.find(".alphabet").empty().html(alphabet[i]);
    currentAnwser.find(".correctAnswer input:checkbox").val(alphabet[i]);
    currentAnwser.find("textarea").attr({
      "tabindex": i + 1
    });
  }
}

//Create new 4 anwsers
function createAnswer() {
  $("#multipleChoice li").each(function() {
    var myId = "editor" + this.id;
    if (CKEDITOR.instances[myId]) CKEDITOR.instances[myId].destroy();
  });
  $("#multipleChoice").empty();

  //Create default (4)item for inlineChoice
  if (isTrueFalse == false) {
    for (var k = 0; k < 4; k++) {
      var editorID = addNewItemMultipleChoice();

      multipleChoiceSelection();
      createCKEditorMulti(editorID);
      $('#bntAddChoice,.ckRemove').css('display', 'inline-block');
    }
  } else {
    for (var k = 0; k < 2; k++) {
      var editorID = addNewItemMultipleChoice();
      multipleChoiceSelection();
      createCKEditorMulti(editorID);
      $('#bntAddChoice,.ckRemove').css('display', 'none');
    }
  }

  $("#multipleChoice .correctAnswer input[type=checkbox]").first().prop("checked", true);
  //$("#mcMoreOne").prop("checked", false);
  $(".divMultipleChoice .point").val("1");
  $("#multipleTop > div:first").show();
  $("#select_single").prop("checked", true);
  $("#select_multi").prop("checked", false);
}

function createCKEditorMulti(ckId) {
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
      top: 'multipleTop'
    },
    extraAllowedContent: 'span[stylename]; span(smallText,normalText,largeText,veryLargeText);',
    width: 'auto',
    height: 60
  });
}

//Play audio
function playAudio(player, audioUrl) {
  var $audio = $(player);
  var audioUrl = audioUrl;

  $audio.next().show();
  $audio.hide();

  if (window.playsound != undefined) {
    window.playsound.pause();
  }

  window.playsound = new vnsAudio({
    src: audioUrl,
    onEnded: function() {
      $audio.next().hide();
      $audio.show();
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

function audioUploadMultipleChoice(form, action_url, currentElement) {
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

    var $currentElement = $(currentElement);

    $currentElement.parents(".audio").find(".audioRemove").show();
    $currentElement.parents(".audio").find(".audioRef").text(data.url);
    $currentElement.parents(".bntUploadAudio").hide();
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

function resetSelectSingle(dialog) {
  var $mcCorrectAnswer = dialog.find('#multipleChoice .correctAnswer input[type=checkbox]');
  var isAlgorithmic = dialog.find('#mc-grading-algorithmic').is(':checked');
  var isInformationOnly = dialog.find('#mc-grading-informational-only').is(':checked');

  dialog.find('.divMultipleChoice .point').parent('.ckUpDownNumber').removeClass('is-disabled');
  dialog.find('.js-btn-mc-add-threshold, .mc-thresholds').hide();
  dialog.find('.mc-widthLabel[for="cbThreshold"]').hide();
  dialog.find('.maxChoiceField').hide();
  dialog.find('.mc-thresholds').addClass('is-hidden');
  $mcCorrectAnswer.prop('checked', false);
  $mcCorrectAnswer.first().prop('checked', true);

  if (isAlgorithmic || isInformationOnly) {
    $mcCorrectAnswer.prop('checked', false).prop('disabled', true);
    dialog.find('.divMultipleChoice .point').parent().addClass('is-disabled');
  }
}

function resetSelectMulti(dialog) {
  var $mcCorrectAnswer = dialog.find('#multipleChoice .correctAnswer input[type=checkbox]');
  var isAlgorithmic = dialog.find('#mc-grading-algorithmic').is(':checked');
  var isInformationOnly = dialog.find('#mc-grading-informational-only').is(':checked');

  dialog.find('.js-btn-mc-add-threshold, .mc-thresholds').hide();
  dialog.find('.mc-widthLabel[for="cbThreshold"]').show();
  dialog.find('#cbThreshold').prop('checked', false);
  dialog.find('.maxChoiceField').show();
  dialog.find('#maxChoices').val($mcCorrectAnswer.length);
  dialog.find('.mc-thresholds').addClass('is-hidden');
  $mcCorrectAnswer.prop('checked', false);
  $mcCorrectAnswer.first().prop('checked', true);

  if (isAlgorithmic || isInformationOnly) {
    $mcCorrectAnswer.prop('checked', false).prop('disabled', true);
    dialog.find('.divMultipleChoice .point').parent().addClass('is-disabled');
  }
}
