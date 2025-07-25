var iResult = [],
  iResultComponent = {},
  newResult,
  isMultiSelection = false,
  alphabet = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'V', 'W', 'X', 'Y', 'Z', 'AA', 'AB', 'AC', 'AD', 'AE', 'AF', 'AG', 'AH', 'AI', 'AJ', 'AK', 'AL', 'AM', 'AN', 'AO', 'AP', 'AQ', 'AR', 'AS', 'AT', 'AU', 'AV', 'AW', 'AX', 'AY', 'AZ', 'BA', 'BB', 'BC', 'BD', 'BE', 'BF', 'BG', 'BH', 'BI', 'BJ', 'BK', 'BL', 'BM', 'BN', 'BO', 'BP', 'BQ', 'BR', 'BS', 'BT', 'BU', 'BV', 'BW', 'BX', 'BY', 'BZ', 'CA', 'CB', 'CC', 'CD', 'CE', 'CF', 'CG', 'CH', 'CI', 'CJ', 'CK', 'CL', 'CM', 'CN', 'CO', 'CP', 'CQ', 'CR', 'CS', 'CT', 'CU', 'CV', 'CW', 'CX', 'CY', 'CZ'],
  iSchemeID = 1,
  quesType = 1, //Store question type: 1, 3, 8, 9, 10 or 21
  isAddnew = false, // Status question edit is addind new question type
  showTableAddItem = false, //Status to show add inline choice and fill in the blank when user add a table to editor
  imgConfig = "../", //Config for icon on MKEditor
  audioConfig = "/TestMaker/AudioUpload/", //Audio config when upload to server
  videoConfig = "/TestMaker/VideoUpload/", //Video config when upload to server
  quesImageUrl = "", //Root URL of image on question and references
  objectId = "",
  imgUpload = "",
  loadAudioUrl = "",
  MKEditor = {},
  hideBorder = 'Hide border on Test Taker and Test Printing',
  labelCkeditor = {
    inlineChoice: 'Insert Inline Choice',
    textEntry: 'Insert Fill-in-the-Blank'
  },
  GetViewReferenceImg = "", //Root URL of image on question and references
  GetViewReferenceImgS3 = "",

  ckID = "",
  errorMsg = {
    notComplex: "This multi-part item only has one part. Are you sure you want to continue?",
    inlineChoice: "You must choose one answer for Inline Choice.",
    textEntry: "You must choose one answer for Text Entry.",
    addAnswer: "You must provide a correct answer in order to save this question.",
    noCorrectAnswer: "Please select the correct combination of hot spot(s) that will allow the student to earn full credit.",
    noTextHotSpot: "Please create text hot spot!",
    addStyleTableHotSpot: "Please add style table hot spot",
    addCorrectHotSpot: "Please select at least one correct answer per table"
  },
  checkSavingComplex = false; //Check to show alert when user select iSchemeID = 21 but save with other iSchemeID
var isPassageEditor = false;

var temps = [],
  currData = [],
  allOrNothingGradingScore = null,
  modeMultiPartGrading = 'partial-grading'
  ResIdElemModul = '',
  isChangeCKeditor = false,
  isTrueFalse = false,
  isChangedctx = false,
  extensionFile = '',
  isAddAnswerLast = false,
  isAddAnswerInlineChoiceLast = false,
  isAddAnswerTextEntryLast = false; // save data major for popup majordepending
var isExtraChar = false,
  isEditChangeType = false,
  isEditMultipChoice = false,
  isAddImage = false,
  eleHotSpot = "",
  typehospot = "",
  totalCorrect = 0;
var isNewPalette = false;
var isCharNumeric = false;
var defaultSrc = window.location.protocol + "//" + window.location.hostname + '/TestAssignmentRegrader/GetViewReferenceImg?imgPath=';
var classNameStyleFontInlineChoice = '',
  isStyleFontInlineChoice = false,
  isOnClickFontSize = false,
  isApplyInlineChoice = false,
  isShowPanelFontSize = false,
  noRemoveBr = false,
  noAppendBrEditor = false,
  noRemoveFirstBr = false,
  idMessage = '',
  hasGuidance = false;
var idtypeGuidanceMessage = '',
  idtypeRationaleMessage = '',
  idtypeGuidanceRationaleMessage = '',
  idSimpleChoicesPopup = '',
  iMessageTemp = [],
  iMessageTempEdit = [],
  isEditMultipChoiceGuidance = false,
  isEditGuidancePopup = false;

var isSurveyTest = false;

// Create Functions for controls audio player
function playVNSAudio() {
  $(CKEDITOR.instances[ckID].window.getFrame().$).contents().find('.audioIcon .bntStop').hide();
  $(CKEDITOR.instances[ckID].window.getFrame().$).contents().find('.audioIcon .bntPlay').show();

  resetUIAudio();

  var me = this;
  $(me).next().show();
  $(me).hide();

  var audioUrl = $(this).parent().find(".audioRef").text();
  var newAudioUrl = "/" + audioUrl.replace(/\//g, '|').substring(1);

  if (window.playsound != undefined) {
    window.playsound.pause();
  }
  var newSrc = '';
  if (audioUrl.indexOf('http') >= 0) { //direct link from S3
    newSrc = audioUrl;
  } else {
    newSrc = loadAudioUrl + newAudioUrl;
  }
  window.playsound = new vnsAudio({
    src: newSrc,
    onEnded: function () {
      $(me).next().hide();
      $(me).show();
    }
  });
}

function stopVNSAudio() {
  $(this).prev().show();
  $(this).hide();
  if (window.playsound != undefined) {
    window.playsound.pause();
  }
}

$(document).ready(function () {
  refeshConfig();

  $(".delReference").click(function () {
    $(this).parent().remove();
  });

  CKEDITOR.on('instanceReady', function (ev) {
    ev.editor.document.on('dragstart', function (ev) {
      ev.data.preventDefault(true);
    });

    //Show add reference and audio after ckeditor loaded
    if ($("#questionType").css("display") == "none") $("#questionType").show();
    window.IS_V2 && $('#topSpace .cke_toolbox').append($('#questionType'));

    //Remove all textEntry and inlineChoice when copy
    ev.editor.on('paste', function (ev) {
      var $content = $("<p>" + ev.data.dataValue + "</p>"),
        content = ev.data.dataValue;
      $content.find(".inlineChoiceInteraction").each(function () {
        content = content.replace($(this).prop("outerHTML"), "");
      });

      $content.find(".textEntryInteraction").each(function () {
        content = content.replace($(this).prop("outerHTML"), "");
      });

      $content.find(".drawTool").each(function () {
        content = content.replace($(this).prop("outerHTML"), "");
      });

      ev.data.dataValue = content;
    });
    //Reload the CK Form when Change to new question type
    if (ev.editor.name == ckID) {
      if (isAddnew) {
        var $changeItemType = $('#changeItemType');
        var txtTrueFalse;
        //Show the main ckeditor as default
        $("#topSpace > div:first-child, #bottomSpace > div:first-child").hide();
        $("#topSpace #cke_" + ckID + ", #bottomSpace #cke_" + ckID).show();

        if (iSchemeID == "1") {
          txtTrueFalse = document.URL.split('&')[2];
          if (txtTrueFalse == 'TrueFalse' || isTrueFalse == true) {
            CKEDITOR.instances[ckID].setData('<span contenteditable="false" class="placeholder" style="position: absolute;z-index: 99999;">Enter Question...</span><br><br><div id="RESPONSE_1" texttruefalse="TrueFalse" class="multipleChoice" contenteditable="false"><button class="single-click" id="single-click">Click here to edit answer choices</button><img class="cke_reset cke_widget_mask multipleChoiceMark" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" /><span class="item answerCorrect"><span class="nonAudioIcon">&nbsp;</span><span class="answer">A.</span><span class="answerContent">True</span></span><span class="item"><span class="nonAudioIcon">&nbsp;</span><span class="answer">B.</span><span class="answerContent">False</span></span></div><br><br>');
            iResult = [{
              type: "choiceInteraction",
              subtype: "TrueFalse",
              responseIdentifier: "RESPONSE_1",
              shuffle: "false",
              maxChoices: "1",
              responseDeclaration: {
                baseType: "identifier",
                cardinality: "single",
                method: "default",
                caseSensitive: "false",
                type: "string",
                pointsValue: "1"
              },
              correctResponse: "A",
              simpleChoice: [{
                arrMessageGuidance: [],
                answerCorrect: "true",
                identifier: "A",
                value: "True"
              }, {
                arrMessageGuidance: [],
                identifier: "B",
                value: "False"
              }]
            }];
          } else {
            if (isSurveyTest) {
              CKEDITOR.instances[ckID].setData('<span contenteditable="false" class="placeholder" style="position: absolute;z-index: 99999;">Enter Question...</span><br><br><div id="RESPONSE_1" class="multipleChoice" contenteditable="false"><button class="single-click" id="single-click">Click here to edit answer choices</button><img class="cke_reset cke_widget_mask multipleChoiceMark" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" /><span class="item"><span class="nonAudioIcon">&nbsp;</span><span class="answer">A.</span><span class="answerContent">Answer 1</span></span><span class="item"><span class="nonAudioIcon">&nbsp;</span><span class="answer">B.</span><span class="answerContent">Answer 2</span></span><span class="item"><span class="nonAudioIcon">&nbsp;</span><span class="answer">C.</span><span class="answerContent">Answer 3</span></span><span class="item"><span class="nonAudioIcon">&nbsp;</span><span class="answer">D.</span><span class="answerContent">Answer 4</span></span></div><br><br>');
              iResult = [{
                type: "choiceInteraction",
                responseIdentifier: "RESPONSE_1",
                shuffle: "false",
                maxChoices: "1",
                display: "vertical",
                responseDeclaration: {
                  baseType: "identifier",
                  cardinality: "single",
                  method: "informational-only",
                  caseSensitive: "false",
                  type: "string",
                  pointsValue: "0"
                },
                correctResponse: "",
                simpleChoice: [{
                  arrMessageGuidance: [],
                  identifier: "A",
                  value: "Answer 1"
                }, {
                  arrMessageGuidance: [],
                  identifier: "B",
                  value: "Answer 2"
                }, {
                  arrMessageGuidance: [],
                  identifier: "C",
                  value: "Answer 3"
                }, {
                  arrMessageGuidance: [],
                  identifier: "D",
                  value: "Answer 4"
                }]
              }];
            } else {
              CKEDITOR.instances[ckID].setData('<span contenteditable="false" class="placeholder" style="position: absolute;z-index: 99999;">Enter Question...</span><br><br><div id="RESPONSE_1" class="multipleChoice" contenteditable="false"><button class="single-click" id="single-click">Click here to edit answer choices</button><img class="cke_reset cke_widget_mask multipleChoiceMark" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" /><span class="item answerCorrect"><span class="nonAudioIcon">&nbsp;</span><span class="answer">A.</span><span class="answerContent">Answer 1</span></span><span class="item"><span class="nonAudioIcon">&nbsp;</span><span class="answer">B.</span><span class="answerContent">Answer 2</span></span><span class="item"><span class="nonAudioIcon">&nbsp;</span><span class="answer">C.</span><span class="answerContent">Answer 3</span></span><span class="item"><span class="nonAudioIcon">&nbsp;</span><span class="answer">D.</span><span class="answerContent">Answer 4</span></span></div><br><br>');
              iResult = [{
                type: "choiceInteraction",
                responseIdentifier: "RESPONSE_1",
                shuffle: "false",
                maxChoices: "1",
                display: "vertical",
                responseDeclaration: {
                  baseType: "identifier",
                  cardinality: "single",
                  method: "default",
                  caseSensitive: "false",
                  type: "string",
                  pointsValue: "1"
                },
                correctResponse: "A",
                simpleChoice: [{
                  arrMessageGuidance: [],
                  answerCorrect: "true",
                  identifier: "A",
                  value: "Answer 1"
                }, {
                  arrMessageGuidance: [],
                  identifier: "B",
                  value: "Answer 2"
                }, {
                  arrMessageGuidance: [],
                  identifier: "C",
                  value: "Answer 3"
                }, {
                  arrMessageGuidance: [],
                  identifier: "D",
                  value: "Answer 4"
                }]
              }];
            }
          }
        } else if (iSchemeID == "3") {
          CKEDITOR.instances[ckID].setData('<span contenteditable="false" class="placeholder" style="position: absolute;z-index: 99999;">Enter Question...</span><p class="no-hover"> </p><span id="RESPONSE_1" class="multipleChoice" contenteditable="false"><button class="single-click" id="single-click">Click here to edit answer choices</button><img class="cke_reset cke_widget_mask multipleChoiceMark" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" /><span class="item answerCorrect"><span class="nonAudioIcon">&nbsp;</span><span class="answer">A.</span><span class="answerContent">Answer 1</span></span><span class="item"><span class="nonAudioIcon">&nbsp;</span><span class="answer">B.</span><span class="answerContent">Answer 2</span></span><span class="item answerCorrect"><span class="nonAudioIcon">&nbsp;</span><span class="answer">C.</span><span class="answerContent">Answer 3</span></span><span class="item"><span class="nonAudioIcon">&nbsp;</span><span class="answer">D.</span><span class="answerContent">Answer 4</span></span></span><p class="no-hover"> </p>');
          iResult = [{
            type: "choiceInteraction",
            responseIdentifier: "RESPONSE_1",
            shuffle: "false",
            maxChoices: "0",
            responseDeclaration: {
              baseType: "identifier",
              cardinality: "multiple",
              method: "default",
              caseSensitive: "false",
              type: "string",
              pointsValue: "1"
            },
            correctResponse: "A,C",
            simpleChoice: [{
              answerCorrect: "true",
              identifier: "A",
              value: "Answer 1"
            }, {
              identifier: "B",
              value: "Answer 2"
            }, {
              answerCorrect: "true",
              identifier: "C",
              value: "Answer 3"
            }, {
              identifier: "D",
              value: "Answer 4"
            }]
          }];
        } else if (iSchemeID == "8") {
          CKEDITOR.instances[ckID].setData('<span contenteditable="false" class="placeholder" style="position: absolute;z-index: 99999;">Enter Question...</span><br/>&nbsp;<span id="RESPONSE_1" style="max-width: 250px;" class="inlineChoiceInteraction" contenteditable="false"><img style="display: none;padding-left: 3px; padding-top: 0.5px;" alt="Guidance" class="imageupload bntGuidance" src="../../Content/themes/TestMaker/images/guidance_checked_small.png" title="Guidance"><img class="cke_reset cke_widget_mask inlineChoiceInteractionMark" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" /></span><span>&nbsp;</span><br><br>');
          iResult = [{
            correctResponse: "A",
            inlineChoice: [{
              arrMessageGuidance: [],
              identifier: "A",
              value: "Answer 1",
              pointsValue: "0",
              answerCorrect: "true"
            }, {
              arrMessageGuidance: [],
              identifier: "B",
              value: "Answer 2",
              pointsValue: "0"
            }],
            responseDeclaration: {
              baseType: "identifier",
              cardinality: "single",
              caseSensitive: "false",
              method: "default",
              pointsValue: "1",
              type: "string"
            },
            responseIdentifier: "RESPONSE_1",
            shuffle: false,
            expectedWidth: '200',
            visibleDimension: '0',
            type: "inlineChoiceInteraction"
          }];
        } else if (iSchemeID == "9") {
          CKEDITOR.instances[ckID].setData('<span contenteditable="false" class="placeholder" style="position: absolute;z-index: 99999;">Enter Question...</span><br/>&nbsp;<span id="RESPONSE_1" style="max-width: 150px;" class="textEntryInteraction" contenteditable="false"><img style="display: none;padding-left: 3px; padding-top: 0.5px;" alt="Guidance" class="imageupload bntGuidance" src="../../Content/themes/TestMaker/images/guidance_checked_small.png" title="Guidance"><img class="cke_reset cke_widget_mask textEntryInteractionMark" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" /></span>&nbsp;<br><br>');
          iResult = [{
            correctResponse: [{
              identifier: "A",
              value: "Answer Correct 1",
              pointsValue: "1",
              arrMessageGuidance: []
            }],
            expectedLengthMax: "8",
            expectedLengthMin: "0",
            expectedWidth: '150',
            validation: "0",
            addPadding: "1",
            responseDeclaration: {
              baseType: "string",
              cardinality: "single",
              method: "default",
              pointsValue: "1",
              spelling: false,
              spellingDeduction: "1",
              type: "string",
              caseSensitive: "false",
              ignoreExtraSpace: "true"
            },
            responseIdentifier: "RESPONSE_1",
            type: "textEntryInteraction"
          }];
        } else if (iSchemeID == "10d") {
          CKEDITOR.instances[ckID].setData('<span contenteditable="false" class="placeholder" style="position: absolute;z-index: 99999;">Enter Question...</span><br><br><div id="RESPONSE_1" style="width:400px; height: 300px" class="drawTool" contenteditable="false"><span class="text-place-holder"></span> <img style="width: 400px; height: 300px;" class="cke_reset cke_widget_mask imageDrawTool" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" /><div id="drawHoder" class="txtHoder"><img src="" width="400" height="300" alt="" id="imageDrawTool" style="position: absolute; border: none; display: none;" /><div class="divContent" style="padding-left: 5px;"></div></div></div>&nbsp;<br><br>');
          iResult = [{
            type: "extendedTextInteractionDraw",
            responseIdentifier: "RESPONSE_1",
            percent: "10",
            wOrgImage: "400",
            hOrgImage: "300",
            width: "400",
            height: "300",
            srcImage: "",
            responseDeclaration: {
              baseType: "string",
              cardinality: "single",
              method: "default",
              caseSensitive: "false",
              pointsValue: "1",
              type: "string"
            },
            drawable: true,
            dataType: 'free-formatted',
            dataWidth: 600,
            dataHeight: 600
          }];
        } else if (iSchemeID == "10") {
          CKEDITOR.instances[ckID].setData('<span contenteditable="false" class="placeholder" style="position: absolute;z-index: 99999;">Enter Question...</span><br><br><label id="RESPONSE_1" class="extendText" contenteditable="false"> <span class="text-place-holder">Placeholder to display text area.</span> <img class="cke_reset cke_widget_mask extentTextInteractionMark" data-cke-saved-src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D">&nbsp;</label><br><br>');
          iResult = [{
            type: "extendedTextInteraction",
            responseIdentifier: "RESPONSE_1",
            expectedLength: "50000",
            formatText: true,
            placeholderText: '',
            responseDeclaration: {
              baseType: "string",
              cardinality: "single",
              method: isSurveyTest ? "ungraded" : "default",
              caseSensitive: "false",
              type: "string",
              pointsValue: "1"
            }
          }];
        } else if (iSchemeID == "21") {
          CKEDITOR.instances[ckID].setData("<span contenteditable='false' class='placeholder' style='position: absolute;z-index: 99999;'>Enter Question...</span>");
          showTableAddItem = true;
          TestMakerComponent.isShowConditionalLogicButton = true;
        } else if (iSchemeID == "30") {
          CKEDITOR.instances[ckID].setData("<span contenteditable='false' class='placeholder' style='position: absolute;z-index: 99999;'>Enter Question...</span>");
          iResult = [{
            type: "partialCredit",
            responseIdentifier: "RESPONSE_1",
            partialID: "Partial_1",
            responseDeclaration: {
              absoluteGrading: "0",
              absoluteGradingPoints: "1",
              partialGradingThreshold: "1",
              relativeGrading: "0",
              relativeGradingPoints: "1",
              thresholdGrading: '0',
              algorithmicGrading: '0',
              pointsValue: "1",
              lineMatching: "0",
              anchorPositionObject: "left",
              anchorPositionDestination: "right"
            },
            source: [], //{ srcIdentifier: "SRC_1", type: "text", value: "Text Label" }
            destination: [],
            correctResponse: []
          }];
        } else if (iSchemeID == "31") {
          // Set default multiple choice
          iSchemeID = 31;
          iResult = [{
            type: 'textHotSpot',
            responseIdentifier: 'RESPONSE_1',
            maxSelected: '1',
            correctResponse: [],
            responseDeclaration: {
              absoluteGrading: '1',
              partialGrading: '0',
              algorithmicGrading: '0',
              pointsValue: '1'
            },
            source: []
          }];
        } else if (iSchemeID == "32") {
          CKEDITOR.instances[ckID].setData('<span contenteditable="false" class="placeholder" style="position: absolute;z-index: 99999;">Enter Question…</span><p class="no-hover"> </p>&nbsp;<div id="RESPONSE_1" class="imageHotspotInteraction" style="width:400px; height: 300px" contenteditable="false"><span class="imageHotspotTitle">Image Hot Spot Selection</span><img style="width: 400px; height: 300px;" class="cke_reset cke_widget_mask imageHotspotMark" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" /></div>&nbsp<p class="no-hover"> </p>');
          iResult = [{
            type: "imageHotSpot",
            responseIdentifier: "RESPONSE_1",
            partialID: "Partial_1",
            responseDeclaration: {
              absoluteGrading: "1",
              partialGrading: "0",
              algorithmicGrading: '0',
              pointsValue: "1"
            },
            source: {},
            sourceItem: [],
            correctResponse: []
          }];
        } else if (iSchemeID == "33") {
          iSchemeID = 33;
          CKEDITOR.instances[ckID].setData("<span contenteditable='false' class='placeholder' style='position: absolute;z-index: 99999;'>Enter Question…</span>");
          var objTableHotSpot = {
            type: "tableHotSpot",
            responseIdentifier: "RESPONSE_1",
            maxSelected: "1",
            correctResponse: [],
            responseDeclaration: {
              absoluteGrading: "1",
              partialGrading: "0",
              algorithmicGrading: '0',
              pointsValue: "1"
            },
            sourceHotSpot: {
              idtableHotspot: 'RESPONSE_1',
              arrayList: []
            }
          }
          iResult = [objTableHotSpot];
        } else if (iSchemeID == '34') {
          CKEDITOR.instances[ckID].setData('<span contenteditable="false" class="placeholder" style="position: absolute;z-index: 99999;">Enter Question…</span><p class="no-hover"> </p>&nbsp;<div id="RESPONSE_1" class="numberline-selection" style="width:300px; height: 100px" contenteditable="false"><img class="cke_reset cke_widget_mask numberLineInteraction" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" /><span class="numberline-selection-title">Number Line Hot Spot</span></div>&nbsp;<p class="no-hover"> </p>');
          iResult = [{
            type: 'numberLineHotSpot',
            responseIdentifier: 'RESPONSE_1',
            partialID: 'Partial_1',
            responseDeclaration: {
              absoluteGrading: '1',
              partialGrading: '0',
              algorithmicGrading: '0',
              pointsValue: '1'
            },
            source: {},
            sourceItem: [],
            correctResponse: []
          }];
        } else if (iSchemeID == '35') {
          CKEDITOR.instances[ckID].setData('<span contenteditable="false" class="placeholder" style="position: absolute;z-index: 99999;">Enter Question...</span><p class="no-hover"> </p>&nbsp;');
          iResult = [{
            type: "dragDropNumerical",
            responseIdentifier: "RESPONSE_1",
            partialID: "Partial_1",
            source: [],
            destination: [],
            correctResponse: {},
            responseDeclaration: {
              absoluteGrading: '1',
              algorithmicGrading: '0'
            }
          }];
        } else if (iSchemeID == '36') {
          CKEDITOR.instances[ckID].setData("<span contenteditable='false' class='placeholder' style='position: absolute;z-index: 99999;'>Enter Question...</span><br><br>");
        } else if (iSchemeID == "37") {
          CKEDITOR.instances[ckID].setData('<span contenteditable="false" class="placeholder" style="position: absolute;z-index: 99999;">Enter Question...</span><br><br><span id="RESPONSE_1" class="multipleChoice multipleChoiceVariable" contenteditable="false"><button class="single-click-variable">Click here to edit answer choices</button><img class="cke_reset cke_widget_mask multipleChoiceVariableMark" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" /><span class="item"><span class="nonAudioIcon">&nbsp;</span><span class="answer">A.</span><span class="answerContent">Answer 1</span></span><span class="item"><span class="nonAudioIcon">&nbsp;</span><span class="answer">B.</span><span class="answerContent">Answer 2</span></span><span class="item"><span class="nonAudioIcon">&nbsp;</span><span class="answer">C.</span><span class="answerContent">Answer 3</span></span><span class="item"><span class="nonAudioIcon">&nbsp;</span><span class="answer">D.</span><span class="answerContent">Answer 4</span></span></span><br />');
          iResult = [{
            type: "choiceInteractionVariable",
            responseIdentifier: "RESPONSE_1",
            shuffle: "false",
            maxChoices: "1",
            display: "vertical",
            responseDeclaration: {
              baseType: "identifier",
              cardinality: "single",
              method: "default",
              caseSensitive: "false",
              type: "string",
              pointsValue: "0"
            },
            simpleChoice: [{
              answerPoint: "0",
              identifier: "A",
              value: "Answer 1"
            },
            {
              answerPoint: "0",
              identifier: "B",
              value: "Answer 2"
            },
            {
              answerPoint: "0",
              identifier: "C",
              value: "Answer 3"
            },
            {
              answerPoint: "0",
              identifier: "D",
              value: "Answer 4"
            }
            ]
          }];
        } else {
          // Set default multiple choice
          iSchemeID = 1;
          CKEDITOR.instances[ckID].setData('<span contenteditable="false" class="placeholder" style="position: absolute;z-index: 99999;">Enter Question…</span><p class="no-hover"> </p><span id="RESPONSE_1" class="multipleChoice" contenteditable="false"><button class="single-click" id="single-click">Click here to edit answer choices</button><img class="cke_reset cke_widget_mask multipleChoiceMark" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" /><span class="item answerCorrect"><span class="nonAudioIcon">&nbsp;</span><span class="answer">A.</span><span class="answerContent">Answer 1</span></span><span class="item"><span class="nonAudioIcon">&nbsp;</span><span class="answer">B.</span><span class="answerContent">Answer 2</span></span><span class="item"><span class="nonAudioIcon">&nbsp;</span><span class="answer">C.</span><span class="answerContent">Answer 3</span></span><span class="item"><span class="nonAudioIcon">&nbsp;</span><span class="answer">D.</span><span class="answerContent">Answer 4</span></span></span><p class="no-hover"> </p>');
          iResult = [{
            type: "choiceInteraction",
            responseIdentifier: "RESPONSE_1",
            shuffle: "false",
            maxChoices: "1",
            responseDeclaration: {
              baseType: "identifier",
              cardinality: "single",
              method: "default",
              caseSensitive: "false",
              type: "string",
              pointsValue: "1"
            },
            correctResponse: "A",
            simpleChoice: [{
              answerCorrect: "true",
              identifier: "A",
              value: "Answer 1"
            },
            {
              identifier: "B",
              value: "Answer 2"
            },
            {
              identifier: "C",
              value: "Answer 3"
            },
            {
              identifier: "D",
              value: "Answer 4"
            }
            ]
          }];
        }
        if (txtTrueFalse == 'TrueFalse' || isTrueFalse == true) {
          $changeItemType.find('option[value="' + iSchemeID + '"][titlespecial="TrueFalse"]').prop('selected', true);
        } else {
          if (iSchemeID == '1') {
            $changeItemType.find('option[value="' + iSchemeID + '"][titlespecial="MultipleChoice"]').prop('selected', true);
          } else {
            $changeItemType.find('option[value="' + iSchemeID + '"]').prop('selected', true);
          }
        }

        isAddnew = false; // Return status after ckeditor loaded

        //Set hide all menu as default.
        if (iSchemeID == "1") {
          $(".cke_button__multiplechoice").parents("span.cke_toolbar").hide(); //Hide after add item
          $(".cke_button__multiplechoice_label").text("Multiple Choice").attr("title", "Multiple Choice");
        } else if (iSchemeID == "3") {
          $(".cke_button__multiplechoice").parents("span.cke_toolbar").hide();
        } else if (iSchemeID == "8") {
          $(".cke_button__inlinechoice").parents("span.cke_toolbar").hide();
        } else if (iSchemeID == "9") {
          $(".cke_button__textentry").parents("span.cke_toolbar").hide();
        } else if (iSchemeID == "10d") {
          $(".cke_button__drawtool").parents("span.cke_toolbar").hide();
        } else if (iSchemeID == "10") {
          $(".cke_button__extendtext").parents("span.cke_toolbar").hide();
        } else if (iSchemeID == "21") {
          $(".cke_button__multiplechoice_label").text("Multiple Choice").attr("title", "Multiple Choice");
        } else if (iSchemeID == "32") {
          $(".cke_button__imagehotspotselection").parents("span.cke_toolbar").hide();
        } else if (iSchemeID == '34') {
          $('.cke_button__numberlinehotspot').parents('span.cke_toolbar').hide();
        } else if (iSchemeID == '37') {
          $('.cke_button__multiplechoicevariable').parents('span.cke_toolbar').hide();
        }
      } else {
        if (iSchemeID == "1" || iSchemeID == "21") {
          $(".cke_button__multiplechoice_label").text("Multiple Choice").attr("title", "Multiple Choice");
        }

        //This make sure this is not passage editor
        if (ev.editor.name != "passageContent") {
          showTableAddItem = true;
        }

        //This case for edit item
        $(".cke_button__multiplechoice").parents("span.cke_toolbar").hide();
        $(".cke_button__inlinechoice").parents("span.cke_toolbar").hide();
        $(".cke_button__textentry").parents("span.cke_toolbar").hide();
        $(".cke_button__drawtool").parents("span.cke_toolbar").hide();
        $(".cke_button__extendtext").parents("span.cke_toolbar").hide();
        $(".cke_combo__partialaddsource").parents("span.cke_toolbar").hide();
        $(".cke_button__dependentgrading").parents("span.cke_toolbar").hide();
        $(".cke_button__sequenceorder").parents("span.cke_toolbar").hide();
        $('.cke_button__multiplechoicevariable').parents('span.cke_toolbar').hide();

        if (iSchemeID == "21") {
          $(".cke_button__addquestiontype").parents("span.cke_toolbar").hide();
          //This case for edit item
          $(".cke_button__multiplechoice").parents("span.cke_toolbar").show();
          $(".cke_button__inlinechoice").parents("span.cke_toolbar").show();
          $(".cke_button__textentry").parents("span.cke_toolbar").show();
          $(".cke_button__drawtool").parents("span.cke_toolbar").show();
          $(".cke_button__extendtext").parents("span.cke_toolbar").show();
          $(".cke_combo__partialaddsource").parents("span.cke_toolbar").hide();
          $(".cke_button__dependentgrading").parents("span.cke_toolbar").show();
        } else if (iSchemeID == "30") {
          $(".cke_button__addquestiontype").parents("span.cke_toolbar").hide();
          $(".cke_combo__partialaddsource").parents("span.cke_toolbar").show();
        } else if (iSchemeID == "32") {
          $(".cke_button__imagehotspotselection").parents("span.cke_toolbar").hide();
        } else if (iSchemeID == '34') {
          $('.cke_button__numberlinehotspot').parents('span.cke_toolbar').hide();
        } else if (iSchemeID == '35') {
          $(".cke_combo__partialaddsource").parents("span.cke_toolbar").show();
        }
      }

      myEditor = ev.editor;

      ev.editor.document.on('keyup', function (evt) {
        if (iSchemeID == "1") {
          //Single choice
          if (myEditor.getData().indexOf('class="multipleChoice"') == -1) {
            $(".cke_button__multiplechoice").parents("span.cke_toolbar").show();
          } else {
            $(".cke_button__multiplechoice").parents("span.cke_toolbar").hide();
          }
        } else if (iSchemeID == "3") {
          //Multiple choice
          if (myEditor.getData().indexOf('class="multipleChoice"') == -1) {
            $(".cke_button__multiplechoice").parents("span.cke_toolbar").show();
          } else {
            $(".cke_button__multiplechoice").parents("span.cke_toolbar").hide();
          }
        } else if (iSchemeID == "8") {
          //inline choice
          if (myEditor.getData().indexOf('class="inlineChoiceInteraction"') == -1) {
            $(".cke_button__inlinechoice").parents("span.cke_toolbar").show();
          } else {
            $(".cke_button__inlinechoice").parents("span.cke_toolbar").hide();
          }
        } else if (iSchemeID == "9") {
          //text entry
          if (myEditor.getData().indexOf('class="textEntryInteraction"') == -1) {
            $(".cke_button__textentry").parents("span.cke_toolbar").show();
          } else {
            $(".cke_button__textentry").parents("span.cke_toolbar").hide();
          }
        } else if (iSchemeID == "10") {
          //Extended text
          if (myEditor.getData().indexOf('class="extendText"') == -1) {
            $(".cke_button__extendtext").parents("span.cke_toolbar").show();
          } else {
            $(".cke_button__extendtext").parents("span.cke_toolbar").hide();
          }
        } else if (iSchemeID == "10d") {
          //Draw tool
          if (myEditor.getData().indexOf('class="drawTool"') == -1) {
            $(".cke_button__drawtool").parents("span.cke_toolbar").show();
          } else {
            $(".cke_button__drawtool").parents("span.cke_toolbar").hide();
          }
        } else if (iSchemeID == "32") {
          // Image Hot Spot
          if (myEditor.getData().indexOf('class="imageHotspotMark"') == -1) {
            $(".cke_button__imagehotspotselection").parents("span.cke_toolbar").show();
          } else {
            $(".cke_button__imagehotspotselection").parents("span.cke_toolbar").hide();
          }
        } else if (iSchemeID == '34') {
          // Number Line Hot Spot'
          if (myEditor.getData().indexOf('class="numberLineInteraction"') == -1) {
            $('.cke_button__numberlinehotspot').parents('span.cke_toolbar').show();
          } else {
            $('.cke_button__numberlinehotspot').parents('span.cke_toolbar').hide();
          }
        } else if (iSchemeID == '37') {
          // Multiple Choice with Variable Points Per Answer
          if (myEditor.getData().indexOf('class="multipleChoice multipleChoiceVariable"') == -1) {
            $('.cke_button__multiplechoicevariable').parents('span.cke_toolbar').show();
          } else {
            $('.cke_button__multiplechoicevariable').parents('span.cke_toolbar').hide();
          }
        }

        //This is a trick add a &#160(a space) next to mathML when user press enter
        var key = evt.data.getKey();
        // Remove the filling char before some keys get
        // executed, so they'll not get blocked by it.
        switch (key) {
          case 13: // ENTER
            setTimeout(function () {
              myEditor.insertHtml("\u200b");
            }, 300);
        }
      });

      ev.editor.on('contentDom', function () {
        var editable = ev.editor.editable();
        var top = $('#divQContent').find('iframe[allowtransparency]').offset().top - 20;
        //handle control z
        editable.attachListener(editable, 'keydown', function (event) {
          //newResult = iResult;
          var kcode = event.data.getKey();
          if (event.data.$.ctrlKey && kcode == 90) { //z
            iResult = newResult;

            event.cancel();
            checkSavingComplex = false;
          }
        });

        editable.attachListener(editable, 'mousemove', function (evt) {
          var ele = evt.data.getTarget();

          var left = $('#divQContent').find('iframe[allowtransparency]').offset().left - 70;
          var topM = evt.data.$.clientY;
          var leftM = evt.data.$.clientX;
          var itemChoice = '';
          if (ele.getAttribute('class') != undefined) {
            if (ele.getAttribute('class').split(" ")[2] != undefined) {
              itemChoice = ele.getAttribute('class').split(" ")[2];
            }
          }

          var topEle = ele.getDocumentPosition().y;
          var leftEle = ele.getDocumentPosition().x;
          var $tooltips = $('.tool-tip-tips');
          var arrowHtml = '<span class="arrow"><span></span></span>';

          if (!ele.hasClass('placeholder')) {
            var newSubPart = [
              'multipleChoiceMark',
              'inlineChoiceInteractionMark',
              'extentTextInteractionMark',
              'textEntryInteractionMark',
              'extendText',
              'imageDrawTool',
              'partialAddSourceTextMark',
              'partialAddSourceImage',
              'partialAddDestinationTextMark',
              'partialAddDestinationImageMark',
              'imageupload',
              'imageHotspotMark',
              'imageHotspotMarkObject',
              'numberLineInteraction',
              'sequenceMark',
              'partialDragDropNumericalSourceMark',
              'partialDragDropNumericalDestinationMark',
              'itemtypeonimageMark'
            ];

            for (var si = 0, lenSubPart = newSubPart.length; si < lenSubPart; si++) {
              if (ele.hasClass(newSubPart[si])) {
                $tooltips.css({
                  'top': (top + topM) + 'px',
                  'left': (left + leftM) + 'px',
                  'opacity': 1,
                  'display': 'block'
                });
              }
            }

            if (ele.hasClass('hotspot-checkbox') || ele.hasClass('hotspot-circle')) {
              $tooltips.css({
                'top': (top + topM) + 'px',
                'left': (left + leftM - 65) + 'px',
                'opacity': 1,
                'display': 'block'
              });
            }

            if (ele.hasClass('imageupload') || ele.hasClass('itemtypeonimageMark')) {
              $tooltips.html('Double click here to edit image' + arrowHtml);
            } else if (ele.hasClass('textEntryInteractionMark')) {
              $tooltips.html('Double click here to edit text entry' + arrowHtml);
            } else if (ele.hasClass('extendText') || ele.hasClass('extentTextInteractionMark')) {
              $tooltips.html('Double click here to edit open ended box' + arrowHtml);
            } else if (ele.hasClass('imageDrawTool')) {
              $tooltips.html('Double click here to edit drawing interaction' + arrowHtml);
            } else if (ele.hasClass('partialAddSourceTextMark') || ele.hasClass('partialAddSourceImage')) {
              $tooltips.html('Double click here to edit source object' + arrowHtml);
            } else if (ele.hasClass('partialAddDestinationTextMark') || ele.hasClass('partialAddDestinationImageMark')) {
              $tooltips.html('Double click here to edit destination object' + arrowHtml);
            } else if (ele.hasClass('imageHotspotMark') || ele.hasClass('imageHotspotMarkObject')) {
              $tooltips.html('Double click here to edit image hot spot selection' + arrowHtml);
            } else if (ele.hasClass('hotspot-checkbox') || ele.hasClass('hotspot-circle')) {
              $tooltips.html('Double click to edit the hot spot properties' + arrowHtml);
            } else if (ele.hasClass('numberLineInteraction')) {
              $tooltips.html('Double click here to edit number line hot spot' + arrowHtml);
            } else if (ele.hasClass('partialDragDropNumericalSourceMark')) {
              $tooltips.html('Double click here to edit source drag and drop numerical' + arrowHtml);
            } else if (ele.hasClass('sequenceMark')) {
              $tooltips.html('Double click here to edit sequence order' + arrowHtml);
            } else if (ele.hasClass('multipleChoiceVariableMark')) {
              $tooltips.html('Double click here to edit answer choices with variable point' + arrowHtml);
            } else if (ele.hasClass('partialDragDropNumericalDestinationMark')) {
              $tooltips.html('Double click here to edit destination drag and drop numerical' + arrowHtml);
            } else {
              $tooltips.html('Double click here to edit answer choices' + arrowHtml);
            }
          }
        });

        editable.attachListener(editable, 'mouseout', function (evt) {
          $('.tool-tip-tips').css({
            'top': '0px',
            'opacity': 0,
            'display': 'none'
          });
        });

        editable.attachListener(editable, 'keyup', function (evt) {
          if (iSchemeID == '36') {
            // Drap and drop sequence
            if (myEditor.getData().indexOf('class="sequenceBlock"') == -1) {
              $('.cke_button__sequenceorder').parents('span.cke_toolbar').show();
            } else {
              $('.cke_button__sequenceorder').parents('span.cke_toolbar').hide();
            }
          }
        });

        editable.attachListener(editable, 'click', function (evt) {
          var element = ev.editor.getSelection().getSelectedElement();

          if (evt.name == "click" && element != null) {
            if (evt.data.$.toElement && evt.data.$.toElement.className == "audioTable bntPlay") {
              var audioUrl = evt.data.$.toElement.getAttribute('audiosrc');
              if (window.playsound != undefined) {
                if (window.playsound.isPaused() == true) {
                  window.playsound = new vnsAudio({
                    src: audioUrl
                  });
                } else {
                  window.playsound.pause();
                }
              } else {
                window.playsound = new vnsAudio({
                  src: audioUrl
                });
              }
              ev.editor.getSelection().removeAllRanges()
            }
          }
          tristateStatusHandler(element, ev.editor);
        });
      });

      //hover audio
      $('#questionType').find('.audioRemove').each(function () {
        $(this).on('mouseover', function () {
          var top = $(this).offset().top;
          var left = $(this).offset().left;
          $('.tool-tip-tips').css({
            'top': (top - 20) + 'px',
            'left': (left - 50) + 'px',
            'opacity': 1,
            'display': 'block'
          }).html('click here to play audio' + '<span class="arrow"><span></span></span>');
        });
        $(this).on('mouseout', function () {
          var top = $(this).offset().top;
          var left = $(this).offset().left;
          $('.tool-tip-tips').css({
            'top': (top - 20) + 'px',
            'left': (left - 50) + 'px',
            'opacity': 1,
            'display': 'none'
          });
        });
      });

      //border item deleted
      if (iSchemeID == "21") {
        ev.editor.on('contentDom', function () {
          var editable = ev.editor.editable();

          editable.attachListener(editable, 'click', function (evt) {
            var element = ev.editor.getSelection().getSelectedElement();

            var newSubPart = ['multipleChoice', 'inlineChoiceInteraction', 'textEntryInteraction', 'extendText', 'drawTool'];

            if (element !== null) {
              var elementParent = element.getParent();
              for (var i = 0, len = newSubPart.length; i < len; i++) {
                if (elementParent.hasClass(newSubPart[i])) {
                  if (elementParent.hasClass('active-border')) {
                    elementParent.removeClass('active-border');
                  } else {
                    elementParent.addClass('active-border');
                  }

                  if (elementParent.hasClass('inlineChoiceInteraction')) {
                    elementParent.addClass('typeFontSize');
                  }
                }
              }
            }
          });
          //add Class typefontsize for inline choice
          if (editable.hasFocus) {
            var bodyEditor = ev.editor.editable();
            var tagBody = $(bodyEditor.$);
            $(tagBody).click(function (evt) {
              if (!$(evt.target).hasClass('inlineChoiceInteractionMark')) {
                if ($(tagBody).find('span.inlineChoiceInteraction').hasClass('typeFontSize')) {
                  $(tagBody).find('span.inlineChoiceInteraction').removeClass('typeFontSize');
                }

                if (navigator.userAgent.indexOf('Trident') > -1) {
                  if ($(tagBody).find('span.inlineChoiceInteraction').hasClass('active-border')) {
                    $(tagBody).find('span.inlineChoiceInteraction').removeClass('active-border');
                  }

                  if (!$(evt.target).hasClass('smallText') || !$(evt.target).hasClass('normalText') || !$(evt.target).hasClass('largeText') || !$(evt.target).hasClass('veryLargeText')) {
                    $('.editorArea .cke_combo__fontsize').find('a .cke_combo_text').text('Normal');
                    $('.editorArea .cke_combo__fontsize').find('.cke_combo_label').text('Normal');
                  }
                }
              } else {
                if (navigator.userAgent.indexOf('Trident') > -1) {
                  $(tagBody).find('span.inlineChoiceInteraction').removeClass('active-border');
                }
                $(tagBody).find('span.inlineChoiceInteraction').removeClass('typeFontSize');
                $(evt.target).parent('span.inlineChoiceInteraction').addClass('typeFontSize');

                if (navigator.userAgent.indexOf('Trident') > -1) {
                  if ($(evt.target).hasClass('inlineChoiceInteractionMark')) {
                    var tagInlineChoice = $(evt.target).parent();
                    var tagFontSize = tagInlineChoice.parent();
                    $(evt.target).parent('span.inlineChoiceInteraction').addClass('active-border');

                    if ($(tagFontSize).hasClass('smallText') || $(tagFontSize).hasClass('normalText') || $(tagFontSize).hasClass('largeText') || $(tagFontSize).hasClass('veryLargeText')) {
                      var eleFontSize = getStyleFontSizeInlineChoice(tagFontSize);
                      $('.editorArea .cke_combo__fontsize').find('a .cke_combo_text').text(eleFontSize);
                      $('.editorArea .cke_combo__fontsize').find('.cke_combo_label').text(eleFontSize);
                    }
                  }
                }
              }
            });
          }
        });
      }

      //watermark for ckeditor
      ev.editor.on('focus', function (ev) {
        var span = myEditor.getData();
        span = replaceVideo(span);
        span = '<div>' + span + '</div>';
        var $span = $(span);
        if (iSchemeID == "21" || iSchemeID == "30" || iSchemeID == "33" || iSchemeID == "36") {
          if ($span.find('.placeholder').length > 0) {
            myEditor.setData("");
            $('iframe[allowtransparency]').contents().find('body').html('');
          }
        }
        if (iSchemeID == "1" || iSchemeID == "3" ||
          iSchemeID == "8" || iSchemeID == "9" ||
          iSchemeID == "10d" || iSchemeID == "10" ||
          iSchemeID == "32" || iSchemeID == "34" || iSchemeID == '35' || iSchemeID == "37") {
          if ($span.find('.placeholder').length > 0) {
            $('iframe[allowtransparency]').contents().find('.placeholder').css({
              'position': 'absolute'
            }).hide();
          }
        }
        if (CKEDITOR.instances[ckID].checkDirty()) {
          isChangedctx = true;
          isChangeCKeditor = true;
        }
      });

      ev.editor.on('blur', function (ev) {
        if (CKEDITOR.instances[ckID].checkDirty()) {
          isChangedctx = true;
          isChangeCKeditor = true;
        }
        var span = myEditor.getData().trim();

        span = replaceVideo(span);
        if (iSchemeID == "21" || iSchemeID == "30" || iSchemeID == "33" || iSchemeID == "36") {
          if (span == '') {
            myEditor.setData("<span class='placeholder' style='position: absolute;'>Enter Question…</span>");
          } else {
            isChangeCKeditor = true;
          }
        }

        var body = $('iframe[allowtransparency]').contents().find('body').prop('outerHTML');
        body = replaceVideo(body);
        if (iSchemeID == "1" || iSchemeID == "3" || iSchemeID == "37") {
          var txtBody = $('<div/>').html(body).text();
          var newStr = txtBody.replace(/\s+/g, '');

          if (newStr == 'EnterQuestion…ClickheretoeditanswerchoicesA.Answer1B.Answer2C.Answer3D.Answer4' || newStr == 'EnterQuestion…ClickheretoeditanswerchoicesA.TrueB.False' ||
            newStr == 'EnterQuestion...ClickheretoeditanswerchoicesA.Answer1B.Answer2C.Answer3D.Answer4' || newStr == 'EnterQuestion...ClickheretoeditanswerchoicesA.TrueB.False') {
            $('iframe[allowtransparency]').contents().find('.placeholder').show();
            isChangeCKeditor = false;

            if ($('iframe[allowtransparency]').contents().find('img.imageupload').length > 0) {
              $('iframe[allowtransparency]').contents().find('.placeholder').hide();
              isChangeCKeditor = true;
            }

            if ($('iframe[allowtransparency]').contents().find('span[data-cke-display-name="math"]').length > 0) {
              $('iframe[allowtransparency]').contents().find('.placeholder').hide();
              isChangeCKeditor = true;
            }
          } else {
            isChangeCKeditor = true;
          }
        }

        if (iSchemeID == "8" || iSchemeID == "9" || iSchemeID == "10d" || iSchemeID == "10" || iSchemeID == "32" || iSchemeID == '34') {
          if ($(body).text().trim() == 'Enter Question...' || $(body).text().trim() == 'Enter Question…' || $(body).text().trim() == 'Enter Question…Image Hot Spot Selection') {
            $('iframe[allowtransparency]').contents().find('.placeholder').show();

            if ($('iframe[allowtransparency]').contents().find('img.imageupload').length > 0) {
              $('iframe[allowtransparency]').contents().find('.placeholder').hide();
              isChangeCKeditor = true;
            }
            if ($('iframe[allowtransparency]').contents().find('span[data-cke-display-name="math"]').length > 0) {
              $('iframe[allowtransparency]').contents().find('.placeholder').hide();
              isChangeCKeditor = true;
            }
          } else {
            isChangeCKeditor = true;
          }
        }
      });
    }

    IS_V2 && $('#cke_' + ev.editor.name + ' [title]:not(iframe)').tip();
    $('iframe').filter((idx, e) => e.src?.startsWith(window.location.origin)).contents().find("video").trigger('pause');
  });

  //Add audio for question
  $("#addAudioQuestion").click(function () {
    $(this).parent().find(".hiddenUpload").val("").trigger("click");
  });

  $(".js-audio .hiddenUpload").change(function () {
    var file = this.value;
    var extension = file.substr((file.lastIndexOf('.') + 1));

    if (extension != "mp3") {
      alert("File is not correct format.");
      return;
    }
    var me = this;
    var data = new FormData();
    data.append('file', $(me).get(0).files[0]);
    data.append('id', objectId);

    $.ajax({
      type: "POST",
      url: audioConfig,
      data: data,
      cache: false,
      contentType: false,
      processData: false,
      success: function (respone) {
        $("#addAudioQuestion").hide();
        $("#audioRemoveQuestion").show();
        $(me).parent().find(".audioRef").text(respone.url);
      },
      error: function (error) { }
    });
  });

  $(".js-audio .removeAudio").live("click", function () {
    $(this).parent().hide();
    $("#addAudioQuestion").show();
    stopVNSAudio();
  });

  //Handlers when controls is clicked
  $(".js-audio .bntPlay").live("click", playVNSAudio);
  $(".js-audio .bntStop").live("click", stopVNSAudio);
});

//Plugin for update and down number
;
(function ($, window, document, undefined) {
  var pluginName = "ckUpDownNumber",
    defaults = {
      height: 32,
      width: 24,
      minNumber: 0,
      maxNumber: 9000
    };

  function Plugin(element, options) {
    this.element = element;

    this.settings = $.extend({}, defaults, options);

    this._defaults = defaults;
    this._name = pluginName;
    this.init();
  }

  // Avoid Plugin.prototype conflicts
  $.extend(Plugin.prototype, {
    init: function () {
      this.generate();
      this.change();
    },

    generate: function () {
      var self = this;
      var el = self.element;
      var $el = $(el);

      $el
        .attr({
          'maxNumber': self.settings.maxNumber,
          'minNumber': self.settings.minNumber
        })
        .wrap("<span class='ckUpDownNumber' style='height:" + self.settings.height + "px;'></span>")
        .parent()
        .append('<input class="ckbutton ckUDNumber ckUpNum' +
          '" style="width: ' + self.settings.width + 'px;height: ' +
          self.settings.height + 'px" type="button" value="&#9650;" />' +
          '<input class="ckbutton ckUDNumber ckDownNum' +
          '" type="button" style="width: ' + self.settings.width +
          'px;height: ' + self.settings.height + 'px" value="&#9660;" />');

      $el.siblings('.ckUpNum').unbind('click').on('click', function (event) {
        event.preventDefault();
        var elValue = parseInt($el.val(), 10);

        if (isNaN(elValue)) {
          $el.val('0');
          elValue = 0;
        }

        if (elValue >= $el.attr('maxnumber')) {
          return;
        }

        $el.val(elValue + 1);
        //trigger auto change for resize content area
        $el.parent('.ckUpDownNumber').find('#nHeight').trigger('change.height');
        $el.parent('.ckUpDownNumber').find('#nWidth').trigger('change.width');
        $el.trigger('change')
      });

      $el.siblings('.ckDownNum').unbind('click').on('click', function (event) {
        event.preventDefault();
        var elValue = parseInt($el.val(), 10);

        if (isNaN(elValue)) {
          $el.val('0');
          elValue = 0;
        }

        if ($el.attr('minnumber') >= elValue) {
          return;
        }

        $el.val(elValue - 1);
        $el.trigger('change');
        //trigger auto change for resize content area
        $el.parent('.ckUpDownNumber').find('#nHeight').trigger('change.height');
        $el.parent('.ckUpDownNumber').find('#nWidth').trigger('change.width');
      });
    },
    change: function () {
      var self = this;
      var el = self.element;
      var $el = $(el);
      $el.on('keydown', function (event) {
        var allowKeydown = [46, 8, 9, 27, 13];
        if (self.settings.minNumber < 0) {
          allowKeydown = [46, 8, 9, 27, 13, 109, 189];
        }
        // Allow: backspace, delete, tab, escape, enter and .
        if ($.inArray(event.keyCode, allowKeydown) !== -1 ||
          // Allow: Ctrl+A
          (event.keyCode == 65 && event.ctrlKey === true) ||
          // Allow: home, end, left, right
          (event.keyCode >= 35 && event.keyCode <= 39)) {
          // let it happen, don't do anything
          return;
        } else {
          // Ensure that it is a number and stop the keypress
          if (event.shiftKey || (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)) {
            event.preventDefault();
          }
        }
      });

      $el.on('change', function () {
        var $that = $(this);
        var iVal = $that.val();
        if (parseInt(iVal) > self.settings.maxNumber) {
          iVal = self.settings.maxNumber;
        } else if (parseInt(iVal) < self.settings.minNumber) {
          iVal = self.settings.minNumber;
        }
        $that.val(iVal);
      });
    }
  });

  // A really lightweight plugin wrapper around the constructor,
  // preventing against multiple instantiations
  $.fn[pluginName] = function (options) {
    this.each(function () {
      if (!$.data(this, "plugin_" + pluginName)) {
        $.data(this, "plugin_" + pluginName, new Plugin(this, options));
      }
    });

    // chain jQuery functions
    return this;
  };
})(jQuery, window, document);

/***
 * Remove all attributes
 *
 ***/
$.fn.removeAttributes = function () {
  return this.each(function () {
    var attributes = $.map(this.attributes, function (item) {
      return item.name;
    });
    var ele = $(this);
    $.each(attributes, function (i, item) {
      ele.removeAttr(item);
    });
  });
}

/***
 * Export data from mkEditor to XML
 * Return empty string if Editor can't export
 * isTextToSpeech: variable for xml still export for text to speech review.
 ***/
function xmlExport(isTextToSpeech) {
  isAddnew = false;
  //Clear unuse response id
  refreshResponseId();
  refreshIdHotSpot(iResult);
  //Clear unuser for depending
  checkElementRemoveIntoIResult();

  var qtiSchemeID = 0, //1: Multiple choice | 3: Multi-select | 8: Inline choice | 9: Text entry | 10: Open ended(Extended text) | 21: Complex
    responseDeclaration = "", //<responseDeclaration identifier="RESPONSE_13" baseType="identifier" cardinality="multiple" method="default" caseSensitive="false" type="string" pointsValue="6"><correctResponse><value>A</value><value>B</value><value>C</value></correctResponse></responseDeclaration>
    questionAudioLink = "",
    refObjectID = "", //<object class="referenceObject" stylename="referenceObject" refObjectID="2994"/>
    mainBody = "",
    questionType = "",
    allowExport = true;

  //create width and height for image incase it in table and has width and height is 100%
  var $imgTable = $('iframe.cke_wysiwyg_frame').contents().find('.linkit-table img');
  $imgTable.each(function () {
    var $currImg = $(this);
    var $attrWidth = $currImg.attr("width");
    var $attrHeight = $currImg.attr("height");
    if ($attrWidth == "100%") {
      $currImg.attr({
        "width": $currImg.width()
      });
    }

    if ($attrHeight == "auto") {
      $currImg.attr({
        "height": $currImg.height()
      })
    }
  });

  var tempData = '<div>' + CKEDITOR.instances[ckID].getData() + '</div>';
  var $tempData = $(tempData);
  $tempData.find("strong").removeAttr("style");
  $tempData.find("em").removeAttr("style");
  $tempData.find("u").removeAttr("style");
  $tempData.find(".placeholder").remove();

  //Return string
  tempData = '<div>' + $tempData.html() + '</div>';

  //Remove old class1 this just happen in old item
  tempData = tempData.replace(/<span class="class1" stylename="class1">/g, "<span>");

  //Processing for Sameline function
  tempData = tempData.replace(/<sameline/g, '<span').replace(/<\/sameline>/g, '</span>');
  tempData = tempData.replace(new RegExp('Your browser does not support the video tag.', "g"), '</source> Your browser does not support the video tag.');
  tempData = replaceVideo(tempData);
  tempData = replaceItemTypeOnImageMark(tempData)
  tempData = cleanXmlContent(tempData);

  // Remove audioref for table without audio
  var $tempDataEl = $(tempData);
  $tempDataEl.find('table').each(function () {
    var table = this;
    var tableAudioId = table.getAttribute('audioid');
    var hasAudio = false;
    if (tableAudioId) {
      var listAudio = $(tempData).find('.audioTable');
      for (var i = 0; i < listAudio.length; i++) {
        if (listAudio[i].getAttribute('audioid') == tableAudioId) {
          hasAudio = true;
          break;
        }
      }

      if (hasAudio == false) {
        table.setAttribute('audiotableref', '');
      }
    }
    //tempData = tempData.replace(this.outerHTML,"");
  });
  tempData = $tempDataEl[0].outerHTML;

  // Remove audio for table
  $(tempData).find('.audioTable').each(function () {
    tempData = tempData.replace(this.outerHTML, "");
  });

  // Make sure the properties match when replace
  tempData = $(tempData).prop("outerHTML").replace(/[\n\r\t]/g, "").replace(new RegExp('<input class="editvideo" value="Edit video" type="button">', "g"), ''),
    inlineChoiceInteraction = $(tempData).find(".inlineChoiceInteraction"),
    textEntryInteraction = $(tempData).find(".textEntryInteraction"),
    drawTool = $(tempData).find(".drawTool"),
    extendText = $(tempData).find(".extendText"),
    multipleChoice = $(tempData).find(".multipleChoice"),
    partialCredit = $(tempData).find(".partialCredit"),
    tableHotSpot = $(tempData).find(".tableHotspotInteraction"),
    tableHotSpotInline = $(tempData).find("span[typehotspot]").not(".tableHotspotInteraction span[typehotspot]"),
    imageHotspotInteraction = $(tempData).find(".imageHotspotInteraction"),
    numberLine = $(tempData).find('.numberline-selection'),
    sequence = $(tempData).find('.sequenceBlock');

  //Processing for inlineChoiceInteraction
  $.each(inlineChoiceInteraction, function () {
    var $inlinechoice = $(this);
    var currentResId = $(this).attr("id"),
      currentText = $(this).prop("outerHTML");

    var typeGuidance = '',
      typeRationale = '',
      typeGuidanceRationale = '';

    for (var i = 0; i < iResult.length; i++) {
      if (iResult[i].responseIdentifier == currentResId && iResult[i].type == "inlineChoiceInteraction") {
        qtiSchemeID = 8;
        var expectedWidth = iResult[i].expectedWidth;
        var visibleDimension = iResult[i].visibleDimension;
        var isAlgorithmicGrading = iResult[i].responseDeclaration.method === 'algorithmic';

        //User must be choice one answer before save
        if ((iResult[i].correctResponse == undefined || iResult[i].inlineChoice.length == 0) && !isAlgorithmicGrading) {
          alert(errorMsg.inlineChoice);
          allowExport = false;
          return;
        }

        var buildInlineChoiceInteraction = '<inlineChoiceInteraction expectedWidth="' + expectedWidth + '" visibleDimension="' + visibleDimension + '" responseIdentifier="' + iResult[i].responseIdentifier + '" shuffle="' + iResult[i].shuffle + '">';

        if (typeof $inlinechoice.attr('data-top') !== 'undefined' ||
          typeof $inlinechoice.attr('data-left') !== 'undefined') {
          var inlinechoiceTop = $inlinechoice.attr('data-top');
          var inlinechoiceLeft = $inlinechoice.attr('data-left');

          buildInlineChoiceInteraction = '<inlineChoiceInteraction';
          buildInlineChoiceInteraction += ' responseIdentifier="' + iResult[i].responseIdentifier + '"'
          buildInlineChoiceInteraction += ' shuffle="' + iResult[i].shuffle + '"'
          buildInlineChoiceInteraction += ' data-top="' + inlinechoiceTop + '"'
          buildInlineChoiceInteraction += ' data-left="' + inlinechoiceLeft + '"'
          buildInlineChoiceInteraction += ' expectedWidth="' + expectedWidth + '"';
          buildInlineChoiceInteraction += ' visibleDimension="' + visibleDimension + '"';
          buildInlineChoiceInteraction += ' style="position: absolute;'
          buildInlineChoiceInteraction += 'top: ' + inlinechoiceTop + 'px;';
          buildInlineChoiceInteraction += 'left: ' + inlinechoiceLeft + 'px;';
          buildInlineChoiceInteraction += '"';
          buildInlineChoiceInteraction += '>';
        }

        var iChoice = iResult[i].inlineChoice;
        for (var n = 0; n < iChoice.length; n++) {
          //Make sure the simple question has wrap by <p><span>
          var simpleContent = iChoice[n].value.replace(/<div/g, "<p").replace(/<\/div>/g, "</p>");

          var objSimpleChoice = $("<div></div>").append(simpleContent);

          if (objSimpleChoice.find("p").length == 0) {
            $(this).wrap("<p><span></span></p>");
          } else {
            objSimpleChoice.find("p").each(function () {
              if ($(this).find("span").length == 0) {
                var currrentP = $(this).html();
                $(this).empty().append("<span>" + currrentP + "</span>");
              }
            });
            //remove type message guidance into content answer choice
            if (objSimpleChoice.find("p[typemessage]").length) {
              objSimpleChoice.find("p[typemessage]").replaceWith('');
            }
          }

          //This make sure simple choice correct html syntax
          simpleContent = $("<div></div>").append(objSimpleChoice.html()).html().replace(/<p><span><\/span><\/p>/g, "");

          //apply data guidance into xml content
          if (iChoice[n].arrMessageGuidance.length) {
            for (var k = 0, lenArrMessage = iChoice[n].arrMessageGuidance.length; k < lenArrMessage; k++) {
              var itemMessage = iChoice[n].arrMessageGuidance[k];

              if (itemMessage.typeMessage === "guidance") {
                typeGuidance = xmlGuidance(itemMessage, "guidance", iChoice[n].identifier);
              }
              if (itemMessage.typeMessage === "rationale") {
                typeRationale = xmlGuidance(itemMessage, "rationale", iChoice[n].identifier);
              }
              if (itemMessage.typeMessage === "guidance_rationale") {
                typeGuidanceRationale = xmlGuidance(itemMessage, "guidance_rationale", iChoice[n].identifier);
              }
            }
          }

          buildInlineChoiceInteraction += '<inlineChoice pointsValue="' + iChoice[n].pointsValue + '" identifier="' + iChoice[n].identifier + '"><span class="inlineChoiceAnswer">' + simpleContent.replace(/(\r\n|\n|\r)/gm, "").replace(new RegExp("\>[\n\t ]+\<", "g"), "><").replace(/<p><\/p>$/g, '') + '</span>';
          buildInlineChoiceInteraction += typeGuidance;
          buildInlineChoiceInteraction += typeRationale;
          buildInlineChoiceInteraction += typeGuidanceRationale;
          buildInlineChoiceInteraction += '</inlineChoice>';

          //reset xml guidance after apply into answer choice
          typeGuidance = '';
          typeRationale = '';
          typeGuidanceRationale = '';
        }

        //return to major or depending
        var strMj = returnMajorDepending(iResult[i].responseIdentifier);

        if (isAlgorithmicGrading) {
          responseDeclaration += '<responseDeclaration identifier="' + iResult[i].responseIdentifier + '" baseType="' + iResult[i].responseDeclaration["baseType"] + '" cardinality="' + iResult[i].responseDeclaration["cardinality"] + '" method="' + iResult[i].responseDeclaration["method"] + '" caseSensitive="' + iResult[i].responseDeclaration["caseSensitive"] + '" type="' + iResult[i].responseDeclaration["type"] + '" pointsValue="' + iResult[i].responseDeclaration["pointsValue"] + '" ' + strMj + '></responseDeclaration>';
        } else {
          responseDeclaration += '<responseDeclaration identifier="' + iResult[i].responseIdentifier + '" baseType="' + iResult[i].responseDeclaration["baseType"] + '" cardinality="' + iResult[i].responseDeclaration["cardinality"] + '" method="' + iResult[i].responseDeclaration["method"] + '" caseSensitive="' + iResult[i].responseDeclaration["caseSensitive"] + '" type="' + iResult[i].responseDeclaration["type"] + '" pointsValue="' + iResult[i].responseDeclaration["pointsValue"] + '" ' + strMj + '><correctResponse><value>' + iResult[i].correctResponse + '</value></correctResponse></responseDeclaration>';
        }

        buildInlineChoiceInteraction += "</inlineChoiceInteraction>";

        tempData = tempData.replace(currentText, buildInlineChoiceInteraction);
        break;
      }
    }
  });

  //Processing for textEntryInteraction
  $.each(textEntryInteraction, function () {
    var $textentry = $(this);
    //Don't run code if export fasle
    if (allowExport == false) {
      return;
    }

    var currentResId = $(this).attr("id"),
      currentText = $(this).prop("outerHTML");

    for (var i = 0; i < iResult.length; i++) {
      if (iResult[i].responseIdentifier == currentResId && iResult[i].type == "textEntryInteraction") {
        qtiSchemeID = 9;
        var iResultItem = iResult[i];
        var isTextentryRange = iResultItem.responseDeclaration.range != undefined &&
          iResultItem.responseDeclaration.range.toString() == "true";
        var textentryResponse = iResultItem.responseIdentifier;
        var expectedLengthMax = iResultItem.expectedLengthMax;
        var expectedLengthMin = iResultItem.expectedLengthMin;
        var expectedWidth = iResultItem.expectedWidth;
        var validation = iResultItem.validation;
        var addPadding = iResultItem.addPadding;
        var customRule = '';
        if (validation === '4') {
          customRule = iResultItem.customRule;
        }
        var textentryMethod = iResultItem.responseDeclaration['method'];
        var textentryUngraded = textentryMethod !== 'ungraded' ? false : true;

        var buildTextEntry = '<textEntryInteraction expectedWidth="'+ expectedWidth +'" customRule="' + customRule + '" addPadding="' + addPadding + '" validation="' + validation + '" responseIdentifier="' + textentryResponse + '" expectedLengthMax="' + expectedLengthMax + '" expectedLengthMin="' + expectedLengthMin + '" data-ungraded="' + textentryUngraded + '"/>';

        if (typeof $textentry.attr('data-top') !== 'undefined' ||
          typeof $textentry.attr('data-left') !== 'undefined') {
          var textentryTop = $textentry.attr('data-top');
          var textentryLeft = $textentry.attr('data-left');

          buildTextEntry = '<textEntryInteraction';
          buildTextEntry += ' responseIdentifier="' + textentryResponse + '"';
          buildTextEntry += ' expectedLength="' + expectedLengthMax + '"';
          buildTextEntry += ' customRule="' + customRule + '"';
          buildTextEntry += ' addPadding="' + addPadding + '"';
          buildTextEntry += ' validation="' + validation + '"';
          buildTextEntry += ' expectedLengthMax="' + expectedLengthMax + '"';
          buildTextEntry += ' expectedLengthMin="' + expectedLengthMin + '"';
          buildTextEntry += ' expectedWidth="' + expectedWidth +'"';
          buildTextEntry += ' data-ungraded="' + textentryUngraded + '"';
          buildTextEntry += ' data-top="' + textentryTop + '"';
          buildTextEntry += ' data-left="' + textentryLeft + '"';
          buildTextEntry += ' style="position: absolute;';
          buildTextEntry += 'top: ' + textentryTop + 'px;';
          buildTextEntry += 'left: ' + textentryLeft + 'px;';
          buildTextEntry += '"';
          buildTextEntry += '/>';
        }

        if (isTextentryRange) {
          //return to major or depending
          var strMj = returnMajorDepending(textentryResponse);

          responseDeclaration += '<responseDeclaration identifier="' + textentryResponse + '" baseType="' + iResult[i].responseDeclaration["baseType"] + '" cardinality="' + iResult[i].responseDeclaration["cardinality"] + '" method="' + textentryMethod + '" caseSensitive="' + iResult[i].responseDeclaration["caseSensitive"] + '" type="' + iResult[i].responseDeclaration["type"] + '" pointsValue="' + iResult[i].responseDeclaration["pointsValue"] + '" range="' + iResult[i].responseDeclaration.range + '" spelling="' + iResult[i].responseDeclaration["spelling"] + '" ignoreExtraSpace="' + iResult[i].responseDeclaration["ignoreExtraSpace"] + '" spellingDeduction="' + iResult[i].responseDeclaration["spellingDeduction"] + '" ' + strMj + '>'
          if (iResult[i].correctResponse.length > 0) {
            responseDeclaration += '<correctResponse>';
            var corResponse = iResult[i].correctResponse;
            var buildValueRange = "";
            $.each(corResponse, function (index) {
              buildValueRange += "<rangeValue><name>" + corResponse[index].name + "</name><valueType>" + corResponse[index].valueType + "</valueType><value>" + corResponse[index].valueRange + "</value><exclusivity>" + corResponse[index].exclusivity + "</exclusivity></rangeValue>";
            });
            responseDeclaration += '<value>' + buildValueRange + '</value>';
            responseDeclaration += '</correctResponse>';
          }
          responseDeclaration += '</responseDeclaration>';

          tempData = tempData.replace($(this).prop("outerHTML"), buildTextEntry);
        } else {
          //User must be choice one answer before save
          if (iResult[i].correctResponse.length == 0) {
            alert(errorMsg.textEntry);
            allowExport = false;
            return;
          }

          //return to major or depending
          var strMj = returnMajorDepending(textentryResponse);

          responseDeclaration += '<responseDeclaration identifier="' + textentryResponse + '" baseType="' + iResult[i].responseDeclaration["baseType"] + '" cardinality="' + iResult[i].responseDeclaration["cardinality"] + '" method="' + textentryMethod + '" caseSensitive="' + iResult[i].responseDeclaration["caseSensitive"] + '" type="' + iResult[i].responseDeclaration["type"] + '" pointsValue="' + iResult[i].responseDeclaration["pointsValue"] + '" range="false" spelling="' + iResult[i].responseDeclaration["spelling"] + '" ignoreExtraSpace="' + iResult[i].responseDeclaration["ignoreExtraSpace"] + '" spellingDeduction="' + iResult[i].responseDeclaration["spellingDeduction"] + '" ' + strMj + '>'
          if (iResult[i].correctResponse.length > 0) {
            responseDeclaration += '<correctResponse>';
            var corResponse = iResult[i].correctResponse;
            for (var j = 0; j < corResponse.length; j++) {
              var simpleContent = corResponse[j].value.replace(/<div/g, "<p").replace(/<\/div>/g, "</p>");
              var objSimpleChoice = $("<div></div>").append(simpleContent);

              if (objSimpleChoice.find("p").length == 0) {
                $(this).wrap("<p><span></span></p>");
              } else {
                objSimpleChoice.find("p").each(function () {
                  if ($(this).find("span").length == 0) {
                    var currrentP = $(this).html();
                    $(this).empty().append("<span>" + currrentP + "</span>");
                  }
                });
                //remove type message guidance into content answer choice
                if (objSimpleChoice.find("p[typemessage]").length) {
                  objSimpleChoice.find("p[typemessage]").replaceWith('');
                }
              }
              //This make sure simple choice correct html syntax
              simpleContent = $("<div></div>").append(objSimpleChoice.html()).html().replace(/<p><span><\/span><\/p>/g, "");

              responseDeclaration += '<value identifier="' + corResponse[j].identifier + '" pointsValue="' + corResponse[j].pointsValue + '">' + simpleContent.replace(/(\r\n|\n|\r)/gm, "").replace(new RegExp("\>[\n\t ]+\<", "g"), "><").replace(/<p><\/p>$/g, '') + '</value>';
            }
            responseDeclaration += '</correctResponse>';

            var isTypeGuidance = false;
            for (var iCount = 0, lencorResponse = corResponse.length; iCount < lencorResponse; iCount++) {
              if (corResponse[iCount].arrMessageGuidance.length) {
                isTypeGuidance = true;
              }
            }
            //apply xml guidance ,rationale for text entry
            if (isTypeGuidance) {
              responseDeclaration += '<responseRubric>';
              for (var iCount = 0, lencorResponse = corResponse.length; iCount < lencorResponse; iCount++) {
                if (corResponse[iCount].arrMessageGuidance.length) {
                  //item guidance
                  for (var k = 0, lenArrMessageGuidance = corResponse[iCount].arrMessageGuidance.length; k < lenArrMessageGuidance; k++) {
                    var itemObjMessage = corResponse[iCount].arrMessageGuidance[k];
                    responseDeclaration += '<value type="' + itemObjMessage.typeMessage + '" audioRef="' + itemObjMessage.audioRef + '" ansIdentifier="' + corResponse[iCount].identifier + '">' + itemObjMessage.valueContent + '</value>';
                  }
                }
              } //end apply xml guidance ,rationale for text entry
              responseDeclaration += '</responseRubric>';
            }
          }
          responseDeclaration += '</responseDeclaration>';

          tempData = tempData.replace($(this).prop("outerHTML"), buildTextEntry);
        }
        break;
      }
    }
  });

  //Processing for drawTool
  $.each(drawTool, function () {
    var $drawinteraction = $(this);
    var currentResId = $drawinteraction.attr('id');
    var currentText = $drawinteraction.prop('outerHTML');
    var drawingTextToSpeech = $drawinteraction.find('img[percent]').attr('texttospeech');

    drawingTextToSpeech = drawingTextToSpeech == null ? '' : drawingTextToSpeech;
    drawingTextToSpeech = convertTexttoHTML(drawingTextToSpeech);

    for (var i = 0; i < iResult.length; i++) {
      var iResultItem = iResult[i];

      if (iResultItem.responseIdentifier == currentResId && iResultItem.type == "extendedTextInteractionDraw") {
        var percent = iResultItem.percent;
        var gridSize = iResultItem.gridSize;
        var wOrgImage = iResultItem.wOrgImage;
        var hOrgImage = iResultItem.hOrgImage;
        var drawingDataType = iResultItem.dataType;
        var buildExtendedText = '';

        qtiSchemeID = 10;

        var buildExtendedTextImg = '<img wOrgImage="' + wOrgImage + '" hOrgImage="' + hOrgImage + '" percent="' + percent + '" src="' + iResultItem.srcImage + '" drawable="' + iResultItem.drawable + '" width="' + iResultItem.width + '" height="' + iResultItem.height + '" texttospeech="' + drawingTextToSpeech + '" />';

        if (drawingDataType === 'basic') {
          var drawingDataWidth = iResultItem.dataWidth;
          var drawingDataHeight = iResultItem.dataHeight;
          buildExtendedText = '<extendedTextInteraction gridSize="' + gridSize + '" drawable="' + iResultItem.drawable + '" responseIdentifier="' + iResultItem.responseIdentifier + '" width="' + iResultItem.width + '" height="' + iResultItem.height + '" data-type="' + drawingDataType + '" data-width="' + drawingDataWidth + '" data-height="' + drawingDataHeight + '">' + buildExtendedTextImg + '</extendedTextInteraction>';
        } else {
          buildExtendedText = '<extendedTextInteraction drawable="' + iResultItem.drawable + '" responseIdentifier="' + iResultItem.responseIdentifier + '" width="' + iResultItem.width + '" height="' + iResultItem.height + '">' + buildExtendedTextImg + '</extendedTextInteraction>';
        }

        //return to major or depending
        var strMj = returnMajorDepending(iResultItem.responseIdentifier);
        responseDeclaration += '<responseDeclaration identifier="' + iResultItem.responseIdentifier + '" baseType="' + iResultItem.responseDeclaration["baseType"] + '" cardinality="' + iResultItem.responseDeclaration["cardinality"] + '" method="' + iResultItem.responseDeclaration["method"] + '" caseSensitive="' + iResultItem.responseDeclaration["caseSensitive"] + '" type="' + iResultItem.responseDeclaration["type"] + '" pointsValue="' + iResultItem.responseDeclaration["pointsValue"] + '" ' + strMj + '/>';

        tempData = tempData.replace(currentText, buildExtendedText);
        break;
      }
    }
  });

  //Processing for extendText
  $.each(extendText, function () {
    var currentResId = $(this).attr("id"),
      currentText = $(this).prop("outerHTML");

    for (var i = 0; i < iResult.length; i++) {
      if (iResult[i].responseIdentifier == currentResId &&
        iResult[i].type == "extendedTextInteraction") {
        qtiSchemeID = 10;
        var extendtextResponse = iResult[i].responseIdentifier;
        var extendtextUngraded = iResult[i].responseDeclaration.method !== 'ungraded' ? false : true;
        var extendedtextLength = iResult[i].expectedLength;
        var formatText = !!iResult[i].formatText;
        var placeholderText = escapeHtml(iResult[i].placeholderText || '');
        var extendedtextHeight = $(currentText).height();

        if (extendedtextHeight == null || extendedtextHeight === 0) {
          extendedtextHeight = 90;
        }

        var buildExtendedText = '<extendedTextInteraction placeholderText="'+ placeholderText +'" responseIdentifier="' + extendtextResponse + '" expectedLength="' + extendedtextLength + '" style="height: ' + extendedtextHeight + 'px;" formatText="'+ formatText +'"  data-ungraded="' + extendtextUngraded + '"/>';
        // Return to major or depending
        var strMj = returnMajorDepending(extendtextResponse);
        responseDeclaration += '<responseDeclaration identifier="' + extendtextResponse + '" baseType="' + iResult[i].responseDeclaration["baseType"] + '" cardinality="' + iResult[i].responseDeclaration["cardinality"] + '" method="' + iResult[i].responseDeclaration["method"] + '" caseSensitive="' + iResult[i].responseDeclaration["caseSensitive"] + '" type="' + iResult[i].responseDeclaration["type"] + '" pointsValue="' + iResult[i].responseDeclaration["pointsValue"] + '" ' + strMj + '/>';

        tempData = tempData.replace(currentText, buildExtendedText);
        break;
      }
    }
  });

  //Processing for multiple Choice
  $.each(multipleChoice, function () {
    var currentResId = $(this).attr("id"),
      currentText = $(this).prop("outerHTML");

    var typeGuidance = '',
      typeRationale = '',
      typeGuidanceRationale = '';

    for (var i = 0; i < iResult.length; i++) {
      if (iResult[i].responseIdentifier == currentResId && iResult[i].type == "choiceInteraction") {
        if (qtiSchemeID != 0) {
          qtiSchemeID = 21;
        }

        if (iResult[i].responseDeclaration.cardinality === 'multiple') {
          qtiSchemeID = 3;
        } else {
          qtiSchemeID = 1;
        }

        //Update answer of multiple Choice
        var buildChoiceInteraction = '<choiceInteraction responseIdentifier="' + iResult[i].responseIdentifier + '" shuffle="' + iResult[i].shuffle + '" maxChoices="' + iResult[i].maxChoices + '" data-display="' + iResult[i].display + '">';

        if (iResult[i].subtype != null && iResult[i].subtype == "TrueFalse") {
          buildChoiceInteraction = '<choiceInteraction subtype="' + iResult[i].subtype + '" responseIdentifier="' + iResult[i].responseIdentifier + '" shuffle="' + iResult[i].shuffle + '" maxChoices="' + iResult[i].maxChoices + '">';
        }

        if (iResult[i].display === 'grid') {
          buildChoiceInteraction = '<choiceInteraction responseIdentifier="' + iResult[i].responseIdentifier + '" shuffle="' + iResult[i].shuffle + '" maxChoices="' + iResult[i].maxChoices + '" data-display="' + iResult[i].display + '" data-grid-per-row="' + iResult[i].gridPerRow + '">';
        }

        var mySimpleChoice = iResult[i].simpleChoice;
        for (m = 0; m < mySimpleChoice.length; m++) {
          var audiolink = "";
          if (mySimpleChoice[m].audioRef != undefined) {
            audiolink = 'audioRef="' + mySimpleChoice[m].audioRef.replace("|", "/") + '" ';
          }

          //Make sure the simple question has wrap by <p><span>
          var simpleContent = mySimpleChoice[m].value.replace(/<div/g, "<p").replace(/<\/div>/g, "</p>");
          //Remove old class1 this just happen in old item
          simpleContent = simpleContent.replace(/<span stylename="class1" class="class1">/g, "<span>").replace(/<span class="class1" stylename="class1">/g, "<span>");
          var objSimpleChoice = $("<div></div>").append(simpleContent);

          if (objSimpleChoice.find("p").length == 0) {
            $(this).wrap("<p><span></span></p>");
          } else {
            objSimpleChoice.find("p").each(function () {
              if ($(this).find("span").length == 0) {
                var currrentP = $(this).html();
                $(this).empty().append("<span>" + currrentP + "</span>")
              }
            });
            //remove type message guidance into content answer choice
            if (objSimpleChoice.find("p[typemessage]").length) {
              objSimpleChoice.find("p[typemessage]").replaceWith('');
            }
          }

          //This make sure simple choice correct html syntax
          simpleContent = $("<div></div>").append(objSimpleChoice.html()).html().replace(/<p><span><\/span><\/p>/g, "");

          //apply data guidance into xml content
          if (mySimpleChoice[m].arrMessageGuidance.length) {
            for (var k = 0, lenArrMessage = mySimpleChoice[m].arrMessageGuidance.length; k < lenArrMessage; k++) {
              var itemMessage = mySimpleChoice[m].arrMessageGuidance[k];

              if (itemMessage.typeMessage === "guidance") {
                typeGuidance = xmlGuidance(itemMessage, "guidance", mySimpleChoice[m].identifier);
              }
              if (itemMessage.typeMessage === "rationale") {
                typeRationale = xmlGuidance(itemMessage, "rationale", mySimpleChoice[m].identifier);
              }
              if (itemMessage.typeMessage === "guidance_rationale") {
                typeGuidanceRationale = xmlGuidance(itemMessage, "guidance_rationale", mySimpleChoice[m].identifier);
              }
            }
          }

          //buildChoiceInteraction += '<simpleChoice ' + audiolink + 'identifier="' + mySimpleChoice[m].identifier + '"><div class="answer" styleName="answer">' + simpleContent.replace(/(\r\n|\n|\r)/gm, "").replace(new RegExp("\>[\n\t]+\<", "g"), "><").replace(/<p><\/p>$/g, '') + '</div></simpleChoice>';
          buildChoiceInteraction += '<simpleChoice ' + audiolink + 'identifier="' + mySimpleChoice[m].identifier + '">';
          buildChoiceInteraction += '<div class="answer" styleName="answer">' + simpleContent.replace(/(\r\n|\n|\r)/gm, "").replace(new RegExp("\>[\n\t]+\<", "g"), "><").replace(/<p><\/p>$/g, '') + '</div>';
          buildChoiceInteraction += typeGuidance;
          buildChoiceInteraction += typeRationale;
          buildChoiceInteraction += typeGuidanceRationale;
          buildChoiceInteraction += '</simpleChoice>';

          //reset xml guidance after apply into answer choice
          typeGuidance = '';
          typeRationale = '';
          typeGuidanceRationale = '';
        }

        buildChoiceInteraction += '</choiceInteraction>';

        var enableThresHold = "0";
        if (!!iResult[i].responseDeclaration.thresholdpoints && iResult[i].responseDeclaration.thresholdpoints.length > 0) {
          enableThresHold = "1";
        }

        //return to major or depending
        var strMj = returnMajorDepending(iResult[i].responseIdentifier);

        //Build Response declaration
        responseDeclaration += '<responseDeclaration identifier="' + iResult[i].responseIdentifier + '" baseType="' + iResult[i].responseDeclaration["baseType"] + '" cardinality="' + iResult[i].responseDeclaration["cardinality"] + '" method="' + iResult[i].responseDeclaration["method"] + '" caseSensitive="' + iResult[i].responseDeclaration["caseSensitive"] + '" type="' + iResult[i].responseDeclaration["type"] + '" pointsValue="' + iResult[i].responseDeclaration["pointsValue"] + '" ' + strMj + ' thresholdGrading="' + enableThresHold + '">';
        if (iResult[i].correctResponse.length > 0) {
          responseDeclaration += '<correctResponse>';
          var corValue = iResult[i].correctResponse.split(',');

          if (iResult[i].responseDeclaration.cardinality == 'multiple' && qtiSchemeID != 21) {
            if (!!iResult[i].responseDeclaration.thresholdpoints) {
              var thresholdpoints = iResult[i].responseDeclaration.thresholdpoints;
              var thresholdpointsHtml = '';
              var thresholdpointObj = [];
              var threshold = '';

              for (var j = 0, len = thresholdpoints.length; j < len; j++) {
                threshold = thresholdpoints[j];
                thresholdpointObj.push('<threshold low="' + threshold.low + '" hi="' + threshold.hi + '" pointsValue="' + threshold.pointsvalue + '"/>');
              }

              thresholdpointsHtml = '<thresholds>' + thresholdpointObj.join('') + '</thresholds>';

              responseDeclaration += thresholdpointsHtml;
            }
          }

          for (var n = 0; n < corValue.length; n++) {
            responseDeclaration += '<value>' + corValue[n] + '</value>';
          }

          responseDeclaration += '</correctResponse>';
        }
        responseDeclaration += '</responseDeclaration>';

        tempData = tempData.replace(currentText, buildChoiceInteraction);
        break;
      } else if (iResult[i].responseIdentifier == currentResId && iResult[i].type == "choiceInteractionVariable") {
        if (qtiSchemeID != 0) {
          qtiSchemeID = 21;
        } else {
          qtiSchemeID = 37;
        }

        //Update answer of multiple Choice
        var buildChoiceInteraction = '<choiceInteraction responseIdentifier="' + iResult[i].responseIdentifier + '" shuffle="' + iResult[i].shuffle + '" maxChoices="' + iResult[i].maxChoices + '" variablePoints="true" data-display="' + iResult[i].display + '">';

        if (iResult[i].display === 'grid') {
          buildChoiceInteraction = '<choiceInteraction responseIdentifier="' + iResult[i].responseIdentifier + '" shuffle="' + iResult[i].shuffle + '" maxChoices="' + iResult[i].maxChoices + '" variablePoints="true" data-display="' + iResult[i].display + '" data-grid-per-row="' + iResult[i].gridPerRow + '">';
        }

        //return to major or depending
        var strMj = returnMajorDepending(iResult[i].responseIdentifier);
        responseDeclaration += '<responseDeclaration identifier="' + iResult[i].responseIdentifier + '" baseType="' + iResult[i].responseDeclaration["baseType"] + '" cardinality="' + iResult[i].responseDeclaration["cardinality"] + '" method="' + iResult[i].responseDeclaration["method"] + '" caseSensitive="' + iResult[i].responseDeclaration["caseSensitive"] + '" type="' + iResult[i].responseDeclaration["type"] + '" pointsValue="' + iResult[i].responseDeclaration["pointsValue"] + '" ' + strMj + '>';
        responseDeclaration += '<correctResponse>';

        var mySimpleChoice = iResult[i].simpleChoice;
        //Build Response declaration

        for (m = 0; m < mySimpleChoice.length; m++) {
          var audiolink = "";
          if (mySimpleChoice[m].audioRef != undefined) {
            audiolink = 'audioRef="' + mySimpleChoice[m].audioRef.replace("|", "/") + '" ';
          }

          //Make sure the simple question has wrap by <p><span>
          var simpleContent = mySimpleChoice[m].value.replace(/<div/g, "<p").replace(/<\/div>/g, "</p>");
          //Remove old class1 this just happen in old item
          simpleContent = simpleContent.replace(/<span stylename="class1" class="class1">/g, "<span>");
          var objSimpleChoice = $("<div></div>").append(simpleContent);

          if (objSimpleChoice.find("p").length == 0) {
            $(this).wrap("<p><span></span></p>");
          } else {
            objSimpleChoice.find("p").each(function () {
              if ($(this).find("span").length == 0) {
                var currrentP = $(this).html();
                $(this).empty().append("<span>" + currrentP + "</span>")
              }
            });
          }

          //This make sure simple choice correct html syntax
          simpleContent = $("<div></div>").append(objSimpleChoice.html()).html().replace(/<p><span><\/span><\/p>/g, "");

          buildChoiceInteraction += '<simpleChoice ' + audiolink + 'identifier="' + mySimpleChoice[m].identifier + '"><div class="answer" styleName="answer">' + simpleContent.replace(/(\r\n|\n|\r)/gm, "").replace(new RegExp("\>[\n\t]+\<", "g"), "><").replace(/<p><\/p>$/g, '') + '</div></simpleChoice>';

          //Build Correct Response
          responseDeclaration += '<value identifier="' + mySimpleChoice[m].identifier + '">' + mySimpleChoice[m].answerPoint + '</value>';
        }

        buildChoiceInteraction += '</choiceInteraction>';
        responseDeclaration += '</correctResponse>';
        responseDeclaration += '</responseDeclaration>';

        tempData = tempData.replace(currentText, buildChoiceInteraction);
        break;
      }
    }
  });

  //Processing responseDeclaration for table hot spot
  for (var i = 0, iResultLen = iResult.length; i < iResultLen; i++) {
    if (iResult[i].type === 'tableHotSpot') {
      //Build Response declaration
      responseDeclaration += '<responseDeclaration partialGrading="' + iResult[i].responseDeclaration.partialGrading + '" absoluteGrading="' + iResult[i].responseDeclaration.absoluteGrading + '" algorithmicGrading="' + iResult[i].responseDeclaration.algorithmicGrading + '" identifier="' + iResult[i].responseIdentifier + '" pointsValue="' + iResult[i].responseDeclaration.pointsValue + '">';
      if (iResult[i].correctResponse.length) {
        for (var j = 0, correctResponseLen = iResult[i].correctResponse.length; j < correctResponseLen; j++) {
          responseDeclaration += '<correctResponse identifier="' + iResult[i].correctResponse[j].identifier + '" pointValue="' + iResult[i].correctResponse[j].pointValue + '" isAbsolute="' + iResult[i].correctResponse[j].isAbsolute + '">';
          responseDeclaration += '</correctResponse>';
        }
      }
      responseDeclaration += '</responseDeclaration>';
    }
  }
  //Processing for table hot spot
  $.each(tableHotSpot, function (tableIndex) {
    //var currentResId = $(this).attr("id");
    var currentResId = iResult[0].responseIdentifier;
    var currentHtml = $(this).prop("outerHTML");
    var buildTableHotSpot = currentHtml;
    var arrIdTooltip = [];
    var idTooltip = '';
    var objcss = "display: none;position: absolute; top: -3px;font-size: 11px;line-height: 9px; background: #AD2041;left: 15px;padding-top: 1px;color: #fff";

    for (var i = 0, iResultLen = iResult.length; i < iResultLen; i++) {
      if (iResult[i].responseIdentifier === currentResId && iResult[i].type === "tableHotSpot") {
        qtiSchemeID = 33;
        //check source table hot spot
        if (isEmpty(iResult[i].sourceHotSpot)) {
          alert(errorMsg.addStyleTableHotSpot);
          allowExport = false;
          return;
        }
        //check select answer correct
        if (iResult[i].correctResponse.length === 0 && iResult[i].responseDeclaration.algorithmicGrading !== '1') {
          alert(errorMsg.addCorrectHotSpot);
          allowExport = false;
          return;
        }

        var tagHotSpots = $(buildTableHotSpot).find('span[typehotspot]');

        $.each(tagHotSpots, function () {
          var spanHotSpot = $(this);
          var oldHotSpot = spanHotSpot.prop("outerHTML");
          var typehotspot = '';
          var classhotspot = '';
          var newHotSpot = '';

          var idHotSpot = spanHotSpot.attr("identifier");
          arrIdTooltip = idHotSpot.split('_');
          idTooltip = arrIdTooltip[1];

          var ischecked = spanHotSpot.attr("ischecked");
          var pointValue = spanHotSpot.attr("pointvalue");
          if (spanHotSpot.attr("typehotspot") === 'circle') {
            typehotspot = 'circle';
            classhotspot = 'hotspot-circle';
            if (spanHotSpot.hasClass('selected')) {
              classhotspot = 'hotspot-circle selected';
            }
            if (spanHotSpot.hasClass('bubble')) {
              classhotspot += ' bubble';
            }
            if (spanHotSpot.hasClass('top')) {
              classhotspot += ' top'
            } else if (spanHotSpot.hasClass('bottom')) {
              classhotspot += ' bottom'
            }
          } else {
            typehotspot = 'checkbox';
            classhotspot = 'hotspot-checkbox';
            if ($(this).hasClass('selected')) {
              classhotspot = 'hotspot-checkbox selected';
            }
          }
          var maxhotspotAttr = '';
          if (!spanHotSpot.closest('.tableHotspotInteraction ').length) {
            maxhotspotAttr = ' maxhotspot="' + spanHotSpot.attr('maxhotspot') + '" '
          }

          newHotSpot = '<tableitem pointvalue="' + pointValue + '" typehotspot="' + typehotspot + '" class="' + classhotspot + '" identifier="' + idHotSpot + '" contenteditable="false" ischecked="' + ischecked + '">' + (classhotspot.includes('bubble') ? '' : '&nbsp;') + '</tableitem>';
          buildTableHotSpot = buildTableHotSpot.replace(oldHotSpot, newHotSpot);
        });

        $tempDataEl = $(tempData);
        $tempDataEl.find(".tableHotspotInteraction").eq(tableIndex).replaceWith(buildTableHotSpot);
        tempData = $tempDataEl[0].outerHTML;
        break;
      }
    }
  });
  if (tableHotSpotInline.length && qtiSchemeID == 0) {
    qtiSchemeID = 33;
  }
  //Processing for image hot spot selection
  $.each(imageHotspotInteraction, function () {
    var $self = $(this);
    var currentText = $self.prop('outerHTML');
    currentResId = $self.attr('id');

    var imghotspotTexttospech = $self.find('.imageHotspotMarkObject').attr('texttospeech');

    imghotspotTexttospech = imghotspotTexttospech === undefined ? '' : imghotspotTexttospech;

    imghotspotTexttospech = convertTexttoHTML(imghotspotTexttospech);

    for (var i = 0; i < iResult.length; i++) {
      var iResultItem = iResult[i];
      if (iResultItem.responseIdentifier == currentResId && iResultItem.type == "imageHotSpot") {
        qtiSchemeID = 32;

        var buildHotSpotSource = '';
        var buildHotSpotText = '';
        buildResponse = "";

        for (var j = 0; j < iResultItem.sourceItem.length; j++) {
          var iResultItemSource = iResultItem.sourceItem[j];
          var identifierItemSource = iResultItemSource.identifier;
          var pointItemSource = iResultItemSource.pointValue;
          var leftItemSource = iResultItemSource.left;
          var topItemSource = iResultItemSource.top;
          var widthItemSource = iResultItemSource.width;
          var heightItemSource = iResultItemSource.height;
          var typeItemSource = iResultItemSource.typeHotSpot;
          var valueItemSource = iResultItemSource.value;
          var hiddenItemSource;

          hiddenItemSource = iResultItemSource.hidden == undefined ? false : iResultItemSource.hidden;

          if (typeItemSource === 'border-style') {
            var showBorderItemSource, fillItemSource, rolloverPreviewItemSource;

            showBorderItemSource = iResultItemSource.showBorder == undefined ? false : iResultItemSource.showBorder;
            fillItemSource = iResultItemSource.fill == undefined ? false : iResultItemSource.fill;
            rolloverPreviewItemSource = iResultItemSource.rolloverPreview == undefined ? false : iResultItemSource.rolloverPreview;

            buildHotSpotSource += '<sourceItem identifier="' + identifierItemSource + '" pointValue="' + pointItemSource + '" left="' + leftItemSource + '" top="' + topItemSource + '" width="' + widthItemSource + '" height="' + heightItemSource + '" typeHotSpot="' + typeItemSource + '" hiddenHotSpot="' + hiddenItemSource + '" showBorderHotSpot="' + showBorderItemSource + '" fillHotSpot="' + fillItemSource + '" rolloverPreviewHotSpot="' + rolloverPreviewItemSource + '">' + valueItemSource + '</sourceItem>';
          } else {
            buildHotSpotSource += '<sourceItem identifier="' + identifierItemSource + '" pointValue="' + pointItemSource + '" left="' + leftItemSource + '" top="' + topItemSource + '" width="' + widthItemSource + '" height="' + heightItemSource + '" typeHotSpot="' + typeItemSource + '" hiddenHotSpot="' + hiddenItemSource + '">' + valueItemSource + '</sourceItem>';
          }
        }

        for (n = 0; n < iResultItem.correctResponse.length; n++) {
          buildResponse += '<correctResponse identifier="' + iResultItem.correctResponse[n].identifier + '" pointValue="' + iResultItem.correctResponse[n].pointValue + '"/>';
        }

        responseDeclaration += '<responseDeclaration identifier="' + iResultItem.responseIdentifier + '" absoluteGrading="' + iResultItem.responseDeclaration.absoluteGrading + '" partialGrading="' + iResultItem.responseDeclaration.partialGrading + '" algorithmicGrading="' + iResultItem.responseDeclaration.algorithmicGrading + '" pointsValue="' + iResultItem.responseDeclaration.pointsValue + '">' + buildResponse + '</responseDeclaration>';

        var imgHotSpotSrc = iResultItem.source.src;
        if (imgHotSpotSrc != null) {
          var idx = imgHotSpotSrc.indexOf('/ItemSet_');
          if (idx > 0) {
            imgHotSpotSrc = imgHotSpotSrc.substring(idx, imgHotSpotSrc.length); //remove s3 domain
          }
        }

        buildHotSpotText += '<imageHotSpot responseIdentifier="' + iResultItem.responseIdentifier + '" src="' + imgHotSpotSrc + '" percent="' + iResultItem.source.percent + '" imgorgw="' + iResultItem.source.imgorgw + '" imgorgh="' + iResultItem.source.imgorgh + '" width="' + iResultItem.source.width + '" height="' + iResultItem.source.height + '" maxhotspot="' + iResultItem.source.maxhotspot + '" texttospeech="' + imghotspotTexttospech + '">';
        buildHotSpotText += buildHotSpotSource;
        buildHotSpotText += '</imageHotSpot>';

        tempData = tempData.replace(currentText, buildHotSpotText);
        break;
      }
    }
  });

  //Processing for partial Credit
  var buildItemThreshold = '';
  for (i = 0; i < iResult.length; i++) {
    if (iResult[i].type == "partialCredit") {
      refreshPartialCredit();
      var isThresholdGrading = iResult[0].responseDeclaration.thresholdGrading;
      var isAlgorithmicGrading = iResult[0].responseDeclaration.algorithmicGrading;
      var currentResId = iResult[i].responseIdentifier,
        partialID = iResult[i].partialID;
      correctResponse = iResult[i].correctResponse;
      var buildResponse = '';

      qtiSchemeID = 30;

      // This build partial Source
      $(tempData).find(".partialSourceObject").each(function () {
        var $partialSourceObj = $(this);

        for (var k = 0; k < iResult[i].source.length; k++) {
          if ($partialSourceObj.attr("srcidentifier") == iResult[i].source[k].srcIdentifier) {
            // Set height if dragging object is text
            if (iResult[i].source[k].type === 'text') {
              var wTemp = $partialSourceObj.css('width');

              if (wTemp == '0px') {
                wTemp = 'auto';
              }

              var cloneObj = $partialSourceObj.clone();
              cloneObj.find('.partialAddSourceTextMark').remove();
              tempData = tempData.replace($partialSourceObj.prop('outerHTML'), '<sourceObject partialID="Partial_1" style="width: ' + wTemp + '; height: ' + $partialSourceObj.css('height') + ';" srcIdentifier="' + $partialSourceObj.attr("srcidentifier") + '" type="' + iResult[i].source[k].type + '" data-limit="' + iResult[i].source[k].limit + '">' + (cloneObj.html() || '') + '</sourceObject>');
            } else {
              //check source has float or not
              var strFloatSource = ' float="' + $partialSourceObj.css("float") + '"';
              if ($partialSourceObj.css("float") == "none" || $partialSourceObj.css("float") == "") {
                strFloatSource = "";
              }
              var originXml = $partialSourceObj.prop('outerHTML');
              if ($partialSourceObj.parents('.partialSourceImageWrapper').length) {
                originXml = $partialSourceObj.parents('.partialSourceImageWrapper').last().prop('outerHTML');
              }
              tempData = tempData.replace(originXml, '<sourceObject partialID="Partial_1" srcIdentifier="' + $partialSourceObj.attr("srcidentifier") + '" type="' + iResult[i].source[k].type + '"' + strFloatSource + ' data-limit="' + iResult[i].source[k].limit + '">' + iResult[i].source[k].value + '</sourceObject>');
            }
            break;
          }
        }
      });

      // This build partial destinationObject
      $(tempData).find(".partialDestinationObject").each(function () {
        var $partialDesObj = $(this);
        var percent = $partialDesObj.attr('percent');
        var imgorgw = $partialDesObj.attr('imgorgw');
        var imgorgh = $partialDesObj.attr('imgorgh');

        if ($partialDesObj.attr("type") == "text") {
          //get order
          var order = 0;
          for (var h = 0; h < correctResponse.length; h++) {
            if ($partialDesObj.attr("destidentifier") == correctResponse[h].destIdentifier) {
              order = correctResponse[h].order;
            }
          }

          var cloneDest = $partialDesObj.clone();
          cloneDest.find('.partialAddDestinationTextMark').remove();
          tempData = tempData.replace($partialDesObj.prop('outerHTML'), '<destinationObject partialID="Partial_1" type="text"><destinationItem destIdentifier="' + $partialDesObj.attr("destidentifier") + '" order="' + order + '" width="' + $partialDesObj.width() + '" height="' + $partialDesObj.height() + '" numberDroppable="' + $partialDesObj.attr('numberDroppable') + '" notRequireAllAnswers="' + $partialDesObj.attr('notRequireAllAnswers') + '">' + (cloneDest.html() || '') + '</destinationItem></destinationObject>');
        } else if ($partialDesObj.attr("type") == "image") {
          //Build destinationItem
          var hotSpot = "";
          $partialDesObj.find(".hotSpot").each(function (index, hotspot) {
            var $hotspot = $(hotspot);
            var gridcell = $hotspot.attr('gridcell');
            var hotspotNumberDroppable = $hotspot.attr('numberDroppable');
            var hotspotIdentifier = $hotspot.attr('destidentifier');
            var hotspotLeft = $hotspot.css('left').replace('px', '');
            var hotspotTop = $hotspot.css('top').replace('px', '');
            var hotspotWidth = $hotspot.css('width').replace('px', '');
            var hotspotHeight = $hotspot.css('height').replace('px', '');

            hotspotNumberDroppable = hotspotNumberDroppable == undefined ? '1' : hotspotNumberDroppable;
            var notRequireAllAnswers = $hotspot.attr('notRequireAllAnswers') || '0';
            //get order
            var order = 0;
            for (var h = 0; h < correctResponse.length; h++) {
              if (hotspotIdentifier == correctResponse[h].destIdentifier) {
                order = correctResponse[h].order;
              }
            }

            if (gridcell !== undefined && gridcell === 'true') {
              hotSpot += '<destinationItem destIdentifier="' + hotspotIdentifier + '" left="' + hotspotLeft + '" top="' + hotspotTop + '" width="' + hotspotWidth + '" height="' + hotspotHeight + '" order="' + order + '" gridcell="true" numberDroppable="' + hotspotNumberDroppable + '" notRequireAllAnswers="' + notRequireAllAnswers + '"></destinationItem>';
            } else {
              hotSpot += '<destinationItem destIdentifier="' + hotspotIdentifier + '" left="' + hotspotLeft + '" top="' + hotspotTop + '" width="' + hotspotWidth + '" height="' + hotspotHeight + '" order="' + order + '" numberDroppable="' + hotspotNumberDroppable + '" notRequireAllAnswers="' + notRequireAllAnswers + '"></destinationItem>';
            }
          });

          //check destination has float or not
          var strFloat = ' float="' + $partialDesObj.css("float") + '"';
          if ($partialDesObj.css("float") == "none" || $partialDesObj.css("float") == "") {
            strFloat = "";
          }

          //get all destination in one group
          var strHTML = '';
          var destinationImageSrc = $partialDesObj.find(".destinationImage").attr("src");
          if (destinationImageSrc != null) {
            var idx = destinationImageSrc.indexOf('/ItemSet_');
            if (idx > 0) {
              destinationImageSrc = destinationImageSrc.substring(idx, destinationImageSrc.length).replace(/%20/g, ' ');
            }
          }

          //Get text to speech
          var tts = $partialDesObj.find(".destinationImage").attr("texttospeech");

          //strHTML += '<destinationObject percent="' + percent + '" imgorgw="' + imgorgw + '" imgorgh="' + imgorgh + '" partialID="Partial_1" src="' + $partialDesObj.find(".destinationImage").attr("src") + '" width="' + $partialDesObj.css("width").replace("px", "") + '" height="' + $partialDesObj.css("height").replace("px", "") + '" type="image"' + strFloat + '>';
          strHTML += '<destinationObject percent="' + percent + '" imgorgw="' + imgorgw + '" imgorgh="' + imgorgh + '" partialID="Partial_1" src="' + destinationImageSrc + '" width="' + $partialDesObj.css("width").replace("px", "") + '" height="' + $partialDesObj.css("height").replace("px", "") + '" type="image"' + strFloat + ' texttospeech="' + tts + '">';
          strHTML += hotSpot;
          strHTML += '</destinationObject>';

          tempData = tempData.replace(" float: ;", "").replace($partialDesObj.prop('outerHTML'), strHTML);
        }
      });

      // Add partialCredit tag to xml when export
      if ($(tempData).find("partialCredit").length == 0) {
        tempData = tempData.replace('<div>', '<div><partialCredit responseIdentifier="' + currentResId + '" partialID="' + partialID + '"></partialCredit>');
      }

      for (var n = 0; n < iResult[i].correctResponse.length; n++) {
        var crItem = iResult[i].correctResponse[n];
        var crItemOrder = crItem.order;
        var crItemDestIdentifier = crItem.destIdentifier;
        var crItemSrcIdentifier = crItem.srcIdentifier;

        buildItemThreshold = '';

        if (isThresholdGrading === '1' &&
          crItem.thresholdpoints !== undefined &&
          crItem.thresholdpoints.length) {
          for (var its = 0; its < crItem.thresholdpoints.length; its++) {
            var threshold = crItem.thresholdpoints[its];
            buildItemThreshold += '<threshold low="' + threshold.low + '" hi="' + threshold.hi + '" pointsValue="' + threshold.pointsvalue + '"/>';
          }

          buildResponse += '<correctResponse order="' + crItemOrder + '" destIdentifier="' + crItemDestIdentifier + '" srcIdentifier="' + crItemSrcIdentifier + '">';
          buildResponse += buildItemThreshold;
          buildResponse += '</correctResponse>';
        } else {
          buildResponse += '<correctResponse order="' + crItemOrder + '" destIdentifier="' + crItemDestIdentifier + '" srcIdentifier="' + crItemSrcIdentifier + '"/>';
        }
      }

      var isSumCap = typeof iResult[i].responseDeclaration.isSumCap === 'undefined' ? false : iResult[i].responseDeclaration.isSumCap;

      responseDeclaration += [
        '<responseDeclaration identifier="' + iResult[i].responseIdentifier + '"',
        ' absoluteGrading="' + iResult[i].responseDeclaration.absoluteGrading + '"',
        ' absoluteGradingPoints="' + iResult[i].responseDeclaration.absoluteGradingPoints + '"',
        ' partialGradingThreshold="' + iResult[i].responseDeclaration.partialGradingThreshold + '"',
        ' relativeGrading="' + iResult[i].responseDeclaration.relativeGrading + '"',
        ' relativeGradingPoints="' + iResult[i].responseDeclaration.relativeGradingPoints + '"',
        ' thresholdGrading="' + iResult[i].responseDeclaration.thresholdGrading + '"',
        ' algorithmicGrading="' + iResult[i].responseDeclaration.algorithmicGrading + '"',
        ' pointsValue="' + iResult[i].responseDeclaration.pointsValue + '"',
        ' lineMatching="' + iResult[i].responseDeclaration.lineMatching + '"',
        ' anchorPositionObject="' + iResult[i].responseDeclaration.anchorPositionObject + '"',
        ' anchorPositionDestination="' + iResult[i].responseDeclaration.anchorPositionDestination + '"',
        ' isSumCap="' + isSumCap + '">',
        buildResponse,
        '</responseDeclaration>'
      ].join('');

      break;
    }
  }

  //Text Hot Spot
  for (i = 0; i < iResult.length; i++) {
    if (iResult[i].type == "textHotSpot") {
      qtiSchemeID = 31;
      var $markerItem = $(tempData).find(".marker-linkit");

      if ($markerItem.length == 0) {
        alert(errorMsg.noTextHotSpot);
        allowExport = false;
        return "";
      } else if (iResult[i].correctResponse == undefined || iResult[i].correctResponse.length == 0 && iResult[i].responseDeclaration.algorithmicGrading !== '1') {
        alert(errorMsg.noCorrectAnswer);
        allowExport = false;
        return "";
      }

      $markerItem.each(function (index) {
        var $self = $(this);
        var point = 0;
        for (var n = 0; n < iResult[i].source.length; n++) {
          if ($self.attr("hs_id") == iResult[i].source[n].identifier) {
            point = iResult[i].source[n].pointValue;
            break;
          }
        }
        tempData = tempData.replace($self.prop('outerHTML'), '<sourceText identifier="' + $self.attr("hs_id") + '" pointValue="' + point + '">' + $(this).html() + '</sourceText>');
      });
      tempData = tempData.replace(/sourceText> <sourceText/g, "sourceText>&nbsp; <sourceText");
      tempData = tempData.replace(/<sourceText [^>]*>​<\/sourceText>/g, "");
      //Add partialCredit tag to xml when export
      if ($(tempData).find("textHotSpot").length == 0) {
        tempData = tempData.replace('<div>', '<div><textHotSpot id="' + iResult[i].responseIdentifier + '" maxSelected="' + iResult[i].maxSelected + '" responseIdentifier="' + iResult[i].responseIdentifier + '"></textHotSpot>');
      } else {
        tempData = tempData.replace($(tempData).find("textHotSpot").prop("outerHTML"), '<textHotSpot id="' + iResult[i].responseIdentifier + '" maxSelected="' + iResult[i].maxSelected + '" responseIdentifier="' + iResult[i].responseIdentifier + '"></textHotSpot>');
      }

      var buildResponse = "";
      for (var n = 0; n < iResult[i].correctResponse.length; n++) {
        buildResponse += '<correctResponse identifier="' + iResult[i].correctResponse[n].identifier + '" pointValue="' + iResult[i].correctResponse[n].pointValue + '"/>';
      }
      responseDeclaration += '<responseDeclaration identifier="' + iResult[i].responseIdentifier + '" absoluteGrading="' + iResult[i].responseDeclaration.absoluteGrading + '" partialGrading="' + iResult[i].responseDeclaration.partialGrading + '" algorithmicGrading="' + iResult[i].responseDeclaration.algorithmicGrading + '" pointsValue="' + iResult[i].responseDeclaration.pointsValue + '">' + buildResponse + '</responseDeclaration>';

      break;
    }
  }

  //handle image into tag responseRubric for text entry
  if ($(responseDeclaration).find('responseRubric').length) {
    var tagresponseRubric = $(responseDeclaration).find('responseRubric');
    $(tagresponseRubric).find("img").each(function () {
      var htmlImage = $(this).prop("outerHTML");
      var newhtmlImage = htmlImage.replace(/>$/g, "/>");
      responseDeclaration = responseDeclaration.replace(htmlImage, newhtmlImage);
    });
    //Remove url responseDeclaration of image
    var demoLink = new RegExp(GetViewReferenceImg.replace(/\?/g, "\\?"), 'g');
    responseDeclaration = responseDeclaration.replace(demoLink, "");
    responseDeclaration = unreplaceVideo(responseDeclaration);
  }

  //Processing for number line
  $.each(numberLine, function (ind, numberLineItem) {
    var $numberLineItem = $(numberLineItem);
    var $outer = $('<div/>');
    var currentHtml = $numberLineItem.prop('outerHTML');
    currentResId = $numberLineItem.attr('id');
    $numberLineItem.find('svg').wrap('div').appendTo($outer);

    var isFirefox = /firefox/i.test(navigator.userAgent);
    var isSafari = /safari/i.test(navigator.userAgent) && /apple computer/i.test(navigator.vendor);

    if (isFirefox || isSafari) {
      var $rephaelMarker = $outer.find('marker').find('use');
      $rephaelMarker.attr('xmlns:xlink', 'http://www.w3.org/1999/xlink');
    }

    for (var ni = 0, len = iResult.length; ni < len; ni++) {
      var iResultItem = iResult[ni];

      if (iResultItem.responseIdentifier === currentResId && iResultItem.type === 'numberLineHotSpot') {
        var hotspotSourceHtml = '';
        var hotspotHtml = '';
        buildResponse = '';

        qtiSchemeID = 34;

        // Build html correct response
        for (var ci = 0, ciLen = iResultItem.correctResponse.length; ci < ciLen; ci++) {
          var correctResponseItem = iResultItem.correctResponse[ci];
          buildResponse += '<correctResponse identifier="' + correctResponseItem.identifier + '" pointValue="' + correctResponseItem.pointValue + '"/>';
        }

        // Build html number line item
        for (var si = 0, siLen = iResultItem.sourceItem.length; si < siLen; si++) {
          var numberLineChild = iResultItem.sourceItem[si];
          hotspotSourceHtml += '<numberLineItem identifier="' + numberLineChild.identifier + '" pointValue="' + numberLineChild.pointValue + '" left="' + numberLineChild.left + '" top="' + numberLineChild.top + '" correct="' + numberLineChild.correct + '"/>';
        }

        responseDeclaration += '<responseDeclaration identifier="' + iResultItem.responseIdentifier + '" absoluteGrading="' + iResultItem.responseDeclaration.absoluteGrading + '" partialGrading="' + iResultItem.responseDeclaration.partialGrading + '" algorithmicGrading="' + iResultItem.responseDeclaration.algorithmicGrading + '" pointsValue="' + iResultItem.responseDeclaration.pointsValue + '">' + buildResponse + '</responseDeclaration>';

        hotspotHtml += '<numberLine responseIdentifier="' + iResultItem.responseIdentifier + '" width="' + iResultItem.source.width + '" height="' + iResultItem.source.height + '" start="' + iResultItem.source.start + '" end="' + iResultItem.source.end + '" numberDivision="' + iResultItem.source.numberDivision + '" maxhotspot="' + iResultItem.source.maxhotspot + '">';
        hotspotHtml += $outer.html();
        hotspotHtml += hotspotSourceHtml;
        hotspotHtml += '</numberLine>';

        tempData = tempData.replace(currentHtml, hotspotHtml);
        break;
      }
    }
  });

  // Processing for drag and drop numerical
  for (i = 0; i < iResult.length; i++) {
    var iResultItem = iResult[i];

    if (iResultItem.type === 'dragDropNumerical') {
      var correctResponseHtml = '';
      var buildResponseHtml = '';
      var iResultResponsePoint = iResultItem.correctResponse['pointsValue'];
      var iResultResponsePattern = $("<div></div>").append(iResultItem.correctResponse['expressionPattern']).html();
      var destNumerical = [];
      var destIResult = [];
      refreshPartialCredit();
      qtiSchemeID = 35;
      var algorithmicGrading = iResultItem.responseDeclaration.algorithmicGrading === '1' ? '1' : '0';
      var absoluteGrading = '1';

      if (algorithmicGrading === '1') {
        absoluteGrading = '0';
      }

      // Show notification when do not have source or destination
      // or relationship in drag and drop numerical
      var isPartialAddSourceNumerical = !$(tempData).find('.partialAddSourceNumerical').length;
      var isPartialAddDestinationNumerical = !$(tempData).find('.partialAddDestinationNumerical').length;

      if (!isTextToSpeech && (isPartialAddSourceNumerical || isPartialAddDestinationNumerical || !iResultResponsePattern)) {
        var msg = '';
        var msgHtml = '';

        if (isPartialAddSourceNumerical) {
          msg = 'Please add source of drag and drop numerical.';
        } else if (isPartialAddDestinationNumerical) {
          msg = 'Please add destination of drag and drop numerical.';
        } else if (!iResultResponsePattern) {
          msg = 'Please input relationship of drag and drop numerical.';

          if (algorithmicGrading === '1') {
            msg = '';
          }
        }

        if (msg !== '') {
          // Common popup alert message from ckeditor_utils.js
          customAlert(msg);
          return false;
        }
      }

      // Export source numerical
      $(tempData).find('.partialAddSourceNumerical').each(function (ind, dndNumericSrc) {
        var $dndNumericSrc = $(dndNumericSrc);
        var dndNumericSrcIdentifier = $dndNumericSrc.attr('srcIdentifier');
        var dndNumericSrcWidth = $dndNumericSrc.css('width');
        var dndNumericSrcHeight = $dndNumericSrc.css('height');
        var dndNumericSrcValue = $('<div>').text($dndNumericSrc.text()).html();
        var dndNumericSrcLimit = $dndNumericSrc.data('limit');
        var dndNumericSrcOuterHtml = $dndNumericSrc.prop('outerHTML');

        correctResponseHtml += '<source><srcIdentifier>' + dndNumericSrcIdentifier + '</srcIdentifier><value>' + dndNumericSrcValue + '</value></source>';

        tempData = tempData.replace(dndNumericSrcOuterHtml, '<sourceObject style="width: ' + dndNumericSrcWidth + '; height: ' + dndNumericSrcHeight + ';" srcIdentifier="' + dndNumericSrcIdentifier + '" type="text" data-limit="' + dndNumericSrcLimit + '">' + dndNumericSrcValue + '</sourceObject>')
      });

      // Export destination numerical
      $(tempData).find('.partialAddDestinationNumerical').each(function (ind, dndNumericDest) {
        var $dndNumericDest = $(dndNumericDest);
        var dndNumericDestIdentifier = $dndNumericDest.attr('destidentifier');
        var dndNumericDestWidth = $dndNumericDest.css('width').replace('px', '');
        var dndNumericDestHeight = $dndNumericDest.css('height').replace('px', '');
        var dndNumericDestValue = $('<div>').text($dndNumericDest.text()).html();
        var dndNumericDestLimit = $dndNumericDest.attr('numberdroppable');
        var dndNumericDestOuterHtml = $dndNumericDest.prop('outerHTML');

        ind++;

        // Add destination identifier
        destNumerical.push(dndNumericDestIdentifier);

        tempData = tempData.replace(dndNumericDestOuterHtml, '<destinationObject type="text"><destinationItem order="' + ind + '" destIdentifier="' + dndNumericDestIdentifier + '" width="' + dndNumericDestWidth + '" height="' + dndNumericDestHeight + '" numberDroppable="' + dndNumericDestLimit + '">' + dndNumericDestValue + '</destinationItem></destinationObject>');
      });

      //Add partialCredit tag to xml when export
      if (!$(tempData).find('partialCredit').length) {
        tempData = tempData.replace('<div>', '<div><partialCredit responseIdentifier="' + iResultItem.responseIdentifier + '" partialID="' + iResultItem.partialID + '"></partialCredit>');
      }

      // Export response declaration
      iResultResponsePoint = iResultResponsePoint != undefined ? iResultResponsePoint : 1;
      iResultResponsePattern = iResultResponsePattern != undefined ? iResultResponsePattern : '';

      if (iResultResponsePattern != undefined) {
        var destTemp = iResultResponsePattern.split(/[{}]/g);
        var destDiff = [];
        var destDiffLength = 0;

        // Check if pattern have destination
        destIResult = _.filter(destTemp, function (dest) {
          return dest.toLowerCase().indexOf('dest_') !== -1;
        });

        // Check difference destination in relationship and destination in ckeditor
        destDiff = _.union(_.difference(destIResult, destNumerical));

        destDiffLength = destDiff.length;

        // If destination difference exists then notification for user input relationship again
        if (destDiffLength === 1) {
          alert('Relationship do not validate pattern because ' + destDiff.toString() + ' does not exist in ckeditor.');
          return false;
        } else if (destDiffLength > 1) {
          alert('Relationship do not validate pattern because ' + destDiff.toString().split(',').join(' and ') + ' do not exist in ckeditor.');
          return false;
        }
      }

      buildResponseHtml += '<correctResponse><value>' + correctResponseHtml + '</value></correctResponse>';

      responseDeclaration += '<responseDeclaration identifier="' + iResultItem.responseIdentifier + '" absoluteGrading="' + absoluteGrading + '" algorithmicGrading="' + algorithmicGrading + '" pointsValue="' + iResultResponsePoint + '" expressionPattern="' + iResultResponsePattern + '">' + buildResponseHtml + '</responseDeclaration>';
    }
  }

  //Processing for sequence/order
  $.each(sequence, function (ind, sequenceItem) {
    var $sequenceItem = $(sequenceItem);
    var currentResId = $sequenceItem.attr("id");
    var currentHtml = $(this).prop("outerHTML");

    for (var ni = 0, len = iResult.length; ni < len; ni++) {
      var iResultItem = iResult[ni];
      if (iResultItem.responseIdentifier === currentResId && iResultItem.type === 'sequence') {
        qtiSchemeID = 36;
        buildResponse = '';
        var sequenceHtml = '';
        var sequenceHtmlItem = '';
        var algorithmicGrading = iResultItem.responseDeclaration.algorithmicGrading === '1' ? '1' : '0';
        var absoluteGrading = '1';

        if (algorithmicGrading === '1') {
          absoluteGrading = '0';
        }

        // Build html correct response
        buildResponse += '<correctResponse><value>' + iResultItem.correctResponse + '</value></correctResponse>';
        responseDeclaration += '<responseDeclaration identifier="' + iResultItem.responseIdentifier + '" baseType="identifier" absoluteGrading="' + absoluteGrading + '" algorithmicGrading="' + algorithmicGrading + '" pointsValue="' + iResultItem.responseDeclaration.pointsValue + '">' + buildResponse + '</responseDeclaration>';

        // Build html number line item
        sequence.find(".sequence .sequenceItem").each(function () {
          sequenceHtmlItem += '<sourceItem identifier="' + $(this).attr("identifier") + '" width="' + iResultItem.widthItem + '"><div styleName="value">' + $(this).html() + '</div></sourceItem>';
        });

        sequenceHtml += '<partialSequence responseIdentifier="' + iResultItem.responseIdentifier + '" orientation="' + iResultItem.orientation + '">';
        sequenceHtml += sequenceHtmlItem;
        sequenceHtml += '</partialSequence>';

        tempData = tempData.replace(currentHtml, sequenceHtml);
        break;
      }
    }
  });

  //Don't run code if export fasle
  if (allowExport == false) {
    return "";
  }

  //Build refObjectId
  var listObj = $("#listReference li");
  if (listObj.length > 0) {
    listObj.each(function () {
      if ($(this).attr("type") == "objectUrl") {
        refObjectID = '<object class="referenceObject" stylename="referenceObject" type="text/html" data="' + $(this).attr("data") + '" ';

        if ($(this).attr("dataFileUploadTypeID")) {
          refObjectID += ' dataFileUploadTypeID="' + $(this).attr("dataFileUploadTypeID") + '"';
        }

        if ($(this).attr("Qti3pPassageID")) {
          refObjectID += ' Qti3pPassageID="' + $(this).attr("Qti3pPassageID") + '"';
        }

        if ($(this).attr("Qti3pSourceID")) {
          refObjectID += ' Qti3pSourceID="' + $(this).attr("Qti3pSourceID") + '"';
        }

      } else if ($(this).attr("type") == "objectId") {
        refObjectID += '<object class="referenceObject" stylename="referenceObject" refObjectID="' + $(this).attr("data") + '"';
      }
      if ($(this).attr("noshuffle") == "true") {
        refObjectID += ' noshuffle="true"';
      }
      refObjectID += '/>';
    });
  }

  //This case when user doesn't add any question type
  if (qtiSchemeID == 0 && !window.exportXmlWarningDisable) {
    customAlert(errorMsg.addAnswer);
    return "";
  }

  //Set qtiSchemeID for question
  if ($(tempData).find("[responseidentifier]").length > 1) {
    qtiSchemeID = 21;
  }

  if ($("#audioRemoveQuestion").css("display") == "block" && $("#audioRemoveQuestion .audioRef").text() != "") {
    var audioLink = $("#audioQuestion .audioRef").text();
    if (audioLink != null) {
      var idx = audioLink.indexOf('/ItemSet_');
      if (idx > 0) {
        audioLink = audioLink.substring(idx, audioLink.length).replace(/%20/g, ' ');
      }
    }

    questionAudioLink = 'audioRef = "' + audioLink + '" ';
  }

  //remove spanVideo
  $(tempData).find(".videoSpan").each(function () {
    var oldSpanVideo = $(this).prop("outerHTML"),
      newSpanVideo = $(this).html().replace("</videolinkit>Your browser does not support the video tag.", "Your browser does not support the video tag.</videolinkit>");

    tempData = tempData.replace(oldSpanVideo, newSpanVideo);
    tempData = tempData.replace(/<(div|p)[^>]*class=["']audio-mask[^>]*>.*?<\/\1>/g, ""); // remove div mask audio
  });

  //Processing for ol to <list></list>
  var htmlList = '<list listStylePosition="outside" listStyleType="decimal" paragraphSpaceAfter="12" styleName="passageNumbering"><listMarkerFormat><ListMarkerFormat color="#aaaaaa" paragraphEndIndent="20"/></listMarkerFormat>';
  var htmlEndList = '</list>'; //Make sure when export to html it will not has break line of textare
  tempData = tempData.replace(/<ol>/g, htmlList).replace(/<\/ol>/g, htmlEndList);

  tempData = tempData.replace(/ol style=/g, 'list style=');
  tempData = tempData.replace(/ol start=/g, 'list start=');

  tempData = escapeBasicStyles(tempData);

  //Processing for sub
  $(tempData).find("sub").each(function () {
    var oldSub = $(this).prop("outerHTML"),
      newSub = '<span styleName="sub" class="sub">' + $(this).html() + "</span>";
    tempData = tempData.replace(oldSub, newSub);
  });

  //Processing for sup
  $(tempData).find("sup").each(function () {
    var oldSup = $(this).prop("outerHTML"),
      newSup = '<span styleName="sup" class="sup">' + $(this).html() + "</span>";
    tempData = tempData.replace(oldSup, newSup);
  });

  //Processing for ol to <list></list>
  $(tempData).find("ol").each(function () {
    var htmlList = '<list listStylePosition="outside" listStyleType="decimal" paragraphSpaceAfter="12" styleName="passageNumbering"><listMarkerFormat><ListMarkerFormat color="#aaaaaa" paragraphEndIndent="20"/></listMarkerFormat>';
    var htmlEndList = '</list>'; //Make sure when export to html it will not has break line of textare
    tempData = tempData.replace("<ol>", htmlList).replace("</ol>", htmlEndList);
  });

  //add styleName for text-align
  $(tempData).find(".center").each(function () {
    var oldCenter = $(this).prop("outerHTML").replace(/stylename="/g, 'styleName="'),
      newCenter = $(this).attr("styleName", "center").prop("outerHTML").replace('stylename="center"', 'styleName="center"');
    tempData = tempData.replace(oldCenter, newCenter);
  });

  $(tempData).find(".alignRight").each(function () {
    var oldRight = $(this).prop("outerHTML").replace(/stylename="/g, 'styleName="'),
      newRight = $(this).attr("styleName", "alignRight").prop("outerHTML").replace('stylename="alignRight"', 'styleName="alignRight"');
    tempData = tempData.replace(oldRight, newRight);
  });

  //remove span speChar
  $(tempData).find(".speChar").each(function () {
    //no remove tag span has white space specical
    if ($(this).html() !== "&nbsp;") {
      var oldCenter = $(this).prop("outerHTML"),
        newCenter = $(this).text();
      tempData = tempData.replace(oldCenter, newCenter);
    }
  });

  //Remove all <br /> tag
  tempData = tempData.replace(/<br>/g, '<br />');

  //Make sure image has close tag
  $(tempData).find("img").each(function () {
    var current_image = $(this).clone();

    if (current_image.attr("data-latex") !== undefined) {
      var data_latex = convertTexttoHTML(current_image.attr("data-latex"));
      current_image.attr({
        "data-latex": data_latex
      });
    }

    var htmlImage = $(this).prop("outerHTML"),
      newhtmlImage = current_image.prop("outerHTML").replace(/>$/, " />");
    newhtmlImage = newhtmlImage.replace('contenteditable="false" ', '');

    if ($(this).css("float") == "left" || $(this).css("float") == "right") {
      if ($(newhtmlImage).attr("float") != undefined) {
        newhtmlImage = newhtmlImage.replace("float: " + $(this).css("float") + ";", "").replace(new RegExp('float="' + $(newhtmlImage).attr("float") + '"', "g"), '').replace("/>", ' float="' + $(this).css("float") + '" />');
      } else {
        newhtmlImage = newhtmlImage.replace("float: " + $(this).css("float") + ";", "").replace("/>", ' float="' + $(this).css("float") + '" />');
      }
    }

    tempData = tempData.replace(htmlImage, newhtmlImage);
  });

  //Make sure only relative link /ItemSet_..... of image is kept
  $(tempData).find("img").each(function () {
    var htmlImage = $(this).prop("outerHTML"),
      newhtmlImage = htmlImage.replace(/>$/, "/>");
    var src = $(newhtmlImage).attr("src");
    if (src != null) {
      var idx = src.indexOf('/ItemSet_');
      if (idx > 0) {
        var newSrc = src.substring(idx, src.length).replace(/%20/g, ' ');
        tempData = tempData.replace(src, newSrc);
      }
    }
  });
  //Make sure only relative link /ItemSet_..... of audio is kept
  $(tempData).find('[audioRef]').each(function () {
    var audioRef = $(this).attr("audioRef");
    if (audioRef != null) {
      var idx = audioRef.indexOf('/ItemSet_');
      if (idx > 0) {
        var newAudioRef = audioRef.substring(idx, audioRef.length).replace(/%20/g, ' ');
        tempData = tempData.replace(audioRef, newAudioRef);
      }
    }
  });
  // Remove videoSpan class wrapper video
  $(tempData).find('.videoSpan').each(function () {
    var $videoSpan = $(this);
    var oldSpanVideo = $videoSpan.prop('outerHTML');
    var newSpanVideo = $videoSpan.find('videolinkit').prop('outerHTML');

    if ($.browser.msie && parseInt($.browser.version, 10) < 10) {
      oldSpanVideo = oldSpanVideo.replace(/contentEditable=\"false\"/g, 'contenteditable="false"');
      newSpanVideo = newSpanVideo.replace(/contentEditable=\"false\"/g, 'contenteditable="false"');
    }
  });
  //Fix for IE9
  if ($.browser.msie && parseInt($.browser.version, 10) < 10) {
    tempData = tempData.replace('<?XML:NAMESPACE PREFIX = [default] http://www.imsglobal.org/xsd/imsqti_v2p0 NS = "http://www.imsglobal.org/xsd/imsqti_v2p0" />', '').replace('<?XML:NAMESPACE PREFIX = [default] http://www.w3.org/1998/Math/MathML NS = "http://www.w3.org/1998/Math/MathML" />', '');
    tempData = tempData.replace('<?XML:NAMESPACE PREFIX = "[default] http://www.w3.org/1998/Math/MathML" NS = "http://www.w3.org/1998/Math/MathML" />', '');
    tempData = tempData.replace('<?XML:NAMESPACE PREFIX = [default] http://www.w3.org/1998/Math/MathML NS = "http://www.w3.org/1998/Math/MathML" />', '');
  }

  tempData = unreplaceVideo(tempData);
  tempData = tempData.replace(/"\>Your browser does not support the video tag./g, '" />Your browser does not support the video tag.');

  //Remove temporally div element
  mainBody = tempData.replace("<div>", "").replace(/<\/div>$/, "");

  // Save text to speech to xmlContent
  var texttospeech = iResultComponent.texttospeech;
  var texttospeechEnable = texttospeech === undefined ? true : texttospeech.enable;
  var texttospeechRate = texttospeech === undefined ? '0.8' : texttospeech.rate;
  var texttospeechVolume = texttospeech === undefined ? '0.5' : texttospeech.volume;

  var attachmentSettingXml = createAttachmentConfigurationXml(TestMakerComponent.attachmentSetting);


  var allOrNothingGradingAttr = allOrNothingGradingScore != null
    ? ' allOrNothingGrading="' + allOrNothingGradingScore + '"'
    : '';
  // set multiPartGradingSetting for multi-part question
  var multiPartGradingSetting = qtiSchemeID === 21 ? '<multiPartGradingSetting mode="' + modeMultiPartGrading + '"' + allOrNothingGradingAttr + '/>' : '';

  //handle responseDeclaration for all or nothing grading
  if (responseDeclaration.length && allOrNothingGradingScore != null) {
    var xmlString = '<root>' + responseDeclaration + '</root>'
    var parser = new DOMParser();
    var xmlDoc = parser.parseFromString(xmlString, "text/xml");
    var responseDeclarationsXML = xmlDoc.getElementsByTagName("responseDeclaration");
    for (var i = 0; i < responseDeclarationsXML.length; i++) {
      responseDeclarationsXML[i].setAttribute("allOrNothingGrading", allOrNothingGradingScore);
    }
    var serializer = new XMLSerializer();
    var newXmlString = serializer.serializeToString(xmlDoc);
    responseDeclaration = newXmlString.replace(/^<root>/, '').replace(/<\/root>$/, '');
  }

  var xmlResult = '<assessmentItem xsi:schemaLocation="http://www.imsglobal.org/xsd/imsqti_v2p0 imsqti_v2p0.xsd"';

  var identifier = isAddnew ? '' : $($('#txtXmlContent').val()).attr('identifier');
  if (identifier) {
    xmlResult += ' identifier="' + identifier + '"';
  }

  xmlResult += ' adaptive="false" timeDependent="false" xmlUnicode="true" toolName="linkitTLF" toolVersion="2.0" qtiSchemeID="' + qtiSchemeID + '" texttospeechrate="' + texttospeechRate + '" texttospeechvolume="' + texttospeechVolume + '" texttospeechenable="' + texttospeechEnable + '" xmlns="http://www.imsglobal.org/xsd/imsqti_v2p0" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">';
  xmlResult += '<stylesheet href="stylesheet/linkitStyleSheet.css" type="text/css"/>';
  xmlResult += responseDeclaration;
  xmlResult += '<itemBody ' + questionAudioLink + '>';
  xmlResult += refObjectID;
  xmlResult += '<div class="mainBody" styleName="mainBody">';
  xmlResult += mainBody;
  xmlResult += '</div>';
  xmlResult += '</itemBody>';
  xmlResult += attachmentSettingXml;
  xmlResult += multiPartGradingSetting;
  xmlResult += '</assessmentItem>';

  /*
      - change all "stylename" to "styleName"
      - Remove break line
      - Un-use span
      - Instead of <br> to <br />
      - Remove "zero width spaces"
  */

  xmlResult = xmlResult.replace(/(\r\n|\n|\r)/gm, "")
    .replace(new RegExp("\>[\n\t]+\<", "g"), "><")
    .replace(/<span><\/span>/g, '')
    .replace(/<p><\/p>/g, '<p><span/></p>')
    .replace(/<p> <\/p>/g, '<p><span/></p>')
    .replace(/<p>&nbsp;<\/p>/g, '<br />')
    .replace(/stylename="/g, 'styleName="')
    .replace(/&nbsp;/g, "&#160;")
    .replace(/<br>/g, '<br />')
    .replace(/[\u200B-\u200D\uFEFF]/g, '');

  //Reset status for CKEditor
  CKEDITOR.instances[ckID].resetDirty();

  xmlResult = xmlResult
    .replace(
      /<source ([^>]*?)>\s*<\/source>/gi,
      function (match, attributes) {
        return '<source ' + attributes.trim() + ' />';
      }
    ).replace(
      /<source ([^>]*?)>/gi,
      function (match, attributes) {
        if (attributes.trim().endsWith('/')) {
          return match;
        }
        return '<source ' + attributes.trim() + ' />';
      }
    );

  if (checkSavingComplex) {
    //Show alert when user select complex type but on xmlContent export is difference
    if (iSchemeID == "21" && qtiSchemeID != "21") {
      if (window.exportXmlWarningDisable) {
        return xmlResult;
      }
      var r = confirm(errorMsg.notComplex);
      if (r == true) {
        return xmlResult;
      } else {
        return "";
      }
    } else {
      return xmlResult;
    }
  } else {
    return xmlResult;
  }
};

/***
 * Import xml data to mkEditor
 ***/
function xmlImport(xmlContent, schemeID, pointsValueAlgorithmic) {
  IS_V2 && setModeMultiPartGrading(xmlContent, schemeID)
  //Get SchemeID to show correct question type
  xmlContent = xmlContent.replace('<?xml version="1.0"?>', "");

  xmlContent = replaceVideo(xmlContent);
  xmlContent = replaceParagraph(xmlContent);

  iSchemeID = schemeID;

  isAddnew = false;

  //When import xmlContent we need to check for draw tool because drawtool and extended text has same schemaID = 10
  if (iSchemeID == "10") {
    if ($("<div>" + xmlContent + "</div>").find('extendedtextinteraction[drawable="true"]').length > 0) {
      iSchemeID = "10d";
    }
  }

  loadEditItem(iSchemeID, isAddnew);

  xmlContent = xmlContent.replace('<?xml version="1.0"?>', '');
  // Remove Old XML content "active-border" class
  xmlContent = xmlContent.replace(/class="active-border"/g, '');
  // Processing for ol to <list></list> Must be replace before convert xml to html by jquery
  // because tag name will be change to lowercase
  xmlContent = replaceListTool(xmlContent);

  // Replace tableitem by span
  xmlContent = xmlContent.replace(/tableitem/g, 'span');

  xmlContent = correctInlineChoice(xmlContent);

  xmlContent = $(xmlContent).prop("outerHTML");

  if ($.browser.msie && parseInt($.browser.version, 10) < 10) {
    xmlContent = xmlContent.replace('<?XML:NAMESPACE PREFIX = \"[default] http://www.imsglobal.org/xsd/imsqti_v2p0\" NS = \"http://www.imsglobal.org/xsd/imsqti_v2p0\" />', '');
    xmlContent = xmlContent.replace(/contentEditable=\"false\"/g, 'contenteditable="false"');
  }

  //Create iResult
  iResult = [];

  //Create depending
  currData = [];

  //Make sure Multiple choice html belong to div.mainBody
  var lastChoiceIndex = xmlContent.lastIndexOf("</div><choiceinteraction");
  if (lastChoiceIndex != -1) {
    xmlContent = xmlContent.substring(0, lastChoiceIndex) + "</div><choiceinteraction" + xmlContent.substring(lastChoiceIndex + 24, xmlContent.length);

    var lastChIndex = xmlContent.lastIndexOf("</choiceinteraction>");
    //Make sure inlinechoice found
    if (lastChIndex != -1) {
      xmlContent = xmlContent.substring(0, lastChIndex) + "</choiceinteraction></div>" + xmlContent.substring(lastChIndex + 20, xmlContent.length);
    }
  }

  //Processing for sameline
  $(xmlContent).find("span.nobreak").each(function () {
    var $this = $(this);
    var oldSameline = $this.prop("outerHTML");
    var buildSameline = $(this).html();

    var newContent = eleExportbyClass(oldSameline, 'nobreak');
    var newSameline = '<sameline class="nobreak" stylename="nobreak">' + newContent + '</sameline>';
    xmlContent = xmlContent.replace(oldSameline, newSameline);
  });
  // Process Video Upload
  $(xmlContent).find(".video").each(function () {
    var _video = $(this).html();
    var newvideo = $(this).prepend('<input type="button" value="Edit video" class="editvideo"/>').html();
    xmlContent = xmlContent.replace(_video, newvideo);
  });
   // Process Audio Upload
   $(xmlContent).find("audio.editvideo").each(function () {
    var audio = $(this).prop("outerHTML");
    var $mask =
      '<div class="audio-mask top"></div>' +
      '<div class="audio-mask bottom"></div>' +
      '<div class="audio-mask left"></div>' +
      '<div class="audio-mask right"></div>';
    var newAudio = '<div contenteditable="false" class="videoSpan audio-container">' + audio + $mask + "</div>";
    xmlContent = xmlContent.replace(audio, newAudio);
  });

  //Processing image url
  $(xmlContent).find('img').each(function (ind, image) {
    var $image = $(image);
    var imageSrc = $image.attr('src') || '';
    var imageOriginal = $image.prop('outerHTML');
    var $newImage;

    // This is check incase image doesn't has src but it has source attribute
    if (imageSrc === '') {
      var imageSource = $image.attr('source');

      if (imageSource != null && imageSource !== '') {
        imageSrc = imageSource;
      }
    }

    if (imageSrc.indexOf('http') > -1 && imageSrc.indexOf('data:image') > -1) {
      imageSrc = GetViewReferenceImg + imageSrc;
    }

    $newImage = $image.attr('src', imageSrc).addClass('imageupload');

    if ($image.attr('drawable') == null) {
      $newImage = $newImage.attr('drawable', false);
    }

    if ($image.attr('percent') == null) {
      $newImage = $newImage.attr('percent', 10);
    }

    if ($image.attr('float') != null) {
      $newImage = $newImage.css('float', $image.attr('float'));
    }

    xmlContent = xmlContent.replace(imageOriginal, $newImage.prop('outerHTML'));
  });

  $(xmlContent).find("table").each(function () {
    var table = $(this);
    var audioSrc = table.get(0).getAttribute('audiotableref');
    var audioId = table.get(0).getAttribute('audioid');

    if (audioSrc && audioSrc != '' && audioId) {
      var tableAudio = '<img audioid="' + audioId + '" class="audioTable bntPlay" audiosrc="' + audioSrc + '" src="../../Content/themes/TestMaker/images/small_audio_play.png">';
      var newTable = tableAudio + table.get(0).outerHTML.substring(0, table.get(0).outerHTML.indexOf('<tbody>'));

      xmlContent = xmlContent.replace(table.get(0).outerHTML.substring(0, table.get(0).outerHTML.indexOf('<tbody>')), newTable);
    }
  });

  xmlContent = escapeBasicStylesFromXml(xmlContent);

  //Processing for sub
  $(xmlContent).find(".sub").each(function () {
    var oldSub = $(this).prop("outerHTML"),
      subClass = $(this).attr("class").replace(/sub/g, ""),
      oldSubClass = "";

    if (subClass != "") {
      oldSubClass = ' class="' + $(this).attr("class").replace(/sub/g, "") + '"';
    }

    var newContent = eleExportbyClass(oldSub, 'sub');
    var newSub = "<sub" + oldSubClass + ">" + newContent + "</sub>";
    xmlContent = xmlContent.replace(oldSub, newSub);
  });

  //Processing for sup
  $(xmlContent).find(".sup").each(function () {
    var oldSup = $(this).prop("outerHTML"),
      supClass = $(this).attr("class").replace(/sup/g, ""),
      oldSupClass = "";

    if (supClass != "") {
      oldSupClass = ' class="' + supClass + '"';
    }

    var newContent = eleExportbyClass(oldSup, 'sup');
    var newSup = "<sup" + oldSupClass + ">" + newContent + "</sup>";
    xmlContent = xmlContent.replace(oldSup, newSup);
  });

  // Adding spanVideo class to item cannot editable
  $(xmlContent).find("videolinkit").each(function () {
    var oldSpanVideo = $(this).prop("outerHTML").replace('<?XML:NAMESPACE PREFIX = "[default] http://www.imsglobal.org/xsd/imsqti_v2p0" NS = "http://www.imsglobal.org/xsd/imsqti_v2p0" />', ""),
      newSpanVideo = '<span class="videoSpan" contenteditable="false">' + oldSpanVideo + '</span>';

    xmlContent = xmlContent.replace(oldSpanVideo, newSpanVideo);
  });

  //Processing for audio in question
  var qAudioRef = $(xmlContent).find("itembody").attr("audioref");
  if (qAudioRef != undefined && qAudioRef != "") {
    $("#addAudioQuestion").hide();
    $("#audioRemoveQuestion").show().find(".audioRef").text(qAudioRef);
  }

  //Processing for textEntry
  $(xmlContent).find("textEntryInteraction").each(function () {
    var $textentry = $(this);
    var resId = $(this).attr("responseIdentifier"),
      expLengthMax = $(this).attr("expectedLengthMax"),
      expLengthMin = $(this).attr("expectedLengthMin"),
      expectedWidth = $(this).attr('expectedWidth') || 100,
      strTextEntry = $(this).prop("outerHTML");
    addPadding = $(this).attr('addPadding');
    validation = $(this).attr('validation');
    if (!expLengthMax) {
      expLengthMax = $(this).attr("expectedLength");
      expLengthMin = 0;
    }
    var customRule = '';
    if (validation === '4') {
      customRule = $(this).attr('customRule');
    }
    var hastypeMessageGuidance = '';

    //Fix for IE9
    if ($.browser.msie && parseInt($.browser.version, 10) < 10) {
      strTextEntry = xmlContentForIE(strTextEntry);
      strTextEntry = strTextEntry.replace(/contentEditable=\"false\"/g, 'contenteditable="false"');
    }

    $(xmlContent).find("responseDeclaration").each(function () {
      if (resId == $(this).attr("identifier")) {
        //check for range value
        var attrRange = $(this).attr("range");
        var isRange = "false";
        var tagGuidanceValues, tagRationaleValues;

        if (attrRange != undefined) {
          isRange = $(this).attr("range").toString().toLowerCase();
        }

        var correntRes = [];
        if (isRange == "true") {
          $(this).find("rangevalue").each(function () {
            correntRes.push({
              name: $(this).find("name").text(),
              valueType: $(this).find("valuetype").text(),
              valueRange: $(this).find("value").text(),
              exclusivity: $(this).find("exclusivity").text()
            });
          });
        } else {
          $(this).find("correctResponse value").each(function () {
            correntRes.push({
              identifier: $(this).attr('identifier'),
              value: $(this).html(),
              pointsValue: $(this).attr("pointsValue"),
              arrMessageGuidance: []
            });
          });

          //aaply for guidance and rationale
          for (var i = 0, lenCorrentRes = correntRes.length; i < lenCorrentRes; i++) {
            var itemMessageGuidance = correntRes[i].arrMessageGuidance;

            if ($(this).find('responseRubric').length) {
              hastypeMessageGuidance = 'hasGuidance';
              tagGuidanceValues = $(this).find('responseRubric value');
              for (var k = 0, lenTagGuidanceValues = tagGuidanceValues.length; k < lenTagGuidanceValues; k++) {
                var ansIdentifier = $(tagGuidanceValues[k]).attr('ansIdentifier');
                if (correntRes[i].identifier === ansIdentifier) {
                  itemMessageGuidance.push({
                    typeMessage: $(tagGuidanceValues[k]).attr('type'),
                    audioRef: $(tagGuidanceValues[k]).attr('audioRef'),
                    valueContent: $(tagGuidanceValues[k]).html()
                  });
                }
              }
            }
          }
        }

        var myResult = {
          type: "textEntryInteraction",
          responseIdentifier: resId,
          expectedLengthMax: expLengthMax,
          expectedLengthMin: expLengthMin,
          expectedWidth: expectedWidth,
          validation: validation,
          addPadding: addPadding,
          customRule: customRule,
          responseDeclaration: {
            baseType: $(this).attr("baseType"),
            cardinality: $(this).attr("cardinality"),
            method: $(this).attr("method"),
            caseSensitive: $(this).attr("caseSensitive"),
            pointsValue: pointsValueAlgorithmic != null ? pointsValueAlgorithmic : $(this).attr("pointsValue"),
            spelling: (typeof $(this).attr("spelling") === 'undefined') ? false : $(this).attr("spelling"),
            spellingDeduction: (typeof $(this).attr("spellingDeduction") === 'undefined') ? 0 : $(this).attr("spellingDeduction"),
            ignoreExtraSpace: (typeof $(this).attr("ignoreExtraSpace") === 'undefined') ? 0 : $(this).attr("ignoreExtraSpace"),
            type: $(this).attr("type"),
            range: isRange
          },
          correctResponse: correntRes
        };

        //Processing to import major and depending
        createImportMajorDepending(this);

        iResult.push(myResult);

        // Build html text entry content append to ckeditor
        var strTextEntryMask = '<img style="padding-left: 3px; padding-top: 0.5px;display: none;" alt="Guidance" class="imageupload bntGuidance" src="../../Content/themes/TestMaker/images/guidance_checked_small.png" title="Guidance"><img class="cke_reset cke_widget_mask textEntryInteractionMark" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" />';
        var strTextEntryReplace = '';
        if (typeof $textentry.attr('data-top') !== 'undefined' ||
          typeof $textentry.attr('data-left') !== 'undefined') {
          var textentryTop = $textentry.attr('data-top');
          var textentryLeft = $textentry.attr('data-left');

          strTextEntryReplace += '<span style="max-width:'+expectedWidth +'px; margin-right: 0.5px" id="' + resId + '" title="' + resId + '" class="textEntryInteraction" contenteditable="false"';
          strTextEntryReplace += ' data-top="' + textentryTop + '"'
          strTextEntryReplace += ' data-left="' + textentryLeft + '"'
          strTextEntryReplace += ' style="position: absolute;'
          strTextEntryReplace += 'top: ' + textentryTop + 'px;';
          strTextEntryReplace += 'left: ' + textentryLeft + 'px;';
          strTextEntryReplace += '"';
          strTextEntryReplace += '>';
          strTextEntryReplace += strTextEntryMask;
          strTextEntryReplace += '</span>\u200B'
        } else {
          strTextEntryReplace += '<span style="max-width:'+expectedWidth +'px; margin-right: 0.5px" id="' + resId + '" title="' + resId + '" class="textEntryInteraction" contenteditable="false">';
          strTextEntryReplace += strTextEntryMask;
          strTextEntryReplace += '</span>\u200B'
        }
        xmlContent = xmlContent.replace(strTextEntry, strTextEntryReplace);

        return false;
      }
    });
  });

  //Processing for inlineChoice
  $(xmlContent).find("inlineChoiceInteraction").each(function () {
    var $inlinechoice = $(this);
    var resId = $(this).attr("responseIdentifier"),
      shuffle = $(this).attr("shuffle"),
      inlineChoice = [],
      expectedWidth = $(this).attr('expectedWidth') || 200,
      visibleDimension = $(this).attr('visibleDimension'),
      strInlineChoice = $(this).prop("outerHTML");
    var typeGuidance = '';
    var typeRationale = '';
    var typeGuidanceRationale = '';
    var arrMessageGuidance = [];
    var hastypeMessageGuidance = '';

    //Fix for IE9
    if ($.browser.msie && parseInt($.browser.version, 10) < 10) {
      strInlineChoice = xmlContentForIE(strInlineChoice);
      strInlineChoice = strInlineChoice.replace(/contentEditable=\"false\"/g, 'contenteditable="false"');
    }

    $(this).find("inlineChoice").each(function () {
      //apply data xml guidance into ckeditor
      if ($(this).find('.guidance').length) {
        typeGuidance = putDataIntoIResult(arrMessageGuidance, this, 'guidance');
        hastypeMessageGuidance = 'hasGuidance';
      } //end apply guidance

      if ($(this).find('.rationale').length) {
        typeRationale = putDataIntoIResult(arrMessageGuidance, this, 'rationale');
        hastypeMessageGuidance = 'hasGuidance';
      } //end apply rationale

      if ($(this).find('.guidance_rationale').length) {
        typeGuidanceRationale = putDataIntoIResult(arrMessageGuidance, this, 'guidance_rationale');
        hastypeMessageGuidance = 'hasGuidance';
      } //end apply guidance rationale

      if ($(this).find('.inlineChoiceAnswer').length) {
        inlineChoice.push({
          identifier: $(this).attr("identifier"),
          value: $(this).find('.inlineChoiceAnswer').html(),
          pointsValue: $(this).attr("pointsValue"),
          arrMessageGuidance: arrMessageGuidance,
          expectedWidth: expectedWidth,
          visibleDimension: visibleDimension,
        });
      } else {
        inlineChoice.push({
          identifier: $(this).attr("identifier"),
          value: $(this).html(),
          pointsValue: $(this).attr("pointsValue"),
          arrMessageGuidance: arrMessageGuidance,
          expectedWidth: expectedWidth,
          visibleDimension: visibleDimension,
        });
      }

      //reset array Message Guidance
      arrMessageGuidance = [];
    });

    $(xmlContent).find("responseDeclaration").each(function () {
      if (resId == $(this).attr("identifier")) {
        var correntRes = "";
        $(this).find("value").each(function () {
          if (correntRes != "") {
            correntRes += ",";
          }
          correntRes += $(this).html();
        });

        for (var i = 0; i < inlineChoice.length; i++) {
          if (correntRes == inlineChoice[i].identifier) {
            inlineChoice[i].answerCorrect = true;
          }
        }

        var myResult = {
          type: "inlineChoiceInteraction",
          responseIdentifier: resId,
          shuffle: shuffle,
          responseDeclaration: {
            baseType: $(this).attr("baseType"),
            cardinality: $(this).attr("cardinality"),
            method: $(this).attr("method"),
            caseSensitive: $(this).attr("caseSensitive"),
            pointsValue: pointsValueAlgorithmic != null ? pointsValueAlgorithmic : $(this).attr("pointsValue"),
            type: $(this).attr("type")
          },
          inlineChoice: inlineChoice,
          correctResponse: correntRes,
          expectedWidth: expectedWidth,
          visibleDimension: visibleDimension,
        };

        //Processing to import major and depending
        createImportMajorDepending(this);

        iResult.push(myResult);

        // Build html inline choice content append to ckeditor
        var strInlineChoiceMask = '<img style="padding-left: 3px; padding-top: 0.5px;display: none;" alt="Guidance" class="imageupload bntGuidance" src="../../Content/themes/TestMaker/images/guidance_checked_small.png" title="Guidance"><img class="cke_reset cke_widget_mask inlineChoiceInteractionMark" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" />';
        var strInlineChoiceReplace = '';

        var _expectedWidth = expectedWidth;
        if (visibleDimension != 1) _expectedWidth = 250;

        if (typeof $inlinechoice.attr('data-top') !== 'undefined' ||
          typeof $inlinechoice.attr('data-left') !== 'undefined') {
          var inlinechoiceTop = $inlinechoice.attr('data-top');
          var inlinechoiceLeft = $inlinechoice.attr('data-left');

            strInlineChoiceReplace += '<span style="margin-right: 0.5px; max-width:' + _expectedWidth + 'px" id="' + resId + '" title="' + resId + '" class="inlineChoiceInteraction" contenteditable="false"';
          strInlineChoiceReplace += ' data-top="' + inlinechoiceTop + '"'
          strInlineChoiceReplace += ' data-left="' + inlinechoiceLeft + '"'
          strInlineChoiceReplace += ' style="position: absolute;'
          strInlineChoiceReplace += 'top: ' + inlinechoiceTop + 'px;';
          strInlineChoiceReplace += 'left: ' + inlinechoiceLeft + 'px;';
          strInlineChoiceReplace += '"';
          strInlineChoiceReplace += '>';
          strInlineChoiceReplace += strInlineChoiceMask;
          strInlineChoiceReplace += '</span>\u200B'
        } else {
            strInlineChoiceReplace += '<span style="margin-right: 0.5px; max-width:' + _expectedWidth + 'px" id="' + resId + '" title="' + resId + '" class="inlineChoiceInteraction" contenteditable="false">';
          strInlineChoiceReplace += strInlineChoiceMask;
          strInlineChoiceReplace += '</span>\u200B'
        }

        xmlContent = xmlContent.replace(strInlineChoice, strInlineChoiceReplace);

        return false;
      }
    });
  });

  var getImgByVersion = CKEDITOR.plugins.getImgByVersion;

  //Processing for extended Text Draw
  $(xmlContent).find("extendedTextInteraction").each(function () {
    //This case for Drawable

    var extendText = $(this);
    var extendedtextDrawable = extendText.attr('drawable');
    var placeholderText = extendText.attr('placeholderText') ? extendText.attr('placeholderText') :'Placeholder to display text area.';
    var extendedtextMask = '<img class="cke_reset cke_widget_mask extentTextInteractionMark" data-cke-saved-src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D">';
    var placeholderValue = extendText.attr('placeholderText') || '';
    if (extendedtextDrawable != null && extendedtextDrawable == "true") {
      var resId = extendText.attr('responseIdentifier');
      var iDrawable = extendText.attr('drawable');
      var iWidth = extendText.attr('width');
      var iHeight = extendText.attr('height');
      var gridSize = extendText.attr('gridSize');
      var srcImage = extendText.find('img').attr("src");
      var percent = extendText.find('img').attr('percent');
      var drawingTextToSpeech = extendText.find('img').attr('texttospeech');
      var drawingDataType = extendText.data('type') === 'basic' ? extendText.data('type') : 'free-formatted';
      var drawingDataWidth = extendText.data('width') != null ? extendText.data('width') : 600;
      var drawingDataHeight = extendText.data('height') != null ? extendText.data('height') : 600;

      drawingTextToSpeech = drawingTextToSpeech == null ? '' : drawingTextToSpeech;

      var wOrgImage = extendText.find('img').attr("worgimage");
      var hOrgImage = extendText.find('img').attr("horgimage");

      $(xmlContent).find("responseDeclaration").each(function () {
        if (resId == $(this).attr("identifier")) {
          var myResult = {
            type: "extendedTextInteractionDraw",
            responseIdentifier: resId,
            wOrgImage: wOrgImage,
            hOrgImage: hOrgImage,
            width: iWidth,
            height: iHeight,
            srcImage: srcImage,
            responseDeclaration: {
              baseType: $(this).attr("baseType"),
              cardinality: $(this).attr("cardinality"),
              method: $(this).attr("method"),
              caseSensitive: $(this).attr("caseSensitive"),
              pointsValue: $(this).attr("pointsValue"),
              type: $(this).attr("type")
            },
            drawable: iDrawable,
            percent: percent,
            gridSize: gridSize,
            dataType: drawingDataType,
            dataWidth: drawingDataWidth,
            dataHeight: drawingDataHeight
          };

          //Processing to import major and depending
          createImportMajorDepending(this);

          iResult.push(myResult);

          /*This case for new test maker the draw tool:
           * Length = 1: It will replace drawtool same place
           * Length = 0: Draw tool will be add on the bottom of test content
           */
          if (extendText.parents(".mainBody").length == 1) {
            //Fix for IE9
            var strExtendText = extendText.prop("outerHTML");
            if ($.browser.msie && parseInt($.browser.version, 10) < 10) {
              strExtendText = xmlContentForIE(strExtendText);
            }
            if (srcImage != '' && srcImage != defaultSrc) {
              xmlContent = xmlContent.replace(strExtendText, '<p id="' + resId + '" title="' + resId + '" style="width:' + iWidth + 'px; height: ' + iHeight + 'px" class="drawTool" contenteditable="false"><img style="width:' + iWidth + 'px; height: ' + iHeight + 'px" class="cke_reset cke_widget_mask imageDrawTool" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" /><img percent="' + percent + '" src="' + srcImage + '" width="' + iWidth + '" height="' + iHeight + '" alt="" class="imageDrawTool" style="position: absolute; border: none;" texttospeech="' + drawingTextToSpeech + '" />&nbsp;</p>');
            } else {
              xmlContent = xmlContent.replace(strExtendText, '<p id="' + resId + '" title="' + resId + '" style="width:' + iWidth + 'px; height: ' + iHeight + 'px" class="drawTool" contenteditable="false"><img style="width:' + iWidth + 'px; height: ' + iHeight + 'px" class="cke_reset cke_widget_mask imageDrawTool" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" />&nbsp;</p>');
            }
          } else {
            var lastIndex = xmlContent.lastIndexOf("</div>");
            xmlContent = xmlContent.substring(0, lastIndex) + '<p id="' + resId + '" title="' + resId + '" style="width:' + iWidth + 'px; height: ' + iHeight + 'px" class="drawTool" contenteditable="false"><img style="width:' + iWidth + 'px; height: ' + iHeight + 'px" class="cke_reset cke_widget_mask imageDrawTool" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" /><img percent="' + percent + '" src="' + srcImage + '" width="' + iWidth + '" height="' + iHeight + '" alt="" class="imageDrawTool" style="position: absolute; border: none;" texttospeech="' + drawingTextToSpeech + '" />&nbsp;</p>' + xmlContent.substring(lastIndex, xmlContent.length);
          }

          return false;
        }
      });
    } else if (extendedtextDrawable == null || extendedtextDrawable == "false") {
      // This case for extended Text
      var resId = extendText.attr("responseIdentifier"),
        iexpLength = extendText.attr("expectedLength");
      var formatText = extendText.attr('formatText');
      $(xmlContent).find("responseDeclaration").each(function () {
        if (resId == $(this).attr("identifier")) {
          var iPoint = $(this).attr("pointsValue");

          if (iPoint === undefined) {
            if ($("#hdPointsPossible").length > 0) {
              iPoint = $("#hdPointsPossible").val();
            }
          }

          //This is case check for old xml without point in responseDeclaration
          if (iPoint == undefined) {
            var outerValue = $(xmlContent).find("outcomedeclaration defaultvalue value");
            if (outerValue.length != 0) {
              iPoint = outerValue.text();
            } else {
              iPoint = 1;
            }
          }

          var myResult = {
            type: "extendedTextInteraction",
            responseIdentifier: resId,
            expectedLength: iexpLength,
            formatText: formatText === 'false' ? false : true,
            placeholderText: placeholderValue || '',
            responseDeclaration: {
              baseType: (typeof $(this).attr("baseType") === 'undefined') ? 'string' : $(this).attr("baseType"),
              cardinality: (typeof $(this).attr("cardinality") === 'undefined') ? 'single' : $(this).attr("cardinality"),
              method: (typeof $(this).attr("method") === 'undefined') ? 'default' : $(this).attr("method"),
              caseSensitive: (typeof $(this).attr("caseSensitive") === 'undefined') ? 'false' : $(this).attr("caseSensitive"),
              pointsValue: iPoint,
              type: (typeof $(this).attr("type") === 'undefined') ? 'string' : $(this).attr("type")
            }
          };

          //Processing to import major and depending
          createImportMajorDepending(this);

          iResult.push(myResult);

          /*This case for new test maker the Extend Text:
           * Length = 1: It will replace Extend Text same place
           * Length = 0: Extend Text will be add on the bottom of test content
           */
          if (extendText.parents(".mainBody").length == 1) {
            var strExtendText = extendText.prop("outerHTML");
            var extendedtextHeight = extendText.height();

            if (extendedtextHeight === null ||
              extendedtextHeight === undefined ||
              extendedtextHeight === 0) {
              extendedtextHeight = 90;
            }

            //Fix for IE9
            if ($.browser.msie && parseInt($.browser.version, 10) < 10) {
              strExtendText = xmlContentForIE(strExtendText);
            }

            xmlContent = xmlContent.replace(strExtendText, '<p id="' + resId + '" title="' + resId + '" class="extendText" contenteditable="false" style="height: ' + extendedtextHeight + 'px;"><span class="text-place-holder">'+ escapeHtml(placeholderText || '') +'</span>' + extendedtextMask + '&nbsp;</p>');
          } else {
            var lastIndex = xmlContent.lastIndexOf("</div>");
            var extendedtextHeight = extendText.height();

            if (extendedtextHeight === null ||
              extendedtextHeight === undefined ||
              extendedtextHeight === 0) {
              extendedtextHeight = 90;
            }

            xmlContent = xmlContent.substring(0, lastIndex) + '<p id="' + resId + '" title="' + resId + '" class="extendText" contenteditable="false" style="height: ' + extendedtextHeight + 'px;"><span class="text-place-holder">'+ escapeHtml(placeholderText || '')  +'</span>' + extendedtextMask + '&nbsp;</p>' + xmlContent.substring(lastIndex, xmlContent.length);
          }

          return false;
        }
      });
    }
  });

  $(xmlContent).find("choiceInteraction").each(function () {
    var isVariablePoints = $(this).attr("variablePoints");
    if (isVariablePoints !== undefined && isVariablePoints === "true") {
      //This is for Multiple Choice with Variable Points Per Answer
      xmlContent = xmlMultipleChoiceVariable(this, xmlContent, pointsValueAlgorithmic);
    } else {
      var $choiceinteraction = $(this);
      //This is for normal choiceInteraction
      var resId = $(this).attr("responseIdentifier"),
        iShuffle = $(this).attr("shuffle"),
        iMaxChoices = (typeof $(this).attr("maxChoices") === 'undefined') ? 1 : $(this).attr("maxChoices"),
        strChoiceInteraction = $(this).prop("outerHTML"),
        newChoiceInteraction = "",
        currentChoice = this,
        correctAnswer = [],
        htmlMulChoice = "";
      var subtype = '';
      var textTrueFalse = "";

      var typeGuidance = '';
      var typeRationale = '';
      var typeGuidanceRationale = '';
      var arrMessageGuidance = [];
      var choiceinteractionDisplay = $choiceinteraction.data('display') == null ? 'vertical' : $choiceinteraction.data('display');
      var choiceinteractionGridPerRow = $choiceinteraction.data('grid-per-row') == null ? 2 : $choiceinteraction.data('grid-per-row');

      if ($(this).attr("subtype") != undefined) {
        subtype = $(this).attr("subtype");
        textTrueFalse = "TrueFalse";
      }

      //Fix for IE9
      if ($.browser.msie && parseInt($.browser.version, 10) < 10) {
        strChoiceInteraction = xmlContentForIE(strChoiceInteraction);
        strChoiceInteraction = strChoiceInteraction.replace(/contentEditable=\"false\"/g, 'contenteditable="false"');
      }

      $(xmlContent).find("responseDeclaration").each(function () {
        var $responseDeclaration = $(this);

        if (resId == $(this).attr("identifier")) {
          var responseDeclarationMethod = $responseDeclaration.attr('method');
          responseDeclarationMethod = responseDeclarationMethod == null ? 'default' : responseDeclarationMethod;

          var iPoint = $(this).attr("pointsValue");

          if (iPoint == undefined) {
            if ($("#hdPointsPossible").length > 0) {
              iPoint = $("#hdPointsPossible").val();
            }
          }

          //This is case check for old xml without point in responseDeclaration
          if (iPoint == undefined) {
            var outerValue = $(xmlContent).find("outcomedeclaration defaultvalue value");
            if (outerValue.length != 0) {
              iPoint = outerValue.text();
            } else {
              iPoint = 1;
            }
          }

          iCardinality = (typeof $(this).attr("cardinality") === 'undefined') ? 'single' : $(this).attr("cardinality"),
            simpleMulChoice = [];

          //Extract correct answer
          var correntRes = "";
          $(this).find("value").each(function () {
            if (correntRes != "") {
              correntRes += ",";
            }
            correntRes += $(this).text();
            correctAnswer.push($(this).text());
          });

          var thresholdPoints = [];
          $responseDeclaration.find('thresholds threshold').each(function (ind, threshold) {
            var $threshold = $(threshold);
            thresholdPoints.push({
              low: $threshold.attr('low'),
              hi: $threshold.attr('hi'),
              pointsvalue: $threshold.attr('pointsValue')
            });
          })

          //Build Multiple question item to append to CKEditor
          $(currentChoice).find("simpleChoice").each(function (index) {
            var $simplechoice = $(this);
            var hastypeMessageGuidance = 'noGuidance';

            // Apply data xml guidance into ckeditor
            if ($simplechoice.find('.guidance').length) {
              typeGuidance = putDataIntoIResult(arrMessageGuidance, this, 'guidance');
              hastypeMessageGuidance = 'hasGuidance';
            }

            // Apply data xml rationale into ckeditor
            if ($simplechoice.find('.rationale').length) {
              typeRationale = putDataIntoIResult(arrMessageGuidance, this, 'rationale');
              hastypeMessageGuidance = 'hasGuidance';
            }

            // Apply data xml guidance rationale into ckeditor
            if ($simplechoice.find('.guidance_rationale').length) {
              typeGuidanceRationale = putDataIntoIResult(arrMessageGuidance, this, 'guidance_rationale');
              hastypeMessageGuidance = 'hasGuidance';
            }

            var iddentify = alphabet[index];
            var item = {
              identifier: iddentify,
              value: $simplechoice.find('.answer').html(),
              arrMessageGuidance: arrMessageGuidance
            };
            var hasAudio = 'class="nonAudioIcon"';
            var hasCorrectAnwser = 'class="item"';
            var audioLink = $simplechoice.attr('audioRef');

            if (item.value == null) {
              item.value = $simplechoice.find('[stylename="answer"]').html();
            }

            if (item.value == null) {
              item.value = '';
            }

            if (audioLink != undefined && audioLink != "") {
              item.audioRef = audioLink;
              hasAudio = 'class="audioIcon "';
            }

            for (var k = 0; k < correctAnswer.length; k++) {
              if (iddentify == correctAnswer[k]) {
                hasCorrectAnwser = 'class="item answerCorrect"';
                item.answerCorrect = true;
              }
            }

            htmlMulChoice += '<div ' + hasCorrectAnwser + '><div ' + hasAudio + '>';
            htmlMulChoice += '<img alt="Play audio" class="bntPlay" src="' + getImgByVersion('multiplechoice', 'images/small_audio_play.png') + '" title="Play audio">';
            htmlMulChoice += '<img alt="Stop audio" class="bntStop" src="' + getImgByVersion('multiplechoice', 'images/small_audio_stop.png') +'" title="Stop audio">';
            htmlMulChoice += '<span class="audioRef">' + audioLink + '</span></div>';
            htmlMulChoice += "<div style='display: none;' class='" + hastypeMessageGuidance + "'>";
            htmlMulChoice += '    <img alt="Guidance" class="imageupload bntGuidance" src="' + getImgByVersion('multiplechoice', 'images/guidance_checked.png') + '" title="Guidance">';
            htmlMulChoice += "</div>";
            htmlMulChoice += '<div class="answer">' + item.identifier + '.</div>';
            htmlMulChoice += '<div class="answerContent">' + item.value + '</div>';
            htmlMulChoice += typeGuidance;
            htmlMulChoice += typeRationale;
            htmlMulChoice += typeGuidanceRationale;
            htmlMulChoice += '</div>';
            simpleMulChoice.push(item);

            // Reset array Message Guidance
            arrMessageGuidance = [];
          });

          newChoiceInteraction = '<br/><div textTrueFalse="' + textTrueFalse + '" class="multipleChoice" id="' + resId + '" title="' + resId + '" contenteditable="false"><button class="single-click" id="single-click">Click here to edit answer choices</button><img class="cke_reset cke_widget_mask multipleChoiceMark" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" />' + htmlMulChoice + ' </div>';

          xmlContent = xmlContent.replace(strChoiceInteraction, newChoiceInteraction);
          var myResult = '';

          iPoint = pointsValueAlgorithmic != null ? pointsValueAlgorithmic : iPoint;

          if (subtype != '') {
            myResult = {
              type: "choiceInteraction",
              responseIdentifier: resId,
              maxChoices: iMaxChoices,
              subtype: "TrueFalse",
              shuffle: iShuffle,
              responseDeclaration: {
                baseType: $(this).attr("baseType"),
                cardinality: iCardinality,
                method: responseDeclarationMethod,
                caseSensitive: $(this).attr("caseSensitive"),
                pointsValue: iPoint,
                type: $(this).attr("type")
              },
              correctResponse: correntRes,
              simpleChoice: simpleMulChoice
            };
          } else {
            myResult = {
              type: "choiceInteraction",
              responseIdentifier: resId,
              maxChoices: iMaxChoices,
              shuffle: iShuffle,
              responseDeclaration: {
                baseType: (typeof $(this).attr("baseType") === 'undefined') ? 'identifier' : $(this).attr("baseType"),
                cardinality: iCardinality,
                method: responseDeclarationMethod,
                caseSensitive: (typeof $(this).attr("caseSensitive") === 'undefined') ? 'false' : $(this).attr("caseSensitive"),
                pointsValue: iPoint,
                type: (typeof $(this).attr("type") === 'undefined') ? 'string' : $(this).attr("type")
              },
              correctResponse: correntRes,
              simpleChoice: simpleMulChoice,
              display: choiceinteractionDisplay
            };

            if (choiceinteractionDisplay === 'grid') {
              myResult.gridPerRow = choiceinteractionGridPerRow;
            }
          }

          if (thresholdPoints.length) {
            myResult.responseDeclaration.thresholdpoints = thresholdPoints;
          }

          //Processing to import major and depending
          createImportMajorDepending(this);

          iResult.push(myResult);

          return false;
        }
      });
    }
  });

  //Processing for partial Credit
  xmlContent = xmlImportPartialCredit(xmlContent, pointsValueAlgorithmic);

  //Processing for table hot spot
  var hasTableHotSpot = false;
  if ($(xmlContent).attr('qtischemeid') === '33') {
    var objCorrectResponse = {};
    var correctResponse = [];
    var sourceHotSpot = {
      idtableHotspot: "RESPONSE_1",
      arrayList: []
    };
    var arrayHotSpot = [];
    var objHotSpot = {};
    var maxSelected = '';

    var objTableHotSpot = {
      type: "tableHotSpot",
      responseIdentifier: '',
      maxSelected: '',
      correctResponse: [],
      responseDeclaration: {},
      sourceHotSpot: {}
    };

    $(xmlContent).find(".tableHotspotInteraction").each(function () {
      var strTableHotSpot = $(this).prop("outerHTML");
      maxSelected = $(this).attr("maxhotspot");

      //Fix for IE9
      if ($.browser.msie && parseInt($.browser.version, 10) < 10) {
        strTableHotSpot = xmlContentForIE(strTableHotSpot);
      }

      $(strTableHotSpot).find('span[typehotspot]').each(function () {
        var idHotSpot = $(this).attr("identifier");
        var ischecked = $(this).attr("ischecked");
        var pointValue = $(this).attr("pointvalue");
        var typeHotspot = $(this).attr("typehotspot");

        objHotSpot = {
          identifier: idHotSpot,
          ischecked: ischecked,
          pointValue: pointValue,
          typeHotSpot: typeHotspot,
          position: $(this).attr("position") || 'bottom'
        };

        arrayHotSpot.push(objHotSpot);
      });

      sourceHotSpot.arrayList = arrayHotSpot;
    });

    // Inline HotSpot
    $(xmlContent).find('span[typehotspot]').not(".tableHotspotInteraction span[typehotspot]").each(function() {
      sourceHotSpot.arrayList.push({
        identifier: $(this).attr("identifier"),
        ischecked: $(this).attr("ischecked"),
        pointValue: $(this).attr("pointvalue"),
        typeHotSpot: $(this).attr("typehotspot"),
        maxHotSpot: $(this).attr("maxhotspot"),
        position: $(this).attr("position") || 'bottom'
      })
    })

    var tagResponseDeclaration = $(xmlContent).find("responseDeclaration")[0];
    var responseIdentifier = $(tagResponseDeclaration).attr('identifier');
    if (responseIdentifier === "RESPONSE_1") {
      var absoluteGrading = $(tagResponseDeclaration).attr('absoluteGrading');
      var partialGrading = $(tagResponseDeclaration).attr('partialGrading');
      var algorithmicGrading = $(tagResponseDeclaration).attr('algorithmicGrading') === '1' ? '1' : '0';
      var pointsValue = $(tagResponseDeclaration).attr('pointsValue');
      var tagsCorrectresponse = $(tagResponseDeclaration).find('correctresponse');

      objTableHotSpot.responseDeclaration = {
        absoluteGrading: absoluteGrading,
        partialGrading: partialGrading,
        algorithmicGrading: algorithmicGrading,
        pointsValue: pointsValueAlgorithmic != null ? pointsValueAlgorithmic : pointsValue
      };

      for (var i = 0, tagsCorrectresponseLen = tagsCorrectresponse.length; i < tagsCorrectresponseLen; i++) {
        var itemTagCorrect = tagsCorrectresponse[i];
        var identifier = $(itemTagCorrect).attr('identifier');
        var pointValue = $(itemTagCorrect).attr('pointvalue');
        var isAbsolute = $(itemTagCorrect).attr('isabsolute');
        objCorrectResponse = {
          identifier: identifier,
          pointValue: pointValue,
          isAbsolute: isAbsolute
        };
        correctResponse.push(objCorrectResponse);
      }
    }

    objTableHotSpot.sourceHotSpot = sourceHotSpot;

    objTableHotSpot.correctResponse = correctResponse;
    objTableHotSpot.responseIdentifier = responseIdentifier;
    objTableHotSpot.maxSelected = maxSelected;

    iResult.push(objTableHotSpot);
    hasTableHotSpot = true;
  }
  //Processing for Text Hot Spot.
  xmlContent = importTextHotSpot(xmlContent, pointsValueAlgorithmic);
  // Processing for number line hot spot
  xmlContent = xmlImportNumberLine(xmlContent, pointsValueAlgorithmic);
  //Processing for hot spot image
  xmlContent = xmlImportImageHotSpot(xmlContent, pointsValueAlgorithmic);
  //Processing for hot spot image
  xmlContent = xmlImportSequence(xmlContent, pointsValueAlgorithmic);

  // Processing for item type on image
  xmlContent = xmlImportItemTypeOnImage(xmlContent);

  //Processing for MathML
  xmlContent = loadMathML(xmlContent);

  //Keep innerHTML for inserting to CKEditor
  if (schemeID == '33') {
    var element = $(xmlContent);
    element.find('span[identifier]').html('&#8203;');
    xmlContent = element.prop('outerHTML');
  }

  //Processing for Reference Object
  $("#listReference").empty(); //Clear the list before load
  var findDataExists = [];
  $('.hiddenReference').children().each(function () {
    findDataExists.push({
      refid: $(this).data('refid'),
      text: $(this).data('text'),
      name: $(this).data('name')
    });
  });
  $(xmlContent).find("object").each(function () {
    var noshuffle = false;
    if ($(this).attr('noshuffle') != undefined) {
      noshuffle = $(this).attr('noshuffle');
    }
    if ($(this).attr("data") != undefined) {
      var urlLink = $(this).attr("data");
      var refObjectId = urlLink.substring(urlLink.lastIndexOf("/") + 1, urlLink.lastIndexOf("."));
      refObjectId = refObjectId.replace("RSC-LOGIC--", "");
      var name = $(this).attr("refobjectid");

      var findExists = findDataExists.find(x => x.refid == refObjectId);
      if (findExists) {
        name = findExists.text;
      }
      if ($(this).attr("dataFileUploadPassageID") != undefined) { //Data File Upload Passage
        if (noshuffle) {
          $("#listReference").append('<li type="objectUrl" data="' + $(this).attr("data") + '" noshuffle=true><input type="button" class="delReference" value=""> Imported Reference# ' + name + ' <input type="button" class="viewReference" value=""></li>');
        } else {
          $("#listReference").append('<li type="objectUrl" data="' + $(this).attr("data") + '"><input type="button" class="delReference" value=""> Imported Reference# ' + name + ' <input type="button" class="viewReference" value=""></li>');
        }
      } else {
        if (noshuffle) {
          $("#listReference").append('<li type="objectUrl" data="' + $(this).attr("data") + '" noshuffle=true><input type="button" class="delReference" value=""> Navigate Reference# ' + name + ' <input type="button" class="viewReference" value=""></li>');
        } else {
          var liElement = '<li type="objectUrl" data="' + $(this).attr("data") + '"';

          if ($(this).attr("dataFileUploadTypeID")) {
            liElement += ' dataFileUploadTypeID="' + $(this).attr("dataFileUploadTypeID") + '" ';
          }


          if ($(this).attr("Qti3pPassageID")) {
            liElement += ' Qti3pPassageID="' + $(this).attr("Qti3pPassageID") + '" ';
          }

          if ($(this).attr("Qti3pSourceID")) {
            liElement += ' Qti3pSourceID="' + $(this).attr("Qti3pSourceID") + '" ';
          }

          liElement += ' > <input type="button" class="delReference" value=""> Certical Reference Number: ' + refObjectId + ' <input type="button" class="viewReference" value=""></li>';
          $("#listReference").append(liElement);
        }
      }
    } else {
      var name = $(this).attr("refobjectid");

      var findExists = findDataExists.find(x => x.refid == name);
      if (findExists) {
        name = findExists.text;
      }

      if (noshuffle) {
        $("#listReference").append('<li type="objectId" data="' + $(this).attr("refobjectid") + '" noshuffle=true><input type="button" class="delReference" value=""> Reference# ' + name + ' <input type="button" class="viewReference" value=""></li>');
      } else {
        $("#listReference").append('<li type="objectId" data="' + $(this).attr("refobjectid") + '"><input type="button" class="delReference" value=""> Reference# ' + name + ' <input type="button" class="viewReference" value=""></li>');
      }
    }

    $(".viewReference").unbind("click").click(function () {
      viewRefObjectContent(this);
    });

    //Create event click for delete button
    $(".delReference").unbind("click").click(function () {
      $(this).parent().remove();
    });
  });

  var mainBody = $(xmlContent).find(".mainBody");

  if (mainBody.length == 0) {
    mainBody = $(xmlContent).find(".mainbody");
  }

  restructureMainbody(mainBody);

  mainBody.find('.textEntryInteraction').each(function() {
    var textEntryInteraction = $(this)
    if (textEntryInteraction) {
      var top = textEntryInteraction.attr('data-top');
      var left = textEntryInteraction.attr('data-left');
      if (top !== undefined && left !== undefined) {
        textEntryInteraction.css({
          top: top + 'px',
          left: left + 'px',
          position: 'absolute'
        })
      }
    }
  })

  //Re-sort iResult to match with mainbody
  var iResultTemp = [];
  $(xmlContent).find("itembody").find("[id]").each(function () {
    for (var k = 0; k < iResult.length; k++) {
      if ($(this).attr("id") == iResult[k].responseIdentifier) {
        iResultTemp.push(iResult[k]);
      }
    }
  });
  if (!hasTableHotSpot) {
    iResult = iResultTemp;
  }

  // Get text to speech from xmlContent
  iResultComponent.texttospeech = getTexttospeechFromXml(xmlContent);

  //Show/hide button Algorithmic
  TestMakerComponent.isShowAlgorithmicConfiguration = getAlgorithmicConfiguration(xmlContent, iSchemeID);

  TestMakerComponent.attachmentSetting = getAttachmentSetting(xmlContent);

  var waitForLoaded = setInterval(() => {
    if (CKEDITOR.instances[ckID].loaded && CKEDITOR.instances[ckID].instanceReady) {
      clearInterval(waitForLoaded);
      CKEDITOR.instances[ckID].setData(unreplaceVideo(mainBody.html()), function () {
        //stop all video after video has been added to item editor. This need setTimeout because the browser take a while to play video. We must stop after the video played.
        setTimeout(function () {
          $("#divQContent iframe").contents().find("video").trigger("pause");
        }, 100);

        //Correct image incase image has width or height is NaN
        var $getAllImageUpload = $("iframe.cke_wysiwyg_frame").contents().find("img.imageupload");
        $getAllImageUpload.each(function (index, currentImage) {
          var $currentImage = $(currentImage);
          var newHeight = $currentImage.height();
          var newWidth = $currentImage.width();
          if ($currentImage.attr('height') == undefined || $currentImage.attr('height').toString() == "NaN") {
            if ($currentImage.attr('percent') != undefined) {
              newHeight = (newHeight * parseInt($currentImage.attr('percent').toString() + "0")) / 100;
              $currentImage.attr({
                'height': newHeight
              });
            } else {
              $currentImage.attr({
                'height': newHeight
              });
            }
          }
          if ($currentImage.attr('width') == undefined || $currentImage.attr('width').toString() == "NaN") {
            if ($currentImage.attr('percent') != undefined) {
              newWidth = (newWidth * parseInt($currentImage.attr('percent').toString() + "0")) / 100;
              $currentImage.attr({
                'width': newWidth
              });
            } else {
              $currentImage.attr({
                'width': newWidth
              });
            }
          }
        });
        CKEDITOR.instances[ckID].fireOnce('editItemLoaded');
      });
    }
  }, 100);
}

function restructureMainbody(mainBody) {
  var nextELements = mainBody.nextAll();

  if (nextELements.length) {
    nextELements.each(function (index, element) {
      mainBody.append(element);
    })
  }
}

/**
 * Create new response identify
 */
function createResponseId() {
  var responseId = "RESPONSE_" + (iResult.length + 1);
  for (var m = 0; m < iResult.length; m++) {
    resId = "RESPONSE_" + (m + 1);
    if (iResult[m].responseIdentifier != resId) {
      var isOnlyOne = true;
      for (k = 0; k < iResult.length; k++) {
        if (resId == iResult[k].responseIdentifier) {
          isOnlyOne = false;
        }
      }

      if (isOnlyOne) {
        responseId = resId;
        break;
      }
    }
  }
  return responseId;
}

/**
 * Create new response identify base on DOM
 */
 function createResponseIdFromDOM(selector) {
  selector += ' [id^=RESPONSE_]';
  if ($(selector).length === 0 || $(selector).is(':hidden')) {
    return createResponseId()
  }

  var ids = [];
  $(selector).each(function(idx, element) {
    ids.push(+$(element).attr('id').replace('RESPONSE_', ''))
  });
  ids.sort(function(a, b) { return a - b; })
  var respIdx = 1;
  while (ids.includes(respIdx)) {
    respIdx++;
  }

  return 'RESPONSE_' + respIdx;
}

/**
 * refresh response id for textEntry and inline choice
 *
 * @data data of question content
 */
function refreshResponseId() {
  var xmlContent = replaceVideo(CKEDITOR.instances[ckID].getData());

  var data = $("<div></div>").append(xmlContent);

  //Processing to get all current Response Id
  var totalResId = []; //{responseId: "RESPONSE_1"}

  $("#questionType input[type=checkbox]").each(function (index, item) {
    if ($(this).is(":checked")) {
      var questionType = $(this).val()
      if (questionType == "mtpChoice") {
        totalResId.push({
          responseId: $(".multipleChoice").attr("responseid")
        });
      } else if (questionType == "exText") {
        totalResId.push({
          responseId: $(".extendedText").attr("responseid")
        });
      } else if (questionType == "drawInter") {
        totalResId.push({
          responseId: $(".drawInteraction").attr("responseid")
        });
      }
    }
  });

  $(data).find(".textEntryInteraction").each(function () {
    totalResId.push({
      responseId: $(this).attr("id")
    });
  });

  $(data).find(".inlineChoiceInteraction").each(function () {
    totalResId.push({
      responseId: $(this).attr("id")
    });
  });

  $(data).find(".drawTool").each(function () {
    totalResId.push({
      responseId: $(this).attr("id")
    });
  });

  $(data).find(".extendText").each(function () {
    totalResId.push({
      responseId: $(this).attr("id")
    });
  });

  $(data).find(".multipleChoice").each(function () {
    totalResId.push({
      responseId: $(this).attr("id")
    });
  });

  $(data).find(".imageHotspotInteraction").each(function () {
    totalResId.push({
      responseId: $(this).attr("id")
    });
  });
  $(data).find(".tableHotspotInteraction").each(function () {
    totalResId.push({
      responseId: 'RESPONSE_1'
    });
  });

  $(data).find(".numberline-selection").each(function () {
    totalResId.push({
      responseId: $(this).attr("id")
    });
  });

  if ($(data).find(".partialSourceObject").length > 0 || $(data).find(".partialDetinationObject").length > 0 || iSchemeID.toString() == "30") {
    var partialID = "Partial_1";
    if ($(data).find(".partialSourceObject").length > 0) {
      partialID = $(data).find(".partialSourceObject").eq(0).attr("partialID");
    } else if ($(data).find(".partialDetinationObject").length > 0) {
      partialID = $(data).find(".partialDetinationObject").eq(0).attr("partialID");
    }
    for (n = 0; n < iResult.length; n++) {
      if (iResult[n].partialID == partialID) {
        totalResId.push({
          responseId: iResult[n].responseIdentifier
        });
        break;
      }
    }
  }
  //push id hot spot when no find tableHotspotInteraction
  if (iResult[0] != undefined) {
    if (iResult[0].type != undefined) {
      if (iResult[0].type === 'tableHotSpot' && $(data).find(".tableHotspotInteraction").length === 0) {
        totalResId.push({
          responseId: 'RESPONSE_1'
        });
      }
    }
  }

  //This is check for Text Hot Spot
  if ($(data).find(".marker-linkit").length > 0 || iSchemeID.toString() == "31") {
    totalResId.push({
      responseId: "RESPONSE_1"
    });
  }

  if ($(data).find('.partialAddSourceNumerical').length > 0 || $(data).find('.partialAddDestinationNumerical').length > 0) {
    totalResId.push({
      responseId: 'RESPONSE_1'
    });
  }

  $(data).find(".sequenceBlock").each(function () {
    totalResId.push({
      responseId: $(this).attr("id")
    });
  });

  //This case happen if user removed all Response Id
  if (totalResId.length === 0) {
    iResult = [];
  } else {
    //Loop to remove ResponseId don't exist in current Editor
    var tempIResult = [];

    for (n = 0; n < iResult.length; n++) {
      for (i = 0; i < totalResId.length; i++) {
        //if responseIdentifier exist return true;
        if (iResult[n].responseIdentifier == totalResId[i].responseId) {
          tempIResult.push(iResult[n]);
          break;
        }
      }
    }

    iResult = tempIResult;
  }
}

function vnsAudio(source) {
  var iniConfig = source;
  //var src = iniConfig.src;
  var id = 'vnsAudio';

  this.init = function () {
    var me = this;

    if (!iniConfig.src) {
      alert('URL for Audio should be defined');
      return;
    }

    if (!$('#' + id).length) {
      // Adding DOM
      var player = $('<audio/>', {
        id: id,
        src: iniConfig.src
      }).appendTo('body');
    } else {
      $('#' + id).attr('src', iniConfig.src);
    }

    // Apply Player
    this.audio = new MediaElement(id, {
      success: function (me) {
        me.play();
        var emptyFn = function () { };
        // Add Listeners
        me.addEventListener('play', (iniConfig.onPlay || emptyFn), false);
        me.addEventListener('pause', (iniConfig.onPause || emptyFn), false);
        me.addEventListener('ended', (iniConfig.onEnded || emptyFn), false);
      },
      error: function (err) { }
    });

    //$('#vnsAudio')[0].play();
  };

  this.play = function () {
    if (this.audio == undefined) this.init();
    this.audio.setCurrentTime(0);
    this.audio.play();
  };
  this.pause = function () {
    if (this.audio == undefined || !this.audio.pause) return;

    this.audio.pause();
  };
  this.isPaused = function () {
    return this.audio.paused;
  };
  this.showMess = function (msg) {
    var dom = document.getElementById('status');
    dom.innerHTML = dom.innerHTML + msg + "<br/>";
  };

  this.init();
};

function resetUIAudio() {
  $(".bntPlay").show();
  $(".bntStop").hide();
}
/**
 * Set the question type Drap and Drop (Sequence/Order)
 */
function setSequenceOrderTool() {
  quesType = 36;
  isAddnew = true;
  iResult = [];
  //Clear content after destroy CKEditor
  document.getElementById(ckID).value = "";

  try {
    CKEDITOR.instances[ckID].destroy(true);
  } catch (e) { }

  CKEDITOR.replace(ckID, {
    extraPlugins: 'indent,mathjax,sharedspace,mathfraction,boxedtext,video,glossary,tabspaces,sequenceorder,leaui_formula',
    alphaBeta: alphabet,
    playAudio: loadAudioUrl,
    qtiSchemeID: 33,
    height: 400,
    toolbar: [
      ['JustifyLeft', 'JustifyCenter', 'JustifyRight', '-', 'NumberedList', 'BulletedList'],
      ['Bold', 'Italic', 'Underline'],
      ['Sameline'],
      ['Mathfraction'],
      ['FontSize', 'Subscript', 'Superscript', '-', 'RemoveFormat'],
      ['Table', 'AudioUpload', 'Reference', 'ImageUpload', 'VideoUpload', 'SpecialChar', 'Mathjax', 'LeauiFormula', 'Glossary'],
      ['Indent', 'Outdent', 'Tabspaces'],
      ['BoxedText'],
      ['SequenceOrder']
    ],
    sharedSpaces: {
      top: 'topSpace',
      bottom: 'bottomSpace'
    },
    extraAllowedContent: 'span[stylename]; span(smallText,normalText,largeText,veryLargeText);'
  });
}
/**
 * Set the question type Multiple Choice with Variable Points
 */
function setMultipleChoiceVariable() {
  quesType = 37;
  isAddnew = true;
  iResult = [];

  //Clear content after destroy CKEditor
  document.getElementById(ckID).value = "";

  CKEDITOR.replace(ckID, {
    extraPlugins: 'indent,mathjax,sharedspace,multiplechoicevariable,mathfraction,imageupload,boxedtext,video,glossary,tabspaces,leaui_formula',
    alphaBeta: alphabet,
    playAudio: loadAudioUrl,
    qtiSchemeID: 37,
    height: 400,
    toolbar: [
      ['JustifyLeft', 'JustifyCenter', 'JustifyRight', '-', 'NumberedList', 'BulletedList'],
      ['Bold', 'Italic', 'Underline'],
      ['Sameline'],
      ['Mathfraction'],
      ['FontSize', 'Subscript', 'Superscript', '-', 'RemoveFormat'],
      ['Link', 'Unlink', 'Table', 'AudioUpload', 'Reference', 'ImageUpload', 'VideoUpload', 'SpecialChar', 'Mathjax', 'LeauiFormula', 'Glossary'],
      ['Indent', 'Outdent', 'Tabspaces'],
      ['MultipleChoiceVariable'],
      ['BoxedText']
    ],
    sharedSpaces: {
      top: 'topSpace',
      bottom: 'bottomSpace'
    },
    extraAllowedContent: 'span[stylename]; span(smallText,normalText,largeText,veryLargeText);'
  });
}

/*
 * 1: Multiple choice
 * 3: Multi-select
 * 8: Inline choice
 * 9: Text entry
 * 10: Open ended
 * 10d: DrawTool
 * 21: Complex
 * 30: Drag & Drop
 */
function loadQuestionType(qtiSchemeID) {
  iSchemeID = qtiSchemeID;
  if (qtiSchemeID == "21") {
    checkSavingComplex = true;
  }
  createCKEditorIndex();

  //switch to correct question type
  switch (qtiSchemeID.toString()) {
    case "1":
      setSimpleChoice();
      break;
    case "3":
      setMultipleChoice();
      break;
    case "8":
      setInlineChoice();
      break;
    case "9":
      setTextEntry();
      break;
    case "10":
      setExtendedText();
      break;
    case "10d":
      setDrawTool();
      break;
    case "21":
      setComplex();
      break;
    case "30":
      setDragTool();
      break;
    case "31":
      setTextHotSpot();
      break;
    case "32":
      setImageHotSpotTool();
      break;
    case "33":
      setTableHotSpot();
      break;
    case '34':
      setNumberLineHotSpotTool();
      break;
    case '36':
      setSequenceOrderTool();
      break;
    case '37':
      setMultipleChoiceVariable();
      break;
    default:
      setSimpleChoice();
  }
}

function createCKEditorIndex() {
  var ckEditorID = "questionContent" + $.now();
  $("#divQContent").unbind().empty().append('<textarea cols="80" id="' + ckEditorID + '" rows="10"></textarea>');
  ckID = ckEditorID;
  currentCkID = ckEditorID;
}
/***
 * Export data from mkEditor to XML
 ***/
function xmlPassageExport() {
  refeshConfig();
  //create width and height for image incase it in table and has width and height is 100%
  var $imgTable = $('iframe.cke_wysiwyg_frame').contents().find('.linkit-table img');
  $imgTable.each(function () {
    var $currImg = $(this);
    var $attrWidth = $currImg.attr("width");
    var $attrHeight = $currImg.attr("height");
    if ($attrWidth == "100%") {
      $currImg.attr({
        "width": $currImg.width()
      });
    }

    if ($attrHeight == "auto") {
      $currImg.attr({
        "height": $currImg.height()
      })
    }
  });

  var tempData = CKEDITOR.instances["passageContent"].getData();
  tempData = replaceVideo(tempData);
  // Step 1: Temporarily mark <div class="videoSpan audio-container"> and its children
  tempData = replaceDivWithKeepDiv(tempData);
  // Step 2: Replace remaining <div> with <p>
  tempData = tempData.replace(/(\r\n|\n|\r)/gm, "").replace(new RegExp("\>[\n\t]+\<", "g"), "><").replace(/<div/g, "<p").replace(/<\/div>/g, "</p>");
  // Step 3: Restore preserved <div> elements
  tempData = tempData.replace(/<keepdiv/g, "<div").replace(/<\/keepdiv>/g, "</div>");
  tempData = "<div>" + $("<div></div>").append(tempData).html() + "</div>";
  tempData = tempData.replace(new RegExp('<input class="editvideo" type="button" value="Edit video">', "g"), '');

  //remove spanVideo
  $(tempData).find(".videoSpan").each(function () {
    var oldSpanVideo = $(this).prop("outerHTML"),
      newSpanVideo = $(this).html().replace("</videolinkit>Your browser does not support the video tag.", "Your browser does not support the video tag.</videolinkit>");

    //This is make sure the video link will be keep when we export to xml
    newSpanVideo = newSpanVideo.replace(GetViewReferenceImg, GetViewReferenceImg + "/s3videolink/");

    tempData = tempData.replace(oldSpanVideo, newSpanVideo);
    tempData = tempData.replace(/<(div|p)[^>]*class=["']audio-mask[^>]*>.*?<\/\1>/g, ""); // remove div mask audio
  });

  //Remove url temp of image
  var re = new RegExp(GetViewReferenceImg.replace(/\?/g, "\\?"), 'g');
  tempData = tempData.replace(re, "");

  //Processing for ol to <list></list>
  var htmlList = '<list listStylePosition="outside" listStyleType="decimal" paragraphSpaceAfter="12" styleName="passageNumbering"><listMarkerFormat><ListMarkerFormat color="#aaaaaa" paragraphEndIndent="20"/></listMarkerFormat>';
  htmlEndList = '</list>'; //Make sure when export to html it will not has break line of textare
  tempData = tempData.replace(/<ol>/g, htmlList).replace(/<\/ol>/g, htmlEndList);
  tempData = tempData.replace(/<ol\b([^>]*)>/g, '<list$1>')

  tempData = escapeBasicStyles(tempData);

  //Processing for sub
  $(tempData).find("sub").each(function () {
    var oldSub = $(this).prop("outerHTML"),
      newSub = '<span styleName="sub" class="sub">' + $(this).html() + "</span>";
    tempData = tempData.replace(oldSub, newSub);
  });

  //Processing for sup
  $(tempData).find("sup").each(function () {
    var oldSup = $(this).prop("outerHTML"),
      newSup = '<span styleName="sup" class="sup">' + $(this).html() + "</span>";
    tempData = tempData.replace(oldSup, newSup);
  });

  //add styleName for text-align
  $(tempData).find(".center").each(function () {
    var oldCenter = $(this).prop("outerHTML").replace(/stylename="/g, 'styleName="'),
      newCenter = $(this).attr("styleName", "center").prop("outerHTML").replace('stylename="center"', 'styleName="center"');
    tempData = tempData.replace(oldCenter, newCenter);
  });

  $(tempData).find(".alignRight").each(function () {
    var oldRight = $(this).prop("outerHTML").replace(/stylename="/g, 'styleName="'),
      newRight = $(this).attr("styleName", "alignRight").prop("outerHTML").replace('stylename="alignRight"', 'styleName="alignRight"');
    tempData = tempData.replace(oldRight, newRight);
  });

  //remove span speChar
  $(tempData).find(".speChar").each(function () {
    //no remove tag span has white space specical
    if ($(this).html() !== "&nbsp;") {
      var oldCenter = $(this).prop("outerHTML"),
        newCenter = $(this).text();
      tempData = tempData.replace(oldCenter, newCenter);
    }
  });

  // Remove videoSpan class wrapper video
  $(tempData).find('.videoSpan').each(function () {
    var $videoSpan = $(this);
    var oldSpanVideo = $videoSpan.prop('outerHTML');
    var newSpanVideo = $videoSpan.find('videolinkit').prop('outerHTML');

    if ($.browser.msie && parseInt($.browser.version, 10) < 10) {
      oldSpanVideo = oldSpanVideo.replace(/contentEditable=\"false\"/g, 'contenteditable="false"');
      newSpanVideo = newSpanVideo.replace(/contentEditable=\"false\"/g, 'contenteditable="false"');
    }
  });
  //Make sure image has close tag
  $(tempData).find("img").each(function () {
    var htmlImage = $(this).prop("outerHTML"),
      newhtmlImage = $(htmlImage.replace('contenteditable="false"', '')).prop("outerHTML");
    newhtmlImage = newhtmlImage.replace(/>$/, " />");

    if ($(this).css("float") == "left" || $(this).css("float") == "right") {
      if ($(newhtmlImage).attr("float") != undefined) {
        newhtmlImage = newhtmlImage.replace("float: " + $(this).css("float") + ";", "").replace(new RegExp('float="' + $(newhtmlImage).attr("float") + '"', "g"), '').replace("/>", ' float="' + $(this).css("float") + '" />');
      } else {
        newhtmlImage = newhtmlImage.replace("float: " + $(this).css("float") + ";", "").replace("/>", ' float="' + $(this).css("float") + '" />');
      }
    }

    tempData = tempData.replace(htmlImage, newhtmlImage);
  });
  //Make sure only relative link /ItemSet_..... of image is kept
  $(tempData).find("img").each(function () {
    var htmlImage = $(this).prop("outerHTML"),
      newhtmlImage = htmlImage.replace(/>$/, "/>");
    var src = $(newhtmlImage).attr("src");
    if (src != null) {
      var idx = src.indexOf('/RO/RO_');
      if (idx > 0) {
        var newSrc = src.substring(idx, src.length).replace(/%20/g, ' ');
        tempData = tempData.replace(src, newSrc);
      }
    }
  });
  //Make sure only relative link /ItemSet_..... of audio is kept
  $(tempData).find('[audioRef]').each(function () {
    var audioRef = $(this).attr("audioRef");
    if (audioRef != null) {
      var idx = audioRef.indexOf('/RO/RO_');
      if (idx > 0) {
        var newAudioRef = audioRef.substring(idx, audioRef.length).replace(/%20/g, ' ');
        tempData = tempData.replace(audioRef, newAudioRef);
      }
    }
  });
  var passageAudioLink = "";
  if ($("#audioRemovePassage").css("display") == "block" && $("#audioRemovePassage .audioRef").text() != "") {
    var audioLink = $("#audioPassage .audioRef").text();
    var idx = audioLink.indexOf('/RO/RO_');
    if (idx > 0) {
      audioLink = audioLink.substring(idx, audioLink.length).replace(/%20/g, ' ');
    }
    passageAudioLink = 'audioRef="' + audioLink + '" ';
  }

  //Reset status for CKEditor
  CKEDITOR.instances[ckID].resetDirty();

  //Revert to correct video link after remove temp link for audio and image.
  if (GetViewReferenceImg != "") {
    var re = new RegExp(("/s3videolink/").replace(/\?/g, "\\?"), 'g');
    tempData = tempData.replace(re, GetViewReferenceImg);
  }

  //Remove all <br /> tag
  tempData = tempData.replace(/<br>/g, '<br />').replace(/stylename="/g, 'styleName="').replace(/style=""/g, "").replace("<div>", "").replace(/<\/div>$/, "").replace(/&nbsp;/g, "&#160;");
  //Fix for IE9
  if ($.browser.msie && parseInt($.browser.version, 10) < 10) {
    tempData = tempData.replace('<?XML:NAMESPACE PREFIX = "[default] http://www.w3.org/1998/Math/MathML" NS = "http://www.w3.org/1998/Math/MathML" />', '').replace('<?XML:NAMESPACE PREFIX = [default] http://www.w3.org/1998/Math/MathML NS = "http://www.w3.org/1998/Math/MathML" />', '');
    tempData = tempData.replace('<?XML:NAMESPACE PREFIX = "[default] http://www.w3.org/1998/Math/MathML" NS = "http://www.w3.org/1998/Math/MathML" />', '');
    tempData = tempData.replace('<?XML:NAMESPACE PREFIX = [default] http://www.w3.org/1998/Math/MathML NS = "http://www.w3.org/1998/Math/MathML" />', '');
  }

  tempData = unreplaceVideo(tempData);

  tempData = '<passage toolName="linkitTLF" xmlUnicode="true" toolVersion="2.0" ' + passageAudioLink + '><div class="passage" styleName="passage">' + tempData + '</div></passage>';

  // Remove "zero width spaces"
  tempData = tempData.replace(/[\u200B-\u200D\uFEFF]/g, '');

  tempData = tempData
    .replace(
      /<source ([^>]*?)>\s*<\/source>/gi,
      function (match, attributes) {
        return '<source ' + attributes.trim() + ' />';
      }
    ).replace(
      /<source ([^>]*?)>/gi,
      function (match, attributes) {
        if (attributes.trim().endsWith('/')) {
          return match;
        }
        return '<source ' + attributes.trim() + ' />';
      }
    );

  return tempData;
}
/***
 * Import data from mkEditor to XML
 ***/
function xmlPassageImport(element) {
  refeshConfig();
  var xmlContent = "";
  if (element.val() != "") {
    var passageAudioLink = "";
    xmlContent = element.val();

    xmlContent = replaceVideo(xmlContent);

    xmlContent = xmlContent.replace(/(\r\n|\n|\r)/gm, "").replace(new RegExp("\>[\n\t]+\<", "g"), "><");

    // Processing for ol to <list></list> Must be replace before convert xml to html by jquery
    // because tag name will be change to lowercase
    xmlContent = replaceListTool(xmlContent);

    //Check audio
    if ($(xmlContent).attr("audioRef") != null && $(xmlContent).attr("audioRef") != "") {
      $('#audioRemovePassage').show();
      $('#audioRemovePassage').parents('.questionType').show();
      $('#audioRemovePassage .audioRef').append($(xmlContent).attr('audioRef'));

      passageAudioLink = 'audioRef="' + $(xmlContent).attr("audioRef") + '" ';
    }

    //Processing for sameline
    $(xmlContent).find("span.nobreak").each(function () {
      var newSameline = '<sameline class="nobreak" stylename="nobreak">' + $(this).html() + '</sameline>';
      xmlContent = xmlContent.replace($(this).prop("outerHTML"), newSameline);
    });

    xmlContent = unescapeBasicStylesFromXml(xmlContent);

    //Transform xml to html and remove passage wrap
    xmlContent = "<div>" + $("<div></div>").append(xmlContent).find("div.passage").html() + "</div>";

    //Processing for sub
    $(xmlContent).find(".sub").each(function () {
      //var oldSub = $(this).prop("outerHTML").replace("stylename", "styleName"),
      var oldSub = $(this).prop("outerHTML"),
        subClass = $(this).attr("class").replace(/sub/g, ""),
        oldSubClass = "";

      if (subClass != "") {
        oldSubClass = ' class = "' + $(this).attr("class").replace(/sub/g, "") + '" ';
      }
      newSub = "<sub" + oldSubClass + ">" + $(this).html() + "</sub>";
      xmlContent = xmlContent.replace(oldSub, newSub);
    });

    //Processing for sup
    $(xmlContent).find(".sup").each(function () {
      //var oldSup = $(this).prop("outerHTML").replace("stylename", "styleName"),
      var oldSup = $(this).prop("outerHTML"),
        supClass = $(this).attr("class").replace(/sup/g, ""),
        oldSupClass = "";

      if (supClass != "") {
        oldSupClass = ' class = "' + supClass + '" ';
      }

      newSup = "<sup" + oldSupClass + ">" + $(this).html() + "</sup>";
      xmlContent = xmlContent.replace(oldSup, newSup);
    });

    // Process Video Upload
    $(xmlContent).find(".video").each(function () {
      var _video = $(this).html();
      var newvideo = $(this).prepend('<input type="button" value="Edit video" class="editvideo"/>').html();
      xmlContent = xmlContent.replace(_video, newvideo);
    });

    // Process Audio Upload
    var tempXmlContent = xmlContent;
    var $tempXmlContent = $(tempXmlContent);
    $tempXmlContent.find("p.audio-mask").each(function () {
        let $this = $(this);
        let $next = $this.next("p:empty"); // Check if next element is an empty <p>
        if ($next.length) {
            $next.remove(); // Remove empty <p> if found
        }
        $this.remove(); // Remove the <p class="audio-mask">
    });
    xmlContent = $tempXmlContent.prop('outerHTML');
    $(xmlContent).find("audio.editvideo").each(function () {
      var audio = $(this).prop("outerHTML");
      var $mask =
        '<div class="audio-mask top"></div>' +
        '<div class="audio-mask bottom"></div>' +
        '<div class="audio-mask left"></div>' +
        '<div class="audio-mask right"></div>';
      var newAudio = '<div contenteditable="false" class="videoSpan audio-container">' + audio + $mask + "</div>";
      xmlContent = xmlContent.replace(audio, newAudio);
    });

    // Adding spanVideo class to item cannot editable
    $(xmlContent).find("videolinkit").each(function () {
      var oldSpanVideo = $(this).prop("outerHTML").replace('<?XML:NAMESPACE PREFIX = "[default] http://www.imsglobal.org/xsd/imsqti_v2p0" NS = "http://www.imsglobal.org/xsd/imsqti_v2p0" />', ""),
        newSpanVideo = '<span class="videoSpan" contenteditable="false">' + oldSpanVideo + '</span>';
      xmlContent = xmlContent.replace(oldSpanVideo, newSpanVideo);
    });

    //Processing image url
    $(xmlContent).find("img").each(function () {
      var currentSrc = $(this).attr("src") || "",
        originalImg = $(this).prop("outerHTML");

      if (currentSrc.indexOf("http") != 0 && currentSrc.indexOf('data:image') !== 0) {
        var s3Link = GetViewReferenceImg;
        s3Link = rightSlash(GetViewReferenceImg);
        currentSrc = leftSlash(currentSrc);

        currentSrc = s3Link + currentSrc;
      }

      var newImage = $(this).attr("src", currentSrc).addClass("imageupload").attr("contenteditable", false);

      if ($(this).attr("drawable") == undefined) {
        newImage = newImage.attr("drawable", false);
      }

      if ($(this).attr("percent") == undefined) {
        newImage = newImage.attr("percent", -1);
      }

      if ($(this).attr("float") != undefined) {
        newImage = newImage.css("float", $(this).attr("float"));
      }

      xmlContent = xmlContent.replace(originalImg.replace(">", ""), newImage.prop("outerHTML").replace(">", ""));
    });

    //Processing for MathML
    xmlContent = loadMathML(xmlContent);

    xmlContent = xmlContent.replace('<passage toolName="linkitTLF" xmlUnicode="true" toolVersion="1.0" ' + passageAudioLink + '><div class="passage" styleName="passage">', "").replace('<passage toolname="linkitTLF" xmlunicode="true" toolversion="1.0" ' + passageAudioLink + '><div class="passage" stylename="passage">', "").replace('<passage toolname="linkitTLF" xmlunicode="true" toolversion="2.0" ' + passageAudioLink + '><div class="passage" stylename="passage">', "").replace(/<\/div><\/passage>$/, '');
    xmlContent = xmlContent.replace("<div>", "").replace(/<\/div>$/, '');
    xmlContent = unreplaceVideo(xmlContent);

    // Update xmlContent when order list format not correct
    if (xmlContent.indexOf('<list liststyleposition="outside"') !== -1) {
      var $div = $('<div/>');
      xmlContent = xmlContent.replace(/<list liststyleposition="outside" liststyletype="decimal" paragraphspaceafter="12" stylename="passageNumbering"><\/list>/g, '')
        .replace(/<list liststyleposition="outside" liststyletype="decimal" paragraphspaceafter="12" stylename="passageNumbering">/g, '')
        .replace(/<\/list>/g, '');

      $div.append(xmlContent);

      // Update li tag when li tag doesn't wrapper by ol
      $div.find('li').not('ol li').replaceWith(function () {
        var $li = $(this);
        var $newSpan = $('<span/>');
        var newSpanHtml = '';

        $newSpan.append($li.html());

        // Add new line break for each span
        newSpanHtml = $newSpan.html() + '<br/>';

        return newSpanHtml;
      });

      // Remove ol tag if ol doesn't contains li tag
      $div.find('ol').each(function (ind, ol) {
        var $ol = $(ol);

        if (!$ol.find('li').length) {
          $ol.remove();
        }
      });

      xmlContent = $div.html();
    }

    // LNKT-56399 fix <br> not display if its at last of <p> children
    var $wrapper = $('<div/>');
    $wrapper.append(xmlContent);
    $wrapper.find('br').each((idx, ele) => {
      if (ele.parentElement.tagName === 'P' && !ele.nextSibling) {
          ele.parentElement.append(document.createElement('br'));
      }
    })

    // If xmlContent have object tag type video (data import), convert it.
    $wrapper.find('object[type^="video/"]').each(function(idx, ele) {
      var $video = $('<video>', {
        class: 'editvideo',
        width: 320,
        height: 240,
        controls: true,
        autoplay: true,
        preload: 'metadata',
        oncontextmenu: 'return false',
        'data-cke-pa-oncontextmenu': 'return false'
      }).append(
        $('<source>', {
          src: ele.data,
          type: ele.type
        }),
        'Your browser does not support the video tag.'
      );
      
      var $span = $('<span>', {
        class: 'videoSpan',
        contenteditable: 'false'
      }).append($video);

      if (ele.parentElement.tagName === 'MEDIAINTERACTION') {
        $(ele.parentElement).replaceWith($span);
      } else {
        $(ele).replaceWith($span);
      }
    })

    xmlContent = $wrapper.html();

    // Make sure the properties match when replace
    CKEDITOR.instances["passageContent"].setData(xmlContent, function () {
      //This to make sure state of CKEditor is false after loaded data. Waiting 1s
      var that = this;
      window.setTimeout(function () {
        that.resetDirty();
        $("#divPassage iframe").contents().find("video").trigger("pause"); //Stop video after it has been added to passage editor
      }, 1000);

      //Correct image incase image has width or height is NaN
      var $getAllImageUpload = $("iframe.cke_wysiwyg_frame").contents().find("img.imageupload");
      $getAllImageUpload.each(function (index, currentImage) {
        $(currentImage).load(function () {
          var $currentImage = $(currentImage);
          var newHeight = $currentImage.height();
          var newWidth = $currentImage.width();
          if ($currentImage.attr('height') == undefined || $currentImage.attr('height').toString() == "NaN") {
            if ($currentImage.attr('percent') != undefined) {
              newHeight = (newHeight * parseInt($currentImage.attr('percent').toString() + "0")) / 100;
              $currentImage.attr({
                'height': newHeight
              });
            } else {
              $currentImage.attr({
                'height': newHeight
              });
            }
          }

          if ($currentImage.attr('width') == undefined || $currentImage.attr('width').toString() == "NaN") {
            if ($currentImage.attr('percent') != undefined) {
              newWidth = (newWidth * parseInt($currentImage.attr('percent').toString() + "0")) / 100;
              $currentImage.attr({
                'width': newWidth
              });
            } else {
              $currentImage.attr({
                'width': newWidth
              });
            }
          }
        });
      });
    });

    CKEDITOR.instances["passageContent"].resetDirty();
  }
}

//This function has detech mathml and return a new mathml when loaded to ckeditor
function loadMathML(strContent) {
  var mathML;

  if (strContent == "") {
    return "";
  }

  //Processing for MathML
  if (strContent !== undefined) {
    var $mathContainer = $('<div/>');

    $mathContainer.append(
      strContent.replace(/<math/g, '<span class="myMath"><math')
        .replace(/<\/math>/g, '</math></span>')
    );

    mathML = $mathContainer.find('.myMath');

    if (mathML.length > 0) {
      mathML.each(function (index, item) {
        var originalMath = $(this).html().replace('<?XML:NAMESPACE PREFIX = [default] http://www.imsglobal.org/xsd/imsqti_v2p0 NS = "http://www.imsglobal.org/xsd/imsqti_v2p0" />', '');
        originalMath = originalMath.replace('<?XML:NAMESPACE PREFIX = "[default] http://www.w3.org/1998/Math/MathML" NS = "http://www.w3.org/1998/Math/MathML" />', '');
        originalMath = originalMath.replace('<?XML:NAMESPACE PREFIX = [default] http://www.w3.org/1998/Math/MathML NS = "http://www.w3.org/1998/Math/MathML" />', '');
        var srcMathML = originalMath.replace(/"/g, '\\"').replace(/(\r\n|\n|\r)/gm, "")
          .replace(/<!--([^>]*?)-->/gm, "");
        var $parent = $(this).parent();
        var ttSpeech = $parent.attr("texttospeech");
        if (ttSpeech == undefined) {
          ttSpeech = "";
        }

        var strMathML = '<span><span class="cke_widget_wrapper cke_widget_inline" contenteditable="false" tabindex="-1" data-cke-widget-wrapper="1" data-cke-filter="off" data-cke-display-name="math" data-cke-widget-id="' + index + '">';
        strMathML += '<span class="math-tex cke_widget_element" data-cke-survive="1" style="display:inline-block" data-cke-widget-keep-attr="0" data-widget="mathjax" data-cke-widget-data=\'{"math":"' + srcMathML + '"}\' texttospeech="' + ttSpeech + '"></span>';
        strMathML += '<img class="cke_reset cke_widget_mask" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D"><span class="cke_reset cke_widget_drag_handler_container" ><img class="cke_reset cke_widget_drag_handler" height="15" width="15" data-cke-widget-drag-handler="1" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" title="Click and drag to move" draggable="true"></span></span></span>';

        //This is mathML for fraction
        if ($(this).find("math").attr("class") == "simpleFraction") {
          strMathML = '<span><span class="cke_widget_wrapper cke_widget_inline" contenteditable="false" tabindex="-1" data-cke-widget-wrapper="1" data-cke-filter="off" data-cke-display-name="math" data-cke-widget-id="' + index + '">';
          strMathML += '<span class="math-tex cke_widget_element" data-cke-survive="1" style="display:inline-block" data-cke-widget-keep-attr="0" data-widget="mathfraction" data-cke-widget-data=\'{"math":"' + srcMathML + '"}\' texttospeech="' + ttSpeech + '">';
          strMathML += '</span><img class="cke_reset cke_widget_mask" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D"><span class="cke_reset cke_widget_drag_handler_container" ><img class="cke_reset cke_widget_drag_handler" height="15" width="15" data-cke-widget-drag-handler="1" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" title="Click and drag to move" draggable="true"></span></span></span>';
        }

        if ($parent.hasClass("math-tex")) {
          if ($parent.attr("texttospeech") != undefined) {
            originalMath = '<span class="math-tex" texttospeech="' + $parent.attr("texttospeech") + '">' + originalMath + '</span>';
          } else {
            originalMath = '<span class="math-tex">' + originalMath + '</span>';
          }
        }

        //Make sure special charater transform to html character
        strContent = $("<div></div>").append(strContent).html().replace(new RegExp("\>[\n\t ]+\<", "g"), "><");
        strContent = strContent.replace('<?XML:NAMESPACE PREFIX = [default] http://www.imsglobal.org/xsd/imsqti_v2p0 NS = "http://www.imsglobal.org/xsd/imsqti_v2p0" />', '').replace('<?XML:NAMESPACE PREFIX = [default] http://www.w3.org/1998/Math/MathML NS = "http://www.w3.org/1998/Math/MathML" />', '');
        strContent = strContent.replace('<?XML:NAMESPACE PREFIX = "[default] http://www.w3.org/1998/Math/MathML" NS = "http://www.w3.org/1998/Math/MathML" />', '');
        strContent = strContent.replace('<?XML:NAMESPACE PREFIX = [default] http://www.w3.org/1998/Math/MathML NS = "http://www.w3.org/1998/Math/MathML" />', '');
        strContent = strContent.replace(originalMath.replace(new RegExp("\>[\n\t ]+\<", "g"), "><"), strMathML);
      });
    }
  }

  return strContent;
}

/**
 * Create CKEditor for question item type
 * 1: Multiple choice (Multiple or true/false)
 * 8: Inline choice
 * 9: Text entry
 * 10: Open ended
 * 10d: DrawTool
 * 21: Complex (multi-part)
 * 30: Drag and drop
 * 31: Text hot spot
 * 32: Image hot spot
 * 33: Table hot spot
 * 34: Number line hot spot
 * 35: Drag and drop numerical
 * @param  {[type]}  schemeId [description]
 * @param  {Boolean} isNew    [description]
 * @return {[type]}           [description]
 */
function loadEditItem(schemeId, isNew) {
  iResult = [];

  iSchemeID = schemeId;
  isAddnew = isNew;

  if (schemeId === '21') {
    checkSavingComplex = true;
    //Create checkbox for no duplicate answers
    var noDuplicateHTML = '<div class="noDuplicateHTML mt-2"><input type="checkbox" id="noDuplicate" /> <label class="ms-1" for="noDuplicate">No Duplicate Answers</label</div><div class="clear10"></div>';
    $('#duplicateAnswer').append(noDuplicateHTML);
  }

  createCKEditorIndex();

  // Destroy ckeditor tool re-create to correct with question type
  if (CKEDITOR.instances[ckID] !== undefined) {
    CKEDITOR.instances[ckID].destroy();
  }

  //Clear content after destroy CKEditor
  document.getElementById(ckID).value = '';

  // Build stylesheet contents.css for unstyled on Firefox
  CKEDITOR.tools.buildStyleHtml = function (css) {
    css = [].concat(css);
    var item,
      retval = [];
    for (var i = 0; i < css.length; i++) {
      if ((item = css[i])) {
        // Is CSS style text ?
        if (/@import|[{}]/.test(item)) {
          retval.push('<style>' + item + '</style>');
        } else {
          item += '?ts=' + new Date().getTime();
          retval.push('<link type="text/css" rel=stylesheet href="' + item + '">');
        }
      }
    }
    return retval.join('');
  };
  var txtTrueFalse = document.URL.split('&')[2];
  var ckConfig = {
    contentsCss: window.location.protocol + '//' + window.location.hostname + '/Content/themes/TestMaker/contentsV2.css' + (window.CKE_VERSION || ''),
    extraPlugins: 'indent,mathjax,sharedspace,mathfraction,boxedtext,video,glossary,tabspaces,messageguidancerationales,texttospeech,leaui_formula',
    alphaBeta: alphabet,
    playAudio: loadAudioUrl,
    qtiSchemeID: schemeId,
    height: 400,
    toolbar: [
      ['JustifyLeft', 'JustifyCenter', 'JustifyRight', '-', 'NumberedList', 'BulletedList'],
      ['Bold', 'Italic', 'Underline'],
      ['Sameline'],
      ['Mathfraction'],
      ['FontSize', 'Subscript', 'Superscript', '-', 'RemoveFormat'],
      ['Link', 'Unlink', 'Table', 'AudioUpload', 'Reference', 'ImageUpload', 'VideoUpload', 'SpecialChar', 'Mathjax', 'LeauiFormula', 'Glossary'],
      ['Indent', 'Outdent', 'Tabspaces'],
      ['BoxedText'],
      ['Texttospeech']
    ],
    sharedSpaces: {
      top: 'topSpace',
      bottom: 'bottomSpace'
    },
    extraAllowedContent: 'span[stylename]; span(smallText,normalText,largeText,veryLargeText);'
  };

  if (schemeId === '21') {
    ckConfig.extraPlugins += ',itemtypeonimage,textentry,inlinechoice,mathfraction,drawtool,extendtext,multiplechoice,dropdownsubpart,deleteplugin,dependentgrading';
    ckConfig.toolbar.push(['DeletePlugin'], ['DropdownSubpart'], ['ItemTypeOnImage']);
  } else if (schemeId === '30') {
    ckConfig.extraPlugins += ',partialaddsource,partialaddsourceimage,partialaddsourcetext,partialaddproperties,partialadddestinationtext,partialadddestinationimage';
    ckConfig.toolbar.push(['PartialAddSource']);
  } else if (schemeId === '31') {
    ckConfig.extraPlugins += ',texthotspot,texthotspotproperties';
    ckConfig.toolbar.push(['TextHotSpot'], ['TextHotSpotProperties']);
  } else if (schemeId === '32') {
    ckConfig.extraPlugins += ',imagehotspotselection';
    ckConfig.toolbar.push(['ImageHotSpotSelection']);
  } else if (schemeId === '33') {
    ckConfig.extraPlugins += ',tablehotspotselection';
    ckConfig.toolbar.push(['TableHotSpot'], ['GardingHotSpot']);
  } else if (schemeId === '34') {
    ckConfig.extraPlugins += ',numberlinehotspot';
    ckConfig.toolbar.push(['NumberLineHotSpot']);
  } else if (schemeId === '35') {
    ckConfig.extraPlugins += ',partialaddsource,dragdropnumericalsource,dragdropnumericaldestination,dragdropnumericalrelationship';
    ckConfig.toolbar.push(['PartialAddSource']);
  } else if (schemeId == '36') {
    //Extra config for number line hot spot
    ckConfig.extraPlugins += ",sequenceorder";
    ckConfig.toolbar.push(['SequenceOrder']);
  } else if (schemeId == '37') {
    //Extra config for number line hot spot
    ckConfig.extraPlugins += ",multiplechoicevariable";
    ckConfig.toolbar.push(['MultipleChoiceVariable']);
  } else if (schemeId == '10') {
    //Extra config for Inline Choices
    ckConfig.extraPlugins += ",extendtext";
    ckConfig.toolbar.push(['ExtendText']);
  } else if (schemeId == '10d') {
    //Extra config for Inline Choices
    ckConfig.extraPlugins += ",drawtool";
    ckConfig.toolbar.push(['DrawTool']);
  } else if (schemeId == '9') {
    //Extra config for Inline Choices
    ckConfig.extraPlugins += ",textentry,itemtypeonimage";
    ckConfig.toolbar.push(['Textentry'], ['ItemTypeOnImage']);
  } else if (schemeId == '8') {
    //Extra config for Inline Choices
    ckConfig.extraPlugins += ",inlinechoice,itemtypeonimage";
    ckConfig.toolbar.push(['InlineChoice'], ['ItemTypeOnImage']);
  } else if (schemeId == '3') {
    //Extra config for Inline Choices
    ckConfig.extraPlugins += ",multiplechoice";
    ckConfig.toolbar.push(['MultipleChoice']);
  } else if (schemeId == '1') {
    //Extra config for Inline Choices
    ckConfig.extraPlugins += ",multiplechoice";
    ckConfig.toolbar.push(['MultipleChoice']);
  } else {
    ckConfig.extraPlugins += ',textentry,inlinechoice,drawtool,extendtext,multiplechoice,dropdownsubpart,deleteplugin';
  }
  if (isSurveyTest && ((schemeId == '1' && txtTrueFalse != 'TrueFalse' && !isTrueFalse) || schemeId == '37' || schemeId == '10')) {
    ckConfig.toolbar[5] = ckConfig.toolbar[5].filter(item => item != 'Reference')
  }
  CKEDITOR.replace(ckID, ckConfig);
}

$.fn.ckOverlay = function (options) {
  // This is the easiest way to have default options.
  var settings = $.extend({
    // These are the defaults.
    msg: "Uploading ..."
  }, options);

  var width = this.outerWidth(),
    height = this.outerHeight(),
    that = this,
    //html = '<div class="blockUI blockOverlay" style="z-index: 11011; border: medium none; margin: 0px; padding: 0px; width: ' + width + 'px; height: ' + height + 'px; top: 0px; left: 0px; background-color: rgb(0, 0, 0); opacity: 0.6; cursor: wait; position: absolute; "></div><div class="blockUI blockMsg blockElement" style="z-index: 11012; position: absolute; padding: 15px; margin: 0px; width: 30%; top: ' + ((height / 2) - 15) + 'px; left: ' + ((width / 2) - (width * 15) / 100) + 'px; text-align: center; color: rgb(255, 255, 255); border: medium none; background-color: rgb(0, 0, 0); cursor: wait; opacity: 1;"><h2 style="color:#fff">' + settings.msg + '</h2></div>';
    html = '<div class="blockUI blockOverlay" style="z-index: 11011; border: medium none; margin: 0px; padding: 0px; width: ' + width + 'px; height: ' + height + 'px; top: 0px; left: 0px; background-color: rgb(0, 0, 0); opacity: 0.6; cursor: wait; position: fixed; "></div><div class="blockUI blockMsg blockElement" style="z-index: 11012; position: fixed; padding: 15px; margin: 0px; width: 30%; top: 50%; left: ' + ((width / 2) - (width * 15) / 100) + 'px; text-align: center; color: rgb(255, 255, 255); border: medium none; background-color: rgb(0, 0, 0); cursor: wait; opacity: 1;"><h2 style="color:#fff">' + settings.msg + '</h2></div>';
  this.css("position", "relative");
  this.append(html);

  $.fn.ckOverlay.destroy = function () {
    that.css({
      "position": ""
    });
    that.find(".blockUI").remove();
  }

  return this;
}

//convert string to xml
function formatXml(xml) {
  var formatted = '';
  var reg = /(>)(<)(\/*)/g;
  xml = xml.replace(reg, '$1\r\n$2$3');
  var pad = 0;
  jQuery.each(xml.split('\r\n'), function (index, node) {
    var indent = 0;
    if (node.match(/.+<\/\w[^>]*>$/)) {
      indent = 0;
    } else if (node.match(/^<\/\w/)) {
      if (pad != 0) {
        pad -= 1;
      }
    } else if (node.match(/^<\w[^>]*[^\/]>.*$/)) {
      indent = 1;
    } else {
      indent = 0;
    }

    var padding = '';
    for (var i = 0; i < pad; i++) {
      padding += '  ';
    }

    formatted += padding + node + '\r\n';
    pad += indent;
  });

  return formatted;
}

function insertAtCaret(areaId, text) {
  var txtarea = document.getElementById(areaId);
  var scrollPos = txtarea.scrollTop;
  var strPos = 0;
  var br = ((txtarea.selectionStart || txtarea.selectionStart == '0') ?
    "ff" : (document.selection ? "ie" : false));
  if (br == "ie") {
    txtarea.focus();
    var range = document.selection.createRange();
    range.moveStart('character', -txtarea.value.length);
    strPos = range.text.length;
  } else if (br == "ff") strPos = txtarea.selectionStart;

  var front = (txtarea.value).substring(0, strPos);
  var back = (txtarea.value).substring(strPos, txtarea.value.length);
  txtarea.value = front + text + back;
  strPos = strPos + text.length;
  if (br == "ie") {
    txtarea.focus();
    var range = document.selection.createRange();
    range.moveStart('character', -txtarea.value.length);
    range.moveStart('character', strPos);
    range.moveEnd('character', 0);
    range.select();
  } else if (br == "ff") {
    txtarea.selectionStart = strPos;
    txtarea.selectionEnd = strPos;
    txtarea.focus();
  }
  txtarea.scrollTop = scrollPos;
}

// set text default enter Enter Question…

/**
 * check responseid into iresult
 *
 */
function checkElementRemoveIntoIResult() {
  //get responseIdentifier into iResult
  if (iResult.length > 0) {
    if (currData.length > 0) {
      //Remove major
      for (var y = 0; y < currData.length; y++) {
        var curMajor = currData[y],
          isMajorExist = false;
        for (var x = 0; x < iResult.length; x++) {
          if (iResult[x].responseIdentifier == curMajor.major) {
            isMajorExist = true;
            break;
          }
        }
        if (isMajorExist == false) {
          currData.splice(y, 1);
          y = y - 1;
        }
      }

      //Remove depending in remain major;
      for (var y = 0; y < currData.length; y++) {
        var curDepending = currData[y].depending;
        for (var m = 0; m < curDepending.length; m++) {
          var isDependingExist = false;
          var curDependingItem = curDepending[m];
          for (var x = 0; x < iResult.length; x++) {
            if (iResult[x].responseIdentifier == curDependingItem) {
              isDependingExist = true;
              break;
            }
          }
          if (!isDependingExist) {
            currData[y].depending.splice(m, 1);
            m = m - 1;
          }
        }
      }

      //After remove major and depending. We clear array if Length of depending is 0.
      for (var y = 0; y < currData.length; y++) {
        if (currData[y].depending.length == 0) {
          currData.splice(y, 1);
          y = y - 1;
        }
      }
    }
  } else {
    currData = [];
  }
}

/****
    This function will be return to know this ResponseID is major or depending
****/
function returnMajorDepending(resId) {
  //Check depending and major
  var strMajor = "";
  for (var m = 0; m < currData.length; m++) {
    if (resId == currData[m].major) {
      strMajor = 'major="true"';
      break;
    } else {
      var isDepending = false;
      for (var k = 0; k < currData[m].depending.length; k++) {
        if (resId == currData[m].depending[k]) {
          strMajor = 'depending="' + currData[m].major + '"';
          isDepending = true;
          break;
        }
      }
      if (isDepending) {
        break;
      }
    }
  }
  return strMajor;
}

function createImportMajorDepending(currentElement) {
  //Processing to import major and depending
  var resId = $(currentElement).attr("identifier");

  if ($(currentElement).attr("major") != undefined && $(currentElement).attr("major").toString() == "true") {
    var isExistMajor = false;
    for (var t = 0; t < currData.length; t++) {
      if (resId == currData[t].major) {
        isExistMajor = true;
        break;
      }
    }
    if (!isExistMajor) {
      currData.push({
        "major": resId,
        "depending": []
      });
    }
  }

  if ($(currentElement).attr("depending") != undefined && $(currentElement).attr("depending").toString() != "") {
    if (currData.length != 0) {
      var hasAddedDepending = false;
      for (var t = 0; t < currData.length; t++) {
        if (currData[t].major == $(currentElement).attr("depending").toString()) {
          //Check if depending has exist or not
          if (currData[t].depending.length > 0) {
            var isCurrDataDepending = false;
            for (var d = 0; d < currData[t].depending.length; d++) {
              if (currData[t].depending[d] == resId) {
                isCurrDataDepending = true;
                break;
              }
            }
            //Only add depending if it doesn't exist
            if (!isCurrDataDepending) {
              currData[t].depending.push(resId);
            }
          } else {
            currData[t].depending.push(resId);
          }
          hasAddedDepending = true;
          break;
        }
      }
      if (!hasAddedDepending) {
        currData.push({
          "major": $(currentElement).attr("depending").toString(),
          "depending": [resId]
        });
      }
    } else {
      currData.push({
        "major": $(currentElement).attr("depending").toString(),
        "depending": [resId]
      });
    }
  }
}

/**
 * Create new src Identifier
 */
function createSrcIdentifier(sourcePartial) {
  var srcId = "SRC_" + (sourcePartial.length + 1);
  for (m = 0; m < sourcePartial.length; m++) {
    resId = "SRC_" + (m + 1);
    if (sourcePartial[m].srcIdentifier != resId) {
      var isOnlyOne = true;
      for (k = 0; k < sourcePartial.length; k++) {
        if (resId == sourcePartial[k].srcIdentifier) {
          isOnlyOne = false;
        }
      }

      if (isOnlyOne) {
        srcId = resId;
        break;
      }
    }
  }
  return srcId;
}

/**
 * refresh response id for textEntry and inline choice
 *
 * @data data of question content
 */
function refreshPartialCredit() {
  var data = $("<p></p>").append(CKEDITOR.instances[ckID].getData());
  var $data = $(data);
  var n = 0,
    h = 0,
    k = 0;
  //Get source from iResult to partialSource
  for (var i = 0; i < iResult.length; i++) {
    var iResultItem = iResult[i];

    if (iResultItem.type == "partialCredit") {
      //This is refresh Source object
      if ($(data).find(".partialSourceObject").length == 0) {
        iResultItem.source = [];
      } else {
        for (n = 0; n < iResultItem.source.length; n++) {
          if ($(data).find(".partialSourceObject[srcidentifier='" + iResultItem.source[n].srcIdentifier + "']").length == 0) {
            //update correctResponse to empty when source has removed
            for (k = 0; k < iResultItem.correctResponse.length; k++) {
              if (iResultItem.correctResponse[k].srcIdentifier == iResultItem.source[n].srcIdentifier) {
                iResultItem.correctResponse[k].srcIdentifier = "";
              }
            }

            iResultItem.source.splice(n, 1);
          }
        }
      }

      //This is refresh correctResponse and fixed for LNKT-26333
      if (iResultItem.correctResponse.length > 0) {
        for (var t = iResultItem.correctResponse.length; t--;) {
          var correntResponseImage = $(data).find(".partialDestinationObject .hotSpot[destidentifier='" + iResultItem.correctResponse[t].destIdentifier + "']");
          var correntResponse = $(data).find(".partialDestinationObject[destidentifier='" + iResultItem.correctResponse[t].destIdentifier + "']");
          if (correntResponse.length === 0 && correntResponseImage.length > 0) {
            correntResponse = correntResponseImage;
          }
          if (correntResponse.length === 0) {
            iResultItem.correctResponse.splice(t, 1);
          }
        }
      }

      //this is refresh destination object
      if ($(data).find(".partialDestinationObject").length == 0) {
        iResultItem.destination = [];
        iResultItem.correctResponse = [];
      } else {
        var tempIResult = JSON.parse(JSON.stringify(iResultItem));
        for (n = 0; n < iResultItem.destination.length; n++) {
          //This remove destination doesn't exist.
          var isExistDestination = false;
          $(data).find(".partialDestinationObject").each(function () {
            if ($(this).attr("destidentifier") == iResultItem.destination[n].destIdentifier || $(this).find(".hotSpot[destidentifier='" + iResultItem.destination[n].destIdentifier + "']").length != 0) {
              isExistDestination = true;
            }
          });

          if (isExistDestination == false) {
            tempIResult.destination.splice(n, 1);
          }
        }
        iResultItem = tempIResult;
      }

      if (iResultItem.correctResponse.length !== iResultItem.destination.length) {
        //this is refresh correctResponse
        if (iResultItem.correctResponse.length > iResultItem.destination.length) {
          for (h = 0; h < iResultItem.correctResponse.length; h++) {
            if ($(data).find('[destidentifier="' + iResultItem.correctResponse[h].destIdentifier + '"]').length == 0) {
              iResultItem.correctResponse.splice(h, 1);
            }
          }
        }

        if (iResultItem.correctResponse.length < iResultItem.destination.length) {
          for (h = 0; h < iResultItem.destination.length; h++) {
            var currentDes = iResultItem.destination[h].destIdentifier;
            var correctExist = false;
            for (var k = 0; k < iResultItem.correctResponse.length; k++) {
              if (currentDes === iResultItem.correctResponse[k].destIdentifier) {
                correctExist = true;
              }
            }

            if (!correctExist) {
              iResultItem.correctResponse.push({
                'destIdentifier': iResultItem.destination[h].destIdentifier,
                'order': iResultItem.destination[h].destIdentifier.replace('DEST_', ''),
                'srcIdentifier': '',
                'thresholdpoints': []
              });
            }
          }
        }
      }
    } else if (iResultItem.type == "dragDropNumerical") {
      // Refresh source object of drag and drop numerical
      if ($data.find('.partialSourceObject').length) {
        for (n = 0; n < iResultItem.source.length; n++) {
          var iResultSource = iResultItem.source[n];

          if (!$data.find('.partialSourceObject[srcidentifier="' + iResultSource.srcIdentifier + '"]').length) {
            // Remove source drag and drop numerical when it remove ckeditor
            iResultItem.source.splice(n, 1);
          }
        }
      } else {
        iResultItem.source = [];
      }

      //this is refresh destination object
      if ($data.find('.partialDestinationObject').length) {
        for (n = 0; n < iResultItem.destination.length; n++) {
          var iResultDest = iResultItem.destination[n];

          if (!$data.find('.partialDestinationObject[destidentifier="' + iResultDest.destIdentifier + '"]').length) {
            // Remove destination drag and drop numerical when it remove ckeditor
            iResultItem.destination.splice(n, 1);
          }
        }
      } else {
        iResultItem.destination = [];
      }
    }

    iResult[i] = iResultItem;
  };
}

/**
 * Create new src Identifier
 */
function createDestinationIdentifier(destinationPartial) {
  var srcId = "DEST_" + (destinationPartial.length + 1);
  for (m = 0; m < destinationPartial.length; m++) {
    resId = "DEST_" + (m + 1);
    if (destinationPartial[m].destIdentifier != resId) {
      var isOnlyOne = true;
      for (k = 0; k < destinationPartial.length; k++) {
        if (resId == destinationPartial[k].destIdentifier) {
          isOnlyOne = false;
        }
      }

      if (isOnlyOne) {
        srcId = resId;
        break;
      }
    }
  }
  return srcId;
}

//Function check editor has added new content or not.
function checkEditor(msg, ckEitorID) {
  var saved = true;

  if (CKEDITOR.instances[ckEitorID] != undefined) {
    if (CKEDITOR.instances[ckEitorID].checkDirty()) {
      saved = confirm(msg);
    }
  }

  if (saved == true) {
    return true;
  } else {
    return false;
  }
}

function destroyEditorInstance(key) {
  if (key != null) {
    if (key.trim().length > 0) {
      if (typeof (CKEDITOR.instances[key]) != "undefined") {
        CKEDITOR.instances[key].destroy();
      }
    }
  }
}

function replaceVideo(originalString) {
  if (originalString) {
    originalString = originalString.replace(/<video /g, "<videolinkit ")
      .replace(/<\/video>/g, "</videolinkit>")
      .replace(/<source /g, "<sourcelinkit ")
      .replace(/<\/source>/g, "</sourcelinkit>");

    var $xml = $('<div>' + originalString + '</div>');
    $xml.find('sourcelinkit').each(function (index, source) {
      if (source.parentElement.tagName.toLowerCase() === 'audio') {
        var newSource = document.createElement('source');
        Array.from(source.attributes).forEach(function (attr) {
          newSource.setAttribute(attr.name, attr.value);
        });
        newSource.innerHTML = source.innerHTML;
        source.replaceWith(newSource);
      }
    });

    return $xml.prop('innerHTML');
  }

  return originalString;
}

function unreplaceVideo(originalString) {
  if (originalString) {
    return originalString.replace(/<videolinkit /g, "<video ")
      .replace(/<\/videolinkit>/g, "</video>")
      .replace(/<sourcelinkit /g, "<source ")
      .replace(/<\/sourcelinkit>/g, "</source>");
  }

  return originalString;
}

//Function to refesh config incase use in many objectIDs
function refeshConfig() {
  if (MKEditor != undefined) {
    if (MKEditor.imgUrl != undefined) {
      imgConfig = MKEditor.imgUrl;
    }

    if (MKEditor.audioUrl != undefined) {
      audioConfig = MKEditor.audioUrl;
    }

    if (MKEditor.quesImageUrl != undefined) {
      quesImageUrl = MKEditor.quesImageUrl;
    }

    if (MKEditor.objectId != undefined) {
      objectId = MKEditor.objectId;
    }

    if (MKEditor.imgUpload != undefined) {
      imgUpload = MKEditor.imgUpload;
    }

    if (MKEditor.loadAudioUrl != undefined) {
      loadAudioUrl = MKEditor.loadAudioUrl;
    }

    //MKEditor.UseS3Content = true; //Set S3 content always true for test purpose only
    //MKEditor.GetViewReferenceImgS3 = 'https://s3.amazonaws.com/testitemmedia/Vina';

    if (MKEditor.GetViewReferenceImg != undefined) {
      GetViewReferenceImg = MKEditor.GetViewReferenceImg;
    }

    //This use for S3 link when upload image, audio
    if (MKEditor.UseS3Content != undefined && MKEditor.UseS3Content === true) {
      if (MKEditor.GetViewReferenceImgS3 != undefined) {
        GetViewReferenceImg = MKEditor.GetViewReferenceImgS3;
      }
    }

    if (MKEditor.videoUrl != undefined) {
      videoConfig = MKEditor.videoUrl;
    }
    isPassageEditor = false;
    if (MKEditor.isPassageEditor != undefined) {
      isPassageEditor = MKEditor.isPassageEditor;
    }

    if (MKEditor.isSurveyTest != undefined) {
      isSurveyTest = MKEditor.isSurveyTest;
    }

  }
}

function eleExportbyClass(ele, skipEle) {
  var $ele = $(ele);
  var tempContent = $ele.html();

  //Processing for same line
  if ($ele.hasClass("nobreak") && skipEle != "nobreak") {
    tempContent = '<sameline class="nobreak" stylename="nobreak">' + tempContent + '</sameline>';
  }

  //Processing for bold
  if ($ele.hasClass("bold") && skipEle != "bold") {
    tempContent = "<strong>" + tempContent + "</strong>";
  }

  //Processing for italic
  if ($ele.hasClass("italic") && skipEle != "italic") {
    tempContent = "<em>" + tempContent + "</em>";
  }

  //Processing for underline
  if ($ele.hasClass("underline") && skipEle != "underline") {
    tempContent = "<u>" + tempContent + "</u>";
  }

  //Processing for small Text
  if ($ele.hasClass("smallText") && skipEle != "smallText") {
    tempContent = '<span class="smallText" stylename="smallText">' + tempContent + '</span>';
  }

  //Processing for small Text
  if ($ele.hasClass("largeText") && skipEle != "largeText") {
    tempContent = '<span class="largeText" stylename="largeText">' + tempContent + '</span>';
  }

  //Processing for small Text
  if ($ele.hasClass("normalText") && skipEle != "normalText") {
    tempContent = '<span class="normalText" stylename="normalText">' + tempContent + '</span>';
  }

  //Processing for small Text
  if ($ele.hasClass("veryLargeText") && skipEle != "veryLargeText") {
    tempContent = '<span class="veryLargeText" stylename="veryLargeText">' + tempContent + '</span>';
  }

  //Processing for small Text
  if ($ele.hasClass("sub") && skipEle != "sub") {
    tempContent = '<sub>' + tempContent + '</sub>';
  }

  //Processing for small Text
  if ($ele.hasClass("sup") && skipEle != "sup") {
    tempContent = '<sup>' + tempContent + '</sup>';
  }

  return tempContent;
}

function cleanXmlContent(oldXmlContent) {
  var newXmlContent = '';
  newXmlContent = oldXmlContent.replace(/Enter Question&hellip;/g, "")
    .replace(/<span class="placeholder" contenteditable="false" style="position: absolute;z-index: 99999;"><\/span>/g, "")
    .replace(/<p class="no-hover">&nbsp;<\/p>/g, "<p>&nbsp;</\p>");

  return newXmlContent;
}

/***
 * build style table hot spot
 ***/
function refreshIdHotSpot(iResult) {
  var stringHtml = '<div>' + CKEDITOR.instances[ckID].getData() + '</div>';
  var totalResIdHotSpot = [];

  $(stringHtml).find("span[typehotspot]").each(function () {
    totalResIdHotSpot.push({
      responseId: $(this).attr("identifier")
    });
  });

  for (var i = 0, iresultLen = iResult.length; i < iresultLen; i++) {
    if (iResult[i].type === 'tableHotSpot') {
      //This case happen if user removed all Response Id
      if (totalResIdHotSpot.length === 0) {
        iResult[0].sourceHotSpot.arrayList = [];
        iResult[0].correctResponse = [];
      } else {
        //Loop to remove ResponseId don't exist in current Editor
        var tempIResult = [];
        var tempCorrect = [];

        for (var n = 0, arrayListLen = iResult[0].sourceHotSpot.arrayList.length; n < arrayListLen; n++) {
          for (var i = 0; i < totalResIdHotSpot.length; i++) {
            //if responseIdentifier exist return true;
            if (iResult[0].sourceHotSpot.arrayList[n].identifier === totalResIdHotSpot[i].responseId) {
              tempIResult.push(iResult[0].sourceHotSpot.arrayList[n]);
              break;
            }
          }
        }

        iResult[0].sourceHotSpot.arrayList = tempIResult;
        for (var j = 0, currarrayListLen = iResult[0].sourceHotSpot.arrayList.length; j < currarrayListLen; j++) {
          for (var i = 0, correctResponseLen = iResult[0].correctResponse.length; i < correctResponseLen; i++) {
            if (iResult[0].sourceHotSpot.arrayList[j].identifier === iResult[0].correctResponse[i].identifier) {
              tempCorrect.push(iResult[0].correctResponse[i]);
              break;
            }
          }
        }
        iResult[0].correctResponse = tempCorrect;
      }
    }
  }
}

/**
 * update font size style for inline choice
 */
function getStyleFontSizeInlineChoice(eleFontSize) {
  var eleClass = '';
  if (eleFontSize.hasClass('smallText') || eleFontSize.hasClass('normalText') || eleFontSize.hasClass('largeText') || eleFontSize.hasClass('veryLargeText')) {
    eleClass = $(eleFontSize).attr('class');
    switch (eleClass) {
      case 'smallText':
        eleClass = 'Small';
        break;
      case 'normalText':
        eleClass = 'Normal';
        break;
      case 'largeText':
        eleClass = 'Large';
        break;
      case 'veryLargeText':
        eleClass = 'X-Large';
        break;
    }
  }
  return eleClass;
}
/**
 * convert format font size for inline choice
 */
function convertStyleFontSizeInlineChoice(styleFontSize) {
  var styleFont = '';
  switch (styleFontSize) {
    case 'Small':
      styleFont = 'smallText';
      break;
    case 'Normal':
      styleFont = 'normalText';
      break;
    case 'Large':
      styleFont = 'largeText';
      break;
    case 'X-Large':
      styleFont = 'veryLargeText';
      break;
  }

  return styleFont;
}
/**
 * apply format font size for inline choice by single click
 */
function applyFontSizeInlineChoiceBySingleClick(editor, editorTable, valFontSize) {
  var tagBody = editorTable;
  if ($(tagBody.$).find('span').hasClass('inlineChoiceInteraction')) {
    var tagsInlineChoice = $(tagBody.$).find('span.inlineChoiceInteraction');
    for (var i = 0, lenTagsInlineChoice = tagsInlineChoice.length; i < lenTagsInlineChoice; i++) {
      var tagInlineChoice = tagsInlineChoice[i];
      if ($(tagInlineChoice).hasClass('typeFontSize')) {
        var idItemInlineChoice = $(tagInlineChoice).attr('id');
        var tagCurrentInlineChoice = $(tagBody.$).find('#' + idItemInlineChoice);

        var idTagCurrentInlineChoice = tagCurrentInlineChoice.attr('id');
        var range = editor.createRange();

        isApplyInlineChoice = true;

        switch (valFontSize) {
          case 'Small':
            if (tagCurrentInlineChoice.parent('span[stylename]').length) {
              var styleFontSize = convertStyleFontSizeInlineChoice(valFontSize);
              tagCurrentInlineChoice.parent().attr('class', styleFontSize);
              tagCurrentInlineChoice.parent().attr('stylename', styleFontSize);
            } else {
              tagCurrentInlineChoice.wrap('<span class="smallText" stylename="smallText"></span>');
            }
            break;
          case 'Normal':
            if (tagCurrentInlineChoice.parent('span[stylename]').length) {
              var styleFontSize = convertStyleFontSizeInlineChoice(valFontSize);
              tagCurrentInlineChoice.parent().attr('class', styleFontSize);
              tagCurrentInlineChoice.parent().attr('stylename', styleFontSize);
            } else {
              tagCurrentInlineChoice.wrap('<span class="normalText" stylename="normalText"></span>');
            }

            break;
          case 'Large':
            if (tagCurrentInlineChoice.parent('span[stylename]').length) {
              var styleFontSize = convertStyleFontSizeInlineChoice(valFontSize);
              tagCurrentInlineChoice.parent().attr('class', styleFontSize);
              tagCurrentInlineChoice.parent().attr('stylename', styleFontSize);
            } else {
              tagCurrentInlineChoice.wrap('<span class="largeText" stylename="largeText"></span>');
            }

            break;
          case 'X-Large':
            if (tagCurrentInlineChoice.parent('span[stylename]').length) {
              var styleFontSize = convertStyleFontSizeInlineChoice(valFontSize);
              tagCurrentInlineChoice.parent().attr('class', styleFontSize);
              tagCurrentInlineChoice.parent().attr('stylename', styleFontSize);
            } else {
              tagCurrentInlineChoice.wrap('<span class="veryLargeText" stylename="veryLargeText"></span>');
            }
            break;
        }

        range.setStart(editor.document.getById(idTagCurrentInlineChoice), 0);
        range.setEnd(editor.document.getById(idTagCurrentInlineChoice), 1);
        editor.getSelection().selectRanges([range]);
        isShowPanelFontSize = true;

        $('.editorArea .cke_combo__fontsize').find('a .cke_combo_text').text(valFontSize);
        $('.editorArea .cke_combo__fontsize').find('.cke_combo_label').text(valFontSize);

        $('iframe[allowtransparency]').contents().find('body').trigger('blur');

        if (!(navigator.userAgent.indexOf('AppleWebKit') > -1)) {
          applySelectedFontSize(valFontSize);
        }
      }
    }
  }
}
//reset default font size for inline choice when save or update item editor
function resetDefaultFontSize() {
  var tagBody = $('iframe[allowtransparency]').contents().find('body');
  $('.editorArea .cke_combo__fontsize').find('a .cke_combo_text').text('Normal');
  $('.editorArea .cke_combo__fontsize').find('.cke_combo_label').text('Normal');
  tagBody.find('span.inlineChoiceInteraction').removeClass('typeFontSize');
  tagBody.find('span.inlineChoiceInteraction').removeClass('active-border');
  $('iframe[allowtransparency]').contents().find('body').trigger('blur');
}
//apply selected font size for panel when choice inline choice
function applySelectedFontSize(valFontSize) {
  var tagsLiPanel = $('.cke_combopanel').find('.cke_panel_frame').contents().find('div[title="Font Size"] .cke_panel_listItem');
  tagsLiPanel.removeClass('cke_selected');
  tagsLiPanel.find('a').removeAttr('aria-selected');

  for (var i = 0, lenTagsLiPanel = tagsLiPanel.length; i < lenTagsLiPanel; i++) {
    var itemLi = tagsLiPanel[i];

    if ($(itemLi).find('a span[stylename]').text() === valFontSize) {
      $(itemLi).addClass('cke_selected');
      $(itemLi).find('a').prop('aria-selected', true);
      $(itemLi).find('a').trigger('focus');
    } else {
      $(itemLi).find('a').removeAttr('aria-selected');
      $(itemLi).find('a').removeAttr('_cke_focus');
    }
  }
  $('iframe[allowtransparency]').contents().find('body').trigger('blur');
}
//buidl html content include into answer choice
function includeHtmlGuidance(itemMessage, typeMessage) {
  var typeMessageContent = '';
  var wContent = 415;
  var hContent = 200;

  valueContent = itemMessage.valueContent; //.replace(new RegExp('Your browser does not support the video tag.', "g"), '</source> Your browser does not support the video tag.');
  itemMessage.valueContent = replaceVideo(valueContent);

  if (itemMessage.audioRef === '') {
    typeMessageContent += "<div style='border: 1px solid #999;padding-top: 5px; padding-left: 5px;z-index: 99999;display: none; position: absolute;background: #fff;' class='" + typeMessage + "' typemessage='" + typeMessage + "'>";
    typeMessageContent += "<div style='overflow: auto; max-width:" + wContent + "px; max-height:" + hContent + "px;' class='contentGuidance'>" + itemMessage.valueContent + "</div>";
  } else {
    typeMessageContent += "<div style='border: 1px solid #999;padding-top: 5px; padding-left: 5px;z-index: 99999;display: none; position: absolute;background: #fff;' class='" + typeMessage + "' typemessage='" + typeMessage + "'>";
    typeMessageContent += "<div class='audioIcon'>";
    typeMessageContent += '<img alt="Play audio" class="imageupload bntPlay" src="../../Content/themes/TestMaker/images/small_audio_play.png" title="Play audio">';
    typeMessageContent += '<img alt="Stop audio" class="bntStop" src="../../Content/themes/TestMaker/plugins/multiplechoice/images/small_audio_stop.png" title="Stop audio">';
    typeMessageContent += "<span class='audioRef'>" + itemMessage.audioRef + "</span></div>";
    typeMessageContent += "<div style='padding-left: 10px; overflow: auto; max-width:" + wContent + "px; max-height:" + hContent + "px;' class='contentGuidance'>" + itemMessage.valueContent + "</div>";
  }

  typeMessageContent += "</div>";

  return typeMessageContent;
}

/**
 * Build xml for guidance, rationale apply into xmlContent
 * @param  {[type]} itemMessage [description]
 * @param  {[type]} typeMessage [description]
 * @param  {[type]} identifier  [description]
 * @return {[type]}             [description]
 */
function xmlGuidance(itemMessage, typeMessage, identifier) {
  var typeMessageContent = '';

  if (itemMessage.audioRef === '') {
    typeMessageContent += "<div identifier='" + identifier + "' style='display: none;' class='" + typeMessage + "' typemessage='" + typeMessage + "'>";
  } else {
    typeMessageContent += "<div identifier='" + identifier + "' style='display: none;' audioRef='" + itemMessage.audioRef + "' class='" + typeMessage + "' typemessage='" + typeMessage + "'>";
  }

  itemMessage.valueContent = replaceSpecialCharacter(itemMessage.valueContent);
  typeMessageContent += itemMessage.valueContent;
  typeMessageContent += '</div>';

  return typeMessageContent;
}

//put data guidance and rationale into iResult
function putDataIntoIResult(arrMessageGuidance, itemTypeMessage, typeMessage) {
  var htmltypeMessage = '';
  var audioTypeMessage = '';
  var valueContentTypeMessage = '';
  var objTypeMessage = '';

  var tagTypeMesssage = $(itemTypeMessage).find('.' + typeMessage);

  if (tagTypeMesssage.attr('audioRef') != undefined) {
    audioTypeMessage = tagTypeMesssage.attr('audioRef');
  }
  if (tagTypeMesssage.html() != '') {
    valueContentTypeMessage = tagTypeMesssage.html();
  }

  objTypeMessage = {
    audioRef: audioTypeMessage,
    valueContent: valueContentTypeMessage,
    typeMessage: typeMessage
  };

  htmltypeMessage = includeHtmlGuidance(objTypeMessage, typeMessage);
  arrMessageGuidance.push(objTypeMessage);

  return htmltypeMessage;
}

function leftSlash(str) {
  if (str.charAt(0) === '/') {
    str = str.slice(1);
  }

  return str;
}

function rightSlash(str) {
  if (str.slice(-1) !== '/') {
    str += '/';
  }

  return str;
}

/**
 * Replace special character and paragraph empty
 * @param  {[type]} str [description]
 * @return {[type]}     [description]
 */
function replaceSpecialCharacter(str) {
  if (str === undefined) {
    return '';
  }

  return str.replace(/(\r\n|\n|\r)/gm, '')
    .replace(new RegExp('\>[\n\t]+\<', 'g'), '><')
    .replace(/<p><\/p>$/g, '');
}

/**
 * Processing for ol to <list></list> Must be replace before convert xml to html by jquery
 * because tag name will be change to lowercase
 * @param  {[type]} str [description]
 * @return {[type]}     [description]
 */
function replaceListTool(str) {
  return str.replace(/<list listStylePosition="outside" listStyleType="decimal" paragraphSpaceAfter="12" styleName="passageNumbering"><listMarkerFormat><ListMarkerFormat color="#aaaaaa" paragraphEndIndent="20"\/><\/listMarkerFormat>/gi, '<ol>')
    .replace(/<list listStylePosition="outside" listStyleType="decimal" paragraphSpaceAfter="12" styleName="passageNumbering"><listMarkerFormat><ListMarkerFormat color="#aaaaaa" paragraphEndIndent="20" \/><\/listMarkerFormat>/gi, '<ol>')
    .replace(/<\/list>/g, '</ol>')
    .replace(/list style=/g, 'ol style=')
    .replace(/list start=/g, 'ol start=');
}

/**
 * Replace item type on image mask
 * @param  {[type]} xml [description]
 * @return {[type]}     [description]
 */
function replaceItemTypeOnImageMark(xml) {
  var $xml = $(xml);
  var html = '';

  $xml.find('.itemtypeonimageMark').remove();

  html = $xml.prop('outerHTML');

  return html;
}

/**
 * Max value of object array by key
 * @param  {[type]} arr [description]
 * @param  {[type]} key [description]
 * @return {[type]}     [description]
 */
function maxOfArray(arr, key) {
  return Math.max.apply(Math, arr.map(function (o) {
    return o[key];
  }));
}

/**
 * Sum of array
 * @param  {[type]} arr [description]
 * @return {[type]}     [description]
 */
function sumOfArray(arr) {
  return arr.reduce(function (a, b) {
    return a + b;
  }, 0);
}

/**
 * get algorithmic Configuration
 */
function getAlgorithmicConfiguration(xmlContent, qtiSchemaId) {
  qtiSchemaId = parseInt(qtiSchemaId, 10);
  if (qtiSchemaId === 1 || qtiSchemaId === 3 || qtiSchemaId === 8 ||
    qtiSchemaId === 9 || qtiSchemaId === 37) {
    var isAlgorithmicGrading = xmlContent.indexOf('method="algorithmic"') != -1 ? true : false;
  } else if (qtiSchemaId === 30 || qtiSchemaId === 31 || qtiSchemaId === 32 || qtiSchemaId === 33 ||
    qtiSchemaId === 34 || qtiSchemaId === 35 || qtiSchemaId === 36) {
    var isAlgorithmicGrading = xmlContent.indexOf('algorithmicgrading="1"') != -1 ? true : false;
  }
  return isAlgorithmicGrading;
}

function replaceParagraph(str) {
  return str.replace(/<p>/g, '<div>')
    .replace(/<p /g, '<div ')
    .replace(/<\/p>/g, '</div>');
}

function dblickHandlerToolbar(editor) {
  var commands = [
    'removeFormat', 'mathfraction', 'numberedlist', 'bulletedlist',
    'bold', 'italic', 'underline', 'strike',
    'subscript', 'superscript', 'sameline', 'specialchar',
    'mathjax', 'leauiFormulaDialog'
  ];

  commands.forEach(function (command) {
    editor.getCommand(command).setState(CKEDITOR.TRISTATE_OFF);
  });
}

function getTristateCommand(editor, commands, isState) {
  var state = CKEDITOR.TRISTATE_OFF;

  if (isState) {
    state = CKEDITOR.TRISTATE_DISABLED;
  }

  commands.forEach(function (command) {
    if (editor.getCommand(command)) {
      editor.getCommand(command).setState(state);
    }
  });
}

function getTristateStatus(el, parts) {
  var status = false;
  var parents = el.getParents();

  for (var i = 0; i < parts.length; i++) {
    var part = parts[i];

    if (el.hasClass(part)) {
      break;
    } else {
      for (var j = 0; j < parents.length; j++) {
        var parent = parents[j];
        var parentClass = parent.$.className;

        if (parentClass.indexOf(part) > -1) {
          status = true;
          break;
        }
      }

      if (status) {
        break;
      }
    }
  }

  return status;
}

function tristateStatusHandler(element, editor) {
  var newSubPart = [
    'multipleChoice', 'inlineChoiceInteraction', 'textEntryInteraction',
    'extendText', 'drawTool', 'partialSourceObject',
    'partialDestinationObject', 'imageHotspotInteraction', 'hotspot-circle',
    'hotspot-checkbox', 'numberline-selection', 'sequenceBlock'
  ];

  var commandName = [
    'removeFormat', 'mathfraction', 'numberedlist', 'bulletedlist',
    'bold', 'italic', 'underline', 'strike',
    'subscript', 'superscript', 'sameline', 'specialchar',
    'mathjax', 'leauiFormulaDialog'
  ];

  var isHasSubPart = false;

  if (element != null) {
    isHasSubPart = getTristateStatus(element, newSubPart);
  }

  getTristateCommand(editor, commandName, isHasSubPart);
}

function escapeBasicStyles(chr) {
  return chr
    .replace(/<strong>/g, '<span styleName="bold" class="bold">')
    .replace(/<\/strong>/g, '</span>')
    .replace(/<em>/g, '<span styleName="italic" class="italic">')
    .replace(/<\/em>/g, '</span>')
    .replace(/<u>/g, '<span styleName="underline" class="underline">')
    .replace(/<\/u>/g, '</span>');
}

function escapeBasicStylesFromXml(xml) {
  // Processing for bold
  $(xml).find('span.bold').each(function () {
    var spanOldBold = $(this).prop('outerHTML');
    var newContent = eleExportbyClass(spanOldBold, 'bold');
    var newBold = '<strong>' + newContent + '</strong>';

    xml = xml.replace(spanOldBold, newBold);
  });

  // Processing for italic
  $(xml).find('span.italic').each(function () {
    var spanOldItalic = $(this).prop('outerHTML');
    var newContent = eleExportbyClass(spanOldItalic, 'italic');
    var newEm = '<em>' + newContent + '</em>';

    xml = xml.replace(spanOldItalic, newEm);
  });

  // Processing for underline
  $(xml).find('span.underline').each(function () {
    var spanOldUnderline = $(this).prop('outerHTML');
    var newContent = eleExportbyClass(spanOldUnderline, 'underline');
    var newUnderline = '<u>' + newContent + '</u>';

    xml = xml.replace(spanOldUnderline, newUnderline);
  });

  return xml;
}

function unescapeBasicStylesFromXml(xml) {
  // Processing for bold
  $(xml).find('span.bold').each(function () {
    var $bold = $(this);
    var spanOldBold = $bold.prop('outerHTML');
    var spanBold = $bold.removeAttributes().prop('outerHTML');
    var newBold = spanBold.replace('<span>', '<strong>').replace(/<\/span>$/, '</strong>');

    xml = xml.replace(spanOldBold, newBold);
  });

  // Processing for italic
  $(xml).find('span.italic').each(function () {
    var $italic = $(this);
    var spanOldItalic = $italic.prop('outerHTML');
    var spanItalic = $italic.removeAttributes().prop('outerHTML');
    var newItalic = spanItalic.replace('<span>', '<em>').replace(/<\/span>$/, '</em>');

    xml = xml.replace(spanOldItalic, newItalic);
  });

  // Processing for underline
  $(xml).find('span.underline').each(function () {
    var $underline = $(this);
    var spanOldUnderline = $underline.prop('outerHTML');
    var spanUnderline = $underline.removeAttributes().prop('outerHTML');
    var newUnderline = spanUnderline.replace('<span>', '<u>').replace(/<\/span>$/, '</u>');

    xml = xml.replace(spanOldUnderline, newUnderline);
  });

  return xml;
}

function createFileTypeGroupXmls(attachmentSetting, assessmentArtifactFileTypeGroups) {
  var fileTypeGroupXmls = attachmentSetting.fileTypeGroupNames
    .filter(function(name) { return assessmentArtifactFileTypeGroups.some(function(f) { return f.name.toLowerCase() == name.toLowerCase(); })})
    .map(function(fileType) { return `<fileTypeGroup name="${fileType.toLowerCase()}"/>`; });

  return fileTypeGroupXmls;
}

function createRecordingOptionXmls(attachmentSetting) {

  var recordingOptionXmls = attachmentSetting.recordingOptionKeys
    .map(function(optionKey) {
      return `${optionKey}="true"`
    });

  return recordingOptionXmls;
}

function createAttachmentConfigurationXml(attachmentSetting) {
  if (!TestMakerComponent.assessmentArtifactConfiguration.showSetting) {
    return "";
  }

  var fileTypeGroupXmls = createFileTypeGroupXmls(attachmentSetting, TestMakerComponent.assessmentArtifactConfiguration.assessmentArtifactFileTypeGroups);
  var recordingOptionXmls = createRecordingOptionXmls(attachmentSetting);

  return "<attachmentSetting " +
    `allowStudentAttachment=\"${attachmentSetting.allowStudentAttachment}\" ` +
    `${recordingOptionXmls.join(' ')} ` +
    `requireAttachment=\"${attachmentSetting.requireAttachment}\" ` +
    ">"
    + fileTypeGroupXmls.join('\n')
    +"</attachmentSetting>";
}

function getTag(xmlContent, tagName) {
  return $(xmlContent)[0].getElementsByTagName(tagName);
}

function getBoolAttributeValue(tag, attributeName, defaultValue) {
  if (!$(tag).length)
    return { name : attributeName, value: defaultValue };

  var value = $(tag)[0].getAttribute(attributeName);
  if (!value)
    return { name : attributeName, value: defaultValue };

  var boolValue = value == 'true' || value == 'True' ? true : false;
  return { name: attributeName, value: boolValue };
}


function getAttributeValue(tag, attributeName) {
  if (!$(tag).length)
    return { name : attributeName, value: undefined };

  var value = $(tag)[0].getAttribute(attributeName);
  return { name: attributeName, value: value };
}

function getAttachmentSetting(xmlContent) {
  if (!TestMakerComponent.assessmentArtifactConfiguration.showSetting) {
    return {
      allowStudentAttachment: false,
      requireAttachment: false,
      recordingOptionKeys: [],
      fileTypeGroupNames: []
    };
  }

  var getTagResult = getTag(xmlContent, 'attachmentSetting');
  var attachmentSettingTag = getTagResult.length ? getTagResult[0] : '';

  var defaultValue = false;

  var allowStudentAttachment =  getBoolAttributeValue(attachmentSettingTag, 'allowStudentAttachment', defaultValue).value;

  var requireAttachment =  getBoolAttributeValue(attachmentSettingTag, 'requireAttachment', defaultValue).value;

  var recordingOptionKeys = TestMakerComponent.assessmentArtifactConfiguration.recordingOptions.map(function(option) {
    return option.name;
  });

  recordingOptionKeys = recordingOptionKeys.filter(function(optionKey) {
    var allowed = getBoolAttributeValue(attachmentSettingTag, optionKey, defaultValue).value;
    return allowed;
  });

  var fileTypeGroupTags = attachmentSettingTag !== '' ? getTag(attachmentSettingTag, 'fileTypeGroup') : [];
  var assessmentArtifactFileTypeGroups = TestMakerComponent.assessmentArtifactConfiguration.assessmentArtifactFileTypeGroups;
  var fileTypeGroupNames = [];

  for (var i = 0; i <  fileTypeGroupTags.length; i++) {
    var name = getAttributeValue(fileTypeGroupTags[i], 'name').value;
    if (!!name && assessmentArtifactFileTypeGroups.some(function(f) { return f.name.toLowerCase() === name.toLowerCase()})) {
      fileTypeGroupNames.push(name.toLowerCase());
    }
  }

  var attachmentSettingResult = {
    allowStudentAttachment,
    requireAttachment,
    recordingOptionKeys,
    fileTypeGroupNames
  };

  return attachmentSettingResult;
}
function setModeMultiPartGrading(xmlContent, schemeID) {
  if (schemeID != 21) return
  var parser = new DOMParser();
  var xmlDoc = parser.parseFromString(xmlContent, "text/xml");
  // Get the multiPartGradingSetting node
  var multiPartGradingSettingNode = xmlDoc.getElementsByTagName("multiPartGradingSetting")[0];

  // Get the mode attribute value
  if (multiPartGradingSettingNode) {
    var mode = multiPartGradingSettingNode.getAttribute("mode");
    var allOrNothingGrading = multiPartGradingSettingNode.getAttribute("allOrNothingGrading");
    if (mode) {
      modeMultiPartGrading = mode
    }
    if (allOrNothingGrading) {
      allOrNothingGradingScore = allOrNothingGrading
    }
  }
}


function childrenMovable(doc) {
  var container = doc.querySelector('.children_movable');
  if (!container) return;

  var currentElement = null;
  var offsetX = 0;
  var offsetY = 0;

  // Function to update the container's min-width and min-height
  function updateContainerSize() {
    var maxRight = 0;
    var maxBottom = 0;

    var children = container.childNodes;
    children.forEach(function(child) {
      if (!child.getBoundingClientRect) return;
      var rect = child.getBoundingClientRect();
      var containerRect = container.getBoundingClientRect();

      var right = rect.left - containerRect.left + rect.width;
      var bottom = rect.top - containerRect.top + rect.height;

      maxRight = Math.max(maxRight, right);
      maxBottom = Math.max(maxBottom, bottom);
    });

    container.style.minWidth = `${maxRight}px`;
    container.style.minHeight = `${maxBottom}px`;
  }

  // Mouse down: Start dragging
  container.addEventListener('mousedown', function(e) {
    var child = e.target;
    while(child !== null && child.parentElement !== container) {
      child = child.parentElement;
    }
    if (child && container === child.parentElement) {
      currentElement = child;
      var rect = currentElement.getBoundingClientRect();
      offsetX = e.clientX - rect.left;
      offsetY = e.clientY - rect.top;
    }
  });

  // Mouse move: Update position
  doc.addEventListener('mousemove', function(e) {
    if (currentElement) {
      var containerRect = container.getBoundingClientRect();
      var newLeft = e.clientX - containerRect.left - offsetX;
      var newTop = e.clientY - containerRect.top - offsetY;

      currentElement.style.left = `${newLeft}px`;
      currentElement.style.top = `${newTop}px`;

      // Update container size dynamically
      updateContainerSize();
    }
  });

  // Mouse up: Stop dragging
  doc.addEventListener('mouseup', function() {
    if (currentElement) {
      currentElement = null;
    }
  });

  // Initial update of container size
  updateContainerSize();
}

function replaceDivWithKeepDiv(html) {
  let regex = /<div\b(.*?)>/gi;
  let stack = [];
  let result = "";
  let lastIndex = 0;

  while ((match = regex.exec(html)) !== null) {
    let divTag = match[0];
    let attributes = match[1];

    // Check if it's the start of "videoSpan audio-container" div
    if (attributes.includes('class="videoSpan audio-container"') || attributes.includes("class='videoSpan audio-container'")) {
      stack.push(true); // Start tracking this div block
    } else if (stack.length > 0) {
      stack.push(false); // Nested div inside, also replace
    }

    // Append previous content
    result += html.substring(lastIndex, match.index);

    // Replace <div> with <keepdiv> if inside videoSpan audio-container
    if (stack.length > 0) {
      result += `<keepdiv${attributes}>`;
    } else {
      result += divTag; // Keep normal <div> unchanged
    }

    lastIndex = regex.lastIndex;
  }

  // Append remaining content
  result += html.substring(lastIndex);

  // Now replace closing </div> for nested elements
  result = result.replace(/<\/div>/g, () => {
    return stack.length > 0 ? "</keepdiv>" : "</div>";
  });

  return result;
}
