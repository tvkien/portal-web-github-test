CKEDITOR.plugins.add('texthotspotproperties', {
    lang: 'en', // %REMOVE_LINE_CORE%
    requires: 'dialog',
    init: function (editor) {


        editor.addCommand('textHotSpotProperties', new CKEDITOR.dialogCommand('textHotSpotProperties'));

        editor.ui.addButton('TextHotSpotProperties',
		{
		    title: 'Set Points',
		    label: 'Set Points',
		    command: 'textHotSpotProperties'
		});

        editor.widgets.add('texthotspotproperties', {
            inline: true,
            mask: true,
            allowedContent: { p: { classes: 'textHotSpotProperties', attributes: '!id,name,contenteditable' } },
            template: '<p class="textHotSpotProperties"></p>'
        });

        var currentPartialCreditResId = "";



        CKEDITOR.dialog.add('textHotSpotProperties', function (editor) {

            myhtml = '<div class="property_text">';
            myhtml += '<div class="hotspot-header-action hotspot-header-property m-b-15">';
            myhtml += '<div class="g-1-3">';
            myhtml += '     <div class="hotspot-list-property m-l-4 m-b-11"><div class="div-radio hotspot-label"><input type="radio" value="all-grading" id="cbAbsolute" name="texthotspotGrading" checked="checked"/><label class="widthLabel" for="cbAbsolute">All or Nothing Grading</label></div></div>';
            myhtml += '     <div class="hotspot-list-property m-l-4 m-b-15"><div class="div-radio"><input type="radio" value="partial-grading" id="cbRelative" name="texthotspotGrading"/><label class="widthLabel" for="cbRelative">Partial Credit Grading</label></div></div>';
            myhtml += '     <div class="hotspot-list-property m-l-4"><div class="div-radio"><input type="radio" value="algorithmic-grading" id="cbAlgorithmic" name="texthotspotGrading"/><label class="widthLabel" for="cbAlgorithmic">Algorithmic Grading</label></div></div>';
            myhtml += '</div>';
            myhtml += '<div class="g-2-3">';
            myhtml += '     <div class="hotspot-list-property text-right"><span class="widthLabel">Full Credit Points:</span> <input type="text" value="1" name="fullcreate" id="txtTextHotSpotFullCredit" class="txtFullcreate" /></div>';
            myhtml += '     <div class="hotspot-list-property text-right"><span class="widthLabel widthLabelSpecial">Maximum hot spots that can be selected:</span> <input type="text" value="1" name="maxselected" id="txtTextHotSpotMaxSelected" class="txtMaxSelected"/></div>';
            myhtml += '</div>';
            myhtml += '</div>';
            myhtml += '     <div class="listTextHotSpot"><fieldset><legend>Text Hot Spot List </legend><ul class="text-header"><li><div class="item"><div class="item-content">Text Hot Spot Content</div><div class="item-point">Point Value</div><div class="item-correct">Correct</div></div></li></ul><ul id="listTextHotSpot"></ul></fieldset></div>';
            myhtml += '</div>';

            return {
                title: 'Text Hot Spot Properties',
                minWidth: IS_V2 ? 650 : 500,
                minHeight: 100,
                resizable: CKEDITOR.DIALOG_RESIZE_NONE,
                contents:
		        [
			        {
			            id: 'texthotspotproperties',
			            label: 'Settings',
			            elements:
				        [
					        {
					            type: 'html',
					            html: myhtml,
					            onLoad: function (a) {
                                    var $dialog = $(this.getDialog().getElement().$);

                                    // Change All or Nothing Grading and Partial Credit Grading
					                $('input[name="texthotspotGrading"]').on('change', function() {
					                    var $grading = $(this);
					                    var gradingMethod = $grading.val();

                                        if (gradingMethod === 'all-grading') {
                                            resetAllGrading($dialog);
                                        } else if (gradingMethod === 'partial-grading') {
                                            resetPartialGrading($dialog);
                                        } else if (gradingMethod === 'algorithmic-grading') {
                                            resetAlgorithmicGrading($dialog);
                                        }
					                });
					            },
					            onShow: function (myEditor) {
                                    getUpDownNumber('.txtFullcreate', 0, 100);

					                //Remove source item
					                removeSourceTextHotSpot();

					                //Remove correctResponse
					                removeCorrectResponseTextHotSpot();

					                for (var i = 0; i < iResult.length; i++) {
					                    if (iResult[i].type == "textHotSpot")
					                    {
					                        //load Full credit points
					                        $('#txtTextHotSpotFullCredit').val(iResult[i].responseDeclaration.pointsValue);

					                        //Append correctResponse to drag and drop list
					                        //Clear the list before add new
					                        $('#listTextHotSpot').empty();
					                        var currentContent = $("<div>" + CKEDITOR.instances[ckID].getData() + "</div>");

					                        for (var n = 0; n < iResult[i].source.length; n++) {
												var hs_id = iResult[i].source[n].identifier;
					                            var source = currentContent.find(".marker-linkit[hs_id=" + hs_id + "]");
												var point = iResult[i].source[n].pointValue;
												var isCorrectHotSpot = "";
												var correctRes = iResult[i].correctResponse;
												for(j = 0; j < correctRes.length; j++ ){
													if(correctRes[j].identifier == hs_id)
													{
														isCorrectHotSpot= ' checked="checked"';
														break;
													}
												}

					                            //Get Text Hot Spot to show on the list
												var text = "";
												source.each(function () {
												    text += $(this).text();
												});

												$('#listTextHotSpot').append('<li identifier="' + hs_id + '"><div class="item"><div class="item-content">' + text + '</div><div class="item-point"><input type="text" value="' + point + '" class="text-item-point" /></div><div class="item-correct"><input type="checkbox"' + isCorrectHotSpot + '"/></div></div></li>');

                                                getUpDownNumber('.text-item-point', 0, 100);

					                            //Set maxSelected when show popup
												$("#txtTextHotSpotMaxSelected").val(iResult[i].maxSelected);

												$("li[identifier=" + hs_id + "] .item-correct input[type=checkbox]").click(function () {
												    if ($("#listTextHotSpot .item-correct input:checked").size() > parseInt($("#txtTextHotSpotMaxSelected").val()) && $(this).is(":checked")) {
												        $(this).attr({ "checked": false });
												        customAlert("Please increase the maximum number of hot spots a student can select.");
												    }
												});
                                            }

                                            var partialGrading = iResult[i].responseDeclaration.partialGrading.toString();
                                            var algorithmicGrading = iResult[i].responseDeclaration.algorithmicGrading.toString();

                                            $('#txtTextHotSpotFullCredit').parent().removeClass('is-disabled');

					                        //load relative grading
					                        if (partialGrading === '1') {
					                            $('#cbRelative').prop('checked', true);
					                            $(".item-point").show();
					                            $(".item-correct").hide();
					                        } else if (algorithmicGrading === '1') {
                                                $('#cbAlgorithmic').prop('checked', true);
                                                $('#txtTextHotSpotFullCredit').parent().addClass('is-disabled');
					                            $(".item-point").hide();
					                            $(".item-correct").hide();
                                            } else {
					                            $('#cbAbsolute').prop('checked', true);
					                            $(".item-point").hide();
					                            $(".item-correct").show();
					                        }
					                        break;
					                    }
					                }

                                    getUpDownNumber('.txtMaxSelected', 0, 9000);

					                refreshResponseId();
					            }
					        }
				        ]

			        }
		        ],
                onOk: function () {
                    var errorMsg = "Students will not be able to earn the maximum points possible" +
                        " on this question based on the current point allocation. " +
                        "You can 1) reduce the total points possible on the item, " +
                        "2) increase the maximum number of hot spots a student can select," +
                        " and/or 3) increase the points earned by certain hot spots.";

                    var textHotSpotMaxSelected = parseInt($('#txtTextHotSpotMaxSelected').val(), 10);
                    var $listTextHotSpot = $('#listTextHotSpot');
                    var isPartialGrading = $('#cbRelative').is(':checked');
                    var isAlgorithmicGrading = $('#cbAlgorithmic').is(':checked');

                    if ($listTextHotSpot.find('.item').length < textHotSpotMaxSelected) {
                        customAlert("Maximum hot spots that can be selected cannot be " +
						       "greater than the total number of hot spots in the item.");
                        return false;
                    }

                    if (isPartialGrading) {
                        var totalPoint = 0;
                        var pointArr = [];
                        var totalMaxSelected = 0;
                        var result = false;
                        var fullMaxChoice = parseInt($('#txtTextHotSpotMaxSelected').val(), 10);
                        var fullPoints = parseInt($('#txtTextHotSpotFullCredit').val(), 10);
                        var largest = 0;

                        $listTextHotSpot.find('.item .text-item-point').each(function (index, itemPoint) {
                            var $itemPoint = $(itemPoint);
                            var point = parseInt($itemPoint.val(), 10);

                            totalPoint += point;

                            if (point > 0) {
                                pointArr.push(point);
                            }
                        });

                        for (var i = 0; i < fullMaxChoice; i++) {
                            if (pointArr.length) {
                                largest = Math.max.apply(Math, pointArr);
                                totalMaxSelected += largest;

                                for (var temp = 0, lenPoint = pointArr.length; temp < lenPoint; temp++) {
                                    if (largest === pointArr[temp]) {
                                        pointArr.splice(temp, 1);
                                        break;
                                    }
                                }
                            }
                        }

                        var isPoint = totalPoint >= fullPoints;
                        var isMaxPoint = totalMaxSelected >= fullPoints;

                        result = isPoint && isMaxPoint;

                        if (!result) {
                            customAlert(errorMsg);
                            return false;
                        }
                    } else if (!isAlgorithmicGrading) {
                        var $hotspotChecked = $listTextHotSpot.find('.item-correct input[type=checkbox]:checked');
                        if ($listTextHotSpot.find('.item').length > 0) {
                            if (!$hotspotChecked.length) {
								customAlert('Please select the correct combination of hot spot(s) that will allow the student to earn full credit.');
								return false;
							}
						}

                        if ($hotspotChecked.length > textHotSpotMaxSelected) {
						    customAlert(errorMsg);
						    return false;
						}
					}

                    $("#cke_" + ckID + " iframe.cke_wysiwyg_frame").contents().find("span.marker-linkit").removeClass("marker-correct");

                    //Update for current textEntry
                    for (var n = 0; n < iResult.length; n++) {
                        if (iResult[n].type == "textHotSpot") {
                            var iResultResponseDeclaration = iResult[n].responseDeclaration;

                            iResultResponseDeclaration.absoluteGrading = '0';
                            iResultResponseDeclaration.partialGrading = '0';
                            iResultResponseDeclaration.algorithmicGrading = '0';

                            if (isPartialGrading) {
                                iResultResponseDeclaration.partialGrading = '1';
                            } else if (isAlgorithmicGrading) {
                                iResultResponseDeclaration.algorithmicGrading = '1';
                            } else {
                                iResultResponseDeclaration.absoluteGrading = '1';
                            }

                            iResult[n].responseDeclaration.pointsValue = $('#txtTextHotSpotFullCredit').val().toString();
                            iResult[n].maxSelected = $('#txtTextHotSpotMaxSelected').val().toString();

                            //reBuild correctResponse and source
                            iResult[n].correctResponse = [];
                            iResult[n].source = []
                            $("#listTextHotSpot li").each(function (index) {
                                var $self = $(this);
                                var pointValue = $self.find(".text-item-point").val();
                                var identify = $self.attr("identifier");
                                if (isPartialGrading) {
                                    if (parseInt($self.find(".item-point input.text-item-point").val()) > 0) {
                                        iResult[n].correctResponse.push({ identifier: identify, pointValue: pointValue });
                                        $("#cke_" + ckID + " iframe.cke_wysiwyg_frame").contents().find("span[hs_id=" + identify + "]").addClass("marker-correct");
                                    }
                                } else {
                                    if ($self.find("input[type=checkbox]").is(":checked")) {
                                        iResult[n].correctResponse.push({ identifier: identify, pointValue: pointValue });
                                        $("#cke_" + ckID + " iframe.cke_wysiwyg_frame").contents().find("span[hs_id=" + identify + "]").addClass("marker-correct");
                                    }
                                }

                                iResult[n].source.push({ identifier: identify, pointValue: pointValue });
                            });

                            break;
                        }
                    }

                    if (isAlgorithmicGrading) {
                        TestMakerComponent.isShowAlgorithmicConfiguration = true;
                    } else {
                        TestMakerComponent.isShowAlgorithmicConfiguration = false;
                    }
                }
            };
        });
    }
});

function getUpDownNumber(selector, min, max) {
    var $selector = $(selector);

    $selector.ckUpDownNumber({
        minNumber: min,
        maxNumber: max,
        width: 18,
        height: 13
    });
}

function resetAllGrading (dialog) {
    var $dialog = $(dialog);
    var $listTextHotSpot = $dialog.find('#listTextHotSpot');

    $dialog.find('#txtTextHotSpotFullCredit').parent().removeClass('is-disabled');
    $dialog.find('#txtTextHotSpotFullCredit').val('1');
    $listTextHotSpot.siblings('.text-header').find('.item-point').hide();
    $listTextHotSpot.siblings('.text-header').find('.item-correct').show();
    $listTextHotSpot.find('.item-point').hide();
    $listTextHotSpot.find('.item-correct').show();
    $listTextHotSpot.find('.item-correct input[type="checkbox"]').prop('checked', false);
    $listTextHotSpot.find('.item-point .text-item-point').val('0');
}

function resetPartialGrading (dialog) {
    var $dialog = $(dialog);
    var $listTextHotSpot = $dialog.find('#listTextHotSpot');

    $dialog.find('#txtTextHotSpotFullCredit').parent().removeClass('is-disabled');
    $dialog.find('#txtTextHotSpotFullCredit').val('1');
    $listTextHotSpot.siblings('.text-header').find('.item-point').show();
    $listTextHotSpot.siblings('.text-header').find('.item-correct').hide();
    $listTextHotSpot.find('.item-point').show();
    $listTextHotSpot.find('.item-correct').hide();
    $listTextHotSpot.find('.item-correct input[type="checkbox"]').prop('checked', false);
    $listTextHotSpot.find('.item-point .text-item-point').val('0');

    if ($listTextHotSpot.find('li').length) {
        $dialog.find('.listTextHotSpot').show();
    }
}

function resetAlgorithmicGrading (dialog) {
    var $dialog = $(dialog);
    var $listTextHotSpot = $dialog.find('#listTextHotSpot');

    $dialog.find('#txtTextHotSpotFullCredit').parent().addClass('is-disabled');
    $dialog.find('#txtTextHotSpotFullCredit').val('0');
    $listTextHotSpot.siblings('.text-header').find('.item-point').hide();
    $listTextHotSpot.siblings('.text-header').find('.item-correct').hide();
    $listTextHotSpot.find('.item-point').hide();
    $listTextHotSpot.find('.item-correct').hide();
    $listTextHotSpot.find('.item-correct input[type="checkbox"]').prop('checked', false);
    $listTextHotSpot.find('.item-point .text-item-point').val('0');
}
