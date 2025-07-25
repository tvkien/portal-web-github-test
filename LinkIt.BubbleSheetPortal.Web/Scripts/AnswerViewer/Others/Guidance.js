/**
 * Guidance Module
 * @param  {[type]} function (             [description]
 * @return {[type]}          [description]
 */
var GuidanceRationale = (function () {

    /**
     * Display icon guidance/rationale multiple choice
     * @param  {[type]} xmlContent [description]
     * @return {[type]}            [description]
     */
    function displayIconGrMultipleChoice (xmlContent) {
        var $tree = $('<div/>');

        $tree.append(xmlContent);

        if ($tree.find('.multiplechoice').length) {
            $tree.find('.multiplechoice').each(function (ind, multiplechoice) {
                var $multiplechoice = $(multiplechoice);

                $multiplechoice.find('.multiplechoice-item').each(function(ind, simplechoice) {
                    var $simplechoice = $(simplechoice);
                    var $simplechoiceAnswer = $simplechoice.find('div[stylename="answer"]');

                    $simplechoice.find('div[typemessage]').each(function(i, type) {
                        var $type = $(type);

                        if (existContentGuidanceRationale($type)) {
                            var typemessage = $type.attr('typemessage');

                            if (typemessage === 'guidance') {
                                $simplechoice.append('<span class="icon-guidance icon-tooltip" data-type="guidance"></span>');
                            } else if (typemessage === 'rationale') {
                                $simplechoice.append('<span class="icon-rationale icon-tooltip" data-type="rationale"></span>');
                            } else {
                                $simplechoice.append('<span class="icon-guidance icon-tooltip" data-type="guidance_rationale"></span>');
                                $simplechoice.append('<span class="icon-rationale icon-tooltip" data-type="guidance_rationale"></span>');
                            }
                        }
                    });
                });
            })
        }

        return $tree.html();
    }

    /**
     * Display icon guidance/rationale inline choice
     * @param  {[type]} xmlContent [description]
     * @return {[type]}            [description]
     */
    function displayIconGrInlineChoice (xmlContent) {
        var $tree = $('<div/>');

        $tree.append(xmlContent);

        if ($tree.find('.inlinechoice').length) {
            $tree.find('.inlinechoice').each(function(index, inlinechoiceinteraction) {
                var $inlinechoiceinteraction = $(inlinechoiceinteraction);

                $inlinechoiceinteraction.find('.inlinechoice-item').each(function(ind, inlinechoice) {
                    var $inlinechoice = $(inlinechoice);
                    var $inlinechoiceAnswer = $inlinechoice.find('.inlineChoiceAnswer');

                    $inlinechoice.find('div[typemessage]').each(function(i, type) {
                        var $type = $(type);
                        var typemessage = $type.attr('typemessage');

                        if (existContentGuidanceRationale($type)) {
                            if (typemessage === 'guidance') {
                                $inlinechoice.append('<span class="icon-guidance icon-tooltip" data-type="guidance"></span>');
                            } else if (typemessage === 'rationale') {
                                $inlinechoice.append('<span class="icon-rationale icon-tooltip" data-type="rationale"></span>');
                            } else {
                                $inlinechoice.append('<span class="icon-guidance icon-tooltip" data-type="guidance_rationale"></span>');
                                $inlinechoice.append('<span class="icon-rationale icon-tooltip" data-type="guidance_rationale"></span>');
                            }
                        }
                    });
                });
            });
        }

        return $tree.html();
    };

    /**
     * Display Display icon guidance/rationale text entry
     * @param  {[type]} xmlContent [description]
     * @return {[type]}            [description]
     */
    function displayIconGrTextEntry (xmlContent) {
        var $tree = $('<div/>');

        $tree.append(xmlContent);

        if ($tree.find('.textentry').length) {
            $tree.find('.textentry').each(function(index, textentry) {
                var $textentry = $(textentry);
                var textentryResponseId = $textentry.attr('responseidentifier');

                $tree.find('assessmentitem responsedeclaration').each(function(ind, response) {
                    var $response = $(response);
                    var responseId = $response.attr('identifier');

                    if (responseId === textentryResponseId) {
                        var $responserubric = $response.find('responserubric');
                        var responserubricGuidance = $responserubric.find('value[type="guidance"]').length;
                        var responserubricRationale = $responserubric.find('value[type="rationale"]').length;
                        var responserubricGR = $responserubric.find('value[type="guidance_rationale"]').length;
                        var guidanceLength = 0;
                        var rationaleLength = 0;

                        // Adding icon tooltip guidance/rationale
                        if ((responserubricGuidance > 0 || responserubricGR > 0) &&
                            !$textentry.siblings('.Icon-tooltip[data-type="guidance"]').length) {
                            $textentry.after('<span class="icon-guidance icon-tooltip" data-type="guidance" data-id="' + responseId + '"></span>');
                        }

                        if ((responserubricRationale > 0 || responserubricGR > 0) &&
                            !$textentry.siblings('.Icon-tooltip[data-type="rationale"]').length) {
                            $textentry.after('<span class="icon-rationale icon-tooltip" data-type="rationale" data-id="' + responseId + '"></span>');
                        }

                        // Remove icon if guidance/rationale is empty
                        $responserubric.find('value').each(function(vi, value) {
                            var $value = $(value);
                            var valueType = $value.attr('type');
                            var valueExist = existContentGuidanceRationale($value);

                            if (!valueExist && valueType === 'guidance') {
                                guidanceLength += 1;
                            }

                            if (!valueExist && valueType === 'rationale') {
                                rationaleLength += 1;
                            }

                            if (!valueExist && valueType === 'guidance_rationale') {
                                guidanceLength += 1;
                                rationaleLength += 1;
                            }
                        });

                        // Remove icon if content guidance/rationale is empty
                        if (guidanceLength === (responserubricGuidance + responserubricGR)) {
                            $textentry.siblings('.icon-tooltip[data-type="guidance"]').remove();
                        }

                        if (rationaleLength === (responserubricRationale + responserubricGR)) {
                            $textentry.siblings('.icon-tooltip[data-type="rationale"]').remove();
                        }
                    }
                });
            });
        }

        return $tree.html();
    };

    /**
     * Show guidance/rationale tooltip
     * @return {[type]} [description]
     */
    function showGuidanceRationale () {
        var $tooltip = $(this);
        var tooltipData = $tooltip.data('type');
        var tooltipContent = '';
        var $element = $('<div/>');

        // Multiple choice
        if ($tooltip.parents('.multiplechoice').length) {
            var $parent = $tooltip.parents('.multiplechoice-item');

            $element.html($parent.find('div[typemessage="' + tooltipData + '"]').clone(true));
        }

        // Inline choice
        if ($tooltip.parents('.inlinechoice').length) {
            var $parent = $tooltip.parents('.inlinechoice-item');

            $element.html($parent.find('div[typemessage="' + tooltipData + '"]').clone(true));
        }

        // Text entry
        if (!!$tooltip.data('id')) {
            var dataId = $tooltip.data('id');
            var dataType = $tooltip.data('type');

            $(document).find('assessmentitem responsedeclaration').each(function(ind, response) {
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

                                Utils.copyAttributes($value, $divTypeMessage);

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
                var $playerPlay = $(document).find('.player-audio .player-audio-play');

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
    }

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
            var elementAudio = $element.attr('audioref');
            var audioHtml = '';

            if (!!elementAudio) {
                audioHtml += '<div class="player-audio">';
                audioHtml += '<div class="player-audio-play" data-src="' + elementAudio + '"></div>';
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
            var audioSrc = '';

            if (window.playsound !== undefined) {
                window.playsound.pause();
            }

            // Direct link from S3
            if (audioUrl.indexOf('http') >= 0) {
                audioSrc = audioUrl;
            } else {
                s3Config = Utils.rightSlash(s3Config);
                audioUrl = Utils.leftSlash(audioUrl);
                audioSrc = s3Config + audioUrl;
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
     * Check empty guidance/rationale
     * @param  {[type]} ratio [description]
     * @return {[type]}       [description]
     */
    function existContentGuidanceRationale (ratio) {
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
    }

    /**
     * Get guidance and rationale
     * @param  {[type]} xmlContent [description]
     * @return {[type]}            [description]
     */
    function getListGuidanceRationale (xmlContent) {
        var $tree = $('<div/>');
        var obj = {
            guidance: false,
            rationale: false
        };

        $tree.append(xmlContent);

        if ($tree.find('.icon-guidance').length) {
            obj.guidance = true;
        }

        if ($tree.find('.icon-rationale').length) {
            obj.rationale = true;
        }

        return obj;
    }

    /**
     * Handle click guidance
     * @return {[type]} [description]
     */
    function handleClickGuidance () {
        var $self = $(this);
        var $guidance = $('.icon-guidance');

        if ($self.is(':checked')) {
            $guidance.css('display', 'inline-block');

            if ($guidance.parents('.multiplechoice-item').length) {
                $guidance.parents('.multiplechoice-item').addClass('is-guidance');
            }

            if ($guidance.parents('.inlinechoice-item').length) {
                $guidance.parents('.inlinechoice-item').addClass('is-guidance');
            }
        } else {
            $guidance.css('display', 'none');
            $guidance.parents('.multiplechoice-item').removeClass('is-guidance');
            $guidance.parents('.inlinechoice-item').removeClass('is-guidance');
        }
    }

    /**
     * Handle click rationale
     * @return {[type]} [description]
     */
    function handleClickRationale () {
        var $self = $(this);
        var $rationale = $('.icon-rationale');

        if ($self.is(':checked')) {
            $rationale.css('display', 'inline-block');

            if ($rationale.parents('.multiplechoice-item').length) {
                $rationale.parents('.multiplechoice-item').addClass('is-rationale');
            }

            if ($rationale.parents('.inlinechoice-item').length) {
                $rationale.parents('.inlinechoice-item').addClass('is-rationale');
            }
        } else {
            $rationale.css('display', 'none');
            $rationale.parents('.multiplechoice-item').removeClass('is-rationale');
            $rationale.parents('.inlinechoice-item').removeClass('is-rationale');
        }
    }

    return {
        displayIconGrMultipleChoice: displayIconGrMultipleChoice,
        displayIconGrInlineChoice: displayIconGrInlineChoice,
        displayIconGrTextEntry: displayIconGrTextEntry,
        getListGuidanceRationale: getListGuidanceRationale,
        showGuidanceRationale: showGuidanceRationale,
        handleClickGuidance: handleClickGuidance,
        handleClickRationale: handleClickRationale
    }
})();
