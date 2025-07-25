function AudioPlayer(src, options) {
  var defaultOptions = { onRemoveClick: function () {}, removeable : true };
  defaultOptions = Object.assign({}, defaultOptions, options);
  var styleContent = ".audio-player-container { background: #fff; border-radius: 8px; padding: 8px; border: 1px solid #ccc; }\n .audio-player-container button {\n  padding: 0;\n  border: 0;\n  background: transparent;\n  cursor: pointer;\n  outline: none;\n     box-shadow: none;border-radius: 0;}\n\n.audio-player-container {\n  --seek-before-width: 0%;\n  --volume-before-width: 100%;\n  --buffered-width: 0%;\n  position: relative;\n  display: flex;\n  align-items: center;\n  z-index: 1;\n}\n\n  .audio-player-container .time {\n    display: inline-block;\n    text-align: center;\n  }\n\n    .audio-player-container .time:last-of-type:before {\n      content: '/';\n      margin: 0px 2px;\n    }\n\n  .audio-player-container input[type=\"range\"] {\n    position: relative;\n    -webkit-appearance: none;\n    margin: 0;\n    padding: 0;\n    height: 16px;\n    outline: none;\n    margin: 0 6px;\n    width: 100%;\n    background-color: transparent;\n  }\n\n    .audio-player-container input[type=\"range\"]::-webkit-slider-runnable-track {\n      width: 100%;\n      height: 3px;\n      cursor: pointer;\n      background: linear-gradient( to right, rgba(0, 125, 181, 0.6) var(--buffered-width), rgba(0, 125, 181, 0.2) var(--buffered-width) );\n    }\n\n    .audio-player-container input[type=\"range\"]::before {\n      position: absolute;\n      content: \"\";\n      top: 8px;\n      left: 0;\n      width: var(--seek-before-width);\n      height: 3px;\n      background-color: #007db5;\n      cursor: pointer;\n    }\n\n    .audio-player-container input[type=\"range\"]::-webkit-slider-thumb {\n      position: relative;\n      -webkit-appearance: none;\n      box-sizing: content-box;\n      border: 1px solid #007db5;\n      height: 15px;\n      width: 15px;\n      border-radius: 50%;\n      background-color: #fff;\n      cursor: pointer;\n      margin: -7px 0 0 0;\n    }\n\n    .audio-player-container input[type=\"range\"]:active::-webkit-slider-thumb {\n      transform: scale(1.2);\n      background: #007db5;\n    }\n\n    .audio-player-container input[type=\"range\"]::-moz-range-track {\n      width: 100%;\n      height: 3px;\n      cursor: pointer;\n      background: linear-gradient( to right, rgba(0, 125, 181, 0.6) var(--buffered-width), rgba(0, 125, 181, 0.2) var(--buffered-width) );\n    }\n\n    .audio-player-container input[type=\"range\"]::-moz-range-progress {\n      background-color: #007db5;\n    }\n\n    .audio-player-container input[type=\"range\"]::-moz-focus-outer {\n      border: 0;\n    }\n\n    .audio-player-container input[type=\"range\"]::-moz-range-thumb {\n      box-sizing: content-box;\n      border: 1px solid #007db5;\n      height: 6px;\n      width: 6px;\n      border-radius: 50%;\n      background-color: #fff;\n      cursor: pointer;\n    }\n\n    .audio-player-container input[type=\"range\"]:active::-moz-range-thumb {\n      transform: scale(1.2);\n      background: #007db5;\n    }\n\n    .audio-player-container input[type=\"range\"]::-ms-track {\n      width: 100%;\n      height: 2px;\n      cursor: pointer;\n      background: transparent;\n      border: solid transparent;\n      color: transparent;\n    }\n\n    .audio-player-container input[type=\"range\"]::-ms-fill-lower {\n      background-color: #007db5;\n    }\n\n    .audio-player-container input[type=\"range\"]::-ms-fill-upper {\n      background: linear-gradient( to right, rgba(0, 125, 181, 0.6) var(--buffered-width), rgba(0, 125, 181, 0.2) var(--buffered-width) );\n    }\n\n    .audio-player-container input[type=\"range\"]::-ms-thumb {\n      box-sizing: content-box;\n      border: 1px solid #007db5;\n      height: 6px;\n      width: 6px;\n      border-radius: 50%;\n      background-color: #fff;\n      cursor: pointer;\n    }\n\n    .audio-player-container input[type=\"range\"]:active::-ms-thumb {\n      transform: scale(1.2);\n      background: #007db5;\n    }\n\n  .audio-player-container #play-icon {\n    display: inline-block;\n    box-sizing: border-box;\n    width: 0;\n    height: 12px;\n    border-color: transparent transparent transparent #202020;\n    transition: 100ms all ease;\n    cursor: pointer;\n    border-style: solid;\n    border-width: 6px 0 6px 12px;\n  }\n\n    .audio-player-container #play-icon.pause {\n      border-style: double;\n      border-width: 0px 0 0px 12px;\n    }\n\n    .audio-player-container #btn-delete-audio {\n      text-decoration: none;\n      font-weight: bold;\n     color: red;\n    margin-left: 9px;\n    font-size: 16px;\n line-height: 1; }";
  if (defaultOptions.isRecord) {
    return new AudioPlayerRecord(options, styleContent)
  }
  var audio = createAudioInstance(src, defaultOptions);
  audio.preload = 'metadata';

  var audioPlayerContainer = $('<div class="audio-player-container">' +
    '<style>' + styleContent + '</style>' +
    '<span id="current-time" class="time">0:00</span>' +
    '<span id="duration" class="time">0:00</span>' +
    '<input type="range" id="seek-slider" max="1" value="0">' +
    '<button id="play-icon"></button>' +
    (defaultOptions.removeable ? '<button id="btn-delete-audio" title="Remove">&times;</button>' : '') +
    '</div>')[0];

  if (src) {
    audio.src = src;
  }
  if (defaultOptions.muted) {
    audio.muted = true;
  }
  if (defaultOptions.srcObject) {
    audio.srcObject = defaultOptions.srcObject;
  }
  if (defaultOptions.autoplay) {
    audio.autoplay = true;
  }
  audioPlayerContainer.appendChild(audio);
  /** Implementation of the presentation of the audio player */

  var playIconContainer = audioPlayerContainer.querySelector('#play-icon');
  var seekSlider = audioPlayerContainer.querySelector('#seek-slider');
  var playState = 'play';

  playIconContainer.addEventListener('click', function () {
    if (playState === 'play') {
      audio.play();
    } else {
      audio.pause();
    }
  });

  var removeAudioFile = audioPlayerContainer.querySelector('#btn-delete-audio');
  if (removeAudioFile){
    removeAudioFile.addEventListener('click', function () {
      defaultOptions.onRemoveClick.apply(this);
    });
  }
 
  var showRangeProgress = function (rangeInput) {
    if (rangeInput === seekSlider) audioPlayerContainer.style.setProperty('--seek-before-width', rangeInput.value / rangeInput.max * 100 + '%');
    else audioPlayerContainer.style.setProperty('--volume-before-width', rangeInput.value / rangeInput.max * 100 + '%');
  }

  seekSlider.addEventListener('input', function (e) {
    showRangeProgress(e.target);
  });

  /** Implementation of the functionality of the audio player */

  var durationContainer = audioPlayerContainer.querySelector('#duration');
  var currentTimeContainer = audioPlayerContainer.querySelector('#current-time');

  var calculateTime = function (secs) {
    var minutes = Math.floor(secs / 60);
    var seconds = Math.floor(secs % 60);
    var returnedSeconds = seconds < 10 ? ('0' + seconds) : (seconds + '');
    return minutes + ':' + returnedSeconds;
  }

  var displayDuration = function () {
    getDuration(src, function (duration) {
      durationContainer.textContent = calculateTime(duration);
    });
  }

  var setSliderMax = function () {
    getDuration(src, function (duration) {
      seekSlider.max = Math.floor(duration);
    });
  }

  var displayBufferedAmount = function () {
    var length = audio.buffered.length;
    if (length > 0) {
      var bufferedAmount = Math.floor(audio.buffered.end(length));
      audioPlayerContainer.style.setProperty('--buffered-width', (bufferedAmount / seekSlider.max * 100) + '%');
    }
  }

  var whilePlaying = function () {
    seekSlider.value = Math.floor(audio.currentTime);
    currentTimeContainer.textContent = calculateTime(Math.floor(audio.currentTime));
    audioPlayerContainer.style.setProperty('--seek-before-width', (seekSlider.value / seekSlider.max * 100) + '%');
  }

  if (audio.readyState > 0) {
    displayDuration();
    setSliderMax();
    displayBufferedAmount();
  } else {
    audio.addEventListener('loadedmetadata', function () {
      displayDuration();
      setSliderMax();
      displayBufferedAmount();
    });
  }

  audio.addEventListener('progress', displayBufferedAmount);

  seekSlider.addEventListener('input', function () {
    currentTimeContainer.textContent = calculateTime(seekSlider.value);
  });

  seekSlider.addEventListener('change', function () {
    audio.currentTime = seekSlider.value;
    if (!audio.paused) {
      whilePlaying();
    }
  });

  audio.addEventListener('pause', function () {
    playState = 'play';
    playIconContainer.classList.remove('pause');
  });

  audio.addEventListener('play', function () {
    whilePlaying();
    playState = 'pause';
    playIconContainer.classList.add('pause');
  });

  audio.addEventListener('timeupdate', whilePlaying);
  var getDuration = function (url, next) {
    var _player = createAudioInstance(url);
    var duration = 0;
    _player.addEventListener("durationchange", function (e) {
      if (this.duration != Infinity && duration === 0) {
        duration = this.duration
        _player.remove();
        next(duration);
      };
    }, false);
    _player.load();
    _player.currentTime = 24 * 60 * 60; //fake big time
    _player.volume = 0;
    //waiting...
  };
  function createAudioInstance(url, opts) {
    const audioElement = document.createElement('audio');
    audioElement.preload = 'metadata';

    if (opts && opts.muted) {
      audioElement.muted = true;
    }
    if (opts && opts.srcObject) {
      if ('srcObject' in audioElement) {
        audioElement.srcObject = opts.srcObject;
      } else {
        // Avoid using this in new browsers, as it is going away.
        audioElement.src = opts.srcObject;
      }
    }
    if (opts && opts.autoplay) {
      audioElement.autoplay = true;
    }
    if (url) {
      var sourceElement = document.createElement('source');
      audioElement.appendChild(sourceElement);
      sourceElement.src = url;
      sourceElement.type = 'audio/mp3';
    }
    return audioElement;
  }
  return audioPlayerContainer;
}

AudioPlayer.isSupportedType = function(type) {
  var formats = {
    mp3: 'audio/mpeg;codecs=opus',
    mp4: 'audio/mp4;codecs=opus',
    webm: 'audio/webm;codecs=opus',
    ogg: 'audio/ogg;codecs=opus',
    wav: 'audio/wav;codecs=opus'
  };

  return new Audio().canPlayType(formats[type] || type);
}

function AudioPlayerRecord(options, styles) {
  var defaultOptions = { };
  defaultOptions = Object.assign({}, defaultOptions, options);

  var audioPlayerContainer = $('<div class="audio-player-container">' +
    '<style>' + styles + '</style>' +
    '<span id="current-time" class="time">0:00</span>' +
    '<span id="duration" class="time">0:00</span>' +
    '<input type="range" id="seek-slider" max="0" value="0">' +
    '</div>');

  var currentTimeContainer = audioPlayerContainer.find('#current-time');

  var calculateTime = function (secs) {
    var minutes = Math.floor(secs / 60);
    var seconds = Math.floor(secs % 60);
    var returnedSeconds = seconds < 10 ? ('0' + seconds) : (seconds + '');
    return minutes + ':' + returnedSeconds;
  }
  var recordTimer = 0;
  intervalTimer = setInterval(function () {
    if (!audioPlayerContainer.parent().length) {
      clearInterval(intervalTimer);
      return;
    }
    recordTimer += 1;
    currentTimeContainer.text(calculateTime(recordTimer));
  }, 1000);
  return audioPlayerContainer;
}
