var TextEntryAlgorithmic = (function () {
    
    function getFilters (xmlContent) {
        var filters = [];
        var $container = $('<div/>');
        $container.html(xmlContent);
        var isRange = xmlContent.indexOf('range="true"') != -1 ? true : false;

        $container.find('textEntryInteraction').each(function () {
            var $textentryinteraction = $(this);
            var textentryinteractionResponseId = $textentryinteraction.attr('responseIdentifier');
            var textentryValues = [];

            $container.find('responseDeclaration correctResponse value').each(function () {
                var $value = $(this);
                var valueIdentifier = $value.html();                
                textentryValues.push(valueIdentifier);
            });

            if(isRange) {
                filters.push({
                    id: textentryinteractionResponseId,
                    label: textentryinteractionResponseId,
                    type: 'double',
                    input: 'number'
                });
            } else {
                textentryValues.push('Anything else');
                filters.push({
                    id: textentryinteractionResponseId,
                    label: textentryinteractionResponseId,
                    type: 'string',
                    input: 'select',
                    values: textentryValues
                });
            }
        });

        return filters;
    }

    function getQueryBuilder (xmlContent) {
        var isRange = xmlContent.indexOf('range="true"') != -1 ? true : false;
        
        var filters = getFilters(xmlContent);
        if(isRange) {
            var options = {
                plugins: [
                    'sortable',
                    'bt-tooltip-errors'
                ],
                operators: ['equal', 'less', 'less_or_equal', 'greater', 'greater_or_equal'],
                conditions: ['AND'],
                filters: filters
            };
        } else {
            var options = {
                plugins: [
                    'sortable',
                    'bt-tooltip-errors'
                ],
                operators: ['equal'],
                conditions: ['OR'],
                default_condition: 'OR',
                filters: filters
            };
        }
        

        return options;
    }

    return {
        getQueryBuilder: getQueryBuilder
    }
})();