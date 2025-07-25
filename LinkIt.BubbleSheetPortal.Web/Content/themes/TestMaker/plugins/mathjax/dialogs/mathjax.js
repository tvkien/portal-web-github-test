/**
 * @license Copyright (c) 2003-2013, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

'use strict';

var mathjaxHelper = {
  isValidHTML: function () {
		var html = this.getValue();
		var hasMathNode = function() {
			var htmlStr = html.replace(/[\n]/g, '').trim();
			var regex = /^<math.+?>.+?<\/math>$/;

			return regex.test(htmlStr);
		};

		var isUniqueMathNode = function() {
			var htmlStr = html.replace(/[\n]/g, '').trim();
			var regex = /<math.+?>.+?<\/math>/g;
			var elementRegex = /\bmath\b/g;

    	return htmlStr.match(regex).length === 1 && htmlStr.match(elementRegex).length === 2;
		};

		var isValidElement = function() {
			var regexOpenTag = /<(?!\/*math)[a-z0-9\.\s"=\-]*?>/gm;
			var regexCloseTag = /<\/(?!\/*math)[a-z]+?>/gm;
			var opentags = html.match(regexOpenTag);
			var closetags = html.match(regexCloseTag);
			var nonAttrOpentags = [];

			for (var i = 0, len = opentags.length; i < len; i++) {
				var regexNonAttr = /<([a-z]+)(\s.*)?>/ig;
				var removedAttrItem = opentags[i];

				removedAttrItem = removedAttrItem.replace(regexNonAttr, '<$1>');
				nonAttrOpentags.push(removedAttrItem);
			}

			if (nonAttrOpentags.length !== closetags.length) {
				return false;
			}

			for (var i = 0, len = closetags.length; i < len; i++) {
				var currenttTag = closetags[i];
				var comparatorTag = currenttTag.replace('/', '');
				var comparingIndex = nonAttrOpentags.indexOf(comparatorTag);

				if (comparingIndex === -1) {
					return false;
				} else {
					nonAttrOpentags.splice(comparingIndex, 1)
				}
			}

			return true;
		};

    return hasMathNode() && isUniqueMathNode() && isValidElement();
  },
  isUnvalidTextNode: function () {
		var regex = /Unexpected text node|Error parsing MathML/i;
		var textNode = this.getDialog()
			.getElement()
			.findOne("iframe")
			.$.contentWindow.document.body;
    var textNodeContent = textNode ? textNode.innerHTML.toString() : '';

    return regex.test(textNodeContent);
  },
  hasErrorMessage: function () {
    var elId = "ck_error_" + this.domId;
    var el = CKEDITOR.document.getById(this.domId).findOne("#" + elId);

    return !!el;
  },
  renderErrorMessage: function (message) {
		var errorHtml =
			'<div id="ck_error_' +
			this.domId +
			'" class="text-danger mt-3">' + message + '</div>';
		CKEDITOR.document.getById(this.domId).appendHtml(errorHtml);
	},
	cleanErrorMessage: function() {
		var elId = 'ck_error_' + this.domId;
		var el = CKEDITOR.document.getById( this.domId ).findOne('#' + elId);

		!!el && el.remove();
	}
};

CKEDITOR.dialog.add( 'mathjax', function( editor ) {

	var preview,
		lang = editor.lang.mathjax;

	return {
		title: lang.title,
		width: 350,
		minHeight: 100,
		contents: [
			{
				id: 'info',
				elements: [
					{
						id: 'plus-button',
						type: 'button',
						label: '+',
						onClick: function() {
						    // this = CKEDITOR.ui.dialog.button
						    var id = this.domId;
							var textArea = $('#'+id).parent('td').parent('tr').parent('tbody').find('textarea.cke_dialog_ui_input_textarea ');
						    if(textArea != undefined && textArea.length > 0){
		                        var oldtext = textArea.val(),
		                        	currCursor = textArea.getCursorPosition();
	                            if(currCursor < oldtext.length && currCursor >= 0){

	                                var leftText = oldtext.substring(0,currCursor),
	                                    rightText = oldtext.substring(currCursor,oldtext.length);
	                                var string = leftText + '+' + rightText;
	                                currCursor += 1;

	                            }else{
	                                var string = oldtext + '+';
	                                currCursor = string.length;
	                            }
	                            var that = this;
	                            textArea.on( 'change', function() {
									// Old code remove \( and )\
									//preview.setValue( '\\(' + that.getInputElement().getValue() + '\\)' );
									preview.setValue( that.getInputElement().getValue() );
								});
	                            textArea.val(string).focus().setCaretPosition(currCursor);

		                    }
						},
						onLoad: function(widget) {
							var id = this.domId;
							$('#'+id).parent('td').parent('tr').css('float', 'left');
						}
					},
					{
						id: 'minus-button',
						type: 'button',
						label: '-',
						onClick: function() {
						    // this = CKEDITOR.ui.dialog.button
						    var id = this.domId;
							var textArea = $('#'+id).parent('td').parent('tr').parent('tbody').find('textarea.cke_dialog_ui_input_textarea ');
						    if(textArea != undefined && textArea.length > 0){
		                        var oldtext = textArea.val(),
		                        	currCursor = textArea.getCursorPosition();
	                            if(currCursor < oldtext.length && currCursor >= 0){

	                                var leftText = oldtext.substring(0,currCursor),
	                                    rightText = oldtext.substring(currCursor,oldtext.length);
	                                var string = leftText + '-' + rightText;
	                                currCursor += 1;

	                            }else{
	                                var string = oldtext + '-';
	                                currCursor = string.length;
	                            }
	                            var that = this;
	                            textArea.on( 'change', function() {
									// Old code remove \( and )\
									//preview.setValue( '\\(' + that.getInputElement().getValue() + '\\)' );
									preview.setValue( that.getInputElement().getValue() );
								});
	                            textArea.val(string).focus().setCaretPosition(currCursor);

		                    }
						},
						onLoad: function(widget) {
							var id = this.domId;
							$('#'+id).parent('td').parent('tr').css('float', 'left');
						}
					},
					{
						id: 'multiply-button',
						type: 'button',
						label: 'x',
						onClick: function() {
						    // this = CKEDITOR.ui.dialog.button
						    var id = this.domId;
							var textArea = $('#'+id).parent('td').parent('tr').parent('tbody').find('textarea.cke_dialog_ui_input_textarea ');
						    if(textArea != undefined && textArea.length > 0){
		                        var oldtext = textArea.val(),
		                        	currCursor = textArea.getCursorPosition();
	                            if(currCursor < oldtext.length && currCursor >= 0){

	                                var leftText = oldtext.substring(0,currCursor),
	                                    rightText = oldtext.substring(currCursor,oldtext.length);
	                                var string = leftText + 'x' + rightText;
	                                currCursor += 1;

	                            }else{
	                                var string = oldtext + 'x';
	                                currCursor = string.length;
	                            }
	                            var that = this;
	                            textArea.on( 'change', function() {
									// Old code remove \( and )\
									//preview.setValue( '\\(' + that.getInputElement().getValue() + '\\)' );
									preview.setValue( that.getInputElement().getValue() );
								});
	                            textArea.val(string).focus().setCaretPosition(currCursor);

		                    }
						},
						onLoad: function(widget) {
							var id = this.domId;
							$('#'+id).parent('td').parent('tr').css('float', 'left');
						}
					},
					{
						id: 'division-button',
						type: 'button',
						label: '÷',
						onClick: function() {
						    // this = CKEDITOR.ui.dialog.button
						    var id = this.domId;
							var textArea = $('#'+id).parent('td').parent('tr').parent('tbody').find('textarea.cke_dialog_ui_input_textarea ');
						    if(textArea != undefined && textArea.length > 0){
		                        var oldtext = textArea.val(),
		                        	currCursor = textArea.getCursorPosition();
	                            if(currCursor < oldtext.length && currCursor >= 0){

	                                var leftText = oldtext.substring(0,currCursor),
	                                    rightText = oldtext.substring(currCursor,oldtext.length);
	                                var string = leftText + '÷' + rightText;
	                                currCursor += 1;

	                            }else{
	                                var string = oldtext + '÷';
	                                currCursor = string.length;
	                            }
	                            var that = this;
	                            textArea.on( 'change', function() {
									// Old code remove \( and )\
									//preview.setValue( '\\(' + that.getInputElement().getValue() + '\\)' );
									preview.setValue( that.getInputElement().getValue() );
								});
	                            textArea.val(string).focus().setCaretPosition(currCursor);

		                    }
						},
						onLoad: function(widget) {
							var id = this.domId;
							$('#'+id).parent('td').parent('tr').css('float', 'left');
						}
					},
					{
						id: 'sigma-button',
						type: 'button',
						label: 'Σ',
						onClick: function() {
						    // this = CKEDITOR.ui.dialog.button
						    var id = this.domId;
							var textArea = $('#'+id).parent('td').parent('tr').parent('tbody').find('textarea.cke_dialog_ui_input_textarea ');
						    if(textArea != undefined && textArea.length > 0){
		                        var oldtext = textArea.val(),
		                        	currCursor = textArea.getCursorPosition();
	                            if(currCursor < oldtext.length && currCursor >= 0){

	                                var leftText = oldtext.substring(0,currCursor),
	                                    rightText = oldtext.substring(currCursor,oldtext.length);
	                                var string = leftText + 'Σ' + rightText;
	                                currCursor += 1;

	                            }else{
	                                var string = oldtext + 'Σ';
	                                currCursor = string.length;
	                            }
	                            var that = this;
	                            textArea.on( 'change', function() {
									// Old code remove \( and )\
									//preview.setValue( '\\(' + that.getInputElement().getValue() + '\\)' );
									preview.setValue( that.getInputElement().getValue() );
								});
	                            textArea.val(string).focus().setCaretPosition(currCursor);

		                    }
						},
						onLoad: function(widget) {
							var id = this.domId;
							$('#'+id).parent('td').parent('tr').css('float', 'left');
						}
					},
					{
						id: 'integral-button',
						type: 'button',
						label: '∫',
						onClick: function() {
						    // this = CKEDITOR.ui.dialog.button
						    var id = this.domId;
							var textArea = $('#'+id).parent('td').parent('tr').parent('tbody').find('textarea.cke_dialog_ui_input_textarea ');
						    if(textArea != undefined && textArea.length > 0){
		                        var oldtext = textArea.val(),
		                        	currCursor = textArea.getCursorPosition();
	                            if(currCursor < oldtext.length && currCursor >= 0){

	                                var leftText = oldtext.substring(0,currCursor),
	                                    rightText = oldtext.substring(currCursor,oldtext.length);
	                                var string = leftText + '∫' + rightText;
	                                currCursor += 1;

	                            }else{
	                                var string = oldtext + '∫';
	                                currCursor = string.length;
	                            }
	                            var that = this;
	                            textArea.on( 'change', function() {
									// Old code remove \( and )\
									//preview.setValue( '\\(' + that.getInputElement().getValue() + '\\)' );
									preview.setValue( that.getInputElement().getValue() );
								});
	                            textArea.val(string).focus().setCaretPosition(currCursor);

		                    }
						},
						onLoad: function(widget) {
							var id = this.domId;
							$('#'+id).parent('td').parent('tr').css('float', 'left');
						}
					},
					{
						id: 'pi-button',
						type: 'button',
						label: 'π',
						onClick: function() {
						    // this = CKEDITOR.ui.dialog.button
						    var id = this.domId;
							var textArea = $('#'+id).parent('td').parent('tr').parent('tbody').find('textarea.cke_dialog_ui_input_textarea ');
						    if(textArea != undefined && textArea.length > 0){
		                        var oldtext = textArea.val(),
		                        	currCursor = textArea.getCursorPosition();
	                            if(currCursor < oldtext.length && currCursor >= 0){

	                                var leftText = oldtext.substring(0,currCursor),
	                                    rightText = oldtext.substring(currCursor,oldtext.length);
	                                var string = leftText + 'π' + rightText;
	                                currCursor += 1;

	                            }else{
	                                var string = oldtext + 'π';
	                                currCursor = string.length;
	                            }
	                            var that = this;
	                            textArea.on( 'change', function() {
									// Old code remove \( and )\
									//preview.setValue( '\\(' + that.getInputElement().getValue() + '\\)' );
									preview.setValue( that.getInputElement().getValue() );
								});
	                            textArea.val(string).focus().setCaretPosition(currCursor);

		                    }
						},
						onLoad: function(widget) {
							var id = this.domId;
							$('#'+id).parent('td').parent('tr').css('float', 'left');
						}
					},
					{
						id: 'degree-button',
						type: 'button',
						label: '°',
						onClick: function() {
						    // this = CKEDITOR.ui.dialog.button
						    var id = this.domId;
							var textArea = $('#'+id).parent('td').parent('tr').parent('tbody').find('textarea.cke_dialog_ui_input_textarea ');
						    if(textArea != undefined && textArea.length > 0){
		                        var oldtext = textArea.val(),
		                        	currCursor = textArea.getCursorPosition();
	                            if(currCursor < oldtext.length && currCursor >= 0){

	                                var leftText = oldtext.substring(0,currCursor),
	                                    rightText = oldtext.substring(currCursor,oldtext.length);
	                                var string = leftText + '°' + rightText;
	                                currCursor += 1;

	                            }else{
	                                var string = oldtext + '°';
	                                currCursor = string.length;
	                            }
	                            var that = this;
	                            textArea.on( 'change', function() {
									// Old code remove \( and )\
									//preview.setValue( '\\(' + that.getInputElement().getValue() + '\\)' );
									preview.setValue( that.getInputElement().getValue() );
								});
	                            textArea.val(string).focus().setCaretPosition(currCursor);

		                    }
						},
						onLoad: function(widget) {
							var id = this.domId;
							$('#'+id).parent('td').parent('tr').css('float', 'left');
						}
					},
					{
						id: 'equation',
						type: 'textarea',
						onLoad: function( widget ) {
							var that = this;
							var validateTextNode = function() {
								var isValidHTML = mathjaxHelper.isValidHTML.bind(that);
								var isUnvalidTextNode = mathjaxHelper.isUnvalidTextNode.bind(that);
								var hasErrorMessage = mathjaxHelper.hasErrorMessage.bind(that);
								var renderErrorMessage = mathjaxHelper.renderErrorMessage.bind(that);
								var cleanErrorMessage = mathjaxHelper.cleanErrorMessage.bind(that);

								var inputIsDirty = !isValidHTML() || isUnvalidTextNode();
								var errorIsDisplay = hasErrorMessage();

								if (inputIsDirty && !errorIsDisplay) {
									renderErrorMessage('XML format not correct. Please verify');
								}

								if (!inputIsDirty && errorIsDisplay) {
									cleanErrorMessage();
								}
							}

							if ( !( CKEDITOR.env.ie && CKEDITOR.env.version == 8 ) ) {
								this.getInputElement().on( 'keyup', function() {
									// Old code remove \( and )\
									//preview.setValue( '\\(' + that.getInputElement().getValue() + '\\)' );
									preview.setValue( that.getInputElement().getValue() );
									validateTextNode();
								} );

								this.getInputElement().on( 'focus', function() {
									// Old code remove \( and )\
								    //preview.setValue( '\\(' + that.getInputElement().getValue() + '\\)' );
								    //alert(that.getInputElement().getValue());
									preview.setValue( that.getInputElement().getValue() );
									validateTextNode();
								} );
								this.getInputElement().on('paste', function (e) {
                                    //delete space when paste a string
							        setTimeout(function () {
							            var strInput = that.getInputElement().getValue();
							            strInput = strInput.replace(new RegExp("\>[\n\t ]+\<", "g"), "><");
													that.setValue(formatXml(strInput));
													validateTextNode();
							        }, 100);
							    });
							}
							var id = this.domId;
							$('#' + id).find('textarea').css({ 'min-height': '250px' });
							$('#'+id).parent('td').parent('tr').css('clear', 'both');
						},

						setup: function( widget ) {
							// Old code remove \( and )\
							//this.setValue( CKEDITOR.plugins.mathjax.trim( widget.data.math ) );
							var cleanErrorMessage = mathjaxHelper.cleanErrorMessage.bind(this);

							cleanErrorMessage();
							this.setValue(formatXml(widget.data.math));
						},

						commit: function (widget) {
							// Old code remove \( and )\
						    //widget.setData( 'math', '\\(' + this.getValue() + '\\)' );
						    var xmlValue = this.getValue().replace(new RegExp("\>[\n\t ]+\<", "g"), "><").replace(/<!--[\s\S]*?-->/g, "");
						    widget.setData('math', xmlValue);
						},

						validate: function() {
							var isValidHTML = mathjaxHelper.isValidHTML.bind(this);
							var isUnvalidTextNode = mathjaxHelper.isUnvalidTextNode.bind(this);
							var hasErrorMessage = mathjaxHelper.hasErrorMessage.bind(this);
							var renderErrorMessage = mathjaxHelper.renderErrorMessage.bind(this);
							var cleanErrorMessage = mathjaxHelper.cleanErrorMessage.bind(this);

							var inputIsDirty = !isValidHTML() || isUnvalidTextNode();
							var errorIsDisplay = hasErrorMessage();

							if (inputIsDirty) {
								if (!errorIsDisplay) {
									renderErrorMessage('XML format not correct. Please verify');
								}
								return false;
							}

							if (!inputIsDirty && errorIsDisplay) {
								cleanErrorMessage();
							}
						}
					},
                    {
                        id: 'texttospeech',
                        type: 'textarea',
                        label: editor.lang.mathjax.texttospeech,
                        style: 'min-height: 100px; height: 100px;',
                        inputStyle: 'min-height: 100px; height: 100px;margin-top: 10px;',
                        onLoad: function (widget) {
                            var id = this.domId;
                            $('#' + id).parent('td').parent('tr').css('clear', 'both');
                        },
                        setup: function( widget ) {
                            this.setValue(widget.parts.span.$.getAttribute("texttospeech"));
                        },
                        commit: function (widget) {
                            var texttoSpeech = convertTexttoHTML(this.getValue());
                            if (widget.parts.span.$.className.indexOf('math-tex') > -1) {
                                widget.parts.span.$.setAttribute("texttospeech", texttoSpeech)
                            }
                        }
                    },
                    {
                        id: 'documentation',
                        type: 'html',
                        html: ''
                        /*'<div style="width:100%;text-align:right;margin:-8px 0 10px">' +
                            '<a class="cke_mathjax_doc" href="' + lang.docUrl + '" target="_black" style="cursor:pointer;color:#00B2CE;text-decoration:underline">' +
                                lang.docLabel +
                            '</a>' +
                        '</div>'*/
                    },
					( !( CKEDITOR.env.ie && CKEDITOR.env.version == 8 ) ) && {
						id: 'preview',
						type: 'html',
						html:
							'<div style="width:100%; text-align:center; max-width: 350px;">' +
								'<iframe style="border:0;width:0;height:0;font-size:20px" scrolling="no" frameborder="0" allowTransparency="true" src="' + CKEDITOR.plugins.mathjax.fixSrc + '"></iframe>' +
							'</div>',

						onLoad: function( widget ) {
							var iFrame = CKEDITOR.document.getById( this.domId ).getChild( 0 );
							preview = new CKEDITOR.plugins.mathjax.frameWrapper( iFrame, editor );
						},

						setup: function( widget ) {
							preview.setValue( widget.data.math );
						}
					}
				]
			}
		]
	};
} );
