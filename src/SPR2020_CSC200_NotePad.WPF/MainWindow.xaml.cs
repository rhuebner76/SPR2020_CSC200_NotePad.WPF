using System;
using System.Windows;

namespace SPR2020_CSC200_NotePad.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string FilterTxt = "Text file (*.txt)|*.txt";
        private const string FilterJson = "JSON file (*.json)|*.json";
        private const string FilterXml = "XML file (*.xml)|*.xml";
        private const string FilterSoap = "SOAP file (*.soap)|*.soap";
        private const string FilterBinary = "Binary file (*.bin)|*.bin";
        private const string FilterAny = "Any files (*.*)|*.*";
        
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnMainFileOpenMenuClicked(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();

            dialog.Filter = FilterTxt + "|" +
                            FilterJson + "|" +
                            FilterXml + "|" +
                            FilterSoap + "|" +
                            FilterBinary + "|" +
                            FilterAny;

            dialog.FilterIndex = 1;

            bool? dialogResult = dialog.ShowDialog();
            if (dialogResult == true)
            {
                string filePath = dialog.FileName;
                if (System.IO.File.Exists(filePath))
                {
                    string content = System.IO.File.ReadAllText(filePath);

                    // determine which format the user want to save as
                    if (dialog.FilterIndex == 1)
                    {
                        // read as txt
                        System.IO.StringReader reader = new System.IO.StringReader(content);
                        string line = reader.ReadLine();
                        System.DateTime lastSaved;
                        if (System.DateTime.TryParse(line, out lastSaved))
                        {
                            // we read the line, so remove the line
                            line = string.Empty; 
                        }
                        else
                        {
                            lastSaved = System.DateTime.Now;
                        }

                        content = line + reader.ReadToEnd();
                        Models.Document document = new Models.Document(content);
                        document.LastSaved = lastSaved;
                        content = document.Content;

                        reader.Dispose();
                    }
                    else if (dialog.FilterIndex == 2)
                    {
                        // read as JSON
                        string json = content;
                        // deserialize to expected type (Models.Document)
                        object jsonObject = Newtonsoft.Json
                            .JsonConvert
                            .DeserializeObject(json, typeof(Models.Document));
                        // cast and assign JSON object to expected type (Models.Document)
                        Models.Document document = (Models.Document)jsonObject;
                        // assign content from deserialized Models.Document
                        content = document.Content;
                    }
                    else if (dialog.FilterIndex == 3)
                    {
                        // read as XML for type of Models.Document
                        System.Xml.Serialization.XmlSerializer serializer =
                            new System.Xml.Serialization.XmlSerializer(typeof(Models.Document));

                        // convert content to byte array (sequence of bytes)
                        byte[] buffer = System.Text.Encoding.ASCII.GetBytes(content);
                        // make stream from buffer
                        System.IO.MemoryStream stream = new System.IO.MemoryStream(buffer);
                        // deserialize stream to an object
                        object xmlObject = serializer.Deserialize(stream);
                        // cast and assign XML object to actual type object
                        Models.Document document = (Models.Document)xmlObject;

                        content = document.Content;
                        stream.Dispose();   // release the resources
                    }
                    else if (dialog.FilterIndex == 4)
                    {
                        // read as soap
                        System.Runtime.Serialization.Formatters.Soap.SoapFormatter serializer =
                            new System.Runtime.Serialization.Formatters.Soap.SoapFormatter();

                        // convert content to byte array (sequence of bytes)
                        byte[] buffer = System.Text.Encoding.ASCII.GetBytes(content);
                        // make stream from buffer
                        System.IO.MemoryStream stream = new System.IO.MemoryStream(buffer);
                        // deserialize stream to an object
                        object soapObject = serializer.Deserialize(stream);
                        // cast and assign SOAP object to actual type object
                        Models.Document document = (Models.Document) soapObject;
                        // read content
                        content = document.Content;
                        
                        stream.Dispose();   // release the resources
                    }

                    else if (dialog.FilterIndex == 5)
                    {
                        // read as binary
                        System.Runtime.Serialization.Formatters.Binary.BinaryFormatter serializer =
                            new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                        // reading and writing binary data directly as string has issues, try not to do it 
                        byte[] buffer = Convert.FromBase64String(content);
                        System.IO.MemoryStream stream = new System.IO.MemoryStream(buffer);
                        // deserialize stream to object
                        object binaryObject = serializer.Deserialize(stream);
                        // assign binary object to actual type object
                        Models.Document document = (Models.Document)binaryObject;
                        // read the content
                        content = document.Content;
                        stream.Dispose();   // release the resources
                    }
                    else // imply this is any file 
                    {
                        // read as is
                    }

                    // assign content to UI control
                    UserText.Text = content;
                }
            }
        }

        private void OnMainFileSaveMenuClicked(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.SaveFileDialog dialog = new Microsoft.Win32.SaveFileDialog();
            
            dialog.Filter = FilterTxt + "|" +
                            FilterJson + "|" +
                            FilterXml + "|" +
                            FilterSoap + "|" +
                            FilterBinary;

            dialog.FilterIndex = 1;

            bool? dialogResult = dialog.ShowDialog();
            if (dialogResult == true)
            {
                string filePath = dialog.FileName;
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // get content from UI control
                string content = UserText.Text;

                // determine which format the user want to save as
                if (dialog.FilterIndex == 1)
                {
                    // write as txt
                    System.IO.StringWriter writer = new System.IO.StringWriter();
                    writer.WriteLine(DateTime.Now);   // write last saved
                    writer.Write(content);            // write content 
                    content = writer.ToString();      // assign assembled content

                    writer.Dispose();                 // release writer
                }
                else if (dialog.FilterIndex == 2)
                {
                    // write as JSON
                    // create object to serialize
                    Models.Document document = new Models.Document(content);
                    // serialize type (Models.Document) to JSON string 
                    string json = Newtonsoft
                        .Json
                        .JsonConvert
                        .SerializeObject(document);
                    // set content to JSON result
                    content = json;                   // assign JSON string to content
                }
                else if (dialog.FilterIndex == 3)
                {
                    // write as XML
                    // create object to serialize
                    Models.Document document = new Models.Document(content);
                    // create serializer
                    System.Xml.Serialization.XmlSerializer serializer =
                        new System.Xml.Serialization.XmlSerializer(typeof(Models.Document));
                    // this serializer writes to a stream
                    System.IO.MemoryStream stream = new System.IO.MemoryStream();
                    serializer.Serialize(stream, document);
                    stream.Seek(0, System.IO.SeekOrigin.Begin);   // reset stream to start

                    // read content from stream
                    System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                    content = reader.ReadToEnd();   // assign XML string to content

                    reader.Dispose();   // dispose the reader
                    stream.Dispose();   // dispose the stream
                }
                else if (dialog.FilterIndex == 4)
                {
                    // write as SOAP
                    // note: have to add a reference to a global assembly (dll) to use in project 
                    // create object to serialize
                    Models.Document document = new Models.Document(content);
                    // create serializer
                    System.Runtime.Serialization.Formatters.Soap.SoapFormatter serializer =
                        new System.Runtime.Serialization.Formatters.Soap.SoapFormatter();
                    // this serializer writes to a stream
                    System.IO.MemoryStream stream = new System.IO.MemoryStream();
                    serializer.Serialize(stream, document);
                    stream.Seek(0, System.IO.SeekOrigin.Begin);   // reset stream to start

                    // read content from stream
                    System.IO.StreamReader reader = new System.IO.StreamReader(stream);
                    content = reader.ReadToEnd();   // assign SOAP string to content

                    reader.Dispose();   // dispose the reader
                    stream.Dispose();   // dispose the stream

                }
                else // implies this is last one (binary)
                {
                    // write as binary
                    // create object to serialize
                    Models.Document document = new Models.Document(content);
                    // create serializer
                    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter serializer =
                        new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    // this serializer writes to a stream
                    System.IO.MemoryStream stream = new System.IO.MemoryStream();
                    serializer.Serialize(stream, document);

                    stream.Seek(0, System.IO.SeekOrigin.Begin);   // reset stream to start
                    // reading and writing binary data directly as string has issues, try not to do it 
                    content = Convert.ToBase64String(stream.ToArray());    // assign base64 to content

                    stream.Dispose();   // dispose the stream
                }

                // write content to file
                System.IO.File.WriteAllText(filePath, content);
            }
        }

        private void OnMainHelpAboutMenuClicked(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("Simple Notepad",
                "About",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }

        private void OnMainFileExitMenuClicked(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
