﻿using System;
using Tfs = Microsoft.TeamFoundation;

namespace Microsoft.IE.Qwiq
{
    public class TfsTeamProjectCollectionProxy : ITfsTeamProjectCollection
    {
        private readonly Tfs.Client.TfsTeamProjectCollection _tfs;

        internal TfsTeamProjectCollectionProxy(Tfs.Client.TfsTeamProjectCollection tfs)
        {
            _tfs = tfs;
        }

        public IIdentityManagementService IdentityManagementService
        {
            get
            {
                return new IdentityManagementServiceProxy(
                        _tfs.GetService<Tfs.Framework.Client.IIdentityManagementService2>());
            }
        }

        public ICommonStructureService CommonStructureService
        {
            get { return new CommonStructureServiceProxy(_tfs.GetService<Tfs.Server.ICommonStructureService4>()); }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_tfs != null)
                {
                    _tfs.Dispose();
                }
            }
        }
    }
}