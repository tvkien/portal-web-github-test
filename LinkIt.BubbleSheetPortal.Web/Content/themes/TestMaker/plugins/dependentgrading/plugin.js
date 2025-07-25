CKEDITOR.plugins.add('dependentgrading', {
    lang: 'en', // %REMOVE_LINE_CORE%
    icons: 'imageupload',
    requires: 'dialog',
    hidpi: true, // %REMOVE_LINE_CORE%
    init: function (editor) {

        editor.addCommand('dependentGrading', new CKEDITOR.dialogCommand('dependentGrading'));

        editor.ui.addButton('dependentGrading',
		    {
            label: 'Multi-Part Properties',
		        command: 'dependentGrading',
		        icon: this.path + 'icons/image.png',
		        toolbar: 'dependentGrading,30'
		    });

        editor.widgets.add('dependentgrading', {
            inline: true,
            mask: true
        });

        var eleDepenentGrading = ""; //This use for first time load popup

        window.editImage = false;

        editor.on('doubleclick', function (evt) {

            var ele = evt.data.element;

            if (ele.hasClass("dependentgrading")) {
                evt.data.dialog = 'dependentGrading';

                eleImage = ele;
                eleDepenentGrading = ele; //This use for first time load popup

                //The status to editor know this is update
                window.editImage = true;

            }
        });

        CKEDITOR.dialog.add('dependentGrading', function (editor) {
            var checkChanged = false,
                checkDrawable = false,
                formUpload = null,
                formUploadButton = null,
                formImageUploadFile = null,
                selectPercent = null,
                selectAlignment = null,
                checkboxDrawable = null,
                previewImage = null;

            myhtml = '\
                    <style>\
                        #previewImage{ max-width:400px; max-height:200px;}\
                    </style>\
                    <div id="wrap_'+editor.name+'_dialog">\
                        <table cellspacing="0" border="0" align="left" style="width:100%;" role="presentation">\
                            <tbody>\
                                <tr>\
                                    <td class="cke_dialog_ui_vbox_child" role="presentation" colspan="2">\
                                        <table class="cke_dialog_ui_hbox" role="presentation">\
                                            <tbody>\
                                                <tr class="cke_dialog_ui_hbox">\
                                                    <td style="width:50%;" role="presentation" class="cke_dialog_ui_hbox_first">\
                                                        <div class="cke_dialog_ui_vbox" role="presentation">\
                                                            <table cellspacing="0" border="0" align="left" style="width:100%;" role="presentation">\
                                                                <thead>\
                                                                    <tr>\
                                                                      <th style="text-align: left;font-weight: bold;font-size: 14px;padding-bottom: 5px;">Major</th>\
                                                                    </tr>\
                                                                 </thead>\
                                                                <tbody style="background: #eee;border: 1px solid #999999;" class="radioMajor">\
                                                                </tbody>\
                                                            </table>\
                                                        </div>\
                                                    </td>\
                                                    <td style="width:50%;" role="presentation" class="cke_dialog_ui_hbox_last">\
                                                        <div class="cke_dialog_ui_vbox" role="presentation">\
                                                            <table cellspacing="0" border="0" align="left" style="width:100%;" role="presentation">\
                                                                <thead>\
                                                                    <tr>\
                                                                      <th style="text-align: left;font-weight: bold;font-size: 14px;padding-bottom: 5px;">Depending</th>\
                                                                    </tr>\
                                                                 </thead>\
                                                                <tbody style="background: #eee;border: 1px solid #999999;" class="checkboxDepending">\
                                                                </tbody>\
                                                            </table>\
                                                        </div>\
                                                    </td>\
                                                </tr>\
                                            </tbody>\
                                        </table>\
                                    </td>\
                                </tr>\
                            </tbody>\
                        </table>\
                      </div>';
          tablePartialGrading = '<table cellspacing="0" border="0" align="left" style="width:100%;" role="presentation">\
                                          <tbody>\
                                              <tr>\
                                                  <td class="cke_dialog_ui_vbox_child" role="presentation" colspan="2">\
                                                      <table class="cke_dialog_ui_hbox" role="presentation">\
                                                          <tbody>\
                                                              <tr class="cke_dialog_ui_hbox">\
                                                                  <td style="width:50%;" role="presentation" class="cke_dialog_ui_hbox_first">\
                                                                      <div class="cke_dialog_ui_vbox" role="presentation">\
                                                                          <table cellspacing="0" border="0" align="left" style="width:100%;" role="presentation">\
                                                                              <thead>\
                                                                                  <tr>\
                                                                                    <th style="text-align: left;font-weight: bold;font-size: 14px;padding-bottom: 5px;">Major</th>\
                                                                                  </tr>\
                                                                               </thead>\
                                                                              <tbody style="background: #eee;border: 1px solid #999999;" class="radioMajor">\
                                                                              </tbody>\
                                                                          </table>\
                                                                      </div>\
                                                                  </td>\
                                                                  <td style="width:50%;" role="presentation" class="cke_dialog_ui_hbox_last">\
                                                                      <div class="cke_dialog_ui_vbox" role="presentation">\
                                                                          <table cellspacing="0" border="0" align="left" style="width:100%;" role="presentation">\
                                                                              <thead>\
                                                                                  <tr>\
                                                                                    <th style="text-align: left;font-weight: bold;font-size: 14px;padding-bottom: 5px;">Depending</th>\
                                                                                  </tr>\
                                                                               </thead>\
                                                                              <tbody style="background: #eee;border: 1px solid #999999;" class="checkboxDepending">\
                                                                              </tbody>\
                                                                          </table>\
                                                                      </div>\
                                                                  </td>\
                                                              </tr>\
                                                          </tbody>\
                                                      </table>\
                                                  </td>\
                                              </tr>\
                                          </tbody>\
                                      </table>';
          allOrNothingGrading = '<div id="allOrNothingGrading" class="form" style="display:flex;align-items: center;justify-content: space-between;">\
                                    <p class="title-small" style="font-weight: 500 !important;">Absolute grading point: </p>\
                                    <input id="allOrNothingGradingScore" style="padding:0 8px !important" type="text" name="score" min="0" value="1"/>\
                                  </div>'
          myhtml_V2 = '\
                    <style>\
                        #previewImage{ max-width:400px; max-height:200px;}\
                    </style>\
                    <div id="wrap_'+ editor.name +'_dialog">\
                      <div class="custom-radio-checkbox">\
                        <input id="partial-grading" type="radio" value="partial-grading" checked="checked" name="multi-part-option">\
                        <label class="title-medium" for= "partial-grading" > Partial Grading</label>\
                      </div >\
                      <div style="display: block;width: 100%;margin: .5rem 0;">\
                        <p class="title-small" style="font-weight: 500 !important;">Note: This option allows you to assign specific point values to each part of the question by double-clicking on each part.</p>\
                        <p class="title-small" style="font-weight: 500 !important;">The total point value for the question is calculated as the sum of the point values assigned to each part.</p>\
                      </div>\
                      <div class="custom-radio-checkbox">\
                        <input id="dependent-grading" type="radio" value="dependent-grading" name="multi-part-option">\
                        <label class="title-medium" for="dependent-grading">Dependent Grading</label>\
                      </div>\
                      <div id="tablePartialGrading" style="display: inline-block;width: 100%;margin: .5rem 0;">'
                         + tablePartialGrading +
                      '</div>\
                      <div class="custom-radio-checkbox">\
                        <input id="all-or-nothing-grading" type="radio" value="all-or-nothing-grading" name="multi-part-option">\
                        <label class="title-medium" for="all-or-nothing-grading">All or Nothing Grading</label>\
                      </div>'
                      + allOrNothingGrading +
                    '</div>';
            return {
                title: 'Multi-Part Properties',
                minWidth: IS_V2 ? 660 : 550,
                minHeight: 200,
                contents:
		        [
			        {
			            id: 'imageUploadExe',
			            label: 'Settings',
			            elements:
				        [
					        {
					            type: 'html',
                      html: IS_V2 ? myhtml_V2 : myhtml,
                      onShow: function () {
					                /*load data when press ok*/
					                refreshResponseId();
					                checkElementRemoveIntoIResult();

					                temps = jQuery.extend(true, [], currData);

					                loadDataforFieldMajor(iResult);
					                loadDataforFieldDepending(iResult);
					                hightlight();
                          if (IS_V2) {
                            getUpDownNumber('#allOrNothingGradingScore', 0, 1000);
                            handleLoadMultiPartOption();
                            handleChangeMultiPartOption();
                            checkApproveAllOrNothingGrading(iResult)
                          }

					              }
					        }
				        ]
			        }
		        ],
              onOk: function () {
                if (!IS_V2 || modeMultiPartGradingTemp == 'dependent-grading') {
                  // Deep copy
                  currData = jQuery.extend(true, [], temps);
                }
                if (IS_V2) {
                  modeMultiPartGrading = modeMultiPartGradingTemp
                  allOrNothingGradingScore = null
                  if (modeMultiPartGradingTemp == 'all-or-nothing-grading') {
                    allOrNothingGradingScore = $('#allOrNothingGradingScore').val()
                  }
                }
                isButtonSaved = true;
                newResult = iResult;
                },
                onCancel: function () {
                  temps = [];
                }
            };
        });
    }
});
/***
* Check Field Major
* with fields inlineChoiceInteraction,textEntryInteraction and choiceInteraction
***/
function checkFieldMajor(result) {
    var textType = result.type;
    if ((textType == 'inlineChoiceInteraction') || (textType == 'textEntryInteraction') || (textType == 'choiceInteraction')) {

        if (textType == 'textEntryInteraction' && result.responseDeclaration.method == "manual") {
            return false;
        } else {
            return true;
        }

    }
    return false;
}
/***
* Load data to Field Major
* Return: responseIdentifier
***/
function loadDataforFieldMajor(results) {
    var tr = "";
    for (var i = 0; i < results.length; i++) {
        var result = results[i],
            textType = result.type,
            responseIdentifier,
            text;

        if (checkFieldMajor(result)) {

            switch (textType) {
                case 'inlineChoiceInteraction':
                    text = 'Inline Choice';
                    responseIdentifier = result.responseIdentifier;
                    break;
                case 'textEntryInteraction':
                    text = 'Text Entry';
                    responseIdentifier = result.responseIdentifier;
                    break;
                case 'choiceInteraction':
                    responseIdentifier = result.responseIdentifier;
                    if (result.maxChoices == 0) {
                        text = 'Multi Select';
                    } else {
                        text = 'Multiple Choice';
                    }
                    break;
                default:
                    text = '';
                    responseIdentifier = '';
                    break;
            }
            tr += '<tr class="trMajor">';
            tr += '<td role="presentation">';
            tr += '<div style="width : 100%; padding-top: 6px;padding-bottom: 6px;" class="cke_dialog_ui_select" role="presentation">';
            tr += '<div aria-labelledby="cke_272_label" role="radiogroup" class="cke_dialog_ui_labeled_content">';
            tr += '<div role="presentation" class="cke_dialog_ui_input_select">';
            tr += '<input type="radio" value="' + responseIdentifier + '" name="major" class="checkboxDrawable" id="radio_' + i + '" style="margin-right:10px;margin-left: 10px;margin-top: 1px;"><label style="vertical-align:top;" for="radio_' + i + '" class="cke_dialog_ui_labeled_label">' + text + '(' + responseIdentifier + ')</label>';
            tr += '</div>';
            tr += '</div>';
            tr += '</div>';
            tr += '</td>';
            tr += '</tr>';
        }
    }
    if (results.length == 0 || tr == "")
    {
        tr = '<tr class="trMajorEmpty"><td class="cke_dialog_ui_vbox_child" role="presentation" style="padding-top: 6px;padding-left: 14px;">Major is empty!</td></tr>';
    }
    $('.radioMajor').html(tr);
}
/***
* Load data to Field Depending
* Return: responseIdentifier
***/
function loadDataforFieldDepending(results) {
    var tr = "";
    for (var i = 0; i < results.length; i++) {
        var result = results[i],
            textType = result.type,
            responseIdentifier = result.responseIdentifier;

        switch (textType) {
            case 'inlineChoiceInteraction':
                textType = 'Inline Choice';
                break;
            case 'textEntryInteraction':
                textType = 'Text Entry';
                break;
            case 'extendedTextInteractionDraw':
                textType = 'Drawing Interaction';
                break;
            case 'extendedTextInteraction':
                textType = 'Constructed Response';
                break;
            case 'choiceInteraction':
                if (result.maxChoices == 0) {
                    textType = 'Multi Select';
                } else {
                    textType = 'Multiple Choice';
                }
                break;
            default:
                textType = '';
                break;
        }
        tr += '<tr  class="trDepending">';
        tr += '<td role="presentation">';
        tr += '<div style="width : 100%; padding-top: 6px;padding-bottom: 6px;" class="cke_dialog_ui_select" role="presentation">';
        tr += '<div aria-labelledby="cke_272_label" role="radiogroup" class="cke_dialog_ui_labeled_content">';
        tr += '<div role="presentation" class="cke_dialog_ui_input_select">';
        tr += '<input type="checkbox" value="' + responseIdentifier  + '" name="depending" class="checkboxDrawable" id="checkbox_' + i + '" style="margin-right:10px;margin-left: 10px;margin-top: 1px;"><label style="vertical-align:top;" for="checkbox_' + i + '" class="cke_dialog_ui_labeled_label">' + textType + '(' + responseIdentifier + ')</label>';
        tr += '</div>';
        tr += '</div>';
        tr += '</div>';
        tr += '</td>';
        tr += '</tr>';
    }

    if (results.length == 0 || $(".trMajorEmpty").length > 0) {
        tr = '<tr class="trDependingEmpty"><td class="cke_dialog_ui_vbox_child" role="presentation" style="padding-top: 7px; padding-left: 10px;">Depending is empty!</td></tr>';
    }
    $('.checkboxDepending').html(tr);
}
/*
* hight light
* choosen
*/
function hightlight() {
    var newMajor = {};

    // add hight light into label
    $('.radioMajor').unbind('click').on('click', 'tr td input[type=radio]', function () {

        var valueItem = $(this).val();
        $('.radioMajor tr td').find('.cke_dialog_ui_select').removeClass('hight-light');
        $(this).parents('.cke_dialog_ui_select').addClass('hight-light');

        //Show all item of depending
        $('.checkboxDepending .trDepending').show();

        //Show check and hide on depending
        for (var i = 0; i < temps.length; i++) {
            if (temps[i].major == valueItem) {
                for (var n = 0; n < temps[i].depending.length; n++) {

                    //Hide major has check is depending
                    $('.radioMajor tr td').find('input[value="' + temps[i].depending[n] + '"]').parents(".trMajor").hide();

                    var currentInput = $('.checkboxDepending tr td input[value="' + temps[i].depending[n] + '"]');
                    currentInput.attr('checked', 'checked');
                    currentInput.parents('.cke_dialog_ui_select').addClass('hight-light');
                }
            } else {
                for (var n = 0; n < temps[i].depending.length; n++) {
                    $('.checkboxDepending tr td input[value="' + temps[i].depending[n] + '"]').parents(".trDepending").hide();
                    $('.radioMajor tr td input[value="' + temps[i].depending[n] + '"]').parents(".trMajor").hide();
                }
            }

            //Hide depending if major has depending
            $('.checkboxDepending tr td input[value="' + temps[i].major + '"]').parents(".trDepending").hide();
        }

        //Remove depending's item of current major
        $('.checkboxDepending tr td input[value="' + valueItem + '"]').parents(".trDepending").hide();
    });

    $('.checkboxDepending').unbind('click').on('click', 'tr td input[type="checkbox"]', function () {
        var val_depending = $(this).val();
        //This is add new depending to major
        if (this.checked) {
            $(this).parents('.cke_dialog_ui_select').addClass('hight-light');
            var major = $('.radioMajor tr td').find('input[name=major]:checked').val();
            var isNewMajor = true;
            //Check major has exist or not
            for (var i = 0; i < temps.length; i++) {
                if (temps[i].major == major) {
                    temps[i].depending.push(val_depending);
                    isNewMajor = false;
                }
            }
            if (isNewMajor) {
                newMajor = {
                    'major': major,
                    'depending': []
                };

                newMajor.depending.push(val_depending);
                temps.push(newMajor);
            }
            //Hide on major column
            $('.radioMajor tr td').find('input[value="' + val_depending + '"]').parents(".trMajor").hide();
        } else {
            //This is remove depending from major
            $(this).parents('.cke_dialog_ui_select').removeClass('hight-light');
            //Check major has exist or not
            for (var i = 0; i < temps.length; i++) {
                if (temps[i].depending.length > 0) {
                    for (var d = 0; d < temps[i].depending.length; d++) {
                        if (temps[i].depending[d] == val_depending) {
                            temps[i].depending.splice(d, 1);
                            break;
                        }
                    }
                }

                //Remove major has depending is 0
                if (temps[i].depending.length == 0) {
                    temps.splice(i, 1);
                    i = i - 1;
                    if (i < 0) {
                        i = 0;
                    }
                }
            }
            $('.radioMajor tr td').find('input[value="' + val_depending + '"]').parents(".trMajor").show();
        }
    });

    //Hide element on major after fire event click
    for (var i = 0; i < temps.length; i++) {
        for (var n = 0; n < temps[i].depending.length; n++) {
            $('.radioMajor tr td input[value="' + temps[i].depending[n] + '"]').parents(".trMajor").hide();
        }
    }

    //Trigger click as default to select the first item of Major
    $('.radioMajor tr td input[type=radio]:visible:first').trigger("click");
}
var results = [];
var isButtonSaved = false;
var modeMultiPartGradingTemp = null
var allOrNothingGradingScoreTemp = null

function showHideOptionMultiPart() {
  if (modeMultiPartGradingTemp == 'dependent-grading') {
    $("#tablePartialGrading").show();
    //Trigger click as default to select the first item of Major
    $('.radioMajor tr td input[type=radio]:visible:first').trigger("click");
    $("#allOrNothingGrading").hide();
  }
  else if (modeMultiPartGradingTemp == 'all-or-nothing-grading') {
    $("#tablePartialGrading").hide();
    $("#allOrNothingGrading").show();
  } else {
    $("#tablePartialGrading").hide();
    $("#allOrNothingGrading").hide();
  }
}

function handleChangeMultiPartOption() {
  $('input:radio[name=multi-part-option]').change(function () {
    modeMultiPartGradingTemp = $("input[name='multi-part-option']:checked").val()
    showHideOptionMultiPart()
  });
}

function handleLoadMultiPartOption() {
  modeMultiPartGradingTemp = modeMultiPartGrading
  allOrNothingGradingScoreTemp = allOrNothingGradingScore != null ? allOrNothingGradingScore : 1
  $("#allOrNothingGradingScore").val(allOrNothingGradingScoreTemp)
  if (modeMultiPartGradingTemp == 'dependent-grading') {
    $("#dependent-grading").prop("checked", true);
  }
  else if (modeMultiPartGradingTemp == 'all-or-nothing-grading') {
    $("#all-or-nothing-grading").prop("checked", true);
  } else {
    $("#partial-grading").prop("checked", true);
  }
  showHideOptionMultiPart()
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
function checkApproveAllOrNothingGrading(results) {
  var disableAllOrNothingGrading = results.some(function (item) {
    var textType = item.type
    var responseDeclaration = item.responseDeclaration
    var isManualFillInBlankQuestion = textType == 'textEntryInteraction' && responseDeclaration.method == 'manual'
    return (isManualFillInBlankQuestion || textType == 'extendedTextInteraction' || textType == 'extendedTextInteractionDraw');
  });
  var inputAllOrNothingGrading = $("#all-or-nothing-grading")
  if (disableAllOrNothingGrading) {
    inputAllOrNothingGrading.prop("disabled", true);
    inputAllOrNothingGrading.parent().addClass('disabled');
  } else {
    inputAllOrNothingGrading.prop("disabled", false);
    inputAllOrNothingGrading.parent().removeClass('disabled');
  }
}

