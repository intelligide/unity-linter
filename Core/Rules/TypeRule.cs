using System;
using Mono.Cecil;

namespace ArsenStudio.Linter
{
    public abstract class TypeRule : Rule
    {
        public abstract ERuleResult CheckType(TypeDefinition type);
    }
}

