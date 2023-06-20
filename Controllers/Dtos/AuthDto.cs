public class SignupRequest
{
    public string userName { get; set; } = string.Empty;
    public string password { get; set; } = string.Empty;
    public string nickName { get; set; } = string.Empty;
}

public class SigninRequest 
{
    public string userName { get; set; } = string.Empty;
    public string password { get; set; } = string.Empty;
}
