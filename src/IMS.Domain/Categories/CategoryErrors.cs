using IMS.SharedKernel;

namespace IMS.Domain.Categories;

public class CategoryErrors
{
    public static Error CategoryAlreadyExists(string name) =>
        new Error("CategoryError.CategoryAlreadyExists", $"Category with name {name} already exists.");

    public static Error CategoryNotFound(ulong id) =>
        new Error("CategoryError.CategoryNotFound", $"Category with id {id} not found.");

    public static Error UpdateCategoryFailed(ulong id, string message) =>
        new Error("CategoryError.UpdateCategoryFailed", $"Update category with id {id} failed. {message}");

    public static Error RemoveCategoryFailed(ulong id, string message) =>
        new Error("CategoryError.DeleteCategoryFailed", $"Failed to remove category with id {id}. {message}");

    public static Error CreateCategoryFailed(string name, string message) =>
        new Error("CategoryError.CreateCategoryFailed", $"Creation of category with name {name} failed. {message}");

    public static Error CategoryInUse(ulong id) =>
        new Error("CategoryError.CategoryInUse", $"Category with id {id} is in use.");
}
