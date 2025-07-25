(function ($) {
    'use strict';

    // Close passage
    $(document).on('click', '.js-get-passage', function (ev) {
        ev.preventDefault();
        var $self = $(this);
        var $passages = $('.VirtualQuestion-passages');
        var $passageLoading = $('.VirtualQuestion-passage-loading');
        var isExist = false;

        $self.addClass('is-active');

        if ($passages.find('.VirtualQuestion-passage').length) {
            $passages.find('.VirtualQuestion-passage').each(function (ind, passage) {
                var $passage = $(passage);

                if ($passage.data('index') === $self.data('index')) {
                    $passage.removeClass('hide');
                    isExist = true;
                }
            });
        }

        if (isExist) {
            $passages.removeClass('hide');
            return;
        }

        var params = {
            refObjectID: $self.data('refobjectid'),
            data: encodeURI($self.data('passage')),
            qti3pPassageID: $self.data('passageid'),
            qti3pSourceID: $self.data('sourceid'),
            dataFileUploadPassageID: $self.data('fileuploadpassageid'),
            dataFileUploadTypeID: $self.data('fileuploadtypeid')
        };

        var title = {
            index: $self.data('index'),
            refObjectID: $self.data('refobjectid'),
            refNumber: $self.data('refnumber')
        };

        $.ajax({
            url: configUrl.showPassageDetail,
            data: params,
            beforeSend: function () {
                $passageLoading.removeClass('hide');
            },
            success: function (data) {
                if (!data) {
                    data = 'Passage is not found.';
                }

                var dataContainer = getPassageContent(title, data);

                $passageLoading.addClass('hide');
                $passages.removeClass('hide')
                        .append(dataContainer);
            }
        });
    });

    // Close passage
    $(document).on('click', '.js-close-passage', function () {
        var $self = $(this);
        var $passages = $('.VirtualQuestion-passages');
        var $passage = $self.parents('.VirtualQuestion-passage');
        var passageIndex = $passage.data('index');

        $passage.addClass('hide');
        $('.js-get-passage[data-index="' + passageIndex + '"]').removeClass('is-active');

        if (!$passages.find('.VirtualQuestion-passage:visible').length) {
            $passages.addClass('hide');
        }
    })

    /**
     * Get passage content
     * @param  {[type]} title   [description]
     * @param  {[type]} content [description]
     * @return {[type]}         [description]
     */
    function getPassageContent(title, content) {
        var passageContainer = document.createElement('div');
        var passageHeader = document.createElement('div');
        var passageContent = document.createElement('div');
        var passageHeaderTitle = document.createElement('h4');
        var passageHeaderClose = document.createElement('span');

        if (title.refObjectID > 0) {
            passageHeaderTitle.innerHTML = 'Reference: ' + title.refObjectID;
        } else {
            passageHeaderTitle.innerHTML = 'Reference: ' + title.refNumber;
        }

        passageHeaderClose.className = 'VirtualQuestion-passage-close js-close-passage';
        passageHeaderClose.innerHTML = 'x';

        passageHeader.className = 'VirtualQuestion-passage-header';
        passageHeader.appendChild(passageHeaderTitle);
        passageHeader.appendChild(passageHeaderClose);

        passageContent.className = 'VirtualQuestion-passage-content';
        passageContent.innerHTML = content;

        passageContainer.setAttribute('data-index', title.index);
        passageContainer.className = 'VirtualQuestion-passage';
        passageContainer.appendChild(passageHeader);
        passageContainer.appendChild(passageContent);

        return passageContainer;
    }
}(jQuery));
