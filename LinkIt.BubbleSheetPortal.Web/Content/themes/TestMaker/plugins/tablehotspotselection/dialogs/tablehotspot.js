/**
 * @license Copyright (c) 2003-2013, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

(function () {
  var defaultToPixel = CKEDITOR.tools.cssLength;
  var sourceGardingHotSpot = {};
  var isDelete = false;
  var audioTableHtml = `\
      <div id="audioTableUploadContent">\
          <table cellspacing="0" border="0" align="left" style="width:100%;">\
              <tbody>\
                  <tr>\
                      <td>\
                          <div>\
                              <div class="cke_dialog_ui_labeled_content cke_dialog_ui_input_file">\
                                  <form name="form-upload" id="formTableAudioUpload" lang="en" action="uploader.php?type=mp3" dir="ltr" method="POST" enctype="multipart/form-data">\
                                      <input type="file" size="38" name="file" aria-labelledby="cke_262_label" id="formTableAudioUploadFile" style="max-width: 100%; overflow: hidden; text-overflow: ellipsis; width: 94%;height: auto;border: solid 1px #cccccc;">\
                                      <input type="hidden" name="id" id="objectId" />\
                                  </form>\
                              </div>\
                          </div>\
                      </td>\
                      <td>\
                          <a id="uploadAudioTableButton" role="button" class="cke_dialog_ui_fileButton cke_dialog_ui_button uploadAudioTableButton" hidefocus="true" title="Upload" href="javascript:void(0)" style="-moz-user-select: none;float: right;">\
                              <span class="cke_dialog_ui_button">Upload</span>\
                          </a>\
                      </td>\
                  </tr>\
                  <tr>\
                      <td colspan="2">\
                          <div id="demoAudio" style="width:72%; -moz-user-select: none; display:none; background:#026211; border-radius:25px;padding: 3px 0; margin: 10px auto;">\
                              <table>\
                                  <tr>\
                                      <td style="vertical-align:middle; text-align:left;">\
                                          <div class="audio" id="audioTableDemo" style="margin-left:-35px">\
                                              <div id="audioTableRemoveQuestion" style="width: 40px;padding-top:2px;">\
                                                  <img alt="Play audio" class="bntPlay" src="../../Content/themes/TestMaker/images/small_audio_play.png" title="Play audio" />\
                                                  <img alt="Stop audio" class="bntStop" src="../../Content/themes/TestMaker/images/small_audio_stop.png" title="Stop audio" />\
                                                  <span class="audioRef"></span>\
                                              </div>\
                                          </div>\
                                      </td>\
                                      <td style="vertical-align:middle;">\
                                          <p style="font-size:12px; color:#fff; text-indent:3px;">Click on icon to play audio.</p>\
                                      </td>\
                                  </tr>\
                              </table>\
                          </div>\
                      </td>\
                  </tr>\
              </tbody>\
          </table>\
      </div>`;

  var commitValue = function (data) {
    var id = this.id;
    if (!data.info)
      data.info = {};
    data.info[id] = this.getValue();
  };

  function tableColumns(table) {
    var cols = 0,
      maxCols = 0;
    for (var i = 0, row, rows = table.$.rows.length; i < rows; i++) {
      row = table.$.rows[i], cols = 0;
      for (var j = 0, cell, cells = row.cells.length; j < cells; j++) {
        cell = row.cells[j];
        cols += cell.colSpan;
      }

      cols > maxCols && (maxCols = cols);
    }

    return maxCols;
  }

  // Whole-positive-integer validator.
  function validatorNum(msg) {
    return function () {
      var value = this.getValue(),
        pass = !!(CKEDITOR.dialog.validate.integer()(value) && value > 0);

      if (!pass) {
        alert(msg);
        this.select();
      }

      return pass;
    };
  }

  function audioUpload(form, action_url) {
    $("body").ckOverlay();

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
      $("#audioTableDemo .audioRef").html(data.absoluteUrl);
      $("#demoAudio").show();
      $('#audioTableDemo .bntStop').hide().end().find('#audioTableDemo .bntPlay').show();
      stopVNSAudio();

      //hide overlay
      $("body").ckOverlay.destroy();
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

  function onLoadTableAudio() {
    var formTableAudioUploadButton = $('#uploadAudioTableButton');
    var formTableAudioUploadFile = $('#formTableAudioUploadFile');

    formTableAudioUploadButton.click(function () {
      refeshConfig();
      var file = formTableAudioUploadFile.get(0).value;

      if (file == "") {
        customAlert("Please select audio file.");
        return;
      }
      var extension = file.substr((file.lastIndexOf('.') + 1));

      if (extension.toLowerCase() != "mp3") {
        customAlert("Unsupported file type. Please select mp3 file.");
        return;
      }
      $("#audioTableUploadContent").find('#objectId').val(objectId);

      audioUpload($('#formTableAudioUpload').get(0), audioConfig);

      return false;
    });

    formTableAudioUploadFile.change(function () {
      $("#audioTableRemoveQuestion .bntPlay").show().next().hide();
      stopVNSAudio();
      resetUIAudio();
    });

    //Handlers when controls is clicked
    $("#audioTableDemo .bntPlay").click(playVNSAudio);
    $("#audioTableDemo .bntStop").click(stopVNSAudio);
  }

  function onShowTableAudio() {
    $('#formTableAudioUploadFile').val('');
    $("#audioTableDemo audioRef").html('');
    $('#demoAudio').hide();
  }

  function tableDialog(editor, command) {
    var makeElement = function (name) {
      return new CKEDITOR.dom.element(name, editor.document);
    };

    var editable = editor.editable();

    var dialogadvtab = editor.plugins.dialogadvtab;

    return {
      title: editor.lang.tablehotspotselection.title,
      minWidth: IS_V2 ? 500 : 310,
      minHeight: CKEDITOR.env.ie ? 100 : 100,

      onLoad: function () {

        var dialog = this;

        var styles = dialog.getContentElement('advanced', 'advStyles');

        if (styles) {
          styles.on('change', function (evt) {
            // Synchronize width value.
            var width = this.getStyle('width', ''),
              txtWidth = dialog.getContentElement('info', 'txtWidth');

            txtWidth && txtWidth.setValue(width, true);

            // Synchronize height value.
            var height = this.getStyle('height', ''),
              txtHeight = dialog.getContentElement('info', 'txtHeight');

            txtHeight && txtHeight.setValue(height, true);
          });
        }
        onLoadTableAudio();
      },

      onShow: function () {

        $('#tips').find('.tool-tip-tips').hide();
        // Detect if there's a selected table.
        var selection = editor.getSelection(),
          ranges = selection.getRanges(),
          table;

        var rowsInput = this.getContentElement('info', 'txtRows'),
          colsInput = this.getContentElement('info', 'txtCols'),
          widthInput = this.getContentElement('info', 'txtWidth'),
          heightInput = this.getContentElement('info', 'txtHeight');

        if (command == 'tablePropertiesHotSpot') {
          var selected = selection.getSelectedElement();
          if (selected && selected.is('table'))
            table = selected;
          else if (ranges.length > 0) {
            // Webkit could report the following range on cell selection (#4948):
            // <table><tr><td>[&nbsp;</td></tr></table>]
            if (CKEDITOR.env.webkit)
              ranges[0].shrink(CKEDITOR.NODE_ELEMENT);

            table = editor.elementPath(ranges[0].getCommonAncestor(true)).contains('table', 1);
          }

          // Save a reference to the selected table, and push a new set of default values.
          this._.selectedElement = table;
        }

        // Enable or disable the row, cols, width fields.
        if (table) {
          this.setupContent(table);
          rowsInput && rowsInput.disable();
          colsInput && colsInput.disable();
        } else {
          rowsInput && rowsInput.enable();
          colsInput && colsInput.enable();
        }

        // Call the onChange method for the widht and height fields so
        // they get reflected into the Advanced tab.
        widthInput && widthInput.onChange();
        heightInput && heightInput.onChange();

        //Hide table border width field
        $("#" + this.getContentElement('info', 'txtBorder').domId).parent("td").hide();

        $('table.cke_dialog').find('.cke_dialog_title').text('Table Properties');

        onShowTableAudio();
      },
      onOk: function () {
        var selection = editor.getSelection(),
          bms = this._.selectedElement && selection.createBookmarks();

        var table = this._.selectedElement || makeElement('table'),
          me = this,
          data = {};

        this.commitContent(data, table);
        var audioId = new Date().getTime();
        table.setAttribute('audioId', audioId);

        if (data.info) {
          var info = data.info;

          // Generate the rows and cols.
          if (!this._.selectedElement) {
            var tbody = table.append(makeElement('tbody')),
              rows = parseInt(info.txtRows, 10) || 0,
              cols = parseInt(info.txtCols, 10) || 0;

            for (var i = 0; i < rows; i++) {
              var row = tbody.append(makeElement('tr'));
              for (var j = 0; j < cols; j++) {
                var cell = row.append(makeElement('td'));
                cell.appendBogus();
              }
            }
          }

          // Modify the table headers. Depends on having rows and cols generated
          // correctly so it can't be done in commit functions.

          // Should we make a <thead>?
          var headers = info.selHeaders;
          if (!table.$.tHead && (headers == 'row' || headers == 'both')) {
            var thead = new CKEDITOR.dom.element(table.$.createTHead());
            tbody = table.getElementsByTag('tbody').getItem(0);
            var theRow = tbody.getElementsByTag('tr').getItem(0);

            // Change TD to TH:
            for (i = 0; i < theRow.getChildCount(); i++) {
              var th = theRow.getChild(i);
              // Skip bookmark nodes. (#6155)
              if (th.type == CKEDITOR.NODE_ELEMENT && !th.data('cke-bookmark')) {
                th.renameNode('th');
                th.setAttribute('scope', 'col');
              }
            }
            thead.append(theRow.remove());
          }

          if (table.$.tHead !== null && !(headers == 'row' || headers == 'both')) {
            // Move the row out of the THead and put it in the TBody:
            thead = new CKEDITOR.dom.element(table.$.tHead);
            tbody = table.getElementsByTag('tbody').getItem(0);

            var previousFirstRow = tbody.getFirst();
            while (thead.getChildCount() > 0) {
              theRow = thead.getFirst();
              for (i = 0; i < theRow.getChildCount(); i++) {
                var newCell = theRow.getChild(i);
                if (newCell.type == CKEDITOR.NODE_ELEMENT) {
                  newCell.renameNode('td');
                  newCell.removeAttribute('scope');
                }
              }
              theRow.insertBefore(previousFirstRow);
            }
            thead.remove();
          }

          // Should we make all first cells in a row TH?
          if (!this.hasColumnHeaders && (headers == 'col' || headers == 'both')) {
            for (row = 0; row < table.$.rows.length; row++) {
              newCell = new CKEDITOR.dom.element(table.$.rows[row].cells[0]);
              newCell.renameNode('th');
              newCell.setAttribute('scope', 'row');
            }
          }

          // Should we make all first TH-cells in a row make TD? If 'yes' we do it the other way round :-)
          if ((this.hasColumnHeaders) && !(headers == 'col' || headers == 'both')) {
            for (i = 0; i < table.$.rows.length; i++) {
              row = new CKEDITOR.dom.element(table.$.rows[i]);
              if (row.getParent().getName() == 'tbody') {
                newCell = new CKEDITOR.dom.element(row.$.cells[0]);
                newCell.renameNode('td');
                newCell.removeAttribute('scope');
              }
            }
          }

          //Add class for table.
          table.addClass("linkit-table tableHotspotInteraction");

          //set id for table hot spot
          //table.setAttribute('id', responseId);

          // Set the width and height.
          info.txtHeight ? table.setStyle('height', info.txtHeight) : table.removeStyle('height');
          info.txtWidth ? table.setStyle('width', info.txtWidth) : table.removeStyle('width');
          info.chkBorder ? table.addClass('no-border-linkit-table') : table.removeClass('no-border-linkit-table');
          info.audioSrc = $("#audioTableDemo").find('.audioRef').html();
          info.audioSrc ? table.setAttribute('audiotableref', info.audioSrc) : table.removeAttribute('audiotableref');

          if (!table.getAttribute('style'))
            table.removeAttribute('style');
        }

        var audio = makeElement('img');
        var audioSrc = $("#audioTableDemo").find('.audioRef').html();
        audio.addClass("audioTable");
        audio.addClass("bntPlay");
        audio.setAttribute('audioId', audioId);
        audio.setAttribute('audiosrc', audioSrc);
        audio.setAttribute('src', "../../Content/themes/TestMaker/images/small_audio_play.png");

        // Insert the table element if we're creating one.
        if (!this._.selectedElement) {
          if (audioSrc != '') {
            editor.insertElement(audio);
            $("#audioTableDemo").find('.audioRef').html('');
          }
          editor.insertElement(table);
          editor.insertHtml("\u200b");
          // Override the default cursor position after insertElement to place
          // cursor inside the first cell (#7959), IE needs a while.
          setTimeout(function () {
            var firstCell = new CKEDITOR.dom.element(table.$.rows[0].cells[0]);
            var range = editor.createRange();
            range.moveToPosition(firstCell, CKEDITOR.POSITION_AFTER_START);
            range.select();
          }, 0);
        }
        // Properly restore the selection, (#4822) but don't break
        // because of this, e.g. updated table caption.
        else
          try {
            selection.selectBookmarks(bms);
          } catch (er) {}
      },
      contents: [{
        id: 'info',
        label: 'Table Properties',
        elements: [{
            type: 'hbox',
            widths: [null, null],
            styles: ['vertical-align:top'],
            children: [{
                type: 'vbox',
                padding: 0,
                children: [{
                    type: 'text',
                    id: 'txtRows',
                    'default': 3,
                    label: editor.lang.table.rows,
                    required: true,
                    controlStyle: 'width:90px',
                    validate: validatorNum(editor.lang.table.invalidRows),
                    setup: function (selectedElement) {
                      this.setValue(selectedElement.$.rows.length);
                    },
                    commit: commitValue
                  },
                  {
                    type: 'text',
                    id: 'txtCols',
                    'default': 2,
                    label: editor.lang.table.columns,
                    required: true,
                    controlStyle: 'width:90px',
                    validate: validatorNum(editor.lang.table.invalidCols),
                    setup: function (selectedTable) {
                      this.setValue(tableColumns(selectedTable));
                    },
                    commit: commitValue
                  },
                  {
                    type: 'html',
                    html: '&nbsp;'
                  },
                  {
                    type: 'checkbox',
                    id: 'chkBorder',
                    label: hideBorder,
                    setup: function (selectedTable) {
                      if (selectedTable.hasClass("no-border-linkit-table")) {
                        this.setValue(true);
                      } else {
                        this.setValue(false);
                      }

                    },
                    commit: commitValue
                  },
                  {
                    type: 'text',
                    id: 'txtBorder',
                    requiredContent: 'table[border]',
                    // Avoid setting border which will then disappear.
                    'default': editor.filter.check('table[border]') ? 0 : 0,
                    label: editor.lang.table.border,
                    className: 'border-table',
                    controlStyle: 'width:90px;display:none;',
                    validate: CKEDITOR.dialog.validate['number'](editor.lang.table.invalidBorder),
                    setup: function (selectedTable) {
                      this.setValue(selectedTable.getAttribute('border') || '');
                    },
                    commit: function (data, selectedTable) {
                      if (this.getValue())
                        selectedTable.setAttribute('border', this.getValue());
                      else
                        selectedTable.removeAttribute('border');
                    }
                  }
                ]
              },
              {
                type: 'vbox',
                padding: 0,
                children: [{
                    type: 'hbox',
                    widths: ['5em'],
                    children: [{
                      type: 'text',
                      id: 'txtWidth',
                      requiredContent: 'table{width}',
                      controlStyle: 'width:90px',
                      label: editor.lang.common.width,
                      title: editor.lang.common.cssLengthTooltip,
                      // Smarter default table width. (#9600)
                      'default': editor.filter.check('table{width}') ? (editable.getSize('width') < 500 ? '100%' : '500px') : 0,
                      getValue: defaultToPixel,
                      validate: CKEDITOR.dialog.validate.cssLength(editor.lang.common.invalidCssLength.replace('%1', editor.lang.common.width)),
                      onChange: function () {
                        var styles = this.getDialog().getContentElement('advanced', 'advStyles');
                        styles && styles.updateStyle('width', this.getValue());
                      },
                      setup: function (selectedTable) {
                        var val = selectedTable.getStyle('width');
                        this.setValue(val);
                      },
                      commit: commitValue
                    }]
                  },
                  {
                    type: 'hbox',
                    widths: ['5em'],
                    children: [{
                      type: 'text',
                      id: 'txtHeight',
                      requiredContent: 'table{height}',
                      controlStyle: 'width:90px',
                      label: editor.lang.common.height,
                      title: editor.lang.common.cssLengthTooltip,
                      'default': editor.filter.check('table{height}') ? (editable.getSize('height') < 0 ? '100%' : '100px') : 0,
                      getValue: defaultToPixel,
                      validate: CKEDITOR.dialog.validate.cssLength(editor.lang.common.invalidCssLength.replace('%1', editor.lang.common.height)),
                      onChange: function () {
                        var styles = this.getDialog().getContentElement('advanced', 'advStyles');
                        styles && styles.updateStyle('height', this.getValue());
                      },

                      setup: function (selectedTable) {
                        var val = selectedTable.getStyle('height');
                        val && this.setValue(val);
                      },
                      commit: commitValue
                    }]
                  }
                ]
              }
            ]
          },
          {
            type: 'html',
            align: 'right',
            html: audioTableHtml
          },
          {
            type: 'html',
            align: 'right',
            html: ''
          }
        ]
      }]
    };
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

  //build garding hot spot
  function gardingDialog(editor, command) {

    var htmlGardingContent = '';
    var arrItemsDelete = [];

    htmlGardingContent = `
      <div class="garding-hotspot">
        <div class="hotspot-header-action hotspot-header-property m-b-15">
            <div class="select_hotspot g-1-3">
                <p><input typeselect="absolute" type="radio" id="cbA_HotSpot" name="Select_HotSpot" /><label for="cbA_HotSpot">All
                        or Nothing Grading</label></p>
                <p class="cbR_HotSpot"><input typeselect="partial" type="radio" id="cbR_HotSpot" name="Select_HotSpot" /><label
                        for="cbR_HotSpot">Partial Credit Grading</label></p>
                <p class="cbR_HotSpot"><input typeselect="algorithmic" type="radio" id="cbR_algorithmic" name="Select_HotSpot" /><label
                        for="cbR_algorithmic">Algorithmic Grading</label></p>
            </div>
            <div class="g-2-3">
                <div class="fullCredit"><span class="widthLabel">Full Credit Points:</span> <input type="text" value="1"
                        name="fullcreate" id="txtTableSpotFullCredit" class="txtFullcreate" /></div>
            </div>
        </div>
        <div class="clear"></div>
        <div id="listItemTableHotspot"></div>
      </div>
    `;


    return {
      title: 'Table Hot Spot Properties',
      minWidth: IS_V2 ? 650 : 500,
      minHeight: 100,
      resizable: CKEDITOR.DIALOG_RESIZE_NONE,
      contents: [{
        id: 'settingGarding',
        label: 'Settings',
        elements: [{
          type: 'html',
          html: htmlGardingContent,
          onLoad: function () {

          },
          onShow: function () {
            $('#tips').find('.tool-tip-tips').hide();
            var selectGrading = $('input[name="Select_HotSpot"]');

            refreshIdHotSpot(iResult);

            if (iResult[0].sourceHotSpot.arrayList.length === 0) {
              var thisDialog = CKEDITOR.dialog.getCurrent();
              thisDialog.hide();
              alert('Please add hot spots into the item');
              return false;
            }

            //refresh id hot spot
            if (iResult.length) {
              refreshIdHotSpot(iResult);
            }
            //reset garding list hot spot
            resetGarding();

            $('table.cke_dialog').find('.cke_dialog_title').text('Table Hot Spot Properties');

            //load list hot spots
            loadHotSpots(editor, iResult);

            getUpDownNumber($('.txtHotSpot'), 0, 100);
            getUpDownNumber($('.txtFullcreate'), 1, 100);

            //handle checkbox partial garding or absolute garding
            selectGrading.click(function () {
              var typeselect = $(this).attr('typeselect');
              var $txtFullCredit = $('#txtTableSpotFullCredit');
              var $txtPoint = $('.text_point');
              var $hotspotCorrect = $('#listItemTableHotspot tbody tr').find('input[type="checkbox"]');
              var $hotspotPoint = $('#listItemTableHotspot tbody tr').find('.type_checkbox');

              $txtPoint.show();
              $hotspotCorrect.parent('td').show();
              $hotspotCorrect.prop('checked', false);
              $hotspotPoint.find('input[type="text"]').val('0');
              $txtFullCredit.parent().removeClass('is-disabled');
              $txtFullCredit.val('1');

              if (typeselect === 'partial') {
                $txtPoint.text('Point Value');
                $hotspotCorrect.hide();
                $hotspotPoint.removeClass('hiddenCheckbox');
              } else if (typeselect === 'algorithmic') {
                $txtPoint.hide();
                $hotspotCorrect.parent('td').hide();
                $hotspotCorrect.hide();
                $hotspotPoint.addClass('hiddenCheckbox');
                $txtFullCredit.parent().addClass('is-disabled');
                $txtFullCredit.val('0');
              } else {
                $txtPoint.text('Correct');
                $hotspotCorrect.show();
                $hotspotPoint.addClass('hiddenCheckbox');
              }
            });

            //handle btn delete
            $('#listItemTableHotspot tbody tr').find('.delete').each(function () {
              $(this).click(function () {

                if ($('#listItemTableHotspot tbody tr').find('.delete').length === 1) {
                  alert("Please don't empty item into list hot spots");
                  return false;
                }
                var td = $(this).parent('td');
                var tr = td.parent('tr');
                var idDelete = $(this).attr('id');

                arrItemsDelete.push(idDelete);

                tr.remove();
                return false;
              });
            });
          }
        }]
      }],
      onOk: function () {
        var isCorrect = isCorrectAnswer();
        if (!isCorrect) {
          return false;
        }

        var arrayHotSpot = [];
        var arrCorrectResponse = [];
        var relativeGrading = '';
        var absoluteGrading = '';

        var tagTr = $('#listItemTableHotspot tbody tr');
        var fullCreditPoint = $('#txtTableSpotFullCredit');
        var isPartialGrading = $('#cbR_HotSpot').is(':checked');
        var isAlgorithmicGrading = $('#cbR_algorithmic').is(':checked');

        var editable = $(editor.editable().$);

        //update list hot spot into garding and get correct answers
        for (var c_tr = 0, ctrLen = tagTr.length; c_tr < ctrLen; c_tr++) {
          var tr_tag = tagTr[c_tr];
          var td_0 = $(tagTr[c_tr]).find('td')[0];
          var td_1 = $(tagTr[c_tr]).find('td')[1];
          var td_2 = $(tagTr[c_tr]).find('td')[2];

          var typeHotSpot = $(td_0).attr('typeHotSpot');
          var idHotSpot = $(td_1).attr('id');
          var pointValue = $(td_2).find('.txtHotSpot').val();
          var ischecked = $(td_2).find('input[type="checkbox"]');

          if (isPartialGrading) {
            var taginputPointRelative = $(td_2).find('.txtHotSpot');
            if (parseInt(taginputPointRelative.val()) > 0) {
              arrCorrectResponse.push({
                identifier: idHotSpot,
                pointValue: taginputPointRelative.val(),
                isAbsolute: false
              });
            }
          } else if (!isAlgorithmicGrading) {
            var tagInputCheckboxAbsolte = $(td_2).find('input[type="checkbox"]');
            var taginputPointAbsolute = $(td_2).find('.txtHotSpot');
            if (tagInputCheckboxAbsolte.prop('checked') === true) {
              arrCorrectResponse.push({
                identifier: idHotSpot,
                pointValue: taginputPointAbsolute.val(),
                isAbsolute: true
              });
            }
          }

          var element = editable.find('[identifier="' + idHotSpot + '"]')
          var hotSpot = {
            identifier: idHotSpot,
            pointValue: pointValue,
            ischecked: ischecked.prop('checked'),
            typeHotSpot: element.hasClass('bubble') ? 'bubble' : typeHotSpot
          }
          if (hotSpot.typeHotSpot === 'bubble') {
            hotSpot.position = element.hasClass("top") ? 'top' : 'bottom';
            hotSpot.maxHotSpot = $(tagTr).closest('.listTableHotSpot').find('.txtTableHotSpotMaxSelected').val()
          }
          arrayHotSpot.push(hotSpot);
        }
        //update new resource hot spots
        sourceGardingHotSpot.arrayList = arrayHotSpot;

        //update iResult for xmlContent
        for (var iCount = 0, iResultLen = iResult.length; iCount < iResultLen; iCount++) {
          var itemIResult = iResult[iCount];
          if (sourceGardingHotSpot.idtableHotspot === itemIResult.responseIdentifier) {
            itemIResult.correctResponse = arrCorrectResponse;
            itemIResult.sourceHotSpot = sourceGardingHotSpot;
            itemIResult.responseDeclaration.absoluteGrading = '0';
            itemIResult.responseDeclaration.partialGrading = '0';
            itemIResult.responseDeclaration.algorithmicGrading = '0';

            if (isPartialGrading) {
              itemIResult.responseDeclaration.partialGrading = '1';
            } else if (isAlgorithmicGrading) {
              itemIResult.responseDeclaration.algorithmicGrading = '1';
            } else {
              itemIResult.responseDeclaration.absoluteGrading = '1';
            }

            itemIResult.responseDeclaration.pointsValue = fullCreditPoint.val();
          }
        }

        updateHotSpots(editor, iResult, arrItemsDelete);
        sourceGardingHotSpot = '';
        arrItemsDelete = [];

        newResult = iResult;

        if (isAlgorithmicGrading) {
          TestMakerComponent.isShowAlgorithmicConfiguration = true;
        } else {
          TestMakerComponent.isShowAlgorithmicConfiguration = false;
        }
      },
      onCancel: function () {
        sourceGardingHotSpot = '';
        arrItemsDelete = [];
      }
    };
  }
  //build garding hot spot
  function dialogHotSpot(editor, command) {
    var objHotSpot = {};
    var htmlContent = '';
    var objCorrectAnswer = {};
    var arrCorrectAnwer = [];


    htmlContent += '<div class="hotspot-table">';
    htmlContent += '<div class="hotspot-item-garding"><span class="widthLabel">Point Value:</span><input type="text" value="0" name="txtPointValue" id="txtPointValue" class="point"/></div>';
    htmlContent += '<div class="hotspot-item-check"><label for="hotspotCorrect" class="widthLabel">Correct:</label><input type="checkbox" id="hotspotCorrect" name="hotspot-correct"/></div>';
    htmlContent += '<div class="hotspot-item-position" style="display: none">\
                        <label class="widthLabel" for="hotSpotPosition">Hot Spot Position:</label>\
                        <select style="width : 100px; margin-left: 20px;" name="hotSpotPosition" id="hotSpotPosition">\
                            <option selected value="bottom">Bottom</option>\
                            <option value="top">Top</option>\
                        </select>\
                    </div>'
    htmlContent += '</div>';
    htmlContent += '<div style="display: none;" class="popup-image-hotspot" id="popupTableHotspot"></div>';
    htmlContent += '<div style="display: none;" class="popup-overlay" id="popupTableHotspotOverlay"></div>';

    return {
      title: 'Property of the Hot Spot',
      minWidth: IS_V2 ? 400 :298,
      minHeight: 50,
      resizable: CKEDITOR.DIALOG_RESIZE_NONE,
      contents: [{
        id: 'info_tablehotspot',
        label: 'Settings',
        elements: [{
          type: 'html',
          html: htmlContent,
          onLoad: function () {
            getUpDownNumber($('.point'), 0, 100);
          },
          onShow: function () {


            $('#tips').find('.tool-tip-tips').hide();
            var path = editor.elementPath();
            var td = path.contains('td', 1);

            //reset hot spot
            $('#txtPointValue').val('0');
            $('#hotspotCorrect').prop('checked', false);
            $('div[name=info_tablehotspot]').parent('td').css('padding', '0');

            if (eleHotSpot == "") {
              var idOtherSpan = $(td.$).find('span[typehotspot]').attr('identifier');
              var isCheckotherSpan = $(td.$).find('span[typehotspot]').attr('ischecked');
              var pointOtherSpan = $(td.$).find('span[typehotspot]').attr('pointvalue');
              var typeOtherSpan = $(td.$).find('span[typehotspot]').attr('typehotspot');

              objHotSpot = {
                id: idOtherSpan,
                isChecked: isCheckotherSpan,
                point: pointOtherSpan,
                type: typeOtherSpan
              };

              typehospot = (objHotSpot.type === 'checkbox') ? 'CheckBox' : 'Circle';

            } else {
              objHotSpot = {
                id: $(eleHotSpot.$).attr('identifier'),
                isChecked: $(eleHotSpot.$).attr('ischecked'),
                point: $(eleHotSpot.$).attr('pointvalue'),
                type: $(eleHotSpot.$).attr('typehotspot'),
                typeHotSpot: $(eleHotSpot.$).attr('typehotspot'),
                position: $(eleHotSpot.$).hasClass('top') ? 'top' : 'bottom',
                maxHotSpot: $(eleHotSpot.$).attr('maxhotspot')
              };
            }

            $('#txtPointValue').val(objHotSpot.point);
            if (objHotSpot.isChecked === "false") {
              $('#hotspotCorrect').prop('checked', false);
            } else {
              $('#hotspotCorrect').prop('checked', true);
            }
            if ($(eleHotSpot.$).hasClass('bubble')) {
              $('#hotSpotPosition').val(objHotSpot.position || 'top')
              objHotSpot.type = 'bubble';
              $('.hotspot-item-position').show();
            } else {
              $('#hotSpotPosition').val(null);
              delete objHotSpot.position;
              $('.hotspot-item-position').hide();
            }
            //load type garding hot spot
            totalCorrect = loadItemHotSpot(editor, iResult);
            $('table.cke_dialog').find('.cke_dialog_title').text('Property of the Hot Spot');

          }
        }]
      }],
      buttons: [CKEDITOR.dialog.okButton, CKEDITOR.dialog.cancelButton, {
        id: 'delete_name',
        type: 'button',
        label: 'Delete',
        title: 'Delete',
        accessKey: 'C',
        disabled: false,
        onClick: function (evt) {
          var itItem = objHotSpot.id;
          var thisDialog = CKEDITOR.dialog.getCurrent();

          showPopupDelete(editor, 'popupTableHotspot', 'popupTableHotspotOverlay', itItem, thisDialog);

        }
      }],
      onOk: function () {

        var idResItem = '';
        var partialGarding = '';
        var ischecked = $('#hotspotCorrect').prop('checked');
        var pointValueItem = $('#txtPointValue').val();
        var path = editor.elementPath();
        var td = path.contains('td', 1);

        objHotSpot.ischecked = ischecked;

        objHotSpot.point = $('#txtPointValue').val();
        if (parseInt(objHotSpot.point) > 0) {
          objHotSpot.ischecked = true;
        }

        // Validate: selected hotspots cannot be greater than maxhotspot
        var hotspots = td ?
          $(td.$).parents('table').find('span[typehotspot]') :
          $(editor.editable().$).find('span[typehotspot]').not('.tableHotspotInteraction span[typehotspot]');
        var maxHotSpot = hotspots.attr('maxhotspot') || hotspots.parents('table').attr('maxhotspot');
        var selectedIds = hotspots.filter(function() {
          return $(this).hasClass('selected');
        }).map(function () {
          return $(this).attr('identifier');
        }).toArray();
        if (objHotSpot.ischecked && !selectedIds.includes(objHotSpot.id)) {
          selectedIds.push(objHotSpot.id);
        } else if (!objHotSpot.ischecked && selectedIds.includes(objHotSpot.id)) {
          selectedIds.splice(selectedIds.indexOf(objHotSpot.id), 1);
        }
        if (selectedIds.length > +maxHotSpot && iResult[0].responseDeclaration.partialGrading != '1') {
          customAlert('Please increase the maximum number of hot spots a student can select.');
          return false;
        }

        var editHotSpot = new StyleHotSpot(objHotSpot.id, objHotSpot.ischecked, objHotSpot.point, objHotSpot.type, maxHotSpot, $('#hotSpotPosition').val());
        editHotSpot.isChecked = objHotSpot.ischecked;
        var hotspot = editHotSpot.render();
        hotspot = hotspot.replace('&nbsp;', '');

        idResItem = iResult[0].responseIdentifier;

        for (var cItem = 0, lengthiResult = iResult.length; cItem < lengthiResult; cItem++) {
          if (idResItem === iResult[cItem].responseIdentifier) {
            var pointsValue = iResult[cItem].responseDeclaration.pointsValue;
            var iTemiResult = iResult[cItem];
            partialGarding = iResult[cItem].responseDeclaration.partialGrading;

            if (parseInt(partialGarding) === 0) {
              objCorrectAnswer = {
                identifier: objHotSpot.id,
                isAbsolute: true,
                pointValue: pointValueItem
              };

              if (iResult[cItem].correctResponse.length) {

                for (var countCorrect = 0, correctResponseLen = iResult[cItem].correctResponse.length; countCorrect < correctResponseLen; countCorrect++) {
                  if (!ischecked) {
                    if (objHotSpot.id === iResult[cItem].correctResponse[countCorrect].identifier) {
                      iResult[cItem].correctResponse.splice(countCorrect, 1);
                      totalCorrect = iResult[cItem].correctResponse.length;
                      break;
                    }
                  } else {
                    totalCorrect++;
                    var isSelectedCorrect = false;
                    for (var i = 0, arrayCorrectAnwserLen = iResult[cItem].correctResponse.length; i < arrayCorrectAnwserLen; i++) {
                      if (objHotSpot.id === iResult[cItem].correctResponse[i].identifier) {
                        isSelectedCorrect = true;
                        totalCorrect = iResult[cItem].correctResponse.length;
                      }
                    }

                    var isCorrectFit = isCorrectItemAnwser(0, totalCorrect, partialGarding, ischecked, true);
                    if (!isCorrectFit) {
                      return false;
                    }

                    if (!isSelectedCorrect) {
                      iResult[cItem].correctResponse.push(objCorrectAnswer);
                      totalCorrect++;
                      break;
                    }
                  }
                }
              } else {
                if (ischecked) {
                  iResult[cItem].correctResponse.push(objCorrectAnswer);
                  totalCorrect++;
                }
              }

            } else {

              objCorrectAnswer = {
                identifier: objHotSpot.id,
                isAbsolute: false,
                pointValue: pointValueItem
              };

              var isCorrect = selectAnwserCorrect(objHotSpot, iResult[cItem].correctResponse, objCorrectAnswer, pointsValue, totalCorrect, iTemiResult, partialGarding, pointValueItem);
              if (!isCorrect) {
                return false;
              }

              for (var i = 0, corrLen = iResult[cItem].correctResponse.length; i < corrLen; i++) {
                if (objCorrectAnswer.identifier === iResult[cItem].correctResponse[i].identifier) {
                  iResult[cItem].correctResponse[i].pointValue = objCorrectAnswer.pointValue;
                  break;
                }
              }
            }


            for (var k = 0, lengthArray = iResult[cItem].sourceHotSpot.arrayList.length; k < lengthArray; k++) {
              var arrList = iResult[cItem].sourceHotSpot.arrayList;
              if (objHotSpot.id === arrList[k].identifier) {
                arrList[k].identifier = objHotSpot.id;
                arrList[k].pointValue = objHotSpot.point;
                arrList[k].typeHotSpot = objHotSpot.type;
                arrList[k].ischecked = objHotSpot.ischecked;
                arrList[k].maxHotSpot = maxHotSpot;
                arrList[k].inline = td ? '0' : '1';
              }
            }
          }
        }

        var editable = editor.editable();

        var spanTagIe = $(editable.$).find('span[typehotspot]');
        for (var i = 0, leSpanTagIe = spanTagIe.length; i < leSpanTagIe; i++) {
          if ($(spanTagIe[i]).attr('identifier') === objHotSpot.id) {
            $(spanTagIe[i]).replaceWith(hotspot);
            break;
          }
        }

        eleHotSpot = '';
        typehospot = '';
        idHS = '';
        newResult = iResult;
      },
      onCancel: function () {

        eleHotSpot = '';
        typehospot = '';
        idHS = '';
      }
    };
  }
  //remove old item hot spot
  function removeOldHotSpot(editor, idhotspot) {
    var spanHotSpot = editor.document.getElementsByTag('span');
    var arrList = $(spanHotSpot.$);
    var itemHs = '';
    for (var count = 0, lengthSpan = $(spanHotSpot.$).length; 0 < lengthSpan; count++) {
      itemHs = arrList[count];
      if ($(itemHs).attr('identifier') === idhotspot) {
        $(itemHs).remove();
        break;
      }
    }
  }
  //reset garding list hot spots
  function resetGarding() {
    var checkGarding = $('#cbR_HotSpot');
    var fullCreditPoint = $('#txtTableSpotFullCredit');
    var checkAbsolute = $('#cbA_HotSpot');

    checkGarding.prop('checked', false);
    checkAbsolute.prop('checked', true);
    fullCreditPoint.val('1');
    $('.txtTableHotSpotMaxSelected').val('1');

  }
  //load item hot spot
  function loadItemHotSpot(editor, resultHotspot) {
    var idTableHotSpot = iResult[0].responseIdentifier;
    var itemCheck = $('.hotspot-item-check');
    var itemGarding = $('.hotspot-item-garding');
    var totalCorrect = 0;

    for (var i = 0, listHotspotLen = resultHotspot.length; i < listHotspotLen; i++) {
      if (idTableHotSpot === resultHotspot[i].responseIdentifier) {
        var valPartialGarding = resultHotspot[i].responseDeclaration.partialGrading;
        var algorithmicGrading = resultHotspot[i].responseDeclaration.algorithmicGrading;

        itemCheck.find('input[type="checkbox"]').prop('disabled', false);

        if (algorithmicGrading === '1') {
          itemGarding.hide();
          itemCheck.show();
          itemCheck.find('input[type="checkbox"]').prop('disabled', true);
        } else if (valPartialGarding === '0') {
          itemGarding.hide();
          itemCheck.show();
          $('#txtPointValue').val('0');
          if (resultHotspot[i].correctResponse.length) {
            totalCorrect = resultHotspot[i].correctResponse.length;
          }
        } else {
          itemGarding.show();
          itemCheck.hide();
          $('#hotspotCorrect').prop('checked', false);
          for (var j = 0, correctResponseLen = resultHotspot[i].correctResponse.length; j < correctResponseLen; j++) {
            totalCorrect += parseInt(resultHotspot[i].correctResponse[j].pointValue);
          }
        }
      }
    }
    return totalCorrect;
  }
  //load list hot spots
  function loadHotSpots(editor, resultHotspot) {
    var idTableHotSpot = resultHotspot[0].responseIdentifier;
    var thPoint = $('.text_point');
    var bodyContent = $('#listItemTableHotspot');
    var partialGarding = $('#cbR_HotSpot');
    var txtFullPoints = $('#txtTableSpotFullCredit');
    var absoluteGarding = $('#cbA_HotSpot');
    var $algorithmicGarding = $('#cbR_algorithmic');
    var htmlTrTags = '';
    var hiddenInputText = '';
    var hiddenCheckbox = '';
    var isChecked = '';
    var arridHotSpot = '';
    var idThs = '';
    var algorithmicGrading = '0';
    var listTable = [];

    // Table hotspot
    $(editor.editable().$).find('.tableHotspotInteraction').each(function(_, table) {
      var hotSpots = $(table).find('[class*="hotspot-"]').toArray();
      hotSpots.length && listTable.push({ hotSpots, table });
    })
    // Inline hotspot
    var inlineHotSpots = $(editor.editable().$).find('[class*="hotspot-"]').not('.tableHotspotInteraction [class*="hotspot-"]').toArray();
    inlineHotSpots.length && listTable.push({
      hotSpots: inlineHotSpots,
      inline: true
    })

    bodyContent.empty();

    for (var i = 0, listHotspotLen = resultHotspot.length; i < listHotspotLen; i++) {
      if (idTableHotSpot === resultHotspot[i].responseIdentifier) {

        txtFullPoints.val(resultHotspot[i].responseDeclaration.pointsValue);

        var valPartialGarding = resultHotspot[i].responseDeclaration.partialGrading;
        var sourceHotSpot = resultHotspot[i].sourceHotSpot;
        sourceGardingHotSpot = resultHotspot[i].sourceHotSpot;

        algorithmicGrading = resultHotspot[i].responseDeclaration.algorithmicGrading;

        for (var t = 0; t < listTable.length; t++) {
          var itemTable = listTable[t].table;
          var $item = $(listTable[t].hotSpots);
          var title = '';

          if ($item.length > 0) {
            var maxChoice = '1';
            var index = 'inline';
            if (itemTable) {
              index = 'tbl-ths' + (t + 1);
              title = 'Table Hot Spot List';
              if (!itemTable.hasAttribute('index')) {
                itemTable.setAttribute('index', index);
              } else {
                index = itemTable.getAttribute('index');
              }

              if (itemTable.hasAttribute('maxhotspot')) {
                maxChoice = itemTable.getAttribute('maxhotspot');
              }
            } else {
              title = 'Inline Hot Spot List';
              maxChoice = $item.attr('maxhotspot') || '1';
            }
            htmlTrTags = '';

            for (var r = 0; r < $item.length; r++) {
              for (var j = 0; j < sourceHotSpot.arrayList.length; j++) {
                var itemHotSpot = sourceHotSpot.arrayList[j];
                if ($item[r].getAttribute('identifier') == itemHotSpot.identifier) {
                  arridHotSpot = itemHotSpot.identifier.split('_');
                  idThs = arridHotSpot[1];

                  htmlTrTags += '<tr>';
                  htmlTrTags += '<td style="display: none;" typeHotSpot="' + itemHotSpot.typeHotSpot + '"><span class="icon-' + itemHotSpot.typeHotSpot + '">' + itemHotSpot.typeHotSpot + '</span></td>';
                  htmlTrTags += '<td id="' + itemHotSpot.identifier + '"><span class="boxId">' + idThs + '</span></td>';
                  htmlTrTags += '<td>';

                  $('#txtTableSpotFullCredit').parent().removeClass('is-disabled');


                  if (valPartialGarding === '1') {
                    thPoint.text('Point Value');
                    partialGarding.prop('checked', true);
                    hiddenInputText = '';
                    hiddenCheckbox = 'hiddenCheckbox';
                  } else if (algorithmicGrading === '1') {
                    $('#txtTableSpotFullCredit').parent().addClass('is-disabled');
                    hiddenInputText = 'hiddenCheckbox';
                    hiddenCheckbox = 'hiddenCheckbox';
                    $algorithmicGarding.prop('checked', true);
                  } else {
                    thPoint.text('Correct');
                    if (itemHotSpot.ischecked == false || itemHotSpot.ischecked === 'false') {
                      isChecked = '';
                    } else {
                      isChecked = 'checked';
                    }
                    hiddenInputText = 'hiddenCheckbox';
                    hiddenCheckbox = '';
                    absoluteGarding.prop('checked', true);
                  }

                  htmlTrTags += '<input type="checkbox" name="input_' + itemHotSpot.identifier + '" value="" class="_checkbox ' + hiddenCheckbox + '" ' + isChecked + '/>';
                  htmlTrTags += '<div class="type_checkbox ' + hiddenInputText + '"><input name="input_' + itemHotSpot.identifier + '" class="txtHotSpot txt_' + itemHotSpot.typeHotSpot + '" value="' + itemHotSpot.pointValue + '" type="text" /></div></td>';
                  htmlTrTags += '<td><a href="#" id="' + itemHotSpot.identifier + '" class="delete" title="delete">{{delete}}</a></td>';
                  htmlTrTags = htmlTrTags.replace('{{delete}}', IS_V2 ? '<span style="margin-top: -4px;" class="icon icon-remove-threshold"></span>' : 'delete')
                  htmlTrTags += '</tr>';
                }
              }
            }

            var tempTable = `
            <div class="listTableHotSpot" index="`+ index +`">
                <fieldset>
                    <legend>` + title + `</legend>
                    <div style="padding-bottom:20px">
                        <div class="maxHotSpot"><span>Maximum hot spots that can be selected:</span> <input type="text"
                            value="`+ maxChoice +`" name="maxselected" class="txtFullcreate txtTableHotSpotMaxSelected" /></div>
                    </div>
                    <table class="tableHotSpot">
                        <thead>
                            <tr>
                                <th style="display: none;">Style Hot Spot</th>
                                <th>Hot Spot</th>
                                <th class="text_point">Point</th>
                                <th>Delete</th>
                            </tr>
                        </thead>
                        <tbody>
                        ` + htmlTrTags + `
                        </tbody>
                    </table>
                </fieldset>
            </div>
            `;
            bodyContent.append(tempTable);
          }

        }
      }
    }

    $('.text_point').show();
    bodyContent.find('.type_checkbox').parent('td').show();

    if (algorithmicGrading === '1') {
      $('.text_point').hide();
      bodyContent.find('.type_checkbox ').parent('td').hide();
    }
  }
  //check corrects answered
  function isCorrectAnswer() {
    var isAbsoluteGrading = $('#cbA_HotSpot').is(':checked');
    var isAlgorithmicGrading = $('#cbR_algorithmic').is(':checked');
    var $tablehotspot = $('#listItemTableHotspot tbody');
    var msg = '';

    if (isAbsoluteGrading) {
      var listTableHotSpot = $('.listTableHotSpot');
      for(var i = 0; i < listTableHotSpot.length; i++) {
        var $itemTableHotspot = $(listTableHotSpot[i]);

        if (!$itemTableHotspot.find('input[type="checkbox"]:checked').length && $itemTableHotspot.find('tbody tr').length > 0) {
          msg = 'Please select at least one correct answer per table.';
          customAlert(msg);
          return false;
        }
        if (
          $itemTableHotspot.find('input[type="checkbox"]').length < $itemTableHotspot.find('.txtTableHotSpotMaxSelected')[0].value &&
          $itemTableHotspot.find('tbody tr').length > 0
        ) {
          msg = showMaxHotSpotErrorMessage();
          customAlert(msg);
          return false;
        }
        if ($itemTableHotspot.find('input[type="checkbox"]:checked').length > $itemTableHotspot.find('.txtTableHotSpotMaxSelected')[0].value) {
          msg = 'Please increase the maximum number of hot spots a student can select.';
          customAlert(msg);
          return false;
        }
      }

    } else if (!isAlgorithmicGrading) {
      var totalPoint = 0;

      $tablehotspot.find('.txtHotSpot').each(function (ind, checkbox) {
        var $checkbox = $(checkbox);
        var checkboxPoint = parseInt($checkbox.val(), 10);

        totalPoint += checkboxPoint;
      });

      if (totalPoint === 0) {
        msg = 'Please set the point(s) earned for the hot spot.';
        customAlert(msg);
        return false;
      }

      // if ($tablehotspot.find('.txtHotSpot').length < tablehotspotMaxSelected) {
      //   msg = showMaxHotSpotErrorMessage();
      //   popupAlertMessage('alert', msg, 420, 100);
      //   return false;
      // }

      var bool = checkPointPartialGrading();
      if (!bool) {
        msg = showLongErrorMessage();
        customAlert(msg);
        return false;
      }
    }

    return true;
  }

  //check item correct hot spot
  function isCorrectItemAnwser(maxSelected, totalCorrect, partialGrading, ischecked, skipValidateMax) {
    if (parseInt(partialGrading) === 0) {
      if (skipValidateMax) return true;
      var listTableHotSpot = $('.listTableHotSpot');
      for(var i = 0; i < listTableHotSpot.length; i++) {
        var $itemTableHotspot = $(listTableHotSpot[i]);

        if (ischecked && $itemTableHotspot.find('input[type="checkbox"]:checked').length + 1 > $itemTableHotspot.find('.txtTableHotSpotMaxSelected')[0].value) {
          msg = 'Please increase the maximum number of hot spots a student can select.';
          customAlert(msg);
          return false;
        }
      }
    } else {

      if (totalCorrect < parseInt(maxSelected)) {
        alert("Students will not be able to earn the maximum points possible on this question based on the current point allocation." +
          "You can 1) reduce the total points possible on the item," +
          "2) increase the maximum number of hot spots a student can select, and/or " +
          "3) increase the points earned by certain hot spots.");
        return false;
      }
    }
    return true;
  }
  //selection answer correct for table hot spot
  function selectAnwserCorrect(objHotSpot, arrCorrectAnwer, objCorrectAnswer, pointsValue, totalCorrect, iTemiResult, partialGarding, pointValueItem) {

    var pointValueItem = parseInt(pointValueItem);
    var correctResponse = iTemiResult.correctResponse;

    if (correctResponse.length) {
      for (var countCorrect = 0, correctResponseLen = correctResponse.length; countCorrect < correctResponseLen; countCorrect++) {
        if (parseInt(pointValueItem) === 0) {

          var remainCorrectPoint = 0;

          for (var i = 0, arrayCorrectAnwserLen = correctResponse.length; i < arrayCorrectAnwserLen; i++) {
            if (objHotSpot.id !== correctResponse[i].identifier) {
              remainCorrectPoint += parseInt(correctResponse[i].pointValue);
            }
          }
          totalCorrect = pointValueItem + remainCorrectPoint;

          var isCorrectFit = isCorrectItemAnwser(pointsValue, totalCorrect, partialGarding);

          if (!isCorrectFit) {
            return false;
          }

          if (objHotSpot.id === correctResponse[countCorrect].identifier) {
            correctResponse.splice(countCorrect, 1);
            break;
          }

        } else {

          var remainCorrectPoint = 0;
          var isSelectedCorrect = false;

          for (var i = 0, arrayCorrectAnwserLen = correctResponse.length; i < arrayCorrectAnwserLen; i++) {
            if (objHotSpot.id === correctResponse[i].identifier) {
              isSelectedCorrect = true;
            }

            if (objHotSpot.id !== correctResponse[i].identifier) {
              remainCorrectPoint += parseInt(correctResponse[i].pointValue);
            }
          }

          totalCorrect = pointValueItem + remainCorrectPoint;

          var isCorrectFit = isCorrectItemAnwser(pointsValue, totalCorrect, partialGarding);

          if (!isCorrectFit) {
            return false;
          }

          if (!isSelectedCorrect) {
            if (parseInt(iTemiResult.maxSelected) === 1) {
              if (parseInt(objCorrectAnswer.pointValue) < parseInt(iTemiResult.responseDeclaration.pointsValue) && totalCorrect < parseInt(iTemiResult.responseDeclaration.pointsValue)) {
                var msg = showLongErrorMessage();
                customAlert(msg);
                return false;
              }

              correctResponse.push(objCorrectAnswer);
              break;

            } else {

              correctResponse.push(objCorrectAnswer);
              break;
            }
          }
        }
      }
    } else {
      if (parseInt(pointValueItem) > 0) {
        arrCorrectAnwer.push(objCorrectAnswer);
        correctResponse = arrCorrectAnwer;
      }
    }

    return true;
  }
  //update hot spots into editor
  function updateHotSpots(editor, newiResult, arrItemsDelete) {

    var pathTable = editor.elementPath();
    var table = pathTable.contains('table', 1);
    //var idTable = table.$.id;
    var idTable = newiResult[0].responseIdentifier;
    updateItemHotspot(editor, table, newiResult, idTable, arrItemsDelete);
  }
  //update item hot spot
  function updateItemHotspot(editor, table, newiResult, idTable, arrItemsDelete) {
    var updateHotSpot = '';

    var editable = $(editor.editable().$);

    for (var icount = 0, iResultLen = newiResult.length; icount < iResultLen; icount++) {
      var itemResult = newiResult[icount];
      if (itemResult.responseIdentifier === idTable) {
        var sourceHotSpot = itemResult.sourceHotSpot;
        var spanTagIe = editable.find('span[typehotspot]');

        if (table) {
          var tableTags = $(table.$.offsetParent).find('table');
          $.each(tableTags, function () {
            var valueMaxChoice = 1;
            for (var i = 0; i < $('.listTableHotSpot').length; i++) {
              if ($('.listTableHotSpot')[i].getAttribute('index') == this.getAttribute('index')) {
                valueMaxChoice = $($('.listTableHotSpot')[i]).find('.txtTableHotSpotMaxSelected')[0].value;
                break;
              }
            }
            $(this).attr('maxhotspot', valueMaxChoice);
          });
        }

        for (var isource = 0, isourceLen = sourceHotSpot.arrayList.length; isource < isourceLen; isource++) {
          var itemHotSpot = sourceHotSpot.arrayList[isource];

          var maxChoice = $('.listTableHotSpot [id^="' + itemHotSpot.identifier + '"]').closest('.listTableHotSpot').find('.txtTableHotSpotMaxSelected').val() || 1;

          var updateItemHotSpot = new StyleHotSpot(
            itemHotSpot.identifier,
            itemHotSpot.ischecked,
            itemHotSpot.pointValue,
            itemHotSpot.typeHotSpot,
            maxChoice,
            itemHotSpot.position
          );

          var idHotSpotItem = itemHotSpot.identifier;
          updateItemHotSpot.isChecked = itemHotSpot.ischecked;
          if (parseInt(itemHotSpot.pointValue) !== 0) {
            updateItemHotSpot.isChecked = true;
          }

          updateHotSpot = updateItemHotSpot.render();
          updateHotSpot = updateHotSpot.replace('&nbsp;', '');

          for (var i = 0, leSpanTagIe = spanTagIe.length; i < leSpanTagIe; i++) {
            if ($(spanTagIe[i]).attr('identifier') === idHotSpotItem) {
              var table = $(spanTagIe[i]).closest('.tableHotspotInteraction ');
              if (table.length) {
                table.attr('maxhotspot', maxChoice)
              }
              $(spanTagIe[i]).replaceWith(updateHotSpot);
              break;
            }
          }
        }
        //remove item hot spot for event delete
        removeItemHotSpot(arrItemsDelete, spanTagIe);
      }
    }

    return idTable;
  }
  //remove item hot spot for event delete
  function removeItemHotSpot(arrItemsDelete, arrspanTag) {
    if (arrItemsDelete.length) {
      for (var i = 0, arrspanTagLen = arrspanTag.length; i < arrspanTagLen; i++) {
        var tagHotSpot = arrspanTag[i];
        for (var j = 0, arrItemsDeleteLen = arrItemsDelete.length; j < arrItemsDeleteLen; j++) {
          if (arrItemsDelete[j] === $(tagHotSpot).attr('identifier')) {
            $(tagHotSpot).replaceWith("");
          }
        }
      }
    }
  }
  //show pop up delete
  function showPopupDelete(editor, popupTableHotspot, popupTableHotspotOverlay, idItem, thisDialog) {

    var $popupTableHotspot = $('#' + popupTableHotspot);
    var $popupTableHotspotOverlay = $('#' + popupTableHotspotOverlay);
    var html = '';

    html += '<div class="cke_dialog_body cke_dialog_image_hotspot"><div class="cke_dialog_title">Delete Hot Spot</div>';
    html += '<a type="image" title="Remove" class="cke_dialog_close_button" id="btnIClose"><span class="cke_label">X</span></a>';
    html += '<div class="hotspot-list tablehotspot">';
    html += '<span>Are you sure you want to delete this hot spot ?</span>';
    html += '</div>';
    html += '<div class="cke_dialog_footer">';
    html += '<div class="cke_dialog_ui_hbox cke_dialog_footer_buttons">';
    html += '<div class="cke_dialog_ui_hbox_first" role="presentation"><a title="YES" id="btnOk" class="cke_dialog_ui_button cke_dialog_ui_button_ok" role="button" type="hotpot"><span class="cke_dialog_ui_button">Yes</span></a></div>';
    html += '<div class="cke_dialog_ui_hbox_last" role="presentation"><a title="NO" id="btnCancel" class="cke_dialog_ui_button cke_dialog_ui_button_cancel" role="button"><span class="cke_dialog_ui_button">No</span></a></div>';
    html += '</div>';
    html += '</div>';

    $popupTableHotspot
      .html(html)
      .show()
      .draggable({
        cursor: 'move',
        handle: '.cke_dialog_title'
      });

    $popupTableHotspotOverlay.show();

    $popupTableHotspot.on('click', '#btnCancel, #btnIClose', function () {
      $popupTableHotspot.empty().hide();
      $popupTableHotspotOverlay.hide();
    });

    $('#btnOk').click(function () {

      removeOldHotSpot(editor, idItem);
      refreshIdHotSpot(iResult);

      $popupTableHotspot.empty().hide();
      $popupTableHotspotOverlay.hide();
      thisDialog.hide();
    });
  }
  //show error message long
  function showLongErrorMessage() {
    var msg = 'Students will not be able to earn the maximum points possible on this question based on the current point allocation. ' +
      'You can <br/>1) reduce the total points possible on the item,<br/>' +
      '2) increase the maximum number of hot spots a student can select, and/or <br/>' +
      '3) increase the points earned by certain hot spots.';

    return msg;
  }
  //show error message Maximum hot spots
  function showMaxHotSpotErrorMessage() {
    var msg = 'Maximum hot spots that can be selected cannot be greater ' +
      'than the total number of hot spots in the item.';
    return msg;
  }

  /**
   * Check Point Partial Grading
   * @return {[type]} [description]
   */
  function checkPointPartialGrading() {
    var totalPoint = 0;
    var pointArr = [];
    var totalMaxSelected = 0;
    var result = false;

    var fullPoints = parseInt($('#txtTableSpotFullCredit').val(), 10);
    var largest = 0;
    var $tablehotspot = $('#listItemTableHotspot');

    var fullMaxChoice = 0;
    if ($('.txtTableHotSpotMaxSelected').length) {
      for (var i = 0; i < $('.txtTableHotSpotMaxSelected').length; i++) {
        fullMaxChoice += parseInt($('.txtTableHotSpotMaxSelected')[i].value, 10);
      }
    }

    $tablehotspot.find('.txtHotSpot').each(function (ind, checkbox) {
      var $checkbox = $(checkbox);
      var checkboxPoint = parseInt($checkbox.val(), 10);

      totalPoint += checkboxPoint;

      if (checkboxPoint > 0) {
        pointArr.push(checkboxPoint);
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

  CKEDITOR.dialog.add('tablehotspot', function (editor) {
    return tableDialog(editor, 'tablehotspot');
  });
  CKEDITOR.dialog.add('gardingHotSpot', function (editor) {
    return gardingDialog(editor, 'gardingHotSpot');
  });
  CKEDITOR.dialog.add('dialogHotSpot', function (editor) {
    return dialogHotSpot(editor, 'dialogHotSpot');
  });
  CKEDITOR.dialog.add('alertDelete', function (editor) {
    return alertDelete(editor, 'alertDelete');
  });
})();
