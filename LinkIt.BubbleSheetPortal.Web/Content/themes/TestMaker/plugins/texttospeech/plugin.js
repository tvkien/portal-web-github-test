'use strict';

(function() {

    var pluginName = 'texttospeech';

    CKEDITOR.plugins.add(pluginName, {
        lang: 'en',
        icons: pluginName,
        hidpi: true,
        init: function(editor) {
            var lang = editor.lang.texttospeech;
            var cmd = new CKEDITOR.dialogCommand(pluginName);

            editor.addCommand(pluginName, cmd);

            CKEDITOR.dialog.add(pluginName, CKEDITOR.getUrl(this.path + 'dialogs/texttospeech.js'));

            editor.ui.addButton(lang.button, {
                label: lang.title,
                command: pluginName,
                toolbar: 'insert,40'
            });
        }
    })
})();
