CKEDITOR.plugins.add('addquestiontype', {
    lang: 'en', // %REMOVE_LINE_CORE%
    icons: 'addquestiontype',
    requires: 'dialog',
    hidpi: true, // %REMOVE_LINE_CORE%
    init: function (editor) {

	
        editor.addCommand('insertAddQuestionType', {exec: function (editor) 
        {
            //Reset to ckeditor item type 21
            iSchemeID = "21";
            CKEDITOR.instances[ckID].config.qtiSchemeID = iSchemeID;
            $(".cke_button__multiplechoice_label").text("Multiple Choice").attr("title", "Multiple Choice");
            $(".cke_button__multiplechoice").parents("span.cke_toolbar").show();
            $(".cke_button__inlinechoice").parents("span.cke_toolbar").show();
            $(".cke_button__textentry").parents("span.cke_toolbar").show();
            $(".cke_button__drawtool").parents("span.cke_toolbar").show();
            $(".cke_button__extendtext").parents("span.cke_toolbar").show();
                //Hide add new item after show all question type
            $(".cke_button__addquestiontype").parents("span.cke_toolbar").hide();
			
			//Create checkbox for no duplicate answers
			var noDuplicateHTML = '<div><input type="checkbox" id="noDuplicate" /> <label for="noDuplicate">No Duplicate Answers</label</div><div class="clear10"></div>';
			$("#duplicateAnswer").append(noDuplicateHTML);
			}
		});

        editor.ui.addButton('AddQuestionType',
		{
		    label: 'Add More Item Type',
		    command: 'insertAddQuestionType',
		    icon: this.path + 'icons/addquestiontype.png',
		    toolbar: 'insertAddQuestionType,30'
		});
	}
});