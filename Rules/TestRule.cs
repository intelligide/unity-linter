using System;
using Mono.Cecil;

namespace ArsenStudio.Linter.Rule
{
    public class TestRule : TypeRule
    {
        public override ERuleResult CheckType(TypeDefinition type)
        {
            return ERuleResult.Success;
        }
    }
}

