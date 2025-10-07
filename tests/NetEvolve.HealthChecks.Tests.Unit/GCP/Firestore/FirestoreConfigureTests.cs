namespace NetEvolve.HealthChecks.Tests.Unit.GCP.Firestore;

using System;
using Microsoft.Extensions.Configuration;
using NetEvolve.Extensions.TUnit;
using NetEvolve.HealthChecks.GCP.Firestore;

[TestGroup(nameof(Firestore))]
public sealed class FirestoreConfigureTests
{
    [Test]
    public void Configure_WhenArgumentNameNull_ThrowArgumentNullException()
    {
        // Arrange
        var configure = new FirestoreOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = default;
        var options = new FirestoreOptions();

        // Act
        void Act() => configure.Configure(name, options);

        // Assert
        _ = Assert.Throws<ArgumentNullException>("name", Act);
    }

    [Test]
    public async Task PostConfigure_WhenTimeoutLessThanInfinite_SetToInfinite()
    {
        // Arrange
        var configure = new FirestoreOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new FirestoreOptions { Timeout = -2 };

        // Act
        configure.PostConfigure(name, options);

        // Assert
        _ = await Assert.That(options.Timeout).IsEqualTo(-1);
    }

    [Test]
    public async Task PostConfigure_WhenTimeoutValid_DoNotChange()
    {
        // Arrange
        var configure = new FirestoreOptionsConfigure(new ConfigurationBuilder().Build());
        const string? name = "Test";
        var options = new FirestoreOptions { Timeout = 100 };

        // Act
        configure.PostConfigure(name, options);

        // Assert
        _ = await Assert.That(options.Timeout).IsEqualTo(100);
    }
}
