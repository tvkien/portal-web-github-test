function getStates(url, getStateDataSuccess) {
  $.ajax(url).done(function (response) {
    getStateDataSuccess(response);
  });
}

function getDistrictByStateId(url, GetDistrictByStateIdData) {
  $.ajax(url).done(function (response) {
    GetDistrictByStateIdData(response);
  });
}

function getSchoolByDistrictId(url, GetSchoolByDistrictIdData) {
  $.ajax(url).done(function (response) {
    GetSchoolByDistrictIdData(response);
  });
}

function getListTestType(url, getListTestTypeData) {
  $.ajax(url).done(function (response) {
    getListTestTypeData(response);
  });
}

function getGradesByDistrict(url, getGradesByDistrictData) {
  $.ajax(url).done(function (response) {
    getGradesByDistrictData(response);
  });
}

function getGradesByDistrict(url, getGradesByDistrictData) {
  $.ajax(url).done(function (response) {
    getGradesByDistrictData(response);
  });
}

function getSubjectsByGradeIdAndAuthor(url, getSubjectsByGradeIdAndAuthorData) {
  $.ajax(url).done(function (response) {
    getSubjectsByGradeIdAndAuthorData(response);
  });
}

function saveStudentTestPreferences(url, model, callback) {
  $.ajax({
    url: url,
    contentType: 'application/json; charset=utf-8',
    dataType: 'json',
    type: 'POST',
    data: JSON.stringify(model)
  }).done(function (response) {
    callback(response);
  });
}

function getDefaultRef(url, callback, model) {
  if (model) {
    $.ajax({
      url: url,
      contentType: 'application/json; charset=utf-8',
      dataType: 'json',
      type: 'GET',
      data: model
    }).done(function (response) {
      callback(response);
    });
  } else {
    $.ajax({
      url: url,
      contentType: 'application/json; charset=utf-8',
      dataType: 'json',
      type: 'GET',
    }).done(function (response) {
      callback(response);
    });
  }
}

function getDefaultOption(model, callback) {
  console.log('getDefaultOption', model);
  $.ajax({
    url: '/StudentPreference/GetStudentPreference',
    contentType: 'application/json; charset=utf-8',
    dataType: 'json',
    type: 'GET',
    data: model
  }).done(function (response) {
    callback(response);
  });
}

function setDefaultOption(model, callback) {
  $.ajax({
    url: '/StudentPreference/SetStudentPreference',
    type: 'POST',
    contentType: "application/json; charset=utf-8",
    dataType: "json",
    data: JSON.stringify(model)
  }).done(function (response) {
    callback(response);
  });
}

function getMatrix(model, callback) {
  $.ajax({
    url: '/StudentPreference/FullOptionIncludeDependency',
    contentType: 'application/json; charset=utf-8',
    dataType: 'json',
    type: 'GET',
    data: model
  }).done(function (response) {
    callback(response);
  });
}
