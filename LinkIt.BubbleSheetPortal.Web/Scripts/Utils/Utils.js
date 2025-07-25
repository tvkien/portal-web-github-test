function convertToBoolean(input) {
    if (input == null)
        return false;
    input = input.trim().toLowerCase();
    return input === 'true';
}

function formatDate(date, format) {
    var d = new Date(date),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear(),
        hours = '' + d.getHours(),
        min = '' + d.getMinutes()
        

    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;
    if (hours.length < 2) hours = '0' + hours;
    if (min.length < 2) min = '0' + min;

    switch (format) {
        case 'MM/dd/yyyy hh:mm':
            return month + "/" + day + "/" + year + " " + hours + ":" + min
        default:
            return [month, day, year].join('/');
    }
}

// Convert 20:00 -> 08:00 PM
function formatTime(time) {
    time = time.toString().match(/^([01]\d|2[0-3])(:)([0-5]\d)(:[0-5]\d)?$/) || [time];

    if (time.length > 1) {
        time = time.slice(1);
        time[5] = +time[0] < 12 ? ' AM' : ' PM';
        time[0] = +time[0] % 12 || 12;
    }
    return time.join('');
}

function formatTime12To24(time) {
    var hours = Number(time.match(/^(\d+)/)[1]);
    var minutes = Number(time.match(/:(\d+)/)[1]);
    var AMPM = time.match(/\s(.*)$/)[1];
    if (AMPM == "PM" && hours < 12) hours = hours + 12;
    if (AMPM == "AM" && hours == 12) hours = hours - 12;
    var sHours = hours.toString();
    var sMinutes = minutes.toString();
    if (hours < 10) sHours = "0" + sHours;
    if (minutes < 10) sMinutes = "0" + sMinutes;
    return sHours + ":" + sMinutes;
}

function CheckValidateObject(obj) {
  if (obj !== 'select' && obj !== '' && obj !== null && obj !== undefined)
    return true;
  else
    return false;
}

$(function () {
  if (sessionStorage.KEEP_SESSION) {
    var data = JSON.parse(sessionStorage.KEEP_SESSION);
    if (_CURRENT_PAGE_NAME && data && data.rootPage !== _CURRENT_PAGE_NAME && data.subPages.indexOf(_CURRENT_PAGE_NAME) === -1) {
      sessionStorage.removeItem("KEEP_SESSION");
    }
  }

  $('.menu-item li').on('click', function () {
    sessionStorage.removeItem("KEEP_SESSION");
  })
});

var SessionTimeOutUtils = (function () {
  function minutesForExpireFormat(seconds) {
    var mm = Math.floor(seconds / 60);
    var ss = seconds - (mm * 60);
    var mmResult = '';
    var ssResult = '';
    var result;

    // Round seconds
    ss = Math.round(ss * 100) / 100;

    if (mm > 0) {
      mmResult = mm <= 1  ? mm + ' minute' : mm + ' minutes'
    }

    ssResult = ss <= 1 ? ss + ' second' : ss + ' seconds';

    if (mmResult !== '') {
      result = mmResult + ' ' + ssResult;
    } else {
      result = ssResult;
    }

    return result;
  }

  function getTodayBySeconds() {
    return new Date().getTime() / 1000 | 0;
  }

  function getParamFromURLQuery(url_string, param) {
    var url = new URL(url_string);
    return url.searchParams.get(param);
  }

  return {
    minutesForExpireFormat: minutesForExpireFormat,
    getTodayBySeconds: getTodayBySeconds,
    getParamFromURLQuery: getParamFromURLQuery
  }
})();
