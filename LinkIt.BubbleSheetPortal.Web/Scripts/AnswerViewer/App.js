var AnswerViewerModel = new Vue({
    el: '#app',
    data: {
        question: {
            QTISchemaID: 0,
            XmlContent: '',
            XmlContentCorrect: '',
            PointsEarned: 0,
            PointsPossible: 0,
            CorrectAnswer: '',
            AnswerChoice: '',
            PassageList: [],
            GuidanceRationale: {},
            IsInformationalOnly: false,
            QuestionGroupCommon: questionGroup
        },
        badge: {
            Classes: '',
            Title: '',
            Icon: '',
            IsFullCredit: false
        },
        glossary: {
            text: '',
            content: ''
        },
        isLoadingQuestion: true,
        isLoadingPassage: false,
        isShowModalGlossary: false,
        msgError: '',
        questionGroup: {
          IsShow: false
        }
    },
    created: function () {
        var self = this;

        AnswerViewerService.getAnswerForStudent(self, answerviewerParams).then(function (answerOfStudentData) {
            if (answerOfStudentData.data.IsSucess) {
                question = Utils.deepAssign(self.question, answerOfStudentData.data.TestSessionAnswer);
                question.PassageList = R.map(self.updatePassage, answerOfStudentData.data.PassageList);
                question.IsInformationalOnly = Question.getInformationalOnly(question.XmlContent);
                self.$set('question', question);
                self.displayHtml();
                self.displayBadge();
            } else {
                self.$set('msgError', answerOfStudentData.data.ErrorMessage);
            }

            self.$set('isLoadingQuestion', false);
        });
    },
    computed: {
        isShowPassages: function () {
            return R.filter(R.propEq('IsShow', true))(this.question.PassageList);
        },
        isGuidanceRationale: function () {
            return this.question.GuidanceRationale.guidance || this.question.GuidanceRationale.rationale;
        }
    },
    methods: {
        updatePassage: function (passage) {
            passage.IsActive = false;
            passage.IsShow = false;
            passage.IsExist = false;
            passage.Title = '';

            if (passage.QtiRefObjectID > 0) {
                if (passage.RefNumber > 0) {
                    passage.Title = 'Reference: ' + passage.RefNumber;
                } else {
                    if (!Utils.isNullOrEmpty(passage.Data)) {
                        // Default length to display
                        var displayedLength = 10;

                        if (passage.Data.Length < displayedLength) {
                            displayedLength = passage.Data.Length;
                        }

                        passage.Title = 'Reference: ' + passage.Data.substr(0, displayedLength) + ' ...';
                    } else {
                        passage.Title = 'Reference: ' + passage.QtiRefObjectID;
                    }
                }
            } else {
                var passageUploadType = '';

                if (passage.DataFileUploadPassageID > 0) {
                    if (passage.DataFileUploadTypeID === 2) {
                        passageUploadType = 'Data File';
                    } else if (passage.DataFileUploadTypeID === 3) {
                        passageUploadType = 'Progress';
                    } else if (passage.DataFileUploadTypeID === 4) {
                        passageUploadType = 'Navigate';
                    }

                    passage.Title = passageUploadType + ' Reference Number: ' + passage.RefNumber;
                } else {
                    if (passage.Qti3pSourceID === 1) {
                        passageUploadType = 'Navigate';
                    } else if (passage.Qti3pSourceID === 4) {
                        passageUploadType = 'Progress';
                    }

                    passage.Title = passageUploadType + ' Reference : ' + passage.RefNumber;
                }
            }

            return passage;
        },
        hidePassage: function (passage) {
            passage.IsShow = false;
            passage.IsActive = false;
        },
        showQuestionGroup: function() {
          this.questionGroup.IsShow = true;
        },
        hideQuestionGroup: function() {
          this.questionGroup.IsShow = false;
        },
        showPassage: function (passage) {
            var self = this;

            passage.IsActive = true;

            // Check if passage exist with data
            if (passage.IsExist) {
                passage.IsShow = true;
                return;
            }

            var passageParams = {
                refObjectID: passage.QtiRefObjectID,
                data: encodeURI(passage.Data),
                qti3pPassageID: passage.Qti3pPassageID,
                qti3pSourceID: passage.Qti3pSource,
                dataFileUploadPassageID: passage.DataFileUploadPassageID,
                dataFileUploadTypeID: passage.DataFileUploadTypeID
            };

            self.isLoadingPassage = true;

            AnswerViewerService
                .getPassage(self, passageParams)
                .then(function (response) {
                    self.isLoadingPassage = false;

                    if (response.data === '') {
                        response.data = 'Passage is not found.';
                    }

                    passage.IsExist = true;
                    passage.IsShow = true;
                    passage.Data = response.data;
                });
        },
        displayHtml: function () {
            var self = this;
            var schemaIdItemTypes = [9, 30, 31, 32, 33, 34, 35, 36];
            var originalXmlContent = '';

            originalXmlContent = self.question.XmlContent;
            originalXmlContent = Question.displayHtml(originalXmlContent);

            self.$set('question.XmlContent', originalXmlContent);
            self.$set('question.XmlContent', Question.updateAnswerHtml(self.question, originalXmlContent));
            self.$set('question.XmlContent', Question.displayIconGuidanceRationale(self.question.XmlContent));

            // Only show correct question for schemaId above
            if (R.indexOf(self.question.QTISchemaID, schemaIdItemTypes) !== -1) {
                if (self.question.QTISchemaID === 9) {
                    self.$set('question.XmlContentCorrect', TextEntry.displayByAnswerSingle(self.question));
                } else if (self.question.QTISchemaID === 35) {
                    self.$set('question.XmlContentCorrect', DragDropNumerical.updateAnswerHtml(self.question));
                } else {
                    self.$set('question.XmlContentCorrect', Question.updateAnswerHtml(self.question, originalXmlContent, true));
                }
            }

            self.$nextTick(function () {
                self.displayOtherHtml();
            });
        },
        displayOtherHtml: function () {
            var self = this;
            var $document = $(document);

            // Loading MathJax
            MathJax.Hub.Queue(['Typeset', MathJax.Hub]);

            // Get list guidance/rationale
            self.$set('question.GuidanceRationale', GuidanceRationale.getListGuidanceRationale(self.question.XmlContent));

            // Show guidance/rationale
            $document.on('mouseover', '.icon-tooltip', GuidanceRationale.showGuidanceRationale);
            $document.on('click', '#ShowTeacherRationale', GuidanceRationale.handleClickRationale);
            $document.on('click', '#ShowStudentGuidance', GuidanceRationale.handleClickGuidance);

            $document.on({
                'mouseenter': Glossary.handleMouseEnter,
                'mouseleave': Glossary.handleMouseLeave,
                'click': function () {
                    var $glossary = $(this);
                    var glossaryText = $glossary.html();
                    var glossaryContent = $glossary.attr('glossary');

                    self.$set('isShowModalGlossary', true);
                    self.$set('glossary.text', glossaryText);
                    self.$set('glossary.content', glossaryContent);
                }
            }, 'span.glossary');
            $document.find('span[class="math-tex"]').each(function (index, value) {
                var span = $(value);
                span.css('display', 'inline-block');
            });
            DragDropStandard.displayLineMatching();
        },
        displayBadge: function () {
            var self = this;
            var schemaIdItemTypes = [1, 3, 8, 9, 10, 21, 35, 36];
            var isItemTypeAllow = R.indexOf(self.question.QTISchemaID, schemaIdItemTypes) !== -1;

            if (isItemTypeAllow) {
                self.$set('badge.Icon', 'app-badge-icon-hide');
            }

            if (self.question.PointsEarned === 0) {
                if (isItemTypeAllow) {
                    self.$set('badge.Title', 'Incorrect');
                } else {
                    self.$set('badge.Title', 'Incorrect');
                    self.$set('badge.Icon', 'app-badge-icon-zero');
                }
            } else if (self.question.PointsEarned < self.question.PointsPossible) {
                if (isItemTypeAllow) {
                    self.$set('badge.Title', 'Incorrect');
                } else {
                    self.$set('badge.Title', 'Partially Correct');
                    self.$set('badge.Icon', 'app-badge-icon-partial');
                }
            } else {
                if (isItemTypeAllow) {
                    self.$set('badge.Title', 'Correct');
                } else {
                    self.$set('badge.Title', 'Correct');
                    self.$set('badge.Icon', 'app-badge-icon-full');
                }

                self.$set('badge.IsFullCredit', true);
                self.$set('question.XmlContentCorrect', '');
            }
        }
  }
});
