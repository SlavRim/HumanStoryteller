using System;
using System.Collections.Generic;

namespace HumanStoryteller.NewtonsoftShell.Newtonsoft.Json.Linq.JsonPath;

	
	
	internal class CompositeExpression : QueryExpression
	{
		public List<QueryExpression> Expressions
		{
			get;
			set;
		}

		public CompositeExpression(QueryOperator @operator)
			: base(@operator)
		{
			Expressions = new List<QueryExpression>();
		}

		public override bool IsMatch(JToken root, JToken t)
		{
			switch (Operator)
			{
			case QueryOperator.And:
				foreach (QueryExpression expression in Expressions)
				{
					if (!expression.IsMatch(root, t))
					{
						return false;
					}
				}
				return true;
			case QueryOperator.Or:
				foreach (QueryExpression expression2 in Expressions)
				{
					if (expression2.IsMatch(root, t))
					{
						return true;
					}
				}
				return false;
			default:
				throw new ArgumentOutOfRangeException();
			}
		}
	}
