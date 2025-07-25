CKEDITOR.plugins.add('texthotspot', {
    lang: 'en', // %REMOVE_LINE_CORE%
    init: function (editor) {
        var config = editor.config;
        var styles = {},
			allowedContent = [];
        var flag = false;
        
        editor.on('stylesSet', function (evt) {

            var stylesDefinitions = evt.data.styles;
            
            if (!stylesDefinitions)
                return;

            var style, styleName;

            // Put all styles into an Array.
            var styleDefinition = stylesDefinitions[1];

            //Get MarkerLinkit style
            for (var i = 0; i < stylesDefinitions.length; i++) {
                if (stylesDefinitions[i].name == "MarkerLinkit") {
                    styleDefinition = stylesDefinitions[i];
                    break;
                }
            }
            
            styleName = styleDefinition.name;
            style = new CKEDITOR.style(styleDefinition);
            
            if (!editor.filter.customConfig || editor.filter.check(style)) {
                style._name = styleName;
                style._.enterMode = config.enterMode;

                styles[styleName] = style;
                allowedContent.push(style);
            }

        });
        
        editor.addCommand('insertTextHotSpot', {
            exec: function (editor) {

            var style = styles['MarkerLinkit'];
            var elementText = editor.getSelection().getSelectedText();  
            var currentElement = editor.elementPath().elements;
            var hasMarker = false;

                var sel = editor.getSelection();
                //var off = sel.anchorOffset;
                var ran = sel.getRanges(0);
                
                //Fix for case the marker has add hot spot after that remove and add again.
                var endNode = "";
                var endoffset = "";
                var startNode = "";
                var startoffset = "";
                if (ran[0].endContainer.type == 1 && ran[0].endContainer.hasClass("marker-linkit"))
                {
                    endNode = ran[0].endContainer.$.lastChild
                    endoffset = ran[0].endContainer.$.lastChild.length;
                    startNode = ran[0].endContainer.$.firstChild;
                    startoffset = ran[0].endContainer.$.firstChild.length;
                }
                
                var allChildNodes = ran[0].cloneContents().$.childNodes;
                for (var i = 0; i < allChildNodes.length; i++) {
                    if (allChildNodes[i].className != undefined && allChildNodes[i].className.indexOf("marker-linkit") > -1) {
                        hasMarker = true;
                        var hsID = allChildNodes[i].getAttribute("hs_id");
                        $('iframe.cke_wysiwyg_frame').contents().find('body .marker-linkit[hs_id=' + hsID + ']').contents().unwrap();
                    } else {
                        if (allChildNodes[i].nodeName != "#text") {
                            var childElementHasMarker = allChildNodes[i].getElementsByClassName("marker-linkit");
                            if (childElementHasMarker.length > 0) {
                                hasMarker = true;
                                $(childElementHasMarker).each(function () {
                                    var myhsID = $(this).attr("hs_id");
                                    $('iframe.cke_wysiwyg_frame').contents().find('body .marker-linkit[hs_id=' + myhsID + ']').contents().unwrap();
                                });
                            }
                        }
                    }
                }

                for (var i = 0; i < currentElement.length; i++) {
                    if (editor.elementPath().elements[i] != undefined) {
                        if (editor.elementPath().elements[i].hasClass("marker-linkit")) {
                            hasMarker = true;
                            $('iframe.cke_wysiwyg_frame').contents().find('body .marker-linkit[hs_id=' + editor.elementPath().elements[i].getAttribute("hs_id") + ']').contents().unwrap();
                        }
                    }
                }

                if (startNode != "") {
                    ran[0].endContainer.$ = endNode;
                    ran[0].endOffset = endoffset;
                    ran[0].startContainer.$ = startNode;
                    ran[0].startOffset = 0;
                }
                
                sel.selectRanges(ran);

                //Remove source item
                removeSourceTextHotSpot();

                //Remove correctResponse 
                removeCorrectResponseTextHotSpot();

                if (hasMarker) {
                    editor['removeStyle'](style);
                } else {
                    if (elementText.trim() == "") {
                        customAlert('Please highlight text to add/remove text hot spot.');
                    } else {
                        var newID = createHSID();
                        style._.definition.attributes.hs_id = newID;
                        editor['applyStyle'](style);
                        iResult[0].source.push({ identifier: newID, pointValue: "0"});
                    }
                }
            }
        });
        
        editor.ui.addButton('TextHotSpot',
		{
		    label: 'Add/Remove Hot Spots',
		    title: 'Add/Remove Hot Spots',
		    command: 'insertTextHotSpot',
		    toolbar: 'insertTextHotSpot,10',
		    allowedContent: allowedContent
		});
	}
});

/***
* Create HS ID when create new Text Hot Spot
***/

function createHSID() {
    //Get all id of text hot spot
    var $allId = $('iframe.cke_wysiwyg_frame').contents().find('body .marker-linkit');
    var srcId = "HS_" + ($allId.size() + 1);

    for (m = 0; m < $allId.length; m++) {
        resId = "HS_" + (m + 1);
        if ($allId.eq(m).attr("hs_id") != resId) {

            var isOnlyOne = true;
            for (k = 0; k < $allId.length; k++) {
                if (resId == $allId.eq(k).attr("hs_id")) {
                    isOnlyOne = false;
                }
            }

            if (isOnlyOne) {
                srcId = resId;
                break;
            }

        }
    }

    return srcId;
}

/***
* Remove source item of text hot spot
***/

function removeSourceTextHotSpot() {
    var sourceItem = iResult[0].source;
    //Store remove index
    var removeId = [];
    for (var i = 0; i < sourceItem.length; i++) {
        if ($('iframe.cke_wysiwyg_frame').contents().find('body .marker-linkit[hs_id=' + sourceItem[i].identifier + ']').size() == 0) {
            removeId.push(i);
        }
    }
    var totalLength = parseInt(removeId.length);
    if (totalLength>0) {
        for (var m = totalLength - 1; m >= 0 ; m--) {
            var indexInt = removeId[m];
            sourceItem.splice(indexInt, 1);
        }
    }
}

/***
* Remove source item of text hot spot
***/

function removeCorrectResponseTextHotSpot() {
    var sourceItem = iResult[0].correctResponse;
    //Store remove index
    var removeId = [];
    for (var i = 0; i < sourceItem.length; i++) {
        if ($('iframe.cke_wysiwyg_frame').contents().find('body .marker-linkit[hs_id=' + sourceItem[i].identifier + ']').size() == 0) {
            removeId.push(i);
        }
    }
    var totalLength = parseInt(removeId.length);
    if (totalLength > 0) {
        for (var m = totalLength - 1; m >= 0 ; m--) {
            var indexInt = removeId[m];
            sourceItem.splice(indexInt, 1);
        }
    }
}