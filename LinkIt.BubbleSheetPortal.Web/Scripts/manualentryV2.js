var answerList;
var focusoutTime;

$('#manEntryGuide').hide();

function buildAnswerList() {
    var all = $(".radioButtonAnswer");

    answerList = Array();
    var answerListCounter = 0;
    var currentQuestionOrder = "";

    $(".radioButtonAnswer").each(function (index) {
        var qOrder = $(this).attr("questionOrder");

        if (currentQuestionOrder == "") {
            answerList[answerListCounter] = {};
            currentQuestionOrder = qOrder;
        }
        else if ((qOrder != currentQuestionOrder) && (currentQuestionOrder != "")) {
            answerList[++answerListCounter] = {};
            currentQuestionOrder = qOrder;
        }
        var qValue = $(this).attr("value");
        var result = $(this);
        answerList[answerListCounter][qValue] = result;
    });
}



buildAnswerList();


var currentQuestion = 0;

function removeAnswer() {
    if (currentQuestion > 0) {
        currentQuestion--;
        var cqObj = answerList[currentQuestion];

        $.each(cqObj, function (i, n) {
            answerList[currentQuestion][i][0].checked = false;
        });

        focusOnQuestion(currentQuestion, false);
    }
}


function addAnswer(answer) {
    if (currentQuestion < answerList.length) {
        if (answer != "-") {
            var isSelected = false;
            for (var key in answerList[currentQuestion]) {
                var item = answerList[currentQuestion][key];
                if (item != null && item[0] != null && item[0].checked == true) {
                    isSelected = true;
                }
            }
            if (isSelected) {
                currentQuestion++;
                addAnswer(answer);
                return;
            }
            if (answerList[currentQuestion][answer] != null) {
                answerList[currentQuestion][answer][0].checked = true;
                currentQuestion++;
            }
        } else {
            currentQuestion++;
        }
            

        focusOnQuestion(currentQuestion, false);
  
        if (currentQuestion == answerList.length) {
            $('#submitUnansweredQuestions').text('Press Enter to Submit and Go to Next Student');
            $('#submitUnansweredQuestions').focus();
        }
    } else {
        answerList.every(function(item, index) {
          var $questionEl = $(Object.values(item)[0]);
          if ($questionEl.parent().find("input:checked").length === 0) {
            item[answer][0].checked = true;
            return false;
          }
          return true;
        });
    }
}

//$('#advancedOptionTog').click(function () {
//    $('#advancedOptionDiv').toggle();
//});


$(document).on('click', '#manualEntryButton', function() {
    if (!window.soundManager)  {
        $.getScript("../../Scripts/soundmanager2-nodebug-jsmin.js", function() {
            if (soundManager.ok())
                soundManager.reboot();

            /*Sound Manager Parameters*/
            soundManager.url = '../../Content/FlashModules/SoundManager/'; // path to SoundManager2 SWF files (note trailing slash)
            soundManager.debugMode = false;
            soundManager.debugFlash = false;
            soundManager.consoleOnly = true;

            soundManager.onload = function() {
                soundManager.flashLoadTimeout = 150000;

                soundManager.createSound({
                    id: 'aSound',
                    url: '../../Content/sounds/a.mp3'
                });

                soundManager.createSound({
                    id: 'bSound',
                    url: '../../Content/sounds/b.mp3'
                });

                soundManager.createSound({
                    id: 'cSound',
                    url: '../../Content/sounds/c.mp3'
                });

                soundManager.createSound({
                    id: 'dSound',
                    url: '../../Content/sounds/d.mp3'
                });

                soundManager.createSound({
                    id: 'eSound',
                    url: '../../Content/sounds/e.mp3'
                });

                soundManager.createSound({
                    id: 'fSound',
                    url: '../../Content/sounds/f.mp3'
                });

                soundManager.createSound({
                    id: 'gSound',
                    url: '../../Content/sounds/g.mp3'
                });

                soundManager.createSound({
                    id: 'hSound',
                    url: '../../Content/sounds/h.mp3'
                });

                soundManager.createSound({
                    id: 'iSound',
                    url: '../../Content/sounds/i.mp3'
                });

                soundManager.createSound({
                    id: 'jSound',
                    url: '../../Content/sounds/j.mp3'
                });

                soundManager.createSound({
                    id: 'kSound',
                    url: '../../Content/sounds/k.mp3'
                });

                soundManager.createSound({
                    id: '0Sound',
                    url: '../../Content/sounds/0.mp3'
                });

                soundManager.createSound({
                    id: '1Sound',
                    url: '../../Content/sounds/1.mp3'
                });

                soundManager.createSound({
                    id: '2Sound',
                    url: '../../Content/sounds/2.mp3'
                });

                soundManager.createSound({
                    id: '3Sound',
                    url: '../../Content/sounds/3.mp3'
                });

                soundManager.createSound({
                    id: '4Sound',
                    url: '../../Content/sounds/4.mp3'
                });

                soundManager.createSound({
                    id: '5Sound',
                    url: '../../Content/sounds/5.mp3'
                });

                soundManager.createSound({
                    id: '6Sound',
                    url: '../../Content/sounds/6.mp3'
                });

                soundManager.createSound({
                    id: '7Sound',
                    url: '../../Content/sounds/7.mp3'
                });

                soundManager.createSound({
                    id: '8Sound',
                    url: '../../Content/sounds/8.mp3'
                });

                soundManager.createSound({
                    id: '9Sound',
                    url: '../../Content/sounds/9.mp3'
                });

                soundManager.createSound({
                    id: 'delete',
                    url: '../../Content/sounds/delete.mp3'
                });

                soundManager.createSound({
                    id: 'skip',
                    url: '../../Content/sounds/skip.mp3'
                });

                focusOnQuestion(currentQuestion, false);
            };
        });
    } else {
      focusOnQuestion(currentQuestion, false);
    }
});

$('#aSelection').click(function (e) {
    var isChecked = $(this).is(':checked');
 
    if (!isChecked) {
       confirmMessageV2(
          {
              message: "Are you sure you want to reset all selections?",
              cbYesBtnFuncName: 'yesCheckAllSelections()',
              cbCancelBtnFuncName: 'closeCheckAllSelections()',
              cbCloseBtnFuncName: 'closeCheckAllSelections()'
          },
          {
              dialogAttr: {
                  attr: {
                      id: 'checkAllSelection'
                  }
              }
          }
        )
    } else {
       changeAllRadio(true);
    }
});

function changeAllRadio(checked) {
   for (cnt = 0; cnt < answerList.length; cnt++) {
        var cqObj = answerList[cnt];

        try {
          if (checked) {
            answerList[cnt]['A'][0].checked = true;
          } else {
            Object.keys(answerList[cnt]).forEach(answer => {
              answerList[cnt][answer][0].checked = false;
            });
          }
        }
        catch (err) { }

        focusOnQuestion(cnt, true);
        }
    //Default Select A for multiple choice
    SelectAllSupportMultipleChoice();
}

 function closeCheckAllSelections() {
     $('#aSelection').prop("checked", true);
     $("#checkAllSelection").dialog("close");
 }

 function yesCheckAllSelections() {
    closeCheckAllSelections();
    $('#aSelection').prop("checked", false);
    changeAllRadio(false);
 }

function SelectAllSupportMultipleChoice() {
  var isChecked = $('#aSelection').is(':checked');
  
  $(".checkboxAnswer").each(function () {
      //debugger;
    var vValue = $(this).val();
    if (isChecked) {
      if (vValue == 'A') {
        $(this).attr('checked', 'checked');
      }
    } else {
      $(this).attr('checked', false);
    }
  });
}

var FocusingOnQuestion = false;
var isSelectAllAClicked = false;
var selectStudentState = false;

function focusOnQuestion(questionNumber, isSelectAllAClickValue) {
    isSelectAllAClicked = isSelectAllAClickValue;
    var cqObj = answerList[questionNumber];
    var count = 0;
    var lastItem;
    if (cqObj == null) {
        FocusingOnQuestion = true;
        isRadioButtonClicked = false;
        return;
    }
    $.each(cqObj, function (i, n) {
        lastItem = answerList[questionNumber][i][0];
        count++;
        return;

    });
    if (count > 0 && lastItem != null) {
        FocusingOnQuestion = true;
        isRadioButtonClicked = false;
        lastItem.focus();
    }
}

function isQuestionFocused() {
    var cqObj = answerList[currentQuestion];
    $.each(cqObj, function (i, n) {
        if ($(answerList[currentQuestion][i][0]).is(":focus")) {
            return true;
        }
    });
    return false;
}

var isRadioButtonClicked = false;
$('.radioButtonAnswer').click(function () {
    isRadioButtonClicked = true;
});

$('.radioButtonAnswer').focusout(function () {
    $('#manEntryGuide').hide().css({ "display": "none", "visibility": "hidden" });

    FocusingOnQuestion = false;
});

$(document).unbind("keypress");
$(document).keypress(function (event) {
    if (
        $(event.target).is('input[type="text"]')
        || $(event.target).attr("aria-controls") === "dataTable"
        || $(event.target).hasClass('openEndedQuestionAnswerTextbox')
        || event.charCode === 13
    ) return true;
    event.preventDefault();
    if (!$('#manualEntryButton').is(':checked')) return false;
    switch (event.charCode) {

        case 44: //, => B
            soundManager.play('bSound');
            addAnswer('B');
            break;

        case 46: //. => C
            soundManager.play('cSound');
            addAnswer('C');
            break;

        case 47: // / => D
            soundManager.play('dSound');
            addAnswer('D');
            break;

        case 97: soundManager.play('aSound'); addAnswer('A'); break; // a
        case 98: soundManager.play('bSound'); addAnswer('B'); break; // b			
        case 99: soundManager.play('cSound'); addAnswer('C'); break; // c			
        case 100: soundManager.play('dSound'); addAnswer('D'); break; // d
        case 101: soundManager.play('eSound'); addAnswer('E'); break; // e
        case 102: soundManager.play('fSound'); addAnswer('F'); break; // f
        case 103: soundManager.play('gSound'); addAnswer('G'); break; // g
        case 104: soundManager.play('hSound'); addAnswer('H'); break; // h
        case 105: soundManager.play('iSound'); addAnswer('I'); break; // i
        case 106: soundManager.play('jSound'); addAnswer('J'); break; // j
        case 107: soundManager.play('kSound'); addAnswer('K'); break; // k

        case 65: soundManager.play('aSound'); addAnswer('A'); break; // a
        case 66: soundManager.play('bSound'); addAnswer('B'); break; // b			
        case 67: soundManager.play('cSound'); addAnswer('C'); break; // c			
        case 68: soundManager.play('dSound'); addAnswer('D'); break; // d
        case 69: soundManager.play('eSound'); addAnswer('E'); break; // e
        case 70: soundManager.play('fSound'); addAnswer('F'); break; // f
        case 71: soundManager.play('gSound'); addAnswer('G'); break; // g
        case 72: soundManager.play('hSound'); addAnswer('H'); break; // h
        case 73: soundManager.play('iSound'); addAnswer('I'); break; // i
        case 74: soundManager.play('jSound'); addAnswer('J'); break; // j
        case 75: soundManager.play('kSound'); addAnswer('K'); break; // k

        case 48: soundManager.play('0Sound'); addAnswer('0'); break; // 0
        case 49: soundManager.play('1Sound'); addAnswer('1'); break; // 1
        case 50: soundManager.play('2Sound'); addAnswer('2'); break; // 2
        case 51: soundManager.play('3Sound'); addAnswer('3'); break; // 3
        case 52: soundManager.play('4Sound'); addAnswer('4'); break; // 4
        case 53: soundManager.play('5Sound'); addAnswer('5'); break; // 5
        case 54: soundManager.play('6Sound'); addAnswer('6'); break; // 6
        case 55: soundManager.play('7Sound'); addAnswer('7'); break; // 7
        case 56: soundManager.play('8Sound'); addAnswer('8'); break; // 8
        case 57: soundManager.play('9Sound'); addAnswer('9'); break; // 9



        case 115: //s => blank
            soundManager.play('skip');
            addAnswer('-');
            break;
        case 83: //s => blank
            soundManager.play('skip');
            addAnswer('-');
            break;

        case 109: //m => A
            soundManager.play('aSound');
            addAnswer('A');
            break;
        case 77: //m => A
            soundManager.play('aSound');
            addAnswer('A');
            break;

        case 110: //n => delete
            soundManager.play('delete');
            removeAnswer();
            break;
        case 78: //n => delete
            soundManager.play('delete');
            removeAnswer();
            break;
    }
    return false;

});

$(document).unbind("keydown");
$(document).keydown(function (event) {    
    processKeyDownToSelectStudent(event);
    disableArrowKeyPress(event);
    if (event.keyCode == 32) event.preventDefault();
});

$(document).unbind("mousedown");
$(document).mousedown(function (event) {
    //alert('mouse down');
    if(focusoutTime == null)
        focusoutTime = new Date();
});

//############################ Support select student by keyboard after submmiting #########################################
function disableArrowKeyPress(event) {
    if (event.which == 37 || event.which == 38 || event.which == 39 || event.which == 40) {
        event.preventDefault();
    }
}

function processKeyDownToSelectStudent(event) {
    if (selectStudentState) {

        switch (event.which) {
            case 37: // left
                break;

            case 38: // up
                {
                    event.preventDefault();
                    studentMoveUp();
                    break;
                }

            case 39: // right
                break;

            case 40: // down
                {
                    event.preventDefault();
                    studentMoveDown();
                    break;
                }

            case 13: // enter
                {                    
                    viewBubbleSheetDetail(); // in Index.cshtml
                    break;
                }

            default:
                return; // exit this handler for other keys
        }
    }
}

function viewBubbleSheetDetail(isApplyGradingShortcut, answers) {
    if (isApplyGradingShortcut == undefined || !isApplyGradingShortcut) {
        $('#hiddenApplyAllCorrectAnswer').val('0');
        $('#hiddenApplyFullCreditAnswer').val('0');
        $('#hiddenApplyZeroCreditAnswer').val('0');
    }

    $('#dataTable .student').removeClass('student-active');
    $($('#dataTable .student')[$('#selectedStudentIndex').val()]).addClass('student-active');
    var blockContainer = $('#parentContainer');
    ShowBlock(blockContainer, "Loading");

    var url = $('#hiddenGetBubbleSheetDetailUrl').val();
    var ticket = $('#hiddenTicket').val();
    var classId = $('#hiddenClassId').val();    
    var studentId = $($('#dataTable .student')[$('#selectedStudentIndex').val()]).attr('StudentId') || selectedStudentId;
    var applyAllCorrectAnswer = $('#hiddenApplyAllCorrectAnswer').val();
    var applyFullCreditAnswer = $('#hiddenApplyFullCreditAnswer').val();
    var applyZeroCreditAnswer = $('#hiddenApplyZeroCreditAnswer').val();

    var answerData = '';
    if (answers != undefined && answers != null) {
        answerData = JSON.stringify(answers);
    }

    $.post(url, {
        ticket: ticket, studentId: studentId, classId: classId
        , applyAllCorrectAnswer: applyAllCorrectAnswer, applyFullCreditAnswer: applyFullCreditAnswer, applyZeroCreditAnswer: applyZeroCreditAnswer
        , answerData: answerData
    }, function (response) {
        $("#bubbleSheetDetails").show();
        $("#bubbleSheetDetails").html(response);
        blockContainer.unblock();
    });
}

function moveToNextStudent() {
    selectStudentState = true;
    studentMoveDown();

    var currentIndex = $('#selectedStudentIndex').val();
    $(window).scrollTop($($('#dataTable .student')[currentIndex]).offset().top - 50);
}

function studentMoveUp() {
    var currentIndex = $('#selectedStudentIndex').val();
    if (currentIndex > 0) {
        currentIndex--;
        $('#selectedStudentIndex').val(currentIndex);

        $('#dataTable .student').removeClass('student-active');
        $($('#dataTable .student')[currentIndex]).addClass('student-active');
    }
}

function studentMoveDown() {
    var currentIndex = $('#selectedStudentIndex').val();
    if (currentIndex < $('#dataTable .student').length - 1) {
        currentIndex++;
        $('#selectedStudentIndex').val(currentIndex);

        $('#dataTable .student').removeClass('student-active');
        $($('#dataTable .student')[currentIndex]).addClass('student-active');
    }
}

$('#btnDisplayInstructions').click(function () {
  if ($('#manEntryGuide').css('display') == 'none') {
    $('#manEntryGuide').show().css({ "display": "block", "visibility": "visible" });
    $('#btnDisplayInstructions .icon-arrow').show().css({ "transform": "rotate(180deg)"});
  } else {
    $('#manEntryGuide').hide().css({ "display": "none", "visibility": "hidden" });
    $('#btnDisplayInstructions .icon-arrow').show().css({ "transform": "rotate(0)" });
  }
});
//############################ Support select student by keyboard after submmiting #########################################
