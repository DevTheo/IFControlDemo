using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IFInterfaces.Support
{
    public interface IIFRuntime: ISimpleTextOnlySupport, IAnimationSupport, IBasicImageSupport, IBasicSoundSupport, IComplexTextSupport, IFileStorageSupport, IOtherInputSupport, IScreenSetup, IDebugInfo
    {

    }
}
