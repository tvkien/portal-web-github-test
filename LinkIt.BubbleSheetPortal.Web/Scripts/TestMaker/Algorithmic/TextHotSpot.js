var TextHotSpotAlgorithmic = (function () {
    
    function getFilters (xmlContent) {
        var filters = [];
        var $container = $('<div/>');
        $container.html(xmlContent);

        $container.find('textHotSpot').each(function () {
            var $texthotspot = $(this);
            var texthotspotResponseId = $texthotspot.attr('responseIdentifier');
            var arrValues = [];
            $container.find('sourceText').each(function () {
                var $sourcetext = $(this);
                var sourcetextIdentifier = $sourcetext.attr('identifier');
                arrValues.push(sourcetextIdentifier);
            });

            filters.push({
                id: texthotspotResponseId,
                label: texthotspotResponseId,
                type: 'string',
                input: 'select',
                values: arrValues,
                multiple: true
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
            operators: ['in'],
            conditions: ['AND', 'OR', 'ATLEAST'],
            filters: filters
        };

        return options;
    }

    return {
        getQueryBuilder: getQueryBuilder
    }

})();