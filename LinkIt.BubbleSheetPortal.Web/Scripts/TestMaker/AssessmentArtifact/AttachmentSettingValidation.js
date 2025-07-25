function atLeastOneOption(attachmentSetting) {
  var valid = attachmentSetting.fileTypeGroupNames && attachmentSetting.fileTypeGroupNames.length > 0;
  return { valid, errorMessage: valid ? '' : 'Select at least one attachment type.' };
}

function schemaAllowRequireAttachment(schemeID) {
  // 10 - Constructed response
  // 10d - Draw response
  var qtiSchemaAllowRequireAttachmentOption = ["10", "10d"];
  var qtiSchemaId = schemeID;
  return qtiSchemaAllowRequireAttachmentOption.some(function (schemaId) { return schemaId === qtiSchemaId; });
}

function validRequireAttachment(attachmentSetting, schemeID) {
  var valid = true;
  if (attachmentSetting.requireAttachment) {
    valid = valid && schemaAllowRequireAttachment(schemeID);
  }
  return { valid, errorMessage: valid ? '' : `This type of question is not allowed to require attachment`};
}

function validateAttachmentSetting(attachmentSetting, schemeID) {
  if (!attachmentSetting.allowStudentAttachment) {
    return { valid: true, errorMessage: "" };
  }

  var validateResult = [atLeastOneOption(attachmentSetting), validRequireAttachment(attachmentSetting, schemeID)].find(function (result) { return !result.valid; });
  return validateResult ? validateResult : { valid: true, errorMessage: "" };
}
