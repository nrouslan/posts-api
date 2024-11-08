
using AutoMapper;
using PostService.Dtos;
using PostService.Models;

namespace PostService.Data
{
  public class UsersDataClient : IUsersDataClient
  {
    private readonly HttpClient _httpClient;

    private readonly IConfiguration _configuration;

    private readonly IMapper _mapper;

    public UsersDataClient(
      HttpClient httpClient,
      IConfiguration configuration,
      IMapper mapper)
    {
      _httpClient = httpClient;
      _configuration = configuration;
      _mapper = mapper;
    }

    public async Task<User?> GetUserById(int id)
    {
      var response = await _httpClient.GetAsync(
        $"{_configuration["ApiGateway"]}/api/users/{id}");

      if (response.IsSuccessStatusCode)
      {
        Console.WriteLine("--> Sync GET to UserService was OK!");

        var readUserDto = await response.Content.ReadFromJsonAsync<ReadUserDto>();

        return _mapper.Map<User>(readUserDto);
      }
      else
      {
        Console.WriteLine("--> Sync GET to UserService was NOT OK!");

        throw new Exception($"Ошибка получения пользователя по Id! (userId = {id})");
      }
    }
  }
}