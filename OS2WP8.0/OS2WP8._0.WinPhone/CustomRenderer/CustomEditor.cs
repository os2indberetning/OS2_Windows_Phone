using OS2Indberetning.WinPhone.CustomRenderer;
using Xamarin.Forms;
using Xamarin.Forms.Platform.WinPhone;

[assembly: ExportRenderer(typeof(Editor), typeof(MyEditorRenderer))]
namespace OS2Indberetning.WinPhone.CustomRenderer
{
    public class MyEditorRenderer : EditorRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Editor> e)
        {
            base.OnElementChanged(e);
            
            if (Control != null)
            {
                //Control.BorderThickness = new Thickness(1);
                //Control.BorderBrush = new SolidColorBrush(Colors.Black);
            }
        }
    }
}
