// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Tvl.VisualStudio.InheritanceMargin
{
    using Guid = System.Guid;

    internal static class InheritanceMarginConstants
    {
        public const string GuidInheritanceMarginPackageString = "B03A0D8A-A6E0-4983-B545-F73D2531D534";

        public const string GuidInheritanceMarginCommandSetString = "102A7E39-1CD8-4F49-816E-245D813D884E";
        public static readonly Guid GuidInheritanceMarginCommandSet = new Guid("{" + GuidInheritanceMarginCommandSetString + "}");

        public static readonly int MenuInheritanceTargets = 0x0100;
        public static readonly int GroupInheritanceTargets = 0x0101;
        public static readonly int CmdidInheritanceTargetsList = 0x0200;
        public static readonly int CmdidInheritanceTargetsListEnd = 0x02FF;
    }
}
