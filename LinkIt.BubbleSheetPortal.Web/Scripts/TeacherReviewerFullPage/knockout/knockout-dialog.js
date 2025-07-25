// Binding Handers Dialog jQuery UI
ko.bindingHandlers.dialog = {
    init: function (element, valueAccessor, allBindingsAccessor) {
        var options = ko.utils.unwrapObservable(valueAccessor()) || {};
        // Do in a setTimeout, so the applyBindings doesn't bind twice from element being copied and moved to bottom
        setTimeout(function () {
            options.close = function () {
                allBindingsAccessor().dialogVisible(false);
            };

            $(element).dialog(options);
        }, 0);

        // Handle disposal (not strictly necessary in this scenario)
        ko.utils.domNodeDisposal.addDisposeCallback(element, function () {
            $(element).dialog('destroy');
        });
    },
    update: function (element, valueAccessor, allBindingsAccessor) {
        var shouldBeOpen = ko.utils.unwrapObservable(allBindingsAccessor().dialogVisible),
            $el = $(element),
            dialog = $el.data('uiDialog') || $el.data('dialog');

        // Don't call open/close before initilization
        if (dialog) {
            $el.dialog(shouldBeOpen ? 'open' : 'close');

            if (shouldBeOpen) {
                var $doc = $(document);
                var $overlayPopup = $('<div/>');

                $overlayPopup
                    .addClass('ui-overlay')
                    .css({
                        'z-index': $('.ui-dialog:visible').css('z-index') - 1,
                        'width': $doc.width() + 'px',
                        'height': $doc.height() + 'px',
                        'position': 'absolute',
                        'top': '0',
                        'left': '0',
                        'background-color': '#000',
                        'opacity': '0.3'
                    });

                $overlayPopup.appendTo($('body'));
            } else {
                $('body').find('.ui-overlay').remove();
            }
        }
    }
};