using AutoMapper;
using WhereToSpendYourTime.Api.Models.Category;
using WhereToSpendYourTime.Api.Models.Comment;
using WhereToSpendYourTime.Api.Models.Item;
using WhereToSpendYourTime.Api.Models.Media;
using WhereToSpendYourTime.Api.Models.Review;
using WhereToSpendYourTime.Api.Models.Tags;
using WhereToSpendYourTime.Api.Models.User;
using WhereToSpendYourTime.Data.Entities;

namespace WhereToSpendYourTime.Api.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Category, CategoryDto>();
        CreateMap<Tag, TagDto>();
        CreateMap<Item, ItemDto>()
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : "Unknown"))
            .ForMember(dest => dest.AverageRating,
                opt => opt.MapFrom(src => src.Reviews.Select(r => (double?)r.Rating).Average() ?? 0))
            .ForMember(dest => dest.Media,
                opt => opt.MapFrom(src => src.Media
                    .OrderByDescending(m => m.Type)
                    .ThenBy(m => m.Id)))
            .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.ItemTags.Select(it => it.Tag)));
        CreateMap<Review, ReviewDto>()
            .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.User!.DisplayName))
            .ForMember(dest => dest.ItemTitle, opt => opt.MapFrom(src => src.Item!.Title));
        CreateMap<Comment, CommentDto>()
            .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.User!.DisplayName));
        CreateMap<ApplicationUser, ApplicationUserDto>();
        CreateMap<Media, MediaDto>();
    }
}
