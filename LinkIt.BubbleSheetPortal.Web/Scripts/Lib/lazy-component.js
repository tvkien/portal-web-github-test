Vue.component('lazy-component', {
  template: '\
      <div>\
      <slot v-if="isIntersected" />\
      <slot v-if="!isIntersected" name="placeholder" />\
    </div>\
  ',
  data: function () {
    return {
      isIntersected: false,
      observer: null
    }
  },
  ready: function () {
    if ("IntersectionObserver" in window) {
      this.observe();
    } else {
      this.isIntersected = true;
    }
  },
  methods: {
    observe: function () {
      var rootMargin = "0px 0px 0px 0px";
      var threshold = 0;
      var config = { root: undefined, rootMargin, threshold };
      this.observer = new IntersectionObserver(this.onIntersection, config);
      this.observer.observe(this.$el);
    },
    onIntersection: function (entries) {
      this.isIntersected = entries.some(function (entry) { return entry.intersectionRatio > 0 });
      if (this.isIntersected) {
        this.unobserve();
      }
    },
    unobserve: function () {
      if ("IntersectionObserver" in window) {
        this.observer.unobserve(this.$el);
      }
    }
  },
  beforeDestroy: function () {
    this.unobserve();
  },
})
