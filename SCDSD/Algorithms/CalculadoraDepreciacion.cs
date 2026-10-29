namespace SCDSD.Algorithms;

// Motor matemático recursivo puro. No usa for, while ni foreach.
public static class CalculadoraDepreciacion
{
    // V(0) = v0
    // V(t) = V(t-1) * (1 - d/100)
    public static decimal CalcularValor(decimal v0, decimal d, int t)
    {
        // Guarda: un t negativo nunca llegaría al caso base
        if (t < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(t), t, "El tiempo no puede ser negativo.");
        }

        // Caso base: sin tiempo transcurrido, el valor es el costo original
        if (t == 0)
        {
            return v0;
        }

        // Caso recursivo: valor del año anterior por el factor de retención
        return CalcularValor(v0, d, t - 1) * (1 - d / 100m);
    }
}