﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Tvl.VisualStudio.InheritanceMargin.CSharp
{
    using System.Collections.Generic;
    using Microsoft.RestrictedUsage.CSharp.Syntax;
    using IEnumerable = System.Collections.IEnumerable;
    using IEnumerator = System.Collections.IEnumerator;

    internal class TypeCollector : ParseTreeVisitor, IEnumerable<ParseTreeNode>
    {
        private readonly List<ParseTreeNode> _nodes = new List<ParseTreeNode>();

        public TypeCollector(ParseTreeNode node)
        {
            Visit(node);
        }

        /// <inheritdoc/>
        public IEnumerator<ParseTreeNode> GetEnumerator()
        {
            return _nodes.GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc/>
        public override bool TraverseInteriorTree(ParseTreeNode node)
        {
            return false;
        }

        /// <inheritdoc/>
        public override void VisitAccessorDeclarationNode(AccessorDeclarationNode node)
        {
            // Types cannot be declared in an accessor, so we stop here
        }

        /// <inheritdoc/>
        public override void VisitConstructorDeclarationNode(ConstructorDeclarationNode node)
        {
            // Types cannot be declared in a constructor, so we stop here
        }

        /// <inheritdoc/>
        public override void VisitDelegateDeclarationNode(DelegateDeclarationNode node)
        {
        }

        /// <inheritdoc/>
        public override void VisitEnumMemberDeclarationNode(EnumMemberDeclarationNode node)
        {
        }

        /// <inheritdoc/>
        public override void VisitFieldDeclarationNode(FieldDeclarationNode node)
        {
        }

        /// <inheritdoc/>
        public override void VisitMemberDeclarationNode(MemberDeclarationNode node)
        {
        }

        /// <inheritdoc/>
        public override void VisitMethodDeclarationNode(MethodDeclarationNode node)
        {
        }

        /// <inheritdoc/>
        public override void VisitNamespaceDeclarationNode(NamespaceDeclarationNode node)
        {
            VisitList<ParseTreeNode>(node.NamespaceMemberDeclarations);
        }

        /// <inheritdoc/>
        public override void VisitNestedTypeDeclarationNode(NestedTypeDeclarationNode node)
        {
            Visit(node.Type);
        }

        /// <inheritdoc/>
        public override void VisitOperatorDeclarationNode(OperatorDeclarationNode node)
        {
        }

        /// <inheritdoc/>
        public override void VisitParseTreeNode(ParseTreeNode node)
        {
        }

        /// <inheritdoc/>
        public override void VisitPropertyDeclarationNode(PropertyDeclarationNode node)
        {
        }

        /// <inheritdoc/>
        public override void VisitTypeDeclarationNode(TypeDeclarationNode node)
        {
            _nodes.Add(node);

            for (MemberDeclarationNode node2 = node.MemberDeclarations; node2 != null; node2 = node2.Next)
            {
                Visit(node2);
            }
        }
    }
}
