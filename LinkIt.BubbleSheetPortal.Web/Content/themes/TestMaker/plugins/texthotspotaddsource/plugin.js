CKEDITOR.plugins.add('texthotspotaddsource',
{
   requires : ['richcombo'], //, 'styles' ],
   init : function( editor )
   {
      var config = editor.config,
         lang = editor.lang.format;

      // Gets the list of tags from the settings.
      var tags = []; //new Array();
      tags[0] = ["labeltexthotspot", "Add/Remove Hot Spot", "Add/Remove Hot Spot"];
      tags[1] = ["propertiestexthotspot", "Set the point(s)", "Set the point(s)"];

      // Create style objects for all defined styles.

      editor.ui.addRichCombo('TextHotSpotAddSource',
         {
            label: "Text Hot Spot",
            title: "Text Hot Spot",
            voiceLabel: "Text Hot Spot",
            toolbar: 'styles,10',
            panel: {
                css: [CKEDITOR.skin.getPath('editor')].concat(config.contentsCss),
                multiSelect: false,
                attributes: { 'aria-label': '', 'title': '' }
            },
            init : function()
            {

               for (var this_tag in tags){
                  this.add(tags[this_tag][0], tags[this_tag][1], tags[this_tag][2]);
               }
               
            },

            onClick : function(value)
            {
                if (value == "labeltexthotspot")
                {
                    editor.execCommand('insertTextHotSpot');
                } else if (value == "propertiestexthotspot") {
					if($("<div>"+ editor.getData() +"</div>").find(".marker-linkit").length == 0){
						customAlert("Please add text hot spots before set the point(s).");
					} else {
						setTimeout(function () { editor.execCommand('textHotSpotProperties'); }, 100);
					}
                    
                }
            }
         });
   }
});