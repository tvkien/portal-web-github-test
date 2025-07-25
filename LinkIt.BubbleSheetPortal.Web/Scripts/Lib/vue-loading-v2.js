var LoadingDirective = {
    params: ['loadingOptions'],
    handleShow: function () {
        var position = window.getComputedStyle(this.el).position;

        if (position === 'static' || position === '') {
            this.static = true;
            this.el.style.position = 'relative';
        }

        var box = document.createElement('div');
        box.className = 'vue-loading';
        box.style.backgroundColor = this.options.backgroundColor;
        this.el.appendChild(box);

        var boxInner = document.createElement('div');
        boxInner.className = 'block-loading';
        box.appendChild(boxInner);

        var icoLoading = document.createElement('div');
        icoLoading.className = 'loading-ring-container';
        boxInner.appendChild(icoLoading);

        for (var ring = 1; ring < 4; ring++) {
          var ringEl = document.createElement('div');
          ringEl.className = 'ring-' + ring;
          icoLoading.appendChild(ringEl);

          for (var i = 0; i < 4; i++) {
            var divEl = document.createElement('div');
            ringEl.appendChild(divEl);
          }
        }

        var msg = document.createElement('span');
        msg.className = 'text-loading';
        msg.textContent = this.options.text;
        boxInner.appendChild(msg);

        window.requestAnimationFrame(function () {
            box.style.opacity = 1;
        });

        this.loadingBox = box;
    },
    handleHide: function () {
        var self = this;

        this.loadingBox.addEventListener('transitionend', function () {
            if (self.loadingBox && self.loadingBox.parentNode) {
                self.loadingBox.parentNode.removeChild(self.loadingBox);
            }

            if (self.static) {
                self.el.style.removeProperty('position');
            }
        });

        self.loadingBox.style.opacity = 0.8;
    },
    bind: function () {
        this.static = false;
        this.loadingBox = null;
        this.first = true;
        this.options = {
            backgroundColor: 'rgba(0, 0, 0, .6)',
            text: 'Loading...'
        };

        if (this.params.loadingOptions) {
            Object.assign(this.options, this.params.loadingOptions);
        }
    },
    update: function (value) {
        if (value) {
            this.first = false;
            this.handleShow();
        } else {
            if (this.first) {
                this.first = false;
                return;
            }
            this.handleHide();
        }
    }
};

Vue.directive('loading-directive', LoadingDirective)
