
const ROLE_ENUM = {
  PUBLISHER: 5,
  NETWORK_ADMIN: 27,
  DISTRICT_ADMIN: 3,
  SCHOOL_ADMIN: 8,
  TEACHER: 2,
  STUDENT: 28
};
var filterNavigatorReportApp = new Vue({
  el: '#filterNavigatorReport',
  data: {
    districtLabel: "",
    breadCrumbs: [
      {
        reportIdentifier: "", name: "Home"
      }
    ],
    baseIdentifier: "",
    searchKeyword: "",
    searchInputDelayer: null,
    searchCancelled: false,
    selectedIdentifier: {},
    reportDetailDelayer: null,
    getReportDetailUrl: "NavigatorReport/GetNavigatorReportDetail",
    publishByRoleUrl: "NavigatorReport/PublishByRole",
    getFilterDownloadUrl: "NavigatorReport/GetFilterDownload",
    tryGetFileUrl: "/NavigatorReport/TryGetFile",
    getFileUrl: "/NavigatorReport/GetFile",
    getNavigatorConfigurationUrl: "NavigatorReport/GetNavigatorConfiguration",
    getRolesToPublishByNodePaths: "NavigatorReport/GetRolesToPublishByNodePaths",
    publishByRoleDelayer: null,
    reportDetailData: null,
    userCloseDetail: true,
    isloadingReportDetail: false,
    selectedIdentifiers: [],
    btnPublishingClicked: false,
    isloadingPublishByRole: false,
    rolesCanPublishInPublishByRole: [],
    arrSelectedRoleId: [],
    allRolesChecked: false,
    isShowModalBatchPrint: false,
    canPublish: false,
    districtTerms: [],
    checkedSingleFile: false,
    classes: [],
    classesByTerm: [],
    preventSearchAction: false,
    debounceTimeForSearchKeyword: 2000,
    selectedDistrictTerm: 0,
    selectedClass: 0,
    searchCompleted: false,
    dbClickDownload: false,
    publishingMultipleItems: false,
    tableSorting: [],
    roleId: 0,
    selectedStateId: 0,
    selectedDistrictId: 0,
    districtData: {
      loading: false,
      data: []
    },
    stateData: {
      loading: false,
      data: []
    },
    alsoSendEmail: false,
    customNote: "",
    emailTo: "",
    emailCC: "",
    notSendingTo: "",
    isNotSendEmail: false,
    history: {
      selectedStateId: 0,
      selectedDistrictId: 0,
    },
    appReady: false,
    students: [],
    selectedStudent: 0,
    showStudentList: false,
    allValidEmails: [],
    isEditEmails: true,
    checkRoleChange: false,
    hiddenEmailTo: [],
    isFirstTimeInit: false,
    isTypingAdd: false
  },
  created: function () {
    this.initData();
  },
  computed: {
    canPublish: function () {
      return [ROLE_ENUM.DISTRICT_ADMIN, ROLE_ENUM.PUBLISHER, ROLE_ENUM.NETWORK_ADMIN, ROLE_ENUM.SCHOOL_ADMIN].includes(this.roleId);
    },
    isPublisher: function () {
      return this.roleId == ROLE_ENUM.PUBLISHER;
    },
    isDistrictAd: function () {
      return this.roleId == ROLE_ENUM.DISTRICT_ADMIN;
    },
    isSchoolAd: function () {
      return this.roleId == ROLE_ENUM.SCHOOL_ADMIN;
    },
    canSendMail: function () {
      return this.roleId == ROLE_ENUM.PUBLISHER
        || this.roleId == ROLE_ENUM.NETWORK_ADMIN
        || this.roleId == ROLE_ENUM.DISTRICT_ADMIN
        || this.roleId == ROLE_ENUM.SCHOOL_ADMIN;
    },
    canPublishOnFocused: function () {
      return this.reportDetailData && !this.reportDetailData.hideManageAccessButton;
    },
    mustSelectDistrict: function () {
      return [ROLE_ENUM.PUBLISHER, ROLE_ENUM.NETWORK_ADMIN].includes(this.roleId);
    },
    filePublishedStatus: function () {
      var _fileDetail = this.reportDetailData.fileDetail;
      return _fileDetail.filePublishStatus;
    },
    showingDetail: function () {
      return (this.reportDetailData || this.isloadingReportDetail) && !this.userCloseDetail && this.selectedIdentifier != null;
    },
    showingFileDetail: function () {
      return this.reportDetailData && !!this.reportDetailData.fileDetail;
    },
    showingKeyword: function () {
      return this.reportDetailData
        && !!this.reportDetailData.fileDetail
        && (this.reportDetailData.fileDetail.primaryKeyword || '').length > 0;
    },
    showingFileDetailPublishTo: function () {
      return this.reportDetailData && !!this.reportDetailData.fileDetail && this.reportDetailData.fileDetail.hidePublishToSection == false;
    },
    showingSchoolFolderDetail: function () {
      return this.reportDetailData && !!this.reportDetailData.schoolFolderDetail;
    },
    showingReportDetailCountOnly: function () {
      return this.reportDetailData && !!this.reportDetailData.reportDetailCountOnly;
    },
    selectedItemIconClasses: function () {
      return {
        'icon': true,
        'icon_folder_custom_large': (this.selectedIdentifier.documentType == 'folder'),
        'icon_pdf_custom_large': (this.selectedIdentifier.documentType == 'pdf'),
        'icon_excel_custom_large': (this.selectedIdentifier.documentType == 'excel'),
        'icon_powerpoint_custom_large': (this.selectedIdentifier.documentType == 'powerpoint'),
      };
    },
    showingPublishByRole: function () {
      var self = this;
      var canShow = self.btnPublishingClicked === true && self.selectedIdentifiers && self.selectedIdentifiers.length > 0;
      if (!canShow) {
        self.btnPublishingClicked = false;
      }
      return canShow;
    },
    disablePublishByRolePopupButton: function () {
      var self = this;
      return self.selectedIdentifiers == null || self.selectedIdentifiers.length == 0 || !self.canPublish || !self.rolesCanPublishInPublishByRole || self.rolesCanPublishInPublishByRole.length <= 0;
    },
    disableDownloadButton: function () {
      var self = this;
      return self.selectedIdentifiers == null || self.selectedIdentifiers.length == 0;
    },
    disablePublishByRoleButton: function () {
      return this.arrSelectedRoleId == null || this.arrSelectedRoleId.length == 0;
    },
    showSearchResultBreadCrumbs: function () {
      return this.searchCompleted == true && this.searchKeyword != null && this.searchKeyword.length > 0;
    },
    disableSelectClass: function () {
      var self = this;
      return self.selectedDistrictTerm == null || self.selectedDistrictTerm == 0;
    },
  },
  watch: {
    alsoSendEmail: function () {
      if (this.alsoSendEmail && this.canSendMail) {
        filterNavigatorReportApp.setupEmailTagit("#emailTo", 'emailTo', 'Send to', 'availableEmailList');
        filterNavigatorReportApp.setupEmailTagit("#emailCC", 'emailCC', 'CC');
        filterNavigatorReportApp.setupEmailTagit("#notSendingTo", 'notSendingTo', 'Not Sending To', 'availableEmailList');
        this.reloadAssociateEmail();
      }
    },
    selectedStateId: function () {
      this.loadDataForDistrict();
    },
    selectedDistrictId: function () {
      if (this.mustSelectDistrict)
        this.reloadNavigatorDirectory();
    },
    selectedIdentifiers: function () {
      var self = this;
      self.reloadRolesForPublishByRole();
    },
    baseIdentifier: function (val, oldVal) {
      var self = this;
      self.reloadNavigatorDirectory();
      self.saveBrowseHistory();
    },
    searchKeyword: function () {
      var self = this;
      if (!self.preventSearchAction) {
        self.searchCompleted = false;
        self.searchCancelled = false;
        if (!self.searchInputDelayer) {
          self.searchInputDelayer = _.debounce(function () {
            if (!self.searchCancelled) {
              self.reloadNavigatorDirectory();
            }
          }, self.debounceTimeForSearchKeyword);
        }
        self.searchInputDelayer();
      }
    },
    btnPublishingClicked: function () {
      if (this.btnPublishingClicked && this.showingPublishByRole) {
        this.alsoSendEmail = false;
      }
    }
  },
  methods: {
    initData: function () {
      this.restoreBrowseHistory();
      this.loadDataForState();
    },
    finishAssignData: function () {
      this.appReady = true;
    },
    reloadAssociateEmail: function () {
      var self = this;
      $('#emailTo').tagit('removeAll');
      $('#notSendingTo').tagit('removeAll');
      self['availableEmailList'] = [];
      if (!self.alsoSendEmail || !self.canSendMail) {
        self.hiddenEmailTo = [];
        return;
      }
      if (!self.arrSelectedRoleId || self.arrSelectedRoleId.length == 0) {
        self.hiddenEmailTo = [];
        return;
      }
      ShowBlock($('#publishByRoleArticle'), "Loading emails");
      $.get('NavigatorReport/GetAssociateEmails', {
        nodePath: self.selectedIdentifiers.join('-_-'),
        selectedRoleIds: self.arrSelectedRoleId.join('-_-'),
        districtId: self.selectedDistrictId
      }, function (response) {
          self.isFirstTimeInit = true;
          $('#publishByRoleArticle').unblock();
          if (response.isSuccess && response.strongData) {

          var validEmails = response.strongData.filter(function (user) {
            return self.isCorrectEmail(user.email);
          });

            var _validEmailsDisplay = response.strongData.filter(function (user) {
              return (self.isCorrectEmail(user.email) && user.roleId != ROLE_ENUM.TEACHER && user.roleId != ROLE_ENUM.STUDENT);
            });

            self['availableEmailList'] = validEmails;
          $("#emailTo").tagit({
            availableTags: _validEmailsDisplay.map(function (tag) {
              return tag.displayName;
            })
          });

          $("#notSendingTo").tagit({
            availableTags: _validEmailsDisplay.map(function (tag) {
              return tag.displayName;
            })
          });

            if (self.isEditEmails) {
              _validEmailsDisplay.forEach(function (email) {
                $('#emailTo').tagit("createTag", email.displayName);
              });
            }
            else {
              self.emailTo = _validEmailsDisplay.map(x => x.displayName);
            }

            self.hiddenEmailTo = validEmails.filter(f => !_validEmailsDisplay.map(x => x.displayName).includes(f.displayName))
            .map(function (item) {
              return item.email;
            });
            self.isFirstTimeInit = false;
        }
      });
    },
    loadDataForDistrict() {
      var self = this;
      this.selectedDistrictId = 0;
      this.districtData.loading = false;
      this.districtData.data = [];
      $('#selectDistrict').parent('.block-text-name').find('.box-select').addClass('short-text');
      if (this.selectedStateId > 0) {
        this.districtData.loading = true;
        $.get('CategoriesAPI/GetDistrictByStateId', { stateId: this.selectedStateId }, function (response) {
          self.districtData.loading = false;
          self.districtData.data = response.data;

          if (self.history.selectedDistrictId > 0) {
            if (response.data.some(function (district) {
              return district.id == self.history.selectedDistrictId;
            })) {
              self.selectedDistrictId = self.history.selectedDistrictId;
            }
            self.history.selectedDistrictId = 0;
          }
        });
      }
    },
    loadDataForState() {
      this.stateData.loading = true;
      var self = this;
      $.get('CategoriesAPI/GetStates', {}, function (response) {
        self.stateData.loading = false;
        self.stateData.data = response.data;
        if (self.history.selectedStateId > 0) {
          if (response.data.some(function (state) {
            return state.id == self.history.selectedStateId;
          })) {
            self.selectedStateId = self.history.selectedStateId;
          }
          self.history.selectedStateId = 0;
        }
      });
    },
    forceSearch: function () {
      var self = this;
      self.searchCancelled = true;
      self.reloadNavigatorDirectory();
    },
    restoreBrowseHistory: function () {
      var self = this;
      if (sessionStorage.KEEP_SESSION) {
        var savedBrowseHistory = JSON.parse(sessionStorage.KEEP_SESSION);
        if (savedBrowseHistory && savedBrowseHistory.rootPage === _CURRENT_PAGE_NAME) {
          if (savedBrowseHistory && savedBrowseHistory.payload && savedBrowseHistory.payload.selectedStateId) {
            self.history.selectedStateId = savedBrowseHistory.payload.selectedStateId;
          }
          if (savedBrowseHistory && savedBrowseHistory.payload && savedBrowseHistory.payload.selectedDistrictId) {
            self.history.selectedDistrictId = savedBrowseHistory.payload.selectedDistrictId;
          }
          if (savedBrowseHistory && savedBrowseHistory.payload && savedBrowseHistory.payload.breadCrumbs) {
            self.breadCrumbs = savedBrowseHistory.payload.breadCrumbs;
          }
          if (savedBrowseHistory && savedBrowseHistory.payload && savedBrowseHistory.payload.baseIdentifier) {
            self.baseIdentifier = savedBrowseHistory.payload.baseIdentifier;
          }
          if (savedBrowseHistory && savedBrowseHistory.payload && savedBrowseHistory.payload.tableSorting) {
            self.tableSorting = savedBrowseHistory.payload.tableSorting;
          }
          if (savedBrowseHistory && savedBrowseHistory.payload && savedBrowseHistory.payload.searchKeyword) {
            self.setSearchKeywordWithoutReloadGrid(savedBrowseHistory.payload.searchKeyword);
          }

        }
      }
    },
    closeReportDetailPanel: function () {
      var self = this;
      self.userCloseDetail = true;
    },
    saveBrowseHistory: function () {
      var self = this;
      self.tableSorting = $('#dataTable').dataTable().fnSettings().aaSorting;
      var data = {
        rootPage: _CURRENT_PAGE_NAME,
        subPages: [_CURRENT_PAGE_NAME, "ManagePublishing"],
        payload: {
          breadCrumbs: self.breadCrumbs,
          baseIdentifier: self.baseIdentifier,
          selectedIdentifier: !self.selectedIdentifier ? '' : self.selectedIdentifier.identifier,
          selectedIdentifiers: self.selectedIdentifiers,
          publishingMultipleItems: self.publishingMultipleItems,
          tableSorting: self.tableSorting,
          searchKeyword: self.searchKeyword,
          selectedStateId: self.selectedStateId,
          selectedDistrictId: self.selectedDistrictId,
        }
      };

      sessionStorage.KEEP_SESSION = JSON.stringify(data);
    },
    loadReportDetail: function () {
      var self = this;
      self.isloadingReportDetail = true;
      self.reportDetailData = null;
      self.userCloseDetail = false;

      if (!self.reportDetailDelayer) {
        self.reportDetailDelayer = _.debounce(function () {
          if (self.selectedIdentifier && self.selectedIdentifier.identifier && self.selectedIdentifier.identifier.length > 0) {
            var _detailIdentifier = self.selectedIdentifier.identifier;
            $.get(self.getReportDetailUrl, { nodePath: self.selectedIdentifier.identifier, districtId: self.selectedDistrictId }, function (response) {
              if (self.selectedIdentifier && _detailIdentifier == self.selectedIdentifier.identifier) {
                self.reportDetailData = response;
                self.isloadingReportDetail = false;
                self.$nextTick(function () {
                  $('.with-tip-custom').tip();
                })
              }
            });
          }
          else {
            self.reportDetailData = null;
            self.isloadingReportDetail = false;
          }

        }, 400);
      }
      self.reportDetailDelayer();
    },
    reloadRolesForPublishByRole: function () {
      var self = this;
      self.isloadingPublishByRole = true;
      self.rolesCanPublishInPublishByRole = [];

      if (!self.publishByRoleDelayer) {
        self.publishByRoleDelayer = _.debounce(function () {
          if (self.selectedIdentifiers && self.selectedIdentifiers.length > 0) {
            var identifierConcated = self.selectedIdentifiers.join('-_-');
            $.get(self.getRolesToPublishByNodePaths, { nodePath: identifierConcated, districtId: self.selectedDistrictId }, function (response) {
              self.rolesCanPublishInPublishByRole = response;
              self.isloadingPublishByRole = false;
            });
          }
          else {
            self.rolesCanPublishInPublishByRole = [];
            self.isloadingPublishByRole = false;
          }


        }, 400);
      }
      self.publishByRoleDelayer();
    },
    managePublishingForSelectedReport: function (selectedMultipleItems) {
      var self = this;
      self.publishingMultipleItems = selectedMultipleItems;
      self.saveBrowseHistory();

      location.href = 'NavigatorReport/Publishing?districtId=' + self.selectedDistrictId;
    },
    reloadNavigatorDirectory: function () {
      var self = this;
      self.searchCancelled = true;
      self.selectedIdentifier = null;
      self.selectedIdentifiers = [];
      var datatable = $("#dataTable").dataTable();
      datatable.fnSettings().fnServerParams = function (aoData) {
        var filterParams = self.getFilterParams();

        filterParams.forEach(function (subParam) {
          aoData.push(subParam);
        });
      };
      datatable.fnFilter($("#dataTable_filter :input").val());
      self.searchCompleted = true;
    },
    filter: function () {
      var self = this;
      self.setupFilters(this);
    },
    setupFilters: function () {
      var dataTable = $('#dataTable').dataTable();
      var settings = dataTable.fnSettings();
      settings.oLanguage["sEmptyTable"] = "Your search returned no results. Try changing your selection and click on 'Apply Filters'";
      dataTable.fnDraw();
    },
    getFilterParams: function () {
      var self = this;
      return [
        { "name": "nodePath", "value": self.baseIdentifier },
        { "name": "districtId", "value": self.selectedDistrictId },
      ];
    },
    breadCrumbsClick: function (index) {
      var self = this;
      if (self.searchKeyword && self.searchKeyword.length > 0) {
        $("#dataTable_filter :input").val('');
        self.emptySearchWithoutReload();
      }
      if (index + 1 < self.breadCrumbs.length) {

        self.breadCrumbs = self.breadCrumbs.splice(0, index + 1);
        var breadCrumbsSelected = self.breadCrumbs[index].reportIdentifier;
        self.baseIdentifier = breadCrumbsSelected;

      } else {
        self.reloadNavigatorDirectory();
      }
    },
    emptySearchWithoutReload: function () {
      var self = this;
      self.setSearchKeywordWithoutReloadGrid(null);
    },
    publishing: function () {
      var self = this;
      $('INPUT[name="checkBoxReport"][type=checkbox]:checked').each(function (i, e) {
        self.selectedIdentifiers.push($(e).val());
      });
      var data = JSON.parse(sessionStorage.KEEP_SESSION);
      data.subPages = ["ManagePublishing"]
      data.selectedIdentifiers = self.selectedIdentifiers;
      sessionStorage.KEEP_SESSION = JSON.stringify(data);
      location.href = 'NavigatorReport/Publishing';
    },
    updateCheckedItems: function () {

      var self = this;
      self.selectedIdentifiers = [];
      $('INPUT[name="checkBoxReport"][type=checkbox]:checked').each(function (i, e) {
        self.selectedIdentifiers.push($(e).val());
      });
    },
    allRoleSelected: function () {
      this.checkRoleChange = true;
      this.arrSelectedRoleId = [];
      if (this.allRolesChecked) {
        for (var item in this.rolesCanPublishInPublishByRole) {
          this.arrSelectedRoleId.push(this.rolesCanPublishInPublishByRole[item].roleId);
        }
      }
      this.reloadAssociateEmail();
      this.checkRoleChange = false;
    },
    checkBoxRoleChangedV2: function (event) {
      this.checkRoleChange = true;
      var _checked = event.currentTarget.checked;
      var _value = event.currentTarget._value;
      if (_checked) {
        event.srcElement.parentNode.parentNode.style.backgroundColor = 'var(--blue3)';
        event.srcElement.parentNode.parentNode.style.color = 'var(--navyColor)';
        if (!this.arrSelectedRoleId.includes(_value))
          this.arrSelectedRoleId.push(_value);
      }
      else {
        event.srcElement.parentNode.parentNode.style.backgroundColor = '';
        event.srcElement.parentNode.parentNode.style.color = '';
        this.arrSelectedRoleId.$remove(_value);
      }
      var _allRoleChecked = this.rolesCanPublishInPublishByRole.length === this.arrSelectedRoleId.length;
      var _isCheckDistrictOrSchool = this.arrSelectedRoleId.indexOf(ROLE_ENUM.DISTRICT_ADMIN) != -1 || this.arrSelectedRoleId.indexOf(ROLE_ENUM.SCHOOL_ADMIN) != -1;
      var _isCheckTeacherOrStudent = this.arrSelectedRoleId.indexOf(ROLE_ENUM.TEACHER) != -1 || this.arrSelectedRoleId.indexOf(ROLE_ENUM.STUDENT) != -1;
      this.isEditEmails = (!(_value == ROLE_ENUM.TEACHER || _value == ROLE_ENUM.STUDENT) && !_isCheckDistrictOrSchool && !_allRoleChecked && !_isCheckTeacherOrStudent)
                          || _allRoleChecked
                          || _isCheckDistrictOrSchool;
      this.reloadAssociateEmail();
      this.allRolesChecked = _allRoleChecked
      this.checkRoleChange = false;
    },
    checkBoxRoleChanged: function (event) {
      this.checkRoleChange = true;
      var _checked = event.currentTarget.checked;
      var _value = event.currentTarget._value;
      if (_checked) {
        if (!this.arrSelectedRoleId.includes(_value))
          this.arrSelectedRoleId.push(_value);
      }
      else {
        this.arrSelectedRoleId.$remove(_value);
      }
      var _allRoleChecked = this.rolesCanPublishInPublishByRole.length === this.arrSelectedRoleId.length;
      var _isCheckDistrictOrSchool = this.arrSelectedRoleId.indexOf(ROLE_ENUM.DISTRICT_ADMIN) != -1 || this.arrSelectedRoleId.indexOf(ROLE_ENUM.SCHOOL_ADMIN) != -1;
      var _isCheckTeacherOrStudent = this.arrSelectedRoleId.indexOf(ROLE_ENUM.TEACHER) != -1 || this.arrSelectedRoleId.indexOf(ROLE_ENUM.STUDENT) != -1;
      this.isEditEmails = (!(_value == ROLE_ENUM.TEACHER || _value == ROLE_ENUM.STUDENT) && !_isCheckDistrictOrSchool && !_allRoleChecked && !_isCheckTeacherOrStudent)
        || _allRoleChecked
        || _isCheckDistrictOrSchool;
      this.reloadAssociateEmail();
      this.allRolesChecked = _allRoleChecked
      this.checkRoleChange = false;
    },
    publishSelectedItemsToSelectedRoles: function () {
      var self = this;
      var _notSendingTo = '';
      var _excludeUserIds = [];
      _notSendingTo = self.availableEmailList.filter(f => self.notSendingTo.includes(f.displayName))
        .map(function (item) {
          return item.email;
        });
      _excludeUserIds = self.availableEmailList.filter(f => self.notSendingTo.includes(f.displayName))
        .map(function (item) {
          return item.userId;
        });
      var _emailTo = self.availableEmailList.filter(f => self.emailTo.includes(f.displayName))
        .map(function (item) {
          return item.email;
        });
      if (self.hiddenEmailTo.length > 0)
        _emailTo = _emailTo.concat(this.hiddenEmailTo);
      var _customNote = self.customNote ? JSON.stringify(self.customNote).slice(1, -1) : '';
      var publishByRoleData = {
        roleIds: self.arrSelectedRoleId,
        nodePaths: self.selectedIdentifiers,
        alsoSendEmail: self.alsoSendEmail,
        emailTo: _emailTo,
        customNote: _customNote,
        districtId: self.selectedDistrictId,
        notSendingTo: _notSendingTo,
        isNotSendingTo: self.isNotSendingTo,
        excludeUserIds: _excludeUserIds.join(',')
      };
      ShowBlock($('#publishByRoleArticle'), "Publishing");
      $.ajax({
        type: "POST",
        url: self.publishByRoleUrl,
        contentType: "application/json",
        data: JSON.stringify(publishByRoleData),
        success: function (result, status, xhr) {
          //don't forget to hide progress bar
          if (result.status && result.status.toLowerCase() == 'error' || !result.strongData) {
            $('#publishByRoleArticle').unblock();
            self.hidePublishDialog();
            customAlertMessage({ message: result.message });
          }
          else {
            $('#publishByRoleArticle').unblock();
            self.hidePublishDialog();
            var totalPublishedReports = result.strongData.totalRelatedReportCount;
            customAlertMessage({ message: (totalPublishedReports + " reports have been published successfully") })
          }
          self.loadReportDetail();
        },
        error: function (xhr, status, error) {
          $('#publishByRoleArticle').unblock();
          self.loadReportDetail();
          customAlertMessage({ message: error });
        }
      });

    },
    confirmBatchPrint: function () {
      var self = this;
      var $topNavigation = $('#dataTable');

      if (self.selectedIdentifiers.length == 1 || self.dbClickDownload) {
        var isfile = (this.selectedIdentifiers.filter(function (item) {
          return item.toLowerCase().indexOf('NavigatorReportId'.toLowerCase()) > -1
        }).length == 1) || self.dbClickDownload;
        if (isfile) {
          var selectedIdentifiers;
          if (self.dbClickDownload) {
            selectedIdentifiers = self.selectedIdentifier.identifier;
          }
          else {
            selectedIdentifiers = self.selectedIdentifiers.join('-_-');
          }
          ShowBlock($topNavigation, 'Downloading Report');
          $.get(self.getNavigatorConfigurationUrl, { nodePath: selectedIdentifiers, districtId: self.selectedDistrictId }, function (response) {
            var useClass = response.useClass;
            var useStudent = response.useStudent;
            if (useClass || useStudent) {
              $.get(self.getFilterDownloadUrl, { nodePath: selectedIdentifiers, districtId: self.selectedDistrictId }, function (response) {
                if (response.students) {
                  if (response.students.length > 1) {
                    var defaultStudentValue = { id: 0, name: 'Download All Students' };

                    $topNavigation.unblock();
                    self.isShowModalBatchPrint = true;
                    self.showStudentList = true;
                    response.students.unshift(defaultStudentValue);
                    self.students = response.students;
                  }
                  else {
                    self.generatePdfBatchPrint();
                  }

                }
                else {
                  var defaultTermValue = { id: 0, name: 'Download All Classes' };

                  if (response.classes.length > 1) {
                    $topNavigation.unblock();

                    self.isShowModalBatchPrint = true;
                    self.showStudentList = false;
                    response.districtTerms.unshift(defaultTermValue);
                    self.districtTerms = response.districtTerms;
                    self.classes = response.classes;
                  }
                  else {
                    self.generatePdfBatchPrint();
                  }
                }

              });
            }
            else {
              self.generatePdfBatchPrint();
            }
          });
        }
        else {
          ShowBlock($topNavigation, 'Downloading Report');
          self.generatePdfBatchPrint();
        }
      }
      else {
        self.generatePdfBatchPrint();
      }
    },
    closeBatchPrint: function () {
      var self = this;
      self.isShowModalBatchPrint = false;
      self.districtTerms = [];
      self.classes = [];
      self.classesByTerm = [];
      self.selectedDistrictTerm = 0;
      self.disableDownloadButton = false;
      self.selectedClass = 0;
      self.dbClickDownload = false;
    },
    generatePdfBatchPrint: function () {
      var self = this;

      var $topNavigation = $('#dataTable');
      if (self.isShowModalBatchPrint)
        ShowBlock($('#filterDonwload'), "Downloading");
      else
        ShowBlock($topNavigation, "Downloading");
      var selectedIdentifiers;
      if (self.dbClickDownload) {
        selectedIdentifiers = self.selectedIdentifier.identifier;
      }
      else {
        selectedIdentifiers = self.selectedIdentifiers.join('-_-');
      }
      var nodePath = encodeURIComponent(selectedIdentifiers);
      var url = self.tryGetFileUrl + '?nodePath=' + nodePath + '&classId=' + self.selectedClass + '&studentId=' + self.selectedStudent + '&districtId=' + self.selectedDistrictId;

      $.ajax(url).done(function (res) {
        if (res.status === "error") {
          if (self.isShowModalBatchPrint) {
            $('#filterDonwload').unblock();
            $('#popup-error-messages').html('<li>' + res.message + ' </li>');
            $('#popup-error-messages').show();
            setTimeout(function () {
              $('#popup-error-messages').hide();
            }, 5000);
          }
          else {
            self.dbClickDownload = false;
            $topNavigation.unblock();
            $('#error-messages').html('<li>' + res.message + ' </li>');
            $('#error-messages').show();
            setTimeout(function () {
              $('#error-messages').hide();
            }, 5000);
          }
        }
        else {

          var url = self.getFileUrl + '?nodePath=' + nodePath + '&classId=' + self.selectedClass + '&studentId=' + self.selectedStudent + '&districtId=' + self.selectedDistrictId;

          var href = document.location.origin + url;
          window.open(href, '_blank');
          $topNavigation.unblock();
          $('#filterDonwload').unblock();
          if (!self.isShowModalBatchPrint)
            self.dbClickDownload = false;
        }
      });
    },
    selectDistrictTerm: function () {
      var self = this;
      var defaultClassValue = {
        districtTermId: 0,
        id: 0,
        name: "Select Class"
      };
      var results = self.classes.filter(function (item) {
        return (item.districtTermId === self.selectedDistrictTerm);
      });
      if (results.length > 0)
        results.unshift(defaultClassValue);
      self.classesByTerm = results;
      self.selectedClass = 0;
    },
    setSearchKeywordWithoutReloadGrid(newKeyword) {
      var self = this;
      self.preventSearchAction = true;
      self.searchKeyword = newKeyword;
      self.$nextTick(function () {
        self.preventSearchAction = false;
      });
    },
    getNewSearchKeyword: function () {
      var self = this;
      var currentValue = $("#dataTable_filter :input").val();
      if (currentValue != self.searchKeyword) {
        self.searchKeyword = currentValue;
      }
    },
    replaceSearchFunction: function () {
      var self = this;
      $("#dataTable_filter :input").unbind();
      if ($("#dataTable_filter :input").val() != self.searchKeyword) {
        $("#dataTable_filter :input").val(self.searchKeyword);
      }
      $("#dataTable_filter :input").change(function () {
        self.getNewSearchKeyword();
      });
      $("#dataTable_filter :input").keyup(function (event) {
        var keycode = (event.keyCode ? event.keyCode : event.which);
        if (keycode == '13') {
          self.forceSearch();
        }
        else {
          self.getNewSearchKeyword();
        }
      });
    },
    resetPublishByRoleStatus: function () {
      var self = this;
      self.allRolesChecked = false;
      self.allRoleSelected();
    },
    btnPublishingByRoleClicked: function () {
      var self = this;
      if (!self.btnPublishingClicked) {
        self.resetPublishByRoleStatus();
        self.isNotSendingTo = false;
        self.customNote = '';
        $('#publishByRoleArticle').dialog({
          title: "",
          modal: false,
          width: 600,
          resizable: false,
          close: function () {
            $("body .my-overlay").remove();
          }
        });
        $("body").append('<div class="my-overlay" style="z-index: ' + ($.ui.dialog.currentZ() - 1) + ';width:' + $(document).width() + 'px;height:' + $(document).height() + 'px;background-color: black;opacity: 0.3;position: absolute;top:0px;left: 0px;"></div>');
        $(".ui-dialog-titlebar-close").hide();
        self.btnPublishingClicked = true;
      }
      else {
        self.hidePublishDialog();
      }
    },
    hidePublishDialog: function () {
      var self = this;
      self.btnPublishingClicked = false;
      $("#publishByRoleArticle").dialog("close");
      $("body .my-overlay").remove();
    },
    setupEmailTagit: function (id, vueValueKey, hint, useSuggestListFieldName) {
      var self = this;
      $(id).tagit({
        availableTags: [],
        autocomplete: { delay: 0, minLength: 0 },
        placeholderText: hint,
        afterTagAdded: function () {
          var tags = $(id).tagit("assignedTags");
          self[vueValueKey] = tags;
          $(id).change();
        },
        afterTagRemoved: function () {
          var tags = $(id).tagit("assignedTags");
          self[vueValueKey] = tags;
          $(id).change();
        },
        beforeTagRemoved: function (event, ui) {
          if (self.checkRoleChange || self.isTypingAdd) return;

          var itemEmail = _.find(self.emailTo, function (item) {
            return item === ui.tagLabel
          });
          var itemNoSend = _.find(self.notSendingTo, function (item) {
            return item === ui.tagLabel
          });
          if (!itemEmail) {
            $('#emailTo').tagit("createTag", ui.tagLabel);
          }
          if (!itemNoSend) {
            $('#notSendingTo').tagit("createTag", ui.tagLabel);
          }
        },
        beforeTagAdded: function (event, ui) {
          if (!useSuggestListFieldName) {
            return !!self.isCorrectEmail(ui.tagLabel);
          }
          else {
            var availableTags = self[useSuggestListFieldName];

            availableTags = availableTags.filter(function (tag) {
              return !self[vueValueKey] || !self[vueValueKey].some(function (kv) { return kv === tag });
            })

            if (!useSuggestListFieldName) {
              return false;
            }
            var matchTags = _.filter(availableTags, function (value) {
              return value.displayName == ui.tagLabel
            });
            if (matchTags.length == 0) {
              matchTags = _.filter(availableTags, function (value) {
                return value.displayName.startsWith(ui.tagLabel);
              });
            }
            if (matchTags.length == 0 || !self.isCorrectEmail(matchTags[0])) {
              return false;
            }

            $(ui.tag).attr('title', matchTags[0].email);
            ui.tag.value = matchTags[0].email;

            self.allValidEmails = self[useSuggestListFieldName];

            if (ui.tagLabel === matchTags[0].displayName) {
              if (!self.isFirstTimeInit) {
                self.isTypingAdd = true;
                self.removeTag(id, ui.tagLabel);
                self.isTypingAdd = false;
              }
              return true;
            }
            else {
              $(id).tagit("createTag", matchTags[0].displayName);
              return false;
            }
          }
        }
      });
      $('ul.tagit input[type="text"]').css("min-width", "10px");
    },
    removeTag: function (id, tagLabel) {
      if (id == '#emailTo') {
        $('#notSendingTo').tagit("removeTagByLabel", tagLabel);
      }
      else {
        $('#emailTo').tagit("removeTagByLabel", tagLabel);
      }

    },
    isCorrectEmail: function (email) {
      if (!email)
        return false;
      email = typeof (email) === 'object' ? email.email : email;
      var mailformat = /^\w+([\.-]?\w+)*@\w+([\.-]?\w+)*(\.\w{2,3})+$/;
      return !!email.match(mailformat)
    },
  }
});
