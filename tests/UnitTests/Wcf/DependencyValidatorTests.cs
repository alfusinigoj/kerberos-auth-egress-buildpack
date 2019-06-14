using Pivotal.RouteService.Auth.Egress.Buildpack.Wcf;
using System;
using System.IO;
using Xunit;

namespace UnitTests.Wcf
{
    public class DependencyValidatorTests
    {
        string interceptorDll = Path.Combine(Environment.CurrentDirectory, "assembly", "RouteServiceIwaWcfInterceptor.dll");
        string kerberosDll = Path.Combine(Environment.CurrentDirectory, "assembly", "Microsoft.AspNetCore.Authentication.GssKerberos.dll");

        public DependencyValidatorTests()
        {
            Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, "assembly"));
        }

        [Fact]
        public void Test_IfDoesNotThrowExecptionIfBothOfTheDllsExist()
        {
            File.WriteAllText(interceptorDll,"");
            File.WriteAllText(kerberosDll, "");
            var validator = new DependencyValidator(Path.Combine(Environment.CurrentDirectory, "assembly"));
            validator.Validate();
        }

        [Fact]
        public void Test_IfThrowsExecptionIfRouteServiceIwaWcfInterceptorIsMissing()
        {
            File.WriteAllText(interceptorDll, "");
            File.WriteAllText(kerberosDll, "");

            File.Delete(interceptorDll);
            var validator = new DependencyValidator(Path.Combine(Environment.CurrentDirectory, "assembly"));
            Assert.Throws<Exception>(()=>validator.Validate());
        }

        [Fact]
        public void Test_IfThrowsExecptionIfGssKerberosIsMissing()
        {
            File.WriteAllText(interceptorDll, "");
            File.WriteAllText(kerberosDll, "");

            File.Delete(kerberosDll);
            var validator = new DependencyValidator(Path.Combine(Environment.CurrentDirectory, "assembly"));
            Assert.Throws<Exception>(() => validator.Validate());
        }

        [Fact]
        public void Test_IfThrowsExecptionIfRouteServiceIwaWcfInterceptorAndGssKerberosIsMissing()
        {
            File.WriteAllText(interceptorDll, "");
            File.WriteAllText(kerberosDll, "");

            File.Delete(interceptorDll);
            File.Delete(kerberosDll);
            var validator = new DependencyValidator(Path.Combine(Environment.CurrentDirectory, "assembly"));
            Assert.Throws<Exception>(() => validator.Validate());
        }
    }
}
