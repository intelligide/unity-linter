using System;

namespace ArsenStudio.Linter
{
    [Serializable]
	public enum ERuleResult 
	{
		/// <summary>
		/// Rules returns this if the required conditions to execute 
		/// are not matched. This is useful to make rules more 
		/// readable (it's not a real success) and for statistics.
		/// </summary>
		DoesNotApply,
		/// <summary>
		/// Rules returns this if it has executed its logic and has
		/// not found any defect.
		/// </summary>
		Success,
		/// <summary>
		/// Rules returns this if it has executed its logic and has
		/// found one or more defects.
		/// </summary>
		Failure,
		/// <summary>
		/// Rules should never report this. The framework will use 
		/// this value to track errors in rule execution.
		/// </summary>
		Abort,
    }
}

