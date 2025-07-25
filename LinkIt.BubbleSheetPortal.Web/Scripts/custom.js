$(function () {
  $.ajaxSetup({
    cache: false
  });

  FirstFocus();

  $(document).ajaxError(function (event, request) {
    if (request.status === 401) {
      var ignoreReload = false;

      if (typeof KeepAliveCalled != 'undefined') {
        if (KeepAliveCalled == true) {
          ignoreReload = true;
        }
      }

      if (!ignoreReload) {
        location.reload(true);
      }
    } else if (request.status === 403) {
      alert("You do not have access to view the requested resource.");
    } else if (request.status === 404) {
      alert("The requested resource could not be found.");
    } else if (request.status === 410) {
      alert("The requested resource has been deleted.");
    } else if (request.status === 500) {
      alert("An unexpected error occurred. Please refresh the page and try again.");
    }
  });
});

function ShowBlock(div, text) {
  div.block({
    css: {
      border: 'none',
      padding: '15px',
      backgroundColor: '#000',
      '-webkit-border-radius': '10px',
      '-moz-border-radius': '10px',
      opacity: 1,
      color: '#fff'
    },
    message: '<h2 style="color:#fff">' + text + ' ...</h2>'
  });
}

function FirstFocus() {
  $(".first-focus").focus();
  $(".first-focus").val($(".first-focus").val());
}

$('.ajax-form').live('submit', function (e) {
  e.preventDefault();
  var thisObj = $(this);
  $(this).ajaxSubmit({
    target: $(this),
    callback: function () {
      thisObj.applyTemplateSetup();
    }
  });
});

if ($.fn.dataTableExt) {
  $.fn.dataTableExt.oStdClasses.sWrapper = 'no-margin last-child';
  $.fn.dataTableExt.oStdClasses.sInfo = 'message no-margin';
  $.fn.dataTableExt.oStdClasses.sLength = 'float-left';
  $.fn.dataTableExt.oStdClasses.sFilter = 'float-right';
  $.fn.dataTableExt.oStdClasses.sPaging = 'sub-hover paging_';
  $.fn.dataTableExt.oStdClasses.sPagePrevEnabled = 'control-prev';
  $.fn.dataTableExt.oStdClasses.sPagePrevDisabled = 'control-prev disabled';
  $.fn.dataTableExt.oStdClasses.sPageNextEnabled = 'control-next';
  $.fn.dataTableExt.oStdClasses.sPageNextDisabled = 'control-next disabled';
  $.fn.dataTableExt.oStdClasses.sPageFirst = 'control-first';
  $.fn.dataTableExt.oStdClasses.sPagePrevious = 'control-prev';
  $.fn.dataTableExt.oStdClasses.sPageNext = 'control-next';
  $.fn.dataTableExt.oStdClasses.sPageLast = 'control-last';
  $.fn.dataTableExt.sErrMode = "throw";

  $('.datatable').each(function (i) {
    initializeDataTable($(this));
  });
}

var ui = {};
var selectGroupPrintingRow = "";

function initializeDataTable(table) {
  var options = table.data('options');

  if (options == null) {
    setTimeout(function () { initializeDataTable(table); }, 100);
    return;
  }

  var defaults = {
    sDom: '<"block-controls"l<\'float-left\'<"#showUser">><f>>rti<"block-footer clearfix"<"controls-buttons"p>>',
    fnDrawCallback: function () {
      this.parent().applyTemplateSetup();
    },
    fnInitComplete: function () {
      this.parent().applyTemplateSetup();
    }
  };

  var oTable = table.dataTable($.extend(defaults, options));

  table.find('thead .sort-up').click(function (event) {
    event.preventDefault();
    var column = $(this).closest('th'),
      //columnIndex = column.parent().children().index(column.get(0));
      columnIndex = $(column.get(0)).attr('aoData-index');
    oTable.fnSort([[columnIndex, 'asc']]);
    return false;
  });
  table.find('thead .sort-down').click(function (event) {
    event.preventDefault();
    var column = $(this).closest('th'),
      //columnIndex = column.parent().children().index(column.get(0));
      columnIndex = $(column.get(0)).attr('aoData-index');
    oTable.fnSort([[columnIndex, 'desc']]);
    return false;
  });

  ui[table.attr('id')] = oTable;
  return oTable;
}

function fillDataTable(table, url) {
  var oTable = table.dataTable();
  oTable.fnSettings().sAjaxSource = url;
  oTable.fnDraw();
}

function fillDataTableWithoutCustomMessage(table, url) {
  var oTable = table.dataTable();
  oTable.fnSettings().sAjaxSource = url;
  oTable.fnDraw();
}

//This is  make sure dataTableExt is not undefined TuanUI fix 2015-07-27
if ($.fn.dataTableExt != undefined) {
  $.fn.dataTableExt.oApi.fnReloadAjax = function (oSettings, sNewSource, fnCallback, bStandingRedraw) {
    if (typeof sNewSource != 'undefined' && sNewSource != null) {
      oSettings.sAjaxSource = sNewSource;
    }
    this.oApi._fnProcessingDisplay(oSettings, true);
    var that = this;
    var iStart = oSettings._iDisplayStart;
    var aData = [];

    this.oApi._fnServerParams(oSettings, aData);

    oSettings.fnServerData(oSettings.sAjaxSource, aData, function (json) {
      /* Clear the old information from the table */
      that.oApi._fnClearTable(oSettings);

      /* Got the data - add it to the table */
      var aData = (oSettings.sAjaxDataProp !== "") ?
        that.oApi._fnGetObjectDataFn(oSettings.sAjaxDataProp)(json) : json;

      for (var i = 0; i < aData.length; i++) {
        that.oApi._fnAddData(oSettings, aData[i]);
      }

      oSettings.aiDisplay = oSettings.aiDisplayMaster.slice();
      that.fnDraw();

      if (typeof bStandingRedraw != 'undefined' && bStandingRedraw === true) {
        oSettings._iDisplayStart = iStart;
        that.fnDraw(false);
      }

      that.oApi._fnProcessingDisplay(oSettings, false);

      /* Callback user function - for event handlers etc */
      if (typeof fnCallback == 'function' && fnCallback != null) {
        fnCallback(oSettings);
      }
    }, oSettings);
  };

  //Tunglittle add
  $.fn.dataTableExt.oApi.fnStandingRedraw = function (oSettings) {
    if (oSettings.oFeatures.bServerSide === false) {
      var before = oSettings._iDisplayStart;

      oSettings.oApi._fnReDraw(oSettings);

      // iDisplayStart has been reset to zero - so lets change it back
      oSettings._iDisplayStart = before;
      oSettings.oApi._fnCalculateEnd(oSettings);
    }

    // draw the 'current' page
    oSettings.oApi._fnDraw(oSettings);
  };
  //20131022: Tuan Vo Add
  $.fn.dataTableExt.oApi.fnSetFilteringDelay = function (oSettings, iDelay) {
    /*
     * Type:        Plugin for DataTables (www.datatables.net) JQuery plugin.
     * Name:        dataTableExt.oApi.fnSetFilteringDelay
     * Version:     2.2.1
     * Description: Enables filtration delay for keeping the browser more
     *              responsive while searching for a longer keyword.
     * Inputs:      object:oSettings - dataTables settings object
     *              integer:iDelay - delay in miliseconds
     * Returns:     JQuery
     * Usage:       $('#example').dataTable().fnSetFilteringDelay(250);
     * Requires:      DataTables 1.6.0+
     *
     * Author:      Zygimantas Berziunas (www.zygimantas.com) and Allan Jardine (v2)
     * Created:     7/3/2009
     * Language:    Javascript
     * License:     GPL v2 or BSD 3 point style
     * Contact:     zygimantas.berziunas /AT\ hotmail.com
     */
    var _that = this,
      iDelay = (typeof iDelay == 'undefined') ? 250 : iDelay;

    this.each(function (i) {
      $.fn.dataTableExt.iApiIndex = i;
      var $this = this,
        oTimerId = null,
        sPreviousSearch = null,
        anControl = $('input', _that.fnSettings().aanFeatures.f);

      anControl.unbind('keyup').bind('keyup', function () {
        var $$this = $this;

        if (sPreviousSearch === null || sPreviousSearch != anControl.val()) {
          window.clearTimeout(oTimerId);
          sPreviousSearch = anControl.val();
          oTimerId = window.setTimeout(function () {
            $.fn.dataTableExt.iApiIndex = i;
            _that.fnFilter(anControl.val());
          }, iDelay);
        }
      });

      return this;
    });
    return this;
  };
}
function ShowSuccess(message, title) {
  $.gritter.add({
    title: title || 'Success',
    text: message,
    image: '/images/icons/check.png'
  });
}

function ShowError(message, title) {
  $.gritter.add({
    title: title || 'Error',
    text: message,
    image: '/images/icons/bomb.png'
  });
}

function ShowInfo(message, title) {
  $.gritter.add({
    title: title,
    text: message,
    image: '/images/icons/info.png'
  });
}

function prettyDate(date_str) {
  var seconds = (new Date() - new Date(date_str)) / 1000;
  var token = 'ago', list_choice = 1;
  if (seconds < 0) {
    seconds = Math.abs(seconds);
    token = 'from now';
    list_choice = 2;
  }
  var i = 0, format;
  while (format = time_formats[i++])
    if (seconds < format[0]) {
      if (typeof format[2] == 'string')
        return format[list_choice];
      else if (i == 1)
        return format[1];
      else
        return Math.floor(seconds / format[2]) + ' ' + format[1] + ' ' + token;
    }
  return date_str;
};

function prettyDateByTicks(date_str) {
  var seconds = (new Date() - new Date(parseFloat(date_str))) / 1000;
  var token = 'ago', list_choice = 1;
  if (seconds < 0) {
    seconds = Math.abs(seconds);
    token = 'from now';
    list_choice = 2;
  }
  var i = 0, format;
  while (format = time_formats[i++])
    if (seconds < format[0]) {
      if (typeof format[2] == 'string')
        return format[list_choice];
      else if (i == 1)
        return format[1];
      else
        return Math.floor(seconds / format[2]) + ' ' + format[1] + ' ' + token;
    }
  return date_str;
};

var time_formats = [
  [60, 'just now', 1], // 60
  [120, '1 minute ago', '1 minute from now'], // 60*2
  [3600, 'minutes', 60], // 60*60, 60
  [7200, '1 hour ago', '1 hour from now'], // 60*60*2
  [86400, 'hours', 3600], // 60*60*24, 60*60
  [172800, 'yesterday', 'tomorrow'], // 60*60*24*2
  [604800, 'days', 86400], // 60*60*24*7, 60*60*24
  [1209600, 'last week', 'next week'], // 60*60*24*7*4*2
  [2419200, 'weeks', 604800], // 60*60*24*7*4, 60*60*24*7
  [4838400, 'last month', 'next month'], // 60*60*24*7*4*2
  [29030400, 'months', 2419200], // 60*60*24*7*4*12, 60*60*24*7*4
  [58060800, 'last year', 'next year'], // 60*60*24*7*4*12*2
  [2903040000, 'years', 29030400], // 60*60*24*7*4*12*100, 60*60*24*7*4*12
  [5806080000, 'last century', 'next century'], // 60*60*24*7*4*12*100*2
  [58060800000, 'centuries', 2903040000] // 60*60*24*7*4*12*100*20, 60*60*24*7*4*12*100
];

function getDate(date_ticks) {
  var date = new Date(parseFloat(date_ticks));
  date = new Date(date.getTime() + (date.getTimezoneOffset() * 60000));

  var day = date.getDate();
  var month = date.getMonth() + 1;
  var year = date.getFullYear();
  return month + "/" + day + "/" + year;
}

function getDateTime(date_ticks) {
  var date = new Date(parseFloat(date_ticks));
  date = new Date(date.getTime() + (date.getTimezoneOffset() * 60000));

  var day = date.getDate();
  var month = date.getMonth() + 1;
  var year = date.getFullYear();

  var hour = date.getHours();
  if (hour == 0) hour = 12;

  var mins = date.getMinutes();

  return month + "/" + day + "/" + year + " " + (hour > 12 ? hour - 12 : hour) + ":" + (mins < 10 ? "0" + mins : mins) + (hour > 12 ? " PM" : " AM");
}

function removeTips() {
  $('#tips').children().each(function () {
    $(this).remove();
  });
}

function addErrorMessagesCustom(diverror, response) {
  addErrors($('#' + diverror), response);
}

function addErrorMessages(response) {
  addErrors($('#error-messages'), response);
}

function addErrorMessagesTeachers(response) {
  addErrors($('#error-messages-teachers'), response);
}

function addErrors(messageList, response) {
  messageList.empty();
  for (var i = 0; i < response.ErrorList.length; i++) {
    messageList.append('<li>' + response.ErrorList[i].ErrorMessage + '</li>');
  }
  messageList.show();
}

function addErrorMessagesToList(response, errorListId) {
  var messageList = $('#' + errorListId);
  messageList.empty();
  for (var i = 0; i < response.ErrorList.length; i++) {
    messageList.append('<li>' + response.ErrorList[i].ErrorMessage + '</li>');
  }
  messageList.show();
}

function removeErrorMessages(errorListId) {
  var messageList = $('#' + errorListId);
  messageList.hide();
  messageList.empty();
}

function addSelectListItems(selectList, results) {
  if (results.length == 0) {
    selectList.html('<option value="-1">No Results Found</option>');
    return;
  }
  if (results.length == 1) {
    selectList
      .append($("<option></option>")
        .attr("value", results[0].Id)
        .attr("selected", true)
        .text(results[0].Name));
    selectList.change();
    return;
  }
  $.each(results, function (i, value) {
    selectList
      .append($("<option></option>")
        .attr("value", value.Id)
        .text(value.Name));
  });
}


function addSelectListItemsForTestAssignment(selectList, results) {
  if (results.length == 0) {
    selectList.html('<option value="-1">No Results Found</option>');
    return;
  }
  if (results.length == 1) {
    selectList
      .append($("<option></option>")
        .attr("value", results[0].Id)
        .attr("selected", true)
        .attr("isteacherled", results[0].IsTeacherLed)
        .attr("totalquestiongroup", results[0].TotalQuestionGroup)
        .text(results[0].Name));
    selectList.change();
    return;
  }
  $.each(results, function (i, value) {
    selectList
      .append($("<option></option>")
        .attr("value", value.Id)
        .attr("isteacherled", value.IsTeacherLed)
        .attr("totalquestiongroup", value.TotalQuestionGroup)
        .text(value.Name));
  });
}

//TOOD: Task: [LNKT-30796] Phase 2
//function addSelectListItemsWithAttribute(selectList, results) {
//    if (results.length == 0) {
//        selectList.html('<option value="-1">No Results Found</option>');
//        return;
//    }
//    if (results.length == 1) {
//        selectList
//                .append($("<option></option>")
//                .attr("value", results[0].Id)
//                .attr("selected", true)
//                .attr("ExtracId", results[0].ExtracId)
//                .text(results[0].Name));
//        selectList.change();
//        return;
//    }
//    $.each(results, function (i, value) {
//        selectList
//                .append($("<option></option>")
//                .attr("value", value.Id)
//                .attr("ExtracId", value.ExtracId)
//                .text(value.Name));
//    });
//}


function addSelectListItemsWithSelectedValue(selectList, results, selectedValue) {
  if (results.length == 0) {
    selectList.html('<option value="-1">No Results Found</option>');
    return;
  }
  if (results.length == 1) {
    selectList
      .append($("<option></option>")
        .attr("value", results[0].Id || results[0].id)
        .attr("selected", true)
        .text(results[0].Name || results[0].name));
    selectList.change();
    return;
  }
  $.each(results, function (i, value) {
    var option = $("<option></option>")
      .attr("value", value.Id || value.id)
      .text(value.Name || value.name);
    if (selectedValue == value.Id || value.id) {
      option.attr('selected', 'selected');
    }
    selectList.append(option);
  });

  if (selectedValue != null && selectedValue != undefined) {
    selectList.change();
    selectList.val(selectedValue);
  }
}

function addSelectListItemsWithSelectedValueIgnoreAutoSelectSingleOption(selectList, results, selectedValue) {
  if (results.length == 0) {
    selectList.html('<option value="-1">No Results Found</option>');
    return;
  }
  $.each(results, function (i, value) {
    var option = $("<option></option>")
      .attr("value", value.Id)
      .text(value.Name);
    if (selectedValue == value.Id) {
      option.attr('selected', 'selected');
    }
    selectList.append(option);
  });

  if (selectedValue != null && selectedValue != undefined) {
    selectList.change();
  }
}

function addOptionToSelect(selectList, results) {
  if (results.length == 0) {
    selectList.html('<option value="-1">No Results Found</option>');
    return;
  }
  $.each(results, function (i, value) {
    selectList
      .append($("<option></option>")
        .attr("value", value.Id)
        .text(value.Name));
  });
}

function addDefaultOptionNew(selectList, objctValue, objectText) {
  selectList.empty();
  selectList
    .append($("<option></option>")
      .attr("value", objctValue)
      .text("Select " + objectText));
}

function addDefaultOption(selectList, objectText) {
  selectList.empty();
  selectList
    .append($("<option></option>")
      .attr("value", "select")
      .text("Select " + objectText));
  var html = unescapeHtml(selectList.html());
  selectList.html(html);
}

function unescapeHtmlDropdownList(idDropdownlist) {
  var html = unescapeHtml(idDropdownlist.html());
  idDropdownlist.html(html);
}

function addDefaultOptionDefaultValue(selectList, objectText, defaultValue) {
  selectList.empty();
  selectList
    .append($("<option></option>")
      .attr("value", defaultValue)
      .text("Select " + objectText));
}

function addDefaultOptionToUserSchool(selectList, objectText) {
  selectList
    .append($("<option></option>")
      .attr("value", "-1")
      .text("Select " + objectText));
}

function addSelectListWithDefaultValue(selectList, results, defaultValue, fnResultFormat) {
  if (results.length == 0) {
    selectList.html('<option value="select">No Results Found</option>');
    return;
  }
  if (results.length === 1) {
    var transformedResult = (fnResultFormat) ? fnResultFormat(results[0]) : results[0].Name;
    selectList.append('<option value="' + results[0].Id + '">' + transformedResult + '</option>');
    selectList.val(results[0].Id);
    selectList.change();
    return;
  }
  $.each(results, function (i, value) {
    var transformedResult = (fnResultFormat) ? fnResultFormat(value) : value.Name;
    if (transformedResult != undefined) {
      var option = $("<option></option>").attr("value", value.Id).text(transformedResult);
      if (defaultValue != null && defaultValue == value.Id) {
        option.attr('selected', 'selected');
      }

      selectList.append(option);
    }
  });
  selectList.change();
}

function addUniqueItems(selectList, value, text) {
  $(selectList).empty();
  $(selectList).append($("<option></option>")
    .attr("value", value)
    .text(text));
}

//Author: TuanVo
//Description: Add vertical scrollbar to table
//Solution: Add a new div with overflow is visible and this div contains the table.
//          To enable the header to be visible, create a new table and move the existing header to new table
function addScrollBarToTable(dataTableId) {
  if (dataTableId == null) {
    return;
  }
  if (dataTableId.trim().length == 0) {
    return;
  }
  /* Add vertical scrollbar to table
  - The header and the footer need to stay visible and in place during scrolling,
      including the "show 25/50/75/100 entries" option and the search box on top;
  - The column headings "Name", "Subject", "Grade" on the test bank list and the column heading "Name"
      on the test list; and the "show nnn entries out of nnn" message at the bottom.
  */
  var divCoverName = 'div' + dataTableId;
  var tempDataTableHeader = dataTableId + 'Header';

  if ($("#" + divCoverName).length == 0) { //to avoid adding new div when call back
    $('<div id="' + divCoverName + '" style="height:328px;overflow-y: auto;overflow-x: hidden;margin-top: -22px">').insertBefore("#" + dataTableId);//height 328px is standard for ten lines
    $('<table id="' + tempDataTableHeader + '" style="height:32px !important" class="datatable table no-margin dataTable">').insertBefore("#" + divCoverName);
  }

  $("#" + dataTableId).appendTo("#" + divCoverName);//move table inside div
  $("#" + dataTableId).css('margin-left', '0px');
  $("#" + dataTableId).css('margin-top', '0px');

  if ($("#" + tempDataTableHeader + " thead").length == 0) {
    //Move header to tempDataTableHeader
    var tableHeader = $("#" + dataTableId + " thead");
    //After moving header to new table tempDataTableHeader, need to keep the same with for each column
    //The best way is to keep the same with is cloning the header and insert it to table dataTableId -> no need to clone anymore
    //var clonedTableHeader = tableHeader.clone().attr("id", "theadTemp" + dataTableId);

    tableHeader.appendTo("#" + tempDataTableHeader);
    //$("#" + dataTableId).prepend(clonedTableHeader); //No need to clone a new virtual header anymore

  }
  //$("#" + dataTableId).css('margin-top', '-32px');//hide the new cloned header
  //add class(es) of dataTableId for dataTableIdHeader
  var classes = $('#' + dataTableId).attr('class');
  $('#' + tempDataTableHeader).attr('class', classes);
}

//Author: TuanVo
//Description: Add word-break: break-all; to td which has long BankName ,such as NewBankDemo_123_456_789_1011
function addWordBreakToTableCell(td, maxLength, content) {
  if (content == null) {
    return;
  }
  var parts = content.split(" ");
  var longestWord = 0;
  if (parts != null) {
    for (var i = 0; i < parts.length; i++) {
      if (parts[i].length > longestWord) {
        longestWord = parts[i].length;
      }
    }
  }
  if (longestWord >= maxLength) {
    td.css('word-break', 'break-all');
  }
}
/* Special for xmlcontent of an item */
function addWordBreakToTableCellItem(td, maxLength, content) {
  if (content == null) {
    return;
  }
  content = $(content.replace(/>/g, '> ')).text(); //ignore all html tag
  var parts = content.split(" ");
  var longestWord = 0;
  if (parts != null) {
    for (var i = 0; i < parts.length; i++) {
      {
        if (parts[i].length > longestWord) {
          longestWord = parts[i].length;
        }
      }

    }
  }
  if (longestWord >= maxLength) {
    td.css('word-break', 'break-all');
  }
}
//Author: TuanVo
//Description: Format table to add vertical scrollbar to table
//Note: Take carefull when modify this function because it's use many places

function formatTableForAddingVertialScrollBar(dataTableId, defaultScrollStyle, defaultNoScrollStyle, defaultScrollIE9Style, defaultNoScrollIE9Style) {
  if (defaultScrollStyle == null) {
    defaultScrollStyle = '';
  }
  if (defaultNoScrollStyle == null) {
    defaultNoScrollStyle = '';
  }
  if (defaultScrollIE9Style == null) {
    defaultScrollIE9Style = '';
  }
  if (defaultNoScrollIE9Style == null) {
    defaultNoScrollIE9Style = '';
  }
  //Insert div to each td to fix the width of td
  $('#' + dataTableId + ' tbody tr td').each(function (idx, td) {
    var tmp = $(td).html();
    $(td).html('<div class="divContainerStyle">' + tmp + '</div>');
  });

  //Must insert div first to get the correct height of tr

  var height = 0;
  $('#' + dataTableId + ' tbody tr').each(function (index, tr) {
    height += $(tr).height();
  });
  if (height > $('#' + dataTableId + ' tbody').height()) {
    //there will be a scrollbar
    $('#' + dataTableId).removeClass(defaultNoScrollStyle);
  } else {
    $('#' + dataTableId).addClass(defaultNoScrollStyle);
  }
  // Select just Internet Explorer 9
  if (jQuery.browser.msie && jQuery.browser.version == '9.0') {
    addScrollBarToTable(dataTableId);//Add scrollbar to table
    $('#' + dataTableId).addClass(defaultScrollIE9Style);

    height = $('#' + dataTableId).height();
    if (height > $('#div' + dataTableId).height()) { //'#div' + dataTableId  was generated in method addScrollBarToTable
      //there will be a scrollbar
      $('#' + dataTableId).removeClass(defaultNoScrollIE9Style);
      $('#' + dataTableId).removeClass(defaultNoScrollStyle);
      $('#' + dataTableId).addClass(defaultScrollIE9Style);
    } else {
      $('#' + dataTableId).addClass(defaultNoScrollIE9Style);
      $('#' + dataTableId).removeClass(defaultScrollIE9Style);
    }
    var classes = $('#' + dataTableId).attr('class');
    $('#' + dataTableId + 'Header').attr('class', classes);
  }
}



(function ($) {
  $.fn.hasScrollBar = function () {
    return this.get(0).scrollHeight > this.height();
  }
})(jQuery);
//===========================================================================
/* jQuery Tiny Pub/Sub - v0.7 - 10/27/2011
 * http://benalman.com/
 * Copyright (c) 2011 "Cowboy" Ben Alman; Licensed MIT, GPL */

(function ($) {

  var o = $({});

  $.subscribe = function () {
    o.on.apply(o, arguments);
  };

  $.unsubscribe = function () {
    o.off.apply(o, arguments);
  };

  $.publish = function () {
    o.trigger.apply(o, arguments);
  };

}(jQuery));

//===========================================================================

var LinkIt = (function ($) {
  /*
   * USAGE: [param1 => some jQuery selector], [param2 => string || string array]
   * LinkIt.success('#notificationContainerId', 'message 1');
   * LinkIt.success('[data-type="notificationContainer"]', ['message 1','message 2']);
  */

  var notifySuccess = function (el, message) {
    var container = $('<ul style="display: none; margin-bottom: 20px;" class="message success"></ul>');
    var messages = $.makeArray(message);

    var messagesHtml = $.map(messages, function (value, index) {
      return '<li>' + value + '</li>';
    }).join('');

    container.append(messagesHtml);

    $(el).prepend(container);

    $(container).fadeIn(1000, function () {
      var self = $(this);
      self.delay(5000).fadeOut();
    });
  };

  var notifyError = function (el, message) {
    var container = $('<ul style="display: none; margin-bottom: 20px;" class="message error"></ul>');
    var messages = $.makeArray(message);

    var messagesHtml = $.map(messages, function (value, index) {
      return '<li>' + value + '</li>';
    }).join('');

    container.append(messagesHtml);

    $(el).prepend(container);

    $(container).fadeIn(1000);
  };
  var notifyWarningFadeOut = function (el, message) {
    var container = $('<ul style="display: none; margin-bottom: 20px;" class="message customwarning"></ul>');
      var messages = $.makeArray(message);

      var messagesHtml = $.map(messages, function (value, index) {
        return '<li>' + value + '</li>';
      }).join('');

      container.append(messagesHtml);

      $(el).prepend(container);


      $(container).fadeIn(1000, function () {
        var self = $(this);
        self.delay(5000).fadeOut();
      });
    };
  var notifyErrorFadeOut = function (el, message) {
    var container = $('<ul style="display: none; margin-bottom: 20px;" class="message error"></ul>');
    var messages = $.makeArray(message);

    var messagesHtml = $.map(messages, function (value, index) {
      return '<li>' + value + '</li>';
    }).join('');

    container.append(messagesHtml);

    $(el).prepend(container);


    $(container).fadeIn(1000, function () {
      var self = $(this);
      self.delay(5000).fadeOut();
    });
  };

  var utilities = {
    // param ==> { selectList, results, defaultValue, fnResultFormat, groupTitle }
    addSelectList: function (param) {
      var paramDefault = {
        groupTitle: 'Please select an option'
      };

      param = $.extend(paramDefault, param);

      if (param.results.length == 0) {
        param.selectList.html('<option value="select">No Results Found</option>');
        return;
      }

      param.selectList.html('<option value="select">' + param.groupTitle + '</option>');

      $.each(param.results, function (i, value) {
        var transformedResult = (param.fnResultFormat) ? param.fnResultFormat(value) : value.Name;
        var option = $("<option></option>").attr("value", value.Id).text(transformedResult);
        if (param.defaultValue != null && param.defaultValue == value.Id) {
          option.attr('selected', 'selected');
        }

        param.selectList.append(option);
      });
    }
  };

  return {
    success: notifySuccess,
    error: notifyError,
    errorFadeout: notifyErrorFadeOut,
    warningFadeout: notifyWarningFadeOut,
    util: utilities
  };


})(jQuery);

function checkValidBrowser(handleSuccess) {

  var urlValid = window.location.protocol + "//" + window.location.hostname + '/registration/getbrowsersupport';
  $.support.cors = true;

  $.ajax({
    type: 'GET',
    url: urlValid,
    cache: false,
    async: false,
    success: function (data) {
      chkBrowser(data, handleSuccess);
    }
  });

  if ('XDomainRequest' in window && window.XDomainRequest !== null) {

    var xdr = new XDomainRequest(); // Use Microsoft XDR
    xdr.open('get', urlValid);
    xdr.onload = function () {
      var dom = new ActiveXObject('Microsoft.XMLDOM'),
        JSON = $.parseJSON(xdr.responseText);

      dom.async = false;

      if (JSON == null || typeof (JSON) == 'undefined') {
        JSON = $.parseJSON(data.firstChild.textContent);
      }
      chkBrowser(JSON, handleSuccess);
    };

    xdr.onerror = function () {
      _result = false;
    };

    xdr.send();
  }
};

function chkBrowser(data, handleSuccess) {
  handleSuccess = handleSuccess || function() {};

  var bSupport = data.BrowserSupports;
  var nAgt = navigator.userAgent;
  var browserName = navigator.appName;
  var fullVersion = '' + parseFloat(navigator.appVersion);
  var majorVersion = parseInt(navigator.appVersion, 10);
  var nameOffset, verOffset, ix;

  // In Opera, the true version is after "Opera" or after "Version"
  if ((verOffset = nAgt.indexOf("Opera")) != -1) {
    browserName = "Opera";
    fullVersion = nAgt.substring(verOffset + 6);
    if ((verOffset = nAgt.indexOf("Version")) != -1)
      fullVersion = nAgt.substring(verOffset + 8);
  } else if ((verOffset = nAgt.indexOf("OPR")) != -1) {
    browserName = "Opera";
    fullVersion = nAgt.substring(verOffset + 6);
  }
  // In MSIE, the true version is after "MSIE" in userAgent
  else if ((verOffset = nAgt.indexOf("MSIE")) != -1) {
    browserName = "IE";
    fullVersion = nAgt.substring(verOffset + 5);
  }

  else if ((verOffset = nAgt.indexOf("rv")) != -1 && nAgt.indexOf("like Gecko") != -1) {
    browserName = "IE";
    fullVersion = nAgt.substring(verOffset + 3);
  }
  // In Chrome, the true version is after "Chrome"
  else if ((verOffset = nAgt.indexOf("Chrome")) != -1) {
    browserName = "Chrome";
    fullVersion = nAgt.substring(verOffset + 7);
  }
  // In Safari, the true version is after "Safari" or after "Version"
  else if ((verOffset = nAgt.indexOf("Safari")) != -1) {
    browserName = "Safari";
    fullVersion = nAgt.substring(verOffset + 7);
    if ((verOffset = nAgt.indexOf("Version")) != -1)
      fullVersion = nAgt.substring(verOffset + 8);
  }
  // In Firefox, the true version is after "Firefox"
  else if ((verOffset = nAgt.indexOf("Firefox")) != -1) {
    browserName = "Firefox";
    fullVersion = nAgt.substring(verOffset + 8);
  }
  // In most other browsers, "name/version" is at the end of userAgent
  else if ((nameOffset = nAgt.lastIndexOf(' ') + 1) <
    (verOffset = nAgt.lastIndexOf('/'))) {
    browserName = nAgt.substring(nameOffset, verOffset);
    fullVersion = nAgt.substring(verOffset + 1);
    if (browserName.toLowerCase() == browserName.toUpperCase()) {
      browserName = navigator.appName;
    }
  }
  // trim the fullVersion string at semicolon/space if present
  if ((ix = fullVersion.indexOf(";")) != -1)
    fullVersion = fullVersion.substring(0, ix);
  if ((ix = fullVersion.indexOf(" ")) != -1)
    fullVersion = fullVersion.substring(0, ix);

  majorVersion = parseInt('' + fullVersion, 10);
  if (isNaN(majorVersion)) {
    fullVersion = '' + parseFloat(navigator.appVersion);
    majorVersion = parseInt(navigator.appVersion, 10);
  }

  var browserSupport = false;

  for (var i = 0; i < bSupport.length; i++) {
    if (bSupport[i].Name.toLowerCase() === browserName.toLowerCase()) {
      if (majorVersion >= bSupport[i].Value) {
        browserSupport = true;
      }
    }
  }

  var acceptToHideBrowsersupport = localStorage.getItem("ACCEPT_TO_HIDE_BROWSERSUPPORT");
  var isUnaccepted = acceptToHideBrowsersupport !== "true";

  if (!browserSupport && isUnaccepted) {
    var msgString = data.MessageAlert.replace("[browserName]", browserName).replace("[browserVersion]", majorVersion);

    $("body").find(".browserSupport").remove();
    $("body").prepend($('<div class="browserSupport"><p class="alert-content"></p><a href="javascript:void(0);" class="close" onclick="closeBrowserSupportAlert()">&#x2715;</a></div>'));
    $(".browserSupport .alert-content").text(msgString);
    $("#message").css({ 'margin-top': '-17em' });
    $("#login-block").css({ 'margin-top': '-10em' });

    handleSuccess(msgString);
  }
}

function closeBrowserSupportAlert() {
  $(".browserSupport").remove();
  localStorage.setItem("ACCEPT_TO_HIDE_BROWSERSUPPORT", "true");
}
//Author: TuanVo
function CustomAlert(message, isLongMessage, customWidth) {
  var yesButton = '<button style="width:63px;margin-left:0px;" onclick="CustomAlertYesClick(this);">OK</button>';
  var messageTextAlign = 'center';
  var messageBoxWidth = 240;
  var diaglogWidth = 300;


  if (typeof isLongMessage != "undefined" && isLongMessage == true) {
    messageTextAlign = 'left';
    messageBoxWidth = 540;
    diaglogWidth = 600;
  }
  if (typeof customWidth != "undefined" && customWidth > 0) {
    messageBoxWidth = 400;
  }


  var strHtml = '<section class="grid_5">' +
    '<div class="block-border" style="width: ' + messageBoxWidth + 'px;">' +
    '<div class="block-content form" style="padding-bottom: 1em;text-align:center;"><div style="text-align:' + messageTextAlign + ';line-height: 18px;">' + message +
    '</div><div style="text-align: center;padding-top:20px;padding-bottom:10px;">' + yesButton + '</div></div></div></section>';
  $("<div></div>")
    .html(strHtml)
    .addClass("dialog")
    .attr("id", "CustomAlertDialog")
    .appendTo("body")

    .dialog({
      close: function () { $(this).remove(); },
      modal: true,
      width: diaglogWidth,
      maxheight: 400,
      resizable: false,
      open: function(event, ui) {
        $(event.target).dialog('widget')
          .css({ position: 'fixed' })
          .position({ my: 'center', at: 'center', of: window });
        $("#CustomAlertDialog").parent().find('.ui-dialog-titlebar-close').hide();
      }
    });
}

function customAlertMessage(option) {
  var yesButton = '<button style="width:63px;margin-left:0px;"';
  yesButton += ' onclick="CustomAlertYesClick(this);">OK</button>';

  if (option.textLeft == 'undefined' || option.textLeft == null) {
    textLeft = false;
  }
  var messageTextAlign = 'left';
  var diaglogWidth = 'auto';
  var messageBoxWidth = 420;
  var message = option.message;
  var style = 'style="max-width: ' + messageBoxWidth + 'px; min-width: 250px"';

  var strHtml = '<section class="grid_5">' +
    '<div class="block-border"' + style + '>' +
    '<div class="block-content form" style="padding-bottom: 1em;text-align:center;"><div style="text-align:' + messageTextAlign + ';line-height: 18px;">' + message +
    '</div><div style="text-align: center;padding-top:20px;padding-bottom:10px;">' + yesButton + '</div></div></div></section>';
  $("<div></div>")
    .html(strHtml)
    .addClass("dialog")
    .attr("id", "CustomAlertDialog")
    .appendTo("body")

    .dialog({
      close: function () {
        option.callback && option.callback();
        $(this).remove();
      },
      modal: true,
      width: diaglogWidth,
      maxheight: 400,
      resizable: false,
      zIndex: option.zIndex,
      open: function () {
        $("#CustomAlertDialog").parent().find('.ui-dialog-titlebar-close').hide();
        if (option.open) {
          option.open(this);
        }
      }
    });
}

function CustomAlertYesClick(item) {
  $("#CustomAlertDialog").dialog("close");
  var url = $(item).attr('url');
  if (url != undefined && $.trim(url) != '') window.open(url);
}

var option___F3882887_C118_41B9_AD02_E5A90B180FEE;
function CustomConfirm(option) {
  // Force options to be an object
  option___F3882887_C118_41B9_AD02_E5A90B180FEE = option || {};

  if (option.message == 'undefined' || option.message == null) {
    option.message = '';
  }

  if (option.textLeft == 'undefined' || option.textLeft == null) {
    option.textLeft = false;
  }
  var yesButton = '<button style="width:' + (option.yesWidth || 63) +'px;margin-left:0px;" onclick="' + "$(this).attr('disabled','disabled');option___F3882887_C118_41B9_AD02_E5A90B180FEE.yes();$('#CustomConfirmDialog__F3882887_C118_41B9_AD02_E5A90B180FEE').dialog('close')" + ';">' + (option.yesMessage || 'Yes')+'</button>';
  var noButton = '<button style="width:' + (option.noWidth || 63) +'px;margin-left:20px;" onclick="' + "option___F3882887_C118_41B9_AD02_E5A90B180FEE.no();$('#CustomConfirmDialog__F3882887_C118_41B9_AD02_E5A90B180FEE').dialog('close')" + ';">' + (option.noMessage || 'No')+'</button>';
  var messageTextAlign = 'center';


  var messageBoxWidth = 240;
  var diaglogWidth = 300;
  var diaglogId = 'CustomConfirmDialog__F3882887_C118_41B9_AD02_E5A90B180FEE';

  if (option.message.length >= 90) {
    messageTextAlign = 'left';
    messageBoxWidth = 540;
    diaglogWidth = 600;
  }

  var style = 'style="width: ' + messageBoxWidth + 'px;"';

  if (option.textLeft && option.message.length < 90 || option.textCenter && option.message.length < 90) {
    messageTextAlign = 'left';
    if (option.textCenter)
      messageTextAlign = 'center';
    diaglogWidth = 'auto';
    messageBoxWidth = 420;
    style = 'style="width: auto;max-width: ' + messageBoxWidth + 'px; min-width: 280px;"';
  }

  var strHtml = '<section class="grid_5 ' + (option.customClass || '') + '">' +
    '<div class="block-border"' + style + ' >' +
    '<div class="block-content form" style="padding-bottom: 1em;text-align:center;"><div style="text-align:' + messageTextAlign + ';line-height: 18px;">' + option.message +
    '</div><div style="text-align: center;padding-top:20px;padding-bottom:10px;">' + yesButton + noButton + '</div></div></div></section>';
  $("<div></div>")
    .html(strHtml)
    .addClass("dialog")
    .attr("id", diaglogId)
    .appendTo("body")

    .dialog({
      close: function () {
        $(this).remove();
        if (typeof option___F3882887_C118_41B9_AD02_E5A90B180FEE.close == 'function') {
          option___F3882887_C118_41B9_AD02_E5A90B180FEE.close();
        }
      },
      modal: true,
      width: diaglogWidth,
      maxheight: 400,
      resizable: false,
      open: function () {
        $("#CustomConfirmDialog__F3882887_C118_41B9_AD02_E5A90B180FEE").parent().find('.ui-dialog-titlebar-close').hide();
        if (typeof option___F3882887_C118_41B9_AD02_E5A90B180FEE.open == 'function') {
          option___F3882887_C118_41B9_AD02_E5A90B180FEE.open();
        }
        if (option.open) {
          option.open(this);
        }
        //$(".ui-dialog-titlebar-close").hide();

      }
    });
}

var option___F3882887_C118_41B9_AD02_E5A90B181FEE;
function CustomConfirmPopup(option, btnYesName, btnNoName) {
  // Force options to be an object
  option___F3882887_C118_41B9_AD02_E5A90B181FEE = option || {};

  if (option.message == 'undefined' || option.message == null) {
    option.message = '';
  }

  if (option.textLeft == 'undefined' || option.textLeft == null) {
    option.textLeft = false;
  }
  var captionYes, captionNo;

  if (btnYesName == 'undefined') {
    captionYes = "Yes";
  } else {
    captionYes = btnYesName;
  }

  if (btnNoName == 'undefined') {
    captionNo = "No";
  } else {
    captionNo = btnNoName;
  }

  var yesButton = '<button style="margin-left:0px;" onclick="' + "$(this).attr('disabled','disabled');option___F3882887_C118_41B9_AD02_E5A90B181FEE.yes();$('#CustomConfirmDialog__F3882887_C118_41B9_AD02_E5A90B181FEE').dialog('close')" + ';">' + captionYes + '</button>';
  var noButton = '<button style="margin-left:20px;" onclick="' + "option___F3882887_C118_41B9_AD02_E5A90B181FEE.no();$('#CustomConfirmDialog__F3882887_C118_41B9_AD02_E5A90B181FEE').dialog('close')" + ';">' + captionNo + '</button>';
  var messageTextAlign = 'center';


  var messageBoxWidth = 240;
  var diaglogWidth = 300;
  var diaglogId = 'CustomConfirmDialog__F3882887_C118_41B9_AD02_E5A90B181FEE';

  if (option.message.length >= 90) {
    messageTextAlign = 'left';
    messageBoxWidth = 540;
    diaglogWidth = 600;
  }

  var style = 'style="width: ' + messageBoxWidth + 'px;"';

  if (option.textLeft && option.message.length < 90 || option.textCenter && option.message.length < 90) {
    messageTextAlign = 'left';
    if (option.textCenter)
      messageTextAlign = 'center';
    diaglogWidth = 'auto';
    messageBoxWidth = 420;
    style = 'style="width: auto;max-width: ' + messageBoxWidth + 'px; min-width: 280px;"';
  }

  var strHtml = '<section class="grid_5">' +
    '<div class="block-border"' + style + ' >' +
    '<div class="block-content form" style="padding-bottom: 1em;text-align:center;"><div style="text-align:' + messageTextAlign + ';line-height: 18px;">' + option.message +
    '</div><div style="text-align: center;padding-top:20px;padding-bottom:10px;">' + yesButton + noButton + '</div></div></div></section>';
  $("<div></div>")
    .html(strHtml)
    .addClass("dialog")
    .attr("id", diaglogId)
    .appendTo("body")

    .dialog({
      close: function () {
        $(this).remove();
        if (typeof option___F3882887_C118_41B9_AD02_E5A90B181FEE.close == 'function') {
          option___F3882887_C118_41B9_AD02_E5A90B181FEE.close();
        }
      },
      modal: true,
      width: diaglogWidth,
      maxheight: 400,
      resizable: false,
      open: function () {
        $("#CustomConfirmDialog__F3882887_C118_41B9_AD02_E5A90B181FEE").parent().find('.ui-dialog-titlebar-close').hide();
        if (typeof option___F3882887_C118_41B9_AD02_E5A90B181FEE.open == 'function') {
          option___F3882887_C118_41B9_AD02_E5A90B181FEE.open();
        }
        //$(".ui-dialog-titlebar-close").hide();

      }
    });
}

function htmlEncode(value) {
  //create a in-memory div, set it's inner text(which jQuery automatically encodes)
  //then grab the encoded contents back out.  The div never exists on the page.
  return $('<div/>').text(value).html();
}
function htmlDecode(value) {
  return $('<div/>').html(value).text();
}

function encodeParameter(value) {
  if (value == null) {
    return '';
  }
  return encodeURIComponent(htmlEncode(value));
}
function addDefaultOptionNoSelect(selectList, objectText) {
  selectList.empty();
  selectList
    .append($("<option></option>")
      .attr("value", "select")
      .text(objectText));
  //.text("Select " + objectText));
}
//Author: TuanVo
//Break a long word to display it in a narrow place
function breakLongWordManually(text, standardLength) {
  if (text == null) {
    return text;
  }
  if (standardLength == undefined) {
    standardLength = 15;
  }
  if (text.length > standardLength) {
    if (text.indexOf(' ') < 0) {
      //there is no space
      var breakWords = ['\\', '/', ',', ';', '.', '&', '%', '(', ')', 'A', 'B', 'C', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'];
      var foundBreakWordIndex = -1;
      for (var i = 0; i < breakWords.length; i++) {
        if (text.indexOf(breakWords[i]) > 0) {
          foundBreakWordIndex = text.indexOf(breakWords[i]);
        }
      }
      if (foundBreakWordIndex > 0) {
        // break at foundBreakWordIndex
        return text.substring(0, foundBreakWordIndex) + ' ' + text.substring(foundBreakWordIndex, breakWords.length);

      } else {
        //break at standardLength
        return text.substring(0, standardLength) + ' ' + text.substring(standardLength, breakWords.length);
      }
    }
  }

}
//Author: TuanVo
//Replace a character or a string by another character or string
function replace(orgString, oldString, newString) {
  if (orgString == 'undefined' || orgString == null) {
    return '';
  }
  if (oldString == 'undefined' || oldString == null) {
    return '';
  }
  if (newString == 'undefined' || newString == null) {
    return '';
  }

  var newValue = orgString.replace(
    new RegExp("\\b" + oldString + "\\b", "gi"),
    newString);
  return newValue;
}
function numberTextBox_Keydown(e) {
  //debugger;
  // Allow: backspace, delete, tab, escape, enter and .
  if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
    // Allow: Ctrl+A
    (e.keyCode == 65 && e.ctrlKey === true) ||
    // Allow: home, end, left, right
    (e.keyCode >= 35 && e.keyCode <= 39)) {
    // let it happen, don't do anything
    return;
  }
  // Ensure that it is a number and stop the keypress
  if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105)) {
    e.preventDefault();
  }
}
$.fn.inputNumberOnly = function () {
  var self = this;
  self.keydown(function (e) {
    numberTextBox_Keydown(e);
  });
};
//Author: TuanVo
//Use to get text data from table (to compare data changed)
$.fn.getTableTextData = function () {
  var self = this;
  var data = [];
  self.find('tr').each(function (rowIndex, r) {
    var cols = [];
    $(this).find('td').each(function (colIndex, c) {
      cols.push(c.textContent);
    });
    data.push(cols);
  });
  return data;
};

function isFloatNumberKey(evt, allowNegative) {//allow to input '.' which charCode = 46
  var target = evt.target;
  if (target != null) {
    //get the current value and manually set the current value as a new attribute
    var currentValue = $(target).val();
    $(target).attr('_oldValue', currentValue);

  }
  var charCode = (evt.which) ? evt.which : evt.keyCode;
  if ($(target).val().indexOf('.') >= 0) {
    //Not allow to type another '.'
    if (charCode == 46) {
      return false;
    }

  }
  if (charCode == 46)// '.'
    return true;
  if (allowNegative != undefined && allowNegative == true) {
    //check if there's any -
    if ($(target).val().indexOf('-') >= 0) {
      //Not allow to type another '-'
      if (charCode == 45) {
        return false;
      }

    }
    if (charCode == 45) {//-
      return true;
    }
  }
  if (charCode > 31 && (charCode < 48 || charCode > 57))
    return false;

  return true;
}
function checkFloatNumberKeyup(evt, allowNegative) {//allow to input '.' which charCode = 46
  var target = evt.target;
  if (target != null) {
    //get the old value
    var oldValue = $(target).attr('_oldValue');
    if (oldValue != undefined && oldValue != null) {
      if (allowNegative != undefined && allowNegative == true) {
        //'-' must be at the begining
        if ($(target).val().indexOf('-') > 0) {
          //recover the old value
          $(target).val(oldValue);
          return;
        }
      }
      //get the new value
      var newValue = $(target).val();
      //
      if (newValue.length > 0) {
        //check if this is a valid float number or not
        var floatVal = parseFloat(newValue);
        if (floatVal == 'NaN') {
          //invalid float
          //recover the old value
          $(target).val(oldValue);
        }
      }
    }
  }

}
function isNumberKey(evt) {
  var charCode = (evt.which) ? evt.which : event.keyCode
  if (charCode > 31 && (charCode < 48 || charCode > 57))
    return false;

  return true;
}
//TuanVo: register event handler
function registerHandlerEvent(eventIdentifier, fn) {
  if (window.addEventListener) {
    window.addEventListener(eventIdentifier, fn);
  } else if (window.attachEvent) {
    window.attachEvent(eventIdentifier, fn);
  }
}
//Author: TuanVo
//Description: Allow a textbox to input float number only
$.fn.floatNumber = function (allowNegative) {
  $(this).keypress(function (e) {
    return isFloatNumberKey(e, allowNegative);
  });
  $(this).keyup(function (event) {
    checkFloatNumberKeyup(event, allowNegative);
  });
};

function EscapeHtml(input) {
  if (!!input) {
    return input.replace(/&/g, '&amp;')
      .replace(/"/g, '&quot;')
      .replace(/'/g, '&#39;')
      .replace(/</g, '&lt;')
      .replace(/>/g, '&gt;');
  }
  return input;
}


function unescapeHtml(safe) {
  if (!!safe) {
    return safe
      .replace(/&amp;/g, '&')
      .replace(/&lt;/g, '<')
      .replace(/&gt;/g, '>')
      .replace(/&quot;/g, '"')
      .replace(/&#039;/g, "'")
      .replace(/&#96;/g, "`");
  }

  return '';
}
// element is parent of loading
// isShow check show loading if isShow = false hide
// optional postion
function loading(element, isShow, option) {
  var defaults = {
    left: 'calc(50% - 12px)',
    top: 'calc(50% - 12px)'
  }
  var $div = $('.loader');
  if ($div.length === 0) {
    var div = document.createElement('div');
    div.className = 'loader';
    element.append(div);
    $div = $(div);
  }

  if (option) {
    var postion = {
      left: option.postionX ? option.postionX : 'calc(50% - 12px)',
      top: option.postionY ? option.postionY : 'calc(50% - 12px)'
    }
    $div.css(postion);
  } else {
    $div.css(defaults);
  }
  if (isShow) {
    $div.show();
    element.addClass('dvloading');
  } else {
    $div.hide();
    element.removeClass('dvloading');
  }
}

function addSelectListItemsWithDefaultValue(selectList, results, defaultValue) {
  if (results.length == 0) {
    selectList.html('<option value="-1">No Results Found</option>');
    return;
  }
  selectList.append($("<option></option>").attr("value", "-1").text(defaultValue));
  $.each(results, function (i, value) {
    selectList
      .append($("<option></option>")
        .attr("value", value.Id)
        .text(value.Name));
  });
}


function addSelectListItemsWithFirstItemSelected(selectList, results) {
  if (results.length == 0) {
    selectList.html('<option value="-1">No Results Found</option>');
    return;
  }
  var firstItem = results[0].Id;

  $.each(results, function (i, value) {
    var option = $('<option></option>')
      .attr('value', value.Id)
      .text(value.Name);
    if (firstItem == value.Id) {
      option.attr('selected', 'selected');
    }
    selectList.append(option);
  });
}

function ClearSessionsManageSchool() {
  if (sessionStorage.length > 0) {
    for (var i = sessionStorage.length; i > 0; i--) {
      if (sessionStorage.key(i) !== 'KEEP_SESSION') {
        sessionStorage.removeItem(sessionStorage.key(i));
      }
    }
  }
}

function getCellBymData(tableid, mData, nRow) {

  var columnIndex = getCellIndexBymData(tableid, mData);
  return $('td:eq(' + columnIndex + ')', nRow);
}
function getCellIndexBymData(tableid, mData) {
  var tableColumnsDefine = $(tableid).dataTable().fnSettings().aoColumns.map(function (c, ind) {
    return { mData: c.mData, index: ind, sName: c.sName };
  });
  var matchedCells = tableColumnsDefine.filter(function (columnDefine) {
    return columnDefine.mData.toLowerCase() === mData.toLowerCase();
  });
  if (matchedCells && matchedCells.length > 0) {
    return matchedCells[0].index;
  }
  return null;
}
function getCellIndexBysName(tableid, sName) {
  var tableColumnsDefine = $(tableid).dataTable().fnSettings().aoColumns.map(function (c, ind) {
    return { index: ind, sName: c.sName };
  });
  var matchedCells = tableColumnsDefine.filter(function (columnDefine) {
    return columnDefine.sName.toLowerCase() === sName.toLowerCase();
  });
  if (matchedCells && matchedCells.length > 0) {
    return matchedCells[0].index;
  }
  return null;
}

//Results List Object (Code(string), Name(string))
function addOptionToSelectWithString(selectList, results) {
  if (results.length == 0) {
    selectList.html('<option value="-1">No Results Found</option>');
    return;
  }
  $(selectList).empty();
  $.each(results, function (i, value) {
    selectList
      .append($("<option></option>")
        .attr("value", value.Code)
        .text(value.Name));
  });
}
