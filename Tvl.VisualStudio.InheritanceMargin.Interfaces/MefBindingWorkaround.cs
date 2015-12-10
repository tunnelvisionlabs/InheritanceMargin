// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Microsoft Reciprocal License (MS-RL). See LICENSE in the project root for license information.

namespace Tvl.VisualStudio.InheritanceMargin.Interfaces
{
    using System.ComponentModel.Composition;

    /// <summary>
    /// This class exists for the sole purpose of working around issue
    /// https://github.com/tunnelvisionlabs/InheritanceMargin/issues/6.
    /// </summary>
    /// <remarks>
    /// <para>Applying the <see cref="ExportAttribute"/> is required for the affected version(s) of Visual Studio to
    /// place this interfaces assembly in the MEF cache.</para>
    /// </remarks>
    [Export]
    internal class MefBindingWorkaround
    {
    }
}
