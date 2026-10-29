using System.Globalization;
using SCDSD.Models;
using SCDSD.Services;

namespace SCDSD.UI;

// Capa de presentación: entradas, validaciones y formato de salida
public sealed class ConsoleUI
{
    // Límite de años para proteger la pila de llamadas de la recursión
    private const int AniosMaximos = 1000;

    private readonly DepreciacionService _servicio;

    public ConsoleUI(DepreciacionService servicio)
    {
        _servicio = servicio ?? throw new ArgumentNullException(nameof(servicio));
    }

    // Punto de entrada de la interfaz
    public void Iniciar()
    {
        MostrarEncabezado();
        EjecutarAutoverificacion();
        ProcesarCalculo();

        if (PreguntarSiContinuar())
        {
            Iniciar();
        }
        else
        {
            Console.WriteLine("\nGracias por usar SCDSD. ¡Hasta pronto!");
        }
    }

    // Verifica internamente el caso V = 1000, d = 20, t = 3 -> 512.00
    private void EjecutarAutoverificacion()
    {
        bool correcto = _servicio.VerificarCasoDePrueba();

        if (correcto)
        {
            Console.WriteLine("Autoverificación: V=1000, d=20, t=3 -> $512.00 [OK]");
        }
        else
        {
            MostrarError("La autoverificación del algoritmo falló.");
        }
    }

    // Captura los datos, calcula y muestra el resultado sin excepciones sin controlar
    private void ProcesarCalculo()
    {
        decimal costo = LeerDecimal("\nCosto inicial (V): ", v => v >= 0m, "El costo no puede ser negativo.");
        decimal tasa = LeerDecimal("Tasa de depreciación anual en % (d): ", d => d >= 0m && d <= 100m,
            "La tasa debe estar entre 0 y 100.");
        int anios = LeerEntero($"Años (t, 0 a {AniosMaximos}): ", t => t >= 0 && t <= AniosMaximos,
            $"Los años deben estar entre 0 y {AniosMaximos}. No se permiten valores negativos.");

        try
        {
            var activo = new ActivoFijo(costo, tasa, anios);
            decimal resultado = _servicio.CalcularValorActual(activo);
            MostrarResultado(activo, resultado);
        }
        catch (ArgumentException ex)
        {
            MostrarError(ex.Message);
        }
    }

    // Lee un decimal; reintenta recursivamente hasta recibir un valor válido
    private static decimal LeerDecimal(string mensaje, Func<decimal, bool> esValido, string mensajeError)
    {
        Console.Write(mensaje);
        string? entrada = Console.ReadLine();

        if (!TryParseDecimal(entrada, out decimal valor))
        {
            MostrarError("Entrada no válida. Ingrese un número (ej. 1000 o 1500.50).");
            return LeerDecimal(mensaje, esValido, mensajeError);
        }

        if (!esValido(valor))
        {
            MostrarError(mensajeError);
            return LeerDecimal(mensaje, esValido, mensajeError);
        }

        return valor;
    }

    // Lee un entero; reintenta recursivamente hasta recibir un valor válido
    private static int LeerEntero(string mensaje, Func<int, bool> esValido, string mensajeError)
    {
        Console.Write(mensaje);
        string? entrada = Console.ReadLine();

        if (!int.TryParse(entrada?.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int valor))
        {
            MostrarError("Entrada no válida. Ingrese un número entero (ej. 0, 3, 10).");
            return LeerEntero(mensaje, esValido, mensajeError);
        }

        if (!esValido(valor))
        {
            MostrarError(mensajeError);
            return LeerEntero(mensaje, esValido, mensajeError);
        }

        return valor;
    }

    // Convierte texto a decimal aceptando punto o coma como separador
    private static bool TryParseDecimal(string? entrada, out decimal valor)
    {
        valor = 0m;

        if (string.IsNullOrWhiteSpace(entrada))
        {
            return false;
        }

        string normalizada = entrada.Trim().Replace(',', '.');
        return decimal.TryParse(normalizada, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
            CultureInfo.InvariantCulture, out valor);
    }

    // Pregunta si se desea otro cálculo; repite la pregunta ante una respuesta inválida
    private static bool PreguntarSiContinuar()
    {
        Console.Write("\n¿Desea realizar otro cálculo? (S/N): ");
        string? respuesta = Console.ReadLine()?.Trim().ToUpperInvariant();

        return respuesta switch
        {
            "S" or "SI" or "SÍ" => true,
            "N" or "NO" => false,
            _ => PreguntarSiContinuar()
        };
    }

    private static void MostrarEncabezado()
    {
        Console.WriteLine();
        Console.WriteLine("==============================================");
        Console.WriteLine("  SCDSD - Módulo de Depreciación Recursiva");
        Console.WriteLine("==============================================");
    }

    private static void MostrarResultado(ActivoFijo activo, decimal resultado)
    {
        Console.WriteLine();
        Console.WriteLine("-------------- Resultado --------------");
        Console.WriteLine($"Costo inicial (V) : {FormatearMoneda(activo.CostoInicial)}");
        Console.WriteLine($"Tasa anual (d)    : {activo.Tasa.ToString("0.####", CultureInfo.InvariantCulture)} %");
        Console.WriteLine($"Años (t)          : {activo.Anios}");
        Console.WriteLine($"Valor V(t)        : {FormatearMoneda(resultado)}");
        Console.WriteLine("---------------------------------------");
    }

    // Formato monetario con exactamente dos decimales (ej. $512.00)
    private static string FormatearMoneda(decimal valor)
        => "$" + valor.ToString("N2", CultureInfo.InvariantCulture);

    private static void MostrarError(string mensaje)
    {
        ConsoleColor colorOriginal = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[Error] {mensaje}");
        Console.ForegroundColor = colorOriginal;
    }
}
