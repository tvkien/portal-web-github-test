var BubbleSheetReview = (function () {
    
    function getBubbleReviewDetailsPage (el) {
        var $el = $(el);
        var host = window.location.href;
    
        $el.each(function () {
            var $page = $(this);
            var pageUrl = $page.data('url');
    
            if (host.indexOf(pageUrl) > -1) {
                $page.addClass('is-checked');
            } else {
                var newHost = '';
    
                if (host.indexOf('Index') > -1) {
                    newHost = host.replace(/BubbleSheetReviewDetails\/Index/g, 'BubbleSheetClassView/ClassViewPage');
                } else if (host.indexOf('ClassViewPage') > -1) {
                    newHost = host.replace(/BubbleSheetClassView\/ClassViewPage/g, 'BubbleSheetReviewDetails/Index');
                }
    
                $page.addClass('u-cursor-pointer').attr('href', newHost);
            }
        });
    }

    function getBubbleReviewCurrentTab () {
        $('#generateTab').addClass('current');
        $('#managebubblesheetsReview').addClass('current');
    }

    return {
        getBubbleReviewDetailsPage: getBubbleReviewDetailsPage,
        getBubbleReviewCurrentTab: getBubbleReviewCurrentTab
    }

})();

