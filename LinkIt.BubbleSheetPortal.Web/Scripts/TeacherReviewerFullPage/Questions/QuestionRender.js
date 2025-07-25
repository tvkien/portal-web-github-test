(function ($) {
    $.widget('jquery.QuestionRender', {
        OpenEndedWidget: null,
        TextEntryWidget: null,
        SimpleChoiceWidget: null,
        MultipleChoiceWidget: null,
        InlineChoiceWidget: null,
        ComplexWidget: null,
        DragDropStandard: null,
        TextHotSpotWidget: null,
        ImageHotSpotWidget: null,
        DragDropSequence: null,
        TableHotspot: null,
        NumberLineHotSpot: null,
        DragDropNumerical: null,
        MultipleChoiceVariableWidget: null,
        DrawingBasicWidget: null,

        options: {
            Util: null,
            Self: null
        },
        Constructor: function (model) {
            var that = this;
            var options = that.options;
            var $body = $('body');
            options.Self = model;

            that.OpenEndedWidget = $body.OpenEnded({ OpenEndedUtil: options.Util, Self: options.Self });
            that.TextEntryWidget = $body.TextEntry({ TextEntryUtil: options.Util, Self: options.Self });
            that.SimpleChoiceWidget = $body.SimpleChoice({ SimpleChoiceUtil: options.Util, Self: options.Self });
            that.MultipleChoiceWidget = $body.MultipleChoice({ MultipleChoiceUtil: options.Util, Self: options.Self });
            that.InlineChoiceWidget = $body.InlineChoice({ InlineChoiceUtil: options.Util, Self: options.Self });
            that.ComplexWidget = $body.ComplexItem({ ComplexItemUtil: options.Util, Self: options.Self });
            that.DragDropStandard = $body.DragDropStandard({ DragDropStandardUtil: options.Util, Self: options.Self });
            that.TextHotSpotWidget = $body.TextHotspot({ TextHotspotUtil: options.Util, Self: options.Self });
            that.ImageHotSpotWidget = $body.ImgHotspot({ ImgHotspotUtil: options.Util, Self: options.Self });
            that.DragDropSequence = $body.DragDropSequence({ DragDropSequenceUtil: options.Util, Self: options.Self });
            that.TableHotspot = $body.TableHotspot({ TableHotspotUtil: options.Util, Self: options.Self });
            that.NumberLineHotSpot = $body.Numberline({ NumberlineUtil: options.Util, Self: options.Self });
            that.DragDropNumerical = $body.DragDropNumerical({ DragDropStandardUtil: options.Util, Self: options.Self });
            that.MultipleChoiceVariableWidget = $body.MultipleChoiceVariable({ MultipleChoiceUtil: options.Util, Self: options.Self });
            that.SimpleChoiceAlgorithmicWidget = $body.SimpleChoiceAlgorithmic({ SimpleChoiceAlgorithmicUtil: options.Util, Self: options.Self });

            that.MultipleChoiceAlgorithmicWidget = $body.MultipleChoiceAlgorithmic({ MultipleChoiceAlgorithmicUtil: options.Util, Self: options.Self });
            that.MultipleChoiceVariableAlgorithmicWidget = $body.MultipleChoiceVariableAlgorithmic({ MultipleChoiceAlgorithmicUtil: options.Util, Self: options.Self });
            that.InlineChoiceAlgorithmicWidget = $body.InlineChoiceAlgorithmic({ InlineChoiceAlgorithmicUtil: options.Util, Self: options.Self });
            that.TextEntryAlgorithmicWidget = $body.TextEntryAlgorithmic({ TextEntryAlgorithmicUtil: options.Util, Self: options.Self });
            that.DragDropStandardAlgorithmicWidget = $body.DragDropStandardAlgorithmic({ DragDropStandardAlgorithmicUtil: options.Util, Self: options.Self });
            that.TextHotSpotAlgorithmicWidget = $body.TextHotspotAlgorithmic({ TextHotspotAlgorithmicUtil: options.Util, Self: options.Self });

            that.DragDropNumericalAlgorithmicWidget = $body.DragDropNumericalAlgorithmic({ DragDropNumericalAlgorithmicUtil: options.Util, Self: options.Self });
            that.DragDropSequenceAlgorithmicWidget = $body.DragDropSequenceAlgorithmic({ DragDropSequenceAlgorithmicUtil: options.Util, Self: options.Self });
            that.ImageHotSpotAlgorithmicWidget = $body.ImgHotspotAlgorithmic({ ImgHotspotAlgorithmicUtil: options.Util, Self: options.Self });
            that.TableHotspotAlgorithmicWidget = $body.TableHotspotAlgorithmic({ TableHotspotAlgorithmicUtil: options.Util, Self: options.Self });
            that.NumberLineHotSpotAlgorithmicWidget = $body.NumberlineAlgorithmic({ NumberlineAlgorithmicUtil: options.Util, Self: options.Self });
            that.DrawingBasicWidget = $body.DrawingBasic({ DrawingBasicUtil: options.Util, Self: options.Self });
        },
        Display: function (question) {
            var that = this;
            var options = that.options;
            var self = options.Self;
            var qtiSchemaId = parseInt(question.QTIItemSchemaID(), 10);
            var isApplyAlgorithmic = question.IsApplyAlgorithmicScoring();

            that._PreProcessQuestion(question);

            if (isApplyAlgorithmic) {
                that._DisplayAlgorithmic(question);
            } else {
                that._Display(question);
            }

            that._PostProcessQuestion(question);
        },
        _PreProcessQuestion: function (question) {
            var that = this;
            var options = that.options;
            var self = options.Self;

            self.SelectedQuestion(question);
            self.RefObjects(question.RefObjects());
            self.PointsPossible(question.PointsPossible());
            self.QTIItemSchemaID(question.QTIItemSchemaID());
            self.AnswerSubID('');
            self.PostAnswerLogs([]);

            var responseProcessingTypeID = '';
            var answerOfStudentForSelectedQuestion = question.Answer();
            if (!options.Util.IsNullOrEmpty(answerOfStudentForSelectedQuestion)) {
                question.PointsEarnedCredit(answerOfStudentForSelectedQuestion.PointsEarned());

                self.PointsEarned(answerOfStudentForSelectedQuestion.PointsEarned());
                self.OldPointsEarned(answerOfStudentForSelectedQuestion.PointsEarned());
                self.AnswerID(answerOfStudentForSelectedQuestion.QTIOnlineTestSessionAnswerID());
                self.AnswerImage(answerOfStudentForSelectedQuestion.AnswerImage());
                self.DrawingContent(answerOfStudentForSelectedQuestion.DrawingContent());
                self.ShowHightLight(answerOfStudentForSelectedQuestion.HighlightQuestion(), question);
                self.ResponseProcessingTypeID(answerOfStudentForSelectedQuestion.ResponseProcessingTypeID());
                self.VisitedTimes(answerOfStudentForSelectedQuestion.VisitedTimes());
                self.TotalSpentTimeOnQuestion(answerOfStudentForSelectedQuestion.TotalSpentTimeOnQuestion());
                self.AnswerAttachments(answerOfStudentForSelectedQuestion.AnswerAttachments());
                self.TeacherFeebackAttachment(answerOfStudentForSelectedQuestion.TeacherFeebackAttachment());

                if (answerOfStudentForSelectedQuestion.VirtualQuestionID() === question.VirtualQuestionID()) {
                    var studentUpdateDate = answerOfStudentForSelectedQuestion.UpdatedDate();

                    if (!options.Util.IsNullOrEmpty(studentUpdateDate)) {
                        studentUpdateDate = displayDateWithFormat(moment.utc(studentUpdateDate).toDate().valueOf(), true);
                    } else {
                        studentUpdateDate = '';
                    }

                    self.UpdatedBy(answerOfStudentForSelectedQuestion.UpdatedBy());
                    self.UpdatedDate(studentUpdateDate);
                    self.Overridden(answerOfStudentForSelectedQuestion.Overridden());
                }
            } else {
                question.XmlContent(question.DataXmlContent());
                question.PointsEarnedCredit('');
                self.PointsEarned(0);
                self.OldPointsEarned(0);
                self.AnswerID(0);
                self.AnswerImage('');
                self.DrawingContent(null);
                self.ShowHightLight('', question);
                self.ResponseProcessingTypeID(0);
                self.VisitedTimes(0);
                self.TotalSpentTimeOnQuestion('0s');
            }
        },
        _PostProcessQuestion: function (question) {
            var that = this;
            var options = that.options;
            var self = options.Self;

            // Reset Feedback After Click Question or Next Student
            //$('.assignment-grading-feedback-audio #audioPlayer').empty();
            self.HasChangedTeacherAttachment(false);
            self.OldFeedbackOverall('');
            self.FeedbackOverall('');
            self.OldFeedbackQuestion('');
            self.FeedbackQuestion('');
            self.FeedbackOverallHistory('');
            self.UpdatedBy('');
            self.UpdatedDate('');
            var studentSelected = self.SelectedStudent;

            if (!options.Util.IsNullOrEmpty(studentSelected)) {
                // Display Overall Feedback
                self.OldFeedbackOverall(studentSelected.FeedbackContent());
                self.FeedbackOverall(studentSelected.FeedbackContent());
                if (!options.Util.IsNullOrEmpty(studentSelected.LastUserUpdatedFeedback()) &&
                    !options.Util.IsNullOrEmpty(studentSelected.LastDateUpdatedFeedback())) {
                    var studentLastDateUpdatedFeedback = studentSelected.LastDateUpdatedFeedback();

                    studentLastDateUpdatedFeedback = displayDateWithFormat(moment.utc(studentLastDateUpdatedFeedback).toDate().valueOf(), true);

                  self.FeedbackOverallHistory('Updated by ' + getFullNameOnly(studentSelected.LastUserUpdatedFeedback()) + ' on ' + studentLastDateUpdatedFeedback);
                }
            } else {
                question.PointsEarnedCredit(null);
            }

            if (!options.Util.IsNullOrEmpty(question.Answer())) {
                var questionAnswer = question.Answer();

                // Display Feedback for Question
                self.OldFeedbackQuestion(questionAnswer.Feedback());
                self.FeedbackQuestion(questionAnswer.Feedback());

                if (!options.Util.IsNullOrEmpty(questionAnswer.TeacherFeebackAttachment())) {
                    var attachment = questionAnswer.TeacherFeebackAttachment();
                    if (attachment && attachment.FileContent) {
                        var playUrl;
                        if(attachment.FileContent instanceof File) {
                            playUrl = URL.createObjectURL(attachment.FileContent)
                        } else {
                            playUrl = this.getObjectURL(attachment.FileContent, attachment.FileType);
                        }
                        var playerOpts = {
                            url: playUrl,
                            options: {
                                onRemoveClick: function () {
                                    var teacherAttachment = questionAnswer.TeacherFeebackAttachment();
                                    teacherAttachment = Object.assign(teacherAttachment, { IsDeleted: true });
                                    questionAnswer.TeacherFeebackAttachment(teacherAttachment);
                                    self.LockRecordFeedbackBtn(false);
                                    self.HasChangedTeacherAttachment(true);
                                    self.AudioPlayerOptions({
                                      url: '', options: { removeable: false }
                                    });
                                }
                            }
                        }
                        self.AudioPlayerOptions(playerOpts);
                        self.LockRecordFeedbackBtn(true);
                    }
                } else {
                    self.AudioPlayerOptions({ url: '', options: { removeable: false } });
                    if(self.LockSaveFeedbackQuestionBtn()) {
                        self.LockRecordFeedbackBtn(true);
                    }
                    else {
                        self.LockRecordFeedbackBtn(false);
                    }
                }
            }
            if (!options.Util.IsNullOrEmpty(self.TestOnlineSessionAnswers())) {
                var testOnlineSessionAnswer = ko.utils.arrayFirst(self.TestOnlineSessionAnswers(), function (item) {
                    return question.VirtualQuestionID() === item.VirtualQuestionID();
                });

                if (!options.Util.IsNullOrEmpty(testOnlineSessionAnswer)) {
                    var updatedDate = testOnlineSessionAnswer.UpdatedDate();

                    if (!options.Util.IsNullOrEmpty(updatedDate)) {
                        updatedDate = displayDateWithFormat(moment.utc(updatedDate).toDate().valueOf(), true);
                    } else {
                        updatedDate = '';
                    }

                    self.UpdatedBy(testOnlineSessionAnswer.UpdatedBy());
                    self.UpdatedDate(updatedDate);
                }
            }
            
            self.SectionInstruction(question.SectionInstruction());

            MathJax.Hub.Queue(["Typeset", MathJax.Hub]);

            that.ScrollToFirstManuallyGradedItem(question);
        },
        getObjectURL: function (fileContent, ext) {
          ext = ext.replace('.', '');
          var bytes = new Uint8Array(fileContent);
          var blob = new Blob([bytes], { type: 'audio/' + ext });
          return window.URL.createObjectURL(blob);
        },
        _DisplayAlgorithmic: function (question) {
            var that = this;
            var qtiSchemaId = parseInt(question.QTIItemSchemaID(), 10);
            var options = that.options;
            var self = options.Self;
            switch (qtiSchemaId) {
                case 1:
                    that.SimpleChoiceAlgorithmicWidget.SimpleChoiceAlgorithmic('Display', question);
                    break;
                case 3:
                    that.MultipleChoiceAlgorithmicWidget.MultipleChoiceAlgorithmic('Display', question);
                    break;
                case 8:
                    that.InlineChoiceAlgorithmicWidget.InlineChoiceAlgorithmic('Display', question);
                    break;
                case 9:
                    that.TextEntryAlgorithmicWidget.TextEntryAlgorithmic('Display', question);
                    break;
                case 10:
                    that.OpenEndedWidget.OpenEnded('Display', question);
                    break;
                case 21:
                    that.ComplexWidget.ComplexItem('Display', question);
                    break;
                case 30:
                    that.DragDropStandardAlgorithmicWidget.DragDropStandardAlgorithmic('Display', question);
                    break;
                case 31:
                    that.TextHotSpotAlgorithmicWidget.TextHotspotAlgorithmic('Display', question);
                    break;
                case 32:
                    that.ImageHotSpotAlgorithmicWidget.ImgHotspotAlgorithmic('Display', question);
                    break;
                case 33:
                    that.TableHotspotAlgorithmicWidget.TableHotspotAlgorithmic('Display', question);
                    break;
                case 34:
                    that.NumberLineHotSpotAlgorithmicWidget.NumberlineAlgorithmic('Display', question);
                    break;
                case 35:
                    that.DragDropNumericalAlgorithmicWidget.DragDropNumericalAlgorithmic('Display', question);
                    break;
                case 36:
                    that.DragDropSequenceAlgorithmicWidget.DragDropSequenceAlgorithmic('Display', question);
                    break;
                case 37:
                    that.MultipleChoiceVariableAlgorithmicWidget.MultipleChoiceVariableAlgorithmic('Display', question);
                    break;
                default:
                    break;
            }
        
        },
        _Display: function (question) {
            var that = this;
            var qtiSchemaId = parseInt(question.QTIItemSchemaID(), 10);

            switch (qtiSchemaId) {
                case 1:
                    that.SimpleChoiceWidget.SimpleChoice('Display', question);
                    break;
                case 3:
                    that.MultipleChoiceWidget.MultipleChoice('Display', question);
                    break;
                case 8:
                    that.InlineChoiceWidget.InlineChoice('Display', question);
                    break;
                case 9:
                    that.TextEntryWidget.TextEntry('Display', question);
                    break;
                case 10:
                    that.OpenEndedWidget.OpenEnded('Display', question);
                    that.DrawingBasicWidget.DrawingBasic('ShowGraph', question);
                    break;
                case 21:
                    that.ComplexWidget.ComplexItem('Display', question);
                    that.DrawingBasicWidget.DrawingBasic('ShowGraph', question);
                    break;
                case 30:
                    that.DragDropStandard.DragDropStandard('Display', question);
                    break;
                case 31:
                    that.TextHotSpotWidget.TextHotspot('Display', question);
                    break;
                case 32:
                    that.ImageHotSpotWidget.ImgHotspot('Display', question);
                    break;
                case 33:
                    that.TableHotspot.TableHotspot('Display', question);
                    break;
                case 34:
                    that.NumberLineHotSpot.Numberline('Display', question);
                    break;
                case 35:
                    that.DragDropNumerical.DragDropNumerical('Display', question);
                    break;
                case 36:
                    that.DragDropSequence.DragDropSequence('Display', question);
                    break;
                case 37:
                    that.MultipleChoiceVariableWidget.MultipleChoiceVariable('Display', question);
                    break;
                default:
                    break;
            }
        },

        ScrollToFirstManuallyGradedItem: function (question) {
            var that = this;
            var options = that.options;
            var self = options.Self;
            var $assignmentQuestion = $('.assignment-desc-question');
            var $manuallyGraded = $assignmentQuestion.find('.is-manually-graded');
            var SCROLL_TIME = 500;
            
            if (question.QTIItemSchemaID() == 21 && $manuallyGraded.length) {
                var $firstManuallyGraded;
                var SCROLL_HEIGHT = 0;

                for (var i = 0, len = $manuallyGraded.length; i < len; i++) {
                    if ($manuallyGraded.eq(i).outerHTML().indexOf('red-border') > 0) {
                        $firstManuallyGraded = $manuallyGraded.eq(i);
                        break;
                    }
                }

                if ($firstManuallyGraded == null) {
                    $firstManuallyGraded = $manuallyGraded.first();
                }

                var offsetFirstManually = $firstManuallyGraded.offset().top;
                var offsetParent = $assignmentQuestion.offset().top;
                
                if (self.IsScrollToFirstManuallyGradedItem()) {
                    if (offsetFirstManually - offsetParent > 0) {
                        SCROLL_HEIGHT = offsetFirstManually - offsetParent;
                    }
                    
                    $assignmentQuestion.animate({
                        scrollTop: SCROLL_HEIGHT
                    }, SCROLL_TIME);
                }
               
                if ($firstManuallyGraded ) {
                    if ($firstManuallyGraded.attr('onclick')) {
                        $firstManuallyGraded.trigger('click')
                    } else {
                        if ($firstManuallyGraded.find('div[onclick]').length) {
                            $firstManuallyGraded.find('div[onclick]').trigger('click');
                        }
                    }
                }
            } else {
                $assignmentQuestion.animate({
                    scrollTop:  0
                }, SCROLL_TIME);
            }
        }
    });
}(jQuery));
