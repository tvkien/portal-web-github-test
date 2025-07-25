var ManageClassModel = new Vue({
    el: '#classes-by-teacher-and-school',
    data: {
        classId: 0,
        showInactive: false,
        isShowModalAssignStudent: false,
        isShowFilter: false,
        getStudentsUrl: '',
        addStudentsToClassUrl: '',
        getSelectedStudentURL: '',
        isCheckAllStudent: false,
        selectedStudents: [],
        selectedStudentsObject: [],
        isShowSelectedStudent: false,
        isShowViewSelectedStudent:false, //use for is-Show of vue
        initialDatableSelectedOnly: false,
        isShowAddNewStudent: true,
        isShowModalAddStudentClass: false,
        isLoading: false,
        isAddedStudentsToClass: false,
        promtMessage: '',
        removeStudentSelectAll: [],
        selectedClassIds: [],
        manageClass: {
            programs: [],
            grades: [],
            programSelected: [],
            gradesSelected: []
        },
        datatableSettingAssigns: null,
        isCheckAll: false,
        isShowModalMassAction: false,
        actionType: 'addRemoveTypeOption',
        classMetaOptions: [],
        classMetas: [],
        oldClassMetas: [],
    },
    computed: {
        showInactiveText: function () {
            return this.showInactive ? 'On' : 'Off';
        },
        isDisabledShowSelectedStudent: function () {
            return this.selectedStudents.length === 0 && !this.isShowSelectedStudent && !this.isCheckAllStudent;
        },
        isDisabledButton: function () {
            return this.selectedStudents.length === 0 && !this.isCheckAllStudent;
        },
        subjectAdded: function () {
            return this.classMetas.filter(function (item) { return item.Data }).map(function (item) { return item.Data });
        },
        subjectRemoved: function () {
          const currentSubjectNames = this.classMetas.map(function (item) { return item.Data });
          return this.oldClassMetas.filter(function (item) { return !currentSubjectNames.includes(item.Data) }).map(function (item) { return item.Data });
        }
    },
    watch: {
        'isShowSelectedStudent': 'showSelectedStudent',
        classMetas: function () {
            const $select = $('.selectTag');
            $select.off('select2:select select2:clear');

            $select.select2({
                allowClear: true,
                minimumResultsForSearch: -1,
                placeholder: ' ',
                dropdownParent: $('.list-subject')
            }).on('select2:select', function (e) {
                var $target = $(e.target);
                var metaId = $target.attr('classmetaid');
                var selectedValue = e.params.data.id;

                ManageClassModel.classMetas = ManageClassModel.classMetas.map(function (item) {
                    if (item.ClassMetaID == metaId) { item.Data = selectedValue; }
                    return item;
                })
            }).on('select2:clear', function (e) {
                var $target = $(e.target);
                var selectedValue = $target.attr('value');

                ManageClassModel.$nextTick(function () {
                    $target.select2('close');
                    ManageClassModel.classMetas = ManageClassModel.classMetas.filter(function (item) {
                        return item.Data !== selectedValue;
                    });
                });
            });
        }
    },
    methods: {
        closeModalConfirmAddStudent: function() {
            this.isShowModalAddStudentClass = false;
        },
        closeModalAddStudentClass: function () {
            this.isShowModalAssignStudent = false;
            this.isCheckAllStudent = false;
            this.isShowSelectedStudent = false;
            this.resetData();

            if (manageStudentDt != undefined) {
                manageStudentDt.fnDraw();
            }
            if (this.isAddedStudentsToClass) {
                this.isAddedStudentsToClass = false;
                //LoadClassToTable();
            }
        },
        changeInactive: function () {
            this.showInactive = !this.showInactive;
            ui.datatablefiltermanageclass.fnSettings().sAjaxSource = this.getAjaxSource();
            ui.datatablefiltermanageclass.fnDraw();
        },
        toggleFilter: function () {
            this.isShowFilter = !this.isShowFilter;
        },
        clearAllCheckBox: function () {
            this.manageClass.programSelected = [];
            this.manageClass.gradesSelected = [];
        },
        applyFilter: function () {
            this.isShowSelectedStudent = false;
            this.isCheckAllStudent = false;
            $('#chkAllStudent').prop('checked', false);
            this.resetData();

            ui.datatablefiltermanageclass.fnSettings().sAjaxSource = this.getAjaxSource();
            ui.datatablefiltermanageclass.fnDraw();
        },
        getAjaxSource: function () {
          return this.getStudentsUrl + this.classId + '&showInactive=' + this.showInactive + '&programIdList=' + this.manageClass.programSelected + '&gradeIdList=' + this.manageClass.gradesSelected;
        },
        checkAllStudent: function () {
          ManageClassModel.removeStudentSelectAll = [];
          var self = this;
          var $chkCheckbox = $('input[name="chkStudentId"][type=checkbox]');
          this.isCheckAllStudent = !this.isCheckAllStudent;
          if (this.isCheckAllStudent) {
                $chkCheckbox.prop('checked', true);
                $chkCheckbox.each(function (i, e) {
                  $(this).parents('tr').addClass('is-selected');
                  var data = allStudentTable.fnGetData($(e).parents('tr').index());
                  var studentId = $(e).val();
                  if (self.selectedStudents.indexOf(studentId) < 0) {
                    self.selectedStudents.push(studentId);
                    self.selectedStudentsObject.push(data);
                  }
                });
              }
              else {
                  $chkCheckbox.prop('checked', false);
                  $chkCheckbox.each(function (i, e) {
                    $(this).parents('tr').removeClass('is-selected');
                    var studentId = $(e).val();
                    self.selectedStudents = $.grep(self.selectedStudents, function (value) {
                      return value != studentId;
                    });
                    self.selectedStudentsObject = $.grep(self.selectedStudentsObject, function (value) {
                      return value[0] != studentId;
                    });
                  });
              }
              portalV2SkinCheckBox()
          },
          addStudentToClass: function () {
          var self = this;
          var studentSelectedCount = self.countSelectedStudent();
          if (studentSelectedCount <= 0) {
              return;
          }
          var columnsSearch = this.datatableSettingAssigns?.aoColumns.filter(function (column) { return column.bSearchable == true });
          var columnSearchs = columnsSearch?.map(function (column, index) { return index + 1 }).join(',');
          self.isLoading = true;
          $.post(self.addStudentsToClassUrl, {
              studentIdsStr: self.selectedStudents.toString(), classId: self.classId, removeStudents: self.removeStudentSelectAll.toString(),
              isCheckAll: false, programIdList: self.manageClass.programSelected.toString(), gradeIdList: self.manageClass.gradesSelected.toString(),
              searchBox: $('#datatablefiltermanageclass_filter input').val(),
              columnSearchs: columnSearchs
            }, function (response) {
                self.isLoading = false;
                self.isAddedStudentsToClass = true;
                if (response == true) {
                  $('#datatablefiltermanageclass #chkAllStudent').prop('checked', false);
                  $('#datatablefiltermanageclass #chkAllStudent').removeClass('input-checked-v2');
                  self.isShowModalAddStudentClass = false;
                  self.promtMessage = '';
                  self.isCheckAllStudent = false;
                  self.resetData();
                  if (self.isShowSelectedStudent) {
                      selectedStudentTable.fnClearTable();

                  } else {
                      ui.datatablefiltermanageclass.fnSettings().sAjaxSource = self.getAjaxSource();
                      ui.datatablefiltermanageclass.fnDraw();
                  }
                } else {
                  $("#isShowButtonAddStudent").hide();
                  self.promtMessage = 'This student is inactive. Please re-active the student before adding them to this class.';
                }
            });
          },
          showSelectedStudent: function () {
            var self = this;

            if (self.isShowSelectedStudent) {

                ShowBlock($('#datatablefiltermanageclass'), 'Loading');
                // Business logic has been changed (L25-322), but I just added a flag for repair if you want to rollback. Future will remove this code.
                var context = false;
                if (self.isCheckAllStudent && context) {
                    var params = {
                        programIdList: self.manageClass.programSelected.toString(),
                        gradeIdList: self.manageClass.gradesSelected.toString(),
                        classId: self.classId,
                        removeStudents: self.removeStudentSelectAll.toString(),
                        showInactive: self.showInactive,
                        searchText: $('#datatablefiltermanageclass_filter').find('input').val()
                    };

                    $.post(self.getSelectedStudentURL, params, function (response) {
                        self.isShowViewSelectedStudent = true;
                        self.selectedStudents = [];
                        self.selectedStudentsObject = [];

                        if (response.length) {
                            response.forEach(function (student) {
                                var arr = [student.StudentId, student.FirstName, student.LastName, student.Code,student.Gender, student.Grade];
                                self.selectedStudentsObject.push(arr);
                                self.selectedStudents.push(student.StudentId.toString());
                            });
                        } else {
                            self.resetData();
                        }

                        if (self.initialDatableSelectedOnly) {
                            selectedStudentTable.fnClearTable();
                            selectedStudentTable.fnAddData(self.selectedStudentsObject);
                            selectedStudentTable.fnDraw();
                        } else {
                            displaySelectedStudentOnly();
                        }
                    });
                } else {
                    self.isShowViewSelectedStudent = true;

                    if (self.initialDatableSelectedOnly) {
                        selectedStudentTable.fnClearTable();
                        selectedStudentTable.fnAddData(self.selectedStudentsObject);
                        selectedStudentTable.fnDraw();
                    } else {
                        displaySelectedStudentOnly();
                    }
                }
            } else {
                self.isShowViewSelectedStudent = false;
                ui.datatablefiltermanageclass.fnSettings().sAjaxSource = self.getAjaxSource();
                ui.datatablefiltermanageclass.fnDraw();
            }
        },
        collectSelectedStudentId: function () {
            var self = this;
            $('INPUT[name="chkStudentId"][type=checkbox]:checked').each(function (i, e) {
                var studentId = $(e).val();
                var data = allStudentTable.fnGetData($(e).parents('tr').index());
                if (self.selectedStudents.indexOf(studentId) < 0) {
                    self.selectedStudents.push(studentId);
                    self.selectedStudentsObject.push(data);
                }
            });
        },
        removeAllSelectedStudent: function () {
            this.resetData();
            this.isCheckAllStudent = false;
            selectedStudentTable.fnClearTable();
        },
        prompAddStudentToClass: function () {
            this.promtMessage = '';
            this.isLoading = false;
            this.isShowModalAddStudentClass = true;
            $("#isShowButtonAddStudent").show();
            var studentSelectedCount = this.countSelectedStudent();

            if (studentSelectedCount === 1) {
                this.promtMessage = 'Are you sure you want to add 1 student to the class?';
            } else {
                this.promtMessage = 'Are you sure you want to add these ' + studentSelectedCount + ' students to the class?';
            }
        },
        resetData:function() {
            this.selectedStudents = [];
            this.selectedStudentsObject = [];
            this.removeStudentSelectAll = [];
        },
        sortByStudentID:function() {
            var sort = arr.sort(function(a, b) {
                if (a == b) {
                    return 0;
                }

                return a > b ? 1 : -1;
            });
            return sort;
        },
        countSelectedStudent: function () {
            return this.selectedStudents.length || 0;
        },
        closeModalMassAction: function () {
            this.isShowModalMassAction = false;
        },
        getMetaOptions: function (subject) {
            var findSubject = this.classMetas.find(function (item) { return (item.ClassMetaID == subject.ClassMetaID) });
            if (findSubject) {
                var optionRemoves = this.classMetas.filter(function (item) { return (item.ClassMetaID != subject.ClassMetaID) });
                return this.classMetaOptions.filter(function (item) {
                    return !optionRemoves.some(function (itemRemove) { return itemRemove.Data === item.AggregateSubjectName })
                });
            }
            return this.classMetaOptions;
        },
        addNewSubject: function () {
            this.classMetas = [...this.classMetas, {
                ClassID: 0,
                ClassMetaID: new Date().getTime(),
                Data: "",
                Label: "Class Subject",
                Name: "Subject " + (this.classMetas.length + 1)
            }];
        },
        subjectTextRender: function (arr) {
            if (arr.length === 0) return '';
            if (arr.length === 1) return arr[0];
            if (arr.length === 2) return arr.join(' <span>and</span> ');
            return arr.slice(0, -1).join(', ') + ' <span>and</span> ' + arr[arr.length - 1];
        },
        resetPopup: function () {
            $("[name=actionType]").removeClass('input-checked-v2')
            this.actionType = 'addRemoveTypeOption';
            this.isShowModalMassAction = false;
            this.isCheckAll = false;
            this.classMetas = [];
            this.oldClassMetas = [];
            this.removeStudentSelectAll = [];
        },
    }
});
