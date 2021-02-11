using Prism.Ioc;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TDMarketData.PriceActionReplay.Views;

namespace TDMarketData.PriceActionReplay
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        // RegisterTypes function is here

        protected override Window CreateShell()
        {
            var w = Container.Resolve(typeof(MainWindow)) as MainWindow;
            return w;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
           // throw new NotImplementedException();
        }
    }
}
