var ModalComponent = Vue.extend({
    template: '\
        <div class="modal-component" v-show="show" transition="modal-component">\
            <div class="modal-component-wrapper">\
                <div class="modal-component-container" v-bind:style="styleModal">\
                    <slot name="close">\
                        <a href="#" v-on:click.prevent="closeModal" class="modal-component-close">Close</a>\
                    </slot>\
                    <div class="modal-component-content">\
                        <div class="modal-component-header">\
                            <slot name="header">\
                                Default header\
                            </slot>\
                        </div>\
                        <div class="modal-component-body">\
                            <slot name="body">\
                                Default body\
                            </slot>\
                        </div>\
                        <div class="modal-component-footer">\
                            <slot name="footer">\
                                Default footer\
                            </slot>\
                        </div>\
                    </div>\
                </div>\
            </div>\
        </div>',
    props: {
        show: {
            type: Boolean,
            required: true,
            default: false,
            twoWay: true
        },
        width: {
            type: Number,
            default: 300
        }
    },
    computed: {
      styleModal: function () {
        return {
          width: this.width + 'px'
        };
      },
    },
    ready: function () {
        var self = this;

        document.addEventListener('keydown', function (e) {
            if (self.show && e.keyCode == 27) {
                self.closeModal();
            }
        });
    },
    methods: {
        closeModal: function () {
          this.show = false;
          this.$emit('update:show', false);
        }
    }
});

Vue.component('modal-component', ModalComponent)
