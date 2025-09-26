using System.Collections.Immutable;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace JsonTypedSelector.Extensions
{
    public static class JsonObjectExtensions
    {
        public static JsonNode? SelectNode<TRoot>(this JsonNode root, Expression<Func<TRoot, object>> selector) =>
            SelectNode<TRoot>(root, GetMemberPath(selector.Body));

        public static JsonNode? SelectNode<TRoot>(this JsonNode root, List<OneOf<string, int>> path)
        {
            ArgumentNullException.ThrowIfNull(root, nameof(root));
            ArgumentNullException.ThrowIfNull(path, nameof(path));

            if(root is null) return null;

            JsonNode? currentNode = root;

            foreach (var segment in path)
            {
                currentNode = segment.IsLeft
                    ? currentNode[segment.Left]
                    : currentNode[segment.Right];

                if (currentNode is null) break;
            }

            return currentNode;
        }

        public static void SetNodeValue<TRoot>(this JsonNode root, Expression<Func<TRoot, object>> selector, object value)
        {
            var path = GetMemberPath(selector.Body);

            JsonNode? current = root;

            foreach (var segment in path[0..^1])
            {
                var next = segment.IsLeft switch
                {
                    true => (current as JsonObject)?[(string)segment],
                    false => (current as JsonArray)?[(int)segment],
                };

                current = next;
            }

            var lastSegment = path[^1];
            var jsonValue = JsonSerializer.SerializeToNode(value);

            _ = lastSegment.IsLeft switch
            {
                true => (current as JsonObject)[(string)lastSegment] = jsonValue,
                false => (current as JsonArray)[(int)lastSegment] = jsonValue,
            };
        }

        private static List<OneOf<string, int>> GetMemberPath(Expression expression)
        {
            ArgumentNullException.ThrowIfNull(expression);

            var expr = expression;
            var path = ImmutableList<OneOf<string, int>>.Empty;

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

        private static (Expression? expression, ImmutableList<OneOf<string, int>> path) HandleMemberExpression(
            MemberExpression memberExpr,
            ImmutableList<OneOf<string, int>> path) =>
            (memberExpr.Expression, path.Add(GetJsonName(memberExpr.Member)));

        private static (Expression? expression, ImmutableList<OneOf<string, int>> path) HandleUnaryExpression(
            UnaryExpression unaryExpr,
            ImmutableList<OneOf<string, int>> path) =>
            (unaryExpr.Operand, path);

        private static (Expression? expression, ImmutableList<OneOf<string, int>> path) HandleGetItemMethodCallExpression(
            MethodCallExpression methodCall,
            ImmutableList<OneOf<string, int>> path) =>
            GetValueFromExpression(methodCall.Arguments[0]) switch
            { 
                int i => (methodCall.Object, path.Add(i)),
                _ => throw new InvalidOperationException("Only integer array indexes are supported.")
            };

        private static string GetJsonName(MemberInfo memberInfo) =>
            memberInfo.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name ?? memberInfo.Name;

        private static object? GetValueFromExpression(Expression expr) =>
            expr switch
            {
                ConstantExpression constExpr => constExpr.Value,
                _ => Expression.Lambda(expr).Compile().DynamicInvoke()
            };
    }
}
