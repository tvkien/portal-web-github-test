function RowCallbackAdmin(nRow, aData) {
    var iconStr = '<a href="javascript:void(0);" title="';
    iconStr += aData["HelpResourceTypeDisplayText"];
    iconStr += '" class="tooltipBox" ';
    iconStr += ' helpResourceID="' + aData["HelpResourceID"] + '"';
    iconStr += '>';
    iconStr += '<img src="';
    iconStr += aData["HelpResourceTypeIcon"];
    iconStr += '" class="u-inline-block u-v-align-middle icon-32-custom" style="margin-right:4px">';
    iconStr += '</a>';

    var editStr = '<a href="javascript:void(0);" title="Edit" class="tooltipBox jsEditHelpResource" ';
    editStr += ' helpResourceID="' + aData["HelpResourceID"] + '"';
    editStr += '>';
    editStr += '<img src="/Content/themes/Constellation/images/icons/fugue/pencil.png" width="16" height="16" style="margin-right: 4px">';
    editStr += '</a>';

    var deleteStr = '<a href="javascript:void(0);" title="Delete" class="tooltipBox jsDeleteHelpResource" ';
    deleteStr += ' helpResourceID="' + aData["HelpResourceID"] + '"';
    deleteStr += '>';
    deleteStr += '<img src="/Content/themes/Constellation/images/icons/fugue/cross-circle.png" width="16" height="16">';
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
