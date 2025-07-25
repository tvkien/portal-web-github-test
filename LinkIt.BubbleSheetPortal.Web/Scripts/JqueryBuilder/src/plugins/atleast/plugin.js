
QueryBuilder.define('atleast', function(options) {
    var self = this;

    // Bind events
    this.on('afterInit', function() {
        self.$el.on('focusout.queryBuilder', '[data-atleast=group]', function() {
            var $group = $(this).closest(QueryBuilder.selectors.group_container);
            var group = self.getModel($group);
            group.atleast = this.value;
        });
        
        self.$el.on('change.queryBuilder', 'input[type=radio]', function() {
            var input = $(this).closest(QueryBuilder.selectors.group_container).find('[data-atleast=group]');
            if($(this).val() === 'ATLEAST') {
                if(input.length > 1) {
                    $(input[0]).show();
                } else {
                    input.show();
                };
            } else {
                if(input.length > 1) {
                    $(input[0]).hide();
                } else {
                    input.hide();
                };
            }
        });
    });

    // Init "atleast" property
    this.on('afterAddGroup', function(e, group) {
        group.__.atleast = 1;
    });

    // Modify templates
    this.on('getGroupTemplate.filter', function(h, level) {
        var $h = $(h.value);
        $h.find(QueryBuilder.selectors.condition_container).prepend(
            // '<input type="number" min="1" max="999" maxlength="3" style="width: 70px; display: none;" data-atleast="group" value=1></input>'
            '<input oninput="javascript: if (this.value.length > this.maxLength) this.value = this.value.slice(0, this.maxLength);" type = "number" min="1" maxlength = "3" style="width: 70px; display: none;" data-atleast="group" value="1"></intput>'
        );
        h.value = $h.prop('outerHTML');
    });

    // Export "atleast" to JSON
    this.on('groupToJson.filter', function(json, group) {
        if(parseInt(group.atleast, 10) < 1 || group.atleast == "") {
            json.value.atleast = "1";
        } else {
            json.value.atleast = group.atleast;
        }
    });

    // Read "atleast" from JSON
    this.on('jsonToGroup.filter', function(group, json) {        
        var input = group.value.$el.closest(QueryBuilder.selectors.group_container).find('[data-atleast=group]');
        if(input.length > 1) {
            $(input[0]).val(json.atleast);
            json.condition === 'ATLEAST' ? $(input[0]).show() : $(input[0]).hide();
        } else {
            input.val(json.atleast);
            json.condition === 'ATLEAST' ? input.show() : input.hide();
        }
        group.value.atleast = json.atleast;
    });

});

Model.defineModelProperties(Group, ['atleast']);

QueryBuilder.selectors.atleast = QueryBuilder.selectors.group_header + ' [data-atleast=group]';

QueryBuilder.extend({
    updateGroupAtleast: function(group) {
        group.$el.find('>' + QueryBuilder.selectors.atleast)
            .css('display', group.condition === 'ATLEAST' ? 'block' : 'none');
    }
});