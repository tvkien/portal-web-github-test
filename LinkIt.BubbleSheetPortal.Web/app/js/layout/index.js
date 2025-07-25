$(function () {
  $("#main-nav li a[href='@Request.Url.LocalPath']").parent().addClass("current");
  $("#main-nav li a[href='@Request.Url.LocalPath']").parent().parent().parent().addClass("current");

  if (_isShowIconInfo == 'True') {
    getHelpTextInfo();
  }
});
$('body').click(function () {
  $('#linkGoBack').hide();
  $('#divNetworkAdminSelectLayout').hide();

});
$('#imgGoBack').click(function (event) {
  if (event.stopPropagation) {
    event.stopPropagation();
  }
  else if (window.event) {
    window.event.cancelBubble = true;
  }
});
$('#linkGoBack').click(function (event) {
  if (event.stopPropagation) {
    event.stopPropagation();
  }
  else if (window.event) {
    window.event.cancelBubble = true;
  }
});
$('#imgNetworkAdminSelect').click(function (event) {
  if (notificationVM) {
    notificationVM.removeIsUnreadNotification();
  }

  if (event.stopPropagation) {
    event.stopPropagation();
  }
  else if (window.event) {
    window.event.cancelBubble = true;
  }
});
$('#divNetworkAdminSelectLayout').click(function (event) {
  if (event.stopPropagation) {
    event.stopPropagation();
  }
  else if (window.event) {
    window.event.cancelBubble = true;
  }
});

$('.js-info-action').on('click', function (event) {
  var html = $('#hdHelpTextInfo').val();
  popupAlertInfoMessage(html, 'ui-popup-info', 500, 500);

  if (isSafari) {
    $('.ui-popup-info').find('.popup-info-content').addClass('scrollbar-safari');
  }
});
function getHelpTextInfo() {
  var url = 'Help/GetHelpTextModule';
  var subTabCurrent = $('.menu-item ul li.current a');

  var displayName = '';
  if (subTabCurrent != null && subTabCurrent.length > 0) {
    displayName = subTabCurrent.text();
  } else {
    var tabCurrent = $('#main-nav ul li.current a');
    if (tabCurrent != null && tabCurrent.length > 0) {
      displayName = tabCurrent.text();
    }
  }
  if (displayName == '') {
    return;
  }

  $.ajax({
    url: url,
    cache: false,
    data: { displayName: displayName.trim() }
  }).done(function (html) {
    $('#hdHelpTextInfo').val(html);
    if (html != '') {
      $('.js-info-action').show().css('display', 'inline-block');
    } else {
      $('.js-info-action').hide();
    }
  });
}

function isSafari() {
  var browserUserAgent = navigator.userAgent.toString().toLowerCase();

  return browserUserAgent.indexOf('safari') != -1 && browserUserAgent.indexOf('chrome') == -1;
}

function popupAlertInfoMessage(content, contentClass, w, h) {
  var now = new Date().getTime();
  var contentHtml = '';
  var $div = $('<div />');

  w = w !== undefined ? w : 400;
  h = h !== undefined ? h : 100;

  contentHtml += '<div class="popup-info">';
  contentHtml += '<div class="popup-info-content">';
  contentHtml += content;
  contentHtml += '</div>';
  contentHtml += '</div>';

  $div.html(contentHtml)
    .attr('id', 'popup-info-' + now)
    .appendTo('body')
    .dialog({
      modal: true,
      width: w,
      maxHeight: h,
      resizable: false,
      dialogClass: contentClass,
      close: function () {
        $(document).find('#popup-info-' + now).dialog('destroy').remove();
      }
    });
};


function showFooter() {
  $('section:visible:last').addClass('lastVisibleSection');
  $('#custom-footer').appendTo('#flashContentContainer');
}

$(document).ready(function () {
  $.ajaxSetup({ cache: false });
  showFooter();
});

function showGoBack() {
  $('#linkGoBack').show();
  var top = $('#imgGoBack').offset().top;
  var left = $('#imgGoBack').offset().left;
  $('#linkGoBack').offset({ top: top + 22, left: left + 2 });

}

function goBackToOriginalUser() {
  ShowBlock($('body'), 'Going back to @(((UserPrincipal)User).OriginalUsername)');
  $.ajax({
    url: 'Admin/GoBackToOriginalUser',
    type: 'POST',
    contentType: 'application/json',
    success: function (response) {
      if (response.Success == true) {
        location.href = response.RedirectUrl;
      } else {
        $('body').unblock();
      }
    }
  });
}
function showNetworkAdminSelect() {
  $('#divNetworkAdminSelectLayout').show();

  var url = 'Account/ShowNetworkAdminSelectLayout';
  $.ajax({
    url: url,
    cache: false
  })
    .done(function (html) {
      $('#divNetworkAdminSelectLayout').html(html);
    });
  var top = $('#imgNetworkAdminSelect').offset().top;
  var left = $('#imgNetworkAdminSelect').offset().left - 200;
  $('#divNetworkAdminSelectLayout').offset({ top: top + 33, left: left + 2 });
  $('#divNetworkAdminSelectLayout').css('border', '2px solid #999');
  $('#divNetworkAdminSelectLayout').css('height', 'auto');

}
