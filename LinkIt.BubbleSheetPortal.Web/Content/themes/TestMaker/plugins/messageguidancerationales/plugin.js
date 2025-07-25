/**
 * @license Copyright (c) 2003-2013, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

CKEDITOR.plugins.add( 'messageguidancerationales', {
	requires: 'dialog',
	lang: 'af,ar,bg,bn,bs,ca,cs,cy,da,de,el,en,en-au,en-ca,en-gb,eo,es,et,eu,fa,fi,fo,fr,fr-ca,gl,gu,he,hi,hr,hu,id,is,it,ja,ka,km,ko,ku,lt,lv,mk,mn,ms,nb,nl,no,pl,pt,pt-br,ro,ru,si,sk,sl,sq,sr,sr-latn,sv,th,tr,ug,uk,vi,zh,zh-cn', // %REMOVE_LINE_CORE%
	icons: 'messageguidancerationales', // %REMOVE_LINE_CORE%
	hidpi: true, // %REMOVE_LINE_CORE%
	init: function( editor ) {
		if ( editor.blockless )
			return;

	    var objGuidance = {};
	    var objRationale = {};
	    var objGuidanceRationale = {};
	    var arrMessageTemp = [];
	    var newobjGuidance = [];
	    var objMessageError = {	        
	        guidance: "The message cannot be empty",
	        rationale: "The message cannot be empty",
	        general_guidance_rationale: "The message cannot be empty"
	    };

	    //editor.getSelection().createBookmarks();
	    //set event click play audio
	    editor.on('contentDom', function () {

	        //play audio
	        var typeMessageDiv = $('iframe[allowtransparency]').contents().find('body').find('div[typeMessage]');
	        var btnPlay = typeMessageDiv.find('.audioIcon .bntPlay');
	        btnPlay.unbind("click").on('click', function (e) {
	            $(editor.window.getFrame().$).contents().find('.audioIcon .bntStop').hide();
	            $(editor.window.getFrame().$).contents().find('.audioIcon .bntPlay').show();
	            resetUIAudio();
	            var audioUrl = $(this).parent().find(".audioRef").text();
	            playAudio(this, audioUrl);
	        });
	        //stop audio
	        var bntStop = typeMessageDiv.find('.audioIcon .bntStop');
	        bntStop.unbind("click").on('click', function (e) {
	            stopAudio(this);
	        });

	    });
	    
		editor.addCommand('messageGuidanceRationales', new CKEDITOR.dialogCommand('messageGuidanceRationales'));
	    
		CKEDITOR.dialog.add('messageGuidanceRationales', function (editor) {
		    
		    // CKEDITOR.dialog.definition
		    var htmlString = '';

		    htmlString += '<div class="content_message" id="' + idMessage + '">';
		    htmlString += ' <div class="message_item"><p><input typemessage="guidance" type="checkbox" id="ck_guidance_' + idMessage + '" name="student_guidance"/><label for="ck_guidance_' + idMessage + '">Student Guidance message</label>';
		    htmlString += '     <span class="error_message" id="error_message_guidance_' + idMessage + '"></span>';
		    htmlString += '     </p>';
		    htmlString += '     <div id="student_guidance_' + idMessage + '" class="content_student_guidance"></div>';
		    htmlString += ' </div>';
		    htmlString += ' <div class="message_item"><p><input typemessage="rationale" type="checkbox" id="ck_rationale_' + idMessage + '" name="teacher_rationale"/><label for="ck_rationale_' + idMessage + '">Teacher Rationale message</label>';
		    htmlString += '     <span class="error_message" id="error_message_rationale_' + idMessage + '"></span>';
		    htmlString += '     </p>';
		    htmlString += '     <div id="teacher_rationale_'+ idMessage + '" class="content_teacher_rationale"></div>';
		    htmlString += ' </div>';
		    htmlString += ' <div class="message_item"><p><input typemessage="guidance_rationale" type="checkbox" id="ck_gr_' + idMessage + '" name="st_guidance_rationale"/><label for="ck_gr_' + idMessage + '">Use the same message for both</label></p>';
		    htmlString += '     <div id="st_guidance_rationale_' + idMessage + '" class="content_st_guidance_rationale"></div>';
		    htmlString += ' </div>';
		    htmlString += ' <p class="error_message_general" id="general_message_' + idMessage + '"></p>';
		    htmlString += '</div>';
		    
		    
		    var dialogDefinition = {
		        title: 'Guidance/Rationale Message',
		        minWidth: 450,
		        minHeight: 50,
		        contents: [
                    {
                        id: 'info_message',
                        label: 'Message Guidance Rationales Answer',
                        elements: [
                            {
                                type: 'html',
                                html: htmlString,
                                onShow: function () {
                                    //reset message guidance or rationales
                                    hasGuidance = true;
                                    resetMessage();
                                    createContentMessage(idMessage);
                                    
                                    $('#ck_guidance_' + idMessage).click(function () {
                                        
                                        if (this.checked) {
                                            
                                            $('#ck_gr_' + idMessage).prop("disabled", true);
                                            $('label[for=ck_gr_' + idMessage + ']').css('opacity', '0.6');
                                            
                                            $('#student_guidance_' + idMessage).css('padding-bottom', '10px').show();
                                            $('#general_message_' + idMessage).hide();
                                        } else {
                                            
                                            if ($('#ck_rationale_' + idMessage).prop('checked') === false) {
                                                $('#ck_gr_' + idMessage).prop("disabled", false);
                                                $('label[for=ck_gr_' + idMessage + ']').css('opacity', '1');
                                            }
                                            $('#student_guidance_' + idMessage).css('padding-bottom', '0').hide();
                                            $('#error_message_guidance_' + idMessage).hide();
                                        }
                                        
                                    });
                                                                       
                                    $('#ck_rationale_' + idMessage).click(function () {

                                        if (this.checked) {
                                            
                                            $('#ck_gr_' + idMessage).prop("disabled", true);
                                            $('label[for=ck_gr_' + idMessage + ']').css('opacity', '0.6');
                                            
                                            $('#teacher_rationale_' + idMessage).css('padding-bottom', '10px').show();
                                            $('#general_message_' + idMessage).hide();
                                        } else {
                                            
                                            if ($('#ck_guidance_' + idMessage).prop('checked') === false) {
                                                $('#ck_gr_' + idMessage).prop("disabled", false);
                                                $('label[for=ck_gr_' + idMessage + ']').css('opacity', '1');
                                            }
                                            $('#teacher_rationale_' + idMessage).css('padding-bottom', '0').hide();
                                            $('#error_message_rationale_' + idMessage).hide();
                                        }

                                    });
                                    
                                    $('#ck_gr_' + idMessage).click(function () {

                                        if (this.checked) {
                                            resetBodyContentEditor();
                                            $('#st_guidance_rationale_' + idMessage).show();
                                            $('#teacher_rationale_' + idMessage).css('padding-bottom', '0').hide();
                                            $('#student_guidance_' + idMessage).css('padding-bottom', '0').hide();
                                            $('#ck_guidance_' + idMessage).prop("disabled", true);
                                            $('#ck_rationale_' + idMessage).prop("disabled", true);
                                            $('label[for=ck_guidance_' + idMessage + ']').css('opacity', '0.6');
                                            $('label[for=ck_rationale_' + idMessage + ']').css('opacity', '0.6');
                                            
                                            var tagAudio = $('#questionType_guidance_rationale_' + idMessage).find('.audioRef');
                                            if (tagAudio.text() != '') {
                                                $('.audioRef_guidance_rationale_' + idMessage).parents('.questionTypeGuidance').show();
                                            }
                                            $('#general_message_' + idMessage).hide();
                                        } else {
                                            //resetBodyContentGuidanceRationale();
                                            $('#st_guidance_rationale_' + idMessage).hide();
                                            //$('#teacher_rationale_' + idMessage).css('padding-bottom', '10px').show();
                                            //$('#student_guidance_' + idMessage).css('padding-bottom', '10px').show();
                                            $('#ck_guidance_' + idMessage).prop("disabled", false);
                                            $('#ck_rationale_' + idMessage).prop("disabled", false);
                                            $('label[for=ck_guidance_' + idMessage + ']').css('opacity', '1');
                                            $('label[for=ck_rationale_' + idMessage + ']').css('opacity', '1');
                                            $('#general_message_' + idMessage).hide();
                                        }

                                    });

                                    $('.questionTypeGuidance .bntPlay').click(function (evt) {
                                        resetUIAudio();

                                        $(evt.target).parent('.audioRemove_').find('.bntStop').show();
                                        $(evt.target).hide();

                                        var audioUrl = $(evt.target).parent().find(".audioRef").text();

                                        if (window.playsound != undefined) {
                                            window.playsound.pause();
                                        }

                                        var audioLink = '';

                                        audioLink = audioUrl;
                                        
                                        if (audioLink.indexOf('http') !== 0) {
                                            var S3Link = '';
                                            S3Link = MKEditor.GetViewReferenceImg;

                                            if (S3Link.slice(-1) !== '/') {
                                                S3Link += '/';
                                            }

                                            if (audioLink.charAt(0) === '/') {
                                                audioLink = audioLink.slice(1);
                                            }

                                            audioLink = S3Link + audioLink;
                                        }

                                        window.playsound = new vnsAudio({
                                            src: audioLink,
                                            onEnded: function () {
                                                $(evt.target).next().hide();
                                                $(evt.target).show();
                                            }
                                        });
                                    });
                                    $('.questionTypeGuidance .bntStop').click(function (evt) {
                                        
                                        $(evt.target).parent('.audioRemove_').find('.bntPlay').show();
                                        $(evt.target).hide();
                                        if (window.playsound != undefined) {
                                            window.playsound.pause();
                                        }
                                        
                                    });
                                    
                                    $('.questionTypeGuidance .removeAudio').click(function (evt) {
                                        
                                        $(evt.target).parents('.questionTypeGuidance').hide();
                                        $(evt.target).next().empty(); // Remove url of audio
                                        
                                        if (window.playsound != undefined) {
                                            window.playsound.pause();
                                        }
                                        resetUIAudio();
                                    });

                                    CKEDITOR.on('instanceReady', function (ev) {
                                        $('.content_student_guidance .content > div').css('width', '100%');
                                        $('.content_teacher_rationale .content > div').css('width', '100%');
                                        $('.content_st_guidance_rationale .content > div').css('width', '100%');
                                    });

                                    resetBodyContentEditor();
                                    resetBodyContentGuidanceRationale();

                                    //load data middle iMessageTemp and iMessageTempEdit
                                    if (isEditGuidancePopup) {
                                        if (iMessageTempEdit.length) {
                                            var isShowEditItem = false;
                                            var i = 0;
                                            var leniMessageTempEdit = iMessageTempEdit.length;

                                            for (i = 0; i < leniMessageTempEdit; i++) {
                                                if (iMessageTempEdit[i].idTemp === idMessage) {
                                                    isShowEditItem = true;
                                                    loadDataMessage(idMessage, iMessageTempEdit);
                                                    break;
                                                }
                                            }

                                            for (i = 0; i < leniMessageTempEdit; i++) {
                                                if (iMessageTempEdit[i].idTemp !== idMessage && !isShowEditItem) {
                                                    loadDataMessage(idMessage, iMessageTemp);
                                                    break;
                                                }
                                            }
                                        } else {
                                            loadDataMessage(idMessage, iMessageTemp);
                                        }
                                    }
                                }
                            }
                        ]
                    }
		        ],
		        buttons: [CKEDITOR.dialog.okButton, CKEDITOR.dialog.cancelButton],
		        onOk: function () {
                    var isCheckGuidance = false;
                    var isCheckRationale = false;
                    var isCheckGuidanceRationale = false;
                    var isSavedData = false;
                    var isGuidance = false;
                    var isRationale = false;
                    var isGuidanceRationale = false;
                    var isGecko = CKEDITOR.env.gecko; // Gecko-based browser, like Firefox.

		            var tagLi = $('#multipleChoice:visible, #inlineChoice:visible, #textEntryList:visible').find('li#' + idMessage);
		                tagLi.find('#selected_' + idMessage).attr('title', '');
		            
		            if ($('#ck_guidance_' + idMessage).prop('checked') === false && $('#ck_rationale_' + idMessage).prop('checked') === false && $('#ck_gr_' + idMessage).prop('checked') === false) {
		                $('#general_message_' + idMessage).text(objMessageError.general_guidance_rationale).show();
		                return false;
		            }
		          
		            //save message edtior into array temp iResult
		            if ($('#ck_guidance_' + idMessage).prop('checked') === true) {
		                isCheckGuidance = true;
		                
		                if (isGecko) {
		                    $(CKEDITOR.instances['textarea_' + idtypeGuidanceMessage].window.getFrame().$).contents().find('div#__zsc_once').replaceWith('');
		                }
		                
		                if ($('.audioRemove_').find('.audioRef_' + idtypeGuidanceMessage).text() === '' && CKEDITOR.instances['textarea_' + idtypeGuidanceMessage].getData() === '') {
		                    //isCheckGuidance = false;
		                    $('#error_message_guidance_' + idMessage).text(objMessageError.guidance).show();
		                    if ($('#ck_rationale_' + idMessage).prop('checked') === true) {
		                        if ($('.audioRemove_').find('.audioRef_' + idtypeRationaleMessage).text() === '' && CKEDITOR.instances['textarea_' + idtypeRationaleMessage].getData() === '') {
		                            $('#error_message_rationale_' + idMessage).text(objMessageError.rationale).show();
		                        }
		                    }                        
		                    return false;
		                }

                        var guidanceContent = CKEDITOR.instances['textarea_' + idtypeGuidanceMessage].getData();
                        guidanceContent = $('<div>' + guidanceContent + '</div>').html();
		                
		                objGuidance = {
		                    typeMessage: 'guidance',
		                    audioRef: $('.audioRemove_').find('.audioRef_' + idtypeGuidanceMessage).text(),
		                    valueContent: guidanceContent
		                };
		            }
		              
		            if ($('#ck_rationale_' + idMessage).prop('checked') === true) {
		                isCheckRationale = true;
		                
		                if (isGecko) {
		                    $(CKEDITOR.instances['textarea_' + idtypeRationaleMessage].window.getFrame().$).contents().find('div#__zsc_once').replaceWith('');
		                }
		                
		                if ($('.audioRemove_').find('.audioRef_' + idtypeRationaleMessage).text() === '' && CKEDITOR.instances['textarea_' + idtypeRationaleMessage].getData() === '') {
		                    //isCheckRationale = false;
		                    $('#error_message_rationale_' + idMessage).text(objMessageError.rationale).show();
		                    if ($('#ck_guidance_' + idMessage).prop('checked') === true) {
		                        if ($('.audioRemove_').find('.audioRef_' + idtypeGuidanceMessage).text() === '' && CKEDITOR.instances['textarea_' + idtypeGuidanceMessage].getData() === '') {
		                            $('#error_message_guidance_' + idMessage).text(objMessageError.guidance).show();
		                        }
		                    }
		                    return false;
		                }

                        var rationaleContent = CKEDITOR.instances['textarea_' + idtypeRationaleMessage].getData();
                        rationaleContent = $('<div>' + rationaleContent + '</div>').html();
		                
		                objRationale = {
		                    typeMessage: 'rationale',
		                    audioRef: $('.audioRemove_').find('.audioRef_' + idtypeRationaleMessage).text(),
		                    valueContent: rationaleContent
		                };
		            }
		            
		            if ($('#ck_gr_' + idMessage).prop('checked') === true) {
		                isCheckGuidanceRationale = true;
		                if (isGecko) {
		                    $(CKEDITOR.instances['textarea_' + idtypeGuidanceRationaleMessage].window.getFrame().$).contents().find('div#__zsc_once').replaceWith('');
		                }
		                if ($('.audioRemove_').find('.audioRef_' + idtypeGuidanceRationaleMessage).text() === '' && CKEDITOR.instances['textarea_' + idtypeGuidanceRationaleMessage].getData() === '') {
		                    //isCheckGuidanceRationale = false;
		                    $('#general_message_' + idMessage).text(objMessageError.general_guidance_rationale).show();
		                    return false;
		                }
		                var GR_Content = CKEDITOR.instances['textarea_' + idtypeGuidanceRationaleMessage].getData();
		                GR_Content = $("<div>" + GR_Content + "</div>").html();
		                objGuidanceRationale = {
		                    typeMessage: 'guidance_rationale',
		                    audioRef: $('.audioRemove_').find('.audioRef_' + idtypeGuidanceRationaleMessage).text(),
		                    valueContent: GR_Content
		                };
		            }
		            
		            //put data temp into iMessageTemp
		            if (isCheckGuidance) {
		                if (isEditGuidancePopup) {
		                     itemStoreMessage(iMessageTempEdit, arrMessageTemp, 'guidance', objGuidance);
		                    arrMessageTemp = [];
		                } else {
		                    if (iMessageTempEdit.length) {
		                        
		                        for (var i = 0, leniMessageTemp = iMessageTempEdit.length; i < leniMessageTemp; i++) {
		                            var itemMessageTempEdit = iMessageTempEdit[i];
		                            
		                            if (itemMessageTempEdit.idTemp === idMessage) {
		                                for (var j = 0, lenArrayMessage = itemMessageTempEdit.arrMessage.length; j < lenArrayMessage; j++) {
		                                    itemMessageTempEdit.arrMessage.push(objGuidance);
		                                    newobjGuidance = [];
		                                }

		                            }
		                            if (itemMessageTempEdit.idTemp !== idMessage && !isRationale) {
		                                if (objGuidance.audioRef != '' || objGuidance.valueContent != '') {
		                                    isGuidance = true;
		                                    arrMessageTemp.push(objGuidance);
		                                    iMessageTempEdit.push({
		                                        idTemp: idMessage,
		                                        arrMessage: arrMessageTemp
		                                    });
		                                    arrMessageTemp = [];
		                                    break;
		                                }
		                            }  
		                        }
		                        
		                    } else {
		                        itemStoreMessage(iMessageTempEdit, arrMessageTemp, 'guidance', objGuidance);
		                        arrMessageTemp = [];
		                    } 
		                }
		                

		                if (objGuidance.audioRef != '' || objGuidance.valueContent != '') {
		                    isSavedData = true;
		                }
		                
		                if (tagLi.find('#selected_' + idMessage).attr('title') != '') {
		                    tagLi.find('#selected_' + idMessage).attr('title', 'Guidance and Rationale');
		                } else {
		                    tagLi.find('#selected_' + idMessage).attr('title', 'Guidance');
		                }
		                
		            } else {
		                //remove guidance into iMessageTemp when unchecked 
		                iMessageTempEdit = removeTypeMessageGuidance(iMessageTempEdit, 'guidance');
		                
		                
		                if (tagLi.find('#selected_' + idMessage).attr('title') != '') {
		                    tagLi.find('#selected_' + idMessage).attr('title', 'Rationale');
		                }
		                
		            }//end guidance data
		            
		            if (isCheckRationale) {
		                if (isEditGuidancePopup) {
		                    itemStoreMessage(iMessageTempEdit, arrMessageTemp, 'rationale', objRationale);
		                    arrMessageTemp = [];
		                } else {
		                    if (iMessageTempEdit.length) {

		                        for (var i = 0, leniMessageTemp = iMessageTempEdit.length; i < leniMessageTemp; i++) {
		                            var itemMessageTempEdit = iMessageTempEdit[i];
		                            
		                            if (itemMessageTempEdit.idTemp === idMessage) {
		                                for (var j = 0, lenArrayMessage = itemMessageTempEdit.arrMessage.length; j < lenArrayMessage; j++) {
		                                    itemMessageTempEdit.arrMessage.push(objRationale);
		                                    newobjGuidance = [];
		                                }
		                            }
		                            
		                            if (itemMessageTempEdit.idTemp !== idMessage && !isGuidance) {
		                                if (objRationale.audioRef != '' || objRationale.valueContent != '') {
		                                    isRationale = true;
		                                    arrMessageTemp.push(objRationale);
		                                    iMessageTempEdit.push({
		                                        idTemp: idMessage,
		                                        arrMessage: arrMessageTemp
		                                    });
		                                    arrMessageTemp = [];
		                                    break;
		                                }
		                            }
		                        }

		                    } else {
		                        itemStoreMessage(iMessageTempEdit, arrMessageTemp, 'guidance', objRationale);
		                        arrMessageTemp = [];
		                    } 
		                }
		                
		                
		                if (objRationale.audioRef != '' || objRationale.valueContent != '') {
		                    isSavedData = true;
		                }
		                
		                if (tagLi.find('#selected_' + idMessage).attr('title') != '') {
		                    tagLi.find('#selected_' + idMessage).attr('title', 'Guidance and Rationale');
		                } else {
		                    tagLi.find('#selected_' + idMessage).attr('title', 'Rationale');
		                }
		                
		            } else {
		                //remove rationale into iMessageTemp when unchecked
		                iMessageTempEdit = removeTypeMessageGuidance(iMessageTempEdit, 'rationale');
		                
		                if (tagLi.find('#selected_' + idMessage).attr('title') != '') {
		                    tagLi.find('#selected_' + idMessage).attr('title', 'Guidance');
		                }
		                
		            }//end rationale data
		            
		            if (isCheckGuidanceRationale) {
		                if (isEditGuidancePopup) {
		                    itemStoreMessage(iMessageTempEdit, arrMessageTemp, 'guidance_rationale', objGuidanceRationale);
		                    arrMessageTemp = [];
		                } else {
		                    if (iMessageTempEdit.length) {
		                        for (var i = 0, leniMessageTemp = iMessageTempEdit.length; i < leniMessageTemp; i++) {
		                            var itemMessageTempEdit = iMessageTempEdit[i];

		                            if (itemMessageTempEdit.idTemp === idMessage) {
		                                for (var j = 0, lenArrayMessage = itemMessageTempEdit.arrMessage.length; j < lenArrayMessage; j++) {
		                                    itemMessageTempEdit.arrMessage.push(objGuidanceRationale);
		                                    newobjGuidance = [];
		                                }
		                            }

		                            if (itemMessageTempEdit.idTemp !== idMessage && !isGuidanceRationale) {
		                                if (objRationale.audioRef != '' || objRationale.valueContent != '') {
		                                    isGuidanceRationale = true;
		                                    arrMessageTemp.push(objGuidanceRationale);
		                                    iMessageTempEdit.push({
		                                        idTemp: idMessage,
		                                        arrMessage: arrMessageTemp
		                                    });
		                                    arrMessageTemp = [];
		                                    break;
		                                }
		                            }
		                        }
		                    } else {
		                        itemStoreMessage(iMessageTempEdit, arrMessageTemp, 'guidance_rationale', objGuidanceRationale);
		                        arrMessageTemp = [];
		                    }
		                }
		                
		                if (objGuidanceRationale.audioRef != '' || objGuidanceRationale.valueContent != '') {
		                    isSavedData = true;
		                }
		                
		                tagLi.find('#selected_' + idMessage).attr('title', 'Guidance and Rationale');
		                
		            } else {
		                //remove guidance rationale into iMessageTemp when unchecked 
		                iMessageTempEdit = removeTypeMessageGuidance(iMessageTempEdit, 'guidance_rationale');

		            }//end guidance rationale data

		            if (!isSavedData) {
		                //alert('Please enter one of items guidance, rationale or guidance rationale');
		                tagLi.find('#selected_' + idMessage).attr('title', '');
		                return false;
		            }
		            
		            tagLi.find('#unselected_' + idMessage).hide();
		            tagLi.find('#selected_' + idMessage).parent('.savedGuidance').show();

		            $('#student_guidance_' + idMessage).css('padding-bottom', '0');
		            $('#teacher_rationale_' + idMessage).css('padding-bottom', '0');
		            $('#ck_gr_' + idMessage).prop("disabled", false);
		            
		            idMessage = '';
		            idtypeGuidanceMessage = '';
		            idtypeRationaleMessage = '';
		            idtypeGuidanceRationaleMessage = '';
		            hasGuidance = false;
		            
		            objGuidance = {};
		            objRationale = {};
		            objGuidanceRationale = {};
		            arrMessage = [];
		            isEditGuidancePopup = false;
		            isRemoveGuidancePopup = false;
		            

		            if (window.playsound != undefined) {
		                window.playsound.pause();
		            }
		        },
		        onCancel: function () {
		            
		            $('#student_guidance_' + idMessage).css('padding-bottom', '0');
		            $('#teacher_rationale_' + idMessage).css('padding-bottom', '0');
		            $('#ck_gr_' + idMessage).prop("disabled", false);
		            
		            idMessage = '';
		            idtypeGuidanceMessage = '';
		            idtypeRationaleMessage = '';
		            idtypeGuidanceRationaleMessage = '';
		            hasGuidance = false;
		            
		            objGuidance = {};
		            objRationale = {};
		            objGuidanceRationale = {};
		            arrMessage = [];
		            isEditGuidancePopup = false;
		            isRemoveGuidancePopup = false;
		            
		            if (window.playsound != undefined) {
		                window.playsound.pause();
		            }
		        }
		    };

		    return dialogDefinition;
		    
		});
	}
});
//remove Type Message Guidance
function removeTypeMessageGuidance(iMessageTempEdit, typeMessageGuidance) {
    var newMessageTempEdit = [];
    for (var i = 0, lenMessageTemp = iMessageTempEdit.length; i < lenMessageTemp; i++) {
        var itemMessage = iMessageTempEdit[i];
        if (itemMessage.idTemp === idMessage) {

            if (itemMessage.arrMessage.length) {
                for (var j = 0, lenArrayMessage = itemMessage.arrMessage.length; j < lenArrayMessage; j++) {
                    if (itemMessage.arrMessage[j].typeMessage === typeMessageGuidance) {
                        itemMessage.arrMessage.splice(j, 1);
                        break;
                    }
                }
            }
        }
    }
    newMessageTempEdit = iMessageTempEdit;
    return newMessageTempEdit;
}
//load data guidance, rationale or guidance rationale
function loadDataMessage(idMessage, iMessageContent) {
    
    for (var i = 0, lenMessageTemp = iMessageContent.length; i < lenMessageTemp; i++) {
        var itemMessage = iMessageContent[i];
        if (itemMessage.idTemp === idMessage) {
            for (var j = 0, lenArrMesssage = itemMessage.arrMessage.length; j < lenArrMesssage; j++) {
                var itemMessageChild = itemMessage.arrMessage[j];
                var itemMessageChildAudio = itemMessageChild.audioRef;
                var itemMessageChildContent = itemMessageChild.valueContent;
                var itemMessageTempId = itemMessage.idTemp;

                if (itemMessageChildContent !== '') {
                    itemMessageChildContent = unreplaceVideo(itemMessageChildContent);
                }

                switch(itemMessage.arrMessage[j].typeMessage) {
                    case 'guidance':
                        if (itemMessageChildAudio !== '') {
                            $('.audioRemove_').find('.audioRef_guidance_' + itemMessageTempId).text(itemMessageChildAudio);
                            $('.audioRemove_').find('.audioRef_guidance_' + itemMessageTempId).parents('.questionTypeGuidance').show();
                        }
                        if (itemMessageChildContent !== '') {
                            CKEDITOR.instances['textarea_guidance_' + itemMessageTempId].setData(itemMessageChildContent);
                        }

                        $('#ck_guidance_' + itemMessageTempId).prop('checked', true);
                        $('#student_guidance_' + itemMessageTempId).show();
                        $('#ck_gr_' + itemMessageTempId).prop('checked', false);
                        $('#ck_gr_' + itemMessageTempId).prop("disabled", true);
                        $('label[for=ck_gr_' + idMessage + ']').css('opacity', '0.6');
                        $('#st_guidance_rationale_' + itemMessageTempId).hide();
                        break;
                    case 'rationale':
                        if (itemMessageChildAudio !== '') {
                            $('.audioRemove_').find('.audioRef_rationale_' + itemMessageTempId).text(itemMessageChildAudio);
                            $('.audioRemove_').find('.audioRef_rationale_' + itemMessageTempId).parents('.questionTypeGuidance').show();
                        }
                        if (itemMessageChildContent !== '') {
                            CKEDITOR.instances['textarea_rationale_' + itemMessageTempId].setData(itemMessageChildContent);
                        }

                        $('#ck_rationale_' + itemMessageTempId).prop('checked', true);
                        $('#teacher_rationale_' + itemMessageTempId).show();
                        $('#ck_gr_' + itemMessageTempId).prop('checked', false);
                        $('#ck_gr_' + itemMessageTempId).prop("disabled", true);
                        $('label[for=ck_gr_' + idMessage + ']').css('opacity', '0.6');
                        $('#st_guidance_rationale_' + itemMessageTempId).hide();
                        break;
                    case 'guidance_rationale':
                        if (itemMessageChildAudio !== '') {
                            $('.audioRemove_').find('.audioRef_guidance_rationale_' + itemMessageTempId).text(itemMessageChildAudio);
                            $('.audioRemove_').find('.audioRef_guidance_rationale_' + itemMessageTempId).parents('.questionTypeGuidance').show();
                        }
                        if (itemMessageChildContent !== '') {
                            CKEDITOR.instances['textarea_guidance_rationale_' + itemMessageTempId].setData(itemMessageChildContent);
                        }
                        $('#ck_gr_' + itemMessageTempId).prop('checked', true);
                        $('#student_guidance_' + itemMessageTempId).hide();
                        $('#teacher_rationale_' + itemMessageTempId).hide();
                        $('#st_guidance_rationale_' + itemMessageTempId).show();
                        $('#ck_guidance_' + itemMessageTempId).prop("disabled", true);
                        $('#ck_rationale_' + itemMessageTempId).prop("disabled", true);
                        $('label[for=ck_guidance_' + idMessage + ']').css('opacity', '0.6');
                        $('label[for=ck_rationale_' + idMessage + ']').css('opacity', '0.6');
                        break;
                }
            }
            break;
        }
    }
}
//reset message guidance or rationales
function resetMessage() {
    $('#ck_guidance_' + idMessage).prop('checked', false);
    $('#ck_rationale_' + idMessage).prop('checked', false);
    $('#ck_gr_' + idMessage).prop('checked', false);
    
    $('#student_guidance_' + idMessage).empty();
    $('#teacher_rationale_' + idMessage).empty();
    $('#st_guidance_rationale_' + idMessage).empty();
    
    $('#ck_guidance_' + idMessage).prop("disabled", false);
    $('#ck_rationale_' + idMessage).prop("disabled", false);
    $('#ck_gr_' + idMessage).prop("disabled", false);

    $('#error_message_guidance_' + idMessage).hide();
    $('#error_message_rationale_' + idMessage).hide();
    $('#general_message_' + idMessage).hide();
    
    $('label[for=ck_guidance_' + idMessage + ']').css('opacity', '1');
    $('label[for=ck_rationale_' + idMessage + ']').css('opacity', '1');
    $('label[for=ck_gr_' + idMessage + ']').css('opacity', '1');
    
}
//build a instance ckEditor
function createMessageCKEditor(ckId) {
    var idContent = 'textarea_' + ckId;

    try {
        CKEDITOR.instances[idContent].destroy(true);
    } catch (e) {}

    CKEDITOR.replace(idContent, {
        extraPlugins: 'video',
        toolbar: [['Bold', 'Italic', 'Underline'], ['FontSize'], ['ImageUpload', 'AudioUpload', 'VideoUpload']],
        sharedSpaces:
             {
                 top: 'messageTop_' + ckId,
                 bottom: 'messageBot_' + ckId
             },
        extraAllowedContent: 'span[stylename]; span(smallText,normalText,largeText,veryLargeText);',
        height: 100,
        width: 450,
        on: {
            dataReady: function(evt) {
                // Loading guidance video then pause
                var $editorContent = $(evt.editor.window.getFrame().$).contents();

                if ($editorContent.find('video').length) {
                    $editorContent.find('video').each(function(ind, video) {
                        video.pause();
                    });
                }
            }
        }
    });
}
//create instants for guidance and rationales
function createContentMessage(idMessage) {
    var idContentGuidance = 'guidance_' + idMessage;
    var idContentRationale = 'rationale_' + idMessage;
    var idContentGuidanceRation = 'guidance_rationale_' + idMessage;

    idtypeGuidanceMessage = idContentGuidance;
    idtypeRationaleMessage = idContentRationale;
    idtypeGuidanceRationaleMessage = idContentGuidanceRation;
    
    var itemGuidance = new typeMessage(idContentGuidance, 'guidance', false);
    var itemRationale = new typeMessage(idContentRationale, 'rationale', false);
    var itemGuidanceRationale = new typeMessage(idtypeGuidanceRationaleMessage, 'guidance_rationale', false);
    
    var htmlContentGuidance = itemGuidance.render();
    var htmlContentRationale = itemRationale.render();
    var htmlContentGuidanceRationale = itemGuidanceRationale.render();

    $('#student_guidance_' + idMessage).css('padding-bottom', '10px').html(htmlContentGuidance).hide();
    createMessageCKEditor(idContentGuidance);
    resetToolbarMessage(idContentGuidance);

    $('#teacher_rationale_' + idMessage).css('padding-bottom', '10px').html(htmlContentRationale).hide();
    createMessageCKEditor(idContentRationale);
    resetToolbarMessage(idContentRationale);
    
    $('#st_guidance_rationale_' + idMessage).css('padding-bottom', '10px').html(htmlContentGuidanceRationale).hide();
    createMessageCKEditor(idContentGuidanceRation);
    resetToolbarMessage(idContentGuidanceRation);
 
}
//create item guidance ,rationale and guidance_rationale
function resetToolbarMessage(idContentMessage) {
    $('#messageTop_' + idContentMessage + ' > div').hide();
    $('#messageTop_' + idContentMessage + ' > div:last-child').show();
    $('#messageContent_' + idContentMessage + ' .content > div').css('width', '100%');
    $('#messageContent_' + idContentMessage + ' .content').find('iframe[allowtransparency]').css('width', '100%');
    $('#messageBot_' + idContentMessage).hide();
}
//reset body content editor for guidance and rationales
function resetBodyContentEditor() {
    $('.content_student_guidance .cke_button__bold').removeClass('cke_button_on').removeAttr('aria-pressed');
    $('.content_teacher_rationale .cke_button__bold').removeClass('cke_button_on').removeAttr('aria-pressed');
    
    $('.content_student_guidance .cke_button__italic').removeClass('cke_button_on').removeAttr('aria-pressed');
    $('.content_teacher_rationale .cke_button__italic').removeClass('cke_button_on').removeAttr('aria-pressed');
    
    $('.content_student_guidance .cke_button__underline').removeClass('cke_button_on').removeAttr('aria-pressed');
    $('.content_teacher_rationale .cke_button__underline').removeClass('cke_button_on').removeAttr('aria-pressed');
    
    $('.content_student_guidance .cke_combo__fontsize').find('.cke_combo_text').text('Normal');
    $('.content_teacher_rationale .cke_combo__fontsize').find('.cke_combo_text').text('Normal');
    
    if (window.playsound != undefined) {
        window.playsound.pause();
    }
}
//reset body content editor for guidance rationales
function resetBodyContentGuidanceRationale() {
    var guidanceRationaleContent = $('.content_st_guidance_rationale .content').find('iframe[allowtransparency]');

    guidanceRationaleContent.contents().find('body').html('');
    $('.content_st_guidance_rationale .cke_button__bold').removeClass('cke_button_on').removeAttr('aria-pressed');
    $('.content_st_guidance_rationale .cke_button__italic').removeClass('cke_button_on').removeAttr('aria-pressed');
    $('.content_st_guidance_rationale .cke_button__underline').removeClass('cke_button_on').removeAttr('aria-pressed');
    $('.content_st_guidance_rationale .cke_combo__fontsize').find('.cke_combo_text').text('Normal');
    
    $('.questionTypeGuidance').hide();
    $('.audioRef_' + idtypeGuidanceRationaleMessage).text('');
    
    if (window.playsound != undefined) {
        window.playsound.pause();
    }
}
//save item store message guidance

function itemStoreMessage(iMessageTempEdit, arrMessageTemp, typeMessageGuidance, objMessage) {
    var hasItemGuidance = false;
    if (iMessageTempEdit.length) {
        for (var i = 0, leniMessageTempEdit = iMessageTempEdit.length; i < leniMessageTempEdit; i++) {
            var itemMessageTempEdit = iMessageTempEdit[i];
            if (itemMessageTempEdit.idTemp === idMessage) {
                hasItemGuidance = true;
                if (itemMessageTempEdit.arrMessage.length) {
                    for (var j = 0, lenArrayMessage = itemMessageTempEdit.arrMessage.length; j < lenArrayMessage; j++) {
                        if (itemMessageTempEdit.arrMessage[j].typeMessage === typeMessageGuidance) {
                            itemMessageTempEdit.arrMessage.splice(j, 1);
                            break;
                        }
                    }
                }

                if (objMessage.audioRef != '' || objMessage.valueContent != '') {
                    itemMessageTempEdit.arrMessage.push(objMessage);
                    break;
                }
            }
            
        }
        //----------------------
        for (var i = 0, leniMessageTempEdit = iMessageTempEdit.length; i < leniMessageTempEdit; i++) {
            var itemMessageTempEdit = iMessageTempEdit[i];
            if (itemMessageTempEdit.idTemp !== idMessage && !hasItemGuidance) {
                arrMessageTemp.push(objMessage);
                iMessageTempEdit.push({
                    idTemp: idMessage,
                    arrMessage: arrMessageTemp
                });
                break;
            }
        }
        

    } else {
        arrMessageTemp.push(objMessage);
        iMessageTempEdit.push({
            idTemp: idMessage,
            arrMessage: arrMessageTemp
        });
        
    }
}
/***
* build type message
***/
function typeMessage(id, typemessage, isCheckboxed) {
    this.id = id;
    this.typemessage = typemessage;
    this.isCheckboxed = isCheckboxed;
}
typeMessage.prototype.render = function () {
    var htmlString = '';
    
    htmlString += '<div class="clear"></div>';
    htmlString += '<div id="messageTop_' + this.id + '"></div>';
    htmlString += '<div class="questionType questionTypeGuidance" id="questionType_' + this.id + '">';
    htmlString += '     <ul>';
    htmlString += '        <li>';
    htmlString += '         <div class="audio">';
    htmlString += '			  <div class="audioRemove_">';
    htmlString += '				<img alt="Play audio" class="bntPlay" src="' + CKEDITOR.plugins.getImgByVersion('multiplechoice', 'images/small_audio_play.png') + '" title="Play audio" />';
    htmlString += '				<img alt="Stop audio" class="bntStop" src="' + CKEDITOR.plugins.getImgByVersion('multiplechoice', 'images/small_audio_stop.png') + '" title="Stop audio" />';
    htmlString += '				<input type="button" value="Remove audio" class="ckbutton removeAudio cke_toolgroup" />';
    htmlString += '				<span class="audioRef audioRef_' + this.id + '"></span>';
    htmlString += '			  </div>';
    htmlString += '		    </div><div class="clear"></div>';
    htmlString += '	       </li>';
    htmlString += '     </ul><div class="clear"></div>';
    htmlString += '</div>';
    htmlString += '<div class="clear"></div>';
    htmlString += '<div id="messageContent_'+ this.id +'">';
    htmlString += '<div class="content">';
    htmlString += '  <textarea cols="50" id="textarea_' + this.id + '" name="editor1" rows="1" ></textarea>';
    htmlString += '</div>';
    htmlString += '</div>';
    htmlString += '<div class="clear"></div>';
    htmlString += '<div id="messageBot_' + this.id + '"></div>';
    htmlString += '<div class="clear"></div>';
    
    return htmlString;
};
