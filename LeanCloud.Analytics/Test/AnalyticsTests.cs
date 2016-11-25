using Moq;
using NUnit.Framework;
using LeanCloud;
using LeanCloud.Analytics.Internal;
using LeanCloud.Core.Internal;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ParseTest {
  [TestFixture]
  public class AnalyticsTests {
    [TearDown]
    public void TearDown() {
      AVAnalyticsPlugins.Instance.Reset();
    }

    [Test]
    [AsyncStateMachine(typeof(AnalyticsTests))]
    public Task TestTrackEvent() {
      var mockController = new Mock<IAVAnalyticsController>();
      var mockCorePlugins = new Mock<IAVCorePlugins>();
      var mockCurrentUserController = new Mock<IAVCurrentUserController>();

      mockCorePlugins
        .Setup(corePlugins => corePlugins.CurrentUserController)
        .Returns(mockCurrentUserController.Object);

      mockCurrentUserController
        .Setup(controller => controller.GetCurrentSessionTokenAsync(It.IsAny<CancellationToken>()))
        .Returns(Task.FromResult("sessionToken"));

      AVAnalyticsPlugins plugins = new AVAnalyticsPlugins();
      plugins.AnalyticsController = mockController.Object;
      plugins.CorePlugins = mockCorePlugins.Object;
      AVAnalyticsPlugins.Instance = plugins;

      return AVAnalytics.TrackEventAsync("SomeEvent").ContinueWith(t => {
        Assert.IsFalse(t.IsFaulted);
        Assert.IsFalse(t.IsCanceled);
        mockController.Verify(obj => obj.TrackEventAsync(It.Is<string>(eventName => eventName == "SomeEvent"),
            It.Is<IDictionary<string, string>>(dict => dict == null),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Exactly(1));
      });
    }

    [Test]
    [AsyncStateMachine(typeof(AnalyticsTests))]
    public Task TestTrackEventWithDimension() {
      var mockController = new Mock<IAVAnalyticsController>();
      var mockCorePlugins = new Mock<IAVCorePlugins>();
      var mockCurrentUserController = new Mock<IAVCurrentUserController>();

      mockCorePlugins
        .Setup(corePlugins => corePlugins.CurrentUserController)
        .Returns(mockCurrentUserController.Object);

      mockCurrentUserController
        .Setup(controller => controller.GetCurrentSessionTokenAsync(It.IsAny<CancellationToken>()))
        .Returns(Task.FromResult("sessionToken"));

      AVAnalyticsPlugins plugins = new AVAnalyticsPlugins();
      plugins.AnalyticsController = mockController.Object;
      plugins.CorePlugins = mockCorePlugins.Object;
      AVAnalyticsPlugins.Instance = plugins;

      var dimensions = new Dictionary<string, string>() {
        { "facebook", "hq" }
      };

      return AVAnalytics.TrackEventAsync("SomeEvent", dimensions).ContinueWith(t => {
        Assert.IsFalse(t.IsFaulted);
        Assert.IsFalse(t.IsCanceled);
        mockController.Verify(obj => obj.TrackEventAsync(It.Is<string>(eventName => eventName == "SomeEvent"),
            It.Is<IDictionary<string, string>>(dict => dict != null && dict.Count == 1),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Exactly(1));
      });
    }

    [Test]
    [AsyncStateMachine(typeof(AnalyticsTests))]
    public Task TestTrackAppOpened() {
      var mockController = new Mock<IAVAnalyticsController>();
      var mockCorePlugins = new Mock<IAVCorePlugins>();
      var mockCurrentUserController = new Mock<IAVCurrentUserController>();

      mockCorePlugins
        .Setup(corePlugins => corePlugins.CurrentUserController)
        .Returns(mockCurrentUserController.Object);

      mockCurrentUserController
        .Setup(controller => controller.GetCurrentSessionTokenAsync(It.IsAny<CancellationToken>()))
        .Returns(Task.FromResult("sessionToken"));

      AVAnalyticsPlugins plugins = new AVAnalyticsPlugins();
      plugins.AnalyticsController = mockController.Object;
      plugins.CorePlugins = mockCorePlugins.Object;
      AVAnalyticsPlugins.Instance = plugins;

      return AVAnalytics.TrackAppOpenedAsync().ContinueWith(t => {
        Assert.IsFalse(t.IsFaulted);
        Assert.IsFalse(t.IsCanceled);
        mockController.Verify(obj => obj.TrackAppOpenedAsync(It.Is<string>(pushHash => pushHash == null),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Exactly(1));
      });
    }
  }
}
