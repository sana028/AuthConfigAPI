using AutoMapper;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AuthConfigAPI.Mappers
{
    public class UpdateMapper<T, P> : Profile
    {
        public UpdateMapper()
        {
            CreateMap<T, P>()
                    .ForAllMembers(opts => opts.Condition((src, dest, srcMember, destMember) => srcMember != null && !Equals(srcMember, destMember)));
        }
    }
}

