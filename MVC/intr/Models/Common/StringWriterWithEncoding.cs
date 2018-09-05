using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.IO;

namespace IntranetSystem.Models.Common
{
    public class StringWriterWithEncoding: StringWriter
    {
        private Encoding _enc;

        public StringWriterWithEncoding(Encoding NewEncoding)
            : base()
        {
            _enc = NewEncoding;
        }

        public override System.Text.Encoding Encoding
        {
            get
            {
                return _enc;
            }
        }
    }
}