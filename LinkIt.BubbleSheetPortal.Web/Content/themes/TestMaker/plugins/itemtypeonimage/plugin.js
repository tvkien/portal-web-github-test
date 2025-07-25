(function () {
    var pluginName = 'itemtypeonimage';

    CKEDITOR.plugins.add(pluginName, {
        lang: 'en',
        icons: pluginName,
        init: function (editor) {
            var lang = editor.lang.itemtypeonimage;

            // Add command dialog
            editor.addCommand(pluginName, new CKEDITOR.dialogCommand('itemTypeOnImage'));

            // Add button toolbar
            editor.ui.addButton('ItemTypeOnImage', {
                'label': lang.label,
                'command': pluginName,
                'toolbar': 'insert',
                'icons': CKEDITOR.getUrl(this.path + 'icons/itemtypeonimage.png')
            });

            // Double click event call dialog
            editor.on('doubleclick', function (evt) {
                var element = evt.data.element;

                if (element.hasClass('itemtypeonimageMark')) {
                    window.editItemtypeonimage = true;
                    editor.elementItemTypeOnImage = element.getParent();
                    evt.data.dialog = 'itemTypeOnImage';
                }
            })

            CKEDITOR.dialog.add('itemTypeOnImage', CKEDITOR.getUrl(this.path + 'dialogs/itemtypeonimage.js'));
        }
    })

})();
