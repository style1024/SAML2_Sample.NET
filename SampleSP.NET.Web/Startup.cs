using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.MvcCore.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Pkcs;
using ITfoxtec.Identity.Saml2.Util;

namespace SampleSP.NET.Web
{
    public class Startup
    {
        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment env;
        public Startup(IConfiguration configuration, IWebHostEnvironment hostEnvironment)
        {
            Configuration = configuration;
            env = hostEnvironment;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // 從 appsettings.json 加載 SAML 2.0 配置
            services.Configure<Saml2Configuration>(Configuration.GetSection("Saml2"));
            // 添加允許的受眾 URI 和載入 IdP 的憑證
            services.Configure<Saml2Configuration>((config) => {
                config.AllowedAudienceUris.Add(config.Issuer);
                // 從配置中載入 IdP 的憑證
                X509Certificate2 idpCertificate = LoadCertificate(Configuration["Saml2:IdPCertificate"]);
                config.SignatureValidationCertificates.Add(idpCertificate);
                // 記得在資料夾內附上證書和私鑰
                config.SigningCertificate = CertificateUtil.Load("your-Certificate-name.pfx", "your-Certificate-password");
            });
            // 設定 SAML 2.0 的服務和路由
            services.AddSaml2(
                loginPath: "/auth/Login",
                slidingExpiration: true
            );
            services.AddControllersWithViews();
        }

        // 載入 Base64 編碼的憑證
        public X509Certificate2 LoadCertificate(string certificate)
        {
            if (string.IsNullOrWhiteSpace(certificate))
            {
                throw new ArgumentNullException(nameof(certificate));
            }

            return new X509Certificate2(Convert.FromBase64String(certificate));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // 啟用 SAML 2.0 認證中間件
            app.UseSaml2();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
