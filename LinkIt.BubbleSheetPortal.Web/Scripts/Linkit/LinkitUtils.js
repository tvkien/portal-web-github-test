(function() {

// Require jQuery Library
var root = this;

// Create a safe reference to the LinkitUtils object for use below.
var LinkitUtils = function(obj) {
    if (obj instanceof LinkitUtils) {
        return obj;
    }

    if (!(this instanceof LinkitUtils)) {
        return new LinkitUtils(obj);
    }

    this._wrapped = obj;
};

// Export the LinkitUtils object for **Node.js**
// with backwards-compatinility for the old `require()` API.
// If we're in the browser, and `LinkitUtils` as a global object.
if (typeof exports !== 'undefined') {
    if (typeof module !== 'undefined' && module.exports) {
        exports = module.exports = LinkitUtils;
    }
    exports.LinkitUtils = LinkitUtils;
} else {
    root.LinkitUtils = LinkitUtils;
}

/**
 * Check value is null or empty
 */
LinkitUtils.IsNullOrEmpty = function(value) {
    return typeof (value) === 'undefined' || value == null || $.trim(value) === '';
};

/**
 * Check input is number or not
 */
LinkitUtils.IsNumber = function(n) {
    try {
        var x = parseInt(n);
        return !isNaN(x);
    } catch(e) {
        return false;
    }
};

/**
 * Copy attributes from element to other element
 */
LinkitUtils.CopyAttributes = function(from, to) {
    var attrs = from.prop('attributes');
    $.each(attrs, function (index, attribute) {
        to.attr(attribute.name, attribute.value);
    });
}

/**
 * Replace linkitvideo and sourcelinkit become video and source
 */
LinkitUtils.ReplaceLinkitVideo = function(html) {
    if (LinkitUtils.IsNullOrEmpty(html)) {
        return;
    }

    html = html.replace(/<videolinkit/g, '<video')
                .replace(/<\/videolinkit>/g, '</video>')
                .replace(/<sourcelinkit /g, '<source ')
                .replace(/<\/sourcelinkit>/g, '</source>');
    return html;
}

}.call(this));