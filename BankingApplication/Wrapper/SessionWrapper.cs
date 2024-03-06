namespace BankingApplication.Wrapper;
public class SessionWrapper : ISessionWrapper
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SessionWrapper(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public int GetInt32(string key)
    {
        return (int) _httpContextAccessor.HttpContext.Session.GetInt32(key);
    }

    public void SetInt32(string key, int value)
    {
        _httpContextAccessor.HttpContext.Session.SetInt32(key, value);
    }
}

