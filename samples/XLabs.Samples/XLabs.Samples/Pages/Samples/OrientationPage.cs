using Xamarin.Forms;
using XLabs.Enums;
using XLabs.Ioc;
using XLabs.Platform.Device;

namespace XLabs.Samples.Pages.Samples
{
    public class OrientationPage : ContentPage
    {
        Label orientation;
        Label PageW;
        Label PageH;
        IDeviceOrientation orientationSvc;
        public OrientationPage()
        {
            orientationSvc = Resolver.Resolve<IDevice>().Orientation;
            var rl = new AbsoluteLayout() { HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand };

            var box1 = new StackLayout {BackgroundColor = Color.Blue};
            AbsoluteLayout.SetLayoutFlags(box1, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(box1,new Rectangle(0.5,0.5,0.5,0.4));

            orientation = new Label { Text = orientationSvc.GetOrientation().ToString(), BackgroundColor = Color.White };
            PageW = new Label { Text = "Page width="+this.Width.ToString(), BackgroundColor = Color.White };
            PageH = new Label { Text = "Page height="+this.Height.ToString(), BackgroundColor = Color.White };

            box1.Children.Add(orientation);
            box1.Children.Add(PageW);
            box1.Children.Add(PageH);

            rl.Children.Add(box1);
            orientationSvc.ScreenOrientationChanged += Orientation_ScreenOrientationChanged;

            

            this.Content = rl;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            PageH.Text = "Page height=" + this.Height.ToString();
            PageW.Text = "Page width=" + this.Width.ToString();
        }

        private void Orientation_ScreenOrientationChanged(object sender, EventArgs<Orientation> e)
        {
            orientation.Text = orientationSvc.GetOrientation().ToString();
            PageH.Text = "Page height=" + this.Height.ToString();
            PageW.Text = "Page width=" + this.Width.ToString();
        }
    }
}