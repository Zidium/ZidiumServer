using Microsoft.AspNetCore.Mvc;

namespace Zidium.UserAccount.Helpers
{
    public class ActionXmlFileResult : ActionResult
    {
        private object objectToSerialize;

        private string fileName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionXmlResult"/> class.
        /// </summary>
        /// <param name="objectToSerialize">The object to serialize to XML.</param>
        public ActionXmlFileResult(object objectToSerialize, string fileName)
        {
            this.objectToSerialize = objectToSerialize;
            this.fileName = fileName;
        }

        /// <summary>
        /// Gets the object to be serialized to XML.
        /// </summary>
        public object ObjectToSerialize
        {
            get { return objectToSerialize; }
        }

        /// <summary>
        /// Serialises the object that was passed into the constructor to XML and writes the corresponding XML to the result stream.
        /// </summary>
        /// <param name="context">The controller context for the current request.</param>
        public override void ExecuteResult(ActionContext context)
        {
            if (objectToSerialize != null)
            {
                context.HttpContext.Response.Headers.Add("content-disposition", "attachment; filename=" + fileName);
                var xs = new System.Xml.Serialization.XmlSerializer(objectToSerialize.GetType());
                context.HttpContext.Response.ContentType = "text/xml";
                xs.Serialize(context.HttpContext.Response.Body, objectToSerialize);
            }
        }
    }
}