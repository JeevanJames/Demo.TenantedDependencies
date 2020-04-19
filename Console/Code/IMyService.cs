namespace Console.Code
{
    public interface IMyService
    {
        string GetValue();
    }

    public sealed class MicrosoftMyService : IMyService
    {
        public string GetValue() => "Microsoft";
    }

    public sealed class AmazonMyService : IMyService
    {
        public string GetValue() => "Amazon";
    }

    public sealed class GoogleMyService : IMyService
    {
        public string GetValue() => "Google";
    }
}
 