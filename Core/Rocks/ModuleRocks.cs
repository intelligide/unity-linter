using System;
using System.IO;
using System.Runtime.InteropServices;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace ArsenStudio.Linter.Rocks 
{
    /// <summary>
	/// ModuleRocks contains extensions methods for ModuleDefinition
	/// and the related collection classes.
	/// </summary>
    public static class ModuleRocks 
    {
        static bool runningOnMono;

		static ModuleRocks ()
		{
			runningOnMono = typeof(object).Assembly.GetType("System.MonoType", false) != null;
		}

        /// <summary>
		/// Load, if available, the debugging symbols associated with the module. This first
		/// try to load a MDB file (symbols from the Mono:: runtime) and then, if not present 
		/// and running on MS.NET, try to load a PDB file (symbols from MS runtime).
		/// </summary>
		/// <param name="self"></param>
		public static void LoadDebuggingSymbols(this ModuleDefinition self)
		{
			if (self == null)
            {
				return;
            }

			// don't create a new reader if the symbols are already loaded
			if (self.HasSymbols)
            {
				return;
            }

			string image_name = self.FileName;
			string symbol_name = image_name + ".mdb";
			Type reader_type = null;

			// we can always load Mono symbols (whatever the runtime we're using)
			// so we start by looking for it's debugging symbol file
			if(File.Exists(symbol_name)) 
            {
				// "always" if we can find Mono.Cecil.Mdb
				reader_type = Type.GetType("Mono.Cecil.Mdb.MdbReaderProvider, Mono.Cecil.Mdb, Version=0.9.4.0, Culture=neutral, PublicKeyToken=0738eb9f132ed756");
				// load the assembly from the current folder if
				// it is here, or fallback to the gac
			}
			
			// if we could not load Mono's symbols then we try, if not running on Mono,
			// to load MS symbols (PDB files)
			if(reader_type == null && !runningOnMono) 
            {
				// assume we're running on MS.NET
				symbol_name = Path.ChangeExtension(image_name, ".pdb");
				if(File.Exists(symbol_name)) 
                {
					reader_type = Type.GetType("Mono.Cecil.Pdb.PdbReaderProvider, Mono.Cecil.Pdb");
				}
			}

			// no symbols are available to load
			if(reader_type == null)
            {
				return;
            }

			ISymbolReaderProvider provider = (ISymbolReaderProvider) Activator.CreateInstance(reader_type);
			try {
				self.ReadSymbols (provider.GetSymbolReader (self, image_name));
			}
			catch(FileNotFoundException) {
				// this happens if a MDB file is missing 	 
			}
			catch(TypeLoadException) {
				// this happens if a Mono.Cecil.Mdb.dll is not found
			}
			catch(COMException) {
				// this happens if a PDB file is missing
			}
			catch(FormatException) {
				// Mono.Cecil.Mdb wrap MonoSymbolFileException inside a FormatException
				// This makes it possible to catch such exception without a reference to the
				// Mono.CompilerServices.SymbolWriter.dll assembly
			}
			catch(InvalidOperationException) {
				// this happens if the PDB is out of sync with the actual DLL (w/new PdbCciReader)
			}
			// in any case (of failure to load symbols) Gendarme can continue its analysis (but some rules
			// can be affected). The HasDebuggingInformation extension method let them adjust themselves
        }
    }
}