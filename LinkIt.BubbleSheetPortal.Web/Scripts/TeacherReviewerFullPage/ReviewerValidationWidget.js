(function($) {
    $.widget('jquery.ReviewerValidationWidget', {
        options: {
            WidgetUtil: null,
            Self: null
        },
        Constructor: function(self) {
            this.options.Self = self;
        },
        CheckRequireSubmitTestFilterStudent: function() {
            var that = this;
            var self = that.options.Self;

            if (that.options.WidgetUtil.IsNullOrEmpty(self.SelectedStudent)) {
                return false;
            }

            if (self.RequireSubmitTest() && self.GradingType() == 'student') {
                var options = {
                    Message: '',
                    HideCloseButton: 1
                };

                if (self.SelectedStudent.CanBulkGrading()) {
                    options.Message = 'You have graded all questions but have not submitted the test. Would you like to submit it now?';
                    ConfirmSubmitTest(options, function () {
                        self.IsGraded(false);
                        self.PostSubmitTestData();
                        self.SelectedStudentFilter(self.SelectedStudentFilterNew());
                        self.SelectFilterStudentChangeTrigger();
                        self.SelectedStudentFilterFunction();
                    }, function () {
                        self.IsGraded(false);
                        self.SelectedStudentFilter(self.SelectedStudentFilterNew());
                    });
                } else {
                    options.Message = 'You have not graded all questions for this student. Are you sure you want to proceed?';
                    ConfirmSubmitTest(options, function () {
                        self.IsGraded(false);
                        self.SelectedStudentFilter(self.SelectedStudentFilterNew());
                    }, function() {
                        self.IsGraded(true);
                        self.SelectedStudentFilterNew(self.SelectedStudentFilter());
                    });
                }
               
                return true;
            }

            return false;
        },
        CheckRequireSubmitTest: function() {
            var that = this;
            var self = that.options.Self;

            if (that.options.WidgetUtil.IsNullOrEmpty(self.SelectedStudent)) {
                return false;
            }

            if (self.RequireSubmitTest() &&
                self.SelectedStudent != self.NextStudent
                && self.GradingType() == 'student') {
                var options = {
                    Message: '',
                    HideCloseButton: 1
                };

                if (self.SelectedStudent.CanBulkGrading()) {
                    options.Message = 'You have graded all questions but have not submitted the test. Would you like to submit it now?';
                    ConfirmSubmitTest(options, function() {
                        self.IsGraded(false);
                        self.PostSubmitTestData();
                        self.StudentClick(self.NextStudent);
                        self.SelectFilterStudentChangeTrigger();
                        self.SelectedStudentFilterFunction();
                    }, function() {
                        self.IsGraded(false);
                        self.StudentClick(self.NextStudent);
                    });
                } else {
                    options.Message = 'You have not graded all questions for this student. Are you sure you want to proceed?';
                    ConfirmSubmitTest(options, function() {
                        self.IsGraded(false);
                        self.StudentClick(self.NextStudent);
                    }, function() {
                        self.IsGraded(true);
                        self.SelectedStudentID(self.SelectedStudent.StudentID());
                    });
                }
               
                return true;
            }

            return false;
        },
        AllAnswersAreReviewed: function () {
            var that = this;
            var self = that.options.Self;

            if (that.options.WidgetUtil.IsNullOrEmpty(self.TestOnlineSessionAnswers())) return false;

            var unreviewedAnswer = ko.utils.arrayFirst(self.TestOnlineSessionAnswers(), function (testOnlineSessionAnswer) {
                if (!testOnlineSessionAnswer.IsReviewed() && !testOnlineSessionAnswer.AnswerOfBaseQuestion()) return true;
                if (that.options.WidgetUtil.IsNullOrEmpty(testOnlineSessionAnswer.TestOnlineSessionAnswerSubs())) return false;

                var unreviewedAnswerSub = ko.utils.arrayFirst(testOnlineSessionAnswer.TestOnlineSessionAnswerSubs(), function (testOnlineSessionAnswerSub) {
                    return !testOnlineSessionAnswerSub.IsReviewed();
                });

                return !that.options.WidgetUtil.IsNullOrEmpty(unreviewedAnswerSub);
            });

            return that.options.WidgetUtil.IsNullOrEmpty(unreviewedAnswer);
        }
    });
}(jQuery));
