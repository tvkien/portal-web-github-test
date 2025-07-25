/**
 * @license Copyright (c) 2003-2013, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

 CKEDITOR.dialog.add( 'specialchar', function( editor ) {
	// Simulate "this" of a dialog for non-dialog events.
	// @type {CKEDITOR.dialog}
	var dialog,
		lang = editor.lang.specialchar;

	var onChoice = function( evt ) {
			var target, value;
			if ( evt.data )
				target = evt.data.getTarget();
			else
				target = new CKEDITOR.dom.element( evt );

			if ( target.getName() == 'a' && ( value = target.getChild( 0 ).getHtml() ) ) {
				target.removeClass( "cke_light_background" );
				//dialog.hide();

				// We must use "insertText" here to keep text styled.
				var span = editor.document.createElement('span');
				if (value == "nbsp") value = "&nbsp;";
				span.setHtml(value);
				var txt = span.getText();
				if (txt == "nbsp") txt = "&nbsp;";
				//editor.insertText(txt);
				var specCharHtml = CKEDITOR.dom.element.createFromHtml('<span class="speChar">'+txt+'</span>',editor.document);
				editor.insertElement(specCharHtml);
			}
		};

	var onClick = CKEDITOR.tools.addFunction( onChoice );

	var focusedNode;

	var onFocus = function( evt, target ) {
			var value;
	        target = target || evt.data.getTarget();

			if ( target.getName() == 'span' )
			    target = target.getParent();

            if (target.getName() == 'a') {
				// Trigger blur manually if there is focused node.
				if ( focusedNode )
					onBlur( null, focusedNode );

				var htmlPreview = dialog.getContentElement( 'info', 'htmlPreview' ).getElement();

				dialog.getContentElement( 'info', 'charPreview' ).getElement().setHtml( value );
				//htmlPreview.setHtml( CKEDITOR.tools.htmlEncode( value ) );
				target.getParent().addClass( "cke_light_background" );

				// Memorize focused node.
				focusedNode = target;
			}
		};

	var onBlur = function( evt, target ) {
			target = target || evt.data.getTarget();

			if ( target.getName() == 'span' )
				target = target.getParent();

			if ( target.getName() == 'a' ) {
				dialog.getContentElement( 'info', 'charPreview' ).getElement().setHtml( '&nbsp;' );
				dialog.getContentElement( 'info', 'htmlPreview' ).getElement().setHtml( '&nbsp;' );
				target.getParent().removeClass( "cke_light_background" );

				focusedNode = undefined;
			}
		};

	var onKeydown = CKEDITOR.tools.addFunction( function( ev ) {
		ev = new CKEDITOR.dom.event( ev );

		// Get an Anchor element.
		var element = ev.getTarget();
		var relative, nodeToMove;
		var keystroke = ev.getKeystroke(),
			rtl = editor.lang.dir == 'rtl';

		switch ( keystroke ) {
			// UP-ARROW
			case 38:
				// relative is TR
				if ( ( relative = element.getParent().getParent().getPrevious() ) ) {
					nodeToMove = relative.getChild( [ element.getParent().getIndex(), 0 ] );
					nodeToMove.focus();
					onBlur( null, element );
					onFocus( null, nodeToMove );
				}
				ev.preventDefault();
				break;
				// DOWN-ARROW
			case 40:
				// relative is TR
				if ( ( relative = element.getParent().getParent().getNext() ) ) {
					nodeToMove = relative.getChild( [ element.getParent().getIndex(), 0 ] );
					if ( nodeToMove && nodeToMove.type == 1 ) {
						nodeToMove.focus();
						onBlur( null, element );
						onFocus( null, nodeToMove );
					}
				}
				ev.preventDefault();
				break;
				// SPACE
				// ENTER is already handled as onClick
			case 32:
				onChoice({ data: ev } );
				ev.preventDefault();
				break;

				// RIGHT-ARROW
			case rtl ? 37:
				39 :
				// relative is TD
				if ( ( relative = element.getParent().getNext() ) ) {
					nodeToMove = relative.getChild( 0 );
					if ( nodeToMove.type == 1 ) {
						nodeToMove.focus();
						onBlur( null, element );
						onFocus( null, nodeToMove );
						ev.preventDefault( true );
					} else
						onBlur( null, element );
				}
				// relative is TR
				else if ( ( relative = element.getParent().getParent().getNext() ) ) {
					nodeToMove = relative.getChild( [ 0, 0 ] );
					if ( nodeToMove && nodeToMove.type == 1 ) {
						nodeToMove.focus();
						onBlur( null, element );
						onFocus( null, nodeToMove );
						ev.preventDefault( true );
					} else
						onBlur( null, element );
				}
				break;

				// LEFT-ARROW
			case rtl ? 39:
				37 :
				// relative is TD
				if ( ( relative = element.getParent().getPrevious() ) ) {
					nodeToMove = relative.getChild( 0 );
					nodeToMove.focus();
					onBlur( null, element );
					onFocus( null, nodeToMove );
					ev.preventDefault( true );
				}
				// relative is TR
				else if ( ( relative = element.getParent().getParent().getPrevious() ) ) {
					nodeToMove = relative.getLast().getChild( 0 );
					nodeToMove.focus();
					onBlur( null, element );
					onFocus( null, nodeToMove );
					ev.preventDefault( true );
				} else
					onBlur( null, element );
				break;
			default:
				// Do not stop not handled events.
				return;
		}
	});

	var tabPalette = function(palette, column) {
		var paletteHtml = '';
		var count = 1;

		for (var i = 0, len = palette.length; i < len; i++) {
			var character = palette[i];
			var charDesc = '';
			var charLetter = '';

			if (character instanceof Array) {
				charDesc = character[1];
				character = character[0];
			} else {
				var tmpName = character.replace('&', '').replace(';', '').replace('#', '');

				// Use character in case description unavailable.
				charDesc = lang[tmpName] || character;
			}

			charLetter = character;

			if (character === '&nbsp;') {
				charLetter = 'nbsp';
			}

			paletteHtml += '<a href="javascript:void(0);" role="option" class="cke_specialchar" onkeydown="CKEDITOR.tools.callFunction(' + onKeydown + ', event, this )" onclick="CKEDITOR.tools.callFunction(' + onClick + ', this); return false;" tabindex="-1">';
			paletteHtml += '<span style="margin: 0 auto;cursor: inherit">' + charLetter + '</span>';
			paletteHtml += '<span class="cke_voice_label">' + charDesc + '</span>';
			paletteHtml += '</a>';

			if (i === len - 1) {
				paletteHtml += '<br/>';
			}

			if (i === column * count - 1) {
				paletteHtml += '<br/>';
				count++;
			}
		}

		return paletteHtml;
	};

	var createHeadingTitle = function (title) {
		var heading = document.createElement('h3');
		heading.className = 'cke_specialchar_title';
		heading.innerHTML = title;
		return heading;
	};

	return {
	    title: 'Insert Special Character',
		minWidth: IS_V2 ? 550 : 320,
		minHeight: 100,
		buttons: [ CKEDITOR.dialog.cancelButton ],
		charColumns: 26,
		onLoad: function () {
			if (isNewPalette) {
				// Special Characters
				var specialCharHtml = '';
				// Math, Spanish and French Palette
				var mathPaletteHtml = '';
				var spanishPaletteHtml = '';
				var frenchPaletteHtml = '';
				var mathPalette;
				var spanishPalette;
				var frenchPalette;
				var simpleSciencePalette;
				var simpleSciencePaletteHtml = '';
				var hideSpecialcharTabs = editor.config.hideSpecialcharTabs || [];

				specialCharHtml += '<div class="palletTab">';
				specialCharHtml += '<div class="menuTab">';
				specialCharHtml += '<a class="cke_dialog_ui_button bntPalletSpecialTab bntPalletSpecialActive" id="palletTabMathSpecial">Math</a>';
				specialCharHtml += '<a class="cke_dialog_ui_button bntPalletSpecialTab" id="palletTabSpanishSpecial">Spanish</a>';
				specialCharHtml += '<a class="cke_dialog_ui_button bntPalletSpecialTab" id="palletTabFrenchSpecial">French</a>';
				specialCharHtml += '<a class="cke_dialog_ui_button bntPalletSpecialTab" id="palletTabSimpleScience">Basic Science</a>';
				specialCharHtml += '</div>';

				hideSpecialcharTabs.forEach(function(tabName) {
					specialCharHtml = specialCharHtml.replace('id="' + tabName, 'style="display:none" id="' + tabName);
				})

				// Math palette
				if (!hideSpecialcharTabs.includes('palletTabMathSpecial')) {
					mathPalette = editor.config.mathPalette;
					mathPaletteHtml = tabPalette(mathPalette, 26);
					mathPalette = editor.config.subscriptsPalette;
					mathPaletteHtml += tabPalette(mathPalette, 26);
				}

				// Spanish palette
				if (!hideSpecialcharTabs.includes('palletTabSpanishSpecial')) {
					spanishPalette = editor.config.specialCharsSpanish;
					spanishPaletteHtml = tabPalette(spanishPalette, 9);
				}

				// French palette
				if (!hideSpecialcharTabs.includes('palletTabFrenchSpecial')) {
					frenchPalette = editor.config.specialCharsFrench;
					frenchPaletteHtml = tabPalette(frenchPalette, 6);
				}

				// Simple Science palette
				if (!hideSpecialcharTabs.includes('palletTabSimpleScience')) {
					simpleSciencePalette = editor.config.specialCharsSimpleScience;
				}

				for (var i = 0; i < simpleSciencePalette.length; i++) {
					var sub = simpleSciencePalette[i];
					var subTitle = createHeadingTitle(sub.Title);
				    simpleSciencePaletteHtml += subTitle.outerHTML;
				    simpleSciencePaletteHtml += tabPalette(sub.Value.split(','), 10)
        		}
				
				specialCharHtml += '<div class="specialChartRow"> <div class="specialChartContainer">' + mathPaletteHtml + '</div></div>';
				specialCharHtml += '<div class="specialChartRow"> <div class="specialChartContainer">' + spanishPaletteHtml + '</div> </div>';
				specialCharHtml += '<div class="specialChartRow"> <div class="specialChartContainer">' + frenchPaletteHtml + '</div> </div>';
				specialCharHtml += '<div class="specialChartRow"> <div class="specialChartContainer">' + simpleSciencePaletteHtml + '</div></div>';
				specialCharHtml += '</div>';



				this.getContentElement('info', 'charContainer').getElement().setHtml(specialCharHtml);
  			} else {
			    var columns = this.definition.charColumns,
			        extraChars = editor.config.extraSpecialChars;
			    var chars = '';

			    if (isExtraChar == true) {
			        chars = extraChars;
		    } else if (isCharNumeric == true) {
		    	chars = editor.config.specialCharsNumeric;
			    } else {
			        chars = editor.config.specialChars;
			    }

				var charsTableLabel = CKEDITOR.tools.getNextId() + '_specialchar_table_label';

				var html = ['<div class="palletTab"><div class="menuTab"><a class="cke_dialog_ui_button bntPalletSpecialTab bntPalletSpecialActive" id="palletTabMathSpecial">Math</a><a class="cke_dialog_ui_button bntPalletSpecialTab" id="palletTabSpanishSpecial">Spanish</a><a class="cke_dialog_ui_button bntPalletSpecialTab" id="palletTabFrenchSpecial">French</a><a class="cke_dialog_ui_button bntPalletSpecialTab" id="palletTabSimpleScience">Basic Science</a></div>'];

				var i = 0,
					size = chars.length,
					character, charDesc;

				var mathCharactersCount = _mathCharactersCount = 45;
				var mathCharactersPerRow = 10;

				var spanishCharactersCount = _spanishCharactersCount = 18;
				var spanishCharactersPerRow = 10;

				var frenchCharactersCount = _frenchCharactersCount = 37;
				var frenchCharactersPerRow = 10;

				var simpleScienceCharactersCount = _simpleScienceCharactersCount = editor.config.simpleScienceCharacterSymbols.length;
				var simpleScienceCharactersPerRow = 10;


				while ( i < size ) {
          html.push('<div class="specialChartRow"><div class="specialChartContainer">');
				    if (isExtraChar == true) {
				        if (i == 0) {
				            columns = 68;
				        } else if (i > (size - 68) && i < (size - 68 + 18)) {
				            columns = size - 68;
				        } else {
				            columns = 18;
				        }
				    } else {
				        if (i == 0) {
				            columns = mathCharactersCount;
				        } else if (i >= mathCharactersCount && i < (mathCharactersCount+spanishCharactersCount)) {
                    		_mathCharactersCount = null;
				            columns = spanishCharactersCount;
				        } else if (i >= (mathCharactersCount+spanishCharactersCount) && i < (mathCharactersCount+spanishCharactersCount+frenchCharactersCount)) {
                    		_spanishCharactersCount = null;
				            columns = frenchCharactersCount;
				        } else if (i >= (mathCharactersCount+spanishCharactersCount+frenchCharactersCount) && i < (mathCharactersCount+spanishCharactersCount+frenchCharactersCount+simpleScienceCharactersCount)) {
				            _frenchCharactersCount = null;
                    		columns = simpleScienceCharactersCount;
				        }
				    }

				    var simpleCharScienceIndex = 0;
				    var k = 0;
					for ( var j = 0; j < columns; j++, i++ ) {
					    if ((character = chars[i])) {
					        charDesc = '';

					        if (character instanceof Array) {
					            charDesc = character[1];
					            character = character[0];
					        } else {
					            var _tmpName = character.replace('&', '').replace(';', '').replace('#', '');

					            // Use character in case description unavailable.
					            charDesc = lang[_tmpName] || character;
					        }

					        var charLabelId = 'cke_specialchar_label_' + i + '_' + CKEDITOR.tools.getNextNumber();

					        var letter = character;
					        if (character == "&nbsp;") {
					            letter = "nbsp";
					        }
					        k = k + 1;
					        if (isExtraChar == false) {
					            if (columns == _simpleScienceCharactersCount) {
					                if (editor.config.simpleScienceCharacterTitles[simpleCharScienceIndex] != undefined
                                        && j == editor.config.simpleScienceCharacterTitles[simpleCharScienceIndex].FirstIndex) {
										var subTitle = createHeadingTitle(editor.config.simpleScienceCharacterTitles[simpleCharScienceIndex].Title);
					                    html.push(subTitle.outerHTML);
					                    simpleCharScienceIndex = simpleCharScienceIndex + 1;
					                    k = 1;
					                }
					            }
					        }
					        html.push('<!--td style="cursor: default" role="presentation"-->' +
								'<a href="javascript: void(0);" role="option"' +
								' aria-posinset="' + (i + 1) + '"', ' aria-setsize="' + size + '"', ' aria-labelledby="' + charLabelId + '"', ' class="cke_specialchar"' +
								' onkeydown="CKEDITOR.tools.callFunction( ' + onKeydown + ', event, this )"' +
								' onclick="CKEDITOR.tools.callFunction(' + onClick + ', this); return false;"' +
								' tabindex="-1">' +
								'<span style="margin: 0 auto;cursor: inherit">' +
		                        letter +
								'</span>' +
								'<span class="cke_voice_label" id="' + charLabelId + '">' +
								charDesc +
								'</span></a>');

					        if (isExtraChar == true) {
					            if (columns == 68) {
					                if (j == 20 || j == 40) {
					                    html.push("<br />");
					                }
					            } else if (columns == 18) {
					                if (j == 8) {
					                    html.push("<br />");
					                }
					            } else {
					                if (j == 18 || j == 37) {
					                    html.push("<br />");
					                }
					            }
					        } else {
					            if (columns == _mathCharactersCount) {
					                if (((j+1)%mathCharactersPerRow)==0) {
					                    html.push("<br />");
					                }
					            } else if (columns == _spanishCharactersCount) {
					                if (((j+1)%spanishCharactersPerRow)==0) {
					                    html.push("<br />");
					                }
					            } else if (columns == _frenchCharactersCount) {
					                if (((j+1)%frenchCharactersPerRow)==0) {
					                    html.push("<br />");
					                }
					            }else if (columns == _simpleScienceCharactersCount) {
					            	  if (((k) % simpleScienceCharactersPerRow) == 0) {
					            	      html.push("<br />");
					            	  }
					            }
					        }

					    } else { html.push('<!--td -->&nbsp;'); }
					}

          html.push( '</div></div>' );
				}

				html.push('</div>', '<span id="' + charsTableLabel + '" class="cke_voice_label">' + lang.options + '</span>');
				//edit position for this dialog
				this.getContentElement('info', 'charContainer').getElement().setHtml(html.join(''));
			}

			var wDialog = $('.cke_single_page').last().width();
			var newW = window.innerWidth - wDialog;

			$('.cke_single_page').last().css('left', newW / 2);

			// Always show first tab
			$(document).find('.palletTab .specialChartRow').eq(0).show();

			// Trigger tab special characters
			$('.bntPalletSpecialTab').on('click', function () {
				var $self = $(this);
				var thisIndex = $self.index();
				$('.palletTab .specialChartRow').hide();
				$('.palletTab .bntPalletSpecialTab').removeClass('bntPalletSpecialActive');
				$self.addClass('bntPalletSpecialActive');
				$self.parents('.palletTab').find('.specialChartRow:eq(' + thisIndex + ')').show();
			});
		},
		contents: [
			{
			id: 'info',
			label: editor.lang.common.generalTab,
			title: editor.lang.common.generalTab,
			padding: 0,
			align: 'top',
			elements: [
				{
				type: 'hbox',
				align: 'top',
				widths: [ '320px', '90px' ],
				children: [
					{
					type: 'html',
					id: 'charContainer',
					html: '',
					onMouseover: onFocus,
					onMouseout: onBlur,
					focus: function() {
						var firstChar = this.getElement().getElementsByTag( 'a' ).getItem( 0 );
						setTimeout( function() {
							firstChar.focus();
							onFocus( null, firstChar );
						}, 0 );
					},
					onShow: function() {
						var $dialog = $(this.getDialog().getElement().$);
						var firstChar = this.getElement().getChild( [ 0, 0, 0, 0, 0 ] );
						setTimeout(function () {
						    if (firstChar != null)
						    {
						        firstChar.focus();
						        onFocus(null, firstChar);
						    }
						}, 0 );

						// Hidden tab spanish and french
						if (isCharNumeric) {
							$('.cke_editor_dragdropnumericalsourceeditor_dialog').find('#palletTabSpanishSpecial').hide();
							$('.cke_editor_dragdropnumericalsourceeditor_dialog').find('#palletTabFrenchSpecial').hide();
						}

						// Show first tab if not tab active
						if (!$dialog.find('.palletTab .specialChartRow:visible').length) {
							$dialog.find('.palletTab .specialChartRow').eq(0).show();
							$dialog.find('.palletTab .menuTab .bntPalletSpecialTab').eq(0).addClass('bntPalletSpecialActive');
						}
					},
					onLoad: function( event ) {
						dialog = event.sender;
					}
				},
					{
					type: 'hbox',
					align: 'top',
					widths: [ '100%' ],
					children: [
						{
						type: 'vbox',
						align: 'top',
						children: [
							{
							type: 'html',
							html: '<div></div>'
						},
							{
							type: 'html',
							id: 'charPreview',
							className: 'cke_dark_background',
							style: 'border:1px solid #eeeeee;font-size:28px;height:40px;width:70px;padding-top:9px;font-family:\'Microsoft Sans Serif\',Arial,Helvetica,Verdana;text-align:center;display:none;',
							html: '<div>&nbsp;</div>'
						},
							{
							type: 'html',
							id: 'htmlPreview',
							className: 'cke_dark_background',
							style: 'border:1px solid #eeeeee;font-size:14px;height:20px;width:70px;padding-top:2px;font-family:\'Microsoft Sans Serif\',Arial,Helvetica,Verdana;text-align:center;display:none;',
							html: '<div>&nbsp;</div>'
						}
						]
					}
					]
				}
				]
			}
			]
		}
		]
	};
});
