using System.Collections;
using System.Windows;
using System.Windows.Media;

namespace WpfApplication2
{
    public class TextDecorator
    {
        public static TextDecoration DashedUnderline()
        {
            double[] dashes = { 0.0, -1.0, 2.0, 4.0 };
            Pen myPen = new Pen();
            myPen.Brush = new SolidColorBrush(new Color() { A = 255, R = 79, G = 129, B = 189 });
            myPen.DashStyle = new DashStyle(dashes, 1);
            myPen.Thickness = 1;

            TextDecoration myUnderline = new System.Windows.TextDecoration();
            myUnderline.Pen = myPen;
            myUnderline.PenThicknessUnit = TextDecorationUnit.Pixel;
            myUnderline.PenOffset = 1;
            return myUnderline;
        }

        public static TextDecoration WavyUnderline()
        {
            Pen myPen = new Pen();
            myPen.Brush = new SolidColorBrush(new Color() { A = 255, R = 255, G = 0, B = 0 });
            myPen.Thickness = 1;

            TextDecoration myUnderline = new System.Windows.TextDecoration();
            myUnderline.Pen = myPen;
            myUnderline.PenThicknessUnit = TextDecorationUnit.Pixel;
            myUnderline.PenOffset = 1;
            return myUnderline;
        }

    }

}
