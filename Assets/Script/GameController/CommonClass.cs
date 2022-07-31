public class Drop
{
    public Hexagon.Pivot pivot;
    public Hexagon.Pivot calc;
    public bool Revert = false; // true for increament top to down

    public Drop(Hexagon.Pivot pivot, Hexagon.Pivot calc, bool revert)
    {
        this.pivot = pivot;
        this.calc = calc;
        Revert = revert;
    }
}