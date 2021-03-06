using System;

using Qwiq.Mapper.Attributes;

namespace Qwiq.Identity.Mocks
{
    public class MockIdentityType : IIdentifiable<int?>
    {
        internal const string BackingField = "Identity WorkItemField";

        internal const string NonExistantField = "Non Existant Identity WorkItemField";

        internal const string UriIdentityField = "Uri Identity WorkItemField";

        private string _anIdentity;

        internal int AnIdentitySetCount;

        [FieldDefinition(BackingField)]
        [IdentityField]
        public string AnIdentity
        {
            get => _anIdentity;
            set
            {
                AnIdentitySetCount++;
                _anIdentity = value;
            }
        }

        [FieldDefinition(BackingField)]
        [IdentityField]
        public IdentityFieldValue AnIdentityValue { get;set;}

        [IdentityField]
        public string NoBacking { get; set; }

        [IdentityField]
        [FieldDefinition(NonExistantField)]
        public string NonExistant { get; set; }

        [FieldDefinition("NotAnIdentityField")]
        public string NotAnIdentity { get; set; }

        [FieldDefinition("PrivateIdentityField")]
        [IdentityField]
        public string PrivateIdentity { get; }

        [FieldDefinition(UriIdentityField)]
        [IdentityField]
        public Uri UriIdentity { get; set; }

        [FieldDefinition("Work Item Id WorkItemField")]
        public int? Id { get; }
    }
}