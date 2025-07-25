$.fn.select2.amd.define("CustomSelectionAdapter", [
    "select2/utils",
    "select2/selection/multiple",
    "select2/selection/placeholder",
    "select2/selection/eventRelay",
    "select2/selection/single",
  ],
  function (Utils, MultipleSelection, Placeholder, EventRelay, SingleSelection) {
    // Decorates MultipleSelection with Placeholder
    let adapter = Utils.Decorate(MultipleSelection, Placeholder);
    // Decorates adapter with EventRelay - ensures events will continue to fire
    // e.g. selected, changed
    adapter = Utils.Decorate(adapter, EventRelay);

    adapter.prototype.render = function () {
      // Use selection-box from SingleSelection adapter
      // This implementation overrides the default implementation
      let $selection = SingleSelection.prototype.render.call(this);
      return $selection;
    };

    adapter.prototype.update = function (data) {
      // copy and modify SingleSelection adapter
      this.clear();

      let $rendered = this.$selection.find('.select2-selection__rendered');
      let noItemsSelected = data.length === 0;
      let formatted = "";

      if (noItemsSelected) {
        formatted = this.options.get("placeholder") || "";
      } else {
        let itemsData = {
          selected: data || [],
          all: this.$element.find("option") || []
        };
        // Pass selected and all items to display method
        // which calls templateSelection
        formatted = this.display(itemsData, $rendered);
      }

      $rendered.empty().append(formatted);
      $rendered.prop('title', formatted);
    };

    return adapter;
  });
