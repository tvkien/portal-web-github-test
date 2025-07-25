

function makeAnnounceDialog(args) {
  var closeButton = '';
  var yesButton = '<a class="button-custom-new-skin red-btn" onclick="' + args.cbYesBtnFuncName + '" >OK</a>';
  if (args.cbCloseBtnFuncName && args.cbCloseBtnFuncName.length) {
    closeButton = '<div style="z-index:9999;position:relative;margin-right:0.5em;"><i class="fa-solid fa-xmark float-right fa-xl btn-close-md mt-1" onclick="' + args.cbCloseBtnFuncName + '"></i></div>'
  }
  if (args.strUrl && args.strUrl.length) {
    yesButton = '<a class="button-custom-new-skin red-btn" href="' + args.strUrl + '" target="_blank" onclick="' + args.cbYesBtnFuncName + '" >OK</a>';
  }
  var strHtml = '<section class="grid_5"><div class="block-border container-content">' +
    closeButton +
    '<div class="block-content form" style="padding: 24px;"><div class="main-content">' + args.message + '</div></div>' +
    '<div class="foot-content">' + yesButton + '</div></div ></section > ';
  return strHtml
}

function makeYesNoDialog(args) {
  var closeButton = '';
  var cancelButton = '';
  var yesButton = '<a class="button-custom-new-skin red-btn" onclick="' + args.cbYesBtnFuncName + '" >OK</a>';
  if (args.cbCloseBtnFuncName && args.cbCloseBtnFuncName.length) {
    closeButton = '<div style="z-index:9999;position:relative;margin-right:0.5em; cursor: pointer"><i class="fa-solid fa-xmark float-right fa-xl btn-close-md mt-1" onclick="' + args.cbCloseBtnFuncName + '"></i></div>'
  }
  if (args.cbCancelBtnFuncName && args.cbCancelBtnFuncName.length) {
    cancelButton = '<a class="button-custom-new-skin grey-btn" onclick="' + args.cbCancelBtnFuncName + '" >Cancel</a>';
  }
  if (args.strUrl && args.strUrl.length) {
    yesButton = '<a class="button-custom-new-skin red-btn" href="' + args.strUrl + '" target="_blank" onclick="' + args.cbYesBtnFuncName + '" >OK</a>';
  }
  var strHtml = '<section class="grid_5"><div class="block-border container-content">' +
    closeButton +
    '<div class="block-content form" style="padding: 24px;"><div class="main-content">' + args.message + '</div></div>' +
    '<div class ="foot-content" style="justify-content: space-around;">'
    + cancelButton + yesButton + '</div></div ></section > ';
  return strHtml
}


function makeConfirmAssignTestDialog(args) {
  var closeButton = '';
  var cancelButton = '';
  var note = '<div style="font-size: 12px; line-height: 14px;padding-top:10px;">Note: If you are trying to assign this as a post-test, please have the original test author uncheck the "Overwrite Test Results" option in Test Design -> Test Properties</div>';
  var yesButton = '<a class="button-custom-new-skin red-btn" onclick="' + args.cbYesBtnFuncName + '" >Generate</a>';
  if (args.cbCloseBtnFuncName && args.cbCloseBtnFuncName.length) {
    closeButton = '<div style="z-index:9999;position:relative;margin-right:0.5em;"><i class="fa-solid fa-xmark float-right fa-xl btn-close-md mt-1"  onclick="' + args.cbCloseBtnFuncName + '"></i></div>'
  }
  if (args.cbCancelBtnFuncName && args.cbCancelBtnFuncName.length) {
    cancelButton = '<a class="button-custom-new-skin grey-btn" onclick="' + args.cbCancelBtnFuncName + '" >Cancel</a>';
  }
  if (args.strUrl && args.strUrl.length) {
    yesButton = '<a class="button-custom-new-skin red-btn" href="' + args.strUrl + '" target="_blank" onclick="' + args.cbYesBtnFuncName + '" >Generate</a>';
  }
  var strHtml = '<section class="grid_5"><div class="block-border container-content">' +
    closeButton +
    '<div class="block-content form" style="padding: 24px;"><div class="main-content">' + args.message +
    '</div>' + note + '</div>' +
    '<div class="foot-content" style="justify-content:space-around;">'
    + cancelButton + yesButton + '</div></div ></section > ';
  return strHtml
}
