namespace MyWhiskyShelf.WebApi.Constants;

public static class Authentication
{
    public static class Roles
    {
        public const string Admin = "admin";
        public const string User = "user";
    }

    public static class Policies
    {
        public const string ReadDistilleries = "Distilleries.Read";
        public const string WriteDistilleries = "Distilleries.Write";
        public const string ReadWhiskyBottles = "WhiskyBottles.Read";
        public const string WriteWhiskyBottles = "WhiskyBottles.Write";
        public const string ReadGeoData = "GeoData.Read";
        public const string WriteGeoData = "GeoData.Write";
    }
}