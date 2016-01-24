using System;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace IFCore
{
    // For Consolas
    public static class CONSOLAS_FONT
    {
        public static string Name { get; private set; } = "Consolas";
        public static double WidthAt10Px { get; private set; } = 5.496666431427;
        public static double HeightAt10Px { get; private set; } = 11.710000038147;
    }
}