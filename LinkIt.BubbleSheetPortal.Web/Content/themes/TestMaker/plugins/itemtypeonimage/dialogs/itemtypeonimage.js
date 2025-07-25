var iframe;
var iframeId;

CKEDITOR.dialog.add('itemTypeOnImage', function (editor) {
    var lang = editor.lang.itemtypeonimage;
    var $itemtypeonimagePreview;

    iSchemeID = parseInt(iSchemeID, 10);

    /**
     * Get html background image
     * @param  {[type]} data [description]
     * @return {[type]}      [description]
     */
    var itemtypeonimageHtml = function (data) {
        var imageUrl = data.imageUrl;
        var percent = parseInt(data.percent, 10);
        var originalWidth = parseInt(data.originalWidth, 10);
        var originalHeight = parseInt(data.originalHeight, 10);
        var width = originalWidth * percent / 10;
        var height = originalHeight * percent / 10;

        var mask = '<img class="cke_reset cke_widget_mask itemtypeonimageMark" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D"/>';

        var div = '<div class="itemtypeonimage" contenteditable="false" style="';
        div += 'width: ' + width + 'px;';
        div += 'height: ' + height + 'px;';
        div += 'position: relative;';
        div += '">';
        div += mask;
        div += '<img class="itemtypeonimageMarkObject" ';
        div += 'data-percent="' + percent + '" ';
        div += 'data-original-width="' + originalWidth + '" ';
        div += 'data-original-height="' + originalHeight + '" ';
        div += 'width="' + width + '" ';
        div += 'height="' + height + '" ';
        div += 'src="' + imageUrl + '"/>';
        div += '</div>';

        return div;
    };

    /**
     * Reset item type: inline choice and text entry (fill in the blank)
     * @param {[type]} editor [description]
     */
    var itemtypeonimageReset = function (editor) {
        var $editor = $(editor);

        $editor.find('.inlineChoiceInteraction')
                .not(':visible')
                .parent('.smallText, .normalText, .largeText, .veryLargeText')
                .remove();
        $editor.find('.inlineChoiceInteraction').not(':visible').remove();
        $editor.find('.textEntryInteraction').not(':visible').remove();
        $editor.find('.inlineChoiceInteraction .itemtypeonimage-tooltip').remove();
        $editor.find('.textEntryInteraction .itemtypeonimage-tooltip').remove();
        $editor.find('.inlineChoiceInteraction .remove-item').remove();
        $editor.find('.textEntryInteraction .remove-item').remove();

        window.dialogItemtypeonimage = false;
        window.editItemtypeonimage = false;
    };

    /**
     * Show toolbar button if here doesn't contain inline choice or text entry
     * @param  {[type]} editor [description]
     * @return {[type]}        [description]
     */
    var itemtypeonimageToolbar = function (editor) {
        var $editor = $(editor);

        if (iSchemeID === 8 && !$editor.find('.inlineChoiceInteraction').length) {
            $('.cke_button__inlinechoice').parents('span.cke_toolbar').show();
        } else if (iSchemeID === 9 && !$editor.find('.textEntryInteraction').length) {
            $('.cke_button__textentry').parents('span.cke_toolbar').show();
        }
    };

    /**
     * Item on type image sub for multi-part
     * @type {String}
     */
    var itemtypeonimageSubpartSize = '150px';
    var itemtypeonimageSubpart = {
        type: 'hbox',
        children: [
            {
                type: 'select',
                id: 'subpart',
                label: lang.subpart,
                items: [
                    ['Add new sub-part', '-1'],
                    ['Inline Choice', '8'],
                    ['Fill-in-the-Blank', '9']
                ],
                'default': '-1',
                className: 'itemtypeonimage__field',
                onChange: function () {
                    var $bgImage = $itemtypeonimagePreview.find('div');
                    var self = this;
                    var subpart = parseInt(this.getValue(), 10);

                    if (!$bgImage.length && subpart !== -1) {
                        self.setValue('-1');
                        customAlert('Please upload image first.');
                        return;
                    }

                    if (subpart === 8) {
                        setTimeout(function () {
                            editor.execCommand('insertInlineChoice');
                            self.setValue('-1');
                        }, 100);
                        editor.fire('saveSnapshot');
                    } else if (subpart === 9) {
                        setTimeout(function () {
                            editor.execCommand('insertTextEntry');
                            self.setValue('-1');
                        }, 100);
                        editor.fire('saveSnapshot');
                    }
                }
            }
        ]
    };

    // Item type on image subpart for inline choice and fil in the blank
    if (iSchemeID === 8 || iSchemeID === 9) {
        itemtypeonimageSubpart = {
            type: 'hbox',
            children: {
                type: 'html',
                html: '\
                    <div class="itemtypeonimage__field">\
                    </div>'
            }
        };

        itemtypeonimageSubpartSize = '1px';
    }

    var itemtypeonimageMainPreview = {
        type: 'vbox',
        padding: 0,
        children: [
            {
                type: 'hbox',
                children: [
                    {
                        type: 'html',
                        html: '\
                            <div class="itemtypeonimage__field">\
                                <div class="itemtypeonimage__preview" id="itemtypeonimagePreview"></div>\
                            </div>'
                    }
                ]
            }
        ]
    };

    // Item type image main preview for multi-part
    if (iSchemeID === 21) {
        itemtypeonimageMainPreview = {
            type: 'vbox',
            padding: 0,
            children: [
                {
                    type: 'hbox',
                    widths: ['150px', '350px'],
                    children: [
                        {
                            type: 'html',
                            html: '\
                                <div class="itemtypeonimage__field">\
                                    <label>Available item type</label>\
                                    <select id="itemtypeonimageSelect" class="cke_dialog_ui_input_select"></select>\
                                </div>'
                        },
                        {
                            type: 'html',
                            html: '\
                                <div class="itemtypeonimage__field">\
                                    <div class="itemtypeonimage__preview" id="itemtypeonimagePreview"></div>\
                                </div>'
                        }
                    ]
                }
            ]
        };
    }

    /**
     * Item type image panel
     * @type {Object}
     */
    var itemtypeonimagePanel = {
        id: 'itemtypeonimage',
        title: lang.title,
        label: lang.label,
        elements: [
            {
                type: 'vbox',
                padding: 0,
                children: [
                    {
                        type: 'hbox',
                        widths: ['80px', '80px', itemtypeonimageSubpartSize, '320px'],
                        align: 'right',
                        children: [
                            {
                                type: 'select',
                                id: 'percent',
                                label: lang.percent,
                                items: [
                                    ['10%', '1'],
                                    ['20%', '2'],
                                    ['30%', '3'],
                                    ['40%', '4'],
                                    ['50%', '5'],
                                    ['60%', '6'],
                                    ['70%', '7'],
                                    ['80%', '8'],
                                    ['90%', '9'],
                                    ['100%', '10']
                                ],
                                'default': '10',
                                className: 'itemtypeonimage__field',
                                onChange: function () {
                                    var $bgImage = $itemtypeonimagePreview.find('div');
                                    var $bgImageObject = $bgImage.find('.itemtypeonimageMarkObject');
                                    var percent = parseInt(this.getValue(), 10);
                                    var originalWidth = parseInt($bgImageObject.attr('data-original-width'), 10);
                                    var originalHeight = parseInt($bgImageObject.attr('data-original-height'), 10);
                                    var width = Math.round(originalWidth * percent / 10);
                                    var height = Math.round(originalHeight * percent / 10);

                                    $bgImage.css({
                                        width: width + 'px',
                                        height: height + 'px'
                                    });

                                    $bgImageObject.attr({
                                        'data-percent': percent,
                                        'width': width,
                                        'height': height
                                    });

                                    $itemtypeonimagePreview.find('.inlineChoiceInteraction, .textEntryInteraction').each(function (ind, itemtype) {
                                        var $itemtype = $(itemtype);
                                        var itemtypeTop = parseInt($itemtype.attr('data-top'), 10);
                                        var itemtypeLeft = parseInt($itemtype.attr('data-left'), 10);
                                        var itemtypeNewTop = Math.round(itemtypeTop * percent / 10);
                                        var itemtypeNewLeft = Math.round(itemtypeLeft * percent / 10);

                                        $itemtype.css({ 'top': itemtypeNewTop + 'px', 'left': itemtypeNewLeft + 'px' });
                                    });
                                }
                            },
                            {
                                type: 'select',
                                id: 'float',
                                label: lang.float,
                                items: [
                                    ['None', 'none'],
                                    ['Left', 'left'],
                                    ['Right', 'right']
                                ],
                                'default': 'none',
                                className: 'itemtypeonimage__field',
                                onChange: function () {
                                    var $bgImage = $itemtypeonimagePreview.find('div');
                                    var $bgImageObject = $bgImage.find('.itemtypeonimageMarkObject');
                                    var float = this.getValue();

                                    $bgImage.css('float', float);

                                    $bgImageObject
                                        .css('float', float)
                                        .attr('float', float);
                                }
                            },
                            itemtypeonimageSubpart,
                            {
                                type: 'hbox',
                                children: [
                                    {
                                        type: 'html',
                                        html: '\
                                            <div class="itemtypeonimage__field">\
                                                <label>' + lang.uploadFile + '</label>\
                                                <div class="itemtypeonimage__uploadfile">\
                                                    <form name="itemtypeonimageForm" id="itemtypeonimageForm" method="POST" enctype="multipart/form-data">\
                                                        <input type="file" name="file" id="itemtypeonimageFormField" value="Upload"/>\
                                                        <input type="hidden" name="id" id="objectId" />\
                                                    </form>\
                                                    <a href="javascript:void(0)" id="itemtypeonimageButton" class="cke_dialog_ui_button" role="button" hidefocus="true">\
                                                        <span class="cke_dialog_ui_button">Upload</span>\
                                                    </a>\
                                                </div>\
                                            </div>'
                                    }
                                ]
                            }
                        ]
                    }
                ]
            },
            itemtypeonimageMainPreview
        ]
    };

    /**
     * Item type on image draggable element
     * @param  {[type]} element [description]
     * @return {[type]}         [description]
     */
    var itemtypeonimageDraggable = function (element) {
        var $element = $(element);

        $element.draggable({
            containment: 'parent',
            drag: function (event, ui) {
                var $target = $(event.target);

                $target.attr('data-top', ui.position.top);
                $target.attr('data-left', ui.position.left);
            }
        });
    };

    /**
     * Item type on image remove element
     * @param  {[type]} element [description]
     * @return {[type]}         [description]
     */
    var itemtypeonimageRemove = function (element) {
        var $element = $(element);

        $element.find(".remove-item").on("click", function () {
            var item = $(this).parent();
            var resId = item.attr("id");
            var select = $(this).parents(".cke_dialog_contents").find("#itemtypeonimageSelect");

            item.remove();
            select.append(new Option(resId, resId));

            //add to the list again.
        });
    };

    /**
     * Init item type on image position
     * @param  {[type]} element [description]
     * @return {[type]}         [description]
     */
    var itemtypeonimageInitPosition = function (preview, element) {
        var $preview = $(preview);
        var $element = $(element);
        var w = parseInt($preview.find('.itemtypeonimageMarkObject').attr('width'), 10);
        var h = parseInt($preview.find('.itemtypeonimageMarkObject').attr('height'), 10);
        var offsetTop = h / 2 - 10;
        var offsetLeft = w / 2 - 45;

        $element
            .css({'left': offsetLeft + 'px', 'top': offsetTop + 'px'})
            .attr({'data-left': offsetLeft, 'data-top': offsetTop});
    };

    /**
     * Item type on image change image
     * @param  {[type]} preview [description]
     * @param  {[type]} element [description]
     * @return {[type]}         [description]
     */
    var itemtypeonimageChangeImage = function (preview, elements) {
        var $preview = $(preview);
        var $elements = $(elements);
        var $container = $('<div/>');
        var clone;

        elements.each(function (ind, element) {
            var $element = $(element);
            $container.append($element.clone(true));
        });

        clone = $container.html();

        return clone;
    };

    /**
     * Clone item by classes and add remove icon
     * @param  {[type]} preview [description]
     * @param  {[type]} element [description]
     * @param  {[type]} classes [description]
     * @return {[type]}         [description]
     */
    var itemtypeonimageCloneItem = function (preview, element, classes) {
        var $preview = $(preview);
        var $element = $(element);
        var $container = $('<div/>');
        var cloneHtml = itemtypeonimageChangeImage($preview, $element);
        var removeTooltipHtml = '';
        var clone;

        $container.append(cloneHtml);

        if (iSchemeID === 21) {
            removeTooltipHtml += '<span class="itemtypeonimage-tooltip">';
            removeTooltipHtml += iResult.length + 1;
            removeTooltipHtml += '</span>';
        }

        removeTooltipHtml += '<span class="remove-item"></span>';

        if (classes === '.inlineChoiceInteraction') {
            $container.find('.inlineChoiceInteraction').append(removeTooltipHtml);
        } else if (classes === '.textEntryInteraction') {
            $container.find('.textEntryInteraction').append(removeTooltipHtml);
        }

        clone = $container.html();

        return clone;
    };

    /**
     * onShow multi-part
     * @param  {[type]} dialog [description]
     * @return {[type]}        [description]
     */
    var onShowMultipart = function (dialog) {
        var $editorBody = $(editor.document.getBody().$);
        var dialogDocument = dialog.getElement().getDocument();
        var $itemtypeonimageSelect = $(dialogDocument.getById('itemtypeonimageSelect').$);
        var itemtypeOptions = [];
        var isSupportItemTypeOnImage = $editorBody.find('.inlineChoiceInteraction').not('.itemtypeonimage .inlineChoiceInteraction').length ||
                                        $editorBody.find('.textEntryInteraction').not('.itemtypeonimage .textEntryInteraction').length;

        $itemtypeonimageSelect.empty().html('');

        // Check item type inline choice and fill in blank
        $editorBody.find('.inlineChoiceInteraction, .textEntryInteraction').each(function (ind, itemtype) {
            var $itemtype = $(itemtype);
            var itemtypeId = $itemtype.attr('id');

            if ($(this).parents(".itemtypeonimage").length === 0) {
                itemtypeOptions.push(itemtypeId);
            }

            $itemtype.append('<span class="itemtypeonimage-tooltip">' + itemtypeId.replace("RESPONSE_", "") + '</span>');
        });

        if (isSupportItemTypeOnImage) {
            $itemtypeonimageSelect.show();

            // Add option item type inline choice and fill in the blank
            if (itemtypeOptions.length) {
                var itemtypeonimageSelect = $itemtypeonimageSelect.get(0);

                itemtypeonimageSelect.options[itemtypeonimageSelect.options.length] = new Option('Select Response', '-1');

                for (var i = 0, len = itemtypeOptions.length; i < len; i++) {
                    var itemtypeOption = itemtypeOptions[i];
                    itemtypeonimageSelect.options[itemtypeonimageSelect.options.length] = new Option(itemtypeOption, itemtypeOption);
                }

                // Detect change item type on select response exists
                $itemtypeonimageSelect.on('change', function () {
                    var $select = $(this);
                    var selectVal = $select.val();
                    var $itemtypeSelected;

                    if (selectVal === '-1') {
                        return;
                    }

                    if (!$itemtypeonimagePreview.find('.itemtypeonimage').length) {
                        $select.val('-1');
                        customAlert('Please upload image first.');
                        return;
                    }

                    if ($editorBody.find('.inlineChoiceInteraction[id="' + selectVal + '"]').length) {
                        var cloneInlinechoice = '';
                        $itemtypeSelected = $editorBody.find('.inlineChoiceInteraction[id="' + selectVal + '"]');
                        cloneInlinechoice = $itemtypeSelected.clone(true);
                        cloneInlinechoice.append('<span class="remove-item"></span>');
                        $itemtypeonimagePreview.find('.itemtypeonimage').append(cloneInlinechoice);
                        itemtypeonimageRemove($itemtypeonimagePreview);
                        itemtypeonimageInitPosition($itemtypeonimagePreview, $itemtypeonimagePreview.find('.itemtypeonimage .inlineChoiceInteraction[id="' + selectVal + '"]'));
                        itemtypeonimageDraggable($itemtypeonimagePreview.find('.itemtypeonimage .inlineChoiceInteraction[id="' + selectVal + '"]'));
                        $select.find('option:selected').remove();
                        $select.val('-1');
                    } else if ($editorBody.find('.textEntryInteraction[id="' + selectVal + '"]').length) {
                        var cloneTextentry = '';
                        $itemtypeSelected = $editorBody.find('.textEntryInteraction[id="' + selectVal + '"]');
                        cloneTextentry = $itemtypeSelected.clone(true);
                        cloneTextentry.append('<span class="remove-item"></span>');
                        $itemtypeonimagePreview.find('.itemtypeonimage').append(cloneTextentry);
                        itemtypeonimageRemove($itemtypeonimagePreview);
                        itemtypeonimageInitPosition($itemtypeonimagePreview, $itemtypeonimagePreview.find('.itemtypeonimage .textEntryInteraction[id="' + selectVal + '"]'));
                        itemtypeonimageDraggable($itemtypeonimagePreview.find('.itemtypeonimage .textEntryInteraction[id="' + selectVal + '"]'));
                        $select.find('option:selected').remove();
                        $select.val('-1');
                    }
                });
            }
        } else {
            $itemtypeonimageSelect.hide();
        }
    };

    /**
     * onShow inline choice
     * @param  {[type]} dialog [description]
     * @return {[type]}        [description]
     */
    var onShowInlinechoice = function (dialog) {
        var $editorBody = $(editor.document.getBody().$);
        var cloneInlinechoice = itemtypeonimageCloneItem($itemtypeonimagePreview, $editorBody.find('.inlineChoiceInteraction'), '.inlineChoiceInteraction');
        $itemtypeonimagePreview.find('.itemtypeonimage').append(cloneInlinechoice);
        itemtypeonimageRemove($itemtypeonimagePreview);
        itemtypeonimageInitPosition($itemtypeonimagePreview, $itemtypeonimagePreview.find('.itemtypeonimage .inlineChoiceInteraction'));
        itemtypeonimageDraggable($itemtypeonimagePreview.find('.itemtypeonimage .inlineChoiceInteraction'));
    };

    /**
     * onShow text entry (fill in the blank)
     * @param  {[type]} dialog [description]
     * @return {[type]}        [description]
     */
    var onShowTextentry = function (dialog) {
        var $editorBody = $(editor.document.getBody().$);
        var cloneTextentry = itemtypeonimageCloneItem($itemtypeonimagePreview, $editorBody.find('.textEntryInteraction'), '.textEntryInteraction')
        $itemtypeonimagePreview.find('.itemtypeonimage').append(cloneTextentry);
        itemtypeonimageInitPosition($itemtypeonimagePreview, $itemtypeonimagePreview.find('.itemtypeonimage .textEntryInteraction'));
        itemtypeonimageRemove($itemtypeonimagePreview);
        itemtypeonimageDraggable($itemtypeonimagePreview.find('.itemtypeonimage .textEntryInteraction'));
    };

    return {
        title: lang.title,
        maxWidth: 700,
        minWidth: IS_V2 ? 700 : 600,
        minHeight: 200,
        contents: [
            itemtypeonimagePanel
        ],
        onLoad: function () {
            var dialog = this;
            var dialogDocument = dialog.getElement().getDocument();
            var itemtypeonimageFormField = dialogDocument.getById('itemtypeonimageFormField');

            $itemtypeonimagePreview = $(dialogDocument.getById('itemtypeonimagePreview').$);

            // Upload file success
            var uploadImageFileSuccess = function (data) {
                var percent = parseInt(dialog.getValueOf('itemtypeonimage', 'percent'), 10);
                var div = '';
                var data = {
                    imageUrl: data.src,
                    percent: percent,
                    originalWidth: data.width,
                    originalHeight: data.height
                };

                div = itemtypeonimageHtml(data);

                if ($itemtypeonimagePreview.find('.itemtypeonimage .itemtypeonimageMarkObject').length) {
                    var $itemtypeonimage = $itemtypeonimagePreview.find('.itemtypeonimage');
                    var cloneInlinechoice = itemtypeonimageCloneItem($itemtypeonimagePreview, $itemtypeonimage.find('.inlineChoiceInteraction'), '.inlineChoiceInteraction');
                    var cloneTextentry = itemtypeonimageCloneItem($itemtypeonimagePreview, $itemtypeonimage.find('.textEntryInteraction'), '.textEntryInteraction');
                    $itemtypeonimagePreview.html(div);
                    $itemtypeonimagePreview.find('.itemtypeonimage').append(cloneInlinechoice);
                    $itemtypeonimagePreview.find('.itemtypeonimage').append(cloneTextentry);
                    itemtypeonimageRemove($itemtypeonimagePreview);
                    itemtypeonimageDraggable($itemtypeonimagePreview.find('.itemtypeonimage .inlineChoiceInteraction'));
                    itemtypeonimageDraggable($itemtypeonimagePreview.find('.itemtypeonimage .textEntryInteraction'));
                } else {
                    var $editorBody = $(editor.document.getBody().$);
                    $itemtypeonimagePreview.html(div);

                    if (iSchemeID === 8) {
                        var cloneInlinechoice = itemtypeonimageCloneItem($itemtypeonimagePreview, $editorBody.find('.inlineChoiceInteraction'), '.inlineChoiceInteraction');
                        $itemtypeonimagePreview.find('.itemtypeonimage').append(cloneInlinechoice);
                        itemtypeonimageRemove($itemtypeonimagePreview);
                        itemtypeonimageInitPosition($itemtypeonimagePreview, $itemtypeonimagePreview.find('.itemtypeonimage .inlineChoiceInteraction'));
                        itemtypeonimageDraggable($itemtypeonimagePreview.find('.itemtypeonimage .inlineChoiceInteraction'));

                        // Initialize default value when create new item type on image inline choice
                        if (!$editorBody.find('.inlineChoiceInteraction').length) {
                            setTimeout(function () {
                                editor.execCommand('insertInlineChoice');
                            }, 100);
                            editor.fire('saveSnapshot');
                        }
                    } else if (iSchemeID === 9) {
                        var cloneTextentry = itemtypeonimageCloneItem($itemtypeonimagePreview, $editorBody.find('.textEntryInteraction'), '.textEntryInteraction')
                        $itemtypeonimagePreview.find('.itemtypeonimage').append(cloneTextentry);
                        itemtypeonimageRemove($itemtypeonimagePreview);
                        itemtypeonimageInitPosition($itemtypeonimagePreview, $itemtypeonimagePreview.find('.itemtypeonimage .textEntryInteraction'));
                        itemtypeonimageDraggable($itemtypeonimagePreview.find('.itemtypeonimage .textEntryInteraction'));

                        // Initialize default value when create new item type on image fill in the blank
                        if (!$editorBody.find('.textEntryInteraction').length) {
                            setTimeout(function () {
                                editor.execCommand('insertTextEntry');
                            }, 100);
                            editor.fire('saveSnapshot');
                        }
                    }
                }
            };

            // Detect upload file
            itemtypeonimageFormField.on('change', function () {
                var $self = $(this.$);
                var self = $self.get(0);
                var file = self.value;
                var extension = (file.substr(file.lastIndexOf('.') + 1)).toLowerCase();
                var characterException = ['&', '#'];
                var fileExtensions = ['jpg', 'jpeg', 'png', 'svg'];

                if (file === '') {
                    customAlert('Please select image file');
                    return;
                }

                var isCorrectFormat = _.some(fileExtensions, function(ext) {
                    return ext === extension;
                });

                if (!isCorrectFormat) {
                    var fileExts = fileExtensions.join(', ');
                    var fileExtsMsg = 'Unsupported file type. Please select ' + fileExts + ' file.';
                    customAlert(fileExtsMsg);
                    return;
                }

                $self.next('input[name="id"]').val(objectId);
                uploadImageFile($self.parent().get(0), 100, imgUpload, self, uploadImageFileSuccess);
            });
        },
        onShow: function () {
            var dialog = this;
            var dialogDocument = dialog.getElement().getDocument();
            var $itemtypeonimageFormField = $(dialogDocument.getById('itemtypeonimageFormField').$);
            var isEdit = window.editItemtypeonimage;

            window.dialogItemtypeonimage = true;

            $itemtypeonimageFormField.parents('form').get(0).reset();

            if (iSchemeID === 21) {
                onShowMultipart(dialog);
            }

            $itemtypeonimagePreview.html('');

            if (isEdit) {
                var $elementItemTypeOnImage = $(editor.elementItemTypeOnImage.$);
                var elementItemTypeOnImageClone = $elementItemTypeOnImage.clone(true);
                var dataPercent = $elementItemTypeOnImage.find('.itemtypeonimageMarkObject').attr('data-percent');
                var dataFloat = $elementItemTypeOnImage.find('.itemtypeonimageMarkObject').attr('float');

                dialog.setValueOf('itemtypeonimage', 'percent', dataPercent);
                dialog.setValueOf('itemtypeonimage', 'float', dataFloat ? dataFloat : 'none');

                $itemtypeonimagePreview.html(elementItemTypeOnImageClone);

                var itemType = $itemtypeonimagePreview.find('.itemtypeonimage').find('.inlineChoiceInteraction, .textEntryInteraction');

                itemType.append('<span class="remove-item"></span>');
                itemType.draggable({
                    containment: 'parent',
                    drag: function (event, ui) {
                        var $target = $(event.target);

                        $target.attr('data-top', ui.position.top);
                        $target.attr('data-left', ui.position.left);
                    }
                });

                itemtypeonimageRemove($itemtypeonimagePreview);

                // Initialize for single item type
                if (iSchemeID === 8 && !$itemtypeonimagePreview.find('.itemtypeonimage .inlineChoiceInteraction').length) {
                    onShowInlinechoice(dialog);
                } else if (iSchemeID === 9 && !$itemtypeonimagePreview.find('.itemtypeonimage .textEntryInteraction').length) {
                    onShowTextentry(dialog);
                }
            } else {
                dialog.setValueOf('itemtypeonimage', 'percent', '10');
                dialog.setValueOf('itemtypeonimage', 'float', 'none');
            }
        },
        onOk: function () {
            var $editorBody = $(editor.document.getBody().$);
            var itemtypeonimageHtml = '';
            var isEdit = window.editItemtypeonimage;

            // Remove ui-draggable
            $itemtypeonimagePreview
                .find('.inlineChoiceInteraction, .textEntryInteraction')
                .removeClass('ui-draggable');

            $itemtypeonimagePreview.find('.inlineChoiceInteraction, .textEntryInteraction').each(function (ind, item) {
                var $item = $(item);

                if (typeof $item.attr('data-top') !== 'undefined' || typeof $item.attr('data-left') !== 'undefined') {
                    $item.css({ position: 'absolute' });
                }
            });

            if ($itemtypeonimagePreview.find('.itemtypeonimage').length) {
                $editorBody.find('.inlineChoiceInteraction, .textEntryInteraction').each(function () {
                    var $itemtype = $(this);
                    var itemtypeId = $itemtype.attr('id');
                    var isExist = false;

                    $itemtypeonimagePreview.find('.inlineChoiceInteraction, .textEntryInteraction').each(function () {
                        var $itemtypeOnImage = $(this);
                        var itemtypeOnImageId = $itemtypeOnImage.attr('id');

                        if (itemtypeId === itemtypeOnImageId) {
                            isExist = true;
                        }
                    });

                    if (isExist) {
                        $itemtype.remove();
                    }
                });
            }

            // Update data attribute of inline choice and text entry
            $itemtypeonimagePreview.find('.inlineChoiceInteraction, .textEntryInteraction').each(function (ind, itemtype) {
                var $itemtype = $(itemtype);
                var itemtypeTop = $itemtype.css('top').replace('px', '');
                var itemtypeLeft = $itemtype.css('left').replace('px', '');

                $itemtype.attr({ 'data-top': itemtypeTop, 'data-left': itemtypeLeft });
            });

            // Update item type
            if (isEdit) {
                var $elementItemTypeOnImage = $(editor.elementItemTypeOnImage.$);
                var $elementItemTypeOnImageObject = $itemtypeonimagePreview.find('.itemtypeonimage. .itemtypeonimageMarkObject');

                itemtypeonimageHtml = $itemtypeonimagePreview.find('.itemtypeonimage').html() + '&nbsp;';

                $elementItemTypeOnImage.css({
                    width: $elementItemTypeOnImageObject.width() + 'px',
                    height: $elementItemTypeOnImageObject.height() + 'px',
                    float: $elementItemTypeOnImageObject.attr('float')
                });

                $elementItemTypeOnImage.attr('float', $elementItemTypeOnImageObject.attr('float'));

                $elementItemTypeOnImage.html(itemtypeonimageHtml);
            } else {
                itemtypeonimageHtml = $itemtypeonimagePreview.html() + '&nbsp;'

                editor.insertHtml(itemtypeonimageHtml);
            }

            itemtypeonimageReset($(editor.document.$).find('body'));
            itemtypeonimageToolbar($(editor.document.$).find('body'));
        },
        onCancel: function () {
            itemtypeonimageReset($(editor.document.$).find('body'));
            itemtypeonimageToolbar($(editor.document.$).find('body'));
        }
    }
});

/**
 * Upload file
 * @param  {[type]}   form               [description]
 * @param  {[type]}   destinationPercent [description]
 * @param  {[type]}   action_url         [description]
 * @param  {[type]}   currentElement     [description]
 * @param  {Function} callback           [description]
 * @return {[type]}                      [description]
 */
function uploadImageFile(form, destinationPercent, action_url, currentElement, callback) {
    var $body = $('body');

    $body.ckOverlay();

    // Create the iframe
    iframe = document.createElement('iframe');
    iframe.setAttribute('id', 'upload_iframe');
    iframe.setAttribute('name', 'upload_iframe');
    iframe.setAttribute('width', '0');
    iframe.setAttribute('height', '0');
    iframe.setAttribute('border', '0');
    iframe.setAttribute('style', 'width: 0;height: 0;border: none;');

    // Add to document
    form.parentNode.appendChild(iframe);

    iframeId = document.getElementById('upload_iframe');

    // Add event
    var eventHandler = function () {
        var content = '';

        if (iframeId.detachEvent) {
            iframeId.detachEvent('onload', eventHandler);
        } else {
            iframeId.removeEventListener('load', eventHandler, false);
        }

        // Message from server
        if (iframeId.contentDocument) {
            content = iframeId.contentDocument.body.innerHTML;
        } else if (iframeId.contentWindow) {
            content = iframeId.contentWindow.document.body.innerHTML;
        } else if (iframeId.document) {
            content = iframeId.document.body.innerHTML;
        }

        var data = $.parseJSON(content.substr(content.indexOf('{'), content.lastIndexOf('}') - content.indexOf('{') + 1));

        // Del the iframe
        setTimeout(removeIFrame, 250);

        var reader = new FileReader();
        var file = $(form).find('input[type="file"]').get(0).files[0];

        reader.readAsDataURL(file);

        reader.onload = function() {
            var img = new Image();

            img.src = reader.result;

            img.onload = function () {
                var result = {};

                result.src = data.absoluteUrl;
                result.width = this.width;
                result.height = this.height;

                callback(result);
            };
        };

        // Hide overlay
        $body.ckOverlay.destroy();
    };

    if (iframeId.addEventListener) {
        iframeId.addEventListener('load', eventHandler, true);
    }

    if (iframeId.attachEvent) {
        iframeId.attachEvent('onload', eventHandler);
    }

    // Set properties of form
    form.setAttribute('target', 'upload_iframe');
    form.setAttribute('action', action_url);
    form.setAttribute('method', 'post');
    form.setAttribute('enctype', 'multipart/form-data');
    form.setAttribute('encoding', 'multipart/form-data');

    // Submit the form
    form.submit();
}

function removeIFrame() {
    //Check iFrame and only remove iframe has existed
    if (iframeId !== null && iframeId.parentNode !== null) {
        iframeId.parentNode.removeChild(iframeId);
    }
}
