using System;
using System.Text;
using System.Collections.Generic;

namespace DatReaderWriter.SourceGenerator {
    public class SourceWriter {
        private StringBuilder _sb = new StringBuilder();
        private int _indent = 0;
        private string _indentString = "    ";
        private bool _startOfLine = true;

        public void Write(string text) {
            if (string.IsNullOrEmpty(text)) return;
            
            if (_startOfLine) {
                _sb.Append(new string(' ', _indent * 4));
                _startOfLine = false;
            }
            _sb.Append(text);
        }

        public void WriteLine(string text = "") {
            if (_startOfLine && !string.IsNullOrEmpty(text)) {
                _sb.Append(new string(' ', _indent * 4));
            }
            _sb.AppendLine(text);
            _startOfLine = true;
        }

        public void Indent() {
            _indent++;
        }

        public void Unindent() {
            if (_indent > 0) _indent--;
        }

        public override string ToString() {
            return _sb.ToString();
        }
        
        public IDisposable IndentScope() {
            return new Indenter(this);
        }

        private class Indenter : IDisposable {
            private SourceWriter _writer;
            public Indenter(SourceWriter writer) {
                _writer = writer;
                _writer.Indent();
            }
            public void Dispose() {
                _writer.Unindent();
            }
        }
    }
}
