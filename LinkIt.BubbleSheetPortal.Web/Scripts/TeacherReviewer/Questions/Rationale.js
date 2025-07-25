(function ($) {
    var $doc = $(document);

    $doc.on('mouseover', '.bntGuidance', function(e) {
        var $self = $(this);
        var $target = $(e.target);
        var $element;
        var responseIdentifier;
        var dataGuidanceContent = '';

        if ($target.attr('responseidentifier') !== undefined) {

            if ($target.attr('name') === 'textEntryGuidance') {
                // Text entry
                var idresponse = $target.parent().attr('idresponse');
                responseIdentifier = $target.attr('responseidentifier');
                $element = $target.parents().find('div[identifier_responseidentifier="' + responseIdentifier + '_' + idresponse + '"]');
            } else {
                // Inline choice
                responseIdentifier = $target.attr('responseidentifier');
                $element = $target.parent().find('div[idreponse="' + responseIdentifier + '"]');
            }
        } else {
            // Multiple choice and true/false
            var identifier = $target.attr('identifier');
            responseIdentifier = $target.parents('div[responseidentifier]').attr('responseidentifier');
            $element = $target.parents('div[responseidentifier]').find('div[identifier_responseidentifier="' + identifier + '_' + responseIdentifier + '"]');
        }

        if ($element.length) {
            dataGuidanceContent = $element.html();
            // Do not autoplay video in guidance
            dataGuidanceContent = dataGuidanceContent.replace(/<guidancevideolinkit /g, '<video ')
                                                    .replace(/<\/guidancevideolinkit>/g, '</video>')
                                                    .replace(/<guidancesourcelinkit /g, '<source ')
                                                    .replace(/<\/guidancesourcelinkit>/g, '</source>')
                                                    .replace(/autoplay=\"autoplay\"/g, '');

            // Call tooltip of guidance
            $self.tooltipster({
                interactive: true,
                contentAsHTML: true,
                maxWidth: 600,
                delay: 50,
                debug: false,
                multiple: true,
                content: dataGuidanceContent,
                functionAfter: function() {
                    if (window.playsound !== undefined) {
                        window.playsound.pause();
                    }
                }
            });

            $self.tooltipster('show', function () {
                var $bntPlay = $('.bntPlay');
                var $bntStop = $('.bntStop');

                $bntPlay.show();
                $bntStop.hide();

                $bntPlay.on('click', function (event) {
                    playVNSAudio($(event.target));
                });

                $bntStop.on('click', function (event) {
                    stopVNSAudio($(event.target));
                });
            });

        }
    });

}(jQuery));
