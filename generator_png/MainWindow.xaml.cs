using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Library;
using Microsoft.Win32;

namespace generator_png
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        Image obraz;

        //  zmienna zawierająca WYSOKOŚĆ generowanej mapy bitowej
        int bitmapHeight;
        //  zmienna zawierająca SZEROKOŚĆ generowanej mapy bitowej
        int bitmapWidth;

        int t;  //  temporary - jest w nim który z powyższych 2 parametrów bitmapy jest tym mniejszym

        //  lista zawierająca odcinki
        List<MyStretch> Stretches;

        //  ścieżka do 'Wygenerowane_Obrazy'
        string path_to_Obrazy = "\\images";

        //  bool który sprawia, że pętla się wykonuje (pętla ze Start())
        bool continueUpdate = true;


        //  kolor odcinków
        byte colorR;
        byte colorG;
        byte colorB;
        Pen stretch;
        Pen background;


        //odpowiada za kolorowanie kwadratów do podglądu kolorów linii i tła - wadą jest to, że zmiana kwadratu zabarwi go kolorem drugiego
        private void ColorPreview_Loaded(object sender, RoutedEventArgs e)
        {
            if (isLine.IsChecked == true)
            {

                colorR = Convert.ToByte(slRed.Value);
                colorG = Convert.ToByte(slGreen.Value);
                colorB = Convert.ToByte(slBlue.Value);

                stretch = new Pen(new SolidColorBrush(Color.FromArgb(255, colorR, colorG, colorB)), 4);
                LinePreview.StrokeThickness = 4;
                LinePreview.Fill = stretch.Brush;

            }
            else if (isBackground.IsChecked == true)
            {
                colorR = Convert.ToByte(slRed.Value);
                colorG = Convert.ToByte(slGreen.Value);
                colorB = Convert.ToByte(slBlue.Value);

                background = new Pen(new SolidColorBrush(Color.FromArgb(255, colorR, colorG, colorB)), 4);
                BackgroundPreview.StrokeThickness = 4;
                BackgroundPreview.Fill = background.Brush;
            }
        }

        private void ColorPreview_LayoutUpdated(object sender, EventArgs e)
        {
            if (isLine.IsChecked == true)
            {

                colorR = Convert.ToByte(slRed.Value);
                colorG = Convert.ToByte(slGreen.Value);
                colorB = Convert.ToByte(slBlue.Value);

                stretch = new Pen(new SolidColorBrush(Color.FromArgb(255, colorR, colorG, colorB)), 4);
                LinePreview.StrokeThickness = 4;
                LinePreview.Fill = stretch.Brush;

            }
            else if (isBackground.IsChecked == true)
            {
                colorR = Convert.ToByte(slRed.Value);
                colorG = Convert.ToByte(slGreen.Value);
                colorB = Convert.ToByte(slBlue.Value);

                background = new Pen(new SolidColorBrush(Color.FromArgb(255, colorR, colorG, colorB)), 4);
                BackgroundPreview.StrokeThickness = 4;
                BackgroundPreview.Fill = background.Brush;
            }

        }

        public void Button_Click(object sender, RoutedEventArgs e)
        {

            bitmapHeight = Convert.ToInt32(PNGHeight.Text);
            bitmapWidth = Convert.ToInt32(PNGWidth.Text);

            MyStretch.Length = Convert.ToInt32(PNGLength.Value);
            MyStretch.pen = stretch;

            Start();
        }

        void Start()
        {

            Stretches = new List<MyStretch>();
            Stretches.Add(new MyStretch((int)(bitmapWidth * 0.5f), bitmapHeight / 2, true));

            if (!Directory.Exists(path_to_Obrazy))
            {
                Directory.CreateDirectory(path_to_Obrazy);
            }


            obraz = new Image();

            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            drawingContext.DrawRectangle(background.Brush, background, new Rect(0, 0, bitmapWidth, bitmapHeight));


            while (continueUpdate)
            {
                List<MyStretch> next = new List<MyStretch>();

                foreach (MyStretch ms in Stretches)
                {
                    if (ms.isNew)
                    {
                        MyStretch nextA = ms.CreateA(Stretches);
                        MyStretch nextB = ms.CreateB(Stretches);

                        if (nextA != null)
                            next.Add(nextA);
                        if (nextB != null)
                            next.Add(nextB);
                        ms.isNew = false;
                    }
                }
                t = (bitmapHeight >= bitmapWidth) ? bitmapWidth : bitmapHeight;

                foreach (MyStretch ms in next)
                {
                    if (ms.ax + MyStretch.Length >= t && ms.ay + MyStretch.Length >= t || ms.bx + MyStretch.Length >= t && ms.by + MyStretch.Length >= t)
                    {
                        continueUpdate = false;
                        break;
                    }
                }
                foreach (MyStretch s in next)
                {
                    Stretches.Add(s);
                    s.Draw(drawingContext);
                }

            }


            drawingContext.Close();

            System.Diagnostics.Debug.WriteLine("Wielkość listy " + Stretches.Count);


            RenderTargetBitmap bmp = new RenderTargetBitmap(bitmapWidth, bitmapHeight, 120, 96, PixelFormats.Pbgra32);
            bmp.Render(drawingVisual);

            obraz.Source = bmp;

            BitmapSource bmS = (BitmapSource)obraz.Source;




            string pathPlusName = path_to_Obrazy + "\\" + bitmapHeight + "_" + bitmapHeight + "_" + colorR + "_" + colorG + "_" + colorB + ".png";


            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = pathPlusName;
            saveFileDialog.Filter = "Pliki PNG | *.png";
            if (saveFileDialog.ShowDialog(this) == true)
            {
                FileStream saveStream = new FileStream(saveFileDialog.FileName, FileMode.OpenOrCreate);
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bmS));
                encoder.Save(saveStream);
                saveStream.Close();
            }

        }


        void Update(DrawingContext cont)
        {

            List<MyStretch> next = new List<MyStretch>();

            foreach (MyStretch ms in Stretches)
            {
                if (ms.isNew)
                {
                    ms.Draw(cont);
                    MyStretch nextA = ms.CreateA(Stretches);
                    MyStretch nextB = ms.CreateB(Stretches);

                    if (nextA != null)
                        next.Add(nextA);
                    if (nextB != null)
                        next.Add(nextB);
                    ms.isNew = false;
                }
            }
            t = (bitmapHeight >= bitmapWidth) ? bitmapWidth : bitmapHeight;

            foreach (MyStretch ms in next)
            {
                if (ms.ax + MyStretch.Length >= t || ms.ay + MyStretch.Length >= t || ms.bx + MyStretch.Length >= t || ms.by + MyStretch.Length >= t)
                {
                    continueUpdate = false;
                    break;
                }
            }
            Stretches.AddRange(next);
        }

    }
}
