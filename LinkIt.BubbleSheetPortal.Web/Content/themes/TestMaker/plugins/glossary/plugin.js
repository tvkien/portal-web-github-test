CKEDITOR.plugins.add('glossary', {
    hidpi: true, // %REMOVE_LINE_CORE%
	icons: 'glossary',
    isEdit: false,
    currentElement: '',
    init: function (editor) {

        var editorName = null;
		var config = editor.config;
        var styles = {};
		var allowedContent = [];
        var currentPartialCreditResId = '';
        var $btnDeleteGlossary = null;
        var $editor = null;
        var $dialog = null;
        var glossaryPlugin = editor.plugins.glossary;
        isEdit = glossaryPlugin.isEdit;
        currentElement = glossaryPlugin.currentElement;

        editor.on('stylesSet', function (evt) {

            var stylesDefinitions = evt.data.styles;

            if (!stylesDefinitions)
                return;

            var style, styleName;

            //Put all styles into an Array.
            var styleDefinition = stylesDefinitions[1];

            //Get MarkerLinkit style
            for (var i = 0; i < stylesDefinitions.length; i++) {
                if (stylesDefinitions[i].name == 'Glossary') {
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

        editor.on('contentDom', function () {
            editorName = editor.name;
            $editor = $('#cke_' + editorName + ' iframe.cke_wysiwyg_frame');
            var $btnGlossary = $('#cke_' + editorName).find('.cke_button__glossary');
            var editorContent = $editor.contents();

            editorContent.find('span.glossary').unbind('click').on('click', function (e) {
                currentElement = this;
                editor.openDialog('glossaryProperties', function() {
                    isEdit = true;
                });
            }).hover(function () {
                var $self = $(this);
                var currentID = $self.attr('glossary_id');
                editorContent.find('span.glossary[glossary_id=' + currentID + ']').addClass('glossary-hover');
            }, function () {
                var $self = $(this);
                var currentID = $self.attr('glossary_id');
                editorContent.find('span.glossary[glossary_id=' + currentID + ']').removeClass('glossary-hover');
            });

            var glossaryIcon = 'url("' + CKEDITOR.plugins.getImgByVersion('glossary', 'icons/glossary.png') + '")';
            $btnGlossary.unbind('click').hover(function() {
                var $self = $(this);
                var currentElement = editor.elementPath().elements;
                var hasMarker = false;

                if (!currentElement) {
                    return;
                }

                var sel = editor.getSelection();
                var ran = sel.getRanges(0);

                var allChildNodes = ran[0].cloneContents().$.childNodes;
                for (var i = 0; i < allChildNodes.length; i++) {
                    if (allChildNodes[i].className == 'glossary') {
                        hasMarker = true;
                    } else {
                        if (allChildNodes[i].nodeName != '#text') {
                            var childElementHasMarker = allChildNodes[i].getElementsByClassName('glossary');
                            if (childElementHasMarker.length > 0) {
                                hasMarker = true;
                            }
                        }
                    }
                }

                if (currentElement != undefined) {
                    for (var i = 0, lenElement = currentElement.length; i < lenElement; i++) {
                        var currentEl = currentElement[i];
                        if (currentEl != undefined) {
                            // Check current element contain glossary
                            if (currentElement[i].hasClass('glossary')) {
                                hasMarker = true;
                                break;
                            }
                        }
                    }
                }

                sel.selectRanges(ran);

                if (hasMarker) {
                    $self.attr('title', 'Delete Definition');
                    $self.find('.cke_button__glossary_icon').css({
                        'background-image': 'url("' + CKEDITOR.plugins.getImgByVersion('glossary', 'icons/glossary-remove.png') + '")'
                    });
                } else {
                    $self.attr('title', 'Insert Definition');
                    $self.find('.cke_button__glossary_icon').css({
                        'background-image': glossaryIcon
                    });
                }
            }, function() {
                var $self = $(this);
                $self.attr('title', 'Insert Definition');
                $self.find('.cke_button__glossary_icon').css({
                    'background-image': glossaryIcon
                });
            });
        });

        editor.addCommand('insertGlossary', {
            exec: function (editor) {
                var style = styles['Glossary'];
                var elementText = editor.getSelection().getSelectedText();
                var currentElement = editor.elementPath().elements;
                var hasMarker = false;

                var sel = editor.getSelection();
                var ran = sel.getRanges(0);

                var allChildNodes = ran[0].cloneContents().$.childNodes;
                for (var i = 0; i < allChildNodes.length; i++) {
                    if (allChildNodes[i].className == 'glossary') {
                        hasMarker = true;
                        var hsID = allChildNodes[i].getAttribute('glossary_id');
                        $('iframe.cke_wysiwyg_frame').contents().find('body .glossary[glossary_id=' + hsID + ']').contents().unwrap();
                    } else {
                        if (allChildNodes[i].nodeName != '#text') {
                            var childElementHasMarker = allChildNodes[i].getElementsByClassName('glossary');
                            if (childElementHasMarker.length > 0) {
                                hasMarker = true;
                                $(childElementHasMarker).each(function () {
                                    var myhsID = $(this).attr('glossary_id');
                                    $('iframe.cke_wysiwyg_frame').contents().find('body .glossary[glossary_id=' + myhsID + ']').contents().unwrap();
                                });
                            }
                        }
                    }
                }

                for (var i = 0; i < currentElement.length; i++) {
                    if (editor.elementPath().elements[i] != undefined) {
                        if (editor.elementPath().elements[i].hasClass('glossary')) {
                            hasMarker = true;
                            $('iframe.cke_wysiwyg_frame').contents().find('body .glossary[glossary_id=' + editor.elementPath().elements[i].getAttribute('glossary_id') + ']').contents().unwrap();
                        }
                    }
                }

                sel.selectRanges(ran);

                if (hasMarker) {
                    editor['removeStyle'](style);
                } else {
                    if (elementText.trim() == '') {
                        customAlert('Please highlight text to add glossary.');
                    } else {
                        editor.openDialog('glossaryProperties', function() {});
                    }
                }
            }
        });

        editor.ui.addButton('Glossary', {
            label: 'Insert Definition',
            title: 'Insert Definition',
            command: 'insertGlossary',
            toolbar: 'insertGlossary,10',
            allowedContent: allowedContent
		});


        editor.widgets.add('glossaryProperties', {
            inline: true,
            mask: true
        });

        CKEDITOR.dialog.add('glossaryProperties', function (editor) {
            var myHtml = '';

            myHtml = '<div class="property_parameters property_text">';
            myHtml += '<div class="fieldFullCredit"><textarea name="fullcreate" class="word-definition txtGlossary" rows="4" cols="50" placeholder="Define content here."></textarea></div> ';
            myHtml += '<div><button class="button-delete-glossary cke_dialog_ui_button" role="button" hidefocus="true" style="display: none;"><span class="cke_dialog_ui_button">Delete Definition</span></button></div>';
            myHtml += '</div>';

            return {
                title: 'Word Definition',
                minWidth: 350,
                minHeight: 100,
                resizable: CKEDITOR.DIALOG_RESIZE_NONE,
                contents:
                [
                    {
                        id: 'glossaryproperties',
                        label: 'Settings',
                        elements:
                        [
                            {
                                type: 'html',
                                html: myHtml,
                                onLoad: function() {
                                    editorName = editor.name;
                                    $editor = $('#cke_' + editorName + ' iframe.cke_wysiwyg_frame');
                                    $dialog = $('.cke_editor_' + editorName + '_dialog');
                                    $btnDeleteGlossary = $dialog.find('.button-delete-glossary');
                                    var editorContent = $editor.contents();

                                    $btnDeleteGlossary.unbind('click').on('click', function() {
                                        var idGL = $(currentElement).attr('glossary_id');
                                        editorContent.find('span.glossary[glossary_id=' + idGL + ']')
                                                        .contents()
                                                        .unwrap();
                                        $dialog.find('.word-definition').val('');
                                        CKEDITOR.dialog.getCurrent().hide();
                                        isEdit = false;
                                    });
                                },
                                onShow: function () {
                                    editorName = editor.name;
                                    $dialog = $('.cke_editor_' + editorName + '_dialog');
                                    $btnDeleteGlossary = $dialog.find('.button-delete-glossary');

                                    if (isEdit) {
                                        $dialog
                                            .find('.word-definition')
                                            .val(
                                                $(currentElement)
                                                    .attr('glossary')
                                                    .replace(/&lt;br\/&gt;/gi, '\n')
                                                    .replace(/&gt;/g, '>')
                                                    .replace(/&lt;/g, '<')
                                                    .replace(/&#36;/g, '$')
                                            );
                                        $btnDeleteGlossary.show();
                                    } else {
                                        $btnDeleteGlossary.hide();
                                    }
                                }
                            }
                        ]
                    }
                ],
                onOk: function () {
                    editorName = editor.name;
                    $editor = $('#cke_' + editorName + ' iframe.cke_wysiwyg_frame');
                    $dialog = $('.cke_editor_' + editorName + '_dialog');
                    var wordDefinition = $dialog
                                            .find('.word-definition')
                                            .val()
                                            .replace(/\n\r?/gi, '<br/>')
                                            .replace(/\>/g, '&gt;')
                                            .replace(/\</g, '&lt;')
                                            .replace(/\$/g, '&#36;')

					if (isEdit) {
                        var idGL = $(currentElement).attr('glossary_id');
                        $editor.contents()
                            .find('span.glossary[glossary_id=' + idGL + ']')
                            .attr({ 'glossary': wordDefinition });
					} else {
                        var style = styles['Glossary'];
                        var newID = createGlossaryID();
                        style._.definition.attributes.glossary_id = newID;
                        style._.definition.attributes.glossary = wordDefinition;
                        editor['applyStyle'](style);
					}

                    // Check input value of glossary
                    if (wordDefinition == '') {
                        customAlert('Please input word definition for glossary');
                        return false;
                    }

					var editorContent = $editor.contents();

                    editorContent.find('span.glossary').unbind('click').on('click', function (e) {
                        currentElement = this;
                        editor.openDialog('glossaryProperties', function() {
                            isEdit = true;
                        });
					}).hover(function () {
                        var $self = $(this);
                        var currentID = $self.attr('glossary_id');
                        editorContent.find('span.glossary[glossary_id=' + currentID + ']').addClass('glossary-hover');
					}, function () {
                        var $self = $(this);
                        var currentID = $self.attr('glossary_id');
                        editorContent.find('span.glossary[glossary_id=' + currentID + ']').removeClass('glossary-hover');
					});

                    //Clear content after add glossary
                    editorContent.find('span.glossary').removeClass('glossary-hover');
					$dialog.find('.word-definition').val('');
					isEdit = false;
                },
                onCancel: function () {
                    //Clear content after add glossary
                    editorName = editor.name;
                    $dialog = $('.cke_editor_' + editorName + '_dialog');

                    $dialog.find('.word-definition').val('');
                    isEdit = false;
                }
			}
        });
    }
});

/**
 * Create Glossary ID
 * @return {srcID} The identify sequence
 */
function createGlossaryID() {
    //Get all id of text hot spot
    var $allId = $('iframe.cke_wysiwyg_frame').contents().find('body .glossary');
    var srcId = 'GL_' + ($allId.size() + 1);

    for (m = 0; m < $allId.length; m++) {
        resId = 'GL_' + (m + 1);
        if ($allId.eq(m).attr('glossary_id') != resId) {

            var isOnlyOne = true;
            for (k = 0; k < $allId.length; k++) {
                if (resId == $allId.eq(k).attr('glossary_id')) {
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
