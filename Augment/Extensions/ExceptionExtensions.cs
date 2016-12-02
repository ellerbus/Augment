using System;
using System.Collections;
using System.Xml.Linq;
using EnsureThat;

namespace Augment
{
    /// <summary>
    /// Handy Exception Extensions
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Serialize an Exception to XML (handy for serializing to a log or database)
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="includeStackTrace"></param>
        /// <returns></returns>
        /// <remarks>
        /// Concept: https://seattlesoftware.wordpress.com/2008/08/22/serializing-exceptions-to-xml/
        /// </remarks>
        public static XElement ToXml(this Exception exp, bool includeStackTrace = true)
        {
            Ensure.That(exp).IsNotNull();

            XElement root = new XElement(exp.GetType().ToString());

            if (exp.Message.IsNotEmpty())
            {
                root.Add(new XElement("Message", exp.Message));
            }

            if (includeStackTrace && exp.StackTrace.IsNotEmpty())
            {
                root.Add(new XElement("StackTrace", exp.StackTrace));
            }

            if (exp.Data != null && exp.Data.Count > 0)
            {
                XElement data = new XElement("Data");

                root.Add(data);

                foreach (DictionaryEntry entry in exp.Data)
                {
                    string key = entry.Key.ToString();
                    string value = entry.Value == null ? "null" : entry.Value.ToString();

                    data.Add(new XElement(key, value));
                }
            }

            if (exp.InnerException != null)
            {
                root.Add(exp.InnerException.ToXml(includeStackTrace));
            }

            return root;
        }
    }
}