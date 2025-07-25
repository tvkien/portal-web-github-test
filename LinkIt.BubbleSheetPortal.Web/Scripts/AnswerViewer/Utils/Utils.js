var Utils = (function () {
    var hasOwnProperty = Object.prototype.hasOwnProperty;
    var propIsEnumerable = Object.prototype.propertyIsEnumerable;

    /**
     * Copy attributes
     * @param  {[type]} from [description]
     * @param  {[type]} to   [description]
     * @return {[type]}      [description]
     */
    function copyAttributes(from, to) {
        var attrs = from.prop('attributes');
        $.each(attrs, function (index, attribute) {
            to.attr(attribute.name, attribute.value);
        });
    }

    /**
     * Is Null Or Empty
     * @param  {[type]}  value [description]
     * @return {Boolean}       [description]
     */
    function isNullOrEmpty(value) {
        return typeof (value) === 'undefined' || value == null || value.trim() == '';
    }

    /**
     * Check is object
     * @param  {[type]}  x [description]
     * @return {Boolean}   [description]
     */
    function isObject (x) {
        return typeof x === 'object' && x !== null;
    }

    /**
     * Check is array
     * @param  {[type]}  x [description]
     * @return {Boolean}   [description]
     */
    function isArray (x) {
        return isObject(x) && x instanceof Array;
    }

    /**
     * Change to object
     * @param  {[type]} val [description]
     * @return {[type]}     [description]
     */
    function toObject (val) {
    	if (val === null || val === undefined) {
    		throw new TypeError('Sources cannot be null or undefined');
    	}

    	return Object(val);
    }

    /**
     * Assign key
     * @param  {[type]} to   [description]
     * @param  {[type]} from [description]
     * @param  {[type]} key  [description]
     * @return {[type]}      [description]
     */
    function assignKey (to, from, key) {
    	var val = from[key];

    	if (val === undefined || val === null) {
    		return;
    	}

    	if (hasOwnProperty.call(to, key)) {
    		if (to[key] === undefined || to[key] === null) {
    			throw new TypeError('Cannot convert undefined or null to object (' + key + ')');
    		}
    	}

    	if (!hasOwnProperty.call(to, key) || !isObject(val)) {
    		to[key] = val;
    	} else {
    		to[key] = assign(Object(to[key]), from[key]);
    	}
    }

    /**
     * Assign to from
     * @param  {[type]} to   [description]
     * @param  {[type]} from [description]
     * @return {[type]}      [description]
     */
    function assign (to, from) {
    	if (to === from) {
    		return to;
    	}

    	from = Object(from);

    	for (var key in from) {
    		if (hasOwnProperty.call(from, key)) {
    			assignKey(to, from, key);
    		}
    	}

    	if (Object.getOwnPropertySymbols) {
    		var symbols = Object.getOwnPropertySymbols(from);

    		for (var i = 0; i < symbols.length; i++) {
    			if (propIsEnumerable.call(from, symbols[i])) {
    				assignKey(to, from, symbols[i]);
    			}
    		}
    	}

    	return to;
    }

    /**
     * Deep assign
     * @param  {[type]} target [description]
     * @return {[type]}        [description]
     */
    function deepAssign (target) {
    	target = toObject(target);

    	for (var s = 1; s < arguments.length; s++) {
    		assign(target, arguments[s]);
    	}

    	return target;
    };

    /**
     * Replace paragraph in textarea value
     * @param  {[type]} str [description]
     * @return {[type]}     [description]
     */
    function replaceParagraph (str) {
        if (typeof str === 'string') {
            str = str.replace(/\r?\n/g, '<br />');
            return str;
        }
    }

    /**
     * Replace opended  value (ckeditor value)
     * @param  {[type]} str [description]
     * @return {[type]}     [description]
     */
    function replaceStringLessOrLarge (str) {
        if (typeof str === 'string') {
            str = str.replace(/&#60;/g, '<').replace(/&#62;/g, '>');
            return str;
        }
    }

    /**
     * Left slash
     * @param  {[type]} str [description]
     * @return {[type]}     [description]
     */
    function leftSlash(str) {
        if (str.charAt(0) === '/') {
            str = str.slice(1);
        }

        return str;
    }

    /**
     * Right slash
     * @param  {[type]} str [description]
     * @return {[type]}     [description]
     */
    function rightSlash(str) {
        if (str.slice(-1) !== '/') {
            str += '/';
        }

        return str;
    }

    return {
        copyAttributes: copyAttributes,
        isNullOrEmpty: isNullOrEmpty,
        deepAssign: deepAssign,
        replaceParagraph: replaceParagraph,
        replaceStringLessOrLarge: replaceStringLessOrLarge,
        isArray: isArray,
        leftSlash: leftSlash,
        rightSlash: rightSlash
    }
})();
