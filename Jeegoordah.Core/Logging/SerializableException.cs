using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Common.Logging
{
    /// <summary>
    /// A wrapper class for serializing exceptions. 
    /// XmlSerializer can't serialize Data property of Exception class.
    /// </summary>
    [Serializable]
    [XmlType(AnonymousType = true)]
    [XmlRoot(IsNullable = false)]
    public class SerializableException
    {
        /// <summary>
        /// Required by serializer
        /// </summary>
        public SerializableException()
        {            
        }

        public SerializableException(Exception exception) : this()
        {
            if (exception == null) throw new ArgumentNullException("exception");
            TypeName = exception.GetType().FullName;
            HelpLink = exception.HelpLink;
            Message = exception.Message;
            Source = exception.Source;
            StackTrace = exception.StackTrace;
            SetData(exception.Data);
            if (exception.InnerException != null)
                InnerException = new SerializableException(exception.InnerException);
        }

        public string TypeName { get; set; }
        public string HelpLink { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
        public string StackTrace { get; set; }        
        public SerializableException InnerException { get; set; }
        public DictionaryEntry[] Data { get; set; }

        private void SetData(ICollection collection)
        {
            if (collection == null || collection.Count == 0) return;
            Data = new DictionaryEntry[collection.Count];
            collection.CopyTo(Data, 0);
        }
    }
}
