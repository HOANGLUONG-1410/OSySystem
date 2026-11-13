using AutoMapper;
using BusinessObjects.Models;
using Services.DTOs.Auth;
using Services.DTOs.Ingredient;
using Services.DTOs.Kitchens;
using Services.DTOs.MenuItem;
using Services.DTOs.Recipes;
using Services.DTOs.Schedules;

namespace Services.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Ingredient mappings
            CreateMap<Ingredient, IngredientResponse>();
            CreateMap<CreateIngredientRequest, Ingredient>();
            CreateMap<UpdateIngredientRequest, Ingredient>();

            // MenuItem mappings
            CreateMap<MenuItem, MenuItemResponse>();
            CreateMap<CreateMenuItemRequest, MenuItem>();
            CreateMap<UpdateMenuItemRequest, MenuItem>();

            // Recipe mappings
            CreateMap<Recipe, RecipeResponse>()
                .ForMember(dest => dest.MenuItemName, opt => opt.MapFrom(src => src.MenuItem != null ? src.MenuItem.MenuItemName : string.Empty))
                .ForMember(dest => dest.IngredientName, opt => opt.MapFrom(src => src.Ingredient != null ? src.Ingredient.IngredientName : string.Empty))
                .ForMember(dest => dest.IngredientUnit, opt => opt.MapFrom(src => src.Ingredient != null ? src.Ingredient.Unit : string.Empty));
            CreateMap<CreateRecipeRequest, Recipe>();
            CreateMap<UpdateRecipeRequest, Recipe>();

            // Kitchen mappings
            CreateMap<Kitchen, KitchenResponse>();
            CreateMap<CreateKitchenRequest, Kitchen>();
            CreateMap<UpdateKitchenRequest, Kitchen>();

            // Account mappings
            CreateMap<SystemAccount, AccountResponse>()
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString()))
                .ForMember(dest => dest.KitchenName, opt => opt.MapFrom(src => src.Kitchen != null ? src.Kitchen.KitchenName : null));

            // Schedule mappings
            CreateMap<Schedule, ScheduleResponse>()
                .ForMember(dest => dest.StaffName, opt => opt.MapFrom(src => src.StaffAccount != null ? src.StaffAccount.FullName : "Unknown"))
                .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedByAccount != null ? src.CreatedByAccount.FullName : "Unknown"))
                .ForMember(dest => dest.WorkDate, opt => opt.MapFrom(src => src.WorkDate.ToString("yyyy-MM-dd")))
                .ForMember(dest => dest.KitchenName, opt => opt.MapFrom(src => src.Kitchen != null ? src.Kitchen.KitchenName : string.Empty));
            CreateMap<CreateScheduleRequest, Schedule>();
            CreateMap<UpdateScheduleRequest, Schedule>();

            // ScheduleLog mappings
            CreateMap<ScheduleLog, ScheduleLogResponse>()
                .ForMember(dest => dest.PerformedByName, opt => opt.MapFrom(src => src.PerformedByAccount != null ? src.PerformedByAccount.FullName : "Unknown"));
        }
    }
}
