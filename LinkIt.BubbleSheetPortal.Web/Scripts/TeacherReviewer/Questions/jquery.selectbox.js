/*!
 * jQuery Selectbox plugin
 *
 * Copyright 2011-2012, Dimitar Ivanov (http://www.bulgaria-web-developers.com/projects/javascript/selectbox/)
 * Licensed under the MIT (http://www.opensource.org/licenses/mit-license.php) license.
 * 
 */
(function ($, undefined) {
	var PROP_NAME = 'selectbox',
		FALSE = false,
		TRUE = true;
	/**
	 * Selectbox manager.
	 * Use the singleton instance of this class, $.selectbox, to interact with the select box.
	 * Settings for (groups of) select boxes are maintained in an instance object,
	 * allowing multiple different settings on the same page
	 */
	function Selectbox() {
		this._state = [];
		this._defaults = { // Global defaults for all the select box instances
			classHolder: "sbHolder",
			classHolderDisabled: "sbHolderDisabled",
			classSelector: "sbSelector",
			classOptions: "sbOptions",
			classGroup: "sbGroup",
			classSub: "sbSub",
			classDisabled: "sbDisabled",
			classToggleOpen: "sbToggleOpen",
			classToggle: "sbToggle",
			classFocus: "sbFocus",
			speed: 100,
			effect: "slide", // "slide" or "fade"
			is_Disabled: false,
			onChange: null, //Define a callback function when the selectbox is changed
			onOpen: null, //Define a callback function when the selectbox is open
			onClose: null //Define a callback function when the selectbox is closed
		};
	}
	
	$.extend(Selectbox.prototype, {
		/**
		 * Is the first field in a jQuery collection open as a selectbox
		 * 
		 * @param {Object} target
		 * @return {Boolean}
		 */
		_isOpenSelectbox: function (target) {
			if (!target) {
				return FALSE;
			}
			var inst = this._getInst(target);
			return inst.isOpen;
		},
		/**
		 * Is the first field in a jQuery collection disabled as a selectbox
		 * 
		 * @param {HTMLElement} target
		 * @return {Boolean}
		 */
		_isDisabledSelectbox: function (target) {
			if (!target) {
				return FALSE;
			}
			var inst = this._getInst(target);
			return inst.isDisabled;
		},
		/**
		 * Attach the select box to a jQuery selection.
		 * 
		 * @param {HTMLElement} target
		 * @param {Object} settings
		 */
		_attachSelectbox: function (target, settings) {
			
			if (this._getInst(target)) {
				return FALSE;
			}
			var $target = $(target),
				self = this,
				inst = self._newInst($target),
				sbHolder, sbSelector, sbToggle, sbOptions,
				s = FALSE, optGroup = $target.find("optgroup"), opts = $target.find("li"), olen = opts.length;
				
			$target.attr("sb", inst.uid);
				
			$.extend(inst.settings, self._defaults, settings);
			self._state[inst.uid] = FALSE;
			$target.hide();
			
			function closeOthers() {
				var key, sel,
					uid = this.attr("id").split("_")[1];
				for (key in self._state) {
					if (key !== uid) {
						if (self._state.hasOwnProperty(key)) {
							sel = $("select[sb='" + key + "']")[0];
							if (sel) {
								self._closeSelectbox(sel);
							}
						}
					}
				}
			}
			
			sbHolder = $("<div>", {
				"id": "sbHolder_" + inst.uid,
				"class": inst.settings.classHolder,
				"tabindex": $target.attr("tabindex")
			});
			
			sbSelector = $("<a>", {
				"id": "sbSelector_" + inst.uid,
				"href": "#",
				"class": inst.settings.classSelector,
				"click": function (e) {
					e.preventDefault();
					closeOthers.apply($(this), []);
					var uid = $(this).attr("id").split("_")[1];
					if (self._state[uid]) {
						self._closeSelectbox(target);
					} else {
						self._openSelectbox(target);
					}
				}
			}).attr('responseidentifier', $(inst.input).attr('responseidentifier'));
			
			sbToggle = $("<a>", {
				"id": "sbToggle_" + inst.uid,
				"href": "#",
				"class": inst.settings.classToggle,
				"click": function (e) {
					e.preventDefault();
					closeOthers.apply($(this), []);
					var uid = $(this).attr("id").split("_")[1];
					if (self._state[uid]) {
						self._closeSelectbox(target);
					} else {
						self._openSelectbox(target);
					}
				}
			});
			sbToggle.appendTo(sbHolder);

			sbOptions = $("<ul>", {
				"id": "sbOptions_" + inst.uid,
				"class": inst.settings.classOptions,
				"css": {
					"display": "none"
				}
			});
			
			$target.children().each(function(i) {
				var that = $(this), li, config = {};
				if (that.is("li")) {
					getOptions(that);
				} else if (that.is("optgroup")) {
					li = $("<li>");
					$("<span>", {
						"text": that.attr("label")
					}).addClass(inst.settings.classGroup).appendTo(li);
					li.appendTo(sbOptions);
					if (that.is(":disabled")) {
						config.disabled = true;
					}
					config.sub = true;
					getOptions(that.find("li"), config);
				}
			});
			
			function getOptions () {
				var sub = arguments[1] && arguments[1].sub ? true : false,
					disabled = arguments[1] && arguments[1].disabled ? true : false;
				arguments[0].each(function (i) {
					var that = $(this),
						li = $("<li>"),
						child;
					if (that.is(":selected")) {
						sbSelector.html(that.html());
						s = TRUE;
					}
					if (i === olen - 1) {
						li.addClass("last");
					}
					if (!that.is(":disabled") && !disabled) {
						child = $("<div>", {
							"rel": 'inlineChoice',
							"identifier": that.attr("identifier")
						}).html(that.html()).bind("click.sb", function (e) {
							if (e && e.preventDefault) {
								e.preventDefault();
							}
							var t = sbToggle,
							 	$this = $(this),
								uid = t.attr("id").split("_")[1];
							self._changeSelectbox(target, that.attr("identifier"), $this.html(), $this);
							self._closeSelectbox(target);
						}).bind("mouseover.sb", function () {
							var $this = $(this);
							$this.parent().siblings().find("a").removeClass(inst.settings.classFocus);
							$this.addClass(inst.settings.classFocus);
						}).bind("mouseout.sb", function () {
							$(this).removeClass(inst.settings.classFocus);
						});
						if (sub) {
							child.addClass(inst.settings.classSub);
						}
						if (that.is(":selected")) {
							child.addClass(inst.settings.classFocus);
						}
						child.appendTo(li);
					} else {
						child = $("<span>", {
							"text": that.text()
						}).addClass(inst.settings.classDisabled);
						if (sub) {
							child.addClass(inst.settings.classSub);
						}
						child.appendTo(li);
					}
					li.appendTo(sbOptions);
				});
			}
			
			if (!s) {
				sbSelector.html(opts.first().html());
			}

			$.data(target, PROP_NAME, inst);
			
			sbHolder.bind("keydown.sb", function (e) {
				var key = e.charCode ? e.charCode : e.keyCode ? e.keyCode : 0;
				var tagul = $(this).find('.sbOptions');
				var id = $(this).attr('id');
				var uid = id.split('_');
				var $f = tagul.find("li.sbFocus");
				var trgt = $(this).siblings(["ul[sb='", uid[1], "']"].join("")).get(0);

				switch (key) {
					case 37: //Arrow Left
					case 38: //Arrow Up
					
						if($(this).find('.sbOptions li').hasClass('sbDisable')){
							return false;
						}
						
						if ($f.length > 0) {
							var $next, lenLi, identifier;
							$(this).find('.sbOptions li').removeClass('sbFocus');
							$(this).find('.sbOptions li').removeClass('selected');
							
							$next = $f.prev("li");
							identifier = $next.find('div[rel]').attr('identifier');
							
							if ($next.length > 0 && identifier != '') {
								$next.addClass('sbFocus').focus();
								$("#sbSelector_" + uid[1]).html($next.find('div[rel]').html());
								$("#sbSelector_" + uid[1]).attr('identifier', identifier);
							}else {
								lenLi = $(this).find('.sbOptions li').length;	
								$(this).find('.sbOptions li').eq(lenLi-1).addClass('sbFocus');
								identifier = $(this).find('.sbOptions li').eq(lenLi-1).find('div[rel]').attr('identifier');
								$("#sbSelector_" + uid[1]).html($(this).find('.sbOptions li').eq(lenLi-1).find('div[rel]').html());
								$("#sbSelector_" + uid[1]).attr('identifier', identifier);
							}
						}
						break;
					case 39: //Arrow Right
					case 40: //Arrow Down
					
						if($(this).find('.sbOptions li').hasClass('sbDisable')){
							return false;
						}
						
						var $next, identifier;
						$(this).find('.sbOptions li').removeClass('sbFocus');
						$(this).find('.sbOptions li').removeClass('selected');
						
						if ($f.length > 0) {
							$next = $f.next("li");
						} else {
							$next = $f.find("ul").find("li").eq(1);
						}
						
						if ($next.length > 0) {
							$next.addClass('sbFocus').focus();
							identifier = $next.find('div[rel]').attr('identifier');
							$("#sbSelector_" + uid[1]).html($next.find('div[rel]').html());
							$("#sbSelector_" + uid[1]).attr('identifier', identifier);
						}else {
							$(this).find('.sbOptions li').eq(1).addClass('sbFocus');
							identifier = $(this).find('.sbOptions li').eq(1).find('div[rel]').attr('identifier');
							$("#sbSelector_" + uid[1]).html($(this).find('.sbOptions li').eq(1).find('div[rel]').html());
							$("#sbSelector_" + uid[1]).attr('identifier', identifier);
						}
						break;				
					case 13: //Enter
						if ($f.length > 0) {
							self._changeSelectbox(trgt, $f.find('div[rel]').attr("identifier"), $f.find('div[rel]').html(), $f);
						}
						self._closeSelectbox(trgt);
						break;
					case 9: //Tab
						
						if (trgt) {
							var inst = self._getInst(trgt);
							if (inst/* && inst.isOpen*/) {
								if ($f.length > 0) {
									self._changeSelectbox(trgt, $f.find('div[rel]').attr("identifier"), $f.find('div[rel]').html(), $f);
								}
								self._closeSelectbox(trgt);
							}
						}
						
						break;
					case 27: //Escape
						
						self._closeSelectbox(trgt);
						break;
				}
				e.stopPropagation();
				return false;
			}).delegate("a", "mouseover", function (e) {
				$(this).addClass(inst.settings.classFocus);
			}).delegate("a", "mouseout", function (e) {
				$(this).removeClass(inst.settings.classFocus);	
			});
			
			sbSelector.appendTo(sbHolder);
			sbOptions.appendTo(sbHolder);			
			sbHolder.insertAfter($target);
			
			if(navigator.userAgent.indexOf('Safari') > -1 && navigator.userAgent.indexOf('Chrome') == -1){
				//$('.sbHolder').css('width', '200px');
			}
				
			$("html").live('mousedown', function(e) {
				e.stopPropagation();          
				$(".inlineChoiceFormat").selectbox('close'); 
			});
			$([".", inst.settings.classHolder, ", .", inst.settings.classSelector].join("")).mousedown(function(e) {    
				e.stopPropagation();
			});
		},
		/**
		 * Remove the selectbox functionality completely. This will return the element back to its pre-init state.
		 * 
		 * @param {HTMLElement} target
		 */
		_detachSelectbox: function (target) {
			var inst = this._getInst(target);
			if (!inst) {
				return FALSE;
			}
			$("#sbHolder_" + inst.uid).remove();
			$.data(target, PROP_NAME, null);
			$(target).show();			
		},
		/**
		 * Change selected attribute of the selectbox.
		 * 
		 * @param {HTMLElement} target
		 * @param {String} value
		 * @param {String} text
		 */
		_changeSelectbox: function (target, value, htmlContent, tagli) {
			var onChange,
				inst = this._getInst(target);
			if (inst) {
				onChange = this._get(inst, 'onChange');
				$("#sbSelector_" + inst.uid).attr({
					'responseidentifier': $(target).attr('responseidentifier'),
					'identifier': value
				})
				$("#sbSelector_" + inst.uid).html(htmlContent);
				if(navigator.userAgent.indexOf('Safari') > -1 && navigator.userAgent.indexOf('Chrome') == -1){
					$('#sbHolder_' + inst.uid).css('width', 'auto');
				}
			}
			//value = value.replace(/\'/g, "\\'");
			
			var tagUl = $(tagli).parents('.sbOptions');
			$(tagUl).find("li").removeClass('selected');
			$(tagUl).find("li").removeClass('sbFocus');
			$(tagUl).find("div[identifier='" + value + "']").parent('li').addClass('selected');
			$(tagUl).find("div[identifier='" + value + "']").parent('li').addClass('sbFocus');
			
			if (inst && onChange) {
				onChange.apply((inst.input ? inst.input[0] : null), [value, inst]);
			} else if (inst && inst.input) {
				inst.input.trigger('change');
			}
		},
		/**
		 * Enable the selectbox.
		 * 
		 * @param {HTMLElement} target
		 */
		_enableSelectbox: function (target) {
			var inst = this._getInst(target);
			if (!inst || !inst.isDisabled) {
				return FALSE;
			}
			$("#sbHolder_" + inst.uid).removeClass(inst.settings.classHolderDisabled);
			inst.isDisabled = FALSE;
			$.data(target, PROP_NAME, inst);
		},
		/**
		 * Disable the selectbox.
		 * 
		 * @param {HTMLElement} target
		 */
		_disableSelectbox: function (target) {
			var inst = this._getInst(target);
			if (!inst || inst.isDisabled) {
				return FALSE;
			}
			$("#sbHolder_" + inst.uid).addClass(inst.settings.classHolderDisabled);
			inst.isDisabled = TRUE;
			$.data(target, PROP_NAME, inst);
		},
		/**
		 * Get or set any selectbox option. If no value is specified, will act as a getter.
		 * 
		 * @param {HTMLElement} target
		 * @param {String} name
		 * @param {Object} value
		 */
		_optionSelectbox: function (target, name, value) {
			var inst = this._getInst(target);
			if (!inst) {
				return FALSE;
			}
			//TODO check name
			inst[name] = value;
			$.data(target, PROP_NAME, inst);
		},
		/**
		 * Call up attached selectbox
		 * 
		 * @param {HTMLElement} target
		 */
		_openSelectbox: function (target) {

			var inst = this._getInst(target);
			var wTarget = $(target).width();
		    var answered = '';
			//if (!inst || this._state[inst.uid] || inst.isDisabled) {
			if (!inst || inst.isOpen || inst.isDisabled) {
				return;
			}

			var	el = $("#sbOptions_" + inst.uid),
				viewportHeight = parseInt($(window).height(), 10),
				offset = $("#sbHolder_" + inst.uid).offset(),
				scrollTop = $(window).scrollTop(),
				height = el.prev().height(),
				diff = viewportHeight - (offset.top - scrollTop) - height / 2,
				onOpen = this._get(inst, 'onOpen');
		
			el.css({
				"top": height + "px",
				"maxHeight": (diff - height) + "px",
				"width": (wTarget - 26) + "px"
			});
		    
            if (el.width() < 200) {
                el.css({ "width": "100%" });
            }
		    
            if (el.parents('.smallText').length) {
                if (el.width() < 200) {
                    el.css({ "width": "170px" });
                }
            }
		    
            if (el.parent().find('.sbSelector').html() != '') {
                var widthParent = el.parent().width();
                var idres = el.parent().find('.sbSelector').attr('responseidentifier');
                var tagMainBody = el.parents('.mainBody');
                var tagUl = tagMainBody.find('ul.inlineChoiceFormat[responseidentifier=' + idres + ']');
                var wTagUl = tagUl.width();

                if (widthParent < wTagUl) {
                    widthParent = wTagUl;
                }
                if (wTagUl > tagMainBody.width()) {
                    widthParent = tagMainBody.width();
                }
                el.css({ "width": widthParent });
            } else {
                var widthParent = el.parent().width();
                el.css({ "width": widthParent });
            }

            $('.sbToggle').removeClass('sbToggleOpen');
            $('.sbOptions').fadeOut(50);

			inst.settings.effect === "fade" ? el.fadeIn(inst.settings.speed) : el.slideDown(inst.settings.speed);
			$("#sbToggle_" + inst.uid).addClass(inst.settings.classToggleOpen);
		    
			answered = el.parent().find('a.sbSelector[identifier]').attr('identifier');
			el.find('div[identifier="' + answered + '"]').parent('li').addClass('selected');
		    
			this._state[inst.uid] = TRUE;
			inst.isOpen = TRUE;
			if (onOpen) {
				onOpen.apply((inst.input ? inst.input[0] : null), [inst]);
			}
			$.data(target, PROP_NAME, inst);
			
			if(inst.settings.is_Disabled){
				el.find('li').addClass('sbDisable');
			}
		},
		/**
		 * Close opened selectbox
		 * 
		 * @param {HTMLElement} target
		 */
		_closeSelectbox: function (target) {
			var inst = this._getInst(target);
			//if (!inst || !this._state[inst.uid]) {
			if (!inst || !inst.isOpen) {
				return;
			}
			var onClose = this._get(inst, 'onClose');
			inst.settings.effect === "fade" ? $("#sbOptions_" + inst.uid).fadeOut(inst.settings.speed) : $("#sbOptions_" + inst.uid).slideUp(inst.settings.speed);
			$("#sbToggle_" + inst.uid).removeClass(inst.settings.classToggleOpen);
			this._state[inst.uid] = FALSE;
			inst.isOpen = FALSE;
			if (onClose) {
				onClose.apply((inst.input ? inst.input[0] : null), [inst]);
			}
			$.data(target, PROP_NAME, inst);
		},
		/**
		 * Create a new instance object
		 * 
		 * @param {HTMLElement} target
		 * @return {Object}
		 */
		_newInst: function(target) {
			var id = target[0].id.replace(/([^A-Za-z0-9_-])/g, '\\\\$1');
			return {
				id: id, 
				input: target, 
				uid: Math.floor(Math.random() * 99999999),
				isOpen: FALSE,
				isDisabled: FALSE,
				settings: {}
			}; 
		},
		/**
		 * Retrieve the instance data for the target control.
		 * 
		 * @param {HTMLElement} target
		 * @return {Object} - the associated instance data
		 * @throws error if a jQuery problem getting data
		 */
		_getInst: function(target) {
			try {
				return $.data(target, PROP_NAME);
			}
			catch (err) {
				throw 'Missing instance data for this selectbox';
			}
		},
		/**
		 * Get a setting value, defaulting if necessary
		 * 
		 * @param {Object} inst
		 * @param {String} name
		 * @return {Mixed}
		 */
		_get: function(inst, name) {
			return inst.settings[name] !== undefined ? inst.settings[name] : this._defaults[name];
		}
	});

	/**
	 * Invoke the selectbox functionality.
	 * 
	 * @param {Object|String} options
	 * @return {Object}
	 */
	$.fn.selectbox = function (options) {
		
		var otherArgs = Array.prototype.slice.call(arguments, 1);
		if (typeof options == 'string' && options == 'isDisabled') {
			return $.selectbox['_' + options + 'Selectbox'].apply($.selectbox, [this[0]].concat(otherArgs));
		}
		
		if (options == 'li' && arguments.length == 2 && typeof arguments[1] == 'string') {
			return $.selectbox['_' + options + 'Selectbox'].apply($.selectbox, [this[0]].concat(otherArgs));
		}
		
		return this.each(function() {
			typeof options == 'string' ?
				$.selectbox['_' + options + 'Selectbox'].apply($.selectbox, [this].concat(otherArgs)) :
				$.selectbox._attachSelectbox(this, options);
		});
	};
	
	$.selectbox = new Selectbox(); // singleton instance
})(jQuery);