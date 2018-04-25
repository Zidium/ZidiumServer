namespace Zidium.Api
{
    public class ApplicationErrorData : SendEventBaseT<ApplicationErrorData>
    {
        public string Stack
        {
            get
            {
                return Properties[ExtentionPropertyName.Stack];
            }
            set
            {
                Properties[ExtentionPropertyName.Stack] = value;
            }
        }

        public ApplicationErrorData(IComponentControl componentControl, string errorTypeSystemName)
            : base(componentControl, errorTypeSystemName)
        {
        }

        public override SendEventCategory EventCategory
        {
            get { return SendEventCategory.ApplicationError; }
        }
    }
}
