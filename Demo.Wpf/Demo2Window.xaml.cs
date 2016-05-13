using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Demo.Wpf
{
    [Serializable]
    public sealed class MyCustomStrokes
    {
        public MyCustomStrokes() { }
        /// <SUMMARY>
        /// The first index is for the stroke no.
        /// The second index is for the keep the 2D point of the Stroke.
        /// </SUMMARY>
        public Point[][] StrokeCollection;
    }

    public partial class Demo2Window : Window
    {
        private byte[] RAM = null;
        public Demo2Window()
        {
            InitializeComponent();
        }

        private void DrawCanvas(byte[] strokeBytes, InkCanvas targetInkCanvas)
        {
            try
            {
                //deserialize it
                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream ms = new MemoryStream(strokeBytes);

                MyCustomStrokes customStrokes = bf.Deserialize(ms) as MyCustomStrokes;

                //rebuilt it
                for (int i = 0; i < customStrokes.StrokeCollection.Length; i++)
                {
                    if (customStrokes.StrokeCollection[i] != null)
                    {
                        StylusPointCollection stylusCollection = new
                          StylusPointCollection(customStrokes.StrokeCollection[i]);

                        Stroke stroke = new Stroke(stylusCollection);
                        StrokeCollection strokes = new StrokeCollection();
                        strokes.Add(stroke);

                        targetInkCanvas.Strokes.Add(strokes);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private byte[] ReadCanvas(InkCanvas sourceInkCanvas)
        {
            StrokeCollection strokes = sourceInkCanvas.Strokes;

            if (strokes.Count > 0)
            {
                MyCustomStrokes customStrokes = new MyCustomStrokes();

                customStrokes.StrokeCollection = new Point[strokes.Count][];

                for (int i = 0; i < strokes.Count; i++)
                {
                    customStrokes.StrokeCollection[i] = new Point[strokes[i].StylusPoints.Count];

                    for (int j = 0; j < strokes[i].StylusPoints.Count; j++)
                    {
                        customStrokes.StrokeCollection[i][j] = new Point();
                        customStrokes.StrokeCollection[i][j].X = strokes[i].StylusPoints[j].X;
                        customStrokes.StrokeCollection[i][j].Y = strokes[i].StylusPoints[j].Y;
                    }
                }

                //Serialize
                MemoryStream ms = new MemoryStream();
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, customStrokes);

                try
                {
                    return ms.GetBuffer();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return null;
        }

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            //RAM = ReadCanvas(ink1);
            //DrawCanvas(RAM, ink2);
            ink2.Strokes = ink1.Strokes;
        }

        private void btnRedPen_Click(object sender, RoutedEventArgs e)
        {
            ink1.EditingMode = InkCanvasEditingMode.Ink;
            ink1.DefaultDrawingAttributes.Color = Colors.Red;
        }

        private void btnBluePen_Click(object sender, RoutedEventArgs e)
        {
            ink1.EditingMode = InkCanvasEditingMode.Ink;
            ink1.DefaultDrawingAttributes.Color = Colors.Blue;
        }

        private void btnErase_Click(object sender, RoutedEventArgs e)
        {
            ink1.EditingMode = InkCanvasEditingMode.EraseByPoint;
        }

        private string xaml;

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            StringWriter wr = new StringWriter();
            XamlWriter.Save(richtext1.Document, wr);
            xaml = wr.ToString();
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FlowDocument doc;

                if (TryParseFlowDocument(xaml, out doc))
                    richtext2.Document = doc;
                else
                    richtext2.Document = new FlowDocument();
            }
            catch (Exception pex)
            {
                throw pex;
            }
        }

        private Boolean TryParseFlowDocument(String text, out FlowDocument flowDocument)
        {
            try
            {
                if (text == null || text == String.Empty)
                    throw new Exception("Argument text cannot be NULL or empty string.");

                flowDocument = XamlReader.Parse(text) as FlowDocument;
                return true;
            }
            catch (Exception ex)
            {
                flowDocument = null;
                return false;
            }
        }
    }
}
