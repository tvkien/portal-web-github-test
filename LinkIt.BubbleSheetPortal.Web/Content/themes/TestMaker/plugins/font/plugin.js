﻿/**
 * @license Copyright (c) 2003-2013, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

(function() {
	function addCombo( editor, comboName, styleType, lang, entries, defaultLabel, styleDefinition, order ) {
		var config = editor.config,
			style = new CKEDITOR.style( styleDefinition );


		// Gets the list of fonts from the settings.
		var names = entries.split( ';' ),
			values = [];

		// Create style objects for all fonts.
		var styles = {};
		for ( var i = 0; i < names.length; i++ ) {
			var parts = names[ i ];

			if ( parts ) {
				parts = parts.split( '/' );

				var vars = {},
					name = names[ i ] = parts[ 0 ];

				vars[ styleType ] = values[ i ] = parts[ 1 ] || name;

				styles[ name ] = new CKEDITOR.style( styleDefinition, vars );
				styles[ name ]._.definition.name = name;
			} else
				names.splice( i--, 1 );
		}

		editor.ui.addRichCombo( comboName, {
			label: lang.label,
			title: lang.panelTitle,
			toolbar: 'styles,' + order,
			allowedContent: style,
			requiredContent: style,

			panel: {
				css: [ CKEDITOR.skin.getPath( 'editor' ) ].concat( config.contentsCss ),
				multiSelect: false,
				attributes: { 'aria-label': lang.panelTitle }
			},

			init: function() {
				this.startGroup( lang.panelTitle );

				for ( var i = 0; i < names.length; i++ ) {
					var name = names[ i ];

					// Add the tag entry to the panel list.
					this.add( name, styles[ name ].buildPreview(), name );
				}

			},

			onClick: function (value) {

			    if (isStyleFontInlineChoice) {
			        isOnClickFontSize = true;
			    } else {
			        editor.focus();
			    }

			    if (navigator.userAgent.indexOf('Trident') != -1) {
			       editor.focus();
			    }

			    classNameStyleFontInlineChoice = value;


			    //apply fontsize for inline choice by single click
			    applyFontSizeInlineChoiceBySingleClick(editor, editor.editable(), value);


        var $textHotspots = $(editor.editable().$).find('.marker-linkit[hs_id]');
        var textHotSpot = {};
        if ($textHotspots.length) {
          $.each($textHotspots, function(i, el) {
            var hs_id = $(el).attr('hs_id');
            textHotSpot[hs_id] = $(el).attr('class');
          })
        }
			    editor.fire('saveSnapshot');

				var style = styles[value];

				/*var txtHtml = '';
			    var newContentEditor = '';
			    var highlightText = editor.getSelection().getSelectedText();

			    if (highlightText != undefined) {
			        txtHtml = '<span id="txtHtml">' + highlightText + '</span>';
			        txtHtml = txtHtml.trim();
			        txtHtml = txtHtml.replace(/[\s\n\r]+/g, " ");
			    }

			    var txthLight = $(txtHtml).text();
			    if (this.getValue() == value) {

			        editor['removeStyle'](style);
			        newContentEditor = editor.getData();
			        var newFont = $(style.buildPreview()).text(txthLight);
			        var htmlNew = $(newFont).prop('outerHTML');
			        newContentEditor = newContentEditor.replace(highlightText, htmlNew);

			        editor.setData(newContentEditor);
			        //editor['noChange'](style);
			    } else {
			        editor['applyStyle'](style);
			    }*/
				if (!isApplyInlineChoice) {
				    editor[this.getValue() == value ? 'noChange' : 'applyStyle'](style);
        }
        var $newEls = $(editor.editable().$).find('span[hs_id]');
        if ($newEls.length) {
          $.each($newEls, function(i, el) {
            var hs_id = $(el).attr('hs_id');
            $(el).addClass(textHotSpot[hs_id]);
          })
        }

				editor.fire('saveSnapshot');
			    isApplyInlineChoice = false;

			},

			onRender: function() {
			    editor.on('selectionChange', function (ev) {

					var currentValue = this.getValue();

					var elementPath = ev.data.path,
						elements = elementPath.elements;

					// For each element into the elements path.
					for ( var i = 0, element; i < elements.length; i++ ) {
						element = elements[ i ];

						// Check if the element is removable by any of
						// the styles.
						for ( var value in styles ) {
							if ( styles[ value ].checkElementMatch( element, true ) ) {
								if ( value != currentValue )
									this.setValue( value );
								return;
							}
						}
					}

			        // If no styles match, just empty it.
			        //prevent change font size when on inline choice
					this.setValue('', defaultLabel);

			        //classNameStyleFontInlineChoice
					if (isStyleFontInlineChoice) {

					    if (classNameStyleFontInlineChoice === '') {
					        classNameStyleFontInlineChoice = 'Normal';
					    }

					    $('.divInlineChoice .cke_combo__fontsize').find('a .cke_combo_text').text(classNameStyleFontInlineChoice);
					    $('.divInlineChoice .cke_combo__fontsize').find('.cke_combo_label').text(classNameStyleFontInlineChoice);
					}

			    }, this );
			},

			refresh: function() {
				if ( !editor.activeFilter.check( style ) )
					this.setState( CKEDITOR.TRISTATE_DISABLED );
			}
		});
	}

	CKEDITOR.plugins.add( 'font', {
		requires: 'richcombo',
		lang: 'af,ar,bg,bn,bs,ca,cs,cy,da,de,el,en,en-au,en-ca,en-gb,eo,es,et,eu,fa,fi,fo,fr,fr-ca,gl,gu,he,hi,hr,hu,id,is,it,ja,ka,km,ko,ku,lt,lv,mk,mn,ms,nb,nl,no,pl,pt,pt-br,ro,ru,si,sk,sl,sq,sr,sr-latn,sv,th,tr,ug,uk,vi,zh,zh-cn', // %REMOVE_LINE_CORE%
		init: function( editor ) {
			var config = editor.config;

			addCombo( editor, 'Font', 'family', editor.lang.font, config.font_names, config.font_defaultLabel, config.font_style, 30 );
			addCombo(editor, 'FontSize', 'size', editor.lang.font.fontSize, config.fontSize_sizes, config.fontSize_defaultLabel, config.fontSize_style, 40);
		}
	});
})();

/**
 * The list of fonts names to be displayed in the Font combo in the toolbar.
 * Entries are separated by semi-colons (`';'`), while it's possible to have more
 * than one font for each entry, in the HTML way (separated by comma).
 *
 * A display name may be optionally defined by prefixing the entries with the
 * name and the slash character. For example, `'Arial/Arial, Helvetica, sans-serif'`
 * will be displayed as `'Arial'` in the list, but will be outputted as
 * `'Arial, Helvetica, sans-serif'`.
 *
 *		config.font_names =
 *			'Arial/Arial, Helvetica, sans-serif;' +
 *			'Times New Roman/Times New Roman, Times, serif;' +
 *			'Verdana';
 *
 *		config.font_names = 'Arial;Times New Roman;Verdana';
 *
 * @cfg {String} [font_names=see source]
 * @member CKEDITOR.config
 */
CKEDITOR.config.font_names = 'Arial/Arial, Helvetica, sans-serif;' +
	'Comic Sans MS/Comic Sans MS, cursive;' +
	'Courier New/Courier New, Courier, monospace;' +
	'Georgia/Georgia, serif;' +
	'Lucida Sans Unicode/Lucida Sans Unicode, Lucida Grande, sans-serif;' +
	'Tahoma/Tahoma, Geneva, sans-serif;' +
	'Times New Roman/Times New Roman, Times, serif;' +
	'Trebuchet MS/Trebuchet MS, Helvetica, sans-serif;' +
	'Verdana/Verdana, Geneva, sans-serif';

/**
 * The text to be displayed in the Font combo is none of the available values
 * matches the current cursor position or text selection.
 *
 *		// If the default site font is Arial, we may making it more explicit to the end user.
 *		config.font_defaultLabel = 'Arial';
 *
 * @cfg {String} [font_defaultLabel='']
 * @member CKEDITOR.config
 */
CKEDITOR.config.font_defaultLabel = '';

/**
 * The style definition to be used to apply the font in the text.
 *
 *		// This is actually the default value for it.
 *		config.font_style = {
 *			element:		'span',
 *			styles:			{ 'font-family': '#(family)' },
 *			overrides:		[ { element: 'font', attributes: { 'face': null } } ]
 *     };
 *
 * @cfg {Object} [font_style=see example]
 * @member CKEDITOR.config
 */
CKEDITOR.config.font_style = {
	element: 'span',
	styles: { 'font-family': '#(family)' },
	overrides: [ {
		element: 'font', attributes: { 'face': null }
	}]
};

/**
 * The list of fonts size to be displayed in the Font Size combo in the
 * toolbar. Entries are separated by semi-colons (`';'`).
 *
 * Any kind of "CSS like" size can be used, like `'12px'`, `'2.3em'`, `'130%'`,
 * `'larger'` or `'x-small'`.
 *
 * A display name may be optionally defined by prefixing the entries with the
 * name and the slash character. For example, `'Bigger Font/14px'` will be
 * displayed as `'Bigger Font'` in the list, but will be outputted as `'14px'`.
 *
 *		config.fontSize_sizes = '16/16px;24/24px;48/48px;';
 *
 *		config.fontSize_sizes = '12px;2.3em;130%;larger;x-small';
 *
 *		config.fontSize_sizes = '12 Pixels/12px;Big/2.3em;30 Percent More/130%;Bigger/larger;Very Small/x-small';
 *
 * @cfg {String} [fontSize_sizes=see source]
 * @member CKEDITOR.config
 */
CKEDITOR.config.fontSize_sizes = 'Small/smallText;Normal/normalText;Large/largeText;X-Large/veryLargeText;';

/**
 * The text to be displayed in the Font Size combo is none of the available
 * values matches the current cursor position or text selection.
 *
 *		// If the default site font size is 12px, we may making it more explicit to the end user.
 *		config.fontSize_defaultLabel = '12px';
 *
 * @cfg {String} [fontSize_defaultLabel='']
 * @member CKEDITOR.config
 */
CKEDITOR.config.fontSize_defaultLabel = '';

/**
 * The style definition to be used to apply the font size in the text.
 *
 *		// This is actually the default value for it.
 *		config.fontSize_style = {
 *			element:		'span',
 *			styles:			{ 'font-size': '#(size)' },
 *			overrides:		[ { element :'font', attributes: { 'size': null } } ]
 *		};
 *
 * @cfg {Object} [fontSize_style=see example]
 * @member CKEDITOR.config
 */
CKEDITOR.config.fontSize_style = {
	element: 'span',
	attributes: {'class': '#(size)', 'styleName': '#(size)'},
	overrides: [ {
		element: 'span', attributes: { 'class': null }
	}]
};

