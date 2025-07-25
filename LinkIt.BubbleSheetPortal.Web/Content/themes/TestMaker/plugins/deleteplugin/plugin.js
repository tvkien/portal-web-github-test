CKEDITOR.plugins.add('deleteplugin', {
    lang: 'en', // %REMOVE_LINE_CORE%
    hidpi: true, // %REMOVE_LINE_CORE%
    init: function (editor) {
        editor.addCommand('insertDeletePlugin', {exec: function (editor) 
        {
            var element = editor.getSelection().getSelectedElement();
            if (element == null) {
                if (CKEDITOR.env.ie) {
                    var doc = editor.document;
                    if ($(doc.$.body).find('.active-border').hasClass('active-border')) {
                        $(doc.$.body).find('.active-border').html('outerHTML').remove();
                    } else {
                        alert('Select which sub-part you would like to delete.');
                    }
                } else {
                    alert('Select which sub-part you would like to delete.');
                }   
               
            } else {
                var eleParent = element.getParent();
                eleParent.remove();
            }
        }
        });
        
        editor.ui.addButton('DeletePlugin',
		{
		    label: 'Delete',
		    title: 'Delete',
		    command: 'insertDeletePlugin',
		    toolbar: 'insertDeletePlugin,10'
		});
	}
});