// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Microsoft Reciprocal License (MS-RL). See LICENSE in the project root for license information.

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

        public override bool TraverseInteriorTree(ParseTreeNode node)
        {
            return false;
        }

        public override void VisitTypeDeclarationNode(TypeDeclarationNode node)
        {
            _nodes.Add(node);
            base.VisitTypeDeclarationNode(node);
        }
    }
}
