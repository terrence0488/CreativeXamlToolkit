using Demo.Wpf.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Demo.Wpf
{
    public static class LogicalTreeUtility
    {
        public static IEnumerable GetChildren(DependencyObject obj, Boolean allChildrenInHierachy)
        {
            if (!allChildrenInHierachy)
                return LogicalTreeHelper.GetChildren(obj);
            else
            {
                List<object> ReturnValues = new List<object>();
                RecursionReturnAllChildren(obj, ReturnValues);
                return ReturnValues;
            }
        }

        private static void RecursionReturnAllChildren(DependencyObject obj, List<object> returnValues)
        {
            foreach (object curChild in LogicalTreeHelper.GetChildren(obj))
            {
                returnValues.Add(curChild);
                if (curChild is DependencyObject)
                    RecursionReturnAllChildren((DependencyObject)curChild, returnValues);
            }
        }

        public static IEnumerable<ReturnType> GetChildren<ReturnType>(DependencyObject obj, Boolean allChildrenInHierachy)
        {
            foreach (object child in GetChildren(obj, allChildrenInHierachy))
                if (child is ReturnType)
                    yield return (ReturnType)child;
        }

        public static DependencyObject GetParent(DependencyObject obj)
        {
            if (obj == null)
                return null;

            ContentElement ce = obj as ContentElement;
            if (ce != null)
            {
                DependencyObject parent = ContentOperations.GetParent(ce);
                if (parent != null)
                    return parent;

                FrameworkContentElement fce = ce as FrameworkContentElement;
                return fce != null ? fce.Parent : null;
            }

            FrameworkElement fe = obj as FrameworkElement;
            if (fe != null)
            {
                DependencyObject parent = fe.Parent;
                if (parent != null)
                    return parent;
            }

            return VisualTreeHelper.GetParent(obj);
        }

        public static T FindAncestorOrSelf<T>(DependencyObject obj) where T : DependencyObject
        {
            while (obj != null)
            {
                T objTest = obj as T;
                if (objTest != null)
                    return objTest;
                obj = GetParent(obj);
            }
            return null;
        }
    }

    public partial class Demo3Window : Window
    {
        InkCanvas editing = null;

        public Demo3Window()
        {
            InitializeComponent();
        }

        private void btnDraw_Click(object sender, RoutedEventArgs e)
        {
            wrapDrawingTool.Visibility = Visibility.Visible;
            wrapRichTextTool.Visibility = Visibility.Collapsed;
            wrapSaveLoad.Visibility = Visibility.Collapsed;
            inkDrawBoard.Visibility = Visibility.Visible;
            txtContent.Visibility = Visibility.Collapsed;

            inkDrawBoard.Strokes.Clear();
        }

        private void btnDrawDone_Click(object sender, RoutedEventArgs e)
        {
            wrapDrawingTool.Visibility = Visibility.Collapsed;
            wrapRichTextTool.Visibility = Visibility.Visible;
            wrapSaveLoad.Visibility = Visibility.Visible;
            inkDrawBoard.Visibility = Visibility.Collapsed;
            txtContent.Visibility = Visibility.Visible;

            if (editing == null)
            {
                InkCanvas ink = new InkCanvas();
                ink.MinHeight = 0;
                ink.EditingMode = InkCanvasEditingMode.None;
                ink.Strokes = inkDrawBoard.Strokes.Clone();
                ink.PreviewMouseLeftButtonDown += EditingInkCanvas_PreviewMouseLeftButtonDown;

                ScrollViewer sv = new ScrollViewer();
                sv.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                sv.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
                sv.Content = ink;

                BlockUIContainer uiContainer = new BlockUIContainer();
                uiContainer.Child = sv;

                txtContent.Document.Blocks.Add(uiContainer);
                txtContent.Document.Blocks.Add(new Paragraph());
                txtContent.CaretPosition = txtContent.Document.ContentEnd;
                txtContent.Focus();
            }
            else
            {
                editing.Strokes = inkDrawBoard.Strokes.Clone();
                editing = null;
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            StringWriter wr = new StringWriter();
            XamlWriter.Save(txtContent.Document, wr);
            Settings.Default["fakeDB"] = wr.ToString();
            Settings.Default.Save();
        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            FlowDocument doc;

            if (TryParseFlowDocument(Settings.Default["fakeDB"].ToString(), out doc))
            {
                txtContent.Document = doc;
                foreach(InkCanvas ic in LogicalTreeUtility.GetChildren<InkCanvas>(txtContent.Document,true))
                {
                    ic.PreviewMouseLeftButtonDown += EditingInkCanvas_PreviewMouseLeftButtonDown;
                }
            }
            else
            {
                txtContent.Document = new FlowDocument();
            }
        }

        private void EditingInkCanvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            InkCanvas ic = sender as InkCanvas;

            wrapDrawingTool.Visibility = Visibility.Visible;
            wrapRichTextTool.Visibility = Visibility.Collapsed;
            inkDrawBoard.Visibility = Visibility.Visible;
            txtContent.Visibility = Visibility.Collapsed;

            inkDrawBoard.Strokes = ic.Strokes.Clone();
            editing = ic;
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

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }

        private void btnBlack_Click(object sender, RoutedEventArgs e)
        {
            inkDrawBoard.EditingMode = InkCanvasEditingMode.Ink;
            inkDrawBoard.DefaultDrawingAttributes.Color = Colors.Black;
        }

        private void btnRed_Click(object sender, RoutedEventArgs e)
        {
            inkDrawBoard.EditingMode = InkCanvasEditingMode.Ink;
            inkDrawBoard.DefaultDrawingAttributes.Color = Colors.Red;
        }

        private void btnBlue_Click(object sender, RoutedEventArgs e)
        {
            inkDrawBoard.EditingMode = InkCanvasEditingMode.Ink;
            inkDrawBoard.DefaultDrawingAttributes.Color = Colors.Blue;
        }

        private void btnEraser_Click(object sender, RoutedEventArgs e)
        {
            inkDrawBoard.EditingMode = InkCanvasEditingMode.EraseByPoint;
        }

        private void btnDeleteDraw_Click(object sender, RoutedEventArgs e)
        {
            wrapDrawingTool.Visibility = Visibility.Collapsed;
            wrapRichTextTool.Visibility = Visibility.Visible;
            wrapSaveLoad.Visibility = Visibility.Visible;
            inkDrawBoard.Visibility = Visibility.Collapsed;
            txtContent.Visibility = Visibility.Visible;

            BlockUIContainer container = LogicalTreeUtility.FindAncestorOrSelf<BlockUIContainer>(editing);
            txtContent.Document.Blocks.Remove(container);
            editing = null;
        }
    }
}
