(function ($, win) {
  var rwebkit = /(webkit)[ \/]([\w.]+)/,
  ropera = /(opera)(?:.*version)?[ \/]([\w.]+)/,
  rmsie = /(msie) ([\w.]+)/,
  rmozilla = /(mozilla)(?:.*? rv:([\w.]+))?/;

  $.browser = {};

  $.extend({
    uaMatch: function (ua) {
      ua = ua.toLowerCase();

      var match = rwebkit.exec(ua) ||
        ropera.exec(ua) ||
        rmsie.exec(ua) ||
        ua.indexOf("compatible") < 0 && rmozilla.exec(ua) ||
        [];

      return { browser: match[1] || "", version: match[2] || "0" };
    }
  })

  var userAgent = navigator.userAgent,
    browserMatch = $.uaMatch(userAgent);
  if (browserMatch.browser) {
    $.browser[browserMatch.browser] = true;
    $.browser.version = browserMatch.version;
  }

  $.curCSS = function (element, prop, val) {
    if (!val) {
      return $(element).css(prop);
    }
    return $(element).css(prop, val);
  };

  var originFunctions = {
    outerHeight: $.fn.outerHeight,
    outerWidth: $.fn.outerWidth,
    attr: $.fn.attr,
    remove: $.fn.remove,
    html: $.fn.html
  }

  $.fn.outerWidth = function (size, margin) {
    if (size === null || size === undefined) {
      return originFunctions.outerWidth.call(this, false);
    }

    return originFunctions.outerWidth.call(this, size, margin);
  };

  $.fn.outerHeight = function (size, margin) {
    if (size === null || size === undefined) {
      return originFunctions.outerHeight.call(this, false);
    }

    return originFunctions.outerHeight.call(this, size, margin);
  };

  $.fn.live = function (event, callback) {
    if (this._selector) {
      return $.fn.on.apply($(win.document), [event, this._selector, callback]);
    }
    return $.fn.on.apply(this, arguments);
  };

  $.fn.die = function (event) {
    if (this._selector) {
      return $.fn.off.apply($(win.document), [event, this._selector]);
    }
    return $.fn.off.apply(this, arguments);
  };

  $.fn.attr = function (event, val) {
    if (event === 'checked' && (typeof val === 'boolean' || val === 'checked')) {
      return $.fn.prop.apply(this, [event, !!val]);
    }
    return originFunctions.attr.apply(this, arguments);
  };

  $.fn.remove = function () {
    if (this.hasClass('ui-dialog') || this.hasClass('ui-dialog-content')) {
      $.fn.dialog.apply(this, ['destroy']);
    }
    return originFunctions.remove.apply(this, arguments);
  };

  function unSelfCloseTag(htmlString) {
    if (typeof htmlString === 'string') {
      var excludes = 'area,base,br,col,embed,hr,img,input,link,meta,source,track,wbr'.split(',');
      var selfClosingTagRegex = /<[^>]+\/>/g;
      var matches = htmlString.match(selfClosingTagRegex) || [];
      matches.forEach(function (stringElement) {
        var tagName = stringElement.trim().replace('<', '').replace('/>', '').split(' ')[0].trim();
        if (!excludes.includes(tagName)) {
          var newStringElement = stringElement.replace('/>', '></' + tagName + '>');
          htmlString = htmlString.replace(stringElement, newStringElement);
        }
      })
    }
    return htmlString;
  }

  $.fn.html = function () {
    if (typeof arguments[0] === 'string') {
      arguments[0] = unSelfCloseTag(arguments[0]);
    }
    return originFunctions.html.apply(this, arguments);
  }

  // change output from undefined to null for jquery qtip
  var func = ['width', 'height', 'outerWidth', 'outerHeight', 'scrollLeft', 'scrollTop'];
  func.forEach(function(name) {
    var old = $.fn[name];
    $.fn[name] = function () {
      var val = old.apply(this, arguments);
      if (val === undefined) {
        val = null;
      }
      return val;
    }
  })
  // end jquery qtip migrate

  function extJQuery (e, t) {
    if (typeof e === 'string') {
      e = unSelfCloseTag(e);
    }
    var jElement = new $.fn.init(e,t);
    if (typeof e === 'string' && !e.includes('/')) {
      jElement._selector = e;
    }
    return jElement;
  }

  $.extend(extJQuery, $);
  win.$ = win.jQuery = extJQuery;

  var updateNativeSelect = function(e) {
    var isFirefox = navigator.userAgent.indexOf("Firefox") != -1;
    var maxWidth = Math.min(window.innerWidth - e.target.getBoundingClientRect().x - (isFirefox ? 60 : 40), 600);
    var testNode = document.createElement('span');
    testNode.style.position = 'fixed';
    testNode.style.top = -10000;
    testNode.style.fontSize = isFirefox ? '1.1rem' : '1rem';
    document.body.appendChild(testNode);
    $(e.target).find('option').each(function(index, option) {
      if (option.text) {
        if (!option.getAttribute('originalLabel')) {
          option.setAttribute('originalLabel', option.text)
        }
        testNode.innerText = option.text;
        while(testNode.getBoundingClientRect().width > maxWidth) {
          testNode.innerText = testNode.innerText.slice(0, testNode.innerText.length - 4) + '...';
        }
        if (option.text !== testNode.innerText) {
          option.text = testNode.innerText;
        }
      }
    })
    testNode.remove();
  }

  $(document).on('mouseenter', 'select', updateNativeSelect)
    .on('focus', 'select', updateNativeSelect)
    .on('change', 'select', function(e) {
      $(e.target).find('option').each(function(index, option) {
        var originalText = option.getAttribute('originalLabel');
        if (originalText && originalText !== option.text) {
          option.text = originalText;
        }
      })
    });

})(jQuery, window);
