namespace RealmHelper.Web.Utils;

public static class ImageHelper
{
    public static string ToSafeUrl(this string url)
    {
        var uriBuilder = new UriBuilder(url) { Scheme = "https" };
        uriBuilder.Host = uriBuilder.Host.Replace("images-eds", "images-eds-ssl");
        uriBuilder.Port = -1;

        return uriBuilder.ToString();
    }

    public static string Minify(this string url) =>
        $"{url}&h=100&w=100";
}