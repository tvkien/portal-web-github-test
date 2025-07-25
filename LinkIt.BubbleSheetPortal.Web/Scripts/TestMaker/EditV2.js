var _qtiSchemaIds = {
  MULTIPART: 21
};

var TestMakerComponent = new Vue({
  el: '#divEditItem',
  data: {
    isShowModalKeepAlive: false,
    minutesForExpire: 0,
    sessionTimeoutWarning: global.sessionTimeoutWarning,
    sessionTimeout: global.sessionTimeout,
    sessionWarningTimer: null,
    sessionCountdown: null,
    sessiongMsg: '',
    sessionKeepAliveUrl: global.sessionKeepAliveUrl,
    isSessionExpire: false,
    sessionButtonStatus: false,
    keepAliveDistanceSecond: global.keepAliveDistanceSecond,
    startKeepAlive: '',
    nowKeepAlive: '',
    isBusy: false,
    basicSciencePaletteSymbol: global.basicSciencePaletteSymbol,
    isShowAlgorithmic: false,
    isShowExpression: false,
    isShowAlgorithmicConfiguration: false,
    listExpression: [],
    selectedExpression: null,
    typeChoice: 'single',
    typeTextEntry: 'text',
    isShowConditionalLogicButton: false,
    isShowPopupConditionalLogicMultipart: false,
    selectedMultiPartExpressionIndex: -1,
    isShowMultiPartExpression: false,
    listMultiPartExpression: [{
      expression: '*',
      enableElements: [],
      rules: []
    }],
    multiPartResponses: ["NEXT"],
    attachmentSetting: {
      allowStudentAttachment: false,
      requireAttachment: false,
      fileTypeGroupNames: [],
      recordingOptionKeys: []
    },
    qtiSchemaId: '1',
    assessmentArtifactConfiguration: {
      showSetting: false,
      assessmentArtifactFileTypeGroups: [],
      recordingOptions: []
    },
    isShowPopupRevertItem: false
  },
  ready: function () {
    var self = this;
    var minutesForExpire = self.sessionTimeout - self.sessionTimeoutWarning;
    self.sessionWarningTimer = setTimeout(function () {
      self.sessionWarning();
    }, minutesForExpire * 60 * 1000);

    self.startKeepAlive = TestMakerUtils.getTodayBySeconds();
    self.sessionKeepAliveTimer = setInterval(function () {
      self.nowKeepAlive = TestMakerUtils.getTodayBySeconds();
      var distance = self.nowKeepAlive - self.startKeepAlive;

      if (distance >= self.keepAliveDistanceSecond) {
        self.isBusy = true;
      }
    }, 1000);

    window.addEventListener('click', self.handlerSessionKeepAlive);
    window.addEventListener('keydown', self.handlerSessionKeepAlive);
    window.addEventListener('scroll', self.handlerSessionKeepAlive);

    sessionStorage.setItem('basicSciencePaletteSymbol', self.basicSciencePaletteSymbol);
  },
  computed: {
    minutesForExpireFormat: function () {
      var result = TestMakerUtils.minutesForExpireFormat(this.minutesForExpire);
      return result;
    }
  },
  watch: {
    listMultiPartExpression: function () {
      this.updateEnableElementTag(this.multiPartResponses);
    },
    multiPartResponses: function () {
      this.updateEnableElementTag(this.multiPartResponses);
    }
  },
  methods: {
    sessionWarning: function () {
      var self = this;
      self.isShowModalKeepAlive = true;

      self.minutesForExpire = this.sessionTimeoutWarning * 60;

      self.sessionCountdown = setInterval(function () {
        self.minutesForExpire -= 1;

        if (self.minutesForExpire < 0) {
          clearInterval(self.sessionCountdown);
          self.isShowModalKeepAlive = false;
          window.location = window.location.origin + '/Account/LogOff';
        }
      }, 1000)
    },
    sessionKeepAlive: function () {
      var self = this;

      self.sessionButtonStatus = true;

      $.get(self.sessionKeepAliveUrl, function (response) {
        var minutesForExpire = self.sessionTimeout - self.sessionTimeoutWarning;
        clearInterval(self.sessionCountdown);
        clearTimeout(self.sessionWarningTimer);
        self.sessionButtonStatus = false;
        self.isShowModalKeepAlive = false;
        self.isSessionExpire = false;
        self.startKeepAlive = TestMakerUtils.getTodayBySeconds();
        self.sessionWarningTimer = setTimeout(function () {
          self.sessionWarning();
        }, minutesForExpire * 60 * 1000)
      });
    },
    handlerSessionKeepAlive: function (e) {
      var self = this;

      if (self.isBusy) {
        if (self.isShowModalKeepAlive) {
          self.isShowModalKeepAlive = false;
        }

        self.startKeepAlive = TestMakerUtils.getTodayBySeconds();
        self.isBusy = false;
        self.sessionKeepAlive();
      }
    },
    openPopupAlgorithmic: function () {
      var self = this;
      var xmlContent = xmlExport();
      var qtiSchemaId = parseInt(iSchemeID, 10);

      if (!xmlContent) {
        return;
      }

      if (qtiSchemaId === 1 || qtiSchemaId === 3 || qtiSchemaId === 37) {
        var isMultiSelect = xmlContent.indexOf('cardinality="multiple"') != -1 ? 'multiple' : 'single';
        if (self.listExpression.length > 0 && self.listExpression.schemaSubType && self.listExpression.schemaSubType !== isMultiSelect) {
          self.confirmChangeTypeExpressionDialog("choice", isMultiSelect);
        } else {
          self.isShowAlgorithmic = true;
        }
      } else if (qtiSchemaId === 9) {
        var isRange = xmlContent.indexOf('range="true"') != -1 ? 'range' : 'text';
        if (self.listExpression.length > 0 && self.typeTextEntry !== isRange) {
          self.confirmChangeTypeExpressionDialog("entry", isRange);
        } else {
          self.isShowAlgorithmic = true;
        }
      } else {
        self.isShowAlgorithmic = true;
      }
      $('html').css('overflow', 'hidden');
      setTimeout(function () {
        $(".with-tip").tip()
      }, 100)
    },
    closePopupAlgorithmic: function () {
      for (var i = 0; i < this.listExpression.length; i++) {
        if (this.listExpression[i].point < 0 || this.listExpression[i].point == "") {
          this.listExpression[i].point = 0;
        }
      }
      var qtiSchemaId = parseInt(iSchemeID, 10);

      if (this.listExpression.length > 0 && (qtiSchemaId === 1 || qtiSchemaId === 3 || qtiSchemaId === 37)) {
        this.listExpression.schemaSubType = xmlExport().indexOf('cardinality="multiple"') != -1 ? 'multiple' : 'single';
      }

      this.isShowAlgorithmic = false;
      $('html').css('overflow', 'scroll');
    },
    savePopupExpression: function () {
      var self = this;
      var $expressionBuilder = $('#expression-builder');
      var rules = $expressionBuilder.queryBuilder('getRules');
      var qtiSchemaId = parseInt($('#hdQTISchemaID').val(), 10);

      if (!$.isEmptyObject(rules)) {
        var expression = TestMakerUtils.convertExpressionAlgorithmic(rules, qtiSchemaId);

        if (self.selectedExpression == null) {
          self.listExpression.push({
            qtiitemAlgorithmID: null,
            virtualQuestionAlgorithmID: null,
            expression: expression,
            point: 1,
            rules: rules
          });
        } else {
          self.listExpression[self.selectedExpression].expression = expression;
          self.listExpression[self.selectedExpression].rules = rules;
        }

        self.isShowExpression = false;
      }

      setTimeout(function () {
        $(".with-tip").tip()
      }, 100)
    },
    closePopupExpression: function () {
      this.isShowExpression = false;
    },
    addExpression: function () {
      var self = this;

      self.isShowExpression = true;
      self.selectedExpression = null;
      self.displayQueryBuilder();
    },
    deleteExpression: function (index) {
      this.listExpression.splice(index, 1);
    },
    editExpression: function (index) {
      var self = this;

      self.isShowExpression = true;
      self.selectedExpression = index;
      self.displayQueryBuilder();
    },
    displayQueryBuilder: function () {
      var self = this;
      var $expressionBuilder = $('#expression-builder');
      var xmlContent = xmlExport();
      var qtiSchemaId = parseInt(iSchemeID, 10);
      var options;

      if (qtiSchemaId === 1 || qtiSchemaId === 3) {
        options = MultipleChoiceAlgorithmic.getQueryBuilder(xmlContent);
      } else if (qtiSchemaId === 8) {
        options = InlineChoiceAlgorithmic.getQueryBuilder(xmlContent);
      } else if (qtiSchemaId === 9) {
        options = TextEntryAlgorithmic.getQueryBuilder(xmlContent);
      } else if (qtiSchemaId === 30) {
        options = DragDropAlgorithmic.getQueryBuilder(xmlContent);
      } else if (qtiSchemaId === 31) {
        options = TextHotSpotAlgorithmic.getQueryBuilder(xmlContent);
      } else if (qtiSchemaId === 32) {
        options = ImageHotSpotAlgorithmic.getQueryBuilder(xmlContent);
      } else if (qtiSchemaId === 33) {
        options = TableHotSpotAlgorithmic.getQueryBuilder(xmlContent);
      } else if (qtiSchemaId === 34) {
        options = NumberlineHotSpotAlgorithmic.getQueryBuilder(xmlContent);
      } else if (qtiSchemaId === 35) {
        options = DragDropNumericalAlgorithmic.getQueryBuilder(xmlContent);
      } else if (qtiSchemaId === 36) {
        options = DragDropSequenceOrderAlgorithmic.getQueryBuilder(xmlContent);
      } else if (qtiSchemaId === 37) {
        options = MultipleChoiceVariableAlgorithmic.getQueryBuilder(xmlContent);
      }

      $expressionBuilder.queryBuilder('destroy');
      $expressionBuilder.queryBuilder(options);

      if (self.selectedExpression != null && !!self.listExpression[self.selectedExpression].rules) {
        $expressionBuilder.queryBuilder('setRules', self.listExpression[self.selectedExpression].rules);
      }
    },
    confirmChangeTypeExpressionDialog: function (type, typeDetail) {
      var self = this;
      var stringHtml = '<p>Expressions will be lost in converting to a different item type. Do you want to continue?</p>';

      return $('<div title="Be Advised"></div>').append(stringHtml)
        .dialog({
          resizable: true,
          modal: true,
          dialogClass: 'customDialogStyle no-close',
          open: function () {
            $('.ui-dialog-titlebar').addClass('cke_dialog_title');
          },
          buttons: {
            "OK": function () {
              $(this).dialog('destroy');
              self.listExpression = [];
              self.isShowAlgorithmic = true;
              if (type === "choice") {
                self.typeChoice = typeDetail;
              } else if (type === "entry") {
                self.typeTextEntry = typeDetail;
              }
            },
            "Cancel": function () {
              $(this).dialog('destroy');
            }
          }
        });
    },
    openPopupConditionalLogicMultipart: function () {
      var self = this;
      var qtiSchemaId = parseInt(iSchemeID, 10);

      if (qtiSchemaId === _qtiSchemaIds.MULTIPART) {
        self.isShowPopupConditionalLogicMultipart = true;
      }
      $('html').css('overflow', 'hidden');
    },
    okPopupMultiPartConfig: function () {
      var self = this;
      var index = 0;

      for (var i = 0; i < self.listMultiPartExpression.length; i++) {
        index = i + 1;
        var enableElements = $('#enableElement' + index).tagit('assignedTags');
        if (enableElements.length > 0) {
          self.listMultiPartExpression[i].enableElements = enableElements;
        }
        else {
          customAlert("There are expressions were not set up any Enabled Elements. Please select enabled elements to complete the setting.", { ZIndex: 10000 });
          return;
        }
      }
      self.isShowPopupConditionalLogicMultipart = false;
      $('html').css('overflow', 'scroll');
    },
    closePopupMultiPartConfig: function () {
      var self = this;
      self.isShowPopupConditionalLogicMultipart = false;
      $('html').css('overflow', 'scroll');
    },
    addMultiPartExpression: function () {
      var self = this;
      self.selectedMultiPartExpressionIndex = -1;
      self.isShowMultiPartExpression = true;
      self.displayMultiPartQueryBuilder();
    },
    displayMultiPartQueryBuilder: function () {
      var self = this;
      var $expressionBuilder = $('#multi-part-expression-builder');
      var xmlContent = '';

      // Silent XML export warning
      window.exportXmlWarningDisable = true;
      try {
        xmlContent = xmlExport();
      } catch (error) {
        console.error(error);
      } finally {
        delete window.exportXmlWarningDisable;
      }

      var qtiSchemaId = parseInt(iSchemeID, 10);
      var options;

      if (qtiSchemaId === _qtiSchemaIds.MULTIPART) {
        options = MultiPart.getQueryBuilder(xmlContent);
        var response = '';
        options.filters.sort(function (a, b) {
          if (a.id < b.id) {
            return -1;
          } else if (a.id > b.id) {
            return 1;
          } else {
            return 0;
          }
        });
        for (var i = 0; i < options.filters.length; i++) {
          response = options.filters[i].id;
          if ($.inArray(response, self.multiPartResponses) < 0) {
            self.multiPartResponses.push(response);
          }
        }
        self.multiPartResponses.sort();
        options.allow_groups = false;
      }

      $expressionBuilder.queryBuilder('destroy');
      if (options.filters.length === 0) {
        customAlert("There are expressions were not set up any Enabled Elements. Please select enabled elements to complete the setting.", { ZIndex: 10000 });
        self.isShowMultiPartExpression = false;
        return;
      }
      $expressionBuilder.queryBuilder(options);

      if (self.selectedMultiPartExpressionIndex > -1) {
        $expressionBuilder.queryBuilder('setRules', self.listMultiPartExpression[self.selectedMultiPartExpressionIndex].rules);
      }
    },
    saveMultiPartExpression: function () {
      var self = this;
      var $expressionBuilder = $('#multi-part-expression-builder');
      var rules = $expressionBuilder.queryBuilder('getRules');
      var qtiSchemaId = parseInt(iSchemeID, 10);

      if (!$.isEmptyObject(rules)) {
        var expression = TestMakerUtils.convertMultiPartExpression(rules, qtiSchemaId);

        if (self.selectedMultiPartExpressionIndex === -1) {
          self.listMultiPartExpression.push({
            qtiitemAlgorithmID: null,
            virtualQuestionAlgorithmID: null,
            expression: expression,
            enableElements: [],
            rules: rules
          });
        } else {
          self.listMultiPartExpression[self.selectedMultiPartExpressionIndex].expression = expression;
          self.listMultiPartExpression[self.selectedMultiPartExpressionIndex].rules = rules;
        }

        self.isShowMultiPartExpression = false;
      }
    },
    closeMultiPartExpression: function () {
      this.isShowMultiPartExpression = false;
    },
    deleteMultiPartExpression: function (index) {
      if (this.listMultiPartExpression[index]) {
        this.listMultiPartExpression.splice(index, 1);
      }
    },
    editMultiPartExpression: function (index) {
      var self = this;
      self.isShowMultiPartExpression = true;
      self.selectedMultiPartExpressionIndex = index;
      self.displayMultiPartQueryBuilder();
    },
    updateEnableElementTag: function (multiPartResponses) {
      $(".enableElements").tagit({
        availableTags: multiPartResponses,
        autocomplete: {
          delay: 0,
          minLength: 0
        }
      });
      $('.enableElements').find('.ui-autocomplete-input').attr('readonly', true);
      $('ul.tagit input[type="text"]').css("min-width", "10px");
    },
    openPopupRevertItem: function () {
      this.isShowPopupRevertItem = true
    },
    revertItemVueInstantClick: function (item) {
      revertItemClick(item)
    },
  }
});

