using System;
using System.Linq;
using System.Threading.Tasks;
using IFEngine_UWP.Support;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace IFEngine_UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        internal class menuItem
        {
            public string Glyph { get; set; }
            public string Text { get; set; }
        }

        public MainPage()
        {
            this.InitializeComponent();

            splitView.DataContext = new { Menu = new []
                {
                    new menuItem { Text = "Open", Glyph = "" }
                }
            };
        }

        private async Task OpenAndRunAsync()
        {
            await AppCore.Inst.LoadAndRunAsync(Console);
        }
        
        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            splitView.IsPaneOpen = !splitView.IsPaneOpen;
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            OpenAndRunAsync();

        }
        
    }
}
