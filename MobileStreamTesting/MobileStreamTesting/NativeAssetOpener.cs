using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MobileStreamTesting
{
    public interface NativeAssetOpener
    {
        Stream GetSampleFileStream();
        Stream GetFrame(int frame);
    }
}
