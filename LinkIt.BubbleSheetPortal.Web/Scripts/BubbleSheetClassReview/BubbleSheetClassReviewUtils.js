var BubbleSheetClassReviewUtils = (function () {
    
    function getHourTime (h) {
        return h >= 12 ? 'PM' : 'AM'
    }
      
    function getZeroPad (n) {
        return (parseInt(n, 10) >= 10 ? '' : '0') + n
    }

    return {
        getHourTime: getHourTime,
        getZeroPad: getZeroPad
    }

})();