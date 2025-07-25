(function ($) {
    var $body = $('body');
    var $showTeacherRationale = $('#ShowTeacherRationale');
    var $showStudentGuidance = $('#ShowStudentGuidance');

    // Show and hide teacher rationale
    $showTeacherRationale.on('click', function() {
        var $self = $(this);
        var $tooltipRationale = $('.Icon-rationale');

        if ($self.is(':checked')) {
            $tooltipRationale.css('display', 'inline-block');
        } else {
            $tooltipRationale.css('display', 'none');
        }
    });

    // Show and hide student guidance
    $showStudentGuidance.on('click', function () {
        var $self = $(this);
        var $tooltipGuidance = $('.Icon-guidance');

        if ($self.is(':checked')) {
            $tooltipGuidance.css('display', 'inline-block');
        } else {
            $tooltipGuidance.css('display', 'none');
        }
    });

    // Show Guidance/Rationale Content
    $body.on('mouseover', '.Icon-tooltip', function(e) {
        var $tooltip = $(this);
        var tooltipData = $tooltip.data('type');
        var tooltipContent = '';
        var $element = $('<div/>');

        // Multiple choice and inline choice
        if ($tooltip.parents('.choiceInteraction').length ||
            $tooltip.parents('.inlineChoiceInteraction').length) {
            var $parent = $tooltip.parents('li');

            $element.html($parent.find('div[typemessage="' + tooltipData + '"]').clone(true));
        }

        // Text entry
        if ($tooltip.data('id') !== undefined &&
            $tooltip.data('id') !== '') {
            var dataId = $tooltip.data('id');
            var dataType = $tooltip.data('type');

            $body.find('assessmentitem responsedeclaration').each(function(ind, response) {
                var $response = $(response);
                var responseId = $response.attr('identifier');

                if (responseId === dataId) {
                    var $responserubic = $response.find('responserubric');

                    $responserubic.find('value').each(function(ind, value) {
                        var $value = $(value);
                        var valueType = $value.attr('type');

                        if (existContentGuidanceRationale($value)) {
                            if (valueType === 'guidance_rationale' ||
                                valueType === dataType) {
                                var $div = $('<div/>');
                                var $divTypeMessageContainer = $('<div/>');
                                var $divTypeMessage = $('<div/>');
                                var valueAnsidentifier = $value.attr('ansidentifier');
                                var valueLabel = $responserubic
                                                    .prev('correctresponse')
                                                    .find('value[identifier="' + valueAnsidentifier + '"]')
                                                    .text();

                                copyAttributes($value, $divTypeMessage);

                                $divTypeMessage.attr('typemessage', '');
                                $divTypeMessage.append($value.html());

                                $divTypeMessageContainer.append($divTypeMessage);

                                $div.addClass('guidance');
                                $div.append('<div class="guidance-label">' + valueLabel + '</div>');
                                $div.append('<div class="guidance-content">' + $divTypeMessageContainer.html() + '</div>');

                                $element.append($div);
                            }
                        }
                    });
                }
            });
        }

        $element.find('div[typemessage]').removeAttr('style');
        updateAudio($element.find('div[typemessage]'));
        // LoadImages_QtiItemDetail From Views/ShowQtiItem/Index.cshtml
        LoadImages_QtiItemDetail($element);

        tooltipContent = $element.html();
        tooltipContent = tooltipContent.replace(/<videolinkit /g, '<video ')
                                        .replace(/<\/videolinkit>/g, '</video>')
                                        .replace(/<sourcelinkit /g, '<source ')
                                        .replace(/<\/sourcelinkit>/g, '</source>')
                                        .replace(/autoplay=\"\"/g, '')
                                        .replace(/autoplay=\"autoplay\"/g, '');

        $tooltip.tooltipster({
            theme: 'tooltipster-default tooltipster-gr',
            interactive: true,
            contentAsHTML: true,
            maxWidth: 400,
            delay: 0,
            debug: false,
            multiple: false,
            content: tooltipContent,
            functionReady: function(origin, tooltip) {
                var $playerPlay = $body.find('.Player-audio .Player-audio-play');

                $playerPlay.on('click', function(e) {
                    var $target = $(e.target);
                    playOrPauseAudio($target);
                });
            },
            functionAfter: function() {
                if (window.playsound !== undefined) {
                    window.playsound.pause();
                }
            }
        });

        $tooltip.tooltipster('show');
    });

    /**
     * Update Audio For Element
     * @param  {[type]} el [description]
     * @return {[type]}    [description]
     */
    function updateAudio(el) {
        var $el = $(el);

        // Show audio in guidance/rationale popup
        $el.each(function(index, element) {
            var $element = $(element);
            var elementAudio = $element.attr('audioRef');
            var audioHtml = '';

            if (elementAudio !== undefined && elementAudio !== '') {
                audioHtml += '<div class="Player-audio">';
                audioHtml += '<div class="Player-audio-play" data-src="' + elementAudio + '"></div>';
                audioHtml += '</div>';

                $element.addClass('guidance-audio').prepend(audioHtml);
            }
        });
    }

    /**
     * Play or Pause Audio
     * @param  {[type]} el [description]
     * @return {[type]}    [description]
     */
    function playOrPauseAudio(el) {
        var $el = $(el);

        if ($el.hasClass('is-playing')) {
            $el.removeClass('is-playing');
            if (window.playsound !== undefined) {
                window.playsound.pause();
            }
        } else {
            var audioUrl = $el.data('src');
            var newAudioUrl = '/' + audioUrl.replace(/\//g, '|').substring(1);
            var audioSrc = '';

            if (window.playsound !== undefined) {
                window.playsound.pause();
            }

            // Direct link from S3
            if (audioUrl.indexOf('http') >= 0) {
                audioSrc = audioUrl;
            } else {
                audioSrc = configUrl.loadAudioUrl + newAudioUrl;
            }

            $el.addClass('is-playing');

            window.playsound = new vnsAudio({
                src: audioSrc,
                onEnded: function () {
                    $el.removeClass('is-playing');
                }
            });
        }
    }

    /**
     * VNS Audio
     * @param  {[type]} source [description]
     * @return {[type]}        [description]
     */
    function vnsAudio(source) {
        var self = this;
        var config = source;
        var audioId = 'vnsAudio';

        self.init = function () {
            var that = this;

            if (!config.src) {
                alert('URL for Audio should be defined');
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
            that.audio = new MediaElement(audioId, {
                success: function (audio) {
                    var emptyFn = function () { };
                    audio.play();
                    // Add Listeners
                    audio.addEventListener('play', (config.onPlay || emptyFn), false);
                    audio.addEventListener('pause', (config.onPause || emptyFn), false);
                    audio.addEventListener('ended', (config.onEnded || emptyFn), false);
                }
            });
        };

        self.play = function () {
            if (this.audio == undefined) {
                this.init();
            }

            this.audio.setCurrentTime(0);
            this.audio.play();
        };

        self.pause = function () {
            if (this.audio == undefined) {
                return;
            }

            this.audio.pause();
        };

        self.init();
    };

    /**
     * Check exist content guidance/rationale
     * @param  {[type]} ratio [description]
     * @return {[type]}       [description]
     */
    function existContentGuidanceRationale(ratio) {
        var emptyGuidanceRationale = false;
        var $ratio = $(ratio);

        if ($ratio.attr('audioref') !== undefined &&
            $ratio.attr('audioref') !== '') {
            emptyGuidanceRationale = true;
            return emptyGuidanceRationale;
        }

        if($ratio.find('img, video').length > 0) {
            emptyGuidanceRationale = true;
            return emptyGuidanceRationale;
        } else if ($.trim($ratio.text()) !== '') {
            emptyGuidanceRationale = true;
            return emptyGuidanceRationale;
        }

        return emptyGuidanceRationale;
    };

}(jQuery));
