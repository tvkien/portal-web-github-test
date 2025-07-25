(function ($) {
    $.widget('jquery.GradingShortcutsWidget', {
        options: {
            WidgetUtil: null,
            GradingShortcutsUrl: '',
        },
        GradingShortcuts: function (self, successCallBack, errorCallBack) {
            var that = this;
            var options = that.options;

            $.ajax({
                type: 'POST',
                url: options.GradingShortcutsUrl,
                cache: false,
                contentType: 'application/json',
                data: JSON.stringify({
                    qtiTestClassAssignmentID: self.QTITestClassAssignmentID(),
                    QTIOnlineTestSessionID: self.QTIOnlineTestSessionID(),
                    GradeType: self.ShortcutGradeBy(),
                    AssignPointsEarned: self.ShortcutAssignType(),
                    UnAnswered: self.ShortcutUnAnswered(),
                    Answered: self.ShortcutAnswered(),
                    AnswerID: self.AnswerID(),
                    AnswerSubID: self.AnswerSubID(),
                    StudentGradingShortcuts: self.GetStudentGradingShortcuts()
                }),
                datatype: 'json',
                beforeSend: function() {
                    self.IsLoadingGradingShortcuts(true);
                },
                success: function(data) {
                    if (!options.WidgetUtil.IsNullOrEmpty(successCallBack)) {
                        successCallBack(data);
                    }
                },
                error: function(data) {
                    if (!options.WidgetUtil.IsNullOrEmpty(errorCallBack)) {
                        errorCallBack(data);
                    }
                }
            });
        },
        
    });
}(jQuery));
