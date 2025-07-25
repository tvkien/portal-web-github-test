CKEDITOR.plugins.add('dropdownsubpart',
{
   requires : ['richcombo'], //, 'styles' ],
   init : function( editor )
   {
      var config = editor.config,
         lang = editor.lang.format;

       // Gets the list of tags from the settings.
       //this.add('value', 'drop_text', 'drop_label');
      var tags = [
          ["lmultipchoice", "Multiple Choice", " "],
          ["linlinechoice", "Inline Choice", " "],
          ["ltextentry", "Fill-in-the-Blank", " "],
          ["lextendtext", "Constructed Response", " "],
          ["ltruefalse", "True/False", " "],
          ["ldrawtool", "Drawing Response", " "],
          ["lmultipart", "Multi-Part Properties", " "]
      ];


      // Create style objects for all defined styles.

      editor.ui.addRichCombo('DropdownSubpart',
      {
            label: "Add New Sub-Part",
            title: "Add New Sub-Part",
            voiceLabel: "Add New Sub-Part",
            toolbar: 'styles,10',
            panel: {
                css: [CKEDITOR.skin.getPath('editor')].concat(config.contentsCss),
                multiSelect: false,

            },
            init : function()
            {
               //this.startGroup( "Tokens" );
               //this.add('value', 'drop_text', 'drop_label');

               for (var tag in tags){
                  this.add(tags[tag][0], tags[tag][1], tags[tag][2]);
               }
            },

            onClick: function (value) {

                var label = value;
                var isALlOrNothingGradingMode = modeMultiPartGrading == 'all-or-nothing-grading'
                switch (label) {
                    case 'ltextentry':
                        editor.focus();
                        editor.fire('saveSnapshot');
                        //show all plugins on toolbar
                        $('.tool-tip-tips').css({
                            'top': '0px',
                            'opacity': 0,
                            'display': 'none'
                        });
                        setTimeout(function () { editor.execCommand('insertTextEntry'); }, 100);
                        editor.fire('saveSnapshot');
                        break;
                    case 'linlinechoice':
                        editor.focus();
                        editor.fire('saveSnapshot');
                        //show all plugins on toolbar
                        $('.tool-tip-tips').css({
                            'top': '0px',
                            'opacity': 0,
                            'display': 'none'
                        });
                        setTimeout(function () { editor.execCommand('insertInlineChoice'); }, 100);
                        editor.fire('saveSnapshot');
                        break;
                    case 'ldrawtool':
                        editor.focus();
                        editor.fire('saveSnapshot');
                        //show all plugins on toolbar
                        $('.tool-tip-tips').css({
                            'top': '0px',
                            'opacity': 0,
                            'display': 'none'
                        });
                        if (isALlOrNothingGradingMode) {
                          customAlert('This question type is not supported All or Nothing Grading.');
                          return
                        }
                        setTimeout(function () { editor.execCommand('insertDrawTool'); }, 100);
                        editor.fire('saveSnapshot');
                        break;
                    case 'lextendtext':
                        editor.focus();
                        editor.fire('saveSnapshot');
                        //show all plugins on toolbar
                        $('.tool-tip-tips').css({
                            'top': '0px',
                            'opacity': 0,
                            'display': 'none'
                        });
                        if (isALlOrNothingGradingMode) {
                          customAlert('This question type is not supported All or Nothing Grading.');
                          return
                        }
                        setTimeout(function () { editor.execCommand('insertExtendText'); }, 100);
                        editor.fire('saveSnapshot');
                        break;
                    case 'lmultipchoice':
                        editor.focus();
                        editor.fire('saveSnapshot');
                        //show all plugins on toolbar
                        $('.tool-tip-tips').css({
                            'top': '0px',
                            'opacity': 0,
                            'display': 'none'
                        });
                        isTrueFalse = false;
                        setTimeout(function () { editor.execCommand('insertMultipleChoice'); }, 100);
                        editor.fire('saveSnapshot');
                        break;
                    case 'lmultipchoicevariable':
                        editor.focus();
                        editor.fire('saveSnapshot');
                        //show all plugins on toolbar
                        $('.tool-tip-tips').css({
                            'top': '0px',
                            'opacity': 0,
                            'display': 'none'
                        });
                        setTimeout(function () { editor.execCommand('insertMultipleChoiceVariable'); }, 100);
                        editor.fire('saveSnapshot');
                        break;
                    case 'ltruefalse':
                        editor.focus();
                        editor.fire('saveSnapshot');
                        //show all plugins on toolbar
                        $('.tool-tip-tips').css({
                            'top': '0px',
                            'opacity': 0,
                            'display': 'none'
                        });
                        isTrueFalse = true;
                        setTimeout(function () { editor.execCommand('insertMultipleChoice'); }, 100);
                        editor.fire('saveSnapshot');
                        break;
                    case 'lmultipart':
                        editor.focus();
                        editor.fire('saveSnapshot');
                        //show all plugins on toolbar
                        $('.tool-tip-tips').css({
                            'top': '0px',
                            'opacity': 0,
                            'display': 'none'
                        });

                        setTimeout(function () { editor.execCommand('dependentGrading'); }, 100);
                        editor.fire('saveSnapshot');
                        break;
                    default:
                        break;
                }
            }
     });
   }
  });
