var BubbleSheetClassReviewService = (function () {

    function getBubbleSheetFiles(params) {
        var url = '/BubbleSheetClassView/GetBubbleSheetFileByBubbleSheetId/';
        return $.get(url, params);
    }
    function autoSaved(params) {
        var url = '/BubbleSheetClassView/AutoSaved/';
        return $.post(url, params);
    }
    function deleteAutoSavedData(params) {
        var url = '/BubbleSheetClassView/DeleteAutoSavedData/';
        return $.post(url, params);
    }
    function submitClassView(params) {
        var url = '/BubbleSheetClassView/SubmitTestQuestions/';
        return $.ajax({
            type: 'POST',
            url: url,
            contentType: 'application/json',
            dataType: "json",
            data: JSON.stringify(params)
        });
    }
    function markAsAbsent(params) {
        var url = '/BubbleSheetClassView/MarkStudentAsAbsent/';
        return $.ajax({
            type: 'POST',
            url: url,
            contentType: 'application/json',
            dataType: "json",
            data: JSON.stringify(params)
        });
    }
    function refreshedStudentDetails(params) {
        var url = '/BubbleSheetClassView/GetStudentsRefreshDetail/';
        return $.post(url, params);
    }

    function getImageUrl(params) {
        var url = '/BubbleSheetClassView/GetImageUrl/';
        return $.ajax({
            type: 'POST',
            url: url,
            contentType: 'application/json',
            dataType: "json",
            data: JSON.stringify(params)
        });
    }
    function deleteArtifactFile(params) {
        var url = '/BubbleSheetReviewDetails/DeleteArtifactFile';
        return $.ajax({
          type: 'POST',
          url: url,
          contentType: 'application/json',
          dataType: "json",
          data: JSON.stringify(params)
        })
    }
    
    return {
        getBubbleSheetFiles: getBubbleSheetFiles,
        autoSaved: autoSaved,
        deleteAutoSavedData: deleteAutoSavedData,
        submitClassView: submitClassView,
        markAsAbsent: markAsAbsent,
        refreshedStudentDetails: refreshedStudentDetails,
        getImageUrl: getImageUrl,
        deleteArtifactFile: deleteArtifactFile
    }
})();
