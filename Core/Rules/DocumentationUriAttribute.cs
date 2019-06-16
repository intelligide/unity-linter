using System;

namespace ArsenStudio.Linter 
{
	[AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class DocumentationUriAttribute : Attribute 
    {
		public DocumentationUriAttribute(string documentationUri) 
		{
			DocumentationUri = new Uri(documentationUri);
		}

		public Uri DocumentationUri {
			get;
			private set;
		}
	}
}