using System;
using System.CodeDom.Compiler;
using System.IO;
using Microsoft.CSharp;
using NUnit.Framework;

namespace HrKit
{
	class Compiller
	{
		public static bool CompileExe(string exePath, string aiCsFile)
		{
			if (!File.Exists(exePath))
			{
				var compilerParameters =
					new CompilerParameters(new[] { "System.dll", "System.Core.dll", "System.Drawing.dll", "System.Windows.Forms.dll" })
					{
						GenerateExecutable = true,
						IncludeDebugInformation = true,
						OutputAssembly = exePath
					};
				var result = new CSharpCodeProvider().CompileAssemblyFromFile(compilerParameters, aiCsFile);
				if (result.Errors.Count > 0)
				{
					return false;
				}
			}
			return true;
		}
	}

	[TestFixture]
	public class Compiller_should
	{
		[Test]
		public void compile_empty_main()
		{
			File.WriteAllText("a.cs", "class A{ static void Main(){} }");
			if (File.Exists("a.exe"))
				File.Delete("a.exe");
			Assert.IsTrue(Compiller.CompileExe("a.exe", "a.cs"));
			Assert.IsTrue(File.Exists("a.exe"));
		}

		[Test]
		public void not_compile_empty_text()
		{
			File.WriteAllText("a.cs", "");
			if (File.Exists("a.exe"))
				File.Delete("a.exe");
			Assert.IsFalse(Compiller.CompileExe("a.exe", "a.cs"));
			Assert.IsFalse(File.Exists("a.exe"));
		}
	}
}