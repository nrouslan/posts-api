using AutoMapper;
using Grpc.Core;
using UserService.Data;

namespace UserService.SyncDataServices.Grpc
{
  public class GrpcUserService : GrpcUser.GrpcUserBase
  {
    private readonly IUserRepo _repo;

    private readonly IMapper _mapper;

    public GrpcUserService(IUserRepo repo, IMapper mapper)
    {
      _repo = repo;
      _mapper = mapper;
    }

    public override Task<UserResponse> GetAllUsers(GetAllRequest request, ServerCallContext ctx)
    {
      var response = new UserResponse();

      var users = _repo.GetAll();

      foreach (var user in users)
      {
        response.User.Add(_mapper.Map<GrpcUserModel>(user));
      }

      return Task.FromResult(response);
    }
  }
}