var USER_ROLE = 'PUBLISHER';

switch (rolesValue) {
  case '5': 
    USER_ROLE = "PUBLISHER";
    break;
  case '27': 
    USER_ROLE = 'NETWORKADMIN';
    break;
  case '3': 
    USER_ROLE = 'DISTRICTADMIN';
    break;
  case '8':
    USER_ROLE = 'SCHOOLADMIN';
    break;
  case '2':
    USER_ROLE = 'TEACHER';
    break;
}

var url = window.location.protocol + '//' + window.location.host;
var URL_STATE = url + '/StudentPreference/GetStates',
  URL_DISTRICT = url + '/StudentPreference/GetDistricts?stateId=',
URL_SUBJECTS = url + '/StudentPreference/GetSubjectsByGradeIdAndAuthor?districtId=';
URL_TEST_TYPE_GRADE_SUBJECT = url + '/PerformanceBandAutomation/GetTestTypeGradeAndSubject';
URL_APPLY_SETTING = url + '/PerformanceBandAutomation/ApplySetting';
URL_REMOVE_SETTING = url + '/PerformanceBandAutomation/RemoveSetting';
URL_GET_PBS_IN_EFFECT = url + '/PerformanceBandAutomation/GetPBSInEffect';
URL_GET_PBS_GROUP = url + '/PerformanceBandAutomation/GetPBSGroup';

var app = new Vue({
  el: '#student-references',
  data: {
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
    testType: [],
    checkTestTypes: false,
    grades: [],
    subjects: [],
    pbsEffects: [],
    pbsGroups: [],
    checkSubject: false,
    checkGrades: false,
    checkPbsEffect: false,
    checkPbsGroup: false,
    arrGrades: [],
    arrSubject: [],
    arrTestTypes: [],
    arrPbsEffects: [],
    arrPbsGroups: [],
    disabledFilter: true,
    showLoadingTest: false,
    showLoadingGrades: false,
    showLoadingSubjects: false,
    showLoadingPBSInEffect: false,
    showLoadingPBSGroup: false,
    districtLabel: districtLabel,
    getTestFilterId: 0
  },
  components: {
    'my-modal': modal,
    'view-modal': view
  },
  computed: {
    disabled: function () {
        var hasSeletedTestFilter = this.arrGrades.length > 0 || 
            this.arrSubject.length > 0 || 
            this.arrTestTypes.length > 0 ||
            this.arrPbsEffects.length > 0 ||
            this.arrPbsGroups.length > 0;
        var hasSelectedState = this.selectedState > 0;
        var hasSeletedDistrict = this.selectedDistrict > 0;
        
        if (this.userRole === 'PUBLISHER') {
            return !(hasSelectedState && hasSeletedDistrict && hasSeletedTestFilter)
        }
        return true;
    },
    addFilteredTestTypes: function () {
      if (this.testType.length === 0)
        return ['-100'];

      if (this.arrTestTypes.length > 0)
        return this.arrTestTypes;
      else {
        return this.testType.map(function (item) {
          return item.Id;
        })
      }
    },
    addFilteredGrades: function () {
      if (this.grades.length === 0)
        return ['-100'];

      if (this.arrGrades.length > 0)
        return this.arrGrades;
      else
        return this.grades.map(function (item) {
          return item.Id;
        })
    }
  },
  created: function () {
    var that = this;

    if (that.userRole === that.roles.PUBLISHER) {
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
    }
  },
  methods: {
    clearFillterValues: function () {
      this.arrGrades = [];
      this.arrTestTypes = [];
      this.arrSubject = [];
      this.arrPbsEffects = [],
      this.arrPbsGroups = [],
      this.checkGrades = false;
      this.checkSubject = false;
      this.checkTestTypes = false;
      this.checkPbsEffect = false;
      this.checkPbsGroup = false;
      this.clearCheckedCheckbox();
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
        this.disabledFilter = false;
        this.clearFillterValues();
        this.selectedDistrict > 0 && this.getTestFilter();
    },
    allSelectTestTypes: function () {
      this.disabledFilter = false;
      this.arrTestTypes = [];
      if (this.checkTestTypes) {
        for (var item in this.testType) {
          if (this.testType[item].IsShow)
            this.arrTestTypes.push(this.testType[item].Id);
        }
        this.clearCheckedCheckbox();
      }
    },
    selectCheckBoxTestTypes: function () {
      this.checkTestTypes = false;
      if (this.testType.length === this.arrTestTypes.length) {
        this.checkTestTypes = true;
        setCheckBoxClassV2Skin(true, $('#test-types'))
      } else {
        setCheckBoxClassV2Skin(false, $('#test-types'))
        this.checkTestTypes = false;
      }
    },
    allSelectGrades: function () {
      this.arrGrades = [];
      if (this.checkGrades) {
        for (var item in this.grades) {
          this.arrGrades.push(this.grades[item].Id);
        }
        this.clearCheckedCheckbox();
      }
    },
    selectCheckBoxGrades: function () {
      this.checkGrades = false;
      if (this.grades.length === this.arrGrades.length) {
        this.checkGrades = true;
        setCheckBoxClassV2Skin(true, $('#grades'))
      } else {
        this.checkGrades = false;
        setCheckBoxClassV2Skin(false, $('#grades'))
      }
    },
    selectCheckBoxSubjects: function () {
      if (this.subjects.length === this.arrSubject.length) {
        this.checkSubject = true;
        setCheckBoxClassV2Skin(true, $('#subjects'))
      } else {
        this.checkSubject = false;
        setCheckBoxClassV2Skin(false, $('#subjects'))
      }
    },
    onFilter: function () {
      $('.check-box-all').attr('checked', false);

      loadData();
    },
    onClearFilter: function () {
      this.clearFillterValues();
    },
    allSelectSubjects: function () {
      this.arrSubject = [];
      if (this.checkSubject) {
        for (var item in this.subjects) {
          this.arrSubject.push(this.subjects[item].Name);
        }
        this.clearCheckedCheckbox();
      }
    },
    selectCheckBoxPbsEffect: function () {
      if (this.pbsEffects.length === this.arrPbsEffects.length) {
        this.checkPbsEffect = true;
        setCheckBoxClassV2Skin(true, $('#pbsEffect'))
      } else {
        this.checkPbsEffect = false;
        setCheckBoxClassV2Skin(false, $('#pbsEffect'))
      }
    },
    allSelectPbsEffect: function () {
      this.arrPbsEffects = [];
      if (this.checkPbsEffect) {
        for (var item in this.pbsEffects) {
          this.arrPbsEffects.push(this.pbsEffects[item].Id);
        }
        this.clearCheckedCheckbox();
      }
    },
    selectCheckBoxPbsGroup: function () {
      if (this.pbsGroups.length === this.arrPbsGroups.length) {
        this.checkPbsGroup = true;
        setCheckBoxClassV2Skin(true, $('#pbsGroup'))
      } else {
        this.checkPbsGroup = false;
        setCheckBoxClassV2Skin(false, $('#pbsGroup'))
      }
    },
    allSelectPbsGroup: function () {
      this.arrPbsGroups = [];
      if (this.checkPbsGroup) {
        for (var item in this.pbsGroups) {
          this.arrPbsGroups.push(this.pbsGroups[item].Id);
        }
        this.clearCheckedCheckbox();
      }
    },
    getTestFilter: function () {
      var that = this;
      that.getTestFilterId = that.getTestFilterId + 1;
      var currentId = that.getTestFilterId;
      that.showLoadingTest = true;
      that.showLoadingGrades = true;
      that.showLoadingSubjects = true;
      that.showLoadingPBSInEffect = true;
      that.showLoadingPBSGroup = true;

      var filter = {
        districtId: that.selectedDistrict
      }

      $.ajax(`${URL_TEST_TYPE_GRADE_SUBJECT}?${$.param(filter)}`)
        .done(function (response) {
          if (currentId !== that.getTestFilterId) {
            return;
          }
          if (response.IsSuccess === true) {
            that.testType = response.StrongData.TestTypes.map(x => ({ ...x, IsShow: true }));
            that.grades = response.StrongData.Grades.map(x => ({ ...x, IsShow: true }));
            that.subjects = response.StrongData.Subjects.map(x => ({ ...x, IsShow: true }));
            that.showLoadingGrades = that.showLoadingTest = that.showLoadingSubjects = false;
          }
          else {
            alert(response.Message);
          }
        });
       $.ajax(`${URL_GET_PBS_IN_EFFECT}`)
        .done(function (response) {
          if (response) {
             that.pbsEffects = response;
             that.showLoadingPBSInEffect = false;
          }
        });
       $.ajax(`${URL_GET_PBS_GROUP}?${$.param(filter)}`)
        .done(function (response) {
          if (response) {
            that.pbsGroups = response.map(p => { return { Id: p.Id, Name: `${p.Name} - ${p.Id}` } });
            that.showLoadingPBSGroup = false;
          }
        });
    },
    viewResult: function (virtualTestID, resultDate) {
      var fromDate = resultDate ? new Date(resultDate) : new Date();
      fromDate.setMonth(fromDate.getMonth() - 6);

      var toDate = resultDate ? new Date(resultDate) : new Date();
      toDate.setMonth(toDate.getMonth() + 6);

      var query = {
        virtualtestids: virtualTestID,
        districtid: this.selectedDistrict,
        datefrom: fromDate.toISOString().slice(0, 10),
        dateto: toDate.toISOString().slice(0, 10),
      }
      window.open(window.location.origin + `/Content/HtmlModules/studentReport/#/student-results?${$.param(query)}`);
    },
    clearCheckedCheckbox() {
      var elCheckbox = $('input[type=checkbox]');
      for (var index = 0; index < elCheckbox.length; index++) {
        if ($(elCheckbox[index]).is(':checked') || $(elCheckbox[index]).attr('checked') == 'checked') {
          setCheckBoxClassV2Skin(false, elCheckbox[index]);
        }
      }
    },
    handleAfterApplyRemoveSettings(virtualTests) {
      var pbsInEffectColumnIndex = getCellIndexBysName('#dataTable', 'PBSInEffect');
      $('.cbTestId').each(function (i, elem) {
        var virtualTestID = +$(elem).val();
        var $tr = $(elem).parent().parent();
        var virtualTest = virtualTests.find(function (x) { return x.VirtualTestID === virtualTestID; });
        if (virtualTest) {
          var $cell = $tr.find(`td:eq(${pbsInEffectColumnIndex})`);
          if (virtualTest.IsChange) {
            $tr.addClass('light-blue');
            $tr.removeClass('gray');
            $cell.html(virtualTest.PBSInEffect);
          } else {
            $tr.addClass('gray');
            $tr.removeClass('light-blue');
          }
        }
      });
    },
    applySettings: function (virtualTestIDs) {
      var _this = this;
      var currentId = ++this.getTestFilterId;
      var payload = {
        districtId: this.selectedDistrict,
        virtualTestIDs: virtualTestIDs.join(",")
      }
      window.isApplayRemoveSettings = true;
      ShowBlock($('#dataTable'), "Loading");
      $('#btnAddNew').addClass('disabled');
      $('#btnRemove').addClass('disabled');

      $.ajax({
        type: 'POST',
        data: payload,
        url: URL_APPLY_SETTING,
        datatype: 'json',
        success: function (response) {
          if (currentId !== _this.getTestFilterId) {
            return;
          }
          if (response.Status !== "success") {
            return;
          }  

          window.virtualTests = response.StrongData.VirtualTests;
          loadData();
        },
        failure: function (response) {
          $('#dataTable').unblock();
          alert(response);
        }
      });
    },
    removeSettings: function (virtualTestIDs) {
      var _this = this;
      var currentId = ++this.getTestFilterId;
      var payload = {
        districtId: this.selectedDistrict,
        virtualTestIDs: virtualTestIDs.join(",")
      }
      window.isApplayRemoveSettings = true;
      ShowBlock($('#dataTable'), "Loading");
      $('#btnAddNew').addClass('disabled');
      $('#btnRemove').addClass('disabled');

      $.ajax({
        type: 'POST',
        data: payload,
        url: URL_REMOVE_SETTING,
        datatype: 'json',
        success: function (response) {

          if (currentId !== _this.getTestFilterId) {
            return;
          }
          if (response.Status !== "success") {
            return;
          }

          window.virtualTests = response.StrongData.VirtualTests;
          loadData();
        },
        failure: function (response) {
          $('#dataTable').unblock();
          alert(response);
        }
      });
    }
  }
});
