using Xunit;
using Moq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using PlatinumDev.KnittingAIWebAPI.Domain;
using PlatinumDev.KnittingAIWebAPI.Infrastructure;

public class KnittingProcessorFacadeTests
{
    [Fact]
    public void AnalyzeImage_ShouldReturnExpectedOutput()
    {
        // Arrange
        var fakeOutput = new ModelOutputData ("test-output", 0.95, new Dictionary<string, object>());
        var modelRunnerMock = new Mock<PlatinumDev.KnittingAIWebAPI.Infrastructure.IModelRunner>();
        modelRunnerMock
            .Setup(m => m.RunModel(It.IsAny<Stream>()))
            .Returns(fakeOutput);

        var mockRepo = new Mock<PlatinumDev.KnittingAIWebAPI.Infrastructure.IProjectRepository>();
        var facade = new KnittingProcessorFacade(modelRunnerMock.Object, mockRepo.Object);

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes("dummy"));

        // Act
        var result = facade.AnalyzeImage(stream);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(0.95, result.Confidence);
    }
}
