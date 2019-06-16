using System;
using Mono.Cecil;

namespace ArsenStudio.Linter
{
    public class Defect
    {
        public Rule Rule {
			get;
			private set;
        }

        public IMetadataTokenProvider Location {
			get;
			private set;
        }

        public ESeverity Severity {
			get;
			private set;
        }

        public IMetadataTokenProvider Target {
			get;
			private set;
        }
        
        public string Text {
			get;
			private set;
        }

        private string source = null;
        public string Source {
			get {
				if (source == null)
                {
					// source = Symbols.GetSource(this);
                }
				return source;
			}
        }

        public Defect(Rule rule, IMetadataTokenProvider target, IMetadataTokenProvider location, ESeverity severity, string text)
		{
			if (rule == null)
            {
				throw new ArgumentNullException("rule");
            }
			if (target == null)
            {
				throw new ArgumentNullException("target");
            }
			if (location == null)
            {
				throw new ArgumentNullException("location");
            }

			Rule = rule;
			Target = target;
			Location = location;
			Severity = severity;
			Text = text;
        }

        public Defect(Rule rule, IMetadataTokenProvider target, IMetadataTokenProvider location, ESeverity severity)
			: this(rule, target, location, severity, String.Empty)
		{
        }
    }
}

