// <copyright file="BulkPayload.cs" company="Allan Hardy">
// Copyright (c) Allan Hardy. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace App.Metrics.Formatters.Elasticsearch.Internal
{
    public class BulkPayload
    {
        private readonly List<MetricsDocument> _documents;
        private readonly string _indexName;
        private readonly string _typeName;
        private readonly JsonSerializer _serializer;

        public BulkPayload(JsonSerializer serializer, string indexName)
            : this(serializer, indexName, "doc")
        {
        }

        public BulkPayload(JsonSerializer serializer, string indexName, string typeName)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));

            if (string.IsNullOrWhiteSpace(indexName))
            {
                throw new ArgumentNullException(nameof(indexName), "The elasticsearch index name cannot be null or whitespace");
            }

            if (string.IsNullOrWhiteSpace(typeName))
            {
                throw new ArgumentNullException(nameof(typeName), "The elasticsearch type name cannot be null or whitespace");
            }

            _indexName = indexName;
            _typeName = typeName;
            _documents = new List<MetricsDocument>();
        }

        public void Add(MetricsDocument document)
        {
            if (document == null)
            {
                return;
            }

            _documents.Add(document);
        }

        public void Write(TextWriter textWriter, Guid? documentId = null)
        {
            if (textWriter == null)
            {
                return;
            }

            foreach (var document in _documents)
            {
                _serializer.Serialize(textWriter, new BulkDocumentMetaData(_indexName, _typeName, documentId));
                textWriter.Write('\n');
                _serializer.Serialize(textWriter, document);
                textWriter.Write('\n');
            }
        }
    }
}