

function restoreZIndexWhenCloseADialog(dialogContentId) {
  if (!dialogContentId) {
    dialogContentId = '#CustomAlertDialog_ConditionalLogic';
  }
  $(dialogContentId).parent().css("z-index", 1002);
  $(".ui-widget-overlay").css("z-index", 1001);
}
function setZIndexWhenOpenADialog(dialogContentId) {
  if (!dialogContentId) {
    dialogContentId = '#CustomAlertDialog_ConditionalLogic';
  }
  $(dialogContentId).parent().css("z-index", 99999);
  $(".ui-widget-overlay").css("z-index", 99998);
}

function CustomAlertYesClick_ConditionalLogic(item) {
  $("#CustomAlertDialog_ConditionalLogic").dialog("close");
  var url = $(item).attr('url');
  if (url != undefined && $.trim(url) != '') window.open(url);
}
//Author: TuanVo
function CustomAlertConditionalLogic(message, isLongMessage, customWidth) {
  var yesButton = '<button style="width:63px;margin-left:0px;" onclick="CustomAlertYesClick_ConditionalLogic(this);">OK</button>';
  var messageTextAlign = 'center';
  var messageBoxWidth = 240;
  var diaglogWidth = 300;


  if (typeof isLongMessage != "undefined" && isLongMessage == true) {
    messageTextAlign = 'left';
    messageBoxWidth = 540;
    diaglogWidth = 600;
  }
  if (typeof customWidth != "undefined" && customWidth > 0) {
    messageBoxWidth = 400;
  }


  var strHtml = '<section class="grid_5">' +
    '<div class="block-border" style="width: ' + messageBoxWidth + 'px;">' +
    '<div class="block-content form" style="padding-bottom: 1em;text-align:center;"><div style="text-align:' + messageTextAlign + ';line-height: 18px;">' + message +
    '</div><div style="text-align: center;padding-top:20px;padding-bottom:10px;">' + yesButton + '</div></div></div></section>';
  $("<div></div>")
    .html(strHtml)
    .addClass("dialog")
    .attr("id", "CustomAlertDialog_ConditionalLogic")
    .appendTo("body")

    .dialog({
      close: function () {
        restoreZIndexWhenCloseADialog();
        $(this).remove();
      },
      modal: true,
      width: diaglogWidth,
      maxheight: 400,
      resizable: false,
      open: function () {
        $("#CustomAlertDialog_ConditionalLogic").parent().find('.ui-dialog-titlebar-close').hide();
        setZIndexWhenOpenADialog();

      }
    });
}

var option___ConditionalLogic;
function CustomConfirmPopupConditionalLogic(option, btnYesName, btnNoName) {
  // Force options to be an object
  option___ConditionalLogic = option || {};

  if (option.message == 'undefined' || option.message == null) {
    option.message = '';
  }

  if (option.textLeft == 'undefined' || option.textLeft == null) {
    option.textLeft = false;
  }
  var captionYes, captionNo;

  if (btnYesName == 'undefined') {
    captionYes = "Yes";
  } else {
    captionYes = btnYesName;
  }

  if (btnNoName == 'undefined') {
    captionNo = "No";
  } else {
    captionNo = btnNoName;
  }

  var yesButton = '<button style="margin-left:0px;" onclick="' + "$(this).attr('disabled','disabled');option___ConditionalLogic.yes();$('#CustomConfirmDialog__ConditionalLogic').dialog('close')" + ';">' + captionYes + '</button>';
  var noButton = '<button style="margin-left:20px;" onclick="' + "option___ConditionalLogic.no();$('#CustomConfirmDialog__ConditionalLogic').dialog('close')" + ';">' + captionNo + '</button>';
  var messageTextAlign = 'center';


  var messageBoxWidth = 240;
  var diaglogWidth = 300;
  var diaglogId = 'CustomConfirmDialog__ConditionalLogic';

  if (option.message.length >= 90) {
    messageTextAlign = 'left';
    messageBoxWidth = 540;
    diaglogWidth = 600;
  }

  var style = 'style="width: ' + messageBoxWidth + 'px;"';

  if (option.textLeft && option.message.length < 90 || option.textCenter && option.message.length < 90) {
    messageTextAlign = 'left';
    if (option.textCenter)
      messageTextAlign = 'center';
    diaglogWidth = 'auto';
    messageBoxWidth = 420;
    style = 'style="width: auto;max-width: ' + messageBoxWidth + 'px; min-width: 280px;"';
  }

  var strHtml = '<section class="grid_5">' +
    '<div class="block-border"' + style + ' >' +
    '<div class="block-content form" style="padding-bottom: 1em;text-align:center;"><div style="text-align:' + messageTextAlign + ';line-height: 18px;">' + option.message +
    '</div><div style="text-align: center;padding-top:20px;padding-bottom:10px;">' + yesButton + noButton + '</div></div></div></section>';
  $("<div></div>")
    .html(strHtml)
    .addClass("dialog")
    .attr("id", diaglogId)
    .appendTo("body")

    .dialog({
      close: function () {
        restoreZIndexWhenCloseADialog('#CustomConfirmDialog__ConditionalLogic');
        $(this).remove();
        if (typeof option___ConditionalLogic.close == 'function') {
          option___ConditionalLogic.close();
        }
      },
      modal: true,
      width: diaglogWidth,
      maxheight: 400,
      resizable: false,
      open: function () {
        $("#CustomConfirmDialog__ConditionalLogic").parent().find('.ui-dialog-titlebar-close').hide();
        if (typeof option___ConditionalLogic.open == 'function') {
          option___ConditionalLogic.open();
        }
        setZIndexWhenOpenADialog('#CustomConfirmDialog__ConditionalLogic');
        //$(".ui-dialog-titlebar-close").hide();

      }
    });
}

function removeAllMultiPartExpression(testMakerComponent) {
  if (!this.hasConditionalLogics(testMakerComponent)) {
    if (IS_V2) {
      customAlert("There is nothing to remove!", { ZIndex: 10000 });
    } else {
      CustomAlertConditionalLogic("There is nothing to remove!");
    }
  } else {
    var confirmMessage = 'This action will remove all conditional logic from this question. Are you sure you want to proceed?';
    var onSuccess = function () {
      var self = testMakerComponent;
      self.listMultiPartExpression = [];
      self.listMultiPartExpression = [{
        expression: '*',
        enableElements: [],
        rules: []
      }];
      testMakerComponent.closePopupMultiPartConfig();
    }
    if (IS_V2) {
      customConfirm(confirmMessage).then(function(result) {
        result && onSuccess();
      })
    } else {
      CustomConfirmPopupConditionalLogic({
        message: confirmMessage,
        yes: onSuccess,
        no: function () {
          //location.reload();
        },
        open: function () {
        },
        close: function () {
        },
      },"OK","Cancel"); 
    }
  }
};
function hasConditionalLogics(testMakerComponent) {
  var self = testMakerComponent;
  var multiPartExpression = self.listMultiPartExpression;
  var hasConditional = false;
  if (multiPartExpression) {
    if (multiPartExpression.length > 1) {
      hasConditional = true;
    }
    else if (this.firstConditionHasResponse()) {
      hasConditional = true;
    }
    return hasConditional;
  }
};
function firstConditionHasResponse() {
  var enableElements = $('#enableElement1').tagit('assignedTags');
  return enableElements.length > 0;
};
