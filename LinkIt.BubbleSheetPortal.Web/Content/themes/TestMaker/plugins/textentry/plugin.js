function cleanFillInBlankAnswerXml(xmlString) {
    var $xml = $('<div/>').append(xmlString);
    $xml.contents().each(function () {
        if (this.nodeType === 8) {
            $(this).remove();
        }
    });
    return $xml.html();
}

CKEDITOR.plugins.add('textentry', {
    lang: 'en', // %REMOVE_LINE_CORE%
    icons: 'textentry',
    requires: 'dialog',
    hidpi: true, // %REMOVE_LINE_CORE%
    init: function (editor) {
        var pluginName = 'insertTextEntry';

        editor.addCommand(pluginName, new CKEDITOR.dialogCommand(pluginName));

        editor.ui.addButton('Textentry', {
		    label: 'Text Entry',
		    command: pluginName,
		    icon: this.path + 'icons/textentry.png',
		    toolbar: 'insertTextEntry,30'
        });
        editor.widgets.add('textentry', {
            inline: true,
            mask: true,
            allowedContent: { span: {styles: 'maxWidth',classes: 'textEntryInteraction', attributes: '!id,name,contenteditable,styles' } },
            template: '<span class="textEntryInteraction"></span>',
        });
        var isEditTextEntry = false,
            currentResponseId = "",
            isAddEntryAnswerCorrect = false,
            eleTextEntry = ""; //This use for firstime load popup
        var isOk = false;
        editor.on('doubleclick', function (evt) {
            var element = evt.data.element;

            if (element.hasClass('textEntryInteractionMark')) {
                var parents = element.getParents();
                var parent;

                for (var i = 0; i < parents.length; i++) {
                    parent = parents[i];
                    if (parent.hasClass('textEntryInteraction')) {
                        break;
                    }
                }

                $('#entryTop').html('');
                //Move selection to parent of multipleChoiceMark
                eleTextEntry = parent;
                editor.getSelection().selectElement(eleTextEntry);
                evt.data.dialog = pluginName;

                //The status to editor know this is update
                isEditTextEntry = true;
                isEditMultipChoiceGuidance = true;

                currentResponseId = loadDataforTextEntry(eleTextEntry);

                dblickHandlerToolbar(editor);
            }
        });

        CKEDITOR.dialog.add(pluginName, function (editor) {
            myhtml = '';
            myhtml += '<div class="dialog-list dialog-list-textentry">';
            myhtml += '    <div class="dialog-list-item"><input type="radio" value="default" name="textentrygrade" id="textentrygrade-normal" checked/> <label for="textentrygrade-normal">Normal Grading</label></div>';
            myhtml += '    <div class="dialog-list-item"><input type="radio" value="manual" name="textentrygrade" id="textentrygrade-manual" /> <label for="textentrygrade-manual">Grade Manually</label></div>';
            myhtml += '    <div class="dialog-list-item"><input type="radio" value="ungraded" name="textentrygrade" id="textentrygrade-ungraded" /> <label for="textentrygrade-ungraded">Ungraded</label></div>';
            myhtml += '    <div class="dialog-list-item"><input type="radio" value="algorithmic" name="textentrygrade" id="textentrygrade-algorithmic" /> <label for="textentrygrade-algorithmic">Algorithmic Grading</label></div>';
            myhtml += '    <div class="point-value" style="display: inline-block;">Point value: <input type="text" id="point" class="point" value="1" /></div>';
            myhtml += '    <div class="textEntryPoint" style="display: none">';
            myhtml += '        <div>Point value: <input type="text" id="pointRange" class="point" value="1" /></div>';
            myhtml += '    </div>';
            myhtml += '</div>';
            myhtml += '<div class="divTextEntryType u-clearfix u-m-t-10">';
            myhtml += '     <div class="rangeTextEntry">';
            myhtml += '         <div class="dialog-list">';
            myhtml += '             <div class="dialog-list-item u-m-r-29"><input type="radio" id="listTextEntry" name="answerTextEntry" checked/> <label for="listTextEntry">Answer List</lable></div>';
            myhtml += '             <div class="dialog-list-item"><input type="radio" id="rangeTextEntry" name="answerTextEntry"/> <label for="rangeTextEntry">Answer Range</lable></div>';
            myhtml += '         </div>';
            myhtml += '     </div>';
            myhtml += '</div>';
            myhtml += '<div class="boxDiv">Expected length(Characters) Min: <input type="text" id="expLengthMin" class="expLength"  />';
            myhtml +='         Max:<input type="text" id="expLengthMax" class="expLength" />';
            myhtml +='         <label><input type="checkbox" checked name="addPadding" value ="1" id="checkBoxAddLength"> Add 5 padding characters</label>';
            myhtml += '         </div>';
            myhtml += '        <div class="box-width-fib">Visible Dimension: <input type="text" id="expWidthFIB" value="30" class="expLength"  /> </div>';
            myhtml +='         <div class="block-select"><label>Validation: <select id="option-vaidation">';
            myhtml +='         <option value="0">None</option>';
            myhtml +='         <option value="1">Letters Only (A-Z) </option>';
            myhtml +='         <option value="2">Numerals Only (0-9)</option>';
            myhtml +='         <option value="5">Numbers (incl. negatives and decimals)</option>';
            myhtml +='         <option value="3">Alphanumeric Only (A-Z, 0-9) </option>';
            myhtml +='         <option value="4">Custom...</option>';
            myhtml +='         </select></label> <select id="custom-vaidation">';
            myhtml +='         <option value="1">Time (hh:mm) </option>';
            myhtml +='         <option value="2">Centimeter (### cm)</option>';
            myhtml +='         </select>';
            myhtml +='</div>';
            myhtml += '<div class="textEntry">';
            myhtml += '    <div class="textEntryPoint">';
            myhtml += '        <div class="boxDiv"><label><input type="checkbox" id="caseSensitiveCheck" /> Case sensitive check</lable></div>';
            myhtml += '        <div class="clear10"></div>  ';
            myhtml += '        <div class="boxDiv"><label><input type="checkbox" id="misSpelling" /> Misspelling check</lable></div>';
            myhtml += '        <div class="boxDiv">Misspelling deduction: <input type="text" id="misspelling-deduction" class="misspelling" value="1" /></div>';
            myhtml += '        <div class="clear10"></div>  ';
            myhtml += '        <div class="boxDiv"><label><input type="checkbox" id="ignoreExtraSpace" /> Ignore Extra Spaces in Student Answers</lable></div>';
            myhtml += '        <div class="clear10"></div>  ';
            myhtml += '    </div>';
            myhtml += '    <div class="clear10"></div>  ';
            myhtml += '    <div class="textEntryList">';
            myhtml += '        <h2>Correct Answers</h2><i class="smalltext">(The first item will be treated as the preferred answer.)</i>';
            myhtml += '        <div class="clear10"></div>  ';
            myhtml += '        <div id="entryTop"></div>';
            myhtml += '        <div class="clear"></div>';
            myhtml += '        <ul id="textEntryList"></ul>'; //<li><span>Answer A</span></li><li><span>Answer B</span></li>
            myhtml += '         <div class="clear10"></div>';
            myhtml += '         <div class="addMore">';
            myhtml += '         	<input type="button" class="ckbutton" id="bntAddEntryChoice" value="Add entry" />';
            myhtml += '         </div>';
            myhtml += '         <div class="clear"></div>';
            myhtml += '         <div id="entryBot"></div>';
            myhtml += '         <div class="clear"></div>';
            myhtml += '    </div>';
            myhtml += '</div>';
            myhtml += '<div class="textEntryRange">';
            myhtml += '    <div class="textEntryList">';
            myhtml += '        <div class="clear10"></div>';
            myhtml += '        <div class="divTextEntryTypeBox">';
            myhtml += '            <div class="decimalBox">';
            myhtml += '                 <div class="divStart"><span class="decimalTitle"><input type="checkbox" id="startValue" relId="divStart" checked="checked" /> <label for="startValue">Start value</lable></span><div class="clear10"></div>';
            myhtml += '                 <div id="divStart">';
            myhtml += '                 <div class="divTextEntryType">';
            myhtml += '                     <div class="decimal"><input type="radio" id="decimalTextEntry" name="textentrytype" checked="checked"/> <label for="decimalTextEntry">Decimal</lable></div>';
            myhtml += '                     <div class="fraction"><input type="radio" id="fractionTextEntry" name="textentrytype" /> <label for="fractionTextEntry">Fraction</lable></div>';
            myhtml += '                     <div class="integer"><input type="radio" id="integerTextEntry" name="textentrytype" /> <label for="integerTextEntry">Integer</lable></div>';
            myhtml += '                 </div>';
            myhtml += '                 <div class="clear5"></div>';
            myhtml += '                 <input type="textbox" id="decimalStart" /> <span class="startDecimalType">.</span> <input type="textbox" id="decimalStart1" />';
            myhtml += '                 <input type="checkbox" id="startExclusivity" /> <label for="startExclusivity">Exclusive</lable></div>';
            myhtml += '                 </div>';
            myhtml += '                 <div class="clear10"></div>';
            myhtml += '                 <div class="divEnd"><span class="decimalTitle"><input type="checkbox" id="endValue" relId="divEnd" /> <label for="endValue">End value</lable></span><div class="clear10"></div>';
            myhtml += '                 <div id="divEnd" style="display: none;">';
            myhtml += '                 <div class="divTextEntryType">';
            myhtml += '                     <div class="decimal"><input type="radio" id="endDecimalTextEntry" name="endtextentrytype" checked="checked"/> <label for="endDecimalTextEntry">Decimal</lable></div>';
            myhtml += '                     <div class="fraction"><input type="radio" id="endFractionTextEntry" name="endtextentrytype" /> <label for="endFractionTextEntry">Fraction</lable></div>';
            myhtml += '                     <div class="integer"><input type="radio" id="endIntegerTextEntry" name="endtextentrytype" /> <label for="endIntegerTextEntry">Integer</lable></div>';
            myhtml += '                 </div>';
            myhtml += '                 <div class="clear5"></div>';
            myhtml += '                 <input type="textbox" id="decimalEnd" /> <span class="endDecimalType">.</span> <input type="textbox" id="decimalEnd1" />';
            myhtml += '                 <input type="checkbox" id="endExclusivity" /> <label for="endExclusivity">Exclusive</lable></div>';
            myhtml += '                 </div>';
            myhtml += '            </div>';
            myhtml += '        </div>';
            myhtml += '    </div>';
            myhtml += '</div>';

            return {
                title: 'Text Entry Properties',
                minWidth: 560,
                minHeight: 200,
                contents:
		        [
			        {
			            id: 'textEntry',
			            label: 'Settings',
			            elements:
				        [
					        {
					            type: 'html',
					            html: myhtml,
					            onLoad: function() {
                          onLoadTextEntry();
					                textEntrySelection();
					                $("#textEntryList li[class='active'] span").empty().text($("#textEntryInput").val());

					                //ad new item answer correct
					                $("#bntAddEntryChoice").on('click', function () {
					                    isAddEntryAnswerCorrect = true;
					                    isAddAnswerTextEntryLast = false;
					                    isEditMultipChoiceGuidance = false;
					                    var editorid = addNewItemTextEntry();
					                    if (editorid === undefined || editorid === "") {
					                        return;
					                    }
					                    textEntrySelection();
					                    createCKEditorEntryList(editorid);
					                });

					                $("#bntTextEntryRemove").click(function () {
					                    $("#textEntryList li[class='active']").remove();
					                });

					                $("#misSpelling").click(function () {
					                    if ($(this).is(':checked')) {
					                        $(".misspelling, .ckUpNum_misspelling, .ckDownNum_misspelling").prop('disabled', false).removeClass("disable");
					                    } else {
					                        $(".misspelling, .ckUpNum_misspelling, .ckDownNum_misspelling").prop('disabled', true).addClass("disable");
					                    }
					                });

					                $("#decimalTextEntry").click(function () {
					                    $(".startDecimalType").empty().html(".");
					                    $(".startDecimalType, #decimalStart1").show();
					                });

					                $("#fractionTextEntry").click(function () {
					                    $(".startDecimalType").empty().html("/");
					                    $(".startDecimalType, #decimalStart1").show();
					                });

					                $("#integerTextEntry").click(function () {
					                    $(".startDecimalType, #decimalStart1").hide();
					                });

					                $("#endDecimalTextEntry").click(function () {
					                    $(".endDecimalType").empty().html(".");
					                    $(".endDecimalType, #decimalEnd1").show();
					                });

					                $("#endFractionTextEntry").click(function () {
					                    $(".endDecimalType").empty().html("/");
					                    $(".endDecimalType, #decimalEnd1").show();
					                });

					                $("#endIntegerTextEntry").click(function () {
					                    $(".endDecimalType, #decimalEnd1").hide();
					                });

					                $("#rangeTextEntry").on("click", function () {
					                    $(".textEntryRange").show();
                              $(".textEntry").hide();
                              $('.dialog-list-textentry #point').parents('.point-value').hide();
                              $('.dialog-list-textentry #pointRange').parents('.textEntryPoint').css('display', 'inline-block')
					                });

					                $("#listTextEntry").on("click", function () {
					                    $(".textEntryRange").hide();
                              $(".textEntry").show();
                              $('.dialog-list-textentry #point').parents('.point-value').show()
                              $('.dialog-list-textentry #pointRange').parents('.textEntryPoint').hide()
					                });


					                $("#startValue, #endValue").click(function () {
					                    if ($(this).is(":checked")) {
					                        $("#" + $(this).attr("relId")).show();
					                    } else {
					                        $("#" + $(this).attr("relId")).hide();
					                    }
					                });

                                    handlerKeydownNegativeNumber('#decimalStart');
                                    handlerKeydownNegativeNumber('#decimalEnd');
                                    handlerKeydownPositionNumber('#decimalStart1');
                                    handlerKeydownPositionNumber('#decimalEnd1');
					            },
					            onShow: function () {
                                    var questionQtiSchemeId = CKEDITOR.instances[ckID].config.qtiSchemeID;
                                    var $dialog = $(this.getElement().$);
					                //hide tooltip
					                $('#tips .tool-tip-tips').css({
					                    'display': 'none'
					                });

                                    if (isEditTextEntry) {
                                        loadDataforTextEntry(eleTextEntry);
                                    } else {
                                        createTextEntryCorrect();
                                        resetTextEntryOnload();
                                    }

                                    refreshResponseId();
					                // set arrIdResp null
                                     checkElementRemoveIntoIResult();

                                    CKEDITOR.on('instanceReady', function (ev) {
                                        //Show the first toolbar when popup has created
                                        $("#entryTop > div").hide();
                                        $("#entryTop > div").last().show();

                                        $('ul#textEntryList li').each(function (index, value) {
                                            var that = this;
                                            var ind = index + 1;
                                            var ckId = $(that).attr('id');
                                            var data = CKEDITOR.instances['editor' + ckId].getData();

                                            $('#' + ckId).find('iframe[allowtransparency]').contents().find('body').on('keypress', function (e) {
                                                $(e.target).attr('isEnter', 'true');
                                            });

                                            $('#' + ckId).find('iframe[allowtransparency]').contents().find('body').on('focus', function () {
                                                var me = this;
                                                var ans = 'Answer ' + ind;
                                                var newAns = $(me).text();
                                                var newNumber = newAns.replace(/[^0-9]/g, '');
                                                var newString = newAns.replace(newNumber, '');

                                                if ($(me).text() === ans || newString.trim() === 'Answer Correct') {
                                                    $(me).attr('defaultText', newAns);
                                                    var isenter = $(me).attr('isenter');
                                                    if (isenter == 'true') {
                                                        return;
                                                    } else {
                                                        $(me).text('');
                                                    }
                                                }
                                            });
                                            $('#' + ckId).find('iframe[allowtransparency]').contents().find('body').on('blur', function () {
                                                var me = this;

                                                if (isOk === true) {

                                                    if ($(me).text() !== '') {
                                                        return 1;
                                                    } else {
                                                        $(me).text($('#' + ckId).find('iframe[allowtransparency]').contents().find('body').attr('defaulttext'));
                                                    }

                                                } else {
                                                    if ($(me).text() === '') {
                                                        $(me).text('Answer Correct ' + ind); // set text when no press Ok
                                                        $(me).text($('#' + ckId).find('iframe[allowtransparency]').contents().find('body').attr('defaulttext'));
                                                    }
                                                }

                                            });
                                        });

                                    });

					                //load item current
                                    if (isEditTextEntry) {
                                        //Only create CKEditor after html appended
                                        $("#textEntryList li").each(function () {
                                            createCKEditorEntryList("editor" + $(this).attr("id"));
                                        });

                                        for (var i = 0; i < iResult.length; i++) {
                                            if (iResult[i].responseIdentifier == currentResponseId &&
                                                iResult[i].type == "textEntryInteraction") {
                                                if (iResult[i].responseDeclaration.range == undefined)
                                                {
                                                    iResult[i].responseDeclaration.range = false;
                                                }

                                                if (iResult[i].responseDeclaration.range.toString() == "false") {
                                                    //Clear all item before add new
                                                    $("#point").val(iResult[i].responseDeclaration.pointsValue);
                                                    var arrEntry = iResult[i].correctResponse;
                                                    for (var m = 0; m < arrEntry.length; m++) {

                                                        //Load data to each item
                                                        var currentItem = $("#textEntryList li").eq(m);
                                                        CKEDITOR.instances["editor" + currentItem.attr("id")].setData(arrEntry[m].value);
                                                        //load point for answer
                                                        currentItem.find('.audio input[type=text]').val(arrEntry[m].pointsValue);

                                                        //apply data guidance has into iResult to iMessageTemp
                                                        if (isEditMultipChoiceGuidance) {
                                                            if (arrEntry[m].arrMessageGuidance.length) {
                                                                for (var k = 0, lensChoice = arrEntry[m].arrMessageGuidance.length; k < lensChoice; k++) {

                                                                    if (arrEntry[m].arrMessageGuidance[k].typeMessage === 'guidance') {
                                                                        currentItem.find('#selected_' + currentItem.attr('id')).attr('title', 'Guidance');
                                                                    }

                                                                    if (arrEntry[m].arrMessageGuidance[k].typeMessage === 'rationale') {
                                                                        currentItem.find('#selected_' + currentItem.attr('id')).attr('title', 'Rationale');
                                                                    }

                                                                    if (arrEntry[m].arrMessageGuidance.length === 2) {
                                                                        currentItem.find('#selected_' + currentItem.attr('id')).attr('title', 'Guidance and Rationale');
                                                                    }

                                                                    if (arrEntry[m].arrMessageGuidance[k].typeMessage === 'guidance_rationale') {
                                                                        currentItem.find('#selected_' + currentItem.attr('id')).attr('title', 'Guidance and Rationale');
                                                                    }
                                                                }

                                                                currentItem.find('#selected_' + currentItem.attr('id')).parent('.savedGuidance').show();
                                                                currentItem.find('#unselected_' + currentItem.attr('id')).hide();
                                                            }

                                                            iMessageTemp.push({
                                                                idTemp: currentItem.attr('id'),
                                                                arrMessage: arrEntry[m].arrMessageGuidance,
                                                            });

                                                        }
                                                    }
                                                }

												break;
                                            }
                                        }
                                    }
                                    //===end
                                    if (questionQtiSchemeId == 21) {
                                        $dialog.find('#textentrygrade-algorithmic').parent('.dialog-list-item').hide();
                                    }
					            }
					        }
						]
			        }
		        ],
                onOk: function () {
                    isAddEntryAnswerCorrect = false;
                    //This is build correctResponse
                    var points = 0;
                    var isManual = "default";
                    var textEntryList = [], arrayPoint = [],
                            htmlEntry = "",
                            correctEntryResponse = "",
                            currentAnswerIndex = 0, // Store correct alphebet for question
                            alphaBe = ['A', 'B', 'C', 'D', 'E', 'F', 'G', 'H'],
                            maxPointStr = '',
                            maxPoint = '';
                    var responseId = "";
                    var hastypeMessageGuidance = '';
                    var msg = '';

                    isRange = false;
                    if ($("#rangeTextEntry").is(":checked")) {
                        isRange = true;
                    }

                  isManual = $('input[type="radio"][name="textentrygrade"]:checked').val();
                  if (isManual == "manual" && modeMultiPartGrading == "all-or-nothing-grading") {
                    customAlert('Manual grading is not compatible with All or Nothing grading. Please select a different grading method.', { contentStyle: { maxWidth: 650 } });
                    return false
                  }
                    //Build Inline choice
                    if (isRange) {

                        //Validate incase user doesn't choice start or end
                        if ($("#startValue").is(':checked') == false && $("#endValue").is(':checked') == false) {
                            msg = 'Please select start value or end value.';
                            customAlert(msg);
                            return false;
                        }

                        if ($("#startValue").is(':checked') == true) {

                            //Check case for integer
                            if ($("#integerTextEntry").is(':checked')) {
                                if ($("#decimalStart").val() == "")
                                {
                                    msg = 'Please enter number for start value.';
                                    customAlert(msg);
                                    return false;
                                }
                            } else {
                                if ($("#decimalStart").val() == "" || $("#decimalStart1").val() == "") {
                                    msg = 'Please enter number for start value.';
                                    customAlert(msg);
                                    return false;
                                }
                            }


							if($(".startDecimalType").text() == "/")
							{
								if($("#decimalStart1").val() != "" && parseInt($("#decimalStart1").val()) == 0)
								{
									msg = 'Please insert valid fraction.';
                                    customAlert(msg);
									return false;
								}
							}

                        }

                        if ($("#endValue").is(':checked') == true) {
                            //Check case for integer
                            if ($("#endIntegerTextEntry").is(':checked')) {
                                if ($("#decimalEnd").val() == "") {
                                    msg = 'Please enter number for start value.';
                                    customAlert(msg);
                                    return false;
                                }
                            } else {
                                if ($("#decimalEnd").val() == "" || $("#decimalEnd1").val() == "") {
                                    msg = 'Please enter number for end value.';
                                    customAlert(msg);
                                    return false;
                                }
                            }


                            if ($(".endDecimalType").text() == "/")
							{
								if($("#decimalEnd1").val() != "" && parseInt($("#decimalEnd1").val()) == 0)
								{
									msg = 'Please insert valid fraction.';
                                    customAlert(msg);
									return false;
								}
							}
                        }

                        //This is for range list
                        points = $('.dialog-list-textentry').find('#pointRange').val();

                        valueType = 0;
                        if ($("#fractionTextEntry").is(":checked")) {
                            valueType = 1;
                        }

                        endValueType = 0;
                        if ($("#endFractionTextEntry").is(":checked")) {
                            endValueType = 1;
                        }

                        startExclusivity = 0;
                        if ($("#startExclusivity").is(":checked")) {
                            startExclusivity = 1;
                        }

                        endExclusivity = 0;
                        if ($("#endExclusivity").is(":checked")) {
                            endExclusivity = 1;
                        }

                        startValue = "";
                        endValue = "";

                        var dS = $("#decimalStart").val();
                        var dS1 = $("#decimalStart1").val();
                        var dE = $("#decimalEnd").val();
                        var dE1 = $("#decimalEnd1").val();

                        if ($("#integerTextEntry").is(":checked")) {
                            dS1 = "";
                        }

                        if ($("#endIntegerTextEntry").is(":checked")) {
                            dE1 = "";
                        }

                        if (dS != "" || dS1 != "") {
                            var isDsNegativeZero = dS.indexOf('-') === 0 && parseInt(dS, 10) === 0;
                            var strDs = parseInt(dS).toString();
                            var strDs1 = parseInt(dS1).toString();
                            var strMath = $(".startDecimalType").text();

                            if (dS === '-') {
                                msg = 'Please enter valid number for start value.';
                                customAlert(msg);
                                return false;
                            }

                            if ($("#integerTextEntry").is(":checked")) {
                                startValue = dS;
                            } else if ($("#fractionTextEntry").is(":checked")) {
                                startValue = strDs + strMath + strDs1;
                            } else {
                                startValue = strDs + strMath + dS1;
                            }

                            if (isDsNegativeZero && !$('#integerTextEntry').is(':checked')) {
                                startValue = '-' + startValue;
                            }
                        }

                        if (dE != "" || dE1 != "") {
                            var isDeNegativeZero = dE.indexOf('-') === 0 && parseInt(dE, 10) === 0;
                            var strDe = parseInt(dE).toString();
                            var strDe1 = parseInt(dE1).toString();
                            var strMath = $(".endDecimalType").text();

                            if (dE === '-') {
                                msg = 'Please enter valid number for end value.';
                                customAlert(msg);
                                return false;
                            }

                            if ($("#endIntegerTextEntry").is(":checked")) {
                                endValue = strDe;
                            } else if ($("#endFractionTextEntry").is(":checked")) {
                                endValue = strDe + strMath + strDe1;
                            } else {
                                endValue = strDe + strMath + dE1;
                            }

                            if (isDeNegativeZero && !$('#endIntegerTextEntry').is(':checked')) {
                                endValue = '-' + endValue;
                            }
                        }

                        if ($("#startValue").is(':checked') == false) {
                            startValue = "";
                            startExclusivity = 0;
                            valueType = 0;
                        }

                        if ($("#endValue").is(':checked') == false)
                        {
                            endValue = "";
                            endExclusivity = 0;
                            endValueType = 0;
                        }

                        //Check start value must less than end value
                        if (startValue != "" && endValue != "")
                        {
                            var startFloat = startValue;
                            if(valueType == 1)
                            {
                                startFloat = eval(startFloat).toString();
                            }

                            var endFloat = endValue;
                            if (endValueType == 1) {
                                endFloat = eval(endFloat).toString();
                            }

                            if (parseFloat(startFloat) >= parseFloat(endFloat)) {
                                msg = 'Start value must be less than End value';
                                customAlert(msg);
                                return false;
                            }
                        }

                        textEntryList.push({ name: "start", valueType: valueType, valueRange: startValue, exclusivity: startExclusivity }, { name: "end", valueType: endValueType, valueRange: endValue, exclusivity: endExclusivity });
                    } else {

                        var typeGuidance = '';
                        var typeRationale = '';
                        var typeGuidanceRationale = '';

                        if (CKEDITOR.instances[ckID].config.alphaBeta != undefined) {
                            alphaBe = CKEDITOR.instances[ckID].config.alphaBeta;
                        }

                        $('#textEntryList li').each(function (index) {
                            var iddentify = alphaBe[currentAnswerIndex],
                                editorId = $(this).find(".content textarea").attr("id"),
                                point = $(this).find('.point').val();

                            //add data guidance and rationale into simplechocie
                            var idTagLi = $(this).attr('id');
                            var arrMessageGuidance = [];

                            if ($(this).find("#selected_" + idTagLi).is(':visible')) {

                                if (iMessageTempEdit.length) {
                                    for (var j = 0, leArrMessageTempEdit = iMessageTempEdit.length; j < leArrMessageTempEdit; j++) {
                                        var itemMessageTempEdit = iMessageTempEdit[j];
                                        for (var k = 0, lenArrMessage = iMessageTemp.length; k < lenArrMessage; k++) {
                                            if (itemMessageTempEdit.idTemp === iMessageTemp[k].idTemp) {
                                                iMessageTemp[k].arrMessage = itemMessageTempEdit.arrMessage;
                                            }
                                        }
                                    }
                                }


                                for (var i = 0, lenArrMessage = iMessageTemp.length; i < lenArrMessage; i++) {
                                    if (iMessageTemp[i].idTemp === idTagLi) {
                                        arrMessageGuidance = iMessageTemp[i].arrMessage;
                                        break;
                                    }
                                }

                                hastypeMessageGuidance = 'hasGuidance';

                                for (var k = 0, lenArrMessageGuidance = arrMessageGuidance.length; k < lenArrMessageGuidance; k++) {
                                    var itemMessage = arrMessageGuidance[k];

                                    if (itemMessage.typeMessage === "guidance") {
                                        typeGuidance = includeHtmlGuidance(itemMessage, "guidance");
                                    }
                                    if (itemMessage.typeMessage === "rationale") {
                                        typeRationale = includeHtmlGuidance(itemMessage, "rationale");
                                    }
                                    if (itemMessage.typeMessage === "guidance_rationale") {
                                        typeGuidanceRationale = includeHtmlGuidance(itemMessage, "guidance_rationale");
                                    }
                                }
                            }

                            //remove span speChar
                            var tempData = CKEDITOR.instances[editorId].getData().toString();
                            tempData = tempData.replace(/<span class="speChar">/g, '').replace(/<\/span>/g, '');
                            tempData = cleanFillInBlankAnswerXml(tempData);
                            var item = { identifier: iddentify,  value: tempData, pointsValue: point, arrMessageGuidance: arrMessageGuidance };

                            //save text Default
                            $(this).find('iframe[allowtransparency]').contents().find('body').attr('defaultText');

                            //Only add item if item has content
                            if (CKEDITOR.instances[editorId].getData() == "") {
                                return;
                            }
                            arrayPoint.push(point);
                            textEntryList.push(item);

                            maxPoint = Math.max.apply(Math, arrayPoint);
                            maxPointStr = maxPoint.toString();

                            currentAnswerIndex += 1;
                        });

                        points = $('.dialog-list-textentry').find('#point').val();
                    }
                    var expWidthFIB = $("#expWidthFIB").val() || 10;
                    if (isEditTextEntry) {
                      var isCancel = false;
                        //Update for current textEntry
                        for (n = 0; n < iResult.length; n++) {
                            if (iResult[n].responseIdentifier == currentResponseId) {

                                responseId = currentResponseId;

                                //This is check for depending grading
                                if (isManual == "manual")
                                {
                                    if (currData.length > 0)
                                    {

                                        for (var c = 0; c < currData.length; c++) {
                                            if (currData[c].major == currentResponseId)
                                            {
                                                var r = confirm("This item is a major item. Saving it with manual grade will terminate the dependence from this item. Are you sure you want to terminate?");
                                                if (r == true) {
                                                    //Remove depending grading
                                                    currData.splice(c, 1);
                                                } else {
                                                    isCancel = true;
                                                    break;
                                                }
                                            }
                                        }

                                        //Break for and will not update iResult
                                        if (isCancel) {
                                            break;
                                        }
                                    }

                                }

                                if(parseInt($("#expLengthMax").val()) < parseInt($("#expLengthMin").val())) {
                                  isCancel = true;
                                  customAlert('Max value must be less than Min value');
                                  break;
                                }

                                iResult[n].expectedLengthMax = $("#expLengthMax").val();
                                iResult[n].expectedLengthMin = $("#expLengthMin").val();
                                iResult[n].expectedWidth = parseInt(expWidthFIB) * 10;
                                iResult[n].responseDeclaration.method = isManual;
                                iResult[n].responseDeclaration.caseSensitive = $("#caseSensitiveCheck").is(':checked');
                                iResult[n].responseDeclaration.spelling = $("#misSpelling").is(':checked');
                                iResult[n].responseDeclaration.spellingDeduction = $(".textEntryPoint .misspelling").val();
                                iResult[n].responseDeclaration.ignoreExtraSpace = $("#ignoreExtraSpace").is(':checked');
                                iResult[n].responseDeclaration.pointsValue = points;
                                iResult[n].responseDeclaration.range = isRange;
                                iResult[n].correctResponse = textEntryList;
                                iResult[n].addPadding = $('#checkBoxAddLength').is(":checked") ? 1 : 0;
                                iResult[n].validation = $('#option-vaidation').val();
                                if(iResult[n].validation === '4') {
                                  iResult[n].customRule = $('#custom-vaidation').val();
                              }
                              $(eleTextEntry.$).css('max-width', parseInt(expWidthFIB) * 10 + 'px')
                              $(eleTextEntry.$).find('img.bntGuidance').hide();

                                break;
                            }


                        }

                        //Case: when user cancel update manual we will not close popup
                        if (isCancel) {
                            return false;
                        }
                    } else {
                        //Create response identify and make sure it doesn't conflict with current.
                        responseId = createResponseIdFromDOM('#itemtypeonimagePreview');
                        expWidthFIB = $("#expWidthFIB").val() || 10;
                        var textentryHtml = '<span style="max-width:'+ parseInt(expWidthFIB) * 10 +'px;" class="textEntryInteraction" contenteditable="false" id="' + responseId + '" title="' + responseId + '"><img style="display: none;padding-left: 3px; padding-top: 0.5px;" alt="Guidance" class="imageupload bntGuidance" src="../../Content/themes/TestMaker/images/guidance_checked_small.png" title="Guidance"><img class="cke_reset cke_widget_mask textEntryInteractionMark" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" /></span>&nbsp;';
                        var allowItemTypeOnImage = window.dialogItemtypeonimage;

                        if ($.browser.msie && parseInt($.browser.version, 10) < 10) {
                            textentryHtml = '&nbsp;' + textentryHtml;
                        }

                        var textentryElement = CKEDITOR.dom.element.createFromHtml(textentryHtml);
                        editor.insertElement(textentryElement);

                        if (allowItemTypeOnImage) {
                            var $editor = $(editor.document.$).find('body');
                            var $itemtypeonimagePreview = $('#itemtypeonimagePreview:visible');
                            var w = parseInt($itemtypeonimagePreview.find('.itemtypeonimageMarkObject').attr('width'), 10);
                            var h = parseInt($itemtypeonimagePreview.find('.itemtypeonimageMarkObject').attr('height'), 10);
                            var offsetTop = h / 2 - 10;
                            var offsetLeft = w / 2 - 45;
                            var $container = $('<div/>');

                            if (iSchemeID == 9) {
                                textentryHtml = $container.append(textentryHtml)
                                    .find('.textEntryInteraction')
                                    .append('<span class="remove-item"></span>')
                                    .prop('outerHTML');
                            } else if (iSchemeID == 21) {
                                textentryHtml = $container.append(textentryHtml)
                                    .find('.textEntryInteraction')
                                    .append('<span class="itemtypeonimage-tooltip">' + responseId.replace('RESPONSE_', '') + '</span>')
                                    .append('<span class="remove-item"></span>')
                                    .prop('outerHTML');
                            }

                            $itemtypeonimagePreview.find('.itemtypeonimage').append(textentryHtml);
                            $itemtypeonimagePreview
                                .find('.itemtypeonimage')
                                .find('.textEntryInteraction[id="' + responseId + '"]')
                                .css({'left': offsetLeft + 'px', 'top': offsetTop + 'px'})
                                .attr({'data-left': offsetLeft,'data-top': offsetTop});
                            $itemtypeonimagePreview
                                .find('.itemtypeonimage')
                                .find('.textEntryInteraction')
                                .draggable({
                                    'containment': 'parent',
                                    drag: function (event, ui) {
                                        var $target = $(event.target);

                                        $target.attr('data-top', ui.position.top);
                                        $target.attr('data-left', ui.position.left);
                                    }
                                });

                            $itemtypeonimagePreview.find(".remove-item").on("click", function () {
                                var item = $(this).parent();
                                var resId = item.attr("id");
                                var select = $(this).parents(".cke_dialog_contents").find("#itemtypeonimageSelect");

                                item.remove();
                                //need remove data in iResult
                                for (var i = 0; i < iResult.length; i++) {
                                    if (iResult[i].responseIdentifier === resId) {
                                        iResult.splice(i, 1);
                                    }
                                }
                            });

                            $editor.find('.textEntryInteraction[id="' + responseId + '"]').css('display', 'none');
                        }

                        hastypeMessageGuidance = '';
                        var customRule = '';
                        if($('#option-vaidation').val() === '4') {
                          customRule = $('#custom-vaidation').val();
                        }
                        //add data
                        iResult.push({
                            type: 'textEntryInteraction',
                            responseIdentifier: responseId,
                            expectedLengthMin: $("#expLengthMin").val(),
                            expectedWidth: parseInt(expWidthFIB) * 10,
                            expectedLengthMax: $("#expLengthMax").val(),
                            addPadding: $('#checkBoxAddLength').is(":checked") ? 1 : 0,
                            validation: $('#option-vaidation').val(),
                            customRule: customRule,
                            correctResponse: textEntryList,
                            responseDeclaration: {
                                baseType: "string",
                                cardinality: "single",
                                method: isManual,
                                caseSensitive: $("#caseSensitiveCheck").is(':checked'),
                                spelling: $("#misSpelling").is(':checked'),
                                ignoreExtraSpace: $("#ignoreExtraSpace").is(':checked'),
                                spellingDeduction: $(".textEntryPoint .misspelling").val(),
                                type: "string",
                                pointsValue: points,
                                range: isRange
                            }
                        });

                        //Hide button on toolbar after add text entry incase qtItem is 9
                        if (iSchemeID == "9") {
                            $(".cke_button__textentry").parents("span.cke_toolbar").hide();
                        }

                        if (iSchemeID == "21" && !$(editor.document.$).find('body').children(':first-child').is('br')) {
                            $(editor.document.$).find('body').prepend('<br>');
                        }
                    }

                    //Reset to default after update or create new textEntry
                    isEditTextEntry = false;
                    currentResponseId = '';
                    isNewPalette = false;
                    isEditMultipChoiceGuidance = false;
                    iMessageTemp = [];
                    idSimpleChoicesPopup = '';
                    iMessageTempEdit = [];
                    isOk = true;
                    newResult = iResult;

                    if (isManual === 'algorithmic') {
                        TestMakerComponent.isShowAlgorithmicConfiguration = true;
                    } else {
                        TestMakerComponent.isShowAlgorithmicConfiguration = false;
                    }
                },
                onCancel: function () {
                    //Reset to default after update or create new textEntry
                    isEditTextEntry = false;
                    currentResponseId = '';
                    isNewPalette = false;
                    isEditMultipChoiceGuidance = false;
                    iMessageTemp = [];
                    idSimpleChoicesPopup = '';
                    iMessageTempEdit = [];
                }
            };
        });
    }
});


/***
* Load data to Text Entry popup form
* Return: responseIdentifier
***/
function loadDataforTextEntry(element) {
    var currentResponseId = "";
    var arrayEntry = [];

    $('#point, #pointRange').parent().removeClass('is-disabled');

    for (var i = 0; i < iResult.length; i++) {
        if (iResult[i].responseIdentifier == element.getId() && iResult[i].type == "textEntryInteraction") {
            var iResultItem = iResult[i];
            var textentryResponseMethod = iResultItem.responseDeclaration.method;

            resetTextEntryOnload();
            currentResponseId = iResultItem.responseIdentifier;

            //This is load data for range case
            arrayEntry = iResultItem.correctResponse;
            if (iResultItem.responseDeclaration.range != undefined &&
                iResultItem.responseDeclaration.range.toString() == "true") {
                //Show Range panel
                $("#rangeTextEntry").trigger("click");
                $('.textEntryRange').css({'display': 'block'}).show();
                $('.textEntry').hide();

                $("#textEntryList").empty();
                $('#option-vaidation').val(iResultItem.validation).change();

                // Reset list item editor
                addNewItemTextEntry();
                textEntrySelection();

                //Load point value
                $("#pointRange").val(iResultItem.responseDeclaration.pointsValue);

                //Check decimal or fraction
                var startValue = iResultItem.correctResponse[0].valueRange.toString();
                if (startValue != "") {
                    $("#startValue").attr({ "checked": true });
                    $("#divStart").show();
                } else {
                    $("#startValue").attr({ "checked": false });
                    $("#divStart").hide();
                }
                if (iResultItem.correctResponse[0].valueType == 0) {
                    $("#decimalTextEntry").trigger("click");

                    if (startValue == "NaN")
                    {
                        startValue = "0";
                    }

                    var dStart = startValue.substring(0, startValue.indexOf("."));

                    //This case for integer
                    if (startValue.indexOf(".") == -1) {
                        $("#integerTextEntry").trigger("click");
                        dStart = startValue;
                    }

                    $("#decimalStart").val(dStart)
                    $("#decimalStart1").val(startValue.substring(dStart.length + 1, startValue.length));
                } else {
                    $("#fractionTextEntry").trigger("click");

                    var dStart = startValue.substring(0, startValue.indexOf("/"));
                    if (startValue.indexOf("/") == -1) {
                        dStart = startValue;
                    }

                    $("#decimalStart").val(dStart)
                    $("#decimalStart1").val(startValue.substring(dStart.length + 1, startValue.length));
                }

                var endValue = iResultItem.correctResponse[1].valueRange.toString();
                if (endValue != "") {
                    $("#endValue").prop('checked', true);
                    $("#divEnd").show();
                } else {
                    $("#endValue").attr({ "checked": false });
                    $("#divEnd").hide();
                }
                if (iResultItem.correctResponse[1].valueType == 0) {
                    $("#endDecimalTextEntry").trigger("click");

                    var dEnd = endValue.substring(0, endValue.indexOf("."));
                    if (endValue.indexOf(".") == -1) {
                        dEnd = endValue;
                        $("#endIntegerTextEntry").trigger("click");
                    }

                    $("#decimalEnd").val(dEnd)
                    $("#decimalEnd1").val(endValue.substring(dEnd.length + 1, endValue.length));
                } else {
                    $("#endFractionTextEntry").trigger("click");
                    var dEnd = endValue.substring(0, endValue.indexOf("/"));
                    if (endValue.indexOf("/") == -1)
                    {
                        dEnd = endValue;
                    }

                    $("#decimalEnd").val(dEnd)
                    $("#decimalEnd1").val(endValue.substring(dEnd.length + 1, endValue.length));
                }

                //Check exclusivity
                if (iResultItem.correctResponse[0].exclusivity == 0) {
                    $('#startExclusivity').prop('checked', false);
                } else {
                    if ($("#startExclusivity").is(":checked") != true)
                    {
                        $("#startExclusivity").trigger("click");
                    }
                }

                if (iResultItem.correctResponse[1].exclusivity == 1) {
                    if ($("#endExclusivity").is(":checked") == false) {
                        $("#endExclusivity").trigger("click");
                    }

                } else {
                    if ($("#endExclusivity").is(":checked") == true) {
                        $("#endExclusivity").trigger("click");
                    }
                }
            } else {
                //Show Range panel
                $("#listTextEntry").trigger("click");
                $('.textEntryRange').hide();
                $('.textEntry').show();

                //Load data
                $(".dialog-list-textentry #point").val(iResultItem.responseDeclaration.pointsValue);
                $("#textEntryList").empty();

                for (var n = 0; n < arrayEntry.length; n++) {
                    //Empty the list before append new item
                    addNewItemTextEntry();
                    textEntrySelection();
                }

                $('#caseSensitiveCheck').prop('checked', convertTrueFalse(iResultItem.responseDeclaration.caseSensitive));
                $('#misSpelling').prop('checked', convertTrueFalse(iResultItem.responseDeclaration.spelling));
                $(".misspelling").val(iResultItem.responseDeclaration.spellingDeduction);
                $("#expLengthMax").val(iResultItem.expectedLengthMax);
                $("#expLengthMin").val(iResultItem.expectedLengthMin);
                var expectedWidth = iResultItem.expectedWidth ? parseInt(iResultItem.expectedWidth) / 10 : 10;
                $('#expWidthFIB').val(expectedWidth);
                $('#option-vaidation').val(iResultItem.validation).change();

                var checked = true;
                if(iResultItem.addPadding == 0){
                  checked = false;
                }

                $('#checkBoxAddLength').attr('checked', checked);
                if(iResultItem.validation === '4') {
                  $('#custom-vaidation').show();
                  $('#custom-vaidation').val(iResultItem.customRule ? iResultItem.customRule : 1).change();
                } else {
                  $('#custom-vaidation').hide();
                }
                $('#ignoreExtraSpace').prop('checked', convertTrueFalse(iResultItem.responseDeclaration.ignoreExtraSpace));

                //Enable for misspelling number
                if (iResultItem.responseDeclaration.spelling.toString() == "true") {
                    $(".misspelling, .ckUpNum_misspelling, .ckDownNum_misspelling").prop('disabled', false).removeClass("disable");
                } else {
                    $(".misspelling, .ckUpNum_misspelling, .ckDownNum_misspelling").prop('disabled', true).addClass("disable");
                }

                $('#textEntryInput').val("");
            }

            if (textentryResponseMethod === 'default') {
                $('#textentrygrade-normal').prop('checked', true);
            } else if (textentryResponseMethod === 'manual') {
                $('#textentrygrade-manual').prop('checked', true);
            } else if (textentryResponseMethod === 'ungraded') {
                $('#textentrygrade-ungraded').prop('checked', true);
                $('#point, #pointRange').parent().addClass('is-disabled');
                $('#point, #pointRange').val(0);
            } else if (textentryResponseMethod === 'algorithmic') {
                $('#textentrygrade-algorithmic').prop('checked', true);
                $('#point, #pointRange').parent().addClass('is-disabled');
            }

            break;
        }
    }

    return currentResponseId;
}
/***
* Reset data to Text Entry popup form
***/
function resetTextEntryOnload() {
    // for test
    // #gradeManually
    // gradeManuallyRange
    $('#point, #pointRange').parent().removeClass('is-disabled');
    $('#point, #pointRange').val("1");
    $(".misspelling, .ckUpNum_misspelling, .ckDownNum_misspelling").prop('disabled', true).addClass("disable");

    //$("#textEntryList").html('');
    $('#textEntryInput').val("");
    $('#misspelling-deduction').val("1");
    $('#expLengthMax').val("8");
    $('#expLengthMin').val("0");
    $('#expWidthFIB').val('30');
    $('#misSpelling, #caseSensitiveCheck, #endExclusivity, #startExclusivity, #endValue').attr('checked', false);
    $('#ignoreExtraSpace').attr('checked', true);

    //Disable misspelling check by default
    $(".misspelling, .ckUpNum_misspelling, .ckDownNum_misspelling").prop('disabled', true).addClass("disable");

    //Clear range of text entry
    $("#decimalStart, #decimalStart1, #decimalEnd, #decimalEnd1").val("");
    $("#decimalTextEntry, #endDecimalTextEntry").attr("checked", true);
    $("#startValue").attr('checked', true);
    $("#divStart").show();
    $("#divEnd").hide();
    $(".startDecimalType, .endDecimalType").text(".");
    $('#custom-vaidation').hide();
    $('#textentrygrade-normal').prop('checked', true);
}
/***
* Function to add new an anwser for text entry list
* Return EditorID this will help to create CKEditor
***/
function addNewItemTextEntry() {
    var itemTextEntryLength = $("#textEntryList li").length;
    var alphabet = [];
    if (CKEDITOR.instances[ckID].config.alphaBeta != undefined) {
        alphabet = CKEDITOR.instances[ckID].config.alphaBeta;
    }
    //maximum answer for text entry list is 42 answer correct
    if (itemTextEntryLength > 74) {
        return;
    }

    var now = Date.now(),
        editorId = 'editor' + alphabet[itemTextEntryLength] + now,
        pointId = 'point' + alphabet[itemTextEntryLength] + now;

    //push id item answer into iMessageTemp
    if (!isEditMultipChoiceGuidance) {
        iMessageTemp.push({
            idTemp: alphabet[itemTextEntryLength] + now,
            arrMessage: []
        });
    }

    var getImgByVersion = CKEDITOR.plugins.getImgByVersion;

    var defaultText = "Answer Correct " + ($("#textEntryList li").length + 1);
    var strHtml = '<li id="' + alphabet[itemTextEntryLength] + now + '">';

    strHtml += '<div class="content">';
    strHtml += '<textarea cols="100" id="' + editorId + '" name="editor1" rows="1" tabindex="' + (itemTextEntryLength + 1) + '">' + defaultText + '</textarea>';
    strHtml += '</div>';
    strHtml += '<div class="sort actions">';
    strHtml += '<input id="unselected_' + (alphabet[itemTextEntryLength]) + now + '" type="image" src="' + getImgByVersion('multiplechoice', 'images/guidance_unchecked.png') + '" class="addGuidance" title="Add Guidance/Rationales" />';
    strHtml += '<div class="savedGuidance" style="display: none;"><span class="btnRemoveGuidance" title="Remove">x</span><input issavedmessage="true" id="selected_' + (alphabet[itemTextEntryLength] + now) + '" type="image" src="' + getImgByVersion('multiplechoice', 'images/guidance_checked.png') + '" class="addGuidance" title="" /></div>';
    strHtml += '<div class="audio" style="visibility: hidden; display: none;"><input type="text" id="' + pointId + '" class="point" value="1" />';
    strHtml += '</div>';
    strHtml += '<input type="image" title="Move Up" src="' + getImgByVersion('multiplechoice', 'images/up.png') + '" class="ckImageButton ckUp" />';
    strHtml += '<input type="image" title="Move Down" src="' + getImgByVersion('multiplechoice', 'images/down.png') + '" class="ckImageButton ckDown" />';
    strHtml += '<input type="image" title="Remove" src="' + getImgByVersion('multiplechoice', 'images/remove.png') + '" class="ckImageButton ckRemove" />';
    strHtml += '</div>';
    strHtml += '<div class="clear"></div>';
    strHtml += '</li>';

    $("#textEntryList").append(strHtml);
    return editorId;
}
/***
* Function to register all events for text entry
***/
function textEntrySelection() {
    $(".ckRemove").unbind("click").click(function () {
        //Don't let's user remove all answer
        isAddAnswerTextEntryLast = true;
        if ($("#textEntryList li").length == 1) {
            return;
        }
        var answer = $(this).parent().parent(),
	        instanceName = answer.find("textarea").attr("id");
        var index = answer.index();
        if (CKEDITOR.instances[instanceName]) CKEDITOR.instances[instanceName].destroy();
        answer.remove();
        sortItemTextEntry(index);
        //Show the first CK Toolbar when remove an item
        $("#entryTop > div").hide();
        $("#entryTop > div").last().show();
    });

    var createEditorTextEntry = function (editorId) {
        try {
            CKEDITOR.instances[editorId].destroy(true);
        } catch (e) { }

        CKEDITOR.replace(editorId, {
            toolbar: [['SpecialChar']],
            sharedSpaces: {
                top: 'entryTop',
                bottom: 'entryBot'
            },
            extraAllowedContent: 'span[stylename]; span(smallText,normalText,largeText,veryLargeText);',
            height: 60,
            width: 350
        });

        // Show the first Ckeditor Toolbar when remove an item
        var $entryTopDiv = $('#entryTop > div');
        $entryTopDiv.hide();
        $entryTopDiv.last().show();
    }

    $(".ckUp").unbind("click").click(function (event) {
        event.preventDefault();
        isAddAnswerTextEntryLast = true;
        var parent = $(this).parents('li');
        var parentIndex = parent.index();
        if (parentIndex == 0) {
            return;
        }

        var editorId = 'editor' + parent.attr('id');
        if (CKEDITOR.instances[editorId]) {
            CKEDITOR.instances[editorId].destroy();
        }

        isExtraChar = false;

        // Change alphabet of anwser
        parent.prev().find('textarea').attr({ 'tabindex': parentIndex + 1 });
        parent.find('textarea').attr({ 'tabindex': parentIndex });
        parent.insertBefore(parent.prev());

        createEditorTextEntry(editorId);
    });

    $(".ckDown").unbind("click").click(function (event) {
        event.preventDefault();
        isAddAnswerTextEntryLast = true;
        var parent = $(this).parents('li');
        var parentIndex = parent.index();
        var textContent = parent.find('.content').find('iframe[allowtransparency]').contents().find('body').text();
        if (parentIndex == $('#textEntryList li').length - 1) {
            return;
        }

        var editorId = 'editor' + parent.attr('id');
        if (CKEDITOR.instances[editorId]) {
            CKEDITOR.instances[editorId].destroy();
        }

        isExtraChar = false;

        // Change alphabet of anwser
        parent.next().find('textarea').attr({ 'tabindex': parentIndex + 1 });
        parent.find('textarea').attr({ 'tabindex': parentIndex + 2 });
        parent.find('textarea').attr('defaultText', textContent);
        parent.insertAfter(parent.next());

        createEditorTextEntry(editorId);
    });

    $(".audio .point").each(function () {
        var idPoint = $(this).attr('id');
        getUpDownNumber($('#' + idPoint), 0, 1000);
    });

    $(".addGuidance").unbind("click").click(function (event) {
        var isSavedMessage = $(this).attr('issavedmessage');

        if (isSavedMessage === 'true') {
            isEditGuidancePopup = true;
        } else {
            isEditGuidancePopup = false;
        }

        var parent = $(this).parents("li");
        var editorId = 'editor' + parent.attr("id");
        idMessage = parent.attr("id");

        CKEDITOR.instances[editorId].openDialog('messageGuidanceRationales');
    });

    $(".btnRemoveGuidance").unbind("click").click(function (event) {
        var idTagLi = $(event.target).parents('li').attr('id');

        if (iMessageTempEdit.length) {
            for (var i = 0, lenMessageTemp = iMessageTempEdit.length; i < lenMessageTemp; i++) {
                var itemMessage = iMessageTempEdit[i];
                if (itemMessage.idTemp === idTagLi) {
                    iMessageTempEdit.splice(i, 1);
                    break;
                }
            }
        }

        $(this).parents(".sort").find('.savedGuidance').hide();
        $(this).parents(".sort").find("#unselected_" + idTagLi).show();
    });
}
//Create new 1 correct anwsers
function createTextEntryCorrect() {
    $("#textEntryList li").each(function() {
        var myId = "editor" + this.id;
        if (CKEDITOR.instances[myId]) CKEDITOR.instances[myId].destroy();
    });

   $("#textEntryList").empty();

    //Create default 1 item for inlineChoice
   var editorId = addNewItemTextEntry();

   if (editorId === undefined || editorId === "") {
       return;
   }

    textEntrySelection();
    createCKEditorEntryList(editorId);

    $("#textEntryList .point").val("1");
    $("#entryTop > div").hide();
    $("#entryTop > div").last().show();
}
// build a instance ckEditor
function createCKEditorEntryList(ckId) {
    isNewPalette = true;

    try {
        CKEDITOR.instances[ckId].destroy(true);
    } catch (e) {}

    CKEDITOR.replace(ckId, {
        toolbar: [['SpecialChar']],
        sharedSpaces:
             {
                 top: 'entryTop',
                 bottom: 'entryBot'
             },
        extraAllowedContent: 'span[stylename]; span(smallText,normalText,largeText,veryLargeText);',
        height: 60,
        width: 350
    });
}
/***
* Function to sort when remove an anwser of inline Choice
***/
function sortItemTextEntry(index) {
    for (var i = index; i < $("#textEntryList li").length; i++) {
        var currentAnwser = $("#textEntryList li").eq(i);
        currentAnwser.find("textarea").attr({ "tabindex": i + 1 });
    }
}

function getUpDownNumber (selector, min, max) {
    var $selector = $(selector);

    $selector.ckUpDownNumber({
        minNumber: min,
        maxNumber: max,
        width: 18,
        height: 13
    });
}

/**
 * Onload text entry
 * @return {[type]} [description]
 */
function onLoadTextEntry() {
    // Initialize up and down number
    getUpDownNumber($('.expLength'), 0, 1000);
    getUpDownNumber($('.point'), 1, 1000);
    // getUpDownNumber($('expLength'), 0, 1000);
    getUpDownNumber($('.misspelling'), 0, 500);

    // Handle point value changes to ensure minimum value of 1
    $('.point').on('change blur', function() {
        var val = $(this).val();
        if (!val || val === '' || isNaN(val) || parseInt(val) < 1) {
            $(this).val('1');
        }
    });

    // Initialize event change text entry grade
    $('.dialog-list-textentry input[type="radio"][name="textentrygrade"]').on('change', function() {
        var textentrygradeVal = $('.dialog-list-textentry input[type="radio"][name="textentrygrade"]:checked').val();
        var $textentryPoint = $('.dialog-list-textentry #point');
        var $textentryPointRange = $('.dialog-list-textentry #pointRange');

        if (textentrygradeVal === 'ungraded' || textentrygradeVal === 'algorithmic') {
            $textentryPoint.val('0');
            $textentryPointRange.val('0');
            $textentryPoint.parent().addClass('is-disabled');
            $textentryPointRange.parent().addClass('is-disabled');
        } else {
            $textentryPoint.val('1');
            $textentryPointRange.val('1');
            $textentryPoint.parent().removeClass('is-disabled');
            $textentryPointRange.parent().removeClass('is-disabled');
        }
    });
    $('input#expLengthMin').on('keyup', function() {
      var val = parseInt($(this).val()),
      max = 999;
      val = isNaN(val) ? 0 : Math.max(Math.min(val, max), 0);
      $(this).val(val);
    });
    $('input#expLengthMax').on('keyup', function() {
      var val = parseInt($(this).val()),
      max = 1000;
      val = isNaN(val) ? 0 : Math.max(Math.min(val, max), 0);
      $(this).val(val);
    });
    $('input#expLengthMax,  input#expLengthMin').on('keydown', function(e){
      if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110]) !== -1 ||
        (e.keyCode === 65 && (e.ctrlKey === true || e.metaKey === true)) ||
        (e.keyCode >= 35 && e.keyCode <= 40)) {
          return;
      }
      if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
          e.preventDefault();
      }
    });

    $('#option-vaidation').on('change', function() {
      if($(this).val() === '4') {
        $('#custom-vaidation').show();
      } else {
        $('#custom-vaidation').hide();
      }
    });
}

/**
 * Hanler keydown negative number
 * @param  {[type]} el [description]
 * @return {[type]}    [description]
 */
function handlerKeydownNegativeNumber (el) {
    var $el = $(el);

    $el.keydown(function (e) {
        var val = $(this).val();
        // Allow: backspace, delete, tab, escape, enter
        if ($.inArray(e.keyCode, [46, 8, 9, 27, 13]) !== -1 ||
            // Allow: Ctrl+A
            (e.keyCode == 65 && e.ctrlKey === true) ||
            // Allow: home, end, left, right
            (e.keyCode >= 35 && e.keyCode <= 39) ||
            // Allow: negative
            (event.keyCode == 109 || event.keyCode == 189 || event.keyCode == 173)) {
            // let it happen, don't do anything
            if (val.indexOf('-') != -1 && val.indexOf('-') == 0 && (event.keyCode == 109 || event.keyCode == 189 || event.keyCode == 173)) {
                event.preventDefault();
            } else {
                return;
            }
        }
        // Ensure that it is a number and stop the keypress
        if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
            e.preventDefault();
        }
    });
}

/**
 * Handler keydown position number
 * @param  {[type]} el [description]
 * @return {[type]}    [description]
 */
function handlerKeydownPositionNumber (el) {
    var $el = $(el);

    $el.keydown(function (e) {
        // Allow: backspace, delete, tab, escape, enter
        if ($.inArray(e.keyCode, [46, 8, 9, 27, 13]) !== -1 ||
            // Allow: Ctrl+A
            (e.keyCode == 65 && e.ctrlKey === true) ||
            // Allow: home, end, left, right
            (e.keyCode >= 35 && e.keyCode <= 39)) {
            // let it happen, don't do anything
            return;
        }
        // Ensure that it is a number and stop the keypress
        if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
            e.preventDefault();
        }
    });
}
