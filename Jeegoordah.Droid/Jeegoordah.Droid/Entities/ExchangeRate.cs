using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Jeegoordah.Droid.Entities
{
    public class ExchangeRate : Identifiable
    {
        public int Currency { get; set; }
        public string Date { get; set; }
        public decimal Rate { get; set; }
    }
}