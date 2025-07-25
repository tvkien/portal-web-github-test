$('#rdbrowserLockdownModeOn').click(function () {
  $('#rdlockedDownTestTakerOff').attr('checked', 'checked');
  $('#rdcanPauseTestOff').attr('checked', 'checked');

  $('input[name="canPauseTest"]').attr('disabled', 'disabled');
  $('input[name="lockedDownTestTaker"]').attr('disabled', 'disabled');
});

$('#rdbrowserLockdownModeOff').click(function () {
  $('input[name="canPauseTest"]').removeAttr('disabled');
  $('input[name="lockedDownTestTaker"]').removeAttr('disabled');
});

$('#rdbrowserLockdownModeOn').change(function () {
  var checked = $(this).is(":checked");
  if (checked) {
    $('#rdlockedDownTestTakerOff').attr('checked', 'checked');
    $('#rdcanPauseTestOff').attr('checked', 'checked');

    $('input[name="canPauseTest"]').attr('disabled', 'disabled').prop('checked', false);
    $('input[name="lockedDownTestTaker"]').attr('disabled', 'disabled').prop('checked', false);

    $('input[name="lockedDownTestTakerCkb"]').attr('disabled', 'disabled').prop('checked', false);
    $('input[name="canPauseTestCkb"]').attr('disabled', 'disabled').prop('checked', false);
  } else {
    $('input[name="canPauseTest"]').removeAttr('disabled');
    $('input[name="lockedDownTestTaker"]').removeAttr('disabled');

    $('input[name="canPauseTestCkb"]').removeAttr('disabled');
    $('input[name="lockedDownTestTakerCkb"]').removeAttr('disabled', 'disabled');
  }
});

$('input[type="checkbox"][name="browserLockdownMode"]').change(function () {
  var checked = $(this).is(":checked");
  $('#rdbrowserLockdownModeOn').prop('checked', checked).change();
});
