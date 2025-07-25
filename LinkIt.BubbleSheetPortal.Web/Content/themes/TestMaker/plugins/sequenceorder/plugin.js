/**
 * @license Copyright (c) 2003-2013, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

CKEDITOR.plugins.add('sequenceorder', {
	requires: 'dialog',
	lang: 'en', // %REMOVE_LINE_CORE%
	icons: 'sortItem', // %REMOVE_LINE_CORE%
	hidpi: true, // %REMOVE_LINE_CORE%
	init: function( editor ) {
        var pluginName = 'insertSequenceOrder';

	    var objMessage = {
	        messageOurOfRange: 'Please edit range fit to source',
	        messageNoEmptySequence: 'Item Sequence cannot be empty',
	        longItemSource: 'Note view into item editor',
	        messageNoOneItem: 'Correct Item sources have more one',
	        messageSameValue: 'Item Sources have been duplicated.'
	    };

	    var eleSequenceOrder = '';
	    var isEditSequenceOrder = false;
        var currentSequenceOrderResId = '';

        var resetGradingAlgorithmic = function (dialog) {
            var $dialog = $(dialog);

            $dialog.find('#pointSequenceOrder').parent().addClass('is-disabled');
            $dialog.find('#pointSequenceOrder').val('0');
            $dialog.find('.correct_answer').css({
                'visibility': 'hidden',
                'height': '0'
            });
        };

        var resetGradingNormal = function (dialog) {
            var $dialog = $(dialog);

            $dialog.find('#pointSequenceOrder').parent().removeClass('is-disabled');
            $dialog.find('#pointSequenceOrder').val('1');
            $dialog.find('.correct_answer').css({
                'visibility': 'visible',
                'height': 'auto'
            });
        };

        var onShowGradingAlgorithmic = function (iresult, currentId, dialog) {
            var $dialog = $(dialog);

            for (var i = 0, len = iresult.length; i < len; i++) {
                var itemResult = iResult[i];
                if (itemResult.responseIdentifier === currentId && itemResult.type === 'sequence') {
                    if (itemResult.responseDeclaration.algorithmicGrading === '1') {
                        $dialog.find('#sq-grading-algorithmic').prop('checked', true);
                        $dialog.find('#pointSequenceOrder').parent().addClass('is-disabled');
                        $dialog.find('.correct_answer').css({
                            'visibility': 'hidden',
                            'height': '0'
                        });
                    }
                    break;
                }
            }
        };

		editor.ui.addButton('SequenceOrder', {
		    label: 'Sequence Order',
		    title: 'Sequence Order',
		    command: pluginName,
		    icon: this.path + 'icons/stock_sort-criteria.png',
		    toolbar: 'insertSequenceOrder,30'
		});

		editor.widgets.add('sequenceorder', {
		    inline: true,
		    mask: true,
		    allowedContent: { div: { classes: 'sequenceBlock,span,audioIcon,sequenceMark', attributes: '!id,name,contenteditable' } },
		    template: '<div class="sequenceBlock"></div>'
		});

	    //set event click single click
		editor.on('contentDom', function () {
		    //set single click popup
		    var singleclick = $(editor.window.getFrame().$).contents().find('.single-click');
		    singleclick.unbind("click").on('click', function (e) {
		        var element = $(e.target).parent();
		        //Move selection to parent of multipleChoiceMark
		        if (CKEDITOR.env.safari) {
		            editor.getSelection().getSelectedElement();
		        } else {
		            editor.getSelection().selectElement(editor.document.getActive().getParent());
		        }
		        //The status to editor know this is update
		        isEditSequenceOrder = true;
		        eleSequenceOrder = element;
		        editor.openDialog(pluginName, function () {
		            currentSequenceOrderResId = loadDataSequenceOrder(element);
		        });
		    });
        });

		editor.on('doubleclick', function (evt) {
		    var element = evt.data.element;

		    if (element.hasClass('sequenceMark')) {
                var parents = element.getParents();
                var parent;

                for (var i = 0; i < parents.length; i++) {
                    parent = parents[i];
                    if (parent.hasClass('sequenceBlock')) {
                        break;
                    }
                }

                eleSequenceOrder = parent;

		        //Move selection to parent of sequenceMark
		        editor.getSelection().selectElement(eleSequenceOrder);

		        evt.data.dialog = pluginName;

		        //The status to editor know this is update
		        isEditSequenceOrder = true;
		        //Load data to popup
                currentSequenceOrderResId = loadDataSequenceOrder(eleSequenceOrder);

                dblickHandlerToolbar(editor);
		    }
        });

        editor.addCommand(pluginName, new CKEDITOR.dialogCommand(pluginName));

        var getUpDownNumber = function (selector, min, max) {
            var $selector = $(selector);

            $selector.ckUpDownNumber({
                minNumber: min,
                maxNumber: max,
                width: 18,
                height: 13
            });
        };

		CKEDITOR.dialog.add(pluginName, function (editor) {
		    var htmlString = '';

            htmlString += '<div class="contentSequenceOrder">';
            htmlString += '<div class="m-b-5">';
            htmlString += '<input type="radio" name="sq-grading" id="sq-grading-normal" checked/>';
            htmlString += '<label for="sq-grading-normal"> Normal Grading</label>';
            htmlString += '&nbsp;&nbsp;&nbsp;<input type="radio" name="sq-grading" id="sq-grading-algorithmic"/>';
            htmlString += '<label for="sq-grading-algorithmic"> Algorithmic Grading</label>';
            htmlString += '</div>';
            htmlString += '<div class="clear10"></div>';
		    htmlString += '<div class="colLeft">';
		    htmlString += '<div class="ck_vertical_horizontal">';
		    htmlString += '<div class="orientation">';
		    htmlString += ' Orientation: <input orientation="vertical" type="radio" id="_vertical" name="ck_vertical_horizontal" />';
		    htmlString += ' <label for="_vertical">Vertical</label>';
		    htmlString += ' <input orientation="horizontal" type="radio" id="_horizontal" name="ck_vertical_horizontal" />';
		    htmlString += ' <label for="_horizontal">Horizontal</label>';
		    htmlString += '</div>';
            htmlString += '<div class="itemWidth">Item Width: <input type="text" maxlength="3" id="itemSequenceWidth" class="itemSequenceWidth point" value="150" /></div>';
		    htmlString += '<div class="fleft" style="display: none;">Out or Range: <input type="text" id="outOfRange" class="point" value="1" /></div>';
		    htmlString += '<div class="clear10"></div>';
		    htmlString += '</div>';
		    htmlString += '</div>';
		    htmlString += '<div class="colRight">';
		    htmlString += '<div class="fleft">Point value: <input type="text" id="pointSequenceOrder" class="point" value="1" /></div>';
		    htmlString += '<div class="clear"></div>';
		    htmlString += '<p><a href="#" class="cke_dialog_ui_button" title="Add Draggable Object" id="btnAddSource">';
		    htmlString += '<span class="cke_dialog_ui_button">Add Draggable Object</span></a></p>';
		    htmlString += '</div>';
		    htmlString += '<div class="clear"></div>';
		    htmlString += '<div class="source_sortable">';
		    htmlString += '<h3>Order Shown in Test Taker</h3>';
		    htmlString += '<div class="content_source_sortable">';
		    htmlString += '<p class="message_source_item">placeholder to display source item</p>';
		    htmlString += '<ul id="source_list_sortable" class="source_list_sortable connectedSortable">';
		    htmlString += '</ul>';
		    htmlString += '<div class="clear"></div>';
		    htmlString += '</div>';
		    htmlString += '</div>';
		    htmlString += '<div class="correct_answer">';
		    htmlString += '<h3>Correct Order</h3>';
		    htmlString += '<div class="content_correct_answer">';
		    htmlString += '<p class="message_source_item_correct">placeholder to display source item correct</p>';
		    htmlString += '<ul id="source_correct_list_sortable" class="source_correct_list_sortable connectedSortable">';
		    htmlString += '</ul>';
		    htmlString += '<div class="clear"></div>';
		    htmlString += '</div>';
		    htmlString += '</div><div class="clear"></div>';
		    htmlString += '<p class="errorMessage" id="messageOurOfRange"></p>';
		    htmlString += '<div class="clear"></div>';
		    htmlString += '</div>';
		    htmlString += '<p class="warningMessage" id="warningMessage"></p>';
		    htmlString += '<div style="display: none;" class="popup-image-hotspot pop-source-item" id="popupSourceItem"></div>';
		    htmlString += '<div style="display: none;" class="popup-overlay" id="popupSourceItemOverlay"></div>';


		    return {
		        title: 'Drag and Drop Sequence/Order',
		        minWidth: 500,
                width: IS_V2 ? 700 : undefined,
		        minHeight: 50,
		        contents: [
                    {
                        id: 'sortitem',
                        label: 'Sort Item',
                        elements: [
                            {
                                type: 'html',
                                html: htmlString,
                                onLoad: function () {
                                    var $dialog = $(this.getDialog().getElement().$);

                                    //load first data sequence
                                    if (isEditSequenceOrder) {
                                        currentSequenceOrderResId = loadDataSequenceOrder(eleSequenceOrder);
                                    }

                                    $dialog .find('input[type="radio"][name="sq-grading"]').on('change', function () {
                                        var $grading = $(this);
                                        var gradingMethod = $grading.attr('id');

                                        if (gradingMethod === 'sq-grading-algorithmic') {
                                            resetGradingAlgorithmic($dialog);
                                        } else {
                                            resetGradingNormal($dialog);
                                        }
                                    });
                                },
                                onShow: function () {
                                    var $dialog = $(this.getDialog().getElement().$);
                                    var source_list_sortable = $('.source_sortable:visible #source_list_sortable');
                                    var source_correct_list_sortable = $('.correct_answer:visible #source_correct_list_sortable');
                                    var btnAddSource = $('#btnAddSource:visible');
                                    var message_source_item = $('.message_source_item');
                                    var message_source_item_correct = $('.message_source_item_correct');
                                    var ck_vertical = $('#_vertical:visible');
                                    var ck_horizontal = $('#_horizontal:visible');
                                    var ckRadio = $('input[name="ck_vertical_horizontal"]');
                                    var source_sortable = $('.source_sortable:visible');
                                    var correct_answer = $('.correct_answer:visible');
                                    var txtRange = $('#outOfRange:visible');
                                    var messageOurOfRange = $('#messageOurOfRange');
                                    var warningMessage = $('#warningMessage');
                                    var htmlItem = '';
                                    var objItem = '';
                                    var valueItem = '';

                                    //reset sequence order
                                    refreshResponseId();
                                    if (!isEditSequenceOrder) {
                                        resetSequenceOrder();
                                    } else {
                                        interactiveUserGraphicInterface();
                                        resetEditSequenceOrder();
                                    }

                                    getUpDownNumber($('#pointSequenceOrder'), 0, 1000);
                                    getUpDownNumber($('#itemSequenceWidth'), 0, 999);

                                    onShowGradingAlgorithmic(iResult, currentSequenceOrderResId, $dialog);

                                    $('#itemSequenceWidth').on("change", function () {
                                        $("#source_list_sortable li, #source_correct_list_sortable li").css({ "width": $(this).val() });
                                    });

                                    $(".itemWidth .ckUpDownNumber .ckUDNumber").click(function () {
                                        $("#source_list_sortable li, #source_correct_list_sortable li").css({ "width": $("#itemSequenceWidth").val() });
                                    });

                                    $('.btnSortItem').unbind('click').click(function (evt) {
                                        showSourceSortable();
                                        return false;
                                    });

                                    btnAddSource.unbind('click').click(function () {

                                        var wBodyContent = $('.cke_dialog_contents_body:visible').width();
                                        var wNewItem = 'auto';
                                        var arrWItems = [];
                                        var wItem = $("#itemSequenceWidth").val();

                                        message_source_item.hide();
                                        message_source_item_correct.hide();
                                        var srcId = createIdSourceItem(source_list_sortable.find('.source_item_type'), 'id', 'SRC_');
                                        var idx = srcId.split('_');
                                        valueItem = 'Item ' + idx[1];

                                        if (ck_horizontal.is(':checked')) {

                                            if (wBodyContent > 660) {
                                                warningMessage.text(objMessage.longItemSource).fadeIn(2000).fadeOut();
                                            }

                                            objItem = new SourceItem(srcId, wItem, "auto", 'horizontal', valueItem);
                                            htmlItem = objItem.render();
                                            source_list_sortable.append(htmlItem).show();
                                            source_correct_list_sortable.append(htmlItem).show();

                                            htmlItem = '';
                                            objItem = '';
                                        }
                                        if (ck_vertical.is(':checked')) {
                                            objItem = new SourceItem(srcId, wItem, "auto", 'vertical', valueItem);
                                            htmlItem = objItem.render();
                                            source_list_sortable.append(htmlItem).show();
                                            source_correct_list_sortable.append(htmlItem).show();
                                            htmlItem = '';
                                            objItem = '';
                                        }

                                        $("#source_list_sortable li, #source_correct_list_sortable li").css({ "width": wItem });

                                        messageOurOfRange.text('').hide();
                                        interactiveUserGraphicInterface();
                                        return false;
                                    });

                                    ckRadio.unbind('click').click(function() {
                                        var typeOrientation = $(this).attr('orientation');
                                        var wBodyContentPopup = $('.cke_dialog_contents_body:visible');
                                        var arrWidthItems = [];
                                        var arrLenCharacter = [];
                                        var wDefault = 245;
                                        var maxWidth = '';
                                        var highest = '';
                                        var idItem = '';

                                        if (source_list_sortable.find('li').length === 0) {
                                            switch (typeOrientation) {
                                                case 'vertical':
                                                    $('.source_list_sortable').attr('orientation', 'vertical');
                                                    $('.source_correct_list_sortable').attr('orientation', 'vertical');
                                                    source_sortable.css({'width': '49%','float': 'left', 'margin-right': '5px'});
                                                    correct_answer.css({'width': '49%','float': 'left'});
                                                    break;
                                                case 'horizontal':
                                                    $('.source_list_sortable').attr('orientation', 'horizontal');
                                                    $('.source_correct_list_sortable').attr('orientation', 'horizontal');
                                                    source_sortable.css({'width': 'auto','float': 'none','margin-right': '0px'});
                                                    correct_answer.css({'width': 'auto','float': 'none'});
                                                    break;
                                            }
                                        } else {
                                            switch (typeOrientation) {
                                                case 'vertical':

                                                    //find max width item source
                                                    maxWidth = source_list_sortable.find('li').eq(0).width();
                                                    $('.source_list_sortable').attr('orientation', 'vertical');
                                                    $('.source_correct_list_sortable').attr('orientation', 'vertical');

                                                    source_sortable.css({
                                                        'width': '49%',
                                                        'float': 'left',
                                                        'margin-right': '5px'
                                                    });

                                                    correct_answer.css({
                                                        'width': '49%',
                                                        'float': 'left',
                                                    });

                                                    $('.contentSequenceOrder .source_list_sortable, .contentSequenceOrder .source_correct_list_sortable').css({
                                                        'width': 'auto'
                                                    });

                                                    var widthItem = $('#itemSequenceWidth').val();

                                                    source_list_sortable.find('li').removeAttr('style').css({
                                                        'display': 'block',
                                                        'width': widthItem + 'px'
                                                    });

                                                    source_correct_list_sortable.find('li').removeAttr('style').css({
                                                        'display': 'block',
                                                        'width': widthItem + 'px'
                                                    });

                                                    break;
                                                case 'horizontal':
                                                    //find max width item source
                                                    maxWidth = source_list_sortable.find('li').eq(0).width();
                                                    $('.source_list_sortable').attr('orientation', 'horizontal');
                                                    $('.source_correct_list_sortable').attr('orientation', 'horizontal');

                                                    source_sortable.css({
                                                        'width': 'auto',
                                                        'float': 'none',
                                                        'margin-right': '0px'
                                                    });

                                                    correct_answer.css({
                                                        'width': 'auto',
                                                        'float': 'none',
                                                    });
                                                    $('.contentSequenceOrder .source_list_sortable, .contentSequenceOrder .source_correct_list_sortable').css({
                                                        'width': '500px'
                                                    });

                                                    var widthItem = $('#itemSequenceWidth').val();

                                                    source_list_sortable.find('li').removeAttr('style').css({
                                                        'width': widthItem + 'px'
                                                    });

                                                    source_correct_list_sortable.find('li').removeAttr('style').css({
                                                        'width': widthItem + 'px'
                                                    });

                                                    //push width item into array
                                                    $('#source_list_sortable').find('li').each(function () {
                                                        idItem = $(this).attr('id');
                                                        var lenItemCharacter = $(this).find('.content').text();
                                                        arrLenCharacter.push({
                                                            lengthChar: lenItemCharacter.length,
                                                            id: idItem
                                                        });
                                                    });

                                                    $.each(arrLenCharacter, function (key, item) {

                                                        if (item.lengthChar > highest) {
                                                            highest = item.lengthChar;
                                                            idItem = item.id;
                                                        }
                                                    });
                                                    maxWidth = $('#source_list_sortable').find('li#' + idItem).outerWidth();

                                                    source_list_sortable.find('li').css({
                                                        'width': maxWidth,
                                                    });

                                                    source_correct_list_sortable.find('li').css({
                                                        'width': maxWidth,
                                                    });


                                                    break;
                                            }
                                        }
                                    });
                                }
                            }
                        ]
                    }
		        ],
		        onOk: function () {
		            var source_list_sortable = $('#source_list_sortable:visible');
		            var source_correct_list_sortable = $('#source_correct_list_sortable');
		            var messageOurOfRange = $('#messageOurOfRange');
		            var ck_vertical = $('#_vertical:visible');
		            var ck_horizontal = $('#_horizontal:visible');
		            var pointSequenceOrder = $('#pointSequenceOrder:visible');
		            var source_sortable = $('.source_sortable:visible');
		            var correct_answer = $('.correct_answer:visible');
		            var wBodyContentPopup = $('.cke_dialog_contents_body:visible');
		            var typeOrientation = '';
		            var responseId = '';
		            var htmlStringEditor = '';
		            var correctResponse = '';
		            var arrCorrectResponse = [];
		            var sourceItem = [];
		            var wFrameVertical = '';
		            var wItemHorizontal = '';
                    var sameValue = false;
                    var isAlgorithmicGrading = $('#sq-grading-algorithmic').is(':checked');

		            if (source_list_sortable.find('li').length === 0) {
		                messageOurOfRange.text(objMessage.messageNoEmptySequence).show();
		                return false;
		            }

		            if (source_correct_list_sortable.find('li').length === 1) {
		                messageOurOfRange.text(objMessage.messageNoOneItem).show();
		                return false;
		            }


		            var $correntLi = source_correct_list_sortable.find('li');
		            $correntLi.each(function (index) {
		                var identifier = $(this).attr('id');
		                var valueContent = $(this).find('.content').text();
		                arrCorrectResponse.push(identifier);

		                //Check if the value of source item has same value
		                for (var i = 0; i < $correntLi.length; i++) {
		                    if (i != index) {
		                        if ($correntLi.eq(i).find('.content').text() == valueContent) {
		                            sameValue = true;
		                            break;
		                        }
		                    }
		                }
		            });

		            if (sameValue) {
		                messageOurOfRange.text(objMessage.messageSameValue).show();
		                arrCorrectResponse = [];
		                return false;
		            }

		            source_list_sortable.find('li').each(function () {
		                var identifier = $(this).attr('id');
		                var valueContent = $(this).find('.content').html();

		                sourceItem.push({
		                    identifier: identifier,
		                    value: valueContent
		                });
		            });

		            correctResponse = arrCorrectResponse.join(',');
		            if (isEditSequenceOrder) {
		                //Update for Sequence order
		                typeOrientation = ck_horizontal.is(':checked') ? 'horizontal' : 'vertical';

		                for (var i = 0, leIresult = iResult.length; i < leIresult; i++) {
		                    var itemResult = iResult[i];
		                    if (itemResult.responseIdentifier === currentSequenceOrderResId && itemResult.type === 'sequence') {
		                        responseId = currentSequenceOrderResId;
		                        iResult[i].responseDeclaration.pointsValue = pointSequenceOrder.val();
		                        iResult[i].correctResponse = correctResponse;
		                        iResult[i].orientation = typeOrientation;
                                iResult[i].widthItem = $('#itemSequenceWidth').val();
                                iResult[i].responseDeclaration.absoluteGrading = isAlgorithmicGrading ? '0' : '1';
                                iResult[i].responseDeclaration.algorithmicGrading = isAlgorithmicGrading ? '1' : '0';
		                        break;
		                    }
		                }
		            }
		            else {
		                //Update for Sequence order
		                typeOrientation = ck_horizontal.is(':checked') ? 'horizontal' : 'vertical';
		                //Create response identify and make sure it doesn't conflict with current.
		                responseId = createResponseId();
		                //push sequence into iResult
		                iResult.push({
		                    correctResponse: correctResponse,
		                    orientation: typeOrientation,
		                    type: "sequence",
		                    widthItem: $('#itemSequenceWidth').val(),
		                    responseIdentifier: responseId,
		                    responseDeclaration: {
                                pointsValue: pointSequenceOrder.val(),
                                absoluteGrading: isAlgorithmicGrading ? '0' : '1',
                                algorithmicGrading: isAlgorithmicGrading ? '1' : '0',
		                        baseType: "identifier"
		                    }
		                });
		            }

		            //apply item correct source into editor
		            htmlStringEditor = buildHtmlSequence(responseId, typeOrientation, sourceItem, wFrameVertical, $('#itemSequenceWidth').val(), "auto", isEditSequenceOrder);

		            //Remove old multiple choice and add new one. This case just for safari only.
		            if (CKEDITOR.env.safari && isEditSequenceOrder) {
		                var oldHtml = $('iframe[allowtransparency]').contents().find('body').find('#' + responseId);
		                    oldHtml.replaceWith(htmlStringEditor); // cause duplicate responseId,so remove old
		            } else {
                        editor.insertHtml(htmlStringEditor);
		            }

		            //set single click popup
		            var tagBodys = $('iframe[allowtransparency]').contents().find('.sequenceBlock#' + responseId);
		            var singleclick = tagBodys.find('.single-click');
		            singleclick.unbind("click").on('click', function (e) {
		                var ele = $(e.target).parent();
		                //Move selection to parent of sequenceMark
		                if (CKEDITOR.env.safari) {
		                    editor.getSelection().getSelectedElement();
		                } else {
		                    editor.getSelection().selectElement(editor.document.getActive().getParent());
		                }
		                //The status to editor know this is update
		                isEditSequenceOrder = true;
		                editor.openDialog('insertSequenceOrder', function () {
		                    currentSequenceOrderResId = loadDataSequenceOrder(ele);
		                });
		            });
		            //Hide button on toolbar after add sequence incase qtItem is 36
		            if (iSchemeID == "36") {
		                $(".cke_button__sequenceorder").parents("span.cke_toolbar").hide();
		            }

		            eleSequenceOrder = '';
		            isEditSequenceOrder = false;
		            currentSequenceOrderResId = '';
                    newResult = iResult;

                    if (isAlgorithmicGrading) {
                        TestMakerComponent.isShowAlgorithmicConfiguration = true;
                    } else {
                        TestMakerComponent.isShowAlgorithmicConfiguration = false;
                    }

		        },
		        onCancel: function () {
		            eleSequenceOrder = '';
		            isEditSequenceOrder = false;
		            currentSequenceOrderResId = '';
		        }
		    };


		});
	}
});
//load item sequence order drap and drop
function loadDataSequenceOrder(ele) {
    var sourceListSortable = $('#source_list_sortable');
    var sourceCorrectListSortable = $('#source_correct_list_sortable');
    var messageSourceItem = $('.message_source_item');
    var messageSourceItemCorrect = $('.message_source_item_correct');
    var ckVertical = $('#_vertical');
    var ckHorizontal = $('#_horizontal');
    var pointSequenceOrder = $('#pointSequenceOrder');
    var source_sortable = $('.source_sortable:visible');
    var correct_answer = $('.correct_answer:visible');
    var pointValue = '';
    var htmlStringSource = '';
    var htmlStringCorrect = '';
    var arrCorrectAnswer = [];
    var typeOrientation = '';
    var eleSequence = '';
    var itemCurrent = '';
    var tagSequence = '';
    var tagsSpan = '';
    var idCurr = "";

    var tagBodyEditor = $('iframe[allowtransparency]').contents().find('body');
    sourceListSortable.empty();
    sourceCorrectListSortable.empty();

    if (ele.getId == undefined) {
        idCurr = $(ele).attr('id');
    } else {
        idCurr = ele.getId();
    }

    for (var i = 0, leIresult = iResult.length; i < leIresult; i++) {
        var itemResult = iResult[i];
        if (itemResult.responseIdentifier === idCurr && itemResult.type === 'sequence') {

            typeOrientation = itemResult.orientation;
            arrCorrectAnswer = itemResult.correctResponse.split(',');
            itemCurrent = tagBodyEditor.find('div[orientation]#' + idCurr);
            tagSequence = itemCurrent.find('.sequence');
            tagsSpan = tagSequence.find('span');
            pointValue = itemResult.responseDeclaration.pointsValue;
            widthItem = itemResult.widthItem;

            switch (typeOrientation) {
                case 'vertical':
                    tagsSpan.each(function () {
                        var id = $(this).attr('identifier');
                        var valueContent = $(this).text();
                        var wItem = $(this).width();
                        var newItem = new SourceItem(id, widthItem, "auto", 'vertical', valueContent);
                        var htmlItem = newItem.render();
                        htmlStringSource += htmlItem;
                    });
                    for (var j = 0, lenarrCorrectAnswer = arrCorrectAnswer.length; j < lenarrCorrectAnswer; j++) {
                        var idItem = arrCorrectAnswer[j];
                        var valueContentCorrect = tagSequence.find('span[identifier="' + idItem + '"]').text();
                        var wItemCorrect = tagSequence.find('span[identifier="' + idItem + '"]').width();
                        var newItemCorrect = new SourceItem(idItem, widthItem, "auto", 'vertical', valueContentCorrect);
                        var htmlItemCorrect = newItemCorrect.render();
                        htmlStringCorrect += htmlItemCorrect;
                    }

                    ckHorizontal.prop('checked', false);
                    ckVertical.prop('checked', true);
                    source_sortable.css({
                        'width': '49%',
                        'float': 'left',
                        'margin-right': '5px'
                    });

                    correct_answer.css({
                        'width': '49%',
                        'float': 'left',
                    });
                    $('.source_list_sortable').attr('orientation', 'vertical');
                    $('.source_correct_list_sortable').attr('orientation', 'vertical');
                    break;
                case 'horizontal':
                    tagsSpan.each(function () {
                        var id = $(this).attr('identifier');
                        var valueContent = $(this).text();
                        var newItem = new SourceItem(id, widthItem, "auto", 'horizontal', valueContent);
                        var htmlItem = newItem.render();
                        htmlStringSource += htmlItem;
                    });

                    for (var j = 0, lenarrCorrectAnswer = arrCorrectAnswer.length; j < lenarrCorrectAnswer; j++) {
                        var idItem = arrCorrectAnswer[j];
                        var valueContentCorrect = tagSequence.find('span[identifier="' + idItem + '"]').text();
                        var newItemCorrect = new SourceItem(idItem, widthItem, "auto", 'horizontal', valueContentCorrect);
                        var htmlItemCorrect = newItemCorrect.render();
                        htmlStringCorrect += htmlItemCorrect;
                    }

                    ckHorizontal.prop('checked', true);
                    ckVertical.prop('checked', false);
                    $('.source_list_sortable').attr('orientation', 'horizontal');
                    $('.source_correct_list_sortable').attr('orientation', 'horizontal');
                    break;
            }
            messageSourceItem.hide();
            messageSourceItemCorrect.hide();
            pointSequenceOrder.val(pointValue);
            sourceListSortable.html(htmlStringSource).show();
            sourceCorrectListSortable.html(htmlStringCorrect).show();

            //Loading width and height of item
            if (widthItem == undefined) {
                widthItem = 100;
            }
            $('#itemSequenceWidth').val(widthItem);
        }
    }

    return idCurr;
}
//show pop up Source Sortable
function showSourceSortable(idTagLi, valueString, typeOrientation, wDefault) {

    $(window).keydown(function (event) {
        if (event.keyCode === 9) {
            return false;
        }
    });

    var $popupTableHotspot = $('#popupSourceItem');
    var $popupTableHotspotOverlay = $('#popupSourceItemOverlay');
    var wBodyContentPopup = $('.cke_dialog_contents_body');
    var sourceSortable = $('.source_sortable');
    var sourceCorrect = $('.correct_answer');
    var arrWidthItems = [];
    var arrLenCharacter = [];
    var maxWidth = '';
    var html = '';
    var highest = 0;
    var idItem = '';
    var isVertical = false;

    html += '<div class="cke_dialog_body cke_dialog_image_hotspot"><div class="cke_dialog_title">Source Sortable</div>';
    html += '<a type="image" title="Remove" class="cke_dialog_close_button" id="btnIClose"><span class="cke_label">X</span></a>';
    html += '<div class="hotspot-list tablehotspot">';
    html += '<div class="content_source_sortable">';
    html += '<p>Value</p>';
    html += '<p><input type="text" class="txtItemSource" id="itemSource" value="" /></p>';
    html += '<span class="errorMessage" id="errorMessage">Value source item cannot be empty</span>';
    html += '</div>';
    html += '</div>';
    html += '<div class="cke_dialog_footer">';
    html += '<div class="cke_dialog_ui_hbox cke_dialog_footer_buttons">';
    html += '<div class="cke_dialog_ui_hbox_first" role="presentation"><a title="YES" id="btnOk" class="cke_dialog_ui_button cke_dialog_ui_button_ok" role="button" type="hotpot"><span class="cke_dialog_ui_button">OK</span></a></div>';
    html += '<div class="cke_dialog_ui_hbox_last" role="presentation"><a title="NO" id="btnCancel" class="cke_dialog_ui_button cke_dialog_ui_button_cancel" role="button"><span class="cke_dialog_ui_button">Cancel</span></a></div>';
    html += '</div>';
    html += '</div>';

    $popupTableHotspot
        .html(html)
        .show()
        .draggable({
            cursor: 'move',
            handle: '.cke_dialog_title'
        });


    var txtValueString = $("<div></div>").html(valueString).text();

    $("#itemSource").val(txtValueString).focus();

    $popupTableHotspotOverlay.show();

    $popupTableHotspot.on('click', '#btnCancel, #btnIClose', function () {
        $popupTableHotspot.empty().hide();
        $popupTableHotspotOverlay.hide();
        $('#errorMessage').show();

        $(window).keydown(function (event) {
            if (event.keyCode === 9) {

            }
        });
    });

    $('#btnOk').click(function () {
        var newValue = $('#itemSource').val().trim();
        if (newValue.trim() === '') {
            $('#errorMessage').text("Value Source item cannot be empty.").show();
            return false;
        }

        var sameValue = false;
        //Check if the value of source item has same value
        $("#source_correct_list_sortable li").each(function (index) {
            var identifier = $(this).attr('id');
            var valueContent = $(this).find('.content').text().trim();

            if (idTagLi != identifier)
            {
                if (valueContent == newValue) {

                    sameValue = true;
                    return false;
                }
            }
        });

        if (sameValue) {
            $('#errorMessage').text("Value Source item has already existed.").show();
            return false;
        }

        switch (typeOrientation) {
            case 'vertical':
                isVertical = true;
                break;
        }
        newValue = $("<div></div>").text(newValue).html();
        $('#source_list_sortable').find('li#' + idTagLi + ' .content').html(newValue);
        $('#source_correct_list_sortable').find('li#' + idTagLi + ' .content').html(newValue);

        //push width item into array
        $('#source_list_sortable').find('li').each(function() {
            var wItem = $(this).outerWidth();
            var idItem = $(this).attr('id');
            var lenItemCharacter = $(this).find('.content').text();

            arrWidthItems.push(wItem);
            arrLenCharacter.push({
                lengthChar: lenItemCharacter.length,
                id: idItem
            });
        });

        maxWidth = Math.max.apply(null, arrWidthItems);
        $.each(arrLenCharacter, function(key, item) {

            if (item.lengthChar > highest) {
                highest = item.lengthChar;
                idItem = item.id;
            }
        });

        valueString = '';
        isVertical = false;
        $popupTableHotspot.empty().hide();
        $popupTableHotspotOverlay.hide();
        $('#errorMessage').show();

        $(window).keydown(function (event) {
            if (event.keyCode === 9) {

            }
        });
    });

    $('.txtItemSource:visible').on('keydown', function (e) {
        if (e.which === 13 || e.which === 9) {
            return false;
        }
    });
}
//reset content source item
function resetSourceItem() {
    $('.txtItemSource').val('');
    $('.errorMessage').text('').hide();
}
//reset content sequence order
function resetSequenceOrder() {
    var isVertical = $('#_vertical');
    $('#_vertical').prop('checked', true);
    $('#_horizontal').prop('checked', false);

    $('#outOfRange').val('3');
    $('#pointSequenceOrder').val('1');

    $('.message_source_item').show();
    $('.message_source_item_correct').show();

    $('.source_list_sortable').empty().hide();
    $('.source_correct_list_sortable').empty().hide();

    $('#messageOurOfRange').text('').hide();
    $('#warningMessage').text('').hide();

    $('.source_sortable').css({
        'width': '49%',
        'float': 'left',
        'margin-right': '5px'
    });
    $('.correct_answer').css({
        'width': '49%',
        'float': 'left',
    });
    if (isVertical.is(':checked')) {
        $('.source_list_sortable').attr('orientation', 'vertical');
        $('.source_correct_list_sortable').attr('orientation', 'vertical');
    }
}
function resetEditSequenceOrder() {
    $('#messageOurOfRange').text('').hide();
    $('#warningMessage').text('').hide();
}
//create id source item
function createIdSourceItem(element, elementAttr, prefix) {
    var $element = $(element);
    var len = $element.length;
    var srcId = prefix + (len + 1);

    for (var m = 0; m < len; m++) {
        var resId = prefix + (m + 1);
        var isOnlyOne = true;
        var elM = $element.eq(m).attr(elementAttr);

        if (elM != resId) {
            for (var k = 0; k < len; k++) {
                var elK = $element.eq(k).attr(elementAttr);
                if (resId == elK) {
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
//handle events when add new item into source
var isSortableCalled = false;
function interactiveUserGraphicInterface() {
    var btnRemoveItem = $('#source_list_sortable .btnRemoveItem:visible');
    var source_list_sortable = $('#source_list_sortable');
    var source_correct_list_sortable = $('#source_correct_list_sortable');
    var message_source_item = $('.message_source_item');
    var message_source_item_correct = $('.message_source_item_correct');
    var source_item_type = $('#source_list_sortable .source_item_type:visible');

    btnRemoveItem.click(function (evt) {
        var idSource = $(evt.target).parent().attr('id');
        $(evt.target).parent().remove();
        if (source_correct_list_sortable.find('li').length) {
            source_correct_list_sortable.find('li#' + idSource).remove();
        }
        if (source_list_sortable.find('li').length === 0) {
            message_source_item.show();
            message_source_item_correct.show();
        }
        return false;
    });

    source_item_type.unbind('click').click(function (evt) {
        var valueString = '';
        var idTagLi = '';
        var typeOrientation = '';
        var contentSequenceOrder = $(evt.target).parents('.contentSequenceOrder');
        var ckvertical = contentSequenceOrder.find('#_vertical');
        var wDefault = 245;
        typeOrientation = ckvertical.is(':checked') ? 'vertical' : 'horizontal';

        if (evt.target.nodeName === 'LI') {
            idTagLi = $(evt.target).attr('id');
            valueString = $(evt.target).find('.content').html();
        }

        if (evt.target.nodeName === 'SPAN' && evt.target.className === 'content') {
            idTagLi = $(evt.target).parent().attr('id');
            valueString = $(evt.target).html();
        }

        if (navigator.userAgent.indexOf('Firefox') > -1) {
            if (!isSortableCalled) {
                showSourceSortable(idTagLi, valueString, typeOrientation, wDefault);
            }
            isSortableCalled = false;
        } else {
            showSourceSortable(idTagLi, valueString, typeOrientation, wDefault);
        }


        return false;
    });

    $('#source_list_sortable').sortable({
        opacity: 0.8,
        start: function (event, ui) {

        },
        stop: function (event, ui) {
            if (navigator.userAgent.indexOf('Firefox') > -1) {
                isSortableCalled = true;
            }
        }
    }).disableSelection();

    $('#source_correct_list_sortable').sortable({
        opacity: 0.8,
    }).disableSelection();

}
function buildHtmlSequence(responseId, typeOrientation, sourceItem, wFrameVertical, wItem, hItem, isEditSequenceOrder) {
    var htmlstring = '';
    htmlstring += '<div contenteditable="false" class="sequenceBlock" id="' + responseId + '" orientation="' + typeOrientation + '">';

    htmlstring += '<button id="single-click" class="single-click">Click here to edit Sequence</button>';
    htmlstring += '<img src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" data-cke-saved-src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" class="cke_reset cke_widget_mask sequenceMark">';
    htmlstring += '<span class="sequence">';
    if (sourceItem.length) {
        for (var i = 0, leSourceItem = sourceItem.length; i < leSourceItem; i++) {
            htmlstring += '<span contenteditable="false" tabindex="-1" style="width: ' + wItem + 'px;" class="sequenceItem" identifier="' + sourceItem[i].identifier + '">' + sourceItem[i].value + '</span>';
        }
    }
    htmlstring += '</span></div>';

    return htmlstring;
}
/***
* build style table hot spot
***/
function SourceItem(id, wItem, hItem, typeSource, valueItem) {
    this.identifier = id;
    this.wItem = wItem;
    this.hItem = hItem;
    this.typeSource = typeSource;
    this.valueItem = valueItem;
}

SourceItem.prototype.render = function () {
    var html = '';
    switch (this.typeSource) {
        case 'vertical':
            html += '<li style="display: block; width:' + this.wItem + 'px;" class="source_item_type"  id="' + this.identifier + '">';
            break;
        case 'horizontal':
            html += '<li style="width:' + (this.wItem) + 'px;" class="source_item_type"  id="' + this.identifier + '">';
            break;
        }
    var valueI = $("<div></div>").text(this.valueItem).html()
        html += '<span class="content">' + valueI + '</span>';
        html += '<span class="btnRemoveItem remove" title="Remove">x</span>';
        html += '</li>';

    return html;
};
