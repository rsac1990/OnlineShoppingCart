namespace OnlineShopping.Domain.Catalog.Helpers;

public static class EntityHelper
{
    public static bool IsValidImageUrl(string imageUrl)
    {
        if (!Uri.TryCreate(imageUrl, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            return false;
        }
        return true;
    }
}
