using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace PoweredSoft.DynamicLinq.Helpers
{
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

            int step = 0;
            pieces.ForEach(piece =>
            {
                var expressionPiece = new ExpressionPiece(currentGroup, parentPiece);
                expressionPiece.Resolve(piece);
                currentGroup.AddSubPart(expressionPiece);

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
    }

    public class ExpressionPathPart
    {
        public Expression ParentExpression { get; set; }
        public ParameterExpression ParameterExpression { get; set; }
        public Expression PartExpression { get; set; }

        public bool IsNullable() => TypeHelpers.IsNullable(PartExpression.Type);
   
        public bool IsGenericEnumerable() => QueryableHelpers.IsGenericEnumerable(PartExpression);
        public Type GenericEnumerableType() => PartExpression.Type.GenericTypeArguments.First();
        public bool IsCallingMethod() => PartExpression is MethodCallExpression;
        public Type GetToListType() => IsGenericEnumerable() ? GenericEnumerableType() : PartExpression.Type;
        public bool IsParentGenericEnumerable() => QueryableHelpers.IsGenericEnumerable(ParentExpression);

        public static List<ExpressionPathPart> Split(ParameterExpression param, string path)
        {
            var ret = new List<ExpressionPathPart>();

            var parts = path.Split('.').ToList();
            Expression parent = param;
            parts.ForEach(part =>
            {
                var p = new ExpressionPathPart();
                p.PartExpression = Expression.PropertyOrField(parent, part);
                p.ParentExpression = parent;
                p.ParameterExpression = param;
                ret.Add(p);

                if (p.IsGenericEnumerable())
                {
                    param = Expression.Parameter(p.GenericEnumerableType());
                    parent = param;
                }
                else
                {
                    parent = p.PartExpression;
                }
            });

            return ret;
        }

        public LambdaExpression GetLambdaExpression()
        {
            var lambda = Expression.Lambda(PartExpression, ParameterExpression);
            return lambda;
        }

        public Type ParentGenericEnumerableType() => ParentExpression.Type.GenericTypeArguments.First();

    }
}
