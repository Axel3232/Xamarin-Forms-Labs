using System.Threading.Tasks;
using Xamarin.Forms;
using XLabs.Enums;
using XLabs.Ioc;
using XLabs.Platform.Device;

namespace XLabs.Samples.Pages.Samples
{
    public class OrientationPage : ContentPage
    {

        Label orientation;
        Label isLandscape;
        Label isPortrait;
        Label PageW;
        Label PageH;
        IDeviceOrientation orientationSvc;
        public OrientationPage()
        {
            orientationSvc = Resolver.Resolve<IDevice>().Orientation;
            var rl = new AbsoluteLayout() { HorizontalOptions = LayoutOptions.FillAndExpand, VerticalOptions = LayoutOptions.FillAndExpand };
            
            var box1 = new StackLayout { BackgroundColor = Color.Blue };
            AbsoluteLayout.SetLayoutFlags(box1, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(box1, new Rectangle(0.5, 0.5, 0.5, 0.4));
            
            orientation = new Label { Text = orientationSvc.GetOrientation().Orientation.ToString(), BackgroundColor = Color.White };
            isPortrait = new Label { Text = "IsPortrait : " + orientationSvc.GetOrientation().IsPortrait.ToString(), BackgroundColor = Color.White };
            isLandscape = new Label { Text = "IsLandscape : " + orientationSvc.GetOrientation().IsLandscape.ToString(), BackgroundColor = Color.White };
            PageW = new Label { Text = "Page width=" + this.Width.ToString(), BackgroundColor = Color.White };
            PageH = new Label { Text = "Page height=" + this.Height.ToString(), BackgroundColor = Color.White };
            
            box1.Children.Add(orientation);
            box1.Children.Add(isPortrait);
            box1.Children.Add(isLandscape);
            box1.Children.Add(PageW);
            box1.Children.Add(PageH);
            
            rl.Children.Add(box1);

            Button changeOrientation = new Button()
            {
                Text = "Change"
            };
            changeOrientation.Clicked += ChangeOrientation_Clicked;
            rl.Children.Add(changeOrientation);
            orientationSvc.ScreenOrientationChanged += Orientation_ScreenOrientationChanged;

            this.Content = rl;
        }

        private void ChangeOrientation_Clicked(object sender, System.EventArgs e)
        {
            if (orientationSvc.GetOrientation().IsLandscape)
                orientationSvc.SetOrientation(Orientation.Portrait);
            else
                orientationSvc.SetOrientation(Orientation.Landscape);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            PageH.Text = "Page height=" + this.Height.ToString();
            PageW.Text = "Page width=" + this.Width.ToString();
        }

        private void Orientation_ScreenOrientationChanged(object sender, EventArgs<CurrentOrientation> e)
        {
            orientation.Text = e.Value.Orientation.ToString();
            isPortrait.Text = "IsPortrait : " + e.Value.IsPortrait.ToString();
            isLandscape.Text = "IsLandscape : " + e.Value.IsLandscape.ToString();
            PageH.Text = "Page height=" + this.Height.ToString();
            PageW.Text = "Page width=" + this.Width.ToString();
        }
    }
}