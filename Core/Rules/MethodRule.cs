using System;
using Mono.Cecil;

namespace ArsenStudio.Linter
{
    public abstract class MethodRule : Rule
    {
        public abstract ERuleResult CheckMethod(MethodDefinition method);
    }
}

