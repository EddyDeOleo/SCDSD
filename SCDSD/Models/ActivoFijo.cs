namespace SCDSD.Models;

// Datos de un activo fijo sujeto a depreciación por saldo decreciente
public sealed class ActivoFijo
{
    public ActivoFijo(decimal costoInicial, decimal tasa, int anios)
    {
        CostoInicial = costoInicial;
        Tasa = tasa;
        Anios = anios;
    }

    // Costo de compra original (V)
    public decimal CostoInicial { get; }

    // Tasa de depreciación anual en porcentaje (d)
    public decimal Tasa { get; }

    // Años transcurridos (t)
    public int Anios { get; }
}