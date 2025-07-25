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
function testSchedule(systemDateFormat) {
  var jQueryDateFormat = systemDateFormat ? systemDateFormat : 'M d, yy';
  this.testClassAssignmentId = null;
  var _dataView = null;
  var valiDateMes = "The scheduled end date must be earlier or equal to the deadline. ";

  // normal mode
  this.init = function (data, level) {
    createControl();
    initEvent();
    initDefaultData(data);
    bindingData(data, level);
  };

  // view mode
  this.initView = function (data) {
    _dataView = data;
    createControl();
    initViewDefaultData();
    bindingDataView(data);
    initControlAndEventForView();
  };

  this.validateTestScheduleForm = function () {
    var isValid = true;
    var errorMessage = '';

    if ($('.test-schedule-action[value="ON"]:checked').length > 0) {
      var startTime = $("input[name='testScheduleStartTime']").val();
      var endTime = $("input[name='testScheduleEndTime']").val();

      if (!startTime) {
        errorMessage += 'Test Taking Hours - Start is wrong format. ';
        isValid = false;
      }

      if (!endTime) {
        errorMessage += 'Test Taking Hours - End is wrong format. ';
        isValid = false;
      }

      if ($("#rdTimeLimitON:checked").length > 0 && $("#enableDeadline:checked").length > 0) {
        var deadlineString = $("#selectDeadline").val() + ' ' + $("#selectHouse").val() + ':' + $("#selectMunite").val();
        var deadline = getDate(deadlineString);

        var endDateInScheduleString = $("input[name='testScheduleToDate']").val() + ' ' + $("input[name='testScheduleEndTime']").val();
        var endDateInSchedule = getDate(endDateInScheduleString);

        if (endDateInSchedule > deadline) {
          errorMessage += valiDateMes;
          isValid = false;
        }
      }

      if (Date.parse('01/01/2011 ' + startTime) >= Date.parse('01/01/2011 ' + endTime)) {
        errorMessage += "Test Taking Hours - End must be greater than Test Taking Hours - Start. ";
        isValid = false;
      }
    }

    if (!isValid) {
      alertMessageSetting(errorMessage);
    }

    return isValid;
  };

  this.save = function (data, isSaveLock) {
    var testScheduleValue = $("input[name='testSchedule']:checked").val();
    if (testScheduleValue == 'ON') {
      testScheduleValue = '1';
    } else {
      testScheduleValue = '0';
    }
    var testSchedule = _.find(data, function (value) { return value.Key == "testSchedule"; });

    if (!isSaveLock && testSchedule && testSchedule.Attributes && testSchedule.Attributes.length > 0) {
      var isLock = _.find(testSchedule.Attributes, function (value) {
        return value.Key == 'lock' && value.Value == 'true'
      });

      if (isLock) return;
    }

    if (testSchedule) {
      testSchedule.Value = testScheduleValue;
    } else {
      testSchedule = { Key: "testSchedule", Value: testScheduleValue, Attributes: [] }
      data.push(testSchedule);
    }

    if (testScheduleValue == 0) {
      initDefaultData(data);
    }

    var testScheduleFromDateValue = $("input[name='testScheduleFromDate']").datepicker("getDate").toString('MM/dd/yyyy');// + " " + $("input[name='testScheduleStartTime']").val();

    var testScheduleFromDate = _.find(data, function (value) { return value.Key == "testScheduleFromDate"; });

    if (testScheduleFromDate) {
      testScheduleFromDate.Value = testScheduleFromDateValue;
    } else {
      data.push({ Key: "testScheduleFromDate", Value: testScheduleFromDateValue });
    }

    var testScheduleStartTimeValue = $("input[name='testScheduleStartTime']").val();

    var testScheduleStartTime = _.find(data, function (value) { return value.Key == "testScheduleStartTime"; });
    if (testScheduleStartTime) {
      testScheduleStartTime.Value = testScheduleStartTimeValue
    } else {
      data.push({ Key: "testScheduleStartTime", Value: testScheduleStartTimeValue });
    }

    var testScheduleToDateValue = $("input[name='testScheduleToDate']").datepicker("getDate").toString('MM/dd/yyyy');//+ " " + $("input[name='testScheduleEndTime']").val();

    var testScheduleToDate = _.find(data, function (value) { return value.Key == "testScheduleToDate"; });

    if (testScheduleToDate) {
      testScheduleToDate.Value = testScheduleToDateValue;
    } else {
      data.push({ Key: "testScheduleToDate", Value: testScheduleToDateValue });
    }

    var testScheduleEndTimeValue = $("input[name='testScheduleEndTime']").val();

    var testScheduleEndTime = _.find(data, function (value) { return value.Key == "testScheduleEndTime"; });

    if (testScheduleEndTime) {
      testScheduleEndTime.Value = testScheduleEndTimeValue
    } else {
      data.push({ Key: "testScheduleEndTime", Value: testScheduleEndTimeValue });
    }

    var days = $("input[name='testScheduleDayOfWeek']:checked");
    var testScheduleDayOfWeekValue = [];

    for (var i = 0; i < days.length; i++) {
      testScheduleDayOfWeekValue.push($(days[i]).val());
    }

    testScheduleDayOfWeekValue = testScheduleDayOfWeekValue.join("|");
    var testScheduleDayOfWeek = _.find(data, function (value) { return value.Key == "testScheduleDayOfWeek"; });

    if (testScheduleDayOfWeek) {
      testScheduleDayOfWeek.Value = testScheduleDayOfWeekValue;
    } else {
      data.push({ Key: "testScheduleDayOfWeek", Value: testScheduleDayOfWeekValue });
    }

    if (isSaveLock) {
      var lockVal = ($("#chklockTestSchedule:checked").length > 0).toString();
      if (testSchedule.Attributes != null && testSchedule.Attributes.length > 0) {
        $.each(testSchedule.Attributes, function (attrIndex, attribute) {
          if (attribute.Key == 'lock') {
            attribute.Value = lockVal;
          }
        });
      } else {
        testSchedule.Attributes.push({ Key: 'lock', Value: lockVal });
      }
    }
  };

  function createControl() {
    $(".datepicker").datepicker({
      dateFormat: jQueryDateFormat,
      beforeShow: function (input) {
        $(input).css({
          "position": "relative",
          "z-index": '2000'
        });

        var tagActive = $(input).parents('body').find('#ui-datepicker-div');
        tagActive.find('.ui-state-highlight').css('min-height', 'auto');
        tagActive.addClass('customCssDataPickerPreference');
      }
    });
  }

  function initEvent() {
    // Show hide Test Schedule form
    $('.test-schedule-action').change("click", function () {
      if ($(this).is(":checked") == false) {
        $(".test-schedule-container.test-schedule-popup").hide();
      } else {
        $(".test-schedule-container.test-schedule-popup").show();
      }
    });

    $("input[name='testScheduleFromDate']").change(function () {
      $("input[name='testScheduleToDate']").datepicker('option', {
        minDate: getDate($(this).datepicker("getDate"))
      });
    });

    $("input[name='testScheduleToDate']").change(function () {
      $("input[name='testScheduleFromDate']").datepicker('option', {
        maxDate: getDate($(this).datepicker("getDate"))
      });
    });

    $("input[name='testScheduleFromDateView']").change(function () {
      $("input[name='testScheduleToDateView']").datepicker('option', {
        minDate: getDate($(this).datepicker("getDate"))
      });
    });

    $("input[name='testScheduleToDateView']").change(function () {
      $("input[name='testScheduleFromDateView']").datepicker('option', {
        maxDate: getDate($(this).datepicker("getDate"))
      });
    });

    $("input[name='testScheduleStartTime']").change(function () {
      $("#btnSubmitUpdatePreference").enableBt();
      $("#btnTestSettingSubmit").enableBt();
      if ($(this).val() == "00:00" && $("input[name='testScheduleEndTime']").val() == "23:59") {
        $("#alway-open").prop("checked", true);
      } else {
        $("#alway-open").prop("checked", false);
      }
    });

    $("input[name='testScheduleEndTime']").change(function () {
      $("#btnSubmitUpdatePreference").enableBt();
      $("#btnTestSettingSubmit").enableBt();
      if ($("input[name='testScheduleStartTime']").val() == "00:00" && $(this).val() == "23:59") {
        $("#alway-open").prop("checked", true);
      } else {
        $("#alway-open").prop("checked", false);
      }
    });

    $("#alway-open").on("click", function () {
      if ($(this).attr("checked") == "checked") {
        $("input[name='testScheduleStartTime']").prop("disabled", true);
        $("input[name='testScheduleEndTime']").prop("disabled", true);

        $("input[name='testScheduleEndTime']").val("23:59");
        $("input[name='testScheduleStartTime']").val("00:00");
      } else {
        $("input[name='testScheduleStartTime']").prop("disabled", false);
        $("input[name='testScheduleEndTime']").prop("disabled", false);
      }
    });
  }

  function initDefaultData(data) {
    var fromDate = new Date();
    var testScheduleFromDate = _.find(data, function (value) { return value.Key == "testScheduleFromDate"; });
    var testScheduleToDate = _.find(data, function (value) { return value.Key == "testScheduleToDate"; });
    $("#testSchedule0").prop("checked", true);
    $("#testSchedule1").prop('checked', false);
    $("input[name='testSchedule']").prop('checked', false);
    if (!testScheduleFromDate) $("input[name='testScheduleFromDate']").datepicker("setDate", fromDate);
    if (!testScheduleToDate) $("input[name='testScheduleToDate']").datepicker("setDate", new Date(fromDate.setMonth(fromDate.getMonth() + 1)));
    $("input[name='testScheduleStartTime']").val("00:00");
    $("input[name='testScheduleEndTime']").val("23:59");
    $("input[name='testScheduleDayOfWeek']").attr("checked", "checked");
    $("#alway-open").attr("checked", "checked");
  }

  function initViewDefaultData() {
    var fromDate = new Date();
    $("#testSchedule0").attr("checked", "checked");
    $("#testSchedule1").prop('checked', false);
    $("input[name='testSchedule']").attr('checked', "");
    $("input[name='testScheduleFromDate']").datepicker("setDate", fromDate);
    $("input[name='testScheduleToDate']").datepicker("setDate", new Date(fromDate.setMonth(fromDate.getMonth() + 1)));
    $("input[name='testScheduleStartTime']").val("00:00");
    $("input[name='testScheduleEndTime']").val("23:59");
    $("input[name='testScheduleDayOfWeek']").attr("checked", "checked");
  }

  function bindingData(data, level) {
    var alwaysOpen = 0;
    var startOffset = endOffset = 0;
    var endDateValue = '';
    currentLevelId = level;
    if (level === preferece_level.ENTERPRISE || level === preferece_level.ENTERPRISE_SURVEY)
      isEnterprise = true;
    else
      isEnterprise = false;

    $.each(data, function (index, tag) {
      switch (tag.Key) {
        case 'requireTestTakerAuthentication':
          if (tag.Value == 1) {
            $("#requireTestTakerAuthentication1").click();
            $("input[name='requireTestTakerAuthentication']").attr('checked', true);
          }
          else {
            $("#requireTestTakerAuthentication0").click();
            $("input[name='requireTestTakerAuthentication']").attr('checked', false);
          }
          break;
        case 'testSchedule':
          if (tag.Tooltips && tag.Tooltips !== '') {
            $('input[name="testSchedule"]').parent().parent().siblings(":first").append("<a href='javascript:void(0)' title='" + tag.Tooltips + "' class='with-tip'><img src='/Content/images/icons/icon-info.svg'></a >");
          }
          if (tag.Value == 1) {
            $("#testSchedule1").click();
            $("input[name='testSchedule']").attr('checked', true);
            $(".testScheduleDetail").show();
          }
          else {
            $("#testSchedule0").click();
            $("input[name='testSchedule']").attr('checked', false);
            $(".testScheduleDetail").hide();
          }

          if (tag.Attributes && tag.Attributes.length > 0) {
            $.each(tag.Attributes, function (attrIndex, attribute) {
              if (attribute.Key == 'lock' && attribute.Value == 'true') {
                toogleCheckboxV2Skin(true, $('#chklockTestSchedule'));

                if ($('#chklockTestSchedule').length == 0) {
                  $(".test-schedule-form input").prop("disabled", true);
                  $(".test-schedule-action").prop("disabled", true);
                }
                if (!isEnterprise && tag.LevelId != currentLevelId) {
                  $('input[name="testSchedule"]').prop('disabled', true);
                  $('.test-schedule-form input').prop('disabled', true);
                  $('#chklockTestSchedule').attr('disabled', true);
                }
                return false;
              }
            });
          }
          break;
        case 'testScheduleFromDate':
          $("input[name='testScheduleFromDate']").datepicker("setDate", new Date(tag.Value));
          break;
        case 'testScheduleStartTime':
          $("input[name='testScheduleStartTime']").val(tag.Value);
          break;
        case 'testScheduleToDate':
          $("input[name='testScheduleToDate']").datepicker("setDate", new Date(tag.Value));
          break;
        case 'testScheduleEndTime':
          $("input[name='testScheduleEndTime']").val(tag.Value);
          break;
        case 'testScheduleDayOfWeek':
          var days = tag.Value.split("|");
          var allDays = $("input[name='testScheduleDayOfWeek']");

          for (var i = 0; i < allDays.length; i++) {
            if (!_.contains(days, $(allDays[i]).val())) {
              $(allDays[i]).prop('checked', false);
            } else {
              const dayBtnstr = "input[name='testScheduleDayOfWeekBtn'][value=" + $(allDays[i]).val() + ']';
              $(dayBtnstr).addClass('btnActive');
              $('.testScheduleDetail').find(dayBtnstr).addClass('btnActive')
            }
          }
          break;
        default:
      }
    });

    if (alwaysOpen != 2) {
      $("#alway-open").prop("checked", false);
    } else {
      $("input[name='testScheduleStartTime']").prop("disabled", true);
      $("input[name='testScheduleEndTime']").prop("disabled", true);
    }

    $("input[name='testScheduleToDate']").datepicker('option', {
      minDate: getDate($("input[name='testScheduleFromDate']").datepicker("getDate"))
    });

    $("input[name='testScheduleFromDate']").datepicker('option', {
      maxDate: getDate($("input[name='testScheduleToDate']").datepicker("getDate"))
    });
  }

  function bindingDataView(data) {

    $.each(data, function (index, tag) {
      
      switch (tag.Key) {
        case 'testSchedule':
          setTimeout(function () {
            $("tr#testScheduleDetail").show();
          }, 1000);
          if (tag.Value == 1) {
            $("input[name='testSchedule']").prop('checked', true);
            $("#lbltestSchedule").text("On");
            $(".testScheduleDetail").show();
            $(".testScheduleDetail tr").show();
          }
          else {
            $("#testSchedule1").attr("checked", false);
            $("input[name='testSchedule']").prop('checked', false);
            $("#lbltestSchedule").text("Off");
            $(".testScheduleDetail").hide();
          }
          break;
        case 'testScheduleFromDate':
          setTimeout(function () {
            $("input[name='testScheduleFromDateView']").datepicker("setDate", new Date(tag.Value));
          }, 50);
          break;
        case 'testScheduleToDate':
          $("input[name='testScheduleToDateView']").datepicker("setDate", new Date(tag.Value));
          break;
        case 'testScheduleStartTime':
          $("input[name='testScheduleStartTime']").val(tag.Value);
          $("input[name='testScheduleStartTimeView']").val(tag.Value);
          break;
        case 'testScheduleEndTime':
          $("input[name='testScheduleEndTime']").val(tag.Value);
          $("input[name='testScheduleEndTimeView']").val(tag.Value);
          break;
        case 'testScheduleDayOfWeek':
          var days = tag.Value.split("|");
          var allDays = $("input[name='testScheduleDayOfWeek']");

          for (var i = 0; i < allDays.length; i++) {
            if (!_.contains(days, $(allDays[i]).val())) {
              $(allDays[i]).prop('checked', false);
              $("input[name='testScheduleDayOfWeekBtn'][value='" + $(allDays[i]).val() + "']").removeClass('btnActive');
              $("input[name='testScheduleDayOfWeek'][value='" + $(allDays[i]).val() + "']").removeClass('btnActive');
            } else {
              $(allDays[i]).prop('checked', true);
              $("input[name='testScheduleDayOfWeekBtn'][value='" + $(allDays[i]).val() + "']").addClass('btnActive');
              $("input[name='testScheduleDayOfWeek'][value='" + $(allDays[i]).val() + "']").addClass('btnActive');
            }
          }
          break;
        default:
      }
    });

    $("input[name='testScheduleToDateView']").datepicker('option', {
      minDate: getDate($("input[name='testScheduleFromDateView']").datepicker("getDate"))
    });

    $("input[name='testScheduleFromDateView']").datepicker('option', {
      maxDate: getDate($("input[name='testScheduleToDateView']").datepicker("getDate"))
    });
  }

  function defineButtonFunctions() {
    // enable edit mode
    $("#btnEnableEditScheduleMode").on("click", function () {
      disableInputs(false);
      $(this).hide();
      $("#btnCancelSchedule").show();
      $("#btnSaveSchedule").show();
    });

    // cancel()
    $("#btnCancelSchedule").on("click", function () {
      bindingDataView(_dataView);
      disableInputs(true);
      $("#btnEnableEditScheduleMode").show();
      $("#btnSaveSchedule").hide();
      $(this).hide();
    });

    $('.test-schedule-action').on("click", function () {
      if ($(this).is(':checked')) {
        $(".test-schedule-container").show();
        $(".day-of-week tr").show();
        $(".active-hours tr").show();
      } else {
        $(".test-schedule-container").hide();
      }
    });
    // save()
    $("#btnSaveSchedule").on("click", function () {
      var testScheduleValue = $("input[name='testSchedule']:checked").val();
      if (testScheduleValue == 'ON') {
        testScheduleValue = '1';
      } else {
        testScheduleValue = '0';
      }
      var updateScheduleData = [];
      var errorMessage = '';
      var startTime = $("#testScheduleStartTimeView").val();
      var endTime = $("#testScheduleEndTimeView").val();

      var testScheduleFromDateValue = $("#testScheduleFromDateView").datepicker("getDate").toString('MM/dd/yyyy');
      var testScheduleToDateValue = $("#testScheduleToDateView").datepicker("getDate").toString('MM/dd/yyyy');

      if (Date.parse($("#testScheduleFromDateView").val()) > Date.parse($("#testScheduleToDateView").val())) {
        errorMessage += "Active period To date must be greater than From date ";
      }

      var limitDate = getDate($("#lbltimeLimit").text());
      if (limitDate && getDate(testScheduleToDateValue) > limitDate) {
        errorMessage = valiDateMes;
      }

      if (Date.parse('01/01/2011 ' + startTime) >= Date.parse('01/01/2011 ' + endTime)) {
        errorMessage += "Test Taking Hours - End must be greater than Test Taking Hours - Start. ";
      }

      if (testScheduleValue === "1") {
        if (errorMessage) {
          alertMessageSetting(errorMessage);
          return;
        }
      }

      updateScheduleData.push({ Key: "testSchedule", Value: testScheduleValue });
      updateScheduleData.push({ Key: "testScheduleFromDate", Value: testScheduleFromDateValue });
      updateScheduleData.push({ Key: "testScheduleStartTime", Value: startTime });
      updateScheduleData.push({ Key: "testScheduleToDate", Value: testScheduleToDateValue });
      updateScheduleData.push({ Key: "testScheduleEndTime", Value: endTime });

      var days = $("input[name='testScheduleDayOfWeek']:checked");
      var testScheduleDayOfWeekValue = [];

      for (var i = 0; i < days.length; i++) {
        testScheduleDayOfWeekValue.push($(days[i]).val());
      }
      testScheduleDayOfWeekValue = testScheduleDayOfWeekValue.join("|");
      updateScheduleData.push({ Key: "testScheduleDayOfWeek", Value: testScheduleDayOfWeekValue });


      var jsonData = JSON.stringify({ testAssignmentId: testSchedule.testClassAssignmentId, scheduleValues: updateScheduleData });

      ShowBlock($('#testScheduleDetail'), 'Loading');
      $.ajax({
        url: "TestAssignment/UpdateTestSchedule",
        type: 'POST',
        dataType: 'json',
        contentType: 'application/json',
        data: jsonData,
        cache: false
      }).done(function (data) {
        $('#testScheduleDetail').unblock();
        if (data.Status) {
          _dataView = updateScheduleData;
          $("#btnCancelSchedule").click();
          $('#lastUpdateInfor label:first-child').empty();
          $('#lastUpdateInfor label:first-child').append('<label>Last Update: ' + data.UpdatedDate + '</label>');
        }
        else {
          alertMessageSetting("Update failed. Please try again.");
        }
      });
    });
  }

  function initControlAndEventForView() {

    var testScheduleFromDateView = getDate($("#testScheduleFromDateView").datepicker("getDate"));

    $("#testScheduleToDateView").datepicker({
      minDate: testScheduleFromDateView
    });

    $("#testScheduleFromDateView").change(function () {
      $("#testScheduleToDateView").datepicker({
        minDate: testScheduleFromDateView
      });
    });

    $("input[name='testScheduleFromDateView']").change(function () {
      var testScheduleFromDateView = getDate($(this).datepicker("getDate"));

      $("input[name='testScheduleToDateView']").datepicker('option', {
        minDate: testScheduleFromDateView
      });
    });

    $("input[name='testScheduleToDateView']").change(function () {
      var testScheduleToDateView = getDate($(this).datepicker("getDate"));

      $("input[name='testScheduleFromDateView']").datepicker('option', {
        maxDate: testScheduleToDateView
      });
    });

    var limitDate = Date.parse($("#lbltimeLimit").text());

    if (limitDate) {
      $("#testScheduleToDateView").datepicker({
        maxDate: limitDate
      });
    }

    disableInputs(true);

    defineButtonFunctions();

    $("#btnEnableEditScheduleMode").show();
    $("#btnSaveSchedule").hide();
    $("#btnCancelSchedule").hide();

    $("input[name='testScheduleStartTime']").change(function () {
      if ($(this).val() == "00:00" && $("input[name='testScheduleEndTime']").val() == "23:59") {
        $("#alway-open").prop("checked", true);
      } else {
        $("#alway-open").prop("checked", false);
      }
    });

    $("input[name='testScheduleEndTime']").change(function () {
      if ($("input[name='testScheduleStartTime']").val() == "00:00" && $(this).val() == "23:59") {
        $("#alway-open").prop("checked", true);
      } else {
        $("#alway-open").prop("checked", false);
      }
    });
    $("#alway-open").on("change", function () {
      if ($(this).attr("checked") == "checked" || $(this).is(':checked')) {
        $("input[name='testScheduleStartTime']").prop("disabled", true);
        $("input[name='testScheduleEndTime']").prop("disabled", true);

        $("input[name='testScheduleEndTime']").val("23:59");
        $("input[name='testScheduleStartTime']").val("00:00");
      } else {
        $("input[name='testScheduleStartTime']").prop("disabled", false);
        $("input[name='testScheduleEndTime']").prop("disabled", false);
      }
    });

    $("#alway-open").on("click", function () {
      if ($(this).attr("checked") == "checked" || $(this).is(':checked')) {
        $("input[name='testScheduleStartTime']").prop("disabled", true);
        $("input[name='testScheduleEndTime']").prop("disabled", true);

        $("input[name='testScheduleEndTime']").val("23:59");
        $("input[name='testScheduleStartTime']").val("00:00");
      } else {
        $("input[name='testScheduleStartTime']").prop("disabled", false);
        $("input[name='testScheduleEndTime']").prop("disabled", false);
      }
    });

    $('input[name="alwayOpenCkb"]').change(function () {
      var checked = $("input[name='alwayOpenCkb']").is(":checked");
      if (checked) {
        $("#alway-open").prop('checked', false);
        $("input[name='testScheduleStartTimeView']").prop("disabled", true);
        $("input[name='testScheduleEndTimeView']").prop("disabled", true);

        $("input[name='testScheduleEndTimeView']").val("23:59");
        $("input[name='testScheduleStartTimeView']").val("00:00");
      } else {
        $("#alway-open").prop('checked', true);
        $("input[name='testScheduleStartTimeView']").prop("disabled", false);
        $("input[name='testScheduleEndTimeView']").prop("disabled", false);
      }
    });
  }

  function disableInputs(disabled) {
    $(".test-schedule-form input").prop("disabled", disabled);
    if (disabled) {
      $('.test-schedule-action-group').hide();
      $('#lbltestSchedule').show();
    } else {
      $('.test-schedule-action-group').show();
      $('#lbltestSchedule').hide();
    }
  }

  function fixDateForAllBrowsers(dateString) {
    return dateString.replace(/-/g, '/');
  }

  function getFormatDateForFirefoxAndIe(dateString) {
    if (isFirefox() || isIE()) {
      dateString = fixDateForAllBrowsers(dateString);
    }

    if (isIE()) {
      dateString = getFormatDateForIe(dateString);
    }

    return dateString;
  }

  function getFormatDateForIe(dateString) {
    // Because Date.getMonth() in JavaScript Jan gives 0
    var months = {
      'Jan': 0,
      'Feb': 1,
      'Mar': 2,
      'Apr': 3,
      'May': 4,
      'Jun': 5,
      'Jul': 6,
      'Aug': 7,
      'Sep': 8,
      'Oct': 9,
      'Nov': 10,
      'Dec': 11
    };

    var a = dateString.split(' ');
    var d = a[0].split('/');

    if (!!a[1]) {
      var t = a[1].split(':');
      dateString = d[0] + '/' + months[d[1]] + '/' + d[2] + ' ' + t[0] + ':' + t[1];
    } else {
      dateString = d[0] + '/' + months[d[1]] + '/' + d[2];
    }

    return dateString;
  }

  function getDate(dateString) {
    if (typeof (dateString) === "object") return dateString;
    return new Date(dateString);

    // dateString = getFormatDateForFirefoxAndIe(dateString);

    // if (isEdge()) {
    //     var a = dateString.split(' ');
    //     var d = a[0].split('/');

    //     if (!!a[1]) {
    //         var t = a[1].split(':');
    //         return new Date(d[2], d[1], d[0], t[0], t[1]);
    //     } else {
    //         return new Date(d[2], d[1], d[0]);
    //     }
    // }
  }

  function isFirefox() {
    return typeof InstallTrigger !== 'undefined';
  }

  function isIE() {
    return (
            /*@cc_on!@*/ false ||
      !!document.documentMode ||
      isEdge()
    );
  }

  function isEdge() {
    return /Edge/.test(navigator.userAgent);
  }
}
