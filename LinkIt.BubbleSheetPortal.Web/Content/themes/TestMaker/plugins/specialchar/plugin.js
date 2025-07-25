/**
 * @license Copyright (c) 2003-2013, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

/**
 * @fileOverview Special Character plugin
 */
CKEDITOR.plugins.add( 'specialchar', {
	// List of available localizations.
	availableLangs: { ar:1,bg:1,ca:1,cs:1,cy:1,de:1,el:1,en:1,eo:1,es:1,et:1,fa:1,fi:1,fr:1,'fr-ca':1,gl:1,he:1,hr:1,hu:1,id:1,it:1,ja:1,km:1,ku:1,lv:1,nb:1,nl:1,no:1,pl:1,pt:1,'pt-br':1,ru:1,si:1,sk:1,sl:1,sq:1,sv:1,th:1,tr:1,ug:1,uk:1,vi:1,zh:1,'zh-cn':1 },
	lang: 'af,ar,bg,bn,bs,ca,cs,cy,da,de,el,en,en-au,en-ca,en-gb,eo,es,et,eu,fa,fi,fo,fr,fr-ca,gl,gu,he,hi,hr,hu,id,is,it,ja,ka,km,ko,ku,lt,lv,mk,mn,ms,nb,nl,no,pl,pt,pt-br,ro,ru,si,sk,sl,sq,sr,sr-latn,sv,th,tr,ug,uk,vi,zh,zh-cn', // %REMOVE_LINE_CORE%
	requires: 'dialog',
	icons: 'specialchar', // %REMOVE_LINE_CORE%
	hidpi: true, // %REMOVE_LINE_CORE%
	init: function( editor ) {
		var pluginName = 'specialchar',
			plugin = this;
		// Register the dialog.
		CKEDITOR.dialog.add( pluginName, this.path + 'dialogs/specialchar.js' );

		editor.addCommand( pluginName, {
			exec: function() {
				var langCode = editor.langCode;
				langCode =
					plugin.availableLangs[ langCode ] ? langCode :
					plugin.availableLangs[ langCode.replace( /-.*/, '' ) ] ? langCode.replace( /-.*/, '' ) :
					'en';

				CKEDITOR.scriptLoader.load( CKEDITOR.getUrl( plugin.path + 'dialogs/lang/' + langCode + '.js' ), function() {
					CKEDITOR.tools.extend( editor.lang.specialchar, plugin.langEntries[ langCode ] );
					editor.openDialog( pluginName );
				});
			   
			},
			modes: { wysiwyg:1 },
			canUndo: false
		});

		// Register the toolbar button.
        if (editor.ui.addButton) {
            editor.ui.addButton( 'SpecialChar', {
                //label: editor.lang.specialchar.toolbar,
                label: 'Insert Special Character',
                command: pluginName,
                toolbar: 'insert,50'
            });
        }
	}
});

/**
 * The list of special characters visible in the "Special Character" dialog window.
 *
 *		config.specialChars = [ '&quot;', '&rsquo;', [ '&custom;', 'Custom label' ] ];
 *		config.specialChars = config.specialChars.concat( [ '&quot;', [ '&rsquo;', 'Custom label' ] ] );
 *
 * @cfg
 * @member CKEDITOR.config
 */


var simpleScienceCharacters = [];
var simpleScienceCharacterSymbols = [];
var simpleScienceCharacterTitles = [];
var basicSciencePaletteSymbol = sessionStorage.getItem('basicSciencePaletteSymbol');
if (basicSciencePaletteSymbol) {
    //simpleScienceCharacters = basicSciencePaletteSymbol.split(',');
    var jsonObject = JSON.parse(basicSciencePaletteSymbol);
    for (var i in jsonObject) {
        simpleScienceCharacters.push(jsonObject[i]);
        simpleScienceCharacterSymbols = simpleScienceCharacterSymbols.concat(jsonObject[i].Value.split(','));
    }
    sessionStorage.removeItem('basicSciencePaletteSymbol');
}

for (var i = 0; i < simpleScienceCharacters.length; i++) {
    if (i == 0) {
        simpleScienceCharacterTitles.push({ 'Title': simpleScienceCharacters[i].Title, 'FirstIndex': 0, 'LastIndex': (simpleScienceCharacters[i].Value.split(',').length) });
    } else {
        var previousIndex = simpleScienceCharacterTitles[i - 1].LastIndex;
        simpleScienceCharacterTitles.push({ 'Title': simpleScienceCharacters[i].Title, 'FirstIndex': previousIndex ,'LastIndex': (simpleScienceCharacters[i].Value.split(',').length + previousIndex) });
    }
}

CKEDITOR.config.specialChars = [
	'+', '-', '&divide;', '&middot;', '&times;', '=', '&ne;', '&#8804;', '&#8805;', '&deg;', '&prime;', '&Prime;', '&#8730;', '~', '&#8776;', '&#8773;', '&plusmn;', '∓', '&#8736;',
	'&#8869;', '&#8741;', '&#960;', '&infin;', '&int;', '&cent;', '&#10230;', '&#945;', '&#946;', '&gamma;', '&#916;', '&#951;', '&#952;', '&kappa;', '&#955;', '&#956;', '&#961;', '&#931;', '&#963;', '&#964;',
	'&#981;', '&#936;', '&#937;', '&#969;', '&#8467;', '&nbsp;',
    '&Aacute;', '&Eacute;', '&Iacute;', '&Oacute;', '&Uacute;', '&Uuml;', '&Ntilde;', '?',
	'&iquest;', '&aacute;', '&eacute;', '&iacute;', '&oacute;', '&uacute;', '&uuml;', '&ntilde;', '!', '&iexcl;',
    '&Aacute;', '&aacute;', '&Agrave;', '&agrave;', '&Acirc;', '&acirc;', '&auml;', 
    '&Eacute;', '&eacute;', '&Egrave;', '&egrave;', '&Ecirc;', '&ecirc;', '&euml;',
    '&Iacute;', '&iacute;', '&Igrave;', '&igrave;', '&Icirc;', '&icirc;', '&iuml;',
    '&Oacute;', '&oacute;', '&Ograve;', '&ograve;', '&Ocirc;', '&ocirc;', '&ouml;', 
    '&Uacute;', '&uacute;', '&Ugrave;', '&ugrave;', '&Ucirc;', '&ucirc;', '&uuml;', 
    '&Ccedil;', '&ccedil;'
].concat(simpleScienceCharacterSymbols);

CKEDITOR.config.specialCharsSpanish = [
	'&Aacute;', '&Eacute;', '&Iacute;', '&Oacute;', '&Uacute;', '&Uuml;', '&Ntilde;', '?',
	'&iquest;', '&aacute;', '&eacute;', '&iacute;', '&oacute;', '&uacute;', '&uuml;', '&ntilde;', '!', '&iexcl;'
];
CKEDITOR.config.specialCharsMath = [
	'+', '-', '&divide;', '&middot;', '&times;', '=', '&ne;', '&#8804;', '&#8805;', '&deg;', '&prime;', '&Prime;', '&#8730;', '~', '&#8776;', '&#8773;', '&plusmn;', '∓', '&#8736;', //&mnplus;
	'&#8869;', '&#8741;', '&#960;', '&infin;', '&int;', '&cent;', '&#10230;', '&#945;', '&#946;', '&gamma;', '&#916;', '&#951;', '&#952;', '&kappa;', '&#955;', '&#956;', '&#961;', '&#931;', '&#963;', '&#964;',
	'&#981;', '&#936;', '&#937;', '&#969;', '&#8467;', '&#8304;', '&sup1;', '&sup2;', '&sup3;', '&#8308;', '&#8309;', '&#8310;', '&#8311;', '&#8312;', '&#8313;', '&#8314;', '&#8315;', '&#8320;', '&#8321;',
    '&#8322;', '&#8323;', '&#8324;', '&#8325;', '&#8326;', '&#8327;', '&#8328;', '&#8329;', '&#8330;', '&#8331;',
    '&Aacute;', '&Eacute;', '&Iacute;', '&Oacute;', '&Uacute;', '&Uuml;', '&Ntilde;', '?',
	'&iquest;', '&aacute;', '&eacute;', '&iacute;', '&oacute;', '&uacute;', '&uuml;', '&ntilde;', '!', '&iexcl;'

];
CKEDITOR.config.specialCharsFrench = [
	'&Aacute;', '&aacute;', '&Agrave;', '&agrave;', '&Acirc;', '&acirc;', '&auml;',
    '&Eacute;', '&eacute;', '&Egrave;', '&egrave;', '&Ecirc;', '&ecirc;', '&euml;',
    '&Iacute;', '&iacute;', '&Igrave;', '&igrave;', '&Icirc;', '&icirc;', '&iuml;',
    '&Oacute;', '&oacute;', '&Ograve;', '&ograve;', '&Ocirc;', '&ocirc;', '&ouml;',
    '&Uacute;', '&uacute;', '&Ugrave;', '&ugrave;', '&Ucirc;', '&ucirc;', '&uuml;',
    '&Ccedil;', '&ccedil;'
];
CKEDITOR.config.extraSpecialChars = [
	'+', '-', '&divide;', '&middot;', '&times;', '=', '&ne;', '&#8804;', '&#8805;', '&deg;', '&prime;', '&Prime;', '&#8730;', '~', '&#8776;', '&#8773;', '&plusmn;', '∓', '&#8736;', //&mnplus;
	'&#8869;', '&#8741;', '&#960;', '&infin;', '&int;', '&cent;', '&#10230;', '&#945;', '&#946;', '&gamma;', '&#916;', '&#951;', '&#952;', '&kappa;', '&#955;', '&#956;', '&#961;', '&#931;', '&#963;', '&#964;',
	'&#981;', '&#936;', '&#937;', '&#969;', '&#8467;', '&#8304;', '&sup1;', '&sup2;', '&sup3;', '&#8308;', '&#8309;', '&#8310;', '&#8311;', '&#8312;', '&#8313;', '&#8314;', '&#8315;', '&#8320;', '&#8321;',
    '&#8322;', '&#8323;', '&#8324;', '&#8325;', '&#8326;', '&#8327;', '&#8328;', '&#8329;', '&#8330;', '&#8331;',
    '&Aacute;', '&Eacute;', '&Iacute;', '&Oacute;', '&Uacute;', '&Uuml;', '&Ntilde;', '?',
	'&iquest;', '&aacute;', '&eacute;', '&iacute;', '&oacute;', '&uacute;', '&uuml;', '&ntilde;', '!', '&iexcl;',
    '&Aacute;', '&Eacute;', '&Iacute;', '&Oacute;', '&Uacute;', '&Uuml;', '&Ntilde;', '?',
	'&iquest;', '&aacute;', '&eacute;', '&iacute;', '&oacute;', '&uacute;', '&uuml;', '&ntilde;', '!', '&iexcl;',
    '&Aacute;', '&aacute;', '&Agrave;', '&agrave;', '&Acirc;', '&acirc;', '&auml;',
    '&Eacute;', '&eacute;', '&Egrave;', '&egrave;', '&Ecirc;', '&ecirc;', '&euml;',
    '&Iacute;', '&iacute;', '&Igrave;', '&igrave;', '&Icirc;', '&icirc;', '&iuml;',
    '&Oacute;', '&oacute;', '&Ograve;', '&ograve;', '&Ocirc;', '&ocirc;', '&ouml;',
    '&Uacute;', '&uacute;', '&Ugrave;', '&ugrave;', '&Ucirc;', '&ucirc;', '&uuml;',
    '&Ccedil;', '&ccedil;'
];
CKEDITOR.config.specialCharsNumeric = [
    '+', '-', '&divide;', '&times;', '=', '<', '>', '<=', '>=', '<>', '(', ')', '[', ']', '.', ','
];


CKEDITOR.config.mathPalette = [
    '+', '-', '&divide;', '&middot;', '&times;',
    '=', '&ne;', '&#8804;', '&#8805;', '&deg;',
    '&prime;', '&Prime;', '&#8730;', '~', '&#8776;',
    '&#8773;', '&plusmn;', '∓', '&#8736;', '&#8869;',
    '&#8741;', '&#960;', '&infin;', '&int;', '&cent;',
    '&#10230;', '&#945;', '&#946;', '&gamma;', '&#916;',
    '&#951;', '&#952;', '&kappa;', '&#955;', '&#956;',
    '&#961;', '&#931;', '&#963;', '&#964;', '&#981;', '&#936;',
    '&#937;', '&#969;', '&#8467;', '&nbsp;'
];

CKEDITOR.config.superscriptsPalette = [
    '&#8304;', '&sup1;', '&sup2;', '&sup3;', '&#8308;',
    '&#8309;', '&#8310;', '&#8311;', '&#8312;', '&#8313;',
    '&#8314;', '&#8315;'
];

CKEDITOR.config.subscriptsPalette = [
    '&#8320;', '&#8321;', '&#8322;', '&#8323;', '&#8324;',
    '&#8325;', '&#8326;', '&#8327;', '&#8328;', '&#8329;',
    '&#8330;', '&#8331;'
];


CKEDITOR.config.specialCharsSimpleScience = simpleScienceCharacters;
CKEDITOR.config.simpleScienceCharacterSymbols = simpleScienceCharacterSymbols;
CKEDITOR.config.simpleScienceCharacterTitles = simpleScienceCharacterTitles;
