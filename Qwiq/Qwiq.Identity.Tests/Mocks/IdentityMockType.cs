using System;
using Microsoft.IE.Qwiq.Identity.Attributes;
using Microsoft.IE.Qwiq.Mapper.Attributes;

namespace Qwiq.Identity.Tests.Mocks
{
    public class MockIdentityType
    {
        private string _anIdentity;

        internal const string BackingField = "Identity WorkItemField";
        internal const string NonExistantField = "Non Existant Identity WorkItemField";
        internal const string UriIdentityField = "Uri Identity WorkItemField";
        internal int AnIdentitySetCount;

        [FieldDefinition(BackingField)]
        [IdentityField]
        public string AnIdentity
        {
            get { return _anIdentity; }
            set
            {
                AnIdentitySetCount++;
                _anIdentity = value;
            }
        }

        [IdentityField]
        public string NoBacking { get; set; }

        [FieldDefinition("NotAnIdentityField")]
        public string NotAnIdentity { get; set; }

        [IdentityField]
        [FieldDefinition("")]
        public string Empty { get; set; }

        [IdentityField]
        [FieldDefinition(NonExistantField)]
        public string NonExistant { get; set; }

        [FieldDefinition("PrivateIdentityField")]
        [IdentityField]
        public string PrivateIdentity { get; }

        [FieldDefinition(UriIdentityField)]
        [IdentityField]
        public Uri UriIdentity { get; set; }
    }
}
