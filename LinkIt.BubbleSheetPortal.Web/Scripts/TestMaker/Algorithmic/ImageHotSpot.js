var ImageHotSpotAlgorithmic = (function () {
    
    function getFilters (xmlContent) {
        var filters = [];
        var $container = $('<div/>');
        $container.html(xmlContent);

        $container.find('imageHotSpot').each(function () {
            var $imagehotspot = $(this);
            var imagehotspotResponseId = $imagehotspot.attr('responseIdentifier');
            var arrValues = [];

            $container.find('sourceItem').each(function () {
                var $sourceitem = $(this);
                var sourceitemIdentifier = $sourceitem.attr('identifier');
                arrValues.push(sourceitemIdentifier);
            });

            filters.push({
                id: imagehotspotResponseId,
                label: imagehotspotResponseId,
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