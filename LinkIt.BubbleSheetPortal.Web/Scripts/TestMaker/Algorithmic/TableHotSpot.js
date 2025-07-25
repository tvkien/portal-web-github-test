var TableHotSpotAlgorithmic = (function () {
    
    function getFilters (xmlContent) {
        var filters = [];
        var $container = $('<div/>');
        $container.html(xmlContent);

        $container.find('responseDeclaration').each(function () {
            var $tablehotspot = $(this);
            var tablehotspotResponseId = $tablehotspot.attr('identifier');
            var arrValues = [];

            $container.find('tableitem,span[typehotspot]').each(function () {
                var $tableitem = $(this);
                var sourcetextIdentifier = $tableitem.attr('identifier');
                arrValues.push(sourcetextIdentifier);
            });

            filters.push({
                id: tablehotspotResponseId,
                label: tablehotspotResponseId,
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