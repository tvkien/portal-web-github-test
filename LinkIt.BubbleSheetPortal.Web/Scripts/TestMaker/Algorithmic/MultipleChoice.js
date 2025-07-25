var MultipleChoiceAlgorithmic = (function () {

    function getFilters (xmlContent) {
        var filters = [];
        var $container = $('<div/>');
        $container.html(xmlContent);
        var isMultipleChoice = xmlContent.indexOf('cardinality="multiple"') != -1 ? true : false;

        $container.find('choiceInteraction').each(function () {
            var $choiceinteraction = $(this);
            var choiceinteractionResponseId = $choiceinteraction.attr('responseIdentifier');
            var simpleChoiceValues = [];

            $choiceinteraction.find('simpleChoice').each(function () {
                var $simplechoice = $(this);
                var simplechoiceIdentifier = $simplechoice.attr('identifier');
                simpleChoiceValues.push(simplechoiceIdentifier);
            });

            filters.push({
                id: choiceinteractionResponseId,
                label: choiceinteractionResponseId,
                type: 'string',
                input: 'select',
                values: simpleChoiceValues,
                multiple: isMultipleChoice
            });
        });

        return filters;
    }

    function getQueryBuilder (xmlContent) {
        var isMultipleChoice = xmlContent.indexOf('cardinality="multiple"') != -1 ? true : false;

        var filters = getFilters(xmlContent);
        var options = {
            plugins: [
              'sortable',
              'atleast',
              'bt-tooltip-errors'
            ],
            operators: isMultipleChoice === true ? ['in'] : ['equal'],
            conditions: ['AND', 'OR', 'ATLEAST'],
            filters: filters,
        };

        return options;
    }

    return {
        getQueryBuilder: getQueryBuilder
    }

})();