namespace Zidium.UserAccount.Models.Controls
{
    public class KeyValueRowModel
    {
        public string Id { get; set; }

        public string CollectionName { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public bool HasId
        {
            get { return string.IsNullOrEmpty(Id) == false; }
        }

        public bool HasKey
        {
            get { return string.IsNullOrEmpty(Key) == false; }
        }

        public void Trim()
        {
            if (Key != null)
            {
                Key = Key.Trim();
            }
            if (Value != null)
            {
                Value = Value.Trim();
            }
        }
    }
}