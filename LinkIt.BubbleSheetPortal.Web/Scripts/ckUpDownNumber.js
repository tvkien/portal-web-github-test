(function ($, window, document, undefined) {

    var pluginName = "ckUpDownNumber",
        defaults = {
            height: 20,
            width: 20,
            minNumber: 0,
            maxNumber: 9000
        };

    function Plugin(element, options) {
        this.element = element;

        this.settings = $.extend({}, defaults, options);

        this._defaults = defaults;
        this._name = pluginName;
        this.init();
    }

    // Avoid Plugin.prototype conflicts
    $.extend(Plugin.prototype, {

        init: function () {
            this.generate();
            this.change();
        },

        generate: function () {
            var self = this;
            var el = self.element;
            var $el = $(el);

            $el
                .attr({
                    'maxNumber': self.settings.maxNumber,
                    'minNumber': self.settings.minNumber
                })
                .wrap("<div class='ckUpDownNumber ckUpDownNumber-custom' style='width:" + self.settings.width + "px;height:" + self.settings.height + "px;'></div>")
                .parent()
                .append('<input class="ckbutton ckUDNumber ckUpNum" type="button" value="&#9650;" />' +
                    '<input class="ckbutton ckUDNumber ckDownNum' +
                    '" type="button" value="&#9660;" />');

            $el.siblings('.ckUpNum').unbind('click').on('click', function (event) {
                event.preventDefault();
                var elValue = parseInt($el.val(), 10);

                if (elValue >= $el.attr('maxnumber')) {
                    return;
                }

                if (elValue == '') {
                    $el.val('0');
                }

                $el.val(elValue + 1);
                //trigger auto change for resize content area
                $el.parent('.ckUpDownNumber').find('#nHeight').trigger('change.height');
                $el.parent('.ckUpDownNumber').find('#nWidth').trigger('change.width');
            });

            $el.siblings('.ckDownNum').unbind('click').on('click', function (event) {
                event.preventDefault();
                var elValue = parseInt($el.val(), 10);

                if ($el.attr('minnumber') >= elValue) {
                    return;
                }

                $el.val(elValue - 1);
                //trigger auto change for resize content area
                $el.parent('.ckUpDownNumber').find('#nHeight').trigger('change.height');
                $el.parent('.ckUpDownNumber').find('#nWidth').trigger('change.width');
            });
        },
        change: function () {
            var self = this;
            var el = self.element;
            var $el = $(el);

            $el.on('keydown', function (event) {

                var allowKeydown = [46, 8, 9, 27, 13];
                if (self.settings.minNumber < 0) {
                    allowKeydown = [46, 8, 9, 27, 13, 109, 189];
                }
                // Allow: backspace, delete, tab, escape, enter and .
                if ($.inArray(event.keyCode, allowKeydown) !== -1 ||
                    // Allow: Ctrl+A
                   (event.keyCode == 65 && event.ctrlKey === true) ||
                    // Allow: home, end, left, right
                   (event.keyCode >= 35 && event.keyCode <= 39)) {
                    // let it happen, don't do anything
                    return;
                }
                else {
                    // Ensure that it is a number and stop the keypress
                    if (event.shiftKey || (event.keyCode < 48 || event.keyCode > 57) && (event.keyCode < 96 || event.keyCode > 105)) {
                        event.preventDefault();
                    }
                }
            });

            $el.on('change', function () {
                var $that = $(this);
                var iVal = $that.val();
                if (parseInt(iVal) > self.settings.maxNumber) {
                    iVal = self.settings.maxNumber;
                } else if (parseInt(iVal) < self.settings.minNumber) {
                    iVal = self.settings.minNumber;
                }
                $that.val(iVal);
            });
        }
    });

    // A really lightweight plugin wrapper around the constructor,
    // preventing against multiple instantiations
    $.fn[pluginName] = function (options) {
        this.each(function () {
            if (!$.data(this, "plugin_" + pluginName)) {
                $.data(this, "plugin_" + pluginName, new Plugin(this, options));
            }
        });

        // chain jQuery functions
        return this;
    };

})(jQuery, window, document);