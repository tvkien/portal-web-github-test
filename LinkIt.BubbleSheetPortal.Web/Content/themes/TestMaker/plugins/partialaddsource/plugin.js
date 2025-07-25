CKEDITOR.plugins.add('partialaddsource', {
	requires : ['richcombo'], //, 'styles' ],
	init : function( editor ) {
		var config = editor.config;

		// Gets the list of tags from the settings.
		var tags = [];
		var tags1 = [];
		var tags2 = [];

		// Display drag and drop by schemeID
		// Add by way: this.add('value', 'drop_text', 'drop_label');
		if (iSchemeID === '35') {
			// Drag and drop numberical
		    tags[0] = ['dragdropnumericalsource', 'Numeric Draggable', 'Numeric Draggable'];
		    tags1[0] = ['dragdropnumericaldestination', 'Numeric Destination', 'Numeric Destination'];
		    tags2[0] = ['dragdropnumericalrelationship', 'Numeric Relationship', 'Numeric Relationship'];
		} else {
			tags[0] = ['labelsource', 'Text Label', 'Text Label'];
			tags[1] = ['imagesource', 'Image', 'Image'];

			tags1[0] = ['textdestination', 'Text Field', 'Text Field'];
			tags1[1] = ['imagedestination', 'Image', 'Image'];

			tags2[0] = ['propertiesdestination', 'Properties', 'Properties'];
		}

		// Create style objects for all defined styles.
		editor.ui.addRichCombo('PartialAddSource', {
            label: 'Drag and Drop',
            title: 'Drag and Drop',
            voiceLabel: 'Drag and Drop',
            toolbar: 'styles,10',
            panel: {
                css: [CKEDITOR.skin.getPath('editor')].concat(config.contentsCss),
                multiSelect: false,
                voiceLabel: 'haha',
                attributes: { 'aria-label': '', 'title': '' }
            },
            init : function() {
				var this_tag;
                this.startGroup('Draggable Object');

				for (this_tag in tags){
					this.add(tags[this_tag][0], tags[this_tag][1], tags[this_tag][2]);
				}
				this.startGroup('Destination Object');

				for (this_tag in tags1) {
					this.add(tags1[this_tag][0], tags1[this_tag][1], tags1[this_tag][2]);
				}
				this.startGroup('Properties');

				for (this_tag in tags2) {
					this.add(tags2[this_tag][0], tags2[this_tag][1], tags2[this_tag][2]);
				}
            },
            onClick : function(value) {
				var i = 0;
				var iResultChild;
				var destinationPartial = [];
				var newDesIdentifier;
				var lenIResult = iResult.length;

                if (value == 'labelsource') {
                    editor.focus();
                    editor.fire('saveSnapshot');
                    refreshPartialCredit();
                    //check srcIdentifier
                    var sourcePartial = [];
                    for (i = 0; i < lenIResult; i++) {
                        if (iResult[i].type == 'partialCredit') {
                            sourcePartial = iResult[i].source;
                        }
                    }

                    //Create srcIdentifier
                    var newsrcIdentifier = createSrcIdentifier(sourcePartial);
                    var sourceHtml = '<span class="partialSourceObject partialAddSourceText" unselectable="on" contenteditable="false" style="width: 120px; height: 40px;" srcIdentifier="' + newsrcIdentifier + '" partialID="Partial_1"><img class="cke_reset cke_widget_mask partialAddSourceTextMark" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" /><span>Text Label</span></span>&nbsp;';
                    var sourceElement = CKEDITOR.dom.element.createFromHtml(sourceHtml);
                    editor.insertElement(sourceElement);

                    // Set limit data dragged for add text source
                    sourcePartial.push({
						srcIdentifier: newsrcIdentifier,
						type: 'text',
						value: 'Text Label',
						limit: 'unlimited'
                    });

                    //This is step to add source to iResult
                    for (i = 0; i < lenIResult; i++) {
                        if (iResult[i].type == 'partialCredit') {
                            iResult[i].source = sourcePartial;
                        }
                    }

                    editor.fire('saveSnapshot');
                } else if (value == 'imagesource') {
                    setTimeout(function () {
						editor.execCommand('insertPartialAddSourceImage');
                    }, 100);
                } else if (value == 'textdestination') {
                    editor.focus();
                    editor.fire('saveSnapshot');
                    refreshPartialCredit();

                    var correctResponse = [];
                    for (i = 0; i < lenIResult; i++) {
                        if (iResult[i].type == 'partialCredit') {
                            destinationPartial = iResult[i].destination;
                            correctResponse = iResult[i].correctResponse;
                        }
                    }

                    //Create srcIdentifier
                    newDesIdentifier = createDestinationIdentifier(destinationPartial);
                    var destinationHtml = '<span class="partialDestinationObject partialAddDestinationText" unselectable="on" type="text" contenteditable="false"  style="width: 120px; height: 40px;" destIdentifier="' + newDesIdentifier + '"><img class="cke_reset cke_widget_mask partialAddDestinationTextMark" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" /><span>' + newDesIdentifier + '</span></span>&nbsp;';
                    var destinationElement = CKEDITOR.dom.element.createFromHtml(destinationHtml);
                    editor.insertElement(destinationElement);

                    destinationPartial.push({
						destIdentifier: newDesIdentifier,
						type: 'text'
                    });

                    correctResponse.push({
						order: (correctResponse.length + 1).toString(),
						destIdentifier: newDesIdentifier,
						srcIdentifier: ''
                    });

                    //This is step to add source to iResult
                    for (i = 0; i < lenIResult; i++) {
                        if (iResult[i].type == 'partialCredit') {
                            iResult[i].destination = destinationPartial;
                            iResult[i].correctResponse = correctResponse;
                        }
                    }

                    editor.fire('saveSnapshot');
                } else if (value == 'imagedestination') {
                    setTimeout(function () {
						editor.execCommand('insertPartialAddDestinationImage');
                    }, 100);
                } else if (value == 'propertiesdestination') {
                    setTimeout(function () {
						editor.execCommand('insertPartialAddProperties');
                    }, 100);
                } else if (value == 'dragdropnumericalsource') {
                  var srcDragDropNumeric = [];
                  var uniqueIdentifier;
                  var uniqueValue = '1';
                  editor.focus();
                  editor.fire('saveSnapshot');
                  refreshPartialCredit();

                  // Load source drag and drop numerical from iResult
                  for (i = 0; i < lenIResult; i++) {
                    iResultChild = iResult[i];

                    if (iResultChild.type === 'dragDropNumerical') {
                      srcDragDropNumeric = iResultChild.source;
                    }
                  }

                  uniqueIdentifier = createSrcIdentifier(srcDragDropNumeric);
                  var sourceobjectHtml = '<span class="partialSourceObject partialAddSourceNumerical" unselectable="on" contenteditable="false" style="width: 30px; height: 20px;" srcIdentifier="' + uniqueIdentifier + '" data-limit="unlimited"><img class="cke_reset cke_widget_mask partialDragDropNumericalSourceMark" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" />' + uniqueValue + '</span>&nbsp;';
                  var sourceobjectElement = CKEDITOR.dom.element.createFromHtml(sourceobjectHtml);
                  editor.insertElement(sourceobjectElement);

                  srcDragDropNumeric.push({
                    srcIdentifier: uniqueIdentifier,
                    value: uniqueValue,
                    type: 'text',
                    limit: 'unlimited'
                  });

                  // Add source drag and drop numerical to iResult
                  for (i = 0; i < lenIResult; i++) {
                    iResultChild = iResult[i];

                    if (iResultChild.type === 'dragDropNumerical') {
                      iResultChild.source = srcDragDropNumeric;
                    }
                  }

                  editor.fire('saveSnapshot');
                } else if (value == 'dragdropnumericaldestination') {
                    editor.focus();
                    editor.fire('saveSnapshot');
                    refreshPartialCredit();

                    for (i = 0; i < lenIResult; i++) {
                        if (iResult[i].type == 'dragDropNumerical') {
                            destinationPartial = iResult[i].destination;
                        }
                    }

                    //Create srcIdentifier
                    newDesIdentifier = createDestinationIdentifier(destinationPartial);

                    var destinationobjectHtml = '<span class="partialDestinationObject partialAddDestinationNumerical" unselectable="on" type="text" contenteditable="false" numberDroppable="1" style="width: 55px; height: 20px;" destIdentifier="' + newDesIdentifier + '"><img class="cke_reset cke_widget_mask partialDragDropNumericalDestinationMark" src="data:image/gif;base64,R0lGODlhAQABAPABAP///wAAACH5BAEKAAAALAAAAAABAAEAAAICRAEAOw%3D%3D" />' + newDesIdentifier + '</span>&nbsp;';
                    var destinationobjectElement = CKEDITOR.dom.element.createFromHtml(destinationobjectHtml);
                    editor.insertElement(destinationobjectElement);

                    destinationPartial.push({
						destIdentifier: newDesIdentifier,
						type: 'text'
                    });

                    //This is step to add source to iResult
                    for (i = 0; i < lenIResult; i++) {
                        if (iResult[i].type == 'numberDroppable') {
                            iResult[i].destination = destinationPartial;
                        }
                    }

                    editor.fire('saveSnapshot');
                } else if (value == 'dragdropnumericalrelationship') {
                  setTimeout(function () {
					editor.execCommand('dragdropnumericalrelationship');
                  }, 100);
                }
            }
		});
	}
});
