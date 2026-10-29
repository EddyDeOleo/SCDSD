using SCDSD.Algorithms;
using SCDSD.Models;

namespace SCDSD.Services;

// Orquesta la validación de reglas de negocio y el uso del algoritmo
public sealed class DepreciacionService
{
    // Calcula el valor en libros del activo luego de t años
    public decimal CalcularValorActual(ActivoFijo activo)
    {
        ArgumentNullException.ThrowIfNull(activo);

        if (activo.CostoInicial < 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(activo), "El costo inicial no puede ser negativo.");
        }

        if (activo.Tasa < 0m || activo.Tasa > 100m)
        {
            throw new ArgumentOutOfRangeException(nameof(activo), "La tasa debe estar entre 0 y 100.");
        }

        if (activo.Anios < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(activo), "Los años no pueden ser negativos.");
        }

        return CalculadoraDepreciacion.CalcularValor(activo.CostoInicial, activo.Tasa, activo.Anios);
    }

    // Prueba interna: V = 1000, d = 20, t = 3 debe dar exactamente 512.00
    public bool VerificarCasoDePrueba()
    {
        var activoPrueba = new ActivoFijo(1000m, 20m, 3);
        decimal resultado = CalcularValorActual(activoPrueba);

        return Math.Round(resultado, 2) == 512.00m;
    }
}
