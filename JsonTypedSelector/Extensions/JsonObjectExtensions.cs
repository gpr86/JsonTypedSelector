using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace JsonTypedSelector.Extensions
{
    public static class JsonObjectExtensions
    {
        public static JsonNode? SelectNode<T>(this JsonObject jsonObject, Expression<Func<T, object>> selector)
        {
            ArgumentNullException.ThrowIfNull(jsonObject, nameof(jsonObject));
            ArgumentNullException.ThrowIfNull(selector, nameof(selector));

            var nodePath = GetMemberPath(selector.Body);
            JsonNode? currentNode = jsonObject;

            foreach (var segment in nodePath)
            {
                currentNode = segment switch
                {
                    IndexSegment isegment => currentNode[isegment],
                    NameSegment nsegment => currentNode[nsegment],
                    _ => null
                };

                if (currentNode is null) break;
            }

            return currentNode;
        }

        private static List<PathSegment> GetMemberPath(Expression expression)
        {
            Expression expr = expression;
            ImmutableList<PathSegment> path = [];

            while (expr != null)
            {
                (expr, path) = expr switch
                {
                    MemberExpression memberExpr => HandleMemberExpression(memberExpr, path),
                    MethodCallExpression { Method.Name: "get_Item", Arguments.Count: 1 } methodCall => HandleGetItemMethodCallExpression(methodCall, path),
                    UnaryExpression unaryExpr => HandleUnaryExpression(unaryExpr, path),
                    _ => (null, path)
                };
            }

            return path.Reverse().ToList();
        }

        private static (Expression? expression, ImmutableList<PathSegment> path) HandleMemberExpression(MemberExpression memberExpr, ImmutableList<PathSegment> path)
            => (memberExpr.Expression, path.Add(new NameSegment(GetJsonName(memberExpr.Member))));

        private static (Expression? expression, ImmutableList<PathSegment> path) HandleUnaryExpression(UnaryExpression unaryExpr, ImmutableList<PathSegment> path)
            => (unaryExpr.Operand, path);

        private static (Expression? expression, ImmutableList<PathSegment> path) HandleGetItemMethodCallExpression(MethodCallExpression methodCall, ImmutableList<PathSegment> path)
        {
            var value = (GetValueFromExpression(methodCall.Arguments[0]) is int i
                ? i
                : throw new ArgumentException("Only integer array indexes are supported.", nameof(methodCall)));

            return (methodCall.Object, path.Add(new IndexSegment(i)));
        }

        private static string GetJsonName(MemberInfo memberInfo)
            => memberInfo.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? memberInfo.Name;

        private static object? GetValueFromExpression(Expression expr)
            => expr switch
            {
                ConstantExpression constExpr => constExpr.Value,
                _ => Expression.Lambda(expr).Compile().DynamicInvoke()
            };

        private abstract record PathSegment;

        private record IndexSegment(int Value) : PathSegment
        {
            public static implicit operator int(IndexSegment segment) => segment.Value;
        };

        private record NameSegment(string Value) : PathSegment
        {
            public static implicit operator string(NameSegment segment) => segment.Value;
        };
    }
}
