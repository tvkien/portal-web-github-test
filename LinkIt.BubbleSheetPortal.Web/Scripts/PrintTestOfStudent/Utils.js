jQuery.fn.outerHTML = function (s) {
    return (s)
        ? this.before(s).remove()
        : jQuery("<div>").append(this.eq(0).clone()).html();
};

(function ($) {
    $.fn.imagesLoaded = function (callback) {
        var elems = this.filter('img'),
            len = elems.length,
            // data uri bypasses webkit log warning (thx doug jones (cjboco))
            blank = "data:image/gif;base64,R0lGODlhAQABAIAAAAAAAP///ywAAAAAAQABAAACAUwAOw==";
        elems.bind('load', function () {
            // check image src to prevent firing twice (thx doug jones (cjboco))
            if (--len <= 0 && this.src !== blank) {
                callback.call(elems, this);
            }
        }).each(function () {
            // cached images don't fire load sometimes, so we reset src.
            if (this.complete || this.complete === undefined) {
                var src = this.src;
                // webkit hack from http://groups.google.com/group/jquery-dev/browse_thread/thread/eee6ab7b2da50e1f
                this.src = blank;
                this.src = src;
            }
        });
    };
}(jQuery));

function TOSUtils() { };

TOSUtils.CopyAttributes = function (from, to) {
    var attrs = from.prop("attributes");
    $.each(attrs, function (index, attribute) {
        to.attr(attribute.name, attribute.value);
    });
};

/**
 * Check Content Guidance/Rationale
 * @param {[type]} ratio [description]
 */
TOSUtils.GetGuidanceRationaleContent = function(ratio) {
    var emptyRationale = false;
    var $ratio = $(ratio);

    if($ratio.find('img, video').length > 0){
        emptyRationale = true;
    } else if ($.trim($ratio.text()) !== ''){
        emptyRationale = true;
    }

    return emptyRationale;
};

/**
 * Check is string
 * @param  {[type]}  str [description]
 * @return {Boolean}     [description]
 */
TOSUtils.isString = function (str) {
    return Object.prototype.toString.call(str) === '[object String]';
};

/**
 * Replace less or large html enties
 * @type {[type]}
 */
TOSUtils.replaceStringLessOrLarge = function (str) {
    if (TOSUtils.isString(str)) {
        return str.replace(/&#60;/g, '<').replace(/&#62;/g, '>');
    }

    return '';
};
