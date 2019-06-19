using Pivotal.RouteService.Auth.Egress.Buildpack;
using System;
using System.IO;
using Xunit;

namespace UnitTests
{
    public class KerberosConfigUpdaterTests
    {
        [Fact]
        public void Test_If_FileIsModifiedAsExpected()
        {
            var originalFile = Path.Combine(Environment.CurrentDirectory, "krb5.ini");
            var testFile = Path.Combine(Environment.CurrentDirectory, "krb5_test.ini");
            var expectedFile = Path.Combine(Environment.CurrentDirectory, "expected_krb5.ini");

            File.Copy(originalFile, testFile, true);

            var updater = new KerberosConfigUpdater(testFile);
            updater.Update();

            Assert.Equal(File.ReadAllText(expectedFile), File.ReadAllText(testFile));
        }

        [Fact]
        public void Test_If_ThrowsExceptionIfAKeyIsMissing()
        {
            var originalFile = Path.Combine(Environment.CurrentDirectory, "krb5_bad.ini");
            var testFile = Path.Combine(Environment.CurrentDirectory, "krb5_bad_test.ini");

            File.Copy(originalFile, testFile, true);

            var updater = new KerberosConfigUpdater(testFile);

            Assert.Throws<ApplicationException>(() => updater.Update());
        }
    }
}
