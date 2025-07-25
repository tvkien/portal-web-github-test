// Binding handers showblock loading
ko.bindingHandlers.showblock = {
    update: function (element, valueAccessor, allBindingsAccessor) {
        var value = valueAccessor();
        var $element = $(element);
        var valueUnwrapped = ko.utils.unwrapObservable(value);

        if (valueUnwrapped) {
            ShowBlock($element, 'Loading');
        } else {
            $element.unblock();
            $element.removeAttr('style');
        }
    }
};