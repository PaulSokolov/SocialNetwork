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
            var temp = soc.Users.AddLanguageAsync(1, 1).GetAwaiter().GetResult();

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
