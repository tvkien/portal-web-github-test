function Reviewer() {};

Reviewer.IsNullOrEmpty = function (value) {
    return typeof (value) === 'undefined' || value === null || $.trim(value) === '';
};

Reviewer.ParseInt = function (value) {
    if (typeof (value) === 'undefined' || value === null || $.trim(value) === '') {
        return 0;
    }

    return parseInt(value, 10);
};

Reviewer.GetGuidanceRationaleContent = function(ratio) {
    var emptyRationale = false;
    var $ratio = $(ratio);

    if($ratio.find('img, video').length > 0){
        emptyRationale = true;
    } else if ($.trim($ratio.text()) !== ''){
        emptyRationale = true;
    }

    return emptyRationale;
};


Reviewer.popupAlertMessage = function (content, contentClass, w, h, isMessageContent) {
    isMessageContent = typeof isMessageContent !== 'undefined' ? isMessageContent : true;
    var now = new Date().getTime();
    var contentHtml = '';
    var $div = $('<div />');

    w = w !== undefined ? w : 400;
    h = h !== undefined ? h : 100;

    contentHtml += '<div class="popup-fullpage">';
    isMessageContent ? contentHtml += `<div class="popup-fullpage-content" style="font-weight: 700; max-height: ${h}px">` : contentHtml += `<div class="popup-fullpage-content" style="max-height: ${h}px">`
    contentHtml += content;
    contentHtml += '</div>';
    contentHtml += '<div class="popup-fullpage-controls">';
    contentHtml += '<button class="btn-cancel-button" id="btn-cancel-' + now + '">Close</button>';
    contentHtml += '</div>';
    contentHtml += '</div>';

    $div.html(contentHtml)
      .attr('id', 'popup-fullpage-' + now)
      .appendTo('body')
      .dialog({
        modal: true,
        width: w,
        resizable: false,
        dialogClass: contentClass,
        close: function () {
          $(document).find('#popup-fullpage-' + now).dialog('destroy').remove();
        }
      });

    // Close Popup Show Full Page
    $(document).on('click', '#btn-cancel-' + now, function () {
      $(document).find('#popup-fullpage-' + now).dialog('destroy').remove();
    });
};

Reviewer.vnsAudio = function(source) {
    var config = source;
    var audioId = 'vnsAudio';

    this.init = function () {
        var self = this;

        if (!config.src) {
            var msg = ('URL for Audio should be defined');
            Reviewer.popupAlertMessage(msg, 'ui-popup-fullpage ui-popup-fullpage-nostudent', 350, 100);
            return;
        }

        if (!$('#' + audioId).length) {
            // Adding DOM
            var player = $('<audio/>', {
                id: audioId,
                src: config.src
            }).appendTo('body');
        } else {
            $('#' + audioId).attr('src', config.src);
        }

        // Apply Player
        this.audio = new MediaElement(audioId, {
            success: function(self) {
                self.play();
                var emptyFn = function() {};
                // Add Listeners
                self.addEventListener('play', (config.onPlay || emptyFn), false);
                self.addEventListener('pause', (config.onPause || emptyFn), false);
                self.addEventListener('ended', (config.onEnded || emptyFn), false);
            }
        });
    };

    this.play = function () {
        if (this.audio == undefined) {
            this.init();
        }
        this.audio.setCurrentTime(0);
        this.audio.play();
    };

    this.pause = function () {
        if (this.audio == undefined) {
            return;
        }
        this.audio.pause();
    };

    this.init();
};

Reviewer.playVNSAudio = function(element) {
    var $element = $(element);
    var elementAudio = '';

    $element.next().show();
    $element.hide();

    elementAudio = $element.parent().find('.audioRef').text();

    if (window.playsound !== undefined) {
        window.playsound.pause();
    }

    // Direct link from S3
    if (elementAudio &&
        !(elementAudio.indexOf('http') >= 0) &&
        window.S3Domain !== undefined) {
        if (elementAudio.charAt(0) === '/') {
            elementAudio = elementAudio.substring(1);
        }

        elementAudio =  window.S3Domain + elementAudio;
    }

    window.playsound = new Reviewer.vnsAudio({
        src: elementAudio,
        onEnded: function () {
            $element.next().hide();
            $element.show();
        }
    });
};

Reviewer.stopVNSAudio = function(element) {
    var $element = $(element);

    $element.prev().show();
    $element.hide();

    if (window.playsound !== undefined) {
        window.playsound.pause();
    }
};

Reviewer.isString = function (str) {
    return Object.prototype.toString.call(str) === '[object String]';
};

Reviewer.replaceStringLessOrLarge = function (str) {
    if (Reviewer.isString(str)) {
        return str.replace(/&#60;/g, '<').replace(/&#62;/g, '>');
    }

    return '';
};

Reviewer.prettyTime = function (time) {
    var hours = Math.floor(time / 3600);
    var mins = Math.floor(time % 3600 / 60);
    var secs = Math.floor(time % 60);

    var result = '';
    if (!isNaN(secs)) {
        if (hours) {
            result = hours + 'h ';
        }

        if (mins) {
            result = result + mins + 'm ';
        }
        if(secs)
            result = result + secs + 's';
    }

    return result;
};

Reviewer.createTabWidget = function (el, algorithmicPoints) {
    var $el = $(el);

    $el.find('.popup-fullpage-content > div').prepend('<div class="box-tab"/>');
    $el.find('.popup-fullpage-content').addClass('is-algorithmic');

    $el.find('.box-answer').hide();
    $el.find('.box-answer').each(function (indexBox, box) {
        var $box = $(box);
        var tab = Reviewer.createTabItem(algorithmicPoints[indexBox]());
        $el.find('.box-tab').append(tab);

        if (!indexBox) {
            tab.className += ' is-active';
            $box.show();
        }
    });

    if ($el.find('.box-answer').length === 1) {
        $el.find('.box-title').addClass('is-alone');
    }
    MathJax.Hub.Queue(["Typeset", MathJax.Hub]);
};

Reviewer.createTabItem = function (pointsEarned) {
    var tab = document.createElement('div');

    tab.className = 'box-title';
    tab.textContent = 'Max Points: ' + pointsEarned;
    tab.addEventListener('click', Reviewer.changeTabItem);

    return tab;
};

Reviewer.changeTabItem = function () {
    var $currentTab = $(this);
    var currentTabIndex = $currentTab.index();
    var $tabTitle = $('.popup-fullpage-content .box-title');
    var $tabContent = $('.popup-fullpage-content > div .box-answer');

    $tabTitle.removeClass('is-active');
    $tabContent.hide();
    $tabContent.eq(currentTabIndex).show();
    $currentTab.addClass('is-active');
};


Reviewer.replaceParagraph = function (str) {
    return str.replace(/<p>/g, '<div>')
                .replace(/<p /g, '<div ')
                .replace(/<\/p>/g, '</div>');
};


Reviewer.getAtleast = function (amount, pointEarned, qtischemaid) {
    var elAtleast = document.createElement('div');
    var elAtLeastTextAnswer = amount > 1 ? 'answers' : 'answer';
    var elAtLeastTextPoint = pointEarned > 1 ? 'points' : 'point';
    var isQuestionMultipleChoice = [1, 3];
    var isQuestionDragDrop = [30, 35, 36];
    var isQuestionHotspot = [31, 32, 33, 34];

    elAtleast.className = 'box-answer-atleast';

    if (isQuestionMultipleChoice.indexOf(qtischemaid) > -1) {
        elAtleast.innerHTML = 'At least <b>' + amount + '</b>  ' + elAtLeastTextAnswer + ' choice is selected, student will earn <b>' + pointEarned + '</b> ' + elAtLeastTextPoint + '.';
    } else if (isQuestionDragDrop.indexOf(qtischemaid) > -1) {
        elAtleast.innerHTML = 'At least <b>' + amount + '</b>  ' + elAtLeastTextAnswer + ' attempt that source is dragged and dropped into destination, student will earn <b>' + pointEarned + '</b> ' + elAtLeastTextPoint + '.';
    } else if (isQuestionHotspot.indexOf(qtischemaid) > -1) {
        elAtleast.innerHTML = 'At least <b>' + amount + '</b>  ' + elAtLeastTextAnswer + ' hotspot is marked, student will earn <b>' + pointEarned + '</b> ' + elAtLeastTextPoint + '.';
    }

    return elAtleast;
};

Reviewer.getCorrectAnswerDnd = function (question, mappingCorrects) {
    var $question = $(question);
    var correctAnswerDnd = document.createElement('div');

    for (var i = 0, len = mappingCorrects.length; i < len; i++) {
        var mapping = mappingCorrects[i];
        var mappingChild = Reviewer.getCorrectAnswerDndItem(question, mapping);
        correctAnswerDnd.appendChild(mappingChild);
    }

    return correctAnswerDnd;
};

Reviewer.getCorrectAnswerDndItem = function (question, mapping) {
    var item = document.createElement('div');
    item.className = 'box-answer-correct-dnd';
    var itemDesc = '';
    var itemSource = '';

    var mappingArray = mapping.split(',');
    for (var i = 0; i < mappingArray.length; i++) {
        var mappingSub = mappingArray[i].split('-');

        itemDesc = mappingSub[0];
        itemSource = question.find('sourceobject[srcidentifier="' + mappingSub[1] + '"]').clone(true).outerHTML();
        item.innerHTML += itemDesc + ' - ' + itemSource + '</br>  ';
    }
    return item;
};
