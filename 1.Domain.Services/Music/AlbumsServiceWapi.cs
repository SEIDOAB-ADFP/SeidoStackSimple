using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

using Configuration;
using Models;
using Models.DTO;
using Microsoft.Extensions.Configuration;
using Services.Music.Interfaces;
using Models.Music;
using Models.Music.Interfaces;
using Models.Music.DTO;

namespace Services.Music;

public class AlbumsServiceWapi : IAlbumsService
{
    private readonly ILogger<AlbumsServiceWapi> _logger;
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

    public AlbumsServiceWapi(IHttpClientFactory httpClientFactory, ILogger<AlbumsServiceWapi> logger, IConfiguration configuration)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient(name: configuration["WebApi:Name"]);
    }

    public async Task<ResponsePageDto<IAlbum>> ReadAlbumsAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize)
    {
        string uri = $"albums/read?seeded={seeded}&flat={flat}&filter={filter}&pagenr={pageNumber}&pagesize={pageSize}";

        //Send the HTTP Message and await the repsonse
        HttpResponseMessage response = await _httpClient.GetAsync(uri);

        //Throw an exception if the response is not successful
        await response.EnsureSuccessStatusMessage();

        //Get the resonse data
        string s = await response.Content.ReadAsStringAsync();
        var resp = JsonConvert.DeserializeObject<ResponsePageDto<IAlbum>>(s, _jsonSettings);
        return resp;
    }
    public async Task<ResponseItemDto<IAlbum>> ReadAlbumAsync(Guid id, bool flat)
    {
        string uri = $"albums/readitem?id={id}&flat={flat}";

        //Send the HTTP Message and await the repsonse
        HttpResponseMessage response = await _httpClient.GetAsync(uri);

        //Throw an exception if the response is not successful
        await response.EnsureSuccessStatusMessage();

        //Get the response body
        string s = await response.Content.ReadAsStringAsync();
        var resp = JsonConvert.DeserializeObject<ResponseItemDto<IAlbum>>(s, _jsonSettings);
        return resp;
    }
    public async Task<ResponseItemDto<IAlbum>> DeleteAlbumAsync(Guid id)
    {
        string uri = $"albums/deleteitem/{id}";

        //Send the HTTP Message and await the repsonse
        HttpResponseMessage response = await _httpClient.DeleteAsync(uri);

        //Throw an exception if the response is not successful
        await response.EnsureSuccessStatusMessage();

        //Get the response body
        string s = await response.Content.ReadAsStringAsync();
        var resp = JsonConvert.DeserializeObject<ResponseItemDto<IAlbum>>(s, _jsonSettings);
        return resp;
    }
    public async Task<ResponseItemDto<IAlbum>> UpdateAlbumAsync(AlbumCUdto item)
    {
        string uri = $"albums/updateitem/{item.AlbumId}";

        //Prepare the request body
        string body = JsonConvert.SerializeObject(item);
        var requestContent = new StringContent(body, System.Text.Encoding.UTF8, "application/json");

        //Send the HTTP Message and await the repsonse
        HttpResponseMessage response = await _httpClient.PutAsync(uri, requestContent);

        //Throw an exception if the response is not successful
        await response.EnsureSuccessStatusMessage();

        //Get the response body
        string s = await response.Content.ReadAsStringAsync();
        var resp = JsonConvert.DeserializeObject<ResponseItemDto<IAlbum>>(s, _jsonSettings);
        return resp;
    }
    public async Task<ResponseItemDto<IAlbum>> CreateAlbumAsync(AlbumCUdto item)
    {
        string uri = $"albums/createitem";

        //Prepare the request content
        string body = JsonConvert.SerializeObject(item);
        var requestContent = new StringContent(body, System.Text.Encoding.UTF8, "application/json");

        //Send the HTTP Message and await the repsonse
        HttpResponseMessage response = await _httpClient.PostAsync(uri, requestContent);

        //Throw an exception if the response is not successful
        await response.EnsureSuccessStatusMessage();

        //Get the resonse data
        string s = await response.Content.ReadAsStringAsync();
        var resp = JsonConvert.DeserializeObject<ResponseItemDto<IAlbum>>(s, _jsonSettings);
        return resp;
    }
}


