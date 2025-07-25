var isEnterprise = false;
var currentLevelId = 0;
var preferece_level = {
  ENTERPRISE: '1',
  DISTRICT: '2',
  SCHOOL: '3',
  TEST_DESGIN: '4',
  USER: '5',
  TEST_ASSIGNMENT: '6',
  ENTERPRISE_SURVEY: '7',
  DISTRICT_SURVEY: '8'
};
var lastUpdatedString = '';

function isTestPreference() {
  return document.URL.indexOf('/ManageTest') > -1;
}

function isSettingPreference() {
  return document.URL.indexOf('/TestPreference') > -1;
}

function isTestAssignment() {
  return document.URL.indexOf('/TestAssignment') > -1;
}

function findWithAttr(array, attr, value) {
  for (var i = 0; i < array.length; i += 1) {
    if (array[i][attr] === value) {
      return i;
    }
  }
  return -1;
}

function BindRadioInputToOption(inputName, optionName, checked) {
  if (testPreferenceModel != null && testPreferenceModel.OptionTags != null && testPreferenceModel.OptionTags.length > 0) {
    var isNewTag = true;
    $.each(testPreferenceModel.OptionTags, function (index, tag) {
      if (tag.Key == optionName) {
        isNewTag = false;
        var lockVal = checked ? 'true' : 'false';
        var value = $('input[name="' + inputName + '"]:checked').val();
        if (value == undefined) value = 0;
        if (value != null && value != tag.Value) tag.Value = value;
        //save lock

        if (tag.Attributes != null && tag.Attributes.length > 0) {
          $.each(tag.Attributes, function (attrIndex, attribute) {
            if (attribute.Key == 'lock') {
              attribute.Value = lockVal;
              return false;
            }
          });
        } else {
          tag.Attributes.push({ Key: 'lock', Value: lockVal });
        }

        return false;
      }
    });

    if (isNewTag) {
      tag = {
        Attributes: [],
        Key: optionName
      };
      var lockVal = checked ? 'true' : 'false';
      var value = $('input[name="' + inputName + '"]:checked').val();
      if (value && value != tag.Value) {
        tag.Value = value;
      }
      //save lock

      if (tag.Attributes != null && tag.Attributes.length > 0) {
        $.each(tag.Attributes, function (attrIndex, attribute) {
          if (attribute.Key == 'lock') {
            attribute.Value = lockVal;
            return false;
          }
        });
      } else {
        tag.Attributes.push({ Key: 'lock', Value: lockVal });
      }

      testPreferenceModel.OptionTags.push(tag);
    }

    // Optional for gradebook
    if (inputName === 'testExtract') {
      var tag = _.find(testPreferenceModel.OptionTags, function (item) {
        return item.Key === 'testExtract';
      });

      var tagGradebook = _.find(testPreferenceModel.OptionTags, function (item) {
        return item.Key === 'testExtract_gradebook';
      });

      if (tagGradebook) {
        tagGradebook.Value = tag.Value;
      }
    }
  }
}

function getDataFromCheckbox(inputName, optionName, checked) {
  if (testPreferenceModel != null && testPreferenceModel.OptionTags != null && testPreferenceModel.OptionTags.length > 0) {
    var tag = _.find(testPreferenceModel.OptionTags, function (tag) {
      return tag.Key === optionName;
    });

    var isNewTag = false;
    if (!tag) {
      tag = {
        Attributes: [],
        Key: optionName
      };

      isNewTag = true;
    }

    var value = $('input[name="' + inputName + '"]:checked').length > 0 ? '1' : '0';
    if (value != null && value != tag.Value) tag.Value = value;

    // check lock condition
    var lockVal = checked ? 'true' : 'false';
    if (tag.Attributes != null && tag.Attributes.length > 0) {
      var attr = _.find(tag.Attributes, function (attr) {
        return attr.Key === 'lock';
      });

      if (attr) {
        attr.Value = lockVal;
      }
    } else {
      if (!tag.Attributes) tag.Attributes = [];
      tag.Attributes.push({ Key: 'lock', Value: lockVal });
    }

    if (isNewTag) {
      testPreferenceModel.OptionTags.push(tag);
    }
  }
}

function addLockDisabled(inputName) {
  $(`#${inputName}SectionItems`).attr('disabled', 'disabled')
}

function BindOptionToRadioInput(inputName, optionName, lockId) {
  if (testPreferenceModel != null && testPreferenceModel.OptionTags != null && testPreferenceModel.OptionTags.length > 0) {
    $.each(testPreferenceModel.OptionTags, function (index, tag) {
      if (tag.Key == optionName) {
        var value = tag.Value;
        if (optionName === 'displayTexttospeechOption' && tag.Value === '3') {
          $('#idTestToSpeedOptionON').css({
            'left': '0',
            'position': 'relative'
          });
        }
        $('input[name="' + inputName + '"]').removeAttr('checked').removeClass('input-checked-v2');

        if (tag.Tooltips && tag.Tooltips !== '') {
          $('input[name="' + inputName + '"]').parent().closest('td').siblings(":first").append("<a href='javascript:void(0)' title='" + tag.Tooltips + "' class='with-tip'><img src='/Content/images/icons/icon-info.svg'></a >");
        }

        if (!isEnterprise) {
          $('input[name="' + inputName + '"]').removeAttr('disabled');
          $('#' + lockId).removeAttr('disabled');
        }
        $('input[name="' + inputName + '"][value="' + value + '"]').attr('checked', true);

        if (tag.Attributes != null && tag.Attributes.length > 0) {
          $.each(tag.Attributes, function (attrIndex, attribute) {
            if (attribute.Key == 'lock' && attribute.Value == 'true') {
              toogleCheckboxV2Skin(true, $('#' + lockId));

              if (!isEnterprise && tag.LevelId != currentLevelId) {
                $('input[name="' + inputName + '"]').attr('disabled', true);
                $('#' + lockId).attr('disabled', true);

                if (optionName === 'displayTexttospeechOption' && (tag.Value === '1' || tag.Value === '2' || tag.Value === '3')) {
                  $('#idTestToSpeedOptionON').css({
                    'pointer-events': 'none',
                    'opacity': '0.4'
                  });
                }

                addLockDisabled(inputName)
              }

              return false;
            }
          });
        }

        return false;
      }
    });
  }
}

function setDependRadioStatus() {
  if (testPreferenceModel != null && testPreferenceModel.OptionTags != null && testPreferenceModel.OptionTags.length > 0) {
    var browserLockdownModeIndex = findWithAttr(testPreferenceModel.OptionTags, "Key", "browserLockdownMode");
    if (browserLockdownModeIndex > -1) {
      var browserLockdownMode = testPreferenceModel.OptionTags[browserLockdownModeIndex];
      var dependLockdownModePreferences = ["canPauseTest", "lockedDownTestTaker"];
      if (browserLockdownMode.Value === '1') {
        $.each(testPreferenceModel.OptionTags, function (index, tag) {
          if (dependLockdownModePreferences.indexOf(tag.Key) > -1) {
            $('input[name="' + tag.Key + '"]').attr('disabled', 'disabled');
          }
        });
      }
    }
  }
}

function bindDataToCheckbox(inputName, optionName, lockId) {
  if (testPreferenceModel != null && testPreferenceModel.OptionTags != null && testPreferenceModel.OptionTags.length > 0) {
    var tag = _.find(testPreferenceModel.OptionTags, function (tag) {
      return tag.Key === optionName;
    });

    if (tag) {
      $('input[name="' + inputName + '"]').removeAttr('checked').removeClass('input-checked-v2');

      if (tag.Value === '1') {
        $('input[name="' + inputName + '"]').attr('checked', true);
      }

      if (tag.Attributes != null && tag.Attributes.length > 0) {
        var attr = _.find(tag.Attributes, function (attr) {
          return attr.Key === 'lock' && attr.Value === 'true';
        });

        if (attr) {
          $('#' + lockId).attr('checked', true);
          if (!isEnterprise && tag.LevelId != currentLevelId) {
            $('input[name="' + inputName + '"]').attr('disabled', true);
            $('#' + lockId).attr('disabled', true);
          }
        }
      }

      if (inputName === 'testExtract_gradebook') {
        $('input[name="testExtract"][value="' + tag.Value + '"]').attr('checked', true);
      }
    }
  }
}

function FillDataToLayout(level) {
  currentLevelId = level;
  if (level === preferece_level.ENTERPRISE || level === preferece_level.ENTERPRISE_SURVEY)
    isEnterprise = true;
  else
    isEnterprise = false;
  //Navigation/Display
  BindOptionToRadioInput('testExtractExportRawScore', 'testExtractExportRawScore', 'chklockTestExtractExportRawScore');
  BindOptionToRadioInput('requireHonorCode', 'requireHonorCode', 'chklockRequireHonorCode');
  BindOptionToRadioInput('autoAdvanceTest', 'autoAdvanceTest', 'chklockAutoAdvance');
  BindOptionToRadioInput('mustAnswerAllQuestions', 'mustAnswerAllQuestions', 'chklockMustAnswerAllQuestions');
  BindOptionToRadioInput('canPauseTest', 'canPauseTest', 'chklockCanPauseTest');
  BindOptionAnswerLabelToCheckBoxInput();
  BindOptionToRadioInput('answerLabelFormat', 'answerLabelFormat', 'chklockAnswerLabelFormat');
  BindOptionToRadioInput('passagePositioninTestTaker', 'passagePositioninTestTaker', 'chklockPassagePositioninTestTaker');
  BindOptionToRadioInput('displayAnswerChoiceGuidance', 'displayAnswerChoiceGuidance', 'chklockdisplayAnswerChoiceGuidance');
  BindOptionToRadioInput('hideCorrectAnswerAndGrading', 'hideCorrectAnswerAndGrading', 'chklockhideCorrectAnswerAndGrading');

  if ($('input[name="questiongrouplabelschema"').length > 0) {
    BindOptionToRadioInput('questiongrouplabelschema', 'questiongrouplabelschema', 'chklockquestiongrouplabelschema');
    BindOptionToRadioInput('questiongroupmultiplechoicesnumbering', 'questiongroupmultiplechoicesnumbering', 'chklockquestiongroupmultiplechoicesnumbering');
  }

  BindOptionToRadioInput('sectionBasedTesting', 'sectionBasedTesting', 'chklockSectionBasedTesting');

  //Security
  BindOptionToRadioInput('verifyStudent', 'verifyStudent', 'chklockVerifyStudent');
  BindOptionToRadioInput('requireTestTakerAuthentication', 'requireTestTakerAuthentication', 'chkLockRequireTestTakerAuthentication');
  BindOptionToRadioInput('shuffleQuestions', 'shuffleQuestions', 'chklockShuffleQuestions');
  BindOptionToRadioInput('shuffleAnswers', 'shuffleAnswers', 'chklockShuffleAnswers');
  BindOptionToRadioInput('lockedDownTestTaker', 'lockedDownTestTaker', 'chklockLockedDownTestTaker');
  BindOptionToRadioInput('browserLockdownMode', 'browserLockdownMode', 'chkLockBrowserLockdownMode');
  BindOptionToRadioInput('overrideAutoGradedTextEntry', 'overrideAutoGradedTextEntry', 'chklockOverrideAutoGradedTextEntry');
  BindOptionToRadioInput('anonymizedScoring', 'anonymizedScoring', 'chkLockAnonymizedScoring');

  //Accomodations
  //BindOptionToRadioInput('timeLimit', 'timeLimit');
  BindOptionToRadioInput('timeLimitDurationType', 'timeLimitDurationType', 'chklockTimeLimit');
  BindOptionTimeLimitToRadioInput();

  BindOptionToRadioInput('showTimeLimitWarning', 'showTimeLimitWarning', 'chklockShowTimeLimitWarning');
  BindOptionToRadioInput('multipleChoiceClickMethod', 'multipleChoiceClickMethod', 'chklockMultipleChoiceClickMethod');
  BindOptionToRadioInput('enableVideoControls', 'enableVideoControls', 'chklockEnableVideoControls');
  BindOptionToRadioInput('enableAudio', 'enableAudio', 'chklockEnableAudio');

  BindOptionToRadioInput('supportFontsize', 'supportFontsize', 'chklocksupportFontsize');
  BindOptionToRadioInput('supportZoom', 'supportZoom', 'chklocksupportZoom');
  BindOptionToRadioInput('supportContrast', 'supportContrast', 'chklocksupportContrast');
  BindOptionToRadioInput('supportLineReader', 'supportLineReader', 'chklocksupportLineReader');

  //Tools
  BindToolToRadioInput('simplePalette', 'simplePalette', 'chklocksimplePalette');
  BindToolToRadioInput('mathPalette', 'mathPalette', 'chklockmathPalette');
  BindToolToRadioInput('spanishPalette', 'spanishPalette', 'chklockspanishPalette');
  BindToolToRadioInput('frenchPalette', 'frenchPalette', 'chklockfrenchPalette');
  BindToolToRadioInput('protractor', 'protractor', 'chklockprotractor');
  // ruler
  BindToolToRadioInput('ruler6inch', 'ruler6inch', 'chklockprotractor6inch');
  BindToolToRadioInput('ruler12inch', 'ruler12inch', 'chklockprotractor12inch');
  BindToolToRadioInput('ruler15cm', 'ruler15cm', 'chklockprotractor15cm');
  BindToolToRadioInput('ruler30cm', 'ruler30cm', 'chklockprotractor30cm');

  BindToolToRadioInput('supportCalculator', 'supportCalculator', 'chklockSupportCalculator');
  BindToolToRadioInput('scientificCalculator', 'scientificCalculator', 'chkLockScientificCalculator');

  BindOptionToRadioInput('supportHighlightText', 'supportHighlightText', 'chklockSupportHighlightText');
  BindOptionToRadioInput('eliminateChoiceTool', 'eliminateChoiceTool', 'chklockEliminateChoiceTool');
  BindOptionToRadioInput('flagItemTool', 'flagItemTool', 'chklockFlagItemTool');
  BindOptionToRadioInput('equationeditor', 'equationeditor', 'chklockEquationeditor');

  BindToolToRadioInput('drawingTool', 'drawingTool', 'chklockDrawingTool');

  //Other
  BindOptionToRadioInput('canReviewTest', 'canReviewTest', 'chklockCanReviewTest');
  BindOptionToRadioInput('adaptiveTest', 'adaptiveTest', 'chklockAdaptiveTest');
  DisplayAdaptiveAndBranchingTest();
  BindOptionToRadioInput('branchingTest', 'branchingTest', 'chklockBranchingTest');
  BindOptionToRadioInput('showTestInstructions', 'showTestInstructions', 'chklockshowTestInstructions');
  BindOptionToRadioInput('displayQuestionNumber', 'displayQuestionNumber', 'chklockdisplayQuestionNumber');
  BindOptionToRadioInput('displayTexttospeechOption', 'displayTexttospeechOption', 'chklockdisplayTexttospeechOption');
  BindOptionToRadioInput('showIncompleteAnswerWarning', 'showIncompleteAnswerWarning', 'chklockshowIncompleteAnswerWarning');
  BindOptionTextToSpeed();

  bindDataToCheckbox('testExtract_gradebook', 'testExtract_gradebook', 'chklockTestExtract');
  bindDataToCheckbox('testExtract_studentRecord', 'testExtract_studentRecord', 'chklockTestExtract');
  BindOptionToRadioInput('testExtract_clever', 'testExtract_clever', 'chklockTestExtract');

  setDependRadioStatus();
  $('.with-tip').tip();
}

function UpdateTestPreferenceModel() {
  var errorMsg = '';

  BindRadioInputToOption('testExtractExportRawScore', 'testExtractExportRawScore', $('#chklockTestExtractExportRawScore').is(":checked"), false);
  BindRadioInputToOption('requireHonorCode', 'requireHonorCode', $('#chklockRequireHonorCode').is(":checked"), false);
  BindRadioInputToOption('autoAdvanceTest', 'autoAdvanceTest', $('#chklockAutoAdvance').is(":checked"));
  BindRadioInputToOption('mustAnswerAllQuestions', 'mustAnswerAllQuestions', $('#chklockMustAnswerAllQuestions').is(":checked"));
  BindRadioInputToOption('canPauseTest', 'canPauseTest', $('#chklockCanPauseTest').is(":checked"));
  BindCheckBoxInputToOptionAnswerLabel();
  BindRadioInputToOption('answerLabelFormat', 'answerLabelFormat', $('#chklockAnswerLabelFormat').is(":checked"));
  BindRadioInputToOption('passagePositioninTestTaker', 'passagePositioninTestTaker', $('#chklockPassagePositioninTestTaker').is(":checked"));
  BindRadioInputToOption('displayAnswerChoiceGuidance', 'displayAnswerChoiceGuidance', $('#chklockdisplayAnswerChoiceGuidance').is(":checked"));
  BindRadioInputToOption('hideCorrectAnswerAndGrading', 'hideCorrectAnswerAndGrading', $('#chklockhideCorrectAnswerAndGrading').is(":checked"));

  if ($('input[name="questiongrouplabelschema"').length > 0) {
    BindRadioInputToOption('questiongrouplabelschema', 'questiongrouplabelschema', $('#chklockquestiongrouplabelschema').is(":checked"));
    BindRadioInputToOption('questiongroupmultiplechoicesnumbering', 'questiongroupmultiplechoicesnumbering', $('#chklockquestiongroupmultiplechoicesnumbering').is(":checked"));
  }

  //Security
  BindRadioInputToOption('verifyStudent', 'verifyStudent', $('#chklockVerifyStudent').is(":checked"));
  BindRadioInputToOption('requireTestTakerAuthentication', 'requireTestTakerAuthentication', $('#chkLockRequireTestTakerAuthentication').is(":checked"));
  BindRadioInputToOption('shuffleQuestions', 'shuffleQuestions', $('#chklockShuffleQuestions').is(":checked"));
  BindRadioInputToOption('shuffleAnswers', 'shuffleAnswers', $('#chklockShuffleAnswers').is(":checked"));
  BindRadioInputToOption('lockedDownTestTaker', 'lockedDownTestTaker', $('#chklockLockedDownTestTaker').is(":checked"));
  BindRadioInputToOption('browserLockdownMode', 'browserLockdownMode', $('#chkLockBrowserLockdownMode').is(":checked"));
  BindRadioInputToOption('overrideAutoGradedTextEntry', 'overrideAutoGradedTextEntry', $('#chklockOverrideAutoGradedTextEntry').is(":checked"));
  BindRadioInputToOption('anonymizedScoring', 'anonymizedScoring', $('#chkLockAnonymizedScoring').is(":checked"));

  //Accomodations
  //BindRadioInputToOption('timeLimit', 'timeLimit');
  var errMsg = BindRadioInputToOptionTimeLimit();
  if (errMsg != '') {
    return errMsg;
  }
  BindRadioInputToOption('showTimeLimitWarning', 'showTimeLimitWarning', $('#chklockShowTimeLimitWarning').is(":checked"));
  BindRadioInputToOption('timeLimitDurationType', 'timeLimitDurationType');
  BindRadioInputToOption('multipleChoiceClickMethod', 'multipleChoiceClickMethod', $('#chklockMultipleChoiceClickMethod').is(":checked"));
  BindRadioInputToOption('enableVideoControls', 'enableVideoControls', $('#chklockEnableVideoControls').is(":checked"));
  BindRadioInputToOption('enableAudio', 'enableAudio', $('#chklockEnableAudio').is(":checked"));

  BindRadioInputToOption('supportFontsize', 'supportFontsize', $('#chklocksupportFontsize').is(":checked"));
  BindRadioInputToOption('supportZoom', 'supportZoom', $('#chklocksupportZoom').is(":checked"));
  BindRadioInputToOption('supportContrast', 'supportContrast', $('#chklocksupportContrast').is(":checked"));
  BindRadioInputToOption('supportLineReader', 'supportLineReader', $('#chklocksupportLineReader').is(":checked"));

  //Tools
  BindRadioInputToTool('simplePalette', 'simplePalette', $('#chklocksimplePalette').is(":checked"));
  BindRadioInputToTool('mathPalette', 'mathPalette', $('#chklockmathPalette').is(":checked"));
  BindRadioInputToTool('spanishPalette', 'spanishPalette', $('#chklockspanishPalette').is(":checked"));
  BindRadioInputToTool('frenchPalette', 'frenchPalette', $('#chklockfrenchPalette').is(":checked"));
  BindRadioInputToTool('protractor', 'protractor', $('#chklockprotractor').is(":checked"));
  // ruler
  BindRadioInputToTool('ruler6inch', 'ruler6inch', $('#chklockprotractor6inch').is(":checked"));
  BindRadioInputToTool('ruler12inch', 'ruler12inch', $('#chklockprotractor12inch').is(":checked"));
  BindRadioInputToTool('ruler15cm', 'ruler15cm', $('#chklockprotractor15cm').is(":checked"));
  BindRadioInputToTool('ruler30cm', 'ruler30cm', $('#chklockprotractor30cm').is(":checked"));

  BindRadioInputToTool('supportCalculator', 'supportCalculator', $('#chklockSupportCalculator').is(":checked"));
  BindRadioInputToTool('scientificCalculator', 'scientificCalculator', $('#chkLockScientificCalculator').is(":checked"));

  BindRadioInputToOption('supportHighlightText', 'supportHighlightText', $('#chklockSupportHighlightText').is(":checked"));
  BindRadioInputToOption('eliminateChoiceTool', 'eliminateChoiceTool', $('#chklockEliminateChoiceTool').is(":checked"));
  BindRadioInputToOption('flagItemTool', 'flagItemTool', $('#chklockFlagItemTool').is(":checked"));
  BindRadioInputToOption('equationeditor', 'equationeditor', $('#chklockEquationeditor').is(":checked"));

  BindRadioInputToTool('drawingTool', 'drawingTool', $('#chklockDrawingTool').is(":checked"));

  //Other
  BindRadioInputToOption('canReviewTest', 'canReviewTest', $('#chklockCanReviewTest').is(":checked"));
  BindRadioInputToOption('sectionBasedTesting', 'sectionBasedTesting', $('#chklockSectionBasedTesting').is(":checked"));
  BindRadioInputToOption('adaptiveTest', 'adaptiveTest', $('#chklockAdaptiveTest').is(":checked"));
  BindRadioInputToOption('branchingTest', 'branchingTest', $('#chklockBranchingTest').is(":checked"));
  BindRadioInputToOption('showTestInstructions', 'showTestInstructions', $('#chklockshowTestInstructions').is(":checked"));
  BindRadioInputToOption('displayQuestionNumber', 'displayQuestionNumber', $('#chklockdisplayQuestionNumber').is(":checked"));
  BindRadioInputToOption('displayTexttospeechOption', 'displayTexttospeechOption', $('#chklockdisplayTexttospeechOption').is(":checked"));
  BindRadioInputToOption('showIncompleteAnswerWarning', 'showIncompleteAnswerWarning', $('#chklockshowIncompleteAnswerWarning').is(":checked"));
  UpdateOptionTextToSpeed();

  testSchedule.save(testPreferenceModel.OptionTags, true);

  getDataFromCheckbox('testExtract_gradebook', 'testExtract_gradebook', $('#chklockTestExtract').is(":checked"));
  getDataFromCheckbox('testExtract_studentRecord', 'testExtract_studentRecord', $('#chklockTestExtract').is(":checked"));

  BindRadioInputToOption('testExtract_clever', 'testExtract_clever', $('#chklockTestExtract').is(":checked"));
  return errorMsg;
}

function BindOptionTextToSpeed() {
  if (testPreferenceModel != null && testPreferenceModel.OptionTags != null && testPreferenceModel.OptionTags.length > 0) {
    $.each(testPreferenceModel.OptionTags, function (index, tag) {
      switch (tag.Key) {
        case 'rate':
          $('#selectRate').val(tag.Value);
          break;
        case 'volume':
          $('#selectVolume').val(tag.Value);
          break;
        case 'displayTexttospeechOption':
          var value = tag.Value;
          $('input[name="displayTexttospeechOption"][value="' + value + '"]').attr('checked', 'checked');
          if (value === '1' || value === '2' || value === '3' || value === '4') {
            $('#idTestToSpeedOptionON').show();
          }
          break;
        default:
      }
    });
  }
  return false;
}

function UpdateOptionTextToSpeed() {
  if (testPreferenceModel != null && testPreferenceModel.OptionTags != null && testPreferenceModel.OptionTags.length > 0) {
    var valueTextToSpeed = $('input[name="displayTexttospeechOption"]:checked').val();
    var vRate = '';
    var vVolume = '';
    if (valueTextToSpeed === "1" || valueTextToSpeed === "2" || valueTextToSpeed === "3" || valueTextToSpeed === "4") {
      vRate = $('#selectRate').val();
      vVolume = $('#selectVolume').val();
    }
    var vExistRateVolume = 0;
    $.each(testPreferenceModel.OptionTags, function (index, tag) {
      if (tag.Key === 'rate') {
        tag.Value = vRate;
        vExistRateVolume = 1;
      }
      if (tag.Key === 'volume') {
        tag.Value = vVolume;
        vExistRateVolume = 1;
      }
    });
    if (vExistRateVolume === 0) {
      testPreferenceModel.OptionTags.push({ Key: 'rate', Value: '' });
    }
    if (vExistRateVolume === 0) {
      testPreferenceModel.OptionTags.push({ Key: 'volume', Value: '' });
    }
  }
}

function BindOptionTimeLimitToRadioInput() {
  if (testPreferenceModel != null && testPreferenceModel.OptionTags != null && testPreferenceModel.OptionTags.length > 0) {
    $.each(testPreferenceModel.OptionTags, function (index, tag) {
      if (tag.Key == 'timeLimit') {
        var value = tag.Value;
        $('input[name="timeLimit"]').removeAttr('checked');
        $('input[name="timeLimit"]').removeAttr('disabled');
        $('input[name="timeLimit"][value="' + value + '"]').attr('checked', 'checked');

        // Lock
        if (tag.Tooltips && tag.Tooltips !== '') {
          $('input[name="timeLimit"]').parent().parent().closest('td').siblings(":first").append("<a href='javascript:void(0)' title='" + tag.Tooltips + "' class='with-tip'><img src='/Content/images/icons/icon-info.svg'></a >");
        }
        if (tag.Attributes != null && tag.Attributes.length > 0) {
          $.each(tag.Attributes, function (attrIndex, attribute) {
            if (attribute.Key == 'lock' && attribute.Value == 'true') {
              toogleCheckboxV2Skin(true, $('#chklockTimeLimit'));

              if (!isEnterprise && tag.LevelId != currentLevelId) {
                $('input[name="timeLimit"]').attr('disabled', 'disabled');
                $('#idTimeLimitON input').prop('disabled', true);
                $('input[name="timeLimitDurationType"]').prop('disabled', true);
                $('#divDuration input').prop('disabled', true);
                $('#divDeadline input').prop('disabled', true);
                $('#divDeadline select').prop('disabled', true);
                $('#chklockTimeLimit').attr('disabled', true);
                addLockDisabled('timeLimit')
              }
            }
          });
        }

        initTimeLimit();
        return false;
      }
    });
  }
  return false;
}

function BindRadioInputToOptionTimeLimit() {
  if (testPreferenceModel != null && testPreferenceModel.OptionTags != null && testPreferenceModel.OptionTags.length > 0) {
    var valueTimeLimit = $('input[name="timeLimit"]:checked').val();
    var valueDuration = 0;
    var valueDurationType = 1;
    var valueDeadline = '';
    if (valueTimeLimit == "1") {
      if ($('#enableDeadline:checked').length > 0) {
        if ($("#selectDeadline").val() == '') {
          return 'Please select a date for the deadline.';
        }
        valueDeadline = BuildLocalDateTime();
        var today = new Date();
        var deadline = new Date(valueDeadline);
        if (deadline < today) {
          return 'Deadline must be equal or greater than current time.';
        }
      } else {
        if (!$("#timeLimitSectionItems").is(":checked")) {
          if ($('#enableDurationOption:checked').length > 0 && ($("#durationId").val() == '' || $("#durationId").val() == '0')) {
            return 'Value must be provided.';
          }
        }
        valueDuration = $('#durationId').val();
        valueDurationType = $('input[name=timeLimitDurationType]:checked').val();
      }
    }

    $('#hdfDdeadlineId').val(valueDeadline);
    $('#hdfDurationId').val(valueDuration);
    $('#hdfDurationType').val(valueDurationType);
    $.each(testPreferenceModel.OptionTags, function (index, tag) {
      if (tag.Key == 'timeLimit') {
        tag.Value = valueTimeLimit;
        //save lock
        var lockVal = $('#chklockTimeLimit').is(":checked") ? 'true' : 'false';
        if (tag.Attributes != null && tag.Attributes.length > 0) {
          $.each(tag.Attributes, function (attrIndex, attribute) {
            if (attribute.Key == 'lock') {
              attribute.Value = lockVal;
              return false;
            }
          });
        } else {
          tag.Attributes.push({ Key: 'lock', Value: lockVal });
        }
      }
      if (tag.Key == 'duration') {
        tag.Value = valueDuration;
      }
      if (tag.Key == 'timeLimitDurationType') {
        tag.Value = valueDurationType;
      }
      if (tag.Key == 'deadline') {
        tag.Value = valueDeadline;
      }
    });
  }
  return '';
}

function BindOptionAnswerLabelToCheckBoxInput() {
  if (testPreferenceModel != null && testPreferenceModel.OptionTags != null && testPreferenceModel.OptionTags.length > 0) {
    $.each(testPreferenceModel.OptionTags, function (index, tag) {
      if (tag.Key == 'displayAnswerLabels') {
        $('input[name="DisplayAnswerLabels"]').removeAttr('checked');
        $('input[name="DisplayAnswerLabels"]').removeAttr('disabled');
        switch (tag.Value) {
          case '1':
            {
              $('input[name="DisplayAnswerLabels"]').attr('checked', true).addClass('input-checked-v2');
            } break;
          case '2':
            {
              $('#DisplayAnswerLabelSingleSelect').attr('checked', true).addClass('input-checked-v2');
            } break;
          case '3':
            {
              $('#DisplayAnswerLabelMultiselect').attr('checked', true).addClass('input-checked-v2');
            } break;
        }

        if (tag.Tooltips && tag.Tooltips !== '') {
          $('input[name="DisplayAnswerLabels"]').parent().closest('td').siblings(":first").append("<a href='javascript:void(0)' title='" + tag.Tooltips + "' class='with-tip'><img src='/Content/images/icons/icon-info.svg'></a >");
        }

        if (tag.Attributes != null && tag.Attributes.length > 0) {
          $.each(tag.Attributes, function (attrIndex, attribute) {
            if (attribute.Key == 'lock' && attribute.Value == 'true') {
              $('#chklockDisplayAnswerLabels').attr('checked', true).addClass('input-checked-v2');

              if (!isEnterprise && tag.LevelId != currentLevelId) {
                $('input[name="DisplayAnswerLabels"]').attr('disabled', true);
                $('#chklockDisplayAnswerLabels').attr('disabled', true);
              }
            }
          });
        }
        return false;
      }
    });
  }
}

function BindCheckBoxInputToOptionAnswerLabel() {
  if (testPreferenceModel != null && testPreferenceModel.OptionTags != null && testPreferenceModel.OptionTags.length > 0) {
    $.each(testPreferenceModel.OptionTags, function (index, tag) {
      if (tag.Key == 'displayAnswerLabels') {
        var value = '0';
        if ($('#DisplayAnswerLabelSingleSelect').is(':checked') && $('#DisplayAnswerLabelMultiselect').is(':checked')) {
          value = '1';
        } else if ($('#DisplayAnswerLabelSingleSelect').is(':checked') && !$('#DisplayAnswerLabelMultiselect').is(':checked')) {
          value = '2';
        }
        else if (!$('#DisplayAnswerLabelSingleSelect').is(':checked') && $('#DisplayAnswerLabelMultiselect').is(':checked')) {
          value = '3';
        }
        tag.Value = value;

        //save lock
        var lockVal = $('#chklockDisplayAnswerLabels').is(":checked") ? 'true' : 'false';
        if (tag.Attributes != null && tag.Attributes.length > 0) {
          $.each(tag.Attributes, function (attrIndex, attribute) {
            if (attribute.Key == 'lock') {
              attribute.Value = lockVal;
              return false;
            }
          });
        } else {
          tag.Attributes.push({ Key: 'lock', Value: lockVal });
        }

        return false;
      }
    });
  }
}

function BindToolToRadioInput(inputName, optionName, lockId) {
  if (testPreferenceModel != null && testPreferenceModel.ToolTags != null && testPreferenceModel.ToolTags.length > 0) {
    $.each(testPreferenceModel.ToolTags, function (index, tag) {
      if (tag.Key == optionName) {
        var value = tag.Value;
        $('input[name="' + inputName + '"]').removeAttr('checked');
        $('input[name="' + inputName + '"]').removeAttr('disabled');
        $('input[name="' + inputName + '"][value="' + value + '"]').attr('checked', true).addClass('input-checked-v2');

        if (tag.Tooltips && tag.Tooltips !== '') {
          $('input[name="' + inputName + '"]').parent().closest('td').siblings(":first").append("<a href='javascript:void(0)' title='" + tag.Tooltips + "' class='with-tip'><img src='/Content/images/icons/icon-info.svg'></a >");
        }

        if (tag.Attributes != null && tag.Attributes.length > 0) {
          $.each(tag.Attributes, function (attrIndex, attribute) {
            if (attribute.Key == 'lock' && attribute.Value == 'true') {
              if (!isEnterprise && tag.LevelId != currentLevelId) {
                $('input[name="' + inputName + '"]').attr('disabled', true);
                $('#' + lockId).attr('disabled', true);
                addLockDisabled(inputName)
              }
              $('input[id="' + lockId + '"]').attr('checked', true).addClass('input-checked-v2');;
              return false;
            }
          });
        }
        return false;
      }
    });
  }
}

function BindRadioInputToTool(inputName, optionName, checked) {
  if (testPreferenceModel != null && testPreferenceModel.ToolTags != null && testPreferenceModel.ToolTags.length > 0) {
    $.each(testPreferenceModel.ToolTags, function (index, tag) {
      if (tag.Key == optionName) {
        var lockVal = checked ? 'true' : 'false';
        var value = $('input[name="' + inputName + '"]:checked').val();
        if (value == undefined) value = 0;
        if (value != null && value != tag.Value) tag.Value = value;

        //save lock
        if (tag.Attributes != null && tag.Attributes.length > 0) {
          $.each(tag.Attributes, function (attrIndex, attribute) {
            if (attribute.Key == 'lock') {
              attribute.Value = lockVal;
              return false;
            }
          });
        } else {
          tag.Attributes.push({ Key: 'lock', Value: lockVal });
        }

        return false;
      }
    });
  }
}

function BuildLocalDateTime() {
  var vDate = $("#selectDeadline").datepicker("getDate");
  vDate.setHours($("#selectHouse").val());
  vDate.setMinutes($("#selectMunite").val());
  return vDate.toString('MM/dd/yyyy HH:mm:ss');
}

function initTimeLimit() {
  if ($('#rdTimeLimitON:checked').length > 0) {
    $('#idTimeLimitON').show();
    var durationValue = $("#hdfDurationId").val();
    if (durationValue > 0) {
      $('#divDuration').show();
      $('#divDeadline').hide();
      $('#durationId').val(durationValue);
    } else {
      $('#divDuration').hide();
      $('#divDeadline').show();
      $('#divDeadline')[0].style.display = 'flex';
      $('#divDeadline')[0].style.gap = '10px';
      $('#enableDeadline').prop('checked', true);
      $('#timeLimitSectionItems').attr("disabled", true)
      var deadlineValue = $("#hdfDdeadlineId").val();
      var now = new Date(deadlineValue);
      $("#selectDeadline").datepicker("setDate", now);
      $("#selectHouse").val(now.getHours());
      $("#selectMunite").val(now.getMinutes());
    }
  }
}

function FillReadOnlyPreferences(isReview) {
  $('#dvTestAssignmentDefaultSetting tr').hide();
  var vRate = '';
  var vVolume = '';
  var vTextToSpeed = '';
  if (testPreferenceModel != null) {
    if (testPreferenceModel.OptionTags != null && testPreferenceModel.OptionTags.length > 0) {
      var textExtract = '';

      if (testExtractOptions.CleverApi) {
        var onClever = _.find(testPreferenceModel.OptionTags, function (option) {
          return option.Key === 'testExtract_clever' && option.Value === '1';
        });
        if (onClever)
          textExtract += 'Clever API';
      }
      else {
        var onGradebook = _.find(testPreferenceModel.OptionTags, function (option) {
          return option.Key === 'testExtract_gradebook' && option.Value === '1';
        });
        var onStudentReport = _.find(testPreferenceModel.OptionTags, function (option) {
          return option.Key === 'testExtract_studentRecord' && option.Value === '1';
        });
        if (onGradebook) textExtract += 'Gradebook';
        if (onStudentReport) textExtract += (textExtract === '' ? '' : ' - ') + 'Student Report';
      }

      if (textExtract !== '') {
        $('#lbltestExtract').text(textExtract);
      } else {
        $('#lbltestExtract').text('Off');
      }
      $.each(testPreferenceModel.OptionTags, function (index, tag) {
        $('#lbl' + tag.Key).parents('tr').show();

        //////////////Navigation/Display
        if (tag.Key == 'sectionAvailability') {
          if (tag.Attributes[0] && tag.Attributes[0].Value == '1') {
            $('#lblsectionAvailability').text('On');
          } else {
            $('#lblsectionAvailability').text('Off');
          }
        }
        if (tag.Key == 'autoAdvanceTest') {
          if (tag.Value == '1') {
            $('#lblautoAdvanceTest').text('On');
          }
          else {
            $('#lblautoAdvanceTest').text('Off');
          }
        }
        if (tag.Key == 'requireHonorCode') {
          if (tag.Value == '1') {
            $('#lblrequireHonorCode').text('On');
          }
          else {
            $('#lblrequireHonorCode').text('Off');
          }
        }
        if (tag.Key == 'testExtractExportRawScore') {
          if (tag.Value == '1') {
            $('#lbltestExtractExportRawScore').text('Raw');
          }
          else {
            $('#lbltestExtractExportRawScore').text('Percent');
          }
        }
        if (tag.Key == 'mustAnswerAllQuestions') {
          if (tag.Value == '1') {
            $('#lblmustAnswerAllQuestions').text('On');
          } else {
            $('#lblmustAnswerAllQuestions').text('Off');
          }
        }
        if (tag.Key == 'canPauseTest') {
          if (tag.Value == '1') {
            $('#lblcanPauseTest').text('On');
          } else {
            $('#lblcanPauseTest').text('Off');
          }
        }
        if (tag.Key == 'displayAnswerLabels') {
          if (tag.Value == '1') {
            $('#lbldisplayAnswerLabels').text('Single Select - Multi Select');
          } else if (tag.Value == '2') {
            $('#lbldisplayAnswerLabels').text('Single Select');
          } else if (tag.Value == '3') {
            $('#lbldisplayAnswerLabels').text('Multi Select');
          }
          else {
            $('#lbldisplayAnswerLabels').text('Off');
          }
        }
        if (tag.Key == 'displayQuestionNumber') {
          if (tag.Value == '1') {
            $('#lbldisplayQuestionNumber').text('On');
          } else {
            $('#lbldisplayQuestionNumber').text('Off');
          }
        }
        if (tag.Key == 'answerLabelFormat') {
          if (tag.Value == '1') {
            $('#lblanswerLabelFormat').text('Numbers (1, 2, 3...)');
          } else {
            $('#lblanswerLabelFormat').text('Letter (A, B, C...)');
          }
        }
        if (tag.Key == 'passagePositioninTestTaker') {
          if (tag.Value == '1') {
            $('#lblpassagePositioninTestTaker').text('Left');
          } else {
            $('#lblpassagePositioninTestTaker').text('Right');
          }
        }
        if (tag.Key == 'displayAnswerChoiceGuidance') {
          if (tag.Value == '1') {
            $('#lbldisplayAnswerChoiceGuidance').text('On');
          } else {
            $('#lbldisplayAnswerChoiceGuidance').text('Off');
          }
        }

        if (tag.Key == 'questiongrouplabelschema') {
          if (tag.Value == '1') {
            $('#lblquestiongrouplabelschema').text('1.1, 1.2, 1.3...');
          } else {
            $('#lblquestiongrouplabelschema').text('1a, 1b, 1c...');
          }
        }
        if (tag.Key == 'questiongroupmultiplechoicesnumbering') {
          if (tag.Value == '1') {
            $('#lblquestiongroupmultiplechoicesnumbering').text('Continue');
          } else {
            $('#lblquestiongroupmultiplechoicesnumbering').text('Restart');
          }
        }
        if (tag.Key == 'hideCorrectAnswerAndGrading') {
          if (tag.Value == '1') {
            $('#lblhideCorrectAnswerAndGrading').text('On');
          } else {
            $('#lblhideCorrectAnswerAndGrading').text('Off');
          }
        }

        /////////////Security
        if (tag.Key == 'verifyStudent') {
          if (tag.Value == '1') {
            $('#lblverifyStudent').text('On');
          } else {
            $('#lblverifyStudent').text('Off');
          }
        }
        if (tag.Key === 'requireTestTakerAuthentication') {
          var labelText = tag.Value === '1' ? 'On' : 'Off';
          $('#lblrequireTestTakerAuthentication').text(labelText);
        }
        if (tag.Key == 'shuffleQuestions') {
          if (tag.Value == '1') {
            $('#lblshuffleQuestions').text('On');
          } else {
            $('#lblshuffleQuestions').text('Off');
          }
        }
        if (tag.Key == 'shuffleAnswers') {
          if (tag.Value == '1') {
            $('#lblshuffleAnswers').text('On');
          } else {
            $('#lblshuffleAnswers').text('Off');
          }
        }
        if (tag.Key == 'lockedDownTestTaker') {
          if (tag.Value == '1') {
            $('#lbllockedDownTestTaker').text('On');
          } else {
            $('#lbllockedDownTestTaker').text('Off');
          }
        }
        if (tag.Key == 'overrideAutoGradedTextEntry') {
          if (tag.Value == '1') {
            $('#lbloverrideAutoGradedTextEntry').text('On');
          } else {
            $('#lbloverrideAutoGradedTextEntry').text('Off');
          }
        }
        if (tag.Key == 'browserLockdownMode') {
          if (tag.Value == '1') {
            $('#lblbrowserLockdownMode').text('On');
          } else {
            $('#lblbrowserLockdownMode').text('Off');
          }
        }
        if (tag.Key == 'anonymizedScoring') {
          if (tag.Value == '1') {
            $('#lblanonymizedScoring').text('On');
          } else {
            $('#lblanonymizedScoring').text('Off');
          }
        }
        ////////////////////Accomodations
        if (tag.Key == 'timeLimit') {
          if (tag.Value == '1') {
            var vDuration = $('#hdfDurationId').val();
            var vDeadLine = $('#hdfDdeadlineId').val();
            if (vDuration > 0) {
              $('#lbltimeLimit').text(vDuration + ' Minutes');
            } else {
              $('#lbltimeLimit').text(formatAMPM());
            }
            $('#trTimeLimitShowTimeLimitWarning').show();
          } else {
            $('#lbltimeLimit').text('Off');
            $('#trTimeLimitShowTimeLimitWarning').hide();
          }
        }
        if (tag.Key == 'showTimeLimitWarning') {
          if (tag.Value == '1') {
            $('#lblshowTimeLimitWarning').text('On');
          } else {
            $('#lblshowTimeLimitWarning').text('Off');
          }
        }
        if (tag.Key == 'multipleChoiceClickMethod') {
          if (tag.Value == '1') {
            $('#lblmultipleChoiceClickMethod').text('Click Answer');
          } else if (tag.Value == '2') {
            $('#lblmultipleChoiceClickMethod').text('Click Answer and Button');
          } else {
            $('#lblmultipleChoiceClickMethod').text('Click Button');
          }
        }
        if (tag.Key == 'enableVideoControls') {
          if (tag.Value == '1') {
            $('#lblenableVideoControls').text('On');
          } else {
            $('#lblenableVideoControls').text('Off');
          }
        }
        if (tag.Key == 'enableAudio') {
          // if (tag.Value == '1') {
          //     $('#lblenableAudio').text('On');
          // } else {
          //     $('#lblenableAudio').text('Off');
          // }
          switch (tag.Value) {
            case '0':
              $('#lblenableAudio').text('Off'); break;
            case '2':
              $('#lblenableAudio').text('Simple Audio'); break;
            case '1':
              $('#lblenableAudio').text('Advanced Audio'); break;
            default: $('#lblenableAudio').text('Off'); break;
          }
        }
        if (tag.Key == 'supportFontsize') {
          if (tag.Value == '1') {
            $('#lblsupportFontsize').text('On');
          } else {
            $('#lblsupportFontsize').text('Off');
          }
        }
        if (tag.Key == 'supportZoom') {
          if (tag.Value == '1') {
            $('#lblsupportZoom').text('On');
          } else {
            $('#lblsupportZoom').text('Off');
          }
        }
        if (tag.Key == 'supportContrast') {
          if (tag.Value == '1') {
            $('#lblsupportContrast').text('On');
          } else {
            $('#lblsupportContrast').text('Off');
          }
        }
        if (tag.Key == 'supportLineReader') {
          if (tag.Value == '1') {
            $('#lblsupportLineReader').text('On');
          } else {
            $('#lblsupportLineReader').text('Off');
          }
        }
        if (tag.Key == 'displayTexttospeechOption') {
          if (tag.Value === '1' || tag.Value === '2' || tag.Value === '3' || tag.Value === '4') {
            $('#lbldisplayTexttospeechOption').text('On');
            vTextToSpeed = tag.Value;
          } else {
            $('#lbldisplayTexttospeechOption').text('Off');
          }
        }
        if (tag.Key == 'showIncompleteAnswerWarning') {
          if (tag.Value == '1') {
            $('#lblshowIncompleteAnswerWarning').text('On');
          } else {
            $('#lblshowIncompleteAnswerWarning').text('Off');
          }
        }

        if (tag.Key === 'rate') {
          vRate = tag.Value;
        }
        if (tag.Key === 'volume') {
          vVolume = tag.Value;
        }
        //////////Tools
        if (tag.Key == 'supportHighlightText') {
          if (tag.Value == '0') {
            $('#lblsupportHighlightText').text('Off');
          } else if (tag.Value == '1') {
            $('#lblsupportHighlightText').text('On');
          }
          else {
            $('#lblsupportHighlightText').text('On (Only Student)');
          }
        }
        if (tag.Key == 'eliminateChoiceTool') {
          if (tag.Value == '1') {
            $('#lbleliminateChoiceTool').text('On');
          } else {
            $('#lbleliminateChoiceTool').text('Off');
          }
        }
        if (tag.Key == 'flagItemTool') {
          if (tag.Value == '1') {
            $('#lblflagItemTool').text('On');
          } else {
            $('#lblflagItemTool').text('Off');
          }
        }
        if (tag.Key === 'equationeditor') {
          if (tag.Value === '1') {
            $('#lblequationeditor').text('On');
          } else {
            $('#lblequationeditor').text('Off');
          }
        }
        ///////////Other
        if (tag.Key == 'canReviewTest') {
          if (tag.Value == '1') {
            $('#lblcanReviewTest').text('On');
          } else {
            $('#lblcanReviewTest').text('Off');
          }
        }
        if (tag.Key == 'sectionBasedTesting') {
          if (tag.Value == '1') {
            $('#lblsectionBasedTesting').text('On');
          } else {
            $('#lblsectionBasedTesting').text('Off');
          }
        }

        if (tag.Key === 'adaptiveTest') {
          if (tag.Value === '1') {
            $('#lbladaptiveTest').text('On');
          } else {
            $('#lbladaptiveTest').text('Off');
          }
        }
        if (tag.Key === 'branchingTest') {
          if (tag.Value === '1') {
            $('#lblbranchingTest').text('Normal Branching');
          } else if (tag.Value === '2') {
            $('#lblbranchingTest').text('Algorithmic Branching');
          } else if (tag.Value === '3') {
            $('#lblbranchingTest').text('Section Based Branching');
          }
          else {
            $('#lblbranchingTest').text('Off');
          }
        }
        if (tag.Key === 'showTestInstructions') {
          if (tag.Value === '1') {
            $('#lblshowTestInstructions').text('On');
          } else {
            $('#lblshowTestInstructions').text('Off');
          }
        }
      });

      testSchedule.initView(testPreferenceModel.OptionTags, isReview);
    }

    if (vTextToSpeed === '1' || vTextToSpeed === '2' || vTextToSpeed === '3' || vTextToSpeed === '4') {
      $('#lbldisplayTexttospeechOption').text('Rate: ' + DisplayRate(vRate) + ',  Volume: ' + DisplayVolume(vVolume));

      if (vTextToSpeed === '1') {
        $('#lbldisplayTexttospeechOption').text($('#lbldisplayTexttospeechOption').text() + ' (All Content)');
      }

      if (vTextToSpeed === '2') {
        $('#lbldisplayTexttospeechOption').text($('#lbldisplayTexttospeechOption').text() + ' (Passage)');
      }

      if (vTextToSpeed === '3') {
        $('#lbldisplayTexttospeechOption').text($('#lbldisplayTexttospeechOption').text() + ' (Instructions/Questions)');
      }

      if (vTextToSpeed === '4') {
        $('#lbldisplayTexttospeechOption').text($('#lbldisplayTexttospeechOption').text() + ' (Instructions Only)');
      }
    }

    if (testPreferenceModel.ToolTags != null && testPreferenceModel.ToolTags.length > 0) {
      $.each(testPreferenceModel.ToolTags, function (index, tag) {
        $('#lbl' + tag.Key).parents('tr').show();

        if (tag.Key == 'simplePalette') {
          if (tag.Value == '1') {
            $('#lblsimplePalette').text('On');
          } else {
            $('#lblsimplePalette').text('Off');
          }
        }
        if (tag.Key == 'mathPalette') {
          if (tag.Value == '1') {
            $('#lblmathPalette').text('On');
          } else {
            $('#lblmathPalette').text('Off');
          }
        }
        if (tag.Key == 'spanishPalette') {
          if (tag.Value == '1') {
            $('#lblspanishPalette').text('On');
          } else {
            $('#lblspanishPalette').text('Off');
          }
        }
        if (tag.Key == 'frenchPalette') {
          if (tag.Value == '1') {
            $('#lblfrenchPalette').text('On');
          } else {
            $('#lblfrenchPalette').text('Off');
          }
        }
        if (tag.Key == 'protractor') {
          if (tag.Value == '1') {
            $('#lblprotractor').text('On');
          } else {
            $('#lblprotractor').text('Off');
          }
        }
        if (tag.Key == 'ruler6inch') {
          if (tag.Value == '1') {
            $('#lblruler6inch').text('On');
          } else {
            $('#lblruler6inch').text('Off');
          }
        }
        if (tag.Key == 'ruler12inch') {
          if (tag.Value == '1') {
            $('#lblruler12inch').text('On');
          } else {
            $('#lblruler12inch').text('Off');
          }
        }
        if (tag.Key == 'ruler15cm') {
          if (tag.Value == '1') {
            $('#lblruler15cm').text('On');
          } else {
            $('#lblruler15cm').text('Off');
          }
        }
        if (tag.Key == 'ruler30cm') {
          if (tag.Value == '1') {
            $('#lblruler30cm').text('On');
          } else {
            $('#lblruler30cm').text('Off');
          }
        }
        if (tag.Key == 'supportCalculator') {
          if (tag.Value == '1') {
            $('#lblsupportCalculator').text('On');
          } else {
            $('#lblsupportCalculator').text('Off');
          }
        }
        if (tag.Key == 'scientificCalculator') {
          if (tag.Value == '1') {
            $('#lblscientificCalculator').text('On');
          } else {
            $('#lblscientificCalculator').text('Off');
          }
        }
        if (tag.Key === 'drawingTool') {
          if (tag.Value === '0') {
            $('#lbldrawingTool').text('Free Formatted');
          } else {
            $('#lbldrawingTool').text('Graphing and Labeling');
          }
        }
      });
    }
  }
}

function DisplayAdaptiveAndBranchingTest() {
  var vVirtualTestSubTypeId = $('#idVirtualTestSubTypeID').val();
  var vVirtualTestSourceId = $('#idVirtualTestSourceID').val();
  var vIdNavigationMethodId = $('#idNavigationMethodID').val();
  //TODO: Order is important
}

function formatAMPM() {
  var date = new Date($('#hdfDdeadlineId').val());
  var hours = date.getHours();
  var minutes = date.getMinutes();
  var ampm = hours >= 12 ? 'PM' : 'AM';
  hours = hours % 12;
  hours = hours ? hours : 12; // the hour '0' should be '12'
  minutes = minutes < 10 ? '0' + minutes : minutes;
  var strTime = date.toLocaleDateString() + ' ' + hours + ':' + minutes + ' ' + ampm;
  return strTime;
}
