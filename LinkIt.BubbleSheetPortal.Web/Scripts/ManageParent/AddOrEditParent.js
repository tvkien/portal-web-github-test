var AddOrEditParent = new Vue({
  el: '#add-or-edit-parent',
  data: {
    parentData: {
      userId: 0,
      stateId: 0,
      districtId: 0,
      userName: '',
      firstName: '',
      lastName: '',
      code: '',
      parentId:0,
      studentIdsThatBeAddedOnCommit: [],      
      parentMetaDatas: [],
    },    
    ignoreWatch: false,
    currentRoleId: 0,
    errorList: [],
    isShowModalAssignStudent: false,
    showInactive: false,
    isShowFilter: false,
    getParentInfoUrl: '/ManageParent/GetParentInfo',
    getStudentsUrl: '/ManageParent/ListAvailableStudent',
    addStudentsToParentUrl: '/ManageParent/AddStudentsToParent',
    getAvailableProgramAndGradeurl: '/ManageParent/GetAvailableProgramAndGrade',
    updateParentInfoUrl: '/ManageParent/UpdateParentInfo',
    createNewParentUrl: '/ManageParent/CreateNewParent',
    unassignStudentUrl: '/ManageParent/UnassignStudent',
    manageClassesAddNewStudentUrl: '/ManageClasses/AddNewStudent',
    getParentMetaDataUrl: '/ManageParent/GetParentMetaData',
    getSelectedStudentURL: '',
    isCheckAllStudent: false,
    selectedStudents: [],
    selectedStudentsObject: [],
    isShowSelectedStudent: false,
    isShowViewSelectedStudent: false, //use for is-Show of vue
    initialDatableSelectedOnly: false,
    isShowAddNewStudent: false,
    isShowModalAddStudentParent: false,
    isLoading: false,
    isAddedStudentsToClass: false,
    promtMessage: '',
    removeStudentSelectAll: [],
    manageClass: {
      programs: [],
      grades: [],
      programSelected: [],
      gradesSelected: []
    },
    isDisableAssignBtn: true    
  },
  computed: {
    currentlyEditing: function () {
      return this.parentData && this.parentData.userId && this.parentData.userId > 0 ? true : false;
    },
    editOrAddTitle: function () {
      return this.currentlyEditing ? 'Edit Parent' : 'Add Parent';
    },
    canEditLocationInfo: function () {
      return ([5, 7, 27].includes(this.currentRoleId));
    },
    showInactiveText: function () {
      return this.showInactive ? 'On' : 'Off';
    },
    isDisabledShowSelectedStudent: function () {
      return this.selectedStudents.length === 0 && !this.isShowSelectedStudent && !this.isCheckAllStudent;
    },
    isDisabledButton: function () {
      return this.selectedStudents.length === 0 && !this.isCheckAllStudent;
    }
  },
  watch: {
    'parentData.stateId': function () {
      if (this.ignoreWatch)
        return;
      this.populateDistrict();
    },
    'parentData.districtId': function () {
      if (!this.currentlyEditing) {
        if (this.parentData.studentIdsThatBeAddedOnCommit.length > 0) {
          this.parentData.studentIdsThatBeAddedOnCommit = [];
          this.reloadAssignedStudentList();
        }
      }
    },
    'isShowSelectedStudent': 'showSelectedStudent'
  },
  methods: {
    populateDistrict: function () {
      var self = this;
      console.log("state", self.parentData.stateId);
      return self.populateDataForDropdown('/CategoriesAPI/GetDistrictByStateId', { stateId: self.parentData.stateId }, '#selectDistrict', 'Select District');
    },
    getAjaxSourceForStudentList: function () {
      return this.getStudentsUrl + '?parentUserId=' + (this.parentData.userId || 0) + '&districtId=' + this.parentData.districtId + '&showInactive=' + this.showInactive + '&programIdList=' + this.manageClass.programSelected + '&gradeIdList=' + this.manageClass.gradesSelected + '&temporaryAddedStudentIds=' + this.parentData.studentIdsThatBeAddedOnCommit.toString();
    },
    populateState: function () {
      return this.populateDataForDropdown('/CategoriesAPI/GetStates', {}, '#selectState', 'Select State');
    },
    populateDataForDropdown: function (endPoint, params, itemId, defaultValue) {
      var self = this;
      console.log('start', endPoint);
      return new Promise(function (resolve, reject) {
        $.ajax({
          type: 'GET',
          data: params,
          url: endPoint,
          success: function (response) {
            self.addSelectListItems(itemId, response.data, defaultValue);
            console.log('end', endPoint);
            resolve();
          },
          error: function (error) {
            reject(error);
          }
        });
      });
    },
    addSelectListItems: function (itemId, results, defaultValue) {
      var self = this;
      var selectList = $(itemId);
      if (results.length == 0) {
        selectList.html('<option value="0">No Results Found</option>');
      } else {
        selectList.html("");
        if (defaultValue.length > 0)
          selectList.append(
            $("<option></option>").attr("value", "0").text(defaultValue)
          );
        $.each(results, function (i, value) {
          selectList.append(
            $("<option></option>").attr("value", value.id).text(value.name)
          );
        });
      }
    },
    loadUserData: function (parentUserId) {
      console.log('start', 'loadUserData');
      var self = this;
      return new Promise(function (resolve, reject) {
        $.ajax({
          type: 'GET',
          data: { parentUserId: parentUserId },
          url: self.getParentInfoUrl,
          success: function (response) {

            self.parentData = {
              userId: response.UserId,
              stateId: response.StateId,
              districtId: response.DistrictId,
              userName: response.UserName,
              firstName: response.FirstName,
              lastName: response.LastName,
              //phoneNumber: response.PhoneNumber,
              code: response.Code,
              parentId: response.ParentId,
              studentIdsThatBeAddedOnCommit: [],              
              parentMetaDatas: response.ParentMetaDatas
            };
            resolve(response);
            console.log('end', 'loadUserData');
          },
          error: function (error) {
            reject(error);
          }
        });
      });
    },
    startBindingData: function (parentUserId) {
      var self = this;

      self.ignoreWatch = true;
      ShowBlock($('#selectFilters'), "Loading");
      self.loadUserData(parentUserId || 0)
        .then(function () {
          return self.populateState();
        })
        .then(function () {
          if ((self.parentData.stateId || 0) > 0)
            return self.populateDistrict();
          else
            return resolve(true);
        })
        .then(function () {
          $('#selectState').val(self.parentData.stateId);
          $('#selectDistrict').val(self.parentData.districtId);
          self.ignoreWatch = false;
          $('#selectFilters').unblock();
        })        
        .catch(function () {
          self.ignoreWatch = false;
          $('#selectFilters').unblock();
        });
    },
    showAssignStudentPopup: function () {
      if ((this.parentData.districtId || 0) < 1) {
        alert('Please select a District first');
        return;
      }
      var self = this;
      ShowBlock($('#available-student'), "Loading");
      this.isShowModalAssignStudent = true;
      $.get(self.getAvailableProgramAndGradeurl, { parentUserId: this.parentData.userId, districtId: self.parentData.userId })
        .done(function (response) {
          $('#available-student').unblock()
          self.manageClass.programs = response.Programs;
          self.manageClass.grades = response.Grades;
          self.manageClass.programSelected = [];
          self.manageClass.gradesSelected = [];
          self.isShowAddNewStudent = response.IsShowAddNewStudent;
          displayFilterManageClass();
        });
    },
    submitParentInfo: function () {

      var self = this;
      var parentDivId = '#selectFilters';
      ShowBlock($(parentDivId), "Submiting");
      var submitDescription = this.currentlyEditing ? { url: self.updateParentInfoUrl, method: 'POST' }
        : { url: self.createNewParentUrl, method: 'POST' };
      self.errorList = [];

      var listStudentParentUpdate = self.collectStudentParentUpdate();

      var submitData = {
        parentId: self.parentData.parentId,
        userId: self.parentData.userId,
        stateId: self.parentData.stateId,
        districtId: self.parentData.districtId,
        userName: self.parentData.userName,
        firstName: self.parentData.firstName,
        lastName: self.parentData.lastName,
        //phoneNumber: self.parentData.phoneNumber,
        code: self.parentData.code,
        studentIdsThatBeAddedOnCommit: self.parentData.studentIdsThatBeAddedOnCommit.toString(),
        ParentMetaDatas: self.parentData.parentMetaDatas,
        StudentParents: listStudentParentUpdate,
      };

      $.ajax({
        type: submitDescription.method,
        data: JSON.stringify({ parentModel: submitData } ),
        url: submitDescription.url,
        contentType: "application/json",
        success: function (response) {
          $(parentDivId).unblock();
          if (response.Success) {

            if (self.currentlyEditing) {
              self.startBindingData(self.parentData.userId);
              self.reloadAssignedStudentList();
            }
            else {
              var _createdParentUserId = response.CreatedParentUserId;
              window.location.replace("/ManageParent/Edit?parentUserId=" + _createdParentUserId);
            }
          }
          else {
            $(parentDivId).unblock();
            self.errorList = response.ErrorList;
          }
        },
        error: function (error) {
          alert(error);
        }
      });
    },
    reloadAssignedStudentList: function () {
      $('#studentTable').dataTable().fnDraw();
    },
    goBack: function () {
      window.location.replace("/ManageParent");
    },
    prompAddStudentToParent: function () {

      this.promtMessage = '';
      this.isLoading = false;
      this.isShowModalAddStudentParent = true;
      $("#isShowButtonAddStudent").show();
      var studentSelectedCount = this.countSelectedStudent();

      if (studentSelectedCount === 1) {
        this.promtMessage = 'Are you sure you want to add 1 student to the Parent?';
      } else {
        this.promtMessage = 'Are you sure you want to add these ' + studentSelectedCount + ' students to the Parent?';
      }
    },
    addStudentToParent: function () {
      var self = this;
      var studentSelectedCount = self.countSelectedStudent();
      if (studentSelectedCount <= 0) {
        return;
      }

      self.isLoading = true;
      if (!self.currentlyEditing) {
        if (!self.parentData.studentIdsThatBeAddedOnCommit) {
          self.parentData.studentIdsThatBeAddedOnCommit = [];          
        }
        self.selectedStudents.forEach(function (value, index) {
          if (!self.parentData.studentIdsThatBeAddedOnCommit.includes(value)) {
            self.parentData.studentIdsThatBeAddedOnCommit.push(value);
             
            tempStudentParentAdd.push({ "studentId": value, "relationship": $('#txtRelationshipStudentParent').val(), "studentDataAccess": $('#cbStudentDataAccessStudentParent').prop("checked") });
            
          }
        });

        self.reloadAvailableTableAfterAssign();
      }
      else {
        $.post(self.addStudentsToParentUrl, {
          studentIds: self.selectedStudents.toString(),
          parentUserId: self.parentData.userId,
          relationship: $('#txtRelationshipStudentParent').val(),
          studentDataAccess: $('#cbStudentDataAccessStudentParent').prop("checked"),
        }, function (response) {
          self.isLoading = false;
          self.isAddedStudentsToClass = true;
          if (response.IsSuccess == true) {
            self.reloadAvailableTableAfterAssign();
          } else {
            $("#isShowButtonAddStudent").hide();
            self.promtMessage = response.Message;
          }
        });
      }
    },
    handleInputChange: function () {
      var self = this;
      self.isDisableAssignBtn = !$('#txtRelationshipStudentParent').val();
    },
    reloadAvailableTableAfterAssign: function () {

      var self = this;
      self.isShowModalAddStudentParent = false;
      self.promtMessage = '';
      self.isCheckAllStudent = false;
      self.resetData();
      if (self.isShowSelectedStudent) {
        selectedStudentTable.fnClearTable();

      } else {
        ui.datatablefiltermanageclass.fnSettings().sAjaxSource = self.getAjaxSourceForStudentList();
        ui.datatablefiltermanageclass.fnDraw();
      }
    },
    closeModalConfirmAddParent: function () {
      this.isShowModalAddStudentParent = false;
    },
    countSelectedStudent: function () {
      return this.selectedStudents.length;
    },
    removeAllSelectedStudent: function () {
      this.resetData();
      this.isCheckAllStudent = false;
      selectedStudentTable.fnClearTable();
    },
    showSelectedStudent: function () {
      var self = this;

      if (self.isShowSelectedStudent) {

        ShowBlock($('#datatablefiltermanageclass'), 'Loading');

        self.isShowViewSelectedStudent = true;

        if (self.initialDatableSelectedOnly) {
          selectedStudentTable.fnClearTable();
          selectedStudentTable.fnAddData(self.selectedStudentsObject);
          selectedStudentTable.fnDraw();
        } else {
          displaySelectedStudentOnly();
        }
      } else {
        self.isShowViewSelectedStudent = false;
        ui.datatablefiltermanageclass.fnSettings().sAjaxSource = self.getAjaxSourceForStudentList();
        ui.datatablefiltermanageclass.fnDraw();
      }
    },
    checkAllStudent: function () {
      var self = this;
      var $chkCheckbox = $('input[name="chkStudentId"][type=checkbox]');
      this.isCheckAllStudent = !this.isCheckAllStudent;
      this.resetData();
      if (this.isCheckAllStudent) {
        var _dataTable = $('#datatablefiltermanageclass').dataTable();
        $chkCheckbox.prop('checked', true);
        $chkCheckbox.each(function () {
          $(this).parents('tr').addClass('is-selected');
          var data = _dataTable.fnGetData($(this).parents('tr').index());
          var studentId = $(this).val();
          if (self.selectedStudents.indexOf(studentId) < 0) {
            self.selectedStudents.push(studentId);
            self.selectedStudentsObject.push(data);
          }
        });
        //this.collectSelectedStudentId();
      } else {
        $chkCheckbox.prop('checked', false);
        $chkCheckbox.each(function () {
          $(this).parents('tr').removeClass('is-selected');
          self.selectedStudents = [];
          self.selectedStudentsObject = [];
        });
      }
    },
    closeModalAddStudentClass: function () {
      this.isShowModalAssignStudent = false;
      this.isCheckAllStudent = false;
      this.isShowSelectedStudent = false;
      this.resetData();

      $("#studentTable").dataTable().fnDraw()

      if (this.isAddedStudentsToClass) {
        this.isAddedStudentsToClass = false;
        this.reloadAssignedStudentList();
      }
    },
    resetData: function () {
      this.selectedStudents = [];
      this.selectedStudentsObject = [];
      this.removeStudentSelectAll = [];
    },
    clearAllCheckBox: function () {
      this.manageClass.programSelected = [];
      this.manageClass.gradesSelected = [];
    },
    applyFilter: function () {
      this.isShowSelectedStudent = false;
      this.isCheckAllStudent = false;
      $('#chkAllStudent').prop('checked', false);
      this.resetData();

      ui.datatablefiltermanageclass.fnSettings().sAjaxSource = this.getAjaxSourceForStudentList();
      ui.datatablefiltermanageclass.fnDraw();
    },
    unassignStudent: function (studentId) {
      if (liststudentAddedToCurrentParent.includes(studentId)) {
        liststudentAddedToCurrentParent.pop(studentId);
      }

      var self = this;
      if (self.parentData.studentIdsThatBeAddedOnCommit.includes(studentId + "")) {
        self.parentData.studentIdsThatBeAddedOnCommit.pop(studentId + "");
      }
      if (self.currentlyEditing) {
        ShowBlock($('#selectFilters'), "Unassigning");
        console.log('start', 'UnassignStudent');
        return new Promise(function (resolve, reject) {
          $.ajax({
            type: 'POST',
            data: { studentId: studentId, parentUserId: self.parentData.userId },
            url: self.unassignStudentUrl,
            success: function (response) {
              if (response.Success == false) {
                errorList = [response.ErrorMessage];
              }
              else {
                self.reloadAssignedStudentList();
              }
              $('#selectFilters').unblock();
              console.log('end', 'UnassignStudent');
              resolve();
            },
            error: function (error) {
              $('#selectFilters').unblock();
              reject(error);
            }
          });
        });
      }
      else {
        self.reloadAssignedStudentList();
      }
    },
    redirecToAddNewStudent: function () {
      if (!this.currentlyEditing) {
        alert('Cannot use this feature before parent profiles has been added');
        return;
      }
      var parentDivId = '#available-student';
      ShowBlock($(parentDivId), "Redirecting");
      location.href = this.manageClassesAddNewStudentUrl + '?parentUserId=' + this.parentData.userId;
    },
    changeInactive: function () {
      this.resetData();
      this.isCheckAllStudent = false;

      this.showInactive = !this.showInactive;
      ui.datatablefiltermanageclass.fnSettings().sAjaxSource = this.getAjaxSourceForStudentList();
      ui.datatablefiltermanageclass.fnDraw();
    },
    resetPassword: function () {
      $.get('/Admin/ResetPassword?userId=' + this.parentData.userId, function (result) {
        $('#sideContainer').html(result);
      });
    },
    collectStudentParentUpdate: function () {      
      
      var listStudentParentUpdate = [];
      if (liststudentAddedToCurrentParent.length > 0) {
        liststudentAddedToCurrentParent.forEach(function (item) {
          listStudentParentUpdate.push({ studentId: item, relationship: $('#txtRelationshipStudentParent_' + item).val(), studentDataAccess: $('#cbStudentDataAccessStudentParent_' + item).is(':checked') })
        });
      }
      //console.log(JSON.stringify(listStudentParentUpdate));
      return listStudentParentUpdate;
    },
  }
})
