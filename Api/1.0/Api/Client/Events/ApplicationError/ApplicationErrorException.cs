using System;

namespace Zidium.Api
{
    public class ApplicationErrorException : Exception
    {
        public string Type { get; protected set; }

        public long? JoinKeyHash { get; set; }

        public ExtentionPropertyCollection Properties { get; protected set; }

        public ApplicationErrorException(string type) : this(type, type)
        {
        }

        public ApplicationErrorException(string type, Exception innerException)
            : this(type, type, innerException)
        {
        }

        public ApplicationErrorException(string type, string message)
            : base(message)
        {
            Type = type;
            Properties = new ExtentionPropertyCollection();
        }

        public ApplicationErrorException(string type, string message, Exception innerException)
            : base(message, innerException)
        {
            Type = type;
            Properties = new ExtentionPropertyCollection();
        }

        #region set extention properties

        public ApplicationErrorException SetProperty(string name, string value)
        {
            Properties[name] = value;
            return this;
        }

        public ApplicationErrorException SetProperty(string name, int value)
        {
            Properties[name] = value;
            return this;
        }

        public ApplicationErrorException SetProperty(string name, long value)
        {
            Properties[name] = value;
            return this;
        }

        public ApplicationErrorException SetProperty(string name, DateTime value)
        {
            Properties[name] = value;
            return this;
        }

        public ApplicationErrorException SetProperty(string name, double value)
        {
            Properties[name] = value;
            return this;
        }

        public ApplicationErrorException SetProperty(string name, Guid value)
        {
            Properties[name] = value;
            return this;
        }

        public ApplicationErrorException SetProperty(string name, bool value)
        {
            Properties[name] = value;
            return this;
        }

        public ApplicationErrorException SetProperty(string name, byte[] value)
        {
            Properties[name] = value;
            return this;
        }

        #endregion 
    }
}
