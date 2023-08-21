# sample-dotnet-backend
> SAML service provider demonstrate in .NET

* Downoload .NET [here](https://dotnet.microsoft.com/download/dotnet/5.0)
* In this sample project, I use **ITfoxtec Identity SAML2**

## Commands
```bash
dotnet new sln --name SampleSP.NET
dotnet new web --name SampleSP.NET.Web
dotnet new gitignore
dotnet sln add SampleSP.NET.Web
```

``` bash
dotnet add package ITfoxtec.Identity.Saml2 --version 4.7.0
dotnet add package ITfoxtec.Identity.Saml2.MvcCore --version 4.7.0
```

## Reference
* ITfoxtec SAML 2.0: https://www.itfoxtec.com/IdentitySaml2

## Things need to do before coding 
* 安裝 OpenSSL prompt : [教學文](https://www.cjkuo.net/window_install_openssl/)
* 打開OpenSSL prompt ，依照下方的語法去製作公私鑰和證書
  
  > 生成RSA私鑰
  ```bash
  openssl genpkey -algorithm RSA -out private_key.pem
  ```  

  > 從私鑰導出公鑰
  ```bash
  openssl rsa -pubout -in private_key.pem -out public_key.pem
  ```  

  > 使用私鑰生成自簽名證書 
  ```bash
  openssl req -new -x509 -key private_key.pem -out selfsigned_cert.pem -days 365
  ```  
* 現在，您有三個文件：
   - private_key.pem : 用於簽名和解密的私鑰。
   - public_key.pem : 公開的公鑰，可用於驗證簽名和加密。
   - selfsigned_cert.pem : 包含您的公鑰的自簽名證書。
   - 將 selfsigned_cert.pem 證書提供給您的IdP，以便它可以驗證您使用 private_key.pem 私鑰所做的簽名。請確保您妥善保管 private_key.pem，並且不要與任何人分享它。
 
## Identity Provider
* 為了實作SAML的 SSO和SLO，必須有個Identity Provider，但因為自行開發Identity Provider的難度有點高，筆者無法自己實作，所以這邊使用第三方平台 [OKTA](https://developer.okta.com/signup/?_ga=2.164352123.559729886.1629730675-1313411396.1629730675)  
  
> 其他的Idp平台也可以做到跟OKTA同樣的功能，出於方便和較多討論度的關係，所以這邊使用OKTA做示範，下面會提到該提供的訊息基本上其他Idp也會要求我們提供。  

### 1. 先申請OKTA帳戶和完成帳戶驗證後登入，點選左邊功能列的 Applications > Applications > Create App Integration
   ![image](https://miro.medium.com/v2/resize:fit:1400/0*SsFHqDYhxpECfwCc)
   
### 1. 選擇 SAML2.0，點選 Next
   ![image](https://miro.medium.com/v2/resize:fit:1400/0*DVvx5O3nMBChrbOJ)  
   
### 1. App name 為你應用程式的名字，可自行命名，縮圖則可不上傳  
   ![image](https://miro.medium.com/v2/resize:fit:1400/0*x9jBOnVddmnKf6tl)
  
### 1. 提供關於你 Service Provider 的 Information，這邊先設定SSO  
  ![image](https://miro.medium.com/v2/resize:fit:1400/0*FRoCzbtdyBYrxumi)

### 1. 往下拉一點會看到 Show Advanced Settings ，點開後可以開始設定證書和SLO  
   ![image](https://miro.medium.com/v2/resize:fit:1400/0*St0lsgWbqmdYRv_d)  
   
### 1. 成功上傳後就可以勾選 Enable Single Logout，就可以設定SLO。都設定好後就可以點選最下方的Next  
   ![image](https://i.imgur.com/Nk7g4Ta.png)  
   
### > 如果需要回傳其他資料可以在這邊這設定，這邊以displayName當作回傳參數回傳 user.displayName 給我們的endpoint  
    ![image](https://i.imgur.com/dR8yBT9.png)  

### 1. 點選 I'm a software vendor. I'd like to integrate my app with Okta，完成後點選Finish  
    ![image](https://i.imgur.com/4EnacBi.png)  

### 1. 完成後，點選右邊的 View SAML setup instructions  
    ![image](https://i.imgur.com/tENLJ0w.png)  

### 1. 裡面會提供需鑰填寫到我們appsetting.json裏頭的資訊，依序填入到SingleSignOnDestination、SingleLogoutDestination、AllowedIssuer、IdPCertificate  
    ![image](https://i.imgur.com/fRXMLm5.png)
