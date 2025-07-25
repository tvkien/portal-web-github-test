var NumberlineHotSpotAlgorithmic = (function () {
    
    function getFilters (xmlContent) {
        var filters = [];
        var $container = $('<div/>');
        $container.html(xmlContent);

        $container.find('numberLine').each(function () {
            var $numberline = $(this);
            var numberlineResponseId = $numberline.attr('responseIdentifier');
            var arrValues = [];

            $container.find('numberLineItem').each(function () {
                var $numberlineitem = $(this);
                var sourcetextIdentifier = $numberlineitem.attr('identifier');
                arrValues.push(sourcetextIdentifier);
            });

            filters.push({
                id: numberlineResponseId,
                label: numberlineResponseId,
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