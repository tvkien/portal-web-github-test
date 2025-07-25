
function Category(data, isOnlyOneCat) {
    var self = this;
    self.Index = data.Index;
    self.ID = data.ID;
    self.DisplayText = data.DisplayText;
    if (isOnlyOneCat) {
        self.Checked = ko.observable(true);
    } else {
        self.Checked = ko.observable(false);
    }
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
    self.Description = data.Description;
    self.ImgPath = data.ImgPath;
}

function HelpResourceViewModel() {
    var self = this;

    self.PostAjax = function (url, data, successCallBack, errorCallBack) {
        $.ajax({
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
                if (errorCallBack != null || typeof(errorCallBack) != undefined) {
                    errorCallBack(data);
                }
            }
        });
    };

    self.Pagging = function (array, totalPage, currentPage) {
        var result = [];
        if (array == null) return result;
        ko.utils.arrayForEach(array, function (item) {
            if (item.Index % totalPage == currentPage) {
                result.push(item);
            }
        });

        return result;
    };

    self.HelpResourceTypes = ko.observableArray([]);

    self.Categories = ko.observableArray([]);

    self.LeftCategories = ko.computed(function () {
        return self.Pagging(self.Categories(), 3, 1);
    });

    self.CenterCategories = ko.computed(function () {
        return self.Pagging(self.Categories(), 3, 2);
    });

    self.RightCategories = ko.computed(function () {
        return self.Pagging(self.Categories(), 3, 0);
    });

    self.SearchText = ko.observable('');
    self.SelectedCategories = ko.computed(function () {
        if (self.Categories() == null) return '';
        var result = '';
        var checkedIndex = 0;
        ko.utils.arrayForEach(self.Categories(), function (item) {
            if (item.Checked()) {
                if (checkedIndex != 0) result += ',';
                result += item.ID;
                checkedIndex++;
            }
        });

        return result;
    });

    self.FileTypes = ko.observableArray([]);

    self.AllChecked = ko.computed({
        read: function () {
            var firstUnchecked = ko.utils.arrayFirst(self.Categories(), function (item) {
                return item.Checked() == false;
            });
            return self.Categories().length && !firstUnchecked;
        },
        write: function (value) {
            ko.utils.arrayForEach(self.Categories(), function (item) {
                item.Checked(value);
            });
        }
    }).extend({ throttle: 1 });

    self.SelectAll = function () {
        self.AllChecked(true);
    };

    self.SelectNone = function () {
        self.AllChecked(false);
    };

    self.GetFileTypeByID = function (id) {
        if (self.FileTypes() == null || self.FileTypes().length == 0) return null;

        var fileType = ko.utils.arrayFirst(self.FileTypes(), function (item) {
            return item.ID == id;
        });

        return fileType;
    };

    self.ClearFilter = function () {
      self.SearchText('');
      self.AllChecked(false);
      if (self.Categories() == null) return;
    };
}
