/**
 * @license Copyright (c) 2003-2013, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function (config) {

    // %REMOVE_START%
    // The configuration options below are needed when running CKEditor from source files.
    config.plugins = 'pastetext,dialogui,dialog,about,a11yhelp,dialogadvtab,basicstyles,bidi,blockquote,clipboard,button,panelbutton,panel,floatpanel,colorbutton,colordialog,templates,menu,contextmenu,div,resize,toolbar,elementspath,enterkey,entities,popup,filebrowser,find,fakeobjects,flash,floatingspace,listblock,richcombo,font,forms,format,horizontalrule,htmlwriter,iframe,wysiwygarea,indent,indentblock,indentlist,smiley,justify,menubutton,language,list,liststyle,magicline,maximize,newpage,pagebreak,preview,print,removeformat,save,selectall,showblocks,showborders,sourcearea,specialchar,scayt,stylescombo,tab,table,tabletools,undo,wsc,xml,ajax,xmltemplates,lineutils,widget,mathjax,sharedspace,imageupload,audioupload,reference,addquestiontype,tableresize,link';
    config.skin = 'moonocolor';
    config.removePlugins = 'magicline,maximize,resize,image,div';
    config.allowedContent = true;
    config.ignoreEmptyParagraph = false;
    config.autoParagraph = false;
    config.forcePasteAsPlainText = true;
    config.enterMode = CKEDITOR.ENTER_BR;
    config.justifyClasses = ['alignLeft', 'center', 'alignRight', 'alignJustify'];
    config.UseBROnCarriageReturn = true;
    config.disableNativeSpellChecker = false;
    //config.enterMode = CKEDITOR.ENTER_P;
    // %REMOVE_END%

    // Define changes to default configuration here. For example:
    // config.language = 'fr';
    // config.uiColor = '#AADC6E';
};

CKEDITOR.on('dialogDefinition', function (ev) {
    var dialogName = ev.data.name;
    var dialogDefinition = ev.data.definition;
    if (dialogName == 'image') {
        dialogDefinition.removeContents('Link');
        dialogDefinition.removeContents('advanced');
        dialogDefinition.removeContents('Upload');
        var infoTab = dialogDefinition.getContents('info');
        infoTab.remove('ratioLock');
        infoTab.remove('txtHeight');
        infoTab.remove('txtWidth');
        infoTab.remove('txtBorder');
        infoTab.remove('txtHSpace');
        infoTab.remove('txtVSpace');
        //infoTab.remove( 'txtUrl' ); 
        infoTab.remove('txtAlt');
  }
  /*TuanDoan - add class for css dialog new UI*/
  if ($("#portal-v2-containter").length > 0) {
    if (dialogDefinition) {
      var dialog = dialogDefinition.dialog.parts.dialog
      if (dialog) {
        dialog.addClass('cke_editor_dialog_V2')
        dialog.addClass(dialogName)
      }
      if (dialogName == 'link') {
        var infoContent = dialogDefinition.getContents('info')
        var urlOptionsContent = infoContent.get("urlOptions")
        //get element warning content
        var vBox = urlOptionsContent.children[1]
        vBox.padding = '24px 4px 0 0 !important'
      }
      else if (dialogName == 'mathjax') {
        dialogDefinition.width = 632
        var mathjaxContent = dialogDefinition.dialog.parts.contents
        if (mathjaxContent) {
          mathjaxContent.addClass('mathJaxContent')
        }
      }
      else if (dialogName == 'mathfraction') {
        var mathFractionContent = dialogDefinition.dialog.parts.contents
        if (mathFractionContent) {
          mathFractionContent.addClass('mathFractionContent')
        }
      }
    }
  }
});
/*
//This code is for handle after command executes, Thinh Le just comment here if someone wanna handle it.
editor.on('afterCommandExec', handleAfterCommandExec);
function handleAfterCommandExec(event)
{
    var commandName = event.data.name;
    // For 'bold' commmand
    if (commandName == 'bold')
    alert("Bold button pressed!");
}
*/
CKEDITOR.dtd.$removeEmpty.sameline = 1;
CKEDITOR.dtd.$inline.sameline = 1;
