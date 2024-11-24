namespace MoviesRepository.Tests.Intergation;

public class RepositoryTests : IClassFixture<DataBaseFixture>
{
    private readonly DataBaseFixture _fixture;

    public RepositoryTests(DataBaseFixture fixture)
    {
        _fixture = fixture;
    }
}
