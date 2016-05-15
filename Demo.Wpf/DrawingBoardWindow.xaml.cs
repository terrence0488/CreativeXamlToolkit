using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Demo.Wpf
{
    /// <summary>
    /// Interaction logic for DrawingBoardWindow.xaml
    /// </summary>
    public partial class DrawingBoardWindow : Window
    {
        public DrawingBoardWindow()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnPen_Click(object sender, RoutedEventArgs e)
        {
            inkMain.UseCustomCursor = true;
            inkMain.Cursor = Cursors.Pen;
            inkMain.DefaultDrawingAttributes = new System.Windows.Ink.DrawingAttributes()
            {
                Color = Colors.Blue,
                Height = 5,
                Width = 5,
                StylusTip = System.Windows.Ink.StylusTip.Ellipse
            };

            inkMain.EditingMode = InkCanvasEditingMode.Ink;
        }

        private void btnEraser_Click(object sender, RoutedEventArgs e)
        {
            inkMain.UseCustomCursor = true;
            inkMain.Cursor = Cursors.Cross;
            inkMain.DefaultDrawingAttributes = new System.Windows.Ink.DrawingAttributes()
            {
                Color = Colors.White,
                Height = 10,
                Width = 10,
                StylusTip = System.Windows.Ink.StylusTip.Rectangle
            };

            inkMain.EditingMode = InkCanvasEditingMode.Ink;
        }

        private void btnSaveImage_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.FileName = "Untitled"; // Default file name
            dlg.DefaultExt = ".png"; // Default file extension
            dlg.Filter = "Images (.png)|*.png"; // Filter files by extension

            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                // Save document
                string sigPath = dlg.FileName;
                MemoryStream ms = new MemoryStream();
                FileStream fs = new FileStream(sigPath, FileMode.Create);

                RenderTargetBitmap rtb = new RenderTargetBitmap(500, 500, 96d, 96d, PixelFormats.Default);
                rtb.Render(inkMain);
                JpegBitmapEncoder encoder = new JpegBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(rtb));

                encoder.Save(fs);
                fs.Close();
            }
        }
    }
}
