namespace NSubstitute.Analyzers.Shared.DiagnosticAnalyzers
{
    internal struct IndexerInfo
    {
        public bool VerifyIndexerCast { get; }

        public bool VerifyAssignment { get; }

        public IndexerInfo(bool verifyIndexerCast, bool verifyAssignment)
        {
            VerifyIndexerCast = verifyIndexerCast;
            VerifyAssignment = verifyAssignment;
        }
    }
}