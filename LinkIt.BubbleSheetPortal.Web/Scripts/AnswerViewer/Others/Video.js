/**
 * Video Module
 * @param  {[type]} function (             [description]
 * @return {[type]}          [description]
 */
var Video = (function () {
    /**
     * Display xml to html
     * @param  {[type]} xmlContent [description]
     * @return {[type]}            [description]
     */
    function displayHtml (xmlContent) {
        var $tree = $('<div/>');

        $tree.append(xmlContent);

        $tree.find('videolinkit').not('.multiplechoice-item videolinkit').replaceWith(function() {
            var $videolinkit = $(this);
            var $newVideolinkit = $('<video />');

            Utils.copyAttributes($videolinkit, $newVideolinkit);

            $newVideolinkit
                .html($videolinkit.html())
                .removeAttr('autoplay');

            return $newVideolinkit;
        });

        $tree.find('sourcelinkit').not('.multiplechoice-item sourcelinkit').replaceWith(function() {
            var $sourcelinkit = $(this);
            var $newSourcelinkit = $('<source />');

            Utils.copyAttributes($sourcelinkit, $newSourcelinkit );

            $newSourcelinkit .html($sourcelinkit.html());

            return $newSourcelinkit;
        });

        return $tree.html();
    }

    return {
        displayHtml: displayHtml
    }
})();
