/**
 * Text to speech ckeditor plugin
 * @required text-to-speech-utils.js
 * @param  {[type]} editor [description]
 * @return {[type]}        [description]
 */
CKEDITOR.dialog.add('texttospeech', function(editor) {
    var lang = editor.lang.texttospeech;
    var texttospeechWidget;

    return {
        title: lang.title,
        minWidth: 300,
        minHeight: 150,
        contents: [{
            id: 'texttospeech',
            elements: [
                {
                    type: 'html',
                    html: '\
                            <div class="texttospeech">\
                                <strong>Enable text to speech</strong>\
                                <div class="u-m-l-10 texttospeech-checkbox">\
                                    <input type="checkbox" id="texttospeechEnable" name="texttospeechEnable" />\
                                    <label for="texttospeechEnable" ></label>\
                                </div>\
                            </div>'
                },
                {
                    type: 'select',
                    id: 'texttospeechRate',
                    label: 'Speed',
                    items: [
                        ['Slow', '0.7'],
                        ['Normal', '0.8']
                    ],
                    'default': '1',
                    className: 'texttospeech',
                    onChange: function () {
                        if (!!texttospeechWidget) {
                            texttospeechWidget.reset();
                            texttospeechWidget.setRate(this.getValue());
                        }
                    }
                },
                {
                    type: 'select',
                    id: 'texttospeechVolume',
                    label: 'Volume',
                    items: [
                        ['0', '0'],
                        ['1', '0.1'],
                        ['2', '0.2'],
                        ['3', '0.3'],
                        ['4', '0.4'],
                        ['5', '0.5'],
                        ['6', '0.6'],
                        ['7', '0.7'],
                        ['8', '0.8'],
                        ['9', '0.9'],
                        ['10', '1']
                    ],
                    'default': '5',
                    className: 'texttospeech',
                    onChange: function () {
                        if (!!texttospeechWidget) {
                            texttospeechWidget.reset();
                            texttospeechWidget.setVolume(this.getValue());
                        }
                    }
                },
                {
                    type: 'html',
                    html: '\
                            <div class="texttospeech">\
                                <label for="texttospeechPlay">Click here to review</label>\
                                <div class="u-m-t-10"><div id="texttospeechPlayPlugin"></div></div>\
                            </div>'
                }
            ]
        }],
        onShow: function () {
            var dialog = this;
            var dialogDocument = dialog.getElement().getDocument();
            var texttospeechPlayPluginId = dialogDocument.getById('texttospeechPlayPlugin');
            var texttospeechEnableId = dialogDocument.getById('texttospeechEnable');
            var texttospeechContent = xmlExport(true);
            var texttospeech = iResultComponent.texttospeech;
            var texttospeechEnable = texttospeech === undefined ? true : JSON.parse(texttospeech.enable);
            var texttospeechRate = texttospeech === undefined ? '0.8' : texttospeech.rate;
            var texttospeechVolume = texttospeech === undefined ? '0.5' : texttospeech.volume;

            $(texttospeechEnableId.$).prop('checked', texttospeechEnable);
            dialog.setValueOf('texttospeech', 'texttospeechRate', texttospeechRate);
            dialog.setValueOf('texttospeech', 'texttospeechVolume', texttospeechVolume);

            texttospeechContent = replaceVideo(texttospeechContent);

            getTexttospeech($(texttospeechPlayPluginId.$).get(0), {
                xmlContent: texttospeechContent,
                qtiSchemeId: iSchemeID,
                voice: "US English Female",
                options: {
                    rate: texttospeechRate,
                    volume: texttospeechVolume
                }
            });
        },
        onOk: function () {
            var dialog = this;
            var texttospeechEnableId = dialog.getElement().getDocument().getById('texttospeechEnable');
            var texttospeechEnable = $(texttospeechEnableId.$).is(':checked');
            var texttospeechRate = dialog.getValueOf('texttospeech', 'texttospeechRate');
            var texttospeechVolume = dialog.getValueOf('texttospeech', 'texttospeechVolume');

            // Update text to speech options
            iResultComponent.texttospeech = {
                enable: texttospeechEnable,
                rate: texttospeechRate,
                volume: texttospeechVolume
            };

            stopTexttospeech();
        },
        onCancel: function () {
            stopTexttospeech();
        }
    };

    /**
     * Stop text to speech
     * @return {[type]} [description]
     */
    function stopTexttospeech() {
        if (!!responsiveVoice) {
            responsiveVoice.cancel();
        }
    }

    /**
     * Get text to speech
     * @param  {[type]} element [description]
     * @param  {[type]} options [description]
     * @return {[type]}         [description]
     */
    function getTexttospeech(element, options) {
        stopTexttospeech();

        texttospeechWidget = new TextToSpeech(element, {
            xmlContent: options.xmlContent,
            qtiSchemeId: options.qtiSchemeId,
            voice: options.voice,
            options: {
                rate: options.options.rate,
                volume: options.options.volume
            },
            showText: false
        });
    }

    /**
     * Replace video tag to videolinkit tag
     * @param  {[type]} xml [description]
     * @return {[type]}     [description]
     */
    function replaceVideo(originalString) {
        if (originalString) {
          originalString = originalString.replace(/<video /g, "<videolinkit ")
            .replace(/<\/video>/g, "</videolinkit>")
            .replace(/<source /g, "<sourcelinkit ")
            .replace(/<\/source>/g, "</sourcelinkit>");
      
            var $xml = $('<div>' + originalString + '</div>');
            $xml.find('sourcelinkit').each(function (index, source) {
                if (source.parentElement.tagName.toLowerCase() === 'audio') {
                var newSource = document.createElement('source');
                Array.from(source.attributes).forEach(function (attr) {
                    newSource.setAttribute(attr.name, attr.value);
                });
                newSource.innerHTML = source.innerHTML;
                source.replaceWith(newSource);
                }
            });
        
            return $xml.prop('innerHTML');
        }
      
        return originalString;
    }
});
