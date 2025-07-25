var url = window.location.protocol + '//' + window.location.host;
var URL_STATE = url + '/CategoriesAPI/GetStates',
  URL_DISTRICT = url + '/CategoriesAPI/GetDistrictByStateId?stateId=',
  URL_SCHOOL = url + '/CategoriesAPI/GetSchoolByDistrictId?districtId=',
  URL_TABLE_LOAD_DISTRICT_DETAIL = url + '/ManageClasses/LoadDistrictDetail?stateId=';

var _roles = {
  PUBLISHER: 5,
  NETWORKADMIN: 27,
  DISTRICTADMIN: 3,
  SCHOOLADMIN: 8
};

var app = new Vue({
  el: '#manageClass',
  data: {
    states: sessionStorage.getItem('states') === "" ? [] : JSON.parse(sessionStorage.getItem('states')),
    districts: [],
    selectedState: 0,
    selectedDistrictId: 0,
    schoolName: !sessionStorage.getItem('schoolName') ? ' ' : JSON.parse(sessionStorage.getItem('schoolName')),
    disabled: true,
    disabledFilter: true,
    roles: _roles,
    userRole: RoleID,
    selectedSchool: !sessionStorage.getItem('selectedSchool') ? 0 : sessionStorage.getItem('selectedSchool'),
    school: sessionStorage.getItem('school') === "" ? [] : JSON.parse(sessionStorage.getItem('school'))
  },
  created: function () {
    var that = this;
    var isSearchBefore = JSON.parse(sessionStorage.getItem('ManageSchoolAndClass'));
    var isApplyFilter = JSON.parse(sessionStorage.getItem('isApplyFilter'));
    if (!isSearchBefore) {
      sessionStorage.setItem('selectedState', 0);
      sessionStorage.setItem('selectedDistrictId', 0);
      sessionStorage.setItem('selectedSchool', 0);
      sessionStorage.setItem('schoolName', '');
      sessionStorage.setItem('states', []);
      sessionStorage.setItem('districts', []);
      sessionStorage.setItem('school', []);
      sessionStorage.setItem('pageSizeSchool', null);
      sessionStorage.setItem('pageSizeTerm', null);
      that.selectStates();
    }
    if (this.userRole === _roles.DISTRICTADMIN) {
      this.disabled = false;
      //this.selectedDistrictId = CurrentDistrictID;
      that.onDistrictChange(true, CurrentDistrictID);
      //console.log(CurrentDistrictID);
      //that.onFilterSchool(CurrentDistrictID); 
    }
    that.schoolName = sessionStorage.getItem('schoolName') ? JSON.parse(sessionStorage.getItem('schoolName')) : '';
    if (!sessionStorage.getItem('states')) {
      getStates(URL_STATE, function (states) {
        if (states.length === 1) {
          that.states = states;
          that.selectedState = states[0].id;
          that.selectStates();
        } else {
          var arr = [{ id: 0, name: 'Select State' }];
          that.states = arr.concat(states);
        }

      });
    } else {
      this.states = JSON.parse(sessionStorage.getItem('states'));
      this.selectedState = JSON.parse(sessionStorage.getItem('selectedState') ? sessionStorage.getItem('selectedState') : 0);
      if (this.selectedState !== 0) {
        this.disabledFilter = false;
      }
      if (sessionStorage.getItem('districts')) {
        that.disabled = false;
        this.districts = JSON.parse(sessionStorage.getItem('districts'));
        if (sessionStorage.getItem('selectedDistrictId')) {
          this.selectedDistrictId = JSON.parse(sessionStorage.getItem('selectedDistrictId'));
        }
        if (sessionStorage.getItem('schoolName')) {
          this.school = JSON.parse(sessionStorage.getItem('school'));
          if (sessionStorage.getItem('selectedSchool')) {
            this.selectedSchool = JSON.parse(sessionStorage.getItem('selectedSchool'));
          }
        }
      }
    }
    
  },
  methods: {
    selectStates: function () {
      var that = this;
      this.school = [];
      this.districts = [];
      sessionStorage.setItem('isApplyFilter', false)
      if (this.selectedState !== 0) {
        GetDistrictByStateId(URL_DISTRICT + this.selectedState, function (districts) {
          that.disabled = true;
          if (districts.length === 1) {
            that.districts = districts;
            that.selectedDistrictId = that.districts[0].id;
            that.disabled = false;
            that.onDistrictChange(false);
          } else {
            var arr = [{ id: 0, name: 'Select ' + DistrictLabel }];
            that.districts = arr.concat(districts);
            that.selectedDistrictId = 0;
          }
          that.disabledFilter = false;
        });
      } else {
        this.disabled = true;
        this.disabledFilter = true;
      }
      
    },
    onDistrictChange: function (isDictrictAdmin, _selectedDistrictId) {
      var that = this;
      this.school = [];
      sessionStorage.setItem('isApplyFilter', false)
      if (this.selectedDistrictId !== 0 || _selectedDistrictId) {
        var selectedSchoolId = JSON.parse(sessionStorage.getItem('selectedSchool')) === null ? 0 : JSON.parse(sessionStorage.getItem('selectedSchool'));
        this.selectedDistrictId = $("#selectDistrictOption").val() === '{{item.name}}' ? 0 : $("#selectDistrictOption").val();

        GetSchoolByDistrictId(URL_SCHOOL + (_selectedDistrictId ? _selectedDistrictId : this.selectedDistrictId), function (school) {
          if (school.length === 1) {
            that.school = school;
            that.selectedSchool = that.school[0].id;
            that.schoolName = that.school[0].name;
            that.selectSchool();
          } else {
            if (school.length === 0) {
              that.school = [];
            } else {
              var isApplyFilter = JSON.parse(sessionStorage.getItem('isApplyFilter'));
              var allSchool = { id: 0, name: 'All Schools' };
              school.splice(0, 0, allSchool);
              that.school = school;
              
              if (isApplyFilter || isDictrictAdmin) {
                that.selectedSchool = selectedSchoolId;
              }
              else {
                that.selectedSchool = 0;
                that.schoolName = '';
              }
            }

          }
        });
        that.disabled = false;
      } else {
        that.disabled = true;
        that.disabledFilter = true;
        if (that.selectedState) {
          that.disabledFilter = false;
        }
      }
    },
    selectSchool: function (e) {
      if (this.selectedSchool !== 0) {
        var options = e.target.options;
        var selected = options[options.selectedIndex];
        this.schoolName = selected.textContent;
      } else {
        this.schoolName = '';
      }
    },
    onFilterSchool: function () {
      if (this.userRole === _roles.DISTRICTADMIN) {
        this.selectedDistrictId = CurrentDistrictID;
      }
      sessionStorage.setItem('isApplyFilter', true);
      var isApplyFilter = JSON.parse(sessionStorage.getItem('isApplyFilter'));
      
      if (isApplyFilter) {
        sessionStorage.setItem('states', JSON.stringify(this.states));
        sessionStorage.setItem('districts', JSON.stringify(this.districts));
        sessionStorage.setItem('school', JSON.stringify(this.school));
        sessionStorage.setItem('schoolName', JSON.stringify(this.schoolName));
        sessionStorage.setItem('selectedDistrictId', JSON.stringify(this.selectedDistrictId));
        sessionStorage.setItem('selectedState', JSON.stringify(this.selectedState));
        sessionStorage.setItem('selectedSchool', JSON.stringify(this.selectedSchool));
        sessionStorage.setItem('ManageSchoolAndClass', true);
        sessionStorage.setItem('textSearch', '');
        sessionStorage.setItem('textTermSearch', '');
        sessionStorage.setItem('isApplyFilter', true);
      }

      if (this.userRole === _roles.DISTRICTADMIN && sessionStorage.getItem('school')) {
        this.school = JSON.parse(sessionStorage.getItem('school'));
        this.schoolName = JSON.parse(sessionStorage.getItem('schoolName'));
        this.selectedSchool = JSON.parse(sessionStorage.getItem('selectedSchool'));
      }

      if (sessionStorage.getItem('selectedDistrictId') && sessionStorage.getItem('schoolName')) {
        var districtId = JSON.parse(sessionStorage.getItem('selectedDistrictId')); 
        this.selectedDistrictId = districtId === '{{item.name}}' ? -1 : districtId;
        this.schoolName = JSON.parse(sessionStorage.getItem('schoolName'));
      }
      loadDistrictDetailData(this.selectedDistrictId , this.schoolName);
    },
    onClearFilter: function () {
      sessionStorage.setItem('isApplyFilter', false);
      if (this.userRole !== _roles.DISTRICTADMIN) {
        this.selectedDistrictId = 0;
      }

      this.districts = [];
      this.school = [];
      this.selectedSchool = 0;
      this.schoolName = '';
      sessionStorage.setItem('selectedDistrictId', 0);
      sessionStorage.setItem('textSearch', '');
      sessionStorage.setItem('textTermSearch', '');
      sessionStorage.setItem('selectedSchool', 0);
      sessionStorage.setItem('districts', []);
      sessionStorage.setItem('school', []);

      if (this.states.length === 1) {
        this.selectedDistrictId = 0;
        this.selectStates();
      } else {
        this.disabled = true;
        this.disabledFilter = true;
        this.selectedState = 0;
        sessionStorage.setItem('selectedState', 0);
      }
    }
  }
});
