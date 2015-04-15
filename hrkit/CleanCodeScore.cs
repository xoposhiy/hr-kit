using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HrKit
{
	public class CleanCodeScore : AbstractScore
	{
		public CleanCodeScore(string sourceCode, int expectedLOC = 600)
		{
			var tree = CSharpSyntaxTree.ParseText(sourceCode);
			var lines = sourceCode.Split('\r', '\n');
			SetMetric("LOC", GetLOC(lines), expectedLOC, expectedLOC*1.5);
			SetMetric("FunLenMax", GetNestingMax(tree), 20, 40, 60, 80);
			SetMetric("NestingMax", GetFunLenMax(tree), 4, 5, 6, 7);
			SetMetric("Voids", GetVoids(tree), 40, 75);
			SetMetric("LineMaxLen", lines.Max(line => line.Length), 120, 160, 200);
		}

		private void SetMetric(string name, double value, params double[] thresholds)
		{
			points["#" + name] = value;
			var i = 0;
			int d = 1;
			while (i < thresholds.Length && value > thresholds[i])
			{
				i++;
				d *= 2;
			}
			d /= 2;
			points[name] = -d;
		}

		private int GetNestingMax(SyntaxTree tree)
		{
			return tree.GetRoot()
				.DescendantNodes().OfType<StatementSyntax>()
				.Select(n => GetParents(n).OfType<BlockSyntax>().Count()).Max();
		}

		private static int GetFunLenMax(SyntaxTree tree)
		{
			var blockSizes = tree.GetRoot()
				.DescendantNodes()
				.OfType<BlockSyntax>()
				.Select(b => tree.GetLineSpan(b.Span))
				.Select(s => s.EndLinePosition.Line - s.StartLinePosition.Line + 1);
			return blockSizes.Max();
		}

		private static int GetVoids(SyntaxTree tree)
		{
			var returnTypes =
				tree.GetRoot().DescendantNodes().OfType<MethodDeclarationSyntax>()
					.Select(d => d.ReturnType.ToString()).ToList();
			var voids = 100*(returnTypes.Count(t => t == "void") - 1)/returnTypes.Count;
			return voids;
		}

		private static int GetLOC(string[] lines)
		{
			return lines.Select(line => line.Trim())
				.Count(line => !(line.StartsWith("//") || line.StartsWith("/*") || string.IsNullOrWhiteSpace(line)));
		}

		private IEnumerable<SyntaxNode> GetParents(SyntaxNode node)
		{
			while (node.Parent != null)
			{
				yield return node = node.Parent;
			}
		}
	}
}