// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Tvl.VisualStudio.InheritanceMargin.CSharp
{
    using System.Collections.Generic;
    using Microsoft.RestrictedUsage.CSharp.Syntax;

    internal class TypeDeclarationNodeSelector : ParseTreeVisitor
    {
        private readonly List<TypeDeclarationNode> _nodes = new List<TypeDeclarationNode>();

        public List<TypeDeclarationNode> Nodes
        {
            get
            {
                return _nodes;
            }
        }

        /// <inheritdoc/>
        public override bool TraverseInteriorTree(ParseTreeNode node)
        {
            return false;
        }

        /// <inheritdoc/>
        public override void VisitTypeDeclarationNode(TypeDeclarationNode node)
        {
            _nodes.Add(node);
            base.VisitTypeDeclarationNode(node);
        }
    }
}
