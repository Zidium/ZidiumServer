using Zidium.Api.Others;

namespace Zidium.Api
{
    public class ExtentionProperty
    {
        public string Name { get; protected set; }

        public ExtentionPropertyValue Value { get; set; }

        public ExtentionProperty(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return string.Format("{0} ({1}) = {2}", Name, Value.DataType, Value.Value);
        }

        public int GetInternalSize()
        {
            int length = StringHelper.GetLengthInMemory(Name) + 10; // 10 = это указание типа в веб-сообщении
            if (Value == null || Value.HasValue() == false)
            {
                return length;
            }

            int valueLength = 0;
            switch (Value.DataType)
            {
                case DataType.Binary:
                    byte[] bytes = Value;
                    valueLength = bytes.Length;
                    break;
                case DataType.DateTime:
                    valueLength = 25;
                    break;
                default:
                    valueLength = Value.Value.ToString().Length;
                    break;
            }
            return length + valueLength;
        }

        public int GetWebSize()
        {
            int length = StringHelper.GetLengthInMemory(Name) + 10; // 10 = это указание типа в веб-сообщении
            if (Value == null || Value.HasValue() == false)
            {
                return length;
            }

            int valueLength = 0;
            switch (Value.DataType)
            {
                case DataType.Binary:
                    byte[] bytes = Value;
                    valueLength = bytes.Length * 2; // потому что будет base64 использоваться
                    break;
                case DataType.DateTime:
                    valueLength = 25;
                    break;
                default:
                    valueLength = Value.Value.ToString().Length;
                    break;
            }
            return length + valueLength;
        }
    }
}
