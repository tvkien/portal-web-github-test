/**
 * Glosary Module
 * @param  {[type]} function (             [description]
 * @return {[type]}          [description]
 */
var Glossary = (function () {
    /**
     * Glossary mouse enter
     * @return {[type]} [description]
     */
    function handleMouseEnter () {
        var $glossary = $(this);
        var glossaryId = $glossary.attr('glossary_id');
        $(document).find('span.glossary[glossary_id=' + glossaryId + ']').addClass('glossary-hover');
    }

    /**
     * Glossary mouse leave
     * @return {[type]} [description]
     */
    function handleMouseLeave () {
        var $glossary = $(this);
        var glossaryId = $glossary.attr('glossary_id');
        $(document).find('span.glossary[glossary_id=' + glossaryId + ']').removeClass('glossary-hover');
    }

    return {
        handleMouseEnter: handleMouseEnter,
        handleMouseLeave: handleMouseLeave
    }
})();
