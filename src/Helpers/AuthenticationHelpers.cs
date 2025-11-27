public static class AuthenticationHelpers
{
    public static string GenerateJwtToken(string secretKey, string issuer, string audience, IEnumerable<Claim> claims, int expireMinutes = 60)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(expireMinutes),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

public    static string GenerateJwtToken(string userId, IConfiguration config)
{
    var jwtSection = config.GetSection("Jwt");
    var key = jwtSection["Key"]!;
    var issuer = jwtSection["Issuer"];
    var audience = jwtSection["Audience"];
    var expiryMinutes = int.Parse(jwtSection["ExpiryMinutes"] ?? "60");

    var claims = new[]
    {
        new Claim(JwtRegisteredClaimNames.Sub, userId),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        // add more claims here (roles, name, etc.) if needed
    };

    var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var creds = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: issuer,
        audience: audience,
        claims: claims,
        expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
        signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

}