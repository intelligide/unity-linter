using System.Collections.ObjectModel;
using Mono.Cecil;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace ArsenStudio.Linter
{
    public class Linter
    {
        [MenuItem("Tools/Lint")]
        public static void Lint()
        {
            /*var asm = CompilationPipeline.GetAssemblies(AssembliesType.Player);
            foreach (var Assembly in asm)
            {
                Debug.Log(Assembly.outputPath);
            }*/
            // AssemblyDefinition.ReadAssembly();

            Collection<AssemblyDefinition> assemblies = new Collection<AssemblyDefinition>();
            assemblies.Add(AssemblyDefinition.ReadAssembly("Library/ScriptAssemblies/Assembly-CSharp.dll"));
            assemblies.Add(AssemblyDefinition.ReadAssembly("Library/ScriptAssemblies/Assembly-CSharp-Editor.dll"));
            try
            {
                Runner runner = new Runner(assemblies);
                runner.Run();
            }
            finally
            {
                foreach (var assembly in assemblies)
                {
                    assembly.Dispose();
                }
            }
        }
    }
}

