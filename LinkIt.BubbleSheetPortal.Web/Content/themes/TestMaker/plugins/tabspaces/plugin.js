/**
 * Insert 5 spaces at the current position in CKEditor
 * @return {[type]} [description]
 */
(function() {
    'use strict';

    CKEDITOR.plugins.add('tabspaces', {
        icons: 'tabspaces',
        hidpi: true,
        init: function(editor) {
            // Create tabspaces toolbar button.
            if (editor.ui.addButton) {
                editor.ui.addButton('Tabspaces', {
                    label: 'tabspaces',
                    title: 'Tab Spaces',
                    command: 'tabspaces',
                    toolbar: 'tabspaces,10'
                });
            }

            // Execute 5 spaces every click button
            editor.addCommand('tabspaces', {
                exec: function(editor) {
                    var spaces = '&#160;&#160;&#160;&#160;&#160;';

                    editor.insertHtml(spaces);
                }
            });
        }
    });
})();
