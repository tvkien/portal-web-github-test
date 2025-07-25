/**
 * Multipart Module
 * @param  {[type]} function (             [description]
 * @return {[type]}          [description]
 */
var Multipart = (function () {

    /**
     * Get answer subs correct
     * @param  {[type]} xmlContent [description]
     * @return {[type]}            [description]
     */
    function getCorrectAnswerSubs (xmlContent) {
        var answerSubsCorrect = [];

        var $tree = $('<div/>');

        $tree.append(xmlContent);

        $tree.find('responsedeclaration').each(function (ind, response) {
            var $response = $(response);
            var obj = {
                ResponseIdentifier: '',
                AnswerChoice: [],
                AnswerText: []
            };

            obj.ResponseIdentifier = $response.attr('identifier');

            $response.find('correctresponse > value').each(function (index, value) {
                var $value = $(value);
                obj.AnswerChoice.push($value.text());
                obj.AnswerText.push($value.text());
            });

            if (obj.AnswerChoice.length) {
                obj.AnswerChoice = obj.AnswerChoice.join(',');
            } else {
                obj.AnswerChoice = '';
            }

            if (obj.AnswerText.length) {
                obj.AnswerText = obj.AnswerText.join(',');
            } else {
                obj.AnswerText = '';
            }

            answerSubsCorrect.push(obj);
        });

        return answerSubsCorrect;
    }

    return {
        getCorrectAnswerSubs: getCorrectAnswerSubs
    }
})();
