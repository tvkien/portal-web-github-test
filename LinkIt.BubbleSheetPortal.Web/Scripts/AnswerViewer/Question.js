/**
 * Question Module
 * @param  {[type]} function (             [description]
 * @return {[type]}          [description]
 */
var Question = (function () {
    /**
     * Display xml to html
     * @param  {[type]} xmlContent [description]
     * @return {[type]}            [description]
     */
    function displayHtml (xmlContent) {
        xmlContent = MultipleChoice.displayHtml(xmlContent);
        xmlContent = MultipleChoiceVariable.displayHtml(xmlContent);
        xmlContent = TextEntry.displayHtml(xmlContent);
        xmlContent = InlineChoice.displayHtml(xmlContent);
        xmlContent = ExtendedText.displayHtml(xmlContent);
        xmlContent = DrawInteraction.displayHtml(xmlContent);
        xmlContent = DragDropStandard.displayHtml(xmlContent);
        xmlContent = TextHotSpot.displayHtml(xmlContent);
        xmlContent = ImageHotSpot.displayHtml(xmlContent);
        xmlContent = TableHotSpot.displayHtml(xmlContent);
        xmlContent = NumberLineHotSpot.displayHtml(xmlContent);
        xmlContent = DragDropSequence.displayHtml(xmlContent);
        xmlContent = Video.displayHtml(xmlContent);

        return xmlContent;
    }

    /**
     * Update answer of student
     * @param  {[type]}  question           [description]
     * @param  {[type]}  originalXmlContent [description]
     * @param  {Boolean} isCorrect          [description]
     * @return {[type]}                     [description]
     */
    function updateAnswerHtml (question, originalXmlContent, isCorrect) {
        var xmlContent = originalXmlContent;
        xmlContent = MultipleChoice.updateAnswerHtml(question, xmlContent);
        xmlContent = MultipleChoiceVariable.updateAnswerHtml(question, xmlContent);
        xmlContent = InlineChoice.updateAnswerHtml(question, xmlContent);
        xmlContent = TextEntry.updateAnswerHtml(question, xmlContent);
        xmlContent = ExtendedText.updateAnswerHtml(question, xmlContent);
        xmlContent = DrawInteraction.updateAnswerHtml(question, xmlContent);
        xmlContent = DragDropStandard.updateAnswerHtml(question, xmlContent, isCorrect);
        xmlContent = TextHotSpot.updateAnswerHtml(question, xmlContent, isCorrect);
        xmlContent = ImageHotSpot.updateAnswerHtml(question, xmlContent, isCorrect);
        xmlContent = TableHotSpot.updateAnswerHtml(question, xmlContent, isCorrect);
        xmlContent = NumberLineHotSpot.updateAnswerHtml(question, xmlContent, isCorrect);
        xmlContent = DragDropSequence.updateAnswerHtml(question, xmlContent, isCorrect);

        return xmlContent;
    }

    /**
     * Display icon guidance/rationale
     * @param  {[type]} xmlContent [description]
     * @return {[type]}            [description]
     */
    function displayIconGuidanceRationale (xmlContent) {
        xmlContent = GuidanceRationale.displayIconGrMultipleChoice(xmlContent);
        xmlContent = GuidanceRationale.displayIconGrInlineChoice(xmlContent);
        xmlContent = GuidanceRationale.displayIconGrTextEntry(xmlContent);

        return xmlContent;
    }

    function getInformationalOnly (xmlContent) {
        var $question = $('<div/>');
        var isInformationalOnly = false;

        $question.html(xmlContent);

        if ($question.find('responseDeclaration').attr('method') === 'informational-only') {
            isInformationalOnly = true; 
        }

        return isInformationalOnly;
    }

    return {
        displayHtml: displayHtml,
        updateAnswerHtml: updateAnswerHtml,
        displayIconGuidanceRationale: displayIconGuidanceRationale,
        getInformationalOnly: getInformationalOnly
    }
})();
