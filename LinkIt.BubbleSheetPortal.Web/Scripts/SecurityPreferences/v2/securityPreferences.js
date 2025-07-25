var isEnterprise = false;
var currentLevelId = 0;
var preferece_level = {
  ENTERPRISE: '1',
  DISTRICT: '2',
  USER: '3'
};
var lastUpdatedString = '';

function BindRadioInputToOption(inputName, optionName, checked) {
  if (securityPreferenceModel != null && securityPreferenceModel.OptionTags != null && securityPreferenceModel.OptionTags.length > 0) {
    var isNewTag = true;
    $.each(securityPreferenceModel.OptionTags, function (index, tag) {
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

      securityPreferenceModel.OptionTags.push(tag);
    }
  }
}

function addLockDisabled(inputName) {
  $(`#${inputName}SectionItems`).attr('disabled', 'disabled')
}

function BindOptionToRadioInput(inputName, optionName, lockId) {
  if (securityPreferenceModel != null && securityPreferenceModel.OptionTags != null && securityPreferenceModel.OptionTags.length > 0) {
    $.each(securityPreferenceModel.OptionTags, function (index, tag) {
      if (tag.Key == optionName) {
        var value = tag.Value;
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

function FillDataToLayout(level) {
  currentLevelId = level;
  if (level === preferece_level.ENTERPRISE)
    isEnterprise = true;
  else
    isEnterprise = false;

  BindOptionToRadioInput('enableMFAEmail_publisher', 'enableMFAEmail_publisher', 'chklockEnableMFAEmail_publisher');
  BindOptionToRadioInput('enableMFAEmail_networkAdmin', 'enableMFAEmail_networkAdmin', 'chklockEnableMFAEmail_networkAdmin');
  BindOptionToRadioInput('enableMFAEmail_districtAdmin', 'enableMFAEmail_districtAdmin', 'chklockEnableMFAEmail_districtAdmin');
  BindOptionToRadioInput('enableMFAEmail_schoolAdmin', 'enableMFAEmail_schoolAdmin', 'chklockEnableMFAEmail_schoolAdmin');
  BindOptionToRadioInput('enableMFAEmail_teacher', 'enableMFAEmail_teacher', 'chklockEnableMFAEmail_teacher');
  BindOptionToRadioInput('enableMFAEmail_parent', 'enableMFAEmail_parent', 'chklockEnableMFAEmail_parent');
  BindOptionToRadioInput('enableMFAEmail_student', 'enableMFAEmail_student', 'chklockEnableMFAEmail_student');
  BindOptionToRadioInput('enableMFAEmail_user', 'enableMFAEmail_user', 'chklockEnableMFAEmail_user');

  $('.with-tip').tip();
}

function UpdateSecurityPreferenceModel() {
  var errorMsg = '';

  BindRadioInputToOption('enableMFAEmail_publisher', 'enableMFAEmail_publisher', $('#chklockEnableMFAEmail_publisher').is(":checked"));
  BindRadioInputToOption('enableMFAEmail_networkAdmin', 'enableMFAEmail_networkAdmin', $('#chklockEnableMFAEmail_networkAdmin').is(":checked"));
  BindRadioInputToOption('enableMFAEmail_districtAdmin', 'enableMFAEmail_districtAdmin', $('#chklockEnableMFAEmail_districtAdmin').is(":checked"));
  BindRadioInputToOption('enableMFAEmail_schoolAdmin', 'enableMFAEmail_schoolAdmin', $('#chklockEnableMFAEmail_schoolAdmin').is(":checked"));
  BindRadioInputToOption('enableMFAEmail_teacher', 'enableMFAEmail_teacher', $('#chklockEnableMFAEmail_teacher').is(":checked"));
  BindRadioInputToOption('enableMFAEmail_parent', 'enableMFAEmail_parent', $('#chklockEnableMFAEmail_parent').is(":checked"));
  BindRadioInputToOption('enableMFAEmail_student', 'enableMFAEmail_student', $('#chklockEnableMFAEmail_student').is(":checked"));
  BindRadioInputToOption('enableMFAEmail_user', 'enableMFAEmail_user', $('#chklockEnableMFAEmail_user').is(":checked"));

  return errorMsg;
}
