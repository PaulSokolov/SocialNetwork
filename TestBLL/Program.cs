using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using BusinessLayer.BusinessModels;
using BusinessLayer.DTO;
using Castle.Components.DictionaryAdapter;

namespace TestBLL
{
    class Program
    {
        static void Main(string[] args)
        {


            var soc = new SocialNetworkFunctionalityUser("14c31b71-275a-44e5-aacf-f2c0b29ddeb0");
            Stopwatch timer = new Stopwatch();
            timer.Start();
            List<Task<List<CityDTO> >> tasks = new EditableList<Task<List<CityDTO>>>();
            Parallel.For(0, 10,i=>
            {
                tasks.Add(soc.Database.GetCitiesAsync(1));
                //var followers = soc.Friends.GetFollowersAsync();
                //var followed = soc.Friends.GetFollowedAsync();
                //await Task.WhenAll(friends, followers, followed);
            });
            Task.WaitAll();

            Parallel.ForEach(tasks, task =>
            {
                var p = task.Result;
            });
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
