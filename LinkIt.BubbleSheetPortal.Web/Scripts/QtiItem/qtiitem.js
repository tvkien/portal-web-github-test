function parseXmlContentQtiItem(xmlContent) {
    var title = '';
    xmlContent = correctInlineChoice(xmlContent);
    $(xmlContent).find('.itemBody, itemBody, itembody').each(function () {
        var itemBody = $(this);
        itemBody.find("videolinkit").replaceWith(function () {
            return $('');
        });

        if ($(xmlContent).find("responsedeclaration").attr("partialgrading") == "1") {
            itemBody.find("sourcetext").each(function () {
                if ($(this).attr("pointvalue") > 0) {
                    $(this).addClass("marker-correct");
                }
            });
        } else {
            $(xmlContent).find("correctResponse").each(function () {
                var id = $(this).attr("identifier");
                itemBody.find("sourcetext[identifier=\"" + id + "\"]").addClass("marker-correct");
            });
        }

        title = itemBody.html();
    });
    //title = title.replace(/<videolinkit/g, "<video").replace(/<\/videolinkit>/g, "</video>").replace(/<sourcelinkit /g, "<source ").replace(/<\/sourcelinkit>/g, "</source>");
    title = title.replaceAll("<object", "<object style='display: none;'");
    var divTitle = '<div style="max-height:60px; overflow:hidden;max-width:346px">' + title + '</div>';
    return divTitle;
}
