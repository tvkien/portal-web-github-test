var ManageParentApp = new Vue({
  el: '#manage-parent',
  data: {
    showingDistributeModal: false,
    distributeMethod: "",
    selectedUsers: [],
    errorMessageList: [],
    successMessageList: [],
  },
  computed: {
    checkedDistributeByEmail: function () {
      return this.distributeMethod === 'distribute-by-email';
    },
    checkedDistributeByPrintedPage: function () {
      return this.distributeMethod === 'distribute-by-print-page';
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
      }
      else {
        self.publishViaPrintedPage();
      }
    },
    publishViaEmail: function () {
      var self = this;
      ShowBlock($('#modal-distribute'), 'Distributing');
      var data = this.getCurrentFilterParameters();
      $.ajax({
        type: 'GET',
        data: data,
        url: '/ManageParent/DistributeByEmail',
        success: function (response) {
          $('#modal-distribute').unblock();
          self.showingDistributeModal = false;
          if (response.IsSuccess === true) {
            if (response.StrongData && response.StrongData > 0)
              LinkIt.success('#divNotifications', 'Email sent to ' + response.StrongData + '/' + self.selectedUsersCount + ' user' + (self.selectedUsersCount > 0 ? 's' : ''));
            else
              LinkIt.error('#divNotifications', 'Email sent to ' + response.StrongData + '/' + self.selectedUsersCount + ' user' + (self.selectedUsersCount > 0 ? 's' : ''));
          }
          else {
            LinkIt.error('#divNotifications',upperFistCharacter(response.Message));
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
      var currentParams = currentFilterParams || _historyData;
      var reportFileName = "ParentDetail_" + (new Date()).getTime() + ".pdf";
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
        districtId: !isFirstLoad ? -1 : (currentParams && currentParams.districtID ? currentParams.districtID : $('#selectDistrict').val()),
        parentName: currentParams.parentName,
        studentName: currentParams.studentName,
        gradeId: currentParams && currentParams.gradeID ? currentParams.gradeID : $('#selectGrade').val(),
        schoolId: currentParams && currentParams.schoolID ? currentParams.schoolID : $('#selectAdminSchool').val(),
        showInactiveParent: currentParams && currentParams.showInactive ? currentParams.showInactive : showInactiveParent,
        ParentDetailPrintingFileName: reportFileName,
        TimezoneOffset: new Date().getTimezoneOffset(),
        sColumns: _sColumns,
        iSortCol_0: _iSortCol_0,
        sSortDir_0: _sSortDir_0,
        sSearch: $('#dataTable_filter input').val(),
        selectedUserIds: (self.selectedUsers || []).join(','),
        loginTimeFrame: currentParams && currentParams.loginTimeFrame ? currentParams.loginTimeFrame : $("#selectLoginTimeframe").val(),
        hasRegistrationCode: currentParams && currentParams.hasRegistrationCode ? currentParams.hasRegistrationCode : $("#selectHasRegistrationCode").val(),
      };
      return data;
    },
    publishViaPrintedPage: function () {
      var self = this;
      ShowBlock($('#modal-distribute'), 'Generating PDF');
      var data = this.getCurrentFilterParameters();
      $.ajax({
        url: '/ManageParent/Generate',
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

  },
});
