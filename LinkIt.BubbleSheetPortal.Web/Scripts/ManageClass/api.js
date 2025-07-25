var url = window.location.protocol + '//' + window.location.host;

function getStates(url, getStateData) {
  $.ajax(url).done(function(response) {
    if(response.status === 'success') {
      data = response.data
      getStateData(response.data)
    }
  });
}

function GetDistrictByStateId(url, GetDistrictByStateIdData) {
  $.ajax(url).done(function(response) {
    if(response.status === 'success') {
      data = response.data
      GetDistrictByStateIdData(response.data)
    }
  });
}

function GetSchoolByDistrictId(url, GetSchoolByDistrictIdData) {
  $.ajax(url).done(function(response) {
    if(response.status === 'success') {
      data = response.data
      GetSchoolByDistrictIdData(response.data)
    }
  });
}

function getCurrentUser(callback) {
  $.ajax(url + '/CategoriesAPI/GetCurrentUser').done(function(response) {
    if(response.status === 'success') {
      callback(response.data);
    }
  });
}