(function($) {
    var originFunctions = {
      dialog: $.fn.dialog
    }
  
    $.fn.dialog = function(action) {
      if (typeof originFunctions.dialog !== 'function') return;
      var dialog = $(this).hasClass('ui-dialog') ? $(this).get(0) : $(this).closest('.ui-dialog').get(0);
      if (action === 'destroy') {
        originFunctions.dialog.call(this, 'close');
        return originFunctions.dialog.call(this, 'destroy');
      } else if (action === 'center' && window.IS_V2 && dialog && dialog.style.position !== 'fixed') {
        var rect = dialog.getBoundingClientRect();
        $(dialog).css({
          left: (document.documentElement.clientWidth - rect.width) / 2 + 'px',
          top: Math.max(document.documentElement.scrollTop + 20, document.documentElement.scrollTop + (window.innerHeight - rect.height - 70) / 2) + 'px',
          transform: 'none'
        })
      } else {
        if (typeof arguments[0] === 'object') {
          var openCallback = arguments[0].open;
          arguments[0].open = function (e) {
            $(this).dialog('center');
            openCallback && openCallback.apply(this, [e]);
          }
        }
        return originFunctions.dialog.apply(this, arguments);
      }
    }

    $.ui.dialog.currentZ = function() {
      var zIndexs = $('.ui-dialog').map(function() {
        return $(this).css('display') !== 'none' ? parseInt($(this).css('z-index')) : 1
      }).toArray();
      return Math.max.apply(Math, zIndexs);
    }
  
    $.effects.highlight = function(o) {
      return this.queue(function() {
        var elem = $(this),
          props = ['backgroundImage', 'backgroundColor', 'opacity'],
          mode = $.effects.setMode(elem, o.options.mode || 'show'),
          animation = {
            backgroundColor: elem.css('backgroundColor')
          };
  
        if (mode == 'hide') {
          animation.opacity = 0;
        }
  
        $.effects.save(elem, props);
        elem
          .show()
          .css({
            backgroundImage: 'none',
            backgroundColor: o.options.color || '#ffff99'
          })
          .animate(animation, {
            queue: false,
            duration: o.duration,
            easing: o.options.easing,
            complete: function() {
              (mode == 'hide' && elem.hide());
              $.effects.restore(elem, props);
              (mode == 'show' && !$.support.opacity && this.style.removeAttribute && this.style.removeAttribute('filter'));
              (o.callback && o.callback.apply(this, arguments));
              elem.dequeue();
            }
          });
      });
    };
  
  })($)
