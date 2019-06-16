using System;
using System.Collections.Generic;
using System.IO;
using Mono.Cecil;

namespace ArsenStudio.Linter
{
    public class AssemblyResolver : BaseAssemblyResolver 
    {
		static private AssemblyResolver resolver;

		static public AssemblyResolver Resolver {
			get {
				if (resolver == null)
					resolver = new AssemblyResolver ();
				return resolver;
			}
		}

		public IDictionary<string,AssemblyDefinition> AssemblyCache { get; private set; } = new Dictionary<string, AssemblyDefinition>();

		public override AssemblyDefinition Resolve(AssemblyNameReference name)
		{
			if(name == null)
			{
				throw new ArgumentNullException("name");
			}

			string aname = name.Name;
			AssemblyDefinition asm = null;
			if (!AssemblyCache.TryGetValue(aname, out asm)) 
			{
				try 
				{
					asm = base.Resolve(name);
				}
				catch (FileNotFoundException) 
				{
					// note: analysis will be incomplete
				}
				AssemblyCache.Add(aname, asm);
			}
			return asm;
		}

		public void CacheAssembly(AssemblyDefinition assembly)
		{
			if (assembly == null)
			{
				throw new ArgumentNullException ("assembly");
			}

			AssemblyCache.Add(assembly.Name.Name, assembly);
			string location = Path.GetDirectoryName(assembly.MainModule.FileName);
			AddSearchDirectory(location);
		}
    }
}
