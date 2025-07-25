function Reviewer() {};

Reviewer.IsNullOrEmpty = function (value) {
    return typeof (value) === "undefined" || value == null || $.trim(value) == '';
};

Reviewer.ParseInt = function (value) {
    if (typeof (value) === "undefined" || value == null || $.trim(value) == '') return 0;
    return parseInt(value);
};

/**
 * Check Content Guidance/Rationale
 * @param {[type]} ratio [description]
 */
Reviewer.GetGuidanceRationaleContent = function(ratio) {
    var emptyRationale = false;
    var $ratio = $(ratio);

    if($ratio.find('img, video').length > 0){
        emptyRationale = true;
    } else if ($.trim($ratio.text()) !== ''){
        emptyRationale = true;
    }

    return emptyRationale;
};
