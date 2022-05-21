namespace DotNetSca;

public static class AstHelper
{
    public static IEnumerable<Ast.FeaturesList> FilterBySize(this IEnumerable<Ast.FeaturesList> featuresLists,
        int charsCount) => featuresLists.Where(x => x.SymbolsLength > charsCount);
}