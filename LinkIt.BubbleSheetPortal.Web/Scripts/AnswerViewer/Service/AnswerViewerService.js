var AnswerViewerService = (function () {

    function getAnswerForStudent (context, params) {
        var ANSWER_URL = '/TestAssignmentRegrader/GetAnswerForStudent/';
        return context.$http.get(ANSWER_URL, {params: params});
    }

    function getPassage (context, params) {
        var PASSAGE_URL = '/ShowQtiItem/ShowPassageDetail/';
        return context.$http.get(PASSAGE_URL, {params: params});
    }

    return {
        getAnswerForStudent: getAnswerForStudent,
        getPassage: getPassage
    }
})();
