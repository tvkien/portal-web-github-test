var customConfirm = function(msg) {
  return new Promise(function(resolve) {
    resolve(confirm(msg));
  })
};
var customAlert = function(msg) {
  popupAlertMessage('alert', msg, 420, 100);
};