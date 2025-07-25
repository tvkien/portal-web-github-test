var LEVEL_DEFAULT_OPTION = [
  { value: '0', text: 'Select Level' },
  { value: 'enterprise', text: 'Enterprise' },
  { value: 'district', text: districtLabel },
  { value: 'school', text: 'School' }
]

var SECONDARY_LEVEL_DEFAULT_OPTION = [
  { value: '0', text: 'Select Level' },
  { value: 'district', text: districtLabel },
  { value: 'school', text: 'School' }
]

var USER_ROLES = 'TEACHER';
switch (rolesValue) {
  case '5': USER_ROLES = "PUBLISHER"; break;
  case '27': USER_ROLES = 'NETWORKADMIN';
    LEVEL_DEFAULT_OPTION = [
      { value: '0', text: 'Select Level' },
      { value: 'district', text: districtLabel },
      { value: 'school', text: 'School' }
    ];
    SECONDARY_LEVEL_DEFAULT_OPTION = [
      { value: '0', text: 'Select Level' },
      { value: 'district', text: districtLabel },
      { value: 'school', text: 'School' }
    ]
    break;
  case '3': USER_ROLES = 'DISTRICTADMIN';
    LEVEL_DEFAULT_OPTION = [
      { value: '0', text: 'Select Level' },
      { value: 'district', text: districtLabel },
      { value: 'school', text: 'School' }
    ];
    SECONDARY_LEVEL_DEFAULT_OPTION = [
      { value: '0', text: 'Select Level' },
      { value: 'district', text: districtLabel },
      { value: 'school', text: 'School' }
    ]
    break;
  case '8':
    USER_ROLES = 'SCHOOLADMIN';
    LEVEL_DEFAULT_OPTION = [
      { value: '0', text: 'Select Level' },
      { value: 'school', text: 'School' }
    ];
    SECONDARY_LEVEL_DEFAULT_OPTION = [
      { value: '0', text: 'Select Level' },
      { value: 'school', text: 'School' }
    ]
    break;
}

var TEST_TYPE = [
  { value: '0', text: 'Select Test Types' },
  { value: '1', text: 'Linkit' }
];
var template =
  '<div class="filter-wrapper"> \
  <h3 class="h3" v-show="!isShowTestTypes">Select Options Level </h3> \
  <div class="row g-3">\
  <div v-show="!isShowTestTypes" class="col-3 fileter-item"> \
    <label>Level</label> \
    <select v-model="levelDefault" v-on:change="selectLevel" class="full-width" id="levelDefault"> \
      <option v-for="item in level" :value="item.value">{{ item.text }}</option> \
    </select> \
  </div> \
  <h3 class="h3" v-show="isShowTestTypes">Select Test Type </h3> \
  <div v-show="isShowTestTypes" class="col-3 fileter-item"> \
    <label>Level</label> \
    <select v-model="secondaryLevelDefault" v-on:change="selectSecondary" class="full-width" id="levelDefault"> \
      <option v-for="item in secondaryLevel" :value="item.value">{{ item.text }}</option> \
    </select> \
  </div> \
  <div v-show="isShowTestTypes" class="col-3 fileter-item filter-item-comboTreeWrapper" v-bind:class="{order1: showSelectDistrict, order2: showSelectSchool}"> \
        <label>Category</label> \
        \
      <input type="text" id="dataSetCategoryId" class="full-width"  placeholder="Select Category" autocomplete="off" /> \
  </div> \
  <div v-show="showSelectState" class="col-3 fileter-item"> \
    <label>State</label> \
    <select v-model="stateDefaults"v-on:change="selectStates" class="full-width"> \
        <option v-for="item in states" :value="item.Id">{{item.Name}}</option> \
    </select> \
  </div> \
  <div v-show="showDistrictAdminRole" class="col-3 fileter-item"> \
    <label v-show="showSelectDistrict">{{districtLabel}}</label> \
    <div class="position-relative">\
      <div v-bind:class="setMarqueeStyle(showSelectDistrict)">\
          <div class="block-text-name w-100"> \
            <select id="selectedDistrictID" v-model="disrtrictsDefault" v-on:change="selectDistrict"  class="full-width selectedDistrictID-default"> \
                <option v-for="item in districts" :value="item.Id">{{item.Name}}</option> \
            </select> \
            <div class="box-select"> \
              <span class="overlay"></span> \
            </div>\
          </div>\
       </div>\
     </div> \
  </div> \
  <div  class="col-3 fileter-item filter-item-school"> \
    <label v-show="showSelectSchool">School</label> \
    <div class="position-relative">\
      <div v-bind:class="setMarqueeStyle(showSelectSchool)">\
        <div class="block-text-name w-100"> \
          <select id="selectedSchoolID" v-model="schoolsDefault"  class="full-width selectedSchoolID-default"> \
              <option v-for="item in schools" :value="item.Id">{{item.Name}}</option> \
          </select> \
          <div class="box-select"> \
            <span class="overlay"></span> \
          </div>\
        </div>\
      </div>\
     </div>\
  </div> \
  </div> \
  </div>';

var DefaultOption = Vue.component('default', {
  template: template,
  data: function () {
    return {
      levelDefault: LEVEL_DEFAULT_OPTION[0].value,
      testTypeDefault: '0',
      categoryIdDefault: 0,
      level: LEVEL_DEFAULT_OPTION,
      testTypes: TEST_TYPE,
      isShowTestTypes: false,
      secondaryLevelDefault: SECONDARY_LEVEL_DEFAULT_OPTION[0].value,
      secondaryLevel: SECONDARY_LEVEL_DEFAULT_OPTION,
      stateDefaults: '',
      states: [],
      disrtrictsDefault: '',
      districts: [],
      schoolsDefault: 0,
      selectDefaultSchoolId: 0,
      schools: [],
      userDefaults: '',
      users: [],
      userRole: USER_ROLES,
      roles: '',
      showSelectSchool: false,
      showSelectState: false,
      showSelectDistrict: false,
      levelID: '',
      flag: false,
      districtLabel: districtLabel,
      isChangeTab: false,
      firstLoad: false,
      showDistrictAdminRole: true
    }
  },
  created: function () {
    var that = this;
    if (this.userRole === 'PUBLISHER' || this.userRole === 'NETWORKADMIN') {
      getStates(URL_STATE, function (success) {
        var arr = [{ Id: 0, Name: 'Select State' }];
        if (success.length > 0) {
          if (success.length === 1) {
            that.states = success;
            that.stateDefaults = success[0].Id;
          } else {
            that.states = arr.concat(success);
            that.stateDefaults = 0;
          }
        } else {
          that.states = arr;
        }
      });
    }

    if (this.userRole === 'DISTRICTADMIN' || this.userRole === 'SCHOOLADMIN') {
      getSchoolByDistrictId(URL_SCHOOL + that.disrtrictsDefault, function (success) {
        var arr = [{ Id: 0, Name: 'Select School' }];
        that.schoolsDefault = 0;
        if (success.length > 0) {
          if (success.length === 1) {
            that.schools = success;
            that.schoolsDefault = that.selectDefaultSchoolId = success[0].Id;
          } else {
            that.schools = arr.concat(success);
            if (that.userRole === 'SCHOOLADMIN') {
              that.firstLoad = true;
              that.schoolsDefault = that.selectDefaultSchoolId = success[0].Id;
            }
          }
        } else {
          that.schools = arr;
        }
      });
    }

    if (this.userRole === 'SCHOOLADMIN')
      URL_TEST_TYPE = URL_TEST_TYPE + "&schoolId=-1";
    getListTestType(URL_TEST_TYPE, function (success) {
      var arr = [{ Value: 0, Text: 'Select Test Type' }];
      if (success.length > 0) {
        that.testTypes = arr.concat(success);
      } else {
        that.testTypes = arr;
      }
    });
  },
  methods: {
    selectStates: function (e) {
      var that = this;
      that.districts = [];
      that.school = [];
      that.schoolsDefault = 0;
      that.disrtrictsDefault = 0;
      if (that.stateDefaults > 0) {
        getDistrictByStateId(URL_DISTRICT + that.stateDefaults, function (success) {
          var arr = [{ Id: 0, Name: 'Select ' + districtLabel }];
          that.disrtrictsDefault = 0;
          if (success.length > 0) {
            if (success.length === 1) {
              that.districts = success;
              that.disrtrictsDefault = success[0].Id;
              GetDataSetCategories(that.disrtrictsDefault);
            } else {
              that.districts = arr.concat(success);
              that.disrtrictsDefault = 0;
              if (comboTree1) {
                comboTree1.setSource([]);
              }
            }
          } else {
            that.districts = arr;
          }
        });
      }
    },
    selectDistrict: function () {
      var that = this;
      that.schools = [];
      that.schoolsDefault = 0;
      GetDataSetCategories(that.disrtrictsDefault);
      if (that.disrtrictsDefault > 0 && this.levelDefault === 'school' || this.secondaryLevelDefault === 'school') {
        getSchoolByDistrictId(URL_SCHOOL + that.disrtrictsDefault, function (success) {
          var arr = [{ Id: 0, Name: 'Select School' }];
          that.schoolsDefault = 0;
          if (success.length > 0) {
            if (success.length === 1) {
              that.schools = success;
              that.schoolsDefault = success[0].Id;
            } else {
              that.schools = arr.concat(success);
              that.schoolsDefault = 0;
            }
          } else {
            that.schools = arr;
            that.schoolsDefault = 0;
          }
        });
      }
    },
    getOptionDefaults: function (value) {
      var levelID = '';
      if (this.isShowTestTypes) {
        switch (this.secondaryLevelDefault) {
          case 'district':
            levelID = this.disrtrictsDefault;
            break;
          case 'school':
            levelID = this.schoolsDefault;
            break;
        }
      }
      else {
        switch (this.levelDefault) {
          case 'district':
            levelID = this.disrtrictsDefault;
            break;
          case 'school':
            levelID = this.schoolsDefault;
            break;
        }
      }

      var obj = {
        level: this.isShowTestTypes ? this.secondaryLevelDefault : this.levelDefault,
        levelID: levelID,
        dataSetCategoryID: this.isShowTestTypes ? this.testTypeDefault : 0
      }
      this.$emit('changes-school', value, obj);
    },
    selectLevel: function () {
      this.stateDefaults = 0;
      this.disrtrictsDefault = 0;
      this.schoolsDefault = 0;
      this.districts = [];
      this.testTypeDefault = 0;
      this.flag = false;
      this.secondaryLevelDefault = this.secondaryLevel[0].value;
      if (this.userRole !== 'DISTRICTADMIN' && this.userRole !== 'SCHOOLADMIN') {
        this.schools = [];
        this.schoolsDefault = '0';
      } else {
        if (this.levelDefault === 'school' || this.levelDefault === 'types' && this.secondaryLevelDefault === 'school') {
          this.schoolsDefault = this.selectDefaultSchoolId;
          this.firstLoad = true;
        }
      }
    },
    selectSecondary: function () {
      this.stateDefaults = 0;
      this.disrtrictsDefault = 0;
      this.schoolsDefault = 0;
      this.flag = false;
      this.districts = [];
      if (this.userRole !== 'DISTRICTADMIN' && this.userRole !== 'SCHOOLADMIN') {
        this.schools = [];
        this.schoolsDefault = '0';
      } else {
        if (this.secondaryLevelDefault === 'school') {
          this.schoolsDefault = this.selectDefaultSchoolId;
        }
      }
    },
    selectTestType: function () {
      this.stateDefaults = 0;
      this.disrtrictsDefault = 0;
      this.schoolsDefault = 0;
      this.flag = false;
      this.secondaryLevelDefault = this.secondaryLevel[0].value;
      this.firstLoad = true;
    },
    changeTabOption: function (value) {
      this.flag = false;
      this.isShowTestTypes = value;
    },
    tryGetOptionDefault: function () {
      if (this.isShowTestTypes) {
        this.testTypeDefault = (comboTree1 !== null ? comboTree1._selectedItem.id : 0) || 0;
        switch (this.secondaryLevelDefault) {
          case 'district':
            this.flag = false;
            if (this.userRole === 'PUBLISHER' || this.userRole === 'NETWORKADMIN') {
              this.showSelectSchool = false;
              this.showSelectState = true;
              this.showSelectDistrict = true;
              if (this.disrtrictsDefault > 0 && this.testTypeDefault != 0) {
                this.getOptionDefaults(true);
              } else {
                this.getOptionDefaults(false);
              }
            } else if (this.userRole === 'DISTRICTADMIN') {
              if ((this.testTypeDefault && this.testTypeDefault > 0) || this.testTypeDefault == -1) {
                this.getOptionDefaults(true);
              }
              this.showSelectSchool = false;
              this.showSelectState = false;
              this.showSelectDistrict = false;
            } else {
              this.showSelectState = true;
              this.showSelectSchool = true;
              this.showSelectDistrict = true;
            }
            break;
          case 'school':
            if (this.userRole === 'PUBLISHER' || this.userRole === 'NETWORKADMIN') {
              this.showSelectDistrict = true;
              this.showSelectState = true;
            }
            else if (this.userRole === 'SCHOOLADMIN' && this.firstLoad) {
              this.schoolsDefault = this.selectDefaultSchoolId;
              this.firstLoad = false;
            }
            else {
              this.showSelectDistrict = false;
              this.showDistrictAdminRole = false;
              this.showSelectState = false;
            }
            this.showSelectSchool = true;
            this.flag = false;
            if (this.schoolsDefault > 0 && this.testTypeDefault != 0) {
              this.getOptionDefaults(true);
            } else {
              this.getOptionDefaults(false);
            }
            break;
          case '0':
            this.showSelectSchool = false;
            this.showSelectDistrict = false;
            this.showSelectState = false;
            this.flag = false;
            this.getOptionDefaults(false);
            break;
        }
      }
      else {
        switch (this.levelDefault) {
          case 'types':
            this.isShowTestTypes = true;

            break;
          case 'district':
            if (this.userRole === 'PUBLISHER' || this.userRole === 'NETWORKADMIN') {
              this.showSelectSchool = false;
              this.showSelectState = true;
              this.showSelectDistrict = true;
              if (this.disrtrictsDefault > 0) {
                this.getOptionDefaults(true);
              } else {
                this.getOptionDefaults(false);
              }
            } else if (this.userRole === 'DISTRICTADMIN') {
              this.getOptionDefaults(true);
              this.showSelectState = false;
              this.showSelectSchool = false;
              this.showSelectDistrict = false;
            } else {
              this.showSelectState = true;
              this.showSelectDistrict = true;
              this.showSelectSchool = true;
            }
            this.flag = false;
            this.isShowTestTypes = false;
            break;
          case 'school':
            if (this.userRole === 'PUBLISHER' || this.userRole === 'NETWORKADMIN') {
              this.showSelectDistrict = true;
              this.showSelectState = true;
            }
            else if (this.userRole === 'SCHOOLADMIN' && this.firstLoad) {
              this.schoolsDefault = this.selectDefaultSchoolId;
              this.firstLoad = false;
            }
            else {
              this.showSelectDistrict = false;
              this.showDistrictAdminRole = false;
              this.showSelectState = false;
            }
            if (this.schoolsDefault > 0) {
              this.getOptionDefaults(true);
            } else {
              this.getOptionDefaults(false);
            }
            this.showSelectSchool = true;
            this.flag = false;
            this.isShowTestTypes = false;
            break;
          case '0':
            this.showSelectSchool = false;
            this.flag = false;
            this.isShowTestTypes = false;
            this.showSelectDistrict = false;
            this.showSelectState = false;
            this.getOptionDefaults(false);
            break;
          case 'enterprise':

            this.showSelectSchool = false;
            this.isShowTestTypes = false;
            this.showSelectDistrict = false;
            this.showSelectState = false;
            if (!this.flag) {
              this.getOptionDefaults(true);
              this.flag = true;
            }
            break;
        }
      }
    },
    setMarqueeStyle: function (isShow) {
      var marqueeStyle = isShow ? 'w-100' : 'w-100 v-hidden';
      return marqueeStyle;
    },
  },
  updated: function () {
    this.$nextTick(this.tryGetOptionDefault);
  },
  mounted: function () {
    window.studentDefaultClass = this;
  }
});
