using System;
using TDMarketData.Service;
using Xunit;

namespace TDMarketData.Test
{
    public class TDAuthTests
    {
        [Fact]
        public void Deserialize_Refresh_Token()
        {
            var tokenResponse = "{\"access_token\": \"hFJrwOr7VnVyM4kYLBAEcdxet8ng48F4BqiyX/1DYclz/YCn/PA4/YZLVDKRXFWDcSqlruncVrsJwJRt2gNMdx2/8abcBnhicqUMcioW4vRVAw2CRUAST1LeWxmE4RiFgAktuYO8+n1AZ2lTrtmpYuvgfzCJdgSVsrXLusqFvSr9EbC3yqbDn0MvSqXs/fVSRMkq0Q2mRZV1QxoGfkgGiig3Re30JPMMFkS+8LmlbwJ+1EVlwl/6+v+iTkGnXgkyYT+GM6xnTRvIGR8290Rw20+MH88khRMtplOG1bA1GopzHXxeH+nO6KAfUp0Zwb4VoxGayDWzbWlmsUrs1tySvRrQwrHqfh6LsctE1YVD+3CjojyHr5NvqflLjoYMCWQoJxf3npyiWc2CoCDW0aB/ootym1G22TDZf69OIcuysJaGEfKW+fWMiCVzvU8109bPhWwzpLkRxNsfoZF7Z2TdaACowK9VcTG2QOQOIGKg8cmrUUSHSehoFc567/dZFshFgd8NLypobt2mzsPdNmMHQ4Ll/gy7yMQALICxldWCznZul1xvi100MQuG4LYrgoVi/JHHvlxmEITGcuqS9Gpd60Mf4m5x/Ziz/Qf5PCbgUHzBWl4KLZlBmz+kvy6349l3KmxJzjhrwwwvtiTHXwUREQvCnPaL2Szfp/WdIaLpy2ziHxfxfYv8li6eQbmvOAnDzlHVDw2vbUAOqCffMoIsF3qiWyCYD6zjgWJiDlwftKdQg4djDAd1q+wqa5VhlcIYo2slBulPyk/yt0Z9zGl5yFpYnGAAYOzRsfg82SdUVsUJ0J9Pc7wbb/vWXi/bzktFRaI6YKQzq/YwVsAxP5i628Xw6Ni/kq0PFSSZQkm/Wjfn53s5qQfmhxRGNby922MCehuMSPVBijoxtPd5JVLr/jopjfCqQAh6Azw5kp22/77JbzStuKqQxLzDGbKSCZygOKADGK5JALaHeMh/Y5+DAS+Lr+wpyAeJeFikRoUUy2ltXtyoUUOMXMpFIl/AvkDv6dehVH1wHKMytcILm03exMZTCmRsfik3fPZmJxM2w0CjwbAKDcirDkwiV4BgCZs0VrdKtSV96lgIIBjwv04RgxIf2+EUR1YLrPxXlZx3mNx3H/kK/WPQnQ==212FD3x19z9sWBHDJACbC00B75E\",\"scope\": \"PlaceTrades AccountAccess MoveMoney\",\"expires_in\": 1800,\"token_type\": \"Bearer\"}";


            var tdAuthToken = Newtonsoft.Json.JsonConvert.DeserializeObject<TDAuthToken>(tokenResponse);

            Assert.NotNull(tdAuthToken.access_token);

        }
    }
}
