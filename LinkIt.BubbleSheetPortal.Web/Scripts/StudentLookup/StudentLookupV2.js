var StudentLookupApp = new Vue({
  el: '#student-lookup',
  data: {
    showingDistributeModal: false,
    distributeMethod: "distribute-by-email",
    exportMethod: "export-by-list",
    selectedUsers: [],
    errorMessageList: [],
    successMessageList: [],
    showingSessionStudentModal: false,
    studentId: 0,
    student: '',
    showingResetPortalModal: false,
    hasGenerateLogin: false,
    showingExportStudentModal: false,
    showSubmitErrorStudentSlip: true
  },
  computed: {
    checkedDistributeByEmail: function () {
      return this.distributeMethod === 'distribute-by-email';
    },
    checkedDistributeByPrintedPage: function () {
      return this.distributeMethod === 'distribute-by-print-page';
    },
    checkedDistributeStudentLogin: function () {
      return this.distributeMethod === 'distribute-by-student-login';
    },
    selectedUsersCount: function () {
      return (this.selectedUsers || []).length;
    }
  },
  methods: {
    updateSelectedUsers: function () {
      var idlist = [];
      $("input.rcode:checked").each(function (index, elem) {
        idlist.push($(elem).attr('did'));
      });
      this.selectedUsers = idlist;
    },
    submitDistribute: function () {
      var self = this;
      if (self.checkedDistributeByEmail) {
        self.publishViaEmail();
      } else {
        self.publishViaPrintedPage();
      }
    },
    okExportPage: function () {
      var self = this;      
      if (self.exportMethod === "export-by-list") {
        self.showingExportStudentModal = false;
        self.exportStudentLogin();
      } else {
        self.publishViaPrintedStudentLogin();
      }  
    },
    publishViaEmail: function () {
      var self = this;
      ShowBlock($('#modal-distribute'), 'Distributing');
      var data = this.getCurrentFilterParameters();
      $.ajax({
        type: 'GET',
        data: data,
        url: '/StudentLookup/DistributeByEmail',
        success: function (response) {
          $('#modal-distribute').unblock();
          self.showingDistributeModal = false;
          if (response.IsSuccess === true) {
            if (response.StrongData && response.StrongData > 0)
              LinkIt.success('#divNotifications', 'Email sent to ' + response.StrongData + '/' + self.selectedUsersCount + ' user' + (self.selectedUsersCount > 0 ? 's' : ''));
            else
              LinkIt.success('#divNotifications', 'Email sent to ' + response.StrongData + '/' + self.selectedUsersCount + ' user' + (self.selectedUsersCount > 0 ? 's' : ''));
          }
          else {
            LinkIt.warningFadeout('#divNotifications', response.Message);
          }
          $('#dataTable').dataTable().fnDraw();
        },
        error: function (error) {
          $('#modal-distribute').unblock();
          self.showingDistributeModal = false;
          alert(error);
        }
      });

    },
    getCurrentFilterParameters: function () {
      var self = this;
      var reportFileName = "StudentDetail_" + (new Date()).getTime() + ".pdf";
      var _fnSettings = $('#dataTable').dataTable().fnSettings();
      var _sSortDir_0 = '';
      var _sColumns = '';
      var _iSortCol_0 = ''; //0
      if (_fnSettings != null) {
        var _aoColumns = _fnSettings.aoColumns;
        for (var i = 0; i < _aoColumns.length; i++) {
          _sColumns += _aoColumns[i].mData + ',';
        }
        if (_sColumns && _fnSettings.aaSorting.length > 0) {
          var _aSortingItem = _fnSettings.aaSorting[0];
          if (_aSortingItem.length == 3) {
            _iSortCol_0 = _aSortingItem[0];
            _sSortDir_0 = _aSortingItem[1]
          }
        }
      }
      var data = {
        districtId: !isFirstLoad ? -1 : (_historyData && _historyData.districtID ? _historyData.districtID : $('#selectDistrict').val()),
        LastName: "",
        FirstName: _historyData && _historyData.selectFirstName ? _historyData.selectFirstName : $('#selectFirstName').val(),
        Code: _historyData && _historyData.selectLocalId ? _historyData.selectLocalId : $('#selectLocalId').val(),
        StateCode: _historyData && _historyData.selectStateId ? _historyData.selectStateId : $('#selectStateId').val(),
        SchoolId: _historyData && _historyData.schoolID ? _historyData.schoolID : $('#selectAdminSchool').val(),
        GradeId: _historyData && _historyData.gradeID ? _historyData.gradeID : $('#selectGrade').val(),
        RaceName: _historyData && _historyData.selectRace ? _historyData.selectRace : ($('#selectRace').val() == null || $('#selectRace').val() == 'select' ? '' : $('#selectRace').val()),
        GenderId: _historyData && _historyData.selectGender ? _historyData.selectGender : $('#selectGender').val(),
        ShowInactiveStudent: _historyData && _historyData.showInactive ? _historyData.showInactive : showInactiveStudent,
        StudentDetailPrintingFileName: reportFileName,
        TimezoneOffset: new Date().getTimezoneOffset(),
        sColumns: _sColumns,
        iSortCol_0: _iSortCol_0,
        sSortDir_0: _sSortDir_0,
        sSearch: $('#dataTable_filter input').val(),
        selectedUserIds: (self.selectedUsers || []).join(','),
      };
      return data;
    },
    publishViaPrintedPage: function () {
      var self = this;
      ShowBlock($('#modal-distribute'), 'Generating PDF');
      var data = this.getCurrentFilterParameters();
      $.ajax({
        url: '/StudentLookup/Generate',
        traditional: true,
        type: 'POST',
        data: data,
        success: function (response) {
          $('#idPopupPrintConfirm').dialog({
            title: "",
            open: function () {
              $(this).parents('.ui-dialog').find('.ui-dialog-titlebar-close').remove();
              //Create overlay for popup
              $("body").append('<div class="my-overlay" style="z-index: ' + ($.ui.dialog.currentZ() - 1) + ';width:' + $(document).width() + 'px;height:' + $(document).height() + 'px;background-color: black;opacity: 0.3;position: absolute;top:0px;left: 0px;"></div>');
            },
            beforeclose: function () {
              //TODO if you want do anything after close popup.
              return true;
            },
            close: function () {
              //$('#idPopupPrintConfirm').remove();
              $("body .my-overlay").remove();
            },
            modal: false,
            width: 360,
            resizable: false,
            hideOnClose: true
          });

          $('#idlinkFilePrint').attr('href', response.Url);
          $('#modal-distribute').unblock();
          self.showingDistributeModal = false;
        },
        failure: function (response) {
          self.showingDistributeModal = false;
          CustomAlert(response);
        },
        timeout: 300000
      });
    },
    publishViaPrintedStudentLogin: function () {
      var self = this;
      ShowBlock($('#modal-export'), 'Generating PDF');
      var data = this.getCurrentFilterParameters();
      if (self.exportMethod === "export-by-per-student") {
        data.singleTemplate = true;
      } else {
        data.singleTemplate = false;
      }
      $.ajax({
        url: '/StudentLookup/GeneratePDFStudentLogin',
        traditional: true,
        type: 'POST',
        data: data,
        success: function (response) {
          self.showingExportStudentModal = false;
          if (response.IsSuccess === false) {
            $('#modal-export').unblock();
            CustomAlert(response.Message);
          } else {
            $('#idPopupPrintConfirm').dialog({
              title: "",
              open: function () {
                $(this).parents('.ui-dialog').find('.ui-dialog-titlebar-close').remove();
                //Create overlay for popup
                $("body").append('<div class="my-overlay" style="z-index: ' + ($.ui.dialog.currentZ() - 1) + ';width:' + $(document).width() + 'px;height:' + $(document).height() + 'px;background-color: black;opacity: 0.3;position: absolute;top:0px;left: 0px;"></div>');
              },
              beforeclose: function () {
                //TODO if you want do anything after close popup.
                return true;
              },
              close: function () {
                //$('#idPopupPrintConfirm').remove();
                $("body .my-overlay").remove();
              },
              modal: false,
              width: 360,
              resizable: false,
              hideOnClose: true
            });

            $('#idlinkFilePrint').attr('href', response.Url);
            $('#modal-export').unblock();
            $('#modal-distribute-student-slip').unblock();
            self.showingExportStudentModal = false;
          }
        },
        failure: function (response) {
          $('#modal-export').unblock();
          $('#modal-distribute-student-slip').unblock();
          self.showingExportStudentModal = false;
          CustomAlert(response);
        },
        timeout: 300000
      });
    },
    closeStudentSession: function () {
      var self = this;
      $('#dataTableStudentSession').dataTable().fnFilter('');
      self.showingSessionStudentModal = false;
    },
    exportStudentLogin() {
      ShowBlock($('#divStudent'), 'Exporting Student Login'); 
      var districtId = $('#selectDistrict').val();
      var schoolId = $('#selectAdminSchool').val();
      $.ajax({
        url: '/StudentLookup/ExportStudentLogin',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({ districtId, schoolId, studentIds: this.selectedUsers }),
        success: function (response) {
          if (response.Success) {
            $('#divStudent').unblock();
            $('#idPopupPrintConfirm').dialog({
              title: "",
              open: function () {
                $(this).parents('.ui-dialog').find('.ui-dialog-titlebar-close').remove();
                //Create overlay for popup
                $("body").append('<div class="my-overlay" style="z-index: ' + ($.ui.dialog.currentZ() - 1) + ';width:' + $(document).width() + 'px;height:' + $(document).height() + 'px;background-color: black;opacity: 0.3;position: absolute;top:0px;left: 0px;"></div>');
              },
              beforeclose: function () {
                //TODO if you want do anything after close popup.
                return true;
              },
              close: function () {
                //$('#idPopupPrintConfirm').remove();
                $("body .my-overlay").remove();
              },
              modal: false,
              width: 360,
              resizable: false,
              hideOnClose: true
            });

            $('#idlinkFilePrint').attr('href', response.Url);

          }
        }
      });
    }
  },
});
