var recordStatus = {
  stoped: 0,
  staring: 1,
  recording: 2
}

var prevewExtsSuport = (function () {
  var ua = navigator.userAgent.toLowerCase();
  var isSafari = ua.indexOf('safari') != -1 && ua.indexOf('chrome') == -1;
  var image = ['apng', 'avif', 'gif', 'jpg', 'jpeg', 'jfif', 'pjpeg', 'pjp', 'png', 'svg', 'ico', 'cur'];
  var audio = ['mp3', 'wav'];
  var video = ['webm', 'mp4'];
  if (!isSafari) {
    audio.push('ogg');
    video.push('ogg');
  }
  return {
    image: image,
    audio: audio,
    video: video
  }
})();

var commonsMixin = {
  methods: {
    getFileExtention: function (fileName) {
      return (/[.]/.exec(fileName) ? /[^.]+$/.exec(fileName) : [''])[0].toLowerCase();
    },
  }
}

var attachmentsMixin = {
  mixins: [commonsMixin],
  props: {
    items: {
      type: Array,
      default: () => ([])
    }
  },
  methods: {
    getFileNameWithIndex: function (fileName, isRecord) {
      var index = isRecord ? 1 : 0;
      var isExistIndex = true;
      var ext = this.getFileExtention(fileName);
      var originName = fileName.substr(0, fileName.lastIndexOf('.'));
      var curentName = '';
      while (isExistIndex) {
        if (index == 0) {
          curentName = originName;
        } else {
          curentName = isRecord ? (originName + ' ' + index) : (originName + ' (' + index + ')');
        }
        curentName += ('.' + ext);
        isExistIndex = this.items.some(function (file) {
          return file.Name.toLowerCase() == curentName.toLowerCase();
        });
        if (isExistIndex) {
          index += 1;
        }
      };
      return curentName;
    },
  },
  filters: {
    timerFormat: function (secs) {
      var minutes = Math.floor(secs / 60);
      var seconds = Math.floor(secs % 60);
      var hours = Math.floor(secs / 3600);

      if (minutes < 10) { minutes = "0" + minutes; }
      if (seconds < 10) { seconds = "0" + seconds; }

      if (hours < 1) return minutes + ':' + seconds;

      if (hours < 10) { hours = "0" + hours; }

      return hours + ':' + minutes + ':' + returnedSeconds;
    }
  },
}

var recordMixin = {
  data: function () {
    return {
      status: recordStatus.stoped,
      recordTime: 0,
      intervalTimer: null,
      recordStatus: recordStatus,
    }
  },
  methods: {
    handleRecordClick: function () {
      if (this.status === recordStatus.stoped) {
        this.handleStartRecord();
      } else {
        this.handleStopRecord();
      }
    },
    handleStartRecord: function () {
      var self = this;
      self.status = recordStatus.staring;
      self.$root.shared.isRecording = true;
      var ext = self.getFileExtention(self.getFileName());
      var type = globalConfig.FileTypeGroups.find(function (i) {
        return i.SupportFileType.includes('.' + ext);
      });
      self.recorderInstance.startRecord({ fileSizeLimit: type.MaxFileSizeInBytes }, function () {
        self.startTimer();
        self.status = recordStatus.recording;
      }, function (err) {
        if (err.type == 'fileSizeLimit') {
          self.handleStopRecord();
          CustomAlert(err.message, false, null, 10000);
          return;
        }
        self.$root.shared.isRecording = false;
        self.stopTimer();
        self.status = recordStatus.stoped;
      });
    },
    handleStopRecord: function () {
      var self = this;
      if (self.status !== recordStatus.stoped) {
        self.status = recordStatus.stoped;
        self.$root.shared.isRecording = false;
        self.stopTimer();
        var fileName = self.getFileName();
        self.recorderInstance.stopRecord(function (file) {
          self.$emit('onAddItems', [{ file: file, isCapture: true }]);
        }, fileName);
      }
    },
    startTimer: function () {
      var self = this;
      self.recordTime = 0;
      self.intervalTimer = setInterval(function () {
        self.recordTime += 1;
      }, 1000);
    },
    stopTimer: function () {
      clearInterval(this.intervalTimer);
      this.intervalTimer = null;
      this.recordTime = 0;
    },
    getFileName: function () {
      return 'fileName.webm';
    }
  },
}
