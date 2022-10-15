using AutoMapper;
using TodoListApi.DataContracts;
using TodoListApi.Models;

namespace TodoListApi.DataProfiles
{
    internal class TodoProfile : Profile
    {
        public TodoProfile()
        {
            CreateMap<CreateTodo, Todo>()
              .ForMember(d => d.Id, o => o.MapFrom(s => Guid.NewGuid()));
        }
    }
}
