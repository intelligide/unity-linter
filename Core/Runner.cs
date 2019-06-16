using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using ArsenStudio.Linter.Rocks;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using UnityEngine;

namespace ArsenStudio.Linter
{
    public class Runner
    {
        protected Rule CurrentRule;
        protected IMetadataTokenProvider CurrentTarget;

        private Collection<Defect> defect_list = new Collection<Defect>();
        public Collection<Defect> Defects {
			get { return defect_list; }
        }

        private IIgnoreList ignoreList;
        public IIgnoreList IgnoreList {
			get {
				if (ignoreList == null)
                {
					throw new InvalidOperationException ("No IgnoreList has been set for this runner.");
                }
				return ignoreList;
			}
			set { ignoreList = value; }
        }

        public ESeverity SeverityLevel { get; }

        public Collection<AssemblyDefinition> Assemblies { get; private set; }

        private int defects_limit = Int32.MaxValue;
        public int DefectsLimit {
			get { return defects_limit; }
			set {
				if (value < 0)
                {
					throw new ArgumentException ("Cannot be negative", "DefectsLimit");
                }
				defects_limit = value;
			}
        }

    	private int defectCountBeforeCheck;
        /// <summary>
		/// 
		/// </summary>
		/// <returns>Return RuleResult.Failure is the number of defects has grown since 
		/// the rule Check* method was called or RuleResult.Success otherwise</returns>
		public ERuleResult CurrentRuleResult {
			get {
				return (Defects.Count > defectCountBeforeCheck) ? ERuleResult.Failure : ERuleResult.Success;
			}
        }

        public Collection<Rule> Rules { get; private set; } = new Collection<Rule>();   

        private IEnumerable<AssemblyRule> assemblyRules;
		private IEnumerable<TypeRule> typeRules;
        private IEnumerable<MethodRule> methodRules;

        public Runner(Collection<AssemblyDefinition> assemblies)
        {
            Assemblies = assemblies;
            AssemblyResolver resolver = AssemblyResolver.Resolver;
            resolver.AssemblyCache.Clear ();
            foreach (AssemblyDefinition assembly in assemblies) {
				assembly.MainModule.LoadDebuggingSymbols();
				resolver.CacheAssembly(assembly);
            }

            LoadRules();
            assemblyRules = Rules.OfType<AssemblyRule>();
			typeRules = Rules.OfType<TypeRule>();
            methodRules = Rules.OfType<MethodRule>();
        }

        public void LoadRules()
		{
			// load every dll to check for rules...
			string dir = Path.GetDirectoryName(typeof (Rule).Assembly.Location);
			FileInfo [] files = new DirectoryInfo (dir).GetFiles ("*.dll");
			foreach (FileInfo info in files) {
				// except for a few, well known, ones
				switch (info.Name) {
                    case "Mono.Cecil.dll":
                    case "Mono.Cecil.Pdb.dll":
                    case "Mono.Cecil.Mdb.dll":
                        continue;
				}

				LoadRulesFromAssembly(info.FullName);
			}
        }

        private void LoadRulesFromAssembly (string assemblyName)
		{
			AssemblyName aname = AssemblyName.GetAssemblyName(Path.GetFullPath(assemblyName));
			Assembly a = Assembly.Load(aname);
			foreach (Type t in a.GetTypes()) 
            {
				if (t.IsAbstract || t.IsInterface)
                {
					continue;
                }
                

				if (t.IsSubclassOf(typeof(Rule))) 
                {
					Rules.Add((Rule) Activator.CreateInstance(t));
				}
			}
        }

        /// <summary>
		/// For all assemblies, every modules in each assembly, every 
		/// type in each module, every methods in each type call all
		/// applicable rules.
		/// </summary>
		public void Run()
		{
			foreach (AssemblyDefinition assembly in Assemblies) 
            {
				CurrentTarget = (IMetadataTokenProvider) assembly;
				OnAssembly(assembly);

				foreach (ModuleDefinition module in assembly.Modules) 
                {
					CurrentTarget = (IMetadataTokenProvider) module;
					OnModule(module);

					foreach (TypeDefinition type in module.GetAllTypes()) 
                    {
						CurrentTarget = (IMetadataTokenProvider) type;
						OnType(type);

						foreach (MethodDefinition method in type.Methods) 
                        {
							CurrentTarget = (IMetadataTokenProvider) method;
							OnMethod(method);
						}
					}
				}
			}
			// don't report them if we hit an exception after analysis is completed (e.g. in reporting)
			CurrentRule = null;
			CurrentTarget = null;
        }

		protected virtual void OnAssembly(AssemblyDefinition assembly)
		{
			foreach (AssemblyRule rule in assemblyRules) {
				defectCountBeforeCheck = Defects.Count;
				// stop if we reach the user defined defect limit
				if (defectCountBeforeCheck >= DefectsLimit)
                {
					break;
                }

				// ignore the rule on some user defined assemblies
				if (IgnoreList.IsIgnored(rule, assembly))
                {
					continue;
                }

				CurrentRule = rule;
				rule.CheckAssembly(assembly);
			}
        }

        protected virtual void OnModule(ModuleDefinition module)
		{
			// Since it has never been used in the previous years 
			// this version of the Gendarme framework doesn't 
			// support ModuleRule. Nor do we support ignore on 
			// modules.
		}

		protected virtual void OnType(TypeDefinition type)
		{
			foreach (TypeRule rule in typeRules) 
            {
				defectCountBeforeCheck = Defects.Count;
				// stop if we reach the user defined defect limit
				if (defectCountBeforeCheck >= DefectsLimit)
                {
					break;
                }

				// ignore the rule on some user defined types
				if (IgnoreList.IsIgnored(rule, type))
                {
					continue;
                }

				CurrentRule = rule;
				rule.CheckType(type);
			}
		}

		protected virtual void OnMethod(MethodDefinition method)
		{
			foreach (MethodRule rule in methodRules) 
            {
				defectCountBeforeCheck = Defects.Count;
				// stop if we reach the user defined defect limit
				if (defectCountBeforeCheck >= DefectsLimit)
                {
					break;
                }

				// ignore the rule on some user defined methods
				if (IgnoreList.IsIgnored (rule, method))
                {
					continue;
                }

				CurrentRule = rule;
				rule.CheckMethod (method);
			}
        }

        private bool Filter(ESeverity severity, IMetadataTokenProvider location)
		{
			if(SeverityLevel > severity)
            {
				return false;
            }
			// for Assembly | Type | Methods we can ignore before executing the rule
			// but for others (e.g. Parameters, Fields...) we can only ignore the results
			return !IgnoreList.IsIgnored(CurrentRule, location);
		}

        public virtual void Report(Defect defect)
		{
			if(defect == null)
            {
				throw new ArgumentNullException("defect");
            }

			if(!Filter(defect.Severity, defect.Location))
            {
				return;
            }
				
			if(IgnoreList.IsIgnored(defect.Rule, defect.Target))
            {
				return;
            }

			defect_list.Add(defect);
        }

        public void Report(IMetadataTokenProvider metadata, ESeverity severity)
		{
			if (!Filter(severity, metadata))
            {
				return;
            }

			Defect defect = new Defect(CurrentRule, CurrentTarget, metadata, severity);
			Report (defect);
		}
    }
}

