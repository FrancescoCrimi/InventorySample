using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace Inventory.Uwp.Library.Common
{
    public static class GridLengths
    {
        public static readonly GridLength Zero = new GridLength(0);
        public static readonly GridLength Star = new GridLength(1, GridUnitType.Star);
        public static readonly GridLength Auto = new GridLength(1, GridUnitType.Auto);
    }
}
