using System;

namespace FieldFallback.Processors.Events
{
    public class FieldFallBackItemDeletedHandler
    {
        public void OnDeleteItem(object sender, EventArgs args)
        {
            //The item will be deleted when the template is deleted
            throw new NotImplementedException("This will get enabled shortly.");
        }
    }
}