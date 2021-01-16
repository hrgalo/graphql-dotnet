using System;
using System.Collections.Generic;
using GraphQL.Execution;
using GraphQL.Instrumentation;
using GraphQL.Language.AST;
using GraphQL.Types;
using Field = GraphQL.Language.AST.Field;

namespace GraphQL
{
    /// <summary>
    /// A readonly implementation of <see cref="IResolveFieldContext"/>.
    /// </summary>
    public readonly struct ReadonlyResolveFieldContextStruct<TSource> : IResolveFieldContext<TSource>
    {
        private readonly ExecutionNode _executionNode;
        private readonly ExecutionContext _executionContext;

        /// <summary>
        /// Initializes an instance with the specified <see cref="ExecutionNode"/> and <see cref="ExecutionContext"/>.
        /// </summary>
        internal ReadonlyResolveFieldContextStruct(ExecutionNode node, ExecutionContext context)
        {
            _executionNode = node ?? throw new ArgumentNullException(nameof(node));
            _executionContext = context ?? throw new ArgumentNullException(nameof(context));
        }

        private IDictionary<string, Field> GetSubFields()
        {
            return _executionNode.Field?.SelectionSet?.Selections?.Count > 0
                ? ExecutionHelper.CollectFields(_executionContext, _executionNode.FieldDefinition.ResolvedType, _executionNode.Field.SelectionSet)
                : null;
        }

        private IDictionary<string, object> GetArguments()
            => ExecutionHelper.GetArgumentValues(_executionContext.Schema, _executionNode.FieldDefinition.Arguments, _executionNode.Field.Arguments, _executionContext.Variables);

        /// <inheritdoc/>
        public TElement[] GetPooledArray<TElement>(int minimumLength)
        {
            var array = System.Buffers.ArrayPool<TElement>.Shared.Rent(minimumLength);
            _executionContext.TrackArray(array);
            return array;
        }

        /// <inheritdoc/>
        public TSource Source => (TSource)_executionNode.Source;

        /// <inheritdoc/>
        public string FieldName => _executionNode.Field.Name;

        /// <inheritdoc/>
        public Field FieldAst => _executionNode.Field;

        /// <inheritdoc/>
        public FieldType FieldDefinition => _executionNode.FieldDefinition;

        /// <inheritdoc/>
        public IGraphType ReturnType => _executionNode.FieldDefinition.ResolvedType;

        /// <inheritdoc/>
        public IObjectGraphType ParentType => _executionNode.GetParentType(_executionContext.Schema);

        /// <inheritdoc/>
        public IDictionary<string, object> Arguments => _executionNode.Arguments ??= GetArguments();

        /// <inheritdoc/>
        public object RootValue => _executionContext.RootValue;

        /// <inheritdoc/>
        public ISchema Schema => _executionContext.Schema;

        /// <inheritdoc/>
        public Document Document => _executionContext.Document;

        /// <inheritdoc/>
        public Operation Operation => _executionContext.Operation;

        /// <inheritdoc/>
        public Fragments Fragments => _executionContext.Fragments;

        /// <inheritdoc/>
        public Variables Variables => _executionContext.Variables;

        /// <inheritdoc/>
        public System.Threading.CancellationToken CancellationToken => _executionContext.CancellationToken;

        /// <inheritdoc/>
        public Metrics Metrics => _executionContext.Metrics;

        /// <inheritdoc/>
        public ExecutionErrors Errors => _executionContext.Errors;

        /// <inheritdoc/>
        public IEnumerable<object> Path => _executionNode.Path;

        /// <inheritdoc/>
        public IEnumerable<object> ResponsePath => _executionNode.ResponsePath;

        /// <inheritdoc/>
        public IDictionary<string, Field> SubFields => _executionNode.SubFields1 ??= GetSubFields();

        /// <inheritdoc/>
        public IDictionary<string, object> UserContext => _executionContext.UserContext;

        object IResolveFieldContext.Source => _executionNode.Source;

        /// <inheritdoc/>
        public IDictionary<string, object> Extensions => _executionContext.Extensions;

        /// <inheritdoc/>
        public IServiceProvider RequestServices => _executionContext.RequestServices;
    }
}