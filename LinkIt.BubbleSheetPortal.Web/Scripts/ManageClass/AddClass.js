var url = window.location.protocol + '//' + window.location.host;
var URL_STATE = url + '/PopulateStateDistrict/GetStates',
  URL_STATE_NETWORKADMIN = url + '/PopulateStateDistrict/GetStatesForNetworkAdmin',
  URL_DISTRICT = url + '/PopulateStateDistrict/GetDistricts?stateId=',
  URL_DISTRICT_NETWORKADMIN = url + '/PopulateStateDistrict/GetDistrictsForNetworkAdmin?stateId=',
  URL_SCHOOL = url + '/Admin/GetSchools/',
  URL_SCHOOL_DISTRICTADMIN = url + '/ManageClasses/GetSchoolsByUser',
  URL_TEACHER = url + '/ManageClasses/GetTeacherBySchool',
  URL_TERM = url + '/ManageClasses/GetTermDistrict',
  URL_ClassType = url + '/ManageClasses/GetClassType',
  URL_POST = url + '/ManageClasses/AddClass';

var _roles = {
  PUBLISHER: 5,
  NETWORKADMIN: 27,
  DISTRICTADMIN: 3,
  SCHOOLADMIN: 8,
  TEACHER: 2
}

var DropDownListName = {
  STATE: "state",
  DISTRICT: "district",
  SCHOOL: "school",
  TEACHER: "teacher"
}

var AddClassModel = new Vue({
  el: '#addNewClassSchool',
  data: {
    states: [],
    districts: [],
    schools: [],
    teachers: [],
    terms: [],
    classTypes: [],
    selectedState: 0,
    selectedDistrict: 0,
    selectedSchool: 0,
    selectedTeacher: 0,
    selectedTerm: 0,
    selectedClassType: 0,
    courseName: '',
    section: '',
    courseNumber:'',
    roles: _roles,
    userRole: _roles.DISTRICTADMIN,
    districtLable: '',
    districtId: 0,
    currentDistrictId: 0,
    userId: 0,
    errors: [],
    _historyData: {}
  },
  created: function () {
    var self = this;
    self.districtLable = districtLable;   //districtLable : "@LabelHelper.DistrictLabel"
    self.initData();
  },
  methods: {
    handleSubmit: function () {
      var self = this;
      this.errors = [];

      if (this.courseName == '') {
        this.errors.push('COURSE IS REQUIRED.')
      }

      if (this.userRole === _roles.NETWORKADMIN || this.userRole === _roles.PUBLISHER) {
        if (this.selectedState == 0) {
          this.errors.push('STATE IS REQUIRED.')
        }
        if (this.selectedDistrict == 0) {
          this.errors.push('DISTRICT IS REQUIRED.')
        }
      }

      if (this.selectedSchool == 0) {
        this.errors.push('SCHOOL IS REQUIRED.')
      }

      if (this.selectedTeacher == 0) {
        this.errors.push('TEACHER IS REQUIRED.')
      }

      if (this.selectedTerm == 0) {
        this.errors.push('TERM IS REQUIRED.')
      }

      if (this.selectedClassType == 0) {
        this.errors.push('CLASS TYPE IS REQUIRED.')
      }

      if (this.errors.length > 0) {
        $('#error-messages').show();
      } else {
        $('#error-messages').hide();
        self.submitForm();
      }

    },
    initData: function () {
      var self = this;
      self.loadDataByRoles();
      self.loadClassType();
      self.loadHistoryData();
    },
    submitForm: function () {
      var self = this;
      $('#btnSubmit').disableBt();
      var token = $('input[name="__RequestVerificationToken"]').val();

      var model = {
        Course: this.courseName,
        Section: this.section,
        CourseNumber: this.courseNumber,
        SchoolId: this.selectedSchool,
        TeacherId: this.selectedTeacher,
        DistrictTermId: this.selectedTerm,
        ClassTypeId: this.selectedClassType,
        DistrictId: self.getCurrentDistrictId(),
        __RequestVerificationToken: token,
      };

      $.ajax({
        type: 'POST',
        url: URL_POST,
        data: model,
        success: function (response) {
          if (response.Success != false) {
            location.href = response.RedirectUrl;
          }
          else {
            addErrorMessages(response);
            $('#success-message').hide();
            $('#btnSubmit').enableBt();
          }
        },
        failure: function (response) {
          alert(response);
        }
      });
    },
    loadDataByRoles: function () {
      var self = this;
      getCurrentUser(function (user) {
        self.userRole = user.roleId;
        self.districtId = user.districtId;
        self.userId = user.id;
        if (user.roleId === _roles.NETWORKADMIN || user.roleId === _roles.PUBLISHER) {
          if (user.roleId === _roles.PUBLISHER) {
            self.loadStatesForPublisher();
          }
          if (user.roleId === _roles.NETWORKADMIN) {
            self.loadStatesForNetworkAdmin();
          }
        } else {
          self.currentDistrictId = user.districtId;
          self.loadSchool();
          self.loadTerm();
        }
      });
    },
    selectState: function () {
      var self = this;
      self.clearDataDropdownlist(DropDownListName.STATE);
      if (this.selectedState !== 0) {
        if (this.userRole === _roles.PUBLISHER) {
          self.loadDistrictOfStateForPublisher();
        }
        if (this.userRole === _roles.NETWORKADMIN) {
          self.loadDistrictOfStateForNetworkAdmin();
        }
      }
    },
    selectDistrict: function () {
      var self = this;
      self.clearDataDropdownlist(DropDownListName.DISTRICT);
      if (this.selectedDistrict !== 0) {
        self.districtId = this.selectedDistrict;
        self.loadSchool();
        self.loadTerm();
      }
    },
    selectSchool: function () {
      var self = this;
      self.clearDataDropdownlist(DropDownListName.SCHOOL);
      if (this.selectedSchool !== 0) {
        self.loadTeacher();
      }
    },
    loadStatesForPublisher: function () {
      var self = this;
      if (!self._historyData) self._historyData = {};
      $.ajax(URL_STATE).done(function (states) {
        if (states.length === 1) {
          self.states = states;
          self.selectedState = !self._historyData.stateID ? states[0].Id : self._historyData.stateID;
          self.selectState();
        } else if (states.length === 0) {
          self.states = [{ Id: 0, Name: 'No Results Found' }];
        } else {
          var arr = [{ Id: 0, Name: 'Select State' }];
          self.states = arr.concat(states);
          self.selectedState = !self._historyData.stateID ? states[0].Id : self._historyData.stateID;
        }
      });
    },
    loadStatesForNetworkAdmin: function () {
      var self = this;
      if (!self._historyData) self._historyData = {};
      $.ajax(URL_STATE_NETWORKADMIN).done(function (states) {
        if (states.length === 1) {
          self.states = states;
          self.selectedState = !self._historyData.stateID ? states[0].Id : self._historyData.stateID;
          self.selectState();
        } else if (states.length === 0) {
          self.states = [{ Id: 0, Name: 'No Results Found' }];
        } else {
          var arr = [{ Id: 0, Name: 'Select State' }];
          self.states = arr.concat(states);
          self.selectedState = !self._historyData.stateID ? states[0].Id : self._historyData.stateID;
        }
      });
    },
    loadDistrictOfStateForPublisher: function () {
      var self = this;
      if (!self._historyData) self._historyData = {};
      if (this.selectedState !== 0) {
        var url = URL_DISTRICT + this.selectedState;
        $.ajax(url).done(function (districts) {
          if (districts.length === 1) {
            self.districts = districts;
            self.selectDisctricts = !self._historyData.districtID ? districts[0].Id : self._historyData.districtID;
            self.selectDisctricts();
          } else if (districts.length == 0) {
            self.districts = [{ Id: 0, Name: 'No Results Found' }];
          } else {
            var arr = [{ Id: 0, Name: 'Select ' + districtLable }];
            self.districts = arr.concat(districts);
            self.selectedDistrict = !self._historyData.districtID ? districts[0].Id : self._historyData.districtID;
          }
        });
      }
    },
    loadDistrictOfStateForNetworkAdmin: function () {
      var self = this;
      if (!self._historyData) self._historyData = {};
      if (this.selectedState !== 0) {
        var url = URL_DISTRICT_NETWORKADMIN + this.selectedState;
        $.ajax(url).done(function (districts) {
          if (districts.length === 1) {
            self.districts = districts;
            self.selectDisctricts = !self._historyData.districtID ? districts[0].Id : self._historyData.districtID;
            self.selectDisctricts();
          } else if (districts.length == 0) {
            self.districts = [{ Id: 0, Name: 'No Results Found' }];
          } else {
            var arr = [{ Id: 0, Name: 'Select ' + districtLable }];
            self.districts = arr.concat(districts);
            self.selectedDistrict = !self._historyData.districtID ? districts[0].Id : self._historyData.districtID;
          }
        });
      }
    },
    loadSchool: function () {
      var self = this;
      if (!self._historyData) self._historyData = {};
      var url = '';
      var districtId = self.getCurrentDistrictId();
      if (this.userRole == _roles.TEACHER || this.userRole == _roles.SCHOOLADMIN) {
        url = URL_SCHOOL_DISTRICTADMIN;
      } else {
        url = URL_SCHOOL;
      }
      $.ajax({
        url: url,
        data: { districtId: districtId, userId: this.userId },
        dataType: 'json',
        type: 'GET',
        success: function (schools) {
          if (schools.length === 1) {
            self.schools = schools;
            self.selectedSchool = !self._historyData.schoolID ? schools[0].Id : self._historyData.schoolID;
            self.selectSchool();
          } else if (schools.length === 0) {
            self.schools = [{ Id: 0, Name: 'No Results Found' }];
          }
          else {
            var defaultValue = { Id: 0, Name: 'Select School' };
            schools.splice(0, 0, defaultValue);
            self.schools = schools;
            self.selectedSchool = !self._historyData.schoolID ? schools[0].Id : self._historyData.schoolID;
          }
        }
      });
    },
    loadTeacher: function () {
      var self = this;
      if (!self._historyData) self._historyData = {};
      var districtId = self.getCurrentDistrictId();
      $.ajax({
        url: URL_TEACHER,
        data: { schoolId: this.selectedSchool, districtId: districtId },
        type: 'GET',
        success: function (data) {
          if (data.length === 1) {
            self.teachers = data;
            self.selectedTeacher = !self._historyData.teacherID ? data[0].Id : self._historyData.teacherID;
          } else if (data.length === 0) {
            self.teachers = [{ Id: 0, Name: 'No Results Found' }];
          }
          else {
            var arr = [{ Id: 0, Name: 'Select Teacher' }];
            self.teachers = arr.concat(data);
            self.selectTeacher = !self._historyData.teacherID ? data[0].Id : self._historyData.teacherID;
          }
        }
      });
    },
    loadTerm: function () {
      var self = this;
      var districtId = self.getCurrentDistrictId();

      if (districtId !== 0) {
        $.ajax({
          url: URL_TERM,
          data: { districtId: districtId },
          type: 'GET',
          success: function (data) {
            if (data.length === 1) {
              self.terms = data;
              self.selectedTerm = data[0].Id;
            } else if (data.length === 0) {
              self.terms = [{ Id: 0, Name: 'No Results Found' }];
            }
            else {
              var arr = [{ Id: 0, Name: 'Select Term' }];
              self.terms = arr.concat(data);
            }
          }
        });
      }
    },
    loadClassType: function () {
      var self = this;
      $.ajax(URL_ClassType).done(function (data) {
        if (data.length === 1) {
          self.classTypes = data;
          self.selectedClassType = data[0].Id;
        } else if (data.length === 0) {
          self.classTypes = [{ Id: 0, Name: 'No Results Found' }];
        } else {
          var arr = [{ Id: 0, Name: 'Select Class Type' }];
          self.classTypes = arr.concat(data);
        }
      });
    },
    getCurrentDistrictId: function () {
      if (this.currentDistrictId !== 0) {
        return this.currentDistrictId;
      } else {
        return this.districtId;
      }
    },
    clearDataDropdownlist: function (value) {
      switch (value) {
        case DropDownListName.STATE:
          this.districts = [];
          this.schools = [];
          this.teachers = [];
          this.terms = [];
          this.selectecDistrict = 0;
          this.selectedSchool = 0;
          this.selectedTeacher = 0;
          this.selectedTerm = 0;
        case DropDownListName.DISTRICT:
          this.schools = [];
          this.teachers = [];
          this.terms = [];
          this.selectedSchool = 0;
          this.selectedTeacher = 0;
          this.selectedTerm = 0;
        case DropDownListName.SCHOOL:
          this.teachers = [];
          this.selectedTeacher = 0;
      }
    },
    loadHistoryData: function () {
      var data = localStorage.getItem("FilterData");
      if (data === undefined)
        return null;

      this._historyData = JSON.parse(data);
    }

  },
  watch: {
    selectedState: function () {
      this.selectState();
    },
    selectedDistrict: function () {
      this.selectDistrict();
    },
    selectedSchool: function () {
      this.selectSchool();
    }
  }

});
