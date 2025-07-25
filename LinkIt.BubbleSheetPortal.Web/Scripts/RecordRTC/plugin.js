function RecordAudio() {
  var self = this;

  RecordBase.call(self, {
    audio: true,
    video: false
  });

  var recorder; // globally accessible
  var microphone;
  var arrayOfBlobs = [];
  this.supportedType = self.getAllSupportedMimeTypes('audio');
  function getArrayOfBlobs() {
    return arrayOfBlobs;
  }
  function record(options) {
    if (
      navigator.platform &&
      navigator.platform.toString().toLowerCase().indexOf('win') === -1
    ) {
      options.sampleRate = 48000; // or 44100 or remove this line for default
    }

    options.sampleRate = 44100;
    options.bufferSize = 4096;
    options.numberOfAudioChannels = 2;
    options.ondataavailable = function (blob) {
      arrayOfBlobs.push(blob);
    }
    if (recorder) {
      recorder.destroy();
      recorder = null;
    }

    recorder = RecordRTC(microphone, options);

    recorder.startRecording();
  }

  self.startRecord = function (options, callback, onError) {
    var defaultOptions = Object.assign({}, options);
    if (microphone) {
      self.stopBothVideoAndAudio(microphone);
      microphone = null;
    }

    self.captureUserMedia(
      function (mic) {
        microphone = mic;
        var recordoptions = {
          type: 'audio',
          numberOfAudioChannels: RecordBase.isEdge ? 1 : 2,
          checkForInactiveTracks: true,
          bufferSize: 16384,
          timeSlice: 100
        };
        recordoptions.recorderType = RecordRTC.StereoAudioRecorder;
        recordoptions.mimeType = 'audio/wav';
        record(recordoptions);
        if (defaultOptions.fileSizeLimit) {
          // - 5%
          var limit = Math.round(defaultOptions.fileSizeLimit * 0.95);
          (function looper() {
            if (!recorder) {
              return;
            }
            var blob = new Blob(getArrayOfBlobs());
            if (blob.size >= limit) {
              onError({
                type: 'fileSizeLimit',
                message: 'File size exceeds limit of ' + RecordRTC.bytesToSize(defaultOptions.fileSizeLimit) + '.',
                blob: blob
              });
              return;
            }
            setTimeout(looper, 500);
          })();
        }

        callback();
      },
      function (error) {
        onError(error);
      }
    );
  };

  self.stopRecord = function (callback, fileName) {
    recorder.stopRecording(function () {
      var blob = recorder.getBlob();
      var file = new File([blob], fileName, {
        type: 'audio/wav'
      });
      arrayOfBlobs = [];
      callback(file);
      if (microphone) {
        self.stopBothVideoAndAudio(microphone);
        microphone = null;
      }
    });
  };
};
extend(RecordBase, RecordAudio);

function RecordVideo (opts) {
  var facingMode = {
    back: 'environment',
    front: 'user'
  };

  var self = this;
  this.recorder = null;
  this.currentCamera = facingMode.back;
  var defaultOpts = Object.assign(
    {},
    {
      audio: true,
      video: {
        facingMode: this.currentCamera
      }
    },
    opts
  );
  RecordBase.call(self, defaultOpts);
  if (RecordBase.isSafari) {
    self.type = "video/mp4";
  } else if (RecordBase.isFirefox) {
    self.type = 'video/webm;codecs=h246';
  } else {
    self.type = 'video/webm;codecs=vp8';
  }
  self.ext = RecordBase.isSafari ? 'mp4' : 'webm';

  this.stopBothVideoAndAudio = function (stream) {
    stream = stream || this.cameraStream;
    if (stream) {
      stream.getTracks().forEach(function (track) {
        if (track.readyState == 'live') {
          track.stop();
        }
      });
    }
  };

  this.captureUserMedia = function (callback, onError) {
    onError = onError || $.noop;

    navigator.getUserMedia =
      navigator.getUserMedia ||
      navigator.webkitGetUserMedia ||
      navigator.mozGetUserMedia ||
      navigator.msGetUserMedia;

    if (!!navigator.getUserMedia) {
      navigator.getUserMedia(defaultOpts, callback, onError);
      return;
    }
    if (!navigator.mediaDevices || !navigator.mediaDevices.getUserMedia) {
      onError(Error('This browser does not supports WebRTC getUserMedia API.'));
      return;
    }

    navigator.mediaDevices
      .getUserMedia(defaultOpts)
      .then(function (stream) {
        callback(stream);
      })['catch'](onError);
  }

  this.startCamera = function (onSuccess, onError) {
    this.captureUserMedia(function (stream) {
      self.cameraStream = stream;
      if (onSuccess) {
        onSuccess(stream);
      }
    }, onError);
  };

  this.swicthCamera = function (onSuccess, onError) {
    var supports = navigator.mediaDevices.getSupportedConstraints();
    if (!supports['facingMode']) {
      onError(new Error('your browser is not supported Facing mode.'));
      return;
    }
    this.stopCamera();
    this.currentCamera =
      this.currentCamera == facingMode.back
        ? facingMode.front
        : facingMode.back;
    this.options.video.facingMode = this.currentCamera;
    this.startCamera(onSuccess, onError);
  };

  this.stopCamera = function () {
    if (this.cameraStream) {
      self.stopBothVideoAndAudio(this.cameraStream);
    }
  };

  this.captureImage = function (videoElm, callback) {
    var canvas = document.createElement('canvas');
    var ctx = canvas.getContext('2d');
    canvas.height = videoElm.videoHeight;
    canvas.width = videoElm.videoWidth;
    ctx.drawImage(videoElm, 0, 0);
    canvas.toBlob(function (blob) {
      callback(blob);
    }, 'image/jpeg');
  };

  this.startRecord = function (options, callback, onError) {
    var defaultOptions = Object.assign({}, options);

    function handler() {
      self.recorder = new RecordRTC(self.cameraStream, {
        type: 'video',
        timeSlice: 100,
        mimeType: self.type
      });
      self.recorder.startRecording();
      self.recorder.camera = self.cameraStream;
      if (defaultOptions.fileSizeLimit) {
        // - 5%
        var limit = Math.round(defaultOptions.fileSizeLimit * 0.95);
        (function looper() {
          if (!self.recorder) {
            return;
          }
          var internal = self.recorder.getInternalRecorder();
          if (internal && internal.getArrayOfBlobs) {
            var blob = new Blob(internal.getArrayOfBlobs());
            if (blob.size >= limit) {
              onError({
                type: 'fileSizeLimit',
                message: 'File size exceeds limit of ' + RecordRTC.bytesToSize(defaultOptions.fileSizeLimit) + '.',
                blob: blob
              });
              return;
            }
          }
          setTimeout(looper, 500);
        })();
      }
      callback();
    }
    try {
      if (!self.cameraStream) {
        self.startCamera(function () {
          handler();
        });
        return;
      }
      handler();
    } catch (err) {
      onError(err);
    }
  }

  this.stopRecord = function (callback, fileName) {
    if (self.recorder) {
      self.recorder.stopRecording(function () {
        var blob = self.recorder.getBlob();
        var file = new File([blob], fileName || 'video-record.' + self.ext, {
          type: self.type
        });
        callback(file);
      });
    }
  }
};
extend(RecordBase, RecordVideo);
