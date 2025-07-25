var url = window.location.protocol + '//' + window.location.host;
var URL_STATE = url + '/PopulateStateDistrict/GetStates',
    URL_STATE_NETWORKADMIN = url + '/PopulateStateDistrict/GetStatesForNetworkAdmin',
    URL_DISTRICT = url + '/PopulateStateDistrict/GetDistricts?stateId=',
    URL_DISTRICT_NETWORKADMIN = url + '/PopulateStateDistrict/GetDistrictsForNetworkAdmin?stateId=',
    URL_SCHOOL = url + '/RemoveTestResults/GetSchoolTestResultDistrict/',
    URL_CLASS = url + '/RemoveTestResults/GetClassBySchoolAndTerm/',
    URL_TEST = url + '/RemoveTestResults/GetTestBySchoolAndDistrictTerm/',
    URL_TERM = url + '/RemoveTestResults/GetTermTestResultDistrict/';
var _roles = {
    PUBLISHER: 5,
    NETWORKADMIN: 27,
    DISTRICTADMIN: 3,
    SCHOOLADMIN: 8
}

var appRemoveTestResults = new Vue({
    el: '#removeTestResults',
    data: {
        states: [],
        districts: [],
        selectedState: 0,
        selectedDisctricts: 0,
        selectedSchool: 0,
        selectedClass: 0,
        selectedTest: 0,
        selectedTerm: 0,
        selectedTimePeriod: 0,
        schoolName: ' ',
        roles: _roles,
        userRole: _roles.DISTRICTADMIN,
        districtLable: '',
        districtId: 0,
        currentDistrictId: 0,
        schools: [],
        classes: [],
        tests: [],
        terms: [],
        teacher: "",
        student: "",
        timePeriod: [
            { Id: 14, Name: '2 weeks' },
            { Id: 30, Name: '1 month' },
            { Id: 60, Name: '2 months' },
            { Id: 90, Name: '3 months' },
            { Id: 180, Name: '6 months' },
            { Id: 365, Name: '1 year' },
            { Id: 0, Name: 'All' }
        ],
        isClearFilter: false,
        termActive: 1
    },
    created: function () {
        var self = this;
        self.districtLable = districtLable;   //districtLable : "@LabelHelper.DistrictLabel"
        self.initData();
        this.selectedTimePeriod = self.timePeriod[4].Id;
    },
    methods: {
        initData: function () {
            var self = this;
            self.onFilterTestResults();
            self.loadDataByRoles();
        },
        loadDataByRoles: function () {
            var self = this;
            getCurrentUser(function (user) {
                self.userRole = user.roleId;
                self.districtId = user.districtId;
                if (user.roleId === _roles.NETWORKADMIN || user.roleId === _roles.PUBLISHER) {
                    if (user.roleId === _roles.PUBLISHER) {
                        self.loadStatesForPublisher();
                    }
                    if (user.roleId === _roles.NETWORKADMIN) {
                        self.loadStatesForNetworkAdmin();
                    }
                } else {
                    self.currentDistrictId = user.districtId;
                    self.loadSchoolOfDistrictAdmin();
                }
            });
        },
        selectStates: function () {
            var self = this;
            self.classes = [];
            self.tests = [];
            self.terms = [];
            self.districts = [];
            self.schools = [];
            self.selectedDisctricts = 0;
            self.selectedSchool = 0;
            self.selectedClass = 0;
            self.selectedTest = 0;
            self.selectedTerm = 0;
            self.student = "";
            self.teacher = "";
            if (this.selectedState !== 0) {
                if (this.userRole === _roles.PUBLISHER) {
                    self.loadDistrictOfStateForPublisher();
                }
                if (this.userRole === _roles.NETWORKADMIN) {
                    self.loadDistrictOfStateForNetworkAdmin();
                }
            }
        },
        selectDisctricts: function () {
            var self = this;
            self.classes = [];
            self.tests = [];
            self.terms = [];
            self.schools = [];
            self.selectedSchool = 0;
            self.selectedClass = 0;
            self.selectedTest = 0;
            self.selectedTerm = 0;

            if (this.selectedDisctricts !== 0) {
                self.districtId = this.selectedDisctricts;
                self.loadTestFilterChildren(self.selectedDisctricts, 0, 0, 0, 0);
            }
            self.checkEnableButton();
        },
        selectSchool: function () {
            var self = this;
            self.selectedClass = 0;
            self.selectedTest = 0;
            self.selectedTerm = 0;
            self.classes = [];
            self.tests = [];
            self.terms = [];

            self.checkEnableButton();
            self.loadChildrenHierarchy();
        },
        selectTerm: function() {
            var self = this;
            self.tests = [];
            self.classes = [];
            self.selectedTest = 0;
            self.selectedClass = 0;
            var districtId = self.selectedDisctricts;
            var schoolId = self.selectedSchool;
            var termId = self.selectedTerm;

            self.loadTestFilter(districtId, schoolId, termId);
            self.loadClassFilter(districtId, schoolId, termId);

            for (var index =0; index < self.terms.length; index++) {
                if (self.terms[index].Id == termId) {
                    self.termActive = self.terms[index].Active;
                    break;
                }
            }

            if (!self.termActive) {
                self.selectedTimePeriod = 0;
            }
        },
        loadStatesForPublisher: function () {
            var self = this;
            $.ajax(URL_STATE).done(function (states) {
                if (states.length === 1) {
                    self.states = states;
                } else if (states.length === 0) {
                    self.states = [{ Id: 0, Name: 'No Results Found' }];
                } else {
                    var arr = [{ Id: 0, Name: 'Select State' }];
                    self.states = arr.concat(states);
                }
            });
        },
        loadStatesForNetworkAdmin: function () {
            var self = this;
            $.ajax(URL_STATE_NETWORKADMIN).done(function (states) {
                if (states.length === 1) {
                    self.states = states;
                } else if (states.length === 0) {
                    self.states = [{ Id: 0, Name: 'No Results Found' }];
                } else {
                    var arr = [{ Id: 0, Name: 'Select State' }];
                    self.states = arr.concat(states);
                }
            });
        },
        loadSchoolOfDistrictAdmin: function () {
            var self = this;
            var districtId = this.districtId;
            $.ajax({
                url: URL_SCHOOL,
                data: { districtId: districtId, virtualTestId: 0, teacherId: 0, classId: 0, studentId: 0 },
                type: 'GET',
                success: function (school) {
                    if (school.length === 1) {
                        self.schools = school;
                        self.selectedSchool = school[0].Id;
                        self.selectSchool();
                    } else if (school.length === 0) {
                        self.schools = [{ Id: 0, Name: 'No Results Found' }];
                    }
                    else {
                        var arr = [{ Id: 0, Name: 'Select School' }];
                        self.schools = arr.concat(school);
                    }
                }
            });
        },
        loadDistrictOfStateForPublisher: function () {
            var self = this;
            if (this.selectedState !== 0) {
                var url = URL_DISTRICT + this.selectedState;
                $.ajax(url).done(function (districts) {
                    if (districts.length === 1) {
                        self.districts = districts;
                        self.selectedDisctricts = districts[0].Id;
                        self.selectDisctricts();
                    } else if (districts.length == 0) {
                        self.districts = [{ Id: 0, Name: 'No Results Found' }];
                    } else {
                        var arr = [{ Id: 0, Name: 'Select ' + districtLable }];
                        self.districts = arr.concat(districts);
                    }
                });
            }
        },
        loadDistrictOfStateForNetworkAdmin: function () {
            var self = this;
            if (this.selectedState !== 0) {
                var url = URL_DISTRICT_NETWORKADMIN + this.selectedState;
                $.ajax(url).done(function (districts) {
                    if (districts.length === 1) {
                        self.districts = districts;
                        self.selectedDisctricts = districts[0].Id;
                        self.selectDisctricts();
                    } else if (districts.length == 0) {
                        self.districts = [{ Id: 0, Name: 'No Results Found' }];
                    } else {
                        var arr = [{ Id: 0, Name: 'Select ' + districtLable }];
                        self.districts = arr.concat(districts);
                    }
                });
            }
        },
        loadTestFilterChildren: function (districtId, virtualTestId, teacherId, classId, studentId) {
            var self = this;
            this.school = [];
            this.selectedSchool = 0;

            $.ajax({
                url: URL_SCHOOL,
                data: { districtId: districtId, virtualTestId: virtualTestId, teacherId: teacherId, classId: classId, studentId: studentId },
                type: 'GET',
                success: function (school) {
                    if (school.length === 1) {
                        self.schools = school;
                        self.selectedSchool = school[0].Id;
                        self.selectSchool();
                    } else if (school.length === 0) {
                        self.schools = [{ Id: 0, Name: 'No Results Found' }];
                    }
                    else {
                        var arr = [{ Id: 0, Name: 'Select School' }];
                        self.schools = arr.concat(school);
                    }
                    self.checkEnableButton();
                }
            });
        },
        loadClassFilter: function (districtId, schoolId, termId) {
            var self = this;
            this.classes = [];
            this.selectedClass = 0;
            $("#pftClassSelect").show();
            $.ajax({
                url: URL_CLASS,
                data: { districtId: districtId, schoolId: schoolId, districtTermId: termId },
                type: 'GET',
                success: function (data) {
                    if (data.length === 1) {
                        self.classes = data;
                        self.selectedClass = data[0].Id;
                    } else if (data.length === 0) {
                        self.classes = [{ Id: 0, Name: 'No Results Found' }];
                    } else {
                        var arr = [{ Id: 0, Name: 'Select Class' }];
                        self.classes = arr.concat(data);
                    }
                }
            });
        },
        loadTestFilter: function (districtId, schoolId, termId) {
            var self = this;
            this.tests = [];
            this.selectedTest = 0;
            $("#pftTestSelect").show();
            $.ajax({
                url: URL_TEST,
                data: { districtId: districtId, schoolId: schoolId, districtTermId: termId },
                type: 'GET',
                success: function (data) {
                    if (data.length === 1) {
                        self.tests = data;
                        self.selectedTest = data[0].Id;
                    } else if (data.length === 0) {
                        self.tests = [{ Id: 0, Name: 'No Results Found' }];
                    } else {
                        var arr = [{ Id: 0, Name: 'Select Test' }];
                        self.tests = arr.concat(data);
                    }
                }
            });
        },
        loadTermFilter: function (districtId, virtualTestId, classId, studentId, schoolId, teacherId) {
            var self = this;
            this.terms = [];
            $("#pftTermSelect").show();

            $.ajax({
                url: URL_TERM,
                data: {
                    districtId: districtId, virtualTestId: virtualTestId, studentId: studentId,
                    schoolId: schoolId, classId: classId, teacherId: teacherId
                },
                type: 'GET',
                success: function (data) {
                    if (data.length === 1) {
                        self.terms = data;
                        self.selectedTerm = data[0].Id;
                        self.selectTerm();
                    } else if (data.length === 0) {
                        self.terms = [{ Id: 0, Name: 'No Results Found', Active: 1 }];
                    } else {
                        var arr = [{ Id: 0, Name: 'Select Term', Active: 1 }];
                        self.terms = arr.concat(data);
                    }
                }
            });
        },
        loadChildrenHierarchy: function () {
            var self = this;
            var districtId = self.getCurrentDistrictId();
            var virtualTestId = 0;
            var teacherId = 0;
            var classId = 0;
            var studentId = 0;
            var schoolId = self.selectedSchool;
            var termId = self.selectedTerm;

            if (self.selectedSchool !== 0) {
                self.loadClassFilter(districtId, schoolId, termId);
                self.loadTestFilter(districtId, schoolId, termId);
                self.loadTermFilter(districtId, virtualTestId, classId, studentId, schoolId, teacherId);
            }
        },
        checkEnableButton: function () {
            var self = this;
            if (self.selectedSchool !== 0) {
                $('#filterSheets').prop('disabled', false);
            } else {
                $('#filterSheets').prop('disabled', true);
            }
        },
        getCurrentDistrictId: function () {
            if (this.currentDistrictId !== 0) {
                return this.currentDistrictId;
            } else {
                return this.districtId;
            }
        },
        onClearFilter: function () {
            var self = this;
            this.classes = [];
            this.tests = [];
            this.terms = [];
            this.districts = [];
            this.schools = [];
            this.selectedDisctricts = 0;
            this.selectedSchool = 0;
            this.selectedClass = 0;
            this.selectedTest = 0;
            this.selectedTerm = 0;
            this.student = "";
            this.teacher = "";
            this.selectedTimePeriod = 180;
            this.selectedState = 0;
            this.isClearFilter = true;
            self.loadDataByRoles();
            $("#filterSheets").attr("disabled", "disabled");

        },
        onFilterTestResults: function () {
            var self = this;
            this.isClearFilter = false;
            var districtId = self.getCurrentDistrictId();
            var schoolId = this.selectedSchool;
            var virtualTestId = this.selectedTest;
            var classId = this.selectedClass;
            var termId = this.selectedTerm;
            var teacherName = this.teacher;
            var studentName = this.student;
            var timePeriod = this.selectedTimePeriod;
            if (self.selectedSchool === 0) {
                districtId = 0;
            }
            LoadTestReTagToView(districtId, virtualTestId, classId, studentName, schoolId, teacherName, termId, timePeriod);
        }
    },
});
