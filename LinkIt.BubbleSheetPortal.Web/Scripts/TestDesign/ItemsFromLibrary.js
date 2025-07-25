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
var qTI3PSubjectData = null;
var criteriaGradesData = null;
var criteriaQTI3PDOKData = null;
var qTI3PDifficultyData = null;
var qTI3PBloomsData = null;
var qTI3PPassageNumberData = null;
var passageGradesData = null;
var assignedGradesForPassagesData = null;
var qTI3PPassageSubjectData = null;
var qtiPassageSubjectsData = null;
var qTI3PWordCountData = null;
var qTI3PTextTypeData = null;
var qTI3PTextSubTypeData = null;
var qTI3PFleschKincaidData = null;
var itemBanksPersonalData = null;
var districtCategoryData = null;
var personalStateStandardData = null;

itemsFromLibraryUtils.callbackAjaxImportItem(
    itemsFromLibraryUrl.qTI3PSubjectDataUrl, function(data) {
    qTI3PSubjectData = data;
});

itemsFromLibraryUtils.callbackAjaxImportItem(
    itemsFromLibraryUrl.criteriaGradesDataUrl, function(data) {
    criteriaGradesData = data;
});

itemsFromLibraryUtils.callbackAjaxImportItem(
    itemsFromLibraryUrl.criteriaQTI3PDOKDataUrl, function(data) {
    criteriaQTI3PDOKData = data;
});

itemsFromLibraryUtils.callbackAjaxImportItem(
    itemsFromLibraryUrl.qTI3PDifficultyDataUrl, function(data) {
    qTI3PDifficultyData = data;
});

itemsFromLibraryUtils.callbackAjaxImportItem(
    itemsFromLibraryUrl.qTI3PBloomsDataUrl, function(data) {
    qTI3PBloomsData = data;
});

itemsFromLibraryUtils.callbackAjaxImportItem(
    itemsFromLibraryUrl.qTI3PPassageNumberDataUrl, function(data) {
    qTI3PPassageNumberData = data;
});

itemsFromLibraryUtils.callbackAjaxImportItem(
    itemsFromLibraryUrl.passageGradesDataUrl, function(data) {
    passageGradesData = data;
});

itemsFromLibraryUtils.callbackAjaxImportItem(
    itemsFromLibraryUrl.assignedGradesForPassagesDataUrl, function(data) {
    assignedGradesForPassagesData = data;
});

itemsFromLibraryUtils.callbackAjaxImportItem(
    itemsFromLibraryUrl.qTI3PPassageSubjectDataUrl, function(data) {
    qTI3PPassageSubjectData = data;
});

itemsFromLibraryUtils.callbackAjaxImportItem(
    itemsFromLibraryUrl.qtiPassageSubjectsDataUrl, function(data) {
    qtiPassageSubjectsData = data;
});

itemsFromLibraryUtils.callbackAjaxImportItem(
    itemsFromLibraryUrl.qTI3PWordCountDataUrl, function(data) {
    qTI3PWordCountData = data;
});

itemsFromLibraryUtils.callbackAjaxImportItem(
    itemsFromLibraryUrl.qTI3PTextTypeDataUrl, function(data) {
    qTI3PTextTypeData = data;
});

itemsFromLibraryUtils.callbackAjaxImportItem(
    itemsFromLibraryUrl.qTI3PTextSubTypeDataUrl, function(data) {
    qTI3PTextSubTypeData = data;
});

itemsFromLibraryUtils.callbackAjaxImportItem(
    itemsFromLibraryUrl.qTI3PFleschKincaidDataUrl, function(data) {
    qTI3PFleschKincaidData = data;
});

itemsFromLibraryUtils.callbackAjaxImportItem(
    itemsFromLibraryUrl.itemBanksPersonalDataUrl, function(data) {
    itemBanksPersonalData = data;
});

itemsFromLibraryUtils.callbackAjaxImportItem(
    itemsFromLibraryUrl.districtCategoryDataUrl, function(data) {
    districtCategoryData = data;
});

itemsFromLibraryUtils.callbackAjaxImportItemJSON(
    itemsFromLibraryUrl.personalStateStandardDataUrl, function(data) {
    personalStateStandardData = data;
});
