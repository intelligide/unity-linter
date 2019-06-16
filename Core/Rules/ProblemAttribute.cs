using System;

namespace ArsenStudio.Linter
{
	[AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class ProblemAttribute : Attribute 
    {
		public ProblemAttribute(string problem) 
		{
			Problem = problem;
		}

		public string Problem {
			get;
			private set;
		}
	}
}