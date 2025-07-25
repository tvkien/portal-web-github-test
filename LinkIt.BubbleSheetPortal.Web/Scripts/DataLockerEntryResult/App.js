Math.mean = function () {
  var values = Array.from(arguments).filter(function (value) {
      return value !== null && value !== undefined && value !== '';
  });
  if (values && values.length === 0) return null;
  return values.reduce(function (a, b) {
      return a + b;
  }) / values.length;
}

var prevewExtsSuport = (function () {
  var ua = navigator.userAgent.toLowerCase();
  var isSafari = ua.indexOf('safari') != -1 && ua.indexOf('chrome') == -1;
  var image = ['apng', 'avif', 'gif', 'jpg', 'jpeg', 'jfif', 'pjpeg', 'pjp', 'png', 'svg', 'ico', 'cur'];
  var audio = ['mp3', 'wav'];
  var video = ['webm', 'mp4'];
  if (!isSafari) {
    audio.push('ogg');
    video.push('ogg');
  }
  return {
    image: image,
    audio: audio,
    video: video
  }
})();

var StudentEntryCtrl = {
  generatePdf: function (url, params) {
    return $.ajax({
      type: 'POST',
      url: url,
      contentType: 'application/json',
      dataType: "json",
      data: JSON.stringify({
        'printModel': params
      })
    });
  }
};

function TaskQueue(concurrency, callbackDone) {
  this.concurrency = concurrency;
  this.running = 0;
  this.finished = 0;
  this.queue = [];
  this.results = [];
  this.callbackDone = callbackDone ?? function () { };
}

TaskQueue.prototype.pushTask = function (task, callback) {
  this.queue.push(task);
  this.next(callback);
}

TaskQueue.prototype.next = function (callback) {
  var self = this;
  while (self.running < self.concurrency && self.queue.length) {
    var task = self.queue.shift();
    task(function (res, err) {
      self.results.push({
        result: res,
        error: err
      });
      self.running--;
      self.finished++;
      callback(self);
      self.next(callback);
    });
    self.running++;
  }
  if (!self.queue.length && !self.running) {
    this.callbackDone(self.results)
  }
}

Vue.filter('formatDate', function (value) {
  if (value) {
    return moment(String(value)).format(window.global.dateFormat + ' HH:mm')
  }
});

var StudentEntryModel = new Vue({
  el: '#studentEntry',
  data: {
    generatePdfUrl: '',
    downloadSupportDocumentUrl: '',
    virtualTestId: '',
    virtualtestFileKey: '',
    classId: '',
    studentsIdSelectedString: '',
    entryResultDate: '',
    allColumnsSelected: false,
    printResult: {
      isExistCustomScore: false,
      customScore: [],
      customSubScores: [],
      layouts: [{
        text: 'Landscape',
        value: 'landscape',
        isDisable: false
      },
      {
        text: 'Portrait',
        value: 'portrait',
        isDisable: false
      },
      {
        text: 'Labels',
        value: 'labels',
        isDisable: false,
        title: 'Labels will only be available if a single column is selected. You must select your label size.'
      }
      ],
      scoreDescriptions: [{
        text: 'Yes',
        value: 'yes'
      },
      {
        text: 'No',
        value: 'no'
      },
      ],
      rubricDescriptions: [{
        text: 'Yes',
        value: 'yes'
      },
      {
        text: 'No',
        value: 'no'
      },
      ],
      scoreSelected: [],
      subScoresSelected: [],
      layoutSelected: 'landscape',
      scoreDescriptionSelected: 'yes',
      rubricDescriptionSelected: 'yes',
      pdfUrl: ''
    },
    isShowPreview: false,
    isLoading: false,
    isPrintSucess: false,
    isWrappedPages: false,
    isWarningPrint: false,
    isShowModalWarning: false,
    isShowModalWarningPublishToPortal: false,
    isShowModalFilterWarning: false,
    isShowModalPrint: false,
    isShowModalPrintPublishToPortal: false,
    isShowModalClearAllScore: false,
    isShowModalCancel: false,
    isShowModalWarningChooseStudent: false,
    msgInvalid: '',
    fullscreen: {
      icon: '<svg xmlns="http://www.w3.org/2000/svg" width="16" height="20" viewBox="0 0 100 125"><path d="M26.947 86.098H5V64.151h6.997v14.95h14.95zM11.997 35.849H5V13.902h21.947v6.997h-14.95zM95 35.849h-6.997v-14.95h-14.95v-6.997H95zM95 86.098H73.053v-6.997h14.95v-14.95H95z"/><g><path d="M82.405 73.503h-64.81V26.498h64.81v47.005zm-57.812-6.998h50.815v-33.01H24.593v33.01z"/></g></svg>',
      text: 'Fullscreen'
    },
    enterResultsUrl: '/DataLockerEntryResult/EnterResults',
    initialedUploadFile: { file_upload_artifact: false, file_folder_artifact: false },
    selectedTD: null,
    selectedRow: 0,
    selectedCol: 0,
    artifact: {
      msgError: ''
    },
    isShowNote: false,
    studentName: '',
    noteColumnName: '',
    isShowModalRubricDescription: false,
    rubricDescriptionContent: '',
    isShowModalArtifactFolder: false,
    artifactFolderModalVM: {
      allowFileTypes: '',
      uploadFileTypeOject: [],
      uploadFileExceedSizeOject: [],
      allowFileTypeInGroups: '',
      fileUploads: [],
      duplicateFile: [],
      exceedFileSize: [],
      notAllowedFile: [],
      maxFileSize: 0,
      currentScore: '',
      inputLink: '',
      artifacts: [],
      Students: [],
      msgError: '',
      isOverallScore: false,
      isMassUpload: false,
      subId: null,
      subName: null,
      artifactsWarning: [],
      isShowPopUpModalErrorFileUploadMass: false,
      objectPopUpModalErrorFileUploadMass: null
    },
    isShowNoteDate: false,
    noteDate: '',
    arrayNoteDate: [],
    tagList: [],
    arrEntryResultDate: [],
    virtualTestName: '',
    itemPreview: null,
    templateId: null,
    warningFileSize: '',
    isClickNavSR: false,
    dateFilter: null,
    triggerKeepAlive: false
  },
  computed: {
    isShowBtnPrint: function () {
      return this.printResult.scoreSelected.length === 0 && this.printResult.subScoresSelected.length === 0 && this.printResult.rubricDescriptionSelected === 'no';
    },
    isDisabledViewSupportingDocument: function () {
      return this.virtualtestFileKey == null || this.virtualtestFileKey === '';
    },
    isUploadFile: function () {
      return this.uploadFile == 'uploadFile';
    },
    isShowFileName: function () {
      return !!this.fileName && this.uploadFile == 'uploadFile';
    },
    isDisabledViewRubricDescription: function () {
      return this.rubricDescriptionContent == null || this.rubricDescriptionContent === '';
    },
    widthModalPrint: function () {
      if (this.msgInvalid == '' && !this.isWrappedPages) {
        var maxCustomScore = this.printResult.customScore.ScoreInfos.length
        var maxCustomSubScore = Math.max(...this.printResult.customSubScores.map(i => i.ScoreInfos.length))
        if (maxCustomScore > 3 || maxCustomSubScore > 3) {
          return {
            width: 800,
            widhPerItem: "25%"
          }
        } else if (maxCustomScore == 0 && maxCustomSubScore == 0) {
          return {
            width: 600,
            widhPerItem: null
          }
        } else {
          var maxItem = maxCustomScore > maxCustomSubScore ? maxCustomScore : maxCustomSubScore
          return {
            width: 600,
            widhPerItem: Number(100 / maxItem).toFixed(2) + '%'
          }
        }
      }
      return {
        width: 600,
        widhPerItem: null
      }
    },
    selectStudentsList: function () {
      return this.artifactFolderModalVM
        .Students
        .map(function (item) {
          return {
            id: item.StudentID,
            text: item.DisplayFullName 
          }
        })
    }
  },
  watch: {
    'printResult.scoreSelected': 'statusAllColumnsSelected',
    'printResult.subScoresSelected': 'statusAllColumnsSelected'
  },
  ready: function () {
    window.addEventListener('resize', this.resizeHandsontable);
  },
  methods: {
    confirmCancelEntryResult: function () {
      var self = this;

      self.isShowModalCancel = false;
      window.onbeforeunload = null;

      ShowBlock($('#sectionStudentEntry'), 'Loading');
      var params = {
        virtualTestId: $('#virtualTestId').val(),
        classId: $('#classId').val()
      };
      StudentEnterCtrl.deleteAutoSave(params)
        .done(function () {
          window.location.href = self.enterResultsUrl;
        });
    },
    toggleCancelEntryResult: function (previewForm) {
      var isDataDirty = oldHandsonStudentData !== JSON.stringify(hansonStudentData);

      if (isDataDirty && previewForm === undefined) {
        this.isShowModalCancel = true;
      } else {
        window.location.href = this.enterResultsUrl;
      }
    },
    closeCancelEntryResult: function () {
      this.isShowModalCancel = false;
    },
    clearUnsavedChanges: function (resultDate) {
      var $sectionStudentEntry = $('#sectionStudentEntry');
      ShowBlock($sectionStudentEntry, 'Loading');

      var params = {
        virtualTestId: $('#virtualTestId').val(),
        classId: $('#classId').val(),
        resultDate: resultDate
      };
      StudentEnterCtrl.deleteAutoSave(params)
        .done(function () {
          loadStudentEntryResult(true);
        });
    },
    confirmClearAllScore: function () {
      this.toggleClearAllScore();
      var $sectionStudentEntry = $('#sectionStudentEntry');
      ShowBlock($sectionStudentEntry, 'Waiting');
      Vue.nextTick(function () {
        confirmClearAllScore();
      });
    },
    toggleClearAllScore: function () {
      this.isShowModalClearAllScore = !this.isShowModalClearAllScore;
    },
    //For publish to student portal 
    checkWarningPublishToStudentPortal: function (previewForm) {
      var isDataDirty = oldHandsonStudentData != JSON.stringify(hansonStudentData);
      if (isDataDirty) {
        this.isShowModalWarningPublishToPortal = !this.isShowModalWarningPublishToPortal;
      } else {
        saveResults({ isSaveForPublish: true });
        isValidStudentEntry() && publichToStudentPortal();
      }
    },
    confirmSavePublishStudentPortal: function (isOpenPrintPopUp) {
      saveResults();
      isValidStudentEntry() && publichToStudentPortal();
    },
    //End publish to student portal
    checkWarningPrint: function (previewForm) {
      var $studentEntryResult = $('.student-entry-result');
      if ($studentEntryResult.find('td.htInvalid').length) {
        StudentEntryModel.isShowModalWarning = false;
        StudentEntryModel.isShowModalFilterWarning = false;
        StudentEntryModel.isShowModalPrint = true;
        StudentEntryModel.msgInvalid = 'Some columns have data validation applied to them. If a cell is highlighted in red it means you entered an invalid value. If you hover-over that cell it will provide a hint as to what you did wrong. Please correct those cells prior to print.';
        return;
      }
      var isDataDirty = oldHandsonStudentData != JSON.stringify(hansonStudentData);
      if (isDataDirty && previewForm === undefined) {
        this.toggleWarningPrint();
      } else {
        this.showPrintResults();
      }
    },
    toggleWarningPrint: function () {
      this.isShowModalWarning = !this.isShowModalWarning;
    },
    showPrintResults: function () {
      this.msgInvalid = '';
      this.isShowModalPrint = true;
      this.isPrintSucess = false;
      this.isWarningPrint = false;
      this.isWrappedPages = false;
      this.allColumnsSelected = false;
      this.printResult.pdfUrl = '';
      this.printResult.scoreSelected = [];
      this.printResult.subScoresSelected = [];
      this.printResult.layoutSelected = 'landscape';
      this.printResult.scoreDescriptionSelected = 'yes';
      this.printResult.rubricDescriptionSelected = 'yes';

      if (this.printResult.customScore.ScoreInfos.length) {
        this.printResult.isExistCustomScore = true;
      }
    },
    selectAllColumns: function () {
      var versionIE = this.detectIE();
      this.printResult.scoreSelected = [];
      this.printResult.subScoresSelected = [];

      if (versionIE !== false && versionIE >= 11) {
        this.allColumnsSelected = !this.allColumnsSelected;
      }

      if (!this.allColumnsSelected) {
        var i = 0;
        var lenI = 0;

        for (i = 0, lenI = this.printResult.customScore.ScoreInfos.length; i < lenI; i++) {
          var scoreInfo = this.printResult.customScore.ScoreInfos[i];
          this.printResult.scoreSelected.push(scoreInfo.ScoreName);
        }

        for (i = 0, lenI = this.printResult.customSubScores.length; i < lenI; i++) {
          var subScore = this.printResult.customSubScores[i];
          var subScoreName = subScore.Name;
          var subScoreInfos = subScore.ScoreInfos;

          for (var j = 0, lenJ = subScoreInfos.length; j < lenJ; j++) {
            var subScoreInfo = subScoreInfos[j];

            this.printResult.subScoresSelected.push(subScoreName + '::' + subScoreInfo.ScoreName);
          }
        }
      }
    },
    statusAllColumnsSelected: function () {
      var selected = this.printResult.scoreSelected.length + this.printResult.subScoresSelected.length;
      var originalSelected = this.printResult.customScore.ScoreInfos.length;

      for (var i = 0, len = this.printResult.customSubScores.length; i < len; i++) {
        var customSubScoresInfo = this.printResult.customSubScores[i];
        originalSelected += customSubScoresInfo.ScoreInfos.length;
      }

      this.allColumnsSelected = selected === originalSelected;
      this.printResult.layouts[2].isDisable = false;
      if (selected > 1) {
        this.printResult.layouts[2].isDisable = true;
        if (this.printResult.layoutSelected == 'labels') {
          this.printResult.layoutSelected = 'landscape';
        }
      } else if (selected == 1) {
        var score = this.printResult.scoreSelected.length > 0 ? this.printResult.scoreSelected[0] : '';
        var subscore = this.printResult.subScoresSelected.length > 0 ? this.printResult.subScoresSelected[0] : '';
        if (score == "Artifact" || subscore.indexOf("Artifact") != -1) {
          this.printResult.layouts[2].isDisable = true;
          if (this.printResult.layoutSelected == 'labels') {
            this.printResult.layoutSelected = 'landscape';
          }
        }
      }
    },
    getSubScorePartList: function (list) {

      var subScorePartList = [];
      var subScoreList = list.map(function (listItem) {
        var subscore = listItem.split('::');
        return subscore[0];
      });

      subScoreList = _.union(subScoreList);

      for (var i = 0, lenI = subScoreList.length; i < lenI; i++) {
        var subScoreItem = subScoreList[i];

        subScorePartList.push({
          Name: subScoreItem,
          SubScoreNameList: []
        });

        for (var j = 0, lenJ = list.length; j < lenJ; j++) {
          var listItem = list[j];
          var listItemPart = list[j].split('::');

          if (subScoreItem === listItemPart[0]) {
            subScorePartList[i].SubScoreNameList.push(listItemPart[1]);
          }
        }
      }

      return subScorePartList;
    },
    beforeGeneratePdf: function () {
      var self = this;
      var selectedColumns = self.printResult.scoreSelected.length + self.printResult.subScoresSelected.length;
      var selectedLayout = self.printResult.layoutSelected;

      if (selectedColumns > 6 && selectedLayout === 'portrait' ||
        selectedColumns > 8 && selectedLayout === 'landscape') {
        self.isWrappedPages = true;
      } else {
        self.generatePdf();
      }
    },
    generatePdf: function () {
      var self = this;
      var subScorePartList = this.getSubScorePartList(self.printResult.subScoresSelected);

      var studentIdArr = hansonStudentData.map(function (stData) {
        return stData["overallScore"]["StudentID"];
      });
      self.studentsIdSelectedString = studentIdArr.join(); //get again student selected incase autosaved pass by student selected in url
      var jsonOverallScores = "";
      var jsonSubScores = "";
      var arrayOverallValue = [];
      if (self.studentsIdSelectedString === "0") {
        var convertValueToLabelNumeric = self.convertValueToLabelNumeric();
        var arrayJson = JSON.parse(JSON.stringify(convertValueToLabelNumeric));
        var elem = {
          overallScores: [],
          subScores: []
        }
        for (var i in arrayJson) {
          for (var j in arrayJson[i]) {
            if (j.includes('subScore'))
              elem.subScores.push(arrayJson[i][j]);
            else
              elem.overallScores.push(arrayJson[i][j]);
          }
        }
        arrayOverallValue.push(elem.overallScores[0]);
        jsonOverallScores = JSON.stringify(arrayOverallValue);
        jsonSubScores = JSON.stringify(elem.subScores);
      }

      var params = {
        VirtualtestId: self.virtualTestId,
        ClassId: self.classId,
        StudentsIdSelectedString: self.studentsIdSelectedString,
        AllColumn: self.allColumnsSelected,
        OverrallScoreNameList: self.printResult.scoreSelected,
        SubScorePartList: subScorePartList,
        Layout: self.printResult.layoutSelected,
        ScoreDescription: self.printResult.scoreDescriptionSelected,
        IncludeRubricDescription: self.printResult.rubricDescriptionSelected,
        EntryResultDate: self.entryResultDate,
        StudentTestResultScores: jsonOverallScores,
        StudentTestResultSubscores: jsonSubScores,
        TimezoneOffset: (new Date()).getTimezoneOffset(),
        TemplateId: self.templateId,
      };

      self.isLoading = true;

      StudentEntryCtrl.generatePdf(self.generatePdfUrl, params)
        .done(function (response) {
          self.isLoading = false;
          self.isWrappedPages = true;
          self.isPrintSucess = true;
          self.printResult.pdfUrl = response;
        })
        .error(function (err) {
          self.isLoading = false;
        });
    },
    convertValueToLabelNumeric: function () {
      var cloneHansonStudentData = JSON.parse(JSON.stringify(hansonStudentData));
      $.each(cloneHansonStudentData, function (k, item) {
        var overallScore = item.overallScore;
        if (overallScore !== null) {
          overallScore.ScoreCustomN_1 = getValueFromLabelDropdown(overallScore.ScoreCustomN_1, "overallScore.ScoreCustomN_1");
          overallScore.ScoreCustomN_2 = getValueFromLabelDropdown(overallScore.ScoreCustomN_2, "overallScore.ScoreCustomN_2");
          overallScore.ScoreCustomN_3 = getValueFromLabelDropdown(overallScore.ScoreCustomN_3, "overallScore.ScoreCustomN_3");
          overallScore.ScoreCustomN_4 = getValueFromLabelDropdown(overallScore.ScoreCustomN_4, "overallScore.ScoreCustomN_4");
        }

        for (var i = 0; i < subScoreLength; i++) {
          var name = "subScore_" + i;
          var subscore = item[name];
          if (subscore !== null) {
            subscore.ScoreCustomN_1 = getValueFromLabelDropdown(subscore.ScoreCustomN_1, name + ".ScoreCustomN_1");
            subscore.ScoreCustomN_2 = getValueFromLabelDropdown(subscore.ScoreCustomN_2, name + ".ScoreCustomN_2");
            subscore.ScoreCustomN_3 = getValueFromLabelDropdown(subscore.ScoreCustomN_3, name + ".ScoreCustomN_3");
            subscore.ScoreCustomN_4 = getValueFromLabelDropdown(subscore.ScoreCustomN_4, name + ".ScoreCustomN_4");
          }
        }
      });
      return cloneHansonStudentData;
    },
    backGeneratePdf: function () {
      this.isWrappedPages = false;
      this.isPrintSucess = false;
      this.printResult.pdfUrl = '';
      this.printResult.scoreSelected = [];
      this.printResult.subScoresSelected = [];
      this.printResult.layoutSelected = 'landscape';
      this.printResult.scoreDescriptionSelected = 'yes';
      this.printResult.rubricDescriptionSelected = 'yes';
    },
    confirmSaveResults: function (isOpenPrintPopUp) {
      if (isOpenPrintPopUp) {
        this.isWarningPrint = false;
      } else {
        this.isWarningPrint = true;
      }
      saveResults().then(function () {
        if (StudentEntryModel.isClickNavSR) {
          navigateDataLockerToSR()
        }
      });
    },
    toggleWarningPublishToStudentPortal: function () {
      this.isShowModalWarningPublishToPortal = false;
    },
    toggleFullScreen: function (isMultiDate) {
      var $body = $('body');
      var hotStudentEntryResult = $('div[name="studentEntryResult"]').handsontable('getInstance');
      var HEIGHT_HS = 405;
      var HEIGHT_FULLSCREEN = isMultiDate ? 410 : 225;

      if ($body.hasClass('data-locker-fullscreen')) {
        $body.removeClass('data-locker-fullscreen');
        this.fullscreen = {
          icon: '<svg xmlns="http://www.w3.org/2000/svg" width="16" height="20" viewBox="0 0 100 125"><path d="M26.947 86.098H5V64.151h6.997v14.95h14.95zM11.997 35.849H5V13.902h21.947v6.997h-14.95zM95 35.849h-6.997v-14.95h-14.95v-6.997H95zM95 86.098H73.053v-6.997h14.95v-14.95H95z"/><g><path d="M82.405 73.503h-64.81V26.498h64.81v47.005zm-57.812-6.998h50.815v-33.01H24.593v33.01z"/></g></svg>',
          text: 'Fullscreen'
        };
        if (hotStudentEntryResult != null) {
          hotStudentEntryResult.updateSettings({
            width: '100%',
            height: HEIGHT_HS
          });
        }        
      } else {
        $body.addClass('data-locker-fullscreen');
        this.fullscreen = {
          icon: '<svg xmlns="http://www.w3.org/2000/svg" width="16" height="20" viewBox="0 0 100 125"><path d="M0 61.836h28.273v28.273h-7.774V69.611H0zM20.499 9.891h7.774v28.273H0v-7.775h20.499zM71.727 9.891h7.775v20.498H100v7.775H71.727zM71.727 61.836H100v7.775H79.502v20.498h-7.775z"/></svg>',
          text: 'Exit fullscreen'
        };
        if (hotStudentEntryResult != null) {
          hotStudentEntryResult.updateSettings({
            width: '100%',
            height: window.innerHeight - HEIGHT_FULLSCREEN
          });
        }        
        $('.ht_master .wtHolder').css('height', 'height', window.innerHeight - HEIGHT_FULLSCREEN + 'px');
      }

      $('#main-nav').find('.menu-item').hideTip();
    },
    resizeHandsontable: function (isMultiDate) {
      var $body = $('body');
      var hotStudentEntryResult = $('div[name="studentEntryResult"]').handsontable('getInstance');
      var HEIGHT_HS = 405;
      var HEIGHT_FULLSCREEN = isMultiDate ? 410 : 175;
      if (hotStudentEntryResult != null) {
        if ($body.hasClass('data-locker-fullscreen')) {
          hotStudentEntryResult.updateSettings({
            width: '100%',
            height: 'auto' //window.innerHeight - HEIGHT_FULLSCREEN
          });
          //$('.ht_master .wtHolder').css('height', window.innerHeight - HEIGHT_FULLSCREEN + 'px');
          $('.ht_master .wtHolder').css('height', 'auto');
        } else {
          hotStudentEntryResult.updateSettings({
            width: '100%',
            height: 'auto' //HEIGHT_HS
          });
        }
      }      
    },
    viewSupportingDocument: function () {
      var url = this.downloadSupportDocumentUrl + this.virtualtestFileKey;
      var rubricEl = document.createElement('a');
      rubricEl.setAttribute('href', url);
      rubricEl.setAttribute('target', '_blank');
      rubricEl.click();
      //window.location = url;
    },
    detectIE: function () {
      var ua = window.navigator.userAgent;

      var msie = ua.indexOf('MSIE ');
      if (msie > 0) {
        // IE 10 or older => return version number
        return parseInt(ua.substring(msie + 5, ua.indexOf('.', msie)), 10);
      }

      var trident = ua.indexOf('Trident/');
      if (trident > 0) {
        // IE 11 => return version number
        var rv = ua.indexOf('rv:');
        return parseInt(ua.substring(rv + 3, ua.indexOf('.', rv)), 10);
      }

      var edge = ua.indexOf('Edge/');
      if (edge > 0) {
        // Edge (IE 12+) => return version number
        return parseInt(ua.substring(edge + 5, ua.indexOf('.', edge)), 10);
      }

      // other browser
      return false;
    },

    resetArtifactModalData: function () {
      this.inputLink = '';
      this.artifactFolderModalVM.currentScore = "";
      this.artifactFolderModalVM.inputLink = "";
      this.artifactFolderModalVM.artifacts = [];
      this.artifactFolderModalVM.msgError = "";
      this.artifactFolderModalVM.isOverallScore = false;
      this.fileName = '';
      this.uploadFile = 'uploadFile';
      this.artifact.msgError = '';
      this.artifactFolderModalVM.isShowPopUpModalErrorFileUploadMass = false;
      this.artifactFolderModalVM.objectPopUpModalErrorFileUploadMass = null;
    },

    openPopupNote: function (noteId, noteValue) {
      this.isShowNote = true;
      var defaultValue = this.getDefaultValueByColumn(this.selectedCol);
      if (noteValue && noteValue != '' && noteValue != 'null') {
        var objectNote = JSON.parse(noteValue);
        CKEDITOR.instances['editor1'].setData(objectNote.Notes[0].Content);
      } else if (defaultValue != null) {
        CKEDITOR.instances['editor1'].setData(this.decodeHtml(defaultValue));
      } else {
        CKEDITOR.instances['editor1'].setData('');
      }
      this.noteId = noteId;
    },

    closePopupNote: function () {
      this.isShowNote = false;
    },

    okPopupNote: function () {
      this.isShowNote = false;
      var noteValue = CKEDITOR.instances['editor1'].getData();
      var objectNoteValue = {
        Notes: [{
          NoteType: "default",
          Content: noteValue
        }]
      };
      objectNoteValue = JSON.stringify(objectNoteValue);

      if (noteValue == '' || noteValue == null) {
        objectNoteValue = '';
      }

      var isEmpty = noteValue == '' || noteValue == null ? true : false;
      this.setNoteToAttribute(objectNoteValue, isEmpty);
      this.setNoteToTable(objectNoteValue, 'default');
    },

    setNoteToTable: function (noteValue, noteType) {
      var tempItemDelete = [];
      var isOverallScore = false;
      var subScoreName = '';
      var colName = columnData[this.selectedCol].data;
      var subColName = colName.split('.');
      if (colName.indexOf('overallScore') > -1) {
        isOverallScore = true;
      } else {
        isOverallScore = false;
        var subNameArr = colName.split('.');
        if (subNameArr.length > 0) {
          subScoreName = subNameArr[0];
        }
      }
      var dataColCheckbox = $('div[name="studentEntryResult"]').handsontable('getDataAtCol', 0);

      if (isOverallScore) {
        //Update current row
        hansonStudentData[this.selectedRow].overallScore[subColName[1]] = noteValue;
        if (this.noteId != '') {
          for (var i = 0; i < hansonStudentData[this.selectedRow].overallScore.Notes.length; i++) {
            var noteArrItem = hansonStudentData[this.selectedRow].overallScore.Notes[i];
            if (noteArrItem.NoteKey == subColName[1]) {
              if (noteType == 'date') {
                tempItemDelete = this.getDeletedItem(noteArrItem.Note, noteValue);
              }
              noteArrItem.Note = noteValue;
            }
          }
        } else {
          this.pushItemToNoteArray(this.selectedRow, subColName, noteValue);
        }

        //Update multiple row
        for (var m = 0; m < dataColCheckbox.length; m++) {
          if (dataColCheckbox[m] == true) {
            hansonStudentData[m][""] = false;
            hansonStudentData[m].overallScore[subColName[1]] = noteValue;
            var arrNoteKey = [];
            if (hansonStudentData[m].overallScore.Notes.length > 0) {
              for (var i = 0; i < hansonStudentData[m].overallScore.Notes.length; i++) {
                var noteArrItem = hansonStudentData[m].overallScore.Notes[i];
                if (noteArrItem.NoteKey == subColName[1]) {
                  if (noteType == 'date') {
                    var newNoteValue = this.buildNoteDateForOtherRow(noteArrItem.Note, noteValue, tempItemDelete);
                    noteArrItem.Note = newNoteValue;
                  } else {
                    noteArrItem.Note = noteValue;
                  }
                }
                arrNoteKey.push(noteArrItem.NoteKey);
                if (arrNoteKey.includes(subColName[1]) == false) {
                  this.pushItemToNoteArray(m, subColName, noteValue);
                }
              }
            } else {
              this.pushItemToNoteArray(m, subColName, noteValue);
            }
          }
        }
      } else if (!!subScoreName) {
        hansonStudentData[this.selectedRow][subScoreName][subColName[1]] = noteValue;
        if (this.noteId != '') {
          for (var i = 0; i < hansonStudentData[this.selectedRow][subScoreName].Notes.length; i++) {
            var noteArrItem = hansonStudentData[this.selectedRow][subScoreName].Notes[i];
            if (noteArrItem.NoteKey == subColName[1]) {
              if (noteType == 'date') {
                tempItemDelete = this.getDeletedItem(noteArrItem.Note, noteValue);
              }
              noteArrItem.Note = noteValue;
            }
          }
        } else {
          this.pushItemToNoteArraySub(this.selectedRow, subColName, subScoreName, noteValue);
        }

        //Update multiple row
        for (var m = 0; m < dataColCheckbox.length; m++) {
          if (dataColCheckbox[m] == true) {
            hansonStudentData[m][""] = false;
            hansonStudentData[m][subScoreName][subColName[1]] = noteValue;
            var arrNoteKey = [];
            if (hansonStudentData[m][subScoreName].Notes.length > 0) {
              for (var i = 0; i < hansonStudentData[m][subScoreName].Notes.length; i++) {
                var noteArrItem = hansonStudentData[m][subScoreName].Notes[i];
                if (noteArrItem.NoteKey == subColName[1]) {
                  if (noteType == 'date') {
                    var newNoteValue = this.buildNoteDateForOtherRow(noteArrItem.Note, noteValue, tempItemDelete);
                    noteArrItem.Note = newNoteValue;
                  } else {
                    noteArrItem.Note = noteValue;
                  }
                }
                arrNoteKey.push(noteArrItem.NoteKey);
                if (arrNoteKey.includes(subColName[1]) == false) {
                  this.pushItemToNoteArraySub(m, subColName, subScoreName, noteValue);
                }
              }
            } else {
              this.pushItemToNoteArraySub(m, subColName, subScoreName, noteValue);
            }

          }
        }
      }
    },

    setNoteToAttribute: function (noteValue, isEmpty) {
      var value = noteValue;

      //Current row
      var td = this.selectedTD;
      var $tag = $(td).find('.note');
      var link = $tag.get(0);
      var button = $tag.get(1);
      button.setAttribute('note-value', value);
      link.setAttribute('note-value', value);
      if (isEmpty == false) {
        $(link).hide();
        $(button).show();
      } else {
        $(link).show();
        $(button).hide();
      }
    },

    pushItemToNoteArray: function (row, subColName, noteValue) {
      var name = null;
      for (var i = 0; i < this.printResult.customScore.ScoreInfos.length; i++) {
        var item = this.printResult.customScore.ScoreInfos[i];
        if (item.ScoreName == subColName[1]) {
          name = item.ScoreLable;
          break;
        }
      }

      var testResulScoreID = hansonStudentData[row].overallScore.TestResultScoreID == null ? 0 : hansonStudentData[this.selectedRow].overallScore.TestResultScoreID;
      var item = {
        "Name": name,
        "Note": noteValue,
        "NoteKey": subColName[1],
        "TestResultScoreID": testResulScoreID,
        "TestResultScoreNoteID": 0
      };
      hansonStudentData[row].overallScore.Notes.push(item);
    },

    pushItemToNoteArraySub: function (row, subColName, subScoreName, noteValue) {
      var subCol = subColName[0].split('_');
      var name = null;
      for (var i = 0; i < this.printResult.customSubScores[subCol[1]].ScoreInfos.length; i++) {
        var item = this.printResult.customSubScores[subCol[1]].ScoreInfos[i];
        if (item.ScoreName == subColName[1]) {
          name = item.ScoreLable;
          break;
        }
      }
      var item = {
        "Name": name,
        "Note": noteValue,
        "NoteKey": subColName[1],
        "TestResultSubScoreID": hansonStudentData[row][subScoreName].TestResultScoreSubID == null ? 0 : hansonStudentData[this.selectedRow][subScoreName].TestResultScoreSubID,
        "TestResultSubScoreNoteID": 0
      };

      var temp = JSON.parse(JSON.stringify(hansonStudentData[row][subScoreName].Notes))
      temp.push(item);
      hansonStudentData[row][subScoreName].Notes = temp;
    },

    getDefaultValueByColumn: function (selectedCol) {
      var defaultValue = '';
      var colName = columnData[selectedCol].data;
      var subColName = colName.split('.');
      if (subColName[0] === "overallScore") {
        for (var i = 0; i < this.printResult.customScore.ScoreInfos.length; i++) {
          var item = this.printResult.customScore.ScoreInfos[i];
          if (subColName[1] === item.ScoreName && item.MetaData.DefaultValue != null) {
            defaultValue = item.MetaData.DefaultValue;
            break;
          }
        }
      } else {
        for (var i = 0; i < this.printResult.customSubScores[subColName[0].split('_')[1]].ScoreInfos.length; i++) {
          var item = this.printResult.customSubScores[subColName[0].split('_')[1]].ScoreInfos[i];
          if (subColName[1] === item.ScoreName && item.MetaData.DefaultValue != null) {
            defaultValue = item.MetaData.DefaultValue;
            break;
          }
        }
      }
      return defaultValue;
    },

    decodeHtml: function (str) {
      var map = {
        '&amp;': '&',
        '&lt;': '<',
        '&gt;': '>',
        '&quot;': '"',
        '&#039;': "'"
      };
      return str.replace(/&amp;|&lt;|&gt;|&quot;|&#039;/g, function (m) {
        return map[m];
      });
    },

    setIntervalAutoSaved: function () {
      this.$options.intervalAutoSaved = setInterval(autoSaveResults, INTERVAL_AUTO_SAVED);
    },

    initArtifactFolderModal: function () {
      this.artifactFolderModalVM.artifacts.forEach(function (art, i, arr) {
        if (!art.IsLink && !art.DocumentGuid) {
          arr[i].DisplayName = StudentEntryModel.getFileDisplayName(arr[i].Name);
        } else {
          arr[i].DisplayName = arr[i].Name;
          if (!art.DocumentGuid) {
            arr[i].Url = arr[i].Name.startsWith("http") ? arr[i].Name : "//" + arr[i].Name;
          }
        }
      });
      this.$nextTick(function () {
        initializeTooltip('#tableArtifactFolder tbody tr a[title]', {
          attr: 'title',
        });
        initializeTooltip('#tableArtifactFolder .btn-icon--preview', {
          attr: 'title',
        });
      })
    },

    getFileDisplayName: function (filename) {
      var ext = filename.split('.').pop();
      var displayName = filename.substring(0, filename.lastIndexOf('-'));
      return (displayName || filename) + '.' + ext;
    },

    removeArtifact: function (artifactname) {
      this.artifactFolderModalVM.artifacts = this.artifactFolderModalVM.artifacts.filter(function (art) {
        return art.Name !== artifactname
      });
    },

    removeArtifactByDisplayName: function (fileName) {
      var vm = this;
      vm.artifactFolderModalVM.artifacts = vm.artifactFolderModalVM.artifacts.filter(function (art) {
        return StudentEntryModel.getFileDisplayName(art.Name) !== fileName;
      });
    },
    removeArtifactByName: function (fileName) {
      var vm = this;
      vm.artifactFolderModalVM.artifacts = vm.artifactFolderModalVM.artifacts.filter(function (art) {
        return art.Name.replace(/\.[^/.]+$/, "") !== fileName.replace(/\.[^/.]+$/, "");
      });
    },
    addNewArtifact: function (fileName, fileUrl, isLink, tagValue, documentGuid) {
      var self = this;
      var extension = fileName.toLowerCase().replace(/^.*\./, '')
      var fileType = Object.keys(prevewExtsSuport).find(function (key) { return prevewExtsSuport[key].includes(extension) }) || 'document';
      var fileArtifact = {
        Name: fileName,
        DisplayName: fileName,
        IsLink: isLink,
        Url: fileUrl,
        UploadDate: new Date(),
        TagValue: tagValue,
        DocumentGuid: documentGuid,
        FileType: fileType,
        StudentID: self.handleGetStudentIdDefault(fileName, self.artifactFolderModalVM.Students)
      };
      if (self.artifactFolderModalVM.isMassUpload) {
        fileArtifact.isFromMassUpload = true;
      }
      if (fileArtifact.StudentID != null && fileArtifact.StudentID != '') {
        self.artifactFolderModalVM.artifacts.push(fileArtifact);
      } else {
        self.artifactFolderModalVM.artifacts.splice(0, 0, fileArtifact);
      }    },

    addLinkArtifact: function () {
      var newLink = this.artifactFolderModalVM.inputLink;
      if (newLink && newLink != '' && this.validateLink(newLink)) {
        var tagValue = null;
        if (this.tagList != null && this.tagList.length > 0) {
          tagValue = this.tagList[0];
        }

        var newLinkAbsUrl = newLink.startsWith("http") ? newLink : "//" + newLink;
        this.addNewArtifact(newLink, newLinkAbsUrl, true, tagValue);
        this.artifactFolderModalVM.inputLink = '';
        this.artifactFolderModalVM.msgError = '';
      }
    },

    closeArtifactModal: function () {
      this.resetArtifactModalData();
      this.isShowModalArtifactFolder = false;
    },

    okFolderModal: function () {
      this.updateArtifacts(this.artifactFolderModalVM.artifacts);
    },

    updateArtifacts: function (artifacts) {
      if (this.artifactFolderModalVM.isMassUpload) {
        let checkNotStudent = artifacts.filter((m) => { return m.StudentID === undefined || m.StudentID === "" || m.StudentID === null })
        if (checkNotStudent != null && checkNotStudent.length > 0) {
          this.isShowModalWarningChooseStudent = true;
          this.artifactFolderModalVM.artifactsWarning = artifacts;
          return;
        }
        else {
          hansonStudentData.forEach((m) => {
            if (this.artifactFolderModalVM.subName) {
              let updateArtifacts = artifacts.filter((n) => { return n.StudentID === m.overallScore?.StudentID });
              if (updateArtifacts && updateArtifacts.length > 0) {
                m[this.artifactFolderModalVM.subName].Artifacts = JSON.parse(JSON.stringify(updateArtifacts));
              }
              else {
                m[this.artifactFolderModalVM.subName].Artifacts = [];
              }
            }
            else {
              let updateArtifacts = artifacts.filter((n) => { return n.StudentID === m.overallScore?.StudentID });
              if (updateArtifacts && updateArtifacts.length > 0) {
                m.overallScore.Artifacts = JSON.parse(JSON.stringify(updateArtifacts));
              }
              else {
                m.overallScore.Artifacts = [];
              }
            }
          });
          this.closeArtifactModal();
        }
        return;
      }
      if (this.artifactFolderModalVM.isOverallScore) {
        hansonStudentData[this.selectedRow].overallScore.Artifacts = JSON.parse(JSON.stringify(artifacts));
      } else {
        var subScoreName = columnData[this.selectedCol].data.split('.')[0];
        hansonStudentData[this.selectedRow][subScoreName].Artifacts = JSON.parse(JSON.stringify(artifacts));
      }

      var dataColCheckbox = $('div[name="studentEntryResult"]').handsontable('getDataAtCol', 0);

      if (dataColCheckbox.length > 0) {
        var duplicateStudentNames = [];
        duplicateStudentNames = this.checkDuplicateUpdateMultipleFiles(artifacts);
        var isDuplicate = duplicateStudentNames.length > 0 ? true : false;

        if (isDuplicate) {
          this.openOverWriteConfirmDialog(duplicateStudentNames);
        } else {
          this.updateArtifactsMultipleRow(artifacts, isDuplicate, false);
          this.closeArtifactModal();
        }
      } else {
        this.closeArtifactModal();
      }
    },

    checkDuplicateUpdateMultipleFiles: function (newArtifacts) {
      var dataColCheckbox = $('div[name="studentEntryResult"]').handsontable('getDataAtCol', 0);

      var duplicateStudentNames = [];
      // check duplicates
      if (this.artifactFolderModalVM.isOverallScore) {
        for (var m = 0; m < dataColCheckbox.length; m++) {
          if (m != this.selectedRow && dataColCheckbox[m] == true) {
            if (this.checkDuplicateFilenames(hansonStudentData[m].overallScore.Artifacts, newArtifacts)) {
              duplicateStudentNames.push(hansonStudentData[m].overallScore.FullName);
            }
          }
        }
      } else {
        var subScoreName = columnData[this.selectedCol].data.split('.')[0];
        for (var m = 0; m < dataColCheckbox.length; m++) {
          if (m != this.selectedRow && dataColCheckbox[m] == true) {
            if (this.checkDuplicateFilenames(hansonStudentData[m][subScoreName].Artifacts, newArtifacts)) {
              duplicateStudentNames.push(hansonStudentData[m].overallScore.FullName);
            }
          }
        }
      }

      return duplicateStudentNames;
    },

    pushArrayToArray: function (arr1, arr2) {
      arr2.forEach(function (item) {
        arr1.push(item);
      });
    },

    updateArtifactsForEachRow: function (rowArtfiacts, newArtifacts, isDuplicate, isOverwrite) {
      var vm = this;
      if (isDuplicate) {
        if (isOverwrite) {
          for (var i = rowArtfiacts.length - 1; i >= 0; i--) {
            if (vm.checkArtifactExist(newArtifacts, rowArtfiacts[i]))
              rowArtfiacts.splice(i, 1);
          }
          vm.pushArrayToArray(rowArtfiacts, newArtifacts);

        } else {
          newArtifacts.forEach(function (nart) {
            if (!vm.checkArtifactExist(rowArtfiacts, nart)) {
              rowArtfiacts.push(nart);
            }
          });
        }
      } else {
        newArtifacts.forEach(function (nart) { // not overwrite link
          if (!vm.checkArtifactExist(rowArtfiacts, nart)) {
            rowArtfiacts.push(nart);
          } else {
            for (var i = 0; i < rowArtfiacts.length; i++) {
              if (rowArtfiacts[i].Name == nart.Name) {
                rowArtfiacts[i].TagValue = nart.TagValue;
              }
            }
          }
        });
      }

      return rowArtfiacts;
    },

    updateArtifactsMultipleRow: function (newArtifacts, isDuplicate, isOverwrite) {
      var dataColCheckbox = $('div[name="studentEntryResult"]').handsontable('getDataAtCol', 0);

      if (this.artifactFolderModalVM.isOverallScore) {
        for (var m = 0; m < dataColCheckbox.length; m++) {
          if (m != this.selectedRow && dataColCheckbox[m] == true) {
            hansonStudentData[m][""] = false;
            hansonStudentData[m].overallScore.Artifacts = JSON.parse(JSON.stringify(this.updateArtifactsForEachRow(hansonStudentData[m].overallScore.Artifacts, newArtifacts, isDuplicate, isOverwrite)));
          }
        }
      } else {
        var subScoreName = columnData[this.selectedCol].data.split('.')[0];
        for (var m = 0; m < dataColCheckbox.length; m++) {
          if (m != this.selectedRow && dataColCheckbox[m] == true) {
            hansonStudentData[m][""] = false;
            hansonStudentData[m][subScoreName].Artifacts = JSON.parse(JSON.stringify(this.updateArtifactsForEachRow(hansonStudentData[m][subScoreName].Artifacts, newArtifacts, isDuplicate, isOverwrite)));
          }
        }
      }
      hansonStudentData[this.selectedRow][""] = false;
    },

    showRubricDescription: function () {
      this.isShowModalRubricDescription = true;
    },

    checkArtifactExist: function (artifacts, newArtifact) {
      var exist = _.find(artifacts, function (art) {
        if (!newArtifact.IsLink) {
          return StudentEntryModel.getFileDisplayName(art.Name) == StudentEntryModel.getFileDisplayName(newArtifact.Name);
        } else {
          return art.Name == newArtifact.Name
        }
      });
      return exist ? true : false;
    },

    checkDuplicateFilenames: function (oldArtifacts, newArtifacts) {
      var dupCheck = false;
      oldArtifacts.some(function (art) {
        if (!art.IsLink && StudentEntryModel.checkArtifactExist(newArtifacts, art)) {
          dupCheck = true;
          return true;
        }
      });
      return dupCheck
    },

    getMaxZindexExist: function (modals) {
      var m = [];
      modals.each(function (i, modal) {
        var modalZindex = parseInt($(modal).css('z-index'), 10)
        m.push(modalZindex)
      })
      var result = Math.max(m)
      return result;
    },

    yesConfirmOverwrite: function () {
      this.updateArtifactsMultipleRow(StudentEntryModel.artifactFolderModalVM.artifacts, true, true);
      StudentEntryModel.closeArtifactModal();
    },

    noConfirmOverwrite: function () {
      this.updateArtifactsMultipleRow(StudentEntryModel.artifactFolderModalVM.artifacts, true, false);
      StudentEntryModel.closeArtifactModal();
    },

    openOverWriteConfirmDialog: function (duplicateStudentNames) {
      var duplicateStudentNamesStr = duplicateStudentNames.join('; ');
      var message = 'Some files with the same name exist for Student: ' + duplicateStudentNamesStr + '.<br>Do you want to replace?';

      customConfirm(message, {
        minWidth: '440px',
        ZIndex: 10000,
        buttons: [
            {
                label: 'Cancel',
                color: 'grey',
                style: "background: none;"
            },
            {
                label: 'No',
                color: 'red',
                callback: function() {
                  StudentEntryModel.noConfirmOverwrite()
                }
            },
            {
                label: 'Yes',
                color: 'red',
                callback: function() {
                  StudentEntryModel.yesConfirmOverwrite()
                }
            }
        ]
      });
      $('.foot-content').filter(':visible').css({
        justifyContent: '',
        gap: 12
      })
    },

    showModalDialogBGCustom: function (zindex) {
      var win = $('body');
      $('body').prepend('<div class="ui-widget-overlay" style="width: ' + win.width() + 'px; height: ' + win.height() + 'px; z-index:' + zindex + ';"></div>');
    },

    isUrl: function (str) {
      var matcher = /^(http:\/\/www\.|https:\/\/www\.|http:\/\/|https:\/\/)?[a-z0-9]+([\-\.]{1}[a-z0-9]+)*\.[a-z]{2,5}(:[0-9]{1,5})?(\/.*)?$/;
      return matcher.test(str);
    },

    validateLink: function (inputLink) {
      var regex = /\w+\.([A-Za-z0-9]{3,4})(?=\?|$)/;
      var execFile = 'exe,bat,msi';
      if (inputLink && !this.isUrl(inputLink)) {
        this.artifactFolderModalVM.msgError = 'Invalid link.';
        return false;
      } else if (inputLink) {
        var matchs = inputLink.match(regex);
        if (!!matchs && execFile.indexOf(matchs[1]) > -1) {
          this.artifactFolderModalVM.msgError = 'Invalid link. You should not upload URLs that link to executable files.';
          return false;
        }
      }
      for (var i = 0, length = this.artifactFolderModalVM.artifacts.length; i < length; i++) {
        if (this.artifactFolderModalVM.artifacts[i].Name.toLowerCase() == inputLink.toLowerCase()) {
          this.artifactFolderModalVM.msgError = 'This link has been added.';
          return false;
        }
      }
      return true;
    },

    openPopupNoteDate: function (noteId, noteValue) {
      this.arrayNoteDate = [];
      $("#noteDate").datepicker("refresh");
      $('#noteDate').datepicker('setDate', new Date());
      var defaultValue = this.getDefaultValueByColumn(this.selectedCol);
      var currentDate = new Date();
      this.noteDate = currentDate.toISOString().substring(0, 10);
      this.isShowNoteDate = true;

      if (noteValue && noteValue != '' && noteValue != 'null') {
        var objectNote = JSON.parse(noteValue);
        this.arrayNoteDate = objectNote.Notes;
        var noteDateContent = defaultValue != null ? this.decodeHtml(defaultValue) : '';
        this.highlightDatePicker(currentDate.getFullYear());
        for (var i = 0; i < objectNote.Notes.length; i++) {
          if (objectNote.Notes[i].NoteDate == this.noteDate && objectNote.Notes[i].Content != '') {
            noteDateContent = objectNote.Notes[i].Content;
            break;
          }
        }
        CKEDITOR.instances['editorNoteDate'].setData(noteDateContent);
      } else if (defaultValue != null) {
        CKEDITOR.instances['editorNoteDate'].setData(this.decodeHtml(defaultValue));
      } else {
        CKEDITOR.instances['editorNoteDate'].setData('');
      }

      this.noteId = noteId;
    },

    loadNoteDate: function (selectedDate) {
      var defaultValue = this.getDefaultValueByColumn(this.selectedCol);
      for (var i = 0; i < this.arrayNoteDate.length; i++) {
        var item = this.arrayNoteDate[i];
        if (item.NoteDate == selectedDate && item.Content != '' && item.Content != null) {
          CKEDITOR.instances['editorNoteDate'].setData(item.Content);
          return;
        }
      }
      if (defaultValue != null) {
        CKEDITOR.instances['editorNoteDate'].setData(this.decodeHtml(defaultValue));
      } else {
        CKEDITOR.instances['editorNoteDate'].setData('');
      }
    },

    closePopupNoteDate: function () {
      this.isShowNoteDate = false;
      this.arrayNoteDate = [];
    },

    addNoteDate: function () {
      for (var i = 0; i < this.arrayNoteDate.length; i++) {
        if (this.noteDate == this.arrayNoteDate[i].NoteDate) {
          this.arrayNoteDate[i].Content = CKEDITOR.instances['editorNoteDate'].getData();
          if (CKEDITOR.instances['editorNoteDate'].getData() == '') {
            this.arrayNoteDate.splice(i, 1);
            this.highlightDatePicker(new Date(this.noteDate).getFullYear());
          }
          return;
        }
      }
      if (CKEDITOR.instances['editorNoteDate'].getData() == '') {
        return;
      }
      var item = {
        NoteType: "date",
        NoteDate: this.noteDate,
        Content: CKEDITOR.instances['editorNoteDate'].getData()
      }
      this.arrayNoteDate.push(item);
      this.highlightDatePicker(new Date(this.noteDate).getFullYear());
    },

    okPopupNoteDate: function () {
      this.addNoteDate();
      this.isShowNoteDate = false;
      var objectNoteValue = {
        Notes: this.arrayNoteDate
      }
      var isEmpty = this.arrayNoteDate.length < 1 ? true : false;
      this.setNoteToAttribute(JSON.stringify(objectNoteValue), isEmpty);
      this.setNoteToTable(JSON.stringify(objectNoteValue), 'date');
    },

    highlightDatePicker: function (selectedYear) {
      var arrMonth = [];
      var arrYear = [];
      for (var i = 0; i < this.arrayNoteDate.length; i++) {
        var date = new Date(this.arrayNoteDate[i].NoteDate);
        var month = date.getMonth();
        var year = date.getFullYear();

        if (year == selectedYear) {
          arrMonth.push(month);
        }
        arrYear.push(year);
      }
      arrMonth = this.removeDuplicates(arrMonth);
      arrYear = this.removeDuplicates(arrYear);

      $("#noteDate").datepicker("refresh");
      setTimeout(function () {
        for (var i = 0; i < arrMonth.length; i++) {
          $("#noteDate .ui-datepicker-month option[value='" + arrMonth[i] + "']").css("background-color", "#42B373");
        }
        for (var j = 0; j < arrYear.length; j++) {
          $("#noteDate .ui-datepicker-year option[value='" + arrYear[j] + "']").css("background-color", "#42B373");
        }
      }, 500);
    },

    removeDuplicates: function (arr) {
      var unique_array = [];
      for (var i = 0; i < arr.length; i++) {
        if (unique_array.indexOf(arr[i]) == -1) {
          unique_array.push(arr[i]);
        }
      }
      return unique_array;
    },

    getDeletedItem: function (oldArrayText, newArrayText) {
      var result = [];
      var oldArray = JSON.parse(oldArrayText).Notes;
      var newArray = JSON.parse(newArrayText).Notes;
      for (var i = 0; i < oldArray.length; i++) {
        var oldItem = oldArray[i];
        var isDeleted = true;
        for (var j = 0; j < newArray.length; j++) {
          var newItem = newArray[j];
          if (oldItem.NoteDate == newItem.NoteDate) {
            isDeleted = false;
            break;
          }
        }
        if (isDeleted == true) {
          result.push(oldItem);
        }
      }
      return result;
    },

    buildNoteDateForOtherRow: function (oldArrayText, newArrayText, deletedArray) {
      var oldArray = JSON.parse(oldArrayText).Notes;
      var newArray = JSON.parse(newArrayText).Notes;
      //Update
      for (var i = 0; i < oldArray.length; i++) {
        var oldItem = oldArray[i];
        for (var n = 0; n < newArray.length; n++) {
          var newItem = newArray[n];
          if (oldItem.NoteDate == newItem.NoteDate) {
            oldItem.Content = newItem.Content;
          }
        }
      }

      //Delete
      for (var d = 0; d < deletedArray.length; d++) {
        var deletedItem = deletedArray[d];
        for (var i = 0; i < oldArray.length; i++) {
          var oldItem = oldArray[i];
          if (oldItem.NoteDate == deletedItem.NoteDate) {
            oldArray = this.removeItemArray(oldArray, deletedItem.NoteDate);
          }
        }
      }

      //Add
      for (var i = 0; i < newArray.length; i++) {
        var newItem = newArray[i];
        var isNewItem = true;
        for (var j = 0; j < oldArray.length; j++) {
          var oldItem = oldArray[j];
          if (oldItem.NoteDate == newItem.NoteDate) {
            isNewItem = false;
            break;
          }
        }
        if (isNewItem == true) {
          oldArray.push(newItem);
        }
      }
      var result = {
        Notes: oldArray
      }
      return JSON.stringify(result);
    },

    removeItemArray: function (oldArray, noteDate) {
      var result = oldArray;
      for (var i = 0; i < result.length; i++) {
        if (result[i].NoteDate == noteDate) {
          result.splice(i, 1);
        }
      }
      return result;
    },

    toggleRubricDescriptionModal: function () {
      this.isShowModalRubricDescription = !this.isShowModalRubricDescription;
    },

    toggleFilterWarning: function () {
      this.isShowModalFilterWarning = !this.isShowModalFilterWarning;
    },

    checkFilterWarning: function (entryResultDate) {
      var isDataDirty = oldHandsonStudentData != JSON.stringify(hansonStudentData);
      if (isDataDirty) {
        this.toggleFilterWarning();
      } else {
        this.isWarningPrint = false;
        loadStudentEntryResult(true, entryResultDate);
      }
    },

    highlightDatePickerResultDate: function (selectedYear, arrayResultDate) {
      var arrMonth = [];
      var arrYear = [];
      for (var i = 0; i < arrayResultDate.length; i++) {
        var date = new Date(arrayResultDate[i].NoteDate);
        var month = date.getMonth();
        var year = date.getFullYear();

        if (year == selectedYear) {
          arrMonth.push(month);
        }
        arrYear.push(year);
      }
      arrMonth = this.removeDuplicates(arrMonth);
      arrYear = this.removeDuplicates(arrYear);

      $("#resultDate").datepicker("refresh");
      setTimeout(function () {
        for (var i = 0; i < arrMonth.length; i++) {
          $("#ui-datepicker-div .ui-datepicker-month option[value='" + arrMonth[i] + "']").css("background-color", "#42B373");
        }
        for (var j = 0; j < arrYear.length; j++) {
          $("#ui-datepicker-div .ui-datepicker-year option[value='" + arrYear[j] + "']").css("background-color", "#42B373");
        }
      }, 500);
    },

    confirmYesWarningChooseStudent: function () {
      let artifacts = this.artifactFolderModalVM.artifactsWarning.filter(f => { return f.StudentID && f.StudentID != "" });
      hansonStudentData.forEach((m) => {
        if (this.artifactFolderModalVM.subName) {
          let updateArtifacts = artifacts.filter((n) => { return n.StudentID === m.overallScore?.StudentID });
          if (updateArtifacts && updateArtifacts.length > 0) {
            m[this.artifactFolderModalVM.subName].Artifacts = JSON.parse(JSON.stringify(updateArtifacts));
          }
          else {
            m[this.artifactFolderModalVM.subName].Artifacts = [];
          }
        }
        else {
          let updateArtifacts = artifacts.filter((n) => { return n.StudentID === m.overallScore?.StudentID });
          if (updateArtifacts && updateArtifacts.length > 0) {
            m.overallScore.Artifacts = JSON.parse(JSON.stringify(updateArtifacts));
          }
          else {
            m.overallScore.Artifacts = [];
          }
        }
      })
      this.isShowModalWarningChooseStudent = !this.isShowModalWarningChooseStudent;
      this.closeArtifactModal();
    },
    toggleWarningChooseStudent: function () {
      this.isShowModalWarningChooseStudent = !this.isShowModalWarningChooseStudent;
    },
    handleArtifactClick: function (ev, item) {
      if (item.DocumentGuid) {
        new EdmService().getStreamFileUrl(item.DocumentGuid).then(function (url) {
          window.open(url, '_blank');
        });
      }
    },
    handleUploadFiles: function (files) {
      var self = this;
      var errorFiles = [];
      ShowBlock($('.modalUploadFile'), 'Uploading');
      return self.uploadFiles(files).then(function (results) {
        results.forEach(function (m) {
          if (m.error) {
            errorFiles.push(m.error)
            return;
          }
          var result = m.result;
          if (self.handleCheckFileExist(self.artifactFolderModalVM.artifacts, result.item.name)) {
            self.removeArtifactByName(result.item.name);
          }
          self.addNewArtifact(result.item.name, null, false, null, result.DocumentGuid)
        });
        if (errorFiles.length) {
          var messageSize = '*The following files were uploaded unsuccessfully (' + errorFiles.length + ' file(s)):';
          messageSize += '<p>';
          messageSize += '<ul style="list-style: inside; padding-left: 2%; margin-bottom: 0px; ">';
          errorFiles.forEach(function (name) {
            messageSize += ('<li style="padding-bottom: 5px">' + name + '</li>')
          })
          messageSize += '</ul>';
          messageSize += '</p>';
          self.artifactFolderModalVM.objectPopUpModalErrorFileUploadMass = messageSize;
          self.artifactFolderModalVM.isShowPopUpModalErrorFileUploadMass = true;
        } else {
          self.artifactFolderModalVM.objectPopUpModalErrorFileUploadMass = null;
          self.artifactFolderModalVM.isShowPopUpModalErrorFileUploadMass = false;
        }
      }).finally(function () {
        self.resetErrorUploadFile();
        $('.modalUploadFile').unblock();
        self.$nextTick(function () {
          initializeTooltip('#tableArtifactFolder tbody tr a[title]', {
            attr: 'title',
          });
          initializeTooltip('#tableArtifactFolder .btn-icon--preview', {
            attr: 'title',
          });
        })
      });
    },
    handlerSessionKeepAlive() {
      var self = this;
      var timeout = function () {
        handle()
        setTimeout(function () {
          if (self.triggerKeepAlive) {
            timeout();
          }
        }, 30000)
      }
      var handle = function () {
        SessionTimeOutComponent.handlerSessionKeepAlive();
        SessionTimeOutComponent.sessionKeepAlive();
      }
      timeout();
    },
    uploadFiles: function (files) {
      var self = this;
      var numberOfFiles = files.length;
      self.triggerKeepAlive = true;
      self.handlerSessionKeepAlive();
      return new Promise(function (resolve) {
        var taskQueue = new TaskQueue(50, function (results) {
          self.triggerKeepAlive = false;
          resolve(results)
        });
        files.forEach(function (item) {
          var edmHelper = new EDMHelper();
          taskQueue.pushTask(function (onDone) {
            var result = {}
            edmHelper.upload(item).then(function (guid) {
              result.res = {
                item: item,
                DocumentGuid: guid
              }
            }).catch(function () {
              result.err = item.name;
              return null;
            }).finally(function () {
              onDone(result.res, result.err);
            });
          }, function (instance) {
            var msg = 'Uploaded: '
            msg += instance.finished + '/' + numberOfFiles;
            $('.modalUploadFile .block-loading span').text(msg)
          });
        });
      })
    },
    handleCheckFileExist: function (artifacts, fileName) {
      var exist = _.find(artifacts, function (art) {
        return art.DisplayName.replace(/\.[^/.]+$/, "") == fileName.replace(/\.[^/.]+$/, "")
      });
      return exist ? true : false;
    },
    buildStreamArtifactUrl: function (item) {
      return new EdmService().getStreamFileUrl(item.DocumentGuid).then(function (url) {
        item.Url = url;
      });
    },
    handlePreviewItem: function (artifact) {
      var self = this;
      if (artifact) {
        var promises = [];
        ShowBlock($('.modalUploadFile'), 'Loading');
        if (!artifact.Url && artifact.DocumentGuid) {
          promises.push(this.buildStreamArtifactUrl(artifact))
        }
        Promise.all(promises).then(function () {
          var extension = artifact.Name.replace(/^.*\./, '').toLowerCase();
          artifact.FileType = Object.keys(prevewExtsSuport).find(function (key) { return prevewExtsSuport[key].includes(extension) }) || 'document';
          self.isShowPreview = true;
          self.itemPreview = artifact;
        }).finally(function () {
          $('.modalUploadFile').unblock();
        });
      }
    },
    downloadFile: function (url, fileName, isLink) {
      if (url) {
        if (isLink) {
          window.open(url, '_blank');
        } else {
          ShowBlock($('.modalUploadFile'), 'Loading');
          fetch(url, {
          }).then(function (res) { return res.blob() }).then(function (blob) {
            RecordRTC.invokeSaveAsDialog(blob, fileName, true);
          }).finally(function () {
            $('.modalUploadFile').unblock();
          });
        }        
      }
    },
    handleDownloadFile: function (artifact) {
      var promises = [];
      ShowBlock($('.modalUploadFile'), 'Loading');
      if (!artifact.Url && artifact.DocumentGuid) {
        promises.push(this.buildStreamArtifactUrl(artifact))
      }
      Promise.all(promises).then(function () {
        if (artifact.IsLink) {
          window.open(artifact.Url, '_blank');
        } else {
          fetch(artifact.Url, {
          }).then(function (res) { return res.blob() }).then(function (blob) {
            RecordRTC.invokeSaveAsDialog(blob, artifact.Name, true);
          })
        }
      }).finally(function () {
        $('.modalUploadFile').unblock();
      });
    },
    onClosePreview: function () {
      var self = this;
      self.itemPreview = null;
      self.isShowPreview = false;
    },
    resetErrorUploadFile: function () {
      var self = this;
      self.artifactFolderModalVM.fileUploads = [];
      self.artifactFolderModalVM.notAllowedFile = [];
      self.artifactFolderModalVM.duplicateFile = [];
      self.artifactFolderModalVM.exceedFileSize = [];
    },
    setStudentForArtifact: function (documentGuid, studentId) {
      var self = this;
      var item = self.artifactFolderModalVM.artifacts.find(function (item) {
        return item.DocumentGuid === documentGuid;
      })
      if (item)
        item.StudentID = +studentId;
    },
    handleGetStudentIdDefault(fileName, students) {
      if (!students || students.length === 0) return null;
      var normalize = function(str) {
        return str.replace(/[,#_-]/g, ' ')
          .replace(/\.[^.$]+$/g, '')
          .replace(/^\s+|\s+$|\s+(?=\s)/g, '')
          .toLowerCase();
      }

      var fileNamePriority = normalize(fileName);
      var fileNameNotExtension = fileName.replace(/\.[^.$]+$/g, '');
      var fileNameNotExtensionLower = fileNameNotExtension.toLowerCase();
      var fileNameWords = fileNamePriority.split(' ');
      var boundaryChars = ['-', ' ', '.', ',', '*', '_', '(', ')', '#', '@', '!', '{', '}', '[', ']', '|', '/', '~', '$'];

      function findUnique(filterFn) {
        return students.filter(filterFn);
      }

      function findIncludeContainCodeAndAltCode() {
        if (!students || students.length === 0) return [];

        return students.filter(s => {
          if (!s.Code && !s.AltCode) return false;

          var lowerCode = s.Code.toLowerCase();
          var lowerAltCode = (s.AltCode !== null && s.AltCode !== '') ? s.AltCode.toLowerCase() : '';
          var fileNameLower = fileNameNotExtensionLower;

          if (fileNameLower.indexOf(lowerCode) === -1 && fileNameLower.indexOf(lowerAltCode) === -1) return false;

          var parts = fileNameLower.split(lowerCode);

          if (parts.length <= 1) parts = fileNameLower.split(lowerAltCode);

          if (parts.length <= 1) return false;

          var left = parts[0].slice(-1) || ' ';
          var right = parts[1].charAt(0) || ' ';

          return boundaryChars.includes(left) && boundaryChars.includes(right);
        });
      }

      function findIncludeContainFirstAndLastName(firstName, lastName) {
        // Check if firstName or lastName contains the fileName
        if (firstName.includes(fileNameNotExtensionLower) || lastName.includes(fileNameNotExtensionLower)) return true;

        // Check if fileName contains neither firstName nor lastName, or vice versa
        if (
          (!fileNameNotExtensionLower.includes(firstName) && !fileNameNotExtensionLower.includes(lastName)) &&
          (!firstName.includes(fileNameNotExtensionLower) && !lastName.includes(fileNameNotExtensionLower))
        ) {
          return false;
        }

        var parts = fileNameNotExtensionLower.split(firstName);
        if (parts.length === 1) parts = fileNameNotExtensionLower.split(lastName);
        if (parts.length === 1) parts = firstName.split(fileNameNotExtensionLower);
        if (parts.length === 1) parts = lastName.split(fileNameNotExtensionLower);

        if (parts.length <= 1) return false;

        var left = parts[0].slice(-1) || ' ';
        var right = parts[1].charAt(0) || ' ';

        return boundaryChars.includes(left) && boundaryChars.includes(right);
      }

      // 1. Exact match by code and altCode
      var studentByCodeAndAltCode = findUnique(function (s) {
        return ((fileNameNotExtensionLower === s.Code.toLowerCase() || fileNamePriority === s.Code.toLowerCase())
          || (s.AltCode && (fileNameNotExtensionLower === s.AltCode.toLowerCase() || fileNamePriority == s.AltCode.toLowerCase())));
      });
      if (studentByCodeAndAltCode && studentByCodeAndAltCode.length == 1) return studentByCodeAndAltCode[0].StudentID;

      // 2. Contains code and altCode
      var studentByCodeAndAltCodeContain = findIncludeContainCodeAndAltCode();
      if (studentByCodeAndAltCodeContain && studentByCodeAndAltCodeContain.length === 1) return studentByCodeAndAltCodeContain[0].StudentID;      

      // 3. Orther
      var groups = {
        exactName: [],
        exactDisplay: [],
        exactFirstLast: [],
        containName: [],
        containDisplay: [],
        containFirstLast: [],
        similarName: []
      };

      for (var j = 0; j < students.length; j++) {
        var student = students[j];
        var displayName = student.DisplayFullName.replace(/\*/g, '')
          .replace(/[,]/g, ' ')
          .replace(/^\s+|\s+$|\s+(?=\s)/g, '')
          .toLowerCase();
        var splitNames = student.DisplayFullName.replace(/\*/g, '').toLowerCase().split(',');
        var firstName = splitNames[0] ? splitNames[0].trim() : '';
        var lastName = splitNames[1] ? splitNames[1].trim() : '';
        const reversedName = `${lastName.trim()} ${firstName.trim()}`;

        var matches = {
          exactPriority: fileNamePriority == displayName || fileNamePriority == reversedName,
          containPriority: fileNamePriority.indexOf(displayName) != -1 || fileNamePriority.indexOf(reversedName) != -1,
          exactDisplay: fileNameWords.join(' ') == displayName || fileNameWords.join(' ') == reversedName,
          containDisplay: fileNameWords.join(' ').indexOf(displayName) != -1 || fileNameWords.join(' ').indexOf(reversedName) != -1,
          exactFirstLast: fileNamePriority == firstName || fileNamePriority == lastName,
          containFirstLast: findIncludeContainFirstAndLastName(firstName, lastName)
        };

        if (matches.exactPriority) groups.exactName.push(student);
        if (matches.containPriority) groups.containName.push(student);
        if (matches.exactDisplay) groups.exactDisplay.push(student);
        if (matches.containDisplay) groups.containDisplay.push(student);
        if (matches.exactFirstLast) groups.exactFirstLast.push(student);
        if (matches.containFirstLast) groups.containFirstLast.push(student);

        if ((fileNamePriority !== displayName && fileNamePriority !== reversedName) && (fileNamePriority.indexOf(displayName) != -1 || displayName.indexOf(fileNamePriority) != -1
          || fileNamePriority.indexOf(reversedName) != -1 || reversedName.indexOf(fileNamePriority) != -1)) {
          if (groups.similarName && groups.similarName.length > 0) {
            var existOld = groups.similarName.find(item => {
              var nameParts = item.student.DisplayFullName.split(',').map(part => part.trim());
              if (nameParts.length > 1) {
                return (fileNameWords.includes(nameParts[0].toLowerCase()) || fileNameWords.includes(nameParts[1].toLowerCase()));
              } else {
                return fileNameWords.includes(nameParts[0].toLowerCase());
              }              
            });
            if (existOld == null) {
              groups.similarName.push({ student: student, matchLength: displayName.length });
            } else {
              groups.similarName = groups.similarName.filter(function (obj) { return obj !== existOld });
            }
          } else {
            groups.similarName.push({ student: student, matchLength: displayName.length });
          }
        }
      }

      function pickOne(arr) {
        return arr.length == 1 ? arr[0].StudentID : null;
      }

      if (pickOne(groups.exactName)) return pickOne(groups.exactName);
      if (pickOne(groups.exactDisplay)) return pickOne(groups.exactDisplay);
      if (pickOne(groups.exactFirstLast)) return pickOne(groups.exactFirstLast);
      if (pickOne(groups.containName)) return pickOne(groups.containName);
      if (pickOne(groups.containDisplay)) return pickOne(groups.containDisplay);
      if (pickOne(groups.containFirstLast)) return pickOne(groups.containFirstLast);

      if (groups.similarName.length > 0) {
        groups.similarName.sort(function (a, b) {
          return b.matchLength - a.matchLength;
        });
        return groups.similarName[0].student.StudentID;
      }

      return null;      
    },
    handleFormatWarningFileSize(arrFileSizeGroup) {
      var self = this;
      var html = '';
      if (arrFileSizeGroup != null && arrFileSizeGroup.length > 0) {
        html += '<span style="display: block; text-align: left !important">';
        arrFileSizeGroup.map(function (m) { return html += ('<span>' + m.DisplayName + ' - ' + self.onBytesToSize(m.MaxFileSizeInBytes) + ' file size limit.' + '&nbsp</span></br>'); })
        html += '</span>';
      }
      return html;
    },
    onBytesToSize(bytes) {
      var k = 1000;
      var sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB'];
      if(bytes === 0) {
        return '0 Bytes';
      }
      var i = parseInt(Math.floor(Math.log(bytes) / Math.log(k)), 10);
      return (bytes / Math.pow(k, i)).toPrecision(3) + ' ' + sizes[i];
    },
    handleUpdateExpressionOriginal(expression, expressionObjs) {
      var self = this;
      var exitsMaxMin = expression.indexOf('Math.min') != -1 || expression.indexOf('Math.max') != -1;
      if (exitsMaxMin) {
        return self.handleUpdatedExpressionForCalculationMinMax(expression, expressionObjs);
      } else {
        return self.handleUpdatedExpressionForCalculationNormal(expression, expressionObjs);
      }      
    },
    handleUpdatedExpressionForCalculationMinMax(expression, expressionObjs) {
      var self = this;
      var expressionMath = expression.replace(/Math\.min|Math\.max/g, '').replace(/[()\s]/g, '');
      var highLowest = expressionMath.split(',');
      var highLowestMath = [];
      if (highLowest && highLowest.length > 0) {
        for (var i = 0; i < highLowest.length; i++) {
          highLowestMath.push(self.handleUpdatedExpressionForCalculationNormal(highLowest[i], expressionObjs));
        }
      }
      var mathJoin = highLowestMath.join(',');
      if (expression.indexOf('Math.min') != -1) {
        return `Math.min(${mathJoin})`;
      } else if (expression.indexOf('Math.max') != -1) {
        return `Math.max(${mathJoin})`;
      }
    },
    handleUpdatedExpressionForCalculationNormal(expression, expressionObjs) {
      var self = this;
      var visited = [];
      function replaceExpression(expression) {
        visited = [];
        var columnExpression = self.handlGetKeyColumnByExpression(expression);
        if (!columnExpression || columnExpression.length === 0) return expression;
        var columnCompares = columnExpression.filter(function (col) {
          return expressionObjs.some(function (item) {
            return col == item.scoreId;
          });
        });
        columnCompares = self.removeDuplicates(columnCompares);
        if (columnCompares && columnCompares.length > 0) {
          for (let i = 0; i < columnCompares.length; i++) {
            var colId = columnCompares[i];
            if (visited.includes(colId)) { return ""; }
            var existCal = _.find(expressionObjs, function (item) {
              return item.scoreId === colId;
            });

            if (existCal && existCal.expression) {
              if (existCal.expression.indexOf('Math.min') != -1 || existCal.expression.indexOf('Math.max') != -1) {
                existCal.expression = self.handleUpdatedExpressionForCalculationMinMax(existCal.expression, expressionObjs);
              }
              expression = expression.replaceAll(colId, `(${existCal.expression})`);
            }
            visited.push(colId);
          }
          return replaceExpression(expression);
        }
        else {
          return expression;
        }
      }

      return replaceExpression(expression);
    },
    handlGetKeyColumnByExpression(expression) {
      var columns = [];
      if (expression != null && expression.length > 0) {
        var expressionSplit = expression.replace('calculation:', '').replace(/Math\.min|Math\.max/g, '').replace(/[()]/g, '').split(/[ ,\x\*\+\-\/]+/);
        var overallScores = expressionSplit.filter(function (item) { return item.startsWith('0&') })
          .map(function (m) { return m.replace(/[()\s]/g, '') });
        if (overallScores != null && overallScores.length > 0) {
          overallScores.forEach(function (item) {
            var scoreSplit = item.split('&');
            if (scoreSplit.length > 1) {
              columns.push(`${0}&${scoreSplit[1]}`);
            }
          });
        }
        var subScores = expressionSplit.filter(function (item) { return item.includes('&') && !item.startsWith('0&') })
          .map(function (m) { return m.replace(/[()\s]/g, '') });
        if (subScores != null && subScores.length > 0) {
          subScores.forEach(function (item) {
            var subScoreSplit = item.split('&');
            if (subScoreSplit.length > 1) {
              columns.push(`${subScoreSplit[0]}&${subScoreSplit[1]}`);
            }
          });
        }
      }
      return columns;
    },
    handlGetColumnByExpression(expression) {
      var self = this;
      var columns = [];
      if (expression != null && expression.length > 0) {
        var expressionSplit = expression.replace('calculation:', '').replace(/Math\.min|Math\.max/g, '').replace(/[()]/g, '').split(/[ ,\x\*\+\-\/]+/);
        var overallScores = expressionSplit.filter(function (item) { return item.startsWith('0&') }).map(function (m) { return m.replace(/[()\s]/g, '') });
        var subScores = expressionSplit.filter(function (item) { return item.includes('&') && !item.startsWith('0&') }).map(function (m) { return m.replace(/[()\s]/g, '') });
        if (overallScores != null && overallScores.length > 0) {
          overallScores.forEach(function (item) {
            var scoreSplit = item.split('&');
            if (scoreSplit.length > 1) {
              var propName = "overallScore.Score" + scoreSplit[1];
              columns.push({ column: propName, expression: item });
            }
          });
        }
        if (subScores != null && subScores.length > 0) {
          subScores.forEach(function (item) {
            var subScoreSplit = item.split('&');
            if (subScoreSplit.length > 1) {
              var subScoreId = parseFloat(subScoreSplit[0]);
              self.printResult.customSubScores.forEach(function (sub, i) {
                if (sub.VirtualTestCustomSubScoreId === subScoreId) {
                  var propName = "subScore_" + i + ".Score" + subScoreSplit[1];
                  columns.push({ column: propName, expression: item });
                }
              });
            }
          });
        }
      }
      return columns;
    },
    handleSetValueExpression(jsonData, customScore, customSubScores) {
      var self = this;
      Object.entries(jsonData).forEach(([key, value]) => {
        var jsonItem = value;
        if (key === 'overallScore' && customScore && customScore.ScoreInfos) {
          customScore.ScoreInfos.forEach(function (score) {
            if (value.hasOwnProperty(`Score${score.ScoreName}`)) {
              if (value[`Score${score.ScoreName}`] == null && score.MetaData && score.MetaData.IsAutoCalculation) {
                if (score.ScoreName == 'Percent') {
                  var calRaw = customScore.ScoreInfos.find(function (item) { return item.ScoreName == 'Raw' });
                  if (calRaw != null && calRaw.MetaData && calRaw.MetaData.IsAutoCalculation) {
                    value[`Score${score.ScoreName}`] = self.handleGetValueExpression(calRaw.MetaData.Expression);
                  }
                } else {
                  value[`Score${score.ScoreName}`] = self.handleGetValueExpression(score.MetaData.Expression);
                }
              }
            }
          })
        }
        if (key.startsWith('subScore_') && customSubScores && customSubScores.length > 0) {
          customSubScores.forEach(function (customSubScore) {
            if (customSubScore.Name == jsonItem.Name) {
              customSubScore.ScoreInfos.forEach(function (score) {
                if (value.hasOwnProperty(`Score${score.ScoreName}`)) {
                  if (value[`Score${score.ScoreName}`] == null && score.MetaData && score.MetaData.IsAutoCalculation) {
                    if (score.ScoreName == 'Percent') {
                      var calRaw = customSubScore.ScoreInfos.find(function (item) { return item.ScoreName == 'Raw' });
                      if (calRaw != null && calRaw.MetaData && calRaw.MetaData.IsAutoCalculation) {
                        value[`Score${score.ScoreName}`] = self.handleGetValueExpression(calRaw.MetaData.Expression);
                      }
                    } else {
                      value[`Score${score.ScoreName}`] = self.handleGetValueExpression(score.MetaData.Expression);
                    }                    
                  }
                }
              })
            }
          })
        }
      });
      return jsonData;
    },
    handleGetValueExpression(expression) {
      try {
        var calValue = Function(`'use strict'; return (${expression})`)();
        if (!isNaN(calValue)) {
          if (calValue === 'Infinity' || calValue === Infinity) {
            return "";
          } else {
            return calValue;
          }
        }
      } catch {
        return "";
      }
    }
  }
});
