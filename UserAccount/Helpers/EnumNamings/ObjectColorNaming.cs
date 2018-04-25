using Zidium.Core.Common;

namespace Zidium.UserAccount.Helpers
{
    public class ObjectColorNaming : IEnumNaming<ObjectColor>
    {
        public string Name(ObjectColor value)
        {
            if (value == ObjectColor.Red)
                return "Красный";
            if (value == ObjectColor.Yellow)
                return "Жёлтый";
            if (value == ObjectColor.Green)
                return "Зелёный";
            if (value == ObjectColor.Gray)
                return "Серый";
            return value.ToString();
        }
    }
}