var HEIGHT_HS = 405;
var HEIGHT_FULLSCREEN = 265;
var COLSPAN_NESTED = 4;

var BubbleSheetClassReviewIcons = {
    fullscreen: '<svg class="icon" xmlns="http://www.w3.org/2000/svg" width="16" height="20" viewBox="0 0 100 125"><path d="M26.947 86.098H5V64.151h6.997v14.95h14.95zM11.997 35.849H5V13.902h21.947v6.997h-14.95zM95 35.849h-6.997v-14.95h-14.95v-6.997H95zM95 86.098H73.053v-6.997h14.95v-14.95H95z"/><g><path d="M82.405 73.503h-64.81V26.498h64.81v47.005zm-57.812-6.998h50.815v-33.01H24.593v33.01z"/></g></svg>',
    exitfullscreen: '<svg class="icon" xmlns="http://www.w3.org/2000/svg" width="16" height="20" viewBox="0 0 100 125"><path d="M0 61.836h28.273v28.273h-7.774V69.611H0zM20.499 9.891h7.774v28.273H0v-7.775h20.499zM71.727 9.891h7.775v20.498H100v7.775H71.727zM71.727 61.836H100v7.775H79.502v20.498h-7.775z"/></svg>'
};

var BubbleSheetClassReview = new Vue({
    el: '[vue="bubbleSheetClassReview"]',
    data: {
        refreshedFirst: false,
        refreshedAt: 0,
        isShowGradingShortcuts: false,
        isShowBubbleSheetImage: false,
        isFullscreen: false,
        fullscreenStatus: {
            icon: BubbleSheetClassReviewIcons.fullscreen,
            text: 'Fullscreen'
        },
        studentStatus: [
            { text: 'Finished', value: 'finished', background: '#00b050' },
            { text: 'Review', value: 'review', background: '#dcdcdc' },
            { text: 'Not Graded', value: 'not-graded', background: '#ff0000' }
        ],
        questionStatus: [
            { text: 'Missing', background: '#ff0000' },
            { text: 'Multi-mark', background: '#ed7d31' },
            { text: 'Incomplete', background: '#ffff00' },
            { text: 'Open Ended', background: '#9dc3e6' }
        ],
        filterStatus: {
            finishedStatus: ['Absent', 'Confirmed', 'Complete', 'Auto'],
            reviewStatus: ['Blank', 'Multi-Mark', 'Incomplete', 'Manual', 'Invalid'],
            notGradingStatus: ['Missing', 'Unavailable']
        },
        gradingShortcutsStatus: [
            { text: 'Apply Zero Points', value: 'apply-zero-points' },
            { text: 'Apply Max Points', value: 'apply-max-points' }
        ],
        classView: {
            hot: {},
            actualData: [],
            data: [],
            actualStudentsData: '',
            students: [],
            settings: {
                nestedHeaders: [],
                colHeaders: ['', 'Student', 'Points Earned', 'View'],
                columns: [
                    { data: 'student.IsChecked', type: 'checkbox' },
                    { data: 'student.StudentName', type: 'text', renderer: rendererStatusStudent },
                    { data: 'student.PointsEarned', type: 'text' },
                    { data: 'student.ImageUrl', type: 'text', renderer: rendererImage, editor: false }
                ]
            }
        },
        bubbleSheetImageSelected: [],
        filterStudentSelected: [],
        applyPointsSelected: '',
        applyCorrectAnswersSelected: '',
        isShowConfirmSubmit: false,
        isShowModalWarning: false,
        isSelectedAllClassView: false,
        isHiddenAllStudents: false,
        isAutoClassView: false,
        isAutoMultiCells: false,
        isHasSelectedFinished: false,
        notReviewedStudentCount: 0,
        intervalAutoSaved: 0,
        isSubmiting: false
    },
    computed: {
        isShowMarkAsAbsent: function () {
            var checkedStudents = this.selectedCheckedStudents;

            if (checkedStudents.length) {
                var filterAbsentStudent = function (stu) {
                    var stuStatus = stu.student.Status;
                    return stuStatus.indexOf('Missing') > -1 || stuStatus.indexOf('Manual') > -1;
                };

                var absentStudents = checkedStudents.filter(filterAbsentStudent);

                if (absentStudents.length === checkedStudents.length) {
                    return true;
                }
            }

            return false;
        },
        isDisabledGradingShortcuts: function () {
            var checkedStudents = this.selectedCheckedStudents;

            if (checkedStudents.length === this.classView.students.length) {
                this.isSelectedAllClassView = true;
            } else {
                this.isSelectedAllClassView = false;
            }

            if (checkedStudents.length) {
                return false;
            }

            return true;
        },
        selectedCheckedStudents: function () {
            var filterCheckedStudent = function (stu) {
                return stu.student.IsChecked;
            };

            var checkedStudents = this.classView.students.filter(filterCheckedStudent);

            return checkedStudents;
        },
        changedStudent: function () {
            // Get changed data, compare with actualdata
            var vm = this;
            var changedData = [];
            vm.classView.students.forEach(function (student, i) {
                var classViewAnswersData = vm.classView.actualData.BubbleSheetStudentDatas[i].BubbleSheetAnswers;

                for (var j = 0; j < classViewAnswersData.length; j++) {
                    var classViewAnswersLetter = classViewAnswersData[j].AnswerLetter;
                    var key = 'question_' + (j + 1);
                    var studentAnswerLetter = student.questions[key].answerLetter;

                    classViewAnswersLetter = classViewAnswersLetter == null ? '' : classViewAnswersLetter;
                    studentAnswerLetter = studentAnswerLetter == null ? '' : studentAnswerLetter;

                    if (classViewAnswersLetter != studentAnswerLetter) {
                        changedData.push(student);
                        break;
                    }
                }
            });

            return changedData;
        },
        isDisabledSubmit: function () {
            return this.changedStudent.length === 0;
        },
        isDisabledUndoAndSave: function () {
            return this.changedStudent.length === 0;
        }
    },
    watch: {
        filterStudentSelected: function () {
            var vm = this;
            var hotClassView = vm.classView.hot.getInstance();
            var filterStatus = vm.getFilterStatus(vm.filterStudentSelected);
            var hiddenRows = vm.getHiddenRows(vm.classView.students);
            var showRows = vm.getShowRows(vm.classView.students, filterStatus);

            hiddenRows = hiddenRows.filter(function (item) {
                return !showRows.includes(item);
            });

            if (hiddenRows.length) {
                vm.classView.students.forEach(function (student, studentIndex) {
                    if (hiddenRows.indexOf(studentIndex) > -1 && student.student.IsChecked) {
                        student.student.IsChecked = false;
                    }
                });
            }

            if (hiddenRows.length === vm.classView.students.length) {
                vm.isHiddenAllStudents = true;
            } else {
                vm.isHiddenAllStudents = false;
            }

            hotClassView.updateSettings({
                hiddenRows: {
                    rows: hiddenRows
                },
                showRows: {
                    rows: showRows
                }
            });
        },
        changedStudent: {
            handler: function () {
                var vm = this;
                var classViewData = vm.classView.data;

                vm.classView.students.forEach(function (student, i) {
                    var classViewAnswersData = classViewData.BubbleSheetStudentDatas[i].BubbleSheetAnswers;
                   
                    classViewAnswersData.forEach(function (item, j) {
                        Object.keys(student.questions).forEach(function (key, value) {
                            if (j === value) {
                                var originalValue = item['AnswerLetter'];
                                var newValue = student.questions[key].answerLetter;

                                if (originalValue != newValue) {
                                    item['AnswerLetter'] = newValue;
                                }
                            }
                        });
                    });
                });
            },
            deep: true
        }
    },
    ready: function () {
        var vm = this;
        this.$options.lastTooltipStudent = null;        
        this.refreshedAt = this.refreshedDatetime();

        $(document.body)
            .on('mouseenter', 'td.htStudent', this.showTooltipStudent)
            .on('mouseleave', 'td.htStudent', this.hideTooltipStudent);

        window.addEventListener('resize', this.resizeHandsontable);

        $('.bubbleSheetClassReviewPage.is-first').click(function (e) {
            if (vm.changedStudent.length > 0) {
                e.preventDefault();
                vm.isShowModalWarning = true;
            }
        });
    },
    methods: {
        toggleGradingShortcutsModal: function () {
            this.isShowGradingShortcuts = !this.isShowGradingShortcuts;
            this.applyPointsSelected = '';
            this.applyCorrectAnswersSelected = '';
        },
        toggleBubbleSheetImageModal: function () {
            this.isShowBubbleSheetImage = !this.isShowBubbleSheetImage;
        },
        toggleFullScreen: function () {
            var elBody = document.querySelector('body');
            var hotClassView = this.classView.hot.getInstance();
            var classViewHeight = 0;

            this.isFullscreen = !this.isFullscreen;

            classViewHeight = this.isFullscreen ? (window.innerHeight - HEIGHT_FULLSCREEN) : HEIGHT_HS;

            if (this.isFullscreen) {
                elBody.classList.add('bubbleSheetClassReviewFullscreen')

                this.fullscreenStatus = {
                    icon: BubbleSheetClassReviewIcons.exitfullscreen,
                    text: 'Exit fullscreen'
                };
            } else {
                elBody.classList.remove('bubbleSheetClassReviewFullscreen');

                this.fullscreenStatus = {
                    icon: BubbleSheetClassReviewIcons.fullscreen,
                    text: 'Fullscreen'
                };
            }

            hotClassView.updateSettings({
                width: '100%',
                height: classViewHeight
            });
        },
        getClassViewData: function () {
            var vm = this;
            var classViewData = vm.classView.data;

            vm.classView.settings.nestedHeaders.push({
                label: '',
                colspan: COLSPAN_NESTED
            });

            classViewData.BubbleSheetStudentDatas.forEach(function (item, i) {
                var studentData = {
                    IsChecked: false,
                    StudentName: item.Name,
                    PointsEarned: item.PointsEarned,
                    StudentId: item.StudentId,
                    Status: item.Status,
                    Graded: item.Graded,
                    BubbleSheetId: item.BubbleSheetId,
                    ImageData: item.BubbleSheetFileViewModel
                };

                var jsonData = { student: studentData };

                var classViewAnswersData = item.BubbleSheetAnswers;

                if (classViewAnswersData.length && i === 0) {
                    vm.classView.settings.nestedHeaders.push({
                        label: 'Questions',
                        colspan: classViewAnswersData.length
                    });
                }

                var questions = {};
                classViewAnswersData.forEach(function (question) {
                    var isOpenEnded = question.QTISchemaId == 10;
                    var answerIdentifiersOpenEnded = '';

                    if (i === 0) {
                        var colName = 'questions.question_' + question.QuestionOrder + '.answerLetter';
                        var colTitle = '';

                        if (isOpenEnded) {
                            //var answerIdentifiersOpenEnded = Array.from(Array(question.PointsPossible + 1).keys());
                            //answerIdentifiersOpenEnded = answerIdentifiersOpenEnded.join(';');
                            //colTitle = answerIdentifiersOpenEnded;
                            colTitle = 'Max points: ' + question.PointsPossible;
                        } else {
                            colTitle = question.AnswerIdentifiers;
                        }

                        colTitle = colTitle.replace(/;/g, ',');

                        vm.classView.settings.colHeaders.push('<span data-title="' + colTitle + '">' + question.QuestionOrder + '</span>');

                        var colData = {
                            data: colName,
                            type: 'text',
                            renderer: rendererStatusColor,
                            validator: validatorAlphanumeric,
                            allowInvalid: false
                        };

                        vm.classView.settings.columns.push(colData);
                    }

                    var questionName = 'question_' + question.QuestionOrder;
                    var questionData = {};
                    var correctAnswser = question.CorrectAnswer.split(',').sort(function(a, b) { return a > b });
                    questionData['answerLetter'] = question.AnswerLetter;
                    questionData['questionId'] = question.VirtualQuestionId;
                    questionData['status'] = question.Status;
                    questionData['pointPossible'] = question.PointsPossible;
                    questionData['wasAnswered'] = question.WasAnswered;
                    questionData['correctAnswer'] = correctAnswser.toString();
                    questionData['qtiSchemaId'] = question.QTISchemaId;
                    questionData['answerIdentifiers'] = question.AnswerIdentifiers;

                    questions[questionName] = questionData;
                });


                jsonData['questions'] = questions;
                vm.classView.students.push(jsonData);
            });

            // Save actualData           
            vm.classView.actualStudentsData = JSON.stringify(vm.classView.students);

            vm.initializeHansontable();
        },
        undoClassView: function () {
            this.classView.students = JSON.parse(this.classView.actualStudentsData);
            this.classView.hot.loadData(this.classView.students);

            var params = {
                ticket: this.classView.data.Ticket,
                classId: this.classView.data.ClassId
            };

            BubbleSheetClassReviewService.deleteAutoSavedData(params).done(function (response) {
                if (!response.IsSuccess) {
                    console.log('Delete fail.');
                }
            }).error(function (e) { });
        },
        autoSavedClassView: function (isClick) {
            var $bubbleSheetClassReviewBorder = $('#BubbleSheetClassReviewBorder');
            var vm = this;
            if (vm.isSubmiting) {
                return;
            }
            var classViewData = vm.classView.data;

            var params = {
                ticket: classViewData.Ticket,
                classId: classViewData.ClassId,
                bubbleSheetStudentDatas: JSON.stringify(classViewData.BubbleSheetStudentDatas),
                actualData: JSON.stringify(vm.classView.actualData)
            };

            if (isClick) {
                ShowBlock($bubbleSheetClassReviewBorder, 'Saving');
            }

            if (isClick || vm.changedStudent.length > 0) {                
                BubbleSheetClassReviewService.autoSaved(params).done(function(response) {
                    if (isClick) {
                        $bubbleSheetClassReviewBorder.unblock();
                    }
                }).error(function(e) {
                    $bubbleSheetClassReviewBorder.unblock();
                });
            }
        },
        confirmSubmitClassView: function () {
            var count = 0;
            this.classView.students.forEach(function (item) {
                if(item.student 
                    && (item.student.Status.indexOf('Confirmed') > -1 
                            || item.student.Status === 'Absent'
                            || item.student.Status === 'Auto')) {
                    count++;
                }
            });
            var isNotReview = this.classView.students.length - count - this.changedStudent.length;
            this.notReviewedStudentCount = isNotReview > 0 ? isNotReview : 0;
            this.isShowConfirmSubmit = true;
        },
        submitClassView: function () {
            var $bubbleSheetClassReviewBorder = $('#BubbleSheetClassReviewBorder');
            var vm = this;
            vm.isShowConfirmSubmit = false;
            var classViewDataOriginal = JSON.stringify(vm.classView.data);
            var classViewData = vm.classView.data;
            var changedStudent = JSON.stringify(vm.changedStudent);

            if (vm.changedStudent.length) {
                ShowBlock($bubbleSheetClassReviewBorder, 'Submitting');
                vm.isSubmiting = true;
                var data = [];
                classViewData.BubbleSheetStudentDatas.forEach(function (item) {
                    vm.changedStudent.forEach(function (student) {
                        if (item.StudentId === student.student.StudentId) {
                            data.push(item);
                        }
                    });
                });

                data.forEach(function (item) {
                    delete item.Graded;
                    delete item.IsChanged;
                    delete item.Name;
                    delete item.PointsEarned;
                    delete item.Status;
                    item.BubbleSheetAnswers.forEach(function (a) {
                        delete a.AnswerIdentifiers;
                        delete a.CorrectAnswer;
                        delete a.MaxChoice;
                        delete a.PointsPossible;
                        delete a.QTISchemaId;
                        delete a.Status;
                        delete a.WasAnswered
                    });

                });

                var postData = classViewData;
                postData.BubbleSheetStudentDatas = data;
                var params = { model: postData };
                vm.classView.data = JSON.parse(classViewDataOriginal);
                BubbleSheetClassReviewService.submitClassView(params).done(function (response) {
                    if (response) {
                        vm.setLockedStudentReviewed(changedStudent);
                        $bubbleSheetClassReviewBorder.unblock();
                        // Set actualData again
                        vm.classView.actualData = JSON.parse(classViewDataOriginal);
                        vm.classView.actualStudentsData = JSON.stringify(vm.classView.students);
                    }
                    vm.isSubmiting = false;
                }).error(function (e) {
                    $bubbleSheetClassReviewBorder.unblock();
                    vm.isShowConfirmSubmit = false;
                    vm.isSubmiting = false;
                });               
            }
        },
        setLockedStudentReviewed: function (changedStudent) {
            var vm = this;
            var lockStudent = JSON.parse(changedStudent);

            lockStudent.forEach(function (stu) {
                var row = vm.classView.students.findIndex(function (student) {
                    return stu.student.StudentId === student.student.StudentId;
                });
                var len = vm.classView.settings.colHeaders.length;
                for (var j = 4; j < len; j++) {
                    var meta = vm.classView.hot.getCellMeta(row, j);
                    meta.readOnly = true;
                }
            });

            vm.classView.hot.render();
        },
        closeConfirmSubmit: function () {
            this.isShowConfirmSubmit = false;
        },
        initializeHansontable: function () {
            var vm = this;
            var hotClassView = vm.$els.bubbleSheetClassReview;

            var hotClassViewBeforeChange = function (changes, source) {
                var i = 0;
                var len = changes.length;
                var isEditing = false;
                var objEditing = {
                    value: '',
                    key: ''
                };

                if (source === 'loadData' || len > 1) {
                    return;
                }

                if (source === 'edit' && len > 0) {
                    for (i = 0; i < len; i++) {
                        if (changes[i][3] != null) {
                            if(typeof changes[i][3] !== 'boolean'){
                                changes[i][3] = changes[i][3].toUpperCase();
                            }
                            
                            isEditing = true;
                            objEditing.value = changes[i][3];
                            objEditing.key = changes[0][1].split('.')[1];
                            break;
                        }
                    }

                    if (isEditing && vm.isAutoMultiCells) {
                        vm.classView.students.forEach(function (stu) {
                            if (stu.student.IsChecked) {
                                stu.questions[objEditing.key].answerLetter = objEditing.value;
                            }
                        });
                    }
                }
            };

            var hotClassViewAfterSelection = function (r, c, r2, c2, preventScrolling, selectionLayerLevel) {
                if (vm.isAutoClassView) {
                    var student = vm.classView.students[r].student;
                    var fileList = student.ImageData.ListFileSubViewModels;

                    if (c === 3) {    
                        vm.bubbleSheetImageSelected = [];
    
                        fileList.forEach(function (item) {
                            if (!!item.ImageUrl) {
                                vm.bubbleSheetImageSelected.push(item.ImageUrl);
                            } else {
                                vm.bubbleSheetImageSelected.push('broken.png');
                            }
                        });
    
                        vm.isShowBubbleSheetImage = true;
                    } else {
                        if (vm.isShowBubbleSheetImage) {
                            vm.bubbleSheetImageSelected = [];
    
                            fileList.forEach(function (item) {
                                if (!!item.ImageUrl) {
                                    vm.bubbleSheetImageSelected.push(item.ImageUrl);
                                } else {
                                    vm.bubbleSheetImageSelected.push('broken.png');
                                }
                            });
                        }
                    }
                }
            };

            var hotClassViewScroll = function () {
                var $bubbleSheetClassReview = $('#BubbleSheetClassReview');
                $bubbleSheetClassReview.find('td').qtip('destroy', true);
                $bubbleSheetClassReview.find('td[data-invalid]').qtip({
                    overwrite: false,
                    content: {
                        attr: 'data-invalid'
                    },
                    position: {
                        my: 'bottom center',
                        at: 'top center'
                    },
                    show: {
                        event: 'mouseover'
                    },
                    hide: {
                        event: 'mouseout'
                    },
                    style: {
                        classes: 'qtip-red'
                    }
                });
            };

            var hotClassViewAfterChange = function (changes, source) {
                if (source === 'edit' && changes.length) {
                    hotClassViewScroll();
                }
            };

            var hotClassViewCells = function (row, column) {
                var cellMeta = {};

                if (column === 0) {
                    cellMeta.className = 'htCenter';
                }

                if (column === 1) {
                    cellMeta.readOnly = true;
                    cellMeta.className = 'htStudent';
                }

                if (column === 2) {
                    cellMeta.readOnly = true;
                    cellMeta.className = 'htCenter';
                }

                return cellMeta;
            };

            var hotClassViewBeforeKeyDown = function (e) {
                if (!this.getSelected()) {
                    return;
                }

                var row = this.getSelected()[0];
                var col = this.getSelected()[1];

                if (e.key === 'Tab' && col === this.countCols() - 1) {
                    this.selectCell(row + 1, 3);
                }
            };

            var hotClassViewSettings = {
                data: vm.classView.students,
                observeChanges: true,
                nestedHeaders: [
                    vm.classView.settings.nestedHeaders,
                    vm.classView.settings.colHeaders
                ],
                columns: vm.classView.settings.columns,
                manualColumnResize: [50, 140, 100, 50],
                fixedRowsTop: 0,
                fixedColumnsLeft: COLSPAN_NESTED,
                currentRowClassName: 'currentRow',
                minSpareRows: 0,
                height: HEIGHT_HS,
                formulas: true,
                manualColumnFreeze: true,
                contextMenu: false,
                fillHandle: {
                    autoInsertRow: false,
                    direction: 'vertical'
                },
                beforeChange: hotClassViewBeforeChange,
                afterChange: hotClassViewAfterChange,
                afterSelection: hotClassViewAfterSelection,
                afterScrollHorizontally: hotClassViewScroll,
                afterScrollVertically: hotClassViewScroll,
                cells: hotClassViewCells,
                licenseKey: 'a70f6-b55ab-a3862-0471e-e915a'
            };

            vm.classView.hot = new Handsontable(hotClassView, hotClassViewSettings);

            vm.classView.hot.updateSettings({
                beforeKeyDown: hotClassViewBeforeKeyDown
            });

            if (!vm.refreshedFirst) {
                vm.filterStudentSelected = ['review', 'not-graded'];
                vm.refreshedFirst = true;
            }            
        },
        resizeHandsontable: function () {
            var hotClassView = this.classView.hot.getInstance();
            var classViewHeight = this.isFullscreen ? (window.innerHeight - HEIGHT_FULLSCREEN) : HEIGHT_HS;

            hotClassView.updateSettings({
                width: '100%',
                height: classViewHeight
            });
        },
        getTooltipStudent: function (student) {
            var studentTooltip = '';
            var studentName = student.StudentName;
            var studentStatus = student.Status;
            var studentProcessed = student.Graded;

            studentTooltip += '<div>';
            studentTooltip += '<strong>' + studentName + '</strong>';
            studentTooltip += '<div>- Student Status: ' + studentStatus + '</div>';
            studentTooltip += '<div>- Items Processed: ' + studentProcessed + '</div>';
            studentTooltip += '</div>';

            return studentTooltip;
        },
        showTooltipStudent: function (event) {
            var $target = $(event.target);
            var targetIndex = $target.parent().index();
            var student = this.classView.students[targetIndex].student;
            var tartgetHtml = this.getTooltipStudent(student);

            this.hideTooltipStudent();

            $target.qtip({
                overwrite: false,
                content: tartgetHtml,
                position: {
                    my: 'bottom center',
                    at: 'top center',
                    viewport: $(window)
                },
                show: {
                    event: 'click'
                },
                hide: false,
                style: {
                    classes: 'qtip-yellow'
                }
            });

            this.$options.lastTooltipStudent = $target;
        },
        hideTooltipStudent: function () {
            if (this.$options.lastTooltipStudent) {
                this.$options.lastTooltipStudent.qtip('destroy');
            }
        },
        getFilterStatus: function (filterStudent) {
            var filterStatus = [];

            if (!filterStudent.length) {
                return filterStatus;
            }

            if (filterStudent.indexOf('finished') > -1) {
                filterStatus = filterStatus.concat(this.filterStatus.finishedStatus);
                this.isHasSelectedFinished = true;
            } else {
                this.isHasSelectedFinished = false;
            }

            if (filterStudent.indexOf('review') > -1) {
                filterStatus = filterStatus.concat(this.filterStatus.reviewStatus);
            }

            if (filterStudent.indexOf('not-graded') > -1) {
                filterStatus = filterStatus.concat(this.filterStatus.notGradingStatus);
            }

            return filterStatus;
        },
        getHiddenRows: function (students) {
            var rows = [];

            students.forEach(function (student, i) {
                rows.push(i);
            });

            return rows;
        },
        getShowRows: function (students, filterStatus) {
            var vm = this;
            var rows = [];

            students.forEach(function (student, i) {
                var studentStatus = student.student.Status;
                studentStatus = studentStatus.split('|');

                for (var j = 0; j < studentStatus.length; j++) {
                    var studentStatusItem = studentStatus[j];

                    if (vm.isHasSelectedFinished) {
                        if (filterStatus.indexOf(studentStatusItem) > -1 && vm.filterStudentSelected.includes('review')) {
                            rows.push(i);
                            break;
                        } else {
                            if (filterStatus.indexOf(studentStatusItem) > -1 && !studentStatus.includes('Multi-Mark')) {
                                rows.push(i);
                                break;
                            }
                        }
                    } else {
                        if (filterStatus.indexOf(studentStatusItem) > -1 && !studentStatus.includes('Confirmed')) {
                            rows.push(i);
                            break;
                        }
                    }
                }
            });

            return rows;
        },
        refreshedDatetime: function () {
            var result;
            var now = new Date();
            var hours = now.getHours();
            var minutes = BubbleSheetClassReviewUtils.getZeroPad(now.getMinutes());
            var hourtime = BubbleSheetClassReviewUtils.getHourTime(hours);
            hours = hours % 12 || 12;

            result = hours + ':' + minutes + ' ' + hourtime;

            return result;
        },
        refreshedStudentDetails: function () {
            var vm = this;
            vm.refreshedAt = vm.refreshedDatetime();
            var $bubbleSheetClassReviewBorder = $('#BubbleSheetClassReviewBorder');
            ShowBlock($bubbleSheetClassReviewBorder, 'Loading');
            var params = {
                id: vm.classView.data.Ticket,
                classId: vm.classView.data.ClassId
            };
            var len = vm.classView.settings.colHeaders.length;
            BubbleSheetClassReviewService.refreshedStudentDetails(params).done(function (response) {
                vm.classView.students.forEach(function (stu, i) {
                    var data = response.filter(function (st) {
                        return st.StudentId === stu.student.StudentId;
                    });

                    stu.student.Status = data[0].Status;
                    stu.student.Graded = data[0].Graded;
                    stu.student.PointsEarned = data[0].PointsEarned;
                    stu.student.ImageData.BubbleSheetFileId = data[0].BubbleSheetFileViewModel.BubbleSheetFileId;
                    stu.student.ImageData.ListFileSubViewModels = data[0].BubbleSheetFileViewModel.ListFileSubViewModels;

                    //update data for originaldata
                    vm.classView.data.BubbleSheetStudentDatas.forEach(function (item, i) {
                        if (item.StudentId === stu.student.StudentId) {
                            item.Status = data[0].Status;
                            item.Graded = data[0].Graded;
                            item.PointsEarned = data[0].PointsEarned;
                            item.BubbleSheetFileViewModel.BubbleSheetFileId = data[0].BubbleSheetFileViewModel.BubbleSheetFileId;
                            item.BubbleSheetFileViewModel.ListFileSubViewModels = data[0].BubbleSheetFileViewModel.ListFileSubViewModels;
                        }
                    });
                    
                    var temp = stu.student.Status.split('|');

                    if (temp.includes(vm.filterStatus.finishedStatus)) {
                        for (var j = 4; j < len; j++) {
                            var meta = vm.classView.hot.getCellMeta(i, j);
                            meta.readOnly = false;
                        }
                    }
                });

                vm.classView.hot.loadData(vm.classView.students);
                $bubbleSheetClassReviewBorder.unblock();
                vm.classView.actualStudentsData = JSON.stringify(vm.classView.students);
            });
        },
        applyGradingShortcuts: function () {
            var checkedStudents = this.selectedCheckedStudents;

            this.isShowGradingShortcuts = false;

            if (this.applyPointsSelected === 'apply-zero-points') {
                this.applyPointsGradingShortcuts(checkedStudents, 'apply-zero-points');
            }

            if (this.applyPointsSelected === 'apply-max-points') {
                this.applyPointsGradingShortcuts(checkedStudents, 'apply-max-points');
            }

            if (this.applyCorrectAnswersSelected) {
                this.applyCorrectAnswersGradingShortcuts(checkedStudents);
            }
        },
        applyPointsGradingShortcuts: function (students, type) {
            students.forEach(function (stu) {
                Object.keys(stu.questions).forEach(function (key, value) {
                    var question = stu.questions[key];

                    if (question.qtiSchemaId === 10 && !question.wasAnswered) {
                        if (type === 'apply-zero-points') {
                            question.answerLetter = '0';
                        }

                        if (type === 'apply-max-points') {
                            question.answerLetter = question.pointPossible;
                        }
                    }
                });
            });
        },
        applyCorrectAnswersGradingShortcuts: function (students) {
            students.forEach(function (stu, i) {
                Object.keys(stu.questions).forEach(function (key, value) {
                    var question = stu.questions[key];
                    if (question.qtiSchemaId != 10 && !question.wasAnswered) {
                        question.answerLetter = question.correctAnswer;
                    }
                });
            });
        },
        markAsAbsent: function () {
            var vm = this;
            var $bubbleSheetClassReviewBorder = $('#BubbleSheetClassReviewBorder');
            ShowBlock($bubbleSheetClassReviewBorder, 'Updating');
            var checkedStudents = vm.selectedCheckedStudents;

            if (checkedStudents.length) {
                var data = [];
                checkedStudents.forEach(function (stu) {
                    var jsonData = {
                        BubbleSheetId: stu.student.BubbleSheetId,
                        ClassId: stu.student.ClassId,
                        StudentId: stu.student.StudentId,
                        BubbleSheetFileId: stu.student.ImageData.BubbleSheetFileId,
                        RosterPosition: stu.student.ImageData.RosterPosition
                    }
                    data.push(jsonData);
                });

                var params = { model: data };

                BubbleSheetClassReviewService.markAsAbsent(params).done(function (response) {
                    // Update status Absent for all checkedStudents
                    if (response) {
                        checkedStudents.forEach(function (stu) {
                            stu.student.Status = 'Absent';
                        });
                    }
                    $bubbleSheetClassReviewBorder.unblock();
                });
            }
        },
        selectedAllClassView: function () {
            var vm = this;

            vm.isSelectedAllClassView = !vm.isSelectedAllClassView;

            if (!vm.isHiddenAllStudents) {
                vm.classView.students = vm.classView.students.map(function (stu) {
                    stu.student.IsChecked = vm.isSelectedAllClassView;
                    return stu;
                });
            }
        },
        setIntervalAutoSaved: function() {
            this.$options.intervalAutoSaved = setInterval(this.autoSavedClassView, this.intervalAutoSaved);
        },
        gotoStudentReviewPage: function() {
            this.isShowModalWarning = false;
            window.location.href = $('.bubbleSheetClassReviewPage.is-first').attr('href');
        },
        closeConfirmWarning: function () {
            this.isShowModalWarning = false;
        }
    },
    beforeDestroy: function () {
        clearInterval(this.$options.intervalAutoSaved);
        this.$options.intervalAutoSaved = null;
    }
});

function rendererImage (instance, td, row, col) {
    Handsontable.dom.empty(td);

    var span = document.createElement('span');
    span.className = 'icon icon-16 icon-document-bookmark u-cursor-pointer';

    rendererImageHandlerClick(span, td, row, col);

    td.appendChild(span);
    td.classList.add('htCenter');

    return td;
}

function rendererImageHandlerClick (span, td, row, col) {
    Handsontable.dom.addEvent(span, 'click', function (e) {
        e.preventDefault();
        var student = BubbleSheetClassReview.classView.students[row].student;
        var fileList = student.ImageData.ListFileSubViewModels;
        BubbleSheetClassReview.bubbleSheetImageSelected = [];

        if(fileList && fileList.length > 0 && !fileList[0].ImageUrl) {
            ShowBlock($('#BubbleSheetClassReviewBorder'), 'Loading');
            BubbleSheetClassReviewService.getImageUrl(fileList).done(function (response) {
                if (response.IsSuccess) {
                    $('#BubbleSheetClassReviewBorder').unblock();
                    student.ImageData.ListFileSubViewModels = response.Data;
                    viewBBSImages(response.Data);
                }
            }).error(function (e) { });
        } else {
                viewBBSImages(fileList);
        }
    });
}

function viewBBSImages (fileList) {
    fileList.forEach(function (item) {
        if (!!item.ImageUrl) {
            BubbleSheetClassReview.bubbleSheetImageSelected.push(item.ImageUrl);
        } else {
            BubbleSheetClassReview.bubbleSheetImageSelected.push('broken.png');
        }
    });

    BubbleSheetClassReview.isShowBubbleSheetImage = true;

    Vue.nextTick(function () {
        BubbleSheetClassReview.$els.bubbleSheetImage.setAttribute('style', '');
    });
}

function rendererStatusStudent (instance, td, row, col, prop, value, cellProperties) {
    Handsontable.dom.empty(td);
    var studentStatus = BubbleSheetClassReview.classView.students[row].student.Status;
    var filter = [];

    studentStatus = studentStatus.split('|');

    td.classList.add('htStudent');
    td.textContent = value;

    if (arrayContainsArray(BubbleSheetClassReview.filterStatus.finishedStatus, studentStatus) && !(studentStatus.indexOf('Multi-Mark') > -1)) {
        td.classList.add('is-finished');
        return td;
    }

    if (arrayContainsArray(BubbleSheetClassReview.filterStatus.reviewStatus, studentStatus)) {
        td.classList.add('is-review');
        return td;
    }

    if (arrayContainsArray(BubbleSheetClassReview.filterStatus.notGradingStatus, studentStatus)) {
        td.classList.add('is-not-grading');
        return td;
    }
}

function rendererStatusColor (instance, td, row, col, prop, value, cellProperties) {
    var studentStatus = BubbleSheetClassReview.classView.students[row].student.Status;

    td.textContent = value;
    td.classList.add('htCenter');

    if (studentStatus.indexOf('Absent') > -1) {
        return td;
    }

    if (studentStatus.indexOf('Missing') > -1) {
        td.classList.add('is-missing');
        return td;
    }

    var questions = BubbleSheetClassReview.classView.students[row].questions;
    var colName = col - 4 + 1;
    var questionName = 'question_' + colName;
    var status = questions[questionName].status;
    var meta = instance.getCellMeta(row, col);

    if (meta.readOnly) {
        td.classList.add('is-readonly');
    }

    if (status === 'Multi-Mark') {
        td.classList.add('is-multi-mark');
        return td;
    }

    if (status === 'Incomplete') {
        td.classList.add('is-incomplete');
        return td;
    }

    if (status === 'Open Ended') {
        td.classList.add('is-open-ended');
        return td;
    }
}

function validatorAlphanumeric (value, cb) {
    var cell = this.instance.getCell(this.row, this.col);
    var questions = BubbleSheetClassReview.classView.students[this.row].questions;
    var key = 'question_' + (this.col - 4 + 1);
    var isOpenEnded = questions[key].qtiSchemaId == 10;
    var arr = [];

    if (value === '') {
        cb(true);
        return;
    }

    if (isOpenEnded) {
        if (isNumber(value)) {
            value = parseInt(value, 10);

            if (isNaN(value)) {
                cb(false);
            } else {
                arr = Array.from(Array(questions[key].pointPossible + 1).keys());
                arr = arr.map(function (number) {
                    return parseInt(number, 10);
                });
    
                var max = Math.max.apply(null, arr);
                var min = Math.min.apply(null, arr);
    
                if (min <= value && value <= max) {
                    cb(true);
                } else {
                    cb(false);
                }
            }
        } else {
            cb(false);
        }
    } else {
        var answerIdentifiers = questions[key].answerIdentifiers;
        arr = answerIdentifiers.split(';');

        value = value.toUpperCase().replace(/,/g, ' ');
        var values = value.split(' ');
        var allExisted = true;

        for (var i = 0; i < values.length; i++) {
            var existed = false;
            
            for (var j = 0; j < arr.length; j++) {
                if (values[i] == arr[j]) {
                    existed = true;
                    break;
                }
            }

            if(!existed){
                allExisted = false;
                break;
            }
        }

        if (allExisted) {
            cb(true);
        } else {
            cb(false);
        }
    }
}

function isNumber (num) {
    var number = +num;
  
    if ((number - number) !== 0) {
      // Discard Infinity and NaN
      return false;
    }
  
    if (number === num) {
      return true;
    }
  
    if (typeof num === 'string') {
      // String parsed, both a non-empty whitespace string and an empty string
      // will have been coerced to 0. If 0 trim the string and see if its empty.
      if (number === 0 && num.trim() === '') {
        return false;
      }
      return true;
    }

    return false;
}

function arrayContainsArray (superset, subset) {
    if (0 === subset.length) {
      return false;
    }

    return subset.some(function (value) {
      return (superset.indexOf(value) >= 0);
    });
}