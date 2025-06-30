using Tournament.Core.DTOs;

namespace Tournament.APITests
{
    public class GameControllerTests
    {
        [Fact]
        public void Test1()
        {
            GameGetParamsDTO getParams = new()
            {
                OrderCriteria = "Title",
                StartTime = DateTime.Now.AddDays(-1),
                EndTime = DateTime.Now.AddDays(1)
            };

            GetGame(getParams);

        }
    }
}