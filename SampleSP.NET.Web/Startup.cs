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
            // �q appsettings.json �[�� SAML 2.0 �t�m
            services.Configure<Saml2Configuration>(Configuration.GetSection("Saml2"));
            // �K�[���\������ URI �M���J IdP ������
            services.Configure<Saml2Configuration>((config) => {
                config.AllowedAudienceUris.Add(config.Issuer);
                // �q�t�m�����J IdP ������
                X509Certificate2 idpCertificate = LoadCertificate(Configuration["Saml2:IdPCertificate"]);
                config.SignatureValidationCertificates.Add(idpCertificate);
                // �O�o�b��Ƨ������W�ҮѩM�p�_
                config.SigningCertificate = CertificateUtil.Load("your-Certificate-name.pfx", "your-Certificate-password");
            });
            // �]�w SAML 2.0 ���A�ȩM����
            services.AddSaml2(
                loginPath: "/auth/Login",
                slidingExpiration: true
            );
            services.AddControllersWithViews();
        }

        // ���J Base64 �s�X������
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

            // �ҥ� SAML 2.0 �{�Ҥ�����
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
