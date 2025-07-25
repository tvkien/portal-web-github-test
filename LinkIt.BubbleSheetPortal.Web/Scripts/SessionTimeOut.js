var url = window.location.protocol + '//' + window.location.host;
var checkTimeOutUrl = url + '/Home/GetConfigTimeOutWarning';
var sessionKeepAliveUrl = url + '/Home/KeepAlive';

var SessionTimeOutComponent = new Vue({
  el: '#notificationTimeout',
  data: {
    isShowModalTimeOutWarning: false,
    isShowTimeOutWarning: false,
    totalSecondRemaning: 0,
    sessionTimeoutWarning: 1,
    isSessionExpire: false,
    defaultRemainingSeconds: 0,
    sessionCountdownIds: [],
    idleTime: 0 // mins
  },
  created: function () {
    this.checkShowWarning();

    window.addEventListener('click', this.handlerSessionKeepAlive);
    window.addEventListener('keydown', this.handlerSessionKeepAlive);
    window.addEventListener('scroll', this.handlerSessionKeepAlive);
  },
  computed: {
    minutesForExpireFormat: function () {
      return SessionTimeOutUtils.minutesForExpireFormat(this.totalSecondRemaning);      
    }
  },
  methods: {
    checkShowWarning: function () {
      var self = this;
      $.ajax(checkTimeOutUrl).done(function (result) {
        self.isShowTimeOutWarning = result.ShowTimeOutWarning;
        self.sessionTimeoutWarning = result.WarningTimeoutMinues;
        self.defaultRemainingSeconds = self.sessionTimeoutWarning * 60;
        self.sessionTimeout = result.DefaultCookieTimeOutMinutes;

        if (self.isShowTimeOutWarning) {
          self.setInvervalCheckExpire(self);
        }

        var stepToCheckIdle = parseInt(self.defaultRemainingSeconds / 3);
        setInterval(function () {
          self.idleTime += stepToCheckIdle;
        }, stepToCheckIdle);
      });
    },
    setInvervalCheckExpire: function (self) {
      self.totalSecondRemaning = self.defaultRemainingSeconds;

      setInterval(function () {
        var expireTime = new Date(decodeURI($.cookie('WarningExpire')) + ' UTC');
        var now = new Date();
        expireTime.setMinutes(expireTime.getMinutes() - self.sessionTimeoutWarning);
        if (expireTime <= now) {
          if (!self.isShowModalTimeOutWarning) {
            self.isShowModalTimeOutWarning = true;
            self.sessionCountdownIds.push(setInterval(function () {
              self.totalSecondRemaning -= 1;

              if (self.totalSecondRemaning < 0) {
                clearInterval(self.sessionCountdownId);
                self.isShowModalTimeOutWarning = false;
                for (var i = 0; i < self.sessionCountdownIds.length; i++) {
                  clearInterval(self.sessionCountdownIds[i]);
                }
                window.location = window.location.origin + '/Account/LogOff';
              }
            }, 1000));
          }
          
        } else {
          self.isShowModalTimeOutWarning = false;
        }
      }, 60000);
    },

    handlerSessionKeepAlive: function (e) {
      self.idleTime = 0;
    },
    sessionKeepAlive: function () {
      var self = this;
      self.isShowModalTimeOutWarning = false;

      for (var i = 0; i < self.sessionCountdownIds.length; i++) {
        clearInterval(self.sessionCountdownIds[i]);
      }
      

      $.get(sessionKeepAliveUrl, function (response) {
      self.totalSecondRemaning = self.defaultRemainingSeconds;
      });
    }
  }
});
