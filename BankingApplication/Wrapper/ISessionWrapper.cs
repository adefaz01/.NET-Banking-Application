namespace BankingApplication.Wrapper;

public interface ISessionWrapper
{
    int  GetInt32(string key);
    void SetInt32(string key, int value);
}
