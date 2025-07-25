var DragDropSequenceOrderAlgorithmic = (function () {
    
    function getFilters (xmlContent) {
        var filters = [];
        var $container = $('<div/>');
        $container.html(xmlContent);

        $container.find('responseDeclaration').each(function () {
            var $dragdropSequence = $(this);
            var dragdropSequenceResponseId = $dragdropSequence.attr('identifier');
            var arrSourceValues = [];
            
            $container.find('sourceItem').each(function () {
                var $sourcetext = $(this);
                var identifier = $sourcetext.attr('identifier');
                arrSourceValues.push(identifier);
            });

            filters.push({
                id: dragdropSequenceResponseId,
                label: dragdropSequenceResponseId,
                type: 'string',
                input: function(rule, name) {
                    var resultHtml = "";
                    var sourceValues = '<option value="-1">-----</option>';
                    for(var i = 0; i < arrSourceValues.length; i++) {
                        sourceValues += '<option value="'+ arrSourceValues[i] +'">'+ arrSourceValues[i] +'</option>';
                    }

                    for(var i = 0; i < arrSourceValues.length; i++) {
                        resultHtml += '<div class="wrap-value-container"><select name="'+ name +'_'+ (i+1) +'">'+ sourceValues +'</select></div><br/>';
                    }
                    return resultHtml;
                },
                valueGetter: function(rule) {
                    var result = "";
                    for(var i = 0; i < arrSourceValues.length; i++) {
                        if(i === arrSourceValues.length - 1) {
                            result += rule.$el.find('.rule-value-container [name$=_'+ (i+1) +']').val();
                        } else {
                            result += rule.$el.find('.rule-value-container [name$=_'+ (i+1) +']').val() + ',';
                        }
                    }
                    return result;
                },
                valueSetter: function(rule, value) {
                    if (rule.operator.nb_inputs > 0) {
                        var val = value.split(',');

                        for(var i = 0; i < val.length; i++) {
                            rule.$el.find('.rule-value-container [name$=_'+ (i+1) +']').val(val[i])
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