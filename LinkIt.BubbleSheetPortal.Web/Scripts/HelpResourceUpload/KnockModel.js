
function Category(data) {
    var self = this;
    self.Index = data.Index;
    self.ID = data.ID;
    self.DisplayText = data.DisplayText;
    self.Checked = ko.observable(true);
}

function FileType(data) {
    var self = this;
    self.ID = data.ID;
    self.DisplayText = data.DisplayText;
}

function HelpResourceType(data) {
    var self = this;
    self.ID = data.ID;
    self.DisplayText = data.DisplayText;
}

function HelpResource(data) {
    var self = this;
    self.HelpResourceID = ko.observable(data.HelpResourceID);
    self.HelpResourceCategoryID = ko.observable(data.HelpResourceCategoryID);
    self.HelpResourceTypeID = ko.observable(data.HelpResourceTypeID);
    self.Topic = ko.observable(data.Topic);
    self.Description = ko.observable(data.Description);
    self.KeyWords = ko.observable(data.KeyWords);
    self.HelpResourceFilePath = ko.observable(data.HelpResourceFilePath);
    self.HelpResourceLink = ko.observable(data.HelpResourceLink);
}

function HelpResourceUploadViewModel() {
    var self = this;

    self.AddHelpResourceDivSelector = ko.observable('#divAddResource');

    self.HelpResourceUrl = ko.observable();
    self.HelpResourceValidateLinkUrl = ko.observable();
    self.UploadUrl = ko.observable();
    self.DownloadUrl = ko.observable();

    self.HelpResourceLinkIDConst = ko.observable();
    self.HelpResourceLinkConst = 'link';

    self.PostAjax = function (url, data, successCallBack, errorCallBack) {
        return $.ajax({
            type: 'POST',
            url: url,
            cache: false,
            data: data,
            datatype: 'json',
            success: function (data) {
                if (successCallBack != null || typeof (successCallBack) != undefined) {
                    successCallBack(data);
                }
            },
            error: function (data) {
                if (errorCallBack != null || typeof (errorCallBack) != undefined) {
                    errorCallBack(data);
                }
            }
        });
    };

    self.Categories = ko.observableArray([]);
    self.HelpResourceTypes = ko.observableArray([]);

    self.HelpResourceID = ko.observable();
    self.HelpResourceCategoryID = ko.observable();
    self.HelpResourceTypeID = ko.observable();
    self.HelpResourceFileTypeID = ko.observable();
    self.Topic = ko.observable();
    self.Description = ko.observable('');
    self.KeyWords = ko.observable('');
    self.HelpResourceFilePath = ko.observable('');
    self.HelpResourceLink = ko.observable('');

    self.HelpResourceFileNameVisible = ko.computed(function () {
        if (self.HelpResourceFilePath() == null || self.HelpResourceFilePath().trim() == '')
            return false;

        return true;
    });

    self.HelpResourceFileName = ko.computed(function () {
        if (self.HelpResourceFilePath() == null || self.HelpResourceFilePath().trim() == '')
            return '';

        var index = self.HelpResourceFilePath().lastIndexOf("/") + 1;
        var fileName = self.HelpResourceFilePath().substr(index);

        return fileName;
    });

    self.HelpResourceFileNameClickHandler = function () {
        if (self.HelpResourceFilePath() == null || self.HelpResourceFilePath().trim() == '')
            return;

        var url = self.DownloadUrl() + '?filePath=' + self.HelpResourceFilePath();
        window.open(url, '_blank');
    };

    self.HelpResourceLinkOrFile = ko.observable(self.HelpResourceLinkConst);
    self.SubmitClicked = ko.observable(false);

    self.TotalFileInQueue = ko.observable(0);
    self.TotalFileInQueue.subscribe(function () {
        if (self.TotalFileInQueue() == 1) {
            $('#uploadifive-file_upload').hide();
        } else {
            $('#uploadifive-file_upload').show();
        }
    });

    self.UploadError = ko.observable();
    self.ResoureLinkValidVisible = ko.observable(false);
    self.ResoureLinkInValidVisible = ko.observable(false);

    self.HelpResourceLink.subscribe(function () {
        self.ResoureLinkValidVisible(false);
        self.ResoureLinkInValidVisible(false);
    });

    self.SubmitButtonEnable = ko.computed(function () {
        if (self.Topic() == null || self.Topic().trim() === '') return false;
        if (self.KeyWords() == null || self.KeyWords().trim() === '') return false;
        if (self.HelpResourceLinkOrFile() === self.HelpResourceLinkConst && (self.HelpResourceLink() == null || self.HelpResourceLink().trim() === '')) return false;
        if (self.HelpResourceLinkOrFile() === self.HelpResourceLinkConst) {
            if (!self.ResoureLinkValidVisible()) return false;
            return true;
        }

        if ((self.HelpResourceID() == null || self.HelpResourceID() == 0) && self.TotalFileInQueue() == 0) return false;

        return true;
    });

    self.TopicStyleAttr = ko.computed(function () {
        if (!self.SubmitClicked()) return '';

        if (self.Topic() == null || self.Topic().trim() === '') {
            return 'border-color:red;';
        }

        return '';
    });

    self.HelpResourceUploadTitle = ko.computed(function () {
        if (typeof (self.HelpResourceID()) == undefined || self.HelpResourceID() == null) {
            return 'Add Help Resource';
        }

        return "Edit Help Resource";
    });

    self.LinkPanelVisible = ko.computed(function () {
        return self.HelpResourceLinkOrFile() === self.HelpResourceLinkConst;
    });

    self.FilePanelVisible = ko.computed(function () {
        return !self.LinkPanelVisible();
    });

    self.BuildFormData = function () {
        var result = {
            'HelpResourceCategoryID': self.HelpResourceCategoryID(),
            'HelpResourceTypeID': self.HelpResourceTypeID(),
            'HelpResourceFileTypeID': self.HelpResourceFileTypeID(),
            'Topic': self.Topic(),
            'Description': self.Description(),
            'KeyWords': self.KeyWords(),
            'HelpResourceLink': self.HelpResourceLink(),
            'HelpResourceID': self.HelpResourceID(),
            'HelpResourceLinkOrFile': self.HelpResourceLinkOrFile()
        };

        return result;
    };

    self.SubmitClickHandler = function () {
        self.SubmitClicked(true);

        var postData = self.BuildFormData();

        if (self.HelpResourceLinkOrFile() != self.HelpResourceLinkConst && self.TotalFileInQueue() > 0) {
            $('#file_upload').data('uploadifive').settings.formData = postData;
            $('#file_upload').uploadifive('upload');
            return;
        }

        ShowBlock($(self.AddHelpResourceDivSelector()), 'Loading');
        self.PostAjax(self.UploadUrl(), postData, function (allData) {
            $(self.AddHelpResourceDivSelector()).unblock();
            if (allData.success) {
                ConfirmDiaglog({ 'Message': 'Upload Successful.', 'width': 400, 'YesButtonCaption': 'Ok', 'ShowNoButton': false, 'HideCloseButton': 1 }, function (options) {
                    location.reload();
                }, null);
            } else {
                ConfirmDiaglog({ 'Message': allData.errorMessage, 'width': 400, 'YesButtonCaption': 'Ok', 'ShowNoButton': false, 'HideCloseButton': 1 }, null, null);
            }

        }, null);
    };

    self.DisplayResourceLinkValidStatus = function (validStatus) {
        if (validStatus == null) {
            self.ResoureLinkValidVisible(false);
            self.ResoureLinkInValidVisible(false);
        } else if (validStatus) {
            self.ResoureLinkValidVisible(true);
            self.ResoureLinkInValidVisible(!self.ResoureLinkValidVisible());
        } else {
            self.ResoureLinkValidVisible(false);
            self.ResoureLinkInValidVisible(!self.ResoureLinkValidVisible());
        }
    }

    self.ValidateLinkClickHandler = function () {
        ShowBlock($(self.AddHelpResourceDivSelector()), 'Loading');
        $.ajax({
            type: 'GET',
            url: self.HelpResourceValidateLinkUrl(),
            cache: false,
            data: { link: self.HelpResourceLink() },
            datatype: 'json',
            success: function (data) {
                $(self.AddHelpResourceDivSelector()).unblock();
                self.DisplayResourceLinkValidStatus(data.success);
            },
            error: function (data) {
                $(self.AddHelpResourceDivSelector()).unblock();
                self.DisplayResourceLinkValidStatus(false);
            }
        });
    };

    self.CancelClickHandler = function () {
        window.location = self.HelpResourceUrl();
    };
}
