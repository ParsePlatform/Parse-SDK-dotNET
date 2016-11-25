using System;
using System.Collections.Generic;
using LeanCloud;

namespace LeanCloud.Push.Internal {
  // TODO: (richardross) once coder is refactored, make this extend IParseObjectCoder.
  public interface IParseInstallationCoder {
    IDictionary<string, object> Encode(AVInstallation installation);

    AVInstallation Decode(IDictionary<string, object> data);
  }
}