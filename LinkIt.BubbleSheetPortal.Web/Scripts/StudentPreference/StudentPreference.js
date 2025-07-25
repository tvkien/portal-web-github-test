var USER_ROLE = 'PUBLISHER';

var LEVEL = [
  { value: '0', text: 'Select Level' },
  { value: 'district', text: districtLabel },
  { value: 'school', text: 'School' },
  { value: 'class', text: 'Teacher' }
];
switch (rolesValue) {
  case '5': USER_ROLE = "PUBLISHER";
    LEVEL = [
      { value: '0', text: 'Select Level' },
      { value: 'enterprise', text: 'Enterprise' },
      { value: 'district', text: districtLabel },
      { value: 'school', text: 'School' },
      { value: 'class', text: 'Teacher' }
    ];

    break;
  case '27': USER_ROLE = 'NETWORKADMIN';
    LEVEL = [
      { value: '0', text: 'Select Level' },
      { value: 'district', text: districtLabel },
      { value: 'school', text: 'School' },
      { value: 'class', text: 'Teacher' }
    ];

    break;
  case '3': USER_ROLE = 'DISTRICTADMIN';
    LEVEL = [
      { value: '0', text: 'Select Level' },
      { value: 'district', text: districtLabel },
      { value: 'school', text: 'School' },
      { value: 'class', text: 'Teacher' }
    ];

    break;
  case '8':
    USER_ROLE = 'SCHOOLADMIN';
    LEVEL = [
      { value: '0', text: 'Select Level' },
      { value: 'school', text: 'School' },
      { value: 'class', text: 'Teacher' }
    ];
    break;
  case '2':
    USER_ROLE = 'TEACHER';
    LEVEL = [
      { value: 'class', text: 'Teacher' }
    ];
    break;
}

var url = window.location.protocol + '//' + window.location.host;
var URL_STATE = url + '/StudentPreference/GetStates',
  URL_DISTRICT = url + '/StudentPreference/GetDistricts?stateId=',
  URL_SCHOOL = url + '/StudentPreference/GetSchools?districtId=',
  URL_TEST_TYPE = url + '/StudentPreference/GetListTestType?districtId=',
  URL_GRADESBYDISTRICT = url + '/StudentPreference/GetGradeByUserId?districtId=';
URL_SUBJECTS = url + '/StudentPreference/GetSubjectsByGradeIdAndAuthor?districtId=';
URL_TEST_TYPE_GRADE_SUBJECT = url + '/StudentPreference/GetTestTypeGradeAndSubject';

var URL_GET_ASSOCIATED_CLASSES = '/StudentPreference/GetAssociatedClasses';
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
    level: LEVEL,
    state: [],
    districts: [],
    userRole: USER_ROLE,
    selectedLevel: LEVEL[0].value,
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
    excludeTestTypes: [],
    disabledFilter: true,
    tabActive: 1,
    showViewOption: false,
    showLoadingTest: false,
    showLoadingGrades: false,
    showLoadingSubjects: false,
    districtLabel: districtLabel,
    showloading: false,
    showloadingview: false,
    rawhtml: '',
    showview: false,
    groupDetails: {
      default: [],
      optionBS: []
    },
    showModal: false,
    spectific: '1',
    debounceReloadTestFilter: null,
    getTestFilterId: 0,
  },
  components: {
    'my-modal': modal,
    'view-modal': view
  },
  computed: {
    showDefaultOption: function () {
      return this.userRole == 'PUBLISHER' || this.userRole == 'NETWORKADMIN' || this.userRole == 'DISTRICTADMIN' || this.userRole == 'SCHOOLADMIN';
    },
    showTestTypeOption: function () {
      return this.userRole == 'PUBLISHER' || this.userRole == 'NETWORKADMIN' || this.userRole == 'DISTRICTADMIN' || this.userRole == 'SCHOOLADMIN';
    },
    showSpecificOption: function () {
      return this.userRole == 'PUBLISHER' || this.userRole == 'NETWORKADMIN' || this.userRole == 'DISTRICTADMIN' || this.userRole == 'SCHOOLADMIN' || this.userRole == 'TEACHER'
    },
    showSelectSchool: function () {
      switch (this.selectedLevel) {
        case 'enterprise':
        case 'district':
          if (this.userRole === 'PUBLISHER' || this.userRole === 'NETWORKADMIN') {
            return false;
          } else if (this.userRole === 'DISTRICTADMIN') {
            return false;
          } else {
            return true;
          }
          break;
        case 'school':
          return true;
          break;
        case '0':
          return false;
          break;
        case 'class':
          return false;
      }
    },
    showSelectState: function () {
      switch (this.selectedLevel) {
        case 'enterprise':
        case 'district':
          if (this.userRole === 'PUBLISHER' || this.userRole === 'NETWORKADMIN') {
            return true;
          } else if (this.userRole === 'DISTRICTADMIN') {
            return false;
          } else {
            return true;
          }
          break;
        case 'school':
          if (this.userRole === 'PUBLISHER' || this.userRole === 'NETWORKADMIN') {
            return true;
          } else {
            return false;
          }
          break;
        case '0':
          return false;
          break;
        case 'class':
          return false;
          break;
      }
    },
    showSelectDistrict: function () {
      switch (this.selectedLevel) {
        case 'enterprise':
        case 'district':
          if (this.userRole === 'PUBLISHER' || this.userRole === 'NETWORKADMIN') {
            return true;
          } else if (this.userRole === 'DISTRICTADMIN') {
            return false;
          } else {
            return true;
          }
          break;
        case 'school':
          if (this.userRole === 'PUBLISHER' || this.userRole === 'NETWORKADMIN') {
            return true;
          } else {
            return false;
          }
          break;
        case '0':
          return false;
          break;
        case 'class':
          return false;
          break;
      }
    },
    disabled: function () {
      switch (this.selectedLevel) {
        case 'enterprise':
        case 'district':
          if (this.userRole === 'PUBLISHER' || this.userRole === 'NETWORKADMIN') {
            if ((this.arrGrades.length > 0 || this.arrSubject.length > 0
              || this.arrTestTypes.length > 0) && this.selectedState > 0 &&
              this.selectedDistrict > 0) {
              return false;
            } else {
              return true;
            }
          } else if (this.userRole === 'DISTRICTADMIN') {
            if (this.arrGrades.length > 0 || this.arrSubject.length > 0
              || this.arrTestTypes.length > 0) {
              return false;
            } else {
              return true;
            }
          }
          break;
        case 'school':
          if (this.userRole === 'PUBLISHER' || this.userRole === 'NETWORKADMIN') {
            if ((this.arrGrades.length > 0 || this.arrSubject.length > 0
              || this.arrTestTypes.length > 0) && this.selectedState > 0 &&
              this.selectedDistrict > 0 && this.selectedSchool > 0) {
              return false;
            } else {
              return true;
            }
          } else {
            if ((this.arrGrades.length > 0 || this.arrSubject.length > 0
              || this.arrTestTypes.length > 0) && this.selectedSchool > 0) {
              return false;
            } else {
              return true;
            }
          }
          break;
        case '0':
          return true;
          break;
        case 'class':
          if (this.arrGrades.length > 0 || this.arrSubject.length > 0 || this.arrTestTypes.length > 0) {
            return false;
          } else {
            return true;
          }
          break;
      }
    },
    showSelectClasses: function () {
      return this.selectedLevel === 'class'
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
    },
    addExcludeTestTypes: function () {
      for (var testType in this.testType) {
        if (!this.testType[testType].IsShow)
          this.excludeTestTypes.push(this.testType[testType].Id);
      }
      return this.excludeTestTypes;
    }
  },
  created: function () {
    var that = this;

    if (that.showDefaultOption) {
      that.tabActive = 1;
    }
    else if (that.showTestTypeOption) {
      that.tabActive = 2;
    }
    else {
      that.tabActive = 3;
    }

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
    }
    else if (that.userRole === that.roles.DISTRICTADMIN || that.userRole === that.roles.SCHOOLADMIN) {
      getSchoolByDistrictId(URL_SCHOOL + that.selectedDistrict, function (success) {
        var arr = [{ Id: 0, Name: 'Select School' }];
        that.selectedSchool = 0;
        if (success.length > 0) {
          if (success.length === 1) {
            that.school = success;
            that.selectedSchool = success[0].Id;
            that.selectSchool();
          } else {
            that.school = arr.concat(success);
            if (that.userRole === that.roles.SCHOOLADMIN)
              that.selectedSchool = success[0].Id;
            else
              that.selectedSchool = 0;
          }
        } else {
          that.school = arr;
        }
      });
    } else if (that.userRole === that.roles.TEACHER) {
      this.getAssociatedClasses();
    };
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
    selectedClassNames: function () {
      var selectedClassNames = $('#selectClasses').tagit("assignedTags");
      return selectedClassNames;
    },
    selectedClassIds: function () {
      if (this.selectedLevel === 'class') {
        var selectedClassNames = this.selectedClassNames();
        return this.associatedClasses.filter(function (item) {
          return selectedClassNames.some(function (className) {
            return className === item.ClassName;
          });
        }).map(function (item) {
          return item.ClassId;
        });;
      }
      return '';
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
      that.school = [];
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
      that.school = [];
      that.clearFillterValues();

      if (that.selectedDistrict > 0) {
        if (this.selectedLevel === 'district' || this.selectedLevel === 'enterprise') {
          this.getTestFilter();
        } else {
          getSchoolByDistrictId(URL_SCHOOL + that.selectedDistrict, function (success) {
            var arr = [{ Id: 0, Name: 'Select School' }];
            that.selectedSchool = 0;
            if (success.length > 0) {
              if (success.length === 1) {
                that.school = success;
                that.selectedSchool = success[0].Id;
                that.selectSchool();
              } else {
                that.school = arr.concat(success);
                that.selectedSchool = 0;
              }
            } else {
              that.school = arr;
            }
          });
        }
      }
    },
    selectLevel: function () {
      this.grades = [];
      this.arrGrades = [];
      this.subjects = [];
      this.arrSubject = [];
      this.testType = [];
      this.arrTestTypes = [];
      this.selectedState = '0';
      this.districts = [];
      this.selectedDistrict = '0';
      this.checkSubject = false;
      this.checkGrades = false;
      this.checkTestTypes = false;
      if (this.userRole !== this.roles.DISTRICTADMIN && this.userRole !== 'SCHOOLADMIN') {
        this.school = [];
        this.selectedSchool = '0';
      } else {
        if (this.selectedLevel === 'district') {
          $('#selectClasses').tagit("removeAll");
          this.selectedSchool = '0';
          this.getTestFilter();
        } else if (this.selectedLevel === 'school') {
          $('#selectClasses').tagit("removeAll");
          this.selectSchool();
        }
      }
      if (this.selectedLevel === 'class') {
        this.getAssociatedClasses();
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
    populateDataToTagIt: function (availableTagNames, tagitId, hint) {
      var self = this;
      $(tagitId).tagit({
        availableTags: availableTagNames,
        autocomplete: { delay: 0, minLength: 0 },
        placeholderText: hint,
        afterTagAdded: function () {
          self.triggerReloadTestFilters();
        },
        afterTagRemoved: function () {
          self.triggerReloadTestFilters();
        },
        beforeTagAdded: function (event, ui) {
          var selectedClassNames = self.selectedClassNames();
          var matchTags = _.filter(availableTagNames, function (value) {
            return value == ui.tagLabel
          });

          if (matchTags.length == 0) {
            matchTags = _.filter(availableTagNames, function (value) {
              return value.toLowerCase() == ui.tagLabel.toLowerCase()
            });
          }
          if (matchTags.length == 0) {
            matchTags = _.filter(availableTagNames, function (value) {
              if (selectedClassNames.some(function (item) {
                return item === value
              })) {
                return false;
              }
              return value.toLowerCase().startsWith(ui.tagLabel.toLowerCase());
            });
          }
          if (matchTags.length == 0) {
            return false;
          }
          else {
            if (ui.tagLabel === matchTags[0]) {
              return true;
            }
            else {
              $(tagitId).tagit("createTag", matchTags[0]);
              return false;
            }
          }
        }
      });
      $('ul.tagit input[type="text"]').css("min-width", "10px");
    },
    selectSchool: function () {
      this.clearFillterValues();
      if (this.selectedSchool > 0) {
        this.getTestFilter();
      }
    },
    allSelectTestTypes: function () {
      this.disabledFilter = false;
      this.arrTestTypes = [];
      if (this.checkTestTypes) {
        for (var item in this.testType) {
          if (this.testType[item].IsShow)
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
      $('.check-box-all').prop('checked', false);

      loadData();
    },
    onClearFilter: function () {
      this.clearFillterValues();
    },
    onSelectTab: function (tab) {
      this.tabActive = tab;
      this.$refs.showTestTypeOption.changeTabOption(tab === 2);
      this.$refs.showViewOption.changeTabOption(tab);
      this.$refs.modal.changeTab(tab);
    },
    changeOptionView: function (isShow, obj) {
      this.showViewOption = isShow;
      this.$refs.showViewOption.defaultOption(isShow, obj, this.tabActive);
    },
    allSelectSubjects: function () {
      this.arrSubject = [];
      if (this.checkSubject) {
        for (var item in this.subjects) {
          this.arrSubject.push(this.subjects[item].Name);
        }
      }
    },
    getTestFilter: function () {
      var that = this;
      that.getTestFilterId = that.getTestFilterId + 1;
      var currentId = that.getTestFilterId;
      that.showLoadingTest = true;
      that.showLoadingGrades = true;
      that.showLoadingSubjects = true;

      $.ajax(URL_TEST_TYPE_GRADE_SUBJECT + '?Level=' + that.selectedLevel + '&districtId=' + that.selectedDistrict + '&schoolId=' + that.selectedSchool + '&classIds=' + that.selectedClassIds())
        .done(function (response) {
          if (currentId !== that.getTestFilterId) {
            return;
          }
          if (response.IsSuccess === true) {
            that.testType = response.StrongData.TestTypes;
            that.grades = response.StrongData.Grades;
            that.subjects = response.StrongData.Subjects;
            that.showLoadingGrades = that.showLoadingTest = that.showLoadingSubjects = false;
          }
          else {
            alert(response.Message);
          }
        });
    },
    getPreferences: function (virtualtestid) {
      var that = this;
      var levelID = '';
      switch (this.selectedLevel) {
        case 'district':
          levelID = this.selectedDistrict;
          break;
        case 'school':
          levelID = this.selectedSchool;
          break;
      }
      var selectedClassIds = that.selectedClassIds() || [];
      var obj = {
        level: this.selectedLevel,
        levelID: levelID,
        virtualTestIds: virtualtestid,
        classIds: selectedClassIds.join()
      };
      getDefaultOption({ level: obj.level, levelID: obj.levelID, virtualTestIds: obj.virtualTestIds, classIds: obj.classIds }, function (res) {
        if (res) {
          that.showloading = false;
        }

        var groupDetails = {
          optionBS: [],
          default: [],
          testTypeID: this.tabActive == 2 ? res.TestTypeID : 0,
          levelID: res.LevelID,
          level: res.Level,
          studentPreferenceID: res.StudentPreferenceID,
          virtualTestIds: virtualtestid
        };
        var topDependItemDatas = ['Tags', 'Averages'];
        var botDependItemDatas = ['Item Analysis'];
        var subDependBotItemDatas = ['Time Spent', 'Item Detail Chart'];
        var dependLaunchItemAnalysisItems = [];
        var dependItemDetailChartItems = [];
        var dependItemDatas = topDependItemDatas.concat(botDependItemDatas).concat(subDependBotItemDatas);
        var dependGroups = [];
        $('#dataTable_wrapper').unblock();

        res.GroupDetails.forEach(function (element, index) {
          if (element.List) {
            element.List.forEach(function (item, i) {
              if (item.Name === "visibilityInTestSpecific")
                element.List.splice(i, 1);

              if (item.IsConflict) {
                item.Value = null;
                item.Locked = null;
              }
            });
          }
          if (dependItemDatas.indexOf(element.GroupName) > -1) {
            dependGroups.push(element);
            if (element.GroupName === subDependBotItemDatas[1]) {
              dependItemDetailChartItems.push(element);
            }
            if (element.GroupName === botDependItemDatas[0] || element.GroupName === subDependBotItemDatas[0]) {
              dependLaunchItemAnalysisItems.push(element);
            }
          }
          else {
            var group = {
              GroupName: element.GroupName,
              List: element.List,
              topDependentItems: [],
              botDependentItems: [],
              dependItemDetailChartItems: [],
              dependLaunchItemAnalysisItems: [],
              IsShow: true
            };

            groupDetails['optionBS'].push(group);
          }
        });

        var index = 0;
        for (var i = 0; i < groupDetails.optionBS.length; i += 1) {
          if (groupDetails.optionBS[i]["GroupName"] === "Item Data") {
            index = i;
          }
        }
        groupDetails['optionBS'][index].topDependentItems = dependGroups.filter(function (group) {
          return topDependItemDatas.indexOf(group.GroupName) != -1;
        });
        groupDetails['optionBS'][index].botDependentItems = dependGroups.filter(function (group) {
          return botDependItemDatas.indexOf(group.GroupName) != -1;
        });
        groupDetails['optionBS'][index].botDependentItems[0]['subDependBotItemDatas'] = dependGroups.filter(function (group) {
          return subDependBotItemDatas.indexOf(group.GroupName) != -1;
        });
        groupDetails['optionBS'][index].dependItemDetailChartItems = dependItemDetailChartItems;
        groupDetails['optionBS'][index].dependLaunchItemAnalysisItems = dependLaunchItemAnalysisItems.concat(dependItemDetailChartItems);

        var itemDataBS = groupDetails.optionBS.find(x => x.GroupName === "Item Data")
        if (itemDataBS && itemDataBS.List.some(x => !x.Value)) {
          var dependLaunchTopDependentItem = itemDataBS.topDependentItems.find(x => x.GroupName === "Tags");
          if (dependLaunchTopDependentItem) {
            for (var j = 0; j < dependLaunchTopDependentItem.List.length; j++) {
              dependLaunchTopDependentItem.List[j].IsDisabled = true;
            }
          }
        }

        that.groupDetails = groupDetails;
        that.groupDetails.classIds = selectedClassIds;
        that.showModal = true;
        that.$refs.modal.getOptions(that.showModal, that.groupDetails);
      });
    },
    getViewOption: function (virtualtestid) {
      var that = this;
      var levelID = '';
      switch (this.selectedLevel) {
        case 'district':
          levelID = this.selectedDistrict;
          break;
        case 'school':
          levelID = this.selectedSchool;
          break;
      }
      var obj = {
        level: this.selectedLevel,
        levelID: levelID,
        virtualTestIds: virtualtestid,
        classIds: (this.selectedClassIds() || []).join()
      };
      that.showview = true;
      that.rawhtml = '';
      that.showloadingview = true;
      that.$refs.viewModal.getViewOptions(that.showview, that.rawhtml, that.showloadingview);
      getMatrix({ level: obj.level, levelID: obj.levelID, virtualTestIds: obj.virtualTestIds, classIds: obj.classIds }, function (res) {
        that.rawhtml = res;
        that.showloadingview = false;
        that.$refs.viewModal.getViewOptions(that.showview, that.rawhtml, that.showloadingview);
      });
    },
    toggleModal: function (value) {
      this.onFilter();
      this.showModal = value;
    },
    toggleModalView: function (value) {
      this.showview = value;
    }
  }
});
