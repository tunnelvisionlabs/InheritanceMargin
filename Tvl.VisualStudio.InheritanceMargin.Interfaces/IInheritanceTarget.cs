// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Tvl.VisualStudio.InheritanceMargin
{
    /// <summary>
    /// Represents the target of an inheritance relation.
    /// </summary>
    public interface IInheritanceTarget
    {
        /// <summary>
        /// Gets the display name of the target of the inheritance relation.
        /// </summary>
        /// <value>
        /// The display name of the target of the inheritance relation.
        /// </value>
        string DisplayName
        {
            get;
        }

        /// <summary>
        /// Navigates to the target of the inheritance relation.
        /// </summary>
        void NavigateTo();
    }
}
