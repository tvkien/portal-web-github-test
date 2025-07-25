function RecordBase(opts) {
  var defaultOption = {
    audio: true,
    video: false
  };
  this.options = Object.assign({}, defaultOption, opts || {});
}

RecordBase.isEdge =
  navigator.userAgent.indexOf('Edge') !== -1 &&
  (!!navigator.msSaveOrOpenBlob || !!navigator.msSaveBlob);
RecordBase.isSafari = /^((?!chrome|android).)*safari/i.test(navigator.userAgent);
RecordBase.isFirefox = navigator.userAgent.indexOf('Firefox') !== -1;

RecordBase.prototype.captureUserMedia = function (onSuccess, onError) {
  onError = onError || $.noop;

  navigator.getUserMedia =
    navigator.getUserMedia ||
    navigator.webkitGetUserMedia ||
    navigator.mozGetUserMedia ||
    navigator.msGetUserMedia;

  if (!!navigator.getUserMedia) {
    navigator.getUserMedia(this.options, onSuccess, onError);
    return;
  }
  if (!navigator.mediaDevices || !navigator.mediaDevices.getUserMedia) {
    onError(Error('This browser does not supports WebRTC getUserMedia API.'));
    return;
  }

  navigator.mediaDevices
    .getUserMedia(this.options)
    .then(onSuccess)
  ['catch'](onError);
};

// stop both mic and camera
RecordBase.prototype.stopBothVideoAndAudio = function (stream) {
  if (stream) {
    stream.getTracks().forEach(function (track) {
      if (track.readyState == 'live') {
        track.stop();
      }
    });
  }

};

// stop only camera
RecordBase.prototype.stopVideoOnly = function (stream) {
  if (stream) {
    stream.getTracks().forEach(function (track) {
      if (track.readyState == 'live' && track.kind === 'video') {
        track.stop();
      }
    });
  }
};

// stop only mic
RecordBase.prototype.stopAudioOnly = function (stream) {
  if (stream) {
    stream.getTracks().forEach(function (track) {
      if (track.readyState == 'live' && track.kind === 'audio') {
        track.stop();
      }
    });
  }
};

RecordBase.prototype.getAllSupportedMimeTypes = function () {
  if (!window.MediaRecorder) return [];
  for (
    var _len = arguments.length, mediaTypes = Array(_len), _key = 0;
    _key < _len;
    _key++
  ) {
    mediaTypes[_key] = arguments[_key];
  }

  if (!mediaTypes.length) mediaTypes.push.apply(mediaTypes, ['video', 'audio']);
  var FILE_EXTENSIONS = ['webm', 'mp3', 'ogg', 'mp4', 'wav'];
  return FILE_EXTENSIONS.reduce(function (accum, ext) {
    mediaTypes.forEach(function (mediaType) {
      var variation = mediaType + '/' + ext;
      if (MediaRecorder.isTypeSupported(variation)) {
        accum.push(variation);
      }
    });
    return accum;
  }, []);
};

function extend(base, constructor) {
  var prototype = new Function();
  prototype.prototype = base.prototype;
  constructor.prototype = new prototype();
  constructor.prototype.constructor = constructor;
}
