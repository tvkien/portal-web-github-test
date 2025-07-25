﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace LinkIt.BubbleSheetPortal.DynamoConnector.Common
{
    public class PropertyPathVisitor : ExpressionVisitor
    {
        private Stack<string> _stack;

        public string GetPropertyPath(Expression expression)
        {
            _stack = new Stack<string>();
            Visit(expression);
            return _stack
                .Aggregate(
                    new StringBuilder(),
                    (sb, name) =>
                        (sb.Length > 0 ? sb.Append(".") : sb).Append(name))
                .ToString();
        }

        protected override Expression VisitMember(MemberExpression expression)
        {
            if (_stack != null)
                _stack.Push(expression.Member.Name);
            return base.VisitMember(expression);
        }

        protected override Expression VisitMethodCall(MethodCallExpression expression)
        {
            if (IsLinqOperator(expression.Method))
            {
                for (var i = 1; i < expression.Arguments.Count; i++)
                {
                    Visit(expression.Arguments[i]);
                }
                Visit(expression.Arguments[0]);
                return expression;
            }
            return base.VisitMethodCall(expression);
        }

        private static bool IsLinqOperator(MethodInfo method)
        {
            if (method == null) throw new ArgumentNullException("method");
            if (method.DeclaringType != typeof(Queryable) && method.DeclaringType != typeof(Enumerable))
                return false;
            return Attribute.GetCustomAttribute(method, typeof(ExtensionAttribute)) != null;
        }
    }
}
