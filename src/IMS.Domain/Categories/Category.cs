using IMS.Domain.Categories.Events;
using IMS.Domain.JoinTables;
using IMS.SharedKernel;

namespace IMS.Domain.Categories;

public sealed class Category : Entity, ISoftDelete
{
    public Name Name { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public bool IsDeleted { get; set; }

    // Navigation properties
    public IList<ProductCategory> ProductCategories { get; private set; } = [];

    private Category() { }

    public static Category Create(string name)
    {
        var category = new Category
        {
            Name = new Name(name)
        };

        category.RaiseDomainEvent(new CategoryCreatedEvent(category));

        return category;
    }


    public void Update(string name)
    {
        Name = new Name(name);

        RaiseDomainEvent(new CategoryUpdatedEvent(Name, this));
    }

    public void MarkAsRemoved()
    {
        RaiseDomainEvent(new CategoryRemovedEvent(Name));
    }
}
