var attachmentDialogVM = new Vue({
  template: document.getElementById('attachment-dialog').innerHTML,
  components: {
    TabAddAttachments: tabAddAttachments,
    TabRecordAudio: tabRecordAudio,
    TabCaptureImage: tabCaptureImage,
    TabCaptureVideo: tabCaptureVideo,
    AttacmentList: attacmentList,
    PreviewItemDialog: previewItemDialog
  },
  computed: {
    allowTabs: function () {
      var tabs = [];

      if (this.config.allowFile) {
        tabs.push('TabAddAttachments');
      }
      if (this.config.allowAudio) {
        tabs.push('TabRecordAudio');
      }
      if (this.config.allowImage) {
        tabs.push('TabCaptureImage');
      }
      if (this.config.allowVideo) {
        tabs.push('TabCaptureVideo');
      }
      return tabs;
    },
    tabInstance: function () {
      return this.allowTabs[this.activeTab] || 'div';
    },
    config: {
      get: function () {
        return this.shared.config;
      },
      set: function (config) {
        var sharedConfig = Object.assign({}, this.shared.config, config);
        this.$set(this.shared, 'config', sharedConfig);
      }
    },
    isFileLimitReached: function () {
      var self = this;
      var usersFile = self.attachmentsList.filter(function (item) { return item.CreatedBy == globalConfig.CurrentUserId && !item.IsDeleted });

      if (usersFile.length >= this.config.maxFileNumber && !this.shared.isRecording) {
        return true;
      }
      return false;
    },
    fileNumberStudent: function () {
      var self = this;
      var usersFile = self.attachmentsList.filter(function (item) { return item.CreatedBy == globalConfig.CurrentUserId && !item.IsDeleted });
      return usersFile.length;
    }
  },
  data: function () {
    return {
      shared: {
        isRecording: false,
        isUploading: false,
        config: {
          allowAudio: false,
          allowVideo: false,
          allowImage: false,
          allowFile: false,
          allowModification: false,
          maxFileNumber: 1,
          suportFileTypes: []
        },
      },
      showDialog: false,
      showPreview: false,
      showConfirm: false,
      width: 600,
      activeTab: 0,
      tabsLabel: {
        TabAddAttachments: 'Add Attachments',
        TabRecordAudio: 'Record Audio',
        TabCaptureImage: 'Capture Image',
        TabCaptureVideo: 'Record Video'
      },
      scoreID: 0,
      subScoreID: 0,
      attachmentsList: [],
      itemPreview: null,
      listEventsBinding: {
        onRemoveItem: this.handleRemoveItem,
        onItemClick: this.handleItemClick,
        onPreviewItem: this.handlePreviewItem,
      },
      inputEventsBinding: {
        onAddItems: this.handleAddItems,
        onAutoSave: this.handleAutoSave
      },
      confirmMessage: 'Are you sure you want to submit your files?'
    }
  },
  methods: {
    handleUpdateShowDialog: function (value) {
      this.showDialog = value;
    },
    handleAddItems: function (items) {
      var self = this;
      if (typeof items === 'string') {
        self.attachmentsList.push({
          Name: items,
          CreatedBy: globalConfig.CurrentUserId,
          IsUrl: true,
          IsNew: true
        })
        return;
      }
      if (items && items.length) {
        var usersFile = self.attachmentsList.filter(function (item) { return item.CreatedBy == globalConfig.CurrentUserId && !item.IsDeleted });
        var maxFileNumber = this.shared.config.maxFileNumber;
        var isVideoAutoSave = items.length == 1 && this.attachmentsList.some(function (item) { return items[0].documentGuid == item.DocumentGuid && !item.IsLink });
        if ((items.length + usersFile.length) > maxFileNumber && !isVideoAutoSave) {
          alert('Maximum of ' + maxFileNumber + ' ' + (maxFileNumber > 1 ? 'files are' : 'file is') + ' allowed')
          return;
        }
        var addItems = items.map(function (item) {
          var file = item.file || item;
          return {
            Name: file.name,
            File: file,
            CreatedBy: globalConfig.CurrentUserId,
            errorMessage: !item.isCapture ? self.fileValidate(file) : '',
            DocumentGuid: item.documentGuid
          }
        });
        if (isVideoAutoSave) {
          var attacment = self.attachmentsList.find(function (item) { return item.DocumentGuid == addItems[0].DocumentGuid });
          self.$set(attacment, 'errorMessage', addItems[0].errorMessage);
          self.$set(attacment, 'File', addItems[0].File);
        } else {
          this.attachmentsList = this.attachmentsList.concat(addItems);
        }
      }
    },
    handleRemoveItem: function (payload) {
      if (!payload.item.File || payload.item.Url) {
        this.$set(this.attachmentsList[payload.index], 'IsDeleted', true);
      } else {
        this.attachmentsList.splice(payload.index, 1);
      }
    },
    handleItemClick: function (item) {
      var self = this;
      var currentItem = self.attachmentsList.find(function (x) { return x.TestResultScoreUploadFileID == item.TestResultScoreUploadFileID });
      if (currentItem.StreamUrl) {
        self.downloadFile(currentItem.StreamUrl, currentItem.Name);
      } else if (currentItem.IsLink) {
        var link = currentItem.Url;
        if (link && !link.includes('://')) {
          link = 'https://' + link;
        }
        window.open(link, '_blank');
      } else {
        self.showOverlay();
        self.getUrl(currentItem).then(function (url) {
          self.$set(currentItem, 'StreamUrl', url);
          self.downloadFile(url, currentItem.Name);
        }).finally(function () {
          self.hideOverlay();
        });
      }
    },
    handlePreviewItem: function (item) {
      var self = this;
      if (item.File) {
        self.showPreview = true;
        self.itemPreview = item;
        return;
      }
      var currentItem = self.attachmentsList.find(function (x) { return x.TestResultScoreUploadFileID == item.TestResultScoreUploadFileID });
      if (currentItem.StreamUrl) {
        this.showPreview = true;
        this.itemPreview = currentItem;
      } else {
        self.showOverlay();
        self.getUrl(currentItem).then(function (url) {
          self.$set(currentItem, 'StreamUrl', url);
          self.showPreview = true;
          self.itemPreview = currentItem;
        }).finally(function () {
          self.hideOverlay();
        });
      }
      
    },
    handleSubmit: function () {
      var hasChanged = this.attachmentsList.some(function (item) {
        if ((!!item.File && !item.errorMessage) || item.IsDeleted || item.IsNew) return true;
        return false;
      });
      if (hasChanged) {
        if (this.config.allowModification) {
          var self = this;
          self.handleConfirm();
        }
        else {
          this.showConfirm = true;
        }        
      }
    },
    fileValidate: function (file) {
      var ext = file.name.split('.').pop();
      ext = ext.toLowerCase();
      if (!this.config.suportFileTypes.includes('.' + ext)) {
        return 'File type is not allowed.';
      }
      var type = globalConfig.FileTypeGroups.find(function (i) {
        return i.SupportFileType.includes('.' + ext)
      });
      if (type.MaxFileSizeInBytes < file.size) {
        return 'File size exceeds limit of ' + RecordRTC.bytesToSize(type.MaxFileSizeInBytes) + '.'
      }
      return null;
    },
    handleConfirm: function () {
      var self = this;
      var files = this.attachmentsList.filter(function (item) { return !!item.File && !item.DocumentGuid && !item.errorMessage });
      var deletedItems = this.attachmentsList.filter(function (item) { return !!item.IsDeleted });
      self.showOverlay('Uploading');
      self.uploadFiles(files).then(function (results) {
        var addedItems = results.map(function (item) {
          return {
            TestResultScoreUploadFileID: 0,
            DocumentGuid: item.DocumentGuid,
            FileName: item.item.Name
          }
        });
        var uploadedItems = self.attachmentsList.filter(function (item) { return (!!item.File && item.DocumentGuid && !item.errorMessage) || (item.IsNew && !item.IsDeleted) })
        addedItems = addedItems.concat(uploadedItems.map(
          function (item) {
            return {
              TestResultScoreUploadFileID: 0,
              DocumentGuid: item.DocumentGuid,
              FileName: item.file ? item.file.name : null,
              Name: item.IsUrl ? item.Name : null,
              IsUrl: item.IsUrl
            }
          }
        ));
        var payload = {
          TestResultScoreID: self.scoreID,
          TestResultSubScoreID: self.subScoreID,
          DeletedItems: deletedItems.map(function (item) {
            return {
              TestResultScoreUploadFileID: item.TestResultScoreUploadFileID,
              DocumentGuid: item.DocumentGuid,
            }
          }),
          AddedItems: addedItems
        };
        self.saveStudentAttachments(payload).then(function (data) {
          self.attachmentsList = data;
          ApplyFilter();
          self.showDialog = false;
        });
      }).finally(function () {
        self.hideOverlay();
      });
    },
    uploadFiles: function (files) {
      var promises = [];
      files.forEach(function (item) {
        var edmHelper = new EDMHelper();
        promises.push((function () {
          return edmHelper.upload(item.File).then(function (guid) {
            return {
              item: item,
              DocumentGuid: guid
            }
          });
        })());
      });
      return Promise.all(promises);
    },
    saveStudentAttachments: function (payload) {
      return new Promise(function (resolve, reject) {
        $.ajax({
          url: '/DataLockerForStudent/SaveStudentAttachments',
          type: 'POST',
          contentType: "application/json",
          data: JSON.stringify(payload),
          success: resolve,
          error: reject
        });
      });
    },
    getAttachmentUrl: function (fileName) {
      return new Promise(function (resolve, reject) {
        $.ajax({
          url: '/DataLockerForStudent/GetAttachmentUrl?fileName=' + fileName,
          type: 'GET',
          contentType: "application/json",
          success: function (result) { resolve(result.data); },
          error: reject
        });
      });
    },
    getUrl: function (item) {
      var self = this;
      return new Promise(function (resolve, reject) {
        if (!item || !item.DocumentGuid) {
          self.getAttachmentUrl(item.Name).then(resolve);
          return;
        }
        if (item && item.DocumentGuid) {
          new EdmService().getStreamFileUrl(item.DocumentGuid).then(resolve);
          return;
        }
        reject();
      });
    },
    showOverlay: function (message) {
      message = message || 'Loading';
      ShowBlock($(this.$refs.dialog.$el), message);
    },
    hideOverlay: function () {
      $(this.$refs.dialog.$el).unblock();
    },
    downloadFile: function (url, fileName) {
      var self = this;
      if (url) {
        self.showOverlay('Downloading');
        fetch(url).then(function (res) { return res.blob() }).then(function (blob) {
          RecordRTC.invokeSaveAsDialog(blob, fileName, true);
        }).finally(function () {
          self.hideOverlay();
        });
      }
    },
    handleCancelDialog: function () {
      var self = this;
      self.attachmentsList = self.attachmentsList.map(function (m) {
        return delete m['IsDeleted']
      })
      self.showDialog = false;
    },
    handleAutoSave: function (options) {
      var self = this;
      var deletedItems = this.attachmentsList.filter(function (item) { return !!item.IsDeleted });

      var payload = {
        TestResultScoreID: self.scoreID,
        TestResultSubScoreID: self.subScoreID,
        DeletedItems: deletedItems.map(function (item) {
          return {
            TestResultScoreUploadFileID: item.TestResultScoreUploadFileID,
            DocumentGuid: item.DocumentGuid,
          }
        }),
        AddedItems: [{
          TestResultScoreUploadFileID: 0,
          DocumentGuid: options.DocumentGuid,
          FileName: options.FileName
        }]
      };
      self.saveStudentAttachments(payload).then(function (data) {
        self.attachmentsList = data;
        ApplyFilter();
      });
    }
  },
  watch: {
    showPreview: function (val) {
      if (!val) {
        this.itemPreview = null;
      }
    },
    showDialog(val) {
      if (!val) {
        Object.assign(this.$data, this.$options.data.apply(this));
      }
    }
  }
});

document.body.appendChild(attachmentDialogVM.$mount().$el);
