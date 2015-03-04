/*
 * Copyright (c) 2015 Nathaniel Wallace
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using SalesForceData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wallace.IDE.SalesForce.Framework
{
    /// <summary>
    /// Used to search files.
    /// </summary>
    public class SearchIndex : IDisposable
    {
        #region Fields

        /// <summary>
        /// The directory of the index.
        /// </summary>
        private Lucene.Net.Store.Directory _directory;

        /// <summary>
        /// The search analyzer.
        /// </summary>
        private Lucene.Net.Analysis.Analyzer _analyzer;

        /// <summary>
        /// The writer if we are writing.
        /// </summary>
        private Lucene.Net.Index.IndexWriter _writer;

        /// <summary>
        /// The searcher if we are reading.
        /// </summary>
        private Lucene.Net.Search.IndexSearcher _searcher;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="directory">The directory of the index.</param>
        public SearchIndex(string directory)
            : this(directory, false)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="directory">The directory of the index.</param>
        /// <param name="isWriteMode">IsWriteMode</param>
        public SearchIndex(string directory, bool isWriteMode)
        {
            if (String.IsNullOrWhiteSpace(directory))
                throw new ArgumentException("directory is null or whitespace.", "directory");

            _directory = Lucene.Net.Store.FSDirectory.Open(directory);

            _analyzer = new Lucene.Net.Analysis.Standard.StandardAnalyzer(
                Lucene.Net.Util.Version.LUCENE_30,
                new HashSet<string>());

            IsWriteMode = isWriteMode;

            if (IsWriteMode)
            {
                _writer = new Lucene.Net.Index.IndexWriter(
                    _directory,
                    _analyzer,
                    new Lucene.Net.Index.IndexWriter.MaxFieldLength(1000000));

                _searcher = null;
            }
            else
            {
                _searcher = new Lucene.Net.Search.IndexSearcher(
                    _directory,
                    true);
            }
        }

        #endregion

        #region Properties

        /// <summary>
        /// True if the index is in write mode, false if it is in read mode.  The default value is false.
        /// </summary>
        public bool IsWriteMode { get; private set; }

        /// <summary>
        /// The number of documents in the index.
        /// </summary>
        public int DocumentCount
        {
            get 
            {
                if (_writer != null)
                    return _writer.NumDocs();
                else if (_searcher != null)
                    return _searcher.IndexReader.NumDocs();
                else
                    return 0;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Clear out all documents from the index.
        /// </summary>
        public void Clear()
        {
            if (!IsWriteMode)
                throw new Exception("You can't modify the index when not in write mode.");

            _writer.DeleteAll();
        }

        /// <summary>
        /// Add the given document to the index.
        /// </summary>
        /// <param name="id">The id of the document.</param>
        /// <param name="fileName">The fileName of the document.</param>
        /// <param name="type">The type of document.</param>
        /// <param name="name">The name of the document.</param>
        /// <param name="body">The body of the document.</param>
        public void Add(
            string id,
            string fileName,
            string type,
            string name,
            string body)
        {
            if (!IsWriteMode)
                throw new Exception("You can't modify the index when not in write mode.");

            Lucene.Net.Documents.Document document = new Lucene.Net.Documents.Document();

            document.Add(new Lucene.Net.Documents.Field(
                "id",
                id,
                Lucene.Net.Documents.Field.Store.YES,
                Lucene.Net.Documents.Field.Index.NOT_ANALYZED));
            document.Add(new Lucene.Net.Documents.Field(
                "fileName",
                fileName,
                Lucene.Net.Documents.Field.Store.YES,
                Lucene.Net.Documents.Field.Index.NOT_ANALYZED));
            document.Add(new Lucene.Net.Documents.Field(
                "type",
                type,
                Lucene.Net.Documents.Field.Store.YES,
                Lucene.Net.Documents.Field.Index.NO));
            document.Add(new Lucene.Net.Documents.Field(
                "name",
                name,
                Lucene.Net.Documents.Field.Store.YES,
                Lucene.Net.Documents.Field.Index.NO));
            document.Add(new Lucene.Net.Documents.Field(
                "body",
                body,
                Lucene.Net.Documents.Field.Store.YES,
                Lucene.Net.Documents.Field.Index.ANALYZED));

            _writer.UpdateDocument(
                new Lucene.Net.Index.Term("fileName", fileName),
                document);
        }

        /// <summary>
        /// Search for files.
        /// </summary>
        /// <param name="queryText">The query text.</param>
        /// <returns>The files that match the query text.</returns>
        public SourceFile[] Search(string queryText)
        {
            Lucene.Net.QueryParsers.QueryParser parser = new Lucene.Net.QueryParsers.QueryParser(
                Lucene.Net.Util.Version.LUCENE_30,
                "body",
                _analyzer);

            Lucene.Net.Search.Query query = parser.Parse(queryText);

            using (Lucene.Net.Search.IndexSearcher searcher = new Lucene.Net.Search.IndexSearcher(_directory, true))
            {
                Lucene.Net.Search.TopDocs result = searcher.Search(query, int.MaxValue);

                List<SourceFile> files = new List<SourceFile>();
                foreach (Lucene.Net.Search.ScoreDoc d in result.ScoreDocs)
                {
                    Lucene.Net.Documents.Document doc = searcher.Doc(d.Doc);
                    files.Add(new SourceFile(
                        doc.Get("id"),
                        doc.Get("type"),
                        doc.Get("name"),
                        doc.Get("fileName"),
                        null));
                }

                return files.ToArray();
            }
        }

        /// <summary>
        /// Checks to see if the given file is indexed.
        /// </summary>
        /// <param name="file">The file to check to see if it's indexed.</param>
        /// <returns>true if the file is indexed, false if it isn't.</returns>
        public bool IsIndexed(SourceFile file)
        {
            if (file == null || String.IsNullOrWhiteSpace(file.FileName))
                return false;

            if (_writer != null)
            {
                using (Lucene.Net.Index.IndexReader reader = _writer.GetReader())
                {
                    using (Lucene.Net.Index.TermDocs docs = reader.TermDocs(new Lucene.Net.Index.Term("fileName", file.FileName)))
                    {
                        return docs.Next();
                    }
                }
            }
            else if (_searcher != null)
            {
                using (Lucene.Net.Index.TermDocs docs = _searcher.IndexReader.TermDocs(new Lucene.Net.Index.Term("fileName", file.FileName)))
                {
                    return docs.Next();
                }
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Dispose of this object.
        /// </summary>
        public void Dispose()
        {
            if (_writer != null)
            {
                _writer.Optimize();
                _writer.Flush(true, true, true);
                _writer.Dispose();
            }

            if (_searcher != null)
                _searcher.Dispose();

            if (_analyzer != null)
                _analyzer.Dispose();

            if (_directory != null)
                _directory.Dispose();
        }

        #endregion
    }
}
