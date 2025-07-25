(function ($) {
    $.widget('jquery.StudentFilterWidget', {
        options: {
            WidgetUtil: null,
            Self: null
        },
        Constructor: function(self) {
            this.options.Self = self;
        },
        FilterStudent: function (students, filterBy) {
            var self = this.options.Self;

            ko.utils.arrayForEach(students, function (student) {
                if (student.StudentID() == -1) {
                    student.StudentVisible(true);
                } else if (filterBy == self.AllStudentFilter().FilterValue()) {
                    student.StudentVisible(true);
                } else if (filterBy == self.CompletedStudentFilter().FilterValue() && student.IsComplete()) {
                    student.StudentVisible(true);
                } else if (filterBy == self.ReadyToSubmitStudentFilter().FilterValue() && student.CanBulkGrading()) {
                    student.StudentVisible(true);
                } else if (filterBy == self.PendingReviewStudentFilter().FilterValue() && student.IsPendingReview() && !student.CanBulkGrading()) {
                    student.StudentVisible(true);
                } else if (filterBy == self.InprogressStudentFilter().FilterValue() && student.InProgress()) {
                    student.StudentVisible(true);
                } else if (filterBy == self.PausedStudentFilter().FilterValue() && student.Paused()) {
                    student.StudentVisible(true);
                } else if (filterBy == self.NotStartedStudentFilter().FilterValue() && student.IsNotStart()) {
                    student.StudentVisible(true);
                } else {
                    student.StudentVisible(false);
                }
            });
        },
        GetStudentByQTIOnlineTestSessionID: function (qtiOnlineTestSessionID) {
            var result = null;

            if (this.options.WidgetUtil.IsNullOrEmpty(qtiOnlineTestSessionID)) {
                return result;
            }

            result = ko.utils.arrayFirst(this.options.Self.Students(), function (student) {
                return qtiOnlineTestSessionID == student.QTIOnlineTestSessionID();
            });

            return result;
        },
        GetStudentByStudentID: function (studentID) {
            var result = null;

            if (this.options.WidgetUtil.IsNullOrEmpty(studentID)) {
                return result;
            }

            result = ko.utils.arrayFirst(this.options.Self.Students(), function (student) {
                return studentID === student.StudentID();
            });

            return result;
        }
    });
}(jQuery));
