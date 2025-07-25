var AttachmentSettingComponent = Vue.extend({
  template: `
  <div v-if="allowShowAttachmentSetting">
   <label class="reset-mb">Attachment Options</label>
   <!--begin allow student attachment and Recording Options-->
   <div class="d-flex label-center mt-2 first-row">
      <div class="w-30">
         <div class="center-checkbox">
            <input type="checkbox" v-model="attachmentSetting.allowStudentAttachment"/> Allow Student Attachment
            <span style="display: inline-block !important" class="icon icon-info icon-16 with-tip" title="{{warningFileSize}}">
         </div>
      </div>
      <div class="w-70" v-if="enableAttachmentDetailConfiguration">
         <div class="d-flex label-center">
            <div class="w-30">
               <label class="reset-mb my-auto">Recording Options: </label>
            </div>
            <div class="w-70">
               <select id="recording-options-select" multiple="multiple"  style="width: 100%">
               </select>
            </div>
         </div>
      </div>
   </div>
   <!--end allow student attachment and Recording Options-->
   <!-- begin required attachment and attachment types-->
   <div class="d-flex mt-8 label-center" v-if="enableAttachmentDetailConfiguration">
      <div class="w-30">
         <div v-if="showRequireAttachment" class="center-checkbox"><input type="checkbox" v-model="attachmentSetting.requireAttachment" /> Require Attachment</div>
      </div>
      <div class="w-70">
         <div class="d-flex label-center">
            <div class="w-30">
               <label class="reset-mb my-auto">Attachment Types: </label>
            </div>
            <div class="w-70">
               <select id="attachment-types-select" multiple="multiple" style="width: 100%">
               </select>
            </div>
         </div>
      </div>
   </div>
   <!-- end required attachment and attachment types-->
</div>
  `,
  props: {
    attachmentSetting: {
      type: Object,
      default: {
        allowStudentAttachment: false,
        requireAttachment: false,
        recordingOptionKeys: [],
        fileTypeGroupNames: [],
      }
    },
    qtiSchemaId: {
      type: String,
      default: '1'
    },
    assessmentArtifactConfiguration: {
      type: Object,
      default: {
        showSetting: false,
        assessmentArtifactFileTypeGroups: [],
        recordingOptions: []
      }
    }
  },
  data: function () {
    return {
      firstLoadAttachmentSetting: false,
      assessmentArtifactFileTypeGroups: [],
      recordingOptions: [],
      attachmentTypes: [],
      attachmentTypeDOMKey: '#attachment-types-select',
      recordingOptionDOMKey: '#recording-options-select',
      isFirstOpenAttachmentTypeDropdown: false,
      warningFileSize: '',
    };
  },
  computed: {
    enableAttachmentDetailConfiguration: function() {
      return this.allowEnableAttachmentConfigurationDetail();
    },
    showRequireAttachment: function() {
      var enabled = this.allowEnableAttachmentConfigurationDetail();
      enabled = enabled && this.schemaAllowRequireAttachment();
      return enabled;
    },
    allowShowAttachmentSetting: function() {
      return this.assessmentArtifactConfiguration.showSetting && this.isSchemaAllowedToShowAttachmentSetting();
    },
    allowStudentAttachment: {
      get: function () {
        return this.attachmentSetting.allowStudentAttachment;
      },
      set: function (newValue) {
        this.attachmentSetting.allowStudentAttachment = newValue;
      }
    }
  },
  watch: {
    allowStudentAttachment: function (nextValue) {

      if (!this.firstLoadAttachmentSetting) {
        this.firstLoadAttachmentSetting = true;

        if (!nextValue) {
          return;
        }

        this.initialAttachmentTypesDropdown();
        var $attachmentTypesSelect = $(this.attachmentTypeDOMKey);
        $attachmentTypesSelect.val(this.attachmentSetting.fileTypeGroupNames);
        $attachmentTypesSelect.trigger('change');

        this.initialRecodingOptionsDropdown();
        var $recordingOptionsSelect = $(this.recordingOptionDOMKey);
        $recordingOptionsSelect.val(this.attachmentSetting.recordingOptionKeys);
        $recordingOptionsSelect.trigger('change');

        return;
      }

      if (nextValue) {
        this.initialRecodingOptionsDropdown();
        this.initialAttachmentTypesDropdown();
        this.chooseAllFileTypeGroups();
      } else {
        this.resetAttachmentSetting();
      }
    },
    assessmentArtifactConfiguration: function (nextValue) {
      this.loadAssessmentArtifactFileTypeGroups();
      this.loadRecordingOptions();
      this.loadWarningFileSize();
      $(".with-tip").tip();
    }
  },
  ready: function () {
    var self = this;

    setTimeout(function() {
      self.firstLoadAttachmentSetting = true;

      self.initialRecodingOptionsDropdown();
      self.initialAttachmentTypesDropdown();

    }, 3000);
  },
  methods: {
    allowEnableAttachmentConfigurationDetail: function() {
      return this.attachmentSetting.allowStudentAttachment;
    },
    schemaAllowRequireAttachment: function() {
      // 10 - Constructed response
      // 10d - Draw response
      var qtiSchemaAllowRequireAttachmentOption = ['10', '10d'];
      var qtiSchemaId = this.qtiSchemaId;
      return qtiSchemaAllowRequireAttachmentOption.some(function(schemaId) {
        return schemaId === qtiSchemaId;
      });
    },
    showExtensionTooltip: function(category, top, left) {
      var extensions = this.assessmentArtifactFileTypeGroups.find(function(fileTypeGroup) {
        return fileTypeGroup.name == category;
      }).supportFileType.map(function(extension) {return `*${extension}`;}).join(', ');
      this.showTooltip(extensions, top, left);
    },
    showTooltip: function(message, top, left) {
      var $tooltips = $('.tool-tip-tips');
      $tooltips.css({
        'top': top,
        'left': left,
        'opacity': 1,
        'display': 'block'
      });
      $tooltips.html(message);
    },
    hideTooltip: function() {
      $('.tool-tip-tips').css({
        'top': '0px',
        'opacity': 0,
        'display': 'none'
      });
    },
    loadAssessmentArtifactFileTypeGroups: function() {
      this.assessmentArtifactFileTypeGroups = this.assessmentArtifactConfiguration.assessmentArtifactFileTypeGroups.map(function(f) {
        return { displayName: f.displayName, name: f.name.toLowerCase(), supportFileType: f.supportFileType }
      }).filter(function(f) { return f.supportFileType && f.supportFileType.length > 0; });
    },
    loadRecordingOptions: function() {
      this.recordingOptions = this.assessmentArtifactConfiguration.recordingOptions;
    },
    loadWarningFileSize: function () {
      this.warningFileSize = this.handleFormatWarningFileSize(this.assessmentArtifactConfiguration.assessmentArtifactFileTypeGroups);
    },
    chooseAllFileTypeGroups: function() {
      var allValues = this.assessmentArtifactFileTypeGroups.map(f => f.name);
      this.attachmentSetting.fileTypeGroupNames = allValues;
      this.setAllFileTypeGroupValues(allValues);
      var $attachmentTypesSelect = $(this.attachmentTypeDOMKey);
      $attachmentTypesSelect.trigger('change');
    },
    setAllFileTypeGroupValues: function(nextValue) {
      var $attachmentTypesSelect = $(this.attachmentTypeDOMKey);
      $attachmentTypesSelect.val(nextValue);
    },
    resetAttachmentSetting: function() {
      this.attachmentSetting.requireAttachment = false;
      this.attachmentSetting.recordingOptionKeys = [];
      this.attachmentSetting.fileTypeGroupNames = [];

      this.setAllFileTypeGroupValues([]);
      var $attachmentTypesSelect = $(this.attachmentTypeDOMKey);
      $attachmentTypesSelect.trigger('change');

      var $recordingOptionsSelect = $(this.recordingOptionDOMKey);
      $recordingOptionsSelect.val([]);
      $recordingOptionsSelect.trigger('change');
    },
    isSchemaAllowedToShowAttachmentSetting: function() {
      var self = this;
      var qtiSchemasNotAllowShowAttachmentSetting = ['21'];
      return !qtiSchemasNotAllowShowAttachmentSetting.some(function(schemaId) {
        return schemaId === self.qtiSchemaId;
      });
    },
    initialRecodingOptionsDropdown: function() {
      if ($(this.recordingOptionDOMKey).hasClass("select2-hidden-accessible")) {
        return;
      }

      var options = this.recordingOptions.map(o => {
        return { id: o.name, text: o.displayName }
      });

      var self = this;
      var $recordingOptionsSelect = $(self.recordingOptionDOMKey);
      $recordingOptionsSelect.select2({
        data: options,
        closeOnSelect: false,
        multiple: true,
        width: 'resolve',
        // Make selection-box similar to single select
        selectionAdapter: $.fn.select2.amd.require("CustomSelectionAdapter"),
        templateSelection: self.selectedTemplate
      });

      $recordingOptionsSelect.on("select2:select", function (e) {
        var recordingOptionKey = e.params.data.id;

        var exist = self.attachmentSetting.recordingOptionKeys.some(function(name) {
          return name == recordingOptionKey;
        });
        if (!exist) {
          self.attachmentSetting.recordingOptionKeys.push(recordingOptionKey);
        }
      });

      $recordingOptionsSelect.on("select2:unselect", function (e) {
        var recordingOptionKey = e.params.data.id;
        var index = self.attachmentSetting.recordingOptionKeys.indexOf(recordingOptionKey);
        if (index != -1){
          self.attachmentSetting.recordingOptionKeys.splice(index, 1);
        }
      });

    },
    initialAttachmentTypesDropdown: function() {
      if ($(this.attachmentTypeDOMKey).hasClass("select2-hidden-accessible")) {
        return;
      }

      var options = this.assessmentArtifactConfiguration.assessmentArtifactFileTypeGroups.map(f => {
        return { id: f.name.toLowerCase(), text: f.displayName }
      });

      var self = this;
      var $attachmentTypesSelect = $(self.attachmentTypeDOMKey);
      $attachmentTypesSelect.select2({
        data: options,
        closeOnSelect: false,
        multiple: true,
        width: 'resolve',
        templateResult: self.formatAttachmentType,
        // Make selection-box similar to single select
        selectionAdapter: $.fn.select2.amd.require("CustomSelectionAdapter"),
        templateSelection: self.selectedTemplate
      });

      $attachmentTypesSelect.on("select2:select", function (e) {
        var fileTypeGroupName = e.params.data.id;
        var exist = self.attachmentSetting.fileTypeGroupNames.some(function(name ) {
          return name.toLowerCase() == fileTypeGroupName.toLowerCase();
        });
        if (!exist) {
          self.attachmentSetting.fileTypeGroupNames.push(fileTypeGroupName);
        }
      });

      $attachmentTypesSelect.on("select2:unselect", function (e) {
        var fileTypeGroupName = e.params.data.id;
        var index = self.attachmentSetting.fileTypeGroupNames.indexOf(fileTypeGroupName);
        if (index != -1) {
          self.attachmentSetting.fileTypeGroupNames.splice(index, 1);
        }
      });

      $attachmentTypesSelect.on("select2:opening", function (e) {
        self.isFirstOpenAttachmentTypeDropdown = true;
      });

      $(document).on('mouseover', '#select2-attachment-types-select-results .select2-results__option', function ($event) {
        // used to force the hover when first open dropdown
        if (self.isFirstOpenAttachmentTypeDropdown) {
          setTimeout(function() {
            self.isFirstOpenAttachmentTypeDropdown = false;
          }, 100);
          return;
        }
        var displayTag = $event.target.querySelector('span');
        if (!displayTag) {
          return;
        }

        var fileTypeGroupName = displayTag.dataset.fileTypeGroupName;
        var tooltipPosition = self.calculateTooltipPosition($event);

        self.showExtensionTooltip(fileTypeGroupName, tooltipPosition.top, tooltipPosition.left);
      });

      $(document).on('mouseleave', '#select2-attachment-types-select-results .select2-results__option', function ($event) {
        self.hideTooltip();
      });

      $attachmentTypesSelect.on("select2:closing", function (e) {
        self.hideTooltip();
      });
    },
    formatAttachmentType: function (item) {

      if (!item.id) { return item.text; }

      var originalText = item.text;
      var fileTypeGroupName = item.id;
      var html = "<span data-file-type-group-name ='" + fileTypeGroupName + "'>" + originalText + "</span>";

      return $(html);
    },
    selectedTemplate: function (data) {
      var display = "";
      var allTexts = data.selected.map(x => x.text);
      if (allTexts.length > 1) {
        var othersCount = allTexts.length - 1;
        display = allTexts[0] + ` (+${othersCount} others)`;
      } else {
        display = allTexts[0];
      }
      return display;
    },
    calculateTooltipPosition: function ($event) {
      var displayTag = $event.target.querySelector('span');
      var displayTagPosition = displayTag.getBoundingClientRect();
      var top = displayTagPosition.y + window.scrollY;

      var parentClientRect = $event.target.parentElement.getBoundingClientRect();
      var left = displayTagPosition.x + parentClientRect.width - 25;

      return {'top': top, 'left': left};
    },
    handleFormatWarningFileSize(arrFileSizeGroup) {
      var self = this;
      var html = '';
      if (arrFileSizeGroup != null && arrFileSizeGroup.length > 0) {
        html += '<span style="display: block; text-align: left">';
        arrFileSizeGroup.map(function (m) { return html += ('<span style="font-size: 0.875rem; font-weight: 400; font-style: normal; ">' + m.displayName + ' - ' + self.onBytesToSize(m.maxFileSizeInBytes) + ' file size limit.' + '&nbsp</span></br>'); })
        html += '</span>';
      }
      return html;
    },
    onBytesToSize(bytes) {
      var k = 1000;
      var sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB'];
      if (bytes === 0) {
        return '0 Bytes';
      }
      var i = parseInt(Math.floor(Math.log(bytes) / Math.log(k)), 10);
      return (bytes / Math.pow(k, i)).toPrecision(3) + ' ' + sizes[i];
    }
  }
});

Vue.component('attachment-setting-component', AttachmentSettingComponent)
