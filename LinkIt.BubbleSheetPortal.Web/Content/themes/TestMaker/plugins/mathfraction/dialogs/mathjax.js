/**
 * @license Copyright (c) 2003-2013, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

'use strict';

CKEDITOR.dialog.add( 'mathfraction', function( editor ) {

	var preview,
		lang = editor.lang.mathfraction,
		numTop = '1',
		numBot = 'x',
		superScript1 = '',
		subScript1 = '',
		superScript2 = '',
		subScript2 = '',
		usingScript = false;

	return {
		title: lang.title,
		minWidth: 350,
		minHeight: 100,
		contents: [
			{
				id: 'info',
				elements: [
					{
					    type: 'hbox',
					    widths: [ '75%', '25%' ],
					    children: [
					        {
					            type: 'text',
					            id: 'topFraction',
					            onLoad: function( widget ) {

					            	var id = this.domId;

									$('#'+id).parent('td').css('vertical-align', 'middle');

									var that = this;

									if ( !( CKEDITOR.env.ie && CKEDITOR.env.version == 8 ) ) {
										this.getInputElement().on( 'keyup', function() {
											// Old code remove \( and )\
											//preview.setValue( '\\(' + that.getInputElement().getValue() + '\\)' );
											numTop = that.getInputElement().getValue();

											if(usingScript == true){
											    preview.setValue('<math class="simpleFraction" mode="display" xmlns="http://www.w3.org/1998/Math/MathML"><mstyle displaystyle="true"><mrow class="MJX-TeXAtom-ORD"><mfrac><msubsup><mo>' + numTop + '</mo><mn>' + subScript1 + '</mn><mn>' + superScript1 + '</mn></msubsup><msubsup><mo>' + numBot + '</mo><mn>' + subScript2  + '</mn><mn>' + superScript2 + '</mn></msubsup></mfrac></mrow></mstyle></math>');
											}else{
											    preview.setValue('<math class="simpleFraction" xmlns="http://www.w3.org/1998/Math/MathML"><mstyle displaystyle="true"><mrow class="MJX-TeXAtom-ORD"><mfrac><mn>' + numTop + '</mn><mn>' + numBot + '</mn></mfrac></mrow></mstyle></math>');
											}
											
										} );
									}
					            },

								commit: function( widget ) {

									numTop = this.getValue();

									if(superScript1 == '' && subScript1 == '' && superScript2 == '' && subScript2 == ''){
										usingScript = false;
									}

									if(usingScript == true){
									    widget.setData('math', '<math class="simpleFraction" mode="display" xmlns="http://www.w3.org/1998/Math/MathML"><mstyle displaystyle="true"><mrow class="MJX-TeXAtom-ORD"><mfrac><msubsup><mo>' + numTop + '</mo><mn>' + subScript1 + '</mn><mn>' + superScript1 + '</mn></msubsup><msubsup><mo>' + numBot + '</mo><mn>' + subScript2  + '</mn><mn>' + superScript2 + '</mn></msubsup></mfrac></mfrac></mrow></mstyle></math>');
									}else{
									    widget.setData('math', '<math class="simpleFraction" xmlns="http://www.w3.org/1998/Math/MathML"><mstyle displaystyle="true"><mrow class="MJX-TeXAtom-ORD"><mfrac><mn>' + numTop + '</mn><mn>' + numBot + '</mn></mfrac></mrow></mstyle></math>');
									}
								}
					        },
					        {
								type : 'vbox',
								children :
								[
									{
										type : 'text',
										id : 'superScript1',
										onLoad: function( widget ) {

											var that = this;

											if ( !( CKEDITOR.env.ie && CKEDITOR.env.version == 8 ) ) {
												this.getInputElement().on( 'keyup', function() {

													if(usingScript == false){
														usingScript = true;
													}

													superScript1 = that.getInputElement().getValue();

													if(superScript1 == '' && subScript1 == '' && superScript2 == '' && subScript2 == ''){
														usingScript = false;
													}

													if(usingScript == true){
														preview.setValue( '<math class="simpleFraction" mode="display" xmlns="http://www.w3.org/1998/Math/MathML"><mrow><mfrac><msubsup><mo>' + numTop + '</mo><mn>' + subScript1 + '</mn><mn>' + superScript1 + '</mn></msubsup><msubsup><mo>' + numBot + '</mo><mn>' + subScript2  + '</mn><mn>' + superScript2 + '</mn></msubsup></mfrac></mrow></math>' );
													}else{
														preview.setValue( '<math class="simpleFraction" xmlns="http://www.w3.org/1998/Math/MathML"><mfrac><mn>' + numTop +'</mn><mn>'+ numBot +'</mn></mfrac></math>' );
													}
												} );
											}
							            },

										commit: function( widget ) {

											superScript1 = this.getValue();

											if(superScript1 == '' && subScript1 == '' && superScript2 == '' && subScript2 == ''){
												usingScript = false;
											}

											if(usingScript == true){
											    widget.setData('math', '<math class="simpleFraction" mode="display" xmlns="http://www.w3.org/1998/Math/MathML"><mstyle displaystyle="true"><mrow class="MJX-TeXAtom-ORD"><mfrac><msubsup><mo>' + numTop + '</mo><mn>' + subScript1 + '</mn><mn>' + superScript1 + '</mn></msubsup><msubsup><mo>' + numBot + '</mo><mn>' + subScript2  + '</mn><mn>' + superScript2 + '</mn></msubsup></mfrac></mrow></mstyle></math>');
											}else{
											    widget.setData('math', '<math class="simpleFraction" xmlns="http://www.w3.org/1998/Math/MathML"><mstyle displaystyle="true"><mrow class="MJX-TeXAtom-ORD"><mfrac><mn>' + numTop + '</mn><mn>' + numBot + '</mn></mfrac></mrow></mstyle></math>');
											}
										}
									},
									{
										type : 'text',
										id : 'subScript1',
										onLoad: function( widget ) {

											var that = this;

											if ( !( CKEDITOR.env.ie && CKEDITOR.env.version == 8 ) ) {
												this.getInputElement().on( 'keyup', function() {

													if(usingScript == false){
														usingScript = true;
													}
													// Old code remove \( and )\
													//preview.setValue( '\\(' + that.getInputElement().getValue() + '\\)' );
													subScript1 = that.getInputElement().getValue();

													if(superScript1 == '' && subScript1 == '' && superScript2 == '' && subScript2 == ''){
														usingScript = false;
													}

													if (usingScript == true) {
													    preview.setValue('<math class="simpleFraction" mode="display" xmlns="http://www.w3.org/1998/Math/MathML"><mstyle displaystyle="true"><mrow class="MJX-TeXAtom-ORD"><mfrac><msubsup><mo>' + numTop + '</mo><mn>' + subScript1 + '</mn><mn>' + superScript1 + '</mn></msubsup><msubsup><mo>' + numBot + '</mo><mn>' + subScript2  + '</mn><mn>' + superScript2 + '</mn></msubsup></mfrac></mrow></mstyle></math>');
													} else {
													    preview.setValue('<math class="simpleFraction" xmlns="http://www.w3.org/1998/Math/MathML"><mstyle displaystyle="true"><mrow class="MJX-TeXAtom-ORD"><mfrac><mn>' + numTop + '</mn><mn>' + numBot + '</mn></mfrac></mrow></mstyle></math>');
													}
												} );
											}
							            },

										commit: function( widget ) {
											subScript1 = this.getValue();
											if(superScript1 == '' && subScript1 == '' && superScript2 == '' && subScript2 == ''){
												usingScript = false;
											}

											if (usingScript == true) {
											    widget.setData('math', '<math class="simpleFraction" mode="display" xmlns="http://www.w3.org/1998/Math/MathML"><mstyle displaystyle="true"><mrow class="MJX-TeXAtom-ORD"><mfrac><msubsup><mo>' + numTop + '</mo><mn>' + subScript1 + '</mn><mn>' + superScript1 + '</mn></msubsup><msubsup><mo>' + numBot + '</mo><mn>' + subScript2  + '</mn><mn>' + superScript2 + '</mn></msubsup></mfrac></mrow></mstyle></math>');
											} else {
											    widget.setData('math', '<math class="simpleFraction" xmlns="http://www.w3.org/1998/Math/MathML"><mstyle displaystyle="true"><mrow class="MJX-TeXAtom-ORD"><mfrac><mn>' + numTop + '</mn><mn>' + numBot + '</mn></mfrac></mrow></mstyle></math>');
											}
										}
									}
								]
							}
					    ]
					},
					{
					    type: 'hbox',
					    widths: [ '75%', '25%' ],
					    children: [
					        {
					            type: 'text',
					            id: 'bottomFraction',
					            onLoad: function( widget ) {
					            	var id = this.domId;

									$('#'+id).parent('td').css('vertical-align', 'middle');

									var that = this;

									if ( !( CKEDITOR.env.ie && CKEDITOR.env.version == 8 ) ) {
										this.getInputElement().on( 'keyup', function() {
											// Old code remove \( and )\
											//preview.setValue( '\\(' + that.getInputElement().getValue() + '\\)' );
											numBot = that.getInputElement().getValue();

											if (usingScript == true) {
											    preview.setValue('<math class="simpleFraction" mode="display" xmlns="http://www.w3.org/1998/Math/MathML"><mstyle displaystyle="true"><mrow class="MJX-TeXAtom-ORD"><mfrac><msubsup><mo>' + numTop + '</mo><mn>' + subScript1 + '</mn><mn>' + superScript1 + '</mn></msubsup><msubsup><mo>' + numBot + '</mo><mn>' + subScript2  + '</mn><mn>' + superScript2 + '</mn></msubsup></mfrac></mrow></mstyle></math>');
											} else {
											    preview.setValue('<math class="simpleFraction" xmlns="http://www.w3.org/1998/Math/MathML"><mstyle displaystyle="true"><mrow class="MJX-TeXAtom-ORD"><mfrac><mn>' + numTop + '</mn><mn>' + numBot + '</mn></mfrac></mrow></mstyle></math>');
											}
											
										} );
									}
					            },
					            setup: function( widget ) {
									// Old code remove \( and )\
									//this.setValue( CKEDITOR.plugins.mathjax.trim( widget.data.math ) );
									this.setValue(numBot);
								},

								commit: function( widget ) {
									// Old code remove \( and )\
									//widget.setData( 'math', '\\(' + this.getValue() + '\\)' );
									numBot = this.getValue();
									if(superScript1 == '' && subScript1 == '' && superScript2 == '' && subScript2 == ''){
										usingScript = false;
									}

									if (usingScript == true) {
									    widget.setData('math', '<math class="simpleFraction" mode="display" xmlns="http://www.w3.org/1998/Math/MathML"><mstyle displaystyle="true"><mrow class="MJX-TeXAtom-ORD"><mfrac><msubsup><mo>' + numTop + '</mo><mn>' + subScript1 + '</mn><mn>' + superScript1 + '</mn></msubsup><msubsup><mo>' + numBot + '</mo><mn>' + subScript2  + '</mn><mn>' + superScript2 + '</mn></msubsup></mfrac></mrow></mstyle></math>');
									} else {
									    widget.setData('math', '<math class="simpleFraction" xmlns="http://www.w3.org/1998/Math/MathML"><mstyle displaystyle="true"><mrow class="MJX-TeXAtom-ORD"><mfrac><mn>' + numTop + '</mn><mn>' + numBot + '</mn></mfrac></mrow></mstyle></math>');
									}
								}
					        },
					        {
								type : 'vbox',
								children :
								[
									{
										type : 'vbox',
										children :
										[
											{
												type : 'text',
												id : 'superScript2',
												onLoad: function( widget ) {

													var that = this;

													if ( !( CKEDITOR.env.ie && CKEDITOR.env.version == 8 ) ) {
														this.getInputElement().on( 'keyup', function() {

															if(usingScript == false){
																usingScript = true;
															}

															superScript2 = that.getInputElement().getValue();

															if(superScript1 == '' && subScript1 == '' && superScript2 == '' && subScript2 == ''){
																usingScript = false;
															}

															if (usingScript == true) {
															    preview.setValue('<math class="simpleFraction" mode="display" xmlns="http://www.w3.org/1998/Math/MathML"><mstyle displaystyle="true"><mrow class="MJX-TeXAtom-ORD"><mfrac><msubsup><mo>' + numTop + '</mo><mn>' + subScript1 + '</mn><mn>' + superScript1 + '</mn></msubsup><msubsup><mo>' + numBot + '</mo><mn>' + subScript2  + '</mn><mn>' + superScript2 + '</mn></msubsup></mfrac></mrow></mstyle></math>');
															} else {
															    preview.setValue('<math class="simpleFraction" xmlns="http://www.w3.org/1998/Math/MathML"><mstyle displaystyle="true"><mrow class="MJX-TeXAtom-ORD"><mfrac><mn>' + numTop + '</mn><mn>' + numBot + '</mn></mfrac></mrow></mstyle></math>');
															}
														} );
													}
									            },

												commit: function( widget ) {

													superScript2 = this.getValue();

													if(superScript1 == '' && subScript1 == '' && superScript2 == '' && subScript2 == ''){
														usingScript = false;
													}

													if (usingScript == true) {
													    widget.setData('math', '<math class="simpleFraction" mode="display" xmlns="http://www.w3.org/1998/Math/MathML"><mstyle displaystyle="true"><mrow class="MJX-TeXAtom-ORD"><mfrac><msubsup><mo>' + numTop + '</mo><mn>' + subScript1 + '</mn><mn>' + superScript1 + '</mn></msubsup><msubsup><mo>' + numBot + '</mo><mn>' + subScript2  + '</mn><mn>' + superScript2 + '</mn></msubsup></mfrac></mrow></mstyle></math>');
													} else {
													    widget.setData('math', '<math class="simpleFraction" xmlns="http://www.w3.org/1998/Math/MathML"><mstyle displaystyle="true"><mrow class="MJX-TeXAtom-ORD"><mfrac><mn>' + numTop + '</mn><mn>' + numBot + '</mn></mfrac></mrow></mstyle></math>');
													}
												}
											},
											{
												type : 'text',
												id : 'subScript2',
												onLoad: function( widget ) {

													var that = this;

													if ( !( CKEDITOR.env.ie && CKEDITOR.env.version == 8 ) ) {
														this.getInputElement().on( 'keyup', function() {

															if(usingScript == false){
																usingScript = true;
															}
															// Old code remove \( and )\
															//preview.setValue( '\\(' + that.getInputElement().getValue() + '\\)' );
															subScript2 = that.getInputElement().getValue();

															if(superScript1 == '' && subScript1 == '' && superScript2 == '' && subScript2 == ''){
																usingScript = false;
															}

															if (usingScript == true) {
															    preview.setValue('<math class="simpleFraction" mode="display" xmlns="http://www.w3.org/1998/Math/MathML"><mstyle displaystyle="true"><mrow class="MJX-TeXAtom-ORD"><mfrac><msubsup><mo>' + numTop + '</mo><mn>' + subScript1 + '</mn><mn>' + superScript1 + '</mn></msubsup><msubsup><mo>' + numBot + '</mo><mn>' + subScript2  + '</mn><mn>' + superScript2 + '</mn></msubsup></mfrac></mrow></mstyle></math>');
															} else {
															    preview.setValue('<math class="simpleFraction" xmlns="http://www.w3.org/1998/Math/MathML"><mstyle displaystyle="true"><mrow class="MJX-TeXAtom-ORD"><mfrac><mn>' + numTop + '</mn><mn>' + numBot + '</mn></mfrac></mrow></mstyle></math>');
															}
														} );
													}
									            },

												commit: function( widget ) {
													subScript2 = this.getValue();
													if(superScript1 == '' && subScript1 == '' && superScript2 == '' && subScript2 == ''){
														usingScript = false;
													}

													if (usingScript == true) {
													    widget.setData('math', '<math class="simpleFraction" mode="display" xmlns="http://www.w3.org/1998/Math/MathML"><mstyle displaystyle="true"><mrow class="MJX-TeXAtom-ORD"><mfrac><msubsup><mo>' + numTop + '</mo><mn>' + subScript1 + '</mn><mn>' + superScript1 + '</mn></msubsup><msubsup><mo>' + numBot + '</mo><mn>' + subScript2  + '</mn><mn>' + superScript2 + '</mn></msubsup></mfrac></mrow></mstyle></math>');
													} else {
													    widget.setData('math', '<math class="simpleFraction" xmlns="http://www.w3.org/1998/Math/MathML"><mstyle displaystyle="true"><mrow class="MJX-TeXAtom-ORD"><mfrac><mn>' + numTop + '</mn><mn>' + numBot + '</mn></mfrac></mrow></mstyle></math>');
													}
												}
											}
										]
									}
								]
							}
					    ]
					},
                    {
                        id: 'texttospeech',
                        type: 'textarea',
                        label: editor.lang.mathjax.texttospeech,
                        style: 'min-height: 100px; height: 100px;',
                        inputStyle: 'min-height: 80px; height: 80px;margin-top: 10px;',
                        onLoad: function (widget) {
                            var id = this.domId;
                            $('#' + id).parent('td').parent('tr').css('clear', 'both');
                        },
                        setup: function (widget) {
                            this.setValue(widget.parts.span.$.getAttribute("texttospeech"));
                        },
                        commit: function (widget) {
                            
                            var texttoSpeech = convertTexttoHTML(this.getValue());
                            if (widget.parts.span.$.className.indexOf('math-tex') > -1) {
                                widget.parts.span.$.setAttribute("texttospeech", texttoSpeech)
                            }
                        }
                    },
					{
						id: 'documentation',
						type: 'html',
						html: ''
					},
					( !( CKEDITOR.env.ie && CKEDITOR.env.version == 8 ) ) && {
						id: 'preview',
						type: 'html',
						html:
							'<div style="width:100%;text-align:center;">' +
								'<iframe style="border:0;width:0;height:0;font-size:20px" scrolling="no" frameborder="0" allowTransparency="true" src="' + CKEDITOR.plugins.mathjax.fixSrc + '"></iframe>' +
							'</div>',

						onLoad: function( widget ) {
							var iFrame = CKEDITOR.document.getById( this.domId ).getChild( 0 );
							preview = new CKEDITOR.plugins.mathfraction.frameWrapper( iFrame, editor );
						},

						setup: function( widget ) {
								
							//var tempNumTop = parseInt(widget.data.math.substring(widget.data.math.indexOf('<mn>') + 4, widget.data.math.indexOf('</mn>')));

							var mathObj = $(widget.data.math).find("msubsup");

							if (mathObj.length != 0) {
								//tempNumTop = 
							    var arrayX = $(widget.data.math).find('mo');
							    var arraySub = $(widget.data.math).find('mn');
								//console.log(arrayX);
								numTop = arrayX[0].textContent;
								superScript1 = arraySub[1].textContent;
								subScript1 = arraySub[0].textContent;
								numBot = arrayX[1].textContent;
								superScript2 = arraySub[3].textContent;
								subScript2 = arraySub[2].textContent;

							}else{
								numTop = widget.data.math.substring(widget.data.math.indexOf('<mn>') + 4, widget.data.math.indexOf('</mn>'));
								numBot = widget.data.math.substring(widget.data.math.lastIndexOf('<mn>') + 4, widget.data.math.lastIndexOf('</mn>'));
                                superScript1 = subScript1 = superScript2 = subScript2 = "";
							}
							
							numTop == '' ? numTop : "1";
							numBot == '' ? numBot : "x";
							
							//set value for input
							var dialog = CKEDITOR.dialog.getCurrent();
								dialog.setValueOf('info','topFraction', numTop);
								dialog.setValueOf('info','bottomFraction', numBot);
								dialog.setValueOf('info','superScript1', superScript1);
								dialog.setValueOf('info','subScript1', subScript1);
								dialog.setValueOf('info','superScript2', superScript2);
								dialog.setValueOf('info','subScript2', subScript2);

							preview.setValue( widget.data.math );
						}
					}
				]
			}
		]
	};
} );
