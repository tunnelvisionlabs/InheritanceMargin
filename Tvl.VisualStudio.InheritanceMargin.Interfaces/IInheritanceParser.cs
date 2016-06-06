// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Tvl.VisualStudio.InheritanceMargin
{
    using System;

    /// <summary>
    /// Defines the common interface by which the Visual Studio extension interacts with the various inheritance
    /// relation analysis implementations which each target a specific version of Visual Studio.
    /// </summary>
    public interface IInheritanceParser
    {
        /// <summary>
        /// Raised after a background inheritance parse operation completes.
        /// </summary>
        event EventHandler<InheritanceParseResultEventArgs> ParseComplete;

        /// <summary>
        /// Requests a background parse operation.
        /// </summary>
        /// <param name="forceReparse"><see langword="true"/> to request a new parse result even if the content of the
        /// document has not changed; otherwise, <see langword="false"/>.</param>
        void RequestParse(bool forceReparse);
    }
}
