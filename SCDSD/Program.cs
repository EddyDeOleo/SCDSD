using SCDSD.Services;
using SCDSD.UI;

namespace SCDSD;

class Program
{
    static void Main(string[] args)
    {
        var ui = new ConsoleUI(new DepreciacionService());
        ui.Iniciar();
    }
}
