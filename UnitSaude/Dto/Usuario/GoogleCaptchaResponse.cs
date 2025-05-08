public class GoogleCaptchaResponse
{
    public bool Success { get; set; }
    public float Score { get; set; }
    public string Action { get; set; }
    public DateTime ChallengeTs { get; set; }
    public string Hostname { get; set; }
    public List<string> ErrorCodes { get; set; }
}