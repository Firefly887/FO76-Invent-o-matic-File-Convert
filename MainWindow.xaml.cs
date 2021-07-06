using System;
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
using System.IO;
using static System.IO.File;
using Microsoft.Win32;
using System.Xml;
using System.Xml.Serialization;


namespace LoopThroughJSONToXML
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public class Sequence
    {
        public Point[] SourcePath { get; set; }
    }

    public partial class MainWindow : Window
    {
        private string fileName;
        public MainWindow()
        {
            InitializeComponent();
            buttonExecute.IsEnabled = false;
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog(this) == true)
            {
                LocationBox.Text = dialog.FileName;
            }
        }

        private void LocationBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            buttonExecute.IsEnabled = !string.IsNullOrEmpty(LocationBox.Text);
            if(LocationBox.Text != null)
                fileName = LocationBox.Text;
        }

        private void buttonExecute_Click(object sender, RoutedEventArgs e)
        {
            // Load the json document.
            textBlockResults2.Text = "Conversion started...";
            try
            {
                string json = ReadAllText(fileName);
                textBlockResults2.Text += "\nFile successfully Read";
                XmlDocument document = Newtonsoft.Json.JsonConvert.DeserializeXmlNode(json, "root");
                textBlockResults2.Text += "\nfile deserialized.";
                StringBuilder sb = new StringBuilder();
                sb.Append(FormatText(document.DocumentElement as XmlNode, "", ""));
                textBlockResults2.Text += "\nAdjusting for illegal characters...";
                string temp = sb.Replace("&", "and").ToString();
                textBlockResults.Text = temp;
                textBlockResults2.Text += "\nFile successfully converted.";
                AppendAllText(@".\Result\Character_Inventory.xml", textBlockResults.Text);
                textBlockResults2.Text += "\nFileSaved.";
            }
            catch (Exception ex)
            {
                textBlockResults.Text = "A error occured...\n" + ex.Message;
            }
        }

        private string FormatText(XmlNode node, string text, string indent)
        {

            if (node is XmlText)
            {
                text += node.Value;
                return text;
            }
            
            if (string.IsNullOrEmpty(indent))
                indent = "";
            else
            {
                text += "\n\n" + indent;
            }
            if (node is XmlComment)
            {
                text += node.OuterXml;
                return text;
            }
            text += "<" + node.Name;
            if (node.Attributes.Count > 0)
            {
                AddAttributes(node, ref text);
            }
            if (node.HasChildNodes)
            {
                text += ">";
                foreach (XmlNode child in node.ChildNodes)
                {
                    text = FormatText(child, text, indent + " ");
                }
                if (node.ChildNodes.Count == 1 && (node.FirstChild is XmlText || node.FirstChild is XmlComment))
                    text += "</" + node.Name + ">";
                else
                    text += "\n\n" + indent + "</" + node.Name + ">";
            }
            else
                text += "/>";
            return text;
        }
        private void AddAttributes(XmlNode node, ref string text)
        {
            foreach (XmlAttribute xa in node.Attributes)
            {
                text += " " + xa.Name + "='" + xa.Value + "'";
            }
        }
    }
}
