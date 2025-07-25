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
