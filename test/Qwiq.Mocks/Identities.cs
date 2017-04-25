using System;

namespace Microsoft.Qwiq.Mocks
{
    public static class Identities
    {
        public static readonly ITeamFoundationIdentity Danj = new MockTeamFoundationIdentity(
                                                                                             MockIdentityDescriptor.Create("danj", "contoso.com"),
                                                                                             "Dan Jump",
                                                                                             Guid.Parse("b7de08a6-8417-491b-be62-85945a538f46"));

        public static readonly ITeamFoundationIdentity Adamb = new MockTeamFoundationIdentity(
                                                                                              MockIdentityDescriptor.Create("adamb", "contoso.com"),
                                                                                              "Adam Barr",
                                                                                              Guid.Parse("7846c22f-d3d8-4e02-8b62-d055d0284783"));

        public static readonly ITeamFoundationIdentity Chrisj = new MockTeamFoundationIdentity(
                                                                                               MockIdentityDescriptor.Create("chrisj", "contoso.com"),
                                                                                               "Chris Johnson",
                                                                                               Guid.Parse("f92c1baa-0038-4247-be68-12043fcc34e3"),
                                                                                               false);

        public static readonly ITeamFoundationIdentity Chrisjoh = new MockTeamFoundationIdentity(
                                                                                                 MockIdentityDescriptor.Create("chrisjoh", "contoso.com"),
                                                                                                 "Chris Johnson (FINANCE)",
                                                                                                 Guid.Parse("41e97533-89f7-45d7-8246-eaa449b5651d"));

        public static readonly ITeamFoundationIdentity Chrisjohn = new MockTeamFoundationIdentity(
                                                                                                  MockIdentityDescriptor.Create("chrisjohn", "contoso.com"),
                                                                                                  "Chris F. Johnson",
                                                                                                  Guid.Parse("b3da460c-6191-4725-b08d-52bba48a574f"));

        public static readonly ITeamFoundationIdentity Chrisjohns = new MockTeamFoundationIdentity(
                                                                                                   MockIdentityDescriptor.Create("chrisjohns", "contoso.com"),
                                                                                                   "Chris Johnson <chrisjohns@contoso.com>",
                                                                                                   Guid.Parse("67b42b6c-6bd8-40e2-a622-fe69eacd3d47"));

        public static readonly ITeamFoundationIdentity[] All = {
                                                                       Danj,
                                                                       Adamb,
                                                                       Chrisj,
                                                                       Chrisjoh,
                                                                       Chrisjohn,
                                                                       Chrisjohns
                                                                   };
    }
}