var TestMakerUtils = (function () {
    function minutesForExpireFormat (seconds) {
      var mm = Math.floor(seconds / 60);
        var ss = minutesForExpire - (mm * 60);
        var mmResult = '';
        var ssResult = '';
        var result;

        // Round seconds
        ss = Math.round(ss * 100) / 100;

        if (mm > 0) {
            mmResult = mm < 10 ? mm + ' minute' : mm + ' minutes'
        }

        ssResult = ss < 10 ? ss + ' second' : ss + ' seconds';

        if (mmResult !== '') {
            result = mmResult + ' ' + ssResult;
        } else {
            result = ssResult;
        }

        return result;
    }

    function getTodayBySeconds () {
        return new Date().getTime() / 1000 | 0;
    }

    function getGroupQuestionType (qtiSchemaId) {
        if([1, 3, 8, 37].indexOf(qtiSchemaId) != -1) {
            return 2;
        } else if(qtiSchemaId === 9) {
            return 3;
        } else if([30, 35].indexOf(qtiSchemaId) != -1) {
            return 4;
        } else if([31, 32, 33, 34].indexOf(qtiSchemaId) != -1) {
            return 5;
        } else if(qtiSchemaId === 36) {
            return 6;
        } else {
            return "";
        }
    }

    function mappingOperator (operator, qtiSchemaId) {
        var result = "";
        if (qtiSchemaId === 9) {
            if(operator === "equal") {
                result = "=";
            } else if(operator === "less") {
                result = "<";
            } else if(operator === "less_or_equal") {
                result = "<=";
            } else if(operator === "greater") {
                result = ">";
            } else if(operator === "greater_or_equal") {
                result = ">=";
            }
        }
        return result;
    }

    function convertExpressionAlgorithmic (expression, qtiSchemaId) {
        var result = '';
        var expressionLength = expression.rules.length;
        if(expression.condition === 'ATLEAST') {
            var n = 1;
            if(expression.atleast > 1) {
                n = expression.atleast;
            }
            result += ' ATLEAST_'+ n +'[';
        }
        for(var i = 0; i < expressionLength; i++) {
            rule = expression.rules[i];
            if(i === expressionLength - 1) {
                if(rule.rules) {
                    var subResult = convertExpressionAlgorithmic(rule, qtiSchemaId);
                    if(expression.condition === 'ATLEAST') {
                        if(subResult.includes('ATLEAST')) {
                            result += subResult + ']';
                        } else {
                            result += '(' + subResult + ')]';
                        }
                    } else {
                        if(subResult.includes('ATLEAST')) {
                            result += subResult;
                        } else {
                            result += '(' + subResult + ')' + ' ';
                        }
                    }
                } else {
                    var value = rule.value;
                    if (qtiSchemaId == 9 && value == 'Anything else') {
                        value = '*';
                    }
                    if(expression.condition === 'ATLEAST') {
                        result += '{'+ rule.field + ';' + getGroupQuestionType(qtiSchemaId) + ';' + mappingOperator(rule.operator, qtiSchemaId) + value + '}]';
                    } else {
                        result += '{'+ rule.field + ';' + getGroupQuestionType(qtiSchemaId) + ';' + mappingOperator(rule.operator, qtiSchemaId) + value + '}';
                    }
                }
            } else {
                if(rule.rules) {
                    var subResult = convertExpressionAlgorithmic(rule, qtiSchemaId);
                    if(expression.condition === 'ATLEAST') {
                        if(subResult.includes('ATLEAST')) {
                            result += subResult + ';'
                        } else {
                            result += '(' + subResult + ');';
                        }
                    } else {
                        if(subResult.includes('ATLEAST')) {
                            result += subResult;
                        } else {
                            result += '(' + subResult + ') ' + expression.condition  + ' ';
                        }
                    }
                } else {
                    var value = rule.value;
                    if (qtiSchemaId == 9 && value == 'Anything else') {
                        value = '*';
                    }
                    if(expression.condition === 'ATLEAST') {
                        result += '{' + rule.field + ';' + getGroupQuestionType(qtiSchemaId) + ';' + mappingOperator(rule.operator, qtiSchemaId) + value + '};';
                    } else {
                        result += '{' + rule.field + ';' + getGroupQuestionType(qtiSchemaId) + ';' + mappingOperator(rule.operator, qtiSchemaId) + value + '} ' + expression.condition + ' ';
                    }
                }
            }
        }

        return result;
    }

    function convertMultiPartExpression(expression, qtiSchemaId) {
        var result = '';
        var expressionLength = expression.rules.length;

        for (var i = 0; i < expressionLength; i++) {
            rule = expression.rules[i];
            if (i === expressionLength - 1) {
                if (rule.rules) {
                    var subResult = convertMultiPartExpression(rule, qtiSchemaId);
                    result += '{' + subResult + '}';
                } else {
                    var value = rule.value;
                    result += '{' + rule.field + ';' + mappingOperator(rule.operator, qtiSchemaId) + value + '}';
                }
            } else {
                if (rule.rules) {
                    var subResult = convertMultiPartExpression(rule, qtiSchemaId);
                    result += '{' + subResult + '} ' + expression.condition;
                } else {
                    var value = rule.value;
                    result += '{' + rule.field + ';' + mappingOperator(rule.operator, qtiSchemaId) + value + '} ' + expression.condition + ' ';
                }
            }
        }
        return result;
    }

    return {
        minutesForExpireFormat: minutesForExpireFormat,
        getTodayBySeconds: getTodayBySeconds,
        convertExpressionAlgorithmic: convertExpressionAlgorithmic,
        convertMultiPartExpression: convertMultiPartExpression
    };
})();
