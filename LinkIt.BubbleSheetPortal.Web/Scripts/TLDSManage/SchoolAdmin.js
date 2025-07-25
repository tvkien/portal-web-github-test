var SchoolAdminCtrl = {
    saveProfileUpload: function (params) {
        return $.ajax({
            type: 'POST',
            url: SchoolAdminModel.url,
            data: params
        });
    },
    getGender: function () {
        return $.ajax({
            type: 'GET',
            url: SchoolAdminModel.getGenderUrl
        });
    }
};

var SchoolAdminModel = new Vue({
    el: '#TLDSSchoolAdminHome',
    data: {
        isShowModalUploadStatement: false,
        genders: [],
        sectionCompleted: [
            { id: 'sc-1', value: 'true', text: 'Yes' },
            { id: 'sc-2', value: 'false', text: 'No' }
        ],
        enrollmentYears: [],
        uploadData: {
            firstName: '',
            lastName: '',
            dob: '',
            gender: 0,
            service: '',
            isCompleted: 'false',
            enrolmentYear: (new Date()).getFullYear() + 1,
            uploadFileUrl: '',
            fileName: ''
        },
        errorFileUploadMsg: '',
        url: '',
        getGenderUrl: '',
        isDisable: true
    },
    ready: function () {
        var defaultCurrentYear = (new Date()).getFullYear();
        var currentYear = new Date().getFullYear();

        this.enrollmentYears.push(defaultCurrentYear);

        for (var i = 0; i < 10; i++) {
            this.enrollmentYears.push(currentYear + i);
        }
    },
    computed: {
        isDisable: function() {
            return this.uploadData.firstName === '' || this.uploadData.lastName === ''  || this.uploadData.fileName === '';
        }
    },
    methods: {
        closeModalUploadPdf: function () {
            this.isShowModalUploadStatement = false;
            this.resetData();
        },
        resetData: function () {
            this.uploadData.firstName = '';
            this.uploadData.lastName = '';
            this.uploadData.dob = '';
            this.uploadData.gender = 0;
            this.uploadData.service = '',
            this.uploadData.isCompleted = 'false';
            this.uploadData.enrolmentYear = (new Date()).getFullYear() + 1;
            this.uploadData.uploadFileUrl = '';
            this.uploadData.fileName = '';
            this.isDisable = true;
        },
        openUploadPdf: function () {
            var self = this;
            SchoolAdminCtrl.getGender().success(function (response) {
                self.genders = response;
                self.uploadData.gender = response[0].Id;
                self.isShowModalUploadStatement = true;
                self.errorFileUploadMsg = [];
                self.onShowDatepicker();
            });
        },
        saveProfileByUploadPdf: function () {
            var self = this;
            self.errorFileUploadMsg = [];
            if (self.uploadData.firstName == '') {
                self.errorFileUploadMsg.push('First Name is required.');
            }
            if (self.uploadData.lastName == '') {
                self.errorFileUploadMsg.push('Last Name is required.');                
            }
            var enrolmentYear = this.uploadData.enrolmentYear;
            if ($('#selectEnrolmentYear').val() == 0) {
                self.errorFileUploadMsg.push('Enrolment Year is required.');
            }
            if (self.uploadData.fileName == '') {
                self.errorFileUploadMsg.push('Please upload a PDF.');
            }
            if (self.errorFileUploadMsg.length > 0) {
                self.isDisable = true;
                return;
            }

            ShowBlock($('#modal-upload-file-school-admin'), "Saving");
            var params = {
                FirstName: this.uploadData.firstName,
                LastName: this.uploadData.lastName,
                GenderId: this.uploadData.gender,
                DateOfBirth: this.uploadData.dob,
                ECSName: this.uploadData.service,
                Section102IsCompleted: this.uploadData.isCompleted,
                EnrollmentYear: $('#selectEnrolmentYear').val(),
                PdfFileName: this.uploadData.fileName
            };
            SchoolAdminCtrl.saveProfileUpload(params).success(function () {
                $('#modal-upload-file-school-admin').unblock();
                self.isShowModalUploadStatement = false;
                self.resetData();
                var dataTable = $('#dataTable').dataTable();
                dataTable.fnSettings().sAjaxSource = getAjaxSourceFilterTLDSProfileForSchoolAdmin(true);
                dataTable.fnDraw();
                populateEnrollmentYearFilter();
            }).error(function (err) {
                $('#modal-upload-file-school-admin').unblock();
            });
        },
        onShowDatepicker: function () {
            var year = new Date().getFullYear();
            var minDate =  new Date('01 01,' + (year -7));

            $('#txtDOB').datepicker({
                changeMonth: true,
                changeYear: true,
                maxDate:  new Date(),
                minDate: minDate,    
                dateFormat: jqueryDatePickerFormat(),
                beforeShow: function (input, inst) {
                    $('#ui-datepicker-div').addClass('uploadStatementDatePicker');
                }
            });
        },
        removePdfFile: function () {
            this.uploadData.uploadFileUrl = '';
            this.uploadData.fileName = '';
            this.isDisable = true;
        }
    }
});
