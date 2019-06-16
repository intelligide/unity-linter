using System;
using Mono.Cecil;

namespace ArsenStudio.Linter
{
	/// <summary>
	/// This interface defines how to query the ignore list
	/// </summary>
	public interface IIgnoreList 
    {
		Runner Runner { get; }
		void Add(string rule, IMetadataTokenProvider metadata);
		bool IsIgnored(Rule rule, IMetadataTokenProvider metadata);
	}
}