// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Microsoft Reciprocal License (MS-RL). See LICENSE in the project root for license information.

namespace Tvl.VisualStudio.InheritanceMargin
{
    using System;
    using System.Collections.Generic;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;

    /// <summary>
    /// Represents the result of a background parse operation which collections information about type and member
    /// inheritance.
    /// </summary>
    public class InheritanceParseResultEventArgs : EventArgs
    {
        private readonly ITextSnapshot _snapshot;
        private readonly TimeSpan _elapsedTime;
        private readonly IEnumerable<ITagSpan<IInheritanceTag>> _tags;

        /// <summary>
        /// Initializes a new instance of the <see cref="InheritanceParseResultEventArgs"/> class.
        /// </summary>
        /// <param name="snapshot">The snapshot which was analyzed.</param>
        /// <param name="elapsedTime">The total time taken to analyze the snapshot.</param>
        /// <param name="tags">A collection of inheritance tags collected for the snapshot.</param>
        public InheritanceParseResultEventArgs(ITextSnapshot snapshot, TimeSpan elapsedTime, IEnumerable<ITagSpan<IInheritanceTag>> tags)
        {
            this._snapshot = snapshot;
            this._elapsedTime = elapsedTime;
            this._tags = tags;
        }

        /// <summary>
        /// Gets the text snapshot which was analyzed.
        /// </summary>
        /// <value>
        /// The <see cref="ITextSnapshot"/> which was analyzed.
        /// </value>
        public ITextSnapshot Snapshot
        {
            get
            {
                return _snapshot;
            }
        }

        /// <summary>
        /// Gets the total time taken to analyze the snapshot.
        /// </summary>
        /// <value>
        /// The total time taken to analyze the snapshot.
        /// </value>
        public TimeSpan ElapsedTime
        {
            get
            {
                return _elapsedTime;
            }
        }

        /// <summary>
        /// Gets a collection of inheritance tags describing the inheritance relations of types and members located
        /// within the snapshot.
        /// </summary>
        /// <value>
        /// A collection of inheritance tags describing the inheritance relations of types and members located within
        /// the snapshot.
        /// </value>
        public IEnumerable<ITagSpan<IInheritanceTag>> Tags
        {
            get
            {
                return _tags;
            }
        }
    }
}
