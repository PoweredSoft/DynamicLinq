using System;

namespace PoweredSoft.DynamicLinq.Parser
{
    public class ExpressionParserPiece
    {
        public Type Type { get; set; }
        public bool IsGenericEnumerable { get; set; }
        public Type EnumerableType { get; set; }
        public ExpressionParserPiece Parent { get; set; }
        public string Name { get; internal set; }

#if DEBUG
        public string DebugPath => $"{Type?.Name} {Name} -> {Parent?.DebugPath ?? "ROOT PARAMETER"}";
        public override string ToString() => $"{Type?.Name} {Name}";
#endif
    }
}
