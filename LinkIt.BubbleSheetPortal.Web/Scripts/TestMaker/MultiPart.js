var MultiPart = (function () {

    function getFilters (xmlContent) {
        var filters = [];
        var $container = $('<div/>');
        $container.html(xmlContent);
        var isMultipleChoice = xmlContent.indexOf('cardinality="multiple"') !== -1 ? true : false;

        // multiple choice
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

        // inline choice
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

        // text entry
        var isRange = xmlContent.indexOf('range="true"') !== -1 ? true : false;

        $container.find('textEntryInteraction').each(function () {
            var $textentryinteraction = $(this);
            var textentryinteractionResponseId = $textentryinteraction.attr('responseIdentifier');
            var textentryValues = [];

            $container.find('responseDeclaration correctResponse value').each(function () {
                var $value = $(this);
                var valueIdentifier = $value.html();
                textentryValues.push(valueIdentifier);
            });

            if (isRange) {
                filters.push({
                    id: textentryinteractionResponseId,
                    label: textentryinteractionResponseId,
                    type: 'string',
                    input: 'select',
                    values: ['Answered', 'Unanswered']
                });
            } else {
                textentryValues.push('Anything else');
                filters.push({
                    id: textentryinteractionResponseId,
                    label: textentryinteractionResponseId,
                    type: 'string',
                    input: 'select',
                    values: ['Answered', 'Unanswered']
                });
            }
        });

        // Extended text
        $container.find('extendedTextInteraction').each(function () {
            var $extendedTextInteraction = $(this);
            var extendedTextInteractionResponseId = $extendedTextInteraction.attr('responseIdentifier');

            filters.push({
                id: extendedTextInteractionResponseId,
                label: extendedTextInteractionResponseId,
                type: 'string',
                input: 'select',
                values: ['Answered', 'Unanswered']
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
            conditions: ['AND', 'OR'],
            filters: filters
        };

        return options;
    }

    return {
        getQueryBuilder: getQueryBuilder
    };

})();
