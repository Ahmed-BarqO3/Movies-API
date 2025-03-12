using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Movies.Api;
using Movies.Contracts.Responses;
using Shouldly;

namespace Movies.IntegrationTest;

public class MovieTests : BaseIntegrationTest
{
    public MovieTests(IntegrationTestWebAppFactory factory) : base(factory){}
    
    private async Task<MovieResponse?> CreateMovieViaApi()
    {
        
        
        var request = _movieRequestFaker.Generate();
        var response = await _client.PostAsJsonAsync(ApiEndpoints.Movies.Create, request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<MovieResponse>();
    }
    

    [Fact]
    public async Task POST_CreateMovie_ReturnsCreatedMovieResponse()
    {
        // Arrange
        var request = _movieRequestFaker.Generate();
        
        var responseMessage =  await _client.PostAsJsonAsync("token", new
        {
            userid = "d8566de3-b1a6-4a9b-b842-8e3887a82e41",
            email = "nick@nickchapsas.com",
            customClaims = new {
                admin= true,
                trusted_member = false
            
            }
        });
        
        var token = await responseMessage.Content.ReadAsStringAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);
        
        
        // Act
        var response = await _client.PostAsJsonAsync(ApiEndpoints.Movies.Create, request);
       
        
        // Assert
        response.StatusCode.ShouldBe(HttpStatusCode.Created);
        var movieResponse = await response.Content.ReadFromJsonAsync<MovieResponse>();
        
        //response.Headers.Location.ShouldBeEquivalentTo(  $"http://localhost/api/movies/{movieResponse?.Id}");
        movieResponse.ShouldNotBeNull();
    }
    
}
