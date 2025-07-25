var a = {
    exec: function (editor) {
        addReferenceCallBack();
    }
};
CKEDITOR.plugins.add('reference', {
    lang: 'en', // %REMOVE_LINE_CORE%
    icons: 'reference',
    hidpi: true, // %REMOVE_LINE_CORE%
    init: function (editor) {

        editor.addCommand('insertReference', a);

        editor.ui.addButton('Reference',
		{
		    label: 'Add Reference/Passage',
		    command: 'insertReference',
		    icon: this.path + 'icons/reference.png',
		    toolbar: 'insertReference,30'
		});

        editor.widgets.add('reference', {
            inline: true,
            mask: true
        });
    }
});