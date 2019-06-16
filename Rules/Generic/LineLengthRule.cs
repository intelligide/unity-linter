using System;
using Mono.Cecil;

namespace ArsenStudio.Linter.Rule
{
    public class LineLengthRule : AssemblyRule
    {
        /// <summary>
        /// The limit that the length of a line should not exceed.
        /// </summary>
        public int LineLimit = 80;

        /// <summary>
        /// The limit that the length of a line must not exceed.
        /// </summary>
        public int AbsoluteLineLimit  = 100;

        public override ERuleResult CheckAssembly(AssemblyDefinition assembly)
        {
            return ERuleResult.Success;
        }
    }
}

