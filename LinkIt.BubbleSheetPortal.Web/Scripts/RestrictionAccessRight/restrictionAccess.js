var USER_ROLE = 'PUBLISHER';

switch (rolesValue) {
  case '27':
    USER_ROLE = 'NETWORKADMIN';
    break;
  case '3':
    USER_ROLE = 'DISTRICTADMIN';
    break;
}
var url = window.location.protocol + '//' + window.location.host;
var URL_STATE = url + '/PopulateStateDistrict/GetStates',
  URL_STATE_NETWORKADMIN = url + '/PopulateStateDistrict/GetStatesForNetworkAdmin',
  URL_DISTRICT = url + '/PopulateStateDistrict/GetDistricts?stateId=',
  URL_DISTRICT_NETWORKADMIN = url + '/PopulateStateDistrict/GetDistrictsForNetworkAdmin?stateId=',
  URL_TEST_TYPE_GRADE_SUBJECT = url + '/RestrictionAccessRights/GetTestTypeGradeAndSubject';

var app = new Vue({
  el: '#restriction-access',
  data: function () {
    return {
      roles: {
        PUBLISHER: 'PUBLISHER',
        NETWORKADMIN: 'NETWORKADMIN',
        DISTRICTADMIN: 'DISTRICTADMIN',
        SCHOOLADMIN: 'SCHOOLADMIN',
        TEACHER: 'TEACHER',
      },
      state: [],
      districts: [],
      userRole: USER_ROLE,
      selectedState: 0,
      selectedDistrict: 0,
      selectedSchool: 0,
      school: [],
      testType: [],
      associatedClasses: [],
      checkTestTypes: false,
      grades: [],
      subjects: [],
      checkSubject: false,
      checkGrades: false,
      arrGrades: [],
      arrSubject: [],
      arrTestTypes: [],
      excludetesttypes: [],
      disabledFilter: true,
      tabActive: 1,
      showLoadingTest: false,
      showLoadingGrades: false,
      showLoadingSubjects: false,
      districtLabel: districtLabel,
      getTestFilterId: 0
    }
  },
  computed: {
    showSelectState: function () {
      return this.userRole !== 'DISTRICTADMIN'
    },
    showSelectDistrict: function () {
      return this.userRole !== 'DISTRICTADMIN'
    },
    disabled: function () {
      if ((this.arrGrades.length > 0 || this.arrSubject.length > 0 || this.arrTestTypes.length > 0) &&
        (!this.showSelectDistrict | (this.selectedState > 0 && this.selectedDistrict > 0))) {
        return false;
      } else {
        return true;
      }
    },
  },
  created: function () {
    var that = this;
    that.tabActive = 1;
    if (that.userRole === that.roles.PUBLISHER || that.userRole === that.roles.NETWORKADMIN) {
      getStates(URL_STATE, function (success) {
        var arr = [{ Id: 0, Name: 'Select State' }];
        if (success.length > 0) {
          if (success.length === 1) {
            that.state = success;
            that.selectedState = success[0].Id;
          } else {
            that.state = arr.concat(success);
            that.selectedState = 0;
          }
        } else {
          that.state = arr;
        }
      });
    } else {
      that.getTestFilter();
    }
  },
  methods: {
    triggerReloadTestFilters: function () {
      var self = this;
      if (!this.debounceReloadTestFilter) {
        this.debounceReloadTestFilter = _.debounce(function () {
          self.getTestFilter();
        }, 1000);
      }
      this.debounceReloadTestFilter();
    },
    clearFillterValues: function () {
      this.arrGrades = [];
      this.arrTestTypes = [];
      this.arrSubject = [];
      this.checkGrades = false;
      this.checkSubject = false;
      this.checkTestTypes = false;
    },
    selectStates: function (e) {
      var that = this;
      that.districts = [];
      that.disabledFilter = false;
      that.clearFillterValues();

      if (that.selectedState > 0) {
        getDistrictByStateId(URL_DISTRICT + that.selectedState, function (success) {
          var arr = [{ Id: 0, Name: 'Select ' + districtLabel }];
          that.selectedDistrict = 0;
          if (success.length > 0) {
            if (success.length === 1) {
              that.districts = success;
              that.selectedDistrict = success[0].Id;
            } else {
              that.districts = arr.concat(success);
              that.selectedDistrict = 0;
            }
          } else {
            that.districts = arr;
          }
        });
      }
    },
    onDistrictChange: function () {
      var that = this;
      that.disabledFilter = false;
      that.clearFillterValues();

      if (that.selectedDistrict > 0) {
        this.getTestFilter();
      }
    },
    getAssociatedClasses: function () {
      var self = this;
      $.ajax(URL_GET_ASSOCIATED_CLASSES)
        .done(function (associatedClassess) {
          if (associatedClassess.IsSuccess === true) {
            self.associatedClasses = associatedClassess.StrongData;
            var tagNames = self.associatedClasses.map(function (item) {
              return item.ClassName;
            });
            var uniqueTagNames = _.uniq(tagNames, false);
            self.populateDataToTagIt(uniqueTagNames, '#selectClasses', uniqueTagNames.length > 0 ? 'Select Classes' : 'No Results Found');
          }
          else {
            alert(associatedClassess.Message);
          }
        });
    },
    allSelectTestTypes: function () {
      this.disabledFilter = false;
      this.arrTestTypes = [];
      if (this.checkTestTypes) {
        for (var item in this.testType) {
          this.arrTestTypes.push(this.testType[item].Id);
        }
      }
    },
    selectCheckBoxTestTypes: function () {
      this.checkTestTypes = false;
      if (this.testType.length === this.arrTestTypes.length) {
        this.checkTestTypes = true;
      } else {
        this.checkTestTypes = false;
      }
    },
    allSelectGrades: function () {
      this.arrGrades = [];
      if (this.checkGrades) {
        for (var item in this.grades) {
          this.arrGrades.push(this.grades[item].Id);
        }
      }
    },
    selectCheckBoxGrades: function () {
      this.checkGrades = false;
      if (this.grades.length === this.arrGrades.length) {
        this.checkGrades = true;
      } else {
        this.checkGrades = false;
      }
    },
    selectCheckBoxSubjects: function () {
      if (this.subjects.length === this.arrSubject.length) {
        this.checkSubject = true;
      } else {
        this.checkSubject = false;
      }
    },
    onFilter: function () {
      this.excludeTestTypes = [];
      $('.check-box-all').attr('checked', false);

      $("#dataTableTest").dataTable().fnDraw();
    },
    onClearFilter: function () {
      this.clearFillterValues();
    },
    onSelectTab: function (tab) {
      this.tabActive = tab;
    },
    allSelectSubjects: function () {
      this.arrSubject = [];
      if (this.checkSubject) {
        for (var item in this.subjects) {
          this.arrSubject.push(this.subjects[item].Id);
        }
      }
    },
    getTestFilter: function (cb) {
      var that = this;
      that.getTestFilterId = that.getTestFilterId + 1;
      var currentId = that.getTestFilterId;
      that.showLoadingTest = true;
      that.showLoadingGrades = true;
      that.showLoadingSubjects = true;

      $.ajax(URL_TEST_TYPE_GRADE_SUBJECT + '?&districtId=' + that.selectedDistrict)
        .done(function (response) {
          if (currentId !== that.getTestFilterId) {
            return;
          }
          if (response.IsSuccess === true) {
            that.testType = response.StrongData.TestTypes;
            that.grades = response.StrongData.Grades;
            that.subjects = response.StrongData.Subjects;
            that.showLoadingGrades = that.showLoadingTest = that.showLoadingSubjects = false;
            cb && cb();
          }
          else {
            alert(response.Message);
          }
        });
    },
  }
});

function loadEditAccessRights(options) {
  var accesslist = [];
  var $listItems = app.tabActive == 1 ? $('input[name=chkCategory]:checked') : $('input[name=chkTest]:checked');
  var $dataTable = app.tabActive == 1 ? $("#dataTableCategory") : $("#dataTableTest");

  var template = document.getElementById('edit-access-right-template').innerHTML;
  var displayType = '';
  if (options.RestrictionTypeName == 'test') {
    displayType = !options.CategoryTestId ? 'these tests' : 'this test';
  } else {
    displayType = !options.CategoryTestId ? 'these categories' : 'this category';
  }
  template = template.replace('{{DisplayTitle}}', options.DisplayName + ' Restrictions')
    .replace('{{TeacherAccessLabel}}', 'Allow Teacher access to ' + displayType)
    .replace('{{SchoolAdminAccessLabel}}', 'Allow School Admin access to ' + displayType);

  var $container = $(template);

  if (options.RestrictionTypeName == 'test') {
    var category = app.testType.find(function (item) { return item.Id == options.CategoryId });
    if (category && category.SchoolAdminRestriction == RESTRICTION_TYPE.NO_ACCESS) {
      $container.find('#chkSchoolAdminAccess').attr('disabled', true);
    }
    if (category && category.TeacherRestriction == RESTRICTION_TYPE.NO_ACCESS) {
      $container.find('#chkTeacherAccess').attr('disabled', true);
    }
  }
  var checkedSchool = options.SchoolAdminRestriction == RESTRICTION_TYPE.FULL_ACCESS;
  var checkedTeacher = options.TeacherRestriction == RESTRICTION_TYPE.FULL_ACCESS;
  if (!options.CategoryTestId) {
    var allTeachers = []
    var allSchools = []
    $listItems.each(function () {
      allTeachers.push($(this).attr('teacherRestrictAccess'));
      allSchools.push($(this).attr('schoolAdminRestrictAccess'));
    });
   
    checkedSchool = allSchools.indexOf(RESTRICTION_TYPE.NO_ACCESS) == -1;
    checkedTeacher = allTeachers.indexOf(RESTRICTION_TYPE.NO_ACCESS) == -1;
  }

  $container.find('#chkSchoolAdminAccess').attr('checked', checkedSchool);
  $container.find('#chkTeacherAccess').attr('checked', checkedTeacher);

  $container.find('#editAccessForm').submit(function (ev) {
    ev.preventDefault();
    $container.find("#dvEditAccess input.accessbox").each(function (index, elem) {
      var roleId = $(elem).attr("data-role");
      var isAccess = $(elem).is(':checked');
      var restrictionModuleID = $(elem).attr("data-restrictionModuleID");
      if (options.CategoryTestId > 0) {
        var xliModuleRoleID = roleId == 2 ? options.XLITeacherModuleRoleId : options.XLISchoolAdminModuleRoleId;
        accesslist.push({
          XLITestRestrictionModuleRoleID: xliModuleRoleID,
          RestrictedObjectCategoryTestId: options.CategoryTestId,
          RoleId: roleId,
          AllowAccess: isAccess,
          RestrictionTypeName: options.RestrictionTypeName,
          DistrictId: app.tabActive == 1 ? districtId : app.selectedDistrict,
          RestrictionModuleID: restrictionModuleID,
        });
      } else {
        $listItems.each(function () {
          var dataObj = {
            CategoryTestId: app.tabActive == 1 ? $(this).attr('categoryId') : $(this).attr('virtualTestId'),
            XLITeacherModuleRoleId: $(this).attr('xLITeacherModuleRoleId'),
            XLISchoolAdminModuleRoleId: $(this).attr('xLISchoolAdminModuleRoleId')
          }
          var xliModuleRoleID = roleId == 2 ? dataObj.XLITeacherModuleRoleId : dataObj.XLISchoolAdminModuleRoleId;
          accesslist.push({
            XLITestRestrictionModuleRoleID: xliModuleRoleID,
            RestrictedObjectCategoryTestId: dataObj.CategoryTestId,
            RoleId: roleId,
            AllowAccess: isAccess,
            RestrictionTypeName: options.RestrictionTypeName,
            DistrictId: app.tabActive == 1 ? districtId : app.selectedDistrict,
            RestrictionModuleID: restrictionModuleID,
          });
        })
      }
    });

    var result = JSON.stringify(accesslist);
    ShowBlock($container.find('#dvEditAccess'), "Updating Restrictions");

    $.ajax({
      url: $container.find('#editAccessForm').attr('action'),
      type: 'POST',
      data: { restrictionCategoriesTests: result },
      success: function (response) {
        if (response.Success == true) {
          $dataTable.dataTable().fnDraw(false);
          window.localStorage.setItem('triggerDataTableTest', new Date().getTime())
          $container.find('#success-message-create').show();
          $container.find('#error-messages-create').hide();
          setTimeout(function () {
            $(".dialog").dialog("close");
          }, 2000);
        } else {
          $container.find('#error-messages-create').html('<li> ' + response.ErrorMessage + ' </li>');
          $container.find('#error-messages-create').show();
          $container.find('#success-message-create').hide();
          $container.find('#btnSave').enableBt();
        }
        $container.find('#dvEditAccess').unblock();
      },
      error: function (err) {
        $container.find('#dvEditAccess').unblock();
      }
    });
  });
  $container.find('#btnCancel').click(function () {
    $(".dialog").dialog("close");
  });
  return $container;
}
