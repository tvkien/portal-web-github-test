/**
 * @license Copyright (c) 2003-2013, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

(function () {
    var defaultToPixel = CKEDITOR.tools.cssLength;
    var audioTableHtml = `\
      <div id="audioTablePropUploadContent">\
          <table cellspacing="0" border="0" align="left" style="width:100%;">\
              <tbody>\
                  <tr>\
                      <td>\
                          <div>\
                              <div class="cke_dialog_ui_labeled_content cke_dialog_ui_input_file">\
                                  <form name="form-upload" id="formTablePropAudioUpload" lang="en" action="uploader.php?type=mp3" dir="ltr" method="POST" enctype="multipart/form-data">\
                                      <input type="file" size="38" name="file" aria-labelledby="cke_262_label" id="formTablePropAudioUploadFile" style="max-width: 100%; overflow: hidden; text-overflow: ellipsis; width: 94%;height: auto;border: solid 1px #cccccc;">\
                                      <input type="hidden" name="id" id="objectId" />\
                                  </form>\
                              </div>\
                          </div>\
                      </td>\
                      <td>\
                          <a id="uploadAudioTablePropButton" role="button" class="cke_dialog_ui_fileButton cke_dialog_ui_button" hidefocus="true" title="Upload" href="javascript:void(0)" style="-moz-user-select: none;float: right;">\
                              <span class="cke_dialog_ui_button">Upload</span>\
                          </a>\
                      </td>\
                  </tr>\
                  <tr>\
                      <td colspan="2">\
                          <div id="demoAudioTableProp" style="width:72%; -moz-user-select: none; display:none; background:#026211; border-radius:25px;padding: 3px 0; margin: 10px auto;">\
                              <table>\
                                  <tr>\
                                      <td style="vertical-align:middle; text-align:left;">\
                                          <div class="audio" id="audioTablePropDemo" style="margin-left:-35px">\
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
        $("#audioTablePropDemo .audioRef").html(data.absoluteUrl);
        $("#demoAudioTableProp").show();
        $('#audioTablePropDemo .bntStop').hide().end().find('#audioTablePropDemo .bntPlay').show();
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
      var formTablePropAudioUploadButton = $('#uploadAudioTablePropButton');
      var formTablePropAudioUploadFile = $('#formTablePropAudioUploadFile');

      formTablePropAudioUploadButton.click(function () {
        refeshConfig();
        var file = formTablePropAudioUploadFile.get(0).value;

        if (file == "") {
          customAlert("Please select audio file.");
          return;
        }
        var extension = file.substr((file.lastIndexOf('.') + 1));

        if (extension.toLowerCase() != "mp3") {
          customAlert("Unsupported file type. Please select mp3 file.");
          return;
        }
        $("#audioTablePropUploadContent").find('#objectId').val(objectId);

        audioUpload($('#formTablePropAudioUpload').get(0), audioConfig);

        return false;
      });

      formTablePropAudioUploadFile.change(function () {
        stopVNSAudio();
        resetUIAudio();
      });

      //Handlers when controls is clicked
      $("#audioTablePropDemo .bntPlay").click(playVNSAudio);
      $("#audioTablePropDemo .bntStop").click(stopVNSAudio);
    }

    function onShowTableAudio(audioref, hasAudio) {
      if (audioref && hasAudio) {
        $('#formTablePropAudioUploadFile').val('');
        $("#audioTablePropDemo .audioRef").html(audioref);
        $('#demoAudioTableProp').show();

      } else {
        $('#formTablePropAudioUploadFile').val('');
        $("#audioTablePropDemo .audioRef").html('');
        $('#demoAudioTableProp').hide();
      }
    }

    function tableDialog(editor, command) {
      var makeElement = function (name) {
        return new CKEDITOR.dom.element(name, editor.document);
      };

      var editable = editor.editable();

      var dialogadvtab = editor.plugins.dialogadvtab;

      return {
        title: editor.lang.table.title,
        minWidth: IS_V2 ? 500 : 310,
        minHeight: CKEDITOR.env.ie ? 310 : 100,

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
          // Detect if there's a selected table.
          var selection = editor.getSelection(),
            ranges = selection.getRanges(),
            table;

          var rowsInput = this.getContentElement('info', 'txtRows'),
            colsInput = this.getContentElement('info', 'txtCols'),
            widthInput = this.getContentElement('info', 'txtWidth'),
            heightInput = this.getContentElement('info', 'txtHeight');

          if (command == 'tableProperties') {
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

          if (!$(this._.selectedElement.$).prev().find('.audioTable')) {
            table.$.setAttribute('audiotableref', '');
          }
          var audioRefTable = table.$.getAttribute('audiotableref');
          var audioid = table.$.getAttribute('audioid');
          var hasAudio = false;

          if ($(this._.selectedElement.$).parent().find("img.audioTable[audioid='"+ audioid +"']").length) {
            hasAudio = true;
          }
          onShowTableAudio(audioRefTable, hasAudio);
        },
        onOk: function () {
          //add type table hot spot
          var typeTableHotSpot = '';

          for (var i = 0, iResultLen = iResult.length; i < iResultLen; i++) {
            if (iResult[i].type === 'tableHotSpot') {
              typeTableHotSpot = 'tableHotspotInteraction';
              break;
            }
          }

          var selection = editor.getSelection(),
            bms = this._.selectedElement && selection.createBookmarks();

          var table = this._.selectedElement || makeElement('table'),
            data = {};

          this.commitContent(data, table);

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
            table.addClass("linkit-table " + typeTableHotSpot + "");

            // Set the width and height.
            info.txtHeight ? table.setStyle('height', info.txtHeight) : table.removeStyle('height');
            info.txtWidth ? table.setStyle('width', info.txtWidth) : table.removeStyle('width');
            info.chkBorder ? table.addClass('no-border-linkit-table') : table.removeClass('no-border-linkit-table');
            info.audioSrc = $("#audioTablePropDemo").find('.audioRef').html();
            info.audioSrc ? table.setAttribute('audiotableref', info.audioSrc) : table.removeAttribute('audiotableref');

            if (!table.getAttribute('style'))
              table.removeAttribute('style');
          }

          var audio = makeElement('img');
          var audioSrc = $("#audioTablePropDemo").find('.audioRef').html();
          audio.addClass("audioTable");
          audio.addClass("bntPlay");
          audio.setAttribute('audiosrc', audioSrc);
          audio.setAttribute('src', "../../Content/themes/TestMaker/images/small_audio_play.png");

          // Insert the table element if we're creating one.
          if (!this._.selectedElement) {
            if (audioSrc != '') {
              editor.insertElement(audio);
              $("#audioTablePropDemo").find('.audioRef').html('');
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
          else {
            var audioId = this._.selectedElement.$.getAttribute('audioid');
            if (!audioId) {
              audioId = new Date().getTime();
              this._.selectedElement.$.setAttribute('audioid', audioId);
            }

            if (audioSrc != '') {
              var itemHasAudio = null;
              var listAudio = this._.selectedElement.getParent().find('.audioTable').$;
              for (var i = 0; i < listAudio.length; i++) {
                if (listAudio[i].getAttribute('audioid') == audioId) {
                  itemHasAudio = listAudio[i];
                  break;
                }
              }

              if (itemHasAudio) {
                itemHasAudio.setAttribute('audiosrc', audioSrc);
              } else {
                audio.setAttribute('audioid', audioId);
                this._.selectedElement.insertBeforeMe(audio);
              }
            }
            try {
              selection.selectBookmarks(bms);
            } catch (er) {}
          }
        },
          contents: [{
            id: 'info',
            label: editor.lang.table.title,
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

      CKEDITOR.dialog.add('tableProperties', function (editor) {
        return tableDialog(editor, 'tableProperties');
      });
    })();
