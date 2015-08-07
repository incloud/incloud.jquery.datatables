using System;
using System.Linq;
using System.Linq.Expressions;
using QueryInterceptor;

namespace Incloud.JQuery.Datatables
{
    public static class CaseInsensitiveInterceptor
    {
        public class ReplacingVisitor : ExpressionVisitor
        {
            readonly Func<Expression, bool> _match;
            readonly Func<Expression, Expression> _createReplacement;
            public ReplacingVisitor(Expression from, Expression to)
            {
                _match = node => from == node;
                _createReplacement = node => to;
            }

            public override Expression Visit(Expression node)
            {
                if (_match(node)) return _createReplacement(node);
                return base.Visit(node);
            }
        }

        public class SetComparerExpressionVisitor : ExpressionVisitor
        {
            readonly StringComparison _comparer;
            public SetComparerExpressionVisitor(StringComparison comparer)
            {
                _comparer = comparer;
            }

            protected override Expression VisitBinary(BinaryExpression node)
            {
                if (node.Left.Type == typeof(string) && node.Right.Type == typeof(string))
                {
                    if (node.NodeType == ExpressionType.Equal)
                    {

                        var exp = ((LambdaExpression)MakeExpression(s => s.Equals("asdf", _comparer))).Body;
                        exp = new ReplacingVisitor(((dynamic)exp).Arguments[0], ((dynamic)node).Left).Visit(exp);
                        exp = new ReplacingVisitor(((dynamic)exp).Object, ((dynamic)node).Right).Visit(exp);
                        return exp;
                    }
                    if (node.NodeType == ExpressionType.NotEqual)
                    {
                        var exp = ((LambdaExpression)MakeExpression(s => !s.Equals("asdf", _comparer))).Body;
                        exp = new ReplacingVisitor(((dynamic)exp).Operand.Arguments[0], ((dynamic)node).Left).Visit(exp);
                        exp = new ReplacingVisitor(((dynamic)exp).Operand.Object, ((dynamic)node).Right).Visit(exp);
                        return exp;
                    }
                }
                return base.VisitBinary(node);
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (node.Method.DeclaringType == typeof(string))
                {
                    if (node.Method.Name == "Contains")
                    {
                        var exp = ((LambdaExpression)MakeExpression(s => s.IndexOf("asdf", _comparer) > -1)).Body;
                        exp =
                            new ReplacingVisitor(((dynamic)exp).Left.Arguments[0], ((dynamic)node).Arguments[0]).Visit(exp);
                        exp = new ReplacingVisitor(((dynamic)exp).Left.Object, ((dynamic)node).Object).Visit(exp);
                        return exp;
                    }
                    if (node.Method.Name == "StartsWith")
                    {
                        var exp = ((LambdaExpression)MakeExpression(s => s.IndexOf("asdf", _comparer) == 0)).Body;
                        exp =
                            new ReplacingVisitor(((dynamic)exp).Left.Arguments[0], ((dynamic)node).Arguments[0]).Visit(exp);
                        exp = new ReplacingVisitor(((dynamic)exp).Left.Object, ((dynamic)node).Object).Visit(exp);
                        return exp;
                    }
                    if (node.Method.Name == "EndsWith")
                    {
                        var exp = ((LambdaExpression)MakeExpression(s => s.EndsWith("asdf", _comparer))).Body;
                        exp = new ReplacingVisitor(((dynamic)exp).Arguments[0], ((dynamic)node).Arguments[0]).Visit(exp);
                        exp = new ReplacingVisitor(((dynamic)exp).Object, ((dynamic)node).Object).Visit(exp);
                        return exp;
                    }
                    if (node.Method.Name == "Equals")
                    {
                        var exp = ((LambdaExpression)MakeExpression(s => s.Equals("asdf", _comparer))).Body;
                        exp = new ReplacingVisitor(((dynamic)exp).Arguments[0], ((dynamic)node).Arguments[0]).Visit(exp);
                        exp = new ReplacingVisitor(((dynamic)exp).Object, ((dynamic)node).Object).Visit(exp);
                        return exp;
                    }
                }

                return base.VisitMethodCall(node);
            }


            public Expression MakeExpression(Expression<Func<string, bool>> exp)
            {
                return exp;
            }
        }

        public static IQueryable<T> SetComparer<T>(this IQueryable<T> q, StringComparison sc)
        {
            return q
                .InterceptWith(new SetComparerExpressionVisitor(sc));
        }
    }
}