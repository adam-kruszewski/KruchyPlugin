using System.Linq;
using System.Text;
using EnvDTE;
using KruchyCodeBuilders.Builders;
using Kruchy.Plugin.Utils.Wrappers;
using KruchyParserKodu.ParserKodu;

#pragma warning disable VSTHRD010
namespace Kruchy.Plugin.Utils._2017.Wrappers
{
    public class DocumentWrapper : IDocumentWrapper
    {
        private readonly TextDocument textDocument;
        private readonly ProjectWrapper projectWrapper;

        public DocumentWrapper(TextDocument textDocument)
        {
            this.textDocument = textDocument;
        }

        public DocumentWrapper(TextDocument textDocument, ProjectWrapper projectWrapper)
        {
            this.textDocument = textDocument;
            this.projectWrapper = projectWrapper;
        }

        public int GetCursorLineNumber()
        {
            return textDocument.Selection.TopPoint.Line;
        }

        public void SetCursor(int row, int column)
        {
            textDocument.Selection.MoveToLineAndOffset(row, column);
        }

        public void SetCursorForAddedMethod(int lineNumber)
        {
            SetCursor(
                lineNumber + 2,
                1 + StaleDlaKodu.WciecieDlaMetody.Length
                + StaleDlaKodu.JednostkaWciecia.Length);
        }

        public string GetContent()
        {
            EditPoint objEditPoint =
                (EditPoint)textDocument.StartPoint.CreateEditPoint();
            EditPoint endEditPoint =
                (EditPoint)textDocument.EndPoint.CreateEditPoint();

            return objEditPoint.GetText(endEditPoint.AbsoluteCharOffset);
        }

        public string GetLineContent(int lineNumber)
        {
            var lineStart = textDocument.CreateEditPoint();
            lineStart.MoveToLineAndOffset(lineNumber, 1);

            var lineEnd = textDocument.CreateEditPoint();
            lineEnd.MoveToLineAndOffset(lineNumber, 1);
            lineEnd.MoveToLineAndOffset(lineNumber, lineEnd.LineLength + 1);

            return lineStart.GetText(lineEnd);
        }

        public string GetContent(
            int startRow,
            int startColumn,
            int endRow,
            int endColumn)
        {
            var lineStart = textDocument.CreateEditPoint();
            lineStart.MoveToLineAndOffset(startRow, startColumn);

            var lineEnd = textDocument.CreateEditPoint();
            lineEnd.MoveToLineAndOffset(endRow, endColumn);

            return lineStart.GetText(lineEnd);
        }

        public void InsertInLine(string text, int lineNumber)
        {
            var lineStart =
            GetEditPointForLineStart(lineNumber);
            lineStart.Insert(text);
        }

        public void InsertInPlace(string text, int lineNumber, int columnNumber)
        {
            var editPoint = GetEditPointForLineStart(lineNumber);
            editPoint.MoveToLineAndOffset(lineNumber, columnNumber);
            editPoint.Insert(text);
        }

        public void RemoveInPlace(int lineNumber, int columnNumber, int length)
        {
            var editPoint = GetEditPointForPlace(lineNumber, columnNumber);
            var editPointKoniec = GetEditPointForPlace(lineNumber, columnNumber + length);
            editPoint.Delete(editPointKoniec);
        }

        public void Remove(
            int startLineNumber,
            int startColumnNumber,
            int endLineNumber,
            int endColumnNumber)
        {
            var editPoint = GetEditPointForPlace(startLineNumber, startColumnNumber);
            var editPointKoniec = GetEditPointForPlace(endLineNumber, endColumnNumber);
            editPoint.Delete(editPointKoniec);
        }

        public void RemoveLine(int lineNumber)
        {
            var startEditPoint = GetEditPointForLineStart(lineNumber);
            var endEditPoint = GetEditPointForLineStart(lineNumber);
            endEditPoint.LineDown();
            startEditPoint.Delete(endEditPoint);
        }

        private EditPoint GetEditPointForLineStart(
            int lineNumber)
        {
            var lineStart = textDocument.CreateEditPoint();
            lineStart.MoveToLineAndOffset(lineNumber, 1);
            return lineStart;
        }

        private EditPoint GetEditPointForPlace(
            int lineNumber, int columnNumber)
        {
            var editPoint = GetEditPointForLineStart(lineNumber);
            editPoint.MoveToLineAndOffset(lineNumber, columnNumber);
            return editPoint;
        }

        public int GetLineCount()
        {
            return textDocument.EndPoint.Line;
        }
    }
}