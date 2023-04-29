using System;

namespace Carotaa.Code
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class PageAddressAttribute: Attribute
    {
        public readonly string Address;
        public PageAddressAttribute(string address)
        {
            Address = address;
        }
    }
}