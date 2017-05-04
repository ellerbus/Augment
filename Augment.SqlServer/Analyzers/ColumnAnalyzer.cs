namespace Augment.SqlServer.Analyzers
{
    class ColumnAnalyzer
    {
        public string Name { get; set; }
        public string Definition { get; set; }
        public bool IsIdentity { get { return Definition.Contains(" identity("); } }
        public bool IsReadOnly { get { return Definition.Contains("timestamp") || Definition.Contains("rowversion") || Definition.StartsWithSameAs("as "); } }
    }
}
