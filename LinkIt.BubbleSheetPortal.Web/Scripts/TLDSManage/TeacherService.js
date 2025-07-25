var TeacherService = {
    generate: function (params) {
        return $.ajax({
            url: '/TLDSReport/Generate',
            traditional: true,
            type: 'POST',
            data: params,
            timeout: 300000
        });
    },
    getTLDSReportS3File: function (params) {
        return $.ajax({
            url: '/TLDSReport/GetTLDSReportS3File',
            type: 'POST',
            data: params
        });
    },
    generateBatchPdf: function (params) {
        return $.ajax({
            url: '/TLDSReport/GenerateBatchPdf',
            type: 'POST',
            data: params
        });
  },
  generateBatchPdfZipFileName: function (params) {
    return $.ajax({
      url: '/TLDSReport/GenerateBatchPdfZipFileName',
      type: 'POST',
      data: params
    });
  },
  getTLDSZipBatchReportS3File: function (params) {
    return $.ajax({
      url: '/TLDSReport/GetTLDSZipBatchReportS3File',
      type: 'POST',
      data: params
    });
  },
    generateSummaryReport: function (params) {
        return $.ajax({
            url: '/TLDSReport/GenerateSummaryReport',
            traditional: true,
            type: 'POST',
            data: params,
            timeout: 300000
        });
    },
    GetTLDSZipSummaryReportS3File: function (params) {
        return $.ajax({
            url: '/TLDSReport/GetTLDSZipSummaryReportS3File',
            type: 'POST',
            data: params
        });
    },
    reopenProfile: function (profileId) {
        return $.ajax({
            url: '/TLDSManage/ReopenProfile',
            type: 'POST',
            data: { profileId: profileId }
        });
    }
};
