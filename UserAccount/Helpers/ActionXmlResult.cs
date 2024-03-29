﻿using Microsoft.AspNetCore.Mvc;

namespace Zidium.UserAccount.Helpers
{
    public class ActionXmlResult : ActionResult
    {
        private object objectToSerialize;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionXmlResult"/> class.
        /// </summary>
        /// <param name="objectToSerialize">The object to serialize to XML.</param>
        public ActionXmlResult(object objectToSerialize)
        {
            this.objectToSerialize = objectToSerialize;
        }

        /// <summary>
        /// Gets the object to be serialized to XML.
        /// </summary>
        public object ObjectToSerialize
        {
            get { return this.objectToSerialize; }
        }

        /// <summary>
        /// Serialises the object that was passed into the constructor to XML and writes the corresponding XML to the result stream.
        /// </summary>
        /// <param name="context">The controller context for the current request.</param>
        public override void ExecuteResult(ActionContext context)
        {
            if (this.objectToSerialize != null)
            {
                var xs = new System.Xml.Serialization.XmlSerializer(objectToSerialize.GetType());
                context.HttpContext.Response.ContentType = "text/xml";
                xs.Serialize(context.HttpContext.Response.Body, objectToSerialize);
            }
        }
    }
}