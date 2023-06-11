using AppDomainConcept;
using System;
using System.Security;
using System.Security.Permissions;

//ref link:https://www.youtube.com/watch?v=DUq84e3cZyo
// Application domain is a logically isolated container in which .NET code runs.

namespace AppDomainConcept
{
    class Program
    {
        static void Main(string[] args)
        {
            var perm = new PermissionSet(PermissionState.None);

            perm.AddPermission(
                new SecurityPermission(
                    SecurityPermissionFlag.Execution));

            perm.AddPermission(
                new FileIOPermission(
                    FileIOPermissionAccess.NoAccess, @"c:\"));
            var setup = new AppDomainSetup();

            setup.ApplicationBase = AppDomain.
                CurrentDomain.SetupInformation.ApplicationBase;

            // App domain which secured c:\
            //ThirdParty third = new ThirdParty();
            AppDomain securedDomain = AppDomain.
                CreateDomain("securedDomain",
                null, setup, perm);
            try
            {
                
                Type thirdparty = typeof(ThirdParty);
                securedDomain.
                    CreateInstanceAndUnwrap(
                    thirdparty.Assembly.FullName, thirdparty.FullName);
            }
            catch(Exception ex)
            {
                AppDomain.Unload(securedDomain);
            }
            
            // In to the current app domain
            Class1 obj1 = new Class1();
            Class2 obj2 = new Class2();
            Console.Read();
        }
    }
    [Serializable] // ref: 
    class ThirdParty
    {   //ctor tab
        public ThirdParty()
        {
            Console.WriteLine("Third party loaded");
            System.IO.File.Create(@"c:\x.txt"); // ref: How to work with files and folders(System.IO)?
        }
        ~ThirdParty()
        {
            Console.WriteLine("Third party unloaded");
        }

    }
    class Class1
    {

    }
    class Class2
    {

    }
}
