var $document = $(document);
var profilerButton = document.createElement('div');
profilerButton.className = 'profiler-button';
profilerButton.textContent = 'Collapse -';

var profilerCollapse = document.createElement('div');
profilerCollapse.className = 'profiler-actions';
profilerCollapse.appendChild(profilerButton);
profilerCollapse.style.position = 'fixed';
profilerCollapse.style.bottom = '0'
profilerCollapse.style.right = '0';
profilerCollapse.style.zIndex = 9999999999;
profilerCollapse.style.padding = '4px 7px';
profilerCollapse.style.cursor = 'pointer';
profilerCollapse.style.textAlign = 'center';

document.body.appendChild(profilerCollapse);

profilerButton.addEventListener('click', function (event) {
    var target = event.target;
    var $profiler = $document.find('.profiler-result');

    if (target.classList.contains('is-active')) {
        target.classList.remove('is-active');
        target.textContent = 'Collapse -';
        $profiler.show();
    } else {
        target.classList.add('is-active');
        target.textContent = 'Expand +';
        $profiler.hide();
    }
}, false)
