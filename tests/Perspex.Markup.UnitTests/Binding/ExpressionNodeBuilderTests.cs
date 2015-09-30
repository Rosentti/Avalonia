﻿// Copyright (c) The Perspex Project. All rights reserved.
// Licensed under the MIT license. See licence.md file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Perspex.Markup.Binding;
using Xunit;

namespace Perspex.Markup.UnitTests.Binding
{
    public class ExpressionNodeBuilderTests
    {
        [Fact]
        public void Should_Build_Single_Property()
        {
            var result = ToList(ExpressionNodeBuilder.Build("Foo"));

            Assert.Equal(1, result.Count);
            Assert.IsType<PropertyAccessorNode>(result[0]);
        }

        [Fact]
        public void Should_Build_Property_Chain()
        {
            var result = ToList(ExpressionNodeBuilder.Build("Foo.Bar.Baz"));

            Assert.Equal(3, result.Count);
            AssertIsProperty(result[0], "Foo");
            AssertIsProperty(result[1], "Bar");
            AssertIsProperty(result[2], "Baz");
        }

        [Fact]
        public void Should_Build_Negated_Property_Chain()
        {
            var result = ToList(ExpressionNodeBuilder.Build("!Foo.Bar.Baz"));

            Assert.Equal(4, result.Count);
            Assert.IsType<LogicalNotNode>(result[0]);
            AssertIsProperty(result[1], "Foo");
            AssertIsProperty(result[2], "Bar");
            AssertIsProperty(result[3], "Baz");
        }

        [Fact]
        public void Should_Build_Double_Negated_Property_Chain()
        {
            var result = ToList(ExpressionNodeBuilder.Build("!!Foo.Bar.Baz"));

            Assert.Equal(5, result.Count);
            Assert.IsType<LogicalNotNode>(result[0]);
            Assert.IsType<LogicalNotNode>(result[1]);
            AssertIsProperty(result[2], "Foo");
            AssertIsProperty(result[3], "Bar");
            AssertIsProperty(result[4], "Baz");
        }

        [Fact]
        public void Should_Build_Indexed_Property()
        {
            var result = ToList(ExpressionNodeBuilder.Build("Foo[15]"));

            Assert.Equal(2, result.Count);
            AssertIsProperty(result[0], "Foo");
            AssertIsIndexer(result[1], 15);
            Assert.IsType<ElementAccessorNode>(result[1]);
            Assert.Equal(new object[] { 15 }, ((ElementAccessorNode)result[1]).Arguments.ToArray());
        }

        [Fact]
        public void Should_Build_Multiple_Indexed_Property()
        {
            var result = ToList(ExpressionNodeBuilder.Build("Foo[15,6]"));

            Assert.Equal(2, result.Count);
            AssertIsProperty(result[0], "Foo");
            Assert.IsType<ElementAccessorNode>(result[1]);
            Assert.Equal(new object[] { 15, 6 }, ((ElementAccessorNode)result[1]).Arguments.ToArray());
        }

        [Fact]
        public void Should_Build_Multiple_Indexed_Property_With_Space()
        {
            var result = ToList(ExpressionNodeBuilder.Build("Foo[5, 16]"));

            Assert.Equal(2, result.Count);
            AssertIsProperty(result[0], "Foo");
            Assert.IsType<ElementAccessorNode>(result[1]);
            Assert.Equal(new object[] { 5, 16 }, ((ElementAccessorNode)result[1]).Arguments.ToArray());
        }

        [Fact]
        public void Should_Build_Indexed_Property_In_Chain()
        {
            var result = ToList(ExpressionNodeBuilder.Build("Foo.Bar[5, 6].Baz"));

            Assert.Equal(4, result.Count);
            AssertIsProperty(result[0], "Foo");
            AssertIsProperty(result[1], "Bar");
            Assert.IsType<ElementAccessorNode>(result[2]);
            AssertIsProperty(result[3], "Baz");
        }

        private void AssertIsProperty(ExpressionNode node, string name)
        {
            Assert.IsType<PropertyAccessorNode>(node);

            var p = (PropertyAccessorNode)node;
            Assert.Equal(name, p.PropertyName);
        }

        private void AssertIsIndexer(ExpressionNode node, params object[] args)
        {
            Assert.IsType<ElementAccessorNode>(node);

            var e = (ElementAccessorNode)node;
            Assert.Equal(e.Arguments.ToArray(), args.ToArray());
        }

        private List<ExpressionNode> ToList(ExpressionNode node)
        {
            var result = new List<ExpressionNode>();
            
            while (node != null)
            {
                result.Add(node);
                node = node.Next;
            }

            return result;
        }
    }
}
