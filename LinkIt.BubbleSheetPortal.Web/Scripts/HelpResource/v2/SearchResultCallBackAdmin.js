function RowCallbackAdmin(nRow, aData) {
    var iconStr = '<a href="javascript:void(0);" title="';
    iconStr += aData["HelpResourceTypeDisplayText"];
    iconStr += '" class="tooltipBox" ';
    iconStr += ' helpResourceID="' + aData["HelpResourceID"] + '"';
    iconStr += '>';
    iconStr += '<img src="';
    iconStr += aData["HelpResourceTypeIcon"];
    iconStr += '" class="d-inline-block u-v-align-middle icon-32-custom me-3">';
    iconStr += '</a>';

    var editStr = '<a href="javascript:void(0);" title="Edit" class="tooltipBox jsEditHelpResource me-3 align-middle" ';
    editStr += ' helpResourceID="' + aData["HelpResourceID"] + '"';
    editStr += '>';
    editStr += '<i class="custom-icon fa-solid fa-pencil icon-grey"></i>';
    editStr += '</a>';

    var deleteStr = '<a href="javascript:void(0);" title="Delete" class="tooltipBox jsDeleteHelpResource align-middle" ';
    deleteStr += ' helpResourceID="' + aData["HelpResourceID"] + '"';
    deleteStr += '>';
    deleteStr += '<i class="custom-icon fa-solid fa-circle-xmark icon-red"></i>';
    deleteStr += '</a>';

    $('td:eq(0)', nRow).addClass('u-text-center').html($(iconStr)).append($(editStr)).append($(deleteStr));

    var openHelpResourceTitle = '';
    if (aData["HelpResourceFileTypeID"] == 2) {
        openHelpResourceTitle += 'Open Link';
    } else {
        if (aData["FileType"] != null) {
            openHelpResourceTitle += 'Open ' + aData["FileType"];
        }
    }

    var openStr = '<a href="javascript:void(0);" title="' + openHelpResourceTitle + '" class="tooltipBox jsOpenHelpResource" '
        + ' helpResourceFileTypeID="' + aData["HelpResourceFileTypeID"] + '"'
        + ' helpResourceFilePath="' + aData["HelpResourceFilePath"] + '"'
        + ' helpResourceLink="' + aData["HelpResourceLink"] + '">';

    openStr += openHelpResourceTitle + '</a>';

    $('td:eq(1)', nRow).html(openStr);

    return nRow;
}
