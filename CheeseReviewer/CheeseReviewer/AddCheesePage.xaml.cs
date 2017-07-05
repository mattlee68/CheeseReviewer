using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CheeseReviewer
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddCheesePage : ContentPage
	{
		public AddCheesePage ()
		{
			InitializeComponent ();
        }

        void EditorTextChanged(object sender, TextChangedEventArgs e)
        {
            var oldText = e.OldTextValue;
            var newText = e.NewTextValue;
        }


        void OnSliderValueChanged(object sender, ValueChangedEventArgs args)
        {

        }

        void OnAdd(object sender, ValueChangedEventArgs args)
        {

        }

        void TakePhoto(object sender, ValueChangedEventArgs args)
        {

        }

    }
}