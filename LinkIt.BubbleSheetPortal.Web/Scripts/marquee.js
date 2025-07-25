(function ( $ ) {
  $.fn.marquee = function(options) {
    var SPACE_ARROW = 17;
    var settings = $.extend({
      widthSelected: 250,
      id: $(this).attr('id')
    }, options);
    $(this).css('border', 'none');
    var translateWidth = (parseInt(settings.widthSelected) - SPACE_ARROW );
    var keyframe = '<style id="keyframe-bouncing-text_' + settings.id +'">'
    keyframe += '@-moz-keyframes keyframe-bouncing-text_'+ settings.id +' { ';
    keyframe += '0%, 10% { -moz-transform: translateX(0);}';
    keyframe += '90%, 100% { -moz-transform: translateX(-100%) translateX(' + translateWidth +'px);}';
    keyframe += '}';
    keyframe += '@-webkit-keyframes keyframe-bouncing-text_'+ settings.id +' {';
    keyframe += '0%,10%  {-webkit-transform:  translateX(0);}';
    keyframe += ' 90%, 100% { -webkit-transform: translateX(-100%) translateX(' + translateWidth +'px);}';
    keyframe += '}';
    keyframe += '@keyframes keyframe-bouncing-text_'+ settings.id +' {';
    keyframe += '0%,10% {-moz-transform:  translateX(0); -webkit-transform:  translateX(0);';
    keyframe += '-ms-transform:  translateX(0);';
    keyframe += 'transform:  translateX(0);}';
    keyframe += '90%, 100% {';
    keyframe += ' -moz-transform: translateX(-100%) translateX(' +translateWidth +'px);';
    keyframe += '-webkit-transform: translateX(-100%) translateX(' + translateWidth +'px);';
    keyframe += ' -ms-transform: translateX(-100%) translateX(' + translateWidth+'px);';
    keyframe += 'transform: translateX(-100%) translateX(' + translateWidth +'px);}';
    keyframe += '}';

    keyframe += '</style>';
    if ($('#keyframe-bouncing-text_' + settings.id).length === 0) {
      $('body').prepend(keyframe);
    }
    var parent = $(this).parent('.block-text-name');

    parent.css('width', parseInt(settings.widthSelected));
    parent.find('.box-select').css('width', parseInt(settings.widthSelected) - SPACE_ARROW);
    parent.find('.box-select').addClass('short-text');

    var idSelect = $(this).attr('id');
    $(document).on('change', 'select', function() {
      setTimeout(function() {
        var select = $('.block-text-name').children('select');
        $.each(select, function(index, el) {
          if ($(el).has('option').length === 0) {
            $(el).parent('.block-text-name').find('.box-select').addClass('short-text')
          }
        })
      }, 200)

    })
    $(document).on('change','#' + idSelect ,function() {
      var fontSize = $(this).css('font-size');
      var fontFamily = $(this).css('font-family');
      parent.find('.box-select').find('.overlay').css({
        'font-size': fontSize,
        'font-family': fontFamily
      })
      var div =  document.createElement('span');
      div.id = 'textWidth_marquee';

      div.className = 'text-width-test';
      div.style.fontSize = fontSize;
      div.style.fontFamily = fontFamily;
      div.style.paddingLeft = '10px';
      div.style.paddingRight = '10px';
      div.innerHTML = $(this).children('option:selected').text();
      var span = $(this).parent('.block-text-name').find('.overlay');
      $(span).html($(this).children('option:selected').text());

      $('body').append(div);
      var width = $('#textWidth_marquee').width() + 10;
      $(span).css({
        '-moz-animation-name': 'none',
         '-webkit-animation-name': 'none',
         '-ms-animation-name': 'none',
        '-o-animation-name': 'none',
        'animation-name': 'none'
      });
      var widthSelected = parseInt(settings.widthSelected) - SPACE_ARROW;
      if (width >= widthSelected) {
          var SPEED = 60;
          var runWidth = width - widthSelected / 2;
          var time = ( runWidth / SPEED) + 's';
         $(span).addClass('animation-text');
         $(this).parent('.block-text-name').find('.box-select').removeClass('short-text');
         $(span).css({
           '-moz-animation-duration': time,
           '-webkit-animation-duration': time,
           '-ms-animation-duration': time,
           '-o-animation-duration': time,
           'animation-duration': time
         });
         $(span).css({
          '-moz-animation-name': 'keyframe-bouncing-text_' + settings.id,
           '-webkit-animation-name': 'keyframe-bouncing-text_' + settings.id,
           '-ms-animation-name': 'keyframe-bouncing-text_' + settings.id,
          '-o-animation-name': 'keyframe-bouncing-text_' + settings.id,
          'animation-name': 'keyframe-bouncing-text_' + settings.id
        });
      } else {

        $(span).css({
          '-moz-animation-name': 'none',
           '-webkit-animation-name': 'none',
           '-ms-animation-name': 'none',
          '-o-animation-name': 'none',
          'animation-name': 'none'
        });
         $(this).parent('.block-text-name').find('.box-select').addClass('short-text')
      }
      $('.text-width-test').remove();
      div.remove();
    })
    return this;
  };

}( jQuery ));
