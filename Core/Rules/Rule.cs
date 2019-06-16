using System;
using System.Globalization;
using Mono.Cecil;

namespace ArsenStudio.Linter
{
    public abstract class Rule
    {
        private Type type;
        private Type Type {
			get {
				if (type == null)
                {
					type = GetType();
                }
				return type;
			}
        }

        /// <summary>
		/// Return true if the rule is currently active, false otherwise.
		/// </summary>
		public bool Active;

        private string name;
        /// <summary>
		/// Return the short name of the rule.
		/// By default this returns the name of the current class.
		/// </summary>
		public virtual string Name {
			get {
				if (name == null)
                {
					name = Type.Name;
                }
				return name;
			}
        }

        private string full_name;
        /// <summary>
		/// Return the full name of the rule.
		/// By default this returns the full name of the current class.
		/// </summary>
		public virtual string FullName {
			get {
				if (full_name == null) 
                {
					full_name = Type.FullName;
                }
				return full_name;
			}
        }

        private Uri uri;
        public virtual Uri Uri {
			get {
				if(uri == null) 
                {
					object [] attributes = Type.GetCustomAttributes(typeof(DocumentationUriAttribute), true);
					if(attributes.Length == 0) 
                    {
						string url = String.Format(CultureInfo.InvariantCulture, 
							"https://github.com/spouliot/gendarme/wiki/{0}.{1}({2})",
							type.Namespace, Name, "2.10");
						uri = new Uri (url);
					}
                    else 
                    {
						uri = (attributes [0] as DocumentationUriAttribute).DocumentationUri;
					}
				}
				return uri;
			}
        }

        private string problem;
        public virtual string Problem { 
			get {
				if(problem == null) 
                {
					object obj = GetCustomAttribute(typeof(ProblemAttribute));
					if(obj == null)
                    {
						problem = "Missing [Problem] attribute on rule.";
                    }
					else
                    {
						problem = (obj as ProblemAttribute).Problem;
                    }
				}
				return problem;
			}
		}

        private string solution;
		public virtual string Solution { 
			get {
				if(solution == null) 
                {
					object obj = GetCustomAttribute(typeof(SolutionAttribute));
					if(obj == null)
                    {
						solution = "Missing [Solution] attribute on rule.";
                    }
					else
                    {
						solution = (obj as SolutionAttribute).Solution;
                    }
				}
				return solution;
			}
        }

        private object GetCustomAttribute (Type t)
		{
			object[] attributes = Type.GetCustomAttributes(t, true);
			if(attributes.Length == 0)
            {
				return null;
            }
			return attributes[0];
        }
    }
}

