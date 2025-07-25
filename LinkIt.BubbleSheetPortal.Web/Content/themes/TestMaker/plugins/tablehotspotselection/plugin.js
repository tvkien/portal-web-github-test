/**
 * @license Copyright (c) 2003-2013, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

CKEDITOR.plugins.add('tablehotspotselection', {
  requires: 'dialog',
  lang: 'en', // %REMOVE_LINE_CORE%
  icons: 'tablehotpot', // %REMOVE_LINE_CORE%
  hidpi: true, // %REMOVE_LINE_CORE%
  init: function (editor) {
    if (editor.blockless)
      return;

    var arrObjItem = [];
    var arrObjHotSpot = [];
    var objHotSpot = {};

    var objfullSubmenu = {
      tablehotspotdelete: CKEDITOR.TRISTATE_OFF,
      deletecellhotspot: CKEDITOR.TRISTATE_OFF,
      cellhotspot: CKEDITOR.TRISTATE_OFF,
      tablehotspot: CKEDITOR.TRISTATE_OFF,
      gardingHotSpot: CKEDITOR.TRISTATE_OFF
    };

    var ojbpartSubmenu = {
      tablehotspotdelete: CKEDITOR.TRISTATE_OFF,
      tablehotspot: CKEDITOR.TRISTATE_OFF
    };

    var table = CKEDITOR.plugins.tablehotspotselection,
      lang = editor.lang.tablehotspotselection;


    function createDef(def) {
      return CKEDITOR.tools.extend(def || {}, {
        contextSensitive: 1,
        refresh: function (editor, path) {
          this.setState(path.contains('table', 1) ? CKEDITOR.TRISTATE_OFF : CKEDITOR.TRISTATE_DISABLED);
        }
      });
    }

    editor.addCommand('tablePropertiesHotSpot', new CKEDITOR.dialogCommand('tablePropertiesHotSpot', createDef()));
    editor.addCommand('tableDeleteHotSpot', createDef({
      exec: function (editor) {
        var path = editor.elementPath(),
          table = path.contains('table', 1);

        var tableHotSpot = '';

        if (!table)
          return;

        // If the table's parent has only one child remove it as well (unless it's the body or a table cell) (#5416, #6289)
        var parent = table.getParent();
        tableHotSpot = parent;

        if (parent.getChildCount() == 1 && !parent.is('body', 'td', 'th'))
          table = parent;

        var idTable = iResult[0].responseIdentifier;
        var range = editor.createRange();
        range.moveToPosition(table, CKEDITOR.POSITION_BEFORE_START);

        var tableAudioId = table.getAttribute('audioid');
        if (tableAudioId) {
          var listAudio = table.getParent().find('.audioTable').$;
          for (var i = 0; i < listAudio.length; i++) {
            if (listAudio[i].getAttribute('audioid') == tableAudioId) {
              $(listAudio[i]).remove();
              break;
            }
          }
        }

        table.remove();

        refreshIdHotSpot(iResult);

        range.select();
      }
    }));
    editor.addCommand('deleteCellHotSpot', createDef({
      exec: function (editor) {
        var path = editor.elementPath(),
          td = path.contains('td', 1);

        if (!td)
          return;

        // If the table's parent has only one child remove it as well (unless it's the body or a table cell) (#5416, #6289)
        var parent = td.getParent();
        //var idtableHotspot = parent.$.offsetParent.id;
        var idtableHotspot = iResult[0].responseIdentifier;
        if (parent.getChildCount() == 1 && !parent.is('body', 'td', 'th'))
          td = parent;

        //remove hot spot when choose button delete
        if ($(td.$).find('span[typehotspot]').length) {
          var idRemoveHotSpot = $(td.$).find('span[typehotspot]').attr('identifier');
          for (var j = 0; j < iResult[0].sourceHotSpot.arrayList.length; j++) {
            if (idRemoveHotSpot === iResult[0].sourceHotSpot.arrayList[j].identifier) {
              iResult[0].sourceHotSpot.arrayList.splice(j, 1);
              break;
            }
          }
          //delete anwser correct hot spot
          for (var i = 0; i < iResult.length; i++) {
            if (idtableHotspot === iResult[i].responseIdentifier) {
              if (iResult[i].correctResponse.length) {
                for (var j = 0; j < iResult[i].correctResponse.length; j++) {
                  if (idRemoveHotSpot === iResult[i].correctResponse[j].identifier) {
                    iResult[i].correctResponse.splice(j, 1);
                    break;
                  }
                }
              }
            }
          }
          $(td.$).find('span[typehotspot]').remove();
        }
      }
    }));

    editor.addCommand('gardingHotSpot', new CKEDITOR.dialogCommand('gardingHotSpot'));

    editor.addCommand('dialogHotSpot', new CKEDITOR.dialogCommand('dialogHotSpot', createDef()));

    //add check box into table hot spot
    editor.addCommand('cmdCheckboxHotSpot', {
      exec: function (editor) {
        createHotSpot(editor, 'checkbox', arrObjItem, arrObjHotSpot, objHotSpot);
      }
    });

    //add circle box into table hot spot
    editor.addCommand('cmdCircleHotSpot', {
      exec: function (editor) {
        createHotSpot(editor, 'circle', arrObjItem, arrObjHotSpot, objHotSpot);
      }
    });

    CKEDITOR.dialog.add('tablehotspot', this.path + 'dialogs/tablehotspot.js');
    CKEDITOR.dialog.add('tablePropertiesHotSpot', this.path + 'dialogs/tablepropertieshotspot.js');
    CKEDITOR.dialog.add('gardingHotSpot', this.path + 'dialogs/tablehotspot.js');
    CKEDITOR.dialog.add('dialogHotSpot', this.path + 'dialogs/tablehotspot.js');

    var createGroupMenuHotSpot = function () {
      // If the "menu" plugin is loaded, register the menu items.
      if (editor.addMenuGroup) {
        editor.addMenuGroup('hotspot');
      }

      var listHotSpot = {
        tablehotspotdelete: {
          label: 'Delete Table',
          command: 'tableDeleteHotSpot',
          group: 'hotspot',
          order: 1
        },
        deletecellhotspot: {
          label: 'Delete Cell Hot Spot',
          command: 'deleteCellHotSpot',
          group: 'hotspot',
          order: 2
        },
        cellhotspot: {
          label: 'Cell Properties Hot Spot',
          command: 'dialogHotSpot',
          group: 'hotspot',
          order: 4
        },
        tablehotspot: {
          label: 'Table Properties',
          command: 'tablePropertiesHotSpot',
          group: 'hotspot',
          order: 5
        },
        gardingHotSpot: {
          label: 'Setting Grading Hot Spots',
          command: 'gardingHotSpot',
          group: 'hotspot',
          order: 6
        },
        styleHotSpot: {
          label: 'Add Style Hot Spot',
          group: 'hotspot',
          getItems: function () {
            return {
              checkboxHotSpot: CKEDITOR.TRISTATE_OFF,
              circleHotSpot: CKEDITOR.TRISTATE_OFF
            };
          },
          order: 7
        },
        checkboxHotSpot: {
          label: 'Checkbox Hot Spot',
          group: 'hotspot',
          command: 'cmdCheckboxHotSpot',
        },
        circleHotSpot: {
          label: 'Circle Hot Spot',
          group: 'hotspot',
          command: 'cmdCircleHotSpot',
        }

      };

      if (editor.addMenuItems) {
        // If the "menu" plugin is loaded, register the group menu.
        editor.addMenuItems(listHotSpot);
      }
    };

    editor.on('doubleclick', function (evt) {
      var element = evt.data.element;

      if (element.is('table'))
        evt.data.dialog = 'tablePropertiesHotSpot';
      if (element.hasClass('hotspot-checkbox') || element.hasClass('hotspot-circle')) {
        eleHotSpot = element;

        typehospot = ($(eleHotSpot.$).attr('typehotspot') === 'checkbox') ? 'CheckBox' : 'Circle';
        evt.data.dialog = 'dialogHotSpot';
      }

    });

    // If the "contextmenu" plugin is loaded, register the listeners.
    if (editor.contextMenu) {
      //create menu hot spot
      createGroupMenuHotSpot();
      editor.contextMenu.addListener(function (element, selection) {
        // menu item state is resolved on commands.
        $('#tips').find('.tool-tip-tips').hide();
        if ($(element.$).find('span[typehotspot]').length === 0) {
          if (CKEDITOR.env.ie) {
            if ($(element.$).attr('typehotspot') != undefined) {
              return objfullSubmenu;
            }
          }
          return ojbpartSubmenu;
        } else {
          return ojbpartSubmenu;
        }

      });
    }

    editor.ui.addButton('GardingHotSpot', {
      label: lang.garding,
      command: 'gardingHotSpot',
      toolbar: 'tablehotspot,30'
    });

    editor.ui.addButton('TableHotSpot', {
      label: lang.toolbar,
      command: 'createtablehotspot',
      toolbar: 'tablehotspot,30'
    });


    editor.addCommand('createtablehotspot', new CKEDITOR.dialogCommand('createtablehotspot'));

    CKEDITOR.dialog.add('createtablehotspot', function (api) {
      // CKEDITOR.dialog.definition
      var htmlString = '';

      htmlString += '<div class="create_hot_spot">';
      htmlString += '<div class="hotspot-item"><input typehotspot="checkbox" type="radio" id="hotspot_checkbox" name="hotspot_style"/><label for="hotspot_checkbox" class="widthLabel">Check Box</label></div>';
      htmlString += '<div class="hotspot-item"><input typehotspot="circle" type="radio" id="hotspot_circle" name="hotspot_style"/><label for="hotspot_circle" class="widthLabel">Circle</label></div>';
      htmlString += '<div class="hotspot-item"><input typehotspot="bubble" type="radio" id="hotspot_bubble" name="hotspot_style"/><label for="hotspot_bubble" class="widthLabel">Bubble Spot</label></div>';
      htmlString += '</div>';


      var dialogDefinition = {
        title: 'Style Hot Spot',
        minWidth: 350,
        minHeight: 50,
        contents: [{
          id: 'info_hotspot',
          label: 'tableHot Spot',
          elements: [{
            type: 'html',
            html: htmlString,
            onLoad: function () {

            },
            onShow: function () {

              //reset dialog style hot spot
              $('#hotspot_checkbox').prop('checked', true);
              $('#hotspot_circle').prop('checked', false);

              $('table.cke_dialog').find('.cke_dialog_title').text('Style Hot Spot');
            }
          }]
        }],
        buttons: [CKEDITOR.dialog.okButton, CKEDITOR.dialog.cancelButton],
        onOk: function () {
          var inputs = $('input[name=hotspot_style]');
          var typehotspot = '';

          $(inputs).each(function () {
            if (this.checked) {
              typehotspot = $(this).attr('typehotspot');
            }
          });

          $('a.cke_button.cke_button__gardinghotspot').show();
          createHotSpot(editor, typehotspot, arrObjItem, arrObjHotSpot, objHotSpot);
        },
        onCancel: function () {

        }
      };

      return dialogDefinition;

    });
  }
});

/***
 * Create TH ID when create a new Hot Spot
 ***/
function createIdHotSpot() {
  var $body = $('iframe.cke_wysiwyg_frame').contents().find('body');
  var $listHotSpot = $body.find('span[typeHotSpot]');

  var idHotSpot = "THS_" + ($listHotSpot.length + 1);
  for (var count = 0, lengthListHotSpot = $listHotSpot.length; count < lengthListHotSpot; count++) {
    var resId = "THS_" + (count + 1);
    if ($($listHotSpot[count]).attr('identifier') != resId) {

      var isOnlyOne = true;
      for (var k = 0, lengthList = $listHotSpot.length; k < lengthList; k++) {
        if (resId == $($listHotSpot[k]).attr('identifier')) {
          isOnlyOne = false;
        }
      }

      if (isOnlyOne) {
        idHotSpot = resId;
        break;
      }

    }
  }

  return idHotSpot;
}
/***
 *create a new Hot Spot
 ***/
function createHotSpot(editor, typeHotSpot, arrObjItem, arrObjHotSpot, objHotSpot) {
  var path = editor.elementPath();
  var td = path.contains('td', 1);
  var idHotSpot = createIdHotSpot();

  if (td) {
    // If the table's parent has only one child remove it as well (unless it's the body or a table cell) (#5416, #6289)
    var parent = td.getParent();

    if (parent.getChildCount() == 1 && !parent.is('body', 'td', 'th'))
      td = parent;

    var maxHotSpot = $(td.$).closest('.tableHotspotInteraction').attr('maxhotspot') || 1;
    var itemHotSpot = new StyleHotSpot(idHotSpot, false, '0', typeHotSpot, maxHotSpot);
    var htmlItem = itemHotSpot.render();

    //td.setHtml(htmlItem);
    td.appendHtml(htmlItem);
    $(td.$).find('br').remove();
  } else {
    var maxInlineHotSpot = $(editor.editable().$).find('span[typehotspot]').not('.tableHotspotInteraction span[typehotspot]').attr('maxhotspot') || 1;
    var itemHotSpot = new StyleHotSpot(idHotSpot, false, '0', typeHotSpot, maxInlineHotSpot);
    var htmlItem = itemHotSpot.render();
    editor.insertHtml(htmlItem)
  }

  refreshIdHotSpot(iResult);

  objHotSpot = {
    identifier: itemHotSpot.identifier,
    pointValue: itemHotSpot.pointValue,
    typeHotSpot: itemHotSpot.typeHotSpot,
    ischecked: false
  };

  if (!td) {
    objHotSpot.inline = '1';
    objHotSpot.maxHotSpot = maxInlineHotSpot;
    objHotSpot.position = 'bottom';
  }

  arrObjHotSpot = iResult[0].sourceHotSpot;

  iResult[0].sourceHotSpot.arrayList = iResult[0].sourceHotSpot.arrayList.filter(function(item) {
    return item.identifier != objHotSpot.identifier;
  })

  if (iResult[0].sourceHotSpot.arrayList.length) {
    for (var i = 0, lenArrObjHotSpot = iResult[0].sourceHotSpot.arrayList.length; i < lenArrObjHotSpot; i++) {
      if (objHotSpot.identifier !== iResult[0].sourceHotSpot.arrayList[i].identifier) {
        iResult[0].sourceHotSpot.arrayList.push(objHotSpot);
        break;
      }
    }
  } else {
    iResult[0].sourceHotSpot.arrayList.push(objHotSpot);
  }
}

/***
 * build style table hot spot
 ***/
function StyleHotSpot(id, isChecked, pointValue, typeHotSpot, maxhotspot, position) {
  this.identifier = id;
  this.isChecked = false;
  this.pointValue = pointValue;
  this.typeHotSpot = typeHotSpot;
  this.maxhotspot = maxhotspot || 1;
  this.position = position || 'top';
}
StyleHotSpot.prototype.render = function () {
  var htmlHotSpot = '';

  if (this.typeHotSpot === 'checkbox') {
    if (this.isChecked == true) {
      htmlHotSpot += '<span typeHotSpot="checkbox" pointvalue="' + this.pointValue + '" ischecked="' + this.isChecked + '" contenteditable="false" class="hotspot-checkbox selected" identifier="' + this.identifier + '" maxhotspot="' + this.maxhotspot + '">';
    } else {
      htmlHotSpot += '<span typeHotSpot="checkbox" pointvalue="' + this.pointValue + '" ischecked="' + this.isChecked + '" contenteditable="false" class="hotspot-checkbox" identifier="' + this.identifier + '" maxhotspot="' + this.maxhotspot + '">';
    }
    htmlHotSpot += '&#8203;</span>&nbsp;';

  }

  if (this.typeHotSpot === 'circle') {
    if (this.isChecked == true) {
      htmlHotSpot += '<span typeHotSpot="circle" pointvalue="' + this.pointValue + '" ischecked="' + this.isChecked + '" contenteditable="false" class="hotspot-circle selected" identifier="' + this.identifier + '" maxhotspot="' + this.maxhotspot + '">';
    } else {
      htmlHotSpot += '<span typeHotSpot="circle" pointvalue="' + this.pointValue + '" ischecked="' + this.isChecked + '" contenteditable="false" class="hotspot-circle" identifier="' + this.identifier + '" maxhotspot="' + this.maxhotspot + '">';
    }
    htmlHotSpot += '&#8203;</span>&nbsp;';

  }

  if (this.typeHotSpot === 'bubble') {
    if (this.isChecked == true) {
      htmlHotSpot += '<span typeHotSpot="circle" pointvalue="' + this.pointValue + '" ischecked="' + this.isChecked + '" contenteditable="false" class="hotspot-circle bubble ' + this.position + ' selected" identifier="' + this.identifier + '" maxhotspot="' + this.maxhotspot + '">';
    } else {
      htmlHotSpot += '<span typeHotSpot="circle" pointvalue="' + this.pointValue + '" ischecked="' + this.isChecked + '" contenteditable="false" class="hotspot-circle bubble ' + this.position + '" identifier="' + this.identifier + '" maxhotspot="' + this.maxhotspot + '">';
    }
    htmlHotSpot += '&#8203;</span>&nbsp;';
  }
  return htmlHotSpot;
};
