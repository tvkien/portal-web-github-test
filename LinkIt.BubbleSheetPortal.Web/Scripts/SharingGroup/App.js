var SharingGroupModel = new Vue({
    el: '#assign-user-sharing-group',
    data: {
      sharingGroupId: 0,
      showInactive: false,
      isShowModalAssignUser: false,
      isShowFilter: false,
      getUserAvailableUrl: '',
      addUserToSharingGroupUrl: '',
      getSelectedStudentURL: '',
      isCheckAllUser: false,
      selectedStudents: [],
      selectedStudentsObject: [],
      isShowSelectedUser: false,
      isShowViewSelectedUser:false,
      initialDatableSelectedOnly: false,
      isShowModalAddUserToSharingGroup: false,
      isLoading: false,
      isAddUserToSharingGroup: false,
      promtMessage: '',
      removeUserSelectAll: [],
      assignUser: {
          roles: [],
          schools: [],
          allSchools: [],
          roleSelected: [],
          schoolSelected: []
      },
      datatableSettingAssigns: null,
      isCheckAll: false,
      isCheckAllRole: false,
      isCheckAllSchool: false
    },
    computed: {
        showInactiveText: function () {
            return this.showInactive ? 'On' : 'Off';
        },
        isDisabledShowSelectedStudent: function () {
          return this.selectedStudents.length === 0 && !this.isShowSelectedUser && !this.isCheckAllUser;
        },
        isDisabledButton: function () {
          return this.selectedStudents.length === 0 && !this.isCheckAllUser;
        }
    },
    watch: {
      'isShowSelectedUser': 'showSelectedUser'
    },
    methods: {
        closeModalConfirmAddUserToSharingGroup: function() {
          this.isShowModalAddUserToSharingGroup = false;
        },
        closeModalAddUserSharingGroup: function () {
            this.isShowModalAssignUser = false;
            this.isCheckAllUser = false;                
            this.isShowSelectedUser = false;
            this.resetData();
            if (this.isAddUserToSharingGroup) {
              this.isAddUserToSharingGroup = false;
              ui.manageUserInSharingGroupTable.dataTable().fnFilter('');
              ui.manageUserInSharingGroupTable.fnDraw();
            }
        },
        changeInactive: function () {
            this.showInactive = !this.showInactive;
            ui.datatablefilterUserAddSharingGroup.fnSettings().sAjaxSource = this.getAjaxSource();
            ui.datatablefilterUserAddSharingGroup.fnDraw();
        },
        toggleFilter: function () {
            this.isShowFilter = !this.isShowFilter;
        },
        clearAllCheckBox: function () {
            this.assignUser.roleSelected = [];
            this.assignUser.schoolSelected = [];
        },
        applyFilter: function () {
          this.isShowSelectedUser = false;
          this.isCheckAllUser = false;
          $('#chkAllUsers').prop('checked', false);
          this.resetData();
          ui.datatablefilterUserAddSharingGroup.fnSettings().sAjaxSource = this.getAjaxSource();
          ui.datatablefilterUserAddSharingGroup.fnDraw();
        },
        getAjaxSource: function () {
          return this.getUserAvailableUrl + '&showInactiveUser=' + this.showInactive + '&roleIdList=' + this.assignUser.roleSelected + '&schoolIdList=' + this.assignUser.schoolSelected;
        },
        checkAllUser: function () {
          var self = this;
          var $chkCheckbox = $('input[name="chkUserId"][type=checkbox]');
          this.isCheckAllUser = !this.isCheckAllUser;
          if (this.isCheckAllUser) {               
                  $chkCheckbox.prop('checked', true);
                  $chkCheckbox.each(function (i, e) {
                    $(this).parents('tr').addClass('is-selected');
                    var data = allUserTable.fnGetData($(e).parents('tr').index());
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
          },
          addUserToSharingGroup: function () {
            var self = this; 
            var studentSelectedCount = self.countSelectedUser();
            if (studentSelectedCount <= 0) {
                return;
            }
            self.isLoading = true;
            $.post(self.addUserToSharingGroupUrl, {
              sharingGroupId: self.sharingGroupId,
              userIdsStr: self.selectedStudents.toString(),
              }, function (response) {
                  self.isLoading = false;
              self.isAddUserToSharingGroup = true;
                  if (response == true) {
                      self.isShowModalAddUserToSharingGroup = false;
                      self.promtMessage = '';
                    self.isCheckAllUser = false;
                      self.resetData();
                    if (self.isShowSelectedUser) {                        
                      selectedUserTable.fnClearTable();

                    } else {
                      ui.datatablefilterUserAddSharingGroup.fnSettings().sAjaxSource = self.getAjaxSource();
                      ui.datatablefilterUserAddSharingGroup.dataTable().fnFilter('');
                      ui.datatablefilterUserAddSharingGroup.fnDraw();
                      $('#chkAllUsers').prop('checked', false);
                      $('#chkAllUsers').removeClass('input-checked-v2');
                    }                    
                  } else {
                    $("#isShowButtonAddUserToSharingGroup").hide();
                    self.promtMessage = 'This user is inactive. Please re-active the user before adding them to this sharing group.';
                  }
              });
          },
          showSelectedUser: function () {            
            var self = this;
            
            if (self.isShowSelectedUser) {

                ShowBlock($('#datatablefilterUserAddSharingGroup'), 'Loading');

              if (self.isCheckAllUser) {
                    var params = {
                        programIdList: self.assignUser.roleSelected.toString(),
                        gradeIdList: self.assignUser.schoolSelected.toString(),
                        classId: self.classId,
                      removeStudents: self.removeUserSelectAll.toString(),
                        showInactive: self.showInactive,
                        searchText: $('#datatablefilterUserAddSharingGroup_filter').find('input').val()
                    };

                    $.post(self.getSelectedStudentURL, params, function (response) {
                      self.isShowViewSelectedUser = true;
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
                          selectedUserTable.fnClearTable();
                          selectedUserTable.fnAddData(self.selectedStudentsObject);
                          selectedUserTable.fnDraw();
                        } else {
                            displaySelectedStudentOnly();
                        }
                    });
                } else {
                self.isShowViewSelectedUser = true;

                    if (self.initialDatableSelectedOnly) {
                      selectedUserTable.fnClearTable();
                      selectedUserTable.fnAddData(self.selectedStudentsObject);
                      selectedUserTable.fnDraw();
                    } else {
                        displaySelectedStudentOnly();
                    }
                }
            } else {
              self.isShowViewSelectedUser = false;
                ui.datatablefilterUserAddSharingGroup.fnSettings().sAjaxSource = self.getAjaxSource();
                ui.datatablefilterUserAddSharingGroup.fnDraw();
            }
        },
        collectSelectedStudentId: function () {
          var self = this;
          $('INPUT[name="chkUserId"][type=checkbox]:checked').each(function (i, e) {
                var studentId = $(e).val();
            var data = allUserTable.fnGetData($(e).parents('tr').index());
                if (self.selectedStudents.indexOf(studentId) < 0) {
                    self.selectedStudents.push(studentId);
                    self.selectedStudentsObject.push(data);
                }
            });
        },
        removeAllSelectedStudent: function () {
            this.resetData();
          this.isCheckAllUser = false;
          selectedUserTable.fnClearTable();
        },
        prompAddUserToSharingGroup: function () {
            this.promtMessage = '';
            this.isLoading = false;
            this.isShowModalAddUserToSharingGroup = true;
            $("#isShowButtonAddUserToSharingGroup").show();
            var userSelectedCount = this.countSelectedUser();
           
          if (userSelectedCount === 1) {
              this.promtMessage = 'Are you sure you want to add 1 user to the sharing group?';
          } else {
            this.promtMessage = 'Are you sure you want to add these ' + userSelectedCount + ' users to the sharing group?';
          }
        },
        resetData:function() {           
          this.selectedStudents = [];
          this.selectedStudentsObject = [];
          this.removeUserSelectAll = [];
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
        countSelectedUser: function () {
          var studentSelectedCount = 0;
          if (this.isCheckAllUser && !this.isShowSelectedUser) {
            studentSelectedCount = allUserTable.fnSettings().aoData.length;
          if (this.removeUserSelectAll.length > 0) {
                studentSelectedCount = studentSelectedCount - this.removeUserSelectAll.length;
              }
          } else {
              studentSelectedCount = this.selectedStudents.length;
          }
          return studentSelectedCount;
      },
      checkAllRoles: function () {
        if (this.isCheckAllRole) {
          this.assignUser.roleSelected = this.assignUser.roles.map(item => item.Id);
        }
        else {
          this.assignUser.roleSelected = [];
        }
      },
      checkRoles: function () {
        this.isCheckAllRole = this.assignUser.roleSelected.length == this.assignUser.roles.length;
      },
      checkAllSchools: function () {
        if (this.isCheckAllSchool) {
          this.assignUser.schoolSelected = this.assignUser.schools.map(item => item.Id);
        }
        else {
          this.assignUser.schoolSelected = [];
        }
      },
      checkSchools: function () {
        this.isCheckAllSchool = this.assignUser.schoolSelected.length == this.assignUser.schools.length;
      },
      onSearchSchool: function (e) {
        var self = this;
        var search = $('#idSearchSchool').val();
        self.assignUser.schools = self.assignUser.allSchools.filter(function (school) { return school.Name.includes(search) });
      }
    }
});
