using System;
using Mono.Cecil;

namespace ArsenStudio.Linter
{
    public abstract class AssemblyRule : Rule
    {
        public abstract ERuleResult CheckAssembly(AssemblyDefinition assembly);
    }
}

