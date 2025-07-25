/**
 * Items From Library URL
 * @views VirtualTest/_ImportItemsFromLibrary.cshtml and ItemBank/ItemsFromLibrary.cshtml
 */
var itemsFromLibraryUrl = itemsFromLibraryUrl || {};
var itemsFromLibraryUtils = itemsFromLibraryUtils || {};

/**
 * Get All Data Initialize
 * Optimize: data here for the first time load pop up Add Item from Library
 */
var qTI3PSubjectData = qTI3PSubjectData || null;
var criteriaGradesData = criteriaGradesData || null;
var criteriaQTI3PDOKData = criteriaQTI3PDOKData || null;
var qTI3PDifficultyData = qTI3PDifficultyData || null;
var qTI3PBloomsData = qTI3PBloomsData || null;
var qTI3PPassageNumberData = qTI3PPassageNumberData || null;
var passageGradesData = passageGradesData || null;
var assignedGradesForPassagesData = assignedGradesForPassagesData || null;
var qTI3PPassageSubjectData = qTI3PPassageSubjectData || null;
var qtiPassageSubjectsData = qtiPassageSubjectsData || null;
var qTI3PWordCountData = qTI3PWordCountData || null;
var qTI3PTextTypeData = qTI3PTextTypeData || null;
var qTI3PTextSubTypeData = qTI3PTextSubTypeData || null;
var qTI3PFleschKincaidData = qTI3PFleschKincaidData || null;
var itemBanksPersonalData = itemBanksPersonalData || null;
var districtCategoryData = districtCategoryData || null;
var personalStateStandardData = personalStateStandardData || null;

function KeyValuePair(data) {
    var self = this;
    self.Text = ko.observable(data.Name);
    self.Value = ko.observable(data.Id);
}

function KeyValuePairExtra(data) {
    var self = this;
    self.Text = ko.observable(data.Name);
    self.Value = ko.observable(data.Id);
    self.Description = ko.observable(data.Description);
}

function CreateItemFromLibraryViewModel() {
    var self = this;

    self.PersonalOrDistrict = ko.observable('PersonalItemLibrar');

    //_ItemFromLibraryFilter
    self.QTI3PSubjectId = ko.observable('');
    self.QTI3PSubjects1 = ko.observableArray([]);

    self.FilterGradeId = ko.observable('');
    self.FilterGrades = ko.observableArray([]);

    self.QTI3pDOKID = ko.observable('');
    self.QTI3pDOKs = ko.observableArray([]);

    self.DifficultyId = ko.observable('');
    self.Difficulties = ko.observableArray([]);

    self.BloomsId = ko.observable('');
    self.Bloomses = ko.observableArray([]);

    self.PassageNumberId = ko.observable('');
    self.PassageNumbers = ko.observableArray([]);

    self.PassageGradeId = ko.observable('');
    self.PassageGrades = ko.observableArray([]);
    self.PassageGradeNewId = ko.observable('');
    self.PassageGradesNew = ko.observableArray([]);

    self.PassageSubjectText = ko.observable('');
    self.PassageSubjects = ko.observableArray([]);
    self.PassageSubjectNewText = ko.observable('');
    self.PassageSubjectsNew = ko.observableArray([]);

    self.WordCountId = ko.observable('');
    self.WordCounts = ko.observableArray([]);

    self.TextTypeId = ko.observable('');
    self.TextTypes = ko.observableArray([]);

    self.TextSubTypeId = ko.observable('');
    self.TextSubTypes = ko.observableArray([]);

    self.FleschKincaidId = ko.observable('');
    self.FleschKincaidIds = ko.observableArray([]);

    self.DisplayQTI3PSubjects1 = ko.computed(function () {
        if (qTI3PSubjectData == null) {
            itemsFromLibraryUtils.callbackAjaxImportItem(
                itemsFromLibraryUrl.qTI3PSubjectDataUrl, function(data) {
                var mappedObjects = $.map(data, function (item) {
                    return new KeyValuePair(item);
                });
                self.QTI3PSubjects1(mappedObjects);
            });
        } else {
            var mappedObjects = $.map(qTI3PSubjectData, function (item) {
                return new KeyValuePair(item);
            });
            self.QTI3PSubjects1(mappedObjects);
        }
    });

    self.DisplayFilterGrades = ko.computed(function () {
        if (criteriaGradesData == null) {
            itemsFromLibraryUtils.callbackAjaxImportItem(
                itemsFromLibraryUrl.criteriaGradesDataUrl, function(data) {
                var mappedObjects = $.map(data, function (item) {
                    return new KeyValuePair(item);
                });
                self.FilterGrades(mappedObjects);
            });
        } else {
            var mappedObjects = $.map(criteriaGradesData, function (item) {
                return new KeyValuePair(item);
            });
            self.FilterGrades(mappedObjects);
        }
    });

    self.DisplayQTI3pDOKs = ko.computed(function () {
        if (criteriaQTI3PDOKData == null) {
            itemsFromLibraryUtils.callbackAjaxImportItem(
                itemsFromLibraryUrl.criteriaQTI3PDOKDataUrl, function(data) {
                var mappedObjects = $.map(data, function (item) {
                    return new KeyValuePair(item);
                });
                self.QTI3pDOKs(mappedObjects);
            });
        } else {
            var mappedObjects = $.map(criteriaQTI3PDOKData, function (item) {
                return new KeyValuePair(item);
            });
            self.QTI3pDOKs(mappedObjects);
        }
    });

    self.DisplayDifficulty = ko.computed(function () {
        if (qTI3PDifficultyData == null) {
            itemsFromLibraryUtils.callbackAjaxImportItem(
                itemsFromLibraryUrl.qTI3PDifficultyDataUrl, function(data) {
                var mappedObjects = $.map(data, function (item) {
                    return new KeyValuePair(item);
                });
                self.Difficulties(mappedObjects);
            });
        } else {
            var mappedObjects = $.map(qTI3PDifficultyData, function (item) {
                return new KeyValuePair(item);
            });
            self.Difficulties(mappedObjects);
        }
    });

    self.DisplayBloomses = ko.computed(function () {
        if (qTI3PBloomsData == null) {
            itemsFromLibraryUtils.callbackAjaxImportItem(
                itemsFromLibraryUrl.qTI3PBloomsDataUrl, function(data) {
                var mappedObjects = $.map(data, function (item) {
                    return new KeyValuePair(item);
                });
                self.Bloomses(mappedObjects);
            });
        } else {
            var mappedObjects = $.map(qTI3PBloomsData, function (item) {
                return new KeyValuePair(item);
            });
            self.Bloomses(mappedObjects);
        }
    });

    self.DisplayPassageNumber = ko.computed(function () {
        if (qTI3PPassageNumberData == null) {
            itemsFromLibraryUtils.callbackAjaxImportItem(
                itemsFromLibraryUrl.qTI3PPassageNumberDataUrl, function(data) {
                var mappedObjects = $.map(data, function (item) {
                    return new KeyValuePair(item);
                });
                self.PassageNumbers(mappedObjects);
            });
        } else {
            var mappedObjects = $.map(qTI3PPassageNumberData, function (item) {
                return new KeyValuePair(item);
            });
            self.PassageNumbers(mappedObjects);
        }
    });

    self.DisplayPassageGrades = ko.computed(function () {
        if (passageGradesData == null) {
            itemsFromLibraryUtils.callbackAjaxImportItem(
                itemsFromLibraryUrl.passageGradesDataUrl, function(data) {
                var mappedObjects = $.map(data, function (item) {
                    return new KeyValuePair(item);
                });
                self.PassageGrades(mappedObjects);
            });
        } else {
            var mappedObjects = $.map(passageGradesData, function (item) {
                return new KeyValuePair(item);
            });
            self.PassageGrades(mappedObjects);
        }
    });

    self.DisplayPassageGradesNew = ko.computed(function () {
        if (assignedGradesForPassagesData == null) {
            itemsFromLibraryUtils.callbackAjaxImportItem(
                itemsFromLibraryUrl.assignedGradesForPassagesDataUrl, function(data) {
                var mappedObjects = $.map(data, function (item) {
                    return new KeyValuePair(item);
                });
                self.PassageGradesNew(mappedObjects);
            });
        } else {
            var mappedObjects = $.map(assignedGradesForPassagesData, function (item) {
                return new KeyValuePair(item);
            });
            self.PassageGradesNew(mappedObjects);
        }
    });

    self.DisplayPassageSubjects = ko.computed(function () {
        if (qTI3PPassageSubjectData == null) {
            itemsFromLibraryUtils.callbackAjaxImportItem(
                itemsFromLibraryUrl.qTI3PPassageSubjectDataUrl, function(data) {
                var mappedObjects = $.map(data, function (item) {
                    return new KeyValuePair(item);
                });
                self.PassageSubjects(mappedObjects);
            });
        } else {
            var mappedObjects = $.map(qTI3PPassageSubjectData, function (item) {
                return new KeyValuePair(item);
            });
            self.PassageSubjects(mappedObjects);
        }
    });

    self.DisplayPassageSubjectsNew = ko.computed(function () {
        if (qtiPassageSubjectsData == null) {
            itemsFromLibraryUtils.callbackAjaxImportItem(
                itemsFromLibraryUrl.qtiPassageSubjectsDataUrl, function(data) {
                var mappedObjects = $.map(data, function (item) {
                    return new KeyValuePair(item);
                });
                self.PassageSubjectsNew(mappedObjects);
            });
        } else {
            var mappedObjects = $.map(qtiPassageSubjectsData, function (item) {
                return new KeyValuePair(item);
            });
            self.PassageSubjectsNew(mappedObjects);
        }
    });

    self.DisplayWordCount = ko.computed(function () {
        if (qTI3PWordCountData == null) {
            itemsFromLibraryUtils.callbackAjaxImportItem(
                itemsFromLibraryUrl.qTI3PWordCountDataUrl, function(data) {
                var mappedObjects = $.map(data, function (item) {
                    return new KeyValuePair(item);
                });
                self.WordCounts(mappedObjects);
            });
        } else {
            var mappedObjects = $.map(qTI3PWordCountData, function (item) {
                return new KeyValuePair(item);
            });
            self.WordCounts(mappedObjects);
        }
    });

    self.DisplayTextType = ko.computed(function () {
        if (qTI3PTextTypeData == null) {
            itemsFromLibraryUtils.callbackAjaxImportItem(
                itemsFromLibraryUrl.qTI3PTextTypeDataUrl, function(data) {
                var mappedObjects = $.map(data, function (item) {
                    return new KeyValuePair(item);
                });
                self.TextTypes(mappedObjects);
            });
        } else {
            var mappedObjects = $.map(qTI3PTextTypeData, function (item) {
                return new KeyValuePair(item);
            });
            self.TextTypes(mappedObjects);
        }
    });

    self.DisplayTextSubType = ko.computed(function () {
        if (qTI3PTextSubTypeData == null) {
            itemsFromLibraryUtils.callbackAjaxImportItem(
                itemsFromLibraryUrl.qTI3PTextSubTypeDataUrl, function(data) {
                var mappedObjects = $.map(data, function (item) {
                    return new KeyValuePair(item);
                });
                self.TextSubTypes(mappedObjects);
            });
        } else {
            var mappedObjects = $.map(qTI3PTextSubTypeData, function (item) {
                return new KeyValuePair(item);
            });
            self.TextSubTypes(mappedObjects);
        }
    });

    self.DisplayFleschKincaid = ko.computed(function () {
        if (qTI3PFleschKincaidData == null) {
            itemsFromLibraryUtils.callbackAjaxImportItem(
                itemsFromLibraryUrl.qTI3PFleschKincaidDataUrl, function(data) {
                var mappedObjects = $.map(data, function (item) {
                    return new KeyValuePair(item);
                });
                self.FleschKincaidIds(mappedObjects);
            });
        } else {
            var mappedObjects = $.map(qTI3PFleschKincaidData, function (item) {
                return new KeyValuePair(item);
            });
            self.FleschKincaidIds(mappedObjects);
        }
    });

    self.DistrictCategoryId = ko.observable('');
    self.DistrictCategories = ko.observableArray([]);

    self.ItemBankId = ko.observable('');
    self.ItemBanks = ko.observableArray([]);

    self.ItemSetId = ko.observable('');
    self.ItemSets = ko.observableArray([]);

    self.DisplayItemSets = ko.computed(function () {
        itemsFromLibraryUtils.callbackAjaxImportItem(
            itemsFromLibraryUrl.DisplayItemSetsUrl + self.ItemBankId(), function(data) {
                var mappedObjects = $.map(data, function (item) {
                return new KeyValuePair(item);
            });
            self.ItemSets(mappedObjects);
        });
    });

    self.DisplayDistrictCategories = ko.computed(function () {
        if (districtCategoryData == null) {
            itemsFromLibraryUtils.callbackAjaxImportItem(
                itemsFromLibraryUrl.districtCategoryDataUrl, function(data) {
                var mappedObjects = $.map(data, function (item) {
                    return new KeyValuePair(item);
                });
                self.DistrictCategories(mappedObjects);
            });
        } else {
            var mappedObjects = $.map(districtCategoryData, function (item) {
                return new KeyValuePair(item);
            });
            self.DistrictCategories(mappedObjects);
        }
    });
}
