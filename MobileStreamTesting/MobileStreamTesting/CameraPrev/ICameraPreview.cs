using System;
using System.Collections.Generic;
using System.Text;

namespace MobileStreamTesting.CameraPrev
{
    public interface ICameraPreview
    {
        void ChangeCamera(bool front = true);
        void SetCamera(bool front = true);
    }
}
