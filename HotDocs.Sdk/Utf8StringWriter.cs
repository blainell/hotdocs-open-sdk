using System.IO;
using System.Text;

namespace HotDocs.Sdk
{
    internal sealed class Utf8StringWriter : StringWriter
    {
        public override Encoding Encoding { get { return Encoding.UTF8; } }
    }
}