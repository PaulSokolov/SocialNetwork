using BusinessLayer.BusinessModels;

namespace TestBLL
{
    class Program
    {
        static void Main(string[] args)
        {


            var functionality = new SocialNetworkFunctionalityUser("14c31b71-275a-44e5-aacf-f2c0b29ddeb0");
            //functionality.Friends.Add("80350603-8ce5-4a63-9735-c75d69949b1c");
            //functionality.Friends.Delete("69c956ce-bfe9-46d7-9ed1-311a91ec1fe8");
            //var list =  functionality.Friends.Get().ToList();
            //var list = functionality.Messages.GetDialog("69c956ce-bfe9-46d7-9ed1-311a91ec1fe8");
            //functionality.Messages.Send("69c956ce-bfe9-46d7-9ed1-311a91ec1fe8", "asdasd");
            //var mes = functionality.Messages.GetLastMessage("69c956ce-bfe9-46d7-9ed1-311a91ec1fe8");
            //var list = functionality.Messages.GetAllDialogs();
            //var user = functionality.Users.Get("14c31b71-275a-44e5-aacf-f2c0b29ddeb0");
            var count = functionality.Friends.Counters;
            var c = functionality.Users.GetCountriesWithUsers();
            var p = functionality.Messages.GetDialog("111136f0-bde3-4b00-82df-32f2ea2d04b2");
            //for (int i = 0; i < 10; i++)
            //{
            //    functionality.Messages.Send("111136f0-bde3-4b00-82df-32f2ea2d04b2", $"{1000 * 1000 * i}");
            //}

        }
    }
    public class A
    {
        public B ClassB{ get; set; }
        public class B
        {
            public void F() { }
        }
    }
}
