var DraggableResizableComponent = Vue.extend({
    template: '\
        <div class="modal-component-draggable-resizable" v-show="show">\
            <div class="modal-component-container" v-el:resizable v-bind:style="styleModal">\
                <slot name="close">\
                    <a href="#" v-on:click.prevent="closeModal" class="modal-component-close">Close</a>\
                </slot>\
                <div class="modal-component-content">\
                    <div class="modal-component-body">\
                        <div v-el:resize-body class="modal-component-body-resize">\
                            <slot name="body">\
                                Default body\
                            </slot>\
                        </div>\
                        <div v-el:resize-handle class="modal-component-resize-handle"></div>\
                    </div>\
                </div>\
            </div>\
        </div>',
    props: {
        show: {
            type: Boolean,
            required: true,
            twoWay: true
        },
        width: {
            type: Number,
            default: 300
        },
        restriction: {
            type: String,
            default: 'parent'
        }
    },
    data: {
        styleModal: {}
    },
    created: function () {
        if (!!this.width) {
            this.styleModal = {
                width: this.width + 'px'
            };
        }
    },
    ready: function () {
        var vm = this;

        interact(vm.$el).draggable({
            inertia: true,
            restrict: {
                restriction: vm.restriction
            },
            onmove: vm.dragMoveListener
        });

        interact(vm.$els.resizable).resizable({
            edges: { 
                left: vm.$els.resizeHandle,
                right: vm.$els.resizeHandle,
                bottom: vm.$els.resizeHandle,
                top: vm.$els.resizeHandle
            },
            restrictSize: {
                min: { width: 300, height: 200 },
                max: { width: 1200, height: 690 }
            },
            inertia: true
        }).on('resizemove', vm.resizeMoveListener);
        
        document.addEventListener('keydown', vm.keydownListener);
    },
    methods: {
        closeModal: function () {
            this.show = false;
        },
        dragMoveListener: function (event) {
            var target = event.target;
            var x = (parseFloat(target.getAttribute('data-x')) || 0) + event.dx;
            var y = (parseFloat(target.getAttribute('data-y')) || 0) + event.dy;
        
            target.style.webkitTransform = target.style.transform = 'translate(' + x + 'px, ' + y + 'px)';
        
            target.setAttribute('data-x', x);
            target.setAttribute('data-y', y);
        },
        keydownListener: function (e) {
            if (this.show && e.keyCode == 27) {
                this.closeModal();
            }
        },
        resizeMoveListener: function (event) {
            var vm = this;
            var target = event.target;
            var x = (parseFloat(target.getAttribute('data-x')) || 0);
            var y = (parseFloat(target.getAttribute('data-y')) || 0);
    
            target.style.width  = event.rect.width + 'px';
            target.style.height = event.rect.height + 'px';
        
            x += event.deltaRect.left;
            y += event.deltaRect.top;
        
            target.style.webkitTransform = target.style.transform = 'translate(' + x + 'px,' + y + 'px)';
        
            target.setAttribute('data-x', x);
            target.setAttribute('data-y', y);
            
            vm.$els.resizeBody.style.height = (event.rect.height - 50) + 'px';
        }
    }
});

Vue.component('draggable-resizable-component', DraggableResizableComponent)
