using PoweredSoft.DynamicLinq.Dal.Pocos;
using PoweredSoft.DynamicLinq.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

#if false

namespace PoweredSoft.DynamicLinq.ConsoleApp
{
    public class StaticProto1
    {
        public static void Run()
        {
            var q = new List<Author>().AsQueryable();
            q.Select(t => new
            {
                Ids = t.Posts.SelectMany(t2 => t2.Comments.Select(t3 => t3.CommentLikes))
            });

            var expressionParser = new ExpressionParser(typeof(Author), "Posts.Comments.CommentLikes");
            expressionParser.Parse();

            var finalExpression = CreateSelectExpressionFromParsed(expressionParser, SelectCollectionHandling.Flatten, SelectNullHandling.New);
        }

        public static Expression CreateSelectExpressionFromParsed(ExpressionParser expressionParser,
           SelectCollectionHandling selectCollectionHandling = SelectCollectionHandling.LeaveAsIs,
           SelectNullHandling nullChecking = SelectNullHandling.LeaveAsIs)
        {
            Type nullHandlingRightType = null;
            Expression nullHandlingNullValue = null;

            if (nullChecking != SelectNullHandling.LeaveAsIs)
            {
                var withoutNullCheckResult = CreateSelectExpressionFromParsed(expressionParser, SelectCollectionHandling.Flatten, SelectNullHandling.LeaveAsIs);
                if (QueryableHelpers.IsGenericEnumerable(withoutNullCheckResult.Type))
                    nullHandlingRightType = typeof(List<>).MakeGenericType(withoutNullCheckResult.Type.GenericTypeArguments.First());
                else
                    nullHandlingRightType = withoutNullCheckResult.Type;

                nullHandlingNullValue = nullChecking == SelectNullHandling.Default
                    ? Expression.Default(nullHandlingRightType) as Expression
                    : Expression.New(nullHandlingRightType) as Expression;
            }

            // reversed :)
            var reversedCopy = expressionParser.Groups.Select(t => t).ToList();
            reversedCopy.Reverse();

            MethodCallExpression lastSelectExpression = null;
            Expression ret = null;

            foreach (var t in reversedCopy)
            {
                if (true == t.Parent?.LastPiece().IsGenericEnumerable())
                {
                    if (lastSelectExpression == null)
                        lastSelectExpression = RegroupSelectExpressions(t, selectCollectionHandling);
                    else
                        lastSelectExpression = RegroupSelectExpressions(t, lastSelectExpression, selectCollectionHandling);
                }
            }

            ret = lastSelectExpression;
            return ret;
        }

        public static MethodCallExpression RegroupSelectExpressions(ExpressionParameterGroup group, SelectCollectionHandling selectCollectionHandling)
        {
            MethodCallExpression ret = null;
            var lambda = group.CreateLambda();
            var parentExpression = group.Parent.LastExpression();
            var isParentExpressionGenericEnumerable = QueryableHelpers.IsGenericEnumerable(parentExpression);
            var parentExpressionGenericEnumerableType = isParentExpressionGenericEnumerable ? parentExpression.Type.GenericTypeArguments.First() : null;
            var lastPiece = group.LastPiece();

            if (selectCollectionHandling == SelectCollectionHandling.LeaveAsIs || !lastPiece.IsGenericEnumerable())
                ret = Expression.Call(typeof(Enumerable), "Select", new Type[] { parentExpressionGenericEnumerableType, lastPiece.Expression.Type }, parentExpression, lambda);
            else
                ret = Expression.Call(typeof(Enumerable), "SelectMany", new Type[] { parentExpressionGenericEnumerableType, lastPiece.GetGenericEnumerableType() }, parentExpression, lambda);

            return ret;
        }

        public static MethodCallExpression RegroupSelectExpressions(ExpressionParameterGroup group, MethodCallExpression innerSelect, SelectCollectionHandling selectCollectionHandling)
        {
            var parent = group.Parent;
            var parentLastPiece = parent.LastPiece();

            MethodCallExpression ret = null;
            var lambda = Expression.Lambda(innerSelect, group.Parameter);
            if (selectCollectionHandling == SelectCollectionHandling.LeaveAsIs)
                ret = Expression.Call(typeof(Enumerable), "Select", new Type[] { parentLastPiece.GetGenericEnumerableType(), innerSelect.Type }, parentLastPiece.Expression, lambda);
            else
                ret = Expression.Call(typeof(Enumerable), "SelectMany", new Type[] { parentLastPiece.GetGenericEnumerableType(), innerSelect.Type.GenericTypeArguments.First() }, parentLastPiece.Expression, lambda);

            return ret;
        }
    }


    public class ExpressionParameterGroup
    {
        public ExpressionParameterGroup Parent { get; set; }
        public ParameterExpression Parameter { get; set; }
        public List<ExpressionPiece> Pieces { get; set; } = new List<ExpressionPiece>();

        public ExpressionParameterGroup(ParameterExpression parameter)
        {
            Parameter = parameter;
        }

        public void AddSubPart(ExpressionPiece expressionPart)
        {
            Pieces.Add(expressionPart);
        }

        public Expression LastExpression() => LastPiece().Expression;

        public ExpressionPiece LastPiece() => Pieces.Last();

        public LambdaExpression CreateLambda()
        {
            var lastExpression = LastPiece().Expression;
            var lambda = Expression.Lambda(lastExpression, Parameter);
            return lambda;
        }
    }

    public class ExpressionPiece
    {
        public ExpressionParameterGroup Parameter { get; set; }
        public ExpressionPiece Parent { get; set; }
        public Expression Expression { get; set; }

        public ExpressionPiece(ExpressionParameterGroup parameter, ExpressionPiece parent = null)
        {
            Parameter = parameter;
            Parent = parent;
        }

        public void Resolve(string piece)
        {
            Expression = Expression.PropertyOrField(Parent?.Expression ?? Parameter.Parameter, piece);
        }

        public bool IsGenericEnumerable() => QueryableHelpers.IsGenericEnumerable(Expression);
        public Type GetGenericEnumerableType() => Expression.Type.GenericTypeArguments.First();
    }

    public class ExpressionParser
    {
        public ParameterExpression Parameter { get; protected set; }
        public string Path { get; protected set; }
        public List<ExpressionParameterGroup> Groups { get; set; } = new List<ExpressionParameterGroup>();

        public ExpressionParser(Type type, string path) : this(Expression.Parameter(type), path)
        {

        }

        public ExpressionParser(ParameterExpression parameter, string path)
        {
            if (parameter == null)
                throw new ArgumentNullException("parameter");

            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            Parameter = parameter;
            Path = path;
        }

        public void Parse()
        {
            Groups = new List<ExpressionParameterGroup>();
            var pieces = Path.Split('.').ToList();

            // create the current parameter.
            var currentGroup = CreateAndAddParameterGroup(Parameter);
            ExpressionPiece parentPiece = null;

            int indexOfPiece = -1;
            pieces.ForEach(piece =>
            {
                ++indexOfPiece;
                bool isLast = indexOfPiece == pieces.Count - 1;

                var expressionPiece = new ExpressionPiece(currentGroup, parentPiece);
                expressionPiece.Resolve(piece);
                currentGroup.AddSubPart(expressionPiece);

                // rest is only if its not the last piece.
                if (isLast) return;

                if (expressionPiece.IsGenericEnumerable())
                {
                    var param = Expression.Parameter(expressionPiece.GetGenericEnumerableType());
                    currentGroup = CreateAndAddParameterGroup(param, currentGroup);
                    parentPiece = null;
                }
                else
                {
                    parentPiece = expressionPiece;
                }
            });
        }

        public ExpressionParameterGroup CreateAndAddParameterGroup(ParameterExpression parameter, ExpressionParameterGroup parent = null)
        {
            var group = new ExpressionParameterGroup(parameter);

            if (parent != null)
                group.Parent = parent;

            Groups.Add(group);
            return group;
        }

        public ExpressionParameterGroup LastGroup()
        {
            return this.Groups.Last();
        }
    }


#endif