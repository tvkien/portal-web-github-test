function RowCallbackPublish(nRow, aData) {
    var iconStr = '<a href="javascript:void(0);" title="';
    iconStr += aData["HelpResourceTypeDisplayText"];
    iconStr += '" class="tooltipBox" ';
    iconStr += ' helpResourceID="' + aData["HelpResourceID"] + '"';
    iconStr += '>';
    iconStr += '<img src="';
    iconStr += aData["HelpResourceTypeIcon"];
    iconStr += '" class="u-inline-block u-v-align-middle icon-32-custom">';
    iconStr += '</a>';

    $('td:eq(0)', nRow).addClass('u-text-center').html($(iconStr));

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
