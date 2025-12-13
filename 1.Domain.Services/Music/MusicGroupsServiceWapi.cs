using Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Models;
using Models.DTO;
using Models.Music;
using Models.Music.DTO;
using Models.Music.Interfaces;
using Newtonsoft.Json;
using Services.Music.Interfaces;

namespace Services.Music;

public class MusicGroupsServiceWapi : IMusicGroupsService
{
    private readonly ILogger<MusicGroupsServiceWapi> _logger;
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
    public MusicGroupsServiceWapi(IHttpClientFactory httpClientFactory, ILogger<MusicGroupsServiceWapi> logger, IConfiguration configuration)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient(name: configuration["WebApi:Name"]);
    }

    public async Task<ResponsePageDto<IMusicGroup>> ReadMusicGroupsAsync(bool seeded, bool flat, string filter, int pageNumber, int pageSize) 
    {
        string uri = $"musicgroups/read?seeded={seeded}&flat={flat}&filter={filter}&pagenr={pageNumber}&pagesize={pageSize}";

        //Send the HTTP Message and await the repsonse
        HttpResponseMessage response = await _httpClient.GetAsync(uri);

        //Throw an exception if the response is not successful
        await response.EnsureSuccessStatusMessage();

        //Get the resonse data
        string s = await response.Content.ReadAsStringAsync();
        var resp = JsonConvert.DeserializeObject<ResponsePageDto<IMusicGroup>>(s, _jsonSettings);
        return resp;
    }
    public async Task<ResponseItemDto<IMusicGroup>> ReadMusicGroupAsync(Guid id, bool flat)
    {
        string uri = $"musicgroups/readitem?id={id}&flat={flat}";

        //Send the HTTP Message and await the repsonse
        HttpResponseMessage response = await _httpClient.GetAsync(uri);

        //Throw an exception if the response is not successful
        await response.EnsureSuccessStatusMessage();

        //Get the response body
        string s = await response.Content.ReadAsStringAsync();
        var resp = JsonConvert.DeserializeObject<ResponseItemDto<IMusicGroup>>(s, _jsonSettings);
        return resp;
    }
    public async Task<ResponseItemDto<IMusicGroup>> DeleteMusicGroupAsync(Guid id)
    {
        string uri = $"musicgroups/deleteitem/{id}";

        //Send the HTTP Message and await the repsonse
        HttpResponseMessage response = await _httpClient.DeleteAsync(uri);

        //Throw an exception if the response is not successful
        await response.EnsureSuccessStatusMessage();

        //Get the response body
        string s = await response.Content.ReadAsStringAsync();
        var resp = JsonConvert.DeserializeObject<ResponseItemDto<IMusicGroup>>(s, _jsonSettings);
        return resp;
    }
    public async Task<ResponseItemDto<IMusicGroup>> UpdateMusicGroupAsync(MusicGroupCUdto item)
    {
        string uri = $"musicgroups/updateitem/{item.MusicGroupId}";

        //Prepare the request body
        string body = JsonConvert.SerializeObject(item);
        var requestContent = new StringContent(body, System.Text.Encoding.UTF8, "application/json");

        //Send the HTTP Message and await the repsonse
        HttpResponseMessage response = await _httpClient.PutAsync(uri, requestContent);

        //Throw an exception if the response is not successful
        await response.EnsureSuccessStatusMessage();

        //Get the response body
        string s = await response.Content.ReadAsStringAsync();
        var resp = JsonConvert.DeserializeObject<ResponseItemDto<IMusicGroup>>(s, _jsonSettings);
        return resp;
    }
    public async Task<ResponseItemDto<IMusicGroup>> CreateMusicGroupAsync(MusicGroupCUdto item)
    {
        string uri = $"musicgroups/createitem";

        //Prepare the request content
        string body = JsonConvert.SerializeObject(item);
        var requestContent = new StringContent(body, System.Text.Encoding.UTF8, "application/json");

        //Send the HTTP Message and await the repsonse
        HttpResponseMessage response = await _httpClient.PostAsync(uri, requestContent);

        //Throw an exception if the response is not successful
        await response.EnsureSuccessStatusMessage();

        //Get the resonse data
        string s = await response.Content.ReadAsStringAsync();
        var resp = JsonConvert.DeserializeObject<ResponseItemDto<IMusicGroup>>(s, _jsonSettings);
        return resp;
    }
}

