using System;

namespace ArsenStudio.Linter
{
	[AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public sealed class SolutionAttribute : Attribute 
    {	
		public SolutionAttribute(string solution) 
		{
			Solution = solution;
		}

		public string Solution {
			get;
			private set;
		}
	}
}