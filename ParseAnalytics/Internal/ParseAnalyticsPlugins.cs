// Copyright (c) 2015-present, LeanCloud, LLC.  All rights reserved.  This source code is licensed under the BSD-style license found in the LICENSE file in the root directory of this source tree.  An additional grant of patent rights can be found in the PATENTS file in the same directory.

using System;
using LeanCloud.Core.Internal;

namespace LeanCloud.Analytics.Internal {
  public class ParseAnalyticsPlugins : IParseAnalyticsPlugins {
    private static readonly object instanceMutex = new object();
    private static IParseAnalyticsPlugins instance;
    public static IParseAnalyticsPlugins Instance {
      get {
        lock (instanceMutex) {
          instance = instance ?? new ParseAnalyticsPlugins();
          return instance;
        }
      }
      set {
        lock (instanceMutex) {
          instance = value;
        }
      }
    }

    private readonly object mutex = new object();

    private IAVCorePlugins corePlugins;
    private IParseAnalyticsController analyticsController;

    public void Reset() {
      lock (mutex) {
        CorePlugins = null;
        AnalyticsController = null;
      }
    }

    public IAVCorePlugins CorePlugins {
      get {
        lock (mutex) {
          corePlugins = corePlugins ?? AVPlugins.Instance;
          return corePlugins;
        }
      }
      set {
        lock (mutex) {
          corePlugins = value;
        }
      }
    }

    public IParseAnalyticsController AnalyticsController {
      get {
        lock (mutex) {
          analyticsController = analyticsController ?? new ParseAnalyticsController(CorePlugins.CommandRunner);
          return analyticsController;
        }
      }
      set {
        lock (mutex) {
          analyticsController = value;
        }
      }
    }
  }
}