var InlineChoiceAlgorithmic = (function () {

    function getFilters (xmlContent) {
        var filters = [];
        var $container = $('<div/>');
        $container.html(xmlContent);

        $container.find('inlineChoiceInteraction').each(function () {
            var $inlinechoiceinteraction = $(this);
            var inlinechoiceinteractionResponseId = $inlinechoiceinteraction.attr('responseIdentifier');
            var inlinechoiceValues = [];

            $inlinechoiceinteraction.find('inlineChoice').each(function () {
                var $inlinechoice = $(this);
                var inlinechoiceIdentifier = $inlinechoice.attr('identifier');
                inlinechoiceValues.push(inlinechoiceIdentifier);
            });

            filters.push({
                id: inlinechoiceinteractionResponseId,
                label: inlinechoiceinteractionResponseId,
                type: 'string',
                input: 'select',
                values: inlinechoiceValues
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