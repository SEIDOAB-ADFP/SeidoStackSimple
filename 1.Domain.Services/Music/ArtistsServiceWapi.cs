using Microsoft.Extensions.Logging;

using Configuration;
using Models.DTO;
using Newtonsoft.Json;
using Models;
using Microsoft.Extensions.Configuration;
using Services.Music.Interfaces;
using Models.Music;
using Models.Music.Interfaces;
using Models.Music.DTO;

namespace Services.Music;

public class ArtistsServiceWapi : IArtistsService
{
    private readonly ILogger<ArtistsServiceWapi> _logger;
    private readonly HttpClient _httpClient;
    //To ensure Json deserializern is using the class implementations instead of interfaces 
    private readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
    {
        Converters = {
            new AbstractConverter<IMusicGroup, MusicGroup>(),
            new AbstractConverter<IAlbum, Album>(),
            new AbstractConverter<IArtist, Artist>()
        },
    };
    public ArtistsServiceWapi(IHttpClientFactory httpClientFactory, ILogger<ArtistsServiceWapi> logger, IConfiguration configuration)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient(name: configuration["WebApi:Name"]);
    }

    public async Task<ResponsePageDto<IArtist>> ReadArtistsAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        string uri = $"artists/read?seeded={seeded}&flat={flat}&filter={filter}&pagenr={pageNumber}&pagesize={pageSize}";

        //Send the HTTP Message and await the repsonse
        HttpResponseMessage response = await _httpClient.GetAsync(uri);

        //Throw an exception if the response is not successful
        await response.EnsureSuccessStatusMessage();

        //Get the resonse data
        string s = await response.Content.ReadAsStringAsync();
        var resp = JsonConvert.DeserializeObject<ResponsePageDto<IArtist>>(s, _jsonSettings);
        return resp;
    }
    public async Task<ResponseItemDto<IArtist>> ReadArtistAsync(Guid id, bool flat)
    {
        string uri = $"artists/readitem?id={id}&flat={flat}";

        //Send the HTTP Message and await the repsonse
        HttpResponseMessage response = await _httpClient.GetAsync(uri);

        //Throw an exception if the response is not successful
        await response.EnsureSuccessStatusMessage();

        //Get the response body
        string s = await response.Content.ReadAsStringAsync();
        var resp = JsonConvert.DeserializeObject<ResponseItemDto<IArtist>>(s, _jsonSettings);
        return resp;
    }
    public async Task<ResponseItemDto<IArtist>> DeleteArtistAsync(Guid id)
    {
        string uri = $"artists/deleteitem/{id}";

        //Send the HTTP Message and await the repsonse
        HttpResponseMessage response = await _httpClient.DeleteAsync(uri);

        //Throw an exception if the response is not successful
        await response.EnsureSuccessStatusMessage();

        //Get the response body
        string s = await response.Content.ReadAsStringAsync();
        var resp = JsonConvert.DeserializeObject<ResponseItemDto<IArtist>>(s, _jsonSettings);
        return resp;
    }
    public async Task<ResponseItemDto<IArtist>> UpdateArtistAsync(ArtistCUdto item)
    {
        string uri = $"artists/updateitem/{item.ArtistId}";

        //Prepare the request body
        string body = JsonConvert.SerializeObject(item);
        var requestContent = new StringContent(body, System.Text.Encoding.UTF8, "application/json");

        //Send the HTTP Message and await the repsonse
        HttpResponseMessage response = await _httpClient.PutAsync(uri, requestContent);

        //Throw an exception if the response is not successful
        await response.EnsureSuccessStatusMessage();

        //Get the response body
        string s = await response.Content.ReadAsStringAsync();
        var resp = JsonConvert.DeserializeObject<ResponseItemDto<IArtist>>(s, _jsonSettings);
        return resp;
    }
    public async Task<ResponseItemDto<IArtist>> CreateArtistAsync(ArtistCUdto item)
    {
        string uri = $"artists/createitem";

        //Prepare the request content
        string body = JsonConvert.SerializeObject(item);
        var requestContent = new StringContent(body, System.Text.Encoding.UTF8, "application/json");

        //Send the HTTP Message and await the repsonse
        HttpResponseMessage response = await _httpClient.PostAsync(uri, requestContent);

        //Throw an exception if the response is not successful
        await response.EnsureSuccessStatusMessage();

        //Get the resonse data
        string s = await response.Content.ReadAsStringAsync();
        var resp = JsonConvert.DeserializeObject<ResponseItemDto<IArtist>>(s, _jsonSettings);
        return resp;
    }
}

