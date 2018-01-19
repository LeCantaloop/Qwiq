using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Microsoft.Qwiq.Linq.Visitors
{
    /// <summary>
    ///     Enables the partial evaluation of queries.
    /// </summary>
    /// <remarks>
    ///     From http://msdn.microsoft.com/en-us/library/bb546158.aspx
    ///     Copyright notice http://msdn.microsoft.com/en-gb/cc300389.aspx#O
    /// </remarks>
    public class PartialEvaluator : ExpressionVisitor
    {
        /// <summary>
        ///     Performs evaluation and replacement of independent sub-trees
        /// </summary>
        /// <param name="expression">The root of the expression tree.</param>
        /// <param name="fnCanBeEvaluated">
        ///     A function that decides whether a given expression node can be part of the local
        ///     function.
        /// </param>
        /// <returns>A new tree with sub-trees evaluated and replaced.</returns>
        public Expression Visit(Expression expression, Func<Expression, bool> fnCanBeEvaluated)
        {
            return new SubtreeEvaluator(new Nominator(fnCanBeEvaluated).Nominate(expression)).Eval(expression);
        }

        /// <summary>
        ///     Performs evaluation and replacement of independent sub-trees
        /// </summary>
        /// <param name="node">The root of the expression tree.</param>
        /// <returns>A new tree with sub-trees evaluated and replaced.</returns>
        public override Expression Visit(Expression node)
        {
            return Visit(node, CanBeEvaluatedLocally);
        }

        private static bool CanBeEvaluatedLocally(Expression expression)
        {
            return expression.NodeType != ExpressionType.Parameter;
        }

        /// <summary>
        ///     Performs bottom-up analysis to determine which nodes can possibly
        ///     be part of an evaluated sub-tree.
        /// </summary>
        private class Nominator : ExpressionVisitor
        {
            private HashSet<Expression> candidates;

            private bool cannotBeEvaluated;

            private readonly Func<Expression, bool> fnCanBeEvaluated;

            internal Nominator(Func<Expression, bool> fnCanBeEvaluated)
            {
                this.fnCanBeEvaluated = fnCanBeEvaluated;
            }

            public override Expression Visit(Expression node)
            {
                if (node != null)
                {
                    var saveCannotBeEvaluated = cannotBeEvaluated;
                    cannotBeEvaluated = false;
                    base.Visit(node);
                    if (!cannotBeEvaluated)
                        if (fnCanBeEvaluated(node)) candidates.Add(node);
                        else cannotBeEvaluated = true;
                    cannotBeEvaluated |= saveCannotBeEvaluated;
                }
                return node;
            }

            internal HashSet<Expression> Nominate(Expression expression)
            {
                candidates = new HashSet<Expression>();
                Visit(expression);
                return candidates;
            }
        }

        /// <summary>
        ///     Evaluates and replaces sub-trees when first candidate is reached (top-down)
        /// </summary>
        private class SubtreeEvaluator : ExpressionVisitor
        {
            private readonly HashSet<Expression> candidates;

            internal SubtreeEvaluator(HashSet<Expression> candidates)
            {
                this.candidates = candidates;
            }

            public override Expression Visit(Expression node)
            {
                if (node == null) return null;
                if (candidates.Contains(node)) return Evaluate(node);
                return base.Visit(node);
            }

            internal Expression Eval(Expression exp)
            {
                return Visit(exp);
            }

            private Expression Evaluate(Expression e)
            {
                if (e.NodeType == ExpressionType.Constant) return e;
                var lambda = Expression.Lambda(e);
                var fn = lambda.Compile();
                return Expression.Constant(fn.DynamicInvoke(null), e.Type);
            }
        }
    }
}