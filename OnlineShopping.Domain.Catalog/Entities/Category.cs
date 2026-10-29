namespace OnlineShopping.Domain.Catalog.Entities;

using Helpers;

public class Category : Entity
{
    public string Name { get; private set; }
    public string? Image { get; private set; }
    public int? ParentCategoryId { get; set; }
    public Category? ParentCategory { get; set; }

    public Category()
    {

    }

    public Category(string name)
    {
        if (string.IsNullOrEmpty(name) || name.Length > 50)
        {
            throw new DomainException("Nombre inválido, debe tener máximo 50 caracteres");
        }

        Name = name;
    }

    public void SetImage(string image)
    {
        if (!EntityHelper.IsValidImageUrl(image))
        {
            throw new DomainException("La URL de la imagen no es válida.");
        }

        Image = image;
    }

    public void SetParentCategory(Category parentCategory)
    {
        ParentCategory = parentCategory;
    }
}
