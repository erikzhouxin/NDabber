using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Data.Cobber;
using System.Data.Hopper;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NullValueHandling = System.Data.Hopper.NullValueHandling;

namespace System.Data.DabberUT
{
    [TestFixture]
    public class CookieTests : HttpTestFixtureBase
    {
        [Test]
        public async Task can_send_and_receive_cookies_per_request()
        {
            HttpTest
                .RespondWith("hi", cookies: new { x = "bar" })
                .RespondWith("hi")
                .RespondWith("hi");

            // explicitly reuse client to be extra certain we're NOT maintaining cookie state between calls.
            var cli = new FlurlClient("https://cookies.com");
            var responses = new[] {
                await cli.Request().WithCookie("x", "foo").GetAsync(),
                await cli.Request().WithCookies(new { y = "bar", z = "fizz" }).GetAsync(),
                await cli.Request().GetAsync()
            };

            HttpTest.ShouldHaveMadeACall().WithCookies(new { x = "foo" }).Times(1);
            HttpTest.ShouldHaveMadeACall().WithCookies(new { y = "bar", z = "fizz" }).Times(1);
            HttpTest.ShouldHaveMadeACall().WithoutCookies().Times(1);

            Assert.AreEqual("bar", responses[0].Cookies.FirstOrDefault(c => c.Name == "x")?.Value);
            Assert.IsEmpty(responses[1].Cookies);
            Assert.IsEmpty(responses[2].Cookies);
        }

        [Test]
        public async Task can_send_and_receive_cookies_with_jar()
        {
            HttpTest
                .RespondWith("hi", cookies: new { x = "foo", y = "bar" })
                .RespondWith("hi")
                .RespondWith("hi", cookies: new { y = "bazz" })
                .RespondWith("hi");

            var responses = new[] {
                await "https://cookies.com".WithCookies(out var jar).GetAsync(),
                await "https://cookies.com/1".WithCookies(jar).GetAsync(),
                await "https://cookies.com".WithCookies(jar).GetAsync(),
                await "https://cookies.com/2".WithCookies(jar).GetAsync()
            };

            Assert.AreEqual(2, responses[0].Cookies.Count);
            Assert.AreEqual(0, responses[1].Cookies.Count);
            Assert.AreEqual(1, responses[2].Cookies.Count);
            Assert.AreEqual(0, responses[3].Cookies.Count);

            HttpTest.ShouldHaveMadeACall().WithCookies(new { x = "foo", y = "bar" }).Times(2);
            HttpTest.ShouldHaveMadeACall().WithCookies(new { x = "foo", y = "bazz" }).Times(1);

            Assert.AreEqual(2, jar.Count);
            Assert.AreEqual(1, jar.Count(c => c.Name == "x" && c.Value == "foo"));
            Assert.AreEqual(1, jar.Count(c => c.Name == "y" && c.Value == "bazz"));
        }

        [Test]
        public async Task can_send_and_receive_cookies_with_jar_initialized()
        {
            HttpTest
                .RespondWith("hi")
                .RespondWith("hi")
                .RespondWith("hi", cookies: new { y = "bazz" })
                .RespondWith("hi");

            var jar = new CookieJar()
                .AddOrReplace("x", "foo", "https://cookies.com")
                .AddOrReplace("y", "bar", "https://cookies.com");

            await "https://cookies.com".WithCookies(jar).GetAsync();
            await "https://cookies.com/1".WithCookies(jar).GetAsync();
            await "https://cookies.com".WithCookies(jar).GetAsync();
            await "https://cookies.com/2".WithCookies(jar).GetAsync();

            HttpTest.ShouldHaveMadeACall().WithCookies(new { x = "foo", y = "bar" }).Times(3);
            HttpTest.ShouldHaveMadeACall().WithCookies(new { x = "foo", y = "bazz" }).Times(1);

            Assert.AreEqual(2, jar.Count);
            Assert.AreEqual(1, jar.Count(c => c.Name == "x" && c.Value == "foo"));
            Assert.AreEqual(1, jar.Count(c => c.Name == "y" && c.Value == "bazz"));
        }

        [Test]
        public async Task can_do_cookie_session()
        {
            HttpTest
                .RespondWith("hi", cookies: new { x = "foo", y = "bar" })
                .RespondWith("hi")
                .RespondWith("hi", cookies: new { y = "bazz" })
                .RespondWith("hi");

            using (var cs = new CookieSession("https://cookies.com"))
            {
                await cs.Request().GetAsync();
                await cs.Request("1").GetAsync();
                await cs.Request().GetAsync();
                await cs.Request("2").GetAsync();

                var cookies = HttpTest.CallLog.Select(c => c.Request.Cookies).ToList();

                HttpTest.ShouldHaveMadeACall().WithCookies(new { x = "foo", y = "bar" }).Times(2);
                HttpTest.ShouldHaveMadeACall().WithCookies(new { x = "foo", y = "bazz" }).Times(1);

                Assert.AreEqual(2, cs.Cookies.Count);
                Assert.AreEqual(1, cs.Cookies.Count(c => c.Name == "x" && c.Value == "foo"));
                Assert.AreEqual(1, cs.Cookies.Count(c => c.Name == "y" && c.Value == "bazz"));
            }
        }

        [Test]
        public void can_parse_set_cookie_header()
        {
            var start = DateTimeOffset.UtcNow;
            var cookie = FlurlExtensions.ParseResponseHeader("https://www.cookies.com/a/b", "x=foo  ; DoMaIn=cookies.com  ;     path=/  ; MAX-AGE=999 ; expires= ;  secure ;HTTPONLY ;samesite=none");
            Assert.AreEqual("https://www.cookies.com/a/b", cookie.OriginUrl.ToString());
            Assert.AreEqual("x", cookie.Name);
            Assert.AreEqual("foo", cookie.Value);
            Assert.AreEqual("cookies.com", cookie.Domain);
            Assert.AreEqual("/", cookie.Path);
            Assert.AreEqual(999, cookie.MaxAge);
            Assert.IsNull(cookie.Expires);
            Assert.IsTrue(cookie.Secure);
            Assert.IsTrue(cookie.HttpOnly);
            Assert.AreEqual(SameSite.None, cookie.SameSite);
            Assert.GreaterOrEqual(cookie.DateReceived, start);
            Assert.LessOrEqual(cookie.DateReceived, DateTimeOffset.UtcNow);

            // simpler case
            start = DateTimeOffset.UtcNow;
            cookie = FlurlExtensions.ParseResponseHeader("https://www.cookies.com/a/b", "y=bar");
            Assert.AreEqual("https://www.cookies.com/a/b", cookie.OriginUrl.ToString());
            Assert.AreEqual("y", cookie.Name);
            Assert.AreEqual("bar", cookie.Value);
            Assert.IsNull(cookie.Domain);
            Assert.IsNull(cookie.Path);
            Assert.IsNull(cookie.MaxAge);
            Assert.IsNull(cookie.Expires);
            Assert.IsFalse(cookie.Secure);
            Assert.IsFalse(cookie.HttpOnly);
            Assert.IsNull(cookie.SameSite);
            Assert.GreaterOrEqual(cookie.DateReceived, start);
            Assert.LessOrEqual(cookie.DateReceived, DateTimeOffset.UtcNow);
        }

        [Test]
        public void cannot_change_cookie_after_adding_to_jar()
        {
            var cookie = new FlurlCookie("x", "foo", "https://cookies.com");

            // good
            cookie.Value = "value2";
            cookie.Path = "/";
            cookie.Secure = true;

            var jar = new CookieJar().AddOrReplace(cookie);

            // bad
            Assert.Throws<Exception>(() => cookie.Value = "value3");
            Assert.Throws<Exception>(() => cookie.Path = "/a");
            Assert.Throws<Exception>(() => cookie.Secure = false);
        }

        [Test]
        public void unquotes_cookie_value()
        {
            var cookie = FlurlExtensions.ParseResponseHeader("https://cookies.com", "x=\"hello there\"");
            Assert.AreEqual("hello there", cookie.Value);
        }

        [Test]
        public void jar_overwrites_request_cookies()
        {
            var jar = new CookieJar()
                .AddOrReplace("b", 10, "https://cookies.com")
                .AddOrReplace("c", 11, "https://cookies.com");

            var req = new FlurlRequest("http://cookies.com")
                .WithCookies(new { a = 1, b = 2 })
                .WithCookies(jar);

            Assert.AreEqual(3, req.Cookies.Count());
            Assert.IsTrue(req.Cookies.Contains(("a", "1")));
            Assert.IsTrue(req.Cookies.Contains(("b", "10"))); // the important one
            Assert.IsTrue(req.Cookies.Contains(("c", "11")));
        }

        [Test]
        public void request_cookies_do_not_overwrite_jar()
        {
            var jar = new CookieJar()
                .AddOrReplace("b", 10, "https://cookies.com")
                .AddOrReplace("c", 11, "https://cookies.com");

            var req = new FlurlRequest("http://cookies.com")
                .WithCookies(jar)
                .WithCookies(new { a = 1, b = 2 });

            Assert.AreEqual(3, req.Cookies.Count());
            Assert.IsTrue(req.Cookies.Contains(("a", "1")));
            Assert.IsTrue(req.Cookies.Contains(("b", "2"))); // value overwritten but just for this request
            Assert.IsTrue(req.Cookies.Contains(("c", "11")));

            // b in jar wasn't touched
            Assert.AreEqual("10", jar.FirstOrDefault(c => c.Name == "b")?.Value);
        }

        [Test]
        public void request_cookies_sync_with_cookie_header()
        {
            var req = new FlurlRequest("http://cookies.com").WithCookie("x", "foo");
            Assert.AreEqual("x=foo", req.Headers.FirstOrDefault("Cookie"));

            // should flow from CookieJar too
            var jar = new CookieJar().AddOrReplace("y", "bar", "http://cookies.com");
            req = new FlurlRequest("http://cookies.com").WithCookies(jar);
            Assert.AreEqual("y=bar", req.Headers.FirstOrDefault("Cookie"));
        }

        [TestCase("https://domain1.com", "https://domain1.com", true)]
        [TestCase("https://domain1.com", "https://domain1.com/path", true)]
        [TestCase("https://domain1.com", "https://www.domain1.com", false)]
        [TestCase("https://www.domain1.com", "https://domain1.com", false)]
        [TestCase("https://domain1.com", "https://domain2.com", false)]
        public void cookies_without_domain_restricted_to_origin_domain(string originUrl, string requestUrl, bool shouldSend)
        {
            var cookie = new FlurlCookie("x", "foo", originUrl);
            AssertCookie(cookie, true, false, requestUrl, shouldSend);
        }

        [TestCase("domain.com", "https://www.domain.com", true)]
        [TestCase("www.domain.com", "https://domain.com", false)] // not vice-versa
        public void cookies_with_domain_sent_to_subdomain(string cookieDomain, string requestUrl, bool shouldSend)
        {
            var cookie = new FlurlCookie("x", "foo", $"https://{cookieDomain}") { Domain = cookieDomain };
            AssertCookie(cookie, true, false, requestUrl, shouldSend);
        }

        [TestCase("/a", "/a", true)]
        [TestCase("/a", "/a/", true)]
        [TestCase("/a", "/a/hello", true)]
        [TestCase("/a", "/ab", false)]
        [TestCase("/a", "/", false)]
        public void cookies_with_path_sent_to_subpath(string cookiePath, string requestPath, bool shouldSend)
        {
            var origin = "https://cookies.com".AppendPathSegment(cookiePath);
            var cookie = new FlurlCookie("x", "foo", origin) { Path = cookiePath };
            var url = "https://cookies.com".AppendPathSegment(requestPath);
            AssertCookie(cookie, true, false, url, shouldSend);
        }

        // default path is /
        [TestCase("", "/", true)]
        [TestCase("", "/a/b/c", true)]
        [TestCase("/", "", true)]
        [TestCase("/", "/a/b/c", true)]
        [TestCase("/a", "", true)]
        [TestCase("/a", "/", true)]
        [TestCase("/a", "/a/b/c", true)]
        [TestCase("/a", "/x", true)]

        // default path is /a
        [TestCase("/a/", "", false)]
        [TestCase("/a/", "/", false)]
        [TestCase("/a/", "/a", true)]
        [TestCase("/a/", "/a/b/c", true)]
        [TestCase("/a/", "/a/x", true)]
        [TestCase("/a/", "/x", false)]
        [TestCase("/a/b", "", false)]
        [TestCase("/a/b", "/", false)]
        [TestCase("/a/b", "/a", true)]
        [TestCase("/a/b", "/a/b/c", true)]
        [TestCase("/a/b", "/a/x", true)]
        [TestCase("/a/b", "/x", false)]
        public void cookies_without_path_sent_to_origin_subpath(string originPath, string requestPath, bool shouldSend)
        {
            var origin = "https://cookies.com" + originPath;
            var cookie = new FlurlCookie("x", "foo", origin);
            var url = "https://cookies.com".AppendPathSegment(requestPath);
            AssertCookie(cookie, true, false, url, shouldSend);
        }

        [Test]
        public void secure_cookies_not_sent_to_insecure_url()
        {
            var cookie = new FlurlCookie("x", "foo", "https://cookies.com") { Secure = true };
            AssertCookie(cookie, true, false, "https://cookies.com", true);
            AssertCookie(cookie, true, false, "http://cookies.com", false);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void validates_expired_absolute(bool localTime)
        {
            var now = localTime ? DateTimeOffset.Now : DateTimeOffset.UtcNow;
            var c1 = new FlurlCookie("x", "foo", "https://cookies.com") { Expires = now.AddSeconds(-2) };
            var c2 = new FlurlCookie("x", "foo", "https://cookies.com") { Expires = now.AddSeconds(2) };
            AssertCookie(c1, true, true, "https://cookies.com", false);
            AssertCookie(c2, true, false, "https://cookies.com", true);
        }

        [TestCase(false)]
        [TestCase(true)]
        public void validates_expired_by_max_age(bool localTime)
        {
            var now = localTime ? DateTimeOffset.Now : DateTimeOffset.UtcNow;
            var c1 = new FlurlCookie("x", "foo", "https://cookies.com", now.AddSeconds(-3602)) { MaxAge = 3600 };
            var c2 = new FlurlCookie("x", "foo", "https://cookies.com", now.AddSeconds(-3598)) { MaxAge = 3600 };
            AssertCookie(c1, true, true, "https://cookies.com", false);
            AssertCookie(c2, true, false, "https://cookies.com", true);
        }

        [TestCase(null, true)]
        [TestCase("", true)] // spec says SHOULD ignore empty
        [TestCase("cookies", false)]
        [TestCase("cookies.com", true)]
        [TestCase(".cookies.com", true)] // "Contrary to earlier specifications, leading dots in domain names are ignored" https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/Set-Cookie
        [TestCase("ww.cookies.com", false)]
        [TestCase("www.cookies.com", true)]
        [TestCase(".www.cookies.com", true)]
        [TestCase("wwww.cookies.com", false)]
        [TestCase("cookies2.com", false)]
        [TestCase("mycookies.com", false)]
        [TestCase("https://www.cookies.com", false)]
        [TestCase("https://www.cookies.com/a", false)]
        [TestCase("www.cookies.com/a", false)]
        public void validates_domain(string domain, bool valid)
        {
            var cookie = new FlurlCookie("x", "foo", "https://www.cookies.com/a") { Domain = domain };
            AssertCookie(cookie, valid, false);
        }

        [Test]
        public void domain_cannot_be_ip_address()
        {
            var cookie = new FlurlCookie("x", "foo", "https://1.2.3.4");
            AssertCookie(cookie, true, false);

            // domain can't be set at all if origin is an IP, but that's kind of impossible to
            // test independently because it'll fail the domain match check first
            cookie = new FlurlCookie("x", "foo", "https://1.2.3.4") { Domain = "1.2.3.4" };
            AssertCookie(cookie, false, false);
        }

        [Test]
        public void validates_secure()
        {
            var cookie = new FlurlCookie("x", "foo", "http://insecure.com") { Secure = true };
            AssertCookie(cookie, false, false);
        }

        // https://developer.mozilla.org/en-US/docs/Web/HTTP/Cookies#Cookie_prefixes
        [TestCase("__Host-", "https://cookies.com", true, null, "/", true)]
        [TestCase("__Host-", "http://cookies.com", true, null, "/", false)]
        [TestCase("__Host-", "https://cookies.com", false, null, "/", false)]
        [TestCase("__Host-", "https://cookies.com", true, "cookies.com", "/", false)]
        [TestCase("__Host-", "https://cookies.com", true, null, null, false)]
        [TestCase("__Host-", "https://cookies.com", true, null, "/a", false)]

        [TestCase("__Secure-", "https://cookies.com", true, null, "/", true)]
        [TestCase("__Secure-", "http://cookies.com", true, null, "/", false)]
        [TestCase("__Secure-", "https://cookies.com", false, null, "/", false)]
        [TestCase("__Secure-", "https://cookies.com", true, "cookies.com", "/", true)]
        [TestCase("__Secure-", "https://cookies.com", true, null, null, true)]
        [TestCase("__Secure-", "https://cookies.com", true, null, "/a", true)]
        public void validates_cookie_prefix(string prefix, string origin, bool secure, string domain, string path, bool valid)
        {
            var cookie = new FlurlCookie(prefix + "x", "foo", origin.AppendPathSegment("a"))
            {
                Secure = secure,
                Domain = domain,
                Path = path
            };
            AssertCookie(cookie, valid, false);
        }

        [Test]
        public async Task invalid_cookie_in_response_doesnt_throw()
        {
            HttpTest.RespondWith("hi", headers: new { Set_Cookie = "x=foo; Secure" });
            var resp = await "http://insecure.com".WithCookies(out var jar).GetAsync();

            Assert.IsEmpty(jar);
            // even though the CookieJar rejected the cookie, it doesn't change the fact
            // that it exists in the response.
            Assert.AreEqual("foo", resp.Cookies.FirstOrDefault(c => c.Name == "x")?.Value);
        }

        [Test]
        public async Task multiple_cookies_same_name_picks_longest_path()
        {
            HttpTest.RespondWith("hi", 200, new[] {
                ("Set-Cookie", "x=foo1; Path=/"),
                ("Set-Cookie", "x=foo2; Path=/"), // overwrites
				("Set-Cookie", "x=foo3; Path=/a/b"), // doesn't overwrite, longer path should be listed first
				("Set-Cookie", "y=bar")
            });

            var resp = await "https://cookies.com".WithCookies(out var jar).GetAsync();
            Assert.AreEqual(4, resp.Headers.Count(h => h.Name == "Set-Cookie"));
            Assert.AreEqual(4, resp.Cookies.Count);

            var req = "https://cookies.com/a/b".WithCookies(jar);
            Assert.AreEqual("x=foo3; x=foo2; y=bar", req.Headers.FirstOrDefault("Cookie"));
        }

        [Test]
        public async Task expired_deletes_from_jar()
        {
            // because the standard https://stackoverflow.com/a/53573622/62600
            HttpTest
                .RespondWith("", headers: new[] {
                    ("Set-Cookie", "x=foo"),
                    ("Set-Cookie", "y=bar"),
                    ("Set-Cookie", "z=bazz")
                })
                .RespondWith("", headers: new[] { ("Set-Cookie", $"x=foo; Expires={DateTime.UtcNow.AddSeconds(-1):R}") })
                .RespondWith("", headers: new[] { ("Set-Cookie", "y=bar; Max-Age=0") })
                // not relevant to the request so shouldn't be deleted
                .RespondWith("", headers: new[] { ("Set-Cookie", "z=bazz; Path=/a; Max-Age=0") });

            await "https://cookies.com".WithCookies(out var jar).GetAsync();
            Assert.AreEqual(3, jar.Count);

            await "https://cookies.com".WithCookies(jar).GetAsync();
            Assert.AreEqual(2, jar.Count);
            Assert.AreEqual("y", jar.Select(c => c.Name).OrderBy(n => n).First());

            await "https://cookies.com".WithCookies(jar).GetAsync();
            Assert.AreEqual(1, jar.Count);
            Assert.AreEqual("z", jar.Single().Name);

            await "https://cookies.com".WithCookies(jar).GetAsync();
            Assert.AreEqual(1, jar.Count);
            Assert.AreEqual("z", jar.Single().Name);
        }

        [Test]
        public void names_are_case_sensitive()
        {
            var req = new FlurlRequest().WithCookie("a", 1).WithCookie("A", 2).WithCookie("a", 3);
            Assert.AreEqual(2, req.Cookies.Count());
            CollectionAssert.AreEquivalent(new[] { "a", "A" }, req.Cookies.Select(c => c.Name));
            CollectionAssert.AreEquivalent(new[] { "3", "2" }, req.Cookies.Select(c => c.Value));

            var jar = new CookieJar()
                .AddOrReplace("a", 1, "https://cookies.com")
                .AddOrReplace("A", 2, "https://cookies.com")
                .AddOrReplace("a", 3, "https://cookies.com");
            Assert.AreEqual(2, jar.Count);
            CollectionAssert.AreEquivalent(new[] { "a", "A" }, jar.Select(c => c.Name));
            CollectionAssert.AreEquivalent(new[] { "3", "2" }, jar.Select(c => c.Value));
        }

        [Test]
        public async Task cookie_received_from_redirect_response_is_added_to_jar()
        {
            HttpTest
                .RespondWith("redir", 302, new { Location = "/redir1" }, cookies: new { x = "foo" })
                .RespondWith("hi", cookies: new { y = "bar" });

            await "https://cookies.com".WithCookies(out var jar).GetAsync();

            Assert.AreEqual(2, jar.Count);
            Assert.AreEqual(1, jar.Count(c => c.Name == "x" && c.Value == "foo"));
            Assert.AreEqual(1, jar.Count(c => c.Name == "y" && c.Value == "bar"));
        }

        [Test]
        public async Task modified_cookie_forwarded_on_redirect()
        {
            // https://github.com/tmenier/Flurl/issues/608#issuecomment-799699525
            HttpTest
                .RespondWith("redir", 302, new { Location = "/redir" }, cookies: new { x = "changed" })
                .RespondWith("hi", cookies: new { x = "changed2" });

            var jar = new CookieJar().AddOrReplace("x", "original", "https://cookies.com");
            await "https://cookies.com".WithCookies(jar).GetAsync();

            HttpTest.ShouldHaveCalled("https://cookies.com").WithCookie("x", "original");
            HttpTest.ShouldHaveCalled("https://cookies.com/redir").WithCookie("x", "changed");
            Assert.IsTrue(jar.Any(c => c.Name == "x" && c.Value == "changed2"));
        }

        /// <summary>
        /// Performs a series of behavioral checks against a cookie based on its state. Used by lots of tests to make them more robust.
        /// </summary>
        private void AssertCookie(FlurlCookie cookie, bool isValid, bool isExpired, string requestUrl = null, bool shouldSend = false)
        {
            Assert.AreEqual(isValid, cookie.IsValid(out var reason), reason);
            Assert.AreEqual(isExpired, cookie.IsExpired(out reason), reason);

            var shouldAddToJar = isValid && !isExpired;
            var jar = new CookieJar();
            Assert.AreEqual(shouldAddToJar, jar.TryAddOrReplace(cookie, out reason));

            if (shouldAddToJar)
                Assert.AreEqual(cookie.Name, jar.SingleOrDefault()?.Name);
            else
            {
                Assert.Throws<InvalidCookieException>(() => jar.AddOrReplace(cookie));
                CollectionAssert.IsEmpty(jar);
            }

            var req = cookie.OriginUrl.WithCookies(jar);
            Assert.AreEqual(shouldAddToJar, req.Cookies.Contains((cookie.Name, cookie.Value)));

            if (requestUrl != null)
            {
                Assert.AreEqual(shouldSend, cookie.ShouldSendTo(requestUrl, out reason), reason);
                req = requestUrl.WithCookies(jar);
                Assert.AreEqual(shouldSend, req.Cookies.Contains((cookie.Name, cookie.Value)));
            }
        }
    }
    [TestFixture, Parallelizable]
    public class DefaultUrlEncodedSerializerTests
    {
        [Test]
        public void can_serialize_object()
        {
            var vals = new
            {
                a = "foo",
                b = 333,
                c = (string)null, // exclude
                d = ""
            };

            var serialized = new DefaultUrlEncodedSerializer().Serialize(vals);
            Assert.AreEqual("a=foo&b=333&d=", serialized);
        }
    }
    [TestFixture]
    public class FlurlClientFactoryTests
    {
        [Test]
        public void default_factory_provides_same_client_per_host_scheme_port()
        {
            var fac = new DefaultFlurlClientFactory();
            var cli1 = fac.Get("http://api.com/foo");
            var cli2 = fac.Get("http://api.com/bar");
            var cli3 = fac.Get("https://api.com/foo");
            var cli4 = fac.Get("https://api.com/bar");
            var cli5 = fac.Get("https://api.com:1234/foo");
            var cli6 = fac.Get("https://api.com:1234/bar");

            Assert.AreSame(cli1, cli2);
            Assert.AreSame(cli3, cli4);
            Assert.AreSame(cli5, cli6);

            Assert.AreNotSame(cli1, cli3);
            Assert.AreNotSame(cli3, cli5);
        }

        [Test]
        public void per_base_url_factory_provides_same_client_per_provided_url()
        {
            var fac = new PerBaseUrlFlurlClientFactory();
            var cli1 = fac.Get("http://api.com/foo");
            var cli2 = fac.Get("http://api.com/bar");
            var cli3 = fac.Get("http://api.com/foo");
            Assert.AreNotSame(cli1, cli2);
            Assert.AreSame(cli1, cli3);
        }

        [Test]
        public void can_configure_client_from_factory()
        {
            var fac = new DefaultFlurlClientFactory()
                .ConfigureClient("http://api.com/foo", c => c.Settings.Timeout = TimeSpan.FromSeconds(123));
            Assert.AreEqual(TimeSpan.FromSeconds(123), fac.Get("http://api.com/bar").Settings.Timeout);
            Assert.AreNotEqual(TimeSpan.FromSeconds(123), fac.Get("http://api2.com/foo").Settings.Timeout);
        }

        [Test]
        public async Task ConfigureClient_is_thread_safe()
        {
            var fac = new DefaultFlurlClientFactory();
            var sequence = new List<int>();

            var task1 = Task.Run(() => fac.ConfigureClient("http://api.com", c => {
                sequence.Add(1);
                Thread.Sleep(5000);
                sequence.Add(3);
            }));

            await Task.Delay(200);

            // modifies same client as task1, should get blocked until task1 is done
            var task2 = Task.Run(() => fac.ConfigureClient("http://api.com", c => {
                sequence.Add(4);
            }));

            await Task.Delay(200);

            // modifies different client, should run immediately
            var task3 = Task.Run(() => fac.ConfigureClient("http://api2.com", c => {
                sequence.Add(2);
            }));

            await Task.WhenAll(task1, task2, task3);
            Assert.AreEqual("1,2,3,4", string.Join(",", sequence));
        }
    }
    [TestFixture, Parallelizable]
    public class FlurlClientTests
    {
        [Test]
        // check that for every FlurlClient extension method, we have an equivalent Url and string extension
        public void extension_methods_consistently_supported()
        {
            var reqExts = ReflectionHelper.GetAllExtensionMethods<IFlurlRequest>(typeof(FlurlClient).GetTypeInfo().Assembly)
                // URL builder methods on IFlurlClient get a free pass. We're looking for things like HTTP calling methods.
                .Where(mi => mi.DeclaringType != typeof(FlurlExtensions))
                .ToList();
            var urlExts = ReflectionHelper.GetAllExtensionMethods<Flurl>(typeof(FlurlClient).GetTypeInfo().Assembly).ToList();
            var stringExts = ReflectionHelper.GetAllExtensionMethods<string>(typeof(FlurlClient).GetTypeInfo().Assembly).ToList();
            var uriExts = ReflectionHelper.GetAllExtensionMethods<Uri>(typeof(FlurlClient).GetTypeInfo().Assembly).ToList();

            Assert.That(reqExts.Count > 20, $"IFlurlRequest only has {reqExts.Count} extension methods? Something's wrong here.");

            // Url and string should contain all extension methods that IFlurlRequest has
            foreach (var method in reqExts)
            {
                if (!urlExts.Any(m => ReflectionHelper.AreSameMethodSignatures(method, m)))
                {
                    var args = string.Join(", ", method.GetParameters().Select(p => p.ParameterType.Name));
                    Assert.Fail($"No equivalent Url extension method found for IFlurlRequest.{method.Name}({args})");
                }
                if (!stringExts.Any(m => ReflectionHelper.AreSameMethodSignatures(method, m)))
                {
                    var args = string.Join(", ", method.GetParameters().Select(p => p.ParameterType.Name));
                    Assert.Fail($"No equivalent string extension method found for IFlurlRequest.{method.Name}({args})");
                }
                if (!uriExts.Any(m => ReflectionHelper.AreSameMethodSignatures(method, m)))
                {
                    var args = string.Join(", ", method.GetParameters().Select(p => p.ParameterType.Name));
                    Assert.Fail($"No equivalent System.Uri extension method found for IFlurlRequest.{method.Name}({args})");
                }
            }
        }

        [Test]
        public void can_create_request_without_base_url()
        {
            var cli = new FlurlClient();
            var req = cli.Request("http://myapi.com/foo?x=1&y=2#foo");
            Assert.AreEqual("http://myapi.com/foo?x=1&y=2#foo", req.Url.ToString());
        }

        [Test]
        public void can_create_request_with_base_url()
        {
            var cli = new FlurlClient("http://myapi.com");
            var req = cli.Request("foo", "bar");
            Assert.AreEqual("http://myapi.com/foo/bar", req.Url.ToString());
        }

        [Test]
        public void request_with_full_url_overrides_base_url()
        {
            var cli = new FlurlClient("http://myapi.com");
            var req = cli.Request("http://otherapi.com", "foo");
            Assert.AreEqual("http://otherapi.com/foo", req.Url.ToString());
        }

        [Test]
        public void can_create_request_with_base_url_and_no_segments()
        {
            var cli = new FlurlClient("http://myapi.com");
            var req = cli.Request();
            Assert.AreEqual("http://myapi.com", req.Url.ToString());
        }

        [Test]
        public void cannot_send_invalid_request()
        {
            var cli = new FlurlClient();
            Assert.ThrowsAsync<ArgumentNullException>(() => cli.SendAsync(null));
            Assert.ThrowsAsync<ArgumentException>(() => cli.SendAsync(new FlurlRequest()));
            Assert.ThrowsAsync<ArgumentException>(() => cli.SendAsync(new FlurlRequest("/relative/url")));
        }

        [Test]
        public async Task default_factory_doesnt_reuse_disposed_clients()
        {
            var req1 = "http://api.com".WithHeader("foo", "1");
            var req2 = "http://api.com".WithHeader("foo", "2");
            var req3 = "http://api.com".WithHeader("foo", "3");

            // client not assigned until request is sent
            using var test = new HttpTest();
            await req1.GetAsync();
            await req2.GetAsync();
            req1.Client.Dispose();
            await req3.GetAsync();

            Assert.AreEqual(req1.Client, req2.Client);
            Assert.IsTrue(req1.Client.IsDisposed);
            Assert.IsTrue(req2.Client.IsDisposed);
            Assert.AreNotEqual(req1.Client, req3.Client);
            Assert.IsFalse(req3.Client.IsDisposed);
        }

        [Test]
        public void can_create_FlurlClient_with_existing_HttpClient()
        {
            var hc = new HttpClient
            {
                BaseAddress = new Uri("http://api.com/"),
                Timeout = TimeSpan.FromSeconds(123)
            };
            var cli = new FlurlClient(hc);

            Assert.AreEqual("http://api.com/", cli.HttpClient.BaseAddress.ToString());
            Assert.AreEqual(123, cli.HttpClient.Timeout.TotalSeconds);
            Assert.AreEqual("http://api.com/", cli.BaseUrl);
        }

        [Test] // #334
        public void can_dispose_FlurlClient_created_with_HttpClient()
        {
            var hc = new HttpClient();
            var fc = new FlurlClient(hc);
            fc.Dispose();

            // ensure the HttpClient got disposed
            Assert.ThrowsAsync<ObjectDisposedException>(() => hc.GetAsync("http://mysite.com"));
        }
    }
    [TestFixture, Parallelizable]
    class FlurlHttpExceptionTests : HttpTestFixtureBase
    {
        [Test]
        public async Task exception_message_is_nice()
        {
            HttpTest.RespondWithJson(new { message = "bad data!" }, 400);

            try
            {
                await "http://myapi.com".PostJsonAsync(new { data = "bad" });
                Assert.Fail("should have thrown 400.");
            }
            catch (FlurlHttpException ex)
            {
                Assert.AreEqual("Call failed with status code 400 (Bad Request): POST http://myapi.com", ex.Message);
            }
        }

        [Test]
        public async Task exception_message_excludes_request_response_labels_when_body_empty()
        {
            HttpTest.RespondWith("", 400);

            try
            {
                await "http://myapi.com".GetAsync();
                Assert.Fail("should have thrown 400.");
            }
            catch (FlurlHttpException ex)
            {
                // no "Request body:", "Response body:", or line breaks
                Assert.AreEqual("Call failed with status code 400 (Bad Request): GET http://myapi.com", ex.Message);
            }
        }

        [Test]
        public async Task can_catch_parsing_error()
        {
            HttpTest.RespondWith("{ \"invalid JSON!");

            try
            {
                await "http://myapi.com".GetJsonAsync<object>();
                Assert.Fail("should have failed to parse response.");
            }
            catch (FlurlParsingException ex)
            {
                Assert.AreEqual("Response could not be deserialized to JSON: GET http://myapi.com", ex.Message);
                // these are equivalent:
                Assert.AreEqual("{ \"invalid JSON!", await ex.GetResponseStringAsync());
                Assert.AreEqual("{ \"invalid JSON!", await ex.Call.Response.GetStringAsync());
                // will differ if you're using a different serializer (which you probably aren't):
                Assert.IsInstanceOf<System.Text.Json.JsonException>(ex.InnerException);
            }
        }

        [Test] // #579
        public void can_create_empty()
        {
            var ex = new FlurlHttpException(null);
            Assert.AreEqual("Call failed.", ex.Message);
        }
    }
    [TestFixture, Parallelizable]
    public class GetTests : HttpMethodTests
    {
        public GetTests() : base(HttpMethod.Get) { }

        protected override Task<IFlurlResponse> CallOnString(string url) => url.GetAsync();
        protected override Task<IFlurlResponse> CallOnUrl(Flurl url) => url.GetAsync();
        protected override Task<IFlurlResponse> CallOnFlurlRequest(IFlurlRequest req) => req.GetAsync();

        [Test]
        public async Task can_get_json()
        {
            HttpTest.RespondWithJson(new { id = 1, name = "Frank" });

            var data = await "http://some-api.com".GetJsonAsync<TestData>();

            Assert.AreEqual(1, data.Id);
            Assert.AreEqual("Frank", data.Name);
        }

        [Test]
        public async Task can_get_response_then_deserialize()
        {
            // FlurlResponse was introduced in 3.0. I don't think we need to go crazy with new tests, because existing
            // methods like FlurlRequest.GetJson, ReceiveJson, etc all go through FlurlResponse now.
            HttpTest.RespondWithJson(new { id = 1, name = "Frank" }, 234, new { my_header = "hi" }, null, true);

            var resp = await "http://some-api.com".GetAsync();
            Assert.AreEqual(234, resp.StatusCode);
            Assert.IsTrue(resp.Headers.TryGetFirst("my-header", out var headerVal));
            Assert.AreEqual("hi", headerVal);

            var data = await resp.GetJsonAsync<TestData>();
            Assert.AreEqual(1, data.Id);
            Assert.AreEqual("Frank", data.Name);
        }

        [Test]
        public async Task can_get_string()
        {
            HttpTest.RespondWith("good job");

            var data = await "http://some-api.com".GetStringAsync();

            Assert.AreEqual("good job", data);
        }

        [Test]
        public async Task can_get_stream()
        {
            HttpTest.RespondWith("good job");

            var data = await "http://some-api.com".GetStreamAsync();

            Assert.AreEqual(new MemoryStream(Encoding.UTF8.GetBytes("good job")), data);
        }

        [Test]
        public async Task can_get_bytes()
        {
            HttpTest.RespondWith("good job");

            var data = await "http://some-api.com".GetBytesAsync();

            Assert.AreEqual(Encoding.UTF8.GetBytes("good job"), data);
        }

        [Test]
        public async Task failure_throws_detailed_exception()
        {
            HttpTest.RespondWith("bad job", status: 500);

            try
            {
                await "http://api.com".GetStringAsync();
                Assert.Fail("FlurlHttpException was not thrown!");
            }
            catch (FlurlHttpException ex)
            {
                Assert.AreEqual("http://api.com/", ex.Call.HttpRequestMessage.RequestUri.AbsoluteUri);
                Assert.AreEqual(HttpMethod.Get, ex.Call.HttpRequestMessage.Method);
                Assert.AreEqual(500, ex.Call.Response.StatusCode);
                // these should be equivalent:
                Assert.AreEqual("bad job", await ex.Call.Response.GetStringAsync());
                Assert.AreEqual("bad job", await ex.GetResponseStringAsync());
            }
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task can_get_error_json(bool useShortcut)
        {
            HttpTest.RespondWithJson(new { code = 999, message = "our server crashed" }, 500);

            try
            {
                await "http://api.com".GetStringAsync();
            }
            catch (FlurlHttpException ex)
            {
                var error = useShortcut ?
                    await ex.GetResponseJsonAsync<TestError>() :
                    await ex.Call.Response.GetJsonAsync<TestError>();
                Assert.IsNotNull(error);
                Assert.AreEqual(999, error.Code);
                Assert.AreEqual("our server crashed", error.Message);
            }
        }

        [Test]
        public async Task can_get_null_json_when_timeout_and_exception_handled()
        {
            HttpTest.SimulateTimeout();
            var data = await "http://api.com"
                .ConfigureRequest(c => c.OnError = call => call.ExceptionHandled = true)
                .GetJsonAsync<TestData>();
            Assert.IsNull(data);
        }

        // https://github.com/tmenier/Flurl/pull/76
        // quotes around charset value is technically legal but there's a bug in .NET we want to avoid: https://github.com/dotnet/corefx/issues/5014
        [Test]
        public async Task can_get_string_with_quoted_charset_header()
        {
            HttpTest.RespondWith(() => {
                var content = new StringContent("foo");
                content.Headers.Clear();
                content.Headers.Add("Content-Type", "text/javascript; charset=\"UTF-8\"");
                return content;
            });

            var resp = await "http://api.com".GetStringAsync(); // without StripCharsetQuotes, this fails
            Assert.AreEqual("foo", resp);
        }

        [Test] // #313
        public async Task can_setting_content_header_with_no_content()
        {
            await "http://api.com"
                .WithHeader("Content-Type", "application/json")
                .GetAsync();

            HttpTest.ShouldHaveMadeACall().WithContentType("application/json");
        }

        [Test] // #571
        public async Task can_deserialize_after_callback_reads_string()
        {
            HttpTest.RespondWithJson(new { id = 123, name = "foo" });
            string logMe = null;
            var result = await new FlurlRequest("http://api.com")
                .AfterCall(async call => logMe = await call.Response.GetStringAsync())
                .GetJsonAsync<TestData>();

            Assert.IsNotNull(result);
            Assert.AreEqual(123, result.Id);
            Assert.AreEqual("foo", result.Name);
            Assert.AreEqual("{\"id\":123,\"name\":\"foo\"}", logMe);
        }

        [Test] // #571 (opposite of previous test and probably less common)
        public async Task can_read_string_after_callback_deserializes()
        {
            HttpTest.RespondWithJson(new { id = 123, name = "foo" });
            TestData logMe = null;
            var result = await new FlurlRequest("http://api.com")
                .AfterCall(async call => logMe = await call.Response.GetJsonAsync<TestData>())
                .GetStringAsync();

            Assert.AreEqual("{\"Id\":123,\"Name\":\"foo\"}", result);
            Assert.IsNotNull(logMe);
            Assert.AreEqual(123, logMe.Id);
            Assert.AreEqual("foo", logMe.Name);
        }

        [Test] // #571
        public async Task can_deserialize_as_different_type_than_callback()
        {
            HttpTest.RespondWithJson(new { id = 123, somethingElse = "bar" });
            TestData logMe = null;
            var result = await new FlurlRequest("http://api.com")
                .AfterCall(async call => logMe = await call.Response.GetJsonAsync<TestData>())
                .GetJsonAsync<TestData2>();

            Assert.IsNotNull(result);
            Assert.AreEqual(123, result.Id);
            // This doesn't work because we deserialized to TestData first, which doesn't have somethingElse, so that value is lost.
            //Assert.AreEqual("bar", result.somethingElse);
            Assert.IsNull(result.SomethingElse);

            Assert.IsNotNull(logMe);
            Assert.AreEqual(123, logMe.Id);
            Assert.IsNull(logMe.Name);
        }

        // Most tests above intentionally respond with camelCase JSON properties, while the C# models
        // use TitleCase, to verify case-insensitive default deserialization (#719)

        private class TestData
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        private class TestData2
        {
            public int Id { get; set; }
            public string SomethingElse { get; set; }
        }

        private class TestError
        {
            public int Code { get; set; }
            public string Message { get; set; }
        }
    }
    /// <summary>
    /// Each HTTP method with first-class support in Flurl (via PostAsync, GetAsync, etc.) should
    /// have a test fixture that inherits from this base class.
    /// </summary>
    public abstract class HttpMethodTests : HttpTestFixtureBase
    {
        private readonly HttpMethod _verb;

        protected HttpMethodTests(HttpMethod verb)
        {
            _verb = verb;
        }

        protected abstract Task<IFlurlResponse> CallOnString(string url);
        protected abstract Task<IFlurlResponse> CallOnUrl(Flurl url);
        protected abstract Task<IFlurlResponse> CallOnFlurlRequest(IFlurlRequest req);

        [Test]
        public async Task can_call_on_FlurlClient()
        {
            var resp = await CallOnFlurlRequest(new FlurlRequest("http://www.api.com"));
            Assert.AreEqual(200, resp.StatusCode);
            HttpTest.ShouldHaveCalled("http://www.api.com").WithVerb(_verb).Times(1);
        }

        [Test]
        public async Task can_call_on_string()
        {
            var resp = await CallOnString("http://www.api.com");
            Assert.AreEqual(200, resp.StatusCode);
            HttpTest.ShouldHaveCalled("http://www.api.com").WithVerb(_verb).Times(1);
        }

        [Test]
        public async Task can_call_on_url()
        {
            var resp = await CallOnUrl(new Flurl("http://www.api.com"));
            Assert.AreEqual(200, resp.StatusCode);
            HttpTest.ShouldHaveCalled("http://www.api.com").WithVerb(_verb).Times(1);
        }
    }

    [TestFixture, Parallelizable]
    public class PutTests : HttpMethodTests
    {
        public PutTests() : base(HttpMethod.Put) { }
        protected override Task<IFlurlResponse> CallOnString(string url) => url.PutAsync(null);
        protected override Task<IFlurlResponse> CallOnUrl(Flurl url) => url.PutAsync(null);
        protected override Task<IFlurlResponse> CallOnFlurlRequest(IFlurlRequest req) => req.PutAsync(null);
    }

    [TestFixture, Parallelizable]
    public class PatchTests : HttpMethodTests
    {
        public PatchTests() : base(new HttpMethod("PATCH")) { }
        protected override Task<IFlurlResponse> CallOnString(string url) => url.PatchAsync(null);
        protected override Task<IFlurlResponse> CallOnUrl(Flurl url) => url.PatchAsync(null);
        protected override Task<IFlurlResponse> CallOnFlurlRequest(IFlurlRequest req) => req.PatchAsync(null);
    }

    [TestFixture, Parallelizable]
    public class DeleteTests : HttpMethodTests
    {
        public DeleteTests() : base(HttpMethod.Delete) { }
        protected override Task<IFlurlResponse> CallOnString(string url) => url.DeleteAsync();
        protected override Task<IFlurlResponse> CallOnUrl(Flurl url) => url.DeleteAsync();
        protected override Task<IFlurlResponse> CallOnFlurlRequest(IFlurlRequest req) => req.DeleteAsync();
    }

    [TestFixture, Parallelizable]
    public class HeadTests : HttpMethodTests
    {
        public HeadTests() : base(HttpMethod.Head) { }
        protected override Task<IFlurlResponse> CallOnString(string url) => url.HeadAsync();
        protected override Task<IFlurlResponse> CallOnUrl(Flurl url) => url.HeadAsync();
        protected override Task<IFlurlResponse> CallOnFlurlRequest(IFlurlRequest req) => req.HeadAsync();
    }

    [TestFixture, Parallelizable]
    public class OptionsTests : HttpMethodTests
    {
        public OptionsTests() : base(HttpMethod.Options) { }
        protected override Task<IFlurlResponse> CallOnString(string url) => url.OptionsAsync();
        protected override Task<IFlurlResponse> CallOnUrl(Flurl url) => url.OptionsAsync();
        protected override Task<IFlurlResponse> CallOnFlurlRequest(IFlurlRequest req) => req.OptionsAsync();
    }
    [TestFixture, Parallelizable]
    public class HttpStatusRangeParserTests
    {
        [TestCase("4**", 399, ExpectedResult = false)]
        [TestCase("4**", 400, ExpectedResult = true)]
        [TestCase("4**", 499, ExpectedResult = true)]
        [TestCase("4**", 500, ExpectedResult = false)]

        [TestCase("4xx", 399, ExpectedResult = false)]
        [TestCase("4xx", 400, ExpectedResult = true)]
        [TestCase("4xx", 499, ExpectedResult = true)]
        [TestCase("4xx", 500, ExpectedResult = false)]

        [TestCase("4XX", 399, ExpectedResult = false)]
        [TestCase("4XX", 400, ExpectedResult = true)]
        [TestCase("4XX", 499, ExpectedResult = true)]
        [TestCase("4XX", 500, ExpectedResult = false)]

        [TestCase("400-499", 399, ExpectedResult = false)]
        [TestCase("400-499", 400, ExpectedResult = true)]
        [TestCase("400-499", 499, ExpectedResult = true)]
        [TestCase("400-499", 500, ExpectedResult = false)]

        [TestCase("100,3xx,600", 100, ExpectedResult = true)]
        [TestCase("100,3xx,600", 101, ExpectedResult = false)]
        [TestCase("100,3xx,600", 300, ExpectedResult = true)]
        [TestCase("100,3xx,600", 399, ExpectedResult = true)]
        [TestCase("100,3xx,600", 400, ExpectedResult = false)]
        [TestCase("100,3xx,600", 600, ExpectedResult = true)]

        [TestCase("400-409,490-499", 399, ExpectedResult = false)]
        [TestCase("400-409,490-499", 405, ExpectedResult = true)]
        [TestCase("400-409,490-499", 450, ExpectedResult = false)]
        [TestCase("400-409,490-499", 495, ExpectedResult = true)]
        [TestCase("400-409,490-499", 500, ExpectedResult = false)]

        [TestCase("*", 0, ExpectedResult = true)]
        [TestCase(",,,*", 9999, ExpectedResult = true)]

        [TestCase("", 0, ExpectedResult = false)]
        [TestCase(",,,", 9999, ExpectedResult = false)]
        public bool parser_works(string pattern, int value)
        {
            return HttpStatusRangeParser.IsMatch(pattern, value);
        }

        [TestCase("-100")]
        [TestCase("100-")]
        [TestCase("1yy")]
        public void parser_throws_on_invalid_pattern(string pattern)
        {
            Assert.Throws<ArgumentException>(() => HttpStatusRangeParser.IsMatch(pattern, 100));
        }
    }
    public abstract class HttpTestFixtureBase
    {
        protected HttpTest HttpTest { get; private set; }

        [SetUp]
        public void CreateHttpTest()
        {
            HttpTest = new HttpTest();
        }

        [TearDown]
        public void DisposeHttpTest()
        {
            HttpTest.Dispose();
        }
    }
    [TestFixture, Parallelizable]
    public class MultipartTests
    {
        [Test]
        public async Task can_build_and_send_multipart_content()
        {
            var content = new CapturedMultipartContent()
                .AddString("string", "foo")
                .AddString("string2", "bar", "text/blah")
                .AddStringParts(new { part1 = 1, part2 = 2, part3 = (string)null }) // part3 should be excluded
                .AddFile("file1", Path.Combine("path", "to", "image1.jpg"), "image/jpeg")
                .AddFile("file2", Path.Combine("path", "to", "image2.jpg"), "image/jpeg", fileName: "new-name.jpg")
                .AddJson("json", new { foo = "bar" })
                .AddUrlEncoded("urlEnc", new { fizz = "buzz" });

            void AssertAll()
            {
                Assert.AreEqual(8, content.Parts.Count);
                AssertStringPart<CapturedStringContent>(content.Parts[0], "string", "foo", null);
                AssertStringPart<CapturedStringContent>(content.Parts[1], "string2", "bar", "text/blah");
                AssertStringPart<CapturedStringContent>(content.Parts[2], "part1", "1", null);
                AssertStringPart<CapturedStringContent>(content.Parts[3], "part2", "2", null);
                AssertFilePart(content.Parts[4], "file1", "image1.jpg", "image/jpeg");
                AssertFilePart(content.Parts[5], "file2", "new-name.jpg", "image/jpeg");
                AssertStringPart<CapturedJsonContent>(content.Parts[6], "json", "{\"foo\":\"bar\"}", "application/json; charset=UTF-8");
                AssertStringPart<CapturedUrlEncodedContent>(content.Parts[7], "urlEnc", "fizz=buzz", "application/x-www-form-urlencoded");
            }

            // Assert before and after sending a request. MultipartContent clears the parts collection after request is sent;
            // CapturedMultipartContent (as the name implies) should preserve it (#580)

            AssertAll();
            using (var test = new HttpTest())
            {
                await "https://upload.com".PostAsync(content);
            }
            AssertAll();
        }

        private void AssertStringPart<TContent>(HttpContent part, string name, string content, string contentType)
        {
            Assert.IsInstanceOf<TContent>(part);
            Assert.AreEqual(name, part.Headers.ContentDisposition.Name);
            Assert.AreEqual(content, (part as CapturedStringContent)?.Content);
            if (contentType == null)
                Assert.IsFalse(part.Headers.Contains("Content-Type")); // #392
            else
                Assert.AreEqual(contentType, part.Headers.GetValues("Content-Type").SingleOrDefault());
        }

        private void AssertFilePart(HttpContent part, string name, string fileName, string contentType)
        {
            Assert.IsInstanceOf<FileContent>(part);
            Assert.AreEqual(name, part.Headers.ContentDisposition.Name);
            Assert.AreEqual(fileName, part.Headers.ContentDisposition.FileName);
            Assert.AreEqual(contentType, part.Headers.ContentType?.MediaType);
        }

        [Test]
        public void must_provide_required_args_to_builder()
        {
            var content = new CapturedMultipartContent();
            Assert.Throws<ArgumentNullException>(() => content.AddStringParts(null));
            Assert.Throws<ArgumentNullException>(() => content.AddString("other", null));
            Assert.Throws<ArgumentException>(() => content.AddString(null, "hello!"));
            Assert.Throws<ArgumentException>(() => content.AddFile("  ", "path"));
        }
    }
    /// <summary>
    /// ISerializer implementation based on Newtonsoft.Json.
    /// Default serializer used in calls to GetJsonAsync, PostJsonAsync, etc.
    /// </summary>
    public class NewtonsoftJsonSerializer : ISerializer
    {
        private readonly JsonSerializerSettings _settings;

        /// <summary>
        /// Initializes a new instance of the <see cref="NewtonsoftJsonSerializer"/> class.
        /// </summary>
        /// <param name="settings">Settings to control (de)serialization behavior.</param>
        public NewtonsoftJsonSerializer(JsonSerializerSettings settings = null)
        {
            _settings = settings;
        }

        /// <summary>
        /// Serializes the specified object to a JSON string.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        public string Serialize(object obj) => JsonConvert.SerializeObject(obj, _settings);

        /// <summary>
        /// Deserializes the specified JSON string to an object of type T.
        /// </summary>
        /// <param name="s">The JSON string to deserialize.</param>
        public T Deserialize<T>(string s) => JsonConvert.DeserializeObject<T>(s, _settings);

        /// <summary>
        /// Deserializes the specified stream to an object of type T.
        /// </summary>
        /// <param name="stream">The stream to deserialize.</param>
        public T Deserialize<T>(Stream stream)
        {
            // https://www.newtonsoft.com/json/help/html/Performance.htm#MemoryUsage
            using var sr = new StreamReader(stream);
            using var jr = new JsonTextReader(sr);
            return JsonSerializer.CreateDefault(_settings).Deserialize<T>(jr);
        }
    }
    [TestFixture, Parallelizable]
    public class PostTests : HttpMethodTests
    {
        public PostTests() : base(HttpMethod.Post) { }

        protected override Task<IFlurlResponse> CallOnString(string url) => url.PostAsync(null);
        protected override Task<IFlurlResponse> CallOnUrl(Flurl url) => url.PostAsync(null);
        protected override Task<IFlurlResponse> CallOnFlurlRequest(IFlurlRequest req) => req.PostAsync(null);

        [Test]
        public async Task can_post_string()
        {
            var expectedEndpoint = "http://some-api.com";
            var expectedBody = "abc123";
            await expectedEndpoint.PostStringAsync(expectedBody);
            HttpTest.ShouldHaveCalled(expectedEndpoint)
                .WithVerb(HttpMethod.Post)
                .WithRequestBody(expectedBody)
                .Times(1);
        }

        [Test]
        public async Task can_post_object_as_json()
        {
            var body = new { a = 1, b = 2 };
            await "http://some-api.com".PostJsonAsync(body);
            HttpTest.ShouldHaveCalled("http://some-api.com")
                .WithVerb(HttpMethod.Post)
                .WithContentType("application/json")
                .WithRequestBody("{\"a\":1,\"b\":2}")
                .Times(1);
        }

        [Test]
        public async Task can_post_url_encoded()
        {
            var body = new { a = 1, b = 2, c = "hi there", d = new[] { 1, 2, 3 } };
            await "http://some-api.com".PostUrlEncodedAsync(body);
            HttpTest.ShouldHaveCalled("http://some-api.com")
                .WithVerb(HttpMethod.Post)
                .WithContentType("application/x-www-form-urlencoded")
                .WithRequestBody("a=1&b=2&c=hi+there&d=1&d=2&d=3")
                .Times(1);
        }

        [Test]
        public async Task can_post_nothing()
        {
            await "http://some-api.com".PostAsync();
            HttpTest.ShouldHaveCalled("http://some-api.com")
                .WithVerb(HttpMethod.Post)
                .WithRequestBody("")
                .Times(1);
        }

        [Test]
        public async Task can_receive_json()
        {
            HttpTest.RespondWithJson(new TestData { id = 1, name = "Frank" });

            var data = await "http://some-api.com".PostJsonAsync(new { a = 1, b = 2 }).ReceiveJson<TestData>();

            Assert.AreEqual(1, data.id);
            Assert.AreEqual("Frank", data.name);
        }

        [Test]
        public async Task can_receive_string()
        {
            HttpTest.RespondWith("good job");

            var data = await "http://some-api.com".PostJsonAsync(new { a = 1, b = 2 }).ReceiveString();

            Assert.AreEqual("good job", data);
        }

        private class TestData
        {
            public int id { get; set; }
            public string name { get; set; }
        }
    }
    /// <summary>
    /// Most HTTP tests in this project are with Flurl in fake mode. These are some real ones, mostly using http://httpbin.org.
    /// </summary>
    [TestFixture, Parallelizable]
    public class RealHttpTests
    {
        [TestCase("gzip", "gzipped")]
        [TestCase("deflate", "deflated"), Ignore("#474")]
        public async Task decompresses_automatically(string encoding, string jsonKey)
        {
            var result = await "https://httpbin.org"
                .AppendPathSegment(encoding)
                .WithHeader("Accept-encoding", encoding)
                .GetJsonAsync<Dictionary<string, object>>();

            Assert.AreEqual(true, result[jsonKey]);
        }

        [TestCase("https://httpbin.org/image/jpeg", null, "my-image.jpg", "my-image.jpg")]
        // should use last path segment url-decoded (foo/bar), then replace illegal filename characters with _ ('/' and '\0' are only illegal chars in *nix)
        [TestCase("https://httpbin.org/anything/foo%2Fbar", null, null, "foo_bar")]
        // should use filename from content-disposition excluding any leading/trailing quotes
        [TestCase("https://httpbin.org/response-headers", "attachment; filename=\"myfile.txt\"", null, "myfile.txt")]
        // should prefer filename* over filename, per https://tools.ietf.org/html/rfc6266#section-4.3
        [TestCase("https://httpbin.org/response-headers", "attachment; filename=filename.txt; filename*=utf-8''filenamestar.txt", null, "filenamestar.txt")]
        // has Content-Disposition header but no filename in it, should use last part of URL
        [TestCase("https://httpbin.org/response-headers", "attachment", null, "response-headers")]
        public async Task can_download_file(string url, string contentDisposition, string suppliedFilename, string expectedFilename)
        {
            var folder = Path.Combine(Path.GetTempPath(), $"flurl-test-{Guid.NewGuid()}"); // random so parallel tests don't trip over each other

            try
            {
                var path = await url.SetQueryParam("Content-Disposition", contentDisposition).DownloadFileAsync(folder, suppliedFilename);
                var expected = Path.Combine(folder, expectedFilename);
                Assert.AreEqual(expected, path);
                Assert.That(File.Exists(expected));
            }
            finally
            {
                Directory.Delete(folder, true);
            }
        }

        [Test]
        public async Task can_post_and_receive_json()
        {
            var result = await "https://httpbin.org/post".PostJsonAsync(new { a = 1, b = 2 }).ReceiveJson<HttpBinResponse>();
            Assert.AreEqual(1, result.json["a"].ToString().ToCInt32());
            Assert.AreEqual(2, result.json["b"].ToString().ToCInt32());
        }

        [Test]
        public async Task can_get_stream()
        {
            using (var stream = await "https://www.google.com".GetStreamAsync())
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                Assert.Greater(ms.Length, 0);
            }
        }

        [Test]
        public async Task can_get_string()
        {
            var s = await "https://www.google.com".GetStringAsync();
            Assert.Greater(s.Length, 0);
        }

        [Test]
        public async Task can_get_byte_array()
        {
            var bytes = await "https://www.google.com".GetBytesAsync();
            Assert.Greater(bytes.Length, 0);
        }

        [Test]
        public void fails_on_non_success_status()
        {
            Assert.ThrowsAsync<FlurlHttpException>(async () => await "https://httpbin.org/status/418".GetAsync());
        }

        [Test]
        public async Task can_allow_non_success_status()
        {
            var resp = await "https://httpbin.org/status/418".AllowHttpStatus("4xx").GetAsync();
            Assert.AreEqual(418, resp.StatusCode);
        }

        [Test]
        public async Task can_post_multipart()
        {
            var folder = "c:\\flurl-test-" + Guid.NewGuid(); // random so parallel tests don't trip over each other
            var path1 = Path.Combine(folder, "upload1.txt");
            var path2 = Path.Combine(folder, "upload2.txt");

            Directory.CreateDirectory(folder);
            try
            {
                File.WriteAllText(path1, "file contents 1");
                File.WriteAllText(path2, "file contents 2");

                using (var stream = File.OpenRead(path2))
                {
                    var resp = await "https://httpbin.org/post"
                        .PostMultipartAsync(content => {
                            content
                                .AddStringParts(new { a = 1, b = 2 })
                                .AddString("DataField", "hello!")
                                .AddFile("File1", path1)
                                .AddFile("File2", stream, "foo.txt");

                            // hack to deal with #179. appears to be fixed on httpbin now.
                            // content.Headers.ContentLength = 735;
                        })
                        //.ReceiveString();
                        .ReceiveJson<HttpBinResponse>();
                    Assert.AreEqual("1", resp.form["a"]);
                    Assert.AreEqual("2", resp.form["b"]);
                    Assert.AreEqual("hello!", resp.form["DataField"]);
                    Assert.AreEqual("file contents 1", resp.files["File1"]);
                    Assert.AreEqual("file contents 2", resp.files["File2"]);
                }
            }
            finally
            {
                Directory.Delete(folder, true);
            }
        }

        [Test]
        public async Task can_handle_http_error()
        {
            var handlerCalled = false;

            try
            {
                await "https://httpbin.org/status/500".ConfigureRequest(c => {
                    c.OnError = call => {
                        call.ExceptionHandled = true;
                        handlerCalled = true;
                    };
                }).GetAsync();
                Assert.IsTrue(handlerCalled, "error handler should have been called.");
            }
            catch (FlurlHttpException)
            {
                Assert.Fail("exception should have been suppressed.");
            }
        }

        [Test]
        public async Task can_handle_parsing_error()
        {
            Exception ex = null;

            try
            {
                await "http://httpbin.org/image/jpeg".ConfigureRequest(c => {
                    c.OnError = call => {
                        ex = call.Exception;
                        call.ExceptionHandled = true;
                    };
                }).GetJsonAsync<object>();
                Assert.IsNotNull(ex, "error handler should have been called.");
                Assert.IsInstanceOf<FlurlParsingException>(ex);
            }
            catch (FlurlHttpException)
            {
                Assert.Fail("exception should have been suppressed.");
            }
        }

        [Test]
        public async Task can_comingle_real_and_fake_tests()
        {
            // do a fake call while a real call is running
            var realTask = "https://www.google.com/".GetStringAsync();
            using (var test = new HttpTest())
            {
                test.RespondWith("fake!");
                var fake = await "https://www.google.com/".GetStringAsync();
                Assert.AreEqual("fake!", fake);
            }
            Assert.AreNotEqual("fake!", await realTask);
        }

        [Test]
        public void can_set_timeout()
        {
            var ex = Assert.ThrowsAsync<FlurlHttpTimeoutException>(async () => {
                await "https://httpbin.org/delay/5"
                    .WithTimeout(TimeSpan.FromMilliseconds(50))
                    .HeadAsync();
            });
            Assert.That(ex.InnerException is TaskCanceledException);
        }

        [Test]
        public void can_cancel_request()
        {
            var cts = new CancellationTokenSource();
            var ex = Assert.ThrowsAsync<FlurlHttpException>(async () => {
                var task = "https://httpbin.org/delay/5".GetAsync(cancellationToken: cts.Token);
                cts.Cancel();
                await task;
            });
            Assert.That(ex.InnerException is TaskCanceledException);
        }

        [Test] // make sure the 2 tokens in play are playing nicely together
        public void can_set_timeout_and_cancellation_token()
        {
            // cancellation with timeout value set
            var cts = new CancellationTokenSource();
            var ex = Assert.ThrowsAsync<FlurlHttpException>(async () => {
                var task = "https://httpbin.org/delay/5"
                    .WithTimeout(TimeSpan.FromMilliseconds(50))
                    .GetAsync(cancellationToken: cts.Token);
                cts.Cancel();
                await task;
            });
            Assert.That(ex.InnerException is OperationCanceledException);
            Assert.IsTrue(cts.Token.IsCancellationRequested);

            // timeout with cancellation token set
            cts = new CancellationTokenSource();
            ex = Assert.ThrowsAsync<FlurlHttpTimeoutException>(async () => {
                await "https://httpbin.org/delay/5"
                    .WithTimeout(TimeSpan.FromMilliseconds(50))
                    .GetAsync(cancellationToken: cts.Token);
            });
            Assert.That(ex.InnerException is OperationCanceledException);
            Assert.IsFalse(cts.Token.IsCancellationRequested);
        }

        [Test]
        public async Task test_settings_override_client_settings()
        {
            // control case
            using (var test1 = new HttpTest())
            {
                test1.AllowRealHttp();

                var s = await "http://httpbingo.org/redirect-to?url=http%3A%2F%2Fexample.com"
                    .WithHeader("x", "1")
                    .GetStringAsync();

                test1.ShouldHaveMadeACall().Times(2);
                test1.ShouldHaveCalled("http://example.com*");
            }

            // this time disable redirects at the test level
            using (var test2 = new HttpTest())
            {
                test2.AllowRealHttp();
                test2.Settings.Redirects.Enabled = false;

                var s = await "http://httpbingo.org/redirect-to?url=http%3A%2F%2Fexample.com"
                    .WithAutoRedirect(true) // test says redirects are off, and test should always win
                    .GetStringAsync();

                test2.ShouldHaveMadeACall().Times(1);
                test2.ShouldNotHaveCalled("http://example.com*");
            }
        }

        [Test]
        public async Task can_allow_real_http_in_test()
        {
            using var test = new HttpTest();
            test.RespondWith("foo");
            test.ForCallsTo("*httpbin*").AllowRealHttp();

            Assert.AreEqual("foo", await "https://www.google.com".GetStringAsync());
            Assert.AreNotEqual("foo", await "https://httpbin.org/get".GetStringAsync());
            Assert.AreEqual("bar", (await "https://httpbin.org/get?x=bar".GetJsonAsync<HttpBinResponse>()).args["x"]);
            Assert.AreEqual("foo", await "https://www.microsoft.com".GetStringAsync());

            // real calls still get logged
            Assert.AreEqual(4, test.CallLog.Count);
            test.ShouldHaveCalled("https://httpbin*").Times(2);
        }

        [Test] // #683
        public async Task configured_client_used_when_real_http_allowed()
        {
            var rh = new MyCustomMessageHandler();
            var hc = new HttpClient(rh);
            var fc = new FlurlClient(hc);

            using var test = new HttpTest();
            test.RespondWith("fake");
            test.ForCallsTo("*httpbin*").AllowRealHttp();

            var resp = await fc.Request("https://httpbin.org/get").GetStringAsync();
            Assert.AreNotEqual("fake", resp);

            // the call got logged
            test.ShouldHaveCalled("https://httpbin*");

            // but the inner handler got hit
            Assert.AreEqual(1, rh.Hits);
        }

        [Test]
        public async Task does_not_create_empty_content_for_forwarding_content_header()
        {
            // Flurl was auto-creating an empty HttpContent object in order to forward content-level headers,
            // and on .NET Framework a GET with a non-null HttpContent throws an exceptions (#583)
            var calls = new List<FlurlCall>();
            var resp = await "http://httpbingo.org/redirect-to?url=http%3A%2F%2Fexample.com%2F".ConfigureRequest(c => {
                c.Redirects.ForwardHeaders = true;
                c.BeforeCall = call => calls.Add(call);
            }).PostUrlEncodedAsync("test=test");

            Assert.AreEqual(2, calls.Count);
            Assert.AreEqual(HttpMethod.Post, calls[0].Request.Verb);
            Assert.IsNotNull(calls[0].HttpRequestMessage.Content);
            Assert.AreEqual(HttpMethod.Get, calls[1].Request.Verb);
            Assert.IsNull(calls[1].HttpRequestMessage.Content);
        }

        #region cookies
        [Test]
        public async Task can_send_cookies()
        {
            var req = "https://httpbin.org/cookies".WithCookies(new { x = 1, y = 2 });
            Assert.AreEqual(2, req.Cookies.Count());
            Assert.IsTrue(req.Cookies.Contains(("x", "1")));
            Assert.IsTrue(req.Cookies.Contains(("y", "2")));

            var s = await req.GetStringAsync();

            var resp = await req.WithAutoRedirect(false).GetJsonAsync<HttpBinResponse>();
            // httpbin returns json representation of cookies that were sent
            Assert.AreEqual("1", resp.cookies["x"]);
            Assert.AreEqual("2", resp.cookies["y"]);
        }

        [Test]
        public async Task can_receive_cookies()
        {
            // endpoint does a redirect, so we need to disable auto-redirect in order to see the cookie in the response
            var resp = await "https://httpbin.org/cookies/set?z=999".WithAutoRedirect(false).GetAsync();
            Assert.AreEqual("999", resp.Cookies.FirstOrDefault(c => c.Name == "z")?.Value);


            // but using WithCookies we can capture it even with redirects enabled
            await "https://httpbin.org/cookies/set?z=999".WithCookies(out var cookies).GetAsync();
            Assert.AreEqual("999", cookies.FirstOrDefault(c => c.Name == "z")?.Value);

            // this works with redirects too
            using (var session = new CookieSession("https://httpbin.org/cookies"))
            {
                await session.Request("set?z=999").GetAsync();
                Assert.AreEqual("999", session.Cookies.FirstOrDefault(c => c.Name == "z")?.Value);
            }
        }

        [Test]
        public async Task can_set_cookies_before_setting_url()
        {
            var req = new FlurlRequest().WithCookie("z", "999");
            req.Url = "https://httpbin.org/cookies";
            var resp = await req.GetJsonAsync<HttpBinResponse>();
            Assert.AreEqual("999", resp.cookies["z"]);
        }

        [Test]
        public async Task can_send_different_cookies_per_request()
        {
            var cli = new FlurlClient();

            var req1 = cli.Request("https://httpbin.org/cookies").WithCookie("x", "123");
            var req2 = cli.Request("https://httpbin.org/cookies").WithCookie("x", "abc");

            var resp2 = await req2.GetJsonAsync<HttpBinResponse>();
            var resp1 = await req1.GetJsonAsync<HttpBinResponse>();

            Assert.AreEqual("123", resp1.cookies["x"]);
            Assert.AreEqual("abc", resp2.cookies["x"]);
        }

        [Test]
        public async Task can_receive_cookie_from_redirect_response_and_add_it_to_jar()
        {
            // use httpbingo instead of httpbin because of redirect issue https://github.com/postmanlabs/httpbin/issues/617
            var resp = await "https://httpbingo.org/redirect-to"
                .SetQueryParam("url", "/cookies/set?x=foo")
                .WithCookies(out var jar)
                .GetJsonAsync<Dictionary<string, string>>();

            Assert.AreEqual("foo", resp["x"]);
            Assert.AreEqual(1, jar.Count);
        }
        #endregion

        class HttpBinResponse
        {
            public Dictionary<string, JToken> json { get; set; }
            public Dictionary<string, string> args { get; set; }
            public Dictionary<string, string> form { get; set; }
            public Dictionary<string, string> cookies { get; set; }
            public Dictionary<string, string> files { get; set; }
        }

        class MyCustomMessageHandler : DelegatingHandler
        {
            public MyCustomMessageHandler() : base(new HttpClientHandler()) { }

            public int Hits { get; private set; }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Hits++;
                return base.SendAsync(request, cancellationToken);
            }
        }
    }
    [TestFixture, Parallelizable]
    public class RedirectTests : HttpTestFixtureBase
    {
        [Test]
        public async Task can_auto_redirect()
        {
            HttpTest
                .RespondWith("", 302, new { Location = "http://redir.com/foo" })
                .RespondWith("", 302, new { Location = "/redir2" })
                .RespondWith("", 302, new { Location = "redir3?x=1&y=2#foo" })
                .RespondWith("", 302, new { Location = "//otherredir.com/bar/?a=b" })
                .RespondWith("done!");

            var resp = await "http://start.com".PostStringAsync("foo!").ReceiveString();

            Assert.AreEqual("done!", resp);
            HttpTest.ShouldHaveMadeACall().Times(5);
            HttpTest.ShouldHaveCalled("http://start.com").WithVerb(HttpMethod.Post).WithRequestBody("foo!")
                .With(call => call.Request.RedirectedFrom == null);
            HttpTest.ShouldHaveCalled("http://redir.com/foo").WithVerb(HttpMethod.Get).WithRequestBody("")
                .With(call => call.Request.RedirectedFrom.Request.Url.ToString() == "http://start.com");
            HttpTest.ShouldHaveCalled("http://redir.com/redir2").WithVerb(HttpMethod.Get).WithRequestBody("")
                .With(call => call.Request.RedirectedFrom.Request.Url.ToString() == "http://redir.com/foo");
            HttpTest.ShouldHaveCalled("http://redir.com/redir2/redir3?x=1&y=2#foo").WithVerb(HttpMethod.Get).WithRequestBody("")
                .With(call => call.Request.RedirectedFrom.Request.Url.ToString() == "http://redir.com/redir2");
            HttpTest.ShouldHaveCalled("http://otherredir.com/bar/?a=b#foo").WithVerb(HttpMethod.Get).WithRequestBody("")
                .With(call => call.Request.RedirectedFrom.Request.Url.ToString() == "http://redir.com/redir2/redir3?x=1&y=2#foo");
        }

        [Test]
        public async Task redirect_location_inherits_fragment_when_none()
        {
            HttpTest
                .RespondWith("", 302, new { Location = "/redir1" })
                .RespondWith("", 302, new { Location = "/redir2#bar" })
                .RespondWith("", 302, new { Location = "/redir3" })
                .RespondWith("done!");
            await "http://start.com?x=y#foo".GetAsync();

            HttpTest.ShouldHaveCalled("http://start.com?x=y#foo");
            // also asserts that they do NOT inherit query params in the same way
            HttpTest.ShouldHaveCalled("http://start.com/redir1#foo");
            HttpTest.ShouldHaveCalled("http://start.com/redir2#bar");
            HttpTest.ShouldHaveCalled("http://start.com/redir3#bar");
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task can_enable_auto_redirect_per_request(bool enabled)
        {
            HttpTest
                .RespondWith("original", 302, new { Location = "http://redir.com/foo" })
                .RespondWith("redirected");

            // whatever we want at the request level, set it the opposite at the client level
            var fc = new FlurlClient().WithAutoRedirect(!enabled);

            var result = await fc.Request("http://start.com").WithAutoRedirect(enabled).GetStringAsync();

            Assert.AreEqual(enabled ? "redirected" : "original", result);
            HttpTest.ShouldHaveMadeACall().Times(enabled ? 2 : 1);
        }

        [Test, Combinatorial]
        public async Task can_configure_header_forwarding([Values(false, true)] bool fwdAuth, [Values(false, true)] bool fwdOther)
        {
            HttpTest
                .RespondWith("", 302, new { Location = "/next" })
                .RespondWith("done!");

            await "http://start.com"
                .WithHeaders(new
                {
                    Authorization = "xyz",
                    Cookie = "x=foo;y=bar",
                    Transfer_Encoding = "chunked",
                    Custom1 = "foo",
                    Custom2 = "bar"
                })
                .ConfigureRequest(settings => {
                    settings.Redirects.ForwardAuthorizationHeader = fwdAuth;
                    settings.Redirects.ForwardHeaders = fwdOther;
                })
                .PostAsync(null);

            HttpTest.ShouldHaveCalled("http://start.com")
                .WithHeader("Authorization")
                .WithHeader("Cookie")
                .WithHeader("Transfer-Encoding")
                .WithHeader("Custom1")
                .WithHeader("Custom2");

            HttpTest.ShouldHaveCalled("http://start.com/next")
                .With(call =>
                    call.Request.Headers.Contains("Authorization") == fwdAuth &&
                    call.Request.Headers.Contains("Custom1") == fwdOther &&
                    call.Request.Headers.Contains("Custom2") == fwdOther)
                .WithoutHeader("Cookie") // special rule: never forward this when CookieJar isn't being used
                .WithoutHeader("Transfer-Encoding"); // special rule: never forward this if verb is changed to GET, which is is on a 302 POST
        }

        [TestCase(301, true)]
        [TestCase(302, true)]
        [TestCase(303, true)]
        [TestCase(307, false)]
        [TestCase(308, false)]
        public async Task redirect_preserves_verb_sometimes(int status, bool changeToGet)
        {
            HttpTest
                .RespondWith("", status, new { Location = "/next" })
                .RespondWith("done!");

            await "http://start.com".PostStringAsync("foo!");

            HttpTest.ShouldHaveCalled("http://start.com/next")
                .WithVerb(changeToGet ? HttpMethod.Get : HttpMethod.Post)
                .WithRequestBody(changeToGet ? "" : "foo!");
        }

        [TestCase(null)] // test the default (10)
        [TestCase(5)]
        public async Task can_limit_redirects(int? max)
        {
            for (var i = 1; i <= 20; i++)
                HttpTest.RespondWith("", 301, new { Location = "/redir" + i });

            var fc = new FlurlClient();
            if (max.HasValue)
                fc.Settings.Redirects.MaxAutoRedirects = max.Value;

            await fc.Request("http://start.com").GetAsync();

            var count = max ?? 10;
            HttpTest.ShouldHaveCalled("http://start.com/redir*").Times(count);
            HttpTest.ShouldHaveCalled("http://start.com/redir" + count);
            HttpTest.ShouldNotHaveCalled("http://start.com/redir" + (count + 1));
        }

        [Test]
        public async Task can_limit_circular_redirects()
        {
            // similar to above test but for the (maybe more common?) case of circular redirects
            HttpTest.ForCallsTo("*/1").RespondWith("", 301, new { Location = "/2" });
            HttpTest.ForCallsTo("*/2").RespondWith("", 301, new { Location = "/1" });

            var resp = await "http://start.com/1".GetAsync();
            // default max auto-redirects is 10, so should have redirected to 5 each, plus the original hit to /1
            HttpTest.ShouldHaveCalled("http://start.com/1").Times(6);
            HttpTest.ShouldHaveCalled("http://start.com/2").Times(5);
        }

        [Test]
        public async Task can_change_redirect_behavior_from_event()
        {
            var eventFired = false;

            HttpTest
                .RespondWith("", 301, new { Location = "/next" })
                .RespondWith("done!");

            var fc = new FlurlClient()
                .OnRedirect(call => {
                    eventFired = true;

                    // assert all the properties of call.Redirect
                    Assert.IsTrue(call.Redirect.Follow);
                    Assert.AreEqual("http://start.com/next", call.Redirect.Url.ToString());
                    Assert.AreEqual(1, call.Redirect.Count);
                    Assert.IsTrue(call.Redirect.ChangeVerbToGet);

                    // now change the behavior
                    call.Redirect.Url.SetQueryParam("x", 999);
                    call.Redirect.ChangeVerbToGet = false;
                });

            await fc.Request("http://start.com").PostStringAsync("foo!");

            Assert.IsTrue(eventFired);

            HttpTest.ShouldHaveCalled("http://start.com/next?x=999")
                .WithVerb(HttpMethod.Post)
                .WithRequestBody("foo!");
        }

        [Test]
        public async Task can_block_redirect_from_event()
        {
            HttpTest
                .RespondWith("", 301, new { Location = "/next" })
                .RespondWith("done!");

            var fc = new FlurlClient();
            await fc.Request("http://start.com")
                .OnRedirect(call => call.Redirect.Follow = false)
                .GetAsync();

            HttpTest.ShouldNotHaveCalled("http://start.com/next");
        }

        [Test]
        public async Task can_disable_redirect()
        {
            HttpTest
                .RespondWith("", 301, new { Location = "/next" })
                .RespondWith("done!");

            var fc = new FlurlClient();
            fc.Settings.Redirects.Enabled = false;
            await fc.Request("http://start.com").GetAsync();

            HttpTest.ShouldNotHaveCalled("http://start.com/next");
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task can_allow_redirect_secure_to_insecure(bool allow)
        {
            HttpTest
                .RespondWith("", 301, new { Location = "http://insecure.com/next" })
                .RespondWith("done!");

            var fc = new FlurlClient();
            if (allow) // test that false is default (don't set explicitly)
                fc.Settings.Redirects.AllowSecureToInsecure = true;

            await fc.Request("https://secure.com").GetAsync();

            if (allow)
                HttpTest.ShouldHaveCalled("http://insecure.com/next");
            else
                HttpTest.ShouldNotHaveCalled("http://insecure.com/next");
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task can_allow_forward_auth_header(bool allow)
        {
            HttpTest
                .RespondWith("", 301, new { Location = "/next" })
                .RespondWith("done!");

            var fc = new FlurlClient();
            if (allow) // test that false is default (don't set explicitly)
                fc.Settings.Redirects.ForwardAuthorizationHeader = true;

            await fc.Request("http://start.com")
                .WithHeader("Authorization", "foo")
                .GetAsync();

            if (allow)
                HttpTest.ShouldHaveCalled("http://start.com/next").WithHeader("Authorization", "foo");
            else
                HttpTest.ShouldHaveCalled("http://start.com/next").WithoutHeader("Authorization");
        }
    }
    // IFlurlClient and IFlurlRequest both implement IHttpSettingsContainer, which defines a number
    // of settings-related extension methods. This abstract test class allows those methods to be
    // tested against both both client-level and request-level implementations.
    public abstract class SettingsExtensionsTests<T> where T : IHttpSettingsContainer
    {
        protected abstract T GetSettingsContainer();
        protected abstract IFlurlRequest GetRequest(T sc);

        [Test]
        public void can_set_timeout()
        {
            var sc = GetSettingsContainer().WithTimeout(TimeSpan.FromSeconds(15));
            Assert.AreEqual(TimeSpan.FromSeconds(15), sc.Settings.Timeout);
        }

        [Test]
        public void can_set_timeout_in_seconds()
        {
            var sc = GetSettingsContainer().WithTimeout(15);
            Assert.AreEqual(sc.Settings.Timeout, TimeSpan.FromSeconds(15));
        }

        [Test]
        public void can_set_header()
        {
            var sc = GetSettingsContainer().WithHeader("a", 1);
            Assert.AreEqual(("a", "1"), sc.Headers.Single());
        }

        [Test]
        public void can_set_headers_from_anon_object()
        {
            // null values shouldn't be added
            var sc = GetSettingsContainer().WithHeaders(new { a = "b", one = 2, three = (object)null });
            Assert.AreEqual(2, sc.Headers.Count);
            Assert.IsTrue(sc.Headers.Contains("a", "b"));
            Assert.IsTrue(sc.Headers.Contains("one", "2"));
        }

        [Test]
        public void can_remove_header_by_setting_null()
        {
            var sc = GetSettingsContainer().WithHeaders(new { a = 1, b = 2 });
            Assert.AreEqual(2, sc.Headers.Count);
            sc.WithHeader("b", null);
            Assert.AreEqual(1, sc.Headers.Count);
            Assert.IsFalse(sc.Headers.Contains("b"));
        }

        [Test]
        public void can_set_headers_from_dictionary()
        {
            var sc = GetSettingsContainer().WithHeaders(new Dictionary<string, object> { { "a", "b" }, { "one", 2 } });
            Assert.AreEqual(2, sc.Headers.Count);
            Assert.IsTrue(sc.Headers.Contains("a", "b"));
            Assert.IsTrue(sc.Headers.Contains("one", "2"));
        }

        [Test]
        public void underscores_in_properties_convert_to_hyphens_in_header_names()
        {
            var sc = GetSettingsContainer().WithHeaders(new { User_Agent = "Flurl", Cache_Control = "no-cache" });
            Assert.IsTrue(sc.Headers.Contains("User-Agent"));
            Assert.IsTrue(sc.Headers.Contains("Cache-Control"));

            // make sure we can disable the behavior
            sc.WithHeaders(new { no_i_really_want_underscores = "foo" }, false);
            Assert.IsTrue(sc.Headers.Contains("no_i_really_want_underscores"));

            // dictionaries don't get this behavior since you can use hyphens explicitly
            sc.WithHeaders(new Dictionary<string, string> { { "exclude_dictionaries", "bar" } });
            Assert.IsTrue(sc.Headers.Contains("exclude_dictionaries"));

            // same with strings
            sc.WithHeaders("exclude_strings=123");
            Assert.IsTrue(sc.Headers.Contains("exclude_strings"));
        }

        [Test]
        public void header_names_are_case_insensitive()
        {
            var sc = GetSettingsContainer().WithHeader("a", 1).WithHeader("A", 2);
            Assert.AreEqual(1, sc.Headers.Count);
            Assert.AreEqual("A", sc.Headers.Single().Name);
            Assert.AreEqual("2", sc.Headers.Single().Value);
        }

        [Test] // #623
        public async Task header_values_are_trimmed()
        {
            var sc = GetSettingsContainer().WithHeader("a", "   1 \t\r\n");
            sc.Headers.Add("b", "   2   ");

            Assert.AreEqual(2, sc.Headers.Count);
            Assert.AreEqual("1", sc.Headers[0].Value);
            // Not trimmed when added directly to Headers collection (implementation seemed like overkill),
            // but below we'll make sure it happens on HttpRequestMessage when request is sent.
            Assert.AreEqual("   2   ", sc.Headers[1].Value);

            using (var test = new HttpTest())
            {
                await GetRequest(sc).GetAsync();
                var sentHeaders = test.CallLog[0].HttpRequestMessage.Headers;
                Assert.AreEqual("1", sentHeaders.GetValues("a").Single());
                Assert.AreEqual("2", sentHeaders.GetValues("b").Single());
            }
        }

        [Test]
        public void can_setup_oauth_bearer_token()
        {
            var sc = GetSettingsContainer().WithOAuthBearerToken("mytoken");
            Assert.AreEqual(1, sc.Headers.Count);
            Assert.IsTrue(sc.Headers.Contains("Authorization", "Bearer mytoken"));
        }

        [Test]
        public void can_setup_basic_auth()
        {
            var sc = GetSettingsContainer().WithBasicAuth("user", "pass");
            Assert.AreEqual(1, sc.Headers.Count);
            Assert.IsTrue(sc.Headers.Contains("Authorization", "Basic dXNlcjpwYXNz"));
        }

        [Test]
        public async Task can_allow_specific_http_status()
        {
            using var test = new HttpTest();
            test.RespondWith("Nothing to see here", 404);
            var sc = GetSettingsContainer().AllowHttpStatus(HttpStatusCode.Conflict, HttpStatusCode.NotFound);
            await GetRequest(sc).DeleteAsync(); // no exception = pass
        }

        [Test]
        public async Task allow_specific_http_status_also_allows_2xx()
        {
            using var test = new HttpTest();
            test.RespondWith("I'm just an innocent 2xx, I should never fail!", 201);
            var sc = GetSettingsContainer().AllowHttpStatus(HttpStatusCode.Conflict, HttpStatusCode.NotFound);
            await GetRequest(sc).GetAsync(); // no exception = pass
        }

        [Test]
        public void can_clear_non_success_status()
        {
            using var test = new HttpTest();
            test.RespondWith("I'm a teapot", 418);
            // allow 4xx
            var sc = GetSettingsContainer().AllowHttpStatus("4xx");
            // but then disallow it
            sc.Settings.AllowedHttpStatusRange = null;
            Assert.ThrowsAsync<FlurlHttpException>(async () => await GetRequest(sc).GetAsync());
        }

        [Test]
        public async Task can_allow_any_http_status()
        {
            using var test = new HttpTest();
            test.RespondWith("epic fail", 500);
            try
            {
                var sc = GetSettingsContainer().AllowAnyHttpStatus();
                var result = await GetRequest(sc).GetAsync();
                Assert.AreEqual(500, result.StatusCode);
            }
            catch (Exception)
            {
                Assert.Fail("Exception should not have been thrown.");
            }
        }
    }

    [TestFixture, Parallelizable]
    public class ClientSettingsExtensionsTests : SettingsExtensionsTests<IFlurlClient>
    {
        protected override IFlurlClient GetSettingsContainer() => new FlurlClient();
        protected override IFlurlRequest GetRequest(IFlurlClient client) => client.Request("http://api.com");

        [Test]
        public void WithUrl_shares_client_but_not_Url()
        {
            var cli = new FlurlClient().WithHeader("myheader", "123");
            var req1 = cli.Request("http://www.api.com/for-req1");
            var req2 = cli.Request("http://www.api.com/for-req2");
            var req3 = cli.Request("http://www.api.com/for-req3");

            CollectionAssert.AreEquivalent(req1.Headers, req2.Headers);
            CollectionAssert.AreEquivalent(req1.Headers, req3.Headers);
            var urls = new[] { req1, req2, req3 }.Select(c => c.Url.ToString());
            CollectionAssert.AllItemsAreUnique(urls);
        }

        [Test]
        public void can_use_uri_with_WithUrl()
        {
            var uri = new System.Uri("http://www.mysite.com/foo?x=1");
            var req = new FlurlClient().Request(uri);
            Assert.AreEqual(uri.ToString(), req.Url.ToString());
        }

        [Test]
        public void can_override_settings_fluently()
        {
            using (var test = new HttpTest())
            {
                var cli = new FlurlClient().Configure(s => s.AllowedHttpStatusRange = "*");
                test.RespondWith("epic fail", 500);
                var req = "http://www.api.com".ConfigureRequest(c => c.AllowedHttpStatusRange = "2xx");
                req.Client = cli; // client-level settings shouldn't win
                Assert.ThrowsAsync<FlurlHttpException>(async () => await req.GetAsync());
            }
        }
    }

    [TestFixture, Parallelizable]
    public class RequestSettingsExtensionsTests : SettingsExtensionsTests<IFlurlRequest>
    {
        protected override IFlurlRequest GetSettingsContainer() => new FlurlRequest("http://api.com");
        protected override IFlurlRequest GetRequest(IFlurlRequest req) => req;
    }
    /// <summary>
    /// FlurlHttpSettings are available at the global, test, client, and request level. This abstract class
    /// allows the same tests to be run against settings at all 4 levels.
    /// </summary>
    public abstract class SettingsTestsBase
    {
        protected abstract FlurlHttpSettings GetSettings();
        protected abstract IFlurlRequest GetRequest();

        [Test]
        public async Task can_allow_non_success_status()
        {
            using (var test = new HttpTest())
            {
                GetSettings().AllowedHttpStatusRange = "4xx";
                test.RespondWith("I'm a teapot", 418);
                try
                {
                    var result = await GetRequest().GetAsync();
                    Assert.AreEqual(418, result.StatusCode);
                }
                catch (Exception)
                {
                    Assert.Fail("Exception should not have been thrown.");
                }
            }
        }

        [Test]
        public async Task can_set_pre_callback()
        {
            var callbackCalled = false;
            using (var test = new HttpTest())
            {
                test.RespondWith("ok");
                GetSettings().BeforeCall = call => {
                    Assert.Null(call.Response); // verifies that callback is running before HTTP call is made
                    callbackCalled = true;
                };
                Assert.IsFalse(callbackCalled);
                await GetRequest().GetAsync();
                Assert.IsTrue(callbackCalled);
            }
        }

        [Test]
        public async Task can_set_post_callback()
        {
            var callbackCalled = false;
            using (var test = new HttpTest())
            {
                test.RespondWith("ok");
                GetSettings().AfterCall = call => {
                    Assert.NotNull(call.Response); // verifies that callback is running after HTTP call is made
                    callbackCalled = true;
                };
                Assert.IsFalse(callbackCalled);
                await GetRequest().GetAsync();
                Assert.IsTrue(callbackCalled);
            }
        }

        [TestCase(true)]
        [TestCase(false)]
        public async Task can_set_error_callback(bool markExceptionHandled)
        {
            var callbackCalled = false;
            using (var test = new HttpTest())
            {
                test.RespondWith("server error", 500);
                GetSettings().OnError = call => {
                    Assert.NotNull(call.Response); // verifies that callback is running after HTTP call is made
                    callbackCalled = true;
                    call.ExceptionHandled = markExceptionHandled;
                };
                Assert.IsFalse(callbackCalled);
                try
                {
                    await GetRequest().GetAsync();
                    Assert.IsTrue(callbackCalled, "OnError was never called");
                    Assert.IsTrue(markExceptionHandled, "ExceptionHandled was marked false in callback, but exception was not propagated.");
                }
                catch (FlurlHttpException)
                {
                    Assert.IsTrue(callbackCalled, "OnError was never called");
                    Assert.IsFalse(markExceptionHandled, "ExceptionHandled was marked true in callback, but exception was propagated.");
                }
            }
        }

        [Test]
        public async Task can_disable_exception_behavior()
        {
            using (var test = new HttpTest())
            {
                GetSettings().OnError = call => {
                    call.ExceptionHandled = true;
                };
                test.RespondWith("server error", 500);
                try
                {
                    var result = await GetRequest().GetAsync();
                    Assert.AreEqual(500, result.StatusCode);
                }
                catch (FlurlHttpException)
                {
                    Assert.Fail("Flurl should not have thrown exception.");
                }
            }
        }

        [Test]
        public void can_reset_defaults()
        {
            GetSettings().JsonSerializer = null;
            GetSettings().Redirects.Enabled = false;
            GetSettings().BeforeCall = (call) => Console.WriteLine("Before!");
            GetSettings().Redirects.MaxAutoRedirects = 5;

            Assert.IsNull(GetSettings().JsonSerializer);
            Assert.IsFalse(GetSettings().Redirects.Enabled);
            Assert.IsNotNull(GetSettings().BeforeCall);
            Assert.AreEqual(5, GetSettings().Redirects.MaxAutoRedirects);

            GetSettings().ResetDefaults();

            Assert.That(GetSettings().JsonSerializer is DefaultJsonSerializer);
            Assert.IsTrue(GetSettings().Redirects.Enabled);
            Assert.IsNull(GetSettings().BeforeCall);
            Assert.AreEqual(10, GetSettings().Redirects.MaxAutoRedirects);
        }

        [Test] // #256
        public async Task explicit_content_type_header_is_not_overridden()
        {
            using (var test = new HttpTest())
            {
                // PostJsonAsync will normally set Content-Type to application/json, but it shouldn't touch it if it was set explicitly.
                await "https://api.com"
                    .WithHeader("content-type", "application/json-patch+json; utf-8")
                    .WithHeader("CONTENT-LENGTH", 10) // header names are case-insensitive
                    .PostJsonAsync(new { foo = "bar" });

                var h = test.CallLog[0].HttpRequestMessage.Content.Headers;
                Assert.AreEqual(new[] { "application/json-patch+json; utf-8" }, h.GetValues("Content-Type"));
                Assert.AreEqual(new[] { "10" }, h.GetValues("Content-Length"));
            }
        }
    }

    [TestFixture, NonParallelizable] // touches global settings, so can't run in parallel
    public class GlobalSettingsTests : SettingsTestsBase
    {
        protected override FlurlHttpSettings GetSettings() => FlurlHttp.GlobalSettings;
        protected override IFlurlRequest GetRequest() => new FlurlRequest("http://api.com");

        [TearDown]
        public void ResetDefaults() => FlurlHttp.GlobalSettings.ResetDefaults();

        [Test]
        public void settings_propagate_correctly()
        {
            FlurlHttp.GlobalSettings.Redirects.Enabled = false;
            FlurlHttp.GlobalSettings.AllowedHttpStatusRange = "4xx";
            FlurlHttp.GlobalSettings.Redirects.MaxAutoRedirects = 123;

            var client1 = new FlurlClient();
            client1.Settings.Redirects.Enabled = true;
            Assert.AreEqual("4xx", client1.Settings.AllowedHttpStatusRange);
            Assert.AreEqual(123, client1.Settings.Redirects.MaxAutoRedirects);
            client1.Settings.AllowedHttpStatusRange = "5xx";
            client1.Settings.Redirects.MaxAutoRedirects = 456;

            var req = client1.Request("http://myapi.com");
            Assert.IsTrue(req.Settings.Redirects.Enabled, "request should inherit client settings when not set at request level");
            Assert.AreEqual("5xx", req.Settings.AllowedHttpStatusRange, "request should inherit client settings when not set at request level");
            Assert.AreEqual(456, req.Settings.Redirects.MaxAutoRedirects, "request should inherit client settings when not set at request level");

            var client2 = new FlurlClient();
            client2.Settings.Redirects.Enabled = false;

            req.Client = client2;
            Assert.IsFalse(req.Settings.Redirects.Enabled, "request should inherit client settings when not set at request level");
            Assert.AreEqual("4xx", req.Settings.AllowedHttpStatusRange, "request should inherit global settings when not set at request or client level");
            Assert.AreEqual(123, req.Settings.Redirects.MaxAutoRedirects, "request should inherit global settings when not set at request or client level");

            client2.Settings.Redirects.Enabled = true;
            client2.Settings.AllowedHttpStatusRange = "3xx";
            client2.Settings.Redirects.MaxAutoRedirects = 789;
            Assert.IsTrue(req.Settings.Redirects.Enabled, "request should inherit client settings when not set at request level");
            Assert.AreEqual("3xx", req.Settings.AllowedHttpStatusRange, "request should inherit client settings when not set at request level");
            Assert.AreEqual(789, req.Settings.Redirects.MaxAutoRedirects, "request should inherit client settings when not set at request level");

            req.Settings.Redirects.Enabled = false;
            req.Settings.AllowedHttpStatusRange = "6xx";
            req.Settings.Redirects.MaxAutoRedirects = 2;
            Assert.IsFalse(req.Settings.Redirects.Enabled, "request-level settings should override any defaults");
            Assert.AreEqual("6xx", req.Settings.AllowedHttpStatusRange, "request-level settings should override any defaults");
            Assert.AreEqual(2, req.Settings.Redirects.MaxAutoRedirects, "request-level settings should override any defaults");

            req.Settings.ResetDefaults();
            Assert.IsTrue(req.Settings.Redirects.Enabled, "request should inherit client settings when cleared at request level");
            Assert.AreEqual("3xx", req.Settings.AllowedHttpStatusRange, "request should inherit client settings when cleared request level");
            Assert.AreEqual(789, req.Settings.Redirects.MaxAutoRedirects, "request should inherit client settings when cleared request level");

            client2.Settings.ResetDefaults();
            Assert.IsFalse(req.Settings.Redirects.Enabled, "request should inherit global settings when cleared at request and client level");
            Assert.AreEqual("4xx", req.Settings.AllowedHttpStatusRange, "request should inherit global settings when cleared at request and client level");
            Assert.AreEqual(123, req.Settings.Redirects.MaxAutoRedirects, "request should inherit global settings when cleared at request and client level");
        }

        [Test]
        public async Task can_provide_custom_client_factory()
        {
            FlurlHttp.GlobalSettings.FlurlClientFactory = new MyCustomClientFactory();
            var req = GetRequest();

            // client not assigned until request is sent
            using var test = new HttpTest();
            await req.GetAsync();

            Assert.IsInstanceOf<MyCustomHttpClient>(req.Client.HttpClient);
        }

        [Test]
        public void can_configure_global_from_FlurlHttp_object()
        {
            FlurlHttp.Configure(settings => settings.Redirects.Enabled = false);
            Assert.IsFalse(FlurlHttp.GlobalSettings.Redirects.Enabled);
        }

        [Test]
        public async Task can_configure_client_from_FlurlHttp_object()
        {
            FlurlHttp.ConfigureClient("http://host1.com/foo", cli => cli.Settings.Redirects.Enabled = false);
            var req1 = new FlurlRequest("http://host1.com/bar"); // different URL but same host, should use above client
            var req2 = new FlurlRequest("http://host2.com"); // different host, should use new client

            // client not assigned until request is sent
            using var test = new HttpTest();
            await Task.WhenAll(req1.GetAsync(), req2.GetAsync());

            Assert.IsFalse(req1.Client.Settings.Redirects.Enabled);
            Assert.IsTrue(req2.Client.Settings.Redirects.Enabled);
        }
    }

    [TestFixture, Parallelizable]
    public class HttpTestSettingsTests : SettingsTestsBase
    {
        private HttpTest _test;

        [SetUp]
        public void CreateTest() => _test = new HttpTest();

        [TearDown]
        public void DisposeTest() => _test.Dispose();

        protected override FlurlHttpSettings GetSettings() => HttpTest.Current.Settings;
        protected override IFlurlRequest GetRequest() => new FlurlRequest("http://api.com");

        [Test] // #246
        public void test_settings_dont_override_request_settings_when_not_set_explicitily()
        {
            var ser1 = new FakeSerializer();
            var ser2 = new FakeSerializer();

            using (var test = new HttpTest())
            {
                var cli = new FlurlClient();
                cli.Settings.JsonSerializer = ser1;
                Assert.AreSame(ser1, cli.Settings.JsonSerializer);

                var req = new FlurlRequest { Client = cli };
                Assert.AreSame(ser1, req.Settings.JsonSerializer);

                req.Settings.JsonSerializer = ser2;
                Assert.AreSame(ser2, req.Settings.JsonSerializer);
            }
        }

        private class FakeSerializer : ISerializer
        {
            public string Serialize(object obj) => "foo";
            public T Deserialize<T>(string s) => default;
            public T Deserialize<T>(Stream stream) => default;
        }
    }

    [TestFixture, Parallelizable]
    public class ClientSettingsTests : SettingsTestsBase
    {
        private readonly Lazy<IFlurlClient> _client = new Lazy<IFlurlClient>(() => new FlurlClient());

        protected override FlurlHttpSettings GetSettings() => _client.Value.Settings;
        protected override IFlurlRequest GetRequest() => _client.Value.Request("http://api.com");
    }

    [TestFixture, Parallelizable]
    public class RequestSettingsTests : SettingsTestsBase
    {
        private readonly Lazy<IFlurlRequest> _req = new Lazy<IFlurlRequest>(() => new FlurlRequest("http://api.com"));

        protected override FlurlHttpSettings GetSettings() => _req.Value.Settings;
        protected override IFlurlRequest GetRequest() => _req.Value;

        [Test]
        public void request_gets_global_settings_when_no_client()
        {
            var req = new FlurlRequest();
            Assert.IsNull(req.Client);
            Assert.IsNull(req.Url);
            Assert.AreEqual(FlurlHttp.GlobalSettings.JsonSerializer, req.Settings.JsonSerializer);
        }
    }

    class MyCustomClientFactory : DefaultFlurlClientFactory
    {
        public override HttpClient CreateHttpClient(HttpMessageHandler handler) => new MyCustomHttpClient();
    }

    class MyCustomHttpClient : HttpClient { }
    [TestFixture, Parallelizable]
    public class TestingTests : HttpTestFixtureBase
    {
        [Test]
        public async Task fake_get_works()
        {
            HttpTest.RespondWith("great job");

            await "http://www.api.com".GetAsync();

            var lastCall = HttpTest.CallLog.Last();
            Assert.AreEqual(200, lastCall.Response.StatusCode);
            Assert.AreEqual("great job", await lastCall.Response.GetStringAsync());
        }

        [Test]
        public async Task fake_post_works()
        {
            HttpTest.RespondWith("great job");

            await "http://www.api.com".PostJsonAsync(new { x = 5 });

            var lastCall = HttpTest.CallLog.Last();
            Assert.AreEqual("{\"x\":5}", lastCall.RequestBody);
            Assert.AreEqual(200, lastCall.Response.StatusCode);
            Assert.AreEqual("great job", await lastCall.Response.GetStringAsync());
        }

        [Test]
        public async Task no_response_setup_returns_empty_response()
        {
            await "http://www.api.com".GetAsync();

            var lastCall = HttpTest.CallLog.Last();
            Assert.AreEqual(200, lastCall.Response.StatusCode);
            Assert.AreEqual("", await lastCall.Response.GetStringAsync());
        }

        [Test] // #606
        public async Task null_response_setup_returns_empty_response()
        {
            HttpTest
                .RespondWith(status: 200)
                .RespondWith((string)null, status: 200)
                .RespondWith(() => null, status: 200);

            for (var i = 0; i < 3; i++)
            {
                var s = await "https://api.com".GetStringAsync();
                Assert.AreEqual("", s);
            }
        }

        [Test]
        public async Task can_setup_multiple_responses()
        {
            HttpTest
                .RespondWith("one")
                .RespondWith("two")
                .RespondWith("three");

            HttpTest.ShouldNotHaveMadeACall();

            await "http://www.api.com/1".GetAsync();
            await "http://www.api.com/2".GetAsync();
            await "http://www.api.com/3".GetAsync();

            var calls = HttpTest.CallLog;
            Assert.AreEqual(3, calls.Count);
            Assert.AreEqual("one", await calls[0].Response.GetStringAsync());
            Assert.AreEqual("two", await calls[1].Response.GetStringAsync());
            Assert.AreEqual("three", await calls[2].Response.GetStringAsync());

            HttpTest.ShouldHaveMadeACall();
            HttpTest.ShouldHaveCalled("http://www.api.com/*").WithVerb(HttpMethod.Get).Times(3);
            HttpTest.ShouldNotHaveCalled("http://www.otherapi.com/*");

            // #323 make sure it's a full string match and not a "contains"
            Assert.Throws<HttpTestException>(() => HttpTest.ShouldHaveCalled("http://www.api.com/"));
            HttpTest.ShouldNotHaveCalled("http://www.api.com/");
        }

        [Test] // #482
        public async Task last_response_is_sticky()
        {
            HttpTest.RespondWith("1").RespondWith("2").RespondWith("3");

            Assert.AreEqual("1", await "http://api.com".GetStringAsync());
            Assert.AreEqual("2", await "http://api.com".GetStringAsync());
            Assert.AreEqual("3", await "http://api.com".GetStringAsync());
            Assert.AreEqual("3", await "http://api.com".GetStringAsync());
            Assert.AreEqual("3", await "http://api.com".GetStringAsync());
        }

        [Test]
        public async Task can_respond_based_on_url()
        {
            HttpTest.RespondWith("never");
            HttpTest.ForCallsTo("*/1").RespondWith("one");
            HttpTest.ForCallsTo("*/2").RespondWith("two");
            HttpTest.ForCallsTo("*/3").RespondWith("three");
            HttpTest.ForCallsTo("http://www.api.com/*").RespondWith("foo!");

            Assert.AreEqual("foo!", await "http://www.api.com/4".GetStringAsync());
            Assert.AreEqual("three", await "http://www.api.com/3".GetStringAsync());
            Assert.AreEqual("two", await "http://www.api.com/2".GetStringAsync());
            Assert.AreEqual("one", await "http://www.api.com/1".GetStringAsync());

            Assert.AreEqual(4, HttpTest.CallLog.Count);
        }

        [Test]
        public async Task can_respond_based_on_verb()
        {
            HttpTest.RespondWith("catch-all");

            HttpTest
                .ForCallsTo("http://www.api.com*")
                .WithVerb(HttpMethod.Post)
                .RespondWith("I posted.");

            HttpTest
                .ForCallsTo("http://www.api.com*")
                .WithVerb("put", "PATCH")
                .RespondWith("I put or patched.");

            Assert.AreEqual("I put or patched.", await "http://www.api.com/1".PatchAsync(null).ReceiveString());
            Assert.AreEqual("I posted.", await "http://www.api.com/2".PostAsync(null).ReceiveString());
            Assert.AreEqual("I put or patched.", await "http://www.api.com/3".SendAsync(HttpMethod.Put, null).ReceiveString());
            Assert.AreEqual("catch-all", await "http://www.api.com/4".DeleteAsync().ReceiveString());

            Assert.AreEqual(4, HttpTest.CallLog.Count);
        }

        [Test]
        public async Task can_respond_based_on_query_params()
        {
            HttpTest
                .ForCallsTo("*")
                .WithQueryParam("x", 1)
                .WithQueryParams(new { y = 2, z = 3 })
                .WithAnyQueryParam("a", "b", "c")
                .WithoutQueryParam("d")
                .WithoutQueryParams(new { c = "n*" })
                .RespondWith("query param conditions met!");

            Assert.AreEqual("", await "http://api.com?x=1&y=2&a=yes".GetStringAsync());
            Assert.AreEqual("", await "http://api.com?y=2&z=3&b=yes".GetStringAsync());
            Assert.AreEqual("", await "http://api.com?x=1&y=2&z=3&c=yes&d=yes".GetStringAsync());
            Assert.AreEqual("", await "http://api.com?x=1&y=2&z=3&c=no".GetStringAsync());
            Assert.AreEqual("query param conditions met!", await "http://api.com?x=1&y=2&z=3&c=yes".GetStringAsync());
        }

        [Test] // #596
        public async Task url_patterns_ignore_query_when_not_specified()
        {
            HttpTest.ForCallsTo("http://api.com/1").RespondWith("one");
            HttpTest.ForCallsTo("http://api.com/2").WithAnyQueryParam().RespondWith("two");
            HttpTest.ForCallsTo("http://api.com/3").WithoutQueryParams().RespondWith("three");

            Assert.AreEqual("one", await "http://api.com/1".GetStringAsync());
            Assert.AreEqual("one", await "http://api.com/1?x=foo&y=bar".GetStringAsync());

            Assert.AreEqual("", await "http://api.com/2".GetStringAsync());
            Assert.AreEqual("two", await "http://api.com/2?x=foo&y=bar".GetStringAsync());

            Assert.AreEqual("three", await "http://api.com/3".GetStringAsync());
            Assert.AreEqual("", await "http://api.com/3?x=foo&y=bar".GetStringAsync());

            HttpTest.ShouldHaveCalled("http://api.com/1").Times(2);
            HttpTest.ShouldHaveCalled("http://api.com/1").WithAnyQueryParam().Times(1);
            HttpTest.ShouldHaveCalled("http://api.com/1").WithoutQueryParams().Times(1);
            HttpTest.ShouldHaveCalled("http://api.com/1?x=foo").Times(1);
            HttpTest.ShouldHaveCalled("http://api.com/1?x=foo").WithQueryParam("y").Times(1);
            HttpTest.ShouldHaveCalled("http://api.com/1?x=foo").WithQueryParam("y", "bar").Times(1);
        }

        [Test]
        public async Task can_respond_based_on_headers()
        {
            HttpTest
                .ForCallsTo("*")
                .WithHeader("x")
                .WithHeader("y", "f*o")
                .WithoutHeader("y", "flo")
                .WithoutHeader("z")
                .RespondWith("header conditions met!");

            Assert.AreEqual("", await "http://api.com".WithHeaders(new { y = "foo" }).GetStringAsync());
            Assert.AreEqual("", await "http://api.com".WithHeaders(new { x = 1, y = "flo" }).GetStringAsync());
            Assert.AreEqual("", await "http://api.com".WithHeaders(new { x = 1, y = "foo", z = 2 }).GetStringAsync());
            Assert.AreEqual("header conditions met!", await "http://api.com".WithHeaders(new { x = 1, y = "foo" }).GetStringAsync());
        }

        [Test]
        public async Task can_respond_based_on_body()
        {
            HttpTest
                .ForCallsTo("*")
                .WithRequestBody("*something*")
                .WithRequestJson(new { a = "*", b = new { c = "*", d = "yes" } })
                .RespondWith("body conditions met!");

            Assert.AreEqual("", await "http://api.com".PostStringAsync("something").ReceiveString());
            Assert.AreEqual("", await "http://api.com".PostJsonAsync(
                new { a = "hi", b = new { c = "bye", d = "yes" } }).ReceiveString());

            Assert.AreEqual("body conditions met!", await "http://api.com".PostJsonAsync(
                new { a = "hi", b = new { c = "this is something!", d = "yes" } }).ReceiveString());
        }

        [Test]
        public async Task can_respond_based_on_any_call_condition()
        {
            HttpTest
                .ForCallsTo("*")
                .With(call => call.Request.Url.Fragment.StartsWith("abc"))
                .Without(call => call.Request.Url.Fragment.EndsWith("xyz"))
                .RespondWith("arbitrary conditions met!");

            Assert.AreEqual("", await "http://api.com#abcxyz".GetStringAsync());
            Assert.AreEqual("", await "http://api.com#xyz".GetStringAsync());
            Assert.AreEqual("arbitrary conditions met!", await "http://api.com#abcxy".GetStringAsync());
        }

        [Test]
        public async Task can_assert_verb()
        {
            await "http://www.api.com/1".PostStringAsync("");
            await "http://www.api.com/2".PutStringAsync("");
            await "http://www.api.com/3".PatchStringAsync("");
            await "http://www.api.com/4".DeleteAsync();

            HttpTest.ShouldHaveMadeACall().WithVerb(HttpMethod.Post).Times(1);
            HttpTest.ShouldHaveMadeACall().WithVerb("put", "PATCH").Times(2);
            HttpTest.ShouldHaveMadeACall().WithVerb("get", "delete").Times(1);
            Assert.Throws<HttpTestException>(() => HttpTest.ShouldHaveMadeACall().WithVerb(HttpMethod.Get));
        }

        [Test]
        public async Task can_assert_json_request()
        {
            var body = new { a = 1, b = 2 };
            await "http://some-api.com".PostJsonAsync(body);

            HttpTest.ShouldHaveMadeACall().WithRequestJson(body);
        }

        [Test]
        public async Task can_assert_url_encoded_request()
        {
            var body = new { a = 1, b = 2, c = "hi there", d = new[] { 1, 2, 3 } };
            await "http://some-api.com".PostUrlEncodedAsync(body);

            HttpTest.ShouldHaveMadeACall().WithRequestUrlEncoded(body);
        }

        [Test]
        public async Task can_assert_query_params()
        {
            await "http://www.api.com?x=111&y=222&z=333#abcd".GetAsync();

            HttpTest.ShouldHaveCalled("http://www.api.com*").WithQueryParams();
            HttpTest.ShouldHaveMadeACall().WithQueryParam("x");
            HttpTest.ShouldHaveCalled("http://www.api.com*").WithQueryParams("z", "y");
            HttpTest.ShouldHaveMadeACall().WithQueryParam("y", 222);
            HttpTest.ShouldHaveCalled("http://www.api.com*").WithQueryParam("z", "*3");
            HttpTest.ShouldHaveMadeACall().WithQueryParams(new { z = 333, y = 222 });
            HttpTest.ShouldHaveMadeACall().WithQueryParams(new { z = "*", y = 222, x = "*" });
            HttpTest.ShouldHaveMadeACall().WithAnyQueryParam("a", "z", "b");

            // without
            HttpTest.ShouldHaveCalled("http://www.api.com*").WithoutQueryParam("w");
            HttpTest.ShouldHaveMadeACall().WithoutQueryParams("t", "u", "v");
            HttpTest.ShouldHaveCalled("http://www.api.com*").WithoutQueryParam("x", 112);
            HttpTest.ShouldHaveMadeACall().WithoutQueryParams(new { x = 112, y = 223, z = 666 });
            HttpTest.ShouldHaveMadeACall().WithoutQueryParams(new { a = "*", b = "*" });

            // failures
            Assert.Throws<HttpTestException>(() =>
                HttpTest.ShouldHaveMadeACall().WithQueryParam("w"));
            Assert.Throws<HttpTestException>(() =>
                HttpTest.ShouldHaveMadeACall().WithQueryParam("y", 223));
            Assert.Throws<HttpTestException>(() =>
                HttpTest.ShouldHaveMadeACall().WithQueryParam("z", "*4"));
            Assert.Throws<HttpTestException>(() =>
                HttpTest.ShouldHaveMadeACall().WithQueryParams(new { x = 111, y = 666 }));

            Assert.Throws<HttpTestException>(() =>
                HttpTest.ShouldHaveMadeACall().WithoutQueryParams());
            Assert.Throws<HttpTestException>(() =>
                HttpTest.ShouldHaveMadeACall().WithoutQueryParam("x"));
            Assert.Throws<HttpTestException>(() =>
                HttpTest.ShouldHaveMadeACall().WithoutQueryParams("z", "y"));
            Assert.Throws<HttpTestException>(() =>
                HttpTest.ShouldHaveMadeACall().WithoutQueryParam("y", 222));
            Assert.Throws<HttpTestException>(() =>
                HttpTest.ShouldHaveMadeACall().WithoutQueryParam("z", "*3"));
            Assert.Throws<HttpTestException>(() =>
                HttpTest.ShouldHaveMadeACall().WithoutQueryParams(new { z = 333, y = 222 }));
        }

        [TestCase(false)]
        [TestCase(true)]
        public async Task can_assert_multiple_occurances_of_query_param(bool buildFluently)
        {
            // #276 showed that this failed when the URL was built fluently (caused by #301)
            var url = buildFluently ?
                "http://www.api.com".SetQueryParam("x", new[] { 1, 2, 3 }).SetQueryParam("y", 4).SetFragment("abcd") :
                new Flurl("http://www.api.com?x=1&x=2&x=3&y=4#abcd");

            await url.GetAsync();

            HttpTest.ShouldHaveMadeACall().WithQueryParam("x");
            HttpTest.ShouldHaveMadeACall().WithQueryParam("x", new[] { 2, 1 }); // order shouldn't matter
            HttpTest.ShouldHaveMadeACall().WithQueryParams(new { x = new[] { 3, 2, 1 } }); // order shouldn't matter

            Assert.Throws<HttpTestException>(() =>
                HttpTest.ShouldHaveMadeACall().WithQueryParam("x", new[] { 1, 2, 4 }));
            Assert.Throws<HttpTestException>(() =>
                HttpTest.ShouldHaveMadeACall().WithQueryParams(new { x = new[] { 1, 2, 4 } }));
        }

        [Test]
        public async Task can_assert_headers()
        {
            await "http://api.com"
                .WithHeaders(new { h1 = "val1", h2 = "val2" })
                .WithHeader("User-Agent", "two words") // #307
                .WithHeader("x", "dos       words")    // crazier than #307
                .WithHeader("y", "hi;  there")         // crazier still
                .GetAsync();

            HttpTest.ShouldHaveMadeACall().WithHeader("h1");
            HttpTest.ShouldHaveMadeACall().WithHeader("h2", "val2");
            HttpTest.ShouldHaveMadeACall().WithHeader("h1", "val*");
            HttpTest.ShouldHaveMadeACall().WithHeader("User-Agent", "two words");
            HttpTest.ShouldHaveMadeACall().WithHeader("x", "dos       words");
            HttpTest.ShouldHaveMadeACall().WithHeader("y", "hi;  there");

            HttpTest.ShouldHaveMadeACall().WithoutHeader("h3");
            HttpTest.ShouldHaveMadeACall().WithoutHeader("h2", "val1");
            HttpTest.ShouldHaveMadeACall().WithoutHeader("h1", "foo*");

            Assert.Throws<HttpTestException>(() =>
                HttpTest.ShouldHaveMadeACall().WithHeader("h3"));
            Assert.Throws<HttpTestException>(() =>
                HttpTest.ShouldHaveMadeACall().WithoutHeader("h1"));
        }

        [Test]
        public async Task can_assert_oauth_token()
        {
            await "https://auth.com".WithOAuthBearerToken("foo").GetAsync();
            HttpTest.ShouldHaveMadeACall().WithOAuthBearerToken();
            HttpTest.ShouldHaveMadeACall().WithOAuthBearerToken("foo");
            HttpTest.ShouldHaveMadeACall().WithOAuthBearerToken("*oo");
            Assert.Throws<HttpTestException>(() => HttpTest.ShouldHaveMadeACall().WithOAuthBearerToken("bar"));
            Assert.Throws<HttpTestException>(() => HttpTest.ShouldHaveMadeACall().WithBasicAuth());
        }

        [Test]
        public async Task can_assert_basic_auth()
        {
            await "https://auth.com".WithBasicAuth("me", "letmein").GetAsync();
            HttpTest.ShouldHaveMadeACall().WithBasicAuth();
            HttpTest.ShouldHaveMadeACall().WithBasicAuth("me", "letmein");
            HttpTest.ShouldHaveMadeACall().WithBasicAuth("me");
            HttpTest.ShouldHaveMadeACall().WithBasicAuth("m*", "*in");
            Assert.Throws<HttpTestException>(() => HttpTest.ShouldHaveMadeACall().WithBasicAuth("me", "wrong"));
            Assert.Throws<HttpTestException>(() => HttpTest.ShouldHaveMadeACall().WithBasicAuth("you"));
            Assert.Throws<HttpTestException>(() => HttpTest.ShouldHaveMadeACall().WithBasicAuth("m*", "*out"));
            Assert.Throws<HttpTestException>(() => HttpTest.ShouldHaveMadeACall().WithOAuthBearerToken());
        }

        [Test]
        public async Task can_simulate_timeout()
        {
            HttpTest.SimulateTimeout();
            try
            {
                await "http://www.api.com".GetAsync();
                Assert.Fail("Exception was not thrown!");
            }
            catch (FlurlHttpTimeoutException ex)
            {
                Assert.IsInstanceOf<TaskCanceledException>(ex.InnerException);
                StringAssert.Contains("timed out", ex.Message);
            }
        }

        [Test]
        public async Task can_simulate_exception()
        {
            var expectedException = new SocketException();
            HttpTest.SimulateException(expectedException);
            try
            {
                await "http://www.api.com".GetAsync();
                Assert.Fail("Exception was not thrown!");
            }
            catch (FlurlHttpException ex)
            {
                Assert.AreEqual(expectedException, ex.InnerException);
            }
        }

        [Test]
        public async Task can_simulate_timeout_with_exception_handled()
        {
            HttpTest.SimulateTimeout();
            var exceptionCaught = false;

            var resp = await "http://api.com"
                .ConfigureRequest(c => c.OnError = call => {
                    exceptionCaught = true;
                    var ex = call.Exception as TaskCanceledException;
                    Assert.NotNull(ex);
                    Assert.IsInstanceOf<TimeoutException>(ex.InnerException);
                    call.ExceptionHandled = true;
                })
                .GetAsync();

            Assert.IsNull(resp);
            Assert.IsTrue(exceptionCaught);
        }

        [Test]
        public async Task can_fake_headers()
        {
            HttpTest.RespondWith(headers: new { h1 = "foo" });

            var resp = await "http://www.api.com".GetAsync();
            Assert.AreEqual("foo", resp.Headers.FirstOrDefault("h1"));
        }

        [Test]
        public async Task can_fake_cookies()
        {
            HttpTest.RespondWith(cookies: new { c1 = "foo" });

            var resp = await "http://www.api.com".GetAsync();
            Assert.AreEqual(1, resp.Cookies.Count);
            Assert.AreEqual("foo", resp.Cookies.FirstOrDefault(c => c.Name == "c1")?.Value);
        }

        // https://github.com/tmenier/Flurl/issues/175
        [Test]
        public async Task can_deserialize_default_response_more_than_once()
        {
            var resp = await "http://www.api.com".GetJsonAsync<object>();
            Assert.IsNull(resp);
            // bug: couldn't deserialize here due to reading stream twice
            resp = await "http://www.api.com".GetJsonAsync<object>();
            Assert.IsNull(resp);
            resp = await "http://www.api.com".GetJsonAsync<object>();
            Assert.IsNull(resp);
        }

        [Test]
        public void can_configure_settings_for_test()
        {
            Assert.IsTrue(new FlurlRequest().Settings.Redirects.Enabled);
            using (new HttpTest().Configure(settings => settings.Redirects.Enabled = false))
            {
                Assert.IsFalse(new FlurlRequest().Settings.Redirects.Enabled);
            }
            // test disposed, should revert back to global settings
            Assert.IsTrue(new FlurlRequest().Settings.Redirects.Enabled);
        }

        [Test]
        public async Task can_test_in_parallel()
        {
            async Task CallAndAssertCountAsync(int calls)
            {
                using (var test = new HttpTest())
                {
                    for (int i = 0; i < calls; i++)
                    {
                        await "http://www.api.com".GetAsync();
                        await Task.Delay(100);
                    }
                    test.ShouldHaveCalled("http://www.api.com").Times(calls);
                    //Console.WriteLine($"{calls} calls expected, {test.CallLog.Count} calls made");
                }
            }

            await Task.WhenAll(
                CallAndAssertCountAsync(7),
                CallAndAssertCountAsync(5),
                CallAndAssertCountAsync(3),
                CallAndAssertCountAsync(4),
                CallAndAssertCountAsync(6));
        }

        [Test] // #285
        public async Task does_not_throw_nullref_for_empty_content()
        {
            await "http://some-api.com".AppendPathSegment("foo").SendAsync(HttpMethod.Post, null);
            await "http://some-api.com".AppendPathSegment("foo").PostJsonAsync(new { foo = "bar" });

            HttpTest.ShouldHaveCalled("http://some-api.com/foo")
                .WithVerb(HttpMethod.Post)
                .WithContentType("application/json");
        }

        [Test] // #331
        public async Task can_fake_content_headers()
        {
            HttpTest.RespondWith("<xml></xml>", 200, new { Content_Type = "text/xml" });
            await "http://api.com".GetAsync();
            HttpTest.ShouldHaveMadeACall().With(call => call.Response.Headers.Contains(("Content-Type", "text/xml")));
            HttpTest.ShouldHaveMadeACall().With(call => call.HttpResponseMessage.Content.Headers.ContentType.MediaType == "text/xml");
        }

        [Test] // #335
        public async Task doesnt_record_calls_made_with_HttpClient()
        {
            using (var httpTest = new HttpTest())
            {
                httpTest.RespondWith("Hello");
                var flurClient = new FlurlClient();
                // use the underlying HttpClient directly
                await flurClient.HttpClient.GetStringAsync("https://www.google.com/");
                CollectionAssert.IsEmpty(httpTest.CallLog);
            }
        }

        [Test] // #366 & #398
        public async Task can_use_response_queue_in_parallel()
        {
            // this was hard to test. numbers used (200 ms delay, 10 calls, repeat 5 times) were not
            // arrived at by any exact science. they just seemed to be the point where failure is
            // virtually guaranteed without thread-safe collections backing ResponseQueue and CallLog,
            // but without making the test unbearably slow.
            var cli = new FlurlClient("http://api.com");
            cli.Settings.BeforeCallAsync = call => Task.Delay(200);

            for (var i = 0; i < 5; i++)
            {
                using (var test = new HttpTest())
                {
                    test
                        .RespondWith("0")
                        .RespondWith("1")
                        .RespondWith("2")
                        .RespondWith("3")
                        .RespondWith("4")
                        .RespondWith("5")
                        .RespondWith("6")
                        .RespondWith("7")
                        .RespondWith("8")
                        .RespondWith("9");

                    var results = await Task.WhenAll(
                        cli.Request().GetStringAsync(),
                        cli.Request().GetStringAsync(),
                        cli.Request().GetStringAsync(),
                        cli.Request().GetStringAsync(),
                        cli.Request().GetStringAsync(),
                        cli.Request().GetStringAsync(),
                        cli.Request().GetStringAsync(),
                        cli.Request().GetStringAsync(),
                        cli.Request().GetStringAsync(),
                        cli.Request().GetStringAsync());

                    CollectionAssert.AllItemsAreUnique(results);
                    test.ShouldHaveMadeACall().Times(10);
                }
            }
        }

        // #721
        [TestCase("https://api.com/foo?", "https://api.com/foo?")]
        [TestCase("https://api.com/foo?", "https://api.com/foo")]
        public async Task can_assert_url_ending_with_question_mark(string actual, string expected)
        {
            using var httpTest = new HttpTest();
            await actual.GetAsync();
            httpTest.ShouldHaveCalled(expected);
        }
    }
    public static class ReflectionHelper
    {
        public static MethodInfo[] GetAllExtensionMethods<T>(Assembly asm)
        {
            // http://stackoverflow.com/a/299526/62600
            return (
                from type in asm.GetTypes()
                where type.GetTypeInfo().IsSealed && !type.GetTypeInfo().IsGenericType && !type.GetTypeInfo().IsNested
                from method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                where method.IsDefined(typeof(ExtensionAttribute), false)
                where method.GetParameters()[0].ParameterType == typeof(T)
                select method).ToArray();
        }

        public static bool AreSameMethodSignatures(MethodInfo method1, MethodInfo method2)
        {
            if (method1.Name != method2.Name)
                return false;

            if (!AreSameType(method1.ReturnType, method2.ReturnType))
                return false;

            var genArgs1 = method1.GetGenericArguments();
            var genArgs2 = method2.GetGenericArguments();

            if (genArgs1.Length != genArgs2.Length)
                return false;

            for (int i = 0; i < genArgs1.Length; i++)
            {
                if (!AreSameType(genArgs1[i], genArgs2[i]))
                    return false;
            }

            var args1 = method1.GetParameters().Skip(IsExtensionMethod(method1) ? 1 : 0).ToArray();
            var args2 = method2.GetParameters().Skip(IsExtensionMethod(method2) ? 1 : 0).ToArray();

            if (args1.Length != args2.Length)
                return false;

            for (int i = 0; i < args1.Length; i++)
            {
                if (args1[i].Name != args2[i].Name) return false;
                if (!AreSameType(args1[i].ParameterType, args2[i].ParameterType)) return false;
                if (args1[i].IsOptional != args2[i].IsOptional) return false;
                if (!AreSameValue(args1[i].DefaultValue, args2[i].DefaultValue)) return false;
                if (args1[i].IsIn != args2[i].IsIn) return false;
            }
            return true;
        }

        public static bool IsExtensionMethod(MethodInfo method)
        {
            var type = method.DeclaringType;
            return
                type.GetTypeInfo().IsSealed &&
                !type.GetTypeInfo().IsGenericType &&
                !type.GetTypeInfo().IsNested &&
                method.IsStatic &&
                method.IsDefined(typeof(ExtensionAttribute), false);
        }

        public static bool AreSameValue(object a, object b)
        {
            if (a == null && b == null)
                return true;
            if (a == null ^ b == null)
                return false;
            // ok, neither is null
            return a.Equals(b);
        }

        public static bool AreSameType(Type a, Type b)
        {
            if (a.IsGenericParameter && b.IsGenericParameter)
            {

                var constraintsA = a.GetTypeInfo().GetGenericParameterConstraints();
                var constraintsB = b.GetTypeInfo().GetGenericParameterConstraints();

                if (constraintsA.Length != constraintsB.Length)
                    return false;

                for (int i = 0; i < constraintsA.Length; i++)
                {
                    if (!AreSameType(constraintsA[i], constraintsB[i]))
                        return false;
                }
                return true;
            }

            if (a.GetTypeInfo().IsGenericType && b.GetTypeInfo().IsGenericType)
            {

                if (a.GetGenericTypeDefinition() != b.GetGenericTypeDefinition())
                    return false;

                var genArgsA = a.GetGenericArguments();
                var genArgsB = b.GetGenericArguments();

                if (genArgsA.Length != genArgsB.Length)
                    return false;

                for (int i = 0; i < genArgsA.Length; i++)
                {
                    if (!AreSameType(genArgsA[i], genArgsB[i]))
                        return false;
                }

                return true;
            }

            return a == b;
        }
    }
    [TestFixture, Parallelizable]
    public class CommonExtensionsTests
    {
        [Test]
        public void can_parse_object_to_kv()
        {
            var kv = new
            {
                one = 1,
                two = "foo",
                three = (string)null
            }.ToKeyValuePairs();

            AssertKV(kv, ("one", 1), ("two", "foo"), ("three", null));
        }

        [Test]
        public void can_parse_dictionary_to_kv()
        {
            var kv = new Dictionary<string, object> {
                { "one", 1 },
                { "two", "foo" },
                { "three", null }
            }.ToKeyValuePairs();

            AssertKV(kv, ("one", 1), ("two", "foo"), ("three", null));
        }

        [Test]
        public void can_parse_collection_of_kvp_to_kv()
        {
            var kv = new[] {
                new KeyValuePair<object, object>("one", 1),
                new KeyValuePair<object, object>("two", "foo"),
                new KeyValuePair<object, object>("three", null),
                new KeyValuePair<object, object>(null, "four"),
            }.ToKeyValuePairs();

            AssertKV(kv, ("one", 1), ("two", "foo"), ("three", null));
        }

        [Test]
        public void can_parse_collection_of_conventional_objects_to_kv()
        {
            // convention is to accept collection of any arbitrary type that contains
            // a property called Key or Name and a property called Value
            var kv = new object[] {
                new { Key = "one", Value = 1 },
                new { key = "two", value = "foo" }, // lower-case should work too
				new { Key = (string)null, Value = 3 }, // null keys should get skipped
				new { Name = "three", Value = (string)null },
                new { name = "four", value = "bar" } // lower-case should work too
			}.ToKeyValuePairs().ToList();

            AssertKV(kv, ("one", 1), ("two", "foo"), ("three", null), ("four", "bar"));
        }

        [Test]
        public void can_parse_string_to_kv()
        {
            var kv = "one=1&two=foo&three".ToKeyValuePairs();
            AssertKV(kv, ("one", "1"), ("two", "foo"), ("three", null));
        }

        [Test]
        public void cannot_parse_null_to_kv()
        {
            object obj = null;
            Assert.Throws<ArgumentNullException>(() => obj.ToKeyValuePairs());
        }

        private class ToKvMe
        {
            public string Read1 { get; private set; }
            public string Read2 { get; private set; }

            private string PrivateRead1 { get; set; } = "c";
            public string PrivateRead2 { private get; set; }

            public string WriteOnly
            {
                set
                {
                    Read1 = value.Split(',')[0];
                    Read2 = value.Split(',')[1];
                }
            }
        }

        [Test] // #373
        public void object_to_kv_ignores_props_not_publicly_readable()
        {
            var kv = new ToKvMe
            {
                PrivateRead2 = "foo",
                WriteOnly = "a,b"
            }.ToKeyValuePairs();

            AssertKV(kv, ("Read1", "a"), ("Read2", "b"));
        }

        [Test]
        public void SplitOnFirstOccurence_works()
        {
            var result = "hello/how/are/you".SplitOnFirstOccurence("/");
            Assert.AreEqual(new[] { "hello", "how/are/you" }, result);
        }

        [TestCase("   \"\thi there \"  \t\t ", ExpectedResult = "\thi there ")]
        [TestCase("   '  hi there  '   ", ExpectedResult = "  hi there  ")]
        [TestCase("  hi there  ", ExpectedResult = "  hi there  ")]
        public string StripQuotes_works(string s) => s.StripQuotes();

        [Test]
        public void ToInvariantString_serializes_dates_to_iso()
        {
            Assert.AreEqual("2017-12-01T02:34:56.7890000", new DateTime(2017, 12, 1, 2, 34, 56, 789, DateTimeKind.Unspecified).ToInvariantString());
            Assert.AreEqual("2017-12-01T02:34:56.7890000Z", new DateTime(2017, 12, 1, 2, 34, 56, 789, DateTimeKind.Utc).ToInvariantString());
            Assert.AreEqual("2017-12-01T02:34:56.7890000-06:00", new DateTimeOffset(2017, 12, 1, 2, 34, 56, 789, TimeSpan.FromHours(-6)).ToInvariantString());
        }

        private void AssertKV(IEnumerable<(string, object)> actual, params (string, object)[] expected)
        {
            CollectionAssert.AreEqual(expected, actual);
        }
    }
    [TestFixture, Parallelizable]
    public class UtilityMethodTests
    {
        [Test]
        public void Combine_works()
        {
            var url = Flurl.Combine("http://www.foo.com/", "/too/", "/many/", "/slashes/", "too", "few", "one/two/");
            Assert.AreEqual("http://www.foo.com/too/many/slashes/too/few/one/two/", url);
        }

        [TestCase("segment?", "foo=bar", "x=1&y=2&")]
        [TestCase("segment", "?foo=bar&x=1", "y=2&")]
        [TestCase("segment", "?", "foo=bar&x=1&y=2&")]
        [TestCase("/segment?foo=bar&", "&x=1&", "&y=2&")]
        [TestCase(null, "segment?foo=bar&x=1&y=2&", "")]
        public void Combine_supports_query(string a, string b, string c)
        {
            var url = Flurl.Combine("http://root.com", a, b, c);
            Assert.AreEqual("http://root.com/segment?foo=bar&x=1&y=2&", url);
        }

        [Test]
        public void Combine_encodes_illegal_chars()
        {
            var url = Flurl.Combine("http://www.foo.com", "hi there", "?", "x=hi there", "#", "hi there");
            Assert.AreEqual("http://www.foo.com/hi%20there?x=hi%20there#hi%20there", url);
        }

        [Test] // #185
        public void can_encode_and_decode_very_long_value()
        {
            // 65,520 chars is the tipping point for Uri.EscapeDataString https://github.com/dotnet/corefx/issues/1936
            var len = 500000;

            // every 10th char needs to be encoded
            var s = string.Concat(Enumerable.Repeat("xxxxxxxxx ", len / 10));
            Assert.AreEqual(len, s.Length); // just a sanity check

            // encode space as %20
            var encoded = Flurl.Encode(s, false);
            // hex encoding will add 2 addtional chars for every char that needs to be encoded
            Assert.AreEqual(len + (2 * len / 10), encoded.Length);
            var expected = string.Concat(Enumerable.Repeat("xxxxxxxxx%20", len / 10));
            Assert.AreEqual(expected, encoded);

            var decoded = Flurl.Decode(encoded, false);
            Assert.AreEqual(s, decoded);

            // encode space as +
            encoded = Flurl.Encode(s, true);
            Assert.AreEqual(len, encoded.Length);
            expected = string.Concat(Enumerable.Repeat("xxxxxxxxx+", len / 10));
            Assert.AreEqual(expected, encoded);

            // interpret + as space
            decoded = Flurl.Decode(encoded, true);
            Assert.AreEqual(s, decoded);

            // don't interpret + as space, encoded and decoded should be the same
            decoded = Flurl.Decode(encoded, false);
            Assert.AreEqual(encoded, decoded);
        }

        [TestCase("http://www.mysite.com/more", true)]
        [TestCase("http://www.mysite.com/more?x=1&y=2", true)]
        [TestCase("http://www.mysite.com/more?x=1&y=2#frag", true)]
        [TestCase("http://www.mysite.com#frag", true)]
        [TestCase("", false)]
        [TestCase("blah", false)]
        [TestCase("http:/www.mysite.com", false)]
        [TestCase("www.mysite.com", false)]
        public void IsValid_works(string s, bool isValid)
        {
            Assert.AreEqual(isValid, Flurl.IsValid(s));
        }
    }
    [TestFixture, Parallelizable]
    public class UrlParsingTests
    {
        // relative
        [TestCase("//relative/with/authority", "", "relative", "", "relative", null, "/with/authority", "", "")]
        [TestCase("/relative/without/authority", "", "", "", "", null, "/relative/without/authority", "", "")]
        [TestCase("relative/without/path/anchor", "", "", "", "", null, "relative/without/path/anchor", "", "")]
        // absolute
        [TestCase("http://www.mysite.com/with/path?x=1", "http", "www.mysite.com", "", "www.mysite.com", null, "/with/path", "x=1", "")]
        [TestCase("https://www.mysite.com/with/path?x=1#foo", "https", "www.mysite.com", "", "www.mysite.com", null, "/with/path", "x=1", "foo")]
        [TestCase("http://user:pass@www.mysite.com:8080/with/path?x=1?y=2", "http", "user:pass@www.mysite.com:8080", "user:pass", "www.mysite.com", 8080, "/with/path", "x=1?y=2", "")]
        [TestCase("http://www.mysite.com/#with/path?x=1?y=2", "http", "www.mysite.com", "", "www.mysite.com", null, "/", "", "with/path?x=1?y=2")]
        // from https://en.wikipedia.org/wiki/Uniform_Resource_Identifier#Examples
        [TestCase("https://john.doe@www.example.com:123/forum/questions/?tag=networking&order=newest#top", "https", "john.doe@www.example.com:123", "john.doe", "www.example.com", 123, "/forum/questions/", "tag=networking&order=newest", "top")]
        [TestCase("ldap://[2001:db8::7]/c=GB?objectClass?one", "ldap", "[2001:db8::7]", "", "[2001:db8::7]", null, "/c=GB", "objectClass?one", "")]
        [TestCase("mailto:John.Doe@example.com", "mailto", "", "", "", null, "John.Doe@example.com", "", "")]
        [TestCase("news:comp.infosystems.www.servers.unix", "news", "", "", "", null, "comp.infosystems.www.servers.unix", "", "")]
        [TestCase("tel:+1-816-555-1212", "tel", "", "", "", null, "+1-816-555-1212", "", "")]
        [TestCase("telnet://192.0.2.16:80/", "telnet", "192.0.2.16:80", "", "192.0.2.16", 80, "/", "", "")]
        [TestCase("urn:oasis:names:specification:docbook:dtd:xml:4.1.2", "urn", "", "", "", null, "oasis:names:specification:docbook:dtd:xml:4.1.2", "", "")]
        // with uppercase letters
        [TestCase("http://www.mySite.com:8080/With/Path?x=1?Y=2", "http", "www.mysite.com:8080", "", "www.mysite.com", 8080, "/With/Path", "x=1?Y=2", "")]
        [TestCase("HTTP://www.mysite.com:8080", "http", "www.mysite.com:8080", "", "www.mysite.com", 8080, "", "", "")]
        public void can_parse_url_parts(string url, string scheme, string authority, string userInfo, string host, int? port, string path, string query, string fragment)
        {
            // there's a few ways to get Url object so let's check all of them
            foreach (var parsed in new[] { new Flurl(url), Flurl.Parse(url), new Flurl(new Uri(url, UriKind.RelativeOrAbsolute)) })
            {
                Assert.AreEqual(scheme, parsed.Scheme);
                Assert.AreEqual(authority, parsed.Authority);
                Assert.AreEqual(userInfo, parsed.UserInfo);
                Assert.AreEqual(host, parsed.Host);
                Assert.AreEqual(port, parsed.Port);
                Assert.AreEqual(path, parsed.Path);
                Assert.AreEqual(query, parsed.Query);
                Assert.AreEqual(fragment, parsed.Fragment);
            }
        }

        [TestCase("http://www.trailing-slash.com/", "/")]
        [TestCase("http://www.trailing-slash.com/a/b/", "/a/b/")]
        [TestCase("http://www.trailing-slash.com/a/b/?x=y", "/a/b/")]
        [TestCase("http://www.no-trailing-slash.com", "")]
        [TestCase("http://www.no-trailing-slash.com/a/b", "/a/b")]
        [TestCase("http://www.no-trailing-slash.com/a/b?x=y", "/a/b")]
        public void path_retains_trailing_slash(string original, string path)
        {
            var url = Flurl.Parse(original);
            Assert.AreEqual(original, url.ToString());
            Assert.AreEqual(path, url.Path);
        }

        [TestCase("https://foo.com/x?")]
        [TestCase("https://foo.com/x#")]
        [TestCase("https://foo.com/x?#")]
        public void retains_trailing_chars(string original)
        {
            var url = Flurl.Parse(original);
            Assert.AreEqual(original, url.ToString());
        }

        [Test]
        public void can_parse_query_params()
        {
            var q = new Flurl("http://www.mysite.com/more?x=1&y=2&z=3&y=4&abc&xyz&foo=&=bar&y=6").QueryParams;

            Assert.AreEqual(new (string, object)[] {
                ("x", "1"),
                ("y", "2"),
                ("z", "3"),
                ("y", "4"),
                ("abc", null),
                ("xyz", null),
                ("foo", ""),
                ("", "bar"),
                ("y", "6")
            }, q.ToArray());

            Assert.AreEqual(("y", "4"), q[3]);
            Assert.AreEqual("foo", q[6].Name);
            Assert.AreEqual("bar", q[7].Value);

            Assert.AreEqual("1", q.FirstOrDefault("x"));
            Assert.AreEqual(new[] { "2", "4", "6" }, q.GetAll("y")); // group values of same name into array
            Assert.AreEqual("3", q.FirstOrDefault("z"));
            Assert.AreEqual(null, q.FirstOrDefault("abc"));
            Assert.AreEqual(null, q.FirstOrDefault("xyz"));
            Assert.AreEqual("", q.FirstOrDefault("foo"));
            Assert.AreEqual("bar", q.FirstOrDefault(""));
        }

        [TestCase("http://www.mysite.com/more?x=1&y=2", true)]
        [TestCase("//how/about/this#hi", false)]
        [TestCase("/how/about/this#hi", false)]
        [TestCase("how/about/this#hi", false)]
        [TestCase("", false)]
        public void Url_converts_to_uri(string s, bool isAbsolute)
        {
            var url = new Flurl(s);
            var uri = url.ToUri();
            Assert.AreEqual(s, uri.OriginalString);
        }

        [Test]
        public void interprets_plus_as_space()
        {
            var url = new Flurl("http://www.mysite.com/foo+bar?x=1+2");
            Assert.AreEqual("1 2", url.QueryParams.FirstOrDefault("x"));
        }

        [Test] // #437
        public void interprets_encoded_plus_as_plus()
        {
            var urlStr = "http://google.com/search?q=param_with_%2B";
            var url = new Flurl(urlStr);
            var paramValue = url.QueryParams.FirstOrDefault("q");
            Assert.AreEqual("param_with_+", paramValue);
        }

        [Test] // #56
        public void does_not_alter_url_passed_to_constructor()
        {
            var expected = "http://www.mysite.com/hi%20there/more?x=%CD%EE%E2%FB%E9%20%E3%EE%E4";
            var url = new Flurl(expected);
            Assert.AreEqual(expected, url.ToString());
        }

        [Test] // #656
        public void queryparams_uses_equals()
        {
            var url = new Flurl("http://www.mysite.com?param=1");
            // String gets boxed, so we need to use Equals, instead of ==
            var contains = url.QueryParams.Contains("param", "1");
            Assert.IsTrue(contains);
        }
    }
    [TestFixture, Parallelizable]
    public class UrlBuildingTests
    {
        [Test]
        // check that for every Url method, we have an equivalent string extension
        public void extension_methods_consistently_supported()
        {
            var urlMethods = typeof(Flurl).GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly).Where(m => !m.IsSpecialName);
            var stringExts = ReflectionHelper.GetAllExtensionMethods<string>(typeof(Flurl).GetTypeInfo().Assembly);
            var uriExts = ReflectionHelper.GetAllExtensionMethods<Uri>(typeof(Flurl).GetTypeInfo().Assembly);
            var whitelist = new[] { "ToString", "IsValid", "ToUri", "Equals", "GetHashCode", "Clone", "Reset" }; // cases where string extension of the same name was excluded intentionally

            foreach (var method in urlMethods)
            {
                if (whitelist.Contains(method.Name)) continue;

                if (!stringExts.Any(m => ReflectionHelper.AreSameMethodSignatures(method, m)))
                {
                    var args = string.Join(", ", method.GetParameters().Select(p => p.ParameterType.Name));
                    Assert.Fail($"No equivalent string extension method found for Url.{method.Name}({args})");
                }
                if (!uriExts.Any(m => ReflectionHelper.AreSameMethodSignatures(method, m)))
                {
                    var args = string.Join(", ", method.GetParameters().Select(p => p.ParameterType.Name));
                    Assert.Fail($"No equivalent System.Uri extension method found for Url.{method.Name}({args})");
                }
            }
        }

        [Test]
        public void can_construct_from_uri()
        {
            var s = "http://www.mysite.com/more?x=1&y=2#foo";
            var uri = new Flurl(s);
            var url = new Flurl(uri);
            Assert.AreEqual(s, url.ToString());
        }

        [Test]
        public void uri_cannot_be_null()
        {
            Assert.Throws<ArgumentNullException>(() => new Flurl((Flurl)null));
        }

        [Test]
        public void can_set_query_params()
        {
            var url = "http://www.mysite.com/more"
                .SetQueryParam("x", 1)
                .SetQueryParam("y", new[] { "2", "4", "6" })
                .SetQueryParam("z", 3)
                .SetQueryParam("abc")
                .SetQueryParam("xyz")
                .SetQueryParam("foo", "")
                .SetQueryParam("", "bar");

            Assert.AreEqual("http://www.mysite.com/more?x=1&y=2&y=4&y=6&z=3&abc&xyz&foo=&=bar", url.ToString());
        }

        // #641 (oddly, passing the params object ("" or null) via another TestCase arg didn't repro the bug in the null case)
        [TestCase("http://www.mysite.com/more")]
        [TestCase("http://www.mysite.com/more?x=1")]
        public void ignores_null_or_empty_query_params(string original)
        {
            var modified1 = original.SetQueryParams("").ToString();
            Assert.AreEqual(original, modified1);
            var modified2 = original.SetQueryParams(null).ToString();
            Assert.AreEqual(original, modified2);
        }

        [Test] // #669
        public void can_set_query_params_using_objects_with_nullable_types()
        {
            int? x = 1;
            int? y = null;
            var query = new { x, y };
            var url = new Flurl("https://api.com");
            url.SetQueryParams(query);
            Assert.AreEqual("https://api.com?x=1", url.ToString());
        }

#if NET
		[Test] // #632
		public void can_set_query_params_to_enums_cast_to_ints() {
			var enumValues = new[] { FileMode.Append, FileMode.Create };
			var intValues = enumValues.Cast<int>();

			var url = "http://www.mysite.com/more".SetQueryParams(new { Filter = intValues });
			Assert.AreEqual("http://www.mysite.com/more?Filter=6&Filter=2", url.ToString());

			// This still doesn't work but is fixable with major changes that break at least one scenario:
			// https://github.com/tmenier/Flurl/issues/632#issuecomment-940507935
			// url = "http://www.mysite.com/more".SetQueryParam("Filter", intValues);
			// Assert.AreEqual("http://www.mysite.com/more?Filter=6&Filter=2", url.ToString());
		}
#endif

        [Test] // #672
        public void can_set_query_params_using_object_with_ienumerable()
        {
            var model = new ModelWithIEnumerable { Values = Enumerable.Range(1, 3).ToArray() };
            var url = "https://api.com".SetQueryParams(model);
            Assert.AreEqual("https://api.com?Values=1&Values=2&Values=3", url.ToString());
        }

        class ModelWithIEnumerable
        {
            public IEnumerable<int> Values { get; set; }
        }

        [Test] // #301
        public void setting_query_param_array_creates_multiple()
        {
            var q = "http://www.mysite.com".SetQueryParam("x", new[] { 1, 2, 3 }).QueryParams;
            Assert.AreEqual(3, q.Count);
            Assert.AreEqual(new[] { 1, 2, 3 }, q.Select(p => p.Value));
        }

        [Test]
        public void can_change_query_param()
        {
            var url = "http://www.mysite.com?x=1".SetQueryParam("x", 2);
            Assert.AreEqual("http://www.mysite.com?x=2", url.ToString());
        }

        [Test]
        public void enumerable_query_param_is_split_into_multiple()
        {
            var url = "http://www.mysite.com".SetQueryParam("x", new[] { "a", "b", null, "c" });
            Assert.AreEqual("http://www.mysite.com?x=a&x=b&x=c", url.ToString());
        }

        [Test]
        public void can_set_multiple_query_params_from_anon_object()
        {
            var url = "http://www.mysite.com".SetQueryParams(new
            {
                x = 1,
                y = 2,
                z = new[] { 3, 4 },
                exclude_me = (string)null
            });
            Assert.AreEqual("http://www.mysite.com?x=1&y=2&z=3&z=4", url.ToString());
        }

        [Test]
        public void can_replace_query_params_from_anon_object()
        {
            var url = "http://www.mysite.com?x=1&y=2&z=3".SetQueryParams(new
            {
                x = 8,
                y = new[] { "a", "b" },
                z = (int?)null
            });
            Assert.AreEqual("http://www.mysite.com?x=8&y=a&y=b", url.ToString());
        }

        [Test]
        public void can_set_query_params_from_string()
        {
            var url = "http://www.mysite.com".SetQueryParams("x=1&y=this%26that");
            Assert.AreEqual("http://www.mysite.com?x=1&y=this%26that", url.ToString());
        }

        [Test] // https://github.com/tmenier/Flurl/issues/620
        public void query_params_string_encodes_consistently_as_object()
        {
            var qs = "x=1&y=this%26that";
            var url1 = "http://www.mysite.com".SetQueryParams(qs as string);
            var url2 = "http://www.mysite.com".SetQueryParams(qs as object);
            Assert.AreEqual(url1.ToString(), url2.ToString());
        }

        [Test]
        public void can_set_query_params_from_dictionary()
        {
            // let's challenge it a little with non-string keys
            var url = "http://www.mysite.com".SetQueryParams(new Dictionary<int, string> { { 1, "x" }, { 2, "y" } });
            Assert.AreEqual("http://www.mysite.com?1=x&2=y", url.ToString());
        }

        [Test, Ignore("tricky to do while maintaining param order. deferring until append param w/o overwriting is fully supported.")]
        public void can_set_query_params_from_kv_pairs()
        {
            var url = "http://foo.com".SetQueryParams(new[] {
                new { key = "x", value = 1 },
                new { key = "y", value = 2 },
                new { key = "x", value = 3 } // this should append, not overwrite (#370)
			});

            Assert.AreEqual("http://foo.com?x=1&y=2&x=3", url.ToString());
        }

        [Test]
        public void can_set_multiple_no_value_query_params_from_enumerable_string()
        {
            IEnumerable<string> GetQueryParamNames()
            {
                yield return "abc";
                yield return "123";
                yield return null;
                yield return "456";
            }
            var url = "http://www.mysite.com".SetQueryParams(GetQueryParamNames());
            Assert.AreEqual("http://www.mysite.com?abc&123&456", url.ToString());
        }

        [Test]
        public void can_set_multiple_no_value_query_params_from_params()
        {
            var url = "http://www.mysite.com".SetQueryParams("abc", "123", null, "456");
            Assert.AreEqual("http://www.mysite.com?abc&123&456", url.ToString());
        }

        [Test]
        public void can_remove_query_param()
        {
            var url = "http://www.mysite.com/more?x=1&y=2".RemoveQueryParam("x");
            Assert.AreEqual("http://www.mysite.com/more?y=2", url.ToString());
        }

        [Test]
        public void can_remove_query_params_by_multi_args()
        {
            var url = "http://www.mysite.com/more?x=1&y=2".RemoveQueryParams("x", "y");
            Assert.AreEqual("http://www.mysite.com/more", url.ToString());
        }

        [Test]
        public void can_remove_query_params_by_enumerable()
        {
            var url = "http://www.mysite.com/more?x=1&y=2&z=3".RemoveQueryParams(new[] { "x", "z" });
            Assert.AreEqual("http://www.mysite.com/more?y=2", url.ToString());
        }

        [TestCase("http://www.mysite.com/?x=1&y=2&z=3#foo", "http://www.mysite.com/#foo")]
        [TestCase("http://www.mysite.com/more?x=1&y=2&z=3#foo", "http://www.mysite.com/more#foo")]
        [TestCase("relative/path?foo", "relative/path")]
        public void can_remove_query(string original, string expected)
        {
            var url = original.RemoveQuery();
            Assert.AreEqual(expected, url.ToString());
        }

        [Test]
        public void removing_nonexisting_query_params_is_ignored()
        {
            var url = "http://www.mysite.com/more".RemoveQueryParams("x", "y");
            Assert.AreEqual("http://www.mysite.com/more", url.ToString());
        }

        [TestCase(NullValueHandling.Ignore, ExpectedResult = "http://www.mysite.com?y=2&x=1&z=foo")]
        [TestCase(NullValueHandling.NameOnly, ExpectedResult = "http://www.mysite.com?y&x=1&z=foo")]
        [TestCase(NullValueHandling.Remove, ExpectedResult = "http://www.mysite.com?x=1&z=foo")]
        public string can_control_null_value_behavior_in_query_params(NullValueHandling nullValueHandling)
        {
            return "http://www.mysite.com?y=2".SetQueryParams(new { x = 1, y = (string)null, z = "foo" }, nullValueHandling).ToString();
        }

        [TestCase("http://www.mysite.com", "endpoint", "http://www.mysite.com/endpoint")]
        [TestCase("path1", "path2", "path1/path2")] // #568
        [TestCase("/path1/path2", "path3", "/path1/path2/path3")]
        public void can_append_path_segment(string original, string segment, string result)
        {
            Assert.AreEqual(result, original.AppendPathSegment(segment).ToString());
            Assert.AreEqual(result, original.AppendPathSegment("/" + segment).ToString());
            Assert.AreEqual(result, (original + "/").AppendPathSegment(segment).ToString());
            Assert.AreEqual(result, (original + "/").AppendPathSegment("/" + segment).ToString());
        }

        [Test]
        public void appending_null_path_segment_throws_arg_null_ex()
        {
            Assert.Throws<ArgumentNullException>(() => "http://www.mysite.com".AppendPathSegment(null));
        }

        [Test]
        public void can_append_multiple_path_segments_by_multi_args()
        {
            var url = "http://www.mysite.com".AppendPathSegments("category", "/endpoint/");
            Assert.AreEqual("http://www.mysite.com/category/endpoint/", url.ToString());
        }

        [Test]
        public void can_append_multiple_path_segments_by_enumerable()
        {
            IEnumerable<string> segments = new[] { "/category/", "endpoint" };
            var url = "http://www.mysite.com".AppendPathSegments(segments);
            Assert.AreEqual("http://www.mysite.com/category/endpoint", url.ToString());
        }

        [TestCase("http://www.site.com/path1/path2/?x=y", "http://www.site.com/path1/?x=y")]
        [TestCase("http://www.site.com/path1/path2?x=y", "http://www.site.com/path1?x=y")]
        [TestCase("http://www.site.com/path1/", "http://www.site.com/")]
        [TestCase("http://www.site.com/path1", "http://www.site.com/")]
        [TestCase("http://www.site.com/", "http://www.site.com/")]
        [TestCase("http://www.site.com", "http://www.site.com")]
        [TestCase("/path1/path2", "/path1")]
        [TestCase("path1/path2", "path1")]
        public void can_remove_path_segment(string original, string expected)
        {
            var url = original.RemovePathSegment();
            Assert.AreEqual(expected, url.ToString());
        }

        [TestCase("http://www.site.com/path1/path2/?x=y", "http://www.site.com?x=y")]
        [TestCase("http://www.site.com/path1/path2?x=y", "http://www.site.com?x=y")]
        [TestCase("http://www.site.com/", "http://www.site.com")]
        [TestCase("http://www.site.com", "http://www.site.com")]
        [TestCase("/path1/path2", "")]
        [TestCase("path1/path2", "")]
        [TestCase("news:comp.infosystems.www.servers.unix", "news:")]
        public void can_remove_path(string original, string expected)
        {
            var url = original.RemovePath();
            Assert.AreEqual(expected, url.ToString());
        }

        [Test]
        public void url_ToString_uses_invariant_culture()
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("es-ES");
            var url = "http://www.mysite.com".SetQueryParam("x", 1.1);
            Assert.AreEqual("http://www.mysite.com?x=1.1", url.ToString());
        }

        [Test]
        public void can_reset_to_root()
        {
            var url = "http://www.mysite.com/more?x=1&y=2#foo".ResetToRoot();
            Assert.AreEqual("http://www.mysite.com", url.ToString());
        }

        [Test]
        public void can_reset()
        {
            var orignal = "http://www.mysite.com/more?x=1&y=2#foo";
            var url = orignal.AppendPathSegment("path2").RemoveQueryParam("y").SetFragment("bar");
            url.Scheme = "https";
            url.Port = 1234;
            url.Host = "www.yoursite.com";
            Assert.AreEqual("https://www.yoursite.com:1234/more/path2?x=1#bar", url.ToString());
            Assert.AreEqual(orignal, url.Reset().ToString());
        }

        [Test]
        public void can_parse_path_segments()
        {
            var path = "/segment 1//segment 3/?segment#4/";
            CollectionAssert.AreEqual(new[] { "segment%201", "", "segment%203", "%3Fsegment%234" }, Flurl.ParsePathSegments(path));
        }

        [Test]
        public void can_do_crazy_long_fluent_expression()
        {
            var url = "http://www.mysite.com"
                .SetQueryParams(new { a = 1, b = 2, c = 999 })
                .SetFragment("fooey")
                .AppendPathSegment("category")
                .RemoveQueryParam("c")
                .SetQueryParam("z", 55)
                .RemoveQueryParams("a", "z")
                .SetQueryParams(new { n = "hi", m = "bye" })
                .AppendPathSegment("endpoint");

            Assert.AreEqual("http://www.mysite.com/category/endpoint?b=2&n=hi&m=bye#fooey", url.ToString());
        }

        [Test]
        public void encodes_illegal_path_chars()
        {
            // should not encode '/'
            var url = "http://www.mysite.com".AppendPathSegment("hi there/bye now");
            Assert.AreEqual("http://www.mysite.com/hi%20there/bye%20now", url.ToString());
        }

        [Test]
        public void can_encode_reserved_path_chars()
        {
            // should encode '/' (tests optional fullyEncode arg)
            var url = "http://www.mysite.com".AppendPathSegment("hi there/bye now", true);
            Assert.AreEqual("http://www.mysite.com/hi%20there%2Fbye%20now", url.ToString());
        }

        [Test]
        public void does_not_reencode_path_escape_chars()
        {
            var url = "http://www.mysite.com".AppendPathSegment("hi%20there/inside%2foutside");
            Assert.AreEqual("http://www.mysite.com/hi%20there/inside%2foutside", url.ToString());
        }

        [Test]
        public void encodes_query_params()
        {
            var url = "http://www.mysite.com".SetQueryParams(new { x = "$50", y = "2+2=4" });
            Assert.AreEqual("http://www.mysite.com?x=%2450&y=2%2B2%3D4", url.ToString());
        }

        [Test] // #582
        public void encodes_date_type_query_param()
        {
            var date = new DateTime(2020, 12, 6, 10, 45, 1);
            var url = "http://www.mysite.com".SetQueryParam("date", date);
            Assert.AreEqual("http://www.mysite.com?date=2020-12-06T10%3A45%3A01.0000000", url.ToString());
        }

        [Test]
        public void does_not_reencode_encoded_query_values()
        {
            var url = "http://www.mysite.com".SetQueryParam("x", "%CD%EE%E2%FB%E9%20%E3%EE%E4", true);
            Assert.AreEqual("http://www.mysite.com?x=%CD%EE%E2%FB%E9%20%E3%EE%E4", url.ToString());
        }

        [Test]
        public void reencodes_encoded_query_values_when_isEncoded_false()
        {
            var url = "http://www.mysite.com".SetQueryParam("x", "%CD%EE%E2%FB%E9%20%E3%EE%E4", false);
            Assert.AreEqual("http://www.mysite.com?x=%25CD%25EE%25E2%25FB%25E9%2520%25E3%25EE%25E4", url.ToString());
        }

        [Test]
        public void does_not_encode_reserved_chars_in_query_param_name()
        {
            var url = "http://www.mysite.com".SetQueryParam("$x", 1);
            Assert.AreEqual("http://www.mysite.com?$x=1", url.ToString());
        }

        [Test]
        public void Url_implicitly_converts_to_string()
        {
            var url = new Flurl("http://www.mysite.com/more?x=1&y=2");
            var someMethodThatTakesAString = new Action<string>(s => { });
            someMethodThatTakesAString(url); // if this compiles, test passed.
        }

        [Test]
        public void encodes_plus()
        {
            var url = new Flurl("http://www.mysite.com").SetQueryParam("x", "1+2");
            Assert.AreEqual("http://www.mysite.com?x=1%2B2", url.ToString());
        }

        [Test]
        public void can_encode_space_as_plus()
        {
            var url = new Flurl("http://www.mysite.com").AppendPathSegment("a b").SetQueryParam("c d", "1 2");
            Assert.AreEqual("http://www.mysite.com/a%20b?c%20d=1%202", url.ToString()); // but not by default
            Assert.AreEqual("http://www.mysite.com/a+b?c+d=1+2", url.ToString(true));
        }

        [Test] // #29
        public void can_add_and_remove_fragment_fluently()
        {
            var url = "http://www.mysite.com".SetFragment("foo");
            Assert.AreEqual("http://www.mysite.com#foo", url.ToString());
            url = "http://www.mysite.com#foo".RemoveFragment();
            Assert.AreEqual("http://www.mysite.com", url.ToString());
            url = "http://www.mysite.com"
                .SetFragment("foo")
                .SetFragment("bar")
                .AppendPathSegment("more")
                .SetQueryParam("x", 1);
            Assert.AreEqual("http://www.mysite.com/more?x=1#bar", url.ToString());
            url = "http://www.mysite.com".SetFragment("foo").SetFragment("bar").RemoveFragment();
            Assert.AreEqual("http://www.mysite.com", url.ToString());
        }

        [Test]
        public void has_fragment_after_SetQueryParam()
        {
            var expected = "http://www.mysite.com/more?x=1#first";
            var url = new Flurl(expected)
                .SetQueryParam("x", 3)
                .SetQueryParam("y", 4);
            Assert.AreEqual("http://www.mysite.com/more?x=3&y=4#first", url.ToString());
        }

        [Test]
        public void Equals_returns_true_for_same_values()
        {
            var url1 = new Flurl("http://mysite.com/hello");
            var url2 = new Flurl("http://mysite.com").AppendPathSegment("hello");
            var url3 = new Flurl("http://mysite.com/hello/"); // trailing slash - not equal

            Assert.IsTrue(url1.Equals(url2));
            Assert.IsTrue(url2.Equals(url1));
            Assert.AreEqual(url1.GetHashCode(), url2.GetHashCode());

            Assert.IsFalse(url1.Equals(url3));
            Assert.IsFalse(url3.Equals(url1));
            Assert.AreNotEqual(url1.GetHashCode(), url3.GetHashCode());

            Assert.IsFalse(url1.Equals("http://mysite.com/hello"));
            Assert.IsFalse(url1.Equals(null));
        }

        [Test]
        public void equality_operator_always_false_for_different_instances()
        {
            var url1 = new Flurl("http://mysite.com/hello");
            var url2 = new Flurl("http://mysite.com/hello");
            Assert.IsFalse(url1 == url2);
        }

        [Test]
        public void clone_creates_copy()
        {
            var url1 = new Flurl("http://mysite.com").SetQueryParam("x", 1);
            var url2 = url1.Clone().AppendPathSegment("foo").SetQueryParam("y", 2);
            url1.SetQueryParam("z", 3);

            Assert.AreEqual("http://mysite.com?x=1&z=3", url1.ToString());
            Assert.AreEqual("http://mysite.com/foo?x=1&y=2", url2.ToString());
        }

        #region writable properties
        [Test]
        public void can_write_scheme()
        {
            var url = new Flurl("https://api.com/foo");
            url.Scheme = "ftp";
            Assert.AreEqual("ftp://api.com/foo", url.ToString());
        }

        [Test]
        public void can_write_user_info()
        {
            var url = new Flurl("https://api.com/foo");
            url.UserInfo = "user:pass";
            Assert.AreEqual("https://user:pass@api.com/foo", url.ToString());

            // null or empty should remove it
            url.UserInfo = null;
            Assert.AreEqual("https://api.com/foo", url.ToString());
            url.UserInfo = "";
            Assert.AreEqual("https://api.com/foo", url.ToString());
        }

        [Test]
        public void can_write_host()
        {
            var url = new Flurl("https://api.com/foo");
            url.Host = "www.othersite.net";
            Assert.AreEqual("https://www.othersite.net/foo", url.ToString());

            // don't squash userinfo/port
            url = new Flurl("https://user:pass@api.com:8080/foo");
            url.Host = "www.othersite.net";
            Assert.AreEqual("https://user:pass@www.othersite.net:8080/foo", url.ToString());
        }

        [Test]
        public void can_write_port()
        {
            var url = new Flurl("https://api.com/foo");
            url.Port = 1234;
            Assert.AreEqual("https://api.com:1234/foo", url.ToString());

            // null should remove it
            url.Port = null;
            Assert.AreEqual("https://api.com/foo", url.ToString());
        }

        [Test]
        public void can_write_path()
        {
            var url = new Flurl("https://api.com/foo");
            url.Path = "/a/b/c/";
            Assert.AreEqual("https://api.com/a/b/c/", url.ToString());
            url.Path = "a/b/c";
            Assert.AreEqual("https://api.com/a/b/c", url.ToString());
            url.Path = "/";
            Assert.AreEqual("https://api.com/", url.ToString());

            // null or empty should remove it (including leading slash)
            url.Path = null;
            Assert.AreEqual("https://api.com", url.ToString());
            url.Path = "";
            Assert.AreEqual("https://api.com", url.ToString());
        }

        [Test]
        public void can_manipulate_path_segments()
        {
            var url = new Flurl("https://api.com/foo?x=0");
            url.PathSegments.Add("bar");
            Assert.AreEqual("https://api.com/foo/bar?x=0", url.ToString());
            // when manipulating PathSegments directly, there's no double-slash check like with AppendPathSegment
            url.PathSegments.Add("/hi/there/");
            Assert.AreEqual("https://api.com/foo/bar//hi/there/?x=0", url.ToString());
            url.PathSegments.RemoveAt(1);
            Assert.AreEqual("https://api.com/foo//hi/there/?x=0", url.ToString());
            url.PathSegments.RemoveAt(1);
            Assert.AreEqual("https://api.com/foo?x=0", url.ToString());
            url.PathSegments.Clear();
            Assert.AreEqual("https://api.com/?x=0", url.ToString());
        }

        [Test]
        public void can_write_query()
        {
            var url = new Flurl("https://api.com/foo?x=0#bar");
            url.Query = "y=1&z=2";
            Assert.AreEqual("https://api.com/foo?y=1&z=2#bar", url.ToString());

            // null or empty should remove
            url.Query = null;
            Assert.AreEqual("https://api.com/foo#bar", url.ToString());
            url.Query = "";
            Assert.AreEqual("https://api.com/foo#bar", url.ToString());
        }

        [Test]
        public void can_manipulate_query_params()
        {
            var url = new Flurl("https://api.com/foo#bar");
            url.QueryParams.Add("x", 0);
            Assert.AreEqual("https://api.com/foo?x=0#bar", url.ToString());
            url.QueryParams.Add("y", 1);
            Assert.AreEqual("https://api.com/foo?x=0&y=1#bar", url.ToString());
            url.QueryParams.AddOrReplace("x", "hi!");
            Assert.AreEqual("https://api.com/foo?x=hi%21&y=1#bar", url.ToString());
            url.QueryParams.AddOrReplace("z", new[] { 8, 9, 10 });
            Assert.AreEqual("https://api.com/foo?x=hi%21&y=1&z=8&z=9&z=10#bar", url.ToString());
            url.QueryParams.Remove("y");
            Assert.AreEqual("https://api.com/foo?x=hi%21&z=8&z=9&z=10#bar", url.ToString());
            url.QueryParams.AddOrReplace("x", null);
            Assert.AreEqual("https://api.com/foo?z=8&z=9&z=10#bar", url.ToString());
            url.QueryParams.Clear();
            Assert.AreEqual("https://api.com/foo#bar", url.ToString());
        }

        [Test]
        public void can_modify_query_param_array()
        {
            var url = new Flurl("http://www.mysite.com/more?x=1&y=2&x=2&z=4");
            // go from 2 values to 3, order should be preserved
            url.QueryParams.AddOrReplace("x", new[] { 8, 9, 10 });
            Assert.AreEqual("http://www.mysite.com/more?x=8&y=2&x=9&z=4&x=10", url.ToString());
            // go from 3 values to 2, order should be preserved
            url.QueryParams.AddOrReplace("x", new[] { 101, 102 });
            Assert.AreEqual("http://www.mysite.com/more?x=101&y=2&x=102&z=4", url.ToString());
            // wipe them out. all of them.
            url.QueryParams.AddOrReplace("x", null);
            Assert.AreEqual("http://www.mysite.com/more?y=2&z=4", url.ToString());
        }

        [Test]
        public void can_write_fragment()
        {
            var url = new Flurl("https://api.com/");
            url.Fragment = "hello";
            Assert.AreEqual("https://api.com/#hello", url.ToString());
            url.Fragment = "#goodbye"; // # isn't technically part of the fragment, so this should give us 2 of them
            Assert.AreEqual("https://api.com/##goodbye", url.ToString());

            // null or empty should remove
            url.Fragment = null;
            Assert.AreEqual("https://api.com/", url.ToString());
            url.Fragment = "";
            Assert.AreEqual("https://api.com/", url.ToString());
        }
        #endregion

        [Test]
        public void can_build_url_from_uri()
        {
            // shouldn't need to test System.Uri extensions exhaustively. they're auto-generated, and we already
            // verified the set of them is uniform and complete as compared to the string extensions
            // (see extension_methods_consistently_supported). just check a couple.

            var url = new Flurl("http://mysite.com").AppendPathSegment("hello");
            Assert.IsInstanceOf<Flurl>(url);
            Assert.AreEqual("http://mysite.com/hello", url.ToString());

            url = new Uri("http://mysite.com/hello").ResetToRoot();
            Assert.IsInstanceOf<Flurl>(url);
            Assert.AreEqual("http://mysite.com", url.ToString());
        }

        [Test] // https://github.com/tmenier/Flurl/issues/510
        public void uri_with_default_port_parses_correctly()
        {
            var originalString = "https://someurl.net:443/api/somepath";
            var uri = new Flurl(originalString);
            var url = new Flurl(uri);
            Assert.AreEqual(443, url.Port);
            Assert.AreEqual(originalString, url.ToString());
        }

        [Test]
        public void can_have_empty_ctor()
        {
            var url1 = new Flurl();
            Assert.AreEqual("", url1.ToString());

            var url2 = new Flurl
            {
                Host = "192.168.1.1",
                Scheme = "http"
            };
            Assert.AreEqual("http://192.168.1.1", url2.ToString());
        }

        [Test]
        public void can_append_trailing_slash()
        {
            var url = new Flurl("https://www.site.com/a/b/c");
            Assert.AreEqual("https://www.site.com/a/b/c", url.ToString());
            url.AppendPathSegment("/");
            Assert.AreEqual("https://www.site.com/a/b/c/", url.ToString());
            url.AppendPathSegment("///");
            Assert.AreEqual("https://www.site.com/a/b/c///", url.ToString());
        }

        [Test]
        public void url_trims_leading_and_trailing_whitespace()
        {
            var url = new Flurl("  https://www.site.com \t");
            Assert.AreEqual("https://www.site.com", url.ToString());
        }
    }
}
