CKEDITOR.plugins.add('partialaddsourcetext', {
    lang: 'en', // %REMOVE_LINE_CORE%
    icons: 'partialaddsourcetext',
    requires: 'dialog',
    hidpi: true, // %REMOVE_LINE_CORE%
    init: function (editor) {
        var pluginName = 'insertPartialAddSourceText';
        var eleAddSourceText = "";
        var sourcePartial = [];// This is store all source of partial credit

        editor.addCommand(pluginName, new CKEDITOR.dialogCommand(pluginName));

        editor.ui.addButton('PartialAddSourceText', {
		    label: 'Source Text Label',
		    command: pluginName,
		    icon: this.path + 'icons/audio.png',
		    toolbar: 'insertPartialAddSourceText,30'
		});

        editor.widgets.add('partialaddsourcetext', {
            inline: true,
            mask: true
        });

        editor.on('doubleclick', function (evt) {
            var element = evt.data.element;

            if (element.hasClass('partialAddSourceTextMark')) {
                var parents = element.getParents();
                var parent;
                for (var i = 0; i < parents.length; i++) {
                    parent = parents[i];
                    if (parent.hasClass('partialAddSourceText')) {
                        break;
                    }
                }

                //Move selection to parent of multipleChoiceMark
                eleAddSourceText = parent;
                editor.getSelection().selectElement(eleAddSourceText);
                evt.data.dialog = pluginName;

                //Get source from iResult to partialSource
                for (var i = 0; i < iResult.length; i++) {
                    if (iResult[i].type == "partialCredit") {
                        sourcePartial = iResult[i].source;
                    }
                }

                dblickHandlerToolbar(editor);
            }
        });

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
            var checkChanged = false;
            var myhtml = '';
            myhtml = '\
                    <div id="audioUploadContent">\
                        <table cellspacing="0" border="0" align="left" style="width:100%;" role="presentation">\
                            <tbody>\
                                <tr>\
                                    <td>\
                                        Width: <input type="text" id="sourceTextWidth" class="txtFullcreate" value="120"/>&nbsp;&nbsp;&nbsp;\
                                        Height: <input type="text" id="sourceTextHeight" class="txtFullcreate" value="40"/>\
                                    </td>\
                                </tr>\
                                <tr>\
                                    <td colspan="2">\
                                        <br/><p>Value:</p><br/>\
                                        <textarea id="sourceTextLabel"></textarea>\
                                        <p class="m-t-10">\
                                            <span class="withLabel" style="height: 32px; line-height: 32px;">Limit the number of times this object is draggable:</span>\
                                            <span class="withLabel" style="height: 32px; line-height: 32px;"><input style="display: inline-block; vertical-align: middle;" type="checkbox" id="sourceTextChecked"/></span>\
                                            <span class="withLabel" style="height: 32px; line-height: 32px;" id="sourceWrapper"><input type="text" id="sourceTextLimit" class="txtFullcreate"/></span>\
                                        </p>\
                                    </td>\
                                </tr>\
                            </tbody>\
                        </table>\
                      </div>';

            return {
                title: 'Drag and Drop - Source Text Label',
                minWidth: IS_V2 ? 500 : 350,
                minHeight: 75,
                contents:
		        [
			        {
			            id: 'imageUploadExe',
			            label: 'Settings',
			            elements:
				        [
					        {
					            type: 'html',
					            html: myhtml,
                                onLoad: function() {
                                    var $sourceTextChecked = $('#sourceTextChecked');
                                    var $sourceWrapper = $('#sourceWrapper');
                                    var $sourceTextLimit = $('#sourceTextLimit');

                                    $sourceTextChecked.on('change', function() {
                                        var $self = $(this);

                                        if ($self.is(':checked')) {
                                            $sourceWrapper.show();

                                            if ($sourceTextLimit.val() === 'unlimited') {
                                                $sourceTextLimit.focus().val(1);
                                            }
                                        } else {
                                            $sourceWrapper.hide();
                                        }
                                    });
                                },
					            onShow: function () {
					                CKEDITOR.replace('sourceTextLabel', {
                                        height: '150px',
                                        width: '100%',
                                        extraPlugins: 'mathfraction,mathjax',
                                        toolbar: [
                                            ['Mathjax']
                                        ],
                                        removePlugins: 'elementspath'
                                    });

                                    var $sourceTextLimit = $('#sourceTextLimit');
                                    var $sourceTextWidth = $('#sourceTextWidth');
                                    var $sourceTextHeight = $('#sourceTextHeight');
                                    var $sourceWrapper = $('#sourceWrapper');
                                    var $sourceTextChecked = $('#sourceTextChecked');
                                    var sourceWidth;
                                    var sourceHeight = eleAddSourceText.getStyle('height').replace('px', '');
                                    var limit;
                                    var sourceTextSrcIdentifier = eleAddSourceText.getAttribute('srcidentifier');
                                    var selectedSourcePartial = null;

                                    // Find the matching sourcePartial item
                                    for (var i = 0; i < sourcePartial.length; i++) {
                                        if (sourcePartial[i].srcIdentifier == sourceTextSrcIdentifier) {
                                            selectedSourcePartial = sourcePartial[i]; // Store reference to the entire object
                                            limit = selectedSourcePartial.limit;

                                            // Check limit if it is unlimited or before not set
                                            limit = (limit === 'unlimited' || limit == null) ? 'unlimited' : limit;

                                            $sourceTextLimit.val(limit);

                                            break;
                                        }
                                    }

                                    getUpDownNumber($sourceTextLimit, 0, 999);
                                    getUpDownNumber($sourceTextWidth, 1, 1000);
                                    getUpDownNumber($sourceTextHeight, 1, 1000);

                                    // Get width if before not set
                                    if (eleAddSourceText.getStyle('width') == '' || eleAddSourceText.getStyle('width') == 'auto') {
                                        sourceWidth = eleAddSourceText.getSize('width');
                                    } else {
                                        sourceWidth = eleAddSourceText.getStyle('width').replace('px', '');
                                    }

                                    $sourceTextLimit.attr('maxlength', 3);
                                    $sourceTextWidth.val(sourceWidth);
                                    $sourceTextHeight.val(sourceHeight);

                                    if (limit === 'unlimited') {
                                        $sourceTextChecked.prop('checked', false);
                                        $sourceWrapper.hide();
                                    } else {
                                        $sourceTextChecked.prop('checked', true);
                                        $sourceWrapper.show();
                                    }

                                    if (selectedSourcePartial) {
                                        var existingContent = eleAddSourceText.getHtml();
                                        const parser = new DOMParser();
                                        const doc = parser.parseFromString(existingContent, 'text/html');

                                        // Find the first <img> element
                                        const firstImg = doc.querySelector('img');
                                        if (firstImg) {
                                            firstImg.remove();
                                        }

                                        // Serialize the updated HTML back to a string and check for math elements
                                        const updatedHtmlString = doc.body.innerHTML;
                                        const hasMathElement = doc.querySelector('math') !== null;

                                        if (hasMathElement) {
                                            CKEDITOR.instances.sourceTextLabel.setData(loadMathML(updatedHtmlString));
                                        } else {
                                            CKEDITOR.instances.sourceTextLabel.setData(updatedHtmlString);
                                        }

                                    }
					                refreshPartialCredit();
					                //hide tooltip
					                $('#tips .tool-tip-tips').css({
					                    'display': 'none'
					                });
					            },
                                onHide: function () {
                                    if (CKEDITOR.instances.sourceTextLabel) {
                                        CKEDITOR.instances.sourceTextLabel.setData('');
                                        CKEDITOR.instances.sourceTextLabel.destroy();
                                    }
                                }
					        }
				        ]
			        }
		        ],
                onOk: function () {
                    var $sourceTextChecked = $('#sourceTextChecked');
                    // Get content from CKEditor
                    var labelText = CKEDITOR.instances.sourceTextLabel.getData();
                    var sourceTextSrcIdentifier = eleAddSourceText.getAttribute('srcidentifier');
                    var sourceTextLimit = $('#sourceTextLimit').val();
                    var sourceWidth = $('#sourceTextWidth').val();
                    var sourceHeight = $('#sourceTextHeight').val();
                    var error = {
                        errorLimit: 'Please input valid value of limit the number of draggable'
                    }

                    if (sourceTextLimit == 0 && $sourceTextChecked.is(':checked')) {
                        alert(error.errorLimit);
                        return false;
                    }

                    sourceTextLimit = sourceTextLimit > 0 ? parseInt(sourceTextLimit) : 'unlimited';
                    sourceWidth = sourceWidth !== null || sourceWidth !== '' ? sourceWidth : 120;
                    sourceHeight = sourceHeight !== null || sourceHeight !== '' ? sourceHeight : 40;

                    if (!$sourceTextChecked.is(':checked')) {
                        sourceTextLimit = 'unlimited';
                    }
					labelText = $("<div/>").html(labelText).html();
					if ($.trim(labelText) == '') {
						labelText = "Text Label";
					}

                    //Update value for source

                    for (var i = 0; i < sourcePartial.length; i++) {
                        if (sourcePartial[i].srcIdentifier == sourceTextSrcIdentifier) {
                            sourcePartial[i].value = labelText;
                            sourcePartial[i].limit = sourceTextLimit; // Adding limit dragged for source text
                        }
                    }

                    //Add source from temp to iResult
                    for (var i = 0; i < iResult.length; i++) {
                        if (iResult[i].type == "partialCredit") {
                            iResult[i].source = sourcePartial;
                        }
                    }

                    var partialSourceObject = '<span class="partialSourceObject partialAddSourceText" unselectable="on" partialID="Partial_1" contenteditable="false" style="width: ' + sourceWidth + 'px; height: ' + sourceHeight + 'px;" srcIdentifier="' + eleAddSourceText.getAttribute("srcidentifier") + '"><img class="cke_reset cke_widget_mask partialAddSourceTextMark" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" /><span>' + loadMathML(labelText) + '</span></span>';
                    if ($.browser.msie && parseInt($.browser.version, 10) < 10) {
                        partialSourceObject = '&nbsp;' + partialSourceObject;
                    }

                    var sourceObjectElement = CKEDITOR.dom.element.createFromHtml(partialSourceObject);
                    editor.insertElement(sourceObjectElement);
                    $(sourceObjectElement.$).find('.math-tex').each(function () {
                        this.parentElement.classList.add('cke_widget_new');
                        editor.widgets.initOn(new CKEDITOR.dom.element(this), 'mathjax');
                    });

                    newResult = iResult;
                }
            };
        });
    }
});
