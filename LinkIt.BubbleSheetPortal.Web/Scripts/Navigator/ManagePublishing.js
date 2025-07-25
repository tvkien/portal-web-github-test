var managePublishing = new Vue({
  el: '#managePublishing',
  data: {
    userIds: [],
    navigatorReportPublishRequest: {
      navigatorReportIds: null,
      userIds: null,
      publishDistrictAdmin: false,
      publishSchoolAdmin: false,
      publishTeacher: false,
      publishStudent: false,
      selectedIdentifier: '',
      districtId: 0
    },
    navigatorReportUnpublishRequest: {
      navigatorReportId: 0,
      userIds: [],
      districtId: 0
    },
    programs: [],
    grades: [],
    programIds: [],
    gradeIds: [],
    studentSelected: false,
    classroomSelected: false,
    schoolSelected: false,
    districtSelected: false,
    isDisable: false,
    isDisableFilter: true,
    canPublishSchoolAdmin: false,
    canPublishDistrictAdmin: false,
    canPublishTeacher: false,
    canPublishStudent: false,
    currentRoleId: global.currentRoleId,
    districtId: global.districtId,
    showDatatable: false,
    roles: {
      SCHOOLADMIN: 8,
      DISTRICTADMIN: 3,
      NETWORKADMIN: 27,
      PUBLISHER: 5
    },
    manage: $(".filter-by-program"),
    publishDialog: {
      showingPublishDialog: false,
      isloadingPublishDialog: false,
      publishDetail: [],
      alsoSendEmail: false,
      customNote: '',
    }

  },
  computed: {
    publishDialogCanPublish: function () {
      return this.publishDialog.publishDetail && this.publishDialog.publishDetail.length > 0;
    }
  },
  created: function () {
    this.initData();
    this.manage.attr('disabled', 'disabled');
  },
  watch: {
    studentSelected: function (newValue) {
      if (this.studentSelected) {
        this.isDisable = false;
        this.manage.removeAttr('disabled');
        this.manage.css('opacity', '1');
      } else {
        this.isDisable = true;
        this.clearCheckedBox();
        var $filter = $('#filterByProgram');
        $filter.hide();
        $filter.removeClass('expand-content');
        $(".manage-class-legend").removeClass('arrow-expand');
        this.manage.attr('disabled', 'disabled');
        this.manage.css('opacity', '0.5');

      }
      this.disableFilter();
    },
    classroomSelected: function () {
      this.disableFilter();
    },
    schoolSelected: function () {
      this.disableFilter();
    },
    districtSelected: function () {
      this.disableFilter();
    }
  },
  methods: {
    initData: function () {
      this.navigatorReportPublishRequest = {
        navigatorReportId: global.navigatorReportId,
        userIds: [],
        districtId: 0
      };

      this.navigatorReportUnpublishRequest = {
        navigatorReportId: global.navigatorReportId,
        userIds: [],
        districtId: 0
      }
      this.manage.css('opacity', '0.5');
      this.isDisable = true;
      $(".manage-class-legend").removeClass('arrow-expand');

      this.getNavigatorConfiguration();
    },
    disableFilter: function () {
      if (!this.classroomSelected && !this.studentSelected && !this.schoolSelected && !this.districtSelected) {
        this.isDisableFilter = true;
      } else {
        this.isDisableFilter = false;
      }
    },
    clearAllCheckBox: function () {
      this.clearCheckedBox();
      this.studentSelected = false;
      this.classroomSelected = false;
      this.schoolSelected = false;
      this.districtSelected = false;
    },
    clearCheckedBox: function () {
      for (var i = 0; i < this.programs.length; i++) {
        var program = this.programs[i];
        if (program && program.programSelected) {
          program.programSelected = false;
        }
      }

      for (var i = 0; i < this.grades.length; i++) {
        var grade = this.grades[i];
        if (grade && grade.gradeSelected) {
          this.gradeIds.push(grade.id);
        }
      }
    },
    applyFilter: function () {
      this.showDatatable = true;
      $('#dataTable').dataTable().fnSettings().sAjaxSource = this.getAjaxSource();
      $('#dataTable').dataTable().fnDraw();
    },
    showPublishedToStaff: function (e) {
      e.preventDefault();
      this.changeInactiveUser();
    },
    changeInactiveUser: function () {
      var $self = $('.js-change-published');

      if ($self.hasClass('show-inactive')) {
        showPublished = false;
        $self.removeClass('show-inactive');
        $self.find('span').html('Off');
        this.refreshDataTable();
      } else {
        showPublished = true;
        $self.addClass('show-inactive');
        $self.find('span').html('On');
        this.refreshDataTable();
      }
    },
    refreshDataTable: function () {
      var url = this.getAjaxSource();
      fillDataTableWithoutCustomMessage($("#dataTable"), url);
    },
    getAjaxSource: function () {
      if (showPublished) {
        var $self = $('.js-change-published');
        $self.addClass('show-inactive');
        $self.find('span').html('On');
        $('#flexSwitchCheckDefault').attr('checked', showPublished);
      }
      this.programIds = [];
      this.gradeIds = [];
      if (this.studentSelected) {
        for (i = 0; i < this.programs.length; i++) {
          var program = this.programs[i];
          if (program && program.programSelected == true) {
            this.programIds.push(program.programID);
          }
        }
        for (i = 0; i < this.grades.length; i++) {
          var grade = this.grades[i];
          if (grade && grade.gradeSelected == true) {
            this.gradeIds.push(grade.gradeID);
          }
        }
      }
      else {
        this.programIds = [];
        this.gradeIds = [];
      }
      var data = JSON.parse(sessionStorage.KEEP_SESSION);
      var identifier = '';
      if (data && data.payload.publishingMultipleItems) {
        identifier = data.payload.selectedIdentifiers.join('-_-');
      }
      else {
        identifier = data.payload.selectedIdentifier;
      }
      return getAssociateUserUrl + '?nodePath=' + encodeURIComponent(identifier) + '&isPublished=' + showPublished + '&canPublishStudent=' + this.studentSelected + '&canPublishTeacher=' + this.classroomSelected + '&canPublishSchoolAdmin=' + this.schoolSelected + '&canPublishDistrictAdmin=' + this.districtSelected + '&programIds=' + this.programIds.join(',') + '&gradeIds=' + this.gradeIds.join(',') + '&districtId=' + this.districtId + '&exludeCurrentUser=true';
    },
    checkedUserIds: function () {
      var userIds = '';
      $('INPUT[name="chkUser"][type=checkbox]:checked').each(function (i, e) {
        userIds += $(e).val() + ',';
      });
      return userIds;
    },
    confirmPublishReports: function () {

      var self = this;
      var model = self.navigatorReportPublishRequest;
      var userIds = self.checkedUserIds();

      model.userIds = userIds;

      var data = JSON.parse(sessionStorage.KEEP_SESSION);
      var identifier = '';
      if (data && data.payload.publishingMultipleItems) {
        identifier = data.payload.selectedIdentifiers.join('-_-');
      }
      else {
        identifier = data.payload.selectedIdentifier;
      }
      model.publishDistrictAdmin = self.districtSelected;
      model.publishSchoolAdmin = self.schoolSelected;
      model.publishTeacher = self.classroomSelected;
      model.publishStudent = self.studentSelected;
      model.nodePath = identifier;
      model.districtId = self.districtId;
      model.alsoSendEmail = self.publishDialog.alsoSendEmail;


      var _customNote = self.publishDialog.customNote ? JSON.stringify(self.publishDialog.customNote).slice(1, -1) : '';


      model.customNote = _customNote;

      ShowBlock($('#publishArticle'), "Publishing");
      $.ajax({
        url: global.publishUrl,
        type: 'POST',
        data: JSON.stringify({ model: model }),
        contentType: "application/json",
        success: function (response) {
          $('#publishArticle').unblock();
          if (response.status === "success") {
            $('#dataTable').dataTable().fnDraw(false);
            $('#chkAllUsers').removeAttr('checked');
          }
          self.cancelPublish();
          self.resetPublishParam();
        }
      });
    },
    cancelPublish: function () {
      var self = this;
      self.publishDialog.isloadingPublishDialog = false;
      $("#publishArticle").dialog("close");
      $("body .my-overlay").remove();
    },
    publishReport: function () {
      var self = this;
      self.publishDialog.showingPublishDialog = true;
      self.publishDialog.isloadingPublishDialog = true;
      self.publishDialog.customNote = '';

      var userIds = self.checkedUserIds();
      if (!userIds || userIds.length == 0) {
        $('#error-messages').html('<li> Please select user </li>');
        $('#error-messages').show();
        setTimeout(function () {
          $('#error-messages').hide();
        }, 5000);
        return;
      }
      var data = JSON.parse(sessionStorage.KEEP_SESSION);
      var identifier = '';
      if (data && data.payload.publishingMultipleItems) {
        identifier = data.payload.selectedIdentifiers.join('-_-');
      }
      else {
        identifier = data.payload.selectedIdentifier;
      }
      self.publishDialog.publishDetail = [];

      $.get(global.getPublishPopupDetailUrl, {
        nodePath: identifier,
        checkedUserIds: userIds,
        districtId: self.districtId
      },
        function (publishDetail) {
          var publishDetailData = [];
          if (publishDetail && publishDetail.strongData) {
            publishDetailData = publishDetail.strongData;
          }
          self.publishDialog.publishDetail = publishDetailData;
          self.publishDialog.isloadingPublishDialog = false;
        }
      );

      $('#publishArticle').dialog({
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

      return;

    },
    unPublishReport: function () {
      var model = this.navigatorReportUnpublishRequest;
      var userIds = '';

      $('INPUT[name="chkUser"][type=checkbox]:checked').each(function (i, e) {
        var userId = $(e).val();
        userIds += $(e).val() + ',';
      });

      model.userIds = userIds;
      if (!model.userIds || model.userIds.length == 0) {
        $('#error-messages').html('<li> Please select user </li>');
        $('#error-messages').show();
        setTimeout(function () {
          $('#error-messages').hide();
        }, 5000);
        return;
      }
      var data = JSON.parse(sessionStorage.KEEP_SESSION);
      var identifier = '';
      if (data && data.payload.publishingMultipleItems) {
        identifier = data.payload.selectedIdentifiers.join('-_-');
      }
      else {
        identifier = data.payload.selectedIdentifier;
      }
      model.publishDistrictAdmin = this.districtSelected;
      model.publishSchoolAdmin = this.schoolSelected;
      model.publishTeacher = this.classroomSelected;
      model.publishStudent = this.studentSelected;
      model.nodePath = identifier;
      model.districtId = this.districtId;

      ShowBlock($('#dataTable'), "Un-Publishing");
      $.ajax({
        url: global.unPublishUrl,
        type: 'POST',
        data: JSON.stringify({ model: model }),
        contentType: "application/json",
        success: function (response) {
          if (response.status === "success") {
            $('#dataTable').unblock();
            $('#dataTable').dataTable().fnDraw(false);
            $('#chkAllUsers').removeAttr('checked');
          }
          $('#dataTable').unblock();
        }
      });

      this.resetPublishParam();
    },
    backToNavigatorReports: function () {
      location.href = global.indexUrl;
    },
    getNavigatorConfiguration: function () {
      var self = this;
      var data = JSON.parse(sessionStorage.KEEP_SESSION);
      var identifier = '';
      if (data && data.payload.publishingMultipleItems) {
        identifier = data.payload.selectedIdentifiers.join('-_-');
      }
      else {
        identifier = data.payload.selectedIdentifier;
      }
      $.get(global.getNavigatorConfigurationUrl, { nodePath: identifier, districtId: self.districtId }, function (response) {
        self.canPublishDistrictAdmin = response.navigatorConfiguration.canPublishDistrictAdmin;
        self.canPublishSchoolAdmin = response.navigatorConfiguration.canPublishSchoolAdmin;
        self.canPublishStudent = response.navigatorConfiguration.canPublishStudent;
        self.canPublishTeacher = response.navigatorConfiguration.canPublishTeacher;
        self.programs = response.studentProgram;
        self.grades = response.studentGrade;
        $('#filterPublishing').unblock();
      });
    },
    resetPublishParam: function () {
      this.navigatorReportPublishRequest = {
        navigatorReportIds: null,
        userIds: null,
        publishDistrictAdmin: false,
        publishSchoolAdmin: false,
        publishTeacher: false,
        publishStudent: false,
        selectedIdentifier: '',
        districtId: 0
      };
    }
  }
});
