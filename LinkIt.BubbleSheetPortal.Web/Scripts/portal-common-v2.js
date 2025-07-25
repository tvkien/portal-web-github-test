
function portalV2AddPluginSelect() {
  //var elSelect = $('select');
  //for (var k = 0; k < elSelect.length; k++) {
  //  $(elSelect[k]).select2({
  //    minimumResultsForSearch: Infinity
  //  });

  //  $(elSelect[k]).next('.select2').keydown(function (event) {
  //    var $selection = $(this).find('.select2-selection');
  //    var ariaOwns = $selection.attr('aria-owns');
  //    var idDropdown = '#' + ariaOwns;
  //    var filterByKey = $(idDropdown).find('li').filter(function () {
  //      var _liText = $(this).get(0).innerText;
  //      if (_liText) {
  //        return _liText[0].toLowerCase() == event.key;
  //      }
  //      return false ;
  //    });
  //    if (filterByKey.length) {
  //      var $selectionId = $(filterByKey[0]).attr('id');
  //      $selection.attr('aria-activedescendant', $selectionId);
  //      $('#' + $selectionId).get(0).scrollIntoView();
  //      $('#' + $selectionId).addClass('select2-results__option--highlighted');
  //    }
  //  })
  //}
}
function portalV2SkinCheckBox(selector) {
  selector = selector || '';
  var elCheckbox = $(selector + ' input[type=checkbox]');
  for (var index = 0; index < elCheckbox.length; index++) {
    if (elCheckbox[index].checked) {
      $(elCheckbox[index]).addClass('input-checked-v2');
    } else {
      $(elCheckbox[index]).removeClass('input-checked-v2');
    }
    $(elCheckbox[index]).change(function () {
      setCheckBoxClassV2Skin($(this).is(':checked'), this);
    })
  }
}

function toogleCheckboxV2Skin(isChecked, elm) {
  $(elm).prop('checked', isChecked);
  setCheckBoxClassV2Skin(isChecked, elm);
}

function setCheckBoxClassV2Skin(isChecked, elm) {
  if (isChecked) {
    $(elm).addClass('input-checked-v2')
  } else {
    $(elm).removeClass('input-checked-v2')
  }
}

function portalV2SkinRadio() {
  var elRadio = $('input[type=radio]');
  for (var i = 0; i < elRadio.length; i++) {
    $(elRadio[i]).change(function () {
      var nameRadio = $(this).get(0).name;
      var queryString = 'input[type=radio][name=' + nameRadio + ']';
      var radioItem = $(queryString);
      for (var j = 0; j < radioItem.length; j++) {
        $(radioItem.get(j)).removeClass('input-checked-v2');
      }
      $(this).addClass('input-checked-v2');
    })
  }
}

function BindNewCkbValue(elementName, applyElement) {
  var value = $('input[name="' + elementName + '"]:checked').val();
  if (value == "1") {
    $('input[name="' + applyElement + '"]').prop('checked', true);
  } else {
    $('input[name="' + applyElement + '"]').prop('checked', false);
  }

  $("input[name='" + applyElement + "']").change(function () {
    var checked = $(this).is(":checked");
    if (checked) {
      $('input[name="' + elementName + '"][value="1"]').prop('checked', true);
    } else {
      $('input[name="' + elementName + '"][value="0"]').prop('checked', true);
    }

    $('input[name="' + elementName + '"]').change();
  });

  var isDisable = $('input[name="' + elementName + '"]').is(':disabled');
  if (isDisable) {
    $('input[name="' + applyElement + '"]').attr('disabled', 'disabled');
  } else {
    $('input[name="' + applyElement + '"]').removeAttr('disabled', 'disabled');
  }
}

function swapFooterTable() {
  $('.block-footer.clearfix:parent').each(function () {
    $(this).insertBefore($(this).prev('#classDataTable_info'));
    $(this).insertBefore($(this).prev('#dataTable_info'));
  });
}

function tranformSearchInputDataTable(str) {

  var elSearchLabel = $('#' + str + ' label');
  var elSearchInput = elSearchLabel.find('input');
  if (elSearchInput.length) {
    elSearchInput.get(0).style.setProperty('padding-left', '32px', 'important');
  }
  elSearchLabel.replaceWith(elSearchInput);
  $('#' + str).addClass('data-search');

};

function confirmMessageV2(configData, config, callback) {
  config = config || {};
  var hbody = $("body").height() - 109;
  if (config.modal && config.modal.hbody) {
    hbody = config.modal.hbody;
  };

  var strHtml = makeYesNoDialog(configData);
  var elDiv = $("<div></div>")
    .html(strHtml)
    .addClass("dialog dialog-custom-new-skin")
    .attr("id", "messageDialog")

  if (config.dialogAttr) {
    for (var key in config.dialogAttr) {
      var attrValue = config.dialogAttr[key];
      if (key == 'attr' && Object.prototype.toString.call(attrValue) === "[object Object]") {
        for (var attrKey in attrValue) {
          elDiv.attr(attrKey, attrValue[attrKey])
        }
      } else {
        elDiv[key](attrValue)
      }
    }
  };

  if (window.CKEDITOR && window.CKEDITOR.dialog) {
    $.ui.dialog.maxZ = Math.max($.ui.dialog.maxZ, CKEDITOR.dialog._.currentZIndex)
  }

  if (config.modal && config.modal.dialog) {
    elDiv.dialog(config.modal.dialog)
  } else {
    var option = config.option || {};
    var defaultOption = {
      open: function () {
        $(this).parents('.ui-dialog').find('.ui-dialog-titlebar-close').remove();
      },
      close: function () {
        $(this).remove();
        var elParent = $(this).parents('.ui-dialog');
        if (elParent) {
          elParent.remove()
        }
      },
      modal: true,
      width: 500,
      maxheight: 400,
      resizable: false
    }
    option = Object.assign(defaultOption, option)
    elDiv
      .appendTo("body")
      .dialog(option);
  };

  $(".ui-dialog").css("height", hbody);
  if (callback) {
    callback();
  }


};

var customAlert = function (msg, config) {
  config = config || {};
  msg = msg.split('\n').join('<br>');
  config.contentStyle = config.contentStyle || {};

  var dialogId = 'dialog' + new Date().getTime();
  var defaultConfig = {
    open: function () {
      if ($(this).find('.content').outerHeight() < 34) {
        $('.content').css('text-align', 'center');
      }
      $(this).parents('.ui-dialog').find('.ui-dialog-titlebar-close').remove()
      $(this).parents('.ui-dialog').prepend('<div id="backdrop-' + dialogId +
        '" style="width: 100vw;height: 100vh;position: fixed;left: 0;top: 0;background: rgba(0,0,0,0.3);"></div>');
      if (config.ZIndex > 0){
        $(this).parents('.ui-dialog').css('z-index', config.ZIndex);
      }
    },
    close: function () {
      $(this).dialog('destroy');
      if (config && config.close) {
        config.close();
      }
      $('#backdrop-' + dialogId).remove();
    },
    modal: true,
    width: 'auto',
    maxHeight: '400px',
    resizable: false
  }
  var dialogConfig = Object.assign(defaultConfig, config);

  var contentMessage = '<div class="content" style="max-width:' + (config.contentStyle.maxWidth || '640') + 'px;min-width:' + (config.contentStyle.minWidth || '300') +'px;height:unset;padding:0;">{{content}}</div>'
  confirmMessageV2(
      {
          message: contentMessage.replace('{{content}}', msg || ''),
      },
      {
          dialogAttr: {
              attr: {
                  id: dialogId
              }
          },
          modal: {
            dialog: dialogConfig
          }
      }
  );
  var $dialog = $('#' + dialogId);
  $('#' + dialogId + ' .button-custom-new-skin.red-btn').click(function() {
    $dialog.dialog('close');
  })
}

var customConfirm = function(
  msg,
  customConfig
) {
  var config = {
    minWidth: '300px',
    maxWidth: '640px',
    buttons: [
      { label: 'Cancel', color: 'grey', style: "background: none;", returnValue: false },
      { label: 'Yes', color: 'red', returnValue: true },
    ]
  }
  customConfig && Object.assign(config, customConfig);
  msg = msg.split('\n').join('<br>');
  var dialogId = 'dialog' + new Date().getTime();
  confirmMessageV2(
      {
          message: '<div class="content" style="max-width: {{maxWidth}}; min-width: {{minWidth}}; height: unset;">{{content}}</div>'
            .replace('{{maxWidth}}', config.maxWidth)
            .replace('{{minWidth}}', config.minWidth)
            .replace('{{content}}', msg || '')
      },
      {
          dialogAttr: { attr: { id: dialogId } },
          modal: {
            dialog: {
              open: function () {
                if ($(this).find('.content').outerHeight() < 34) {
                  $('.content').css('text-align', 'center');
                }
                $(this).parents('.ui-dialog').find('.ui-dialog-titlebar-close').remove();
                $(this).parents('.ui-dialog').prepend('<div id="backdrop-' + dialogId +
                  '" style="width: 100vw;height: 100vh;position: fixed;left: 0;top: 0;background: rgba(0,0,0,0.3);"></div>');
                if (config.ZIndex > 0){
                  $(this).parents('.ui-dialog').css('z-index', config.ZIndex);
                }
              },
              close: function () {
                $(this).dialog('destroy');
                $('#backdrop-' + dialogId).remove();
              },
              modal: true,
              width: 'auto',
              maxHeight: '400px',
              resizable: false
            }
          }
      }
  );
  var $dialog = $('#' + dialogId);
  return new Promise(function(resolve) {
    var $footer = $dialog.find('.foot-content').html('');
    config.buttons.forEach(function(btn) {
      var btnText = '<a class="button-custom-new-skin {color}-btn" style="{style}">{label}</a>'
        .replace('{color}', btn.color || '')
        .replace('{style}', btn.style || '')
        .replace('{label}', btn.label || '');
      $footer.append($(btnText).on('click', function() {
        $dialog.dialog('close');
        btn.callback && btn.callback();
        resolve(btn.returnValue);
      }))
    })
  });
}

function upperFistCharacter(str) {
  var arr = str.split(' ');
  arr = arr.map(function (item) {
    return item.slice(0, 1).toUpperCase() + item.slice(1, item.length).toLowerCase();
  })
  return arr.join(' ')
}


function addBreadcrumbHeader(parentTitle, childTitle) {
  var parentNodeBreadcrumb = $('#main-nav .menu_ul > li[title="' + parentTitle + '"]');
  parentNodeBreadcrumb.addClass("current");
  var childNodeBreadcrumb = parentNodeBreadcrumb.find('.dropdown-menu.submenu > li a[title="' + childTitle +'"]');
  childNodeBreadcrumb.parent().addClass("current");

  renderBreadcrumb(parentNodeBreadcrumb.html(), childNodeBreadcrumb.parent().html())
}

$(function () {
  // init slect2 for select elemtent
  portalV2AddPluginSelect();

  // set background input checkbox
  portalV2SkinCheckBox();

  portalV2SkinRadio();
})
