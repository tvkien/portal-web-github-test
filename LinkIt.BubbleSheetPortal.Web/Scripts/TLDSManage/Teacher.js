var TeacherModel = new Vue({
    el: '#TLDSTeacherHome',
    data: {
        isShowModalBatchPrint: false,
        isShowModalSummaryReport: false,
        isShowModalConfirmReopen: false,
        profileId: 0,
        batchPrint: {
            isNoSelected: false,
            content: '',
            idList: '',
            downloadUrl: '',
            downloadText: '',
            btnCreateStatus: false
        },
        summaryReport: {
            downloadUrl: '',
            downloadText: '',
            btnCreateStatus: false
        }
    },
    methods: {
        confirmBatchPrint: function () {
            var idList = getSelectedProfileIdsToPrint();

            this.isShowModalBatchPrint = true;
            this.batchPrint.downloadUrl = '';
            this.batchPrint.btnCreateStatus = false;

            if (!idList.length) {
                this.batchPrint.isNoSelected = true;
                this.batchPrint.content = 'Please select Profile(s) to download and print.';
                return;
            }

            this.batchPrint.isNoSelected = false;
            this.batchPrint.content = 'Click to generate a PDF of each selected TLDS. You can then download and print.';
            this.batchPrint.idList = idList;
        },
        closeBatchPrint: function () {
            this.isShowModalBatchPrint = false;
            this.batchPrint.idList = '';
            this.batchPrint.downloadUrl = '';
            this.batchPrint.btnCreateStatus = false;
        },
        generatePdfBatchPrint: function () {
            var self = this;
            self.isShowModalBatchPrint = false;
            self.batchPrint.btnCreateStatus = true;

            if (self.batchPrint.idList.indexOf(',') > 0) {
                self.generateMultiBatchPrint();
            } else {
                var $topNavigation = $('#idTopNavigation');
                ShowBlock($topNavigation, 'Generating PDF');
                self.generatePdfSingleBatchPrint();
            }
        },
        generatePdfSingleBatchPrint: function () {
            var self = this;
            var reportParams = {
                profileId: self.batchPrint.idList,
                ReportFileName: 'demo_blank.pdf',
                TimezoneOffset: new Date().getTimezoneOffset()
            };

            TeacherService.generate(reportParams).success(function (res) {
                var reportS3Params = {
                    fileName: res.fileName,
                    profileId: self.batchPrint.idList
                };

                self.getTLDSReportS3File(reportS3Params);
            });
        },
        getTLDSReportS3File: function (params) {
            var self = this;
            var $topNavigation = $('#idTopNavigation');

            TeacherService.getTLDSReportS3File(params).success(function (res) {
                if (res.Result) {
                    self.isShowModalBatchPrint = true;
                    self.batchPrint.downloadUrl = res.Url;
                    self.batchPrint.downloadText = 'Click here to download file.';
                    $topNavigation.unblock();
                } else {
                    setTimeout(function () {
                        self.getTLDSReportS3File(params);
                    }, 3000);
                }
            });
        },
      generateMultiBatchPrint: function () {
        loading($('#idTopNavigation .block-border'), true, { postionY: '180px' });
            var self = this;
          TeacherService.generateBatchPdfZipFileName({}).success(function (response) {
            var zipFileName = response.zipFileName;

            //call ajax to generate the pdf file
            var reportFileName = "demo_blank.pdf";
            var data = {
              profileId: 0,
              ReportFileName: reportFileName,
              TimezoneOffset: new Date().getTimezoneOffset(),
              profileIdList: self.batchPrint.idList,
              zipFileName: encodeURIComponent(zipFileName)
            };

            TeacherService.generateBatchPdf(data).success(function (res) {
              
            });

            self.getTLDSZipBatchReportS3File(zipFileName);
          });
      },
      getTLDSZipBatchReportS3File: function (zipFileName) {
        var completedLoading = $('.dvloading');
        var text = $('.dvloading').children('.txt-loading');
        var self = this;
        TeacherService.getTLDSZipBatchReportS3File({ zipFileName: encodeURIComponent(zipFileName) }).success(function (response) {
          if (response.Result != true) {
            if (text.length === 0 && response.Total > 0) {
              text = '<p class="txt-loading">Generated ' + response.CompletedFiles + ' of ' + response.Total + ' profiles</p>';
              completedLoading.append(text);
            } else {
              $(text).text('Generated ' + response.CompletedFiles + ' of ' + response.Total + ' profiles');
            }
            setTimeout(function () {
              self.getTLDSZipBatchReportS3File(zipFileName);//try again
            }, 3000);
          } else {
            $(text).text('');
            loading($('#idTopNavigation .block-border'), false, { postionY: '35%' });
            self.isShowModalBatchPrint = true;
            self.batchPrint.downloadUrl = response.Url;
            self.batchPrint.downloadText = 'Click here to download file.';
            var $topNavigation = $('#idTopNavigation');
            $topNavigation.unblock();
          }
        });
      },
        confirmSummaryReport: function () {
            this.isShowModalSummaryReport = true;
            this.summaryReport.downloadUrl = '';
            this.summaryReport.btnCreateStatus = false;
        },
        closeSummaryReport: function () {
            this.isShowModalSummaryReport = false;
            this.summaryReport.downloadUrl = '';
            this.summaryReport.btnCreateStatus = false;
        },
        generatePdfSummaryReport: function () {
            var enrollmentYear = $('#selectEnrollmentYear').val();
            if (enrollmentYear == 'select' || enrollmentYear < 0) {
                enrollmentYear = 0;
            }

            var self = this;
            var summaryReportParams = {
                profileId: 0,
                ReportFileName: 'summary_report.pdf',
                TimezoneOffset: new Date().getTimezoneOffset(),
                enrollmentYear: enrollmentYear
            };
            var $topNavigation = $('#idTopNavigation');

            ShowBlock($topNavigation, 'Generating PDF');
            self.isShowModalSummaryReport = false;
            self.summaryReport.btnCreateStatus = true;

            TeacherService.generateSummaryReport(summaryReportParams).success(function (res) {
                var summaryReportS3Params = {
                    zipFileName: encodeURIComponent(res.zipFileName)
                };

                self.getTLDSZipSummaryReportS3File(summaryReportS3Params);
            });
        },
        getTLDSZipSummaryReportS3File: function (params) {
            var self = this;
            var $topNavigation = $('#idTopNavigation');

            TeacherService.GetTLDSZipSummaryReportS3File(params).success(function (res) {
                if (res.Result) {
                    self.isShowModalSummaryReport = true;
                    self.summaryReport.downloadUrl = res.Url;
                    self.summaryReport.downloadText = 'Click here to download file.';
                    $topNavigation.unblock();
                } else {
                    setTimeout(function () {
                        self.getTLDSZipSummaryReportS3File(params);
                    }, 3000);
                }
            });
        },
        reopenProfile: function () {
            this.isShowModalConfirmReopen = false;
            ShowBlock($('#idTopNavigation'), "Loading"); 
            TeacherService.reopenProfile(this.profileId).success(function (res) {
                if (res.Success) {
                    refreshDataTable();
                } else {
                    $('#tlds-manage-success-error-message').show();
                    $('#tlds-manage-success-error-message').html('<li>' + res.Error + '</li>');
                }
                $('#idTopNavigation').unblock();
            });
        },
        cancelReopen: function () {
            this.isShowModalConfirmReopen = false;
        }
    }
})
