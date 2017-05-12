using System;
using System.Diagnostics;
using System.Threading.Tasks;
using BusinessLayer.BusinessModels;

namespace TestBLL
{
    class Program
    {
        static void Main(string[] args)
        {


            var soc = new SocialNetworkFunctionalityUser("14c31b71-275a-44e5-aacf-f2c0b29ddeb0");
            Stopwatch timer = new Stopwatch();
            timer.Start();
            var friends = soc.Friends.GetFriendsAsync();
            var followers = soc.Friends.GetFollowersAsync();
            var followed = soc.Friends.GetFollowedAsync();
            //await Task.WhenAll(friends, followers, followed);
            Task.WaitAll();
            timer.Stop();
            Console.WriteLine(timer.ElapsedMilliseconds);
            Console.ReadKey();
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
