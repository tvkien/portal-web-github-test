CKEDITOR.plugins.add('boxedtext', {
    lang: 'en', // %REMOVE_LINE_CORE%
    hidpi: true, // %REMOVE_LINE_CORE%
    init: function (editor) {
        var config = editor.config;
        var styles = {},
			allowedContent = [];
        var flag = false;
        
        editor.on('stylesSet', function (evt) {

            var stylesDefinitions = evt.data.styles;
            
            if (!stylesDefinitions)
                return;

            var style, styleName;

            // Put all styles into an Array.
            var styleDefinition = stylesDefinitions[2];
            styleName = styleDefinition.name;
            style = new CKEDITOR.style(styleDefinition);
            
            if (!editor.filter.customConfig || editor.filter.check(style)) {
                style._name = styleName;
                style._.enterMode = config.enterMode;

                styles[styleName] = style;
                allowedContent.push(style);
            }

        });
        
        editor.addCommand('insertBoxedText', {
            exec: function (editor) {

            var style = styles['Special Container']; 
            var elementText = editor.getSelection().getSelectedText();
            var eleBoxText = editor.getSelection().getStartElement();// case ele boxtext if has into item editor
            var demoEle = editor.getSelection().getRanges();// case ele boxtext if has into item editor
            var stringSpace = '<span class="whitespace">\u200b</span>';
            var txtHtml = '<span id="txtHtml">' + elementText + '</span>';
            var content = '';
            txtHtml = txtHtml.trim();
            txtHtml = txtHtml.replace(/[\s\n\r]+/g, "\u200b");
                
            if (elementText.trim() == "") {
                customAlert('Please highlight to add border or remove border');
            } else {
                noRemoveBr = false;
                noAppendBrEditor = true;
                var $iframes = $('iframe[allowtransparency]').contents().find('body');

                if (!$(eleBoxText.$).hasClass('boxedText') && !$(demoEle[0].startContainer.$).hasClass('boxedText')) {
                    flag = false;
                } else {
                    flag = true;
                }
                // fix bug in FB #36 https://devblock.atlassian.net/browse/LNKT-61226
                if (
                    !$(eleBoxText.$).hasClass('boxedText')
                    && $(eleBoxText.$).parent('.boxedText').length > 0
                    && $(eleBoxText.$).text() === $(eleBoxText.$).parent('.boxedText').text()
                ) {
                    flag = true;
                }
                if (flag) {
                    if (CKEDITOR.env.ie) {
                        editor.getSelection().getStartElement().removeAttribute('style');
                    }
                    noRemoveFirstBr = true;
                    editor['removeStyle'](style);
                    flag = false;
                } else {
                    noRemoveBr = true;
                    noAppendBrEditor = false;
                    editor['applyStyle'](style);
                    //$iframes.find('.boxedText').after("\u200b"); // Make sure user can focus behind boxes
                    
                    flag = true;
                }
            }
        }
        });
        
        editor.ui.addButton('BoxedText',
		{
		    label: 'Boxed Text',
		    title: 'Boxed Text',
		    command: 'insertBoxedText',
		    toolbar: 'insertBoxedText,10',
		    allowedContent: allowedContent
		});
	}
});
