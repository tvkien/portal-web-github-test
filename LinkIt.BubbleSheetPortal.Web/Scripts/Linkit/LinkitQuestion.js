(function() {

// Require jQuery Library
var root = this;

// Create a safe reference to the LinkitQuestion object for use below.
var LinkitQuestion = function(obj) {
    if (obj instanceof LinkitQuestion) {
        return obj;
    }

    if (!(this instanceof LinkitQuestion)) {
        return new LinkitQuestion(obj);
    }

    this._wrapped = obj;
};

// Export the LinkitQuestion object for **Node.js**
// with backwards-compatinility for the old `require()` API.
// If we're in the browser, and `LinkitQuestion` as a global object.
if (typeof exports !== 'undefined') {
    if (typeof module !== 'undefined' && module.exports) {
        exports = module.exports = LinkitQuestion;
    }
    exports.LinkitQuestion = LinkitQuestion;
} else {
    root.LinkitQuestion = LinkitQuestion;
}

// Variables
LinkitQuestion.ImgUrl = '/TestAssignmentRegrader/GetViewReferenceImg?imgPath=';
LinkitQuestion.ImgUrlNotFound = '/Content/images/img-not-found.png';

/**
 * Load real images upload
 */
LinkitQuestion.LoadImageLinkit = function(el) {
    var $el = $(el);

    // Replace link all imglinkit upload
    $el.find('imglinkit').replaceWith(function() {
        var $img = $(this);
        var imgSrc = $img.attr('src');
        var $newImage = $('<img />');

        // If src attribute image is null get source attribute
        if (LinkitUtils.IsNullOrEmpty(imgSrc)) {
            var imgSrc = $img.attr('source');
        }

        // Check src attribute image exist or not
        if (LinkitUtils.IsNullOrEmpty(imgSrc)) {
            $img.attr('src', LinkitQuestion.ImgUrlNotFound);
            return;
        }

        // Check src attribute for linkit server or s3 server image
        if (imgSrc.substring(0, 7) === 'http://' ||
            imgSrc.substring(0, 8) === 'https://') {
            $img.attr('src', imgSrc).attr('source', '');
            return;
        }

        // Assign new link for image
        if (imgSrc.charAt(0) == '/') {
            imgSrc = imgSrc.substring(1);
        }

        if (imgSrc && imgSrc.toLowerCase().indexOf('itemset') >= 0
            && imgSrc.toLowerCase().indexOf('getviewreferenceimg') < 0) {
            imgSrc = LinkitQuestion.ImgUrl + imgSrc;
            imgSrc = imgSrc + '&timestamp=' + new Date().getTime();
            $img.attr('src', imgSrc).attr('source', '');
        }

        LinkitUtils.CopyAttributes($img, $newImage);

        return $newImage;
    });
};

/**
 * Load glossary
 */
LinkitQuestion.LoadGlossary = function(el, elGlossary) {
    var $el = $(el);
    var $elGlossary;

    if (elGlossary === undefined) {
        $elGlossary = $('#glossaryMessage');
    } else {
        $elGlossary = $(elGlossary);
    }

    $el.on('click', 'span.glossary', function (e) {
        var $self = $(this);
        var $glossaryMessage = $elGlossary;
        var glossary_text = $self.html();
        var glossary_content = $self.attr('glossary')
                                        .replace(/&lt;br\/&gt;/gi, '<br/>')
                                        .replace(/&gt;/g, '>')
                                        .replace(/&lt;/g, '<');
        var win = $(document);
        var z_index = parseInt($('.ui-dialog').css('z-index'));

        z_index = isNaN(z_index) ? 3 : z_index;

        $glossaryMessage.find('.glossary_text').html(glossary_text);
        $glossaryMessage.find('.glossary_define').html(glossary_content);

        $('body').prepend('<div class="ui-widget-overlay" style="width: ' + win.width() + 'px; height: ' + win.height() + 'px; z-index:' + (z_index + 1) + ';"></div>');

        $glossaryMessage.dialog({
            modal: false,
            width: 480,
            resizable: false,
            open: function (dialog) {
                $glossaryMessage.prev().css('top', '37px');
            },
            close: function () {
                $('.ui-widget-overlay').remove();
            }
        });
    }).on({
        mouseenter: function () {
            var currentID = $(this).attr('glossary_id');
            $el.find('span.glossary[glossary_id=' + currentID + ']').addClass('glossary-hover');
        },
        mouseleave: function () {
            var currentID = $(this).attr('glossary_id');
            $el.find('span.glossary[glossary_id=' + currentID + ']').removeClass('glossary-hover');
        }
    }, 'span.glossary');
}

}.call(this));