// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Microsoft Reciprocal License (MS-RL). See LICENSE in the project root for license information.

namespace Tvl.VisualStudio.InheritanceMargin
{
    using System;
    using Microsoft.VisualStudio.Shell.Interop;

    // Stolen from Microsoft.RestrictedUsage.CSharp.Utilities in Microsoft.VisualStudio.CSharp.Services.Language.dll
    internal static class RoslynUtilities
    {
        private static bool? _roslynInstalled;

        public static bool IsRoslynInstalled(IServiceProvider serviceProvider)
        {
            if (_roslynInstalled.HasValue)
                return _roslynInstalled.Value;

            _roslynInstalled = false;
            if (serviceProvider == null)
                return false;

            IVsShell vsShell = serviceProvider.GetService(typeof(SVsShell)) as IVsShell;
            if (vsShell == null)
                return false;

            Guid guid = new Guid("6cf2e545-6109-4730-8883-cf43d7aec3e1");
            int isInstalled;
            if (vsShell.IsPackageInstalled(ref guid, out isInstalled) == 0 && isInstalled != 0)
                _roslynInstalled = true;

            return _roslynInstalled.Value;
        }
    }
}
