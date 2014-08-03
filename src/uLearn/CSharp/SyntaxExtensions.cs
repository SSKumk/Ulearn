using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace uLearn.CSharp
{
	public static class SyntaxExtensions
	{
		public static bool HasAttribute<TAttr>(this MemberDeclarationSyntax node) where TAttr : Attribute
		{
			return GetAttributes<TAttr>((SyntaxList<AttributeListSyntax>)((dynamic)node).AttributeLists).Any();
		}

		public static string GetHint(this AttributeSyntax attribute)
		{
			if (attribute.Name.ToString() != GetAttributeShortName<HintAttribute>()) throw new Exception("Not a HintAttribute");
			return attribute.GetArgument();
		}

		public static string GetArgument(this AttributeSyntax attribute)
		{
			var expr = (LiteralExpressionSyntax)attribute.ArgumentList.Arguments[0].Expression;
			return (string)expr.Token.Value;
		}

		public static IEnumerable<AttributeSyntax> GetAttributes<TAttr>(this MethodDeclarationSyntax node)
			where TAttr : Attribute
		{
			return GetAttributes<TAttr>(node.AttributeLists);
		}

		public static IEnumerable<AttributeSyntax> GetAttributes<TAttr>(this ClassDeclarationSyntax node)
			where TAttr : Attribute
		{
			return GetAttributes<TAttr>(node.AttributeLists);
		}

		public static IEnumerable<AttributeSyntax> GetAttributes<TAttr>(SyntaxList<AttributeListSyntax> attributes)
			where TAttr : Attribute
		{
			string attrShortName = GetAttributeShortName<TAttr>();
			return attributes
				.SelectMany(a => a.Attributes)
				.Where(a => a.Name.ToString() == attrShortName);
		}

		public static string GetAttributeShortName<TAttr>()
		{
			string attrName = typeof (TAttr).Name;
			return attrName.EndsWith("Attribute") ? attrName.Substring(0, attrName.Length - "Attribute".Length) : attrName;
		}

		public static MethodDeclarationSyntax WithoutAttributes(this MethodDeclarationSyntax node)
		{
			return node.WithAttributeLists(new SyntaxList<AttributeListSyntax>());
		}

		public static string ToPrettyString(this MethodDeclarationSyntax node)
		{
			int bodyNestingSize = node.SyntaxTree.GetLineSpan(node.Body.OpenBraceToken.Span).StartLinePosition.Character;
			return new string('\t', bodyNestingSize) + node;
		}

		public static string ToPrettyString(this SyntaxNode node)
		{
			return node.ToNotIdentedString();
		}

		private static string CutFirstCharacters(string str, char character, int size)
		{
			var counter = 0;
			foreach (var c in str)
			{
				if (c == character)
					counter++;
				else
					break;
			}
			return str.Remove(Math.Min(counter, size));
		}

		public static string ToNotIdentedString(this SyntaxNode node)
		{
			return node.ToString().RemoveCommonNesting();
		}

		public static MethodDeclarationSyntax TransformExercise(this MethodDeclarationSyntax method)
		{
			return method
				.WithoutAttributes()
				.WithBody(method.Body.WithStatements(new SyntaxList<StatementSyntax>()));
		}
	}
}