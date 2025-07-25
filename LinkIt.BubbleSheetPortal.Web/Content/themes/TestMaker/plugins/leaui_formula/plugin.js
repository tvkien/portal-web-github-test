(function() {

  var init = false;

  // iframe plugin
  function initIfr() {
    if (init) {
      return;
    }
    init = true;

    /**
     * An iframe element.
     *
     * @class CKEDITOR.ui.dialog.iframeElement
     * @extends CKEDITOR.ui.dialog.uiElement
     * @constructor
     * @private
     * @param {CKEDITOR.dialog} dialog Parent dialog object.
     * @param {CKEDITOR.dialog.definition.uiElement} elementDefinition
     * The element definition. Accepted fields:
     *
     * * `src` (Required) The src field of the iframe.
     * * `width` (Required) The iframe's width.
     * * `height` (Required) The iframe's height.
     * * `onContentLoad` (Optional) A function to be executed
     *     after the iframe's contents has finished loading.
     *
     * @param {Array} htmlList List of HTML code to output to.
     */
    var iframeElement = function(dialog, elementDefinition, htmlList) {
      if (arguments.length < 3) return;

      var _ = (this._ || (this._ = {})),
      contentLoad = elementDefinition.onContentLoad && CKEDITOR.tools.bind(elementDefinition.onContentLoad, this),
      cssWidth = CKEDITOR.tools.cssLength(elementDefinition.width),
      cssHeight = CKEDITOR.tools.cssLength(elementDefinition.height);
      // _.frameId = CKEDITOR.tools.getNextId() + '_iframe';
      _.frameId = elementDefinition.id;
      _.frameName = elementDefinition.name;

      // IE BUG: Parent container does not resize to contain the iframe automatically.
      dialog.on('load',
      function() {
        var iframe = CKEDITOR.document.getById(_.frameId),
        parentContainer = iframe.getParent();

        parentContainer.setStyles({
          width: cssWidth,
          height: cssHeight
        });
      });

      var attributes = {
        src: '%2',
        id: _.frameId,
        name: _.frameName,
        frameborder: 0,
        scrolling: 'no',
        allowtransparency: true
      };
      var myHtml = [];

      if (typeof elementDefinition.onContentLoad == 'function') attributes.onload = 'CKEDITOR.tools.callFunction(%1);';

      CKEDITOR.ui.dialog.uiElement.call(this, dialog, elementDefinition, myHtml, 'iframe', {
        width: cssWidth,
        height: cssHeight
      },
      attributes, '');

      // Put a placeholder for the first time.
      htmlList.push('<div style="width:' + cssWidth + ';height:' + cssHeight + ';" id="' + this.domId + '"></div>');

      // Iframe elements should be refreshed whenever it is shown.
      myHtml = myHtml.join('');
      dialog.on('show',
      function() {
        var iframe = CKEDITOR.document.getById(_.frameId),
        parentContainer = iframe.getParent(),
        callIndex = CKEDITOR.tools.addFunction(contentLoad),
        html = myHtml.replace('%1', callIndex).replace('%2', CKEDITOR.tools.htmlEncode(elementDefinition.src));

        parentContainer.setHtml(html);
      });
    };

    iframeElement.prototype = new CKEDITOR.ui.dialog.uiElement();

    CKEDITOR.dialog.addUIElement('iframeLeaui', {
      build: function(dialog, elementDefinition, output) {
        return new iframeElement(dialog, elementDefinition, output);
      }
    });
  }

  var j = 0;
  // main
  CKEDITOR.plugins.add('leaui_formula', {
    requires: 'dialog',
    // requires: ['iframedialog'],
    init: function(editor) {
      initIfr();
      var me = this;
      var height = 382, width = 822;

      CKEDITOR.dialog.add('myiframedialogDialog',
        function(editor) {
          window.CKEDITOR_LEAUI_FORMULAR = editor;
          j++;
          var id = 'leauiFormulaIfr' + j;
          return {
            title: 'Formula',
            minWidth: width,
            minHeight: height,
            contents: [{
              id: 'iframe',
              label: 'Insert Formula',
              expand: true,
              elements: [{
                type: 'iframeLeaui',  
                name: id,
                id: id,
                src: me.path + 'index.html?v=20241122',
                width: "100%",
                height: height,
                onContentLoad: function() {}
              }]
            }],

            buttons: [
              // CKEDITOR.dialog.okButton,
              // http://docs.ckeditor.com/#!/api/CKEDITOR.dialog.definition.buttons
              CKEDITOR.dialog.cancelButton,
              {
                 id: 'Insert_Formula',
                 label: 'Insert Formula',
                 title: 'Insert Formula',
                 type: 'button',
                 className: 'cke_dialog_ui_button_ok',
                 disabled: false,
                 onClick: function() {
                    var me = this;
                    if (window.frames[id] && window.frames[id].getData) {
                      window.frames[id].getData(function(src, latex) {
                        if (src) {
                          var formulaHtml = '<img class="imageupload" percent="10" data-math-type="leaui_fomula" src="' + src + '" data-latex="' + latex + '"/>';
                          var formulaElement = CKEDITOR.dom.element.createFromHtml(formulaHtml);
                          editor.insertElement(formulaElement);
                        }
                        CKEDITOR.dialog.getCurrent().hide();
                      });
                    } else {
                      CKEDITOR.dialog.getCurrent().hide();
                    }
                 }
              }
            ]
          };
        }
      );

      var allowed = 'img[data-latex,alt,!src]{border-style,border-width,float,height,margin,margin-bottom,margin-left,margin-right,margin-top,width}';
      editor.addCommand('leauiFormulaDialog', new CKEDITOR.dialogCommand('myiframedialogDialog', {
        allowedContent: allowed,
        a: 'life',
        refresh: function() {
          this.setState( CKEDITOR.TRISTATE_DISABLED );
        }
      }));

      // button
      editor.ui.addButton('LeauiFormula', {
        label: 'Insert Formula',
        command: 'leauiFormulaDialog',
        icon: this.path + 'icons/icon.png',
        toolbar: 'insert',
        refresh: function() {
          this.setState( CKEDITOR.TRISTATE_DISABLED );
        }
      });
      
      // menu
      /*
      if(editor.addMenuItems) {
        editor.addMenuItems({
         leauiFormulaDialog : {
            label: 'Insert Formula',
            command: 'leauiFormulaDialog',
            group: 'insert',
            order: 1
          }
        });
      }
      */

      // change button's state
      editor.on('selectionChange', function(evt) {
        if(editor.readOnly) {
          return;
        }
        var btn = editor.ui.get('LeauiFormula');
        var element = editor.getSelection().getSelectedElement();
        if(element && element.is('img')) {
          var latex = element.data('latex');
          if(latex) {
            btn.setState(CKEDITOR.TRISTATE_ON);
            return;
          }
        }
        btn.setState(CKEDITOR.TRISTATE_OFF);
      });
    }
  });
})();