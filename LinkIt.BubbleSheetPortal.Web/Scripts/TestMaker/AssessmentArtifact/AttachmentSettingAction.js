function closeSelect2BySelector(selector) {
  var $element = $(selector);
  if (!($element && $element.length > 0)) {
    return;
  }
  if (typeof $element.select2 !== 'function') {
    return;
  }
  $element.select2("close");
}

function closeAttachmentTypesSelect() {
  closeSelect2BySelector('#attachment-types-select');
}

function closeRecordingOptionsSelect() {
  closeSelect2BySelector('#recording-options-select');
}
