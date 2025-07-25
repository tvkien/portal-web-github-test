(function($) {
    $.widget('jquery.QuestionFilterWidget', {
        options: {
            WidgetUtil: null,
            Self: null
        },
        Constructor: function(self) {
            this.options.Self = self;
        },
        QuestionFilterComputed: function(questions, self) {
            self.AllQuestionFilter().FilterCount(0);
            self.MultipleChoiceQuestionFilter().FilterCount(0);
            self.MultipleChoiceVariableQuestionFilter().FilterCount(0);
            self.TrueFalseQuestionFilter().FilterCount(0);
            self.InlineChoiceQuestionFilter().FilterCount(0);
            self.FillBlankQuestionFilter().FilterCount(0);
            self.ExtendedTextQuestionFilter().FilterCount(0);
            self.DrawingResponseQuestionFilter().FilterCount(0);
            self.ExtendedTextUploadFileQuestionFilter().FilterCount(0);
            self.MultiPartQuestionFilter().FilterCount(0);
            self.DragDropQuestionFilter().FilterCount(0);
            self.TextHotSpotQuestionFilter().FilterCount(0);
            self.ImageHotSpotSelectionQuestionFilter().FilterCount(0);
            self.TableHotSpotSelectionQuestionFilter().FilterCount(0);
            self.NumberLineQuestionFilter().FilterCount(0);
            self.DragDropNumericalQuestionFilter().FilterCount(0);
            self.SequenceOrderQuestionFilter().FilterCount(0);

            ko.utils.arrayForEach(questions, function(question) {
                var questionSchemaId = question.QTIItemSchemaID();
                var questionXmlContent = question.DataXmlContent();
                var $div = $('<div/>');

                $div.html(questionXmlContent);

                var isTrueFalse = $div.find('choiceinteraction[subtype="TrueFalse"]').length;
                var isDrawResponse = $div.find('extendedtextinteraction[drawable="true"]').length;
                var isUploadFile = $div.find('extendedtextinteraction[uploadfile="true"]').length;

                self.AllQuestionFilter().Plus();

                if (questionSchemaId === 1) {
                    if (isTrueFalse) {
                        self.TrueFalseQuestionFilter().Plus();
                    } else {
                        self.MultipleChoiceQuestionFilter().Plus();
                    }
                } else if (questionSchemaId === 8) {
                    self.InlineChoiceQuestionFilter().Plus();
                } else if (questionSchemaId === 9) {
                    self.FillBlankQuestionFilter().Plus();
                } else if (questionSchemaId === 10) {
                    if (isDrawResponse) {
                        self.DrawingResponseQuestionFilter().Plus();
                    } else if (isUploadFile) {
                        self.ExtendedTextUploadFileQuestionFilter().Plus();
                    } else {
                        self.ExtendedTextQuestionFilter().Plus();
                    }
                } else if (questionSchemaId === 21) {
                    self.MultiPartQuestionFilter().Plus();
                } else if (questionSchemaId === 30) {
                    self.DragDropQuestionFilter().Plus();
                } else if (questionSchemaId === 31) {
                    self.TextHotSpotQuestionFilter().Plus();
                } else if (questionSchemaId === 32) {
                    self.ImageHotSpotSelectionQuestionFilter().Plus();
                } else if (questionSchemaId === 33) {
                    self.TableHotSpotSelectionQuestionFilter().Plus();
                } else if (questionSchemaId === 34) {
                    self.NumberLineQuestionFilter().Plus();
                } else if (questionSchemaId === 35) {
                    self.DragDropNumericalQuestionFilter().Plus();
                } else if (questionSchemaId === 36) {
                    self.SequenceOrderQuestionFilter().Plus();
                } else if (questionSchemaId === 37) {
                    self.MultipleChoiceVariableQuestionFilter().Plus();
                }
            });
        },
        FilterQuestions: function(questions, self) {
            var selectedQuestionFilter = self.SelectedQuestionFilter();

            if (selectedQuestionFilter === 'All') {
                ko.utils.arrayForEach(questions, function(question) {
                    question.IsShowByQuestion(true);
                }); 
            } else {
                ko.utils.arrayForEach(questions, function(question) {
                    var questionSchemaId = question.QTIItemSchemaID();
                    var questionXmlContent = question.DataXmlContent();
                    var $div = $('<div/>');

                    $div.html(questionXmlContent);

                    var isTrueFalse = $div.find('choiceinteraction[subtype="TrueFalse"]').length;
                    var isDrawResponse = $div.find('extendedtextinteraction[drawable="true"]').length;
                    var isUploadFile = $div.find('extendedtextinteraction[uploadfile="true"]').length;

                    selectedQuestionFilter = parseInt(selectedQuestionFilter, 10);
                    question.IsShowByQuestion(false);

                    if (selectedQuestionFilter === questionSchemaId) {
                        if (selectedQuestionFilter === 21) {
                            // Check question is multi-part
                            question.IsShowByQuestion(true);
                        } else if (!isTrueFalse && !isDrawResponse && !isUploadFile) {
                            // Other question
                            question.IsShowByQuestion(true);
                        }
                    }

                    // Check question not is multi-part
                    if (questionSchemaId !== 21) {
                        if (selectedQuestionFilter === 2 && isTrueFalse) {
                            // Check question is true/false
                            question.IsShowByQuestion(true);
                        } else if (selectedQuestionFilter === 11 && isDrawResponse) {
                            // Check question is draw response
                            question.IsShowByQuestion(true);
                        } else if (selectedQuestionFilter === 12 && isUploadFile) {
                            // Check question is extended text upload file
                            question.IsShowByQuestion(true);
                        }
                    }
                });
            }
        }
    });
}(jQuery));
