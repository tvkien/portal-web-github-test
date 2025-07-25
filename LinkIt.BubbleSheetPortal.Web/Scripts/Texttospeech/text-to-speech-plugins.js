(function ($, window) {
    'use strict';

    /**
     * Extend prototype
     * @param  {[type]} obj    [description]
     * @param  {[type]} source [description]
     * @return {[type]}        [description]
     */
    function defaults(obj, source) {
        obj = obj || {};

        for (var prop in source) {
            if (source.hasOwnProperty(prop) && obj[prop] === void 0) {
                obj[prop] = source[prop];
            }
        }

        return obj;
    }

    /**
     * Create new element new properties and attribute
     * @param  {[type]} tagName     [description]
     * @param  {[type]} properties  [description]
     * @param  {[type]} attributes  [description]
     * @param  {[type]} textContent [description]
     * @return {[type]}             [description]
     */
    function createEl(tagName, properties, attributes, textContent) {
        var el = document.createElement(tagName);
        var isText = Boolean(textContent);

        Object.getOwnPropertyNames(properties).forEach(function (propName) {
            var val = properties[propName];

            if (propName.indexOf('aria-') !== -1 || propName !== 'role' || propName !== 'type') {
                el.setAttribute(propName, val);
            } else {
                el[propName] = val;
            }
        });

        Object.getOwnPropertyNames(attributes).forEach(function (attrName) {
            el.setAttribute(attrName, attributes[attrName]);
        });

        if (isText) {
            el.innerHTML = textContent;
        }

        return el;
    }

    /**
     * Check value is null or empty
     * @param  {[type]}  str [description]
     * @return {Boolean}     [description]
     */
    function isNullOrEmpty(str) {
        return (typeof str === 'undefined' || str === null || str.trim() === '');
    }

    /**
     * Regex check className
     * @param  {[type]} className [description]
     * @return {[type]}           [description]
     */
    function classReg(className) {
        return new RegExp('(^|\\s+)' + className + '(\\s+|$)');
    }

    /**
     * Dom element utils
     * @param  {[type]} el [description]
     * @return {[type]}    [description]
     */
    var dom = function (el) {
        return {
            addClass: function (c) {
                if (el.classList) {
                    el.classList.add(c);
                } else {
                    el.className += ' ' + c;
                }
            },
            removeClass: function (c) {
                if (el.classList) {
                    el.classList.remove(c);
                } else {
                    el.className = el.className.replace(
                        new RegExp('(^|\\b)' + c + '(\\b|$)', 'gi'), ' '
                    );
                }
            },
            hasClass: function (c) {
                if (el.classList) {
                    return el.classList.contains(c);
                }
                return classReg(c).test(el.className);
            }
        };
    };

    /**
     * Text to speech plugins
     * @param {[type]} element [description]
     * @param {[type]} options [description]
     */
    function TextToSpeech(element, options) {
        if (!element) {
            throw new Error('Missing anchor element');
        }

        this.setIntervalPlay = '';
        this.el = element;
        this.streaming = 0;
        this.streamingText = '';
        this.streamingInterval = '';
        this.speechArray = [];
        this.speechIndex = 0;
        this.options = defaults(options, {
            xmlContent: '',
            qtiSchemeId: '',
            voice: 'US English Female',
            options: {
                rate: 0.8,
                volume: 0.5
            },
            showText: true,
            textContent: 'Text-to-Speech',
            textStreaming: 'Audio streaming has been stopped. You need to replay to listen text to speech. ',
            usingTemplate: true,
            pauseSpeed: 500,
            speechType: 'other' // Type of speech(question|section|passage|intruction)
        });

        this.init();
    }

    TextToSpeech.prototype = {
        init: function () {
            this.bindElements();
            this.bindEvents();
        },
        reset: function () {
            this.handleStop();
            this.init();
        },
        bindElements: function () {
            // Initialize text to speech container
            var tssContainer = createEl('div', {
                    class: 'texttospeech-container'
                }, {
                    role: 'group'
                }
            );

            // Initialize play and pause toogle control
            this.btnPlay = createEl('button', {
                    class: 'texttospeech-control texttospeech-control-play'
                }, {
                    'role': 'button',
                    'title': 'Play',
                    'aria-live': 'polite',
                    'aria-label': 'Play'
                }, 'Play'
            );

            // Initialize pause control bar
            this.btnPause = createEl('button', {
                    class: 'texttospeech-control texttospeech-control-pause texttospeech-hide'
                }, {
                    'role': 'button',
                    'title': 'Pause',
                    'aria-live': 'polite',
                    'aria-label': 'Pause'
                }, 'Pause'
            );

            // Initialize stop control bar
            this.btnStop = createEl('button', {
                    class: 'texttospeech-control texttospeech-control-stop texttospeech-hide'
                }, {
                    'role': 'button',
                    'title': 'Stop',
                    'aria-live': 'polite',
                    'aria-label': 'Stop'
                }, 'Stop'
            );

            // Initialize resume control bar
            this.btnResume = createEl('button', {
                    class: 'texttospeech-control texttospeech-control-resume texttospeech-hide'
                }, {
                    'role': 'button',
                    'title': 'Resume',
                    'aria-live': 'polite',
                    'aria-label': 'Resume'
                }, 'Resume'
            );

            // Initialize text control bar
            var btnText = createEl('div', {
                    class: 'texttospeech-text'
                }, {
                    role: 'button'
                }, this.options.textContent
            );

            // Append to element
            tssContainer.appendChild(this.btnPlay);
            tssContainer.appendChild(this.btnPause);
            tssContainer.appendChild(this.btnResume);
            tssContainer.appendChild(this.btnStop);

            if (this.options.showText) {
                tssContainer.appendChild(btnText);
            }

            this.el.innerHTML = '';
            this.el.appendChild(tssContainer);

            // Change properties
            this.reset = this.reset;
            this.setXmlContent = this.setXmlContent;
            this.setVoice = this.setVoice;
            this.setRate = this.setRate;
            this.setVolume = this.setVolume;
        },
        bindEvents: function () {
            // Register event for text to speech control
            this.btnPlay.addEventListener('click', this.handlePlay.bind(this));
            this.btnPause.addEventListener('click', this.handlePause.bind(this));
            this.btnResume.addEventListener('click', this.handleResume.bind(this));
            this.btnStop.addEventListener('click', this.handleStop.bind(this));
        },
        handlePlay: function () {
            var that = this;

            // Pause all other audio
            var audios = document.querySelectorAll('audio');

            if (audios.length) {
                for (var i = 0, len = audios.length; i < len; i++) {
                    audios[i].pause();
                    audios[i].currentTime = 0;
                }
            }

            // Read from begin when stop
            responsiveVoice.cancel();
            that.speechIndex = 0;
            delete window.currentSpeech;

            that.handleStopOtherControlPlaying();
            that.handleControlPlaying();

            var isReponsiveVoice = Boolean(responsiveVoice);
            if (isReponsiveVoice) {
                var ttsContent = that.getTexttospeech(this.options.xmlContent);

                ttsContent = this.streamingText + ttsContent;

                that.speechArray = that.replaceMark(ttsContent);

                // This variable only use when reading text to speech and it will be release after read
                window.currentSpeech = that;

                that.speechParagraph();
            }
        },
        speechParagraph: function () {
            if (window.currentSpeech === undefined) {
                return;
            }

            var that = window.currentSpeech;
            // Since the function has been called, it means that a paragraph has been finished,
            // so we can go to the next one by adding to the index.
            that.speechIndex += 1;

            if (that.speechIndex <= that.speechArray.length) {
                var timeOut = setTimeout(function () {
                    clearTimeout(timeOut);
                    responsiveVoice.cancel();

                    responsiveVoice.speak(
                        that.speechArray[that.speechIndex - 1],
                        that.options.voice,
                        {
                            rate: that.options.options.rate,
                            volume: that.options.options.volume,
                            onend: that.speechParagraph
                        }
                    );
                }, that.options.pauseSpeed);
            } else {
                // When there's nothing more to read, we start from the beginning.
                that.speechIndex = 0;

                that.streamingText = '';

                that.setIntervalPlay = setInterval(function () {
                    if (!responsiveVoice.isPlaying()) {
                        clearInterval(that.setIntervalPlay);

                        if (!dom(that.el).hasClass('visuallyhidden')) {
                            that.handleControlInit(that.el);
                        }
                        that.streaming = 0;
                        that.streamingText = '';
                    }
                }, 1000);

                // Release resource after read complete
                delete window.currentSpeech;
            }
        },
        handlePause: function () {
            var isReponsiveVoice = Boolean(responsiveVoice);
            if (isReponsiveVoice) {
                responsiveVoice.pause();
                this.handleControlPause();
            }
        },
        handleStop: function () {
            var isReponsiveVoice = Boolean(responsiveVoice);
            if (isReponsiveVoice) {
                responsiveVoice.cancel();
                this.handleControlInit(this.el);
                clearInterval(this.setIntervalPlay);
                // Read from begin when stop
                this.speechIndex = 0;
                delete window.currentSpeech;
            }
        },
        handleResume: function () {
            var isReponsiveVoice = Boolean(responsiveVoice);
            if (isReponsiveVoice) {
                if (this.streaming >= 10) {
                    // Display popup text  to speech after pause 10 seconds
                    this.streaming = 0;
                    this.streamingText = this.options.textStreaming;
                    this.handlePlay();
                } else {
                    responsiveVoice.resume();
                    this.handleControlResume();
                }
            }
        },
        handleOptionShowOrHide: function () {
            var classHide = 'texttospeech-hide';

            if (dom(this.tssContainerOptions).hasClass(classHide)) {
                dom(this.tssContainerOptions).removeClass(classHide);
                this.btnOptions.setAttribute('title', 'Hide Options');
                this.btnOptions.setAttribute('aria-label', 'Hide Options');
            } else {
                dom(this.tssContainerOptions).addClass(classHide);
                this.btnOptions.setAttribute('title', 'Show Options');
                this.btnOptions.setAttribute('aria-label', 'Show Options');
            }
        },
        handleVoice: function (e) {
            this.setVoice(e.target.value);
        },
        handleRate: function (e) {
            this.setRate(e.target.value);
        },
        handleVolume: function (e) {
            this.setVolume(e.target.value);
        },
        handleControlInit: function (el) {
            var classHide = 'texttospeech-hide';

            $(el).find(this.btnPlay).removeClass(classHide);
            $(el).find(this.btnPause).addClass(classHide);
            $(el).find(this.btnStop).addClass(classHide);
            $(el).find(this.btnResume).addClass(classHide);
        },
        handleControlPlaying: function () {
            var classHide = 'texttospeech-hide';

            dom(this.btnPlay).addClass(classHide);
            dom(this.btnPause).removeClass(classHide);
            dom(this.btnStop).removeClass(classHide);
        },
        handleStopOtherControlPlaying: function () {
            var classHide = 'texttospeech-hide';
            var texttospeechPlugin = document.querySelectorAll('.texttospeech-plugin');
            var texttospeechPluginLength = texttospeechPlugin.length;

            for (var i = 0; i < texttospeechPluginLength; i++) {
                var texttospeechControl = texttospeechPlugin[i].getElementsByClassName('texttospeech-control');
                var texttospeechControlLength = texttospeechControl.length;

                if (texttospeechControlLength) {
                    var texttospeechControlPlay = texttospeechPlugin[i].querySelector('.texttospeech-control-play');

                    for (var j = 0; j < texttospeechControlLength; j++) {
                        dom(texttospeechControl[j]).addClass(classHide);
                    }

                    dom(texttospeechControlPlay).removeClass(classHide);
                }
            }
        },
        handleControlPause: function () {
            var that = this;
            var classHide = 'texttospeech-hide';
            this.streaming = 0;

            dom(this.btnPause).addClass(classHide);
            dom(this.btnResume).removeClass(classHide);
            this.streamingInterval = setInterval(function () {
                that.streaming++;
                if (that.streaming === 10) {
                    clearInterval(that.streamingInterval);
                }
            }, 1000);
        },
        handleControlResume: function () {
            var classHide = 'texttospeech-hide';
            dom(this.btnPause).removeClass(classHide);
            dom(this.btnResume).addClass(classHide);
        },
        setXmlContent: function (xmlContent) {
            this.options.xmlContent = xmlContent;
        },
        setVoice: function (voice) {
            this.options.voice = voice;
        },
        setRate: function (rate) {
            this.options.options.rate = rate;
        },
        setVolume: function (volume) {
            this.options.options.volume = volume;
        },
        getTexttospeech: function (xmlContent) {
            var ttsContent = '';
            var $element = $('<div/>');

            $element.append(xmlContent);
            $element.find('responsedeclaration, outcomedeclaration, stylesheet').remove();

            $element = this.getSpeakMultipleChoice($element);
            $element = this.getSpeakInlineChoice($element);
            $element = this.getSpeakTextEntry($element);
            $element = this.getSpeakDrawingResponse($element);
            $element = this.getSpeakExtendedText($element);
            $element = this.getSpeakDragDropStandard($element);
            $element = this.getSpeakTextHotSpot($element);
            $element = this.getSpeakImageHotSpot($element);
            $element = this.getSpeakTableHotSpot($element);
            $element = this.getSpeakNumberLineHotSpot($element);
            $element = this.getSpeakDragDropSequenceOrder($element);
            $element = this.getSpeakMultiPart($element);
            $element = this.getSpeakImage($element);
            $element = this.getSpeakMathjax($element);
            $element = this.getSpeakVideo($element);

            ttsContent = $element.text();
            return ttsContent;
        },
        getSpeakMultipleChoice: function (el) {
            var that = this;
            var $multiplechoice = el.find('choiceInteraction');

            if ($multiplechoice.length) {
                $multiplechoice.each(function (ind, mc) {
                    var $mc = $(mc);
                    var mcType = parseInt($mc.attr('maxchoices'), 10);
                    var tssMultipleChoice = '{{pause}} Please select the correct answer.';

                    if (mcType !== 1 && $mc.attr('variablepoints') !== 'true') {
                        tssMultipleChoice = '{{pause}} Please select the correct answer.{{pause}} Note: You can select more than one answer.';
                    }

                    $mc.find('simpleChoice').each(function (index, simplechoice) {
                        var $simplechoice = $(simplechoice);
                        var ttsSimplechoice = '';
                        var idx = $simplechoice.attr('identifier');

                        if (typeof STATIC !== 'undefined' &&
                            typeof STATIC.SETTINGS !== 'undefined' &&
                            typeof STATIC.SETTINGS.answerLabelFormat !== 'undefined' &&
                            STATIC.SETTINGS.answerLabelFormat === '1') {
                            idx = index + 1;
                        }

                        $simplechoice = that.getSpeakImage($simplechoice);
                        $simplechoice = that.getSpeakMathjax($simplechoice);

                        ttsSimplechoice = $simplechoice.find('.answer').text();

                        if (index === $mc.find('simpleChoice').length - 1) {
                            ttsSimplechoice += '{{pause}}';
                        }

                        tssMultipleChoice += [
                            '{{pause}} ' + idx + '{{pause}}',
                            ttsSimplechoice
                        ].join(' ');
                    });

                    $mc.replaceWith(tssMultipleChoice);
                });

                that.getSpeakUsingTemplate(el, $multiplechoice);
            }

            return el;
        },
        getSpeakInlineChoice: function (el) {
            var that = this;
            var $inlinechoiceinteraction = el.find('inlineChoiceInteraction');

            if ($inlinechoiceinteraction.length) {
                $inlinechoiceinteraction.each(function (ind, inlincechoice) {
                    var $inlincechoice = $(inlincechoice);
                    var tssInlineChoice = "{{pause}}Please select the correct answer from the dropdown menu.";

                    $inlincechoice.find('inlineChoice').each(function (index, ic) {
                        var $ic = $(ic);
                        var ttsIc = $ic.text();

                        if (index === $inlincechoice.find('inlineChoice').length - 1) {
                            ttsIc += '{{pause}}';
                        }

                        tssInlineChoice += [
                            '{{pause}}Option: ' + (index + 1) + '{{pause}}',
                            ttsIc
                        ].join(' ');
                    });

                    $inlincechoice.replaceWith(tssInlineChoice + "{{pause}}");
                });

                that.getSpeakUsingTemplate(el, $inlinechoiceinteraction);
            }

            return el;
        },
        getSpeakTextEntry: function (el) {
            var that = this;
            var $textentry = el.find('textEntryInteraction');

            if ($textentry.length) {
                $textentry.replaceWith("{{pause}}Input Box{{pause}}");

                that.getSpeakUsingTemplate(el, $textentry);
            }

            return el;
        },
        getSpeakExtendedText: function (el) {
            var that = this;
            var $extendedtext = el.find('extendedTextInteraction');

            if ($extendedtext.length) {
                $extendedtext.replaceWith("{{pause}}Input Box{{pause}}");

                that.getSpeakUsingTemplate(el, $extendedtext);
            }

            return el;
        },
        getSpeakDrawingResponse: function (el) {
            var that = this;
            var $extendedtextdraw = el.find('extendedTextInteraction[drawable="true"]');

            if ($extendedtextdraw.length) {
                var imgDec = $extendedtextdraw.find('img').attr('texttospeech');
                imgDec = isNullOrEmpty(imgDec) ? '' : ('{{pause}}Description:{{pause}}' + imgDec);
                $extendedtextdraw.replaceWith("{{pause}}Drawing Box" + imgDec);
                that.getSpeakUsingTemplate(el, $extendedtextdraw);
            }

            return el;
        },
        getSpeakDragDropStandard: function (el) {
            var that = this;
            var $dragdropstandard = el.find('partialCredit');

            if ($dragdropstandard.length) {
                var $qtiItemView = $('#qtiItemView');

                el.find('sourceobject').each(function (ind, sourceobject) {
                    var $sourceobject = $(sourceobject);
                    var ttsSourceobject = '';

                    ttsSourceobject = [
                        '{{pause}} Draggable {{pause}}',
                        that.getSpeakObjectDragDrop($sourceobject, 'sourceobject'),
                        '{{pause}} End Draggable {{pause}}'
                    ].join(' ');

                    $sourceobject.replaceWith(ttsSourceobject);
                });

                el.find('destinationobject').each(function (ind, destination) {
                    var $destination = $(destination);
                    var destinationIdentifier = $destination.find('destinationitem').attr('destidentifier');
                    var $destinationSource = $qtiItemView.find('.destinationItem[id="' + destinationIdentifier + '"] .sourceObject');
                    var ttsDestination = '';

                    if ($destinationSource.length) {
                        var destinationAnswer = [];
                        $destinationSource.each(function () {
                            destinationAnswer.push(that.getSpeakObjectDragDrop($(this), 'sourceobject') + ' {{pause}}');
                        });

                        ttsDestination = [
                            '{{pause}} Destination {{pause}}',
                            destinationAnswer.join(' with '),
                            'Dragged In',
                            '{{pause}} End Destination {{pause}}'
                        ].join(' ');
                    } else {
                        ttsDestination = [
                            '{{pause}} Destination {{pause}}',
                            that.getSpeakObjectDragDrop($destination, 'destination'),
                            '{{pause}} End Destination {{pause}}'
                        ].join(' ');
                    }

                    $destination.replaceWith(ttsDestination);
                });

                that.getSpeakUsingTemplate(el, $dragdropstandard);
            }

            return el;
        },
        getSpeakTextHotSpot: function (el) {
            var that = this;
            var $texthotspot = el.find('textHotSpot');

            if ($texthotspot.length) {
                // Get all selected hotspot
                $(this.el).parents("body").find("sourcetext.active").each(function () {
                    var hpId = $(this).attr("identifier");
                    var currentEl = el.find("sourcetext[identifier=" + hpId + "]");
                    var newSelected = currentEl.prop("outerHTML") + "{{pause}} Selected {{pause}}";

                    currentEl.replaceWith(newSelected);
                });

                that.getSpeakUsingTemplate(el, $texthotspot);
            }

            return el;
        },
        getSpeakImageHotSpot: function (el) {
            var that = this;
            var $imagehotspot = el.find('imageHotSpot');

            if ($imagehotspot.length) {
                $imagehotspot.each(function (ind, ihs) {
                    var $ihs = $(ihs);
                    var ttsImageHotspot = '';
                    var ttsIhsDesc = $ihs.attr('texttospeech');

                    ttsIhsDesc = isNullOrEmpty(ttsIhsDesc) ? ' ' : (ttsIhsDesc + '{{pause}}');

                    ttsImageHotspot = [
                        '{{pause}} Clickable Image {{pause}}',
                        ttsIhsDesc,
                        '{{pause}} End Clickable Image {{pause}}'
                    ].join(' ');

                    $ihs.replaceWith(ttsImageHotspot);
                });

                that.getSpeakUsingTemplate(el, $imagehotspot);
            }

            return el;
        },
        getSpeakTableHotSpot: function (el) {
            var that = this;
            var $tablehotspot = el.find('table.tableHotspotInteraction');

            if ($tablehotspot.length) {
                $tablehotspot.each(function (ind, hotspottable) {
                    // Build col and row
                    var hotspotcell = "Row <row_number> {{pause}} Column <col_number> {{pause}} <cell_content> {{pause}}";
                    $(hotspottable).find("td").each(function () {
                        var col = $(this).parent().children().index($(this)) + 1;
                        var row = $(this).parent().parent().children().index($(this).parent()) + 1;

                        var tablehotspotid = $(this).find("tableitem").attr("identifier");
                        var ttscell = hotspotcell.replace("<row_number>", row).replace("<col_number>", col);

                        if ($(that.el).parents("#qtiItemView").find("#" + tablehotspotid).is(":checked")) {
                            $(this).find("tableitem").replaceWith("Hot Spot {{pause}} Selected {{pause}}");
                        } else {
                            $(this).find("tableitem").replaceWith("Hot Spot {{pause}}");
                        }

                        // Remove table hotspot item
                        $(this).find("tableitem").replaceWith("");

                        var cellcontent = $(this).html();
                        ttscell = ttscell.replace("<cell_content>", cellcontent);

                        $(this).empty().append(ttscell);
                    });
                });

                that.getSpeakUsingTemplate(el, $tablehotspot);
            }

            return el;
        },
        getSpeakNumberLineHotSpot: function (el) {
            var that = this;
            var $numberlinehotspot = el.find('numberLine');

            if ($numberlinehotspot.length) {
                var $qtiItemView = $('#qtiItemView');
                var ttsNumberline = '';
                var ttsNumberlineChecked = '';
                var numberlinehotspotChecked = [];

                if ($qtiItemView.find('numberLine .numberline-hotspot.checked').length) {
                    $qtiItemView.find('numberLine .numberline-hotspot.checked').each(function () {
                        var ind = $(this).index();
                        numberlinehotspotChecked.push($numberlinehotspot.find('text').eq(ind - 1).text());
                    });
                }

                $numberlinehotspot.find('text').each(function () {
                    var text = this.textContent;

                    if (numberlinehotspotChecked.indexOf(text) > -1) {
                        ttsNumberlineChecked += '{{pause}} ' + this.textContent + '{{pause}} Selected {{pause}}';
                    } else {
                        ttsNumberlineChecked += '{{pause}} ' + this.textContent;
                    }
                });

                ttsNumberline = [
                    '{{pause}} Number Line Values',
                    ttsNumberlineChecked,
                    '{{pause}} End Number Line'
                ].join(' ');

                $numberlinehotspot.replaceWith(ttsNumberline);

                that.getSpeakUsingTemplate(el, $numberlinehotspot);
            }

            return el;
        },
        getSpeakDragDropSequenceOrder: function (el) {
            var that = this;
            var $dragdropsequence = el.find('partialSequence');
            var $currentDragdropsequence = $(this.el).parents('body');

            if ($dragdropsequence.length) {
                $dragdropsequence.each(function (ind, sequence) {
                    var $sequence = $(sequence);
                    var ttsSequence = '{{pause}} Current Order';

                    if ($currentDragdropsequence.hasClass('testtaker')) {
                        $currentDragdropsequence.find('.sortSequence li').each(function (index, sc) {
                            var scIdentifier = $(sc).attr('identifier');

                            ttsSequence += [
                                '{{pause}} Draggable {{pause}} ',
                                $dragdropsequence.find('sourceitem[identifier="' + scIdentifier + '"] div[styleName="value"]').text(),
                                ' {{pause}} End Draggable'
                            ].join(' ');
                        });
                    } else {
                        $sequence.find('sourceitem').each(function (index, sourceitem) {
                            var $sourceitem = $(sourceitem);

                            ttsSequence += [
                                '{{pause}} Draggable {{pause}} ',
                                $sourceitem.find('div[styleName="value"]').text(),
                                ' {{pause}} End Draggable'
                            ].join(' ');
                        });
                    }

                    $sequence.replaceWith(ttsSequence);
                });

                that.getSpeakUsingTemplate(el, $dragdropsequence);
            }

            return el;
        },
        getSpeakMultiPart: function (el) {
            var that = this;

            if (that.options.qtiSchemeId.toString() === '21') {
                that.getSpeakUsingTemplate(el);
            }

            return el;
        },
        getSpeakImage: function (el) {
            var $imgs = el.find('img');

            $imgs.each(function (ind, img) {
                var $img = $(img);
                var ttsImg = $img.attr('texttospeech');
                var isTssImg = Boolean(ttsImg);
                if (isTssImg) {
                    $img.replaceWith(" " + ttsImg + " ");
                }
            });

            return el;
        },
        getSpeakMathjax: function (el) {
            var $mathtext = el.find('.math-tex');

            $mathtext.each(function (ind, mathjax) {
                var $mathjax = $(mathjax);
                var ttsMathjax = $mathjax.attr('texttospeech');

                if (!ttsMathjax) {
                    $mathjax.replaceWith(" " + ttsMathjax + " ");
                }
            });

            return el;
        },
        getSpeakVideo: function (el) {
            var $videos = el.find('videolinkit');

            $videos.each(function (ind, video) {
                var $video = $(video);
                var ttsVideo = '{{pause}} Click to watch the video. {{pause}}';

                $video.replaceWith(ttsVideo);
            });

            return el;
        },
        getSpeakObjectDragDrop: function (el, type) {
            var $el = $(el);
            var elType = $el.attr('type');
            var elTexttospeech = '';
            var elDefault = ' Blank ';

            if (type === 'sourceobject') {
                elTexttospeech = $el.find('img').attr('texttospeech');
            } else {
                elTexttospeech = $el.attr('texttospeech');
            }

            if (elType === 'text') {
                elTexttospeech = $el.text();
            }

            elTexttospeech = isNullOrEmpty(elTexttospeech) ? elDefault : elTexttospeech;

            if (elTexttospeech.trim().indexOf('DEST_') === 0) {
                // Check default value destination
                elTexttospeech = elDefault;
            }

            return elTexttospeech;
        },
        getSpeakUsingTemplate: function (el, question) {
            if (this.options.usingTemplate) {
                var $question;
                var questionSchemeId = this.options.qtiSchemeId.toString();
                var questionHeader = '';
                var questionBody = '';
                var questionContent = el.html();

                if (question !== undefined) {
                    $question = $(question);
                }

                if (questionSchemeId === '1' || questionSchemeId === '3' || questionSchemeId === '37') {
                    if ($question.attr('subtype') === 'TrueFalse') {
                        questionHeader = '{{pause}} True or False {{pause}}';
                    } else {
                        questionHeader = '{{pause}} Multiple Choice {{pause}}';
                    }
                } else if (questionSchemeId === '8') {
                    questionHeader = '{{pause}}  Inline Choice {{pause}}';
                } else if (questionSchemeId === '9') {
                    questionHeader = '{{pause}} Fill in the Blank {{pause}} For this question, please type your response into the box labeled "Input Box".{{pause}}';
                } else if (questionSchemeId === '10' &&
                    $question.attr('drawable') === undefined &&
                    $question.attr('drawable') !== 'true') {
                    questionHeader = '{{pause}} Constructed Response {{pause}} For this question, please type your response into the box labeled "Input Box".{{pause}}';
                } else if (questionSchemeId === '10' && $question.attr('drawable') === 'true') {
                    questionHeader = '{{pause}} Drawing Response {{pause}}For this question, please use the draw tool to draw your response into the box labeled "Drawing Box".{{pause}}';
                } else if (questionSchemeId === '21') {
                    questionHeader = '{{pause}} Multiple Parts Question {{pause}} This question contains multiple sub questions for you to answer. Some questions may require that you type or draw a response. For those questions, you should type into the box labeled Input Box or draw into the box labeled Drawing Box.{{pause}}';

                    if (!!questionContent && questionContent.indexOf(questionHeader) !== -1) {
                        questionHeader = '';
                    }
                } else if (questionSchemeId === '30' || questionSchemeId === '35') {
                    questionHeader = '{{pause}} Drag and Drop {{pause}} For this question you will be presented with objects referred to as draggable objects. These draggable objects should be dragged into objects referred to as destination objects.{{pause}}';
                } else if (questionSchemeId === '31') {
                    questionHeader = '{{pause}} Text Hot Spot {{pause}}Some of the words in this question can be selected by clicking on them. Click on the word or words to answer the question.{{pause}}';
                } else if (questionSchemeId === '32') {
                    questionHeader = '{{pause}} Image Hot Spot {{pause}} This question contains an image. You can click on certain parts of that image to select your answer.{{pause}}';
                } else if (questionSchemeId === '33') {
                    questionHeader = '{{pause}} Table Hot Spot {{pause}} This question contains a table. Some of the cells in the table can be selected by clicking on them. Click on the cells to answer the question.{{pause}}';
                } else if (questionSchemeId === '34') {
                    questionHeader = '{{pause}} Number Line Hot Spot {{pause}} This question contains a number line. You can select that number line\'s hash marks by clicking on them. {{pause}}';
                } else if (questionSchemeId === '36') {
                    questionHeader = '{{pause}} Drag and Drop Sequence {{pause}} For this question you will be asked to drag a series of draggable objects into the correct order.{{pause}}';
                }

                questionBody = questionHeader + questionContent;

                el.empty().append(questionBody);

                return el;
            }
        },
        replaceMark: function (str) {
            var that = this;

            if (that.options.speechType === 'question') {
                var questionNumber = parseInt($(that.el).parents('body').find('#questionNumber').text(), 10);
                var questionNumberSpeech = '';

                // Don't read question when STATIC.SETTINGS.displayQuestionNumber option is OFF
                if (isNaN(questionNumber)) {
                    questionNumberSpeech = '';
                } else {
                    questionNumberSpeech = 'Question ' + questionNumber;
                }

                str = questionNumberSpeech + str;
            }

            return str.split('{{pause}}');
        }
    };

    window.TextToSpeech = TextToSpeech;
})(jQuery, window);
