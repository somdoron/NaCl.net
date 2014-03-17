using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nacl.Core
{
    public interface ISalsa20
    {
        int OutputBytes { get; }

        int InputBytes { get; }

        int KeyBytes { get; }

        int ConstBytes { get; }

        void Transform(byte[] @out, byte[] @in, byte[] k, byte[] c);
    }
}
