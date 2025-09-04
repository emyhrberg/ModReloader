using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ModReloader.Core.Features.Publicizer
{
    public sealed class PublicizerAssemblyContext
    {
        public PublicizerAssemblyContext(string assemblyName)
        {
            AssemblyName = assemblyName;
        }

        internal string AssemblyName { get; }
        internal bool ExplicitlyPublicizeAssembly { get; set; } = false;
        internal bool IncludeCompilerGeneratedMembers { get; set; } = true;
        internal bool IncludeVirtualMembers { get; set; } = true;
        internal bool ExplicitlyDoNotPublicizeAssembly { get; set; } = false;
        internal HashSet<string> PublicizeMemberPatterns { get; } = new HashSet<string>();
        internal Regex PublicizeMemberRegexPattern { get; set; }
        internal HashSet<string> DoNotPublicizeMemberPatterns { get; } = new HashSet<string>();
    }
}
