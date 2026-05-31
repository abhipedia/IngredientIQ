namespace IngredientIQ.Domain.Enums;

[Flags]
public enum RiskTag
{
    None = 0,
    Carcinogen = 1 << 0,
    Allergen = 1 << 1,
    EndocrineDisruptor = 1 << 2,
    GMO = 1 << 3,
    Artificial = 1 << 4,
    Natural = 1 << 5,
    Vegan = 1 << 6,
    GlutenFree = 1 << 7,
    Organic = 1 << 8
}
