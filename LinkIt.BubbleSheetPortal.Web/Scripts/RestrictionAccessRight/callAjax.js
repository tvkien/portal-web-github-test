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
