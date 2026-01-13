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
    private const string Unknown = "Unknown";
    private const string DefaultUserRole = "User";

    public MappingProfile()
    {
        ConfigureCategoryMappings();
        ConfigureTagMappings();
        ConfigureItemMappings();
        ConfigureReviewMappings();
        ConfigureCommentMappings();
        ConfigureUserMappings();
        ConfigureMediaMappings();
    }

    private void ConfigureCategoryMappings()
    {
        CreateMap<Category, CategoryDto>();
    }

    private void ConfigureTagMappings()
    {
        CreateMap<Tag, TagDto>();
    }

    private void ConfigureItemMappings()
    {
        CreateMap<Item, ItemDto>()
            .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : Unknown))
            .ForMember(dest => dest.AverageRating,
                opt => opt.MapFrom(src => src.Reviews.Select(r => (double?)r.Rating).Average() ?? 0))
            .ForMember(dest => dest.Media,
                opt => opt.MapFrom(src => src.Media
                    .OrderByDescending(m => m.Type)
                    .ThenBy(m => m.Id)))
            .ForMember(dest => dest.Tags,
                opt => opt.MapFrom(src => src.ItemTags.Select(it => it.Tag)));
    }

    private void ConfigureReviewMappings()
    {
        CreateMap<Review, ReviewDto>()
            .ForMember(dest => dest.Author,
                opt => opt.MapFrom(src => src.User != null ? src.User.DisplayName : Unknown))
            .ForMember(dest => dest.AuthorRole,
                opt => opt.MapFrom(src =>
                    src.User != null
                        ? src.User.UserRoles
                            .Select(ur => ur.Role.Name)
                            .OrderBy(name => name)
                            .FirstOrDefault() ?? DefaultUserRole
                        : DefaultUserRole))
            .ForMember(dest => dest.ItemTitle,
                opt => opt.MapFrom(src => src.Item != null ? src.Item.Title : Unknown));
    }

    private void ConfigureCommentMappings()
    {
        CreateMap<Comment, CommentDto>()
            .ForMember(dest => dest.Author,
                opt => opt.MapFrom(src => src.User != null ? src.User.DisplayName : Unknown))
            .ForMember(dest => dest.AuthorRole,
                opt => opt.MapFrom(src =>
                    src.User != null
                        ? src.User.UserRoles
                            .Select(ur => ur.Role.Name)
                            .OrderBy(name => name)
                            .FirstOrDefault() ?? DefaultUserRole
                        : DefaultUserRole));
    }

    private void ConfigureUserMappings()
    {
        CreateMap<ApplicationUser, ApplicationUserDto>()
            .ForMember(dest => dest.Role,
                opt => opt.MapFrom(src =>
                    src.UserRoles
                       .Select(ur => ur.Role.Name)
                       .OrderBy(r => r)
                       .FirstOrDefault() ?? DefaultUserRole));
    }

    private void ConfigureMediaMappings()
    {
        CreateMap<Media, MediaDto>();
    }
}