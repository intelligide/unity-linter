using System;
using Mono.Cecil;

namespace ArsenStudio.Linter
{
    /// <summary>
    /// How severe is a defect found by the rule.
    /// </summary>
    [Serializable]
    public enum ESeverity 
    {
        /// <summary>
        /// The actual code works but should be reviewed for potential problems.
        /// Often the code cannot be changed to satisfy the rule logic, 
        /// i.e. the rule will always report it unless the rule or defect is ignored.
        /// </summary>
        Audit,
        /// <summary>
        /// The actual code works, fixing the defect doesn't have a big impact.
        /// By default some runners won't display such low severity issues to keep the number of defects to a reasonable level.
        /// </summary>
        Low,
        /// <summary>
        /// The code will work most of the time or on the default, or most, common configuration
        /// </summary>
        Medium,
        /// <summary>
        /// The code may work or fails depending on values, configuration...
        /// </summary>
        High,
        /// <summary>
        /// The code can not work as expected.
        /// </summary>
        Critical,
    }
}

