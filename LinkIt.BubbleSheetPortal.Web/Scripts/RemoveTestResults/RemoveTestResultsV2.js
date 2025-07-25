var url = window.location.protocol + '//' + window.location.host;
var URL_STATE = url + '/PopulateStateDistrict/GetStates',
	URL_STATE_NETWORKADMIN = url + '/PopulateStateDistrict/GetStatesForNetworkAdmin',
	URL_DISTRICT = url + '/PopulateStateDistrict/GetDistricts?stateId=',
	URL_DISTRICT_NETWORKADMIN = url + '/PopulateStateDistrict/GetDistrictsForNetworkAdmin?stateId=',
  URL_SCHOOL = url + '/RemoveTestResults/GetSchoolTestResultDistrict/',
	URL_CLASS = url + '/RemoveTestResults/GetClassBySchoolAndTermV2/',
	URL_TERM = url + '/RemoveTestResults/GetTermTestResultDistrictV2/',
  URL_CATEGORY_GRADE_SUBJECT = url + '/RemoveTestResults/GetCategoriesGradesAndSubjects/',
  DATE_FORMAT = 'MMM DD, YY',
  _roles = {
    PUBLISHER: 5,
    NETWORKADMIN: 27,
    DISTRICTADMIN: 3,
    SCHOOLADMIN: 8
  };


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
		selectedCategory: 0,
		selectedGrade: 0,
		selectedSubject: 0,
		selectedTimePeriod: 0,
		selectedSchools: [],
		selectedCategories: [],
		selectedGrades: [],
		selectedSubjects: [],
		schoolName: ' ',
		roles: _roles,
		userRole: _roles.DISTRICTADMIN,
		districtLable: '',
        termLabel: '',
		districtId: 0,
		currentDistrictId: 0,
		schools: [],
		categories: [],
		grades: [],
		subjects: [],
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
    isEmptyResultDateSelect: true,
    termActive: 1,
    fromResultDate: null,
    toResultDate: null,
    fromCreatedDate: null,
    toCreatedDate: null,
    fromUpdatedDate: null,
    toUpdatedDate: null,
    virtualTest: ''
	},
	created: function () {
		var self = this;
		self.districtLable = districtLable;   //districtLable : "@LabelHelper.DistrictLabel"
        self.termLabel = termLabel; // //termLabel : "@LabelHelper.Term"
		self.initData();
	},
	methods: {
		initData: function () {
			var self = this;
			self.onFilterTestResults();
      self.loadDataByRoles();

      self.initMultiSelectFilter('ftSchoolSelect', 'schools', 'selectedSchools');
      self.initMultiSelectFilter('ftCategorySelect', 'categories', 'selectedCategories');
      self.initMultiSelectFilter('ftGradeSelect', 'grades', 'selectedGrades');
      self.initMultiSelectFilter('ftSubjectSelect', 'subjects', 'selectedSubjects');

      self.initDateRangePickers();
    },
    initDateRangePickers() {
      var self = this;
      self.initResultDateSelect();
      self.initCreatedDateSelect();
      self.initModifiedDateSelect();
    },
    initResultDateSelect() {
      var self = this;
      $('#ftResultDateSelect').daterangepicker({
        autoUpdateInput: false,
        showDropdowns: true,
        linkedCalendars: false,
        startDate: moment().subtract(30, 'd'),
        endDate: moment(),
        locale: {
          format: DATE_FORMAT,
          cancelLabel: 'Clear'
        }
      });
      self.setDefaultResultDateRange();
      $('#ftResultDateSelect').on('apply.daterangepicker', function (ev, picker) {
        self.fromResultDate = picker.startDate;
        self.toResultDate = picker.endDate;
        $(this).val(picker.startDate.format(DATE_FORMAT) + ' - ' + picker.endDate.format(DATE_FORMAT));
      });
      $('#ftResultDateSelect').on('cancel.daterangepicker', function (ev, picker) {
        self.fromResultDate = null;
        self.toResultDate = null;
        picker.setStartDate({});
        picker.setEndDate({});
        $(this).val('');
      });
    },
    setDefaultResultDateRange() {
      var self = this;
      let from = moment().subtract(30, 'd');
      let to = moment();
      self.fromResultDate = from;
      self.toResultDate = to;
      $('#ftResultDateSelect').data('daterangepicker').setStartDate(from);
      $('#ftResultDateSelect').data('daterangepicker').setEndDate(to);
      $('#ftResultDateSelect').val(`${from.format(DATE_FORMAT)} - ${to.format(DATE_FORMAT)}`);
    },
    initCreatedDateSelect() {
      var self = this;
      $('#ftCreatedDateSelect').daterangepicker({
        autoUpdateInput: false,
        showDropdowns: true,
        linkedCalendars: false,
        locale: {
          format: DATE_FORMAT,
          cancelLabel: 'Clear'
        }
      });
      $('#ftCreatedDateSelect').on('apply.daterangepicker', function (ev, picker) {
        self.fromCreatedDate = picker.startDate;
        self.toCreatedDate = picker.endDate;
        $(this).val(picker.startDate.format(DATE_FORMAT) + ' - ' + picker.endDate.format(DATE_FORMAT));
      });
      $('#ftCreatedDateSelect').on('cancel.daterangepicker', function (ev, picker) {
        self.fromCreatedDate = null;
        self.toCreatedDate = null;
        picker.setStartDate({});
        picker.setEndDate({});
        $(this).val('');
      });
    },
    initModifiedDateSelect() {
      var self = this;
      $('#ftModifiedDateSelect').daterangepicker({
        autoUpdateInput: false,
        showDropdowns: true,
        linkedCalendars: false,
        locale: {
          format: DATE_FORMAT,
          cancelLabel: 'Clear'
        }
      });
      $('#ftModifiedDateSelect').on('apply.daterangepicker', function (ev, picker) {
        self.fromUpdatedDate = picker.startDate;
        self.toUpdatedDate = picker.endDate;
        $(this).val(picker.startDate.format(DATE_FORMAT) + ' - ' + picker.endDate.format(DATE_FORMAT));
      });
      $('#ftModifiedDateSelect').on('cancel.daterangepicker', function (ev, picker) {
        self.fromUpdatedDate = null;
        self.toUpdatedDate = null;
        picker.setStartDate({});
        picker.setEndDate({});
        $(this).val('');
      });
    },
		selectedTemplate: function (data) {
			var display = "";
			var allTexts = data.selected.map(x => x.text);
			if (allTexts.length > 1) {
				var othersCount = allTexts.length - 1;
				display = allTexts[0] + ` (+${othersCount} others)`;
			} else {
				display = allTexts[0];
			}
			return display;
		},
    loadDataByRoles: function () {
			var self = this;
			getCurrentUser(function (user) {
				self.userRole = user.roleId;
				self.districtId = user.districtId;
				if (user.roleId === _roles.NETWORKADMIN || user.roleId === _roles.PUBLISHER) {
					if (user.roleId === _roles.PUBLISHER) {
            self.loadStatesForPublisher();
            $('#ftCreatedDateContainer').removeClass('d-none');
            $('#ftModifiedDateContainer').removeClass('d-none');
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
			self.categories = [];
			self.districts = [];
			self.schools = [];
			self.selectedDisctricts = 0;
			self.selectedSchool = 0;
			self.selectedClass = 0;
			self.selectedTest = 0;
			self.selectedTerm = 0;
			self.selectedCategory = 0;
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
			clearMarqueeInput();
		},
		selectDisctricts: function () {
			var self = this;
			self.classes = [];
			self.tests = [];
			self.terms = [];
			self.categories = [];
			self.schools = [];
			self.selectedSchool = 0;
			self.selectedClass = 0;
			self.selectedTest = 0;
			self.selectedTerm = 0;
			self.selectedCategory = 0;

			if (this.selectedDisctricts !== 0) {
				self.districtId = this.selectedDisctricts;
        self.loadSchoolFilter(self.selectedDisctricts, 0, 0, 0, 0);
      }

			self.checkEnableButton();
		},
		selectSchool: function () {
			var self = this;
			self.selectedClass = 0;
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
			var schoolIds = self.selectedSchools;
			var termId = self.selectedTerm;

      if (termId > 0) {
        self.loadClassFilter(districtId, schoolIds, termId);
      }
      else {
        self.classes = [];
        self.selectedClass = 0;
      }

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
				success: function (schools) {
					if (schools.length === 1) {
						self.schools = schools;
						self.selectSchool();
					} else if (schools.length === 0) {
						self.schools = [{ Id: 0, Name: 'No Results Found' }];
					}
					else {
						var arr = [{ Id: -1, Name: 'Select All' }];
						self.schools = arr.concat(schools);
          }
          self.loadCategoriesGradesSubjects(districtId, String(schools.map(s => s.Id)));
          self.bindingMultiSelectData('ftSchoolSelect', 'schools');
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
		loadClassFilter: function (districtId, schoolIds, termId) {
			var self = this;
			this.classes = [];
      this.selectedClass = 0;
			$("#pftClassSelect").show();
			$.ajax({
				url: URL_CLASS,
        data: { districtId: districtId, schoolIds: schoolIds.join(), districtTermId: termId },
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
		loadTermFilter: function (districtId, virtualTestId, classId, studentId, schoolIds, teacherId) {
			var self = this;
			this.terms = [];
			$("#pftTermSelect").show();

			$.ajax({
				url: URL_TERM,
				data: {
					districtId: districtId, virtualTestId: virtualTestId, studentId: studentId,
					schoolIds: schoolIds.join(), classId: classId, teacherId: teacherId
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
						var arr = [{ Id: 0, Name: 'Select ' + termLabel, Active: 1 }];
						self.terms = arr.concat(data);
					}
				}
			});
    },
    initMultiSelectFilter(objectId, objectList, objectSelected) {
      let self = this;
      self.bindingMultiSelectData(objectId, objectList);
      $(`#${objectId}`).on("select2:select", e => {
        let id = e.params.data.id;
        if (id !== '-1') {
          var exist = self[objectSelected].some(function (x) {
            return x === id;
          });
          if (!exist) {
            self[objectSelected].push(id);
            if (self[objectSelected].length + 1 === self[objectList].length) {
              self[objectSelected].unshift('-1');
              $(`#${objectId}`).val(self[objectSelected]);
              $(`#${objectId}`).trigger('change.select2');
              $(`#select2-${objectId}-results > li`)[0].setAttribute('aria-selected', 'true');
            }
          }
        }
        else {
          self[objectSelected] = [...self[objectList].map(x => x.Id.toString())];
          $(`#${objectId}`).val(self[objectSelected]);
          $(`#${objectId}`).trigger('change.select2');
          $(`#select2-${objectId}-results > li`).each((index, item) => {
            item.setAttribute('aria-selected', 'true');
          });
        }

        if (objectId === 'ftSchoolSelect') {
          self.selectSchool();
        }
      });
      $(`#${objectId}`).on("select2:unselect", e => {
        var index = self[objectSelected].indexOf(e.params.data.id);
        if (index != -1) {
          if (e.params.data.id !== '-1') {
            self[objectSelected].splice(index, 1);
            if (self[objectSelected].includes('-1')) {
              self[objectSelected].splice(0, 1);
              $(`#${objectId}`).val(self[objectSelected]);
              $(`#${objectId}`).trigger('change.select2');
              $(`#select2-${objectId}-results > li`)[0].setAttribute('aria-selected', 'false');
            }
          }
          else {
            self[objectSelected] = [];
            $(`#${objectId}`).val([]);
            $(`#${objectId}`).trigger('change.select2');
            $(`#select2-${objectId}-results > li`).each((index, item) => {
              item.setAttribute('aria-selected', 'false');
            });
          }
        }

        if (objectId === 'ftSchoolSelect') {
          self.selectSchool();
        }
      });
      $(`#${objectId}`).on("select2:open", e => {
        $(document).on("keypress", function (e) {
          let k = e.keyCode;
          if (k >= 48 && k <= 57 || k >= 65 && k <= 90 || k >= 97 && k <= 122) {
            let elements = $(`#select2-${objectId}-results > li > span`);
            let targets = elements.filter((i, el) => el.innerText !== 'Select All' && el.innerText.toLowerCase().startsWith(e.key.toLowerCase()));
            if (targets.length > 0) {
              targets[0].scrollIntoView({ block: "nearest" });
            }
          }
        });
      });
      $(`#${objectId}`).on("select2:close", e => {
        $(document).unbind('keypress');
      });
    },
    bindingMultiSelectData(objectId, objectList) {
      $(`#${objectId}`).empty();
      $(`#${objectId}`).select2({
        data: this[objectList].map(item => { return { id: item.Id, text: item.Name } }),
        closeOnSelect: false,
        multiple: true,
        width: 'resolve',
        templateResult: this.formatAttachmentType,
        selectionAdapter: $.fn.select2.amd.require("CustomSelectionAdapter"),
        templateSelection: this.selectedTemplate
      })
        // select2 issue: https://github.com/select2/select2/issues/4417
        // https://stackoverflow.com/questions/40520293/select2-with-closeonselect-false-loses-scroll-position-on-select
        // these are to fix the issue
        .on('select2:selecting', e => $(e.currentTarget).data('scrolltop', $('.select2-results__options').scrollTop()))
        .on('select2:select', e => $('.select2-results__options').scrollTop($(e.currentTarget).data('scrolltop')))
        .on('select2:unselecting', e => $(e.currentTarget).data('scrolltop', $('.select2-results__options').scrollTop()))
        .on('select2:unselect', e => $('.select2-results__options').scrollTop($(e.currentTarget).data('scrolltop')));
    },
    clearMultiSelectFilter(objectId, objectList, objectSelected) {
      this[objectList] = [{ Id: 0, Name: 'No Results Found', Active: 1 }];
      this[objectSelected] = [];
      $(`#${objectId}`).empty();
    },
    loadSchoolFilter: function (districtId, virtualTestId, teacherId, classId, studentId) {
      var self = this;
      this.schools = [];
      this.selectedSchools = [];

      $.ajax({
        url: URL_SCHOOL,
        data: { districtId: districtId, virtualTestId: virtualTestId, teacherId: teacherId, classId: classId, studentId: studentId },
        type: 'GET',
        success: function (schools) {
          if (schools.length === 1) {
            self.schools = schools;
            self.selectSchool();
          } else if (schools.length === 0) {
            self.schools = [{ Id: 0, Name: 'No Results Found' }];
          }
          else {
            var arr = [{ Id: -1, Name: 'Select All' }];
            self.schools = arr.concat(schools);
          }
          self.loadCategoriesGradesSubjects(districtId, String(schools.map(s => s.Id)));
          self.checkEnableButton();
          self.bindingMultiSelectData('ftSchoolSelect', 'schools');
        }
      });
    },
    loadCategoriesGradesSubjects: function (districtId, schoolIds) {
      var self = this;
      this.categories = [];
      this.selectedCategories = [];
      this.grades = [];
      this.selectedGrades = [];
      this.subjects = [];
      this.selectedSubjects = [];

      $.ajax({
        url: URL_CATEGORY_GRADE_SUBJECT,
        data: {
          districtId: districtId,
          schoolIds: schoolIds
        },
        type: 'GET',
        success: function (data) {

          // Categiories
          let categories = data.Categories;
          if (categories.length === 1) {
            self.categories = categories;
          }
          else if (categories.length === 0) {
            self.categories = [{ Id: 0, Name: 'No Results Found', Active: 1 }];
          }
          else {
            var arr = [{ Id: -1, Name: 'Select All', Active: 1 }];
            self.categories = arr.concat(categories);
          }
          self.bindingMultiSelectData('ftCategorySelect', 'categories');

          // Grades
          let grades = data.Grades;
          if (grades.length === 1) {
            self.grades = grades;
          }
          else if (grades.length === 0) {
            self.grades = [{ Id: 0, Name: 'No Results Found', Active: 1 }];
          }
          else {
            var arr = [{ Id: -1, Name: 'Select All', Active: 1 }];
            self.grades = arr.concat(grades);
          }
          self.bindingMultiSelectData('ftGradeSelect', 'grades');

          // Subjects
          let subjects = data.Subjects;
          if (subjects.length === 1) {
            self.subjects = subjects;
          }
          else if (subjects.length === 0) {
            self.subjects = [{ Id: 0, Name: 'No Results Found', Active: 1 }];
          }
          else {
            var arr = [{ Id: -1, Name: 'Select All', Active: 1 }];
            self.subjects = arr.concat(subjects);
          }
          self.bindingMultiSelectData('ftSubjectSelect', 'subjects');
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
			var schoolIds = self.selectedSchools;

			if (self.selectedSchools.length !== 0) {
				self.loadTermFilter(districtId, virtualTestId, classId, studentId, schoolIds, teacherId);
			}
    },
    formatAttachmentType: function (item) {
			if (!item.id) { return item.text; }

			var originalText = item.text;
			var fileTypeGroupName = item.id;
			var html = "<span data-file-type-group-name ='" + fileTypeGroupName + "'>" + originalText + "</span>";

			return $(html);
		},
    selectedTemplate: function (data) {
			var display = "";
			var allTexts = data.selected.map(x => x.text);
			if (allTexts.length > 1) {
				var othersCount = allTexts.length - 1;
				display = allTexts[0] + ` (+${othersCount} others)`;
			} else {
				display = allTexts[0];
			}
			return display;
		},
    checkEnableButton: function () {
			var self = this;
      if (self.selectedSchools.length === 0) {
				$('#filterSheets').prop('disabled', true);
			} else {
				$('#filterSheets').prop('disabled', false);
			}
		},
		getCurrentDistrictId: function () {
			if (this.userRole === _roles.PUBLISHER || this.userRole === _roles.NETWORKADMIN) {
				return this.selectedDisctricts;
			}
			return this.districtId;
    },
    onClearFilter: function () {
      this.selectedState = 0;
      this.selectedDisctricts = 0;
      this.selectedTerm = 0;
      this.selectedClass = 0;
      this.student = "";
      this.teacher = "";
      this.fromCreatedDate = null;
      this.toCreatedDate = null;
      this.fromUpdatedDate = null;
      this.toUpdatedDate = null;
      this.virtualTest = "";
      
      this.clearMultiSelectFilter('ftSchoolSelect', 'schools', 'selectedSchools');
      this.clearMultiSelectFilter('ftCategorySelect', 'categories', 'selectedCategories');
      this.clearMultiSelectFilter('ftGradeSelect', 'grades', 'selectedGrades');
      this.clearMultiSelectFilter('ftSubjectSelect', 'subjects', 'selectedSubjects');

      this.setDefaultResultDateRange();
      $('#ftCreatedDateSelect').val('');
      $('#ftCreatedDateSelect').data('daterangepicker').setStartDate({});
      $('#ftCreatedDateSelect').data('daterangepicker').setEndDate({});
      $('#ftModifiedDateSelect').val('');
      $('#ftModifiedDateSelect').data('daterangepicker').setStartDate({});
      $('#ftModifiedDateSelect').data('daterangepicker').setEndDate({});

      this.districts = [];
			this.classes = [];
			this.terms = [];
			
			this.isClearFilter = true;
			this.loadDataByRoles();
			$("#filterSheets").attr("disabled", "disabled");

			var animationText = $('.overlay.animation-text');
			if (animationText.length > 0) {
				animationText.empty();
				var parentAnimationText = animationText.parent('.box-select');
				if (parentAnimationText.length > 0) {
					$(parentAnimationText).each(function (index) {
						if (!$(this).hasClass('short-text')) {
							$(this).addClass('short-text')
						}
					})
				}
			}
		},
		onFilterTestResults: function () {
			var self = this;
			this.isClearFilter = false;
			var districtId = self.getCurrentDistrictId();
			var schoolIds = String(this.selectedSchools.filter(s => s.Id !== '-1'));
      var categoryIds = String(this.selectedCategories.filter(s => s.Id !== '-1'));
      var gradeIds = String(this.selectedGrades.filter(s => s.Id !== '-1'));
      var subjectNames = String(this.subjects.filter(s => s.Id !== -1 && this.selectedSubjects.includes(s.Id.toString())).map(s => s.Name));
			var classId = this.selectedClass;
			var termId = this.selectedTerm;
			var teacherName = this.teacher;
      var studentName = this.student;
      var fromResultDate = this.fromResultDate ? this.fromResultDate.format('YYYY-MM-DD').concat('T00:00:00') : '';
      var toResultDate = this.toResultDate ? this.toResultDate.clone().add(1, 'd').format('YYYY-MM-DD').concat('T00:00:00') : '';
      var fromCreatedDate = this.fromCreatedDate ? this.fromCreatedDate.format('YYYY-MM-DD').concat('T00:00:00') : '';
      var toCreatedDate = this.toCreatedDate ? this.toCreatedDate.clone().add(1, 'd').format('YYYY-MM-DD').concat('T00:00:00') : '';
      var fromUpdatedDate = this.fromUpdatedDate ? this.fromUpdatedDate.format('YYYY-MM-DD').concat('T00:00:00') : '';
      var toUpdatedDate = this.toUpdatedDate ? this.toUpdatedDate.clone().add(1, 'd').format('YYYY-MM-DD').concat('T00:00:00') : '';
      var virtualTestName = this.virtualTest;
			if (self.selectedSchools.length === 0) {
				districtId = 0;
			}
      var isRemoveAll = $('#removeAllFlag').val();
      if (isRemoveAll === 'true') {
        LoadFilterRemoveTestResultV2(districtId, schoolIds, categoryIds, gradeIds, subjectNames, termId, classId, teacherName, studentName,
          fromResultDate, toResultDate, fromCreatedDate, toCreatedDate, fromUpdatedDate, toUpdatedDate, virtualTestName);
      }
      else {
        LoadTestReTagToView(districtId, schoolIds, categoryIds, gradeIds, subjectNames, termId, classId, teacherName, studentName,
          fromResultDate, toResultDate, fromCreatedDate, toCreatedDate, fromUpdatedDate, toUpdatedDate, virtualTestName);
      }
    },
    parseDateToString(date) {
      return `${date.getFullYear()}-${("0" + (date.getMonth() + 1)).slice(-2)}-${("0" + date.getDate()).slice(-2)}T00:00:00`;
    }
	},
	watch: {
		selectedClass: function () {
			$("#ftClassSelect").change();
    },
    selectedState: function () {
      this.clearMultiSelectFilter('ftSchoolSelect', 'schools', 'selectedSchools');
      this.clearMultiSelectFilter('ftCategorySelect', 'categories', 'selectedCategories');
      this.clearMultiSelectFilter('ftGradeSelect', 'grades', 'selectedGrades');
      this.clearMultiSelectFilter('ftSubjectSelect', 'subjects', 'selectedSubjects');
    },
    selectedDisctricts: function () {
      if (!this.selectedDisctricts) {
        this.clearMultiSelectFilter('ftSchoolSelect', 'schools', 'selectedSchools');
        this.clearMultiSelectFilter('ftCategorySelect', 'categories', 'selectedCategories');
        this.clearMultiSelectFilter('ftGradeSelect', 'grades', 'selectedGrades');
        this.clearMultiSelectFilter('ftSubjectSelect', 'subjects', 'selectedSubjects');
      }
    }
	}
});
