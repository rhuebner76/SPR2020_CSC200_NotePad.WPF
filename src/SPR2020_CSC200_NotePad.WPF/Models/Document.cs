
namespace SPR2020_CSC200_NotePad.WPF.Models
{
    /// <summary>
    /// Represents a document
    /// </summary>
    [System.Serializable]  
    public class Document
    {
        // NOTE: to use binary or soap formatting for serialization the [Serializable] attribute is required

        /// <summary>
        /// Gets, sets the content of the document
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Gets, sets the date and time last saved
        /// </summary>
        public System.DateTime LastSaved { get; set; }

        /// <summary>
        /// Default, parameter-less constructor
        /// </summary>
        public Document()
        {
            Content = string.Empty;
            LastSaved = System.DateTime.Now;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="content">Content for instance</param>
        public Document(string content)
        {
            Content = content;
            LastSaved = System.DateTime.Now;
        }
    }
}
