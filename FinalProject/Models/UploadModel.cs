using System;

namespace FinalProject.Models
{
    public class UploadModel
    {
        public string Title { get; set; }
        public string ImageURL { get; set; }

        public bool IsModelValid()
        {
            if ((Title == null) || (ImageURL == null))
            {
                return false;
            }
            Title = Title.Trim();
            if (Title.Length > 3)
            {
                Uri uriResult;
                bool result = Uri.TryCreate(ImageURL, UriKind.Absolute, out uriResult) && ((uriResult.Scheme == Uri.UriSchemeHttp) || (uriResult.Scheme == Uri.UriSchemeHttps));
                return result;
            }
            return false;
        }
    }
}
