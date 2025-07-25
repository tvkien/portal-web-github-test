/**
 * Tweet Sized JavaScript Templating Engine
 * @param  {[type]} s [description]
 * @param  {[type]} d [description]
 * @return {[type]}   [description]
 */
function t(s, d) {
    for (var p in d) {
        s = s.replace(new RegExp('{' + p + '}', 'g'), d[p]);
    }
    return s;
}

/**
 * Check object is null or not null
 * @param  {[type]}  obj [description]
 * @return {Boolean}     [description]
 */
function isEmpty(obj) {
    for (var i in obj) { return false; }
    return true;
}

/**
 * Get ramdon integer
 * @param  {[type]} min [description]
 * @param  {[type]} max [description]
 * @return {[type]}     [description]
 */
function getRandomInt(min, max) {
    return Math.floor(Math.random() * (max - min + 1)) + min;
}

/**
 * Return max number in array
 * @param  {[type]} myArray [description]
 * @return {[type]}         [description]
 */
function getMaxNumber(myArray) {
    var maxNumber = myArray.length + 1;
    for (var i = 0; i < myArray.length; i++) {
        var  num = parseInt(myArray[i].match(/\d+$/)[0]);
        if (num >= maxNumber) maxNumber = num + 1;
    }
    return maxNumber;
}

/**
 * Convert from string true|false to boolean
 * @param  {[type]} str [description]
 * @return {[type]}     [description]
 */
function convertTrueFalse(str) {
    if (!!str && str.toString().toLowerCase() == "true") {
        return true;
    } else {
        return false;
    }
}

/**
 * Replace all text
 * @param  {[type]} node [description]
 * @return {[type]}      [description]
 */
function replaceText(node) {
    var current = node.nodeValue;
    var replaced = current.replace(searchpattern, replacepattern);
    node.nodeValue = replaced;
}

function traverse(node) {
    var children = node.childNodes;
    var childLen = children.length;
    for (var i = 0; i < childLen; i++) {
        var child = children.item(i);
        if (child.nodeType == 3)//or if(child instanceof Text)
            replaceText(child);
        else
            traverse(child);
    }
}

function replaceAll() {
    traverse(document.body);
}

/**
 * Common Popup
 * @param  {[type]} type Type of popup is [alert] or [confirm]
 * @param  {[type]} msg  Message content of popup
 * @param  {[type]} w    Width of popup
 * @param  {[type]} h    Height of popup
 * @return {[type]}      [description]
 */
function popupAlertMessage(type, msg, w, h) {
    var now = new Date().getTime();
    var msgHtml = '';
    var maxIndex;
    var zIndexArr = [];
    var $ckeDialog = $('.cke_dialog');

    w = w !== undefined ? w : 400;
    h = h !== undefined ? h : 100;

    type = type !== undefined ? type : 'alert';

    type = type.toLowerCase();

    msgHtml += '<div class="popup-alert">';
    msgHtml += '<div class="popup-alert-border">';
    msgHtml += '<div class="popup-alert-content">';
    msgHtml += '<table class="popup-alert-table">';
    msgHtml += '<tr>';
    msgHtml += '<td><p>' + msg + '</p></td>';
    msgHtml += '</tr>';
    msgHtml += '<tr>';
    msgHtml += '<td>';
    msgHtml += '<div class="popup-alert-controls">';

    if (type === 'alert') {
        msgHtml += '<button id="btn-cancel-' + now + '">OK</button>';
    } else if (type === 'confirm') {
        msgHtml += '<button id="btn-yes-' + now + '" onClick="YesConfirmPopup(this);">Yes</button>';
        msgHtml += '<button id="btn-cancel-' + now + '">No</button>';
    }

    msgHtml += '</div>';

    msgHtml += '</td>';
    msgHtml += '</tr>';
    msgHtml += '</table>';
    msgHtml += '</div>';
    msgHtml += '</div>';
    msgHtml += '</div>';

    $('<div/>')
        .html(msgHtml)
        .attr({
            'id': 'popup-alert-' + now,
            'class': 'dialog'
        })
        .appendTo('body')
        .dialog({
            open: function () {
                $(this).parent().find('.ui-dialog-titlebar-close').remove();
            },
            modal: true,
            width: w,
            maxheight: h,
            resizable: false
        });

    // Check when ckeditor dialog exist
    if ($ckeDialog.length) {
        $ckeDialog.each(function(ind, dialog) {
            var $dialog = $(dialog);
            var zIndex = $dialog.css('z-index');

            zIndexArr.push(zIndex);
        });

        maxIndex = Math.max.apply(Math, zIndexArr);
    }

    // Ui dialog appear
    var $uiDialog = $('.ui-dialog');
    $uiDialog.css('height', 'auto');

    if (maxIndex) {
        $uiDialog.last().css('z-index', maxIndex + 2);
        $('.ui-widget-overlay').last().css('z-index', maxIndex + 1);
    }

    $(document).on('click', '#btn-cancel-' + now, function() {
        $(document).find('#popup-alert-' + now).dialog('destroy').remove();
    });
}

function popupAlertMessageV2(type, msg, w, h, functionName, valueInfor) {
    var now = new Date().getTime();
    var msgHtml = '';
    var maxIndex;
    var zIndexArr = [];
    var $ckeDialog = $('.cke_dialog');

    w = w !== undefined ? w : 400;
    h = h !== undefined ? h : 100;

    type = type !== undefined ? type : 'alert';

    type = type.toLowerCase();

    msgHtml += '<div class="popup-alert">';
    msgHtml += '<div class="popup-alert-border">';
    msgHtml += '<div class="popup-alert-content">';
    msgHtml += '<table class="popup-alert-table">';
    msgHtml += '<tr>';
    msgHtml += '<td><p>' + msg + '</p></td>';
    msgHtml += '</tr>';
    msgHtml += '<tr>';
    msgHtml += '<td>';
    msgHtml += '<div class="popup-alert-controls">';

    if (type === 'alert') {
        msgHtml += '<button id="btn-cancel-' + now + '">OK</button>';
    } else if (type === 'confirm') {
        msgHtml += '<button id="btn-yes-' + now + '" valueInfor="' + valueInfor + '" onClick="' + functionName + '(this);">Yes</button>';
        msgHtml += '<button id="btn-cancel-' + now + '">No</button>';
    }

    msgHtml += '</div>';

    msgHtml += '</td>';
    msgHtml += '</tr>';
    msgHtml += '</table>';
    msgHtml += '</div>';
    msgHtml += '</div>';
    msgHtml += '</div>';

    $('<div/>')
        .html(msgHtml)
        .attr({
            'id': 'popup-alert-' + now,
            'class': 'dialog'
        })
        .appendTo('body')
        .dialog({
            open: function () {
                $(this).parent('.ui-dialog').addClass('popup-alert-custom');
                $(this).parent().find('.ui-dialog-titlebar-close').remove();
            },
            modal: true,
            width: w,
            maxheight: h,
            resizable: false
        });

    // Check when ckeditor dialog exist
    if ($ckeDialog.length) {
        $ckeDialog.each(function (ind, dialog) {
            var $dialog = $(dialog);
            var zIndex = $dialog.css('z-index');

            zIndexArr.push(zIndex);
        });

        maxIndex = Math.max.apply(Math, zIndexArr);
    }

    // Ui dialog appear
    var $uiDialog = $('.ui-dialog');
    $uiDialog.css('height', 'auto');

    if (maxIndex) {
        $uiDialog.css('z-index', maxIndex + 2);
        $('.ui-widget-overlay').css('z-index', maxIndex + 1);
    }

    $(document).on('click', '#btn-cancel-' + now, function () {
        $(document).find('#popup-alert-' + now).dialog('destroy').remove();
    });
}

//All User Custom function
//If
function popupAlertMessageV1(type, msg, w, h, functionName, valueInfor, vlabel) {
    var now = new Date().getTime();
    var msgHtml = '';
    var maxIndex;
    var zIndexArr = [];
    var $ckeDialog = $('.cke_dialog');

    w = w !== undefined ? w : 400;
    h = h !== undefined ? h : 100;

    type = type !== undefined ? type : 'alert';

    type = type.toLowerCase();

    msgHtml += '<div class="popup-alert">';
    msgHtml += '<div class="popup-alert-border">';
    msgHtml += '<div class="popup-alert-content">';
    msgHtml += '<table class="popup-alert-table">';
    msgHtml += '<tr>';
    msgHtml += '<td><p>' + msg + '</p></td>';
    msgHtml += '</tr>';
    msgHtml += '<tr>';
    msgHtml += '<td>';
    msgHtml += '<div class="popup-alert-controls">';

    if (type === 'alert') {
        msgHtml += '<button onClick="' + functionName + '(' + valueInfor + ');" >' + vlabel + '</button>';
    } else if (type === 'confirm') {
        msgHtml += '<button id="btn-yes-' + now + '" onClick="' + functionName + '(' + valueInfor + ');">Yes</button>';
        msgHtml += '<button id="btn-cancel-' + now + '">No</button>';
    }

    msgHtml += '</div>';

    msgHtml += '</td>';
    msgHtml += '</tr>';
    msgHtml += '</table>';
    msgHtml += '</div>';
    msgHtml += '</div>';
    msgHtml += '</div>';

    $('<div/>')
        .html(msgHtml)
        .attr({
            'id': 'popup-alert-' + now,
            'class': 'dialog'
        })
        .appendTo('body')
        .dialog({
            open: function () {
                $(this).parent().find('.ui-dialog-titlebar-close').remove();
            },
            modal: true,
            width: w,
            maxheight: h,
            resizable: false
        });

    // Check when ckeditor dialog exist
    if ($ckeDialog.length) {
        $ckeDialog.each(function (ind, dialog) {
            var $dialog = $(dialog);
            var zIndex = $dialog.css('z-index');

            zIndexArr.push(zIndex);
        });

        maxIndex = Math.max.apply(Math, zIndexArr);
    }

    // Ui dialog appear
    var $uiDialog = $('.ui-dialog');
    $uiDialog.css('height', 'auto');

    if (maxIndex) {
        $uiDialog.css('z-index', maxIndex + 2);
        $('.ui-widget-overlay').css('z-index', maxIndex + 1);
    }

    $(document).on('click', '#btn-cancel-' + now, function () {
        $(document).find('#popup-alert-' + now).dialog('destroy').remove();
    });
}

/**
 * Popup Custom For Move Tab Navigation SGO
 * @param  {[type]} type         [description]
 * @param  {[type]} msg          [description]
 * @param  {[type]} w            [description]
 * @param  {[type]} h            [description]
 * @param  {[type]} functionName [description]
 * @param  {[type]} valueInfor   [description]
 * @param  {[type]} vlabel       [description]
 * @return {[type]}              [description]
 */
function NavigationTabSGO(msg, w, h, funca, funcb) {
    var now = new Date().getTime();
    var msgHtml = '';
    var maxIndex;
    var zIndexArr = [];
    var $ckeDialog = $('.cke_dialog');

    w = w !== undefined ? w : 400;
    h = h !== undefined ? h : 100;

    msgHtml += '<div class="popup-alert">';
    msgHtml += '<div class="popup-alert-border">';
    msgHtml += '<div class="popup-alert-content">';
    msgHtml += '<table class="popup-alert-table">';
    msgHtml += '<tr>';
    msgHtml += '<td>' + msg + '</td>';
    msgHtml += '</tr>';
    msgHtml += '<tr>';
    msgHtml += '<td>';
    msgHtml += '<div class="popup-alert-controls">';
    msgHtml += '<button id="btn-yes-' + now + '" onClick="' + funca + '();">Yes</button>';
    msgHtml += '<button id="btn-no-' + now + '" onClick="' + funcb + '();">No</button>';
    msgHtml += '<button id="btn-cancel-' + now + '">Cancel</button>';
    msgHtml += '</div>';
    msgHtml += '</td>';
    msgHtml += '</tr>';
    msgHtml += '</table>';
    msgHtml += '</div>';
    msgHtml += '</div>';
    msgHtml += '</div>';

    $('<div/>')
        .html(msgHtml)
        .attr({
            'id': 'popup-alert-' + now,
            'class': 'dialog'
        })
        .appendTo('body')
        .dialog({
            open: function () {
                $(this).parent().find('.ui-dialog-titlebar-close').remove();
            },
            modal: true,
            width: w,
            maxheight: h,
            resizable: false
        });

    // Check when ckeditor dialog exist
    if ($ckeDialog.length) {
        $ckeDialog.each(function (ind, dialog) {
            var $dialog = $(dialog);
            var zIndex = $dialog.css('z-index');

            zIndexArr.push(zIndex);
        });

        maxIndex = Math.max.apply(Math, zIndexArr);
    }

    // Ui dialog appear
    var $uiDialog = $('.ui-dialog');
    $uiDialog.css('height', 'auto');

    if (maxIndex) {
        $uiDialog.css('z-index', maxIndex + 2);
        $('.ui-widget-overlay').css('z-index', maxIndex + 1);
    }

    $(document).on('click', '#btn-cancel-' + now, function () {
        $(document).find('#popup-alert-' + now).dialog('destroy').remove();
    });
}

/**
 * Function correct old inline choice item type
 * @param  {[type]} xmlContent  This is old xmlContent
 * @return {[type]}      [xmlContent]
 */
function correctInlineChoice(xmlContent) {
    var newXmlContent = '';
    var $xmlContent = $(xmlContent);

    var tempXML = $("<div>" + xmlContent + "</div>");

    if (tempXML.find(".boxedText").length > 0 && tempXML.find(".boxedText").find(".inlineChoiceAnswer")) {
        //Replace all <p> to <span>
        xmlContent = xmlContent.replace(/<p class="boxedText"/g, '<span class="boxedText"');

    }
    xmlContent = xmlContent.replace(/<p>/g, '<span>').replace(/<p /g, '<span ').replace(/<\/p>/g, '</span>');

    $xmlContent.find('.inlineChoiceAnswer').each(function(index, icAnswer) {
        var $icAnswer = $(icAnswer);
        var $newIcAnswer = $('<span/>');

        $newIcAnswer.html($icAnswer.html());
        copyAttributes($icAnswer, $newIcAnswer);

        return $newIcAnswer;
    });

    newXmlContent = $xmlContent.prop('outerHTML');

    return newXmlContent;
}

function removeObjectTags(xmlContent) {
    var $xmlContent = $('<div>' + xmlContent + '</div>');
    $xmlContent.find('object').remove();
    return $xmlContent.html();
}

/**
 * Copy Attributes From Element To Other Element
 * @param  {[type]} from [description]
 * @param  {[type]} to   [description]
 * @return {[type]}      [description]
 */
function copyAttributes(from, to) {
    var attrs = from.prop('attributes');
    $.each(attrs, function (index, attribute) {
        to.attr(attribute.name, attribute.value);
    });
}


function popupAlertMessageV3(type, msg, w, h, functionName, valueInfor, vlabelOke, vLabelCancel) {
    var now = new Date().getTime();
    var msgHtml = '';
    var maxIndex;
    var zIndexArr = [];
    var $ckeDialog = $('.cke_dialog');

    w = w !== undefined ? w : 400;
    h = h !== undefined ? h : 100;

    type = type !== undefined ? type : 'alert';

    type = type.toLowerCase();

    msgHtml += '<div class="popup-alert">';
    msgHtml += '<div class="popup-alert-border">';
    msgHtml += '<div class="popup-alert-content">';
    msgHtml += '<table class="popup-alert-table">';
    msgHtml += '<tr>';
    msgHtml += '<td><p>' + msg + '</p></td>';
    msgHtml += '</tr>';
    msgHtml += '<tr>';
    msgHtml += '<td>';
    msgHtml += '<div class="popup-alert-controls">';

    if (type === 'alert') {
        msgHtml += '<button onClick="' + functionName + '(' + valueInfor + ');" >' + vlabelOke + '</button>';
    } else if (type === 'confirm') {
        msgHtml += '<button id="btn-yes-' + now + '" onClick="' + functionName + '(' + valueInfor + ');">' + vlabelOke + '</button>';
        msgHtml += '<button id="btn-cancel-' + now + '">' + vLabelCancel + '</button>';
    }

    msgHtml += '</div>';

    msgHtml += '</td>';
    msgHtml += '</tr>';
    msgHtml += '</table>';
    msgHtml += '</div>';
    msgHtml += '</div>';
    msgHtml += '</div>';

    $('<div/>')
        .html(msgHtml)
        .attr({
            'id': 'popup-alert-' + now,
            'class': 'dialog'
        })
        .appendTo('body')
        .dialog({
            open: function () {
                $(this).parent().find('.ui-dialog-titlebar-close').remove();
            },
            modal: true,
            width: w,
            maxheight: h,
            resizable: false
        });

    // Check when ckeditor dialog exist
    if ($ckeDialog.length) {
        $ckeDialog.each(function (ind, dialog) {
            var $dialog = $(dialog);
            var zIndex = $dialog.css('z-index');

            zIndexArr.push(zIndex);
        });

        maxIndex = Math.max.apply(Math, zIndexArr);
    }

    // Ui dialog appear
    var $uiDialog = $('.ui-dialog');
    $uiDialog.css('height', 'auto');

    if (maxIndex) {
        $uiDialog.css('z-index', maxIndex + 2);
        $('.ui-widget-overlay').css('z-index', maxIndex + 1);
    }

    $(document).on('click', '#btn-cancel-' + now, function () {
        $(document).find('#popup-alert-' + now).dialog('destroy').remove();
    });
}

/**
 * Function to play audio
 * @param  {[source]} audio source.
 */
function vnsAudio(source) {
    var iniConfig = source;
    //var src = iniConfig.src;
    var id = 'vnsAudio';

    this.init = function () {
        var me = this;

        if (!iniConfig.src) {
            alert('URL for Audio should be defined');
            return;
        }

        if (!$('#' + id).length) {
            // Adding DOM
            var player = $('<audio/>', {
                id: id,
                src: iniConfig.src
            }).appendTo('body');


        } else {
            $('#' + id).attr('src', iniConfig.src);
        }

        // Apply Player
        this.audio = new MediaElement(id, {
            success: function (me) {
                me.play();
                var emptyFn = function () { };
                // Add Listeners
                me.addEventListener('play', (iniConfig.onPlay || emptyFn), false);
                me.addEventListener('pause', (iniConfig.onPause || emptyFn), false);
                me.addEventListener('ended', (iniConfig.onEnded || emptyFn), false);
            }
        });
    };

    this.play = function () {
        if (this.audio == undefined) this.init();
        this.audio.setCurrentTime(0);
        this.audio.play();
    };
    this.pause = function () {
        if (this.audio == undefined) return;

        this.audio.pause();
    };
    this.isPaused = function () {
        return this.audio.paused;
    };
    this.showMess = function (msg) {
        var dom = document.getElementById('status');
        dom.innerHTML = dom.innerHTML + msg + "<br/>";
    };

    this.init();
};

function convertTexttoHTML (safe) {
    if (!!safe) {
        return safe
            .replace(/&/g, '&amp;')
            .replace(/>/g, '&gt;')
            .replace(/</g, '&lt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#39;')
            .replace(/`/g, '&#96;');
    }

    return '';
}

function escapeHtml(text) {
    return !!text ? text
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;") : '';
}

function unescapeHtml(html) {
    return !!html ? html
        .replace(/&amp;/g, "&")
        .replace(/&lt;/g, "<")
        .replace(/&gt;/g, ">")
        .replace(/&quot;/g, '"')
        .replace(/&#039;/g, "'") : '';
}
