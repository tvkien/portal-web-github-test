(function($) {
    'use strict';

    // Tab Pane
    var $itemLibraryTab = $('.itemLibraryTab');

    if ($itemLibraryTab.length) {
        var $itemLibraryTabListItem = $itemLibraryTab.find('.itemLibraryTab-list-item');
        var $itemLibraryTabPane = $itemLibraryTab.find('.itemLibraryTab-pane');

        $itemLibraryTabListItem.on('click', function() {
            var $self = $(this);
            var dataTab = $self.data('tab');

            $itemLibraryTabListItem.removeClass('active');
            $self.addClass('active');

            $itemLibraryTabPane.hide();
            $itemLibraryTab.find('.itemLibraryTab-pane[id="' + dataTab + '"]').show();
        });
    }
}(jQuery));
