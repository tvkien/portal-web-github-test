var DragDropAlgorithmic = (function () {

    function getFilters (xmlContent) {
        var filters = [];
        var $container = $('<div/>');
        $container.html(xmlContent);

        $container.find('responseDeclaration').each(function () {
            var $dragdrop = $(this);
            var dragdropResponseId = $dragdrop.attr('identifier');
            var arrSourceValues = [];
            var arrDestValues = [];
            $container.find('sourceObject').each(function () {
                var $sourcetext = $(this);
                var identifier = $sourcetext.attr('srcIdentifier');
                arrSourceValues.push(identifier);
            });

            $container.find('destinationItem').each(function () {
                var $sourcetext = $(this);
                var identifier = $sourcetext.attr('destIdentifier');
                arrDestValues.push(identifier);
            });

            filters.push({
                id: dragdropResponseId,
                label: dragdropResponseId,
                type: 'string',
                input: function(rule, name) {
                    var self = this;
                    var $container = rule.$el.find('.rule-value-container');
                    var rule = rule;
                    
                    $container.on('click', '[name='+ name +'_Add]', function(){
                        var i = $container.find('select').length;
                        var html = '<br/>\
                        <div class="wrap-value-container">\
                        <select name="'+ name +'_'+ (i+1) +'">'+ destValues +'</select> \
                        <select name="'+ name +'_'+ (i+2) +'">'+ sourceValues +'</select>\
                        </div>'; 
                        $container.append(html);
                    });

                    $container.on('change', '.wrap-value-container :not([name='+ name +'_1])', function() {
                        rule.$el.find('.wrap-value-container [name='+ name +'_1]').trigger('change');
                    });

                    var sourceValues = '<option value="-1">-----</option>';
                    for(var i = 0; i < arrSourceValues.length; i++) {
                        sourceValues += '<option value="'+ arrSourceValues[i] +'">'+ arrSourceValues[i] +'</option>';
                    }

                    var destValues = '<option value="-1">-----</option>';
                    for(var i = 0; i < arrDestValues.length; i++) {
                        destValues += '<option value="'+ arrDestValues[i] +'">'+ arrDestValues[i] +'</option>';
                    }

                    return '\
                    <div class="wrap-value-container">\
                    <select name="'+ name +'_1">'+ destValues +'</select> \
                    <select name="'+ name +'_2">'+ sourceValues +'</select> \
                    <button class="btn btn-xs btn-success" type="button" name="'+ name +'_Add"><i class="glyphicon glyphicon-plus"></i> Add More</button>\
                    </div>'; 
            
                },
                valueGetter: function(rule) {
                    var length = rule.$el.find('.rule-value-container select').length;
                    var result = "";
                    for (var i = 0; i < length; i+=2) {
                        if(i === length - 2) {
                            result += rule.$el.find('.rule-value-container [name$=_'+ (i+1) +']').val();
                            result += '-'+ rule.$el.find('.rule-value-container [name$=_'+ (i+2) +']').val();
                        } else {
                            result += rule.$el.find('.rule-value-container [name$=_'+ (i+1) +']').val();
                            result += '-'+ rule.$el.find('.rule-value-container [name$=_'+ (i+2) +']').val() + ',';
                        }   
                    }
                    return result;
                },
                valueSetter: function(rule, value) {
                    if (rule.operator.nb_inputs > 0) {
                        var grpval = value.split(',');

                        var sourceValues = '<option value="-1">-----</option>';
                        for(var i = 0; i < arrSourceValues.length; i++) {
                            sourceValues += '<option value="'+ arrSourceValues[i] +'">'+ arrSourceValues[i] +'</option>';
                        }
    
                        var destValues = '<option value="-1">-----</option>';
                        for(var i = 0; i < arrDestValues.length; i++) {
                            destValues += '<option value="'+ arrDestValues[i] +'">'+ arrDestValues[i] +'</option>';
                        }
                        
                        for (var i = 0; i< grpval.length; i++) {
                            var val = grpval[i].split('-');
                            if (i > 0) {
                                var html = '<br/>\
                                <div class="wrap-value-container">\
                                <select name="'+ name +'_'+ ((i*2)+1) +'">'+ destValues +'</select> \
                                <select name="'+ name +'_'+ ((i*2)+2) +'">'+ sourceValues +'</select>\
                                </div>'; 
                                rule.$el.find('.rule-value-container').append(html);          
                            }
                            rule.$el.find('.rule-value-container [name$=_'+ ((i*2)+1) +']').val(val[0]);
                            rule.$el.find('.rule-value-container [name$=_'+ ((i*2)+2) +']').val(val[1]);
                        }
                    }   
                },
                validation: {
                    callback: function(value, rule) {
                        if(value.indexOf("-1") !== -1) {
                            return "No value selected";
                        }
                        return true;
                    }
                }
            });
        });

        return filters;
    }

    function getQueryBuilder (xmlContent) {
        var filters = getFilters(xmlContent);
        var options = {
            plugins: [
                'sortable',
                'atleast',
                'bt-tooltip-errors'
            ],
            operators: ['equal'],
            conditions: ['AND', 'OR', 'ATLEAST'],
            filters: filters
        };

        return options;
    }

    return {
        getQueryBuilder: getQueryBuilder
    }

})();