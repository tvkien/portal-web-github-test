/**
 * Portal URL
 * @views VirtualTest/_ImportItemsFromLibrary.cshtml and ItemBank/ItemsFromLibrary.cshtml
 */
var portalUrl = portalUrl || {};
var itemsFromLibraryUtils = itemsFromLibraryUtils || {};

/**
 * Items From Library Utils
 * @type {Object}
 */
itemsFromLibraryUtils = {
    callbackAjaxImportItem: function(url, callback) {
        $.ajax({
            url: url,
            type: 'get',
            cache: false,
            data: {}
        }).done(callback);
    },
    callbackAjaxImportItemJSON: function(url, callback) {
        $.ajax({
            url: url,
            dataType: 'json',
            async: false,
            type: 'get',
            cache: false
        }).done(callback);
    }
};

function LoadImages(containerSelector) {
    $(containerSelector).find("img").each(function () {
        var image = $(this);
        var imageUrl = image.attr("src");
        if (IsNullOrEmpty(imageUrl)) {
            imageUrl = image.attr("source");
        }

        if (IsNullOrEmpty(imageUrl)) imageUrl = portalUrl.imageEmpty;

        var testItemMediaPath = $('#hidTestItemMediaPath').val();
        var isLoadImage = imageUrl.indexOf(testItemMediaPath) != -1;

        if (isLoadImage) imageUrl = imageUrl.replace(testItemMediaPath, '');

        if (imageUrl.charAt(0) == '/') imageUrl = imageUrl.substring(1);

        image.attr("source", '');
        image.attr("src", imageUrl);
        if (imageUrl.toLowerCase().indexOf("http") == 0) return;
        if (((imageUrl && imageUrl.toLowerCase().indexOf("itemset") >= 0) || isLoadImage)
            && imageUrl.toLowerCase().indexOf("getviewreferenceimg") < 0) {
            imageUrl = '/TestAssignmentRegrader/GetViewReferenceImg?imgPath=' + imageUrl;
            imageUrl = imageUrl + "&timestamp=" + new Date().getTime();
            image.attr("src", imageUrl);
        }
    });

    ResizeImagesBaseOnPercent('#qtiItemDataTable');
}

function IsNullOrEmpty(value) {
    return typeof (value) === "undefined" || value == null || $.trim(value) == '';
}
